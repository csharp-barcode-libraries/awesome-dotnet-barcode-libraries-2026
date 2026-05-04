# Migrating from Barkoder SDK to IronBarcode

Barkoder does have a .NET surface — `Plugin.Maui.Barkoder` and `Barkoder.Xamarin` are real, current NuGet packages, both maintained by barKoder. What they do not give you is a general-purpose .NET barcode library. They wrap the native Android and iOS Barkoder SDKs and surface a `BarkoderView` camera control inside MAUI or Xamarin mobile apps. They do not read from files, streams, byte arrays, or PDFs. They do not run on a server. They do not generate barcodes. If you arrived at Barkoder for any of those use cases — backend processing, document workflows, label generation, batch decoding from disk — you have hit the wall this guide is written for. IronBarcode covers each of those scenarios in .NET, with working code for every workflow that brought you to Barkoder in the first place.

## Why Migrate from Barkoder SDK

Teams moving from a Barkoder evaluation to IronBarcode consistently report the same triggers:

**Mobile-Only Scope:** `Plugin.Maui.Barkoder` and `Barkoder.Xamarin` exist on NuGet, but they are camera-scanning components for MAUI and Xamarin mobile apps. They have no equivalent for ASP.NET Core, Azure Functions, Docker, console applications, worker services, WPF, WinForms, Avalonia, or Blazor. For any non-mobile .NET workload, Barkoder is unavailable regardless of the NuGet listings.

**Read-Only Architecture:** Barkoder is a scanning-only SDK. If your application needs to generate barcodes — print shipping labels, create QR codes for packaging, embed codes in documents — Barkoder provides nothing. Teams that discover a generation requirement mid-project need a library that covers both sides of the workflow.

**No File or PDF Input:** Barkoder consumes live camera frames. There is no API for handing it a path, a `Stream`, or a `byte[]`. In .NET enterprise applications, barcodes frequently appear in PDF documents — invoices, shipping manifests, medical records, compliance documents — and Barkoder has no concept of this workflow.

**No Server-Side Deployment:** Barkoder's processing model is on-device mobile scanning. It cannot run in ASP.NET Core, Azure Functions, Docker containers, or AWS Lambda. Teams building server-side barcode APIs, document processing pipelines, or cloud functions have no path to a Barkoder deployment.

**Subscription Licensing Per App and Per Device:** Barkoder's enterprise pricing tiers are annual subscriptions tiered by device count — at the time of writing, EUR 1,250 / 50 devices, EUR 1,999 / 100 devices, and EUR 2,999 / 250 devices, each covering a single app. Teams that need to ship to many apps, many devices, or any non-mobile context find the cost model misaligned with their use case. (Confirm current pricing on barKoder's pricing page before quoting.)

### The Fundamental Problem

Barkoder is a camera-UI component, not a barcode library. A .NET developer cannot write any of the following with Barkoder:

```csharp
// None of these are possible with Plugin.Maui.Barkoder or Barkoder.Xamarin:
var fromFile     = BarcodeReader.Read("scan.png");        // file input
var fromStream   = BarcodeReader.Read(httpRequestStream); // upload
var fromPdf      = BarcodeReader.Read("invoice.pdf");     // PDF input
BarcodeWriter.CreateBarcode("X", BarcodeEncoding.Code128) // generation
    .SaveAsPng("label.png");
```

IronBarcode provides a native .NET API for every one of these:

```csharp
// IronBarcode — installs from NuGet, runs in any .NET project
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.ExtremeDetail,  // Equivalent to DeBlur mode
    ExpectMultipleBarcodes = false,
};

var results = BarcodeReader.Read("scanned-label.png", options);
foreach (var result in results)
{
    Console.WriteLine($"Type: {result.BarcodeType} | Value: {result.Value}");
}
```

## IronBarcode vs Barkoder SDK: Feature Comparison

