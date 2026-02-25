# Migrating from Aspose.BarCode to IronBarcode

Two things change in this migration: the reading pattern (no more `DecodeType` specification) and the pricing model (perpetual option replaces annual subscription). Everything else is mostly mechanical — property names, constructor calls, namespace imports. None of it is architecturally complex, but there is enough of it across a real codebase that it pays to be systematic.

## Quick Start

### Step 1: Swap the NuGet Packages

```bash
dotnet remove package Aspose.BarCode
dotnet add package IronBarcode
```

If you added Aspose.PDF solely to support barcode extraction from PDFs, remove it too:

```bash
dotnet remove package Aspose.PDF
```

IronBarcode reads barcodes directly from PDF files with no additional dependency.

### Step 2: Replace the Namespaces

```csharp
// Remove
using Aspose.BarCode.Generation;
using Aspose.BarCode.BarCodeRecognition;

// Add
using IronBarCode;
```

### Step 3: Set the License Key

```csharp
// NuGet: dotnet add package IronBarcode
// Add once at application startup — Program.cs or your DI configuration
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

Remove the Aspose license initialization:

```csharp
// Remove this
var license = new Aspose.BarCode.License();
license.SetLicense("Aspose.BarCode.lic");

// Or if using metered licensing, remove this
var metered = new Aspose.BarCode.Metered();
metered.SetMeteredKey("publicKey", "privateKey");
```

In production, read the IronBarcode key from your secrets manager or environment:

```csharp
IronBarCode.License.LicenseKey =
    Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE")
    ?? throw new InvalidOperationException("IronBarcode license key not configured");
```

## Code Migration Examples

### Barcode Generation

Aspose.BarCode uses a `BarcodeGenerator` class with a `Parameters` hierarchy for configuration. IronBarcode uses a static factory method with a fluent API for customization.

**Before — Aspose.BarCode generation:**

```csharp
using Aspose.BarCode.Generation;

var generator = new BarcodeGenerator(EncodeTypes.Code128, "ITEM-12345");
generator.Parameters.Barcode.XDimension.Pixels = 2;
generator.Parameters.Barcode.BarHeight.Pixels = 100;
generator.Parameters.Barcode.CodeTextParameters.Location = CodeLocation.Below;
generator.Save("barcode.png", BarCodeImageFormat.Png);
```

**After — IronBarcode generation:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("barcode.png");
```

The `XDimension`, `BarHeight`, and `CodeTextParameters.Location` settings in Aspose.BarCode configure dimensions and text placement. IronBarcode's `ResizeTo(width, height)` covers the dimension side. Default text placement is handled automatically and looks correct for most uses — if you need fine-grained control, IronBarcode provides additional fluent methods.

To get bytes instead of saving to disk (common for API responses and database storage):

```csharp
using IronBarCode;

byte[] pngBytes = BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

### Reading with a Known Format

When you know the barcode format, Aspose.BarCode lets you pass the decode type to the constructor. IronBarcode uses the same one-liner regardless of whether you know the format — it auto-detects.

**Before — Aspose.BarCode reading with known format:**

```csharp
using Aspose.BarCode.BarCodeRecognition;

public string ReadCode128(string imagePath)
{
    using var reader = new BarCodeReader(imagePath, DecodeType.Code128);
    foreach (var result in reader.ReadBarCodes())
    {
        return result.CodeText;
    }
    return null;
}
```

**After — IronBarcode reading (auto-detects format):**

```csharp
using IronBarCode;

public string ReadBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value;
}
```

The `using var reader` pattern disappears entirely. `BarcodeReader.Read` is a static method that returns a result collection directly — no disposable object to manage. If the image contains a Code 128, you get a Code 128 result. The format is in `result.Format` if you need it.

### Reading with Unknown Format

This is where the migration saves the most operational overhead. Aspose.BarCode's `DecodeType.AllSupportedTypes` is the fallback for format-unknown scenarios; IronBarcode uses the same API call for everything.

**Before — Aspose.BarCode with AllSupportedTypes:**

```csharp
using Aspose.BarCode.BarCodeRecognition;

