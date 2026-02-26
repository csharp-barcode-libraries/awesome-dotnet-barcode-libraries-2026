# Migrating from Scanbot SDK to IronBarcode

This guide is written for .NET development teams moving barcode processing from Scanbot SDK to IronBarcode for server-side, desktop, or expanded file-based scenarios. This is not a like-for-like replacement: Scanbot SDK is a mobile camera scanning component, and IronBarcode is a file and document processing library. The migration is a scope change driven by requirements that the camera-only model cannot satisfy. Teams replacing a live mobile camera scanning UI without adding other requirements should evaluate whether migration is appropriate before proceeding.

For teams who started with Scanbot for a MAUI mobile app and now need barcode processing in ASP.NET Core, a desktop build, or a document pipeline, the mechanical migration is straightforward. The core change is replacing `ScanbotBarcodeSDK.BarcodeScanner.Open(configuration)` with `BarcodeReader.Read(source)`, where `source` is a file path, a stream, a byte array, or a PDF.

## Why Migrate from Scanbot SDK

**Server-Side Processing Requirements:** Scanbot SDK does not compile in ASP.NET Core, console applications, Azure Functions, or any non-MAUI project. When the mobile application requires a server-side companion that also processes barcodes — extracting them from uploaded PDFs or validating barcode values in incoming documents — Scanbot cannot cross that boundary. IronBarcode runs in all of those contexts with the same `BarcodeReader.Read()` API.

**Desktop MAUI Support:** Scanbot's package targets `net8.0-android` and `net8.0-ios`. A MAUI project that adds `net8.0-windows` or `net8.0-maccatalyst` to its `TargetFrameworks` will fail to build the desktop targets because the Scanbot package provides no assemblies for those platforms. IronBarcode [supports Windows and macOS MAUI](https://ironsoftware.com/csharp/barcode/how-to/barcode-desktop-maui/) alongside iOS and Android, which means the same package works in a multi-target MAUI project without platform-specific exclusions or conditional references.

**PDF Document Workflows:** There is no `BarcodeScanner.Read(filePath)` in Scanbot. When a workflow evolves to include reading barcodes from document uploads, scanned image archives, or PDF invoices, Scanbot's architecture cannot accommodate it. IronBarcode reads PDFs and images directly, with each result carrying the page number on which the barcode was found.

**Pricing Model Predictability:** Scanbot uses a yearly flat fee. For some organizations the annual renewal is the trigger for evaluating alternatives — particularly when the project scope has grown to include server-side or desktop targets that fall outside Scanbot's coverage. IronBarcode is a one-time perpetual purchase, eliminating the annual renewal obligation.

### The Fundamental Problem

Scanbot SDK requires a live camera feed. When requirements expand beyond mobile, there is no equivalent path through the existing dependency:

```csharp
// Scanbot SDK: camera-only — no file or server equivalent exists
var configuration = new BarcodeScannerConfiguration
{
    BarcodeFormats = new[] { BarcodeFormat.Code128, BarcodeFormat.QrCode }
};

// This call requires iOS or Android hardware and a MAUI camera UI
// It cannot be redirected to a file path, stream, or server context
var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(configuration);
```

IronBarcode accepts the same source types — file, stream, byte array, or PDF — in any project context:

```csharp
// IronBarcode: the same call works in MAUI, ASP.NET Core, console, WPF, Azure Functions
IronBarCode.License.LicenseKey = "YOUR-KEY";

// From a file — works on any platform
var results = BarcodeReader.Read("invoice.pdf");
foreach (var result in results)
    Console.WriteLine($"Page {result.PageNumber}: {result.Value} ({result.Format})");
```

## IronBarcode vs Scanbot SDK: Feature Comparison

