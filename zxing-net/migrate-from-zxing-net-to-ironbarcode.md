# Migrating from ZXing.Net to IronBarcode

This guide covers a complete migration from ZXing.Net to IronBarcode for .NET applications that read and generate barcodes. It addresses the removal of ZXing.Net's core package and binding packages, the elimination of PdfiumViewer workarounds where present, and the replacement of every stateful `BarcodeReader` instantiation with IronBarcode's static API. Each section includes before-and-after code drawn from the patterns most commonly found in ZXing.Net integrations.

## Why Migrate from ZXing.Net

ZXing.Net has served the .NET community well for over a decade as a free, Apache 2.0 barcode library. However, several architectural constraints become operational costs as applications grow in scale or complexity.

**Thread Safety Ownership:** ZXing.Net's `BarcodeReader` is a stateful object. The library's own documentation states that instances must not be shared across threads. In practice, this means every concurrent processing path — every controller action handling simultaneous requests, every task in a parallel batch — must allocate, configure, and discard its own reader. As request volume increases, this allocation pattern produces measurable memory pressure. The `ThreadLocal<BarcodeReader>` alternative reduces allocation frequency but introduces its own disposal complexity. In both cases, correctness under concurrency is the caller's problem, not a guarantee provided by the library.

**Format Specification Risk:** ZXing.Net requires `reader.Options.PossibleFormats` to be populated before each decode operation. A barcode in a format not present in that list returns null — no exception, no warning, no indication that a barcode was present but unrecognised. For applications that receive documents from external partners, this silent-miss behaviour is a reliability risk: a mis-shipped order, a missed patient check-in record, or an audit entry that never gets written can all originate from a format list that was configured for a previous requirement and never updated.

**PDF Processing Gap:** ZXing.Net reads bitmaps only. When the source documents are PDFs — invoices, shipping manifests, compliance certificates — callers must integrate a separate PDF rendering library (PdfiumViewer being the most common), implement a page enumeration loop, configure DPI, manage temporary files, and handle cleanup on both success and failure paths. This is 30 to 50 lines of infrastructure code per integration that exists solely to convert what ZXing.Net cannot read (PDFs) into what it can (bitmaps). That infrastructure has its own dependencies, its own deployment requirements, and its own failure modes.

**Platform Binding Fragmentation:** ZXing.Net's image loading capability is split across platform-specific binding packages. The Windows binding uses `System.Drawing.Bitmap`; the cross-platform binding uses `SixLabors.ImageSharp` with a different generic type parameter and a different `using` directive. A project that starts on Windows and later containerises on Linux must change both its NuGet references and its image-loading code. Two code paths for the same operation increase the surface area for bugs and add friction to future changes in the scanning layer.

### The Fundamental Problem

The clearest illustration of the ZXing.Net architecture is the gap between a single-threaded call and a concurrent one. In ZXing.Net, concurrent processing requires creating a full reader object per parallel operation:

```csharp
// ZXing.Net: full reader instantiation and configuration on every parallel task
Parallel.ForEach(files, file =>
{
    var reader = new BarcodeReader();
    reader.Options.PossibleFormats = new List<BarcodeFormat>
    {
        BarcodeFormat.QR_CODE,
        BarcodeFormat.CODE_128,
        BarcodeFormat.EAN_13
    };

    using var bitmap = new Bitmap(file);
    var result = reader.Decode(bitmap);
    if (result != null)
        processed[file] = result.Text;
});
```

IronBarcode removes the object entirely. The static call is the same whether it runs on one thread or one hundred:

```csharp
// IronBarcode: static call, same code for single-threaded and concurrent use
Parallel.ForEach(files, file =>
{
    var result = BarcodeReader.Read(file).FirstOrDefault();
    if (result != null)
        processed[file] = result.Value;
});
```

## IronBarcode vs ZXing.Net: Feature Comparison

| Feature | ZXing.Net | IronBarcode |
|---|---|---|
| **Licence** | Apache 2.0 (free) | Commercial |
| **Thread Safety** | Not thread-safe — new instance per thread required | Thread-safe static API |
| **Format Detection** | Manual — `PossibleFormats` list required | Automatic — 50+ formats |
| **PDF Reading** | No — requires PdfiumViewer or similar | Native — single method call |
| **Platform Support** | Separate binding packages per platform | Single package, all platforms |
| **Multiple Barcodes per Image** | Yes — `DecodeMultiple` | Yes — default behaviour |
| **Barcode Generation** | Returns `Bitmap`, requires manual save | Fluent API with built-in output |
| **QR Code Logo Support** | No | Yes |
| **QR Code Colour Control** | No | Yes |
| **Damaged Barcode Recovery** | `TryHarder` flag | ML-powered image correction |
| **Docker Extra Dependencies** | May require `libgdiplus` | None |
| **Commercial SLA Support** | No | Yes |
| **NuGet Packages Required** | 2–3 (core + binding + optional ImageSharp) | 1 |

