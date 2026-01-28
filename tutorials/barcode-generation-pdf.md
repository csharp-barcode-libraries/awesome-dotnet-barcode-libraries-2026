---
title: "How to Generate Barcodes in PDF Documents in C#"
reading_time: "8 min"
difficulty: "intermediate"
last_updated: "2026-01-26"
category: "generate-barcodes"
---

# How to Generate Barcodes in PDF Documents in C#

Generate PDF documents with embedded barcodes using IronBarcode's native PDF support. This tutorial demonstrates direct PDF creation with `SaveAsPdf`, adding barcodes to existing PDFs with IronPDF integration, and batch label generation for shipping and inventory systems.

---

## Prerequisites

Before starting, ensure you have:

- .NET 6 or later (or .NET Framework 4.6.2+)
- IronBarcode NuGet package installed
- Visual Studio 2022 or VS Code
- IronPDF (optional, for advanced PDF manipulation)

Install IronBarcode via NuGet Package Manager or CLI:

```bash
dotnet add package IronBarcode
```

For advanced PDF features, install IronPDF:

```bash
dotnet add package IronPdf
```

---

## Step 1: Direct PDF Generation with SaveAsPdf

The simplest approach creates a PDF document containing just the barcode. This one-line method is perfect for shipping labels, simple product tags, and standalone barcode documents.

```csharp
using IronBarCode;

// Generate barcode and save directly to PDF
var barcode = BarcodeWriter.CreateBarcode("SHIP-2024-001", BarcodeEncoding.Code128);
barcode.SaveAsPdf("shipping-label.pdf");

Console.WriteLine("PDF created: shipping-label.pdf");
```

The `SaveAsPdf` method creates a PDF document sized to the barcode dimensions. IronBarcode's native PDF support means no external libraries or image-to-PDF conversion steps are required.

**When to use direct SaveAsPdf:**

- Shipping labels with only barcode identifier
- Simple product tags without additional layout
- Barcode generation for immediate printing
- Quick prototyping and testing

This approach differentiates IronBarcode from libraries like ZXing.Net, which generate only images and require a separate PDF library plus manual image embedding to create PDF documents.

---

## Step 2: Add Barcode to Existing PDF

For adding barcodes to existing PDF documents such as invoices, reports, or pre-designed forms, use IronPDF's BarcodeStamper to position barcodes at specific coordinates.

```csharp
using IronBarCode;
using IronPdf;
using IronPdf.Editing;

// Generate the barcode
var barcode = BarcodeWriter.CreateBarcode("INV-2024-12345", BarcodeEncoding.Code128);

// Load existing PDF document
var pdf = PdfDocument.FromFile("invoice-template.pdf");

// Create barcode stamper with positioning
var stamper = new BarcodeStamper(barcode)
{
    VerticalAlignment = VerticalAlignment.Top,
    HorizontalAlignment = HorizontalAlignment.Right,
    VerticalOffset = 50,  // 50 points from top
    HorizontalOffset = -50  // 50 points from right edge
};

// Apply barcode to first page
pdf.ApplyStamp(stamper, 0);  // Page index 0

// Save modified PDF
pdf.SaveAs("invoice-with-barcode.pdf");

Console.WriteLine("Barcode added to existing PDF");
```

The `BarcodeStamper` provides precise control over barcode placement on PDF pages. This is useful for:

- **Invoices:** Add invoice number barcode to header
- **Shipping documents:** Stamp tracking barcode on packing slip
- **Reports:** Embed document identifier for automated filing
- **Certificates:** Add verification barcode to official documents

The integration between IronBarcode and IronPDF is seamless because both are part of the Iron Software product suite, eliminating format conversion and compatibility issues common when combining third-party libraries.

---

## Step 3: Batch Label Generation

Generate multiple shipping labels or product tags in a single PDF document with one barcode per page. This is essential for warehouse operations, inventory management, and fulfillment centers.

```csharp
using IronBarCode;
using IronPdf;

// Generate batch of shipping labels
var shipmentIds = new[] { "SHIP-001", "SHIP-002", "SHIP-003", "SHIP-004", "SHIP-005" };

var renderer = new ChromePdfRenderer();
var pdfPages = new List<PdfDocument>();

foreach (var shipmentId in shipmentIds)
{
    // Create barcode for this shipment
    var barcode = BarcodeWriter.CreateBarcode(shipmentId, BarcodeEncoding.Code128);

    // Generate HTML template for label (can include text, styling, etc.)
    var labelHtml = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial;
                    text-align: center;
                    padding: 40px;
                }}
                h2 {{ margin: 20px 0; }}
                img {{ max-width: 400px; }}
            </style>
        </head>
        <body>
            <h2>Shipment Label</h2>
            <p><strong>{shipmentId}</strong></p>
            <img src='data:image/png;base64,{Convert.ToBase64String(barcode.ToImageBytes())}' />
        </body>
        </html>
    ";

    // Render HTML to PDF page
    var page = renderer.RenderHtmlAsPdf(labelHtml);
    pdfPages.Add(page);
}

// Merge all pages into single PDF
var mergedPdf = PdfDocument.Merge(pdfPages);
mergedPdf.SaveAs("batch-shipping-labels.pdf");

Console.WriteLine($"Generated {shipmentIds.Length} shipping labels in single PDF");
```

