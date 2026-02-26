# Migrate from Spire.Barcode to IronBarcode

Every call to `BarcodeScanner.Scan()` in Spire.Barcode requires a `BarCodeType` argument. If your codebase has grown organically — new document sources, new supplier labels, new barcode formats added over time — there's a reasonable chance you've already built a type-guessing loop somewhere. This guide covers the mechanical steps to replace Spire.Barcode with IronBarcode, along with the API mapping you'll use to convert each pattern.

---

## Why Developers Migrate

The type specification requirement is the most common trigger for migration. Spire.Barcode's `Scan()` cannot auto-detect format without accuracy tradeoffs, which means unknown-format workflows require iterating through candidate types until something matches. That iteration grows as your format diversity grows.

The other recurring pressure point is PDF support. Spire.Barcode does not read barcodes directly from PDF files — that requires `Spire.PDF` as a separate package and license, plus manual page-and-image extraction code. Teams maintaining both libraries for a task that should be one method call eventually look for a simpler path.

The free tier evaluation problem also surfaces regularly. `FreeSpire.Barcode` generates watermarked output and scans with intentionally degraded performance. Because the free version doesn't represent the commercial product faithfully, teams sometimes commit to Spire.Barcode based on a limited evaluation and only discover the full constraint set after purchase.

IronBarcode resolves all three: auto-detection requires no type parameter, PDF reading is a single native call, and the trial runs at full speed with full symbologies.

---

## Quick Start: Three Steps

### Step 1 — Remove Spire packages

```bash
# Remove whichever variant you're using
dotnet remove package FreeSpire.Barcode
# or
dotnet remove package Spire.Barcode

# If Spire.PDF was installed only for barcode extraction, remove it too
dotnet remove package Spire.PDF
```

If `Spire.PDF` is used for other document operations beyond barcode extraction, keep it and only remove the barcode-related code paths.

### Step 2 — Install IronBarcode

```bash
dotnet add package IronBarcode
```

### Step 3 — Replace namespace and license call

```csharp
// Remove:
using Spire.Barcode;

// Add:
using IronBarCode;
```

License activation changes from:

```csharp
// FreeSpire registration key
Spire.Barcode.BarcodeSettings.ApplyKey("your-free-key");

// or commercial Spire.Barcode license
Spire.License.LicenseProvider.SetLicenseKey("your-commercial-key");
```

To a single property assignment, set once at application startup. See the [IronBarcode licensing page](https://ironsoftware.com/csharp/barcode/licensing/) for key formats and deployment options:

```csharp
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-LICENSE-KEY";
```

This also works with environment variables for Docker and CI deployments:

```csharp
IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

---

## Code Migration Examples

### Reading Barcodes — Known Format

The most common Spire.Barcode read pattern specifies a type explicitly. The IronBarcode equivalent removes the type parameter entirely:

**Before:**
```csharp
using Spire.Barcode;

BarcodeScanner scanner = new BarcodeScanner();
string[] results = scanner.Scan("barcode.png", BarCodeType.Code128);

foreach (string result in results)
{
    Console.WriteLine(result);
}
```

**After:**
```csharp
using IronBarCode;

var results = BarcodeReader.Read("barcode.png");

foreach (var result in results)
{
    Console.WriteLine($"{result.BarcodeType}: {result.Value}");
}
```

The result objects in IronBarcode include the detected `BarcodeType`, so if your downstream code needs to know the format, it's available without maintaining a separate type variable.

### Reading Barcodes — Unknown or Mixed Formats

If your codebase has a type-guessing loop, the entire loop collapses to one line. [IronBarcode's image reading](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) handles all 50+ symbologies in a single pass:

**Before:**
```csharp
using Spire.Barcode;

BarCodeType[] candidates = new[]
{
    BarCodeType.Code128,
    BarCodeType.QRCode,
    BarCodeType.DataMatrix,
    BarCodeType.EAN13,
    BarCodeType.PDF417
};

BarcodeScanner scanner = new BarcodeScanner();
List<string> allResults = new List<string>();

foreach (BarCodeType type in candidates)
{
    string[] found = scanner.Scan("barcode.png", type);
    allResults.AddRange(found);
}
```

**After:**
```csharp
using IronBarCode;

// Single call replaces the entire loop
var results = BarcodeReader.Read("barcode.png");
var allValues = results.Select(r => r.Value).ToList();
```

You can tune the balance between speed and thoroughness using `BarcodeReaderOptions`. The [reading speed options guide](https://ironsoftware.com/csharp/barcode/how-to/reading-speed-options/) covers when each `ReadingSpeed` value is appropriate:

```csharp
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};
var results = BarcodeReader.Read("barcode.png", options);
```

### Generating Barcodes

Spire.Barcode uses a settings-object pattern with a separate generator class. IronBarcode replaces this with a static factory method:

**Before:**
```csharp
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

**After:**
```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");

// With sizing:
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("barcode.png");
```

For QR codes:

**Before:**
```csharp
BarcodeSettings settings = new BarcodeSettings();
settings.Type = BarCodeType.QRCode;
settings.Data = "https://example.com";

BarCodeGenerator generator = new BarCodeGenerator(settings);
Image qrImage = generator.GenerateImage();
qrImage.Save("qr.png", ImageFormat.Png);
```

**After:**
```csharp
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");
```

