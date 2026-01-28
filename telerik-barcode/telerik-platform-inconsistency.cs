/**
 * Telerik RadBarcode Platform Inconsistency Comparison
 *
 * This example demonstrates the fundamental platform limitation of Telerik RadBarcode:
 * - Barcode GENERATION works on all platforms (WPF, WinForms, Blazor, ASP.NET)
 * - Barcode READING only works on WPF and WinForms (desktop platforms)
 * - Web applications (Blazor, ASP.NET Core, ASP.NET AJAX) have NO reading capability
 *
 * IronBarcode provides consistent read/write capabilities across ALL platforms.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Comparison: Telerik RadBarcode vs IronBarcode
 */

// ============================================================================
// TELERIK APPROACH: Platform-Specific Code Required
// ============================================================================

namespace TelerikPlatformExample
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Telerik WPF Implementation - READING WORKS HERE
    /// Only WPF and WinForms have RadBarcodeReader available
    /// </summary>
    public class TelerikWpfBarcodeService
    {
        // Install: Telerik.UI.for.Wpf.60.Xaml
        // Requires: Telerik UI for WPF license ($1,149+/year)

        /*
        using Telerik.Windows.Controls.Barcode;
        using System.Windows.Media.Imaging;

        public string ReadBarcode(string imagePath)
        {
            // Load image as WPF BitmapImage
            var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

            // Create reader - only available in WPF
            var reader = new RadBarcodeReader();

            // Must specify decode types - only 1D formats supported
            reader.DecodeTypes = new DecodeType[]
            {
                DecodeType.Code128,
                DecodeType.Code39,
                DecodeType.EAN13,
                DecodeType.EAN8,
                DecodeType.UPCA,
                DecodeType.UPCE,
                DecodeType.Codabar,
                DecodeType.ITF
                // Note: No QR, DataMatrix, PDF417 support
            };

            // Decode the barcode
            var result = reader.Decode(bitmap);

            return result?.Text ?? "No barcode found";
        }

        public BitmapImage GenerateBarcode(string value)
        {
            // Generation works - create RadBarcode in code-behind
            var barcode = new RadBarcode();
            barcode.Value = value;
            barcode.Symbology = new Code128ASettings();

            // Render to image
            return RenderBarcode(barcode);
        }
        */

        public void DemoWpfCapabilities()
        {
            Console.WriteLine("Telerik WPF Barcode Capabilities:");
            Console.WriteLine("  - Generation: YES");
            Console.WriteLine("  - Reading: YES (1D only)");
            Console.WriteLine("  - QR Reading: NO");
            Console.WriteLine("  - PDF Support: NO");
        }
    }

    /// <summary>
    /// Telerik WinForms Implementation - READING WORKS HERE
    /// WinForms also has barcode reader available
    /// </summary>
    public class TelerikWinFormsBarcodeService
    {
        // Install: Telerik.UI.for.WinForms.Barcode
        // Requires: Telerik UI for WinForms license ($1,149+/year)

        /*
        using Telerik.WinControls.UI;
        using Telerik.WinControls.UI.Barcode;

        public string ReadBarcode(string imagePath)
        {
            // Load image as System.Drawing.Image
            using var image = Image.FromFile(imagePath);

            // Create reader - available in WinForms
            var reader = new BarCodeReader();

            // Must specify barcode types to look for
            reader.DecodeType = new DecodeType[]
            {
                DecodeType.Code128,
                DecodeType.Code39,
                DecodeType.EAN13,
                DecodeType.Code11,
                DecodeType.ITF
                // Still no 2D barcode support
            };

            // Read the barcode
            var result = reader.Read(image);

            return result?.Text ?? "No barcode found";
        }

        public void GenerateBarcode(string value, string outputPath)
        {
            var barcode = new RadBarcode();
            barcode.Symbology = new Code128();
            barcode.Value = value;

            // Save as image
            barcode.ExportToImage(outputPath);
        }
        */

        public void DemoWinFormsCapabilities()
        {
            Console.WriteLine("Telerik WinForms Barcode Capabilities:");
            Console.WriteLine("  - Generation: YES");
            Console.WriteLine("  - Reading: YES (1D only)");
            Console.WriteLine("  - QR Reading: NO");
            Console.WriteLine("  - PDF Support: NO");
        }
    }

    /// <summary>
    /// Telerik Blazor Implementation - NO READING AVAILABLE
    /// Blazor TelerikBarcode is generation-only
    /// </summary>
    public class TelerikBlazorBarcodeService
    {
        // Install: Telerik.UI.for.Blazor
        // Requires: Telerik UI for Blazor license ($1,099+/year)

        /*
        // In .razor file:
        // <TelerikBarcode Value="12345678" Type="@BarcodeType.Code128" />

        // PROBLEM: There is NO reading component for Blazor
        // If you need to read barcodes in a Blazor app, Telerik cannot help you

        public string ReadBarcode(string imagePath)
        {
            // NOT POSSIBLE - No Blazor barcode reader exists
            throw new NotSupportedException(
                "Telerik does not provide barcode reading for Blazor applications. " +
                "You need a different library like IronBarcode.");
        }
        */

        public void DemoBlazorCapabilities()
        {
            Console.WriteLine("Telerik Blazor Barcode Capabilities:");
            Console.WriteLine("  - Generation: YES");
            Console.WriteLine("  - Reading: NO (not available)");
            Console.WriteLine("  - QR Reading: NO");
            Console.WriteLine("  - PDF Support: NO");
            Console.WriteLine();
            Console.WriteLine("  WARNING: If you need barcode reading in Blazor,");
            Console.WriteLine("  you MUST use a different library.");
        }
    }

    /// <summary>
    /// Telerik ASP.NET Core Implementation - NO READING AVAILABLE
    /// ASP.NET Core barcode component is generation-only
    /// </summary>
    public class TelerikAspNetCoreBarcodeService
    {
        // Install: Telerik.UI.for.AspNet.Core
        // Requires: Telerik UI for ASP.NET Core license

        /*
        // In Razor view:
        // @(Html.Kendo().Barcode()
        //     .Name("barcode")
        //     .Value("12345678")
        //     .Type(BarcodeSymbology.Code128))

        public string ReadBarcode(string imagePath)
        {
            // NOT POSSIBLE - No ASP.NET Core barcode reader exists
            throw new NotSupportedException(
                "Telerik does not provide barcode reading for ASP.NET Core. " +
                "Consider IronBarcode for server-side barcode reading.");
        }
        */

        public void DemoAspNetCoreCapabilities()
        {
            Console.WriteLine("Telerik ASP.NET Core Barcode Capabilities:");
            Console.WriteLine("  - Generation: YES");
            Console.WriteLine("  - Reading: NO (not available)");
            Console.WriteLine("  - QR Reading: NO");
            Console.WriteLine("  - PDF Support: NO");
            Console.WriteLine();
            Console.WriteLine("  WARNING: Server-side barcode reading not possible.");
        }
    }
}

