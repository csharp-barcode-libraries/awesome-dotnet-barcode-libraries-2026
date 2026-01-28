#!/usr/bin/env python3
"""
C# Code Example Validator

Validates that C# code examples in markdown files compile successfully.
Extracts code blocks, creates temporary .csproj files, and runs dotnet build.

Supports:
- IronBarcode package references (primary)
- Competitor barcode library packages
- Common dependencies (System.Drawing, etc.)

Usage:
    python tooling/validate_code.py [--verbose] [--file PATH]

Exit codes:
    0: All code examples compile
    1: Compilation errors found
"""

import os
import re
import sys
import tempfile
import subprocess
import shutil
import argparse
from pathlib import Path
from typing import List, Tuple, Optional


# Common packages used across examples
COMMON_PACKAGES = {
    'IronBarcode': '2024.*',
    'IronSoftware.System.Drawing': '2024.*',
}

# Competitor barcode libraries
COMPETITOR_PACKAGES = {
    'ZXing.Net': 'ZXing.Net',
    'ZXingCore': 'ZXing.Net Core',
    'Aspose.BarCode': 'Aspose.BarCode',
    'Dynamsoft.BarcodeReader': 'Dynamsoft Barcode Reader',
    'Dynamsoft.DBR': 'Dynamsoft DBR',
    'LEADTOOLS': 'LEADTOOLS',
    'LEADTOOLS.Barcode': 'LEADTOOLS Barcode',
    'Syncfusion.SfBarcode': 'Syncfusion Barcode',
    'DevExpress.XtraBarcodes': 'DevExpress Barcode',
    'Telerik.Barcode': 'Telerik Barcode',
    'Infragistics.Barcode': 'Infragistics Barcode',
    'Spire.Barcode': 'Spire.Barcode',
    'OnBarcode': 'OnBarcode',
    'Neodynamic': 'Neodynamic',
    'BarcodeLib': 'BarcodeLib',
    'NetBarcode': 'NetBarcode',
    'Barcoder': 'Barcoder',
    'Scandit': 'Scandit',
    'Scanbot': 'Scanbot',
    'barKoder': 'barKoder SDK',
    'BarcodeScanning.Maui': 'BarcodeScanning.MAUI',
    'MessagingToolkit.Barcode': 'MessagingToolkit.Barcode',
    'QrCodeNet': 'QrCodeNet',
    'Cloudmersive': 'Cloudmersive',
    'Accusoft': 'Accusoft BarcodeXpress',
    'GrapeCity.Barcode': 'GrapeCity Barcode',
}


class CodeValidator:
    """Validates C# code examples compile successfully."""

    def __init__(self, repo_root: Path, verbose: bool = False):
        self.repo_root = repo_root
        self.verbose = verbose
        self.excluded_dirs = {'.planning', 'tooling', '.git', 'node_modules', '__pycache__'}
        self.failures: List[Tuple[str, str, str]] = []

    def find_markdown_files(self, specific_file: Optional[str] = None) -> List[Path]:
        """Find markdown files to validate."""
        if specific_file:
            path = Path(specific_file)
            if path.exists():
                return [path]
            else:
                print(f"Error: File not found: {specific_file}", file=sys.stderr)
                return []

        md_files = []
        for root, dirs, files in os.walk(self.repo_root):
            dirs[:] = [d for d in dirs if d not in self.excluded_dirs]

            for file in files:
                if file.endswith('.md'):
                    md_files.append(Path(root) / file)

        return md_files

    def extract_csharp_blocks(self, content: str) -> List[str]:
        """Extract C# code blocks from markdown."""
        # Match ```csharp or ```cs code blocks
        pattern = r'```(?:csharp|cs)\n(.*?)```'
        matches = re.findall(pattern, content, re.DOTALL)
        return matches

    def detect_packages(self, code: str) -> List[Tuple[str, str]]:
        """Detect required NuGet packages from code."""
        packages = []

        # Check for IronBarcode usage
        if 'IronBarcode' in code or 'BarcodeReader' in code:
            for pkg, version in COMMON_PACKAGES.items():
                packages.append((pkg, version))

        # Check for competitor packages
        for pkg_name, display_name in COMPETITOR_PACKAGES.items():
            if pkg_name in code or display_name in code:
                packages.append((pkg_name, '*'))

        # If no packages detected, assume IronBarcode
        if not packages:
            for pkg, version in COMMON_PACKAGES.items():
                packages.append((pkg, version))

        return packages

    def create_temp_project(self, code: str, packages: List[Tuple[str, str]]) -> Optional[Path]:
        """Create temporary .NET project with code."""
        try:
            temp_dir = Path(tempfile.mkdtemp(prefix='barcode_validate_'))

            # Create .csproj file
            csproj_content = f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
