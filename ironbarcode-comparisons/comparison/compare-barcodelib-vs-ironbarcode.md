BarcodeLib has been downloaded over 12 million times. Most of those developers eventually discover it can only create barcodes. A significant portion of them also discover the SkiaSharp conflict — usually at the worst possible moment, when they have just added BarcodeLib to a MAUI or Blazor project that already depends on SkiaSharp, and the build starts throwing NU1608 warnings they did not expect to be debugging that afternoon. This article covers both issues honestly. BarcodeLib is a legitimate library with a clear use case. Understanding where it stops is the practical question.

## Understanding BarcodeLib

BarcodeLib is an open-source barcode image generation library for .NET, maintained on GitHub by Brad Barnhill. It has been active since 2007 and supports over 25 barcode symbologies. The Apache 2.0 license makes it free for commercial use. For pure barcode generation — creating an image from a string — it works reliably and has served that purpose well across many years of active use.

The API is instance-based. You create a `Barcode` object, set properties, and call `Encode()` with a type constant and a data string. The result is a `System.Drawing.Image` that you then save or stream as needed. This workflow is clean and approachable, and for projects where the requirement is strictly printing barcode images — shipping labels, inventory tags, retail price tags — it is sufficient.

Key architectural characteristics of BarcodeLib:

- **Generation-only scope:** The library has no reading or decoding API of any kind; its entire public surface area is oriented toward producing images from data strings
- **Instance-based API:** Each operation requires instantiating a `Barcode` object and setting width, height, and label properties before calling `Encode()`
- **Returns `System.Drawing.Image`:** Output is a GDI+ image object, which requires a `MemoryStream` intermediate step to produce byte array output for HTTP responses or database storage
- **1D and QR generation only:** Supports over 25 symbologies including Code128, EAN-13, UPC-A, Code39, and QR Code, but offers no 2D reading capability
- **SkiaSharp dependency (v3.x):** The 3.x series replaced `System.Drawing.Common` with SkiaSharp to enable cross-platform support; this introduces version conflict risk when other packages in the project also depend on SkiaSharp
- **Free, no license key required:** Apache 2.0 license covers commercial use with no runtime key or activation needed

### BarcodeLib Core Generation Pattern

The standard BarcodeLib generation workflow requires creating an instance, configuring properties, and calling `Encode()`:

```csharp
// BarcodeLib
using BarcodeLib;
using System.Drawing;
using System.Drawing.Imaging;

var b = new Barcode();
b.IncludeLabel = true;
b.Width = 300;
b.Height = 100;
Image img = b.Encode(TYPE.CODE128, "PRODUCT-12345");
img.Save("barcode.png", ImageFormat.Png);
```

This is the full generation workflow. The property-setter pattern is the design: all configuration happens on the instance before the `Encode()` call, and the return value is a `System.Drawing.Image` that must be separately saved or converted to bytes.

## Understanding IronBarcode

IronBarcode is a commercial .NET barcode library that covers both generation and reading in a single package. It installs via NuGet, runs on .NET Framework 4.6.2 through .NET 9, and works on Windows, Linux, macOS, Docker, Azure, and AWS Lambda. The library is developed and maintained by Iron Software with a commercial support model.

The generation API is static and fluent — no instance to create, no properties to set before the primary call. Configuration options chain onto the result of `BarcodeWriter.CreateBarcode()` or `QRCodeWriter.CreateQrCode()`. Output methods at the end of the chain — `.SaveAsPng()`, `.ToPngBinaryData()`, `.ToAnyImageData()` — eliminate the intermediate `MemoryStream` pattern that BarcodeLib requires. Reading is part of the same package with no separate library or ZXing.Net integration to maintain.

Key characteristics of IronBarcode:

- **Generation and reading in one package:** `BarcodeWriter` handles generation; `BarcodeReader` handles reading; both ship in the same NuGet install
- **Static fluent API:** No instantiation required; configuration chains after `CreateBarcode()` using fluent methods
- **Direct byte array output:** `.ToPngBinaryData()` returns `byte[]` without a `MemoryStream` step
- **PDF support on both ends:** `BarcodeReader.Read()` accepts PDF files natively; generation output can be embedded in PDFs
- **No SkiaSharp dependency:** Independent of the SkiaSharp version graph, eliminating NU1608 conflicts in MAUI and other projects
- **Commercial license with SLA:** Priced at $749–$5,999 perpetual; includes commercial support and guaranteed update cadence
- **`ReadingSpeed` tuning:** `BarcodeReaderOptions` allows trading scan thoroughness for performance at volume