// ============================================================================
// IRONBARCODE APPROACH: One Codebase for All Platforms
// ============================================================================

namespace IronBarcodePlatformExample
{
    using IronBarCode;
    using System;
    using System.Linq;

    /// <summary>
    /// IronBarcode Universal Implementation
    /// The SAME code works on ALL platforms:
    /// - Console applications
    /// - WPF applications
    /// - WinForms applications
    /// - Blazor Server
    /// - ASP.NET Core
    /// - Azure Functions
    /// - Docker containers
    /// - Linux servers
    /// </summary>
    public class UniversalBarcodeService
    {
        // Install: dotnet add package IronBarcode
        // License: IronBarCode.License.LicenseKey = "YOUR-KEY";

        /// <summary>
        /// Read any barcode from any image - works on ALL platforms
        /// </summary>
        public string ReadBarcode(string imagePath)
        {
            // One line - automatic format detection
            // Works in: Console, WPF, WinForms, Blazor, ASP.NET, Docker, Azure
            var results = BarcodeReader.Read(imagePath);

            if (results.Any())
            {
                var barcode = results.First();
                return $"Type: {barcode.BarcodeType}, Value: {barcode.Text}";
            }

            return "No barcode found";
        }

        /// <summary>
        /// Read QR codes - impossible with Telerik, trivial with IronBarcode
        /// </summary>
        public string ReadQrCode(string imagePath)
        {
            // QR codes read automatically - no special configuration
            var results = BarcodeReader.Read(imagePath);

            var qrCodes = results.Where(r => r.BarcodeType == BarcodeEncoding.QRCode);

            return qrCodes.FirstOrDefault()?.Text ?? "No QR code found";
        }

