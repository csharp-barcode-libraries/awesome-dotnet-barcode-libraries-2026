# Migrating from Dynamsoft Barcode Reader to IronBarcode

Most developers who migrate from Dynamsoft to IronBarcode fall into one of two groups: those who chose Dynamsoft for its reputation and then discovered the camera-centric API didn't match a document processing use case, and those running in air-gapped or Docker environments where the license server dependency caused production incidents.

If you are in the first group, the migration removes the external PDF rendering library, the per-page render loop, and the error-code license pattern. If you are in the second group, the migration removes the `InitLicense` network call, the `OutputLicenseToString` UUID management, and the outbound network policy from your Docker or VPC configuration. Either way, the codebase gets shorter after this migration.

This guide is honest about what you lose: if your application processes real-time camera frames at 30fps, Dynamsoft's algorithms are tuned for that workload and IronBarcode is not the right replacement. This migration guide is for server-side file processing, document workflows, and environments where license server access is a problem.

## Step 1: Swap the NuGet Packages

```bash
dotnet remove package Dynamsoft.DotNet.BarcodeReader.Bundle
dotnet add package BarCode
```

The IronBarcode NuGet package id is `BarCode` even though the namespace is `IronBarCode`. If your project also has a PDF rendering library added specifically for Dynamsoft (PdfiumViewer is the most common), that can be removed too:

```bash
# Remove if added only for Dynamsoft PDF support
dotnet remove package PdfiumViewer
dotnet remove package PdfiumViewer.Native.x86_64.v8-xfa
```

## Step 2: Replace License Initialization

This is where the most immediate simplification happens. The Dynamsoft pattern requires an error code check and exception handling around every startup:

**Before — Dynamsoft:**

```csharp
using Dynamsoft.License;
using Dynamsoft.Core;

// Must run before any barcode operations — performs online activation
// against Dynamsoft's license servers and re-validates periodically.
int errorCode = LicenseManager.InitLicense("YOUR-LICENSE-KEY", out string errorMsg);
if (errorCode != (int)EnumErrorCode.EC_OK)
    throw new InvalidOperationException($"License validation failed [{errorCode}]: {errorMsg}");
```

**After — IronBarcode:**

```csharp
// NuGet: dotnet add package BarCode
using IronBarCode;

// Local validation — no network call, no error code
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

In an ASP.NET Core application, add this to `Program.cs` before `builder.Build()`:

```csharp
IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_KEY")
    ?? "YOUR-KEY";
```

In a Docker or Kubernetes environment, set the `IRONBARCODE_KEY` environment variable in your deployment manifest. No outbound network rules required.

## Step 3: Replace Namespace Imports

Find and replace across all source files:

```bash
grep -r "using Dynamsoft\." --include="*.cs" .
```

Replace each occurrence:

```csharp
// Before
using Dynamsoft.CVR;
using Dynamsoft.DBR;
using Dynamsoft.License;
using Dynamsoft.Core;

// After
using IronBarCode;
```

## Code Migration Examples

### Basic File Reading

The most fundamental operation — reading a barcode from an image file.

**Before — Dynamsoft:**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;

public string ReadBarcodeFromFile(CaptureVisionRouter router, string imagePath)
{
    CapturedResult result = router.Capture(imagePath, PresetTemplate.PT_READ_BARCODES);
    var items = result.GetDecodedBarcodesResult()?.GetItems();
    if (items == null || items.Length == 0)
        return null;

    return items[0].GetText();
}
```

**After — IronBarcode:**

```csharp
// NuGet: dotnet add package BarCode
using IronBarCode;

public string ReadBarcodeFromFile(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results?.FirstOrDefault()?.Value;
}
```

The `CaptureVisionRouter` instance is gone. `BarcodeReader.Read` is static. `BarcodeResultItem.GetText()` becomes `.Value`. The `null` check is cleaner with LINQ.

### Reading Multiple Barcodes

**Before — Dynamsoft:**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;

