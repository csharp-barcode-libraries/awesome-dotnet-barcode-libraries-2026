# Migrating from Infragistics Barcode Controls to IronBarcode

Most Infragistics barcode migrations happen because the team needed barcode reading in a project where only generation was available — WinForms, ASP.NET Core, or any project outside WPF — or because the event-driven WPF reader API proved too brittle for production use. The `TaskCompletionSource` bridge pattern, the shared `_result` field that makes concurrent calls unsafe, and the silent failures from missing `Symbology` flags on the `Decode()` call are all common reasons teams look for an alternative.

This guide walks through the full migration: removing Infragistics barcode packages, replacing generation and reading code in both WinForms and WPF contexts, and adding barcode functionality to project types where Infragistics had no support at all.

## Quick Start

### Step 1: Remove Infragistics Barcode References

Infragistics Ultimate is distributed via the platform installer or via Infragistics' private NuGet feed (`https://packages.infragistics.com/nuget/labs/`) — there is no `Infragistics.WPF.BarcodeReader` package on the public nuget.org feed. Remove the barcode-related assembly references from your `.csproj`:

```xml
<!-- Remove from .csproj -->
<Reference Include="InfragisticsWPF.Controls.Barcodes.BarcodeReader" />
<Reference Include="InfragisticsWPF.Controls.Barcodes.XamBarcode" />
<Reference Include="Infragistics.Win.UltraWinBarcode" />
```

If your project also pulled umbrella packages from the private feed, remove those `PackageReference` entries too. Your Infragistics Ultimate license is not affected by removing barcode-specific references if you have other Infragistics components in use.

### Step 2: Install IronBarcode

```bash
dotnet add package BarCode
```

IronBarcode is a single package that works across WinForms, WPF, ASP.NET Core, console, Blazor Server, Docker, Azure Functions, and AWS Lambda. You do not need separate packages per framework.

### Step 3: Update Namespaces

Replace Infragistics barcode namespaces with `IronBarCode` (capital B, capital C):

```csharp
// Remove these:
// using Infragistics.Win.UltraWinBarcode;
// using Infragistics.Controls.Barcodes;
// using System.Windows.Media.Imaging; // if only used for barcode BitmapImage loading

// Add this:
using IronBarCode;
```

### Step 4: Initialize the License

Add license initialization once at application startup — `Program.cs`, `App.xaml.cs`, or your DI composition root:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

## Code Migration Examples

### WinForms Generation: UltraWinBarcode to BarcodeWriter

The WinForms generation pattern maps cleanly to IronBarcode's `BarcodeWriter`:

**Before (Infragistics WinForms):**
```csharp
using Infragistics.Win.UltraWinBarcode;

var barcode = new UltraWinBarcode();
barcode.Symbology = Symbology.Code128;
barcode.Data = "ITEM-12345";
barcode.SaveTo(outputPath);
```

**After (IronBarcode):**
```csharp
// NuGet: dotnet add package BarCode
using IronBarCode;

BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
             .SaveAsPng(outputPath);
```

The Infragistics `Symbology` enum value moves to the `BarcodeEncoding` enum, which is a parameter to `CreateBarcode()` rather than a property assignment. The data becomes the first argument. `SaveTo()` becomes `.SaveAsPng()`, `.SaveAsJpeg()`, `.SaveAsSvg()`, or `.SaveAsPdf()` depending on your output format.

If you need to resize the barcode before saving:

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
             .ResizeTo(400, 100)
             .SaveAsPng(outputPath);
```

### WPF Event-Driven Reading: Service Class to Two Lines

This is the most significant reduction in the migration. The Infragistics WPF reading pattern requires a full service class:

**Before (Infragistics WPF — ~35 lines):**
```csharp
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

    var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

    // Symbology is the second argument to Decode(), not a property.
    // Note: EAN-8/EAN-13/UPC-A/UPC-E share the EanUpc flag; DataMatrix
    // is NOT supported by the WPF BarcodeReader.
    var symbologies = Symbology.Code128 | Symbology.Code39Ext |
                      Symbology.QRCode | Symbology.EanUpc |
                      Symbology.Interleaved2Of5;

    _reader.DecodeAsync(bitmap, symbologies);
    return await _result.Task;
}

