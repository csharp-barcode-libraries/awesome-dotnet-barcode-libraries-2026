# Migrating from BarcodeLib to IronBarcode

This guide provides a complete migration path from BarcodeLib to IronBarcode for .NET developers. It covers the reasons teams make this transition, step-by-step package replacement, code migration examples for every common BarcodeLib pattern, a full API mapping reference, and a structured checklist for managing the migration across a codebase.

## Why Migrate from BarcodeLib

Teams migrating from BarcodeLib to IronBarcode report these triggers:

**No Reading API:** BarcodeLib has never included a reading or decoding capability. When a project that was generating barcode images receives a new requirement to also scan barcodes — from uploaded images, warehouse scanners, or supplier documents — BarcodeLib cannot fulfill it. The only option is adding a second library such as ZXing.Net, which introduces a second dependency graph and second API surface to maintain alongside BarcodeLib.

**SkiaSharp Version Conflict:** BarcodeLib 3.x introduced SkiaSharp as a graphics backend to replace `System.Drawing.Common`. Version 3.1.5 pins `SkiaSharp >= 2.88.8`. In MAUI projects, Blazor projects, and any project where another dependency also pulls in SkiaSharp 3.x, the resolved version can fall outside BarcodeLib's expected range. This produces NU1608 warnings during restore and, in the worst cases, runtime assembly binding failures on device.

**No PDF Support:** Applications that generate PDF documents with embedded barcodes — invoices, work orders, shipping manifests — sometimes need to read those barcodes back during downstream processing. BarcodeLib generates barcode images but has no PDF support on either end. Extracting barcodes from a PDF with BarcodeLib requires rendering the PDF pages to images with a separate PDF library and then passing those images to a separate reading library.

**Stream Encoding Required for Byte Array Output:** BarcodeLib 3.x returns a `SkiaSharp.SKImage` (or `System.Drawing.Image` on 2.x), which requires encoding to a `MemoryStream` via `SKImage.Encode(...).SaveTo(stream)` (or `Image.Save(ms, ImageFormat.Png)`) before you can produce the `byte[]` output that HTTP responses, database BLOB columns, and most downstream consumers actually need. IronBarcode provides `.ToPngBinaryData()` directly on the generation chain.

### The Fundamental Problem

BarcodeLib's generation-only architecture means that adding any scan capability forces a second library into the stack:

```csharp
// BarcodeLib 3.x: generation only — reading requires a completely separate library
using BarcodeStandard;
using SkiaSharp;

var b = new Barcode();
SKImage img = b.Encode(BarcodeStandard.Type.Code128, "PRODUCT-12345", 300, 100);
using (var stream = File.OpenWrite("barcode.png"))
{
    img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
}

// To read it back, you have no option but to add ZXing.Net or another scanner:
// using ZXing;
// using ZXing.SkiaSharp;
// var reader = new BarcodeReader();  // separate library, separate API, separate dependency
// var result = reader.Decode(bitmap);
```

IronBarcode handles both in the same package with the same `using` statement:

```csharp
// IronBarcode: generation and reading — no second library needed
using IronBarCode;

// Generate
BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .SaveAsPng("barcode.png");

// Read back — same package, same namespace
var result = BarcodeReader.Read("barcode.png").First().Value;  // .Value is the decoded string
Console.WriteLine(result);  // "PRODUCT-12345"
```

## IronBarcode vs BarcodeLib: Feature Comparison

| Feature | BarcodeLib | IronBarcode |
|---|---|---|
| Barcode generation (1D) | Yes | Yes |
| Barcode generation (2D / QR / Data Matrix) | No | Yes |
| Barcode reading / scanning | No | Yes (`BarcodeReader.Read()`) |
| QR code generation | No | Yes (advanced, with logo embedding) |
| PDF barcode reading | No | Yes (native, no extra library) |
| PDF barcode generation output | No | Yes |
| SkiaSharp dependency | Yes (version conflict risk) | No |
| MAUI project compatibility | Conflict risk (NU1608) | No conflict |
| Fluent chainable API | No | Yes |
| `byte[]` output directly | Manual (via `MemoryStream`) | `.ToPngBinaryData()` |
| Multi-barcode detection | No | Yes (`ExpectMultipleBarcodes`) |
| Reading speed tuning | N/A | Yes (`ReadingSpeed` enum) |
| Linux / macOS support | Partial (SkiaSharp-dependent) | Full |
| Docker / container support | Configuration required | Yes |
| Active maintenance | Yes (community) | Yes (commercial) |
| Commercial support / SLA | No | Yes |
| License | Apache 2.0 (free) | $799–$4,799 perpetual |