This approach creates professional multi-page documents where each page contains a barcode with associated text and formatting. The HTML template allows complete control over layout, styling, and additional content like company logos, addresses, or shipping instructions.

**Batch generation use cases:**

- **Warehouse operations:** Print 100 picking labels for daily orders
- **Inventory management:** Generate product tags for new stock arrival
- **Event management:** Create badge barcodes for attendees
- **Asset tracking:** Produce equipment labels for entire facility

The combination of IronBarcode's barcode generation and IronPDF's HTML-to-PDF rendering enables complex label layouts without manual PDF programming.

---

## Step 4: Advanced Layouts with Precise Positioning

For complex documents requiring multiple barcodes or specific positioning, use IronPDF's coordinate-based placement along with styled barcode generation.

```csharp
using IronBarCode;
using IronPdf;
using IronPdf.Editing;

// Create styled barcode
var barcode = BarcodeWriter.CreateBarcode("PROD-2024-XYZ", BarcodeEncoding.Code128);
barcode.AddBarcodeValueTextBelowBarcode();  // Add human-readable text

// Create new PDF document from HTML template
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf($@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial;
                padding: 40px;
            }}
            .header {{
                border-bottom: 2px solid #333;
                padding-bottom: 20px;
                margin-bottom: 30px;
            }}
            .content {{
                line-height: 1.6;
            }}
        </style>
    </head>
    <body>
        <div class='header'>
            <h1>Product Specification Sheet</h1>
            <p>Document Date: {DateTime.Now:yyyy-MM-dd}</p>
        </div>
        <div class='content'>
            <h2>Product Details</h2>
            <p><strong>Product ID:</strong> PROD-2024-XYZ</p>
            <p><strong>Category:</strong> Electronic Components</p>
            <p><strong>Warehouse Location:</strong> A-15-3</p>
        </div>
    </body>
    </html>
");

// Add barcode to specific position (bottom-right corner)
var stamper = new BarcodeStamper(barcode)
{
    VerticalAlignment = VerticalAlignment.Bottom,
    HorizontalAlignment = HorizontalAlignment.Right,
    VerticalOffset = -30,
    HorizontalOffset = -30
};

pdf.ApplyStamp(stamper);
pdf.SaveAs("product-spec-with-barcode.pdf");

Console.WriteLine("Created professional document with integrated barcode");
```

**Advanced positioning features:**

- **Multiple barcodes per page:** Apply different stampers for various identifiers
- **Page-specific placement:** Different positions for first page vs. subsequent pages
- **Rotation and scaling:** Rotate barcodes or adjust size for layout requirements
- **Watermark-style placement:** Semi-transparent barcodes as background elements

This level of control enables production-quality documents for:

- **Invoices with multiple identifiers:** Invoice number, customer ID, order reference
- **Multi-barcode labels:** Product code, batch number, expiration date
- **Security documents:** Visible and hidden barcode elements
- **Compliance documentation:** Regulatory barcodes at mandated positions

---

## Complete Working Example

Here's the complete code demonstrating all PDF generation scenarios:

