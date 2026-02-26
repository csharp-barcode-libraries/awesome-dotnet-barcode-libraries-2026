# Migrating from Barkoder SDK to IronBarcode

There is nothing to migrate from in the traditional sense. Barkoder SDK has no .NET library, no NuGet package, and no C# API. If you are reading this, you likely spent time evaluating Barkoder — reading about MatrixSight, DeBlur mode, multi-barcode detection, and DPM support — before discovering that none of it is accessible from a C# project. This guide shows you how IronBarcode covers each of those requirements in .NET, with working code for every scenario that brought you to Barkoder in the first place.

## Why Migrate from Barkoder SDK

Teams moving from a Barkoder evaluation to IronBarcode consistently report the same triggers:

**No .NET Support:** Barkoder SDK has no NuGet package, no C# bindings, and no official path into a .NET project. NuGet.org returns zero results for "barkoder." The platform list on Barkoder's website — iOS, Android, React Native, Flutter, Cordova, Capacitor — does not include .NET, C#, ASP.NET Core, Windows, or Linux. For any .NET developer, this is a complete blocker before any feature evaluation begins.

**Read-Only Architecture:** Barkoder is a scanning-only SDK. If your application needs to generate barcodes — print shipping labels, create QR codes for packaging, embed codes in documents — Barkoder provides nothing. Teams that discover a generation requirement mid-project need a library that covers both sides of the workflow.

**No PDF Processing:** In .NET enterprise applications, barcodes frequently appear in PDF documents — invoices, shipping manifests, medical records, compliance documents. Barkoder has no concept of this workflow. It is designed for live camera input on a mobile device and cannot process a PDF file.

**No Server-Side Deployment:** Barkoder's processing model is on-device mobile scanning. It cannot run in ASP.NET Core, Azure Functions, Docker containers, or AWS Lambda. Teams building server-side barcode APIs, document processing pipelines, or cloud functions have no path to Barkoder deployment.

**Community Binding Risk:** The community-maintained MAUI binding project is not officially supported by Barkoder, exposes only a subset of the SDK, and is not updated alongside the main SDK releases. Depending on an unsupported community binding for a production barcode workflow represents a maintenance and reliability risk that most teams are not willing to accept.

### The Fundamental Problem

Barkoder is simply not present on the .NET platform. A .NET developer cannot write the Barkoder equivalent of this call:

```swift
// iOS Swift — no C# equivalent exists
let barkoderView = BarkoderView(frame: frame)
barkoderView.config = BarkoderConfig(licenseKey: "LICENSE_KEY") { config in
    config.decoder.deblurEnabled = true
    config.decoder.decoderType = .code128
}
barkoderView.startScanning { result in
    print(result.textualData ?? "No result")
}
```

IronBarcode provides a native .NET API for the same capability:

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
    Console.WriteLine($"Format: {result.Format} | Value: {result.Value}");
}
```

## IronBarcode vs Barkoder SDK: Feature Comparison

| Feature | Barkoder SDK | IronBarcode |
|---|---|---|
| **.NET / C# API** | None | Full |
| **NuGet Package** | None | `IronBarcode` |
| **Damaged Barcode Recovery** | MatrixSight / DeBlur | `ReadingSpeed.ExtremeDetail` + ML |
| **Multi-Barcode Detection** | Yes | `ExpectMultipleBarcodes = true` |
| **DataMatrix / DPM** | Yes (read only) | Yes — read and generate |
| **QR Code** | Yes (read only) | Yes — read and generate |
| **PDF417** | Yes | Yes |
| **Aztec** | Yes | Yes |
| **Code128, Code39, EAN** | Yes | Yes — 50+ formats |
| **Barcode Generation** | None | Full — all formats |
| **PDF Processing** | None | Native |
| **ASP.NET Core** | None | Full |
| **Docker / Linux** | None | Full |
| **Azure Functions** | None | Full |
| **AWS Lambda** | None | Full |
| **Offline / Air-Gapped** | On-device | Fully local |
| **Async / Await** | N/A | Full |
| **Dependency Injection** | None | Full .NET DI |
| **iOS via MAUI** | Native SDK | Via .NET MAUI |
| **Android via MAUI** | Native SDK | Via .NET MAUI |
| **Pricing** | Mobile SDK licensing | From $749 perpetual |

## Quick Start: Barkoder SDK to IronBarcode Migration

### Step 1: Replace NuGet Package

Since Barkoder has no NuGet package, there is nothing to remove. Install IronBarcode directly:

```bash
dotnet add package IronBarcode
```

If your project contains any reference to the community Barkoder MAUI binding, remove it:

```bash
dotnet remove package Barkoder.MAUI
```

### Step 2: Update Namespaces

Remove any community binding namespaces and add the IronBarcode namespace:

```csharp
// Remove (community binding — if present)
// using BarkoderLib;

