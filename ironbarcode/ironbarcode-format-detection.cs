/**
 * IronBarcode Automatic Format Detection Example
 *
 * This example demonstrates IronBarcode's ability to read any barcode
 * without specifying its type. The library automatically detects from
 * 50+ supported symbologies, eliminating format specification errors.
 *
 * Key Features Shown:
 * - Automatic format detection (50+ symbologies)
 * - No format specification required
 * - Mixed format processing
 * - Format identification in results
 * - Comparison to manual specification approach
 *
 * NuGet Package: IronBarcode version 2024.x+
 * Documentation: https://ironsoftware.com/csharp/barcode/
 */

// Install: dotnet add package IronBarcode
using IronBarCode;
using System;
using System.Collections.Generic;
using System.Linq;

public class FormatDetectionExample
{
    public static void Main()
    {
        // Automatic detection - just pass the image
        AutomaticDetection();

        // Works across all 50+ symbologies
        AllSymbologiesDemo();

        // Process mixed formats without configuration
        MixedFormatBatch();

        // Optional: Specify format when known (for performance)
        ManualSpecificationExample();

        // Identify unknown barcodes
        IdentifyBarcodeType();
    }

    static void AutomaticDetection()
    {
        // No format specification - IronBarcode figures it out
        var result = BarcodeReader.Read("unknown-barcode.png").First();

        Console.WriteLine("Automatic Detection:");
        Console.WriteLine($"  Value: {result.Value}");
        Console.WriteLine($"  Detected format: {result.Format}");
        Console.WriteLine();

        // Works identically for any format
        var qr = BarcodeReader.Read("qr-code.png").First();
        var code128 = BarcodeReader.Read("shipping-label.png").First();
        var ean = BarcodeReader.Read("product.png").First();

        Console.WriteLine($"  QR Code: {qr.Value} (detected as {qr.Format})");
        Console.WriteLine($"  Code128: {code128.Value} (detected as {code128.Format})");
        Console.WriteLine($"  EAN: {ean.Value} (detected as {ean.Format})");
    }

    static void AllSymbologiesDemo()
    {
        Console.WriteLine("\nSupported Symbologies (50+):");
        Console.WriteLine();
        Console.WriteLine("1D Linear Barcodes:");
        Console.WriteLine("  - Code128, Code39, Code93, Code11");
        Console.WriteLine("  - EAN-13, EAN-8, UPC-A, UPC-E");
        Console.WriteLine("  - ITF (Interleaved 2 of 5)");
        Console.WriteLine("  - Codabar, MSI, Plessey");
        Console.WriteLine("  - Pharmacode, PZN");
        Console.WriteLine("  - ISBN, ISSN, ISMN");
        Console.WriteLine();
        Console.WriteLine("2D Matrix Barcodes:");
        Console.WriteLine("  - QR Code (all versions)");
        Console.WriteLine("  - Data Matrix (ECC 200)");
        Console.WriteLine("  - PDF417, MicroPDF417");
        Console.WriteLine("  - Aztec Code");
        Console.WriteLine("  - MaxiCode");
        Console.WriteLine();
        Console.WriteLine("All detected automatically - no configuration needed.");
    }

    static void MixedFormatBatch()
    {
        // Process folder containing various barcode types
        var files = new[]
        {
            "warehouse/qr-code.png",
            "warehouse/shipping-code128.png",
            "warehouse/product-ean13.png",
            "warehouse/serial-datamatrix.png",
            "warehouse/document-pdf417.png"
        };

        Console.WriteLine("\nMixed Format Batch Processing:");
        var formatCounts = new Dictionary<BarcodeEncoding, int>();

        foreach (var file in files)
        {
            var results = BarcodeReader.Read(file);
            foreach (var barcode in results)
            {
                Console.WriteLine($"  {file}: {barcode.Value} ({barcode.Format})");

                if (!formatCounts.ContainsKey(barcode.Format))
                    formatCounts[barcode.Format] = 0;
                formatCounts[barcode.Format]++;
            }
        }

        Console.WriteLine("\nFormat Summary:");
        foreach (var kvp in formatCounts.OrderByDescending(k => k.Value))
        {
            Console.WriteLine($"  {kvp.Key}: {kvp.Value} barcodes");
        }
    }

    static void ManualSpecificationExample()
    {
        Console.WriteLine("\nManual Specification (optional performance optimization):");

        // When you KNOW the format, you can specify it for faster processing
        // This skips detection of other formats
        var result = BarcodeReader.Read("product.png", BarcodeEncoding.EAN13).First();
        Console.WriteLine($"  Specified EAN-13: {result.Value}");

        // Limit to specific format family
        var options = new BarcodeReaderOptions
        {
            ExpectBarcodeTypes = BarcodeEncoding.AllOneDimensional
        };
        var oneDResults = BarcodeReader.Read("shipping.png", options);
        Console.WriteLine($"  1D only scan found: {oneDResults.Count()} barcodes");

        // Multiple specific formats
        options.ExpectBarcodeTypes = BarcodeEncoding.Code128 | BarcodeEncoding.QRCode;
        var specificResults = BarcodeReader.Read("label.png", options);
        Console.WriteLine($"  Code128 or QR scan found: {specificResults.Count()} barcodes");

        Console.WriteLine();
        Console.WriteLine("Note: Manual specification is optional. Automatic detection");
        Console.WriteLine("works for all cases and is recommended for most use cases.");
    }

    static void IdentifyBarcodeType()
    {
        Console.WriteLine("\nBarcode Type Identification:");
        Console.WriteLine("(Useful when you receive unknown barcodes)");

        var unknownBarcodes = new[]
        {
            "samples/unknown1.png",
            "samples/unknown2.png",
            "samples/unknown3.png"
        };

        foreach (var file in unknownBarcodes)
        {
            var results = BarcodeReader.Read(file);
            if (results.Any())
            {
                var barcode = results.First();
                Console.WriteLine($"\n  File: {file}");
                Console.WriteLine($"  Type: {barcode.Format}");
                Console.WriteLine($"  Value: {barcode.Value}");
                Console.WriteLine($"  Confidence: {barcode.Confidence}%");
            }
        }
    }
}
