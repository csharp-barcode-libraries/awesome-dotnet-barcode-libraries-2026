# Migrating from BarcodeScanning.Native.Maui to IronBarcode

This guide provides a complete migration path from BarcodeScanning.Native.Maui to IronBarcode, covering the camera event pattern replacement, namespace changes, code migration examples, and the handling of scenarios that BarcodeScanning.Native.Maui cannot address — Windows MAUI, file and PDF input, server-side processing, and barcode generation.

## Why Migrate from BarcodeScanning.MAUI

Teams migrating from BarcodeScanning.Native.Maui report these triggers:

**Windows MAUI Target Required:** BarcodeScanning.Native.Maui wraps iOS and Android native APIs. It has no Windows implementation and none is planned. If your MAUI app targets Windows alongside iOS and Android, you need a library that works on all three targets without platform-specific branching.

**File or PDF Input Added to Requirements:** BarcodeScanning.Native.Maui only accepts live camera frames. When users need to upload an image from their gallery, or when a server-side endpoint needs to extract barcodes from PDFs, the library has no code path to offer. Any file or PDF barcode scenario requires a different tool.

**iOS UPC-A Data Was Wrong in Production:** Apple's Vision framework returns 13 digits for UPC-A barcodes (EAN-13 encoding). BarcodeScanning.Native.Maui passes this through uncorrected. If UPC-A codes were being stored with a leading zero, inventory records, point-of-sale lookups, or supply chain integrations may have been silently broken. IronBarcode returns the correct 12-digit UPC-A value without manual normalization.

**PDF417 Scanning Was Unreliable:** The library's own GitHub issues document PDF417 as "very problematic — most scans never occur." For shipping labels, driver's licenses, and boarding passes, this is a direct blocker.

**Generation Was Needed:** BarcodeScanning.Native.Maui cannot generate barcodes. IronBarcode generates Code128, QR, DataMatrix, and other formats as image files or byte arrays.

**Server-Side Processing Introduced:** BarcodeScanning.Native.Maui is a camera UI control — it cannot run in a server process. When server-side barcode reading is required alongside mobile scanning, IronBarcode covers both sides with the same package and the same API.

### The Fundamental Problem

BarcodeScanning.Native.Maui ties your barcode reading entirely to the live camera event model. The moment any requirement falls outside that model, the library offers nothing:

```csharp
// BarcodeScanning.Native.Maui: only works inside this event, only on iOS/Android,
// only with live camera frames — no file, no PDF, no Windows, no server
private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArgs e)
{
    var barcode = e.BarcodeResults.FirstOrDefault();
    if (barcode != null)
        ResultLabel.Text = barcode.DisplayValue; // may return 13-digit UPC-A on iOS
}
```

IronBarcode accepts any data input — camera capture, file, PDF, byte array — and runs on every platform:

```csharp
// IronBarcode: works on iOS, Android, Windows, macOS, and server
// NuGet: dotnet add package IronBarcode
using IronBarCode;

private async void ScanBarcodeButton_Clicked(object sender, EventArgs e)
{
    var photo = await MediaPicker.CapturePhotoAsync();
    if (photo == null) return;

    using var stream = await photo.OpenReadAsync();
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);

    var results = BarcodeReader.Read(ms.ToArray());
    ResultLabel.Text = results.FirstOrDefault()?.Value ?? "No barcode found";
}
```

## IronBarcode vs BarcodeScanning.MAUI: Feature Comparison

| Feature | BarcodeScanning.MAUI | IronBarcode |
|---------|----------------------|-------------|
| **Live camera frame reading** | Yes — CameraView control | No (use MediaPicker to capture, then read) |
| **In-app camera viewfinder** | Yes — real-time continuous | No — uses system camera UI via MediaPicker |
| **Read from image file** | No | Yes — `BarcodeReader.Read(path)` |
| **Read from byte array** | No | Yes — `BarcodeReader.Read(bytes)` |
| **Read from stream** | No | Yes — `BarcodeReader.Read(stream)` |
| **Read from PDF** | No | Yes — `BarcodeReader.Read(pdf)` |
| **Barcode generation** | No | Yes — `BarcodeWriter` + `QRCodeWriter` |
| **Windows MAUI support** | No | Yes |
| **iOS MAUI support** | Yes | Yes |
| **Android MAUI support** | Yes | Yes |
| **macOS MAUI support** | Not documented | Yes |
| **Server-side / ASP.NET** | No | Yes |
| **Docker / Azure / AWS Lambda** | No | Yes |
| **iOS UPC-A accuracy** | Returns 13 digits (bug), requires manual normalization | Returns correct 12-digit UPC-A |
| **PDF417 reliability** | "Most scans never occur" (GitHub issues) | Supported |
| **Multi-barcode detection** | Yes (multiple per frame via `e.BarcodeResults`) | Yes (`ExpectMultipleBarcodes` option) |
| **Reading speed control** | None | `ReadingSpeed.Balanced` / `Faster` / `Slower` |
| **License** | MIT (open source, free) | Commercial — Lite $749, Plus $1,499, Professional $2,999, Unlimited $5,999 |
| **.NET Framework support** | No (MAUI only) | Yes — .NET Framework 4.6.2+ |

