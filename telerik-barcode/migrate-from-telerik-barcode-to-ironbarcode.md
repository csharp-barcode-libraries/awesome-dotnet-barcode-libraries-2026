# Migrating from Telerik RadBarcode to IronBarcode

This guide provides a complete migration path from Telerik RadBarcode to IronBarcode for .NET developers who have reached the limits of what `RadBarcode` and `RadBarcodeReader` can do. It covers package replacement, namespace updates, license initialization, code-level migration examples, an API mapping reference, and a checklist for verifying the migration is complete. The examples focus on the scenarios most commonly encountered during Telerik barcode migrations: XAML control replacement, reading format expansion, server-side deployment, and PDF processing.

## Why Migrate from Telerik RadBarcode

Telerik RadBarcode migrations are triggered by concrete technical requirements that the existing components cannot satisfy. The reasons below represent the conditions most commonly reported by teams who have completed or are evaluating this transition.

**2D Reading Boundary:** The `RadBarcodeReader` component in WPF and the `BarCodeReader` component in WinForms share the same reading engine, which is bounded by the `DecodeType` enum. That enum contains entries for 1D linear formats — Code128, Code39, EAN13, EAN8, UPCA, UPCE, Codabar, ITF — and nothing else. No entry exists for QR Code, DataMatrix, PDF417, or Aztec. When a new requirement arrives to process inbound QR codes or read DataMatrix labels from scanned documents, the `DecodeType` constraint becomes a hard blocker with no workaround available within the Telerik product family.

**Platform Code Fragmentation:** Reading with Telerik requires UI framework dependencies: `RadBarcodeReader` uses WPF types (`BitmapImage`) and `BarCodeReader` uses WinForms types (`System.Drawing.Image`). These types cannot be referenced in a shared .NET library, a console application, an ASP.NET Core project, or a Blazor server. A team that wants to centralize barcode reading logic in a shared service layer cannot do so with Telerik types — the reader implementations are permanently tied to their respective UI frameworks.

**PDF Processing Gap:** Many barcode workflows operate on documents rather than image files. Telerik RadBarcodeReader accepts only bitmap images. Reading a barcode embedded in a PDF page requires converting the page to an image using a separate library, then feeding that image to the Telerik reader — adding a dependency and still receiving only 1D results. For teams whose documents are primarily PDFs, this workaround adds friction at every point in the workflow.

**Subscription Licensing:** Telerik barcode functionality is not sold as a standalone package. Access to `RadBarcode` and `RadBarcodeReader` is bundled with platform-specific UI suite subscriptions — approximately $1,149 per developer per year for WinForms or WPF, or $1,469 per developer per year for all platforms under DevCraft UI. Teams whose primary requirement is barcode functionality, and who do not use other Telerik UI controls, absorb the full suite cost for a component that represents a small fraction of what is purchased.

### The Fundamental Problem

The clearest expression of the Telerik reading limitation is that the component that generates a format cannot round-trip it through the reader:

```csharp
// Telerik — generates a QR code, then cannot read it back
var qrBarcode = new RadBarcode();
qrBarcode.Value = "https://example.com/order/4821";
qrBarcode.Symbology = new QRCode { ErrorCorrectionLevel = ErrorCorrectionLevel.H };
// Save to file...

// Attempt to read the same QR code:
var reader = new RadBarcodeReader();
// DecodeType.QR does not exist — QR cannot be added to this list
reader.DecodeTypes = new[] { DecodeType.Code128, DecodeType.EAN13 };
var bitmap = new BitmapImage(new Uri("qr-output.png", UriKind.Absolute));
var result = reader.Decode(bitmap);
// result == null — always, for any QR code image
```

IronBarcode closes this gap. Generation and reading use the same package, and the reading engine detects QR codes, DataMatrix, PDF417, and all other formats automatically:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// Generate a QR code
BarcodeWriter.CreateBarcode("https://example.com/order/4821", BarcodeEncoding.QRCode)
             .SaveAsPng("qr-output.png");

