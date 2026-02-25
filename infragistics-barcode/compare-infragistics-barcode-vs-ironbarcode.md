Infragistics barcode reading works in WPF. Getting there requires a `BarcodeReader` instance, a `DecodeComplete` event handler, a `TaskCompletionSource<string>` to bridge the callback into async code, a `BitmapImage` load from a URI, and an explicit bitwise OR of every `SymbologyType` you want to support. Miss one flag — say, `SymbologyType.EAN8` — and any EAN-8 barcode in your images silently returns nothing. No exception. No warning. Just an empty result.

That is the WPF side. On the WinForms side, the `Infragistics.Win.UltraWinBarcode` package contains generation controls but no reader class at all. If you need to read barcodes in a WinForms project, there is nothing in the Infragistics barcode package to call. The same applies to any ASP.NET Core controller, console tool, Azure Function, Blazor Server component, or Docker container. Infragistics barcode support exists inside UI framework boundaries: WPF gets generation and event-driven reading; WinForms gets generation only; everything else gets nothing.

This comparison examines what that split means in practice, then looks at how IronBarcode handles the same work with a single static API that behaves identically across every project type.

## Understanding Infragistics Barcode Support

Infragistics is one of the most established .NET UI component vendors. The Infragistics Ultimate suite — the subscription that covers barcode functionality — includes hundreds of controls for WinForms, WPF, ASP.NET, Blazor, and mobile. For teams already using Infragistics grids, charts, or schedulers, the barcode controls are a logical addition: they're already paying for the subscription.

The barcode support, however, is not a unified library. It is two separate assemblies with separate capabilities that only partially overlap.

### WinForms: UltraWinBarcode

The `Infragistics.Win.UltraWinBarcode` package provides barcode generation in WinForms applications through the `UltraWinBarcode` class. The API is straightforward:

```csharp
// Infragistics WinForms generation
using Infragistics.Win.UltraWinBarcode;

var barcode = new UltraWinBarcode();
barcode.Symbology = Symbology.Code128;
barcode.Data = "ITEM-12345";
barcode.SaveTo(outputPath);
```

You set a symbology, assign data, call `SaveTo()`. For generation-only scenarios in WinForms, this works. The `Symbology` enum covers common formats: Code128, Code39, QR, EAN13, and others.

What does not exist in this assembly is a reader. There is no `UltraBarcodeReader` class. There is no `Scan()` method. If you try to read a barcode image in a WinForms application using only the `Infragistics.Win.UltraWinBarcode` package, there is nothing to call.

### WPF: XamBarcode and BarcodeReader

The WPF side includes both a generation control (`XamBarcode`) and a separate reader class (`BarcodeReader` in `Infragistics.Controls.Barcodes`, from the `Infragistics.WPF.BarcodeReader` assembly). The reader is event-driven, designed around the WPF threading and imaging model.

Reading a barcode in WPF requires wiring up the `DecodeComplete` event, loading images as `BitmapSource` objects rather than file paths, and converting the callback pattern into something awaitable if your code is async.

### ASP.NET Core, Console, Docker: Nothing

There is no Infragistics barcode package that targets `net8.0` without WPF or WinForms UI assemblies. ASP.NET Core projects, console tools, Azure Functions, Blazor Server, and Linux Docker containers have no Infragistics barcode option. The library is coupled to the UI framework.

## The WPF Reading Pattern

Here is what reading a barcode actually looks like in WPF with Infragistics:

```csharp
// Infragistics WPF reading: event-driven, requires WPF assemblies
using Infragistics.Controls.Barcodes;
using System.Windows.Media.Imaging;

private BarcodeReader _reader;
private TaskCompletionSource<string> _result;

public InfragisticsBarcodeService()
{
    _reader = new BarcodeReader();
    _reader.DecodeComplete += OnDecodeComplete;
}

public async Task<string> ReadBarcodeAsync(string imagePath)
{
    _result = new TaskCompletionSource<string>();

    // Load as WPF BitmapSource — not a file path
    var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

    // Must specify EVERY format — no auto-detection
    _reader.SymbologyTypes = SymbologyType.Code128 |
                             SymbologyType.Code39 |
                             SymbologyType.QR |
                             SymbologyType.EAN8 |
                             SymbologyType.EAN13 |
                             SymbologyType.UPCA |
                             SymbologyType.DataMatrix |
                             SymbologyType.Interleaved2of5;

    // Trigger decode — result comes via callback
    _reader.Decode(bitmap);

    return await _result.Task;
}

private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
{
    _result?.TrySetResult(e.SymbologyValue ?? "No barcode found");
}

public void Dispose()
{
    if (_reader != null)
    {
        _reader.DecodeComplete -= OnDecodeComplete;
        _reader = null;
    }
}
```