## Quick Start: BarcodeLib to IronBarcode Migration

The migration can begin immediately with these foundational steps.

### Step 1: Replace NuGet Package

Remove BarcodeLib first:

```bash
dotnet remove package BarcodeLib
```

Note that the canonical package on nuget.org is `BarcodeLib` (by Brad Barnhill). Some legacy projects also reference now-archived community forks under different IDs — handle those the same way. Check the `.csproj` file to be sure:

```bash
grep -n "BarcodeLib\|BarcodeStandard" YourProject.csproj
```

Remove all BarcodeLib-related `<PackageReference>` entries. If you added explicit `<PackageReference Include="SkiaSharp">` overrides to work around NU1608 warnings from BarcodeLib, remove those too — after installing IronBarcode, evaluate whether SkiaSharp is still needed for other reasons. Then install IronBarcode:

```bash
dotnet add package IronBarcode
```

### Step 2: Update Namespaces

Replace the BarcodeLib `using` directives in each file that referenced them. Note that BarcodeLib 2.x used `using BarcodeLib;` and 3.x switched to `using BarcodeStandard;` (alongside `using SkiaSharp;` for the `SKImage` return type):

```csharp
// Before (BarcodeLib 2.x)
using BarcodeLib;
using System.Drawing.Imaging;  // often paired with BarcodeLib for ImageFormat

// Before (BarcodeLib 3.x)
using BarcodeStandard;
using SkiaSharp;

// After
using IronBarCode;
```

### Step 3: Initialize License

Add license initialization at application startup:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

For ASP.NET Core applications, put this in `Program.cs` before `builder.Build()`. For console applications, put it at the top of `Main()`. For class libraries called from other apps, initialize it wherever the host application starts.

## Code Migration Examples

### Basic Code128 Generation

The most common BarcodeLib pattern: create an instance, set properties, call `Encode()`.

**BarcodeLib Approach (3.x):**

```csharp
using BarcodeStandard;
using SkiaSharp;
using System.IO;

public void GenerateShippingLabel(string trackingNumber, string outputPath)
{
    var b = new Barcode();
    b.IncludeLabel = true;
    SKImage img = b.Encode(BarcodeStandard.Type.Code128, trackingNumber, 400, 120);
    using var stream = File.OpenWrite(outputPath);
    img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
}
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public void GenerateShippingLabel(string trackingNumber, string outputPath)
{
    BarcodeWriter.CreateBarcode(trackingNumber, BarcodeEncoding.Code128)
        .ResizeTo(400, 120)
        .AddAnnotationTextBelowBarcode(trackingNumber)
        .SaveAsPng(outputPath);
}
```

The instance-plus-`Encode` pattern collapses into a fluent chain. `.AddAnnotationTextBelowBarcode()` replaces `b.IncludeLabel = true` — it accepts the label string explicitly so you control what text appears below the bars. `.ResizeTo()` replaces the explicit width/height arguments to `Encode()`. The `SKImage` round-trip through a stream disappears entirely. For advanced generation options, see the [IronBarcode barcode generation documentation](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/).

### Returning byte[] — the Common Web API Pattern

BarcodeLib 3.x returns an `SKImage`. Getting bytes out of it requires encoding to a `MemoryStream` via `SKImage.Encode(...).SaveTo(ms)`. IronBarcode provides `.ToPngBinaryData()` directly.

**BarcodeLib Approach (3.x):**

```csharp
using BarcodeStandard;
using SkiaSharp;
using System.IO;

public byte[] GetBarcodeBytes(string data)
{
    var b = new Barcode();
    SKImage img = b.Encode(BarcodeStandard.Type.Code128, data, 300, 100);

    using var ms = new MemoryStream();
    img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
    return ms.ToArray();
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

public byte[] GetBarcodeBytes(string data)
{
    return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ResizeTo(300, 100)
        .ToPngBinaryData();
}
```

The `MemoryStream` intermediate step is gone. `.ToPngBinaryData()` returns the `byte[]` directly, which is what HTTP response bodies, database BLOB columns, and file writers actually want.

### Web API Controller Action