| Feature | Scanbot SDK | IronBarcode |
|---|---|---|
| **Input from file path** | No | Yes |
| **Input from stream** | No | Yes |
| **Input from byte array** | No | Yes |
| **PDF barcode extraction** | No | Yes |
| **Live camera viewfinder** | Yes | No (use MediaPicker) |
| **Barcode generation** | No | Yes |
| **iOS MAUI** | Yes | Yes |
| **Android MAUI** | Yes | Yes |
| **Windows MAUI** | No | Yes |
| **macOS MAUI** | No | Yes |
| **ASP.NET Core** | No | Yes |
| **Console / Server** | No | Yes |
| **Azure / Lambda / Docker** | No | Yes |
| **.NET Framework 4.6.2+** | No | Yes |
| **ML error correction** | No | Yes |
| **Damaged barcode recovery** | No | Yes |
| **Auto format detection** | Yes | Yes |
| **1D formats** | 20+ | 30+ |
| **2D formats** | QR, DataMatrix, PDF417, Aztec | QR, DataMatrix, PDF417, Aztec, MaxiCode |
| **License model** | Yearly flat fee | One-time perpetual |
| **Published pricing** | Contact sales | Yes ($749–$5,999) |

## Quick Start: Scanbot SDK to IronBarcode Migration

### Step 1: Replace NuGet Package

Remove the Scanbot package:

```bash
dotnet remove package ScanbotBarcodeSDK.MAUI
```

If the project uses conditional package references in the `.csproj` file, remove those entries as well:

```xml
<!-- Remove any Scanbot-related package references -->
<PackageReference Include="ScanbotBarcodeSDK.MAUI" Version="7.1.1" />
```

Install IronBarcode:

```bash
dotnet add package IronBarcode
```

IronBarcode is available on NuGet and supports .NET 6, 7, 8, 9, and .NET Framework 4.6.2+. For a MAUI multi-target project, a single `dotnet add package` works across all targets — no platform-conditional package references are needed.

### Step 2: Update Namespaces

Replace Scanbot namespaces with the IronBarcode namespace:

```csharp
// Remove
using ScanbotBarcodeSDK.MAUI;

// Add
using IronBarCode;
```

### Step 3: Initialize License

Remove the Scanbot initialization block and replace it with the IronBarcode license key. Add the license initialization at application startup in `MauiProgram.cs`, `App.xaml.cs`, or `Program.cs` depending on project type:

```csharp
// Remove this
ScanbotSDK.Initialize(new ScanbotSDKConfiguration
{
    LicenseKey = "YOUR-SCANBOT-LICENSE-KEY",
    EnableLogging = false
});

// Add this — one line, placed once at startup
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

## Code Migration Examples

### MAUI Photo Capture

Scanbot's scanning call opens a full-screen camera UI and returns when the user scans a barcode or cancels. IronBarcode does not provide a camera UI; the replacement uses MAUI's `MediaPicker` to invoke the system camera, capture a photo, and pass the image bytes to `BarcodeReader.Read()`.

**Scanbot SDK Approach:**

```csharp
using ScanbotBarcodeSDK.MAUI;

private async void ScanButton_Clicked(object sender, EventArgs e)
{
    var configuration = new BarcodeScannerConfiguration
    {
        BarcodeFormats = new[]
        {
            BarcodeFormat.Code128,
            BarcodeFormat.QrCode,
            BarcodeFormat.Ean13
        },
        FinderAspectRatio = new AspectRatio(1, 1),
        TopBarBackgroundColor = Colors.DarkBlue
    };

    var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(configuration);

    if (result.Status == OperationResult.Ok)
    {
        foreach (var barcode in result.Barcodes)
            ResultLabel.Text = $"{barcode.Format}: {barcode.Text}";
    }
}
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

private async void ScanButton_Clicked(object sender, EventArgs e)
{
    var photo = await MediaPicker.CapturePhotoAsync();
    if (photo == null) return;

    using var stream = await photo.OpenReadAsync();
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);

    var results = BarcodeReader.Read(ms.ToArray());
    var first = results.FirstOrDefault();
    ResultLabel.Text = first != null
        ? $"{first.Format}: {first.Value}"
        : "No barcode found";
}
```

The UX change is that the user sees the system camera screen rather than Scanbot's embedded viewfinder with a scan region overlay. For business applications — inventory, logistics, document workflows — the system camera is adequate. The [.NET MAUI barcode scanner tutorial](https://ironsoftware.com/csharp/barcode/get-started/net-maui-barcode-scanner-reader-tutorial/) covers the full integration pattern for both iOS and Android targets, including permission handling.

### Scan Options and Configuration

Scanbot's `BarcodeScannerConfiguration` controls which formats the scanner accepts and how the camera UI looks. IronBarcode's `BarcodeReaderOptions` controls how the image is analyzed — processing speed, multi-barcode detection, and similar parameters. The configuration concepts map to different concerns because the underlying operations are different.

**Scanbot SDK Approach:**

```csharp
var configuration = new BarcodeScannerConfiguration
{
    BarcodeFormats = new[] { BarcodeFormat.Code128, BarcodeFormat.QrCode },
    FinderAspectRatio = new AspectRatio(1, 1),
    FlashEnabled = false,
    OrientationLockMode = OrientationLockMode.Portrait
};

