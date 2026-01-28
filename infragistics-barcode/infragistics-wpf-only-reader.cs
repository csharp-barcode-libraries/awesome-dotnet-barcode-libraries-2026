/**
 * Infragistics BarcodeReader WPF-Only Limitation
 *
 * This example demonstrates the platform restriction of Infragistics barcode reading:
 * - BarcodeReader is ONLY available for WPF applications
 * - WinForms has UltraWinBarcode for generation but NO reading
 * - Web applications (ASP.NET, Blazor) have no reading capability
 *
 * IronBarcode provides consistent read/write on ALL platforms.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Comparison: Infragistics Barcode vs IronBarcode
 */

// ============================================================================
// INFRAGISTICS APPROACH: WPF-Only Reading
// ============================================================================

namespace InfragisticsPlatformLimitation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Infragistics WPF Implementation - READING WORKS HERE ONLY
    /// BarcodeReader assembly is exclusive to WPF package
    /// </summary>
    public class InfragisticsWpfBarcodeService
    {
        // Install: Infragistics.WPF.BarcodeReader
        // Requires: Infragistics Ultimate license ($1,675+/year)

        /*
        using Infragistics.Controls.Barcodes;
        using System.Windows.Media.Imaging;

        private BarcodeReader _reader;

        public InfragisticsWpfBarcodeService()
        {
            _reader = new BarcodeReader();
            _reader.DecodeComplete += OnDecodeComplete;
        }

        public void ReadBarcode(string imagePath)
        {
            // Load as WPF BitmapSource
            var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

            // Specify symbologies to look for
            _reader.SymbologyTypes = SymbologyType.Code128 |
                                     SymbologyType.QR |
                                     SymbologyType.EAN13 |
                                     SymbologyType.DataMatrix;

            // Trigger async decode
            _reader.Decode(bitmap);
            // Result comes via event callback
        }

        private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
        {
            if (e.SymbologyValue != null)
            {
                Console.WriteLine($"Decoded: {e.Symbology} = {e.SymbologyValue}");
            }
        }
        */

        public void DemoWpfCapabilities()
        {
            Console.WriteLine("Infragistics WPF Barcode Capabilities:");
            Console.WriteLine("  - Generation: YES (XamBarcode control)");
            Console.WriteLine("  - Reading: YES (BarcodeReader assembly)");
            Console.WriteLine("  - API Style: Event-driven (async callbacks)");
            Console.WriteLine("  - Symbologies: ~15 formats");
            Console.WriteLine();
            Console.WriteLine("  This is the ONLY platform with reading capability.");
        }
    }

    /// <summary>
    /// Infragistics WinForms Implementation - NO READING AVAILABLE
    /// UltraWinBarcode provides generation but no recognition
    /// </summary>
    public class InfragisticsWinFormsBarcodeService
    {
        // Install: Infragistics.Win.UltraWinBarcode
        // Requires: Infragistics Ultimate license ($1,675+/year)

        /*
        using Infragistics.Win.UltraWinBarcode;

        public void GenerateBarcode(string value, string outputPath)
        {
            // Generation WORKS
            var barcode = new UltraWinBarcode();
            barcode.Symbology = Symbology.Code128;
            barcode.Data = value;

            // Save as image
            barcode.SaveTo(outputPath);

            Console.WriteLine("Barcode generated successfully!");
        }

        public string ReadBarcode(string imagePath)
        {
            // NOT POSSIBLE - No BarcodeReader for WinForms
            // The BarcodeReader class does not exist in the WinForms assembly

            // This will not compile:
            // var reader = new BarcodeReader(); // Type not found

            throw new NotSupportedException(
                "Infragistics does not provide barcode reading for WinForms. " +
                "BarcodeReader is only available in the WPF package.");
        }
        */

        public void DemoWinFormsCapabilities()
        {
            Console.WriteLine("Infragistics WinForms Barcode Capabilities:");
            Console.WriteLine("  - Generation: YES (UltraWinBarcode control)");
            Console.WriteLine("  - Reading: NO (not available)");
            Console.WriteLine();
            Console.WriteLine("  PROBLEM: You can create barcodes but cannot decode them.");
            Console.WriteLine("  This is a significant product gap.");
            Console.WriteLine();
            Console.WriteLine("  If you need barcode reading in WinForms with Infragistics,");
            Console.WriteLine("  you MUST use a different library.");
        }
    }

    /// <summary>
    /// Infragistics ASP.NET/Blazor Implementation - NO READING AVAILABLE
    /// Web platforms have no barcode reading capability
    /// </summary>
    public class InfragisticsWebBarcodeService
    {
        // Ignite UI for Blazor, ASP.NET
        // Requires: Infragistics Ultimate license ($1,675+/year)

        /*
        // In Blazor, can use IgbBarcode for generation
        // <IgbBarcode Type="BarcodeType.QRCode" Value="Hello World" />

        public string ReadBarcode(string imagePath)
        {
            // NOT POSSIBLE - No server-side barcode reading
            // Ignite UI web components are client-side only

            throw new NotSupportedException(
                "Infragistics does not provide server-side barcode reading. " +
                "Web barcode components are generation-only.");
        }
        */

        public void DemoWebCapabilities()
        {
            Console.WriteLine("Infragistics Web (Blazor/ASP.NET) Barcode Capabilities:");
            Console.WriteLine("  - Generation: YES (client-side IgbBarcode)");
            Console.WriteLine("  - Reading: NO (not available)");
            Console.WriteLine();
            Console.WriteLine("  PROBLEM: Cannot process uploaded barcode images on server.");
            Console.WriteLine("  This blocks many common web application scenarios:");
            Console.WriteLine("    - Document upload with barcode extraction");
            Console.WriteLine("    - API endpoints for barcode processing");
            Console.WriteLine("    - Background job barcode scanning");
        }
    }

    /// <summary>
    /// Real-world scenario: Building a multi-platform application
    /// </summary>
    public class MultiPlatformScenario
    {
        public void ShowProblem()
        {
            Console.WriteLine("Scenario: Enterprise Document Processing System");
            Console.WriteLine();
            Console.WriteLine("  Requirements:");
            Console.WriteLine("  1. WPF desktop app for scanning documents");
            Console.WriteLine("  2. WinForms legacy app for warehouse scanning");
            Console.WriteLine("  3. ASP.NET Core API for mobile uploads");
            Console.WriteLine("  4. Blazor web app for customer self-service");
            Console.WriteLine();
            Console.WriteLine("  With Infragistics Ultimate ($1,675/year per dev):");
            Console.WriteLine();
            Console.WriteLine("  ┌────────────────────────────────────────────────┐");
            Console.WriteLine("  │ Platform        │ Generation │ Reading         │");
            Console.WriteLine("  ├────────────────────────────────────────────────┤");
            Console.WriteLine("  │ WPF Desktop     │     ✓      │     ✓           │");
            Console.WriteLine("  │ WinForms Legacy │     ✓      │     ✗  BLOCKED  │");
            Console.WriteLine("  │ ASP.NET Core    │     ✗      │     ✗  BLOCKED  │");
            Console.WriteLine("  │ Blazor Web      │     ✓*     │     ✗  BLOCKED  │");
            Console.WriteLine("  └────────────────────────────────────────────────┘");
            Console.WriteLine("  * Client-side only");
            Console.WriteLine();
            Console.WriteLine("  Result: Only 1 of 4 platforms can read barcodes!");
            Console.WriteLine("  You'd need a different library for the other 3 platforms.");
        }
    }
}

