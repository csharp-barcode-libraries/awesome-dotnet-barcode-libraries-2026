# QRCoder vs IronBarcode: C# QR Code Generator Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

QRCoder is the most popular open-source QR code generator for .NET, with over 8 million NuGet downloads. The library excels at its singular focus: generating QR codes with zero dependencies. It includes useful payload generators for WiFi credentials, contact cards, URLs, and more. However, QRCoder is QR-only—it cannot generate Code128, EAN, DataMatrix, or any other barcode format. Additionally, like other open-source generators, QRCoder cannot read QR codes, only create them. This guide examines QRCoder's strengths in its niche, its fundamental limitations, and how it compares to [IronBarcode](https://ironsoftware.com/csharp/barcode/) for developers who need flexibility beyond QR codes.

## Table of Contents

1. [What is QRCoder?](#what-is-qrcoder)
2. [Critical Limitation: QR Only](#critical-limitation-qr-only)
3. [QRCoder Strengths](#qrcoder-strengths)
4. [Capabilities Comparison](#capabilities-comparison)
5. [Code Comparison](#code-comparison)
6. [Total Cost of Ownership Analysis](#total-cost-of-ownership-analysis)
7. [When to Use Each](#when-to-use-each)
8. [Migration Guide](#migration-guide)

---

## What is QRCoder?

QRCoder is a pure C# QR code generation library with no external dependencies. Originally created by Raffael Herrmann in 2013, the project is now maintained by Shane32 (as of 2021+). The library has become the de facto standard for QR-only generation in the .NET ecosystem.

### Core Characteristics

| Attribute | Details |
|-----------|---------|
| **GitHub Repository** | [Shane32/QRCoder](https://github.com/Shane32/QRCoder) |
| **NuGet Package** | QRCoder 1.7.0+ |
| **License** | MIT (Free for commercial use) |
| **Primary Function** | QR code generation only |
| **Dependencies** | None (pure managed code) |
| **Downloads** | 8M+ on NuGet |

### Why QRCoder is Popular

QRCoder's popularity stems from several factors:

1. **Zero Dependencies** - Pure C# with no native libraries, no ImageSharp, no SkiaSharp. Works everywhere .NET runs without platform-specific issues.

2. **Focused Excellence** - Does one thing exceptionally well rather than many things adequately.

3. **Payload Generators** - Built-in helpers for WiFi, URLs, SMS, phone numbers, calendar events, and more.

4. **Multiple Renderers** - PNG, SVG, ASCII art, Base64, PDF (via separate package).

5. **MIT License** - Genuinely free for any use with no hidden commercial license requirements.

### Maintainer Transition

QRCoder's original creator Raffael Herrmann stepped back from active maintenance around 2020. Shane32 forked the project and has maintained it since 2021, ensuring continued .NET compatibility and bug fixes. The Shane32/QRCoder fork is now the primary active version.

---

## Critical Limitation: QR Only

QRCoder's defining limitation is right in its name: it only generates QR codes. No other barcode format is supported.

### Formats NOT Supported

QRCoder cannot generate:

**1D Barcodes:**
- Code 128 (shipping, inventory)
- Code 39 (logistics, healthcare)
- EAN-13 (retail products)
- UPC-A (North American retail)
- Code 93, Codabar, ITF

**Other 2D Barcodes:**
- DataMatrix (pharmaceutical, electronics)
- PDF417 (ID cards, shipping)
- Aztec (boarding passes)
- MaxiCode (UPS shipping)

### Impact on Projects

**Applications Where QR-Only Works:**
- Marketing campaigns (QR links to websites)
- WiFi sharing (QR credential encoding)
- Payment links (static QR payments)
- Two-factor authentication (TOTP QR setup)
- Simple mobile app deep links

**Applications Requiring More:**
- Retail (UPC/EAN product codes)
- Shipping (Code128, MaxiCode)
- Healthcare (DataMatrix drug tracking)
- Inventory (mixed format environments)
- Government IDs (PDF417)
- Airlines (Aztec boarding passes)

### The Multi-Library Reality

When a project starts with QRCoder and later needs other formats:

```csharp
// QRCoder for QR codes
using QRCoder;
var qrGenerator = new QRCodeGenerator();
var qrCode = qrGenerator.CreateQrCode("data", QRCodeGenerator.ECCLevel.M);

// Need Code128? Add NetBarcode
using NetBarcode;
var code128 = new Barcode("data", Type.Code128);

// Need DataMatrix? Add another library
// Need to READ barcodes? Add ZXing.Net

// Result: 3-4 different APIs, different patterns, more to maintain
```

---

## QRCoder Strengths

While acknowledging limitations, QRCoder genuinely excels in its niche. Fair comparison requires recognizing what it does well.

### PayloadGenerator Feature

QRCoder's PayloadGenerator creates properly formatted QR code data for common use cases:

```csharp
using QRCoder;

// WiFi credentials
var wifi = new PayloadGenerator.WiFi(
    ssid: "MyNetwork",
    password: "SecurePass123",
    authenticationMode: PayloadGenerator.WiFi.Authentication.WPA
);

// Contact card (vCard)
var contact = new PayloadGenerator.ContactData(
    outputType: PayloadGenerator.ContactData.ContactOutputType.VCard4,
    firstname: "John",
    lastname: "Doe",
    email: "john@example.com"
);

// Calendar event
var calendarEvent = new PayloadGenerator.CalendarEvent(
    subject: "Team Meeting",
    description: "Weekly sync",
    location: "Conference Room A",
    start: DateTime.Now.AddDays(1),
    end: DateTime.Now.AddDays(1).AddHours(1)
);

// Use with QR code generation
var qrGenerator = new QRCodeGenerator();
var qrCodeData = qrGenerator.CreateQrCode(wifi.ToString(), QRCodeGenerator.ECCLevel.M);
```

This payload generation is genuinely useful and well-implemented.

### Multiple Output Formats

QRCoder supports various output formats through different renderer classes:

| Renderer | Output | Use Case |
|----------|--------|----------|
| `PngByteQRCode` | PNG bytes | File/stream output |
| `QRCode` | System.Drawing.Bitmap | WinForms/GDI+ apps |
| `BitmapByteQRCode` | BMP bytes | Low-level access |
| `SvgQRCode` | SVG string | Web/scalable |
| `AsciiQRCode` | ASCII art | Console/terminals |
| `Base64QRCode` | Base64 string | Data URIs |
| `PdfByteQRCode` | PDF bytes | Document embedding |

### Zero Dependency Advantage

QRCoder's lack of dependencies means:
- No SkiaSharp version conflicts
- No ImageSharp license considerations
- No native library deployment issues
- Works on any .NET platform without setup

This is a genuine advantage for projects where simplicity matters.

---

## Capabilities Comparison

### Feature Matrix

| Feature | QRCoder | IronBarcode |
|---------|---------|-------------|
| **QR Code Generation** | Yes (excellent) | Yes |
| **Other 1D Formats** | No | Yes (30+) |
| **Other 2D Formats** | No | Yes (8+) |
| **Barcode Reading** | No | Yes |
| **PDF Support** | Limited (output only) | Full (read/write) |
| **Payload Generators** | Yes (built-in) | Partial |
| **Logo Embedding** | Yes | Yes |
| **Dependencies** | None | Self-contained |
| **Error Correction** | Full ECC control | Full ECC control |

### QR Code Feature Comparison

When comparing QR-specific features:

| QR Feature | QRCoder | IronBarcode |
|------------|---------|-------------|
| Error Correction Levels | L, M, Q, H | L, M, Q, H |
| Logo/Image Embedding | Yes | Yes |
| Color Customization | Yes | Yes |
| Quiet Zone Control | Yes | Yes |
| Module Size Control | Yes (via pixel size) | Yes (via ResizeTo) |
| SVG Output | Yes | Yes |
| Micro QR | Yes | No |

For pure QR code generation, both libraries are capable. QRCoder has a slight edge with Micro QR support.

### Total Format Support

| Library | 1D Formats | 2D Formats | Can Read |
|---------|------------|------------|----------|
| QRCoder | 0 | 1 (QR only) | No |
| IronBarcode | 30+ | 8+ | Yes |

---

## Code Comparison

### Scenario 1: Basic QR Code Generation

Both libraries handle QR generation well:

**QRCoder:**

```csharp
using QRCoder;
using System.IO;

public byte[] GenerateQRCode(string data)
{
    // Create generator
    var qrGenerator = new QRCodeGenerator();

    // Create QR code data with error correction level
    var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);

    // Create PNG renderer
    var qrCode = new PngByteQRCode(qrCodeData);

    // Get PNG bytes (module size = 20 pixels)
    return qrCode.GetGraphic(20);
}

// Save to file
var qrBytes = GenerateQRCode("https://ironsoftware.com");
File.WriteAllBytes("qrcoder-output.png", qrBytes);
```

**IronBarcode:**

```csharp
using IronBarCode;

public void GenerateQRCode(string data)
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
        .ResizeTo(200, 200)
        .SaveAsPng("ironbarcode-output.png");
}
```

**Comparison:**
- QRCoder: More explicit control over QR-specific options
- IronBarcode: Simpler API, consistent with other barcode types

For comprehensive QR code generation techniques including styling and data embedding, see our [QR Code Generation tutorial](../tutorials/qr-code-generation.md).

### Scenario 2: WiFi QR Code

**QRCoder (with PayloadGenerator):**

```csharp
using QRCoder;

public byte[] GenerateWiFiQR(string ssid, string password)
{
    // Create WiFi payload
    var wifiPayload = new PayloadGenerator.WiFi(
        ssid: ssid,
        password: password,
        authenticationMode: PayloadGenerator.WiFi.Authentication.WPA
    );

    var qrGenerator = new QRCodeGenerator();
    var qrCodeData = qrGenerator.CreateQrCode(
        wifiPayload.ToString(),
        QRCodeGenerator.ECCLevel.H
    );

    var qrCode = new PngByteQRCode(qrCodeData);
    return qrCode.GetGraphic(10);
}
```

**IronBarcode:**

```csharp
using IronBarCode;

public void GenerateWiFiQR(string ssid, string password)
{
    // WiFi QR format: WIFI:T:WPA;S:ssid;P:password;;
    string wifiData = $"WIFI:T:WPA;S:{ssid};P:{password};;";

    BarcodeWriter.CreateBarcode(wifiData, BarcodeEncoding.QRCode)
        .SaveAsPng("wifi-qr.png");
}
```

QRCoder's PayloadGenerator is a convenience feature, though the WiFi QR format is standardized and can be constructed manually.

### Scenario 3: Reading QR Codes (QRCoder Cannot)

**QRCoder:**

```csharp
// QRCoder is generation-only
// There is NO reading API

// These methods DO NOT EXIST:
// qrGenerator.Read("image.png");
// QRCodeReader.Decode("image.png");

// Would need ZXing.Net or similar library
```

**IronBarcode:**

```csharp
using IronBarCode;

public string ReadQRCode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);

    foreach (var result in results)
    {
        if (result.BarcodeType == BarcodeEncoding.QRCode)
        {
            return result.Text;
        }
    }

    return null;
}

// Also works with PDFs
var pdfResults = BarcodeReader.Read("document.pdf");
```

For QR-only examples, see [QRCoder QR-Only Example](qrcoder-qr-only.cs).

### Scenario 4: Multi-Format Requirements

**QRCoder (cannot do this):**

```csharp
// QRCoder can ONLY generate QR codes
// For Code128 shipping labels, you need another library
// For EAN-13 product codes, you need another library
// For DataMatrix pharmaceutical codes, you need another library

// Typical multi-library solution:
// QRCoder for QR
// NetBarcode for 1D codes
// No good option for DataMatrix

// Multiple APIs, multiple patterns, multiple maintenance burdens
```

**IronBarcode (unified):**

```csharp
using IronBarCode;

public class BarcodeService
{
    public void GenerateQRCode(string data, string path)
    {
        BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
            .SaveAsPng(path);
    }

    public void GenerateShippingLabel(string data, string path)
    {
        BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
            .SaveAsPng(path);
    }

    public void GenerateProductCode(string ean, string path)
    {
        BarcodeWriter.CreateBarcode(ean, BarcodeEncoding.EAN13)
            .SaveAsPng(path);
    }

    public void GeneratePharmaCode(string data, string path)
    {
        BarcodeWriter.CreateBarcode(data, BarcodeEncoding.DataMatrix)
            .SaveAsPng(path);
    }

    // Same API for all formats
}
```

For format limitation examples, see [QRCoder Format Limitation Example](qrcoder-format-limitation.cs).

---

## Total Cost of Ownership Analysis

### Direct Costs

| Cost | QRCoder | IronBarcode |
|------|---------|-------------|
| License | $0 | $749 one-time |
| Dependencies | None | Self-contained |
| Commercial restrictions | None | None |

### Honest Assessment: QRCoder is Genuinely Free

Unlike some open-source libraries with hidden commercial license triggers (Syncfusion's revenue cap, ImageSharp's commercial license), QRCoder under MIT license is genuinely free for any use. There's no gotcha.

### Indirect Costs: When You Need More

The cost calculation changes when requirements expand:

**Scenario A: QR-Only Forever**
```
QRCoder: $0
IronBarcode: $749

Winner: QRCoder (if genuinely QR-only)
```

**Scenario B: Start QR, Add Code128 Later**
```
QRCoder path:
  QRCoder license:        $0
  NetBarcode integration: 4 hours × $100/hr = $400
  Maintain 2 libraries:   10 hours × $100/hr = $1,000
  Total:                  $1,400

IronBarcode path:
  License:                $749
  Additional work:        0
  Total:                  $749

Winner: IronBarcode
```

**Scenario C: Full Barcode Needs (read + write, multiple formats)**
```
QRCoder path:
  QRCoder:                $0
  NetBarcode:             Integration $400
  ZXing.Net for reading:  Integration $800
  DataMatrix library:     Integration $400
  Maintain 4 libraries:   $2,000
  Total:                  $3,600

IronBarcode path:
  License:                $749
  Total:                  $749

Winner: IronBarcode by $2,851
```

### Decision Matrix

| Scenario | Recommendation |
|----------|---------------|
| Only QR codes, ever | QRCoder |
| QR codes now, maybe more later | IronBarcode |
| Multiple formats needed | IronBarcode |
| Need to read barcodes | IronBarcode |
| Zero budget, only QR | QRCoder |

---

## When to Use Each

### Choose QRCoder When:

1. **Exclusively QR codes** - You genuinely only need QR code generation and are certain this won't change.

2. **Zero dependencies desired** - You want the smallest possible footprint with no external libraries.

3. **PayloadGenerator value** - The built-in WiFi, vCard, calendar event generators match your use case.

4. **Budget is truly zero** - You have no software budget and won't need other formats.

5. **Single-purpose application** - A focused tool like a QR code generator app.

QRCoder is an excellent library for its specific purpose. If QR codes are all you need, it's a solid choice.

### Choose IronBarcode When:

1. **Multiple format possibilities** - Any chance you'll need Code128, EAN, or other formats.

2. **Reading capability needed** - Need to scan/decode QR codes or other barcodes.

3. **PDF processing** - Extract QR codes from documents or embed in PDFs.

4. **Future flexibility** - Want to add formats without library changes.

5. **Unified API preference** - One consistent API for all barcode operations.

6. **Professional support** - Need guaranteed response for production issues.

---

## Migration Guide

### Why Migrate from QRCoder?

Common migration triggers:

| Trigger | Issue |
|---------|-------|
| "Now we need Code128" | QR-only |
| "Need to read QR codes" | Generation-only |
| "Multiple barcode types required" | Single format |
| "Need PDF extraction" | Not supported |
| "Consolidating libraries" | Currently using 3+ libraries |

### Package Migration

**Remove QRCoder:**

```bash
dotnet remove package QRCoder
```

**Add IronBarcode:**

```bash
dotnet add package IronBarcode
```

### API Mapping Reference

| QRCoder | IronBarcode | Notes |
|---------|-------------|-------|
| `new QRCodeGenerator()` | `BarcodeWriter` | Static class |
| `qrGenerator.CreateQrCode(data, level)` | `BarcodeWriter.CreateBarcode(data, QRCode)` | Direct |
| `new PngByteQRCode(data)` | Not needed | Built into CreateBarcode |
| `qrCode.GetGraphic(20)` | `.ToPngBinaryData()` | Size via ResizeTo |
| `QRCodeGenerator.ECCLevel.M` | Automatic or configurable | Default is M |
| N/A | `BarcodeReader.Read()` | New capability |
| N/A | `BarcodeEncoding.Code128` | New capability |

### Code Migration Example

**Before (QRCoder):**

```csharp
using QRCoder;
using System.IO;

public class QRService
{
    private readonly QRCodeGenerator _generator;

    public QRService()
    {
        _generator = new QRCodeGenerator();
    }

    public byte[] GenerateQR(string data)
    {
        var qrCodeData = _generator.CreateQrCode(
            data,
            QRCodeGenerator.ECCLevel.M
        );

        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(10);
    }

    public void SaveQR(string data, string path)
    {
        var bytes = GenerateQR(data);
        File.WriteAllBytes(path, bytes);
    }

    // Cannot read QR codes
    // Cannot generate other formats
}
```

**After (IronBarcode):**

```csharp
using IronBarCode;

public class QRService
{
    public byte[] GenerateQR(string data)
    {
        return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
            .ResizeTo(200, 200)
            .ToPngBinaryData();
    }

    public void SaveQR(string data, string path)
    {
        BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
            .SaveAsPng(path);
    }

    // NEW: Read QR codes
    public string ReadQR(string imagePath)
    {
        var results = BarcodeReader.Read(imagePath);
        return results.FirstOrDefault()?.Text;
    }

    // NEW: Generate other formats
    public void GenerateCode128(string data, string path)
    {
        BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
            .SaveAsPng(path);
    }
}
```

### PayloadGenerator Migration

QRCoder's PayloadGenerator helpers can be replaced with standard QR code formats:

```csharp
// QRCoder WiFi payload
var wifi = new PayloadGenerator.WiFi("SSID", "pass", WiFi.Authentication.WPA);
string wifiData = wifi.ToString();
// Output: WIFI:T:WPA;S:SSID;P:pass;;

// IronBarcode equivalent (use standard format directly)
string wifiData = "WIFI:T:WPA;S:SSID;P:pass;;";
BarcodeWriter.CreateBarcode(wifiData, BarcodeEncoding.QRCode)
    .SaveAsPng("wifi.png");
```

The payload formats are standardized, so you can construct them directly or create helper methods.

### Migration Checklist

**Pre-Migration:**
- [ ] Inventory all QRCoder usage
- [ ] List PayloadGenerator types used
- [ ] Check for reading requirements (add new capability)
- [ ] Check for multi-format requirements (add new capability)
- [ ] Obtain IronBarcode license

**Migration:**
- [ ] Remove QRCoder NuGet package
- [ ] Add IronBarcode NuGet package
- [ ] Add license initialization
- [ ] Replace QRCodeGenerator with BarcodeWriter
- [ ] Replace GetGraphic with SaveAsPng/ToPngBinaryData
- [ ] Migrate PayloadGenerator calls to direct format strings

**Post-Migration:**
- [ ] Test all QR generation
- [ ] Add reading capability where useful
- [ ] Add other formats where useful
- [ ] Update documentation

---

## Additional Resources

### Documentation

- [IronBarcode QR Code Guide](https://ironsoftware.com/csharp/barcode/how-to/create-qr-code/)
- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/)
- [IronBarcode on NuGet](https://www.nuget.org/packages/BarCode)

### Code Examples

- [QRCoder QR-Only Limitation](qrcoder-qr-only.cs)
- [QRCoder Format Limitation](qrcoder-format-limitation.cs)

### Related Comparisons

- [BarcodeLib Comparison](../barcodelib/) - Open-source with more 1D formats
- [NetBarcode Comparison](../netbarcode/) - Open-source 1D-only generator
- [Barcoder Comparison](../barcoder/) - Open-source with 2D support

---

*Last verified: January 2026*
*Author: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*
