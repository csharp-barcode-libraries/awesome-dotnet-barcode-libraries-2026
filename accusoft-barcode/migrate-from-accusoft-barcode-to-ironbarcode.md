# Migrating from Accusoft BarcodeXpress to IronBarcode

Most of the work in this migration is deletion. The two-key licensing system — `SolutionName`, `SolutionKey`, `UnlockRuntime`, and the `IsRuntimeUnlocked` guard — disappears entirely. The actual barcode operations (read an image, write a barcode to a file) translate to shorter, simpler IronBarcode equivalents. If your codebase has a `BarcodeService` class that wraps BarcodeXpress, the licensing code is probably 60–70% of it. After migration, that class shrinks dramatically.

## Why Migrate from Accusoft BarcodeXpress

Teams migrating from BarcodeXpress report these triggers:

**Evaluation Mode Blocks Accurate Benchmarking:** BarcodeXpress evaluation mode deliberately obscures decoded values — "1234567890" comes back as "1234...XXX". Any pre-purchase accuracy testing on real documents produces degraded output, so you cannot verify that the library reads your actual scan types correctly before committing to a purchase.

**Two-Key Licensing Increases Operational Overhead:** BarcodeXpress separates SDK licenses (`SolutionName` + `SolutionKey`) from runtime licenses (`UnlockRuntime`). CI/CD pipelines, Docker containers, and container orchestrators each need both key systems managed as separate secrets. A misconfigured runtime key silently returns partial values rather than raising an error, creating a failure mode that can pass automated tests if assertions check only for non-null results.

**Minimum Five Runtime Licenses for Any Production Deployment:** Even a single-server deployment requires purchasing at least five runtime licenses. Teams running one production server and one staging server pay for ten runtime licenses minimum, regardless of actual usage.

**40 PPM Standard Edition Ceiling:** The Standard Edition throttles processing to 40 pages per minute. A batch of 100,000 documents takes roughly 41 hours at that rate. Removing the ceiling requires upgrading to the Professional Edition, which carries a higher per-developer seat cost on top of the runtime licenses already purchased.

**No Native PDF Support:** PDF files must be rendered to images using a separate library before BarcodeXpress can read them. That external dependency adds a second licensing cost, a conversion step, and additional memory pressure from holding rendered page images.

**Thread Safety Requires Instance Isolation:** BarcodeXpress's `reader` object is stateful and not safe for concurrent access. Parallel batch processing requires one fully initialized `BarcodeXpress` instance per thread, with the full two-layer license initialization repeated in each thread context.

### The Fundamental Problem

BarcodeXpress requires a multi-step initialization and a property-based configuration call before any barcode can be read:

```csharp
// BarcodeXpress: instance creation, two-layer license setup, property call, then analyze
var barcodeXpress = new BarcodeXpress();
barcodeXpress.Licensing.SolutionName = "AcmeCorp";
barcodeXpress.Licensing.SolutionKey = Convert.ToInt64("12345678901234");
barcodeXpress.Licensing.UnlockRuntime("RuntimeKey", Convert.ToInt64("98765432109876"));

barcodeXpress.reader.SetPropertyValue(BarcodeXpress.cycBxeSetFilename, imagePath);
barcodeXpress.reader.BarcodeTypes =
    BarcodeType.LinearBarcode |
    BarcodeType.DataMatrixBarcode |
    BarcodeType.QRCodeBarcode;

var results = barcodeXpress.reader.Analyze();
var value = results.FirstOrDefault()?.BarcodeValue;
```

IronBarcode replaces this entire block with a single static call:

```csharp
// IronBarcode: one call, auto-detection, no instance, no format specification
var value = BarcodeReader.Read(imagePath).First().Value;
```

## IronBarcode vs Accusoft BarcodeXpress: Feature Comparison

| Feature | Accusoft BarcodeXpress | IronBarcode |
|---|---|---|
| **License model** | SDK key + runtime key (separate purchases) | Single perpetual key |
| **Minimum runtime licenses** | 5 (even for 1 server) | No runtime license concept |
| **Evaluation mode** | Values obscured as "1234...XXX" | Full values returned; watermark on generated output only |
| **Throughput limit** | 40 PPM (Standard Edition) | No limit at any tier |
| **Format auto-detection** | Manual — must specify `BarcodeTypes` | Automatic across all supported formats |
| **PDF support** | Requires external library to render pages | Native — `BarcodeReader.Read("doc.pdf")` |
| **API style** | Instance-based, property configuration | Static factory methods, fluent API |
| **Thread safety** | Instance-per-thread required | Stateless static methods — naturally thread-safe |
| **Docker license config** | License file mount or license server | Single environment variable |
| **CI/CD secrets required** | SDK key pair + runtime key pair | One secret |
| **.NET Framework** | Separate legacy SDK | .NET Framework 4.6.2+ (same package) |
| **Linux/macOS** | Yes | Yes — Windows x64/x86, Linux x64, macOS x64/ARM |
| **QR code with logo** | Manual GDI+ overlay required | `AddBrandLogo("logo.png")` built in |
| **Pricing entry point** | $1,960+ SDK + $2,500+ runtime (min 5) | $749 perpetual (Lite, 1 developer) |
| **Perpetual license** | Not standard — contact sales | Yes, all tiers |

