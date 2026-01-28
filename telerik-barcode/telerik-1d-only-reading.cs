/**
 * Telerik RadBarcodeReader 1D-Only Limitation
 *
 * This example demonstrates the format limitation of Telerik RadBarcodeReader:
 * - RadBarcodeReader ONLY supports 1D linear barcode formats
 * - QR codes, DataMatrix, PDF417, Aztec codes CANNOT be read
 * - These 2D formats CAN be generated but NOT recognized
 *
 * IronBarcode reads ALL formats (50+) with automatic detection.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Comparison: Telerik RadBarcode vs IronBarcode
 */

// ============================================================================
// TELERIK APPROACH: 1D Barcodes Only
// ============================================================================

namespace TelerikFormatLimitation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Telerik RadBarcodeReader Format Support (WPF version)
    /// Only 1D linear barcodes can be read
    /// </summary>
    public class TelerikBarcodeReader
    {
        // These are the ONLY formats RadBarcodeReader supports:
        public static readonly string[] SupportedFormats = new[]
        {
            "Code128",
            "Code39",
            "Code39Extended",
            "Code93",
            "Code93Extended",
            "Code11",
            "Code25Standard",
            "Code25Interleaved",
            "EAN13",
            "EAN8",
            "UPCA",
            "UPCE",
            "Codabar",
            "ITF",
            "MSI"
        };

        // These formats CANNOT be read (only generated):
        public static readonly string[] UnsupportedFormats = new[]
        {
            "QRCode",           // Very common - not readable
            "DataMatrix",       // Industrial standard - not readable
            "PDF417",           // Documents/IDs - not readable
            "MicroQR",          // Compact QR - not readable
            "Aztec",            // Tickets/boarding passes - not readable
            "MaxiCode"          // Shipping - not readable
        };

        /*
        using Telerik.Windows.Controls.Barcode;
        using System.Windows.Media.Imaging;

        public string TryReadQrCode(string imagePath)
        {
            var bitmap = new BitmapImage(new Uri(imagePath));
            var reader = new RadBarcodeReader();

            // You MUST specify DecodeTypes, but QR is not an option
            reader.DecodeTypes = new DecodeType[]
            {
                DecodeType.Code128,
                DecodeType.Code39,
                DecodeType.EAN13
                // DecodeType.QR - DOES NOT EXIST
                // DecodeType.DataMatrix - DOES NOT EXIST
                // DecodeType.PDF417 - DOES NOT EXIST
            };

            var result = reader.Decode(bitmap);

            // If the image contains a QR code, result will be null
            // because RadBarcodeReader cannot read 2D formats
            return result?.Text; // Returns null for QR codes
        }

        public string Read1DBarcode(string imagePath)
        {
            var bitmap = new BitmapImage(new Uri(imagePath));
            var reader = new RadBarcodeReader();

            // Only these 1D types work
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
            };

            var result = reader.Decode(bitmap);
            return result?.Text;
        }
        */

        public void ShowFormatLimitation()
        {
            Console.WriteLine("Telerik RadBarcodeReader Format Support:");
            Console.WriteLine();

            Console.WriteLine("SUPPORTED (Can Read):");
            foreach (var format in SupportedFormats)
            {
                Console.WriteLine($"  [OK] {format}");
            }

            Console.WriteLine();
            Console.WriteLine("NOT SUPPORTED (Can Generate, Cannot Read):");
            foreach (var format in UnsupportedFormats)
            {
                Console.WriteLine($"  [!!] {format}");
            }

            Console.WriteLine();
            Console.WriteLine("This means:");
            Console.WriteLine("  - If someone sends you a QR code, you cannot decode it");
            Console.WriteLine("  - If you generate a QR code, you cannot verify it");
            Console.WriteLine("  - DataMatrix codes on products cannot be scanned");
            Console.WriteLine("  - PDF417 on IDs/licenses cannot be read");
        }
    }

    /// <summary>
    /// Demonstrates the problem: You can generate QR codes but not read them
    /// </summary>
    public class QrCodeParadox
    {
        /*
        using Telerik.Windows.Controls.Barcode;

        public void GenerateQrCode(string value)
        {
            // This WORKS - QR generation is supported
            var barcode = new RadBarcode();
            barcode.Value = value;
            barcode.Symbology = new QRCode()
            {
                ErrorCorrectionLevel = ErrorCorrectionLevel.H
            };

            // Save the QR code...
            Console.WriteLine("QR code generated successfully!");
        }

        public string ReadQrCode(string imagePath)
        {
            // This FAILS - QR reading is NOT supported
            var bitmap = new BitmapImage(new Uri(imagePath));
            var reader = new RadBarcodeReader();

            // There is no DecodeType.QR option
            // reader.DecodeTypes = new[] { DecodeType.QR }; // DOES NOT EXIST

            var result = reader.Decode(bitmap);
            // Result is always null for QR codes

            throw new NotSupportedException(
                "Telerik RadBarcodeReader cannot read QR codes. " +
                "You can generate them, but you cannot decode them.");
        }
        */

        public void DemonstrateParadox()
        {
            Console.WriteLine("The Telerik QR Code Paradox:");
            Console.WriteLine();
            Console.WriteLine("  1. You create a QR code with RadBarcode");
            Console.WriteLine("     - RadBarcode supports QR code generation");
            Console.WriteLine("     - QR code is saved to file");
            Console.WriteLine("     - Works perfectly!");
            Console.WriteLine();
            Console.WriteLine("  2. User sends you back the same QR code image");
            Console.WriteLine("     - You need to read the value");
            Console.WriteLine("     - RadBarcodeReader does not support QR codes");
            Console.WriteLine("     - You cannot decode the QR code you created");
            Console.WriteLine();
            Console.WriteLine("  This is a fundamental product limitation.");
            Console.WriteLine("  You would need a different library for QR reading.");
        }
    }

    /// <summary>
    /// Real-world scenario: Inventory management with mixed barcode types
    /// </summary>
    public class InventoryScenario
    {
        public void ShowProblem()
        {
            Console.WriteLine("Real-World Scenario: Mixed Inventory Barcodes");
            Console.WriteLine();
            Console.WriteLine("  Your warehouse has products with different barcode types:");
            Console.WriteLine();
            Console.WriteLine("  [Product A] EAN-13: 5901234123457");
            Console.WriteLine("  [Product B] Code 128: PROD-2024-001");
            Console.WriteLine("  [Product C] QR Code: {\"sku\":\"SKU123\",\"batch\":\"B001\"}");
            Console.WriteLine("  [Product D] DataMatrix: 01034567890123451721...");
            Console.WriteLine();
            Console.WriteLine("  With Telerik RadBarcodeReader:");
            Console.WriteLine("    [OK] Product A - EAN-13 works");
            Console.WriteLine("    [OK] Product B - Code 128 works");
            Console.WriteLine("    [!!] Product C - QR Code FAILS");
            Console.WriteLine("    [!!] Product D - DataMatrix FAILS");
            Console.WriteLine();
            Console.WriteLine("  You can only scan 50% of your inventory!");
            Console.WriteLine("  Modern warehouses increasingly use 2D codes for more data.");
        }
    }
}

