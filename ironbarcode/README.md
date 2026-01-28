# IronBarcode for .NET: The Complete Developer Guide (2026)

IronBarcode is a commercial barcode library for .NET that delivers accurate barcode generation and reading with minimal configuration. Designed for C# developers who need production-ready barcode processing without the complexity of configuring ZXing, managing cloud API credentials, or building preprocessing pipelines, IronBarcode .NET offers a "fire and forget" approach to barcode operations.

## Why Developers Choose IronBarcode

### The Core Value Proposition

Barcode processing in .NET has historically required trade-offs. Developers face a choice between:

1. **Raw ZXing.Net** - Popular but requires manual format specification, no PDF support, and poor handling of damaged barcodes
2. **Cloud APIs** - Simple but introduce latency, per-transaction costs, and data security concerns
3. **Enterprise SDKs** - Comprehensive but complex, expensive, and often require license servers or multiple components

**IronBarcode C# occupies a unique position:** the accuracy and on-premise security of enterprise solutions with the simplicity of a single NuGet package. One line to read any barcode, one line to write any barcode.

### What Sets IronBarcode Apart

| Challenge | Traditional Solutions | IronBarcode Approach |
|-----------|----------------------|----------------------|
| Format detection | Manual specification required | Automatic (50+ formats) |
| PDF support | External libraries needed | Native, built-in |
| Damaged barcodes | Manual preprocessing | ML-powered correction |
| Deployment | Multiple components | One NuGet package |
| Threading | Manual thread safety | Built-in parallelization |
| Cloud dependency | Required for some | None |
| Pricing | Per-transaction or subscription | Perpetual option available |

## Quick Start

### Installation

```bash
dotnet add package IronBarcode
```

### Basic Usage

```csharp
using IronBarCode;

// One line barcode generation
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128).SaveAsPng("barcode.png");

// One line barcode reading with automatic format detection
var result = BarcodeReader.Read("barcode.png").First();
Console.WriteLine(result.Value);
```

### PDF Processing

```csharp
// Read barcodes from PDF - all pages automatically processed
var results = BarcodeReader.Read("invoices.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.Value} ({barcode.Format})");
}

// Specific pages only
var pageResults = BarcodeReader.Read("document.pdf", pages: new[] { 1, 5, 10 });
```

See complete examples in the code files:
- [One-Line Generation](ironbarcode-one-line-generation.cs) - Basic generation and reading patterns
- [Multi-Barcode Reading](ironbarcode-multi-read.cs) - Detecting multiple barcodes in one image
- [PDF Batch Processing](ironbarcode-pdf-batch.cs) - Document workflow automation
- [Styled QR Codes](ironbarcode-styled-qr.cs) - Colors, logos, and customization
- [Error Correction](ironbarcode-error-correction.cs) - ML-powered damaged barcode handling
- [Format Detection](ironbarcode-format-detection.cs) - Automatic symbology detection

## Core Features

### Automatic Format Detection

Read any barcode without specifying its type:

```csharp
// No need to know what type of barcode it is
var result = BarcodeReader.Read("unknown-barcode.png").First();

Console.WriteLine($"Value: {result.Value}");
Console.WriteLine($"Format: {result.Format}");  // IronBarcode tells you
Console.WriteLine($"Confidence: {result.Confidence}%");
```

IronBarcode automatically detects and decodes from 50+ symbologies including Code128, QR, EAN-13, UPC-A, PDF417, DataMatrix, Aztec, and many more.

### Multi-Format Support

Support for 50+ barcode symbologies across 1D and 2D formats:

**1D Linear Barcodes:**
- Code128, Code39, Code93
- EAN-13, EAN-8, UPC-A, UPC-E
- ITF, Codabar, MSI
- Pharmacode, PZN, ISBN, ISSN

**2D Matrix Barcodes:**
- QR Code (all versions)
- DataMatrix (ECC 200)
- PDF417, MicroPDF417
- Aztec Code
- MaxiCode