## Quick Start: Accusoft BarcodeXpress to IronBarcode Migration

### Step 1: Replace NuGet Package

```bash
dotnet remove package Accusoft.BarcodeXpress.NetCore
dotnet add package IronBarcode
```

### Step 2: Update Namespaces

```csharp
// Remove
using Accusoft.BarcodeXpressSdk;

// Add
using IronBarCode;
```

### Step 3: Initialize License

Add license initialization once at application startup in `Program.cs`, `Startup.cs`, or your dependency injection configuration:

```csharp
// NuGet: dotnet add package IronBarcode
// Add once at application startup
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

In production, read the key from an environment variable or your secrets manager:

```csharp
IronBarCode.License.LicenseKey =
    Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE")
    ?? throw new InvalidOperationException("IronBarcode license key not configured");
```

No `SolutionKey`, no `UnlockRuntime`, no `IsRuntimeUnlocked` check. That is the complete license setup.

## Code Migration Examples

### License Initialization

**BarcodeXpress Approach:**

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

**IronBarcode Approach:**

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

The entire 15-line constructor exists solely to manage two license key systems. IronBarcode replaces it with one line at application startup. The `BarcodeService` class requires no constructor at all.

### Barcode Reading

**BarcodeXpress Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

public string ReadFirstBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.First().Value;
}
```

The `BarcodeTypes` filter disappears entirely. IronBarcode auto-detects format across all supported symbologies. If a supplier switches from Code 128 to DataMatrix, IronBarcode continues to work without any code change. Result property names also change: `BarcodeValue` becomes `Value`, and `BarcodeType` becomes `Format`. A solution-wide search-and-replace handles both renames.

### Reading Multiple Barcodes from One Image

**BarcodeXpress Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

public IReadOnlyList<string> ReadAllBarcodes(string imagePath)
{
    var options = new BarcodeReaderOptions { ExpectMultipleBarcodes = true };
    var results = BarcodeReader.Read(imagePath, options);
    return results.Select(r => r.Value).ToList();
}
```

`ExpectMultipleBarcodes` tells IronBarcode to continue scanning after the first match, which is useful for documents with several codes in different regions. For more detail on reading options, see the [IronBarcode reading documentation](https://ironsoftware.com/csharp/barcode/how-to/read-barcode-from-image/).

### Batch Processing

BarcodeXpress is instance-based and its `reader` object is stateful, so parallel batch processing requires one instance per thread with the full two-layer license initialization repeated per thread. IronBarcode's static methods are stateless, so `Parallel.ForEach` needs no instance management.

**BarcodeXpress Approach:**

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

**IronBarcode Approach:**

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

If the BarcodeXpress Standard Edition's 40-pages-per-minute ceiling caused you to add rate-limit handling code — sleeping between groups of 40, tracking a per-minute counter, or adding delays — remove that code entirely. IronBarcode has no throughput ceiling.

### Adding PDF Support

BarcodeXpress does not read barcodes directly from PDF files. The typical workaround involves adding a separate PDF rendering library, rendering each page to an image in memory, and then passing those images to the barcode reader one at a time.

**BarcodeXpress Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

// One call — no rendering step, no external dependency
var results = BarcodeReader.Read("document.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: [{barcode.Format}] {barcode.Value}");
}
```

If `Aspose.PDF`, `PdfiumViewer`, `itext7`, or any other PDF library was added solely to support the BarcodeXpress reading workflow, it can be removed after migration. See the [IronBarcode PDF reading guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcode-pdf/) for additional options including page range selection.

### Barcode Generation

**BarcodeXpress Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

public void GenerateBarcode(string data, string outputPath)
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ResizeTo(400, 100)
        .SaveAsPng(outputPath);
}
```

The fluent chain replaces four property assignments and a `SaveToFile` call. To get the barcode as binary data for storing in a database or returning from an API:

```csharp
byte[] pngBytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

### QR Code Generation

**BarcodeXpress Approach:**

