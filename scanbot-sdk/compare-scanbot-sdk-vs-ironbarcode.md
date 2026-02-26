Scanbot SDK opens a full-screen barcode scanning view using the device camera. There is no `BarcodeScanner.Read(imagePath)` method. The scanner is the camera UI. If your barcode is in a PDF invoice on a server, Scanbot cannot help.

That is not a criticism — it is an architectural description. Scanbot SDK is a MAUI camera control that wraps native iOS and Android scanning APIs into a polished viewfinder component. `ScanbotBarcodeSDK.BarcodeScanner.Open(configuration)` hands control to a full-screen camera experience. The user points the device at a barcode, the SDK detects it in the live video feed, and the result returns to your app. That is the complete model. Scanbot makes no pretense of being a file processing library because it was never designed to be one.

The comparison matters because the NuGet package name — `ScanbotBarcodeSDK.MAUI` — and the product category — "barcode SDK" — can lead developers to evaluate it for server-side document processing, WPF desktop apps, or ASP.NET Core APIs. It will not compile in any of those contexts. This article explains why the two tools occupy different territory, what each one actually does, and where IronBarcode covers scenarios that Scanbot structurally cannot.

## What Scanbot SDK Actually Does

Scanbot's .NET story is `ScanbotBarcodeSDK.MAUI`, a package that targets `net8.0-android` and `net8.0-ios`. The SDK requires a `.NET MAUI Application` project with `<UseMaui>true</UseMaui>` and mobile targets in its `TargetFrameworks`. It will not resolve against a console app, a class library, an ASP.NET Core project, or a MAUI app targeting Windows.

The initialization sequence:

```csharp
// In App.xaml.cs or MauiProgram.cs
ScanbotSDK.Initialize(new ScanbotSDKConfiguration
{
    LicenseKey = "YOUR-SCANBOT-LICENSE-KEY",
    EnableLogging = false
});
```

And the scanning call:

```csharp
// Configure accepted formats and camera appearance
var configuration = new BarcodeScannerConfiguration();
configuration.BarcodeFormats = new[]
{
    BarcodeFormat.Code128,
    BarcodeFormat.QrCode,
    BarcodeFormat.Ean13
};

// Open full-screen camera scanner — UI takes over the entire screen
var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(configuration);

if (result.Status == OperationResult.Ok)
{
    foreach (var barcode in result.Barcodes)
        Console.WriteLine($"{barcode.Format}: {barcode.Text}");
}
```

There is no overload of `BarcodeScanner.Open()` that accepts a file path, a stream, or a byte array. The method signature requires a `BarcodeScannerConfiguration` — a camera UI configuration — because the entire operation is camera-driven. The SDK processes live video frames in real time, highlights detected barcodes in the viewfinder, and returns when a barcode is confirmed or the user cancels. That is the design.

Scanbot's camera experience is genuinely polished: torch control, aspect ratio configuration for the scan region, audio and haptic feedback, orientation locking, and a real-time overlay that highlights the detected barcode. For an iOS/Android consumer app where "point and scan" is the primary interaction model, this is a well-built tool.

## What IronBarcode Does Instead

IronBarcode is a file processing library. Its input sources are files, streams, byte arrays, and PDFs. There is no camera UI and no requirement for mobile hardware. The API reads whatever you hand it:

```csharp
// Install: dotnet add package IronBarcode
IronBarCode.License.LicenseKey = "YOUR-KEY";

// From a file path
var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
    Console.WriteLine($"{result.Value} ({result.Format})");

// From a PDF — reads all barcodes on all pages
var pdfResults = BarcodeReader.Read("invoice.pdf");
foreach (var result in pdfResults)
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");

// From a stream (useful for HTTP uploads)
using var stream = File.OpenRead("document.pdf");
var streamResults = BarcodeReader.Read(stream);

// With options
var options = new BarcodeReaderOptions { Speed = ReadingSpeed.Balanced };
var configuredResults = BarcodeReader.Read("image.png", options);
```

