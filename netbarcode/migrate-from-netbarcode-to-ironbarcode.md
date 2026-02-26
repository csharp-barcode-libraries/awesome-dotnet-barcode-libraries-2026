# Migrate from NetBarcode to IronBarcode

Migrating from NetBarcode to IronBarcode is usually a one-afternoon project. The API shapes are similar enough that most call sites change in a single find-and-replace pass. What changes after the migration is what the codebase can do: QR codes, DataMatrix, PDF417, barcode reading, and PDF processing become available through the same library that was already handling Code128 and EAN-13.

This guide covers the removal steps, the API translation, code-level migration examples for each common scenario, and a checklist of grep terms to locate every call site before you start.

---

## Why Migrate

NetBarcode is a reasonable 1D generator. The migration pressure comes from three directions, and most teams encounter at least one before a project reaches its first anniversary.

**The QR code requirement arrives.** A project that started with shipping labels or product UPCs eventually gets a request for QR codes — contactless delivery links, mobile app deep links, marketing URLs. NetBarcode's `Type` enum has no entry for QR Code. The team adds QRCoder. Now the codebase has two barcode libraries with two APIs and a shared ImageSharp dependency that may drift out of sync across versions. When QR requirements expand to DataMatrix or PDF417, the library count grows again.

**Reading is needed.** Scanning a return label, extracting barcode values from a supplier invoice, validating that a printed barcode is correct — none of this is possible with NetBarcode. The library generates images and stops there. ZXing.Net is the usual addition, bringing a third API to learn and maintain.

**The ImageSharp commercial license applies.** NetBarcode depends on SixLabors.ImageSharp, which requires a commercial license for companies above $1M annual gross revenue. For a retail or logistics business — precisely the use case NetBarcode targets — this is a real cost hidden inside what appears to be a free dependency. IronBarcode has no ImageSharp dependency.

---

## Quick Start: Three Steps

### Step 1 — Remove NetBarcode and ImageSharp

```bash
dotnet remove package NetBarcode
dotnet remove package SixLabors.ImageSharp  # only if not used by other packages
```

Check your `.csproj` after removal. If other packages in your project still pull in ImageSharp transitively, the `SixLabors.ImageSharp` package reference may reappear — that's expected. If your own code imports `SixLabors.ImageSharp` directly, those imports will be removed during the code migration step.

### Step 2 — Add IronBarcode

```bash
dotnet add package IronBarcode
```

This adds the `IronBarCode` NuGet package. The package name on NuGet is `IronBarcode`; the namespace in code is `IronBarCode`.

### Step 3 — Update Usings and Add License Init

Remove the old using directives and add the IronBarcode equivalents at the top of each file that contained NetBarcode calls:

```csharp
// Remove these:
// using NetBarcode;
// using SixLabors.ImageSharp;
// using SixLabors.ImageSharp.PixelFormats;
// using SixLabors.Fonts;

// Add this:
using IronBarCode;
```

Add the license initialization once at application startup — in `Program.cs`, `Startup.cs`, or whatever entry point your project uses:

```csharp
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

A free trial key is available without a purchase. Licensed versions remove the trial watermark from generated images.

---

## Code Migration Examples

### Example 1: Code128 Generation

This is the most common NetBarcode usage pattern. The translation is direct.

**Before:**

```csharp
using NetBarcode;
using SixLabors.ImageSharp;

var barcode = new Barcode("12345678901234", Type.Code128);
barcode.SaveImageFile("shipping-label.png");
```

**After:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678901234", BarcodeEncoding.Code128)
    .SaveAsPng("shipping-label.png");
```

The `new Barcode(data, Type.X)` constructor becomes `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.X)`. The `SaveImageFile()` method becomes `SaveAsPng()` — or `SaveAsJpeg()`, `SaveAsBitmap()`, `SaveAsTiff()` depending on your output format.

### Example 2: EAN-13 and UPC-A

```csharp
// Before
var ean13 = new Barcode("5901234123457", Type.EAN13);
ean13.SaveImageFile("product-ean.png");

var upca = new Barcode("012345678905", Type.UPCA);
upca.SaveImageFile("product-upc.png");

// After
BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
    .SaveAsPng("product-ean.png");

BarcodeWriter.CreateBarcode("012345678905", BarcodeEncoding.UPCA)
    .SaveAsPng("product-upc.png");
```

