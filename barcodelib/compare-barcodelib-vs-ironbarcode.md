BarcodeLib has been downloaded over 12 million times. Most of those developers eventually discover it can only create barcodes. A significant portion of them also discover the SkiaSharp conflict — usually at the worst possible moment, when they've just added BarcodeLib to a MAUI or Blazor project that already depends on SkiaSharp, and the build starts throwing NU1608 warnings they didn't expect to be debugging that afternoon.

This article covers both issues honestly. BarcodeLib is a legitimate library with a clear use case. Understanding where it stops is the practical question.

## What BarcodeLib Does

BarcodeLib is an open-source barcode image generation library for .NET, maintained on GitHub by Brad Barnhill. It's been active since 2007 and supports over 25 barcode symbologies. The Apache 2.0 license makes it free for commercial use. For pure barcode generation — creating an image from a string — it works.

The API is instance-based. You create a `Barcode` object, set properties, and call `Encode()` with a type and data string:

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

This is the full generation workflow. It's clean, and for projects where you only ever need to print barcode images — shipping labels, inventory tags, retail price tags — it's sufficient.

## What BarcodeLib Cannot Do

There is no reading API in BarcodeLib. No `Decode()` method. No `Scan()` method. No `ReadBarcode()`. The library's public surface area is entirely generation-oriented, and this is not a feature that's missing from a particular version — it has never been part of the library's design.

If you search BarcodeLib's GitHub repository or its NuGet package API reference for any method that takes an image and returns a decoded value, you'll find nothing. Attempting to call a decode method will result in a compile error, not a runtime exception, because the method simply doesn't exist:

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

This is the gap that most teams hit when requirements expand. You generate barcodes for shipping labels. The warehouse team asks if the app can also scan labels received from suppliers. You open BarcodeLib's documentation, find no reading capability, and start evaluating alternatives.

## The SkiaSharp Conflict

Starting with BarcodeLib 3.x, the library introduced SkiaSharp as a graphics backend to replace `System.Drawing.Common`, which became Windows-only after .NET 6. The intent was correct — SkiaSharp is a cross-platform graphics library that works on Linux and macOS. The problem is version pinning.

BarcodeLib pins to a specific SkiaSharp version range. If your project already uses SkiaSharp through another dependency — and this is common in MAUI projects, which use `SkiaSharp.Views.Maui` and `Microsoft.Maui.Graphics` — you may be on a different SkiaSharp version than BarcodeLib expects. The result is a NU1608 warning at best and an assembly binding failure at runtime at worst.

In a typical MAUI project, the conflict looks like this in the build output:

```
warning NU1608: Detected package version outside of dependency constraint:
BarcodeLib 3.1.5 requires SkiaSharp (>= 2.88.7 && < 2.89.0) but
version SkiaSharp 3.116.1 was resolved.
```

The NU1608 warning means the package manager resolved a version outside the expected range. Whether this causes a runtime failure depends on which SkiaSharp APIs BarcodeLib calls internally — if it calls APIs that changed between versions, you get an exception at runtime. If it's lucky, it works but the version mismatch is a ticking clock.

Attempting to force a resolution through explicit package references adds complexity:

```xml
<!-- Attempt to force SkiaSharp version — may or may not resolve the conflict -->
<ItemGroup>
  <PackageReference Include="BarcodeLib" Version="3.1.5" />
  <!-- Override to satisfy MAUI's SkiaSharp requirements -->
  <PackageReference Include="SkiaSharp" Version="3.116.1" />
  <PackageReference Include="SkiaSharp.Views.Maui.Controls" Version="3.116.1" />
</ItemGroup>
```

Even with explicit overrides, you're relying on BarcodeLib being compatible with the version you've pinned. If it's not, you have no recourse — there's no SLA or support contract that guarantees a fix on a timeline that matters to your project.

IronBarcode avoids this class of problem entirely. It doesn't share the SkiaSharp dependency graph with your application code, so there's no version negotiation to manage and no NU1608 to diagnose.

## Understanding IronBarcode

IronBarcode is a commercial .NET barcode library that covers both generation and reading in a single package. It installs via NuGet, runs on .NET Framework 4.6.2 through .NET 9, and works on Windows, Linux, macOS, Docker, Azure, and AWS Lambda.

The generation API is static and fluent — no instance to create, no properties to set before calling `Encode()`:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// Generate Code128
BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .SaveAsPng("barcode.png");

// Generate QR code
QRCodeWriter.CreateQrCode("https://example.com/product", 500)
    .SaveAsPng("qr.png");

// QR code with embedded brand logo
QRCodeWriter.CreateQrCode("https://example.com/product", 500)
    .AddBrandLogo("logo.png")
    .SaveAsPng("qr-branded.png");

