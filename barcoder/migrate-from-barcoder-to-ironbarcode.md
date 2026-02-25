# Migrating from Barcoder to IronBarcode

The migration consolidates two packages into one and flattens the encoder-renderer pipeline into a single method call. The format-specific namespace imports disappear — `using Barcoder.Code128`, `using Barcoder.Qr`, `using Barcoder.DataMatrix` all get replaced with a single `using IronBarCode;`. The `IBarcode` type and the `ImageRenderer` / `ImageRendererOptions` types go away entirely.

If you added barcode reading to your requirements and found that Barcoder offered nothing, IronBarcode covers reading natively with `BarcodeReader.Read()` — no second library, no new NuGet dependency.

## Why Teams Migrate

- **Reading requirement added:** Barcoder is generation-only. When reading is needed, a new dependency was required. IronBarcode handles both.
- **.NET Framework support broken:** `Barcoder.Renderer.Image` dropped .NET Framework. IronBarcode supports .NET Framework 4.6.2 through .NET 9.
- **Package version sync:** `Barcoder` and `Barcoder.Renderer.Image` are independently versioned. Updating one can break the other. A single package eliminates this.
- **Multiple formats being added:** Each new format in Barcoder means a new namespace import and a new encoder class. IronBarcode uses a `BarcodeEncoding` enum — adding a format is a one-word change.

## Quick Start

### Step 1: Remove Barcoder Packages

```bash
dotnet remove package Barcoder
dotnet remove package Barcoder.Renderer.Image
```

If you also added the SVG renderer:

```bash
dotnet remove package Barcoder.Renderer.Svg
```

### Step 2: Install IronBarcode

```bash
# NuGet: dotnet add package IronBarcode
dotnet add package IronBarcode
```

### Step 3: Replace Namespace Imports

Remove all `using Barcoder.*` statements:

```csharp
// Remove all of these
using Barcoder;
using Barcoder.Code128;
using Barcoder.Qr;
using Barcoder.DataMatrix;
using Barcoder.Renderers;
using Barcoder.Ean;
using Barcoder.Pdf417;
// ... and any other Barcoder.* namespaces in your project
```

Replace with a single using:

```csharp
using IronBarCode;
```

### Step 4: Add License Initialization

At application startup (e.g., `Program.cs`, `Startup.cs`, or `MauiProgram.cs`):

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

