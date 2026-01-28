"""
SERP Analyzer for Barcode Library Comparisons
=============================================
Analyzes search results to determine content strategy for barcode comparisons.

Uses DataForSEO API to:
- Get top-ranking pages for barcode comparison keywords
- Analyze competitor content word counts
- Determine optimal keyword density targets
- Identify content gaps to exploit

Example keyword patterns for barcode:
- "[library] vs ironbarcode"
- "[library] alternative c#"
- "best .net barcode library"
- "[library] review"
"""

import re
import requests
from typing import Dict, List, Any, Optional
from collections import Counter
from bs4 import BeautifulSoup
from dataforseo_client import DataForSEOClient


class SERPAnalyzer:
    """Analyzes SERP results to determine content strategy."""

    def __init__(self, client: Optional[DataForSEOClient] = None):
        """
        Initialize SERP analyzer.

        Args:
            client: DataForSEO client (creates new one if not provided)
        """
        self.client = client or DataForSEOClient()

    def analyze_keyword(
        self,
        keyword: str,
        top_n: int = 5,
        fetch_content: bool = True
    ) -> Dict[str, Any]:
        """
        Analyze SERP for a keyword.

        Args:
            keyword: Search keyword to analyze
            top_n: Number of top results to analyze
            fetch_content: Whether to fetch and analyze page content

        Returns:
            Analysis results including word counts, keywords, and recommendations
        """
        print(f"Analyzing SERP for: {keyword}")

        # Get SERP results
        serp = self.client.get_serp_results(keyword, depth=top_n)

        if not serp.get('tasks') or not serp['tasks'][0].get('result'):
            return {"error": "No SERP results found"}

        items = serp['tasks'][0]['result'][0].get('items', [])

        if not items:
            return {"error": "No SERP items found"}

        # Analyze each result
        results = []
        for item in items[:top_n]:
            url = item.get('url')
            title = item.get('title', '')
            position = item.get('rank_absolute', 0)

            result = {
                'url': url,
                'title': title,
                'position': position,
                'word_count': None,
                'keywords': []
            }

            if fetch_content and url:
                try:
                    content_analysis = self._analyze_page_content(url)
                    result['word_count'] = content_analysis['word_count']
                    result['keywords'] = content_analysis['top_keywords']
                except Exception as e:
                    print(f"  Warning: Could not analyze {url}: {e}")

            results.append(result)

        # Calculate recommendations
        recommendations = self._calculate_recommendations(results)

        return {
            'keyword': keyword,
            'results': results,
            'recommendations': recommendations
        }

    def _analyze_page_content(self, url: str) -> Dict[str, Any]:
        """
        Fetch and analyze page content.

        Args:
            url: URL to analyze

        Returns:
            Content analysis with word count and keywords
        """
        # Fetch page
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
        }
        response = requests.get(url, headers=headers, timeout=10)
        response.raise_for_status()

        # Parse HTML
        soup = BeautifulSoup(response.text, 'html.parser')

        # Remove script and style elements
        for script in soup(['script', 'style', 'nav', 'header', 'footer']):
            script.decompose()

        # Get text
        text = soup.get_text()

        # Clean text
        lines = (line.strip() for line in text.splitlines())
        chunks = (phrase.strip() for line in lines for phrase in line.split("  "))
        text = ' '.join(chunk for chunk in chunks if chunk)

        # Count words
        words = re.findall(r'\b[a-zA-Z]+\b', text.lower())
        word_count = len(words)

        # Get top keywords (excluding common words)
        stop_words = {
            'the', 'a', 'an', 'and', 'or', 'but', 'in', 'on', 'at', 'to', 'for',
            'of', 'with', 'by', 'from', 'as', 'is', 'was', 'are', 'were', 'been',
            'be', 'have', 'has', 'had', 'do', 'does', 'did', 'will', 'would',
            'could', 'should', 'may', 'might', 'can', 'this', 'that', 'these',
            'those', 'it', 'its', 'you', 'your', 'we', 'our', 'they', 'their'
        }

        filtered_words = [w for w in words if w not in stop_words and len(w) > 3]
        word_freq = Counter(filtered_words)
        top_keywords = word_freq.most_common(10)

        return {
            'word_count': word_count,
            'top_keywords': [{'word': word, 'count': count} for word, count in top_keywords]
        }

    def _calculate_recommendations(self, results: List[Dict]) -> Dict[str, Any]:
        """
        Calculate content recommendations based on SERP analysis.

        Args:
            results: List of analyzed SERP results

        Returns:
            Recommendations for content strategy
        """
        # Filter results with word counts
        word_counts = [r['word_count'] for r in results if r['word_count']]

        if not word_counts:
            return {
                'target_word_count': 2000,
                'note': 'Default target (could not analyze competitors)'
            }

        # Calculate statistics
        avg_word_count = sum(word_counts) / len(word_counts)
        max_word_count = max(word_counts)
        min_word_count = min(word_counts)

        # Recommend: Beat the top result by 20-30%
        top_result_words = results[0]['word_count'] if results[0]['word_count'] else avg_word_count
        target_word_count = int(top_result_words * 1.25)

        # But not less than average
        target_word_count = max(target_word_count, int(avg_word_count))

        # And not ridiculously long
        target_word_count = min(target_word_count, 5000)

        return {
            'target_word_count': target_word_count,
            'competitor_avg': int(avg_word_count),
            'competitor_max': max_word_count,
            'competitor_min': min_word_count,
            'top_result_words': results[0]['word_count'],
            'strategy': 'Target 25% more than top result (skyscraper technique)'
        }

    def analyze_competitor_library(self, library_name: str) -> Dict[str, Any]:
        """
        Analyze multiple keywords for a competitor library.

        Args:
            library_name: Name of competitor library

        Returns:
            Combined analysis across multiple keyword patterns
        """
        keywords = [
            f"{library_name} vs ironbarcode",
            f"{library_name} alternative c#",
            f"{library_name} review"
        ]

        analyses = {}
        for keyword in keywords:
            try:
                analyses[keyword] = self.analyze_keyword(keyword, top_n=3)
            except Exception as e:
                print(f"Error analyzing '{keyword}': {e}")
                analyses[keyword] = {"error": str(e)}

        return {
            'library': library_name,
            'keywords_analyzed': keywords,
            'analyses': analyses
        }


