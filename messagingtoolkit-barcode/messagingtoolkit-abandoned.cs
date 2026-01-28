/**
 * MessagingToolkit.Barcode Abandonment Documentation
 *
 * IMPORTANT: MessagingToolkit.Barcode was abandoned in 2014.
 * This file documents the library's limitations and why migration is necessary.
 *
 * Last update: 2014
 * Target platforms: .NET Framework 3.5-4.5, Silverlight 3-5, Windows Phone 7.x/8.0
 * Modern .NET support: NONE
 */

using System;
using System.Drawing;
using System.IO;

// Note: These namespaces only work in .NET Framework
// They will NOT compile in .NET Core, .NET 5, 6, 7, 8, or 9
#if NET45 || NET40 || NET35
using MessagingToolkit.Barcode;
#endif

namespace MessagingToolkitAbandonedExample
{
    /// <summary>
    /// Documents why MessagingToolkit.Barcode cannot be used in modern .NET projects.
    /// This is NOT production code - it's documentation of a legacy library.
    /// </summary>
    public class AbandonmentDocumentation
    {
        /// <summary>
        /// Shows the obsolete platform targets of MessagingToolkit.Barcode
        /// </summary>
        public void DocumentObsoletePlatforms()
        {
            Console.WriteLine("=== MessagingToolkit.Barcode Obsolete Platforms ===\n");

            var platforms = new[]
            {
                ("Silverlight 3", "Discontinued", "No browser support since 2021"),
                ("Silverlight 4", "Discontinued", "No browser support since 2021"),
                ("Silverlight 5", "Discontinued", "No browser support since 2021"),
                ("Windows Phone 7.0", "Discontinued", "End of support 2014"),
                ("Windows Phone 7.5", "Discontinued", "End of support 2014"),
                ("Windows Phone 7.8", "Discontinued", "End of support 2014"),
                ("Windows Phone 8.0", "Discontinued", "End of support 2017"),
                (".NET Framework 3.5", "Maintenance only", "Security updates only"),
                (".NET Framework 4.0", "Maintenance only", "Security updates only"),
                (".NET Framework 4.5", "Maintenance only", "Security updates only"),
            };

            Console.WriteLine($"{"Platform",-25} {"Status",-15} {"Notes"}");
            Console.WriteLine(new string('-', 70));

            foreach (var (platform, status, notes) in platforms)
            {
                Console.WriteLine($"{platform,-25} {status,-15} {notes}");
            }

            Console.WriteLine();
            Console.WriteLine("None of these platforms are suitable for new development in 2026.");
        }

        /// <summary>
        /// Documents the unsupported modern platforms
        /// </summary>
        public void DocumentUnsupportedPlatforms()
        {
            Console.WriteLine("\n=== Modern Platforms NOT Supported ===\n");

            var modernPlatforms = new[]
            {
                ".NET Core 1.0, 1.1",
                ".NET Core 2.0, 2.1, 2.2",
                ".NET Core 3.0, 3.1",
                ".NET 5.0",
                ".NET 6.0 (LTS)",
                ".NET 7.0",
                ".NET 8.0 (LTS)",
                ".NET 9.0",
                ".NET MAUI",
                "Blazor WebAssembly",
                "Blazor Server",
                "ASP.NET Core",
                "Azure Functions",
                "AWS Lambda",
            };

            foreach (var platform in modernPlatforms)
            {
                Console.WriteLine($"  X  {platform}");
            }

            Console.WriteLine();
            Console.WriteLine("MessagingToolkit.Barcode cannot run on any modern .NET platform.");
        }

        /// <summary>
        /// Documents the security implications
        /// </summary>
        public void DocumentSecurityConcerns()
        {
            Console.WriteLine("\n=== Security Concerns ===\n");

            Console.WriteLine("Abandoned since: 2014 (10+ years)");
            Console.WriteLine();

            Console.WriteLine("Security implications:");
            Console.WriteLine("  - No security patches since 2014");
            Console.WriteLine("  - Dependencies have known CVEs");
            Console.WriteLine("  - No response to vulnerability reports");
            Console.WriteLine("  - Fails security audit tools (Snyk, WhiteSource, etc.)");
            Console.WriteLine();

            Console.WriteLine("Compliance considerations:");
            Console.WriteLine("  - PCI DSS requires actively maintained software");
            Console.WriteLine("  - HIPAA security rule requires current patches");
            Console.WriteLine("  - SOC 2 audits flag abandoned packages");
            Console.WriteLine("  - ISO 27001 controls require patch management");
        }

