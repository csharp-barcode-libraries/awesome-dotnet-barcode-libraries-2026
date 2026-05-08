/**
 * Telerik RadBarcode Platform Inconsistency Comparison
 *
 * This example demonstrates the platform inconsistency of Telerik RadBarcode:
 * - Barcode GENERATION works on all platforms (WPF, WinForms, Blazor, ASP.NET)
 * - Barcode READING only ships on WPF and WinForms (desktop platforms)
 * - The WPF reader supports 1D + QR/PDF417/DataMatrix (no Aztec/MaxiCode/MicroQR/DotCode)
 * - The WinForms reader supports 1D ONLY (no 2D formats at all)
 * - Blazor / ASP.NET AJAX / ASP.NET Core have NO barcode reader
 *
 * Distribution: Telerik UI packages are NOT on public nuget.org. They are
 * distributed via the licensed feed at https://nuget.telerik.com/v3/index.json
 * with API-key authentication.
 *
 * IronBarcode provides consistent read/write capabilities across ALL platforms,
 * is shipped on public nuget.org as the `BarCode` package, and uses one API
 * surface everywhere.
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
    /// Namespace: Telerik.Windows.Controls.Barcode
    /// Assembly:  Telerik.Windows.Controls.DataVisualization.dll
    /// </summary>
    public class TelerikWpfBarcodeService
    {
        // Install (private feed): Telerik.UI.for.Wpf.60.Xaml
        // Requires: Telerik UI for WPF license — 2026 list price
        //   $749 (Lite) to $1,249 (Ultimate) per developer per year

        /*
        using Telerik.Windows.Controls.Barcode;
        using System.Windows.Media.Imaging;

        public string ReadBarcode(string imagePath)
        {
            // Load image as WPF BitmapImage
            var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

            // Create reader - only available in WPF and WinForms
            var reader = new RadBarcodeReader();

            // The WPF DecodeType is a [Flags] enum — combine with bitwise OR.
            // 1D formats plus QR, PDF417, and DataMatrix are supported.
            // Aztec, MaxiCode, MicroQR, and DotCode are NOT in the enum.
            reader.DecodeTypes = DecodeType.Code128
                               | DecodeType.Code39
                               | DecodeType.EAN13
                               | DecodeType.EAN8
                               | DecodeType.UPCA
                               | DecodeType.UPCE
                               | DecodeType.Codabar
                               | DecodeType.QR
                               | DecodeType.PDF417
                               | DecodeType.DataMatrix;

            // Decode the barcode
            var result = reader.Decode(bitmap);

            return result?.Text ?? "No barcode found";
        }

        public BitmapImage GenerateBarcode(string value)
        {
            // Generation works - create RadBarcode in code-behind
            var barcode = new RadBarcode();
            barcode.Value = value;
            barcode.Symbology = new Code128();

            // Render to image
            return RenderBarcode(barcode);
        }
        */

        public void DemoWpfCapabilities()
        {
            Console.WriteLine("Telerik WPF Barcode Capabilities:");
            Console.WriteLine("  - Generation: YES");
            Console.WriteLine("  - Reading (1D): YES");
            Console.WriteLine("  - Reading (QR / PDF417 / DataMatrix): YES");
            Console.WriteLine("  - Reading (Aztec / MaxiCode / MicroQR / DotCode): NO");
            Console.WriteLine("  - PDF Support: NO");
        }
    }

    /// <summary>
    /// Telerik WinForms Implementation - READING WORKS HERE
    /// Namespace: Telerik.WinControls.UI.Barcode
    /// Class:     RadBarcodeReader (1D only)
    ///
    /// Telerik docs: "Currently, all of the 1D barcodes, offered by Telerik,
    ///                are supported."
    /// </summary>
    public class TelerikWinFormsBarcodeService
    {
        // Install (private feed): Telerik.UI.for.WinForms
        // Requires: Telerik UI for WinForms license — 2026 list price
        //   $749 (Lite) to $1,249 (Ultimate) per developer per year

        /*
        using Telerik.WinControls.UI;
        using Telerik.WinControls.UI.Barcode;
        using System.Drawing;

        public string ReadBarcode(string imagePath)
        {
            // Load image as System.Drawing.Image
            using var image = Image.FromFile(imagePath);

            // Create reader - available in WinForms (1D only)
            var reader = new RadBarcodeReader();

            // The WinForms DecodeType is a [Flags] enum — 1D formats only.
            // No QR, DataMatrix, PDF417, Aztec, MaxiCode, MicroQR, DotCode.
            reader.DecodeType = DecodeType.Code128
                              | DecodeType.Code39
                              | DecodeType.EAN13
                              | DecodeType.Code11
                              | DecodeType.IntelligentMail;

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
            Console.WriteLine("  - Reading (1D): YES");
            Console.WriteLine("  - Reading (any 2D — QR, DataMatrix, PDF417, Aztec, ...): NO");
            Console.WriteLine("  - PDF Support: NO");
        }
    }

    /// <summary>
    /// Telerik Blazor Implementation - NO READING AVAILABLE
    /// Blazor TelerikBarcode is generation-only
    /// </summary>
    public class TelerikBlazorBarcodeService
    {
        // Install (private feed): Telerik.UI.for.Blazor
        // Requires: Telerik UI for Blazor license — 2026 list price
        //   $749 (Lite) to $1,249 (Ultimate) per developer per year

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
        // Install (private feed): Telerik.UI.for.AspNet.Core
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
        // Install: dotnet add package BarCode
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
                return $"Type: {barcode.BarcodeType}, Value: {barcode.Value}";
            }

            return "No barcode found";
        }

        /// <summary>
        /// Read QR codes — works on every platform, including those Telerik
        /// does not cover at all (Blazor, ASP.NET Core, ASP.NET AJAX, WinForms).
        /// </summary>
        public string ReadQrCode(string imagePath)
        {
            // QR codes read automatically - no special configuration
            var results = BarcodeReader.Read(imagePath);

            var qrCodes = results.Where(r => r.BarcodeType == BarcodeEncoding.QRCode);

            return qrCodes.FirstOrDefault()?.Value ?? "No QR code found";
        }

        /// <summary>
        /// Read barcodes from PDF documents - not available in Telerik at all
        /// </summary>
        public string[] ReadBarcodesFromPdf(string pdfPath)
        {
            // Native PDF support - reads all barcodes from all pages
            var results = BarcodeReader.Read(pdfPath);

            return results.Select(r => $"Page {r.PageNumber}: {r.Value}").ToArray();
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
            Console.WriteLine("  - QR / 2D Reading (incl. Aztec, MaxiCode, MicroQR, DotCode): YES");
            Console.WriteLine("  - PDF Support: YES");
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
            Console.WriteLine("==================================================================");
            Console.WriteLine("       TELERIK VS IRONBARCODE: PLATFORM SUPPORT MATRIX");
            Console.WriteLine("==================================================================");
            Console.WriteLine();
            Console.WriteLine("  TELERIK RADBARCODE");
            Console.WriteLine("  Platform         | Generation | 1D Reading | 2D Reading                 ");
            Console.WriteLine("  -----------------+------------+------------+----------------------------");
            Console.WriteLine("  WPF              |    YES     |    YES     | QR / PDF417 / DataMatrix only");
            Console.WriteLine("  WinForms         |    YES     |    YES     | NO (1D-only reader)        ");
            Console.WriteLine("  Blazor           |    YES     |    NO      | NO                         ");
            Console.WriteLine("  ASP.NET Core     |    YES     |    NO      | NO                         ");
            Console.WriteLine("  ASP.NET AJAX     |    YES     |    NO      | NO                         ");
            Console.WriteLine("  Docker/Linux     |    n/a     |    NO      | NO                         ");
            Console.WriteLine("  Azure Functions  |    n/a     |    NO      | NO                         ");
            Console.WriteLine();
            Console.WriteLine("  IRONBARCODE");
            Console.WriteLine("  Platform         | Generation | 1D Reading | 2D Reading                 ");
            Console.WriteLine("  -----------------+------------+------------+----------------------------");
            Console.WriteLine("  WPF              |    YES     |    YES     | YES                        ");
            Console.WriteLine("  WinForms         |    YES     |    YES     | YES                        ");
            Console.WriteLine("  Blazor           |    YES     |    YES     | YES                        ");
            Console.WriteLine("  ASP.NET Core     |    YES     |    YES     | YES                        ");
            Console.WriteLine("  Console          |    YES     |    YES     | YES                        ");
            Console.WriteLine("  Docker/Linux     |    YES     |    YES     | YES                        ");
            Console.WriteLine("  Azure Functions  |    YES     |    YES     | YES                        ");
            Console.WriteLine("  AWS Lambda       |    YES     |    YES     | YES                        ");
            Console.WriteLine();
            Console.WriteLine("  Key Differences:");
            Console.WriteLine("  - Telerik: Reading restricted to desktop (WPF / WinForms)");
            Console.WriteLine("  - Telerik: 2D reading is partial on WPF, absent on WinForms,");
            Console.WriteLine("    and unavailable on every web platform.");
            Console.WriteLine("  - Telerik: No Aztec, MaxiCode, MicroQR, or DotCode on any platform.");
            Console.WriteLine("  - IronBarcode: Same API on every platform with broader format coverage.");
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
            Console.WriteLine("  Telerik Solution: NOT POSSIBLE with Telerik UI alone");
            Console.WriteLine("  - RadBarcodeReader ships only for WPF and WinForms");
            Console.WriteLine("  - No server-side reading component on Blazor / ASP.NET Core / AJAX");
            Console.WriteLine("  - You would need a different library for the server side");
            Console.WriteLine();
            Console.WriteLine("  Result: Telerik cannot cover this common use case on its own.");
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
            r.Value,
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
