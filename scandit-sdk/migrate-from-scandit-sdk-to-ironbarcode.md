# Migrating from Scandit SDK to IronBarcode

Scandit SDK is a camera scanning platform built for real-time mobile barcode detection. IronBarcode is a server-side and file-based barcode processing library for .NET. These two libraries serve different primary use cases, and migrating from one to the other is relevant only in specific circumstances.

This guide covers the migration path for teams using Scandit's .NET packages to process files, PDFs, or server-side document streams — scenarios where Scandit's camera pipeline architecture creates friction rather than value. If your core requirement is pointing a mobile camera at physical objects and receiving real-time feedback with augmented reality overlays, Scandit remains the appropriate tool and this guide does not apply. If your requirement is reading barcodes from uploaded images, extracting barcode data from PDF documents, or running barcode detection in ASP.NET Core, Azure Functions, Docker containers, or background processing services, this guide provides a complete migration path.

## Why Migrate from Scandit SDK

**Camera Pipeline Overhead:** Every Scandit integration begins with the same initialization sequence regardless of whether a camera is present in the deployment environment: construct a `DataCaptureContext`, configure `BarcodeCaptureSettings`, enumerate the symbologies to enable, acquire a `Camera` instance, assign it as the frame source, transition the camera to an active state, and enable barcode capture. This initialization exists because Scandit processes live video frames. For server-side file processing, the sequence provides no benefit and introduces stateful complexity that has no equivalent in the target environment — there is no camera in a Docker container and no frame source in an Azure Function.