private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
{
    _result?.TrySetResult(e.SymbolFound ? e.Value : "No barcode found");
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

**After (IronBarcode — 2 lines):**
```csharp
using IronBarCode;

var results = BarcodeReader.Read(imagePath);
return results.FirstOrDefault()?.Value ?? "No barcode found";
```

The entire service class — instance management, event wiring, `TaskCompletionSource`, `BitmapImage` loading, `Dispose()` — is gone. `BarcodeReader.Read()` is synchronous, static, and accepts a file path string directly.

If you were returning the symbology type as well:

```csharp
using IronBarCode;

var results = BarcodeReader.Read(imagePath);
var first = results.FirstOrDefault();
if (first != null)
{
    Console.WriteLine($"Value: {first.Value}, Type: {first.BarcodeType}");
}
```

`result.Value` replaces `e.Value` (the property name happens to match). `result.BarcodeType` replaces `e.Symbology`.

### Adding WinForms Reading (Previously Impossible)

If you were in a WinForms project that only had generation, reading is now available with the same package:

```csharp
using IronBarCode;

// In a WinForms button handler or service method — same API as WPF or ASP.NET Core
var results = BarcodeReader.Read(imagePath);
foreach (var result in results)
{
    MessageBox.Show($"Found: {result.Value} ({result.BarcodeType})");
}
```

No additional packages, no framework-specific setup. The same `BarcodeReader.Read()` call works in WinForms as in WPF.

### ASP.NET Core Endpoint (Previously Impossible)

Barcode generation and reading in an ASP.NET Core controller was not possible with Infragistics packages. With IronBarcode:

```csharp
using IronBarCode;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/barcode")]
public class BarcodeController : ControllerBase
{
    [HttpPost("read")]
    public IActionResult ReadBarcode(IFormFile imageFile)
    {
        using var stream = imageFile.OpenReadStream();
        var results = BarcodeReader.Read(stream);
        var values = results.Select(r => new { r.Value, BarcodeType = r.BarcodeType.ToString() });
        return Ok(values);
    }

    [HttpGet("generate")]
    public IActionResult GenerateBarcode([FromQuery] string data)
    {
        var bytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
                                 .ResizeTo(400, 100)
                                 .ToPngBinaryData();
        return File(bytes, "image/png");
    }
}
```

`BarcodeReader.Read()` accepts a `Stream` directly — no need to save the upload to disk first.

### PDF Barcode Reading

IronBarcode reads barcodes from PDF documents natively — no separate package required:

```csharp
using IronBarCode;

// Read all barcodes from every page of a PDF
var results = BarcodeReader.Read("document.pdf");
foreach (var result in results)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value} ({result.BarcodeType})");
}
```

Infragistics had no equivalent — PDF barcode reading required converting PDF pages to images externally before scanning.

### Batch Processing: Sequential to Parallel

**Before (Infragistics — sequential, not concurrency-safe for parallelization):**
```csharp
// Must process one at a time — shared _result field is not thread-safe
var service = new InfragisticsBarcodeService();
var results = new List<string>();

foreach (var file in imageFiles)
{
    var value = await service.ReadBarcodeAsync(file);
    results.Add(value);
}

service.Dispose();
```

**After (IronBarcode — parallel, thread-safe):**
```csharp
using IronBarCode;

// Option 1: pass an array directly — IronBarcode handles parallelization internally
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};

var results = BarcodeReader.Read(imageFiles.ToArray(), options);
foreach (var result in results)
{
    Console.WriteLine($"{result.Value} ({result.BarcodeType})");
}
```

```csharp
using IronBarCode;
using System.Collections.Concurrent;

// Option 2: Parallel.ForEach — static BarcodeReader.Read is thread-safe
var allValues = new ConcurrentBag<string>();

Parallel.ForEach(imageFiles, file =>
{
    var results = BarcodeReader.Read(file);
    foreach (var r in results)
        allValues.Add(r.Value);
});
```

### Removing the Dispose Pattern

If your code had `IDisposable` implementations around the Infragistics `BarcodeReader`, those can be removed entirely. IronBarcode's `BarcodeReader` is a static class. There is no instance to create, no event handler to detach, and no `Dispose()` to call.

```csharp
// Before: service registered as IDisposable in DI container
services.AddScoped<InfragisticsBarcodeService>(); // implements IDisposable

// After: no registration needed — use static method directly, or wrap in thin service if preferred
// services.AddScoped<IBarcodeService, IronBarcodeService>();
```

A thin wrapper service for DI purposes is straightforward:

```csharp
using IronBarCode;

public class BarcodeService
{
    public IEnumerable<string> Read(string path)
        => BarcodeReader.Read(path).Select(r => r.Value);

    public byte[] Generate(string data, BarcodeEncoding encoding)
        => BarcodeWriter.CreateBarcode(data, encoding).ToPngBinaryData();
}
```

No constructor injection, no `Dispose()`, no event management.

## Common Migration Issues

### Symbology Flags Removed

The `Symbology` parameter passed to `Decode()` on the Infragistics WPF reader required you to enumerate every barcode family you wanted to support (or pass `Symbology.All`). The `Symbology` enum is a `[Flags]` enum with values like `Code128`, `Code39Ext`, `EanUpc`, `QRCode`, `Interleaved2Of5`, `Linear`, `All`. Note that EAN-8/EAN-13/UPC-A/UPC-E share a single `EanUpc` flag, and DataMatrix is not in the enum at all (the WPF reader does not support DataMatrix). IronBarcode has no equivalent — it auto-detects every supported format on every read.

This has an important side effect: if a particular barcode format was silently failing before because its `Symbology` flag was missing from the `Decode()` argument, IronBarcode will now find it. You may see barcode values appearing in production that were previously being swallowed silently. This is the correct behavior — those barcodes were always there.

If you need to constrain which formats are returned (for performance or ambiguity reasons), `BarcodeReaderOptions` supports this:

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = false
};

