---
title: "How to Customize Barcode Appearance in C#"
reading_time: "10 min"
difficulty: "intermediate"
last_updated: "2026-01-23"
category: "generate-barcodes"
---

# How to Customize Barcode Appearance in C#

IronBarcode customizes barcode appearance with colors, margins, annotations, and sizing options for branding and integration requirements. This tutorial demonstrates styling barcodes with the fluent API for readable, chainable customization code.

For basic barcode generation concepts, see [Generate Your First Barcode](./generate-barcode-basic.md).

---

## Prerequisites

Before starting, ensure you have:

- .NET 6 or later (or .NET Framework 4.6.2+)
- IronBarcode NuGet package installed
- Visual Studio 2022 or VS Code

Install IronBarcode via NuGet Package Manager or CLI:

```bash
dotnet add package IronBarcode
```

Note: IronBarcode uses the `IronSoftware.Drawing.Color` namespace for cross-platform color support.

---

## Step 1: Change Barcode Colors

Apply custom colors to barcode bars and background for branding or visual integration with your application design.

```csharp
using IronBarCode;
using IronSoftware.Drawing;

// Generate barcode with custom colors
var barcode = BarcodeWriter.CreateBarcode("PRODUCT-001", BarcodeEncoding.Code128);

// Set barcode bars to dark blue
barcode.ChangeBarCodeColor(Color.DarkBlue);

// Set background to light gray
barcode.ChangeBackgroundColor(Color.LightGray);

// Save the styled barcode
barcode.SaveAsPng("colored-barcode.png");
```

The `ChangeBarCodeColor()` method sets the color of the barcode bars themselves, while `ChangeBackgroundColor()` controls the background color surrounding the bars.

**Important:** Maintain sufficient contrast between barcode and background colors for reliable scanning. Dark bars on light backgrounds work best. Avoid low-contrast combinations like light blue on white or dark gray on black.

---

## Step 2: Set Margins (Quiet Zone)

Add white space around barcodes to ensure reliable scanning. The quiet zone prevents scanner confusion from adjacent text or graphics.

```csharp
using IronBarCode;

// Generate barcode
var barcode = BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128);

// Add 20 pixels of margin on all sides
barcode.SetMargins(20);

// Save with margins
barcode.SaveAsPng("barcode-with-margins.png");
```

The `SetMargins()` method with a single parameter applies the same margin to all four sides. For asymmetric margins, use the four-parameter overload:

```csharp
// Set different margins for each side (top, right, bottom, left)
barcode.SetMargins(10, 20, 10, 20);
```

The quiet zone (margin) is critical for barcode scanning reliability. Most barcode standards specify minimum quiet zone requirements:

| Format | Minimum Quiet Zone |
|--------|-------------------|
| **Code128** | 10x (10 times the width of the narrowest bar) |
| **Code39** | 10x |
| **EAN-13** | 7x minimum, 11x recommended |
| **QR Code** | 4 modules (cells) width |

For print applications at 300 DPI, margins of 15-20 pixels typically provide adequate quiet zones for most formats.

---

## Step 3: Add Text Annotations

Display human-readable text above or below barcodes for product names, SKUs, descriptions, or other contextual information.

```csharp
using IronBarCode;

// Generate barcode
var barcode = BarcodeWriter.CreateBarcode("SKU-2024-001", BarcodeEncoding.Code128);

// Add text above the barcode
barcode.AddAnnotationTextAboveBarcode("Product Name: Widget Pro");

// Add text below the barcode
barcode.AddAnnotationTextBelowBarcode("SKU-2024-001");

// Save annotated barcode
barcode.SaveAsPng("annotated-barcode.png");
```

Annotations appear in a default font and size. Customize the annotation appearance with font options:

```csharp
using IronBarCode;
using IronSoftware.Drawing;

var barcode = BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128);

// Customize annotation font and size
barcode.AddAnnotationTextAboveBarcode("Heavy Equipment", FontTypes.Arial, 14);
barcode.AddAnnotationTextBelowBarcode("Serial: 12345", FontTypes.Arial, 12);

barcode.SaveAsPng("custom-annotation.png");
```

Common use cases for annotations:
- **Product labels:** Product name above, SKU below
- **Shipping labels:** Destination above, tracking number below
- **Asset tracking:** Asset description above, ID number below
- **Inventory tags:** Category above, bin location below

---

## Step 4: Resize and Scale Barcodes

Control barcode dimensions for specific printing requirements or display constraints.

```csharp
using IronBarCode;

// Generate barcode
var barcode = BarcodeWriter.CreateBarcode("RESIZE-TEST", BarcodeEncoding.Code128);

// Set exact dimensions (400x150 pixels)
barcode.ResizeTo(400, 150);

// Save at specific size
barcode.SaveAsPng("resized-barcode.png");
```

