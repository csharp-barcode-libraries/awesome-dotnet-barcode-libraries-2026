/**
 * Camera-First vs Document-First: Dynamsoft Barcode Reader vs IronBarcode
 *
 * This example demonstrates the architectural differences between
 * Dynamsoft's Capture Vision pipeline (built around video and camera input)
 * and IronBarcode's document-first design.
 *
 * Note: Dynamsoft's .NET edition is fully supported on Windows (x86/x64)
 * and Linux (x64) — it is NOT mobile-only. However, the API surface and
 * tuning defaults are inherited from a video-frame pipeline shared with
 * Dynamsoft's iOS/Android/MAUI editions, which is why a server-side
 * document workload often feels more natural in IronBarcode.
 *
 * Use Each When:
 * - Dynamsoft: Mobile/MAUI camera apps, point-of-sale, real-time scanning,
 *   high-throughput server pipelines that already run a CaptureVisionRouter
 * - IronBarcode: Server-side document and PDF processing without an
 *   external PDF render step, simpler licensing, perpetual pricing
 *
 * NuGet Packages Required:
 * - Dynamsoft: Dynamsoft.DotNet.BarcodeReader.Bundle (v11.x)
 * - IronBarcode: BarCode (NuGet package id `BarCode`; namespace `IronBarCode`)
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ============================================================
// DYNAMSOFT APPROACH: Capture Vision Pipeline
// ============================================================

namespace DynamsoftMobileFirst
{
    using Dynamsoft.CVR;
    using Dynamsoft.Core;
    using Dynamsoft.DBR;
    using Dynamsoft.License;

    /// <summary>
    /// Dynamsoft's architecture is built around the CaptureVisionRouter,
    /// a single entry point optimised for streaming image inputs.
    /// </summary>
    public class DynamsoftVideoProcessor : IDisposable
    {
        private readonly CaptureVisionRouter _router;

        public DynamsoftVideoProcessor(string licenseKey)
        {
            // Initialize licence (required before any operations)
            int errorCode = LicenseManager.InitLicense(licenseKey, out string errorMsg);
            if (errorCode != (int)EnumErrorCode.EC_OK)
            {
                throw new InvalidOperationException($"License error: {errorMsg}");
            }

            _router = new CaptureVisionRouter();

            // Configure for speed — tune the simplified settings for
            // a low-latency, single-barcode camera pipeline.
            ConfigureForRealTime();
        }

        private void ConfigureForRealTime()
        {
            SimplifiedCaptureVisionSettings settings = _router.GetSimplifiedSettings(
                PresetTemplate.PT_READ_BARCODES);

            // Optimise for camera scanning speed
            settings.BarcodeSettings.ExpectedBarcodesCount = 1; // single barcode
            settings.Timeout = 100; // 100ms timeout suits a 30fps pipeline

            _router.UpdateSettings(PresetTemplate.PT_READ_BARCODES, settings);
        }

        /// <summary>
        /// Process a camera frame — Dynamsoft's sweet spot.
        /// </summary>
        public List<string> ProcessCameraFrame(byte[] frameData, int width, int height)
        {
            var results = new List<string>();

            var imageData = new ImageData
            {
                Bytes = frameData,
                Width = width,
                Height = height,
                Stride = width * 3,
                Format = EnumImagePixelFormat.IPF_RGB_888
            };

            CapturedResult capturedResult = _router.Capture(imageData,
                PresetTemplate.PT_READ_BARCODES);

            DecodedBarcodesResult barcodes = capturedResult.GetDecodedBarcodesResult();
            if (barcodes != null)
            {
                foreach (BarcodeResultItem item in barcodes.GetItems())
                {
                    results.Add(item.GetText());
                }
            }

            return results;
        }

        /// <summary>
        /// Process static image — works just as well on Windows and Linux servers.
        /// </summary>
        public List<string> ProcessStaticImage(string imagePath)
        {
            var results = new List<string>();
            CapturedResult capturedResult = _router.Capture(imagePath,
                PresetTemplate.PT_READ_BARCODES);

            DecodedBarcodesResult barcodes = capturedResult.GetDecodedBarcodesResult();
            if (barcodes != null)
            {
                foreach (BarcodeResultItem item in barcodes.GetItems())
                {
                    results.Add(item.GetText());
                }
            }

            return results;
        }

        /// <summary>
        /// Process PDF — typically requires an external PDF render step.
        /// </summary>
        public List<string> ProcessPdf(string pdfPath)
        {
            var results = new List<string>();

            // The .NET SDK can accept some PDF inputs directly on Windows,
            // but for cross-platform deployments many teams pre-render pages
            // with PdfiumViewer / PDFsharp / Magick.NET and pass each page
            // image to CaptureVisionRouter.Capture().

            // Pseudocode showing the typical pattern:
            /*
            using (var pdfDoc = PdfiumViewer.PdfDocument.Load(pdfPath))
            using (var router = new CaptureVisionRouter())
            {
                for (int page = 0; page < pdfDoc.PageCount; page++)
                {
                    using (var image = pdfDoc.Render(page, 300, 300, true))
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Png);
                        CapturedResult result = router.Capture(ms.ToArray(),
                            PresetTemplate.PT_READ_BARCODES);
                        foreach (var item in result.GetDecodedBarcodesResult().GetItems())
                            results.Add(item.GetText());
                    }
                }
            }
            */

            Console.WriteLine("PDF batch processing with Dynamsoft typically requires a PDF render library");
            return results;
        }

        public void Dispose() => _router?.Dispose();
    }
}

