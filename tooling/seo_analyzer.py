#!/usr/bin/env python3
"""
SEO Analyzer for Markdown Content

Analyzes README.md files for SEO metrics:
- Keyword density (primary keyword derived from folder name)
- H1/H2 headers containing target keywords
- IronBarcode links (ironsoftware.com/csharp/barcode, NuGet)
- Internal cross-links to competitor folders
- External authority links

Usage:
    python tooling/seo_analyzer.py [--verbose] [--min-score N]
"""

import os
import re
import sys
import argparse
from pathlib import Path
from typing import Dict, List, Tuple, Optional
from collections import defaultdict


# IronBarcode domains to track
IRONBARCODE_DOMAINS = [
    'ironsoftware.com/csharp/barcode',
    'nuget.org/packages/IronBarcode',
    'github.com/iron-software',
]


class SEOAnalyzer:
    """Analyzes markdown files for SEO metrics."""

    def __init__(self, repo_root: Path, verbose: bool = False, min_score: int = 60):
        self.repo_root = repo_root
        self.verbose = verbose
        self.min_score = min_score
        self.excluded_dirs = {'.planning', 'tooling', '.git', 'node_modules', '__pycache__'}
        self.results: List[Dict] = []

    def find_readme_files(self) -> List[Tuple[Path, str]]:
        """Find all README.md files with their folder context."""
        readme_files = []

        for root, dirs, files in os.walk(self.repo_root):
            dirs[:] = [d for d in dirs if d not in self.excluded_dirs]

            if 'README.md' in files:
                folder_path = Path(root)
                folder_name = folder_path.name

                # Skip root README
                if folder_path == self.repo_root:
                    continue

                # Derive primary keyword from folder name
                keyword = self._derive_keyword(folder_name)
                readme_files.append((folder_path / 'README.md', keyword))

        return readme_files

    def _derive_keyword(self, folder_name: str) -> str:
        """Derive primary keyword from folder name."""
        # Convert hyphens to spaces, handle special cases
        keyword = folder_name.replace('-', ' ')

        # Special handling for use-cases
        if 'use-cases' in folder_name or 'use_cases' in folder_name:
            keyword = keyword.replace('dotnet barcode use cases', '').strip()
            if not keyword:
                keyword = 'barcode'

        return keyword.strip()

    def analyze_file(self, readme_path: Path, keyword: str) -> Dict:
        """Analyze a single README file for SEO metrics."""
        try:
            content = readme_path.read_text(encoding='utf-8')
        except Exception as e:
            return {
                'file': str(readme_path),
                'keyword': keyword,
                'error': str(e),
                'score': 0
            }

        # Calculate metrics
        word_count = len(content.split())
        keyword_count = len(re.findall(re.escape(keyword), content, re.IGNORECASE))
        keyword_density = (keyword_count / word_count * 100) if word_count > 0 else 0

        h1_count = len(re.findall(r'^# .+', content, re.MULTILINE))
        h2_count = len(re.findall(r'^## .+', content, re.MULTILINE))

        h1_with_keyword = len(re.findall(rf'^# .*{re.escape(keyword)}.*', content, re.MULTILINE | re.IGNORECASE))
        h2_with_keyword = len(re.findall(rf'^## .*{re.escape(keyword)}.*', content, re.MULTILINE | re.IGNORECASE))

        ironbarcode_links = self._count_ironbarcode_links(content)
        internal_links = self._count_internal_links(content, readme_path)
        external_links = self._count_external_links(content)

        # Calculate score (0-100)
        score = self._calculate_score(
            keyword_density,
            h1_with_keyword,
            h2_with_keyword,
            ironbarcode_links,
            internal_links,
            external_links,
            word_count
        )

        return {
            'file': str(readme_path.relative_to(self.repo_root)),
            'keyword': keyword,
            'word_count': word_count,
            'keyword_density': round(keyword_density, 2),
            'h1_count': h1_count,
            'h2_count': h2_count,
            'h1_with_keyword': h1_with_keyword,
            'h2_with_keyword': h2_with_keyword,
            'ironbarcode_links': ironbarcode_links,
            'internal_links': internal_links,
            'external_links': external_links,
            'score': score
        }

    def _count_ironbarcode_links(self, content: str) -> int:
        """Count links to IronBarcode domains."""
        count = 0
        for domain in IRONBARCODE_DOMAINS:
            count += len(re.findall(re.escape(domain), content, re.IGNORECASE))
        return count

    def _count_internal_links(self, content: str, current_file: Path) -> int:
        """Count internal cross-links to other competitor folders."""
        # Match markdown links: [text](path)
        pattern = r'\[([^\]]+)\]\(([^)]+)\)'
        count = 0

        for match in re.finditer(pattern, content):
            url = match.group(2)
            # Count relative links to other folders (not http/https)
            if not url.startswith(('http://', 'https://', 'mailto:', '#')):
                # Check if it's a cross-link (not same folder)
                if '../' in url or url.startswith('/'):
                    count += 1

        return count

    def _count_external_links(self, content: str) -> int:
        """Count external authority links (excluding IronBarcode domains)."""
        pattern = r'\[([^\]]+)\]\((https?://[^)]+)\)'
        count = 0

        for match in re.finditer(pattern, content):
            url = match.group(2)
            # Exclude IronBarcode domains
            is_ironbarcode = any(domain in url for domain in IRONBARCODE_DOMAINS)
            if not is_ironbarcode:
                count += 1

        return count

    def _calculate_score(
        self,
        keyword_density: float,
        h1_with_keyword: int,
        h2_with_keyword: int,
        ironbarcode_links: int,
        internal_links: int,
        external_links: int,
        word_count: int
    ) -> int:
        """Calculate SEO score (0-100)."""
        score = 0

        # Keyword density (20 points) - optimal range 1-3%
        if 1.0 <= keyword_density <= 3.0:
            score += 20
        elif 0.5 <= keyword_density < 1.0 or 3.0 < keyword_density <= 4.0:
            score += 10
        elif keyword_density > 0:
            score += 5

        # H1 with keyword (15 points)
        if h1_with_keyword >= 1:
            score += 15

        # H2 with keyword (15 points)
        if h2_with_keyword >= 2:
            score += 15
        elif h2_with_keyword == 1:
            score += 8

        # IronBarcode links (20 points)
        if ironbarcode_links >= 3:
            score += 20
        elif ironbarcode_links >= 2:
            score += 15
        elif ironbarcode_links >= 1:
            score += 10

        # Internal cross-links (15 points)
        if internal_links >= 3:
            score += 15
        elif internal_links >= 2:
            score += 10
        elif internal_links >= 1:
            score += 5

        # External authority links (10 points)
        if external_links >= 2:
            score += 10
        elif external_links >= 1:
            score += 5

        # Word count (5 points) - prefer 800+
        if word_count >= 800:
            score += 5
        elif word_count >= 500:
            score += 3

        return min(score, 100)

    def analyze_all(self) -> None:
        """Analyze all README files in repository."""
        readme_files = self.find_readme_files()

        if not readme_files:
            print("No README.md files found to analyze.")
            return

        print(f"Analyzing {len(readme_files)} README.md file(s)...\n")

        for readme_path, keyword in readme_files:
            result = self.analyze_file(readme_path, keyword)
            self.results.append(result)

            if self.verbose or result['score'] < self.min_score:
                self._print_result(result)

    def _print_result(self, result: Dict) -> None:
        """Print analysis result for a file."""
        if 'error' in result:
            print(f"✗ {result['file']}: Error - {result['error']}\n")
            return

        score = result['score']
        status = '✓' if score >= self.min_score else '✗'

        print(f"{status} {result['file']} (Score: {score}/100)")
        print(f"   Keyword: '{result['keyword']}'")
        print(f"   Word count: {result['word_count']}")
        print(f"   Keyword density: {result['keyword_density']}%")
        print(f"   H1 with keyword: {result['h1_with_keyword']}/{result['h1_count']}")
        print(f"   H2 with keyword: {result['h2_with_keyword']}/{result['h2_count']}")

        if result['ironbarcode_links'] > 0:
            print(f"   IronBarcode links: {result['ironbarcode_links']}")
        else:
            print(f"   IronBarcode links: {result['ironbarcode_links']} ⚠ NO IronBarcode links")

        print(f"   Internal cross-links: {result['internal_links']}")
        print(f"   External authority links: {result['external_links']}")
        print()

    def report_summary(self) -> None:
        """Print summary report."""
        if not self.results:
            return

        total = len(self.results)
        passed = sum(1 for r in self.results if r['score'] >= self.min_score)
        failed = total - passed

        avg_score = sum(r['score'] for r in self.results) / total

        print("=" * 60)
        print("SEO ANALYSIS SUMMARY")
        print("=" * 60)
        print(f"Total files analyzed: {total}")
        print(f"Passed (>= {self.min_score}): {passed}")
        print(f"Failed (< {self.min_score}): {failed}")
        print(f"Average score: {avg_score:.1f}/100")
        print()

        if failed > 0:
            print("Files needing improvement:")
            for result in sorted(self.results, key=lambda r: r['score']):
                if result['score'] < self.min_score:
                    print(f"  {result['file']}: {result['score']}/100")
            sys.exit(1)
        else:
            print("✓ All files meet minimum SEO standards!")
            sys.exit(0)


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(
        description='Analyze README.md files for SEO metrics'
    )
    parser.add_argument(
        '--verbose', '-v',
        action='store_true',
        help='Show all results, not just failures'
    )
    parser.add_argument(
        '--min-score', '-m',
        type=int,
        default=60,
        help='Minimum acceptable SEO score (default: 60)'
    )

    args = parser.parse_args()

    repo_root = Path(__file__).parent.parent
    analyzer = SEOAnalyzer(repo_root, verbose=args.verbose, min_score=args.min_score)

    analyzer.analyze_all()
    analyzer.report_summary()


if __name__ == '__main__':
    main()
