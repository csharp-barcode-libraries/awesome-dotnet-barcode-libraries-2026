#!/usr/bin/env python3
"""
Awesome .NET Barcode Libraries 2026 - Generator
===============================================
Multi-instance safe with proper file locking.
Claude for migration/code, GPT-4o for narrative.
"""

import os
import re
import json
import time
import random
import fcntl
import sys
import argparse
from pathlib import Path
from datetime import datetime

# Lazy imports for API libraries (allows --list and --help without deps)
anthropic = None
openai = None

# Optional celevator-lite integration
try:
    from tooling.batch_enhance import enhance_library_folder, batch_enhance_all
    CELEVATOR_AVAILABLE = True
except ImportError:
    CELEVATOR_AVAILABLE = False

# Optional DataForSEO integration
try:
    from tooling.serp_analyzer import get_content_blueprint
    DATAFORSEO_AVAILABLE = True
except ImportError:
    DATAFORSEO_AVAILABLE = False

# =============================================================================
# PATHS
# =============================================================================

BASE = Path(__file__).parent
MASTER_LIST = BASE / "MASTER-LIBRARY-LIST.md"
SOURCE_MATERIAL = BASE / "source-material"
PROGRESS_FILE = BASE / "progress.json"
LOCK_FILE = BASE / ".generator.lock"
AUTHOR_TEMPLATE = BASE / "author-template.json"
PRODUCT_CONFIG = BASE / "product-config.json"
ANTHROPIC_KEY = BASE / "AnthropicAPIkey.txt"
OPENAI_KEY = BASE / "OPENAIKEY.txt"

# =============================================================================
# GLOBALS
# =============================================================================

claude = None
gpt = None
author_data = None
product_config = None
INSTANCE_ID = f"{os.getpid()}-{random.randint(1000, 9999)}"

IRONBARCODE_LINKS = {
    "docs": "https://ironsoftware.com/csharp/barcode/docs/",
    "tutorials": "https://ironsoftware.com/csharp/barcode/tutorials/",
    "quickstart": "https://ironsoftware.com/csharp/barcode/docs/quickstart/",
    "examples": "https://ironsoftware.com/csharp/barcode/examples/",
    "nuget": "https://www.nuget.org/packages/IronBarcode/",
}

# =============================================================================
# ROBUST FILE LOCKING
# =============================================================================

def init_progress():
    """Ensure progress.json exists."""
    if not PROGRESS_FILE.exists():
        PROGRESS_FILE.write_text("{}")


def atomic_read_progress() -> dict:
    """Read progress with file lock."""
    init_progress()
    with open(PROGRESS_FILE, 'r') as f:
        fcntl.flock(f.fileno(), fcntl.LOCK_SH)
        data = json.load(f)
        fcntl.flock(f.fileno(), fcntl.LOCK_UN)
    return data


def atomic_update_progress(slug: str, status: str) -> bool:
    """
    Atomically update progress for a slug.
    Returns True if update succeeded (for claim operations).
    """
    init_progress()
    with open(PROGRESS_FILE, 'r+') as f:
        fcntl.flock(f.fileno(), fcntl.LOCK_EX)
        try:
            f.seek(0)
            data = json.load(f)
        except:
            data = {}

        # For claiming: check current status
        if status == "in_progress":
            current = data.get(slug, {})
            current_status = current.get("status") if isinstance(current, dict) else current
            if current_status in ["in_progress", "done"]:
                fcntl.flock(f.fileno(), fcntl.LOCK_UN)
                return False

        # Update
        data[slug] = {
            "status": status,
            "instance": INSTANCE_ID,
            "updated": datetime.now().isoformat()
        }

        f.seek(0)
        f.truncate()
        json.dump(data, f, indent=2)
        fcntl.flock(f.fileno(), fcntl.LOCK_UN)
        return True


