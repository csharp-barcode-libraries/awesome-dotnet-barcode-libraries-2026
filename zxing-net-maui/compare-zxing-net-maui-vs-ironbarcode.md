ZXing.Net.Maui.Controls v0.5.0. Still pre-release. Windows MAUI not supported. iPhone 15 Pro auto-focus documented as broken. Camera resource leak with no Dispose(). This is the library most .NET MAUI developers reach for first — and the one that most frequently surfaces production concerns before a project ships.

## Understanding ZXing.Net.MAUI

ZXing.Net.MAUI is a community-maintained .NET MAUI port of the ZXing.Net barcode library, developed and published by Jon Dick (GitHub: Redth) under the MIT license. It provides a XAML camera control, `CameraBarcodeReaderView`, that embeds a live barcode scanning viewfinder into MAUI pages. Developers wire a `BarcodesDetected` event to receive scan results as the camera captures frames. The library inherits the full barcode format engine from ZXing.Net, including its `BarcodeFormats` enum and `BarcodeReaderOptions` configuration model.

The package `ZXing.Net.Maui.Controls` is registered in `MauiProgram.cs` via `builder.UseBarcodeReader()` and is designed around the mobile camera pipeline for iOS and Android. The library is not backed by a commercial organization, carries no SLA, and has no paid support tier. Its v0.5.0 version number communicates pre-release status in semantic versioning terms.

Key architectural characteristics of ZXing.Net.MAUI:

- **Pre-Release Status:** The NuGet package is flagged as pre-release at v0.5.0. API stability, bug fix timelines, and production readiness guarantees are not provided by the community maintainer.
- **iOS and Android Only:** The library is built around platform camera APIs for iOS and Android. The Windows MAUI target is explicitly documented as unsupported with no stated plan to add it.
- **Continuous Camera Viewfinder:** `CameraBarcodeReaderView` is a live camera control that runs continuously while the page is visible. It occupies screen real estate and requires page lifecycle management.
- **No Dispose() Implementation:** The control does not implement `IDisposable`. Camera resources are not formally released on page navigation, requiring a manual `IsDetecting = false` workaround in `OnDisappearing()`.
- **iPhone 15 Pro Auto-Focus Issue:** The GitHub issue tracker for the repository documents that iPhone 15 Pro and Pro Max devices (hardware identifiers `iPhone16,1` and `iPhone16,2`) fail to achieve reliable focus for barcode detection. No programmatic fix is available.
- **Android Camera Compatibility Issue:** Compatibility conflicts between ZXing.Net.MAUI and the Android Camera 1.5.0 library cause build failures on some Android configurations, requiring manual dependency pinning in the project file.
- **Inherits ZXing.Net Format Specification:** Every scanning session requires explicit declaration of which `BarcodeFormats` values to scan. Formats not listed in `BarcodeReaderOptions.Formats` are silently ignored even when visible in the camera frame.
- **Camera-Only Architecture:** The library has no file input API, no stream reading API, and no PDF barcode extraction capability. All scanning must occur through the live camera viewfinder.

### The CameraBarcodeReaderView Architecture

The `CameraBarcodeReaderView` control is the central component of ZXing.Net.MAUI. It is declared in XAML and configured through a `BarcodeReaderOptions` binding. Every page that uses it must implement `OnAppearing` and `OnDisappearing` overrides to manage the `IsDetecting` state:

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

Every scan page in a ZXing.Net.MAUI application carries this lifecycle boilerplate. The absence of `IDisposable` means there is no idiomatic C# pattern to rely on — camera state is managed manually in a framework that otherwise handles resource cleanup automatically.

## Understanding IronBarcode

IronBarcode is a commercially supported .NET barcode library developed by Iron Software. It provides both barcode reading and generation through a static API, `BarcodeReader.Read()`, that accepts image bytes, file paths, streams, and PDF documents. In a MAUI context, IronBarcode pairs with MAUI's built-in `MediaPicker` to capture images through the system camera, then processes the resulting image after capture rather than processing a continuous camera stream.

The library targets all MAUI platforms — iOS, Android, Windows, and macOS — through the same package and the same code paths. No platform-specific service registration is required in `MauiProgram.cs`, no camera view control is embedded in XAML, and no lifecycle management is needed because IronBarcode does not maintain running camera state.

