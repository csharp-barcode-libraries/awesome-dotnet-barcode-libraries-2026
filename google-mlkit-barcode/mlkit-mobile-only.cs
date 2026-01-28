/**
 * Google ML Kit Mobile-Only Limitation
 *
 * IMPORTANT: Google ML Kit does NOT provide a .NET SDK.
 * This file documents the platform constraints and shows the IronBarcode
 * alternative for .NET developers.
 *
 * ML Kit platforms: iOS (Swift/Obj-C), Android (Kotlin/Java)
 * ML Kit does NOT support: .NET, C#, ASP.NET, WPF, Console apps, etc.
 */

using System;
using System.Collections.Generic;
using System.IO;

// IronBarcode - the .NET solution
// Install: dotnet add package IronBarcode
using IronBarcode;

namespace MLKitMobileOnlyExample
{
    /// <summary>
    /// Demonstrates that ML Kit is mobile-only and cannot be used directly in .NET.
    /// Shows IronBarcode as the .NET alternative.
    /// </summary>
    public class PlatformComparisonDemo
    {
        /// <summary>
        /// Documents what ML Kit requires (native mobile code, not C#)
        /// </summary>
        public void DocumentMLKitRequirements()
        {
            Console.WriteLine("=== Google ML Kit Platform Requirements ===\n");

            Console.WriteLine("ML Kit is available for:");
            Console.WriteLine("  - iOS: Swift or Objective-C native code");
            Console.WriteLine("  - Android: Kotlin or Java native code");
            Console.WriteLine();

            Console.WriteLine("ML Kit is NOT available for:");
            Console.WriteLine("  - .NET / C#");
            Console.WriteLine("  - ASP.NET Core / MVC");
            Console.WriteLine("  - WPF / WinForms");
            Console.WriteLine("  - Console Applications");
            Console.WriteLine("  - .NET MAUI (directly)");
            Console.WriteLine("  - Azure Functions");
            Console.WriteLine("  - AWS Lambda (.NET)");
            Console.WriteLine("  - Docker containers (Linux .NET)");
            Console.WriteLine();

            Console.WriteLine("To use ML Kit from MAUI, you would need:");
            Console.WriteLine("  1. Platform-specific native bindings");
            Console.WriteLine("  2. Separate iOS and Android implementations");
            Console.WriteLine("  3. Firebase project configuration");
            Console.WriteLine("  4. Platform-specific NuGet packages");
            Console.WriteLine();

            Console.WriteLine("This is fundamentally different from a native .NET library.");
        }

