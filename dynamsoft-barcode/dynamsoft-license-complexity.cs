/**
 * License Complexity: Dynamsoft Barcode Reader vs IronBarcode
 *
 * This example demonstrates the licensing model differences between
 * Dynamsoft's runtime license validation and IronBarcode's simple key activation.
 *
 * Key Differences:
 * - Dynamsoft: Runtime license validation with callbacks and error handling
 * - Dynamsoft: License server dependency for online validation
 * - Dynamsoft: Offline licensing requires license files and device UUIDs
 * - IronBarcode: Single line license key assignment
 * - IronBarcode: No network calls, no server dependencies
 *
 * NuGet Packages Required:
 * - Dynamsoft: Dynamsoft.DotNet.BarcodeReader
 * - IronBarcode: IronBarcode version 2024.x+
 */

using System;

// ============================================================
// DYNAMSOFT APPROACH: Complex Runtime License Validation
// ============================================================

namespace DynamsoftLicensing
{
    using Dynamsoft.DBR;

    public class DynamsoftLicenseManager
    {
        private static bool _isInitialized = false;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initialize Dynamsoft license - MUST be called before any barcode operations.
        /// This method contacts Dynamsoft's license server for validation.
        /// </summary>
        public static void Initialize(string licenseKey)
        {
            lock (_lockObject)
            {
                if (_isInitialized) return;

                // Dynamsoft license initialization can fail for many reasons:
                // - Network unavailable
                // - License server unreachable
                // - Invalid or expired license key
                // - License quota exceeded
                int errorCode = BarcodeReader.InitLicense(licenseKey, out string errorMsg);

                if (errorCode != (int)EnumErrorCode.DBR_OK)
                {
                    // Must handle license failures - app cannot proceed
                    throw new DynamsoftLicenseException(
                        $"License initialization failed. Code: {errorCode}, Message: {errorMsg}");
                }

                _isInitialized = true;
                Console.WriteLine("Dynamsoft license validated successfully");
            }
        }