That same call runs in a console app, an ASP.NET Core controller, an Azure Function, a WPF form, or a MAUI page. The API does not change based on platform. There is no concept of "the camera" in the IronBarcode model because IronBarcode does not know or care how the image was produced. A file is a file.

IronBarcode [reads barcodes from PDFs natively](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) — not as images extracted from PDFs, but by parsing the PDF directly and finding barcodes on each page. This is a capability Scanbot has no equivalent for, because the PDF would need to be on a mobile device, open in a camera viewfinder, and physically held in front of the lens.

## Platform Coverage

This is where the difference becomes a hard blocker rather than a design preference.

| Project Type | Scanbot SDK | IronBarcode |
|---|---|---|
| .NET MAUI (iOS + Android) | Yes | Yes |
| .NET MAUI (Windows) | No | Yes |
| .NET MAUI (macOS) | No | Yes |
| Console Application | No | Yes |
| ASP.NET Core Web API | No | Yes |
| ASP.NET Core MVC | No | Yes |
| Blazor Server | No | Yes |
| WPF Application | No | Yes |
| WinForms Application | No | Yes |
| Azure Functions | No | Yes |
| AWS Lambda | No | Yes |
| Docker / Linux | No | Yes |
| Windows Service | No | Yes |
| .NET Framework 4.6.2+ | No | Yes |

Scanbot SDK supports one row in that table. IronBarcode supports all of them.

The MAUI desktop gap matters more than it might initially appear. A common MAUI development scenario targets iOS, Android, and Windows in a single project. Scanbot's package references will fail to resolve on the Windows target. Teams discover this during the first Windows build, not at NuGet install time. For a [MAUI application that also targets Windows or macOS desktop](https://ironsoftware.com/csharp/barcode/how-to/barcode-desktop-maui/), Scanbot is structurally incompatible.

## Licensing

Scanbot uses a yearly flat fee model. The annual license cost is fixed regardless of how many scans the application performs — 100 scans per year or 10 million scans per year, the fee is the same. This is predictable, which is a genuine advantage for teams with stable mobile-only deployments.

IronBarcode is a one-time perpetual purchase: Lite at $749, Plus at $1,499, Professional at $2,999, Unlimited at $5,999. There is no annual renewal requirement for software updates within the license tier. Volume is not a pricing variable for either product.

## Code Side by Side

The same task — scanning a Code128 barcode from a mobile device — looks different in each tool because the input model is different.

**Scanbot SDK: camera-driven workflow**

```csharp
// Requires: .NET MAUI project, iOS or Android target, camera permissions
using ScanbotBarcodeSDK.MAUI;

ScanbotSDK.Initialize(new ScanbotSDKConfiguration
{
    LicenseKey = "YOUR-SCANBOT-LICENSE-KEY",
    EnableLogging = false
});

var configuration = new BarcodeScannerConfiguration
{
    BarcodeFormats = new[] { BarcodeFormat.Code128, BarcodeFormat.QrCode },
    FinderAspectRatio = new AspectRatio(1, 1),
    TopBarBackgroundColor = Colors.DarkBlue
};

var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(configuration);

if (result.Status == OperationResult.Ok)
{
    foreach (var barcode in result.Barcodes)
        await DisplayAlert("Scanned", $"{barcode.Format}: {barcode.Text}", "OK");
}
```

**IronBarcode: file-driven workflow (MAUI with MediaPicker)**

```csharp
// Works in MAUI, ASP.NET Core, console, WPF — same code everywhere
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// In a MAUI app: capture photo, read barcodes from the resulting image
private async void ScanButton_Clicked(object sender, EventArgs e)
{
    var photo = await MediaPicker.CapturePhotoAsync();
    if (photo == null) return;

    using var stream = await photo.OpenReadAsync();
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);

    var results = BarcodeReader.Read(ms.ToArray());
    foreach (var result in results)
        await DisplayAlert("Scanned", $"{result.Format}: {result.Value}", "OK");
}
```

