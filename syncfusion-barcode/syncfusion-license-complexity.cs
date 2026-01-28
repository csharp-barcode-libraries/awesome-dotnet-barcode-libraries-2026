// =============================================================================
// Syncfusion vs IronBarcode: License Registration Complexity Comparison
// =============================================================================
// This file demonstrates the difference in license setup between Syncfusion
// Barcode (part of Essential Studio suite) and IronBarcode (standalone SDK).
//
// Key differences highlighted:
// 1. Syncfusion requires version-specific license keys
// 2. Syncfusion Community License has strict eligibility requirements
// 3. IronBarcode uses a single, simple license key
// =============================================================================

using System;

namespace LicenseComplexityComparison
{
    // =========================================================================
    // SYNCFUSION LICENSE REGISTRATION
    // =========================================================================
    // Syncfusion requires license registration to remove trial messages.
    // The license key is tied to the Essential Studio version you purchased.
    // =========================================================================

    /// <summary>
    /// Syncfusion license configuration requires careful version management
    /// and awareness of Community License eligibility restrictions.
    /// </summary>
    public class SyncfusionLicenseSetup
    {
        /// <summary>
        /// Register Syncfusion license at application startup.
        /// This must be called before any Syncfusion control is instantiated.
        /// </summary>
        public static void RegisterLicense()
        {
            // ---------------------------------------------------------------------
            // IMPORTANT: License key format varies by Essential Studio version
            // Each major version (24.x, 25.x, 26.x) requires its own key
            // You must update the key when upgrading Essential Studio versions
            // ---------------------------------------------------------------------

            // Option 1: Direct license key registration
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
                "YOUR-LICENSE-KEY-FROM-SYNCFUSION-ACCOUNT-PORTAL"
            );

            // The license key is obtained from:
            // 1. Log into your Syncfusion account
            // 2. Navigate to Downloads > License Keys
            // 3. Select the correct Essential Studio version
            // 4. Copy the key for your purchased platform
        }

        /// <summary>
        /// For ASP.NET Core / Blazor applications, register in Program.cs
        /// </summary>
        public static void RegisterInBlazorApp()
        {
            // In Program.cs, add before builder.Build():
            // Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("KEY");
            //
            // Also required in Program.cs:
            // builder.Services.AddSyncfusionBlazor();
            //
            // And in _Imports.razor:
            // @using Syncfusion.Blazor
            // @using Syncfusion.Blazor.BarcodeGenerator
        }