| Feature | Barkoder SDK | IronBarcode |
|---|---|---|
| **.NET / C# API** | MAUI + Xamarin only (camera UI) | Full, all .NET project types |
| **NuGet Packages** | `Plugin.Maui.Barkoder`, `Barkoder.Xamarin` | `BarCode` |
| **Damaged Barcode Recovery** | MatrixSight / DeBlur | `ReadingSpeed.ExtremeDetail` + ML |
| **Multi-Barcode Detection** | Yes (Batch MultiScan) | `ExpectMultipleBarcodes = true` |
| **DataMatrix / DPM** | Yes (read only) | Yes — read and generate |
| **QR Code** | Yes (read only) | Yes — read and generate |
| **PDF417** | Yes | Yes |
| **Aztec** | Yes | Yes |
| **Code128, Code39, EAN** | Yes | Yes — 50+ formats |
| **Barcode Generation** | None | Full — all formats |
| **File / Stream / PDF Input** | None — camera frames only | Native |
| **ASP.NET Core** | None | Full |
| **Docker / Linux** | None | Full |
| **Azure Functions / AWS Lambda** | None | Full |
| **Console / Worker Service** | None | Full |
| **WPF / WinForms / Avalonia** | None | Full |
| **Async / Await** | N/A (delegate callback) | Full |
| **Dependency Injection** | None | Full .NET DI |
| **iOS / Android camera UI** | Native MAUI / Xamarin plugin | Not the use case |
| **Pricing** | Annual subscription, per-app, per-device tiers (from EUR 1,250/yr) | From $799 perpetual |

## Quick Start: Barkoder SDK to IronBarcode Migration

### Step 1: Install IronBarcode

If you currently have `Plugin.Maui.Barkoder` or `Barkoder.Xamarin` for camera scanning, you may want to keep them for the camera UI and add IronBarcode for everything else. If you are replacing the camera workflow with file or upload processing, remove the Barkoder packages first.

```bash
# Replace camera scanning with file / upload processing
dotnet remove package Plugin.Maui.Barkoder
dotnet remove package Barkoder.Xamarin

# Install IronBarcode
dotnet add package BarCode
```

If you are running a hybrid architecture, keep Barkoder for the camera UI and just add IronBarcode alongside it.

### Step 2: Update Namespaces

Add the IronBarcode namespace where you handle file or stream inputs:

```csharp
// Barkoder MAUI (if kept for camera UI)
// using Barkoder.Maui;

// Add for file / stream / PDF / generation
using IronBarCode;
```

### Step 3: Initialize License

Add license initialization at application startup — in `Program.cs` for ASP.NET Core, or before any barcode operation in console or desktop apps:

```csharp
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

## Code Migration Examples

### Reading Barcodes from Images

Barkoder's MAUI plugin reads from a live camera through `BarkoderView` and an `IBarkoderDelegate` callback. There is no file path overload. IronBarcode reads from any image source.

**Barkoder SDK Approach (MAUI camera UI):**
```csharp
// Plugin.Maui.Barkoder — camera frames only, no file input
using Barkoder.Maui;

public partial class ScanPage : ContentPage, IBarkoderDelegate
{
    public ScanPage()
    {
        InitializeComponent();
        BarkoderView.StartScanning(this);
    }

    public void DidFinishScanning(BarcodeResult[] result)
    {
        foreach (var r in result)
        {
            Console.WriteLine($"Type: {r.BarcodeTypeName}");
            Console.WriteLine($"Value: {r.TextualData}");
        }
    }
}
```

**IronBarcode Approach (file or stream input, anywhere in .NET):**
```csharp
// NuGet: dotnet add package BarCode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var results = BarcodeReader.Read("scan.png");
foreach (var result in results)
{
    Console.WriteLine($"Type: {result.BarcodeType}");
    Console.WriteLine($"Value: {result.Value}");
}
```

IronBarcode's `BarcodeReader.Read()` auto-detects the barcode format without requiring a format to be specified in advance. The same call works for images captured by a mobile device and uploaded to a server, images loaded from a file system, or images received as byte arrays from a stream. For comprehensive reading documentation, see the [IronBarcode reading guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/).

### Damaged Barcode Recovery (DeBlur Mode Equivalent)

Barkoder's DeBlur and MatrixSight modes are among its most cited capabilities. IronBarcode provides the equivalent for file-based input through `ReadingSpeed.ExtremeDetail`.

**Barkoder SDK Approach (camera-only):**
```csharp
// Plugin.Maui.Barkoder — applies to camera frames only
BarkoderView.SetDecodingSpeed(DecodingSpeed.Slow);          // bias toward DeBlur
BarkoderView.SetEnableMisshaped1DEnabled(true);             // MatrixSight family
BarkoderView.StartScanning(this);
```

**IronBarcode Approach (any image source):**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.ExtremeDetail,
    ExpectMultipleBarcodes = false,
};

var results = BarcodeReader.Read("worn-shipping-label.png", options);

if (results.Any())
{
    Console.WriteLine($"Recovered barcode: {results.First().Value}");
}
else
{
    Console.WriteLine("Could not decode — image quality too low");
}
```