var results = BarcodeReader.Read(imagePath, options);
```

### BitmapImage Loading Not Needed

The Infragistics WPF reader required loading images as `BitmapSource` objects via `new BitmapImage(new Uri(path, UriKind.Absolute))`. IronBarcode accepts file path strings, byte arrays, and streams directly:

```csharp
// All of these work — no BitmapImage conversion needed
BarcodeReader.Read("path/to/image.png");
BarcodeReader.Read(File.ReadAllBytes("path/to/image.png"));
BarcodeReader.Read(File.OpenRead("path/to/image.png"));
```

You can remove any `using System.Windows.Media.Imaging;` imports that were only there for barcode-related image loading.

### TaskCompletionSource Pattern Removed

The `TaskCompletionSource<string>` field that bridged the event-driven `DecodeComplete` callback into async code is gone entirely. `BarcodeReader.Read()` is synchronous and returns its results directly.

If your calling code was `await service.ReadBarcodeAsync(path)`, you can simplify to:

```csharp
// Before
var value = await service.ReadBarcodeAsync(imagePath);

// After
var value = BarcodeReader.Read(imagePath).FirstOrDefault()?.Value ?? "No barcode found";
```

If you want to preserve an async interface for downstream callers without blocking a thread:

```csharp
public Task<string> ReadBarcodeAsync(string imagePath)
{
    return Task.Run(() =>
    {
        var results = BarcodeReader.Read(imagePath);
        return results.FirstOrDefault()?.Value ?? "No barcode found";
    });
}
```

### Property Name Changes

| Infragistics | IronBarcode |
|---|---|
| `e.Value` (in `ReaderDecodeArgs`) | `result.Value` (same property name) |
| `e.Symbology` (in `ReaderDecodeArgs`) | `result.BarcodeType` |
| `e.SymbolFound` (bool) | Check `results.Any()` instead |
| `barcode.Data` | First argument of `BarcodeWriter.CreateBarcode()` |
| `barcode.Symbology = Symbology.Code128` | `BarcodeEncoding.Code128` (parameter) |
| `barcode.SaveTo(path)` | `.SaveAsPng(path)` / `.SaveAsJpeg(path)` / `.SaveAsSvg(path)` |

## Migration Checklist

Use these grep patterns to find all Infragistics barcode usage in your codebase:

```bash
# Namespace references
grep -r "Infragistics.Win.UltraWinBarcode" --include="*.cs" .
grep -r "Infragistics.Controls.Barcodes" --include="*.cs" .
grep -r "Infragistics.WPF.BarcodeReader" --include="*.csproj" .

