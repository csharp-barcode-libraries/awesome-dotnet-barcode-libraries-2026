# Migrate from Neodynamic Barcode to IronBarcode

If you reached this page after discovering that Neodynamic's Barcode Reader SDK does not support QR codes, you are not alone. That is the most common trigger for this migration. The second most common is managing two separate packages with two separate license keys for what should be a single capability.

This guide covers the complete migration from `Neodynamic.SDK.Barcode` and `Neodynamic.SDK.BarcodeReader` to IronBarcode, including package removal, API translation, and gaining 2D reading capability your application previously could not have.

---

## Why Teams Migrate

Neodynamic's split SDK model works for a narrow set of requirements. When those requirements grow — and they usually do — the model shows its edges.

**The reading gap.** Neodynamic Barcode Professional can generate QR codes, DataMatrix, PDF417, and Aztec. None of those formats can be read back by the companion Barcode Reader SDK. Teams that built product labelling workflows with QR codes discovered this when they tried to build the scanning side of the same system. IronBarcode handles [creating and reading 2D barcodes](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/) in a single library.

**Two licenses, two packages, one job.** Every project using both Neodynamic SDKs carries two `PackageReference` entries, two `using` statements, two license blocks, and two separate update cycles. None of that overhead delivers any capability that a single unified SDK cannot provide.

**The third-library problem.** Teams that need both Neodynamic SDKs and 2D reading are effectively maintaining three barcode-related dependencies: the generator, the 1D-only reader, and whatever library they added for QR support. IronBarcode replaces all three.

The migration itself is straightforward. The API patterns are similar enough in intent that the translation is mechanical, and switching to IronBarcode does not require rearchitecting anything — it replaces calls line by line.

---

## Quick Start: Three Steps

### Step 1 — Remove Neodynamic packages

```bash
dotnet remove package Neodynamic.SDK.Barcode
dotnet remove package Neodynamic.SDK.BarcodeReader
```

### Step 2 — Add IronBarcode

```bash
dotnet add package IronBarcode
```

### Step 3 — Replace namespace imports and license configuration

Remove both `using` statements and both license blocks. Replace them with a single import and a single key:

**Before:**
```csharp
using Neodynamic.SDK.Barcode;
using Neodynamic.SDK.BarcodeReader;

BarcodeInfo.LicenseOwner = "Your Company";
BarcodeInfo.LicenseKey = "GEN-LICENSE-KEY";

Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseOwner = "Your Company";
Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseKey = "READ-LICENSE-KEY";
```

**After:**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

---

## Code Migration Examples

### Example 1 — Generating a Code 128 Barcode

The Neodynamic generation API uses a `BarcodeInfo` instance with property assignment, followed by `GetImage()` and a `System.Drawing` save call:

```csharp
// Neodynamic: instance-based generation
using Neodynamic.SDK.Barcode;

public void GenerateCode128(string data, string outputPath)
{
    var barcode = new BarcodeInfo();
    barcode.Value = data;
    barcode.Symbology = Symbology.Code128;
    barcode.TextAlign = BarcodeTextAlignment.BelowCenter;

    System.Drawing.Image image = barcode.GetImage();
    image.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
}
```

IronBarcode replaces this with a fluent static call. The `data` and encoding become parameters to `CreateBarcode`, and the output format is expressed as a method name rather than an enum:

```csharp
// IronBarcode: fluent static generation
using IronBarCode;

public void GenerateCode128(string data, string outputPath)
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .SaveAsPng(outputPath);
}
```

The `System.Drawing.Imaging` import is no longer needed. The `BarcodeInfo` instance and its property assignments collapse into a single expression. IronBarcode's guide on [generating 1D barcodes](https://ironsoftware.com/csharp/barcode/how-to/create-1d-barcodes/) covers additional options for sizing, DPI, and annotation if you need to match Neodynamic's output exactly.

---

### Example 2 — Generating a QR Code

Neodynamic Barcode Professional can generate QR codes. The translation here changes only the API surface:

```csharp
// Neodynamic: QR code generation
using Neodynamic.SDK.Barcode;

public void GenerateQrCode(string data, string outputPath)
{
    var barcode = new BarcodeInfo();
    barcode.Value = data;
    barcode.Symbology = Symbology.QRCode;
    barcode.QRCodeECL = QRCodeErrorCorrectionLevel.H;

    barcode.GetImage().Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
}
```

