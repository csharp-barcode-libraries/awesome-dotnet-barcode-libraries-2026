OnBarcode's pricing page says "Contact Us." The generator and the reader are separate products with separate quotes. You cannot calculate your total cost until you have spoken to sales twice and compared two proposals. That is the situation before you write a single line of code.

## Understanding OnBarcode

OnBarcode is a commercial barcode library with roots in the DLL-distribution era of .NET development. The company offers barcode tools across .NET, Java, Crystal Reports, SSRS, and other platforms. Within the .NET ecosystem, the primary products are a Generator SDK and a Reader SDK, which are sold, licensed, and distributed as entirely separate packages.

For much of its history, OnBarcode was delivered as a downloadable DLL. A developer would contact sales, receive a download link, extract the archive, drop the assembly into a `lib/` folder, and wire it up manually in the `.csproj`. This model meant binary files in source control, no automatic restore on fresh clone, manual upgrade procedures, and CI pipelines that either carried those DLLs or required a custom download step. NuGet packages were added in 2025–2026, which is an improvement, but the documentation still partially reflects the older distribution model.

The contact-sales pricing model applies to both products independently. Neither the Generator SDK nor the Reader SDK displays a price on the product page. An organization that needs both capabilities must negotiate two separate agreements, receive two invoices, manage two license keys, and track two separate version lifecycles.

Key architectural characteristics of OnBarcode:

- **Split product model:** Generation and reading are separate SKUs with separate NuGet packages, separate license keys, and separate API namespaces
- **Contact-sales pricing:** Neither the Generator SDK nor the Reader SDK displays a published price; both require a sales conversation to obtain a quote
- **Instance-based API design:** The generator uses a `Barcode` class with property assignments before calling a generation method
- **Explicit format specification on reading:** The reader requires a `BarcodeType[]` array to be configured before scanning; there is no automatic format detection
- **No native PDF support:** Neither product reads barcodes directly from PDF documents; a separate PDF-to-image rendering library is required for PDF workflows
- **Source code access:** The Unlimited Developer tier includes access to the library source code, which is not available at lower tiers
- **DLL-era distribution history:** The older manual reference workflow is still partially documented alongside the newer NuGet packages

### The Split Product Architecture

The most immediately visible consequence of the two-product model is the license configuration required in a project that uses both generation and reading. Developers must configure two separate license keys through two separate namespaces:

```csharp
using OnBarcode.Barcode;
using OnBarcode.Barcode.Reader;

// Two licenses, two calls, two keys to manage and rotate
OnBarcode.Barcode.License.SetLicense("GENERATOR-LICENSE-KEY");
OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");
```

Projects that started as generation-only and later added reading capability must acquire the Reader SDK as a separate purchase, add a second NuGet package, and extend the license configuration. The two packages version independently, which introduces the possibility of API drift between the generator and reader over time.

## Understanding IronBarcode

IronBarcode is a commercial .NET barcode library from Iron Software that provides both barcode generation and barcode reading through a single package under a single license key. The library follows a static factory method design, where generation and reading operations are invoked through class-level methods rather than configured object instances.

IronBarcode has been distributed through NuGet since its initial release. The library supports more than fifty barcode symbologies across both one-dimensional and two-dimensional formats. Pricing is published on the product website and does not require a sales conversation to obtain.

Key characteristics of IronBarcode:

- **Single package:** Generation, reading, PDF support, and all other capabilities are in one NuGet package under one license key
- **Published pricing:** Three perpetual license tiers with prices displayed on the product website without requiring contact with sales
- **Static factory API:** `BarcodeWriter.CreateBarcode()` and `BarcodeReader.Read()` are stateless class-level methods; no object instantiation or property configuration required for common operations
- **Automatic format detection:** The reader identifies barcode formats without being told what to look for
- **Native PDF support:** Reads barcodes directly from PDF files without an external rendering library
- **Fluent chaining:** Generation calls can be chained — create, style, and save in a single expression
- **Structured reading results:** Returns `BarcodeResults` with decoded value, detected format, bounding box coordinates, page number for PDF reads, and confidence information

