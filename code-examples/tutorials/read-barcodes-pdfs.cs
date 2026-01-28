// Install: dotnet add package IronBarcode

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
            Console.WriteLine("IronBarcode PDF Reading Examples");
            Console.WriteLine("========================================\n");

            // Example 1: Basic PDF reading
            Console.WriteLine("Example 1: Basic PDF Reading");
            Console.WriteLine("----------------------------------------");

            try
            {
                var basicResults = BarcodeReader.ReadPdf("sample-invoice.pdf");

                if (basicResults.Any())
                {
                    Console.WriteLine($"✓ Found {basicResults.Count()} barcode(s):\n");

                    foreach (var barcode in basicResults)
                    {
                        Console.WriteLine($"  Value: {barcode.Text}");
                        Console.WriteLine($"  Format: {barcode.BarcodeType}");
                        Console.WriteLine($"  Page: {barcode.PageNumber}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("⚠ No barcodes detected in PDF");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Could not read sample-invoice.pdf: {ex.Message}");
            }

            // Example 2: Specific page reading
            Console.WriteLine("\nExample 2: Read from Specific Pages");
            Console.WriteLine("----------------------------------------");

            try
            {
                var pageOptions = new PdfBarcodeReaderOptions
                {
                    PageNumbers = new int[] { 1, 2, 3 }
                };

                var pageResults = BarcodeReader.ReadPdf("multi-page-document.pdf", pageOptions);
                Console.WriteLine($"✓ Scanned pages 1-3 only");
                Console.WriteLine($"✓ Found {pageResults.Count()} barcode(s)");

                foreach (var barcode in pageResults)
                {
                    Console.WriteLine($"  Page {barcode.PageNumber}: {barcode.Text}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Could not read multi-page-document.pdf: {ex.Message}");
            }

            // Example 3: Multi-page processing with grouping
            Console.WriteLine("\nExample 3: Multi-Page Document Processing");
            Console.WriteLine("----------------------------------------");

            try
            {
                var multiResults = BarcodeReader.ReadPdf("shipping-manifest.pdf");
                var barcodesByPage = multiResults.GroupBy(b => b.PageNumber);

                Console.WriteLine($"✓ Processed {barcodesByPage.Count()} page(s) with barcodes");

                foreach (var pageGroup in barcodesByPage)
                {
                    Console.WriteLine($"\n  Page {pageGroup.Key}:");
                    foreach (var barcode in pageGroup)
                    {
                        Console.WriteLine($"    • {barcode.BarcodeType}: {barcode.Text}");
                    }
                }

                Console.WriteLine($"\n  Total barcodes: {multiResults.Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Could not read shipping-manifest.pdf: {ex.Message}");
            }

            // Example 4: High-quality rendering for small barcodes
            Console.WriteLine("\nExample 4: High-Quality PDF Rendering");
            Console.WriteLine("----------------------------------------");

            try
            {
                var qualityOptions = new PdfBarcodeReaderOptions
                {
                    Dpi = 300,        // High resolution rendering
                    Scale = 2.0       // 200% image scaling
                };

                var qualityResults = BarcodeReader.ReadPdf("small-barcodes.pdf", qualityOptions);
                Console.WriteLine($"✓ Rendered at 300 DPI with 2x scaling");
                Console.WriteLine($"✓ Detected {qualityResults.Count()} barcode(s)");

                foreach (var barcode in qualityResults)
                {
                    Console.WriteLine($"  {barcode.Text} (confidence: {barcode.Confidence}%)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Could not read small-barcodes.pdf: {ex.Message}");
            }

            // Example 5: Batch processing multiple PDFs
            Console.WriteLine("\nExample 5: Batch Processing Multiple PDFs");
            Console.WriteLine("----------------------------------------");

            string pdfDirectory = "./sample-pdfs";

            if (Directory.Exists(pdfDirectory))
            {
                var pdfFiles = Directory.GetFiles(pdfDirectory, "*.pdf");
                Console.WriteLine($"✓ Found {pdfFiles.Length} PDF file(s) in {pdfDirectory}\n");

                int totalBarcodes = 0;
                int successfulFiles = 0;

                foreach (var pdfPath in pdfFiles)
                {
                    try
                    {
                        var batchResults = BarcodeReader.ReadPdf(pdfPath);
                        int barcodeCount = batchResults.Count();
                        totalBarcodes += barcodeCount;
                        successfulFiles++;

                        Console.WriteLine($"  ✓ {Path.GetFileName(pdfPath)}: {barcodeCount} barcode(s)");

                        foreach (var barcode in batchResults)
                        {
                            Console.WriteLine($"      Page {barcode.PageNumber}: {barcode.Text}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ✗ {Path.GetFileName(pdfPath)}: {ex.Message}");
                    }
                }

                Console.WriteLine($"\n  Summary:");
                Console.WriteLine($"  • Files processed: {successfulFiles}/{pdfFiles.Length}");
                Console.WriteLine($"  • Total barcodes: {totalBarcodes}");
            }
            else
            {
                Console.WriteLine($"⚠ Directory not found: {pdfDirectory}");
                Console.WriteLine($"  Create the directory and add PDF files to test batch processing");
            }

            // Example 6: Page-specific validation
            Console.WriteLine("\nExample 6: Page-Specific Validation");
            Console.WriteLine("----------------------------------------");

            try
            {
                var results = BarcodeReader.ReadPdf("labeled-document.pdf");

                // Validate that each page has exactly one barcode
                var pagesWithBarcodes = results.GroupBy(b => b.PageNumber);

                foreach (var pageGroup in pagesWithBarcodes)
                {
                    int barcodeCount = pageGroup.Count();

                    if (barcodeCount == 1)
                    {
                        Console.WriteLine($"  ✓ Page {pageGroup.Key}: Valid (1 barcode)");
                    }
                    else
                    {
                        Console.WriteLine($"  ⚠ Page {pageGroup.Key}: Warning ({barcodeCount} barcodes - expected 1)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Could not read labeled-document.pdf: {ex.Message}");
            }

            Console.WriteLine("\n========================================");
            Console.WriteLine("PDF barcode reading examples complete!");
            Console.WriteLine("========================================");
        }
    }
}
