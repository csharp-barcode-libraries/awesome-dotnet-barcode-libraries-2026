/**
 * Performance Comparison: Dynamsoft Barcode Reader vs IronBarcode
 *
 * This example honestly demonstrates the performance characteristics of both
 * libraries in their respective areas of strength.
 *
 * Key Points:
 * - Dynamsoft IS faster for real-time camera scanning (acknowledge this)
 * - IronBarcode prioritizes accuracy over raw frame speed for documents
 * - Speed benchmarks without context are misleading
 * - Choose based on your actual use case, not benchmark numbers
 *
 * NuGet Packages Required:
 * - Dynamsoft: Dynamsoft.DotNet.BarcodeReader
 * - IronBarcode: IronBarcode version 2024.x+
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

// ============================================================
// HONEST ASSESSMENT: Dynamsoft Speed Advantage
// ============================================================

namespace PerformanceComparison
{
    /// <summary>
    /// Dynamsoft excels at real-time speed. This is a fact, not marketing.
    /// When milliseconds matter for user experience, Dynamsoft delivers.
    /// </summary>
    public static class SpeedReality
    {
        /*
         * HONEST ACKNOWLEDGMENT
         * =====================
         *
         * In real-time camera scanning scenarios, Dynamsoft is genuinely faster.
         * This isn't a weakness we're trying to spin - it's their core competency.
         *
         * Dynamsoft has spent years optimizing for:
         * - Sub-100ms frame decoding
         * - Motion blur compensation
         * - Autofocus prediction
         * - Video buffer management
         *
         * If you're building a mobile scanning app and need instant feedback
         * when users point their camera at a barcode, Dynamsoft deserves
         * serious consideration.
         *
         * However, speed isn't everything. Different use cases have different
         * priorities. A benchmark showing "Library A decodes a frame in 50ms
         * vs Library B in 150ms" tells you nothing about which library will
         * better serve your document processing workflow.
         */
    }

    /// <summary>
    /// Benchmark context matters more than raw numbers.
    /// </summary>
    public class BenchmarkContext
    {
        // SCENARIO 1: Real-time camera scanning (30 fps target)
        // ------------------------------------------------------
        // Dynamsoft: ~50-100ms per frame = good user experience
        // IronBarcode: ~100-300ms per frame = noticeable lag
        // Winner: Dynamsoft (this is their specialty)

        // SCENARIO 2: Processing 1000 PDF invoices overnight
        // ---------------------------------------------------
        // Per-frame speed irrelevant - total batch time matters
        // Dynamsoft: Requires PDF library + per-page rendering overhead
        // IronBarcode: Native PDF support, optimized batch processing
        // Winner: IronBarcode (faster total throughput, less code)

        // SCENARIO 3: Reading damaged shipping labels
        // -------------------------------------------
        // Speed less important than accuracy
        // Missing one barcode costs more than extra processing time
        // IronBarcode: ML-powered error correction, higher accuracy
        // Winner: IronBarcode (accuracy matters more than speed)

        // SCENARIO 4: High-volume real-time scanning (warehouse scanners)
        // ----------------------------------------------------------------
        // Dedicated hardware with camera input
        // Workers expect instant feedback
        // Dynamsoft: Optimized for this exact scenario
        // Winner: Dynamsoft (purpose-built for this)
    }
}

// ============================================================
// DOCUMENT ACCURACY DEMONSTRATION
// ============================================================

namespace DocumentAccuracy
{
    using IronBarcode;

    /// <summary>
    /// For document processing, accuracy often matters more than speed.
    /// IronBarcode's ML-powered error correction handles damaged codes.
    /// </summary>
    public class AccuracyDemonstration
    {
        /// <summary>
        /// Process damaged document barcodes where accuracy trumps speed.
        /// </summary>
        public static void ProcessDamagedDocuments()
        {
            // Real-world document barcodes suffer from:
            // - Thermal printer fading
            // - Label scratches and tears
            // - Scanner artifacts
            // - Poor print quality
            // - Smudges and stains

            var options = new BarcodeReaderOptions
            {
                // Detailed mode spends more time analyzing unclear codes
                Speed = ReadingSpeed.Detailed,

                // Correct for scanner rotation
                UseAutoRotate = true,

                // Find all barcodes on multi-code documents
                MultipleBarcodes = true
            };

            // IronBarcode's ML-powered processing can reconstruct:
            // - Partially obscured bars
            // - Faded thermal prints
            // - Codes with missing quiet zones

            string[] damagedDocuments = Directory.GetFiles("damaged-labels/", "*.png");

            foreach (string file in damagedDocuments)
            {
                var results = BarcodeReader.Read(file, options);

                foreach (var result in results)
                {
                    Console.WriteLine($"File: {Path.GetFileName(file)}");
                    Console.WriteLine($"  Value: {result.Value}");
                    Console.WriteLine($"  Format: {result.Format}");

                    // In document processing, catching damaged codes matters more
                    // than saving 50ms per image
                }
            }
        }