### Example 3: Working with the Image Object

NetBarcode's `GetImage()` method returns `SixLabors.ImageSharp.Image<Rgba32>`, which required the ImageSharp namespace import and ImageSharp-typed variables in calling code. IronBarcode's equivalent returns a `GeneratedBarcode` object with its own output methods.

**Before:**

```csharp
using NetBarcode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

var barcode = new Barcode("12345", Type.Code128);
Image<Rgba32> image = barcode.GetImage();

// Further processing with ImageSharp API...
using var stream = new MemoryStream();
image.SaveAsPng(stream);
byte[] bytes = stream.ToArray();
```

**After:**

```csharp
using IronBarCode;

var barcode = BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128);
byte[] bytes = barcode.ToPngBinaryData();

// Or save directly:
barcode.SaveAsPng("output.png");

// Or write to a stream:
using var stream = new MemoryStream();
barcode.SaveAsPng(stream);
```

### Example 4: Adding QR Code Support (New Capability)

If you were previously running NetBarcode alongside QRCoder, the QRCoder code can be replaced at the same time. The [2D barcode generation](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/) API in IronBarcode is identical in structure to the 1D generation API — same method, different encoding constant.

**Before (NetBarcode + QRCoder):**

```csharp
// 1D with NetBarcode
using NetBarcode;
var code128 = new Barcode("12345", Type.Code128);
code128.SaveImageFile("label.png");

// QR with QRCoder (separate library)
using QRCoder;
var qrGenerator = new QRCodeGenerator();
var qrData = qrGenerator.CreateQrCode("https://example.com", QRCodeGenerator.ECCLevel.M);
var qrCode = new PngByteQRCode(qrData);
File.WriteAllBytes("qr.png", qrCode.GetGraphic(20));
```

**After (IronBarcode only):**

```csharp
using IronBarCode;

// 1D — same as before
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128)
    .SaveAsPng("label.png");

// QR — same API, no second library
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");

// DataMatrix, PDF417, Aztec — all the same
BarcodeWriter.CreateBarcode("01034531200000111719112510ABCD1234", BarcodeEncoding.DataMatrix)
    .SaveAsPng("pharma-label.png");
```

### Example 5: Adding Barcode Reading (New Capability)

If you were using ZXing.Net for reading alongside NetBarcode for writing, both can be replaced.

**Before (ZXing.Net for reading):**

```csharp
using ZXing;
using ZXing.Common;

var reader = new BarcodeReaderGeneric();
reader.Options = new DecodingOptions { TryHarder = true };
// ... bitmap loading, decoding, result handling
```

**After:**

```csharp
using IronBarCode;

// Read from an image file
var result = BarcodeReader.Read("label.png").FirstOrDefault();
Console.WriteLine($"{result?.Value} ({result?.BarcodeType})");

// Read multiple barcodes from a PDF
var results = BarcodeReader.Read("invoice.pdf");
foreach (var r in results)
{
    Console.WriteLine($"Page {r.PageNumber}: {r.Value}");
}
```

Full documentation for reading options — including speed tuning, multi-barcode detection, and image correction — is in the [read barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) guide.

### Example 6: Complete Service Class Migration

**Before:**

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

    public void GenerateShippingLabel(string trackingCode)
    {
        var barcode = new Barcode(trackingCode, Type.Code128);
        barcode.SaveImageFile($"shipping-{trackingCode}.png");
    }

    public byte[] GetBarcodeBytes(string data)
    {
        var barcode = new Barcode(data, Type.Code128);
        Image<Rgba32> image = barcode.GetImage();
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }

    // Cannot generate QR codes — Type.QRCode does not exist
    // Cannot read barcodes — no reading API
}
```

**After:**

```csharp
using IronBarCode;

public class BarcodeService
{
    public void GenerateProductBarcode(string upc)
    {
        BarcodeWriter.CreateBarcode(upc, BarcodeEncoding.UPCA)
            .SaveAsPng($"product-{upc}.png");
    }

    public void GenerateShippingLabel(string trackingCode)
    {
        BarcodeWriter.CreateBarcode(trackingCode, BarcodeEncoding.Code128)
            .SaveAsPng($"shipping-{trackingCode}.png");
    }