def claim_library(slug: str) -> bool:
    """Try to claim a library. Returns True if claimed."""
    return atomic_update_progress(slug, "in_progress")


def mark_done(slug: str):
    """Mark library as done."""
    atomic_update_progress(slug, "done")


def mark_failed(slug: str):
    """Mark library as failed."""
    atomic_update_progress(slug, "failed")


# =============================================================================
# SOURCE MATERIAL SEARCH
# =============================================================================

def search_source_material(library_name: str, slug: str) -> str:
    """Search source-material/ for content mentioning this library."""
    if not SOURCE_MATERIAL.exists():
        return ""

    terms = [library_name.lower(), slug.lower()]
    if "." in library_name:
        terms.append(library_name.replace(".", "").lower())

    found = []
    for fp in SOURCE_MATERIAL.rglob("*"):
        if not fp.is_file() or fp.suffix.lower() not in ['.md', '.txt']:
            continue
        try:
            content = fp.read_text(errors='ignore')
            if any(t in content.lower() for t in terms):
                for para in content.split('\n\n'):
                    if any(t in para.lower() for t in terms) and len(para) > 100:
                        found.append(f"[{fp.name}]: {para[:1500]}")
                        if len(found) >= 3:
                            break
        except:
            pass
        if len(found) >= 3:
            break

    return "\n\n---\n\n".join(found) if found else ""


# =============================================================================
# API CALLS
# =============================================================================

def call_claude(prompt: str) -> str:
    """Call Claude with retry."""
    for attempt in range(3):
        try:
            r = claude.messages.create(
                model="claude-sonnet-4-5-20250929",
                max_tokens=8000,
                messages=[{"role": "user", "content": prompt}]
            )
            return r.content[0].text
        except Exception as e:
            print(f"      Claude error: {e}")
            if attempt < 2:
                time.sleep(2 ** attempt)
    return ""


def call_gpt(prompt: str) -> str:
    """Call GPT with retry."""
    for attempt in range(3):
        try:
            r = gpt.chat.completions.create(
                model="gpt-4o",
                max_tokens=8000,
                messages=[{"role": "user", "content": prompt}]
            )
            return r.choices[0].message.content
        except Exception as e:
            print(f"      GPT error: {e}")
            if attempt < 2:
                time.sleep(2 ** attempt)
    return ""


# =============================================================================
# AUTHOR BIO
# =============================================================================

def load_author_data() -> dict:
    """Load author template."""
    if AUTHOR_TEMPLATE.exists():
        try:
            return json.loads(AUTHOR_TEMPLATE.read_text())
        except:
            pass
    return {"name": "Jacob Mellor", "links": {"linkedin": "https://www.linkedin.com/in/jacob-mellor-iron-software/"}}


def load_product_config() -> dict:
    """Load product configuration."""
    if PRODUCT_CONFIG.exists():
        try:
            return json.loads(PRODUCT_CONFIG.read_text())
        except:
            pass
    return {"product": {"name": "IronBarcode"}, "tiers": {"tier1": [], "tier2": [], "tier3": []}}