public List<string> ReadUnknownFormat(string imagePath)
{
    var values = new List<string>();

    // AllSupportedTypes is slower — exhaustive scan through all decode types
    using var reader = new BarCodeReader(imagePath);
    reader.SetBarCodeReadType(DecodeType.AllSupportedTypes);

    foreach (var result in reader.ReadBarCodes())
    {
        values.Add(result.CodeText);
    }

    return values;
}
```

**After — IronBarcode (same call, always auto-detects):**

```csharp
using IronBarCode;

public List<string> ReadBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.Select(r => r.Value).ToList();
}
```

There is no "AllSupportedTypes" equivalent in IronBarcode because it is not needed. The auto-detection runs the same algorithm regardless of format. To tune performance versus accuracy, use `ReadingSpeed`:

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Faster,        // High throughput, clean images
    // Speed = ReadingSpeed.Balanced,   // Default
    // Speed = ReadingSpeed.Detailed,   // Low-quality or damaged images
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};

var results = BarcodeReader.Read(imagePath, options);
```

### Reading Barcodes from PDFs

This is typically the most impactful part of the migration for teams that handle PDF documents. If you were using Aspose.PDF to render pages before scanning with Aspose.BarCode, that entire pipeline collapses into one IronBarcode call.

**Before — Aspose.PDF + Aspose.BarCode two-package workflow:**

```csharp
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.BarCode.BarCodeRecognition;
using System.IO;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    // Step 1: Load PDF with Aspose.PDF (separate license required)
    var pdfDocument = new Aspose.Pdf.Document(pdfPath);
    var resolution = new Resolution(300);
    var device = new PngDevice(resolution);
    var barcodeValues = new List<string>();

    for (int pageNum = 1; pageNum <= pdfDocument.Pages.Count; pageNum++)
    {
        using var pageStream = new MemoryStream();

        // Step 2: Render each PDF page to an image
        device.Process(pdfDocument.Pages[pageNum], pageStream);
        pageStream.Seek(0, SeekOrigin.Begin);

        // Step 3: Scan the rendered image for barcodes
        using var reader = new BarCodeReader(pageStream, DecodeType.AllSupportedTypes);
        foreach (var result in reader.ReadBarCodes())
        {
            barcodeValues.Add(result.CodeText);
        }
    }

    return barcodeValues;
}
```

**After — IronBarcode native PDF reading:**

```csharp
using IronBarCode;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    var results = BarcodeReader.Read(pdfPath);
    return results.Select(r => r.Value).ToList();
}
```

If you need page context:

```csharp
using IronBarCode;

public Dictionary<int, List<string>> ReadBarcodesFromPdfByPage(string pdfPath)
{
    var results = BarcodeReader.Read(pdfPath);
    return results
        .GroupBy(r => r.PageNumber)
        .ToDictionary(
            g => g.Key,
            g => g.Select(r => r.Value).ToList());
}
```

Remove `Aspose.PDF` from the project if it was only there for this use case:

```bash
dotnet remove package Aspose.PDF
```

### QR Code Generation

**Before — Aspose.BarCode QR generation:**

```csharp
using Aspose.BarCode.Generation;
using System.Drawing;

var generator = new BarcodeGenerator(EncodeTypes.QR, "https://example.com");
generator.Parameters.Barcode.QR.QrEncodeMode = QREncodeMode.Auto;
generator.Parameters.Barcode.QR.QrErrorLevel = QRErrorLevel.LevelH;
generator.Parameters.Barcode.XDimension.Pixels = 10;
generator.Parameters.Barcode.BarColor = Color.DarkBlue;
generator.Parameters.BackColor = Color.White;
generator.Save("qr.png", BarCodeImageFormat.Png);
```

**After — IronBarcode QR generation:**

```csharp
using IronBarCode;
using IronSoftware.Drawing;

// Basic QR code
QRCodeWriter.CreateQrCode("https://example.com", 500)
    .SaveAsPng("qr.png");

// With error correction and color
QRCodeWriter.CreateQrCode(
    "https://example.com",
    500,
    QRCodeWriter.QrErrorCorrectionLevel.Highest)
    .ChangeBarCodeColor(Color.DarkBlue)
    .SaveAsPng("qr.png");

// With brand logo (Aspose required manual GDI+ overlay for this)
QRCodeWriter.CreateQrCode("https://example.com", 500)
    .AddBrandLogo("logo.png")
    .SaveAsPng("qr-branded.png");
```

