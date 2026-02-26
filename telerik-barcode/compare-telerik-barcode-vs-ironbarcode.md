Telerik's RadBarcode can generate a QR code. RadBarcodeReader cannot read a QR code. They are in the same product family and one cannot decode what the other produces.

This is not a configuration issue or an obscure edge case. Open the `DecodeType` enum in the Telerik WPF or WinForms barcode assembly and look for `DecodeType.QR`. It does not exist. There is no `DecodeType.DataMatrix`, no `DecodeType.PDF417`, no `DecodeType.Aztec`. The reading component was built for 1D linear formats only, while the generation component supports 2D formats. Progress Telerik ships both under the same product umbrella and neither documentation page goes out of its way to surface the mismatch.

Understanding that mismatch — and its second layer, which is that `RadBarcodeReader` only runs on WPF and WinForms while `RadBarcode` generation runs everywhere — is the core of this comparison.

## What Telerik RadBarcode Is

Telerik RadBarcode is part of the Progress Telerik UI suite, one of the most established commercial control libraries in .NET. It is not a barcode library. It is a barcode component inside a UI suite that includes roughly 150 WinForms controls, 150 WPF controls, 100 Blazor components, and more. Barcode generation lives in all of those platform packages. Barcode reading lives in exactly two of them.

The component surfaces differently depending on the platform. In WPF and WinForms, you get `RadBarcode` for generation and `RadBarcodeReader` (or `BarCodeReader` in WinForms) for reading. In Blazor, ASP.NET Core, and ASP.NET AJAX, you get `TelerikBarcode` or equivalent tag helpers for generation and nothing for reading. That reading gap is not a future roadmap item — it has existed across multiple release generations.

### Generation Setup

For WPF, `RadBarcode` is a XAML control:

```xml
<!-- WPF — works for generation -->
<Window xmlns:telerik="http://schemas.telerik.com/2008/xaml/presentation">
    <telerik:RadBarcode Value="12345678" Symbology="Code128" />
</Window>
```

For Blazor, `TelerikBarcode` is a Razor component:

```razor
@* Blazor — generation only *@
<TelerikBarcode Value="12345678" Type="@BarcodeType.Code128" />
```

Both work. Both look good. Neither platform provides a matching reading component. Blazor never will — there is no Blazor barcode reader in the Telerik suite at any tier.

For WPF reading, the NuGet package is `Telerik.UI.for.Wpf.60.Xaml`. For WinForms reading, it is `Telerik.UI.for.WinForms.Common` and `Telerik.UI.for.WinForms.Barcode`. For Blazor generation, it is `Telerik.UI.for.Blazor`. Each requires a separate license, each at a separate annual cost.

## The 1D-Only Reading Boundary

Even on WPF and WinForms — the two platforms where reading is available — the formats `RadBarcodeReader` accepts are exclusively 1D linear barcodes.

```csharp
// Telerik RadBarcodeReader (WPF) — this is all you can specify
using Telerik.Windows.Controls.Barcode;
using System.Windows.Media.Imaging;

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
    // DecodeType.QR        — does not exist
    // DecodeType.DataMatrix — does not exist
    // DecodeType.PDF417     — does not exist
    // DecodeType.Aztec      — does not exist
};

var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
var result = reader.Decode(bitmap);
return result?.Text; // null for any QR code, always
```

You can look through every value in `DecodeType`. The 2D formats are absent. This is not an omission from the code sample above — the enum genuinely does not contain them. If you pass an image containing a QR code to `RadBarcodeReader`, `result` will be null and the call will succeed without any error.

For [generating QR codes and other 2D formats](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/) that can then be round-tripped back through a reader, Telerik offers no solution within its own product family.

IronBarcode takes the opposite approach. There is no `DecodeTypes` specification at all — format detection is automatic across all 50+ supported types:

```csharp
// IronBarcode — identical for QR, DataMatrix, PDF417, Code128, and everything else
using IronBarCode;

var results = BarcodeReader.Read(imagePath);
foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.BarcodeType}: {barcode.Value}");
}
```

The same call that reads an EAN-13 barcode reads a QR code. If the image you pass contains a DataMatrix, `BarcodeReader.Read()` finds it. If it contains a QR code that a `RadBarcode` control generated, IronBarcode reads it. Telerik cannot.

For a detailed breakdown of [reading barcodes from images across all formats](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/), the IronBarcode documentation covers the full options including multi-barcode detection and speed tuning.

## The Platform Gap

The `DecodeType` limitation applies on the two platforms where reading is even possible. But the more fundamental gap is that those two platforms are the only ones where any reading exists at all.

