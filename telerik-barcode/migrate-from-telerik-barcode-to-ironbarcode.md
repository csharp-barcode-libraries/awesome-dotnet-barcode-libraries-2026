# Migrating from Telerik RadBarcode to IronBarcode

Most Telerik barcode migrations are triggered by one of two discoveries. The first: a requirement arrives to read QR codes, DataMatrix barcodes, or PDF417 from scanned images, and the developer finds that `RadBarcodeReader`'s `DecodeType` enum contains no QR entry, no DataMatrix entry, and no PDF417 entry. The second: the application needs barcode reading in an ASP.NET Core endpoint, a Blazor service, or a background worker, and the developer finds that `RadBarcodeReader` does not exist outside WPF and WinForms.

Both discoveries point to the same constraint: Telerik's barcode reading is 1D-only and desktop-only. This guide walks through replacing it with IronBarcode, which reads all 50+ formats on every .NET platform.

## Why Migrate

The case for migrating is most concrete when listed against specific requirements:

**You need to read QR codes.** `RadBarcodeReader.DecodeTypes` cannot include QR Code. Telerik can generate QR codes via `RadBarcode`, but no component in the Telerik suite can read one. If a downstream workflow requires reading QR codes — validating printed labels, decoding inbound shipment codes, scanning customer-presented QR codes — Telerik cannot satisfy it.

**You need reading in a web or server context.** `RadBarcodeReader` is available in the `Telerik.UI.for.Wpf` and `Telerik.UI.for.WinForms` packages only. Blazor, ASP.NET Core, console, and Azure Function projects have no Telerik barcode reading component to reference.

**You want consistent code across project types.** A shared service that reads barcodes cannot use Telerik types because those types only compile in platform-specific projects. IronBarcode's static `BarcodeReader.Read()` compiles identically in any .NET project.

**You want to stop paying for a UI suite to access a barcode component.** Telerik barcode functionality is not available as a standalone package. It requires purchasing WinForms (~$1,149/yr), WPF (~$1,149/yr), Blazor (~$1,099/yr), or DevCraft UI (~$1,469/yr) depending on which platform you need. IronBarcode's [perpetual licensing options](https://ironsoftware.com/csharp/barcode/licensing/) start at $749 for a single developer — a one-time purchase without an annual renewal requirement.

## Quick Start

### Step 1: Remove Telerik Barcode Packages

Remove the Telerik UI package for whichever platform you are on. You may have one or several:

```bash
# WPF
dotnet remove package Telerik.UI.for.Wpf.60.Xaml

# WinForms
dotnet remove package Telerik.UI.for.WinForms.Common
dotnet remove package Telerik.UI.for.WinForms.Barcode

# Blazor
dotnet remove package Telerik.UI.for.Blazor
```

If your project also uses Telerik components other than barcode, only remove the packages that are exclusively for barcode functionality. If the same package is shared with other Telerik controls, leave it in place and proceed to step 2 — you will simply stop calling the barcode-specific APIs.

### Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