Key characteristics of IronBarcode:

- **Stable Commercial Release:** Published as a production-ready NuGet package with commercial support, SLA, and a regular release cadence.
- **All MAUI Platforms:** Supports iOS, Android, Windows, and macOS MAUI targets from a single package and a single code pattern.
- **Stateless API:** `BarcodeReader.Read()` is a static method call. No background processes run between scans, no lifecycle hooks are required, and no camera resources accumulate across page navigations.
- **Auto-Detection of All Formats:** Detects over 50 barcode formats automatically without a format specification list. No configuration is required to scan an unknown format.
- **File and PDF Reading:** Reads barcodes from file paths, byte arrays, streams, and PDF documents natively, covering scenarios that live camera libraries cannot address.
- **ML-Powered Damaged Barcode Recovery:** Applies machine learning models to recover barcodes from damaged, partially obscured, or low-quality images beyond what threshold algorithms can achieve.
- **Barcode Generation:** Generates all major 1D and 2D barcode formats as images with configurable sizing, color, and margin.
- **Cross-Deployment:** The same package runs in ASP.NET Core, WPF, WinForms, console applications, Azure Functions, and Docker containers alongside MAUI.

## Feature Comparison

The following table highlights the primary differences between ZXing.Net.MAUI and IronBarcode:

| Feature | ZXing.Net.MAUI | IronBarcode |
|---|---|---|
| **Release Status** | Pre-release (v0.5.0) | Stable, commercial release |
| **Platform Support** | iOS, Android (Windows unsupported) | iOS, Android, Windows, macOS, server |
| **Format Specification Required** | Yes | No (auto-detection) |
| **Camera Resource Management** | Manual (`IsDetecting`) | Not applicable — stateless |
| **PDF Barcode Extraction** | Not available | Yes |
| **License** | MIT (free, community) | Commercial |
| **Commercial Support** | None | Yes |

### Detailed Feature Comparison

| Feature | ZXing.Net.MAUI | IronBarcode |
|---|---|---|
| **Platform** | | |
| iOS MAUI | Yes (iPhone 15 Pro focus broken) | Yes |
| Android MAUI | Yes (Camera 1.5.0 build issues) | Yes |
| Windows MAUI | Not supported | Yes |
| macOS MAUI | Not supported | Yes |
| ASP.NET Core / Server | No | Yes |
| WPF / WinForms | No | Yes |
| Azure Functions / Docker | No | Yes |
| .NET Framework 4.6.2+ | No | Yes |
| **Reading** | | |
| Format auto-detection | No — must specify formats | Yes (50+ formats) |
| File path input | Via ZXing.Net core only | Yes |
| Stream input | Via ZXing.Net core only | Yes |
| PDF barcode extraction | No | Yes |
| Damaged barcode recovery | TryHarder only | Yes (ML-powered) |
| **Camera Integration** | | |
| Live viewfinder control | Yes | No (MediaPicker system UI) |
| Lifecycle management required | Yes (IsDetecting) | No |
| Dispose() implementation | No | Not applicable |
| iPhone 15 Pro auto-focus | Broken (documented) | Not applicable |
| **Generation** | | |
| Barcode generation | Yes (via ZXing.Net) | Yes |
| **Maintenance** | | |
| Release status | Pre-release | Production-ready |
| Commercial support | None | Yes |
| API stability guarantee | None (pre-release) | Yes |
| License | MIT (free) | Commercial |

## Platform Support

Platform coverage is a structural difference between ZXing.Net.MAUI and IronBarcode because the two libraries are built on fundamentally different assumptions about where .NET MAUI applications run.

### ZXing.Net.MAUI Approach

ZXing.Net.MAUI is designed exclusively for iOS and Android because it is built around platform camera APIs that do not have a Windows MAUI counterpart in the library's current implementation. The repository explicitly documents Windows MAUI as unsupported, and no roadmap exists for adding it. This is not a temporary gap — it reflects the architectural choice to build the library around a live camera viewfinder control whose platform implementations were written only for mobile operating systems.

