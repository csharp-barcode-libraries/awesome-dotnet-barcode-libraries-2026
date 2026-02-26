# Migrating from ZXing.Net.MAUI to IronBarcode

This guide provides a complete migration path from ZXing.Net.MAUI to IronBarcode for .NET MAUI developers. It covers the reasons teams undertake this migration, a feature comparison to frame the decision, mechanical steps to replace the package and update project files, before-and-after code examples for every major usage pattern, an API translation reference, a troubleshooting section for issues that arise during the transition, a migration checklist for tracking progress, and a summary of the outcomes that the migration delivers.

## Why Migrate from ZXing.Net.MAUI

The decision to migrate away from ZXing.Net.MAUI is typically triggered by one or more concrete project conditions. These are not stylistic preferences — they are cases where the library's architecture or maintenance status prevents a requirement from being met.

**Windows MAUI Not Supported:** ZXing.Net.MAUI has no Windows camera implementation and no public plan to build one. The library is constructed around iOS and Android platform camera APIs. If a MAUI project adds a Windows target after the initial build — a common pattern in teams that start mobile-first — ZXing.Net.MAUI cannot serve that target at all. There is no stub, no fallback, and no workaround.

**iPhone 15 Pro Auto-Focus Issue:** The GitHub issue tracker for `Redth/ZXing.Net.Maui` documents that iPhone 15 Pro and Pro Max devices (`iPhone16,1` and `iPhone16,2`) fail to achieve reliable focus for barcode detection when using `CameraBarcodeReaderView`. The barcode is visible in the camera frame, but the auto-focus system does not lock sharply enough for the decoder to extract a result. The only documented mitigation is to instruct the user to manually adjust the distance between device and barcode — an instruction that requires a visible UX affordance and user patience, and that is not an acceptable production outcome for a primary workflow.

**Camera Resource Leak:** `CameraBarcodeReaderView` does not implement `IDisposable`. When the user navigates away from a scan page, camera resources are not released through a standard disposal pattern. The documented workaround is to set `IsDetecting = false` in `OnDisappearing()`, which reduces the impact but does not formally free the camera. Applications that navigate frequently to and from scan pages accumulate resource consumption that can present as memory growth, battery drain, and intermittent camera initialization failures on return to the scan page.

**Format Specification Silent Failure:** ZXing.Net.MAUI inherits ZXing.Net's requirement to declare every barcode format to scan before scanning begins. Formats omitted from `BarcodeReaderOptions.Formats` are silently ignored even when clearly visible in the camera frame. A user who points the device at a barcode format that the developer did not anticipate sees no error — the application simply does not detect anything. In environments where barcode formats are controlled by external suppliers, customers, or third-party systems, this silent miss becomes a persistent support issue.

**Pre-Release Stability:** ZXing.Net.MAUI is published at v0.5.0 — a pre-release designation under semantic versioning. No API stability guarantees, no bug fix timelines, and no production readiness commitments are made. For enterprise applications subject to dependency audits or software composition analysis, a pre-release community library without commercial support may not clear the approval process.

### The Fundamental Problem

The structural problem is that `CameraBarcodeReaderView` locks the developer into a camera-centric event loop with manual lifecycle management and a fixed format list. For anything beyond basic camera scanning on iOS and Android with known barcode formats, the architecture reaches its boundary:

```csharp
// ZXing.Net.MAUI: event loop, format list, lifecycle boilerplate on every scan page
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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        CameraView.IsDetecting = false;  // Required — no Dispose() available
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CameraView.IsDetecting = true;
    }
}
```

IronBarcode replaces the event loop with a single async call on a button tap, removes format configuration entirely, and eliminates lifecycle management from the page:

```csharp
// NuGet: dotnet add package IronBarcode
// IronBarcode: stateless, all platforms, auto-detection, no lifecycle boilerplate
using IronBarCode;

public partial class ScannerPage : ContentPage
{
    public ScannerPage() => InitializeComponent();

    private async void ScanButton_Clicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.CapturePhotoAsync();
        if (photo == null) return;

        using var stream = await photo.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        var results = BarcodeReader.Read(ms.ToArray());
        ResultLabel.Text = results.FirstOrDefault()?.Value ?? "No barcode found";
    }
    // No OnAppearing / OnDisappearing needed
}
```

## IronBarcode vs ZXing.Net.MAUI: Feature Comparison