// ============================================================================
// IRONBARCODE APPROACH: All Platforms Supported
// ============================================================================

namespace IronBarcodeAllPlatforms
{
    using IronBarCode;
    using System;
    using System.Linq;

    /// <summary>
    /// IronBarcode Universal Implementation
    /// The SAME code works identically on ALL platforms
    /// </summary>
    public class UniversalBarcodeService
    {
        // Install: dotnet add package IronBarcode
        // License: IronBarCode.License.LicenseKey = "YOUR-KEY";

        /// <summary>
        /// Read barcode - works on ANY platform
        /// </summary>
        public string ReadBarcode(string imagePath)
        {
            // This exact code works in:
            // - WPF applications
            // - WinForms applications
            // - ASP.NET Core APIs
            // - Blazor Server
            // - Console applications
            // - Azure Functions
            // - Docker containers

            var results = BarcodeReader.Read(imagePath);

            if (results.Any())
            {
                var barcode = results.First();
                return $"{barcode.BarcodeType}: {barcode.Text}";
            }

            return "No barcode found";
        }

        /// <summary>
        /// Generate barcode - also works everywhere
        /// </summary>
        public void GenerateBarcode(string value, string outputPath)
        {
            BarcodeWriter.CreateBarcode(value, BarcodeEncoding.Code128)
                .SaveAsPng(outputPath);
        }

        /// <summary>
        /// Read from PDF - not available in Infragistics at all
        /// </summary>
        public string[] ReadFromPdf(string pdfPath)
        {
            var results = BarcodeReader.Read(pdfPath);
            return results.Select(r => $"Page {r.PageNumber}: {r.Text}").ToArray();
        }

