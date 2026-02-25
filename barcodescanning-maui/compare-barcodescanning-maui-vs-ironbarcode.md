BarcodeScanning.Native.Maui is a camera plugin, not a barcode library. If your app runs on Windows, processes uploaded files, or reads barcodes from PDFs, it's the wrong tool — and the README doesn't make that obvious upfront. The package page describes it as a "barcode scanning library for .NET MAUI," which is technically accurate but omits the critical constraint: it is a `CameraView` control that fires an event when a barcode enters a live camera frame. That's the complete feature set.

This comparison is honest about that distinction. BarcodeScanning.Native.Maui does what it does — live camera scanning on iOS and Android — reasonably well for a free, open-source package. The comparison is about scope, not quality. If your use case fits inside "live camera, iOS/Android only, no file processing," BarcodeScanning.Native.Maui is a reasonable choice. The moment requirements step outside that box, you need a different tool.

## What BarcodeScanning.Native.Maui Actually Is

BarcodeScanning.Native.Maui wraps the native camera barcode detection APIs on iOS (Apple's Vision framework) and Android (ML Kit) into a MAUI `CameraView` control. You drop the control into a XAML page, wire up an event, and the library fires that event each time a barcode is detected in the live camera feed.

```xml
<!-- BarcodeScanning.Native.Maui: add the CameraView to a page -->
<scanner:CameraView x:Name="CameraView"
                    OnDetectionFinished="OnBarcodeDetected"
                    CameraEnabled="True"
                    BarcodeFormats="All" />
```

```csharp
private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArgs e)
{
    var barcode = e.BarcodeResults.FirstOrDefault();
    if (barcode != null)
        ResultLabel.Text = barcode.DisplayValue;
}
```

The library provides `CameraView`, `OnDetectionFinished`, `OnDetectionFinishedEventArgs`, and `BarcodeResult` with `DisplayValue` and `BarcodeFormat`. That is the entire public surface area relevant to application code. There is no `ReadFromFile()`, no `ReadFromBytes()`, no `ReadFromStream()`, and no `GenerateBarcode()`.

## Where It Falls Short

### No Windows MAUI Support

BarcodeScanning.Native.Maui does not support the Windows target in MAUI applications. This is not a planned feature — it is a structural limitation because the library wraps iOS-specific and Android-specific native camera APIs. Windows has neither.

For a MAUI application targeting iOS, Android, and Windows — a standard scenario — BarcodeScanning.Native.Maui cannot be the barcode solution. You either implement platform-specific code for Windows or switch to a library that works cross-platform.

### iOS UPC-A Returns 13 Digits Instead of 12

UPC-A is a 12-digit barcode format. On iOS, BarcodeScanning.Native.Maui's underlying detection (Apple Vision) returns 13 digits — a leading zero is prepended, matching EAN-13 encoding. The library passes this raw value through without correction.

The workaround documented in the library's GitHub issues:

```csharp
// Workaround required for iOS UPC-A — strip the leading zero manually
private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArgs e)
{
    var barcode = e.BarcodeResults.FirstOrDefault();
    if (barcode == null) return;

    var value = barcode.DisplayValue;
    if (barcode.BarcodeFormat == BarcodeFormat.UPC_A && value.Length == 13)
        value = value.Substring(1); // strip Apple's prepended leading zero

    ProcessBarcode(value);
}
```

This workaround silently breaks EAN-13 barcodes that happen to start with 0 if you forget to check the format first. It also requires iOS-specific branching if your handler runs on multiple platforms. If UPC-A data accuracy matters for inventory, point-of-sale, or supply chain workflows, this is a production bug waiting for conditions to surface it.

### PDF417 Reliability

The library's own GitHub issue tracker documents PDF417 scanning as "very problematic — most scans never occur." PDF417 is widely used in shipping labels, driver's licenses, and boarding passes. If your application needs to scan these documents reliably, the known issue is a direct blocker.

### No File, Stream, or PDF Input

The library only accepts live camera frames. There is no API for:

- Reading a barcode from an image file the user selected via file picker
- Decoding a barcode from bytes received from an HTTP endpoint
- Extracting barcodes from a PDF document
- Processing a camera-captured image after the fact (as opposed to in real time)

If a user uploads a photo of a barcode instead of scanning it live, or your server-side code needs to process PDFs containing barcodes, BarcodeScanning.Native.Maui offers nothing.

## What IronBarcode Does Instead

IronBarcode is a generation and reading library, not a camera control. On MAUI, it integrates with the system camera through `MediaPicker` — the same `MediaPicker` that MAUI apps already use for photo selection. The user sees the platform camera UI, takes a photo, and IronBarcode processes the resulting image.

```csharp
// IronBarcode in MAUI: capture a photo, then read barcodes from it
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
    foreach (var result in results)
        ResultLabel.Text = result.Value;
}
```

This code runs identically on iOS, Android, and Windows MAUI. No platform-specific branching, no format-specific workarounds, no Windows exclusion.

For applications that also process uploaded files or PDFs:

```csharp
using IronBarCode;

// Read barcodes from a file the user selected
var file = await FilePicker.PickAsync();
if (file != null)
{
    var results = BarcodeReader.Read(file.FullPath);
    foreach (var result in results)
        Console.WriteLine($"{result.Format}: {result.Value}");
}

// Read barcodes directly from a PDF — no image extraction step needed
var pdfResults = BarcodeReader.Read("shipment-manifest.pdf");

// Multiple barcodes in one image
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};
var multiResults = BarcodeReader.Read("warehouse-label.png", options);
```

IronBarcode also generates barcodes, which BarcodeScanning.Native.Maui cannot do:

```csharp
using IronBarCode;

// Generate a QR code
QRCodeWriter.CreateQrCode("https://example.com", 500)
    .SaveAsPng("qr.png");

// Generate a Code128 barcode
BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("barcode.png");
```

## Feature Comparison

| Feature | BarcodeScanning.Native.Maui | IronBarcode |
|---------|----------------------------|-------------|
| **Live camera scanning** | Yes — CameraView control | No (use MediaPicker to capture, then read) |
| **In-app camera preview** | Yes — real-time viewfinder | No — uses system camera UI via MediaPicker |
| **Barcode reading from image files** | No | Yes — BarcodeReader.Read(path) |
| **Barcode reading from byte arrays** | No | Yes — BarcodeReader.Read(bytes) |
| **Barcode reading from streams** | No | Yes — BarcodeReader.Read(stream) |
| **Barcode reading from PDFs** | No | Yes — BarcodeReader.Read(pdf) |
| **Barcode generation** | No | Yes — BarcodeWriter + QRCodeWriter |
| **Windows MAUI support** | No | Yes |
| **iOS MAUI support** | Yes | Yes |
| **Android MAUI support** | Yes | Yes |
| **macOS MAUI support** | Not documented | Yes |
| **iOS UPC-A accuracy** | Returns 13 digits (bug), requires manual normalization | Returns correct 12-digit UPC-A |
| **PDF417 reliability** | "Most scans never occur" (GitHub issues) | Supported |
| **Server-side / ASP.NET use** | No | Yes |
| **Docker / Azure / AWS Lambda** | No | Yes |
| **Multi-barcode detection** | Yes (multiple per frame via e.BarcodeResults) | Yes (ExpectMultipleBarcodes option) |
| **ReadingSpeed control** | None | ReadingSpeed.Balanced / Faster / Slower |
| **License** | MIT (open source) | Commercial — Lite $749, Plus $1,499, Professional $2,999, Unlimited $5,999 |
| **.NET Framework support** | No (MAUI only) | Yes — .NET Framework 4.6.2+ |

## API Mapping Reference

| BarcodeScanning.Native.Maui | IronBarcode |
|-----------------------------|-------------|
| `CameraView` XAML control | No camera control — use `MediaPicker.CapturePhotoAsync()` to capture |
| `OnDetectionFinished` event | `BarcodeReader.Read(imageBytes)` |
| `e.BarcodeResults` | Return value of `BarcodeReader.Read()` (IEnumerable) |
| `e.BarcodeResults.FirstOrDefault()` | `results.FirstOrDefault()` |
| `barcode.DisplayValue` | `result.Value` |
| `barcode.BarcodeFormat` | `result.Format` |
| `BarcodeFormats="All"` | Auto-detected — no configuration needed for multi-format |
| `CameraEnabled="True"` | `MediaPicker.CapturePhotoAsync()` call |
| iOS + Android only | iOS, Android, Windows, macOS (MAUI) + ASP.NET, desktop |
| Camera frames only | Files, byte arrays, streams, PDFs |
| No file/PDF API | `BarcodeReader.Read(path)` — accepts image files and PDFs |
| No Windows MAUI | Full Windows MAUI support |
| No generation API | `BarcodeWriter.CreateBarcode()` + `QRCodeWriter.CreateQrCode()` |

## When to Use Which

Be direct about this: BarcodeScanning.Native.Maui's genuine strength is the live camera preview experience. The `CameraView` control gives you a real-time viewfinder embedded in your MAUI page, with barcode detection firing continuously as the user moves the camera. For a consumer iOS/Android app where "point and scan" is the primary UX pattern and users expect a live preview — BarcodeScanning.Native.Maui handles that use case with a thin API surface and no licensing cost.

IronBarcode's domain is different:

- **File and PDF processing:** Any scenario where barcodes come from uploaded files, documents, or PDFs — BarcodeScanning.Native.Maui cannot help at all
- **Windows MAUI:** Any app targeting Windows in a MAUI multi-target build
- **Server-side:** Any ASP.NET endpoint, background job, or Azure Function that processes barcodes
- **Generation:** Any app that needs to create barcodes, not just read them
- **UPC-A and PDF417:** Any app where iOS UPC-A accuracy or PDF417 reliability matters
- **Mixed mobile and server:** An app that scans on mobile and also has server-side processing logic — IronBarcode can cover both sides with the same API

The simplest decision rule: if you need live camera preview as the primary UX and Windows support is not a target, BarcodeScanning.Native.Maui is reasonable. If any of the above scenarios apply, IronBarcode is the right scope.

## Conclusion

The scope mismatch is the story. BarcodeScanning.Native.Maui is named and marketed like a general barcode library, but it is a camera control for two platforms. Teams discover this when they need to support Windows, when a user uploads a photo instead of scanning live, when server-side barcode processing enters the picture, or when iOS UPC-A data starts appearing as 13 digits in their database.

IronBarcode is not a drop-in replacement for the live camera preview experience — `MediaPicker` captures a frame and hands it to `BarcodeReader.Read()`, which works correctly but does not give you an embedded viewfinder. What it does is handle every input source (files, bytes, streams, PDFs, camera captures), every output target (iOS, Android, Windows, macOS MAUI, server), and every operation direction (reading and generating) through a single package and a consistent API.

If your requirements have grown beyond "live camera, iOS/Android only," BarcodeScanning.Native.Maui's camera control architecture is not the right foundation.