## Quick Start: BarcodeScanning.MAUI to IronBarcode Migration

### Step 1: Replace NuGet Package

Remove BarcodeScanning.Native.Maui:

```bash
dotnet remove package BarcodeScanning.Native.Maui
```

Install IronBarcode:

```bash
dotnet add package IronBarcode
```

### Step 2: Update Namespaces

Remove the BarcodeScanning namespace from all files:

```csharp
// Remove
using BarcodeScanning;
```

Add the IronBarCode namespace:

```csharp
// Add
using IronBarCode;
```

In XAML files, remove the `scanner:` XML namespace declaration:

```xml
<!-- Remove this line from ContentPage attributes -->
xmlns:scanner="clr-namespace:BarcodeScanning;assembly=BarcodeScanning.Native.Maui"
```

### Step 3: Initialize License

Add license initialization at application startup — in `MauiProgram.cs` or `App.xaml.cs`:

```csharp
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

## Code Migration Examples

### Camera Scanning: CameraView to MediaPicker

The `CameraView` control provided a real-time viewfinder with continuous frame detection. The IronBarcode replacement uses MAUI's `MediaPicker` to open the system camera, capture a photo, and process the resulting image.

**BarcodeScanning.MAUI Approach — XAML:**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:scanner="clr-namespace:BarcodeScanning;assembly=BarcodeScanning.Native.Maui">
    <StackLayout>
        <scanner:CameraView x:Name="CameraView"
                            OnDetectionFinished="OnBarcodeDetected"
                            CameraEnabled="True"
                            BarcodeFormats="All"
                            VerticalOptions="FillAndExpand" />
        <Label x:Name="ResultLabel" Text="Waiting for scan..." />
    </StackLayout>
</ContentPage>
```

**BarcodeScanning.MAUI Approach — code-behind:**

```csharp
using BarcodeScanning;

private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArgs e)
{
    var barcode = e.BarcodeResults.FirstOrDefault();
    if (barcode != null)
        MainThread.BeginInvokeOnMainThread(() =>
            ResultLabel.Text = barcode.DisplayValue);
}
```

**IronBarcode Approach — XAML:**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui">
    <StackLayout>
        <Button Text="Scan Barcode" Clicked="ScanButton_Clicked" />
        <Label x:Name="ResultLabel" Text="Tap to scan..." />
    </StackLayout>
</ContentPage>
```

**IronBarcode Approach — code-behind:**

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
    ResultLabel.Text = first?.Value ?? "No barcode found";
}
```

This code runs on iOS, Android, and Windows MAUI without any platform-specific branching. The user experience changes from a live in-app viewfinder to the platform's native camera screen — appropriate for most business applications. The [IronBarcode MAUI reading guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) covers additional configuration options.

### Handling Multiple Barcodes Per Scan

**BarcodeScanning.MAUI Approach:**

```csharp
private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArgs e)
{
    foreach (var barcode in e.BarcodeResults)
    {
        MainThread.BeginInvokeOnMainThread(() =>
            Console.WriteLine($"Found: {barcode.DisplayValue}"));
    }
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

private async void ScanButton_Clicked(object sender, EventArgs e)
{
    var photo = await MediaPicker.CapturePhotoAsync();
    if (photo == null) return;

    using var stream = await photo.OpenReadAsync();
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);

    var options = new BarcodeReaderOptions
    {
        Speed = ReadingSpeed.Balanced,
        ExpectMultipleBarcodes = true
    };

    var results = BarcodeReader.Read(ms.ToArray(), options);
    foreach (var result in results)
        Console.WriteLine($"{result.Format}: {result.Value}");
}
```

