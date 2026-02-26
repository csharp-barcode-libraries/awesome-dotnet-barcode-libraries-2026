# Migrate from LEADTOOLS Barcode to IronBarcode

Every LEADTOOLS deployment carries a `.LIC` file. That file has to live somewhere the process can reach it, at a path you have configured, with permissions set correctly, whether you are on a developer laptop, a build agent, a staging server, or a production container. When the path is wrong, or the file is missing, or the permissions are off, LEADTOOLS refuses to initialize — not gracefully, not with a watermark, but hard. You write defensive code around it because you have to.

This guide replaces all of that with a string.

The migration removes five LEADTOOLS NuGet packages, eliminates the `MSVC++ 2017 Runtime` dependency on Windows, strips out 20+ lines of initialization boilerplate, and replaces the entire license check sequence with `IronBarCode.License.LicenseKey = "your-key"`. The reading and writing API surfaces are directionally similar — `BarcodeEngine.Reader` becomes `BarcodeReader`, `BarcodeEngine.Writer` becomes `BarcodeWriter` — but IronBarcode's static methods take file paths directly instead of requiring you to load images through a codec layer first.

---

## Why Migrate

**The license file is a deployment artifact, not a configuration value.** Strings belong in environment variables, secrets managers, and configuration files. Binary files belong in source control or artifact stores. LEADTOOLS conflates these two things. Every new environment — developer onboarding, new CI agent, new cloud region, new Kubernetes node pool — requires provisioning the license file. IronBarcode's string key fits cleanly into the secrets management system you already use.

**Five packages for one capability.** `Leadtools.Barcode` depends on `Leadtools` (core), `Leadtools.Codecs` (codec infrastructure), and format-specific codec packages for each image type you work with. PDF support adds another. IronBarcode is one package that includes all image format support and native PDF barcode extraction.

**The LEADTOOLS API is a consequence of its history.** When you read a barcode with LEADTOOLS, you instantiate a `RasterCodecs` to load the image, a `BarcodeEngine` to scan it, and you pass an explicit array of `BarcodeSymbology` values for every format you expect to encounter. When you generate one, you create a `BarcodeData` object, construct a `RasterImage` with explicit bit depth and byte order parameters, fill it with `FillCommand`, call `WriteBarcode`, and save with `RasterCodecs`. That API was designed in the era of explicit resource management and manual configuration. IronBarcode was designed after that era.

**Pricing transparency.** LEADTOOLS development licenses are $1,295–$1,469 per developer annually, and deployment licenses for production servers are separately quoted through sales. IronBarcode's Professional license is $1,499 one-time — perpetual, covering 10 developers and unlimited servers, with no deployment tracking required.

---

## Quick Start (3 Steps)

### Step 1: Remove LEADTOOLS Packages

```bash
dotnet remove package Leadtools.Barcode
dotnet remove package Leadtools
dotnet remove package Leadtools.Codecs
dotnet remove package Leadtools.Codecs.Png
dotnet remove package Leadtools.Codecs.Jpeg
```

If you also added the PDF codec:

```bash
dotnet remove package Leadtools.Codecs.Pdf
```

On Windows Server deployments, you can also remove the MSVC++ 2017 Runtime from your provisioning scripts — IronBarcode has no native runtime dependencies.

### Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

### Step 3: Replace License Initialization

Find every call to `RasterSupport.SetLicense` in your codebase — there should be exactly one, at application startup — and replace the entire initialization block with a single assignment. The before and after look like this:

**Before:**
```csharp
using Leadtools;
using Leadtools.Barcode;

// Step 1: set license with file path + key
RasterSupport.SetLicense(
    @"C:\LEADTOOLS23\Support\Common\License\LEADTOOLS.LIC",
    "your-developer-key-here");

// Step 2: verify license has not expired
if (RasterSupport.KernelExpired)
    throw new InvalidOperationException("LEADTOOLS license has expired");

// Step 3: verify each barcode feature is unlocked
if (RasterSupport.IsLocked(RasterSupportType.Barcode1DRead))
    throw new InvalidOperationException("1D barcode reading is locked");

if (RasterSupport.IsLocked(RasterSupportType.Barcode2DRead))
    throw new InvalidOperationException("2D barcode reading is locked");

if (RasterSupport.IsLocked(RasterSupportType.BarcodeWrite))
    throw new InvalidOperationException("Barcode writing is locked");

// Step 4: create the engine
var engine = new BarcodeEngine();
```

**After:**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

For environment-based license configuration, Docker deployment patterns, and [license key setup](https://ironsoftware.com/csharp/barcode/licensing/), the IronBarcode documentation has full examples for each deployment target.

---

## Code Migration Examples

### Reading Barcodes from Images

The conceptual difference is that LEADTOOLS separates image loading (via `RasterCodecs`) from barcode scanning (via `BarcodeEngine`), and requires you to declare which symbologies to search for. IronBarcode takes a file path and auto-detects all formats across its 50+ supported symbologies.

**Before (LEADTOOLS):**
```csharp
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

// RasterCodecs handles image loading
using var codecs = new RasterCodecs();
using var image = codecs.Load("barcode.png");

var engine = new BarcodeEngine();

// Must enumerate every symbology you expect
var symbologies = new[]
{
    BarcodeSymbology.Code128,
    BarcodeSymbology.Code39,
    BarcodeSymbology.QR,
    BarcodeSymbology.DataMatrix,
    BarcodeSymbology.EAN13,
    BarcodeSymbology.UPCA
};

var barcodes = engine.Reader.ReadBarcodes(
    image,
    LogicalRectangle.Empty,  // search entire image
    0,                        // 0 = find all barcodes
    symbologies);

foreach (var barcode in barcodes)
    Console.WriteLine($"{barcode.Value} ({barcode.Symbology})");
```

**After (IronBarcode):**
```csharp
using IronBarCode;

var results = BarcodeReader.Read("barcode.png");

foreach (var result in results)
    Console.WriteLine($"{result.Value} ({result.Format})");
```

For advanced configuration — tuning reading speed, enabling multi-barcode detection, or working with low-quality images — the [reading barcodes from images documentation](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) covers `BarcodeReaderOptions` in detail:

```csharp
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};

var results = BarcodeReader.Read("image.png", options);
```

---

### Generating Barcodes

LEADTOOLS generation requires constructing a blank `RasterImage` with explicit pixel parameters before you can write a barcode onto it. IronBarcode's `BarcodeWriter` handles image creation internally.

**Before (LEADTOOLS):**
```csharp
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

var engine = new BarcodeEngine();

// Configure the barcode data object
var barcodeData = new BarcodeData(BarcodeSymbology.Code128)
{
    Value = "12345",
    Bounds = new LeadRect(0, 0, 400, 100)
};

// Create a blank raster image to write onto
using var image = new RasterImage(
    RasterMemoryFlags.Conventional,
    400, 100, 24,
    RasterByteOrder.Bgr,
    RasterViewPerspective.TopLeft,
    null, IntPtr.Zero, 0);

// Fill with white background
new FillCommand(RasterColor.White).Run(image);

// Write the barcode onto the image
engine.Writer.WriteBarcode(image, barcodeData, null);

// Save the image
using var codecs = new RasterCodecs();
codecs.Save(image, "out.png", RasterImageFormat.Png, 0);
```

**After (IronBarcode):**
```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("out.png");
```

For a full walkthrough of sizing, styling, margin control, and output formats, see [creating barcode images](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/).

---

### Generating QR Codes

**Before (LEADTOOLS):**
```csharp
var engine = new BarcodeEngine();

var qrData = new BarcodeData(BarcodeSymbology.QR)
{
    Value = "https://example.com",
    Bounds = new LeadRect(0, 0, 500, 500)
};

using var image = new RasterImage(
    RasterMemoryFlags.Conventional,
    500, 500, 24,
    RasterByteOrder.Bgr,
    RasterViewPerspective.TopLeft,
    null, IntPtr.Zero, 0);

new FillCommand(RasterColor.White).Run(image);
engine.Writer.WriteBarcode(image, qrData, null);

using var codecs = new RasterCodecs();
codecs.Save(image, "qr.png", RasterImageFormat.Png, 0);
```

**After (IronBarcode):**
```csharp
using IronBarCode;

var qr = QRCodeWriter.CreateQrCode(
    "https://example.com",
    500,
    QRCodeWriter.QrErrorCorrectionLevel.Highest);

qr.AddBrandLogo("logo.png");   // optional — embed a logo in the center
qr.SaveAsPng("qr.png");
```

---

### Reading Barcodes from PDFs

LEADTOOLS requires the separate `Leadtools.Codecs.Pdf` package for PDF support, plus manual page iteration. IronBarcode includes PDF support in the base package and handles multi-page documents automatically.

**Before (LEADTOOLS):**
```csharp
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

// Requires: dotnet add package Leadtools.Codecs.Pdf
using var codecs = new RasterCodecs();
codecs.Options.Pdf.Load.Resolution = 300;
codecs.Options.Pdf.Load.DisplayDepth = 24;

var engine = new BarcodeEngine();
int pageCount = codecs.GetTotalPages("document.pdf");

for (int page = 1; page <= pageCount; page++)
{
    using var image = codecs.Load(
        "document.pdf",
        0,
        CodecsLoadByteOrder.BgrOrGray,
        page,
        page);

    var barcodes = engine.Reader.ReadBarcodes(
        image, LogicalRectangle.Empty, 0, null);

    foreach (var barcode in barcodes)
        Console.WriteLine($"Page {page}: {barcode.Value}");
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

var results = BarcodeReader.Read("document.pdf");

foreach (var result in results)
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
```

---

### Docker Deployment

The file management problem that defines every LEADTOOLS deployment disappears entirely.

**Before (LEADTOOLS Dockerfile):**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY publish/ .

# License file must travel with every deployment
COPY LEADTOOLS.LIC /app/license/LEADTOOLS.LIC

ENV LEADTOOLS_LICENSE_PATH=/app/license/LEADTOOLS.LIC
ENV LEADTOOLS_DEVELOPER_KEY=your-developer-key

ENTRYPOINT ["dotnet", "YourApp.dll"]
```

**After (IronBarcode Dockerfile):**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY publish/ .

ENV IRONBARCODE_LICENSE=your-license-key

ENTRYPOINT ["dotnet", "YourApp.dll"]
```

The license key can be injected at runtime through Docker secrets, Kubernetes secrets, or any environment variable mechanism — no file provisioning required. The [Docker and Linux deployment guide](https://ironsoftware.com/csharp/barcode/get-started/docker-linux/) covers Alpine and Debian base images, multi-stage builds, and Kubernetes deployment patterns.

---

## API Mapping Reference

| LEADTOOLS | IronBarcode | Notes |
|---|---|---|
| `RasterSupport.SetLicense(path, key)` | `IronBarCode.License.LicenseKey = "key"` | Key only — no file |
| `RasterSupport.KernelExpired` | (removed) | No expiry check needed |
| `RasterSupport.IsLocked(RasterSupportType.Barcode1DRead)` | (removed) | All features included |
| `RasterSupport.IsLocked(RasterSupportType.Barcode2DRead)` | (removed) | All features included |
| `RasterSupport.IsLocked(RasterSupportType.BarcodeWrite)` | (removed) | All features included |
| `new BarcodeEngine()` | Static — no instance | `BarcodeReader`, `BarcodeWriter` are static |
| `new RasterCodecs()` | (removed) | Pass file path directly |
| `codecs.Load(imagePath)` | (removed) | Pass file path directly |
| `engine.Reader.ReadBarcodes(image, LogicalRectangle.Empty, 0, symbologies)` | `BarcodeReader.Read(imagePath)` | Auto-detects symbologies |
| `BarcodeData.Value` | `result.Value` | Same property name |
| `BarcodeData.Symbology` | `result.Format` | Property renamed |
| `new BarcodeData(BarcodeSymbology.Code128)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)` | Fluent creation |
| `BarcodeSymbology.Code128` | `BarcodeEncoding.Code128` | Namespace change |
| `BarcodeSymbology.QR` | `BarcodeEncoding.QRCode` | Name change |
| `BarcodeSymbology.DataMatrix` | `BarcodeEncoding.DataMatrix` | Same name |
| `BarcodeSymbology.PDF417` | `BarcodeEncoding.PDF417` | Same name |
| `BarcodeSymbology.EAN13` | `BarcodeEncoding.EAN13` | Same name |
| `BarcodeSymbology.UPCA` | `BarcodeEncoding.UPCA` | Same name |
| `BarcodeSymbology.Interleaved2of5` | `BarcodeEncoding.Interleaved2of5` | Same name |
| `BarcodeSymbology.Codabar` | `BarcodeEncoding.Codabar` | Same name |
| `engine.Writer.WriteBarcode(image, data, null)` + `codecs.Save(...)` | `.SaveAsPng(path)` | One method chain |
| `new RasterImage(...)` + `new FillCommand(RasterColor.White).Run(image)` | (removed) | Internal to IronBarcode |

---

## Migration Checklist

Use these search terms to find LEADTOOLS code that needs updating. Most codebases have a single initialization block and scattered read/write calls.

**Find and remove LEADTOOLS initialization:**
- [ ] Search: `using Leadtools` — remove all three namespace imports
- [ ] Search: `RasterSupport.SetLicense` — replace entire initialization block with `IronBarCode.License.LicenseKey`
- [ ] Search: `KernelExpired` — remove
- [ ] Search: `IsLocked(RasterSupportType` — remove all feature lock checks
- [ ] Search: `BarcodeEngine` — remove all instances (IronBarcode uses static classes)
- [ ] Search: `RasterCodecs` — remove all instances from barcode paths

**Update reading code:**
- [ ] Search: `engine.Reader.ReadBarcodes` — replace with `BarcodeReader.Read(path)`
- [ ] Search: `BarcodeData.Value` — update to `result.Value`
- [ ] Search: `BarcodeSymbology.` — replace with `BarcodeEncoding.` (check `QR` → `QRCode`)

**Update generation code:**
- [ ] Search: `new BarcodeData(` — replace with `BarcodeWriter.CreateBarcode(`
- [ ] Search: `engine.Writer.WriteBarcode` — replace with `.SaveAsPng()` or `.SaveAsJpeg()`

**Update deployment configuration:**
- [ ] Remove `COPY LEADTOOLS.LIC` from Dockerfiles
- [ ] Remove `LEADTOOLS_LICENSE_PATH` environment variable references
- [ ] Remove license file from Kubernetes secrets (replace with string key)
- [ ] Remove MSVC++ 2017 Runtime installation from Windows provisioning scripts
- [ ] Remove license file from CI/CD artifact pipeline

**Verify after migration:**
- [ ] All barcode read operations return expected results
- [ ] All generation operations produce valid output
- [ ] PDF barcode extraction works without `Leadtools.Codecs.Pdf`
- [ ] Container starts cleanly without license file present
- [ ] CI/CD pipeline runs without license file handling steps

---

The `.LIC` file is gone. Your Dockerfile is two lines shorter, your CI/CD pipeline has no file-handling step, and your secrets manager stores a string instead of a binary file. The same barcode operations that required 20 lines of initialization and five packages now require one.