**BarcodeLib Approach (3.x):**

```csharp
using BarcodeStandard;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System.IO;

[ApiController]
[Route("api/labels")]
public class LabelsController : ControllerBase
{
    [HttpGet("{sku}")]
    public IActionResult GetLabel(string sku)
    {
        var b = new Barcode();
        SKImage img = b.Encode(BarcodeStandard.Type.Code128, sku, 400, 120);

        using var ms = new MemoryStream();
        img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
        return File(ms.ToArray(), "image/png");
    }
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/labels")]
public class LabelsController : ControllerBase
{
    [HttpGet("{sku}")]
    public IActionResult GetLabel(string sku)
    {
        byte[] pngBytes = BarcodeWriter.CreateBarcode(sku, BarcodeEncoding.Code128)
            .ResizeTo(400, 120)
            .ToPngBinaryData();

        return File(pngBytes, "image/png");
    }
}
```

The controller action shrinks by removing the `MemoryStream` block and the `System.Drawing.Imaging` import. The byte array flows directly from `.ToPngBinaryData()` into `File()`.

### QR Code Generation (Net-New Capability)

BarcodeLib does **not** generate QR codes — its `Type` enum is 1D-only and contains no QR Code, Data Matrix, or other 2D entry. Projects that need QR generation alongside their existing BarcodeLib usage typically had to add a second library (such as QRCoder). IronBarcode covers both with `QRCodeWriter`.

**BarcodeLib Approach:**

```csharp
// BarcodeLib has no QR Code support.
// Projects historically added a second library (e.g. QRCoder) for QR codes
// alongside BarcodeLib's 1D generation. This is a separate dependency
// graph and a separate API to maintain.
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Basic QR code
QRCodeWriter.CreateQrCode("https://example.com/product/42", 300)
    .SaveAsPng("qr.png");

// QR code with embedded brand logo (not possible with BarcodeLib at all)
QRCodeWriter.CreateQrCode("https://example.com/product/42", 300)
    .AddBrandLogo("logo.png")
    .SaveAsPng("qr-branded.png");
```

