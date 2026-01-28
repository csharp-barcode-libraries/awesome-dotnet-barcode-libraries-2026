---
title: "How to Generate QR Codes in C#"
reading_time: "8 min"
difficulty: "intermediate"
last_updated: "2026-01-23"
category: "generate-barcodes"
---

# How to Generate QR Codes in C#

IronBarcode generates QR codes with customizable error correction levels and supports logo embedding for branded QR codes. This tutorial demonstrates creating QR codes for URLs, plain text, and custom data types with advanced styling options.

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

---

## Step 1: Generate a Basic QR Code

Create a QR code with the simplest possible API. QR codes store URLs, text, contact information, WiFi credentials, and other data in a two-dimensional matrix format that's scannable by smartphones.

```csharp
using IronBarCode;

// Generate QR code for a URL
var qrCode = BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode);

// Save as PNG image
qrCode.SaveAsPng("qr-code.png");
```

The `BarcodeWriter.CreateBarcode()` method accepts any string data and the `BarcodeEncoding.QRCode` format. QR codes automatically adjust their size and complexity based on the amount of data encoded.

---

## Step 2: Generate QR Code with Embedded Logo

Add a custom logo to QR codes for branding while maintaining scannability. IronBarcode automatically centers the logo and sizes it appropriately.

```csharp
using IronBarCode;

// Generate QR code with logo
var qrCodeWithLogo = QRCodeWriter.CreateQrCodeWithLogo(
    "https://ironsoftware.com",
    new QRCodeLogo("company-logo.png")
);

// Save the branded QR code
qrCodeWithLogo.SaveAsPng("qr-code-with-logo.png");
```

The `QRCodeWriter.CreateQrCodeWithLogo()` method accepts a string value and a `QRCodeLogo` object initialized with the logo image path. The logo is automatically positioned in the center and scaled to occupy approximately 20-30% of the QR code area.

QR codes use error correction that allows up to 30% of the code to be damaged or obscured while remaining scannable. This error correction enables logo embedding without data loss.

---

## Step 3: Customize QR Code Size and Margins

Control the physical dimensions and quiet zone (margins) of generated QR codes to meet specific printing or display requirements.

```csharp
using IronBarCode;

// Generate QR code
var qrCode = BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode);

// Set specific dimensions (300x300 pixels)
qrCode.ResizeTo(300, 300);

// Add quiet zone margins (10 pixels on all sides)
qrCode.SetMargins(10);

// Save the customized QR code
qrCode.SaveAsPng("qr-code-sized.png");
```

The `ResizeTo()` method accepts width and height in pixels for precise control over output dimensions. QR codes maintain their square aspect ratio.

The `SetMargins()` method adds white space around the QR code, known as the quiet zone. This margin is essential for reliable scanning as it provides visual separation from surrounding content. The ISO/IEC 18004 standard recommends a quiet zone at least 4 modules wide, though 10 pixels typically works well for most use cases.

---

## Step 4: Set Error Correction Level

Configure QR code error correction to balance data capacity with damage resistance. Higher error correction levels allow more damage but require larger QR codes for the same data.

```csharp
using IronBarCode;

// Create QR code with high error correction
var options = new QRCodeWriterOptions
{
    ErrorCorrectionLevel = ErrorCorrectionLevel.High
};

var qrCode = QRCodeWriter.CreateQrCode("https://example.com", options);
qrCode.SaveAsPng("qr-code-high-ecc.png");
```

QR codes support four error correction levels:

| Level | Recovery Capacity | Use Case |
|-------|-------------------|----------|
| **L (Low)** | 7% data recovery | Clean environments, digital displays |
| **M (Medium)** | 15% data recovery | Standard printing, general use (default) |
| **Q (Quartile)** | 25% data recovery | Industrial labeling, outdoor use |
| **H (High)** | 30% data recovery | Required when embedding logos |

When embedding logos, use Level H to ensure the logo doesn't prevent successful scanning. For plain QR codes without logos, Level M provides a good balance between size and reliability.

---

## Step 5: Generate QR Codes for Different Data Types

QR codes support various data types beyond plain text. Format the string data appropriately for each use case.

