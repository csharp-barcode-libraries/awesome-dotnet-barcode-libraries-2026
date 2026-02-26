ZXing.Net.Maui.Controls v0.5.0. Still pre-release. Windows MAUI not supported. iPhone 15 Pro auto-focus documented as broken. Camera resource leak with no Dispose(). This is the library most .NET MAUI developers reach for first.

The name makes sense: ZXing is the barcode engine the .NET community has used for a decade, and ZXing.Net.MAUI wraps it with MAUI camera controls. `CameraBarcodeReaderView` drops into XAML, the `BarcodesDetected` event fires when something is scanned, and you are done in thirty minutes. On a good day.

The problems surface in production. The GitHub repository for `Redth/ZXing.Net.Maui` documents iPhone 15 Pro auto-focus failure — current flagship hardware, not an edge case device. The `CameraBarcodeReaderView` has no `Dispose()` method, which means camera resources are not formally released when navigating away from a scan page. The Windows MAUI target is absent entirely, with no roadmap. And the version number — 0.5.0 — communicates exactly what it means in semantic versioning: this is pre-release software. For a prototype, that is acceptable. For a production application, each of those issues is a separate conversation with stakeholders.

## Pre-Release Status

The NuGet package is `ZXing.Net.Maui.Controls`, currently at v0.5.0 and flagged as a pre-release in the package registry. The repository is community-maintained by Jon Dick (GitHub: Redth), not backed by a commercial organization. There is no SLA, no paid support tier, and no guaranteed timeline for any fix.

Pre-release status means the API can change between versions without a deprecation cycle. It means bugs are fixed when contributors have time. It means "this may not be production-ready" is the implicit stance of the version number, regardless of whether the code is functionally useful for your specific use case today.

For a proof of concept or an internal prototype, none of this matters. For a customer-facing application where barcode scanning is a primary workflow, pre-release stability is worth factoring into the evaluation — alongside the specific issues documented below.

## Known Issues from the GitHub Repository

These are confirmed issues in the ZXing.Net.Maui GitHub issue tracker, not speculation.

**iPhone 15 Pro auto-focus failure.** The camera view displays the barcode clearly but the auto-focus system fails to lock sharply enough for detection. iPhone 15 Pro and Pro Max (hardware identifiers `iPhone16,1` and `iPhone16,2`) are specifically cited. The workaround is to have the user physically move the phone closer or farther from the barcode until focus locks — an instruction that requires a visible UX affordance and user patience. There is no programmatic fix that reliably resolves this.

**Windows MAUI not supported.** The repository states explicitly that there is no Windows implementation and no plan to add one. This is not a temporary gap or a known issue being tracked — it is a structural limitation of a library built around platform camera APIs for iOS and Android. A MAUI project that targets `net8.0-windows10.0.19041.0` will not get barcode scanning from ZXing.Net.MAUI.

**Camera resource leak with no Dispose().** When the user navigates away from a scan page, `CameraBarcodeReaderView` does not release camera resources through a standard `IDisposable` pattern. The documented workaround is to set `cameraBarcodeReaderView.IsDetecting = false` in `OnDisappearing()` and reinitialize in `OnAppearing()`. This may reduce the leak in practice, but there is no formal `Dispose()` call available, and memory growth with repeated navigation cycles is a documented outcome.

**Android Camera 1.5.0 build errors.** Compatibility issues between ZXing.Net.MAUI and the Android Camera 1.5.0 library cause build failures on some Android configurations. The workaround is to pin the AndroidX camera libraries to an earlier version in the `.csproj`. This is a moving target that requires maintenance attention when updating dependencies.

## The ZXing.Net Inheritance Problem

ZXing.Net.MAUI inherits the full limitation set of ZXing.Net, documented separately in the [ZXing.Net comparison](../zxing-net/). The most operationally painful one in a MAUI context is the format specification requirement.

ZXing requires you to declare which barcode formats you want to scan before scanning begins. The `BarcodeReaderOptions` object receives a bitmask of `BarcodeFormats` values, and only those formats will be detected. If a barcode appears in the camera frame in a format you did not include, it will not be detected — silently, with no error. The user points the camera at a real barcode, nothing happens, and the app looks broken.