A MAUI project that targets `net8.0-windows10.0.19041.0` will not get barcode scanning functionality from ZXing.Net.MAUI. macOS MAUI is similarly absent. Teams that start a project with mobile-only targets and later add a Windows or macOS requirement will find that ZXing.Net.MAUI cannot be part of the solution for those targets. The iOS support itself carries a caveat: iPhone 15 Pro and Pro Max devices are documented as affected by an auto-focus failure that prevents reliable detection.

### IronBarcode Approach

IronBarcode supports all MAUI target frameworks — iOS, Android, Windows, and macOS — from the same package and the same code pattern. The `MediaPicker` + `BarcodeReader.Read()` approach maps naturally to each platform: on mobile, `MediaPicker.CapturePhotoAsync()` invokes the device camera; on Windows, it maps to the file picker, which is appropriate behavior for a desktop environment. No platform-specific code, no conditional compilation, and no platform service registration is required.

The [MAUI desktop barcode pattern for Windows and macOS](https://ironsoftware.com/csharp/barcode/how-to/barcode-desktop-maui/) is covered by the same package that handles mobile scanning. The same `BarcodeReader.Read()` call that runs on an Android device also runs in an ASP.NET Core endpoint, a WinForms desktop application, or an Azure Function — the deployment target does not affect the API.

## Camera Integration and Lifecycle

The two libraries take opposite architectural positions on how camera access works in a MAUI application.

### ZXing.Net.MAUI Approach

`CameraBarcodeReaderView` is a persistent camera control embedded in the XAML page. It begins processing camera frames when `IsDetecting` is set to `true` and stops when it is set to `false`. The absence of a `Dispose()` implementation means camera resources are not released through the standard `IDisposable` pattern when the user navigates away. The documented mitigation is to set `IsDetecting = false` in `OnDisappearing()` and restore it in `OnAppearing()`:

```csharp
// ZXing.Net.MAUI: required on every page to prevent resource leaks
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
```

This pattern must be repeated on every page that hosts a scanner. Applications that navigate frequently to and from scan pages accumulate camera resource leaks that can manifest as memory growth, battery drain, and intermittent camera initialization failures on return. The iPhone 15 Pro auto-focus issue is a separate concern within the camera integration layer: the camera view renders the barcode clearly, but the auto-focus system does not lock sharply enough for detection on `iPhone16,1` and `iPhone16,2` hardware. The only documented workaround is to instruct the user to manually adjust the distance between the device and the barcode.

### IronBarcode Approach

IronBarcode does not embed a camera control in the XAML layout. Instead, the `MediaPicker.CapturePhotoAsync()` call opens the system camera UI when the user taps a button. The system camera handles focus, exposure, and auto-focus independently. When the user confirms the capture, the resulting image is passed to `BarcodeReader.Read()` as a byte array:

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
    foreach (var barcode in results)
        Console.WriteLine($"{barcode.Format}: {barcode.Value}");
}

// No OnAppearing or OnDisappearing needed — no camera state to manage
```

The [.NET MAUI barcode scanner tutorial](https://ironsoftware.com/csharp/barcode/get-started/net-maui-barcode-scanner-reader-tutorial/) covers the full project setup for this pattern, including permissions configuration for both iOS and Android. Because there is no persistent camera view, there is no resource to release, no state to toggle, and no lifecycle boilerplate to maintain across pages.

## Format Specification

How a library handles barcode format detection has direct consequences for scan reliability in real-world deployments.

### ZXing.Net.MAUI Approach

ZXing.Net.MAUI inherits the ZXing.Net format specification requirement. Before scanning begins, the developer must populate `BarcodeReaderOptions.Formats` with a bitmask of the `BarcodeFormats` enum values that should be detected. Formats not included in this list will not be detected — silently, with no error or warning:

```csharp
// ZXing.Net.MAUI: format specification required
// Any format not listed here will never be detected
ReaderOptions = new BarcodeReaderOptions
{
    Formats = BarcodeFormats.QRCode |
              BarcodeFormats.Code128 |
              BarcodeFormats.Ean13 |
              BarcodeFormats.UpcA,
    TryHarder = true,
    AutoRotate = true
};
```

If a user points the camera at a GS1 DataBar, an Aztec code, a MaxiCode, or any format that was not anticipated when the options were configured, the scan silently fails. The user sees a barcode that the camera renders clearly, nothing is detected, and the application appears broken. In a controlled environment — a warehouse where every item carries a Code128 label — this is manageable. In any deployment where barcode formats are determined by external suppliers, customers, or third-party systems, the silent miss becomes a recurring support issue.

### IronBarcode Approach

IronBarcode performs automatic format detection across all supported formats without any pre-configuration. `BarcodeReader.Read()` analyzes the image and returns results for every barcode it identifies, regardless of format. No `BarcodeReaderOptions` list is required:

```csharp
// IronBarcode: no format specification needed
// All 50+ formats detected automatically
var results = BarcodeReader.Read(imageBytes);
foreach (var barcode in results)
    Console.WriteLine($"{barcode.Format}: {barcode.Value}");
