/**
 * Performance Comparison: Dynamsoft Barcode Reader vs IronBarcode
 *
 * This example honestly demonstrates the performance characteristics of both
 * libraries in their respective areas of strength.
 *
 * Key Points:
 * - Dynamsoft IS faster on raw decoding, especially for streaming inputs
 * - IronBarcode prioritises document accuracy and deployment simplicity
 * - Speed benchmarks without context are misleading
 * - Choose based on your actual workload, not benchmark numbers
 *
 * NuGet Packages Required:
 * - Dynamsoft: Dynamsoft.DotNet.BarcodeReader.Bundle (v11.x)
 * - IronBarcode: BarCode (NuGet package id `BarCode`; namespace `IronBarCode`)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

// ============================================================
// HONEST ASSESSMENT: Dynamsoft Speed Advantage
// ============================================================

namespace PerformanceComparison
{
    /// <summary>
    /// Dynamsoft excels at raw decoding speed. This is a fact, not marketing.
    /// When milliseconds matter for user experience, Dynamsoft delivers.
    /// </summary>
    public static class SpeedReality
    {
        /*
         * HONEST ACKNOWLEDGMENT
         * =====================
         *
         * In real-time camera scanning and high-throughput streaming
         * scenarios, Dynamsoft is genuinely faster. This isn't a weakness
         * we're trying to spin — it's their core competency.
         *
         * Dynamsoft has spent years optimising for:
         * - Sub-100ms frame decoding
         * - Motion blur compensation
         * - Autofocus prediction
         * - DPM and deformed-barcode recognition
         * - Video buffer management
         *
         * If you're building a mobile / MAUI scanning app and need instant
         * feedback when users point their camera at a barcode, Dynamsoft
         * deserves serious consideration.
         *
         * However, speed isn't the whole story. The right answer depends on
         * what you're actually doing — a benchmark showing "Library A
         * decodes a frame in 50ms vs Library B in 150ms" tells you nothing
         * about which library will better serve your overnight document
         * processing workflow.
         */
    }

    /// <summary>
    /// Benchmark context matters more than raw numbers.
    /// </summary>
    public class BenchmarkContext
    {
        // SCENARIO 1: Real-time camera scanning (30 fps target)
        // ------------------------------------------------------
        // Dynamsoft: tuned for sub-100ms per frame
        // IronBarcode: not optimised for sustained streaming
        // Winner: Dynamsoft (this is their specialty)

        // SCENARIO 2: Processing 1000 PDF invoices overnight
        // ---------------------------------------------------
        // Per-frame speed irrelevant — total batch time + integration cost matters
        // Dynamsoft: typically requires PDF render step (PdfiumViewer etc.)
        // IronBarcode: native PDF support, fewer moving parts
        // Winner: IronBarcode (less code, less to deploy)

        // SCENARIO 3: Reading damaged shipping labels
        // -------------------------------------------
        // Both libraries have strong damaged-code handling. Dynamsoft has
        // explicit DPM modes; IronBarcode has ML-assisted error correction.
        // Either may win depending on the exact label degradation mode.

        // SCENARIO 4: High-volume real-time scanning (warehouse scanners)
        // ----------------------------------------------------------------
        // Dedicated hardware with camera input, instant feedback expected
        // Winner: Dynamsoft (purpose-built for this)
    }
}

// ============================================================
// DOCUMENT ACCURACY DEMONSTRATION
// ============================================================

namespace DocumentAccuracy
{
    using IronBarCode;

    /// <summary>
    /// For document processing, accuracy and deployment simplicity often
    /// matter more than raw decoding speed. IronBarcode's static API and
    /// native PDF support handle these workloads cleanly.
    /// </summary>
    public class AccuracyDemonstration
    {
        /// <summary>
        /// Process damaged document barcodes.
        /// </summary>
        public static void ProcessDamagedDocuments()
        {
            // Real-world document barcodes suffer from:
            // - Thermal printer fading
            // - Label scratches and tears
            // - Scanner artefacts
            // - Poor print quality
            // - Smudges and stains

            var options = new BarcodeReaderOptions
            {
                // Detailed mode spends more time analysing unclear codes
                Speed = ReadingSpeed.Detailed,

                // Find all barcodes on multi-code documents
                ExpectMultipleBarcodes = true
            };

            // IronBarcode's processing can reconstruct:
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
                    Console.WriteLine($"  Format: {result.BarcodeType}");
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
    using IronBarCode;

    /// <summary>
    /// For batch document processing, total throughput matters more than
    /// per-frame speed. IronBarcode's architecture optimises for this.
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
        }

