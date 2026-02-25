# Migrating from Dynamsoft Barcode Reader to IronBarcode

Most developers who migrate from Dynamsoft to IronBarcode fall into one of two groups: those who chose Dynamsoft for its reputation and then discovered the camera-centric API didn't match a document processing use case, and those running in air-gapped or Docker environments where the license server dependency caused production incidents.

If you are in the first group, the migration removes the external PDF rendering library, the per-page render loop, and the error-code license pattern. If you are in the second group, the migration removes the `InitLicense` network call, the `OutputLicenseToString` UUID management, and the outbound network policy from your Docker or VPC configuration. Either way, the codebase gets shorter after this migration.

This guide is honest about what you lose: if your application processes real-time camera frames at 30fps, Dynamsoft's algorithms are tuned for that workload and IronBarcode is not the right replacement. This migration guide is for server-side file processing, document workflows, and environments where license server access is a problem.

## Step 1: Swap the NuGet Packages

```bash
dotnet remove package Dynamsoft.DotNet.BarcodeReader
dotnet add package IronBarcode
```

If your project also has a PDF rendering library added specifically for Dynamsoft (PdfiumViewer is the most common), that can be removed too:

```bash
# Remove if added only for Dynamsoft PDF support
dotnet remove package PdfiumViewer
dotnet remove package PdfiumViewer.Native.x86_64.v8-xfa
```

## Step 2: Replace License Initialization

This is where the most immediate simplification happens. The Dynamsoft pattern requires an error code check and exception handling around every startup:

**Before — Dynamsoft:**

```csharp
using Dynamsoft.DBR;

// Must run before any barcode operations — contacts license.dynamsoft.com
int errorCode = BarcodeReader.InitLicense("YOUR-LICENSE-KEY", out string errorMsg);
if (errorCode != (int)EnumErrorCode.DBR_OK)
    throw new InvalidOperationException($"License validation failed [{errorCode}]: {errorMsg}");
```

**After — IronBarcode:**

```csharp
// NuGet: dotnet add package IronBarcode
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
grep -r "using Dynamsoft.DBR" --include="*.cs" .
```

Replace each occurrence:

```csharp
// Before
using Dynamsoft.DBR;

// After
using IronBarCode;
```

## Code Migration Examples

### Basic File Reading

The most fundamental operation — reading a barcode from an image file.

**Before — Dynamsoft:**

```csharp
using Dynamsoft.DBR;

public string ReadBarcodeFromFile(BarcodeReader reader, string imagePath)
{
    TextResult[] results = reader.DecodeFile(imagePath, "");
    if (results == null || results.Length == 0)
        return null;

    return results[0].BarcodeText;
}
```

**After — IronBarcode:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public string ReadBarcodeFromFile(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results?.FirstOrDefault()?.Value;
}
```

The `reader` instance is gone. `BarcodeReader.Read` is static. `TextResult[].BarcodeText` becomes `.Value`. The `null` check on `results` is cleaner with LINQ.

### Reading Multiple Barcodes

**Before — Dynamsoft:**

```csharp
using Dynamsoft.DBR;

