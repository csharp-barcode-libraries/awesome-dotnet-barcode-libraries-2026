/**
 * Google ML Kit Firebase Dependency
 *
 * This file documents the Firebase requirements for Google ML Kit
 * and demonstrates IronBarcode's dependency-free approach.
 *
 * ML Kit requires:
 * - Google account
 * - Firebase project
 * - Platform-specific configuration files
 * - Google Play Services (Android)
 * - Firebase SDK integration
 *
 * IronBarcode requires:
 * - dotnet add package IronBarcode
 * - Nothing else
 */

using System;
using System.IO;

// IronBarcode - no external service dependencies
// Install: dotnet add package IronBarcode
using IronBarcode;

namespace MLKitFirebaseDependencyExample
{
    /// <summary>
    /// Documents Firebase requirements that ML Kit imposes
    /// </summary>
    public class FirebaseDependencyDocumentation
    {
        /// <summary>
        /// Documents the setup steps required for ML Kit
        /// </summary>
        public void DocumentMLKitSetupRequirements()
        {
            Console.WriteLine("=== Google ML Kit Setup Requirements ===\n");

            Console.WriteLine("Step 1: Google Account");
            Console.WriteLine("  - Must have a Google account");
            Console.WriteLine("  - Must agree to Firebase terms of service");
            Console.WriteLine();

            Console.WriteLine("Step 2: Create Firebase Project");
            Console.WriteLine("  - Go to console.firebase.google.com");
            Console.WriteLine("  - Create new project or add to existing");
            Console.WriteLine("  - Enable billing (even for free tier)");
            Console.WriteLine();

            Console.WriteLine("Step 3: Register Your App");
            Console.WriteLine("  - Add iOS app (Bundle ID required)");
            Console.WriteLine("  - Add Android app (Package name required)");
            Console.WriteLine("  - Download configuration files");
            Console.WriteLine();

            Console.WriteLine("Step 4: Configuration Files");
            Console.WriteLine("  - Android: google-services.json → app root");
            Console.WriteLine("  - iOS: GoogleService-Info.plist → app root");
            Console.WriteLine("  - Must be included in build");
            Console.WriteLine();

            Console.WriteLine("Step 5: SDK Integration");
            Console.WriteLine("  - Add Firebase SDK NuGet packages (MAUI/Xamarin)");
            Console.WriteLine("  - Add ML Kit barcode scanning package");
            Console.WriteLine("  - Configure initialization code");
            Console.WriteLine();

            Console.WriteLine("Step 6: Android Additional Requirements");
            Console.WriteLine("  - Google Play Services required on device");
            Console.WriteLine("  - Minimum SDK version constraints");
            Console.WriteLine("  - ProGuard/R8 rules may be needed");
            Console.WriteLine();

            Console.WriteLine("Step 7: iOS Additional Requirements");
            Console.WriteLine("  - CocoaPods or Swift Package Manager setup");
            Console.WriteLine("  - Info.plist camera permission");
            Console.WriteLine("  - Bitcode considerations");
        }

