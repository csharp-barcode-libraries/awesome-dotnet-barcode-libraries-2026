/**
 * Licensing Model Comparison: Aspose.BarCode vs IronBarcode
 *
 * This example demonstrates the licensing and deployment differences
 * between Aspose.BarCode's subscription model and IronBarcode's
 * perpetual licensing option.
 *
 * Key Differences:
 * - Aspose.BarCode: Subscription-only ($999+/year)
 * - IronBarcode: Perpetual option available ($749 one-time)
 * - Aspose: License file or metered API deployment
 * - IronBarcode: Simple license key string
 *
 * NuGet Packages Required:
 * - Aspose.BarCode: Aspose.BarCode version 24.x+
 * - IronBarcode: IronBarcode version 2024.x+
 *
 * Note: This file documents factual licensing differences for
 * developers evaluating options. Prices as of January 2026.
 */

using System;
using System.IO;

// ============================================================================
// EXAMPLE 1: License Configuration at Application Startup
// ============================================================================

namespace LicenseConfiguration
{
    /// <summary>
    /// Aspose.BarCode license configuration requires license file deployment
    /// or metered API key setup. This adds complexity to deployment pipelines.
    /// </summary>
    public class AsposeLicenseExample
    {
        public void ConfigureLicenseFromFile()
        {
            // Option A: License file deployment
            // Requires .lic file to be deployed with application
            var license = new Aspose.BarCode.License();

            // File must be accessible at runtime
            string licensePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Aspose.BarCode.lic");

            if (File.Exists(licensePath))
            {
                license.SetLicense(licensePath);
                Console.WriteLine("Aspose license loaded from file");
            }
            else
            {
                Console.WriteLine("Warning: License file not found. " +
                    "Running in evaluation mode with watermarks.");
            }
        }

        public void ConfigureLicenseFromStream()
        {
            // Option B: License from embedded resource or stream
            var license = new Aspose.BarCode.License();

            // Load from embedded resource
            var assembly = typeof(AsposeLicenseExample).Assembly;
            using var stream = assembly.GetManifestResourceStream(
                "MyApp.Resources.Aspose.BarCode.lic");

            if (stream != null)
            {
                license.SetLicense(stream);
                Console.WriteLine("Aspose license loaded from embedded resource");
            }
        }

        public void ConfigureMeteredLicense()
        {
            // Option C: Metered licensing (pay-per-use)
            // Requires public/private key pair from Aspose account
            var metered = new Aspose.BarCode.Metered();

            string publicKey = Environment.GetEnvironmentVariable("ASPOSE_PUBLIC_KEY");
            string privateKey = Environment.GetEnvironmentVariable("ASPOSE_PRIVATE_KEY");

            if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey))
            {
                metered.SetMeteredKey(publicKey, privateKey);
                Console.WriteLine("Aspose metered license configured");

                // Note: This tracks API usage and bills accordingly
                // Requires internet connectivity for usage tracking
            }
        }

        public void CheckLicenseStatus()
        {
            // Verify license is properly loaded
            if (Aspose.BarCode.License.IsLicensed)
            {
                Console.WriteLine("Aspose.BarCode is licensed");
            }
            else
            {
                Console.WriteLine("Aspose.BarCode is in evaluation mode");
                Console.WriteLine("- Generated barcodes will have watermarks");
                Console.WriteLine("- Recognition results may be limited");
            }
        }
    }

    /// <summary>
    /// IronBarcode uses a simple license key string.
    /// No file deployment or metering complexity required.
    /// </summary>
    public class IronBarcodeLicenseExample
    {
        public void ConfigureLicense()
        {
            // Single line configuration at application startup
            IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY-HERE";
        }

        public void ConfigureFromEnvironment()
        {
            // Production best practice: use environment variable
            IronBarCode.License.LicenseKey =
                Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
        }

        public void ConfigureFromConfiguration()
        {
            // Or from appsettings.json via configuration
            // IronBarCode.License.LicenseKey = Configuration["IronBarcode:LicenseKey"];
        }

        public void CheckLicenseStatus()
        {
            if (IronBarCode.License.IsLicensed)
            {
                Console.WriteLine("IronBarcode is licensed");
            }
            else
            {
                Console.WriteLine("IronBarcode is in trial mode");
                Console.WriteLine("- Full functionality available");
                Console.WriteLine("- Output includes trial watermark");
            }
        }
    }
}


