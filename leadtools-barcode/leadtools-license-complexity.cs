// =============================================================================
// LEADTOOLS Barcode: License Complexity Demo
// =============================================================================
// This example demonstrates the licensing differences between LEADTOOLS
// (file-based two-tier licensing) and IronBarcode (simple key-based licensing).
//
// Author: Jacob Mellor, CTO of Iron Software
// Last verified: January 2026
// =============================================================================

/*
 * INSTALLATION
 *
 * LEADTOOLS:
 * dotnet add package Leadtools.Barcode
 * dotnet add package Leadtools
 * dotnet add package Leadtools.Codecs
 *
 * IronBarcode:
 * dotnet add package IronBarcode
 */

using System;
using System.IO;

// LEADTOOLS namespaces (commented because they require license)
// using Leadtools;
// using Leadtools.Barcode;
// using Leadtools.Codecs;

// IronBarcode namespace
using IronBarCode;

namespace BarcodeComparison
{
    /// <summary>
    /// Demonstrates the license initialization complexity difference between
    /// LEADTOOLS (file-based, feature-locked) and IronBarcode (key-based, simple).
    /// </summary>
    public class LicenseComplexityComparison
    {
        // =====================================================================
        // PART 1: LEADTOOLS LICENSE INITIALIZATION (COMPLEX)
        // =====================================================================

        /// <summary>
        /// LEADTOOLS license initialization - requires multiple steps.
        /// </summary>
        public class LeadtoolsLicenseManager
        {
            private readonly string _licensePath;
            private readonly string _developerKey;

            public LeadtoolsLicenseManager(string licensePath, string developerKey)
            {
                _licensePath = licensePath;
                _developerKey = developerKey;
            }

            /// <summary>
            /// Initialize LEADTOOLS licensing - production code pattern.
            /// </summary>
            public void Initialize()
            {
                /*
                 * LEADTOOLS License Initialization Steps:
                 *
                 * 1. Locate license file (must be accessible at runtime)
                 * 2. Call SetLicense with file path AND developer key
                 * 3. Verify kernel is not expired
                 * 4. Check each feature you need is unlocked
                 * 5. Handle license failures gracefully
                 */

                Console.WriteLine("LEADTOOLS License Initialization:");
                Console.WriteLine("-".PadRight(60, '-'));

                // Step 1: Verify license file exists
                Console.WriteLine($"Step 1: Checking license file at: {_licensePath}");
                if (!File.Exists(_licensePath))
                {
                    throw new FileNotFoundException(
                        $"LEADTOOLS license file not found: {_licensePath}. " +
                        "You must deploy the license file with your application.");
                }
                Console.WriteLine("  [OK] License file found");

                // Step 2: Set the license (requires both file AND key)
                Console.WriteLine("Step 2: Setting license...");
                /*
                RasterSupport.SetLicense(_licensePath, _developerKey);
                */
                Console.WriteLine("  RasterSupport.SetLicense(licensePath, developerKey)");
                Console.WriteLine("  [OK] License set");

                // Step 3: Verify kernel is not expired
                Console.WriteLine("Step 3: Verifying license validity...");
                /*
                if (RasterSupport.KernelExpired)
                {
                    throw new InvalidOperationException(
                        "LEADTOOLS license has expired. " +
                        "Contact LEAD Technologies to renew your license.");
                }
                */
                Console.WriteLine("  if (RasterSupport.KernelExpired) { throw... }");
                Console.WriteLine("  [OK] License not expired");

                // Step 4: Check each barcode feature is unlocked
                Console.WriteLine("Step 4: Checking barcode feature locks...");
                CheckBarcodeFeatures();

                Console.WriteLine();
                Console.WriteLine("LEADTOOLS initialization complete (5 steps, 20+ lines)");
            }

            private void CheckBarcodeFeatures()
            {
                /*
                 * LEADTOOLS uses feature-based licensing.
                 * Each capability must be checked separately.
                 */

                var features = new[]
                {
                    ("Barcode1DRead", "1D barcode reading"),
                    ("Barcode2DRead", "2D barcode reading"),
                    ("Barcode1DWrite", "1D barcode writing"),
                    ("Barcode2DWrite", "2D barcode writing"),
                    ("BarcodeQRRead", "QR code reading"),
                    ("BarcodeQRWrite", "QR code writing"),
                    ("BarcodePDF417Read", "PDF417 reading"),
                    ("BarcodePDF417Write", "PDF417 writing"),
                    ("BarcodeDataMatrixRead", "DataMatrix reading"),
                    ("BarcodeDataMatrixWrite", "DataMatrix writing"),
                };

                foreach (var (featureType, description) in features)
                {
                    /*
                    if (RasterSupport.IsLocked(RasterSupportType.{featureType}))
                    {
                        throw new InvalidOperationException($"{description} is locked");
                    }
                    */
                    Console.WriteLine($"  Checking {description}...");
                }

                Console.WriteLine("  [OK] All barcode features unlocked");
            }
        }