// ============================================================
// IRONBARCODE APPROACH: Document/Server Processing Optimized
// ============================================================

namespace IronBarcodeDocumentFirst
{
    using IronBarCode;

    /// <summary>
    /// IronBarcode's architecture is built around document processing.
    /// Native PDF support and batch operations are first-class features.
    /// </summary>
    public class IronBarcodeDocumentProcessor
    {
        public IronBarcodeDocumentProcessor(string licenseKey)
        {
            // Simple licence — no network calls, no callbacks
            IronBarCode.License.LicenseKey = licenseKey;
        }

        /// <summary>
        /// Process a PDF document natively — IronBarcode's sweet spot.
        /// No additional libraries required.
        /// </summary>
        public List<BarcodeData> ProcessPdf(string pdfPath)
        {
            var results = new List<BarcodeData>();

            // IronBarcode handles PDFs directly
            // No external PDF library required
            var barcodes = BarcodeReader.Read(pdfPath);

            foreach (var barcode in barcodes)
            {
                results.Add(new BarcodeData
                {
                    Value = barcode.Value,
                    Format = barcode.BarcodeType.ToString(),
                    PageNumber = barcode.PageNumber,
                    X = barcode.X,
                    Y = barcode.Y
                });
            }

            return results;
        }

        /// <summary>
        /// Process multiple files in a batch - mixed formats supported.
        /// </summary>
        public List<BarcodeData> ProcessBatch(string[] filePaths)
        {
            var results = new List<BarcodeData>();

            // IronBarcode accepts an array of files
            // Can mix images and PDFs in the same batch
            var barcodes = BarcodeReader.Read(filePaths);

            foreach (var barcode in barcodes)
            {
                results.Add(new BarcodeData
                {
                    Value = barcode.Value,
                    Format = barcode.BarcodeType.ToString(),
                    FileName = Path.GetFileName(barcode.BarcodeImage?.ToString() ?? "unknown"),
                    PageNumber = barcode.PageNumber
                });
            }

            return results;
        }

        /// <summary>
        /// Process with options for damaged document barcodes.
        /// ML-powered error correction for print degradation.
        /// </summary>
        public List<BarcodeData> ProcessDamagedDocuments(string[] filePaths)
        {
            var results = new List<BarcodeData>();

            var options = new BarcodeReaderOptions
            {
                // Enable detailed scanning for damaged codes
                Speed = ReadingSpeed.Detailed,

                // Detect all barcodes on multi-barcode documents
                ExpectMultipleBarcodes = true
            };

            var barcodes = BarcodeReader.Read(filePaths, options);

            foreach (var barcode in barcodes)
            {
                results.Add(new BarcodeData
                {
                    Value = barcode.Value,
                    Format = barcode.BarcodeType.ToString()
                });
            }

            return results;
        }

        /// <summary>
        /// Process a single image - works fine but PDFs are the primary strength.
        /// </summary>
        public List<BarcodeData> ProcessImage(string imagePath)
        {
            var results = new List<BarcodeData>();
            var barcodes = BarcodeReader.Read(imagePath);

            foreach (var barcode in barcodes)
            {
                results.Add(new BarcodeData
                {
                    Value = barcode.Value,
                    Format = barcode.BarcodeType.ToString()
                });
            }

            return results;
        }
    }

