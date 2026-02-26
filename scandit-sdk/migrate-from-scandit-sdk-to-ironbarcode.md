# Migrate from Scandit SDK to IronBarcode

Scandit is a camera scanning platform. If your project is using Scandit's .NET package to process files, PDFs, or server-side documents, you are fighting the library's architecture — and you have been from the first line of initialization code.

This guide is for teams evaluating IronBarcode as an alternative for server-side or file-based barcode processing. If your core requirement is pointing a mobile camera at physical objects and getting real-time feedback with AR overlays, that is Scandit's domain and this migration is not applicable. If your requirement is reading barcodes from images, extracting barcode data from PDFs, or running barcode processing in ASP.NET Core, Azure Functions, or Docker containers — read on.

---

## Why Migrate

### The camera pipeline is unavoidable

The Scandit SDK for .NET is built around a camera initialization sequence. Every integration starts the same way: create a `DataCaptureContext`, build `BarcodeCaptureSettings`, enable each symbology you expect to encounter, get a `Camera` instance, assign it as the frame source, switch it to an active state, and enable barcode capture. That sequence exists because Scandit was designed to process live video frames from a mobile camera. There is no "file mode" that bypasses it.

For server-side file processing, that architecture creates friction at every layer. There is no camera in a Docker container. There is no frame source in an Azure Function. Attempting to use Scandit in those environments means adapting the library to a use case it was never designed for.

### Pricing requires a sales conversation

