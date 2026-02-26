# Migrating from Scanbot SDK to IronBarcode

This migration is not a straight replacement — it is a scope change. Scanbot SDK launches a full-screen camera UI. IronBarcode reads from files, streams, and PDFs. If you are migrating, something in your requirements changed: server-side processing entered the picture, the project started targeting Windows MAUI, or a use case arrived that the camera-only model cannot handle. This guide is written for that situation.

For teams who started with Scanbot for a MAUI mobile app and now need barcode processing in ASP.NET Core, a desktop build, or a document pipeline — the migration path is straightforward. The core change is replacing `ScanbotBarcodeSDK.BarcodeScanner.Open(configuration)` with `BarcodeReader.Read(source)`, where `source` is a file path, a stream, or a byte array.

## Why Teams Migrate

**Server-side processing requirements were added.** Scanbot SDK does not compile in ASP.NET Core, console apps, Azure Functions, or any non-MAUI project. When the mobile app needs a server-side companion that also processes barcodes — extracting them from uploaded PDFs, for instance — Scanbot cannot cross that boundary. IronBarcode runs in all of those contexts with the same API.

**The Windows or macOS MAUI target was added.** Scanbot's package targets `net8.0-android` and `net8.0-ios`. A MAUI app that adds `net8.0-windows` or `net8.0-maccatalyst` to its `TargetFrameworks` will fail to build the desktop targets. IronBarcode [supports Windows and macOS MAUI](https://ironsoftware.com/csharp/barcode/how-to/barcode-desktop-maui/) alongside iOS and Android, which means the same package works in a multi-target MAUI project without platform-specific exclusions.

**PDF or file processing was needed.** There is no `BarcodeScanner.Read(filePath)` in Scanbot. If your workflow evolved to include reading barcodes from document uploads, scanned images, or PDF invoices, Scanbot's architecture cannot accommodate it. IronBarcode reads PDFs and images directly.

**Licensing cost at scale.** Scanbot uses a yearly flat fee. For some organizations the annual renewal is the trigger — evaluating alternatives at license renewal time is a common pattern. IronBarcode is a one-time perpetual purchase.

## Quick Start

### Step 1: Remove ScanbotBarcodeSDK.MAUI

```bash
dotnet remove package ScanbotBarcodeSDK.MAUI
```

If you have conditional references in your `.csproj`, remove those too:

```xml
<!-- Remove any Scanbot-related package references -->
<PackageReference Include="ScanbotBarcodeSDK.MAUI" Version="7.1.1" />
```

### Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

IronBarcode is available on [NuGet](https://www.nuget.org/packages/BarCode) and works in .NET 6, 7, 8, and .NET Framework 4.6.2+. For a MAUI multi-target project, a single `dotnet add package` works — no platform-conditional package references needed.

### Step 3: Replace License Initialization

Remove the Scanbot initialization block:

```csharp
// Remove this
ScanbotSDK.Initialize(new ScanbotSDKConfiguration
{
    LicenseKey = "YOUR-SCANBOT-LICENSE-KEY",
    EnableLogging = false
});
```

Add the IronBarcode license key at application startup — in `MauiProgram.cs`, `App.xaml.cs`, or `Program.cs` depending on project type:

```csharp
// Add this — one line, placed once at startup
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

Replace the using directive:

```csharp
// Remove
using ScanbotBarcodeSDK.MAUI;

// Add
using IronBarCode;
```

## Code Migration Examples

### MAUI Mobile: Camera Scanning

Scanbot's scanning call opens a full-screen camera UI and returns when the user scans a barcode or cancels. IronBarcode does not provide a camera UI — the replacement uses MAUI's `MediaPicker` to invoke the system camera, capture a photo, and pass the bytes to `BarcodeReader.Read()`.

**Before (Scanbot SDK):**

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

**After (IronBarcode):**

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

The UX change: the user sees the system camera screen rather than Scanbot's embedded viewfinder with a scan region overlay. For business applications — inventory, logistics, document workflows — the system camera is adequate. The [.NET MAUI barcode scanner tutorial](https://ironsoftware.com/csharp/barcode/get-started/net-maui-barcode-scanner-reader-tutorial/) covers the full pattern for both iOS and Android targets, including permission handling.

### MAUI Mobile: Scan Options

Scanbot's `BarcodeScannerConfiguration` controls which formats the scanner accepts and how the camera UI looks. IronBarcode's `BarcodeReaderOptions` controls how the image is analyzed — speed, multi-barcode detection, and similar processing parameters.

**Before (Scanbot SDK):**

```csharp
var configuration = new BarcodeScannerConfiguration
{
    BarcodeFormats = new[] { BarcodeFormat.Code128, BarcodeFormat.QrCode },
    FinderAspectRatio = new AspectRatio(1, 1),
    FlashEnabled = false,
    OrientationLockMode = OrientationLockMode.Portrait
};

var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(configuration);
```

**After (IronBarcode):**

```csharp
using IronBarCode;

// Options control reading behavior — format detection is automatic
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = false
};

