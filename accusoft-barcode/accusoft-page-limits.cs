/**
 * Accusoft BarcodeXpress vs IronBarcode: Page Processing Limits
 *
 * This file demonstrates the Standard Edition 40 pages-per-minute limit
 * in BarcodeXpress and how IronBarcode has no such processing restrictions.
 *
 * Key takeaway: BarcodeXpress Standard Edition throttles processing to
 * 40 pages per minute. Professional Edition removes this limit but costs more.
 * IronBarcode has no processing limits at any license tier.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

// ============================================================================
// PART 1: ACCUSOFT BARCODEXPRESS PAGE LIMIT HANDLING
// ============================================================================

namespace AccusoftBarcodeXpressExample
{
    // Install: dotnet add package Accusoft.BarcodeXpress.NetCore
    using Accusoft.BarcodeXpressSdk;

    /// <summary>
    /// BarcodeXpress Standard Edition has a 40 pages-per-minute limit.
    /// This class demonstrates the impact and workaround strategies.
    /// </summary>
    public class AccusoftPageLimitHandler
    {
        private readonly BarcodeXpress _barcodeXpress;
        private readonly int _pagesPerMinuteLimit = 40; // Standard Edition limit
        private int _pagesProcessedThisMinute = 0;
        private DateTime _minuteWindowStart;

        public AccusoftPageLimitHandler()
        {
            _barcodeXpress = new BarcodeXpress();
            _barcodeXpress.Licensing.SolutionName = "YourSolutionName";
            _barcodeXpress.Licensing.SolutionKey = Convert.ToInt64("YourSolutionKey");
            _minuteWindowStart = DateTime.Now;
        }

        /// <summary>
        /// Process a batch of images with rate limiting awareness.
        /// Standard Edition will throttle after 40 pages/minute.
        /// </summary>
        public async Task<Dictionary<string, string[]>> ProcessBatchWithRateLimit(
            string[] imagePaths)
        {
            var results = new Dictionary<string, string[]>();
            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine($"Processing {imagePaths.Length} images...");
            Console.WriteLine($"Standard Edition limit: {_pagesPerMinuteLimit} pages/minute");
            Console.WriteLine();

            foreach (var path in imagePaths)
            {
                // Check if we need to wait for rate limit reset
                await EnforceRateLimitAsync();

                // Process the image
                _barcodeXpress.reader.SetPropertyValue(
                    BarcodeXpress.cycBxeSetFilename, path);
                var barcodes = _barcodeXpress.reader.Analyze();

                results[path] = barcodes.Select(r => r.BarcodeValue).ToArray();
                _pagesProcessedThisMinute++;

                // Log progress
                if (_pagesProcessedThisMinute % 10 == 0)
                {
                    Console.WriteLine($"Processed: {results.Count}/{imagePaths.Length}");
                }
            }

            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"Total processing time: {stopwatch.Elapsed.TotalSeconds:F1} seconds");
            Console.WriteLine($"Effective rate: {imagePaths.Length / stopwatch.Elapsed.TotalMinutes:F1} pages/minute");

            return results;
        }

        /// <summary>
        /// Enforce the 40 PPM rate limit by waiting if necessary
        /// </summary>
        private async Task EnforceRateLimitAsync()
        {
            if (_pagesProcessedThisMinute >= _pagesPerMinuteLimit)
            {
                var elapsed = DateTime.Now - _minuteWindowStart;

                if (elapsed.TotalSeconds < 60)
                {
                    var waitTime = TimeSpan.FromSeconds(60 - elapsed.TotalSeconds);
                    Console.WriteLine($"Rate limit reached. Waiting {waitTime.TotalSeconds:F1}s...");
                    await Task.Delay(waitTime);
                }

                // Reset the window
                _pagesProcessedThisMinute = 0;
                _minuteWindowStart = DateTime.Now;
            }
        }

        /// <summary>
        /// Demonstrates the practical impact of the 40 PPM limit
        /// </summary>
        public void ExplainRateLimitImpact()
        {
            Console.WriteLine("=== STANDARD EDITION RATE LIMIT IMPACT ===");
            Console.WriteLine();
            Console.WriteLine("40 pages/minute = 2,400 pages/hour = 57,600 pages/day");
            Console.WriteLine();
            Console.WriteLine("Processing time for different batch sizes:");
            Console.WriteLine();

            var batchSizes = new[] { 40, 100, 500, 1000, 5000 };

            foreach (var size in batchSizes)
            {
                // Calculate minimum time with rate limit
                double minutesNeeded = Math.Ceiling((double)size / _pagesPerMinuteLimit);
                double secondsPerPage = 60.0 / _pagesPerMinuteLimit;

                // Estimate time without rate limit (assuming ~50ms per page)
                double unlimitedSeconds = size * 0.05;

                Console.WriteLine($"  {size,5} pages:");
                Console.WriteLine($"    Standard Edition: {minutesNeeded:F0} minutes minimum");
                Console.WriteLine($"    Without limit:    {unlimitedSeconds:F1} seconds");
                Console.WriteLine($"    Slowdown factor:  {(minutesNeeded * 60) / unlimitedSeconds:F1}x");
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// Edition comparison showing the difference between Standard and Professional
    /// </summary>
    public class AccusoftEditionComparison
    {
        public void CompareEditions()
        {
            Console.WriteLine("=== BARCODEXPRESS EDITION COMPARISON ===");
            Console.WriteLine();
            Console.WriteLine("STANDARD EDITION:");
            Console.WriteLine("  Price: $1,960+ per developer");
            Console.WriteLine("  Processing limit: 40 pages/minute");
            Console.WriteLine("  Runtime licenses: Required (min 5)");
            Console.WriteLine("  Use case: Low-volume processing");
            Console.WriteLine();
            Console.WriteLine("PROFESSIONAL EDITION:");
            Console.WriteLine("  Price: $3,500+ per developer");
            Console.WriteLine("  Processing limit: None");
            Console.WriteLine("  Runtime licenses: Required (min 5)");
            Console.WriteLine("  Use case: High-volume processing");
            Console.WriteLine();
            Console.WriteLine("UPGRADE COST:");
            Console.WriteLine("  5-developer team Standard -> Professional:");
            Console.WriteLine("  Additional cost: ~$7,700");
            Console.WriteLine();
            Console.WriteLine("IRONBARCODE (COMPARISON):");
            Console.WriteLine("  Price: $749 one-time (Lite) or $2,999 (10-dev)");
            Console.WriteLine("  Processing limit: None at any tier");
            Console.WriteLine("  Runtime licenses: Not required");
        }

        /// <summary>
        /// Calculate when Professional Edition upgrade pays off
        /// </summary>
        public void CalculateUpgradeBreakEven(int dailyPageVolume)
        {
            int standardPPM = 40;
            double hoursPerDay = 8;

            // Standard Edition throughput
            double standardPagesPerDay = standardPPM * 60 * hoursPerDay;

            // Check if Standard Edition can handle the volume
            if (dailyPageVolume <= standardPagesPerDay)
            {
                Console.WriteLine($"Daily volume: {dailyPageVolume} pages");
                Console.WriteLine($"Standard Edition capacity: {standardPagesPerDay:N0} pages/day");
                Console.WriteLine("Standard Edition can handle this volume.");
                Console.WriteLine();
                Console.WriteLine("However, processing will take longer than necessary.");
                Console.WriteLine($"Time with limit: {dailyPageVolume / (standardPPM * 60.0):F1} hours");
                Console.WriteLine($"Time without limit: {dailyPageVolume * 0.05 / 3600:F2} hours (estimated)");
            }
            else
            {
                Console.WriteLine($"Daily volume: {dailyPageVolume:N0} pages");
                Console.WriteLine($"Standard Edition capacity: {standardPagesPerDay:N0} pages/day");
                Console.WriteLine("Standard Edition CANNOT handle this volume.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("1. Upgrade to Professional Edition ($3,500+/dev)");
                Console.WriteLine("2. Switch to IronBarcode ($749 one-time, no limits)");
            }
        }
    }

    /// <summary>
    /// Real-world scenario demonstrating rate limit problems
    /// </summary>
    public class AccusoftRealWorldScenario
    {
        /// <summary>
        /// Invoice processing scenario - common enterprise use case
        /// </summary>
        public void InvoiceProcessingScenario()
        {
            Console.WriteLine("=== INVOICE PROCESSING SCENARIO ===");
            Console.WriteLine();
            Console.WriteLine("Business requirement:");
            Console.WriteLine("- Process 500 invoices per day");
            Console.WriteLine("- Each invoice has 1 barcode");
            Console.WriteLine("- Processing window: 8am - 6pm (10 hours)");
            Console.WriteLine();

            int invoicesPerDay = 500;
            int standardPPM = 40;

            double minutesNeeded = Math.Ceiling((double)invoicesPerDay / standardPPM);
            double hoursNeeded = minutesNeeded / 60;

            Console.WriteLine("With BarcodeXpress Standard Edition:");
            Console.WriteLine($"  Processing time: {minutesNeeded:F0} minutes ({hoursNeeded:F1} hours)");
            Console.WriteLine($"  Can finish within window: {(hoursNeeded <= 10 ? "Yes" : "No")}");
            Console.WriteLine();

            // Without rate limit
            double unlimitedMinutes = invoicesPerDay * 0.05 / 60; // ~50ms per invoice
            Console.WriteLine("Without rate limit (IronBarcode):");
            Console.WriteLine($"  Processing time: {unlimitedMinutes:F1} minutes");
            Console.WriteLine($"  Time saved: {minutesNeeded - unlimitedMinutes:F1} minutes");
        }

        /// <summary>
        /// Warehouse scanning scenario - high throughput required
        /// </summary>
        public void WarehouseScanningScenario()
        {
            Console.WriteLine("=== WAREHOUSE SCANNING SCENARIO ===");
            Console.WriteLine();
            Console.WriteLine("Business requirement:");
            Console.WriteLine("- Scan packages as they arrive");
            Console.WriteLine("- Peak rate: 200 packages per hour");
            Console.WriteLine("- Real-time processing required");
            Console.WriteLine();

            int packagesPerHour = 200;
            int standardPPM = 40;
            int standardPPH = standardPPM * 60;

            bool canHandlePeak = packagesPerHour <= standardPPH;

            Console.WriteLine("With BarcodeXpress Standard Edition:");
            Console.WriteLine($"  Maximum throughput: {standardPPH} packages/hour");
            Console.WriteLine($"  Required throughput: {packagesPerHour} packages/hour");
            Console.WriteLine($"  Can handle peak: {(canHandlePeak ? "Yes" : "No")}");

            if (canHandlePeak)
            {
                Console.WriteLine();
                Console.WriteLine("  However, during peak periods:");
                double utilizationPercent = (double)packagesPerHour / standardPPH * 100;
                Console.WriteLine($"  Rate limit utilization: {utilizationPercent:F1}%");
                Console.WriteLine("  Any burst above average will cause delays");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("  Standard Edition cannot meet this requirement.");
                Console.WriteLine("  Options:");
                Console.WriteLine("    1. Professional Edition (unlimited speed)");
                Console.WriteLine("    2. IronBarcode (no limits at any tier)");
            }
        }
    }
}


// ============================================================================
// PART 2: IRONBARCODE - NO PROCESSING LIMITS
// ============================================================================

namespace IronBarcodeExample
{
    // Install: dotnet add package IronBarcode
    using IronBarCode;

    /// <summary>
    /// IronBarcode has no pages-per-minute limits at any license tier.
    /// Process as fast as your hardware allows.
    /// </summary>
    public class IronBarcodeUnlimitedProcessing
    {
        /// <summary>
        /// Process a batch of images at full speed - no rate limiting
        /// </summary>
        public Dictionary<string, string[]> ProcessBatchAtFullSpeed(string[] imagePaths)
        {
            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine($"Processing {imagePaths.Length} images...");
            Console.WriteLine("No processing limits - full speed ahead");
            Console.WriteLine();

            // Process all images - IronBarcode handles parallelization internally
            var allResults = BarcodeReader.Read(imagePaths);

            stopwatch.Stop();

            var results = allResults.GroupBy(r => r.InputPath ?? "")
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.Text).ToArray());

            Console.WriteLine($"Total processing time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            Console.WriteLine($"Effective rate: {imagePaths.Length / stopwatch.Elapsed.TotalMinutes:F0} pages/minute");
            Console.WriteLine();
            Console.WriteLine("No artificial limits. No waiting. Just results.");

            return results;
        }

        /// <summary>
        /// PDF batch processing - no page limits apply
        /// </summary>
        public void ProcessPdfBatch(string[] pdfPaths)
        {
            Console.WriteLine("PDF Batch Processing with IronBarcode");
            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();
            int totalPages = 0;

            foreach (var pdfPath in pdfPaths)
            {
                var results = BarcodeReader.Read(pdfPath);

                foreach (var result in results)
                {
                    Console.WriteLine($"  {pdfPath} (page {result.PageNumber}): {result.Text}");
                    totalPages++;
                }
            }

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Processed {totalPages} pages in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            Console.WriteLine($"Rate: {totalPages / stopwatch.Elapsed.TotalMinutes:F0} pages/minute");
            Console.WriteLine();
            Console.WriteLine("No 40 PPM limit. No Professional Edition required.");
        }

        /// <summary>
        /// High-throughput scenario - process thousands of barcodes
        /// </summary>
        public async Task HighThroughputDemo(string[] imagePaths)
        {
            Console.WriteLine("=== HIGH THROUGHPUT DEMONSTRATION ===");
            Console.WriteLine();
            Console.WriteLine($"Processing {imagePaths.Length} images");
            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();

            // Process in parallel batches for maximum throughput
            var tasks = imagePaths.Select(path => Task.Run(() =>
            {
                var results = BarcodeReader.Read(path);
                return new { Path = path, Results = results };
            }));

            var allResults = await Task.WhenAll(tasks);

            stopwatch.Stop();

            int totalBarcodes = allResults.Sum(r => r.Results.Count());
            double pagesPerMinute = imagePaths.Length / stopwatch.Elapsed.TotalMinutes;

            Console.WriteLine($"Results:");
            Console.WriteLine($"  Images processed: {imagePaths.Length}");
            Console.WriteLine($"  Barcodes found: {totalBarcodes}");
            Console.WriteLine($"  Total time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            Console.WriteLine($"  Rate: {pagesPerMinute:F0} pages/minute");
            Console.WriteLine();

            // Compare to BarcodeXpress Standard
            double barcodeXpressMinutes = Math.Ceiling(imagePaths.Length / 40.0);
            Console.WriteLine("Comparison to BarcodeXpress Standard Edition:");
            Console.WriteLine($"  BarcodeXpress time: {barcodeXpressMinutes:F0} minutes minimum");
            Console.WriteLine($"  IronBarcode time: {stopwatch.Elapsed.TotalMinutes:F2} minutes");
            Console.WriteLine($"  Speed advantage: {barcodeXpressMinutes / stopwatch.Elapsed.TotalMinutes:F1}x faster");
        }
    }

    /// <summary>
    /// Pricing comparison with processing limits in mind
    /// </summary>
    public class ProcessingLimitPricingComparison
    {
        public void CompareOptions()
        {
            Console.WriteLine("=== PROCESSING LIMITS + PRICING COMPARISON ===");
            Console.WriteLine();
            Console.WriteLine("ACCUSOFT BARCODEXPRESS STANDARD:");
            Console.WriteLine("  SDK Price: $1,960+ per developer");
            Console.WriteLine("  Runtime: +$2,500+ (min 5 licenses)");
            Console.WriteLine("  Processing limit: 40 pages/minute");
            Console.WriteLine("  Total (1 dev, 1 server): $4,460+");
            Console.WriteLine();
            Console.WriteLine("ACCUSOFT BARCODEXPRESS PROFESSIONAL:");
            Console.WriteLine("  SDK Price: $3,500+ per developer");
            Console.WriteLine("  Runtime: +$2,500+ (min 5 licenses)");
            Console.WriteLine("  Processing limit: None");
            Console.WriteLine("  Total (1 dev, 1 server): $6,000+");
            Console.WriteLine();
            Console.WriteLine("IRONBARCODE:");
            Console.WriteLine("  License: $749 one-time (Lite)");
            Console.WriteLine("  License: $2,999 one-time (Professional, 10 devs)");
            Console.WriteLine("  Runtime: Not required");
            Console.WriteLine("  Processing limit: None at any tier");
            Console.WriteLine();
            Console.WriteLine("RECOMMENDATION:");
            Console.WriteLine("  If you need unlimited processing speed:");
            Console.WriteLine("    BarcodeXpress Professional: $6,000+ upfront");
            Console.WriteLine("    IronBarcode: $749 upfront");
            Console.WriteLine("    Savings: $5,251+");
        }
    }
}