`QRCodeWriter.CreateQrCode()` takes the data string and pixel size as parameters. Logo embedding, color customization, and error correction level are all available through chained methods. For QR code styling options, see the [IronBarcode QR code documentation](https://ironsoftware.com/csharp/barcode/how-to/create-qr-code/).

### EAN-13 and UPC-A

These are common in retail inventory systems. The enum names change but the values are direct equivalents.

**BarcodeLib Approach (3.x):**

```csharp
using BarcodeStandard;
using SkiaSharp;
using System.IO;

// EAN-13 product barcode
var b = new Barcode();
SKImage ean = b.Encode(BarcodeStandard.Type.Ean13, "5901234123457", 250, 100);
using (var s = File.OpenWrite("product-ean.png"))
    ean.Encode(SKEncodedImageFormat.Png, 100).SaveTo(s);

// UPC-A for US retail
SKImage upc = b.Encode(BarcodeStandard.Type.UpcA, "012345678905", 250, 100);
using (var s = File.OpenWrite("product-upc.png"))
    upc.Encode(SKEncodedImageFormat.Png, 100).SaveTo(s);
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// EAN-13 product barcode
BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
    .ResizeTo(250, 100)
    .SaveAsPng("product-ean.png");

// UPC-A for US retail
BarcodeWriter.CreateBarcode("012345678905", BarcodeEncoding.UPCA)
    .ResizeTo(250, 100)
    .SaveAsPng("product-upc.png");
```

### Adding Barcode Reading (Net-New Capability)

BarcodeLib has no reading API. If your migration is driven by a new requirement to scan barcodes — from uploaded images, warehouse scanners, or scanned PDF documents — add this without a second library:

**BarcodeLib Approach:**

```csharp
// BarcodeLib — no reading API exists
// Adding reading requires a separate library such as ZXing.Net:
// dotnet add package ZXing.Net
// using ZXing;
// var reader = new BarcodeReader();
// // ... separate API, separate dependency graph to manage
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Read a barcode from an image file
var results = BarcodeReader.Read("incoming-label.png");
foreach (var result in results)
{
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Format: {result.BarcodeType}");
}

// Read all barcodes from a multi-page PDF — no PDF library required
var pdfResults = BarcodeReader.Read("supplier-invoice.pdf");
foreach (var result in pdfResults)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
}

// Configure for high-volume scanning with multiple barcodes per image
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};
var warehouseResults = BarcodeReader.Read("dock-scan.png", options);
```

`BarcodeReader.Read()` accepts image files, PDF files, streams, and `System.Drawing.Bitmap` objects. The `ReadingSpeed` enum lets you trade thoroughness for performance when scanning at volume. For reading configuration options, see the [IronBarcode reading documentation](https://ironsoftware.com/csharp/barcode/how-to/read-barcode-from-image/).

### Resolving the SkiaSharp Conflict

If your migration is driven by NU1608 warnings, verify the conflict is resolved after switching packages. After running `dotnet remove package BarcodeLib` and `dotnet add package IronBarcode`, rebuild and check the output:

```bash
dotnet build 2>&1 | grep -i "NU1608\|SkiaSharp"
```

If no output appears, the conflict is resolved. If SkiaSharp warnings remain, they are coming from a different package in your dependency graph — IronBarcode is not the source.

**BarcodeLib Approach:**

```xml
<!-- Before — explicit overrides needed to pacify BarcodeLib -->
<ItemGroup>
  <PackageReference Include="BarcodeLib" Version="3.1.5" />
  <!-- BarcodeLib pulls in SkiaSharp >= 2.88.8 transitively -->
  <!-- Override often added because MAUI needs a 3.x SkiaSharp -->
  <PackageReference Include="SkiaSharp" Version="3.116.1" />
</ItemGroup>
```

**IronBarcode Approach:**

```xml
<!-- After — clean, no conflict -->
<ItemGroup>
  <PackageReference Include="IronBarcode" Version="*" />
  <!-- SkiaSharp version is now only determined by MAUI's requirements -->
  <PackageReference Include="SkiaSharp.Views.Maui.Controls" Version="3.116.1" />
</ItemGroup>
```

## BarcodeLib API to IronBarcode Mapping Reference

| BarcodeLib | IronBarcode |
|---|---|
| `new Barcode()` | Static API — no instance required |
| `b.Encode(BarcodeStandard.Type.Code128, "data", w, h)` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128).ResizeTo(w, h)` |
| `b.IncludeLabel = true` | `.AddAnnotationTextBelowBarcode("text")` |
| `width` / `height` args to `Encode()` | `.ResizeTo(width, height)` |
| Returns `SKImage` (3.x) / `System.Drawing.Image` (2.x) | `.SaveAsPng(path)` / `.ToPngBinaryData()` |
| `BarcodeStandard.Type.Code128` | `BarcodeEncoding.Code128` |
| `BarcodeStandard.Type.Code39` | `BarcodeEncoding.Code39` |
| `BarcodeStandard.Type.Ean13` | `BarcodeEncoding.EAN13` |
| `BarcodeStandard.Type.UpcA` | `BarcodeEncoding.UPCA` |
| (no QR Code in BarcodeLib) | `BarcodeEncoding.QRCode` / `QRCodeWriter.CreateQrCode()` |
| `BarcodeStandard.Type.Itf14` | `BarcodeEncoding.ITF14` |
| `BarcodeStandard.Type.Codabar` | `BarcodeEncoding.Codabar` |
| No reading API | `BarcodeReader.Read(path)` |
| SkiaSharp version conflict in MAUI | No conflicting dependencies |
| `img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream)` | `.SaveAsPng(path)` |
| `MemoryStream` + `SKImage.Encode().SaveTo(ms)` | `.ToPngBinaryData()` |

## Common Migration Issues and Solutions

### Issue 1: Type Enum Naming and Namespace Change

**BarcodeLib:** 2.x exposed `using BarcodeLib;` with values like `TYPE.CODE128` (uppercase). 3.x switched to `using BarcodeStandard;` with PascalCase values on the `Type` enum: `BarcodeStandard.Type.Code128`, `Type.Ean13`, `Type.UpcA`, `Type.QR_Code` does **not** exist (BarcodeLib has no QR support).

**Solution:** Replace with `BarcodeEncoding.Code128` (PascalCase, standard `enum` type). A grep across `.cs` files identifies all occurrences, and a systematic find-and-replace covers the common formats:

```bash
grep -rn "TYPE\.\|BarcodeStandard\.Type\." --include="*.cs" .
```

Common replacements (covering both 2.x and 3.x source forms): `TYPE.CODE128` / `Type.Code128` → `BarcodeEncoding.Code128`; `TYPE.EAN13` / `Type.Ean13` → `BarcodeEncoding.EAN13`; `TYPE.UPCA` / `Type.UpcA` → `BarcodeEncoding.UPCA`; `TYPE.ITF14` / `Type.Itf14` → `BarcodeEncoding.ITF14`; `TYPE.CODABAR` / `Type.Codabar` → `BarcodeEncoding.Codabar`. There is no QR equivalent to map from BarcodeLib — adopt `BarcodeEncoding.QRCode` directly.

### Issue 2: SKImage / System.Drawing.Image Return Type

**BarcodeLib:** `b.Encode()` returns `SKImage` on 3.x and `System.Drawing.Image` on 2.x. Code typed to either will not compile against IronBarcode.

**Solution:** Remove the intermediate image variable and replace the save logic with the appropriate terminal method on the fluent chain:

```csharp
// Before (3.x)
SKImage img = b.Encode(BarcodeStandard.Type.Code128, data, 300, 100);
using var ms = new MemoryStream();
img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
return ms.ToArray();