One package. Works in WPF, WinForms, ASP.NET Core, Blazor Server, console applications, Docker containers, Azure Functions, and AWS Lambda without any platform-specific configuration. The [full platform compatibility details](https://ironsoftware.com/csharp/barcode/features/compatibility/) cover the exact .NET versions and OS targets that are supported.

### Step 3: Replace Namespace Imports and License

Remove Telerik barcode namespace imports and license initialization, then add the IronBarcode equivalents:

```csharp
// Remove these:
// using Telerik.Windows.Controls.Barcode;      // WPF reader/generator
// using Telerik.WinControls.UI;                // WinForms
// using Telerik.WinControls.UI.Barcode;        // WinForms barcode
// Telerik.WinControls.TelerikLicenseManager.InstallLicense("key");

// Add this:
using IronBarCode;
// At application startup (Program.cs, App.xaml.cs, or composition root):
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

The license key line goes once at application startup. There is no license file to deploy, no per-platform configuration, and no separate Blazor-vs-WPF initialization path.

## Code Migration Examples

### Example 1: WPF XAML Generation to Code-Based Generation

Telerik's generation in WPF uses XAML controls. IronBarcode generates barcodes in code and returns an image object you can save or display. The same API handles both 1D and 2D formats — see the [2D barcode generation guide](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/) for QR, DataMatrix, and PDF417 generation options.

**Before (Telerik WPF XAML):**

```xml
<telerik:RadBarcode Value="12345678" Symbology="Code128" />
```

**After (IronBarcode):**

```csharp
using IronBarCode;

// Generate and save to file
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
             .SaveAsPng("barcode.png");

// For display in WPF, get as BitmapSource
var barcodeImage = BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                                .ToBitmapSource();
barcodeImageControl.Source = barcodeImage;
```

For Blazor component replacement:

**Before (Telerik Blazor):**

```razor
<TelerikBarcode Value="12345678" Type="@BarcodeType.Code128" />
```

**After (IronBarcode — server-side generation, served as image):**

```csharp
// In a controller or Blazor page code-behind
using IronBarCode;

byte[] barcodeBytes = BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                                   .ResizeTo(400, 100)
                                   .ToPngBinaryData();
// Return as image response or embed as base64 src
string base64 = Convert.ToBase64String(barcodeBytes);
string imgSrc = $"data:image/png;base64,{base64}";
```

### Example 2: WPF Barcode Reading

This is the migration that reduces the most code. Telerik's WPF reading requires a reader instance, explicit `DecodeType` specification, a `BitmapImage` load, and direct bitmap passing to `Decode()`.

**Before (Telerik RadBarcodeReader — WPF only):**

```csharp
using Telerik.Windows.Controls.Barcode;
using System.Windows.Media.Imaging;

public string ReadBarcode(string imagePath)
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
        // No QR, DataMatrix, PDF417 available
    };

    var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
    var result = reader.Decode(bitmap);

    return result?.Text ?? "No barcode found";
}
```

**After (IronBarcode — any platform):**

```csharp
using IronBarCode;

public string ReadBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value ?? "No barcode found";
}
```

`BarcodeReader.Read()` is static — no instance to create. It accepts a file path string directly — no `BitmapImage` load required. Format detection is automatic across all 50+ types — no `DecodeType` list to maintain. The `result.Value` property replaces `result.Text`.

### Example 3: WinForms Barcode Reading

The WinForms reader class is named `BarCodeReader` (note the capital C) and takes a `System.Drawing.Image` rather than a WPF `BitmapImage`.

**Before (Telerik WinForms BarCodeReader):**

```csharp
using Telerik.WinControls.UI.Barcode;
using System.Drawing;

public string ReadBarcode(string imagePath)
{
    var reader = new BarCodeReader();
    reader.DecodeType = new DecodeType[]
    {
        DecodeType.Code128,
        DecodeType.Code39,
        DecodeType.EAN13,
        DecodeType.ITF
    };

    using var image = Image.FromFile(imagePath);
    var result = reader.Read(image);

    return result?.Text ?? "No barcode found";
}
```

**After (IronBarcode — same code as WPF, or any other platform):**

```csharp
using IronBarCode;

public string ReadBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value ?? "No barcode found";
}
```

The IronBarcode call is identical to the WPF example above. This is intentional — the same method handles the same operation regardless of which project type is calling it. There is no need for a `BitmapImage` variant for WPF and a `System.Drawing.Image` variant for WinForms.

### Example 4: Reading QR Codes (Previously Impossible with Telerik)

This scenario has no Telerik equivalent. If your codebase has a comment like `// QR reading not supported — use different library`, this is the replacement:

**Before (Telerik — not possible):**

