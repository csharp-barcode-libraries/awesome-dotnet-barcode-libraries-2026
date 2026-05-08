/**
 * Google ML Kit Setup Footprint (Historical Firebase Dependency)
 *
 * This file documents the setup footprint for Google ML Kit Barcode
 * Scanning and contrasts it with IronBarcode's single-package install.
 *
 * Important historical correction:
 *   Google ML Kit was originally a Firebase product. In June 2020,
 *   Google split ML Kit out of Firebase as a standalone product
 *   (https://developers.google.com/ml-kit). The standalone Barcode
 *   Scanning API (com.google.mlkit:barcode-scanning) does NOT
 *   require a Firebase project, google-services.json, or
 *   GoogleService-Info.plist. The legacy "Firebase ML Kit"
 *   branding still applies to the older, deprecated Firebase-bound
 *   product, but new integrations use standalone ML Kit.
 *
 * Current ML Kit Barcode Scanning still requires:
 * - Native iOS or Android project (no .NET SDK from Google)
 * - Maven or CocoaPods dependency
 * - Google Play Services on Android (for the unbundled model only)
 * - Community Xamarin/MAUI binding if approaching from .NET
 * - Camera permission and platform-specific setup for live scanning
 *
 * IronBarcode requires:
 * - dotnet add package BarCode
 * - Nothing else
 */

using System;
using System.IO;
using System.Linq;

// IronBarcode - no external service dependencies, no community binding required
// Install: dotnet add package BarCode
using IronBarCode;

namespace MLKitFirebaseDependencyExample
{
    /// <summary>
    /// Documents the setup footprint of standalone ML Kit Barcode Scanning
    /// and the historical Firebase dependency that no longer applies.
    /// </summary>
    public class SetupFootprintDocumentation
    {
        /// <summary>
        /// Documents the historical Firebase dependency and current state.
        /// </summary>
        public void DocumentHistoricalFirebaseStatus()
        {
            Console.WriteLine("=== ML Kit and Firebase: Historical vs Current ===\n");

            Console.WriteLine("Pre-June 2020 (Firebase ML Kit):");
            Console.WriteLine("  - Firebase project required");
            Console.WriteLine("  - google-services.json / GoogleService-Info.plist required");
            Console.WriteLine("  - Firebase SDK initialization required");
            Console.WriteLine();

            Console.WriteLine("Post-June 2020 (standalone ML Kit at mlkit.dev):");
            Console.WriteLine("  - NO Firebase project required");
            Console.WriteLine("  - NO google-services.json required");
            Console.WriteLine("  - NO Firebase SDK required");
            Console.WriteLine("  - Distributed via Maven (Android) and CocoaPods (iOS)");
            Console.WriteLine();

            Console.WriteLine("Older articles, blog posts, and Stack Overflow answers");
            Console.WriteLine("often still describe the Firebase-bound integration. If you");
            Console.WriteLine("see google-services.json instructions for ML Kit Barcode");
            Console.WriteLine("Scanning today, those instructions are stale.");
        }

        /// <summary>
        /// Documents the current standalone ML Kit Barcode Scanning setup steps.
        /// </summary>
        public void DocumentCurrentMLKitSetupRequirements()
        {
            Console.WriteLine("\n=== Current Standalone ML Kit Setup Requirements ===\n");

            Console.WriteLine("Step 1: Native Mobile Project");
            Console.WriteLine("  - Android (Kotlin/Java) project, or");
            Console.WriteLine("  - iOS (Swift/Objective-C) project, or");
            Console.WriteLine("  - .NET MAUI / Xamarin project with a community binding");
            Console.WriteLine();

            Console.WriteLine("Step 2: Android Gradle Dependency (choose one)");
            Console.WriteLine("  - Bundled model (no Play Services required, +~2.4 MB):");
            Console.WriteLine("      implementation 'com.google.mlkit:barcode-scanning:17.3.0'");
            Console.WriteLine("  - Unbundled model (smaller, requires Play Services):");
            Console.WriteLine("      implementation 'com.google.android.gms:play-services-mlkit-barcode-scanning:18.3.1'");
            Console.WriteLine();

            Console.WriteLine("Step 3: iOS CocoaPod");
            Console.WriteLine("  - pod 'GoogleMLKit/BarcodeScanning'");
            Console.WriteLine("  - Run 'pod install' to fetch the framework");
            Console.WriteLine();

            Console.WriteLine("Step 4: .NET MAUI / Xamarin (community bindings only)");
            Console.WriteLine("  - No first-party Google .NET package exists");
            Console.WriteLine("  - Options include: Xamarin.GooglePlayServices.MLKit.BarcodeScanning,");
            Console.WriteLine("    BarcodeScanning.Native.Maui, or hand-written bindings");
            Console.WriteLine("  - Maintenance lag behind upstream ML Kit releases is common");
            Console.WriteLine();

            Console.WriteLine("Step 5: Permissions");
            Console.WriteLine("  - Android: CAMERA permission for live scanning");
            Console.WriteLine("  - iOS: NSCameraUsageDescription in Info.plist");
            Console.WriteLine();

            Console.WriteLine("Step 6: Platform-Specific Code");
            Console.WriteLine("  - Separate iOS and Android code paths");
            Console.WriteLine("  - InputImage construction differs per platform");
            Console.WriteLine("  - Result handling tied to native callback APIs");
        }

