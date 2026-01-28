/*
 * barKoder SDK: No .NET Support Documentation
 *
 * This file documents the critical limitation that barKoder SDK
 * does not provide any .NET SDK, NuGet package, or C# API.
 *
 * Key Message: barKoder cannot be used in any .NET project.
 * .NET developers must use alternative libraries.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.IO;

namespace BarkoderNoDotnetAnalysis
{
    // ============================================================
    // BARKODER SDK: NO .NET SUPPORT
    // ============================================================

    /// <summary>
    /// barKoder SDK does not provide any .NET SDK or C# API.
    /// This file documents what .NET developers should use instead.
    /// </summary>
    public class BarkoderNoDotnetLimitation
    {
        /*
         * CRITICAL: barKoder has NO .NET support
         *
         * barKoder SDK provides:
         * - iOS Native SDK (Swift/Objective-C)
         * - Android Native SDK (Kotlin/Java)
         * - React Native plugin
         * - Cordova plugin
         * - Capacitor plugin
         * - Flutter plugin
         *
         * barKoder SDK does NOT provide:
         * - NuGet package
         * - C# API
         * - .NET MAUI bindings
         * - Xamarin support
         * - Any .NET integration
         *
         * Verification:
         * - NuGet.org search for "barkoder": 0 results
         * - barKoder.com platform list: No .NET mentioned
         * - Official documentation: No .NET guides
         */

        public void DocumentLimitation()
        {
            Console.WriteLine("=== barKoder SDK: No .NET Support ===");
            Console.WriteLine();
            Console.WriteLine("barKoder SDK CANNOT be used in any .NET project.");
            Console.WriteLine();
            Console.WriteLine("Platforms barKoder supports:");
            Console.WriteLine("  - iOS Native (Swift/Objective-C)");
            Console.WriteLine("  - Android Native (Kotlin/Java)");
            Console.WriteLine("  - React Native");
            Console.WriteLine("  - Cordova");
            Console.WriteLine("  - Capacitor");
            Console.WriteLine("  - Flutter");
            Console.WriteLine();
            Console.WriteLine("Platforms barKoder does NOT support:");
            Console.WriteLine("  - .NET / C#");
            Console.WriteLine("  - .NET MAUI");
            Console.WriteLine("  - Xamarin");
            Console.WriteLine("  - ASP.NET Core");
            Console.WriteLine("  - WPF / WinForms");
            Console.WriteLine("  - Console Applications");
            Console.WriteLine("  - Any .NET variant");
        }

        public void ExplainWhatBarkoderCodeLooksLike()
        {
            Console.WriteLine();
            Console.WriteLine("=== What barKoder Code Looks Like ===");
            Console.WriteLine();
            Console.WriteLine("barKoder is JavaScript-based (React Native example):");
            Console.WriteLine();
            Console.WriteLine("  // JavaScript - NOT C#");
            Console.WriteLine("  import { Barkoder } from 'barkoder-react-native';");
            Console.WriteLine();
            Console.WriteLine("  await Barkoder.initialize(licenseKey);");
            Console.WriteLine("  await Barkoder.setDecodingSpeed(DecodingSpeed.Fast);");
            Console.WriteLine("  Barkoder.startScanning((result) => {");
            Console.WriteLine("    console.log(result.textualData);");
            Console.WriteLine("  });");
            Console.WriteLine();
            Console.WriteLine("This JavaScript code CANNOT be used in .NET projects.");
            Console.WriteLine("There is NO equivalent C# API.");
        }
    }

    // ============================================================
    // .NET ALTERNATIVES
    // ============================================================

    /// <summary>
    /// Since barKoder is not available for .NET, here are the alternatives.
    /// </summary>
    public class DotnetAlternatives
    {
        /*
         * For .NET developers who found barKoder in their search,
         * here are actual .NET barcode libraries:
         *
         * For File/Document Processing:
         * - IronBarcode (commercial, comprehensive)
         * - ZXing.Net (open-source, basic)
         *
         * For MAUI Camera Scanning:
         * - Scandit SDK (commercial, enterprise)
         * - Scanbot SDK (commercial, MAUI)
         * - BarcodeScanning.Native.MAUI (open-source)
         * - ZXing.Net.MAUI (open-source)
         */

        public void ListAlternatives()
        {
            Console.WriteLine("=== .NET Barcode Alternatives ===");
            Console.WriteLine();
            Console.WriteLine("For FILE/DOCUMENT Processing:");
            Console.WriteLine();
            Console.WriteLine("  IronBarcode (Recommended)");
            Console.WriteLine("    dotnet add package IronBarcode");
            Console.WriteLine("    - Read barcodes from images");
            Console.WriteLine("    - Read barcodes from PDFs");
            Console.WriteLine("    - Generate barcodes");
            Console.WriteLine("    - Works in any .NET project");
            Console.WriteLine();
            Console.WriteLine("  ZXing.Net (Open-Source)");
            Console.WriteLine("    dotnet add package ZXing.Net");
            Console.WriteLine("    - Basic reading and writing");
            Console.WriteLine("    - No PDF support");
            Console.WriteLine("    - Manual format specification");
            Console.WriteLine();
            Console.WriteLine("For MAUI CAMERA Scanning:");
            Console.WriteLine();
            Console.WriteLine("  Scandit SDK");
            Console.WriteLine("    dotnet add package Scandit.BarcodePicker");
            Console.WriteLine("    - Enterprise mobile scanning");
            Console.WriteLine("    - AR overlay features");
            Console.WriteLine("    - Volume-based pricing");
            Console.WriteLine();
            Console.WriteLine("  Scanbot SDK");
            Console.WriteLine("    dotnet add package ScanbotBarcodeSDK.MAUI");
            Console.WriteLine("    - MAUI camera scanning");
            Console.WriteLine("    - Offline processing");
            Console.WriteLine("    - Yearly licensing");
            Console.WriteLine();
            Console.WriteLine("  BarcodeScanning.Native.MAUI (Open-Source)");
            Console.WriteLine("    dotnet add package BarcodeScanning.Native.Maui");
            Console.WriteLine("    - Native platform camera APIs");
            Console.WriteLine("    - Free / MIT license");
            Console.WriteLine();
            Console.WriteLine("  ZXing.Net.MAUI (Open-Source)");
            Console.WriteLine("    dotnet add package ZXing.Net.Maui.Controls");
            Console.WriteLine("    - ZXing wrapper for MAUI");
            Console.WriteLine("    - Free / MIT license");
        }
    }

    // ============================================================
    // IRONBARCODE: THE .NET SOLUTION
    // ============================================================

    /// <summary>
    /// IronBarcode provides the .NET barcode functionality that
    /// barKoder cannot provide.
    /// </summary>
    public class IronBarcodeSolution
    {
        /*
         * IronBarcode is the .NET barcode library that works
         * in ALL .NET project types.
         *
         * Install: dotnet add package IronBarcode
         */

        public void ShowIronBarcodeCapabilities()
        {
            Console.WriteLine("=== IronBarcode: .NET Barcode Solution ===");
            Console.WriteLine();
            Console.WriteLine("What IronBarcode can do:");
            Console.WriteLine();
            Console.WriteLine("  1. Read barcodes from images:");
            Console.WriteLine("     var results = BarcodeReader.Read(\"image.png\");");
            Console.WriteLine();
            Console.WriteLine("  2. Read barcodes from PDFs:");
            Console.WriteLine("     var results = BarcodeReader.Read(\"document.pdf\");");
            Console.WriteLine();
            Console.WriteLine("  3. Read barcodes from streams:");
            Console.WriteLine("     var results = BarcodeReader.Read(fileStream);");
            Console.WriteLine();
            Console.WriteLine("  4. Generate barcodes:");
            Console.WriteLine("     BarcodeWriter.CreateBarcode(\"12345\", Code128)");
            Console.WriteLine("         .SaveAsPng(\"output.png\");");
            Console.WriteLine();
            Console.WriteLine("  5. Automatic format detection:");
            Console.WriteLine("     No need to specify barcode type - ML-powered");
            Console.WriteLine();
            Console.WriteLine("  6. Error correction for damaged barcodes:");
            Console.WriteLine("     ML-powered recovery of partial/damaged codes");
        }

        public void DemonstrateIronBarcode()
        {
            Console.WriteLine();
            Console.WriteLine("=== IronBarcode Code Examples ===");
            Console.WriteLine();

            // Example 1: Read from image
            Console.WriteLine("// Read barcode from image");
            Console.WriteLine("var results = BarcodeReader.Read(\"barcode.png\");");
            Console.WriteLine("foreach (var barcode in results)");
            Console.WriteLine("{");
            Console.WriteLine("    Console.WriteLine($\"{barcode.BarcodeType}: {barcode.Text}\");");
            Console.WriteLine("}");
            Console.WriteLine();

            // Example 2: Read from PDF
            Console.WriteLine("// Read barcodes from PDF");
            Console.WriteLine("var pdfResults = BarcodeReader.Read(\"invoice.pdf\");");
            Console.WriteLine("foreach (var barcode in pdfResults)");
            Console.WriteLine("{");
            Console.WriteLine("    Console.WriteLine($\"Page {barcode.PageNumber}: {barcode.Text}\");");
            Console.WriteLine("}");
            Console.WriteLine();

            // Example 3: Generate barcode
            Console.WriteLine("// Generate barcode");
            Console.WriteLine("BarcodeWriter.CreateBarcode(\"12345\", BarcodeEncoding.Code128)");
            Console.WriteLine("    .ResizeTo(400, 100)");
            Console.WriteLine("    .SaveAsPng(\"shipping-label.png\");");
        }

        // Actual working IronBarcode examples

        public void ReadFromImage(string imagePath)
        {
            // This actually works - IronBarcode is a real .NET library
            var results = IronBarCode.BarcodeReader.Read(imagePath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Found: {barcode.BarcodeType} = {barcode.Text}");
            }
        }

        public void ReadFromPdf(string pdfPath)
        {
            // Read all barcodes from PDF document
            var results = IronBarCode.BarcodeReader.Read(pdfPath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
            }
        }

        public void GenerateBarcode(string data, string outputPath)
        {
            // Generate and save barcode
            IronBarCode.BarcodeWriter.CreateBarcode(data,
                    IronBarCode.BarcodeEncoding.Code128)
                .ResizeTo(400, 100)
                .SaveAsPng(outputPath);
        }

        public void BatchProcessDirectory(string directoryPath)
        {
            // Process all images in directory
            foreach (var file in Directory.GetFiles(directoryPath, "*.png"))
            {
                var results = IronBarCode.BarcodeReader.Read(file);
                Console.WriteLine($"{Path.GetFileName(file)}: {results.Count()} barcodes");
            }
        }
    }

    // ============================================================
    // DECISION GUIDE
    // ============================================================

    public static class DecisionGuide
    {
        public static void PrintDecisionTree()
        {
            Console.WriteLine("=== Barcode SDK Decision Guide ===");
            Console.WriteLine();
            Console.WriteLine("What is your technology stack?");
            Console.WriteLine();
            Console.WriteLine("├── React Native");
            Console.WriteLine("│   └── barKoder is an option");
            Console.WriteLine("│");
            Console.WriteLine("├── Flutter");
            Console.WriteLine("│   └── barKoder is an option");
            Console.WriteLine("│");
            Console.WriteLine("├── Cordova / Capacitor");
            Console.WriteLine("│   └── barKoder is an option");
            Console.WriteLine("│");
            Console.WriteLine("└── .NET (any variant)");
            Console.WriteLine("    └── barKoder is NOT available");
            Console.WriteLine("        │");
            Console.WriteLine("        ├── Need file/PDF processing?");
            Console.WriteLine("        │   └── Use IronBarcode");
            Console.WriteLine("        │");
            Console.WriteLine("        ├── Need MAUI camera scanning?");
            Console.WriteLine("        │   ├── Scandit SDK (commercial)");
            Console.WriteLine("        │   ├── Scanbot SDK (commercial)");
            Console.WriteLine("        │   └── BarcodeScanning.MAUI (free)");
            Console.WriteLine("        │");
            Console.WriteLine("        └── Need both?");
            Console.WriteLine("            ├── Camera: Use MAUI camera library");
            Console.WriteLine("            └── Files: Use IronBarcode");
        }

        public static void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine("=== Summary ===");
            Console.WriteLine();
            Console.WriteLine("barKoder SDK:");
            Console.WriteLine("  - Works with: React Native, Flutter, Cordova, Capacitor");
            Console.WriteLine("  - Does NOT work with: .NET, C#, MAUI, Xamarin");
            Console.WriteLine("  - No NuGet package exists");
            Console.WriteLine("  - No way to use from C# code");
            Console.WriteLine();
            Console.WriteLine("For .NET developers:");
            Console.WriteLine("  - barKoder cannot help you");
            Console.WriteLine("  - Use IronBarcode for file processing");
            Console.WriteLine("  - Use MAUI camera libraries for camera scanning");
            Console.WriteLine();
            Console.WriteLine("If you found barKoder in search results while looking");
            Console.WriteLine("for a .NET barcode library, you now know:");
            Console.WriteLine("  barKoder is not an option for .NET projects.");
        }
    }

    // ============================================================
    // MAIN ENTRY POINT
    // ============================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("barKoder SDK: No .NET Support Documentation");
            Console.WriteLine("=============================================");
            Console.WriteLine();

            var limitation = new BarkoderNoDotnetLimitation();
            limitation.DocumentLimitation();
            limitation.ExplainWhatBarkoderCodeLooksLike();

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            var alternatives = new DotnetAlternatives();
            alternatives.ListAlternatives();

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            var solution = new IronBarcodeSolution();
            solution.ShowIronBarcodeCapabilities();
            solution.DemonstrateIronBarcode();

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            DecisionGuide.PrintDecisionTree();
            DecisionGuide.PrintSummary();
        }
    }
}