```

If performance tuning is needed for a controlled scenario where only one format is expected, format hints can be passed optionally — but they are never required for correct detection. A barcode in a format that was not anticipated by the developer will still be detected and returned.

## File and PDF Processing

Beyond live camera scanning, many production barcode workflows involve reading from stored documents — shipping invoices, digital tickets, uploaded images, and PDF attachments.

### ZXing.Net.MAUI Approach

ZXing.Net.MAUI is a camera control library. It has no API for reading barcodes from file paths, byte arrays passed directly from storage, or PDF documents. The `CameraBarcodeReaderView` control requires a live camera feed; there is no static method that accepts a file path and returns barcode results. Teams that need to read barcodes from uploaded PDFs, document management systems, or batch image processing queues cannot use ZXing.Net.MAUI for those scenarios and must introduce a separate library.

### IronBarcode Approach

IronBarcode reads barcodes from any source through the same `BarcodeReader.Read()` method. It accepts file paths, byte arrays, streams, and PDF documents. PDF parsing is native — IronBarcode processes PDF pages directly without an intermediate rasterization step:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

// Read barcodes from a PDF file
var results = BarcodeReader.Read("invoice.pdf");
foreach (var barcode in results)
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Format} — {barcode.Value}");

// Read from a user-selected file using MAUI FilePicker
var file = await FilePicker.PickAsync();
if (file != null)
{
    var fileResults = BarcodeReader.Read(file.FullPath);
    foreach (var result in fileResults)
        ResultLabel.Text += $"\n{result.Format}: {result.Value}";
}
```

The full PDF barcode reading workflow, including multi-page documents and mixed barcode formats across pages, is documented in the [read barcodes from PDF guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/). This covers shipping invoices, digital tickets, document management workflows, and batch processing scenarios that the camera-only architecture of ZXing.Net.MAUI cannot address.

## API Mapping Reference

| ZXing.Net.MAUI | IronBarcode | Notes |
|---|---|---|
| `builder.UseBarcodeReader()` | Not required | Remove from `MauiProgram.cs` |
| `xmlns:zxing="clr-namespace:ZXing.Net.Maui.Controls;..."` | Not required | Remove XAML namespace declaration |
| `<zxing:CameraBarcodeReaderView>` | Replace with `<Button>` + `MediaPicker` | Architectural change |
| `Options="{Binding ReaderOptions}"` | Not required | No options object needed |
| `BarcodesDetected="OnBarcodesDetected"` | `BarcodeReader.Read()` return value | Event → method return |
| `new BarcodeReaderOptions { Formats = BarcodeFormats.X \| ... }` | Not required | Auto-detection replaces this |
| `BarcodeDetectionEventArgs e` | `IEnumerable<BarcodeResult>` | Different result delivery model |
| `e.Results` | Return value of `BarcodeReader.Read()` | |
| `barcode.Value` | `result.Value` | Same property name |
| `barcode.Format` | `result.Format` | Same property name |
| `BarcodeFormats.QRCode` | `BarcodeEncoding.QRCode` | Enum rename |
| `BarcodeFormats.Code128` | `BarcodeEncoding.Code128` | Enum rename |
| `BarcodeFormats.Ean13` | `BarcodeEncoding.EAN13` | Enum rename |
| `CameraView.IsDetecting = false` (OnDisappearing) | Not required — remove the method | IronBarcode is stateless |
| `CameraView.IsDetecting = true` (OnAppearing) | Not required — remove the method | IronBarcode is stateless |
| No file input API | `BarcodeReader.Read("path/to/file.png")` | New capability |
| No PDF API | `BarcodeReader.Read("document.pdf")` | New capability |

