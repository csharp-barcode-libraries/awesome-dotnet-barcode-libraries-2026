NetBarcode's `Type` enum has no QR Code entry. When a shipping label project adds a QR code requirement in month three, developers reach for a second library. That second library brings its own API surface, its own release schedule, and a shared SixLabors.ImageSharp dependency whose version may drift out of alignment with the version NetBarcode already requires. This comparison examines what NetBarcode is, where it fits well, and where [IronBarcode](https://ironsoftware.com/csharp/barcode/) covers the gaps without adding a second or third package.

## Understanding NetBarcode

NetBarcode is an open-source .NET barcode generation library published under the MIT license. It was built to produce linear barcode images from string data, and it fulfils that purpose cleanly. The library targets Code128, EAN-13, UPC-A, and ten other 1D formats — all exposed through a simple constructor and a small set of output methods. Its dependency on SixLabors.ImageSharp provides the image rendering layer, and since version 1.8 that dependency is reflected in the public API through the `Image<Rgba32>` return type on `GetImage()`.

NetBarcode does not attempt to be a general-purpose barcode toolkit. It has no reading capability and no 2D format support. These are deliberate scope decisions. The library is well-suited to applications that need 1D barcodes and nothing else, and its MIT license makes adoption straightforward in open-source contexts.

Key architectural characteristics:

- **MIT License:** The library itself is MIT-licensed, though its SixLabors.ImageSharp dependency carries a split commercial licence that applies at a revenue threshold
- **1D-Only Design:** The `Type` enum defines exactly 14 barcode formats, all linear; there are no 2D entries
- **SixLabors.ImageSharp Dependency:** Image rendering is delegated to ImageSharp, and since v1.8 the `GetImage()` method returns `Image<Rgba32>`, exposing ImageSharp's type directly in the public API
- **Constructor-Based API:** Barcodes are created with `new Barcode(data, Type.X)` and saved or retrieved with `SaveImageFile()` or `GetImage()`
- **No Reading API:** NetBarcode is generation-only; there is no method or class for decoding barcode images
- **No Batch Processing:** Each barcode is an independent constructor call; no built-in enumeration or batch pipeline

### The Type Enum Design Boundary

The `Type` enum is the authoritative list of what NetBarcode can generate. Inspecting it reveals the scope of the library:

```csharp
// NetBarcode Type enum — complete list as of v1.8
public enum Type
{
    Code128,
    Code128A,
    Code128B,
    Code128C,
    Code39,
    Code39Extended,
    Code93,
    EAN8,
    EAN13,
    UPCA,
    UPCE,
    Codabar,
    ITF,
    MSI
}

// These entries do not exist — attempting to use them produces a CS0117 compile error:
// Type.QRCode      — does not exist
// Type.DataMatrix  — does not exist
// Type.PDF417      — does not exist
// Type.Aztec       — does not exist
```

This is not a missing feature pending a pull request. The enum has fourteen entries, all 1D, and that reflects the library's intended scope. Any application requiring QR codes, DataMatrix, PDF417, or Aztec must obtain a separate package to supply those formats.

## Understanding IronBarcode

IronBarcode is a commercial .NET barcode library that covers both generation and reading in a single package. It is developed and maintained by Iron Software with regular updates targeting current .NET versions. The library's static API surface is designed so that switching from one barcode format to another requires only changing a single constant — the same `BarcodeWriter.CreateBarcode` call that generates Code128 also generates QR codes, DataMatrix, PDF417, and Aztec.

IronBarcode handles barcode reading through the `BarcodeReader` class, which accepts image files and PDF documents and returns decoded results with format identification. This means generation and reading share a single dependency, a single license, and a single set of release notes to track.

Key characteristics:

- **Unified Generation and Reading:** Both `BarcodeWriter` and `BarcodeReader` are included in a single NuGet package
- **50+ Supported Formats:** 1D formats include all NetBarcode equivalents; 2D formats include QR Code, DataMatrix, PDF417, Aztec, and others
- **Fluent Chain API:** `BarcodeWriter.CreateBarcode(data, encoding)` returns a `GeneratedBarcode` object with output methods including `SaveAsPng()`, `SaveAsJpeg()`, `ToPngBinaryData()`, and stream-based overloads
- **No ImageSharp Dependency:** IronBarcode's image rendering is self-contained; no SixLabors transitive dependency is introduced
- **PDF Support:** The reading API accepts `.pdf` files directly in addition to image formats
- **Commercial License:** A license key is required; trial mode is available and removes watermarks upon purchase

