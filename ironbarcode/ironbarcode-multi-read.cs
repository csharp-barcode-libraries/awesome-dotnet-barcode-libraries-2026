/**
 * IronBarcode Multi-Barcode Detection Example
 *
 * This example demonstrates reading multiple barcodes from a single image.
 * IronBarcode automatically detects all barcodes regardless of format,
 * returning detailed metadata about each one.
 *
 * Key Features Shown:
 * - Multiple barcode detection
 * - Mixed format support (1D and 2D together)
 * - Barcode metadata access
 * - Confidence scoring
 * - Location/positioning data
 *
 * NuGet Package: IronBarcode version 2024.x+
 * Documentation: https://ironsoftware.com/csharp/barcode/
 */

// Install: dotnet add package IronBarcode
using IronBarCode;
using System;
using System.Linq;

public class MultiReadExample
{
    public static void Main()
    {
        // Read all barcodes from image (any format, any quantity)
        BasicMultiRead();

        // Access detailed metadata for each barcode
        DetailedMetadataExample();

        // Handle mixed formats in same image
        MixedFormatExample();

        // Process results with confidence filtering
        ConfidenceFilteringExample();
    }

    static void BasicMultiRead()
    {
        // Single call detects all barcodes in the image
        var results = BarcodeReader.Read("warehouse-pallet.jpg");

        Console.WriteLine($"Found {results.Count()} barcodes:");
        foreach (var barcode in results)
        {
            Console.WriteLine($"  {barcode.Value} ({barcode.Format})");
        }
    }

    static void DetailedMetadataExample()
    {
        var results = BarcodeReader.Read("shipping-label.png");

        foreach (var barcode in results)
        {
            // Core data
            Console.WriteLine($"Value: {barcode.Value}");
            Console.WriteLine($"Format: {barcode.Format}");
            Console.WriteLine($"Confidence: {barcode.Confidence}%");

            // Position in image (useful for document processing)
            Console.WriteLine($"Location: X={barcode.X}, Y={barcode.Y}");
            Console.WriteLine($"Size: {barcode.Width}x{barcode.Height}");

            // Page info (for multi-page documents)
            Console.WriteLine($"Page: {barcode.PageNumber}");

            Console.WriteLine("---");
        }
    }

    static void MixedFormatExample()
    {
        // Image containing both 1D and 2D barcodes
        var results = BarcodeReader.Read("product-packaging.jpg");

        // Separate by format type
        var linearBarcodes = results.Where(b =>
            b.Format == BarcodeEncoding.Code128 ||
            b.Format == BarcodeEncoding.EAN13 ||
            b.Format == BarcodeEncoding.UPCA);

        var matrixBarcodes = results.Where(b =>
            b.Format == BarcodeEncoding.QRCode ||
            b.Format == BarcodeEncoding.DataMatrix);

        Console.WriteLine($"Linear (1D) barcodes: {linearBarcodes.Count()}");
        foreach (var bc in linearBarcodes)
        {
            Console.WriteLine($"  {bc.Format}: {bc.Value}");
        }

        Console.WriteLine($"Matrix (2D) barcodes: {matrixBarcodes.Count()}");
        foreach (var bc in matrixBarcodes)
        {
            Console.WriteLine($"  {bc.Format}: {bc.Value}");
        }
    }

    static void ConfidenceFilteringExample()
    {
        var results = BarcodeReader.Read("damaged-scan.jpg");

        // Filter for high-confidence results only
        var reliable = results.Where(b => b.Confidence >= 90);
        var uncertain = results.Where(b => b.Confidence < 90);

        Console.WriteLine($"High confidence (>=90%): {reliable.Count()}");
        foreach (var bc in reliable)
        {
            Console.WriteLine($"  {bc.Value} - {bc.Confidence}%");
        }

        Console.WriteLine($"Lower confidence (<90%): {uncertain.Count()}");
        foreach (var bc in uncertain)
        {
            Console.WriteLine($"  {bc.Value} - {bc.Confidence}% (verify manually)");
        }
    }
}
