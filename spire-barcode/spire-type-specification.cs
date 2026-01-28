/*
 * Spire.Barcode Manual Type Specification vs IronBarcode Auto-Detection
 *
 * This example demonstrates the fundamental API difference between
 * Spire.Barcode (requires barcode type specification) and IronBarcode
 * (automatic format detection).
 *
 * Key differences demonstrated:
 * 1. Manual type specification requirement
 * 2. Handling unknown barcode formats
 * 3. Multi-format scanning approaches
 * 4. Real-world mixed-format scenarios
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/about-us/authors/jacobmellor/
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// Spire.Barcode
// Install: dotnet add package Spire.Barcode
using Spire.Barcode;

// IronBarcode
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace BarcodeComparison
{
    /// <summary>
    /// Demonstrates manual type specification vs automatic detection
    /// </summary>
    public class TypeSpecificationDemo
    {
        /// <summary>
        /// Demonstrates Spire.Barcode's requirement for type specification
        /// </summary>
        public void DemonstrateSpireTypeRequirement()
        {
            Console.WriteLine("=== Spire.Barcode Type Specification Requirement ===\n");

            // Create test barcodes of different types
            string code128Path = "test-code128.png";
            string qrCodePath = "test-qrcode.png";

            // Generate test images using IronBarcode (for demonstration)
            BarcodeWriter.CreateBarcode("CODE128DATA", BarcodeEncoding.Code128)
                .SaveAsPng(code128Path);
            BarcodeWriter.CreateBarcode("QRCODE-DATA", BarcodeEncoding.QRCode)
                .SaveAsPng(qrCodePath);

            Console.WriteLine("Spire.Barcode requires specifying the barcode type:\n");

            BarcodeScanner scanner = new BarcodeScanner();

            // Correct type specification - works
            Console.WriteLine("1. Correct type specification (Code128 image with Code128 type):");
            try
            {
                string[] results = scanner.Scan(code128Path, BarCodeType.Code128);
                Console.WriteLine($"   Result: {(results.Length > 0 ? results[0] : "No barcode found")}");
                Console.WriteLine("   Status: Success - correct type specified\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}\n");
            }

            // Wrong type specification - fails
            Console.WriteLine("2. Wrong type specification (Code128 image with QRCode type):");
            try
            {
                string[] results = scanner.Scan(code128Path, BarCodeType.QRCode);
                Console.WriteLine($"   Result: {(results.Length > 0 ? results[0] : "No barcode found")}");
                Console.WriteLine("   Status: Fails to detect - wrong type specified\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}\n");
            }

            // Unknown format scenario
            Console.WriteLine("3. Unknown format scenario:");
            Console.WriteLine("   If you don't know the barcode type, you must scan with all possible types:\n");

            // Cleanup
            File.Delete(code128Path);
            File.Delete(qrCodePath);
        }

        /// <summary>
        /// Demonstrates the multi-type scanning approach required by Spire.Barcode
        /// </summary>
        public void DemonstrateSpireMultiTypeScan()
        {
            Console.WriteLine("=== Spire.Barcode Multi-Type Scanning ===\n");

            // Create test barcode
            string testPath = "test-barcode.png";
            BarcodeWriter.CreateBarcode("UNKNOWN-FORMAT", BarcodeEncoding.DataMatrix)
                .SaveAsPng(testPath);

            Console.WriteLine("When barcode format is unknown, Spire requires scanning with all types:\n");

            // Define all possible barcode types to check
            BarCodeType[] allTypes = new BarCodeType[]
            {
                BarCodeType.Code128,
                BarCodeType.Code39,
                BarCodeType.Code39Extended,
                BarCodeType.Code93,
                BarCodeType.Code93Extended,
                BarCodeType.EAN13,
                BarCodeType.EAN8,
                BarCodeType.UPCA,
                BarCodeType.UPCE,
                BarCodeType.QRCode,
                BarCodeType.DataMatrix,
                BarCodeType.PDF417
                // ... many more types
            };

            Console.WriteLine($"Scanning with {allTypes.Length} different barcode types...\n");

            BarcodeScanner scanner = new BarcodeScanner();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            string foundResult = null;
            string foundType = null;
            int scansPerformed = 0;

            foreach (var barcodeType in allTypes)
            {
                scansPerformed++;
                try
                {
                    string[] results = scanner.Scan(testPath, barcodeType);
                    if (results.Length > 0)
                    {
                        foundResult = results[0];
                        foundType = barcodeType.ToString();
                        break;
                    }
                }
                catch
                {
                    // Type didn't match, continue to next
                }
            }

            stopwatch.Stop();

            Console.WriteLine($"Result: {foundResult ?? "Not found"}");
            Console.WriteLine($"Detected type: {foundType ?? "Unknown"}");
            Console.WriteLine($"Scans performed: {scansPerformed}");
            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds}ms\n");

            Console.WriteLine("Problems with this approach:");
            Console.WriteLine("- Multiple scan passes required");
            Console.WriteLine("- Performance scales linearly with type count");
            Console.WriteLine("- Complex code to manage type iteration");
            Console.WriteLine("- Easy to miss types in the list\n");

            File.Delete(testPath);
        }

        /// <summary>
        /// Demonstrates IronBarcode's automatic detection
        /// </summary>
        public void DemonstrateIronBarcodeAutoDetection()
        {
            Console.WriteLine("=== IronBarcode Automatic Detection ===\n");

            // Create test barcodes of different types
            var testCases = new (string filename, BarcodeEncoding encoding, string data)[]
            {
                ("auto-code128.png", BarcodeEncoding.Code128, "CODE128-DATA"),
                ("auto-qrcode.png", BarcodeEncoding.QRCode, "QRCODE-DATA"),
                ("auto-datamatrix.png", BarcodeEncoding.DataMatrix, "DATAMATRIX-DATA"),
                ("auto-ean13.png", BarcodeEncoding.EAN13, "5901234123457"),
                ("auto-pdf417.png", BarcodeEncoding.PDF417, "PDF417-DATA")
            };

            // Generate test images
            foreach (var (filename, encoding, data) in testCases)
            {
                BarcodeWriter.CreateBarcode(data, encoding).SaveAsPng(filename);
            }

            Console.WriteLine("IronBarcode automatically detects barcode format:\n");

            var stopwatch = new Stopwatch();

            foreach (var (filename, expectedEncoding, expectedData) in testCases)
            {
                stopwatch.Restart();

                // Single call - no type specification needed
                var results = BarcodeReader.Read(filename);

                stopwatch.Stop();

                if (results.Any())
                {
                    var result = results.First();
                    Console.WriteLine($"File: {filename}");
                    Console.WriteLine($"  Detected type: {result.BarcodeType}");
                    Console.WriteLine($"  Value: {result.Text}");
                    Console.WriteLine($"  Time: {stopwatch.ElapsedMilliseconds}ms\n");
                }
                else
                {
                    Console.WriteLine($"File: {filename} - No barcode found\n");
                }
            }

            Console.WriteLine("Benefits of automatic detection:");
            Console.WriteLine("- Single scan handles all formats");
            Console.WriteLine("- No need to know format in advance");
            Console.WriteLine("- Simpler code, fewer bugs");
            Console.WriteLine("- Consistent performance regardless of format\n");

            // Cleanup
            foreach (var (filename, _, _) in testCases)
            {
                File.Delete(filename);
            }
        }

        /// <summary>
        /// Demonstrates mixed-format batch processing
        /// </summary>
        public void DemonstrateMixedFormatBatch()
        {
            Console.WriteLine("=== Mixed-Format Batch Processing ===\n");

            // Create a batch of mixed-format barcodes
            var batchFiles = new List<string>();

            var formats = new (BarcodeEncoding encoding, string data)[]
            {
                (BarcodeEncoding.Code128, "ITEM-001"),
                (BarcodeEncoding.QRCode, "https://example.com/product/1"),
                (BarcodeEncoding.DataMatrix, "LOT-2026-001"),
                (BarcodeEncoding.Code128, "ITEM-002"),
                (BarcodeEncoding.EAN13, "5901234123457"),
                (BarcodeEncoding.QRCode, "https://example.com/product/2")
            };

            for (int i = 0; i < formats.Length; i++)
            {
                string filename = $"batch-{i:D3}.png";
                BarcodeWriter.CreateBarcode(formats[i].data, formats[i].encoding)
                    .SaveAsPng(filename);
                batchFiles.Add(filename);
            }

            // Spire.Barcode approach (complex)
            Console.WriteLine("Spire.Barcode batch processing (mixed formats):\n");
            Console.WriteLine("  // Must iterate through all types for each file");
            Console.WriteLine("  // Code becomes complex and slow");
            Console.WriteLine(@"
    BarcodeScanner scanner = new BarcodeScanner();
    BarCodeType[] allTypes = { BarCodeType.Code128, BarCodeType.QRCode, ... };

    foreach (var file in batchFiles)
    {
        foreach (var barcodeType in allTypes)
        {
            string[] results = scanner.Scan(file, barcodeType);
            if (results.Length > 0)
            {
                // Found it, break inner loop
                break;
            }
        }
    }
");
            Console.WriteLine("  Time complexity: O(files * types)\n");

            // IronBarcode approach (simple)
            Console.WriteLine("IronBarcode batch processing (mixed formats):\n");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Single call processes all files, all formats
            var allResults = BarcodeReader.Read(batchFiles.ToArray());

            stopwatch.Stop();

            Console.WriteLine($"  Files processed: {batchFiles.Count}");
            Console.WriteLine($"  Barcodes found: {allResults.Count()}");
            Console.WriteLine($"  Total time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Average per file: {stopwatch.ElapsedMilliseconds / batchFiles.Count}ms\n");

            Console.WriteLine("  Results:");
            foreach (var result in allResults)
            {
                Console.WriteLine($"    {Path.GetFileName(result.InputPath ?? "unknown")}: {result.BarcodeType} = {result.Text}");
            }
            Console.WriteLine();

            // Cleanup
            foreach (var file in batchFiles)
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Real-world scenario: Processing shipping documents
        /// </summary>
        public void DemonstrateRealWorldScenario()
        {
            Console.WriteLine("=== Real-World Scenario: Shipping Documents ===\n");

            Console.WriteLine("Scenario: Processing shipping labels with mixed barcode formats");
            Console.WriteLine("Labels may contain: Code128 (tracking), QR (URL), DataMatrix (lot info)\n");

            // Create simulated shipping label image with multiple barcodes
            string labelPath = "shipping-label.png";

            // In reality, this would be a scanned image
            // For demo, we'll create individual barcodes and describe the scenario

            Console.WriteLine("Spire.Barcode approach:");
            Console.WriteLine(@"
    // Must know or guess what formats the label might contain
    BarcodeScanner scanner = new BarcodeScanner();
    List<string> allData = new List<string>();

    // Try each expected format
    var trackingBarcodes = scanner.Scan(labelPath, BarCodeType.Code128);
    allData.AddRange(trackingBarcodes);

    var urlBarcodes = scanner.Scan(labelPath, BarCodeType.QRCode);
    allData.AddRange(urlBarcodes);

    var lotBarcodes = scanner.Scan(labelPath, BarCodeType.DataMatrix);
    allData.AddRange(lotBarcodes);

    // Problem: What if label has unexpected format?
    // Must add more scan passes for each possibility
");

            Console.WriteLine("\nIronBarcode approach:");
            Console.WriteLine(@"
    // Automatically handles any format on the label
    var results = BarcodeReader.Read(labelPath);

    foreach (var barcode in results)
    {
        switch (barcode.BarcodeType)
        {
            case BarcodeType.Code128:
                ProcessTrackingNumber(barcode.Text);
                break;
            case BarcodeType.QRCode:
                ProcessUrl(barcode.Text);
                break;
            case BarcodeType.DataMatrix:
                ProcessLotInfo(barcode.Text);
                break;
            default:
                ProcessUnexpectedBarcode(barcode);
                break;
        }
    }
");

            Console.WriteLine("\nKey advantages of auto-detection in this scenario:");
            Console.WriteLine("- Handles format changes without code updates");
            Console.WriteLine("- Future-proof for new barcode types");
            Console.WriteLine("- Single scan pass vs multiple");
            Console.WriteLine("- Type information included in results for routing\n");
        }

        /// <summary>
        /// Summary comparison
        /// </summary>
        public void PrintSummary()
        {
            Console.WriteLine("=== Summary: Type Specification Comparison ===\n");

            Console.WriteLine("| Aspect                    | Spire.Barcode           | IronBarcode             |");
            Console.WriteLine("|---------------------------|-------------------------|-------------------------|");
            Console.WriteLine("| Type specification        | Required                | Not required            |");
            Console.WriteLine("| Unknown format handling   | Scan with all types     | Automatic detection     |");
            Console.WriteLine("| Multi-format documents    | Multiple scan passes    | Single scan             |");
            Console.WriteLine("| Code complexity           | High                    | Low                     |");
            Console.WriteLine("| Performance (unknown fmt) | O(n) where n = types    | O(1)                    |");
            Console.WriteLine("| Future format support     | Add to type list        | Automatic               |");
            Console.WriteLine();

            Console.WriteLine("Recommendation:");
            Console.WriteLine("- Use IronBarcode when barcode format is unknown or variable");
            Console.WriteLine("- Use IronBarcode for mixed-format batch processing");
            Console.WriteLine("- Spire.Barcode type specification only practical for fixed-format workflows\n");
        }
    }

    /// <summary>
    /// Program entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var demo = new TypeSpecificationDemo();

            demo.DemonstrateSpireTypeRequirement();
            demo.DemonstrateSpireMultiTypeScan();
            demo.DemonstrateIronBarcodeAutoDetection();
            demo.DemonstrateMixedFormatBatch();
            demo.DemonstrateRealWorldScenario();
            demo.PrintSummary();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
