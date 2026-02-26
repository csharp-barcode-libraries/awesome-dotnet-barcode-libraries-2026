# Migrating from ZXing.Net.MAUI to IronBarcode

The migration from ZXing.Net.MAUI typically happens for one of three reasons: Windows MAUI support was added to the requirements and ZXing.Net.MAUI simply has no implementation for it, iPhone 15 Pro scanning failures surfaced in QA or production and there is no programmatic fix, or the camera resource leak became a stability problem across page navigations. In each case the fix involves the same structural change: replacing the `CameraBarcodeReaderView` event-driven pattern with `MediaPicker.CapturePhotoAsync()` and `BarcodeReader.Read()`.

## Why Migrate

**Windows MAUI is not supported.** ZXing.Net.MAUI has no Windows camera implementation and no stated plan to add one. If the project added a Windows target after the initial build — common in MAUI teams that started mobile-first — ZXing.Net.MAUI cannot be part of the solution. IronBarcode's `MediaPicker` + `BarcodeReader.Read()` pattern works on Windows MAUI without platform-specific code.

**iPhone 15 Pro auto-focus is documented as broken.** The GitHub issue tracker for `Redth/ZXing.Net.Maui` documents that iPhone 15 Pro and Pro Max devices (hardware identifiers `iPhone16,1` and `iPhone16,2`) fail to achieve the focus required for reliable barcode detection. The only workaround is instructing users to manually adjust distance. For a production application, this is not a workaround — it is a UX failure on the flagship iOS device. IronBarcode's image-capture approach sidesteps the issue entirely: the system camera handles focus independently, and the resulting image is processed after capture.

**Camera resources are not released properly.** `CameraBarcodeReaderView` has no `Dispose()` method. The documented mitigation — setting `IsDetecting = false` in `OnDisappearing()` — reduces the problem but does not formally release resources. Applications that navigate to and from scan pages repeatedly accumulate camera resource leaks that manifest as memory growth, battery drain, and intermittent camera initialization failures on return.

**Format specification adds a silent failure mode.** ZXing.Net.MAUI inherits ZXing.Net's requirement to declare every barcode format before scanning. If a format is missing from `BarcodeReaderOptions.Formats`, barcodes in that format appear in the camera frame and are silently ignored. IronBarcode detects all formats automatically — no configuration, no missed barcodes.

**PDF and file reading were added to the requirements.** ZXing.Net.MAUI is a live camera control. There is no `BarcodeReader.Read(filePath)` API. When requirements expand to include reading barcodes from uploaded PDFs, image files, or server-side documents, ZXing.Net.MAUI offers nothing. IronBarcode reads from files, streams, byte arrays, and PDFs natively.

**The library is pre-release (v0.5.0).** Version 0.5.0 in semantic versioning indicates pre-release status. API stability, bug fix timelines, and production readiness guarantees are not provided. For applications where barcode scanning is a primary workflow, a stable, commercially supported library is the appropriate foundation.

## Quick Start

### Step 1: Remove ZXing.Net.Maui.Controls

```bash
dotnet remove package ZXing.Net.Maui.Controls
```

If `UseBarcodeReader()` was added to `MauiProgram.cs` during ZXing.Net.MAUI setup, remove that registration:

```csharp
// Remove this line from MauiProgram.cs if present
builder.UseBarcodeReader();
```

### Step 2: Install IronBarcode

```bash
# NuGet: dotnet add package IronBarcode
dotnet add package IronBarcode
```

The [.NET MAUI barcode scanner tutorial](https://ironsoftware.com/csharp/barcode/get-started/net-maui-barcode-scanner-reader-tutorial/) walks through the full project setup, including permissions configuration for iOS and Android.

### Step 3: Replace Namespace and Initialize License

Remove the ZXing.Net.MAUI namespace:

```csharp
// Remove
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
```

Add IronBarCode and initialize the license key at application startup — in `MauiProgram.cs` or `App.xaml.cs`:

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";
```

## Code Migration Examples

### Camera Scanning: CameraBarcodeReaderView to MediaPicker

This is the core structural change. The `CameraBarcodeReaderView` runs continuously in the XAML layout and fires `OnBarcodesDetected` events. The IronBarcode replacement uses MAUI's `MediaPicker` to open the system camera when the user taps a button, captures a photo, and passes it to `BarcodeReader.Read()`.

**Before — XAML:**

```xml
<ContentPage xmlns:zxing="clr-namespace:ZXing.Net.Maui.Controls;assembly=ZXing.Net.MAUI.Controls">
    <StackLayout>
        <zxing:CameraBarcodeReaderView
            x:Name="CameraView"
            Options="{Binding ReaderOptions}"
            BarcodesDetected="OnBarcodesDetected"
            VerticalOptions="FillAndExpand" />
        <Label x:Name="ResultLabel" Text="Scanning..." />
    </StackLayout>
</ContentPage>
```

**Before — code-behind:**

```csharp
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

public partial class ScannerPage : ContentPage
{
    public BarcodeReaderOptions ReaderOptions { get; }

    public ScannerPage()
    {
        InitializeComponent();
        ReaderOptions = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.QRCode |
                      BarcodeFormats.Code128 |
                      BarcodeFormats.Ean13 |
                      BarcodeFormats.UpcA,
            TryHarder = true,
            AutoRotate = true
        };
        BindingContext = this;
    }

    private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var barcode in e.Results)
                ResultLabel.Text = $"{barcode.Format}: {barcode.Value}";
        });
        CameraView.IsDetecting = false;
    }

    // Required boilerplate to avoid camera resource leaks
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        CameraView.IsDetecting = false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CameraView.IsDetecting = true;
    }
}
```

**After — XAML:**

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui">
    <StackLayout>
        <Button Text="Scan Barcode" Clicked="ScanButton_Clicked" />
        <Label x:Name="ResultLabel" Text="Tap to scan..." />
    </StackLayout>
</ContentPage>
```