This is roughly 35 lines of infrastructure to read one barcode. Count what is actually happening:

1. A `BarcodeReader` instance is created and kept alive as a field.
2. An event handler is wired in the constructor.
3. Every call to `ReadBarcodeAsync` creates a new `TaskCompletionSource<string>` assigned to a shared field — which means this service is not thread-safe as written. Concurrent calls would overwrite `_result`.
4. The image must be loaded as a `BitmapImage` from a `Uri` — not a file path string, not a byte array, not a stream.
5. `Decode()` fires the event asynchronously. The `TaskCompletionSource` is the glue between the callback world and the async/await world.
6. The callback extracts `e.SymbologyValue`.
7. The `Dispose()` method must detach the event handler to prevent memory leaks.

None of this logic is about barcodes. It is infrastructure required to work around the event-driven design. In production code, you'd also need to handle the case where `_result.Task` never completes — a timeout, a cancellation token, or some guard against the event never firing.

## The WinForms Gap

The WinForms gap is more abrupt than it might initially appear. Teams building WinForms applications often arrive at the Infragistics barcode page expecting a symmetrical experience — generation and reading on both UI frameworks. What they find is that `Infragistics.Win.UltraWinBarcode` provides no reading capability whatsoever.

This is not a documentation oversight. The WinForms barcode assembly was designed as a generation control. If you need to scan barcodes in a WinForms application — say, reading a barcode from an image file a user uploads, or decoding a barcode from a camera feed — you cannot do it with Infragistics barcode tools. You would need to bring in a separate library entirely, at which point the rationale for using Infragistics for generation weakens.

The asymmetry creates a genuinely awkward situation for teams running mixed-framework projects. A team with both a WPF desktop client and a WinForms desktop client cannot use Infragistics for barcode reading in the WinForms project, even though they are using Infragistics everywhere else.

## Symbology Specification: A Silent Failure Mode

The `SymbologyTypes` flag property in the WPF reader deserves its own section because its failure mode is subtle and dangerous in production.

When you configure the reader, you must explicitly OR together every barcode format you want to support:

```csharp
// Must enumerate every format — miss one, that format silently returns empty
_reader.SymbologyTypes = SymbologyType.Code128 |
                         SymbologyType.Code39 |
                         SymbologyType.QR |
                         SymbologyType.EAN8 |
                         SymbologyType.EAN13 |
                         SymbologyType.UPCA |
                         SymbologyType.DataMatrix |
                         SymbologyType.Interleaved2of5;
```

If a barcode image contains an EAN-8 barcode and `SymbologyType.EAN8` is not in the flags, `e.SymbologyValue` comes back null or empty. The decode event still fires. No exception is thrown. The caller receives "No barcode found" and has no indication whether the image was unreadable or simply not configured.

In practice, this means:
- Initial setup works fine for formats the developer tested.
- A new barcode format enters the system (a supplier changes label type, a new product line uses a different symbology).
- The reader silently fails for all images with that format.
- The failure looks identical to "image has no barcode" rather than "format not configured."

Teams debugging this spend time examining image quality before realizing the format was never in the flags list.

IronBarcode has no `SymbologyTypes` property. It auto-detects all 50+ supported formats on every read. There is no flag to forget.

## The Platform Matrix

The capability gap across platforms is the clearest way to understand the architectural constraint:

| Platform | Infragistics Generation | Infragistics Reading | IronBarcode Generation | IronBarcode Reading |
|----------|------------------------|---------------------|----------------------|-------------------|
| WPF | XamBarcode control | BarcodeReader (event-driven) | Yes | Yes |
| WinForms | UltraWinBarcode | NOT AVAILABLE | Yes | Yes |
| ASP.NET Core | NOT AVAILABLE | NOT AVAILABLE | Yes | Yes |
| Console | NOT AVAILABLE | NOT AVAILABLE | Yes | Yes |
| Blazor Server | NOT AVAILABLE | NOT AVAILABLE | Yes | Yes |
| Docker / Linux | NOT AVAILABLE | NOT AVAILABLE | Yes | Yes |
| Azure Functions | NOT AVAILABLE | NOT AVAILABLE | Yes | Yes |

