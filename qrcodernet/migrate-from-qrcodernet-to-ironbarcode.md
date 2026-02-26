# Migrating from QRCoder to IronBarcode

This guide is written for development teams whose projects started with QRCoder and have grown to require capabilities beyond QR code generation. QRCoder is a well-maintained, high-quality library for its intended scope, and migration is not a reflection of its quality. It is a response to requirements — format expansion, barcode reading, PDF integration — that fall outside what QRCoder was designed to do. This guide covers the complete technical path from QRCoder to IronBarcode: package replacement, namespace changes, code translation for every common pattern, a mapping of API equivalences, and solutions to the issues that arise during the transition.

## Why Migrate from QRCoder

**Format Requirements Beyond QR:** QRCoder generates QR codes exclusively. Projects that grow to require Code 128 labels for logistics, EAN-13 codes for retail product identification, DataMatrix for pharmaceutical serialisation, or PDF417 for government document compliance must introduce a separate library for each additional format. As the number of required formats increases, so does the number of independent packages, APIs, and maintenance obligations.

**Barcode Reading Requirements:** QRCoder has no barcode reading API — it is a generation-only library by design. Any application that needs to decode QR codes from incoming images, verify barcodes on scanned documents, or process shipment labels must add a dedicated reading library alongside QRCoder. That second library brings its own API surface, its own update cycle, and its own compatibility concerns on each .NET upgrade.

**Multi-Library Maintenance:** The realistic accumulation pattern for a QRCoder-based project runs from QRCoder for QR generation to a 1D barcode library for shipping labels to a reading library for decoding. Managing three or four independent packages means tracking separate release notes, resolving version conflicts when any one of them updates, and maintaining familiarity with multiple APIs across a development team. The maintenance burden compounds over time in ways that are not visible in the initial integration estimate.

**PDF Support:** Embedding barcodes in PDF documents or extracting barcodes from incoming PDF files are both outside QRCoder's scope. Document-processing pipelines — invoice handling, label generation from PDF templates, compliance reporting — encounter this boundary early. A dedicated PDF library must be added alongside QRCoder, or the architecture must route barcode generation and PDF generation through separate subsystems.

### The Fundamental Problem

QRCoder solves QR generation efficiently. The moment any requirement reaches past QR, the codebase must start pulling in additional packages to fill each gap:

```csharp
// QRCoder handles QR generation well — but no further
using QRCoder;
using System.IO;

var generator = new QRCodeGenerator();
var qrCodeData = generator.CreateQrCode("SHIP-12345", QRCodeGenerator.ECCLevel.M);
var qrCode = new PngByteQRCode(qrCodeData);
File.WriteAllBytes("qr.png", qrCode.GetGraphic(10));

// Now the shipping team asks for Code 128 — add NetBarcode
// Now the warehouse needs to scan barcodes — add ZXing.Net
// Now a report needs a barcode in a PDF — add a PDF library
// Each addition: new namespace, new API, new maintenance cycle
```

IronBarcode consolidates generation, reading, and PDF integration under one API:

```csharp
// IronBarcode — one library covers all of the above
using IronBarCode;

// Generate QR
BarcodeWriter.CreateBarcode("SHIP-12345", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");

// Generate Code 128 — same API
BarcodeWriter.CreateBarcode("SHIP-12345", BarcodeEncoding.Code128)
    .SaveAsPng("label.png");

// Read a barcode from an image — built in
var results = BarcodeReader.Read("incoming.png");
```

## IronBarcode vs QRCoder: Feature Comparison

| Feature | QRCoder | IronBarcode |
|---|---|---|
| QR Code Generation | Yes — excellent | Yes |
| Micro QR | Yes | No |
| Code 128 / Code 39 | No | Yes |
| EAN-13 / UPC-A | No | Yes |
| DataMatrix | No | Yes |
| PDF417 | No | Yes |
| Aztec | No | Yes |
| Total Formats | 1 | 50+ |
| Barcode Reading | No | Yes — auto-detection |
| PDF Barcode Extraction | No | Yes |
| PDF Barcode Stamping | No | Yes |
| Logo Embedding | Yes | Yes |
| Colour Customisation | Yes | Yes |
| SVG Output | Yes | Yes |
| PayloadGenerator Helpers | Yes | No — manual string construction |
| Zero External Dependencies | Yes | Self-contained |
| Licence | MIT — unrestricted | Commercial ($749 single developer) |

## Quick Start: QRCoder to IronBarcode Migration