### URL QR Code

```csharp
// URL encoding (most common)
var urlQR = BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode);
urlQR.SaveAsPng("url-qr.png");
```

### Plain Text QR Code

```csharp
// Plain text message
var textQR = BarcodeWriter.CreateBarcode("Visit our booth at Hall A, Stand 42", BarcodeEncoding.QRCode);
textQR.SaveAsPng("text-qr.png");
```

### vCard Contact QR Code

```csharp
// vCard format for contact information
string vCard = @"BEGIN:VCARD
VERSION:3.0
FN:Jacob Mellor
ORG:Iron Software
TEL:+1-555-0123
EMAIL:jacob@ironsoftware.com
URL:https://ironsoftware.com
END:VCARD";

var vCardQR = BarcodeWriter.CreateBarcode(vCard, BarcodeEncoding.QRCode);
vCardQR.SaveAsPng("contact-qr.png");
```

### WiFi Network QR Code

```csharp
// WiFi network credentials (WIFI:S:SSID;T:WPA;P:password;;)
string wifiCredentials = "WIFI:S:GuestNetwork;T:WPA;P:SecurePassword123;;";

var wifiQR = BarcodeWriter.CreateBarcode(wifiCredentials, BarcodeEncoding.QRCode);
wifiQR.SaveAsPng("wifi-qr.png");
```

The QR code format automatically adapts to the data size. URLs and short text produce smaller QR codes, while vCards and WiFi credentials generate denser, larger QR codes.

---

## Complete Working Example

Here's the complete code demonstrating QR code generation with various customization options:

```csharp
using System;
using IronBarCode;

namespace QRCodeExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Basic QR code
            var basicQR = BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode);
            basicQR.SaveAsPng("basic-qr.png");
            Console.WriteLine("Basic QR code generated: basic-qr.png");

            // QR code with logo
            var logoQR = QRCodeWriter.CreateQrCodeWithLogo(
                "https://ironsoftware.com/csharp/barcode",
                new QRCodeLogo("logo.png")
            );
            logoQR.SaveAsPng("logo-qr.png");
            Console.WriteLine("Logo QR code generated: logo-qr.png");

            // Custom-sized QR code with margins
            var customQR = BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode);
            customQR.ResizeTo(400, 400);
            customQR.SetMargins(15);
            customQR.SaveAsPng("custom-qr.png");
            Console.WriteLine("Custom-sized QR code generated: custom-qr.png");

            // High error correction QR code
            var options = new QRCodeWriterOptions { ErrorCorrectionLevel = ErrorCorrectionLevel.High };
            var highEccQR = QRCodeWriter.CreateQrCode("Important data that needs protection", options);
            highEccQR.SaveAsPng("high-ecc-qr.png");
            Console.WriteLine("High ECC QR code generated: high-ecc-qr.png");

            // vCard contact QR code
            string vCard = @"BEGIN:VCARD
VERSION:3.0
FN:Jacob Mellor
ORG:Iron Software
EMAIL:jacob@ironsoftware.com
END:VCARD";
            var contactQR = BarcodeWriter.CreateBarcode(vCard, BarcodeEncoding.QRCode);
            contactQR.SaveAsPng("contact-qr.png");
            Console.WriteLine("Contact QR code generated: contact-qr.png");

            Console.WriteLine("\nAll QR codes generated successfully!");
        }
    }
}
```

**Download:** [Complete code file](../../code-examples/tutorials/qr-code-generation.cs)

---

## Next Steps

Now that you understand QR code generation with logos and customization, explore these related tutorials:

- **[How to Customize Barcode Appearance](./barcode-styling.md)** - Apply colors, margins, and annotations to all barcode types
- **[Generate Your First Barcode](./generate-barcode-basic.md)** - Learn about other 1D barcode formats like Code128 and EAN-13
- **[Barcode Format Types Guide](./barcode-format-types.md)** - Compare QR codes with other 2D formats like Data Matrix and PDF417

Return to [All Tutorials](./README.md)

---

Last verified: January 2026

*Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [Iron Software](https://ironsoftware.com)*