## Feature Comparison

| Feature | BarcodeLib | IronBarcode |
|---|---|---|
| Barcode generation | Yes | Yes |
| Barcode reading | No | Yes |
| PDF barcode reading | No | Yes |
| SkiaSharp dependency conflict | Yes (v3.x) | No |
| Fluent chainable API | No | Yes |
| License | Apache 2.0 (free) | $749–$5,999 perpetual |

### Detailed Feature Comparison

| Feature | BarcodeLib | IronBarcode |
|---|---|---|
| **Generation** | | |
| Code128 generation | Yes | Yes |
| EAN-13 / UPC-A generation | Yes | Yes |
| QR code generation | Yes (basic) | Yes (advanced, with logo embedding) |
| 25+ symbologies | Yes | Yes |
| Fluent chainable generation API | No | Yes |
| Direct `byte[]` output | Manual (`MemoryStream`) | `.ToPngBinaryData()` |
| PDF generation output | No | Yes |
| **Reading** | | |
| Barcode reading from image | No | Yes (`BarcodeReader.Read()`) |
| Barcode reading from PDF | No | Yes (native, no extra library) |
| Multi-barcode detection | No | Yes (`ExpectMultipleBarcodes`) |
| Reading speed tuning | N/A | Yes (`ReadingSpeed` enum) |
| **Platform** | | |
| Windows | Yes | Yes |
| Linux / macOS | Partial (SkiaSharp-dependent) | Full |
| Docker / container | Configuration required | Yes |
| MAUI project compatibility | Conflict risk (NU1608) | No conflict |
| .NET Framework 4.6.2+ | Yes | Yes |
| .NET 6–9 | Yes (SkiaSharp 3.x required) | Yes |
| **Licensing** | | |
| Open source / free | Yes (Apache 2.0) | No |
| Commercial support / SLA | No | Yes |
| License key required | No | Yes |
| Pricing | Free | $749–$5,999 perpetual |

## Barcode Generation API

The generation APIs represent different design philosophies: BarcodeLib uses mutable instance configuration while IronBarcode uses an immutable fluent chain.

### BarcodeLib Approach

BarcodeLib requires constructing an instance and setting properties before calling `Encode()`. Output is a `System.Drawing.Image` object:

```csharp
using BarcodeLib;
using System.Drawing;
using System.Drawing.Imaging;

public byte[] GenerateCode128(string data)
{
    var b = new Barcode();
    b.IncludeLabel = true;
    b.Width = 300;
    b.Height = 100;
    Image img = b.Encode(TYPE.CODE128, data);

    using var ms = new MemoryStream();
    img.Save(ms, ImageFormat.Png);
    return ms.ToArray();
}
```

The `System.Drawing.Image` return type means byte array output requires a `MemoryStream` intermediary. The `IncludeLabel` property is a boolean toggle — BarcodeLib automatically renders the encoded data string as the visible label beneath the bars.

### IronBarcode Approach

IronBarcode's generation is fully static. Configuration chains after `CreateBarcode()`, and output methods terminate the chain directly:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public byte[] GenerateCode128(string data)
{
    return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ResizeTo(300, 100)
        .AddAnnotationTextBelowBarcode(data)
        .ToPngBinaryData();
}
```

`.ToPngBinaryData()` returns the byte array directly — no intermediate `Image` or `MemoryStream` object. `.AddAnnotationTextBelowBarcode()` takes the label string explicitly, giving control over what text appears below the bars. For advanced generation scenarios, see the [IronBarcode barcode generation documentation](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/).

## Barcode Reading Capability

Reading is the most significant functional boundary between these two libraries. BarcodeLib has no reading capability; IronBarcode includes a full reading engine in the same package.

### BarcodeLib Approach

BarcodeLib has no reading API. There is no `Decode()`, `Scan()`, or `ReadBarcode()` method. The absence is not a version difference — reading has never been part of the library's design. Any attempt to call a decode method results in a compile error:

```csharp
// BarcodeLib — reading does not exist
using BarcodeLib;