### Step 1: Replace NuGet Package

Remove QRCoder from the project:

```bash
dotnet remove package QRCoder
```

Install IronBarcode:

```bash
dotnet add package IronBarcode
```

If the project was using ZXing.Net alongside QRCoder for barcode reading, that package can also be removed at this stage:

```bash
dotnet remove package ZXing.Net
```

### Step 2: Update Namespaces

Replace the QRCoder namespace import with the IronBarcode namespace:

```csharp
// Before (QRCoder)
using QRCoder;

// After (IronBarcode)
using IronBarCode;
```

### Step 3: Initialize License

Add licence initialization at application startup — `Program.cs`, `Startup.cs`, or a static constructor:

```csharp
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

A free trial key is available without registration. Production deployments require a purchased licence.

## Code Migration Examples

### Basic QR Code Generation

The most common QRCoder pattern requires three objects: a `QRCodeGenerator` instance, a `QRCodeData` intermediate object, and a renderer class such as `PngByteQRCode`. In IronBarcode, a single static call replaces this chain.

**QRCoder Approach:**

```csharp
using QRCoder;

public class BarcodeService
{
    private readonly QRCodeGenerator _generator = new QRCodeGenerator();

    public byte[] GenerateQR(string content)
    {
        var qrCodeData = _generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(10); // 10px per module
    }
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

public class BarcodeService
{
    public byte[] GenerateQR(string content)
    {
        return BarcodeWriter.CreateBarcode(content, BarcodeEncoding.QRCode)
            .ResizeTo(200, 200)
            .ToPngBinaryData();
    }
}
```

`BarcodeWriter` is a static class — no instance is required, and there is no intermediate data object. The `ResizeTo(width, height)` method replaces the pixel-per-module integer that QRCoder accepts in `GetGraphic`. Where QRCoder sizes the output by module count (e.g., `GetGraphic(10)` for a 10px-per-module image), IronBarcode takes absolute pixel dimensions.

### Saving QR Codes to File

QRCoder separates in-memory bytes from file output only by the terminal step — the renderer chain is the same. IronBarcode uses different terminal methods on the same chain.

**QRCoder Approach:**

```csharp
using QRCoder;
using System.IO;

var generator = new QRCodeGenerator();
var qrCodeData = generator.CreateQrCode("https://example.com", QRCodeGenerator.ECCLevel.M);
var pngBytes = new PngByteQRCode(qrCodeData).GetGraphic(10);
File.WriteAllBytes("output.png", pngBytes);
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Save directly — no intermediate byte array needed
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .ResizeTo(300, 300)
    .SaveAsPng("output.png");

// Or obtain bytes for in-memory use
byte[] pngBytes = BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .ResizeTo(300, 300)
    .ToPngBinaryData();
```

The `SaveAsPng` method is a terminal that writes directly to disk. `ToPngBinaryData` returns a `byte[]` for streaming, HTTP responses, or database storage.

### SVG Output

**QRCoder Approach:**

```csharp
using QRCoder;
using System.IO;

var generator = new QRCodeGenerator();
var qrCodeData = generator.CreateQrCode("https://example.com", QRCodeGenerator.ECCLevel.M);
string svgContent = new SvgQRCode(qrCodeData).GetGraphic(10);
File.WriteAllText("output.svg", svgContent);
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsSvg("output.svg");
```

The `SvgQRCode` renderer class and its `GetGraphic` call collapse into a single `SaveAsSvg` terminal method.

### Error Correction Level Mapping

QRCoder requires explicit ECC selection on every `CreateQrCode` call using the `QRCodeGenerator.ECCLevel` enum. IronBarcode applies a sensible default when using `BarcodeWriter.CreateBarcode`, and exposes explicit control through `QRCodeWriter.QrErrorCorrectionLevel`.

**QRCoder Approach:**

```csharp
using QRCoder;

var generator = new QRCodeGenerator();

// Medium — suitable for most generation without logos
var dataMedium = generator.CreateQrCode("https://example.com", QRCodeGenerator.ECCLevel.M);

// High — required when a logo will occlude part of the code
var dataHigh = generator.CreateQrCode("https://example.com", QRCodeGenerator.ECCLevel.H);
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Default — equivalent to Medium, applied automatically
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsPng("default-ecc.png");

// Explicit level via QRCodeWriter
var qr = QRCodeWriter.CreateQrCode(
    "https://example.com",
    500,
    QRCodeWriter.QrErrorCorrectionLevel.Highest
);
qr.SaveAsPng("high-ecc.png");
```

The level mapping is: `ECCLevel.L` → `Low`, `ECCLevel.M` → `Medium`, `ECCLevel.Q` → `Quartile`, `ECCLevel.H` → `Highest`.

### Logo Embedding

Both libraries support logo embedding on QR codes. QRCoder routes this through the `QRCode` renderer class (not `PngByteQRCode`) with `System.Drawing.Bitmap` parameters. IronBarcode provides `AddBrandLogo` as a named method accepting a file path. The [QR code style customisation guide](https://ironsoftware.com/csharp/barcode/how-to/customize-qr-code-style/) covers logo sizing, positioning, and colour combination options.

**QRCoder Approach:**

```csharp
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

var generator = new QRCodeGenerator();
// ECCLevel.H is required — the logo occludes part of the code
var qrCodeData = generator.CreateQrCode("https://example.com", QRCodeGenerator.ECCLevel.H);

var qrCode = new QRCode(qrCodeData);
var logoBitmap = new Bitmap("logo.png");
var qrBitmap = qrCode.GetGraphic(10, Color.Black, Color.White, logoBitmap);
qrBitmap.Save("qr-logo.png", ImageFormat.Png);
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

var qr = QRCodeWriter.CreateQrCode(
    "https://example.com",
    500,
    QRCodeWriter.QrErrorCorrectionLevel.Highest
);
qr.AddBrandLogo("logo.png");
qr.SaveAsPng("qr-logo.png");
```

The `System.Drawing.Bitmap` dependency is removed — IronBarcode accepts a file path string. Error correction at the Highest level is still recommended when a logo occludes a portion of the code; this is set explicitly through `QrErrorCorrectionLevel.Highest`.

### PayloadGenerator.WiFi Migration

QRCoder's `PayloadGenerator.WiFi` formats the standard WiFi QR payload string. The output format is a public standard (`WIFI:T:{auth};S:{ssid};P:{password};;`) defined by the QR code specification. When migrating, these strings can be constructed directly.

**QRCoder Approach:**

```csharp
using QRCoder;

var wifi = new PayloadGenerator.WiFi(
    ssid: "OfficeNetwork",
    password: "SecurePass123",
    authenticationMode: PayloadGenerator.WiFi.Authentication.WPA
);
// wifi.ToString() produces: WIFI:T:WPA;S:OfficeNetwork;P:SecurePass123;;

var generator = new QRCodeGenerator();
var qrData = generator.CreateQrCode(wifi.ToString(), QRCodeGenerator.ECCLevel.M);
var qrCode = new PngByteQRCode(qrData);
System.IO.File.WriteAllBytes("wifi.png", qrCode.GetGraphic(10));
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

// Construct the standard WiFi QR string directly
string wifiPayload = "WIFI:T:WPA;S:OfficeNetwork;P:SecurePass123;;";
BarcodeWriter.CreateBarcode(wifiPayload, BarcodeEncoding.QRCode)
    .ResizeTo(300, 300)
    .SaveAsPng("wifi.png");
```

For teams with many `PayloadGenerator` types in use (vCard, CalendarEvent, Geo, SMS), the approach is the same: identify the formatted string that each generator produces using its `.ToString()` output, then construct that string directly or through a small static helper method.

## QRCoder API to IronBarcode Mapping Reference

| QRCoder | IronBarcode | Notes |
|---|---|---|
| `new QRCodeGenerator()` | Static class — no instance | `BarcodeWriter` requires no instantiation |
| `qrGenerator.CreateQrCode(data, ECCLevel.M)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)` | Direct replacement |
| `new PngByteQRCode(qrCodeData)` | Not needed | Rendering is part of the fluent chain |
| `qrCode.GetGraphic(20)` returning `byte[]` | `.ToPngBinaryData()` | Size via `.ResizeTo(w, h)` |
| `File.WriteAllBytes(path, bytes)` pattern | `.SaveAsPng(path)` | Terminal method on `GeneratedBarcode` |
| `new SvgQRCode(qrCodeData).GetGraphic(10)` | `.SaveAsSvg(path)` | |
| `new QRCode(qrCodeData).GetGraphic(...)` | `QRCodeWriter.CreateQrCode(data, size, level)` | Used for logo and colour customisation |
| `QRCodeGenerator.ECCLevel.L` | `QRCodeWriter.QrErrorCorrectionLevel.Low` | |
| `QRCodeGenerator.ECCLevel.M` | `QRCodeWriter.QrErrorCorrectionLevel.Medium` | Default when unspecified |
| `QRCodeGenerator.ECCLevel.Q` | `QRCodeWriter.QrErrorCorrectionLevel.Quartile` | |
| `QRCodeGenerator.ECCLevel.H` | `QRCodeWriter.QrErrorCorrectionLevel.Highest` | Required for logo embedding |
| `PayloadGenerator.WiFi(...).ToString()` | `"WIFI:T:WPA;S:{ssid};P:{pass};;"` | Construct standard string directly |
| `PayloadGenerator.ContactData(...).ToString()` | `"BEGIN:VCARD\n..."` | vCard is a public standard |
| No reading API | `BarcodeReader.Read(path)` | New capability — no separate library |
| QR format only | 50+ formats via `BarcodeEncoding.*` | New capability |

## Common Migration Issues and Solutions

### Issue 1: PayloadGenerator String Format Change

**QRCoder:** `PayloadGenerator.WiFi`, `PayloadGenerator.ContactData`, and other helpers produce formatted strings automatically. Code passes the helper object's `.ToString()` output to `CreateQrCode`.

**Solution:** Identify the string each `PayloadGenerator` type produces — its format is documented in the QR code specification and straightforward to reproduce. For WiFi: `WIFI:T:WPA;S:{ssid};P:{password};;`. For vCard: standard `BEGIN:VCARD / END:VCARD` blocks. Write static helper methods if many call sites use the same payload type:

```csharp
public static string WifiPayload(string ssid, string password, string auth = "WPA")
    => $"WIFI:T:{auth};S:{ssid};P:{password};;";