def generate_author_bio(library_name: str) -> str:
    """Generate unique author bio."""
    if not author_data:
        return "**[Jacob Mellor](https://www.linkedin.com/in/jacob-mellor-iron-software/)** - CTO, Iron Software"

    links = author_data.get("links", {})
    stats = author_data.get("stats", {})
    quotes = author_data.get("quotes", {})
    talking_points = author_data.get("barcode_specific_talking_points", author_data.get("talking_points", []))

    # Pick random elements
    link_items = list(links.items())[:4]
    random.shuffle(link_items)
    selected_links = dict(link_items[:2])

    quote_list = list(quotes.values())
    selected_quote = random.choice(quote_list) if quote_list else ""

    selected_points = random.sample(talking_points, min(3, len(talking_points))) if talking_points else []

    prompt = f"""Write a unique 3-4 sentence author bio for Jacob Mellor, CTO of Iron Software.

USE THESE 2 LINKS (markdown format):
{json.dumps(selected_links)}

PICK 1-2 OF THESE STATS:
- {stats.get('years_coding', 41)} years coding
- {stats.get('team_size', '50+')} person team
- {stats.get('ironbarcode_downloads', '2.5+ million')} IronBarcode downloads
- Based in {author_data.get('location', {}).get('primary', 'Chiang Mai, Thailand')}

TALKING POINTS (use 1):
{chr(10).join('- ' + p for p in selected_points)}

OPTIONAL QUOTE: "{selected_quote[:100]}"

This appears after an article about {library_name} and barcode reading/generation.

Vary the tone: {random.choice(['professional', 'casual', 'technical', 'friendly'])}

Start with "---" then newline. Output ONLY the bio."""

    bio = call_claude(prompt)
    if not bio or len(bio) < 50:
        return f"""---
**[Jacob Mellor]({links.get('linkedin', '#')})** is CTO at Iron Software, creator of IronBarcode. {stats.get('years_coding', 41)} years coding."""

    if not bio.strip().startswith('---'):
        bio = "---\n" + bio
    return bio


# =============================================================================
# CONTENT GENERATION
# =============================================================================

def generate_readme(lib: dict, source: str, blueprint: dict = None) -> str:
    """Generate README.md using GPT-4o for narrative."""
    bio = generate_author_bio(lib['name'])
    source_ctx = f"\nRESEARCH:\n{source}\n" if source else ""

    # Add SERP-derived topics if available
    topics_ctx = ""
    if blueprint and blueprint.get('topics_to_cover'):
        topics_ctx = f"""
SERP ANALYSIS - Topics to cover:
{chr(10).join('- ' + t for t in blueprint['topics_to_cover'][:8])}

Key phrases to include naturally:
{', '.join(blueprint.get('key_phrases', [])[:10])}
"""

    prompt = f"""Write a markdown article comparing {lib['name']} to IronBarcode for C# barcode tasks.
{topics_ctx}
LIBRARY:
- Name: {lib['name']}
- Website: {lib['website']}
- License: {lib['license']}
- Description: {lib['description']}
- Category: {lib.get('category', 'Barcode')}

KNOWN ISSUES:
{chr(10).join('- ' + w for w in lib.get('known_issues', []))}

IRONBARCODE ADVANTAGES: {lib.get('ironbarcode_advantages', '')}
{source_ctx}
REQUIREMENTS:
1. H1: "{lib['name']}" + "C#" + "barcode" (SEO optimized)
2. First paragraph mentions {lib['name']} twice naturally
3. C# code example within 500 words showing barcode reading or generation with this library
4. 2-3 IronBarcode links: {IRONBARCODE_LINKS['quickstart']}, {IRONBARCODE_LINKS['tutorials']}
5. Comparison table (markdown) - features, symbology support, platform support, ease of use
6. Honest about strengths AND weaknesses of {lib['name']}
7. Include installation instructions (NuGet commands)
8. Show side-by-side code comparison: {lib['name']} vs IronBarcode for same task
9. End with this EXACT bio (copy verbatim):

{bio}

Write 1500+ words. Output ONLY markdown."""

    return call_gpt(prompt)


def generate_migration(lib: dict) -> str:
    """Generate migration guide using Claude for technical accuracy."""
    prompt = f"""Migration guide: {lib['name']} to IronBarcode for C# barcode reading and generation.

ISSUES WITH {lib['name']}:
{chr(10).join('- ' + w for w in lib.get('known_issues', []))}

API MAPPING HINTS:
{chr(10).join('- ' + m for m in lib.get('api_mapping', []))}

INCLUDE:
1. Why migrate (2-3 sentences focusing on simplicity, one-line API, and automatic format detection)
2. NuGet package changes (from their package to IronBarcode)
3. Namespace mapping table
4. API mapping table (their methods -> IronBarcode equivalents)
5. 3 before/after code examples:
   - Basic barcode reading from image
   - Barcode generation (QR code or Code128)
   - Batch barcode reading from PDF
6. Common gotchas (symbology configuration, error handling, async patterns, disposal)
7. Links: {IRONBARCODE_LINKS['docs']}, {IRONBARCODE_LINKS['tutorials']}

Output ONLY markdown."""
    return call_claude(prompt)