// Read that same QR code back — no format specification needed
var results = BarcodeReader.Read("qr-output.png");
Console.WriteLine(results.First().Value);
// Output: https://example.com/order/4821
```

## IronBarcode vs Telerik RadBarcode: Feature Comparison

| Feature | Telerik RadBarcode | IronBarcode |
|---|---|---|
| 1D barcode generation | Yes (all platforms) | Yes |
| 2D barcode generation | Yes (all platforms) | Yes |
| 1D barcode reading | WPF + WinForms only | Yes (all platforms) |
| QR code reading | Not available | Yes |
| DataMatrix reading | Not available | Yes |
| PDF417 reading | Not available | Yes |
| Aztec reading | Not available | Yes |
| PDF file reading | Not available | Yes (native) |
| Auto format detection | No — DecodeType required | Yes |
| ASP.NET Core reading | Not available | Yes |
| Blazor reading | Not available | Yes |
| Docker / Linux reading | Not available | Yes |
| Shared service library | Not possible (platform types) | Yes |
| Static read API | No (requires instance) | Yes |
| Standalone NuGet package | No — UI suite required | Yes |
| Perpetual license | No — subscription | Yes — from $749 |
| 5-year TCO (10 devs) | ~$73,450 | ~$2,999 |

## Quick Start

### Step 1: Replace NuGet Package

Remove the Telerik UI package for the platform or platforms in your project:

```bash
# WPF
dotnet remove package Telerik.UI.for.Wpf.60.Xaml

# WinForms
dotnet remove package Telerik.UI.for.WinForms.Common
dotnet remove package Telerik.UI.for.WinForms.Barcode

# Blazor
dotnet remove package Telerik.UI.for.Blazor
```

If your project also uses other Telerik controls from the same package, leave the package in place and proceed to step 2. You will remove the barcode-specific API calls without removing the shared package.

Install IronBarcode:

```bash
dotnet add package IronBarcode
```

One package covers WPF, WinForms, ASP.NET Core, Blazor Server, console applications, Docker, Azure Functions, and AWS Lambda. The [full platform compatibility details](https://ironsoftware.com/csharp/barcode/features/compatibility/) document the exact .NET versions and operating system targets supported.

### Step 2: Update Namespaces

Remove Telerik barcode namespace imports and replace them with the IronBarcode namespace:

```csharp
// Remove these namespace imports:
// using Telerik.Windows.Controls.Barcode;      // WPF reader and generator
// using Telerik.Windows.Controls;              // WPF general
// using Telerik.WinControls.UI;               // WinForms
// using Telerik.WinControls.UI.Barcode;        // WinForms barcode reader

// Add this:
using IronBarCode;
```

### Step 3: Initialize License

Replace the Telerik license initialization with the IronBarcode key assignment at application startup:

```csharp
// Remove:
// Telerik.WinControls.TelerikLicenseManager.InstallLicense("key");

// Add at application startup (Program.cs, App.xaml.cs, or composition root):
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

The key assignment goes once in the application entry point. There is no license file to deploy, no per-platform initialization path, and no separate configuration for WPF versus ASP.NET Core.

## Code Migration Examples

### Generating a Barcode: XAML Control to Code-Based API

Telerik's generation in WPF is declarative — the `RadBarcode` XAML element renders directly in the UI tree. IronBarcode generates barcodes in code and returns a `GeneratedBarcode` object that can be saved as a file or converted to a `BitmapSource` for WPF display.

**Telerik Approach:**

```xml
<!-- WPF XAML — RadBarcode renders inline in the UI -->
<Window xmlns:telerik="http://schemas.telerik.com/2008/xaml/presentation">
    <telerik:RadBarcode Value="PROD-2026-00184" Symbology="Code128" />
</Window>
```

