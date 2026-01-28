#!/usr/bin/env python3
"""
Batch Content Enhancement with celevator-lite
==============================================
Orchestrates celevator-lite for SEO optimization and quality enhancement
of barcode library comparison articles.

Usage:
    python tooling/batch_enhance.py [path]           # Enhance single file/folder
    python tooling/batch_enhance.py --all            # Enhance all library folders
    python tooling/batch_enhance.py --dry-run        # Validate without changes

Environment variables:
    CELEVATOR_LITE_PATH - Path to celevator-lite installation
                          (default: /Users/jacob/Sites/celevator-lite)
"""

import os
import sys
import subprocess
import argparse
from pathlib import Path
from typing import Optional, List, Dict


def get_celevator_path() -> Path:
    """
    Locate celevator-lite installation.

    Returns:
        Path: Path to celevator-lite directory

    Raises:
        FileNotFoundError: If celevator-lite cannot be found
    """
    # Check environment variable first
    env_path = os.environ.get('CELEVATOR_LITE_PATH')
    if env_path:
        path = Path(env_path)
        if path.exists():
            return path
        raise FileNotFoundError(
            f"CELEVATOR_LITE_PATH points to non-existent path: {env_path}"
        )

    # Check default location
    default_path = Path.home() / 'Sites' / 'celevator-lite'
    if default_path.exists():
        return default_path

    raise FileNotFoundError(
        "celevator-lite not found. Set CELEVATOR_LITE_PATH environment variable "
        f"or install at default location: {default_path}"
    )


def enhance_file(file_path: Path, celevator_path: Path, dry_run: bool = False,
                 verbose: bool = False) -> Dict[str, any]:
    """
    Enhance a single file with celevator-lite.

    Args:
        file_path: Path to the file to enhance
        celevator_path: Path to celevator-lite installation
        dry_run: If True, validate without making changes
        verbose: If True, show detailed output

    Returns:
        Dict with keys: success (bool), message (str), output (str)
    """
    if not file_path.exists():
        return {
            'success': False,
            'message': f"File not found: {file_path}",
            'output': ''
        }

    if not file_path.suffix.lower() in ['.md', '.markdown']:
        return {
            'success': False,
            'message': f"Skipping non-markdown file: {file_path}",
            'output': ''
        }

    # Build celevator-lite command
    cmd = ['php', str(celevator_path / 'celevator-lite.php')]

    if dry_run:
        cmd.append('--dry-run')

    cmd.append(str(file_path))

    try:
        if verbose:
            print(f"Running: {' '.join(cmd)}")

        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            cwd=str(celevator_path)
        )

        success = result.returncode == 0
        output = result.stdout + result.stderr

        if verbose or not success:
            print(output)

        return {
            'success': success,
            'message': f"{'Enhanced' if success else 'Failed'}: {file_path.name}",
            'output': output
        }

    except Exception as e:
        return {
            'success': False,
            'message': f"Error processing {file_path.name}: {str(e)}",
            'output': ''
        }


def enhance_directory(dir_path: Path, celevator_path: Path, dry_run: bool = False,
                      verbose: bool = False, recursive: bool = True) -> List[Dict]:
    """
    Enhance all markdown files in a directory.

    Args:
        dir_path: Path to directory
        celevator_path: Path to celevator-lite installation
        dry_run: If True, validate without making changes
        verbose: If True, show detailed output
        recursive: If True, process subdirectories

    Returns:
        List of result dictionaries from enhance_file()
    """
    if not dir_path.exists() or not dir_path.is_dir():
        print(f"Error: {dir_path} is not a valid directory")
        return []

    results = []
    pattern = '**/*.md' if recursive else '*.md'

    for md_file in sorted(dir_path.glob(pattern)):
        if md_file.is_file():
            result = enhance_file(md_file, celevator_path, dry_run, verbose)
            results.append(result)

    return results


def enhance_library_folder(library_name: str, base_path: Path, celevator_path: Path,
                           dry_run: bool = False, verbose: bool = False) -> List[Dict]:
    """
    Enhance all content in a specific library folder.

    Args:
        library_name: Name of the library folder
        base_path: Base project path
        celevator_path: Path to celevator-lite installation
        dry_run: If True, validate without making changes
        verbose: If True, show detailed output

    Returns:
        List of result dictionaries
    """
    library_path = base_path / library_name

    if not library_path.exists():
        print(f"Warning: Library folder not found: {library_path}")
        return []

    print(f"\nProcessing library: {library_name}")
    print("=" * 60)

    results = enhance_directory(library_path, celevator_path, dry_run, verbose)

    # Summary
    success_count = sum(1 for r in results if r['success'])
    print(f"\nCompleted: {success_count}/{len(results)} files enhanced")

    return results