**After — code-behind:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public partial class ScannerPage : ContentPage
{
    public ScannerPage()
    {
        InitializeComponent();
    }

    private async void ScanButton_Clicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.CapturePhotoAsync();
        if (photo == null) return;

        using var stream = await photo.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        // No format specification — all formats detected automatically
        var results = BarcodeReader.Read(ms.ToArray());
        var first = results.FirstOrDefault();
        ResultLabel.Text = first != null
            ? $"{first.Format}: {first.Value}"
            : "No barcode found";
    }

    // No OnAppearing / OnDisappearing needed — no camera state to manage
}
```

The lifecycle management boilerplate is gone. The `zxing:` XAML namespace declaration is gone. The `BarcodeReaderOptions` format list is gone. The `MainThread.BeginInvokeOnMainThread()` wrapper is gone — with `async`/`await`, the continuation runs on the calling context, which is the main thread for a UI event handler.

### Removing Format Specification

If the codebase has multiple `BarcodeReaderOptions` configurations across different pages or scan scenarios, delete them. IronBarcode detects all formats automatically. No format list is needed or accepted in the same way.

**Before:**

```csharp
// ZXing.Net.MAUI — must enumerate every format you want to detect
var readerOptions = new BarcodeReaderOptions
{
    Formats = BarcodeFormats.QRCode |
              BarcodeFormats.DataMatrix |
              BarcodeFormats.Aztec |
              BarcodeFormats.Pdf417 |
              BarcodeFormats.Code128 |
              BarcodeFormats.Code39 |
              BarcodeFormats.Ean13 |
              BarcodeFormats.UpcA |
              BarcodeFormats.Codabar,
    TryHarder = true
};
```

**After:**

```csharp
// IronBarcode — no format configuration required
// All 50+ formats are detected automatically
var results = BarcodeReader.Read(imageBytes);
```

If you want to tune performance for a scenario where only one format is expected, `BarcodeReaderOptions` exists in IronBarcode but format specification is optional, not required.

### Removing Camera Lifecycle Management

Any `OnAppearing` and `OnDisappearing` overrides that exist only to manage `CameraView.IsDetecting` can be removed. IronBarcode is stateless — there is no camera view running in the background, no resource to release, and no reinitializer to call on page return.

**Before (on every scan page):**

```csharp
protected override void OnDisappearing()
{
    base.OnDisappearing();
    if (CameraView != null)
        CameraView.IsDetecting = false;  // Prevent resource leak
}

protected override void OnAppearing()
{
    base.OnAppearing();
    if (CameraView != null)
        CameraView.IsDetecting = true;
}
```

**After:**

```csharp
// Delete both methods if they only exist to manage camera state
// If OnDisappearing / OnAppearing have other logic, keep them — just remove the IsDetecting lines
```

### Windows MAUI: Same Code, No Changes

With ZXing.Net.MAUI, the Windows target either failed to compile or required a stub implementation. With IronBarcode, the `MediaPicker` + `BarcodeReader.Read()` pattern above runs on Windows MAUI without modification. The `MediaPicker.CapturePhotoAsync()` call maps to the Windows file picker on the Windows target — the user selects an image file instead of triggering a camera, which is appropriate behavior for a desktop environment.

No `#if WINDOWS` conditional compilation. No platform-specific service registration. No stub interfaces. [Windows and macOS desktop barcode functionality](https://ironsoftware.com/csharp/barcode/how-to/barcode-desktop-maui/) is covered by the same package that handles mobile.

### PDF Barcode Reading (Net-New Capability)

ZXing.Net.MAUI has no API for reading barcodes from PDF documents. If this is a new requirement:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

// Read all barcodes from all pages of a PDF
var results = BarcodeReader.Read("invoice.pdf");
foreach (var barcode in results)
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Format} — {barcode.Value}");

