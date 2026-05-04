/**
 * License Complexity: Dynamsoft Barcode Reader vs IronBarcode
 *
 * This example demonstrates the licensing model differences between
 * Dynamsoft's online activation flow and IronBarcode's local key activation.
 *
 * Key Differences:
 * - Dynamsoft: Online activation via LicenseManager.InitLicense, periodic re-check
 * - Dynamsoft: First activation requires outbound HTTPS to Dynamsoft's licence servers
 * - Dynamsoft: Air-gapped environments need a device-bound offline licence file
 * - IronBarcode: Single line license key assignment
 * - IronBarcode: No network calls, no server dependencies
 *
 * NuGet Packages Required:
 * - Dynamsoft: Dynamsoft.DotNet.BarcodeReader.Bundle (v11.x)
 * - IronBarcode: BarCode (NuGet package id is `BarCode`; namespace is `IronBarCode`)
 */

using System;

// ============================================================
// DYNAMSOFT APPROACH: Online License Activation
// ============================================================

namespace DynamsoftLicensing
{
    using Dynamsoft.CVR;
    using Dynamsoft.DBR;
    using Dynamsoft.License;
    using Dynamsoft.Core;

    public class DynamsoftLicenseManager
    {
        private static bool _isInitialized = false;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initialize the Dynamsoft licence — must be called before any
        /// CaptureVisionRouter operations. The first call performs an online
        /// activation against Dynamsoft's licence servers; subsequent runs
        /// re-validate periodically against the cached credentials.
        /// </summary>
        public static void Initialize(string licenseKey)
        {
            lock (_lockObject)
            {
                if (_isInitialized) return;

                // LicenseManager.InitLicense can fail for many reasons:
                // - Network unavailable on first activation
                // - Periodic re-check window expired with no connectivity
                // - Invalid or expired licence key
                // - Activation quota exceeded
                int errorCode = LicenseManager.InitLicense(licenseKey, out string errorMsg);

                if (errorCode != (int)EnumErrorCode.EC_OK)
                {
                    // Must handle licence failures - app cannot proceed
                    throw new DynamsoftLicenseException(
                        $"License initialization failed. Code: {errorCode}, Message: {errorMsg}");
                }

                _isInitialized = true;
                Console.WriteLine("Dynamsoft license activated successfully");
            }
        }

        /// <summary>
        /// For air-gapped or offline deployments, Dynamsoft requires a
        /// device-bound licence file obtained from their support team.
        /// </summary>
        public static void InitializeOffline(string licenseKey, string licenseFileContent)
        {
            // Offline licensing requires:
            // 1. Contact Dynamsoft support to obtain an offline licence
            // 2. Receive a licence file bound to the target device fingerprint
            // 3. Pass it to the LicenseManager APIs documented for your SDK
            //    version (the exact method names have moved across versions)
            // 4. Files have an expiry date and need periodic renewal

            try
            {
                // Method shape varies by SDK version; consult the current
                // Dynamsoft.License.LicenseManager API reference for your build.
                int errorCode = LicenseManager.InitLicenseFromDevice(
                    licenseKey, licenseFileContent, out string errorMsg);

                if (errorCode != (int)EnumErrorCode.EC_OK)
                    throw new DynamsoftLicenseException(
                        $"Offline license activation failed: {errorMsg}");

                Console.WriteLine("Offline license activated");
            }
            catch (Exception ex)
            {
                throw new DynamsoftLicenseException(
                    $"Offline license activation failed: {ex.Message}");
            }
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
    public class DynamsoftBarcodeService : IDisposable
    {
        private readonly CaptureVisionRouter _router;

        public DynamsoftBarcodeService(string licenseKey)
        {
            // Must initialize licence first
            DynamsoftLicenseManager.Initialize(licenseKey);

            // Only now can we create the router
            _router = new CaptureVisionRouter();
        }

        public string[] ReadBarcodes(string filePath)
        {
            CapturedResult capturedResult = _router.Capture(filePath,
                PresetTemplate.PT_READ_BARCODES);

            DecodedBarcodesResult barcodes = capturedResult.GetDecodedBarcodesResult();
            if (barcodes == null) return Array.Empty<string>();

            BarcodeResultItem[] items = barcodes.GetItems();
            string[] values = new string[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                values[i] = items[i].GetText();
            }

            return values;
        }

        public void Dispose() => _router?.Dispose();
    }
}

// ============================================================
// IRONBARCODE APPROACH: Simple Key-Based Licensing
// ============================================================

namespace IronBarcodeLicensing
{
    using IronBarCode;

    public class IronBarcodeLicenseManager
    {
        /// <summary>
        /// Set IronBarcode license - a single line, no network calls.
        /// </summary>
        public static void Initialize(string licenseKey)
        {
            // That's it. One line. No network validation.
            // No callbacks, no error handling required for licensing.
            IronBarCode.License.LicenseKey = licenseKey;

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
            // Set licence (can be done anywhere, anytime)
            IronBarCode.License.LicenseKey = licenseKey;

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
         * - Outbound HTTPS access to Dynamsoft's licence servers on first
         *   activation and during periodic re-checks
         * - OR a pre-configured offline licence file bound to the container
         *   image / device fingerprint
         * - Environment variables for licence key
         * - Health checks must wait for licence activation
         *
         * Example Dockerfile snippet:
         *
         *   ENV DYNAMSOFT_LICENSE_KEY="your-key"
         *   # If using offline licensing:
         *   COPY license-file.lic /app/license/
         *   ENV DYNAMSOFT_LICENSE_FILE="/app/license/license-file.lic"
         *
         * Kubernetes considerations:
         * - License server reachable from cluster (or offline file mounted)
         * - NetworkPolicy must allow egress to Dynamsoft licence endpoints
         * - Pod restarts may trigger licence re-validation
         *
         *
         * IRONBARCODE DOCKER DEPLOYMENT:
         * ------------------------------
         *
         * Dockerfile requirements:
         * - Environment variable with licence key
         * - No network requirements
         *
         * Example Dockerfile snippet:
         *
         *   ENV IRONBARCODE_LICENSE_KEY="your-key"
         *
         * Kubernetes considerations:
         * - Secret containing licence key
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
         * - Build agents need licence-server access for integration tests
         * - Development/trial licences may have usage limits
         * - Parallel test execution may hit concurrent activation limits
         * - Online activation adds latency to first test run after a clean cache
         *
         *
         * IRONBARCODE IN CI/CD:
         * ---------------------
         *
         * - Licence key in environment variable or secrets
         * - No network calls during test execution
         * - No concurrent usage concerns
         * - Consistent test timing without licence-validation latency
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
        // Dynamsoft: Must handle potential licence failures
        try
        {
            DynamsoftLicensing.DynamsoftLicenseManager.Initialize("DYNAMSOFT-LICENSE-KEY");
            using var dynamsoftService = new DynamsoftLicensing.DynamsoftBarcodeService("DYNAMSOFT-LICENSE-KEY");
            // Use service...
        }
        catch (DynamsoftLicensing.DynamsoftLicenseException ex)
        {
            Console.WriteLine($"Dynamsoft license error: {ex.Message}");
            // Application cannot proceed without valid licence
        }

        // IronBarcode: Simple assignment, works locally
        IronBarcodeLicensing.IronBarcodeLicenseManager.Initialize("IRONBARCODE-LICENSE-KEY");
        var ironService = new IronBarcodeLicensing.IronBarcodeBarcodeService("IRONBARCODE-LICENSE-KEY");
        // Use service immediately - no error handling needed for licensing
    }
}
