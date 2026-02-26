# Migrating from Spire.Barcode to IronBarcode

Spire.Barcode's `BarcodeScanner.Scan()` requires a `BarCodeType` argument on every call. As a codebase grows to handle diverse document sources, new supplier labels, and new barcode formats, this design produces type-guessing loops that accumulate maintenance cost over time. This guide covers the complete path from Spire.Barcode to IronBarcode: the reasons teams make this transition, the mechanical steps to execute it, and the before-and-after code patterns for every common operation.

## Why Migrate from Spire.Barcode

**Format Specification Overhead:** Every call to `BarcodeScanner.Scan()` in Spire.Barcode requires a `BarCodeType` enum value that the calling code must supply. Applications that process barcodes from unknown or mixed-format sources must maintain a candidate list of possible types and iterate through it until a match is found. Each format added to the workflow requires updating the candidate array, re-testing detection logic, and confirming that iteration order does not cause one format to shadow another. This overhead grows in proportion to format diversity.

**Free Tier Evaluation Constraints:** FreeSpire.Barcode intentionally degrades reading performance and limits the available symbology set compared to the commercial product. Benchmarks and format coverage tests conducted during a FreeSpire.Barcode evaluation do not represent commercial Spire.Barcode behavior. Teams that commit to a purchase based on a free tier evaluation may discover post-purchase that the product behaves differently in production — particularly around throughput and supported symbologies.

**PDF Requiring Two Libraries:** Spire.Barcode cannot read barcodes from PDF files directly. PDF-based workflows require the separate `Spire.PDF` package with its own license, plus developer-written code to iterate pages, extract images from each page, and feed those images to the barcode scanner. A task that conceptually belongs to barcode reading is split across two products, two license agreements, and significant infrastructure code.

**Return Type Complexity:** `BarcodeScanner.Scan()` returns `string[]`, which contains only decoded values. The detected barcode format is not included in the return. Applications that need to route results by format — sending QR code values to one processor and Code128 values to another, for example — must maintain a separate type variable derived from whichever iteration step produced the result, creating additional state management beyond the scan itself.

### The Fundamental Problem

The type-guessing loop is the most visible symptom of the mandatory `BarCodeType` constraint. In a Spire.Barcode codebase processing documents from multiple sources, this pattern appears repeatedly:

```csharp
// Spire.Barcode: growing candidate loop with silent misses
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

IronBarcode removes the loop entirely. Format detection is built into the library:

```csharp
// IronBarcode: single call, all formats, type included in result
var results = BarcodeReader.Read("barcode.png");

foreach (var result in results)
{
    Console.WriteLine($"{result.BarcodeType}: {result.Value}");
}
```

## IronBarcode vs Spire.Barcode: Feature Comparison

| Feature | Spire.Barcode | IronBarcode |
|---|---|---|
| Automatic format detection | No | Yes |
| BarCodeType parameter required | Yes | No |
| Format metadata in result | No | Yes |
| Native PDF reading | No (requires Spire.PDF) | Yes |
| Additional library for PDF | Spire.PDF (separate license) | None |
| Free tier reading performance | Intentionally degraded | Full speed |
| Free tier watermarks | Large, covers barcode | Small, edge only |
| Free tier symbologies | ~20 types | 50+ types |
| Registration for free tier | Yes | No |
| Generation API model | Settings object + generator | Static factory with fluent chain |
| QR code with custom logo | Commercial tier only | All tiers |
| Return type from read | `string[]` | `BarcodeResults` (with type metadata) |
| Symbology count | 39+ (commercial) | 50+ |
| License model | Per-seat perpetual + subscription | Perpetual with optional renewal |

## Quick Start

### Step 1: Replace NuGet Package

Remove the Spire package or packages currently in use:

```bash
# Remove whichever variant is installed
dotnet remove package FreeSpire.Barcode
# or
dotnet remove package Spire.Barcode

# If Spire.PDF was installed only for barcode extraction, remove it as well
dotnet remove package Spire.PDF
```

If `Spire.PDF` is used for other document operations beyond barcode extraction, retain it and remove only the barcode-related code paths from it.

Install IronBarcode:

```bash
dotnet add package IronBarcode
```

### Step 2: Update Namespaces

Replace all Spire namespace imports with the IronBarCode namespace:

```csharp
// Remove:
using Spire.Barcode;
using Spire.Pdf;

