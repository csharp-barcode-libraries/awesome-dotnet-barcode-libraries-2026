Migrating from OnBarcode to [IronBarcode](https://ironsoftware.com/csharp/barcode/) is a mechanical process. The API shapes are different enough that you'll touch every usage site, but the concepts map directly — generation is still generation, reading is still reading, format names are similar. What changes is the ceremony: two packages become one, two license calls become one, and the split `drawBarcode()` / `Scan()` pattern is replaced by a consistent static factory model.

This guide covers the full migration: why teams make this move, the three-step setup change, code-level before/after examples for every common pattern, and a checklist of grep terms to make sure nothing gets left behind.

## Why Migrate

Most OnBarcode migrations are triggered by one of three situations.

**Adding reading capability.** If your application was generation-only and you now need to read barcodes — for verification, receiving, document parsing, or any scanning workflow — you face a second sales conversation with OnBarcode, a second license purchase, and a second package to add and maintain. Teams doing this calculation frequently decide it is simpler to switch to a library where reading is already included.

**Budget planning friction.** OnBarcode does not publish prices for either the generator or the reader. When a project needs both capabilities, the total cost is unknown until two separate quotes have been negotiated. Some organizations cannot raise a purchase order without a firm number, and "contact sales twice" is not a firm number.

**PDF workflow requirements.** OnBarcode has no native PDF support. If your pipeline needs to read barcodes from PDF documents — invoices, shipping labels, archival scans — you need an additional library. IronBarcode's [PDF reading capability](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) is built into the same package with no extra dependency.

Any one of these is a legitimate reason. All three at once is a strong signal.

## Quick Start: Three Steps

### Step 1 — Remove OnBarcode Packages and References

```bash
dotnet remove package OnBarcode.Barcode.Generator
dotnet remove package OnBarcode.Barcode.Reader
```

If you're on the old DLL-based distribution rather than NuGet, also remove the manual reference from your `.csproj`:

```xml
<!-- Remove this block entirely -->
<ItemGroup>
  <Reference Include="OnBarcode.Barcode">
    <HintPath>lib\OnBarcode.Barcode.dll</HintPath>
    <Private>true</Private>
  </Reference>
</ItemGroup>
```

Delete the DLL files from `lib/` or wherever you stored them. They will not be needed.

### Step 2 — Add IronBarcode

```bash
dotnet add package IronBarcode
```

That is the only package required for both reading and writing.

### Step 3 — Update Namespaces and License Configuration

Replace the old using directives:

```csharp
// Remove both of these
using OnBarcode.Barcode;
using OnBarcode.Barcode.Reader;
```

With a single import:

```csharp
using IronBarCode;
```

Replace the dual license setup — which previously required two separate calls for two separate products:

```csharp
// Before: two license calls, two keys
OnBarcode.Barcode.License.SetLicense("GENERATOR-LICENSE-KEY");
OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");
```

With a single assignment:

```csharp
// After: one key covers everything
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

Place this once, early in application startup, before any barcode operations. It does not need to be called per-operation.

## Code Migration Examples

### Basic Code 128 Generation

The most common generation pattern in OnBarcode requires instantiating a `Barcode` object, assigning properties, and calling `drawBarcode()`. Note the lowercase `d` — this is the actual method name, not a typo.

**Before:**

```csharp
using OnBarcode.Barcode;

Barcode barcode = new Barcode();
barcode.Symbology = Symbology.Code128Auto;
barcode.Data = "12345678";
barcode.Resolution = 96;
barcode.BarWidth = 1;
barcode.BarHeight = 80;
barcode.ShowText = true;
barcode.drawBarcode("barcode.png");
```

**After:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128).SaveAsPng("barcode.png");
```

The data and the encoding are parameters to `CreateBarcode`. You do not need to set resolution, bar width, or other properties to get a valid barcode — the defaults produce scannable output. If you need custom sizing or styling, IronBarcode exposes those as chainable method calls rather than property assignments.

### QR Code Generation

**Before:**

```csharp
using OnBarcode.Barcode;

Barcode qr = new Barcode();
qr.Symbology = Symbology.QRCode;
qr.Data = "https://example.com";
qr.QRCodeDataMode = QRCodeDataMode.Auto;
qr.QRCodeECL = QRCodeECL.M;
qr.drawBarcode("qrcode.png");
```

**After:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode).SaveAsPng("qrcode.png");
```

Or, for QR code-specific features like logo embedding:

```csharp
var qr = QRCodeWriter.CreateQrCodeWithLogo("https://example.com", "logo.png", 500);
qr.SaveAsPng("qr-with-logo.png");
```

The OnBarcode equivalent requires manually handling the `Graphics.FromImage()` / `DrawImage()` overlay after generation. There is no built-in equivalent.

### Reading from an Image

Reading is where the migration eliminates a separate product entirely. Previously, this required the Reader SDK package, a separate license key, and an explicit `BarcodeTypes` array.

**Before (requires separate `OnBarcode.Barcode.Reader` package and license):**

```csharp
using OnBarcode.Barcode.Reader;

OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");

BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128 };
string[] results = reader.Scan("barcode.png");

foreach (string value in results)
    Console.WriteLine(value);
```

**After (same package as generation, same license key):**

```csharp
using IronBarCode;

var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
    Console.WriteLine($"{result.Value} ({result.Format})");