// Get bytes instead of saving to disk (useful for web APIs)
byte[] pngBytes = BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .ToPngBinaryData();
```

Reading is part of the same package — no separate library, no ZXing.Net integration to maintain:

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

## Feature Comparison

| Feature | BarcodeLib | IronBarcode |
|---|---|---|
| Barcode generation | Yes | Yes |
| Barcode reading / scanning | No | Yes (`BarcodeReader.Read()`) |
| QR code generation | Yes (basic) | Yes (advanced, with logo embedding) |
| PDF barcode reading | No | Yes (native, no extra library) |
| PDF barcode generation output | No | Yes |
| SkiaSharp dependency | Yes (version conflict risk) | No |
| MAUI project compatibility | Conflict risk (NU1608) | No conflict |
| Fluent chainable API | No | Yes |
| `byte[]` output directly | Manual (via SkiaSharp stream) | `.ToPngBinaryData()` |
| Multi-barcode detection | No | Yes (`ExpectMultipleBarcodes`) |
| Reading speed tuning | N/A | Yes (`ReadingSpeed` enum) |
| Linux / macOS support | Partial (SkiaSharp-dependent) | Full |
| Docker / container support | Configuration required | Yes |
| Active maintenance | Yes (community) | Yes (commercial) |
| Commercial support / SLA | No | Yes |
| License | Apache 2.0 (free) | $749–$5,999 perpetual |

## Side-by-Side Generation Code

Both libraries can produce a Code128 barcode from a string. Here's the same operation in each:

**BarcodeLib:**

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

**IronBarcode:**

```csharp
using IronBarCode;

public byte[] GenerateCode128(string data)
{
    return BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ResizeTo(300, 100)
        .AddAnnotationTextBelowBarcode(data)
        .ToPngBinaryData();
}
```

The IronBarcode version is shorter, but the more relevant difference is the absence of intermediate `Image` and `MemoryStream` objects. `ToPngBinaryData()` gives you the byte array directly, which is what most downstream consumers — HTTP responses, database BLOB columns, file writes — actually want.

## Adding Reading (The Scenario That Triggers Migration)

The scenario that most commonly pushes teams away from BarcodeLib is a feature request: "Can the app also read barcodes?"

With BarcodeLib installed, the answer is no — you'd need to add ZXing.Net or another scanning library alongside it. That means two libraries to maintain, two dependency graphs to manage, and two sets of API surfaces to learn. In a MAUI project that's already navigating SkiaSharp version conflicts, adding ZXing.Net (which also has its own dependency considerations) compounds the problem.

With IronBarcode, reading is available immediately:

```csharp
// IronBarcode: read back what you generated — or anything else
// NuGet: dotnet add package IronBarcode
using IronBarCode;

var results = BarcodeReader.Read("barcode.png");
Console.WriteLine(results.First().Value);  // "PRODUCT-12345"
```

Reading from a PDF requires the same call — the library handles PDF parsing internally:

```csharp
using IronBarCode;

// Read barcodes from every page of a scanned PDF
var results = BarcodeReader.Read("scanned-invoices.pdf");
foreach (var result in results)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value} ({result.Format})");
}
```

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

## When Teams Switch

Three scenarios drive most BarcodeLib-to-IronBarcode migrations:

**Reading requirement appears.** A system that generated shipping labels now needs to process returned labels. The warehouse integration needs to parse supplier barcodes. A document management system needs to index barcodes on scanned PDFs. BarcodeLib can't do any of these. The team evaluates adding ZXing.Net alongside BarcodeLib, decides the dual-library maintenance burden isn't worth it, and switches to a library that does both.

**SkiaSharp conflict in a MAUI project.** The team adds BarcodeLib to a MAUI app and immediately hits NU1608 warnings. They spend time investigating, try explicit package version overrides, get the build to pass, then hit a runtime crash on device when SkiaSharp's native binaries load the wrong version. The fix is non-trivial. Switching to IronBarcode removes the conflict because IronBarcode doesn't share the SkiaSharp dependency graph.

**Need to read barcodes from their own PDFs.** Applications that generate PDF documents with embedded barcodes — invoices, work orders, shipping manifests — sometimes need to read those barcodes back later. BarcodeLib generates barcode images but has no PDF support on either end. IronBarcode reads from PDFs natively with `BarcodeReader.Read("file.pdf")`.

## Conclusion

BarcodeLib's 12 million downloads reflect a real use case: generating barcode images in .NET is something a lot of developers need, and BarcodeLib has done it reliably for years at no cost. The library's limits are also real and documented: no reading API, no PDF support, and a SkiaSharp dependency that conflicts with MAUI projects and other SkiaSharp consumers. For teams whose requirements stay within pure image generation on Windows, those limits may never matter. For teams that discover they need scanning, PDF processing, or a clean dependency graph in a modern cross-platform app, the BarcodeLib generation-only boundary is exactly where the migration conversation starts.
