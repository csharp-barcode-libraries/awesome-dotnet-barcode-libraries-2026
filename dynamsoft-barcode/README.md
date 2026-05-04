# Dynamsoft Barcode Reader for C# and .NET: The Complete 2026 Developer's Guide

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

Dynamsoft Barcode Reader is a high-performance commercial barcode SDK known for exceptional decoding speed and broad platform reach — the .NET edition runs on Windows and Linux servers as well as inside MAUI mobile apps. When C# developers need to scan barcodes from live camera feeds, Dynamsoft consistently delivers impressive frame-by-frame decoding performance. Having tested numerous barcode solutions over the past decade, I can confirm that Dynamsoft C# real-time scanning capabilities are genuinely excellent. However, the .NET ecosystem has varied needs—mobile camera scanning represents just one use case. For server-side document processing, PDF batch operations, and simpler deployment scenarios in C# applications, IronBarcode offers a different approach optimized for those workflows. This guide provides an honest comparison to help you choose the right tool for your specific .NET requirements.

## Table of Contents

1. [Understanding the Barcode SDK Ecosystem](#understanding-the-barcode-sdk-ecosystem)
2. [Security and Compliance Considerations](#security-and-compliance-considerations)
3. [Dynamsoft Product Variants Compared](#dynamsoft-product-variants-compared)
4. [Installation and Setup](#installation-and-setup)
5. [Code Comparison: Basic to Advanced](#code-comparison-basic-to-advanced)
6. [Handling Real-World Barcode Challenges](#handling-real-world-barcode-challenges)
7. [Enterprise Deployment Considerations](#enterprise-deployment-considerations)
8. [Performance Benchmarking](#performance-benchmarking)
9. [When to Use Each Option](#when-to-use-each-option)
10. [Migration Guide: Dynamsoft to IronBarcode](#migration-guide-dynamsoft-to-ironbarcode)

**Code Examples:**
- [dynamsoft-license-complexity.cs](dynamsoft-license-complexity.cs) - License server vs simple key activation
- [dynamsoft-mobile-focus.cs](dynamsoft-mobile-focus.cs) - Camera-first vs document-first architecture
- [dynamsoft-speed-demonstration.cs](dynamsoft-speed-demonstration.cs) - Performance comparison with honest acknowledgment

---

## Understanding the Barcode SDK Ecosystem

The barcode SDK market divides into distinct categories based on primary use case and deployment model. Understanding where Dynamsoft and IronBarcode fit helps clarify when each solution makes sense.

### Camera-First vs Document-First Design Philosophy

Dynamsoft built their barcode reader from the ground up for camera-based scanning. Their architecture reflects this: the CaptureVisionRouter processes video frames in real-time, optimized for the continuous stream of images a mobile camera produces. This design philosophy yields exceptional performance when scanning products at checkout, reading tickets at event venues, or capturing IDs in mobile banking apps.

IronBarcode takes a document-first approach. The architecture optimizes for batch processing PDF invoices, reading barcodes from scanned shipping labels, and extracting data from document management systems. While both libraries can handle either scenario, their design priorities show in their respective strengths.

### The Mobile SDK Landscape

Dynamsoft competes in the mobile barcode space alongside Scandit, barKoder, and Cognex. These companies invest heavily in camera optimization—autofocus algorithms, frame selection heuristics, and motion blur compensation. When milliseconds matter for user experience, this specialization pays off.

For server-side .NET applications, different considerations apply. Processing PDFs doesn't require frame-by-frame optimization. Document workflows need native PDF support, batch processing efficiency, and straightforward licensing for deployment across multiple servers.

### Dynamsoft's Product Portfolio

Dynamsoft has consolidated several products into the Dynamsoft Capture Vision (DCV) family. The .NET bundle pulls in:

- **Dynamsoft Barcode Reader (DBR)** - Their core barcode scanning module, supporting 30+ symbologies including DPM and deformed codes
- **Dynamsoft Label Recognizer (DLR)** - OCR-style text recognition for labels, with MRZ parsing support
- **Dynamsoft Document Normalizer (DDN)** - Document edge detection and perspective correction
- **Dynamsoft Camera Enhancer** - Video frame preprocessing for mobile applications

These modules are routed through a single `CaptureVisionRouter` entry point in the modern API. The .NET SDK is fully supported on Windows (x86/x64) and Linux (x64), targeting .NET 6.0+ and .NET Framework 3.5+. Dynamsoft also ships JavaScript, iOS, Android, and MAUI editions, and the .NET edition is not a second-class citizen — it shares the same Capture Vision architecture and core C++ engine.

### Where IronBarcode Fits

IronBarcode focuses exclusively on .NET developers processing barcodes in document and server contexts. The library doesn't attempt to compete with mobile camera scanning specialists. Instead, it excels at reading barcodes from PDFs, processing scanned documents, generating barcodes for labels and reports, and handling damaged or poorly printed codes that appear in business workflows.

---

## Security and Compliance Considerations

Enterprise deployments require careful attention to licensing mechanics, network requirements, and data handling. Dynamsoft and IronBarcode take different approaches that impact compliance postures.

### License Validation Models

Dynamsoft uses online license activation with a periodic handshake. When you call `LicenseManager.InitLicense(key, out errorMsg)` the SDK contacts Dynamsoft's license server to activate the key, and once activated it caches credentials locally and re-validates them periodically (typically once per usage period). This model gives Dynamsoft visibility into license usage but introduces considerations:

- The first activation requires outbound network connectivity to Dynamsoft's license servers
- Periodic re-validation calls happen during application lifetime
- Air-gapped environments need an offline license file obtained from Dynamsoft support
- Activated trial keys expire and require renewal

IronBarcode uses a simpler key-based activation model. You set a license key string in your code, and the library validates it locally without network calls. For developers deploying to restricted environments, isolated networks, or containerized infrastructure, this difference matters significantly.

### Data Processing Location

Both libraries process barcode images locally—no image data travels to external servers for decoding. This on-premise processing satisfies data sovereignty requirements for sensitive industries.

However, Dynamsoft's license validation does send requests to their servers. While these requests contain only licensing metadata (not image data), some security policies prohibit any external network calls from production systems. IronBarcode's offline validation avoids this concern entirely.

### Government and Regulated Industries

Organizations in government, healthcare, and financial services often operate under strict compliance frameworks. The network-dependent license validation that Dynamsoft employs can trigger security review requirements. Teams must document the external endpoints contacted, data transmitted, and failure modes when those endpoints become unreachable.

For FedRAMP, HIPAA, or PCI-DSS environments, the simpler the network profile, the easier the compliance documentation. IronBarcode's zero-external-call design simplifies these assessments.

---

## Dynamsoft Product Variants Compared

Dynamsoft offers their barcode SDK across multiple platforms with different capabilities and licensing models. Understanding these variants helps when evaluating total cost and integration complexity.

### Dynamsoft Barcode Reader JavaScript

The JavaScript SDK enables barcode scanning directly in web browsers using the device camera. This client-side approach works well for point-of-sale applications and mobile web apps where installing a native application isn't feasible.

**Pricing Model:** Annual subscription tied to active devices or domain. Public list pricing is limited; most deals route through "contact sales" with tier-based pricing depending on volume.

### Dynamsoft Barcode Reader .NET

The .NET SDK provides barcode reading for Windows (x86/x64) and Linux (x64) through .NET 6.0+ and .NET Framework 3.5+. The current package, `Dynamsoft.DotNet.BarcodeReader.Bundle`, sits inside the broader Dynamsoft Capture Vision family and exposes its functionality through the `CaptureVisionRouter` class.

**Pricing Model:** A mix of perpetual and annual subscription tiers. Public reseller pages (e.g. ComponentSource) list .NET SDK perpetual licences starting around the low four-figure range per developer, but the Dynamsoft pricing page itself is largely "contact sales" with custom quotes for production deployments. A free 30-day trial is available.

### Dynamsoft Barcode Reader Mobile (iOS/Android)

Native mobile SDKs optimized for real-time camera scanning. These represent Dynamsoft's flagship products where their speed advantage is most pronounced.

**Pricing Model:** Per-app or per-device annual licensing, typically negotiated through sales. Pricing scales with active device count rather than per-scan credit consumption.

### IronBarcode Alternative

IronBarcode uses a perpetual licensing model with optional annual maintenance. Public list pricing is $799 (Lite), $1,199 (Plus), $2,399 (Professional), and $4,799 (Unlimited). A single license covers development and production deployment without per-server or per-device counting for most scenarios. The pricing simplicity appeals to organizations that struggle with tracking device counts or predicting usage volumes.

| Aspect | Dynamsoft | IronBarcode |
|--------|-----------|-------------|
| License Model | Annual subscription / negotiated perpetual | Perpetual + optional maintenance |
| Cost Structure | Custom quotes; usually contact-sales | Published list price |
| Usage Tracking | Online activation, periodic re-check | Not required |
| Offline Activation | Requires offline license file from support | Simple key |
| Price Predictability | Negotiated per deployment | Fixed upfront |

---

## Installation and Setup

Getting started with either library involves NuGet installation, but the initialization process differs substantially.

### Dynamsoft Installation

```bash
dotnet add package Dynamsoft.DotNet.BarcodeReader.Bundle
```

After installation, Dynamsoft requires license initialization before any barcode operations:

```csharp
// Dynamsoft: Initialize license before using the SDK
// This must happen once at application startup
using Dynamsoft.License;
using Dynamsoft.Core;

public class App
{
    public static void InitializeDynamsoft()
    {
        string licenseKey = "YOUR-DYNAMSOFT-LICENSE-KEY";

        // License initialization can fail if:
        // - Network is unavailable on first activation
        // - License key is invalid or expired
        // - License server is unreachable during a periodic re-check
        int errorCode = LicenseManager.InitLicense(licenseKey, out string errorMsg);

        if (errorCode != (int)EnumErrorCode.EC_OK)
        {
            // Handle license initialization failure
            throw new InvalidOperationException($"License error: {errorMsg}");
        }
    }
}
```

For offline deployments, you'll need to request an offline license file from Dynamsoft support and configure it through the license manager APIs documented for your version.

### IronBarcode Installation

```bash
dotnet add package BarCode
```

IronBarcode initialization is a single line:

```csharp
// IronBarcode: Set license key once
// No network calls, no failure modes to handle
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-LICENSE-KEY";

// Start using immediately
var results = BarcodeReader.Read("invoice.pdf");
```

### The Setup Difference

The contrast in initialization complexity reflects the underlying architectural differences. Dynamsoft's approach ensures real-time license compliance tracking at the cost of startup complexity. IronBarcode's approach prioritizes deployment simplicity with offline validation.

For CI/CD pipelines, Docker containers, and serverless functions, the network-dependent initialization adds latency and failure modes that require defensive coding. IronBarcode's fire-and-forget key assignment integrates cleanly into any deployment model.

---

## Code Comparison: Basic to Advanced

Comparing actual code for common scenarios reveals how each library approaches barcode processing.

### Scenario 1: Reading a Single Barcode from an Image

**Dynamsoft Approach:**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;

// After license initialization...
using CaptureVisionRouter cvRouter = new CaptureVisionRouter();
CapturedResult result = cvRouter.Capture("barcode.png",
    PresetTemplate.PT_READ_BARCODES);

DecodedBarcodesResult barcodesResult = result.GetDecodedBarcodesResult();
foreach (BarcodeResultItem item in barcodesResult.GetItems())
{
    Console.WriteLine($"Format: {item.GetFormatString()}");
    Console.WriteLine($"Value: {item.GetText()}");
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

var results = BarcodeReader.Read("barcode.png");

foreach (var result in results)
{
    Console.WriteLine($"Format: {result.BarcodeType}");
    Console.WriteLine($"Value: {result.Value}");
}
```

Both approaches work for basic scenarios. IronBarcode is slightly more concise, but the difference is minimal for single-image reading.

### Scenario 2: Processing a Multi-Page PDF

PDF processing reveals more significant differences.

**Dynamsoft Approach:**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;
using System.Drawing.Imaging;

// The .NET SDK can hand a PDF directly to CaptureVisionRouter on
// Windows builds, but for cross-platform deployments many teams
// still pre-render pages with a PDF library and pass each page in.
using (var pdfDoc = PdfiumViewer.PdfDocument.Load("invoices.pdf"))
using (var cvRouter = new CaptureVisionRouter())
{
    for (int pageNum = 0; pageNum < pdfDoc.PageCount; pageNum++)
    {
        using var pageImage = pdfDoc.Render(pageNum, 300, 300, true);
        using var ms = new MemoryStream();
        pageImage.Save(ms, ImageFormat.Png);

        CapturedResult result = cvRouter.Capture(ms.ToArray(),
            PresetTemplate.PT_READ_BARCODES);

        foreach (BarcodeResultItem item in result.GetDecodedBarcodesResult().GetItems())
        {
            Console.WriteLine($"Page {pageNum + 1}: {item.GetText()}");
        }
    }
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// IronBarcode handles PDFs natively
var results = BarcodeReader.Read("invoices.pdf");

foreach (var result in results)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
}
```

For document workflows, IronBarcode's native PDF support eliminates an entire dependency and significant code complexity.

### Scenario 3: Batch Processing Multiple Files

**Dynamsoft Approach:**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;

string[] files = Directory.GetFiles("invoices/", "*.png");

using var cvRouter = new CaptureVisionRouter();

foreach (string file in files)
{
    CapturedResult result = cvRouter.Capture(file, PresetTemplate.PT_READ_BARCODES);
    foreach (BarcodeResultItem item in result.GetDecodedBarcodesResult().GetItems())
    {
        Console.WriteLine($"{Path.GetFileName(file)}: {item.GetText()}");
    }
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

string[] files = Directory.GetFiles("invoices/", "*.*");  // Includes PDFs

var results = BarcodeReader.Read(files);

foreach (var result in results)
{
    Console.WriteLine($"{result.FileName}: {result.Value}");
}
```

IronBarcode accepts an array of files directly and handles mixed formats (images and PDFs) automatically.

### Scenario 4: Camera Frame Processing

For video frame processing, Dynamsoft's design shows its strength.

**Dynamsoft Approach (using CaptureVisionRouter):**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.Core;
using Dynamsoft.DBR;

// Dynamsoft's video processing architecture
using var router = new CaptureVisionRouter();

// Wrap a raw frame from the camera in an ImageData struct
byte[] frameData = GetCameraFrame(); // Your camera integration
int width = 1920;
int height = 1080;

var imageData = new ImageData
{
    Bytes = frameData,
    Width = width,
    Height = height,
    Stride = width * 3,
    Format = EnumImagePixelFormat.IPF_RGB_888
};

CapturedResult capturedResult = router.Capture(imageData,
    PresetTemplate.PT_READ_BARCODES);

// Process results optimized for real-time display
DecodedBarcodesResult barcodeResults = capturedResult.GetDecodedBarcodesResult();
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// IronBarcode can process individual frames but isn't optimized for video speed
byte[] frameData = GetCameraFrame();
using var ms = new MemoryStream(frameData);
using var bitmap = new System.Drawing.Bitmap(ms);

var results = BarcodeReader.Read(bitmap);
```

IronBarcode can process individual frames, but it lacks the video-optimized architecture that makes Dynamsoft excel at sustained real-time scanning. For mobile camera applications requiring 30+ fps scanning with instant feedback, Dynamsoft's specialized design delivers better user experience.

### Scenario 5: Reading Barcodes with Specific Format Filtering

**Dynamsoft Approach:**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;

using var router = new CaptureVisionRouter();

// Edit the runtime template JSON to limit symbologies to QR + Code 128.
// The Capture Vision API uses string template names rather than the
// older PublicRuntimeSettings struct.
SimplifiedCaptureVisionSettings settings = router.GetSimplifiedSettings(
    PresetTemplate.PT_READ_BARCODES);
settings.BarcodeSettings.BarcodeFormatIds =
    (long)(EnumBarcodeFormat.BF_QR_CODE | EnumBarcodeFormat.BF_CODE_128);
router.UpdateSettings(PresetTemplate.PT_READ_BARCODES, settings);

CapturedResult result = router.Capture("mixed-barcodes.png",
    PresetTemplate.PT_READ_BARCODES);
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// IronBarcode auto-detects by default, but you can filter results
var results = BarcodeReader.Read("mixed-barcodes.png");

// Or use read options for format filtering
var options = new BarcodeReaderOptions
{
    ExpectBarcodeTypes = BarcodeEncoding.QRCode | BarcodeEncoding.Code128
};

var filteredResults = BarcodeReader.Read("mixed-barcodes.png", options);
```

Both libraries support format filtering. IronBarcode's default auto-detection often makes explicit filtering unnecessary for document processing where you want to capture all barcodes present.

---

## Handling Real-World Barcode Challenges

Production barcode scanning rarely involves pristine test images. Real-world challenges test each library's capabilities differently.

### Challenge: Damaged or Poorly Printed Barcodes

Business documents often contain barcodes printed by thermal printers on labels that scratch, fade, or smudge. Shipping warehouses, retail environments, and healthcare settings produce countless damaged codes daily.

**Dynamsoft's Approach:**
Dynamsoft focuses on motion blur and autofocus challenges common in camera scanning. Their preprocessing targets camera artifacts rather than print degradation.

**IronBarcode's Approach:**
IronBarcode includes ML-powered error correction specifically designed for document barcodes. The library can reconstruct partially damaged codes, fill in missing bars, and handle the specific degradation patterns that appear on printed documents.

```csharp
using IronBarCode;

// ML-powered reading handles damaged document barcodes
var options = new BarcodeReaderOptions
{
    // Enable aggressive error correction for damaged codes
    Speed = ReadingSpeed.Detailed,
    ExpectMultipleBarcodes = true
};

var results = BarcodeReader.Read("damaged-shipping-label.png", options);
```

For document workflows, IronBarcode's accuracy on damaged codes often matters more than raw speed.

### Challenge: Multi-Barcode Documents

Shipping labels, medical specimens, and warehouse manifests frequently contain multiple barcodes on a single document. Reliably detecting all codes matters for compliance and tracking.

**Both libraries support multi-barcode detection:**

```csharp
// Dynamsoft
using var router = new CaptureVisionRouter();
CapturedResult result = router.Capture("multi-barcode.png",
    PresetTemplate.PT_READ_BARCODES);
// result.GetDecodedBarcodesResult().GetItems() lists every code

// IronBarcode
var results = BarcodeReader.Read("multi-barcode.png");
// Returns all detected barcodes with positions
foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.Value} at ({barcode.X}, {barcode.Y})");
}
```

### Challenge: Rotated or Skewed Barcodes

Document scanners don't always feed pages perfectly straight. Barcodes may appear at angles or skewed perspectives.

**IronBarcode handles rotation automatically:**

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced
    // IronBarcode auto-rotates by default during the scan
};

var results = BarcodeReader.Read("skewed-scan.pdf", options);
```

Both libraries can handle rotation, though IronBarcode's document-focused processing often handles the specific skew patterns from flatbed and ADF scanners more reliably.

### Challenge: Low Resolution or Small Barcodes

Scanned documents sometimes contain small barcodes that become pixelated at lower scan resolutions.

**IronBarcode's approach:**

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Detailed  // More thorough analysis
};

var results = BarcodeReader.Read("low-res-document.png", options);
```

The `Detailed` speed setting instructs IronBarcode to spend more time analyzing the image, which helps with small or unclear barcodes in document processing contexts where real-time speed isn't critical.

---

## Enterprise Deployment Considerations

Deploying barcode libraries in enterprise environments involves licensing mechanics, containerization, and scaling considerations.

### Docker Deployment

**Dynamsoft in Docker:**
Licensing in containers requires careful planning. Dynamsoft's license validation happens at runtime, so containers need network access to license servers or pre-configured offline license files.

```dockerfile
# Dynamsoft requires license setup in container
ENV DYNAMSOFT_LICENSE_KEY="your-key"
# Or mount offline license file
VOLUME /app/license
```

**IronBarcode in Docker:**

```dockerfile
# IronBarcode just needs the key
ENV IRONBARCODE_LICENSE_KEY="your-key"
```

```csharp
// In code
IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE_KEY");
```

IronBarcode's simpler licensing model integrates naturally with container orchestration platforms like Kubernetes without license server dependencies.

### Cloud and Serverless Deployment

Serverless functions (AWS Lambda, Azure Functions) have cold start considerations. Network-dependent license validation adds latency and potential failure modes to cold starts.

**Dynamsoft cold start concerns:**
- License validation adds network roundtrip
- License server outages cause function failures
- Offline licensing complex in serverless contexts

**IronBarcode serverless deployment:**
- No network calls for license validation
- Key validation is synchronous and local
- Consistent cold start behavior

### High-Availability Requirements

For systems requiring high availability, license server dependencies become failure points. IronBarcode's local validation eliminates this dependency, simplifying high-availability architectures.

### License Compliance Tracking

Enterprise software management teams need to track license usage. Dynamsoft's server-based validation provides usage telemetry automatically. IronBarcode's offline model requires self-managed compliance tracking if that capability is needed.

---

## Performance Benchmarking

Performance comparisons must be honest about what each library optimizes for.

### Real-Time Camera Scanning

**Dynamsoft excels here.** In my testing, Dynamsoft consistently achieves faster frame-by-frame decoding when processing video streams. For applications where users hold a phone camera over a barcode and expect instant recognition, Dynamsoft's millisecond-level optimizations create noticeably better user experience.

This isn't a claim I make lightly. When testing mobile scanning scenarios with continuous video feeds, Dynamsoft's specialized architecture delivers real advantages. If your primary use case is mobile camera scanning, Dynamsoft deserves serious consideration.

### Document Batch Processing

For processing 1,000 PDF invoices overnight in a batch job, the per-frame speed difference becomes irrelevant. What matters:

- Native PDF support (IronBarcode: yes, Dynamsoft: requires additional library)
- Memory efficiency for large batches
- Accuracy on printed document barcodes

IronBarcode's document-focused design handles these batch scenarios efficiently without the overhead of video stream optimization.

For high-volume processing techniques including parallel execution and progress tracking, see our [Batch Processing tutorial](../tutorials/batch-processing.md).

### Accuracy vs Speed Trade-offs

Dynamsoft optimizes for speed by making assumptions appropriate for camera scanning—good lighting, live preview for repositioning, multiple attempts possible. Document processing faces different constraints: you get one scan, the barcode may be damaged, and batch processing makes per-image speed less critical than overall throughput.

IronBarcode's ML-powered error correction trades some speed for accuracy on document barcodes. For batch document processing where you need to extract every barcode reliably, this trade-off often makes sense.

### Benchmark Context

Raw benchmark numbers without context mislead. A benchmark showing "Library A is 5x faster than Library B" means nothing if Library A optimizes for video and Library B for documents. Choose the library optimized for your actual use case.

---

## When to Use Each Option

### Choose Dynamsoft When:

- **Mobile camera scanning is your primary use case** - Dynamsoft's video stream optimization genuinely excels here
- **Real-time user experience matters** - Point-and-scan applications benefit from millisecond optimizations
- **You're building cross-platform mobile apps** - Their iOS, Android, and JavaScript SDKs share consistent architecture
- **You need their full product suite** - Label recognition, document normalization, and camera enhancement integrate smoothly
- **Credit-based pricing fits your usage** - Predictable low-volume scenarios where credits won't be exhausted unexpectedly

### Choose IronBarcode When:

- **Server-side document processing** - PDF invoices, shipping labels, scanned documents
- **Batch processing workflows** - Processing thousands of files where per-frame speed doesn't matter
- **Simpler licensing requirements** - No license servers, credit tracking, or per-scan consumption (with perpetual license)
- **Air-gapped or restricted networks** - Offline validation without network dependencies
- **Damaged barcode handling** - ML-powered error correction for poorly printed document codes
- **Native PDF support needed** - Direct PDF reading without additional libraries

### Hybrid Approach

Some organizations legitimately need both. A retail company might use Dynamsoft for their point-of-sale mobile app while using IronBarcode for their warehouse document processing system. Using the right tool for each job isn't compromise—it's good architecture.

---

## Migration Guide: Dynamsoft to IronBarcode

For developers with existing Dynamsoft implementations in server-side document processing scenarios, migrating to IronBarcode can simplify licensing and improve damaged barcode handling.

### Why Migrate?

**Consider migrating if:**
- License server management creates operational overhead
- Docker/container deployments struggle with Dynamsoft licensing
- Damaged document barcodes cause accuracy issues
- PDF processing requires additional library dependencies
- Simpler licensing model would reduce costs

**Keep Dynamsoft if:**
- Mobile camera scanning is the primary use case
- Real-time video processing is required
- You've already invested in their full product ecosystem

### API Mapping Reference

| Dynamsoft | IronBarcode |
|-----------|-------------|
| `LicenseManager.InitLicense(key, out msg)` | `IronBarCode.License.LicenseKey = "..."` |
| `cvRouter.Capture(path, PresetTemplate.PT_READ_BARCODES)` | `BarcodeReader.Read(path)` |
| `cvRouter.Capture(imageData, template)` | `BarcodeReader.Read(bytes)` |
| `BarcodeResultItem.GetText()` | `BarcodeResult.Value` |
| `BarcodeResultItem.GetFormatString()` | `BarcodeResult.BarcodeType` |
| `BarcodeResultItem.GetLocation()` | `BarcodeResult.X` / `Y` |
| `SimplifiedCaptureVisionSettings.BarcodeSettings.BarcodeFormatIds` | `BarcodeReaderOptions.ExpectBarcodeTypes` |

### Migration Example: Basic Reading

**Before (Dynamsoft):**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;
using Dynamsoft.License;
using Dynamsoft.Core;

public class BarcodeService : IDisposable
{
    private readonly CaptureVisionRouter _router;

    public BarcodeService(string licenseKey)
    {
        int errorCode = LicenseManager.InitLicense(licenseKey, out string errorMsg);
        if (errorCode != (int)EnumErrorCode.EC_OK)
        {
            throw new InvalidOperationException($"License error: {errorMsg}");
        }
        _router = new CaptureVisionRouter();
    }

    public List<string> ReadBarcodes(string filePath)
    {
        CapturedResult result = _router.Capture(filePath, PresetTemplate.PT_READ_BARCODES);
        return result.GetDecodedBarcodesResult()
            .GetItems()
            .Select(i => i.GetText())
            .ToList();
    }

    public void Dispose() => _router?.Dispose();
}
```

**After (IronBarcode):**

```csharp
using IronBarCode;

public class BarcodeService
{
    public BarcodeService(string licenseKey)
    {
        IronBarCode.License.LicenseKey = licenseKey;
        // No network validation, no error handling needed
    }

    public List<string> ReadBarcodes(string filePath)
    {
        var results = BarcodeReader.Read(filePath);
        return results.Select(r => r.Value).ToList();
    }

    // No Dispose needed - IronBarcode manages resources automatically
}
```

### Migration Example: PDF Processing

**Before (Dynamsoft with PDF library):**

```csharp
using Dynamsoft.CVR;
using Dynamsoft.DBR;
using PdfiumViewer;

public List<BarcodeData> ProcessPdfInvoices(string pdfPath)
{
    var barcodes = new List<BarcodeData>();

    using (var pdfDoc = PdfDocument.Load(pdfPath))
    using (var router = new CaptureVisionRouter())
    {
        for (int page = 0; page < pdfDoc.PageCount; page++)
        {
            using (var pageImage = pdfDoc.Render(page, 300, 300, true))
            using (var ms = new MemoryStream())
            {
                pageImage.Save(ms, ImageFormat.Png);
                CapturedResult result = router.Capture(ms.ToArray(),
                    PresetTemplate.PT_READ_BARCODES);

                foreach (var item in result.GetDecodedBarcodesResult().GetItems())
                {
                    barcodes.Add(new BarcodeData
                    {
                        Value = item.GetText(),
                        Format = item.GetFormatString(),
                        Page = page + 1
                    });
                }
            }
        }
    }

    return barcodes;
}
```

**After (IronBarcode):**

```csharp
using IronBarCode;

public List<BarcodeData> ProcessPdfInvoices(string pdfPath)
{
    var results = BarcodeReader.Read(pdfPath);

    return results.Select(r => new BarcodeData
    {
        Value = r.Value,
        Format = r.BarcodeType.ToString(),
        Page = r.PageNumber
    }).ToList();
}
```

### Common Migration Issues

**Issue: Different enum names for barcode formats**

Dynamsoft uses `EnumBarcodeFormat.BF_QR_CODE`, IronBarcode uses `BarcodeEncoding.QRCode`. Create a mapping function if you need to preserve format filtering:

```csharp
public static BarcodeEncoding MapFormat(EnumBarcodeFormat dynamsoft)
{
    return dynamsoft switch
    {
        EnumBarcodeFormat.BF_QR_CODE => BarcodeEncoding.QRCode,
        EnumBarcodeFormat.BF_CODE_128 => BarcodeEncoding.Code128,
        EnumBarcodeFormat.BF_CODE_39 => BarcodeEncoding.Code39,
        EnumBarcodeFormat.BF_EAN_13 => BarcodeEncoding.EAN13,
        EnumBarcodeFormat.BF_EAN_8 => BarcodeEncoding.EAN8,
        EnumBarcodeFormat.BF_UPC_A => BarcodeEncoding.UPCA,
        EnumBarcodeFormat.BF_UPC_E => BarcodeEncoding.UPCE,
        EnumBarcodeFormat.BF_PDF417 => BarcodeEncoding.PDF417,
        EnumBarcodeFormat.BF_DATAMATRIX => BarcodeEncoding.DataMatrix,
        _ => BarcodeEncoding.All
    };
}
```

**Issue: License initialization timing**

Dynamsoft requires `LicenseManager.InitLicense()` before any barcode operations. IronBarcode's `License.LicenseKey` can be set at any time, though setting it early in application startup is recommended.

**Issue: Video stream processing**

If your Dynamsoft code uses `CaptureVisionRouter` for video processing, that use case isn't IronBarcode's strength. Consider keeping Dynamsoft for those specific features while migrating document processing to IronBarcode.

---

## Conclusion

Dynamsoft Barcode Reader excels at mobile camera scanning and real-time video processing. Their specialized architecture delivers genuine speed advantages for applications where users interact with live camera feeds. For developers building mobile apps, point-of-sale systems, or ticket scanning applications, Dynamsoft merits strong consideration.

IronBarcode takes a different approach, optimizing for document processing workflows common in enterprise .NET development. Native PDF support, ML-powered error correction for damaged barcodes, and simplified licensing create advantages for server-side batch processing scenarios.

The honest recommendation: use the right tool for your use case. Mobile camera scanning? Dynamsoft's specialization shows. Server-side document processing? IronBarcode's architecture fits better. Some organizations legitimately need both.

For detailed code examples and documentation, visit the [IronBarcode documentation](https://ironsoftware.com/csharp/barcode/docs/).

---

## References

- <a href="https://www.dynamsoft.com/barcode-reader/overview/" rel="nofollow">Dynamsoft Barcode Reader Overview</a>
- <a href="https://www.dynamsoft.com/barcode-reader/docs/server/programming/dotnet/user-guide.html" rel="nofollow">Dynamsoft .NET SDK User Guide</a>
- <a href="https://www.nuget.org/packages/Dynamsoft.DotNet.BarcodeReader.Bundle" rel="nofollow">Dynamsoft.DotNet.BarcodeReader.Bundle on NuGet</a>
- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/)
- [IronBarcode NuGet Package](https://www.nuget.org/packages/BarCode)

---

*Last verified: May 2026*
