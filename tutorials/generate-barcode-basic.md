---
title: "How to Generate Barcodes in C#"
reading_time: "5 min"
difficulty: "beginner"
last_updated: "2026-01-23"
category: "generate-barcodes"
---

# How to Generate Barcodes in C#

IronBarcode generates barcodes with a single line of code for common formats including Code128, Code39, EAN-13, and UPC-A. This tutorial demonstrates creating barcodes and saving them in multiple image formats.

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

---

## Step 1: Generate Your First Barcode

Create a Code128 barcode with the simplest possible API. Code128 is a general-purpose format supporting alphanumeric characters and is widely used in logistics and inventory management.

```csharp
using IronBarCode;

// Generate Code128 barcode
var barcode = BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128);

// Save as PNG image
barcode.SaveAsPng("barcode.png");
```

The `BarcodeWriter.CreateBarcode()` method accepts the data to encode and the barcode format from the `BarcodeEncoding` enum. The returned barcode object provides methods to save in multiple image formats.

---

## Step 2: Generate Code39 Barcodes

Code39 supports alphanumeric characters and is commonly used in defense, automotive, and healthcare industries. Code39 requires uppercase letters and has specific character limitations.

```csharp
using IronBarCode;

// Generate Code39 barcode (alphanumeric support)
var barcode = BarcodeWriter.CreateBarcode("ABC-123", BarcodeEncoding.Code39);
barcode.SaveAsPng("code39.png");

// Code39 automatically converts to uppercase
var uppercase = BarcodeWriter.CreateBarcode("item-456", BarcodeEncoding.Code39);
uppercase.SaveAsPng("code39-uppercase.png");
```

Code39 accepts numbers, uppercase letters, and limited special characters (dash, period, space, dollar sign, slash, plus, percent). Lowercase letters are automatically converted to uppercase. Code39 barcodes are larger than Code128 for equivalent data.

---

## Step 3: Generate EAN-13 and UPC-A (Retail Barcodes)

EAN-13 and UPC-A are retail product barcodes used worldwide. EAN-13 is the international standard (used in Europe and most countries), while UPC-A is primarily used in North America.

```csharp
using IronBarCode;

// Generate EAN-13 barcode (13 digits total, last digit is checksum)
// Input 12 digits, checksum calculated automatically
var ean13 = BarcodeWriter.CreateBarcode("501234567890", BarcodeEncoding.EAN13);
ean13.SaveAsPng("ean13.png");

// Generate UPC-A barcode (12 digits total, last digit is checksum)
// Input 11 digits, checksum calculated automatically
var upcA = BarcodeWriter.CreateBarcode("01234567890", BarcodeEncoding.UPCA);
upcA.SaveAsPng("upca.png");
```

IronBarcode automatically calculates and appends the checksum digit for EAN-13 and UPC-A barcodes. EAN-13 requires 12 input digits (13th is checksum), and UPC-A requires 11 input digits (12th is checksum).

**Retail barcode requirements:**

| Format | Input Digits | Total Digits | Primary Use |
|--------|--------------|--------------|-------------|
| EAN-13 | 12 | 13 (with checksum) | International retail products |
| UPC-A  | 11 | 12 (with checksum) | North American retail products |

EAN-13 and UPC-A encode numbers only. For alphanumeric product codes, use Code128 instead.

---

## Step 4: Save in Different Formats

IronBarcode supports saving barcodes as PNG, JPEG, GIF, TIFF, and BMP images. Choose the format based on your use case.

```csharp
using IronBarCode;

var barcode = BarcodeWriter.CreateBarcode("PRODUCT-001", BarcodeEncoding.Code128);

// Save in different image formats
barcode.SaveAsPng("barcode.png");
barcode.SaveAsJpeg("barcode.jpg");
barcode.SaveAsGif("barcode.gif");
barcode.SaveAsTiff("barcode.tiff");
```

**Format selection guide:**

| Format | Best For | File Size | Quality Loss |
|--------|----------|-----------|--------------|
| PNG | Web use, transparency support | Small | None (lossless) |
| JPEG | Photos, web (no transparency) | Smaller | Some (lossy) |
| GIF | Simple graphics, animations | Small | None for barcodes |
| TIFF | Print, archival | Larger | None (lossless) |

PNG is recommended for most barcode applications due to lossless compression and broad compatibility. Avoid JPEG for barcodes when possible, as compression artifacts can reduce scan reliability.

---

## Complete Working Example

Here's the complete code generating all four barcode types and saving in multiple formats:

```csharp
// Install: dotnet add package IronBarcode
using System;
using IronBarCode;

namespace BarcodeGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example 1: Code128 (general purpose, alphanumeric)
            var code128 = BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128);
            code128.SaveAsPng("code128.png");
            Console.WriteLine("Generated Code128 barcode");

            // Example 2: Code39 (alphanumeric, uppercase)
            var code39 = BarcodeWriter.CreateBarcode("ORDER-ABC-789", BarcodeEncoding.Code39);
            code39.SaveAsPng("code39.png");
            Console.WriteLine("Generated Code39 barcode");

            // Example 3: EAN-13 (retail, international)
            var ean13 = BarcodeWriter.CreateBarcode("501234567890", BarcodeEncoding.EAN13);
            ean13.SaveAsPng("ean13.png");
            Console.WriteLine("Generated EAN-13 barcode");

            // Example 4: UPC-A (retail, North America)
            var upcA = BarcodeWriter.CreateBarcode("01234567890", BarcodeEncoding.UPCA);
            upcA.SaveAsPng("upca.png");
            Console.WriteLine("Generated UPC-A barcode");

            // Example 5: Save in multiple formats
            var multi = BarcodeWriter.CreateBarcode("MULTI-FORMAT", BarcodeEncoding.Code128);
            multi.SaveAsPng("output.png");
            multi.SaveAsJpeg("output.jpg");
            multi.SaveAsGif("output.gif");
            multi.SaveAsTiff("output.tiff");
            Console.WriteLine("Saved barcode in PNG, JPEG, GIF, and TIFF formats");

            Console.WriteLine("\nAll barcodes generated successfully");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

**Download:** [Complete code file](../code-examples/tutorials/generate-barcode-basic.cs)

---

## Next Steps

Now that you understand basic barcode generation, explore these related tutorials:

- **[Understanding Barcode Format Types](./barcode-format-types.md)** - Learn when to use different barcode formats
- **[How to Generate QR Codes with Logos](./qr-code-generation.md)** - Create 2D barcodes with custom branding
- **[Barcode Styling and Customization](./barcode-styling.md)** - Add colors, resize, and customize appearance
- **[How to Read Barcodes from Images](./read-barcodes-images.md)** - Read and verify generated barcodes

Return to [All Tutorials](./README.md)

---

Last verified: January 2026

*Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [Iron Software](https://ironsoftware.com)*