        /// <summary>
        /// Documents the development blockers
        /// </summary>
        public void DocumentDevelopmentBlockers()
        {
            Console.WriteLine("\n=== Development Blockers ===\n");

            Console.WriteLine("1. Cannot upgrade to .NET 6/7/8/9");
            Console.WriteLine("   - MessagingToolkit only targets .NET Framework");
            Console.WriteLine("   - Blocking your entire application modernization");
            Console.WriteLine();

            Console.WriteLine("2. Cannot use modern C# features");
            Console.WriteLine("   - C# 8+ features require .NET Core 3+ or .NET 5+");
            Console.WriteLine("   - Stuck with C# 7.3 or earlier patterns");
            Console.WriteLine();

            Console.WriteLine("3. Cannot use modern NuGet packages");
            Console.WriteLine("   - Many libraries now require .NET Standard 2.0+");
            Console.WriteLine("   - Dependency conflicts with modern packages");
            Console.WriteLine();

            Console.WriteLine("4. Cannot deploy to modern platforms");
            Console.WriteLine("   - No Linux containers (Docker)");
            Console.WriteLine("   - No Azure App Service (.NET 8)");
            Console.WriteLine("   - No serverless functions");
        }
    }

#if NET45 || NET40 || NET35
    /// <summary>
    /// Example of MessagingToolkit.Barcode code (only compiles on .NET Framework)
    /// This demonstrates the legacy API before migration.
    /// </summary>
    public class LegacyMessagingToolkitCode
    {
        /// <summary>
        /// Legacy barcode reading - MessagingToolkit.Barcode
        /// This code ONLY works on .NET Framework 4.5 or earlier
        /// </summary>
        public string ReadBarcodeOldWay(string imagePath)
        {
            // Note: BarcodeDecoder is the MessagingToolkit reader class
            var decoder = new BarcodeDecoder();

            // Must use System.Drawing.Bitmap - not available in .NET Core without packages
            using (var bitmap = new Bitmap(imagePath))
            {
                // Decode returns null if no barcode found
                var result = decoder.Decode(bitmap);

                if (result != null)
                {
                    return result.Text;
                }
            }

            return null;
        }

        /// <summary>
        /// Legacy barcode generation - MessagingToolkit.Barcode
        /// This code ONLY works on .NET Framework 4.5 or earlier
        /// </summary>
        public void CreateBarcodeOldWay(string data, string outputPath)
        {
            // BarcodeEncoder is the MessagingToolkit writer class
            var encoder = new BarcodeEncoder();

            // Must set format before encoding
            encoder.Format = BarcodeFormat.QrCode;

            // Returns a Bitmap
            var bitmap = encoder.Encode(data);

            // Save using System.Drawing
            bitmap.Save(outputPath);
        }
    }
#else
    /// <summary>
    /// This class replaces the legacy code for modern .NET
    /// The MessagingToolkit code above cannot compile here
    /// </summary>
    public class ModernReplacementNotice
    {
        public void ShowMessage()
        {
            Console.WriteLine("MessagingToolkit.Barcode code cannot compile on modern .NET.");
            Console.WriteLine("This is running on: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            Console.WriteLine();
            Console.WriteLine("Migration to IronBarcode is required.");
            Console.WriteLine("See messagingtoolkit-migration.cs for the migration guide.");
        }
    }
#endif

    /// <summary>
    /// Timeline of MessagingToolkit.Barcode abandonment
    /// </summary>
    public class AbandonmentTimeline
    {
        public void ShowTimeline()
        {
            Console.WriteLine("\n=== Abandonment Timeline ===\n");

            var events = new[]
            {
                ("2011", "Initial release", "Based on ZXing port"),
                ("2012", "Active development", "Multiple releases"),
                ("2013", "Slowing updates", "Fewer commits"),
                ("2014", "Last update", "Version 1.7.0.2 released"),
                ("2015-2025", "Abandoned", "No updates, no responses"),
                ("2026", "Critical risk", "10+ years without maintenance"),
            };

            foreach (var (year, status, notes) in events)
            {
                Console.WriteLine($"{year}: {status}");
                Console.WriteLine($"       {notes}");
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// NuGet package analysis
    /// </summary>
    public class PackageAnalysis
    {
        public void AnalyzePackage()
        {
            Console.WriteLine("\n=== NuGet Package Analysis ===\n");

            Console.WriteLine("Package: MessagingToolkit.Barcode");
            Console.WriteLine("Version: 1.7.0.2");
            Console.WriteLine("Published: 2014");
            Console.WriteLine("Downloads: Legacy installs only");
            Console.WriteLine();

            Console.WriteLine("Dependencies:");
            Console.WriteLine("  - .NET Framework 3.5+ (no modern .NET)");
            Console.WriteLine("  - System.Drawing (Windows-only)");
            Console.WriteLine();

            Console.WriteLine("Package health indicators:");
            Console.WriteLine("  X  No recent updates (10+ years)");
            Console.WriteLine("  X  No open issues addressed");
            Console.WriteLine("  X  No pull requests merged");
            Console.WriteLine("  X  Repository appears abandoned");
            Console.WriteLine("  X  No .NET Standard target");
            Console.WriteLine("  X  No modern .NET target");
        }
    }

    /// <summary>
    /// Entry point demonstrating abandonment documentation
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var docs = new AbandonmentDocumentation();
            var timeline = new AbandonmentTimeline();
            var package = new PackageAnalysis();

            Console.WriteLine("========================================");
            Console.WriteLine("MessagingToolkit.Barcode Status: ABANDONED");
            Console.WriteLine("========================================\n");

            docs.DocumentObsoletePlatforms();
            docs.DocumentUnsupportedPlatforms();
            docs.DocumentSecurityConcerns();
            docs.DocumentDevelopmentBlockers();

            timeline.ShowTimeline();
            package.AnalyzePackage();

            Console.WriteLine("\n========================================");
            Console.WriteLine("RECOMMENDATION: Migrate to IronBarcode");
            Console.WriteLine("========================================");
            Console.WriteLine("See messagingtoolkit-migration.cs for migration guide");
        }
    }
}