// Add
using IronBarCode;
```

### Step 3: Initialize License

Add license initialization at application startup — in `Program.cs` for ASP.NET Core, or before any barcode operation in console or desktop apps:

```csharp
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

## Code Migration Examples

### Reading Barcodes from Images

The core reading scenario maps directly from Barkoder's camera scanning capability to IronBarcode's file-based API.

**Barkoder SDK Approach:**
```swift
// iOS Swift — no C# equivalent exists
let barkoderView = BarkoderView(frame: frame)
barkoderView.config = BarkoderConfig(licenseKey: "LICENSE_KEY") { config in
    config.decoder.decoderType = .code128
}
barkoderView.startScanning { result in
    if let value = result.textualData {
        print("Barcode: \(value)")
        print("Type: \(result.barcodeTypeName)")
    }
}
```

**IronBarcode Approach:**
```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var results = BarcodeReader.Read("scan.png");
foreach (var result in results)
{
    Console.WriteLine($"Format: {result.Format}");
    Console.WriteLine($"Value: {result.Value}");
}
```

IronBarcode's `BarcodeReader.Read()` auto-detects the barcode format without requiring a format to be specified in advance. The same call works for images captured by a mobile device and uploaded to a server, images loaded from a file system, or images received as byte arrays from a stream. For comprehensive reading documentation, see the [IronBarcode reading guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/).

### Damaged Barcode Recovery (DeBlur Mode Equivalent)

Barkoder's DeBlur mode is one of its most cited capabilities. IronBarcode provides the equivalent through `ReadingSpeed.ExtremeDetail`.

**Barkoder SDK Approach:**
```kotlin
// Android Kotlin — no C# equivalent exists
barkoderView.config = BarkoderConfig("LICENSE_KEY")
barkoderView.config.decoder.deblurEnabled = true
barkoderView.config.decoder.decoderType = DecoderType.Code128
barkoderView.startScanning { result ->
    println("Recovered: ${result.textualData}")
}
```

**IronBarcode Approach:**
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

`ReadingSpeed.ExtremeDetail` runs a multi-pass ML-based image analysis pipeline, applying contrast enhancement, sharpening, rotation correction, and damage recovery before attempting to decode. For batch workflows, use `ReadingSpeed.Balanced` as the default and fall back to `ExtremeDetail` only for images that `Balanced` fails to decode.

### Multi-Barcode Detection

Barkoder's multi-barcode frame scanning maps to IronBarcode's `ExpectMultipleBarcodes` option, which works on images and PDFs alike.

**Barkoder SDK Approach:**
```kotlin
// Android Kotlin — no C# equivalent exists
barkoderView.config.decoder.multicodingEnabled = true
barkoderView.startScanning { results ->
    results.forEach { result ->
        println("${result.barcodeType}: ${result.textualData}")
    }
}
```

**IronBarcode Approach:**
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
    Console.WriteLine($"Page {barcode.PageNumber} | {barcode.Format} | {barcode.Value}");
}
```

This is a single method call. No page extraction, no multiple requests, no per-barcode processing overhead. The PDF is read natively.

### DataMatrix Generation and Reading

Barkoder reads DataMatrix but cannot generate it. IronBarcode covers both sides of the workflow, which is the common requirement in industrial tracking systems.

**Barkoder SDK Approach:**
```swift
// iOS Swift — read only, no generation, no C# equivalent
barkoderView.config = BarkoderConfig(licenseKey: "LICENSE_KEY") { config in
    config.decoder.decoderType = .dataMatrix
}
barkoderView.startScanning { result in
    print("Part ID: \(result.textualData ?? "No result")")
}
```

**IronBarcode Approach:**
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
    Console.WriteLine($"Barcode Type: {result.Format}");
}
```