```

Two differences worth noting. First, `BarcodeReader.Read` is a static method — no instance, no configuration, no `BarcodeTypes` array. It auto-detects all supported formats. Second, the return type is `BarcodeResults` rather than `string[]`. Each result carries the decoded value, the barcode format, bounding box coordinates, and confidence information, rather than just the raw string. To [read barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) of unknown format, you need no additional configuration.

### Reading from a PDF

OnBarcode has no native PDF support. This migration example applies to teams who were rendering PDF pages to images themselves before scanning them.

**Before (manual PDF rendering required — typical pattern with a third-party library):**

```csharp
// Required a separate PDF-to-image library
// e.g., PdfiumViewer, Aspose.PDF, etc.
var pages = PdfToImages("document.pdf");

using OnBarcode.Barcode.Reader;
BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128, BarcodeType.QRCode };

foreach (var pageImage in pages)
{
    string[] results = reader.Scan(pageImage);
    // process results
}
```

**After:**

```csharp
using IronBarCode;

var results = BarcodeReader.Read("document.pdf");
foreach (var result in results)
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
```

No rendering step. No format specification. No third-party PDF library.

### Batch Generation

**Before:**

```csharp
using OnBarcode.Barcode;

foreach (var item in items)
{
    Barcode barcode = new Barcode();
    barcode.Symbology = Symbology.Code128Auto;
    barcode.Data = item.Code;
    barcode.drawBarcode($"{item.Id}.png");
}
```

**After:**

```csharp
using IronBarCode;

foreach (var item in items)
{
    BarcodeWriter.CreateBarcode(item.Code, BarcodeEncoding.Code128)
        .SaveAsPng($"{item.Id}.png");
}
```

### Batch Reading

**Before (requires Reader SDK):**

```csharp
using OnBarcode.Barcode.Reader;

BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128 };

foreach (var file in Directory.GetFiles(".", "*.png"))
{
    string[] results = reader.Scan(file);
    foreach (string value in results)
        Console.WriteLine($"{file}: {value}");
}
```

**After:**

```csharp
using IronBarCode;

var allResults = BarcodeReader.Read(Directory.GetFiles(".", "*.png"));
foreach (var result in allResults)
    Console.WriteLine($"{result.InputPath}: {result.Value}");
```

## API Mapping Reference

| OnBarcode | IronBarcode | Notes |
|---|---|---|
| `OnBarcode.Barcode.License.SetLicense("key")` | `IronBarCode.License.LicenseKey = "key"` | Single assignment, no method call |
| `OnBarcode.Barcode.Reader.License.SetLicense("key")` | (merged into single key) | No separate reader license |
| `new Barcode()` | `BarcodeWriter.CreateBarcode(data, encoding)` | Static factory, no instance required |
| `barcode.Symbology = Symbology.Code128Auto` | `BarcodeEncoding.Code128` (parameter) | Passed as second argument to `CreateBarcode` |
| `barcode.Data = "..."` | First parameter of `CreateBarcode` | Data goes first in the method call |
| `barcode.drawBarcode("file.png")` | `.SaveAsPng("file.png")` | Format-named save method |
| `barcode.drawBarcode("file.jpg")` | `.SaveAsJpeg("file.jpg")` | Explicit format in method name |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` | Same concept, different enum name |
| `Symbology.EAN13` | `BarcodeEncoding.EAN13` | Direct mapping |
| `Symbology.DataMatrix` | `BarcodeEncoding.DataMatrix` | Direct mapping |
| `Symbology.PDF417` | `BarcodeEncoding.PDF417` | Direct mapping |
| `new BarcodeReader() { BarcodeTypes = ... }` | Static — no instance | No format specification needed |
| `reader.Scan("file.png")` returns `string[]` | `BarcodeReader.Read("file.png")` returns `BarcodeResults` | Richer result type |
| `results[0]` (raw string) | `result.Value` | Access decoded value |
| N/A | `result.Format` | Format detected — not available in OnBarcode |
| N/A | `result.PageNumber` | Page number for PDF reads |
| `BarcodeType.Code128` | (not required) | Auto-detection removes this |

## Migration Checklist

Use these grep terms to find every usage site in your codebase before considering the migration complete.

**Namespaces to replace:**
- `using OnBarcode.Barcode`
- `using OnBarcode.Barcode.Reader`

**License calls to remove:**
- `OnBarcode.Barcode.License.SetLicense`
- `OnBarcode.Barcode.Reader.License.SetLicense`

**Generation patterns to convert:**
- `new Barcode()`
- `barcode.drawBarcode(`
- `Symbology.Code128Auto`
- `Symbology.QRCode`

**Reading patterns to convert:**
- `.Scan(`
- `BarcodeType[]`
- `new BarcodeReader()`

**Project file patterns to remove:**
- `OnBarcode.Barcode.dll` (manual DLL reference)
- `<HintPath>lib\OnBarcode` (manual reference path)

Run your test suite after each group of changes. Generation and reading can be migrated independently — if your application only generates barcodes, the reader patterns are irrelevant and the migration is smaller.

One thing that is not on this list: a second license call. Once you set `IronBarCode.License.LicenseKey`, there is nothing else to configure. The [IronBarcode licensing page](https://ironsoftware.com/csharp/barcode/licensing/) has the full details on key setup and environment variable configuration for Docker and cloud deployments.

The two-product model was the most inconvenient part of OnBarcode for teams that needed both read and write. That inconvenience is gone once you complete the migration.
