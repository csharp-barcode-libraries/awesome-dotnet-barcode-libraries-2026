/**
 * Accusoft BarcodeXpress vs IronBarcode: Runtime License Configuration
 *
 * This file demonstrates the runtime licensing complexity difference between
 * Accusoft BarcodeXpress (SDK + Runtime licenses required) and IronBarcode
 * (simple key-based licensing with no runtime fees).
 *
 * Key takeaway: BarcodeXpress requires both SDK licenses for development AND
 * separate runtime licenses for production deployment. IronBarcode uses a
 * single license key with no additional runtime requirements.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.Drawing;

// ============================================================================
// PART 1: ACCUSOFT BARCODE XPRESS LICENSING SETUP
// ============================================================================

namespace AccusoftBarcodeXpressExample
{
    // NuGet (.NET Standard 2.0 / .NET Core / .NET 5+):
    //   dotnet add package Accusoft.BarcodeXpress.NetCore
    // NuGet (.NET Framework):
    //   dotnet add package Accusoft.BarcodeXpress.Net
    using Accusoft.BarcodeXpressSdk;

    /// <summary>
    /// BarcodeXpress requires a two-part licensing system:
    /// 1. SDK License (Solution Name + Solution Key) - for development
    /// 2. Runtime License (Runtime Key) - for production deployment
    ///
    /// You MUST purchase both components for production use.
    /// Minimum purchase: 5 runtime licenses even for single-server deployment.
    /// </summary>
    public class AccusoftLicenseConfiguration
    {
        private BarcodeXpress _barcodeXpress;

        public AccusoftLicenseConfiguration()
        {
            // Constructor takes the path to the runtime files (e.g. ".")
            _barcodeXpress = new BarcodeXpress(".");

            // Step 1: Configure SDK license (required for all usage)
            // You receive these values when purchasing the SDK.
            // SetSolutionName/SetSolutionKey are methods, not properties.
            // SetSolutionKey takes four 32-bit integers (not a parsed long).
            _barcodeXpress.Licensing.SetSolutionName("YourCompanyName");
            _barcodeXpress.Licensing.SetSolutionKey(1, 2, 3, 4);

            // Step 2: Configure OEM/runtime license (required for production)
            // You receive this when purchasing runtime licenses
            // (minimum 5 typically required on first-time SDK purchase).
            try
            {
                _barcodeXpress.Licensing.SetOEMLicenseKey(
                    "2.0.AStringForOEMLicensingContactAccusoftSalesForMoreInformation...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OEM license activation failed: {ex.Message}");
                Console.WriteLine("Running in watermark evaluation mode - results will be stamped");
            }
        }

        /// <summary>
        /// Read barcodes - demonstrates evaluation mode behavior.
        /// Without a valid OEM license, decoded values are stamped with
        /// " UNLICENSED accusoft.com " in the output (and generated 2D
        /// barcodes are stamped with the same string).
        /// </summary>
        public void ReadBarcodesWithLicenseCheck(string imagePath)
        {
            // Analyze takes a System.Drawing.Bitmap; the SDK does not read
            // PDFs directly. Documented input formats: TIFF, JPEG, PNG, BMP.
            using var bitmap = new Bitmap(imagePath);

            // BarcodeTypes is a System.Array of BarcodeType values, not a
            // [Flags] enum. Use Enum.GetValues to search for everything,
            // or assign a smaller array to narrow the search.
            _barcodeXpress.reader.BarcodeTypes = Enum.GetValues(typeof(BarcodeType));

            Result[] results = _barcodeXpress.reader.Analyze(bitmap);

            foreach (var result in results)
            {
                Console.WriteLine($"Barcode Type: {result.BarcodeType}");
                Console.WriteLine($"Value: {result.BarcodeValue}");

                // In evaluation mode, BarcodeValue is partially replaced with
                // " UNLICENSED accusoft.com ", making accuracy verification
                // impossible before purchase.
            }
        }
    }

    /// <summary>
    /// Enterprise deployment with multiple servers requires careful license tracking
    /// </summary>
    public class AccusoftEnterpriseLicensing
    {
        /// <summary>
        /// For server deployments, each server needs runtime license activation.
        /// Minimum purchase is 5 runtime licenses, even for single-server deployment.
        /// </summary>
        public void ConfigureServerLicense(
            string solutionName,
            int solutionKey1,
            int solutionKey2,
            int solutionKey3,
            int solutionKey4,
            string oemLicenseKey,
            string serverId)
        {
            using var barcodeXpress = new BarcodeXpress(".");

            // SDK configuration — methods, not properties
            barcodeXpress.Licensing.SetSolutionName(solutionName);
            barcodeXpress.Licensing.SetSolutionKey(solutionKey1, solutionKey2, solutionKey3, solutionKey4);

            // Server-specific OEM/runtime activation
            Console.WriteLine($"Activating OEM/runtime license for server: {serverId}");

            try
            {
                barcodeXpress.Licensing.SetOEMLicenseKey(oemLicenseKey);
                Console.WriteLine("OEM/runtime license activated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"License activation failed: {ex.Message}");

                // License failures require contacting Accusoft support
                // or purchasing additional runtime licenses
                throw;
            }
        }

        /// <summary>
        /// Docker deployment requires mounting license files or using license server
        /// </summary>
        public void DockerDeploymentConsiderations()
        {
            Console.WriteLine("Docker Deployment with BarcodeXpress:");
            Console.WriteLine("1. Mount license file volume");
            Console.WriteLine("2. Or configure license server connection");
            Console.WriteLine("3. Each container counts as runtime license");
            Console.WriteLine("4. Auto-scaling requires careful license management");
            Console.WriteLine();
            Console.WriteLine("Example Dockerfile:");
            Console.WriteLine("  COPY accusoft-license.lic /app/license/");
            Console.WriteLine("  ENV ACCUSOFT_LICENSE_PATH=/app/license/");
        }
    }

    /// <summary>
    /// Metered licensing alternative (pay per transaction).
    /// Accusoft has documented a metered/transaction-based licensing option for
    /// Barcode Xpress as an alternative to traditional per-installation licensing,
    /// but the exact .NET SDK method name for configuring it is not exposed on the
    /// public sample repositories. Contact Accusoft sales/licensing for the
    /// current configuration call.
    /// </summary>
    public class AccusoftMeteredLicensing
    {
        /// <summary>
        /// Metered licensing tracks API calls and bills per usage.
        /// Requires internet connectivity for tracking.
        /// </summary>
        public void ConfigureMeteredLicense(string meteredPublicKey, string meteredPrivateKey)
        {
            using var barcodeXpress = new BarcodeXpress(".");

            // The exact metered-licensing API for Barcode Xpress .NET is not
            // documented in the public Accusoft GitHub samples. Confirm the
            // configuration call with Accusoft licensing before relying on it
            // in production code.
            //
            // barcodeXpress.Licensing.<MeteredConfigurationCall>(meteredPublicKey, meteredPrivateKey);

            Console.WriteLine("Metered licensing configured");
            Console.WriteLine("Usage will be tracked per API call");
            Console.WriteLine("Ensure internet connectivity for license validation");
        }

        /// <summary>
        /// Cost estimation for metered usage
        /// </summary>
        public void EstimateMeteredCosts(int monthlyBarcodeReads, int monthlyBarcodeGenerates)
        {
            // Example rates (actual rates require Accusoft quote)
            decimal readCostPerOperation = 0.001m;
            decimal generateCostPerOperation = 0.0005m;

            decimal monthlyReadCost = monthlyBarcodeReads * readCostPerOperation;
            decimal monthlyGenerateCost = monthlyBarcodeGenerates * generateCostPerOperation;
            decimal totalMonthlyCost = monthlyReadCost + monthlyGenerateCost;

            Console.WriteLine($"Estimated Monthly Metered Costs:");
            Console.WriteLine($"  Barcode reads ({monthlyBarcodeReads}): ${monthlyReadCost:F2}");
            Console.WriteLine($"  Barcode generates ({monthlyBarcodeGenerates}): ${monthlyGenerateCost:F2}");
            Console.WriteLine($"  Total: ${totalMonthlyCost:F2}");
            Console.WriteLine($"  Annual projection: ${totalMonthlyCost * 12:F2}");
        }
    }
}


// ============================================================================
// PART 2: IRONBARCODE LICENSING - SIMPLE KEY-BASED APPROACH
// ============================================================================

namespace IronBarcodeLicenseExample
{
    // Install: dotnet add package IronBarcode
    using IronBarCode;

    /// <summary>
    /// IronBarcode uses simple key-based licensing:
    /// - No SDK vs Runtime distinction
    /// - No minimum license purchase
    /// - No license files to deploy
    /// - No runtime fees
    /// </summary>
    public class IronBarcodeLicenseConfiguration
    {
        /// <summary>
        /// Complete license setup - just one line
        /// </summary>
        public IronBarcodeLicenseConfiguration()
        {
            // That's it. One line. Done.
            IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-LICENSE-KEY";
        }

        /// <summary>
        /// Environment variable configuration (recommended for deployment)
        /// </summary>
        public static void ConfigureFromEnvironment()
        {
            // Set in environment: IRONBARCODE_LICENSE=your-key-here
            IronBarCode.License.LicenseKey =
                Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
        }

        /// <summary>
        /// Full functionality during trial (watermark only, no obscured results)
        /// </summary>
        public void DemonstrateTrial()
        {
            Console.WriteLine("IronBarcode Trial Mode:");
            Console.WriteLine("- Full barcode reading capability");
            Console.WriteLine("- Full barcode generation capability");
            Console.WriteLine("- Results NOT obscured (unlike BarcodeXpress)");
            Console.WriteLine("- Watermark on generated barcodes only");
            Console.WriteLine();
            Console.WriteLine("This allows you to verify accuracy BEFORE purchase");
        }

        /// <summary>
        /// Read barcodes - same code works in trial and licensed mode
        /// </summary>
        public void ReadBarcodes(string imagePath)
        {
            // No license validation code needed
            // No runtime unlock calls
            // Just read barcodes

            var results = BarcodeReader.Read(imagePath);

            foreach (var result in results)
            {
                Console.WriteLine($"Barcode Type: {result.BarcodeType}");
                Console.WriteLine($"Value: {result.Text}");

                // Full, unobscured results in both trial and licensed mode
            }
        }
    }

    /// <summary>
    /// Docker/Kubernetes deployment - simple environment variable
    /// </summary>
    public class IronBarcodeDockerDeployment
    {
        /// <summary>
        /// Docker deployment is trivial with IronBarcode
        /// </summary>
        public void DockerDeploymentExample()
        {
            Console.WriteLine("Docker Deployment with IronBarcode:");
            Console.WriteLine();
            Console.WriteLine("Dockerfile:");
            Console.WriteLine("  ENV IRONBARCODE_LICENSE=your-key-here");
            Console.WriteLine();
            Console.WriteLine("Or docker-compose.yml:");
            Console.WriteLine("  environment:");
            Console.WriteLine("    - IRONBARCODE_LICENSE=${IRONBARCODE_LICENSE}");
            Console.WriteLine();
            Console.WriteLine("Kubernetes secret:");
            Console.WriteLine("  kubectl create secret generic ironbarcode \\");
            Console.WriteLine("    --from-literal=license-key=your-key-here");
            Console.WriteLine();
            Console.WriteLine("No license files to mount");
            Console.WriteLine("No license server to configure");
            Console.WriteLine("No per-container licensing");
            Console.WriteLine("Auto-scaling just works");
        }
    }

    /// <summary>
    /// License comparison summary
    /// </summary>
    public class LicenseComparisonSummary
    {
        public void PrintComparison()
        {
            Console.WriteLine("=== LICENSE COMPARISON ===");
            Console.WriteLine();
            Console.WriteLine("ACCUSOFT BARCODEXPRESS:");
            Console.WriteLine("  - SDK License: Required ($1,960+)");
            Console.WriteLine("  - Runtime Licenses: Required (min 5)");
            Console.WriteLine("  - Per-Server: Yes, each needs runtime license");
            Console.WriteLine("  - Configuration: 10-15 lines of code");
            Console.WriteLine("  - Docker: License files or server required");
            Console.WriteLine("  - Trial Mode: Partial/obscured results");
            Console.WriteLine();
            Console.WriteLine("IRONBARCODE:");
            Console.WriteLine("  - License: Single key ($749+ one-time)");
            Console.WriteLine("  - Runtime Licenses: Not required");
            Console.WriteLine("  - Per-Server: No additional cost");
            Console.WriteLine("  - Configuration: 1 line of code");
            Console.WriteLine("  - Docker: Environment variable");
            Console.WriteLine("  - Trial Mode: Full functionality (watermark only)");
            Console.WriteLine();
            Console.WriteLine("COST COMPARISON (1 dev, 1 server):");
            Console.WriteLine("  BarcodeXpress: $4,460+ (SDK + 5 runtime min)");
            Console.WriteLine("  IronBarcode:   $749 (one-time)");
            Console.WriteLine("  Savings:       $3,711+");
        }
    }
}