| Feature | ZXing.Net.MAUI | IronBarcode |
|---|---|---|
| Release status | Pre-release (v0.5.0) | Stable, commercial release |
| iOS MAUI | Yes (iPhone 15 Pro focus broken) | Yes |
| Android MAUI | Yes (Camera 1.5.0 build issues) | Yes |
| Windows MAUI | Not supported | Yes |
| macOS MAUI | Not supported | Yes |
| Server-side / ASP.NET Core | No | Yes |
| Live camera viewfinder | Yes | No (MediaPicker system UI) |
| Format specification required | Yes | No (auto-detection, 50+ formats) |
| Camera lifecycle management | Manual (IsDetecting) | Not applicable |
| Dispose() implementation | No | Not applicable — stateless |
| iPhone 15 Pro auto-focus | Broken (documented) | Not applicable |
| PDF barcode extraction | No | Yes |
| File path input | No (camera only) | Yes |
| Damaged barcode recovery | TryHarder only | Yes (ML-powered) |
| Barcode generation | Yes (via ZXing.Net) | Yes |
| Commercial support | None | Yes |
| License | MIT (free) | Commercial |

## Quick Start

### Step 1: Remove ZXing.Net.Maui.Controls and Clean Up MauiProgram.cs

Remove the ZXing.Net.MAUI NuGet package:

```bash
dotnet remove package ZXing.Net.Maui.Controls
```

ZXing.Net.MAUI requires a one-time registration call in `MauiProgram.cs`. Remove this line if it is present:

```csharp
// Remove this line from MauiProgram.cs
builder.UseBarcodeReader();
```

The `using ZXing.Net.Maui;` import that supports this call can also be removed from `MauiProgram.cs`.

### Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