        /// <summary>
        /// Shows what ML Kit code looks like (NOT C# - this is for documentation)
        /// </summary>
        public void ShowMLKitNativeCodeSamples()
        {
            Console.WriteLine("=== ML Kit Native Code (NOT C#) ===\n");

            // These are documentation strings showing native code - not actual C#
            var kotlinCode = @"
// Android (Kotlin) - ML Kit barcode scanning
// This is Kotlin, NOT C# - cannot be used in .NET projects

val scanner = BarcodeScanning.getClient()
val inputImage = InputImage.fromBitmap(bitmap, 0)

scanner.process(inputImage)
    .addOnSuccessListener { barcodes ->
        for (barcode in barcodes) {
            Log.d(""Barcode"", barcode.rawValue ?: ""null"")
        }
    }
    .addOnFailureListener { e ->
        Log.e(""Barcode"", ""Scan failed"", e)
    }
";

            var swiftCode = @"
// iOS (Swift) - ML Kit barcode scanning
// This is Swift, NOT C# - cannot be used in .NET projects

import MLKitBarcodeScanning

let barcodeScanner = BarcodeScanner.barcodeScanner()
let image = VisionImage(image: uiImage)

barcodeScanner.process(image) { barcodes, error in
    guard error == nil, let barcodes = barcodes else { return }

    for barcode in barcodes {
        print(""Barcode: \(barcode.rawValue ?? """")"")
    }
}
";

            Console.WriteLine("Android (Kotlin):");
            Console.WriteLine(kotlinCode);

            Console.WriteLine("\niOS (Swift):");
            Console.WriteLine(swiftCode);

            Console.WriteLine("\nKey point: These are native mobile languages, not C#.");
            Console.WriteLine("You cannot copy this code into a .NET project.");
        }

        /// <summary>
        /// IronBarcode - the actual .NET solution
        /// This code works in any .NET project
        /// </summary>
        public void DemonstrateIronBarcode()
        {
            Console.WriteLine("=== IronBarcode - Native .NET Solution ===\n");

            Console.WriteLine("IronBarcode works in:");
            Console.WriteLine("  - Console applications");
            Console.WriteLine("  - ASP.NET Core / MVC");
            Console.WriteLine("  - WPF / WinForms");
            Console.WriteLine("  - .NET MAUI (all platforms)");
            Console.WriteLine("  - Azure Functions");
            Console.WriteLine("  - AWS Lambda");
            Console.WriteLine("  - Docker containers");
            Console.WriteLine("  - Any .NET environment");
            Console.WriteLine();

            // Actual working C# code - no native bindings required
            Console.WriteLine("Example code (works in any .NET project):\n");

            // Demonstrate barcode reading
            Console.WriteLine("// Reading barcodes from images");
            Console.WriteLine("var results = BarcodeReader.Read(\"barcode.png\");");
            Console.WriteLine("foreach (var barcode in results)");
            Console.WriteLine("{");
            Console.WriteLine("    Console.WriteLine($\"Type: {barcode.BarcodeType}\");");
            Console.WriteLine("    Console.WriteLine($\"Value: {barcode.Value}\");");
            Console.WriteLine("}");
            Console.WriteLine();

            // Demonstrate barcode generation
            Console.WriteLine("// Generating barcodes");
            Console.WriteLine("var qrCode = BarcodeWriter.CreateBarcode(\"Hello\", BarcodeEncoding.QRCode);");
            Console.WriteLine("qrCode.SaveAsPng(\"output.png\");");
        }

        /// <summary>
        /// Actual IronBarcode implementation (not documentation)
        /// </summary>
        public void ActualIronBarcodeUsage(string imagePath)
        {
            // This is real, working C# code using IronBarcode
            // No platform bindings, no native code, no Firebase

            // Read barcodes from an image
            var results = BarcodeReader.Read(imagePath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Barcode Type: {barcode.BarcodeType}");
                Console.WriteLine($"Barcode Value: {barcode.Value}");
                Console.WriteLine($"Confidence: {barcode.Confidence}");
            }

            // Generate a barcode
            var qrCode = BarcodeWriter.CreateBarcode(
                "https://ironsoftware.com",
                BarcodeEncoding.QRCode);

            qrCode.SaveAsPng("generated-qr.png");
            Console.WriteLine("Generated QR code saved.");
        }
    }

    /// <summary>
    /// Platform support matrix
    /// </summary>
    public class PlatformMatrix
    {
        public void ShowSupportMatrix()
        {
            Console.WriteLine("=== Platform Support Matrix ===\n");

            var platforms = new[]
            {
                ("Platform", "ML Kit", "IronBarcode"),
                ("---", "---", "---"),
                ("iOS Native (Swift)", "Yes", "N/A"),
                ("Android Native (Kotlin)", "Yes", "N/A"),
                (".NET MAUI iOS", "Via bindings", "Yes"),
                (".NET MAUI Android", "Via bindings", "Yes"),
                (".NET MAUI Windows", "No", "Yes"),
                (".NET MAUI macOS", "No", "Yes"),
                ("WPF", "No", "Yes"),
                ("WinForms", "No", "Yes"),
                ("ASP.NET Core", "No", "Yes"),
                ("Console App", "No", "Yes"),
                ("Azure Functions", "No", "Yes"),
                ("AWS Lambda", "No", "Yes"),
                ("Docker Linux", "No", "Yes"),
                ("Blazor Server", "No", "Yes"),
            };

            foreach (var (platform, mlKit, ironBarcode) in platforms)
            {
                Console.WriteLine($"{platform,-25} {mlKit,-15} {ironBarcode}");
            }
        }
    }

    /// <summary>
    /// Use case analysis
    /// </summary>
    public class UseCaseAnalysis
    {
        public void AnalyzeUseCases()
        {
            Console.WriteLine("=== Use Case Analysis ===\n");

            var useCases = new Dictionary<string, (string mlKitOption, string ironBarcodeOption)>
            {
                ["ASP.NET API processing uploaded documents"] =
                    ("Not possible - ML Kit is mobile only",
                     "Perfect fit - server-side processing"),

                ["WPF desktop app scanning files"] =
                    ("Not possible - ML Kit is mobile only",
                     "Perfect fit - desktop file processing"),

                ["Azure Function processing blob storage images"] =
                    ("Not possible - ML Kit is mobile only",
                     "Perfect fit - serverless processing"),

                ["Console app for batch processing"] =
                    ("Not possible - ML Kit is mobile only",
                     "Perfect fit - CLI tool processing"),

                ["MAUI app with live camera scanning"] =
                    ("Possible via native bindings (complex)",
                     "Works for processing captured images"),

                ["MAUI app processing PDF documents"] =
                    ("Not possible - ML Kit has no PDF support",
                     "Perfect fit - native PDF processing"),

                ["Native iOS app (Swift)"] =
                    ("Native support",
                     "Not applicable - IronBarcode is for .NET"),

                ["Native Android app (Kotlin)"] =
                    ("Native support",
                     "Not applicable - IronBarcode is for .NET"),
            };

            foreach (var useCase in useCases)
            {
                Console.WriteLine($"Use Case: {useCase.Key}");
                Console.WriteLine($"  ML Kit: {useCase.Value.mlKitOption}");
                Console.WriteLine($"  IronBarcode: {useCase.Value.ironBarcodeOption}");
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// Entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var platformDemo = new PlatformComparisonDemo();
            var matrix = new PlatformMatrix();
            var useCases = new UseCaseAnalysis();

            platformDemo.DocumentMLKitRequirements();
            Console.WriteLine();

            platformDemo.ShowMLKitNativeCodeSamples();
            Console.WriteLine();

            platformDemo.DemonstrateIronBarcode();
            Console.WriteLine();

            matrix.ShowSupportMatrix();
            Console.WriteLine();

            useCases.AnalyzeUseCases();

            Console.WriteLine("=== Summary ===");
            Console.WriteLine("Google ML Kit = Native mobile SDK (not for .NET)");
            Console.WriteLine("IronBarcode = Native .NET library (works everywhere .NET runs)");
            Console.WriteLine();
            Console.WriteLine("For .NET barcode processing, use IronBarcode.");
        }
    }
}