var b = new Barcode();

// This will not compile — there is no Decode or Read method
// var value = b.Decode("barcode.png");       // CS1061: no definition
// var value = b.Scan("barcode.png");          // CS1061: no definition
// var value = b.ReadBarcode("barcode.png");   // CS1061: no definition

// The only thing you can do is generate:
Image img = b.Encode(TYPE.CODE128, "PRODUCT-12345");  // this works
```

Teams that need both generation and reading alongside BarcodeLib must add a second library — typically ZXing.Net — which introduces its own dependency management burden and a second API surface to maintain.

### IronBarcode Approach

`BarcodeReader.Read()` accepts image files, PDF files, streams, and `System.Drawing.Bitmap` objects. Reading from a PDF requires no additional library:

```csharp
using IronBarCode;

// Read from an image
var results = BarcodeReader.Read("barcode.png");
Console.WriteLine(results.First().Value);  // "PRODUCT-12345"

// Read all barcodes from a PDF — no separate PDF library needed
var pdfResults = BarcodeReader.Read("invoice-batch.pdf");
foreach (var result in pdfResults)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
}

// Tune reading for speed vs. thoroughness
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};
var multiResults = BarcodeReader.Read("warehouse-scan.png", options);
```

The `ReadingSpeed` enum allows tuning scan performance for high-volume scenarios. For guidance on reading configuration, see the [IronBarcode reading documentation](https://ironsoftware.com/csharp/barcode/how-to/read-barcode-from-image/).

## SkiaSharp Dependency Conflict

The SkiaSharp dependency introduced in BarcodeLib 3.x creates a class of conflict that does not exist in IronBarcode.

### BarcodeLib Approach

Starting with BarcodeLib 3.x, the library introduced SkiaSharp as a graphics backend to replace `System.Drawing.Common`, which became Windows-only after .NET 6. BarcodeLib pins to a specific SkiaSharp version range. If a project already uses SkiaSharp through another dependency — common in MAUI projects using `SkiaSharp.Views.Maui` and `Microsoft.Maui.Graphics` — the resolved version may fall outside BarcodeLib's expected range. The result is a NU1608 warning at minimum and an assembly binding failure at runtime at worst:

```
warning NU1608: Detected package version outside of dependency constraint:
BarcodeLib 3.1.5 requires SkiaSharp (>= 2.88.7 && < 2.89.0) but
version SkiaSharp 3.116.1 was resolved.
```

Forcing a resolution through explicit package references adds complexity without a guarantee:

```xml
<!-- Attempt to force SkiaSharp version — may or may not resolve the conflict -->
<ItemGroup>
  <PackageReference Include="BarcodeLib" Version="3.1.5" />
  <!-- Override to satisfy MAUI's SkiaSharp requirements -->
  <PackageReference Include="SkiaSharp" Version="3.116.1" />
  <PackageReference Include="SkiaSharp.Views.Maui.Controls" Version="3.116.1" />
</ItemGroup>
```

Even with explicit overrides, compatibility depends on BarcodeLib's internal API calls matching what the pinned version exposes. There is no support contract that guarantees a fix on a schedule that matters to the project.

### IronBarcode Approach

IronBarcode does not share the SkiaSharp dependency graph with application code. There is no version negotiation to manage, no NU1608 to diagnose, and no runtime assembly binding risk tied to SkiaSharp version resolution. MAUI projects, Blazor projects, and any other application that depends on SkiaSharp can install IronBarcode without any version conflict. For MAUI-specific integration patterns, see the [IronBarcode MAUI documentation](https://ironsoftware.com/csharp/barcode/how-to/maui-barcode-scanner/).

## API Mapping Reference

| BarcodeLib | IronBarcode |
|---|---|
| `new Barcode()` | Static API — no instance required |
| `b.Encode(TYPE.CODE128, "data")` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.Code128)` |
| `b.IncludeLabel = true` | `.AddAnnotationTextBelowBarcode("text")` |
| `b.Width = 300; b.Height = 100` | `.ResizeTo(300, 100)` |
| Returns `System.Drawing.Image` | `.SaveAsPng(path)` / `.ToPngBinaryData()` |
| `TYPE.CODE128` | `BarcodeEncoding.Code128` |
| `TYPE.CODE39` | `BarcodeEncoding.Code39` |
| `TYPE.EAN13` | `BarcodeEncoding.EAN13` |
| `TYPE.UPCA` | `BarcodeEncoding.UPCA` |
| `TYPE.QR_Code` | `BarcodeEncoding.QRCode` (also `QRCodeWriter`) |
| No reading API | `BarcodeReader.Read(path)` |
| SkiaSharp version conflict in MAUI | No conflicting dependencies |