// Combined with the MediaPicker capture pattern:
var photo = await MediaPicker.CapturePhotoAsync();
if (photo == null) return;

using var stream = await photo.OpenReadAsync();
using var ms = new MemoryStream();
await stream.CopyToAsync(ms);

var results = BarcodeReader.Read(ms.ToArray(), options);
```

Format specification is not needed — IronBarcode auto-detects all supported formats. The `BarcodeReaderOptions` parameters that directly correspond to Scanbot configuration are `Speed` (controls thoroughness vs. performance) and `ExpectMultipleBarcodes` (continue scanning after first match).

### Server-Side PDF Processing (New Capability)

This has no Scanbot equivalent — it is a net-new capability. With IronBarcode, the same package that runs in the MAUI app can extract barcodes from PDFs in a server-side ASP.NET Core endpoint:

```csharp
// NuGet: dotnet add package IronBarcode
// Works in ASP.NET Core, console apps, Azure Functions, Docker — not available in Scanbot at all
using IronBarCode;

// ASP.NET Core controller endpoint
[HttpPost("extract-barcodes")]
public async Task<IActionResult> ExtractBarcodes(IFormFile file)
{
    using var ms = new MemoryStream();
    await file.CopyToAsync(ms);

    // Reads all barcodes from all pages of the PDF
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

For a background job or Azure Function processing PDFs from blob storage:

```csharp
using IronBarCode;

// Azure Function or Worker Service batch processing
public void ProcessInvoicePdf(string filePath)
{
    var results = BarcodeReader.Read(filePath);
    foreach (var result in results)
        Console.WriteLine($"Page {result.PageNumber}: {result.Value} ({result.Format})");
}
```

IronBarcode handles multi-page PDFs natively — each result carries a `PageNumber` property indicating which page the barcode was found on. The [PDF barcode extraction guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) covers the full range of PDF reading options including page range selection and performance tuning for large documents.

### Handling Multiple Barcodes

**Before (Scanbot SDK):**

```csharp
var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(configuration);

if (result.Status == OperationResult.Ok)
{
    foreach (var barcode in result.Barcodes)
        Console.WriteLine($"{barcode.Format}: {barcode.Text}");
}
```

**After (IronBarcode):**

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};

var results = BarcodeReader.Read(imageBytes, options);
foreach (var result in results)
    Console.WriteLine($"{result.Format}: {result.Value}");
```

`ExpectMultipleBarcodes = true` tells IronBarcode to continue scanning the image after finding the first barcode. Without it, `BarcodeReader.Read()` returns on the first match — which is faster for single-barcode workflows.

### Result Property Names

The property mapping is direct. `barcode.Text` becomes `result.Value`, and `barcode.Format` stays `result.Format` (though it is now an `IronBarCode.BarcodeEncoding` enum rather than Scanbot's `BarcodeFormat` enum):

```csharp
// Before (Scanbot)
foreach (var barcode in result.Barcodes)
{
    string text = barcode.Text;
    string format = barcode.Format.ToString();
}