# WinForms generation types
grep -r "UltraWinBarcode" --include="*.cs" .
grep -r "Symbology\.Code" --include="*.cs" .
grep -r "barcode\.SaveTo(" --include="*.cs" .

# WPF reading types and patterns
grep -r "new BarcodeReader()" --include="*.cs" .
grep -r "DecodeComplete" --include="*.cs" .
grep -r "ReaderDecodeArgs" --include="*.cs" .
grep -r "Symbology\." --include="*.cs" .
grep -r "ReaderDecodeArgs" --include="*.cs" .
grep -r "BitmapImage" --include="*.cs" .

# TaskCompletionSource usage (likely barcode-related if near above patterns)
grep -r "TaskCompletionSource" --include="*.cs" .
```

Work through each match:
- `UltraWinBarcode` usages: replace with `BarcodeWriter.CreateBarcode()`
- `new BarcodeReader()` + `DecodeComplete` blocks: replace with `BarcodeReader.Read()`
- `Symbology` flag lists passed to `Decode()`: delete entirely (auto-detection replaces them)
- `BitmapImage` loads used for barcode input: replace with direct path/stream/bytes
- `ReaderDecodeArgs e` callback bodies: extract `e.Value` → `result.Value`, `e.Symbology` → `result.BarcodeType`, drop `e.SymbolFound` checks (use `results.Any()` instead)
- `IDisposable` on barcode service classes: remove if the only disposable resource was `_reader`

## Feature Comparison

| Feature | Infragistics Barcode | IronBarcode |
|---------|---------------------|-------------|
| WinForms barcode generation | Yes (UltraWinBarcode) | Yes |
| WinForms barcode reading | Not available | Yes |
| WPF barcode generation | Yes (XamBarcode) | Yes |
| WPF barcode reading | Yes (event-driven) | Yes (synchronous) |
| ASP.NET Core | Not available | Yes |
| Console / Worker Service | Not available | Yes |
| Docker / Linux | Not available | Yes |
| Azure Functions | Not available | Yes |
| Blazor Server | Not available | Yes |
| Auto format detection | Partial — pass `Symbology.All` to `Decode()` | Yes (always on) |
| PDF barcode reading | Not available | Yes (native) |
| Thread-safe concurrent reads | No | Yes |
| Batch reading (built-in) | No | Yes |
| QR error correction control | Not available | Yes |
| Static API (no instance) | No | Yes |
| Suite subscription required | Yes (Infragistics Ultimate, ~$2,300+/year) | No |
| Perpetual license | No | Yes — from $799 (Lite) |

## Key Benefits After Migration

**Barcode reading in WinForms.** If the migration was driven by a WinForms reading requirement, this is the primary outcome: `BarcodeReader.Read(imagePath)` compiles and runs in WinForms with no additional setup.

**ASP.NET Core and console support.** Web APIs, background services, and console tools can now both generate and read barcodes using the same package. If you were maintaining separate tool chains or workarounds for these project types, they are gone.

**No event handler boilerplate.** The `DecodeComplete` handler, the `TaskCompletionSource` bridge, the `BitmapImage` load, and the `Dispose()` cleanup are all removed. The barcode logic is what remains.

**No symbology specification.** Formats that were silently failing due to missing `Symbology` flags on the `Decode()` call will now be detected correctly. Auto-detection covers every supported format without configuration.

**Same service class across all project types.** A single `BarcodeService` wrapper using `BarcodeReader.Read()` and `BarcodeWriter.CreateBarcode()` can be shared across WinForms, WPF, ASP.NET Core, and any other project in the solution. No framework-specific implementations.

**No Infragistics Ultimate subscription required for barcode functionality alone.** If barcode support was the only reason your project had an Infragistics Ultimate subscription, or if it was a significant factor in the decision, IronBarcode is a standalone package with perpetual licensing options starting at $799 (Lite) for a single developer.

The migration itself is mechanical once you have located all the Infragistics barcode usage. The most involved part is the WPF service class replacement, and even that reduces to removing the class and replacing calls with `BarcodeReader.Read()`. The generation migration is a one-to-one property/method rename. What you get back is a single, consistent API that works in every project type you need.
