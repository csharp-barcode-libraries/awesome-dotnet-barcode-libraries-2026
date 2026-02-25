# Getting Started with IronBarcode for .NET Developers Who Evaluated Barkoder

There is nothing to migrate from. Barkoder SDK has no .NET library, no NuGet package, and no C# API. If you're reading this, you probably spent time evaluating Barkoder — reading about MatrixSight, DeBlur mode, multi-barcode detection, DPM support — before discovering that none of it is accessible from a C# project.

That evaluation is not entirely wasted. It tells you what you need: damage-tolerant barcode reading, multi-barcode detection, support for high-density formats, and high accuracy on challenging input. This guide shows you how IronBarcode covers each of those requirements in .NET, with working code for each scenario.

---

## Why You're Reading This

Barkoder appears in barcode SDK comparison roundups alongside genuine .NET libraries. It has strong marketing around technical capabilities that solve real problems in logistics, manufacturing, and healthcare. A developer researching barcode solutions for a .NET application encounters it, reads the feature list, and finds it compelling — before running into the platform wall.

The platform list on Barkoder's website:

- iOS Native (Swift / Objective-C)
- Android Native (Kotlin / Java)
- React Native
- Flutter
- Cordova
- Capacitor

Absent from that list: .NET, C#, .NET MAUI, Xamarin, Windows, Linux, ASP.NET Core.

NuGet.org search for "barkoder": zero results.

That is the end of the Barkoder evaluation for a .NET developer. This guide picks up from there.

---

## Quick Setup

Install IronBarcode from NuGet:

```bash
dotnet add package IronBarcode
```

Add the license key at application startup — in `Program.cs` for ASP.NET Core applications, or before any barcode operations in console or desktop apps:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";
```

That's the complete setup. No API configuration, no HTTP client, no key management infrastructure. IronBarcode runs entirely locally — no network calls are made during barcode operations.

### Supported Platforms

IronBarcode runs on every .NET environment:

| Platform | Support |
|---|---|
| Windows | Full |
| Linux | Full |
| macOS | Full |
| Docker | Full |
| Azure Functions | Full |
| AWS Lambda | Full |
| ASP.NET Core | Full |
| .NET Framework 4.6.2+ | Full |
| .NET 6 / 7 / 8 / 9 | Full |
| .NET MAUI | Full |
| Air-gapped / offline | Full |

---

## Barkoder Use Cases, Covered in .NET

Each section below maps a Barkoder strength to the equivalent IronBarcode capability with working C# code.

### DeBlur Mode → `ReadingSpeed.ExtremeDetail`

Barkoder's DeBlur mode applies image preprocessing to recover barcode data from blurred, damaged, or poorly-captured images. This capability is heavily promoted for logistics workflows where physical labels get worn, wet, or partially obscured.

In IronBarcode, the equivalent is `ReadingSpeed.ExtremeDetail`. At this speed setting, IronBarcode runs a multi-pass image analysis pipeline — applying contrast enhancement, sharpening, rotation correction, and ML-based damage recovery — before attempting to decode.

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.ExtremeDetail,
    ExpectMultipleBarcodes = false,
};

var results = BarcodeReader.Read("worn-shipping-label.png", options);

if (results.Any())
{
    Console.WriteLine($"Decoded: {results.First().Value}");
    Console.WriteLine($"Format: {results.First().Format}");
}
else
{
    Console.WriteLine("Image quality too low for recovery");
}
```

**When to use ExtremeDetail:** Use it when `ReadingSpeed.Balanced` fails on a specific image. `Balanced` is faster and handles most clean or lightly damaged barcodes. `ExtremeDetail` is more thorough and significantly slower — reserve it for problem cases or batch jobs where time is not the constraint.

For a batch processing workflow that tries `Balanced` first and falls back to `ExtremeDetail` on failures:

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

string[] imagePaths = Directory.GetFiles("label-scans", "*.png");

var fastOptions = new BarcodeReaderOptions { Speed = ReadingSpeed.Balanced };
var deepOptions = new BarcodeReaderOptions { Speed = ReadingSpeed.ExtremeDetail };

foreach (var path in imagePaths)
{
    var results = BarcodeReader.Read(path, fastOptions);

    if (!results.Any())
    {
        // Retry with deep analysis
        results = BarcodeReader.Read(path, deepOptions);
    }

    if (results.Any())
    {
        Console.WriteLine($"{path}: {results.First().Value}");
    }
    else
    {
        Console.WriteLine($"{path}: Failed — flagging for manual review");
    }
}
```

---

### Multi-Barcode Detection → `ExpectMultipleBarcodes = true`

Barkoder's multi-barcode mode detects and decodes several barcodes simultaneously from a single camera frame. In a .NET context, the equivalent scenario is processing a document, image, or PDF page that contains multiple barcodes — a shipping manifest with a pallet barcode and multiple item barcodes, or an invoice with a PO number QR code and a product barcode.

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
};

// Works on images, byte arrays, streams, and PDFs
var results = BarcodeReader.Read("invoice-scan.png", options);

Console.WriteLine($"Found {results.Count()} barcodes:");
foreach (var barcode in results)
{
    Console.WriteLine($"  {barcode.Format}: {barcode.Value}");
}
```