// Read from a user-selected file using MAUI FilePicker
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
```

IronBarcode [reads barcodes from PDFs natively](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) — parsing each page directly without an intermediate image rendering step. This covers shipping invoices, digital tickets, document management, and batch processing scenarios that ZXing.Net.MAUI cannot address.

### Server-Side Barcode Processing (Net-New Capability)

If the application has a backend ASP.NET Core endpoint that should also process barcodes — for uploaded documents or server-side batch jobs — the same package covers it:

```csharp
using IronBarCode;

// ASP.NET Core endpoint: reads barcodes from an uploaded file or PDF
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

One package, one `BarcodeReader.Read()` call, deployed on mobile and server. No separate barcode library for the backend.

## API Mapping

| ZXing.Net.MAUI | IronBarcode | Notes |
|---|---|---|
| `new BarcodeReaderOptions { Formats = BarcodeFormats.QRCode \| ... }` | Not required | IronBarcode auto-detects all formats |
| `CameraBarcodeReaderView` XAML control | Remove — use `Button` + `MediaPicker.CapturePhotoAsync()` | Different input model |
| `BarcodesDetected="OnBarcodesDetected"` event | `BarcodeReader.Read(imageBytes)` return value | Method return vs event |
| `BarcodeDetectionEventArgs e` | `IEnumerable<BarcodeResult>` from `BarcodeReader.Read()` | |
| `e.Results` | Return value of `BarcodeReader.Read()` | |
| `barcode.Value` | `result.Value` | Same data, same name |
| `barcode.Format` | `result.Format` | Same data, same name |
| `BarcodeFormats.QRCode` | `BarcodeEncoding.QRCode` | Enum rename |
| `BarcodeFormats.Code128` | `BarcodeEncoding.Code128` | Enum rename |
| `BarcodeFormats.Ean13` | `BarcodeEncoding.EAN13` | Enum rename |
| `cameraBarcodeReaderView.IsDetecting = false` | Not needed — IronBarcode is stateless | Remove from `OnDisappearing` |
| `cameraBarcodeReaderView.IsDetecting = true` | Not needed | Remove from `OnAppearing` |
| `builder.UseBarcodeReader()` in `MauiProgram.cs` | Remove | Not required by IronBarcode |
| iOS + Android only | iOS, Android, Windows, macOS, ASP.NET Core | |
| v0.5.0 pre-release | Stable release | |
| No PDF support | `BarcodeReader.Read("file.pdf")` | Native PDF parsing |
| No file input | `BarcodeReader.Read("path/to/image.png")` | |

## Migration Checklist

Run these searches to find all ZXing.Net.MAUI usage in the codebase:

```bash
grep -rn "ZXing.Net.Maui" --include="*.cs" --include="*.xaml" .
grep -rn "CameraBarcodeReaderView" --include="*.cs" --include="*.xaml" .
grep -rn "BarcodeDetectionEventArgs" --include="*.cs" .
grep -rn "e\.Results" --include="*.cs" .
grep -rn "barcode\.Value" --include="*.cs" .
grep -rn "BarcodeReaderOptions" --include="*.cs" .
grep -rn "BarcodeFormats\." --include="*.cs" .
grep -rn "IsDetecting" --include="*.cs" .
grep -rn "UseBarcodeReader" --include="*.cs" .
grep -rn "zxing:" --include="*.xaml" .
```

Work through each hit:

- `using ZXing.Net.Maui;` and `using ZXing.Net.Maui.Controls;` — replace with `using IronBarCode;`
- `zxing:CameraBarcodeReaderView` in XAML — remove the control and remove the `xmlns:zxing` namespace declaration; replace with a `Button` that calls `MediaPicker.CapturePhotoAsync()`
- `BarcodesDetected="OnBarcodesDetected"` event wiring in XAML — remove; replace with `Clicked` handler on the scan button
- `BarcodeDetectionEventArgs e` parameter — remove; replace with the return value of `BarcodeReader.Read(imageBytes)`
- `e.Results` iteration — replace with iteration over the return value of `BarcodeReader.Read()`
- `barcode.Value` — no change needed; IronBarcode uses `result.Value`
- `barcode.Format` — no change needed; IronBarcode uses `result.Format`
- `BarcodeReaderOptions { Formats = BarcodeFormats.X | ... }` — delete entirely; no format specification needed
- `BarcodeFormats.QRCode` and similar enum values — replace with `BarcodeEncoding.QRCode` if used in IronBarcode options
- `IsDetecting = false` in `OnDisappearing()` — remove; delete the method if it has no other logic
- `IsDetecting = true` in `OnAppearing()` — remove; delete the method if it has no other logic
- `builder.UseBarcodeReader()` in `MauiProgram.cs` — remove the line

After the changes compile, verify that [Android MAUI scanning](https://ironsoftware.com/csharp/barcode/get-started/android/) and iOS scanning both work through the `MediaPicker` pattern. If the project has a Windows target, confirm the Windows build succeeds — it will, without any additional changes.

The camera resource leak, the iPhone 15 Pro auto-focus workarounds, and the `IsDetecting` lifecycle boilerplate are all gone. What remains is a button, a `MediaPicker` call, and `BarcodeReader.Read()`.
