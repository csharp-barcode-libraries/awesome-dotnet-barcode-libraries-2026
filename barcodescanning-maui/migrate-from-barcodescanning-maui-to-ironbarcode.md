# Migrating from BarcodeScanning.Native.Maui to IronBarcode

This migration usually happens for one of three reasons: Windows MAUI support is needed, file or PDF processing was added to the requirements, or the iOS UPC-A 13-digit bug caused incorrect data in production. In all three cases the same structural change applies — the camera event pattern gets replaced by a `MediaPicker` capture followed by `BarcodeReader.Read()`.

For apps that also need server-side processing, IronBarcode handles both the mobile and server-side cases with the same API and the same package.

## Why Teams Migrate

**Windows MAUI support required.** BarcodeScanning.Native.Maui wraps iOS and Android native APIs. It has no Windows implementation and none planned. If your MAUI app targets Windows, you need a different approach.

**File or PDF input added.** BarcodeScanning.Native.Maui only accepts live camera frames. When users need to upload an image from their gallery, or when a server-side endpoint needs to extract barcodes from PDFs, the library offers nothing. IronBarcode reads from files, byte arrays, streams, and PDFs natively.

**iOS UPC-A data was wrong in production.** Apple's Vision framework returns 13 digits for UPC-A barcodes (EAN-13 encoding). BarcodeScanning.Native.Maui passes this through uncorrected. If UPC-A codes were being stored with a leading zero, inventory records, point-of-sale lookups, or supply chain integrations may have been silently broken. IronBarcode returns the correct 12-digit UPC-A value without manual normalization.

**PDF417 scanning was unreliable.** The library's GitHub issues document PDF417 as "very problematic — most scans never occur." For shipping labels, driver's licenses, and boarding passes, this is a blocker.

**Generation was needed.** BarcodeScanning.Native.Maui cannot generate barcodes. IronBarcode generates Code128, QR, DataMatrix, and other formats.

## Quick Start

### Step 1: Remove BarcodeScanning.Native.Maui

```bash
dotnet remove package BarcodeScanning.Native.Maui
```

### Step 2: Install IronBarcode

```bash
# NuGet: dotnet add package IronBarcode
dotnet add package IronBarcode
```

### Step 3: Replace Namespace and Add License

Remove the BarcodeScanning namespace:

```csharp
// Remove
using BarcodeScanning;
```

Add IronBarCode:

```csharp
using IronBarCode;
```

Add license initialization at startup — in `MauiProgram.cs` or `App.xaml.cs`:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

### Step 4: Update MAUI Permissions

BarcodeScanning.Native.Maui adds camera permissions automatically. With IronBarcode using `MediaPicker`, you need the standard MAUI camera permissions in `AndroidManifest.xml` and `Info.plist`. These are the same permissions any MAUI app needs for `MediaPicker.CapturePhotoAsync()` and are typically already present.

## Code Migration Examples

### Camera Scanning: CameraView to MediaPicker

The `CameraView` control provided a real-time viewfinder with continuous frame detection. The IronBarcode replacement uses MAUI's `MediaPicker` to open the system camera, capture a photo, and process the resulting image.

**Before (BarcodeScanning.Native.Maui) — XAML:**

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

**Before (BarcodeScanning.Native.Maui) — code-behind:**

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

**After (IronBarcode) — XAML:**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui">
    <StackLayout>
        <Button Text="Scan Barcode" Clicked="ScanButton_Clicked" />
        <Label x:Name="ResultLabel" Text="Tap to scan..." />
    </StackLayout>