var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(configuration);

if (result.Status == OperationResult.Ok)
{
    foreach (var barcode in result.Barcodes)
        Console.WriteLine($"{barcode.Format}: {barcode.Text}");
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Format detection is automatic — BarcodeReaderOptions controls processing behavior
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = false
};

// Combined with the MediaPicker capture pattern
var photo = await MediaPicker.CapturePhotoAsync();
if (photo == null) return;

using var photoStream = await photo.OpenReadAsync();
using var ms = new MemoryStream();
await photoStream.CopyToAsync(ms);

var results = BarcodeReader.Read(ms.ToArray(), options);
foreach (var result in results)
    Console.WriteLine($"{result.Format}: {result.Value}");
```

Format specification is not required — IronBarcode auto-detects all supported formats. The `BarcodeReaderOptions` parameters that correspond most directly to Scanbot configuration are `Speed` (controls processing thoroughness versus performance) and `ExpectMultipleBarcodes` (continues scanning after the first match).

### Server-Side PDF Processing

This scenario has no Scanbot equivalent. With IronBarcode, the same package installed for the MAUI mobile app can extract barcodes from PDFs in a server-side ASP.NET Core endpoint, giving teams a single barcode dependency that works across both deployment contexts.

**Scanbot SDK Approach:**

```csharp
// Scanbot SDK cannot be used here — the package does not compile
// in ASP.NET Core, Azure Functions, console apps, or any non-MAUI project.
// There is no server-side Scanbot equivalent for this endpoint.
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
// Works in ASP.NET Core, console apps, Azure Functions, Docker — the same package as MAUI
using IronBarCode;

[HttpPost("extract-barcodes")]
public async Task<IActionResult> ExtractBarcodes(IFormFile file)
{
    using var ms = new MemoryStream();
    await file.CopyToAsync(ms);

    // Reads all barcodes from all pages of the uploaded PDF
    var results = BarcodeReader.Read(ms.ToArray());
    var values = results.Select(r => new
    {
        r.Value,
        Format = r.Format.ToString(),
        r.PageNumber
    });

    return Ok(values);
}
```

IronBarcode handles multi-page PDFs natively — each result carries a `PageNumber` property indicating which page the barcode was found on. The [PDF barcode extraction guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) covers the full range of PDF reading options including page range selection and performance tuning for large document batches.

### ASP.NET Core Background Processing

For batch workflows such as Azure Functions or Worker Services processing archived PDF documents, IronBarcode provides the same API used in the MAUI app and the ASP.NET Core controller.

**Scanbot SDK Approach:**

```csharp
// Scanbot SDK does not support this deployment context.
// The package will not compile in Azure Functions or Worker Services.
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