The Aspose approach for adding a logo to a QR code requires generating the barcode image, then using `System.Drawing.Graphics` to manually composite the logo into the center. IronBarcode's `AddBrandLogo` handles that internally and automatically selects appropriate error correction.

### Batch Processing

**Before — Aspose.BarCode batch with per-instance readers:**

```csharp
using Aspose.BarCode.BarCodeRecognition;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public Dictionary<string, List<string>> ProcessBatch(string[] imagePaths)
{
    var results = new ConcurrentDictionary<string, List<string>>();

    Parallel.ForEach(imagePaths,
        new ParallelOptions { MaxDegreeOfParallelism = 4 },
        imagePath =>
        {
            var barcodes = new List<string>();

            // Each parallel invocation needs its own BarCodeReader instance
            using var reader = new BarCodeReader(imagePath,
                DecodeType.Code128,
                DecodeType.QR,
                DecodeType.EAN13,
                DecodeType.DataMatrix);

            foreach (var result in reader.ReadBarCodes())
            {
                barcodes.Add(result.CodeText);
            }

            results[imagePath] = barcodes;
        });

    return new Dictionary<string, List<string>>(results);
}
```

**After — IronBarcode parallel batch:**

```csharp
using IronBarCode;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public Dictionary<string, List<string>> ProcessBatch(string[] imagePaths)
{
    var results = new ConcurrentDictionary<string, List<string>>();

    Parallel.ForEach(imagePaths, file =>
    {
        var r = BarcodeReader.Read(file);
        results[file] = r.Select(b => b.Value).ToList();
    });

    return new Dictionary<string, List<string>>(results);
}
```

Because `BarcodeReader.Read` is a stateless static method, there is no need to construct a new reader per thread. The `using var reader` pattern disappears, as does the explicit `DecodeType` list. Formats are detected automatically.

## Common Migration Issues

### result.CodeText to result.Value

This is the most common compile error after swapping packages. Every place you access `result.CodeText` needs to become `result.Value`.

```bash
grep -r "\.CodeText" --include="*.cs" .
```

Replace all occurrences: `result.CodeText` becomes `result.Value`.

### result.CodeTypeName to result.Format.ToString()

If you use the format name string (for logging, display, or routing logic):

```bash
grep -r "\.CodeTypeName" --include="*.cs" .
```

Replace: `result.CodeTypeName` becomes `result.Format.ToString()`.

The `result.Format` property is a `BarcodeEncoding` enum value, which gives you a typed comparison if you need it:

```csharp
if (result.Format == BarcodeEncoding.QRCode)
{
    // Handle QR code specifically
}
```

### Remove DecodeType Hardcoding Throughout the Codebase

Search for all `DecodeType.` usages:

```bash
grep -r "DecodeType\." --include="*.cs" .
```

Every one of these can be removed. IronBarcode always auto-detects. If a specific `DecodeType` was listed to improve performance on a known format, `ReadingSpeed.Faster` in `BarcodeReaderOptions` provides a similar benefit without format knowledge requirements:

```csharp
var options = new BarcodeReaderOptions { Speed = ReadingSpeed.Faster };
var results = BarcodeReader.Read(imagePath, options);
```

### Remove EncodeTypes References in Generation Code

```bash
grep -r "EncodeTypes\." --include="*.cs" .
```

Map these to `BarcodeEncoding` equivalents:

| Aspose `EncodeTypes` | IronBarcode `BarcodeEncoding` |
|---|---|
| `EncodeTypes.Code128` | `BarcodeEncoding.Code128` |
| `EncodeTypes.Code39` | `BarcodeEncoding.Code39` |
| `EncodeTypes.QR` | `BarcodeEncoding.QRCode` |
| `EncodeTypes.DataMatrix` | `BarcodeEncoding.DataMatrix` |
| `EncodeTypes.PDF417` | `BarcodeEncoding.PDF417` |
| `EncodeTypes.EAN13` | `BarcodeEncoding.EAN13` |
| `EncodeTypes.UPCA` | `BarcodeEncoding.UPCA` |
| `EncodeTypes.Aztec` | `BarcodeEncoding.Aztec` |