The `ResizeTo()` method sets the exact width and height in pixels. For proportional scaling, use the `ChangeSize()` method with a scaling factor:

```csharp
// Scale barcode to 200% of original size
var largeBarcode = BarcodeWriter.CreateBarcode("SCALE-TEST", BarcodeEncoding.Code128);
largeBarcode.ChangeSize(2.0);
largeBarcode.SaveAsPng("large-barcode.png");

// Scale barcode to 50% of original size
var smallBarcode = BarcodeWriter.CreateBarcode("SCALE-TEST", BarcodeEncoding.Code128);
smallBarcode.ChangeSize(0.5);
smallBarcode.SaveAsPng("small-barcode.png");
```

**Print resolution considerations:**

For professional printing, target these resolutions:
- **Desktop printers:** 300 DPI minimum
- **Commercial printing:** 600 DPI recommended
- **Thermal label printers:** 203 DPI (standard) or 300 DPI (high quality)

Calculate barcode size for print:
- **1 inch barcode at 300 DPI:** 300 pixels width
- **2 inch barcode at 300 DPI:** 600 pixels width
- **0.5 inch barcode at 300 DPI:** 150 pixels width

For display on screens or web applications, 96 DPI is standard, so a 200-pixel barcode displays at approximately 2 inches on typical monitors.

---

## Step 5: Advanced Styling with Fluent API

Chain multiple styling methods together for clean, readable customization code.

```csharp
using IronBarCode;
using IronSoftware.Drawing;

// Generate and style barcode with method chaining
var barcode = BarcodeWriter.CreateBarcode("STYLED-001", BarcodeEncoding.Code128)
    .ChangeBarCodeColor(Color.Navy)
    .ChangeBackgroundColor(Color.WhiteSmoke)
    .SetMargins(15)
    .AddAnnotationTextAboveBarcode("Premium Product")
    .AddAnnotationTextBelowBarcode("STYLED-001")
    .ResizeTo(500, 200);

// Save the fully styled barcode
barcode.SaveAsPng("fully-styled-barcode.png");
```

The fluent API pattern enables concise styling without intermediate variable assignments. Each method returns the barcode object, allowing immediate chaining of the next styling operation.

This approach is particularly useful when creating multiple styled barcodes with similar formatting:

```csharp
// Create a styling function
GeneratedBarcode StyleProductBarcode(string data)
{
    return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ChangeBarCodeColor(Color.Black)
        .ChangeBackgroundColor(Color.White)
        .SetMargins(20)
        .ResizeTo(400, 150);
}

// Apply consistent styling to multiple barcodes
var barcode1 = StyleProductBarcode("PROD-001").SaveAsPng("product-001.png");
var barcode2 = StyleProductBarcode("PROD-002").SaveAsPng("product-002.png");
var barcode3 = StyleProductBarcode("PROD-003").SaveAsPng("product-003.png");
```

---

## Step 6: Style QR Codes

Apply color and sizing customizations to QR codes while maintaining scannability.

```csharp
using IronBarCode;
using IronSoftware.Drawing;

// Generate and style QR code
var qrCode = BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode)
    .ChangeBarCodeColor(Color.DarkBlue)
    .ChangeBackgroundColor(Color.White)
    .SetMargins(10)
    .ResizeTo(300, 300);

qrCode.SaveAsPng("styled-qr-code.png");
```

**QR code color considerations:**

QR codes are more sensitive to color contrast than 1D barcodes because smartphone cameras must distinguish individual cells in the matrix. Follow these guidelines:

- **High contrast is critical:** Use dark foreground on light background
- **Avoid mid-tone colors:** Medium gray on light gray may fail to scan
- **Test on target devices:** Verify QR codes scan on intended smartphones
- **Consider lighting:** Glossy surfaces or poor lighting reduce scan reliability

Safe color combinations:
- Black on white (best)
- Dark blue on white
- Dark green on white
- Black on light yellow

Risky combinations (test thoroughly):
- Light colors on white
- Dark colors on dark backgrounds
- Red on green or green on red (color blindness)

