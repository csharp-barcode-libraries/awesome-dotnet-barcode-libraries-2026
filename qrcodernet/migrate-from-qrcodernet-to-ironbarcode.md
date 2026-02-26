# Migrate from QRCoder to IronBarcode in C#

Most QRCoder migrations are not driven by dissatisfaction with QRCoder itself. The library works well and its MIT license is genuinely free. They are driven by scope creep: a project that started with QR codes for a marketing campaign now needs Code128 for shipping labels and the ability to scan incoming barcodes. At that point, the choice becomes maintaining three separate libraries or consolidating into one.

This guide walks through the replacement of QRCoder with IronBarcode — covering package installation, namespace changes, code translation, and the new capabilities that become available once the migration is done.

---

## Table of Contents

1. [Why Migrate](#why-migrate)
2. [Quick Start: Three Steps](#quick-start-three-steps)
3. [Code Migration Examples](#code-migration-examples)
4. [API Mapping Reference](#api-mapping-reference)
5. [Migration Checklist](#migration-checklist)
6. [Related Comparisons](#related-comparisons)

---

## Why Migrate

QRCoder generates QR codes. That is its complete scope, and it executes that scope well. Migration makes sense when one or more of these scenarios applies:

**You need barcode formats beyond QR.** Code128 for shipping, EAN-13 for retail products, DataMatrix for pharmaceutical compliance, PDF417 for government documents — none of these are available in QRCoder. Each additional format means another library with another API.

**You need to read barcodes.** QRCoder has no decoding capability. A project that generates QR codes for outbound shipments and also needs to scan incoming codes already requires ZXing.Net or a similar library. IronBarcode's `BarcodeReader.Read` handles this without a separate package.

**You are maintaining multiple barcode libraries simultaneously.** The natural progression for a QRCoder-based project is: QRCoder for QR generation, NetBarcode for 1D formats, ZXing.Net for reading. Three libraries, three APIs, three upgrade cycles. Consolidating to one reduces that overhead considerably.

**You need PDF barcode support.** Extracting barcodes from PDFs, or embedding barcodes in PDF documents, falls outside QRCoder's capabilities entirely.

If none of these apply and your project genuinely only needs QR generation, QRCoder remains a solid choice. This migration is for projects where requirements have expanded past the QR boundary.

---

## Quick Start: Three Steps

### Step 1 — Remove QRCoder

```bash
dotnet remove package QRCoder
```

### Step 2 — Add IronBarcode

```bash
dotnet add package IronBarcode
```

### Step 3 — Update the using directives and add license initialization

Replace:

```csharp
using QRCoder;
```

With:

```csharp
using IronBarCode;
```

Add license initialization early in your application startup — `Program.cs`, `Startup.cs`, or a static constructor:

```csharp
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

A free trial key is available without registration. Production deployments require a purchased license.

---

## Code Migration Examples

### Basic QR Code Generation

The most common QRCoder pattern involves three objects: a `QRCodeGenerator`, a `QRCodeData` intermediate object, and a renderer like `PngByteQRCode`. IronBarcode collapses these into a single fluent call.

**Before (QRCoder):**

```csharp
using QRCoder;
using System.IO;

public class QRService
{
    private readonly QRCodeGenerator _generator = new QRCodeGenerator();

    public byte[] GenerateQR(string data)
    {
        var qrCodeData = _generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(10);
    }

    public void SaveQR(string data, string path)
    {
        var bytes = GenerateQR(data);
        File.WriteAllBytes(path, bytes);
    }
}
```

**After (IronBarcode):**

```csharp
using IronBarCode;

public class QRService
{
    public byte[] GenerateQR(string data)
    {
        return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
            .ResizeTo(200, 200)
            .ToPngBinaryData();
    }

    public void SaveQR(string data, string path)
    {
        BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
            .SaveAsPng(path);
    }
}
```

`BarcodeWriter` is a static class — no instance is needed. The `ResizeTo(width, height)` call replaces the module-size integer that QRCoder passes to `GetGraphic`. Where QRCoder used a pixel-per-module value (e.g., `GetGraphic(20)` producing a 20px-per-module image), IronBarcode takes absolute pixel dimensions.

### Saving to File vs. Returning Bytes

QRCoder separates these paths by renderer class. IronBarcode uses the same chain with different terminal methods.

**Before:**

```csharp
// To file
var qrCodeData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
var pngBytes = new PngByteQRCode(qrCodeData).GetGraphic(10);
File.WriteAllBytes("output.png", pngBytes);

// To bytes (in-memory)
byte[] bytes = new PngByteQRCode(qrCodeData).GetGraphic(10);
```

**After:**

```csharp
// To file
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
    .SaveAsPng("output.png");

// To bytes (in-memory)
byte[] bytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
    .ToPngBinaryData();
```

### SVG Output

**Before:**

```csharp
var qrCodeData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
string svgContent = new SvgQRCode(qrCodeData).GetGraphic(10);
File.WriteAllText("output.svg", svgContent);
```

**After:**

```csharp
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
    .SaveAsSvg("output.svg");
```

### Error Correction Level

QRCoder exposes the ECC level as `QRCodeGenerator.ECCLevel` (L, M, Q, H). IronBarcode uses `QRCodeWriter.QrErrorCorrectionLevel` when explicit control is needed, or applies a sensible default automatically.

**Before:**

```csharp
// Medium (default for most use cases)
generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);

// High (for QR codes with logos — logo occludes part of the code)
generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
```

**After:**

```csharp
// Default (equivalent to M)
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode);

// Explicit level via QRCodeWriter
var qr = QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);
qr.SaveAsPng("output.png");
```

### QR Code with Logo

Both libraries support logo embedding, but IronBarcode handles it as a first-class operation without requiring `System.Drawing` types directly.

**Before:**

```csharp
using QRCoder;
using System.Drawing;

// Must use ECCLevel.H so the logo doesn't break readability
var qrCodeData = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
var qrCode = new QRCode(qrCodeData);
var logoBitmap = new Bitmap("logo.png");
var qrBitmap = qrCode.GetGraphic(10, Color.Black, Color.White, logoBitmap);
qrBitmap.Save("qr-logo.png", System.Drawing.Imaging.ImageFormat.Png);
```

**After:**

```csharp
using IronBarCode;

// IronBarcode automatically uses high error correction when a logo is added
var qr = QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);
qr.AddBrandLogo("logo.png");
qr.SaveAsPng("qr-logo.png");
```

The [QR code style customization guide](https://ironsoftware.com/csharp/barcode/how-to/customize-qr-code-style/) covers logo sizing, color changes, and quiet zone adjustments in detail.

### WiFi QR Codes (PayloadGenerator Migration)

QRCoder's `PayloadGenerator` produces properly formatted strings for common QR code standards — WiFi credentials, vCards, calendar events, and more. These formats are public standards, which means the strings can be constructed directly when switching libraries.

**Before:**

```csharp
using QRCoder;

// WiFi
var wifi = new PayloadGenerator.WiFi("MyNetwork", "SecurePass123",
    PayloadGenerator.WiFi.Authentication.WPA);
var qrData = generator.CreateQrCode(wifi.ToString(), QRCodeGenerator.ECCLevel.M);
// wifi.ToString() → "WIFI:T:WPA;S:MyNetwork;P:SecurePass123;;"

// Contact (vCard)
var contact = new PayloadGenerator.ContactData(
    PayloadGenerator.ContactData.ContactOutputType.VCard4,
    firstname: "Jane",
    lastname: "Smith",
    email: "jane@example.com"
);
var contactQR = generator.CreateQrCode(contact.ToString(), QRCodeGenerator.ECCLevel.M);
```

**After:**

```csharp
using IronBarCode;

// WiFi — construct the standard format directly
string wifiData = "WIFI:T:WPA;S:MyNetwork;P:SecurePass123;;";
BarcodeWriter.CreateBarcode(wifiData, BarcodeEncoding.QRCode)
    .SaveAsPng("wifi.png");

// Contact — vCard format is a public standard
string vcard = "BEGIN:VCARD\nVERSION:4.0\nFN:Jane Smith\nEMAIL:jane@example.com\nEND:VCARD";
BarcodeWriter.CreateBarcode(vcard, BarcodeEncoding.QRCode)
    .SaveAsPng("contact.png");
```

If your codebase uses many `PayloadGenerator` types, consider writing small helper methods to wrap the string construction — the formatted output of each generator is documented in the QR code standard and straightforward to reproduce.

### Reading QR Codes (New Capability)

QRCoder has no reading API. If your project was previously pairing QRCoder with ZXing.Net for decoding, that dependency can also be removed after migrating to IronBarcode.

**Before (with ZXing.Net):**

```csharp
using ZXing;
using ZXing.Common;

var reader = new BarcodeReaderGeneric();
reader.Options = new DecodingOptions { PossibleFormats = new[] { BarcodeFormat.QR_CODE } };

using var bmp = new System.Drawing.Bitmap("qr.png");
var result = reader.Decode(bmp);
string text = result?.Text;
```

**After (IronBarcode):**

```csharp
using IronBarCode;

var results = BarcodeReader.Read("qr.png");
string text = results.FirstOrDefault()?.Text;
```

The same `BarcodeReader.Read` method works on image files, PDFs, streams, and `System.Drawing.Bitmap` objects. It detects multiple barcodes in a single image and returns all of them. The [barcode reading from images guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) covers filtering by format, adjusting image preprocessing, and handling noisy or low-resolution inputs.

### Adding New Formats After Migration

Once QRCoder is replaced, adding Code128, EAN-13, DataMatrix, or any other format requires no additional packages — just a different `BarcodeEncoding` value.

```csharp
using IronBarCode;

// Shipping label (Code128)
BarcodeWriter.CreateBarcode("SHIP-12345-US", BarcodeEncoding.Code128)
    .SaveAsPng("shipping-label.png");

// Retail product code (EAN-13)
BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
    .SaveAsPng("product-ean.png");

// Pharmaceutical (DataMatrix)
BarcodeWriter.CreateBarcode("LOT-ALPHA-001", BarcodeEncoding.DataMatrix)
    .SaveAsPng("pharma-label.png");

// Government ID (PDF417)
BarcodeWriter.CreateBarcode("ID-DATA-HERE", BarcodeEncoding.PDF417)
    .SaveAsPng("id-barcode.png");
```

All of these use the same `BarcodeWriter.CreateBarcode` entry point with the same fluent API — no new documentation to learn, no new dependency to manage.

---

## API Mapping Reference

| QRCoder | IronBarcode | Notes |
|---|---|---|
| `new QRCodeGenerator()` | Static — no instance needed | `BarcodeWriter` is a static class |
| `qrGenerator.CreateQrCode(data, ECCLevel.M)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)` | Direct replacement |
| `new PngByteQRCode(qrCodeData)` | Not needed — built in | Rendering is part of the chain |
| `qrCode.GetGraphic(20)` returns `byte[]` | `.ToPngBinaryData()` | Size via `.ResizeTo(w, h)` instead |
| `QRCodeGenerator.ECCLevel.L` | `QRCodeWriter.QrErrorCorrectionLevel.Low` | Or omit for auto |
| `QRCodeGenerator.ECCLevel.M` | `QRCodeWriter.QrErrorCorrectionLevel.Medium` (default) | Auto-applied if not specified |
| `QRCodeGenerator.ECCLevel.Q` | `QRCodeWriter.QrErrorCorrectionLevel.Quartile` | |
| `QRCodeGenerator.ECCLevel.H` | `QRCodeWriter.QrErrorCorrectionLevel.Highest` | Use when adding a logo |
| `new SvgQRCode(data).GetGraphic(10)` | `.SaveAsSvg(path)` | |
| `new QRCode(data).GetGraphic(...)` | `QRCodeWriter.CreateQrCode(data, size, level)` | Richer styling options |
| `PayloadGenerator.WiFi(...).ToString()` | `"WIFI:T:WPA;S:{ssid};P:{pass};;"` | Construct the standard string directly |
| No reading API | `BarcodeReader.Read(path)` | New capability |
| QR format only | 50+ formats via `BarcodeEncoding.*` | New capability |

---

## Migration Checklist

Use this list as a systematic grep-and-replace guide for finding all QRCoder usage in a codebase.

### Locate All QRCoder Code

Search for these patterns:

- `using QRCoder` — namespace imports to replace
- `new QRCodeGenerator()` — generator instances to remove
- `CreateQrCode(` — creation calls to translate
- `new PngByteQRCode(` — PNG renderer instantiations to remove
- `GetGraphic(` — rendering calls to replace with `.ToPngBinaryData()` or `.SaveAsPng()`
- `PayloadGenerator.WiFi` — WiFi payload helpers to replace with direct strings
- `QRCodeGenerator.ECCLevel` — ECC level references to translate

### Pre-Migration

- [ ] Run the grep patterns above across the entire solution
- [ ] Note which `PayloadGenerator` types are in use (WiFi, ContactData, CalendarEvent, etc.) and plan their string equivalents
- [ ] Identify any ZXing.Net usage that was paired with QRCoder for reading — this can be removed at the same time
- [ ] Obtain an IronBarcode license key (trial keys are available without registration)
- [ ] Confirm there are no other packages in the solution that depend on QRCoder transitively

### Migration

- [ ] `dotnet remove package QRCoder`
- [ ] Remove ZXing.Net if it was used only for barcode reading alongside QRCoder
- [ ] `dotnet add package IronBarcode`
- [ ] Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` to application startup
- [ ] Replace all `using QRCoder;` with `using IronBarCode;`
- [ ] Replace `new QRCodeGenerator()` instantiations — the static `BarcodeWriter` class needs no instance
- [ ] Replace `CreateQrCode(data, ECCLevel.M)` + `new PngByteQRCode(...)` + `.GetGraphic(n)` chains with `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode).ToPngBinaryData()`
- [ ] Replace `File.WriteAllBytes(path, bytes)` save patterns with `.SaveAsPng(path)` directly on the barcode
- [ ] Translate `PayloadGenerator.WiFi` and other helpers to direct format strings
- [ ] Replace any ZXing.Net `BarcodeReader.Decode(...)` calls with `BarcodeReader.Read(...)`

### Post-Migration

- [ ] Run the full test suite against QR generation output — scan generated codes to confirm they are readable
- [ ] Verify logo-embedded QR codes if applicable — confirm error correction level is `Highest`
- [ ] Test reading paths if ZXing.Net was also replaced
- [ ] Add any new format requirements (Code128, EAN-13, DataMatrix) now that the unified library is in place
- [ ] Remove any helper methods that were wrapping `PayloadGenerator` if they are no longer needed
- [ ] Update CI/CD configuration if the license key needs to be provided as an environment variable in build environments

---

QRCoder's scope ends at QR generation, and within that scope it performs reliably. The migration to IronBarcode is not about fixing something broken — it is about removing the ceiling that forces additional libraries into the project the moment any requirement reaches past a QR code. Once consolidated, the same API handles every barcode format the project will ever need.
