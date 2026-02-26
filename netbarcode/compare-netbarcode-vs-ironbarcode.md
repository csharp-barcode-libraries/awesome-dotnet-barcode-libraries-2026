NetBarcode's `Type` enum doesn't include QR Code. When the shipping label project adds a QR code requirement on month three, developers add a second library. That library has its own API, its own bugs, and its own ImageSharp version that may conflict with the first.

That's the real story of using NetBarcode in a project with growing requirements. The library itself is well-built for what it does — clean API, good 1D format coverage, MIT license. The problem isn't NetBarcode. The problem is that modern applications outgrow 1D-only generators faster than developers expect, and the exit cost is measured in library integrations rather than license fees.

This comparison examines where NetBarcode fits well, where it stops, and how [IronBarcode](https://ironsoftware.com/csharp/barcode/) covers the gaps without requiring a second or third package.

---

## The `Type` Enum Gap

Open NetBarcode's source and look at the `Type` enum. You'll find Code128, Code128A, Code128B, Code128C, Code39, Code39Extended, Code93, EAN8, EAN13, UPCA, UPCE, Codabar, ITF, and MSI. Fourteen entries — all 1D.

There is no `Type.QRCode`. There is no `Type.DataMatrix`. There is no `Type.PDF417`.

This isn't a bug or a missing feature waiting on a pull request. It's the library's design boundary. NetBarcode was built to generate linear barcodes, and it does that well. The consequence is that any application requiring 2D formats must bring in a second package.

The typical workaround looks like this:

```csharp
// NetBarcode for 1D
using NetBarcode;
var code128 = new Barcode("12345", Type.Code128);
code128.SaveImageFile("label.png");

// QRCoder added separately for QR — two APIs, two packages
using QRCoder;
var qrGenerator = new QRCodeGenerator();
var qrCode = qrGenerator.CreateQrCode("data", QRCodeGenerator.ECCLevel.M);
var png = new PngByteQRCode(qrCode).GetGraphic(20);
File.WriteAllBytes("qr.png", png);
```

Now the codebase has two barcode dependencies with two APIs. When QRCoder releases a new version, you update and retest. When NetBarcode updates SixLabors.ImageSharp, you check whether it conflicts with whatever version QRCoder expects. If you later need to read barcodes back from images — scanning a supplier invoice, validating a generated label — that's a third library. ZXing.Net is the usual answer, and it brings a third API surface.

IronBarcode handles all three scenarios with a single import:

```csharp
using IronBarCode;

// 1D — same as NetBarcode's strongest use case
BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128)
    .SaveAsPng("label.png");

// QR — same API, just change the encoding
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");

// Reading — no second library required
var result = BarcodeReader.Read("label.png").First();
Console.WriteLine($"{result.Value} ({result.Format})");
```

For teams needing [2D barcode generation](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/) alongside their existing 1D workflow, the API is identical — swap the encoding constant and the method works.

---

## Generation Comparison: Side by Side

For the 1D formats both libraries share, the code is similar in shape. Here is Code128 in both:

**NetBarcode:**

```csharp
using NetBarcode;
using SixLabors.ImageSharp;

var barcode = new Barcode("12345678901234", Type.Code128);
barcode.SaveImageFile("code128.png");

// GetImage() returns Image<Rgba32> — requires SixLabors.ImageSharp import
Image<Rgba32> image = barcode.GetImage();
```

**IronBarcode:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678901234", BarcodeEncoding.Code128)
    .SaveAsPng("code128.png");
```

The functional difference is minor for this use case. The structural difference is that NetBarcode's `GetImage()` method returns a `SixLabors.ImageSharp.Image<Rgba32>` object — which means any code that processes the returned image must import and understand the ImageSharp API. IronBarcode's fluent chain keeps image handling internal.

For [1D barcode generation](https://ironsoftware.com/csharp/barcode/how-to/create-1d-barcodes/) specifically, NetBarcode is a reasonable tool. The divergence shows up when the format list needs to expand.

---

## Generation Comparison: The Formats NetBarcode Cannot Generate

```csharp
// NetBarcode — these fail at compile time because the Type enum has no entry
// var qr = new Barcode("data", Type.QRCode);          // CS0117 error
// var dm = new Barcode("data", Type.DataMatrix);       // CS0117 error
// var p417 = new Barcode("data", Type.PDF417);         // CS0117 error

// IronBarcode — same syntax as Code128
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode)
    .SaveAsPng("qr.png");

BarcodeWriter.CreateBarcode("01034531200000111719112510ABCD1234", BarcodeEncoding.DataMatrix)
    .SaveAsPng("datamatrix.png");

BarcodeWriter.CreateBarcode("M1DOE/JOHN MR ABC123 JFKLHR 0012 123Y015A0001 100", BarcodeEncoding.Aztec)
    .SaveAsPng("boarding-pass.png");
```

The compile errors in the NetBarcode block are the entire story. There is no workaround within the library — you either add QRCoder or you choose a different library from the start.

Industries where this matters most: pharmaceutical tracking requires DataMatrix under FDA 2D barcode mandates; airline boarding passes use Aztec; FedEx and IATA shipping manifests use PDF417; mobile payment integrations and marketing links require QR Code. Any of these requirements eliminates NetBarcode as a standalone solution. The full [supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) in IronBarcode cover all of these without additional packages.

---

## No Reading API

NetBarcode is generation-only. There is no method to decode a barcode image back to its data. This is a deliberate scope choice, not an oversight.

For projects that only generate barcodes and never read them, this is fine. For projects that need to scan incoming documents, validate printed labels, or process supplier barcodes, a second library becomes necessary before the project is complete.

```csharp
// NetBarcode — no reading capability exists
// var result = barcode.Read("image.png");  // method does not exist

