---
title: "How to Read Barcodes from PDF Files in C#"
reading_time: "8 min"
difficulty: "intermediate"
last_updated: "2026-01-23"
category: "read-barcodes"
---

# How to Read Barcodes from PDF Files in C#

IronBarcode reads barcodes directly from PDF files without requiring a separate image extraction step. This native PDF support simplifies workflows when processing invoices, shipping documents, and other PDF-based documents containing barcodes.

For basic barcode reading concepts, see [Read Barcodes from Images](./read-barcodes-images.md).

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

## Step 1: Read Barcodes from a PDF

Read barcodes from a PDF file with a single method call. IronBarcode automatically scans all pages and detects barcodes without manual configuration.

```csharp
using IronBarCode;

// Read barcodes from PDF with automatic page scanning
var results = BarcodeReader.ReadPdf("invoice.pdf");

// Access detected barcodes
foreach (var barcode in results)
{
    Console.WriteLine($"Value: {barcode.Text}");
    Console.WriteLine($"Format: {barcode.BarcodeType}");
    Console.WriteLine($"Page: {barcode.PageNumber}");
}
```

The `BarcodeReader.ReadPdf()` method accepts a PDF file path and returns a collection of `BarcodeResult` objects. Each result includes the barcode value, format type, and the PDF page number where the barcode was found.

This approach eliminates the multi-step workflow required by many barcode libraries (extract PDF pages as images → process images → read barcodes). IronBarcode handles PDF rendering internally.

---

## Step 2: Read from Specific Pages

Process only specific pages from large PDF documents to improve performance when barcode locations are known.

```csharp
using IronBarCode;

// Configure reader to process only pages 1-5
var options = new PdfBarcodeReaderOptions
{
    PageNumbers = new int[] { 1, 2, 3, 4, 5 }
};

var results = BarcodeReader.ReadPdf("multi-page-document.pdf", options);

foreach (var barcode in results)
{
    Console.WriteLine($"Found on page {barcode.PageNumber}: {barcode.Text}");
}
```

The `PdfBarcodeReaderOptions` class provides the `PageNumbers` property for specifying which pages to scan. Page numbers are 1-indexed (first page is 1, not 0).

Limiting page scanning reduces processing time for large documents. For example, if shipping labels always appear on the first page of a 50-page PDF, scanning only page 1 provides significant performance benefits.

---

## Step 3: Handle Multi-Page Documents

Process all pages in a PDF and organize results by page number for reporting or validation workflows.

```csharp
using IronBarCode;
using System.Linq;

// Read all barcodes from multi-page PDF
var results = BarcodeReader.ReadPdf("shipping-manifest.pdf");

// Group barcodes by page number
var barcodesByPage = results.GroupBy(b => b.PageNumber);

foreach (var pageGroup in barcodesByPage)
{
    Console.WriteLine($"\nPage {pageGroup.Key}:");
    foreach (var barcode in pageGroup)
    {
        Console.WriteLine($"  {barcode.BarcodeType}: {barcode.Text}");
    }
}

// Summary statistics
Console.WriteLine($"\nTotal barcodes: {results.Count()}");
Console.WriteLine($"Pages with barcodes: {barcodesByPage.Count()}");
```

The `PageNumber` property on each `BarcodeResult` enables grouping, filtering, and page-specific validation logic. This is particularly useful when processing batch documents where each page represents a separate item or shipment.

---

## Step 4: Configure PDF Reading Options

Adjust PDF rendering quality to handle documents with small or low-resolution barcodes.

```csharp
using IronBarCode;

// Configure high-quality PDF rendering for small barcodes
var options = new PdfBarcodeReaderOptions
{
    // Increase DPI for better quality (default is 96)
    Dpi = 300,

    // Scale the rendered image (2.0 = 200%)
    Scale = 2.0
};

var results = BarcodeReader.ReadPdf("high-density-labels.pdf", options);

foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.Text} (confidence: {barcode.Confidence}%)");
}
```

The `Dpi` option controls the resolution at which PDF pages are rendered before barcode detection. Higher DPI values improve detection of small barcodes but increase memory usage and processing time.

The `Scale` option multiplies the rendered image size. A scale of 2.0 doubles both width and height, which can help with barcode detection when the PDF contains very small barcodes.

**When to increase DPI or Scale:**
- Barcodes smaller than 0.5 inches (1.27 cm) on printed page
- PDF created at low resolution (72 DPI or lower)
- Barcodes failing to scan at default settings

**When to use defaults:**
- Standard document printing (8.5" x 11" at 96-150 DPI)
- Barcodes larger than 1 inch (2.54 cm)
- Performance-critical batch processing

---

## Step 5: Process Multiple PDF Files

Batch process multiple PDF files with error handling for production workflows.

