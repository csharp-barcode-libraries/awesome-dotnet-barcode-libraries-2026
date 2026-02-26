# Migrating from NetBarcode to IronBarcode

This guide covers the complete migration path from NetBarcode to [IronBarcode](https://ironsoftware.com/csharp/barcode/), with package removal steps, namespace updates, API translation examples, and a checklist for locating every call site before the migration begins. The guide addresses the common scenario in which a project starts with NetBarcode for 1D barcode generation and later requires 2D formats, reading capability, or clarification of the SixLabors.ImageSharp licence obligation — all of which are outside NetBarcode's scope.

## Why Migrate from NetBarcode

**Format Ceiling:** NetBarcode's `Type` enum defines exactly 14 barcode formats, all linear. There is no entry for QR Code, DataMatrix, PDF417, or Aztec. When a project adds a QR code requirement — for contactless delivery links, mobile deep links, or a pharmaceutical DataMatrix mandate — NetBarcode cannot satisfy it. The only path within the library's design is to stop using it for that format and introduce a separate package. Each additional format requirement that falls outside the 14-entry enum adds another library to the dependency tree.

**No Reading API:** NetBarcode generates barcode images and provides no method to decode them. A project that needs to scan return shipment labels, extract barcode values from supplier invoices, or validate that a printed barcode matches its source data must add a separate reading library. ZXing.Net is the most common choice, introducing a third API surface in a codebase that already holds NetBarcode and, typically, a QR-specific library.

**ImageSharp Commercial Licence:** NetBarcode depends on SixLabors.ImageSharp, which uses a split commercial licence. The free tier applies to open-source projects and companies below a defined annual revenue threshold; above that threshold a commercial licence is required. This condition is embedded in the transitive dependency chain and is not surfaced by NuGet during installation. A retail or logistics company processing barcodes at scale — precisely the use case NetBarcode targets — may be operating above the threshold without having evaluated the obligation.

**Multi-Library Accumulation:** Projects that reach for QRCoder when QR codes are needed, and then ZXing.Net when reading becomes necessary, accumulate three separate barcode-related dependencies. Each has its own release schedule, its own ImageSharp version expectation, and its own API to maintain. Consolidating to a single library that covers generation, reading, 1D, and 2D eliminates the cross-library version management problem.

### The Fundamental Problem

A project that starts with NetBarcode alone and then grows to require 2D formats ends up with this kind of split import:

```csharp
// NetBarcode for 1D — one library, one API
using NetBarcode;
var code128 = new Barcode("12345", Type.Code128);
code128.SaveImageFile("label.png");

// QRCoder added separately for QR — second library, second API
using QRCoder;
var qrGenerator = new QRCodeGenerator();
var qrData = qrGenerator.CreateQrCode("https://example.com", QRCodeGenerator.ECCLevel.M);
var png = new PngByteQRCode(qrData).GetGraphic(20);
File.WriteAllBytes("qr.png", png);
```

After migrating to IronBarcode, the same two outputs come from a single import:

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128)
    .SaveAsPng("label.png");

BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");
```

## IronBarcode vs NetBarcode: Feature Comparison

| Feature | NetBarcode | IronBarcode |
|---|---|---|
| 1D barcode generation | Yes | Yes |
| 2D barcode generation (QR, DataMatrix, PDF417, Aztec) | No | Yes |
| Barcode reading from images | No | Yes |
| Barcode reading from PDF documents | No | Yes |
| 1D format count | 14 | 30+ |
| 2D format count | 0 | 8+ |
| Total symbologies | 14 | 50+ |
| GS1-128, GS1 DataBar | No | Yes |
| Postal formats | No | Yes |
| SVG output | No | Yes |
| Batch processing | Manual | Built-in |
| ImageSharp dependency | Yes (split licence) | No |
| Commercial support | Community | Professional |
| License model | MIT (+ ImageSharp conditions) | Commercial |

## Quick Start: NetBarcode to IronBarcode Migration

### Step 1: Replace NuGet Package

Remove NetBarcode and its transitive ImageSharp dependency:

```bash
dotnet remove package NetBarcode
dotnet remove package SixLabors.ImageSharp
```

Note that `SixLabors.ImageSharp` may reappear in the restored package list if other packages in the project also reference it transitively. After removal, run `dotnet list package --include-transitive` to confirm whether ImageSharp remains and whether its commercial licence condition still applies to the project.

### Step 2: Add IronBarcode

```bash
dotnet add package IronBarcode
```

The NuGet package name is `IronBarcode`; the namespace used in code is `IronBarCode` (note the capital C).

### Step 3: Update Namespaces and Initialize License

Replace the old using directives in every file that contained NetBarcode calls:

```csharp
// Remove these:
// using NetBarcode;
// using SixLabors.ImageSharp;
// using SixLabors.ImageSharp.PixelFormats;
// using SixLabors.Fonts;

