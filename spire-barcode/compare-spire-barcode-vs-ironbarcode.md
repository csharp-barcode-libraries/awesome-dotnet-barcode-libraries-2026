Spire.Barcode requires the barcode type when reading: `scanner.Scan(path, BarCodeType.Code128)`. If you're processing documents where incoming barcodes could be any format, you write a format-detection loop before you can extract a value. That single API decision shapes everything about how you build barcode scanning workflows with Spire — and it's worth understanding before you commit to it.

This article compares Spire.Barcode and IronBarcode across the areas that matter in production: API design, free tier honesty, PDF support, generation ergonomics, and pricing.

---

## The Type Specification Problem

Spire.Barcode's `BarcodeScanner.Scan()` requires a `BarCodeType` parameter. There is no call signature that accepts only a file path. Every read operation demands that you know the format in advance:

```csharp
// Spire.Barcode — type is mandatory
BarcodeScanner scanner = new BarcodeScanner();
string[] results = scanner.Scan("barcode.png", BarCodeType.Code128);
```

This is fine when your application exclusively processes a single known format — a warehouse system that only scans Code128 labels, for example. But the moment your documents contain mixed formats, or you can't guarantee what format a user will upload, the API turns against you. The workaround is to iterate through candidate types until one returns results:

```csharp
// Spire.Barcode: multi-format detection requires a guessing loop
BarcodeScanner scanner = new BarcodeScanner();
var candidates = new[] { BarCodeType.Code128, BarCodeType.QRCode, BarCodeType.DataMatrix, BarCodeType.EAN13, BarCodeType.PDF417 };

string foundValue = null;
foreach (var type in candidates)
{
    string[] found = scanner.Scan("barcode.png", type);
    if (found.Length > 0)
    {
        foundValue = found[0];
        break;
    }
}
```

Every type you add to that list is an extra scan pass. If the format isn't in your list, the barcode is silently missed. You're now responsible for maintaining an exhaustive type inventory and re-testing whenever a new format enters your workflow.

IronBarcode takes the opposite approach. [Reading barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) requires no type parameter — the library detects the format automatically across all 50+ supported symbologies:

```csharp
// IronBarcode — auto-detection built in
var results = BarcodeReader.Read("barcode.png");

foreach (var result in results)
{
    Console.WriteLine($"{result.BarcodeType}: {result.Value}");
}
```

The result objects include the detected type, so you can route values downstream based on what was actually found rather than what you assumed would be there.

---

## FreeSpire.Barcode: What the Free Tier Actually Gives You

E-iceblue publishes two packages: `FreeSpire.Barcode` (no cost) and `Spire.Barcode` (commercial). The free version is not a time-limited trial with full capabilities — it is a permanently capped product with intentional limitations baked in.

Generated barcodes in FreeSpire.Barcode carry an evaluation watermark that covers the image. The watermark is not a subtle corner stamp; it renders the barcode unsuitable for any real use. You cannot test your generation output meaningfully in the free tier because what you see is not what you'd ship.

Scan performance is intentionally degraded in the free version. E-iceblue is explicit about this: batch processing becomes slow enough that you can't benchmark real-world throughput during evaluation. What you measure with FreeSpire.Barcode is not representative of commercial Spire.Barcode.

The free version also requires a registration key obtained from E-iceblue's website. Without it, warning dialogs appear during execution. So the "free" experience involves registration, degraded performance, watermarked output, and limited symbology support.

IronBarcode's trial runs from the same package as the licensed product. You get full speed, full symbology support, and a small edge watermark on generated images. The [reading speed and accuracy options](https://ironsoftware.com/csharp/barcode/how-to/reading-speed-options/) behave identically in trial and licensed mode — what you benchmark during evaluation is what you deploy.

| Aspect | FreeSpire.Barcode | IronBarcode Trial |
|---|---|---|
| Watermarks | Large, covers barcode | Small, edge of image |
| Performance | Intentionally degraded | Full speed |
| Symbology support | ~20 types | 50+ types |
| Registration required | Yes (free key from E-iceblue) | No |
| Features available | Limited subset | Full feature set |
| Time limit | None | 30 days |
| Support | None | Available |

---

## PDF Support: One Library or Two?

Spire.Barcode does not read barcodes directly from PDF files. To extract barcodes from a PDF, you need to also purchase and install `Spire.PDF`, extract images from each page manually, and then pass those images to the barcode scanner:

```csharp
// Spire.Barcode + Spire.PDF: two libraries, two licenses
using Spire.Pdf;

var pdf = new PdfDocument();
pdf.LoadFromFile("document.pdf");

var scanner = new BarcodeScanner();
foreach (PdfPageBase page in pdf.Pages)
{
    var images = page.ExtractImages();
    foreach (var image in images)
    {
        string[] results = scanner.Scan(image, BarCodeType.QRCode); // still need the type
    }
}
```