```razor
@* Blazor — TelerikBarcode renders as a Razor component *@
<TelerikBarcode Value="PROD-2026-00184" Type="@BarcodeType.Code128" />
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

// Save to file — replaces XAML generation + manual export
BarcodeWriter.CreateBarcode("PROD-2026-00184", BarcodeEncoding.Code128)
             .ResizeTo(400, 100)
             .SaveAsPng("barcode.png");

// Display in WPF — assign to an Image control's Source property
var bitmapSource = BarcodeWriter.CreateBarcode("PROD-2026-00184", BarcodeEncoding.Code128)
                                .ToBitmapSource();
barcodeImageControl.Source = bitmapSource;

// Serve in Blazor or ASP.NET Core — return as base64 src attribute
byte[] bytes = BarcodeWriter.CreateBarcode("PROD-2026-00184", BarcodeEncoding.Code128)
                            .ResizeTo(400, 100)
                            .ToPngBinaryData();
string imgSrc = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
```

The `BarcodeEncoding` enum covers the same symbologies as Telerik's `Symbology` enum. Switching to QR code generation changes only the enum argument. For the full range of [2D barcode generation options](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/), including QR error correction levels and DataMatrix dimensions, the IronBarcode documentation provides complete examples.

### Reading 1D Barcodes: WPF Desktop

Telerik's WPF reading requires a `RadBarcodeReader` instance, explicit `DecodeType` configuration, a `BitmapImage` object loaded from a URI, and a call to `reader.Decode()`. The result property is `.Text`.

**Telerik Approach:**

```csharp
using Telerik.Windows.Controls.Barcode;
using System.Windows.Media.Imaging;

public string ReadProductBarcode(string imagePath)
{
    var reader = new RadBarcodeReader();
    reader.DecodeTypes = new DecodeType[]
    {
        DecodeType.Code128,
        DecodeType.Code39,
        DecodeType.EAN13,
        DecodeType.EAN8,
        DecodeType.UPCA,
        DecodeType.UPCE,
        DecodeType.Codabar,
        DecodeType.ITF
    };

    var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
    var result = reader.Decode(bitmap);

    return result?.Text ?? "No barcode found";
}
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public string ReadProductBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value ?? "No barcode found";
}
```

`BarcodeReader.Read()` is a static method — no reader instance is needed. It accepts a file path string directly without requiring a `BitmapImage` load. Format detection is automatic across all 50+ supported types. The result property is `.Value` rather than `.Text`. This same method call also reads QR codes, DataMatrix, and any other format present in the image.

### Reading 1D Barcodes: WinForms Desktop

The WinForms `BarCodeReader` class uses a different name, different property name (`DecodeType` singular rather than `DecodeTypes` plural), and takes a `System.Drawing.Image` rather than a `BitmapImage`.

**Telerik Approach:**

```csharp
using Telerik.WinControls.UI.Barcode;
using System.Drawing;

public string ReadShippingLabel(string imagePath)
{
    var reader = new BarCodeReader();
    reader.DecodeType = new DecodeType[]
    {
        DecodeType.Code128,
        DecodeType.ITF,
        DecodeType.EAN13
    };

    using var image = Image.FromFile(imagePath);
    var result = reader.Read(image);

    return result?.Text ?? "No barcode found";
}
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public string ReadShippingLabel(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value ?? "No barcode found";
}
```

The IronBarcode call is identical to the WPF example above. There is no WinForms-specific variant. The same method handles the same operation regardless of which project type is calling it, and the result works without a `System.Drawing.Image` dependency.

### Reading QR Codes: Previously Impossible with Telerik

If your codebase contains a comment or a `NotSupportedException` documenting that QR reading is unavailable, this is its replacement. No configuration path exists in Telerik that enables QR reading — this scenario has no Telerik equivalent.

**Telerik Approach:**

```csharp
// RadBarcodeReader does not support QR codes.
// DecodeType.QR does not exist in the Telerik enum.
public string ReadQrCode(string imagePath)
{
    throw new NotSupportedException(
        "Telerik RadBarcodeReader cannot read QR codes. " +
        "DecodeType.QR is not a valid enum value.");
}
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public string ReadQrCode(string imagePath)
{
    // QR codes, DataMatrix, PDF417, and Aztec are all detected automatically
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value ?? "No QR code found";
}
```

The same `BarcodeReader.Read()` call that reads EAN-13 barcodes reads QR codes. There is no separate configuration for 2D formats. For a complete list of [supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) across reading and generation, the IronBarcode documentation covers all 50+ entries with format-specific notes.