// ============================================================================
// IRONBARCODE APPROACH: All Formats Supported
// ============================================================================

namespace IronBarcodeAllFormats
{
    using IronBarCode;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// IronBarcode reads ALL barcode formats with automatic detection
    /// </summary>
    public class UniversalBarcodeReader
    {
        // Install: dotnet add package IronBarcode
        // License: IronBarCode.License.LicenseKey = "YOUR-KEY";

        /// <summary>
        /// Read any barcode - format detected automatically
        /// </summary>
        public List<BarcodeResult> ReadAnyBarcode(string imagePath)
        {
            // One line - reads any of 50+ formats
            // No need to specify format type
            var results = BarcodeReader.Read(imagePath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Found: {barcode.BarcodeType} = {barcode.Text}");
            }

            return results.ToList();
        }

        /// <summary>
        /// Specifically read QR codes
        /// </summary>
        public string ReadQrCode(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            var qrCode = results.FirstOrDefault(r =>
                r.BarcodeType == BarcodeEncoding.QRCode);

            return qrCode?.Text ?? "No QR code found";
        }

        /// <summary>
        /// Read DataMatrix codes (commonly used in manufacturing/healthcare)
        /// </summary>
        public string ReadDataMatrix(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            var dataMatrix = results.FirstOrDefault(r =>
                r.BarcodeType == BarcodeEncoding.DataMatrix);

            return dataMatrix?.Text ?? "No DataMatrix found";
        }