## When Teams Consider Moving from BarcodeLib to IronBarcode

### Reading Requirement Appears

A system that has generated shipping labels for months receives a new requirement: the application must also process returned labels from suppliers. The warehouse integration needs to parse barcodes from inbound shipment manifests. A document management system needs to index barcodes on scanned PDFs. BarcodeLib cannot fulfill any of these requirements — the reading API does not exist. The team evaluates adding ZXing.Net alongside BarcodeLib, weighs the dual-library maintenance burden and the two separate dependency graphs, and decides the cleaner path is a library that handles both generation and reading under a single NuGet install.

### SkiaSharp Conflict in a MAUI Project

A team adds BarcodeLib to an existing MAUI application and immediately encounters NU1608 warnings during restore. They investigate, identify the version mismatch between BarcodeLib's expected SkiaSharp range and the version MAUI requires, add explicit `<PackageReference>` overrides to force a resolution, and get the build to pass. Then they encounter a runtime crash on device when SkiaSharp's native binaries load the wrong version. The fix requires deeper investigation into the assembly binding log. Switching to IronBarcode removes the conflict at the root — not by finding a compatible SkiaSharp version, but by eliminating the shared dependency entirely.

### PDF Barcode Processing Required

Applications that generate PDF documents with embedded barcodes — invoices, work orders, shipping manifests — sometimes need to read those barcodes back during downstream processing. BarcodeLib generates barcode images but has no PDF support on either end. Reading barcodes from a PDF with BarcodeLib requires rendering the PDF to images first using a separate PDF library, then passing those images to a separate reading library. IronBarcode handles the full chain natively: `BarcodeReader.Read("file.pdf")` traverses every page and returns all detected barcodes without an intermediate rendering step.

### QR Code Features Outgrow Basic Generation

Projects that initially required only basic QR code generation often evolve to need logo embedding, color customization, or error correction level configuration. BarcodeLib supports QR codes through `TYPE.QR_Code` but provides no options beyond the standard `Width`, `Height`, and `IncludeLabel` properties. IronBarcode's `QRCodeWriter` exposes logo embedding, color control, and error correction tuning through chained methods. Teams whose QR code requirements have expanded past what BarcodeLib's basic implementation supports find that the feature gap drives the migration decision.

## Common Migration Considerations

### Instance API to Static Fluent API

BarcodeLib code uses a mutable object pattern: create a `Barcode` instance, set properties, call `Encode()`. IronBarcode uses a static fluent pattern: call `BarcodeWriter.CreateBarcode()`, chain configuration methods, terminate with an output method. Existing code that stores a `Barcode` instance as a field or passes it between methods will need restructuring. The typical change is replacing the property-setter block with a method chain:

```csharp
// The property setters on b become chained methods
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .AddAnnotationTextBelowBarcode(data)
    .SaveAsPng(outputPath);
```

### System.Drawing.Image to Direct Output

BarcodeLib returns `System.Drawing.Image`, which requires `MemoryStream` to produce bytes. Any code typed to `Image` or `System.Drawing.Image` will need updating. IronBarcode's fluent chain terminates with the desired output format directly — `.SaveAsPng()`, `.ToPngBinaryData()`, `.ToAnyImageData()` — removing the need for the intermediate image object and the `MemoryStream` step.

### TYPE Enum to BarcodeEncoding Enum

BarcodeLib uses the `TYPE` class with uppercase constants such as `TYPE.CODE128`. IronBarcode uses the `BarcodeEncoding` enum with PascalCase values such as `BarcodeEncoding.Code128`. The values map directly. A grep for `TYPE\.` across `.cs` files identifies all occurrences that need updating, and a systematic find-and-replace covers the common formats: `CODE128` → `Code128`, `EAN13` → `EAN13`, `UPCA` → `UPCA`, `QR_Code` → `QRCode`.