`ReadingSpeed.ExtremeDetail` runs a multi-pass image-analysis pipeline — contrast enhancement, sharpening, rotation correction, and damage recovery — before attempting to decode. For batch workflows, use `ReadingSpeed.Balanced` as the default and fall back to `ExtremeDetail` only for images that `Balanced` fails to decode.

### Multi-Barcode Detection

Barkoder's Batch MultiScan reads multiple barcodes per camera frame. IronBarcode does the same on images and PDFs alike via `ExpectMultipleBarcodes`.

**Barkoder SDK Approach (camera frames):**
```csharp
BarkoderView.SetMulticodeCachingEnabled(true);
BarkoderView.StartScanning(this);
// Results delivered to IBarkoderDelegate.DidFinishScanning(BarcodeResult[])
```

**IronBarcode Approach (file or PDF):**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
};

// Works identically on images and PDFs
var results = BarcodeReader.Read("shipping-manifest.pdf", options);

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber} | {barcode.BarcodeType} | {barcode.Value}");
}
```

This is a single method call. No page extraction, no multiple requests, no per-barcode processing overhead. The PDF is read natively.

### DataMatrix Generation and Reading

Barkoder reads DataMatrix but cannot generate it. IronBarcode covers both sides of the workflow, which is the common requirement in industrial tracking systems.

**Barkoder SDK Approach (read only, camera only):**
```csharp
// Plugin.Maui.Barkoder — read only, no generation, camera-frame input only
BarkoderView.SetEnabledBarcodeTypes(BarcodeType.Datamatrix);
BarkoderView.StartScanning(this);
```

**IronBarcode Approach (read and generate, any input source):**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// Generate a DataMatrix barcode for a part label
BarcodeWriter.CreateBarcode("PART-SN-20240315-A42291", BarcodeEncoding.DataMatrix)
    .SaveAsPng("part-label.png");

// Read DataMatrix from a scanned industrial part image
// ExtremeDetail handles the low contrast and rough edges of DPM marks
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.ExtremeDetail,
    ExpectMultipleBarcodes = false,
};

var results = BarcodeReader.Read("etched-part-photo.png", options);
foreach (var result in results)
{
    Console.WriteLine($"Part Serial: {result.Value}");
    Console.WriteLine($"Barcode Type: {result.BarcodeType}");
}
```