Scandit does not publish pricing. SparkScan, MatrixScan, ID Scanning, AR Overlays, and Parser are each separately priced products that require individual quotes. If you are building a budget proposal, a vendor comparison, or a cost-benefit analysis, you cannot complete it without entering a sales cycle. IronBarcode [lists its pricing publicly](https://ironsoftware.com/csharp/barcode/licensing/) — $749, $1,499, or $2,999 as a one-time perpetual purchase — so the number is on the page before you write a line of code.

### Symbologies must be declared in advance

Scandit requires explicit symbology enablement before scanning. In real-time camera scanning, this is a sensible performance optimization — filtering symbologies reduces per-frame processing overhead. In file-based document processing, it becomes a constraint. When incoming documents may carry any barcode format, you either enumerate every possible symbology upfront or write your own format-guessing loop on top of the library. IronBarcode auto-detects all supported formats with no specification required.

### No PDF support

There is no `BarcodeReader.Read("invoice.pdf")` equivalent in Scandit. PDF barcode extraction requires rendering PDF pages to images using a separate library, then feeding those images through the camera simulation pipeline. IronBarcode reads PDFs natively — pass the file path and get back results indexed by page number, with no additional dependencies.

---

## Quick Start: Three Steps

### Step 1: Remove Scandit

```bash
dotnet remove package Scandit.BarcodePicker
```

Also remove any Scandit DataCapture packages if added separately:

```bash
dotnet remove package Scandit.DataCapture.Core
dotnet remove package Scandit.DataCapture.Barcode
```

### Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

### Step 3: Replace the initialization and reading logic

Remove the entire camera pipeline block. Replace it with a license key assignment and a direct read call:

```csharp
// Before: Scandit camera pipeline initialization
// (all of this gets deleted)
var dataCaptureContext = DataCaptureContext.ForLicenseKey("YOUR-SCANDIT-LICENSE");
var settings = BarcodeCaptureSettings.Create();
settings.EnableSymbologies(new HashSet<Symbology> { Symbology.Code128, Symbology.QrCode });
var barcodeCapture = BarcodeCapture.Create(dataCaptureContext, settings);
var camera = Camera.GetDefaultCamera();
await dataCaptureContext.SetFrameSourceAsync(camera);
await camera.SwitchToDesiredStateAsync(FrameSourceState.On);
barcodeCapture.IsEnabled = true;

// After: IronBarcode
// NuGet: dotnet add package IronBarcode
IronBarCode.License.LicenseKey = "YOUR-KEY";
var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
    Console.WriteLine($"{result.Value} ({result.Format})");
```

---

## Code Migration Examples

### Reading barcodes from an image file

The Scandit approach to reading a static image requires constructing a frame source from a bitmap and routing it through the capture pipeline. IronBarcode [reads barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) with a direct file path or stream argument.

```csharp
// Before: Scandit (camera pipeline, simplified)
// Requires DataCaptureContext, BarcodeCapture setup (see initialization above)
// There is no direct file-read equivalent — static images must be adapted to the pipeline

// After: IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var results = BarcodeReader.Read("product-label.png");
foreach (var result in results)
{
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Format: {result.Format}");
}
```

### Extracting barcodes from a PDF

Scandit has no PDF reading capability. This capability is new territory when migrating to IronBarcode.

```csharp
// IronBarcode: read all barcodes from a multi-page PDF
// NuGet: dotnet add package IronBarcode

using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var results = BarcodeReader.Read("shipping-manifest.pdf");
foreach (var result in results)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value} ({result.Format})");
}
```

Scandit users adding document processing capabilities to their stack will find this is a common reason to introduce IronBarcode alongside an existing Scandit deployment rather than replacing it entirely.

### Replacing camera event callbacks with direct return values

Scandit delivers barcode results through event callbacks because the scanning process is asynchronous — results arrive when the camera detects a barcode in the current frame. IronBarcode returns results synchronously as a collection because the input is a file with a known completion boundary.

```csharp
// Before: Scandit event-callback pattern
barcodeCapture.BarcodeScanned += (sender, args) =>
{
    foreach (var barcode in args.Session.NewlyRecognizedBarcodes)
    {
        string value = barcode.Data;
        string symbology = barcode.Symbology.ToString();
        ProcessBarcode(value, symbology);
    }
};

// After: IronBarcode direct return
var results = BarcodeReader.Read("document.png");
foreach (var result in results)
{
    ProcessBarcode(result.Value, result.Format.ToString());
}
```

### Server-side processing in ASP.NET Core

This entire pattern — a stateless server endpoint reading barcodes from an uploaded file — is not supported by Scandit's camera-pipeline architecture. It is a core supported scenario for IronBarcode.

```csharp
// IronBarcode: ASP.NET Core endpoint
// NuGet: dotnet add package IronBarcode

using IronBarCode;

[ApiController]
[Route("api/[controller]")]
public class BarcodeController : ControllerBase
{
    [HttpPost("read")]
    public IActionResult ReadBarcode(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        using var stream = file.OpenReadStream();
        var results = BarcodeReader.Read(stream);

        if (!results.Any())
            return NotFound("No barcodes detected");

        return Ok(results.Select(r => new { r.Value, Format = r.Format.ToString() }));
    }
}
```

### Concurrent batch processing

For high-volume document queues, IronBarcode supports configurable parallelism. [Multithreaded barcode reading](https://ironsoftware.com/csharp/barcode/how-to/async-multithread/) can significantly improve throughput on multi-core server hardware.

```csharp
// IronBarcode: batch processing with parallel threads
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};

var files = Directory.GetFiles("./documents/", "*.pdf");

Parallel.ForEach(files, file =>
{
    var results = BarcodeReader.Read(file, options);
    foreach (var result in results)
        Console.WriteLine($"{file}: {result.Value} ({result.Format})");
});
```

### MAUI: capturing a photo and reading it

If your MAUI app used Scandit for live camera scanning but you are moving to a capture-and-process model (user takes a photo, app reads the barcode from the captured image), IronBarcode handles the processing step:

```csharp
// IronBarcode: MAUI photo capture + file processing
// NuGet: dotnet add package IronBarcode

using IronBarCode;

private async Task ScanFromCamera()
{
    var photo = await MediaPicker.CapturePhotoAsync();
    if (photo == null) return;

    using var stream = await photo.OpenReadAsync();
    var results = BarcodeReader.Read(stream);

    if (results.Any())
    {
        var first = results.First();
        await DisplayAlert("Barcode Found", $"{first.Value} ({first.Format})", "OK");
    }
}
```

This is not a real-time scanning flow — the user taps to capture, then sees the result. For live viewfinder scanning on mobile, Scandit or a dedicated MAUI camera scanning component remains the appropriate choice.

---

## API Mapping Reference

| Scandit SDK | IronBarcode | Notes |
|---|---|---|
| `DataCaptureContext.ForLicenseKey("key")` | `IronBarCode.License.LicenseKey = "key"` | One line, no context object |
| `BarcodeCaptureSettings.Create()` | `new BarcodeReaderOptions()` | Optional in IronBarcode |
| `settings.EnableSymbologies(Symbology.Code128, ...)` | (not needed) | Auto-detection is default |
| `Camera.GetDefaultCamera()` | (not applicable) | No camera in file processing |
| `dataCaptureContext.SetFrameSourceAsync(camera)` | (not applicable) | No frame source concept |
| `camera.SwitchToDesiredStateAsync(FrameSourceState.On)` | (not applicable) | No camera state machine |
| `barcodeCapture.IsEnabled = true` | `BarcodeReader.Read(path)` | Single call initiates reading |
| `args.Session.NewlyRecognizedBarcodes` | Return value of `BarcodeReader.Read()` | Synchronous collection |
| `barcode.Data` | `result.Value` | Same semantic content |
| `barcode.Symbology` | `result.Format` | Equivalent format enum |
| Camera frame event handling | Synchronous return value | No event system needed |
| Scandit product add-ons (MatrixScan, etc.) | Single `IronBarcode` package | No separate products |

---

## Migration Checklist

Use these search terms to find all Scandit integration points in your codebase:

- [ ] `DataCaptureContext.ForLicenseKey` — main initialization, replace with `IronBarCode.License.LicenseKey = "key"`
- [ ] `BarcodeCaptureSettings.Create` — settings object, replace with `new BarcodeReaderOptions()` if format/speed tuning is needed, or remove entirely
- [ ] `settings.EnableSymbologies` — symbology declarations, remove entirely (IronBarcode auto-detects)
- [ ] `Camera.GetDefaultCamera` — camera acquisition, remove entirely
- [ ] `SwitchToDesiredStateAsync` — camera state management, remove entirely
- [ ] `barcodeCapture.IsEnabled` — capture activation flag, replace with `BarcodeReader.Read(path/stream/bytes)`
- [ ] `BarcodeScanned +=` — event handler subscriptions, replace with direct iteration over `BarcodeReader.Read()` results
- [ ] `barcode.Data` — result value access, replace with `result.Value`
- [ ] `barcode.Symbology` — result format access, replace with `result.Format`
- [ ] `using Scandit.DataCapture.Core` — namespace import, remove
- [ ] `using Scandit.DataCapture.Barcode` — namespace import, remove
- [ ] Add `using IronBarCode;` at the top of each file that reads barcodes

---

The camera pipeline Scandit requires for every integration makes sense for its intended use case. It makes none for a server reading invoices. If your use case is the latter, the migration is straightforward: remove the camera infrastructure, pass the file path, read the results.
