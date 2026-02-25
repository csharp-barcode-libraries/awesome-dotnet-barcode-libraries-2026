# Migrating from Accusoft BarcodeXpress to IronBarcode

Most of the work in this migration is deletion. The two-key licensing system — `SolutionName`, `SolutionKey`, `UnlockRuntime`, and the `IsRuntimeUnlocked` guard — disappears entirely. The actual barcode operations (read an image, write a barcode to a file) translate to shorter, simpler IronBarcode equivalents. If your codebase has a `BarcodeService` class that wraps BarcodeXpress, the licensing code is probably 60-70% of it. After migration, that class shrinks dramatically.

## Quick Start

### Step 1: Swap the NuGet Package

```bash
dotnet remove package Accusoft.BarcodeXpress.NetCore
dotnet add package IronBarcode
```

### Step 2: Replace the Namespace

```csharp
// Remove
using Accusoft.BarcodeXpressSdk;

// Add
using IronBarCode;
```

### Step 3: Set the License Key

```csharp
// NuGet: dotnet add package IronBarcode
// Add once at application startup (Program.cs, Startup.cs, or your DI setup)
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

That is the complete license setup. No `SolutionKey`, no `UnlockRuntime`, no `IsRuntimeUnlocked` check. In production, read the key from an environment variable or your secrets manager:

```csharp
IronBarCode.License.LicenseKey =
    Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE")
    ?? throw new InvalidOperationException("IronBarcode license key not configured");
```

## Code Migration Examples

### License Initialization

This is where most migration time is saved. BarcodeXpress requires a 10-15 line initialization block with two separate key systems. IronBarcode replaces it with one line.

**Before — BarcodeXpress two-key initialization:**

```csharp
using Accusoft.BarcodeXpressSdk;

public class BarcodeService
{
    private readonly BarcodeXpress _barcodeXpress;

    public BarcodeService()
    {
        _barcodeXpress = new BarcodeXpress();

        // Layer 1: SDK license
        _barcodeXpress.Licensing.SolutionName = "AcmeCorp";
        _barcodeXpress.Licensing.SolutionKey = Convert.ToInt64("12345678901234");

        // Layer 2: Runtime license — separate purchase, minimum 5 licenses required
        _barcodeXpress.Licensing.UnlockRuntime(
            "RuntimeKey-XXXXXX",
            Convert.ToInt64("98765432109876"));

        // Guard: evaluation mode silently returns partial values
        if (!_barcodeXpress.Licensing.IsRuntimeUnlocked)
        {
            throw new InvalidOperationException(
                "Runtime license not active — barcode values will be obscured");
        }
    }
}
```

**After — IronBarcode single-key initialization:**

```csharp
using IronBarCode;

// In Program.cs
IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");

// BarcodeService no longer needs a constructor or any license management
public class BarcodeService
{
    // Ready to use — no initialization needed
}
```

### Barcode Reading

The BarcodeXpress reading pattern uses `SetPropertyValue` with a string constant to identify the file, then a separate `Analyze()` call. IronBarcode combines this into a single `BarcodeReader.Read()` call.

**Before — BarcodeXpress reading:**

```csharp
using Accusoft.BarcodeXpressSdk;

public string ReadFirstBarcode(string imagePath)
{
    // Instance must already be licensed — see initialization above
    _barcodeXpress.reader.SetPropertyValue(
        BarcodeXpress.cycBxeSetFilename, imagePath);

    _barcodeXpress.reader.BarcodeTypes =
        BarcodeType.LinearBarcode |
        BarcodeType.DataMatrixBarcode |
        BarcodeType.QRCodeBarcode;

    var results = _barcodeXpress.reader.Analyze();

    return results.FirstOrDefault()?.BarcodeValue;
}
```

**After — IronBarcode reading:**

```csharp
using IronBarCode;

public string ReadFirstBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.First().Value;
}
```

The `BarcodeTypes` filter disappears entirely. IronBarcode auto-detects format across all supported symbologies. If a document's barcode format changes — say, a supplier switches from Code 128 to DataMatrix — IronBarcode continues to work without any code change.

Result property names also change: `BarcodeValue` becomes `Value`, and `BarcodeType` becomes `Format`. If you are using these properties throughout the codebase, search-and-replace handles them.

### Reading Multiple Barcodes from One Image

**Before — BarcodeXpress multi-result reading:**

```csharp
using Accusoft.BarcodeXpressSdk;

