ZXing.Net's `BarcodeReader` is not thread-safe. The documentation says to create a new instance per thread. In a web API with concurrent requests, every request instantiates a reader, configures its format list, loads a bitmap, decodes it, and throws it away. IronBarcode's static method handles the same work without per-request allocation.

That is not an obscure edge case. It is the first problem you encounter when you move ZXing.Net code from a console app into an ASP.NET Core controller, or when you batch-process shipping labels across multiple threads. And thread safety is just one item on the list. No native PDF support, binding package fragmentation across Windows and Linux, and the requirement to specify every barcode format you want to scan for — these are the characteristics that cause ZXing.Net projects to accumulate workarounds over time.

ZXing.Net is the most downloaded open-source barcode library for .NET, a port of the Java ZXing ("Zebra Crossing") library that has been widely used for over a decade. Its format support is broad, its cost is zero, and for a prototype scanning known-format barcodes from controlled images, it does the job. This comparison is about what happens after the prototype.

[IronBarcode](https://ironsoftware.com/csharp/barcode/) is a commercial .NET barcode library. One package, a string license key, static read and write methods. The comparison below covers thread safety, format detection, PDF handling, platform support, barcode generation, and when each library is the better fit.

---

## Thread Safety

This is the most important operational difference between the two libraries, and it affects more projects than the documentation warnings might suggest.

ZXing.Net's `BarcodeReader` is a stateful object. When you call `Decode`, internal state is written. If two threads share the same instance, those writes race. The results are non-deterministic: you may get a result from a different image, a null where a barcode exists, or an exception. The correct approach is to create one reader per thread — meaning every parallel or async path in your application allocates a reader, configures it, uses it once, and discards it.

```csharp
// ZXing.Net: must create a new reader per thread — not safe to share
Parallel.ForEach(imagePaths, path =>
{
    var reader = new BarcodeReader();               // new instance per iteration
    reader.Options.PossibleFormats = new List<BarcodeFormat>
    {
        BarcodeFormat.QR_CODE,
        BarcodeFormat.CODE_128,
        BarcodeFormat.EAN_13
    };

    using var bitmap = new Bitmap(path);
    var result = reader.Decode(bitmap);

    if (result != null)
        results[path] = result.Text;
});
```

The `ThreadLocal<BarcodeReader>` pattern reduces per-call allocation by reusing one instance per thread, but it adds disposal complexity. Either way, thread safety in ZXing.Net is your responsibility to implement.

```csharp
// IronBarcode: static method, thread-safe, no instance management
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};

Parallel.ForEach(imagePaths, path =>
{
    var result = BarcodeReader.Read(path, options).FirstOrDefault();
    if (result != null)
        results[path] = result.Value;
});
```

IronBarcode's [async and multithreaded barcode reading](https://ironsoftware.com/csharp/barcode/how-to/async-multithread/) documentation explains the internal thread pooling model. The short version: `BarcodeReader.Read` is stateless from the caller's perspective. You can call it from any number of concurrent threads without coordinating access.

---

## Format Detection

ZXing.Net requires you to tell it what to look for. If a barcode format is not in your `PossibleFormats` list, it will not be detected — no error, no warning, just a null result. This is fine when you control what barcodes appear in your images, but it becomes a problem when images come from external sources.

```csharp
// ZXing.Net: format list required — miss one and it goes undetected
var reader = new BarcodeReader();
reader.Options.PossibleFormats = new List<BarcodeFormat>
{
    BarcodeFormat.QR_CODE,
    BarcodeFormat.CODE_128,
    BarcodeFormat.CODE_39,
    BarcodeFormat.EAN_13,
    BarcodeFormat.EAN_8,
    BarcodeFormat.UPC_A,
    BarcodeFormat.DATA_MATRIX,
    BarcodeFormat.PDF_417
    // What if the document has an AZTEC barcode? You'll get null.
};
reader.Options.TryHarder = true;

using var bitmap = new Bitmap(imagePath);
var results = reader.DecodeMultiple(bitmap);
```

If you list every format to avoid missing anything, you pay a performance cost. If you list only the formats you expect, you risk silent misses. There is no mechanism for ZXing.Net to report "I found a barcode but not in a format you asked for."

```csharp
// IronBarcode: no format list — all 50+ formats detected automatically
var results = BarcodeReader.Read(imagePath);
foreach (var barcode in results)
{
    Console.WriteLine($"{barcode.Value} ({barcode.Format})");
}
```

For applications that tune [reading speed against detection thoroughness](https://ironsoftware.com/csharp/barcode/how-to/reading-speed-options/), IronBarcode exposes a `ReadingSpeed` enum — `Faster`, `Balanced`, `Detailed`, `ExtremeDetail` — without requiring format enumeration.

---

## PDF Document Support

This is a hard technical limit in ZXing.Net: it has no PDF support. None. If a barcode arrives embedded in a PDF, you have two options: buy and integrate a separate PDF rendering library, or tell your users to submit images instead.

The most common workaround is PdfiumViewer, which adds roughly 20MB of native binaries, a page-rendering loop, temporary file management, and error handling for each page:

```csharp
// ZXing.Net: PDF requires PdfiumViewer + a 30-line render loop
using var pdfDocument = PdfDocument.Load(pdfPath);

var reader = new BarcodeReader();
reader.Options.PossibleFormats = new List<BarcodeFormat>
{
    BarcodeFormat.QR_CODE,
    BarcodeFormat.CODE_128,
    BarcodeFormat.PDF_417
};

for (int i = 0; i < pdfDocument.PageCount; i++)
{
    string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
    try
    {
        using var pageImage = pdfDocument.Render(i, 200, 200, PdfRenderFlags.CorrectFromDpi);
        pageImage.Save(tempPath, ImageFormat.Png);

        using var bitmap = new Bitmap(tempPath);
        var decoded = reader.DecodeMultiple(bitmap);
        if (decoded != null)
            results.AddRange(decoded.Select(d => d.Text));
    }
    finally
    {
        File.Delete(tempPath);
    }
}
```

This code has real failure modes beyond missing barcodes: DPI misconfiguration causes silent misses, temp file permissions fail in locked-down environments, the PdfiumViewer native libraries require separate deployment management, and it does not work on Linux without additional setup.

IronBarcode [reads barcodes from PDFs natively](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) — all pages, all formats, no external dependencies:

```csharp
// IronBarcode: one line, all pages, native PDF support
var results = BarcodeReader.Read("invoice.pdf");
foreach (var barcode in results)
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Value}");
```

---

## Platform Bindings

ZXing.Net splits its image-handling capability across platform-specific binding packages. The core `ZXing.Net` package provides decoding logic but no image loading. You choose a binding based on your platform:

| Environment | Package | Limitation |
|---|---|---|
| Windows desktop / WPF | `ZXing.Net.Bindings.Windows` | Uses `System.Drawing` — Windows only |
| Linux / Docker / cross-platform | `ZXing.Net.Bindings.ImageSharp` | Different API surface, additional `SixLabors.ImageSharp` dependency |
| macOS | `ZXing.Net.Bindings.ImageSharp` | Same as Linux path |

The practical consequence is that Windows code using `System.Drawing.Bitmap` does not run on Linux. If you start on Windows and later containerize on Linux, you refactor the image-loading code:

```csharp
// Windows path (does not compile/run on Linux)
using ZXing.Windows.Compatibility;
using System.Drawing;

var reader = new BarcodeReader();
using var bitmap = new Bitmap(imagePath);
var result = reader.Decode(bitmap);
```

```csharp
// Linux / cross-platform path — different API, different packages
using ZXing.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

var reader = new BarcodeReader<Rgba32>();
using var image = Image.Load<Rgba32>(imagePath);
var result = reader.Decode(image);
```

If you do use the Windows binding on a Linux host, you need `libgdiplus` in your Docker image, which adds roughly 50MB to your container size. IronBarcode ships one package for all platforms — the same `BarcodeReader.Read(path)` call compiles and runs identically on Windows, Linux, macOS, and inside Docker containers. For teams containerizing .NET applications, the [IronBarcode Docker and Linux setup guide](https://ironsoftware.com/csharp/barcode/get-started/docker-linux/) covers the single-package deployment pattern in detail.

---

## Feature Comparison Table

| Feature | ZXing.Net | IronBarcode |
|---|---|---|
| License | Apache 2.0 (free) | Commercial |
| NuGet packages needed | 2–3 (core + binding + optional ImageSharp) | 1 |
| Thread safety | Not thread-safe — new instance per thread required | Thread-safe static API |
| Format detection | Manual — `PossibleFormats` list required | Automatic — 50+ formats |
| PDF reading | No — requires PdfiumViewer or similar (~20MB extra) | Native — single method call |
| Platform support | Windows (System.Drawing) or Linux (ImageSharp) — separate code paths | All platforms, one API |
| Docker setup | May require `libgdiplus` apt-get | No extra dependencies |
| Barcode generation | Yes — returns `Bitmap`, requires manual save | Yes — fluent API, saves directly |
| QR code customization | No logo support | Logo embedding, color control |
| Damaged barcode handling | `TryHarder` flag | ML-powered image correction |
| Support | Community / GitHub issues | Commercial SLA |
| Pricing | Free | From $749 (Lite) / $1,499 (Professional) |

---

## Barcode Generation

Both libraries generate barcodes, but the APIs reflect different design eras.

```csharp
// ZXing.Net: create writer, set options, call Write, save Bitmap manually
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using System.Drawing.Imaging;

var writer = new BarcodeWriter
{
    Format = BarcodeFormat.CODE_128,
    Options = new EncodingOptions
    {
        Width = 300,
        Height = 100,
        Margin = 10
    }
};

using var bitmap = writer.Write("PRODUCT-001");
bitmap.Save("output.png", ImageFormat.Png);
```

```csharp
// IronBarcode: create and save in one fluent chain
BarcodeWriter.CreateBarcode("PRODUCT-001", BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .SaveAsPng("output.png");
```

The IronBarcode writer also returns a `GeneratedBarcode` object that supports `.ToPngBinaryData()`, `.ToJpegBinaryData()`, `.ToStream()`, and `.SaveAsPdf()` — output targets that ZXing.Net requires you to implement yourself using `System.Drawing`.

---

## Side-by-Side: Basic Read

The simplest possible operation shows the ergonomic gap clearly:

```csharp
// ZXing.Net: 8 lines for a single read
var reader = new BarcodeReader();
reader.Options.PossibleFormats = new List<BarcodeFormat>
{
    BarcodeFormat.QR_CODE,
    BarcodeFormat.CODE_128
};
using var bitmap = new Bitmap("barcode.png");
var result = reader.Decode(bitmap);
string value = result?.Text ?? "";
```

```csharp
// IronBarcode: 1 line
string value = BarcodeReader.Read("barcode.png").FirstOrDefault()?.Value ?? "";
```

The difference is not cosmetic. The ZXing.Net version has three things that can return null (`Decode`, `result`, `result.Text`) and one `using` block that must be managed. The IronBarcode version has one nullable access point and no resource management.

---

## When to Choose ZXing.Net

ZXing.Net makes sense when:

- Budget is genuinely zero and the project is small-scale
- You are prototyping or learning and need a working example quickly
- Your application runs in a mobile context using ZXing.Net.Maui for live camera scanning — that specific niche is not covered by IronBarcode
- The project is open-source with a license requirement for Apache 2.0 components
- You control the barcode format completely — your own labels, known format, clean images

## When to Choose IronBarcode

IronBarcode makes sense when:

- Your application processes PDFs with embedded barcodes
- Images arrive from external sources with unknown barcode formats
- The code runs in a concurrent environment (web API, background job, parallel batch)
- You deploy to Docker or Linux and want one consistent code path
- Production missed barcodes carry a real cost — logistics errors, failed check-ins, audit failures
- Developer time costs more than the license

---

## Further Reading

For a deeper look at how ZXing.Net compares to IronBarcode in specific scanning scenarios, the [ZXing.Net vs IronBarcode barcode scanner comparison](https://ironsoftware.com/csharp/barcode/blog/compare-to-other-components/zxing-barcode-scanner/) covers detection rates and real-world use case analysis.
