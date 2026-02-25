# Migrating from GrapeCity ComponentOne Barcode to IronBarcode

The migration removes two constraints at once: Windows-only deployment and generation-only capability. The API change is manageable — C1BarCode's property-setter model maps cleanly to IronBarcode's fluent chain, and the concept translations are direct. What you gain is the ability to deploy to Linux and Docker, run in ASP.NET Core on any platform, add barcode reading without a second library, and drop the ComponentOne Studio Enterprise subscription if the barcode control is the primary reason you are paying for it.

This guide covers the package swap, namespace changes, code translation patterns, and the cross-platform target framework change that unlocks Linux/Docker deployment.

## Quick Start

Four steps get the basic migration done:

**Step 1: Remove the ComponentOne package.**

```bash
dotnet remove package C1.Win.C1BarCode
```

If your project references other ComponentOne packages you are keeping, remove only the barcode-related reference. If C1BarCode was your only reason for having ComponentOne Studio installed, also clean up the ComponentOne license configuration from your startup code.

**Step 2: Install IronBarcode.**

```bash
dotnet add package IronBarcode
```

**Step 3: Update namespaces and license initialization.**

```csharp
// Before
using C1.Win.C1BarCode;
// C1.C1License.Key = "YOUR-COMPONENTONE-KEY"; // remove this

// After
using IronBarCode;
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

The IronBarCode license key can be set at any point before the first barcode operation. You do not need to set it at application start — though doing so in `Program.cs` or `Startup.cs` is the cleanest pattern.

**Step 4 (optional but recommended): Update the target framework for cross-platform deployment.**

```xml
<!-- Before: ComponentOne requires Windows -->
<TargetFramework>net8.0-windows</TargetFramework>
<UseWindowsForms>true</UseWindowsForms>

<!-- After: IronBarcode works on any platform -->
<TargetFramework>net8.0</TargetFramework>
```

If your project is a WinForms desktop application that will only run on Windows, you can keep `net8.0-windows`. If your project is an ASP.NET Core API, a console application, a background service, or anything intended to run on Linux, Docker, or Azure — remove the Windows TFM and `UseWindowsForms`. IronBarcode has no dependency on either.

## Code Migration Examples

### Code 128 Generation

The most common C1BarCode usage — generating a 1D barcode and saving it to a file.

**Before (ComponentOne C1BarCode):**

```csharp
using C1.Win.C1BarCode;
using System.Drawing;
using System.Drawing.Imaging;

C1.C1License.Key = "YOUR-COMPONENTONE-KEY";

var barcode = new C1BarCode();
barcode.CodeType = CodeType.Code128;
barcode.Text = "SHIP-20240312-7834";
barcode.BarHeight = 100;
barcode.ModuleSize = 2;
barcode.ShowText = true;
barcode.CaptionPosition = CaptionPosition.Below;

using var image = barcode.GetImage();
image.Save("shipping-label.png", ImageFormat.Png);
```

**After (IronBarcode):**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";

BarcodeWriter.CreateBarcode("SHIP-20240312-7834", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("shipping-label.png");
```

The `C1BarCode` instance is gone. `BarcodeWriter.CreateBarcode()` is a static call — pass the value and the encoding type, chain any styling, call `.SaveAsPng()`. The `BarHeight` and `ModuleSize` properties from C1BarCode translate to `.ResizeTo(width, height)` in pixels — see the sizing note in the Common Migration Issues section below.

### QR Code Generation

**Before (ComponentOne C1BarCode):**

```csharp
using C1.Win.C1BarCode;
using System.Drawing;
using System.Drawing.Imaging;

C1.C1License.Key = "YOUR-COMPONENTONE-KEY";

var barcode = new C1BarCode();
barcode.CodeType = CodeType.QRCode;
barcode.Text = "https://example.com/product/4821";
barcode.QRCodeVersion = QRCodeVersion.Version5;
barcode.QRCodeErrorCorrectionLevel = QRCodeErrorCorrectionLevel.High;
barcode.QRCodeModel = QRCodeModel.Model2;
barcode.ModuleSize = 4;

using var image = barcode.GetImage();
image.Save("product-qr.png", ImageFormat.Png);
```