For PDF documents with multiple barcodes across multiple pages — the kind of multi-page manifest that appears in logistics and supply chain workflows:

```csharp
using IronBarCode;

IronBarCode.License.LicenseItem = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
};

var results = BarcodeReader.Read("shipping-manifest.pdf", options);

// Group by page for reporting
var byPage = results.GroupBy(r => r.PageNumber);
foreach (var page in byPage)
{
    Console.WriteLine($"Page {page.Key}: {page.Count()} barcodes");
    foreach (var barcode in page)
    {
        Console.WriteLine($"  {barcode.Format}: {barcode.Value}");
    }
}
```

IronBarcode reads the PDF natively — no intermediate image extraction step, no multiple API calls, no per-page overhead.

---

### High-Accuracy Scanning → IronBarcode ML Correction

Barkoder promotes high-accuracy scanning as a core differentiator, particularly for the transportation, automotive, and healthcare verticals where a misread barcode has real consequences. The accuracy claim rests on MatrixSight's image processing pipeline.

IronBarcode includes ML-based image correction that runs automatically as part of the read pipeline. There is no accuracy mode to enable explicitly — IronBarcode applies correction during every read. At `ReadingSpeed.ExtremeDetail`, the correction passes are the most thorough.

For a high-accuracy read with explicit format targeting:

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// Targeting specific formats improves accuracy — IronBarcode doesn't need to
// test every format, reducing false positives
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.ExtremeDetail,
    ExpectMultipleBarcodes = false,
};

var results = BarcodeReader.Read("medical-specimen-label.png", options);

foreach (var result in results)
{
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Format: {result.Format}");
}
```

For server-side ASP.NET Core integration, reading from an uploaded file stream:

```csharp
using IronBarCode;

// In Program.cs
IronBarCode.License.LicenseKey = "YOUR-KEY";

// In a controller or minimal API endpoint
app.MapPost("/api/scan", async (IFormFile file) =>
{
    using var stream = file.OpenReadStream();
    var imageBytes = new byte[stream.Length];
    await stream.ReadAsync(imageBytes);

    var options = new BarcodeReaderOptions
    {
        Speed = ReadingSpeed.Balanced,
        ExpectMultipleBarcodes = true,
    };

    var results = BarcodeReader.Read(imageBytes, options);

    return Results.Ok(results.Select(r => new
    {
        Value = r.Value,
        Format = r.Format.ToString(),
    }));
});
```

---

### DPM DataMatrix → `BarcodeEncoding.DataMatrix`

Direct Part Marking (DPM) is a manufacturing process where barcodes are laser-etched, dot-peened, or chemically etched directly onto parts. DataMatrix is the most common format for DPM because it tolerates physical damage and low contrast. Barkoder promotes DPM support as a key capability.

IronBarcode reads and generates DataMatrix natively:

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// Generate a DataMatrix barcode for a part label
var partBarcode = BarcodeWriter.CreateBarcode(
    "PART-SN-20240315-A42291",
    BarcodeEncoding.DataMatrix
);
partBarcode.SaveAsPng("part-label.png");

// Read DataMatrix from a scan of an etched part
// ExtremeDetail handles the low contrast and rough edges of DPM
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.ExtremeDetail,
    ExpectMultipleBarcodes = false,
};

var results = BarcodeReader.Read("etched-part-photo.png", options);
foreach (var result in results)
{
    Console.WriteLine($"Part Serial: {result.Value}");
    Console.WriteLine($"Barcode Type: {result.Format}");
}
```

For an industrial tracking system that reads part serials and logs them:

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

public record PartScan(string SerialNumber, string BarcodeFormat, DateTime ScannedAt);

