/*
 * Scanbot SDK vs IronBarcode: Camera-Bound Platform Limitation Analysis
 *
 * This file demonstrates Scanbot SDK's restriction to camera-bound
 * .NET runtimes (MAUI iOS/Android and UWP/WinUI 3 with autofocus
 * camera), contrasted with IronBarcode's universal .NET compatibility.
 *
 * Key Insight: Scanbot only works in camera-bound .NET runtimes
 * (MAUI mobile + UWP/WinUI 3). IronBarcode works in every .NET
 * project type, including headless server contexts.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.IO;
using System.Threading.Tasks;

namespace ScanbotMauiOnlyAnalysis
{
    // ============================================================
    // SCANBOT SDK: MAUI MOBILE ONLY
    // ============================================================

    /// <summary>
    /// Scanbot SDK requires either .NET MAUI with iOS/Android targets
    /// or a UWP/WinUI 3 desktop app with an autofocus camera.
    /// It cannot be used in headless or server-side .NET project types.
    /// </summary>
    public class ScanbotPlatformRestrictions
    {
        /*
         * Scanbot SDK Requirements (any of):
         *
         * A. .NET MAUI Application targeting iOS and/or Android
         *    - Package: ScanbotBarcodeSDK.MAUI 8.0.0 (Feb 2026)
         *    - Targets: net9.0-android, net9.0-ios (also net10.0)
         *    - Native iOS / Android bindings via ScanbotBarcodeSDK.NET
         *
         * B. UWP / WinUI 3 desktop app on Windows 10 1809+ (x64)
         *    - Package: Scanbot.BarcodeSDK.Windows 7.0.0 (Mar 2025)
         *    - Targets: net8.0-windows, net9.0-windows, UAP 10.0
         *    - Requires autofocus camera attached to the device
         *
         * All paths require: camera hardware, camera permissions,
         * and a UI shell that can host the camera view.
         *
         * Projects where Scanbot SDK CANNOT be used:
         * - Console Application
         * - Class Library
         * - ASP.NET Core Web API
         * - ASP.NET Core MVC
         * - Blazor Server
         * - Blazor WebAssembly
         * - WPF Application (no Scanbot WPF binding)
         * - WinForms Application
         * - Worker Service
         * - Azure Functions
         * - AWS Lambda
         * - Docker containers (server)
         * - Linux server applications
         * - Windows Service
         * - .NET MAUI Windows / macOS targets
         */

        public void ExplainMauiOnlyRequirement()
        {
            Console.WriteLine("=== Scanbot SDK Platform Requirement ===");
            Console.WriteLine();
            Console.WriteLine("Scanbot SDK for .NET requires a camera-bound runtime:");
            Console.WriteLine();
            Console.WriteLine("MAUI mobile project structure:");
            Console.WriteLine("  <Project Sdk=\"Microsoft.NET.Sdk\">");
            Console.WriteLine("    <PropertyGroup>");
            Console.WriteLine("      <TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>");
            Console.WriteLine("      <OutputType>Exe</OutputType>");
            Console.WriteLine("      <UseMaui>true</UseMaui>");
            Console.WriteLine("    </PropertyGroup>");
            Console.WriteLine("  </Project>");
            Console.WriteLine();
            Console.WriteLine("Or a UWP / WinUI 3 desktop app with an autofocus camera.");
            Console.WriteLine("Cannot be used in headless / server / non-camera projects.");
        }

        public void ShowPlatformMatrix()
        {
            Console.WriteLine("=== Platform Compatibility Matrix ===");
            Console.WriteLine();

            var platforms = new[]
            {
                ("Console Application", false, true),
                ("ASP.NET Core Web API", false, true),
                ("ASP.NET Core MVC", false, true),
                ("Blazor Server", false, true),
                ("Blazor WebAssembly", false, true),
                ("WPF Desktop", false, true),
                ("WinForms Desktop", false, true),
                ("Azure Functions", false, true),
                ("AWS Lambda", false, true),
                ("Docker Container", false, true),
                ("Windows Service", false, true),
                ("Linux Server", false, true),
                ("UWP / WinUI 3 (with camera)", true, true),
                (".NET MAUI (iOS/Android)", true, true),
                (".NET MAUI (Windows/macOS)", false, true)
            };

            Console.WriteLine("| Project Type             | Scanbot | IronBarcode |");
            Console.WriteLine("|--------------------------|---------|-------------|");

            foreach (var (projectType, scanbotWorks, ironbarcodeWorks) in platforms)
            {
                var scanbotStatus = scanbotWorks ? "Yes" : "No";
                var ironStatus = ironbarcodeWorks ? "Yes" : "Yes";
                Console.WriteLine($"| {projectType,-24} | {scanbotStatus,-7} | {ironStatus,-11} |");
            }
        }
    }

    // ============================================================
    // WHAT THIS MEANS FOR DEVELOPERS
    // ============================================================

    /// <summary>
    /// Practical implications of Scanbot's MAUI-only restriction.
    /// </summary>
    public class DeveloperImplications
    {
        public void ExplainServerSideImpact()
        {
            Console.WriteLine("=== Server-Side Processing Impact ===");
            Console.WriteLine();
            Console.WriteLine("Scenario: Process barcodes from uploaded documents");
            Console.WriteLine();
            Console.WriteLine("With Scanbot SDK:");
            Console.WriteLine("  - Cannot create ASP.NET Core API endpoint");
            Console.WriteLine("  - Cannot process in Azure Function");
            Console.WriteLine("  - Cannot run in Docker container");
            Console.WriteLine("  - Must use different library for server");
            Console.WriteLine();
            Console.WriteLine("With IronBarcode:");
            Console.WriteLine("  - Create ASP.NET Core endpoint");
            Console.WriteLine("  - Accept file upload");
            Console.WriteLine("  - Process barcode");
            Console.WriteLine("  - Return results");
        }

        public void ExplainDesktopImpact()
        {
            Console.WriteLine("=== Desktop Application Impact ===");
            Console.WriteLine();
            Console.WriteLine("Scenario: WPF app that reads barcodes from scanned images");
            Console.WriteLine();
            Console.WriteLine("With Scanbot SDK:");
            Console.WriteLine("  - WPF projects cannot reference Scanbot");
            Console.WriteLine("  - WinForms projects cannot reference Scanbot");
            Console.WriteLine("  - Must use different library for desktop");
            Console.WriteLine();
            Console.WriteLine("With IronBarcode:");
            Console.WriteLine("  - Add NuGet package to WPF project");
            Console.WriteLine("  - Call BarcodeReader.Read(imagePath)");
            Console.WriteLine("  - Done");
        }

        public void ExplainBatchProcessingImpact()
        {
            Console.WriteLine("=== Batch Processing Impact ===");
            Console.WriteLine();
            Console.WriteLine("Scenario: Nightly job to extract barcodes from PDF archive");
            Console.WriteLine();
            Console.WriteLine("With Scanbot SDK:");
            Console.WriteLine("  - Console apps cannot use Scanbot");
            Console.WriteLine("  - Worker services cannot use Scanbot");
            Console.WriteLine("  - Scheduled tasks cannot use Scanbot");
            Console.WriteLine("  - Must use different library for batch jobs");
            Console.WriteLine();
            Console.WriteLine("With IronBarcode:");
            Console.WriteLine("  - Create console application");
            Console.WriteLine("  - Loop through PDF files");
            Console.WriteLine("  - Extract barcodes from each");
            Console.WriteLine("  - Store results in database");
        }
    }

    // ============================================================
    // IRONBARCODE: UNIVERSAL .NET COMPATIBILITY
    // ============================================================

    /// <summary>
    /// Demonstrates IronBarcode working across all .NET project types.
    /// </summary>
    public class IronBarcodeUniversalCompatibility
    {
        /*
         * IronBarcode works in ANY .NET project:
         *
         * Install: dotnet add package BarCode
         *
         * Same API everywhere:
         * - BarcodeReader.Read() for reading
         * - BarcodeWriter.CreateBarcode() for writing
         *
         * No platform-specific code required.
         */

        // Console Application Example
        public void ConsoleAppExample()
        {
            Console.WriteLine("=== Console Application ===");

            // Works in console apps
            var results = IronBarCode.BarcodeReader.Read("document.pdf");
            foreach (var barcode in results)
            {
                Console.WriteLine($"Found: {barcode.Value}");
            }
        }

        // ASP.NET Core API Example
        public async Task<string[]> WebApiExample(Stream uploadedFile)
        {
            // Works in ASP.NET Core endpoints
            var results = IronBarCode.BarcodeReader.Read(uploadedFile);
            return results.Select(b => b.Value).ToArray();
        }

        // WPF Desktop Example
        public void WpfExample(string imagePath)
        {
            // Works in WPF applications
            var results = IronBarCode.BarcodeReader.Read(imagePath);
            // Update UI with results
        }

        // Azure Function Example
        public string[] AzureFunctionExample(byte[] blobContent)
        {
            // Works in Azure Functions
            using var stream = new MemoryStream(blobContent);
            var results = IronBarCode.BarcodeReader.Read(stream);
            return results.Select(b => b.Value).ToArray();
        }

        // Docker Container Example
        public void DockerContainerExample()
        {
            // Works in Docker containers
            // Same code as any other environment
            var results = IronBarCode.BarcodeReader.Read("/data/invoice.pdf");
            foreach (var barcode in results)
            {
                ProcessBarcode(barcode.Value);
            }
        }

        // Worker Service Example
        public async Task WorkerServiceExample()
        {
            // Works in background services
            while (true)
            {
                var files = Directory.GetFiles("/inbox/", "*.pdf");
                foreach (var file in files)
                {
                    var results = IronBarCode.BarcodeReader.Read(file);
                    await ProcessAndMove(file, results);
                }
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }

        private void ProcessBarcode(string value)
        {
            Console.WriteLine($"Processing: {value}");
        }

        private async Task ProcessAndMove(string file, IronBarCode.BarcodeResults results)
        {
            // Process and archive
            await Task.CompletedTask;
        }
    }

    // ============================================================
    // MAUI: BOTH TOOLS WORK (DIFFERENTLY)
    // ============================================================

    /// <summary>
    /// In MAUI projects, both tools work but for different purposes.
    /// </summary>
    public class MauiBothToolsComparison
    {
        /*
         * Scanbot in MAUI:
         * - Provides camera UI components
         * - Real-time video frame scanning
         * - User points camera at barcode
         * - UI feedback and confirmation
         *
         * IronBarcode in MAUI:
         * - Processes image files programmatically
         * - No camera UI (use MediaPicker)
         * - Generates barcodes for display
         * - Processes PDF attachments
         */

        public void CompareApproaches()
        {
            Console.WriteLine("=== MAUI: Both Tools Available ===");
            Console.WriteLine();
            Console.WriteLine("Scanbot in MAUI:");
            Console.WriteLine("  - Camera viewfinder UI");
            Console.WriteLine("  - Real-time scanning");
            Console.WriteLine("  - Visual feedback");
            Console.WriteLine();
            Console.WriteLine("IronBarcode in MAUI:");
            Console.WriteLine("  - Process gallery images");
            Console.WriteLine("  - Process PDF attachments");
            Console.WriteLine("  - Generate barcodes");
            Console.WriteLine("  - No camera UI");
        }

        // Scanbot approach in MAUI (conceptual)
        public async Task ScanbotCameraScanning()
        {
            /*
             * Scanbot provides camera UI:
             *
             * using ScanbotSDK.MAUI;
             * using ScanbotSDK.MAUI.Barcode;
             *
             * var config = new BarcodeScannerScreenConfiguration
             * {
             *     UseCase = new SingleScanningMode()
             * };
             * var result = await ScanbotSDKMain.Barcode.StartScannerAsync(config);
             *
             * This opens full-screen camera with viewfinder.
             * User points at barcode, SDK detects and returns.
             */
            await Task.CompletedTask;
        }

        // IronBarcode approach in MAUI
        public async Task IronBarcodeFileProcessing()
        {
            // IronBarcode processes files, not camera feed

            // Option 1: Process image from gallery
            // var photo = await MediaPicker.PickPhotoAsync();
            // var results = BarcodeReader.Read(photoPath);

            // Option 2: Process captured photo (after capture)
            // var photo = await MediaPicker.CapturePhotoAsync();
            // var results = BarcodeReader.Read(photoPath);

            // Option 3: Process PDF attachment
            // var results = BarcodeReader.Read(pdfStream);

            await Task.CompletedTask;
        }
    }

    // ============================================================
    // DECISION GUIDE
    // ============================================================

    public static class MauiDecisionGuide
    {
        public static void PrintDecisionTree()
        {
            Console.WriteLine("=== MAUI-Only Impact Decision Tree ===");
            Console.WriteLine();
            Console.WriteLine("Is your project .NET MAUI?");
            Console.WriteLine("├── NO: Scanbot cannot be used. Use IronBarcode.");
            Console.WriteLine("│   ├── Console app? → IronBarcode");
            Console.WriteLine("│   ├── ASP.NET Core? → IronBarcode");
            Console.WriteLine("│   ├── WPF/WinForms? → IronBarcode");
            Console.WriteLine("│   ├── Azure Function? → IronBarcode");
            Console.WriteLine("│   └── Docker/Server? → IronBarcode");
            Console.WriteLine("│");
            Console.WriteLine("└── YES (MAUI Mobile): Both options available");
            Console.WriteLine("    ├── Need camera viewfinder UI?");
            Console.WriteLine("    │   ├── YES: Scanbot (or BarcodeScanning.MAUI)");
            Console.WriteLine("    │   └── NO: IronBarcode");
            Console.WriteLine("    │");
            Console.WriteLine("    ├── Processing existing files/PDFs?");
            Console.WriteLine("    │   └── IronBarcode");
            Console.WriteLine("    │");
            Console.WriteLine("    └── Generating barcodes for display?");
            Console.WriteLine("        └── IronBarcode");
        }

        public static void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine("=== Summary ===");
            Console.WriteLine();
            Console.WriteLine("Scanbot SDK limitations:");
            Console.WriteLine("  - Only works in camera-bound runtimes:");
            Console.WriteLine("    MAUI iOS/Android, or UWP/WinUI 3 with autofocus camera");
            Console.WriteLine("  - Cannot be used in 12+ common .NET project types");
            Console.WriteLine("  - No headless server-side .NET SDK available");
            Console.WriteLine("  - Designed specifically for camera-based scanning");
            Console.WriteLine();
            Console.WriteLine("IronBarcode advantages:");
            Console.WriteLine("  - Works in ALL .NET project types");
            Console.WriteLine("  - Same API everywhere (console, web, desktop, mobile)");
            Console.WriteLine("  - Processes files, PDFs, and images");
            Console.WriteLine("  - No platform restrictions");
        }
    }

    // ============================================================
    // MAIN ENTRY POINT
    // ============================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Scanbot SDK: MAUI-Only Limitation Analysis");
            Console.WriteLine("============================================");
            Console.WriteLine();

            var restrictions = new ScanbotPlatformRestrictions();
            restrictions.ExplainMauiOnlyRequirement();
            Console.WriteLine();
            restrictions.ShowPlatformMatrix();

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();

            var implications = new DeveloperImplications();
            implications.ExplainServerSideImpact();
            Console.WriteLine();
            implications.ExplainDesktopImpact();
            Console.WriteLine();
            implications.ExplainBatchProcessingImpact();

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();

            MauiDecisionGuide.PrintDecisionTree();
            MauiDecisionGuide.PrintSummary();
        }
    }
}