// After
return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .ToPngBinaryData();
```

Search for `SKImage img = b.Encode` and `Image img = b.Encode` patterns to find all affected locations.

### Issue 3: Width and Height Arguments to Encode()

**BarcodeLib:** 3.x passes width and height as the third and fourth arguments to `b.Encode(...)`. 2.x exposed `b.Width` and `b.Height` as separate property assignments on the instance.

**Solution:** Whether your code uses the property-setter form or the constructor-arg form, both collapse into a single `.ResizeTo(width, height)` call chained after `CreateBarcode()`. Find both shapes before replacing:

```bash
# 2.x property assignments
grep -n "\.Width = \|\.Height = " --include="*.cs" -r .
# 3.x positional arguments to Encode
grep -n "\.Encode(" --include="*.cs" -r .
```

Then replace each occurrence with the single `.ResizeTo(width, height)` chain call.

### Issue 4: IncludeLabel Boolean Toggle

**BarcodeLib:** `b.IncludeLabel = true` is a boolean that automatically renders the encoded data string as the visible text below the bars.

**Solution:** Use `.AddAnnotationTextBelowBarcode("text")`, which takes the label string explicitly. In most cases, pass the same data string that was encoded into the barcode. If the original code used `IncludeLabel = true` and relied on BarcodeLib to auto-render the data as the label, pass that same data string explicitly:

```csharp
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .AddAnnotationTextBelowBarcode(data)  // pass the same string
    .SaveAsPng(outputPath);
```

### Issue 5: SkiaSharp References Left After BarcodeLib Removal

**BarcodeLib:** Projects frequently accumulated explicit `<PackageReference Include="SkiaSharp">` entries specifically to override BarcodeLib's version constraint. These become orphaned after BarcodeLib is removed.

**Solution:** After switching to IronBarcode, verify whether SkiaSharp is still needed by any other package before removing the explicit reference:

```bash
# Check if SkiaSharp is still referenced by anything other than the explicit override
dotnet list package --include-transitive 2>&1 | grep -i skia
```

If SkiaSharp only appears due to the now-removed explicit `<PackageReference>`, remove that entry. If it is still pulled in by MAUI or another dependency, leave it — IronBarcode will not conflict with it.

## BarcodeLib Migration Checklist

### Pre-Migration Tasks

Run these searches before starting to understand the scope of changes needed:

```bash
# Find all BarcodeLib using directives (covers 2.x and 3.x)
grep -rn "using BarcodeLib\|using BarcodeStandard" --include="*.cs" .

# Find Barcode object instantiation
grep -rn "new Barcode()" --include="*.cs" .

# Find Encode calls
grep -rn "\.Encode(" --include="*.cs" .

# Find Type/TYPE enum usage (both 2.x and 3.x casing)
grep -rn "TYPE\.\|BarcodeStandard\.Type\." --include="*.cs" .

# Find IncludeLabel usage
grep -rn "IncludeLabel" --include="*.cs" .

# Find the package references in project files
grep -rn "BarcodeLib\|BarcodeStandard" --include="*.csproj" .

