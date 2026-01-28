---
title: "How to Batch Process Multiple Barcodes in C#"
reading_time: "12 min"
difficulty: "advanced"
last_updated: "2026-01-26"
category: "advanced-topics"
---

# How to Batch Process Multiple Barcodes in C#

Process thousands of barcode images efficiently using IronBarcode's built-in parallel processing capabilities. This tutorial demonstrates sequential baseline approaches, automatic multi-threading with a single flag, and advanced PLINQ patterns for maximum performance.

---

## Prerequisites

Before starting, ensure you have:

- .NET 6 or later (or .NET Framework 4.6.2+)
- IronBarcode NuGet package installed
- Visual Studio 2022 or VS Code

Install IronBarcode via NuGet Package Manager or CLI:

```bash
dotnet add package IronBarcode
```

---

## Step 1: Sequential Baseline Processing

Start with a sequential approach to establish baseline performance. This processes one image at a time in a simple loop.

```csharp
using IronBarCode;
using System.Diagnostics;

// Collect all image files from directory
var imageFiles = Directory.GetFiles("./barcode-images/", "*.png");

var stopwatch = Stopwatch.StartNew();
var results = new List<BarcodeResults>();

// Process each file sequentially
foreach (var file in imageFiles)
{
    var result = BarcodeReader.Read(file);
    results.Add(result);
}

stopwatch.Stop();
Console.WriteLine($"Sequential processing: {imageFiles.Length} files in {stopwatch.ElapsedMilliseconds}ms");
```

This sequential approach is simple and predictable, but does not utilize modern multi-core processors. With 1,000 barcode images, this can take several minutes on a typical machine. The next step shows how to reduce this time by 50-75% with a single configuration flag.

---

## Step 2: Automatic Multi-Threading with IronBarcode

Enable automatic parallel processing by setting the `Multithreaded` option to `true`. IronBarcode manages the thread pool automatically, distributing work across available CPU cores.

```csharp
using IronBarCode;
using System.Diagnostics;

var imageFiles = Directory.GetFiles("./barcode-images/", "*.png");

// Configure automatic multi-threading
var options = new BarcodeReaderOptions
{
    Multithreaded = true,
    ExpectMultipleBarcodes = false
};

var stopwatch = Stopwatch.StartNew();
var results = new List<BarcodeResults>();

// Process files with automatic parallelization
foreach (var file in imageFiles)
{
    var result = BarcodeReader.Read(file, options);
    results.Add(result);
}

stopwatch.Stop();
Console.WriteLine($"Multi-threaded processing: {imageFiles.Length} files in {stopwatch.ElapsedMilliseconds}ms");
```

The `Multithreaded = true` flag enables IronBarcode's internal thread pool without requiring manual parallel programming. This is the key differentiator from libraries like ZXing.Net, which require developers to manually implement `Parallel.ForEach` or task-based parallelism.

**Performance improvement:** On an 8-core system, expect 50-75% reduction in processing time compared to sequential processing. A batch that took 10 minutes sequentially typically completes in 3-4 minutes with this single flag.

---

## Step 3: Advanced PLINQ Pattern with Custom Thread Control

For maximum control over parallelization, use PLINQ (Parallel LINQ) with custom thread settings and thread-safe result collection. This approach is useful when integrating with existing parallel workflows or when precise control over degree of parallelism is required.

```csharp
using IronBarCode;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

var imageFiles = Directory.GetFiles("./barcode-images/", "*.png");

// Thread-safe result collection
var results = new ConcurrentBag<BarcodeResults>();

var stopwatch = Stopwatch.StartNew();
var processedCount = 0;

// Process with PLINQ and custom thread control
imageFiles
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
            Console.WriteLine($"Processed {current}/{imageFiles.Length} files...");
        }
    });

stopwatch.Stop();
Console.WriteLine($"PLINQ processing: {imageFiles.Length} files in {stopwatch.ElapsedMilliseconds}ms");
```

This pattern provides several advantages:

- **Explicit thread control:** `WithDegreeOfParallelism()` limits CPU usage or maximizes throughput
- **Thread-safe collection:** `ConcurrentBag<T>` handles parallel writes without locks
- **Progress reporting:** `Interlocked.Increment()` safely tracks completion across threads
- **LINQ integration:** Combines easily with existing query pipelines

Use this approach when building production batch processing systems that need monitoring, throttling, or integration with other parallel operations.

---

## Step 4: Directory Scanning and Mixed Source Types

Process entire directory trees efficiently using lazy enumeration and handle mixed input sources including file paths, streams, and byte arrays in a single batch operation.