The [.NET MAUI barcode scanner tutorial](https://ironsoftware.com/csharp/barcode/get-started/net-maui-barcode-scanner-reader-tutorial/) covers the full project configuration, including `Info.plist` camera permission entries for iOS and `AndroidManifest.xml` permission declarations for Android.

### Step 3: Update Namespaces and Initialize License

Remove ZXing.Net.MAUI namespace imports from all files:

```csharp
// Remove these from every .cs file that imported them
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
```

Add the IronBarCode namespace and initialize the license key at application startup. The appropriate location is `MauiProgram.cs` or `App.xaml.cs`, before any barcode operations are performed:

```csharp
using IronBarCode;

// In MauiProgram.cs CreateMauiApp() or App.xaml.cs constructor
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

## Code Migration Examples

### Replacing the XAML Camera Control and Code-Behind

The `CameraBarcodeReaderView` XAML control and its supporting code-behind are the primary migration target. The replacement removes the camera view from the page layout and replaces the event-driven scanning pattern with a button-triggered async capture.

**ZXing.Net.MAUI Approach:**

XAML:
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

Code-behind:
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
            Formats = BarcodeFormats.QRCode | BarcodeFormats.Code128 | BarcodeFormats.Ean13,
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

**IronBarcode Approach:**

XAML:
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui">
    <StackLayout>
        <Button Text="Scan Barcode" Clicked="ScanButton_Clicked" />
        <Label x:Name="ResultLabel" Text="Tap to scan..." />
    </StackLayout>
</ContentPage>
```

Code-behind:
```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public partial class ScannerPage : ContentPage
{
    public ScannerPage() => InitializeComponent();

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

    // No OnAppearing or OnDisappearing required — no camera state exists between scans
}
```

The `zxing:` XAML namespace, the `BarcodeReaderOptions` format list, the `MainThread.BeginInvokeOnMainThread()` wrapper, and both lifecycle overrides are eliminated entirely. The continuation of the `async` method runs on the calling context — which is the main thread for a UI event handler — so no explicit thread marshalling is needed.

### Removing Format Specification

Any `BarcodeReaderOptions` configuration blocks scattered across multiple pages or scan scenarios in the codebase are deleted. IronBarcode performs automatic format detection across all 50+ supported formats without pre-configuration.

**ZXing.Net.MAUI Approach:**

```csharp
// ZXing.Net.MAUI: every anticipated format must be listed explicitly
// Formats not listed here will silently fail to detect
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

**IronBarcode Approach:**

```csharp
// IronBarcode: no format configuration needed
// All formats are detected automatically on every read call
var results = BarcodeReader.Read(imageBytes);

// Optional: restrict to specific formats for performance tuning (not required for correctness)
var options = new BarcodeReaderOptions
{
    ExpectBarcodeTypes = BarcodeEncoding.QRCode | BarcodeEncoding.Code128
};
var tunedResults = BarcodeReader.Read(imageBytes, options);
```

Format hints are available in IronBarcode for performance-sensitive scenarios, but they are optional. A barcode in a format not listed in an options object will still be detected and returned.

### Removing IsDetecting Lifecycle Management

Every `OnAppearing` and `OnDisappearing` override that exists to toggle `CameraView.IsDetecting` is removed. If those method overrides contain other page lifecycle logic, preserve that logic and remove only the `IsDetecting` lines.

**ZXing.Net.MAUI Approach:**

```csharp
// Required boilerplate on every page — omitting this causes camera resource leaks
protected override void OnDisappearing()
{
    base.OnDisappearing();
    if (CameraView != null)
        CameraView.IsDetecting = false;
}

protected override void OnAppearing()
{
    base.OnAppearing();
    if (CameraView != null)
        CameraView.IsDetecting = true;
}
```

**IronBarcode Approach:**

```csharp
// Delete both methods if they contain only IsDetecting management.
// If they contain other logic, remove only the IsDetecting lines and keep the rest.
// IronBarcode is stateless — there is no camera view running between button taps.
```

### Windows MAUI: The Same Code, No Conditional Compilation

With ZXing.Net.MAUI, adding a Windows target to a project either failed to compile or required platform-specific stubs because the Windows implementation was never written. With IronBarcode, the same code that runs on iOS and Android compiles and runs on Windows without modification.

**ZXing.Net.MAUI Approach:**

```csharp
// Windows MAUI: either fails to compile or requires a platform-specific stub
// There is no documented path to Windows support
#if ANDROID || IOS
    // ZXing.Net.MAUI scanning — Windows has no implementation
#endif
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
// No platform conditionals — same code runs on iOS, Android, Windows, and macOS
private async void ScanButton_Clicked(object sender, EventArgs e)
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

On Windows, `MediaPicker.CapturePhotoAsync()` maps to the Windows file picker, allowing the user to select an image file — appropriate behavior for a desktop environment. The [MAUI desktop barcode guide](https://ironsoftware.com/csharp/barcode/how-to/barcode-desktop-maui/) covers Windows and macOS MAUI configuration in detail.

### PDF Barcode Reading (New Capability)

ZXing.Net.MAUI has no API for reading barcodes from PDF documents. If this is a new requirement that the migration enables, the following pattern applies:

**ZXing.Net.MAUI Approach:**

```csharp
// ZXing.Net.MAUI: no API for PDF or file-based barcode reading
// Cannot fulfill this requirement — a separate library is required
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

// Read all barcodes from all pages of a PDF
var pdfResults = BarcodeReader.Read("invoice.pdf");
foreach (var barcode in pdfResults)
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Format} — {barcode.Value}");

// Read from a user-selected file using MAUI FilePicker
private async void ReadFileButton_Clicked(object sender, EventArgs e)
{
    var file = await FilePicker.PickAsync(new PickOptions
    {
        PickerTitle = "Select image or PDF"
    });
    if (file == null) return;

    var fileResults = BarcodeReader.Read(file.FullPath);
    foreach (var result in fileResults)
        ResultLabel.Text += $"\n{result.Format}: {result.Value}";
}
```

The complete PDF barcode reading workflow — including multi-page documents, page number metadata, and mixed format documents — is documented in the [read barcodes from PDF guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/).

## ZXing.Net.MAUI API to IronBarcode Mapping Reference

| ZXing.Net.MAUI | IronBarcode | Notes |
|---|---|---|
| `builder.UseBarcodeReader()` | Not required | Remove from `MauiProgram.cs` |
| `using ZXing.Net.Maui;` | `using IronBarCode;` | Namespace replacement |
| `using ZXing.Net.Maui.Controls;` | Not required | Remove |
| `xmlns:zxing="clr-namespace:ZXing.Net.Maui.Controls;..."` | Not required | Remove from XAML |
| `<zxing:CameraBarcodeReaderView>` | `<Button>` + `MediaPicker.CapturePhotoAsync()` | Architectural replacement |
| `Options="{Binding ReaderOptions}"` | Not required | Remove binding |
| `BarcodesDetected="OnBarcodesDetected"` | `BarcodeReader.Read()` return value | Event → async return |
| `new BarcodeReaderOptions { Formats = BarcodeFormats.X \| ... }` | Not required | Auto-detection replaces format lists |
| `BarcodeDetectionEventArgs e` | `IEnumerable<BarcodeResult>` | Different result delivery |
| `e.Results` | Return value of `BarcodeReader.Read()` | |
| `barcode.Value` | `result.Value` | Same property name |
| `barcode.Format` | `result.Format` | Same property name |
| `BarcodeFormats.QRCode` | `BarcodeEncoding.QRCode` | Enum rename |
| `BarcodeFormats.Code128` | `BarcodeEncoding.Code128` | Enum rename |
| `BarcodeFormats.Ean13` | `BarcodeEncoding.EAN13` | Enum rename |
| `BarcodeFormats.UpcA` | `BarcodeEncoding.UPCA` | Enum rename |
| `CameraView.IsDetecting = false` | Not required | Remove from `OnDisappearing` |
| `CameraView.IsDetecting = true` | Not required | Remove from `OnAppearing` |
| No file input API | `BarcodeReader.Read("path/to/image.png")` | New capability |
| No PDF input API | `BarcodeReader.Read("document.pdf")` | New capability |
| iOS and Android only | iOS, Android, Windows, macOS, server | Platform expansion |

## Common Migration Issues and Solutions

### Issue 1: No Live Viewfinder Equivalent

**ZXing.Net.MAUI:** The `CameraBarcodeReaderView` displays a continuous camera feed in the page layout, showing the user a live preview with overlaid scan feedback.

**Solution:** IronBarcode does not provide a live viewfinder control. The replacement pattern uses `MediaPicker.CapturePhotoAsync()`, which opens the platform system camera UI. The system camera provides its own live preview and focus indicator. After the user captures the image and confirms, the result is passed to `BarcodeReader.Read()`. If a persistent in-app viewfinder is a required UX element that cannot be replaced by the system camera UI, the camera integration layer must be built separately using `Microsoft.Maui.Media` or platform camera APIs, with IronBarcode handling the decode step.

### Issue 2: Camera Permission Declarations

**ZXing.Net.MAUI:** Camera permissions may have been declared in the project as part of the ZXing.Net.MAUI setup instructions.

**Solution:** Camera permissions remain necessary for the `MediaPicker.CapturePhotoAsync()` call that IronBarcode's MAUI pattern uses. Verify that `NSCameraUsageDescription` is present in `Info.plist` for iOS and that `<uses-permission android:name="android.permission.CAMERA" />` is present in `AndroidManifest.xml` for Android. IronBarcode itself does not access the camera directly — it processes images — but the `MediaPicker` call that provides images to it requires camera permission. Permission setup is covered in the [.NET MAUI barcode scanner tutorial](https://ironsoftware.com/csharp/barcode/get-started/net-maui-barcode-scanner-reader-tutorial/).

### Issue 3: Windows Build Now Succeeds Where It Previously Failed

**ZXing.Net.MAUI:** Projects that attempted to include a Windows target with ZXing.Net.MAUI typically encountered compile errors or required the library to be excluded from the Windows build through conditional MSBuild logic.

**Solution:** After removing ZXing.Net.MAUI and installing IronBarcode, the Windows MAUI build succeeds without any platform conditionals. Remove any `#if ANDROID || IOS` guards that were placed around ZXing.Net.MAUI calls to exclude them from the Windows build. The IronBarcode `BarcodeReader.Read()` call compiles and runs on all target frameworks. If `MediaPicker.CapturePhotoAsync()` was excluded from Windows builds, that exclusion can also be removed — the method is supported on Windows MAUI and maps to the file picker. Verify the full solution builds cleanly for all target frameworks after removing the conditionals.

### Issue 4: BarcodeFormats Enum References

**ZXing.Net.MAUI:** The `BarcodeFormats` enum from `ZXing.Net.Maui` is used extensively in `BarcodeReaderOptions` configurations. After the package is removed, any remaining references produce compile errors.

**Solution:** Delete all `BarcodeReaderOptions` initialization blocks that were used to configure format lists. IronBarcode does not require format specification for correct operation. If any remaining code references `BarcodeFormats` values for logging, display, or comparison purposes, replace them with `BarcodeEncoding` values from the `IronBarCode` namespace. Run `grep -rn "BarcodeFormats\." --include="*.cs" .` to find all remaining references after the package removal.

## ZXing.Net.MAUI Migration Checklist

### Pre-Migration Tasks

Audit all ZXing.Net.MAUI usage in the codebase before making changes:

```bash
grep -rn "ZXing.Net.Maui" --include="*.cs" --include="*.xaml" .
grep -rn "CameraBarcodeReaderView" --include="*.cs" --include="*.xaml" .
grep -rn "BarcodeDetectionEventArgs" --include="*.cs" .
grep -rn "BarcodeReaderOptions" --include="*.cs" .
grep -rn "BarcodeFormats\." --include="*.cs" .
grep -rn "IsDetecting" --include="*.cs" .
grep -rn "UseBarcodeReader" --include="*.cs" .
grep -rn "zxing:" --include="*.xaml" .
grep -rn "e\.Results" --include="*.cs" .
```

Document all scan pages identified by the audit. Note which `OnAppearing` and `OnDisappearing` overrides contain only `IsDetecting` management (to be deleted) versus those that contain other logic (to be partially modified). Note any `BarcodeReaderOptions` instances that may contain format lists used in ways beyond detection configuration.

### Code Update Tasks

1. Remove the `ZXing.Net.Maui.Controls` NuGet package from the project file
2. Remove `builder.UseBarcodeReader()` from `MauiProgram.cs`
3. Remove the `using ZXing.Net.Maui;` and `using ZXing.Net.Maui.Controls;` namespace imports from all files
4. Install the `IronBarcode` NuGet package
5. Add `using IronBarCode;` to all files that will use barcode reading
6. Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` to application startup
7. Remove the `xmlns:zxing` namespace declaration from all XAML files
8. Remove all `<zxing:CameraBarcodeReaderView>` elements from XAML files
9. Replace the removed camera view with a `<Button>` control in each XAML file
10. Add `Clicked="ScanButton_Clicked"` to each scan button
11. Delete the `OnBarcodesDetected` event handler methods from all code-behind files
12. Add `async void ScanButton_Clicked` methods implementing `MediaPicker.CapturePhotoAsync()` + `BarcodeReader.Read()` to each page
13. Delete all `BarcodeReaderOptions` initialization blocks
14. Delete all `OnDisappearing` and `OnAppearing` overrides that exist solely for `IsDetecting` management
15. Remove `IsDetecting` lines from any `OnDisappearing` and `OnAppearing` overrides that contain other logic
16. Remove any `#if ANDROID || IOS` conditional compilation guards that were isolating ZXing.Net.MAUI from Windows builds
17. Replace any remaining `BarcodeFormats.X` enum references with `BarcodeEncoding.X` equivalents

### Post-Migration Testing

After the migration compiles cleanly, verify the following:

- Android MAUI: scan button opens the system camera, captures a photo, and returns a correct barcode result — confirm with the [Android scanning guide](https://ironsoftware.com/csharp/barcode/get-started/android/)
- iOS MAUI: the same flow works on iOS, including on iPhone 15 Pro hardware where the auto-focus issue previously occurred
- Windows MAUI: the Windows build compiles without errors and the scan button opens the file picker and returns a correct result from a selected image
- Formats: test scanning barcodes in formats that were not included in the old `BarcodeFormats` list to confirm auto-detection
- Page navigation: navigate to and from the scan page multiple times and verify no memory growth or camera initialization failures occur
- PDF reading: if the migration adds PDF barcode reading as a new capability, verify that multi-page PDFs return results with correct page number metadata

## Key Benefits of Migrating to IronBarcode

**Expanded Platform Coverage:** After migration, the application supports Windows and macOS MAUI targets in addition to iOS and Android — all from the same package and the same scan pattern. Projects that previously required platform-specific stubs or excluded Windows from barcode functionality gain full coverage without additional code.

**Current-Generation Hardware Reliability:** The image-capture approach through `MediaPicker` and `BarcodeReader.Read()` is not affected by the `CameraBarcodeReaderView` auto-focus model that fails on iPhone 15 Pro and Pro Max hardware. The system camera handles focus independently, and IronBarcode processes the captured image after the user confirms the shot.

**Elimination of Camera Resource Management:** Removing `CameraBarcodeReaderView` removes the entire category of camera resource leak bugs. There is no `IsDetecting` state to track, no `OnAppearing` and `OnDisappearing` boilerplate to maintain on every scan page, and no accumulation of camera resources across navigation cycles. The stateless API makes scan pages indistinguishable from any other page in terms of resource lifecycle.

**Format Coverage Without Configuration:** Any barcode format encountered in the field is detected automatically. Scan failures caused by missing entries in a `BarcodeFormats` list are eliminated. Support requests from users whose barcodes were silently ignored because a supplier changed their label format cease to occur.

**File and Document Processing:** The migration opens the ability to read barcodes from PDF documents, image files, and byte streams with no additional library. Workflows that were previously outside the scope of ZXing.Net.MAUI — reading barcodes from uploaded invoices, processing digital tickets, batch scanning image directories — become available through the same `BarcodeReader.Read()` call used for camera captures.

**Production-Grade Stability:** IronBarcode ships as a stable commercial release with an active development cadence, commercial support, and regular updates tracking .NET version releases. Dependency audits, software composition analysis, and enterprise approval processes encounter a supported library with a documented maintenance commitment rather than a community pre-release package.