Note that `EncodeTypes.QR` maps to `BarcodeEncoding.QRCode` — the naming difference is where teams most often get the first compile error.

### Remove BarCodeImageFormat References

```bash
grep -r "BarCodeImageFormat\." --include="*.cs" .
```

Replace with the format-specific save method:

| Aspose | IronBarcode |
|---|---|
| `.Save("file.png", BarCodeImageFormat.Png)` | `.SaveAsPng("file.png")` |
| `.Save("file.jpg", BarCodeImageFormat.Jpeg)` | `.SaveAsJpeg("file.jpg")` |
| `.GenerateBarCodeImage()` | `.ToBitmap()` or `.ToPngBinaryData()` |

### Remove License File Handling

```bash
grep -r "SetLicense\|Aspose.BarCode.License\|Aspose.BarCode.Metered" --include="*.cs" .
grep -r "SetLicense\|\.lic" --include="*.cs" .
```

Remove all blocks that load a `.lic` file or configure metered keys. Replace with:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

### Docker: License File to Environment Variable

**Before — Aspose.BarCode Docker setup:**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY Aspose.BarCode.lic /app/license/
ENV ASPOSE_LICENSE_PATH=/app/license/Aspose.BarCode.lic
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

```csharp
// In startup
var license = new Aspose.BarCode.License();
license.SetLicense(Environment.GetEnvironmentVariable("ASPOSE_LICENSE_PATH"));
```

**After — IronBarcode Docker setup:**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV IRONBARCODE_LICENSE=YOUR-KEY-HERE
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