public List<string> ReadAllBarcodes(CaptureVisionRouter router, string imagePath)
{
    SimplifiedCaptureVisionSettings settings = router.GetSimplifiedSettings(
        PresetTemplate.PT_READ_BARCODES);
    settings.BarcodeSettings.ExpectedBarcodesCount = 0; // 0 = find all
    router.UpdateSettings(PresetTemplate.PT_READ_BARCODES, settings);

    CapturedResult result = router.Capture(imagePath, PresetTemplate.PT_READ_BARCODES);
    var values = new List<string>();

    foreach (var item in result.GetDecodedBarcodesResult().GetItems())
        values.Add(item.GetText());

    return values;
}
```

**After — IronBarcode:**

```csharp
using IronBarCode;

public List<string> ReadAllBarcodes(string imagePath)
{
    var options = new BarcodeReaderOptions
    {
        ExpectMultipleBarcodes = true,
        MaxParallelThreads = 4
    };

    return BarcodeReader.Read(imagePath, options)
        .Select(r => r.Value)
        .ToList();
}
```

### Reading from Bytes (In-Memory Images)

**Before — Dynamsoft:**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.Core;
using Dynamsoft.DBR;

// Requires width, height, stride, and pixel format — low-level buffer API
public string ReadFromBuffer(CaptureVisionRouter router, byte[] rawPixels, int width, int height)
{
    int stride = width * 3; // assuming 24bpp RGB
    var imageData = new ImageData
    {
        Bytes = rawPixels,
        Width = width,
        Height = height,
        Stride = stride,
        Format = EnumImagePixelFormat.IPF_RGB_888
    };

    CapturedResult result = router.Capture(imageData, PresetTemplate.PT_READ_BARCODES);
    return result.GetDecodedBarcodesResult()?.GetItems()?.FirstOrDefault()?.GetText();
}
```

**After — IronBarcode:**

```csharp
using IronBarCode;

// Pass PNG/JPEG/BMP bytes directly — no pixel format or stride calculation
public string ReadFromImageBytes(byte[] imageBytes)
{
    return BarcodeReader.Read(imageBytes)?.FirstOrDefault()?.Value;
}
```

If your application previously converted image bytes into a raw pixel buffer for Dynamsoft, you can pass the original encoded image bytes (PNG, JPEG, BMP) directly to IronBarcode without decoding to raw pixels first.

### PDF Barcode Reading — Remove the Render Loop

This is typically the largest code reduction in the migration. Remove the entire PdfiumViewer render loop and replace it with a single call.

**Before — Dynamsoft with PdfiumViewer:**

```csharp
// Requires: Dynamsoft.DotNet.BarcodeReader.Bundle + PdfiumViewer + PdfiumViewer.Native.*
using Dynamsoft.CVR;
using Dynamsoft.DBR;
using PdfiumViewer;
using System.Drawing.Imaging;

public List<string> ReadBarcodesFromPdf(CaptureVisionRouter router, string pdfPath)
{
    var allBarcodes = new List<string>();

    using (var pdfDoc = PdfDocument.Load(pdfPath))
    {
        for (int page = 0; page < pdfDoc.PageCount; page++)
        {
            using var image = pdfDoc.Render(page, 300, 300, true);
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);

            CapturedResult result = router.Capture(ms.ToArray(),
                PresetTemplate.PT_READ_BARCODES);
            foreach (var item in result.GetDecodedBarcodesResult().GetItems())
                allBarcodes.Add(item.GetText());
        }
    }

    return allBarcodes;
}
```

**After — IronBarcode:**

```csharp
using IronBarCode;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    return BarcodeReader.Read(pdfPath)
        .Select(r => r.Value)
        .ToList();
}
```

The page loop, the PdfDocument, the image rendering at 300 DPI, the MemoryStream, and the `Capture` call per page all disappear. IronBarcode handles PDF pages internally.

If you need to read from a PDF with options (for dense or difficult barcodes):

```csharp
using IronBarCode;

public List<string> ReadBarcodesFromPdfAccurate(string pdfPath)
{
    var options = new BarcodeReaderOptions
    {
        Speed = ReadingSpeed.Balanced,
        ExpectMultipleBarcodes = true
    };

    return BarcodeReader.Read(pdfPath, options)
        .Select(r => r.Value)
        .ToList();
}
```