// Add this:
using IronBarCode;
```

Add license initialization once at application startup — in `Program.cs`, `Startup.cs`, or the project's entry point:

```csharp
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

A free trial key is available for evaluation; purchasing a license removes the trial watermark from generated barcode images.

## Code Migration Examples

### Code128 Generation

The most common NetBarcode usage pattern translates directly to IronBarcode with a constructor-to-static-method change and a method rename.

**NetBarcode Approach:**

```csharp
using NetBarcode;

var barcode = new Barcode("12345678901234", Type.Code128);
barcode.SaveImageFile("shipping-label.png");
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678901234", BarcodeEncoding.Code128)
    .SaveAsPng("shipping-label.png");
```

The `new Barcode(data, Type.X)` constructor becomes `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.X)`. The `SaveImageFile()` method becomes `SaveAsPng()`, `SaveAsJpeg()`, or `SaveAsBitmap()` depending on the output format required. All available [1D barcode generation](https://ironsoftware.com/csharp/barcode/how-to/create-1d-barcodes/) options are documented in the IronBarcode how-to guides.

### EAN-13 and UPC-A

Retail product barcode types map directly between libraries with a constant rename and a method rename.

**NetBarcode Approach:**

```csharp
using NetBarcode;

var ean13 = new Barcode("5901234123457", Type.EAN13);
ean13.SaveImageFile("product-ean.png");

var upca = new Barcode("012345678905", Type.UPCA);
upca.SaveImageFile("product-upc.png");
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
    .SaveAsPng("product-ean.png");

BarcodeWriter.CreateBarcode("012345678905", BarcodeEncoding.UPCA)
    .SaveAsPng("product-upc.png");
```

The encoding constant names match the NetBarcode `Type` enum member names where direct equivalents exist.

### GetImage() to ToPngBinaryData()

NetBarcode's `GetImage()` method returns `SixLabors.ImageSharp.Image<Rgba32>`, which requires explicit ImageSharp imports in calling code and ties downstream logic to the ImageSharp API. IronBarcode exposes binary and stream output methods directly on the `GeneratedBarcode` object, eliminating the ImageSharp dependency from calling code entirely.

**NetBarcode Approach:**

```csharp
using NetBarcode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

var barcode = new Barcode("12345", Type.Code128);
Image<Rgba32> image = barcode.GetImage();

using var stream = new MemoryStream();
image.SaveAsPng(stream);
byte[] bytes = stream.ToArray();
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

var barcode = BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128);

// Retrieve bytes directly — no ImageSharp type in calling code
byte[] bytes = barcode.ToPngBinaryData();

// Or write to a stream directly
using var stream = new MemoryStream();
barcode.SaveAsPng(stream);
```

Code that performed further ImageSharp manipulations on the `Image<Rgba32>` result — resizing, compositing, format conversion — will need those operations evaluated individually, as IronBarcode does not expose an ImageSharp object.

### Replacing NetBarcode and QRCoder Together

Projects running NetBarcode alongside QRCoder can replace both at once. IronBarcode's 2D generation uses the same `BarcodeWriter.CreateBarcode` method as its 1D generation; only the `BarcodeEncoding` constant differs. Refer to the [2D barcode generation](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/) guide for encoding options and configuration.

**NetBarcode and QRCoder Approach:**

```csharp
using NetBarcode;
using QRCoder;

// 1D with NetBarcode
var code128 = new Barcode("12345", Type.Code128);
code128.SaveImageFile("label.png");

// QR with QRCoder
var qrGenerator = new QRCodeGenerator();
var qrData = qrGenerator.CreateQrCode("https://example.com", QRCodeGenerator.ECCLevel.M);
var qrCode = new PngByteQRCode(qrData);
File.WriteAllBytes("qr.png", qrCode.GetGraphic(20));
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// 1D — same API as 2D
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128)
    .SaveAsPng("label.png");

// QR — change the encoding constant, nothing else
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");

// Additional 2D formats available with no further imports
BarcodeWriter.CreateBarcode("01034531200000111719112510ABCD1234", BarcodeEncoding.DataMatrix)
    .SaveAsPng("pharma-label.png");
```

### Adding Reading Capability

If the project also used ZXing.Net for reading, that dependency can be removed alongside NetBarcode. IronBarcode's `BarcodeReader` handles images and PDF documents with automatic format detection.

**ZXing.Net Approach:**

```csharp
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

var reader = new BarcodeReaderGeneric();
reader.Options = new DecodingOptions { TryHarder = true };

// Bitmap loading and format-specific handling required before decode call
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Read from an image file — automatic format detection
var imageResults = BarcodeReader.Read("shipping-label.png");
foreach (var r in imageResults)
{
    Console.WriteLine($"{r.BarcodeType}: {r.Value}");
}

// Read from a PDF document — no separate PDF library needed
var pdfResults = BarcodeReader.Read("supplier-invoice.pdf");
foreach (var r in pdfResults)
{
    Console.WriteLine($"Page {r.PageNumber}: {r.BarcodeType}: {r.Value}");
}
```

Full documentation for reading options is available in the [read barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) guide, covering speed tuning, multi-barcode detection, and image correction settings.

## NetBarcode API to IronBarcode Mapping Reference

| NetBarcode | IronBarcode | Notes |
|---|---|---|
| `new Barcode(data, Type.Code128)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)` | Constructor → static method |
| `new Barcode(data, Type.EAN13)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.EAN13)` | Direct mapping |
| `new Barcode(data, Type.UPCA)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.UPCA)` | Direct mapping |
| `new Barcode(data, Type.Code39)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code39)` | Direct mapping |
| `new Barcode(data, Type.EAN8)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.EAN8)` | Direct mapping |
| `new Barcode(data, Type.UPCE)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.UPCE)` | Direct mapping |
| `new Barcode(data, Type.ITF)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.ITF)` | Direct mapping |
| `new Barcode(data, Type.Codabar)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Codabar)` | Direct mapping |
| `new Barcode(data, Type.MSI)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.MSI)` | Direct mapping |
| `new Barcode(data, Type.Code93)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code93)` | Direct mapping |
| `barcode.SaveImageFile("x.png")` | `.SaveAsPng("x.png")` | Method rename |
| `barcode.SaveImageFile("x.jpg")` | `.SaveAsJpeg("x.jpg")` | Method rename |
| `barcode.GetImage()` → `Image<Rgba32>` | `.ToPngBinaryData()` or `.SaveAsPng()` | No ImageSharp type exposed |
| No `Type.QRCode` | `BarcodeEncoding.QRCode` | New capability |
| No `Type.DataMatrix` | `BarcodeEncoding.DataMatrix` | New capability |
| No `Type.PDF417` | `BarcodeEncoding.PDF417` | New capability |
| No `Type.Aztec` | `BarcodeEncoding.Aztec` | New capability |
| No reading API | `BarcodeReader.Read(path)` | New capability |
| `using NetBarcode;` | `using IronBarCode;` | Namespace replacement |
| `using SixLabors.ImageSharp;` | Remove | No longer needed |
| `using SixLabors.ImageSharp.PixelFormats;` | Remove | No longer needed |
| `using SixLabors.Fonts;` | Remove | No longer needed |

The complete list of encoding constants is in the [supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) reference.

## Common Migration Issues and Solutions

### Issue 1: ImageSharp Transitive Dependency Remains After Package Removal

**Problem:** After running `dotnet remove package NetBarcode`, `dotnet restore` shows SixLabors.ImageSharp still present in the dependency tree because another package in the project also references it.

**Solution:** Run `dotnet list package --include-transitive` to identify which package is still pulling in ImageSharp. If it is only NetBarcode, removing the explicit package reference and restoring should clear it. If another package is responsible, that package's ImageSharp dependency and its commercial licence condition remain in scope independently of the NetBarcode removal.

```bash
dotnet list package --include-transitive | grep -i imagesharp
```

### Issue 2: Image<Rgba32> Type Cannot Be Resolved After Removal

**Problem:** After removing the SixLabors.ImageSharp package, any variable declared as `Image<Rgba32>` or parameter typed as `SixLabors.ImageSharp.Image<Rgba32>` produces a compile error because the type is no longer available.

**Solution:** Replace each usage with the appropriate IronBarcode output method. If the variable was used to obtain bytes, replace with `.ToPngBinaryData()`. If it was written to a stream, replace with `.SaveAsPng(stream)`. If it was saved to a file, replace with `.SaveAsPng(path)`.

```csharp
// Before — requires SixLabors.ImageSharp reference
Image<Rgba32> img = barcode.GetImage();
img.SaveAsPng(stream);

// After — no external image type required
barcode.SaveAsPng(stream);
```

### Issue 3: v1.8 Breaking Change Context

**Problem:** Codebases that were written against NetBarcode before v1.8 may have `GetImage()` calls that stored the result without explicit typing, relying on the pre-v1.8 internal return type. These calls may already be broken after a NetBarcode update, independently of the IronBarcode migration.

**Solution:** Use the IronBarcode migration as an opportunity to resolve any pre-existing v1.8 breakage at the same time. All `GetImage()` call sites should be located with a project-wide search and replaced with the appropriate IronBarcode output method during the migration pass. The grep commands in the migration checklist below identify these locations.

## NetBarcode Migration Checklist

### Pre-Migration

Audit all NetBarcode usage before making any changes:

```bash
grep -r "using NetBarcode" ./src
grep -r "new Barcode(" ./src
grep -r "Type\.Code128\|Type\.EAN13\|Type\.UPCA\|Type\.Code39" ./src
grep -r "Type\.EAN8\|Type\.UPCE\|Type\.ITF\|Type\.Codabar\|Type\.MSI" ./src
grep -r "SaveImageFile(" ./src
grep -r "GetImage(" ./src
grep -r "using SixLabors\.ImageSharp" ./src
grep -r "Image<Rgba32>" ./src
```

- Identify every file that requires a using directive change
- Note all `GetImage()` call sites and how the returned `Image<Rgba32>` object is used downstream
- Check whether QRCoder or ZXing.Net are present and can be removed in the same pass
- Run `dotnet list package --include-transitive` to document the current ImageSharp dependency state before removal
- Obtain an IronBarcode license key (a free trial key is available without purchase)

### Code Update

1. Run `dotnet remove package NetBarcode`
2. Run `dotnet remove package SixLabors.ImageSharp` if not used by other packages
3. Run `dotnet add package IronBarcode`
4. Add `IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";` at application startup
5. Replace `using NetBarcode;` with `using IronBarCode;` in each affected file
6. Remove `using SixLabors.ImageSharp;`, `using SixLabors.ImageSharp.PixelFormats;`, and `using SixLabors.Fonts;` from all files
7. Translate each `new Barcode(data, Type.X)` call to `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.X)` using the API mapping table
8. Replace `SaveImageFile()` with `SaveAsPng()` or the appropriate format-specific method
9. Replace `GetImage()` calls with `ToPngBinaryData()`, `SaveAsPng()`, or `SaveAsPng(stream)` as appropriate
10. If removing QRCoder: replace `QRCodeGenerator` + `PngByteQRCode` chains with `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)`
11. If removing ZXing.Net: replace reader setup with `BarcodeReader.Read(path)`

### Post-Migration Testing

- Build the project and confirm zero compile errors
- Run existing unit tests against generated barcode output
- Visually verify barcode images for correct symbology and legible content
- Verify QR code output if that format was added during migration
- Test reading functionality if barcode scanning was added
- Run `dotnet list package --include-transitive` to confirm ImageSharp is no longer present if it was removed
- Validate that the license key initialization runs before the first barcode operation

## Key Benefits of Migrating to IronBarcode

**Unified Format Coverage:** After migration, QR codes, DataMatrix, PDF417, Aztec, and all other 2D formats are available through the same API call used for Code128 and EAN-13. No second library is needed when format requirements expand, and the `BarcodeEncoding` constant is the only thing that changes between format types.

**Built-In Reading:** `BarcodeReader.Read()` handles image files and PDF documents with automatic format detection. Projects that previously required ZXing.Net for reading can remove that dependency and consolidate to a single barcode library, reducing the API surface area that developers on the project must learn.

**No ImageSharp Obligation:** IronBarcode has no SixLabors.ImageSharp dependency. After migration, the split commercial licence condition embedded in NetBarcode's transitive dependencies no longer applies to the project. Licence compliance reviews become simpler because the barcode library's obligations are stated directly and do not depend on a third-party image processing library's revenue threshold.

**Stable Public API:** IronBarcode does not expose third-party types in its public API surface. The `GeneratedBarcode` return type from `BarcodeWriter.CreateBarcode` is IronBarcode's own type, which means future updates to IronBarcode's internal rendering will not produce breaking changes in calling code the way that NetBarcode's v1.8 `GetImage()` type change did.

**Single Dependency to Maintain:** Replacing NetBarcode, QRCoder, and ZXing.Net with IronBarcode reduces three upgrade cycles, three sets of release notes, and three potential version conflicts to one. As .NET versions advance and dependency updates become necessary, the maintenance work scales with a single library instead of three.