### Reading Barcodes in an ASP.NET Core Endpoint

Telerik provides no barcode reading component for ASP.NET Core or Blazor. If an existing application routes uploaded images to a desktop service for reading because the web layer had no Telerik reader, that workaround is replaced by direct reading in the web layer.

**Telerik Approach:**

```csharp
// No Telerik reading API available in ASP.NET Core.
// Workaround: forward uploaded file to a WPF or WinForms service
// that can reference Telerik.Windows.Controls.Barcode.
[HttpPost("read")]
public async Task<IActionResult> ReadBarcode(IFormFile imageFile)
{
    // Must proxy to desktop service — no Telerik reader available here
    var bytes = await ReadAllBytesAsync(imageFile);
    var result = await _desktopBridgeService.ReadAsync(bytes);
    return Ok(result);
}
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/barcode")]
public class BarcodeController : ControllerBase
{
    [HttpPost("read")]
    public IActionResult ReadBarcode(IFormFile imageFile)
    {
        using var stream = imageFile.OpenReadStream();
        var results = BarcodeReader.Read(stream);
        return Ok(results.Select(r => new
        {
            Format = r.BarcodeType.ToString(),
            Value = r.Value
        }));
    }

    [HttpGet("generate")]
    public IActionResult GenerateBarcode([FromQuery] string data)
    {
        var bytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
                                 .ResizeTo(400, 100)
                                 .ToPngBinaryData();
        return File(bytes, "image/png");
    }
}
```

`BarcodeReader.Read()` accepts a `Stream` directly, so the uploaded file stream is passed without writing to disk. This controller deploys to Linux, Docker, or Azure App Service without modification.

### Reading Barcodes from PDF Files

Telerik has no PDF barcode reading capability. Adding PDF reading to an existing Telerik-based codebase requires a PDF rendering library to convert pages to images before passing them to `RadBarcodeReader`. IronBarcode reads PDF files directly.

**Telerik Approach:**

```csharp
// No direct PDF reading support in RadBarcodeReader.
// Workaround: use a separate PDF library to extract page images
using SomePdfLibrary; // third-party dependency
using Telerik.Windows.Controls.Barcode;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    var results = new List<string>();
    var pages = PdfRenderer.ExtractPages(pdfPath); // third-party call

    foreach (var pageImage in pages)
    {
        var reader = new RadBarcodeReader();
        reader.DecodeTypes = new[] { DecodeType.Code128, DecodeType.EAN13 };
        var bitmap = ConvertToBitmapImage(pageImage); // conversion required
        var result = reader.Decode(bitmap);
        if (result != null)
            results.Add(result.Text); // 1D only — QR codes on PDF pages are missed
    }
    return results;
}
```

**IronBarcode Approach:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    var results = BarcodeReader.Read(pdfPath);
    return results.Select(r =>
        $"Page {r.PageNumber}: {r.BarcodeType} — {r.Value}").ToList();
}
```

No separate PDF library is needed. No page extraction step. No image conversion. Pass the PDF path and IronBarcode processes all pages, returning all barcodes with page number tracking. Both 1D and 2D barcodes on PDF pages are detected.

## Telerik RadBarcode API to IronBarcode Mapping Reference

| Telerik RadBarcode | IronBarcode |
|---|---|
| `TelerikLicenseManager.InstallLicense("key")` | `IronBarCode.License.LicenseKey = "key"` |
| `<telerik:RadBarcode Value="..." Symbology="Code128" />` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)` |
| `<TelerikBarcode Type="@BarcodeType.Code128" />` | Server-side generation returning `ToPngBinaryData()` |
| `Symbology.Code128` | `BarcodeEncoding.Code128` |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` |
| `new RadBarcodeReader()` (WPF) | Static class — no instance needed |
| `new BarCodeReader()` (WinForms) | Static class — no instance needed |
| `reader.DecodeTypes = new DecodeType[] { ... }` (WPF) | Auto-detection — no specification needed |
| `reader.DecodeType = new DecodeType[] { ... }` (WinForms) | Auto-detection — no specification needed |
| `reader.Decode(bitmapImage)` (WPF) | `BarcodeReader.Read(imagePath)` |
| `reader.Read(drawingImage)` (WinForms) | `BarcodeReader.Read(imagePath)` |
| `result.Text` | `result.Value` |
| `result.Symbology` | `result.BarcodeType` |
| `BitmapImage` required as input (WPF) | File path, `Stream`, or byte array accepted |
| `System.Drawing.Image` required as input (WinForms) | File path, `Stream`, or byte array accepted |
| 1D formats only in reading | 50+ formats including all 2D types |
| WPF and WinForms reading only | All .NET platforms |
| No PDF reading support | `BarcodeReader.Read("file.pdf")` — native |

## Common Migration Issues and Solutions

### Issue 1: XAML Barcode Control Replacement

**Telerik:** `<telerik:RadBarcode>` is a visual control that renders directly in the WPF UI element tree. It cannot be replaced with an `Image` element without code-behind changes.

**Solution:** Replace the `<telerik:RadBarcode>` element with an `<Image>` control and generate the barcode in code-behind. Bind the `Image.Source` to a `BitmapSource` generated by IronBarcode:

```csharp
// In code-behind or ViewModel
using IronBarCode;