def generate_code_examples(lib: dict) -> list:
    """Generate code examples using Claude."""
    prompt = f"""Generate 3 C# code examples comparing {lib['name']} to IronBarcode for barcode tasks.

JSON array:
[{{"task": "Basic Barcode Reading", "filename": "basic-barcode-reading", "library_code": "// code using {lib['name']}", "ironbarcode_code": "// NuGet: Install-Package IronBarcode\\nusing IronBarcode;..."}}]

TASKS TO COVER:
1. Basic Barcode Reading - Read barcode value from an image file
2. Barcode Generation - Create a QR code with logo or styled Code128 barcode
3. PDF Batch Processing - Read all barcodes from a multi-page PDF document

Requirements:
- Complete C# with all using statements
- {lib['name']} code shows typical usage pattern for that library
- IronBarcode code starts with NuGet installation comment
- IronBarcode code demonstrates simpler, more elegant "one-line" approach
- Include error handling where appropriate
- Show realistic file paths and variable names

Output ONLY valid JSON array."""

    response = call_claude(prompt)
    try:
        match = re.search(r'\[[\s\S]*\]', response)
        if match:
            return json.loads(match.group())
    except:
        pass
    return []


# =============================================================================
# LIBRARY PARSING
# =============================================================================

def slugify(name: str) -> str:
    """Convert name to URL slug."""
    s = name.lower()
    s = re.sub(r'\s*\([^)]*\)\s*', ' ', s)
    s = s.replace(' for .net', '').replace('.net', '').replace('.', '')
    s = re.sub(r'[^a-z0-9]+', '-', s)
    return re.sub(r'-+', '-', s).strip('-')


