/**
 * BarcodeLib Generation-Only Limitation Example
 *
 * This example demonstrates BarcodeLib's core limitation: it can only
 * generate barcodes, not read them. For reading capability, developers
 * must integrate a separate library.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Article: BarcodeLib vs IronBarcode Comparison 2026
 */

using System;
using System.IO;

// BarcodeLib packages
// Install: dotnet add package BarcodeLib
using BarcodeStandard;
using SkiaSharp;

// IronBarcode package
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace BarcodeLibComparison
{
    /// <summary>
    /// Demonstrates BarcodeLib's generation-only limitation compared to
    /// IronBarcode's unified read/write capabilities.
    /// </summary>
    public class GenerationOnlyExample
    {
        /// <summary>
        /// BarcodeLib can generate barcodes, but cannot read them.
        /// This is a fundamental architectural limitation, not a bug.
        /// </summary>
        public void BarcodeLibGeneration()
        {
            Console.WriteLine("=== BarcodeLib: Generation Only ===\n");

            var barcode = new Barcode();
            barcode.IncludeLabel = true;
            barcode.AlternateLabel = "PRODUCT-12345";

            // Generate Code128 barcode
            SKImage code128Image = barcode.Encode(
                BarcodeStandard.Type.Code128,
                "PRODUCT-12345",
                300,
                100
            );

            // Save to file
            using (var stream = File.OpenWrite("barcodelib-code128.png"))
            {
                code128Image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
            }

            Console.WriteLine("Generated: barcodelib-code128.png");
            Console.WriteLine("Format: Code128");
            Console.WriteLine("Data: PRODUCT-12345");

            // Generate QR Code
            SKImage qrImage = barcode.Encode(
                BarcodeStandard.Type.QR_Code,
                "https://ironsoftware.com",
                200,
                200
            );

            using (var stream = File.OpenWrite("barcodelib-qr.png"))
            {
                qrImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
            }

            Console.WriteLine("Generated: barcodelib-qr.png");
            Console.WriteLine("Format: QR Code");
            Console.WriteLine("Data: https://ironsoftware.com\n");

            // THE LIMITATION: BarcodeLib cannot read barcodes
            Console.WriteLine("LIMITATION: BarcodeLib has NO reading API");
            Console.WriteLine("The following code DOES NOT EXIST in BarcodeLib:");
            Console.WriteLine("  // var result = barcode.Read(\"barcode.png\");");
            Console.WriteLine("  // var decoded = barcode.Decode(imageBytes);");
            Console.WriteLine("  // NOT AVAILABLE - Need separate library\n");
        }

        /// <summary>
        /// To read barcodes with BarcodeLib, you need a second library.
        /// This demonstrates the integration complexity.
        /// </summary>
        public void BarcodeLibWithZXingForReading()
        {
            Console.WriteLine("=== BarcodeLib + ZXing.Net: Two-Library Solution ===\n");

            // Would need to add ZXing.Net for reading:
            // dotnet add package ZXing.Net
            // dotnet add package ZXing.Net.Bindings.SkiaSharp

            Console.WriteLine("To read barcodes with BarcodeLib, you need:");
            Console.WriteLine("1. BarcodeLib for generation");
            Console.WriteLine("2. ZXing.Net for reading");
            Console.WriteLine("3. ZXing.Net.Bindings.SkiaSharp for image compatibility");
            Console.WriteLine("\nThis means:");
            Console.WriteLine("- 3+ NuGet packages to manage");
            Console.WriteLine("- Multiple APIs to learn");
            Console.WriteLine("- Potential version conflicts between libraries");
            Console.WriteLine("- Increased maintenance burden\n");

            // Example of what ZXing.Net reading code looks like
            // (commented out as it requires the package)
            /*
            using ZXing;
            using ZXing.SkiaSharp;

            var reader = new BarcodeReader<SKBitmap>();
            reader.AutoRotate = true;
            reader.TryHarder = true;

            using var bitmap = SKBitmap.Decode("barcodelib-code128.png");
            var result = reader.Decode(bitmap);

            if (result != null)
            {
                Console.WriteLine($"Read: {result.Text}");
            }
            */
        }

        /// <summary>
        /// IronBarcode handles both generation AND reading in one library.
        /// </summary>
        public void IronBarcodeUnifiedSolution()
        {
            Console.WriteLine("=== IronBarcode: Unified Generation + Reading ===\n");

            // Set license (required for production)
            // IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";

            // GENERATION: Same capability as BarcodeLib
            var code128Barcode = BarcodeWriter.CreateBarcode(
                "PRODUCT-12345",
                BarcodeEncoding.Code128
            );
            code128Barcode.ResizeTo(300, 100);
            code128Barcode.AddBarcodeValueTextBelowBarcode();
            code128Barcode.SaveAsPng("ironbarcode-code128.png");

            Console.WriteLine("Generated: ironbarcode-code128.png");
            Console.WriteLine("Format: Code128");
            Console.WriteLine("Data: PRODUCT-12345");

            var qrBarcode = BarcodeWriter.CreateBarcode(
                "https://ironsoftware.com",
                BarcodeEncoding.QRCode
            );
            qrBarcode.ResizeTo(200, 200);
            qrBarcode.SaveAsPng("ironbarcode-qr.png");

            Console.WriteLine("Generated: ironbarcode-qr.png");
            Console.WriteLine("Format: QR Code");
            Console.WriteLine("Data: https://ironsoftware.com\n");

            // READING: Built-in capability (not available in BarcodeLib)
            Console.WriteLine("READING (not available in BarcodeLib):");

            // Read Code128 barcode back
            var code128Results = BarcodeReader.Read("ironbarcode-code128.png");
            foreach (var result in code128Results)
            {
                Console.WriteLine($"  Read from Code128: {result.Text}");
                Console.WriteLine($"  Detected format: {result.BarcodeType}");
            }

            // Read QR code back
            var qrResults = BarcodeReader.Read("ironbarcode-qr.png");
            foreach (var result in qrResults)
            {
                Console.WriteLine($"  Read from QR: {result.Text}");
                Console.WriteLine($"  Detected format: {result.BarcodeType}");
            }

            Console.WriteLine("\nKey Advantage: Same library for read AND write");
        }

        /// <summary>
        /// Real-world scenario: Verifying generated barcodes are scannable.
        /// BarcodeLib cannot do this; IronBarcode can.
        /// </summary>
        public void VerifyGeneratedBarcode()
        {
            Console.WriteLine("\n=== Real-World Scenario: Barcode Verification ===\n");

            Console.WriteLine("Scenario: After generating barcodes for shipping labels,");
            Console.WriteLine("you want to verify each barcode is readable before printing.\n");

            // BarcodeLib approach (cannot verify)
            Console.WriteLine("BarcodeLib approach:");
            Console.WriteLine("  1. Generate barcode");
            Console.WriteLine("  2. ??? (Cannot read/verify with same library)");
            Console.WriteLine("  3. Ship label and hope it scans");
            Console.WriteLine("  Result: No quality assurance before printing\n");

            // IronBarcode approach (can verify)
            Console.WriteLine("IronBarcode approach:");

            string[] productCodes = { "SKU-001", "SKU-002", "SKU-003" };

            foreach (string code in productCodes)
            {
                // Generate
                var barcode = BarcodeWriter.CreateBarcode(code, BarcodeEncoding.Code128);
                string filename = $"label-{code}.png";
                barcode.SaveAsPng(filename);

                // Verify (same library)
                var readResults = BarcodeReader.Read(filename);
                bool verified = readResults.Any(r => r.Text == code);

                Console.WriteLine($"  {code}: Generated -> {(verified ? "VERIFIED" : "FAILED")}");
            }

            Console.WriteLine("\n  Result: Quality assurance built into workflow");
        }

        public static void Main(string[] args)
        {
            var example = new GenerationOnlyExample();

            example.BarcodeLibGeneration();
            example.BarcodeLibWithZXingForReading();
            example.IronBarcodeUnifiedSolution();
            example.VerifyGeneratedBarcode();

            Console.WriteLine("\n=== Summary ===");
            Console.WriteLine("BarcodeLib: Generation-only, requires separate library for reading");
            Console.WriteLine("IronBarcode: Unified generation + reading in single library");
        }
    }
}
