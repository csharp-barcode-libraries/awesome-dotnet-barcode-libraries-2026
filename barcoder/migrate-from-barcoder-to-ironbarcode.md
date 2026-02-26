# Migrating from Barcoder to IronBarcode

This guide covers the complete migration path from Barcoder to IronBarcode for .NET developers. The migration consolidates two packages into one, replaces the encoder-renderer pipeline with a single method call, and collapses all format-specific namespace imports — `using Barcoder.Code128`, `using Barcoder.Qr`, `using Barcoder.DataMatrix` — into a single `using IronBarCode;`. The `IBarcode` type and the `ImageRenderer` / `ImageRendererOptions` types are removed entirely. If reading was a missing capability that caused you to add a second library alongside Barcoder, IronBarcode covers reading natively with `BarcodeReader.Read()`.

## Why Migrate from Barcoder

Teams migrating from Barcoder report these triggers:

**Reading Requirement Added:** Barcoder is a generation-only library with no decoding capability. When an application needs to verify incoming barcodes, process scanned documents, or read barcodes from uploaded PDFs, Barcoder provides no path forward. Adding a second library for reading creates a split codebase with two NuGet dependencies, two APIs to maintain, and two version histories to track. IronBarcode handles both generation and reading through the same package.

**.NET Framework Compatibility Broken:** `Barcoder.Renderer.Image` dropped .NET Framework support. Teams running services or desktop applications on .NET Framework 4.x who update the image renderer package during routine dependency maintenance encounter a build failure. IronBarcode supports .NET Framework 4.6.2 through .NET 9 without conditional packaging for different targets.

**Package Version Drift:** `Barcoder` and `Barcoder.Renderer.Image` are independently versioned NuGet packages. Updating one without the other during a dependency refresh can introduce incompatibilities. In repositories with multiple projects, ensuring consistent versions of each renderer across every project is a coordination burden that grows with team size.

**Multiple Formats Being Added:** A project that starts with Code128 and later adds QR, then DataMatrix, must add a new namespace import and learn a different encoder class with different method parameters for each format. IronBarcode uses a `BarcodeEncoding` enum — adding a format is a one-word change to an existing call. No new namespace, no new class, no different method signature.

**MAUI and Cloud Deployment Requirements:** Barcoder does not document support for MAUI, Docker, AWS Lambda, or Azure deployments. Teams building cross-platform mobile applications or serverless barcode processing pipelines find that Barcoder's documented targets do not cover these scenarios.

### The Fundamental Problem

Barcoder couples format selection to namespace imports and forces a renderer pipeline for every output operation. Switching formats or output types requires structural changes to the code:

```csharp
// Barcoder: 3 namespaces, encoder-specific class, renderer pipeline, stream management
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

IronBarcode expresses the same operation as a single chained call:

```csharp
// IronBarcode: 1 namespace, 1 method, 1 output call
using IronBarCode;

BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");
```

## IronBarcode vs Barcoder: Feature Comparison

| Feature | Barcoder | IronBarcode |
|---------|----------|-------------|
| **NuGet packages required** | 2 minimum (Barcoder + Barcoder.Renderer.Image) | 1 (IronBarcode) |
| **Barcode generation** | Yes | Yes |
| **Barcode reading / decoding** | No | Yes |
| **Per-format encoder classes** | Yes — `Code128Encoder`, `QrEncoder`, `DataMatrixEncoder`, etc. | No — `BarcodeEncoding` enum |
| **Per-format namespace imports** | Yes — `Barcoder.Code128`, `Barcoder.Qr`, `Barcoder.DataMatrix` | No — `using IronBarCode` only |
| **Output methods on result object** | No — `IBarcode` has no save methods | Yes — `GeneratedBarcode` has `SaveAsPng`, `ToPngBinaryData`, `ToStream` |
| **PDF reading** | No | Yes — `BarcodeReader.Read(path)` |
| **QR with logo** | No | Yes — `.AddBrandLogo(path)` |
| **.NET Framework support** | Dropped in image renderer | .NET Framework 4.6.2+ |
| **.NET 9 support** | Unclear / limited activity | Yes |
| **MAUI support** | No | Yes — iOS, Android, Windows, macOS |
| **Docker / Azure / AWS Lambda** | Not documented | Yes |
| **License** | MIT (open source) | Commercial — Lite $749, Plus $1,499, Professional $2,999, Unlimited $5,999 |
| **ReadingSpeed control** | No | Yes — `ReadingSpeed` enum |
| **Multi-barcode detection** | No | Yes — `ExpectMultipleBarcodes` option |

## Quick Start: Barcoder to IronBarcode Migration

### Step 1: Replace NuGet Package

Remove all Barcoder packages from your project:

```bash
dotnet remove package Barcoder
dotnet remove package Barcoder.Renderer.Image
```

If you also added the SVG renderer:

```bash
dotnet remove package Barcoder.Renderer.Svg
```

Install IronBarcode:

```bash
dotnet add package IronBarcode
```

### Step 2: Update Namespaces

Remove all `using Barcoder.*` statements from every file in the project:

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

Replace all of them with a single using directive:

```csharp
using IronBarCode;
```

### Step 3: Initialize License

Add license initialization at application startup — `Program.cs`, `Startup.cs`, or `MauiProgram.cs`:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

A free trial key is available at [ironsoftware.com](https://ironsoftware.com/csharp/barcode/).

## Code Migration Examples

### Code128 Generation

**Barcoder Approach:**

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

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");
```

The `Code128Encoder`, `ImageRenderer`, `ImageRendererOptions`, and stream management are removed entirely. The format is specified as `BarcodeEncoding.Code128` on the unified `BarcodeWriter` class. Output methods live directly on the returned `GeneratedBarcode` object. The [IronBarcode barcode generation documentation](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/) covers styling, margin, and color customization.

### QR Code Generation

**Barcoder Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com", 500)
    .SaveAsPng("qr.png");
```

The second parameter to `CreateQrCode` is the output dimension in pixels. Error correction level defaults to a sensible value and is configurable if needed. IronBarcode also supports an operation not possible in Barcoder — embedding a brand logo in the QR code center:

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com", 500)
    .AddBrandLogo("logo.png")
    .SaveAsPng("qr-branded.png");
```

### DataMatrix Generation

**Barcoder Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("ITEM-XYZ-001", BarcodeEncoding.DataMatrix)
    .SaveAsPng("datamatrix.png");
```

The pattern is identical to Code128 — only the `BarcodeEncoding` value changes. No new namespace import, no different class, no different method signature.

### Controlling Output Size

Barcoder uses `PixelSize` as a scale multiplier on the barcode module size and `BarHeightFor1DBarcode` for 1D height — there is no direct width/height API.

**Barcoder Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("barcode.png");
```

`.ResizeTo(width, height)` takes explicit pixel dimensions. It chains with other output methods on the same `GeneratedBarcode` object.

### Getting Binary Data Instead of a File

**Barcoder Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

byte[] pngBytes = BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

The `MemoryStream` and `ToArray()` pattern is replaced by a single chained method call on `GeneratedBarcode`.

### Reading Barcodes (Net-New Capability)

Barcoder has no reading API. If you previously added a second library for reading, it can be removed. IronBarcode reads from files, byte arrays, streams, and PDFs with the same package used for generation.

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Read from image file
var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
{
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Format: {result.Format}");
}

// Read from PDF natively — no image extraction step, no extra dependency
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

// Read from a stream directly
using var fileStream = File.OpenRead("barcode.png");
var streamResults = BarcodeReader.Read(fileStream);
```

The [IronBarcode barcode reading documentation](https://ironsoftware.com/csharp/barcode/how-to/read-barcode-from-pdf/) covers multi-page PDFs, region-of-interest reading, and performance tuning options.

### SVG Output

**Barcoder Approach:**

```csharp
using Barcoder.Renderers;
var svgRenderer = new SvgRenderer();
using var stream = File.OpenWrite("barcode.svg");
svgRenderer.Render(barcode, stream);
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)
    .SaveAsSvg("barcode.svg");