```csharp
using IronBarCode;
using System.IO;

// Get all PDF files in directory
var pdfFiles = Directory.GetFiles("./invoices", "*.pdf");

foreach (var pdfPath in pdfFiles)
{
    try
    {
        // Read barcodes from each PDF
        var results = BarcodeReader.ReadPdf(pdfPath);

        Console.WriteLine($"\n{Path.GetFileName(pdfPath)}:");

        if (results.Any())
        {
            foreach (var barcode in results)
            {
                Console.WriteLine($"  Page {barcode.PageNumber}: {barcode.Text}");
            }
        }
        else
        {
            Console.WriteLine("  No barcodes detected");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  Error processing {pdfPath}: {ex.Message}");
    }
}
```

This batch processing pattern handles directories of PDF files with appropriate error handling. The `try-catch` block prevents a single corrupted PDF from stopping the entire batch.

For high-volume processing, consider adding:
- Parallel processing with `Parallel.ForEach()` for multi-core utilization
- Progress tracking for long-running batches
- Logging to file for audit trails
- Database storage for extracted barcode data

---

## Complete Working Example

Here's the complete code demonstrating PDF barcode reading with various configurations:

```csharp
using System;
using System.Linq;
using System.IO;
using IronBarCode;

namespace ReadBarcodesFromPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example 1: Basic PDF reading
            Console.WriteLine("Example 1: Basic PDF Reading");
            Console.WriteLine("========================================");

            var basicResults = BarcodeReader.ReadPdf("document.pdf");

            foreach (var barcode in basicResults)
            {
                Console.WriteLine($"Value: {barcode.Text}");
                Console.WriteLine($"Format: {barcode.BarcodeType}");
                Console.WriteLine($"Page: {barcode.PageNumber}\n");
            }

            // Example 2: Specific page reading
            Console.WriteLine("\nExample 2: Read from Specific Pages");
            Console.WriteLine("========================================");

            var pageOptions = new PdfBarcodeReaderOptions
            {
                PageNumbers = new int[] { 1, 2, 3 }
            };

            var pageResults = BarcodeReader.ReadPdf("multi-page.pdf", pageOptions);
            Console.WriteLine($"Found {pageResults.Count()} barcodes on pages 1-3");

            // Example 3: Multi-page processing with grouping
            Console.WriteLine("\nExample 3: Multi-Page Document Processing");
            Console.WriteLine("========================================");

            var multiResults = BarcodeReader.ReadPdf("shipping-manifest.pdf");
            var barcodesByPage = multiResults.GroupBy(b => b.PageNumber);

            foreach (var pageGroup in barcodesByPage)
            {
                Console.WriteLine($"\nPage {pageGroup.Key}:");
                foreach (var barcode in pageGroup)
                {
                    Console.WriteLine($"  {barcode.BarcodeType}: {barcode.Text}");
                }
            }

            // Example 4: High-quality rendering for small barcodes
            Console.WriteLine("\nExample 4: High-Quality PDF Rendering");
            Console.WriteLine("========================================");

            var qualityOptions = new PdfBarcodeReaderOptions
            {
                Dpi = 300,
                Scale = 2.0
            };

            var qualityResults = BarcodeReader.ReadPdf("small-barcodes.pdf", qualityOptions);
            Console.WriteLine($"Detected {qualityResults.Count()} barcodes at 300 DPI");

            // Example 5: Batch processing
            Console.WriteLine("\nExample 5: Batch Processing Multiple PDFs");
            Console.WriteLine("========================================");

            if (Directory.Exists("./pdfs"))
            {
                var pdfFiles = Directory.GetFiles("./pdfs", "*.pdf");

                foreach (var pdfPath in pdfFiles)
                {
                    try
                    {
                        var batchResults = BarcodeReader.ReadPdf(pdfPath);
                        Console.WriteLine($"{Path.GetFileName(pdfPath)}: {batchResults.Count()} barcodes");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{Path.GetFileName(pdfPath)}: Error - {ex.Message}");
                    }
                }
            }

            Console.WriteLine("\nProcessing complete!");
        }
    }
}
```

**Download:** [Complete code file](../../code-examples/tutorials/read-barcodes-pdfs.cs)

---

## Next Steps

Now that you understand PDF barcode reading, explore these related tutorials:

- **[Batch Processing Barcodes](./batch-processing.md)** - Process multiple PDF files efficiently with parallel processing
- **[Read Barcodes from Images](./read-barcodes-images.md)** - Learn the foundational image reading techniques
- **[Handle Damaged Barcodes](./damaged-barcode-reading.md)** - Configure ML error correction for poor quality scans

Return to [All Tutorials](./README.md)

---

Last verified: January 2026

*Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [Iron Software](https://ironsoftware.com)*