public IReadOnlyList<string> ReadAllBarcodes(string imagePath)
{
    _barcodeXpress.reader.SetPropertyValue(
        BarcodeXpress.cycBxeSetFilename, imagePath);
    _barcodeXpress.reader.BarcodeTypes =
        BarcodeType.LinearBarcode | BarcodeType.QRCodeBarcode;

    var results = _barcodeXpress.reader.Analyze();
    return results.Select(r => r.BarcodeValue).ToList();
}
```

**After — IronBarcode multi-result reading:**

```csharp
using IronBarCode;

public IReadOnlyList<string> ReadAllBarcodes(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.Select(r => r.Value).ToList();
}
```

To explicitly tell IronBarcode to look for multiple barcodes per image (useful for documents with several codes in different regions):

```csharp
using IronBarCode;

public IReadOnlyList<string> ReadAllBarcodes(string imagePath)
{
    var options = new BarcodeReaderOptions { ExpectMultipleBarcodes = true };
    var results = BarcodeReader.Read(imagePath, options);
    return results.Select(r => r.Value).ToList();
}
```

### Batch Processing

BarcodeXpress is instance-based, which means parallel batch processing requires one instance per thread (because the `reader` object is stateful and not thread-safe across concurrent callers). IronBarcode's static methods are stateless, so `Parallel.ForEach` needs no instance management.

**Before — BarcodeXpress sequential batch (or per-thread instances for parallel):**

```csharp
using Accusoft.BarcodeXpressSdk;
using System.Collections.Generic;

public Dictionary<string, string> ProcessBatch(IEnumerable<string> imagePaths)
{
    var results = new Dictionary<string, string>();

    foreach (var path in imagePaths)
    {
        _barcodeXpress.reader.SetPropertyValue(
            BarcodeXpress.cycBxeSetFilename, path);
        _barcodeXpress.reader.BarcodeTypes =
            BarcodeType.LinearBarcode | BarcodeType.QRCodeBarcode;

        var barcodes = _barcodeXpress.reader.Analyze();
        if (barcodes.Length > 0)
            results[path] = barcodes[0].BarcodeValue;
    }

    return results;
}
```

**After — IronBarcode parallel batch:**

```csharp
using IronBarCode;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public Dictionary<string, string> ProcessBatch(string[] imagePaths)
{
    var results = new ConcurrentDictionary<string, string>();

    Parallel.ForEach(imagePaths, file =>
    {
        var r = BarcodeReader.Read(file);
        var first = r.FirstOrDefault();
        if (first != null)
            results[file] = first.Value;
    });

    return new Dictionary<string, string>(results);
}
```

Note also that the Standard Edition of BarcodeXpress throttles processing to 40 pages per minute. If your batch was large enough that you added rate-limit handling code — sleeping between groups of 40 — remove that code entirely. IronBarcode has no throughput ceiling.

### Adding PDF Support

BarcodeXpress does not read barcodes directly from PDF files. Teams that need this typically add a separate PDF rendering library, render each page to an image in memory, then pass those images to the barcode reader one at a time. That workflow adds a dependency, adds a license cost, and adds memory pressure from holding rendered page images.

**Before — BarcodeXpress with separate PDF rendering:**

```csharp
// Requires a separate PDF library (not shown — varies by team choice)
// Pattern: render PDF pages to images, then scan each image
foreach (var pageImage in pdfRenderer.RenderPages("document.pdf"))
{
    using var ms = new MemoryStream();
    pageImage.Save(ms, ImageFormat.Png);
    ms.Seek(0, SeekOrigin.Begin);

    _barcodeXpress.reader.SetPropertyValue(
        BarcodeXpress.cycBxeSetFilename, /* temp file path */);
    var barcodes = _barcodeXpress.reader.Analyze();
    // ... collect results
}
```

**After — IronBarcode native PDF reading:**

```csharp
using IronBarCode;

// One call — no rendering step, no external dependency
var results = BarcodeReader.Read("document.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: [{barcode.Format}] {barcode.Value}");
}
```

If you added `Aspose.PDF`, `PdfiumViewer`, `itext7`, or any other PDF library purely to support the BarcodeXpress reading workflow, you can remove it. IronBarcode reads PDFs natively.

### Barcode Generation

**Before — BarcodeXpress Code 128 generation:**

```csharp
using Accusoft.BarcodeXpressSdk;

