To start reading barcodes with Scandit, you configure a `DataCaptureContext`, create `BarcodeCaptureSettings`, enable symbologies explicitly, get a camera, set it as the frame source, switch the camera to the On state, and enable capture. The price is still not on the website.

That combination — mandatory camera pipeline and opaque enterprise pricing — defines exactly when Scandit is the wrong tool for a .NET project. If you are building a server-side document processing workflow, an ASP.NET Core API that reads barcodes from uploaded files, or an Azure Function that processes shipping PDFs, Scandit's architecture works against you at every step. This comparison examines where that architecture comes from, where IronBarcode fits instead, and what it costs to find out either way.

---

## Table of Contents

1. [The Camera Pipeline Problem](#the-camera-pipeline-problem)
2. [Pricing: Sales Calls vs Published Tiers](#pricing-sales-calls-vs-published-tiers)
3. [Platform and Deployment Support](#platform-and-deployment-support)
4. [Symbology Detection: Manual vs Automatic](#symbology-detection-manual-vs-automatic)
5. [Product Line Complexity](#product-line-complexity)
6. [Feature Comparison Table](#feature-comparison-table)
7. [Code Comparison](#code-comparison)
8. [When to Choose Each Library](#when-to-choose-each-library)
9. [Related Comparisons](#related-comparisons)

---

## The Camera Pipeline Problem

Scandit SDK was built for a specific and legitimate scenario: a mobile worker pointing a phone camera at a physical barcode and getting real-time visual feedback. Everything about the architecture serves that scenario. The `DataCaptureContext` manages the device's camera session. `BarcodeCaptureSettings` configures how each video frame is analyzed. The camera itself is treated as a first-class object — you get it, assign it as the frame source, switch it to an active state, and then flip a flag to start processing frames.

Here is what that initialization looks like in practice before a single barcode value is read:

```csharp
// Scandit SDK: full camera pipeline setup
// NuGet: Scandit.BarcodePicker

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
```

Every line in that block is camera infrastructure. There is no file path argument. There is no PDF. There is no stream. The library assumes a running camera before any barcode work can begin.

For file-based document processing, IronBarcode takes a different approach entirely:

```csharp
// IronBarcode: read a file
// NuGet: dotnet add package IronBarcode

IronBarCode.License.LicenseKey = "YOUR-KEY";
var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
    Console.WriteLine($"{result.Value} ({result.Format})");
```

That is the complete program. No camera. No pipeline setup. No `async` state machine to initialize a frame source. The input is a file path, a stream, a byte array, or a PDF — whichever form your barcode data arrives in.

The distinction matters because it is not a surface-level API preference. It reflects a fundamentally different intended deployment. Scandit ships into mobile apps with camera hardware and live users. IronBarcode ships into servers, scheduled jobs, and document workflows where there is no camera, no user, and no UI.

---

## Pricing: Sales Calls vs Published Tiers

Scandit's pricing page does not list prices. It lists products — SparkScan, MatrixScan, ID Scanning, AR Overlays, Parser — and routes every inquiry through a sales conversation. The quote process involves explaining your use case, estimating scan volume, specifying devices, discussing support tiers, and negotiating contract length before a number appears. Reviews on G2 and DiscoverSDK describe this consistently: "costs should not be underestimated for small and medium-sized enterprises," "challenges for predictable budgeting," "higher costs for extensive scanning needs." None of those reviewers are wrong. They just found out after the sales call.

IronBarcode [publishes its pricing](https://ironsoftware.com/csharp/barcode/licensing/) on the product page with no form required:

| License | Price | Developers | Projects |
|---|---|---|---|
| Lite | $749 one-time | 1 | 1 |
| Professional | $1,499 one-time | 10 | 10 |
| Unlimited | $2,999 one-time | Unlimited | Unlimited |

There are no per-scan fees. No per-device fees. No volume thresholds that trigger tier changes. Process one barcode or one billion — same price. Annual renewal is optional at half the original cost, but the license you purchased is perpetual whether you renew or not.

The budget impact of the contact-sales model is not just the potential cost difference. It is the timeline. A developer evaluating Scandit for a new project cannot put a number in a budget request, a sprint plan, or a build-vs-buy analysis without first entering a sales cycle that may take days or weeks. With IronBarcode, the number is on the page.

---

## Platform and Deployment Support

Scandit's architecture requires camera hardware and a mobile platform that can expose it. That constrains where the SDK can run.

| Platform | Scandit | IronBarcode |
|---|---|---|
| iOS / Android (MAUI) | Primary target | Programmatic use (no camera UI) |
| ASP.NET Core | Not designed for | Full support |
| Azure Functions | Not practical | Full support |
| Docker / Linux server | Not supported | Full support |
| Console application | Not designed for | Full support |
| Blazor Server | Not designed for | Full support |
| Windows Desktop (WPF / WinForms) | Secondary | Full support |
| Offline file processing | No | Yes |

The "not practical" designation for Azure Functions is not a configuration problem — it is an architecture problem. Azure Functions are stateless compute units with no persistent camera session, no hardware to attach, and no camera preview surface. Scandit's pipeline assumes all of those things exist. Attempting to use Scandit in serverless or containerized environments means working around the entire foundation of the library.

IronBarcode was built for those environments. Deploying to [Azure Functions for barcode processing](https://ironsoftware.com/csharp/barcode/get-started/azure/) is a supported, documented path. There is nothing to work around.

---

## Symbology Detection: Manual vs Automatic

Before Scandit reads a single barcode, you must tell it which symbologies to look for:

```csharp
settings.EnableSymbologies(new HashSet<Symbology>
{
    Symbology.Ean13Upca,
    Symbology.Ean8,
    Symbology.Upce,
    Symbology.Code128,
    Symbology.Code39,
    Symbology.QrCode,
    Symbology.DataMatrix
});
```

This design makes sense for real-time camera scanning where filtering symbologies reduces processing overhead per frame. If you only scan EAN-13 barcodes in a retail checkout app, there is no reason to run QR code detection on every camera frame.

For document processing, the same constraint becomes a liability. Incoming PDFs from suppliers, logistics partners, or customers may carry Code 128 labels, QR codes, DataMatrix, Aztec, or Interleaved 2-of-5. The format is not always known in advance. With Scandit's model, you either enumerate every possible format (removing the performance benefit) or write format-detection logic on top of the library.

IronBarcode detects all supported formats automatically. [Reading barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) requires no format specification — the library tries all formats and returns whatever it finds. If you have a use case where you know the format and want to restrict detection for performance, `BarcodeReaderOptions` accepts explicit format constraints. But that is an optimization, not a requirement to get started.

---

## Product Line Complexity

Scandit is not one product. It is a platform of individually priced features:

- **SparkScan** — consumer scanning UX framework
- **MatrixScan** — multi-barcode simultaneous detection with AR
- **ID Scanning** — identity document and passport scanning
- **AR Overlays** — augmented reality product information display
- **Parser** — structured barcode data parsing (GS1, HIBC, etc.)

Each of these is a separate product. Capability access depends on which products are included in your contract. If you start with SparkScan and later need MatrixScan capabilities, that requires a new quote and a contract amendment. Building on this platform means your feature roadmap is gated by sales conversations, not just engineering effort.

IronBarcode is a single package. Multi-barcode reading, PDF support, image processing, barcode generation, format auto-detection — all of it ships in the same NuGet package at the same price tier. There is no upsell path because there is no separate product to upsell.

---

## Feature Comparison Table

| Feature | Scandit | IronBarcode |
|---|---|---|
| Camera live scanning | Yes (primary) | Not applicable |
| File / image reading | Not designed for | Primary focus |
| PDF barcode extraction | No | Yes (native) |
| Automatic format detection | No (must specify) | Yes |
| Server-side processing | No | Yes |
| ASP.NET Core support | No | Yes |
| Azure Functions / serverless | No | Yes |
| Docker / Linux deployment | No | Yes |
| AR overlay support | Yes | No |
| Multi-barcode simultaneous scan | MatrixScan (separate product) | Yes (single package) |
| Damaged barcode recovery | Limited | Yes (ML-powered) |
| Barcode generation | No | Yes |
| Published pricing | No (sales quote) | Yes |
| Perpetual license | No (annual) | Yes |
| Per-scan / per-device fees | Yes | No |
| 1D formats (Code 128, EAN, etc.) | 30+ | 30+ |
| 2D formats (QR, DataMatrix, etc.) | Yes | Yes |
| PDF417, Aztec | Yes | Yes |
| Offline processing | Yes | Yes |

---

## Code Comparison

### Scenario: Read a Barcode from an Uploaded Image

With Scandit, this scenario does not map cleanly to the library's design. Scandit processes camera frames in real time. Reading a static image file requires adapting the camera pipeline to treat a file as a frame source, which is not a standard supported workflow.

With IronBarcode:

```csharp
// IronBarcode: ASP.NET Core endpoint reading an uploaded image
// NuGet: dotnet add package IronBarcode

[HttpPost("scan")]
public IActionResult ScanUploadedFile(IFormFile file)
{
    using var stream = file.OpenReadStream();
    var results = BarcodeReader.Read(stream);

    if (!results.Any())
        return NotFound("No barcode found");

    return Ok(results.First().Value);
}
```

### Scenario: Extract All Barcodes from a Multi-Page PDF

Scandit has no native PDF support. Implementing this requires external PDF rendering to extract page images, then feeding those images through the camera pipeline simulation — significant engineering overhead for a common document processing task.

IronBarcode [reads barcodes from PDFs natively](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/), including multi-page documents:

```csharp
// IronBarcode: extract barcodes from every page of a PDF
var results = BarcodeReader.Read("shipping-manifest.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Value} ({barcode.Format})");
}
```

### Scenario: High-Throughput Batch Processing

Scandit's camera pipeline is not designed for batch file processing. The `FrameSourceState` model assumes a persistent camera session, not a queue of documents.

IronBarcode supports concurrent processing across multiple threads for server throughput scenarios:

```csharp
// IronBarcode: concurrent batch processing
// NuGet: dotnet add package IronBarcode

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};

var files = Directory.GetFiles("./incoming/", "*.pdf");
var allResults = new ConcurrentBag<BarcodeResult>();

Parallel.ForEach(files, file =>
{
    var results = BarcodeReader.Read(file, options);
    foreach (var r in results)
        allResults.Add(r);
});
```

For more on [async and multithreaded barcode reading](https://ironsoftware.com/csharp/barcode/how-to/async-multithread/) with IronBarcode, the documentation covers thread-safe patterns and throughput tuning.

---

## When to Choose Each Library

**Choose Scandit when:**

- You are building a mobile app where a user points a device camera at a physical barcode and needs sub-100ms feedback
- You need AR overlay experiences — product information displayed over the camera view, multi-item scanning with visual highlights
- You are deploying mobile field workers at scale (hundreds or thousands of devices) and need device management, analytics, and enterprise support SLAs
- Your use case is genuinely real-time, camera-based, and mobile-first — and your budget process can accommodate sales-driven pricing

**Choose IronBarcode when:**

- You are processing barcodes from files — images, PDFs, scanned documents, or byte arrays from any source
- You are building a server-side API, background service, scheduled job, or Azure Function that reads barcodes without a user present
- You need PDF barcode extraction without an additional library or license
- Predictable, published pricing matters for budget planning
- You need to deploy on Linux, in Docker containers, or in cloud functions
- You want automatic format detection without pre-specifying symbologies for every read call

**Consider both when:**

Some organizations genuinely need both. Mobile warehouse workers scan physical items in real time — that is Scandit's domain. The same organization's back-office system processes incoming shipping PDFs and generates barcode labels — that is IronBarcode's domain. These tools solve different problems and can coexist without conflict.
