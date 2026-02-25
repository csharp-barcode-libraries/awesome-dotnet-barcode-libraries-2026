# Migrating from BarcodeLib to IronBarcode

The migration from BarcodeLib to IronBarcode resolves two problems in the same step: it removes the SkiaSharp version conflict, and it adds barcode reading capability that BarcodeLib doesn't have. The generation API is similar enough that most changes are surface-level — the `Encode()` call becomes `CreateBarcode()`, type enums change namespace, and output switches from a `System.Drawing.Image` to a byte array or a direct file write. If the SkiaSharp conflict was your primary motivation, the migration is essentially a package swap with minor API updates. If you also need reading capability, you get that immediately without pulling in a second library.

## Step 1: Remove BarcodeLib

The package name varies depending on which variant your project uses:

```bash
# Remove the standard package
dotnet remove package BarcodeLib

# Or the signed variant
dotnet remove package BarcodeLib.Signed

# Or the SkiaSharp variant (most likely to have caused NU1608)
dotnet remove package BarcodeLib.SkiaSharp
```

If you're not sure which variant is installed, check the `.csproj` file:

```bash
grep -n "BarcodeLib" YourProject.csproj
```

Remove all BarcodeLib-related `<PackageReference>` entries. If you added explicit `<PackageReference Include="SkiaSharp">` overrides to work around NU1608 warnings from BarcodeLib, remove those too — after installing IronBarcode, you can evaluate whether SkiaSharp is still needed for other reasons.

## Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

## Step 3: Update Namespaces and License

Replace the BarcodeLib `using` directive in each file that referenced it:

```csharp
// Before
using BarcodeLib;
using System.Drawing.Imaging;  // often paired with BarcodeLib for ImageFormat

// After
using IronBarCode;
```

Add license initialization at application startup:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

For ASP.NET Core applications, put this in `Program.cs` before `builder.Build()`. For console applications, put it at the top of `Main()`. For class libraries called from other apps, initialize it wherever the host application starts.

## Code Migration Examples

### Basic Code128 Generation

The most common BarcodeLib pattern: create an instance, set properties, call `Encode()`.

**Before:**

```csharp
using BarcodeLib;
using System.Drawing;
using System.Drawing.Imaging;

public void GenerateShippingLabel(string trackingNumber, string outputPath)
{
    var b = new Barcode();
    b.IncludeLabel = true;
    b.Width = 400;
    b.Height = 120;
    Image img = b.Encode(TYPE.CODE128, trackingNumber);
    img.Save(outputPath, ImageFormat.Png);
}
```

**After:**

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

The property-setter block collapses into a fluent chain. `.AddAnnotationTextBelowBarcode()` replaces `b.IncludeLabel = true` — it lets you control what text appears below the bars, whether that's the raw data string or a formatted label. `.ResizeTo()` replaces the `Width` and `Height` property assignments.

### Returning `byte[]` — the Common Web API Pattern

BarcodeLib returns a `System.Drawing.Image`. Getting bytes out of it requires saving to a `MemoryStream`. IronBarcode provides `.ToPngBinaryData()` directly.

**Before:**

```csharp
using BarcodeLib;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public byte[] GetBarcodeBytes(string data)
{
    var b = new Barcode();
    b.Width = 300;
    b.Height = 100;
    Image img = b.Encode(TYPE.CODE128, data);

    using var ms = new MemoryStream();
    img.Save(ms, ImageFormat.Png);
    return ms.ToArray();
}
```

**After:**

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

**Before (with BarcodeLib):**

```csharp
using BarcodeLib;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.IO;

[ApiController]
[Route("api/labels")]
public class LabelsController : ControllerBase
{
    [HttpGet("{sku}")]
    public IActionResult GetLabel(string sku)
    {
        var b = new Barcode();
        b.Width = 400;
        b.Height = 120;
        var img = b.Encode(TYPE.CODE128, sku);

        using var ms = new MemoryStream();
        img.Save(ms, ImageFormat.Png);
        return File(ms.ToArray(), "image/png");
    }
}
```

**After:**

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

### QR Code Generation

BarcodeLib supports QR codes through `TYPE.QR_Code`. IronBarcode uses a dedicated `QRCodeWriter` class with more options.

**Before:**

```csharp
using BarcodeLib;
using System.Drawing;
using System.Drawing.Imaging;

var b = new Barcode();
b.Width = 300;
b.Height = 300;
Image qr = b.Encode(TYPE.QR_Code, "https://example.com/product/42");
qr.Save("qr.png", ImageFormat.Png);
```

**After:**

