# OnBarcode vs IronBarcode: C# Barcode Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

OnBarcode is a focused barcode generation library for .NET that has recently expanded its distribution through NuGet packages. Originally distributed as manual DLL downloads, OnBarcode positions itself as a straightforward generation solution. However, barcode reading requires a separate product purchase. This comprehensive guide examines OnBarcode's capabilities, its split product model, pricing transparency, and how it compares to [IronBarcode](https://ironsoftware.com/csharp/barcode/) for production deployments.

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

### Recent NuGet Transition

Historically, OnBarcode was distributed as downloadable DLL files. Users would:

1. Purchase license from website
2. Download DLL files
3. Manually add references to projects
4. Manage updates manually

Recently (2025-2026), OnBarcode added NuGet packages:

- `OnBarcode.Barcode.Generator`
- `OnBarcode.QRCode`
- Other symbology-specific packages

This modernization improves the developer experience but changes the traditional workflow.

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

### Pricing Opacity

OnBarcode's pricing is not publicly listed on their website in a clear price table. Instead:

- Contact sales for pricing
- Custom quotes based on needs
- License types available:
  - Single Developer
  - 5 Developer
  - Unlimited Developer
  - Server licenses

This "contact sales" model creates uncertainty:

- No upfront price comparison possible
- Negotiation may be required
- Prices may vary by customer
- Budget planning is difficult

---

## Capabilities Comparison

A direct comparison of what each library provides.

### Core Features

| Feature | OnBarcode | IronBarcode |
|---------|----------|-------------|
| Generation | Yes | Yes |
| Reading/Scanning | Separate purchase | Included |
| Symbology Count | 20+ | 50+ |
| Auto Format Detection | N/A (no reader) | Yes |
| PDF Native Support | No | Built-in |
| ML Error Correction | N/A (no reader) | Yes |
| QR with Logo | Manual implementation | Built-in |
| NuGet First-Class | Recent addition | Day one |
| Unified SDK | No (separate products) | Yes |

### Generation Capabilities

Both libraries support barcode generation, but with different approaches:

**OnBarcode Generation:**
```csharp
// Create barcode with specific settings
Barcode barcode = new Barcode();
barcode.Symbology = Symbology.Code128Auto;
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

**Via NuGet (Modern Approach):**
```bash
dotnet add package OnBarcode.Barcode.Generator
```

**Via Manual DLL (Traditional):**
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

// Apply license (format may vary)
OnBarcode.Barcode.License.SetLicense("YOUR-LICENSE-KEY");
// or
OnBarcode.Barcode.License.SetLicenseFile("license.lic");
```

**If Reader is Needed (Separate Installation):**
```bash
# Separate package
dotnet add package OnBarcode.Barcode.Reader

# Separate license
OnBarcode.Barcode.Reader.License.SetLicense("YOUR-READER-LICENSE-KEY");
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

// Create barcode object
Barcode barcode = new Barcode();
barcode.Symbology = Symbology.Code128Auto;
barcode.Data = "12345678";
barcode.Resolution = 96;
barcode.BarWidth = 1;
barcode.BarHeight = 80;
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

using OnBarcode.Barcode.Reader;

// Separate license for reader
OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");

// Then use reader
BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128 };
string[] results = reader.Scan("barcode.png");
```

**IronBarcode (included):**
```csharp
using IronBarCode;

// Same package, same license as generation
var results = BarcodeReader.Read("barcode.png");

foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.BarcodeType}: {barcode.Text}");
}
```

### Batch Processing

