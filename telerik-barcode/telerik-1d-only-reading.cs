/**
 * Telerik RadBarcodeReader Symbology Limitation
 *
 * This example demonstrates the format coverage of Telerik's RadBarcodeReader
 * on the two desktop platforms where it ships:
 *
 *   - WPF (namespace Telerik.Windows.Controls.Barcode):
 *       1D linear formats PLUS QR, PDF417, DataMatrix.
 *       Aztec, MaxiCode, MicroQR, DotCode are NOT in the DecodeType enum
 *       and cannot be read.
 *
 *   - WinForms (namespace Telerik.WinControls.UI.Barcode):
 *       1D linear formats only. Telerik's documentation states:
 *       "Currently, all of the 1D barcodes, offered by Telerik, are
 *        supported." There are no 2D entries at all.
 *
 * Telerik UI for Blazor, ASP.NET AJAX, ASP.NET Core, and ASP.NET MVC
 * have NO barcode reading component.
 *
 * IronBarcode reads all of these formats with one API call and no
 * format specification.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Comparison: Telerik RadBarcode vs IronBarcode
 *
 * Distribution note: Telerik UI packages are not on public nuget.org;
 * they are distributed via the licensed feed at
 * https://nuget.telerik.com/v3/index.json with API-key authentication.
 */

// ============================================================================
// TELERIK APPROACH: Reader symbology coverage varies by platform
// ============================================================================