For a full list of supported encoding formats and generation options, see the [IronBarcode barcode generation documentation](https://ironsoftware.com/csharp/barcode/how-to/create-barcode/).

### QR Code Generation with Branding

Barkoder does not generate barcodes. IronBarcode generates QR codes with embedded logos, which is a common requirement for marketing, packaging, and product identification workflows.

**Barkoder SDK Approach:**
```swift
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

Barkoder has no server-side deployment model. The equivalent of a mobile scanning upload endpoint — receiving an image from any client and returning barcode data — requires a .NET library.

**Barkoder SDK Approach:**
```csharp
// Not possible — Barkoder has no ASP.NET Core integration,
// no NuGet package, and no server-side execution model
```

**IronBarcode Approach:**
```csharp
// In Program.cs
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

app.MapPost("/api/scan", async (IFormFile file) =>
{
    using var stream = file.OpenReadStream();
    var imageBytes = new byte[stream.Length];
    await stream.ReadAsync(imageBytes);

    var options = new BarcodeReaderOptions
    {
        Speed = ReadingSpeed.Balanced,
        ExpectMultipleBarcodes = true,
    };

    var results = BarcodeReader.Read(imageBytes, options);

    return Results.Ok(results.Select(r => new
    {
        Value = r.Value,
        Format = r.Format.ToString(),
    }));
});
```

## Barkoder SDK API to IronBarcode Mapping Reference

Since there is no Barkoder C# code to map directly, this table frames the conceptual equivalences between what Barkoder does on mobile and how IronBarcode achieves the same result in .NET:

| Barkoder Concept | IronBarcode Equivalent |
|---|---|
| SDK initialization with license key | `IronBarCode.License.LicenseKey = "YOUR-KEY"` |
| `DeblurEnabled = true` | `Speed = ReadingSpeed.ExtremeDetail` |
| `MultipleScanningEnabled = true` | `ExpectMultipleBarcodes = true` |
| `DecoderType = .dataMatrix` | `BarcodeEncoding.DataMatrix` (auto-detected on read) |
| `startScanning(callback)` | `BarcodeReader.Read("image.png")` — synchronous result |
| `result.textualData` | `result.Value` |
| `result.symbology` / `result.barcodeTypeName` | `result.Format` |
| MatrixSight damage recovery | `ReadingSpeed.ExtremeDetail` with ML preprocessing |
| Camera frame analysis | File path / `Stream` / `byte[]` input |
| On-device processing | Fully local — no network calls |
| No generation support | `BarcodeWriter.CreateBarcode()` / `QRCodeWriter.CreateQrCode()` |
| No PDF support | `BarcodeReader.Read("document.pdf")` — native |
| No server deployment | Works in ASP.NET Core, Docker, Azure Functions, Lambda |
| `result.pageNumber` (N/A) | `result.PageNumber` — available on PDF reads |

## Common Migration Issues and Solutions

### Issue 1: No NuGet Package Found

**Barkoder SDK:** Searching NuGet.org or using `dotnet add package barkoder` returns zero results. There is no official .NET package.

**Solution:** Install IronBarcode directly:
```bash
dotnet add package IronBarcode
```

If using the community Barkoder MAUI binding, remove it and replace with IronBarcode:
```bash
dotnet remove package Barkoder.MAUI
dotnet add package IronBarcode
```

### Issue 2: Camera Stream Input vs File Input

**Barkoder SDK:** All input arrives as a live camera frame — a continuous video feed processed on-device. There is no file path input mode.

**Solution:** IronBarcode accepts files, streams, byte arrays, and System.Drawing.Image objects. For images uploaded from a mobile client, read the request stream directly:
```csharp
using var stream = file.OpenReadStream();
var imageBytes = new byte[stream.Length];
await stream.ReadAsync(imageBytes);
var results = BarcodeReader.Read(imageBytes, options);
```

### Issue 3: Callback-Based API vs Synchronous Result

**Barkoder SDK:** `startScanning()` uses a callback pattern — results are delivered asynchronously as barcodes are detected in the camera feed.

**Solution:** IronBarcode's `BarcodeReader.Read()` is synchronous and returns an `IEnumerable<BarcodeResult>` directly. In ASP.NET Core contexts where async is required, wrap in `Task.Run()`:
```csharp
var results = await Task.Run(() => BarcodeReader.Read(imageBytes, options));
```

### Issue 4: DeBlur Mode Configuration

**Barkoder SDK:** `deblurEnabled = true` is set on the config object before scanning begins and applies to all subsequent scans.

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

**Barkoder SDK:** Barkoder is read-only. There is no API for generating barcodes or QR codes.

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

Audit your codebase for any Barkoder references — including community binding packages:

```bash
grep -r "Barkoder\|BarkoderLib\|barkoderView\|BarkoderConfig" --include="*.cs" .
grep -r "Barkoder\|barkoder" --include="*.csproj" .
grep -r "Barkoder\|barkoder" --include="*.xml" .
```

Document the barcode scenarios currently implemented or planned:
- Which barcode formats need to be read (Code128, DataMatrix, QR, PDF417, etc.)
- Whether damaged barcode recovery is required (`ReadingSpeed.ExtremeDetail`)
- Whether multi-barcode detection is required (`ExpectMultipleBarcodes = true`)
- Whether barcode generation is required (`BarcodeWriter`, `QRCodeWriter`)
- Whether PDF processing is required (`BarcodeReader.Read("doc.pdf")`)
- Deployment targets (ASP.NET Core, Azure Functions, Docker, Lambda, desktop, MAUI)

### Code Update Tasks

1. Remove any community Barkoder MAUI binding package from `.csproj`
2. Install `IronBarcode` NuGet package
3. Remove any `using BarkoderLib;` or community binding namespace imports
4. Add `using IronBarCode;` to all barcode processing files
5. Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` at application startup
6. Replace `startScanning(callback)` patterns with `BarcodeReader.Read(filePath, options)`
7. Map `result.textualData` to `result.Value`
8. Map `result.symbology` or `result.barcodeTypeName` to `result.Format`
9. Map `deblurEnabled = true` to `Speed = ReadingSpeed.ExtremeDetail` in `BarcodeReaderOptions`
10. Map `multicodingEnabled = true` to `ExpectMultipleBarcodes = true` in `BarcodeReaderOptions`
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

**A Platform That Actually Works in .NET:** IronBarcode installs from NuGet, runs in any .NET project, and requires no community bindings, no native platform targets, and no unsupported experimental packages. The transition from a Barkoder evaluation dead-end to a working .NET barcode implementation is a matter of one `dotnet add package` command and a license key.

**Damage-Tolerant Reading Without a Camera:** The capabilities that made Barkoder compelling — DeBlur, MatrixSight, damage recovery — are present in IronBarcode through `ReadingSpeed.ExtremeDetail` and ML-based image preprocessing. The same accuracy improvements that Barkoder achieves on live camera input, IronBarcode achieves on files, scanned documents, and uploaded images.

**Complete Read and Generate Workflow:** Barkoder is read-only. IronBarcode covers both barcode reading and generation across 50+ formats from a single package. Teams that need to generate shipping labels, create QR codes for packaging, or embed barcodes in documents do not need a second library.

**Native PDF Support:** Reading barcodes from PDF documents — invoices, manifests, medical records, compliance documents — is a first-class IronBarcode capability. No intermediate image extraction, no per-page API calls, and no additional dependencies. A multi-page PDF is processed in a single `BarcodeReader.Read()` call.

**Full Server-Side Deployment:** IronBarcode runs in ASP.NET Core, Azure Functions, AWS Lambda, Docker containers, and any other .NET hosting environment. It processes barcodes locally with no network calls, no API keys to manage, and no latency added by a remote service. Processing one barcode or one million costs the same — there are no per-request charges.

**Predictable Perpetual Pricing:** IronBarcode is available on a one-time perpetual license starting at $749 for a single developer, with no per-request charges, no volume limits, and no subscription renewals. For teams building production applications that process barcodes at scale, the cost model is simple and predictable.