```csharp
using IronBarCode;

// Basic QR code
QRCodeWriter.CreateQrCode("https://example.com/product/42", 300)
    .SaveAsPng("qr.png");

// QR code with embedded brand logo (not possible with BarcodeLib)
QRCodeWriter.CreateQrCode("https://example.com/product/42", 300)
    .AddBrandLogo("logo.png")
    .SaveAsPng("qr-branded.png");
```

`QRCodeWriter.CreateQrCode()` takes the data string and pixel size as parameters. The second parameter is the size of the square image in pixels. Logo embedding, color customization, and error correction level are all available through chained methods.

### Adding Barcode Reading (Net-New Capability)

BarcodeLib has no reading API. If your migration is being driven by a new requirement to scan barcodes — from uploaded images, warehouse scanners, or scanned PDF documents — add this without a second library:

```csharp
using IronBarCode;

// Read a barcode from an image file
var results = BarcodeReader.Read("incoming-label.png");
foreach (var result in results)
{
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Format: {result.Format}");
}

// Read the first result directly
string labelData = BarcodeReader.Read("shipping-label.png").First().Value;
Console.WriteLine(labelData);

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

`BarcodeReader.Read()` accepts image files, PDF files, streams, and `System.Drawing.Bitmap` objects. The `ReadingSpeed` enum lets you trade thoroughness for performance when scanning at volume.

### EAN-13 and UPC-A

These are common in retail inventory systems. The enum names change but the values are direct equivalents.

**Before:**

```csharp
using BarcodeLib;
using System.Drawing;
using System.Drawing.Imaging;

// EAN-13 product barcode
var b = new Barcode();
b.Width = 250;
b.Height = 100;
Image ean = b.Encode(TYPE.EAN13, "5901234123457");
ean.Save("product-ean.png", ImageFormat.Png);

// UPC-A for US retail
Image upc = b.Encode(TYPE.UPCA, "012345678905");
upc.Save("product-upc.png", ImageFormat.Png);
```

**After:**

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

### Resolving the SkiaSharp Conflict

If your migration is driven by NU1608 warnings, verify the conflict is resolved after switching packages. After running `dotnet remove package BarcodeLib.SkiaSharp` and `dotnet add package IronBarcode`, rebuild and check the output:

```bash
dotnet build 2>&1 | grep -i "NU1608\|SkiaSharp"
```

If no output appears, the conflict is resolved. If SkiaSharp warnings remain, they're coming from a different package in your dependency graph — IronBarcode is not the source.

For a MAUI project that had explicit SkiaSharp version overrides to work around BarcodeLib:

```xml
<!-- Before — explicit overrides needed to pacify BarcodeLib -->
<ItemGroup>
  <PackageReference Include="BarcodeLib.SkiaSharp" Version="3.1.5" />
  <PackageReference Include="SkiaSharp" Version="2.88.7" />
  <!-- Override required because MAUI needs a different version -->
  <PackageReference Include="SkiaSharp" Version="3.116.1" />
</ItemGroup>

<!-- After — clean, no conflict -->
<ItemGroup>
  <PackageReference Include="IronBarcode" Version="*" />
  <!-- SkiaSharp version is now only determined by MAUI's requirements -->
  <PackageReference Include="SkiaSharp.Views.Maui.Controls" Version="3.116.1" />
</ItemGroup>
```

## Common Migration Issues

**`TYPE.CODE128` vs `BarcodeEncoding.Code128` — enum namespace change.** BarcodeLib uses `TYPE.CODE128` (uppercase, `TYPE` class). IronBarcode uses `BarcodeEncoding.Code128` (PascalCase, `BarcodeEncoding` enum). The names are consistent between the two but the casing and container type differ. A find-and-replace works for the common ones:

| BarcodeLib | IronBarcode |
|---|---|
| `TYPE.CODE128` | `BarcodeEncoding.Code128` |
| `TYPE.CODE39` | `BarcodeEncoding.Code39` |
| `TYPE.EAN13` | `BarcodeEncoding.EAN13` |
| `TYPE.UPCA` | `BarcodeEncoding.UPCA` |
| `TYPE.QR_Code` | `BarcodeEncoding.QRCode` |
| `TYPE.ITF14` | `BarcodeEncoding.ITF14` |
| `TYPE.CODABAR` | `BarcodeEncoding.Codabar` |

**`b.Encode()` returns `System.Drawing.Image`, IronBarcode returns a barcode result object.** If your code stores the result of `b.Encode()` in a variable typed as `Image` or `System.Drawing.Image`, you'll need to update that variable's type and the subsequent save logic. Search for `Image img = b.Encode` and `Image barcode = b.Encode` patterns.

**`b.Width` and `b.Height` are properties in BarcodeLib, `.ResizeTo()` is a method in IronBarcode.** A simple find-and-replace on `b.Width =` and `b.Height =` won't produce valid C#. These two property assignments need to become a single `.ResizeTo(width, height)` call chained after `CreateBarcode()`. Find them together:

```bash
# Find Width/Height property assignments on Barcode objects
grep -n "\.Width = \|\.Height = " --include="*.cs" -r .
```

**`b.IncludeLabel = true` is a boolean toggle; IronBarcode's equivalent takes the label text explicitly.** `.AddAnnotationTextBelowBarcode("text")` lets you pass the label string directly. In most cases, you'll pass the same data string you encoded into the barcode. If your original code used `IncludeLabel = true` and relied on BarcodeLib to automatically render the data as the label, pass the same data string explicitly in IronBarcode.

**Removing SkiaSharp references that existed only for BarcodeLib.** If your project had `<PackageReference Include="SkiaSharp">` only because BarcodeLib required a specific version override, you can remove that explicit reference after switching to IronBarcode. Check whether any other packages still need it:

```bash
# Check if SkiaSharp is still referenced by anything other than the explicit override
dotnet list package --include-transitive 2>&1 | grep -i skia
```

## Migration Checklist

Run these searches before starting to understand the scope:

```bash
# Find all BarcodeLib using directives
grep -rn "using BarcodeLib" --include="*.cs" .