```

The `Barcoder.Renderer.Svg` package can be removed once all SVG rendering sites are migrated.

## Barcoder API to IronBarcode Mapping Reference

| Barcoder | IronBarcode |
|----------|-------------|
| `Code128Encoder.Encode("data", false)` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)` |
| `QrEncoder.Encode("data", ErrorCorrectionLevel.M, false, false)` | `QRCodeWriter.CreateQrCode("data", 500)` |
| `DataMatrixEncoder.Encode("data")` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.DataMatrix)` |
| `new ImageRenderer(new ImageRendererOptions { ... })` | Not needed |
| `renderer.Render(barcode, fileStream)` | `.SaveAsPng(path)` |
| `renderer.Render(barcode, memoryStream)` + `ms.ToArray()` | `.ToPngBinaryData()` |
| `IBarcode` | `GeneratedBarcode` |
| `PixelSize = 2, BarHeightFor1DBarcode = 50` | `.ResizeTo(width, height)` |
| `SvgRenderer` + `renderer.Render(barcode, stream)` | `.SaveAsSvg(path)` |
| No reading API | `BarcodeReader.Read(path / stream / bytes / pdf)` |
| 2 packages — `Barcoder` + `Barcoder.Renderer.Image` | 1 package — `IronBarcode` |
| Format namespace per encoder (`Barcoder.Code128`, `Barcoder.Qr`, etc.) | `BarcodeEncoding` enum, single namespace |

## Common Migration Issues and Solutions

### Issue 1: IBarcode Has No Save Methods

**Barcoder:** Code that stores an `IBarcode` variable and passes it to a renderer later follows a deferred rendering pattern. Any helper method that accepts `IBarcode` and calls `renderer.Render()` must be refactored.

**Solution:** Change `IBarcode` type references to `GeneratedBarcode`. The `GeneratedBarcode` object carries its output methods with it — pass it to any method that needs to save or serialize the result:

```csharp
// IronBarcode — GeneratedBarcode carries output methods
using IronBarCode;

var barcode = BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128);
SaveBarcodeToFile(barcode, "output.png");

static void SaveBarcodeToFile(GeneratedBarcode barcode, string path)
{
    barcode.SaveAsPng(path);
}
```

A search for `renderer.Render(` across the solution will locate all deferred rendering sites.

### Issue 2: PixelSize Has No Direct Equivalent

**Barcoder:** `PixelSize` is a scale multiplier on the barcode's natural module size. The output dimensions depend on content length, format, and the multiplier in combination. `BarHeightFor1DBarcode` controls 1D height independently. There is no direct width/height specification.

**Solution:** Measure the actual pixel output that existing Barcoder code produces and pass those values to `.ResizeTo(width, height)`:

```csharp
BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)
    .ResizeTo(300, 80)
    .SaveAsPng("barcode.png");