public void GenerateBarcode(string data, string outputPath)
{
    _barcodeXpress.writer.BarcodeType = BarcodeType.Code128;
    _barcodeXpress.writer.BarcodeValue = data;
    _barcodeXpress.writer.Dpi = 300;
    _barcodeXpress.writer.ImageFormat = ImageFormat.Png;
    _barcodeXpress.writer.SaveToFile(outputPath);
}
```

**After — IronBarcode Code 128 generation:**

```csharp
using IronBarCode;

public void GenerateBarcode(string data, string outputPath)
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .SaveAsPng(outputPath);
}
```

To generate with specific dimensions:

```csharp
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng(outputPath);
```

To get the barcode as binary data (for storing in a database or returning from an API):

```csharp
byte[] pngBytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

### QR Code Generation

**Before — BarcodeXpress QR generation (no native logo support):**

```csharp
_barcodeXpress.writer.BarcodeType = BarcodeType.QRCodeBarcode;
_barcodeXpress.writer.BarcodeValue = data;
_barcodeXpress.writer.SaveToFile("qr.png");
// Logo overlay required manual GDI+ drawing afterward
```

**After — IronBarcode QR generation:**

```csharp
using IronBarCode;

// Simple QR
QRCodeWriter.CreateQrCode("https://example.com", 500)
    .SaveAsPng("qr.png");

// With branded logo
QRCodeWriter.CreateQrCode("https://example.com", 500)
    .AddBrandLogo("logo.png")
    .SaveAsPng("qr-branded.png");

// With high error correction (for logo overlays)
QRCodeWriter.CreateQrCode(
    "https://example.com",
    500,
    QRCodeWriter.QrErrorCorrectionLevel.Highest)
    .AddBrandLogo("logo.png")
    .SaveAsPng("qr-branded-high-ecc.png");

// Colored QR code
using IronSoftware.Drawing;
QRCodeWriter.CreateQrCode("https://example.com", 300)
    .ChangeBarCodeColor(Color.DarkBlue)
    .SaveAsPng("qr-colored.png");
```

## Common Migration Issues

### Property Name Changes

These two are the most frequent compile errors after swapping packages:

| BarcodeXpress | IronBarcode |
|---|---|
| `result.BarcodeValue` | `result.Value` |
| `result.BarcodeType` | `result.Format` |

Run a find-and-replace across the solution:

```bash
# Find all occurrences to update
grep -r "\.BarcodeValue" --include="*.cs" .
grep -r "\.BarcodeType" --include="*.cs" .
```

### Removing the IsRuntimeUnlocked Check Pattern

Search for `IsRuntimeUnlocked` throughout the codebase and remove those guard blocks. IronBarcode has no equivalent state — the license key is either valid or not, and if it is not, the library throws a clear exception rather than silently returning degraded output.

```bash
grep -r "IsRuntimeUnlocked" --include="*.cs" .
grep -r "UnlockRuntime" --include="*.cs" .
grep -r "SolutionName" --include="*.cs" .
```

All of these call sites can be deleted.

### Docker: License File Mounting to Environment Variable

If your Docker deployment mounts a BarcodeXpress license configuration file:

```dockerfile
# Remove this
COPY accusoft-license.config /app/config/
```

Replace with an environment variable:

```dockerfile
# Add this
ENV IRONBARCODE_LICENSE=YOUR-KEY-HERE
```

And in your application startup:

```csharp
IronBarCode.License.LicenseKey =
    Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

In Kubernetes, this becomes a secret reference in the pod spec:

```yaml
env:
  - name: IRONBARCODE_LICENSE
    valueFrom:
      secretKeyRef:
        name: ironbarcode-secrets
        key: license-key