        /// <summary>
        /// Read barcodes from PDF documents - not available in Telerik at all
        /// </summary>
        public string[] ReadBarcodesFromPdf(string pdfPath)
        {
            // Native PDF support - reads all barcodes from all pages
            var results = BarcodeReader.Read(pdfPath);

            return results.Select(r => $"Page {r.PageNumber}: {r.Text}").ToArray();
        }

        /// <summary>
        /// Generate barcode - same API on all platforms
        /// </summary>
        public void GenerateBarcode(string value, string outputPath)
        {
            // One line generation
            BarcodeWriter.CreateBarcode(value, BarcodeEncoding.Code128)
                .SaveAsPng(outputPath);
        }

        /// <summary>
        /// Generate QR code with logo - not available in Telerik
        /// </summary>
        public void GenerateQrCodeWithLogo(string value, string logoPath, string outputPath)
        {
            var qr = QRCodeWriter.CreateQrCodeWithLogo(value, logoPath, 500);
            qr.SaveAsPng(outputPath);
        }

        public void DemoUniversalCapabilities()
        {
            Console.WriteLine("IronBarcode Universal Capabilities:");
            Console.WriteLine("  - Generation: YES (all platforms)");
            Console.WriteLine("  - Reading: YES (all platforms)");
            Console.WriteLine("  - QR/2D Reading: YES");
            Console.WriteLine("  - PDF Support: YES");
            Console.WriteLine("  - ML Error Correction: YES");
            Console.WriteLine("  - Automatic Detection: YES");
            Console.WriteLine();
            Console.WriteLine("  Same code works everywhere - no platform restrictions.");
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
            Console.WriteLine("║        TELERIK VS IRONBARCODE: PLATFORM SUPPORT MATRIX          ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  TELERIK RADBARCODE                                              ║");
            Console.WriteLine("║  ┌──────────────────┬────────────┬─────────────┬─────────────┐  ║");
            Console.WriteLine("║  │ Platform         │ Generation │ 1D Reading  │ 2D Reading  │  ║");
            Console.WriteLine("║  ├──────────────────┼────────────┼─────────────┼─────────────┤  ║");
            Console.WriteLine("║  │ WPF              │     ✓      │      ✓      │      ✗      │  ║");
            Console.WriteLine("║  │ WinForms         │     ✓      │      ✓      │      ✗      │  ║");
            Console.WriteLine("║  │ Blazor           │     ✓      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ ASP.NET Core     │     ✓      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ ASP.NET AJAX     │     ✓      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ Docker/Linux     │     ?      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  │ Azure Functions  │     ?      │      ✗      │      ✗      │  ║");
            Console.WriteLine("║  └──────────────────┴────────────┴─────────────┴─────────────┘  ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  IRONBARCODE                                                     ║");
            Console.WriteLine("║  ┌──────────────────┬────────────┬─────────────┬─────────────┐  ║");
            Console.WriteLine("║  │ Platform         │ Generation │ 1D Reading  │ 2D Reading  │  ║");
            Console.WriteLine("║  ├──────────────────┼────────────┼─────────────┼─────────────┤  ║");
            Console.WriteLine("║  │ WPF              │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ WinForms         │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ Blazor           │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ ASP.NET Core     │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ Console          │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ Docker/Linux     │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ Azure Functions  │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  │ AWS Lambda       │     ✓      │      ✓      │      ✓      │  ║");
            Console.WriteLine("║  └──────────────────┴────────────┴─────────────┴─────────────┘  ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║  Key Differences:                                                ║");
            Console.WriteLine("║  • Telerik: Reading restricted to desktop (WPF/WinForms)        ║");
            Console.WriteLine("║  • Telerik: No 2D barcode reading on ANY platform               ║");
            Console.WriteLine("║  • IronBarcode: Full read/write on ALL platforms                ║");
            Console.WriteLine("║  • IronBarcode: 50+ formats including all 2D types              ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
        }
    }