// ============================================================================
// EXAMPLE 2: Docker/Container Deployment
// ============================================================================

namespace ContainerDeployment
{
    /// <summary>
    /// Aspose.BarCode in Docker requires license file mounting
    /// or environment variables for metered licensing.
    /// </summary>
    public class AsposeDockerDeployment
    {
        /*
         * Dockerfile for Aspose.BarCode:
         * ================================
         *
         * FROM mcr.microsoft.com/dotnet/aspnet:8.0
         * WORKDIR /app
         *
         * # Copy application
         * COPY bin/Release/net8.0/publish .
         *
         * # OPTION A: Include license file in image (not recommended for secrets)
         * COPY Aspose.BarCode.lic /app/license/
         * ENV ASPOSE_LICENSE_PATH=/app/license/Aspose.BarCode.lic
         *
         * # OPTION B: Mount license file at runtime (docker-compose)
         * # volumes:
         * #   - ./licenses:/app/license:ro
         *
         * # OPTION C: Metered licensing via environment
         * # ENV ASPOSE_PUBLIC_KEY=your-public-key
         * # ENV ASPOSE_PRIVATE_KEY=your-private-key
         *
         * ENTRYPOINT ["dotnet", "MyApp.dll"]
         */

        public void InitializeLicenseInContainer()
        {
            var licensePath = Environment.GetEnvironmentVariable("ASPOSE_LICENSE_PATH");

            if (!string.IsNullOrEmpty(licensePath) && File.Exists(licensePath))
            {
                var license = new Aspose.BarCode.License();
                license.SetLicense(licensePath);
            }
            else
            {
                // Try metered fallback
                var publicKey = Environment.GetEnvironmentVariable("ASPOSE_PUBLIC_KEY");
                var privateKey = Environment.GetEnvironmentVariable("ASPOSE_PRIVATE_KEY");

                if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey))
                {
                    var metered = new Aspose.BarCode.Metered();
                    metered.SetMeteredKey(publicKey, privateKey);
                }
            }
        }
    }

    /// <summary>
    /// IronBarcode in Docker uses a simple environment variable.
    /// No file mounting or complex configuration required.
    /// </summary>
    public class IronBarcodeDockerDeployment
    {
        /*
         * Dockerfile for IronBarcode:
         * ============================
         *
         * FROM mcr.microsoft.com/dotnet/aspnet:8.0
         * WORKDIR /app
         *
         * # Copy application
         * COPY bin/Release/net8.0/publish .
         *
         * # License via environment variable (set at runtime)
         * # docker run -e IRONBARCODE_LICENSE=your-key myapp
         *
         * ENTRYPOINT ["dotnet", "MyApp.dll"]
         */

        public void InitializeLicenseInContainer()
        {
            // Single line - no file dependencies
            IronBarCode.License.LicenseKey =
                Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
        }
    }
}


// ============================================================================
// EXAMPLE 3: CI/CD Pipeline Configuration
// ============================================================================

namespace CICDConfiguration
{
    /// <summary>
    /// Aspose.BarCode in CI/CD pipelines requires secret file management
    /// or metered key secrets.
    /// </summary>
    public class AsposeCICDExample
    {
        /*
         * GitHub Actions Example (Aspose.BarCode):
         * =========================================
         *
         * jobs:
         *   build:
         *     runs-on: ubuntu-latest
         *     steps:
         *       - uses: actions/checkout@v4
         *
         *       # OPTION A: License file from secrets (base64 encoded)
         *       - name: Create license file
         *         run: |
         *           echo "${{ secrets.ASPOSE_LICENSE_BASE64 }}" | base64 -d > Aspose.BarCode.lic
         *
         *       # OPTION B: Metered keys as environment variables
         *       - name: Build with Aspose
         *         env:
         *           ASPOSE_PUBLIC_KEY: ${{ secrets.ASPOSE_PUBLIC_KEY }}
         *           ASPOSE_PRIVATE_KEY: ${{ secrets.ASPOSE_PRIVATE_KEY }}
         *         run: dotnet build
         *
         * Azure DevOps Example:
         * ======================
         *
         * - task: DownloadSecureFile@1
         *   name: asposeLicense
         *   inputs:
         *     secureFile: 'Aspose.BarCode.lic'
         *
         * - script: |
         *     cp $(asposeLicense.secureFilePath) $(Build.SourcesDirectory)/
         *   displayName: 'Copy license file'
         */
    }