| Platform | Telerik Generation | Telerik Reading | IronBarcode Generation | IronBarcode Reading |
|---|---|---|---|---|
| WPF | Yes | Yes (1D only) | Yes | Yes (all formats) |
| WinForms | Yes | Yes (1D only) | Yes | Yes (all formats) |
| Blazor | Yes | Not available | Yes | Yes (server-side) |
| ASP.NET Core | Yes | Not available | Yes | Yes |
| ASP.NET AJAX | Yes | Not available | Yes | Yes |
| Console / Worker Service | N/A | Not available | Yes | Yes |
| Docker / Linux | Partial | Not available | Yes | Yes |
| Azure Functions | Partial | Not available | Yes | Yes |

A team running a Blazor application that generates QR codes for customers to scan cannot use Telerik to validate or process scanned QR codes returned to the server. There is no Telerik API for that. They either accept that limitation, add a second barcode library, or replace the generation tooling with something that also reads.

IronBarcode runs on every .NET platform without UI framework dependencies. The [full platform compatibility coverage](https://ironsoftware.com/csharp/barcode/features/compatibility/) includes Docker, Azure, AWS Lambda, macOS, and Linux alongside the Windows targets. One NuGet package, one namespace, one consistent API.

## Side-by-Side: Generating and Reading QR

This is the scenario that most directly exposes the internal contradiction.

**Telerik — generation works, reading fails:**

```csharp
// STEP 1: Generate a QR code — this works fine with RadBarcode
// (XAML approach in WPF)
// <telerik:RadBarcode Value="https://example.com/order/4821" Symbology="QRCode" />
// — or — set programmatically
var qrBarcode = new RadBarcode();
qrBarcode.Value = "https://example.com/order/4821";
qrBarcode.Symbology = new QRCode { ErrorCorrectionLevel = ErrorCorrectionLevel.H };
// Save to file...

// STEP 2: Read that same QR code back — this fails
var reader = new RadBarcodeReader();
// There is no DecodeType for QR. Setting only 1D types:
reader.DecodeTypes = new[] { DecodeType.Code128, DecodeType.EAN13 };
var bitmap = new BitmapImage(new Uri("qr-output.png", UriKind.Absolute));
var result = reader.Decode(bitmap);
// result == null — always. The QR code is unreadable by RadBarcodeReader.
```

**IronBarcode — generation and reading work with the same library:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// STEP 1: Generate a QR code
BarcodeWriter.CreateBarcode("https://example.com/order/4821", BarcodeEncoding.QRCode)
             .SaveAsPng("qr-output.png");

// STEP 2: Read that QR code back
var results = BarcodeReader.Read("qr-output.png");
Console.WriteLine(results.First().Value);
// Output: https://example.com/order/4821
```

No platform constraint. No format restriction. The code that generates and the code that reads use the same package, the same namespace, and the same API style.

## Side-by-Side: 1D Barcode Reading on Multiple Platforms

**Telerik — requires platform-specific code, two different assemblies:**

```csharp
// WPF — reading works (1D only), different assembly
// using Telerik.Windows.Controls.Barcode;
var wpfReader = new RadBarcodeReader();
wpfReader.DecodeTypes = new[] { DecodeType.Code128, DecodeType.EAN13 };
var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
var wpfResult = wpfReader.Decode(bitmap);
var value = wpfResult?.Text;

// ---

// WinForms — different class name, different assembly
// using Telerik.WinControls.UI.Barcode;
var formsReader = new BarCodeReader();
formsReader.DecodeType = new[] { DecodeType.Code128, DecodeType.EAN13 };
var image = System.Drawing.Image.FromFile(imagePath);
var formsResult = formsReader.Read(image);
var formsValue = formsResult?.Text;

// ---

// Blazor — NO reading component exists
// ASP.NET Core — NO reading component exists
```

**IronBarcode — identical code on every platform:**

```csharp
// NuGet: dotnet add package IronBarcode
// Works in WPF, WinForms, ASP.NET Core, Blazor, console, Docker, Lambda
using IronBarCode;

var results = BarcodeReader.Read(imagePath);
var value = results.FirstOrDefault()?.Value ?? "No barcode found";
```

The Telerik approach requires knowing which platform you are on, using the right class for that platform, and accepting that the code you write in WPF cannot be extracted into a shared service library that also compiles for ASP.NET Core or Blazor.

## Pricing and TCO

RadBarcode is not sold separately. Every platform requires purchasing that platform's UI suite or the combined DevCraft bundle:

| Product | Annual Price (2026) |
|---|---|
| UI for WinForms | ~$1,149/year |
| UI for WPF | ~$1,149/year |
| UI for Blazor | ~$1,099/year |
| DevCraft UI (all platforms combined) | ~$1,469/year |

*Pricing as of January 2026. Visit the Telerik pricing page for current rates.*

A team of 10 developers who purchase DevCraft UI to cover all platforms pays approximately $14,690 per year. Over five years, that is $73,450 — for a feature set where reading cannot reach QR codes, cannot run in web or server contexts, and the barcode component represents roughly 2% of what they are actually purchasing.

IronBarcode's [licensing structure](https://ironsoftware.com/csharp/barcode/licensing/) offers perpetual options starting from $749 for a single developer. For 10 developers, the Professional tier is $2,999 — a one-time purchase covering full read/write across all platforms and all 50+ formats.

| Scenario | Telerik DevCraft UI | IronBarcode |
|---|---|---|
| Single developer, all platforms | ~$1,469/year | ~$749 perpetual |
| 10 developers, all platforms | ~$14,690/year | ~$2,999 perpetual |
| 5-year TCO (10 developers) | ~$73,450 | ~$2,999 |

The TCO calculation assumes the Telerik subscription runs for five years, and that reading needs remain unsatisfied for 2D formats and non-desktop platforms throughout that period.

## PDF Barcode Processing

One capability difference that does not appear in the format discussion is PDF handling. Many production barcode workflows involve documents — invoices, shipping manifests, identity documents — that contain barcodes embedded in PDF pages.

IronBarcode reads barcodes from PDF files natively:

```csharp
using IronBarCode;

// Read all barcodes from every page of a PDF
var results = BarcodeReader.Read("shipping-manifest.pdf");
foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.BarcodeType} — {barcode.Value}");
}
```

Telerik has no equivalent. `RadBarcodeReader` accepts only bitmap images. Reading barcodes from a PDF with Telerik requires converting each PDF page to an image using a separate library, then passing those images to `RadBarcodeReader` one at a time — and the results will still be 1D only. For the complete set of [barcode formats IronBarcode supports](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) across both reading and generation, the reference page covers all 50+ entries with notes on which formats support specific features.

## Feature Comparison

| Feature | Telerik RadBarcode | IronBarcode |
|---|---|---|
| 1D barcode generation | Yes (all platforms) | Yes |
| 2D barcode generation | Yes (all platforms) | Yes |
| 1D barcode reading | WPF and WinForms only | Yes (all platforms) |
| QR code reading | Not available | Yes |
| DataMatrix reading | Not available | Yes |
| PDF417 reading | Not available | Yes |
| Aztec reading | Not available | Yes |
| PDF barcode reading | Not available | Yes (native) |
| Auto format detection | No — must specify DecodeType | Yes |
| Server-side reading | Not available | Yes |
| ASP.NET Core reading | Not available | Yes |
| Blazor reading | Not available | Yes |
| Docker / Linux reading | Not available | Yes |
| Azure Functions support | Partial | Yes |
| Platform-specific code required | Yes (reading) | No |
| Standalone NuGet package | No — requires UI suite purchase | Yes |
| Perpetual license option | No — subscription | Yes |
| 5-year TCO (10 devs) | ~$73,450 | ~$2,999 |

## When to Consider Each

**Telerik RadBarcode makes sense when** your application is already deeply invested in the Telerik UI suite for non-barcode reasons — using its grids, charts, schedulers, or other controls extensively. In that case, adding the barcode generation control costs nothing marginal. If reading requirements are limited to 1D formats on WPF or WinForms, the RadBarcodeReader covers that scenario. If you only need to print or display barcodes and never need to scan them, the generation component is adequate on any Telerik-supported platform.

**IronBarcode makes sense when** your reading requirements extend to 2D formats, when reading needs to happen in a web application or server process, when consistency across platforms matters, or when you are evaluating barcode functionality without an existing Telerik investment. Teams choosing IronBarcode are typically not choosing between IronBarcode and RadBarcode alone — they are choosing between a UI suite subscription that includes a constrained barcode component and a dedicated barcode library that handles the full requirement.

Teams that purchased Telerik specifically for barcode functionality, discovered the QR reading gap or the platform restriction, and are looking for alternatives are the clearest migration candidates.

The internal product contradiction at the start of this comparison — `RadBarcode` generating QR codes that `RadBarcodeReader` cannot read — is not a version mismatch or a configuration gap. It is a structural boundary between what the generation component supports and what the reading component was built to handle. Any workflow that involves generating 2D barcodes and later reading them hits that boundary immediately.
