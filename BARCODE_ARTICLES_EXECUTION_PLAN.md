# Barcode Articles Execution Plan

## Overview

This plan covers 13 competitors (alphabetically preceding `ironbarcode`). For each competitor we produce two articles:

- **Comparison guide** — `compare-[competitor]-vs-ironbarcode.md`
  Style reference: `reference-compare-xfinium-pdf-vs-ironpdf.md`
- **Migration guide** — `migrate-from-[competitor]-to-ironbarcode.md`
  Style reference: `reference-migrate-from-zetpdf-to-ironpdf.md`

Articles are placed in each competitor's own folder (e.g. `accusoft-barcode/compare-accusoft-barcode-vs-ironbarcode.md`).

### Article Style Rules

- Human tone, not robotic. No bullet-point-heavy filler.
- Every code block uses correct, current IronBarcode API (NuGet package: `IronBarcode`).
- Each article has a distinct hook — no two feel like the same article with different names swapped in.
- Comparison articles: feature tables, side-by-side code, when-to-consider section.
- Migration articles: Why Migrate section, Quick Start (3 steps: remove, install, license), Code Migration Examples, API Mapping table, Migration Checklist.
- No article ends with a generic conclusion — each closes on the specific pain point introduced at the top.

### IronBarcode API Quick Reference

```csharp
// Install
dotnet add package IronBarcode

// License
IronBarCode.License.LicenseKey = "YOUR-KEY";

// Generate
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128).SaveAsPng("out.png");
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128).ResizeTo(400, 100).SaveAsPng("out.png");

// Read (auto-detect format, 50+ symbologies)
var result = BarcodeReader.Read("barcode.png").First();
Console.WriteLine($"{result.Value} ({result.Format})");

// Read with options
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};
var results = BarcodeReader.Read("image.png", options);

// Native PDF support
var pdfResults = BarcodeReader.Read("document.pdf");

// QR with logo
var qr = QRCodeWriter.CreateQrCode("https://example.com", 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);
qr.AddBrandLogo("logo.png");
qr.SaveAsPng("qr.png");

// QR with colors
var qr2 = QRCodeWriter.CreateQrCode("https://example.com", 500);
qr2.ChangeBarCodeColor(Color.DarkBlue);
qr2.SaveAsPng("colored-qr.png");
```

---

## Competitor Plans

---

### 1. accusoft-barcode

**Files:**
- `accusoft-barcode/compare-accusoft-barcode-vs-ironbarcode.md`
- `accusoft-barcode/migrate-from-accusoft-barcode-to-ironbarcode.md`

**NuGet to remove:** `Accusoft.BarcodeXpress.NetCore`
**Namespace to remove:** `Accusoft.BarcodeXpressSdk`

**Core Pain Point:** BarcodeXpress uses a mandatory two-key licensing model. Every deployment requires both an SDK license (`SolutionName` + `SolutionKey`) and a separate runtime license (`UnlockRuntime`). The minimum purchase for runtime licenses is 5 seats even for a single-server deployment. In evaluation mode, barcode values are partially obscured — `1234567890` comes back as `1234...XXX` — so you cannot verify accuracy without purchasing both license components first.

**Comparison Hook:** "BarcodeXpress evaluation mode obscures your barcode results. To see actual values, you need to purchase both the SDK license and a minimum of five runtime licenses before you've confirmed the library does what you need."

**Comparison Article Sections:**
1. The two-key problem — show the initialization code:
```csharp
// Accusoft: two separate license components required
var barcodeXpress = new BarcodeXpress();
barcodeXpress.Licensing.SolutionName = "YourCompanyName";
barcodeXpress.Licensing.SolutionKey = Convert.ToInt64("12345678901234");
barcodeXpress.Licensing.UnlockRuntime("YourRuntimeKey", Convert.ToInt64("98765432109876"));
```
vs:
```csharp
// IronBarcode: one line
IronBarCode.License.LicenseKey = "YOUR-KEY";
```
2. Evaluation mode obscures results — contrast with IronBarcode trial (watermark only, full values returned)
3. 40 PPM throughput limit on Standard tier
4. Minimum 5 runtime licenses even for single-server use
5. Feature comparison table
6. Docker/CI implications of license files vs environment variable

**Migration Article Sections:**
- Step 1: `dotnet remove package Accusoft.BarcodeXpress.NetCore`
- Step 2: `dotnet add package IronBarcode`
- Step 3: Replace `Accusoft.BarcodeXpressSdk` namespace with `IronBarCode`; replace two-key init with `IronBarCode.License.LicenseKey = "key"`
- Code scenarios:
  - Reading: `_barcodeXpress.reader.SetPropertyValue(BarcodeXpress.cycBxeSetFilename, imagePath)` + `_barcodeXpress.reader.Analyze()` → `result.BarcodeValue` becomes `BarcodeReader.Read(imagePath).First().Value`
  - Format: `result.BarcodeType` → `result.Format`
  - Remove `ValidateLicense()` / `IsRuntimeUnlocked` checks entirely
- Migration checklist: grep for `BarcodeXpress`, `Licensing.SolutionName`, `UnlockRuntime`, `cycBxeSetFilename`

**Key API Mapping:**

| Accusoft BarcodeXpress | IronBarcode |
|---|---|
| `new BarcodeXpress()` | Static — no instance |
| `Licensing.SolutionName = "..."` | `IronBarCode.License.LicenseKey = "key"` |
| `Licensing.SolutionKey = longValue` | (removed) |
| `Licensing.UnlockRuntime(key, solutionKey)` | (removed) |
| `Licensing.IsRuntimeUnlocked` | (removed) |
| `reader.SetPropertyValue(BarcodeXpress.cycBxeSetFilename, path)` | `BarcodeReader.Read(path)` |
| `reader.Analyze()` | `BarcodeReader.Read(path)` |
| `result.BarcodeValue` | `result.Value` |
| `result.BarcodeType` | `result.Format` |

---

### 2. aspose-barcode

**Files:**
- `aspose-barcode/compare-aspose-barcode-vs-ironbarcode.md`
- `aspose-barcode/migrate-from-aspose-barcode-to-ironbarcode.md`

**NuGet to remove:** `Aspose.BarCode` (and `Aspose.PDF` if used for PDF workflows)
**Namespaces to remove:** `Aspose.BarCode.Generation`, `Aspose.BarCode.BarCodeRecognition`

**Core Pain Point:** Aspose.BarCode is subscription-only ($999–$4,995/yr, no perpetual option). Reading a barcode requires specifying the format in advance — `new BarCodeReader(filePath, DecodeType.Code128)`. If you don't know the format, you pass `DecodeType.AllSupportedTypes`, which is slower and not the default pattern. There is no native PDF support — reading barcodes from a PDF requires a separate `Aspose.PDF` license, adding another $999+ to the annual bill.

**Comparison Hook:** "Aspose.BarCode supports 60+ formats. But you have to tell it which one you're looking for, and if the barcode is in a PDF, you need a second Aspose subscription to open the file first."

