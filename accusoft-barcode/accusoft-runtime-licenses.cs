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

// ============================================================================
// PART 1: ACCUSOFT BARCODEXPRESS LICENSING SETUP
// ============================================================================

namespace AccusoftBarcodeXpressExample
{
    // Install: dotnet add package Accusoft.BarcodeXpress.NetCore
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
            _barcodeXpress = new BarcodeXpress();

            // Step 1: Configure SDK license (required for all usage)
            // You receive these values when purchasing the SDK
            _barcodeXpress.Licensing.SolutionName = "YourCompanyName";
            _barcodeXpress.Licensing.SolutionKey = Convert.ToInt64("12345678901234");

            // Step 2: Configure runtime license (required for production)
            // You receive this when purchasing runtime licenses (minimum 5)
            try
            {
                _barcodeXpress.Licensing.UnlockRuntime(
                    "YourRuntimeLicenseKey",
                    Convert.ToInt64("98765432109876"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Runtime license activation failed: {ex.Message}");
                Console.WriteLine("Running in evaluation mode - results will be obscured");
            }
        }

        /// <summary>
        /// Check if the license is properly configured
        /// </summary>
        public bool ValidateLicense()
        {
            // Check SDK license
            bool sdkLicensed = !string.IsNullOrEmpty(_barcodeXpress.Licensing.SolutionName);

            // Check runtime license
            bool runtimeLicensed = _barcodeXpress.Licensing.IsRuntimeUnlocked;

            if (!sdkLicensed)
            {
                Console.WriteLine("WARNING: SDK license not configured");
                Console.WriteLine("Full barcode recognition is disabled");
            }

            if (!runtimeLicensed)
            {
                Console.WriteLine("WARNING: Runtime license not activated");
                Console.WriteLine("Barcode results will be partially obscured");
                Console.WriteLine("Example: '1234567890' may return as '1234...XXX'");
            }

            return sdkLicensed && runtimeLicensed;
        }

        /// <summary>
        /// Read barcodes - demonstrates evaluation mode behavior
        /// </summary>
        public void ReadBarcodesWithLicenseCheck(string imagePath)
        {
            if (!ValidateLicense())
            {
                Console.WriteLine("\n--- EVALUATION MODE DEMONSTRATION ---");
                Console.WriteLine("Without runtime license, BarcodeXpress obscures results:");
            }

            _barcodeXpress.reader.SetPropertyValue(
                BarcodeXpress.cycBxeSetFilename, imagePath);

            var results = _barcodeXpress.reader.Analyze();

            foreach (var result in results)
            {
                Console.WriteLine($"Barcode Type: {result.BarcodeType}");
                Console.WriteLine($"Value: {result.BarcodeValue}");

                // In evaluation mode, this value will be partially obscured
                // making it impossible to verify accuracy before purchase
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
            long solutionKey,
            string runtimeKey,
            long runtimeSolutionKey,
            string serverId)
        {
            using var barcodeXpress = new BarcodeXpress();

            // SDK configuration
            barcodeXpress.Licensing.SolutionName = solutionName;
            barcodeXpress.Licensing.SolutionKey = solutionKey;

            // Server-specific runtime activation
            Console.WriteLine($"Activating runtime license for server: {serverId}");

            try
            {
                barcodeXpress.Licensing.UnlockRuntime(runtimeKey, runtimeSolutionKey);
                Console.WriteLine("Runtime license activated successfully");
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
    /// Metered licensing alternative (pay per transaction)
    /// </summary>
    public class AccusoftMeteredLicensing
    {
        /// <summary>
        /// Metered licensing tracks API calls and bills per usage.
        /// Requires internet connectivity for tracking.
        /// </summary>
        public void ConfigureMeteredLicense(string meteredPublicKey, string meteredPrivateKey)
        {
            using var barcodeXpress = new BarcodeXpress();

            // Metered licensing configuration
            // Note: Requires internet connectivity for usage tracking
            barcodeXpress.Licensing.SetMeteredKey(meteredPublicKey, meteredPrivateKey);

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
