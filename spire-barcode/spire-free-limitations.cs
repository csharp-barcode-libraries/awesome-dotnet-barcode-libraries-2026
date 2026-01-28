/*
 * Spire.Barcode Free Version Limitations vs IronBarcode Trial
 *
 * This example demonstrates the practical differences between
 * FreeSpire.Barcode's limited functionality and IronBarcode's
 * full-featured trial experience.
 *
 * Key differences demonstrated:
 * 1. Watermarks on generated barcodes
 * 2. Performance degradation
 * 3. Limited symbology support
 * 4. Registration requirements
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/about-us/authors/jacobmellor/
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// FreeSpire.Barcode
// Install: dotnet add package FreeSpire.Barcode
using Spire.Barcode;

// IronBarcode
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace BarcodeComparison
{
    /// <summary>
    /// Demonstrates FreeSpire.Barcode limitations compared to IronBarcode trial
    /// </summary>
    public class FreeVersionLimitationsDemo
    {
        /// <summary>
        /// Demonstrates watermark differences between free and trial versions
        /// </summary>
        public void CompareWatermarks()
        {
            Console.WriteLine("=== Watermark Comparison ===\n");

            string testData = "DEMO-12345";

            // FreeSpire.Barcode - Generates with evaluation watermark
            Console.WriteLine("FreeSpire.Barcode:");
            Console.WriteLine("- Evaluation watermark covers significant portion of barcode");
            Console.WriteLine("- Watermark text appears across the barcode image");
            Console.WriteLine("- Makes generated barcodes unsuitable for production use");
            Console.WriteLine("- Cannot be disabled without commercial license\n");

            try
            {
                BarcodeSettings settings = new BarcodeSettings();
                settings.Type = BarCodeType.Code128;
                settings.Data = testData;
                settings.ShowText = true;

                BarCodeGenerator generator = new BarCodeGenerator(settings);
                Image barcodeImage = generator.GenerateImage();

                // The generated image contains evaluation watermark
                barcodeImage.Save("spire-free-watermarked.png", ImageFormat.Png);

                Console.WriteLine("Generated: spire-free-watermarked.png");
                Console.WriteLine("(Contains evaluation watermark)\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with FreeSpire: {ex.Message}\n");
            }

            // IronBarcode - Full functionality with minimal watermark
            Console.WriteLine("IronBarcode Trial:");
            Console.WriteLine("- Small watermark at edge of image");
            Console.WriteLine("- Barcode itself is fully readable");
            Console.WriteLine("- Suitable for demos and evaluation");
            Console.WriteLine("- Full features available during trial\n");

            try
            {
                var ironBarcode = BarcodeWriter.CreateBarcode(testData, BarcodeEncoding.Code128);
                ironBarcode.SaveAsPng("iron-trial-barcode.png");

                Console.WriteLine("Generated: iron-trial-barcode.png");
                Console.WriteLine("(Minimal trial watermark, fully functional)\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with IronBarcode: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Demonstrates performance differences between free and commercial versions
        /// </summary>
        public void ComparePerformance()
        {
            Console.WriteLine("=== Performance Comparison ===\n");

            int iterations = 10;

            // FreeSpire.Barcode - Intentionally degraded performance
            Console.WriteLine("FreeSpire.Barcode reading performance:");
            Console.WriteLine("- Scanning speed intentionally reduced in free version");
            Console.WriteLine("- Multi-barcode scanning significantly slower");
            Console.WriteLine("- Batch processing becomes impractical\n");

            var stopwatch = new Stopwatch();

            // Generate a test barcode first
            string testImagePath = "test-barcode.png";
            BarcodeWriter.CreateBarcode("TEST12345", BarcodeEncoding.Code128)
                .SaveAsPng(testImagePath);

            // FreeSpire performance test
            try
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    BarcodeScanner scanner = new BarcodeScanner();
                    string[] results = scanner.Scan(testImagePath, BarCodeType.Code128);
                }
                stopwatch.Stop();

                Console.WriteLine($"FreeSpire.Barcode: {iterations} scans in {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"Average: {stopwatch.ElapsedMilliseconds / iterations}ms per scan\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FreeSpire scan error: {ex.Message}\n");
            }

            // IronBarcode performance test
            stopwatch.Reset();

            try
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    var results = BarcodeReader.Read(testImagePath);
                }
                stopwatch.Stop();

                Console.WriteLine($"IronBarcode: {iterations} scans in {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"Average: {stopwatch.ElapsedMilliseconds / iterations}ms per scan\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IronBarcode scan error: {ex.Message}\n");
            }

            // Cleanup
            if (File.Exists(testImagePath))
                File.Delete(testImagePath);
        }

        /// <summary>
        /// Demonstrates symbology support differences
        /// </summary>
        public void CompareSymbologySupport()
        {
            Console.WriteLine("=== Symbology Support Comparison ===\n");

            // FreeSpire.Barcode limited symbologies
            Console.WriteLine("FreeSpire.Barcode symbology support:");
            Console.WriteLine("- ~20 barcode types in free version");
            Console.WriteLine("- Some 2D formats restricted");
            Console.WriteLine("- Postal codes may be limited");
            Console.WriteLine("- Full 39+ types require commercial license\n");

            // Types commonly restricted in free version
            var restrictedTypes = new[]
            {
                "PDF417 (may be limited)",
                "Aztec Code (may require commercial)",
                "MaxiCode (commercial only)",
                "Postal formats (varies)"
            };

            Console.WriteLine("Potentially restricted in FreeSpire.Barcode:");
            foreach (var type in restrictedTypes)
            {
                Console.WriteLine($"  - {type}");
            }
            Console.WriteLine();

            // IronBarcode full support in trial
            Console.WriteLine("IronBarcode trial symbology support:");
            Console.WriteLine("- All 50+ barcode types available");
            Console.WriteLine("- No feature restrictions during trial");
            Console.WriteLine("- Full 2D format support (QR, DataMatrix, PDF417, Aztec)");
            Console.WriteLine("- Complete postal code support\n");

            // Demonstrate generating multiple formats with IronBarcode
            Console.WriteLine("Generating multiple format examples with IronBarcode:\n");

            var formats = new (string name, BarcodeEncoding encoding)[]
            {
                ("Code 128", BarcodeEncoding.Code128),
                ("QR Code", BarcodeEncoding.QRCode),
                ("DataMatrix", BarcodeEncoding.DataMatrix),
                ("PDF417", BarcodeEncoding.PDF417),
                ("Aztec", BarcodeEncoding.Aztec)
            };

            foreach (var (name, encoding) in formats)
            {
                try
                {
                    var barcode = BarcodeWriter.CreateBarcode("TEST-DATA", encoding);
                    string filename = $"iron-{name.Replace(" ", "-").ToLower()}.png";
                    barcode.SaveAsPng(filename);
                    Console.WriteLine($"  Generated: {filename} ({name})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error generating {name}: {ex.Message}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates registration requirements for free version
        /// </summary>
        public void DemonstrateRegistrationRequirement()
        {
            Console.WriteLine("=== Registration Requirements ===\n");

            // FreeSpire.Barcode registration requirement
            Console.WriteLine("FreeSpire.Barcode registration:");
            Console.WriteLine("1. Download FreeSpire.Barcode from NuGet or website");
            Console.WriteLine("2. Apply for free registration key from E-iceblue");
            Console.WriteLine("3. Provide contact information for registration");
            Console.WriteLine("4. Set registration key in code to suppress warnings\n");

            // Code example for FreeSpire registration
            Console.WriteLine("FreeSpire.Barcode registration code:");
            Console.WriteLine(@"
    // Apply registration key (obtained from E-iceblue)
    Spire.Barcode.BarcodeSettings.ApplyKey(""FREE-REGISTRATION-KEY"");

    // Without this, warning dialogs may appear
    // during barcode generation or scanning
");

            // IronBarcode trial approach
            Console.WriteLine("IronBarcode trial approach:");
            Console.WriteLine("1. Install IronBarcode from NuGet");
            Console.WriteLine("2. Use immediately - no registration required");
            Console.WriteLine("3. Trial watermark appears but features work");
            Console.WriteLine("4. Request trial license for watermark-free evaluation\n");

            // Code example for IronBarcode
            Console.WriteLine("IronBarcode trial code:");
            Console.WriteLine(@"
    // Works immediately with trial watermark
    var barcode = BarcodeWriter.CreateBarcode(""data"", BarcodeEncoding.Code128);
    barcode.SaveAsPng(""output.png"");

    // Optional: Request trial license for watermark-free evaluation
    // IronBarCode.License.LicenseKey = ""TRIAL-LICENSE-KEY"";
");
        }

        /// <summary>
        /// Summary of key differences
        /// </summary>
        public void PrintSummary()
        {
            Console.WriteLine("=== Summary: Free Version vs Trial Comparison ===\n");

            Console.WriteLine("| Aspect                 | FreeSpire.Barcode      | IronBarcode Trial      |");
            Console.WriteLine("|------------------------|------------------------|------------------------|");
            Console.WriteLine("| Watermarks             | Large, intrusive       | Small, edge-placed     |");
            Console.WriteLine("| Feature access         | Limited subset         | Full features          |");
            Console.WriteLine("| Performance            | Intentionally degraded | Full speed             |");
            Console.WriteLine("| Symbology support      | ~20 types              | 50+ types              |");
            Console.WriteLine("| Registration required  | Yes (free key)         | No                     |");
            Console.WriteLine("| Support available      | No                     | Yes                    |");
            Console.WriteLine("| Time limit             | None                   | 30 days                |");
            Console.WriteLine("| Production suitable    | No                     | With license           |");
            Console.WriteLine();

            Console.WriteLine("Recommendation:");
            Console.WriteLine("- For genuine evaluation: IronBarcode trial provides better insight");
            Console.WriteLine("- FreeSpire.Barcode limitations make real evaluation difficult");
            Console.WriteLine("- Consider actual production requirements when evaluating\n");
        }
    }

    /// <summary>
    /// Program entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var demo = new FreeVersionLimitationsDemo();

            demo.CompareWatermarks();
            demo.ComparePerformance();
            demo.CompareSymbologySupport();
            demo.DemonstrateRegistrationRequirement();
            demo.PrintSummary();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