```

### Issue 2: ECCLevel Enum Mapping

**QRCoder:** `QRCodeGenerator.ECCLevel` is a nested enum with values `L`, `M`, `Q`, `H`. It is a required parameter on every `CreateQrCode` call.

**Solution:** Map each value to its `QRCodeWriter.QrErrorCorrectionLevel` equivalent when using the `QRCodeWriter` path. When using `BarcodeWriter.CreateBarcode`, the ECC level defaults to Medium automatically and requires no parameter. Update each call site:

```csharp
// QRCoder ECCLevel.H → IronBarcode QrErrorCorrectionLevel.Highest
var qr = QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);

// QRCoder ECCLevel.M → IronBarcode default (no parameter needed)
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode);
```

### Issue 3: Renderer Class Removal

**QRCoder:** Each output format requires its own renderer class instantiation — `PngByteQRCode`, `SvgQRCode`, `AsciiQRCode`, `Base64QRCode`, `QRCode`. These classes accept `QRCodeData` and expose `GetGraphic` with different return types.

**Solution:** Remove all renderer class instantiations. IronBarcode builds format output into the terminal methods of the fluent chain. Replace `new PngByteQRCode(data).GetGraphic(n)` with `.ToPngBinaryData()` after `.ResizeTo()`. Replace `new SvgQRCode(data).GetGraphic(n)` with `.SaveAsSvg(path)`. The `QRCodeData` intermediate object also disappears — it is not needed in IronBarcode.

### Issue 4: PngByteQRCode Removal and Module-Based Sizing

**QRCoder:** `PngByteQRCode.GetGraphic(int pixelsPerModule)` sizes the output by the number of pixels per QR module. The resulting image dimensions depend on the number of modules, which varies with content length and ECC level.

**Solution:** Replace module-based sizing with `ResizeTo(int width, int height)` for absolute pixel dimensions. If your code calculated the expected output size dynamically from module count, switch to a fixed target size that meets your output requirements:

```csharp
// Before: GetGraphic(20) — output size varies with content
var bytes = new PngByteQRCode(qrData).GetGraphic(20);