**After (IronBarcode):**

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode(
        "https://example.com/product/4821",
        300,
        QRCodeWriter.QrErrorCorrectionLevel.High)
    .SaveAsPng("product-qr.png");
```

`QRCodeVersion` has no direct equivalent — IronBarcode selects the version automatically based on data length and error correction level. `QRCodeModel.Model2` is the standard modern QR model and does not require explicit configuration. The second parameter to `CreateQrCode()` is the output size in pixels (here, 300×300).

### QR Code Color Customization

**Before (ComponentOne C1BarCode):**

```csharp
using C1.Win.C1BarCode;
using System.Drawing;

C1.C1License.Key = "YOUR-COMPONENTONE-KEY";

var barcode = new C1BarCode();
barcode.CodeType = CodeType.QRCode;
barcode.Text = "https://example.com/brand/home";
barcode.ForeColor = Color.DarkBlue;
barcode.BackColor = Color.White;
barcode.ModuleSize = 4;

using var image = barcode.GetImage();
image.Save("branded-qr.png", System.Drawing.Imaging.ImageFormat.Png);
```

**After (IronBarcode):**

```csharp
using IronBarCode;
using System.Drawing;

QRCodeWriter.CreateQrCode("https://example.com/brand/home", 300)
    .ChangeBarCodeColor(Color.DarkBlue)
    .SaveAsPng("branded-qr.png");
```

`barcode.ForeColor` maps to `.ChangeBarCodeColor()`. Background color defaults to white — use `.ChangeBackgroundColor()` if you need something other than the default.

### QR Code with Logo (New Capability)

C1BarCode has no logo embedding feature. IronBarcode's `QRCodeWriter` supports it directly via `.AddBrandLogo()`:

```csharp
using IronBarCode;
using System.Drawing;

QRCodeWriter.CreateQrCode("https://example.com/product/4821", 500)
    .AddBrandLogo("company-logo.png")
    .ChangeBarCodeColor(Color.DarkBlue)
    .SaveAsPng("branded-product-qr.png");
```

Use `QrErrorCorrectionLevel.High` or `.Highest` when embedding logos — the error correction capacity compensates for the area covered by the logo image.

### Generating Other 1D Formats

The same `BarcodeWriter.CreateBarcode()` pattern applies to all 1D formats:

```csharp
using IronBarCode;

// EAN-13 (product barcode)
BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
    .SaveAsPng("product-ean13.png");

// Code 39 (uppercase alphanumeric)
BarcodeWriter.CreateBarcode("ORDER-7734", BarcodeEncoding.Code39)
    .ResizeTo(350, 90)
    .SaveAsPng("order-code39.png");

// PDF417 (high-capacity 2D)
BarcodeWriter.CreateBarcode("MANIFEST-2024-03-12T08:45:00Z", BarcodeEncoding.PDF417)
    .SaveAsPng("manifest-pdf417.png");
```

### Returning Barcode as Byte Array

C1BarCode's `GetImage()` returns a `System.Drawing.Image`, which you then call `.Save()` on with an `ImageFormat`. If your code passes that image to an HTTP response or a downstream system, the IronBarcode equivalent is `.ToPngBinaryData()`:

**Before:**

```csharp
using C1.Win.C1BarCode;
using System.Drawing;
using System.IO;

C1.C1License.Key = "YOUR-COMPONENTONE-KEY";

var barcode = new C1BarCode();
barcode.CodeType = CodeType.Code128;
barcode.Text = "ITEM-9921";

using var image = barcode.GetImage();
using var ms = new MemoryStream();
image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
byte[] barcodeBytes = ms.ToArray();
// return barcodeBytes to caller or HTTP response
```

**After:**

```csharp
using IronBarCode;