**Comparison Article Sections:**
1. Format-specification burden — side-by-side code:
```csharp
// Aspose: must specify format upfront
var reader = new BarCodeReader(filePath, DecodeType.Code128);
reader.ReadBarCodes();
foreach (var result in reader.FoundBarCodes)
    Console.WriteLine(result.CodeText);
```
vs:
```csharp
// IronBarcode: auto-detection
var result = BarcodeReader.Read(filePath).First();
Console.WriteLine(result.Value); // format auto-detected
```
2. PDF scenario — Aspose needs two packages + two subscriptions vs IronBarcode's `BarcodeReader.Read("doc.pdf")`
3. Subscription-only pricing vs perpetual option
4. Generator verbosity: `new BarCodeGenerator(EncodeTypes.Code128, "data")` + `.Save("file.png", BarCodeImageFormat.Png)` vs `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128).SaveAsPng("file.png")`
5. Feature comparison table

**Migration Article Sections:**
- Step 1: `dotnet remove package Aspose.BarCode` (and `Aspose.PDF` if present)
- Step 2: `dotnet add package IronBarcode`
- Step 3: Replace namespaces; add `IronBarCode.License.LicenseKey = "key"`
- Code scenarios:
  - Generation: `new BarCodeGenerator(EncodeTypes.Code128, "data")` + `.Save(path, BarCodeImageFormat.Png)` → `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128).SaveAsPng(path)`
  - Reading: `new BarCodeReader(path, DecodeType.Code128)` + `.ReadBarCodes()` + `result.CodeText` → `BarcodeReader.Read(path).First().Value`
  - Remove `DecodeType` hardcoding throughout codebase
  - PDF workflow: remove Aspose.PDF rendering pipeline, replace with `BarcodeReader.Read("doc.pdf")`
- Migration checklist: grep for `BarCodeReader`, `BarCodeGenerator`, `DecodeType.`, `EncodeTypes.`, `BarCodeImageFormat`

**Key API Mapping:**

| Aspose.BarCode | IronBarcode |
|---|---|
| `new BarCodeGenerator(EncodeTypes.Code128, "data")` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)` |
| `generator.Save("file.png", BarCodeImageFormat.Png)` | `.SaveAsPng("file.png")` |
| `new BarCodeReader(path, DecodeType.Code128)` | `BarcodeReader.Read(path)` |
| `reader.ReadBarCodes()` | (part of `BarcodeReader.Read()`) |
| `result.CodeText` | `result.Value` |
| `result.CodeTypeName` | `result.Format` |
| Aspose.PDF + Aspose.BarCode for PDFs | `BarcodeReader.Read("doc.pdf")` — one package |

---

### 3. barcode4net

**Files:**
- `barcode4net/compare-barcode4net-vs-ironbarcode.md`
- `barcode4net/migrate-from-barcode4net-to-ironbarcode.md`

**NuGet to remove:** None (Barcode4NET has no NuGet package — it's a manual DLL reference)
**Reference to remove:** `<Reference Include="Barcode4NET"><HintPath>..\ThirdParty\Barcode4NET\Barcode4NET.dll</HintPath></Reference>`
**Namespace to remove:** `Barcode4NET`

**Core Pain Point:** Barcode4NET is end-of-life. New licenses are not available. The library has no NuGet package — it distributes as a manual DLL that must be committed to source control or artifact storage. It only supports .NET Framework (no .NET 5, 6, 7, 8, 9, Core, MAUI, Blazor, or Docker/Linux). It is generation-only with no reading capability.

**Comparison Hook:** "Barcode4NET has no NuGet package, no new license sales, and no .NET 6+ support. If you're still running it, you're not choosing to — you just haven't replaced it yet."

**Comparison Article Sections:**
1. EOL status stated plainly — this is not a feature comparison but a "here's the path forward" article
2. No NuGet: show the `.csproj` manual reference vs `<PackageReference Include="IronBarcode" Version="*" />`
3. Platform gap: .NET Framework only (no .NET 5+, no Docker, no Linux, no Azure Functions) vs IronBarcode's full platform coverage
4. Generation-only — no reading capability
5. CI/CD problem: pipeline must download or commit DLL vs `dotnet restore` handles everything
6. Feature comparison table: Barcode4NET last-release capabilities vs current IronBarcode

**Migration Article Sections:**
- Step 1: Delete `ThirdParty/Barcode4NET/` folder; remove `<Reference Include="Barcode4NET">` from `.csproj`; remove manual DLL copy steps from build scripts
- Step 2: `dotnet add package IronBarcode`
- Step 3: Replace `using Barcode4NET;` with `using IronBarCode;`; add license init
- Code scenarios (Barcode4NET last-known API → IronBarcode):
```csharp
// Barcode4NET (.NET Framework only, manual DLL)
var barcode = new Barcode4NET.Barcode();
barcode.Symbology = Symbology.Code128;
barcode.Data = "ITEM-12345";
barcode.Width = 300;
barcode.Height = 100;
Bitmap result = barcode.GenerateBarcode();
result.Save(outputPath);
```
→
```csharp
// IronBarcode (any .NET, NuGet)
BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .SaveAsPng(outputPath);
```
- WinForms migration: `pictureBoxBarcode.Image = barcode.GenerateBarcode()` → `Image.FromStream(new MemoryStream(BarcodeWriter.CreateBarcode(...).ToPngBinaryData()))`
- New capability unlocked: barcode reading, `BarcodeReader.Read(path)`
- Migration checklist: grep for `using Barcode4NET`, `new Barcode4NET.Barcode()`, `Symbology.Code`, `.GenerateBarcode()`

**Key API Mapping:**

| Barcode4NET | IronBarcode |
|---|---|
| `new Barcode4NET.Barcode()` | `BarcodeWriter.CreateBarcode(data, encoding)` |
| `barcode.Symbology = Symbology.Code128` | `BarcodeEncoding.Code128` (parameter) |
| `barcode.Data = "..."` | First parameter of `CreateBarcode` |
| `barcode.Width = 300; barcode.Height = 100` | `.ResizeTo(300, 100)` |
| `barcode.GenerateBarcode()` returns `Bitmap` | `.SaveAsPng()` / `.ToPngBinaryData()` |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` |
| No reading API | `BarcodeReader.Read(path)` |
| Manual DLL reference | `<PackageReference Include="IronBarcode" />` |

---

### 4. barcodelib

**Files:**
- `barcodelib/compare-barcodelib-vs-ironbarcode.md`
- `barcodelib/migrate-from-barcodelib-to-ironbarcode.md`

**NuGet to remove:** `BarcodeLib` or `BarcodeLib.Signed` or `BarcodeLib.SkiaSharp` (version-dependent)
**Namespace to remove:** `BarcodeLib`