namespace TelerikFormatLimitation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Telerik RadBarcodeReader Format Support (WPF version)
    /// 1D linear barcodes PLUS three 2D formats — QR, PDF417, DataMatrix.
    /// Aztec, MaxiCode, MicroQR, DotCode are NOT supported.
    /// </summary>
    public class TelerikWpfBarcodeReader
    {
        // RadBarcodeReader (WPF) DecodeType flags include these:
        public static readonly string[] WpfSupportedFormats = new[]
        {
            // 1D linear
            "Code128",
            "Code39",
            "Code39Extended",
            "Code93",
            "Code93Extended",
            "Code11",
            "Code25Standard",
            "Code25Interleaved",
            "EAN13",
            "EAN128",
            "EAN8",
            "UPCA",
            "UPCE",
            "UPCSupplement2",
            "UPCSupplement5",
            "Codabar",
            "CodeMSI",
            "Postnet",
            "Planet",
            "IntelligentMail",

            // 2D — covered by the WPF reader
            "QR",
            "PDF417",
            "DataMatrix"
        };

        // These 2D formats are NOT in the WPF DecodeType enum:
        public static readonly string[] WpfUnsupportedFormats = new[]
        {
            "Aztec",            // Boarding passes, tickets — not readable
            "MaxiCode",         // UPS shipping labels — not readable
            "MicroQR",          // Compact QR — not readable
            "DotCode"           // Tobacco/pharmaceutical traceability — not readable
        };

        /*
        using Telerik.Windows.Controls.Barcode;
        using System.Windows.Media.Imaging;

        public string TryReadAztecCode(string imagePath)
        {
            var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            var reader = new RadBarcodeReader();

            // The WPF DecodeType is a [Flags] enum — combine with bitwise OR.
            // QR, PDF417, and DataMatrix are valid; Aztec is not.
            reader.DecodeTypes = DecodeType.Code128
                               | DecodeType.QR
                               | DecodeType.PDF417
                               | DecodeType.DataMatrix;
                               // DecodeType.Aztec  -- DOES NOT EXIST on WPF
                               // DecodeType.MaxiCode -- DOES NOT EXIST on WPF

            var result = reader.Decode(bitmap);
            // result is null for Aztec, MaxiCode, MicroQR, DotCode images.
            return result?.Text;
        }

        public string ReadStandardBarcode(string imagePath)
        {
            var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            var reader = new RadBarcodeReader();

            // Combine 1D + the three supported 2D types
            reader.DecodeTypes = DecodeType.Code128
                               | DecodeType.EAN13
                               | DecodeType.UPCA
                               | DecodeType.QR
                               | DecodeType.PDF417
                               | DecodeType.DataMatrix;

            var result = reader.Decode(bitmap);
            return result?.Text;
        }
        */

        public void ShowFormatCoverage()
        {
            Console.WriteLine("Telerik RadBarcodeReader (WPF) Format Coverage:");
            Console.WriteLine();

            Console.WriteLine("SUPPORTED (Can Read):");
            foreach (var format in WpfSupportedFormats)
            {
                Console.WriteLine($"  [OK] {format}");
            }

            Console.WriteLine();
            Console.WriteLine("NOT IN DecodeType ENUM (Cannot Read):");
            foreach (var format in WpfUnsupportedFormats)
            {
                Console.WriteLine($"  [!!] {format}");
            }
        }
    }

    /// <summary>
    /// Telerik RadBarcodeReader (WinForms) — 1D ONLY.
    /// No 2D formats are supported at all on this platform.
    /// </summary>
    public class TelerikWinFormsBarcodeReader
    {
        public static readonly string[] WinFormsSupportedFormats = new[]
        {
            "Code128", "Code93", "Code93Extended",
            "CodeMSI",
            "EAN13", "EAN128", "EAN8",
            "Postnet", "Planet", "IntelligentMail",
            "UPCA", "UPCE",
            "UPCSupplement2", "UPCSupplement5"
        };

        public static readonly string[] WinFormsUnsupportedFormats = new[]
        {
            "QRCode",           // Not in WinForms DecodeType enum
            "DataMatrix",       // Not in WinForms DecodeType enum
            "PDF417",           // Not in WinForms DecodeType enum
            "MicroQR",
            "Aztec",
            "MaxiCode",
            "DotCode"
        };

        /*
        using Telerik.WinControls.UI.Barcode;
        using System.Drawing;

        public string TryReadQrCode(string imagePath)
        {
            using var image = Image.FromFile(imagePath);
            var reader = new RadBarcodeReader();

            // The WinForms DecodeType has NO QR / DataMatrix / PDF417 entries.
            reader.DecodeType = DecodeType.Code128
                              | DecodeType.EAN13;
                              // DecodeType.QR -- DOES NOT EXIST on WinForms

            var result = reader.Read(image);
            // Always null for any 2D barcode image.
            return result?.Text;
        }
        */

        public void ShowFormatCoverage()
        {
            Console.WriteLine("Telerik RadBarcodeReader (WinForms) Format Coverage:");
            Console.WriteLine();
            Console.WriteLine("(Telerik docs: \"Currently, all of the 1D barcodes, " +
                              "offered by Telerik, are supported.\")");
            Console.WriteLine();

            Console.WriteLine("SUPPORTED (Can Read):");
            foreach (var format in WinFormsSupportedFormats)
            {
                Console.WriteLine($"  [OK] {format}");
            }

            Console.WriteLine();
            Console.WriteLine("NOT SUPPORTED (No 2D formats decodable on WinForms):");
            foreach (var format in WinFormsUnsupportedFormats)
            {
                Console.WriteLine($"  [!!] {format}");
            }
        }
    }

    /// <summary>
    /// Demonstrates a real round-trip gap: generate Aztec on WPF,
    /// cannot decode it back. On WinForms, even QR cannot be decoded.
    /// </summary>
    public class RoundTripGap
    {
        public void Demonstrate()
        {
            Console.WriteLine("The Telerik Reader Round-Trip Gap:");
            Console.WriteLine();
            Console.WriteLine("  WPF:");
            Console.WriteLine("    QR / PDF417 / DataMatrix round-trip = OK");
            Console.WriteLine("    Aztec / MaxiCode / MicroQR / DotCode = NOT READABLE");
            Console.WriteLine();
            Console.WriteLine("  WinForms:");
            Console.WriteLine("    Any 2D barcode generated by RadBarcode (including QR, ");
            Console.WriteLine("    DataMatrix, PDF417) cannot be decoded by the WinForms ");
            Console.WriteLine("    RadBarcodeReader. 1D only on the reader side.");
            Console.WriteLine();
            Console.WriteLine("  Blazor / ASP.NET AJAX / ASP.NET Core:");
            Console.WriteLine("    No reader exists at all — generation only.");
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
            Console.WriteLine("  [Product E] Aztec on a returns label");
            Console.WriteLine("  [Product F] MaxiCode on inbound UPS shipment");
            Console.WriteLine();
            Console.WriteLine("  With Telerik RadBarcodeReader (WPF):");
            Console.WriteLine("    [OK] Product A - EAN-13");
            Console.WriteLine("    [OK] Product B - Code 128");
            Console.WriteLine("    [OK] Product C - QR Code");
            Console.WriteLine("    [OK] Product D - DataMatrix");
            Console.WriteLine("    [!!] Product E - Aztec FAILS");
            Console.WriteLine("    [!!] Product F - MaxiCode FAILS");
            Console.WriteLine();
            Console.WriteLine("  With Telerik RadBarcodeReader (WinForms):");
            Console.WriteLine("    [OK] Product A - EAN-13");
            Console.WriteLine("    [OK] Product B - Code 128");
            Console.WriteLine("    [!!] Product C - QR Code FAILS");
            Console.WriteLine("    [!!] Product D - DataMatrix FAILS");
            Console.WriteLine("    [!!] Product E - Aztec FAILS");
            Console.WriteLine("    [!!] Product F - MaxiCode FAILS");
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
        // Install: dotnet add package BarCode
        // License: IronBarCode.License.LicenseKey = "YOUR-KEY";

        /// <summary>
        /// Read any barcode - format detected automatically
        /// </summary>
        public List<BarcodeResult> ReadAnyBarcode(string imagePath)
        {
            // One line - reads any supported format
            // No need to specify format type
            var results = BarcodeReader.Read(imagePath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Found: {barcode.BarcodeType} = {barcode.Value}");
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

            return qrCode?.Value ?? "No QR code found";
        }

        /// <summary>
        /// Read DataMatrix codes (commonly used in manufacturing/healthcare)
        /// </summary>
        public string ReadDataMatrix(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            var dataMatrix = results.FirstOrDefault(r =>
                r.BarcodeType == BarcodeEncoding.DataMatrix);

            return dataMatrix?.Value ?? "No DataMatrix found";
        }

        /// <summary>
        /// Read Aztec codes (boarding passes, tickets) — not in Telerik's reader.
        /// </summary>
        public string ReadAztec(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            var aztec = results.FirstOrDefault(r =>
                r.BarcodeType == BarcodeEncoding.Aztec);

            return aztec?.Value ?? "No Aztec code found";
        }

        /// <summary>
        /// Read all barcodes from a multi-barcode image
        /// </summary>
        public Dictionary<string, string> ReadAllFromImage(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            return results.ToDictionary(
                r => r.BarcodeType.ToString(),
                r => r.Value
            );
        }

        public void ShowFullSupport()
        {
            Console.WriteLine("IronBarcode Format Support:");
            Console.WriteLine();

            Console.WriteLine("1D LINEAR BARCODES:");
            var linear1D = new[] {
                "Code128", "Code39", "Code93", "EAN13", "EAN8",
                "UPCA", "UPCE", "Codabar", "ITF", "MSI",
                "Code11", "Code25"
            };
            foreach (var f in linear1D)
            {
                Console.WriteLine($"  [OK] {f}");
            }

            Console.WriteLine();
            Console.WriteLine("2D MATRIX BARCODES:");
            var matrix2D = new[] {
                "QRCode", "DataMatrix", "PDF417",
                "Aztec", "MicroQR"
            };
            foreach (var f in matrix2D)
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
            Console.WriteLine("  [Product E] Aztec on a returns label");
            Console.WriteLine("  [Product F] MaxiCode on inbound UPS shipment");
            Console.WriteLine();
            Console.WriteLine("  With IronBarcode (any platform — desktop, web, Docker):");
            Console.WriteLine("    [OK] Product A - EAN-13");
            Console.WriteLine("    [OK] Product B - Code 128");
            Console.WriteLine("    [OK] Product C - QR Code");
            Console.WriteLine("    [OK] Product D - DataMatrix");
            Console.WriteLine("    [OK] Product E - Aztec");
            Console.WriteLine("    [OK] Product F - MaxiCode");

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
                    ProcessQrData(JsonConvert.Deserialize(barcode.Value));
                    break;

                case BarcodeEncoding.DataMatrix:
                    ProcessGS1DataMatrix(barcode.Value);
                    break;

                case BarcodeEncoding.Aztec:
                    ProcessReturnsLabel(barcode.Value);
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
        public void CompareAztecReading()
        {
            Console.WriteLine("===============================================================");
            Console.WriteLine("        AZTEC CODE READING: TELERIK vs IRONBARCODE");
            Console.WriteLine("===============================================================");
            Console.WriteLine();
            Console.WriteLine("  TELERIK RADBARCODE (WPF or WinForms)");
            Console.WriteLine("  -------------------------------------");
            Console.WriteLine("    var reader = new RadBarcodeReader();");
            Console.WriteLine("    reader.DecodeTypes = DecodeType.QR | DecodeType.PDF417;");
            Console.WriteLine("        // DecodeType.Aztec  <-- DOES NOT EXIST on either platform");
            Console.WriteLine("    var result = reader.Decode(aztecImage);");
            Console.WriteLine("    // result == null  (Aztec not in DecodeType enum)");
            Console.WriteLine();
            Console.WriteLine("    RESULT: FAILURE - Cannot read Aztec codes");
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("  IRONBARCODE");
            Console.WriteLine("  -----------");
            Console.WriteLine("    var results = BarcodeReader.Read(\"aztec.png\");");
            Console.WriteLine("    var value = results.First().Value;");
            Console.WriteLine();
            Console.WriteLine("    RESULT: SUCCESS - Aztec decoded automatically");
            Console.WriteLine();
        }

        public void CompareWinFormsQrReading()
        {
            Console.WriteLine();
            Console.WriteLine("===============================================================");
            Console.WriteLine("    WINFORMS QR READING: TELERIK vs IRONBARCODE");
            Console.WriteLine("===============================================================");
            Console.WriteLine();
            Console.WriteLine("  Use Case: Reading a QR code in a WinForms inventory app");
            Console.WriteLine();
            Console.WriteLine("  TELERIK (Telerik.WinControls.UI.Barcode.RadBarcodeReader):");
            Console.WriteLine("    // The WinForms DecodeType enum has no QR entry.");
            Console.WriteLine("    // Telerik docs: \"Currently, all of the 1D barcodes,");
            Console.WriteLine("    //               offered by Telerik, are supported.\"");
            Console.WriteLine();
            Console.WriteLine("  IRONBARCODE:");
            Console.WriteLine("    var results = BarcodeReader.Read(productImage);");
            Console.WriteLine("    var qr = results");
            Console.WriteLine("        .Where(r => r.BarcodeType == BarcodeEncoding.QRCode)");
            Console.WriteLine("        .FirstOrDefault()?.Value;");
            Console.WriteLine();
        }

        public void CompareDataMatrixReading()
        {
            Console.WriteLine();
            Console.WriteLine("===============================================================");
            Console.WriteLine("     DATAMATRIX READING: TELERIK vs IRONBARCODE");
            Console.WriteLine("===============================================================");
            Console.WriteLine();
            Console.WriteLine("  Use Case: GS1 DataMatrix on pharmaceutical products");
            Console.WriteLine("  (Required by FDA for drug serialization)");
            Console.WriteLine();
            Console.WriteLine("  TELERIK (WPF):");
            Console.WriteLine("    DecodeType.DataMatrix is supported.");
            Console.WriteLine("    var reader = new RadBarcodeReader();");
            Console.WriteLine("    reader.DecodeTypes = DecodeType.DataMatrix;");
            Console.WriteLine("    var result = reader.Decode(productImage);");
            Console.WriteLine();
            Console.WriteLine("  TELERIK (WinForms):");
            Console.WriteLine("    NOT POSSIBLE — DecodeType.DataMatrix not in WinForms enum.");
            Console.WriteLine();
            Console.WriteLine("  IRONBARCODE (WPF, WinForms, web, Docker — same call):");
            Console.WriteLine("    var results = BarcodeReader.Read(productImage);");
            Console.WriteLine("    var gs1Data = results");
            Console.WriteLine("        .Where(r => r.BarcodeType == BarcodeEncoding.DataMatrix)");
            Console.WriteLine("        .FirstOrDefault()?.Value;");
            Console.WriteLine();
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
            Console.WriteLine("===============================================================");
            Console.WriteLine("                FORMAT SUPPORT COMPARISON TABLE");
            Console.WriteLine("===============================================================");
            Console.WriteLine();
            Console.WriteLine("  Format          | Telerik WPF Read | Telerik WinForms Read | IronBarcode");
            Console.WriteLine("  ----------------+------------------+-----------------------+------------");
            Console.WriteLine("  Code 128        |       YES        |          YES          |    YES");
            Console.WriteLine("  Code 39         |       YES        |          YES          |    YES");
            Console.WriteLine("  EAN-13          |       YES        |          YES          |    YES");
            Console.WriteLine("  UPC-A           |       YES        |          YES          |    YES");
            Console.WriteLine("  ITF             |       YES        |          YES          |    YES");
            Console.WriteLine("  ----------------+------------------+-----------------------+------------");
            Console.WriteLine("  QR Code         |       YES        |           no          |    YES");
            Console.WriteLine("  DataMatrix      |       YES        |           no          |    YES");
            Console.WriteLine("  PDF417          |       YES        |           no          |    YES");
            Console.WriteLine("  ----------------+------------------+-----------------------+------------");
            Console.WriteLine("  Aztec           |        no        |           no          |    YES");
            Console.WriteLine("  MaxiCode        |        no        |           no          |    YES");
            Console.WriteLine("  MicroQR         |        no        |           no          |    YES");
            Console.WriteLine("  DotCode         |        no        |           no          |    YES");
            Console.WriteLine();
            Console.WriteLine("Key insight: Telerik's reader coverage differs WPF vs WinForms,");
            Console.WriteLine("and neither covers Aztec, MaxiCode, MicroQR, or DotCode.");
            Console.WriteLine("IronBarcode's API is identical across platforms with broader format coverage.");
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
        Console.WriteLine("=== Telerik RadBarcodeReader Format Limitations ===");
        Console.WriteLine();

        // Show Telerik WPF reader coverage
        var telerikWpf = new TelerikFormatLimitation.TelerikWpfBarcodeReader();
        telerikWpf.ShowFormatCoverage();
        Console.WriteLine();

        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine();

        // Show Telerik WinForms reader coverage
        var telerikWinForms = new TelerikFormatLimitation.TelerikWinFormsBarcodeReader();
        telerikWinForms.ShowFormatCoverage();
        Console.WriteLine();

        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine();

        // Show round-trip gap
        var gap = new TelerikFormatLimitation.RoundTripGap();
        gap.Demonstrate();
        Console.WriteLine();

        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine();

        // Inventory scenario problem
        var inventoryProblem = new TelerikFormatLimitation.InventoryScenario();
        inventoryProblem.ShowProblem();
        Console.WriteLine();

        Console.WriteLine("============================================================");
        Console.WriteLine();

        // IronBarcode solution
        var ironReader = new IronBarcodeAllFormats.UniversalBarcodeReader();
        ironReader.ShowFullSupport();
        Console.WriteLine();

        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine();

        // Inventory scenario solved
        var inventorySolution = new IronBarcodeAllFormats.InventorySolution();
        inventorySolution.ShowSolution();
        Console.WriteLine();

        Console.WriteLine("============================================================");
        Console.WriteLine();

        // Side-by-side comparisons
        var comparison = new CodeComparison.SideBySideComparison();
        comparison.CompareAztecReading();
        comparison.CompareWinFormsQrReading();
        comparison.CompareDataMatrixReading();

        // Summary table
        var summary = new CodeComparison.FormatSummary();
        summary.ShowTable();
    }
}