`ExpectMultipleBarcodes = true` tells the reader to continue scanning after finding the first barcode. Without this option, the call returns on the first match, which is faster for single-barcode scenarios.

### iOS UPC-A Fix: Remove the Normalization Workaround

If your codebase has the UPC-A leading-zero workaround, remove it entirely. IronBarcode returns the correct 12-digit value without any manual intervention.

**BarcodeScanning.MAUI Approach — workaround in place:**

```csharp
private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArgs e)
{
    var barcode = e.BarcodeResults.FirstOrDefault();
    if (barcode == null) return;

    var value = barcode.DisplayValue;

    // Workaround: Apple Vision returns 13 digits for UPC-A
    if (barcode.BarcodeFormat == BarcodeFormat.UPC_A && value.Length == 13)
        value = value.Substring(1);

    ProcessBarcode(value, barcode.BarcodeFormat.ToString());
}
```

**IronBarcode Approach — no workaround needed:**

```csharp
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
    if (first == null) return;

    // result.Value is the correct 12-digit UPC-A — no normalization needed
    ProcessBarcode(first.Value, first.Format.ToString());
}
```

Delete any matches for `BarcodeFormat.UPC_A` paired with `Substring(1)` — that code is dead after migration.

### Adding File and PDF Support

BarcodeScanning.Native.Maui has no equivalent for file or PDF input. If this is a new requirement being fulfilled at migration time:

**BarcodeScanning.MAUI Approach:**

```csharp
// No equivalent exists — BarcodeScanning.Native.Maui cannot read from files or PDFs
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Read from a file the user picked with FilePicker
private async void ReadFileButton_Clicked(object sender, EventArgs e)
{
    var file = await FilePicker.PickAsync(new PickOptions
    {
        PickerTitle = "Select image or PDF"
    });
    if (file == null) return;

    var results = BarcodeReader.Read(file.FullPath);
    foreach (var result in results)
        ResultLabel.Text += $"\n{result.Format}: {result.Value}";
}

// Read barcodes from a PDF directly — no intermediate image step
private void ReadPdfBarcodes()
{
    var results = BarcodeReader.Read("shipment-manifest.pdf");
    foreach (var result in results)
        Console.WriteLine($"{result.Format}: {result.Value}");
}
```

The [IronBarcode PDF reading documentation](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) covers multi-page PDF support and page range selection.

### Server-Side Barcode Processing

If your application has a backend ASP.NET API that also needs barcode processing, the same `BarcodeReader.Read()` call runs there without modification. BarcodeScanning.Native.Maui has no server-side equivalent.

**BarcodeScanning.MAUI Approach:**

```csharp
// No equivalent exists — BarcodeScanning.Native.Maui is a camera UI control
// and cannot run in a server process
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// ASP.NET endpoint — reads barcodes from an uploaded file
[HttpPost("scan")]
public async Task<IActionResult> ScanBarcode(IFormFile file)
{
    using var ms = new MemoryStream();
    await file.CopyToAsync(ms);

    var results = BarcodeReader.Read(ms.ToArray());
    var values = results.Select(r => new { r.Value, Format = r.Format.ToString() });
    return Ok(values);
}
```

The same package, the same API, the same behavior — on mobile and server.

### Generating Barcodes

BarcodeScanning.Native.Maui has no generation API. IronBarcode generates multiple formats.

**BarcodeScanning.MAUI Approach:**