**Core Pain Point:** BarcodeLib (Apache 2.0, 12M+ NuGet downloads) only generates barcodes — it has no reading capability. Later versions introduced SkiaSharp as a dependency, which conflicts with other SkiaSharp consumers in projects using MAUI, `SkiaSharp.Views.Maui`, or Blazor WASM tooling. The conflict manifests as `NU1608` warnings or runtime assembly binding failures.

**Comparison Hook:** "BarcodeLib has been downloaded 12 million times, which means 12 million developers eventually discover it can only create barcodes — not read them. Many discover the SkiaSharp conflict shortly after."

**Comparison Article Sections:**
1. Generation-only gap — feature table with every reading row blank for BarcodeLib
2. SkiaSharp conflict — show the `NU1608` / assembly binding issue when using `BarcodeLib` alongside `SkiaSharp.Views.Maui` or `Microsoft.Maui.Graphics`
3. Side-by-side generation code:
```csharp
// BarcodeLib
var b = new Barcode();
b.IncludeLabel = true;
Image img = b.Encode(TYPE.CODE128, "12345678");
img.Save("barcode.png", ImageFormat.Png);
```
vs:
```csharp
// IronBarcode
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");
```
4. Adding reading (new capability scenario)
5. Feature comparison table

**Migration Article Sections:**
- Step 1: `dotnet remove package BarcodeLib` (or `BarcodeLib.Signed` / `BarcodeLib.SkiaSharp`)
- Step 2: `dotnet add package IronBarcode`
- Step 3: Replace `using BarcodeLib;` with `using IronBarCode;`; add license init
- Code scenarios:
  - Generation: `new Barcode()` + `.Encode(TYPE.CODE128, "data")` → `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)`
  - Output: `.Encode()` returns `System.Drawing.Image` → `.SaveAsPng()` / `.ToPngBinaryData()`
  - Reading (net-new): `BarcodeReader.Read("barcode.png").First().Value`
  - Web API pattern: returning bytes
- Migration checklist: grep for `using BarcodeLib`, `TYPE.CODE128`, `b.Encode(`, `NU1608`

**Key API Mapping:**

| BarcodeLib | IronBarcode |
|---|---|
| `new Barcode()` | Static — no instance |
| `b.Encode(TYPE.CODE128, "data")` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)` |
| `b.IncludeLabel = true` | `.AddAnnotationTextBelowBarcode(text)` |
| Returns `System.Drawing.Image` | `.SaveAsPng()` / `.ToPngBinaryData()` |
| `TYPE.QRCode` | `BarcodeEncoding.QRCode` |
| No reading API | `BarcodeReader.Read(path)` |

---

### 5. barcoder

**Files:**
- `barcoder/compare-barcoder-vs-ironbarcode.md`
- `barcoder/migrate-from-barcoder-to-ironbarcode.md`

**NuGet to remove:** `Barcoder` + `Barcoder.Renderer.Image` (+ `Barcoder.Renderer.Svg` if used)
**Namespaces to remove:** `Barcoder`, `Barcoder.Code128`, `Barcoder.Qr`, `Barcoder.DataMatrix`, `Barcoder.Renderers` (and `Barcoder.Ean`, `Barcoder.Pdf417`, `Barcoder.Aztec` depending on formats used)

**Core Pain Point:** Barcoder requires at minimum two NuGet packages for basic PNG output (`Barcoder` + `Barcoder.Renderer.Image`), and each barcode format has its own namespace (`Barcoder.Code128`, `Barcoder.Qr`, `Barcoder.DataMatrix`, etc.). Encoding produces an `IBarcode` data structure that has no output methods — you must create a separate `ImageRenderer` with `ImageRendererOptions`, open a stream, and call `renderer.Render(barcode, stream)`. The image renderer dropped .NET Framework support. Barcoder has no reading capability.

**Comparison Hook:** "To generate a PNG barcode with Barcoder, you install two packages, import three namespaces, create an encoder, create a renderer with an options object, open a stream, render into it, and dispose the stream. IronBarcode is two lines."

**Comparison Article Sections:**
1. The multi-package install chain:
```bash
dotnet add package Barcoder
dotnet add package Barcoder.Renderer.Image
```
vs:
```bash
dotnet add package IronBarcode
```
2. Format-specific encoders + shared renderer pattern — show the full workflow:
```csharp
// Barcoder: 3 namespaces, 7 steps for one PNG
using Barcoder;
using Barcoder.Code128;
using Barcoder.Renderers;

IBarcode barcode = Code128Encoder.Encode("PRODUCT-12345");
var renderer = new ImageRenderer(new ImageRendererOptions
{
    ImageFormat = ImageFormat.Png,
    PixelSize = 2,
    BarHeightFor1DBarcode = 50
});
using var stream = File.OpenWrite("barcode.png");
renderer.Render(barcode, stream);
```
vs:
```csharp
// IronBarcode: 1 namespace, 1 line
using IronBarCode;
BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128).SaveAsPng("barcode.png");
```
3. Format-specific encoder namespace proliferation: `Barcoder.Code128`, `Barcoder.Qr`, `Barcoder.DataMatrix`, `Barcoder.Ean`, `Barcoder.Pdf417`, `Barcoder.Aztec` — one per symbology
4. `IBarcode` interface has no save/output methods — rendering requires the second package
5. Generation-only — no reading capability
6. Feature comparison table

**Migration Article Sections:**
- Step 1: `dotnet remove package Barcoder`, `dotnet remove package Barcoder.Renderer.Image` (and `.Svg`)
- Step 2: `dotnet add package IronBarcode`
- Step 3: Remove all `Barcoder.*` namespaces; replace with `using IronBarCode;`; add license init
- Code scenarios: format-specific encoder + renderer chain → `BarcodeWriter.CreateBarcode()` for each symbology
- Reading (net-new capability): `BarcodeReader.Read(path)`
- Version coordination: no more `Barcoder` + `Barcoder.Renderer.Image` version sync required
- Migration checklist: grep for `using Barcoder`, `Code128Encoder.Encode`, `QrEncoder.Encode`, `DataMatrixEncoder.Encode`, `ImageRenderer`, `IBarcode`

**Key API Mapping:**

| Barcoder | IronBarcode |
|---|---|
| `Code128Encoder.Encode("data")` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)` |
| `QrEncoder.Encode("data", ErrorCorrectionLevel.M)` | `QRCodeWriter.CreateQrCode("data", 500)` |
| `DataMatrixEncoder.Encode("data")` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.DataMatrix)` |
| `new ImageRenderer(new ImageRendererOptions {...})` | (not needed) |
| `renderer.Render(barcode, stream)` | `.SaveAsPng(path)` / `.ToPngBinaryData()` |
| `IBarcode` data structure | `GeneratedBarcode` returned by `CreateBarcode` |
| No reading API | `BarcodeReader.Read(path)` |

---

### 6. barcodescanning-maui

**Files:**
- `barcodescanning-maui/compare-barcodescanning-maui-vs-ironbarcode.md`
- `barcodescanning-maui/migrate-from-barcodescanning-maui-to-ironbarcode.md`

**NuGet:** `BarcodeScanning.Native.Maui`
**Namespace:** `BarcodeScanning`

**Core Pain Point:** BarcodeScanning.Native.Maui is a MAUI camera control (`CameraView`) that fires an `OnDetectionFinished` event when a barcode appears in the live camera frame. It has no API for reading from image files, byte arrays, streams, or PDFs. It does not support Windows MAUI targets. iOS has a known issue returning 13 digits for UPC-A instead of 12, requiring manual normalization. PDF417 scanning is documented in GitHub issues as "very problematic — most scans never occur." These are not edge cases; they're the library's design limits.

**Comparison Hook:** "BarcodeScanning.Native.Maui requires a live camera feed. There is no `ReadFromFile()` method. There is no PDF support. There is no Windows MAUI support. If any of those matter for your app, this is the wrong tool."

**Comparison Article Sections:**
1. Use-case scope clearly stated upfront — camera-only control vs file/PDF library
2. What BarcodeScanning.Native.Maui is actually doing — XAML CameraView, event-driven:
```xml
<scanner:CameraView x:Name="CameraView"
                    OnDetectionFinished="OnBarcodeDetected"
                    CameraEnabled="True" />