```csharp
_barcodeXpress.writer.BarcodeType = BarcodeType.QRCodeBarcode;
_barcodeXpress.writer.BarcodeValue = data;
_barcodeXpress.writer.SaveToFile("qr.png");
// Logo overlay required manual GDI+ drawing afterward
```

**IronBarcode Approach:**

```csharp
using IronBarCode;
using IronSoftware.Drawing;

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
QRCodeWriter.CreateQrCode("https://example.com", 300)
    .ChangeBarCodeColor(Color.DarkBlue)
    .SaveAsPng("qr-colored.png");
```

Logo embedding, error correction level, and color customization are all built into the fluent API. BarcodeXpress requires manual GDI+ drawing after the initial file write for any of these features. For more QR code options, see the [QR code generation guide](https://ironsoftware.com/csharp/barcode/how-to/create-qr-code-logo/).

## Accusoft BarcodeXpress API to IronBarcode Mapping Reference

| Accusoft BarcodeXpress | IronBarcode |
|---|---|
| `new BarcodeXpress()` | Static methods — no instance required |
| `Licensing.SolutionName = "..."` | `IronBarCode.License.LicenseKey = "key"` |
| `Licensing.SolutionKey = longValue` | (removed — not needed) |
| `Licensing.UnlockRuntime(key, solutionKey)` | (removed — no runtime license concept) |
| `Licensing.IsRuntimeUnlocked` | (removed — license is either valid or not) |
| `reader.SetPropertyValue(BarcodeXpress.cycBxeSetFilename, path)` | `BarcodeReader.Read(path)` |
| `reader.BarcodeTypes = BarcodeType.LinearBarcode \| ...` | (removed — auto-detection handles all formats) |
| `reader.Analyze()` | (part of `BarcodeReader.Read`) |
| `result.BarcodeValue` | `result.Value` |
| `result.BarcodeType` | `result.Format` |
| `writer.BarcodeType = BarcodeType.Code128` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)` |
| `writer.BarcodeValue = "data"` | (first argument to `CreateBarcode`) |
| `writer.Dpi = 300` | `.ResizeTo(width, height)` |
| `writer.SaveToFile(path)` | `.SaveAsPng(path)` |
| `writer.BarcodeType = BarcodeType.QRCodeBarcode` | `QRCodeWriter.CreateQrCode(data, size)` |
| 40 PPM Standard limit | No throughput limit at any tier |

## Common Migration Issues and Solutions

### Issue 1: Property Name Compile Errors

**BarcodeXpress:** Result objects expose `BarcodeValue` and `BarcodeType`, which do not exist in IronBarcode.

**Solution:** Run search-and-replace across the solution before building:

```bash
grep -r "\.BarcodeValue" --include="*.cs" .
grep -r "\.BarcodeType" --include="*.cs" .
```

Replace `result.BarcodeValue` with `result.Value` and `result.BarcodeType` with `result.Format` throughout all files found.

### Issue 2: Orphaned IsRuntimeUnlocked Guard Blocks

**BarcodeXpress:** Guard blocks that check `IsRuntimeUnlocked` and throw if the runtime license is not active exist throughout many codebases as a defense against the silent partial-value failure mode.

**Solution:** Locate and delete all such guard blocks — IronBarcode has no equivalent state. The license key is either valid or not, and if it is not, the library raises a clear exception rather than returning degraded output:

```bash
grep -r "IsRuntimeUnlocked" --include="*.cs" .
grep -r "UnlockRuntime" --include="*.cs" .
grep -r "SolutionName" --include="*.cs" .
```

All of these call sites can be deleted.

### Issue 3: Docker License File Mount to Environment Variable

**BarcodeXpress:** Docker deployments typically mount a BarcodeXpress license configuration file into the container at a known path using a `COPY` or volume mount instruction.

**Solution:** Remove the config file copy and replace with an environment variable:

```dockerfile
# Remove this
COPY accusoft-license.config /app/config/

# Add this
ENV IRONBARCODE_LICENSE=YOUR-KEY-HERE
```

In application startup:

```csharp
IronBarCode.License.LicenseKey =
    Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

In Kubernetes, the license key becomes a secret reference in the pod spec:

```yaml
env:
  - name: IRONBARCODE_LICENSE
    valueFrom:
      secretKeyRef:
        name: ironbarcode-secrets
        key: license-key
```

### Issue 4: Rate Limit Handling Code

**BarcodeXpress:** Teams processing large batches under the Standard Edition 40-PPM ceiling often added code to sleep between groups of documents, track per-minute counters, or introduce delays to stay within the limit.

**Solution:** Remove all rate-limiting code. Locate it with:

```bash
grep -r "40\|PPM\|pagesPerMinute\|minuteStart" --include="*.cs" .
```

