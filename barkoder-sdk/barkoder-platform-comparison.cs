/*
 * barKoder SDK vs IronBarcode: Platform Comparison
 *
 * This file provides a comprehensive platform comparison showing
 * where each SDK can and cannot be used.
 *
 * Key Message: barKoder = JavaScript mobile frameworks
 *              IronBarcode = .NET ecosystem
 *              No overlap.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace BarkoderPlatformComparison
{
    // ============================================================
    // PLATFORM MATRIX
    // ============================================================

    /// <summary>
    /// Comprehensive platform support comparison between barKoder and IronBarcode.
    /// </summary>
    public class PlatformMatrix
    {
        public void PrintFullMatrix()
        {
            Console.WriteLine("=== Complete Platform Support Matrix ===");
            Console.WriteLine();

            var platforms = new List<(string Platform, bool BarKoder, bool IronBarcode, string Notes)>
            {
                // Mobile Native
                ("iOS Native (Swift)", true, false, "barKoder primary, IronBarcode via MAUI"),
                ("Android Native (Kotlin)", true, false, "barKoder primary, IronBarcode via MAUI"),

                // Hybrid Mobile
                ("React Native", true, false, "barKoder plugin available"),
                ("Flutter", true, false, "barKoder plugin available"),
                ("Cordova", true, false, "barKoder plugin available"),
                ("Capacitor", true, false, "barKoder plugin available"),

                // .NET Mobile
                (".NET MAUI (iOS/Android)", false, true, "IronBarcode works programmatically"),
                ("Xamarin.Forms", false, true, "IronBarcode works"),
                ("Xamarin.iOS", false, true, "IronBarcode works"),
                ("Xamarin.Android", false, true, "IronBarcode works"),

                // .NET Desktop
                ("WPF", false, true, "IronBarcode full support"),
                ("WinForms", false, true, "IronBarcode full support"),
                ("Avalonia", false, true, "IronBarcode works"),
                ("UWP", false, true, "IronBarcode works"),

                // .NET Server
                ("ASP.NET Core", false, true, "IronBarcode full support"),
                ("ASP.NET MVC", false, true, "IronBarcode works"),
                ("Blazor Server", false, true, "IronBarcode full support"),
                ("Blazor WebAssembly", false, true, "IronBarcode partial"),

                // .NET Cloud/Container
                ("Azure Functions", false, true, "IronBarcode full support"),
                ("AWS Lambda", false, true, "IronBarcode works"),
                ("Docker (Linux)", false, true, "IronBarcode full support"),
                ("Docker (Windows)", false, true, "IronBarcode full support"),

                // .NET Other
                ("Console Application", false, true, "IronBarcode full support"),
                ("Class Library", false, true, "IronBarcode works"),
                ("Windows Service", false, true, "IronBarcode works"),
                ("Worker Service", false, true, "IronBarcode full support")
            };

            Console.WriteLine("| Platform                  | barKoder | IronBarcode | Notes                        |");
            Console.WriteLine("|---------------------------|----------|-------------|------------------------------|");

            foreach (var (platform, barKoder, ironBarcode, notes) in platforms)
            {
                var bk = barKoder ? "Yes" : "No";
                var ib = ironBarcode ? "Yes" : "No";
                Console.WriteLine($"| {platform,-25} | {bk,-8} | {ib,-11} | {notes,-28} |");
            }
        }

        public void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine("=== Platform Summary ===");
            Console.WriteLine();
            Console.WriteLine("barKoder supports:");
            Console.WriteLine("  - 6 platforms (all JavaScript/native mobile)");
            Console.WriteLine("  - React Native, Flutter, Cordova, Capacitor, iOS, Android");
            Console.WriteLine();
            Console.WriteLine("IronBarcode supports:");
            Console.WriteLine("  - 20+ platforms (all .NET ecosystem)");
            Console.WriteLine("  - Every .NET project type");
            Console.WriteLine();
            Console.WriteLine("Overlap: ZERO");
            Console.WriteLine("  These are completely different technology ecosystems.");
        }
    }

    // ============================================================
    // USE CASE ALIGNMENT
    // ============================================================

    /// <summary>
    /// Shows which use cases each SDK is designed for.
    /// </summary>
    public class UseCaseAlignment
    {
        public void PrintBarkoderUseCases()
        {
            Console.WriteLine("=== barKoder Use Cases ===");
            Console.WriteLine();
            Console.WriteLine("barKoder is designed for:");
            Console.WriteLine();
            Console.WriteLine("1. React Native mobile apps");
            Console.WriteLine("   - Cross-platform iOS/Android from JavaScript");
            Console.WriteLine("   - Real-time camera scanning");
            Console.WriteLine("   - Consumer-facing mobile apps");
            Console.WriteLine();
            Console.WriteLine("2. Flutter mobile apps");
            Console.WriteLine("   - Cross-platform iOS/Android from Dart");
            Console.WriteLine("   - Camera-based scanning");
            Console.WriteLine("   - Material Design mobile apps");
            Console.WriteLine();
            Console.WriteLine("3. Cordova/Capacitor hybrid apps");
            Console.WriteLine("   - Web technologies wrapped as mobile apps");
            Console.WriteLine("   - Camera integration via plugins");
            Console.WriteLine("   - PWA-style mobile development");
            Console.WriteLine();
            Console.WriteLine("4. Native iOS/Android apps");
            Console.WriteLine("   - Direct platform integration");
            Console.WriteLine("   - Maximum camera control");
            Console.WriteLine("   - Platform-specific optimization");
        }

        public void PrintIronBarcodeUseCases()
        {
            Console.WriteLine();
            Console.WriteLine("=== IronBarcode Use Cases ===");
            Console.WriteLine();
            Console.WriteLine("IronBarcode is designed for:");
            Console.WriteLine();
            Console.WriteLine("1. Server-side document processing");
            Console.WriteLine("   - ASP.NET Core APIs");
            Console.WriteLine("   - Azure Functions");
            Console.WriteLine("   - Batch processing services");
            Console.WriteLine("   - Docker containerized workloads");
            Console.WriteLine();
            Console.WriteLine("2. Desktop applications");
            Console.WriteLine("   - WPF business applications");
            Console.WriteLine("   - WinForms legacy apps");
            Console.WriteLine("   - Avalonia cross-platform desktop");
            Console.WriteLine();
            Console.WriteLine("3. MAUI mobile (programmatic)");
            Console.WriteLine("   - Process images from gallery");
            Console.WriteLine("   - Process PDF attachments");
            Console.WriteLine("   - Generate barcodes for display");
            Console.WriteLine("   - (Not camera UI - use camera library for that)");
            Console.WriteLine();
            Console.WriteLine("4. Automation and utilities");
            Console.WriteLine("   - Console batch processors");
            Console.WriteLine("   - Worker services");
            Console.WriteLine("   - Scheduled tasks");
            Console.WriteLine("   - CI/CD pipelines");
        }
    }

    // ============================================================
    // IRONBARCODE ACROSS PLATFORMS
    // ============================================================

    /// <summary>
    /// Demonstrates IronBarcode working in various .NET platforms.
    /// </summary>
    public class IronBarcodeMultiPlatform
    {
        // Console Application
        public void ConsoleExample()
        {
            Console.WriteLine("// Console Application");
            Console.WriteLine("var results = BarcodeReader.Read(\"document.pdf\");");
            Console.WriteLine("foreach (var b in results) Console.WriteLine(b.Text);");
        }

        // ASP.NET Core API
        public string[] AspNetCoreExample(Stream uploadedFile)
        {
            // Works exactly the same in ASP.NET Core
            var results = IronBarCode.BarcodeReader.Read(uploadedFile);
            return results.Select(b => b.Text).ToArray();
        }

        // WPF Desktop
        public void WpfExample(string imagePath)
        {
            // Works in WPF applications
            var results = IronBarCode.BarcodeReader.Read(imagePath);
            foreach (var barcode in results)
            {
                Console.WriteLine($"Found: {barcode.Text}");
            }
        }

        // Azure Function
        public string[] AzureFunctionExample(byte[] blobContent)
        {
            // Works in Azure Functions
            using var stream = new MemoryStream(blobContent);
            var results = IronBarCode.BarcodeReader.Read(stream);
            return results.Select(b => b.Text).ToArray();
        }

        // Docker Container
        public void DockerExample()
        {
            // Works in Docker on Linux
            var results = IronBarCode.BarcodeReader.Read("/data/invoice.pdf");
            foreach (var barcode in results)
            {
                ProcessBarcode(barcode.Text);
            }
        }

        // MAUI (programmatic, not camera UI)
        public async System.Threading.Tasks.Task MauiExample()
        {
            /*
             * // In MAUI, process image from gallery
             * var photo = await MediaPicker.PickPhotoAsync();
             * if (photo != null)
             * {
             *     using var stream = await photo.OpenReadAsync();
             *     var results = BarcodeReader.Read(stream);
             *     // Display results
             * }
             */
            await System.Threading.Tasks.Task.CompletedTask;
        }

        private void ProcessBarcode(string value)
        {
            Console.WriteLine($"Processing: {value}");
        }

        public void ShowCodeConsistency()
        {
            Console.WriteLine();
            Console.WriteLine("=== IronBarcode API Consistency ===");
            Console.WriteLine();
            Console.WriteLine("Same code works everywhere:");
            Console.WriteLine();
            Console.WriteLine("  // This exact code works in:");
            Console.WriteLine("  // - Console apps");
            Console.WriteLine("  // - ASP.NET Core");
            Console.WriteLine("  // - WPF/WinForms");
            Console.WriteLine("  // - Azure Functions");
            Console.WriteLine("  // - Docker containers");
            Console.WriteLine("  // - MAUI apps");
            Console.WriteLine();
            Console.WriteLine("  var results = BarcodeReader.Read(inputPath);");
            Console.WriteLine("  foreach (var barcode in results)");
            Console.WriteLine("  {");
            Console.WriteLine("      Console.WriteLine(barcode.Text);");
            Console.WriteLine("  }");
        }
    }

    // ============================================================
    // HYBRID ARCHITECTURES
    // ============================================================

    /// <summary>
    /// Shows how organizations with mixed technology stacks can use both.
    /// </summary>
    public class HybridArchitecture
    {
        public void DescribeHybridApproach()
        {
            Console.WriteLine("=== Hybrid Architecture Approach ===");
            Console.WriteLine();
            Console.WriteLine("Organizations with both React Native and .NET:");
            Console.WriteLine();
            Console.WriteLine("Mobile Layer (React Native + barKoder):");
            Console.WriteLine("  - Consumer mobile app");
            Console.WriteLine("  - Camera scanning for users");
            Console.WriteLine("  - barKoder processes camera frames");
            Console.WriteLine("  - Sends barcode data to backend API");
            Console.WriteLine();
            Console.WriteLine("Backend Layer (.NET + IronBarcode):");
            Console.WriteLine("  - ASP.NET Core API");
            Console.WriteLine("  - Document processing endpoints");
            Console.WriteLine("  - IronBarcode extracts from PDFs");
            Console.WriteLine("  - Generates labels and documents");
            Console.WriteLine();
            Console.WriteLine("Example Flow:");
            Console.WriteLine("  1. Mobile app scans with barKoder → sends ID to API");
            Console.WriteLine("  2. API looks up related PDF document");
            Console.WriteLine("  3. IronBarcode extracts additional barcodes from PDF");
            Console.WriteLine("  4. Combined data returned to mobile app");
        }

        public void PrintHybridDecision()
        {
            Console.WriteLine();
            Console.WriteLine("=== Hybrid Decision Tree ===");
            Console.WriteLine();
            Console.WriteLine("Do you have a React Native/Flutter mobile app?");
            Console.WriteLine("├── YES: Consider barKoder for mobile camera scanning");
            Console.WriteLine("│");
            Console.WriteLine("Do you have a .NET backend?");
            Console.WriteLine("├── YES: Use IronBarcode for server-side processing");
            Console.WriteLine("│");
            Console.WriteLine("Do you have .NET MAUI mobile?");
            Console.WriteLine("├── YES: Use IronBarcode for file processing");
            Console.WriteLine("│        Use MAUI camera library for camera scanning");
            Console.WriteLine("│");
            Console.WriteLine("Summary:");
            Console.WriteLine("  - barKoder and IronBarcode don't compete");
            Console.WriteLine("  - They serve different technology stacks");
            Console.WriteLine("  - Use each where appropriate");
        }
    }

    // ============================================================
    // MIGRATION SCENARIOS
    // ============================================================

    /// <summary>
    /// Migration paths for different scenarios.
    /// </summary>
    public class MigrationScenarios
    {
        public void ReactNativeToDotnetMaui()
        {
            Console.WriteLine("=== Migration: React Native → .NET MAUI ===");
            Console.WriteLine();
            Console.WriteLine("If you're migrating a React Native app to .NET MAUI:");
            Console.WriteLine();
            Console.WriteLine("Before (React Native + barKoder):");
            Console.WriteLine("  - JavaScript/TypeScript codebase");
            Console.WriteLine("  - barKoder for camera scanning");
            Console.WriteLine("  - React Native bridge to native");
            Console.WriteLine();
            Console.WriteLine("After (.NET MAUI):");
            Console.WriteLine("  - C# codebase");
            Console.WriteLine("  - barKoder NOT available");
            Console.WriteLine("  - Options:");
            Console.WriteLine("    1. BarcodeScanning.Native.MAUI (camera, free)");
            Console.WriteLine("    2. ZXing.Net.MAUI (camera, free)");
            Console.WriteLine("    3. Scanbot SDK (camera, commercial)");
            Console.WriteLine("    4. IronBarcode (file processing)");
        }

        public void AddingServerProcessing()
        {
            Console.WriteLine();
            Console.WriteLine("=== Adding Server Processing to Mobile App ===");
            Console.WriteLine();
            Console.WriteLine("Scenario: You have React Native + barKoder mobile app,");
            Console.WriteLine("          now need server-side document processing.");
            Console.WriteLine();
            Console.WriteLine("Mobile (keep barKoder):");
            Console.WriteLine("  - barKoder continues handling camera scanning");
            Console.WriteLine("  - Mobile app sends barcode data to API");
            Console.WriteLine();
            Console.WriteLine("Server (add IronBarcode):");
            Console.WriteLine("  - New ASP.NET Core API");
            Console.WriteLine("  - IronBarcode processes uploaded documents");
            Console.WriteLine("  - Extracts barcodes from PDFs");
            Console.WriteLine("  - Generates barcode labels");
            Console.WriteLine();
            Console.WriteLine("Code for server API:");
            Console.WriteLine("  [HttpPost(\"process-document\")]");
            Console.WriteLine("  public IActionResult ProcessDocument(IFormFile file)");
            Console.WriteLine("  {");
            Console.WriteLine("      var results = BarcodeReader.Read(file.OpenReadStream());");
            Console.WriteLine("      return Ok(results.Select(b => b.Text));");
            Console.WriteLine("  }");
        }
    }

    // ============================================================
    // DECISION FRAMEWORK
    // ============================================================

    public static class PlatformDecision
    {
        public static void PrintDecisionMatrix()
        {
            Console.WriteLine("=== Platform Decision Matrix ===");
            Console.WriteLine();
            Console.WriteLine("Technology Stack → Barcode SDK Choice");
            Console.WriteLine();

            var decisions = new[]
            {
                ("React Native", "barKoder or alternatives"),
                ("Flutter", "barKoder or alternatives"),
                ("Cordova / Capacitor", "barKoder or alternatives"),
                ("iOS Native (Swift)", "barKoder or alternatives"),
                ("Android Native (Kotlin)", "barKoder or alternatives"),
                (".NET MAUI (camera)", "Scandit, Scanbot, BarcodeScanning.MAUI"),
                (".NET MAUI (files)", "IronBarcode"),
                ("ASP.NET Core", "IronBarcode"),
                ("WPF / WinForms", "IronBarcode"),
                ("Azure Functions", "IronBarcode"),
                ("Docker / Linux", "IronBarcode"),
                ("Console / Worker", "IronBarcode")
            };

            Console.WriteLine("| Technology Stack        | Barcode SDK Choice                   |");
            Console.WriteLine("|-------------------------|--------------------------------------|");

            foreach (var (stack, choice) in decisions)
            {
                Console.WriteLine($"| {stack,-23} | {choice,-36} |");
            }
        }

        public static void PrintFinalSummary()
        {
            Console.WriteLine();
            Console.WriteLine("=== Final Summary ===");
            Console.WriteLine();
            Console.WriteLine("barKoder SDK:");
            Console.WriteLine("  - JavaScript/native mobile frameworks only");
            Console.WriteLine("  - React Native, Flutter, Cordova, Capacitor");
            Console.WriteLine("  - Camera scanning focus");
            Console.WriteLine("  - MatrixSight damaged barcode algorithm");
            Console.WriteLine("  - NO .NET support");
            Console.WriteLine();
            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  - .NET ecosystem exclusively");
            Console.WriteLine("  - All .NET project types");
            Console.WriteLine("  - File/document processing focus");
            Console.WriteLine("  - ML-powered error correction");
            Console.WriteLine("  - PDF native support");
            Console.WriteLine();
            Console.WriteLine("The choice is determined by your technology stack.");
            Console.WriteLine("There is no overlap between these SDKs.");
        }
    }

    // ============================================================
    // MAIN ENTRY POINT
    // ============================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("barKoder vs IronBarcode: Platform Comparison");
            Console.WriteLine("==============================================");
            Console.WriteLine();

            var matrix = new PlatformMatrix();
            matrix.PrintFullMatrix();
            matrix.PrintSummary();

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();

            var useCases = new UseCaseAlignment();
            useCases.PrintBarkoderUseCases();
            useCases.PrintIronBarcodeUseCases();

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();

            var multiPlatform = new IronBarcodeMultiPlatform();
            multiPlatform.ShowCodeConsistency();

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();

            var hybrid = new HybridArchitecture();
            hybrid.DescribeHybridApproach();
            hybrid.PrintHybridDecision();

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine();

            PlatformDecision.PrintDecisionMatrix();
            PlatformDecision.PrintFinalSummary();
        }
    }
}
