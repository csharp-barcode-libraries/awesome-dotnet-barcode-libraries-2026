/*
 * OnBarcode Separate Reader vs IronBarcode Unified SDK
 *
 * This example demonstrates the fundamental product model difference:
 * OnBarcode requires separate purchase for reading capability,
 * while IronBarcode includes both generation and reading in one package.
 *
 * Key differences demonstrated:
 * 1. OnBarcode Generator vs Reader as separate products
 * 2. Two licenses vs one license
 * 3. Two APIs vs unified API
 * 4. Cost implications of split model
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/about-us/authors/jacobmellor/
 */

using System;
using System.Collections.Generic;
using System.IO;

// OnBarcode Generator
// Install: dotnet add package OnBarcode.Barcode.Generator
// Note: Reading requires SEPARATE OnBarcode.Barcode.Reader package
using OnBarcode.Barcode;

// IronBarcode (includes BOTH generation and reading)
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace BarcodeComparison
{
    /// <summary>
    /// Demonstrates OnBarcode's split product model vs IronBarcode's unified approach
    /// </summary>
    public class SeparateReaderDemo
    {
        /// <summary>
        /// Demonstrates OnBarcode's generation-only capability
        /// </summary>
        public void DemonstrateOnBarcodeGeneration()
        {
            Console.WriteLine("=== OnBarcode Generator (Standard Package) ===\n");

            Console.WriteLine("OnBarcode.Barcode.Generator provides barcode creation:\n");

            // Basic generation with OnBarcode
            try
            {
                Barcode barcode = new Barcode();
                barcode.Symbology = Symbology.Code128Auto;
                barcode.Data = "PRODUCT-12345";
                barcode.Resolution = 96;
                barcode.BarWidth = 1;
                barcode.BarHeight = 60;
                barcode.ShowText = true;

                barcode.drawBarcode("onbarcode-generated.png");

                Console.WriteLine("Generated: onbarcode-generated.png");
                Console.WriteLine("Generation works with OnBarcode.Barcode.Generator package\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generation error: {ex.Message}\n");
            }

            // Attempting to read - NOT POSSIBLE with generator package
            Console.WriteLine("Attempting to read barcode with Generator package...\n");
            Console.WriteLine("ERROR: OnBarcode.Barcode.Generator does NOT include reading capability");
            Console.WriteLine("Reading requires separate OnBarcode.Barcode.Reader package");
            Console.WriteLine("Reader package requires SEPARATE license purchase\n");
        }

        /// <summary>
        /// Demonstrates OnBarcode's separate reader requirement
        /// </summary>
        public void DemonstrateOnBarcodeReaderRequirement()
        {
            Console.WriteLine("=== OnBarcode Reader (Separate Purchase Required) ===\n");

            Console.WriteLine("To read barcodes with OnBarcode, you must:\n");

            Console.WriteLine("1. Purchase OnBarcode Reader SDK separately");
            Console.WriteLine("   - Contact sales for pricing");
            Console.WriteLine("   - Separate license from Generator");
            Console.WriteLine("   - Additional cost\n");

            Console.WriteLine("2. Install separate NuGet package:");
            Console.WriteLine("   dotnet add package OnBarcode.Barcode.Reader\n");

            Console.WriteLine("3. Configure separate license:");
            Console.WriteLine(@"
    // OnBarcode requires TWO licenses
    // License for Generator
    OnBarcode.Barcode.License.SetLicense(""GENERATOR-LICENSE"");

    // Separate license for Reader
    OnBarcode.Barcode.Reader.License.SetLicense(""READER-LICENSE"");
");

            Console.WriteLine("4. Use different API for reading:");
            Console.WriteLine(@"
    // Different namespace
    using OnBarcode.Barcode.Reader;

    // Different class
    BarcodeReader reader = new BarcodeReader();
    reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128 };
    string[] results = reader.Scan(""barcode.png"");
");

            Console.WriteLine("\nProblems with this approach:");
            Console.WriteLine("- Two separate purchases");
            Console.WriteLine("- Two separate licenses to manage");
            Console.WriteLine("- Two different APIs to learn");
            Console.WriteLine("- Two packages to track and update");
            Console.WriteLine("- Hidden total cost (need both quotes)\n");
        }

        /// <summary>
        /// Demonstrates IronBarcode's unified approach
        /// </summary>
        public void DemonstrateIronBarcodeUnified()
        {
            Console.WriteLine("=== IronBarcode Unified SDK ===\n");

            Console.WriteLine("IronBarcode includes BOTH generation and reading:\n");

            // Generate barcode
            Console.WriteLine("Generation:");
            try
            {
                var barcode = BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128);
                barcode.SaveAsPng("iron-generated.png");
                Console.WriteLine("Generated: iron-generated.png\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generation error: {ex.Message}\n");
            }

            // Read barcode - SAME package, SAME license
            Console.WriteLine("Reading (same package, same license):");
            try
            {
                var results = BarcodeReader.Read("iron-generated.png");
                foreach (var result in results)
                {
                    Console.WriteLine($"Read: {result.BarcodeType} = {result.Text}");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reading error: {ex.Message}\n");
            }

            Console.WriteLine("Benefits of unified SDK:");
            Console.WriteLine("- Single NuGet package");
            Console.WriteLine("- Single license purchase");
            Console.WriteLine("- One API to learn");
            Console.WriteLine("- Clear total cost upfront");
            Console.WriteLine("- Consistent updates\n");
        }

        /// <summary>
        /// Demonstrates real-world workflow comparison
        /// </summary>
        public void DemonstrateWorkflowComparison()
        {
            Console.WriteLine("=== Real-World Workflow: Label Verification System ===\n");

            Console.WriteLine("Scenario: Print labels, then scan to verify correct encoding\n");

            // OnBarcode approach (two products)
            Console.WriteLine("OnBarcode approach (requires two products):\n");
            Console.WriteLine(@"
    // Product 1: OnBarcode Generator
    using OnBarcode.Barcode;

    // Product 2: OnBarcode Reader (separate purchase!)
    using OnBarcode.Barcode.Reader;

    // Two separate licenses required
    OnBarcode.Barcode.License.SetLicense(""GENERATOR-KEY"");
    OnBarcode.Barcode.Reader.License.SetLicense(""READER-KEY"");

    public class LabelVerifier
    {
        public void CreateAndVerify(string data, string outputPath)
        {
            // Generate with Generator SDK
            Barcode barcode = new Barcode();
            barcode.Symbology = Symbology.Code128Auto;
            barcode.Data = data;
            barcode.drawBarcode(outputPath);

            // Verify with Reader SDK (different API)
            BarcodeReader reader = new BarcodeReader();
            reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128 };
            string[] results = reader.Scan(outputPath);

            if (results.Length == 0 || results[0] != data)
            {
                throw new Exception(""Verification failed"");
            }
        }
    }
");

            // IronBarcode approach (one product)
            Console.WriteLine("\nIronBarcode approach (single product):\n");
            Console.WriteLine(@"
    // Single product includes everything
    using IronBarCode;

    // Single license
    IronBarCode.License.LicenseKey = ""YOUR-KEY"";

    public class LabelVerifier
    {
        public void CreateAndVerify(string data, string outputPath)
        {
            // Generate
            BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
                .SaveAsPng(outputPath);

            // Verify (same API family)
            var results = BarcodeReader.Read(outputPath);

            if (!results.Any() || results.First().Text != data)
            {
                throw new Exception(""Verification failed"");
            }
        }
    }
");

            Console.WriteLine("\nKey differences:");
            Console.WriteLine("- OnBarcode: Two using statements, two licenses, two API patterns");
            Console.WriteLine("- IronBarcode: One using statement, one license, consistent API\n");
        }

        /// <summary>
        /// Demonstrates cost comparison
        /// </summary>
        public void DemonstrateCostComparison()
        {
            Console.WriteLine("=== Cost Comparison: Split vs Unified Model ===\n");

            Console.WriteLine("OnBarcode cost structure:\n");
            Console.WriteLine("  Generator SDK: Contact sales (unknown price)");
            Console.WriteLine("  Reader SDK:    Contact sales (unknown price, separate)");
            Console.WriteLine("  Total cost:    Unknown until both quotes received\n");

            Console.WriteLine("IronBarcode cost structure:\n");
            Console.WriteLine("  Lite (1 dev):       $749  - includes generation AND reading");
            Console.WriteLine("  Professional:       $1,499 - includes generation AND reading");
            Console.WriteLine("  Unlimited:          $2,999 - includes generation AND reading\n");

            Console.WriteLine("Scenario: 5-developer team needs both generation and reading\n");

            Console.WriteLine("OnBarcode:");
            Console.WriteLine("  Generator SDK (5 devs):  $X (unknown)");
            Console.WriteLine("  Reader SDK (5 devs):     $Y (unknown)");
            Console.WriteLine("  Total:                   $X + $Y");
            Console.WriteLine("  Budget planning:         Impossible without quotes\n");

            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  Professional (10 devs):  $1,499");
            Console.WriteLine("  Includes everything:     Yes");
            Console.WriteLine("  Budget planning:         Clear from day one\n");

            Console.WriteLine("Additional OnBarcode considerations:");
            Console.WriteLine("- Price may vary by negotiation");
            Console.WriteLine("- Updates/support terms may differ per product");
            Console.WriteLine("- Version compatibility between products not guaranteed\n");
        }

        /// <summary>
        /// Demonstrates API consistency advantages
        /// </summary>
        public void DemonstrateApiConsistency()
        {
            Console.WriteLine("=== API Consistency: Separate Products vs Unified SDK ===\n");

            Console.WriteLine("OnBarcode API differences between Generator and Reader:\n");
            Console.WriteLine(@"
    // Generator namespace
    using OnBarcode.Barcode;

    // Generator pattern
    Barcode barcode = new Barcode();
    barcode.Symbology = Symbology.Code128Auto;
    barcode.Data = ""data"";
    barcode.drawBarcode(""output.png"");

    // ---------------------------------

    // Reader namespace (different!)
    using OnBarcode.Barcode.Reader;

    // Reader pattern (different!)
    BarcodeReader reader = new BarcodeReader();
    reader.BarcodeTypes = new BarcodeType[] { ... };
    string[] results = reader.Scan(""input.png"");
");

            Console.WriteLine("\nIronBarcode consistent API:\n");
            Console.WriteLine(@"
    // Single namespace
    using IronBarCode;

    // Consistent writing pattern
    BarcodeWriter.CreateBarcode(""data"", BarcodeEncoding.Code128)
        .SaveAsPng(""output.png"");

    // Consistent reading pattern
    var results = BarcodeReader.Read(""input.png"");
    // Returns BarcodeResult with .BarcodeType and .Text
");

            Console.WriteLine("\nBenefits of consistent API:");
            Console.WriteLine("- Less cognitive load");
            Console.WriteLine("- Faster onboarding for team members");
            Console.WriteLine("- Fewer bugs from API differences");
            Console.WriteLine("- Single documentation source\n");
        }

        /// <summary>
        /// Summary comparison
        /// </summary>
        public void PrintSummary()
        {
            Console.WriteLine("=== Summary: Separate Reader vs Unified SDK ===\n");

            Console.WriteLine("| Aspect                  | OnBarcode              | IronBarcode           |");
            Console.WriteLine("|-------------------------|------------------------|-----------------------|");
            Console.WriteLine("| Generation package      | OnBarcode.Barcode.Gen  | IronBarcode           |");
            Console.WriteLine("| Reading package         | SEPARATE purchase      | Same package          |");
            Console.WriteLine("| Licenses needed         | 2 (gen + reader)       | 1                     |");
            Console.WriteLine("| APIs to learn           | 2 different            | 1 unified             |");
            Console.WriteLine("| Total cost visibility   | Unknown (contact)      | Published             |");
            Console.WriteLine("| Package management      | 2 packages             | 1 package             |");
            Console.WriteLine("| Version compatibility   | Must verify            | Guaranteed            |");
            Console.WriteLine();

            Console.WriteLine("Recommendation:");
            Console.WriteLine("- Choose IronBarcode for applications needing both read and write");
            Console.WriteLine("- OnBarcode split model adds cost and complexity");
            Console.WriteLine("- Unified SDK simplifies development and maintenance\n");
        }
    }

    /// <summary>
    /// Program entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var demo = new SeparateReaderDemo();

            demo.DemonstrateOnBarcodeGeneration();
            demo.DemonstrateOnBarcodeReaderRequirement();
            demo.DemonstrateIronBarcodeUnified();
            demo.DemonstrateWorkflowComparison();
            demo.DemonstrateCostComparison();
            demo.DemonstrateApiConsistency();
            demo.PrintSummary();

            // Cleanup
            if (File.Exists("onbarcode-generated.png"))
                File.Delete("onbarcode-generated.png");
            if (File.Exists("iron-generated.png"))
                File.Delete("iron-generated.png");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