def get_content_targets(library_name: str) -> Dict[str, Any]:
    """
    Convenience function to get content targets for a barcode library comparison.

    Args:
        library_name: Name of competitor library (e.g., "zxing", "aspose.barcode")

    Returns:
        Content strategy recommendations
    """
    try:
        analyzer = SERPAnalyzer()
        primary_keyword = f"{library_name} vs ironbarcode"

        print(f"\nAnalyzing content targets for: {library_name}")
        print("="*60)

        analysis = analyzer.analyze_keyword(primary_keyword, top_n=5)

        if "error" in analysis:
            return {"error": analysis["error"]}

        recommendations = analysis.get('recommendations', {})

        return {
            'library': library_name,
            'primary_keyword': primary_keyword,
            'target_word_count': recommendations.get('target_word_count', 2000),
            'competitor_avg': recommendations.get('competitor_avg', 0),
            'strategy': recommendations.get('strategy', ''),
            'top_results': [
                {
                    'position': r['position'],
                    'title': r['title'],
                    'word_count': r['word_count']
                }
                for r in analysis.get('results', [])[:3]
            ]
        }

    except Exception as e:
        return {"error": str(e)}


if __name__ == "__main__":
    import sys
    import json

    # Default test keyword
    library = sys.argv[1] if len(sys.argv) > 1 else "zxing"

    print(f"SERP Analysis for: {library}")
    print("="*60)

    targets = get_content_targets(library)

    if "error" in targets:
        print(f"Error: {targets['error']}")
        sys.exit(1)

    print("\nContent Strategy Recommendations:")
    print(f"  Primary Keyword: {targets['primary_keyword']}")
    print(f"  Target Word Count: {targets['target_word_count']}")
    print(f"  Competitor Average: {targets['competitor_avg']}")
    print(f"  Strategy: {targets['strategy']}")

    print("\nTop Competing Pages:")
    for result in targets['top_results']:
        print(f"  #{result['position']}: {result['title']}")
        if result['word_count']:
            print(f"    Word count: {result['word_count']}")

    print("\n" + "="*60)
    print("Full analysis:")
    print(json.dumps(targets, indent=2))