```
```csharp
private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArgs e)
{
    var barcode = e.BarcodeResults[0];
    ResultLabel.Text = barcode.DisplayValue; // iOS: may return 13 digits for UPC-A
}
```
3. Windows MAUI gap — not supported, not planned
4. iOS UPC-A 13-digit bug and the normalization workaround code it requires
5. PDF417 reliability ("most scans never occur" per GitHub issues)
6. Cross-platform inconsistencies: `barcode.BarcodeFormat` returns different enum values on iOS vs Android for the same physical barcode
7. IronBarcode for MAUI — capture photo with `MediaPicker`, process bytes:
```csharp
var photo = await MediaPicker.CapturePhotoAsync();
using var stream = await photo.OpenReadAsync();
using var ms = new MemoryStream();
await stream.CopyToAsync(ms);
var results = BarcodeReader.Read(ms.ToArray());
// Works on iOS, Android, AND Windows MAUI
```
8. Feature comparison table clearly separating camera-live vs file-processing use cases

**Migration Article Sections:**
- Framed as "extending your stack for file processing" or "adding Windows MAUI support"
- If fully replacing: `dotnet remove package BarcodeScanning.Native.Maui`; remove `CameraView` XAML; replace with `MediaPicker` + `BarcodeReader.Read()` pattern
- If adding alongside: keep BarcodeScanning.Native.Maui for mobile live camera; add IronBarcode for file/PDF/Windows processing
- Code scenarios: camera event handler → MediaPicker + BarcodeReader.Read, PDF processing (new capability), Windows MAUI support (new capability)
- Migration checklist: grep for `OnDetectionFinishedEventArgs`, `e.BarcodeResults`, `barcode.DisplayValue`, `CameraView`

**Key API Mapping:**

| BarcodeScanning.Native.Maui | IronBarcode |
|---|---|
| `CameraView` XAML control | No camera control — processes captured images |
| `OnDetectionFinished` event | `BarcodeReader.Read(imageBytes)` |
| `e.BarcodeResults[0].DisplayValue` | `result.Value` |
| `barcode.BarcodeFormat` | `result.Format` |
| iOS-only + Android-only | iOS, Android, Windows MAUI, macOS, ASP.NET |
| No file/PDF/byte-array API | `BarcodeReader.Read(path / bytes / stream / pdf)` |

---

### 7. barkoder-sdk

**Files:**
- `barkoder-sdk/compare-barkoder-sdk-vs-ironbarcode.md`
- `barkoder-sdk/migrate-from-barkoder-sdk-to-ironbarcode.md`

**NuGet to remove:** None (no official .NET package exists)
**Platform:** Native iOS (Swift/Objective-C) and Android (Kotlin/Java) SDKs only

**Core Pain Point:** Barkoder SDK has no .NET library. It appears in barcode SDK comparison articles and roundups, but its distribution is native iOS and Android only. .NET developers have no official path to use it. Community Xamarin/MAUI binding attempts exist but are not production-supported by Barkoder.

**Comparison Hook:** "Barkoder shows up in every 'best barcode SDK' comparison. There's no NuGet package. There's no .NET API. If you're writing C#, Barkoder is not an option."

**Comparison Article Sections:**
1. State the situation clearly — no .NET SDK, no NuGet, no official C# support
2. What Barkoder is praised for (accuracy, ML correction, format breadth) — acknowledge its strengths honestly
3. What .NET developers need instead — IronBarcode as the equivalent for managed code
4. Side-by-side: what Barkoder does for iOS/Android (conceptual) vs IronBarcode for .NET
5. IronBarcode capabilities that match Barkoder's strengths: ML error correction, 50+ formats, PDF processing
6. Feature comparison table: Barkoder platform coverage vs IronBarcode platform coverage

**Migration Article Sections:**
- Framed as "starting with IronBarcode if you evaluated Barkoder and needed .NET"
- Not a code migration (no Barkoder .NET code exists)
- Show what Barkoder use cases look like in IronBarcode:
  - Damaged barcode correction: `BarcodeReader.Read("damaged.png")` with ML handling
  - Multi-format batch: `BarcodeReader.Read(fileArray, options)` with `ExpectMultipleBarcodes = true`
  - PDF document scanning: `BarcodeReader.Read("invoice.pdf")`
- License setup: `IronBarCode.License.LicenseKey = "key"` — no device UUID, no native binding

---

### 8. cloudmersive-barcode

**Files:**
- `cloudmersive-barcode/compare-cloudmersive-barcode-vs-ironbarcode.md`
- `cloudmersive-barcode/migrate-from-cloudmersive-barcode-to-ironbarcode.md`

**NuGet to remove:** `Cloudmersive.APIClient.NETCore.Barcode`
**Namespace to remove:** `Cloudmersive.APIClient.NETCore.Barcode.Api`, `Cloudmersive.APIClient.NETCore.Barcode.Client`

**Core Pain Point:** Cloudmersive is a REST API — every barcode read and write is an HTTPS request to their servers. Per-request pricing accumulates quickly at scale. Round-trip latency is 100–500ms per call. Images and document data are transmitted to Cloudmersive's infrastructure, creating compliance blockers for HIPAA, GDPR, ITAR, and air-gapped environments. Rate limits apply. API keys expire. When their service has an outage, your barcode processing stops.

**Comparison Hook:** "At 10,000 barcodes a day — a realistic number for invoice processing — Cloudmersive costs $3,650 a year. Plus 100–500ms per barcode. Plus every barcode leaving your network."

**Comparison Article Sections:**
1. Cost math upfront — $0.001/request × 10K/day × 365 = $3,650/yr vs $749 IronBarcode perpetual
2. Latency comparison: 100–500ms REST round-trip vs 10–50ms local
3. Data sovereignty: every image/barcode leaves your infrastructure to Cloudmersive servers
4. Compliance table: HIPAA, GDPR, ITAR, CMMC, FedRAMP, air-gapped — all blocked by cloud transmission
5. Reliability: API outage = barcode outage
6. Side-by-side code — HTTP client pattern vs local NuGet:
```csharp
// Cloudmersive: HTTP client, API key, per-request cost
var apiInstance = new BarCodeLookupApi();
var result = apiInstance.BarCodeLookupEanLookup("0123456789012");
```
vs:
```csharp
// IronBarcode: local, no network
var result = BarcodeReader.Read("barcode.png").First();
Console.WriteLine(result.Value);
```
7. Feature comparison table

**Migration Article Sections:**
- Step 1: `dotnet remove package Cloudmersive.APIClient.NETCore.Barcode`
- Step 2: `dotnet add package IronBarcode`
- Step 3: Remove `Cloudmersive.APIClient.NETCore.Barcode.Api` imports; add `using IronBarCode;`; add license init
- Code scenarios:
  - QR generation: `api.BarcodeQRCodePost(value)` → `BarcodeWriter.CreateBarcode(value, BarcodeEncoding.QRCode).ToPngBinaryData()`
  - Barcode reading: `api.BarcodeImageRecognitionPost(imageFile)` → `BarcodeReader.Read(imageBytes).First().Value`
  - Remove `Configuration.Default.ApiKey["Apikey"] = "..."`
- Cost calculation in the checklist: how many calls/month × rate = how quickly IronBarcode pays for itself
- Migration checklist: grep for `Cloudmersive`, `ApiKey\["Apikey"\]`, `BarCodeLookupApi`, `BarcodeQRCodePost`

**Key API Mapping:**

| Cloudmersive | IronBarcode |
|---|---|
| `Configuration.Default.ApiKey["Apikey"] = "..."` | `IronBarCode.License.LicenseKey = "key"` |
| `new BarCodeLookupApi()` | Static — no instance |
| `api.BarcodeQRCodePost(value)` | `BarcodeWriter.CreateBarcode(value, BarcodeEncoding.QRCode)` |
| `api.BarcodeImageRecognitionPost(imageFile)` | `BarcodeReader.Read(imageBytes)` |
| HTTP response parsing | `.Value`, `.Format` properties |
| Network latency 100–500ms | Local: 10–50ms |
| Data leaves your network | Fully local, no transmission |

---

### 9. devexpress-barcode

**Files:**
- `devexpress-barcode/compare-devexpress-barcode-vs-ironbarcode.md`
- `devexpress-barcode/migrate-from-devexpress-barcode-to-ironbarcode.md`

**NuGet for WinForms:** `DevExpress.Win.Core` (barcode is part of `DevExpress.XtraBars.BarCode`)
**NuGet for Blazor:** `DevExpress.Blazor`
**Namespace:** `DevExpress.XtraBars.BarCode`, `DevExpress.XtraBars.BarCode.Symbologies`

**Core Pain Point:** DevExpress barcode components are UI controls, not a library. The WinForms `BarCodeControl` and Blazor `DxBarCode` are designed for rendering barcodes visually in a UI — they are generation-only. DevExpress has publicly stated in their support center: "DevExpress does not provide barcode recognition/scanning functionality." The components are bundled in the DXperience suite; there is no standalone barcode package. File output from `BarCodeControl` requires `DrawToBitmap()` or a `PrintingSystem` export — neither is straightforward. No server-side/console/Azure Function usage is supported.

**Comparison Hook:** "DevExpress's barcode support is a `BarCodeControl` you drop on a form. DevExpress support has confirmed they have no plans to add reading. If your barcode lives in a background job or a web API, the control doesn't help."

**Comparison Article Sections:**
1. UI control vs library — what the DevExpress barcode actually is
2. The generation verbosity:
```csharp
// DevExpress WinForms — 8+ lines, UI context required
using DevExpress.XtraBars.BarCode;
using DevExpress.XtraBars.BarCode.Symbologies;