public List<string> ReadAllBarcodes(BarcodeReader reader, string imagePath)
{
    var settings = reader.GetRuntimeSettings();
    settings.ExpectedBarcodesCount = 0; // 0 = find all
    reader.UpdateRuntimeSettings(settings);

    TextResult[] results = reader.DecodeFile(imagePath, "");
    var values = new List<string>();

    if (results != null)
    {
        foreach (var result in results)
            values.Add(result.BarcodeText);
    }

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
using Dynamsoft.DBR;

// Requires width, height, stride, and pixel format — low-level buffer API
public string ReadFromBuffer(BarcodeReader reader, byte[] rawPixels, int width, int height)
{
    int stride = width * 3; // assuming 24bpp RGB
    var results = reader.DecodeBuffer(rawPixels, width, height, stride,
        EnumImagePixelFormat.IPF_RGB_888, "");

    return results?.FirstOrDefault()?.BarcodeText;
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
// Requires: Dynamsoft.DotNet.BarcodeReader + PdfiumViewer + PdfiumViewer.Native.*
using Dynamsoft.DBR;
using PdfiumViewer;
using System.Drawing.Imaging;

public List<string> ReadBarcodesFromPdf(BarcodeReader reader, string pdfPath)
{
    var allBarcodes = new List<string>();

    using (var pdfDoc = PdfDocument.Load(pdfPath))
    {
        for (int page = 0; page < pdfDoc.PageCount; page++)
        {
            using var image = pdfDoc.Render(page, 300, 300, true);
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);

            TextResult[] results = reader.DecodeFileInMemory(ms.ToArray(), "");
            if (results != null)
            {
                foreach (var result in results)
                    allBarcodes.Add(result.BarcodeText);
            }
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

The page loop, the PdfDocument, the image rendering at 300 DPI, the MemoryStream, and the `DecodeFileInMemory` call all disappear. IronBarcode handles PDF pages internally.

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
// Dynamsoft offline: get UUID, send to support, receive licenseContent
string uuid = BarcodeReader.OutputLicenseToString();
// ... send uuid to support@dynamsoft.com, receive licenseContent ...
int errorCode = BarcodeReader.InitLicenseFromLicenseContent(
    "YOUR-LICENSE-KEY",
    licenseContent,
    uuid,
    out string errorMsg);

if (errorCode != (int)EnumErrorCode.DBR_OK)
    throw new InvalidOperationException($"Offline license failed: {errorMsg}");
```

**After — IronBarcode:**

```csharp
// Remove all of the above. Replace with:
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

No UUID. No support email. No licenseContent string. The key validates locally.

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
public class BarcodeService : IDisposable
{
    private readonly BarcodeReader _reader;

    public BarcodeService()
    {
        int errorCode = BarcodeReader.InitLicense("KEY", out string errorMsg);
        if (errorCode != (int)EnumErrorCode.DBR_OK)
            throw new InvalidOperationException(errorMsg);

        _reader = new BarcodeReader();

        var settings = _reader.GetRuntimeSettings();
        settings.ExpectedBarcodesCount = 0;
        _reader.UpdateRuntimeSettings(settings);
    }

    public string[] ReadFile(string path)
    {
        var results = _reader.DecodeFile(path, "");
        return results?.Select(r => r.BarcodeText).ToArray() ?? Array.Empty<string>();
    }

    public void Dispose()
    {
        _reader?.Dispose();
    }
}
```

**After — IronBarcode static API:**

```csharp
// NuGet: dotnet add package IronBarcode
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

The class loses its constructor, its `IDisposable` implementation, and its `_reader` field. If this service was registered in DI as a singleton or scoped service to manage the Dynamsoft instance lifecycle, that registration can be simplified or the service can become a set of static methods.

### Reading Speed vs Timeout Mapping

Dynamsoft uses a `Timeout` in milliseconds optimized for camera frame rates. IronBarcode uses a `ReadingSpeed` enum:

| Dynamsoft setting | IronBarcode equivalent |
|---|---|
| `settings.Timeout = 100` (30fps pipeline) | `Speed = ReadingSpeed.ExtremeDetail` is NOT this — this is actually the opposite |
| Low timeout (prioritize speed) | `Speed = ReadingSpeed.Balanced` |
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

### TextResult[].BarcodeText vs result.Value

The property name changes:

```csharp
// Before
string value = textResult.BarcodeText;

// After
string value = result.Value;
```

### TextResult[].BarcodeFormat vs result.Format

Both are enums, but they are different enum types. If you were using Dynamsoft's `EnumBarcodeFormat` values for comparisons or logging, switch to IronBarcode's `BarcodeEncoding`:

```csharp
// Before
if (textResult.BarcodeFormat == EnumBarcodeFormat.BF_QR_CODE)
    Console.WriteLine("Found QR code");

// After
if (result.Format == BarcodeEncoding.QRCode)
    Console.WriteLine("Found QR code");

// For logging without enum comparison — .ToString() works on both
Console.WriteLine($"Format: {result.Format}");
```

### Null Results vs Empty Collection

Dynamsoft can return `null` from `DecodeFile` when no barcodes are found. IronBarcode returns an empty collection. Update null checks:

```csharp
// Before: null check required
var results = reader.DecodeFile(path, "");
if (results != null && results.Length > 0)
    Process(results[0].BarcodeText);

// After: null-safe but also correct to check Count
var results = BarcodeReader.Read(path);
if (results.Any())
    Process(results.First().Value);
```

### RuntimeSettings to BarcodeReaderOptions

The `GetRuntimeSettings` / `UpdateRuntimeSettings` pattern becomes `BarcodeReaderOptions` passed to `Read`:

```csharp
// Before
var settings = reader.GetRuntimeSettings();
settings.DeblurLevel = 5;
settings.ExpectedBarcodesCount = 0;
settings.Timeout = 500;
reader.UpdateRuntimeSettings(settings);
var results = reader.DecodeFile(path, "");

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
grep -r "using Dynamsoft.DBR" --include="*.cs" .
grep -r "BarcodeReader.InitLicense\|EnumErrorCode\|DBR_OK" --include="*.cs" .
grep -r "new BarcodeReader()\|reader\.DecodeFile\|reader\.DecodeBuffer\|reader\.DecodeFileInMemory" --include="*.cs" .
grep -r "TextResult\|BarcodeText\|BarcodeFormat" --include="*.cs" .
grep -r "GetRuntimeSettings\|UpdateRuntimeSettings\|PublicRuntimeSettings" --include="*.cs" .
grep -r "reader\.Dispose\|OutputLicenseToString\|InitLicenseFromLicenseContent" --include="*.cs" .
```

Work through each match:

- `using Dynamsoft.DBR` → `using IronBarCode`
- `BarcodeReader.InitLicense(key, out errorMsg)` + error check → `IronBarCode.License.LicenseKey = "key"`
- `new BarcodeReader()` → remove (static API, no instance)
- `reader.DecodeFile(path, "")` → `BarcodeReader.Read(path)`
- `reader.DecodeBuffer(bytes, w, h, stride, format, "")` → `BarcodeReader.Read(imageBytes)`
- `reader.DecodeFileInMemory(bytes, "")` + PDF render loop → `BarcodeReader.Read(pdfPath)`
- `TextResult[].BarcodeText` → `result.Value`
- `TextResult[].BarcodeFormat` → `result.Format`
- `GetRuntimeSettings()` + `UpdateRuntimeSettings(settings)` → `new BarcodeReaderOptions { ... }`
- `reader.Dispose()` → remove
- `BarcodeReader.OutputLicenseToString()` + `InitLicenseFromLicenseContent(...)` → remove entirely
- Remove PdfiumViewer NuGet packages if they were added only to support Dynamsoft PDF processing
- Remove Docker/Kubernetes network egress rules for `license.dynamsoft.com`
- Set `IRONBARCODE_KEY` environment variable in deployment configuration
