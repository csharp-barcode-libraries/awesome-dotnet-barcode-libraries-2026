/*
 * barKoder SDK vs IronBarcode: Platform Comparison
 *
 * This file maps where each SDK can and cannot be used.
 *
 * Verified facts (May 2026):
 *   - barKoder publishes Plugin.Maui.Barkoder and Barkoder.Xamarin
 *     on NuGet. Both are camera-scanning components for mobile MAUI
 *     and Xamarin apps.
 *   - barKoder has NO general-purpose .NET library: no ASP.NET Core,
 *     Azure Functions, AWS Lambda, Docker, console, worker service,
 *     WPF, WinForms, Avalonia, or Blazor surface.
 *   - barKoder is read-only and consumes camera frames only — no
 *     file, stream, byte[], or PDF input.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BarkoderPlatformComparison
{
    // ============================================================
    // PLATFORM MATRIX
    // ============================================================

    /// <summary>
    /// Comprehensive platform support comparison between barKoder
    /// and IronBarcode.
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
                (".NET MAUI (camera UI)", true, false, "Plugin.Maui.Barkoder"),
                (".NET MAUI (file/stream)", false, true, "IronBarcode"),
                ("Xamarin.Forms", true, true, "Barkoder.Xamarin OR IronBarcode"),
                ("Xamarin.iOS", true, true, "Both supported"),
                ("Xamarin.Android", true, true, "Both supported"),

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
                ("Worker Service", false, true, "IronBarcode full support"),

                // Input modes
                ("File / Stream / byte[] input", false, true, "barKoder is camera-only"),
                ("PDF input", false, true, "barKoder has no PDF support"),
                ("Barcode generation", false, true, "barKoder is reader-only")
            };

            Console.WriteLine("| Platform                    | barKoder | IronBarcode | Notes                          |");
            Console.WriteLine("|-----------------------------|----------|-------------|--------------------------------|");

            foreach (var (platform, barKoder, ironBarcode, notes) in platforms)
            {
                var bk = barKoder ? "Yes" : "No";
                var ib = ironBarcode ? "Yes" : "No";
                Console.WriteLine($"| {platform,-27} | {bk,-8} | {ib,-11} | {notes,-30} |");
            }
        }

        public void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine("=== Platform Summary ===");
            Console.WriteLine();
            Console.WriteLine("barKoder supports:");
            Console.WriteLine("  - Mobile camera UI: iOS, Android, MAUI, Xamarin,");
            Console.WriteLine("    React Native, Flutter, Cordova, Capacitor");
            Console.WriteLine("  - Camera-frame input only");
            Console.WriteLine("  - Reading only (no generation)");
            Console.WriteLine();
            Console.WriteLine("IronBarcode supports:");
            Console.WriteLine("  - Every .NET project type (server, desktop, mobile, cloud)");
            Console.WriteLine("  - File / stream / byte[] / PDF input");
            Console.WriteLine("  - Barcode reading and generation");
            Console.WriteLine();
            Console.WriteLine("Overlap is narrow: MAUI + Xamarin ONLY,");
            Console.WriteLine("and only for the camera-UI use case.");
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
            Console.WriteLine("barKoder is designed for live camera scanning in:");
            Console.WriteLine();
            Console.WriteLine("1. .NET MAUI mobile apps (Plugin.Maui.Barkoder)");
            Console.WriteLine("   - C# / XAML codebase");
            Console.WriteLine("   - BarkoderView control hosts the camera");
            Console.WriteLine("   - IBarkoderDelegate.DidFinishScanning callback");
            Console.WriteLine();
            Console.WriteLine("2. Xamarin mobile apps (Barkoder.Xamarin)");
            Console.WriteLine("   - C# / Xamarin.Forms codebase");
            Console.WriteLine("   - Same component model as MAUI plugin");
            Console.WriteLine();
            Console.WriteLine("3. React Native mobile apps");
            Console.WriteLine("   - JavaScript / TypeScript bridge");
            Console.WriteLine("   - Real-time camera scanning");
            Console.WriteLine();
            Console.WriteLine("4. Flutter mobile apps");
            Console.WriteLine("   - Dart codebase");
            Console.WriteLine("   - Camera-based scanning");
            Console.WriteLine();
            Console.WriteLine("5. Cordova / Capacitor hybrid apps");
            Console.WriteLine("   - Web technologies wrapped as mobile apps");
            Console.WriteLine();
            Console.WriteLine("6. Native iOS / Android apps");
            Console.WriteLine("   - Direct platform integration");
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
            Console.WriteLine("3. MAUI mobile (programmatic, not camera UI)");
            Console.WriteLine("   - Process images from gallery");
            Console.WriteLine("   - Process PDF attachments");
            Console.WriteLine("   - Generate barcodes for display");
            Console.WriteLine("   - Pair with a camera library if camera UI is needed");
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
            Console.WriteLine("foreach (var b in results) Console.WriteLine(b.Value);");
        }

        // ASP.NET Core API
        public string[] AspNetCoreExample(Stream uploadedFile)
        {
            // Works exactly the same in ASP.NET Core
            var results = IronBarCode.BarcodeReader.Read(uploadedFile);
            return results.Select(b => b.Value).ToArray();
        }

        // WPF Desktop
        public void WpfExample(string imagePath)
        {
            // Works in WPF applications
            var results = IronBarCode.BarcodeReader.Read(imagePath);
            foreach (var barcode in results)
            {
                Console.WriteLine($"Found: {barcode.Value}");
            }
        }

        // Azure Function
        public string[] AzureFunctionExample(byte[] blobContent)
        {
            // Works in Azure Functions
            using var stream = new MemoryStream(blobContent);
            var results = IronBarCode.BarcodeReader.Read(stream);
            return results.Select(b => b.Value).ToArray();
        }

        // Docker Container
        public void DockerExample()
        {
            // Works in Docker on Linux
            var results = IronBarCode.BarcodeReader.Read("/data/invoice.pdf");
            foreach (var barcode in results)
            {
                ProcessBarcode(barcode.Value);
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
            Console.WriteLine("  // - WPF / WinForms / Avalonia");
            Console.WriteLine("  // - Azure Functions / AWS Lambda");
            Console.WriteLine("  // - Docker containers");
            Console.WriteLine("  // - MAUI / Xamarin (programmatic)");
            Console.WriteLine();
            Console.WriteLine("  var results = BarcodeReader.Read(inputPath);");
            Console.WriteLine("  foreach (var barcode in results)");
            Console.WriteLine("  {");
            Console.WriteLine("      Console.WriteLine(barcode.Value);");
            Console.WriteLine("  }");
        }
    }

    // ============================================================
    // HYBRID ARCHITECTURES
    // ============================================================

    /// <summary>
    /// Shows how organizations can use barKoder for camera UI and
    /// IronBarcode for everything else.
    /// </summary>
    public class HybridArchitecture
    {
        public void DescribeHybridApproach()
        {
            Console.WriteLine("=== Hybrid Architecture Approach ===");
            Console.WriteLine();
            Console.WriteLine("Common split when both libraries are in use:");
            Console.WriteLine();
            Console.WriteLine("Mobile / MAUI camera UI (Plugin.Maui.Barkoder):");
            Console.WriteLine("  - BarkoderView hosts the camera");
            Console.WriteLine("  - MatrixSight / DeBlur for damaged barcodes");
            Console.WriteLine("  - Sends decoded values (or raw images) to backend");
            Console.WriteLine();
            Console.WriteLine("Backend Layer (.NET + IronBarcode):");
            Console.WriteLine("  - ASP.NET Core API");
            Console.WriteLine("  - Document processing endpoints");
            Console.WriteLine("  - IronBarcode extracts barcodes from PDFs");
            Console.WriteLine("  - Generates labels and documents");
            Console.WriteLine();
            Console.WriteLine("Example Flow:");
            Console.WriteLine("  1. MAUI client scans with barKoder -> sends ID to API");
            Console.WriteLine("  2. API looks up related PDF document");
            Console.WriteLine("  3. IronBarcode extracts additional barcodes from PDF");
            Console.WriteLine("  4. Combined data returned to mobile app");
        }

        public void PrintHybridDecision()
        {
            Console.WriteLine();
            Console.WriteLine("=== Hybrid Decision Tree ===");
            Console.WriteLine();
            Console.WriteLine("Do you need a camera-scanning UI?");
            Console.WriteLine("├── YES (mobile): Plugin.Maui.Barkoder, Scandit, Scanbot,");
            Console.WriteLine("│                  or open-source MAUI camera library");
            Console.WriteLine("│");
            Console.WriteLine("Do you need server-side / file / PDF processing?");
            Console.WriteLine("├── YES: IronBarcode");
            Console.WriteLine("│");
            Console.WriteLine("Do you need to generate barcodes?");
            Console.WriteLine("├── YES: IronBarcode (barKoder is reader-only)");
            Console.WriteLine("│");
            Console.WriteLine("Summary:");
            Console.WriteLine("  - barKoder and IronBarcode mostly do not compete");
            Console.WriteLine("  - barKoder = camera UI; IronBarcode = everything else");
            Console.WriteLine("  - Many projects use both");
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
            Console.WriteLine("=== Migration: React Native -> .NET MAUI ===");
            Console.WriteLine();
            Console.WriteLine("If you're migrating a React Native app to .NET MAUI:");
            Console.WriteLine();
            Console.WriteLine("Before (React Native + barKoder):");
            Console.WriteLine("  - JavaScript / TypeScript codebase");
            Console.WriteLine("  - barKoder React Native plugin for camera scanning");
            Console.WriteLine();
            Console.WriteLine("After (.NET MAUI):");
            Console.WriteLine("  - C# codebase");
            Console.WriteLine("  - barKoder STILL available via Plugin.Maui.Barkoder");
            Console.WriteLine("  - Camera-UI options:");
            Console.WriteLine("    1. Plugin.Maui.Barkoder      (commercial)");
            Console.WriteLine("    2. Scandit SDK               (commercial)");
            Console.WriteLine("    3. Scanbot SDK               (commercial)");
            Console.WriteLine("    4. BarcodeScanning.Native.MAUI (open-source)");
            Console.WriteLine("    5. ZXing.Net.MAUI            (open-source)");
            Console.WriteLine("  - For file / PDF / generation: IronBarcode");
        }

        public void AddingServerProcessing()
        {
            Console.WriteLine();
            Console.WriteLine("=== Adding Server Processing to a Mobile App ===");
            Console.WriteLine();
            Console.WriteLine("Scenario: You have a barKoder mobile app (RN, MAUI, etc.)");
            Console.WriteLine("and now need server-side document processing.");
            Console.WriteLine();
            Console.WriteLine("Mobile (keep barKoder):");
            Console.WriteLine("  - barKoder continues handling camera scanning");
            Console.WriteLine("  - Mobile app sends barcode payloads or files to API");
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
            Console.WriteLine("      return Ok(results.Select(b => b.Value));");
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
            Console.WriteLine("Use case + Stack -> Barcode SDK Choice");
            Console.WriteLine();

            var decisions = new[]
            {
                ("React Native (camera)", "barKoder or alternatives"),
                ("Flutter (camera)", "barKoder or alternatives"),
                ("Cordova / Capacitor (camera)", "barKoder or alternatives"),
                ("iOS Native (camera)", "barKoder or alternatives"),
                ("Android Native (camera)", "barKoder or alternatives"),
                (".NET MAUI (camera UI)", "Plugin.Maui.Barkoder, Scandit, Scanbot"),
                (".NET MAUI (file / PDF)", "IronBarcode"),
                ("Xamarin (camera UI)", "Barkoder.Xamarin or alternatives"),
                ("Xamarin (file / PDF)", "IronBarcode"),
                ("ASP.NET Core", "IronBarcode"),
                ("WPF / WinForms / Avalonia", "IronBarcode"),
                ("Azure Functions / Lambda", "IronBarcode"),
                ("Docker / Linux", "IronBarcode"),
                ("Console / Worker", "IronBarcode"),
                ("Barcode generation", "IronBarcode (barKoder cannot generate)")
            };

            Console.WriteLine("| Use Case + Stack             | Barcode SDK Choice                       |");
            Console.WriteLine("|------------------------------|------------------------------------------|");

            foreach (var (stack, choice) in decisions)
            {
                Console.WriteLine($"| {stack,-28} | {choice,-40} |");
            }
        }

        public static void PrintFinalSummary()
        {
            Console.WriteLine();
            Console.WriteLine("=== Final Summary ===");
            Console.WriteLine();
            Console.WriteLine("barKoder SDK:");
            Console.WriteLine("  - Mobile camera-scanning SDK");
            Console.WriteLine("  - .NET surface: Plugin.Maui.Barkoder, Barkoder.Xamarin");
            Console.WriteLine("  - Plus React Native, Flutter, Cordova, Capacitor, iOS, Android");
            Console.WriteLine("  - MatrixSight damaged-barcode algorithm");
            Console.WriteLine("  - Camera-frame input only — no file / stream / PDF");
            Console.WriteLine("  - Reader-only — no generation");
            Console.WriteLine();
            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  - .NET ecosystem exclusively");
            Console.WriteLine("  - All .NET project types (server, desktop, MAUI, console)");
            Console.WriteLine("  - File / stream / byte[] / PDF input");
            Console.WriteLine("  - 50+ barcode formats, read AND generate");
            Console.WriteLine("  - ML-powered damage recovery (ReadingSpeed.ExtremeDetail)");
            Console.WriteLine();
            Console.WriteLine("Choose by use case: camera UI vs. everything else.");
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