```csharp
// RadBarcodeReader does not support QR codes.
// DecodeType.QR does not exist in the Telerik enum.
// This method throws to document the limitation.
public string ReadQrCode(string imagePath)
{
    throw new NotSupportedException(
        "Telerik RadBarcodeReader cannot read QR codes. " +
        "Use a different library for QR reading.");
}
```

**After (IronBarcode):**

```csharp
using IronBarCode;

public string ReadQrCode(string imagePath)
{
    // QR codes are detected automatically — no special configuration
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value ?? "No QR code found";
}
```

The same `BarcodeReader.Read()` call handles QR, DataMatrix, PDF417, Aztec, and every other 2D format automatically. For a complete list of [supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) across read and write operations, the IronBarcode documentation covers all 50+ entries.

### Example 5: Barcode Reading in ASP.NET Core (Previously Impossible with Telerik)

Telerik provides no barcode reading capability for ASP.NET Core or Blazor. If your application was routing uploaded images to a desktop service for reading because the web layer had no Telerik reader, that workaround is no longer needed.

```csharp
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
        // BarcodeReader.Read() accepts a Stream — no temp file needed
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

`BarcodeReader.Read()` accepts a `Stream` directly — the uploaded file stream can be passed without writing to disk. This works in an ASP.NET Core application deployed to Linux, Docker, or Azure App Service in exactly the same way as on Windows.

### Example 6: Reading Barcodes from PDFs

Telerik has no PDF barcode reading capability. IronBarcode reads barcodes from PDF files natively, with page number tracking:

```csharp
using IronBarCode;

// All barcodes from every page of a PDF document
var results = BarcodeReader.Read("invoice.pdf");
foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.BarcodeType} — {barcode.Value}");
}
```

No separate PDF conversion step, no page-by-page image extraction. Pass the PDF path and IronBarcode handles the rest.

## API Mapping

| Telerik | IronBarcode |
|---|---|
| `TelerikLicenseManager.InstallLicense("key")` | `IronBarCode.License.LicenseKey = "key"` |
| `<telerik:RadBarcode Value="..." Symbology="Code128" />` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)` |
| `<TelerikBarcode Type="@BarcodeType.Code128" />` | Server-side generation returning `ToPngBinaryData()` |
| `Symbology.Code128` | `BarcodeEncoding.Code128` |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` |
| `new RadBarcodeReader()` | Static class — no instance needed |
| `new BarCodeReader()` (WinForms) | Static class — no instance needed |
| `reader.DecodeTypes = new DecodeType[] { ... }` | Auto-detection — no specification needed |
| `reader.Decode(bitmapImage)` (WPF) | `BarcodeReader.Read(imagePath)` |
| `reader.Read(drawingImage)` (WinForms) | `BarcodeReader.Read(imagePath)` |
| `result.Text` | `result.Value` |
| 1D formats only (no QR, DataMatrix, PDF417) | 50+ formats including all 2D types |
| WPF and WinForms reading only | All .NET platforms |
| `BitmapImage` required as input (WPF) | File path, byte array, or `Stream` accepted |
| `System.Drawing.Image` required as input (WinForms) | File path, byte array, or `Stream` accepted |

### Property Name Changes

The most common substitution in reading code is `result.Text` → `result.Value`. In Telerik's WPF `RadBarcodeReaderResult`, the decoded string is exposed as `.Text`. In IronBarcode's `BarcodeResult`, it is `.Value`.

Similarly, the barcode type/format on the result object changes:

| Telerik | IronBarcode |
|---|---|
| `result.Text` | `result.Value` |
| `result.Symbology` (on result) | `result.BarcodeType` |

## Migration Checklist

Use these patterns to find all Telerik barcode usage in your codebase before migrating:

```bash
# License initialization
grep -r "TelerikLicenseManager.InstallLicense" --include="*.cs" .

# Generation controls (code-behind and XAML/Razor references)
grep -r "RadBarcode" --include="*.cs" .
grep -r "RadBarcode" --include="*.xaml" .
grep -r "TelerikBarcode" --include="*.razor" .