## When Teams Consider Moving from ZXing.Net.MAUI to IronBarcode

Several concrete scenarios drive the decision to evaluate IronBarcode as a replacement for ZXing.Net.MAUI. These are project-level and product-level conditions, not implementation preferences.

### Windows MAUI Requirements

The most common trigger for re-evaluation is the addition of a Windows MAUI target to a project that started as iOS and Android only. MAUI teams frequently begin with mobile-first builds and expand to desktop targets as requirements evolve. When that expansion happens, ZXing.Net.MAUI provides no path forward — the Windows platform is not implemented and is not on a public roadmap. The team must either accept that barcode scanning will be unavailable on Windows or replace the library. Because the replacement involves a structural change to the scanning pattern regardless, teams typically make the change for all platforms at the same time.

### Current-Generation Hardware Compatibility

The documented iPhone 15 Pro auto-focus failure represents an exposure for any team shipping a production barcode scanner to iOS users. iPhone 15 Pro and Pro Max are flagship devices purchased by the consumer segment most likely to demand a polished application experience. A workaround that requires users to manually adjust their distance from a barcode is not a solution that survives a product review. Teams that surface this issue in QA, or receive support reports from iPhone 15 Pro users, face a choice between remaining on a library with no fix available or migrating to an approach that is not affected by the camera view focus model.

### File and Document Processing

Barcode scanning requirements rarely stay scoped to live camera capture. Applications that begin as inventory scanners frequently expand to include reading barcodes from uploaded PDFs, processing shipping invoices, or handling digital tickets from email attachments. ZXing.Net.MAUI has no API for any of these scenarios. When a product requirement lands on the backlog that involves reading a barcode from a file or document, teams using ZXing.Net.MAUI must introduce a separate library to handle it. If the team is already using IronBarcode for any of those file-based scenarios, consolidating the MAUI camera scanning into the same library becomes the natural next step.

### Production Stability Requirements

The v0.5.0 pre-release designation carries real implications for teams subject to software composition analysis, dependency audits, or internal approval processes. Many enterprise development environments require that production dependencies carry a stable release designation, a commercial support contract, or both. ZXing.Net.MAUI meets neither criterion. Community-maintained pre-release libraries are appropriate for internal tools and prototypes, but become a harder conversation in customer-facing applications where the barcode scanner is a primary workflow. The absence of a paid support tier means that any critical bug depends entirely on volunteer contributor availability.

## Common Migration Considerations

The structural change from ZXing.Net.MAUI to IronBarcode involves three specific technical replacements that affect every file in the codebase that participates in barcode scanning.

### CameraBarcodeReaderView to MediaPicker Pattern

The `CameraBarcodeReaderView` XAML control and its `xmlns:zxing` namespace declaration are removed entirely. In each XAML file that contained a scanner view, the replacement is a `Button` control that invokes `MediaPicker.CapturePhotoAsync()` in its `Clicked` handler. The event-driven model — where results arrive through `BarcodesDetected` — is replaced by reading the return value of `BarcodeReader.Read()` directly in the async handler.

### IsDetecting Lifecycle Removal

Every `OnAppearing` and `OnDisappearing` override that exists solely to toggle `CameraView.IsDetecting` can be deleted. If those overrides contain other page lifecycle logic, the `IsDetecting` lines are removed and the remaining logic is preserved. There is no IronBarcode equivalent to `IsDetecting` because there is no persistent camera state to manage between page navigations.

### UseBarcodeReader() Registration Removal

ZXing.Net.MAUI requires a one-time `builder.UseBarcodeReader()` call in `MauiProgram.cs` to register its platform camera services. IronBarcode does not require any `MauiProgram.cs` registration. The `UseBarcodeReader()` line is removed, and the `using ZXing.Net.Maui;` namespace import that supports it is removed alongside the package uninstall.

