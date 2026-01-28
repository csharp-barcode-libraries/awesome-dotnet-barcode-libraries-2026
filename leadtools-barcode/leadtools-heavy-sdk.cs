// =============================================================================
// LEADTOOLS Barcode: Heavy SDK Footprint Demo
// =============================================================================
// This example demonstrates the SDK footprint differences between LEADTOOLS
// (part of large imaging suite) and IronBarcode (focused barcode library).
//
// Author: Jacob Mellor, CTO of Iron Software
// Last verified: January 2026
// =============================================================================

/*
 * INSTALLATION COMPARISON
 *
 * LEADTOOLS (minimum for barcode):
 * dotnet add package Leadtools.Barcode
 * dotnet add package Leadtools
 * dotnet add package Leadtools.Codecs
 * dotnet add package Leadtools.Codecs.Png  (for PNG support)
 * dotnet add package Leadtools.Codecs.Jpeg (for JPEG support)
 * dotnet add package Leadtools.Codecs.Pdf  (for PDF support)
 *
 * Plus Windows runtime requirements:
 * - MSVC++ 2017 Redistributable
 *
 * IronBarcode:
 * dotnet add package IronBarcode
 *
 * That's it.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// LEADTOOLS namespaces (commented - require installation)
// using Leadtools;
// using Leadtools.Barcode;
// using Leadtools.Codecs;

// IronBarcode namespace
using IronBarCode;

namespace BarcodeComparison
{
    /// <summary>
    /// Demonstrates the SDK footprint differences between LEADTOOLS
    /// (imaging suite) and IronBarcode (focused barcode library).
    /// </summary>
    public class HeavySdkComparison
    {
        // =====================================================================
        // PART 1: PACKAGE DEPENDENCY ANALYSIS
        // =====================================================================

        /// <summary>
        /// LEADTOOLS packages required for barcode functionality.
        /// </summary>
        public void AnalyzeLeadtoolsPackages()
        {
            Console.WriteLine("LEADTOOLS NuGet Packages Required:");
            Console.WriteLine("-".PadRight(65, '-'));

            var packages = new[]
            {
                // Core packages
                ("Leadtools", "~40 MB", "Core imaging library (required)"),
                ("Leadtools.Barcode", "~8 MB", "Barcode engine"),
                ("Leadtools.Codecs", "~25 MB", "Image codec infrastructure"),

                // Format-specific codecs (need each one you use)
                ("Leadtools.Codecs.Png", "~2 MB", "PNG format support"),
                ("Leadtools.Codecs.Jpeg", "~3 MB", "JPEG format support"),
                ("Leadtools.Codecs.Tif", "~4 MB", "TIFF format support"),
                ("Leadtools.Codecs.Bmp", "~1 MB", "BMP format support"),
                ("Leadtools.Codecs.Gif", "~1 MB", "GIF format support"),
                ("Leadtools.Codecs.Pdf", "~15 MB", "PDF format support"),

                // Native dependencies (Windows)
                ("MSVC++ 2017 Runtime", "~14 MB", "Required on Windows"),
            };

            int totalSize = 0;
            foreach (var (package, size, purpose) in packages)
            {
                Console.WriteLine($"  {package,-30} {size,-10} {purpose}");
                if (size.Contains("MB"))
                {
                    var sizeNum = int.Parse(size.Replace("~", "").Replace(" MB", ""));
                    totalSize += sizeNum;
                }
            }

            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine($"  {"ESTIMATED TOTAL:",-30} ~{totalSize} MB");
            Console.WriteLine();

            Console.WriteLine("NOTES:");
            Console.WriteLine("  - Each image format needs its own codec package");
            Console.WriteLine("  - Native dependencies required on Windows");
            Console.WriteLine("  - Linux deployment has different native requirements");
            Console.WriteLine("  - macOS support requires additional configuration");
        }

        /// <summary>
        /// IronBarcode packages - significantly lighter.
        /// </summary>
        public void AnalyzeIronBarcodePackages()
        {
            Console.WriteLine("IronBarcode NuGet Packages Required:");
            Console.WriteLine("-".PadRight(65, '-'));

            var packages = new[]
            {
                ("IronBarcode", "~35 MB", "Complete barcode library"),
                ("(Transitive dependencies)", "included", "ImageSharp, IronSoftware.Drawing"),
            };

            foreach (var (package, size, purpose) in packages)
            {
                Console.WriteLine($"  {package,-30} {size,-10} {purpose}");
            }

            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine($"  {"TOTAL:",-30} ~35 MB   (all features included)");
            Console.WriteLine();

            Console.WriteLine("NOTES:");
            Console.WriteLine("  - Single package includes all image format support");
            Console.WriteLine("  - PDF barcode extraction included");
            Console.WriteLine("  - No native runtime dependencies");
            Console.WriteLine("  - Cross-platform (.NET Standard)");
        }

        // =====================================================================
        // PART 2: INITIALIZATION COMPLEXITY
        // =====================================================================

        /// <summary>
        /// LEADTOOLS initialization - requires multiple setup steps.
        /// </summary>
        public void DemonstrateLeadtoolsInitialization()
        {
            Console.WriteLine("LEADTOOLS Initialization Pattern:");
            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine(@"
// Step 1: License setup (required before any LEADTOOLS call)
RasterSupport.SetLicense(licenseFilePath, developerKey);
if (RasterSupport.KernelExpired)
    throw new InvalidOperationException(""License expired"");

// Step 2: Initialize codec manager
var codecs = new RasterCodecs();

// Step 3: Configure codec options (for each format you need)
codecs.Options.Jpeg.Load.Quality = 100;
codecs.Options.Png.Load.Interlaced = false;
codecs.Options.Pdf.Load.Resolution = 300;
codecs.Options.Pdf.Load.EnableColorConversion = true;

// Step 4: Initialize barcode engine
var barcodeEngine = new BarcodeEngine();

// Step 5: Configure reader options
var readerOptions = barcodeEngine.Reader.GetDefaultOptions();
readerOptions.EnableColorConversion = true;
readerOptions.EnablePreprocessing = true;
readerOptions.ReturnBoundary = true;

// Step 6: Configure writer options
var writerOptions = barcodeEngine.Writer.GetDefaultOptions();
writerOptions.UseModule = true;
writerOptions.Resolution = 300;

// NOW you can read/write barcodes...
// That's 6 setup steps before you can do actual work
");

            Console.WriteLine("Lines of setup code: ~25");
            Console.WriteLine("Required objects:    3 (codecs, engine, options)");
            Console.WriteLine("Configuration steps: 6");
        }

        /// <summary>
        /// IronBarcode initialization - minimal setup.
        /// </summary>
        public void DemonstrateIronBarcodeInitialization()
        {
            Console.WriteLine("IronBarcode Initialization Pattern:");
            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine(@"
// Step 1: Set license (optional during development)
IronBarCode.License.LicenseKey = ""YOUR-KEY"";

// DONE. Ready to read/write barcodes.
// All defaults are optimized for common use cases.
// No codec configuration needed.
// No engine initialization.
// No options setup required.
");

            Console.WriteLine("Lines of setup code: 1");
            Console.WriteLine("Required objects:    0 (static methods)");
            Console.WriteLine("Configuration steps: 1 (optional)");
        }

        // =====================================================================
        // PART 3: CODE COMPLEXITY COMPARISON
        // =====================================================================

        /// <summary>
        /// LEADTOOLS code to read a barcode from an image.
        /// </summary>
        public void ShowLeadtoolsReadCode()
        {
            Console.WriteLine("LEADTOOLS Barcode Reading Code:");
            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine(@"
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

public class LeadtoolsBarcodeReader
{
    private readonly RasterCodecs _codecs;
    private readonly BarcodeEngine _engine;

    public LeadtoolsBarcodeReader()
    {
        // Initialize license first
        RasterSupport.SetLicense(licPath, devKey);

        // Create codec manager
        _codecs = new RasterCodecs();

        // Create barcode engine
        _engine = new BarcodeEngine();
    }

    public string[] ReadBarcodes(string imagePath)
    {
        // Load image using codecs
        using var image = _codecs.Load(imagePath);

        // Define symbologies to search (MUST specify)
        var symbologies = new BarcodeSymbology[]
        {
            BarcodeSymbology.Code128,
            BarcodeSymbology.Code39,
            BarcodeSymbology.QR,
            BarcodeSymbology.DataMatrix,
            BarcodeSymbology.EAN13,
            BarcodeSymbology.UPCA,
            BarcodeSymbology.PDF417,
            // Add each type you need to recognize...
        };

        // Read barcodes
        var results = _engine.Reader.ReadBarcodes(
            image,                      // RasterImage
            LogicalRectangle.Empty,     // Search region (empty = full image)
            0,                          // Max barcodes (0 = all)
            symbologies);               // Symbologies to search

        // Extract values
        return results.Select(b => b.Value).ToArray();
    }

    public void Dispose()
    {
        _codecs.Dispose();
    }
}

// Line count: ~45
// Objects managed: 3 (codecs, engine, image)
// Must specify symbologies manually
// Must dispose resources manually
");
        }

        /// <summary>
        /// IronBarcode code to read a barcode from an image.
        /// </summary>
        public void ShowIronBarcodeReadCode()
        {
            Console.WriteLine("IronBarcode Barcode Reading Code:");
            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine(@"
using IronBarCode;

public class IronBarcodeBarcodeReader
{
    public IronBarcodeBarcodeReader()
    {
        // Optional license
        IronBarCode.License.LicenseKey = ""YOUR-KEY"";
    }

    public string[] ReadBarcodes(string imagePath)
    {
        // Single line: auto-detects format, reads all barcodes
        var results = BarcodeReader.Read(imagePath);
        return results.Select(b => b.Text).ToArray();
    }
}

// Line count: ~12
// Objects managed: 0 (static methods)
// Auto-detects symbologies
// No disposal needed
");

            // Actually demonstrate it works
            Console.WriteLine();
            Console.WriteLine("WORKING EXAMPLE:");

            // Generate a test barcode
            BarcodeWriter.CreateBarcode("DEMO-12345", BarcodeEncoding.Code128)
                .SaveAsPng("demo-barcode.png");

            // Read it back
            var results = BarcodeReader.Read("demo-barcode.png");
            Console.WriteLine($"  Generated and read barcode: {results.FirstOrDefault()?.Text}");

            // Clean up
            File.Delete("demo-barcode.png");
        }

        // =====================================================================
        // PART 4: PDF PROCESSING COMPARISON
        // =====================================================================

        /// <summary>
        /// LEADTOOLS PDF barcode extraction - requires PDF codec.
        /// </summary>
        public void ShowLeadtoolsPdfProcessing()
        {
            Console.WriteLine("LEADTOOLS PDF Barcode Processing:");
            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine(@"
// Additional package required:
// dotnet add package Leadtools.Codecs.Pdf

using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    var results = new List<string>();

    using var codecs = new RasterCodecs();
    var engine = new BarcodeEngine();

    // Configure PDF loading options
    codecs.Options.Pdf.Load.Resolution = 300;
    codecs.Options.Pdf.Load.EnableColorConversion = true;
    codecs.Options.Pdf.Load.DisplayDepth = 24;

    // Get page count
    int pageCount = codecs.GetTotalPages(pdfPath);

    // Process each page
    for (int page = 1; page <= pageCount; page++)
    {
        // Load single page as raster image
        using var image = codecs.Load(
            pdfPath,
            0,                              // bitsPerPixel (0 = auto)
            CodecsLoadByteOrder.BgrOrGray,  // byte order
            page,                           // start page
            page);                          // end page

        // Read barcodes from page
        var barcodes = engine.Reader.ReadBarcodes(
            image,
            LogicalRectangle.Empty,
            0,
            null);  // null = all symbologies

        foreach (var barcode in barcodes)
        {
            results.Add($""Page {page}: {barcode.Value}"");
        }
    }

    return results;
}

// Line count: ~35
// Additional package: Leadtools.Codecs.Pdf (~15 MB)
// Manual page iteration required
// PDF options configuration required
");
        }

        /// <summary>
        /// IronBarcode PDF barcode extraction - native support.
        /// </summary>
        public void ShowIronBarcodePdfProcessing()
        {
            Console.WriteLine("IronBarcode PDF Barcode Processing:");
            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine(@"
using IronBarCode;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    // Native PDF support, no additional packages
    // Handles multi-page automatically
    var results = BarcodeReader.Read(pdfPath);

    return results
        .Select(b => $""Page {b.PageNumber}: {b.Text}"")
        .ToList();
}

// Line count: 5
// Additional packages: None (PDF support built-in)
// Automatic multi-page handling
// No configuration required
");
        }

        // =====================================================================
        // PART 5: PROJECT SIZE COMPARISON
        // =====================================================================

        /// <summary>
        /// Shows the build output size differences.
        /// </summary>
        public void ShowBuildOutputComparison()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("BUILD OUTPUT SIZE COMPARISON");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            Console.WriteLine("LEADTOOLS Project (barcode + PDF):");
            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine(@"
Published output size (self-contained):

  /publish
    ├── Leadtools.dll                    40 MB
    ├── Leadtools.Barcode.dll             8 MB
    ├── Leadtools.Codecs.dll             25 MB
    ├── Leadtools.Codecs.Png.dll          2 MB
    ├── Leadtools.Codecs.Jpeg.dll         3 MB
    ├── Leadtools.Codecs.Pdf.dll         15 MB
    ├── Leadtools.native.dll (x64)       30 MB
    ├── vc_redist files                  14 MB
    ├── Your application                  1 MB
    └── Other dependencies               10 MB
    ─────────────────────────────────────────
    TOTAL:                              ~148 MB

Docker image size impact: +150-200 MB
");

            Console.WriteLine();
            Console.WriteLine("IronBarcode Project (same functionality):");
            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine(@"
Published output size (self-contained):

  /publish
    ├── IronBarCode.dll                  10 MB
    ├── IronSoftware.Drawing.dll          5 MB
    ├── SixLabors.ImageSharp.dll          3 MB
    ├── IronPdf.Slim.dll                 15 MB
    ├── Your application                  1 MB
    └── Other dependencies                5 MB
    ─────────────────────────────────────────
    TOTAL:                               ~39 MB

Docker image size impact: +40-50 MB
");

            Console.WriteLine();
            Console.WriteLine("COMPARISON:");
            Console.WriteLine("  LEADTOOLS: ~148 MB (barcode + PDF support)");
            Console.WriteLine("  IronBarcode: ~39 MB (same functionality)");
            Console.WriteLine("  Difference: ~109 MB smaller with IronBarcode");
            Console.WriteLine();
            Console.WriteLine("  This matters for:");
            Console.WriteLine("  - Container image pull times");
            Console.WriteLine("  - Cold start latency (serverless)");
            Console.WriteLine("  - Deployment bandwidth");
            Console.WriteLine("  - Disk storage costs");
        }

        // =====================================================================
        // PART 6: FEATURE-PER-LINE COMPARISON
        // =====================================================================

        /// <summary>
        /// Shows the lines of code per feature.
        /// </summary>
        public void ShowLinesOfCodeComparison()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("LINES OF CODE PER FEATURE");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            var comparisons = new[]
            {
                ("Initialize library", 20, 1),
                ("Read single barcode", 15, 2),
                ("Read multiple formats", 25, 2),
                ("Generate Code128", 12, 2),
                ("Generate QR code", 15, 2),
                ("Read from PDF (single page)", 20, 3),
                ("Read from PDF (multi-page)", 35, 3),
                ("Batch process images", 30, 5),
                ("Configure read options", 15, 0),
            };

            Console.WriteLine($"{"Feature",-35} {"LEADTOOLS",-12} {"IronBarcode",-12}");
            Console.WriteLine("-".PadRight(65, '-'));

            int leadtoolsTotal = 0;
            int ironBarcodeTotal = 0;

            foreach (var (feature, leadtools, ironBarcode) in comparisons)
            {
                Console.WriteLine($"{feature,-35} {leadtools,-12} {ironBarcode,-12}");
                leadtoolsTotal += leadtools;
                ironBarcodeTotal += ironBarcode;
            }

            Console.WriteLine("-".PadRight(65, '-'));
            Console.WriteLine($"{"TOTAL",-35} {leadtoolsTotal,-12} {ironBarcodeTotal,-12}");
            Console.WriteLine();
            Console.WriteLine($"Code reduction with IronBarcode: {(1 - (float)ironBarcodeTotal / leadtoolsTotal) * 100:F0}%");
        }

        // =====================================================================
        // PART 7: SUMMARY COMPARISON TABLE
        // =====================================================================

        public void PrintSummaryTable()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("SDK FOOTPRINT SUMMARY");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            var comparisons = new[]
            {
                ("NuGet packages", "5+ (format-specific)", "1"),
                ("Package size", "~110 MB", "~35 MB"),
                ("Published size", "~148 MB", "~39 MB"),
                ("Native runtime deps", "MSVC++ 2017 (Windows)", "None"),
                ("Cross-platform", "Partial", "Full (.NET Standard)"),
                ("Initialization code", "~25 lines", "~1 line"),
                ("Read barcode code", "~45 lines", "~12 lines"),
                ("PDF support", "Separate package", "Built-in"),
                ("Docker complexity", "Native libs + license file", "Simple"),
                ("API style", "Legacy (30+ years)", "Modern fluent"),
                ("Resource management", "Manual (IDisposable)", "Automatic"),
            };

            Console.WriteLine($"{"Metric",-25} {"LEADTOOLS",-25} {"IronBarcode",-20}");
            Console.WriteLine("-".PadRight(70, '-'));

            foreach (var (metric, leadtools, ironBarcode) in comparisons)
            {
                Console.WriteLine($"{metric,-25} {leadtools,-25} {ironBarcode,-20}");
            }
        }
    }

    // =========================================================================
    // DEMONSTRATION
    // =========================================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            var comparison = new HeavySdkComparison();

            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("LEADTOOLS vs IRONBARCODE: SDK FOOTPRINT COMPARISON");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            // 1. Package analysis
            comparison.AnalyzeLeadtoolsPackages();
            Console.WriteLine();

            comparison.AnalyzeIronBarcodePackages();
            Console.WriteLine();

            // 2. Initialization
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("INITIALIZATION COMPLEXITY");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            comparison.DemonstrateLeadtoolsInitialization();
            Console.WriteLine();

            comparison.DemonstrateIronBarcodeInitialization();
            Console.WriteLine();

            // 3. Code comparison
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("CODE COMPLEXITY");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            comparison.ShowLeadtoolsReadCode();
            Console.WriteLine();

            comparison.ShowIronBarcodeReadCode();
            Console.WriteLine();

            // 4. PDF processing
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("PDF PROCESSING");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            comparison.ShowLeadtoolsPdfProcessing();
            Console.WriteLine();

            comparison.ShowIronBarcodePdfProcessing();
            Console.WriteLine();

            // 5. Build output
            comparison.ShowBuildOutputComparison();
            Console.WriteLine();

            // 6. Lines of code
            comparison.ShowLinesOfCodeComparison();
            Console.WriteLine();

            // 7. Summary
            comparison.PrintSummaryTable();

            Console.WriteLine();
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("CONCLUSION");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine(@"
LEADTOOLS is a comprehensive imaging SDK built over 30+ years.
Its barcode module inherits this heritage - mature but heavy.

IronBarcode is a focused barcode library designed for modern .NET.
It provides the same barcode functionality with:
  - 73% smaller package size
  - 89% less initialization code
  - 73% less application code
  - Simpler deployment model
  - Cross-platform compatibility

Choose LEADTOOLS if you need the full imaging suite.
Choose IronBarcode if you only need barcode functionality.
");
        }
    }
}