    /// <summary>
    /// IronBarcode in CI/CD uses standard secret management
    /// with no file handling complexity.
    /// </summary>
    public class IronBarcodeCICDExample
    {
        /*
         * GitHub Actions Example (IronBarcode):
         * =====================================
         *
         * jobs:
         *   build:
         *     runs-on: ubuntu-latest
         *     steps:
         *       - uses: actions/checkout@v4
         *
         *       - name: Build with IronBarcode
         *         env:
         *           IRONBARCODE_LICENSE: ${{ secrets.IRONBARCODE_LICENSE }}
         *         run: dotnet build
         *
         * Azure DevOps Example:
         * ======================
         *
         * - script: dotnet build
         *   env:
         *     IRONBARCODE_LICENSE: $(IronBarcodeKey)
         *   displayName: 'Build with IronBarcode'
         *
         * Note: Single secret variable, no file management
         */
    }
}


// ============================================================================
// EXAMPLE 4: Cost Analysis Over Time
// ============================================================================

namespace CostAnalysis
{
    /// <summary>
    /// Factual cost comparison between Aspose.BarCode subscription
    /// and IronBarcode perpetual licensing models.
    /// Prices as of January 2026.
    /// </summary>
    public static class LicensingCostComparison
    {
        // Aspose.BarCode Pricing (subscription required annually)
        public const decimal AsposeDevSingleYear = 999m;      // 1 developer
        public const decimal AsposeSiteYear = 4995m;          // Up to 10 developers
        public const decimal AsposeOEMYear = 14985m;          // Unlimited deployment

        // IronBarcode Pricing (one-time perpetual)
        public const decimal IronBarcodeLite = 749m;          // 1 developer
        public const decimal IronBarcodePlus = 1499m;         // 3 developers
        public const decimal IronBarcodeProfessional = 2999m; // 10 developers
        public const decimal IronBarcodeUnlimited = 5999m;    // Unlimited

        public static void CalculateFiveYearCost()
        {
            Console.WriteLine("=== 5-Year Total Cost Comparison ===\n");

            // Single Developer Scenario
            Console.WriteLine("Single Developer:");
            Console.WriteLine($"  Aspose.BarCode: ${AsposeDevSingleYear * 5:N0} ({AsposeDevSingleYear:N0}/year x 5)");
            Console.WriteLine($"  IronBarcode:    ${IronBarcodeLite:N0} (one-time)");
            Console.WriteLine($"  Savings:        ${(AsposeDevSingleYear * 5) - IronBarcodeLite:N0}\n");

            // Small Team (3 developers)
            Console.WriteLine("Small Team (3 developers):");
            Console.WriteLine($"  Aspose.BarCode: ${AsposeDevSingleYear * 3 * 5:N0} ({AsposeDevSingleYear * 3:N0}/year x 5)");
            Console.WriteLine($"  IronBarcode:    ${IronBarcodePlus:N0} (one-time)");
            Console.WriteLine($"  Savings:        ${(AsposeDevSingleYear * 3 * 5) - IronBarcodePlus:N0}\n");

            // Medium Team (10 developers)
            Console.WriteLine("Medium Team (10 developers):");
            Console.WriteLine($"  Aspose.BarCode: ${AsposeSiteYear * 5:N0} ({AsposeSiteYear:N0}/year x 5)");
            Console.WriteLine($"  IronBarcode:    ${IronBarcodeProfessional:N0} (one-time)");
            Console.WriteLine($"  Savings:        ${(AsposeSiteYear * 5) - IronBarcodeProfessional:N0}\n");
        }

        public static void CalculateBreakEvenPoint()
        {
            Console.WriteLine("=== Break-Even Analysis ===\n");

            // Single developer break-even
            decimal singleDevMonths = (IronBarcodeLite / (AsposeDevSingleYear / 12));
            Console.WriteLine($"Single Developer: IronBarcode pays for itself in {singleDevMonths:F1} months");

            // Team break-even
            decimal teamMonths = (IronBarcodeProfessional / (AsposeSiteYear / 12));
            Console.WriteLine($"10-Developer Team: IronBarcode pays for itself in {teamMonths:F1} months");
        }
    }