```csharp
// In startup
IronBarCode.License.LicenseKey =
    Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

Remove the `COPY Aspose.BarCode.lic` line and the corresponding `ENV ASPOSE_LICENSE_PATH`. In CI/CD, replace the file-based secret with a string secret containing the license key.

### Remove Quality Settings Code

Aspose.BarCode provides an extensive `QualitySettings` API for tuning recognition on damaged or low-quality images:

```bash
grep -r "QualitySettings\|AllowMedianSmoothing\|MedianSmoothingWindowSize\|AllowSaltAndPaper" --include="*.cs" .
```

Remove all of this. IronBarcode handles image quality internally without manual parameter tuning. Use `ReadingSpeed.Detailed` if you encounter recognition issues on difficult images — that is the only adjustment needed.

## Migration Checklist

Run these grep patterns first to get a complete inventory before making any changes:

```bash
# Find all Aspose.BarCode usages
grep -r "BarCodeReader" --include="*.cs" .
grep -r "BarCodeGenerator" --include="*.cs" .
grep -r "DecodeType\." --include="*.cs" .
grep -r "EncodeTypes\." --include="*.cs" .
grep -r "BarCodeImageFormat" --include="*.cs" .
grep -r "\.CodeText" --include="*.cs" .
grep -r "\.CodeTypeName" --include="*.cs" .
grep -r "SetLicense.*Aspose" --include="*.cs" .
grep -r "Aspose.BarCode.Metered" --include="*.cs" .
grep -r "QualitySettings" --include="*.cs" .
grep -r "using Aspose.BarCode" --include="*.cs" .
```

### Pre-Migration

- [ ] Run all grep patterns above and build a list of every affected file
- [ ] Note all `DecodeType` values in use — these all become auto-detected (no changes needed in logic, just remove the specification)
- [ ] Identify whether Aspose.PDF is used exclusively for barcode extraction — if so, it can be removed entirely
- [ ] Verify IronBarcode supports all symbologies your application uses (50+ formats; most standard symbologies are covered)
- [ ] Obtain your IronBarcode license key and add it to your secrets manager / environment

### Package and Namespace Changes

- [ ] Remove `Aspose.BarCode` from all `.csproj` files
- [ ] Remove `Aspose.PDF` from `.csproj` files if used only for barcode PDF extraction
- [ ] Add `IronBarcode` to all `.csproj` files that need barcode functionality
- [ ] Replace `using Aspose.BarCode.Generation;` with `using IronBarCode;`
- [ ] Replace `using Aspose.BarCode.BarCodeRecognition;` with `using IronBarCode;`
- [ ] Remove any remaining `using Aspose.*` statements not needed for other purposes
- [ ] Add `IronBarCode.License.LicenseKey = ...` to application startup

### Generation Code Changes

- [ ] Replace `new BarcodeGenerator(EncodeTypes.X, "data")` with `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.X)`
- [ ] Replace `.Save("file.png", BarCodeImageFormat.Png)` with `.SaveAsPng("file.png")`
- [ ] Replace `.GenerateBarCodeImage()` with `.ToPngBinaryData()` or `.ToBitmap()`
- [ ] Replace `EncodeTypes.QR` with `BarcodeEncoding.QRCode` (naming difference — easy to miss)
- [ ] Replace remaining `EncodeTypes.*` with corresponding `BarcodeEncoding.*` values
- [ ] Remove `Parameters.Barcode.*` configuration chains (use `ResizeTo()` if dimensions matter)
- [ ] For QR codes with logos: replace manual GDI+ overlay code with `.AddBrandLogo("logo.png")`
- [ ] For colored QR codes: replace `generator.Parameters.Barcode.BarColor = ...` with `.ChangeBarCodeColor(Color.X)`

### Reading Code Changes

- [ ] Replace `new BarCodeReader(path, DecodeType.X)` with `BarcodeReader.Read(path)`
- [ ] Remove `reader.SetBarCodeReadType(DecodeType.AllSupportedTypes)` — auto-detection is always on
- [ ] Remove all `DecodeType.*` specifications — no longer needed
- [ ] Remove `reader.ReadBarCodes()` calls — results come directly from `BarcodeReader.Read()`
- [ ] Remove `reader.FoundBarCodes` references — use the return value of `BarcodeReader.Read()` directly
- [ ] Replace `result.CodeText` with `result.Value`
- [ ] Replace `result.CodeTypeName` with `result.Format.ToString()`
- [ ] Remove all `using var reader = new BarCodeReader(...)` disposable patterns
- [ ] For PDF reading: remove Aspose.PDF rendering pipeline and replace with `BarcodeReader.Read("file.pdf")`
- [ ] Remove all `QualitySettings.*` configuration blocks

### Infrastructure Changes

- [ ] Remove Aspose `.lic` file from repository and build artifacts
- [ ] Remove `COPY Aspose.BarCode.lic` from Dockerfiles
- [ ] Remove `ASPOSE_LICENSE_PATH` environment variable references
- [ ] Add `IRONBARCODE_LICENSE` environment variable to Docker, Kubernetes, and CI/CD configs
- [ ] Update CI/CD pipeline secrets — remove Aspose license file secret, add `IRONBARCODE_LICENSE` string secret
- [ ] Remove Aspose license from Kubernetes `ConfigMap` or `Secret` if mounted as a file

### Post-Migration Verification

- [ ] Build the solution — fix any remaining compile errors from renamed types and properties
- [ ] Run unit tests for all barcode generation code paths
- [ ] Run unit tests for all barcode reading code paths
- [ ] If the application reads PDFs, test with multi-page PDF documents and verify `result.PageNumber` is populated
- [ ] Test batch processing — verify parallel execution works correctly (no instance management needed)
- [ ] Deploy to a staging environment and confirm the license key resolves from the environment variable
- [ ] Compare barcode read accuracy on your actual production document samples against Aspose results

## Pricing Reference

| IronBarcode Tier | Price | Coverage |
|---|---|---|
| Lite | $749 perpetual | 1 developer, 1 project |
| Plus | $1,499 perpetual | 3 developers |
| Professional | $2,999 perpetual | 10 developers |
| Unlimited | $5,999 perpetual | Unlimited developers |

All tiers include Windows x64/x86, Linux x64, macOS x64/ARM, Docker, Azure App Service, AWS Lambda, .NET Framework 4.6.2+, .NET Core 3.1+, .NET 5/6/7/8/9. No annual renewal required. PDF reading is included in the base package — no additional purchase needed.

The `DecodeType` removal is the change that saves the most future maintenance. Every time a new barcode format appears in your document sources, an Aspose.BarCode codebase requires a code change to add the new `DecodeType` to every reader. IronBarcode handles new formats without any code change. That is the reading pattern change the migration brief describes — and it compounds in value as your application's input sources grow.