var generated = BarcodeWriter.CreateBarcode(barcodeValue, BarcodeEncoding.Code128)
                             .ResizeTo(400, 100);
BarcodeImage.Source = generated.ToBitmapSource();
```

For Blazor, replace `<TelerikBarcode>` with an `<img>` element whose `src` attribute receives a base64-encoded PNG generated server-side.

### Issue 2: DecodeType Removal Side Effects

**Telerik:** The `DecodeTypes` (WPF) or `DecodeType` (WinForms) property filters which formats the reader attempts to detect. Removing this filter in an IronBarcode migration means IronBarcode will now return results for all formats present in the image, including formats that were previously excluded by the filter.

**Solution:** Remove the `DecodeType` assignment block entirely. If the additional detected formats cause unexpected results in downstream logic — for example, if code assumed only one barcode format would ever be returned — add optional filtering using `BarcodeReaderOptions`:

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    ExpectedBarcodeTypes = BarcodeEncoding.Code128 | BarcodeEncoding.EAN13
};
var results = BarcodeReader.Read(imagePath, options);
```

This is optional. The default behavior (detect all formats) is correct for most use cases and produces results that the previous Telerik implementation could not.

### Issue 3: TelerikLicenseManager Cleanup

**Telerik:** `Telerik.WinControls.TelerikLicenseManager.InstallLicense(key)` calls appear at application startup, typically in `Program.cs` (WinForms) or `App.xaml.cs` (WPF). The key is often stored in app settings, a configuration file, or hardcoded in the startup method.

**Solution:** Remove the `TelerikLicenseManager.InstallLicense()` call. Replace it with the IronBarcode key assignment at the same startup location:

```csharp
// Remove:
// Telerik.WinControls.TelerikLicenseManager.InstallLicense("TELERIK-KEY");

// Add:
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

If the Telerik package remains installed (because other non-barcode Telerik controls are still in use), the `TelerikLicenseManager` call for those controls should be preserved. Only remove the call if the Telerik packages themselves are being fully removed.

## Telerik RadBarcode Migration Checklist

### Pre-Migration Tasks

Audit the codebase for all Telerik barcode usage before making code changes:

```bash
# License initialization
grep -r "TelerikLicenseManager.InstallLicense" --include="*.cs" .

# XAML generation controls
grep -r "RadBarcode" --include="*.xaml" .
grep -r "TelerikBarcode" --include="*.razor" .

# Code-behind generation references
grep -r "RadBarcode" --include="*.cs" .
grep -r "Symbology\." --include="*.cs" .

# Reading API — WPF
grep -r "RadBarcodeReader" --include="*.cs" .
grep -r "DecodeTypes\." --include="*.cs" .
grep -r "reader\.Decode(" --include="*.cs" .