### Native PDF Support

Read barcodes directly from PDF documents without external libraries:

```csharp
// Read all pages
var results = BarcodeReader.Read("multi-page-document.pdf");

// Read specific pages (memory efficient for large documents)
var results = BarcodeReader.Read("large-document.pdf", pages: new[] { 1, 2, 3 });

// Generate barcode into PDF
BarcodeWriter.CreateBarcode("INV-2026-001", BarcodeEncoding.Code128)
    .SaveAsPdf("label.pdf");
```

### ML-Powered Error Correction

Handle damaged, partial, or low-quality barcodes automatically:

```csharp
// ML correction happens automatically - no configuration needed
var result = BarcodeReader.Read("damaged-barcode.png").First();

// Even scratched, faded, or partially obscured barcodes
var partialResult = BarcodeReader.Read("partial-scan.jpg").First();

// Confidence score indicates reconstruction certainty
Console.WriteLine($"Value: {result.Value}");
Console.WriteLine($"Confidence: {result.Confidence}%");
```

IronBarcode's machine learning engine automatically corrects for common real-world issues: scratches, fading, rotation, skew, blur, and partial damage.

### Styled Barcodes

Create visually customized barcodes and QR codes:

```csharp
// QR code with custom colors
var qr = QRCodeWriter.CreateQrCode("https://ironsoftware.com");
qr.ChangeBarCodeColor(Color.Navy);
qr.ChangeBackgroundColor(Color.WhiteSmoke);
qr.SaveAsPng("styled-qr.png");

// QR code with embedded logo
qr.AddBrandLogo("company-logo.png");
qr.SaveAsPng("branded-qr.png");

// Custom sizing
var barcode = BarcodeWriter.CreateBarcode("PRODUCT-123", BarcodeEncoding.Code128);
barcode.ResizeTo(400, 150);
barcode.SetMargins(20);
barcode.SaveAsPng("sized-barcode.png");
```

## Input Sources

IronBarcode accepts input from any source:

```csharp
// File path
var result = BarcodeReader.Read("barcode.png");

// Byte array
byte[] imageBytes = GetImageBytes();
var result = BarcodeReader.Read(imageBytes);

// Stream
using var stream = File.OpenRead("barcode.png");
var result = BarcodeReader.Read(stream);

// URL
var result = BarcodeReader.Read(new Uri("https://example.com/barcode.png"));

// System.Drawing.Image
var image = Image.FromFile("barcode.png");
var result = BarcodeReader.Read(image);

// PDF (native)
var result = BarcodeReader.Read("document.pdf");

// Multi-page documents (TIFF)
var result = BarcodeReader.Read("multipage.tiff");
```

### Supported Image Formats

| Format | Read | Write |
|--------|------|-------|
| PNG | Yes | Yes |
| JPG/JPEG | Yes | Yes |
| BMP | Yes | Yes |
| GIF | Yes | Yes |
| TIFF | Yes | Yes |
| PDF | Yes | Yes |

## Output Options

### Image Files

```csharp
var barcode = BarcodeWriter.CreateBarcode("DATA", BarcodeEncoding.QRCode);

barcode.SaveAsPng("barcode.png");
barcode.SaveAsJpeg("barcode.jpg");
barcode.SaveAsBmp("barcode.bmp");
barcode.SaveAsGif("barcode.gif");
barcode.SaveAsTiff("barcode.tiff");
```

### PDF Output

```csharp
// Standalone barcode PDF
barcode.SaveAsPdf("barcode.pdf");

// Embedded in existing PDF (stamps barcode onto document)
barcode.StampToExistingPdf("invoice.pdf", x: 50, y: 800, pageIndex: 0);
```

### Vector and Web Formats

```csharp
// SVG for web/print
barcode.SaveAsSvg("barcode.svg");

// HTML image tag with embedded base64
string html = barcode.ToHtmlTag();

// Data URI for inline embedding
string dataUri = barcode.ToDataUrl();
```