**No Public Pricing:** Scandit does not publish pricing. SparkScan, MatrixScan, ID Scanning, AR Overlays, and Parser are individually priced products that each require a separate sales inquiry. A budget proposal, vendor comparison, or cost-benefit analysis cannot be completed without entering a sales cycle that may span days or weeks. IronBarcode [lists its pricing publicly](https://ironsoftware.com/csharp/barcode/licensing/) — $749, $1,499, or $2,999 as a one-time perpetual purchase — so the number is on the page before a line of code is written.

**Mandatory Symbology Declaration:** Scandit requires explicit enablement of each barcode format before a scanning session begins. In real-time camera scanning, this is a performance optimization — restricting symbologies reduces per-frame processing overhead. In file-based document processing, where incoming documents may carry any barcode format from any supplier or partner, mandatory format declaration becomes a constraint. Enumerating every possible symbology removes the performance benefit; writing format-guessing logic on top of the library adds engineering overhead.

**No PDF Support:** There is no direct equivalent to `BarcodeReader.Read("invoice.pdf")` in Scandit's API. Extracting barcodes from a PDF document requires rendering each page to a raster image using a separate library, then feeding those rendered images through a camera-simulation pipeline. This approach introduces additional dependencies, additional licensing, and substantial engineering effort for a task that is routine in document processing workflows.

### The Fundamental Problem

Scandit requires a running camera before any barcode work can begin. IronBarcode requires only a file path:

```csharp
// Scandit SDK: mandatory camera pipeline before first barcode read
var dataCaptureContext = DataCaptureContext.ForLicenseKey("YOUR-SCANDIT-LICENSE");
var settings = BarcodeCaptureSettings.Create();
settings.EnableSymbologies(new HashSet<Symbology>
{
    Symbology.Ean13Upca,
    Symbology.Ean8,
    Symbology.Code128,
    Symbology.QrCode
});
var barcodeCapture = BarcodeCapture.Create(dataCaptureContext, settings);
var camera = Camera.GetDefaultCamera();
await dataCaptureContext.SetFrameSourceAsync(camera);
await camera.SwitchToDesiredStateAsync(FrameSourceState.On);
barcodeCapture.IsEnabled = true;
// Results arrive later via BarcodeScanned event callback
```

```csharp
// IronBarcode: direct file reading
// NuGet: dotnet add package IronBarcode
IronBarCode.License.LicenseKey = "YOUR-KEY";
var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
    Console.WriteLine($"{result.Value} ({result.Format})");
```

The entire camera pipeline block — context creation, settings, symbology enablement, camera acquisition, frame source assignment, state transition — is replaced by a license key assignment and a single method call.

## IronBarcode vs Scandit SDK: Feature Comparison

| Feature | Scandit SDK | IronBarcode |
|---|---|---|
| **Primary deployment** | Mobile camera scanning | Server, cloud, desktop file processing |
| **Camera required** | Yes | No |
| **Image file reading** | Not designed for | Primary input type |
| **PDF barcode extraction** | Not supported | Native multi-page support |
| **Stream / byte array input** | Not supported | Yes |
| **Automatic format detection** | No (must specify) | Yes (default behavior) |
| **Event-driven results** | Yes (BarcodeScanned callback) | No (synchronous return) |
| **ASP.NET Core / server-side** | Not designed for | Fully supported |
| **Azure Functions / serverless** | Not practical | Fully supported |
| **Docker / Linux** | Not supported | Fully supported |
| **Barcode generation** | Not supported | Included in package |
| **Multi-barcode detection** | MatrixScan (separate product) | Included in package |
| **Pricing model** | Contact sales (per product) | Published perpetual tiers |
| **Per-scan / per-device fees** | Yes | No |

## Quick Start

### Step 1: Replace NuGet Package

Remove Scandit packages:

```bash
dotnet remove package Scandit.BarcodePicker
dotnet remove package Scandit.DataCapture.Core
dotnet remove package Scandit.DataCapture.Barcode
```

Install IronBarcode:

```bash
dotnet add package IronBarcode
```

### Step 2: Update Namespaces

Replace Scandit namespace imports with the IronBarCode namespace:

```csharp
// Before (Scandit SDK)
using Scandit.DataCapture.Core;
using Scandit.DataCapture.Barcode;
using Scandit.DataCapture.Barcode.Data;

// After (IronBarcode)
using IronBarCode;
```

### Step 3: Initialize License

Add license initialization at application startup, replacing the `DataCaptureContext.ForLicenseKey` call:

```csharp
// Before (Scandit SDK)
var dataCaptureContext = DataCaptureContext.ForLicenseKey("YOUR-SCANDIT-LICENSE");

// After (IronBarcode)
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

## Code Migration Examples

### Reading Barcodes from an Image File

Processing a static image file is a core supported scenario in IronBarcode and an unsupported workflow in Scandit. The Scandit SDK processes live camera frames; reading a file requires adapting the capture pipeline to treat a bitmap as a frame source, which has no native implementation.

**Scandit SDK Approach:**

```csharp
// Scandit SDK has no direct file-read API.
// Reading a static image requires constructing a bitmap frame source
// and routing it through the capture pipeline — an unsupported workaround.
// The standard Scandit integration assumes a running camera at all times.
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var results = BarcodeReader.Read("product-label.png");
foreach (var result in results)
{
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Format: {result.Format}");
    Console.WriteLine($"Image Region: {result.ImageRegion}");
}
```

[Reading barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) with IronBarcode requires no pipeline setup. The result collection is available immediately after the method returns, with no event subscription required.

### Extracting Barcodes from a PDF Document

PDF barcode extraction is entirely new territory when migrating from Scandit, which has no PDF support. IronBarcode reads PDF documents directly, returning results indexed by page number.

**Scandit SDK Approach:**

```csharp
// Scandit SDK: no PDF support.
// Implementing PDF barcode extraction with Scandit requires:
// 1. A separate PDF rendering library to convert pages to raster images
// 2. Routing each rendered image through the camera simulation pipeline
// 3. Correlating results back to page numbers manually
// This is not a supported workflow and requires significant custom engineering.
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var results = BarcodeReader.Read("shipping-manifest.pdf");
foreach (var result in results)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value} ({result.Format})");
}
```

For complete guidance on [reading barcodes from PDF documents](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/), including page range selection and multi-barcode handling, the IronBarcode documentation covers the full range of PDF processing scenarios. No additional PDF library dependency is required.

### Converting Event Callbacks to Direct Return Values

Scandit delivers barcode results through the `BarcodeScanned` event because live camera scanning is inherently asynchronous — results arrive when the camera detects a barcode in the current frame. IronBarcode returns results as a typed collection because file-based reading has a known completion boundary. This is one of the most structurally significant changes in the migration.

**Scandit SDK Approach:**

```csharp
// Scandit SDK: event-driven result delivery
barcodeCapture.BarcodeScanned += (sender, args) =>
{
    foreach (var barcode in args.Session.NewlyRecognizedBarcodes)
    {
        string value = barcode.Data;
        string symbology = barcode.Symbology.ToString();

        // Processing logic embedded inside event handler
        LogBarcodeResult(value, symbology);
        UpdateInventory(value);
    }
};