# Reading API — WinForms
grep -r "BarCodeReader" --include="*.cs" .
grep -r "DecodeType\." --include="*.cs" .
grep -r "reader\.Read(" --include="*.cs" .

# Result property access
grep -r "result\.Text" --include="*.cs" .
grep -r "result\.Symbology" --include="*.cs" .
```

Document each match. Note which files contain XAML controls, which contain reading code, and which platform each file belongs to.

### Code Update Tasks

1. Remove Telerik barcode NuGet packages (or retain if needed for other controls)
2. Install `IronBarcode` NuGet package in each project that requires barcode functionality
3. Add `IronBarCode.License.LicenseKey = "YOUR-KEY"` at each application startup entry point
4. Remove Telerik namespace imports and add `using IronBarCode`
5. Replace `<telerik:RadBarcode>` XAML elements with `<Image>` controls
6. Generate barcode images in code-behind using `BarcodeWriter.CreateBarcode(...).ToBitmapSource()`
7. Replace `<TelerikBarcode>` Razor components with base64 `<img>` sources generated server-side
8. Replace `new RadBarcodeReader()` instances with `BarcodeReader.Read(path)` static calls
9. Replace `new BarCodeReader()` instances with `BarcodeReader.Read(path)` static calls
10. Delete all `reader.DecodeTypes = new DecodeType[] { ... }` and `reader.DecodeType = new DecodeType[] { ... }` blocks
11. Replace `reader.Decode(bitmapImage)` calls with `BarcodeReader.Read(imagePath)`
12. Replace `reader.Read(drawingImage)` calls with `BarcodeReader.Read(imagePath)`
13. Rename `result.Text` to `result.Value` at all reading result access points
14. Rename `result.Symbology` to `result.BarcodeType` at all format check points
15. Replace `Symbology.Code128` and similar generation enum values with `BarcodeEncoding.Code128`
16. Remove `TelerikLicenseManager.InstallLicense()` calls where Telerik packages are fully removed

### Post-Migration Testing

- Verify that all previously-read 1D barcodes (Code128, EAN13, etc.) continue to return correct values
- Confirm that QR codes in test images are now read successfully where they previously returned null
- Test that generated barcodes (saved as PNG or served as bytes) render visibly and scan correctly
- Verify that ASP.NET Core or Blazor endpoints that previously could not perform reading now function correctly
- If PDF reading was added, confirm that barcodes on each PDF page are detected with correct page numbers
- Run the grep audit commands from the Pre-Migration section to confirm no remaining Telerik barcode API references
- Confirm that `IronBarCode.License.LicenseKey` is set before the first `BarcodeReader.Read()` or `BarcodeWriter.CreateBarcode()` call

## Key Benefits of Migrating to IronBarcode

**Complete 2D Format Support:** After migration, QR Code, DataMatrix, PDF417, and Aztec formats are readable by the same API call that was already reading Code128 and EAN13. No additional configuration is required and no separate library is needed for 2D formats. The reading engine handles format identification automatically.

**Unified Cross-Platform API:** The same `BarcodeReader.Read()` and `BarcodeWriter.CreateBarcode()` calls compile and run identically in WPF, WinForms, ASP.NET Core, Blazor, console applications, and Docker containers. Barcode logic written once in a shared service library works across every project type in the solution without modification.

**Native PDF File Processing:** IronBarcode reads barcodes from PDF files directly, processing all pages and returning results with page number metadata. Workflows that previously required a PDF rendering library plus a Telerik reading step are consolidated into a single method call, removing the third-party dependency and the intermediate image conversion step.

**Eliminated Subscription Dependency for Barcode Access:** After migration, barcode functionality is available through IronBarcode's perpetual licensing model. Continued access does not require annual renewal. Teams whose primary use of the Telerik suite was barcode-related reduce their recurring licensing overhead to a one-time cost.

**Simplified Codebase:** The `DecodeType` configuration block, the `BitmapImage` loading step, the platform-specific reader class selection, and the `result.Text` property chain are all replaced by a single static method call with an auto-detecting result collection. Code that previously required different implementations for WPF and WinForms reading collapses into a single shared implementation.