```csharp
// IronBarcode: QR code generation
using IronBarCode;

public void GenerateQrCode(string data, string outputPath)
{
    QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest)
        .SaveAsPng(outputPath);
}
```

The important difference here is not the generation code — it is that after this migration, the same application can also read back the QR codes it generates. With Neodynamic, those two operations required different libraries.

---

### Example 3 — Reading 1D Barcodes

Neodynamic's reader takes a `System.Drawing.Bitmap` and returns results through a collection where each item has a `Value` property:

```csharp
// Neodynamic: 1D reading only
using Neodynamic.SDK.BarcodeReader;
using System.Drawing;

public string[] ReadBarcodes(string imagePath)
{
    BarcodeReader.LicenseOwner = "Your Company";
    BarcodeReader.LicenseKey = "YOUR-READER-KEY";

    using var bitmap = new Bitmap(imagePath);
    var results = BarcodeReader.Read(bitmap);

    return results?.Select(r => r.Value).ToArray() ?? Array.Empty<string>();
}
```

IronBarcode accepts the file path directly, removing the `Bitmap` construction step. The result property is also `Value`, so the downstream code does not change. The full capabilities of [reading barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) in IronBarcode include automatic format detection across all 1D and 2D symbologies:

```csharp
// IronBarcode: reads 1D and 2D with the same call
using IronBarCode;

public string[] ReadBarcodes(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.Select(r => r.Value).ToArray();
}
```

The license configuration is gone from this method because it is set once at application startup rather than per-call.

---

### Example 4 — QR Code Reading (Previously Impossible)

This is the migration change with the most immediate functional impact. The Neodynamic Reader throws or silently returns nothing for any 2D barcode. After migrating to IronBarcode, no code changes are needed for the reading side — the same call that reads a Code 128 barcode also reads a QR code:

```csharp
// Neodynamic: QR reading is not possible
// This method existed in many codebases as a known limitation placeholder
public string ReadQrCode(string imagePath)
{
    throw new NotSupportedException(
        "Neodynamic Barcode Reader does not support QR codes");
}
```

```csharp
// IronBarcode: QR reading works without any special handling
using IronBarCode;

public string ReadQrCode(string imagePath)
{
    var result = BarcodeReader.Read(imagePath).FirstOrDefault();
    return result?.Value;
}
```

For teams that had `throw new NotSupportedException(...)` in their codebase as a temporary placeholder, this is a removal, not a replacement. The method body becomes a standard read call.

---

### Example 5 — Complete Service Class Migration

Before migration, a service that handles both generation and 1D reading carries the overhead of two license configurations and two separate API surfaces:

```csharp
// Before: Neodynamic split SDK service
using Neodynamic.SDK.Barcode;
using Neodynamic.SDK.BarcodeReader;
using System.Drawing;

public class BarcodeService
{
    public BarcodeService()
    {
        // Generation license
        BarcodeInfo.LicenseOwner = "Company";
        BarcodeInfo.LicenseKey = "GEN-KEY";

        // Reader license (separate purchase)
        Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseOwner = "Company";
        Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseKey = "READ-KEY";
    }

    public void GenerateCode128(string data, string outputPath)
    {
        var barcode = new BarcodeInfo();
        barcode.Value = data;
        barcode.Symbology = Symbology.Code128;
        barcode.GetImage().Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
    }

    public string ReadBarcode(string imagePath)
    {
        using var bitmap = new Bitmap(imagePath);
        var results = Neodynamic.SDK.BarcodeReader.BarcodeReader.Read(bitmap);
        return results?.FirstOrDefault()?.Value;
    }

    public string ReadQrCode(string imagePath)
    {
        // Cannot do this — Neodynamic Reader does not support QR codes
        throw new NotSupportedException("Neodynamic cannot read QR codes");
    }
}
```

After migration:

```csharp
// After: IronBarcode unified service
using IronBarCode;

public class BarcodeService
{
    static BarcodeService()
    {
        // Single license covers generation and reading, 1D and 2D
        IronBarCode.License.LicenseKey = "YOUR-KEY";
    }

    public void GenerateCode128(string data, string outputPath)
    {
        BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
            .SaveAsPng(outputPath);
    }

    public string ReadBarcode(string imagePath)
    {
        var result = BarcodeReader.Read(imagePath).FirstOrDefault();
        return result?.Value;
    }

    public string ReadQrCode(string imagePath)
    {
        // Now works — same API, format detected automatically
        var result = BarcodeReader.Read(imagePath).FirstOrDefault();
        return result?.Value;
    }
}
```