## Additional IronBarcode Capabilities

Beyond the scenarios covered in this comparison, IronBarcode provides barcode functionality that extends well past what a camera-based MAUI control can offer:

- **[iOS Barcode Scanning](https://ironsoftware.com/csharp/barcode/get-started/ios/):** Full iOS MAUI support using the same `MediaPicker` + `BarcodeReader.Read()` pattern — no platform-specific camera management, no format lists, and no lifecycle boilerplate.
- **[Android Barcode Scanning](https://ironsoftware.com/csharp/barcode/get-started/android/):** Android MAUI scanning through the same unified API, without the Camera 1.5.0 dependency pinning issues present in ZXing.Net.MAUI.
- **Barcode Generation:** Generates QR codes, Code128, EAN-13, PDF417, Data Matrix, and all major formats as images with configurable sizing, color, quiet zone, and error correction level.
- **Batch Processing:** Reads all barcodes from all pages of a multi-page PDF or a directory of images in a single call, returning page number metadata with each result.
- **Server-Side Deployment:** The same NuGet package and the same `BarcodeReader.Read()` call runs in ASP.NET Core endpoints, Azure Functions, and Docker containers — one package covers both the mobile client and the server backend.
- **Damaged Barcode Recovery:** Machine learning models recover barcodes from physically damaged labels, low-contrast prints, and images captured at suboptimal angles that standard threshold-based decoders cannot process.
- **Styled Barcode Generation:** Generates barcodes with custom colors, embedded logos, rounded corners, and annotation text — beyond the plain monochrome output available through ZXing.Net.

## .NET Compatibility and Future Readiness

IronBarcode maintains active development with regular updates targeting current .NET releases. The library supports .NET 8, .NET 9, and is updated for future releases including .NET 10 expected in late 2026. It also supports .NET Framework 4.6.2 and later for legacy application environments. ZXing.Net.MAUI, as a community-maintained pre-release package, does not carry formal commitments on .NET version support timelines. For MAUI projects — which are tied to the .NET release cadence — the availability of an actively maintained, commercially supported library that tracks each .NET version is relevant to long-term planning.

## Conclusion

ZXing.Net.MAUI and IronBarcode represent different answers to the same problem — reading barcodes in a .NET MAUI application — but they start from different architectural premises. ZXing.Net.MAUI embeds a live camera viewfinder directly in the XAML page, operating as a persistent camera control that fires events as frames are analyzed. IronBarcode treats the camera as a capture device accessed through the system `MediaPicker`, processing a static image after capture rather than a continuous stream. This difference in approach determines nearly everything else: platform coverage, lifecycle complexity, format handling, and deployment scope.

ZXing.Net.MAUI is genuinely appropriate for specific project profiles: iOS and Android applications, prototypes and internal tools, teams that know their barcode formats in advance, projects with no Windows MAUI requirement, and situations where the zero-cost MIT license is the deciding factor. For a warehouse inventory scanner on a fixed set of Android devices scanning Code128 labels, ZXing.Net.MAUI will function correctly. The pre-release status, the iPhone 15 Pro issue, and the lifecycle boilerplate are real trade-offs that are worth accepting in the right context.

IronBarcode is appropriate when the project scope is broader: Windows or macOS MAUI targets, unknown or variable barcode formats from external systems, requirements that include reading barcodes from PDFs or uploaded files, production-grade stability needs, or server-side barcode processing in addition to mobile scanning. The commercial license is an investment that covers support, maintenance, and compatibility updates across .NET versions. The stateless API removes a category of bugs — camera resource leaks, lifecycle state errors — that ZXing.Net.MAUI requires developers to prevent manually.

The honest evaluation is that neither library is universally correct. ZXing.Net.MAUI earns its position as the first library most MAUI developers try because it is free, familiar, and quick to integrate. The problems it carries are real, but they only matter in certain project conditions. When those conditions are present — a Windows requirement, current-generation iPhone hardware, file-based scanning, or production stability standards — IronBarcode addresses all of them. The choice depends on whether the project's specific constraints place it inside or outside the scenarios where ZXing.Net.MAUI's known limitations become blockers.
