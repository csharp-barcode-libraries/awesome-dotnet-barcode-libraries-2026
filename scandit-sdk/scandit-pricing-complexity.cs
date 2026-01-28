/*
 * Scandit SDK vs IronBarcode: Pricing Complexity Analysis
 *
 * This file demonstrates the pricing model differences between
 * Scandit's volume-based enterprise pricing and IronBarcode's
 * transparent perpetual licensing.
 *
 * Key Insight: Scandit requires sales quotes with variable costs.
 * IronBarcode has fixed, published pricing.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.Collections.Generic;

namespace ScanditPricingAnalysis
{
    // ============================================================
    // SCANDIT PRICING MODEL
    // ============================================================

    /// <summary>
    /// Scandit uses enterprise volume-based pricing with multiple
    /// variables affecting total cost.
    /// </summary>
    public class ScanditPricingModel
    {
        /*
         * Scandit Pricing Variables:
         *
         * 1. Products Licensed:
         *    - SparkScan (consumer scanning UX)
         *    - MatrixScan (multi-barcode scanning)
         *    - ID Scanning (document verification)
         *    - AR Overlays (augmented reality features)
         *    - Parser (barcode data parsing)
         *
         * 2. Usage Tiers:
         *    - Per-device licensing
         *    - Per-scan licensing (some products)
         *    - Volume discounts at thresholds
         *
         * 3. Support Levels:
         *    - Standard support
         *    - Premium support
         *    - Enterprise support (SLA)
         *
         * 4. Contract Terms:
         *    - Annual contracts
         *    - Multi-year discounts
         *    - Bundling discounts
         *
         * 5. No Public Pricing:
         *    - Requires sales conversation
         *    - Custom quotes for each customer
         *    - Negotiation expected
         */

        public void PrintPricingComplexity()
        {
            Console.WriteLine("=== Scandit Pricing Complexity ===");
            Console.WriteLine();
            Console.WriteLine("To get Scandit pricing, you must:");
            Console.WriteLine("1. Contact Scandit sales team");
            Console.WriteLine("2. Explain your use case and volume");
            Console.WriteLine("3. Specify which products you need");
            Console.WriteLine("4. Discuss support requirements");
            Console.WriteLine("5. Negotiate contract terms");
            Console.WriteLine("6. Wait for custom quote");
            Console.WriteLine();
            Console.WriteLine("Pricing factors affecting your quote:");
            PrintPricingFactors();
        }

        private void PrintPricingFactors()
        {
            var factors = new Dictionary<string, string[]>
            {
                ["Products"] = new[]
                {
                    "SparkScan - consumer scanning UX",
                    "MatrixScan - multi-barcode scanning",
                    "ID Scanning - document verification",
                    "Parser - data parsing and validation",
                    "AR Overlays - augmented reality features"
                },
                ["Volume Metrics"] = new[]
                {
                    "Number of mobile devices",
                    "Number of scans per month",
                    "Number of active users",
                    "Geographic distribution"
                },
                ["Support Tiers"] = new[]
                {
                    "Standard - email support",
                    "Premium - priority support",
                    "Enterprise - SLA with uptime guarantees"
                },
                ["Contract Options"] = new[]
                {
                    "Annual billing",
                    "Multi-year commitment discount",
                    "Bundled product discount",
                    "Enterprise agreement terms"
                }
            };

            foreach (var category in factors)
            {
                Console.WriteLine($"\n{category.Key}:");
                foreach (var item in category.Value)
                {
                    Console.WriteLine($"  - {item}");
                }
            }
        }
    }

    // ============================================================
    // IRONBARCODE PRICING MODEL
    // ============================================================

    /// <summary>
    /// IronBarcode uses transparent perpetual licensing with
    /// published prices and no volume fees.
    /// </summary>
    public class IronBarcodePricingModel
    {
        /*
         * IronBarcode Published Pricing (2026):
         *
         * Lite License: $749 one-time
         * - 1 developer
         * - 1 project
         * - Unlimited barcodes
         *
         * Professional License: $1,499 one-time
         * - 10 developers
         * - 10 projects
         * - Unlimited barcodes
         *
         * Unlimited License: $2,999 one-time
         * - Unlimited developers
         * - Unlimited projects
         * - Unlimited barcodes
         *
         * Optional Annual Renewal:
         * - 50% of original price
         * - Includes updates and support
         * - Not required to continue using
         */

        public void PrintPricingTransparency()
        {
            Console.WriteLine("=== IronBarcode Pricing Transparency ===");
            Console.WriteLine();
            Console.WriteLine("Published prices, no sales call required:");
            Console.WriteLine();

            PrintLicenseTiers();

            Console.WriteLine();
            Console.WriteLine("Key Benefits:");
            Console.WriteLine("- No per-scan fees");
            Console.WriteLine("- No per-device fees");
            Console.WriteLine("- No volume thresholds");
            Console.WriteLine("- No surprise costs");
            Console.WriteLine("- Buy once, own forever");
        }

        private void PrintLicenseTiers()
        {
            var licenses = new[]
            {
                ("Lite", 749m, "1 developer, 1 project"),
                ("Professional", 1499m, "10 developers, 10 projects"),
                ("Unlimited", 2999m, "Unlimited developers, unlimited projects")
            };

            Console.WriteLine("| License       | Price (one-time) | Coverage                    |");
            Console.WriteLine("|--------------|------------------|----------------------------|");

            foreach (var (name, price, coverage) in licenses)
            {
                Console.WriteLine($"| {name,-12} | ${price,-15:N0} | {coverage,-26} |");
            }
        }
    }

    // ============================================================
    // COST COMPARISON SCENARIOS
    // ============================================================

    /// <summary>
    /// Demonstrates cost differences across various scenarios.
    /// </summary>
    public class CostComparisonScenarios
    {
        public void RunAllScenarios()
        {
            Console.WriteLine("=== Cost Comparison Scenarios ===");
            Console.WriteLine();

            SmallStartupScenario();
            Console.WriteLine();

            MidSizeCompanyScenario();
            Console.WriteLine();

            EnterpriseScenario();
            Console.WriteLine();

            FiveYearTcoComparison();
        }

        public void SmallStartupScenario()
        {
            Console.WriteLine("--- Scenario 1: Small Startup ---");
            Console.WriteLine("Requirements: 5 developers, mobile app + server processing");
            Console.WriteLine();
            Console.WriteLine("If use case is CAMERA SCANNING:");
            Console.WriteLine("  Scandit: Requires quote (typically $X,000+/year)");
            Console.WriteLine("  IronBarcode: Not appropriate for camera scanning");
            Console.WriteLine();
            Console.WriteLine("If use case is DOCUMENT PROCESSING:");
            Console.WriteLine("  Scandit: Over-engineered, expensive");
            Console.WriteLine("  IronBarcode: $1,499 one-time (Professional)");
        }

        public void MidSizeCompanyScenario()
        {
            Console.WriteLine("--- Scenario 2: Mid-Size Company ---");
            Console.WriteLine("Requirements: 50 mobile devices, 100K scans/month");
            Console.WriteLine();
            Console.WriteLine("Scandit (camera scanning):");
            Console.WriteLine("  - 50 device licenses");
            Console.WriteLine("  - MatrixScan for warehouse");
            Console.WriteLine("  - Premium support");
            Console.WriteLine("  - Estimated: $XX,000+/year (requires quote)");
            Console.WriteLine();
            Console.WriteLine("IronBarcode (document processing):");
            Console.WriteLine("  - Unlimited License: $2,999 one-time");
            Console.WriteLine("  - Process unlimited barcodes");
            Console.WriteLine("  - No volume fees");
        }

        public void EnterpriseScenario()
        {
            Console.WriteLine("--- Scenario 3: Enterprise ---");
            Console.WriteLine("Requirements: 500+ devices, global deployment");
            Console.WriteLine();
            Console.WriteLine("Scandit:");
            Console.WriteLine("  - Enterprise licensing agreement");
            Console.WriteLine("  - Custom pricing negotiation");
            Console.WriteLine("  - Multi-year contract terms");
            Console.WriteLine("  - Estimated: $XXX,000+/year (requires negotiation)");
            Console.WriteLine();
            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  - Unlimited License: $2,999 one-time");
            Console.WriteLine("  - Same price regardless of scale");
            Console.WriteLine("  - Optional renewal: $1,500/year");
        }

        public void FiveYearTcoComparison()
        {
            Console.WriteLine("--- 5-Year Total Cost of Ownership ---");
            Console.WriteLine();

            // IronBarcode calculation
            decimal ironBarcodeLicense = 2999m;
            decimal ironBarcodeRenewal = 1500m; // Optional
            decimal ironBarcode5YearWithRenewal = ironBarcodeLicense + (ironBarcodeRenewal * 4);
            decimal ironBarcode5YearNoRenewal = ironBarcodeLicense;

            Console.WriteLine("IronBarcode Unlimited License:");
            Console.WriteLine($"  One-time purchase: ${ironBarcodeLicense:N0}");
            Console.WriteLine($"  5-year with renewals: ${ironBarcode5YearWithRenewal:N0}");
            Console.WriteLine($"  5-year without renewals: ${ironBarcode5YearNoRenewal:N0}");
            Console.WriteLine();

            // Scandit is unknown without quote
            Console.WriteLine("Scandit (50-device deployment, estimated):");
            Console.WriteLine("  Year 1: $XX,000 (requires quote)");
            Console.WriteLine("  Year 2: $XX,000 (annual renewal)");
            Console.WriteLine("  Year 3: $XX,000 (annual renewal)");
            Console.WriteLine("  Year 4: $XX,000 (annual renewal)");
            Console.WriteLine("  Year 5: $XX,000 (annual renewal)");
            Console.WriteLine("  5-year total: $XXX,000+ (requires negotiation)");
            Console.WriteLine();
            Console.WriteLine("Note: Scandit pricing not public. Contact Scandit for quotes.");
        }
    }

    // ============================================================
    // BUDGET PLANNING IMPACT
    // ============================================================

    /// <summary>
    /// Demonstrates how pricing models affect budget planning.
    /// </summary>
    public class BudgetPlanningImpact
    {
        public void CompareBudgetApproaches()
        {
            Console.WriteLine("=== Budget Planning Impact ===");
            Console.WriteLine();

            Console.WriteLine("Scandit Budget Challenges:");
            Console.WriteLine("  - Cannot budget until you get a quote");
            Console.WriteLine("  - Costs may change with usage growth");
            Console.WriteLine("  - Annual renewals require re-approval");
            Console.WriteLine("  - Volume increases may trigger tier changes");
            Console.WriteLine("  - Procurement process for each renewal");
            Console.WriteLine();

            Console.WriteLine("IronBarcode Budget Certainty:");
            Console.WriteLine("  - Know exact cost before purchase");
            Console.WriteLine("  - No usage-based cost increases");
            Console.WriteLine("  - Perpetual license = one-time budget line");
            Console.WriteLine("  - Optional renewal is predictable");
            Console.WriteLine("  - No procurement cycles for renewals");
            Console.WriteLine();

            PrintBudgetExample();
        }

        private void PrintBudgetExample()
        {
            Console.WriteLine("Budget Planning Example:");
            Console.WriteLine();
            Console.WriteLine("Project: Document processing system");
            Console.WriteLine("Processing: 500,000 barcodes/month");
            Console.WriteLine();

            Console.WriteLine("With Scandit (if applicable to document processing):");
            Console.WriteLine("  Budget: 'TBD pending Scandit quote'");
            Console.WriteLine("  Risk: Quote may exceed budget");
            Console.WriteLine("  Timeline: Weeks for quote process");
            Console.WriteLine();

            Console.WriteLine("With IronBarcode:");
            Console.WriteLine("  Budget: $2,999 (Unlimited License)");
            Console.WriteLine("  Risk: None - price is fixed");
            Console.WriteLine("  Timeline: Purchase immediately");
        }
    }

    // ============================================================
    // DECISION HELPER
    // ============================================================

    public class PricingDecisionHelper
    {
        public static void PrintDecisionMatrix()
        {
            Console.WriteLine("=== Pricing Model Decision Matrix ===");
            Console.WriteLine();

            var comparisons = new[]
            {
                ("Public Pricing", "No (sales quote)", "Yes (published)"),
                ("Per-Scan Fees", "Possible", "No"),
                ("Per-Device Fees", "Yes", "No"),
                ("Volume Tiers", "Yes", "No"),
                ("Annual Renewal", "Required", "Optional"),
                ("Budget Certainty", "Low", "High"),
                ("Procurement Cycles", "Annual", "One-time"),
                ("Scale Cost Impact", "Increases cost", "No change")
            };

            Console.WriteLine("| Factor             | Scandit          | IronBarcode      |");
            Console.WriteLine("|--------------------|------------------|------------------|");

            foreach (var (factor, scandit, ironbarcode) in comparisons)
            {
                Console.WriteLine($"| {factor,-18} | {scandit,-16} | {ironbarcode,-16} |");
            }

            Console.WriteLine();
            Console.WriteLine("Key Takeaway:");
            Console.WriteLine("  If your use case fits IronBarcode (document processing),");
            Console.WriteLine("  the transparent pricing model provides significant");
            Console.WriteLine("  budget certainty and lower long-term costs.");
            Console.WriteLine();
            Console.WriteLine("  If your use case requires Scandit (camera scanning),");
            Console.WriteLine("  be prepared for sales-driven pricing negotiations");
            Console.WriteLine("  and ongoing budget variability.");
        }
    }

    // ============================================================
    // MAIN ENTRY POINT
    // ============================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Scandit vs IronBarcode Pricing Analysis");
            Console.WriteLine("========================================");
            Console.WriteLine();

            var scanditPricing = new ScanditPricingModel();
            scanditPricing.PrintPricingComplexity();

            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();

            var ironBarcodePricing = new IronBarcodePricingModel();
            ironBarcodePricing.PrintPricingTransparency();

            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();

            var scenarios = new CostComparisonScenarios();
            scenarios.RunAllScenarios();

            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
            Console.WriteLine();

            PricingDecisionHelper.PrintDecisionMatrix();
        }
    }
}