    public byte[] GetBarcodeBytes(string data)
    {
        return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
            .ToPngBinaryData();
    }

    // Now available — no second library needed
    public void GenerateQRCode(string url)
    {
        BarcodeWriter.CreateBarcode(url, BarcodeEncoding.QRCode)
            .SaveAsPng($"qr-{Guid.NewGuid()}.png");
    }

    // Now available — no ZXing.Net needed
    public string? ReadBarcode(string imagePath)
    {
        return BarcodeReader.Read(imagePath).FirstOrDefault()?.Value;
    }
}
```

---

## API Mapping Reference

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
| `barcode.SaveImageFile("x.png")` | `.SaveAsPng("x.png")` | Method rename |
| `barcode.SaveImageFile("x.jpg")` | `.SaveAsJpeg("x.jpg")` | Method rename |
| `barcode.GetImage()` → `Image<Rgba32>` | `.ToPngBinaryData()` or `.SaveAsPng()` | No ImageSharp type exposed |
| No `Type.QRCode` entry | `BarcodeEncoding.QRCode` | New capability |
| No `Type.DataMatrix` entry | `BarcodeEncoding.DataMatrix` | New capability |
| No `Type.PDF417` entry | `BarcodeEncoding.PDF417` | New capability |
| No `Type.Aztec` entry | `BarcodeEncoding.Aztec` | New capability |
| No reading API | `BarcodeReader.Read(path)` | New capability |
| `using NetBarcode;` | `using IronBarCode;` | Namespace |
| `using SixLabors.ImageSharp;` | Remove | No longer needed |

The complete list of available encoding constants is in the [supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) reference.

---

## Migration Checklist

Use this list to locate every NetBarcode call site before starting. Run each grep against your source directory.

**Find all NetBarcode usage:**

```bash
grep -r "using NetBarcode" ./src
grep -r "new Barcode(" ./src
grep -r "Type\.Code128" ./src
grep -r "Type\.EAN13" ./src
grep -r "Type\.UPCA" ./src
grep -r "Type\.Code39" ./src
grep -r "Type\.EAN8" ./src
grep -r "barcode\.SaveImageFile(" ./src
grep -r "barcode\.GetImage(" ./src
grep -r "using SixLabors\.ImageSharp" ./src
```

**Before migration:**
- [ ] Run all grep terms above and note every file that needs updating
- [ ] Identify which barcode formats are in use (not all `Type.*` values are the same migration effort)
- [ ] Check whether `SixLabors.ImageSharp` is imported directly in any file, not just pulled in transitively by NetBarcode
- [ ] Check whether QRCoder or ZXing.Net are present and can also be removed
- [ ] Obtain an IronBarcode license key (trial available at no cost)

**During migration:**
- [ ] `dotnet remove package NetBarcode`
- [ ] `dotnet remove package SixLabors.ImageSharp` (if safe to remove)
- [ ] `dotnet add package IronBarcode`
- [ ] Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` at application startup
- [ ] Replace `using NetBarcode;` with `using IronBarCode;` in each file
- [ ] Remove `using SixLabors.ImageSharp;`, `using SixLabors.ImageSharp.PixelFormats;`, `using SixLabors.Fonts;`
- [ ] Translate each `new Barcode(data, Type.X)` call using the API mapping table above
- [ ] Replace `SaveImageFile()` with `SaveAsPng()` or the appropriate format method
- [ ] Replace `GetImage()` calls with `ToPngBinaryData()`, `SaveAsPng()`, or the appropriate output method
- [ ] If removing QRCoder: translate `QRCodeGenerator` + `PngByteQRCode` chains to `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)`
- [ ] If removing ZXing.Net: translate reader setup to `BarcodeReader.Read(path)`

**After migration:**
- [ ] Build and confirm no compile errors
- [ ] Run existing tests against generated barcode images
- [ ] Verify QR code output if that format was added during migration
- [ ] Test reading functionality if barcode scanning was added
- [ ] Remove any remaining `SixLabors.*` package references that are now unused

---

The migration removes the constraint that started this process — the `Type` enum with no QR Code entry. After the switch, adding DataMatrix for a pharmaceutical client, reading barcodes from incoming invoices, or generating Aztec codes for event tickets are all the same API call as Code128 was before. The library grows with the project instead of stopping at its original scope.
