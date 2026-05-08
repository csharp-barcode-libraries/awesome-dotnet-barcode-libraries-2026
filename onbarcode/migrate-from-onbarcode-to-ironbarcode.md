# Migrating from OnBarcode to IronBarcode

This guide covers the complete migration path from OnBarcode to [IronBarcode](https://ironsoftware.com/csharp/barcode/). It addresses the three-step setup change, provides before-and-after code examples for every common pattern, maps the OnBarcode API to its IronBarcode equivalents, documents issues that arise during migration, and supplies a checklist of search terms for auditing your codebase before considering the migration complete.

## Why Migrate from OnBarcode

**Two-SKU Procurement Overhead:** OnBarcode publishes per-product pricing, but generation and reading are sold as separate products with separate license keys. The Generator Suite is further split into Linear (1D-only) and Linear + 2D tiers — Single Developer prices are $990 (Linear) and $1,690 (Linear + 2D), with five-developer and unlimited tiers escalating accordingly. The Reader SDK is priced separately starting from $990. An organization that needs full 2D generation plus reading must complete two purchases against two SKUs and reconcile two licenses at runtime; a single comparable IronBarcode tier covers both capabilities under one purchase.

**Split Product Overhead:** Generation and reading are separate products with separate NuGet packages, separate license namespaces, separate license keys, and separate version schedules. A project using both products must configure two license keys at startup, track two package versions across upgrades, and maintain awareness of two independent release cycles. When the reading requirement is added to a project that was originally generation-only, the entire procurement and integration process must be repeated for the second product.

**PDF Workflow Gaps:** OnBarcode has no native support for reading barcodes from PDF documents. Projects that process barcodes embedded in invoices, shipping manifests, purchase orders, or archival scans must acquire a separate PDF-to-image rendering library, integrate it into the pipeline, manage its license separately, and pass rendered pages to the OnBarcode Reader SDK one at a time. This produces a two-dependency solution where a one-dependency solution would otherwise suffice.

**API Verbosity:** The OnBarcode generator requires instantiating a `Barcode` object, assigning multiple properties — including properties with sensible defaults such as `Resolution`, `BarWidth`, and `BarHeight` — and then calling a generation method. The same output can be produced with significantly less code using a static factory API. Multiplied across batch generation, the property-assignment pattern produces large volumes of repetitive code that must be reviewed and maintained.

### The Fundamental Problem

The dual-license configuration is the most immediate friction point for any project that needs both generation and reading. OnBarcode requires two separate calls to two separate license namespaces at startup:

```csharp
// OnBarcode: two products, two keys, two registrations
using OnBarcode.Barcode; // shared namespace, but two distinct NuGet packages

License.RegisterLicense("GENERATOR-LICENSE-KEY");
License.RegisterLicense("READER-LICENSE-KEY");
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
| Format auto-detection on reading | No — explicit `BarcodeType` argument required on `BarcodeScanner.Scan` | Yes |
| Result metadata (format, position, page) | Raw `string[]` from `BarcodeScanner.Scan` | Yes — `BarcodeResults` with full metadata |
| QR Code with logo overlay | Manual GDI+ code required | Built-in |
| Published pricing | Yes — but split across multiple SKUs (Linear vs Linear+2D, Generator vs Reader) | Yes — single SKU covers all capabilities |
| Single license key for all capabilities | No — separate keys per product | Yes |
| NuGet distribution | `OnBarcode.Barcode.Generator`, `OnBarcode.Barcode.Reader`, plus framework variants | Single `BarCode` package |
| .NET target frameworks | .NET Standard 2.0, .NET 5/6/7/8, Core 3.x, .NET Framework 4.x | .NET Standard 2.0+ across modern .NET |
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

One package covers generation, reading, and PDF support. The NuGet package id is `BarCode`:

```bash
dotnet add package BarCode
```

No second package is required for reading. No second package is required for PDF support.

### Step 3: Update Namespaces and Replace the Dual License Configuration

Replace both OnBarcode using directives with the single IronBarcode import:

```csharp
// Remove
using OnBarcode.Barcode;

// Add
using IronBarCode;
```

Replace the two `License.RegisterLicense` calls with a single key assignment. Place this once, early in application startup, before any barcode operation runs:

```csharp
// Remove
License.RegisterLicense("GENERATOR-LICENSE-KEY");
License.RegisterLicense("READER-LICENSE-KEY");

// Add
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

## Code Migration Examples

### Code 128 Barcode Generation

The basic Code 128 case illustrates the property-assignment pattern versus the static factory pattern.

**OnBarcode Approach:**

```csharp
using OnBarcode.Barcode;

Linear barcode = new Linear();
barcode.Type = BarcodeType.CODE128;
barcode.Data = "SHIP-2024-001";
barcode.Resolution = 96;
barcode.X = 1;             // module width
barcode.BarcodeHeight = 80;
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

QRCode qr = new QRCode();
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
using OnBarcode.Barcode;

// Separate license call required — distinct from the generator license
License.RegisterLicense("READER-LICENSE-KEY");

// Static API; format must be specified up front. To scan multiple symbologies,
// call BarcodeScanner.Scan once per BarcodeType and combine the results.
string[] code128Results = BarcodeScanner.Scan("received-label.png", BarcodeType.CODE128);
string[] qrResults = BarcodeScanner.Scan("received-label.png", BarcodeType.QRCode);

foreach (string value in code128Results.Concat(qrResults))
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
// OnBarcode ships an optional PDF reader add-on (OnBarcode.Barcode.Reader.Document.PDF).
// Without it, projects must render PDF pages to images first using a separate library
// (PdfiumViewer, Aspose.PDF, etc.) and then call BarcodeScanner.Scan per page.
var pageImages = RenderPdfPagesToImages("invoices.pdf"); // external library call

using OnBarcode.Barcode;

foreach (var pageImage in pageImages)
{
    string[] results = BarcodeScanner.Scan(pageImage, BarcodeType.CODE128);
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
    Linear barcode = new Linear();
    barcode.Type = BarcodeType.CODE128;
    barcode.Data = item.Sku;
    barcode.Resolution = 96;
    barcode.X = 1;
    barcode.BarcodeHeight = 80;
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
| `License.RegisterLicense("key")` | `IronBarCode.License.LicenseKey = "key"` | Property assignment; called once at startup |
| Second `License.RegisterLicense` for Reader | (merged into single key above) | No separate reader license |
| `new Linear()` (1D) | `BarcodeWriter.CreateBarcode(data, encoding)` | Static factory; no per-symbology classes |
| `new QRCode()`, `new DataMatrix()`, `new PDF417()` (2D) | `BarcodeWriter.CreateBarcode(data, encoding)` | Single factory parameterized by encoding |
| `barcode.Type = BarcodeType.CODE128` | `BarcodeEncoding.Code128` as parameter | Passed as second argument to `CreateBarcode` |
| `barcode.Data = "..."` | First parameter of `CreateBarcode` | Data is the first argument |
| `barcode.drawBarcode("file.png")` | `.SaveAsPng("file.png")` | Format-named method |
| `barcode.drawBarcode("file.jpg")` | `.SaveAsJpeg("file.jpg")` | Format-named method |
| `barcode.drawBarcode("file.pdf")` | `.SaveAsPdf("file.pdf")` | Native PDF output |
| `BarcodeType.CODE128` | `BarcodeEncoding.Code128` | Direct mapping |
| `BarcodeType.EAN13` | `BarcodeEncoding.EAN13` | Direct mapping |
| `QRCode` class | `BarcodeEncoding.QRCode` | Direct mapping |
| `DataMatrix` class | `BarcodeEncoding.DataMatrix` | Direct mapping |
| `PDF417` class | `BarcodeEncoding.PDF417` | Direct mapping |
| `BarcodeScanner.Scan(path, BarcodeType.X)` | `BarcodeReader.Read(path)` — static | No format argument; auto-detection |
| `BarcodeScanner.Scan(...)` → `string[]` | `BarcodeReader.Read(...)` → `BarcodeResults` | Richer return type |
| `results[0]` (raw string) | `result.Value` | Decoded string value |
| N/A | `result.BarcodeType` | Detected format — not available from `BarcodeScanner.Scan` |
| N/A | `result.PageNumber` | Page source for PDF reads |
| `OnBarcode.Barcode.Reader.Document.PDF` add-on | `BarcodeReader.Read("document.pdf")` | Built-in PDF reading vs add-on package |
| Manual GDI+ overlay for QR logo | `QRCodeWriter.CreateQrCodeWithLogo()` | Built-in logo support |
| One scan call per `BarcodeType` | Not required | Auto-detection is the default |

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

**OnBarcode:** The reader product exports a static class named `BarcodeScanner`. IronBarcode has a static class named `BarcodeReader`. The names differ, so direct collision is rare — but if any file imports `OnBarcode.Barcode` and `IronBarCode` simultaneously during a phased migration, both namespaces define overlapping enum and helper names (e.g., `BarcodeType`) and the compiler cannot resolve them without a fully qualified reference.

**Solution:** Complete the migration of each file atomically — remove the `using OnBarcode.Barcode` directive and add `using IronBarCode` in the same edit. Do not leave both using directives present in any file. If a phased approach is required, use fully qualified names temporarily:

```csharp
// Temporary disambiguation during phased migration
var results = IronBarCode.BarcodeReader.Read("file.png");
```

### Issue 3: BarcodeType Argument Removal

**OnBarcode:** `BarcodeScanner.Scan(path, BarcodeType.X)` requires the format to be passed up front. To scan multiple symbologies in one image, code typically calls `BarcodeScanner.Scan` once per format and concatenates the results.

**Solution:** Remove the format specification entirely. `BarcodeReader.Read` performs automatic detection across all supported formats in a single call. No configuration is needed, and no per-format scan loop is required in IronBarcode.

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
grep -rn "RegisterLicense\|SetLicense" --include="*.cs" .

# Find all generator usage
grep -rn "new Linear(\|new QRCode(\|new DataMatrix(\|new PDF417(\|drawBarcode\|BarcodeType\." --include="*.cs" .

# Find all reader usage
grep -rn "BarcodeScanner\|\.Scan(" --include="*.cs" .

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
5. Run `dotnet add package BarCode`
6. Replace `using OnBarcode.Barcode` with `using IronBarCode` in each file
7. Remove both `License.RegisterLicense(...)` calls (Generator and Reader)
8. Add `IronBarCode.License.LicenseKey = "YOUR-KEY"` once at application startup
9. Convert each `new Linear()` / `new QRCode()` / `new DataMatrix()` / `new PDF417()` + property-assignment block to `BarcodeWriter.CreateBarcode(data, encoding)`
10. Replace `barcode.drawBarcode("file.png")` with `.SaveAsPng("file.png")` (or appropriate format method)
11. Replace `BarcodeScanner.Scan(path, BarcodeType.X)` with `BarcodeReader.Read(path)`
12. Update result handling from `string[]` indexing to `result.Value` and `result.BarcodeType` on `BarcodeResults`
13. Remove any PDF-to-image rendering code, or remove the `OnBarcode.Barcode.Reader.Document.PDF` add-on, used to work around OnBarcode's PDF model
14. Replace multi-step PDF rendering + scanning loops with a single `BarcodeReader.Read("file.pdf")` call

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