```csharp
// ZXing.Net.MAUI: format specification required
// Miss a format? That barcode will never be detected.
ReaderOptions = new BarcodeReaderOptions
{
    Formats = BarcodeFormats.QRCode |
              BarcodeFormats.Code128 |
              BarcodeFormats.Ean13 |
              BarcodeFormats.UpcA,
    TryHarder = true,
    AutoRotate = true
};

private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
{
    foreach (var barcode in e.Results)
        Console.WriteLine($"{barcode.Format}: {barcode.Value}");
}
```

If your users encounter a GS1 DataBar, an Aztec code, a MaxiCode, or any format you did not list, the scan fails. In a controlled environment — a warehouse where every item has a Code128 label — this is manageable. In any scenario where the barcode format is determined by suppliers, customers, or external systems, it becomes a support problem.

There is also no PDF support. ZXing.Net has no native PDF processing, and ZXing.Net.MAUI inherits that gap. A user who wants to scan a barcode from a PDF invoice, a digital ticket, or a downloaded shipping label must print it first, or find another solution.

## XAML Control and Lifecycle Management

The `CameraBarcodeReaderView` is a XAML control that occupies screen real estate and runs continuously while the page is visible. The pattern requires lifecycle management on every page that uses it:

```xml
<!-- ZXing.Net.Maui XAML: requires xmlns declaration and lifecycle wiring -->
<ContentPage xmlns:zxing="clr-namespace:ZXing.Net.Maui.Controls;assembly=ZXing.Net.MAUI.Controls">
    <zxing:CameraBarcodeReaderView
        x:Name="CameraView"
        Options="{Binding ReaderOptions}"
        BarcodesDetected="OnBarcodesDetected" />
</ContentPage>
```

```csharp
// Required on every page to avoid camera resource leaks
protected override void OnDisappearing()
{
    base.OnDisappearing();
    if (CameraView != null)
        CameraView.IsDetecting = false;  // No Dispose() — this is the best available option
}

protected override void OnAppearing()
{
    base.OnAppearing();
    if (CameraView != null)
        CameraView.IsDetecting = true;
}
```

Every scan page carries this lifecycle boilerplate. Forget it once, and the camera remains locked after navigation. The absence of `IDisposable` means there is no idiomatic C# pattern to rely on — this is manual resource management in a framework that otherwise handles it automatically.

## Platform Coverage

| Platform | ZXing.Net.MAUI | IronBarcode |
|---|---|---|
| iOS MAUI | Partial (iPhone 15 Pro auto-focus broken) | Yes |
| Android MAUI | Partial (Camera 1.5.0 build issues) | Yes |
| Windows MAUI | Not supported | Yes |
| macOS MAUI | Not supported | Yes |
| ASP.NET Core | No | Yes |
| Console / Server | No | Yes |
| WPF / WinForms | No | Yes |
| Azure Functions / Docker | No | Yes |
| .NET Framework 4.6.2+ | No | Yes |
| File input (image paths) | Via ZXing.Net core | Yes |
| Stream input | Via ZXing.Net core | Yes |
| PDF barcode extraction | No | Yes |
| Barcode generation | Yes (ZXing.Net core) | Yes |

## IronBarcode: The MediaPicker Pattern