        public void DemoUniversalCapabilities()
        {
            Console.WriteLine("IronBarcode Universal Capabilities:");
            Console.WriteLine("  - Generation: YES (all platforms)");
            Console.WriteLine("  - Reading: YES (all platforms)");
            Console.WriteLine("  - API Style: Synchronous (simple)");
            Console.WriteLine("  - Symbologies: 50+ formats");
            Console.WriteLine("  - PDF Support: YES");
            Console.WriteLine("  - Auto Detection: YES");
            Console.WriteLine();
            Console.WriteLine("  Same code works everywhere - no platform restrictions.");
        }
    }

    /// <summary>
    /// Same multi-platform scenario - works with IronBarcode
    /// </summary>
    public class MultiPlatformSolution
    {
        public void ShowSolution()
        {
            Console.WriteLine("Same Scenario with IronBarcode:");
            Console.WriteLine();
            Console.WriteLine("  Requirements:");
            Console.WriteLine("  1. WPF desktop app for scanning documents");
            Console.WriteLine("  2. WinForms legacy app for warehouse scanning");
            Console.WriteLine("  3. ASP.NET Core API for mobile uploads");
            Console.WriteLine("  4. Blazor web app for customer self-service");
            Console.WriteLine();
            Console.WriteLine("  With IronBarcode ($749 one-time or $2,999 team):");
            Console.WriteLine();
            Console.WriteLine("  ┌────────────────────────────────────────────────┐");
            Console.WriteLine("  │ Platform        │ Generation │ Reading         │");
            Console.WriteLine("  ├────────────────────────────────────────────────┤");
            Console.WriteLine("  │ WPF Desktop     │     ✓      │     ✓           │");
            Console.WriteLine("  │ WinForms Legacy │     ✓      │     ✓           │");
            Console.WriteLine("  │ ASP.NET Core    │     ✓      │     ✓           │");
            Console.WriteLine("  │ Blazor Server   │     ✓      │     ✓           │");
            Console.WriteLine("  │ Docker/Linux    │     ✓      │     ✓           │");
            Console.WriteLine("  │ Azure Functions │     ✓      │     ✓           │");
            Console.WriteLine("  └────────────────────────────────────────────────┘");
            Console.WriteLine();
            Console.WriteLine("  Result: ALL platforms work with identical code!");
        }

        public void ShowSharedCode()
        {
            Console.WriteLine();
            Console.WriteLine("  The best part: ONE shared service class for ALL platforms");
            Console.WriteLine();

            var code = @"
    // This class is used by ALL applications
    public class BarcodeProcessingService
    {
        public BarcodeResult ProcessDocument(string filePath)
        {
            // Works in WPF, WinForms, ASP.NET, Blazor, Docker...
            var results = BarcodeReader.Read(filePath);

            return new BarcodeResult
            {
                Success = results.Any(),
                Barcodes = results.Select(r => new BarcodeInfo
                {
                    Type = r.BarcodeType.ToString(),
                    Value = r.Text,
                    Page = r.PageNumber
                }).ToList()
            };
        }
    }

    // Called from WPF:
    var service = new BarcodeProcessingService();
    var result = service.ProcessDocument(wpfFilePath);

    // Called from WinForms:
    var service = new BarcodeProcessingService();
    var result = service.ProcessDocument(winFormsFilePath);

    // Called from ASP.NET API:
    [HttpPost]
    public IActionResult ProcessUpload(IFormFile file)
    {
        var service = new BarcodeProcessingService();
        var result = service.ProcessDocument(tempFilePath);
        return Ok(result);
    }

    // Same service, same code, all platforms!";

            Console.WriteLine(code);
        }
    }
}

// ============================================================================
// COMPARISON: Platform Support Matrix
// ============================================================================

namespace PlatformComparison
{
    using System;

    public class PlatformMatrix
    {
        public static void ShowComparison()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║      INFRAGISTICS VS IRONBARCODE: PLATFORM SUPPORT MATRIX       ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  INFRAGISTICS                                                    ║");
            Console.WriteLine("║  ┌──────────────────┬────────────┬─────────────┬─────────────┐  ║");
            Console.WriteLine("║  │ Platform         │ Generation │ Reading     │ PDF Reading │  ║");
            Console.WriteLine("║  ├──────────────────┼────────────┼─────────────┼─────────────┤  ║");
            Console.WriteLine("║  │ WPF              │     ✓      │      ✓      │      ✗      │  ║");
            Console.WriteLine("║  │ WinForms         │     ✓      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ Blazor           │     ✓*     │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ ASP.NET Core     │     ✗      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ Console          │     ✗      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ Docker/Linux     │     ✗      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ Azure Functions  │     ✗      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  └──────────────────┴────────────┴─────────────┴─────────────┘  ║");
            Console.WriteLine("║  * Client-side component only                                   ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  IRONBARCODE                                                     ║");
            Console.WriteLine("║  ┌──────────────────┬────────────┬─────────────┬─────────────┐  ║");
            Console.WriteLine("║  │ Platform         │ Generation │ Reading     │ PDF Reading │  ║");
            Console.WriteLine("║  ├──────────────────┼────────────┼─────────────┼─────────────┤  ║");
            Console.WriteLine("║  │ WPF              │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ WinForms         │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ Blazor Server    │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ ASP.NET Core     │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ Console          │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ Docker/Linux     │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ Azure Functions  │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ AWS Lambda       │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  └──────────────────┴────────────┴─────────────┴─────────────┘  ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  Key Differences:                                                ║");
            Console.WriteLine("║  • Infragistics: Reading ONLY on WPF platform                   ║");
            Console.WriteLine("║  • Infragistics: No server-side barcode processing              ║");
            Console.WriteLine("║  • IronBarcode: Full read/write on ALL platforms                ║");
            Console.WriteLine("║  • IronBarcode: Native PDF support everywhere                   ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
        }
    }