### Offline / Air-Gapped Deployment

If your current code includes the offline licensing pattern, remove it entirely:

**Before — Dynamsoft offline license:**

```csharp
// Dynamsoft offline: request a device-bound license file from support,
// then activate it through the LicenseManager APIs documented for your
// SDK version (the exact method names have moved across versions).
using Dynamsoft.License;
using Dynamsoft.Core;

int errorCode = LicenseManager.InitLicenseFromDevice(
    "YOUR-LICENSE-KEY",
    licenseContent,
    out string errorMsg);

if (errorCode != (int)EnumErrorCode.EC_OK)
    throw new InvalidOperationException($"Offline license failed: {errorMsg}");
```

**After — IronBarcode:**

```csharp
// Remove all of the above. Replace with:
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

No device-bound license file. No support email round-trip. The key validates locally.

### Docker Configuration

If you previously had network egress rules or proxy configuration to allow `license.dynamsoft.com`:

```yaml
# Before: Docker or Kubernetes egress policy
# Required: Allow outbound HTTPS to license.dynamsoft.com

# After: Remove that egress rule.
# IronBarcode does not require outbound network access for license validation.

# Set license via environment variable
env:
  - name: IRONBARCODE_KEY
    valueFrom:
      secretKeyRef:
        name: ironbarcode-license
        key: key
```

### Instance Management Cleanup

Dynamsoft uses an instance-based API. If your code creates `BarcodeReader` instances in service classes, field initializers, or DI registrations, all of that disappears:

**Before — Dynamsoft instance management:**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;
using Dynamsoft.License;
using Dynamsoft.Core;

public class BarcodeService : IDisposable
{
    private readonly CaptureVisionRouter _router;

    public BarcodeService()
    {
        int errorCode = LicenseManager.InitLicense("KEY", out string errorMsg);
        if (errorCode != (int)EnumErrorCode.EC_OK)
            throw new InvalidOperationException(errorMsg);

        _router = new CaptureVisionRouter();

        var settings = _router.GetSimplifiedSettings(PresetTemplate.PT_READ_BARCODES);
        settings.BarcodeSettings.ExpectedBarcodesCount = 0;
        _router.UpdateSettings(PresetTemplate.PT_READ_BARCODES, settings);
    }

    public string[] ReadFile(string path)
    {
        var result = _router.Capture(path, PresetTemplate.PT_READ_BARCODES);
        return result.GetDecodedBarcodesResult()?.GetItems()
            ?.Select(i => i.GetText()).ToArray() ?? Array.Empty<string>();
    }

    public void Dispose() => _router?.Dispose();
}
```

**After — IronBarcode static API:**

```csharp
// NuGet: dotnet add package BarCode
using IronBarCode;

public class BarcodeService
{
    // No constructor initialization — license set once at app startup
    // No Dispose — no instance to clean up

    public string[] ReadFile(string path)
    {
        var options = new BarcodeReaderOptions { ExpectMultipleBarcodes = true };
        return BarcodeReader.Read(path, options)
            .Select(r => r.Value)
            .ToArray();
    }
}
```

The class loses its constructor, its `IDisposable` implementation, and its `_router` field. If this service was registered in DI as a singleton or scoped service to manage the Dynamsoft instance lifecycle, that registration can be simplified or the service can become a set of static methods.

### Reading Speed vs Timeout Mapping

Dynamsoft uses a `Timeout` in milliseconds optimized for camera frame rates. IronBarcode uses a `ReadingSpeed` enum:

| Dynamsoft setting | IronBarcode equivalent |
|---|---|
| Low `Timeout` (30fps pipeline, prioritize speed) | `Speed = ReadingSpeed.Faster` |
| Default timeout | `Speed = ReadingSpeed.Balanced` |
| Higher timeout (prioritize accuracy) | `Speed = ReadingSpeed.Detailed` |
| Maximum accuracy, no time pressure | `Speed = ReadingSpeed.ExtremeDetail` |

For most document processing workflows where throughput matters more than sub-100ms response time, `ReadingSpeed.Balanced` is the right default:

```csharp
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};
```

## Common Migration Issues