IronBarcode takes a structurally different approach in MAUI. There is no camera view control, no event handler, no `IsDetecting` state to manage. The [MAUI desktop barcode pattern](https://ironsoftware.com/csharp/barcode/how-to/barcode-desktop-maui/) uses MAUI's `MediaPicker` to open the system camera, capture an image, and pass the resulting bytes to `BarcodeReader.Read()`.

```csharp
// NuGet: dotnet add package IronBarcode
// IronBarcode MAUI: stable, no pre-release, Windows supported
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

private async void ScanButton_Clicked(object sender, EventArgs e)
{
    var photo = await MediaPicker.CapturePhotoAsync();
    if (photo == null) return;

    using var stream = await photo.OpenReadAsync();
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);

    var results = BarcodeReader.Read(ms.ToArray());
    foreach (var barcode in results)
        Console.WriteLine($"{barcode.Format}: {barcode.Value}");
}
```

No format specification. No lifecycle management. No platform conditionals. The [.NET MAUI barcode scanner tutorial](https://ironsoftware.com/csharp/barcode/get-started/net-maui-barcode-scanner-reader-tutorial/) walks through this pattern end-to-end, including permissions setup for both platforms. The same code compiles and runs on Windows MAUI without any changes — `MediaPicker.CapturePhotoAsync()` maps to the Windows file picker when camera capture is requested.

## Feature Comparison

| Feature | ZXing.Net.MAUI | IronBarcode |
|---|---|---|
| Current version | 0.5.0 (pre-release) | Stable release |
| Release status | Pre-release | Production-ready |
| Commercial support | No (community) | Yes |
| iOS MAUI scanning | Yes (with focus bugs on iPhone 15 Pro) | Yes |
| Android MAUI scanning | Yes (with Camera 1.5.0 build issues) | Yes |
| Windows MAUI | Not supported | Yes |
| macOS MAUI | Not supported | Yes |
| Live camera viewfinder | Yes | No (MediaPicker system UI) |
| Requires format specification | Yes | No (auto-detection) |
| Camera resource lifecycle | Manual (`IsDetecting`) | Not applicable — stateless |
| `Dispose()` implementation | No | Not applicable |
| iPhone 15 Pro auto-focus | Broken (documented) | Not applicable |
| PDF barcode extraction | No | Yes |
| Server-side / ASP.NET Core | No | Yes |
| Damaged barcode recovery | Limited (TryHarder only) | Yes (ML-powered) |
| Formats detected automatically | No | Yes (50+) |
| Barcode generation | Yes | Yes |
| License | MIT (free) | Commercial |

## Code Side by Side

**ZXing.Net.MAUI — event-driven camera scanning:**

```csharp
// ZXing.Net.Maui.Controls v0.5.0 — pre-release
// Requires: format specification, lifecycle management
// Platforms: iOS (partial), Android (partial) — NOT Windows
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
        // Stop detecting after first result
        CameraView.IsDetecting = false;
    }

    // Required to prevent camera resource leaks (no Dispose() available)
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

**IronBarcode — capture and process:**

```csharp
// NuGet: dotnet add package IronBarcode
// IronBarcode — stable, stateless, all MAUI platforms
// Platforms: iOS, Android, Windows, macOS
using IronBarCode;

public partial class ScannerPage : ContentPage
{
    public ScannerPage()
    {
        IronBarCode.License.LicenseKey = "YOUR-KEY";
        InitializeComponent();
    }

    private async void ScanButton_Clicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.CapturePhotoAsync();
        if (photo == null) return;

        using var stream = await photo.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        // No format specification — detects all 50+ formats automatically
        var results = BarcodeReader.Read(ms.ToArray());
        var first = results.FirstOrDefault();
        ResultLabel.Text = first != null
            ? $"{first.Format}: {first.Value}"
            : "No barcode found";
    }

    // No lifecycle methods needed — no camera state to manage
}
```

The IronBarcode version is shorter, has no lifecycle boilerplate, detects any format without configuration, and runs on Windows MAUI without modification.

## When to Consider Each

**ZXing.Net.MAUI fits when:**

The project is iOS and Android only, there is no Windows requirement, the barcode formats are known in advance and controlled (warehouse, internal tools), the team is comfortable with pre-release dependencies, and budget is zero. For a prototype or internal tool scanning Code128 inventory labels on iPhones that are not iPhone 15 Pro, ZXing.Net.MAUI will work.

**IronBarcode fits when:**

The app targets Windows MAUI alongside iOS and Android. The barcode formats come from external suppliers, customers, or documents and cannot be predicted in advance. The application needs to read barcodes from PDFs or image files — shipping invoices, document management, batch processing. The scanner must work reliably on current-generation hardware. Production stability and commercial support are requirements, not nice-to-haves.

**The hard blockers:**

If you need Windows MAUI, ZXing.Net.MAUI is not an option — it is documented as unsupported. If you need PDF barcode extraction, ZXing.Net.MAUI (and its parent ZXing.Net) has no API for it. These are not gaps that can be closed with configuration; they are structural limitations.

[Reading barcodes on iOS](https://ironsoftware.com/csharp/barcode/get-started/ios/) with IronBarcode uses the same `MediaPicker` + `BarcodeReader.Read()` pattern as Android — no platform-specific camera management, no format lists, no lifecycle boilerplate on either platform.