## Feature Comparison

| Feature | NetBarcode | IronBarcode |
|---|---|---|
| 1D barcode generation | Yes | Yes |
| 2D barcode generation | No | Yes |
| Barcode reading | No | Yes |
| PDF support | No | Yes |
| Total symbologies | 14 | 50+ |
| ImageSharp dependency | Yes (split licence) | No |
| License model | MIT (+ ImageSharp conditions) | Commercial |

### Detailed Feature Comparison

| Feature | NetBarcode | IronBarcode |
|---|---|---|
| **Generation** | | |
| Code128, EAN-13, UPC-A, Code39 | Yes | Yes |
| EAN-8, UPC-E, Code93, Codabar, ITF, MSI | Yes | Yes |
| QR Code | No | Yes |
| DataMatrix | No | Yes |
| PDF417 | No | Yes |
| Aztec | No | Yes |
| GS1-128, GS1 DataBar | No | Yes |
| Postal formats (Intelligent Mail, Royal Mail) | No | Yes |
| SVG output | No | Yes |
| **Reading** | | |
| Decode barcode images | No | Yes |
| Read from PDF documents | No | Yes |
| Multi-barcode detection | No | Yes |
| Automatic format detection | No | Yes |
| **API Design** | | |
| Constructor-based creation | Yes | No (static method) |
| Fluent output chain | No | Yes |
| Batch processing support | Manual | Built-in |
| **Licensing and Dependencies** | | |
| Library licence | MIT | Commercial |
| ImageSharp dependency | Yes | No |
| Commercial support | Community | Professional |

## Format Coverage

### NetBarcode Approach

NetBarcode provides 14 linear barcode formats through the `Type` enum. Within that scope, format selection is straightforward — pass the appropriate enum member to the constructor. The boundary is equally clear: attempting to use a format outside the enum produces a compile-time error.

```csharp
// NetBarcode — formats that compile and produce output
using NetBarcode;

var code128 = new Barcode("12345678901234", Type.Code128);
code128.SaveImageFile("shipping.png");

var ean13 = new Barcode("5901234123457", Type.EAN13);
ean13.SaveImageFile("product.png");

// NetBarcode — formats that produce CS0117 compile errors
// var qr     = new Barcode("data", Type.QRCode);     // error CS0117
// var dm     = new Barcode("data", Type.DataMatrix);  // error CS0117
// var p417   = new Barcode("data", Type.PDF417);      // error CS0117
// var aztec  = new Barcode("data", Type.Aztec);       // error CS0117
```

Industries where this boundary becomes a constraint include pharmaceutical tracking (DataMatrix required under FDA 2D barcode mandates), airline boarding passes (Aztec), logistics manifests (PDF417), and mobile marketing (QR Code). Each of these requirements eliminates NetBarcode as a standalone solution.

### IronBarcode Approach

IronBarcode exposes all supported formats through the same `BarcodeWriter.CreateBarcode` method. The API surface does not change when moving from a 1D format to a 2D format — only the `BarcodeEncoding` constant differs.

```csharp
using IronBarCode;

// 1D formats — identical API to the 2D examples below
BarcodeWriter.CreateBarcode("12345678901234", BarcodeEncoding.Code128)
    .SaveAsPng("shipping.png");

BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
    .SaveAsPng("product.png");

// 2D formats — same method, different encoding constant
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");

BarcodeWriter.CreateBarcode("01034531200000111719112510ABCD1234", BarcodeEncoding.DataMatrix)
    .SaveAsPng("pharma-label.png");

BarcodeWriter.CreateBarcode("M1DOE/JOHN MR ABC123 JFKLHR 0012 123Y015A0001 100", BarcodeEncoding.Aztec)
    .SaveAsPng("boarding-pass.png");
```

The complete list of supported constants is available in the [supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) reference, covering all [2D barcode generation](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/) formats alongside the full 1D set.

## Generation API Design

### NetBarcode Approach

NetBarcode's generation model is constructor-based. A `Barcode` object is instantiated with the data string and a `Type` enum value. Output is either saved directly with `SaveImageFile()` or retrieved as an `Image<Rgba32>` via `GetImage()`. Since version 1.8, the return type of `GetImage()` is the SixLabors.ImageSharp type, which means any code that stores or processes the return value must import and work within the ImageSharp API.