### Binary Data

```csharp
// Byte array
byte[] pngBytes = barcode.ToPngBinaryData();
byte[] jpegBytes = barcode.ToJpegBinaryData();

// Stream
using var stream = barcode.ToStream();
```

## Batch Processing

### Sequential Processing

```csharp
var files = Directory.GetFiles("warehouse-scans", "*.jpg");

foreach (var file in files)
{
    var results = BarcodeReader.Read(file);
    foreach (var barcode in results)
    {
        Console.WriteLine($"{Path.GetFileName(file)}: {barcode.Value}");
    }
}
```

### Parallel Processing

IronBarcode is thread-safe by design:

```csharp
var files = Directory.GetFiles("batch-documents", "*.pdf");

Parallel.ForEach(files, file =>
{
    var results = BarcodeReader.Read(file);
    ProcessResults(file, results);
});
```

### Multi-Document Workflows

```csharp
// Process folder of invoices, extract all barcodes to CSV
var outputLines = new List<string> { "File,Barcode,Format,Confidence" };

foreach (var pdf in Directory.GetFiles("invoices", "*.pdf"))
{
    var results = BarcodeReader.Read(pdf);
    foreach (var barcode in results)
    {
        outputLines.Add($"{Path.GetFileName(pdf)},{barcode.Value},{barcode.Format},{barcode.Confidence}");
    }
}

File.WriteAllLines("barcode-inventory.csv", outputLines);
```

## Configuration Options

### Format Specification

When you know the barcode type, specify it for faster processing:

```csharp
var result = BarcodeReader.Read("product.png", BarcodeEncoding.EAN13);
```

### Quality Settings

```csharp
// Higher quality output
var barcode = BarcodeWriter.CreateBarcode("DATA", BarcodeEncoding.QRCode);
barcode.SetMargins(10);
barcode.ResizeTo(500, 500);

// Error correction level for QR codes
var qr = QRCodeWriter.CreateQrCode("data", 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);
```

### Read Options

```csharp
// Speed vs accuracy trade-off
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,  // Faster, Balanced, Detailed, ExtremeDetail
    ExpectMultipleBarcodes = true,
    ExpectBarcodeTypes = BarcodeEncoding.AllOneDimensional,
    CropArea = new Rectangle(0, 0, 200, 200),  // Only scan this region
    MaxParallelThreads = 4
};

var result = BarcodeReader.Read("image.png", options);
```

## Deployment

### Single NuGet Package

IronBarcode deploys as a single NuGet package with all dependencies bundled:

```xml
<PackageReference Include="IronBarcode" Version="2024.x.x" />
```

No additional files, native libraries, or configuration required.

### Platform Support

| Platform | Status |
|----------|--------|
| Windows x64 | Fully supported |
| Windows x86 | Fully supported |
| Linux x64 | Fully supported |
| macOS x64 | Fully supported |
| macOS ARM (M1/M2) | Fully supported |
| Azure App Service | Fully supported |
| AWS Lambda | Fully supported |
| Docker | Works out of box |
| .NET Framework 4.6.2+ | Supported |
| .NET Core 3.1+ | Supported |
| .NET 5/6/7/8 | Supported |

### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# libgdiplus for System.Drawing on Linux
RUN apt-get update && apt-get install -y libgdiplus

COPY --from=build /app/publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "YourApp.dll"]
```

### License Configuration

```csharp
// At application startup
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";

// Or from environment variable (recommended for production)
IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");