## Quick Start

### Step 1: Remove ZXing.Net Packages

Remove all ZXing.Net and related packages from the project:

```bash
dotnet remove package ZXing.Net
dotnet remove package ZXing.Net.Bindings.Windows
dotnet remove package ZXing.Net.Bindings.ImageSharp
dotnet remove package PdfiumViewer
```

If the native PdfiumViewer binaries were installed as a separate package:

```bash
dotnet remove package PdfiumViewer.Native.x64.v8-xfa
```

If the Dockerfile contains `apt-get install -y libgdiplus` — added to support `System.Drawing` through the Windows binding on Linux — that line can be removed after the migration.

### Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

No additional binding packages or platform-specific dependencies are required. The same NuGet package runs on Windows, Linux, macOS, and in Docker containers. For deployment details, the [IronBarcode Docker and Linux setup guide](https://ironsoftware.com/csharp/barcode/get-started/docker-linux/) covers the exact Dockerfile pattern.

### Step 3: Update Namespaces and Initialise Licence

Remove all ZXing-related namespaces from every file that uses barcode operations:

```csharp
// Remove all of these:
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using ZXing.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using PdfiumViewer;

// Add this single import:
using IronBarCode;
```

Add licence initialisation at application startup, before any barcode operations:

```csharp
// Program.cs or Startup.cs — initialise once at startup
IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

The licence key is a plain string that can be loaded from an environment variable, `appsettings.json`, or a secrets manager. There is no file to locate and no path to configure.

## Code Migration Examples

### Reading a Single Barcode from an Image

This example replaces the most common ZXing.Net usage pattern: instantiating a reader, configuring the format list, loading a bitmap, and decoding.

**ZXing.Net Approach:**
```csharp
using ZXing;
using ZXing.Windows.Compatibility;
using System.Drawing;

public string ReadSingleBarcode(string imagePath)
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

    return result?.Text ?? string.Empty;
}
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

public string ReadSingleBarcode(string imagePath)
{
    var result = BarcodeReader.Read(imagePath).FirstOrDefault();
    return result?.Value ?? string.Empty;
}
```

The format list, the `Bitmap` allocation, and the `using` block are all removed. IronBarcode accepts a file path directly and detects all supported formats automatically. The property that previously returned the decoded string was `result.Text` in ZXing.Net; it is `result.Value` in IronBarcode.

### Reading All Barcodes from an Image

This example replaces `DecodeMultiple`, which in ZXing.Net requires an exhaustive format list to avoid silent misses.

**ZXing.Net Approach:**
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

**IronBarcode Approach:**
```csharp
using IronBarCode;

public List<string> ReadAllBarcodes(string imagePath)
{
    return BarcodeReader.Read(imagePath)
        .Select(r => r.Value)
        .ToList();
}
```

`BarcodeReader.Read` always returns a collection and never returns null, eliminating the null-guard on the result. To tune the accuracy-versus-speed balance, see the [reading speed options guide](https://ironsoftware.com/csharp/barcode/how-to/reading-speed-options/), which covers the `ReadingSpeed` enum without narrowing the format scope.

### Concurrent Batch Processing

This example shows the thread-safety difference most directly. ZXing.Net requires a new reader per parallel operation; IronBarcode uses the same static call regardless of the concurrency level.

**ZXing.Net Approach:**
```csharp
using ZXing;
using ZXing.Windows.Compatibility;
using System.Collections.Concurrent;
using System.Drawing;

public Dictionary<string, string> ProcessBatch(IEnumerable<string> filePaths)
{
    var output = new ConcurrentDictionary<string, string>();

    Parallel.ForEach(filePaths, new ParallelOptions { MaxDegreeOfParallelism = 4 }, file =>
    {
        // ZXing.Net is not thread-safe — a new reader is required per parallel operation
        var reader = new BarcodeReader();
        reader.Options.PossibleFormats = new List<BarcodeFormat>
        {
            BarcodeFormat.QR_CODE,
            BarcodeFormat.CODE_128
        };

        using var bitmap = new Bitmap(file);
        var result = reader.Decode(bitmap);
        if (result != null)
            output[file] = result.Text;
    });

    return output.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
}
```

**IronBarcode Approach:**
```csharp
using IronBarCode;
using System.Collections.Concurrent;