// Add:
using IronBarCode;
```

### Step 3: Initialize License

Replace the Spire license initialization with a single IronBarcode property assignment at application startup. See the [IronBarcode licensing page](https://ironsoftware.com/csharp/barcode/licensing/) for key formats and deployment options:

```csharp
// Remove (FreeSpire registration):
Spire.Barcode.BarcodeSettings.ApplyKey("your-free-key");

// Remove (commercial Spire license):
Spire.License.LicenseProvider.SetLicenseKey("your-commercial-key");

// Add (IronBarcode):
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-LICENSE-KEY";
```

For Docker and CI environments, the license key can be supplied from an environment variable:

```csharp
IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

## Code Migration Examples

### Reading a Known-Format Barcode

A single-format read in Spire.Barcode passes the type explicitly and receives a `string[]`.

**Spire.Barcode Approach:**
```csharp
using Spire.Barcode;

BarcodeScanner scanner = new BarcodeScanner();
string[] results = scanner.Scan("shipping-label.png", BarCodeType.Code128);

if (results.Length > 0)
{
    ProcessShipment(results[0]);
}
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

var results = BarcodeReader.Read("shipping-label.png");

if (results.Any())
{
    ProcessShipment(results.First().Value);
}
```

The `BarCodeType` parameter is removed. If downstream logic needs to confirm the detected format, `results.First().BarcodeType` provides it without requiring a separate state variable.

### Collapsing a Mixed-Format Type Loop

When a Spire.Barcode codebase handles documents that may contain any of several formats, the type-guessing loop accumulates over time as new formats enter the workflow.

**Spire.Barcode Approach:**
```csharp
using Spire.Barcode;

BarcodeScanner scanner = new BarcodeScanner();
BarCodeType[] candidates = new[]
{
    BarCodeType.Code128,
    BarCodeType.QRCode,
    BarCodeType.DataMatrix,
    BarCodeType.EAN13,
    BarCodeType.PDF417
};

List<string> allValues = new List<string>();
foreach (BarCodeType type in candidates)
{
    string[] found = scanner.Scan("document-scan.png", type);
    allValues.AddRange(found);
}
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

// The entire loop is replaced by a single call
var results = BarcodeReader.Read("document-scan.png");
List<string> allValues = results.Select(r => r.Value).ToList();
```