var barCode = new BarCodeControl();
var symbology = new Code128Generator();
symbology.CharacterSet = Code128CharacterSet.CharsetAuto;
barCode.Symbology = symbology;
barCode.Text = "ITEM-12345";
barCode.Module = 0.02f;
barCode.ShowText = true;
// Then awkward DrawToBitmap or PrintingSystem export for file output
```
vs:
```csharp
// IronBarcode — 1 line, works anywhere
BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128).SaveAsPng("barcode.png");
```
3. The Module property DPI calculation complexity (document units, not pixels)
4. No reading capability — DevExpress support quotes from their public support center
5. Suite bundling: DevExpress barcode is only available as part of the DXperience suite (~$2,499+/yr for WinForms), no standalone package
6. Headless/server use: `BarCodeControl` requires UI assembly; cannot run in console or ASP.NET Core minimal API without assembly loading workarounds
7. Feature comparison table

**Migration Article Sections:**
- Step 1: DevExpress is kept if used for other UI controls; replace barcode-specific code with IronBarcode
- Step 2: `dotnet add package IronBarcode`
- Step 3: Add `using IronBarCode;`; add license init
- Code scenarios:
  - Generation: `BarCodeControl` + `Code128Generator` → `BarcodeWriter.CreateBarcode()`
  - QR: `QRCodeGenerator` with `ErrorCorrectionLevel.H` → `QRCodeWriter.CreateQrCode("data", 500, QRCodeWriter.QrErrorCorrectionLevel.Highest)`
  - File output: `DrawToBitmap` workaround → `.SaveAsPng()`
  - Reading (net-new): `BarcodeReader.Read()`
  - Web API pattern: returning `ToPngBinaryData()` as file response
- Migration checklist: grep for `BarCodeControl`, `Code128Generator`, `QRCodeGenerator`, `DataMatrixGenerator`, `PDF417Generator`, `barCode.Symbology`, `barCode.Module`

**Key API Mapping:**

| DevExpress | IronBarcode |
|---|---|
| `new BarCodeControl()` | Static — no instance |
| `new Code128Generator()` + `barCode.Symbology = symbology` | `BarcodeEncoding.Code128` (parameter) |
| `new QRCodeGenerator()` + `QRCodeErrorCorrectionLevel.H` | `QRCodeWriter.CreateQrCode("data", 500, QRCodeWriter.QrErrorCorrectionLevel.Highest)` |
| `new DataMatrixGenerator()` + `DataMatrixSize.Matrix26x26` | `BarcodeEncoding.DataMatrix` (auto-sizes) |
| `new PDF417Generator()` | `BarcodeEncoding.PDF417` |
| `barCode.Module = 0.02f` (document units) | `.ResizeTo(width, height)` (pixels) |
| `DrawToBitmap(bitmap, rect)` for file output | `.SaveAsPng(path)` |
| No reading API | `BarcodeReader.Read(path)` |

---

### 10. dynamsoft-barcode

**Files:**
- `dynamsoft-barcode/compare-dynamsoft-barcode-vs-ironbarcode.md`
- `dynamsoft-barcode/migrate-from-dynamsoft-barcode-to-ironbarcode.md`

**NuGet to remove:** `Dynamsoft.DotNet.BarcodeReader`
**Namespace to remove:** `Dynamsoft.DBR`

**Core Pain Point:** Dynamsoft Barcode Reader is a camera-first, real-time video scanning SDK. Its license requires an online validation call (`BarcodeReader.InitLicense(key, out errorMsg)`) that contacts Dynamsoft's license server — which fails in air-gapped deployments and adds startup latency in CI/CD. Offline licensing requires contacting Dynamsoft support, obtaining a device-specific license file, and providing a device UUID (`BarcodeReader.OutputLicenseToString()`). There is no native PDF support — PDFs must be rendered page-by-page via an external library (e.g. PdfiumViewer) before passing images to Dynamsoft.

**Comparison Hook:** "Dynamsoft is exceptional at reading barcodes from a 30fps camera feed. If your barcodes are in PDF files on a server that can't reach the internet, it's the wrong tool — and the license validation will remind you on every startup."

**Comparison Article Sections:**
1. Acknowledge Dynamsoft's camera/mobile strengths genuinely — it is the best in its class for live camera scanning
2. The architectural mismatch for server/document use cases
3. License initialization complexity:
```csharp
// Dynamsoft: may throw, requires network, returns error codes
int errorCode = BarcodeReader.InitLicense(licenseKey, out string errorMsg);
if (errorCode != (int)EnumErrorCode.DBR_OK)
    throw new InvalidOperationException($"License error: {errorMsg}");