A free trial key is available at [ironsoftware.com](https://ironsoftware.com/csharp/barcode/).

## Code Migration Examples

### Code128 Generation

**Before (Barcoder):**

```csharp
using Barcoder;
using Barcoder.Code128;
using Barcoder.Renderers;

IBarcode barcode = Code128Encoder.Encode("PRODUCT-12345", false);

var renderer = new ImageRenderer(new ImageRendererOptions
{
    ImageFormat = ImageFormat.Png,
    PixelSize = 2,
    BarHeightFor1DBarcode = 50
});

using var stream = File.OpenWrite("barcode.png");
renderer.Render(barcode, stream);
```

**After (IronBarcode):**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");
```

The `Code128Encoder`, `ImageRenderer`, `ImageRendererOptions`, and stream management are all gone. The format is specified as `BarcodeEncoding.Code128` on the unified `BarcodeWriter` class. Output methods (`SaveAsPng`, `ToPngBinaryData`, `ToStream`) are on the returned `GeneratedBarcode` object.

### QR Code Generation

**Before (Barcoder):**

```csharp
using Barcoder;
using Barcoder.Qr;
using Barcoder.Renderers;

IBarcode barcode = QrEncoder.Encode("https://example.com", ErrorCorrectionLevel.M, false, false);

var renderer = new ImageRenderer(new ImageRendererOptions
{
    ImageFormat = ImageFormat.Png,
    PixelSize = 4
});

using var stream = File.OpenWrite("qr.png");
renderer.Render(barcode, stream);
```

**After (IronBarcode):**

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com", 500)
    .SaveAsPng("qr.png");
```

The second parameter to `CreateQrCode` is the output dimension in pixels. Error correction level defaults to a sensible value; advanced options are configurable if needed.

**QR with a brand logo (not possible in Barcoder):**

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com", 500)
    .AddBrandLogo("logo.png")
    .SaveAsPng("qr-branded.png");
```

### DataMatrix Generation

**Before (Barcoder):**

```csharp
using Barcoder;
using Barcoder.DataMatrix;
using Barcoder.Renderers;

IBarcode barcode = DataMatrixEncoder.Encode("ITEM-XYZ-001");

var renderer = new ImageRenderer(new ImageRendererOptions
{
    ImageFormat = ImageFormat.Png,
    PixelSize = 5
});

using var stream = File.OpenWrite("datamatrix.png");
renderer.Render(barcode, stream);
```

**After (IronBarcode):**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("ITEM-XYZ-001", BarcodeEncoding.DataMatrix)
    .SaveAsPng("datamatrix.png");
```

The pattern is identical to Code128 — only the `BarcodeEncoding` value changes. No new namespace, no different encoder class.

### Controlling Output Size

Barcoder's size control is indirect — `PixelSize` is a scale factor applied to module size, and `BarHeightFor1DBarcode` controls 1D height separately. There is no direct width/height API.

**Before (Barcoder):**

```csharp
var renderer = new ImageRenderer(new ImageRendererOptions
{
    ImageFormat = ImageFormat.Png,
    PixelSize = 3,
    BarHeightFor1DBarcode = 80
});
using var stream = File.OpenWrite("barcode.png");
renderer.Render(barcode, stream);
```

**After (IronBarcode):**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("barcode.png");
```

`.ResizeTo(width, height)` takes explicit pixel dimensions. It chains with other output methods on the same `GeneratedBarcode` object.

### Getting Binary Data Instead of a File

**Before (Barcoder):**

```csharp
using Barcoder;
using Barcoder.Code128;
using Barcoder.Renderers;

IBarcode barcode = Code128Encoder.Encode("PRODUCT-12345", false);

var renderer = new ImageRenderer(new ImageRendererOptions
{
    ImageFormat = ImageFormat.Png,
    PixelSize = 2
});

using var ms = new MemoryStream();
renderer.Render(barcode, ms);
byte[] pngBytes = ms.ToArray();
```

**After (IronBarcode):**

```csharp
using IronBarCode;

byte[] pngBytes = BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

### Reading Barcodes (Net-New Capability)

Barcoder has no reading API. If you previously used a second library for reading, you can remove it. IronBarcode reads from files, byte arrays, streams, and PDFs.

```csharp
using IronBarCode;

// Read from image file
var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
{
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Format: {result.Format}");
}

// Read from PDF natively — no image extraction step
var pdfResults = BarcodeReader.Read("invoice.pdf");

// Read multiple barcodes from one image
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};
var multiResults = BarcodeReader.Read("warehouse-sheet.png", options);
foreach (var result in multiResults)
    Console.WriteLine(result.Value);
```

### Reading from a Stream

```csharp
using IronBarCode;

// IronBarcode can read from a stream directly
using var fileStream = File.OpenRead("barcode.png");
var results = BarcodeReader.Read(fileStream);
```

## Common Migration Issues

### IBarcode Has No Save Methods

The biggest conceptual shift: in Barcoder, encoding and rendering are decoupled. Code that stores an `IBarcode` variable and renders it later looks like this:

```csharp
// Barcoder pattern — store IBarcode, render elsewhere
IBarcode barcode = Code128Encoder.Encode("data", false);
// ... pass barcode around ...
RenderToFile(barcode, "output.png");

static void RenderToFile(IBarcode barcode, string path)
{
    var renderer = new ImageRenderer(new ImageRendererOptions { ImageFormat = ImageFormat.Png });
    using var stream = File.OpenWrite(path);
    renderer.Render(barcode, stream);
}
```

In IronBarcode, the `GeneratedBarcode` object carries its output methods with it. Refactor to call the output method at the point of use, or pass the `GeneratedBarcode` object:

```csharp
// IronBarcode — GeneratedBarcode carries output methods
using IronBarCode;

var barcode = BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128);
// Pass barcode around if needed — it has .SaveAsPng(), .ToPngBinaryData(), .ToStream()
SaveBarcodeToFile(barcode, "output.png");

static void SaveBarcodeToFile(GeneratedBarcode barcode, string path)
{
    barcode.SaveAsPng(path);
}
```

If your code has many deferred rendering sites, a simple search for `renderer.Render(` will locate all of them.

### PixelSize and BarHeightFor1DBarcode

Barcoder uses `PixelSize` as a multiplier on the barcode's natural module size. There is no direct mapping because the output dimensions depend on the barcode content. Use `.ResizeTo(width, height)` in IronBarcode to specify explicit output dimensions:

```csharp
// If Barcoder code used PixelSize = 2, BarHeightFor1DBarcode = 50:
// Measure what that produced and pass those dimensions to ResizeTo
BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)
    .ResizeTo(300, 80)
    .SaveAsPng("barcode.png");
```

### Version Sync Between Packages Is Gone

With Barcoder, `Barcoder` and `Barcoder.Renderer.Image` are independent NuGet packages. A common issue is bumping one and not the other during dependency updates. Once you are on IronBarcode, there is one package and one version. Your `.csproj` or `packages.lock.json` complexity decreases.

### Barcoder.Renderer.Svg

If your project used the SVG renderer:

```csharp
// Barcoder SVG
using Barcoder.Renderers;
var svgRenderer = new SvgRenderer();
using var stream = File.OpenWrite("barcode.svg");
svgRenderer.Render(barcode, stream);
```

IronBarcode's SVG output is a method on `GeneratedBarcode`:

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)
    .SaveAsSvg("barcode.svg");