The IronBarcode version gives up the real-time viewfinder. The user sees the system camera UI, takes a photo, and returns to the app — no live overlay, no continuous detection. In exchange, the same `BarcodeReader.Read()` call runs in a server-side endpoint processing uploaded documents, in a background job extracting barcodes from PDF archives, and in a Windows desktop app scanning image files. The [.NET MAUI barcode scanning tutorial](https://ironsoftware.com/csharp/barcode/get-started/net-maui-barcode-scanner-reader-tutorial/) covers the full MAUI integration pattern for both iOS and Android.

## Feature Comparison

| Feature | Scanbot SDK | IronBarcode |
|---|---|---|
| Live camera viewfinder | Yes — full-screen scanner UI | No (use MediaPicker for capture) |
| Real-time frame scanning | Yes | No |
| Input from file path | No | Yes |
| Input from stream | No | Yes |
| Input from byte array | No | Yes |
| PDF barcode extraction | No | Yes |
| Barcode generation | No | Yes |
| iOS MAUI | Yes | Yes |
| Android MAUI | Yes | Yes |
| Windows MAUI | No | Yes |
| macOS MAUI | No | Yes |
| ASP.NET Core | No | Yes |
| Console / Server | No | Yes |
| Azure / Lambda / Docker | No | Yes |
| ML error correction | No | Yes |
| Damaged barcode recovery | No | Yes |
| Auto format detection | Yes | Yes |
| 1D formats | 20+ | 30+ |
| 2D formats | QR, DataMatrix, PDF417, Aztec | QR, DataMatrix, PDF417, Aztec, MaxiCode |
| Desktop MAUI (Windows/macOS) | No | Yes |
| Offline processing | Yes | Yes |
| License model | Yearly flat fee | One-time perpetual |

## When to Use Each

**Scanbot SDK makes sense when:**

You are building a consumer-facing iOS or Android mobile app where the live camera scanning experience — real-time viewfinder, animated scan region, haptic feedback — is central to the product UX. Retail self-checkout, warehouse item lookup, ticket validation at the door: scenarios where the user is actively pointing a device camera at a physical barcode and expects visual confirmation. Scanbot has invested in that experience and the UI component reflects it. If the SDK's yearly flat fee fits your budget and your deployment is mobile-only, it is a reasonable choice for that narrow case.

**IronBarcode makes sense when:**

You need barcode processing in any context that is not a live mobile camera UI: a server-side API that accepts document uploads, an ASP.NET Core endpoint that extracts barcodes from incoming PDFs, a WPF or WinForms desktop app, a console batch job, an Azure Function triggered by blob storage, or a MAUI app that also targets Windows. IronBarcode also covers the mobile scenarios where Scanbot falls short — [iOS MAUI barcode reading](https://ironsoftware.com/csharp/barcode/get-started/ios/) and [Android MAUI barcode reading](https://ironsoftware.com/csharp/barcode/get-started/android/) both work through the `MediaPicker` + `BarcodeReader.Read()` pattern. The gap is the real-time viewfinder, not barcode detection accuracy.

**Both, for mixed requirements:**

Some teams use Scanbot for the mobile camera UI and IronBarcode for server-side document processing. This is architecturally clean — each tool handles the scenario it was designed for. The cost is maintaining two barcode dependencies and two license agreements.

## The Scope Mismatch in Practice

The teams most likely to discover the scope mismatch late are those evaluating Scanbot for a project that started mobile-only and grew. The MAUI app ships, requirements expand to include a web portal that processes uploaded PDFs, or the desktop Windows target gets added to the roadmap. Scanbot cannot grow with the project. `dotnet add package ScanbotBarcodeSDK.MAUI` to an ASP.NET Core project produces build errors; adding it to a MAUI project with a Windows target breaks the Windows build.

IronBarcode's single package handles the entire expansion path — from the original mobile MAUI app through the server API and the Windows desktop companion — with the same `BarcodeReader.Read()` call throughout.
