// Install-Package IronBarcode
// Install-Package IronPdf
using System;
using System.Collections.Generic;
using System.Linq;
using IronBarCode;
using IronPdf;
using IronPdf.Editing;

namespace BarcodeGenerationPDF
{
    /// <summary>
    /// Demonstrates barcode PDF generation with IronBarcode and IronPDF
    /// Scenarios: direct PDF, stamp on existing, batch labels, advanced layouts
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== IronBarcode PDF Generation Demo ===");
            Console.WriteLine("Native PDF support without image conversion\n");

            // Scenario 1: Simple direct PDF generation
            SimpleDirectPDF();

            // Scenario 2: Add barcode to existing PDF
            AddToExistingPDF();

            // Scenario 3: Batch label generation
            BatchLabelGeneration();

            // Scenario 4: Advanced layout with positioning
            AdvancedLayout();

            Console.WriteLine("\nAll PDF examples completed successfully!");
        }

        /// <summary>
        /// Direct PDF generation using SaveAsPdf
        /// Simplest approach for standalone barcode documents
        /// </summary>
        static void SimpleDirectPDF()
        {
            Console.WriteLine("--- Direct PDF Generation (SaveAsPdf) ---");

            // One-line barcode to PDF creation
            var barcode = BarcodeWriter.CreateBarcode("SHIP-2024-001", BarcodeEncoding.Code128);
            barcode.SaveAsPdf("simple-label.pdf");

            Console.WriteLine("Created: simple-label.pdf");
            Console.WriteLine("Use case: Shipping labels, simple product tags\n");
        }

        /// <summary>
        /// Add barcode to existing PDF document
        /// Uses IronPDF BarcodeStamper for precise positioning
        /// </summary>
        static void AddToExistingPDF()
        {
            Console.WriteLine("--- Add Barcode to Existing PDF ---");

            // Generate barcode
            var barcode = BarcodeWriter.CreateBarcode("INV-2024-12345", BarcodeEncoding.Code128);

            // Create sample PDF (in production, use PdfDocument.FromFile("existing.pdf"))
            var renderer = new ChromePdfRenderer();
            var pdf = renderer.RenderHtmlAsPdf(@"
                <html>
                <body style='font-family: Arial; padding: 40px;'>
                    <h1>Invoice</h1>
                    <p>Invoice Number: INV-2024-12345</p>
                    <p>Date: " + DateTime.Now.ToString("yyyy-MM-dd") + @"</p>
                    <hr />
                    <p>Invoice content here...</p>
                </body>
                </html>
            ");

            // Stamp barcode in top-right corner
            var stamper = new BarcodeStamper(barcode)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalOffset = 50,    // 50 points from top
                HorizontalOffset = -50  // 50 points from right edge
            };

            pdf.ApplyStamp(stamper);
            pdf.SaveAs("invoice-with-barcode.pdf");

            Console.WriteLine("Created: invoice-with-barcode.pdf");
            Console.WriteLine("Use case: Invoices, reports, existing documents\n");
        }

        /// <summary>
        /// Generate batch of shipping labels in single PDF
        /// One barcode per page with professional layout
        /// </summary>
        static void BatchLabelGeneration()
        {
            Console.WriteLine("--- Batch Label Generation ---");

            var shipmentIds = new[] { "SHIP-001", "SHIP-002", "SHIP-003", "SHIP-004", "SHIP-005" };
            var renderer = new ChromePdfRenderer();
            var pdfPages = new List<PdfDocument>();

            foreach (var shipmentId in shipmentIds)
            {
                // Generate barcode for this shipment
                var barcode = BarcodeWriter.CreateBarcode(shipmentId, BarcodeEncoding.Code128);

                // Create HTML template for shipping label
                var labelHtml = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                text-align: center;
                                padding: 60px;
                            }}
                            .header {{
                                margin-bottom: 30px;
                            }}
                            h2 {{
                                margin: 20px 0;
                                font-size: 28px;
                                color: #333;
                            }}
                            .shipment-id {{
                                font-size: 20px;
                                font-weight: bold;
                                margin: 15px 0;
                                color: #000;
                            }}
                            .barcode-image {{
                                max-width: 450px;
                                margin: 30px auto;
                                display: block;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 14px;
                                color: #666;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='header'>
                            <h2>Shipping Label</h2>
                        </div>
                        <div class='shipment-id'>
                            Shipment ID: {shipmentId}
                        </div>
                        <img class='barcode-image' src='data:image/png;base64,{Convert.ToBase64String(barcode.ToImageBytes())}' />
                        <div class='footer'>
                            <p>Scan barcode for tracking information</p>
                            <p>Generated: {DateTime.Now:yyyy-MM-dd HH:mm}</p>
                        </div>
                    </body>
                    </html>
                ";

                // Render HTML to PDF page
                var page = renderer.RenderHtmlAsPdf(labelHtml);
                pdfPages.Add(page);
            }

            // Merge all pages into single PDF document
            var mergedPdf = PdfDocument.Merge(pdfPages);
            mergedPdf.SaveAs("batch-shipping-labels.pdf");

            Console.WriteLine($"Created: batch-shipping-labels.pdf ({shipmentIds.Length} pages)");
            Console.WriteLine("Use case: Warehouse operations, inventory labels\n");
        }

        /// <summary>
        /// Advanced layout with multiple elements and precise barcode positioning
        /// Professional document with integrated barcode
        /// </summary>
        static void AdvancedLayout()
        {
            Console.WriteLine("--- Advanced Layout with Positioning ---");

            // Create styled barcode with human-readable text
            var productBarcode = BarcodeWriter.CreateBarcode("PROD-2024-XYZ", BarcodeEncoding.Code128);
            productBarcode.AddBarcodeValueTextBelowBarcode();

            var renderer = new ChromePdfRenderer();
            var pdf = renderer.RenderHtmlAsPdf($@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            padding: 40px;
                        }}
                        .header {{
                            border-bottom: 3px solid #333;
                            padding-bottom: 20px;
                            margin-bottom: 30px;
                        }}
                        .header h1 {{
                            margin: 0 0 10px 0;
                            color: #000;
                        }}
                        .header p {{
                            margin: 5px 0;
                            color: #666;
                        }}
                        .content {{
                            line-height: 1.8;
                        }}
                        .content h2 {{
                            color: #333;
                            margin-top: 30px;
                        }}
                        table {{
                            width: 100%;
                            border-collapse: collapse;
                            margin: 20px 0;
                        }}
                        td {{
                            padding: 10px;
                            border: 1px solid #ddd;
                        }}
                        td:first-child {{
                            background-color: #f5f5f5;
                            font-weight: bold;
                            width: 200px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h1>Product Specification Sheet</h1>
                        <p>Document Date: {DateTime.Now:yyyy-MM-dd}</p>
                        <p>Department: Inventory Management</p>
                    </div>
                    <div class='content'>
                        <h2>Product Information</h2>
                        <table>
                            <tr>
                                <td>Product ID</td>
                                <td>PROD-2024-XYZ</td>
                            </tr>
                            <tr>
                                <td>Product Name</td>
                                <td>Electronic Component Kit</td>
                            </tr>
                            <tr>
                                <td>Category</td>
                                <td>Electronic Components</td>
                            </tr>
                            <tr>
                                <td>Warehouse Location</td>
                                <td>Building A, Shelf 15, Position 3</td>
                            </tr>
                            <tr>
                                <td>Current Stock</td>
                                <td>245 units</td>
                            </tr>
                            <tr>
                                <td>Status</td>
                                <td>In Stock</td>
                            </tr>
                        </table>
                        <h2>Additional Notes</h2>
                        <p>This product requires special handling. Store in climate-controlled environment.</p>
                    </div>
                </body>
                </html>
            ");

            // Position barcode in bottom-right corner
            var stamper = new BarcodeStamper(productBarcode)
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalOffset = -30,   // 30 points from bottom
                HorizontalOffset = -30  // 30 points from right
            };

            pdf.ApplyStamp(stamper);
            pdf.SaveAs("product-spec-with-barcode.pdf");

            Console.WriteLine("Created: product-spec-with-barcode.pdf");
            Console.WriteLine("Use case: Product specs, compliance docs, certificates\n");
        }
    }
}