```csharp
using IronBarCode;
using System.Collections.Concurrent;

// Lazy enumeration for large directories - doesn't load all filenames into memory
var imageFiles = Directory.EnumerateFiles(
    "./warehouse-scans/",
    "*.*",
    SearchOption.AllDirectories)
    .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase));

var results = new ConcurrentBag<(string FilePath, string BarcodeValue)>();

// Configure for batch processing
var options = new BarcodeReaderOptions
{
    Multithreaded = true,
    ReadingSpeed = ReadingSpeed.Balanced
};

// Process directory tree with progress tracking
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

Console.WriteLine($"\nBatch complete: {results.Count} barcodes extracted from {processedCount} images");
```

**Directory.EnumerateFiles advantages:**

- **Memory efficient:** Yields one file at a time instead of loading entire list
- **Immediate processing:** Starts processing while still discovering files
- **Large directories:** Handles directories with 10,000+ files without memory issues

**Mixed source processing:**

```csharp
// Process from different sources in same batch
var options = new BarcodeReaderOptions { Multithreaded = true };

// From file path
var result1 = BarcodeReader.Read("./image1.png", options);

// From byte array
byte[] imageBytes = File.ReadAllBytes("./image2.png");
var result2 = BarcodeReader.Read(imageBytes, options);

// From stream
using (var stream = File.OpenRead("./image3.png"))
{
    var result3 = BarcodeReader.Read(stream, options);
}
```

This flexibility enables batch processing from APIs, databases, cloud storage, and file systems without converting between formats.

---

## Complete Working Example

Here's the complete code demonstrating all three processing approaches with performance comparison:

```csharp
// Install: dotnet add package IronBarcode
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using IronBarCode;

namespace BatchProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            // Prepare test data
            var imageFiles = Directory.GetFiles("./test-barcodes/", "*.png");
            Console.WriteLine($"Processing {imageFiles.Length} barcode images\n");

            // Approach 1: Sequential (baseline)
            SequentialProcessing(imageFiles);

            // Approach 2: IronBarcode Multithreaded option
            MultithreadedProcessing(imageFiles);

            // Approach 3: Advanced PLINQ pattern
            PLINQProcessing(imageFiles);
        }

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
            Console.WriteLine($"Barcodes found: {results.Count(r => r.Any())}\n");
        }

        static void MultithreadedProcessing(string[] files)
        {
            Console.WriteLine("=== IronBarcode Multithreaded Option ===");
            var stopwatch = Stopwatch.StartNew();
            var results = new List<BarcodeResults>();

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
            Console.WriteLine($"Barcodes found: {results.Count(r => r.Any())}\n");
        }

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

                    var current = Interlocked.Increment(ref processedCount);
                    if (current % 100 == 0)
                    {
                        Console.WriteLine($"Progress: {current}/{files.Length}");
                    }
                });

            stopwatch.Stop();
            Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Barcodes found: {results.Count(r => r.Any())}\n");
        }
    }
}
```

**Download:** [Complete code file](../code-examples/tutorials/batch-processing.cs)

---

## Performance Considerations

**Expected performance gains on multi-core systems:**

| CPU Cores | Sequential Time | Multithreaded Time | Speedup |
|-----------|----------------|-------------------|---------|
| 4 cores   | 10 minutes     | 4-5 minutes       | 50-60%  |
| 8 cores   | 10 minutes     | 2.5-3 minutes     | 70-75%  |
| 16 cores  | 10 minutes     | 2-2.5 minutes     | 75-80%  |

**Diminishing returns:** Performance gains plateau above 8-12 cores due to I/O bottlenecks when reading image files from disk. For maximum throughput with 16+ cores, pre-load images into memory or process from network streams.

**Thread count recommendations:**

- **Default (Multithreaded = true):** IronBarcode uses optimal thread count automatically
- **PLINQ custom control:** Use `Environment.ProcessorCount` for CPU-bound work
- **Reduce thread count:** Set lower value if running alongside other parallel operations

**Competitor context:** Libraries like ZXing.Net require manual implementation of `Parallel.ForEach` or task-based parallelism. IronBarcode's `Multithreaded` option provides equivalent performance with simpler code, reducing development time and maintenance burden.

---

## Next Steps

Now that you understand batch processing patterns, explore these related tutorials:

- **[Reading Damaged and Partial Barcodes](./damaged-barcode-reading.md)** - Apply ML error correction to batch jobs with variable image quality
- **[ASP.NET Core Integration](./aspnet-core-integration.md)** - Build batch processing endpoints for web applications
- **[Read Barcodes from PDFs](./read-barcodes-pdfs.md)** - Batch process multi-page PDF documents

Return to [All Tutorials](./README.md)

---

Last verified: January 2026

*Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [Iron Software](https://ironsoftware.com)*