// After (IronBarcode)
foreach (var result in BarcodeReader.Read(imageBytes))
{
    string value = result.Value;
    string format = result.Format.ToString();
}
```

## API Mapping

| Scanbot SDK | IronBarcode |
|---|---|
| `ScanbotSDK.Initialize(new ScanbotSDKConfiguration { LicenseKey = "..." })` | `IronBarCode.License.LicenseKey = "key"` |
| `new BarcodeScannerConfiguration()` | `new BarcodeReaderOptions()` |
| `ScanbotBarcodeSDK.BarcodeScanner.Open(configuration)` | `BarcodeReader.Read(path / stream / bytes)` |
| `result.Status == OperationResult.Ok` | Check `results.Any()` or `results.FirstOrDefault() != null` |
| `result.Barcodes` | Return value of `BarcodeReader.Read()` |
| `barcode.Format` | `result.Format` (IronBarCode.BarcodeEncoding) |
| `barcode.Text` | `result.Value` |
| `BarcodeFormat.Code128` | `BarcodeEncoding.Code128` |
| `BarcodeFormat.QrCode` | `BarcodeEncoding.QRCode` |
| `BarcodeFormat.Ean13` | `BarcodeEncoding.EAN13` |
| `BarcodeScannerConfiguration.FinderAspectRatio` | No equivalent — image framing is handled by MediaPicker |
| `BarcodeScannerConfiguration.FlashEnabled` | No equivalent — use MediaPicker options |
| Camera required (no file/stream input) | File path, stream, byte array, PDF all accepted |
| iOS + Android MAUI only | All .NET platforms |

## Migration Checklist

Search your codebase for the following patterns. Every hit needs attention:

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

Work through each match:

- `ScanbotSDK.Initialize(...)` — replace with `IronBarCode.License.LicenseKey = "key"`
- `new BarcodeScannerConfiguration()` — replace with `new BarcodeReaderOptions()` where processing options are needed, or remove entirely if defaults are sufficient
- `ScanbotBarcodeSDK.BarcodeScanner.Open(configuration)` — replace with `MediaPicker.CapturePhotoAsync()` + stream copy + `BarcodeReader.Read(bytes)` in MAUI; replace with `BarcodeReader.Read(filePath)` or `BarcodeReader.Read(stream)` in server contexts
- `result.Status == OperationResult.Ok` — replace with `results.Any()` or a null check on `results.FirstOrDefault()`
- `result.Barcodes` — replace with the return value of `BarcodeReader.Read()`
- `barcode.Format` — replace with `result.Format`
- `barcode.Text` — replace with `result.Value`
- `BarcodeFormat.Code128` (and other format enum values) — replace with `BarcodeEncoding.Code128` (IronBarcode auto-detects by default — explicit format filtering is only needed for performance optimization)
- `using ScanbotBarcodeSDK.MAUI;` — replace with `using IronBarCode;`

After completing the above replacements, remove `ScanbotBarcodeSDK.MAUI` from the package references and verify the build succeeds on all target platforms including Windows if present.

## Common Migration Issues

### No Live Viewfinder

Scanbot's most visible feature is its camera UI: a real-time viewfinder embedded in the scanning experience with a scan region overlay, haptic feedback, and continuous barcode detection. IronBarcode does not replicate this. The `MediaPicker.CapturePhotoAsync()` pattern opens the system camera app, the user takes a photo, and control returns to your app for processing.

For most business applications — shipping logistics, asset tracking, document workflows — the system camera is sufficient. For consumer apps where the live overlay is central to the product experience, this is a genuine UX difference, not just an API detail. Evaluate whether that difference matters for your specific use case before committing to the migration.

### Camera Permissions

Scanbot's package handled camera permission requests as part of its UI flow. With `MediaPicker`, MAUI handles permissions through the standard mechanism — `AndroidManifest.xml` camera permission declarations and the `NSCameraUsageDescription` key in `Info.plist`. These are the same permissions any MAUI app using `MediaPicker` needs, and they are typically present in scaffolded MAUI project templates. If the app was previously using only Scanbot for camera access, verify these manifest entries exist.

### Windows Target Build Errors

If removing `ScanbotBarcodeSDK.MAUI` resolves build errors on the Windows MAUI target, that confirms the package was incompatible with the Windows build. IronBarcode does not have this issue — the single package resolves correctly across all MAUI targets. No `#if` guards or conditional package references are needed.

### Format Enum Namespace

Scanbot's `BarcodeFormat` enum (e.g., `BarcodeFormat.Code128`) and IronBarcode's `BarcodeEncoding` enum (e.g., `BarcodeEncoding.Code128`) contain similar members but are different types. Any code that stores or compares format values by enum type — rather than by string — needs the type updated. String comparisons (`barcode.Format.ToString() == "Code128"`) should be migrated to direct enum comparisons against `BarcodeEncoding` values.

## What You Gain

The migration converts a camera-only mobile scanning dependency into a general-purpose barcode processing library that runs wherever .NET runs. The immediate gains depend on what triggered the migration:

If the project is adding Windows MAUI support, the desktop targets now build without exclusions. If server-side PDF processing is the new requirement, `BarcodeReader.Read("invoice.pdf")` handles it in the same package already installed for mobile. If the annual Scanbot license renewal was the trigger, the one-time perpetual IronBarcode license eliminates the renewal obligation.

What does not transfer is Scanbot's real-time camera viewfinder. If that component is actively used and valued by your users, weigh that against the new requirements that drove the migration evaluation.

The Scanbot dependency is MAUI mobile only. Every use case that lands outside that box — every PDF, every server endpoint, every Windows desktop window — is territory where IronBarcode works and Scanbot does not.