public Dictionary<string, string> ProcessBatch(IEnumerable<string> filePaths)
{
    var output = new ConcurrentDictionary<string, string>();
    var options = new BarcodeReaderOptions { MaxParallelThreads = 4 };

    Parallel.ForEach(filePaths, file =>
    {
        var result = BarcodeReader.Read(file, options).FirstOrDefault();
        if (result != null)
            output[file] = result.Value;
    });

    return output.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
}
```

There is no reader instance to protect or isolate. For async pipeline patterns and thread count configuration, see the [async and multithreaded barcode reading guide](https://ironsoftware.com/csharp/barcode/how-to/async-multithread/).

### Extracting Barcodes from a PDF

This example replaces the PdfiumViewer render loop. If the existing integration used Ghostscript or iTextSharp instead of PdfiumViewer, the before block will look different in detail but structurally identical: a page enumeration loop feeding bitmaps to ZXing.Net.

**ZXing.Net Approach:**
```csharp
using ZXing;
using ZXing.Windows.Compatibility;
using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;

public List<string> ExtractBarcodesFromPdf(string pdfPath)
{
    var collected = new List<string>();

    var reader = new BarcodeReader();
    reader.Options.PossibleFormats = new List<BarcodeFormat>
    {
        BarcodeFormat.QR_CODE,
        BarcodeFormat.CODE_128,
        BarcodeFormat.PDF_417
    };

    using var document = PdfDocument.Load(pdfPath);
    for (int page = 0; page < document.PageCount; page++)
    {
        string temp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
        try
        {
            using var rendered = document.Render(page, 200, 200, PdfRenderFlags.CorrectFromDpi);
            rendered.Save(temp, ImageFormat.Png);

            using var bmp = new Bitmap(temp);
            var decoded = reader.DecodeMultiple(bmp);
            if (decoded != null)
                collected.AddRange(decoded.Select(d => d.Text));
        }
        finally
        {
            if (File.Exists(temp))
                File.Delete(temp);
        }
    }

    return collected;
}
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

public List<string> ExtractBarcodesFromPdf(string pdfPath)
{
    return BarcodeReader.Read(pdfPath)
        .Select(r => r.Value)
        .ToList();
}
```

The PdfiumViewer dependency, the page render loop, the DPI configuration, the temp file creation, and the cleanup block are all removed. IronBarcode [reads barcodes from PDFs natively](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-pdf/), processing all pages automatically.

### Generating a Barcode

The ZXing.Net writer returns a `Bitmap` that callers must save using `System.Drawing.Imaging`. IronBarcode returns a `GeneratedBarcode` with built-in output methods.

**ZXing.Net Approach:**
```csharp
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using System.Drawing;
using System.Drawing.Imaging;

public void WriteCode128(string content, string outputPath)
{
    var writer = new BarcodeWriter
    {
        Format = BarcodeFormat.CODE_128,
        Options = new EncodingOptions
        {
            Width = 400,
            Height = 120,
            Margin = 10
        }
    };

    using var bitmap = writer.Write(content);
    bitmap.Save(outputPath, ImageFormat.Png);
}
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

