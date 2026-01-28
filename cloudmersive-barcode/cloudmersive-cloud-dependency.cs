/**
 * Cloudmersive Cloud Dependency Example
 *
 * This example demonstrates the cloud API architecture of Cloudmersive Barcode
 * and contrasts it with IronBarcode's local processing approach.
 *
 * Key differences:
 * - Cloudmersive: Every operation requires internet connection and API key
 * - IronBarcode: Processes locally with no network dependency
 */

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

// Cloudmersive packages (cloud API client)
// Install: dotnet add package Cloudmersive.APIClient.NET.Barcode
using Cloudmersive.APIClient.NET.Barcode.Api;
using Cloudmersive.APIClient.NET.Barcode.Client;
using Cloudmersive.APIClient.NET.Barcode.Model;

// IronBarcode (local processing)
// Install: dotnet add package IronBarcode
using IronBarcode;

namespace CloudmersiveCloudDependencyExample
{
    /// <summary>
    /// Demonstrates Cloudmersive's cloud API architecture and requirements
    /// compared to IronBarcode's local processing model.
    /// </summary>
    public class CloudDependencyComparison
    {
        // Cloudmersive requires API key configuration
        private const string CloudmersiveApiKey = "YOUR_CLOUDMERSIVE_API_KEY";

        /// <summary>
        /// Cloudmersive setup - API key required for all operations
        /// </summary>
        public void SetupCloudmersive()
        {
            // API key must be configured before any operation
            // This key is sent with every request to Cloudmersive servers
            Configuration.Default.ApiKey.Add("Apikey", CloudmersiveApiKey);

            // Optional: Configure base URL (default is Cloudmersive cloud)
            // Configuration.Default.BasePath = "https://api.cloudmersive.com";

            // Optional: Configure timeout (default may be insufficient for large images)
            Configuration.Default.Timeout = 30000; // 30 seconds

            Console.WriteLine("Cloudmersive configured - requires internet for all operations");
        }

        /// <summary>
        /// IronBarcode setup - minimal configuration, works offline
        /// </summary>
        public void SetupIronBarcode()
        {
            // Optional: Configure license for production (removes watermark)
            // IronBarcode.License.LicenseKey = "YOUR_LICENSE_KEY";

            // No API keys, no network configuration, no external dependencies
            Console.WriteLine("IronBarcode ready - processes locally, works offline");
        }

        /// <summary>
        /// Cloudmersive barcode scanning - every call is a network request
        /// </summary>
        public async Task<string> ScanWithCloudmersiveAsync(string imagePath)
        {
            // REQUIREMENT: Internet connection must be available
            // REQUIREMENT: API key must be valid and have quota remaining

            var scanApi = new BarcodeScanApi();

            try
            {
                using (var stream = File.OpenRead(imagePath))
                {
                    // This call sends the entire image to Cloudmersive servers
                    // Network latency: typically 150-400ms per request
                    var result = await scanApi.BarcodeScanImageAsync(stream);

                    if (result.Successful == true)
                    {
                        return result.RawText;
                    }
                    else
                    {
                        return "Scan failed - no barcode detected";
                    }
                }
            }
            catch (ApiException ex)
            {
                // Handle API-specific errors
                if (ex.ErrorCode == 401)
                {
                    return "Error: Invalid API key";
                }
                else if (ex.ErrorCode == 429)
                {
                    return "Error: Rate limit exceeded - request queued or rejected";
                }
                else
                {
                    return $"API Error: {ex.Message}";
                }
            }
            catch (HttpRequestException ex)
            {
                // Network connectivity issues
                return $"Network Error: {ex.Message} - Internet connection required";
            }
        }