        /// <summary>
        /// Shows example Firebase configuration (NOT actual working code)
        /// </summary>
        public void ShowFirebaseConfiguration()
        {
            Console.WriteLine("\n=== Firebase Configuration Files ===\n");

            // These are example configuration file contents
            var googleServicesJson = @"
// google-services.json (Android)
// Must be placed in Android project root
{
  ""project_info"": {
    ""project_number"": ""123456789012"",
    ""firebase_url"": ""https://your-project.firebaseio.com"",
    ""project_id"": ""your-project-id"",
    ""storage_bucket"": ""your-project.appspot.com""
  },
  ""client"": [
    {
      ""client_info"": {
        ""mobilesdk_app_id"": ""1:123456789012:android:abc123def456"",
        ""android_client_info"": {
          ""package_name"": ""com.yourcompany.yourapp""
        }
      },
      ""api_key"": [
        {
          ""current_key"": ""AIza...your-api-key...""
        }
      ]
    }
  ]
}";

            var googleServicePlist = @"
<!-- GoogleService-Info.plist (iOS) -->
<!-- Must be placed in iOS project root -->
<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"">
<plist version=""1.0"">
<dict>
    <key>CLIENT_ID</key>
    <string>123456789012-abc123.apps.googleusercontent.com</string>
    <key>REVERSED_CLIENT_ID</key>
    <string>com.googleusercontent.apps.123456789012-abc123</string>
    <key>API_KEY</key>
    <string>AIza...your-api-key...</string>
    <key>GCM_SENDER_ID</key>
    <string>123456789012</string>
    <key>BUNDLE_ID</key>
    <string>com.yourcompany.yourapp</string>
    <key>PROJECT_ID</key>
    <string>your-project-id</string>
</dict>
</plist>";

            Console.WriteLine("Android configuration (google-services.json):");
            Console.WriteLine(googleServicesJson);
            Console.WriteLine("\niOS configuration (GoogleService-Info.plist):");
            Console.WriteLine(googleServicePlist);

            Console.WriteLine("\nThese files contain sensitive API keys and must be managed carefully.");
        }

        /// <summary>
        /// Documents privacy and data considerations
        /// </summary>
        public void DocumentPrivacyConsiderations()
        {
            Console.WriteLine("\n=== Privacy and Data Considerations ===\n");

            Console.WriteLine("Firebase Data Collection:");
            Console.WriteLine("  - Firebase Analytics enabled by default");
            Console.WriteLine("  - Crash reporting data");
            Console.WriteLine("  - Performance monitoring data");
            Console.WriteLine("  - App usage statistics");
            Console.WriteLine();

            Console.WriteLine("Google Account Association:");
            Console.WriteLine("  - Firebase project linked to Google account");
            Console.WriteLine("  - Google's terms of service apply");
            Console.WriteLine("  - Data subject to Google's privacy policies");
            Console.WriteLine();

            Console.WriteLine("Enterprise Concerns:");
            Console.WriteLine("  - Some organizations prohibit Google service dependencies");
            Console.WriteLine("  - GDPR data processing considerations");
            Console.WriteLine("  - Data residency requirements may be affected");
            Console.WriteLine("  - Security audits may require documentation of data flows");
        }
    }

    /// <summary>
    /// Demonstrates IronBarcode's zero-dependency approach
    /// </summary>
    public class IronBarcodeNoDependencies
    {
        /// <summary>
        /// Shows IronBarcode setup - just install and use
        /// </summary>
        public void DemonstrateSimpleSetup()
        {
            Console.WriteLine("\n=== IronBarcode Setup ===\n");

            Console.WriteLine("Complete setup:");
            Console.WriteLine("  dotnet add package IronBarcode");
            Console.WriteLine();

            Console.WriteLine("That's it. No:");
            Console.WriteLine("  - Google account required");
            Console.WriteLine("  - Firebase project required");
            Console.WriteLine("  - Configuration files required");
            Console.WriteLine("  - External service registration");
            Console.WriteLine("  - API key management");
            Console.WriteLine("  - Platform-specific setup");
        }

        /// <summary>
        /// Actual working IronBarcode code
        /// </summary>
        public void DemonstrateUsage()
        {
            Console.WriteLine("\n=== IronBarcode Usage ===\n");

            Console.WriteLine("Reading barcodes (works immediately after package install):");

            // This is actual working C# code
            // No initialization, no configuration, just use the API

            Console.WriteLine(@"
using IronBarcode;

// Read from file
var results = BarcodeReader.Read(""barcode.png"");
Console.WriteLine(results.First().Value);

// Read from stream
using var stream = File.OpenRead(""barcode.png"");
var results = BarcodeReader.Read(stream);

// Read from PDF
var results = BarcodeReader.Read(""document.pdf"");

// Generate barcode
var qr = BarcodeWriter.CreateBarcode(""Hello"", BarcodeEncoding.QRCode);
qr.SaveAsPng(""output.png"");
");
        }

        /// <summary>
        /// Real working example
        /// </summary>
        public void RealWorkingExample(string imagePath)
        {
            // This actually runs - no Firebase, no Google account, no configuration

            if (File.Exists(imagePath))
            {
                var results = BarcodeReader.Read(imagePath);

                Console.WriteLine($"Found {results.Count} barcodes:");
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
                "IronBarcode - No Dependencies Required",
                BarcodeEncoding.QRCode);

            var bytes = qrCode.ToPngBinaryData();
            Console.WriteLine($"Generated QR code: {bytes.Length} bytes");
        }
    }