def batch_enhance_all(base_path: Path, celevator_path: Path, dry_run: bool = False,
                      verbose: bool = False, exclude: Optional[List[str]] = None) -> Dict:
    """
    Enhance all library folders in the project.

    Args:
        base_path: Base project path
        celevator_path: Path to celevator-lite installation
        dry_run: If True, validate without making changes
        verbose: If True, show detailed output
        exclude: List of folder names to exclude

    Returns:
        Dict with overall statistics
    """
    exclude = exclude or ['.git', '.planning', 'source-material', 'tooling',
                          '__pycache__', 'dotnet-barcode-use-cases']

    # Find all library folders (directories in base_path, excluding special folders)
    library_folders = [
        d for d in base_path.iterdir()
        if d.is_dir() and d.name not in exclude and not d.name.startswith('.')
    ]

    if not library_folders:
        print("No library folders found")
        return {'total_files': 0, 'total_success': 0, 'libraries': 0}

    print(f"Found {len(library_folders)} library folders")
    print("=" * 60)

    all_results = []

    for library_folder in sorted(library_folders):
        results = enhance_library_folder(
            library_folder.name,
            base_path,
            celevator_path,
            dry_run,
            verbose
        )
        all_results.extend(results)

    # Overall summary
    total_success = sum(1 for r in all_results if r['success'])

    print("\n" + "=" * 60)
    print("BATCH ENHANCEMENT COMPLETE")
    print("=" * 60)
    print(f"Libraries processed: {len(library_folders)}")
    print(f"Total files: {len(all_results)}")
    print(f"Successfully enhanced: {total_success}")
    print(f"Failed: {len(all_results) - total_success}")

    return {
        'total_files': len(all_results),
        'total_success': total_success,
        'libraries': len(library_folders)
    }


def main():
    """Main entry point for command-line usage."""
    parser = argparse.ArgumentParser(
        description='Batch content enhancement with celevator-lite',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=__doc__
    )

    parser.add_argument(
        'path',
        nargs='?',
        help='Path to file or directory to enhance'
    )

    parser.add_argument(
        '--all',
        action='store_true',
        help='Enhance all library folders in the project'
    )

    parser.add_argument(
        '--dry-run',
        action='store_true',
        help='Validate without making changes'
    )

    parser.add_argument(
        '--verbose',
        action='store_true',
        help='Show detailed output'
    )

    parser.add_argument(
        '--base-path',
        type=Path,
        default=Path.cwd(),
        help='Base project path (default: current directory)'
    )

    args = parser.parse_args()

    # Validate arguments
    if not args.path and not args.all:
        parser.print_help()
        print("\nError: Specify a path or use --all")
        sys.exit(1)

    try:
        # Locate celevator-lite
        celevator_path = get_celevator_path()
        print(f"Using celevator-lite at: {celevator_path}")

        if args.dry_run:
            print("DRY RUN MODE - No changes will be made")

        print()

        # Execute based on mode
        if args.all:
            batch_enhance_all(
                args.base_path,
                celevator_path,
                args.dry_run,
                args.verbose
            )
        else:
            path = Path(args.path)

            if path.is_file():
                result = enhance_file(path, celevator_path, args.dry_run, args.verbose)
                print(result['message'])
                sys.exit(0 if result['success'] else 1)

            elif path.is_dir():
                results = enhance_directory(path, celevator_path, args.dry_run, args.verbose)
                success_count = sum(1 for r in results if r['success'])
                print(f"\nCompleted: {success_count}/{len(results)} files enhanced")
                sys.exit(0 if success_count == len(results) else 1)

            else:
                print(f"Error: Path not found: {path}")
                sys.exit(1)

    except FileNotFoundError as e:
        print(f"Error: {e}")
        sys.exit(1)

    except KeyboardInterrupt:
        print("\n\nInterrupted by user")
        sys.exit(130)


if __name__ == '__main__':
    main()