var reader = new BarcodeReader();
```
vs:
```csharp
// IronBarcode: local, no network, no error codes
IronBarCode.License.LicenseKey = "YOUR-KEY";
```
4. No native PDF — show the PdfiumViewer render loop workaround required
5. Offline licensing: `reader.InitLicenseFromLicenseContent(licenseContent, deviceUuid)` + `BarcodeReader.OutputLicenseToString()` for UUID — per-device, must request from Dynamsoft support
6. Camera API concepts that don't map to file processing: `DecodeBuffer`, `EnumImagePixelFormat.IPF_RGB_888`, `PublicRuntimeSettings.Timeout`
7. Feature comparison table with clear "camera" vs "document" separation

**Migration Article Sections:**
- Step 1: `dotnet remove package Dynamsoft.DotNet.BarcodeReader`
- Step 2: `dotnet add package IronBarcode`
- Step 3: Replace `Dynamsoft.DBR` namespace; replace `BarcodeReader.InitLicense()` + error handling with `IronBarCode.License.LicenseKey = "key"`
- Code scenarios:
  - File reading: `_reader.DecodeFile(imagePath, "")` → `TextResult[]` → `.BarcodeText` becomes `BarcodeReader.Read(imagePath)` → `.Value`
  - Camera buffer reading: `_reader.DecodeBuffer(frameData, width, height, stride, IPF_RGB_888, "")` — for server use cases, replace with `BarcodeReader.Read(imageBytes)`
  - PDF: remove PdfiumViewer render loop; replace with `BarcodeReader.Read("document.pdf")`
  - Remove `_reader.Dispose()` — IronBarcode is static, no instance management
  - Remove `PublicRuntimeSettings` configuration — IronBarcode handles automatically via `BarcodeReaderOptions.Speed`
- Migration checklist: grep for `using Dynamsoft.DBR`, `BarcodeReader.InitLicense`, `_reader.DecodeFile`, `TextResult[]`, `BarcodeText`, `PublicRuntimeSettings`, `OutputLicenseToString`

**Key API Mapping:**

| Dynamsoft | IronBarcode |
|---|---|
| `BarcodeReader.InitLicense(key, out errorMsg)` | `IronBarCode.License.LicenseKey = "key"` |
| `new BarcodeReader()` | Static — no instance |
| `_reader.DecodeFile(imagePath, "")` | `BarcodeReader.Read(imagePath)` |
| `_reader.DecodeBuffer(frameData, width, height, ...)` | `BarcodeReader.Read(imageBytes)` |
| `TextResult[].BarcodeText` | `result.Value` |
| `result.BarcodeFormat` | `result.Format` |
| External PDF library + render loop required | `BarcodeReader.Read("doc.pdf")` native |
| `_reader.Dispose()` | Not needed |
| `BarcodeReader.OutputLicenseToString()` for device UUID | Not needed |
| `reader.InitLicenseFromLicenseContent(content, uuid)` | Not needed |

---

### 11. google-mlkit-barcode

**Files:**
- `google-mlkit-barcode/compare-google-mlkit-barcode-vs-ironbarcode.md`
- `google-mlkit-barcode/migrate-from-google-mlkit-barcode-to-ironbarcode.md`

**NuGet:** None (no official .NET package — native Android: `com.google.mlkit:barcode-scanning`, native iOS: `GoogleMLKit/BarcodeScanning`)
**Platform:** Native Android (Kotlin/Java) and iOS (Swift) only

**Core Pain Point:** Google ML Kit Barcode Scanning has no .NET SDK. It is distributed as a native Android library via Google Maven and a native iOS pod via CocoaPods. The only .NET path is Xamarin/MAUI native bindings, which are community-maintained and require Firebase/Google Play Services on Android. ML Kit is camera-frame focused and has no file/PDF processing API.

**Comparison Hook:** "ML Kit's barcode scanner is one of the best on Android. There is no NuGet package. You're either writing Kotlin, or you're writing Xamarin binding wrappers that break on SDK updates."

**Comparison Article Sections:**
1. State clearly: no official .NET SDK, no NuGet, no C# API
2. What ML Kit Barcode Scanning is — native mobile, camera/image frames, Firebase dependency on Android
3. What the Xamarin/MAUI binding path looks like (complex, community-maintained, not production-reliable)
4. ML Kit barcode formats vs IronBarcode format coverage — they overlap heavily (QR, EAN-13, Code128, DataMatrix, PDF417, Aztec all supported by both)
5. IronBarcode capabilities that exceed ML Kit for .NET: PDF processing, generation, batch server processing, Windows support
6. Feature comparison table: ML Kit platform column (Android native, iOS native) vs IronBarcode (.NET everywhere)

**Migration Article Sections:**
- Framed as "porting Android app barcode logic to .NET"
- Show what ML Kit code looks like (Kotlin) and the IronBarcode C# equivalent:
```kotlin
// Android Kotlin: ML Kit
val options = BarcodeScannerOptions.Builder()
    .setBarcodeFormats(Barcode.FORMAT_QR_CODE, Barcode.FORMAT_CODE_128)
    .build()
val scanner = BarcodeScanning.getClient(options)
scanner.process(inputImage)
    .addOnSuccessListener { barcodes ->
        for (barcode in barcodes) {
            val rawValue = barcode.rawValue
        }
    }
