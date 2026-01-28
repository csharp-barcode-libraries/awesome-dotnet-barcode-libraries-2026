# ZXing.Net.MAUI vs IronBarcode: C# MAUI Barcode Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

ZXing.Net.MAUI is a .NET MAUI wrapper that brings ZXing barcode scanning to MAUI mobile applications with camera support. If you've used ZXing.Net in previous projects and are migrating to MAUI, ZXing.Net.MAUI seems like the natural choice. However, this library inherits all the core limitations of ZXing.Net while adding MAUI-specific issues.

Before integrating ZXing.Net.MAUI, understand both the inherited ZXing.Net limitations (detailed in our [comprehensive ZXing.Net comparison](../zxing-net/README.md)) and the MAUI-specific issues covered in this guide. [IronBarcode](https://ironsoftware.com/csharp/barcode/) provides a universal .NET solution that works across all MAUI platforms including Windows, without the manual format specification and platform-specific bugs documented here.

## Table of Contents

1. [What is ZXing.Net.MAUI?](#what-is-zxingnetmaui)
2. [Relationship to ZXing.Net](#relationship-to-zxingnet)
3. [Known Issues from GitHub](#known-issues-from-github)
4. [Capabilities Comparison](#capabilities-comparison)
5. [Inherited ZXing.Net Core Issues](#inherited-zxingnet-core-issues)
6. [MAUI-Specific Issues](#maui-specific-issues)
7. [Code Comparison](#code-comparison)
8. [When to Use Each Library](#when-to-use-each-library)
9. [Migration Guide](#migration-guide)
10. [References](#references)

---

## What is ZXing.Net.MAUI?

ZXing.Net.MAUI is a community-maintained .NET MAUI library that wraps ZXing.Net's barcode engine with MAUI camera controls:

| Aspect | Details |
|--------|---------|
| **NuGet Package** | ZXing.Net.Maui.Controls |
| **Current Version** | 0.5.0 (pre-release) |
| **GitHub Repository** | Redth/ZXing.Net.Maui |
| **Maintainer** | Jon Dick (Redth) - Community |
| **License** | MIT (Free) |
| **Core Engine** | ZXing.Net (Apache 2.0) |
| **Platforms** | iOS, Android (Windows NOT supported) |

### How It Works

ZXing.Net.MAUI provides:
- `CameraBarcodeReaderView`: XAML control for camera-based scanning
- `BarcodeReader`: ZXing.Net's decoding engine
- `BarcodeWriter`: ZXing.Net's generation engine

The camera view component uses platform camera APIs to capture frames, which are then processed by ZXing.Net's decoding engine. This architecture means you get ZXing's barcode detection with MAUI camera integration.

### Pre-Release Status Warning

ZXing.Net.MAUI is currently at version 0.5.0 - a pre-release version number. This indicates:
- API may change between versions
- Stability is not guaranteed for production
- Bug fixes may be slow
- Community-maintained, not commercially supported

---

## Relationship to ZXing.Net

**ZXing.Net.MAUI is a MAUI wrapper around ZXing.Net.** This means it inherits all the core ZXing.Net limitations documented in our [complete ZXing.Net guide](../zxing-net/README.md).

### Inherited Limitations

| ZXing.Net Core Issue | Impact on ZXing.Net.MAUI |
|---------------------|-------------------------|
| **Manual format specification required** | Must set BarcodeFormats in camera view |
| **No automatic format detection** | Can't scan unknown barcodes reliably |
| **No PDF support** | Cannot process PDF documents |
| **Limited error correction** | Poor performance on damaged barcodes |
| **Thread safety concerns** | Potential issues in parallel processing |

### Plus MAUI-Specific Issues

| MAUI-Specific Issue | Details |
|--------------------|---------|
| **Windows NOT supported** | No implementation for Windows MAUI |
| **iPhone 15 auto-focus bugs** | Camera focus issues on newer devices |
| **Camera resource leaks** | Memory not released properly |
| **Android Camera 1.5.0 build errors** | Compatibility issues with camera libraries |
| **Pre-release stability** | v0.5.0 not production-ready |

---

## Known Issues from GitHub

These issues are actively tracked in the ZXing.Net.MAUI GitHub repository:

### Issue 1: iPhone 15 PRO Auto-Focus Problems

Users report that iPhone 15 PRO models have difficulty focusing on barcodes. The camera view shows the barcode but fails to focus sharply enough for detection.

**From GitHub:** "On iPhone 15 PRO, barcodes that scan fine on iPhone 12 won't scan at all. Manual distance adjustment (moving phone closer/farther) sometimes helps."

**Workaround:** Manually adjust device distance from barcode until focus locks. No programmatic fix available.

### Issue 2: Windows Support Not Implemented

The GitHub repository explicitly states Windows MAUI is not supported. There is no camera implementation for Windows, and no roadmap for adding it.

**Impact:** If you need a cross-platform MAUI app including Windows, ZXing.Net.MAUI cannot be used alone.

### Issue 3: Camera Not Released Properly (Memory Leaks)

When navigating away from scanning pages, the camera resources may not be released properly, causing:
- Memory leaks over time
- Camera remaining locked
- Battery drain from background camera activity
- Potential crashes on return to scanning

**Workaround:** Manually dispose camera view on page disappearing and reinitialize on appearing.

### Issue 4: Android Camera 1.5.0 Build Errors

Compatibility issues with Android's Camera 1.5.0 library cause build failures in some configurations. The error manifests as missing method exceptions or type load failures.

**Workaround:** Pin to earlier camera library versions or await library updates.

---

## Capabilities Comparison

| Feature | ZXing.Net.MAUI | IronBarcode |
|---------|---------------|-------------|
| **MAUI Mobile** | Yes (iOS, Android) | Yes (all MAUI targets) |
| **MAUI Windows** | No | Yes |
| **MAUI macOS** | No | Yes |
| **Auto-Detection** | No (inherited from ZXing.Net) | Yes |
| **PDF Support** | No (inherited from ZXing.Net) | Yes |
| **Camera Input** | Yes (primary purpose) | No (image-based) |
| **Image Input** | Via ZXing.Net core | Yes |
| **Damaged Barcode Handling** | Limited | ML-powered |
| **Production Status** | Pre-release (v0.5.0) | Production-ready |
| **Commercial Support** | No (community) | Yes |
| **License** | MIT (Free) | Commercial |

### Architectural Comparison

**ZXing.Net.MAUI Architecture:**
```
Camera Frame → ZXing.Net Decoder → Result
                    ↓
         (Requires format hints)
```

**IronBarcode Architecture:**
```
Image/PDF/Stream → IronBarcode Engine → Result
                         ↓
               (Automatic detection)
```

---

## Inherited ZXing.Net Core Issues

For complete details on ZXing.Net limitations, see our [ZXing.Net vs IronBarcode comparison](../zxing-net/README.md). Here's a summary of key issues that carry over to ZXing.Net.MAUI:

### Format Specification Required

ZXing.Net (and by extension ZXing.Net.MAUI) requires you to specify which barcode formats to scan for:

```csharp
// ZXing.Net.MAUI - Must specify formats
<zxing:CameraBarcodeReaderView
    Options="{Binding BarcodeOptions}"
    BarcodesDetected="OnBarcodesDetected" />

// In view model or code-behind
BarcodeOptions = new BarcodeReaderOptions
{
    Formats = BarcodeFormats.Code128 | BarcodeFormats.QRCode | BarcodeFormats.Ean13
    // Miss a format? It won't be detected even if in frame
};
```

If a barcode format isn't in your list, it won't be detected - even if perfectly visible to the camera.

### No PDF Support

ZXing.Net has no native PDF processing. ZXing.Net.MAUI inherits this limitation. You cannot scan barcodes from PDF documents.

### Limited Error Correction

ZXing.Net's decoding engine has limited tolerance for damaged, faded, or partially obscured barcodes. This limitation carries directly into ZXing.Net.MAUI's camera scanning.

---

## MAUI-Specific Issues

Beyond the inherited ZXing.Net issues, ZXing.Net.MAUI adds these MAUI-specific problems:

### Auto-Focus Issues on Modern iPhones

```csharp
// iPhone 15 PRO auto-focus workaround attempts

// ISSUE: Camera view shows barcode but won't focus
// Documented in GitHub issues

// Attempted workaround 1: Torch toggle sometimes helps focus
private void ToggleTorchForFocus()
{
    // Turn torch on briefly, then off
    // Sometimes triggers focus system
    _cameraView.IsTorchOn = true;
    await Task.Delay(200);
    _cameraView.IsTorchOn = false;
}

// Attempted workaround 2: User instruction
// "Move phone closer or farther until barcode scans"
// Not ideal for production UX
```

### Memory/Camera Lifecycle Management

```csharp
// ISSUE: Camera resources not released properly

public partial class ScannerPage : ContentPage
{
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // MUST manually stop camera to prevent leaks
        if (cameraBarcodeReaderView != null)
        {
            cameraBarcodeReaderView.IsDetecting = false;
            // Note: No formal Dispose() method available
            // Camera may remain locked
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Reinitialize camera on return
        if (cameraBarcodeReaderView != null)
        {
            cameraBarcodeReaderView.IsDetecting = true;
        }
    }
}
```

### Pre-Release Version Concerns

At v0.5.0, ZXing.Net.MAUI is not considered production-ready:

- API breaking changes may occur
- Bug fixes depend on community availability
- No SLA for issue resolution
- Limited documentation
- Testing coverage may be incomplete

---

## Code Comparison

### ZXing.Net.MAUI: Camera Scanning with Format Hints

See complete example: [zxing-maui-limitations.cs](zxing-maui-limitations.cs)

```csharp
// ZXing.Net.MAUI - Camera view with format specification required
// XAML:
// <zxing:CameraBarcodeReaderView x:Name="CameraView"
//     Options="{Binding ReaderOptions}"
//     BarcodesDetected="OnBarcodesDetected" />

public partial class ZXingScannerPage : ContentPage
{
    public BarcodeReaderOptions ReaderOptions { get; }

    public ZXingScannerPage()
    {
        InitializeComponent();

        // MUST specify formats - inherited ZXing.Net requirement
        ReaderOptions = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.QRCode |
                     BarcodeFormats.Code128 |
                     BarcodeFormats.Ean13 |
                     BarcodeFormats.UpcA,

            // TryHarder attempts more detection passes
            TryHarder = true,

            // AutoRotate helps with angled barcodes
            AutoRotate = true
        };

        BindingContext = this;
    }

    private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        // Results only include formats from your Options list
        // Other formats are ignored even if visible

        foreach (var barcode in e.Results)
        {
            Console.WriteLine($"Format: {barcode.Format}");
            Console.WriteLine($"Value: {barcode.Value}");
        }
    }
}
```

### IronBarcode: Universal Approach with Automatic Detection

```csharp
// IronBarcode - Process captured images with automatic detection
using IronBarCode;

public partial class IronBarcodePage : ContentPage
{
    private async void OnCaptureClicked(object sender, EventArgs e)
    {
        // Step 1: Capture with MAUI Essentials
        var photo = await MediaPicker.CapturePhotoAsync();

        if (photo != null)
        {
            // Step 2: Get image bytes
            using var stream = await photo.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();

            // Step 3: Process with automatic format detection
            var results = BarcodeReader.Read(bytes);

            // All 50+ formats detected automatically
            // No format specification required
            foreach (var barcode in results)
            {
                Console.WriteLine($"Type: {barcode.BarcodeType}");
                Console.WriteLine($"Value: {barcode.Text}");
            }
        }
    }
}
```

---

## When to Use Each Library

### Choose ZXing.Net.MAUI When:

| Scenario | Reasoning |
|----------|-----------|
| **Zero budget** | MIT license, completely free |
| **Familiar with ZXing.Net** | Same core engine, familiar APIs |
| **iOS/Android only** | Windows not needed |
| **Known barcode formats** | You control what formats appear |
| **Prototype or POC** | Pre-release acceptable for testing |
| **Accept camera bugs** | iPhone 15 issues won't affect your users |

### Choose IronBarcode When:

| Scenario | Reasoning |
|----------|-----------|
| **Windows MAUI required** | Only IronBarcode works on Windows MAUI |
| **Unknown barcode formats** | Automatic detection eliminates guesswork |
| **Production application** | Production-ready with commercial support |
| **PDF processing needed** | Native PDF barcode extraction |
| **Cross-platform consistency** | Same code works everywhere |
| **Modern device support** | No iPhone 15 focus issues |
| **Reliability requirements** | ML-powered error correction |
| **SLA/support needed** | Commercial support available |

### Decision Matrix

| Your Situation | Recommendation |
|---------------|----------------|
| Mobile-only prototype, zero budget | ZXing.Net.MAUI |
| Production mobile app, reliability matters | IronBarcode |
| Cross-platform including Windows | IronBarcode (only option) |
| Processing PDFs | IronBarcode (only option) |
| Unknown barcode formats in documents | IronBarcode |
| Team familiar with ZXing, simple use case | ZXing.Net.MAUI |
| Need commercial support | IronBarcode |

---

## Migration Guide

If you've started with ZXing.Net.MAUI and need to migrate to IronBarcode, here's the process.

### Why Migrate from ZXing.Net.MAUI?

| Pain Point | Root Cause | IronBarcode Solution |
|------------|------------|---------------------|
| Windows MAUI doesn't work | Not supported | Full Windows support |
| Must specify formats | ZXing.Net inheritance | Automatic detection |
| iPhone 15 focus issues | MAUI camera bugs | Image capture workflow |
| Memory leaks | Camera lifecycle bugs | No camera dependency |
| Pre-release instability | v0.5.0 status | Production-ready |
| PDF processing needed | Not supported | Native PDF support |

### Migration Steps

#### Step 1: Change Architecture

ZXing.Net.MAUI uses real-time camera detection. IronBarcode uses image processing. Change from "scan in frame" to "capture and process."

**Before (ZXing.Net.MAUI):**
```xml
<zxing:CameraBarcodeReaderView x:Name="CameraView"
    Options="{Binding ReaderOptions}"
    BarcodesDetected="OnBarcodesDetected" />
```

**After (IronBarcode):**
```xml
<Button Text="Scan" Clicked="OnCaptureClicked" />
<Image x:Name="Preview" />
<Label x:Name="ResultLabel" />
```

#### Step 2: Update NuGet Packages

```xml
<!-- Remove -->
<PackageReference Include="ZXing.Net.Maui.Controls" Version="*" />

<!-- Add -->
<PackageReference Include="IronBarcode" Version="2024.*" />
```

#### Step 3: Update Code

**Before:**
```csharp
// ZXing.Net.MAUI - Format specification required
private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
{
    foreach (var result in e.Results)
    {
        ProcessBarcode(result.Value);
    }
}
```

**After:**
```csharp
// IronBarcode - Automatic detection
private async void OnCaptureClicked(object sender, EventArgs e)
{
    var photo = await MediaPicker.CapturePhotoAsync();

    if (photo != null)
    {
        using var stream = await photo.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        var results = BarcodeReader.Read(ms.ToArray());

        foreach (var barcode in results)
        {
            ProcessBarcode(barcode.Text);
        }
    }
}
```

#### Step 4: Remove Format Specification Code

```csharp
// DELETE this code - no longer needed
ReaderOptions = new BarcodeReaderOptions
{
    Formats = BarcodeFormats.QRCode | BarcodeFormats.Code128,
    TryHarder = true
};

// IronBarcode detects all formats automatically
```

### API Mapping Reference

| ZXing.Net.MAUI | IronBarcode | Notes |
|---------------|-------------|-------|
| `CameraBarcodeReaderView` | N/A - use MediaPicker | Different architecture |
| `BarcodeReaderOptions.Formats` | Not needed | Automatic detection |
| `BarcodeDetectionEventArgs.Results` | `BarcodeReader.Read()` return | Method vs event |
| `BarcodeResult.Value` | `BarcodeResult.Text` | Same data |
| `BarcodeResult.Format` | `BarcodeResult.BarcodeType` | Renamed |
| Camera view lifecycle | N/A | No camera management needed |

### Benefits After Migration

1. **Windows MAUI works** - Deploy to Windows desktop
2. **No format specification** - Automatic detection
3. **No camera bugs** - Image capture is reliable
4. **No memory leaks** - No camera lifecycle issues
5. **PDF processing** - Native document support
6. **Production-ready** - Stable, supported library

---

## Code Examples

- [ZXing.Net.MAUI Limitations](zxing-maui-limitations.cs) - Format specification, inherited issues
- [Camera Issues and Workarounds](zxing-maui-camera-issues.cs) - iPhone 15 focus, memory leaks

---

## References

- [ZXing.Net.MAUI GitHub](https://github.com/Redth/ZXing.Net.Maui) *(nofollow)*
- [ZXing.Net.MAUI NuGet](https://www.nuget.org/packages/ZXing.Net.Maui.Controls) *(nofollow)*
- [ZXing.Net Core Comparison](../zxing-net/README.md) - Inherited limitations detailed
- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/)
- [IronBarcode MAUI Tutorial](https://ironsoftware.com/csharp/barcode/tutorials/maui-barcode-scanner/)

---

*Last verified: January 2026*
