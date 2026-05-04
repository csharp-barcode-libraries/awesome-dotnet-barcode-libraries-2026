/*
 * barKoder SDK: Mobile-Only Scope on .NET
 *
 * This file documents the actual boundary of barKoder support on .NET.
 *
 * Key facts (verified May 2026):
 *   - barKoder DOES publish .NET packages: Plugin.Maui.Barkoder and
 *     Barkoder.Xamarin (both on nuget.org).
 *   - Both packages wrap the native iOS / Android Barkoder SDKs and
 *     expose a BarkoderView camera-scanning control.
 *   - There is NO general-purpose .NET library: no ASP.NET Core,
 *     console, worker, desktop, Docker, server, or PDF surface.
 *   - barKoder is read-only; it does not generate barcodes.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.IO;
using System.Linq;

namespace BarkoderScopeAnalysis
{
    // ============================================================
    // BARKODER SDK: WHAT EXISTS ON .NET, WHAT DOES NOT
    // ============================================================

    /// <summary>
    /// barKoder SDK ships .NET MAUI and Xamarin plugins. Both are
    /// strictly mobile camera-scanning components. This class
    /// documents the boundary so .NET developers know whether their
    /// use case is in scope.
    /// </summary>
    public class BarkoderDotnetScope
    {
        /*
         * WHAT BARKODER COVERS ON .NET
         *
         *   Plugin.Maui.Barkoder    - .NET MAUI plugin
         *                             Targets net8.0+ on iOS / Android
         *                             (Windows / MacCatalyst listed on
         *                             barKoder's MAUI product page;
         *                             verify per-release).
         *
         *   Barkoder.Xamarin        - Xamarin.Forms plugin
         *                             Wraps native iOS / Android SDK
         *
         * Both expose:
         *   - BarkoderView                  (camera-scanning control)
         *   - SetFlashEnabled / SetZoomFactor / SetEnabledBarcodeTypes
         *   - StartScanning(IBarkoderDelegate)
         *   - DidFinishScanning(BarcodeResult[])
         *   - BarcodeResult.TextualData     (decoded value)
         *   - BarcodeResult.BarcodeTypeName (format)
         *
         * WHAT BARKODER DOES NOT COVER ON .NET
         *
         *   - No general-purpose Barkoder.NET package
         *   - No ASP.NET Core, Azure Functions, AWS Lambda, Docker
         *   - No console, worker service, WPF, WinForms, Avalonia
         *   - No file path / Stream / byte[] input — camera frames only
         *   - No PDF reading
         *   - No barcode generation — reader-only SDK
         */

        public void DocumentScope()
        {
            Console.WriteLine("=== barKoder SDK: .NET Scope ===");
            Console.WriteLine();
            Console.WriteLine("In scope (camera UI inside MAUI / Xamarin mobile apps):");
            Console.WriteLine("  - Plugin.Maui.Barkoder  (NuGet)");
            Console.WriteLine("  - Barkoder.Xamarin      (NuGet)");
            Console.WriteLine("  - iOS Native (Swift/Objective-C)");
            Console.WriteLine("  - Android Native (Kotlin/Java)");
            Console.WriteLine("  - React Native, Flutter, Cordova, Capacitor");
            Console.WriteLine();
            Console.WriteLine("Out of scope (no barKoder option):");
            Console.WriteLine("  - ASP.NET Core / Azure Functions / AWS Lambda");
            Console.WriteLine("  - Docker / Linux server");
            Console.WriteLine("  - Console / Worker Service");
            Console.WriteLine("  - WPF / WinForms / Avalonia / Blazor");
            Console.WriteLine("  - File / Stream / byte[] / PDF input");
            Console.WriteLine("  - Barcode generation (reader-only SDK)");
        }

        public void ExplainBarkoderMauiCode()
        {
            Console.WriteLine();
            Console.WriteLine("=== What barKoder MAUI Code Looks Like ===");
            Console.WriteLine();
            Console.WriteLine("// dotnet add package Plugin.Maui.Barkoder");
            Console.WriteLine("using Barkoder.Maui;");
            Console.WriteLine();
            Console.WriteLine("public partial class ScanPage : ContentPage, IBarkoderDelegate");
            Console.WriteLine("{");
            Console.WriteLine("    public ScanPage()");
            Console.WriteLine("    {");
            Console.WriteLine("        InitializeComponent();");
            Console.WriteLine("        BarkoderView.SetFlashEnabled(false);");
            Console.WriteLine("        BarkoderView.StartScanning(this);");
            Console.WriteLine("    }");
            Console.WriteLine();
            Console.WriteLine("    public void DidFinishScanning(BarcodeResult[] result)");
            Console.WriteLine("    {");
            Console.WriteLine("        foreach (var r in result)");
            Console.WriteLine("            Console.WriteLine($\"{r.BarcodeTypeName}: {r.TextualData}\");");
            Console.WriteLine("    }");
            Console.WriteLine("}");
            Console.WriteLine();
            Console.WriteLine("This works ONLY inside a MAUI app head against a live camera.");
            Console.WriteLine("There is no file overload, no stream overload, no PDF overload,");
            Console.WriteLine("and no equivalent surface for ASP.NET Core, console, or desktop.");
        }
    }

    // ============================================================
    // .NET ALTERNATIVES BY USE CASE
    // ============================================================

    /// <summary>
    /// For .NET use cases barKoder cannot cover, here are the
    /// libraries that can.
    /// </summary>
    public class DotnetAlternatives
    {
        /*
         * For File / Stream / PDF Processing (any .NET project):
         *   - IronBarcode   (commercial, comprehensive)
         *   - ZXing.Net     (open-source, basic)
         *
         * For MAUI Camera Scanning (alongside or instead of barKoder):
         *   - Plugin.Maui.Barkoder            (commercial)
         *   - Scandit SDK                     (commercial, enterprise)
         *   - Scanbot SDK                     (commercial)
         *   - BarcodeScanning.Native.MAUI     (open-source)
         *   - ZXing.Net.MAUI                  (open-source)
         */

        public void ListAlternatives()
        {
            Console.WriteLine("=== .NET Barcode Alternatives ===");
            Console.WriteLine();
            Console.WriteLine("For FILE / STREAM / PDF Processing:");
            Console.WriteLine();
            Console.WriteLine("  IronBarcode (Recommended)");
            Console.WriteLine("    dotnet add package BarCode");
            Console.WriteLine("    - Read from images, streams, byte arrays, PDFs");
            Console.WriteLine("    - Generate barcodes (50+ formats)");
            Console.WriteLine("    - Works in any .NET project type");
            Console.WriteLine();
            Console.WriteLine("  ZXing.Net (Open-Source)");
            Console.WriteLine("    dotnet add package ZXing.Net");
            Console.WriteLine("    - Basic reading and writing");
            Console.WriteLine("    - No PDF support");
            Console.WriteLine("    - Manual format specification");
            Console.WriteLine();
            Console.WriteLine("For MAUI CAMERA Scanning:");
            Console.WriteLine();
            Console.WriteLine("  Plugin.Maui.Barkoder  (commercial)");
            Console.WriteLine("    dotnet add package Plugin.Maui.Barkoder");
            Console.WriteLine("    - MatrixSight damage recovery");
            Console.WriteLine("    - Batch MultiScan");
            Console.WriteLine("    - Subscription, per-app, per-device pricing");
            Console.WriteLine();
            Console.WriteLine("  Scandit SDK");
            Console.WriteLine("    - Enterprise mobile scanning, AR overlays");
            Console.WriteLine();
            Console.WriteLine("  Scanbot SDK");
            Console.WriteLine("    - MAUI camera scanning, offline");
            Console.WriteLine();
            Console.WriteLine("  BarcodeScanning.Native.MAUI (Open-Source)");
            Console.WriteLine("    dotnet add package BarcodeScanning.Native.Maui");
            Console.WriteLine();
            Console.WriteLine("  ZXing.Net.MAUI (Open-Source)");
            Console.WriteLine("    dotnet add package ZXing.Net.Maui.Controls");
        }
    }

    // ============================================================
    // IRONBARCODE: COVERS WHAT BARKODER CANNOT
    // ============================================================

    /// <summary>
    /// IronBarcode covers the .NET surface that barKoder leaves open:
    /// file, stream, byte[] and PDF input across every project type,
    /// plus full barcode generation.
    /// </summary>
    public class IronBarcodeSolution
    {
        /*
         * Install: dotnet add package BarCode
         */

        public void ShowIronBarcodeCapabilities()
        {
            Console.WriteLine("=== IronBarcode: .NET Coverage barKoder Lacks ===");
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
            Console.WriteLine("     BarcodeWriter.CreateBarcode(\"12345\", BarcodeEncoding.Code128)");
            Console.WriteLine("         .SaveAsPng(\"output.png\");");
            Console.WriteLine();
            Console.WriteLine("  5. Automatic format detection:");
            Console.WriteLine("     No need to specify barcode type up-front.");
            Console.WriteLine();
            Console.WriteLine("  6. Damage recovery for low-quality images:");
            Console.WriteLine("     Speed = ReadingSpeed.ExtremeDetail");
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
            Console.WriteLine("    Console.WriteLine($\"{barcode.BarcodeType}: {barcode.Value}\");");
            Console.WriteLine("}");
            Console.WriteLine();

            // Example 2: Read from PDF
            Console.WriteLine("// Read barcodes from PDF");
            Console.WriteLine("var pdfResults = BarcodeReader.Read(\"invoice.pdf\");");
            Console.WriteLine("foreach (var barcode in pdfResults)");
            Console.WriteLine("{");
            Console.WriteLine("    Console.WriteLine($\"Page {barcode.PageNumber}: {barcode.Value}\");");
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
            // Auto-detects format; uses standard Speed.
            var results = IronBarCode.BarcodeReader.Read(imagePath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Found: {barcode.BarcodeType} = {barcode.Value}");
            }
        }

        public void ReadFromPdf(string pdfPath)
        {
            // Reads all barcodes from every page of a PDF document
            var results = IronBarCode.BarcodeReader.Read(pdfPath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Value}");
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
            // Process all images in a directory
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
            Console.WriteLine("What is your technology stack and use case?");
            Console.WriteLine();
            Console.WriteLine("├── React Native / Flutter / Cordova / Capacitor");
            Console.WriteLine("│   └── barKoder (native plugin)");
            Console.WriteLine("│");
            Console.WriteLine("├── .NET MAUI / Xamarin — camera-scanning UI");
            Console.WriteLine("│   ├── Commercial: Plugin.Maui.Barkoder, Scandit, Scanbot");
            Console.WriteLine("│   └── Open-source: BarcodeScanning.Native.MAUI, ZXing.Net.MAUI");
            Console.WriteLine("│");
            Console.WriteLine("├── .NET MAUI / Xamarin — file or PDF processing");
            Console.WriteLine("│   └── IronBarcode (barKoder cannot do this)");
            Console.WriteLine("│");
            Console.WriteLine("├── ASP.NET Core / Azure Functions / Lambda / Docker");
            Console.WriteLine("│   └── IronBarcode (barKoder has no server surface)");
            Console.WriteLine("│");
            Console.WriteLine("├── Console / Worker Service / WPF / WinForms / Avalonia");
            Console.WriteLine("│   └── IronBarcode (barKoder is mobile-only)");
            Console.WriteLine("│");
            Console.WriteLine("└── Need to GENERATE barcodes anywhere");
            Console.WriteLine("    └── IronBarcode (barKoder is reader-only)");
        }

        public static void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine("=== Summary ===");
            Console.WriteLine();
            Console.WriteLine("barKoder SDK:");
            Console.WriteLine("  - Mobile camera-scanning SDK");
            Console.WriteLine("  - .NET surface: Plugin.Maui.Barkoder + Barkoder.Xamarin");
            Console.WriteLine("  - Camera-frame input only — no file / stream / PDF");
            Console.WriteLine("  - Reader-only — no generation");
            Console.WriteLine();
            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  - Works in any .NET project type");
            Console.WriteLine("  - Reads from files, streams, byte[], PDFs");
            Console.WriteLine("  - Generates 50+ barcode formats");
            Console.WriteLine();
            Console.WriteLine("Choose by use case, not by 'does it have a NuGet package'.");
        }
    }

    // ============================================================
    // MAIN ENTRY POINT
    // ============================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("barKoder SDK: Mobile-Only Scope on .NET");
            Console.WriteLine("=========================================");
            Console.WriteLine();

            var scope = new BarkoderDotnetScope();
            scope.DocumentScope();
            scope.ExplainBarkoderMauiCode();

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