# Reading API
grep -r "RadBarcodeReader" --include="*.cs" .
grep -r "BarCodeReader" --include="*.cs" .
grep -r "DecodeType\." --include="*.cs" .
grep -r "reader\.Decode(" --include="*.cs" .

# Result property access
grep -r "result\.Text" --include="*.cs" .

# Symbology enum
grep -r "Symbology\.Code128" --include="*.cs" .
grep -r "Symbology\." --include="*.cs" .
```

Work through each match:

- `TelerikLicenseManager.InstallLicense(...)` — replace with `IronBarCode.License.LicenseKey = "key"`
- `RadBarcode` in XAML — replace with `BarcodeWriter.CreateBarcode()` in code-behind and an `Image` control displaying the result
- `TelerikBarcode` in Razor — replace with server-side generation returning base64 or binary data
- `new RadBarcodeReader()` or `new BarCodeReader()` — replace with `BarcodeReader.Read(path)`
- `reader.DecodeTypes = new DecodeType[] { ... }` blocks — delete entirely; IronBarcode auto-detects
- `reader.Decode(bitmap)` or `reader.Read(image)` calls — replace with `BarcodeReader.Read(imagePath)`
- `result.Text` — rename to `result.Value`
- `result.Symbology` — rename to `result.BarcodeType`
- `Symbology.Code128` in generation code — replace with `BarcodeEncoding.Code128`

## Feature Comparison After Migration

| Feature | Telerik RadBarcode | IronBarcode |
|---|---|---|
| 1D barcode generation | Yes | Yes |
| 2D barcode generation | Yes | Yes |
| 1D barcode reading | WPF + WinForms only | All platforms |
| QR code reading | Not available | Yes |
| DataMatrix reading | Not available | Yes |
| PDF417 reading | Not available | Yes |
| PDF barcode reading | Not available | Yes (native) |
| ASP.NET Core reading | Not available | Yes |
| Blazor reading | Not available | Yes |
| Docker / Linux reading | Not available | Yes |
| Auto format detection | No | Yes |
| Static read API | No (requires instance) | Yes |
| Perpetual license | No — subscription | Yes — from $749 |
| Standalone package | No — UI suite required | Yes |

## Common Migration Issues

### XAML Barcode Controls

`RadBarcode` in XAML is a visual control — it renders inline in the UI. IronBarcode generates barcode images in code. The migration pattern depends on how the barcode was being displayed:

- If the XAML barcode was bound to a data property and displayed in a form, replace it with an `Image` control bound to a `BitmapSource` generated by `BarcodeWriter.CreateBarcode(...).ToBitmapSource()`.
- If the barcode was being saved or exported rather than displayed, `BarcodeWriter.CreateBarcode(...).SaveAsPng(path)` is a direct replacement.

### DecodeType Removal

Deleting the `reader.DecodeTypes = new DecodeType[] { ... }` block is intentional. IronBarcode detects all supported formats on every read. If your production code had a subset of `DecodeType` values listed (perhaps because only certain formats were expected in your image source), IronBarcode may now return results for additional barcode types in the same images. This is correct behavior — those barcodes were always present; they were simply excluded by the `DecodeTypes` filter. If you want to constrain results for performance reasons, `BarcodeReaderOptions.ExpectedBarcodeTypes` provides optional filtering.

### Reading on Non-Desktop Platforms

If your application previously had no barcode reading in Blazor, ASP.NET Core, or server-side code because `RadBarcodeReader` was unavailable there, those code paths can now be implemented directly with [IronBarcode's cross-platform reading capability](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) — no architecture change required.

The absence of a `DecodeType.QR` in Telerik's reading API is not something a workaround can fix. The decoder was not built to handle 2D formats. After migrating, QR code images, DataMatrix codes on product labels, and PDF417 on scanned documents will all be read by the same `BarcodeReader.Read()` call that was already handling your 1D formats. There is no separate configuration path for 2D — it is included in the default detection.