        /// <summary>
        /// LEADTOOLS deployment license complexity.
        /// </summary>
        public class LeadtoolsDeploymentLicense
        {
            public void ExplainDeploymentLicensing()
            {
                Console.WriteLine("=".PadRight(70, '='));
                Console.WriteLine("LEADTOOLS DEPLOYMENT LICENSE REQUIREMENTS");
                Console.WriteLine("=".PadRight(70, '='));
                Console.WriteLine();

                Console.WriteLine(@"
LEADTOOLS uses a TWO-TIER licensing model:

1. DEVELOPMENT LICENSE (what you buy first):
   - Per-developer annual subscription
   - ~$1,295-$1,469 per developer
   - Enables development and testing
   - DOES NOT include production deployment rights

2. DEPLOYMENT LICENSE (what you also need):
   - Separate purchase from development license
   - Pricing varies by deployment type
   - Must contact sales for quotes
   - Options include:

   | Deployment Type      | Pricing Model     |
   |---------------------|-------------------|
   | Server Application   | Per server/year   |
   | Desktop Application  | Per seat          |
   | Mobile Application   | Per app           |
   | OEM/Redistribution   | Custom negotiation|

PROBLEMS WITH THIS MODEL:

1. Budget Unpredictability:
   - Can't know deployment cost without sales call
   - Pricing may change between quote and purchase
   - Volume discounts require negotiation

2. Ongoing License Management:
   - Track development licenses per developer
   - Track deployment licenses per server
   - Manage license renewals separately
   - Audit compliance across teams

3. Deployment Complexity:
   - License FILE must be present on each server
   - File path must be configured correctly
   - Secrets management for developer key
   - Different files for dev vs prod
");
            }
        }

        // =====================================================================
        // PART 2: IRONBARCODE LICENSE INITIALIZATION (SIMPLE)
        // =====================================================================

        /// <summary>
        /// IronBarcode license initialization - single line.
        /// </summary>
        public class IronBarcodeLicenseManager
        {
            /// <summary>
            /// Initialize IronBarcode licensing - production code pattern.
            /// </summary>
            public void Initialize()
            {
                Console.WriteLine("IronBarcode License Initialization:");
                Console.WriteLine("-".PadRight(60, '-'));

                // ONE step. ONE line. That's it.
                Console.WriteLine("Step 1: Set license key");

                // Option A: Direct key
                IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
                Console.WriteLine("  IronBarCode.License.LicenseKey = \"YOUR-KEY\";");

                // Option B: From environment variable (recommended for production)
                var envKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
                if (!string.IsNullOrEmpty(envKey))
                {
                    IronBarCode.License.LicenseKey = envKey;
                    Console.WriteLine("  OR from environment: IronBarCode.License.LicenseKey = env[\"IRONBARCODE_LICENSE\"]");
                }

                Console.WriteLine();
                Console.WriteLine("IronBarcode initialization complete (1 step, 1 line)");
            }

            public void ExplainLicensingModel()
            {
                Console.WriteLine("=".PadRight(70, '='));
                Console.WriteLine("IRONBARCODE LICENSING MODEL");
                Console.WriteLine("=".PadRight(70, '='));
                Console.WriteLine();

                Console.WriteLine(@"
IronBarcode uses SINGLE-TIER perpetual licensing:

1. ONE LICENSE COVERS EVERYTHING:
   - Development: Included
   - Testing: Included
   - Production: Included
   - All features: Included
   - No deployment licenses: Correct, none needed

2. PRICING IS TRANSPARENT:
   | License     | Price (one-time) | Developers | Servers   |
   |-------------|------------------|------------|-----------|
   | Lite        | $749             | 1          | Unlimited |
   | Professional| $1,499           | 10         | Unlimited |
   | Enterprise  | $2,999           | Unlimited  | Unlimited |

3. NO ONGOING FEES:
   - Perpetual license (you own it)
   - No annual renewal required
   - No per-server fees
   - No deployment tracking

4. SIMPLE DEPLOYMENT:
   - Set environment variable: IRONBARCODE_LICENSE=your-key
   - No license files to manage
   - No file paths to configure
   - Works in Docker, Kubernetes, serverless, anywhere
");
            }
        }