```

## Migration Checklist

Run these searches to find all Barcoder usage in your codebase before starting:

```bash
grep -rn "using Barcoder" --include="*.cs" .
grep -rn "Code128Encoder\.Encode" --include="*.cs" .
grep -rn "QrEncoder\.Encode" --include="*.cs" .
grep -rn "DataMatrixEncoder\.Encode" --include="*.cs" .
grep -rn "ImageRenderer\|ImageRendererOptions" --include="*.cs" .
grep -rn "SvgRenderer" --include="*.cs" .
grep -rn "renderer\.Render(" --include="*.cs" .
grep -rn "IBarcode" --include="*.cs" .
grep -rn "ErrorCorrectionLevel" --include="*.cs" .
```

Work through each hit:

- `using Barcoder.*` — replace with `using IronBarCode;`
- `Code128Encoder.Encode(...)` — replace with `BarcodeWriter.CreateBarcode(..., BarcodeEncoding.Code128)`
- `QrEncoder.Encode(...)` — replace with `QRCodeWriter.CreateQrCode(..., size)`
- `DataMatrixEncoder.Encode(...)` — replace with `BarcodeWriter.CreateBarcode(..., BarcodeEncoding.DataMatrix)`
- `new ImageRenderer(new ImageRendererOptions {...})` — remove entirely
- `renderer.Render(barcode, stream)` — replace with `.SaveAsPng(path)` or `.ToPngBinaryData()` on the `GeneratedBarcode`
- `IBarcode` type references — change to `GeneratedBarcode`
- `PixelSize` / `BarHeightFor1DBarcode` — replace with `.ResizeTo(width, height)`

After replacing each file, verify it compiles. The type system will catch anything you missed — `IBarcode` won't resolve, `Code128Encoder` won't resolve, `ImageRenderer` won't resolve.

## API Reference

| Barcoder | IronBarcode |
|----------|-------------|
| `Code128Encoder.Encode("data", false)` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)` |
| `QrEncoder.Encode("data", ErrorCorrectionLevel.M, false, false)` | `QRCodeWriter.CreateQrCode("data", 500)` |
| `DataMatrixEncoder.Encode("data")` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.DataMatrix)` |
| `new ImageRenderer(new ImageRendererOptions {...})` | Not needed |
| `renderer.Render(barcode, fileStream)` | `.SaveAsPng(path)` |
| `renderer.Render(barcode, memoryStream)` + `ms.ToArray()` | `.ToPngBinaryData()` |
| `IBarcode` | `GeneratedBarcode` |
| `PixelSize = 2, BarHeightFor1DBarcode = 50` | `.ResizeTo(width, height)` |
| `SvgRenderer` + `renderer.Render(barcode, stream)` | `.SaveAsSvg(path)` |
| No reading API | `BarcodeReader.Read(path / stream / bytes / pdf)` |
| 2 packages — Barcoder + Barcoder.Renderer.Image | 1 package — IronBarcode |
| Format namespace per encoder | `BarcodeEncoding` enum, single namespace |

The migration is mechanical: remove the renderer pipeline, unify the namespace imports, and swap encoder classes for a `BarcodeEncoding` enum value. The result is fewer lines of code and one fewer package to version-track.