```

### Rate Limit Handling Code

If you added code to stay within BarcodeXpress Standard's 40-pages-per-minute ceiling — sleeping between batches, tracking a per-minute counter, adding delays — remove it entirely:

```bash
# Find throttle patterns to delete
grep -r "40\|PPM\|pagesPerMinute\|minuteStart" --include="*.cs" .
```

None of that applies to IronBarcode. Process at whatever rate your infrastructure supports.

### Thread Safety: Instance-Per-Thread to Stateless

If you maintained a pool of `BarcodeXpress` instances to support concurrent processing, or used a `ThreadLocal<BarcodeXpress>` pattern, remove the pooling logic. IronBarcode's `BarcodeReader.Read` and `BarcodeWriter.CreateBarcode` are stateless static methods. Concurrent calls from any number of threads do not interfere with each other.

## Migration Checklist

Use these grep patterns to locate every BarcodeXpress usage in the codebase before making any code changes:

```bash
# Find all BarcodeXpress references
grep -r "BarcodeXpress" --include="*.cs" .
grep -r "Accusoft" --include="*.cs" .
grep -r "Licensing\.SolutionName" --include="*.cs" .
grep -r "Licensing\.SolutionKey" --include="*.cs" .
grep -r "UnlockRuntime" --include="*.cs" .
grep -r "IsRuntimeUnlocked" --include="*.cs" .
grep -r "cycBxeSetFilename" --include="*.cs" .
grep -r "reader\.Analyze" --include="*.cs" .
grep -r "\.BarcodeValue" --include="*.cs" .
grep -r "\.BarcodeType" --include="*.cs" .
grep -r "BarcodeType\." --include="*.cs" .
```

### Pre-Migration

- [ ] Run all grep patterns above and catalog every file that needs changes
- [ ] Note which barcode formats are explicitly listed in `BarcodeTypes` assignments — these all become auto-detected in IronBarcode
- [ ] Identify any rate-limit handling code that can be removed
- [ ] Identify any PDF rendering libraries added solely to support BarcodeXpress — these can be removed
- [ ] Obtain your IronBarcode license key and add it to your secrets manager

### Package and Namespace Changes

- [ ] Remove `Accusoft.BarcodeXpress.NetCore` from all `.csproj` files
- [ ] Add `IronBarcode` to all `.csproj` files that need barcode functionality
- [ ] Replace `using Accusoft.BarcodeXpressSdk;` with `using IronBarCode;` across all files
- [ ] Add `IronBarCode.License.LicenseKey = ...` to application startup

### Code Changes

- [ ] Delete all `BarcodeXpress` constructor calls and instance fields
- [ ] Delete all `Licensing.SolutionName`, `Licensing.SolutionKey`, and `UnlockRuntime` calls
- [ ] Delete all `IsRuntimeUnlocked` guard blocks
- [ ] Replace `reader.SetPropertyValue(BarcodeXpress.cycBxeSetFilename, path)` + `reader.Analyze()` with `BarcodeReader.Read(path)`
- [ ] Remove all `reader.BarcodeTypes = ...` assignments
- [ ] Replace `result.BarcodeValue` with `result.Value`
- [ ] Replace `result.BarcodeType` with `result.Format`
- [ ] Replace `writer.SaveToFile(path)` with `BarcodeWriter.CreateBarcode(data, encoding).SaveAsPng(path)`
- [ ] Remove rate-limit throttling code
- [ ] Remove PDF rendering intermediary code if present
- [ ] Remove instance pooling or `ThreadLocal` patterns for BarcodeXpress instances

### Infrastructure

- [ ] Remove license file mounts from Dockerfile and docker-compose files
- [ ] Add `IRONBARCODE_LICENSE` environment variable to Docker and container configs
- [ ] Update Kubernetes secrets and pod specs
- [ ] Update CI/CD pipeline secrets — remove `ACCUSOFT_SOLUTION_KEY`, `ACCUSOFT_RUNTIME_KEY`; add `IRONBARCODE_LICENSE`
- [ ] Remove any BarcodeXpress license server infrastructure if applicable

### Post-Migration Verification

- [ ] Build the solution — fix any remaining compile errors from renamed properties
- [ ] Run unit tests against all barcode read operations
- [ ] Run unit tests against all barcode generation operations
- [ ] Test with PDF inputs if applicable
- [ ] Run a batch processing test to confirm no throughput throttling behavior
- [ ] Deploy to a staging environment and verify the license key resolves correctly from the environment variable

## Pricing Reference

| IronBarcode Tier | Price | Coverage |
|---|---|---|
| Lite | $749 perpetual | 1 developer, 1 project |
| Plus | $1,499 perpetual | 3 developers |
| Professional | $2,999 perpetual | 10 developers |
| Unlimited | $5,999 perpetual | Unlimited developers |

All tiers include: Windows x64/x86, Linux x64, macOS x64/ARM, Docker, Azure App Service, AWS Lambda, .NET Framework 4.6.2+, .NET Core 3.1+, .NET 5/6/7/8/9. No runtime licenses, no minimum purchase quantities, no throughput limits.

The migration is largely mechanical. The real benefit is what you remove — two separate key systems, five-license minimums, 40 PPM ceilings, and a trial mode that obscures the values you need to evaluate the library accurately. Once those are gone, what remains is a barcode API that does less in configuration and more in actual barcode processing.