// After: fixed 400×400 regardless of content
byte[] bytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
    .ResizeTo(400, 400)
    .ToPngBinaryData();
```

## QRCoder Migration Checklist

### Pre-Migration Tasks

Audit the codebase for all QRCoder usage before making any changes:

```bash
# Find all QRCoder namespace imports
grep -r "using QRCoder" --include="*.cs" .

# Find all generator and renderer instantiations
grep -r "QRCodeGenerator\|PngByteQRCode\|SvgQRCode\|AsciiQRCode\|QRCode\b" --include="*.cs" .

# Find all PayloadGenerator usages
grep -r "PayloadGenerator\." --include="*.cs" .

# Find ECCLevel references
grep -r "ECCLevel\." --include="*.cs" .

# Find ZXing.Net usage to remove at the same time
grep -r "using ZXing" --include="*.cs" .
```

- Document all `PayloadGenerator` types in use and note their string output formats
- Note all ECC level values in use and their call sites
- Identify any ZXing.Net reading code that was paired with QRCoder for decoding — this can be replaced at the same time
- Obtain an IronBarcode licence key (trial keys are available without registration)

### Code Update Tasks

1. Run `dotnet remove package QRCoder`
2. Run `dotnet remove package ZXing.Net` if it was used only for reading alongside QRCoder
3. Run `dotnet add package IronBarcode`
4. Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` to application startup
5. Replace all `using QRCoder;` imports with `using IronBarCode;`
6. Remove all `new QRCodeGenerator()` instance declarations — `BarcodeWriter` is static
7. Replace all `generator.CreateQrCode(data, ECCLevel.X)` + renderer chain patterns with `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)` chains
8. Replace `.GetGraphic(n)` calls with `.ToPngBinaryData()` and add `.ResizeTo(w, h)` before the terminal
9. Replace `File.WriteAllBytes(path, bytes)` save patterns with `.SaveAsPng(path)` terminal methods
10. Replace `new SvgQRCode(data).GetGraphic(n)` with `.SaveAsSvg(path)`
11. Replace `PayloadGenerator.WiFi(...)` and other helper instantiations with direct format strings
12. Map each `QRCodeGenerator.ECCLevel` value to its `QRCodeWriter.QrErrorCorrectionLevel` equivalent
13. Replace any ZXing.Net `BarcodeReaderGeneric.Decode(bitmap)` calls with `BarcodeReader.Read(path)`
14. For logo embedding, replace `new QRCode(data).GetGraphic(n, ..., logoBitmap)` with `QRCodeWriter.CreateQrCode(...).AddBrandLogo(path)`

