# Migrate from ZXing.Net to IronBarcode

ZXing.Net's `BarcodeReader` is not thread-safe. Every concurrent read path in your application — every parallel `ForEach`, every async controller action handling simultaneous requests — needs its own reader instance. That means format configuration repeated per instance, `Bitmap` loading and disposal per call, and reader allocation on every request. When you hit scale, you are not paying for barcode decoding. You are paying for object churn.

This guide replaces that pattern with a static method call. It removes `ZXing.Net`, `ZXing.Net.Bindings.Windows`, `ZXing.Net.Bindings.ImageSharp`, and `PdfiumViewer` (if you added it for PDF workarounds), installs one package, and replaces every `new BarcodeReader()` with `BarcodeReader.Read()`. No format lists. No `Bitmap` management. No `ThreadLocal` gymnastics.

---

## Why Migrate

**Thread safety is your problem in ZXing.Net, not the library's.** When a shared `BarcodeReader` is accessed from two threads, internal state corrupts silently. The result is non-deterministic: wrong values, nulls where barcodes exist, occasional exceptions. The recommended fix is per-thread instances, but that trades race conditions for memory pressure — each instance carries the full decoder set. IronBarcode's static API is designed to be called from any thread concurrently; there is no instance to share or isolate.

**You specify formats or you miss barcodes.** ZXing.Net requires `reader.Options.PossibleFormats` to be set before every decode. If a QR code arrives on a document you only configured for Code 128, you get null — not an error, not a partial result, just nothing. When processing documents from third parties, that silent miss can mean a mis-shipped order, a failed patient check-in, or an audit record that never gets written. IronBarcode detects all formats automatically.

**PDF support is not included.** ZXing.Net has no PDF reader. Adding `PdfiumViewer` (or Ghostscript, or iTextSharp) gives you a PDF renderer, but you own the integration: page-by-page rendering, DPI configuration, temp file creation, cleanup on success and failure, and then the ZXing read loop on top. That is 30–50 lines of infrastructure code that exists solely because ZXing.Net cannot open a PDF. IronBarcode [reads barcodes directly from PDFs](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/) — all pages, no temp files, no external PDF library.

**Platform bindings fragment your code.** If you started on Windows with `ZXing.Net.Bindings.Windows` and later containerized on Linux, you had to switch to `ZXing.Net.Bindings.ImageSharp` and rewrite the image-loading code. Different packages, different APIs, different `using` directives. IronBarcode uses one API surface across all platforms.

---

## Quick Start (3 Steps)

### Step 1: Remove ZXing.Net Packages

```bash
dotnet remove package ZXing.Net
dotnet remove package ZXing.Net.Bindings.Windows
dotnet remove package ZXing.Net.Bindings.ImageSharp
dotnet remove package PdfiumViewer
```

If you installed native PdfiumViewer binaries separately:

```bash
dotnet remove package PdfiumViewer.Native.x64.v8-xfa
```

If your Dockerfile contains `apt-get install -y libgdiplus` — added because you used `System.Drawing` through the Windows binding on Linux — that line can be removed after the migration.

### Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