```csharp
using NetBarcode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

// Constructor-based creation
var barcode = new Barcode("12345678901234", Type.Code128);

// Save to file — straightforward
barcode.SaveImageFile("code128.png");

// GetImage() returns Image<Rgba32> — ImageSharp import required
Image<Rgba32> image = barcode.GetImage();

// Further processing requires familiarity with the ImageSharp API
using var stream = new MemoryStream();
image.SaveAsPng(stream);
byte[] bytes = stream.ToArray();
```

The `GetImage()` method's return type ties downstream code to the ImageSharp library. Any method that accepts or stores the result must declare it as `Image<Rgba32>`, introducing a transitive dependency into calling code.

### IronBarcode Approach

IronBarcode uses a fluent chain. `BarcodeWriter.CreateBarcode` returns a `GeneratedBarcode` object that carries multiple output methods. Image handling is internal — no ImageSharp type is exposed to the calling code.

```csharp
using IronBarCode;

// Fluent generation — save directly to file
BarcodeWriter.CreateBarcode("12345678901234", BarcodeEncoding.Code128)
    .SaveAsPng("code128.png");

// Multiple output options on the same GeneratedBarcode object
var barcode = BarcodeWriter.CreateBarcode("12345678901234", BarcodeEncoding.Code128);
barcode.SaveAsPng("code128.png");
barcode.SaveAsJpeg("code128.jpg");
byte[] bytes = barcode.ToPngBinaryData();

using var stream = new MemoryStream();
barcode.SaveAsPng(stream);
```