This table illustrates why teams running anything beyond pure WPF desktop applications find Infragistics barcode support insufficient. The moment a project spans WinForms and ASP.NET Core — or WPF and a background worker service — the Infragistics barcode library covers only part of the codebase.

## Understanding IronBarcode

IronBarcode is a dedicated barcode library for .NET with no dependency on WinForms, WPF, or any UI framework. The same NuGet package, the same namespace, and the same API work in any .NET project: WinForms, WPF, ASP.NET Core, console, Blazor Server, Docker, Azure Functions, AWS Lambda.

```csharp
// IronBarcode: identical code in WinForms, WPF, ASP.NET Core, console, Docker
// NuGet: dotnet add package IronBarcode
using IronBarCode;

// Read — 2 lines, any platform
var results = BarcodeReader.Read(imagePath);
return results.FirstOrDefault()?.Value ?? "No barcode found";
```

`BarcodeReader.Read()` is a static method. No instance to manage, no event to wire, no `TaskCompletionSource` to bridge callback patterns. It accepts a file path string, a byte array, a `Stream`, or an array of any of those for batch processing.

For generation, `BarcodeWriter.CreateBarcode()` returns a barcode object you can save to PNG, JPEG, SVG, or get as binary data:

```csharp
using IronBarCode;

// Generate Code 128
BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
             .ResizeTo(400, 100)
             .SaveAsPng("barcode.png");

// Generate QR code
QRCodeWriter.CreateQrCode("https://example.com", 500,
    QRCodeWriter.QrErrorCorrectionLevel.Highest)
    .SaveAsPng("qr.png");
```

The license initialization goes at application startup, once:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

## Side-by-Side: Batch Processing

Batch processing exposes another structural limitation in the Infragistics WPF reader. Because the reader uses a shared event handler and the `_result` field is overwritten on every call, the service class shown above cannot safely process multiple images concurrently. You must sequence calls:

```csharp
// Infragistics: must process sequentially — shared event handler and TaskCompletionSource
// are not thread-safe; concurrent calls would corrupt _result
var service = new InfragisticsBarcodeService();
var results = new List<string>();

foreach (var file in imageFiles)
{
    // Each call must await before starting the next
    var value = await service.ReadBarcodeAsync(file);
    results.Add(value);
}
```

Making this concurrent requires significant additional infrastructure: a lock, a queue, or a semaphore to ensure `_result` is not overwritten while a previous decode is still in flight. That is a non-trivial concurrency problem for what should be a simple I/O operation.

IronBarcode's static `BarcodeReader.Read()` is thread-safe. It can be called from multiple threads simultaneously without any additional synchronization. For batch workloads, you can use `Parallel.ForEach` directly:

```csharp
using IronBarCode;

// IronBarcode: parallel batch with thread-safe static API
var results = new System.Collections.Concurrent.ConcurrentBag<string>();

Parallel.ForEach(imageFiles, file =>
{
    var barcodeResults = BarcodeReader.Read(file);
    foreach (var result in barcodeResults)
    {
        results.Add(result.Value);
    }
});
```

You can also pass multiple files in a single call and configure parallelism through `BarcodeReaderOptions`:

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};

