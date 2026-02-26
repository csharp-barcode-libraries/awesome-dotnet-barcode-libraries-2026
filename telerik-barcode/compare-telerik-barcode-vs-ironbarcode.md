Telerik's RadBarcode can generate a QR code. RadBarcodeReader cannot read a QR code. They are in the same product family and one cannot decode what the other produces.

This is not a configuration issue or an obscure edge case. Open the `DecodeType` enum in the Telerik WPF or WinForms barcode assembly and look for `DecodeType.QR`. It does not exist. There is no `DecodeType.DataMatrix`, no `DecodeType.PDF417`, no `DecodeType.Aztec`. The reading component was built for 1D linear formats only, while the generation component supports 2D formats. Progress Telerik ships both under the same product umbrella and neither documentation page goes out of its way to surface the mismatch. Understanding that mismatch — and its second layer, which is that `RadBarcodeReader` only runs on WPF and WinForms while `RadBarcode` generation runs everywhere — is the core of this comparison.

## Understanding Telerik RadBarcode

Telerik RadBarcode is part of the Progress Telerik UI suite, one of the most established commercial control libraries in .NET. It is not a standalone barcode library. It is a barcode component embedded inside a UI suite that includes roughly 150 WinForms controls, 150 WPF controls, 100 Blazor components, and more. Barcode generation is available across all of those platform packages. Barcode reading is available in exactly two of them: WPF and WinForms.

The component surfaces differently depending on the platform. In WPF and WinForms, developers get `RadBarcode` for generation and `RadBarcodeReader` (or `BarCodeReader` in WinForms) for reading. In Blazor, ASP.NET Core, and ASP.NET AJAX, developers get `TelerikBarcode` or equivalent tag helpers for generation and no reader component at all. That reading gap has existed across multiple release generations and is not listed as a roadmap item.

Key architectural characteristics of Telerik RadBarcode:

- **UI Suite Component:** RadBarcode is not a standalone NuGet package. It requires purchasing the Telerik UI suite for the target platform — WinForms (~$1,149/yr), WPF (~$1,149/yr), Blazor (~$1,099/yr), or DevCraft UI (~$1,469/yr).
- **XAML and Razor Control Model:** Generation is handled declaratively through `<telerik:RadBarcode>` in XAML or `<TelerikBarcode>` in Razor components, not through a code-based API.
- **Platform-Specific Assemblies:** Reading is available in `Telerik.UI.for.Wpf.60.Xaml` and `Telerik.UI.for.WinForms.Barcode`. No equivalent reading assembly exists for Blazor, ASP.NET Core, or server targets.
- **1D-Only Reading Engine:** The `RadBarcodeReader` component accepts only formats present in the `DecodeType` enum. That enum does not include QR Code, DataMatrix, PDF417, or Aztec.
- **Subscription-Based Licensing:** Telerik barcode access is included in the UI suite subscription. There is no perpetual option and no standalone barcode license.
- **Reading Class Name Divergence:** WPF uses `RadBarcodeReader` while WinForms uses `BarCodeReader` — different class names, different input types (`BitmapImage` vs `System.Drawing.Image`), different assemblies.

### The RadBarcode and RadBarcodeReader Split

The `DecodeType` enum defines every format the WPF `RadBarcodeReader` can process. The 2D formats are structurally absent from it:

```csharp
// Telerik RadBarcodeReader (WPF) — complete list of available DecodeType values for reading
using Telerik.Windows.Controls.Barcode;

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
    // DecodeType.QR        — does not exist in the enum
    // DecodeType.DataMatrix — does not exist in the enum
    // DecodeType.PDF417     — does not exist in the enum
    // DecodeType.Aztec      — does not exist in the enum
};
```

This is not an omission from the code sample. The enum genuinely does not contain entries for 2D formats. If an image containing a QR code is passed to `RadBarcodeReader.Decode()`, the result will be null and no exception is raised. The same `RadBarcode` XAML control that generated the QR code cannot process it on the reading side.

## Understanding IronBarcode

IronBarcode is a dedicated .NET barcode library that handles both barcode generation and barcode reading through a unified static API. It is not part of a UI suite and does not carry UI framework dependencies. The library is available as a single NuGet package (`IronBarcode`) and installs identically in WPF, WinForms, ASP.NET Core, Blazor Server, console applications, Docker containers, Azure Functions, and AWS Lambda.

The library's reading engine performs automatic format detection across all supported formats. Developers do not specify which format to look for — the engine identifies the format from the image content. The same `BarcodeReader.Read()` call that detects an EAN-13 barcode in one image will detect a QR code, DataMatrix, or PDF417 in another without any configuration change.