[IronBarcode's image reading](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) handles all 50+ symbologies in a single pass. When reading speed needs to be tuned for high-volume batch processing, `BarcodeReaderOptions` provides that control without adding format-specific branches:

```csharp
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};
var results = BarcodeReader.Read("document-scan.png", options);
```

For guidance on selecting the appropriate `ReadingSpeed` value, see the [reading speed options documentation](https://ironsoftware.com/csharp/barcode/how-to/reading-speed-options/).

### Generating a Barcode

Spire.Barcode generation requires instantiating a `BarcodeSettings` object, assigning each property individually, passing the settings to a `BarCodeGenerator`, and then calling `GenerateImage()` before any file is written.

**Spire.Barcode Approach:**
```csharp
using Spire.Barcode;

BarcodeSettings settings = new BarcodeSettings();
settings.Type = BarCodeType.Code128;
settings.Data = "SHIP-9842-XZ";
settings.ShowText = true;
settings.TextMargin = 5;
settings.BarHeight = 60;
settings.Unit = GraphicsUnit.Pixel;

BarCodeGenerator generator = new BarCodeGenerator(settings);
Image barcodeImage = generator.GenerateImage();
barcodeImage.Save("shipping-label.png", ImageFormat.Png);
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("SHIP-9842-XZ", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng("shipping-label.png");
```

The mutable `BarcodeSettings` object and the separate `BarCodeGenerator` instance are replaced by a single static factory call with a fluent chain. There is no risk of settings being accidentally shared across concurrent calls because each `CreateBarcode()` invocation is independent.

### Reading Barcodes from a PDF Document

Spire.Barcode cannot read PDFs directly. The Spire-based approach requires the separate `Spire.PDF` package to open the file, iterate its pages, extract raster images from each page, and then pass those images one at a time to the barcode scanner.

**Spire.Barcode Approach:**
```csharp
using Spire.Pdf;
using Spire.Barcode;

var pdf = new PdfDocument();
pdf.LoadFromFile("invoices.pdf");

var scanner = new BarcodeScanner();
var extractedValues = new List<string>();

foreach (PdfPageBase page in pdf.Pages)
{
    Image[] images = page.ExtractImages();
    foreach (Image image in images)
    {
        string[] found = scanner.Scan(image, BarCodeType.QRCode);
        extractedValues.AddRange(found);
    }
}
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

var results = BarcodeReader.Read("invoices.pdf");

foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.BarcodeType} = {barcode.Value}");
}
```

[IronBarcode reads PDF documents directly](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) without requiring `Spire.PDF` or any manual page management. The page number from which each barcode was extracted is available in the result object. Once the PDF reading code is replaced, the `Spire.PDF` package reference and all `using Spire.Pdf;` imports can be removed from the project.

## Spire.Barcode API to IronBarcode Mapping Reference

| Spire.Barcode | IronBarcode | Notes |
|---|---|---|
| `new BarcodeScanner()` | `BarcodeReader.Read()` (static) | No scanner instance needed |
| `scanner.Scan(path, BarCodeType.X)` | `BarcodeReader.Read(path)` | Type parameter removed |
| `BarCodeType.Code128` (required) | Auto-detected | No equivalent required |
| `BarCodeType.QRCode` (required) | Auto-detected | No equivalent required |
| `string[] results` from `Scan()` | `BarcodeResults` collection | Access values via `.Value` |
| `results[0]` (string) | `results[0].Value` | |
| Format unknown on result | `result.BarcodeType` | Type included in result object |
| `new BarcodeSettings()` | Parameters on `CreateBarcode()` | No settings object needed |
| `settings.Type = BarCodeType.X` | `BarcodeEncoding.X` as second param | |
| `settings.Data = "value"` | First parameter of `CreateBarcode()` | |
| `new BarCodeGenerator(settings)` | Not needed | Static factory replaces it |
| `generator.GenerateImage()` | `.SaveAsPng()` / `.SaveAsJpeg()` | Format-specific output methods |
| `BarcodeSettings.ApplyKey("key")` | `IronBarCode.License.LicenseKey = "key"` | Set once at startup |
| `Spire.License.LicenseProvider.SetLicenseKey("key")` | `IronBarCode.License.LicenseKey = "key"` | Same single-line replacement |
| `Spire.PDF` (for PDF reading) | Not needed | Native PDF support built in |

## Common Migration Issues and Solutions

### Issue 1: BarCodeType Parameter Removal

**Spire.Barcode:** `scanner.Scan(path, BarCodeType.Code128)` — the type argument is required by the method signature. Removing it causes a compile error.

**Solution:** Replace the entire call with `BarcodeReader.Read(path)`. Search the codebase for `BarCodeType.` to locate every occurrence. Type-guessing loops (foreach blocks iterating over a `BarCodeType[]` candidate array) can be deleted in their entirety; a single `BarcodeReader.Read()` call replaces the loop.

### Issue 2: string[] Return Type

**Spire.Barcode:** `Scan()` returns `string[]`. Downstream code that assigns this to a `string[]` variable or passes it to a method expecting `string[]` will fail to compile after migration.

**Solution:** Update call sites to work with `BarcodeResults`. To obtain a plain string array, use `.Select(r => r.Value).ToArray()`. To access the first result, use `.First()?.Value`. If the detected format is needed downstream, it is available on the result object via `.BarcodeType` — no separate state variable is required.

```csharp
// If a plain string array is needed:
string[] texts = BarcodeReader.Read("barcode.png")
    .Select(r => r.Value)
    .ToArray();

// If only the first value is needed:
string first = BarcodeReader.Read("barcode.png").First()?.Value;
```

### Issue 3: Spire.PDF Package Cleanup

**Spire.Barcode:** PDF reading workflows depend on `Spire.PDF` for page access and image extraction. The project references the package and contains `using Spire.Pdf;` imports and `PdfDocument` / `PdfPageBase` usage spread across multiple files.

**Solution:** After replacing all PDF barcode extraction code with `BarcodeReader.Read("file.pdf")`, run a grep audit to confirm no remaining `Spire.Pdf` references exist before removing the package:

```bash
grep -r "using Spire.Pdf" --include="*.cs" .
grep -r "Spire\.Pdf\|PdfPageBase\|ExtractImages" --include="*.cs" .
```

Remove the package only when all references are cleared:

```bash
dotnet remove package Spire.PDF
```

### Issue 4: Namespace Changes

**Spire.Barcode:** The `Spire.Barcode` namespace contains `BarcodeScanner`, `BarcodeSettings`, `BarCodeGenerator`, and `BarCodeType`. The `IronBarCode` namespace contains `BarcodeReader`, `BarcodeWriter`, `BarcodeEncoding`, and `BarcodeReaderOptions`.

**Solution:** Replace `using Spire.Barcode;` with `using IronBarCode;` globally. The `BarCodeType` enum used in generation is replaced by `BarcodeEncoding`. The `BarCodeType` enum used in reading has no IronBarcode equivalent — it is simply removed, as format detection is automatic.

## Spire.Barcode Migration Checklist

### Pre-Migration Tasks

Audit the codebase to identify all Spire.Barcode usage before making changes:

```bash
grep -r "using Spire.Barcode" --include="*.cs" .
grep -r "using Spire.Pdf" --include="*.cs" .
grep -r "BarcodeScanner\|BarcodeSettings\|BarCodeGenerator\|BarCodeType\." --include="*.cs" .
grep -r "ApplyKey\|SetLicenseKey" --include="*.cs" .
grep -r "ExtractImages\|PdfPageBase" --include="*.cs" .
```

- Document all files containing `BarCodeType` candidate arrays (type-guessing loops to be removed)
- Identify any `string[]` variables that receive results from `Scan()` and will need type updates
- Confirm whether `Spire.PDF` is used for operations other than barcode extraction

### Code Update Tasks

1. Remove `FreeSpire.Barcode` or `Spire.Barcode` NuGet package
2. Remove `Spire.PDF` NuGet package if barcode extraction was its only use
3. Install `IronBarcode` NuGet package
4. Replace all `using Spire.Barcode;` with `using IronBarCode;`
5. Remove all `using Spire.Pdf;` imports where no longer needed
6. Replace license initialization: `ApplyKey()` or `SetLicenseKey()` with `IronBarCode.License.LicenseKey = "key"`
7. Remove all `new BarcodeScanner()` instantiations
8. Replace all `scanner.Scan(path, BarCodeType.X)` calls with `BarcodeReader.Read(path)`
9. Delete type-guessing foreach loops; replace each with a single `BarcodeReader.Read()` call
10. Remove PDF page iteration and image extraction blocks; replace with `BarcodeReader.Read("file.pdf")`
11. Update `string[]` result assignments to use `.Value` or `.Select(r => r.Value).ToArray()`
12. Replace `new BarcodeSettings()` blocks with `BarcodeWriter.CreateBarcode(data, encoding)` chains
13. Remove `new BarCodeGenerator(settings)` instantiations
14. Replace `.GenerateImage()` + `image.Save()` with `.SaveAsPng()` or `.SaveAsJpeg()`
15. Replace `BarCodeType.X` enum values in generation with `BarcodeEncoding.X`

### Post-Migration Testing

- Confirm all formats previously scanned via the candidate loop are detected correctly by `BarcodeReader.Read()`
- Measure batch processing throughput and confirm it meets production requirements
- Verify PDF barcode extraction produces correct values and page numbers
- Confirm generated barcode output quality and scanability with a physical or software scanner
- Check that the license key is correctly loaded in all deployment environments (local, staging, production, CI/CD)
- Run the grep audit commands from Pre-Migration Tasks to confirm no Spire references remain

## Key Benefits of Migrating to IronBarcode

**Elimination of Format Maintenance Overhead:** After migration, there is no candidate type list to maintain and no type-guessing loop to update when a new barcode format enters the workflow. `BarcodeReader.Read()` handles the full symbology set automatically, and the detected format is included in each result object for downstream routing.

**Accurate Evaluation Before Commitment:** The IronBarcode trial runs from the same binary as the licensed product at full reading speed with the complete symbology set. Performance benchmarks and format coverage tests conducted during trial evaluation transfer directly to production. Teams can make a purchase decision with confidence that the evaluated behavior is the deployed behavior.

**Single-Package PDF Support:** Barcode extraction from PDF files is handled by the same `BarcodeReader.Read()` call used for images. No secondary library, no additional license, and no manual page iteration code are required. Teams that previously maintained both `Spire.Barcode` and `Spire.PDF` reduce to a single dependency for the entire barcode workflow.

**Result Objects with Format Metadata:** Each entry in a `BarcodeResults` collection carries the decoded value, the detected symbology, the source page number for PDF inputs, and the bounding region within the source image. Applications that need to route, filter, or log by format have that information available on the result without additional state management.

**Simplified Generation API:** The static factory pattern in `BarcodeWriter.CreateBarcode()` eliminates the mutable `BarcodeSettings` object and the separate `BarCodeGenerator` instance. Each generation call is self-contained, reducing the risk of configuration bleed between concurrent operations.

**Forward Compatibility:** IronBarcode receives regular updates aligned with .NET runtime releases. As .NET 10 adoption increases through 2026, compatibility updates ensure that migrated codebases continue to build and run without requiring library changes on each new .NET version.