```
vs:
```csharp
// .NET C#: IronBarcode
var results = BarcodeReader.Read("captured-image.jpg");
foreach (var barcode in results)
    Console.WriteLine(barcode.Value);
```
- No Google Play Services dependency in .NET server code
- No Firebase setup required
- PDF processing (not possible in ML Kit at all)
- Migration checklist: grep for `BarcodeScanning.getClient`, `addOnSuccessListener`, `barcode.rawValue` in any Kotlin/Java files being ported

---

### 12. grapecity-barcode

**Files:**
- `grapecity-barcode/compare-grapecity-barcode-vs-ironbarcode.md`
- `grapecity-barcode/migrate-from-grapecity-barcode-to-ironbarcode.md`

**NuGet to remove:** `C1.Win.C1BarCode`
**Namespace to remove:** `C1.Win.C1BarCode`
**License init to remove:** `C1.C1License.Key = "..."`

**Core Pain Point:** GrapeCity barcode controls (ComponentOne) are UI controls bundled in the ComponentOne Studio Enterprise suite (~$1,473/developer/year subscription). The WinForms control (`C1BarCode`) is generation-only — there is no reading API. The suite requires `net8.0-windows` target framework and `UseWindowsForms = true`, preventing cross-platform deployment. No Linux, no macOS, no Docker Linux containers.

**Comparison Hook:** "ComponentOne's barcode control generates barcodes inside a Windows Forms app. It does this well. But there's no reading API, no cross-platform support, and you're paying for 100+ controls you didn't ask for."

**Comparison Article Sections:**
1. Suite bundling cost — ComponentOne Studio Enterprise includes 100+ components; show the list and highlight the barcode control as ~1% of the suite
2. Generation code:
```csharp
// ComponentOne C1BarCode
using C1.Win.C1BarCode;

var barcode = new C1BarCode();
barcode.CodeType = CodeType.Code128;
barcode.Text = "ITEM-12345";
barcode.BarHeight = 100;
barcode.ModuleSize = 2;
barcode.ShowText = true;
barcode.CaptionPosition = CaptionPosition.Below;
using var image = barcode.GetImage();
image.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
```
vs:
```csharp
// IronBarcode
BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
    .SaveAsPng(outputPath);
```
3. QR code color customization comparison:
```csharp
// ComponentOne: 12 lines including QRCodeVersion, QRCodeErrorCorrectionLevel, QRCodeModel
barcode.QRCodeVersion = QRCodeVersion.Version5;
barcode.QRCodeErrorCorrectionLevel = QRCodeErrorCorrectionLevel.High;
barcode.QRCodeModel = QRCodeModel.Model2;
barcode.ForeColor = Color.DarkBlue;
```
vs:
```csharp
// IronBarcode: 3 lines
QRCodeWriter.CreateQrCode("data", 300)
    .ChangeBarCodeColor(Color.DarkBlue)
    .SaveAsPng("qr.png");
```
4. No reading API — `throw new NotSupportedException(...)` in any attempt to read
5. Windows-only deployment: `net8.0-windows`, `UseWindowsForms = true`, `System.Drawing.Common` dependency
6. Feature comparison table

**Migration Article Sections:**
- Step 1: `dotnet remove package C1.Win.C1BarCode`; remove `C1.C1License.Key = "..."` from startup
- Step 2: `dotnet add package IronBarcode`; change `<TargetFramework>net8.0-windows</TargetFramework>` to `net8.0` if cross-platform deployment is needed
- Step 3: Replace `using C1.Win.C1BarCode;` with `using IronBarCode;`; add license init
- Code scenarios: `C1BarCode` generation → `BarcodeWriter.CreateBarcode()`, `barcode.GetImage()` → `.ToPngBinaryData()`, reading (net-new capability)
- Migration checklist: grep for `using C1.Win.C1BarCode`, `new C1BarCode()`, `CodeType.`, `C1.C1License.Key`, `.GetImage()`

**Key API Mapping:**

| ComponentOne C1BarCode | IronBarcode |
|---|---|
| `C1.C1License.Key = "..."` | `IronBarCode.License.LicenseKey = "key"` |
| `new C1BarCode()` | Static — no instance |
| `barcode.CodeType = CodeType.Code128` | `BarcodeEncoding.Code128` (parameter) |
| `barcode.Text = "data"` | First parameter of `CreateBarcode` |
| `barcode.BarHeight = 100` | `.ResizeTo(width, 100)` |
| `barcode.ModuleSize = 2` | `.ResizeTo()` handles sizing |
| `barcode.ForeColor = Color.DarkBlue` | `.ChangeBarCodeColor(Color.DarkBlue)` |
| `barcode.GetImage()` returns `Image` | `.SaveAsPng()` / `.ToPngBinaryData()` |
| No reading API | `BarcodeReader.Read(path)` |

---

### 13. infragistics-barcode

**Files:**
- `infragistics-barcode/compare-infragistics-barcode-vs-ironbarcode.md`
- `infragistics-barcode/migrate-from-infragistics-barcode-to-ironbarcode.md`

**NuGet to remove:** `Infragistics.Win.UltraWinBarcode` (WinForms generation) and/or `Infragistics.WPF.BarcodeReader` (WPF reading)
**Namespaces to remove:** `Infragistics.Win.UltraWinBarcode` (WinForms), `Infragistics.Controls.Barcodes` (WPF)

**Core Pain Point:** Infragistics barcode support is split across two separate packages serving different UI frameworks. WinForms uses `UltraWinBarcode` for generation only — no reading API exists. WPF uses `BarcodeReader` (separate assembly) for reading, which uses an event-driven pattern (`DecodeComplete` callback) requiring `TaskCompletionSource` wiring for async/await use, and mandates explicit `SymbologyType` specification (no auto-detection). ASP.NET Core, console apps, Azure Functions, and Docker have no Infragistics barcode support at all. All of this requires an Infragistics Ultimate subscription at $1,675+/year.

**Comparison Hook:** "Infragistics barcode reading works in WPF. It requires an event handler, a TaskCompletionSource, a BitmapSource load, and explicit symbology flags. It doesn't work in WinForms. It doesn't work in ASP.NET Core. It's one platform, one direction, 40+ lines."

**Comparison Article Sections:**
1. The split-component architecture — two packages, two frameworks, neither complete
2. Platform support matrix (concrete):
   - WinForms: generation via `UltraWinBarcode` only, no reading
   - WPF: generation via `XamBarcode` + reading via `BarcodeReader` (event-driven)
   - ASP.NET Core: no barcode support at all
   - Console/Docker: no barcode support at all
3. The event-driven reading complexity — show full WPF reading pattern:
```csharp
// Infragistics WPF: 40+ lines for a basic read
_reader = new BarcodeReader();
_reader.DecodeComplete += OnDecodeComplete;

public async Task<string> ReadBarcodeAsync(string imagePath)
{
    _result = new TaskCompletionSource<string>();
    var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
    _reader.SymbologyTypes = SymbologyType.Code128 | SymbologyType.QR |
                             SymbologyType.EAN13 | SymbologyType.DataMatrix |
                             SymbologyType.Interleaved2of5;
    _reader.Decode(bitmap);
    return await _result.Task;
}

