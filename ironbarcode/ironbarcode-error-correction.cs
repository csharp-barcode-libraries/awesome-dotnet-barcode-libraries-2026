/**
 * IronBarcode ML-Powered Error Correction Example
 *
 * This example demonstrates IronBarcode's automatic error correction
 * for damaged, partial, or low-quality barcodes. The ML engine handles
 * real-world imperfections without manual configuration.
 *
 * Key Features Shown:
 * - Automatic damaged barcode recovery
 * - Partial barcode reading
 * - Low-quality scan handling
 * - Confidence score interpretation
 * - No manual preprocessing required
 *
 * NuGet Package: IronBarcode version 2024.x+
 * Documentation: https://ironsoftware.com/csharp/barcode/
 */

// Install: dotnet add package IronBarcode
using IronBarCode;
using System;
using System.Linq;

public class ErrorCorrectionExample
{
    public static void Main()
    {
        // ML correction happens automatically - no configuration
        ReadDamagedBarcode();

        // Handle various quality issues
        ReadPartialBarcode();
        ReadLowQualityScan();
        ReadRotatedBarcode();
        ReadBlurredBarcode();

        // Interpret confidence scores
        ConfidenceInterpretation();
    }

    static void ReadDamagedBarcode()
    {
        // Scratched, faded, or marked barcodes
        // ML correction is automatic - just call Read()
        var result = BarcodeReader.Read("scratched-barcode.png").First();

        Console.WriteLine("Damaged Barcode Recovery:");
        Console.WriteLine($"  Value: {result.Value}");
        Console.WriteLine($"  Format: {result.Format}");
        Console.WriteLine($"  Confidence: {result.Confidence}%");
        Console.WriteLine();

        // The same call works for stained barcodes
        var stainedResult = BarcodeReader.Read("stained-label.jpg").First();
        Console.WriteLine($"Stained barcode: {stainedResult.Value} ({stainedResult.Confidence}% confidence)");
    }

    static void ReadPartialBarcode()
    {
        // Barcode is partially visible (torn label, cropped image)
        // ML engine reconstructs missing portions when possible
        var result = BarcodeReader.Read("partial-barcode.png").FirstOrDefault();

        if (result != null)
        {
            Console.WriteLine("Partial Barcode Recovery:");
            Console.WriteLine($"  Recovered value: {result.Value}");
            Console.WriteLine($"  Confidence: {result.Confidence}%");

            if (result.Confidence < 80)
            {
                Console.WriteLine("  Note: Low confidence suggests manual verification recommended");
            }
        }
        else
        {
            Console.WriteLine("Partial barcode: Too much damage for reliable recovery");
        }
    }

    static void ReadLowQualityScan()
    {
        // Low resolution, high compression, or poor lighting
        var result = BarcodeReader.Read("low-res-scan.jpg").First();

        Console.WriteLine("Low Quality Scan:");
        Console.WriteLine($"  Value: {result.Value}");
        Console.WriteLine($"  Confidence: {result.Confidence}%");

        // For very low quality, use detailed scanning mode
        var options = new BarcodeReaderOptions
        {
            Speed = ReadingSpeed.ExtremeDetail  // More processing time, better accuracy
        };

        var detailedResult = BarcodeReader.Read("very-low-quality.jpg", options).First();
        Console.WriteLine($"  With ExtremeDetail: {detailedResult.Value} ({detailedResult.Confidence}%)");
    }

    static void ReadRotatedBarcode()
    {
        // Barcode at angle (not perfectly horizontal/vertical)
        // Automatic rotation detection and correction
        var result = BarcodeReader.Read("rotated-label.png").First();

        Console.WriteLine("Rotated Barcode:");
        Console.WriteLine($"  Value: {result.Value}");
        Console.WriteLine($"  Confidence: {result.Confidence}%");
        Console.WriteLine("  (Rotation detected and corrected automatically)");
    }

    static void ReadBlurredBarcode()
    {
        // Motion blur, focus issues
        var result = BarcodeReader.Read("blurred-scan.jpg").First();

        Console.WriteLine("Blurred Barcode:");
        Console.WriteLine($"  Value: {result.Value}");
        Console.WriteLine($"  Confidence: {result.Confidence}%");
    }

    static void ConfidenceInterpretation()
    {
        Console.WriteLine("\nConfidence Score Guide:");
        Console.WriteLine("  95-100%: Perfect read - high quality source");
        Console.WriteLine("  85-94%:  Reliable - minor quality issues corrected");
        Console.WriteLine("  70-84%:  Usable - significant correction applied");
        Console.WriteLine("  50-69%:  Review recommended - substantial reconstruction");
        Console.WriteLine("  <50%:    Verify manually - may contain errors");
        Console.WriteLine();

        // Example: Production workflow with confidence thresholds
        var results = BarcodeReader.Read("batch-scans");

        var reliable = results.Where(r => r.Confidence >= 85);
        var needsReview = results.Where(r => r.Confidence >= 50 && r.Confidence < 85);
        var rejected = results.Where(r => r.Confidence < 50);

        Console.WriteLine("Batch Processing Results:");
        Console.WriteLine($"  Auto-accepted (>=85%): {reliable.Count()}");
        Console.WriteLine($"  Needs review (50-84%): {needsReview.Count()}");
        Console.WriteLine($"  Rejected (<50%): {rejected.Count()}");
    }
}