        /// <summary>
        /// For air-gapped or offline deployments, Dynamsoft requires
        /// license files obtained from their support team.
        /// </summary>
        public static void InitializeOffline(string licenseFileContent, string deviceUuid)
        {
            // Offline licensing requires:
            // 1. Contact Dynamsoft support to obtain offline license
            // 2. Provide device UUID from target machine
            // 3. Receive license file content specific to that device
            // 4. License files expire and need periodic renewal

            BarcodeReader reader = new BarcodeReader();

            try
            {
                reader.InitLicenseFromLicenseContent(licenseFileContent, deviceUuid);
                Console.WriteLine("Offline license activated for device: " + deviceUuid);
            }
            catch (Exception ex)
            {
                throw new DynamsoftLicenseException(
                    $"Offline license activation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Get device UUID for offline licensing - needed when requesting license files.
        /// </summary>
        public static string GetDeviceUuid()
        {
            // This must be generated and stored for each deployment target
            // Different containers/servers need different UUIDs
            return BarcodeReader.OutputLicenseToString();
        }
    }

    /// <summary>
    /// Custom exception for Dynamsoft licensing issues.
    /// </summary>
    public class DynamsoftLicenseException : Exception
    {
        public DynamsoftLicenseException(string message) : base(message) { }
    }

    /// <summary>
    /// Example service using Dynamsoft - note the initialization requirement.
    /// </summary>
    public class DynamsoftBarcodeService
    {
        private readonly BarcodeReader _reader;

        public DynamsoftBarcodeService(string licenseKey)
        {
            // Must initialize license first
            DynamsoftLicenseManager.Initialize(licenseKey);

            // Only now can we create the reader
            _reader = new BarcodeReader();
        }

        public string[] ReadBarcodes(string filePath)
        {
            TextResult[] results = _reader.DecodeFile(filePath, "");
            string[] values = new string[results.Length];

            for (int i = 0; i < results.Length; i++)
            {
                values[i] = results[i].BarcodeText;
            }

            return values;
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}

// ============================================================
// IRONBARCODE APPROACH: Simple Key-Based Licensing
// ============================================================

namespace IronBarcodeLicensing
{
    using IronBarcode;

    public class IronBarcodeLicenseManager
    {
        /// <summary>
        /// Set IronBarcode license - a single line, no network calls.
        /// </summary>
        public static void Initialize(string licenseKey)
        {
            // That's it. One line. No network validation.
            // No callbacks, no error handling required for licensing.
            IronBarcode.License.LicenseKey = licenseKey;

            // License validation happens locally and synchronously
            // Works in air-gapped environments immediately
        }
    }

    /// <summary>
    /// Example service using IronBarcode - note the simplicity.
    /// </summary>
    public class IronBarcodeBarcodeService
    {
        public IronBarcodeBarcodeService(string licenseKey)
        {
            // Set license (can be done anywhere, anytime)
            IronBarcode.License.LicenseKey = licenseKey;

            // No other initialization needed
            // No reader instance management required
        }

        public string[] ReadBarcodes(string filePath)
        {
            // Static method - no instance management
            var results = BarcodeReader.Read(filePath);

            var values = new string[results.Count()];
            int i = 0;
            foreach (var result in results)
            {
                values[i++] = result.Value;
            }

            return values;
        }

        // No Dispose needed - IronBarcode manages resources automatically
    }
}

// ============================================================
// DEPLOYMENT COMPARISON
// ============================================================

namespace DeploymentComparison
{
    /// <summary>
    /// Docker deployment comparison showing licensing complexity differences.
    /// </summary>
    public static class DockerDeploymentNotes
    {
        /*
         * DYNAMSOFT DOCKER DEPLOYMENT:
         * ----------------------------
         *
         * Dockerfile requirements:
         * - Network access to license.dynamsoft.com during startup
         * - OR pre-configured offline license file with container-specific UUID
         * - Environment variables for license key
         * - Health checks must wait for license validation
         *
         * Example Dockerfile snippet:
         *
         *   ENV DYNAMSOFT_LICENSE_KEY="your-key"
         *   # If using offline licensing:
         *   COPY license-file.lic /app/license/
         *   ENV DYNAMSOFT_LICENSE_FILE="/app/license/license-file.lic"
         *
         * Kubernetes considerations:
         * - Each pod may need unique device UUID for offline licensing
         * - License server must be reachable from cluster
         * - NetworkPolicy must allow egress to license.dynamsoft.com
         * - Pod restarts trigger license re-validation
         *
         *
         * IRONBARCODE DOCKER DEPLOYMENT:
         * ------------------------------
         *
         * Dockerfile requirements:
         * - Environment variable with license key
         * - No network requirements
         *
         * Example Dockerfile snippet:
         *
         *   ENV IRONBARCODE_LICENSE_KEY="your-key"
         *
         * Kubernetes considerations:
         * - Secret containing license key
         * - No network dependencies
         * - No pod-specific configuration needed
         * - Works identically in air-gapped clusters
         */
    }

    /// <summary>
    /// CI/CD pipeline comparison.
    /// </summary>
    public static class CICDPipelineNotes
    {
        /*
         * DYNAMSOFT IN CI/CD:
         * -------------------
         *
         * - Build agents need license server access for integration tests
         * - Development/trial licenses may have usage limits
         * - Parallel test execution may hit concurrent license limits
         * - License validation adds latency to test startup
         *
         *
         * IRONBARCODE IN CI/CD:
         * ---------------------
         *
         * - License key in environment variable or secrets
         * - No network calls during test execution
         * - No concurrent usage concerns
         * - Consistent test timing without license validation latency
         */
    }
}

// ============================================================
// USAGE EXAMPLE: Side-by-Side Comparison
// ============================================================

public class LicensingComparisonExample
{
    public static void Main()
    {
        // Dynamsoft: Must handle potential license failures
        try
        {
            DynamsoftLicensing.DynamsoftLicenseManager.Initialize("DYNAMSOFT-LICENSE-KEY");
            var dynamsoftService = new DynamsoftLicensing.DynamsoftBarcodeService("DYNAMSOFT-LICENSE-KEY");
            // Use service...
            dynamsoftService.Dispose();
        }
        catch (DynamsoftLicensing.DynamsoftLicenseException ex)
        {
            Console.WriteLine($"Dynamsoft license error: {ex.Message}");
            // Application cannot proceed without valid license
        }

        // IronBarcode: Simple assignment, guaranteed to work
        IronBarcodeLicensing.IronBarcodeLicenseManager.Initialize("IRONBARCODE-LICENSE-KEY");
        var ironService = new IronBarcodeLicensing.IronBarcodeBarcodeService("IRONBARCODE-LICENSE-KEY");
        // Use service immediately - no error handling needed for licensing
    }
}