// Azure Function or Worker Service batch processing
public void ProcessInvoiceBatch(IEnumerable<string> filePaths)
{
    foreach (var filePath in filePaths)
    {
        var results = BarcodeReader.Read(filePath);
        foreach (var result in results)
            Console.WriteLine($"File: {filePath} | Page {result.PageNumber}: {result.Value} ({result.Format})");
    }
}
```

## Scanbot SDK API to IronBarcode Mapping Reference

| Scanbot SDK | IronBarcode |
|---|---|
| `ScanbotSDK.Initialize(new ScanbotSDKConfiguration { LicenseKey = "..." })` | `IronBarCode.License.LicenseKey = "key"` |
| `new BarcodeScannerConfiguration()` | `new BarcodeReaderOptions()` |
| `ScanbotBarcodeSDK.BarcodeScanner.Open(configuration)` | `BarcodeReader.Read(path / stream / bytes)` |
| `result.Status == OperationResult.Ok` | `results.Any()` or `results.FirstOrDefault() != null` |
| `result.Barcodes` | Return value of `BarcodeReader.Read()` |
| `barcode.Format` | `result.Format` (IronBarCode.BarcodeEncoding) |
| `barcode.Text` | `result.Value` |
| `BarcodeFormat.Code128` | `BarcodeEncoding.Code128` |
| `BarcodeFormat.QrCode` | `BarcodeEncoding.QRCode` |
| `BarcodeFormat.Ean13` | `BarcodeEncoding.EAN13` |
| `BarcodeScannerConfiguration.FinderAspectRatio` | No equivalent — image framing handled by MediaPicker |
| `BarcodeScannerConfiguration.FlashEnabled` | No equivalent — use MediaPicker options |
| Camera-required input | File path, stream, byte array, or PDF |
| iOS and Android MAUI only | All .NET platforms and project types |

## Common Migration Issues and Solutions

### Issue 1: No Live Viewfinder Equivalent

**Scanbot SDK:** Provides a real-time camera viewfinder embedded in the scanning experience, with a scan region overlay, haptic feedback, and continuous barcode detection in the live video feed.

**Solution:** Use MAUI's `MediaPicker.CapturePhotoAsync()` to open the system camera and capture a still image for processing:

```csharp
var photo = await MediaPicker.CapturePhotoAsync();
if (photo == null) return;

using var stream = await photo.OpenReadAsync();
using var ms = new MemoryStream();
await stream.CopyToAsync(ms);

var results = BarcodeReader.Read(ms.ToArray());
```

For business applications, the system camera is adequate. For consumer apps where the live overlay is central to the user experience, evaluate whether this UX change is acceptable before committing to the migration.

### Issue 2: Camera Permissions Differences

**Scanbot SDK:** The Scanbot package handled camera permission requests as part of its UI flow, so camera permissions were implicitly managed by the SDK initialization and scanner launch.

**Solution:** With `MediaPicker`, MAUI handles permissions through the standard mechanism. Verify that `AndroidManifest.xml` includes the camera permission declaration and that `Info.plist` includes the `NSCameraUsageDescription` key. These entries are typically present in scaffolded MAUI project templates. If the app was previously using only Scanbot for camera access, confirm these manifest entries are in place before testing:

```xml
<!-- Android: AndroidManifest.xml -->
<uses-permission android:name="android.permission.CAMERA" />

<!-- iOS: Info.plist -->
<key>NSCameraUsageDescription</key>
<string>This app uses the camera to scan barcodes.</string>
```

### Issue 3: Windows Build Errors Cleared

**Scanbot SDK:** The `ScanbotBarcodeSDK.MAUI` package targets only `net8.0-android` and `net8.0-ios`, causing build failures when `net8.0-windows` is present in `TargetFrameworks`.

**Solution:** Removing the Scanbot package reference resolves the Windows build failure. IronBarcode resolves correctly across all MAUI targets with no platform-conditional configuration:

```bash
# Verify the build succeeds on all targets after removing Scanbot and adding IronBarcode
dotnet build -f net8.0-windows10.0.19041.0
dotnet build -f net8.0-ios
dotnet build -f net8.0-android
```

### Issue 4: Format Enum Namespace Change

**Scanbot SDK:** Uses `BarcodeFormat` enum values such as `BarcodeFormat.Code128` from the `ScanbotBarcodeSDK.MAUI` namespace.

**Solution:** IronBarcode uses the `BarcodeEncoding` enum in the `IronBarCode` namespace. Update all format references and any code that stores, compares, or switches on format enum values:

```csharp
// Before (Scanbot)
BarcodeFormat.Code128
BarcodeFormat.QrCode
BarcodeFormat.Ean13