byte[] barcodeBytes = BarcodeWriter.CreateBarcode("ITEM-9921", BarcodeEncoding.Code128)
    .ToPngBinaryData();
// return barcodeBytes to caller or HTTP response
```

`.ToPngBinaryData()` eliminates the `MemoryStream` roundtrip. The byte array is ready to use directly.

### Adding Barcode Reading (Net-New Capability)

Reading is not a migration from C1BarCode — it is a new capability. No equivalent existed before. The most common starting point is reading from a single image file:

```csharp
using IronBarCode;

var results = BarcodeReader.Read("scanned-label.png");
foreach (var barcode in results)
{
    Console.WriteLine($"Format: {barcode.Format}");
    Console.WriteLine($"Value: {barcode.Value}");
}
```

For multi-barcode images or higher-throughput scenarios, use `BarcodeReaderOptions`:

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    ExpectedBarcodeTypes = BarcodeEncoding.Code128 | BarcodeEncoding.QRCode
};

var results = BarcodeReader.Read("warehouse-shelf-photo.jpg", options);
foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.Format}: {barcode.Value}");
}
```

Reading from PDF documents works with the same method — no separate PDF extraction step needed:

```csharp
using IronBarCode;

var results = BarcodeReader.Read("invoice-batch.pdf");
foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Value}");
}
```

### Removing the Windows Constraint

If your project was `net8.0-windows` because C1BarCode required it, and you are now running an ASP.NET Core API or a background service, the target framework change is straightforward:

**Before (project file):**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="C1.Win.C1BarCode" Version="..." />
  </ItemGroup>
</Project>
```

**After (project file):**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="IronBarcode" Version="..." />
  </ItemGroup>
</Project>
```

After changing the target framework, check the rest of the project for any other `net8.0-windows` or WinForms dependencies. If C1BarCode was the only WinForms dependency, the project should build clean after the package swap and namespace update.

For WinForms desktop applications that are staying on Windows, you can keep `net8.0-windows`. IronBarcode runs there too. The change is only necessary if Linux or cross-platform deployment is the goal.

## Common Migration Issues

### BarHeight and ModuleSize vs. ResizeTo

C1BarCode's `BarHeight` and `ModuleSize` are in module units, which is the width of the narrowest bar in the barcode. Setting `ModuleSize = 2` makes each module 2 units wide; `BarHeight = 100` sets the bar height in the same unit system.

IronBarcode's `.ResizeTo(width, height)` takes pixel dimensions for the entire output image. The two systems do not convert with a simple multiplier — you need to work out the target image size you actually want.

A practical starting point: a `ModuleSize = 2, BarHeight = 100` Code 128 barcode for a short string produces an image roughly 200-300px wide and 80-120px tall, depending on the data length. Start with `.ResizeTo(300, 100)` and adjust visually.

```csharp
// Approximate equivalent for ModuleSize = 2, BarHeight = 100
BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .SaveAsPng("barcode.png");
```

### License Key Timing

C1BarCode required `C1.C1License.Key = "..."` to be set before the first `new C1BarCode()` instantiation. IronBarCode.License.LicenseKey can be set at any point before the first barcode operation — instantiating options objects or building configuration does not trigger the license check. The check happens when `BarcodeWriter.CreateBarcode()`, `QRCodeWriter.CreateQrCode()`, or `BarcodeReader.Read()` is first called.

The recommended pattern is still to set it at startup:

```csharp
// In Program.cs or Startup.cs
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### GetImage() to ToPngBinaryData()

`barcode.GetImage()` returns a `System.Drawing.Image`. If your code uses that image object beyond just saving it — passing it to a drawing context, compositing it with other images, using GDI+ operations on it — the migration requires more thought than a direct swap.

For the common case of saving to disk or returning from an API, the direct translation is:

```csharp
// Before: GetImage() + Save()
using var image = barcode.GetImage();
image.Save("output.png", System.Drawing.Imaging.ImageFormat.Png);