        /// <summary>
        /// Speed modes let you choose the accuracy/speed trade-off.
        /// </summary>
        public static void SpeedModeComparison()
        {
            string testFile = "sample-invoice.pdf";

            // BALANCED: Good default for most document processing
            var balancedOptions = new BarcodeReaderOptions
            {
                Speed = ReadingSpeed.Balanced
            };
            var balancedResults = BarcodeReader.Read(testFile, balancedOptions);
            Console.WriteLine($"Balanced mode found: {balancedResults.Count()} barcodes");

            // DETAILED: Maximum accuracy for damaged/unclear codes
            var detailedOptions = new BarcodeReaderOptions
            {
                Speed = ReadingSpeed.Detailed
            };
            var detailedResults = BarcodeReader.Read(testFile, detailedOptions);
            Console.WriteLine($"Detailed mode found: {detailedResults.Count()} barcodes");

            // FASTER: When you know codes are clear and want speed
            var fasterOptions = new BarcodeReaderOptions
            {
                Speed = ReadingSpeed.Faster
            };
            var fasterResults = BarcodeReader.Read(testFile, fasterOptions);
            Console.WriteLine($"Faster mode found: {fasterResults.Count()} barcodes");

            // The point: You control the trade-off based on your needs
        }
    }
}

// ============================================================
// BATCH PROCESSING PERFORMANCE
// ============================================================

namespace BatchPerformance
{
    using IronBarcode;

    /// <summary>
    /// For batch document processing, total throughput matters more than
    /// per-frame speed. IronBarcode's architecture optimizes for this.
    /// </summary>
    public class BatchProcessingDemo
    {
        /// <summary>
        /// Process a batch of PDF documents efficiently.
        /// </summary>
        public static void ProcessPdfBatch(string[] pdfPaths)
        {
            var stopwatch = Stopwatch.StartNew();
            var allResults = new List<string>();

            // IronBarcode handles PDFs natively
            // No PDF rendering library required
            // No page-by-page image conversion overhead

            foreach (string pdfPath in pdfPaths)
            {
                var results = BarcodeReader.Read(pdfPath);

                foreach (var result in results)
                {
                    allResults.Add($"{Path.GetFileName(pdfPath)}: {result.Value}");
                }
            }

            stopwatch.Stop();

            Console.WriteLine($"Processed {pdfPaths.Length} PDFs");
            Console.WriteLine($"Found {allResults.Count} barcodes");
            Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average per PDF: {stopwatch.ElapsedMilliseconds / pdfPaths.Length}ms");

            // For batch jobs running overnight, a few hundred milliseconds
            // per document doesn't matter. What matters:
            // 1. Did we find all the barcodes? (accuracy)
            // 2. Did we handle damaged codes? (reliability)
            // 3. Did the job complete successfully? (robustness)
        }

        /// <summary>
        /// The Dynamsoft equivalent would require:
        /// </summary>
        public static void DynamsoftPdfBatchComparison()
        {
            /*
             * To process PDFs with Dynamsoft, you would need:
             *
             * 1. Add PdfiumViewer or similar package:
             *    dotnet add package PdfiumViewer
             *
             * 2. For each PDF:
             *    - Load with PdfiumViewer
             *    - Loop through pages
             *    - Render each page to bitmap (memory overhead)
             *    - Convert bitmap to byte array
             *    - Pass to Dynamsoft
             *    - Dispose resources carefully
             *
             * 3. The code would be approximately:
             *
             * using (var pdfDoc = PdfDocument.Load(pdfPath))
             * {
             *     for (int page = 0; page < pdfDoc.PageCount; page++)
             *     {
             *         using (var image = pdfDoc.Render(page, 300, 300, true))
             *         using (var ms = new MemoryStream())
             *         {
             *             image.Save(ms, ImageFormat.Png);
             *             byte[] bytes = ms.ToArray();
             *             TextResult[] results = reader.DecodeFileInMemory(bytes, "");
             *             // Process results...
             *         }
             *     }
             * }
             *
             * This overhead adds up for large batches:
             * - Memory allocation for each rendered page
             * - PNG encoding/decoding overhead
             * - Additional library dependency
             *
             * IronBarcode's native PDF support eliminates all of this.
             */
        }
    }
}

