"""
DataForSEO API Client
=====================
SERP analysis and keyword density for barcode content generation.

Rate limits: 2 requests/minute for On-Page API

Environment variables required:
- DATAFORSEO_LOGIN
- DATAFORSEO_PASSWORD
"""

import os
import time
import base64
import requests
from typing import Dict, Any, Optional, List
from datetime import datetime, timedelta


class RateLimiter:
    """Simple rate limiter to prevent API abuse."""

    def __init__(self, max_requests: int = 2, time_window: int = 60):
        """
        Initialize rate limiter.

        Args:
            max_requests: Maximum number of requests allowed in time window
            time_window: Time window in seconds
        """
        self.max_requests = max_requests
        self.time_window = time_window
        self.requests: List[datetime] = []

    def wait_if_needed(self):
        """Wait if rate limit would be exceeded."""
        now = datetime.now()
        cutoff = now - timedelta(seconds=self.time_window)

        # Remove old requests outside time window
        self.requests = [req_time for req_time in self.requests if req_time > cutoff]

        # If at limit, wait until oldest request expires
        if len(self.requests) >= self.max_requests:
            oldest = self.requests[0]
            wait_until = oldest + timedelta(seconds=self.time_window)
            wait_seconds = (wait_until - now).total_seconds()

            if wait_seconds > 0:
                print(f"Rate limit reached. Waiting {wait_seconds:.1f} seconds...")
                time.sleep(wait_seconds)

        # Record this request
        self.requests.append(datetime.now())


class DataForSEOClient:
    """Client for DataForSEO API."""

    BASE_URL = "https://api.dataforseo.com/v3"

    def __init__(self, login: Optional[str] = None, password: Optional[str] = None):
        """
        Initialize DataForSEO client.

        Args:
            login: DataForSEO login (defaults to DATAFORSEO_LOGIN env var)
            password: DataForSEO password (defaults to DATAFORSEO_PASSWORD env var)
        """
        self.login = login or os.environ.get('DATAFORSEO_LOGIN')
        self.password = password or os.environ.get('DATAFORSEO_PASSWORD')

        if not self.login or not self.password:
            raise ValueError(
                "DataForSEO credentials not found. "
                "Set DATAFORSEO_LOGIN and DATAFORSEO_PASSWORD environment variables "
                "or pass login/password to constructor."
            )

        self.rate_limiter = RateLimiter(max_requests=2, time_window=60)
        self.session = requests.Session()

        # Set up authentication header
        credentials = f"{self.login}:{self.password}"
        encoded = base64.b64encode(credentials.encode()).decode()
        self.session.headers.update({
            'Authorization': f'Basic {encoded}',
            'Content-Type': 'application/json'
        })

    def _make_request(self, endpoint: str, data: Optional[Dict] = None) -> Dict[str, Any]:
        """
        Make API request with rate limiting.

        Args:
            endpoint: API endpoint (e.g., '/serp/google/organic/live/advanced')
            data: Request payload

        Returns:
            API response as dict
        """
        self.rate_limiter.wait_if_needed()

        url = f"{self.BASE_URL}{endpoint}"

        if data:
            response = self.session.post(url, json=data)
        else:
            response = self.session.get(url)

        response.raise_for_status()
        return response.json()

    def get_serp_results(
        self,
        keyword: str,
        location_code: int = 2840,  # USA
        language_code: str = "en",
        depth: int = 10
    ) -> Dict[str, Any]:
        """
        Get SERP results for a keyword.

        Args:
            keyword: Search keyword
            location_code: Location code (2840 = USA)
            language_code: Language code
            depth: Number of results to retrieve

        Returns:
            SERP results
        """
        endpoint = "/serp/google/organic/live/advanced"

        data = [{
            "keyword": keyword,
            "location_code": location_code,
            "language_code": language_code,
            "depth": depth
        }]

        return self._make_request(endpoint, data)

    def get_on_page_analysis(self, url: str) -> Dict[str, Any]:
        """
        Get on-page analysis for a URL.

        Args:
            url: URL to analyze

        Returns:
            On-page analysis results
        """
        endpoint = "/on_page/instant_pages"

        data = [{
            "url": url,
            "enable_javascript": True,
            "enable_browser_rendering": True
        }]

        return self._make_request(endpoint, data)


def get_keyword_density_targets(library_name: str) -> Dict[str, Any]:
    """
    Convenience function to get keyword density targets for a barcode library.

    Args:
        library_name: Name of the competitor library

    Returns:
        Dict with target keywords and recommended densities
    """
    try:
        client = DataForSEOClient()

        # Get SERP for comparison query (barcode domain)
        serp = client.get_serp_results(f"{library_name} vs ironbarcode c#")

        targets = {
            "primary_keyword": f"{library_name} vs IronBarcode",
            "secondary_keywords": [],
            "serp_data": serp
        }

        # Extract keywords from SERP titles
        if serp.get('tasks') and serp['tasks'][0].get('result'):
            items = serp['tasks'][0]['result'][0].get('items', [])
            for item in items[:5]:
                if item.get('title'):
                    targets['secondary_keywords'].append(item['title'])

        return targets

    except Exception as e:
        return {"error": str(e)}


if __name__ == "__main__":
    import sys

    if len(sys.argv) > 1:
        keyword = sys.argv[1]
    else:
        keyword = "zxing vs ironbarcode"

    print(f"Testing DataForSEO with keyword: {keyword}")

    try:
        client = DataForSEOClient()
        print("✓ Client initialized")

        print(f"\nFetching SERP results for: {keyword}")
        results = client.get_serp_results(keyword)

        print(f"✓ Results retrieved")

        if results.get('tasks'):
            task = results['tasks'][0]
            if task.get('result'):
                items = task['result'][0].get('items', [])
                print(f"\nTop {len(items)} results:")
                for i, item in enumerate(items[:5], 1):
                    print(f"{i}. {item.get('title', 'N/A')}")
                    print(f"   {item.get('url', 'N/A')}")

        # Test convenience function
        print("\n" + "="*50)
        print("Testing get_keyword_density_targets()...")
        targets = get_keyword_density_targets("zxing")

        if "error" in targets:
            print(f"Error: {targets['error']}")
        else:
            print(f"Primary keyword: {targets['primary_keyword']}")
            print(f"Secondary keywords found: {len(targets['secondary_keywords'])}")
            for kw in targets['secondary_keywords'][:3]:
                print(f"  - {kw}")

    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)