This means two separate NuGet packages, two license purchases, and code that manually manages PDF page iteration and image extraction before barcode reading even begins. If your barcodes are embedded in vector content rather than raster images, extraction becomes unreliable.

IronBarcode handles PDFs natively without any additional dependency. [Reading barcodes from PDF documents](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) is a single method call with page numbers included in the results:

```csharp
// IronBarcode: native PDF support, no additional library
var results = BarcodeReader.Read("document.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.BarcodeType} = {barcode.Value}");
}
```

---

## Generation: BarcodeSettings Object vs Fluent API

Spire.Barcode's generation model centers on a `BarcodeSettings` configuration object. You instantiate it, set properties, pass it to a `BarCodeGenerator`, and call `GenerateImage()`:

```csharp
// Spire.Barcode generation
using Spire.Barcode;

BarcodeSettings settings = new BarcodeSettings();
settings.Type = BarCodeType.Code128;
settings.Data = "12345678";
settings.ShowText = true;
settings.TextMargin = 5;
settings.BarHeight = 60;
settings.Unit = GraphicsUnit.Pixel;

BarCodeGenerator generator = new BarCodeGenerator(settings);
Image barcodeImage = generator.GenerateImage();
barcodeImage.Save("barcode.png", ImageFormat.Png);
```

That is eight lines of boilerplate before any file is written. The settings object is mutable, which means configurations can be accidentally shared across calls in scenarios where a single instance is reused.

IronBarcode uses a static factory method with optional fluent chaining:

```csharp
// IronBarcode generation
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");

// With sizing:
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("barcode.png");
```

QR codes with custom logos — a commercial-only feature in Spire.Barcode — are available at all IronBarcode license tiers:

```csharp
var qr = QRCodeWriter.CreateQrCode("https://example.com", 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);
qr.AddBrandLogo("logo.png");
qr.SaveAsPng("qr-with-logo.png");
```

---

## Feature Comparison

| Feature | Spire.Barcode | IronBarcode |
|---|---|---|
| Barcode generation | Yes | Yes |
| Barcode reading | Yes (type required) | Yes (auto-detect) |
| Symbology count | 39+ (commercial) | 50+ |
| Auto format detection | No (requires BarCodeType) | Yes |
| Free tier | FreeSpire.Barcode (limited) | Trial mode (full features) |
| Watermarks in free tier | Yes, intrusive | Yes, minimal |
| PDF reading | Via Spire.PDF (separate purchase) | Built-in, no extra package |
| QR with logo | Commercial only | All tiers |
| ML error correction | No | Yes |
| Batch image processing | Manual loop | Native array/list input |
| Fluent generation API | No | Yes |
| License model | Per-seat tiers + subscription | Perpetual with optional support |

---

## Pricing

Spire.Barcode's pricing structure has four tiers:

| License | Price |
|---|---|
| Single Developer | $349 |
| Site License | $1,398 |
| OEM | $6,990 |
| Subscription | $999/year |

The perpetual tiers (Single Developer, Site, OEM) typically cover a specific version. Updates and continued support require an active subscription on top. The OEM tier at $6,990 covers redistribution rights. If your team grows beyond one developer, you jump directly to the $1,398 Site license.

[IronBarcode's licensing](https://ironsoftware.com/csharp/barcode/licensing/) uses perpetual tiers:

| License | Price | Developers |
|---|---|---|
| Lite | $749 | 1 |
| Professional | $1,499 | 10 |
| Unlimited | $2,999 | Unlimited |

All tiers are perpetual with the option to add an annual support and update subscription. A five-developer team on Spire.Barcode's Site license ($1,398) plus a year of subscription ($999) runs $2,397 in year one — comparable to IronBarcode Professional at $1,499 once, with no year-two cost unless you opt into support renewal.

---

## When to Consider Spire.Barcode

Spire.Barcode makes sense when your application exclusively processes a single, known barcode format and you're already using other E-iceblue products. If every scan call will always pass `BarCodeType.Code128` because your workflow guarantees it, the mandatory type parameter is not a burden. Teams invested in the E-iceblue ecosystem may also benefit from API familiarity and potential bundle pricing across Spire.Doc, Spire.XLS, and Spire.PDF.

## When IronBarcode Is the Better Fit

If your application processes documents where the barcode format isn't guaranteed — shipping labels with mixed Code128 and QR, uploaded images from customers using different scanners, PDFs that arrived from external systems — the mandatory `BarCodeType` parameter in Spire.Barcode creates ongoing maintenance work. Every time a new format enters your workflow, you update the type loop. IronBarcode's auto-detection handles that invisibly.

PDF barcode extraction without a second library purchase is a meaningful simplification for document-heavy workflows. The honest trial experience — full speed, full symbologies, minimal watermark — also makes evaluation more reliable. What you test is what you ship.
