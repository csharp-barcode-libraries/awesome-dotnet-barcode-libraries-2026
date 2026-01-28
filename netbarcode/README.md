# NetBarcode vs IronBarcode: C# Barcode Generator Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

NetBarcode is a lightweight open-source library for generating 1D barcodes in .NET. While the library excels at creating common linear barcode formats like Code128, EAN, and UPC, it has a fundamental limitation: NetBarcode supports only 1D barcodes—no QR codes, DataMatrix, or any other 2D formats. Additionally, like other open-source generators, NetBarcode is generation-only with no reading capability. This guide examines NetBarcode's strengths within its niche, its format limitations, and how it compares to [IronBarcode](https://ironsoftware.com/csharp/barcode/) for developers who need flexibility beyond 1D formats.

## Table of Contents

1. [What is NetBarcode?](#what-is-netbarcode)
2. [Critical Limitation: 1D Only](#critical-limitation-1d-only)
3. [Capabilities Comparison](#capabilities-comparison)
4. [ImageSharp Migration Issues](#imagesharp-migration-issues)
5. [Code Comparison](#code-comparison)
6. [Total Cost of Ownership Analysis](#total-cost-of-ownership-analysis)
7. [When to Use Each](#when-to-use-each)
8. [Migration Guide](#migration-guide)

---

## What is NetBarcode?

NetBarcode is an open-source 1D barcode generation library for .NET Standard 2.0+, created and maintained by Mauricio Tagliatti. The library focuses on simplicity, providing a clean API for generating the most common linear barcode formats.

### Core Characteristics

| Attribute | Details |
|-----------|---------|
| **GitHub Repository** | [Tagliatti/NetBarcode](https://github.com/Tagliatti/NetBarcode) |
| **NuGet Package** | NetBarcode 1.8.2 |
| **License** | MIT (Free for commercial use) |
| **Primary Function** | 1D barcode image generation |
| **Target Framework** | .NET Standard 2.0+ |
| **Dependency** | SixLabors.ImageSharp |

### Supported Formats

NetBarcode supports approximately 15 1D barcode formats:

**Linear Barcode Formats:**
- Code 128 (with auto-switching)
- Code 39 (Standard and Extended)
- Code 93
- EAN-8
- EAN-13
- UPC-A
- UPC-E
- Codabar
- ITF (Interleaved 2 of 5)
- MSI (Mod 10)

**NOT Supported (2D Formats):**
- QR Code
- DataMatrix
- PDF417
- Aztec
- MaxiCode
- Micro QR

### Why 1D Only?

NetBarcode was designed as a lightweight, focused library for the most common retail and shipping barcode use cases. The developers chose to support 1D formats thoroughly rather than partially support 2D formats. This is a reasonable design decision for its target use case, but it means the library cannot serve applications requiring QR codes or other 2D symbologies.

---

## Critical Limitation: 1D Only

The most significant constraint of NetBarcode is its exclusive focus on 1D (linear) barcodes. In today's development landscape, QR codes and DataMatrix are ubiquitous—and NetBarcode cannot generate them.

### What This Means for Projects

**Projects That Fit NetBarcode:**
- Retail point-of-sale (UPC/EAN only)
- Basic shipping labels (Code128 only)
- Internal inventory tags (1D sufficient)
- Legacy integrations with 1D-only scanners

**Projects That Don't Fit:**
- Mobile payment systems (QR codes required)
- Marketing materials (QR links to websites)
- Pharmaceutical tracking (DataMatrix common)
- Industrial parts marking (2D preferred)
- Document management (PDF417 often used)
- Event ticketing (QR standard practice)

### The Expanding Role of QR Codes

QR code usage has grown dramatically:

| Use Case | Format Required |
|----------|-----------------|
| Payment apps (Apple Pay, Google Pay) | QR Code |
| Restaurant menus (post-2020) | QR Code |
| Two-factor authentication | QR Code |
| App store links | QR Code |
| Shipping manifests (modern) | DataMatrix or QR |
| Pharmaceutical tracking | DataMatrix (required by FDA) |
| Automotive parts | DataMatrix |

If your application involves any of these scenarios, NetBarcode cannot help—you'll need a different library from the start.

### The Two-Library Problem

When a project starts with NetBarcode and later needs QR codes, developers face a choice:

**Option 1: Add a Second Library**
```csharp
// NetBarcode for 1D
using NetBarcode;
var code128 = new Barcode("12345", Type.Code128);

// QRCoder for QR codes (separate library)
using QRCoder;
var qrGenerator = new QRCodeGenerator();
var qrCode = qrGenerator.CreateQrCode("data", QRCodeGenerator.ECCLevel.M);

// Two APIs, two dependencies, two sets of bugs to track
```

**Option 2: Migrate Entirely**
Replace NetBarcode with a library that supports both 1D and 2D formats.

---

## Capabilities Comparison

### Feature Matrix

| Feature | NetBarcode | IronBarcode |
|---------|------------|-------------|
| **1D Barcode Generation** | Yes | Yes |
| **2D Barcode Generation** | No | Yes |
| **Barcode Reading** | No | Yes |
| **1D Format Count** | ~15 | 30+ |
| **2D Format Count** | 0 | 8+ |
| **PDF Generation** | No | Yes |
| **PDF Extraction** | No | Yes |
| **Automatic Detection** | N/A | Yes |
| **Batch Processing** | Manual | Built-in |
| **Cross-Platform** | Yes | Yes |

### Format Coverage Deep Dive

**Formats NetBarcode Supports:**
```
Code128, Code39, Code39Extended, Code93, EAN8, EAN13,
UPCA, UPCE, Codabar, ITF, MSI
```

**Additional Formats in IronBarcode:**

*1D Additions:*
```
GS1-128, GS1 DataBar (Omnidirectional, Stacked, Limited, Expanded),
Intelligent Mail, Royal Mail, Australia Post, Plessey, Telepen,
Pharmacode, PostNet, Planet, Japan Post, Swiss QR
```

*2D Formats (unavailable in NetBarcode):*
```
QRCode, DataMatrix (ECC 200), PDF417, Micro PDF417,
Aztec, MaxiCode, Han Xin
```

### Output Comparison

| Output Format | NetBarcode | IronBarcode |
|---------------|------------|-------------|
| PNG | Yes | Yes |
| JPEG | Yes | Yes |
| GIF | Yes | Yes |
| BMP | Yes | Yes |
| WebP | Via ImageSharp | Yes |
| SVG | No | Yes |
| PDF | No | Yes |
| TIFF | Yes | Yes |
| Stream | Yes | Yes |

---

## ImageSharp Migration Issues

NetBarcode uses SixLabors.ImageSharp for image rendering. While this provides good cross-platform support, version 1.8+ introduced breaking changes that affected existing projects.

### The Breaking Change

In NetBarcode 1.8+, the namespace for GetImage() and customization options changed:

**Before NetBarcode 1.8:**
```csharp
using NetBarcode;

var barcode = new Barcode("12345", Type.Code128);
var image = barcode.GetImage();
```

**After NetBarcode 1.8:**
```csharp
using NetBarcode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Fonts;

var barcode = new Barcode("12345", Type.Code128);
Image<Rgba32> image = barcode.GetImage();

// Font and color customization now requires ImageSharp types
```

### ImageSharp Commercial License Consideration

SixLabors.ImageSharp uses a "Six Labors Split License":
- **Free** for open-source projects and companies under $1M annual gross revenue
- **Commercial license required** for larger companies

This means NetBarcode's "free" status has a hidden commercial license dependency for companies above the revenue threshold—the same limitation as Syncfusion's community license.

### Dependency Chain

```xml
<PackageReference Include="NetBarcode" Version="1.8.2">
  <!-- Brings in: -->
  <PackageReference Include="SixLabors.ImageSharp" Version="3.x" />
  <PackageReference Include="SixLabors.Fonts" Version="2.x" />
</PackageReference>
```

---

## Code Comparison

### Scenario 1: Basic 1D Barcode Generation

Both libraries handle basic Code128 generation well:

**NetBarcode:**

```csharp
using NetBarcode;
using SixLabors.ImageSharp;

var barcode = new Barcode("12345678901234", Type.Code128);
barcode.SaveImageFile("netbarcode-code128.png");

// Or get as Image<Rgba32> for further processing
var image = barcode.GetImage();
```

**IronBarcode:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678901234", BarcodeEncoding.Code128)
    .SaveAsPng("ironbarcode-code128.png");
```

For simple 1D generation, both libraries are straightforward.

### Scenario 2: QR Code Generation (NetBarcode Fails)

**NetBarcode:**

```csharp
using NetBarcode;

// QR Code is NOT supported in NetBarcode
// The Type enum does not include QR:
// Type.Code128, Type.Code39, Type.EAN13, Type.UPCA... (no QR)

// This would require a second library like QRCoder:
// dotnet add package QRCoder
// using QRCoder;
// var qrGenerator = new QRCodeGenerator();
// var qrCode = qrGenerator.CreateQrCode("data", ECCLevel.M);
```

**IronBarcode:**

```csharp
using IronBarCode;

// Same API as Code128 - just change the encoding
BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode)
    .SaveAsPng("ironbarcode-qr.png");

// DataMatrix works the same way
BarcodeWriter.CreateBarcode("DMX-DATA", BarcodeEncoding.DataMatrix)
    .SaveAsPng("ironbarcode-datamatrix.png");

// PDF417 also available
BarcodeWriter.CreateBarcode("PDF417-DATA", BarcodeEncoding.PDF417)
    .SaveAsPng("ironbarcode-pdf417.png");
```

For complete examples, see [NetBarcode 1D-Only Example](netbarcode-1d-only.cs).

### Scenario 3: Reading Barcodes

**NetBarcode:**

```csharp
// NetBarcode is generation-only
// No reading API exists

// var result = barcode.Read("image.png"); // NOT AVAILABLE
```

**IronBarcode:**

```csharp
using IronBarCode;

// Read any barcode with automatic format detection
var results = BarcodeReader.Read("barcode.png");

foreach (var result in results)
{
    Console.WriteLine($"Type: {result.BarcodeType}");
    Console.WriteLine($"Value: {result.Value}");
}

// Works with PDFs too
var pdfResults = BarcodeReader.Read("invoice.pdf");
```

### Scenario 4: Mixed Format Batch Processing

**NetBarcode + Multiple Libraries:**

```csharp
// Would need three libraries:
// 1. NetBarcode for 1D
// 2. QRCoder for QR
// 3. ZXing.Net for reading

// NetBarcode
using NetBarcode;
var code128 = new Barcode("12345", Type.Code128);
code128.SaveImageFile("code128.png");

// QRCoder (separate library)
using QRCoder;
var qrGenerator = new QRCodeGenerator();
var qrData = qrGenerator.CreateQrCode("data", QRCodeGenerator.ECCLevel.M);
var qrCode = new PngByteQRCode(qrData);
File.WriteAllBytes("qr.png", qrCode.GetGraphic(20));

// ZXing.Net for reading (third library)
using ZXing;
var reader = new BarcodeReaderGeneric();
// ...complex setup for reading
```

**IronBarcode (unified):**

```csharp
using IronBarCode;

// Generate mixed formats
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128)
    .SaveAsPng("code128.png");

BarcodeWriter.CreateBarcode("data", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");

BarcodeWriter.CreateBarcode("data", BarcodeEncoding.DataMatrix)
    .SaveAsPng("datamatrix.png");

// Read all with automatic detection
foreach (var file in Directory.GetFiles(".", "*.png"))
{
    var result = BarcodeReader.Read(file).FirstOrDefault();
    Console.WriteLine($"{file}: {result?.BarcodeType} = {result?.Text}");
}
```

For format limitation examples, see [NetBarcode Format Limitations Example](netbarcode-format-limitations.cs).

---

## Total Cost of Ownership Analysis

### Direct Costs

| Cost | NetBarcode | IronBarcode |
|------|------------|-------------|
| License | $0 | $749 one-time |
| ImageSharp license (>$1M company) | Variable | Included |
| Support | Community only | Professional |

### Indirect Costs: The QR Code Tax

The most significant hidden cost with NetBarcode is when you eventually need QR codes:

**Scenario: E-commerce Platform**

| Phase | NetBarcode Path | IronBarcode Path |
|-------|-----------------|------------------|
| Initial: Product UPC codes | Works | Works |
| Month 3: Add QR for mobile payments | Need QRCoder | Works (same code) |
| Month 6: Read barcodes from supplier docs | Need ZXing.Net | Works (same code) |
| Month 12: PDF invoice processing | Need PDF library | Works (same code) |

**Cost Calculation:**

```
NetBarcode "Free" Path:
  NetBarcode license:         $0
  QRCoder integration:        8 hours × $100/hr = $800
  ZXing.Net integration:      12 hours × $100/hr = $1,200
  PDF integration:            16 hours × $100/hr = $1,600
  Maintaining 3 libraries:    20 hours × $100/hr = $2,000
  ─────────────────────────────────────────────────────
  Total over 2 years:         $5,600

IronBarcode Path:
  License:                    $749
  Integration time:           0 extra hours
  ─────────────────────────────────────────────────────
  Total over 2 years:         $749

Savings: $4,851
```

### Decision Framework

**NetBarcode makes sense if:**
- You are 100% certain you'll never need QR codes
- You'll never need to read barcodes
- Budget is literally zero (and your time has no value)
- The project is short-term or throwaway

**IronBarcode makes sense if:**
- There's any chance you'll need QR codes or 2D formats
- You might need reading capability
- You value developer time over zero-dollar license fees
- The project has a multi-year lifespan

---

## When to Use Each

### Choose NetBarcode When:

1. **Exclusively 1D needs** - You generate only Code128, EAN, UPC and will never need 2D formats.

2. **Retail-only application** - Point-of-sale system working with traditional UPC/EAN product codes only.

3. **Legacy integration** - Connecting to systems that only support 1D barcodes.

4. **Zero budget** - Genuinely cannot afford any software license (though consider total cost above).

5. **Minimal footprint priority** - Need the smallest possible dependency for 1D-only use.

### Choose IronBarcode When:

1. **Mixed format requirements** - Any possibility of needing QR codes, DataMatrix, or PDF417.

2. **Reading capability needed** - Need to scan/decode barcodes from images or documents.

3. **PDF processing** - Extract barcodes from PDFs or generate barcodes to PDFs.

4. **Future-proofing** - Want flexibility to add 2D formats without library changes.

5. **Single-vendor preference** - Prefer one library that covers all barcode needs.

6. **Professional support** - Need guaranteed response times for production issues.

---

## Migration Guide

### Why Migrate from NetBarcode?

Common migration triggers:

| Trigger | Issue |
|---------|-------|
| "Now we need QR codes" | 2D not supported |
| "Need to read barcodes" | No reading API |
| "PDF barcode processing" | Not supported |
| "Reduce library count" | Currently using 3+ libraries |
| "ImageSharp license" | Commercial license required |

### Package Migration

**Remove NetBarcode:**

```bash
dotnet remove package NetBarcode
dotnet remove package SixLabors.ImageSharp  # If no longer needed elsewhere
```

**Add IronBarcode:**

```bash
dotnet add package IronBarcode
```

### API Mapping Reference

| NetBarcode | IronBarcode | Notes |
|------------|-------------|-------|
| `new Barcode(data, Type)` | `BarcodeWriter.CreateBarcode(data, encoding)` | Static method |
| `Type.Code128` | `BarcodeEncoding.Code128` | Direct mapping |
| `Type.EAN13` | `BarcodeEncoding.EAN13` | Direct mapping |
| `Type.UPCA` | `BarcodeEncoding.UPCA` | Direct mapping |
| `Type.Code39` | `BarcodeEncoding.Code39` | Direct mapping |
| `barcode.SaveImageFile("x.png")` | `.SaveAsPng("x.png")` | Method naming |
| `barcode.GetImage()` | `.ToImage()` or `.ToPngBinaryData()` | Multiple options |
| N/A | `BarcodeEncoding.QRCode` | New capability |
| N/A | `BarcodeReader.Read()` | New capability |

### Code Migration Example

**Before (NetBarcode):**

```csharp
using NetBarcode;
using SixLabors.ImageSharp;

public class BarcodeService
{
    public void GenerateProductBarcode(string upc)
    {
        var barcode = new Barcode(upc, Type.UPCA);
        barcode.SaveImageFile($"product-{upc}.png");
    }

    public void GenerateShippingLabel(string code)
    {
        var barcode = new Barcode(code, Type.Code128);
        barcode.SaveImageFile($"shipping-{code}.png");
    }

    // Cannot generate QR codes
    // Cannot read barcodes
}
```

**After (IronBarcode):**

```csharp
using IronBarCode;

public class BarcodeService
{
    public void GenerateProductBarcode(string upc)
    {
        BarcodeWriter.CreateBarcode(upc, BarcodeEncoding.UPCA)
            .SaveAsPng($"product-{upc}.png");
    }

    public void GenerateShippingLabel(string code)
    {
        BarcodeWriter.CreateBarcode(code, BarcodeEncoding.Code128)
            .SaveAsPng($"shipping-{code}.png");
    }

    // NEW: Now can generate QR codes
    public void GenerateQRCode(string data)
    {
        BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
            .SaveAsPng($"qr-{data.GetHashCode()}.png");
    }

    // NEW: Now can read barcodes
    public string ReadBarcode(string imagePath)
    {
        var results = BarcodeReader.Read(imagePath);
        return results.FirstOrDefault()?.Text;
    }
}
```

### Migration Checklist

**Pre-Migration:**
- [ ] Inventory all NetBarcode usage
- [ ] Identify barcode types used (Code128, EAN, UPC, etc.)
- [ ] List any format extensions needed (QR, DataMatrix)
- [ ] Check for ImageSharp usage elsewhere
- [ ] Obtain IronBarcode license

**Migration:**
- [ ] Remove NetBarcode NuGet package
- [ ] Add IronBarcode NuGet package
- [ ] Add license initialization
- [ ] Update code per API mapping
- [ ] Add 2D format support where needed
- [ ] Add reading capability where needed

**Post-Migration:**
- [ ] Test all barcode generation
- [ ] Test new QR/2D capabilities
- [ ] Test reading functionality
- [ ] Remove unused ImageSharp packages
- [ ] Update documentation

---

## Additional Resources

### Documentation

- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/)
- [IronBarcode Format Reference](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/)
- [IronBarcode on NuGet](https://www.nuget.org/packages/BarCode)

### Code Examples

- [NetBarcode 1D-Only Limitation](netbarcode-1d-only.cs)
- [NetBarcode Format Limitations](netbarcode-format-limitations.cs)

### Related Comparisons

- [BarcodeLib Comparison](../barcodelib/) - Another open-source generator
- [QRCoder Comparison](../qrcodernet/) - QR-only complement to NetBarcode
- [ZXing.Net Comparison](../zxing-net/) - Open-source with reading capability

---

*Last verified: January 2026*
*Author: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*