    /// <summary>
    /// Demonstrates the practical impact of subscription vs perpetual licensing.
    /// </summary>
    public static class LicenseExpirationScenarios
    {
        public static void DemonstrateSubscriptionRisks()
        {
            Console.WriteLine("=== Subscription License Considerations ===\n");

            Console.WriteLine("What happens when Aspose.BarCode subscription lapses:");
            Console.WriteLine("1. Cannot legally deploy new versions");
            Console.WriteLine("2. Existing deployments require valid license");
            Console.WriteLine("3. No access to security patches");
            Console.WriteLine("4. No access to bug fixes");
            Console.WriteLine("5. Must renew to restore compliance\n");

            Console.WriteLine("What happens with IronBarcode perpetual license:");
            Console.WriteLine("1. Licensed version continues working indefinitely");
            Console.WriteLine("2. Existing deployments remain fully licensed");
            Console.WriteLine("3. No expiration pressure");
            Console.WriteLine("4. Optional renewals for new versions only");
        }
    }
}


// ============================================================================
// EXAMPLE 5: Production Usage Patterns
// ============================================================================

namespace ProductionUsage
{
    /// <summary>
    /// Common production patterns with license management.
    /// </summary>
    public class AsposeProductionExample
    {
        private readonly bool _isLicensed;

        public AsposeProductionExample()
        {
            // Initialize license at startup
            InitializeLicense();
            _isLicensed = Aspose.BarCode.License.IsLicensed;
        }

        private void InitializeLicense()
        {
            // Check multiple sources for license
            var license = new Aspose.BarCode.License();

            // 1. Try environment path
            var envPath = Environment.GetEnvironmentVariable("ASPOSE_LICENSE_PATH");
            if (!string.IsNullOrEmpty(envPath) && File.Exists(envPath))
            {
                license.SetLicense(envPath);
                return;
            }

            // 2. Try common locations
            var commonPaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Aspose.BarCode.lic"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license", "Aspose.BarCode.lic"),
                "/etc/aspose/Aspose.BarCode.lic"  // Linux convention
            };

            foreach (var path in commonPaths)
            {
                if (File.Exists(path))
                {
                    license.SetLicense(path);
                    return;
                }
            }

            // 3. Try metered as fallback
            var publicKey = Environment.GetEnvironmentVariable("ASPOSE_PUBLIC_KEY");
            var privateKey = Environment.GetEnvironmentVariable("ASPOSE_PRIVATE_KEY");

            if (!string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(privateKey))
            {
                var metered = new Aspose.BarCode.Metered();
                metered.SetMeteredKey(publicKey, privateKey);
            }
        }

        public void GenerateBarcode(string data, string outputPath)
        {
            if (!_isLicensed)
            {
                Console.WriteLine("Warning: Operating in evaluation mode");
            }

            var generator = new Aspose.BarCode.Generation.BarcodeGenerator(
                Aspose.BarCode.Generation.EncodeTypes.Code128,
                data);

            generator.Save(outputPath,
                Aspose.BarCode.Generation.BarCodeImageFormat.Png);
        }
    }

    /// <summary>
    /// IronBarcode production usage is simpler with key-based licensing.
    /// </summary>
    public class IronBarcodeProductionExample
    {
        private readonly bool _isLicensed;

        public IronBarcodeProductionExample()
        {
            // Initialize license at startup - single line
            IronBarCode.License.LicenseKey =
                Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE")
                ?? ""; // Empty string for trial mode

            _isLicensed = IronBarCode.License.IsLicensed;
        }

        public void GenerateBarcode(string data, string outputPath)
        {
            if (!_isLicensed)
            {
                Console.WriteLine("Warning: Operating in trial mode");
            }

            IronBarCode.BarcodeWriter.CreateBarcode(data, IronBarCode.BarcodeEncoding.Code128)
                .SaveAsPng(outputPath);
        }
    }
}


// ============================================================================
// EXAMPLE 6: Kubernetes Deployment
// ============================================================================