# Find NU1608 evidence in lock files
grep -rn "NU1608" .
```

Document all files affected by each search. Note which projects reference BarcodeLib directly and which inherit it transitively. Identify any explicit SkiaSharp version overrides added only to resolve BarcodeLib conflicts.

### Code Update Tasks

1. Run `dotnet remove package BarcodeLib` for each project
2. Remove any explicit SkiaSharp version override references added only to fix BarcodeLib conflicts
3. Run `dotnet add package IronBarcode` for each project
4. Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` to application startup in each project
5. Replace `using BarcodeLib;` (2.x) or `using BarcodeStandard;` (3.x) with `using IronBarCode;` across all `.cs` files
6. Remove `using SkiaSharp;` and `using System.Drawing.Imaging;` imports that existed only for the BarcodeLib output pipeline
7. Replace `new Barcode()` + property setters / `Encode(type, data, w, h)` with `BarcodeWriter.CreateBarcode()` fluent calls
8. Replace `TYPE.CODE128` / `Type.Code128` → `BarcodeEncoding.Code128` and all other enum values
9. Replace `b.Width = N; b.Height = M;` pairs (2.x) and the width/height arguments to `Encode()` (3.x) with `.ResizeTo(N, M)` chain calls
10. Replace `b.IncludeLabel = true;` with `.AddAnnotationTextBelowBarcode(data)`
11. Replace `img.Save(path, ImageFormat.Png)` (2.x) and `img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream)` (3.x) with `.SaveAsPng(path)` or `.ToPngBinaryData()`
12. Remove intermediate `SKImage` / `Image` variables and `MemoryStream` blocks where `.ToPngBinaryData()` replaces them
13. Add barcode reading code where needed (`BarcodeReader.Read()`)
14. Add QR Code generation if previously deferred to a second library (`BarcodeEncoding.QRCode` or `QRCodeWriter`)

### Post-Migration Testing

- Build the project and confirm zero NU1608 warnings remain in the restore output
- Run `dotnet build 2>&1 | grep -i "NU1608\|SkiaSharp"` to verify the SkiaSharp conflict is fully resolved
- Compare visual output of generated barcodes against known-good samples from BarcodeLib
- Verify that QR codes decode correctly using a mobile scanner or the `BarcodeReader.Read()` method
- Test EAN-13 and UPC-A barcodes against retail scanner hardware if applicable
- Verify images display correctly in all output targets: file system, HTTP response, database storage
- Test any PDF barcode reading scenarios using `BarcodeReader.Read("file.pdf")` on real documents
- Confirm cross-platform builds succeed if the project targets Linux or macOS
- Verify that MAUI builds complete without SkiaSharp binding errors on Android and iOS targets

## Key Benefits of Migrating to IronBarcode

**Barcode Reading Without a Second Library:** The most immediate gain for teams that needed reading capability is eliminating the second-library dependency. `BarcodeReader.Read()` is in the same package, uses the same `using IronBarCode;` statement, and requires no additional NuGet installs. ZXing.Net and its own dependency graph are no longer part of the project.

**No SkiaSharp Version Conflict:** IronBarcode does not share the SkiaSharp dependency graph with application code. MAUI projects, Blazor projects, and any project where multiple packages converge on SkiaSharp can install IronBarcode without NU1608 warnings or runtime binding failures. The version negotiation that BarcodeLib introduced is gone.

**Direct Byte Array Output:** `.ToPngBinaryData()` returns `byte[]` at the end of the fluent chain. The `MemoryStream` intermediary that BarcodeLib required for byte array output is eliminated from every controller action, service method, and API handler that generates barcodes.

**PDF Barcode Processing:** `BarcodeReader.Read()` accepts PDF files natively. Applications that generate PDF documents with embedded barcodes can read those barcodes back without a separate PDF rendering library. The full chain — generation, PDF embedding, and reading — is handled within IronBarcode.

**Commercial Support and SLA:** IronBarcode is backed by Iron Software's commercial support model with a guaranteed update cadence. When .NET 10 releases in 2026 or breaking changes appear in the .NET ecosystem, IronBarcode publishes compatibility updates on a timeline tied to the commercial SLA rather than community availability.

**QR Code Generation Without a Second Library:** BarcodeLib does not generate QR Code, Data Matrix, PDF417, or any other 2D symbology. `QRCodeWriter` brings QR generation into the same package alongside 1D generation and reading, with logo embedding, color customization, and error correction level configuration through chained methods. Teams that previously paired BarcodeLib with QRCoder (or similar) can drop the second library entirely.
