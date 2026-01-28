/**
 * Mobile vs Server Architecture: Dynamsoft Barcode Reader vs IronBarcode
 *
 * This example demonstrates the fundamental architectural differences between
 * Dynamsoft's camera-first design and IronBarcode's document-first design.
 *
 * Key Differences:
 * - Dynamsoft: Optimized for real-time video frame processing from cameras
 * - Dynamsoft: CaptureVisionRouter architecture for continuous scanning
 * - IronBarcode: Optimized for static document and PDF processing
 * - IronBarcode: Native PDF support without additional dependencies
 *
 * Use Each When:
 * - Dynamsoft: Mobile apps, point-of-sale, real-time camera scanning
 * - IronBarcode: Server-side processing, batch documents, PDF workflows
 *
 * NuGet Packages Required:
 * - Dynamsoft: Dynamsoft.DotNet.BarcodeReader
 * - IronBarcode: IronBarcode version 2024.x+
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ============================================================
// DYNAMSOFT APPROACH: Camera/Video Stream Optimized
// ============================================================

namespace DynamsoftMobileFirst
{
    using Dynamsoft.DBR;

    /// <summary>
    /// Dynamsoft's architecture is built around video frame processing.
    /// This is their core strength for mobile and camera applications.
    /// </summary>
    public class DynamsoftVideoProcessor
    {
        private BarcodeReader _reader;

        public DynamsoftVideoProcessor(string licenseKey)
        {
            // Initialize license (required before any operations)
            int errorCode = BarcodeReader.InitLicense(licenseKey, out string errorMsg);
            if (errorCode != (int)EnumErrorCode.DBR_OK)
            {
                throw new InvalidOperationException($"License error: {errorMsg}");
            }

            _reader = new BarcodeReader();

            // Configure for speed - Dynamsoft excels at real-time decoding
            ConfigureForRealTime();
        }

        private void ConfigureForRealTime()
        {
            PublicRuntimeSettings settings = _reader.GetRuntimeSettings();

            // These settings optimize for camera scanning speed
            // Dynamsoft has fine-tuned these for mobile camera scenarios
            settings.DeblurLevel = 5;
            settings.ExpectedBarcodesCount = 1; // Optimize for single barcode
            settings.ScaleDownThreshold = 2300; // Balance quality vs speed
            settings.Timeout = 100; // Fast timeout for video frames

            _reader.UpdateRuntimeSettings(settings);
        }

        /// <summary>
        /// Process a camera frame - this is Dynamsoft's sweet spot.
        /// Designed for 30+ fps processing from mobile cameras.
        /// </summary>
        public List<string> ProcessCameraFrame(byte[] frameData, int width, int height)
        {
            var results = new List<string>();

            // Dynamsoft efficiently handles raw frame data from cameras
            // This is optimized for continuous video stream processing
            TextResult[] barcodes = _reader.DecodeBuffer(
                frameData,
                width,
                height,
                width * 3, // stride
                EnumImagePixelFormat.IPF_RGB_888,
                ""
            );

            foreach (TextResult barcode in barcodes)
            {
                results.Add(barcode.BarcodeText);
            }

            return results;
        }

        /// <summary>
        /// Process static image - works but not the primary design target.
        /// </summary>
        public List<string> ProcessStaticImage(string imagePath)
        {
            var results = new List<string>();
            TextResult[] barcodes = _reader.DecodeFile(imagePath, "");

            foreach (TextResult barcode in barcodes)
            {
                results.Add(barcode.BarcodeText);
            }

            return results;
        }

        /// <summary>
        /// Process PDF - requires external library with Dynamsoft.
        /// </summary>
        public List<string> ProcessPdf(string pdfPath)
        {
            var results = new List<string>();

            // IMPORTANT: Dynamsoft doesn't natively read PDFs
            // You need an additional PDF library like PdfiumViewer, PDFsharp, etc.
            //
            // Typical workflow:
            // 1. Load PDF with PdfiumViewer or similar
            // 2. Render each page to an image
            // 3. Pass image bytes to Dynamsoft
            // 4. Collect results

            // Pseudocode showing the complexity:
            /*
            using (var pdfDoc = PdfiumViewer.PdfDocument.Load(pdfPath))
            {
                for (int page = 0; page < pdfDoc.PageCount; page++)
                {
                    using (var image = pdfDoc.Render(page, 300, 300, true))
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Png);
                        byte[] imageBytes = ms.ToArray();

                        TextResult[] barcodes = _reader.DecodeFileInMemory(imageBytes, "");
                        foreach (var barcode in barcodes)
                        {
                            results.Add(barcode.BarcodeText);
                        }
                    }
                }
            }
            */

            Console.WriteLine("PDF processing with Dynamsoft requires additional PDF library");
            return results;
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}

