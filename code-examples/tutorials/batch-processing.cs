// Install-Package IronBarcode
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using IronBarCode;

namespace BatchProcessing
{
    /// <summary>
    /// Demonstrates three approaches to batch processing barcodes:
    /// 1. Sequential processing (baseline)
    /// 2. IronBarcode Multithreaded option (simple parallelization)
    /// 3. PLINQ with custom thread control (advanced)
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Prepare test data
            var imageFiles = Directory.GetFiles("./test-barcodes/", "*.png");
            Console.WriteLine($"Processing {imageFiles.Length} barcode images");
            Console.WriteLine($"CPU Cores: {Environment.ProcessorCount}\n");

            // Approach 1: Sequential (baseline)
            SequentialProcessing(imageFiles);

            // Approach 2: IronBarcode Multithreaded option
            MultithreadedProcessing(imageFiles);

            // Approach 3: Advanced PLINQ pattern
            PLINQProcessing(imageFiles);

            // Approach 4: Directory scanning example
            DirectoryScanExample();
        }

        /// <summary>
        /// Sequential processing - one file at a time
        /// Establishes baseline performance for comparison
        /// </summary>
        static void SequentialProcessing(string[] files)
        {
            Console.WriteLine("=== Sequential Processing ===");
            var stopwatch = Stopwatch.StartNew();
            var results = new List<BarcodeResults>();

            foreach (var file in files)
            {
                var result = BarcodeReader.Read(file);
                results.Add(result);
            }

            stopwatch.Stop();
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Barcodes found: {results.Count(r => r.Any())}");
            Console.WriteLine($"Average per file: {stopwatch.ElapsedMilliseconds / (double)files.Length:F2}ms\n");
        }

        /// <summary>
        /// IronBarcode's built-in Multithreaded option
        /// Single flag enables automatic parallelization
        /// </summary>
        static void MultithreadedProcessing(string[] files)
        {
            Console.WriteLine("=== IronBarcode Multithreaded Option ===");
            var stopwatch = Stopwatch.StartNew();
            var results = new List<BarcodeResults>();

            // Single flag enables automatic parallel processing
            var options = new BarcodeReaderOptions
            {
                Multithreaded = true
            };

            foreach (var file in files)
            {
                var result = BarcodeReader.Read(file, options);
                results.Add(result);
            }

            stopwatch.Stop();
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Barcodes found: {results.Count(r => r.Any())}");
            Console.WriteLine($"Average per file: {stopwatch.ElapsedMilliseconds / (double)files.Length:F2}ms\n");
        }

        /// <summary>
        /// Advanced PLINQ pattern with custom thread control
        /// Use when you need explicit control over parallelization
        /// </summary>
        static void PLINQProcessing(string[] files)
        {
            Console.WriteLine("=== PLINQ with Custom Thread Control ===");
            var stopwatch = Stopwatch.StartNew();
            var results = new ConcurrentBag<BarcodeResults>();
            var processedCount = 0;

            files
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .ForAll(file =>
                {
                    var result = BarcodeReader.Read(file);
                    results.Add(result);

                    // Thread-safe progress reporting
                    var current = Interlocked.Increment(ref processedCount);
                    if (current % 100 == 0)
                    {
                        Console.WriteLine($"Progress: {current}/{files.Length}");
                    }
                });

            stopwatch.Stop();
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Barcodes found: {results.Count(r => r.Any())}");
            Console.WriteLine($"Average per file: {stopwatch.ElapsedMilliseconds / (double)files.Length:F2}ms\n");
        }

        /// <summary>
        /// Directory scanning with lazy enumeration
        /// Efficient for large directory trees
        /// </summary>
        static void DirectoryScanExample()
        {
            Console.WriteLine("=== Directory Scanning Example ===");

            // Create test directory if it doesn't exist
            if (!Directory.Exists("./warehouse-scans/"))
            {
                Console.WriteLine("Directory './warehouse-scans/' not found - skipping example\n");
                return;
            }

            var stopwatch = Stopwatch.StartNew();

            // Lazy enumeration - doesn't load all filenames into memory
            var imageFiles = Directory.EnumerateFiles(
                "./warehouse-scans/",
                "*.*",
                SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase));

            var results = new ConcurrentBag<(string FilePath, string BarcodeValue)>();

            var options = new BarcodeReaderOptions
            {
                Multithreaded = true,
                ReadingSpeed = ReadingSpeed.Balanced
            };

            var processedCount = 0;
            foreach (var file in imageFiles)
            {
                try
                {
                    var result = BarcodeReader.Read(file, options);
                    var barcode = result.FirstOrDefault();

                    if (barcode != null)
                    {
                        results.Add((file, barcode.Text));
                    }

                    processedCount++;
                    if (processedCount % 500 == 0)
                    {
                        Console.WriteLine($"Processed {processedCount} files, found {results.Count} barcodes");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {file}: {ex.Message}");
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"\nBatch complete:");
            Console.WriteLine($"  Files processed: {processedCount}");
            Console.WriteLine($"  Barcodes found: {results.Count}");
            Console.WriteLine($"  Total time: {stopwatch.ElapsedSeconds:F2}s\n");
        }
    }
}