        // =====================================================================
        // PART 3: DOCKER DEPLOYMENT COMPARISON
        // =====================================================================

        /// <summary>
        /// Shows Docker deployment differences.
        /// </summary>
        public class DockerDeploymentComparison
        {
            public void ShowLeadtoolsDockerfile()
            {
                Console.WriteLine("LEADTOOLS Dockerfile:");
                Console.WriteLine("-".PadRight(60, '-'));
                Console.WriteLine(@"
# LEADTOOLS requires license file mounting
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy application
COPY publish/ .

# PROBLEM 1: License file must be included
COPY LEADTOOLS.LIC /app/license/LEADTOOLS.LIC

# PROBLEM 2: Path must be configured
ENV LEADTOOLS_LICENSE_PATH=/app/license/LEADTOOLS.LIC

# PROBLEM 3: Key is separate from file
ENV LEADTOOLS_DEVELOPER_KEY=your-developer-key

# PROBLEM 4: Need runtime verification in code
# if (!File.Exists(Environment.GetEnvironmentVariable(""LEADTOOLS_LICENSE_PATH"")))
# {
#     throw new FileNotFoundException(""License file missing"");
# }

ENTRYPOINT [""dotnet"", ""YourApp.dll""]

# ISSUES:
# - License file is a deployment artifact
# - File must be readable at runtime
# - Path configuration is error-prone
# - Secrets management is complex (file + key)
# - CI/CD must handle license file copying
");
            }

            public void ShowIronBarcodeDockerfile()
            {
                Console.WriteLine("IronBarcode Dockerfile:");
                Console.WriteLine("-".PadRight(60, '-'));
                Console.WriteLine(@"
# IronBarcode uses simple environment variable
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy application
COPY publish/ .

# SIMPLE: Just set the key
ENV IRONBARCODE_LICENSE=your-license-key

ENTRYPOINT [""dotnet"", ""YourApp.dll""]

# ADVANTAGES:
# - No files to manage
# - Standard secrets management (env vars)
# - Works with Kubernetes secrets
# - Works with Docker secrets
# - Works with AWS Secrets Manager
# - Works with Azure Key Vault
# - No runtime file access needed
");
            }
        }

        // =====================================================================
        // PART 4: CI/CD PIPELINE COMPARISON
        // =====================================================================

        /// <summary>
        /// Shows CI/CD complexity differences.
        /// </summary>
        public class CiCdComparison
        {
            public void ShowLeadtoolsPipeline()
            {
                Console.WriteLine("LEADTOOLS GitHub Actions Workflow:");
                Console.WriteLine("-".PadRight(60, '-'));
                Console.WriteLine(@"
# LEADTOOLS CI/CD requires file handling
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      # PROBLEM 1: License file must be created from secrets
      - name: Create license file
        run: |
          echo ""${{ secrets.LEADTOOLS_LICENSE_CONTENT }}"" | base64 -d > LEADTOOLS.LIC

      # PROBLEM 2: Developer key is separate
      - name: Set developer key
        run: echo ""LEADTOOLS_KEY=${{ secrets.LEADTOOLS_DEVELOPER_KEY }}"" >> $GITHUB_ENV

      - name: Build
        run: dotnet build

      # PROBLEM 3: Tests need license file path
      - name: Test
        env:
          LEADTOOLS_LICENSE_PATH: ${{ github.workspace }}/LEADTOOLS.LIC
          LEADTOOLS_DEVELOPER_KEY: ${{ env.LEADTOOLS_KEY }}
        run: dotnet test

      # PROBLEM 4: License file must be included in artifacts
      - name: Publish
        run: |
          dotnet publish -o publish
          cp LEADTOOLS.LIC publish/
");
            }

            public void ShowIronBarcodePipeline()
            {
                Console.WriteLine("IronBarcode GitHub Actions Workflow:");
                Console.WriteLine("-".PadRight(60, '-'));
                Console.WriteLine(@"
# IronBarcode CI/CD is straightforward
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Build
        run: dotnet build

      # SIMPLE: Just set the environment variable
      - name: Test
        env:
          IRONBARCODE_LICENSE: ${{ secrets.IRONBARCODE_LICENSE }}
        run: dotnet test

      - name: Publish
        run: dotnet publish -o publish
        # No license file to copy!
");
            }
        }

        // =====================================================================
        // PART 5: SIDE-BY-SIDE COMPARISON
        // =====================================================================

        public void PrintLicenseComparisonTable()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("LICENSE MODEL COMPARISON");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            var comparisons = new[]
            {
                ("License type", "File + Key (two-tier)", "Key only (single-tier)"),
                ("Development license", "$1,295-$1,469/dev/year", "$749-$2,999 one-time"),
                ("Deployment license", "Separate purchase", "Included"),
                ("Deployment pricing", "Contact sales", "Published on website"),
                ("License artifact", "File + Key string", "Key string only"),
                ("Docker deployment", "Mount file + set key", "Set environment variable"),
                ("Kubernetes secrets", "File + key secrets", "Single key secret"),
                ("CI/CD complexity", "High (file handling)", "Low (env variable)"),
                ("Feature locking", "Per-feature unlock", "All features included"),
                ("License verification", "Multi-step check", "Automatic"),
                ("Renewal tracking", "Dev + Deploy separate", "Optional (perpetual)"),
                ("Budget predictability", "Requires sales quote", "Published pricing"),
            };

            Console.WriteLine($"{"Aspect",-25} {"LEADTOOLS",-25} {"IronBarcode",-20}");
            Console.WriteLine("-".PadRight(70, '-'));

            foreach (var (aspect, leadtools, ironbarcode) in comparisons)
            {
                Console.WriteLine($"{aspect,-25} {leadtools,-25} {ironbarcode,-20}");
            }
        }
    }

    // =========================================================================
    // DEMONSTRATION
    // =========================================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("LEADTOOLS vs IRONBARCODE: LICENSE COMPLEXITY COMPARISON");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            var comparison = new LicenseComplexityComparison();

            // 1. LEADTOOLS initialization
            Console.WriteLine("SECTION 1: LEADTOOLS LICENSE INITIALIZATION");
            Console.WriteLine();
            var leadtoolsLicense = new LicenseComplexityComparison.LeadtoolsLicenseManager(
                @"C:\LEADTOOLS23\Support\Common\License\LEADTOOLS.LIC",
                "demo-developer-key");
            leadtoolsLicense.Initialize();
            Console.WriteLine();

            // 2. IronBarcode initialization
            Console.WriteLine("SECTION 2: IRONBARCODE LICENSE INITIALIZATION");
            Console.WriteLine();
            var ironBarcodeLicense = new LicenseComplexityComparison.IronBarcodeLicenseManager();
            ironBarcodeLicense.Initialize();
            Console.WriteLine();

            // 3. Deployment licensing explanation
            Console.WriteLine("SECTION 3: DEPLOYMENT LICENSE MODELS");
            Console.WriteLine();
            var leadtoolsDeployment = new LicenseComplexityComparison.LeadtoolsDeploymentLicense();
            leadtoolsDeployment.ExplainDeploymentLicensing();
            Console.WriteLine();

            ironBarcodeLicense.ExplainLicensingModel();
            Console.WriteLine();

            // 4. Docker comparison
            Console.WriteLine("SECTION 4: DOCKER DEPLOYMENT");
            Console.WriteLine();
            var dockerComparison = new LicenseComplexityComparison.DockerDeploymentComparison();
            dockerComparison.ShowLeadtoolsDockerfile();
            Console.WriteLine();
            dockerComparison.ShowIronBarcodeDockerfile();
            Console.WriteLine();

            // 5. CI/CD comparison
            Console.WriteLine("SECTION 5: CI/CD PIPELINE");
            Console.WriteLine();
            var cicdComparison = new LicenseComplexityComparison.CiCdComparison();
            cicdComparison.ShowLeadtoolsPipeline();
            Console.WriteLine();
            cicdComparison.ShowIronBarcodePipeline();
            Console.WriteLine();

            // 6. Summary table
            comparison.PrintLicenseComparisonTable();

            Console.WriteLine();
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("CONCLUSION: IronBarcode licensing is simpler at every level");
            Console.WriteLine("=".PadRight(70, '='));
        }
    }
}
