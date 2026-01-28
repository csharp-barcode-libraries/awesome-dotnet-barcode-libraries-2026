// =============================================================================
// GrapeCity ComponentOne Barcode: Suite Dependency Comparison
// =============================================================================
// This example demonstrates the suite dependency differences between
// ComponentOne (bundled with 100+ component UI suite) and IronBarcode
// (standalone barcode-focused package).
//
// Author: Jacob Mellor, CTO of Iron Software
// Last verified: January 2026
// =============================================================================

/*
 * INSTALLATION COMPARISON
 *
 * ComponentOne (requires suite):
 * dotnet add package C1.Win.C1BarCode
 *
 * This pulls in:
 * - C1.dll (core library)
 * - C1.Win.dll (Windows Forms core)
 * - C1.Win.C1BarCode.dll
 * - Various imaging dependencies
 * - License registration assemblies
 *
 * IronBarcode (standalone):
 * dotnet add package IronBarcode
 *
 * This pulls in:
 * - IronBarcode.dll
 * - IronSoftware.Drawing dependencies (cross-platform imaging)
 * - Self-contained with minimal external dependencies
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeComparison
{
    /// <summary>
    /// Demonstrates the installation and dependency differences between
    /// ComponentOne (UI suite component) and IronBarcode (standalone SDK).
    /// </summary>
    public class SuiteDependencyComparison
    {
        // =====================================================================
        // PART 1: PACKAGE DEPENDENCY ANALYSIS
        // =====================================================================

        /// <summary>
        /// ComponentOne WinForms barcode package dependencies.
        /// This is what gets installed when you add C1.Win.C1BarCode.
        /// </summary>
        public void AnalyzeComponentOneDependencies()
        {
            Console.WriteLine("ComponentOne C1.Win.C1BarCode Dependencies:");
            Console.WriteLine("-".PadRight(60, '-'));

            var dependencies = new[]
            {
                // Core dependencies
                ("C1.dll", "Core ComponentOne library (required for all C1 products)"),
                ("C1.Win.dll", "Windows Forms core UI library"),
                ("C1.Win.C1BarCode.dll", "Barcode control assembly"),

                // Imaging dependencies
                ("System.Drawing", "GDI+ imaging (Windows-only)"),

                // License infrastructure
                ("C1.C1License", "License validation assembly"),

                // Potential transitive dependencies based on version
                ("Microsoft.CSharp", "Dynamic language support"),
                ("System.ComponentModel.Annotations", "Data annotations"),
            };

            foreach (var (assembly, purpose) in dependencies)
            {
                Console.WriteLine($"  {assembly,-40} {purpose}");
            }

            Console.WriteLine();
            Console.WriteLine("NOTES:");
            Console.WriteLine("  - Suite assemblies share code across ComponentOne products");
            Console.WriteLine("  - C1.dll is required even for barcode-only usage");
            Console.WriteLine("  - License validation runs at startup");
            Console.WriteLine("  - Windows-specific System.Drawing dependency");
        }

        /// <summary>
        /// IronBarcode package dependencies - significantly lighter.
        /// </summary>
        public void AnalyzeIronBarcodeDependencies()
        {
            Console.WriteLine("IronBarcode Dependencies:");
            Console.WriteLine("-".PadRight(60, '-'));

            var dependencies = new[]
            {
                // Core
                ("IronBarcode.dll", "Core barcode library"),

                // Cross-platform imaging
                ("IronSoftware.Drawing", "Cross-platform imaging abstraction"),
                ("SixLabors.ImageSharp", "Cross-platform image processing"),

                // PDF support (included)
                ("IronPdf.Slim", "PDF barcode extraction support"),
            };

            foreach (var (assembly, purpose) in dependencies)
            {
                Console.WriteLine($"  {assembly,-40} {purpose}");
            }

            Console.WriteLine();
            Console.WriteLine("NOTES:");
            Console.WriteLine("  - Cross-platform (Windows, Linux, macOS)");
            Console.WriteLine("  - No Windows-specific GDI+ dependency");
            Console.WriteLine("  - PDF support included in base package");
            Console.WriteLine("  - Single license key (no file deployment)");
        }

        // =====================================================================
        // PART 2: PROJECT FILE COMPARISON
        // =====================================================================

        /// <summary>
        /// Shows the .csproj differences between the two approaches.
        /// </summary>
        public void ShowProjectFileComparison()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("PROJECT FILE COMPARISON");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            Console.WriteLine("ComponentOne .csproj (WinForms barcode only):");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine(@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>

  <ItemGroup>
    <!-- Barcode package - pulls in suite dependencies -->
    <PackageReference Include=""C1.Win.C1BarCode"" Version=""8.x.x"" />

    <!-- These come transitively with C1.Win.C1BarCode: -->
    <!-- C1.dll, C1.Win.dll, System.Drawing.Common, etc. -->
  </ItemGroup>
</Project>

NOTES:
- Requires 'net8.0-windows' target (Windows-specific)
- UseWindowsForms must be true (even for non-UI usage)
- Cannot deploy to Linux or macOS
- Suite DLLs included even for barcode-only usage
");

            Console.WriteLine();
            Console.WriteLine("IronBarcode .csproj (standalone):");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine(@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <!-- No Windows-specific settings required -->
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""IronBarcode"" Version=""2024.x.x"" />
    <!-- That's it - single package, all functionality included -->
  </ItemGroup>
</Project>

NOTES:
- Standard 'net8.0' target (cross-platform)
- No Windows Forms dependency
- Deploys to Windows, Linux, macOS
- Single package includes generation, reading, PDF support
");
        }

        // =====================================================================
        // PART 3: INITIALIZATION COMPARISON
        // =====================================================================

        /// <summary>
        /// ComponentOne initialization - requires license key at startup.
        /// </summary>
        public void InitializeComponentOne()
        {
            Console.WriteLine("ComponentOne Initialization:");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine(@"
// In Program.cs or App.xaml.cs BEFORE any ComponentOne usage

public class Program
{
    static void Main()
    {
        // REQUIRED: Set license key before using any C1 component
        C1.C1License.Key = ""YOUR-COMPONENTONE-LICENSE-KEY"";

        // License must be set before Application.Run() for WinForms
        // Without license: Evaluation watermarks on barcodes
        // Without license: Potential nag dialogs

        Application.EnableVisualStyles();
        Application.Run(new MainForm());
    }
}

// Alternative: License key in settings/config file
// ComponentOne checks for license at component instantiation
// License validation happens on EVERY C1 component creation
");
        }

        /// <summary>
        /// IronBarcode initialization - optional key, simple setup.
        /// </summary>
        public void InitializeIronBarcode()
        {
            Console.WriteLine("IronBarcode Initialization:");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine(@"
// In Program.cs or startup - single line

public class Program
{
    static void Main()
    {
        // OPTIONAL during development (watermarked output)
        // REQUIRED for production (clean output)
        IronBarCode.License.LicenseKey = ""YOUR-KEY"";

        // OR from environment variable (better for containers)
        IronBarCode.License.LicenseKey =
            Environment.GetEnvironmentVariable(""IRONBARCODE_LICENSE"");

        // That's it - ready to use anywhere
        RunApplication();
    }
}

// No nag dialogs during evaluation
// No functionality restrictions during trial
// License check is lightweight (key validation only)
");
        }

        // =====================================================================
        // PART 4: DEPLOYMENT COMPARISON
        // =====================================================================

        /// <summary>
        /// Docker deployment comparison showing the differences.
        /// </summary>
        public void ShowDockerDeployment()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("DOCKER DEPLOYMENT COMPARISON");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            Console.WriteLine("ComponentOne Dockerfile:");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine(@"
# ComponentOne requires Windows containers
FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2022

WORKDIR /app

# Must include Windows Forms support
# Cannot use Linux containers

# Copy application
COPY publish/ .

# Set license key
ENV C1_LICENSE_KEY=YOUR-KEY-HERE

ENTRYPOINT [""dotnet"", ""YourApp.dll""]

# LIMITATIONS:
# - Windows containers only (no Linux deployment)
# - Larger container images (~5GB+ vs ~200MB)
# - Slower startup
# - Higher hosting costs (Windows VMs)
# - Not compatible with most Kubernetes clusters
");

            Console.WriteLine();
            Console.WriteLine("IronBarcode Dockerfile:");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine(@"
# IronBarcode works on Linux containers
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# No special Windows requirements

# Copy application
COPY publish/ .

# Set license key
ENV IRONBARCODE_LICENSE=YOUR-KEY-HERE

ENTRYPOINT [""dotnet"", ""YourApp.dll""]

# ADVANTAGES:
# - Linux or Windows containers
# - Lightweight images (~200MB)
# - Fast startup
# - Lower hosting costs
# - Compatible with all Kubernetes clusters
# - Works on ARM64 (Apple Silicon, AWS Graviton)
");
        }

        // =====================================================================
        // PART 5: COST OF SUITE BUNDLING
        // =====================================================================

        /// <summary>
        /// Analyzes the true cost of ComponentOne's suite bundling.
        /// </summary>
        public void AnalyzeSuiteBundlingCost()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("SUITE BUNDLING COST ANALYSIS");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            Console.WriteLine("ComponentOne Studio Enterprise includes:");
            Console.WriteLine("-".PadRight(60, '-'));

            var components = new[]
            {
                "FlexGrid (data grid)",
                "FlexChart (charting)",
                "FlexReport (reporting)",
                "FlexViewer (document viewer)",
                "Input controls (50+ types)",
                "Calendar and scheduling",
                "Ribbon and toolbar",
                "Docking and MDI",
                "PDF document library",
                "Excel document library",
                "Word document library",
                "BarCode control <-- THIS IS WHAT YOU NEED",
                "Sparkline controls",
                "TreeView and ListView",
                "Gauge controls",
                "Maps and GIS controls",
                "...and 80+ more components",
            };

            for (int i = 0; i < components.Length; i++)
            {
                var marker = components[i].Contains("BarCode") ? " ***" : "";
                Console.WriteLine($"  {i + 1,2}. {components[i]}{marker}");
            }

            Console.WriteLine();
            Console.WriteLine("COST BREAKDOWN:");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine($"  ComponentOne Studio Enterprise: ~$1,473/developer/year");
            Console.WriteLine($"  Total components:               100+");
            Console.WriteLine($"  Components you need:            1 (barcode)");
            Console.WriteLine($"  Percentage of suite used:       ~1%");
            Console.WriteLine($"  Effective barcode cost:         $1,473 for 1% of value");
            Console.WriteLine();
            Console.WriteLine($"  IronBarcode Professional:       $1,499 ONE-TIME");
            Console.WriteLine($"  What you get:                   100% barcode functionality");
            Console.WriteLine($"  Plus reading capability:        Included (ComponentOne lacks this)");
            Console.WriteLine($"  Plus PDF support:               Included");
            Console.WriteLine();
            Console.WriteLine("5-YEAR COMPARISON (3 developers):");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine($"  ComponentOne: $1,473 x 3 + renewals = ~$11,000+");
            Console.WriteLine($"  IronBarcode:  $1,499 one-time       = $1,499");
            Console.WriteLine($"  Savings:      $9,500+ over 5 years");
            Console.WriteLine();
            Console.WriteLine("CONCLUSION:");
            Console.WriteLine("  Suite bundling makes sense if you use 10+ components.");
            Console.WriteLine("  For barcode-only needs, standalone IronBarcode is 7x better value.");
        }

        // =====================================================================
        // PART 6: CODE COMPLEXITY COMPARISON
        // =====================================================================

        /// <summary>
        /// Side-by-side code showing the API complexity differences.
        /// </summary>
        public void CompareCodeComplexity()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("CODE COMPLEXITY COMPARISON");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            Console.WriteLine("SCENARIO: Generate QR code with custom colors");
            Console.WriteLine();

            Console.WriteLine("ComponentOne approach:");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine(@"
using C1.Win.C1BarCode;
using System.Drawing;

var barcode = new C1BarCode();
barcode.CodeType = CodeType.QRCode;
barcode.Text = ""https://example.com"";

// Configure QR-specific settings
barcode.QRCodeVersion = QRCodeVersion.Version5;
barcode.QRCodeErrorCorrectionLevel = QRCodeErrorCorrectionLevel.High;
barcode.QRCodeModel = QRCodeModel.Model2;

// Configure appearance
barcode.BarHeight = 200;
barcode.ModuleSize = 5;
barcode.BackColor = Color.White;
barcode.ForeColor = Color.DarkBlue;

// Generate image
using var image = barcode.GetImage();
image.Save(""qr-code.png"", System.Drawing.Imaging.ImageFormat.Png);

// Lines of code: 14
// Cannot: Add logo to QR code
// Cannot: Read QR code back
");

            Console.WriteLine();
            Console.WriteLine("IronBarcode approach:");
            Console.WriteLine("-".PadRight(60, '-'));
            Console.WriteLine(@"
using IronBarCode;
using IronSoftware.Drawing;

// Generate with custom colors - 3 lines
QRCodeWriter.CreateQrCode(""https://example.com"", 200)
    .ChangeBarCodeColor(Color.DarkBlue)
    .SaveAsPng(""qr-code.png"");

// OR with logo embedded (ComponentOne cannot do this)
QRCodeWriter.CreateQrCodeWithLogo(""https://example.com"", ""logo.png"", 300)
    .ChangeBarCodeColor(Color.DarkBlue)
    .SaveAsPng(""qr-with-logo.png"");

// AND read it back (ComponentOne cannot do this)
var result = BarcodeReader.Read(""qr-code.png"");
Console.WriteLine(result.First().Text);

// Lines of code: 3-6
// Can: Add logo to QR code
// Can: Read QR code back
");
        }
    }

    // =========================================================================
    // DEMONSTRATION
    // =========================================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            var comparison = new SuiteDependencyComparison();

            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("GRAPECITY COMPONENTONE vs IRONBARCODE: SUITE DEPENDENCY COMPARISON");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            // 1. Dependency analysis
            comparison.AnalyzeComponentOneDependencies();
            Console.WriteLine();

            comparison.AnalyzeIronBarcodeDependencies();
            Console.WriteLine();

            // 2. Project file comparison
            comparison.ShowProjectFileComparison();
            Console.WriteLine();

            // 3. Initialization comparison
            comparison.InitializeComponentOne();
            Console.WriteLine();

            comparison.InitializeIronBarcode();
            Console.WriteLine();

            // 4. Docker deployment
            comparison.ShowDockerDeployment();
            Console.WriteLine();

            // 5. Suite bundling cost
            comparison.AnalyzeSuiteBundlingCost();
            Console.WriteLine();

            // 6. Code complexity
            comparison.CompareCodeComplexity();

            Console.WriteLine();
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("SUMMARY");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine(@"
ComponentOne:
- UI suite with 100+ components (barcode is ~1% of offering)
- Windows-only deployment (System.Drawing dependency)
- Suite licensing model (~$1,473/year/developer)
- Generation-only (no reading capability)

IronBarcode:
- Standalone barcode SDK (100% barcode functionality)
- Cross-platform (Windows, Linux, macOS, Docker)
- Perpetual license option ($749-$1,499 one-time)
- Full read/write capability

Recommendation:
- Use ComponentOne if you're already using their suite for other controls
- Use IronBarcode for barcode-specific needs (better value, more features)
");
        }
    }
}