// ============================================================
// WHEN SPEED MATTERS vs WHEN ACCURACY MATTERS
// ============================================================

public static class UseCase Guidance
{
    /*
     * SPEED-CRITICAL SCENARIOS (Consider Dynamsoft)
     * =============================================
     *
     * 1. Mobile apps with live camera preview
     *    - Users expect instant feedback
     *    - They can reposition if scan fails
     *    - Speed directly impacts user experience
     *
     * 2. Point-of-sale checkout
     *    - High volume, fast-paced environment
     *    - Cashiers need instant confirmation
     *    - Retry is easy (re-scan the item)
     *
     * 3. Event ticket scanning
     *    - Lines of people waiting
     *    - Gate throughput matters
     *    - Phones/tickets can be repositioned easily
     *
     * 4. Real-time inventory counting
     *    - Workers with handheld scanners
     *    - Continuous scanning session
     *    - Immediate visual/audio feedback needed
     *
     *
     * ACCURACY-CRITICAL SCENARIOS (Consider IronBarcode)
     * ==================================================
     *
     * 1. Invoice/document processing
     *    - Barcodes are printed, not displayed on screens
     *    - Print quality varies
     *    - Missing a barcode causes data integrity issues
     *
     * 2. Medical specimen tracking
     *    - Labels on tubes may be scratched or faded
     *    - Missing a scan has serious consequences
     *    - Accuracy >> speed
     *
     * 3. Legal document management
     *    - Documents may be old, damaged, or poorly scanned
     *    - Compliance requires capturing all codes
     *    - Speed is not a factor for batch processing
     *
     * 4. Shipping label processing
     *    - Thermal labels fade over time
     *    - Labels get scratched during handling
     *    - Wrong tracking number = lost package
     *
     * 5. Historical document digitization
     *    - Old documents with degraded barcodes
     *    - One-time processing, not real-time
     *    - Maximum accuracy required
     *
     *
     * THE HONEST RECOMMENDATION
     * =========================
     *
     * Don't choose based on benchmark numbers alone.
     * Choose based on what your application actually needs.
     *
     * Building a mobile camera scanner? Dynamsoft's speed advantage
     * will make your users happier.
     *
     * Processing documents on a server? IronBarcode's accuracy and
     * native PDF support will make your operations more reliable.
     *
     * Need both? Use both. That's not compromise - that's smart engineering.
     */
}

// Workaround for C# keyword
public static class UseCaseGuidance
{
    // See comments above
}

// ============================================================
// MAIN: Quick Demonstration
// ============================================================

public class PerformanceComparisonDemo
{
    public static void Main()
    {
        Console.WriteLine("Performance Comparison: Dynamsoft vs IronBarcode");
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();

        Console.WriteLine("HONEST ASSESSMENT:");
        Console.WriteLine("- Dynamsoft IS faster for real-time camera scanning");
        Console.WriteLine("- IronBarcode prioritizes accuracy for documents");
        Console.WriteLine("- Choose based on YOUR use case, not benchmarks");
        Console.WriteLine();

        Console.WriteLine("SPEED-CRITICAL? (mobile camera, POS, events)");
        Console.WriteLine("  -> Consider Dynamsoft");
        Console.WriteLine();

        Console.WriteLine("ACCURACY-CRITICAL? (documents, PDFs, damaged codes)");
        Console.WriteLine("  -> Consider IronBarcode");
        Console.WriteLine();

        Console.WriteLine("NEED BOTH? (mobile app + backend processing)");
        Console.WriteLine("  -> Use both - that's good architecture");
    }
}