    /// <summary>
    /// Real-world scenario: Building a document processing service
    /// that needs to run on a Linux server or in Docker
    /// </summary>
    public class ServerSideBarcodeProcessing
    {
        public void TelerikApproach()
        {
            Console.WriteLine("Telerik Server-Side Barcode Processing:");
            Console.WriteLine();
            Console.WriteLine("  Problem: You need to build an API that reads barcodes from");
            Console.WriteLine("  uploaded documents on an ASP.NET Core server running in Docker.");
            Console.WriteLine();
            Console.WriteLine("  Telerik Solution: NOT POSSIBLE");
            Console.WriteLine("  - RadBarcodeReader only works on WPF/WinForms");
            Console.WriteLine("  - No server-side reading capability");
            Console.WriteLine("  - You would need to use a different library");
            Console.WriteLine();
            Console.WriteLine("  Result: Telerik cannot help with this common use case.");
        }

        public void IronBarcodeApproach()
        {
            Console.WriteLine("IronBarcode Server-Side Barcode Processing:");
            Console.WriteLine();
            Console.WriteLine("  Solution: Simple and works on any platform");
            Console.WriteLine();

            // This code runs anywhere - ASP.NET Core, Docker, Linux, Windows
            var code = @"
    [HttpPost]
    public IActionResult ProcessDocument(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var tempPath = Path.GetTempFileName();

        // Save uploaded file
        using (var fs = System.IO.File.Create(tempPath))
        {
            stream.CopyTo(fs);
        }

        // Read barcodes - works on Linux, Docker, anywhere
        var results = BarcodeReader.Read(tempPath);

        return Ok(results.Select(r => new {
            r.BarcodeType,
            r.Text,
            r.PageNumber
        }));
    }";

            Console.WriteLine(code);
            Console.WriteLine();
            Console.WriteLine("  Result: Full barcode reading in any deployment environment.");
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
        Console.WriteLine("=== Telerik RadBarcode vs IronBarcode: Platform Comparison ===");
        Console.WriteLine();

        // Show platform capabilities
        var telerikWpf = new TelerikPlatformExample.TelerikWpfBarcodeService();
        telerikWpf.DemoWpfCapabilities();
        Console.WriteLine();

        var telerikWinForms = new TelerikPlatformExample.TelerikWinFormsBarcodeService();
        telerikWinForms.DemoWinFormsCapabilities();
        Console.WriteLine();

        var telerikBlazor = new TelerikPlatformExample.TelerikBlazorBarcodeService();
        telerikBlazor.DemoBlazorCapabilities();
        Console.WriteLine();

        var telerikAspNet = new TelerikPlatformExample.TelerikAspNetCoreBarcodeService();
        telerikAspNet.DemoAspNetCoreCapabilities();
        Console.WriteLine();

        Console.WriteLine("────────────────────────────────────────────────────────────");
        Console.WriteLine();

        var ironBarcode = new IronBarcodePlatformExample.UniversalBarcodeService();
        ironBarcode.DemoUniversalCapabilities();
        Console.WriteLine();

        Console.WriteLine("────────────────────────────────────────────────────────────");
        Console.WriteLine();

        // Show matrix comparison
        PlatformComparison.PlatformMatrix.ShowComparison();
        Console.WriteLine();

        // Real-world scenario
        Console.WriteLine("=== Real-World Scenario: Server-Side Processing ===");
        Console.WriteLine();
        var serverScenario = new PlatformComparison.ServerSideBarcodeProcessing();
        serverScenario.TelerikApproach();
        Console.WriteLine();
        serverScenario.IronBarcodeApproach();
    }
}