// ============================================================
// IRONBARCODE APPROACH: Document/Server Processing Optimized
// ============================================================

namespace IronBarcodeDocumentFirst
{
    using IronBarcode;

    /// <summary>
    /// IronBarcode's architecture is built around document processing.
    /// Native PDF support and batch operations are first-class features.
    /// </summary>
    public class IronBarcodeDocumentProcessor
    {
        public IronBarcodeDocumentProcessor(string licenseKey)
        {
            // Simple license - no network calls, no callbacks
            IronBarcode.License.LicenseKey = licenseKey;
        }

        /// <summary>
        /// Process a PDF document natively - IronBarcode's sweet spot.
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
                    Format = barcode.Format.ToString(),
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
                    Format = barcode.Format.ToString(),
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

                // Handle rotated/skewed document scans
                UseAutoRotate = true,

                // Detect all barcodes on multi-barcode documents
                MultipleBarcodes = true
            };

            var barcodes = BarcodeReader.Read(filePaths, options);

            foreach (var barcode in barcodes)
            {
                results.Add(new BarcodeData
                {
                    Value = barcode.Value,
                    Format = barcode.Format.ToString(),
                    Confidence = 1.0 // IronBarcode provides confidence scores
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
                    Format = barcode.Format.ToString()
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
        public double Confidence { get; set; }
    }
}

// ============================================================
// REAL-WORLD SCENARIO: Invoice Processing Comparison
// ============================================================

namespace InvoiceProcessingComparison
{
    using IronBarcode;

    /// <summary>
    /// Real-world scenario: Processing 1000 PDF invoices overnight.
    /// This demonstrates where IronBarcode's document-first design excels.
    /// </summary>
    public class InvoiceProcessor
    {
        // DYNAMSOFT APPROACH:
        // Would require:
        // 1. PdfiumViewer or similar for PDF rendering
        // 2. Loop through pages, render to images
        // 3. Pass each image to Dynamsoft
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
                MultipleBarcodes = true,
                UseAutoRotate = true
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
                        BarcodeType = barcode.Format.ToString(),
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
     * - CaptureVisionRouter optimized for continuous video input
     * - Frame buffer management for camera streams
     * - Autofocus and motion blur compensation
     * - Sub-100ms decoding for real-time feedback
     * - Excellent mobile SDK integration
     *
     * Best For:
     * - Mobile apps with camera scanning
     * - Point-of-sale systems
     * - Ticket/event scanning
     * - Real-time warehouse scanning
     * - Any scenario where speed and live preview matter
     *
     * Limitations:
     * - No native PDF support
     * - License server dependency
     * - API designed for video, less intuitive for documents
     * - Per-device/per-server licensing complexity
     *
     *
     * IRONBARCODE ARCHITECTURE
     * ========================
     *
     * Strengths:
     * - Native PDF processing without dependencies
     * - Batch file operations with mixed formats
     * - ML-powered error correction for damaged documents
     * - Simple static API for server-side code
     * - Offline license validation
     *
     * Best For:
     * - Server-side document processing
     * - PDF batch operations
     * - Warehouse management (scanned documents)
     * - Invoice/receipt processing
     * - Any scenario prioritizing accuracy over frame speed
     *
     * Limitations:
     * - Not optimized for real-time camera feeds
     * - No specialized mobile SDKs
     * - Document focus may be overkill for simple image scanning
     *
     *
     * RECOMMENDATION
     * ==============
     *
     * Use Dynamsoft when:
     * - Building mobile apps with live camera scanning
     * - Real-time user feedback is critical
     * - Speed matters more than document accuracy
     *
     * Use IronBarcode when:
     * - Processing documents (especially PDFs)
     * - Running batch jobs on servers
     * - Working in air-gapped environments
     * - Damaged/poor quality document barcodes are common
     *
     * Use Both when:
     * - Mobile app (Dynamsoft) + backend processing (IronBarcode)
     * - Different teams have different requirements
     * - Some workflows need speed, others need accuracy
     */
}
