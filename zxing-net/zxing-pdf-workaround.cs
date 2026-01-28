/**
 * PDF Processing: ZXing.Net Workaround vs IronBarcode Native Support
 *
 * This example demonstrates the critical difference in PDF handling:
 * - ZXing.Net has NO native PDF support
 * - Must use external libraries (PdfiumViewer, Ghostscript, etc.)
 * - Requires page-by-page rendering and temp file management
 * - IronBarcode reads PDFs directly with one line
 *
 * Key Differences:
 * - ZXing.Net: 30+ lines, external dependencies, temp files
 * - IronBarcode: 1 line, no external dependencies
 *
 * NuGet Packages Required:
 * - ZXing.Net: ZXing.Net, ZXing.Net.Bindings.Windows, PdfiumViewer (~20MB)
 * - IronBarcode: IronBarcode (PDF support built-in)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

// ============================================================================
// THE PROBLEM: PDF Barcode Processing
// ============================================================================

namespace PdfProcessingProblem
{
    /// <summary>
    /// Real-world scenario: Processing PDF documents with embedded barcodes.
    /// This is extremely common in enterprise environments.
    /// </summary>
    public class PdfBarcodeScenarios
    {
        /// <summary>
        /// Common PDF barcode use cases that ZXing.Net cannot handle directly.
        /// </summary>
        public static void ShowCommonScenarios()
        {
            Console.WriteLine("=== COMMON PDF BARCODE SCENARIOS ===");
            Console.WriteLine();

            Console.WriteLine("1. Invoice Processing");
            Console.WriteLine("   - Vendors send PDF invoices with barcoded PO numbers");
            Console.WriteLine("   - Automated matching to purchase orders");
            Console.WriteLine();

            Console.WriteLine("2. Shipping Labels");
            Console.WriteLine("   - Carriers provide PDF shipping labels");
            Console.WriteLine("   - Extract tracking numbers for system integration");
            Console.WriteLine();

            Console.WriteLine("3. ID Verification");
            Console.WriteLine("   - Scanned IDs arrive as PDF documents");
            Console.WriteLine("   - PDF417 barcodes contain encoded data");
            Console.WriteLine();

            Console.WriteLine("4. Healthcare Records");
            Console.WriteLine("   - Patient documents in PDF format");
            Console.WriteLine("   - Barcoded patient/specimen identifiers");
            Console.WriteLine();

            Console.WriteLine("5. Legal Documents");
            Console.WriteLine("   - Court filings with barcoded case numbers");
            Console.WriteLine("   - Contracts with document tracking barcodes");
            Console.WriteLine();

            Console.WriteLine("ZXing.Net CANNOT process any of these directly.");
            Console.WriteLine("You must extract images from PDF first.");
        }
    }
}


// ============================================================================
// ZXING.NET APPROACH - PDF Workaround
// ============================================================================

namespace ZXingExamples
{
    using ZXing;
    using ZXing.Common;
    using ZXing.Windows.Compatibility;
    // Requires: PdfiumViewer NuGet package
    // Note: This is pseudo-code showing the pattern - actual PdfiumViewer API

    /// <summary>
    /// ZXing.Net PDF processing requires external PDF library.
    /// This demonstrates the complexity involved.
    /// </summary>
    public class ZXingPdfWorkaround
    {
        /// <summary>
        /// Read barcodes from PDF using PdfiumViewer.
        /// This is the most common ZXing.Net PDF workaround.
        /// </summary>
        public static List<string> ReadBarcodesFromPdf(string pdfPath)
        {
            // Step 1: Validate PDF exists
            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException("PDF file not found", pdfPath);
            }

            var results = new List<string>();

            // Step 2: Initialize ZXing reader with format specification
            var reader = new BarcodeReader();
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128,
                BarcodeFormat.PDF_417,
                BarcodeFormat.DATA_MATRIX
            };
            reader.Options.TryHarder = true;

            // Step 3: Load PDF document (requires PdfiumViewer)
            // using var pdfDocument = PdfDocument.Load(pdfPath);

            // Simulated page count for demonstration
            int pageCount = 5; // In reality: pdfDocument.PageCount

            // Step 4: Process each page
            for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                string tempImagePath = null;

                try
                {
                    // Step 5: Render PDF page to image
                    // DPI affects barcode detection quality
                    // 200 DPI is minimum for reliable barcode detection
                    // Higher DPI = larger image = slower processing

                    // using var pageImage = pdfDocument.Render(
                    //     pageIndex,
                    //     200, // X DPI
                    //     200, // Y DPI
                    //     PdfRenderFlags.CorrectFromDpi
                    // );

                    // Step 6: Save to temporary file
                    // ZXing needs file path or Bitmap
                    tempImagePath = Path.Combine(
                        Path.GetTempPath(),
                        $"pdf_page_{Guid.NewGuid()}.png"
                    );

                    // pageImage.Save(tempImagePath, ImageFormat.Png);

                    // Step 7: Load image and decode
                    using (var bitmap = new Bitmap(tempImagePath))
                    {
                        var decoded = reader.DecodeMultiple(bitmap);

                        if (decoded != null)
                        {
                            foreach (var result in decoded)
                            {
                                results.Add($"Page {pageIndex + 1}: {result.Text}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Step 8: Error handling per page
                    Console.WriteLine($"Error processing page {pageIndex + 1}: {ex.Message}");
                }
                finally
                {
                    // Step 9: Clean up temp file
                    if (tempImagePath != null && File.Exists(tempImagePath))
                    {
                        try
                        {
                            File.Delete(tempImagePath);
                        }
                        catch
                        {
                            // Temp file cleanup failure - not critical
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Read barcodes from specific PDF pages only.
        /// More complex page selection logic.
        /// </summary>
        public static List<string> ReadBarcodesFromPages(string pdfPath, int[] pageNumbers)
        {
            var results = new List<string>();

            var reader = new BarcodeReader();
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128
            };

            // using var pdfDocument = PdfDocument.Load(pdfPath);

            foreach (int pageNum in pageNumbers)
            {
                // Validate page number (0-based vs 1-based)
                int pageIndex = pageNum - 1;

                // if (pageIndex < 0 || pageIndex >= pdfDocument.PageCount)
                // {
                //     Console.WriteLine($"Invalid page number: {pageNum}");
                //     continue;
                // }

                string tempPath = Path.GetTempFileName() + ".png";

                try
                {
                    // Render specific page
                    // using var pageImage = pdfDocument.Render(pageIndex, 200, 200, PdfRenderFlags.CorrectFromDpi);
                    // pageImage.Save(tempPath, ImageFormat.Png);

                    using (var bitmap = new Bitmap(tempPath))
                    {
                        var decoded = reader.DecodeMultiple(bitmap);
                        if (decoded != null)
                        {
                            results.AddRange(decoded.Select(d => d.Text));
                        }
                    }
                }
                finally
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Process PDF with memory optimization.
        /// Demonstrates complexity for large PDFs.
        /// </summary>
        public static IEnumerable<string> ReadBarcodesStreaming(string pdfPath)
        {
            var reader = new BarcodeReader();
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128,
                BarcodeFormat.PDF_417
            };

            // using var pdfDocument = PdfDocument.Load(pdfPath);

            int pageCount = 5; // pdfDocument.PageCount

            for (int i = 0; i < pageCount; i++)
            {
                string tempPath = Path.Combine(Path.GetTempPath(), $"stream_{i}_{Guid.NewGuid()}.png");

                try
                {
                    // Render page at lower DPI for initial detection
                    // using var pageImage = pdfDocument.Render(i, 150, 150, PdfRenderFlags.CorrectFromDpi);
                    // pageImage.Save(tempPath, ImageFormat.Png);

                    using (var bitmap = new Bitmap(tempPath))
                    {
                        var decoded = reader.DecodeMultiple(bitmap);

                        if (decoded != null)
                        {
                            foreach (var result in decoded)
                            {
                                yield return $"Page {i + 1}: {result.Text}";
                            }
                        }
                    }
                }
                finally
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }

                // Allow GC to clean up
                GC.Collect();
            }
        }
    }

    /// <summary>
    /// Alternative PDF approaches with other libraries.
    /// Shows the variety of workarounds developers use.
    /// </summary>
    public class AlternativePdfApproaches
    {
        /// <summary>
        /// Using Ghostscript for PDF rendering.
        /// Another common workaround.
        /// </summary>
        public static void GhostscriptApproach()
        {
            Console.WriteLine("=== GHOSTSCRIPT APPROACH ===");
            Console.WriteLine();
            Console.WriteLine("Pros:");
            Console.WriteLine("  - Free and open-source");
            Console.WriteLine("  - Good PDF compatibility");
            Console.WriteLine();
            Console.WriteLine("Cons:");
            Console.WriteLine("  - Large dependency (~50MB)");
            Console.WriteLine("  - AGPL license (commercial license expensive)");
            Console.WriteLine("  - Complex installation on Docker/Linux");
            Console.WriteLine("  - Spawns external processes");
        }

        /// <summary>
        /// Using ImageMagick for PDF conversion.
        /// </summary>
        public static void ImageMagickApproach()
        {
            Console.WriteLine("=== IMAGEMAGICK APPROACH ===");
            Console.WriteLine();
            Console.WriteLine("Pros:");
            Console.WriteLine("  - Handles many formats");
            Console.WriteLine();
            Console.WriteLine("Cons:");
            Console.WriteLine("  - Requires Ghostscript anyway");
            Console.WriteLine("  - Another large dependency");
            Console.WriteLine("  - Native library management");
            Console.WriteLine("  - Security considerations");
        }

        /// <summary>
        /// Using iTextSharp for PDF extraction.
        /// </summary>
        public static void iTextSharpApproach()
        {
            Console.WriteLine("=== ITEXTSHARP APPROACH ===");
            Console.WriteLine();
            Console.WriteLine("Pros:");
            Console.WriteLine("  - Pure .NET library");
            Console.WriteLine("  - Good PDF parsing");
            Console.WriteLine();
            Console.WriteLine("Cons:");
            Console.WriteLine("  - AGPL license (commercial license required)");
            Console.WriteLine("  - Extracts embedded images only");
            Console.WriteLine("  - Cannot render PDF pages as images");
            Console.WriteLine("  - Still need ZXing for barcode detection");
        }
    }

    /// <summary>
    /// Demonstrates error scenarios with PDF workarounds.
    /// </summary>
    public class PdfWorkaroundErrors
    {
        /// <summary>
        /// Common error scenarios when using PDF workarounds.
        /// </summary>
        public static void DocumentCommonErrors()
        {
            Console.WriteLine("=== COMMON PDF WORKAROUND ERRORS ===");
            Console.WriteLine();

            Console.WriteLine("1. Corrupted PDF");
            Console.WriteLine("   Error: PdfDocument.Load throws exception");
            Console.WriteLine("   Impact: Entire document fails");
            Console.WriteLine();

            Console.WriteLine("2. Password-protected PDF");
            Console.WriteLine("   Error: Cannot open without password");
            Console.WriteLine("   Impact: No barcodes extracted");
            Console.WriteLine();

            Console.WriteLine("3. Large PDF (100+ pages)");
            Console.WriteLine("   Error: OutOfMemoryException");
            Console.WriteLine("   Impact: Need streaming approach");
            Console.WriteLine();

            Console.WriteLine("4. DPI mismatch");
            Console.WriteLine("   Error: Barcodes too small to detect");
            Console.WriteLine("   Impact: False negatives");
            Console.WriteLine();

            Console.WriteLine("5. Temp file permission");
            Console.WriteLine("   Error: Cannot write to temp folder");
            Console.WriteLine("   Impact: Processing fails");
            Console.WriteLine();

            Console.WriteLine("6. Native library not found");
            Console.WriteLine("   Error: DllNotFoundException");
            Console.WriteLine("   Impact: PDF rendering fails completely");
        }
    }
}


// ============================================================================
// IRONBARCODE APPROACH - Native PDF Support
// ============================================================================

namespace IronBarcodeExamples
{
    using IronBarCode;

    /// <summary>
    /// IronBarcode has native PDF support.
    /// No external libraries, no temp files, no page rendering.
    /// </summary>
    public class IronBarcodePdfNative
    {
        /// <summary>
        /// Read all barcodes from a PDF document.
        /// One line, all formats detected automatically.
        /// </summary>
        public static List<string> ReadBarcodesFromPdf(string pdfPath)
        {
            // That's it. One line. All pages. All formats.
            var results = BarcodeReader.Read(pdfPath);
            return results.Select(r => r.Text).ToList();
        }

        /// <summary>
        /// Read barcodes with page information.
        /// </summary>
        public static void ReadWithPageInfo(string pdfPath)
        {
            var results = BarcodeReader.Read(pdfPath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page: {barcode.PageNumber}");
                Console.WriteLine($"Format: {barcode.BarcodeType}");
                Console.WriteLine($"Value: {barcode.Text}");
                Console.WriteLine($"Position: ({barcode.X}, {barcode.Y})");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Read barcodes from specific pages.
        /// </summary>
        public static List<string> ReadFromSpecificPages(string pdfPath, params int[] pageNumbers)
        {
            // Specify which pages to process
            var results = BarcodeReader.Read(pdfPath, pageNumbers);
            return results.Select(r => r.Text).ToList();
        }

        /// <summary>
        /// Process PDF with additional options.
        /// </summary>
        public static List<BarcodeResult> ReadWithOptions(string pdfPath)
        {
            // Create read options for PDF
            var options = new BarcodeReaderOptions
            {
                ExpectMultipleBarcodes = true,
                ExpectBarcodeTypes = BarcodeEncoding.All // Optional: filter types
            };

            var results = BarcodeReader.Read(pdfPath, options);
            return results.ToList();
        }

        /// <summary>
        /// Batch process multiple PDF documents.
        /// </summary>
        public static Dictionary<string, List<string>> ProcessMultiplePdfs(string[] pdfPaths)
        {
            var allResults = new Dictionary<string, List<string>>();

            foreach (var pdfPath in pdfPaths)
            {
                var barcodes = BarcodeReader.Read(pdfPath);
                allResults[pdfPath] = barcodes.Select(b => b.Text).ToList();
            }

            return allResults;
        }
    }

    /// <summary>
    /// Demonstrates IronBarcode handling all PDF scenarios
    /// that require workarounds in ZXing.Net.
    /// </summary>
    public class IronBarcodePdfScenarios
    {
        /// <summary>
        /// Invoice processing - automatic barcode extraction.
        /// </summary>
        public static void ProcessInvoice(string invoicePdfPath)
        {
            Console.WriteLine("Processing invoice PDF...");

            var barcodes = BarcodeReader.Read(invoicePdfPath);

            foreach (var barcode in barcodes)
            {
                // Automatic format detection handles any barcode type
                Console.WriteLine($"Found {barcode.BarcodeType} on page {barcode.PageNumber}");
                Console.WriteLine($"Value: {barcode.Text}");
            }
        }

        /// <summary>
        /// Shipping label processing.
        /// </summary>
        public static string GetTrackingNumber(string shippingLabelPdf)
        {
            var barcodes = BarcodeReader.Read(shippingLabelPdf);

            // First barcode is typically the tracking number
            return barcodes.FirstOrDefault()?.Text;
        }

        /// <summary>
        /// ID verification from scanned document.
        /// </summary>
        public static void VerifyIdentification(string idDocumentPdf)
        {
            var barcodes = BarcodeReader.Read(idDocumentPdf);

            // PDF417 on driver's licenses/IDs contains encoded data
            var idBarcode = barcodes
                .FirstOrDefault(b => b.BarcodeType == BarcodeEncoding.PDF417);

            if (idBarcode != null)
            {
                Console.WriteLine("ID barcode found");
                Console.WriteLine($"Encoded data: {idBarcode.Text}");
            }
        }

        /// <summary>
        /// Multi-page document processing.
        /// </summary>
        public static void ProcessLargeDocument(string documentPdf)
        {
            var barcodes = BarcodeReader.Read(documentPdf);

            var byPage = barcodes.GroupBy(b => b.PageNumber);

            foreach (var pageGroup in byPage)
            {
                Console.WriteLine($"Page {pageGroup.Key}: {pageGroup.Count()} barcodes");

                foreach (var barcode in pageGroup)
                {
                    Console.WriteLine($"  - {barcode.BarcodeType}: {barcode.Text}");
                }
            }
        }
    }
}


// ============================================================================
// DIRECT COMPARISON
// ============================================================================

namespace ComparisonExamples
{
    /// <summary>
    /// Direct code comparison for PDF processing.
    /// </summary>
    public class PdfProcessingComparison
    {
        /// <summary>
        /// Compare lines of code required.
        /// </summary>
        public static void CompareCodeComplexity()
        {
            Console.WriteLine("=== PDF PROCESSING CODE COMPARISON ===");
            Console.WriteLine();

            Console.WriteLine("ZXing.Net Approach:");
            Console.WriteLine("  - External dependency: PdfiumViewer (~20MB)");
            Console.WriteLine("  - Code lines: 40-60 lines for basic PDF processing");
            Console.WriteLine("  - Temp file management required");
            Console.WriteLine("  - DPI configuration required");
            Console.WriteLine("  - Error handling per page");
            Console.WriteLine("  - Format specification still required");
            Console.WriteLine();

            Console.WriteLine("IronBarcode Approach:");
            Console.WriteLine("  - No external dependency");
            Console.WriteLine("  - Code lines: 1 line");
            Console.WriteLine("  - No temp files");
            Console.WriteLine("  - Automatic DPI optimization");
            Console.WriteLine("  - Built-in error handling");
            Console.WriteLine("  - Automatic format detection");
        }

        /// <summary>
        /// Compare dependencies.
        /// </summary>
        public static void CompareDependencies()
        {
            Console.WriteLine("=== DEPENDENCY COMPARISON ===");
            Console.WriteLine();

            Console.WriteLine("ZXing.Net PDF Processing:");
            Console.WriteLine("  Packages required:");
            Console.WriteLine("    - ZXing.Net (~2MB)");
            Console.WriteLine("    - ZXing.Net.Bindings.Windows (~1MB)");
            Console.WriteLine("    - PdfiumViewer (~20MB)");
            Console.WriteLine("    - PdfiumViewer.Native.x64 (~50MB)");
            Console.WriteLine("  Total: ~73MB");
            Console.WriteLine();

            Console.WriteLine("IronBarcode PDF Processing:");
            Console.WriteLine("  Packages required:");
            Console.WriteLine("    - IronBarcode (~30MB)");
            Console.WriteLine("  Total: ~30MB");
            Console.WriteLine("  PDF support included.");
        }

        /// <summary>
        /// Side-by-side code for same operation.
        /// </summary>
        public static void SideBySideCode()
        {
            Console.WriteLine("=== SIDE-BY-SIDE CODE ===");
            Console.WriteLine();

            Console.WriteLine("Task: Extract all barcodes from a 10-page PDF");
            Console.WriteLine();

            Console.WriteLine("ZXing.Net (with PdfiumViewer):");
            Console.WriteLine("----------------------------------");
            Console.WriteLine(@"
var results = new List<string>();
var reader = new BarcodeReader();
reader.Options.PossibleFormats = new List<BarcodeFormat> { ... };
using var pdf = PdfDocument.Load(pdfPath);
for (int i = 0; i < pdf.PageCount; i++)
{
    var temp = Path.GetTempFileName() + "".png"";
    try
    {
        using var img = pdf.Render(i, 200, 200, PdfRenderFlags.CorrectFromDpi);
        img.Save(temp, ImageFormat.Png);
        using var bmp = new Bitmap(temp);
        var decoded = reader.DecodeMultiple(bmp);
        if (decoded != null) results.AddRange(decoded.Select(d => d.Text));
    }
    finally { File.Delete(temp); }
}
");
            Console.WriteLine();

            Console.WriteLine("IronBarcode:");
            Console.WriteLine("----------------------------------");
            Console.WriteLine(@"
var results = BarcodeReader.Read(pdfPath).Select(r => r.Text).ToList();
");
        }
    }
}