// Scan continues indefinitely until camera session is terminated
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var results = BarcodeReader.Read("document.png");
foreach (var result in results)
{
    // Same processing logic, now in standard control flow
    LogBarcodeResult(result.Value, result.Format.ToString());
    UpdateInventory(result.Value);
}
```

All processing logic that was embedded in event handler callbacks moves into standard sequential code. Error handling, logging, and downstream operations can be structured using ordinary control flow rather than event handler patterns.

### ASP.NET Core Endpoint for File Upload

A stateless server endpoint reading barcodes from an uploaded file represents the primary server-side use case for IronBarcode. This pattern is not supported by Scandit's camera-pipeline architecture.

**Scandit SDK Approach:**

```csharp
// Scandit SDK: not applicable to ASP.NET Core file upload scenarios.
// The DataCaptureContext requires a camera hardware session.
// There is no mechanism to process an IFormFile through the Scandit pipeline
// without substantial custom camera-simulation infrastructure.
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

[ApiController]
[Route("api/[controller]")]
public class BarcodeController : ControllerBase
{
    [HttpPost("scan")]
    public IActionResult ScanUploadedDocument(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        using var stream = file.OpenReadStream();
        var results = BarcodeReader.Read(stream);

        if (!results.Any())
            return NotFound("No barcodes detected");

        return Ok(results.Select(r => new
        {
            r.Value,
            Format = r.Format.ToString(),
            r.PageNumber
        }));
    }
}
```

The `BarcodeReader.Read` method accepts a `Stream` directly from `IFormFile.OpenReadStream()`, requiring no temporary file creation. Results are returned synchronously within the request-response cycle with no async camera management.

### Concurrent Batch Processing Across Multiple Documents

High-volume document queues require parallel processing across multiple threads. Because IronBarcode's API is stateless, it is inherently thread-safe and composes directly with `Parallel.ForEach` and async task patterns.

**Scandit SDK Approach:**

```csharp
// Scandit SDK: no supported batch file processing pattern.
// The FrameSourceState model assumes a continuous camera session,
// not a queue of discrete documents. Batch processing would require
// one camera simulation pipeline per document or serialized processing
// through a shared pipeline — neither is a supported workflow.
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;
using System.Collections.Concurrent;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};

var files = Directory.GetFiles("./documents/", "*.pdf");
var allResults = new ConcurrentBag<(string File, BarcodeResult Result)>();

Parallel.ForEach(files, file =>
{
    var results = BarcodeReader.Read(file, options);
    foreach (var result in results)
        allResults.Add((file, result));
});

foreach (var (file, result) in allResults)
    Console.WriteLine($"{Path.GetFileName(file)}: {result.Value} ({result.Format})");