</ContentPage>
```

**After (IronBarcode) — code-behind:**

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

This code runs on iOS, Android, and Windows MAUI without any platform-specific branching. The difference in UX is that the user sees the platform's native camera screen rather than an embedded in-app viewfinder. For most business applications, this is acceptable; for consumer apps that want a custom scanning UI with a live overlay, see the note in the Common Migration Issues section.

### Handling Multiple Barcodes Per Scan

**Before (BarcodeScanning.Native.Maui):**

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

**After (IronBarcode):**

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

`ExpectMultipleBarcodes = true` tells the reader to keep scanning after finding the first barcode. Without this option, it returns on the first match (faster for single-barcode scenarios).

### iOS UPC-A Fix: Remove the Normalization Workaround

If your codebase has the UPC-A leading-zero workaround, remove it. IronBarcode returns the correct 12-digit value without manual intervention.

**Before (BarcodeScanning.Native.Maui) — workaround in place:**

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

**After (IronBarcode) — no workaround needed:**

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

Delete any grep matches for `BarcodeFormat.UPC_A` paired with `Substring(1)` — that code is now dead.

### Adding File/PDF Support (Net-New Capability)

BarcodeScanning.Native.Maui has no equivalent for this. If file or PDF reading is a new requirement:

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

### Windows MAUI: Same Code, No Changes

With BarcodeScanning.Native.Maui, Windows required a separate implementation or a compile-time exclusion. With IronBarcode, the `MediaPicker` pattern above works on Windows MAUI without modification. No `#if WINDOWS` blocks, no conditional dependency loading, no stub implementations.

### Server-Side Barcode Processing

If your application has a backend ASP.NET API that also needs barcode processing, the same `BarcodeReader.Read()` call runs there:

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

### Generating Barcodes (Net-New Capability)

BarcodeScanning.Native.Maui has no generation API. IronBarcode generates:

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

## Common Migration Issues

### CameraView Had a Live Viewfinder

The `CameraView` control embedded a real-time camera preview directly in the MAUI page. Users could see the camera feed, aim at a barcode, and the detection happened automatically — no button press, no camera shutter.

`MediaPicker.CapturePhotoAsync()` shows the platform camera screen instead. The user opens the camera, takes a photo, and returns to the app. This is appropriate for most business workflows. For consumer apps that specifically want the continuous-scanning live preview experience, you can access camera frames through MAUI's camera APIs and pass each frame's bytes to `BarcodeReader.Read()`:

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

This requires wiring up a camera frame source, which is more work than the `CameraView` control. Evaluate whether a live preview is genuinely needed or whether the system camera UI is sufficient for your use case.

### e.BarcodeResults Returns Multiple Per Frame

BarcodeScanning.Native.Maui fires `OnDetectionFinished` with all barcodes detected in a single frame via `e.BarcodeResults`. The iteration pattern is the same in IronBarcode — `BarcodeReader.Read()` returns an enumerable:

```csharp
// Before
foreach (var barcode in e.BarcodeResults)
    Console.WriteLine(barcode.DisplayValue);

// After
var results = BarcodeReader.Read(imageBytes);
foreach (var result in results)
    Console.WriteLine(result.Value);
```

The property name changes from `DisplayValue` to `Value`, and the format property changes from `barcode.BarcodeFormat` (library enum) to `result.Format` (IronBarCode `BarcodeEncoding` enum).

### Thread Marshaling for UI Updates

BarcodeScanning.Native.Maui fires `OnDetectionFinished` on a background thread, so UI updates require `MainThread.BeginInvokeOnMainThread()`. With the `MediaPicker` + `async` pattern, the continuation after `await` returns on the calling context, which is typically the main thread — so `MainThread.BeginInvokeOnMainThread()` is usually not needed for the result display.

## Migration Checklist

Run these searches to find all BarcodeScanning.Native.Maui usage:

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

Work through each hit:

- `using BarcodeScanning` — replace with `using IronBarCode;`
- `scanner:CameraView` in XAML — remove the control and its namespace declaration; replace with a `Button` that triggers `MediaPicker.CapturePhotoAsync()`
- `OnDetectionFinished="..."` event wiring — remove; replace with `Clicked` handler on the button
- `OnDetectionFinishedEventArgs` parameter — remove; replace with `BarcodeReader.Read(bytes)` return value
- `e.BarcodeResults` — replace with the return value of `BarcodeReader.Read()`
- `barcode.DisplayValue` — replace with `result.Value`
- `barcode.BarcodeFormat` — replace with `result.Format`
- `BarcodeFormat.UPC_A` + `Substring(1)` workaround — delete entirely
- `scanner:` XML namespace declarations in XAML — remove

## API Reference

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

The core of this migration is replacing the camera-event pattern with a `MediaPicker` capture and a `BarcodeReader.Read()` call. The result runs on Windows, returns accurate UPC-A values without workarounds, and extends naturally to file and PDF input when requirements grow in that direction.