        /// <summary>
        /// Documents privacy and data considerations.
        /// </summary>
        public void DocumentPrivacyConsiderations()
        {
            Console.WriteLine("\n=== Privacy and Data Considerations ===\n");

            Console.WriteLine("Standalone ML Kit Barcode Scanning (on-device):");
            Console.WriteLine("  - Image data processed locally on the device");
            Console.WriteLine("  - No barcode payloads sent to Google by ML Kit itself");
            Console.WriteLine("  - No Firebase Analytics tied to barcode scanning");
            Console.WriteLine();

            Console.WriteLine("Indirect Considerations:");
            Console.WriteLine("  - If your app uses Firebase Analytics or Crashlytics for");
            Console.WriteLine("    other reasons, those have their own data behaviour");
            Console.WriteLine("  - The unbundled model is fetched via Play Services");
            Console.WriteLine("  - Google Play Services itself has its own data flows");
            Console.WriteLine();

            Console.WriteLine("Enterprise Concerns:");
            Console.WriteLine("  - Some organizations limit Google Play Services dependence");
            Console.WriteLine("  - GDPR and data-residency reviews still apply to any SDK");
            Console.WriteLine("  - Community .NET binding maintenance is a supply-chain factor");
        }
    }

    /// <summary>
    /// Demonstrates IronBarcode's zero-dependency approach.
    /// </summary>
    public class IronBarcodeNoDependencies
    {
        /// <summary>
        /// Shows IronBarcode setup - just install and use.
        /// </summary>
        public void DemonstrateSimpleSetup()
        {
            Console.WriteLine("\n=== IronBarcode Setup ===\n");

            Console.WriteLine("Complete setup:");
            Console.WriteLine("  dotnet add package BarCode");
            Console.WriteLine();

            Console.WriteLine("That's it. No:");
            Console.WriteLine("  - Maven or CocoaPods step");
            Console.WriteLine("  - Google Play Services constraint");
            Console.WriteLine("  - Community Xamarin/MAUI binding to chase");
            Console.WriteLine("  - Separate iOS and Android implementations");
            Console.WriteLine("  - Native callback wiring");
        }

        /// <summary>
        /// Actual working IronBarcode code.
        /// </summary>
        public void DemonstrateUsage()
        {
            Console.WriteLine("\n=== IronBarcode Usage ===\n");

            Console.WriteLine("Reading barcodes (works immediately after package install):");

            Console.WriteLine(@"
using IronBarCode;

// Read from file
var results = BarcodeReader.Read(""barcode.png"");
Console.WriteLine(results.First().Value);

// Read from stream
using var stream = File.OpenRead(""barcode.png"");
var streamResults = BarcodeReader.Read(stream);

// Read from PDF
var pdfResults = BarcodeReader.Read(""document.pdf"");

// Generate barcode
var qr = BarcodeWriter.CreateBarcode(""Hello"", BarcodeEncoding.QRCode);
qr.SaveAsPng(""output.png"");
");
        }

        /// <summary>
        /// Real working example.
        /// </summary>
        public void RealWorkingExample(string imagePath)
        {
            // This actually runs - no Maven, no CocoaPods, no community binding

            IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";

            if (File.Exists(imagePath))
            {
                var results = BarcodeReader.Read(imagePath);

                Console.WriteLine($"Found {results.Count()} barcodes:");
                foreach (var barcode in results)
                {
                    Console.WriteLine($"  {barcode.BarcodeType}: {barcode.Value}");
                }
            }
            else
            {
                Console.WriteLine("Demo: BarcodeReader.Read() works immediately with no setup");
            }

            // Generate a barcode - works instantly
            var qrCode = BarcodeWriter.CreateBarcode(
                "IronBarcode - No Native Bindings Required",
                BarcodeEncoding.QRCode);

            var bytes = qrCode.ToPngBinaryData();
            Console.WriteLine($"Generated QR code: {bytes.Length} bytes");
        }
    }

