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
// Note: Reading requires SEPARATE OnBarcode.Barcode.Reader package and license
using OnBarcode.Barcode;

// IronBarcode (includes BOTH generation and reading; NuGet package id is "BarCode")
// Install: dotnet add package BarCode
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
                Linear barcode = new Linear();
                barcode.Type = BarcodeType.CODE128;
                barcode.Data = "PRODUCT-12345";
                barcode.Resolution = 96;
                barcode.X = 1;
                barcode.BarcodeHeight = 60;
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
    // OnBarcode requires TWO licenses (Generator and Reader purchased separately)
    using OnBarcode.Barcode;

    License.RegisterLicense(""GENERATOR-LICENSE""); // from Generator purchase
    License.RegisterLicense(""READER-LICENSE"");    // from Reader purchase
");

            Console.WriteLine("4. Use the static Reader API:");
            Console.WriteLine(@"
    // Same root namespace, but BarcodeScanner ships only with the Reader package
    using OnBarcode.Barcode;

    // Static API; format must be specified up front
    string[] results = BarcodeScanner.Scan(""barcode.png"", BarcodeType.CODE128);
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
                    Console.WriteLine($"Read: {result.BarcodeType} = {result.Value}");
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
    // Two NuGet packages required:
    //   dotnet add package OnBarcode.Barcode.Generator
    //   dotnet add package OnBarcode.Barcode.Reader   (separate purchase!)
    using OnBarcode.Barcode;

    // Two separate licenses required
    License.RegisterLicense(""GENERATOR-KEY"");
    License.RegisterLicense(""READER-KEY"");

    public class LabelVerifier
    {
        public void CreateAndVerify(string data, string outputPath)
        {
            // Generate with Generator SDK
            Linear barcode = new Linear();
            barcode.Type = BarcodeType.CODE128;
            barcode.Data = data;
            barcode.drawBarcode(outputPath);

            // Verify with Reader SDK (static API; explicit BarcodeType)
            string[] results = BarcodeScanner.Scan(outputPath, BarcodeType.CODE128);

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

            if (!results.Any() || results.First().Value != data)
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

            Console.WriteLine("OnBarcode cost structure (per-product, perpetual):\n");
            Console.WriteLine("  .NET Generator Suite Linear (1D only):");
            Console.WriteLine("    Single Developer:    $990");
            Console.WriteLine("    Five Developer:      $1,990");
            Console.WriteLine("    Unlimited Developer: $2,990");
            Console.WriteLine("  .NET Generator Suite Linear + 2D:");
            Console.WriteLine("    Single Developer:    $1,690");
            Console.WriteLine("    Five Developer:      $2,690");
            Console.WriteLine("    Unlimited Developer: $3,990");
            Console.WriteLine("  .NET Barcode Reader SDK (sold separately): from $990\n");

            Console.WriteLine("IronBarcode cost structure (single product, perpetual):\n");
            Console.WriteLine("  Lite (1 dev):           $799   - generation + reading + PDF");
            Console.WriteLine("  Plus (3 devs):          $1,199 - generation + reading + PDF");
            Console.WriteLine("  Professional (10 devs): $2,399 - generation + reading + PDF");
            Console.WriteLine("  Unlimited:              $4,799 - generation + reading + PDF\n");

            Console.WriteLine("Scenario: 5-developer team needs 2D generation and reading\n");

            Console.WriteLine("OnBarcode:");
            Console.WriteLine("  Generator Suite Linear+2D (5 devs):  $2,690");
            Console.WriteLine("  Reader SDK (5 devs, from):           $1,690+");
            Console.WriteLine("  Total:                               $4,380+\n");

            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  Professional (10 devs):  $2,399");
            Console.WriteLine("  Includes everything:     Yes\n");

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
    using OnBarcode.Barcode;

    // Generator pattern (one class per symbology family)
    Linear barcode = new Linear();
    barcode.Type = BarcodeType.CODE128;
    barcode.Data = ""data"";
    barcode.drawBarcode(""output.png"");

    // ---------------------------------

    // Reader pattern (static; explicit BarcodeType per scan)
    string[] results = BarcodeScanner.Scan(""input.png"", BarcodeType.CODE128);
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
    // Returns BarcodeResults with .BarcodeType and .Value per result
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
