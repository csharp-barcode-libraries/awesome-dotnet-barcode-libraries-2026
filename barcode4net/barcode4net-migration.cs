/**
 * Barcode4NET to IronBarcode Migration Guide
 *
 * This file provides complete before/after examples for migrating
 * from the end-of-life Barcode4NET to IronBarcode.
 *
 * Migration benefits:
 * - Active product with available licenses
 * - Modern .NET support (6, 7, 8, 9)
 * - NuGet package distribution
 * - Barcode reading capability (new!)
 * - PDF support (new!)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

// IronBarcode - modern, actively supported
// Install: dotnet add package IronBarcode
using IronBarcode;

namespace Barcode4NetMigrationExample
{
    #region API Mapping

    /// <summary>
    /// API mapping reference between Barcode4NET and IronBarcode
    /// </summary>
    public static class ApiMappingReference
    {
        public static void ShowApiMapping()
        {
            Console.WriteLine("=== API Mapping Reference ===\n");

            var mappings = new[]
            {
                ("Barcode4NET", "IronBarcode", "Notes"),
                ("---", "---", "---"),
                ("new Barcode()", "BarcodeWriter.CreateBarcode()", "Static method"),
                ("barcode.Symbology = ...", "BarcodeEncoding.X", "Enum parameter"),
                ("barcode.Data = ...", "First parameter", "Inline"),
                ("barcode.Width", "barcode.ResizeTo(width, height)", "Method call"),
                ("barcode.Height", "barcode.ResizeTo(width, height)", "Method call"),
                ("GenerateBarcode()", "SaveAsPng(), ToBitmap()", "Multiple options"),
                ("Symbology.Code128", "BarcodeEncoding.Code128", "Similar naming"),
                ("Symbology.QRCode", "BarcodeEncoding.QRCode", "Similar naming"),
                ("N/A", "BarcodeReader.Read()", "NEW: Reading!"),
                ("N/A", "PDF support", "NEW: Documents!"),
            };

            foreach (var (b4n, ib, notes) in mappings)
            {
                Console.WriteLine($"{b4n,-30} {ib,-35} {notes}");
            }
        }

        public static void ShowSymbologyMapping()
        {
            Console.WriteLine("\n=== Symbology Mapping ===\n");

            var symbologies = new[]
            {
                ("Barcode4NET", "IronBarcode"),
                ("---", "---"),
                ("Symbology.Code128", "BarcodeEncoding.Code128"),
                ("Symbology.Code39", "BarcodeEncoding.Code39"),
                ("Symbology.Code93", "BarcodeEncoding.Code93"),
                ("Symbology.EAN13", "BarcodeEncoding.EAN13"),
                ("Symbology.EAN8", "BarcodeEncoding.EAN8"),
                ("Symbology.UPCA", "BarcodeEncoding.UPCA"),
                ("Symbology.UPCE", "BarcodeEncoding.UPCE"),
                ("Symbology.QRCode", "BarcodeEncoding.QRCode"),
                ("Symbology.DataMatrix", "BarcodeEncoding.DataMatrix"),
                ("Symbology.PDF417", "BarcodeEncoding.PDF417"),
                ("Symbology.Aztec", "BarcodeEncoding.Aztec"),
                ("Symbology.ITF14", "BarcodeEncoding.ITF"),
                ("Symbology.Codabar", "BarcodeEncoding.Codabar"),
            };

            foreach (var (b4n, ib) in symbologies)
            {
                Console.WriteLine($"{b4n,-30} {ib}");
            }
        }
    }

    #endregion

    #region Basic Migration Examples

    /// <summary>
    /// Basic barcode generation migration
    /// </summary>
    public class GenerationMigration
    {
        /// <summary>
        /// BEFORE: Barcode4NET code pattern
        /// </summary>
        public void ShowOldCode()
        {
            Console.WriteLine("\n=== OLD CODE (Barcode4NET) ===\n");

            var code = @"
// Barcode4NET - .NET Framework only, manual DLL
using Barcode4NET;
using System.Drawing;

public class BarcodeService
{
    public Bitmap GenerateCode128(string data)
    {
        var barcode = new Barcode4NET.Barcode();
        barcode.Symbology = Symbology.Code128;
        barcode.Data = data;
        barcode.Width = 300;
        barcode.Height = 100;

        return barcode.GenerateBarcode();
    }

    public void SaveBarcode(string data, string outputPath)
    {
        using (var bitmap = GenerateCode128(data))
        {
            bitmap.Save(outputPath);
        }
    }
}
";
            Console.WriteLine(code);
        }

        /// <summary>
        /// AFTER: IronBarcode code
        /// </summary>
        public void GenerateBarcode(string data, string outputPath)
        {
            // IronBarcode - simple, fluent API
            var barcode = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128);
            barcode.ResizeTo(300, 100);
            barcode.SaveAsPng(outputPath);
        }

        public void ShowNewCode()
        {
            Console.WriteLine("\n=== NEW CODE (IronBarcode) ===\n");

            var code = @"
// IronBarcode - all .NET platforms, NuGet package
using IronBarcode;

public class BarcodeService
{
    public void GenerateCode128(string data, string outputPath)
    {
        var barcode = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128);
        barcode.ResizeTo(300, 100);
        barcode.SaveAsPng(outputPath);
    }

    // Or get as bytes for web response, database storage, etc.
    public byte[] GenerateCode128Bytes(string data)
    {
        var barcode = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128);
        return barcode.ToPngBinaryData();
    }
}
";
            Console.WriteLine(code);
        }
    }

    /// <summary>
    /// Multiple format generation
    /// </summary>
    public class MultiFormatGeneration
    {
        /// <summary>
        /// BEFORE: Barcode4NET switching symbologies
        /// </summary>
        public void ShowOldPattern()
        {
            Console.WriteLine("\n=== OLD PATTERN (Multiple Formats) ===\n");

            var code = @"
// Barcode4NET - each format requires property change
var barcode = new Barcode4NET.Barcode();

// Generate Code 128
barcode.Symbology = Symbology.Code128;
barcode.Data = ""12345"";
var code128 = barcode.GenerateBarcode();

// Generate QR Code - same object, change property
barcode.Symbology = Symbology.QRCode;
barcode.Data = ""https://example.com"";
var qrCode = barcode.GenerateBarcode();
";
            Console.WriteLine(code);
        }

        /// <summary>
        /// AFTER: IronBarcode unified approach
        /// </summary>
        public void GenerateMultipleFormats()
        {
            Console.WriteLine("\n=== NEW PATTERN (Multiple Formats) ===\n");

            // IronBarcode - each call is independent, clean pattern
            var code128 = BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128);
            var qrCode = BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode);
            var ean13 = BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13);

            Console.WriteLine("Generated 3 different format barcodes with same API pattern");
            Console.WriteLine($"  Code128: {code128.ToPngBinaryData().Length} bytes");
            Console.WriteLine($"  QR Code: {qrCode.ToPngBinaryData().Length} bytes");
            Console.WriteLine($"  EAN-13: {ean13.ToPngBinaryData().Length} bytes");
        }

        public void ShowNewCode()
        {
            var code = @"
// IronBarcode - each barcode is independent call
var code128 = BarcodeWriter.CreateBarcode(""12345"", BarcodeEncoding.Code128);
var qrCode = BarcodeWriter.CreateBarcode(""https://example.com"", BarcodeEncoding.QRCode);
var ean13 = BarcodeWriter.CreateBarcode(""5901234123457"", BarcodeEncoding.EAN13);

// Save all
code128.SaveAsPng(""code128.png"");
qrCode.SaveAsPng(""qrcode.png"");
ean13.SaveAsPng(""ean13.png"");
";
            Console.WriteLine(code);
        }
    }

    #endregion

    #region New Capabilities

    /// <summary>
    /// Demonstrates capabilities not available in Barcode4NET
    /// </summary>
    public class NewCapabilities
    {
        /// <summary>
        /// Barcode reading - NOT in Barcode4NET
        /// </summary>
        public void DemonstrateBarcodeReading()
        {
            Console.WriteLine("\n=== NEW: Barcode Reading ===\n");

            Console.WriteLine("Barcode4NET: CANNOT read barcodes");
            Console.WriteLine("IronBarcode: Full reading capability\n");

            var code = @"
// Read barcodes from images - NEW capability
var results = BarcodeReader.Read(""barcode.png"");

foreach (var barcode in results)
{
    Console.WriteLine($""Type: {barcode.BarcodeType}"");
    Console.WriteLine($""Value: {barcode.Value}"");
}

// Verify your generated barcodes
var generated = BarcodeWriter.CreateBarcode(""TEST123"", BarcodeEncoding.Code128);
generated.SaveAsPng(""test.png"");

var verified = BarcodeReader.Read(""test.png"");
bool correct = verified.First().Value == ""TEST123"";
";
            Console.WriteLine(code);

            // Actually demonstrate
            var qr = BarcodeWriter.CreateBarcode("Migration Complete", BarcodeEncoding.QRCode);
            var bytes = qr.ToPngBinaryData();

            using var stream = new MemoryStream(bytes);
            var readBack = BarcodeReader.Read(stream);

            Console.WriteLine($"\nLive demo: Generated QR, read back: \"{readBack.First().Value}\"");
        }

        /// <summary>
        /// PDF support - NOT in Barcode4NET
        /// </summary>
        public void DemonstratePdfSupport()
        {
            Console.WriteLine("\n=== NEW: PDF Support ===\n");

            Console.WriteLine("Barcode4NET: No PDF capability");
            Console.WriteLine("IronBarcode: Native PDF read and write\n");

            var code = @"
// Generate barcode as PDF - NEW
var barcode = BarcodeWriter.CreateBarcode(""INV-2026-001"", BarcodeEncoding.QRCode);
barcode.SaveAsPdf(""invoice-barcode.pdf"");

// Read barcodes from existing PDFs - NEW
var results = BarcodeReader.Read(""multi-page-document.pdf"");

foreach (var found in results)
{
    Console.WriteLine($""Page {found.PageNumber}: {found.Value}"");
}
";
            Console.WriteLine(code);
        }

        /// <summary>
        /// Automatic format detection - NOT in Barcode4NET
        /// </summary>
        public void DemonstrateAutoDetection()
        {
            Console.WriteLine("\n=== NEW: Automatic Format Detection ===\n");

            Console.WriteLine("Barcode4NET: Must know format in advance");
            Console.WriteLine("IronBarcode: Auto-detects from 50+ formats\n");

            var code = @"
// Just read - IronBarcode figures out the format
var results = BarcodeReader.Read(""unknown-barcode.png"");

// Format is detected automatically
Console.WriteLine($""Detected: {results.First().BarcodeType}"");
// Could be: Code128, QRCode, EAN13, DataMatrix, PDF417, Aztec, etc.
";
            Console.WriteLine(code);
        }

        /// <summary>
        /// Cross-platform - NOT in Barcode4NET
        /// </summary>
        public void DemonstrateCrossPlatform()
        {
            Console.WriteLine("\n=== NEW: Cross-Platform Deployment ===\n");

            Console.WriteLine("Barcode4NET deployments:");
            Console.WriteLine("  X  Windows only (.NET Framework)");
            Console.WriteLine("  X  No Linux support");
            Console.WriteLine("  X  No macOS support");
            Console.WriteLine("  X  No Docker support");
            Console.WriteLine();

            Console.WriteLine("IronBarcode deployments:");
            Console.WriteLine("  +  Windows (x64, x86, ARM)");
            Console.WriteLine("  +  Linux (x64, ARM)");
            Console.WriteLine("  +  macOS (Intel, Apple Silicon)");
            Console.WriteLine("  +  Docker containers");
            Console.WriteLine("  +  Azure App Service");
            Console.WriteLine("  +  AWS Lambda");
            Console.WriteLine("  +  Google Cloud Functions");
        }
    }

    #endregion

    #region Common Scenarios

    /// <summary>
    /// WinForms application migration
    /// </summary>
    public class WinFormsMigration
    {
        public void ShowMigration()
        {
            Console.WriteLine("\n=== WinForms Migration ===\n");

            var oldCode = @"
// Old WinForms - Barcode4NET
private void btnGenerate_Click(object sender, EventArgs e)
{
    var barcode = new Barcode4NET.Barcode();
    barcode.Symbology = Symbology.QRCode;
    barcode.Data = textBoxData.Text;

    pictureBoxBarcode.Image = barcode.GenerateBarcode();
}
";
            Console.WriteLine("Before (Barcode4NET):");
            Console.WriteLine(oldCode);

            var newCode = @"
// New WinForms - IronBarcode
private void btnGenerate_Click(object sender, EventArgs e)
{
    var barcode = BarcodeWriter.CreateBarcode(
        textBoxData.Text,
        BarcodeEncoding.QRCode);

    using var stream = new MemoryStream(barcode.ToPngBinaryData());
    pictureBoxBarcode.Image = Image.FromStream(stream);
}
";
            Console.WriteLine("After (IronBarcode):");
            Console.WriteLine(newCode);
        }
    }

    /// <summary>
    /// Web application migration
    /// </summary>
    public class WebMigration
    {
        public void ShowMigration()
        {
            Console.WriteLine("\n=== Web Application Migration ===\n");

            Console.WriteLine("Barcode4NET: ASP.NET Framework only");
            Console.WriteLine("IronBarcode: ASP.NET Core and Framework\n");

            var code = @"
// ASP.NET Core Controller - now possible with IronBarcode
[ApiController]
[Route(""api/[controller]"")]
public class BarcodeController : ControllerBase
{
    [HttpGet(""{data}"")]
    public IActionResult Generate(string data, [FromQuery] string format = ""Code128"")
    {
        var encoding = format switch
        {
            ""QR"" => BarcodeEncoding.QRCode,
            ""EAN13"" => BarcodeEncoding.EAN13,
            _ => BarcodeEncoding.Code128
        };

        var barcode = BarcodeWriter.CreateBarcode(data, encoding);
        return File(barcode.ToPngBinaryData(), ""image/png"");
    }

    [HttpPost(""scan"")]
    public async Task<IActionResult> Scan(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var results = BarcodeReader.Read(stream);

        return Ok(results.Select(r => new {
            Type = r.BarcodeType.ToString(),
            Value = r.Value
        }));
    }
}
";
            Console.WriteLine(code);
        }
    }

    #endregion

    #region Migration Steps

    /// <summary>
    /// Complete migration steps
    /// </summary>
    public class MigrationSteps
    {
        public void ShowSteps()
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("MIGRATION STEPS");
            Console.WriteLine("========================================\n");

            Console.WriteLine("Step 1: Install IronBarcode");
            Console.WriteLine("  dotnet add package IronBarcode");
            Console.WriteLine();

            Console.WriteLine("Step 2: Remove Barcode4NET DLLs");
            Console.WriteLine("  - Delete ThirdParty/Barcode4NET/ folder");
            Console.WriteLine("  - Remove manual references from .csproj");
            Console.WriteLine("  - Remove DLL copy steps from build scripts");
            Console.WriteLine();

            Console.WriteLine("Step 3: Update using statements");
            Console.WriteLine("  Remove: using Barcode4NET;");
            Console.WriteLine("  Add:    using IronBarcode;");
            Console.WriteLine();

            Console.WriteLine("Step 4: Replace generation code");
            Console.WriteLine("  Old: new Barcode4NET.Barcode() + properties");
            Console.WriteLine("  New: BarcodeWriter.CreateBarcode(data, encoding)");
            Console.WriteLine();

            Console.WriteLine("Step 5: Update output handling");
            Console.WriteLine("  Old: GenerateBarcode() returns Bitmap");
            Console.WriteLine("  New: SaveAsPng() or ToPngBinaryData()");
            Console.WriteLine();

            Console.WriteLine("Step 6: Add reading if beneficial");
            Console.WriteLine("  New: BarcodeReader.Read() for verification");
            Console.WriteLine();

            Console.WriteLine("Step 7: Update target framework (optional)");
            Console.WriteLine("  Change: net472 to net8.0");
            Console.WriteLine("  Benefit: Modern .NET performance and features");
        }
    }

    #endregion

    /// <summary>
    /// Entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("Barcode4NET to IronBarcode Migration");
            Console.WriteLine("========================================\n");

            // Show API mappings
            ApiMappingReference.ShowApiMapping();
            ApiMappingReference.ShowSymbologyMapping();

            // Show basic migration
            var basic = new GenerationMigration();
            basic.ShowOldCode();
            basic.ShowNewCode();

            // Show multi-format
            var multi = new MultiFormatGeneration();
            multi.ShowOldPattern();
            multi.GenerateMultipleFormats();

            // Show new capabilities
            var newCaps = new NewCapabilities();
            newCaps.DemonstrateBarcodeReading();
            newCaps.DemonstratePdfSupport();
            newCaps.DemonstrateAutoDetection();
            newCaps.DemonstrateCrossPlatform();

            // Show common scenarios
            var winforms = new WinFormsMigration();
            winforms.ShowMigration();

            var web = new WebMigration();
            web.ShowMigration();

            // Show migration steps
            var steps = new MigrationSteps();
            steps.ShowSteps();

            Console.WriteLine("\n========================================");
            Console.WriteLine("Migration provides: Active licensing,");
            Console.WriteLine("NuGet distribution, reading capability,");
            Console.WriteLine("PDF support, and modern .NET.");
            Console.WriteLine("========================================");
        }
    }
}
