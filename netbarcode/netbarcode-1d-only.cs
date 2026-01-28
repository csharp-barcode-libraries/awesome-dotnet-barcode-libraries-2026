/**
 * NetBarcode 1D-Only Limitation Example
 *
 * This example demonstrates NetBarcode's exclusive support for 1D barcodes.
 * QR codes, DataMatrix, and other 2D formats are not available.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Article: NetBarcode vs IronBarcode Comparison 2026
 */

using System;
using System.IO;

// NetBarcode package
// Install: dotnet add package NetBarcode
using NetBarcode;

// IronBarcode package
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace NetBarcodeComparison
{
    /// <summary>
    /// Demonstrates NetBarcode's 1D-only limitation compared to
    /// IronBarcode's comprehensive format support.
    /// </summary>
    public class OneDimensionalOnlyExample
    {
        /// <summary>
        /// NetBarcode works well for 1D barcodes.
        /// </summary>
        public void NetBarcode1DGeneration()
        {
            Console.WriteLine("=== NetBarcode: 1D Barcode Generation ===\n");

            // Code128 - works
            var code128 = new Barcode("CODE128-12345", Type.Code128);
            code128.SaveImageFile("netbarcode-code128.png");
            Console.WriteLine("Generated: netbarcode-code128.png (Code128) - SUCCESS");

            // EAN-13 - works
            var ean13 = new Barcode("5901234123457", Type.EAN13);
            ean13.SaveImageFile("netbarcode-ean13.png");
            Console.WriteLine("Generated: netbarcode-ean13.png (EAN-13) - SUCCESS");

            // UPC-A - works
            var upca = new Barcode("012345678905", Type.UPCA);
            upca.SaveImageFile("netbarcode-upca.png");
            Console.WriteLine("Generated: netbarcode-upca.png (UPC-A) - SUCCESS");

            // Code39 - works
            var code39 = new Barcode("CODE39TEST", Type.Code39);
            code39.SaveImageFile("netbarcode-code39.png");
            Console.WriteLine("Generated: netbarcode-code39.png (Code39) - SUCCESS");

            Console.WriteLine("\nNetBarcode handles these 1D formats well.\n");
        }

        /// <summary>
        /// NetBarcode cannot generate QR codes or other 2D formats.
        /// </summary>
        public void NetBarcode2DAttempt()
        {
            Console.WriteLine("=== NetBarcode: 2D Barcode Attempt ===\n");

            Console.WriteLine("Available NetBarcode.Type enum values:");
            foreach (var type in Enum.GetValues(typeof(Type)))
            {
                Console.WriteLine($"  - {type}");
            }

            Console.WriteLine("\nNote: There is NO QRCode, DataMatrix, PDF417, or Aztec option.");
            Console.WriteLine("NetBarcode is strictly 1D-only.\n");

            Console.WriteLine("If you try to generate a QR code with NetBarcode:");
            Console.WriteLine("  // This code cannot exist - no QR type in enum");
            Console.WriteLine("  // var qr = new Barcode(\"data\", Type.QRCode); // COMPILE ERROR\n");

            Console.WriteLine("Real-world impact:");
            Console.WriteLine("  - Cannot create QR codes for mobile payments");
            Console.WriteLine("  - Cannot create DataMatrix for pharmaceutical tracking");
            Console.WriteLine("  - Cannot create PDF417 for ID cards");
            Console.WriteLine("  - Need a SECOND library for 2D formats\n");
        }

        /// <summary>
        /// IronBarcode supports both 1D and 2D formats seamlessly.
        /// </summary>
        public void IronBarcodeUnifiedFormats()
        {
            Console.WriteLine("=== IronBarcode: Unified 1D and 2D Support ===\n");

            // IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";

            // 1D Barcodes - Same API
            Console.WriteLine("1D Barcodes (same as NetBarcode capability):");

            BarcodeWriter.CreateBarcode("CODE128-12345", BarcodeEncoding.Code128)
                .SaveAsPng("ironbarcode-code128.png");
            Console.WriteLine("  Generated: ironbarcode-code128.png (Code128)");

            BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
                .SaveAsPng("ironbarcode-ean13.png");
            Console.WriteLine("  Generated: ironbarcode-ean13.png (EAN-13)");

            BarcodeWriter.CreateBarcode("012345678905", BarcodeEncoding.UPCA)
                .SaveAsPng("ironbarcode-upca.png");
            Console.WriteLine("  Generated: ironbarcode-upca.png (UPC-A)");

            // 2D Barcodes - SAME API (not available in NetBarcode)
            Console.WriteLine("\n2D Barcodes (NOT available in NetBarcode):");

            BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode)
                .SaveAsPng("ironbarcode-qr.png");
            Console.WriteLine("  Generated: ironbarcode-qr.png (QR Code)");

            BarcodeWriter.CreateBarcode("DMX-1234567890", BarcodeEncoding.DataMatrix)
                .SaveAsPng("ironbarcode-datamatrix.png");
            Console.WriteLine("  Generated: ironbarcode-datamatrix.png (DataMatrix)");

            BarcodeWriter.CreateBarcode("PDF417-SAMPLE-DATA", BarcodeEncoding.PDF417)
                .SaveAsPng("ironbarcode-pdf417.png");
            Console.WriteLine("  Generated: ironbarcode-pdf417.png (PDF417)");

            BarcodeWriter.CreateBarcode("AZTEC-DATA-HERE", BarcodeEncoding.Aztec)
                .SaveAsPng("ironbarcode-aztec.png");
            Console.WriteLine("  Generated: ironbarcode-aztec.png (Aztec)");

            Console.WriteLine("\nKey Point: Same API, same library, all formats.\n");
        }

        /// <summary>
        /// Real-world scenario: Adding QR code to existing 1D workflow.
        /// </summary>
        public void RealWorldScenario()
        {
            Console.WriteLine("=== Real-World Scenario: E-commerce Evolution ===\n");

            Console.WriteLine("Phase 1: Product UPC barcodes");
            Console.WriteLine("  NetBarcode: Works");
            Console.WriteLine("  IronBarcode: Works\n");

            // Both can generate UPC codes
            var netBarcodeUpc = new Barcode("012345678905", Type.UPCA);
            netBarcodeUpc.SaveImageFile("product-upc-net.png");

            BarcodeWriter.CreateBarcode("012345678905", BarcodeEncoding.UPCA)
                .SaveAsPng("product-upc-iron.png");

            Console.WriteLine("Phase 2: Add QR code for mobile app deep link");
            Console.WriteLine("  NetBarcode: CANNOT DO - need second library");
            Console.WriteLine("  IronBarcode: Works (same API)\n");

            // NetBarcode CANNOT do this
            // var qr = new Barcode("https://myapp.com/product/123", Type.QRCode);
            // ^ COMPILE ERROR: Type.QRCode does not exist

            // IronBarcode handles it seamlessly
            BarcodeWriter.CreateBarcode("https://myapp.com/product/123", BarcodeEncoding.QRCode)
                .SaveAsPng("product-qr-iron.png");
            Console.WriteLine("  IronBarcode generated: product-qr-iron.png\n");

            Console.WriteLine("Phase 3: Read barcodes from supplier invoices");
            Console.WriteLine("  NetBarcode: CANNOT DO - generation only");
            Console.WriteLine("  IronBarcode: Works\n");

            // NetBarcode cannot read
            // IronBarcode can
            var results = BarcodeReader.Read("product-upc-iron.png");
            Console.WriteLine($"  IronBarcode read UPC: {results.FirstOrDefault()?.Text}");

            var qrResults = BarcodeReader.Read("product-qr-iron.png");
            Console.WriteLine($"  IronBarcode read QR: {qrResults.FirstOrDefault()?.Text}\n");
        }

        /// <summary>
        /// Compare what happens when requirements change.
        /// </summary>
        public void RequirementsEvolution()
        {
            Console.WriteLine("=== Requirements Evolution Cost ===\n");

            Console.WriteLine("Starting requirement: Generate Code128 shipping labels");
            Console.WriteLine("  NetBarcode: Good choice initially ($0)");
            Console.WriteLine("  IronBarcode: Also works ($749)\n");

            Console.WriteLine("6 months later: 'We need QR codes for contactless delivery'");
            Console.WriteLine("  NetBarcode path:");
            Console.WriteLine("    1. Research QR code libraries");
            Console.WriteLine("    2. Install QRCoder (different API)");
            Console.WriteLine("    3. Maintain two libraries");
            Console.WriteLine("    4. Dev time: ~8 hours ($800 at $100/hr)");
            Console.WriteLine("    Running total: $800\n");

            Console.WriteLine("  IronBarcode path:");
            Console.WriteLine("    1. Change encoding parameter");
            Console.WriteLine("    2. Done");
            Console.WriteLine("    Dev time: ~10 minutes ($17 at $100/hr)");
            Console.WriteLine("    Running total: $749 + $17 = $766\n");

            Console.WriteLine("12 months later: 'We need to scan return labels from customers'");
            Console.WriteLine("  NetBarcode path:");
            Console.WriteLine("    1. Research reading libraries");
            Console.WriteLine("    2. Install ZXing.Net (third library, third API)");
            Console.WriteLine("    3. Maintain three libraries");
            Console.WriteLine("    4. Dev time: ~12 hours ($1,200 at $100/hr)");
            Console.WriteLine("    Running total: $800 + $1,200 = $2,000\n");

            Console.WriteLine("  IronBarcode path:");
            Console.WriteLine("    1. Use BarcodeReader.Read()");
            Console.WriteLine("    2. Done");
            Console.WriteLine("    Dev time: ~30 minutes ($50 at $100/hr)");
            Console.WriteLine("    Running total: $766 + $50 = $816\n");

            Console.WriteLine("18 months: Ongoing maintenance");
            Console.WriteLine("  NetBarcode path: 3 libraries to update, test compatibility");
            Console.WriteLine("  IronBarcode path: 1 library to update\n");

            Console.WriteLine("SUMMARY:");
            Console.WriteLine("  NetBarcode 'free' path: $2,000+ in dev time");
            Console.WriteLine("  IronBarcode paid path: $816 total");
            Console.WriteLine("  Savings with IronBarcode: $1,184+\n");
        }

        public static void Main(string[] args)
        {
            var example = new OneDimensionalOnlyExample();

            example.NetBarcode1DGeneration();
            example.NetBarcode2DAttempt();
            example.IronBarcodeUnifiedFormats();
            example.RealWorldScenario();
            example.RequirementsEvolution();

            Console.WriteLine("=== Summary ===");
            Console.WriteLine("NetBarcode: 1D barcodes only, generation only");
            Console.WriteLine("IronBarcode: All formats, read + write, single library");
        }
    }
}