### BarcodeResultItem.GetText() vs result.Value

The property accessor changes:

```csharp
// Before
string value = item.GetText();

// After
string value = result.Value;
```

### GetFormatString() vs result.BarcodeType

Dynamsoft items expose format via `GetFormatString()` (string) or `GetFormat()` (enum). IronBarcode exposes `BarcodeType` as a `BarcodeEncoding` enum:

```csharp
// Before
if (item.GetFormat() == EnumBarcodeFormat.BF_QR_CODE)
    Console.WriteLine("Found QR code");

// After
if (result.BarcodeType == BarcodeEncoding.QRCode)
    Console.WriteLine("Found QR code");

// For logging without enum comparison — .ToString() works on both
Console.WriteLine($"Format: {result.BarcodeType}");
```

### Null Results vs Empty Collection

Dynamsoft's `GetDecodedBarcodesResult()` can return `null` when no barcodes are found. IronBarcode returns an empty collection. Update null checks:

```csharp
// Before: null check required
var result = router.Capture(path, PresetTemplate.PT_READ_BARCODES);
var items = result.GetDecodedBarcodesResult()?.GetItems();
if (items != null && items.Length > 0)
    Process(items[0].GetText());

// After: null-safe but also correct to check Count
var results = BarcodeReader.Read(path);
if (results.Any())
    Process(results.First().Value);
```

### SimplifiedCaptureVisionSettings to BarcodeReaderOptions

The `GetSimplifiedSettings` / `UpdateSettings` pattern becomes `BarcodeReaderOptions` passed to `Read`:

```csharp
// Before
var settings = router.GetSimplifiedSettings(PresetTemplate.PT_READ_BARCODES);
settings.BarcodeSettings.DeblurModes = ...;
settings.BarcodeSettings.ExpectedBarcodesCount = 0;
settings.Timeout = 500;
router.UpdateSettings(PresetTemplate.PT_READ_BARCODES, settings);
var result = router.Capture(path, PresetTemplate.PT_READ_BARCODES);

// After
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};
var results = BarcodeReader.Read(path, options);
```

## Migration Checklist

Run these searches to find every Dynamsoft reference that needs updating:

```bash
grep -r "using Dynamsoft\." --include="*.cs" .
grep -r "LicenseManager.InitLicense\|EnumErrorCode\|EC_OK\|DBR_OK" --include="*.cs" .
grep -r "new CaptureVisionRouter()\|router\.Capture\|cvRouter" --include="*.cs" .
grep -r "BarcodeResultItem\|GetDecodedBarcodesResult\|GetText()\|GetFormatString" --include="*.cs" .
grep -r "GetSimplifiedSettings\|UpdateSettings\|SimplifiedCaptureVisionSettings" --include="*.cs" .
grep -r "InitLicenseFromDevice\|InitLicenseFromLicenseContent" --include="*.cs" .
```

Work through each match:

- `using Dynamsoft.CVR / DBR / License / Core` → `using IronBarCode`
- `LicenseManager.InitLicense(key, out errorMsg)` + error check → `IronBarCode.License.LicenseKey = "key"`
- `new CaptureVisionRouter()` → remove (static API, no instance)
- `router.Capture(path, PresetTemplate.PT_READ_BARCODES)` → `BarcodeReader.Read(path)`
- `router.Capture(imageData, ...)` → `BarcodeReader.Read(imageBytes)`
- PDF page render loop + per-page `Capture` → `BarcodeReader.Read(pdfPath)`
- `BarcodeResultItem.GetText()` → `result.Value`
- `BarcodeResultItem.GetFormat()` / `GetFormatString()` → `result.BarcodeType`
- `GetSimplifiedSettings()` + `UpdateSettings(...)` → `new BarcodeReaderOptions { ... }`
- `router.Dispose()` → remove
- Offline `InitLicenseFromDevice(...)` → remove entirely
- Remove PdfiumViewer NuGet packages if they were added only to support Dynamsoft PDF processing
- Remove Docker/Kubernetes network egress rules for `*.dynamsoft.com` license endpoints
- Set `IRONBARCODE_KEY` environment variable in deployment configuration
