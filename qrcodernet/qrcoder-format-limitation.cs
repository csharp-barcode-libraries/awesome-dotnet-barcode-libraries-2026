/**
 * QRCoder Format Limitation Example
 *
 * This example demonstrates QRCoder's limitation to only QR codes,
 * showing what happens when other barcode formats are needed.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Article: QRCoder vs IronBarcode Comparison 2026
 */

using System;
using System.Collections.Generic;
using System.IO;

// QRCoder package
// Install: dotnet add package QRCoder
using QRCoder;

// IronBarcode package
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace QRCoderComparison
{
    /// <summary>
    /// Demonstrates what happens when a project needs barcode formats
    /// beyond QR codes when using QRCoder.
    /// </summary>
    public class FormatLimitationExample
    {
        /// <summary>
        /// Shows what QRCoder supports vs. common industry requirements.
        /// </summary>
        public void FormatRequirementsMatrix()
        {
            Console.WriteLine("=== Industry Barcode Format Requirements ===\n");

            var requirements = new Dictionary<string, (string[] Formats, bool QRCoderSupports)>
            {
                { "Mobile Marketing", (new[] { "QR Code" }, true) },
                { "WiFi Sharing", (new[] { "QR Code" }, true) },
                { "Payment Links", (new[] { "QR Code" }, true) },
                { "2FA Authentication", (new[] { "QR Code" }, true) },
                { "Retail Products", (new[] { "EAN-13", "UPC-A" }, false) },
                { "Shipping Labels", (new[] { "Code128", "Code39" }, false) },
                { "Pharmaceutical", (new[] { "DataMatrix", "GS1" }, false) },
                { "Government IDs", (new[] { "PDF417" }, false) },
                { "Airline Boarding", (new[] { "Aztec", "PDF417" }, false) },
                { "UPS Shipping", (new[] { "MaxiCode" }, false) },
                { "Library Books", (new[] { "Code39" }, false) },
                { "Warehouse Inventory", (new[] { "Code128", "DataMatrix" }, false) },
            };

            Console.WriteLine("Industry            | Required Formats        | QRCoder");
            Console.WriteLine("--------------------|------------------------|--------");

            int supported = 0;
            int total = requirements.Count;

            foreach (var kvp in requirements)
            {
                string formats = string.Join(", ", kvp.Value.Formats);
                string support = kvp.Value.QRCoderSupports ? "YES" : "NO";
                Console.WriteLine($"{kvp.Key,-19} | {formats,-22} | {support}");
                if (kvp.Value.QRCoderSupports) supported++;
            }

            Console.WriteLine($"\nQRCoder supports: {supported}/{total} industry use cases ({100 * supported / total}%)");
            Console.WriteLine("IronBarcode supports: 12/12 industry use cases (100%)\n");
        }

        /// <summary>
        /// Shows the error you DON'T see - QRCoder simply lacks the API.
        /// </summary>
        public void WhatYouCantCode()
        {
            Console.WriteLine("=== Code That Cannot Exist in QRCoder ===\n");

            Console.WriteLine("QRCoder's QRCodeGenerator class has ONE method for barcode creation:");
            Console.WriteLine("  CreateQrCode(string data, ECCLevel level)");
            Console.WriteLine("\nThere is NO:");
            Console.WriteLine("  - CreateCode128(data)");
            Console.WriteLine("  - CreateEAN13(data)");
            Console.WriteLine("  - CreateDataMatrix(data)");
            Console.WriteLine("  - CreatePDF417(data)");
            Console.WriteLine("  - CreateAztec(data)");
            Console.WriteLine("  - CreateUPCA(data)");
            Console.WriteLine("  - CreateAnyOtherFormat(data)");

            Console.WriteLine("\nThe following code CANNOT be written with QRCoder:\n");
            Console.WriteLine(@"
// This code DOES NOT COMPILE - these types don't exist in QRCoder

// Attempt Code128
var code128 = qrGenerator.CreateCode128(""SHIPPING-LABEL"");
// Error: QRCodeGenerator does not contain 'CreateCode128'

// Attempt EAN-13
var ean13 = qrGenerator.CreateEAN13(""5901234123457"");
// Error: QRCodeGenerator does not contain 'CreateEAN13'

// Attempt DataMatrix
var dataMatrix = qrGenerator.CreateDataMatrix(""PHARMA-LOT"");
// Error: QRCodeGenerator does not contain 'CreateDataMatrix'
");

            Console.WriteLine("\nQRCoder is DESIGNED to only support QR codes.");
            Console.WriteLine("This is intentional, not a bug or missing feature.\n");
        }

        /// <summary>
        /// Shows the multi-library solution when you need more than QR.
        /// </summary>
        public void MultiLibrarySolution()
        {
            Console.WriteLine("=== Multi-Library Workaround ===\n");

            Console.WriteLine("When you need QR + other formats with open-source:");
            Console.WriteLine("\n1. For QR codes: QRCoder (what you already have)");
            Console.WriteLine("   using QRCoder;");
            Console.WriteLine("   var qrGenerator = new QRCodeGenerator();");

            Console.WriteLine("\n2. For 1D barcodes: Add NetBarcode");
            Console.WriteLine("   dotnet add package NetBarcode");
            Console.WriteLine("   using NetBarcode;");
            Console.WriteLine("   var barcode = new Barcode(data, Type.Code128);");

            Console.WriteLine("\n3. For DataMatrix: Add Barcoder");
            Console.WriteLine("   dotnet add package Barcoder");
            Console.WriteLine("   dotnet add package Barcoder.Renderer.Image");
            Console.WriteLine("   using Barcoder.DataMatrix;");
            Console.WriteLine("   var dm = DataMatrixEncoder.Encode(data);");

            Console.WriteLine("\n4. For reading ANY barcode: Add ZXing.Net");
            Console.WriteLine("   dotnet add package ZXing.Net");
            Console.WriteLine("   using ZXing;");
            Console.WriteLine("   var reader = new BarcodeReaderGeneric();");

            Console.WriteLine("\nResult:");
            Console.WriteLine("  - 4+ NuGet packages");
            Console.WriteLine("  - 4+ different APIs");
            Console.WriteLine("  - 4+ sets of documentation");
            Console.WriteLine("  - 4+ potential version conflicts");
            Console.WriteLine("  - 4+ things to test with .NET updates\n");
        }

        /// <summary>
        /// Shows IronBarcode handling all formats with one API.
        /// </summary>
        public void IronBarcodeUnifiedSolution()
        {
            Console.WriteLine("=== IronBarcode: One Library, All Formats ===\n");

            // IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";

            Console.WriteLine("One NuGet package, one API:\n");

            var formats = new Dictionary<string, BarcodeEncoding>
            {
                { "QR Code", BarcodeEncoding.QRCode },
                { "Code128", BarcodeEncoding.Code128 },
                { "EAN-13", BarcodeEncoding.EAN13 },
                { "UPC-A", BarcodeEncoding.UPCA },
                { "Code39", BarcodeEncoding.Code39 },
                { "DataMatrix", BarcodeEncoding.DataMatrix },
                { "PDF417", BarcodeEncoding.PDF417 },
                { "Aztec", BarcodeEncoding.Aztec },
            };

            // Sample data for each format
            var sampleData = new Dictionary<string, string>
            {
                { "QR Code", "https://example.com" },
                { "Code128", "CODE128-DATA" },
                { "EAN-13", "5901234123457" },
                { "UPC-A", "012345678905" },
                { "Code39", "CODE39DATA" },
                { "DataMatrix", "DATAMATRIX" },
                { "PDF417", "PDF417DATA" },
                { "Aztec", "AZTECDATA" },
            };

            foreach (var kvp in formats)
            {
                string formatName = kvp.Key;
                BarcodeEncoding encoding = kvp.Value;
                string data = sampleData[formatName];

                try
                {
                    BarcodeWriter.CreateBarcode(data, encoding)
                        .SaveAsPng($"iron-unified-{formatName.ToLower().Replace(" ", "")}.png");
                    Console.WriteLine($"  {formatName,-12}: Generated (same API)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  {formatName,-12}: {ex.Message}");
                }
            }

            Console.WriteLine("\nSame pattern for ALL formats:");
            Console.WriteLine("  BarcodeWriter.CreateBarcode(data, encoding).SaveAsPng(path);");
            Console.WriteLine("\nPLUS reading capability:");
            Console.WriteLine("  BarcodeReader.Read(imagePath);\n");
        }

        /// <summary>
        /// Real-world scenario showing format evolution.
        /// </summary>
        public void RealWorldScenario()
        {
            Console.WriteLine("=== Real-World Scenario: E-commerce Platform ===\n");

            Console.WriteLine("Month 1: Marketing team wants QR codes for promotions");
            Console.WriteLine("  Decision: Use QRCoder (free, simple, QR-only)");
            Console.WriteLine("  Effort: 2 hours setup\n");

            // QRCoder for initial QR needs
            var qrGen = new QRCodeGenerator();
            var promoQR = qrGen.CreateQrCode("https://shop.example.com/promo", QRCodeGenerator.ECCLevel.M);
            var promoImg = new PngByteQRCode(promoQR);
            File.WriteAllBytes("scenario-promo-qr.png", promoImg.GetGraphic(10));
            Console.WriteLine("  Generated: scenario-promo-qr.png\n");

            Console.WriteLine("Month 3: Shipping team needs Code128 for labels");
            Console.WriteLine("  Problem: QRCoder can't do Code128");
            Console.WriteLine("  Solution: Add NetBarcode package");
            Console.WriteLine("  Effort: 4 hours integration\n");

            // Now using IronBarcode to show what WOULD be needed
            BarcodeWriter.CreateBarcode("SHIP-12345", BarcodeEncoding.Code128)
                .SaveAsPng("scenario-shipping.png");
            Console.WriteLine("  Generated: scenario-shipping.png\n");

            Console.WriteLine("Month 6: Warehouse needs to SCAN incoming shipments");
            Console.WriteLine("  Problem: Neither QRCoder nor NetBarcode can read");
            Console.WriteLine("  Solution: Add ZXing.Net package");
            Console.WriteLine("  Effort: 8 hours integration\n");

            // Demonstrate reading
            var scanResult = BarcodeReader.Read("scenario-shipping.png");
            Console.WriteLine($"  Scanned: {scanResult.FirstOrDefault()?.Text}\n");

            Console.WriteLine("Month 9: Pharma product line needs DataMatrix for FDA compliance");
            Console.WriteLine("  Problem: None of the current libraries support DataMatrix well");
            Console.WriteLine("  Solution: Add Barcoder + Barcoder.Renderer.Image");
            Console.WriteLine("  Effort: 6 hours integration\n");

            BarcodeWriter.CreateBarcode("LOT-ABC-123", BarcodeEncoding.DataMatrix)
                .SaveAsPng("scenario-pharma.png");
            Console.WriteLine("  Generated: scenario-pharma.png\n");

            Console.WriteLine("=== Total Cost Analysis ===");
            Console.WriteLine("\nQRCoder path (actual cost):");
            Console.WriteLine("  Month 1: QRCoder setup         2 hours");
            Console.WriteLine("  Month 3: NetBarcode integration 4 hours");
            Console.WriteLine("  Month 6: ZXing.Net integration  8 hours");
            Console.WriteLine("  Month 9: Barcoder integration   6 hours");
            Console.WriteLine("  Ongoing: Maintain 4 libraries   10 hours");
            Console.WriteLine("  ────────────────────────────────────────");
            Console.WriteLine("  Total:                          30 hours @ $100/hr = $3,000\n");

            Console.WriteLine("IronBarcode path (alternative):");
            Console.WriteLine("  Month 1: IronBarcode setup      1 hour");
            Console.WriteLine("  Month 3: Add Code128 (same lib) 0.5 hours");
            Console.WriteLine("  Month 6: Add reading (same lib) 0.5 hours");
            Console.WriteLine("  Month 9: Add DataMatrix (same)  0.5 hours");
            Console.WriteLine("  Ongoing: Maintain 1 library     2 hours");
            Console.WriteLine("  ────────────────────────────────────────");
            Console.WriteLine("  Total:                          4.5 hours @ $100/hr = $450");
            Console.WriteLine("  License:                        $749");
            Console.WriteLine("  ────────────────────────────────────────");
            Console.WriteLine("  Grand total:                    $1,199\n");

            Console.WriteLine("Savings with IronBarcode: $1,801 over 1 year\n");
        }

        /// <summary>
        /// Shows format count comparison.
        /// </summary>
        public void FormatCountComparison()
        {
            Console.WriteLine("=== Format Support Comparison ===\n");

            Console.WriteLine("QRCoder supported formats:");
            Console.WriteLine("  - QR Code (standard)");
            Console.WriteLine("  - Micro QR (compact variant)");
            Console.WriteLine("  Total: 2 formats\n");

            Console.WriteLine("IronBarcode supported formats (partial list):");
            Console.WriteLine("  1D Linear:");
            Console.WriteLine("    Code128, Code128A, Code128B, Code128C");
            Console.WriteLine("    Code39, Code39Extended, Code93");
            Console.WriteLine("    EAN8, EAN13, UPCA, UPCE");
            Console.WriteLine("    Codabar, ITF14, Interleaved2of5");
            Console.WriteLine("    GS1-128, GS1 DataBar variants");
            Console.WriteLine("    Intelligent Mail, Royal Mail, Australia Post");
            Console.WriteLine("    Pharmacode, Plessey, MSI, Telepen");
            Console.WriteLine("  2D Matrix:");
            Console.WriteLine("    QRCode, DataMatrix, PDF417");
            Console.WriteLine("    Aztec, MaxiCode, MicroPDF417");
            Console.WriteLine("  Total: 50+ formats\n");

            Console.WriteLine("Format coverage:");
            Console.WriteLine("  QRCoder:    2 formats   (4% of IronBarcode)");
            Console.WriteLine("  IronBarcode: 50+ formats (100%)\n");

            Console.WriteLine("Reading capability:");
            Console.WriteLine("  QRCoder:    NO");
            Console.WriteLine("  IronBarcode: YES (all formats)\n");
        }

        /// <summary>
        /// Final comparison summary.
        /// </summary>
        public void ComparisonSummary()
        {
            Console.WriteLine("=== Format Limitation Summary ===\n");

            Console.WriteLine("If you need ONLY QR codes and will NEVER need:");
            Console.WriteLine("  - Other barcode formats");
            Console.WriteLine("  - Reading capability");
            Console.WriteLine("  - PDF processing");
            Console.WriteLine("Then QRCoder is a good choice. It's excellent at QR generation.\n");

            Console.WriteLine("If there's ANY chance you'll need:");
            Console.WriteLine("  - Code128 for shipping");
            Console.WriteLine("  - EAN/UPC for retail");
            Console.WriteLine("  - DataMatrix for healthcare");
            Console.WriteLine("  - Reading/scanning capability");
            Console.WriteLine("Then IronBarcode saves time and money long-term.\n");

            Console.WriteLine("The question is not 'Is QRCoder good?' (it is).");
            Console.WriteLine("The question is 'Will I only ever need QR codes?' (probably not).\n");
        }

        public static void Main(string[] args)
        {
            var example = new FormatLimitationExample();

            example.FormatRequirementsMatrix();
            example.WhatYouCantCode();
            example.MultiLibrarySolution();
            example.IronBarcodeUnifiedSolution();
            example.RealWorldScenario();
            example.FormatCountComparison();
            example.ComparisonSummary();

            Console.WriteLine("=== Conclusion ===");
            Console.WriteLine("QRCoder: Excellent if QR is truly your only need");
            Console.WriteLine("IronBarcode: Better choice for most real-world projects");
        }
    }
}