public static IEnumerable<PartScan> ScanPartBatch(string[] imagePaths)
{
    var options = new BarcodeReaderOptions
    {
        Speed = ReadingSpeed.ExtremeDetail,
        ExpectMultipleBarcodes = false,
    };

    var scans = new List<PartScan>();

    foreach (var path in imagePaths)
    {
        var results = BarcodeReader.Read(path, options);
        foreach (var result in results)
        {
            scans.Add(new PartScan(result.Value, result.Format.ToString(), DateTime.UtcNow));
        }
    }

    return scans;
}
```

---

## Feature Comparison

| Feature | Barkoder SDK | IronBarcode |
|---|---|---|
| **.NET / C# API** | None | Full |
| **NuGet Package** | None | `IronBarcode` |
| **Damaged Barcode Recovery** | MatrixSight / DeBlur | `ReadingSpeed.ExtremeDetail` + ML |
| **Multi-Barcode Detection** | Yes | `ExpectMultipleBarcodes = true` |
| **DataMatrix / DPM** | Yes (read only) | Yes — read and generate |
| **QR Code** | Yes (read only) | Yes — read and generate |
| **PDF417** | Yes | Yes |
| **Aztec** | Yes | Yes |
| **Code128, Code39, EAN** | Yes | Yes — 50+ formats |
| **Barcode Generation** | None | Full — all formats |
| **PDF Processing** | None | Native |
| **ASP.NET Core** | None | Full |
| **Docker / Linux** | None | Full |
| **Azure Functions** | None | Full |
| **AWS Lambda** | None | Full |
| **Offline / Air-Gapped** | On-device | Fully local |
| **Async / Await** | N/A | Full |
| **Dependency Injection** | None | Full .NET DI |
| **iOS via MAUI** | Native SDK | Via .NET MAUI |
| **Android via MAUI** | Native SDK | Via .NET MAUI |
| **Pricing** | Mobile SDK licensing | From $749 perpetual |

---

## What You Get That Barkoder Doesn't Have

### PDF Processing

Barkoder is a mobile camera scanning SDK. It has no concept of PDF documents. In .NET enterprise applications, barcodes in PDFs are common — invoices, shipping labels, compliance documents, medical records. IronBarcode reads them natively:

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// Entire PDF processed in one call — all pages, all barcodes
var results = BarcodeReader.Read("purchase-orders.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Value} ({barcode.Format})");
}
```

### Barcode Generation

Barkoder generates nothing — it is read-only by design (mobile cameras don't generate barcodes). IronBarcode covers the full workflow:

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// Code128 — standard linear barcode for logistics labels
BarcodeWriter.CreateBarcode("SHIP-2024031500428", BarcodeEncoding.Code128)
    .SaveAsPng("shipping-barcode.png");

// QR code with logo for marketing or packaging
QRCodeWriter.CreateQrCode("https://products.example.com/A42", 500)
    .AddBrandLogo("brand-logo.png")
    .SaveAsPng("product-qr.png");

// Get barcode as bytes for embedding in a document or API response
byte[] barcodeBytes = BarcodeWriter.CreateBarcode("INV-100291", BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

### Full .NET Ecosystem Integration

Barkoder's mobile SDK has no concept of dependency injection, middleware, or .NET hosting. IronBarcode integrates naturally into any .NET application architecture:

```csharp
// ASP.NET Core — register as a service
builder.Services.AddSingleton<IBarcodeProcessor, IronBarcodeProcessor>();

public class IronBarcodeProcessor : IBarcodeProcessor
{
    public IEnumerable<BarcodeResult> Read(Stream imageStream)
    {
        var options = new BarcodeReaderOptions
        {
            Speed = ReadingSpeed.Balanced,
            ExpectMultipleBarcodes = true,
        };
        return BarcodeReader.Read(imageStream, options);
    }

    public byte[] Generate(string data, BarcodeEncoding format)
    {
        return BarcodeWriter.CreateBarcode(data, format).ToPngBinaryData();
    }
}
```

### Pricing Transparency

Barkoder's pricing is mobile SDK licensing, which varies by application and distribution volume. IronBarcode pricing is flat and perpetual:

| Tier | Price | Developers |
|---|---|---|
| Lite | $749 one-time | 1 developer |
| Plus | $1,499 one-time | 3 developers |
| Professional | $2,999 one-time | 10 developers |
| Unlimited | $5,999 one-time | Unlimited |

No per-request charges. No monthly subscriptions. No volume limits. Process one barcode or a million — the license cost is the same.

---

## Next Steps

If you're ready to move forward with IronBarcode after a Barkoder evaluation dead-end:

1. `dotnet add package IronBarcode`
2. Set `IronBarCode.License.LicenseKey = "YOUR-KEY"` at startup
3. Start with `BarcodeReader.Read("file.png")` for reading
4. Use `BarcodeReaderOptions` with `ReadingSpeed.ExtremeDetail` for damaged or difficult images
5. Use `ExpectMultipleBarcodes = true` for documents with more than one barcode
6. Use `BarcodeWriter.CreateBarcode()` and `QRCodeWriter.CreateQrCode()` for generation

The capabilities that made Barkoder interesting — damage recovery, multi-barcode detection, high-density format support — are all here, implemented natively for .NET and available through a straightforward C# API.