```

For advanced patterns covering [async and multithreaded barcode reading](https://ironsoftware.com/csharp/barcode/how-to/async-multithread/), including throughput tuning and thread count configuration, the IronBarcode documentation provides detailed guidance.

## Scandit SDK API to IronBarcode Mapping Reference

| Scandit SDK | IronBarcode | Notes |
|---|---|---|
| `DataCaptureContext.ForLicenseKey("key")` | `IronBarCode.License.LicenseKey = "key"` | One assignment; no context object |
| `BarcodeCaptureSettings.Create()` | `new BarcodeReaderOptions()` | Optional; not required for basic reading |
| `settings.EnableSymbologies(Symbology.Code128, ...)` | (not needed) | IronBarcode auto-detects all formats |
| `Camera.GetDefaultCamera()` | (not applicable) | No camera concept |
| `dataCaptureContext.SetFrameSourceAsync(camera)` | (not applicable) | No frame source |
| `camera.SwitchToDesiredStateAsync(FrameSourceState.On)` | (not applicable) | No camera state machine |
| `barcodeCapture.IsEnabled = true` | `BarcodeReader.Read(path)` | Single call initiates reading |
| `BarcodeScanned +=` event handler | Standard `foreach` over return value | No event system needed |
| `args.Session.NewlyRecognizedBarcodes` | Return value of `BarcodeReader.Read()` | Direct collection |
| `barcode.Data` | `result.Value` | Same semantic content |
| `barcode.Symbology` | `result.Format` | Equivalent format enumeration |
| `using Scandit.DataCapture.Core` | `using IronBarCode` | Single namespace |
| `using Scandit.DataCapture.Barcode` | `using IronBarCode` | Included in same namespace |
| SparkScan product | `BarcodeReader.Read(path)` | No separate product required |
| MatrixScan product | `BarcodeReaderOptions { ExpectMultipleBarcodes = true }` | Included in base package |

## Common Migration Issues and Solutions

### Issue 1: Camera Pipeline Code Has No Direct Translation

**Scandit SDK:** The `DataCaptureContext`, `BarcodeCaptureSettings`, `Camera`, and `BarcodeCapture` setup block spans 7–10 lines of initialization code with async state transitions.

**Solution:** This block is deleted entirely. It has no IronBarcode equivalent because IronBarcode's file-based API requires no session initialization. Replace the entire block with:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

### Issue 2: BarcodeScanned Event Handler Logic

**Scandit SDK:** Processing logic is embedded inside `BarcodeScanned +=` event handler lambdas or methods. This pattern does not exist in IronBarcode.

**Solution:** Move the handler body into standard code following the `BarcodeReader.Read` call:

```csharp
// Before: event handler
barcodeCapture.BarcodeScanned += (s, args) =>
{
    foreach (var b in args.Session.NewlyRecognizedBarcodes)
        Console.WriteLine(b.Data);
};

// After: direct iteration
foreach (var result in BarcodeReader.Read("document.png"))
    Console.WriteLine(result.Value);
```

### Issue 3: EnableSymbologies Calls Scattered Across Codebase

**Scandit SDK:** Multiple places in the codebase may call `settings.EnableSymbologies(...)` with different symbology sets for different scanning contexts.

**Solution:** Remove all `EnableSymbologies` calls. IronBarcode auto-detects all supported formats by default. If format restriction is needed for performance in specific scenarios, use `BarcodeReaderOptions.ExpectBarcodeTypes`:

```csharp
var options = new BarcodeReaderOptions
{
    ExpectBarcodeTypes = BarcodeEncoding.Code128 | BarcodeEncoding.QRCode
};
var results = BarcodeReader.Read("document.png", options);
```

### Issue 4: No PDF Reading in Existing Codebase

**Scandit SDK:** If the existing codebase converts PDF pages to images before feeding them into the Scandit pipeline, this custom rendering infrastructure is no longer needed.

**Solution:** Remove the PDF-to-image rendering step entirely. Pass the PDF path directly to IronBarcode:

```csharp
// Before: render PDF pages to images, then pass each image to Scandit pipeline
// (multiple steps, external PDF library dependency)

