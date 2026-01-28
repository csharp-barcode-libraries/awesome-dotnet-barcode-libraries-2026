/**
 * NetBarcode Format Limitations Example
 *
 * This example provides a comprehensive comparison of format support
 * between NetBarcode and IronBarcode, showing the gap in symbology coverage.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Article: NetBarcode vs IronBarcode Comparison 2026
 */

using System;
using System.Collections.Generic;

// NetBarcode package
// Install: dotnet add package NetBarcode
using NetBarcode;

// IronBarcode package
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace NetBarcodeComparison
{
    /// <summary>
    /// Demonstrates the format coverage differences between
    /// NetBarcode and IronBarcode.
    /// </summary>
    public class FormatLimitationsExample
    {
        /// <summary>
        /// Lists all formats supported by NetBarcode.
        /// </summary>
        public void NetBarcodeSupportedFormats()
        {
            Console.WriteLine("=== NetBarcode Supported Formats ===\n");

            Console.WriteLine("NetBarcode Type enum includes:");

            var netBarcodeTypes = new Dictionary<string, string>
            {
                { "Type.Code128", "Code 128 (auto-switching)" },
                { "Type.Code128A", "Code 128A (uppercase, numbers)" },
                { "Type.Code128B", "Code 128B (mixed case)" },
                { "Type.Code128C", "Code 128C (numeric pairs)" },
                { "Type.Code39", "Code 39 (alphanumeric)" },
                { "Type.Code39Extended", "Code 39 Extended (full ASCII)" },
                { "Type.Code93", "Code 93" },
                { "Type.EAN8", "EAN-8 (8-digit European)" },
                { "Type.EAN13", "EAN-13 (13-digit European)" },
                { "Type.UPCA", "UPC-A (12-digit American)" },
                { "Type.UPCE", "UPC-E (6-digit compressed)" },
                { "Type.Codabar", "Codabar (numeric with symbols)" },
                { "Type.ITF", "ITF/Interleaved 2 of 5" },
                { "Type.MSI", "MSI (Mod 10 checksum)" }
            };

            foreach (var kvp in netBarcodeTypes)
            {
                Console.WriteLine($"  {kvp.Key,-20} - {kvp.Value}");
            }

            Console.WriteLine($"\nTotal: {netBarcodeTypes.Count} formats");
            Console.WriteLine("Category: 1D linear barcodes ONLY\n");
        }

        /// <summary>
        /// Lists formats NOT supported by NetBarcode.
        /// </summary>
        public void NetBarcodeUnsupportedFormats()
        {
            Console.WriteLine("=== Formats NetBarcode CANNOT Generate ===\n");

            Console.WriteLine("2D Matrix Barcodes (high-capacity, widely used):");
            Console.WriteLine("  - QR Code (most requested format)");
            Console.WriteLine("  - DataMatrix (pharmaceutical/industrial standard)");
            Console.WriteLine("  - PDF417 (ID cards, shipping manifests)");
            Console.WriteLine("  - Aztec (boarding passes, tickets)");
            Console.WriteLine("  - MaxiCode (UPS shipping)");
            Console.WriteLine("  - Micro QR (small form factor)");
            Console.WriteLine("  - Han Xin (Chinese national standard)");

            Console.WriteLine("\nSpecialized 1D Barcodes:");
            Console.WriteLine("  - GS1-128 (supply chain)");
            Console.WriteLine("  - GS1 DataBar (variable measure items)");
            Console.WriteLine("  - Intelligent Mail (USPS)");
            Console.WriteLine("  - Royal Mail 4-State");
            Console.WriteLine("  - Australia Post");
            Console.WriteLine("  - Pharmacode (pharmaceutical)");
            Console.WriteLine("  - Plessey (libraries)");
            Console.WriteLine("  - Telepen (UK retail)");

            Console.WriteLine("\nImpact: Need additional library for ANY of these formats.\n");
        }

        /// <summary>
        /// Shows IronBarcode's comprehensive format coverage.
        /// </summary>
        public void IronBarcodeSupportedFormats()
        {
            Console.WriteLine("=== IronBarcode Format Coverage ===\n");

            Console.WriteLine("1D Linear Barcodes (all NetBarcode formats plus more):");
            var linear1D = new[]
            {
                "Code128, Code128A, Code128B, Code128C",
                "Code39, Code39Extended, Code93",
                "EAN8, EAN13, UPCA, UPCE",
                "Codabar, ITF14, Interleaved2of5",
                "MSI, Plessey, Telepen",
                "GS1128, GS1DataBar, GS1DataBarExpanded",
                "IntelligentMail, RoyalMail, AustraliaPost",
                "PostNet, Planet, JapanPost",
                "Pharmacode"
            };

            foreach (var group in linear1D)
            {
                Console.WriteLine($"  {group}");
            }

            Console.WriteLine("\n2D Matrix Barcodes (NOT available in NetBarcode):");
            var matrix2D = new[]
            {
                "QRCode - Most popular 2D format",
                "DataMatrix - Industrial/pharma standard (ECC 200)",
                "PDF417 - High-capacity stacked barcode",
                "MicroPDF417 - Compact PDF417 variant",
                "Aztec - Self-correcting, no quiet zone",
                "MaxiCode - UPS shipping standard",
            };

            foreach (var format in matrix2D)
            {
                Console.WriteLine($"  {format}");
            }

            Console.WriteLine("\nTotal: 50+ symbologies in single library\n");
        }

        /// <summary>
        /// Demonstrates generating formats unavailable in NetBarcode.
        /// </summary>
        public void GenerateUnsupportedFormats()
        {
            Console.WriteLine("=== Generating Formats Unavailable in NetBarcode ===\n");

            // IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";

            // QR Code - Most requested format globally
            Console.WriteLine("QR Code (most requested, not in NetBarcode):");
            var qr = BarcodeWriter.CreateBarcode(
                "https://ironsoftware.com/csharp/barcode/",
                BarcodeEncoding.QRCode
            );
            qr.ResizeTo(200, 200);
            qr.SaveAsPng("format-qr.png");
            Console.WriteLine("  Generated: format-qr.png");
            Console.WriteLine("  Use case: Mobile links, payments, authentication\n");

            // DataMatrix - FDA requirement for pharmaceuticals
            Console.WriteLine("DataMatrix (FDA pharma requirement, not in NetBarcode):");
            var datamatrix = BarcodeWriter.CreateBarcode(
                "01034531200000111719112510ABCD1234",
                BarcodeEncoding.DataMatrix
            );
            datamatrix.SaveAsPng("format-datamatrix.png");
            Console.WriteLine("  Generated: format-datamatrix.png");
            Console.WriteLine("  Use case: Pharmaceutical, electronic components\n");

            // PDF417 - High capacity, ID cards
            Console.WriteLine("PDF417 (high capacity, not in NetBarcode):");
            var pdf417 = BarcodeWriter.CreateBarcode(
                "DRIVER LICENSE DATA: NAME=JOHN DOE|DOB=1990-01-15|LICENSE=D1234567",
                BarcodeEncoding.PDF417
            );
            pdf417.SaveAsPng("format-pdf417.png");
            Console.WriteLine("  Generated: format-pdf417.png");
            Console.WriteLine("  Use case: ID cards, shipping manifests\n");

            // Aztec - Boarding passes, no quiet zone needed
            Console.WriteLine("Aztec (boarding passes, not in NetBarcode):");
            var aztec = BarcodeWriter.CreateBarcode(
                "M1DOE/JOHN MR ABC123 JFKLHR 0012 123Y015A0001 100",
                BarcodeEncoding.Aztec
            );
            aztec.SaveAsPng("format-aztec.png");
            Console.WriteLine("  Generated: format-aztec.png");
            Console.WriteLine("  Use case: Airline boarding passes, train tickets\n");

            // GS1-128 - Supply chain standard
            Console.WriteLine("GS1-128 (supply chain, not in NetBarcode):");
            var gs1 = BarcodeWriter.CreateBarcode(
                "(01)00012345678905(17)230615(10)ABC123",
                BarcodeEncoding.GS1128
            );
            gs1.SaveAsPng("format-gs1-128.png");
            Console.WriteLine("  Generated: format-gs1-128.png");
            Console.WriteLine("  Use case: Shipping, inventory tracking\n");
        }

        /// <summary>
        /// Industry-specific format requirements that NetBarcode cannot meet.
        /// </summary>
        public void IndustryRequirements()
        {
            Console.WriteLine("=== Industry-Specific Format Requirements ===\n");

            var industries = new Dictionary<string, (string Required, bool NetBarcodeSupports)>
            {
                { "Pharmaceutical (US FDA)", ("DataMatrix", false) },
                { "Pharmaceutical (EU FMD)", ("DataMatrix", false) },
                { "Healthcare (HIBC)", ("DataMatrix or Code39", true) }, // partial
                { "Retail (Global)", ("EAN-13, UPC-A, QR Code", true) }, // partial
                { "E-commerce Mobile", ("QR Code", false) },
                { "Shipping (UPS)", ("MaxiCode", false) },
                { "Shipping (FedEx)", ("PDF417", false) },
                { "Airlines (IATA)", ("Aztec, PDF417", false) },
                { "Train Tickets", ("Aztec", false) },
                { "ID Documents", ("PDF417, DataMatrix", false) },
                { "Automotive (AIAG)", ("DataMatrix, Code39", true) }, // partial
                { "Electronics", ("DataMatrix", false) },
                { "Mobile Payments", ("QR Code", false) },
                { "Marketing (URLs)", ("QR Code", false) },
            };

            Console.WriteLine("Industry          | Required Format     | NetBarcode Support");
            Console.WriteLine("------------------|--------------------|-----------------");

            foreach (var kvp in industries)
            {
                string support = kvp.Value.NetBarcodeSupports ? "Partial" : "NO";
                Console.WriteLine($"{kvp.Key,-17} | {kvp.Value.Required,-18} | {support}");
            }

            Console.WriteLine("\nConclusion: NetBarcode only fully supports basic retail.");
            Console.WriteLine("Any modern application likely needs 2D formats.\n");
        }

        /// <summary>
        /// What happens when you need an unsupported format with NetBarcode.
        /// </summary>
        public void UnsupportedFormatWorkaround()
        {
            Console.WriteLine("=== NetBarcode Workaround for 2D Formats ===\n");

            Console.WriteLine("When you need QR codes with NetBarcode, you must:");
            Console.WriteLine("\n1. Add QRCoder library:");
            Console.WriteLine("   dotnet add package QRCoder\n");

            Console.WriteLine("2. Learn different API:");
            Console.WriteLine(@"   using QRCoder;

   var qrGenerator = new QRCodeGenerator();
   var qrCodeData = qrGenerator.CreateQrCode(
       ""https://example.com"",
       QRCodeGenerator.ECCLevel.Q
   );
   var qrCode = new PngByteQRCode(qrCodeData);
   var qrBytes = qrCode.GetGraphic(20);
   File.WriteAllBytes(""qr.png"", qrBytes);");

            Console.WriteLine("\n3. Now maintaining two libraries with different APIs");
            Console.WriteLine("   - NetBarcode for 1D (Barcode class)");
            Console.WriteLine("   - QRCoder for QR (QRCodeGenerator class)");

            Console.WriteLine("\n4. If you need reading, add a THIRD library:");
            Console.WriteLine("   dotnet add package ZXing.Net");
            Console.WriteLine("   (Yet another different API to learn)\n");

            Console.WriteLine("With IronBarcode, same API for everything:");
            Console.WriteLine(@"   // 1D barcode
   BarcodeWriter.CreateBarcode(""12345"", BarcodeEncoding.Code128);

   // 2D barcode (same API)
   BarcodeWriter.CreateBarcode(""data"", BarcodeEncoding.QRCode);

   // Reading (same library)
   BarcodeReader.Read(""image.png"");");

            Console.WriteLine();
        }

        /// <summary>
        /// Side-by-side format coverage summary.
        /// </summary>
        public void FormatCoverageSummary()
        {
            Console.WriteLine("=== Format Coverage Summary ===\n");

            Console.WriteLine("Category              | NetBarcode | IronBarcode");
            Console.WriteLine("----------------------|------------|------------");
            Console.WriteLine("Linear 1D (basic)     | 14 formats | 30+ formats");
            Console.WriteLine("GS1 family            | No         | Yes");
            Console.WriteLine("Postal codes          | No         | Yes");
            Console.WriteLine("QR Code               | No         | Yes");
            Console.WriteLine("DataMatrix            | No         | Yes");
            Console.WriteLine("PDF417                | No         | Yes");
            Console.WriteLine("Aztec                 | No         | Yes");
            Console.WriteLine("MaxiCode              | No         | Yes");
            Console.WriteLine("----------------------|------------|------------");
            Console.WriteLine("Total Symbologies     | ~14        | 50+");
            Console.WriteLine("Can Read Barcodes     | No         | Yes");
            Console.WriteLine("Single Library        | No*        | Yes");

            Console.WriteLine("\n* NetBarcode alone cannot meet most modern requirements");
            Console.WriteLine("  Typically requires 2-3 additional libraries.\n");
        }

        public static void Main(string[] args)
        {
            var example = new FormatLimitationsExample();

            example.NetBarcodeSupportedFormats();
            example.NetBarcodeUnsupportedFormats();
            example.IronBarcodeSupportedFormats();
            example.GenerateUnsupportedFormats();
            example.IndustryRequirements();
            example.UnsupportedFormatWorkaround();
            example.FormatCoverageSummary();

            Console.WriteLine("=== Conclusion ===");
            Console.WriteLine("NetBarcode: Good for basic 1D retail barcodes");
            Console.WriteLine("IronBarcode: Complete solution for all barcode needs");
        }
    }
}