### Post-Migration Testing

- Scan all generated QR codes with at least two independent QR code scanner applications to verify readability
- Verify logo-embedded QR codes scan correctly — confirm error correction level is `Highest`
- Compare visual output of generated codes with pre-migration samples where appearance consistency is required
- Test all `PayloadGenerator` replacements — confirm WiFi, vCard, and other payload strings decode correctly on target devices
- If ZXing.Net was also removed, verify all barcode reading paths function through `BarcodeReader.Read`
- Run the application in a clean environment to confirm no residual QRCoder or ZXing.Net references cause binding failures
- Validate that the IronBarcode licence key is loaded correctly in production and staging environments

## Key Benefits of Migrating to IronBarcode

**Single API for All Barcode Operations:** After migration, every barcode task — generation, reading, PDF stamping — uses the same `IronBarCode` namespace and the same entry points. Developers joining the project learn one API rather than three. Code review across the barcode subsystem becomes consistent. Documentation lookup is centralised.

**Barcode Reading Without a Second Library:** `BarcodeReader.Read` handles image files, PDF pages, streams, and in-memory bitmaps with automatic format detection. The reading capability that previously required ZXing.Net — with its format specification requirements and thread-safety considerations — is available immediately through a single method call. The [barcode reading from images guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) covers multi-barcode detection, image preprocessing, and PDF page scanning.

**Format Expansion Without New Dependencies:** Adding Code 128, EAN-13, DataMatrix, PDF417, or any other supported format after migration requires only a different `BarcodeEncoding` parameter value. There are no new packages to install, no new APIs to learn, and no version compatibility issues to manage. New format requirements become a one-line change rather than a new library integration.

**Simplified QR Code Customisation:** Logo embedding, colour changes, and quiet zone control are available as named methods on the `QRCodeWriter` result. The `System.Drawing.Bitmap` dependency that QRCoder requires for logo embedding is replaced by a simple file path string. The [QR code style customisation guide](https://ironsoftware.com/csharp/barcode/how-to/customize-qr-code-style/) documents all available customisation options.

**PDF Integration Built In:** Barcodes can be read from incoming PDF documents and stamped into existing PDF files without adding a PDF library. Document-processing workflows that required a separate PDF dependency now have barcode and PDF functionality consolidated.

**Reduced Long-Term Maintenance Overhead:** Moving from three or four independent packages to one eliminates the compounding overhead of tracking multiple release cycles, resolving inter-library version conflicts, and updating multiple test suites when .NET versions advance. The maintenance cost of the barcode subsystem reduces to a single package dependency.