IronBarcode has no throughput ceiling — process at whatever rate your infrastructure supports.

### Issue 5: Thread Safety — Instance Pool Removal

**BarcodeXpress:** Codebases that support concurrent processing often maintain a pool of `BarcodeXpress` instances or use a `ThreadLocal<BarcodeXpress>` pattern to isolate the stateful reader per thread.

**Solution:** Remove the pooling logic entirely. IronBarcode's `BarcodeReader.Read` and `BarcodeWriter.CreateBarcode` are stateless static methods. Concurrent calls from any number of threads do not interfere with each other.

## Accusoft BarcodeXpress Migration Checklist

### Pre-Migration Tasks

Audit the codebase to identify all BarcodeXpress usage before making any code changes:

```bash
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

- Note which barcode formats are explicitly listed in `BarcodeTypes` assignments — these all become auto-detected in IronBarcode
- Identify any rate-limit handling code that can be removed
- Identify any PDF rendering libraries added solely to support BarcodeXpress reading — these can also be removed
- Obtain your IronBarcode license key and add it to your secrets manager

### Code Update Tasks

1. Remove `Accusoft.BarcodeXpress.NetCore` from all `.csproj` files
2. Add `IronBarcode` to all `.csproj` files that need barcode functionality
3. Replace `using Accusoft.BarcodeXpressSdk;` with `using IronBarCode;` across all files
4. Add `IronBarCode.License.LicenseKey = ...` to application startup
5. Delete all `BarcodeXpress` constructor calls and instance fields
6. Delete all `Licensing.SolutionName`, `Licensing.SolutionKey`, and `UnlockRuntime` calls
7. Delete all `IsRuntimeUnlocked` guard blocks
8. Replace `reader.SetPropertyValue(BarcodeXpress.cycBxeSetFilename, path)` + `reader.Analyze()` with `BarcodeReader.Read(path)`
9. Remove all `reader.BarcodeTypes = ...` assignments
10. Replace `result.BarcodeValue` with `result.Value`
11. Replace `result.BarcodeType` with `result.Format`
12. Replace `writer.SaveToFile(path)` with `BarcodeWriter.CreateBarcode(data, encoding).SaveAsPng(path)`
13. Remove rate-limit throttling code
14. Remove PDF rendering intermediary code if present
15. Remove instance pooling or `ThreadLocal` patterns for BarcodeXpress instances
16. Remove license file mounts from Dockerfile and docker-compose files
17. Add `IRONBARCODE_LICENSE` environment variable to Docker and container configs
18. Update Kubernetes secrets and pod specs
19. Update CI/CD pipeline secrets — remove `ACCUSOFT_SOLUTION_KEY` and `ACCUSOFT_RUNTIME_KEY`; add `IRONBARCODE_LICENSE`

### Post-Migration Testing

- Build the solution — fix any remaining compile errors from renamed properties
- Run unit tests against all barcode read operations
- Run unit tests against all barcode generation operations
- Test with PDF inputs if applicable
- Run a batch processing test to confirm no throughput throttling behavior
- Deploy to a staging environment and verify the license key resolves correctly from the environment variable

## Key Benefits of Migrating to IronBarcode

**Elimination of Two-Key License Complexity:** The `SolutionName`, `SolutionKey`, `UnlockRuntime`, and `IsRuntimeUnlocked` guard pattern disappears entirely. One key covers every environment from development to production, and the trial mode returns complete values so pre-purchase benchmarking on real documents is possible.

**No Runtime License Minimum Purchase:** BarcodeXpress requires a minimum of five runtime licenses for any production deployment. IronBarcode has no runtime license concept — the single perpetual key covers any number of production deployments within its tier.

**Throughput Without a Ceiling:** The Standard Edition 40-PPM cap and any rate-limiting code written to stay within it can be removed. IronBarcode processes at whatever rate the underlying hardware and network support, with no software-imposed ceiling at any pricing tier.

**Native PDF Reading:** `BarcodeReader.Read("document.pdf")` processes PDF files directly, returning per-page results with page number, format, value, and confidence score. Any PDF rendering library added solely to support BarcodeXpress can be removed, reducing dependencies and licensing costs.

**Stateless Thread Safety:** `BarcodeReader.Read` and `BarcodeWriter.CreateBarcode` are stateless static methods. Instance pools, `ThreadLocal` patterns, and per-thread initialization blocks can all be removed. Concurrent processing with `Parallel.ForEach` requires no structural changes beyond removing the threading workarounds.

**Simplified Container Deployment:** Docker and Kubernetes deployments use a single environment variable for license configuration. License file distribution, license server infrastructure, and config file mounts are all eliminated.