    /// <summary>
    /// Side-by-side comparison
    /// </summary>
    public class SetupComparison
    {
        public void CompareSetupSteps()
        {
            Console.WriteLine("\n=== Setup Steps Comparison ===\n");

            Console.WriteLine("Google ML Kit Setup:");
            Console.WriteLine("  1. Create Google account (if needed)");
            Console.WriteLine("  2. Go to Firebase Console");
            Console.WriteLine("  3. Create Firebase project");
            Console.WriteLine("  4. Add iOS app to project");
            Console.WriteLine("  5. Add Android app to project");
            Console.WriteLine("  6. Download google-services.json");
            Console.WriteLine("  7. Download GoogleService-Info.plist");
            Console.WriteLine("  8. Add files to projects");
            Console.WriteLine("  9. Install Firebase SDK packages");
            Console.WriteLine("  10. Install ML Kit packages");
            Console.WriteLine("  11. Configure Android build.gradle");
            Console.WriteLine("  12. Configure iOS Info.plist");
            Console.WriteLine("  13. Initialize Firebase in app startup");
            Console.WriteLine("  14. Write platform-specific code (separate for iOS/Android)");
            Console.WriteLine("  Estimated time: 1-3 hours (if everything works)");
            Console.WriteLine();

            Console.WriteLine("IronBarcode Setup:");
            Console.WriteLine("  1. dotnet add package IronBarcode");
            Console.WriteLine("  2. Write code");
            Console.WriteLine("  Estimated time: 30 seconds");
        }

        public void CompareOngoingMaintenance()
        {
            Console.WriteLine("\n=== Ongoing Maintenance ===\n");

            Console.WriteLine("Google ML Kit:");
            Console.WriteLine("  - Keep Firebase SDK updated");
            Console.WriteLine("  - Monitor Firebase Console for warnings");
            Console.WriteLine("  - Manage API keys and configuration");
            Console.WriteLine("  - Handle Google Play Services version requirements");
            Console.WriteLine("  - Respond to Firebase deprecation notices");
            Console.WriteLine("  - Manage separate iOS and Android implementations");
            Console.WriteLine();

            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  - Update NuGet package when new version available");
            Console.WriteLine("  - (That's it)");
        }
    }

    /// <summary>
    /// Entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var firebaseDoc = new FirebaseDependencyDocumentation();
            var ironBarcode = new IronBarcodeNoDependencies();
            var comparison = new SetupComparison();

            // Document Firebase requirements
            firebaseDoc.DocumentMLKitSetupRequirements();
            firebaseDoc.ShowFirebaseConfiguration();
            firebaseDoc.DocumentPrivacyConsiderations();

            // Show IronBarcode simplicity
            ironBarcode.DemonstrateSimpleSetup();
            ironBarcode.DemonstrateUsage();

            // Optional: Run with test image
            // ironBarcode.RealWorkingExample("test.png");

            // Compare setup processes
            comparison.CompareSetupSteps();
            comparison.CompareOngoingMaintenance();

            Console.WriteLine("\n=== Conclusion ===");
            Console.WriteLine("ML Kit: Complex multi-step setup with external service dependency");
            Console.WriteLine("IronBarcode: One-line package install, immediate functionality");
            Console.WriteLine();
            Console.WriteLine("For .NET developers, IronBarcode eliminates the Firebase");
            Console.WriteLine("complexity entirely while providing superior .NET integration.");
        }
    }
}