Key characteristics of IronBarcode:

- **Unified Static API:** `BarcodeReader.Read()` and `BarcodeWriter.CreateBarcode()` are the primary entry points. No instance management required.
- **Automatic Format Detection:** Reads 50+ barcode formats — both 1D and 2D — without requiring format pre-specification.
- **Single Package, All Platforms:** One NuGet reference compiles in any .NET project type without platform-specific code paths.
- **Code-Based Generation:** `BarcodeWriter.CreateBarcode()` returns an image object that can be saved, served as bytes, or converted to a `BitmapSource` for WPF display.
- **Native PDF Support:** Reads barcodes from PDF files directly, including multi-page documents with page number tracking.
- **Perpetual Licensing Available:** License tiers include perpetual options starting at $749 for a single developer.

## Feature Comparison

The following table summarizes the top-level capability differences between Telerik RadBarcode and IronBarcode:

| Feature | Telerik RadBarcode | IronBarcode |
|---|---|---|
| 1D barcode generation | Yes (all platforms) | Yes |
| 2D barcode generation | Yes (all platforms) | Yes |
| 1D barcode reading | WPF and WinForms only | Yes (all platforms) |
| QR code reading | Not available | Yes |
| PDF barcode reading | Not available | Yes (native) |
| Auto format detection | No — DecodeType required | Yes |
| Standalone package | No — UI suite required | Yes |
| Perpetual license | No — subscription only | Yes |

### Detailed Feature Comparison

| Feature | Telerik RadBarcode | IronBarcode |
|---|---|---|
| **Generation** | | |
| Code128, Code39, EAN, UPC | Yes | Yes |
| QR Code generation | Yes | Yes |
| DataMatrix generation | Yes | Yes |
| PDF417 generation | Yes | Yes |
| Aztec generation | Yes | Yes |
| XAML/Razor control | Yes | No (code-based) |
| Code-based generation API | Limited | Yes (`BarcodeWriter`) |
| **Reading** | | |
| Code128, Code39, EAN, UPC reading | WPF + WinForms only | Yes (all platforms) |
| QR code reading | Not available | Yes |
| DataMatrix reading | Not available | Yes |
| PDF417 reading | Not available | Yes |
| Aztec reading | Not available | Yes |
| Auto format detection | No | Yes |
| PDF file reading | Not available | Yes (native) |
| Multi-barcode per image | No | Yes |
| **Platform** | | |
| WPF reading | Yes (1D only) | Yes (all formats) |
| WinForms reading | Yes (1D only) | Yes (all formats) |
| ASP.NET Core reading | Not available | Yes |
| Blazor reading | Not available | Yes |
| Console / Worker Service | Not available | Yes |
| Docker / Linux | Partial | Yes |
| Azure Functions | Partial | Yes |
| Shared service library | Not possible (platform types) | Yes |
| **Licensing** | | |
| Standalone barcode package | No | Yes |
| Subscription model | Yes | Optional |
| Perpetual license | No | Yes |
| Single developer entry price | ~$1,149/yr (suite) | ~$749 perpetual |
| 10-developer price | ~$14,690/yr (DevCraft) | ~$2,999 perpetual |

## Reading Format Support

The boundary between what Telerik RadBarcode can read and what it cannot is determined by the `DecodeType` enum rather than by the image content or the barcode standard.

### Telerik RadBarcodeReader Approach

`RadBarcodeReader` requires explicit format declaration before each read operation. The developer must populate `reader.DecodeTypes` with an array of `DecodeType` values. The enum contains only 1D linear format entries. Passing an image containing a QR code, DataMatrix, or any 2D symbol results in a null return value with no error. The WinForms equivalent, `BarCodeReader`, uses the same `DecodeType` constraint under a slightly different property name (`DecodeType` singular rather than `DecodeTypes` plural).

There is no workaround for this limitation within Telerik's own product family. The generation component (`RadBarcode`) supports QR and other 2D formats. The reading component does not. The two components share a product name but not a format registry.

### IronBarcode Approach

IronBarcode performs format detection automatically on every read call. No format list is required. The same method call that reads a Code128 barcode reads a QR code, a DataMatrix symbol, or a PDF417 stack:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