# Find Barcode object instantiation
grep -rn "new Barcode()" --include="*.cs" .

# Find Encode calls (with the b. prefix common in BarcodeLib code)
grep -rn "\.Encode(" --include="*.cs" .

# Find TYPE enum usage
grep -rn "TYPE\.CODE128\|TYPE\.EAN13\|TYPE\.QR_Code\|TYPE\.UPCA\|TYPE\.CODE39" --include="*.cs" .

# Find IncludeLabel usage
grep -rn "IncludeLabel" --include="*.cs" .

# Find the package references in project files
grep -rn "BarcodeLib\|BarcodeLib\.SkiaSharp\|BarcodeLib\.Signed" --include="*.csproj" .

# Find NU1608 evidence in lock files (if you have package.lock.json files)
grep -rn "NU1608" .
```

Work through the checklist in this order:

- [ ] Run searches above and note affected files
- [ ] Run `dotnet remove package BarcodeLib` (or `BarcodeLib.Signed` / `BarcodeLib.SkiaSharp` as applicable) for each project
- [ ] Remove any explicit SkiaSharp version override references added only to fix BarcodeLib conflicts
- [ ] Run `dotnet add package IronBarcode` for each project
- [ ] Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` to application startup
- [ ] Replace `using BarcodeLib;` with `using IronBarCode;` across all `.cs` files
- [ ] Replace `new Barcode()` + property setters with `BarcodeWriter.CreateBarcode()` fluent calls
- [ ] Replace `TYPE.CODE128` → `BarcodeEncoding.Code128` (and all other `TYPE.*` values)
- [ ] Replace `b.Width = N; b.Height = M;` patterns with `.ResizeTo(N, M)`
- [ ] Replace `b.IncludeLabel = true;` with `.AddAnnotationTextBelowBarcode(data)`
- [ ] Replace `img.Save(path, ImageFormat.Png)` with `.SaveAsPng(path)` or `.ToPngBinaryData()`
- [ ] Remove `using System.Drawing.Imaging;` imports that existed only for `ImageFormat.Png`
- [ ] Add barcode reading code where needed (`BarcodeReader.Read()`)
- [ ] Build and verify no NU1608 warnings remain
- [ ] Test barcode generation output against known-good samples
- [ ] Verify cross-platform builds if targeting Linux or macOS

## Pricing Reference

IronBarcode licenses are perpetual with no annual renewal required:

| Tier | Developers | Price |
|---|---|---|
| Lite | 1 developer | $749 |
| Plus | 3 developers | $1,499 |
| Professional | 10 developers | $2,999 |
| Unlimited | Unlimited | $5,999 |

All tiers include royalty-free deployment and cover .NET Framework 4.6.2 through .NET 9.

## What You Gain

The immediate gains are: no more NU1608 warnings, no more SkiaSharp version negotiation, and a barcode reading API that doesn't require a second library. The generation code is shorter and doesn't go through an intermediate `System.Drawing.Image`. The `ToPngBinaryData()` method eliminates the `MemoryStream` pattern that BarcodeLib required for byte array output.

Beyond the direct replacement, you get reading capability that BarcodeLib never offered. If your application will ever need to scan barcodes from images, read them from PDFs, or verify that generated barcodes decode correctly — capabilities that are increasingly common as systems evolve — those are available immediately with the same library and the same `using` statement.

The SkiaSharp conflict was the acute problem. The reading gap was the structural limitation that eventually made the migration inevitable regardless.
