# Scanbot SDK vs IronBarcode: C# Barcode SDK Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

Scanbot SDK is a mobile document scanning platform that includes barcode scanning capabilities for iOS, Android, and .NET MAUI applications. With a focus on camera-based scanning and offline processing, Scanbot positions itself as a privacy-focused alternative to cloud-dependent solutions. This comparison examines Scanbot's MAUI-only .NET support, its camera-centric architecture, and how it compares to [IronBarcode](https://ironsoftware.com/csharp/barcode/)'s universal .NET compatibility for programmatic barcode processing scenarios.

## Table of Contents

1. [What is Scanbot SDK?](#what-is-scanbot-sdk)
2. [Platform Limitation: MAUI Mobile Only](#platform-limitation-maui-mobile-only)
3. [Capabilities Comparison](#capabilities-comparison)
4. [Camera Focus vs Programmatic Processing](#camera-focus-vs-programmatic-processing)
5. [Code Comparison](#code-comparison)
6. [When to Use Each](#when-to-use-each)
7. [IronBarcode on Mobile](#ironbarcode-on-mobile)
8. [Migration Guide](#migration-guide)

---

## What is Scanbot SDK?

Scanbot is a German software company that provides mobile document scanning SDKs with integrated barcode recognition. The SDK is designed primarily for mobile applications that need to capture and process documents using the device camera.

### Core Product Focus

**What Scanbot SDK Does:**
- Mobile camera-based barcode scanning
- Document scanning with edge detection and perspective correction
- Optical Character Recognition (OCR)
- Multi-format barcode detection from camera feed
- Offline processing (no server required)

**Technical Architecture:**
- Native iOS SDK (Swift/Objective-C)
- Native Android SDK (Kotlin/Java)
- .NET MAUI SDK (ScanbotBarcodeSDK.MAUI)
- Camera-first design with viewfinder UI components
- Completely offline - no network code included

### NuGet Package

The .NET package is specifically for MAUI mobile development:

```bash
dotnet add package ScanbotBarcodeSDK.MAUI
```

Current version as of 2026: 7.1.1

### Licensing Model

Scanbot uses a yearly flat fee licensing model (non-volume-based), which differentiates it from per-scan or per-device pricing models used by some competitors. This makes costs more predictable, though still requires annual renewal.

---

## Platform Limitation: MAUI Mobile Only

The most significant limitation for .NET developers evaluating Scanbot SDK is its platform restriction.

### What "MAUI Mobile Only" Means

Scanbot's .NET SDK specifically requires:

1. **.NET MAUI framework** - Cannot be used without MAUI project structure
2. **Mobile platforms** - iOS and Android only
3. **Native platform dependencies** - Uses iOS and Android native APIs
4. **Camera hardware** - Designed for camera input, not file processing

### Where Scanbot Does NOT Work

| Project Type | Scanbot SDK | IronBarcode |
|--------------|-------------|-------------|
| Console Application | No | Yes |
| ASP.NET Core Web API | No | Yes |
| ASP.NET Core MVC | No | Yes |
| Blazor Server | No | Yes |
| Blazor WebAssembly | No | Yes |
| WPF Desktop | No | Yes |
| WinForms Desktop | No | Yes |
| Azure Functions | No | Yes |
| AWS Lambda | No | Yes |
| Docker Container | No | Yes |
| Windows Service | No | Yes |
| Linux Server | No | Yes |
| .NET MAUI | Yes | Yes |

This means Scanbot cannot be used for:
- Server-side barcode processing
- Document batch processing workflows
- Desktop application barcode features
- Serverless function barcode handling
- Any non-MAUI .NET project

### Project Type Requirements

To use Scanbot SDK for .NET, you must:

1. Create a .NET MAUI Application project
2. Target iOS and/or Android platforms
3. Include camera permissions in app manifests
4. Implement camera preview UI components
5. Handle platform-specific initialization

If your project is not a mobile MAUI app, Scanbot SDK is not an option.

---

## Capabilities Comparison

### Feature Matrix

| Feature | Scanbot SDK | IronBarcode |
|---------|-------------|-------------|
| **Camera Scanning** | Yes (native UI) | N/A (programmatic) |
| **File/Image Processing** | Limited | Primary focus |
| **PDF Barcode Extraction** | No | Yes (native) |
| **Server Batch Processing** | No (mobile only) | Yes |
| **MAUI Mobile** | Yes | Yes |
| **ASP.NET Core** | No | Yes |
| **Console Apps** | No | Yes |
| **WPF/WinForms** | No | Yes |
| **Docker/Linux** | No | Yes |
| **Offline Processing** | Yes | Yes |
| **1D Formats** | 20+ | 30+ |
| **2D Formats** | QR, DataMatrix, PDF417, Aztec | QR, DataMatrix, PDF417, Aztec, MaxiCode |
| **ML Error Correction** | No | Yes |
| **Automatic Format Detection** | Yes | Yes |

### What Scanbot Does Well

**Camera Integration:**
Scanbot provides polished camera UI components:
- Pre-built viewfinder with scan region
- Real-time barcode highlighting
- Haptic and audio feedback
- Flash/torch control
- Front/back camera switching

**Offline Privacy:**
Scanbot emphasizes offline processing:
- No data sent to external servers
- Compliant with strict privacy requirements
- Suitable for healthcare and finance apps
- No internet permission needed

**Document Scanning Extras:**
Beyond barcodes, Scanbot offers:
- Document edge detection
- Perspective correction
- Image filters and enhancement
- PDF creation from scanned images
- OCR for text extraction

### What IronBarcode Does Well

**Universal .NET Compatibility:**
IronBarcode works in any .NET project:
- No platform restrictions
- Single package for all scenarios
- Same API everywhere

**Document Processing:**
IronBarcode handles document workflows:
- PDF barcode extraction
- Batch image processing
- Server-side automation
- High-volume processing

**Advanced Recognition:**
IronBarcode includes:
- ML-powered error correction
- Damaged barcode recovery
- Low-quality image handling
- Multi-barcode document processing

---

## Camera Focus vs Programmatic Processing

### Scanbot's Camera-Centric Design

Scanbot assumes your workflow is:

1. User opens mobile app
2. User points camera at barcode
3. Camera viewfinder displays preview
4. Barcode detected in video frames
5. Result returned via callback
6. User sees visual confirmation

This is excellent for mobile point-of-scan scenarios.

### IronBarcode's File-Centric Design

IronBarcode assumes your workflow is:

1. Barcode exists in file (image, PDF, document)
2. Code loads file
3. Library processes content
4. Results returned as data
5. Code uses barcode values

This is excellent for document processing and server scenarios.

### The Fundamental Difference

```
Scanbot: Camera feed → Process frames → Return results
IronBarcode: File input → Process content → Return results
```

Neither approach is "better" - they solve different problems.

---

## Code Comparison

### Camera Scanning (Scanbot's Strength)

**Scanbot MAUI Implementation:**

```csharp
// Requires: dotnet add package ScanbotBarcodeSDK.MAUI
// Requires: .NET MAUI mobile app project
// Requires: Camera permissions in app manifests

using ScanbotBarcodeSDK.MAUI;

namespace MyMauiApp;

public partial class ScanPage : ContentPage
{
    public ScanPage()
    {
        InitializeComponent();
        SetupScanner();
    }

    private void SetupScanner()
    {
        // Initialize Scanbot with license
        ScanbotSDK.Initialize(new ScanbotSDKConfiguration
        {
            LicenseKey = "YOUR-SCANBOT-LICENSE-KEY",
            EnableLogging = false
        });
    }

    private async void OnScanButtonClicked(object sender, EventArgs e)
    {
        // Configure scanner
        var configuration = new BarcodeScannerConfiguration
        {
            AcceptedFormats = new[]
            {
                BarcodeFormat.Code128,
                BarcodeFormat.QrCode,
                BarcodeFormat.Ean13
            },
            FinderAspectRatio = new AspectRatio(1, 1),
            TopBarBackgroundColor = Colors.Blue
        };

        // Launch camera scanner
        var result = await ScanbotBarcodeSDK.BarcodeScanner
            .Open(configuration);

        if (result.Status == OperationResult.Ok)
        {
            foreach (var barcode in result.Barcodes)
            {
                await DisplayAlert("Scanned",
                    $"{barcode.Format}: {barcode.Text}",
                    "OK");
            }
        }
    }
}
```

This code:
- Only works in MAUI mobile project
- Requires camera permissions
- Opens camera UI for scanning
- Returns results after user scans

### File Processing (IronBarcode's Strength)

**IronBarcode Implementation:**

```csharp
// Install: dotnet add package IronBarcode
// Works in: Console, ASP.NET, WPF, WinForms, MAUI, Azure Functions, etc.

using IronBarCode;

// Set license once at startup
IronBarCode.License.LicenseKey = "YOUR-KEY";

// Read from image file
var imageResults = BarcodeReader.Read("barcode.png");
foreach (var barcode in imageResults)
{
    Console.WriteLine($"{barcode.BarcodeType}: {barcode.Text}");
}

// Read from PDF document
var pdfResults = BarcodeReader.Read("invoice.pdf");
foreach (var barcode in pdfResults)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
}

// Batch process directory
foreach (var file in Directory.GetFiles("./documents/", "*.pdf"))
{
    var results = BarcodeReader.Read(file);
    ProcessResults(file, results);
}
```

This code:
- Works in any .NET project type
- No camera or UI requirements
- Processes files programmatically
- Suitable for server automation

For detailed MAUI-specific comparisons, see [Scanbot MAUI-Only Example](scanbot-maui-only.cs).

---

## When to Use Each

### Choose Scanbot SDK When:

1. **Building mobile-only MAUI app** - If your entire project is a mobile application using .NET MAUI for iOS and Android.

2. **Needing camera scanning UI** - If users will actively scan physical barcodes with their device camera.

3. **Requiring offline mobile processing** - If privacy requirements prohibit any server communication and all processing must happen on-device.

4. **Document scanning beyond barcodes** - If you also need document edge detection, OCR, or image enhancement in the same mobile app.

5. **Predictable mobile licensing** - If the yearly flat fee model fits your budget better than alternatives.

### Choose IronBarcode When:

1. **Building server-side processing** - ASP.NET Core APIs, Azure Functions, batch processing systems.

2. **Building desktop applications** - WPF, WinForms, Avalonia desktop apps with barcode features.

3. **Building console utilities** - Command-line tools for barcode processing.

4. **Processing existing files** - PDFs, images, or documents that already contain barcodes.

5. **Needing cross-project compatibility** - Same barcode library across web, desktop, and mobile projects.

6. **Requiring PDF barcode extraction** - Native support for extracting barcodes from multi-page PDFs.

7. **Handling damaged barcodes** - ML-powered error correction for low-quality images.

### Hybrid Approach Possibility

For organizations with both mobile scanning and server processing needs:

- **Scanbot** for mobile app camera scanning UI
- **IronBarcode** for server-side document processing

Both tools handle their optimal use case well.

---

## IronBarcode on Mobile

An important clarification about IronBarcode's mobile capabilities.

### What IronBarcode CAN Do on Mobile

IronBarcode works in MAUI (and Avalonia, Xamarin) for programmatic scenarios:

```csharp
// MAUI app - Process image from camera roll
public async Task ProcessGalleryImage()
{
    // User selects image from gallery
    var photo = await MediaPicker.PickPhotoAsync();

    if (photo != null)
    {
        using var stream = await photo.OpenReadAsync();
        var results = BarcodeReader.Read(stream);

        foreach (var barcode in results)
        {
            await DisplayAlert("Found",
                $"{barcode.BarcodeType}: {barcode.Text}",
                "OK");
        }
    }
}

// MAUI app - Process PDF attachment
public void ProcessPdfAttachment(Stream pdfStream)
{
    var results = BarcodeReader.Read(pdfStream);
    foreach (var barcode in results)
    {
        Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
    }
}

// MAUI app - Generate QR for display
public byte[] GenerateQrCode(string data)
{
    var qr = BarcodeWriter.CreateBarcode(data,
        BarcodeEncoding.QRCode);
    return qr.ToPngBinaryData();
}
```

### What IronBarcode Does NOT Do on Mobile

IronBarcode does not provide:
- Camera preview UI components
- Real-time video frame processing
- Viewfinder with scan region
- Camera permission handling
- Flash/torch control

For live camera scanning, use Scanbot, BarcodeScanning.MAUI, or similar camera-focused libraries.

### Mobile Use Case Summary

| Mobile Scenario | Scanbot | IronBarcode |
|-----------------|---------|-------------|
| Camera viewfinder scanning | Yes | No |
| Process image from gallery | Limited | Yes |
| Process PDF attachment | No | Yes |
| Generate barcode for display | No | Yes |
| Real-time video scanning | Yes | No |
| Server upload after scan | Via API | N/A |

---

## Migration Guide

### Understanding the Migration Context

Migrating from Scanbot to IronBarcode makes sense when your actual need is file processing, not camera scanning.

**When Migration IS Appropriate:**
- Your "mobile app" is actually just processing files
- You need server-side barcode processing
- You're building desktop applications
- Your camera scanning needs are minimal

**When Migration is NOT Appropriate:**
- You genuinely need camera UI for scanning
- Offline mobile scanning is a core requirement
- Document scanning (beyond barcodes) is needed

### Package Migration

**Remove Scanbot:**

```bash
dotnet remove package ScanbotBarcodeSDK.MAUI
```

**Add IronBarcode:**

```bash
dotnet add package IronBarcode
```

### License Migration

**Remove Scanbot initialization:**

```csharp
// Remove
ScanbotSDK.Initialize(new ScanbotSDKConfiguration
{
    LicenseKey = "YOUR-SCANBOT-KEY"
});
```

**Add IronBarcode license:**

```csharp
// Add at startup
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

### Workflow Transformation

The key migration challenge is transforming from camera-based to file-based workflow.

**Scanbot Camera Workflow:**
```csharp
// User points camera, UI handles scanning
var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(config);
var barcodes = result.Barcodes;
```

**IronBarcode File Workflow:**
```csharp
// If you have an image file
var results = BarcodeReader.Read("captured-image.png");

// If processing from stream
var results = BarcodeReader.Read(imageStream);

// If processing PDF
var results = BarcodeReader.Read("document.pdf");
```

**Hybrid Approach for MAUI:**

If you still need camera capture, use platform camera APIs to capture image, then process with IronBarcode:

```csharp
public async Task CaptureAndProcess()
{
    // Use MAUI's MediaPicker to capture photo
    var photo = await MediaPicker.CapturePhotoAsync();

    if (photo != null)
    {
        // Save to file
        var path = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        using var sourceStream = await photo.OpenReadAsync();
        using var destStream = File.OpenWrite(path);
        await sourceStream.CopyToAsync(destStream);

        // Process with IronBarcode
        var results = BarcodeReader.Read(path);
        HandleResults(results);
    }
}
```

This gives you camera capture without Scanbot's UI, then IronBarcode's processing.

### Feature Mapping

| Scanbot Feature | IronBarcode Equivalent |
|-----------------|----------------------|
| `BarcodeScannerConfiguration` | `BarcodeReaderOptions` |
| `AcceptedFormats` | Automatic detection |
| `result.Barcodes` | `BarcodeReader.Read()` results |
| `barcode.Format` | `result.BarcodeType` |
| `barcode.Text` | `result.Text` |
| Camera UI | Use MediaPicker + IronBarcode |

### Migration Checklist

**Pre-Migration:**
- [ ] Confirm use case is file processing (not camera UI)
- [ ] Obtain IronBarcode license
- [ ] Identify all Scanbot usage points
- [ ] Plan camera capture alternative if needed

**Migration:**
- [ ] Remove ScanbotBarcodeSDK.MAUI package
- [ ] Add IronBarcode package
- [ ] Update license initialization
- [ ] Replace camera workflow with file workflow
- [ ] Update result handling code

**Post-Migration:**
- [ ] Test all barcode processing scenarios
- [ ] Verify PDF processing works
- [ ] Test on target platforms
- [ ] Remove Scanbot license configuration

For camera workflow examples, see [Scanbot Camera Focus Example](scanbot-camera-focus.cs).

---

## Additional Resources

### Documentation Links

- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/) - Official guides and API reference
- [IronBarcode on NuGet](https://www.nuget.org/packages/BarCode) - Package download
- [Scanbot SDK Documentation](https://docs.scanbot.io/) - Official Scanbot guides
- [ScanbotBarcodeSDK.MAUI on NuGet](https://www.nuget.org/packages/ScanbotBarcodeSDK.MAUI) - Scanbot package

### Code Example Files

Working code demonstrating the concepts in this article:

- [MAUI-Only Limitation Examples](scanbot-maui-only.cs) - Platform restriction demonstrations
- [Camera Focus Comparison](scanbot-camera-focus.cs) - Camera vs file processing workflows

### Related Comparisons

- [Scandit SDK Comparison](../scandit-sdk/) - Enterprise mobile scanning platform
- [barKoder SDK Comparison](../barkoder-sdk/) - Mobile SDK with no .NET support
- [BarcodeScanning.MAUI Comparison](../barcodescanning-maui/) - Open-source MAUI camera library

---

*Last verified: January 2026*
*Author: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*