        /// <summary>
        /// The Dynamsoft equivalent typically requires:
        /// </summary>
        public static void DynamsoftPdfBatchComparison()
        {
            /*
             * To process PDFs with Dynamsoft on Linux containers, you would
             * normally need:
             *
             * 1. Add a PDF render package (e.g. PdfiumViewer, Magick.NET).
             *
             * 2. For each PDF:
             *    - Load with the PDF library
             *    - Loop through pages
             *    - Render each page to a bitmap
             *    - Save / encode as PNG bytes
             *    - Pass to CaptureVisionRouter.Capture()
             *    - Dispose resources carefully
             *
             * 3. The code looks roughly like:
             *
             * using (var pdfDoc = PdfDocument.Load(pdfPath))
             * using (var router = new CaptureVisionRouter())
             * {
             *     for (int page = 0; page < pdfDoc.PageCount; page++)
             *     {
             *         using (var image = pdfDoc.Render(page, 300, 300, true))
             *         using (var ms = new MemoryStream())
             *         {
             *             image.Save(ms, ImageFormat.Png);
             *             byte[] bytes = ms.ToArray();
             *             CapturedResult result = router.Capture(bytes,
             *                 PresetTemplate.PT_READ_BARCODES);
             *             // Process result.GetDecodedBarcodesResult()...
             *         }
             *     }
             * }
             *
             * This overhead adds up for large batches:
             * - Memory allocation for each rendered page
             * - PNG encoding/decoding overhead
             * - Additional library dependency to deploy and patch
             *
             * IronBarcode's native PDF support eliminates all of this.
             */
        }
    }
}

// ============================================================
// WHEN SPEED MATTERS vs WHEN ACCURACY MATTERS
// ============================================================

public static class UseCaseGuidance
{
    /*
     * SPEED-CRITICAL SCENARIOS (Consider Dynamsoft)
     * =============================================
     *
     * 1. Mobile / MAUI apps with live camera preview
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
     *    - Phones / tickets can be repositioned easily
     *
     * 4. Real-time inventory counting
     *    - Workers with handheld scanners
     *    - Continuous scanning session
     *    - Immediate visual / audio feedback needed
     *
     * 5. DPM / deformed-barcode pipelines
     *    - Manufacturing, automotive, electronics traceability
     *    - Dynamsoft has explicit DPM modes
     *
     *
     * DOCUMENT / DEPLOYMENT-FOCUSED SCENARIOS (Consider IronBarcode)
     * ==============================================================
     *
     * 1. Invoice / document processing
     *    - Barcodes are printed, not displayed on screens
     *    - Print quality varies
     *    - Native PDF input avoids an extra render pipeline
     *
     * 2. Medical specimen tracking (server side)
     *    - Labels on tubes may be scratched or faded
     *    - Missing a scan has serious consequences
     *
     * 3. Legal document management
     *    - Documents may be old, damaged, or poorly scanned
     *    - Compliance requires capturing all codes
     *    - Speed is not a factor for batch processing
     *
     * 4. Shipping label processing on backend services
     *    - Thermal labels fade over time
     *    - Wrong tracking number = lost package
     *
     * 5. Air-gapped or strict-egress environments
     *    - No outbound licence-server traffic permitted
     *    - Single-key local activation simplifies compliance
     *
     *
     * THE HONEST RECOMMENDATION
     * =========================
     *
     * Don't choose based on benchmark numbers alone. Choose based on what
     * your application actually needs.
     *
     * Building a mobile camera scanner or a high-throughput streaming
     * pipeline? Dynamsoft's speed advantage is real.
     *
     * Processing documents on a server, especially behind a strict egress
     * policy? IronBarcode's native PDF support and offline licensing make
     * the deployment cheaper and shorter.
     *
     * Need both? Use both. That's not compromise — that's smart engineering.
     */
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
        Console.WriteLine("- Dynamsoft IS faster for real-time / streaming decoding");
        Console.WriteLine("- IronBarcode prioritises document workflows and deployment simplicity");
        Console.WriteLine("- Choose based on YOUR use case, not benchmarks");
        Console.WriteLine();

        Console.WriteLine("SPEED-CRITICAL? (mobile camera, POS, events, DPM)");
        Console.WriteLine("  -> Consider Dynamsoft");
        Console.WriteLine();

        Console.WriteLine("DOCUMENT / DEPLOYMENT-CRITICAL? (PDFs, air-gapped, perpetual pricing)");
        Console.WriteLine("  -> Consider IronBarcode");
        Console.WriteLine();

        Console.WriteLine("NEED BOTH? (mobile app + backend processing)");
        Console.WriteLine("  -> Use both - that's good architecture");
    }
}