        /// <summary>
        /// Read PDF417 codes (commonly used on IDs/licenses)
        /// </summary>
        public string ReadPdf417(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            var pdf417 = results.FirstOrDefault(r =>
                r.BarcodeType == BarcodeEncoding.PDF417);

            return pdf417?.Text ?? "No PDF417 found";
        }

        /// <summary>
        /// Read all barcodes from a multi-barcode image
        /// </summary>
        public Dictionary<string, string> ReadAllFromImage(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            return results.ToDictionary(
                r => r.BarcodeType.ToString(),
                r => r.Text
            );
        }

        public void ShowFullSupport()
        {
            Console.WriteLine("IronBarcode Format Support (50+ formats):");
            Console.WriteLine();

            Console.WriteLine("1D LINEAR BARCODES:");
            var linear1D = new[] {
                "Code128", "Code39", "Code93", "EAN13", "EAN8",
                "UPCA", "UPCE", "Codabar", "ITF", "MSI",
                "Code11", "Code25", "Pharmacode", "PZN"
            };
            foreach (var f in linear1D)
            {
                Console.WriteLine($"  [OK] {f}");
            }

            Console.WriteLine();
            Console.WriteLine("2D MATRIX BARCODES:");
            var matrix2D = new[] {
                "QRCode", "MicroQR", "DataMatrix", "PDF417",
                "MicroPDF417", "Aztec", "MaxiCode", "DotCode"
            };
            foreach (var f in matrix2D)
            {
                Console.WriteLine($"  [OK] {f}");
            }

            Console.WriteLine();
            Console.WriteLine("POSTAL CODES:");
            var postal = new[] {
                "USPS", "RoyalMail", "AustraliaPost", "PostNet"
            };
            foreach (var f in postal)
            {
                Console.WriteLine($"  [OK] {f}");
            }

            Console.WriteLine();
            Console.WriteLine("All formats: READ + WRITE supported");
            Console.WriteLine("No format specification required - automatic detection");
        }
    }

    /// <summary>
    /// Same inventory scenario - works with IronBarcode
    /// </summary>
    public class InventorySolution
    {
        public void ShowSolution()
        {
            Console.WriteLine("Same Scenario with IronBarcode:");
            Console.WriteLine();
            Console.WriteLine("  Your warehouse has products with different barcode types:");
            Console.WriteLine();
            Console.WriteLine("  [Product A] EAN-13: 5901234123457");
            Console.WriteLine("  [Product B] Code 128: PROD-2024-001");
            Console.WriteLine("  [Product C] QR Code: {\"sku\":\"SKU123\",\"batch\":\"B001\"}");
            Console.WriteLine("  [Product D] DataMatrix: 01034567890123451721...");
            Console.WriteLine();
            Console.WriteLine("  With IronBarcode:");
            Console.WriteLine("    [OK] Product A - EAN-13 works");
            Console.WriteLine("    [OK] Product B - Code 128 works");
            Console.WriteLine("    [OK] Product C - QR Code works");
            Console.WriteLine("    [OK] Product D - DataMatrix works");
            Console.WriteLine();
            Console.WriteLine("  100% of your inventory is scannable!");

            // Example implementation
            var code = @"
    public void ScanProduct(string imagePath)
    {
        // One line handles ALL formats
        var results = BarcodeReader.Read(imagePath);

        foreach (var barcode in results)
        {
            // Format automatically detected
            switch (barcode.BarcodeType)
            {
                case BarcodeEncoding.EAN13:
                case BarcodeEncoding.UPCA:
                    ProcessRetailBarcode(barcode);
                    break;

                case BarcodeEncoding.QRCode:
                    ProcessQrData(JsonConvert.Deserialize(barcode.Text));
                    break;

                case BarcodeEncoding.DataMatrix:
                    ProcessGS1DataMatrix(barcode.Text);
                    break;

                default:
                    ProcessGenericBarcode(barcode);
                    break;
            }
        }
    }";

            Console.WriteLine();
            Console.WriteLine("  Implementation:");
            Console.WriteLine(code);
        }
    }
}