var results = BarcodeReader.Read(imagePath);
foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.BarcodeType}: {barcode.Value}");
}
```

The `BarcodeType` property on each result identifies which format was detected. No pre-specification is needed. For applications where performance tuning is required, `BarcodeReaderOptions.ExpectedBarcodeTypes` provides optional filtering to narrow the detection scope. For a detailed breakdown of [reading barcodes from images across all formats](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/), the IronBarcode documentation covers the full options including multi-barcode detection and speed tuning.

## Platform Coverage

Telerik RadBarcode's reading capability is tied to two UI framework assemblies, which determines where reading code can run.

### Telerik Approach

Generation through Telerik is available on WPF, WinForms, Blazor, ASP.NET Core, and ASP.NET AJAX. Reading through Telerik is restricted to WPF (via `Telerik.UI.for.Wpf.60.Xaml`) and WinForms (via `Telerik.UI.for.WinForms.Barcode`). No reading component exists for Blazor, ASP.NET Core, console applications, background workers, or server-side deployments of any kind.

The platform restriction also prevents extracting reading logic into a shared .NET library. `RadBarcodeReader` references WPF types (`BitmapImage`) and `BarCodeReader` references WinForms types (`System.Drawing.Image`). A shared service library that imports either reading class cannot compile in a non-WPF, non-WinForms project. Teams that want consistent barcode reading behavior across a WPF desktop client and an ASP.NET Core backend must use different code paths — or different libraries — for each target.

### IronBarcode Approach

IronBarcode uses a single static API that compiles and runs identically across all .NET project types. The same `BarcodeReader.Read(imagePath)` call works in a WPF application, a WinForms form, an ASP.NET Core controller, a Blazor page code-behind, a console application, a Docker container, or an Azure Function:

```csharp
// NuGet: dotnet add package IronBarcode
// Works identically in WPF, WinForms, ASP.NET Core, Blazor, console, Docker, Lambda
using IronBarCode;

var results = BarcodeReader.Read(imagePath);
var value = results.FirstOrDefault()?.Value ?? "No barcode found";
```

No UI framework types are referenced. No platform-specific configuration is required. Barcode reading logic written for a WPF application compiles and runs without modification in an ASP.NET Core endpoint. The [full platform compatibility coverage](https://ironsoftware.com/csharp/barcode/features/compatibility/) includes Docker, Azure, AWS Lambda, macOS, and Linux alongside the Windows UI targets.

## Generation API

Both Telerik RadBarcode and IronBarcode support the same generation formats, but the API model is different in structure and usage context.

### Telerik Approach

Telerik RadBarcode generation is a declarative UI control. In WPF, `RadBarcode` is placed in XAML and configured through attributes. In Blazor, `TelerikBarcode` is a Razor component. The `Symbology` attribute (WPF) or `Type` attribute (Blazor) accepts an enum value from `Symbology` or `BarcodeType`:

```xml
<!-- WPF — declarative XAML generation -->
<Window xmlns:telerik="http://schemas.telerik.com/2008/xaml/presentation">
    <telerik:RadBarcode Value="12345678" Symbology="Code128" />
</Window>
```

```razor
@* Blazor — Razor component generation *@
<TelerikBarcode Value="12345678" Type="@BarcodeType.Code128" />
```

This approach integrates naturally with XAML data binding and Blazor component trees. It is not a code-first API — it does not return an image object, a byte array, or a stream. Exporting the generated barcode as an image requires additional steps through the UI rendering pipeline.

### IronBarcode Approach

IronBarcode generation is code-first. `BarcodeWriter.CreateBarcode()` accepts a value string and a `BarcodeEncoding` enum value and returns a `GeneratedBarcode` object that can be saved, converted to bytes, or converted to a `BitmapSource` for WPF binding:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

// Save directly to file
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
             .SaveAsPng("barcode.png");

// Get as WPF BitmapSource for display
var barcodeImage = BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                                .ToBitmapSource();
```

The same method call covers 1D and 2D generation. Switching from Code128 to QR code changes only the `BarcodeEncoding` argument. For the full range of [2D barcode generation options](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/) including QR error correction levels and DataMatrix sizing, the IronBarcode documentation provides complete examples.

## Licensing Model

### Telerik Approach

Telerik RadBarcode is licensed as part of the Progress Telerik UI suite. There is no standalone barcode license. Access to `RadBarcode` and `RadBarcodeReader` requires an active subscription to the appropriate platform suite:

| Product | Annual Price (2026) |
|---|---|
| UI for WinForms | ~$1,149/year |
| UI for WPF | ~$1,149/year |
| UI for Blazor | ~$1,099/year |
| DevCraft UI (all platforms combined) | ~$1,469/year |

*Pricing as of January 2026. Visit the Telerik pricing page for current rates.*