        /// <summary>
        /// For WinForms applications, register before Application.Run()
        /// </summary>
        public static void RegisterInWinFormsApp()
        {
            // In Program.cs Main method:
            // Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("KEY");
            // Application.EnableVisualStyles();
            // Application.Run(new MainForm());
        }
    }

    // =========================================================================
    // SYNCFUSION COMMUNITY LICENSE RESTRICTIONS
    // =========================================================================
    // The Community License appears free but has strict eligibility rules.
    // Violation of these terms requires purchasing commercial licenses.
    // =========================================================================

    /// <summary>
    /// Documents the Community License eligibility requirements.
    /// ALL conditions must be met simultaneously.
    /// </summary>
    public static class CommunityLicenseEligibility
    {
        // Eligibility Requirements (ALL must be TRUE):

        /// <summary>
        /// Company annual gross revenue must be less than $1 million USD.
        /// This includes ALL revenue, not just software revenue.
        /// </summary>
        public const decimal MaxAnnualRevenue = 1_000_000.00m;

        /// <summary>
        /// Company must have 5 or fewer developers.
        /// Developer = anyone who writes or modifies code using Syncfusion.
        /// </summary>
        public const int MaxDevelopers = 5;

        /// <summary>
        /// Company must have 10 or fewer total employees.
        /// This includes all roles: developers, designers, managers, etc.
        /// </summary>
        public const int MaxEmployees = 10;

        /// <summary>
        /// Company must have never received more than $3 million in outside capital.
        /// This includes all funding rounds, loans, investments.
        /// </summary>
        public const decimal MaxOutsideCapital = 3_000_000.00m;

        // Automatic Disqualifications:

        /// <summary>
        /// Government organizations are NOT eligible regardless of size.
        /// </summary>
        public const bool GovernmentOrgsEligible = false;

        /// <summary>
        /// Check if organization qualifies for Community License.
        /// </summary>
        public static bool IsEligible(
            decimal annualRevenue,
            int developerCount,
            int employeeCount,
            decimal outsideCapital,
            bool isGovernment)
        {
            // Government organizations never qualify
            if (isGovernment)
                return false;

            // All financial/team thresholds must be met
            return annualRevenue < MaxAnnualRevenue
                && developerCount <= MaxDevelopers
                && employeeCount <= MaxEmployees
                && outsideCapital <= MaxOutsideCapital;
        }

        /// <summary>
        /// The license agreement includes provisions for verification.
        /// Syncfusion may request documentation proving eligibility.
        /// </summary>
        public static void AuditRiskWarning()
        {
            // If your company is approaching any threshold:
            // - Budget for commercial licensing
            // - Don't rely on Community License for production systems
            // - Plan migration before hitting limits
        }
    }

    // =========================================================================
    // IRONBARCODE LICENSE REGISTRATION
    // =========================================================================
    // IronBarcode uses a simple license key string with no version restrictions.
    // =========================================================================

    /// <summary>
    /// IronBarcode license configuration is straightforward.
    /// One key works across all versions (major version updates may require renewal).
    /// </summary>
    public class IronBarcodeLicenseSetup
    {
        /// <summary>
        /// Register IronBarcode license at application startup.
        /// Single line, works across all supported platforms.
        /// </summary>
        public static void RegisterLicense()
        {
            // One line - that's it
            IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";
        }

        /// <summary>
        /// For environment-based configuration (recommended for CI/CD)
        /// </summary>
        public static void RegisterFromEnvironment()
        {
            // Store key in environment variable for security
            IronBarCode.License.LicenseKey =
                Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
        }

        /// <summary>
        /// For appsettings.json configuration
        /// </summary>
        public static void RegisterFromConfiguration(string keyFromConfig)
        {
            // Read from IConfiguration in your DI setup
            IronBarCode.License.LicenseKey = keyFromConfig;
        }
    }

    // =========================================================================
    // DOCKER DEPLOYMENT COMPARISON
    // =========================================================================
    // License deployment in containerized environments differs significantly.
    // =========================================================================

    /// <summary>
    /// Compares license deployment in Docker containers.
    /// </summary>
    public static class DockerDeploymentComparison
    {
        /// <summary>
        /// Syncfusion Docker deployment requires license key in environment
        /// </summary>
        public static void SyncfusionDockerExample()
        {
            // Dockerfile approach:
            // ENV SYNCFUSION_LICENSE_KEY=your-version-specific-key
            //
            // Then in startup code:
            // var key = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
            // Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(key);
            //
            // Note: Key changes when upgrading Syncfusion NuGet packages
            // CI/CD pipeline must update secret when version changes
        }

        /// <summary>
        /// IronBarcode Docker deployment
        /// </summary>
        public static void IronBarcodeDockerExample()
        {
            // Dockerfile approach:
            // ENV IRONBARCODE_LICENSE=your-license-key
            //
            // Then in startup code (one line):
            // IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
            //
            // Same key works across updates (within major version)
        }
    }

    // =========================================================================
    // SIDE-BY-SIDE COMPARISON SUMMARY
    // =========================================================================

    /// <summary>
    /// Quick reference comparison table
    /// </summary>
    public static class LicenseComparisonSummary
    {
        /*
        | Aspect                    | Syncfusion              | IronBarcode         |
        |---------------------------|-------------------------|---------------------|
        | License key format        | Version-specific        | Version-agnostic    |
        | Registration method       | Static class method     | Property assignment |
        | Code lines required       | 1-3                     | 1                   |
        | Free tier                 | Community (restricted)  | Trial (30 days)     |
        | Free tier restrictions    | Revenue/team caps       | Watermark only      |
        | Audit risk               | Yes (eligibility check) | No                  |
        | Key update on upgrade    | Sometimes required      | Rarely required     |
        | Platform-specific setup   | Yes (varies by target)  | No                  |
        | Docker deployment         | Environment variable    | Environment variable|
        */
    }

    // =========================================================================
    // PRACTICAL EXAMPLE: COMPLETE STARTUP CONFIGURATION
    // =========================================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            // -------------------------------------------------------------
            // SYNCFUSION STARTUP (more steps)
            // -------------------------------------------------------------
            // 1. Register license (before any Syncfusion control used)
            // Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("KEY");

            // 2. For Blazor, also add in Program.cs:
            // builder.Services.AddSyncfusionBlazor();

            // 3. For Blazor, add in _Imports.razor:
            // @using Syncfusion.Blazor
            // @using Syncfusion.Blazor.BarcodeGenerator

            // 4. Verify you're using correct Essential Studio version
            // Key from v24.x won't work with v25.x packages

            // -------------------------------------------------------------
            // IRONBARCODE STARTUP (one line)
            // -------------------------------------------------------------
            IronBarCode.License.LicenseKey = "YOUR-KEY";

            // That's it. Works across all platforms, all supported versions.
            // No additional configuration, no version matching required.
        }
    }
}
