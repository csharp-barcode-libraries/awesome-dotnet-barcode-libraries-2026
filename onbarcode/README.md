# OnBarcode vs IronBarcode: C# Barcode Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

OnBarcode is a focused barcode generation library for .NET that distributes through both manual DLL downloads and NuGet packages (`OnBarcode.Barcode.Generator`, currently version 10.5.x). Barcode reading is sold as a separate product (`OnBarcode.Barcode.Reader`) with a separate license. This comprehensive guide examines OnBarcode's capabilities, its split product model, pricing structure, and how it compares to [IronBarcode](https://ironsoftware.com/csharp/barcode/) for production deployments.

## Table of Contents

1. [What is OnBarcode?](#what-is-onbarcode)
2. [Distribution and Purchase Model](#distribution-and-purchase-model)
3. [Capabilities Comparison](#capabilities-comparison)
4. [Installation and Setup](#installation-and-setup)
5. [Code Comparison](#code-comparison)
6. [Pricing Analysis](#pricing-analysis)
7. [When to Use Each Option](#when-to-use-each-option)
8. [Migration Guide: OnBarcode to IronBarcode](#migration-guide-onbarcode-to-ironbarcode)

---

## What is OnBarcode?

OnBarcode is a barcode generation library developed by OnBarcode.com. The company focuses on barcode technology with products spanning multiple platforms including .NET, Java, and various reporting tools.

### Product Family

OnBarcode offers several related products:

**Barcode Generator SDK:**
- .NET barcode generation library
- Multiple NuGet packages for different barcode types
- Focus on barcode creation, not reading

**Barcode Reader SDK:**
- Separate product for barcode recognition
- Requires additional purchase
- Not included with generator SDK

**Other Products:**
- Java barcode libraries
- Crystal Reports barcode fonts
- SSRS barcode components
- Excel barcode add-ins

### Platform Support

OnBarcode's .NET library supports:

- .NET Standard 2.0
- .NET 8, .NET 7, .NET 6, .NET 5
- .NET Core 3.1
- .NET Framework 4.x

### NuGet and DLL Distribution

OnBarcode publishes its libraries on nuget.org under the `OnBarcode` profile. Current packages include:

- `OnBarcode.Barcode.Generator` (generation)
- `OnBarcode.Barcode.Generator.SkiaSharp` (cross-platform variant)
- `OnBarcode.Barcode.Generator.AspNet.Framework`
- `OnBarcode.Barcode.Generator.WinForms.Framework`
- `OnBarcode.Barcode.Reader` (reading — separate package and license)
- `OnBarcode.Barcode.Reader.Framework`
- `OnBarcode.Barcode.Reader.Document.PDF` (PDF reading add-on)

OnBarcode also continues to offer manual DLL downloads from its website for customers who prefer that distribution model. Whichever path is used, runtime activation requires a license obtained directly from OnBarcode.

### Target Use Cases

OnBarcode primarily targets:

- Label printing applications
- Document generation with barcodes
- Product packaging systems
- Basic barcode generation needs

The library is generation-focused, making it less suitable for applications requiring both creation and scanning capabilities.

---

## Distribution and Purchase Model

Understanding OnBarcode's distribution and pricing model reveals important considerations for adoption.

### Historical DLL Distribution

Before NuGet packages were available, OnBarcode used a traditional software distribution model:

```
Purchase Flow (Traditional):
1. Contact sales or purchase from website
2. Receive download link for DLL files
3. Download ZIP archive with assemblies
4. Manually add assembly references
5. Deploy DLLs with application
```

This approach had several drawbacks:

- No automatic dependency management
- Manual version tracking
- Complex upgrade process
- Deployment required including DLLs

### Current NuGet Distribution

The newer NuGet packages simplify installation:

```bash
# Generator packages
dotnet add package OnBarcode.Barcode.Generator
dotnet add package OnBarcode.QRCode
```

However, licensing still requires separate purchase:

- NuGet packages can be installed freely
- Runtime requires valid license
- License obtained through sales contact

### Separate Reader Purchase

OnBarcode's most significant product decision: barcode reading requires a completely separate purchase.

**Generator SDK:**
- Creates barcodes
- Multiple format support
- Single license purchase

**Reader SDK:**
- Scans/reads barcodes
- Separate product
- Separate license required
- Separate NuGet package

This means organizations needing both generation and reading must:

1. Purchase Generator SDK
2. Purchase Reader SDK separately
3. Manage two different products
4. Potentially deal with API differences between products

### Published Pricing — Two Separate Products

OnBarcode publishes per-product pricing on dedicated purchase pages. The .NET Barcode Generator Suite uses these tiers (perpetual licenses, 30-day money-back guarantee):

**Linear-only (1D barcodes):**
- Single Developer: $990
- Five Developer: $1,990
- Unlimited Developer: $2,990

**Linear + 2D (full symbology coverage including QR Code, Data Matrix, PDF417):**
- Single Developer: $1,690
- Five Developer: $2,690
- Unlimited Developer: $3,990

The Barcode Reader SDK is priced and purchased separately ("License Price From $990"). An optional one-year Premier Support and Free Upgrade subscription costs 20–35% of the license price.

The friction is not opacity — it is duplication. An organization that needs both generation and reading must complete two purchases against two SKUs, and an organization that needs 2D barcodes must select the Linear + 2D tier rather than the cheaper Linear-only tier.

---

## Capabilities Comparison

A direct comparison of what each library provides.

### Core Features

| Feature | OnBarcode | IronBarcode |
|---------|----------|-------------|
| Generation | Yes | Yes |
| Reading/Scanning | Separate purchase | Included |
| Symbology Count | 20+ | 50+ |
| Auto Format Detection | No — explicit `BarcodeType` argument required (Reader SDK only) | Yes |
| PDF Native Support | Add-on (`OnBarcode.Barcode.Reader.Document.PDF`) | Built-in |
| ML Error Correction | Reader SDK only — no auto-detection | Yes |
| QR with Logo | Manual implementation | Built-in |
| NuGet First-Class | Recent addition | Day one |
| Unified SDK | No (separate products) | Yes |

### Generation Capabilities

Both libraries support barcode generation, but with different approaches:

**OnBarcode Generation:**
```csharp
using OnBarcode.Barcode;

// Create a Linear (1D) barcode object and configure properties
Linear barcode = new Linear();
barcode.Type = BarcodeType.CODE128;
barcode.Data = "12345678";
barcode.Resolution = 96;
barcode.drawBarcode("barcode.png");
```

**IronBarcode Generation:**
```csharp
// One-liner with sensible defaults
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");
```

### Reading Capabilities

This is the most significant difference:

**OnBarcode:**
- Reading requires separate OnBarcode Reader SDK
- Additional purchase required
- Separate API to learn
- Different installation process

**IronBarcode:**
- Reading included in same package
- Same purchase includes both capabilities
- Unified API for reading and writing
- Single NuGet package

### PDF Support

**OnBarcode:**
Does not natively support generating barcodes in PDFs or reading barcodes from PDFs. Requires additional libraries for PDF integration.

**IronBarcode:**
Native PDF support:
```csharp
// Read barcodes directly from PDF
var results = BarcodeReader.Read("document.pdf");

// Generate barcode and add to PDF
var barcode = BarcodeWriter.CreateBarcode("data", BarcodeEncoding.QRCode);
barcode.SaveAsPdf("barcode.pdf");
```

### Format Coverage

| Barcode Type | OnBarcode | IronBarcode |
|-------------|----------|-------------|
| Code 128 | Yes | Yes |
| Code 39 | Yes | Yes |
| QR Code | Yes | Yes |
| DataMatrix | Yes | Yes |
| PDF417 | Yes | Yes |
| EAN-13 | Yes | Yes |
| UPC-A | Yes | Yes |
| Aztec | Limited | Yes |
| Postal codes | Limited | Full |
| Total formats | 20+ | 50+ |

---

## Installation and Setup

### OnBarcode Installation

**Via NuGet:**
```bash
dotnet add package OnBarcode.Barcode.Generator
```

**Via Manual DLL (Alternative):**
1. Download DLL from OnBarcode website
2. Add reference to project:
```xml
<Reference Include="OnBarcode.Barcode">
  <HintPath>lib\OnBarcode.Barcode.dll</HintPath>
</Reference>
```

**License Configuration:**
```csharp
using OnBarcode.Barcode;

// Apply license (license key obtained from OnBarcode after purchase)
License.RegisterLicense("YOUR-LICENSE-KEY");
```

**If Reader is Needed (Separate Installation and License):**
```bash
# Separate package
dotnet add package OnBarcode.Barcode.Reader

# Separate license — purchased independently from the Generator license
License.RegisterLicense("YOUR-READER-LICENSE-KEY");
```

### IronBarcode Installation

**Single NuGet Package:**
```bash
dotnet add package IronBarcode
```

**Single License (Includes Reading and Writing):**
```csharp
using IronBarCode;

// One license for everything
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";

// Or from environment (Docker-friendly)
IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

### Setup Comparison

| Step | OnBarcode | IronBarcode |
|------|----------|-------------|
| Generator install | dotnet add package | dotnet add package |
| Reader install | Separate package | Same package |
| Generator license | Configure separately | Single key |
| Reader license | Purchase + configure | Same key |
| Total packages | 2 (gen + read) | 1 |
| Total licenses | 2 | 1 |

---

## Code Comparison

### Scenario 1: Generation-Only vs Unified SDK

This example demonstrates the difference between OnBarcode's generation-focused approach and IronBarcode's unified SDK.

See: [Separate Reader Demo](onbarcode-separate-reader.cs)

**Key Observations:**
- OnBarcode requires separate purchase for reading
- IronBarcode includes both capabilities in one package
- Unified SDK simplifies development and deployment

### Scenario 2: Installation and Setup History

This example shows the evolution of OnBarcode's distribution and how it compares to IronBarcode's always-modern approach.

See: [Manual Setup Comparison](onbarcode-manual-setup.cs)

**Key Observations:**
- OnBarcode historically required manual DLL management
- NuGet adoption is recent
- IronBarcode has been NuGet-first from the beginning

### Basic Generation Comparison

**OnBarcode:**
```csharp
using OnBarcode.Barcode;
using System.Drawing;

// Create Linear (1D) barcode object and configure properties
Linear barcode = new Linear();
barcode.Type = BarcodeType.CODE128;
barcode.Data = "12345678";
barcode.Resolution = 96;
barcode.X = 1;        // module width
barcode.BarcodeHeight = 80;
barcode.ShowText = true;
barcode.TextFont = new Font("Arial", 10);

// Draw to image
barcode.drawBarcode("barcode.png");
```

**IronBarcode:**
```csharp
using IronBarCode;

// One-liner generation
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");
```

**Line Count:** OnBarcode requires 10+ lines; IronBarcode achieves the same in 2 lines.

### Adding Reading Capability

**OnBarcode (requires separate product):**
```csharp
// IMPORTANT: OnBarcode Reader is a separate purchase
// This code requires OnBarcode.Barcode.Reader package
// AND a separate Reader license

using OnBarcode.Barcode;

// Separate license for the Reader product
License.RegisterLicense("READER-LICENSE-KEY");

// Static scanner API; format must be specified up front
string[] results = BarcodeScanner.Scan("barcode.png", BarcodeType.CODE128);
```

**IronBarcode (included):**
```csharp
using IronBarCode;

// Same package, same license as generation
var results = BarcodeReader.Read("barcode.png");

foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.BarcodeType}: {barcode.Value}");
}
```

### Batch Processing

**OnBarcode Generation + Reading (two products):**
```csharp
using OnBarcode.Barcode; // Generator and Reader share this namespace
                         // but ship as two separate NuGet packages and licenses

// Generate barcodes
foreach (var item in items)
{
    Linear barcode = new Linear();
    barcode.Type = BarcodeType.CODE128;
    barcode.Data = item.Code;
    barcode.drawBarcode($"{item.Id}.png");
}

// Read barcodes (requires separate Reader license + OnBarcode.Barcode.Reader package)
foreach (var file in Directory.GetFiles(".", "*.png"))
{
    string[] results = BarcodeScanner.Scan(file, BarcodeType.CODE128);
    // Process results
}
```

**IronBarcode (unified):**
```csharp
using IronBarCode;

// Generate barcodes
foreach (var item in items)
{
    BarcodeWriter.CreateBarcode(item.Code, BarcodeEncoding.Code128)
        .SaveAsPng($"{item.Id}.png");
}

// Read barcodes (same package, same license)
var allResults = BarcodeReader.Read(Directory.GetFiles(".", "*.png"));

foreach (var result in allResults)
{
    Console.WriteLine($"{result.InputPath}: {result.Value}");
}
```

### QR Code with Logo

**OnBarcode:**
```csharp
using OnBarcode.Barcode;
using System.Drawing;

// Generate base QR code using the dedicated QRCode class
QRCode qr = new QRCode();
qr.Data = "https://example.com";
qr.QRCodeDataMode = QRCodeDataMode.Auto;
qr.QRCodeECL = QRCodeECL.H; // High for logo overlay

Image qrImage = qr.drawBarcode();

// Manual logo overlay required
using (Graphics g = Graphics.FromImage(qrImage))
{
    Image logo = Image.FromFile("logo.png");
    int logoSize = qrImage.Width / 5;
    int x = (qrImage.Width - logoSize) / 2;
    int y = (qrImage.Height - logoSize) / 2;
    g.DrawImage(logo, x, y, logoSize, logoSize);
}

qrImage.Save("qr-with-logo.png");
```

**IronBarcode:**
```csharp
using IronBarCode;

// Built-in logo support
var qr = QRCodeWriter.CreateQrCodeWithLogo("https://example.com", "logo.png", 500);
qr.SaveAsPng("qr-with-logo.png");
```

---

## Pricing Analysis

### OnBarcode Pricing Structure

OnBarcode publishes pricing on its purchase pages. The Generator SDK is split into Linear-only and Linear + 2D tiers, and the Reader SDK is sold separately.

**.NET Barcode Generator Suite — Linear (1D only):**
- Single Developer: $990
- Five Developer: $1,990
- Unlimited Developer: $2,990

**.NET Barcode Generator Suite — Linear + 2D (full coverage):**
- Single Developer: $1,690
- Five Developer: $2,690
- Unlimited Developer: $3,990

**.NET Barcode Reader SDK:**
- Sold as a separate product, separate license. Pricing starts from $990 with tiered Single / Five / Unlimited Developer licenses, plus Server Distribution licenses for production deployment.

All licenses are perpetual with a 30-day money-back guarantee. An optional one-year Premier Support and Free Upgrade subscription costs 20–35% of the license price.

### IronBarcode Pricing

Single-product pricing covers both generation and reading:

| License Type | Price | Developers | Includes |
|-------------|-------|-----------|----------|
| Lite | $799 | 1 | Generation + Reading + PDF |
| Plus | $1,199 | 3 | Generation + Reading + PDF |
| Professional | $2,399 | 10 | Generation + Reading + PDF |
| Unlimited | $4,799 | Unlimited | Generation + Reading + PDF |

All licenses are perpetual with the option for support subscriptions.

### Cost Comparison Scenario

**Scenario: Five-developer team needs 2D generation AND reading**

```
OnBarcode:
  Generator Suite Linear + 2D, Five Developer:  $2,690
  Reader SDK, Five Developer (from):            $1,690+
  Total:                                        $4,380+

  Two procurement transactions, two licenses to manage,
  two NuGet packages, separate version cadences.

IronBarcode Professional:
  Everything included (10 devs):                $2,399

  One purchase, one license, one package.
  Includes generation, reading, and native PDF support.
```

### Value Transparency

| Aspect | OnBarcode | IronBarcode |
|--------|----------|-------------|
| Published pricing | Yes — per-product pages | Yes — single page |
| Generation cost | $990–$3,990 (1D vs 1D+2D tier) | Included |
| Reading cost | Sold separately ($990+) | Included |
| Total cost visibility | Sum of two SKUs | Single SKU |
| 1D vs 2D tier choice required | Yes | No |

---

## When to Use Each Option

### Choose OnBarcode When:

1. **You only need generation** - If your application creates barcodes but never needs to read them, OnBarcode's generator-only model isn't a limitation.

2. **You need source code access** - OnBarcode's Unlimited Developer license includes full source code, which some organizations require for auditing or customization.

3. **You have existing OnBarcode investment** - If your organization already uses OnBarcode products, adding to the existing relationship may make sense.

4. **Price negotiation is acceptable** - If your procurement process is comfortable with sales-based pricing, OnBarcode's model isn't a barrier.

5. **You're comfortable with DLL management** - For organizations not using NuGet or preferring direct DLL references, OnBarcode's traditional distribution works.

### Choose IronBarcode When:

1. **You need both generation and reading** - IronBarcode includes both capabilities in a single package with single license, avoiding the complexity of separate purchases.

2. **You prefer transparent pricing** - If knowing costs upfront is important for budgeting, IronBarcode's published pricing provides clarity.

3. **You want modern distribution** - First-class NuGet support from day one means proper dependency management and easy updates.

4. **You need PDF support** - Native PDF barcode reading and generation without additional libraries.

5. **You prefer unified APIs** - Single API to learn for both reading and writing simplifies development.

6. **You want automatic format detection** - When processing unknown barcode types, IronBarcode's auto-detection saves development effort.

---

## Migration Guide: OnBarcode to IronBarcode

### Why Developers Migrate

Common reasons for migrating from OnBarcode:

| Issue | OnBarcode | IronBarcode Solution |
|-------|----------|---------------------|
| "Need reading capability" | Separate purchase | Included |
| "Can't find pricing" | Contact sales | Published pricing |
| "Manual DLL management" | Traditional model | NuGet-first |
| "Two APIs for read/write" | Separate products | Unified SDK |
| "No PDF support" | Not available | Native PDF |
| "Limited documentation" | Varies | Comprehensive |

### Migration Complexity

| Application Type | Estimated Effort |
|-----------------|------------------|
| Generation only | 1-2 hours |
| Adding reading (was separate purchase) | 2-4 hours |
| Batch processing | 4-8 hours |
| Enterprise integration | 1-2 days |

### Package Changes

**Remove OnBarcode:**
```bash
dotnet remove package OnBarcode.Barcode.Generator
dotnet remove package OnBarcode.Barcode.Reader  # If using reader
```

**Add IronBarcode:**
```bash
dotnet add package IronBarcode
```

### Namespace Changes

**Before:**
```csharp
using OnBarcode.Barcode;  // covers both Generator and Reader namespaces
```

**After:**
```csharp
using IronBarCode;
```

### API Mapping Reference

#### Generation Classes

| OnBarcode | IronBarcode | Notes |
|----------|-------------|-------|
| `Linear` / `QRCode` / `DataMatrix` / `PDF417` classes | `BarcodeWriter.CreateBarcode()` | Static factory |
| `barcode.Type = BarcodeType.X` | Second parameter | Encoding enum |
| `barcode.Data` | First parameter | Data string |
| `barcode.drawBarcode()` | `barcode.SaveAsPng()` | Format-specific |
| `BarcodeType.CODE128` | `BarcodeEncoding.Code128` | Direct mapping |
| `QRCode` class | `BarcodeEncoding.QRCode` | Direct mapping |

#### Reading Classes (If Using Separate Reader)

| OnBarcode Reader | IronBarcode | Notes |
|-----------------|-------------|-------|
| `BarcodeScanner.Scan()` static | `BarcodeReader.Read()` | Static method |
| Format must be specified up front | Auto-detection | No `BarcodeType` argument needed |
| Return `string[]` | Return `BarcodeResults` | Richer result with `.Value`, `.BarcodeType`, `.PageNumber` |

### Code Migration Examples

#### Example 1: Basic Generation

**Before (OnBarcode):**
```csharp
using OnBarcode.Barcode;

Linear barcode = new Linear();
barcode.Type = BarcodeType.CODE128;
barcode.Data = "12345678";
barcode.Resolution = 96;
barcode.ShowText = true;
barcode.drawBarcode("barcode.png");
```

**After (IronBarcode):**
```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");
```

#### Example 2: QR Code Generation

**Before (OnBarcode):**
```csharp
using OnBarcode.Barcode;

QRCode qr = new QRCode();
qr.Data = "https://example.com";
qr.QRCodeDataMode = QRCodeDataMode.Auto;
qr.QRCodeECL = QRCodeECL.M;
qr.drawBarcode("qrcode.png");
```

**After (IronBarcode):**
```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com")
    .SaveAsPng("qrcode.png");
```

#### Example 3: Adding Reading (Previously Separate Purchase)

**Before (OnBarcode - required separate product):**
```csharp
// Required OnBarcode.Barcode.Reader package
// Required separate Reader license
using OnBarcode.Barcode;

License.RegisterLicense("READER-KEY");

string[] results = BarcodeScanner.Scan("barcode.png", BarcodeType.CODE128);
```

**After (IronBarcode - included):**
```csharp
using IronBarCode;

// Same package, same license as generation
var results = BarcodeReader.Read("barcode.png");

foreach (var result in results)
{
    Console.WriteLine(result.Value);
}
```

#### Example 4: Batch Processing

**Before (OnBarcode):**
```csharp
using OnBarcode.Barcode;

foreach (var file in files)
{
    Linear barcode = new Linear();
    barcode.Type = BarcodeType.CODE128;
    barcode.Data = GetDataForFile(file);
    barcode.drawBarcode(GetOutputPath(file));
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

foreach (var file in files)
{
    BarcodeWriter.CreateBarcode(GetDataForFile(file), BarcodeEncoding.Code128)
        .SaveAsPng(GetOutputPath(file));
}
```

### Common Migration Issues

#### Issue 1: No Symbology-Specific Class in IronBarcode

**Problem:** OnBarcode uses one class per symbology family (`Linear`, `QRCode`, `DataMatrix`, `PDF417`). Developers look for the same shape in IronBarcode.

**Solution:** IronBarcode uses a single static factory parameterized by encoding:

```csharp
// OnBarcode pattern
Linear barcode = new Linear();
barcode.Type = BarcodeType.CODE128;
barcode.Data = "...";
barcode.drawBarcode("out.png");

// IronBarcode pattern
BarcodeWriter.CreateBarcode("...", BarcodeEncoding.Code128).SaveAsPng("out.png");
```

#### Issue 2: BarcodeType Enum Mapping

**Problem:** Enum value casing and names differ.

**Solution:** Common mappings:

```csharp
// OnBarcode -> IronBarcode
BarcodeType.CODE128   -> BarcodeEncoding.Code128
BarcodeType.CODE39    -> BarcodeEncoding.Code39
BarcodeType.EAN13     -> BarcodeEncoding.EAN13
BarcodeType.UPCA      -> BarcodeEncoding.UPCA
// 2D classes -> encoding values
QRCode class      -> BarcodeEncoding.QRCode
DataMatrix class  -> BarcodeEncoding.DataMatrix
PDF417 class      -> BarcodeEncoding.PDF417
```

#### Issue 3: Reader License Not Needed

**Problem:** Separate reader license was required before.

**Solution:** IronBarcode includes reading with the same license:

```csharp
// Remove separate reader license registration

// IronBarcode - same key works for everything
IronBarCode.License.LicenseKey = "YOUR-KEY";

// Reading now works
var results = BarcodeReader.Read("image.png");
```

#### Issue 4: drawBarcode Method Not Found

**Problem:** `drawBarcode()` method doesn't exist.

**Solution:** Use format-specific save methods:

```csharp
// OnBarcode
barcode.drawBarcode("output.png");

// IronBarcode
barcode.SaveAsPng("output.png");
barcode.SaveAsJpeg("output.jpg");
barcode.SaveAsPdf("output.pdf");
```

### Migration Checklist

**Pre-Migration:**
- [ ] Inventory all OnBarcode usage points
- [ ] Note if Reader SDK is in use (separate product)
- [ ] Document current symbologies used
- [ ] Obtain IronBarcode license

**Migration:**
- [ ] Remove OnBarcode NuGet packages
- [ ] Remove OnBarcode DLL references (if manual)
- [ ] Add IronBarcode NuGet package
- [ ] Update namespace imports
- [ ] Configure IronBarcode license
- [ ] Convert generation code
- [ ] Convert reading code (if applicable)
- [ ] Remove separate reader license configuration

**Post-Migration:**
- [ ] Run test suite
- [ ] Verify all barcode types generate correctly
- [ ] Test reading functionality (now included)
- [ ] Compare output quality
- [ ] Update deployment configuration
- [ ] Remove old DLL files from repository

---

## Additional Resources

### Documentation Links

- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/) - Official guides and API reference
- [IronBarcode on NuGet](https://www.nuget.org/packages/BarCode) - Package download
- [OnBarcode Documentation](https://www.onbarcode.com/products/net_barcode/) - Official OnBarcode guides

### Code Example Files

Working code demonstrating the concepts in this article:

- [Separate Reader Demo](onbarcode-separate-reader.cs) - Generation vs unified SDK comparison
- [Manual Setup Comparison](onbarcode-manual-setup.cs) - Distribution model evolution

### Related Comparisons

- [Aspose.BarCode Comparison](../aspose-barcode/) - Enterprise SDK comparison
- [Spire.Barcode Comparison](../spire-barcode/) - Freemium model comparison
- [ZXing.Net Comparison](../zxing-net/) - Open-source alternative comparison

---

*Last verified: January 2026*
*Author: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*