```

### Issue 3: Format Namespace Imports Accumulate

**Barcoder:** Each barcode format used in a project adds a `using Barcoder.[Format]` directive to each file that uses it. A project using Code128, QR, and DataMatrix typically has three format-specific imports per file plus `using Barcoder` and `using Barcoder.Renderers`.

**Solution:** Run a search for all `using Barcoder` statements and replace all of them with `using IronBarCode;`:

```bash
grep -rn "using Barcoder" --include="*.cs" .
```

Every result is a line to remove, replaced by the single `using IronBarCode;` directive once per file.

### Issue 4: Package Version Sync Errors

**Barcoder:** Building after updating only one of `Barcoder` or `Barcoder.Renderer.Image` can produce type-resolution errors because the two packages may not be compatible at different version combinations.

**Solution:** After removing both Barcoder packages and installing `IronBarcode`, there is one package and one version. Package lock conflicts in this category are eliminated. Verify the `.csproj` file contains only the single `IronBarcode` reference and no remaining `Barcoder.*` references before building.

## Barcoder Migration Checklist

### Pre-Migration Tasks

Audit your codebase to identify all Barcoder usage before making changes:

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

Document all locations where `IBarcode` is stored as a variable type or passed as a method parameter — these are the deferred rendering sites that require the most attention. Note all `PixelSize` and `BarHeightFor1DBarcode` values and measure the corresponding output dimensions.

### Code Update Tasks

1. Remove `Barcoder` NuGet package from all projects
2. Remove `Barcoder.Renderer.Image` NuGet package from all projects
3. Remove `Barcoder.Renderer.Svg` NuGet package if present
4. Install `IronBarcode` NuGet package
5. Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` at application startup
6. Replace all `using Barcoder.*` statements with `using IronBarCode;`
7. Replace `Code128Encoder.Encode(...)` with `BarcodeWriter.CreateBarcode(..., BarcodeEncoding.Code128)`
8. Replace `QrEncoder.Encode(...)` with `QRCodeWriter.CreateQrCode(..., size)`
9. Replace `DataMatrixEncoder.Encode(...)` with `BarcodeWriter.CreateBarcode(..., BarcodeEncoding.DataMatrix)`
10. Remove all `new ImageRenderer(new ImageRendererOptions {...})` constructions
11. Replace `renderer.Render(barcode, fileStream)` with `.SaveAsPng(path)` on `GeneratedBarcode`
12. Replace `renderer.Render(barcode, memoryStream)` + `ms.ToArray()` with `.ToPngBinaryData()`
13. Replace `SvgRenderer` usage with `.SaveAsSvg(path)`
14. Change `IBarcode` type references to `GeneratedBarcode`
15. Replace `PixelSize` / `BarHeightFor1DBarcode` options with `.ResizeTo(width, height)`

### Post-Migration Testing

- Verify each generated barcode format scans correctly with a standard scanner or IronBarcode's own reader
- Compare visual output dimensions to pre-migration output — confirm `.ResizeTo()` values produce equivalent sizes
- Test QR code output against a phone scanner to confirm error correction level is adequate
- Verify SVG output renders correctly in browser and print contexts if your project uses SVG
- If you removed a second reading library, verify `BarcodeReader.Read()` produces equivalent decoded values for the same inputs
- Test PDF reading if your project processes PDF documents — confirm multi-page PDFs read all pages correctly
- Run a build with zero `Barcoder.*` references remaining — compiler errors indicate missed migration sites

## Key Benefits of Migrating to IronBarcode

**Unified Package Dependency:** One NuGet package replaces two (or three, if the SVG renderer was also used). One version to track, one changelog to monitor, and no cross-package compatibility issues during dependency updates. The `.csproj` and `packages.lock.json` complexity decreases immediately.

**Native Reading Capability:** Applications that previously required a second library for barcode reading can consolidate onto IronBarcode. `BarcodeReader.Read()` accepts image files, byte arrays, streams, and PDFs through the same API, with multi-barcode detection and reading speed tuning built in.

**Format Selection as Configuration:** Adding a new barcode format to an existing project is a one-word change to the `BarcodeEncoding` enum value in an existing `CreateBarcode` call. No new namespace import, no new encoder class, and no new package. This makes format expansion low-friction as product requirements evolve.

**Explicit Output Dimensions:** `.ResizeTo(width, height)` replaces the indirect `PixelSize` multiplier system with an explicit pixel specification. The relationship between code and output is direct and predictable, independent of barcode content length or format-specific module sizing.

**Expanded Platform Coverage:** IronBarcode supports .NET Framework 4.6.2 through .NET 9, MAUI deployments on iOS, Android, Windows, and macOS, and documented deployment to Docker, AWS Lambda, and Azure Functions. Teams whose platform requirements expand beyond .NET Core will not encounter the framework support gap that affects `Barcoder.Renderer.Image`.

**Active Maintenance and Future Compatibility:** IronBarcode releases regular updates with documented .NET compatibility timelines. The library's active development cadence provides assurance of compatibility with upcoming .NET versions, including .NET 10 expected in late 2026, which is not available from a library with limited recent release activity.