namespace KubernetesDeployment
{
    /// <summary>
    /// Kubernetes deployment patterns for licensed software.
    /// </summary>
    public class KubernetesLicenseManagement
    {
        /*
         * Aspose.BarCode in Kubernetes:
         * ==============================
         *
         * Option A: License file via ConfigMap (not recommended for secrets)
         * -------------------------------------------------------------------
         * apiVersion: v1
         * kind: ConfigMap
         * metadata:
         *   name: aspose-license
         * data:
         *   Aspose.BarCode.lic: |
         *     <license content here>
         *
         * Option B: License file via Secret (recommended)
         * ------------------------------------------------
         * apiVersion: v1
         * kind: Secret
         * metadata:
         *   name: aspose-license
         * type: Opaque
         * data:
         *   Aspose.BarCode.lic: <base64-encoded-license>
         *
         * Deployment volume mount:
         * ------------------------
         * spec:
         *   containers:
         *   - name: myapp
         *     volumeMounts:
         *     - name: license-volume
         *       mountPath: /app/license
         *       readOnly: true
         *   volumes:
         *   - name: license-volume
         *     secret:
         *       secretName: aspose-license
         *
         * Option C: Metered keys via environment
         * --------------------------------------
         * spec:
         *   containers:
         *   - name: myapp
         *     env:
         *     - name: ASPOSE_PUBLIC_KEY
         *       valueFrom:
         *         secretKeyRef:
         *           name: aspose-metered
         *           key: public-key
         *     - name: ASPOSE_PRIVATE_KEY
         *       valueFrom:
         *         secretKeyRef:
         *           name: aspose-metered
         *           key: private-key
         */

        /*
         * IronBarcode in Kubernetes:
         * ==========================
         *
         * Simple environment variable from Secret:
         * -----------------------------------------
         * apiVersion: v1
         * kind: Secret
         * metadata:
         *   name: ironbarcode-license
         * type: Opaque
         * data:
         *   license-key: <base64-encoded-key>
         *
         * Deployment configuration:
         * -------------------------
         * spec:
         *   containers:
         *   - name: myapp
         *     env:
         *     - name: IRONBARCODE_LICENSE
         *       valueFrom:
         *         secretKeyRef:
         *           name: ironbarcode-license
         *           key: license-key
         *
         * Note: No volume mounts, no file management.
         * Single secret, single environment variable.
         */
    }
}


// ============================================================================
// LICENSING MODEL SUMMARY
// ============================================================================

/*
 * Licensing Model Comparison
 * ==========================
 *
 * Feature                    | Aspose.BarCode         | IronBarcode
 * ---------------------------|------------------------|-------------------------
 * License Type               | Subscription only      | Perpetual + subscription
 * Minimum Annual Cost        | $999/year              | $749 one-time
 * 10-Developer 5-Year Cost   | $24,975                | $2,999
 * License Deployment         | File or metered keys   | Single key string
 * Docker Complexity          | Volume mount or env    | Single env variable
 * CI/CD Complexity           | Secret file management | Single secret variable
 * Kubernetes Deployment      | Secret + volume mount  | Single secret + env
 * Expiration Consequence     | Cannot deploy/update   | Perpetual use rights
 * Renewal Requirement        | Mandatory annually     | Optional for updates
 *
 * Practical Implications:
 * =======================
 *
 * 1. Budget Planning
 *    - Aspose: Must budget annually indefinitely
 *    - IronBarcode: One-time capital expense
 *
 * 2. Deployment Simplicity
 *    - Aspose: File management across environments
 *    - IronBarcode: Environment variable only
 *
 * 3. Long-term Ownership
 *    - Aspose: Lose rights when subscription lapses
 *    - IronBarcode: Perpetual rights to purchased version
 *
 * 4. Team Scaling
 *    - Aspose: Cost scales with subscription tier
 *    - IronBarcode: One-time cost for team size
 *
 * Decision Framework:
 * ===================
 *
 * Choose Aspose.BarCode subscription when:
 * - Already using other Aspose products (bundle pricing)
 * - Organization prefers OPEX over CAPEX
 * - Need metered pricing for variable workloads
 * - Using Aspose.BarCode Cloud API
 *
 * Choose IronBarcode perpetual when:
 * - Budget-conscious over multi-year horizon
 * - Prefer simpler deployment model
 * - Want perpetual ownership rights
 * - Need predictable single expense
 */