### Reading from PDF

If you were using `Spire.PDF` to extract barcode images from PDF pages before scanning them, replace the entire block with a single native call. [IronBarcode reads PDF documents directly](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) without any additional library:

**Before:**
```csharp
using Spire.Pdf;
using Spire.Barcode;

var pdf = new PdfDocument();
pdf.LoadFromFile("document.pdf");

var scanner = new BarcodeScanner();
var allValues = new List<string>();

foreach (PdfPageBase page in pdf.Pages)
{
    var images = page.ExtractImages();
    foreach (var image in images)
    {
        string[] found = scanner.Scan(image, BarCodeType.QRCode);
        allValues.AddRange(found);
    }
}
```

**After:**
```csharp
using IronBarCode;

var results = BarcodeReader.Read("document.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.BarcodeType} = {barcode.Value}");
}
```

Remove the `Spire.PDF` package reference and all `using Spire.Pdf;` imports once the reading code is replaced.

### Handling the Return Type Change

Spire.Barcode's `Scan()` returns `string[]`. IronBarcode returns a `BarcodeResults` collection. If your existing code assigns to `string[]` or passes results to methods expecting that type, update those call sites:

```csharp
// If you need a plain string array
string[] texts = BarcodeReader.Read("barcode.png")
    .Select(r => r.Value)
    .ToArray();

// If you need the first result only
string first = BarcodeReader.Read("barcode.png").First()?.Value;
```

---

## API Mapping Reference

| Spire.Barcode | IronBarcode | Notes |
|---|---|---|
| `Spire.Barcode.BarcodeSettings.ApplyKey("key")` | `IronBarCode.License.LicenseKey = "key"` | Set once at startup |
| `Spire.License.LicenseProvider.SetLicenseKey("key")` | `IronBarCode.License.LicenseKey = "key"` | Same single-line replacement |
| `new BarcodeSettings()` | Parameters to `CreateBarcode()` | No settings object needed |
| `settings.Type = BarCodeType.Code128` | `BarcodeEncoding.Code128` | Passed as second parameter |
| `settings.Data = "12345678"` | First parameter of `CreateBarcode()` | |
| `new BarCodeGenerator(settings).GenerateImage()` | `BarcodeWriter.CreateBarcode(...)` | Static, no generator instance |
| `generator.GenerateImage()` + `image.Save()` | `.SaveAsPng(path)` | Format-specific save methods |
| `new BarcodeScanner()` | Static `BarcodeReader.Read()` | No scanner instance needed |
| `scanner.Scan(path, BarCodeType.Code128)` | `BarcodeReader.Read(path)` | Type parameter removed |
| `BarCodeType.Code128` (required) | Auto-detected | No equivalent needed |
| `string[] results` | `BarcodeResults results` | Use `.Value` or `.Select(r => r.Value)` |
| Needs `Spire.PDF` for PDF reading | `BarcodeReader.Read("doc.pdf")` | Native, no additional package |

---

## Migration Checklist

Use these grep patterns to find every Spire.Barcode usage in your codebase:

```
using Spire.Barcode
BarcodeSettings
BarCodeGenerator
BarcodeScanner
.Scan(
BarCodeType.
ApplyKey
Spire.PDF
```

Work through each match:

**Package and namespace cleanup**
- [ ] `FreeSpire.Barcode` or `Spire.Barcode` removed from `.csproj`
- [ ] `Spire.PDF` removed from `.csproj` (if barcode extraction was the only use)
- [ ] All `using Spire.Barcode;` replaced with `using IronBarCode;`
- [ ] All `using Spire.Pdf;` removed (if no longer needed)
- [ ] License call replaced: `ApplyKey` / `SetLicenseKey` → `IronBarCode.License.LicenseKey`

**Reading code**
- [ ] All `new BarcodeScanner()` instances removed
- [ ] All `.Scan(path, BarCodeType.X)` replaced with `BarcodeReader.Read(path)`
- [ ] Type-guessing loops removed
- [ ] PDF page extraction code removed and replaced with direct `BarcodeReader.Read("file.pdf")`
- [ ] `string[]` return type references updated to use `.Value` or `.Select(r => r.Value)`

**Generation code**
- [ ] `new BarcodeSettings()` blocks replaced with `BarcodeWriter.CreateBarcode(data, encoding)`
- [ ] `BarCodeGenerator` instances removed
- [ ] `.GenerateImage()` + `image.Save()` calls replaced with `.SaveAsPng()` or `.SaveAsJpeg()`
- [ ] `BarCodeType.X` enum values replaced with `BarcodeEncoding.X`

**Verification**
- [ ] All existing tests pass
- [ ] Batch processing throughput measured and meets requirements
- [ ] All previously scanned barcode formats confirmed working
- [ ] Generated barcode output quality confirmed
- [ ] CI/CD environment variable for license key set

---

## The Format Detection Problem, Resolved

The format-guessing loop that grows with every new barcode type entering your workflow is the thing that disappears after this migration. `BarcodeReader.Read()` with no type argument handles the full symbology set in a single pass. There is no list to maintain, no silent miss when a supplier switches from Code128 to DataMatrix, and no linear performance penalty as your type candidate list grows.

If PDF extraction was also driving complexity, that simplification compounds: two libraries and manual page iteration reduce to one package and one method call.