The subscription is per developer. A team of 10 developers purchasing DevCraft UI pays approximately $14,690 per year. Subscriptions renew annually at these rates. There is no perpetual option that grants permanent use rights without continued payment.

### IronBarcode Approach

IronBarcode offers both perpetual and subscription licensing. The perpetual tiers grant permanent use rights — a license purchased today remains valid without renewal. Annual renewal is optional for continued access to updates. Entry pricing starts at $749 for a single developer with perpetual rights. For teams, the Professional tier covers multiple developers at a fixed one-time cost.

IronBarcode's [licensing page](https://ironsoftware.com/csharp/barcode/licensing/) details the current tier structure, including royalty-free redistribution options and SaaS deployment licensing.

## API Mapping Reference

| Telerik RadBarcode | IronBarcode |
|---|---|
| `TelerikLicenseManager.InstallLicense("key")` | `IronBarCode.License.LicenseKey = "key"` |
| `<telerik:RadBarcode Value="..." Symbology="Code128" />` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)` |
| `<TelerikBarcode Type="@BarcodeType.Code128" />` | Server-side generation returning `ToPngBinaryData()` |
| `Symbology.Code128` | `BarcodeEncoding.Code128` |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` |
| `new RadBarcodeReader()` (WPF) | Static class — no instance needed |
| `new BarCodeReader()` (WinForms) | Static class — no instance needed |
| `reader.DecodeTypes = new DecodeType[] { ... }` | Auto-detection — no specification needed |
| `reader.Decode(bitmapImage)` (WPF) | `BarcodeReader.Read(imagePath)` |
| `reader.Read(drawingImage)` (WinForms) | `BarcodeReader.Read(imagePath)` |
| `result.Text` | `result.Value` |
| `result.Symbology` | `result.BarcodeType` |
| 1D formats only (no QR, DataMatrix, PDF417) | 50+ formats including all 2D types |
| WPF and WinForms reading only | All .NET platforms |

## When Teams Consider Moving from Telerik RadBarcode to IronBarcode

Teams that evaluate IronBarcode as an alternative to Telerik RadBarcode typically do so in response to a specific requirement that the existing tooling cannot satisfy.

### QR Code Reading Requirements

A common trigger is the arrival of a QR code reading requirement in an application that already uses `RadBarcode` for QR generation. The developer opens `RadBarcodeReader`, attempts to configure `DecodeTypes` to include QR, and discovers that no such entry exists in the enum. This is not a documentation gap or a configuration option that can be enabled — the format is absent from the reading engine. Teams that need to validate printed QR codes, process inbound QR-encoded payloads, or close the loop between generation and reading within a single application reach the boundary of what Telerik can provide at this point.

### PDF Barcode Processing

Production barcode workflows often involve documents rather than standalone image files. Invoices, shipping manifests, identity documents, and regulatory filings frequently embed barcodes in PDF pages. A team whose workflow must extract barcode values from PDF content will find that `RadBarcodeReader` accepts only bitmap images and has no mechanism for processing PDF files. The workaround — converting each PDF page to an image externally, then feeding those images to `RadBarcodeReader` one at a time — introduces additional dependencies, increases processing time, and still produces only 1D results. Teams that identify this gap at the architecture stage often evaluate libraries that handle PDF reading natively before committing to the integration cost of a multi-library approach.

### Cross-Platform Consistency

Applications that span a WPF desktop client and an ASP.NET Core backend cannot share barcode reading logic when that logic depends on Telerik types. `RadBarcodeReader` references WPF-specific types and does not compile in a non-WPF project. `BarCodeReader` references WinForms types. A shared service that performs barcode reading cannot reference either class without creating a platform dependency. Teams that want a single barcode reading implementation in a shared library — usable from both the desktop client and the web service — require a library without UI framework dependencies. This typically surfaces when teams apply clean architecture patterns or attempt to extract business logic into separate assemblies.

### Licensing Cost Structure

Teams that purchased a Telerik UI suite primarily for barcode functionality, or that discover the barcode capability is included in a suite they purchased for other reasons, sometimes re-evaluate their options when assessing renewal costs. A development team using Telerik barcode components embedded in a larger suite may find that the annual per-developer cost is difficult to justify when the barcode feature set does not cover the full reading requirements. Teams that need barcode functionality without the broader UI suite — particularly teams building server-side or headless applications where UI controls provide no value — often explore standalone barcode libraries as a cost-effective alternative.

## Common Migration Considerations

Teams transitioning from Telerik RadBarcode to IronBarcode encounter a consistent set of technical changes across the codebase.