For logo embedding and advanced QR code customization, see [How to Generate QR Codes in C#](./qr-code-generation.md).

---

## Complete Working Example

Here's the complete code demonstrating barcode styling techniques:

```csharp
using System;
using IronBarCode;
using IronSoftware.Drawing;

namespace BarcodeStyleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example 1: Custom colors
            Console.WriteLine("Generating colored barcode...");
            var coloredBarcode = BarcodeWriter.CreateBarcode("COLOR-001", BarcodeEncoding.Code128);
            coloredBarcode.ChangeBarCodeColor(Color.DarkBlue);
            coloredBarcode.ChangeBackgroundColor(Color.LightGray);
            coloredBarcode.SaveAsPng("colored-barcode.png");
            Console.WriteLine("✓ Saved: colored-barcode.png");

            // Example 2: Margins
            Console.WriteLine("\nGenerating barcode with margins...");
            var marginBarcode = BarcodeWriter.CreateBarcode("MARGIN-001", BarcodeEncoding.Code128);
            marginBarcode.SetMargins(20);
            marginBarcode.SaveAsPng("margin-barcode.png");
            Console.WriteLine("✓ Saved: margin-barcode.png");

            // Example 3: Annotations
            Console.WriteLine("\nGenerating annotated barcode...");
            var annotatedBarcode = BarcodeWriter.CreateBarcode("SKU-2024-001", BarcodeEncoding.Code128);
            annotatedBarcode.AddAnnotationTextAboveBarcode("Premium Widget");
            annotatedBarcode.AddAnnotationTextBelowBarcode("SKU-2024-001");
            annotatedBarcode.SaveAsPng("annotated-barcode.png");
            Console.WriteLine("✓ Saved: annotated-barcode.png");

            // Example 4: Resizing
            Console.WriteLine("\nGenerating resized barcode...");
            var resizedBarcode = BarcodeWriter.CreateBarcode("RESIZE-001", BarcodeEncoding.Code128);
            resizedBarcode.ResizeTo(500, 200);
            resizedBarcode.SaveAsPng("resized-barcode.png");
            Console.WriteLine("✓ Saved: resized-barcode.png");

            // Example 5: Fluent API - Full styling
            Console.WriteLine("\nGenerating fully styled barcode...");
            var styledBarcode = BarcodeWriter.CreateBarcode("STYLE-001", BarcodeEncoding.Code128)
                .ChangeBarCodeColor(Color.Navy)
                .ChangeBackgroundColor(Color.WhiteSmoke)
                .SetMargins(15)
                .AddAnnotationTextAboveBarcode("Deluxe Product")
                .AddAnnotationTextBelowBarcode("STYLE-001")
                .ResizeTo(500, 200);
            styledBarcode.SaveAsPng("styled-barcode.png");
            Console.WriteLine("✓ Saved: styled-barcode.png");

            // Example 6: Scaled barcode
            Console.WriteLine("\nGenerating scaled barcode...");
            var scaledBarcode = BarcodeWriter.CreateBarcode("SCALE-001", BarcodeEncoding.Code128);
            scaledBarcode.ChangeSize(2.5); // 250% scale
            scaledBarcode.SaveAsPng("scaled-barcode.png");
            Console.WriteLine("✓ Saved: scaled-barcode.png");

            // Example 7: Styled QR code
            Console.WriteLine("\nGenerating styled QR code...");
            var styledQR = BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode)
                .ChangeBarCodeColor(Color.DarkBlue)
                .ChangeBackgroundColor(Color.White)
                .SetMargins(10)
                .ResizeTo(300, 300);
            styledQR.SaveAsPng("styled-qr.png");
            Console.WriteLine("✓ Saved: styled-qr.png");

            // Example 8: Product label template
            Console.WriteLine("\nGenerating product label template...");
            GeneratedBarcode CreateProductLabel(string sku, string productName)
            {
                return BarcodeWriter.CreateBarcode(sku, BarcodeEncoding.Code128)
                    .ChangeBarCodeColor(Color.Black)
                    .ChangeBackgroundColor(Color.White)
                    .SetMargins(20)
                    .AddAnnotationTextAboveBarcode(productName, FontTypes.Arial, 14)
                    .AddAnnotationTextBelowBarcode(sku, FontTypes.Arial, 12)
                    .ResizeTo(400, 150);
            }

            CreateProductLabel("PROD-A001", "Widget Alpha").SaveAsPng("product-a001.png");
            CreateProductLabel("PROD-B002", "Widget Beta").SaveAsPng("product-b002.png");
            CreateProductLabel("PROD-C003", "Widget Gamma").SaveAsPng("product-c003.png");
            Console.WriteLine("✓ Saved: product-a001.png, product-b002.png, product-c003.png");

            Console.WriteLine("\n========================================");
            Console.WriteLine("All styled barcodes generated successfully!");
            Console.WriteLine("========================================");
        }
    }
}
```

**Download:** [Complete code file](../../code-examples/tutorials/barcode-styling.cs)

---

## Next Steps

Now that you understand barcode styling and customization, explore these related tutorials:

- **[How to Generate QR Codes in C#](./qr-code-generation.md)** - QR-specific styling including logo embedding
- **[Generate Your First Barcode](./generate-barcode-basic.md)** - Learn the foundational barcode generation concepts
- **[Barcode Format Types Guide](./barcode-format-types.md)** - Understand which formats work best for different use cases

Return to [All Tutorials](./README.md)

---

Last verified: January 2026

*Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [Iron Software](https://ironsoftware.com)*