// After (IronBarcode)
BarcodeEncoding.Code128
BarcodeEncoding.QRCode
BarcodeEncoding.EAN13
```

Note that IronBarcode auto-detects all supported formats by default — explicit format filtering through `BarcodeReaderOptions` is only needed for performance optimization when the expected format is known in advance.

## Scanbot SDK Migration Checklist

### Pre-Migration Tasks

Audit the codebase to identify all Scanbot SDK usage:

```bash
grep -rn "ScanbotSDK.Initialize" --include="*.cs" .
grep -rn "BarcodeScannerConfiguration" --include="*.cs" .
grep -rn "ScanbotBarcodeSDK.BarcodeScanner.Open" --include="*.cs" .
grep -rn "result\.Barcodes" --include="*.cs" .
grep -rn "barcode\.Format" --include="*.cs" .
grep -rn "barcode\.Text" --include="*.cs" .
grep -rn "OperationResult\.Ok" --include="*.cs" .
grep -rn "using ScanbotBarcodeSDK" --include="*.cs" .
grep -rn "BarcodeFormat\." --include="*.cs" .
```

- Document all scanning call sites and note whether they are in shared code, platform-specific code, or UI event handlers
- Identify whether any pages or flows rely on Scanbot's real-time viewfinder as a primary user interaction
- Confirm that camera permission entries exist in `AndroidManifest.xml` and `Info.plist`
- Review the `.csproj` for any conditional package references tied to Scanbot

### Code Update Tasks

1. Remove `ScanbotBarcodeSDK.MAUI` NuGet package
2. Remove any conditional `<PackageReference>` entries for Scanbot from the `.csproj`
3. Install `IronBarcode` NuGet package
4. Replace `using ScanbotBarcodeSDK.MAUI;` with `using IronBarCode;` in all files
5. Replace `ScanbotSDK.Initialize(...)` with `IronBarCode.License.LicenseKey = "key"` at application startup
6. Replace `ScanbotBarcodeSDK.BarcodeScanner.Open(configuration)` with `MediaPicker.CapturePhotoAsync()` followed by stream copy and `BarcodeReader.Read(bytes)` in MAUI camera workflows
7. Replace `ScanbotBarcodeSDK.BarcodeScanner.Open(configuration)` with `BarcodeReader.Read(filePath)` or `BarcodeReader.Read(stream)` in server-side and file processing contexts
8. Replace `result.Status == OperationResult.Ok` checks with `results.Any()` or `results.FirstOrDefault() != null`
9. Replace `result.Barcodes` collection references with the return value of `BarcodeReader.Read()`
10. Replace `barcode.Text` property access with `result.Value`
11. Replace `BarcodeFormat.*` enum values with `BarcodeEncoding.*` equivalents
12. Replace `new BarcodeScannerConfiguration()` with `new BarcodeReaderOptions()` where processing options are needed

### Post-Migration Testing

- Verify the build succeeds on all MAUI target platforms, including Windows and macOS if present
- Test the `MediaPicker.CapturePhotoAsync()` flow on a physical iOS device and a physical Android device
- Confirm that barcode values decoded by IronBarcode match values previously decoded by Scanbot for the same physical barcodes
- Test multi-page PDF extraction if the project includes document processing workflows
- Verify that format detection covers all barcode types previously configured in `BarcodeScannerConfiguration.BarcodeFormats`
- Confirm that server-side endpoints or background jobs produce correct results on the same document samples
- Verify camera permission prompts appear correctly on first launch on iOS and Android

## Key Benefits of Migrating to IronBarcode

**Unified Package Across All Deployment Targets:** IronBarcode installs once and runs in MAUI mobile, MAUI desktop, ASP.NET Core, Azure Functions, console applications, and Windows desktop applications. Teams no longer need separate barcode dependencies for different parts of the same system.

**PDF and Document Processing:** IronBarcode reads barcodes from PDF files natively, with each result carrying the page number on which it was found. Document workflows that were previously impossible within the Scanbot model become straightforward operations with `BarcodeReader.Read("document.pdf")`.

**Windows and macOS Desktop MAUI Support:** Multi-target MAUI projects that include Windows or macOS build without errors after removing Scanbot. IronBarcode provides the same barcode reading API on all four MAUI targets, eliminating the platform-conditional dependency management that Scanbot required.

**Perpetual License Ownership:** The one-time purchase model eliminates annual renewal obligations. Once licensed, IronBarcode can be used indefinitely at the purchased tier without recurring fees. Teams whose project scope expanded beyond Scanbot's mobile coverage find that the perpetual model better aligns with long-term cost planning.

**Barcode Generation:** IronBarcode can produce barcodes in all supported formats as image files, streams, or embedded content. Projects that need both reading and generation no longer require a second library alongside Scanbot.

**Machine Learning-Assisted Reading:** IronBarcode applies machine learning-based error correction and damaged barcode recovery for difficult real-world images — low contrast, partial damage, poor print quality — that standard detection algorithms fail to decode. This capability is particularly relevant for document processing workflows where image quality is variable.