public void WriteCode128(string content, string outputPath)
{
    BarcodeWriter.CreateBarcode(content, BarcodeEncoding.Code128)
        .ResizeTo(400, 120)
        .SaveAsPng(outputPath);
}
```

If binary output is needed instead of saving to a file, `ToPngBinaryData()` and `ToJpegBinaryData()` are available on the returned `GeneratedBarcode` object without any `MemoryStream` plumbing.

## ZXing.Net API to IronBarcode Mapping Reference

| ZXing.Net | IronBarcode | Notes |
|---|---|---|
| `new BarcodeReader()` | Static — no instance | `BarcodeReader.Read(...)` is a static call |
| `reader.Options.PossibleFormats = new List<BarcodeFormat> { ... }` | Not needed | Auto-detection covers all 50+ formats |
| `reader.Options.TryHarder = true` | `Speed = ReadingSpeed.Balanced` | Accuracy tuning via `BarcodeReaderOptions` |
| `reader.Decode(bitmap)` | `BarcodeReader.Read(imagePath).FirstOrDefault()` | Pass file path — no `Bitmap` construction needed |
| `reader.DecodeMultiple(bitmap)` | `BarcodeReader.Read(imagePath)` | Always returns a collection; never null |
| `result.Text` | `result.Value` | Property renamed |
| `result.BarcodeFormat` | `result.Format` | Property renamed |
| `BarcodeFormat.QR_CODE` | `BarcodeEncoding.QRCode` | Enum naming convention change |
| `BarcodeFormat.CODE_128` | `BarcodeEncoding.Code128` | Enum naming convention change |
| `BarcodeFormat.EAN_13` | `BarcodeEncoding.EAN13` | Enum naming convention change |
| `BarcodeFormat.DATA_MATRIX` | `BarcodeEncoding.DataMatrix` | Enum naming convention change |
| `BarcodeFormat.PDF_417` | `BarcodeEncoding.PDF417` | Enum naming convention change |
| `new BarcodeWriter { Format = ..., Options = new EncodingOptions { ... } }` | `BarcodeWriter.CreateBarcode(data, encoding)` | Static factory, no options object |
| `writer.Write(data)` returns `Bitmap` | `.SaveAsPng(path)` / `.ToPngBinaryData()` | Built-in output on `GeneratedBarcode` |
| `EncodingOptions { Width, Height }` | `.ResizeTo(width, height)` | Fluent sizing method |
| No PDF support | `BarcodeReader.Read("file.pdf")` | Native PDF support, all pages |

## Common Migration Issues and Solutions

### Issue 1: BarcodeReader Instantiation Patterns

**ZXing.Net:** Callers create a `new BarcodeReader()`, set `reader.Options.PossibleFormats`, load a `Bitmap`, call `reader.Decode(bitmap)` or `reader.DecodeMultiple(bitmap)`, and read `result.Text`.

**Solution:** Remove the instantiation entirely. Replace the full pattern with a static call:
```csharp
// Before: reader creation + format config + bitmap + decode + result.Text
// After:
var result = BarcodeReader.Read(imagePath).FirstOrDefault();
var value = result?.Value ?? string.Empty;
```

Search the codebase for `new BarcodeReader()` to find every instance. Each one is removed, not refactored.

### Issue 2: PossibleFormats Block Removal

**ZXing.Net:** Every decode call is preceded by a `reader.Options.PossibleFormats = new List<BarcodeFormat> { ... }` assignment. These lists are often long and project-specific.

**Solution:** Delete each `PossibleFormats` assignment block in its entirety. IronBarcode performs automatic format detection; no format list is accepted or required. If the `TryHarder = true` flag was set alongside the format list, replace it with `Speed = ReadingSpeed.Balanced` on a `BarcodeReaderOptions` object if accuracy tuning is needed.

### Issue 3: Binding Package Cleanup

**ZXing.Net:** Projects using `ZXing.Net.Bindings.Windows` have code referencing `System.Drawing.Bitmap` as the input type. Projects using `ZXing.Net.Bindings.ImageSharp` reference `SixLabors.ImageSharp.Image<Rgba32>`. Both patterns break after the binding packages are removed.

**Solution:** Remove both binding packages and replace all image-loading code. IronBarcode's `BarcodeReader.Read` accepts a file path string, a `byte[]`, a `Stream`, or a `Uri` — no `Bitmap` or `Image<T>` construction is needed. Remaining `using System.Drawing;` imports that existed solely for `Bitmap` loading can also be removed.

### Issue 4: result.Text to result.Value

**ZXing.Net:** The decoded barcode value is accessed via `result.Text`. The barcode format is accessed via `result.BarcodeFormat`.

**Solution:** Replace `result.Text` with `result.Value` and `result.BarcodeFormat` with `result.Format` throughout the codebase. Both changes are simple property renames with no behavioural difference.

## ZXing.Net Migration Checklist

### Pre-Migration Tasks

Audit the codebase to find all ZXing.Net references before making changes:

```bash
grep -r "using ZXing" --include="*.cs" .
grep -r "new BarcodeReader" --include="*.cs" .
grep -r "PossibleFormats" --include="*.cs" .
grep -r "BarcodeFormat\." --include="*.cs" .
grep -r "reader\.Decode\|reader\.DecodeMultiple" --include="*.cs" .
grep -r "result\.Text\|result\.BarcodeFormat" --include="*.cs" .
grep -r "new BarcodeWriter\|writer\.Write\|EncodingOptions" --include="*.cs" .
grep -r "PdfiumViewer\|PdfDocument\.Load" --include="*.cs" .
grep -r "using System\.Drawing" --include="*.cs" .
```

Document which files reference ZXing binding packages (Windows vs ImageSharp) and note where `System.Drawing.Bitmap` is used solely for ZXing input. Review the Dockerfile (if present) for `libgdiplus` installation.

### Code Update Tasks

1. Remove `ZXing.Net`, `ZXing.Net.Bindings.Windows`, `ZXing.Net.Bindings.ImageSharp`, and `PdfiumViewer` NuGet packages
2. Install the `IronBarcode` NuGet package
3. Add `IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";` to application startup
4. Replace all `using ZXing*` and `using SixLabors.ImageSharp*` imports with `using IronBarCode;`
5. Remove every `new BarcodeReader()` instantiation
6. Delete every `reader.Options.PossibleFormats = ...` block
7. Replace `reader.Decode(bitmap)` with `BarcodeReader.Read(imagePath).FirstOrDefault()`
8. Replace `reader.DecodeMultiple(bitmap)` with `BarcodeReader.Read(imagePath)`
9. Replace `result.Text` with `result.Value`
10. Replace `result.BarcodeFormat` with `result.Format`
11. Update `BarcodeFormat.X` enum references to `BarcodeEncoding.X` equivalents
12. Replace `new BarcodeWriter { Format = ..., Options = new EncodingOptions { ... } }` with `BarcodeWriter.CreateBarcode(data, encoding).ResizeTo(w, h)`
13. Replace `writer.Write(data)` + `bitmap.Save(...)` with `.SaveAsPng(path)` or `.ToPngBinaryData()`
14. Remove all `using System.Drawing;` imports that existed solely for ZXing.Net bitmap loading
15. Remove the PdfiumViewer page-render loop wherever it was implemented as a PDF workaround
16. Remove `libgdiplus` from Dockerfile if it was added to support `System.Drawing` on Linux