// ============================================================================
// SIDE-BY-SIDE CODE COMPARISON
// ============================================================================

namespace CodeComparison
{
    using System;

    public class SideBySideComparison
    {
        public void CompareQrCodeReading()
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║            QR CODE READING: TELERIK vs IRONBARCODE            ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  TELERIK RADBARCODE                                           ║");
            Console.WriteLine("║  ───────────────────                                          ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║    // Attempt to read QR code                                 ║");
            Console.WriteLine("║    var reader = new RadBarcodeReader();                       ║");
            Console.WriteLine("║    reader.DecodeTypes = new[] {                               ║");
            Console.WriteLine("║        // DecodeType.QR  <-- DOES NOT EXIST                   ║");
            Console.WriteLine("║        DecodeType.Code128,  // Only 1D types available        ║");
            Console.WriteLine("║        DecodeType.EAN13                                       ║");
            Console.WriteLine("║    };                                                         ║");
            Console.WriteLine("║    var result = reader.Decode(qrCodeImage);                   ║");
            Console.WriteLine("║    // result = null  (QR codes not supported)                 ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║    RESULT: FAILURE - Cannot read QR codes                     ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  IRONBARCODE                                                  ║");
            Console.WriteLine("║  ──────────                                                   ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║    // Read QR code (or any other format)                      ║");
            Console.WriteLine("║    var results = BarcodeReader.Read(\"qrcode.png\");            ║");
            Console.WriteLine("║    var qrValue = results.First().Text;                        ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║    // That's it - format detected automatically               ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║    RESULT: SUCCESS - QR code decoded                          ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        }

        public void CompareDataMatrixReading()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         DATAMATRIX READING: TELERIK vs IRONBARCODE            ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  Use Case: GS1 DataMatrix on pharmaceutical products          ║");
            Console.WriteLine("║  (Required by FDA for drug serialization)                     ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  TELERIK:                                                     ║");
            Console.WriteLine("║    // NOT POSSIBLE                                            ║");
            Console.WriteLine("║    // RadBarcodeReader has no DataMatrix support              ║");
            Console.WriteLine("║    // You cannot comply with FDA drug tracing requirements    ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  IRONBARCODE:                                                 ║");
            Console.WriteLine("║    var results = BarcodeReader.Read(productImage);            ║");
            Console.WriteLine("║    var gs1Data = results                                      ║");
            Console.WriteLine("║        .Where(r => r.BarcodeType == BarcodeEncoding.DataMatrix)║");
            Console.WriteLine("║        .FirstOrDefault()?.Text;                               ║");
            Console.WriteLine("║    // Parse GTIN, lot number, expiry from GS1 string          ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        }

        public void ComparePdf417Reading()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║            PDF417 READING: TELERIK vs IRONBARCODE             ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  Use Case: Reading PDF417 on driver's licenses                ║");
            Console.WriteLine("║  (Required for age verification, identity checking)           ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  TELERIK:                                                     ║");
            Console.WriteLine("║    // NOT POSSIBLE                                            ║");
            Console.WriteLine("║    // RadBarcodeReader has no PDF417 support                  ║");
            Console.WriteLine("║    // Cannot read AAMVA-compliant driver's licenses           ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  IRONBARCODE:                                                 ║");
            Console.WriteLine("║    var results = BarcodeReader.Read(licenseImage);            ║");
            Console.WriteLine("║    var pdf417 = results                                       ║");
            Console.WriteLine("║        .Where(r => r.BarcodeType == BarcodeEncoding.PDF417)   ║");
            Console.WriteLine("║        .FirstOrDefault()?.Text;                               ║");
            Console.WriteLine("║    // Parse AAMVA data: name, DOB, address, etc.              ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        }
    }

    /// <summary>
    /// Format support summary table
    /// </summary>
    public class FormatSummary
    {
        public void ShowTable()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              FORMAT SUPPORT COMPARISON TABLE                  ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("║  Format          │ Telerik Read │ Telerik Write │ IronBarcode ║");
            Console.WriteLine("║  ────────────────┼──────────────┼───────────────┼─────────────║");
            Console.WriteLine("║  Code 128        │      ✓       │       ✓       │      ✓      ║");
            Console.WriteLine("║  Code 39         │      ✓       │       ✓       │      ✓      ║");
            Console.WriteLine("║  EAN-13          │      ✓       │       ✓       │      ✓      ║");
            Console.WriteLine("║  UPC-A           │      ✓       │       ✓       │      ✓      ║");
            Console.WriteLine("║  ITF             │      ✓       │       ✓       │      ✓      ║");
            Console.WriteLine("║  ────────────────┼──────────────┼───────────────┼─────────────║");
            Console.WriteLine("║  QR Code         │      ✗       │       ✓       │      ✓      ║");
            Console.WriteLine("║  Micro QR        │      ✗       │       ✓       │      ✓      ║");
            Console.WriteLine("║  DataMatrix      │      ✗       │       ✓       │      ✓      ║");
            Console.WriteLine("║  PDF417          │      ✗       │       ✓       │      ✓      ║");
            Console.WriteLine("║  Aztec           │      ✗       │       ✓       │      ✓      ║");
            Console.WriteLine("║  MaxiCode        │      ✗       │       ✓       │      ✓      ║");
            Console.WriteLine("║  ────────────────┼──────────────┼───────────────┼─────────────║");
            Console.WriteLine("║  Total Read      │     ~15      │      N/A      │     50+     ║");
            Console.WriteLine("║  Total Write     │     ~20      │      ~20      │     50+     ║");
            Console.WriteLine("║                                                               ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Key insight: Telerik can GENERATE 2D barcodes but cannot READ them.");
            Console.WriteLine("IronBarcode has symmetric read/write support for ALL formats.");
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
        Console.WriteLine("=== Telerik RadBarcode 1D-Only Reading Limitation ===");
        Console.WriteLine();

        // Show Telerik limitations
        var telerikReader = new TelerikFormatLimitation.TelerikBarcodeReader();
        telerikReader.ShowFormatLimitation();
        Console.WriteLine();

        Console.WriteLine("────────────────────────────────────────────────────────────");
        Console.WriteLine();

        // Show the QR code paradox
        var paradox = new TelerikFormatLimitation.QrCodeParadox();
        paradox.DemonstrateParadox();
        Console.WriteLine();

        Console.WriteLine("────────────────────────────────────────────────────────────");
        Console.WriteLine();

        // Inventory scenario problem
        var inventoryProblem = new TelerikFormatLimitation.InventoryScenario();
        inventoryProblem.ShowProblem();
        Console.WriteLine();

        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine();

        // IronBarcode solution
        var ironReader = new IronBarcodeAllFormats.UniversalBarcodeReader();
        ironReader.ShowFullSupport();
        Console.WriteLine();

        Console.WriteLine("────────────────────────────────────────────────────────────");
        Console.WriteLine();

        // Inventory scenario solved
        var inventorySolution = new IronBarcodeAllFormats.InventorySolution();
        inventorySolution.ShowSolution();
        Console.WriteLine();

        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine();

        // Side-by-side comparisons
        var comparison = new CodeComparison.SideBySideComparison();
        comparison.CompareQrCodeReading();
        comparison.CompareDataMatrixReading();
        comparison.ComparePdf417Reading();

        // Summary table
        var summary = new CodeComparison.FormatSummary();
        summary.ShowTable();
    }
}
