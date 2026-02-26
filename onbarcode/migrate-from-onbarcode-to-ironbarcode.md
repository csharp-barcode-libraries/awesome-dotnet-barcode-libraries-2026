# Migrating from OnBarcode to IronBarcode

This guide covers the complete migration path from OnBarcode to [IronBarcode](https://ironsoftware.com/csharp/barcode/). It addresses the three-step setup change, provides before-and-after code examples for every common pattern, maps the OnBarcode API to its IronBarcode equivalents, documents issues that arise during migration, and supplies a checklist of search terms for auditing your codebase before considering the migration complete.

## Why Migrate from OnBarcode

**Opaque Pricing Model:** OnBarcode does not publish prices for the Generator SDK or the Reader SDK. Each requires a separate sales conversation to obtain a quote. An organization that needs both capabilities must complete two independent procurement processes before it knows the total cost. Budget spreadsheets cannot be filled in, purchase orders cannot be raised, and timelines cannot be set until both conversations are finished.

**Split Product Overhead:** Generation and reading are separate products with separate NuGet packages, separate license namespaces, separate license keys, and separate version schedules. A project using both products must configure two license keys at startup, track two package versions across upgrades, and maintain awareness of two independent release cycles. When the reading requirement is added to a project that was originally generation-only, the entire procurement and integration process must be repeated for the second product.

**PDF Workflow Gaps:** OnBarcode has no native support for reading barcodes from PDF documents. Projects that process barcodes embedded in invoices, shipping manifests, purchase orders, or archival scans must acquire a separate PDF-to-image rendering library, integrate it into the pipeline, manage its license separately, and pass rendered pages to the OnBarcode Reader SDK one at a time. This produces a two-dependency solution where a one-dependency solution would otherwise suffice.

**API Verbosity:** The OnBarcode generator requires instantiating a `Barcode` object, assigning multiple properties — including properties with sensible defaults such as `Resolution`, `BarWidth`, and `BarHeight` — and then calling a generation method. The same output can be produced with significantly less code using a static factory API. Multiplied across batch generation, the property-assignment pattern produces large volumes of repetitive code that must be reviewed and maintained.

### The Fundamental Problem

The dual-license configuration is the most immediate friction point for any project that needs both generation and reading. OnBarcode requires two separate calls to two separate license namespaces at startup:

```csharp
// OnBarcode: two products, two keys, two calls
using OnBarcode.Barcode;
using OnBarcode.Barcode.Reader;

OnBarcode.Barcode.License.SetLicense("GENERATOR-LICENSE-KEY");
OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");
```

IronBarcode reduces this to a single property assignment that covers all capabilities — generation, reading, PDF support, and batch operations — under one key:

```csharp
// IronBarcode: one package, one key, one line
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

Every barcode operation in the application — regardless of whether it generates, reads, or processes PDFs — runs under that single configuration.

## IronBarcode vs OnBarcode: Feature Comparison

| Feature | OnBarcode | IronBarcode |
|---|---|---|
| Barcode generation | Yes | Yes |
| Barcode reading | Separate product, separate purchase | Included |
| PDF barcode reading | Not supported natively | Native — no external library required |
| Format auto-detection on reading | No — explicit `BarcodeType[]` required | Yes |
| Result metadata (format, position, page) | Not available — raw `string[]` only | Yes — `BarcodeResults` with full metadata |
| QR Code with logo overlay | Manual GDI+ code required | Built-in |
| Published pricing | No — contact sales for each product | Yes — perpetual tiers on website |
| Single license key for all capabilities | No — separate keys per product | Yes |
| NuGet distribution | Added 2025–2026 | Available since launch |
| Source code access | Unlimited tier only | Not available |
| .NET 8 / 9 support | Yes | Yes |
| Docker / cloud license configuration | Manual | Environment variable support |

## Quick Start: OnBarcode to IronBarcode Migration

### Step 1: Remove OnBarcode Package References

If your project uses the NuGet packages, remove both:

```bash
dotnet remove package OnBarcode.Barcode.Generator
dotnet remove package OnBarcode.Barcode.Reader
```

If your project uses the older DLL-based distribution, also remove the manual reference block from your `.csproj`:

```xml
<!-- Remove this block entirely -->
<ItemGroup>
  <Reference Include="OnBarcode.Barcode">
    <HintPath>lib\OnBarcode.Barcode.dll</HintPath>
    <Private>true</Private>
  </Reference>
</ItemGroup>
```

Delete the DLL files from the `lib/` directory or wherever they are stored. Leaving the old assemblies in place alongside the new NuGet package will produce namespace conflicts at compile time.

### Step 2: Add IronBarcode

One package covers generation, reading, and PDF support:

```bash
dotnet add package IronBarcode
```

No second package is required for reading. No second package is required for PDF support.

### Step 3: Update Namespaces and Replace the Dual License Configuration

Replace both OnBarcode using directives with the single IronBarcode import:

```csharp
// Remove
using OnBarcode.Barcode;
using OnBarcode.Barcode.Reader;

// Add
using IronBarCode;
```

Replace the two `SetLicense` calls with a single key assignment. Place this once, early in application startup, before any barcode operation runs:

```csharp
// Remove
OnBarcode.Barcode.License.SetLicense("GENERATOR-LICENSE-KEY");
OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");

// Add
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

## Code Migration Examples

### Code 128 Barcode Generation

The basic Code 128 case illustrates the property-assignment pattern versus the static factory pattern.

**OnBarcode Approach:**

```csharp
using OnBarcode.Barcode;

Barcode barcode = new Barcode();
barcode.Symbology = Symbology.Code128Auto;
barcode.Data = "SHIP-2024-001";
barcode.Resolution = 96;
barcode.BarWidth = 1;
barcode.BarHeight = 80;
barcode.ShowText = true;
barcode.drawBarcode("shipping-label.png");
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("SHIP-2024-001", BarcodeEncoding.Code128)
    .SaveAsPng("shipping-label.png");
```

The data and encoding type are passed as arguments to `CreateBarcode`. Properties such as resolution and bar height are not required to produce a scannable output; defaults are suitable for most uses. When custom dimensions are needed, they can be added as chained calls between `CreateBarcode` and `SaveAsPng` without introducing mutable object state.

### QR Code Generation with Logo

Logo-branded QR codes require OnBarcode to fall back to `System.Drawing` for the image overlay step. IronBarcode provides this as a built-in operation.

**OnBarcode Approach:**

```csharp
using OnBarcode.Barcode;
using System.Drawing;

Barcode qr = new Barcode();
qr.Symbology = Symbology.QRCode;
qr.Data = "https://example.com/product/4891";
qr.QRCodeDataMode = QRCodeDataMode.Auto;
qr.QRCodeECL = QRCodeECL.H; // H-level error correction required for logo overlay

Image qrImage = qr.drawBarcode();

using (Graphics g = Graphics.FromImage(qrImage))
{
    Image logo = Image.FromFile("brand-logo.png");
    int logoSize = qrImage.Width / 5;
    int x = (qrImage.Width - logoSize) / 2;
    int y = (qrImage.Height - logoSize) / 2;
    g.DrawImage(logo, x, y, logoSize, logoSize);
}

qrImage.Save("product-qr.png");
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

var qr = QRCodeWriter.CreateQrCodeWithLogo("https://example.com/product/4891", "brand-logo.png", 500);
qr.SaveAsPng("product-qr.png");
```

The error-correction level, logo sizing calculation, and image composition are handled internally. No `System.Drawing` dependency is required.

### Reading Barcodes from an Image

Reading from an image file illustrates the product consolidation most clearly. With OnBarcode, reading requires the separate Reader SDK, a separate license call, and explicit format specification. With IronBarcode, it is a single static call using the same package and key already in place.

**OnBarcode Approach:**

```csharp
using OnBarcode.Barcode.Reader;

// Separate license call required — distinct from the generator license
OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");

BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128, BarcodeType.QRCode };
string[] results = reader.Scan("received-label.png");

foreach (string value in results)
    Console.WriteLine(value);
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

var results = BarcodeReader.Read("received-label.png");
foreach (var result in results)
    Console.WriteLine($"{result.BarcodeType}: {result.Value}");
```

No instance creation, no format array, no separate license configuration. The format is detected automatically. Each result carries the decoded value and the detected format. For details on reading options and supported image types, see the guide on how to [read barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/).

### Reading Barcodes from a PDF

This migration example applies to teams that were rendering PDF pages to images using a third-party library before passing those images to the OnBarcode Reader SDK.

**OnBarcode Approach:**

```csharp
// OnBarcode has no native PDF support.
// A separate PDF rendering library was required — for example PdfiumViewer or Aspose.PDF.
var pageImages = RenderPdfPagesToImages("invoices.pdf"); // external library call

using OnBarcode.Barcode.Reader;
BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128, BarcodeType.QRCode };

foreach (var pageImage in pageImages)
{
    string[] results = reader.Scan(pageImage);
    foreach (string value in results)
        Console.WriteLine(value);
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

var results = BarcodeReader.Read("invoices.pdf");
foreach (var result in results)
    Console.WriteLine($"Page {result.PageNumber}: {result.BarcodeType} — {result.Value}");
```

No rendering step, no external PDF library, no format array. The [PDF barcode reading](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) capability processes all pages automatically and includes the page number in each result.

### Batch Generation

Generating barcodes for a collection of items follows the same pattern difference as single generation.

**OnBarcode Approach:**

```csharp
using OnBarcode.Barcode;

foreach (var item in inventoryItems)
{
    Barcode barcode = new Barcode();
    barcode.Symbology = Symbology.Code128Auto;
    barcode.Data = item.Sku;
    barcode.Resolution = 96;
    barcode.BarWidth = 1;
    barcode.BarHeight = 80;
    barcode.ShowText = true;
    barcode.drawBarcode($"labels/{item.Id}.png");
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

foreach (var item in inventoryItems)
{
    BarcodeWriter.CreateBarcode(item.Sku, BarcodeEncoding.Code128)
        .SaveAsPng($"labels/{item.Id}.png");
}
```

## OnBarcode API to IronBarcode Mapping Reference

| OnBarcode | IronBarcode | Notes |
|---|---|---|
| `OnBarcode.Barcode.License.SetLicense("key")` | `IronBarCode.License.LicenseKey = "key"` | Property assignment; called once at startup |
| `OnBarcode.Barcode.Reader.License.SetLicense("key")` | (merged into single key above) | No separate reader license |
| `new Barcode()` | `BarcodeWriter.CreateBarcode(data, encoding)` | Static factory; no object instance |
| `barcode.Symbology = Symbology.Code128Auto` | `BarcodeEncoding.Code128` as parameter | Passed as second argument to `CreateBarcode` |
| `barcode.Data = "..."` | First parameter of `CreateBarcode` | Data is the first argument |
| `barcode.drawBarcode("file.png")` | `.SaveAsPng("file.png")` | Format-named method |
| `barcode.drawBarcode("file.jpg")` | `.SaveAsJpeg("file.jpg")` | Format-named method |
| `barcode.drawBarcode("file.pdf")` | `.SaveAsPdf("file.pdf")` | Native PDF output |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` | Direct mapping |
| `Symbology.EAN13` | `BarcodeEncoding.EAN13` | Direct mapping |
| `Symbology.DataMatrix` | `BarcodeEncoding.DataMatrix` | Direct mapping |
| `Symbology.PDF417` | `BarcodeEncoding.PDF417` | Direct mapping |
| `new BarcodeReader() { BarcodeTypes = [...] }` | `BarcodeReader.Read(path)` — static | No instance, no format specification |
| `reader.Scan("file.png")` → `string[]` | `BarcodeReader.Read("file.png")` → `BarcodeResults` | Richer return type |
| `results[0]` (raw string) | `result.Value` | Decoded string value |
| N/A | `result.BarcodeType` | Detected format — not available in OnBarcode |
| N/A | `result.PageNumber` | Page source for PDF reads |
| N/A | `BarcodeReader.Read("document.pdf")` | Native PDF reading; no OnBarcode equivalent |
| Manual GDI+ overlay for QR logo | `QRCodeWriter.CreateQrCodeWithLogo()` | Built-in logo support |
| `BarcodeType[]` configuration | Not required | Auto-detection is the default |

## Common Migration Issues and Solutions

### Issue 1: DLL Reference Conflicts

**OnBarcode:** Projects on the legacy DLL distribution have `<Reference>` elements in `.csproj` pointing to `lib\OnBarcode.Barcode.dll` or similar paths. If the old DLL remains on disk when IronBarcode is added, the compiler will see two assemblies providing the same namespace prefixes and produce ambiguous reference errors.

**Solution:** Remove the `<ItemGroup>` reference block from `.csproj` before running `dotnet add package IronBarcode`. Delete the DLL files from disk. Verify with a clean build that no stale assembly references remain.

```bash
# Search for lingering OnBarcode DLL references in project files
grep -r "OnBarcode" --include="*.csproj" .
grep -r "OnBarcode" --include="*.props" .
```

### Issue 2: Namespace Conflicts on BarcodeReader

**OnBarcode:** The reader namespace exports a class named `BarcodeReader`. IronBarcode also has a static class named `BarcodeReader`. If any file imports both namespaces simultaneously during a phased migration, the compiler cannot resolve the name without a fully qualified reference.

**Solution:** Complete the migration of each file atomically — remove the `using OnBarcode.Barcode.Reader` directive and add `using IronBarCode` in the same edit. Do not leave both using directives present in any file. If a phased approach is required, use fully qualified names temporarily:

```csharp
// Temporary disambiguation during phased migration
var results = IronBarCode.BarcodeReader.Read("file.png");
```

### Issue 3: BarcodeType Array Removal

**OnBarcode:** The reader requires `reader.BarcodeTypes = new BarcodeType[] { ... }` to function. Developers migrating the reader code sometimes carry the habit of specifying formats explicitly and look for an equivalent configuration in IronBarcode.

**Solution:** Remove the format specification entirely. `BarcodeReader.Read` performs automatic detection across all supported formats. No configuration is needed, and no equivalent of `BarcodeType[]` exists in IronBarcode because it is not required.

```csharp
// No format configuration required
var results = BarcodeReader.Read("label.png");
```

## OnBarcode Migration Checklist

### Pre-Migration Tasks

Audit the codebase to locate all OnBarcode usage before making changes:

```bash
# Find all OnBarcode namespace imports
grep -rn "using OnBarcode" --include="*.cs" .

# Find all license configuration calls
grep -rn "SetLicense" --include="*.cs" .

# Find all generator usage
grep -rn "new Barcode()\|drawBarcode\|Symbology\." --include="*.cs" .

# Find all reader usage
grep -rn "BarcodeReader\|\.Scan(\|BarcodeType\[\]" --include="*.cs" .

# Find DLL references in project files
grep -rn "OnBarcode" --include="*.csproj" .
grep -rn "OnBarcode" --include="*.sln" .
```

Document which files use generation only, which use reading, and which use both. Note any files that include the manual `<Reference>` DLL block rather than a NuGet package reference.

### Code Update Tasks

1. Remove `OnBarcode.Barcode.Generator` NuGet package
2. Remove `OnBarcode.Barcode.Reader` NuGet package
3. Remove any manual `<Reference>` DLL entries from `.csproj` files
4. Delete OnBarcode DLL files from the repository
5. Run `dotnet add package IronBarcode`
6. Replace `using OnBarcode.Barcode` with `using IronBarCode` in each file
7. Replace `using OnBarcode.Barcode.Reader` with `using IronBarCode` in each file
8. Remove both `OnBarcode.Barcode.License.SetLicense(...)` calls
9. Remove both `OnBarcode.Barcode.Reader.License.SetLicense(...)` calls
10. Add `IronBarCode.License.LicenseKey = "YOUR-KEY"` once at application startup
11. Convert each `new Barcode()` + property-assignment block to `BarcodeWriter.CreateBarcode(data, encoding)`
12. Replace `barcode.drawBarcode("file.png")` with `.SaveAsPng("file.png")` (or appropriate format method)
13. Replace `new BarcodeReader() { BarcodeTypes = [...] }` + `.Scan(path)` with `BarcodeReader.Read(path)`
14. Update result handling from `string[]` indexing to `result.Value` and `result.BarcodeType` on `BarcodeResults`
15. Remove any PDF-to-image rendering code used to work around OnBarcode's PDF limitation
16. Replace multi-step PDF rendering + scanning loops with a single `BarcodeReader.Read("file.pdf")` call

### Post-Migration Testing

- Verify each barcode type used in the project (Code 128, QR, EAN, Data Matrix, PDF417) generates a scannable output
- Scan generated barcodes with a physical scanner or mobile device to confirm readability
- Verify that barcode reading returns correct values for known test images
- Confirm that PDF reading returns correct values and correct page numbers for multi-page test documents
- Run the application through any CI environment to confirm that license key configuration works correctly in the pipeline
- Check that no `OnBarcode` string remains in `.cs`, `.csproj`, or configuration files after the migration is complete

## Key Benefits of Migrating to IronBarcode

**Consolidated Package and License:** After migration, one NuGet package and one license key cover all barcode operations. There are no separate products to procure, no independent version schedules to track, and no dual license configuration to maintain at startup. The complexity of a two-product arrangement is replaced by a single dependency.

**Transparent Procurement:** IronBarcode publishes perpetual license tiers with prices on the product website. An organization can determine total cost, submit a purchase order, and complete procurement without a sales conversation. The [IronBarcode licensing page](https://ironsoftware.com/csharp/barcode/licensing/) includes pricing for all tiers and details on key configuration for Docker and cloud environments.

**Native PDF Support:** Projects that previously required a separate PDF rendering library to process barcode-embedded PDFs can eliminate that dependency entirely. A single `BarcodeReader.Read` call processes all pages of a PDF document without any rendering step, and each result includes the page number from which it was extracted.

**Richer Reading Results:** The `BarcodeResults` return type provides the decoded value, the detected barcode format, bounding box coordinates within the source image, and page number for PDF reads. Applications that previously worked with a flat `string[]` can now surface format information, support annotation workflows, or implement confidence-based filtering without additional processing.

**Reduced API Surface:** The static factory pattern replaces the property-assignment model. Common generation operations require fewer lines of code, and reading operations require no instance creation or format pre-configuration. Batch operations benefit from a smaller per-item code footprint that is easier to review and maintain over time.
