# BarcodeLib vs IronBarcode: C# Barcode Generator Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

BarcodeLib is a long-standing open-source 1D barcode generation library in the .NET ecosystem, maintained by Brad Barnhill since 2007. It provides a straightforward API for creating linear (1D) barcode images across roughly 30 symbology variants. However, BarcodeLib is generation-only — it cannot read or scan barcodes — and it does not generate 2D symbologies such as QR Code or Data Matrix. This guide examines BarcodeLib's capabilities, known limitations, dependency footprint, and how it compares to [IronBarcode](https://ironsoftware.com/csharp/barcode/) for developers who need a complete barcode solution.

## Table of Contents

1. [What is BarcodeLib?](#what-is-barcodelib)
2. [Capabilities Comparison](#capabilities-comparison)
3. [Known Issues from GitHub](#known-issues-from-github)
4. [Installation and Setup](#installation-and-setup)
5. [Code Comparison](#code-comparison)
6. [Total Cost of Ownership Analysis](#total-cost-of-ownership-analysis)
7. [When to Use Each](#when-to-use-each)
8. [Migration Guide](#migration-guide)

---

## What is BarcodeLib?

BarcodeLib is an open-source barcode image generation library for .NET, maintained by Brad Barnhill on GitHub. The library has been actively developed since 2007, making it one of the longest-running barcode projects in the .NET ecosystem.

### Core Characteristics

| Attribute | Details |
|-----------|---------|
| **GitHub Repository** | [barnhill/barcodelib](https://github.com/barnhill/barcodelib) |
| **NuGet Package** | `BarcodeLib` 3.1.5 (March 2025) |
| **License** | Apache 2.0 (Free for commercial use) |
| **Primary Function** | 1D (linear) barcode image generation |
| **Supported Formats** | ~30 1D symbology variants; no 2D symbologies |
| **First Release** | 2007 |
| **Maintenance** | Actively maintained by Brad Barnhill |

### What BarcodeLib Does Well

BarcodeLib excels at its core mission: generating barcode images from text data. The library supports a comprehensive range of symbologies including:

**1D Formats:** Code 128 (A/B/C), Code 39 (Standard/Extended/Mod43), Code 93, Code 11, EAN-8, EAN-13, UPC-A, UPC-E, UPC Supplemental (2 and 5 digit), Codabar, ITF-14, Interleaved 2 of 5 (with Mod 10), Standard 2 of 5 (with Mod 10), Industrial 2 of 5, IATA 2 of 5, MSI (with various check digit options), Modified Plessey, Pharmacode, PostNet, Bookland, ISBN, JAN-13, LOGMARS, Telepen, FIM.

**2D Formats:** None. BarcodeLib does not generate QR Code, Data Matrix, PDF417, Aztec, or any other 2D symbology. The `Type` enum on the current 3.x source contains no 2D entries.

For developers who only need to generate linear barcodes — print shipping labels, create inventory tags, display 1D product codes — BarcodeLib provides this functionality at no cost.

### What BarcodeLib Cannot Do

The library has clear boundaries that are important to understand upfront:

1. **No Barcode Reading** - BarcodeLib cannot read, scan, or recognize barcodes from images. If you need to decode a barcode, you need a separate library.

2. **No 2D Symbologies** - BarcodeLib generates linear barcodes only. QR Code, Data Matrix, PDF417, Aztec, and MaxiCode are not supported.

3. **No PDF Support** - Cannot generate barcodes directly to PDF documents or extract barcodes from PDFs.

4. **No Automatic Detection** - Not applicable since reading isn't supported.

5. **No Batch Processing** - Designed for single barcode generation; no built-in batch workflows.

---

## Capabilities Comparison

### Feature Matrix

| Feature | BarcodeLib | IronBarcode |
|---------|------------|-------------|
| **Barcode Generation** | Yes (1D only) | Yes (1D and 2D) |
| **Barcode Reading** | No | Yes |
| **1D Format Count** | ~30 variants | 30+ |
| **2D Format Count** | 0 | 8+ (QR, Data Matrix, PDF417, Aztec, etc.) |
| **PDF Generation** | No | Yes |
| **PDF Extraction** | No | Yes |
| **Automatic Format Detection** | N/A | Yes |
| **ML Error Correction** | N/A | Yes |
| **Batch Processing** | Manual | Built-in |
| **Cross-Platform** | Yes via SkiaSharp (3.x) | Full |
| **Commercial Support** | No (community only) | Yes |

### Format Support Comparison

**BarcodeLib Formats (from the `BarcodeStandard.Type` enum, v3.1.5):**
```
1D: Code128, Code128A, Code128B, Code128C, Code39, Code39Extended,
    Code39Mod43, Code93, Code11, Ean13, Ean8, UpcA, UpcE,
    UpcSupplemental2Digit, UpcSupplemental5Digit, Codabar, Itf14,
    Interleaved2Of5, Interleaved2Of5Mod10, Standard2Of5,
    Standard2Of5Mod10, Industrial2Of5, Industrial2Of5Mod10,
    IATA2of5, MsiMod10, Msi2Mod10, MsiMod11, MsiMod11Mod10,
    ModifiedPlessey, Pharmacode, PostNet, Bookland, Isbn, Jan13,
    Logmars, Telepen, Fim, Ucc12, Ucc13, Usd8

2D: (none — BarcodeLib does not generate 2D symbologies)
```

**IronBarcode Formats:**
```
1D: All BarcodeLib formats plus GS1-128, GS1 DataBar (all variants),
    Intelligent Mail, Royal Mail, Australia Post, Plessey, and more

2D: QR Code (advanced with logo embedding), Data Matrix (ECC 200),
    PDF417, Aztec, MaxiCode
```

### Output Format Comparison

| Output | BarcodeLib | IronBarcode |
|--------|------------|-------------|
| PNG | Yes | Yes |
| JPEG | Yes | Yes |
| BMP | Yes | Yes |
| GIF | Yes | Yes |
| TIFF | Yes | Yes |
| SVG | No | Yes |
| PDF | No | Yes |
| HTML | No | Yes |
| Stream | Yes | Yes |

---

## Known Issues from GitHub

BarcodeLib's GitHub issue tracker documents friction points that affect real-world usage. The patterns below represent recurring developer experiences across the 2.x → 3.x transition.

### SkiaSharp Version Conflicts

When BarcodeLib 3.x adopted SkiaSharp as its graphics backend in place of System.Drawing, version incompatibility reports became common:

```
Error: "libSkiaSharp library version incompatible"
```

The issue manifests when other packages in the same project — MAUI, Blazor host packages, or other imaging libraries — resolve to SkiaSharp versions outside the range BarcodeLib expects. Version 3.1.5 currently pins SkiaSharp 2.88.8+, but projects converging on a newer 3.x SkiaSharp via MAUI may still see NU1608 warnings.

**Impact:** Build warnings and runtime assembly binding failures on version mismatch.

### Linux / .NET 6+ Compatibility

The transition from System.Drawing.Common (Windows-only after .NET 6) to cross-platform alternatives caused significant breaking changes:

```
System.TypeInitializationException: The type initializer for
'Gdip' threw an exception. ---> System.DllNotFoundException:
Unable to load shared library 'libgdiplus' or one of its dependencies.
```

Users on Linux or macOS with .NET 6+ faced runtime failures on older BarcodeLib versions until the library migrated to a SkiaSharp backend.

**Impact:** Linux deployments broken for extended period; workarounds required.

### Dependency Chain

BarcodeLib 3.x depends on **SkiaSharp** (native graphics with platform-specific binaries) — specifically `SkiaSharp >= 2.88.8` and `SkiaSharp.NativeAssets.Linux.NoDependencies >= 2.88.8`, plus `System.Resources.Extensions` and `System.Text.Json`. The package does not currently use ImageSharp.

```xml
<!-- BarcodeLib dependency chain (v3.1.5) -->
<PackageReference Include="BarcodeLib" Version="3.1.5" />
<!-- Transitively pulls in:
     SkiaSharp 2.88.8+
     SkiaSharp.NativeAssets.Linux.NoDependencies 2.88.8+
     System.Resources.Extensions 8.0.0+
     System.Text.Json 8.0.5+ -->
```

**Impact:** Version conflicts when SkiaSharp is also referenced by MAUI or other imaging packages, and platform-specific deployment work for Linux containers.

### No SLA or Guaranteed Support

As an open-source project with a single primary maintainer, BarcodeLib has no service level agreement:

- Issue response time: Varies (days to months)
- Bug fixes: When maintainer has time
- Feature requests: Community contributions accepted
- Security patches: No guaranteed timeline

**Impact:** Production issues may remain unresolved for extended periods.

---

## Installation and Setup

### BarcodeLib Installation

**Step 1: Add NuGet Package**

```bash
dotnet add package BarcodeLib
```

**Step 2: Handle Platform Dependencies**

For Linux deployments, you may need to install native libraries:

```bash
# For System.Drawing.Common fallback (older versions)
apt-get install libgdiplus

# For SkiaSharp backend
# Native binaries included, but verify platform support
```

**Step 3: Verify Backend Configuration**

BarcodeLib 3.1.5 allows backend selection:

```csharp
using BarcodeStandard;

// Default uses SkiaSharp on supported platforms
var barcode = new Barcode();

// Backend selection happens automatically, but may conflict
// with other SkiaSharp users in your project
```

### IronBarcode Installation

**Step 1: Add NuGet Package**

```bash
dotnet add package IronBarcode
```

**Step 2: Configure License (for production)**

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";
```

**Step 3: Ready to Use**

No additional configuration required. Single self-contained package works across all platforms.

### Setup Complexity Comparison

| Step | BarcodeLib | IronBarcode |
|------|------------|-------------|
| Package installation | 1 package (with transitive deps) | 1 package (self-contained) |
| Native dependencies | May require manual setup | None |
| License configuration | None | Single line |
| Platform setup | Varies by OS | None |
| Backend conflicts | Possible | None |

---

## Code Comparison

### Scenario 1: Basic Code128 Generation

Both libraries handle basic generation similarly:

**BarcodeLib:**

```csharp
using BarcodeStandard;
using SkiaSharp;

var barcode = new Barcode();
barcode.IncludeLabel = true;

SKImage image = barcode.Encode(
    BarcodeStandard.Type.Code128,
    "12345678901234",
    300,
    100
);

using var stream = File.OpenWrite("barcode.png");
image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
```

**IronBarcode:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678901234", BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .SaveAsPng("barcode.png");
```

**Comparison:**
- BarcodeLib: 11 lines, requires SkiaSharp knowledge
- IronBarcode: 3 lines, fluent API

For a complete walkthrough of barcode generation with format options and customization, see our [Generate Barcode Basic tutorial](../tutorials/generate-barcode-basic.md).

### Scenario 2: Barcode Reading (Where BarcodeLib Falls Short)

**BarcodeLib:**

```csharp
// BarcodeLib cannot read barcodes
// No API exists for this functionality
// You need a separate library like ZXing.Net

// This code does NOT exist in BarcodeLib:
// var result = barcode.Decode("barcode.png"); // NOT AVAILABLE
```

**IronBarcode:**

```csharp
using IronBarCode;

// Read barcode with automatic format detection
var results = BarcodeReader.Read("barcode.png");

foreach (var result in results)
{
    Console.WriteLine($"Format: {result.BarcodeType}");
    Console.WriteLine($"Value: {result.Value}");
}
```

For a complete example of this limitation, see [BarcodeLib Generation-Only Example](barcodelib-generation-only.cs).

### Scenario 3: PDF Processing

**BarcodeLib:**

```csharp
// BarcodeLib has no PDF support
// Cannot generate barcodes to PDF
// Cannot extract barcodes from PDF

// Would need to:
// 1. Generate barcode as image
// 2. Use separate PDF library (iTextSharp, PDFsharp)
// 3. Manually embed image into PDF
```

**IronBarcode:**

```csharp
using IronBarCode;

// Generate barcode directly to PDF
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPdf("barcode.pdf");

// Read all barcodes from existing PDF
var results = BarcodeReader.Read("invoice-scans.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Value}");
}
```

### Scenario 4: Cross-Platform Deployment

**BarcodeLib (potential issues):**

```csharp
// On Windows: Works
// On Linux: May require libgdiplus or fail with SkiaSharp
// On Docker: Requires specific base image or native libs

// Common Linux error:
// System.DllNotFoundException: Unable to load shared library 'libSkiaSharp'

// Workaround requires:
// apt-get install -y libfontconfig1 libice6 libsm6 libx11-6 libxext6
```

**IronBarcode:**

```csharp
// Same code works everywhere
// No platform-specific configuration
// Works in Docker with standard .NET images
var barcode = BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128);
```

For platform-specific issues, see [BarcodeLib Dependency Issues Example](barcodelib-dependency-issues.cs).

---

## Total Cost of Ownership Analysis

While BarcodeLib is free to use, the true cost of ownership includes developer time for workarounds, integration complexity, and missing features.

### Cost Breakdown

**Direct Costs:**

| Cost Type | BarcodeLib | IronBarcode |
|-----------|------------|-------------|
| License fee | $0 | From $799 one-time (Lite tier) |
| Annual renewal | $0 | $0 (perpetual) |
| Support contract | Not available | Included |

**Indirect Costs (Developer Time):**

| Task | BarcodeLib | IronBarcode |
|------|------------|-------------|
| Adding reading capability | 4-8 hours (integrate ZXing.Net) | 0 hours (included) |
| Fixing Linux deployment | 2-8 hours | 0 hours |
| Handling dependency conflicts | 1-4 hours per incident | 0 hours |
| PDF integration | 8-16 hours | 0 hours (native) |
| Debugging without support | Varies | Support available |

**5-Year TCO Example (10-Developer Team):**

```
BarcodeLib "Free" Path:
  License cost:           $0
  ZXing.Net integration:  16 hours × $100/hr = $1,600
  Linux fixes (2 incidents): 16 hours × $100/hr = $1,600
  PDF integration:        40 hours × $100/hr = $4,000
  Dependency conflicts:   20 hours × $100/hr = $2,000
  ───────────────────────────────────────────────
  Total:                  $9,200

IronBarcode Path:
  License cost:           $2,399 (Professional, 10-dev team)
  Integration work:       0 hours
  Linux fixes:            0 hours
  PDF integration:        0 hours
  Dependency conflicts:   0 hours
  ───────────────────────────────────────────────
  Total:                  $2,399

Savings with IronBarcode: $6,801
```

### Hidden Costs of "Free"

1. **Second Library Requirement** - BarcodeLib cannot read barcodes. Any workflow that includes scanning requires a second library (ZXing.Net, additional commercial tool), doubling maintenance burden.

2. **No SLA Risk** - Production issues have no guaranteed resolution timeline. A blocking bug could take weeks to fix upstream.

3. **Dependency Tax** - BarcodeLib 3.x pulls in SkiaSharp 2.88.8+ and its native binaries. SkiaSharp has its own version management requirements (especially in MAUI projects, which trend toward SkiaSharp 3.x) and platform-specific deployment quirks for Linux containers.

4. **Integration Glue Code** - PDF processing, batch workflows, and format detection all require custom code or additional libraries.

---

## When to Use Each

### Choose BarcodeLib When:

1. **Simple generation-only needs** - You only create barcodes (shipping labels, product tags) and never read them.

2. **Tight budget constraints** - You genuinely cannot afford any software license and have developer time to handle workarounds.

3. **Already in your codebase** - Existing integration works and you have no reading requirements.

4. **Non-Linux deployment** - Windows-only applications avoid the cross-platform dependency issues.

5. **Low-volume, low-criticality** - Internal tools where occasional issues aren't business-impacting.

### Choose IronBarcode When:

1. **You need reading AND writing** - Any workflow that scans barcodes (warehouse management, document processing, inventory) requires reading capability.

2. **You process PDFs** - Extracting barcodes from scanned invoices, embedding barcodes in generated documents.

3. **Cross-platform is required** - Linux servers, Docker containers, Azure Functions, AWS Lambda.

4. **Support matters** - Production applications where you can't afford to wait for community fixes.

5. **TCO perspective** - Developer time costs more than software licenses over project lifetime.

6. **Automatic format detection** - Processing mixed barcodes without knowing formats in advance.

7. **ML error correction** - Scanning damaged, partial, or low-quality barcodes from real-world documents.

---

## Migration Guide

### Why Migrate from BarcodeLib?

Common migration triggers:

| Trigger | Description |
|---------|-------------|
| "Need to read barcodes" | BarcodeLib is generation-only |
| "Linux deployment failing" | Dependency issues |
| "Need PDF support" | Not available |
| "Hit dependency conflict" | SkiaSharp version mismatch |
| "Need commercial support" | Community-only support |

### Package Migration

**Remove BarcodeLib:**

```bash
dotnet remove package BarcodeLib
```

**Add IronBarcode:**

```bash
dotnet add package IronBarcode
```

### API Mapping Reference

| BarcodeLib | IronBarcode | Notes |
|------------|-------------|-------|
| `new Barcode()` | `BarcodeWriter` | Static class vs instance |
| `barcode.Encode(Type, data, w, h)` | `BarcodeWriter.CreateBarcode(data, encoding)` | Fluent API |
| `BarcodeStandard.Type.Code128` | `BarcodeEncoding.Code128` | Enum naming |
| `BarcodeStandard.Type.UpcA` | `BarcodeEncoding.UPCA` | Direct mapping |
| `BarcodeStandard.Type.Ean13` | `BarcodeEncoding.EAN13` | Direct mapping |
| N/A (no QR support in BarcodeLib) | `BarcodeEncoding.QRCode` | New capability |
| `SKImage.Encode().SaveTo(stream)` | `.SaveAsPng()` / `.SaveAsJpeg()` | Direct methods |
| N/A | `BarcodeReader.Read()` | New capability |
| N/A | `.SaveAsPdf()` | New capability |

### Code Migration Example

**Before (BarcodeLib):**

```csharp
using BarcodeStandard;
using SkiaSharp;

public byte[] GenerateBarcode(string data)
{
    var barcode = new Barcode();
    barcode.IncludeLabel = true;
    barcode.LabelFont = new SKFont(SKTypeface.Default, 10);

    SKImage image = barcode.Encode(
        BarcodeStandard.Type.Code128,
        data,
        300,
        100
    );

    using var ms = new MemoryStream();
    image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
    return ms.ToArray();
}
```

**After (IronBarcode):**

```csharp
using IronBarCode;

public byte[] GenerateBarcode(string data)
{
    return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ResizeTo(300, 100)
        .AddBarcodeValueTextBelowBarcode()
        .ToPngBinaryData();
}

// NEW: Also add reading capability
public string ReadBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value;
}
```

### Migration Checklist

**Pre-Migration:**
- [ ] Inventory all BarcodeLib usage locations
- [ ] Identify barcode types used (Code128, EAN13, etc.)
- [ ] Check for any reading requirements (add new capability)
- [ ] Check for PDF requirements (add new capability)
- [ ] Obtain IronBarcode license

**Migration:**
- [ ] Remove BarcodeLib NuGet package
- [ ] Remove SkiaSharp references (if only used by BarcodeLib)
- [ ] Add IronBarcode NuGet package
- [ ] Add license initialization
- [ ] Update generation code per API mapping
- [ ] Add reading code where needed

**Post-Migration:**
- [ ] Test all barcode generation scenarios
- [ ] Test cross-platform deployment
- [ ] Verify PDF workflows (if applicable)
- [ ] Remove unused native dependencies from deployment

---

## Additional Resources

### Documentation

- [IronBarcode Getting Started](https://ironsoftware.com/csharp/barcode/docs/) - Official documentation
- [IronBarcode API Reference](https://ironsoftware.com/csharp/barcode/object-reference/api/) - Complete API documentation
- [IronBarcode on NuGet](https://www.nuget.org/packages/BarCode) - Package download

### Code Examples

- [BarcodeLib Generation-Only Limitation](barcodelib-generation-only.cs) - Demonstrates reading limitation
- [BarcodeLib Dependency Issues](barcodelib-dependency-issues.cs) - Platform-specific challenges

### Related Comparisons

- [ZXing.Net Comparison](../zxing-net/) - Open-source with reading capability
- [QRCoder Comparison](../qrcodernet/) - QR-only open-source generator
- [NetBarcode Comparison](../netbarcode/) - 1D-only open-source generator

---

*Last verified: January 2026*
*Author: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*