Detailed options for [1D barcode generation](https://ironsoftware.com/csharp/barcode/how-to/create-1d-barcodes/) including width, height, and label configuration are covered in the IronBarcode documentation.

## Reading Capability

### NetBarcode Approach

NetBarcode has no reading API. There is no method, class, or configuration that decodes a barcode image back to its data string. This is a deliberate scope boundary, not an omission pending a release. A project that generates barcodes with NetBarcode and later needs to read them — to validate a printed label, scan a return shipment, or extract values from a supplier invoice — must introduce a separate library for that purpose.

```csharp
// NetBarcode — no reading method exists
// The following does not compile because the method does not exist:
// var result = barcode.Read("image.png");  // method does not exist

// The typical workaround requires ZXing.Net as a third-party dependency
```

The ZXing.Net library is the most common addition for reading alongside NetBarcode, bringing a third API surface and a third package to version-manage alongside NetBarcode and any 2D library already added for QR codes.

### IronBarcode Approach

IronBarcode includes `BarcodeReader` in the same package as `BarcodeWriter`. The reading API accepts image files and PDF documents and returns a collection of decoded results, each with the barcode value, format type, and page number if reading from a PDF.

```csharp
using IronBarCode;

// Read barcodes from an image file
var imageResults = BarcodeReader.Read("shipping-label.png");
foreach (var r in imageResults)
{
    Console.WriteLine($"{r.BarcodeType}: {r.Value}");
}

// Read barcodes from a PDF document — no additional library required
var pdfResults = BarcodeReader.Read("invoice.pdf");
foreach (var r in pdfResults)
{
    Console.WriteLine($"Page {r.PageNumber}: {r.Value}");
}
```

The [read barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) guide covers speed tuning, multi-barcode detection, and image correction options available in the reading API.

## Dependency and Licensing Considerations

### NetBarcode and ImageSharp

NetBarcode is MIT-licensed. The SixLabors.ImageSharp library it depends on uses a different model: free for open-source projects and for companies with annual gross revenue below a defined threshold, but a commercial license is required above that threshold. This split applies regardless of whether ImageSharp is listed explicitly in a project's `.csproj` or arrives transitively through NetBarcode.

```xml
<!-- This single reference pulls in SixLabors.ImageSharp and SixLabors.Fonts
     under their respective split licences. NuGet does not surface the
     commercial licence condition during package installation. -->
<PackageReference Include="NetBarcode" Version="1.8.2" />
```

For a retail or logistics company processing barcodes at scale — the primary use case NetBarcode targets — annual revenue is often above the threshold at which the ImageSharp commercial licence applies. A compliance audit may reveal this obligation embedded in the package tree.

The v1.8 release introduced an additional consequence of the ImageSharp dependency: the return type of `GetImage()` changed from an internal representation to `SixLabors.ImageSharp.Image<Rgba32>`. Existing code that called `GetImage()` without explicit typing broke at compile time, and new `using` directives for `SixLabors.ImageSharp` and `SixLabors.ImageSharp.PixelFormats` became required. When ImageSharp's own API evolves in future versions, NetBarcode's public API surface is affected in turn.

### IronBarcode

IronBarcode has no SixLabors.ImageSharp dependency. Its licensing terms are stated directly on the [IronBarcode licensing page](https://ironsoftware.com/csharp/barcode/licensing/) with no split threshold or transitive commercial obligation. A trial key is available for evaluation; purchased licenses remove the trial watermark from generated output.

## API Mapping Reference

| NetBarcode | IronBarcode | Notes |
|---|---|---|
| `new Barcode(data, Type.Code128)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)` | Constructor → static method |
| `new Barcode(data, Type.EAN13)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.EAN13)` | Direct mapping |
| `new Barcode(data, Type.UPCA)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.UPCA)` | Direct mapping |
| `new Barcode(data, Type.Code39)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code39)` | Direct mapping |
| `new Barcode(data, Type.EAN8)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.EAN8)` | Direct mapping |
| `new Barcode(data, Type.UPCE)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.UPCE)` | Direct mapping |
| `new Barcode(data, Type.ITF)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.ITF)` | Direct mapping |
| `new Barcode(data, Type.Codabar)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Codabar)` | Direct mapping |
| `barcode.SaveImageFile("x.png")` | `.SaveAsPng("x.png")` | Method rename |
| `barcode.SaveImageFile("x.jpg")` | `.SaveAsJpeg("x.jpg")` | Method rename |
| `barcode.GetImage()` → `Image<Rgba32>` | `.ToPngBinaryData()` or `.SaveAsPng()` | No ImageSharp type exposed |
| No `Type.QRCode` | `BarcodeEncoding.QRCode` | New capability |
| No `Type.DataMatrix` | `BarcodeEncoding.DataMatrix` | New capability |
| No `Type.PDF417` | `BarcodeEncoding.PDF417` | New capability |
| No `Type.Aztec` | `BarcodeEncoding.Aztec` | New capability |
| No reading API | `BarcodeReader.Read(path)` | New capability |
| `using NetBarcode;` | `using IronBarCode;` | Namespace replacement |
| `using SixLabors.ImageSharp;` | Remove | No longer needed |

The complete format reference is available in the [supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) documentation.

## When Teams Consider Moving from NetBarcode to IronBarcode

### QR Code and 2D Format Requirements

The most frequent trigger for evaluating an alternative to NetBarcode is a new requirement for QR codes. Applications that begin with 1D barcode generation for retail labels or shipping manifests commonly receive a follow-on requirement for QR codes — contactless links, mobile app deep links, marketing campaigns. Because the `Type` enum has no QR entry, this requirement cannot be met within NetBarcode. Teams that add a separate QR library to address the gap then face a second evaluation when DataMatrix is required for a pharmaceutical integration, or PDF417 for a logistics carrier that mandates it on shipping labels.

### Barcode Reading Becomes Necessary

Some projects begin with pure generation and later add a validation or document-processing requirement: confirm that a printed barcode matches its source data, extract barcode values from incoming supplier invoices, or scan return shipment labels. NetBarcode provides no path for this. The addition of ZXing.Net or a comparable reading library introduces a third API to learn and maintain in the same codebase that already holds NetBarcode and a QR library. Projects that anticipate reading requirements, even in a future phase, often find it more efficient to select a library that handles both concerns from the start.

### ImageSharp Commercial Licence Audit

Legal and compliance reviews of third-party dependencies occasionally surface the SixLabors.ImageSharp commercial licence condition embedded in the NetBarcode package tree. For companies whose annual gross revenue exceeds the threshold, the obligation applies whether ImageSharp was selected deliberately or arrived transitively through NetBarcode. Teams that discover this during an audit — rather than before adoption — face a retroactive remediation rather than a planned migration. Evaluating the dependency licence before starting a project is the cleaner path.

### Reducing Multi-Library Complexity

Teams that have accumulated NetBarcode for 1D generation, a QR-specific library for 2D output, and ZXing.Net for reading find themselves maintaining version compatibility across three separate packages. Each upgrade cycle requires checking whether the three libraries agree on their shared ImageSharp version. Each new developer on the project encounters three different APIs for what is conceptually one concern. Consolidation to a single barcode library simplifies onboarding, reduces version conflict surface, and concentrates maintenance to one release cycle.

## Common Migration Considerations

### Package Swap and Transitive Dependency Cleanup

Removing NetBarcode with `dotnet remove package NetBarcode` is the first step. The SixLabors.ImageSharp package may reappear in the dependency tree if other packages in the project also pull it in transitively. After removal, inspect the restored package list with `dotnet list package --include-transitive` to confirm whether ImageSharp is still present and whether its commercial licence condition still applies.

### GetImage() Return Type Replacement

Any code that stored the result of `GetImage()` as `Image<Rgba32>` must be updated. The ImageSharp type has no direct equivalent in IronBarcode; the replacement depends on how the image was used downstream. Code that saved the image to a stream can be replaced with `.SaveAsPng(stream)` directly on the `GeneratedBarcode` object. Code that retrieved raw bytes can use `.ToPngBinaryData()`. Code that performed further ImageSharp manipulations on the returned image will need those operations evaluated individually.

### Namespace Update

Files that imported `using NetBarcode;`, `using SixLabors.ImageSharp;`, `using SixLabors.ImageSharp.PixelFormats;`, or `using SixLabors.Fonts;` need those directives replaced with `using IronBarCode;`. A project-wide search for these using statements identifies every file that requires attention before the build is attempted.

## Additional IronBarcode Capabilities

Beyond the core generation and reading features covered in this comparison, IronBarcode provides:

- **[SVG Barcode Output](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/):** Generate vector-format barcode images suitable for print workflows and scalable label designs
- **[Barcode Styling](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/):** Configure bar color, background color, annotation font, margin, and rotation on generated barcodes
- **[GS1-128 and GS1 DataBar](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/):** Application identifier-structured barcodes for retail and supply chain compliance
- **[Postal Formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/):** Intelligent Mail, Royal Mail, and other postal symbologies for mailing applications
- **[PDF Barcode Extraction](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/):** Read barcodes directly from multi-page PDF documents without a separate PDF library
- **[Batch Generation](https://ironsoftware.com/csharp/barcode/how-to/create-1d-barcodes/):** Process collections of barcode data efficiently within a single pipeline
- **[MAUI and Mobile Targets](https://ironsoftware.com/csharp/barcode/):** IronBarcode supports .NET MAUI applications for cross-platform mobile and desktop barcode workflows

## .NET Compatibility and Future Readiness

IronBarcode targets .NET 8, .NET 9, and maintains compatibility with .NET Standard for projects that have not yet migrated to modern .NET. As .NET 10 is expected in late 2026, Iron Software's regular release cadence ensures that compatibility updates follow each major .NET release. NetBarcode targets .NET Standard 2.0 and is functional on current runtimes through that compatibility layer, though the library's update frequency and 2D format set are fixed by its design scope.

## Conclusion

NetBarcode and IronBarcode represent different positions on the spectrum of barcode library scope. NetBarcode is a focused, clean implementation of 1D barcode generation: fourteen formats, a straightforward constructor API, and an MIT license that keeps adoption frictionless for open-source projects within the ImageSharp revenue threshold. IronBarcode is a broader toolkit covering generation across 50+ formats, reading from images and PDFs, and a fluent API that treats 1D and 2D formats identically.

For projects where the requirements are genuinely limited to linear barcode generation — a point-of-sale system producing EAN-13 and UPC-A codes for traditional retail scanners, or an internal tool with a fixed and short lifespan — NetBarcode delivers what is needed without introducing a commercial dependency. The library is well-built within its scope, and that scope is explicit from the first look at the `Type` enum.

For projects where format scope may expand, where reading will eventually be needed, or where a compliance review of the ImageSharp transitive dependency is a concern, IronBarcode addresses all three through a single package. Teams that begin with NetBarcode for 1D generation and later add QRCoder for 2D and ZXing.Net for reading accumulate three separate library maintenance obligations; IronBarcode consolidates those into one.

The choice follows directly from the project's requirements. If fourteen 1D formats and no reading capability match the specification precisely, NetBarcode is a technically sound selection. If the specification includes any 2D format, any reading workflow, or any concern about the ImageSharp licence condition, IronBarcode is the more complete answer.