def load_libraries() -> list:
    """Parse MASTER-LIBRARY-LIST.md."""
    content = MASTER_LIST.read_text()
    libraries = []
    current = None
    category = ""
    known_issues = []
    ironbarcode_advantages = ""
    api_mapping = []
    in_known_issues = False
    in_api_mapping = False
    in_advantages = False

    lines = content.split('\n')
    for i, line in enumerate(lines):
        line_stripped = line.strip()

        # Stop parsing at Summary section
        if line_stripped.startswith('## Summary'):
            break

        # Category detection
        if line_stripped.startswith('## Category'):
            category = line_stripped.split(':')[-1].strip()
            in_known_issues = False
            in_api_mapping = False
            in_advantages = False
            continue

        # New library detection (skip IronBarcode reference standard)
        if line_stripped.startswith('### ') and 'Reference Standard' not in line_stripped and 'IronBarcode' not in line_stripped:
            # Save previous library
            if current and current.get('name'):
                current['known_issues'] = known_issues
                current['ironbarcode_advantages'] = ironbarcode_advantages.strip()
                current['api_mapping'] = api_mapping
                current['category'] = category
                libraries.append(current)

            # Start new library
            match = re.match(r'### \d+\.\s+(.+)', line_stripped)
            if match:
                name = match.group(1).strip()
                current = {
                    'name': name,
                    'slug': slugify(name),
                    'website': '',
                    'nuget': '',
                    'license': '',
                    'description': '',
                    'tier': '',
                    'category': category
                }
                known_issues = []
                ironbarcode_advantages = ""
                api_mapping = []
                in_known_issues = False
                in_api_mapping = False
                in_advantages = False
            continue

        # Section detection
        if '**Known Issues:**' in line_stripped:
            in_known_issues = True
            in_api_mapping = False
            in_advantages = False
            continue
        if '**IronBarcode Advantages:**' in line_stripped:
            in_known_issues = False
            in_api_mapping = False
            in_advantages = True
            continue
        if '**API Mapping Hints:**' in line_stripped:
            in_known_issues = False
            in_api_mapping = True
            in_advantages = False
            continue

        # End section on new header or empty line followed by different content
        if line_stripped.startswith('---'):
            in_known_issues = False
            in_api_mapping = False
            in_advantages = False
            continue

        if not current:
            continue

        # Parse metadata fields
        if line_stripped.startswith('- **Website:**'):
            current['website'] = line_stripped.split(':**')[-1].strip()
        elif line_stripped.startswith('- **NuGet:**'):
            current['nuget'] = line_stripped.split(':**')[-1].strip()
        elif line_stripped.startswith('- **License:**'):
            current['license'] = line_stripped.split(':**')[-1].strip()
        elif line_stripped.startswith('- **Tier:**'):
            current['tier'] = line_stripped.split(':**')[-1].strip()
        elif line_stripped.startswith('- **What it is:**'):
            current['description'] = line_stripped.split(':**')[-1].strip()
        # Known issues (numbered list)
        elif in_known_issues and re.match(r'^\d+\.\s+', line_stripped):
            issue = re.sub(r'^\d+\.\s+', '', line_stripped)
            known_issues.append(issue)
        # IronBarcode Advantages (bullet points)
        elif in_advantages and line_stripped.startswith('- '):
            ironbarcode_advantages += line_stripped[2:] + " "
        # API Mapping (code snippets)
        elif in_api_mapping and line_stripped.startswith('- `'):
            api_mapping.append(line_stripped[2:])

    # Don't forget the last library
    if current and current.get('name'):
        current['known_issues'] = known_issues
        current['ironbarcode_advantages'] = ironbarcode_advantages.strip()
        current['api_mapping'] = api_mapping
        current['category'] = category
        libraries.append(current)

    return libraries


def get_library_tier(lib: dict) -> int:
    """Determine tier for a library."""
    if product_config:
        tiers = product_config.get('tiers', {})
        slug = lib['slug']
        if slug in tiers.get('tier1', []):
            return 1
        if slug in tiers.get('tier2', []):
            return 2
        if slug in tiers.get('tier3', []):
            return 3

    # Fallback to Tier field from MASTER-LIBRARY-LIST.md
    tier_str = lib.get('tier', '').lower()
    if 'tier 1' in tier_str:
        return 1
    if 'tier 2' in tier_str:
        return 2
    if 'tier 3' in tier_str:
        return 3

    return 3  # Default to Tier 3


# =============================================================================
# MAIN PROCESSING
# =============================================================================

