OnBarcode's pricing page says "Contact Us." The generator and reader are separate products with separate quotes. You cannot calculate your total cost until you've spoken to sales twice and compared two proposals. That is the situation before you write a single line of code.

The split product model has a practical consequence beyond pricing opacity. If your application generates barcodes today and you later need to add reading — for label verification, receiving workflows, or document parsing — you face a second sales conversation, a second purchase decision, a second license key to configure, and a second NuGet package to manage. The products have different APIs. They version independently. They can drift. [IronBarcode](https://ironsoftware.com/csharp/barcode/) ships generation and reading in a single package under a single license key, with a published price you can check right now without talking to anyone.

This comparison covers both libraries in detail: how they're set up, how the APIs compare, where each one has real strengths, and what the total cost of ownership looks like for a team that needs both capabilities.

## Understanding OnBarcode

OnBarcode is a barcode library built primarily around barcode generation. The company offers products across .NET, Java, Crystal Reports, SSRS, and other platforms. Within the .NET ecosystem, the main offerings are a Generator SDK and a Reader SDK — sold and distributed separately.

### The Distribution History

For much of its lifetime, OnBarcode was distributed as a DLL download. You'd contact sales, receive a download link, extract the ZIP, drop the assembly into a `lib/` folder, and wire it up manually in your `.csproj`:

```xml
<Reference Include="OnBarcode.Barcode">
  <HintPath>lib\OnBarcode.Barcode.dll</HintPath>
  <Private>true</Private>
</Reference>
```

That means binary files in source control, no automatic restore from a fresh clone, manual upgrade ceremonies, and CI pipelines that either carry those DLLs or require a custom download step. NuGet packages were added recently (2025–2026), which is a welcome improvement. But the documentation still partially assumes the old workflow, and the NuGet packages are new enough that the download counts reflect that — a meaningful signal for teams evaluating long-term reliability.

IronBarcode has been on NuGet since its launch. Over 2.1 million downloads. `dotnet add package IronBarcode` is the entire installation.

### The Split Product API

The most immediately visible consequence of OnBarcode's two-product model is the license configuration. A project that uses both generator and reader must do this:

```csharp
using OnBarcode.Barcode;
using OnBarcode.Barcode.Reader;

// Two licenses, two calls, two keys to rotate and manage
OnBarcode.Barcode.License.SetLicense("GENERATOR-LICENSE-KEY");
OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");
```

With IronBarcode:

```csharp
using IronBarCode;

// One key covers generation, reading, and everything else
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

That single line is the full license setup. The same key works for everything — reading, writing, PDFs, batch operations — regardless of what you're doing.

## Generation: Verbosity Comparison

OnBarcode's generation API is instance-based and property-driven. The primary output method is named `drawBarcode()` with a lowercase `d`, which is the first thing every developer notices when looking through IntelliSense. A basic Code 128 barcode looks like this:

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

Eight lines. Three of those lines are property assignments for values that have sensible defaults. The equivalent in IronBarcode:

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128).SaveAsPng("barcode.png");
```

One line. The method name is `SaveAsPng`, not `drawBarcode`. And there are parallel methods for every format: `SaveAsJpeg`, `SaveAsPdf`, `SaveAsGif`, `SaveAsTiff`. You do not need to check which format a generic save method will produce.

### QR Code Generation

Generating a QR code follows the same property-assignment pattern in OnBarcode:

```csharp
using OnBarcode.Barcode;

Barcode qr = new Barcode();
qr.Symbology = Symbology.QRCode;
qr.Data = "https://example.com";
qr.QRCodeDataMode = QRCodeDataMode.Auto;
qr.QRCodeECL = QRCodeECL.M;
qr.drawBarcode("qrcode.png");
```

IronBarcode:

```csharp
BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode).SaveAsPng("qrcode.png");
```

### QR Code with a Branded Logo

This is where the API gap is most visible. OnBarcode can generate the base QR code, but there is no built-in logo overlay. You have to reach into `System.Drawing`, set high error correction level, do the math for centering, and draw the overlay yourself:

```csharp
using OnBarcode.Barcode;
using System.Drawing;

Barcode qr = new Barcode();
qr.Symbology = Symbology.QRCode;
qr.Data = "https://example.com";
qr.QRCodeDataMode = QRCodeDataMode.Auto;
qr.QRCodeECL = QRCodeECL.H; // H required for logo overlay

Image qrImage = qr.drawBarcode();

using (Graphics g = Graphics.FromImage(qrImage))
{
    Image logo = Image.FromFile("logo.png");
    int logoSize = qrImage.Width / 5;
    int x = (qrImage.Width - logoSize) / 2;
    int y = (qrImage.Height - logoSize) / 2;
    g.DrawImage(logo, x, y, logoSize, logoSize);
}

qrImage.Save("qr-with-logo.png");
```

IronBarcode has this built in:

```csharp
var qr = QRCodeWriter.CreateQrCodeWithLogo("https://example.com", "logo.png", 500);
qr.SaveAsPng("qr-with-logo.png");
```

## Reading Barcodes: A Separate Purchase

OnBarcode's reading capability is not included in the Generator SDK. It is a distinct product with its own NuGet package, its own license, and its own API namespace. To read barcodes from images, you need to:

1. Contact sales for a separate quote
2. `dotnet add package OnBarcode.Barcode.Reader`
3. Configure a second license key
4. Learn a second API

The reading code looks like this:

```csharp
using OnBarcode.Barcode.Reader;

OnBarcode.Barcode.Reader.License.SetLicense("READER-LICENSE-KEY");

BarcodeReader reader = new BarcodeReader();
reader.BarcodeTypes = new BarcodeType[] { BarcodeType.Code128 };
string[] results = reader.Scan("barcode.png");
```

A few things to note. You must specify `BarcodeTypes` up front — there is no auto-detection. The reader is an instance you construct and configure. And `Scan()` returns `string[]`, a flat array of raw string values with no metadata about which format was detected, what position the barcode occupied in the image, or what confidence level the recognition achieved.

IronBarcode's reading API auto-detects format and returns structured results. You do not need to tell it what format to look for, and you get back significantly more than a string array:

```csharp
using IronBarCode;

var results = BarcodeReader.Read("barcode.png");
foreach (var result in results)
    Console.WriteLine($"{result.Value} ({result.Format})");
```

Same package. Same license key. No format specification required. For applications that [read barcodes from images](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) that originate from external sources — scanners, cameras, uploads — auto-detection removes a whole category of bugs.

## PDF Support

OnBarcode does not support reading barcodes from PDFs. This affects a large number of real-world workflows: invoice processing, shipping label verification, document management systems, and any pipeline where documents arrive as PDFs rather than images.

To process barcodes in a PDF with OnBarcode, you'd need a separate PDF-to-image rendering library. That's another dependency to evaluate, license, and maintain.

IronBarcode reads barcodes directly from PDFs without any additional library. The [PDF barcode reading](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) capability is part of the same package:

```csharp
// Read barcodes directly from a PDF — no PDF rendering library needed
var pdfResults = BarcodeReader.Read("document.pdf");
```

Every page is processed. All barcode formats are detected automatically.

## Feature Comparison Table

| Feature | OnBarcode | IronBarcode |
|---|---|---|
| Barcode generation | Yes | Yes |
| Barcode reading | Separate purchase required | Included |
| PDF reading | No | Native |
| PDF generation | No | Native |
| Format auto-detection | No | Yes |
| QR with logo (built-in) | No (manual GDI+ required) | Yes |
| Published pricing | No — contact sales | Yes |
| Single license for read + write | No | Yes |
| NuGet-first distribution | Recent addition | Since launch |
| API style | Instance + properties | Static factory methods |
| `drawBarcode()` method | Yes (lowercase `d`) | No — `SaveAsPng()`, `SaveAsPdf()`, etc. |
| `string[]` results from reader | Yes (flat, no metadata) | No — `BarcodeResults` with format, value, position |
| 50+ symbologies | ~20 | Yes |
| Source code available | Unlimited tier only | No |

## Pricing

OnBarcode does not publish prices for either product. The generator has a "Contact Us" CTA. The reader has a "Contact Us" CTA. Until you've completed two separate sales conversations, you do not have a number you can put in a budget spreadsheet or procurement request.

IronBarcode publishes its [pricing](https://ironsoftware.com/csharp/barcode/licensing/) directly:

| License | Price | Developers | Includes |
|---|---|---|---|
| Lite | $749 | 1 | Generation + Reading + PDF |
| Professional | $1,499 | 10 | Generation + Reading + PDF |
| Unlimited | $2,999 | Unlimited | Generation + Reading + PDF |

All licenses are perpetual. There is no annual renewal required to continue using the software you purchased. Support and update subscriptions are optional.

For a five-developer team that needs both generation and reading, the OnBarcode path is: negotiate two separate deals, receive two invoices, and manage two license keys. The IronBarcode path is: $1,499 for the Professional license.

## When to Choose OnBarcode

OnBarcode makes sense in a specific set of circumstances:

**You only need generation.** If your application creates barcodes and will never need to read them, the Reader SDK is irrelevant and the split product model stops being a problem. OnBarcode's generator works and its property-based API is familiar to developers coming from other object-oriented barcode libraries.

**You need source code access.** OnBarcode's Unlimited Developer license includes full source code, which some organizations require for security auditing or internal customization. IronBarcode does not provide source code at any tier.

**You have an existing OnBarcode relationship.** If your team is already licensed for OnBarcode and the use case fits what you have, there is no reason to migrate unless the limitations are causing real friction.

## When to Choose IronBarcode

**You need both reading and writing.** The single-package, single-license model eliminates the setup overhead and budget uncertainty of managing two separate products.

**You need transparent pricing.** If upfront cost clarity matters for your procurement process — and it usually does — published pricing is a meaningful practical advantage.

**You need PDF support.** If your barcode workflow involves PDF documents, IronBarcode's native PDF reading removes a library dependency and a rendering step.

**You want auto-format detection.** When processing barcodes from external sources where format is not guaranteed, automatic detection reduces the surface area for bugs.

For a deeper look at how IronBarcode compares to OnBarcode across practical scenarios, see the [IronBarcode vs OnBarcode alternatives](https://ironsoftware.com/csharp/barcode/blog/compare-to-other-components/onbarcode-generator-alternatives/) overview.
