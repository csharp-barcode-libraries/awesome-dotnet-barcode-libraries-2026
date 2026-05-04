# barKoder SDK vs IronBarcode: Barcode SDK Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

barKoder SDK is a mobile-first barcode scanning solution built around live camera capture on iOS and Android, with hybrid framework bindings for React Native, Flutter, Cordova, and Capacitor. Since 2023 barKoder also ships an official `Plugin.Maui.Barkoder` NuGet package and a `Barkoder.Xamarin` NuGet package, so .NET developers building MAUI or Xamarin mobile apps now have a supported integration path. What barKoder still does not offer is a general-purpose .NET library: there is no server-side, console, ASP.NET Core, or document-processing surface, no file or PDF input mode, and no barcode generation capability. This comparison clarifies exactly where barKoder fits in a .NET stack today, and where [IronBarcode](https://ironsoftware.com/csharp/barcode/) is the appropriate choice instead.

## Table of Contents

1. [What is barKoder SDK?](#what-is-barkoder-sdk)
2. [Where barKoder Fits in a .NET Stack](#where-barkoder-fits-in-a-net-stack)
3. [Platform Support Comparison](#platform-support-comparison)
4. [Why Include This Comparison?](#why-include-this-comparison)
5. [For .NET Developers: Your Options](#for-net-developers-your-options)
6. [Code Comparison](#code-comparison)
7. [When to Use Each](#when-to-use-each)
8. [Migration Guide](#migration-guide)

---

## What is barKoder SDK?

barKoder is a commercial barcode scanning SDK from barKoder Ltd., designed specifically for mobile camera scanning with their proprietary MatrixSight algorithm for damaged and hard-to-read barcodes.

### Core Product Focus

**What barKoder SDK Does:**
- Real-time camera-based barcode scanning
- MatrixSight algorithm for damaged barcode recovery
- Multi-barcode scanning from a single camera frame
- Optimized for logistics, automotive, travel, and retail
- On-device processing (no cloud dependency)

**Technical Architecture:**
- Native iOS SDK (Swift/Objective-C)
- Native Android SDK (Kotlin/Java)
- React Native plugin
- Cordova plugin
- Capacitor plugin
- Flutter plugin
- .NET MAUI plugin (`Plugin.Maui.Barkoder`)
- Xamarin plugin (`Barkoder.Xamarin`)

**What barKoder Does NOT Provide:**
- A general-purpose .NET library for non-MAUI projects
- Server-side / ASP.NET Core / Azure Functions / Docker support
- Console or worker-service support
- File-based or PDF-based barcode reading
- Barcode generation (it is a reader-only SDK)

### Target Market

barKoder serves mobile developers building apps for:
- Logistics and supply chain
- Automotive manufacturing
- Travel and transportation
- Retail and inventory
- Healthcare (asset tracking)

All of these use cases involve mobile workers scanning physical barcodes with device cameras.

### MatrixSight Technology

barKoder promotes their MatrixSight algorithm as capable of:
- Scanning damaged or partially obscured barcodes
- Reading barcodes under poor lighting conditions
- Handling unusual angles and perspectives
- Processing 1D barcodes with damage

This is a legitimate strength for their target mobile scanning scenarios.

---

## Where barKoder Fits in a .NET Stack

The most important fact for .NET developers reading this comparison:

**barKoder is a mobile camera-scanning SDK. It has a .NET MAUI / Xamarin plugin, but no general-purpose .NET library.**

### What Is Available on .NET

barKoder publishes two .NET packages on NuGet.org:

- `Plugin.Maui.Barkoder` — official .NET MAUI plugin (current version 1.6.8, last updated March 2026, around 15K total downloads). Targets .NET 8.0+ on iOS and Android. Windows and Mac Catalyst targets are listed on barKoder's MAUI product page; verify per-platform binary support against the latest release notes before relying on them.
- `Barkoder.Xamarin` — official Xamarin plugin for Xamarin.Forms / iOS / Android.

Both wrap the underlying native iOS and Android SDKs and surface a `BarkoderView` control for camera scanning, configuration via methods such as `SetFlashEnabled` / `SetZoomFactor`, an `IBarkoderDelegate` callback that delivers a `BarcodeResult[]`, and result properties `TextualData` and `BarcodeTypeName`.

### What Is Not Available on .NET

Outside MAUI and Xamarin, there is no barKoder integration:

- No general-purpose `Barkoder.NET` package on NuGet
- No ASP.NET Core, Azure Functions, AWS Lambda, Docker, or Linux server target
- No console, worker service, WPF, WinForms, Avalonia, or Blazor target
- No file-path, byte-array, or stream input — input is always a live camera frame
- No PDF reading
- No barcode generation — barKoder is read-only

### Official Platform List

barKoder's published platform list, as of this verification pass, is:

- iOS Native
- Android Native
- React Native
- Cordova / Capacitor
- Flutter
- .NET MAUI
- Xamarin
- Web (WASM)

Notably absent from any direct support: ASP.NET Core, console / worker, desktop (.NET on Windows/Linux/macOS), and any server-side processing model.

### Why This Article Exists

Developers searching for a .NET barcode library may find barKoder, install `Plugin.Maui.Barkoder`, and then discover that it is exclusively a camera-scanning component for MAUI mobile apps — not a library that processes images, PDFs, or streams from a backend. This article maps that boundary: barKoder for MAUI camera UI, IronBarcode for everything else.

---

## Platform Support Comparison

### barKoder Platform Matrix

| Platform | barKoder Support | Notes |
|----------|------------------|-------|
| iOS Native (Swift) | Yes | Primary platform |
| Android Native (Kotlin) | Yes | Primary platform |
| React Native | Yes | Official plugin |
| Cordova | Yes | Official plugin |
| Capacitor | Yes | Official plugin |
| Flutter | Yes | Official plugin |
| .NET MAUI | Yes | `Plugin.Maui.Barkoder` (camera UI only) |
| Xamarin | Yes | `Barkoder.Xamarin` (camera UI only) |
| Web (WASM) | Yes | JavaScript SDK |
| ASP.NET Core | **No** | No server-side surface |
| Console / Worker (.NET) | **No** | Mobile-only SDK |
| WPF / WinForms / Avalonia | **No** | Not supported |
| Linux Server / Docker | **No** | Not supported |
| File / Stream / PDF input | **No** | Camera frame input only |
| Barcode Generation | **No** | Reader-only SDK |

### IronBarcode Platform Matrix

| Platform | IronBarcode Support | Notes |
|----------|---------------------|-------|
| iOS / Android (via MAUI) | Yes | Programmatic processing |
| React Native | No | Wrong stack |
| Cordova / Capacitor | No | Wrong stack |
| .NET / C# | Yes | Primary platform |
| .NET MAUI | Yes | Full support (programmatic, not camera UI) |
| Xamarin | Yes | Supported |
| Windows Desktop | Yes | WPF, WinForms, Avalonia |
| Linux Server | Yes | Full support |
| ASP.NET Core | Yes | Full support |
| Console Apps | Yes | Full support |
| Azure Functions | Yes | Full support |
| Docker | Yes | Full support |
| File / Stream / PDF input | Yes | Native |
| Barcode Generation | Yes | All major formats |

### The Real Boundary

```
barKoder: live camera frames on a mobile device
          (iOS, Android, MAUI, Xamarin, RN, Flutter, Cordova, Capacitor)

IronBarcode: image / stream / PDF input across the .NET ecosystem
             (server, desktop, MAUI, console, Docker, cloud functions)
```

If your need is a camera-scanning UI inside a MAUI mobile app, barKoder is a credible commercial option alongside Scandit, Scanbot, and the open-source MAUI camera libraries.

If your need is reading or generating barcodes anywhere else in a .NET stack — a backend API, a document-processing service, a desktop application, a worker that consumes PDFs — barKoder is not an option, regardless of whether it appears on NuGet.

---

## Why Include This Comparison?

### Developer Search Patterns

Developers researching barcode solutions often search for:
- "best .NET barcode SDK 2026"
- "MAUI barcode scanning library"
- "barcode scanner SDK comparison"

These searches surface barKoder alongside general-purpose .NET libraries. Without clear scope documentation, developers may install `Plugin.Maui.Barkoder` for a server or desktop project where it cannot help.

### Preventing Confusion

This comparison serves to:

1. **Clarify the scope** — barKoder is mobile-camera-only, even on .NET
2. **Save research time** for developers with non-mobile use cases
3. **Redirect appropriately** to libraries built for the actual use case
4. **Acknowledge** barKoder's strength in its real domain

### Fair Assessment

barKoder is a legitimate product that serves its target market — mobile camera scanning — well. This comparison is not a criticism of barKoder. It is clarification of where its boundary sits in a .NET stack: MAUI/Xamarin camera UI, nothing else.

If you are building a .NET MAUI app that needs damage-tolerant camera scanning, barKoder is worth evaluating.

If you are building anything else in .NET, IronBarcode covers the use case barKoder cannot.

---

## For .NET Developers: Your Options

### If You Need Camera Scanning in .NET MAUI

For real-time camera scanning in .NET MAUI mobile apps:

**Commercial Options:**
- **barKoder** (`Plugin.Maui.Barkoder`) — MatrixSight damage recovery, multi-scan
- **Scandit SDK** — enterprise mobile scanning with AR features
- **Scanbot SDK** — MAUI package for camera scanning

**Open-Source Options:**
- **BarcodeScanning.Native.MAUI** — native platform camera APIs
- **ZXing.Net.MAUI** — ZXing wrapper for MAUI

### If You Need File / Document / Server-Side Processing

For processing barcodes from images, PDFs, or documents in any .NET project:

**IronBarcode** — works in any .NET project:

```bash
dotnet add package BarCode
```

```csharp
using IronBarCode;

// Read from image
var results = BarcodeReader.Read("barcode.png");

// Read from PDF
var pdfResults = BarcodeReader.Read("invoice.pdf");

// Generate barcode
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128)
    .SaveAsPng("output.png");
```

### If You Need Both

For organizations needing both mobile camera capture and server-side document processing:

- **Camera UI** (MAUI mobile): use barKoder, Scandit, Scanbot, or an open-source MAUI camera library
- **Document / PDF / server processing**: use IronBarcode

### Decision Tree for .NET Developers

```
Are you using .NET?
├── NO: Consider barKoder if using React Native / Cordova / Flutter
│
└── YES
    │
    ├── Need a camera-scanning UI inside a MAUI / Xamarin mobile app?
    │   ├── Commercial: barKoder, Scandit, Scanbot
    │   └── Open-source: BarcodeScanning.Native.MAUI, ZXing.Net.MAUI
    │
    ├── Need image / stream / PDF barcode reading in a backend, desktop,
    │   or non-camera context?
    │   └── IronBarcode (barKoder cannot do this)
    │
    ├── Need to generate barcodes?
    │   └── IronBarcode (barKoder cannot do this)
    │
    └── Need both camera UI and document processing?
        ├── Camera: MAUI camera library (barKoder, Scandit, Scanbot, etc.)
        └── Files / server: IronBarcode
```

---

## Code Comparison

### barKoder MAUI: Camera-Only Surface

barKoder's `Plugin.Maui.Barkoder` exposes a `BarkoderView` control and an `IBarkoderDelegate` callback. There is no file or stream input.

```csharp
// NuGet: dotnet add package Plugin.Maui.Barkoder
// XAML hosts a BarkoderView; this is the code-behind callback.
using Barkoder.Maui;

public partial class ScanPage : ContentPage, IBarkoderDelegate
{
    public ScanPage()
    {
        InitializeComponent();
        // BarkoderView in XAML, license set via Barkoder portal
        BarkoderView.SetFlashEnabled(false);
        BarkoderView.SetZoomFactor(1.5f);
        BarkoderView.StartScanning(this);
    }

    public void DidFinishScanning(BarcodeResult[] result)
    {
        foreach (var r in result)
        {
            Console.WriteLine($"Type: {r.BarcodeTypeName}");
            Console.WriteLine($"Value: {r.TextualData}");
        }
    }
}
```

This works in a MAUI mobile app. It does not work in ASP.NET Core, in a console app, or against a file or PDF.

### IronBarcode: Full .NET Surface

```csharp
// Install: dotnet add package BarCode
using IronBarCode;

// License configuration
IronBarCode.License.LicenseKey = "YOUR-KEY";

// Read barcode from image
var results = BarcodeReader.Read("barcode.png");
foreach (var barcode in results)
{
    Console.WriteLine($"Type: {barcode.BarcodeType}");
    Console.WriteLine($"Value: {barcode.Value}");
}

// Read barcodes from a PDF (barKoder cannot do this)
var pdfResults = BarcodeReader.Read("document.pdf");
foreach (var barcode in pdfResults)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Value}");
}

// Generate a barcode (barKoder cannot do this)
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("output.png");
```

For detailed scope documentation, see [barKoder Mobile-Only Scope Example](barkoder-no-dotnet.cs).

---

## When to Use Each

### When barKoder Is The Right Choice

Consider barKoder if:

1. **You're building a mobile camera-scanning UI** — MAUI, Xamarin, React Native, Flutter, Cordova, Capacitor
2. **Damaged barcode scanning is critical** — MatrixSight algorithm
3. **You only need reading, not generation** — barKoder is read-only
4. **All input arrives via a device camera** — not from files, PDFs, or streams

### When IronBarcode Is The Right Choice

Choose IronBarcode if:

1. **You're building a non-camera .NET application** — ASP.NET Core, Azure Functions, Docker, console, WPF, WinForms, Avalonia
2. **You need to process images, streams, or byte arrays** — uploads, batch jobs, document pipelines
3. **You need to read barcodes from PDFs** — native, no pre-rendering required
4. **You need to generate barcodes** — barcode and QR code creation in the same library
5. **You need MAUI programmatic processing** — process captured images / gallery images, not run a camera UI

### The Simple Rule

| Your Need | Your Option |
|-----------|-------------|
| MAUI / Xamarin camera UI | barKoder, Scandit, Scanbot, or open-source MAUI camera lib |
| Server-side / file / PDF / generate | IronBarcode |
| Non-MAUI desktop or console reading | IronBarcode |
| React Native / Flutter / Cordova / Capacitor camera | barKoder or another mobile SDK |

The two libraries solve different problems even where both are technically present on NuGet.

---

## Migration Guide

### Migration Scope: From barKoder MAUI to IronBarcode

Migrating from `Plugin.Maui.Barkoder` to IronBarcode is a meaningful exercise only when the original camera-scanning use case is being replaced — for example, when input is moving from a live camera to gallery images, uploaded files, or PDFs.

If you have an existing MAUI app that uses barKoder for live camera scanning and you simply want to keep camera scanning, IronBarcode is not a drop-in replacement; pair barKoder (or another camera library) for the camera surface with IronBarcode for any file or PDF processing the app also needs.

### From MAUI Camera UI to MAUI File / Gallery Processing

**Old code (barKoder camera UI in MAUI):**
```csharp
using Barkoder.Maui;

public partial class ScanPage : ContentPage, IBarkoderDelegate
{
    public ScanPage()
    {
        InitializeComponent();
        BarkoderView.StartScanning(this);
    }

    public void DidFinishScanning(BarcodeResult[] result)
    {
        foreach (var r in result)
            HandleResult(r.TextualData);
    }
}
```

**New code (IronBarcode for captured / picked images in MAUI):**
```csharp
// dotnet add package BarCode
using IronBarCode;

public async Task ProcessCapturedImage()
{
    var photo = await MediaPicker.CapturePhotoAsync();
    if (photo != null)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        using var source = await photo.OpenReadAsync();
        using var dest = File.OpenWrite(path);
        await source.CopyToAsync(dest);

        var results = BarcodeReader.Read(path);
        foreach (var barcode in results)
        {
            HandleResult(barcode.Value);
        }
    }
}
```

### From Mobile-Only to Server Processing

A common pattern: keep barKoder for the mobile camera UI and add IronBarcode for the backend.

**Mobile App (barKoder for MAUI / React Native):**
- Continue using barKoder for camera scanning
- Send captured images or barcode payloads to the server

**Server API (IronBarcode for .NET):**
- Add IronBarcode for document processing
- Process uploaded files, PDFs, and images
- Generate barcodes for labels and documents

```csharp
// ASP.NET Core API endpoint
[HttpPost("process-document")]
public IActionResult ProcessDocument(IFormFile file)
{
    using var stream = file.OpenReadStream();
    var results = BarcodeReader.Read(stream);

    return Ok(results.Select(b => new
    {
        Type = b.BarcodeType.ToString(),
        Value = b.Value,
        Page = b.PageNumber
    }));
}
```

This hybrid approach uses each library for its strength.

### Migration Checklist

If transitioning from barKoder MAUI camera scanning to IronBarcode file processing (or to a hybrid):

**Planning:**
- [ ] Confirm whether camera UI is still required, or if input is moving to files / PDFs
- [ ] List barcode formats and image sources in scope
- [ ] Decide whether barcode generation is also required
- [ ] Identify deployment targets — MAUI client, ASP.NET Core, Docker, etc.

**For Camera Scanning (if still required):**
- [ ] Keep `Plugin.Maui.Barkoder`, or evaluate Scandit / Scanbot / open-source MAUI alternatives
- [ ] Confirm license model (annual, per-app, per-device)

**For File / PDF / Generation:**
- [ ] Install `BarCode` NuGet package
- [ ] Configure `IronBarCode.License.LicenseKey`
- [ ] Replace any "scan from camera" pipeline that actually consumes uploads with `BarcodeReader.Read()`
- [ ] Add `BarcodeWriter` calls for generation requirements
- [ ] Test PDF extraction if relevant

**For Both:**
- [ ] Adopt a hybrid architecture: barKoder for camera, IronBarcode for files / server
- [ ] Document which library handles which input source

For platform comparison code, see [barKoder Platform Comparison Example](barkoder-platform-comparison.cs).

---

## Additional Resources

### Documentation Links

- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/) — official .NET barcode library guides
- [IronBarcode on NuGet](https://www.nuget.org/packages/BarCode) — package download
- [Plugin.Maui.Barkoder on NuGet](https://www.nuget.org/packages/Plugin.Maui.Barkoder) — barKoder's official MAUI plugin
- [barKoder .NET MAUI Documentation](https://barkoder.com/docs/v1/maui) — official MAUI integration guide
- [barKoder SDK Documentation](https://docs.barkoder.com/) — overall barKoder documentation

### Code Example Files

Working code demonstrating the concepts in this article:

- [barKoder Mobile-Only Scope](barkoder-no-dotnet.cs) — what barKoder does and does not cover on .NET
- [Platform Comparison](barkoder-platform-comparison.cs) — technology-stack decision guide

### Related Comparisons

- [Scandit SDK Comparison](../scandit-sdk/) — enterprise mobile scanning with .NET MAUI support
- [Scanbot SDK Comparison](../scanbot-sdk/) — mobile SDK with .NET MAUI package
- [BarcodeScanning.MAUI Comparison](../barcodescanning-maui/) — open-source MAUI camera library
- [ZXing.Net.MAUI Comparison](../zxing-net-maui/) — open-source ZXing wrapper for MAUI

---

## Summary

**barKoder SDK:**
- Mobile camera-scanning SDK across iOS, Android, React Native, Flutter, Cordova, Capacitor, MAUI, and Xamarin
- Official `Plugin.Maui.Barkoder` and `Barkoder.Xamarin` NuGet packages
- MatrixSight algorithm for damaged barcode recovery
- Reader-only — no barcode generation
- Camera-frame input only — no file, stream, or PDF input
- No server-side, console, desktop, ASP.NET Core, or non-MAUI .NET surface

**For .NET developers:**
- Use barKoder (or alternatives) for MAUI / Xamarin camera UI
- Use IronBarcode for file / stream / PDF reading and barcode generation across the entire .NET ecosystem
- A hybrid architecture is common: barKoder on the mobile client, IronBarcode on the server

**Key takeaway:** barKoder is the right tool for a damage-tolerant mobile camera-scanning UI. IronBarcode is the right tool for everything else a .NET application does with barcodes.

---

*Last verified: May 2026*
*Author: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*