### Post-Migration Testing

- Verify that every barcode format the application was previously configured to detect is found successfully
- Test with images containing barcode formats that were previously excluded from the `PossibleFormats` list, to confirm auto-detection picks them up
- Run the concurrent processing paths under load and confirm no race conditions or null results
- If the application processed PDFs via PdfiumViewer, provide the same PDF inputs and confirm barcode values match
- Confirm that generated barcodes (Code 128, QR Code, etc.) produce the correct output format and dimensions
- On Linux or Docker deployments, confirm that image reading and PDF reading work without `libgdiplus`
- Run the full test suite and compare output to the pre-migration baseline

## Key Benefits of Migrating to IronBarcode

**Eliminated Thread Safety Burden:** IronBarcode's static API removes the per-thread instance pattern entirely. There is no reader to instantiate, no format list to configure per thread, and no `ThreadLocal` pool to manage. The same call is correct in both single-threaded and highly concurrent environments, and concurrency correctness is provided by the library rather than delegated to every caller.

**Automatic Format Detection:** Removing the `PossibleFormats` list eliminates the category of silent-miss failures that occur when documents contain barcode formats that were not anticipated during development. Applications that process documents from external partners, or that operate across evolving barcode standards, no longer require format list maintenance to remain reliable.

**Native PDF Processing:** The PdfiumViewer render loop — its page enumeration, DPI configuration, temp file lifecycle, and cleanup code — is removed entirely. Applications that previously required two libraries (ZXing.Net and a PDF renderer) to process a PDF barcode now use one. The reduction in deployed dependencies simplifies both the build and the deployment surface.

**Single Cross-Platform Package:** The binding package distinction between Windows and Linux deployments is eliminated. The same `BarcodeReader.Read(path)` call compiles and behaves identically on all target platforms, removing the need for platform conditionals in image-loading code and simplifying cross-platform development and containerisation workflows.

**Reduced Infrastructure Code:** The most significant outcome of the migration is the removal of surrounding infrastructure — the Bitmap loading blocks, the format lists, the PdfiumViewer render loops, the ThreadLocal pools — that existed in ZXing.Net integrations not to do barcode work but to satisfy the library's architectural requirements. What remains after the migration is the barcode logic itself. For additional context on detection rates and scanning scenarios, the [ZXing.Net vs IronBarcode scanner comparison](https://ironsoftware.com/csharp/barcode/blog/compare-to-other-components/zxing-barcode-scanner/) provides a broader analysis.