For a full list of supported encoding formats and generation options, see the [IronBarcode barcode generation documentation](https://ironsoftware.com/csharp/barcode/how-to/create-barcode/).

### QR Code Generation with Branding

Barkoder does not generate barcodes at all. IronBarcode generates QR codes with embedded logos, which is a common requirement for marketing, packaging, and product identification workflows.

**Barkoder SDK Approach:**
```csharp
// Not possible — Barkoder is read-only
// No QR code generation capability exists in the SDK
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// QR code with brand logo embedded at centre
QRCodeWriter.CreateQrCode("https://parts.example.com/product/A42", 500)
    .AddBrandLogo("company-logo.png")
    .SaveAsPng("branded-qr.png");
```

### ASP.NET Core Barcode API Endpoint

Barkoder has no server-side deployment model. The equivalent of a mobile scanning upload endpoint — receiving an image from any client and returning barcode data — requires a .NET library that runs server-side.

**Barkoder SDK Approach:**
```csharp
// Not possible — Plugin.Maui.Barkoder and Barkoder.Xamarin only run
// inside MAUI / Xamarin mobile apps. There is no ASP.NET Core integration.
```

**IronBarcode Approach:**
```csharp
// In Program.cs
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

app.MapPost("/api/scan", async (IFormFile file) =>
{
    using var stream = file.OpenReadStream();
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);

    var options = new BarcodeReaderOptions
    {
        Speed = ReadingSpeed.Balanced,
        ExpectMultipleBarcodes = true,
    };

    var results = BarcodeReader.Read(ms.ToArray(), options);

    return Results.Ok(results.Select(r => new
    {
        Value = r.Value,
        Format = r.BarcodeType.ToString(),
    }));
});
```

## Barkoder SDK API to IronBarcode Mapping Reference

This table maps Barkoder's MAUI / Xamarin camera-scanning API to the equivalent IronBarcode call for file, stream, or PDF input:

| Barkoder Concept | IronBarcode Equivalent |
|---|---|
| License set via barKoder portal / config | `IronBarCode.License.LicenseKey = "YOUR-KEY"` |
| `SetDecodingSpeed(DecodingSpeed.Slow)` / DeBlur | `Speed = ReadingSpeed.ExtremeDetail` |
| `SetMulticodeCachingEnabled(true)` | `ExpectMultipleBarcodes = true` |
| `SetEnabledBarcodeTypes(BarcodeType.Datamatrix)` | `BarcodeEncoding.DataMatrix` (auto-detected on read) |
| `BarkoderView.StartScanning(delegate)` | `BarcodeReader.Read("image.png")` — synchronous result |
| `result.TextualData` | `result.Value` |
| `result.BarcodeTypeName` | `result.BarcodeType` |
| MatrixSight damage recovery | `ReadingSpeed.ExtremeDetail` with ML preprocessing |
| Camera frame analysis | File path / `Stream` / `byte[]` input |
| On-device processing | Fully local — no network calls |
| No generation support | `BarcodeWriter.CreateBarcode()` / `QRCodeWriter.CreateQrCode()` |
| No PDF support | `BarcodeReader.Read("document.pdf")` — native |
| No server deployment | Works in ASP.NET Core, Docker, Azure Functions, Lambda |
| No PageNumber on result | `result.PageNumber` — available on PDF reads |

## Common Migration Issues and Solutions

### Issue 1: Need Barcode Reading Outside MAUI / Xamarin

**Barkoder SDK:** `Plugin.Maui.Barkoder` only loads inside a MAUI app head; `Barkoder.Xamarin` only inside a Xamarin app head. Neither runs in ASP.NET Core, console, worker, or desktop projects.

**Solution:** Install IronBarcode in any .NET project:
```bash
dotnet add package BarCode
```

### Issue 2: Camera Stream Input vs File Input

**Barkoder SDK:** All input arrives as a live camera frame — a continuous video feed processed on-device. There is no file-path or stream input mode.

**Solution:** IronBarcode accepts files, streams, and byte arrays. For images uploaded from a mobile client, read the request stream directly:
```csharp
using var stream = file.OpenReadStream();
using var ms = new MemoryStream();
await stream.CopyToAsync(ms);
var results = BarcodeReader.Read(ms.ToArray(), options);
```

### Issue 3: Callback-Based API vs Synchronous Result

**Barkoder SDK:** `BarkoderView.StartScanning(delegate)` uses a callback pattern — results are delivered asynchronously as barcodes are detected in the camera feed via `IBarkoderDelegate.DidFinishScanning`.

**Solution:** IronBarcode's `BarcodeReader.Read()` is synchronous and returns an `IEnumerable<BarcodeResult>` directly. In ASP.NET Core contexts where async is required, wrap in `Task.Run()`:
```csharp
var results = await Task.Run(() => BarcodeReader.Read(imageBytes, options));
```

### Issue 4: DeBlur Mode Configuration

**Barkoder SDK:** Set on the `BarkoderView` before scanning begins and applies to all subsequent camera frames.

**Solution:** In IronBarcode, the equivalent is `ReadingSpeed.ExtremeDetail` set per-read through `BarcodeReaderOptions`. Create the options object once and reuse it:
```csharp
var deepOptions = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.ExtremeDetail,
    ExpectMultipleBarcodes = false,
};

// Reuse across multiple reads
var results = BarcodeReader.Read("image.png", deepOptions);
```

### Issue 5: Missing Generation Capability

**Barkoder SDK:** Barkoder is read-only. There is no API for generating barcodes or QR codes in any of its packages.

**Solution:** IronBarcode covers both reading and generation. Use `BarcodeWriter` for standard formats and `QRCodeWriter` for QR codes:
```csharp
// Standard barcode generation
BarcodeWriter.CreateBarcode("SHIP-20240315-00428", BarcodeEncoding.Code128)
    .SaveAsPng("shipping-label.png");

// QR code generation
QRCodeWriter.CreateQrCode("https://example.com/product/A42", 500)
    .SaveAsPng("product-qr.png");

// Get as bytes for embedding in API response or PDF
byte[] barcodeBytes = BarcodeWriter.CreateBarcode("INV-100291", BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

## Barkoder SDK Migration Checklist

### Pre-Migration Tasks

Audit your codebase for any Barkoder references:

```bash
grep -r "Barkoder\|BarkoderView\|IBarkoderDelegate" --include="*.cs" .
grep -r "Plugin.Maui.Barkoder\|Barkoder.Xamarin" --include="*.csproj" .
```

Document the barcode scenarios currently implemented or planned:
- Which barcode formats need to be read (Code128, DataMatrix, QR, PDF417, etc.)
- Whether damaged barcode recovery is required (`ReadingSpeed.ExtremeDetail`)
- Whether multi-barcode detection is required (`ExpectMultipleBarcodes = true`)
- Whether barcode generation is required (`BarcodeWriter`, `QRCodeWriter`)
- Whether PDF processing is required (`BarcodeReader.Read("doc.pdf")`)
- Deployment targets (ASP.NET Core, Azure Functions, Docker, Lambda, desktop, MAUI)

### Code Update Tasks

1. Decide whether to remove `Plugin.Maui.Barkoder` / `Barkoder.Xamarin` outright or keep them for the camera UI
2. Install the `BarCode` NuGet package
3. Remove any `using Barkoder.Maui;` imports for files that are moving off the camera UI
4. Add `using IronBarCode;` to all barcode processing files
5. Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` at application startup
6. Replace `BarkoderView.StartScanning(delegate)` patterns with `BarcodeReader.Read(filePath, options)` for file workflows
7. Map `result.TextualData` to `result.Value`
8. Map `result.BarcodeTypeName` to `result.BarcodeType`
9. Map `SetDecodingSpeed(Slow)` / DeBlur configuration to `Speed = ReadingSpeed.ExtremeDetail`
10. Map `SetMulticodeCachingEnabled(true)` to `ExpectMultipleBarcodes = true`
11. Add `BarcodeWriter.CreateBarcode()` calls for any generation requirements
12. Add `QRCodeWriter.CreateQrCode()` calls for QR code generation requirements
13. Replace any PDF pre-processing code with `BarcodeReader.Read("document.pdf")`
14. Wrap synchronous `BarcodeReader.Read()` calls in `Task.Run()` for async ASP.NET Core endpoints

### Post-Migration Testing

After completing the code updates, verify the following:

- Confirm `BarcodeReader.Read()` returns results for all image formats used (PNG, JPEG, TIFF, BMP)
- Verify that `ReadingSpeed.ExtremeDetail` recovers barcodes from the lowest-quality samples in your test set
- Confirm `ExpectMultipleBarcodes = true` returns the correct count of barcodes from multi-barcode documents
- Test PDF reading against all PDF versions used in production document workflows
- Verify `BarcodeWriter.CreateBarcode()` output is scannable by your target scanners
- Verify `QRCodeWriter.CreateQrCode()` output is scannable by mobile QR readers
- Test the ASP.NET Core endpoint (if applicable) with file uploads from mobile clients
- Confirm correct operation in Docker, Azure, or Lambda deployment targets
- Run end-to-end tests covering the full read-process-generate cycle where applicable

## Key Benefits of Migrating to IronBarcode

**A Library That Works Across the Whole .NET Surface:** IronBarcode installs from a single NuGet package and runs in any .NET project — ASP.NET Core, Azure Functions, Docker, AWS Lambda, console apps, worker services, WPF, WinForms, Avalonia, Blazor, MAUI, Xamarin. There is no MAUI-only or Xamarin-only constraint, and no separate package per project type.

**Damage-Tolerant Reading Without a Camera:** The capabilities that made Barkoder compelling — DeBlur, MatrixSight, damage recovery — are matched in IronBarcode through `ReadingSpeed.ExtremeDetail` and ML-based image preprocessing. The same accuracy improvements that Barkoder achieves on live camera input, IronBarcode achieves on files, scanned documents, and uploaded images.

**Complete Read and Generate Workflow:** Barkoder is read-only. IronBarcode covers both barcode reading and generation across more than 50 formats from a single package. Teams that need to generate shipping labels, create QR codes for packaging, or embed barcodes in documents do not need a second library.

**Native PDF Support:** Reading barcodes from PDF documents — invoices, manifests, medical records, compliance documents — is a first-class IronBarcode capability. No intermediate image extraction, no per-page API calls, and no additional dependencies. A multi-page PDF is processed in a single `BarcodeReader.Read()` call.

**Full Server-Side Deployment:** IronBarcode runs in ASP.NET Core, Azure Functions, AWS Lambda, Docker containers, and any other .NET hosting environment. It processes barcodes locally with no network calls, no API keys to manage, and no latency added by a remote service. Processing one barcode or one million costs the same — there are no per-request charges.

**Predictable Perpetual Pricing:** IronBarcode is available on a one-time perpetual license starting at $799 for the Lite tier and ranging through $1,199 (Plus), $2,399 (Pro), and $4,799 (Unlimited), with no per-request charges, no per-device caps, no per-app limits, and no annual subscription renewals. For teams replacing a per-device, per-app annual subscription, the cost model is simple and predictable.