## Feature Comparison

The following table provides a high-level overview of the two libraries across the dimensions most relevant to project planning:

| Feature | OnBarcode | IronBarcode |
|---|---|---|
| Barcode generation | Yes | Yes |
| Barcode reading | Separate product, separate purchase | Included in the same package |
| PDF barcode reading | Not supported natively | Native, no external library required |
| Published pricing | No — contact sales | Yes — perpetual tiers listed on website |
| Single license for generation and reading | No | Yes |
| NuGet distribution | Added 2025–2026 | Available since launch |
| Format auto-detection on reading | No | Yes |
| Source code access | Unlimited tier only | Not available |

### Detailed Feature Comparison

| Feature | OnBarcode | IronBarcode |
|---|---|---|
| **Generation** | | |
| Code 128 generation | Yes | Yes |
| QR Code generation | Yes | Yes |
| EAN/UPC generation | Yes | Yes |
| Data Matrix generation | Yes | Yes |
| PDF417 generation | Yes | Yes |
| QR Code with logo overlay | Manual GDI+ required | Built-in `QRCodeWriter.CreateQrCodeWithLogo()` |
| Output formats | Image file via `drawBarcode()` | PNG, JPEG, PDF, GIF, TIFF via named save methods |
| API style | Instance + property assignment | Static factory + fluent chain |
| **Reading** | | |
| Image reading | Separate Reader SDK required | Included |
| PDF reading | Not supported | Native |
| Format auto-detection | No — explicit `BarcodeType[]` required | Yes |
| Result metadata | Raw `string[]` only | Value, format, bounding box, page number, confidence |
| Batch file reading | Separate Reader SDK, one file at a time | `BarcodeReader.Read(string[])` — multiple files |
| **Licensing** | | |
| Pricing transparency | Contact sales — no published price | Published perpetual tiers |
| Products required for read + write | Two separate purchases | One purchase |
| License keys required | Two — one per product | One |
| Source code access | Unlimited tier | Not available |
| **Platform and Distribution** | | |
| NuGet package | Available since 2025–2026 | Available since launch |
| .NET Framework support | Yes | Yes |
| .NET 6 / 7 / 8 / 9 support | Yes | Yes |
| Docker / cloud deployment | Manual configuration | Single key, environment variable support |

## Generation API

Barcode generation is available in both libraries but differs significantly in the amount of code required for common operations.

### OnBarcode Approach

OnBarcode's generator uses an instance-based, property-assignment pattern. A `Barcode` object is created, properties are set one by one, and generation is triggered by calling `drawBarcode()` with an output file path:

```csharp
using OnBarcode.Barcode;

Barcode barcode = new Barcode();
barcode.Symbology = Symbology.Code128Auto;
barcode.Data = "12345678";
barcode.Resolution = 96;
barcode.BarWidth = 1;
barcode.BarHeight = 80;
barcode.ShowText = true;
barcode.drawBarcode("barcode.png");
```

Eight lines are required for a basic Code 128 barcode. Three of those lines set properties that have reasonable default values. The method name `drawBarcode()` uses a lowercase `d`, which diverges from standard .NET naming conventions and is the first thing developers notice when browsing IntelliSense. The output format is determined by the file extension passed to `drawBarcode()` rather than by an explicit format method.

### IronBarcode Approach

IronBarcode uses a static factory method that accepts data and encoding as parameters, then chains directly to a format-named save method:

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128).SaveAsPng("barcode.png");
```

The save method name specifies the output format explicitly: `SaveAsPng`, `SaveAsJpeg`, `SaveAsPdf`, `SaveAsGif`, `SaveAsTiff`. There is no ambiguity about which format a generic save method will produce. Additional styling or sizing options can be chained between `CreateBarcode` and the save call without introducing mutable object state.

For complete [C# barcode generation](https://ironsoftware.com/csharp/barcode/) guidance including supported symbologies and output options, the IronBarcode documentation covers all common patterns.

## Barcode Reading

Reading is where the two libraries diverge most sharply, because for OnBarcode it is a separate commercial product rather than an included capability.

### OnBarcode Approach

Reading barcodes with OnBarcode requires the separate `OnBarcode.Barcode.Reader` NuGet package, a separate license key, and a distinct API configuration. The developer must specify which barcode formats to scan for before the operation runs:

```csharp
using OnBarcode.Barcode.Reader;

OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");

BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128 };
string[] results = reader.Scan("barcode.png");

foreach (string value in results)
    Console.WriteLine(value);
```

The return type is `string[]` — a flat array of raw decoded strings. There is no metadata about which format was detected, where in the image the barcode was located, or what confidence level the recognition achieved. If the barcode format at scan time does not match the `BarcodeType[]` configuration, the barcode will not be found.

### IronBarcode Approach

IronBarcode reading is part of the same package as generation. No separate purchase, no separate license key, and no format specification required:

```csharp
using IronBarCode;

var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
    Console.WriteLine($"{result.Value} ({result.Format})");
```

`BarcodeReader.Read` is a static method that accepts a file path and returns `BarcodeResults`. Each result in the collection carries the decoded value, the detected barcode format, bounding box coordinates, and — for PDF inputs — the page number. For applications that [read barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) arriving from external sources where the format is not known in advance, automatic detection removes an entire category of configuration errors.

## PDF Support

PDF barcode reading is a common requirement in document processing workflows: invoice parsing, shipping label verification, archival document scanning, and supply chain integrations where PDFs are the delivery format.

### OnBarcode Approach

OnBarcode does not support reading barcodes directly from PDF files. A project that needs to extract barcodes from a PDF must acquire a separate PDF-to-image rendering library, render each page of the PDF to an image, pass those images to the OnBarcode Reader SDK one at a time, and aggregate the results. This introduces an additional dependency with its own licensing, maintenance, and integration overhead.

There is no native PDF generation output for barcodes either — output is to image formats via `drawBarcode()`.

### IronBarcode Approach

IronBarcode reads barcodes directly from PDF documents without any additional library. The same `BarcodeReader.Read` method accepts a PDF file path and processes all pages automatically:

```csharp
// Read barcodes directly from a PDF — no PDF rendering library needed
var pdfResults = BarcodeReader.Read("document.pdf");
foreach (var result in pdfResults)
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
```

Every page is scanned. All barcode formats are detected automatically. The page number of each result is accessible through `result.PageNumber`. The [PDF barcode reading](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) capability is part of the same package and the same license key used for generation and image reading.

## Pricing and Licensing

Pricing structure has a direct effect on procurement timelines, budget planning, and how quickly a team can make a purchasing decision.

### OnBarcode Approach

OnBarcode does not publish prices for either the Generator SDK or the Reader SDK. Both product pages display a "Contact Us" call to action. An organization that needs both capabilities must initiate two separate sales conversations, receive two separate quotes, and obtain approval for two separate purchase orders. The total cost of the combined capability is unknown until both negotiations are complete.

License keys are also product-specific. A project using both products carries two keys, which must be managed, rotated, and stored separately. The source code is included only in the Unlimited Developer tier; lower tiers receive compiled binaries only.

### IronBarcode Approach

IronBarcode publishes its [pricing](https://ironsoftware.com/csharp/barcode/licensing/) directly on the product website:

| License | Price | Developers | Covers |
|---|---|---|---|
| Lite | $749 | 1 | Generation + Reading + PDF |
| Professional | $1,499 | 10 | Generation + Reading + PDF |
| Unlimited | $2,999 | Unlimited | Generation + Reading + PDF |

All licenses are perpetual. Support and update subscriptions are optional. A five-developer team can check the Professional tier price, place it in a budget spreadsheet, and raise a purchase order without a sales conversation.

## API Mapping Reference

| OnBarcode | IronBarcode | Notes |
|---|---|---|
| `OnBarcode.Barcode.License.SetLicense("key")` | `IronBarCode.License.LicenseKey = "key"` | Single property assignment replaces method call |
| `OnBarcode.Barcode.Reader.License.SetLicense("key")` | (merged into single key above) | No separate reader license |
| `new Barcode()` | `BarcodeWriter.CreateBarcode(data, encoding)` | Static factory; no instance required |
| `barcode.Symbology = Symbology.Code128Auto` | `BarcodeEncoding.Code128` as parameter | Passed as argument rather than property |
| `barcode.Data = "..."` | First parameter of `CreateBarcode` | Data and encoding are constructor parameters |
| `barcode.drawBarcode("file.png")` | `.SaveAsPng("file.png")` | Format-named save method |
| `barcode.drawBarcode("file.jpg")` | `.SaveAsJpeg("file.jpg")` | Explicit format in method name |
| `barcode.drawBarcode("file.pdf")` | `.SaveAsPdf("file.pdf")` | Native PDF output |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` | Same concept, different enum name |
| `Symbology.EAN13` | `BarcodeEncoding.EAN13` | Direct mapping |
| `Symbology.DataMatrix` | `BarcodeEncoding.DataMatrix` | Direct mapping |
| `Symbology.PDF417` | `BarcodeEncoding.PDF417` | Direct mapping |
| `new BarcodeReader() { BarcodeTypes = ... }` | `BarcodeReader.Read(path)` — static, no instance | No format specification required |
| `reader.Scan("file.png")` → `string[]` | `BarcodeReader.Read("file.png")` → `BarcodeResults` | Richer result type with metadata |
| `results[0]` (raw string) | `result.Value` | Value property on result object |
| N/A | `result.Format` | Detected format — not available in OnBarcode |
| N/A | `result.PageNumber` | Page number for PDF reads |
| N/A | `BarcodeReader.Read("document.pdf")` | Native PDF reading — no equivalent in OnBarcode |
| `QRCodeWriter` + manual GDI+ overlay | `QRCodeWriter.CreateQrCodeWithLogo()` | Built-in logo overlay |