```csharp
// No equivalent exists — BarcodeScanning.Native.Maui cannot generate barcodes
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// QR code
QRCodeWriter.CreateQrCode("https://example.com/product/12345", 500)
    .SaveAsPng("qr.png");

// Code128 barcode sized to specific dimensions
BarcodeWriter.CreateBarcode("ITEM-98765", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("label.png");

// Get bytes for returning from an API or storing in a database
byte[] barcodeBytes = BarcodeWriter.CreateBarcode("ITEM-98765", BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

The [IronBarcode generation documentation](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/) covers all supported formats and styling options.

## BarcodeScanning.MAUI API to IronBarcode Mapping Reference

| BarcodeScanning.Native.Maui | IronBarcode |
|-----------------------------|-------------|
| `CameraView` XAML control | Remove — use `Button` + `MediaPicker.CapturePhotoAsync()` |
| `OnDetectionFinished` event | `BarcodeReader.Read(imageBytes)` return value |
| `OnDetectionFinishedEventArgs e` | IEnumerable result from `BarcodeReader.Read()` |
| `e.BarcodeResults` | Return value of `BarcodeReader.Read()` |
| `e.BarcodeResults.FirstOrDefault()` | `results.FirstOrDefault()` |
| `barcode.DisplayValue` | `result.Value` |
| `barcode.BarcodeFormat` | `result.Format` |
| `BarcodeFormats="All"` | Auto-detected — no configuration needed |
| `CameraEnabled="True"` | `await MediaPicker.CapturePhotoAsync()` |
| iOS + Android only | iOS, Android, Windows, macOS MAUI |
| No file input | `BarcodeReader.Read(filePath)` |
| No PDF input | `BarcodeReader.Read("document.pdf")` |
| No generation | `BarcodeWriter.CreateBarcode()` / `QRCodeWriter.CreateQrCode()` |
| iOS UPC-A returns 13 digits | Returns correct 12 digits — no normalization needed |
| PDF417 unreliable | Supported |

## Common Migration Issues and Solutions

### Issue 1: Live Viewfinder Experience Lost

**BarcodeScanning.MAUI:** The `CameraView` control embedded a real-time camera preview directly in the MAUI page. Users could see the camera feed and aim at a barcode — detection happened automatically with no button press.

**Solution:** `MediaPicker.CapturePhotoAsync()` shows the platform camera screen instead. For most business workflows this is acceptable. For consumer apps that require a continuous live preview, camera frames can be passed directly to `BarcodeReader.Read()`:

```csharp
using IronBarCode;

// Continuous scanning: pass camera frame bytes to BarcodeReader.Read()
// (frame capture depends on your MAUI camera frame source)
private void ProcessCameraFrame(byte[] frameBytes)
{
    var results = BarcodeReader.Read(frameBytes);
    if (results.Any())
    {
        var first = results.First();
        MainThread.BeginInvokeOnMainThread(() =>
            ResultLabel.Text = first.Value);
    }
}
```

This requires wiring up a camera frame source separately from IronBarcode. Evaluate whether a live preview is genuinely required or whether the system camera UI is sufficient before taking this path.

### Issue 2: e.BarcodeResults Property Name and Enum Changes

**BarcodeScanning.MAUI:** `barcode.DisplayValue` returns the decoded string; `barcode.BarcodeFormat` returns a BarcodeScanning library enum value.

**Solution:** Replace `DisplayValue` with `result.Value` and `barcode.BarcodeFormat` with `result.Format`. The iteration pattern is the same:

```csharp
// Before
foreach (var barcode in e.BarcodeResults)
    Console.WriteLine(barcode.DisplayValue);

// After
var results = BarcodeReader.Read(imageBytes);
foreach (var result in results)
    Console.WriteLine(result.Value);