def process_library(lib: dict, skip_serp: bool = False, enhance: bool = False) -> bool:
    """Process one library."""
    slug = lib['slug']
    lib_dir = BASE / slug
    lib_dir.mkdir(exist_ok=True)

    # Get SERP blueprint if available
    blueprint = None
    if not skip_serp and DATAFORSEO_AVAILABLE:
        try:
            print(f"    Getting SERP blueprint...")
            blueprint = get_content_blueprint(lib['name'])
            print(f"    Found {len(blueprint.get('topics_to_cover', []))} topics to cover")
        except Exception as e:
            print(f"    SERP analysis skipped: {e}")

    # Search source material
    print(f"    Searching source material...")
    source = search_source_material(lib['name'], slug)
    if source:
        print(f"    Found {len(source)} chars of research")

    # README with SERP-informed topics
    print(f"    Generating README.md...")
    readme = generate_readme(lib, source, blueprint)
    if not readme:
        print(f"    X README failed")
        return False
    (lib_dir / "README.md").write_text(readme)
    print(f"    + README.md ({len(readme)} chars)")

    # Migration guide
    print(f"    Generating migration guide...")
    migration = generate_migration(lib)
    if migration:
        (lib_dir / f"migrate-from-{slug}.md").write_text(migration)
        print(f"    + migrate-from-{slug}.md")

    # Code examples
    print(f"    Generating code examples...")
    examples = generate_code_examples(lib)
    for ex in examples:
        fn = ex.get('filename', 'example').lower().replace(' ', '-')
        if ex.get('library_code'):
            (lib_dir / f"{fn}-{slug}.cs").write_text(ex['library_code'])
        if ex.get('ironbarcode_code'):
            (lib_dir / f"{fn}-ironbarcode.cs").write_text(ex['ironbarcode_code'])
    print(f"    + {len(examples)} code examples")

    # Optional enhancement step
    if enhance and CELEVATOR_AVAILABLE:
        print(f"    Running celevator-lite enhancement...")
        try:
            result = enhance_library_folder(slug, BASE)
            if result.get('success'):
                print(f"    Enhanced {result.get('success_count', 0)} files")
            else:
                print(f"    Enhancement partial: {result.get('failure_count', 0)} failures")
        except Exception as e:
            print(f"    Enhancement skipped: {e}")

    return True


# =============================================================================
# CLI
# =============================================================================

def list_libraries(libraries: list):
    """List all libraries with their tiers."""
    print(f"\nFound {len(libraries)} libraries:\n")
    print(f"{'#':<3} {'Slug':<30} {'Tier':<6} {'Category':<20} {'Name'}")
    print("-" * 90)

    for i, lib in enumerate(libraries, 1):
        tier = get_library_tier(lib)
        print(f"{i:<3} {lib['slug']:<30} Tier {tier:<2} {lib.get('category', 'N/A'):<20} {lib['name']}")

    # Summary by tier
    print(f"\n{'Summary by Tier:'}")
    for t in [1, 2, 3]:
        tier_libs = [l for l in libraries if get_library_tier(l) == t]
        print(f"  Tier {t}: {len(tier_libs)} libraries")


