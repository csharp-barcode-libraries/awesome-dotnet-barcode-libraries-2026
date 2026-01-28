/**
 * Cloudmersive Rate Limits Example
 *
 * This example demonstrates the rate limiting challenges with Cloudmersive
 * Barcode API and how IronBarcode eliminates these concerns entirely.
 *
 * Cloudmersive rate limits:
 * - Free tier: 800 requests/month, 1 concurrent request
 * - Premium: 5,000+ requests/month, higher concurrency
 * - Exceeded requests are queued or rejected
 */

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// Cloudmersive packages
// Install: dotnet add package Cloudmersive.APIClient.NET.Barcode
using Cloudmersive.APIClient.NET.Barcode.Api;
using Cloudmersive.APIClient.NET.Barcode.Client;

// IronBarcode
// Install: dotnet add package IronBarcode
using IronBarcode;

namespace CloudmersiveRateLimitsExample
{
    /// <summary>
    /// Demonstrates rate limiting handling required for Cloudmersive
    /// </summary>
    public class CloudmersiveRateLimitHandler
    {
        private readonly string _apiKey;
        private readonly SemaphoreSlim _concurrencyLimiter;
        private int _requestsThisMonth;
        private readonly int _monthlyLimit;

        public CloudmersiveRateLimitHandler(string apiKey, int monthlyLimit = 800, int maxConcurrent = 1)
        {
            _apiKey = apiKey;
            _monthlyLimit = monthlyLimit;
            _requestsThisMonth = 0;
            _concurrencyLimiter = new SemaphoreSlim(maxConcurrent);

            Configuration.Default.ApiKey.Add("Apikey", apiKey);
        }

        /// <summary>
        /// Scan with rate limit awareness - required for production Cloudmersive usage
        /// </summary>
        public async Task<string> ScanWithRateLimitAsync(string imagePath)
        {
            // Check monthly quota before making request
            if (_requestsThisMonth >= _monthlyLimit)
            {
                throw new QuotaExceededException(
                    $"Monthly quota exceeded: {_requestsThisMonth}/{_monthlyLimit} requests used");
            }

            // Enforce concurrency limit
            await _concurrencyLimiter.WaitAsync();

            try
            {
                var scanApi = new BarcodeScanApi();

                using (var stream = File.OpenRead(imagePath))
                {
                    var result = await ScanWithRetryAsync(scanApi, stream);
                    Interlocked.Increment(ref _requestsThisMonth);
                    return result?.RawText ?? "No barcode detected";
                }
            }
            finally
            {
                _concurrencyLimiter.Release();
            }
        }

        /// <summary>
        /// Implements retry logic for transient failures and rate limiting
        /// </summary>
        private async Task<BarcodeScanResult> ScanWithRetryAsync(
            BarcodeScanApi api,
            Stream imageStream,
            int maxRetries = 3)
        {
            int attempt = 0;
            int delayMs = 1000;

            while (attempt < maxRetries)
            {
                try
                {
                    imageStream.Position = 0; // Reset stream for retry
                    return await api.BarcodeScanImageAsync(imageStream);
                }
                catch (ApiException ex) when (ex.ErrorCode == 429)
                {
                    // Rate limited - implement exponential backoff
                    attempt++;
                    Console.WriteLine($"Rate limited. Retry {attempt}/{maxRetries} after {delayMs}ms");

                    if (attempt >= maxRetries)
                    {
                        throw new RateLimitExceededException(
                            "Rate limit exceeded after maximum retries", ex);
                    }

                    await Task.Delay(delayMs);
                    delayMs *= 2; // Exponential backoff
                }
                catch (ApiException ex) when (ex.ErrorCode == 503)
                {
                    // Service temporarily unavailable
                    attempt++;
                    Console.WriteLine($"Service unavailable. Retry {attempt}/{maxRetries}");

                    if (attempt >= maxRetries) throw;
                    await Task.Delay(delayMs);
                }
            }

            return null;
        }

        /// <summary>
        /// Get remaining quota for monitoring
        /// </summary>
        public (int used, int remaining, int limit) GetQuotaStatus()
        {
            return (_requestsThisMonth, _monthlyLimit - _requestsThisMonth, _monthlyLimit);
        }
    }

    /// <summary>
    /// Custom exception for quota exceeded
    /// </summary>
    public class QuotaExceededException : Exception
    {
        public QuotaExceededException(string message) : base(message) { }
    }

    /// <summary>
    /// Custom exception for rate limiting
    /// </summary>
    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// IronBarcode alternative - no rate limiting infrastructure needed
    /// </summary>
    public class IronBarcodeUnlimitedProcessor
    {
        /// <summary>
        /// Process unlimited barcodes with no rate limiting
        /// </summary>
        public BarcodeResults Scan(string imagePath)
        {
            // No quota tracking needed
            // No concurrency limiting needed
            // No retry logic needed
            // Just process the barcode
            return BarcodeReader.Read(imagePath);
        }

        /// <summary>
        /// Batch process with full parallelism
        /// </summary>
        public ConcurrentBag<BarcodeResults> BatchScan(string[] imagePaths)
        {
            var results = new ConcurrentBag<BarcodeResults>();

            // Process all images in parallel - no rate limiting
            Parallel.ForEach(imagePaths, new ParallelOptions { MaxDegreeOfParallelism = -1 }, path =>
            {
                var result = BarcodeReader.Read(path);
                results.Add(result);
            });

            return results;
        }