private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
{
    _result?.TrySetResult(e.SymbologyValue);
}
```
vs:
```csharp
// IronBarcode: 2 lines, any platform
var results = BarcodeReader.Read(imagePath);
return results.FirstOrDefault()?.Value ?? "No barcode found";
```
4. WinForms generation:
```csharp
// Infragistics WinForms generation
using Infragistics.Win.UltraWinBarcode;
var barcode = new UltraWinBarcode();
barcode.Symbology = Symbology.Code128;
barcode.Data = "ITEM-12345";
barcode.SaveTo(outputPath);
```
5. Symbology specification — must manually OR together every format or miss barcodes
6. WPF-only `BitmapImage` dependency for reading — not portable to other platforms
7. Feature comparison table showing the platform/capability matrix

**Migration Article Sections:**
- Step 1: `dotnet remove package Infragistics.Win.UltraWinBarcode` and/or `Infragistics.WPF.BarcodeReader`
- Step 2: `dotnet add package IronBarcode`
- Step 3: Remove `Infragistics.Win.UltraWinBarcode` / `Infragistics.Controls.Barcodes` namespaces; add `using IronBarCode;`; add license init
- Code scenarios:
  - WinForms generation: `new UltraWinBarcode()` + `barcode.SaveTo(path)` → `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128).SaveAsPng(path)`
  - WPF reading: remove event handler pattern entirely; replace with `BarcodeReader.Read(imagePath).FirstOrDefault()?.Value`
  - Remove `TaskCompletionSource` async wrapper
  - Remove `SymbologyType` flag configuration
  - Remove `BitmapImage` loading — IronBarcode accepts file paths directly
  - `_reader.DecodeComplete -= OnDecodeComplete` cleanup — not needed
  - Batch processing: sequential Infragistics loop → `BarcodeReader.Read(fileArray)` with internal parallelization
- Migration checklist: grep for `UltraWinBarcode`, `Symbology.Code128`, `barcode.SaveTo(`, `BarcodeReader` (Infragistics), `DecodeComplete`, `ReaderDecodeArgs`, `SymbologyType.`, `SymbologyValue`, `BitmapImage`

**Key API Mapping:**

| Infragistics | IronBarcode |
|---|---|
| `new UltraWinBarcode()` (WinForms) | Static — `BarcodeWriter.CreateBarcode()` |
| `barcode.Symbology = Symbology.Code128` | `BarcodeEncoding.Code128` (parameter) |
| `barcode.Data = "..."` | First parameter of `CreateBarcode` |
| `barcode.SaveTo(outputPath)` | `.SaveAsPng(outputPath)` |
| `new BarcodeReader()` (WPF) | Static — `BarcodeReader.Read()` |
| `_reader.DecodeComplete += handler` | (not needed) |
| `_reader.SymbologyTypes = SymbologyType.Code128 | SymbologyType.QR | ...` | Auto-detection — no specification needed |
| `new BitmapImage(new Uri(path))` + `_reader.Decode(bitmap)` | `BarcodeReader.Read(path)` |
| `e.SymbologyValue` (in callback) | `result.Value` |
| `e.Symbology` (in callback) | `result.Format` |
| WPF-only reading | Reads on any .NET platform |

---

## Execution Order

Recommended order for maximum article angle diversity (no two consecutive articles share the same hook type):

1. `cloudmersive-barcode` — cost/cloud removal (most concrete ROI story)
2. `barcodelib` — generation-only gap
3. `aspose-barcode` — subscription pricing + format verbosity
4. `accusoft-barcode` — licensing complexity
5. `devexpress-barcode` — suite bundling + UI control
6. `dynamsoft-barcode` — use-case mismatch (camera vs document)
7. `infragistics-barcode` — split-component architecture + event API complexity
8. `grapecity-barcode` — suite bundling + Windows-only deployment
9. `barcoder` — multi-package fragmentation
10. `barcode4net` — EOL/rescue migration
11. `barcodescanning-maui` — mobile scope mismatch
12. `barkoder-sdk` — no .NET SDK
13. `google-mlkit-barcode` — no .NET SDK (different platform context)

## Output Directory Structure

```
accusoft-barcode/
  compare-accusoft-barcode-vs-ironbarcode.md
  migrate-from-accusoft-barcode-to-ironbarcode.md

aspose-barcode/
  compare-aspose-barcode-vs-ironbarcode.md
  migrate-from-aspose-barcode-to-ironbarcode.md

barcode4net/
  compare-barcode4net-vs-ironbarcode.md
  migrate-from-barcode4net-to-ironbarcode.md

barcodelib/
  compare-barcodelib-vs-ironbarcode.md
  migrate-from-barcodelib-to-ironbarcode.md

barcoder/
  compare-barcoder-vs-ironbarcode.md
  migrate-from-barcoder-to-ironbarcode.md

barcodescanning-maui/
  compare-barcodescanning-maui-vs-ironbarcode.md
  migrate-from-barcodescanning-maui-to-ironbarcode.md

barkoder-sdk/
  compare-barkoder-sdk-vs-ironbarcode.md
  migrate-from-barkoder-sdk-to-ironbarcode.md

cloudmersive-barcode/
  compare-cloudmersive-barcode-vs-ironbarcode.md
  migrate-from-cloudmersive-barcode-to-ironbarcode.md

devexpress-barcode/
  compare-devexpress-barcode-vs-ironbarcode.md
  migrate-from-devexpress-barcode-to-ironbarcode.md

dynamsoft-barcode/
  compare-dynamsoft-barcode-vs-ironbarcode.md
  migrate-from-dynamsoft-barcode-to-ironbarcode.md

google-mlkit-barcode/
  compare-google-mlkit-barcode-vs-ironbarcode.md
  migrate-from-google-mlkit-barcode-to-ironbarcode.md

grapecity-barcode/
  compare-grapecity-barcode-vs-ironbarcode.md
  migrate-from-grapecity-barcode-to-ironbarcode.md

infragistics-barcode/
  compare-infragistics-barcode-vs-ironbarcode.md
  migrate-from-infragistics-barcode-to-ironbarcode.md
```

## Quality Checklist (Per Article)

- [ ] Opens with a specific, non-generic hook about that competitor's actual documented limitation
- [ ] All code blocks use current IronBarcode API with correct class/method names
- [ ] Code blocks include `// NuGet: dotnet add package IronBarcode` comment where appropriate
- [ ] Feature comparison table includes at least 10 rows covering all major barcode scenarios
- [ ] Migration article includes exact NuGet package name(s) for the removal step
- [ ] API mapping table uses competitor API names confirmed from their CS source files
- [ ] Conclusion circles back to the specific hook — does not end with generic praise
- [ ] No two articles use the same opening sentence structure
- [ ] No article is a template fill-in — each reflects the competitor's unique situation