### SkiaSharp Reference Cleanup

Projects that added explicit `<PackageReference Include="SkiaSharp">` entries only to resolve BarcodeLib's NU1608 warnings can remove those overrides after switching to IronBarcode. The `dotnet list package --include-transitive` command confirms whether SkiaSharp is still required by other packages in the project before removing it.

## Additional IronBarcode Capabilities

Beyond the direct generation and reading comparison, IronBarcode includes capabilities that BarcodeLib does not address:

- **[QR code logo embedding](https://ironsoftware.com/csharp/barcode/how-to/create-qr-code-with-logo/):** `QRCodeWriter.CreateQrCode().AddBrandLogo("logo.png")` embeds a brand logo at the center of a QR code with automatic error correction adjustment
- **[PDF barcode extraction](https://ironsoftware.com/csharp/barcode/how-to/read-barcode-from-pdf/):** `BarcodeReader.Read("file.pdf")` reads barcodes from every page of a PDF document without a separate PDF rendering library
- **[Multi-barcode detection](https://ironsoftware.com/csharp/barcode/how-to/read-multiple-barcodes/):** `BarcodeReaderOptions.ExpectMultipleBarcodes = true` detects and returns all barcodes present in a single image
- **[Reading speed configuration](https://ironsoftware.com/csharp/barcode/how-to/read-barcode-from-image/):** `ReadingSpeed.Faster`, `ReadingSpeed.Balanced`, and `ReadingSpeed.ExtremeDetail` tune the scan engine for throughput vs. accuracy
- **[Styled QR code generation](https://ironsoftware.com/csharp/barcode/how-to/create-qr-code/):** Color, finder pattern style, and error correction level are configurable through chained methods on `QRCodeWriter`
- **[Stream and Bitmap input for reading](https://ironsoftware.com/csharp/barcode/how-to/read-barcode-from-image/):** `BarcodeReader.Read()` accepts file paths, streams, `System.Drawing.Bitmap`, and `AnyBitmap` inputs

## .NET Compatibility and Future Readiness

IronBarcode supports .NET Framework 4.6.2 through .NET 9 and maintains regular release cadence aligned with Microsoft's .NET release schedule. As .NET 10 adoption increases through 2026, IronBarcode's active development ensures forward compatibility without requiring project changes. The library runs without modification on Windows, Linux, macOS, Docker, Azure, and AWS Lambda. BarcodeLib also maintains active community development, though its cross-platform support in the 3.x series depends on the SkiaSharp version compatibility discussed in the comparison sections above. For projects targeting modern .NET on Linux or in containers, IronBarcode's dependency-free cross-platform architecture avoids the version negotiation that BarcodeLib's SkiaSharp backend introduces.

## Conclusion

BarcodeLib and IronBarcode represent different scopes of solution for barcode work in .NET. BarcodeLib is a focused, free, generation-only library that has served its defined use case reliably for nearly two decades. IronBarcode is a commercial library that covers both generation and reading, with a static fluent API and no SkiaSharp dependency. The difference is not one of quality within shared scope — it is one of scope itself.

BarcodeLib remains a genuinely appropriate choice for projects with stable, generation-only requirements on Windows or in environments where the SkiaSharp version landscape is controlled. Its Apache 2.0 license, zero cost, and straightforward API make it a practical solution for shipping label systems, inventory tag generators, and similar applications that will never need to scan barcodes. The 12 million downloads reflect that a large segment of .NET developers have exactly this use case.

IronBarcode becomes the more practical choice when requirements move beyond pure image generation: when reading capability is needed, when the project is a MAUI or cross-platform application where SkiaSharp version conflicts are likely, when PDF barcode processing is on the roadmap, or when QR code features beyond basic generation are required. The commercial license cost represents the threshold question — for teams whose requirements align with what IronBarcode adds over BarcodeLib, the single-package solution and commercial SLA are the value exchange.

The honest evaluation is that most teams do not start with IronBarcode from the beginning. They start with BarcodeLib because it is free and sufficient. They migrate to IronBarcode when their requirements grow past what BarcodeLib's generation-only scope supports. The migration is well-documented and the API surface changes are predictable. Understanding where BarcodeLib stops — and specifically that it stops at generation — is the practical information needed to make that timing decision correctly.
