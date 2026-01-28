#!/usr/bin/env python3
"""
Internal Link Validator for Markdown Files

Scans all .md files in repository and validates that internal markdown links
resolve to existing files.

Excludes: .planning/ and tooling/ directories

Exit codes:
    0: All links valid
    1: Broken links found

Usage:
    python tooling/validate_links.py
"""

import os
import re
import sys
from pathlib import Path
from typing import List, Tuple, Set


class LinkValidator:
    """Validates internal markdown links in repository."""

    def __init__(self, repo_root: Path):
        self.repo_root = repo_root
        self.excluded_dirs = {'.planning', 'tooling', '.git', 'node_modules', '__pycache__'}
        self.broken_links: List[Tuple[str, str, str]] = []

    def find_markdown_files(self) -> List[Path]:
        """Find all markdown files, excluding specific directories."""
        md_files = []
        for root, dirs, files in os.walk(self.repo_root):
            # Remove excluded directories from search
            dirs[:] = [d for d in dirs if d not in self.excluded_dirs]

            for file in files:
                if file.endswith('.md'):
                    md_files.append(Path(root) / file)

        return md_files

    def extract_links(self, content: str) -> Set[str]:
        """Extract internal markdown links from content."""
        # Match markdown links: [text](path)
        # Exclude external links (http://, https://, mailto:, etc.)
        pattern = r'\[([^\]]+)\]\(([^)]+)\)'
        links = set()

        for match in re.finditer(pattern, content):
            url = match.group(2)
            # Skip external URLs, anchors, and mailto links
            if not url.startswith(('http://', 'https://', 'mailto:', '#', 'ftp://')):
                links.add(url)

        return links

    def resolve_link(self, source_file: Path, link: str) -> bool:
        """Check if a link resolves to an existing file."""
        # Remove anchor fragments
        link_path = link.split('#')[0]
        if not link_path:  # Pure anchor link
            return True

        # Resolve relative to source file's directory
        source_dir = source_file.parent
        target = source_dir / link_path

        # Also try resolving from repo root
        if not target.exists():
            target = self.repo_root / link_path

        return target.exists()

    def validate_file(self, md_file: Path) -> int:
        """Validate all links in a markdown file."""
        broken_count = 0

        try:
            content = md_file.read_text(encoding='utf-8')
        except Exception as e:
            print(f"Error reading {md_file}: {e}", file=sys.stderr)
            return 0

        links = self.extract_links(content)

        for link in links:
            if not self.resolve_link(md_file, link):
                rel_path = md_file.relative_to(self.repo_root)
                self.broken_links.append((str(rel_path), link, str(md_file)))
                broken_count += 1

        return broken_count

    def validate_all(self) -> int:
        """Validate all markdown files in repository."""
        md_files = self.find_markdown_files()

        if not md_files:
            print("No markdown files found to validate.")
            return 0

        print(f"Validating {len(md_files)} markdown files...")

        total_broken = 0
        for md_file in md_files:
            broken = self.validate_file(md_file)
            total_broken += broken

        return total_broken

    def report(self) -> None:
        """Print validation report."""
        if not self.broken_links:
            print("✓ All internal links valid!")
            return

        print(f"\n✗ Found {len(self.broken_links)} broken link(s):\n")

        for source, link, _ in self.broken_links:
            print(f"  {source}")
            print(f"    → {link}")
            print()


def main():
    """Main entry point."""
    repo_root = Path(__file__).parent.parent
    validator = LinkValidator(repo_root)

    broken_count = validator.validate_all()
    validator.report()

    sys.exit(1 if broken_count > 0 else 0)


if __name__ == '__main__':
    main()
