/**
 * BarcodeLib Dependency Issues Example
 *
 * This example demonstrates common dependency and platform compatibility
 * issues encountered with BarcodeLib, particularly around SkiaSharp and
 * System.Drawing.Common on different platforms.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Article: BarcodeLib vs IronBarcode Comparison 2026
 *
 * References:
 * - GitHub Issue #216: SkiaSharp version conflicts
 * - GitHub Issue #141: Linux/.NET 6 compatibility
 */

using System;
using System.IO;
using System.Runtime.InteropServices;

// BarcodeLib packages
// Install: dotnet add package BarcodeLib
using BarcodeStandard;
using SkiaSharp;

// IronBarcode package
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace BarcodeLibComparison
{
    /// <summary>
    /// Demonstrates dependency and platform issues with BarcodeLib
    /// compared to IronBarcode's self-contained approach.
    /// </summary>
    public class DependencyIssuesExample
    {
        /// <summary>
        /// BarcodeLib's dependency chain can cause version conflicts.
        /// This is especially common with SkiaSharp in larger projects.
        /// </summary>
        public void SkiaSharpVersionConflicts()
        {
            Console.WriteLine("=== BarcodeLib: SkiaSharp Version Conflicts ===\n");

            Console.WriteLine("BarcodeLib 3.1.4+ uses SkiaSharp for cross-platform graphics.");
            Console.WriteLine("This can conflict with other packages using SkiaSharp.\n");

            Console.WriteLine("Common Error (GitHub Issue #216):");
            Console.WriteLine("  'libSkiaSharp library version incompatible'\n");

            Console.WriteLine("Scenario:");
            Console.WriteLine("  Your project uses:");
            Console.WriteLine("    - BarcodeLib 3.1.5 (wants SkiaSharp 2.88.x)");
            Console.WriteLine("    - SkiaSharp.Views.Forms 2.80.x (older version)");
            Console.WriteLine("    - Another imaging library (wants SkiaSharp 2.90.x)");
            Console.WriteLine("  Result: Runtime error or build failure\n");

            Console.WriteLine("Workaround attempts:");
            Console.WriteLine("  1. Explicit SkiaSharp version in csproj");
            Console.WriteLine("     <PackageReference Include=\"SkiaSharp\" Version=\"2.88.6\" />");
            Console.WriteLine("  2. Binding redirects in app.config (Framework only)");
            Console.WriteLine("  3. Downgrade conflicting packages");
            Console.WriteLine("  4. Fork BarcodeLib and update dependencies\n");

            Console.WriteLine("None of these solutions are ideal for production.\n");
        }

        /// <summary>
        /// Linux deployment issues with BarcodeLib due to System.Drawing
        /// and SkiaSharp native library requirements.
        /// </summary>
        public void LinuxDeploymentIssues()
        {
            Console.WriteLine("=== BarcodeLib: Linux Deployment Issues ===\n");

            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            Console.WriteLine($"Current OS: {RuntimeInformation.OSDescription}");
            Console.WriteLine($"Is Linux: {isLinux}\n");

            Console.WriteLine("BarcodeLib History on Linux:");
            Console.WriteLine("  - Pre-3.0: Used System.Drawing.Common");
            Console.WriteLine("  - System.Drawing.Common deprecated on Linux in .NET 6");
            Console.WriteLine("  - Error: 'Unable to load shared library libgdiplus'");
            Console.WriteLine("  - 3.0+: Migrated to SkiaSharp/ImageSharp");
            Console.WriteLine("  - New Error: 'Unable to load shared library libSkiaSharp'\n");

            Console.WriteLine("Required native libraries for Linux:");
            Console.WriteLine("  # For System.Drawing fallback (older versions):");
            Console.WriteLine("  apt-get install -y libgdiplus\n");

            Console.WriteLine("  # For SkiaSharp (current versions):");
            Console.WriteLine("  apt-get install -y \\");
            Console.WriteLine("    libfontconfig1 \\");
            Console.WriteLine("    libice6 \\");
            Console.WriteLine("    libsm6 \\");
            Console.WriteLine("    libx11-6 \\");
            Console.WriteLine("    libxext6\n");

            Console.WriteLine("Docker Dockerfile additions required:");
            Console.WriteLine("  FROM mcr.microsoft.com/dotnet/aspnet:8.0");
            Console.WriteLine("  RUN apt-get update && apt-get install -y \\");
            Console.WriteLine("      libfontconfig1 \\");
            Console.WriteLine("      libice6 \\");
            Console.WriteLine("      && rm -rf /var/lib/apt/lists/*\n");

            // Attempt to generate barcode and show potential issues
            try
            {
                Console.WriteLine("Attempting barcode generation...");
                var barcode = new Barcode();
                var image = barcode.Encode(
                    BarcodeStandard.Type.Code128,
                    "TEST123",
                    200,
                    80
                );
                Console.WriteLine("Success on current platform.\n");

                using (var stream = File.OpenWrite("dependency-test.png"))
                {
                    image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("\nThis is the typical failure mode on Linux without native libs.\n");
            }
        }

        /// <summary>
        /// Platform-specific code paths required with BarcodeLib
        /// to handle different runtime environments.
        /// </summary>
        public void PlatformSpecificCodePaths()
        {
            Console.WriteLine("=== BarcodeLib: Platform-Specific Handling ===\n");

            Console.WriteLine("Because BarcodeLib's dependencies behave differently per platform,");
            Console.WriteLine("you may need platform-specific code:\n");

            string code = @"
public byte[] GenerateBarcodePortable(string data)
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        // Check for required native libraries
        if (!NativeLibraryExists(""libSkiaSharp""))
        {
            throw new PlatformNotSupportedException(
                ""Linux deployment requires libSkiaSharp. "" +
                ""Install with: apt-get install libfontconfig1 libice6 libsm6""
            );
        }
    }

    try
    {
        var barcode = new Barcode();
        var image = barcode.Encode(Type.Code128, data, 300, 100);

        using var ms = new MemoryStream();
        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
        return ms.ToArray();
    }
    catch (DllNotFoundException ex)
    {
        // Platform-specific fallback or error handling
        throw new InvalidOperationException(
            $""Native library missing on {RuntimeInformation.OSDescription}: {ex.Message}""
        );
    }
}";
            Console.WriteLine(code);
            Console.WriteLine("\nThis defensive coding adds complexity and maintenance burden.\n");
        }

        /// <summary>
        /// IronBarcode provides a self-contained solution without
        /// external native dependencies.
        /// </summary>
        public void IronBarcodeCrossPlatform()
        {
            Console.WriteLine("=== IronBarcode: Self-Contained Cross-Platform ===\n");

            Console.WriteLine("IronBarcode includes all dependencies:");
            Console.WriteLine("  - No external native library requirements");
            Console.WriteLine("  - No SkiaSharp version conflicts");
            Console.WriteLine("  - No System.Drawing.Common issues");
            Console.WriteLine("  - Works on Windows, Linux, macOS, Docker out of the box\n");

            Console.WriteLine("Platform detection (informational only - same code works everywhere):");
            Console.WriteLine($"  OS: {RuntimeInformation.OSDescription}");
            Console.WriteLine($"  Architecture: {RuntimeInformation.ProcessArchitecture}");
            Console.WriteLine($"  Framework: {RuntimeInformation.FrameworkDescription}\n");

            // Same code works on all platforms
            Console.WriteLine("Generating barcode (same code on any platform):");

            // IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";

            var barcode = BarcodeWriter.CreateBarcode(
                "CROSS-PLATFORM-TEST",
                BarcodeEncoding.Code128
            );

            // Multiple output formats, all cross-platform
            barcode.SaveAsPng("ironbarcode-crossplatform.png");
            Console.WriteLine("  Saved: ironbarcode-crossplatform.png");

            barcode.SaveAsJpeg("ironbarcode-crossplatform.jpg");
            Console.WriteLine("  Saved: ironbarcode-crossplatform.jpg");

            barcode.SaveAsPdf("ironbarcode-crossplatform.pdf");
            Console.WriteLine("  Saved: ironbarcode-crossplatform.pdf");

            // Reading also works everywhere
            var readResults = BarcodeReader.Read("ironbarcode-crossplatform.png");
            Console.WriteLine($"\n  Read back: {readResults.FirstOrDefault()?.Text}");
            Console.WriteLine("  Platform: No special handling required\n");
        }

        /// <summary>
        /// Dockerfile comparison: BarcodeLib vs IronBarcode
        /// </summary>
        public void DockerComparison()
        {
            Console.WriteLine("=== Docker Deployment Comparison ===\n");

            Console.WriteLine("BarcodeLib Dockerfile:");
            Console.WriteLine("  FROM mcr.microsoft.com/dotnet/aspnet:8.0");
            Console.WriteLine("  ");
            Console.WriteLine("  # Required for SkiaSharp");
            Console.WriteLine("  RUN apt-get update && apt-get install -y \\");
            Console.WriteLine("      libfontconfig1 \\");
            Console.WriteLine("      libice6 \\");
            Console.WriteLine("      libsm6 \\");
            Console.WriteLine("      libx11-6 \\");
            Console.WriteLine("      libxext6 \\");
            Console.WriteLine("      && rm -rf /var/lib/apt/lists/*");
            Console.WriteLine("  ");
            Console.WriteLine("  WORKDIR /app");
            Console.WriteLine("  COPY . .");
            Console.WriteLine("  ENTRYPOINT [\"dotnet\", \"MyApp.dll\"]");
            Console.WriteLine("\n  Issues:");
            Console.WriteLine("    - Larger image size (~50-100MB more)");
            Console.WriteLine("    - Longer build time");
            Console.WriteLine("    - Security scanning may flag native packages");
            Console.WriteLine("    - Updates require testing native lib compatibility\n");

            Console.WriteLine("IronBarcode Dockerfile:");
            Console.WriteLine("  FROM mcr.microsoft.com/dotnet/aspnet:8.0");
            Console.WriteLine("  ");
            Console.WriteLine("  # No additional packages needed");
            Console.WriteLine("  ");
            Console.WriteLine("  WORKDIR /app");
            Console.WriteLine("  COPY . .");
            Console.WriteLine("  ENTRYPOINT [\"dotnet\", \"MyApp.dll\"]");
            Console.WriteLine("\n  Benefits:");
            Console.WriteLine("    - Minimal image size");
            Console.WriteLine("    - Standard .NET base image only");
            Console.WriteLine("    - No native dependency management");
            Console.WriteLine("    - Predictable behavior across environments\n");
        }

        /// <summary>
        /// Demonstrates the maintenance burden of dependency management
        /// </summary>
        public void MaintenanceBurdenComparison()
        {
            Console.WriteLine("=== Long-Term Maintenance Comparison ===\n");

            Console.WriteLine("BarcodeLib Maintenance Tasks:");
            Console.WriteLine("  1. Monitor SkiaSharp version updates");
            Console.WriteLine("  2. Test compatibility after .NET upgrades");
            Console.WriteLine("  3. Update native libraries in CI/CD pipelines");
            Console.WriteLine("  4. Debug platform-specific failures");
            Console.WriteLine("  5. Manage transitive dependency conflicts");
            Console.WriteLine("  6. Track community bug fixes (no SLA)\n");

            Console.WriteLine("IronBarcode Maintenance Tasks:");
            Console.WriteLine("  1. Update NuGet package periodically");
            Console.WriteLine("  2. (That's it)\n");

            Console.WriteLine("Time estimate over 1 year:");
            Console.WriteLine("  BarcodeLib: 10-40 hours of dependency management");
            Console.WriteLine("  IronBarcode: 1-2 hours of version updates\n");
        }

        public static void Main(string[] args)
        {
            var example = new DependencyIssuesExample();

            example.SkiaSharpVersionConflicts();
            example.LinuxDeploymentIssues();
            example.PlatformSpecificCodePaths();
            example.IronBarcodeCrossPlatform();
            example.DockerComparison();
            example.MaintenanceBurdenComparison();

            Console.WriteLine("=== Summary ===");
            Console.WriteLine("BarcodeLib: External dependencies cause platform and version issues");
            Console.WriteLine("IronBarcode: Self-contained, works everywhere with no extra setup");
        }
    }
}