### XAML Control Replacement

`RadBarcode` in XAML is a visual control that renders inline in the UI. IronBarcode generates barcode images in code and returns a `GeneratedBarcode` object. The migration pattern for a WPF form is to replace the `<telerik:RadBarcode>` element with an `<Image>` control and generate the barcode in the code-behind using `BarcodeWriter.CreateBarcode(...).ToBitmapSource()`. For Blazor, the `<TelerikBarcode>` component is replaced with server-side generation that returns the barcode as a base64-encoded `<img>` source.

### DecodeType Removal

The `reader.DecodeTypes = new DecodeType[] { ... }` block present in all `RadBarcodeReader` usage is removed entirely during migration. IronBarcode does not require format pre-specification. If the existing `DecodeType` list contained only a subset of 1D formats, IronBarcode will now detect additional formats present in the same images. This is expected behavior. If narrower detection is needed for performance, `BarcodeReaderOptions.ExpectedBarcodeTypes` provides optional filtering without requiring the format list to be declared at the reader instance level.

### TelerikLicenseManager Replacement

`TelerikLicenseManager.InstallLicense(...)` calls at application startup are replaced with a single `IronBarCode.License.LicenseKey = "YOUR-KEY"` assignment. IronBarcode requires no license file on disk and no platform-specific initialization path. The single key assignment works identically in WPF `App.xaml.cs`, ASP.NET Core `Program.cs`, and any other application entry point.

## Additional IronBarcode Capabilities

Beyond the reading and generation features covered in this comparison, IronBarcode includes additional capabilities that may be relevant depending on the application type:

- **[Multi-Barcode Detection](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/):** A single `BarcodeReader.Read()` call returns all barcodes present in an image, including mixed 1D and 2D formats.
- **Stream and Byte Array Input:** `BarcodeReader.Read()` accepts file paths, `Stream` objects, and byte arrays — useful for processing uploaded files in ASP.NET Core without writing to disk.
- **Barcode Styling:** `GeneratedBarcode` supports annotation text, margin adjustment, color customization, and resizing before output.
- **[Supported Barcode Formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/):** The full format catalog covers 50+ symbologies across both reading and generation, including GS1-128, MicroQR, and MaxiCode.
- **Image Enhancement for Reading:** Built-in preprocessing options improve read accuracy on low-quality, rotated, or partially obscured barcodes.
- **Output Flexibility:** Generated barcodes can be exported as PNG, JPEG, BMP, GIF, TIFF, PDF, SVG, or returned as binary data.

## .NET Compatibility and Future Readiness

IronBarcode supports .NET 8, .NET 9, and is actively maintained with updates that track new .NET releases. As .NET 10 adoption increases through 2026, IronBarcode's regular release cadence ensures compatibility with current and upcoming runtime versions. The library targets .NET Standard 2.0 for broad project compatibility, meaning it compiles in legacy .NET Framework projects alongside modern .NET targets. Telerik UI suite components are also actively maintained and receive platform updates, though barcode reading capabilities have remained bounded by the 1D-only `DecodeType` architecture across recent release cycles.

## Conclusion

Telerik RadBarcode and IronBarcode represent different answers to what a barcode library is. Telerik RadBarcode is a UI control embedded in a comprehensive commercial control suite. Its generation capabilities are broad and its visual integration with WPF and Blazor UIs is polished. Its reading component is 1D-only and restricted to two desktop UI frameworks. These boundaries are architectural, not incidental — the product was designed as a UI component, and the reading engine reflects that scope.

IronBarcode is a purpose-built barcode library without UI framework dependencies. It handles 1D and 2D reading and generation through a unified static API that compiles and runs identically across every .NET project type. The reading engine detects formats automatically, processes PDF files natively, and operates in server, cloud, and containerized contexts where Telerik's UI framework requirement cannot be satisfied.

For teams already invested in the Telerik UI suite — using its grids, charts, schedulers, and other controls extensively — RadBarcode adds generation capability at no marginal cost. If reading requirements stay within 1D formats and desktop platforms, the existing suite covers that need. The value of Telerik lies in its breadth of UI controls, and barcode functionality is a small portion of what the suite provides.

For teams whose barcode requirements include 2D reading, PDF processing, server-side operation, or cross-platform consistency, IronBarcode addresses those needs directly. The structural gap between what `RadBarcode` generates and what `RadBarcodeReader` can read is not addressable through configuration — it is the fundamental design boundary that defines where Telerik's barcode capability ends and where a dedicated barcode library becomes the appropriate tool.