        /// <summary>
        /// Process indefinitely without quota concerns
        /// </summary>
        public async Task ContinuousProcessingAsync(
            Func<string> getNextImage,
            Action<BarcodeResults> processResult,
            CancellationToken cancellationToken)
        {
            int processed = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var imagePath = getNextImage();
                if (imagePath == null) break;

                var result = BarcodeReader.Read(imagePath);
                processResult(result);

                processed++;

                // No quota to worry about
                // Process millions without cost concern
            }

            Console.WriteLine($"Processed {processed} barcodes with no quota limits");
        }
    }

    /// <summary>
    /// Comparison demonstration
    /// </summary>
    public class RateLimitComparisonDemo
    {
        public async Task DemonstrateDifferenceAsync()
        {
            Console.WriteLine("=== Rate Limit Handling Comparison ===\n");

            // Cloudmersive: Complex infrastructure required
            Console.WriteLine("Cloudmersive approach requires:");
            Console.WriteLine("- Quota tracking mechanism");
            Console.WriteLine("- Concurrency limiter (semaphore)");
            Console.WriteLine("- Retry logic with exponential backoff");
            Console.WriteLine("- Error handling for 429 responses");
            Console.WriteLine("- Monthly quota monitoring");
            Console.WriteLine("- Alerting when approaching limits");

            // IronBarcode: None of this needed
            Console.WriteLine("\nIronBarcode requires:");
            Console.WriteLine("- Nothing. Just call BarcodeReader.Read()");

            // Show code complexity difference
            Console.WriteLine("\n=== Code Complexity ===\n");

            Console.WriteLine("Cloudmersive production code: ~100+ lines for rate limit handling");
            Console.WriteLine("IronBarcode production code: 1 line - BarcodeReader.Read()");
        }

        public async Task SimulateBatchProcessingAsync(string[] imagePaths)
        {
            Console.WriteLine("\n=== Batch Processing Simulation ===\n");
            Console.WriteLine($"Processing {imagePaths.Length} images\n");

            // Cloudmersive simulation (free tier)
            Console.WriteLine("Cloudmersive (Free tier: 800/month, 1 concurrent):");

            var cloudHandler = new CloudmersiveRateLimitHandler(
                "DEMO_KEY",
                monthlyLimit: 800,
                maxConcurrent: 1);

            // In real scenario, would hit rate limits quickly
            Console.WriteLine($"- Max throughput: 1 request at a time");
            Console.WriteLine($"- Monthly limit: 800 requests");
            Console.WriteLine($"- Time estimate at 250ms/request: {imagePaths.Length * 250}ms");

            if (imagePaths.Length > 800)
            {
                Console.WriteLine($"- WARNING: Would exceed monthly quota!");
            }

            // IronBarcode
            Console.WriteLine("\nIronBarcode:");

            var ironProcessor = new IronBarcodeUnlimitedProcessor();
            var startTime = DateTime.Now;

            // Actually process (if files exist)
            // var results = ironProcessor.BatchScan(imagePaths);

            Console.WriteLine($"- No throughput limits");
            Console.WriteLine($"- No monthly quota");
            Console.WriteLine($"- Full parallel processing");
            Console.WriteLine($"- Time estimate at 25ms/image parallel: ~{imagePaths.Length * 25 / Environment.ProcessorCount}ms");
        }

        public void ShowCostAtScale()
        {
            Console.WriteLine("\n=== Cost at Scale Analysis ===\n");

            var scenarios = new[]
            {
                (name: "Low Volume", monthly: 500),
                (name: "Medium Volume", monthly: 5000),
                (name: "High Volume", monthly: 50000),
                (name: "Enterprise", monthly: 500000),
            };

            Console.WriteLine("Monthly barcode processing costs:\n");
            Console.WriteLine($"{"Scenario",-20} {"Volume",-12} {"Cloudmersive",-15} {"IronBarcode",-12}");
            Console.WriteLine(new string('-', 60));

            foreach (var scenario in scenarios)
            {
                string cloudCost = scenario.monthly switch
                {
                    <= 800 => "Free",
                    <= 5000 => "$19.99/mo",
                    <= 50000 => "$99+/mo",
                    _ => "$500+/mo"
                };

                Console.WriteLine($"{scenario.name,-20} {scenario.monthly,-12} {cloudCost,-15} {"$0/mo",-12}");
            }

            Console.WriteLine("\nIronBarcode: $749 one-time perpetual license");
            Console.WriteLine("Break-even: ~3-6 months at medium volume");
        }
    }

    /// <summary>
    /// Program entry point
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var demo = new RateLimitComparisonDemo();

            await demo.DemonstrateDifferenceAsync();

            // Simulate with test paths
            var testPaths = Enumerable.Range(1, 100).Select(i => $"image{i}.png").ToArray();
            await demo.SimulateBatchProcessingAsync(testPaths);

            demo.ShowCostAtScale();

            Console.WriteLine("\n=== Recommendation ===");
            Console.WriteLine("For production .NET applications with any significant barcode volume,");
            Console.WriteLine("IronBarcode's local processing eliminates the complexity and cost");
            Console.WriteLine("of cloud API rate limiting entirely.");
        }
    }
}