```

### Issue 3: Thread Marshaling for UI Updates

**BarcodeScanning.MAUI:** `OnDetectionFinished` fires on a background thread, so all UI updates require `MainThread.BeginInvokeOnMainThread()`.

**Solution:** With the `MediaPicker` + `async` pattern, the continuation after `await` returns on the calling context — typically the main thread. The `MainThread.BeginInvokeOnMainThread()` wrappers around result display can generally be removed, simplifying the handler code.

### Issue 4: MAUI Camera Permissions

**BarcodeScanning.MAUI:** The package adds camera permissions to `AndroidManifest.xml` and `Info.plist` automatically as part of its setup.

**Solution:** With IronBarcode using `MediaPicker`, the standard MAUI camera permissions must be present manually. These are the same permissions any MAUI app needs for `MediaPicker.CapturePhotoAsync()` and are typically already in place. Verify that `android.permission.CAMERA` is declared in `AndroidManifest.xml` and `NSCameraUsageDescription` is set in `Info.plist` before testing on device.

## BarcodeScanning.MAUI Migration Checklist

### Pre-Migration Tasks

Run these searches to find all BarcodeScanning.Native.Maui usage before making any changes:

```bash
grep -rn "using BarcodeScanning" --include="*.cs" .
grep -rn "using BarcodeScanning" --include="*.xaml" .
grep -rn "CameraView" --include="*.cs" .
grep -rn "CameraView" --include="*.xaml" .
grep -rn "OnDetectionFinished" --include="*.cs" .
grep -rn "OnDetectionFinishedEventArgs" --include="*.cs" .
grep -rn "e\.BarcodeResults" --include="*.cs" .
grep -rn "DisplayValue" --include="*.cs" .
grep -rn "BarcodeFormat\.UPC_A" --include="*.cs" .
grep -rn "scanner:" --include="*.xaml" .
```

Document each hit. Note which files contain `CameraView` XAML usage (require XAML changes) versus which contain only code-behind changes. Identify any UPC-A normalization workarounds that must be deleted after migration.

### Code Update Tasks

1. Remove `BarcodeScanning.Native.Maui` NuGet package
2. Install `IronBarcode` NuGet package
3. Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` in `MauiProgram.cs` or `App.xaml.cs`
4. Replace `using BarcodeScanning;` with `using IronBarCode;` in all `.cs` files
5. Remove `xmlns:scanner="..."` namespace declarations from all XAML files
6. Replace `scanner:CameraView` controls in XAML with a `Button` triggering `MediaPicker.CapturePhotoAsync()`
7. Remove `OnDetectionFinished="..."` event wiring from XAML
8. Replace `OnDetectionFinished` event handlers with `async` button click handlers using `BarcodeReader.Read()`
9. Replace `barcode.DisplayValue` with `result.Value` throughout
10. Replace `barcode.BarcodeFormat` with `result.Format` throughout
11. Delete all `BarcodeFormat.UPC_A` + `Substring(1)` normalization workarounds
12. Add `ExpectMultipleBarcodes = true` to `BarcodeReaderOptions` where multi-barcode detection was previously relying on `e.BarcodeResults` returning multiple items
13. Remove `MainThread.BeginInvokeOnMainThread()` wrappers from result display code where the async pattern makes them unnecessary
14. Verify `AndroidManifest.xml` and `Info.plist` camera permissions are present

### Post-Migration Testing

- Verify iOS barcode scanning works and UPC-A values are returned as 12-digit strings without leading zeros
- Verify Android barcode scanning produces correct values for all formats used in the application
- Verify Windows MAUI barcode scanning works if Windows is a build target
- Test PDF417 scanning against real shipping labels, driver's licenses, or boarding passes if these are used
- Test multi-barcode scenarios with `ExpectMultipleBarcodes = true` and confirm all barcodes in an image are returned
- Verify file picker scanning (`BarcodeReader.Read(filePath)`) works on all MAUI targets
- Verify PDF barcode reading if this was a new capability added alongside migration
- Confirm server-side `BarcodeReader.Read()` produces correct results if a backend component was added
- Run any existing automated tests and compare barcode value outputs to pre-migration baselines

## Key Benefits of Migrating to IronBarcode

**Full Windows MAUI Support:** IronBarcode runs on all four MAUI targets — iOS, Android, Windows, and macOS — using the same code and the same package. No platform-specific barcode implementation is needed for Windows, and no `#if WINDOWS` blocks are required in application code.

**Any Input Source:** `BarcodeReader.Read()` accepts file paths, byte arrays, streams, and PDF documents. Any barcode scenario — camera capture, file upload, gallery image, server-side PDF processing — uses the same static method with the same result type.

**Correct UPC-A Values:** IronBarcode returns the correct 12-digit UPC-A value on iOS without any normalization code in the application. Historical UPC-A data stored with a leading zero due to BarcodeScanning.Native.Maui's behavior does not affect the accuracy of values read after migration.

**Reliable PDF417:** PDF417 is fully supported and reads reliably. Shipping labels, driver's licenses, and boarding passes scan without the "most scans never occur" limitation documented in BarcodeScanning.Native.Maui's GitHub issues.

**Barcode Generation:** `BarcodeWriter.CreateBarcode()` and `QRCodeWriter.CreateQrCode()` generate Code128, QR, DataMatrix, and other formats as PNG files or byte arrays. Generation and reading are available from the same package with no additional dependency.

**Server-Side Deployment:** The same `BarcodeReader.Read()` call runs in ASP.NET, Azure Functions, Docker containers, and AWS Lambda. Mobile and server barcode logic can share the same API, the same format support, and the same result type without maintaining two separate barcode implementations.