No additional binding packages, no platform-specific dependencies. The same NuGet package runs on Windows, Linux, macOS, and in Docker containers. For details on [deploying IronBarcode in Docker and Linux environments](https://ironsoftware.com/csharp/barcode/get-started/docker-linux/), the setup guide covers the exact Dockerfile pattern.

### Step 3: Replace Imports and Add License

Remove all ZXing namespaces from your files:

```csharp
// Remove these:
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using ZXing.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using PdfiumViewer;
```

Add the IronBarcode namespace and license initialization at application startup:

```csharp
// Add this import to all files using barcode operations:
using IronBarCode;

// Add this once at startup (Program.cs / Startup.cs):
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

The license key can come from an environment variable, `appsettings.json`, or a secrets manager — anywhere you can read a string. There is no file to locate and no path to configure.

---

## Code Migration Examples

### Example 1: Basic Barcode Reading

This is the most common ZXing.Net usage pattern. Every reader instance requires format configuration before it can be used.

**Before (ZXing.Net):**
```csharp
using ZXing;
using ZXing.Windows.Compatibility;
using System.Drawing;

public string ReadBarcode(string imagePath)
{
    var reader = new BarcodeReader();
    reader.Options.PossibleFormats = new List<BarcodeFormat>
    {
        BarcodeFormat.QR_CODE,
        BarcodeFormat.CODE_128,
        BarcodeFormat.EAN_13
    };

    using var bitmap = new Bitmap(imagePath);
    var result = reader.Decode(bitmap);

    return result?.Text ?? "";
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

public string ReadBarcode(string imagePath)
{
    var result = BarcodeReader.Read(imagePath).FirstOrDefault();
    return result?.Value ?? "";
}
```

No format list, no `Bitmap` allocation, no `using` block. If the image contains a barcode of any of the 50+ supported formats, it will be found.

---

### Example 2: Reading Multiple Barcodes

**Before (ZXing.Net):**
```csharp
using ZXing;
using ZXing.Windows.Compatibility;
using System.Drawing;

public List<string> ReadAllBarcodes(string imagePath)
{
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
    };
    reader.Options.TryHarder = true;

    using var bitmap = new Bitmap(imagePath);
    var results = reader.DecodeMultiple(bitmap);

    return results?.Select(r => r.Text).ToList() ?? new List<string>();
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

public List<string> ReadAllBarcodes(string imagePath)
{
    return BarcodeReader.Read(imagePath)
        .Select(r => r.Value)
        .ToList();
}
```

`BarcodeReader.Read` always returns a collection — never null. No need for null-guarding the result, and no need to enumerate every format you might encounter. If throughput matters, the [reading speed options guide](https://ironsoftware.com/csharp/barcode/how-to/reading-speed-options/) covers the `ReadingSpeed` enum that controls the accuracy-versus-speed trade-off without requiring you to narrow the format list.

---

### Example 3: Concurrent / Parallel Reading

This is where the thread-safety difference is most visible. ZXing.Net's `BarcodeReader` cannot be shared across threads. The safest approach is a new instance per parallel operation, but that allocates and discards a reader — with its full set of configured decoders — for every image.

**Before (ZXing.Net):**
```csharp
using ZXing;
using ZXing.Windows.Compatibility;
using System.Collections.Concurrent;
using System.Drawing;

public Dictionary<string, string> ProcessBatch(string[] files)
{
    var results = new ConcurrentDictionary<string, string>();

    // ZXing.Net is NOT thread-safe — new reader per parallel operation
    Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 4 }, file =>
    {
        var reader = new BarcodeReader();
        reader.Options.PossibleFormats = new List<BarcodeFormat>
        {
            BarcodeFormat.QR_CODE,
            BarcodeFormat.CODE_128
        };

        using var bitmap = new Bitmap(file);
        var result = reader.Decode(bitmap);
        if (result != null)
            results[file] = result.Text;
    });

    return results.ToDictionary(x => x.Key, x => x.Value);
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;
using System.Collections.Concurrent;

public Dictionary<string, string> ProcessBatch(string[] files)
{
    var results = new ConcurrentDictionary<string, string>();
    var options = new BarcodeReaderOptions { MaxParallelThreads = 4 };

    Parallel.ForEach(files, file =>
    {
        var result = BarcodeReader.Read(file, options).FirstOrDefault();
        if (result != null)
            results[file] = result.Value;
    });

    return results.ToDictionary(x => x.Key, x => x.Value);
}
```

No reader instance to manage. `BarcodeReader.Read` is safe to call from concurrent threads; the library handles internal resource pooling. For more detail on configuring [concurrent barcode reading with thread count controls](https://ironsoftware.com/csharp/barcode/how-to/async-multithread/), the how-to guide covers `MaxParallelThreads` and async patterns.

---

### Example 4: PDF Reading

If your ZXing.Net integration used PdfiumViewer, this migration produces the largest code reduction. The entire PDF rendering pipeline — including page enumeration, DPI configuration, temp file management, and cleanup — is replaced by a single method call.

**Before (ZXing.Net + PdfiumViewer):**
```csharp
using ZXing;
using ZXing.Windows.Compatibility;
using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    var results = new List<string>();
    var reader = new BarcodeReader();
    reader.Options.PossibleFormats = new List<BarcodeFormat>
    {
        BarcodeFormat.QR_CODE,
        BarcodeFormat.CODE_128,
        BarcodeFormat.PDF_417
    };

    using var pdfDocument = PdfDocument.Load(pdfPath);

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
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    return results;
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

public List<string> ReadBarcodesFromPdf(string pdfPath)
{
    return BarcodeReader.Read(pdfPath)
        .Select(r => r.Value)
        .ToList();
}
```

All pages are processed automatically. Format detection is automatic. There are no temp files and no cleanup to write.

---

### Example 5: Barcode Generation

The ZXing.Net writer returns a `Bitmap`, which you then save using `System.Drawing.Imaging`. IronBarcode's writer returns a `GeneratedBarcode` object with built-in output methods.

**Before (ZXing.Net):**
```csharp
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using System.Drawing;
using System.Drawing.Imaging;

public void GenerateCode128(string data, string outputPath)
{
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

    using var bitmap = writer.Write(data);
    bitmap.Save(outputPath, ImageFormat.Png);
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

public void GenerateCode128(string data, string outputPath)
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ResizeTo(300, 100)
        .SaveAsPng(outputPath);
}
```

If you need binary output instead of saving to disk, `ToPngBinaryData()` and `ToJpegBinaryData()` are available on the returned `GeneratedBarcode` object — no `MemoryStream` plumbing required.

---

## API Mapping Reference

| ZXing.Net | IronBarcode | Notes |
|---|---|---|
| `new BarcodeReader()` | Static — no instance | `BarcodeReader.Read(...)` is a static call |
| `reader.Options.PossibleFormats = new List<BarcodeFormat> { ... }` | Not needed | Auto-detection covers all formats |
| `reader.Options.TryHarder = true` | `Speed = ReadingSpeed.Balanced` | Accuracy tuning via `BarcodeReaderOptions` |
| `reader.Decode(bitmap)` | `BarcodeReader.Read(imagePath)` | Pass path directly — no `Bitmap` needed |
| `reader.DecodeMultiple(bitmap)` | `BarcodeReader.Read(imagePath)` | Always returns a collection |
| `result.Text` | `result.Value` | Property renamed |
| `result.BarcodeFormat` | `result.Format` | Property renamed |
| `BarcodeFormat.QR_CODE` | `BarcodeEncoding.QRCode` | Enum naming convention change |
| `BarcodeFormat.CODE_128` | `BarcodeEncoding.Code128` | Enum naming convention change |
| `BarcodeFormat.EAN_13` | `BarcodeEncoding.EAN13` | Enum naming convention change |
| `new BarcodeWriter { Format = BarcodeFormat.CODE_128 }` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)` | Static factory method |
| `writer.Write(data)` returns `Bitmap` | `.SaveAsPng(path)` / `.ToPngBinaryData()` | Built-in output methods |
| No PDF support — needs PdfiumViewer | `BarcodeReader.Read("doc.pdf")` | Native PDF support |
| Not thread-safe | Thread-safe static API | No instance management needed |

---

## Migration Checklist

Use the following grep patterns to find every ZXing.Net reference in your codebase:

```
using ZXing
new BarcodeReader()
reader.Options.PossibleFormats
BarcodeFormat.
reader.Decode(
reader.DecodeMultiple(
result.Text
result.BarcodeFormat
new BarcodeWriter
writer.Write(
EncodingOptions
```

For each match:

- `using ZXing` — Replace with `using IronBarCode;`
- `new BarcodeReader()` — Remove the instantiation; replace `reader.Decode(bitmap)` calls with `BarcodeReader.Read(path)`
- `reader.Options.PossibleFormats = ...` — Delete the entire block; no format list is needed
- `BarcodeFormat.QR_CODE` / `BarcodeFormat.CODE_128` / `BarcodeFormat.EAN_13` — Update to `BarcodeEncoding.QRCode` / `BarcodeEncoding.Code128` / `BarcodeEncoding.EAN13`
- `reader.Decode(bitmap)` — Replace with `BarcodeReader.Read(imagePath).FirstOrDefault()`
- `reader.DecodeMultiple(bitmap)` — Replace with `BarcodeReader.Read(imagePath)`
- `result.Text` — Replace with `result.Value`
- `result.BarcodeFormat` — Replace with `result.Format`
- `new BarcodeWriter { Format = BarcodeFormat.X, Options = new EncodingOptions { ... } }` — Replace with `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.X).ResizeTo(w, h)`
- `writer.Write(data)` returning `Bitmap` — Replace with `BarcodeWriter.CreateBarcode(data, format).SaveAsPng(path)`
- `EncodingOptions` — Remove; sizing is handled by `.ResizeTo()`

After making changes, search for remaining `using System.Drawing` imports. If they were only there to support `Bitmap` loading for ZXing.Net, remove them. IronBarcode accepts file paths and byte arrays directly — no `Bitmap` construction needed anywhere.

---

The thread-safety problem that motivated this migration does not go away on its own. Every time a new team member writes a service, every time a background job gets parallelized, every time request volume increases — a shared `BarcodeReader` is a latent race condition. The static method removes the instance entirely. There is nothing to share, nothing to protect, and nothing to configure before the first call. The [ZXing.Net vs IronBarcode scanner comparison](https://ironsoftware.com/csharp/barcode/blog/compare-to-other-components/zxing-barcode-scanner/) provides additional context on detection rates and format support if you want a broader picture before committing to the migration.