    /// <summary>
    /// Side-by-side comparison.
    /// </summary>
    public class SetupComparison
    {
        public void CompareSetupSteps()
        {
            Console.WriteLine("\n=== Setup Steps Comparison ===\n");

            Console.WriteLine("Google ML Kit Setup (per platform):");
            Console.WriteLine("  Android:");
            Console.WriteLine("    1. Add Maven dependency to build.gradle");
            Console.WriteLine("    2. Decide bundled vs unbundled model");
            Console.WriteLine("    3. Add CAMERA permission to manifest");
            Console.WriteLine("    4. Wire BarcodeScanning.getClient(options)");
            Console.WriteLine("    5. Build InputImage from camera/file");
            Console.WriteLine("    6. Handle success/failure listeners");
            Console.WriteLine("  iOS:");
            Console.WriteLine("    1. Add CocoaPod and run pod install");
            Console.WriteLine("    2. Add NSCameraUsageDescription to Info.plist");
            Console.WriteLine("    3. Build VisionImage from UIImage / sample buffer");
            Console.WriteLine("    4. Handle the completion closure");
            Console.WriteLine("  .NET MAUI:");
            Console.WriteLine("    1. Choose a community binding (e.g., BarcodeScanning.Native.Maui)");
            Console.WriteLine("    2. Add platform-specific initialization");
            Console.WriteLine("    3. Track upstream ML Kit version drift");
            Console.WriteLine();

            Console.WriteLine("IronBarcode Setup:");
            Console.WriteLine("  1. dotnet add package BarCode");
            Console.WriteLine("  2. Set license key (one line)");
            Console.WriteLine("  3. Write code");
        }

        public void CompareOngoingMaintenance()
        {
            Console.WriteLine("\n=== Ongoing Maintenance ===\n");

            Console.WriteLine("Google ML Kit:");
            Console.WriteLine("  - Track ML Kit release notes for SDK updates");
            Console.WriteLine("  - Manage Google Play Services version constraints");
            Console.WriteLine("  - Re-validate community .NET bindings against new ML Kit drops");
            Console.WriteLine("  - Maintain separate iOS and Android implementations");
            Console.WriteLine();

            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  - Update NuGet package when new version available");
            Console.WriteLine("  - (That's it)");
        }
    }

    /// <summary>
    /// Entry point.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var setupDoc = new SetupFootprintDocumentation();
            var ironBarcode = new IronBarcodeNoDependencies();
            var comparison = new SetupComparison();

            // Document historical Firebase status and current ML Kit setup
            setupDoc.DocumentHistoricalFirebaseStatus();
            setupDoc.DocumentCurrentMLKitSetupRequirements();
            setupDoc.DocumentPrivacyConsiderations();

            // Show IronBarcode simplicity
            ironBarcode.DemonstrateSimpleSetup();
            ironBarcode.DemonstrateUsage();

            // Optional: Run with test image
            // ironBarcode.RealWorkingExample("test.png");

            // Compare setup processes
            comparison.CompareSetupSteps();
            comparison.CompareOngoingMaintenance();

            Console.WriteLine("\n=== Conclusion ===");
            Console.WriteLine("ML Kit (current): Native mobile SDK with Maven/CocoaPods setup,");
            Console.WriteLine("optional Play Services dependency, and community-binding overhead");
            Console.WriteLine("for any .NET access.");
            Console.WriteLine();
            Console.WriteLine("IronBarcode: One-line package install, immediate functionality,");
            Console.WriteLine("first-party .NET API across all .NET platforms.");
        }
    }
}