## When Teams Consider Moving from OnBarcode to IronBarcode

### Adding Barcode Reading to an Existing Generator Project

The most common trigger for evaluating an alternative to OnBarcode is the decision to add barcode reading to a project that was originally generation-only. A team that built a label-printing workflow using the OnBarcode Generator SDK and then receives a requirement to also verify those labels faces a straightforward accounting problem: the reading capability is not included, so adding it requires a second sales conversation, a second purchase, a second NuGet package, and a second license key to configure and maintain. Teams doing this calculation frequently find it more efficient to consolidate onto a single library rather than extend the two-product arrangement.

### Budget Planning and Procurement

Some organizations cannot initiate a purchase order without a confirmed figure. When a team needs both barcode generation and reading, OnBarcode requires two separate quotes from two separate sales processes before a number exists to put in a procurement request. This is not a technical constraint, but it is a practical one with real timelines attached. Published pricing allows a team to complete the internal approval process without waiting for a sales response.

### PDF Document Workflows

Document processing systems frequently receive barcodes embedded in PDFs — invoices, shipping manifests, purchase orders, and archival scans. When a team using OnBarcode encounters this requirement, the gap in native PDF support requires adding a separate PDF rendering library to the dependency chain. That library carries its own licensing terms, maintenance schedule, and integration complexity. Teams whose workflows are primarily PDF-based frequently conclude that the integration overhead is not justified when alternatives include native PDF support in the same package.

### Reducing Product Complexity

Some development teams or organizations have internal policies about the number of third-party dependencies a project can carry, or they prioritize reducing the surface area of dependencies for security or audit reasons. A project that uses OnBarcode for generation and OnBarcode's separate Reader SDK for reading is carrying two packages, two license keys, two version lifecycles, and two API namespaces. When simplification is a goal — whether driven by policy, audit requirements, or team preference — consolidating onto a single package for barcode operations is a straightforward path to that goal.

## Common Migration Considerations

### DLL Reference Cleanup