        /// <summary>
        /// IronBarcode scanning - local processing, no network needed
        /// </summary>
        public string ScanWithIronBarcode(string imagePath)
        {
            // No internet required - processes entirely on local machine
            // No API quotas - unlimited processing
            // No network latency - millisecond response times

            try
            {
                var results = BarcodeReader.Read(imagePath);

                if (results.Count > 0)
                {
                    return results.First().Value;
                }
                else
                {
                    return "No barcode detected";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Cloudmersive barcode generation - network call required
        /// </summary>
        public async Task<byte[]> GenerateWithCloudmersiveAsync(string data, string format)
        {
            var generateApi = new GenerateBarcodeApi();

            try
            {
                // Each generation is a separate API call
                // Different methods for different barcode types
                byte[] result;

                switch (format.ToUpper())
                {
                    case "QR":
                        result = await generateApi.GenerateBarcodeQRCodeAsync(data);
                        break;
                    case "EAN13":
                        result = await generateApi.GenerateBarcodeEAN13Async(data);
                        break;
                    case "CODE128":
                        // Note: Cloudmersive has separate methods for different formats
                        result = await generateApi.GenerateBarcodeUPCAAsync(data);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported format: {format}");
                }

                return result;
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// IronBarcode generation - local, instant, offline
        /// </summary>
        public byte[] GenerateWithIronBarcode(string data, string format)
        {
            // All processing happens locally
            // Same unified API for all barcode types

            BarcodeEncoding encoding = format.ToUpper() switch
            {
                "QR" => BarcodeEncoding.QRCode,
                "EAN13" => BarcodeEncoding.EAN13,
                "CODE128" => BarcodeEncoding.Code128,
                _ => BarcodeEncoding.Code128
            };

            var barcode = BarcodeWriter.CreateBarcode(data, encoding);
            return barcode.ToPngBinaryData();
        }

        /// <summary>
        /// Demonstrates network dependency in batch processing
        /// </summary>
        public async Task BatchProcessingComparisonAsync(string[] imagePaths)
        {
            Console.WriteLine("\n=== Batch Processing Comparison ===\n");

            // Cloudmersive: Sequential due to rate limits, network overhead
            Console.WriteLine("Cloudmersive batch processing:");
            Console.WriteLine("- Each image requires network round-trip (150-400ms)");
            Console.WriteLine("- Free tier: 1 concurrent request max");
            Console.WriteLine("- Monthly quota consumed per image");

            var cloudmersiveStart = DateTime.Now;
            var scanApi = new BarcodeScanApi();

            foreach (var path in imagePaths)
            {
                try
                {
                    using (var stream = File.OpenRead(path))
                    {
                        // Each call is a separate network request
                        await scanApi.BarcodeScanImageAsync(stream);
                    }
                }
                catch
                {
                    // Handle errors
                }
            }

            var cloudmersiveTime = DateTime.Now - cloudmersiveStart;
            Console.WriteLine($"Cloudmersive time: {cloudmersiveTime.TotalMilliseconds}ms");
            Console.WriteLine($"Estimated network overhead: {imagePaths.Length * 250}ms minimum");

            // IronBarcode: Parallel processing, no network overhead
            Console.WriteLine("\nIronBarcode batch processing:");
            Console.WriteLine("- All processing local (10-50ms per image typical)");
            Console.WriteLine("- Unlimited parallel processing");
            Console.WriteLine("- No quota concerns");

            var ironBarcodeStart = DateTime.Now;

            // Can process in parallel with no rate limiting
            Parallel.ForEach(imagePaths, path =>
            {
                BarcodeReader.Read(path);
            });

            var ironBarcodeTime = DateTime.Now - ironBarcodeStart;
            Console.WriteLine($"IronBarcode time: {ironBarcodeTime.TotalMilliseconds}ms");
        }

        /// <summary>
        /// Demonstrates offline scenario handling
        /// </summary>
        public void OfflineScenarioDemo(string imagePath)
        {
            Console.WriteLine("\n=== Offline Scenario ===\n");

            // Simulate network unavailable
            Console.WriteLine("Scenario: Internet connection lost");

            // Cloudmersive: Cannot process
            Console.WriteLine("\nCloudmersive behavior:");
            Console.WriteLine("- All barcode operations fail");
            Console.WriteLine("- HttpRequestException thrown");
            Console.WriteLine("- Application must queue work for later");

            // IronBarcode: Works normally
            Console.WriteLine("\nIronBarcode behavior:");
            Console.WriteLine("- Processing continues unaffected");
            Console.WriteLine("- No dependency on external services");

            var result = BarcodeReader.Read(imagePath);
            Console.WriteLine($"- Successfully processed: {result.Count} barcodes found");
        }

        /// <summary>
        /// Data privacy comparison
        /// </summary>
        public void DataPrivacyDemo()
        {
            Console.WriteLine("\n=== Data Privacy Comparison ===\n");

            Console.WriteLine("Cloudmersive data flow:");
            Console.WriteLine("1. Your barcode image data is sent to Cloudmersive servers");
            Console.WriteLine("2. Processing happens on external infrastructure");
            Console.WriteLine("3. Results returned over network");
            Console.WriteLine("4. Consider compliance: HIPAA, GDPR, PCI DSS implications");

            Console.WriteLine("\nIronBarcode data flow:");
            Console.WriteLine("1. All processing happens on your infrastructure");
            Console.WriteLine("2. No data leaves your network");
            Console.WriteLine("3. Full control over data handling");
            Console.WriteLine("4. Simplified compliance - data never transmitted externally");
        }
    }

    /// <summary>
    /// Example entry point demonstrating cloud dependency issues
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var demo = new CloudDependencyComparison();

            // Setup comparison
            demo.SetupCloudmersive();
            demo.SetupIronBarcode();

            // If you have test images, uncomment:
            // await demo.BatchProcessingComparisonAsync(new[] { "test1.png", "test2.png" });
            // demo.OfflineScenarioDemo("test.png");

            demo.DataPrivacyDemo();

            Console.WriteLine("\n=== Summary ===");
            Console.WriteLine("Cloudmersive: Cloud API - requires internet, has quotas, per-request costs");
            Console.WriteLine("IronBarcode: Local SDK - works offline, unlimited processing, one-time license");
        }
    }
}