    public class BarcodeData
    {
        public string Value { get; set; }
        public string Format { get; set; }
        public string FileName { get; set; }
        public int PageNumber { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}

// ============================================================
// REAL-WORLD SCENARIO: Invoice Processing Comparison
// ============================================================

namespace InvoiceProcessingComparison
{
    using IronBarCode;

    /// <summary>
    /// Real-world scenario: Processing 1000 PDF invoices overnight.
    /// This demonstrates where IronBarcode's document-first design excels.
    /// </summary>
    public class InvoiceProcessor
    {
        // DYNAMSOFT APPROACH:
        // Would typically require:
        // 1. PdfiumViewer or similar for PDF rendering
        // 2. Loop through pages, render to images
        // 3. Pass each image bytes to CaptureVisionRouter.Capture()
        // 4. Manage memory for large batches
        // 5. ~50 lines of code for PDF handling alone

        // IRONBARCODE APPROACH:
        public static List<InvoiceBarcode> ProcessInvoicesWithIronBarcode(string[] pdfPaths)
        {
            var results = new List<InvoiceBarcode>();

            // Single call handles all PDFs, all pages
            var options = new BarcodeReaderOptions
            {
                Speed = ReadingSpeed.Balanced,
                ExpectMultipleBarcodes = true
            };

            foreach (var pdfPath in pdfPaths)
            {
                var barcodes = BarcodeReader.Read(pdfPath, options);

                foreach (var barcode in barcodes)
                {
                    results.Add(new InvoiceBarcode
                    {
                        InvoiceFile = Path.GetFileName(pdfPath),
                        BarcodeValue = barcode.Value,
                        BarcodeType = barcode.BarcodeType.ToString(),
                        PageNumber = barcode.PageNumber,
                        ProcessedAt = DateTime.UtcNow
                    });
                }
            }

            return results;
        }
    }

    public class InvoiceBarcode
    {
        public string InvoiceFile { get; set; }
        public string BarcodeValue { get; set; }
        public string BarcodeType { get; set; }
        public int PageNumber { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}

// ============================================================
// ARCHITECTURE COMPARISON SUMMARY
// ============================================================

public static class ArchitectureComparisonNotes
{
    /*
     * DYNAMSOFT ARCHITECTURE
     * ======================
     *
     * Strengths:
     * - CaptureVisionRouter optimised for streaming and video input
     * - Frame buffer management for camera streams
     * - Strong DPM, deformed-barcode and motion-blur handling
     * - Sub-100ms decoding for real-time feedback
     * - Shared core engine across .NET, JS, iOS, Android, MAUI
     * - Fully supported on Windows and Linux servers (not mobile-only)
     *
     * Best For:
     * - MAUI / mobile apps with camera scanning
     * - Point-of-sale systems
     * - Ticket / event scanning
     * - High-throughput server pipelines that already run CVR
     * - Any scenario where raw decoding speed matters
     *
     * Trade-offs:
     * - PDF batch processing usually wants an external render step
     * - Online activation + periodic re-check
     * - API surface designed for streaming, more verbose for one-off jobs
     * - Pricing is largely contact-sales / negotiated
     *
     *
     * IRONBARCODE ARCHITECTURE
     * ========================
     *
     * Strengths:
     * - Native PDF processing without dependencies
     * - Batch file operations with mixed formats
     * - ML-assisted error correction for damaged documents
     * - Static API, no router/instance lifecycle to manage
     * - Offline licence validation
     * - Published perpetual pricing
     *
     * Best For:
     * - Server-side document processing
     * - PDF batch operations
     * - Warehouse management (scanned documents)
     * - Invoice/receipt processing
     * - Any scenario prioritising deployment simplicity over frame rate
     *
     * Trade-offs:
     * - Not optimised for sustained 30fps camera feeds
     * - No native iOS/Android/MAUI camera SDK
     *
     *
     * RECOMMENDATION
     * ==============
     *
     * Use Dynamsoft when:
     * - Building mobile / MAUI apps with live camera scanning
     * - You need DPM or deformed-barcode handling at speed
     * - You already standardise on Capture Vision across platforms
     *
     * Use IronBarcode when:
     * - Processing documents (especially PDFs)
     * - Running batch jobs on servers
     * - Working in air-gapped environments
     * - You prefer published perpetual pricing over negotiated quotes
     *
     * Use Both when:
     * - Mobile app (Dynamsoft) + backend processing (IronBarcode)
     * - Different teams have different requirements
     */
}