// After: SaveAsPng()
BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)
    .SaveAsPng("output.png");
```

```csharp
// Before: GetImage() + MemoryStream
using var image = barcode.GetImage();
using var ms = new MemoryStream();
image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
byte[] bytes = ms.ToArray();

// After: ToPngBinaryData()
byte[] bytes = BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

If you need to work with the barcode as a `System.Drawing.Bitmap` for compositing, call `.ToPngBinaryData()` and construct the `Bitmap` from a `MemoryStream` over the byte array.

### CodeType Enum Values

C1BarCode's `CodeType` enum and IronBarcode's `BarcodeEncoding` enum use different naming conventions. The full mapping:

| C1BarCode CodeType | IronBarcode BarcodeEncoding |
|---|---|
| `CodeType.Code128` | `BarcodeEncoding.Code128` |
| `CodeType.Code39` | `BarcodeEncoding.Code39` |
| `CodeType.Code93` | `BarcodeEncoding.Code93` |
| `CodeType.EAN13` | `BarcodeEncoding.EAN13` |
| `CodeType.EAN8` | `BarcodeEncoding.EAN8` |
| `CodeType.UPCA` | `BarcodeEncoding.UPCA` |
| `CodeType.UPCE` | `BarcodeEncoding.UPCE` |
| `CodeType.ITF14` | `BarcodeEncoding.ITF` |
| `CodeType.QRCode` | `BarcodeEncoding.QRCode` (use `QRCodeWriter`) |
| `CodeType.PDF417` | `BarcodeEncoding.PDF417` |
| `CodeType.DataMatrix` | `BarcodeEncoding.DataMatrix` |
| `CodeType.Codabar` | `BarcodeEncoding.Codabar` |

For QR code generation, `QRCodeWriter.CreateQrCode()` is the recommended API over `BarcodeWriter.CreateBarcode()` with `BarcodeEncoding.QRCode` — it exposes QR-specific options like error correction level and logo embedding.

## Migration Checklist

Search your codebase for these patterns and apply the translations from the sections above:

```bash
# Find all C1BarCode usage
grep -r "using C1.Win.C1BarCode" --include="*.cs" .
grep -r "new C1BarCode()" --include="*.cs" .
grep -r "CodeType\." --include="*.cs" .
grep -r "barcode\.BarHeight" --include="*.cs" .
grep -r "barcode\.ModuleSize" --include="*.cs" .
grep -r "barcode\.GetImage()" --include="*.cs" .
grep -r "C1\.C1License\.Key" --include="*.cs" .
grep -r "QRCodeVersion\." --include="*.cs" .
grep -r "QRCodeErrorCorrectionLevel\." --include="*.cs" .
grep -r "CaptionPosition\." --include="*.cs" .
```

```bash
# Find Windows-specific project configuration
grep -r "net8.0-windows" --include="*.csproj" .
grep -r "UseWindowsForms" --include="*.csproj" .
```

Prioritized migration sequence:

1. Replace `C1.Win.C1BarCode` package reference with `IronBarcode`
2. Replace `using C1.Win.C1BarCode;` with `using IronBarCode;`
3. Replace `C1.C1License.Key = "..."` with `IronBarCode.License.LicenseKey = "..."`
4. Replace each `new C1BarCode()` + property-setter block + `GetImage()` with `BarcodeWriter.CreateBarcode()` chain
5. Replace QR code generation blocks with `QRCodeWriter.CreateQrCode()` chains
6. Translate `CodeType.*` enum values to `BarcodeEncoding.*`
7. Replace `GetImage().Save()` with `.SaveAsPng()` or `.ToPngBinaryData()`
8. Update `BarHeight`/`ModuleSize` pairs to `.ResizeTo(width, height)` pixel values
9. Update project file target framework if cross-platform deployment is needed
10. Add barcode reading with `BarcodeReader.Read()` where applicable