    /// <summary>
    /// Cost comparison when you need multiple platforms
    /// </summary>
    public class CostAnalysis
    {
        public void ShowCostBreakdown()
        {
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 COST TO ACHIEVE FULL COVERAGE                   ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  With Infragistics (to cover all platforms):                     ║");
            Console.WriteLine("║  ┌──────────────────────────────────────────────────────────┐   ║");
            Console.WriteLine("║  │ Infragistics Ultimate     $1,675/year × 10 devs          │   ║");
            Console.WriteLine("║  │                           = $16,750/year                 │   ║");
            Console.WriteLine("║  │                                                          │   ║");
            Console.WriteLine("║  │ PLUS: Still need another library for:                    │   ║");
            Console.WriteLine("║  │   - WinForms reading                                     │   ║");
            Console.WriteLine("║  │   - ASP.NET Core reading                                 │   ║");
            Console.WriteLine("║  │   - Console/Docker reading                               │   ║");
            Console.WriteLine("║  │                                                          │   ║");
            Console.WriteLine("║  │ Total: $16,750/year + IronBarcode $2,999 one-time        │   ║");
            Console.WriteLine("║  └──────────────────────────────────────────────────────────┘   ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  With IronBarcode alone (covers ALL platforms):                  ║");
            Console.WriteLine("║  ┌──────────────────────────────────────────────────────────┐   ║");
            Console.WriteLine("║  │ IronBarcode Professional  $2,999 one-time (10 devs)      │   ║");
            Console.WriteLine("║  │                                                          │   ║");
            Console.WriteLine("║  │ Covers: WPF, WinForms, ASP.NET, Blazor, Console,         │   ║");
            Console.WriteLine("║  │         Docker, Azure Functions, AWS Lambda              │   ║");
            Console.WriteLine("║  │                                                          │   ║");
            Console.WriteLine("║  │ Total: $2,999 one-time                                   │   ║");
            Console.WriteLine("║  └──────────────────────────────────────────────────────────┘   ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  5-Year Comparison:                                              ║");
            Console.WriteLine("║    Infragistics alone: $83,750 (still incomplete!)              ║");
            Console.WriteLine("║    IronBarcode alone:  $2,999  (complete coverage)              ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
        }
    }
}

// ============================================================================
// MAIN: Run the comparisons
// ============================================================================

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Infragistics vs IronBarcode: Platform Comparison ===");
        Console.WriteLine();

        // Infragistics platform capabilities
        var wpfService = new InfragisticsPlatformLimitation.InfragisticsWpfBarcodeService();
        wpfService.DemoWpfCapabilities();
        Console.WriteLine();

        var winFormsService = new InfragisticsPlatformLimitation.InfragisticsWinFormsBarcodeService();
        winFormsService.DemoWinFormsCapabilities();
        Console.WriteLine();

        var webService = new InfragisticsPlatformLimitation.InfragisticsWebBarcodeService();
        webService.DemoWebCapabilities();
        Console.WriteLine();

        // Multi-platform problem
        Console.WriteLine("────────────────────────────────────────────────────────────");
        Console.WriteLine();

        var scenario = new InfragisticsPlatformLimitation.MultiPlatformScenario();
        scenario.ShowProblem();
        Console.WriteLine();

        // IronBarcode solution
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine();

        var ironService = new IronBarcodeAllPlatforms.UniversalBarcodeService();
        ironService.DemoUniversalCapabilities();
        Console.WriteLine();

        var solution = new IronBarcodeAllPlatforms.MultiPlatformSolution();
        solution.ShowSolution();
        solution.ShowSharedCode();
        Console.WriteLine();

        // Platform matrix
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine();

        PlatformComparison.PlatformMatrix.ShowComparison();

        // Cost analysis
        var costAnalysis = new PlatformComparison.CostAnalysis();
        costAnalysis.ShowCostBreakdown();
    }
}