```csharp
// Install: dotnet add package IronBarcode
// Install: dotnet add package IronPdf
using System;
using System.Collections.Generic;
using System.Linq;
using IronBarCode;
using IronPdf;
using IronPdf.Editing;

namespace BarcodeGenerationPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Barcode PDF Generation Examples ===\n");

            // Scenario 1: Simple direct PDF generation
            SimpleDirectPDF();

            // Scenario 2: Add barcode to existing PDF
            AddToExistingPDF();

            // Scenario 3: Batch label generation
            BatchLabelGeneration();

            // Scenario 4: Advanced layout with positioning
            AdvancedLayout();
        }

        static void SimpleDirectPDF()
        {
            Console.WriteLine("--- Direct PDF Generation ---");

            var barcode = BarcodeWriter.CreateBarcode("SHIP-2024-001", BarcodeEncoding.Code128);
            barcode.SaveAsPdf("simple-label.pdf");

            Console.WriteLine("Created: simple-label.pdf\n");
        }

        static void AddToExistingPDF()
        {
            Console.WriteLine("--- Add to Existing PDF ---");

            // Generate barcode
            var barcode = BarcodeWriter.CreateBarcode("INV-2024-12345", BarcodeEncoding.Code128);

            // Create sample PDF (in real scenario, load existing file)
            var renderer = new ChromePdfRenderer();
            var pdf = renderer.RenderHtmlAsPdf("<h1>Invoice Template</h1><p>Content here...</p>");

            // Stamp barcode in top-right corner
            var stamper = new BarcodeStamper(barcode)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalOffset = 50,
                HorizontalOffset = -50
            };

            pdf.ApplyStamp(stamper);
            pdf.SaveAs("invoice-with-barcode.pdf");

            Console.WriteLine("Created: invoice-with-barcode.pdf\n");
        }

        static void BatchLabelGeneration()
        {
            Console.WriteLine("--- Batch Label Generation ---");

            var shipmentIds = new[] { "SHIP-001", "SHIP-002", "SHIP-003", "SHIP-004", "SHIP-005" };
            var renderer = new ChromePdfRenderer();
            var pdfPages = new List<PdfDocument>();

            foreach (var shipmentId in shipmentIds)
            {
                var barcode = BarcodeWriter.CreateBarcode(shipmentId, BarcodeEncoding.Code128);

                var labelHtml = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial; text-align: center; padding: 60px; }}
                            h2 {{ margin: 20px 0; font-size: 24px; }}
                            p {{ font-size: 18px; margin: 10px 0; }}
                            img {{ max-width: 400px; margin: 20px 0; }}
                        </style>
                    </head>
                    <body>
                        <h2>Shipping Label</h2>
                        <p><strong>Shipment ID:</strong> {shipmentId}</p>
                        <img src='data:image/png;base64,{Convert.ToBase64String(barcode.ToImageBytes())}' />
                        <p>Scan for tracking information</p>
                    </body>
                    </html>
                ";

                var page = renderer.RenderHtmlAsPdf(labelHtml);
                pdfPages.Add(page);
            }

            var mergedPdf = PdfDocument.Merge(pdfPages);
            mergedPdf.SaveAs("batch-shipping-labels.pdf");

            Console.WriteLine($"Created: batch-shipping-labels.pdf ({shipmentIds.Length} pages)\n");
        }

        static void AdvancedLayout()
        {
            Console.WriteLine("--- Advanced Layout ---");

            var productBarcode = BarcodeWriter.CreateBarcode("PROD-2024-XYZ", BarcodeEncoding.Code128);
            productBarcode.AddBarcodeValueTextBelowBarcode();

            var renderer = new ChromePdfRenderer();
            var pdf = renderer.RenderHtmlAsPdf($@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial; padding: 40px; }}
                        .header {{ border-bottom: 2px solid #333; padding-bottom: 20px; margin-bottom: 30px; }}
                        .content {{ line-height: 1.8; }}
                        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
                        td {{ padding: 8px; border: 1px solid #ddd; }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h1>Product Specification Sheet</h1>
                        <p>Date: {DateTime.Now:yyyy-MM-dd}</p>
                    </div>
                    <div class='content'>
                        <h2>Product Information</h2>
                        <table>
                            <tr><td><strong>Product ID</strong></td><td>PROD-2024-XYZ</td></tr>
                            <tr><td><strong>Category</strong></td><td>Electronic Components</td></tr>
                            <tr><td><strong>Location</strong></td><td>Warehouse A-15-3</td></tr>
                            <tr><td><strong>Status</strong></td><td>In Stock</td></tr>
                        </table>
                    </div>
                </body>
                </html>
            ");

            // Position barcode in bottom-right
            var stamper = new BarcodeStamper(productBarcode)
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalOffset = -30,
                HorizontalOffset = -30
            };

            pdf.ApplyStamp(stamper);
            pdf.SaveAs("product-spec-with-barcode.pdf");

            Console.WriteLine("Created: product-spec-with-barcode.pdf\n");
        }
    }
}
```

**Download:** [Complete code file](../code-examples/tutorials/barcode-generation-pdf.cs)

---

## IronPDF Integration Benefits

IronBarcode and IronPDF work seamlessly together as part of the Iron Software product suite. This integration provides:

**Advantages over separate libraries:**

- **No format conversion:** Barcodes transfer directly to PDF without intermediate steps
- **Consistent API design:** Similar patterns and conventions across both libraries
- **Type compatibility:** Native support for Iron types without adapters
- **Single vendor support:** Unified documentation and technical assistance

**Competitor comparison:**

Libraries like ZXing.Net generate only bitmap images and require:
1. Generate barcode as PNG/JPEG image
2. Install separate PDF library (iTextSharp, PdfSharp, etc.)
3. Write image-to-PDF conversion code
4. Handle coordinate systems and positioning manually
5. Manage dependencies from multiple vendors

IronBarcode's native `SaveAsPdf` eliminates steps 2-5 for simple use cases. The IronPDF integration provides professional features without the complexity.

---

## Next Steps

Now that you understand PDF barcode generation, explore these related tutorials:

- **[Barcode Styling and Customization](./barcode-styling.md)** - Style barcodes before adding to PDFs (colors, margins, annotations)
- **[QR Code Generation](./qr-code-generation.md)** - Generate QR codes for PDF documents with logo embedding
- **[Batch Processing with Multi-Threading](./batch-processing.md)** - Generate thousands of PDF labels efficiently

Return to [All Tutorials](./README.md)

---

Last verified: January 2026

*Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [Iron Software](https://ironsoftware.com)*