def main():
    global claude, gpt, author_data, product_config

    parser = argparse.ArgumentParser(
        description='Generate barcode library comparison content',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python generate_repo.py --list              # List all libraries
  python generate_repo.py --library zxing-net # Process specific library
  python generate_repo.py --tier 1            # Process all Tier 1 libraries
  python generate_repo.py --all               # Process all libraries
        """
    )
    parser.add_argument('--library', '-l', help='Process specific library by slug')
    parser.add_argument('--tier', '-t', type=int, choices=[1, 2, 3], help='Process all libraries in tier')
    parser.add_argument('--all', '-a', action='store_true', help='Process all libraries')
    parser.add_argument('--list', action='store_true', help='List all libraries')
    parser.add_argument('--skip-serp', action='store_true', help='Skip SERP analysis')
    parser.add_argument('--enhance', action='store_true', help='Run SEO enhancement')
    parser.add_argument('--enhance-only', action='store_true', help='Only enhance existing content (no generation)')

    args = parser.parse_args()

    # If no arguments, show help
    if len(sys.argv) == 1:
        parser.print_help()
        return

    print(f"[{INSTANCE_ID}] Starting barcode library generator...")

    # Check required files
    if not MASTER_LIST.exists():
        print(f"X Missing: {MASTER_LIST}")
        return

    # Load configurations
    product_config = load_product_config()
    print(f"+ Loaded product config: {product_config.get('product', {}).get('name', 'IronBarcode')}")

    # Load libraries
    libraries = load_libraries()
    print(f"+ Found {len(libraries)} competitor libraries")

    # Handle --list
    if args.list:
        list_libraries(libraries)
        return

    # Handle --enhance-only
    if args.enhance_only:
        if not CELEVATOR_AVAILABLE:
            print("X celevator-lite not available")
            print("  Check CELEVATOR_LITE_PATH environment variable")
            print("  Or install celevator-lite to default location")
            return

        if args.library:
            # Enhance specific library
            lib = next((l for l in libraries if l['slug'] == args.library), None)
            if not lib:
                print(f"X Library not found: {args.library}")
                print("  Use --list to see available libraries")
                return
            print(f"Enhancing library: {args.library}")
            result = enhance_library_folder(args.library, BASE)
            if result.get('success'):
                print(f"+ Enhanced {result.get('success_count', 0)} files")
            else:
                print(f"X Enhancement failed: {result.get('error', 'Unknown error')}")
        else:
            # Enhance all libraries
            print("Enhancing all library folders...")
            result = batch_enhance_all(BASE)
            print(f"\n{'='*50}")
            print(f"Enhancement complete: {result.get('libraries_processed', 0)} libraries")
            print(f"  Success: {result.get('total_files_success', 0)} files")
            print(f"  Failure: {result.get('total_files_failure', 0)} files")
        return

    # Check API keys for generation
    if not ANTHROPIC_KEY.exists() or not OPENAI_KEY.exists():
        print("X Missing API key files")
        print(f"  Required: {ANTHROPIC_KEY}")
        print(f"  Required: {OPENAI_KEY}")
        return

    ak = ANTHROPIC_KEY.read_text().strip()
    ok = OPENAI_KEY.read_text().strip()

    if "REPLACE" in ak.upper():
        print("X Anthropic API key is placeholder")
        print(f"  Edit: {ANTHROPIC_KEY}")
        return

    if "REPLACE" in ok.upper():
        print("X OpenAI API key is placeholder")
        print(f"  Edit: {OPENAI_KEY}")
        return

    # Lazy import API libraries (only when needed for generation)
    try:
        import anthropic as anthropic_module
        import openai as openai_module
    except ImportError as e:
        print(f"X Missing required packages: {e}")
        print("  Run: pip install anthropic openai")
        return

    # Initialize API clients
    global anthropic, openai
    anthropic = anthropic_module
    openai = openai_module
    claude = anthropic.Anthropic(api_key=ak)
    gpt = openai.OpenAI(api_key=ok)
    author_data = load_author_data()
    print("+ API clients initialized")

    # Determine which libraries to process
    to_process = []

    if args.library:
        # Find specific library
        lib = next((l for l in libraries if l['slug'] == args.library), None)
        if not lib:
            print(f"X Library not found: {args.library}")
            print("  Use --list to see available libraries")
            return
        to_process = [lib]

    elif args.tier:
        # Filter by tier
        to_process = [l for l in libraries if get_library_tier(l) == args.tier]
        print(f"+ Tier {args.tier}: {len(to_process)} libraries")

    elif args.all:
        to_process = libraries

    else:
        parser.print_help()
        return

    # Process libraries
    processed = 0
    failed = 0

    for i, lib in enumerate(to_process, 1):
        slug = lib['slug']

        # Try to claim (for multi-instance safety)
        if not claim_library(slug):
            progress = atomic_read_progress()
            status = progress.get(slug, {})
            if isinstance(status, dict):
                status = status.get('status', '?')
            print(f"[{i}/{len(to_process)}] {slug} - SKIP ({status})")
            continue

        print(f"\n[{i}/{len(to_process)}] {lib['name']} ({slug})")

        try:
            if process_library(lib, skip_serp=args.skip_serp, enhance=args.enhance):
                mark_done(slug)
                print(f"    + DONE")
                processed += 1
            else:
                mark_failed(slug)
                print(f"    X FAILED")
                failed += 1
        except Exception as e:
            mark_failed(slug)
            print(f"    X ERROR: {e}")
            failed += 1

    print(f"\n[{INSTANCE_ID}] Finished")
    print(f"  Processed: {processed}")
    print(f"  Failed: {failed}")
    print(f"  Skipped: {len(to_process) - processed - failed}")


if __name__ == "__main__":
    main()