// IronBarcode — reading is built in
var results = BarcodeReader.Read("invoice.pdf");
foreach (var r in results)
{
    Console.WriteLine($"{r.BarcodeType}: {r.Value}");
}
```

IronBarcode's reader works across image files and PDF documents with automatic format detection. A full walkthrough of the reading options is available in the [read barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) guide.

---

## The ImageSharp Commercial License

NetBarcode depends on SixLabors.ImageSharp for image rendering. The ImageSharp project uses a split license: free for open-source projects and companies under $1M annual gross revenue, but a commercial license is required above that threshold.

This means NetBarcode's "free MIT" status has a dependency with a commercial restriction embedded in the package tree. A company processing retail barcodes at scale — which is precisely the use case NetBarcode targets — may be operating above the ImageSharp revenue threshold without realizing it.

The dependency chain in the `.csproj` file looks harmless:

```xml
<PackageReference Include="NetBarcode" Version="1.8.2" />
```

But this pulls in `SixLabors.ImageSharp` and `SixLabors.Fonts`, both under the split license. The ImageSharp license requirement is not surfaced by NuGet during installation.

IronBarcode has no ImageSharp dependency. Its licensing is transparent and up front — details are on the [IronBarcode licensing page](https://ironsoftware.com/csharp/barcode/licensing/).

---

## The v1.8 Breaking Change

NetBarcode 1.8 changed the return type of `GetImage()` from an internal bitmap representation to `SixLabors.ImageSharp.Image<Rgba32>`. Existing code that called `GetImage()` and passed the result to other methods broke at compile time. Projects also needed to add explicit `using SixLabors.ImageSharp;` and `using SixLabors.ImageSharp.PixelFormats;` imports that were not previously required.

**Before NetBarcode 1.8:**

```csharp
using NetBarcode;

var barcode = new Barcode("12345", Type.Code128);
var image = barcode.GetImage();  // returned internal type, worked fine
```

**After NetBarcode 1.8:**

```csharp
using NetBarcode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Fonts;

var barcode = new Barcode("12345", Type.Code128);
Image<Rgba32> image = barcode.GetImage();  // now returns ImageSharp type
```

Any code that stored, passed, or processed the result of `GetImage()` without explicit typing worked before and failed after. This is not a criticism of the NetBarcode maintainer — it's the expected cost of a library that exposes a third-party type in its public API. When ImageSharp's API evolves, so does NetBarcode's interface.

---

## Feature Comparison Table

| Feature | NetBarcode | IronBarcode |
|---|---|---|
| 1D barcode generation | Yes | Yes |
| 2D barcode generation (QR, DataMatrix, PDF417, Aztec) | No | Yes |
| Barcode reading / decoding | No | Yes |
| PDF reading and generation | No | Yes |
| 1D format count | ~14 | 30+ |
| 2D format count | 0 | 8+ |
| Total symbologies | ~14 | 50+ |
| GS1-128, GS1 DataBar | No | Yes |
| Postal formats (Intelligent Mail, Royal Mail) | No | Yes |
| SVG output | No | Yes |
| Batch processing | Manual | Built-in |
| ImageSharp dependency | Yes (split license) | No |
| Reading API | None | `BarcodeReader.Read()` |
| PDF barcode extraction | None | Built-in |
| Commercial support | Community only | Professional |
| License cost | $0 (+ ImageSharp for >$1M) | $749 one-time |

---

## Total Cost of Ownership

The "free" label on NetBarcode is accurate for projects that stay within 1D generation and stay under the ImageSharp revenue threshold. It becomes misleading when requirements expand.

A realistic e-commerce or logistics project timeline:

| Timeline | Requirement | NetBarcode Path | IronBarcode Path |
|---|---|---|---|
| Launch | Product UPC codes | Works — $0 | Works — $749 |
| Month 3 | QR codes for mobile app | Add QRCoder — ~8 hrs dev time | Change encoding constant — 10 min |
| Month 6 | Read incoming supplier barcodes | Add ZXing.Net — ~12 hrs dev time | `BarcodeReader.Read()` — 30 min |
| Month 12 | Process PDF invoices with barcodes | Add PDF library — ~16 hrs dev time | Built-in — 30 min |
| Ongoing | Maintain 3 libraries, manage version conflicts | ~20 hrs/year | 1 library to update |

At $100/hr developer time, the NetBarcode "free" path accumulates approximately $5,600 in integration and maintenance cost over two years. IronBarcode totals $749 plus minimal integration time.

---

## When NetBarcode Is the Right Choice

NetBarcode fits well in a narrow set of situations:

**Genuinely 1D-only projects** — A point-of-sale system that generates UPC-A and EAN-13 codes for traditional retail scanners, with no mobile component, no QR requirement, and no reading need. If the requirements document explicitly rules out 2D formats and the project has a defined end date, NetBarcode delivers everything necessary at no cost.

**Legacy integrations** — Systems connecting to older industrial scanners that accept only Code39 or Code128, where the integration spec explicitly prohibits 2D formats.

**Zero-budget prototypes** — Throwaway tools or internal scripts where the total lifespan is measured in weeks and requirements will not evolve.

If there is any ambiguity about whether QR codes might be needed — and the answer from stakeholders is usually "we'll add that later" — NetBarcode starts a clock that runs out the moment that requirement lands.

## When IronBarcode Is the Right Choice

The clearer answer is IronBarcode when:

- The project needs any 2D format, now or in the future
- Barcode reading from images or documents is required
- The application serves industries with DataMatrix mandates (pharmaceutical, aerospace, automotive)
- Reducing the number of third-party dependencies is a priority
- The company's revenue is above $1M and the ImageSharp commercial license applies