The `ReadBarcode` and `ReadQrCode` methods now share the same implementation because IronBarcode detects format automatically across its [full list of supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/). If your codebase had separate reading methods for different formats, you can consolidate them.

---

### Example 6 — Reading from PDFs (New Capability)

If your application processes PDF documents that contain barcodes, IronBarcode handles PDF input natively. This was not possible with Neodynamic Reader without first extracting images:

```csharp
using IronBarCode;

// Read all barcodes from a PDF — works across all pages, all formats
var pdfResults = BarcodeReader.Read("invoice.pdf");

foreach (var result in pdfResults)
{
    Console.WriteLine($"{result.Value} ({result.Format}) — page {result.PageNumber}");
}
```

No image extraction, no intermediate files, no additional packages.

---

## API Mapping Reference

| Neodynamic | IronBarcode | Notes |
|---|---|---|
| `BarcodeInfo.LicenseOwner = "..."` | `IronBarCode.License.LicenseKey = "key"` | Merged into single key |
| `BarcodeInfo.LicenseKey = "..."` | (part of single key above) | No separate owner field |
| `Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseOwner = "..."` | (removed) | Not needed |
| `Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseKey = "..."` | (removed) | Not needed |
| `new BarcodeInfo()` | `BarcodeWriter.CreateBarcode(data, encoding)` | Static, no instance |
| `barcode.Value = data` | First parameter of `CreateBarcode` | Moved to constructor |
| `barcode.Symbology = Symbology.Code128` | `BarcodeEncoding.Code128` | Second parameter |
| `barcode.GetImage().Save(path, ImageFormat.Png)` | `.SaveAsPng(path)` | Fluent, no ImageFormat enum |
| `BarcodeReader.Read(bitmap)` (1D only) | `BarcodeReader.Read(path)` (50+ formats) | Path instead of Bitmap |
| `result.Value` | `result.Value` | Same property name |
| `Symbology.QRCode` (generate only) | `BarcodeEncoding.QRCode` (generate + read) | Full round-trip |
| `throw new NotSupportedException(...)` for QR reading | `BarcodeReader.Read(imagePath)` | Remove and replace |

---

## Migration Checklist

Use these grep terms to find every location in your codebase that needs updating.

**Find all Neodynamic references:**

```
using Neodynamic.SDK.Barcode
using Neodynamic.SDK.BarcodeReader
BarcodeInfo.LicenseOwner
BarcodeInfo.LicenseKey
new BarcodeInfo()
barcode.GetImage()
Symbology.Code128
Symbology.QRCode
BarcodeReader.LicenseOwner
BarcodeReader.LicenseKey
```

**Work through each category:**

- [ ] Remove `Neodynamic.SDK.Barcode` and `Neodynamic.SDK.BarcodeReader` from `.csproj`
- [ ] Add `IronBarcode` to `.csproj`
- [ ] Remove both `using Neodynamic.SDK.Barcode` and `using Neodynamic.SDK.BarcodeReader` imports
- [ ] Add `using IronBarCode` where needed
- [ ] Replace dual `LicenseOwner` / `LicenseKey` configuration blocks with `IronBarCode.License.LicenseKey = "key"` — set once at startup
- [ ] Replace `new BarcodeInfo()` + property assignments + `barcode.GetImage().Save(...)` with `BarcodeWriter.CreateBarcode(data, encoding).SaveAsPng(path)`
- [ ] Replace `BarcodeReader.Read(bitmap)` calls — remove the `Bitmap` construction, pass the file path directly
- [ ] Remove any `throw new NotSupportedException(...)` placeholders for QR/2D reading — replace with standard `BarcodeReader.Read(path)` calls
- [ ] Test all generation paths — Code 128, QR Code, any other symbologies in use
- [ ] Test 1D reading paths — confirm values are identical
- [ ] Test QR code reading — this is likely the new capability you are gaining
- [ ] Remove old Neodynamic license keys from configuration files and environment variables

If your project added ZXing.Net or another library specifically to cover the 2D reading gap, that dependency can be removed after migration. See the [IronBarcode licensing page](https://ironsoftware.com/csharp/barcode/licensing/) if you need to evaluate pricing before committing to the switch.