'''
            for pkg_name, version in packages:
                csproj_content += f'    <PackageReference Include="{pkg_name}" Version="{version}" />\n'

            csproj_content += '''  </ItemGroup>
</Project>
'''

            csproj_path = temp_dir / 'TestProject.csproj'
            csproj_path.write_text(csproj_content)

            # Create Program.cs with wrapped code
            program_content = f'''using System;
using System.IO;
using System.Linq;
using IronBarcode;

{code}
'''

            program_path = temp_dir / 'Program.cs'
            program_path.write_text(program_content)

            return temp_dir

        except Exception as e:
            print(f"Error creating temp project: {e}", file=sys.stderr)
            return None

    def build_project(self, project_dir: Path) -> Tuple[bool, str]:
        """Build the .NET project and return success status and output."""
        try:
            result = subprocess.run(
                ['dotnet', 'build', '--verbosity', 'quiet'],
                cwd=project_dir,
                capture_output=True,
                text=True,
                timeout=60
            )

            success = result.returncode == 0
            output = result.stdout + result.stderr

            return success, output

        except subprocess.TimeoutExpired:
            return False, "Build timed out after 60 seconds"
        except Exception as e:
            return False, f"Build error: {str(e)}"

    def validate_file(self, md_file: Path) -> int:
        """Validate all C# code blocks in a markdown file."""
        failures = 0

        try:
            content = md_file.read_text(encoding='utf-8')
        except Exception as e:
            print(f"Error reading {md_file}: {e}", file=sys.stderr)
            return 0

        code_blocks = self.extract_csharp_blocks(content)

        if not code_blocks:
            if self.verbose:
                print(f"No C# code blocks in {md_file}")
            return 0

        rel_path = md_file.relative_to(self.repo_root)
        print(f"Validating {len(code_blocks)} code block(s) in {rel_path}...")

        for idx, code in enumerate(code_blocks, 1):
            if self.verbose:
                print(f"  Block {idx}: ", end='')

            packages = self.detect_packages(code)
            project_dir = self.create_temp_project(code, packages)

            if not project_dir:
                failures += 1
                if self.verbose:
                    print("✗ (failed to create project)")
                continue

            try:
                success, output = self.build_project(project_dir)

                if success:
                    if self.verbose:
                        print("✓")
                else:
                    failures += 1
                    if self.verbose:
                        print("✗")
                    self.failures.append((str(rel_path), f"Block {idx}", output))

            finally:
                # Clean up temp directory
                shutil.rmtree(project_dir, ignore_errors=True)

        return failures

    def validate_all(self, specific_file: Optional[str] = None) -> int:
        """Validate all markdown files."""
        md_files = self.find_markdown_files(specific_file)

        if not md_files:
            print("No markdown files found to validate.")
            return 0

        print(f"Validating C# code in {len(md_files)} file(s)...\n")

        total_failures = 0
        for md_file in md_files:
            failures = self.validate_file(md_file)
            total_failures += failures

        return total_failures

    def report(self) -> None:
        """Print validation report."""
        if not self.failures:
            print("\n✓ All C# code examples compile successfully!")
            return

        print(f"\n✗ Found {len(self.failures)} compilation error(s):\n")

        for source, block, output in self.failures:
            print(f"  {source} - {block}")
            print(f"    Error output:")
            # Print first few lines of error
            lines = output.strip().split('\n')
            for line in lines[:10]:
                if line.strip():
                    print(f"      {line}")
            if len(lines) > 10:
                print(f"      ... ({len(lines) - 10} more lines)")
            print()


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(
        description='Validate C# code examples in markdown files'
    )
    parser.add_argument(
        '--verbose', '-v',
        action='store_true',
        help='Verbose output'
    )
    parser.add_argument(
        '--file', '-f',
        type=str,
        help='Validate specific file only'
    )

    args = parser.parse_args()

    repo_root = Path(__file__).parent.parent
    validator = CodeValidator(repo_root, verbose=args.verbose)

    failure_count = validator.validate_all(specific_file=args.file)
    validator.report()

    sys.exit(1 if failure_count > 0 else 0)


if __name__ == '__main__':
    main()