**OnBarcode Generation + Reading (two products):**
```csharp
using OnBarcode.Barcode;
using OnBarcode.Barcode.Reader; // Separate package

// Generate barcodes
foreach (var item in items)
{
    Barcode barcode = new Barcode();
    barcode.Symbology = Symbology.Code128Auto;
    barcode.Data = item.Code;
    barcode.drawBarcode($"{item.Id}.png");
}

// Read barcodes (requires separate Reader license)
BarcodeReader reader = new BarcodeReader();
foreach (var file in Directory.GetFiles(".", "*.png"))
{
    string[] results = reader.Scan(file);
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

// Generate base QR code
Barcode qr = new Barcode();
qr.Symbology = Symbology.QRCode;
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

OnBarcode does not publish clear pricing. Based on available information:

**License Types:**
- Single Developer: Contact sales
- 5 Developer: Contact sales
- Unlimited Developer: Contact sales (includes source code)
- Server licenses: Contact sales

**Separate Products:**
- Generator SDK: Requires quote
- Reader SDK: Separate quote required
- Bundle pricing: Unknown

### Pricing Opacity Concerns

The "contact sales" model creates challenges:

1. **No Price Comparison:** Cannot compare OnBarcode pricing with competitors without going through sales process

2. **Budget Planning Difficult:** Hard to allocate budget without knowing costs upfront

3. **Negotiation Variability:** Prices may vary based on customer, making value assessment difficult

4. **Hidden Total Cost:** Generator + Reader pricing unknown until both quotes received

### IronBarcode Pricing

Clear, published pricing:

| License Type | Price | Developers | Includes |
|-------------|-------|-----------|----------|
| Lite | $749 | 1 | Generation + Reading |
| Professional | $1,499 | 10 | Generation + Reading |
| Unlimited | $2,999 | Unlimited | Generation + Reading |

All licenses are perpetual with the option for support subscriptions.

### Cost Comparison Scenario

**Scenario: Development team needs generation AND reading**

```
OnBarcode:
  Generator SDK: $X (contact sales)
  Reader SDK: $Y (contact sales)
  Total: $X + $Y (unknown until both quotes)

  Additional considerations:
  - May need to negotiate
  - Prices may not be consistent
  - Budget approval difficult without firm numbers

IronBarcode Professional:
  Everything included: $1,499

  Known upfront:
  - Includes generation
  - Includes reading
  - Includes PDF support
  - Clear budget planning
```

### Value Transparency

| Aspect | OnBarcode | IronBarcode |
|--------|----------|-------------|
| Published pricing | No | Yes |
| Generation cost | Unknown | Included |
| Reading cost | Unknown (separate) | Included |
| Total cost visibility | Requires quotes | Clear |
| Budget planning | Difficult | Straightforward |

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
using OnBarcode.Barcode;
using OnBarcode.Barcode.Reader;  // If using reader
```

**After:**
```csharp
using IronBarCode;
```

### API Mapping Reference

#### Generation Classes

| OnBarcode | IronBarcode | Notes |
|----------|-------------|-------|
| `Barcode` class | `BarcodeWriter.CreateBarcode()` | Static factory |
| `barcode.Symbology` | Second parameter | Encoding enum |
| `barcode.Data` | First parameter | Data string |
| `barcode.drawBarcode()` | `barcode.SaveAsPng()` | Format-specific |
| `Symbology.Code128Auto` | `BarcodeEncoding.Code128` | Similar enums |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` | Same name |

#### Reading Classes (If Using Separate Reader)

| OnBarcode Reader | IronBarcode | Notes |
|-----------------|-------------|-------|
| `BarcodeReader` class | `BarcodeReader.Read()` | Static method |
| `reader.Scan()` | `BarcodeReader.Read()` | Similar signature |
| `reader.BarcodeTypes` | Not needed | Auto-detection |
| Return `string[]` | Return `BarcodeResults` | Richer result |

### Code Migration Examples

#### Example 1: Basic Generation

**Before (OnBarcode):**
```csharp
using OnBarcode.Barcode;

Barcode barcode = new Barcode();
barcode.Symbology = Symbology.Code128Auto;
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

Barcode qr = new Barcode();
qr.Symbology = Symbology.QRCode;
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
using OnBarcode.Barcode.Reader;

OnBarcode.Barcode.Reader.License.SetLicense("READER-KEY");

BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128 };
string[] results = reader.Scan("barcode.png");
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
    Barcode barcode = new Barcode();
    barcode.Symbology = Symbology.Code128Auto;
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

#### Issue 1: No Equivalent for Barcode Class

**Problem:** Looking for OnBarcode `Barcode` class equivalent.

**Solution:** Use static factory methods:

```csharp
// OnBarcode pattern
var barcode = new Barcode();
barcode.Symbology = ...;
barcode.Data = ...;

// IronBarcode pattern
var barcode = BarcodeWriter.CreateBarcode(data, encoding);
```

#### Issue 2: Symbology Enum Mapping

**Problem:** Symbology enum names differ.

**Solution:** Common mappings:

```csharp
// OnBarcode -> IronBarcode
Symbology.Code128Auto -> BarcodeEncoding.Code128
Symbology.QRCode -> BarcodeEncoding.QRCode
Symbology.EAN13 -> BarcodeEncoding.EAN13
Symbology.DataMatrix -> BarcodeEncoding.DataMatrix
Symbology.PDF417 -> BarcodeEncoding.PDF417
```

#### Issue 3: Reader License Not Needed

**Problem:** Separate reader license was required before.

**Solution:** IronBarcode includes reading with the same license:

```csharp
// Remove separate reader license
// OnBarcode.Barcode.Reader.License.SetLicense("...");

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