// After: pass PDF directly to IronBarcode
var results = BarcodeReader.Read("document.pdf");
// Results include PageNumber property for each detected barcode
```

### Issue 5: Async Camera Initialization Patterns

**Scandit SDK:** Camera initialization uses `await` for `SetFrameSourceAsync` and `SwitchToDesiredStateAsync`. If the calling code is structured around these async operations, removing them simplifies the async chain.

**Solution:** Remove the async camera initialization entirely. `BarcodeReader.Read` is synchronous. If async execution is needed for responsiveness in a UI or server context, wrap it in `Task.Run`:

```csharp
// Async wrapper for non-blocking execution
var results = await Task.Run(() => BarcodeReader.Read("document.png"));
```

## Scandit SDK Migration Checklist

### Pre-Migration Tasks

Audit all Scandit integration points in the codebase:

```bash
grep -r "DataCaptureContext\|BarcodeCaptureSettings\|BarcodeCapture" --include="*.cs" .
grep -r "EnableSymbologies\|BarcodeScanned\|NewlyRecognizedBarcodes" --include="*.cs" .
grep -r "using Scandit" --include="*.cs" .
grep -r "barcode\.Data\|barcode\.Symbology\|FrameSourceState" --include="*.cs" .
```

Document all scanning contexts — identify which code paths handle real-time camera scanning (staying on Scandit) versus file or document processing (migrating to IronBarcode). Note all places where PDF pages are rendered to images before Scandit processing, as this rendering step will be eliminated.

### Code Update Tasks

1. Remove Scandit NuGet packages (`Scandit.BarcodePicker`, `Scandit.DataCapture.Core`, `Scandit.DataCapture.Barcode`)
2. Install `IronBarcode` NuGet package
3. Replace `using Scandit.DataCapture.*` imports with `using IronBarCode`
4. Delete the `DataCaptureContext` initialization block in its entirety
5. Delete all `Camera.GetDefaultCamera()` and `SetFrameSourceAsync` calls
6. Delete all `EnableSymbologies` calls
7. Replace `barcodeCapture.IsEnabled = true` with `BarcodeReader.Read(filePath)`
8. Convert `BarcodeScanned +=` event handlers to `foreach` over `BarcodeReader.Read()` results
9. Replace `barcode.Data` with `result.Value`
10. Replace `barcode.Symbology` with `result.Format`
11. Remove PDF-to-image rendering pipelines where IronBarcode now reads PDFs directly
12. Add `IronBarCode.License.LicenseKey = "YOUR-KEY"` at application startup

### Post-Migration Testing

Verify the following after completing code updates:

- Confirm barcode values returned by IronBarcode match values previously returned by Scandit for the same input documents
- Verify that all supported barcode formats in the document set are detected without `ExpectBarcodeTypes` configuration
- Test PDF extraction returns correct `PageNumber` values for multi-page documents
- Confirm ASP.NET Core endpoints return results within acceptable response times under expected load
- Verify Docker or containerized deployments complete barcode reads successfully without camera hardware
- Test concurrent batch processing with `Parallel.ForEach` produces correct results without race conditions
- Confirm the application starts and initializes correctly with the IronBarcode license key in place of the Scandit context

## Key Benefits of Migrating to IronBarcode

**Elimination of Camera Pipeline Overhead:** Removing the `DataCaptureContext` initialization sequence eliminates stateful camera management, async state transitions, and platform-specific camera APIs from server-side code. The resulting code is simpler, shorter, and does not carry platform constraints that prevent deployment on Linux, in Docker containers, or in serverless compute environments.

**Direct PDF Processing:** Native PDF barcode extraction removes the dependency on a separate PDF rendering library, the engineering overhead of the page-to-image pipeline, and the additional licensing cost of that dependency. Multi-page PDF documents are processed in a single method call with page-indexed results available immediately.

**Synchronous Stateless API:** The direct return value model replaces event-driven callback patterns with standard sequential code. Error handling, logging, and downstream processing integrate naturally with existing application architecture without requiring event subscription management or async state machine coordination.

**Published Transparent Pricing:** Perpetual license tiers listed publicly at $749, $1,499, and $2,999 enable budget proposals, vendor comparisons, and procurement processes to proceed without a sales cycle. The license cost is determinable before writing a line of integration code.

**Full Cloud and Container Support:** IronBarcode's stateless, hardware-independent API runs without modification in Azure Functions, AWS Lambda, Docker containers on Linux, and any other compute environment where the .NET runtime is available. No camera simulation, no hardware dependency workarounds, and no platform-specific configuration is required to deploy in these environments.

**Automatic Format Detection:** Removing `EnableSymbologies` declarations means incoming documents carrying unexpected barcode formats are handled correctly without code changes. When the set of formats present in incoming documents changes — new suppliers, new label formats, new document types — IronBarcode handles the new formats automatically.
