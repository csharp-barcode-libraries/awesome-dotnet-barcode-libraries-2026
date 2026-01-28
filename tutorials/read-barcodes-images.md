---
title: "How to Read Barcodes from Images in C#"
reading_time: "5 min"
difficulty: "beginner"
last_updated: "2026-01-23"
category: "read-barcodes"
---

# How to Read Barcodes from Images in C#

IronBarcode reads barcodes from image files with a single line of code and automatic format detection. This tutorial demonstrates reading barcodes from JPG, PNG, and TIFF files without manually specifying barcode types.

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

## Step 1: Read Your First Barcode

The simplest barcode reading operation requires just one line of code. IronBarcode automatically detects the barcode format without requiring manual specification.

```csharp
using IronBarCode;

// Read barcode from image with automatic format detection
var result = BarcodeReader.Read("product-label.png");

// Access the first detected barcode value
Console.WriteLine(result.FirstOrDefault()?.Text);
```

The `BarcodeReader.Read()` method accepts image file paths in JPG, PNG, TIFF, BMP, and GIF formats. The automatic format detection handles Code128, QR codes, EAN-13, UPC-A, and 50+ other barcode types without configuration.

---

## Step 2: Handle Multiple Barcodes

When processing images containing multiple barcodes, configure the reader to expect multiple results. This is common with batch processing, shipping labels, or inventory documents.

```csharp
using IronBarCode;

// Configure reader for multiple barcodes
var options = new BarcodeReaderOptions
{
    ExpectMultipleBarcodes = true
};

// Read all barcodes from image
var results = BarcodeReader.Read("shipping-label.png", options);

// Process each detected barcode
foreach (var barcode in results)
{
    Console.WriteLine($"Type: {barcode.BarcodeType}");
    Console.WriteLine($"Value: {barcode.Text}");
    Console.WriteLine("---");
}
```

The reader returns all detected barcodes with their format types and decoded values. This works with mixed barcode types in a single image.

---

## Step 3: Read Damaged or Low-Quality Barcodes

IronBarcode uses machine learning for error correction, enabling it to read damaged, rotated, or partially obscured barcodes that other libraries fail to process. This capability is critical for real-world scenarios with warehouse labels, weathered packaging, or scanned documents.

```csharp
using IronBarCode;

// Enable full ML error correction for damaged barcodes
var options = new BarcodeReaderOptions
{
    ReadingSpeed = ReadingSpeed.Detailed,
    ExpectBarcodeTypes = BarcodeEncoding.All
};

// Read barcode from damaged or low-quality image
var result = BarcodeReader.Read("damaged-barcode.png", options);

Console.WriteLine($"Decoded: {result.FirstOrDefault()?.Text}");
```

The `ReadingSpeed.Detailed` setting activates IronBarcode's full machine learning algorithms to reconstruct missing or corrupted barcode elements. This mode succeeds where faster processing modes and other libraries fail, handling:

- Crumpled or folded labels
- Faded or low-contrast prints
- Rotated barcodes (any angle)
- Partially obscured barcodes
- Poor lighting conditions

The ML error correction differentiates IronBarcode from traditional barcode libraries that rely solely on pixel pattern matching. Having built barcode tools for 10+ years, this capability consistently proves valuable in production environments where image quality cannot be guaranteed.

---

## Step 4: Configure Reading Speed

Choose the appropriate reading speed based on image quality and performance requirements. The `ReadingSpeed` enum provides three modes optimized for different scenarios.

```csharp
using IronBarCode;

// Faster: Maximum performance for clean, single barcodes
var fastOptions = new BarcodeReaderOptions
{
    ReadingSpeed = ReadingSpeed.Faster
};

// Balanced: Default mode for typical use cases
var balancedOptions = new BarcodeReaderOptions
{
    ReadingSpeed = ReadingSpeed.Balanced
};

// Detailed: Full ML error correction for damaged barcodes
var detailedOptions = new BarcodeReaderOptions
{
    ReadingSpeed = ReadingSpeed.Detailed
};
```

**When to use each mode:**

| Speed Mode | Best For | Processing Time | ML Error Correction |
|------------|----------|----------------|---------------------|
| Faster | Clean, high-quality images with single barcodes | Minimal | Basic |
| Balanced | Most real-world scenarios | Moderate | Standard |
| Detailed | Damaged, rotated, or low-quality barcodes | Maximum | Full |

Start with `Balanced` mode for typical applications. Use `Detailed` mode when working with variable image quality or when initial reads fail. Use `Faster` mode only when processing high-quality images where performance is critical.

---

## Complete Working Example

Here's the complete code combining all reading scenarios:

```csharp
// Install: dotnet add package IronBarcode
using System;
using IronBarCode;

namespace BarcodeReading
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example 1: Simple single barcode reading
            var simpleResult = BarcodeReader.Read("barcode.png");
            Console.WriteLine($"Simple read: {simpleResult.FirstOrDefault()?.Text}");

            // Example 2: Multiple barcodes from one image
            var multiOptions = new BarcodeReaderOptions
            {
                ExpectMultipleBarcodes = true
            };
            var multiResults = BarcodeReader.Read("multi-barcode.png", multiOptions);

            Console.WriteLine("\nMultiple barcodes:");
            foreach (var barcode in multiResults)
            {
                Console.WriteLine($"{barcode.BarcodeType}: {barcode.Text}");
            }

            // Example 3: Damaged barcode with ML error correction
            var damagedOptions = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Detailed,
                ExpectBarcodeTypes = BarcodeEncoding.All
            };
            var damagedResult = BarcodeReader.Read("damaged-label.png", damagedOptions);
            Console.WriteLine($"\nDamaged barcode recovered: {damagedResult.FirstOrDefault()?.Text}");

            // Example 4: High-performance reading for clean images
            var fastOptions = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Faster
            };
            var fastResult = BarcodeReader.Read("clean-barcode.png", fastOptions);
            Console.WriteLine($"Fast read: {fastResult.FirstOrDefault()?.Text}");
        }
    }
}
```

**Download:** [Complete code file](../code-examples/tutorials/read-barcodes-images.cs)

---

## Next Steps

Now that you understand barcode reading from images, explore these related tutorials:

- **[How to Generate Barcodes in C#](./generate-barcode-basic.md)** - Create your own barcodes for testing and production
- **[Understanding Barcode Format Types](./barcode-format-types.md)** - Learn when to use 1D vs 2D barcode formats
- **[How to Read Barcodes from PDFs](./read-barcodes-pdfs.md)** - Extract barcodes from PDF documents
- **[Batch Processing Multiple Images](./batch-processing.md)** - Process entire directories of barcode images efficiently

Return to [All Tutorials](./README.md)

---

Last verified: January 2026

*Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [Iron Software](https://ironsoftware.com)*