// Or from configuration file
IronBarCode.License.LicenseKey = Configuration["IronBarcode:LicenseKey"];
```

## Security Advantages

### Complete Data Sovereignty

IronBarcode processes all barcodes locally:

- **No cloud transmission** - Barcodes and documents never leave your infrastructure
- **No internet required** - Works in air-gapped environments
- **No third-party access** - Your data stays under your control
- **Simple compliance** - No external processors to audit

### Compliance Benefits

| Requirement | How IronBarcode Helps |
|-------------|----------------------|
| HIPAA | Patient data never leaves your environment |
| GDPR | No data transfer to third parties |
| ITAR | Controlled data stays on-premise |
| CMMC | Simpler audit scope |
| FedRAMP | No cloud dependency |
| Air-gapped | Full offline operation |

Unlike cloud barcode APIs that transmit your images and data to external servers, IronBarcode runs entirely within your application, making compliance straightforward.

## Licensing

### License Tiers

| License | Price | Developers | Projects |
|---------|-------|------------|----------|
| Lite | $749 | 1 | 1 |
| Plus | $1,499 | 3 | 3 |
| Professional | $2,999 | 10 | 10 |
| Unlimited | $5,999 | Unlimited | Unlimited |

### What's Included

- Perpetual license (use forever)
- One year of updates and support
- All platforms (Windows, Linux, macOS, cloud)
- All features (generation, reading, PDF, styling)
- Email support

### No Per-Transaction Costs

Unlike cloud barcode services, IronBarcode licensing means:

- Process unlimited barcodes
- No metering or usage tracking
- Predictable budget
- No cost surprises at scale

A single $749 Lite license processes the same barcodes that would cost $10,000+ annually with cloud APIs at typical volumes.

## Performance

### Benchmarks

| Scenario | Typical Performance |
|----------|---------------------|
| Single barcode read | 10-50ms |
| QR code generation | 5-20ms |
| PDF page scan | 50-150ms per page |
| Batch (100 images) | 3-8 seconds |
| Damaged barcode (with ML) | 30-100ms |

### Optimization Tips

1. **Specify format when known** - Reduces scan time by skipping unnecessary format checks
2. **Use appropriate resolution** - 300 DPI is optimal; higher wastes processing time
3. **Batch operations** - Process multiple files in single operations when possible
4. **Parallel processing** - IronBarcode is thread-safe; use `Parallel.ForEach` for large batches
5. **Crop regions** - If barcode location is known, specify `CropArea` to scan only that region

## Comparison Summary

### vs. Open Source (ZXing.Net)

| Aspect | ZXing.Net | IronBarcode |
|--------|-----------|-------------|
| Format detection | Manual specification | Automatic |
| PDF support | None (external lib) | Native |
| Damaged barcodes | Poor handling | ML correction |
| Commercial support | None | Included |

### vs. Cloud APIs

| Aspect | Cloud APIs | IronBarcode |
|--------|------------|-------------|
| Data location | Their servers | Your infrastructure |
| Internet required | Yes | No |
| Per-barcode cost | Yes ($0.001-0.01) | No |
| Latency | 100-500ms | 10-50ms |
| Air-gapped | Impossible | Fully supported |

### vs. Enterprise SDKs (Aspose, LEADTOOLS)

| Aspect | Enterprise SDKs | IronBarcode |
|--------|-----------------|-------------|
| Licensing | Annual subscription | Perpetual option |
| Setup complexity | High (license servers, SDK install) | Single NuGet |
| Typical 5-year cost | $15,000-50,000+ | $749-5,999 |
| ML error correction | Manual tuning | Automatic |

## Getting Started

### 1. Install

```bash
dotnet add package IronBarcode
```

### 2. Add License (optional for development)

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";
```

### 3. Generate

```csharp
BarcodeWriter.CreateBarcode("Hello World", BarcodeEncoding.QRCode).SaveAsPng("qr.png");
```

### 4. Read

```csharp
var value = BarcodeReader.Read("qr.png").First().Value;
```

## Support Resources

- **Documentation:** https://ironsoftware.com/csharp/barcode/docs/
- **Tutorials:** https://ironsoftware.com/csharp/barcode/tutorials/
- **API Reference:** https://ironsoftware.com/csharp/barcode/object-reference/
- **Support:** support@ironsoftware.com
- **NuGet:** https://www.nuget.org/packages/BarCode/

---

*Last verified: January 2026*