Projects using the older DLL-based OnBarcode distribution carry manual `<Reference>` entries in their `.csproj` files. Before adding IronBarcode, those entries must be removed and the DLL files deleted from the `lib/` or equivalent directory. Leaving the old DLL in place while adding the new NuGet package will produce namespace conflicts at compile time.

### Dual License Replacement

OnBarcode projects that used both products require two `SetLicense` calls at startup. After migrating to IronBarcode, both of those calls are replaced by a single property assignment: `IronBarCode.License.LicenseKey = "YOUR-KEY"`. The old license keys are no longer valid and should be removed from configuration files, environment variables, and secrets stores.

### Namespace Changes

The `OnBarcode.Barcode` and `OnBarcode.Barcode.Reader` namespaces are replaced by `IronBarCode`. The class names also change: the `Barcode` instance class becomes the static `BarcodeWriter`, and the `BarcodeReader` class name is shared — but the OnBarcode version is an instance class configured with `BarcodeTypes`, while the IronBarcode version is a static class with no instance. Any type-disambiguation comments in existing code should be updated to avoid confusion.

## Additional IronBarcode Capabilities

The following IronBarcode features were not discussed in the comparison above but may be relevant depending on the project:

- **[Batch image reading](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/):** `BarcodeReader.Read` accepts an array of file paths and processes all of them in a single call, returning a unified result set
- **[QR Code with logo](https://ironsoftware.com/csharp/barcode/):** `QRCodeWriter.CreateQrCodeWithLogo()` handles the error-correction math and image overlay without any `System.Drawing` code
- **Multi-page PDF processing:** All pages of a PDF are scanned automatically; `result.PageNumber` identifies which page each barcode came from
- **Environment variable license configuration:** For Docker and cloud deployments, the license key can be supplied through an environment variable rather than a code assignment
- **Bounding box coordinates:** Each reading result includes the pixel coordinates of the barcode within the source image, useful for annotation or cropping workflows
- **Confidence scoring:** Each result carries a confidence value, allowing applications to filter or flag low-confidence reads for manual review

## .NET Compatibility and Future Readiness

IronBarcode supports .NET Framework 4.6.2 and later, .NET Core 3.1, and the full .NET 5 through .NET 9 release series. The library receives regular updates aligned with .NET release cycles, ensuring compatibility with .NET 10 when it ships in late 2026. Deployment targets include Windows, Linux, macOS, Docker containers, Azure, AWS, and Google Cloud. Because IronBarcode ships as a single NuGet package covering all capabilities, dependency management across .NET version upgrades involves one package reference rather than two.

## Conclusion

OnBarcode and IronBarcode represent different approaches to the same problem space. OnBarcode treats barcode generation and barcode reading as separate commercial products with separate purchase workflows, separate license keys, and separate API surfaces. IronBarcode treats generation and reading as two functions of a single library, licensed together under a single key with a published price.

OnBarcode is a reasonable choice for teams with a narrowly scoped generation-only requirement. The Generator SDK performs its stated function, the property-based API is familiar to developers who have used similar object-oriented barcode libraries, and the Unlimited Developer tier offers source code access that IronBarcode does not provide at any tier. For an organization that already holds an OnBarcode license and whose workflow remains generation-only, there is no practical pressure to change.

IronBarcode is better suited for teams that need both generation and reading, or that anticipate adding reading in the future. The single-package model eliminates the procurement overhead of managing two separate products, and native PDF support removes the need for an additional rendering library in document-processing workflows. Published pricing allows budget planning to begin without a sales conversation. The static factory API and automatic format detection reduce the configuration required for common read and write operations.

The decision ultimately turns on scope. If the requirement is purely barcode generation with no reading, no PDF processing, and a stable long-term use case, OnBarcode covers that scope. For requirements that include reading, PDF support, transparent procurement, or the expectation of growing capability over time, IronBarcode's unified model offers a more direct path. The [IronBarcode vs OnBarcode alternatives](https://ironsoftware.com/csharp/barcode/blog/compare-to-other-components/onbarcode-generator-alternatives/) overview provides additional context for teams conducting a formal evaluation.