var results = BarcodeReader.Read(imageFiles, options);
foreach (var result in results)
{
    Console.WriteLine($"{result.Value} ({result.Format})");
}
```

## Feature Comparison

| Feature | Infragistics Barcode | IronBarcode |
|---------|---------------------|-------------|
| WinForms barcode reading | Not available | Yes |
| WPF barcode reading | Yes (event-driven) | Yes (synchronous) |
| ASP.NET Core support | Not available | Yes |
| Console / Worker Service | Not available | Yes |
| Docker / Linux | Not available | Yes |
| Azure Functions | Not available | Yes |
| Blazor Server | Not available | Yes |
| Auto format detection | No — must specify every SymbologyType flag | Yes — all 50+ formats auto-detected |
| PDF barcode reading | Not available | Yes — native, no extra package |
| Thread-safe reading | No (shared event handler) | Yes (static API) |
| Event-driven API required | Yes (WPF) | No |
| Explicit image load (BitmapSource) | Yes | No — accepts file path, bytes, stream |
| Synchronous reading | No (must bridge via TaskCompletionSource) | Yes |
| Batch processing | Sequential only (concurrency unsafe) | Built-in parallelization |
| Silent format failures | Yes (missing SymbologyType flag) | No |
| Suite dependency required | Yes — Infragistics Ultimate subscription | No — standalone package |
| Perpetual license option | No — annual subscription | Yes |
| Approximate license cost | $1,675+/year (Infragistics Ultimate) | From $749 perpetual (Lite) |

## API Mapping Reference

### WinForms (UltraWinBarcode) to IronBarcode

| Infragistics WinForms — UltraWinBarcode | IronBarcode |
|-----------------------------------------|-------------|
| `new UltraWinBarcode()` | `BarcodeWriter.CreateBarcode(data, encoding)` |
| `barcode.Symbology = Symbology.Code128` | `BarcodeEncoding.Code128` (parameter to `CreateBarcode`) |
| `barcode.Data = "ITEM-12345"` | First argument of `CreateBarcode()` |
| `barcode.SaveTo(outputPath)` | `.SaveAsPng(outputPath)` |
| No reading API exists | `BarcodeReader.Read(imagePath)` |

### WPF (BarcodeReader) to IronBarcode

| Infragistics WPF — BarcodeReader | IronBarcode |
|----------------------------------|-------------|
| `new BarcodeReader()` | Static class — no instance needed |
| `_reader.DecodeComplete += OnDecodeComplete` | Not needed |
| `_reader.SymbologyTypes = SymbologyType.X \| SymbologyType.Y \| ...` | Auto-detection — no configuration |
| `new BitmapImage(new Uri(path))` + `_reader.Decode(bitmap)` | `BarcodeReader.Read(path)` |
| `e.SymbologyValue` (in callback) | `result.Value` |
| `e.Symbology` (in callback) | `result.Format` |
| `TaskCompletionSource<string>` async wrapper | Synchronous — no wrapper needed |
| `Dispose()` — detach event handler | Not needed — no instance or event |
| WPF project only | Any .NET project type |

## When Teams Switch

Several specific situations consistently drive teams away from Infragistics barcode support.

**Reading needed in WinForms.** This is the most common scenario. A WinForms application generates barcodes fine with `UltraWinBarcode` but then a new requirement arrives: scan a barcode from an uploaded image or validate a label before printing. There is no Infragistics reading API for WinForms. The team either pulls in a second library or replaces the generation code with something that does both.

**New ASP.NET Core endpoint.** A desktop application with Infragistics barcode generation gets a companion web API. The endpoint needs to accept image uploads and return barcode values, or generate barcode images on demand. Neither is possible with Infragistics barcode packages in an ASP.NET Core project. IronBarcode installs with `dotnet add package IronBarcode` and works in a controller action the same way it works in a console method.

**Docker deployment.** A WPF application is being containerized or its barcode logic is being extracted into a microservice. WPF assemblies do not run in Linux Docker containers. The Infragistics WPF `BarcodeReader` goes with them. IronBarcode targets Linux x64 natively.

**Batch processing performance.** A workflow processes hundreds or thousands of barcode images. The event-driven Infragistics reader processes them sequentially. IronBarcode's static reader is thread-safe and supports `Parallel.ForEach` or its built-in `MaxParallelThreads` option without any concurrency infrastructure.

**Silent format failures in production.** A team discovers that barcodes of a certain format have been silently failing for weeks because the `SymbologyTypes` flags did not include that format. Switching to auto-detection eliminates the failure mode entirely.

**Reducing Infragistics subscription scope.** Some teams are paying the Infragistics Ultimate subscription price specifically because the barcode controls are part of it. When the barcode requirement is the only reason for the subscription, a dedicated barcode library at a fraction of the cost is worth evaluating.

## Conclusion

The central issue with Infragistics barcode support is architectural rather than capability-related. The WPF `BarcodeReader` does read barcodes. The WinForms `UltraWinBarcode` does generate them. Within the narrow context each component was designed for, they function. The problem is that those two contexts do not cover what most .NET teams actually need.

A barcode feature in a modern .NET application rarely lives in a single UI framework. It appears in a WinForms client and a web API. It runs in a Docker container and a desktop. It needs to scan images uploaded to an ASP.NET endpoint and print labels from a console tool. None of that works with Infragistics barcode packages, and the WPF reader's event-driven pattern with required symbology flags adds significant complexity even in the one context where it does work.

IronBarcode solves the same problem — reading and generating barcodes — with a static API that compiles and runs identically in every .NET project type. The `BarcodeReader.Read()` call you write in a WPF service class is the same call you write in an ASP.NET Core controller and the same call you write in a Linux Docker container. No events, no flags, no `TaskCompletionSource`. The barcode logic is two lines rather than thirty-five, and those two lines work everywhere.
