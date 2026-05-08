# Porting Barcode Logic from Google ML Kit to IronBarcode in .NET

This guide is for teams in one of two situations: you are porting an Android application to .NET MAUI or .NET 9 and need to replace ML Kit's barcode scanner with a managed alternative, or you were recommended Google ML Kit in a cross-platform barcode discussion and discovered — when you went to add the NuGet package — that no first-party .NET package exists.

Google ML Kit Barcode Scanning is a native Android and iOS library. It ships as a Maven dependency (`com.google.mlkit:barcode-scanning:17.3.0`, or the unbundled `com.google.android.gms:play-services-mlkit-barcode-scanning:18.3.1`) for Kotlin/Java, and as a CocoaPod (`GoogleMLKit/BarcodeScanning`) for Swift. Google does not publish a .NET SDK. Community-maintained Xamarin and MAUI bindings have appeared over the years (`Xamarin.GooglePlayServices.MLKit.BarcodeScanning`, `BarcodeScanning.Native.Maui`, and others), but they trail upstream releases and only work on Android and iOS.

IronBarcode is a native .NET library that installs from NuGet, integrates with standard .NET patterns, and runs on Windows, Linux, macOS, Docker, Azure, and AWS. This guide shows how to translate the patterns you wrote in Kotlin or Java into equivalent C# code.

## The Porting Context

When porting from ML Kit to IronBarcode, a few things change structurally — not just syntactically:

**Callbacks become return values.** ML Kit uses Android's `Task` API with `addOnSuccessListener` and `addOnFailureListener`. IronBarcode's `BarcodeReader.Read()` returns a collection synchronously. You iterate it directly. No callback registration, no thread coordination.

**No scanner object.** ML Kit requires you to build a `BarcodeScannerOptions` object, call `BarcodeScanning.getClient(options)` to get a scanner instance, then call `scanner.process(inputImage)`. IronBarcode uses static methods — `BarcodeReader.Read()` is the entry point. There is no instance to manage or dispose.

**No InputImage construction.** ML Kit's `InputImage` must be constructed from an Android-specific source: `InputImage.fromFilePath(context, uri)`, `InputImage.fromBitmap(bitmap, rotation)`, or `InputImage.fromMediaImage(image, rotation)`. IronBarcode accepts a file path string, a `Stream`, a `byte[]`, or a `System.Drawing.Bitmap`. No Android context, no URI, no rotation metadata.

**No Google Play Services constraint.** The unbundled ML Kit model runs through Google Play Services. Devices without Play Services cannot use it; the bundled model works without Play Services but adds about 2.4 MB to your APK. IronBarcode has no such dependency — it runs identically on any platform .NET supports.

## Quick Setup in .NET

Remove any Xamarin/MAUI ML Kit binding packages if they exist in your project, then install IronBarcode:

```bash
dotnet add package BarCode
```

Add the license key at application startup — in `Program.cs`, `MauiProgram.cs`, or `Startup.cs` depending on your app type:

```csharp
// NuGet: dotnet add package BarCode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

The license can be set at any point before the first `BarcodeReader.Read()` or `BarcodeWriter.CreateBarcode()` call. A free trial is available; trial mode watermarks generated barcodes but does not restrict reading.

## Reading Barcodes: Kotlin to C#

### Basic Single-Barcode Read

Here is a typical ML Kit read in Kotlin, scanning a single QR code from a file URI:

```kotlin
// Android Kotlin — ML Kit
val options = BarcodeScannerOptions.Builder()
    .setBarcodeFormats(Barcode.FORMAT_QR_CODE)
    .build()
val scanner = BarcodeScanning.getClient(options)
val inputImage = InputImage.fromFilePath(context, imageUri)

scanner.process(inputImage)
    .addOnSuccessListener { barcodes ->
        val barcode = barcodes.firstOrNull()
        if (barcode != null) {
            Log.d("MLKit", "Value: ${barcode.rawValue}")
            Log.d("MLKit", "Format: ${barcode.format}")
        }
    }
    .addOnFailureListener { e ->
        Log.e("MLKit", "Scan failed: ${e.message}")
    }
```

The equivalent in C# with IronBarcode:

```csharp
using IronBarCode;

try
{
    var results = BarcodeReader.Read("captured-image.jpg");
    var barcode = results.FirstOrDefault();
    if (barcode != null)
    {
        Console.WriteLine($"Value: {barcode.Value}");
        Console.WriteLine($"Type: {barcode.BarcodeType}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Scan failed: {ex.Message}");
}
```

The result is available immediately as a return value. `barcode.Value` corresponds to `barcode.rawValue`. `barcode.BarcodeType` corresponds to `barcode.format`. Error handling uses standard try/catch instead of a separate failure listener.

### Multi-Barcode Read

ML Kit scans a single `InputImage` and returns a list. For multiple barcodes in one image, you iterate the success listener's list:

```kotlin
// Android Kotlin — ML Kit, multiple barcodes
val options = BarcodeScannerOptions.Builder()
    .setBarcodeFormats(Barcode.FORMAT_ALL_FORMATS)
    .build()
val scanner = BarcodeScanning.getClient(options)
val inputImage = InputImage.fromFilePath(context, imageUri)

scanner.process(inputImage)
    .addOnSuccessListener { barcodes ->
        for (barcode in barcodes) {
            val rawValue = barcode.rawValue
            val format = barcode.format
            processBarcode(rawValue, format)
        }
    }
    .addOnFailureListener { e -> Log.e("MLKit", e.message ?: "Unknown error") }
```

With IronBarcode, set `ExpectMultipleBarcodes = true` in `BarcodeReaderOptions` and iterate the result collection:

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};

var results = BarcodeReader.Read("warehouse-shelf.jpg", options);
foreach (var barcode in results)
{
    Console.WriteLine($"Type: {barcode.BarcodeType}, Value: {barcode.Value}");
    ProcessBarcode(barcode.Value, barcode.BarcodeType);
}
```

## Format Specification: setBarcodeFormats to BarcodeReaderOptions

ML Kit requires you to specify which formats to look for via `setBarcodeFormats()`. If you omit it, ML Kit searches all formats. IronBarcode works the same way — omitting format constraints searches everything, but specifying expected types improves performance.

| ML Kit Kotlin | IronBarcode C# |
|---|---|
| `Barcode.FORMAT_QR_CODE` | `BarcodeEncoding.QRCode` |
| `Barcode.FORMAT_CODE_128` | `BarcodeEncoding.Code128` |
| `Barcode.FORMAT_CODE_39` | `BarcodeEncoding.Code39` |
| `Barcode.FORMAT_CODE_93` | `BarcodeEncoding.Code93` |
| `Barcode.FORMAT_EAN_13` | `BarcodeEncoding.EAN13` |
| `Barcode.FORMAT_EAN_8` | `BarcodeEncoding.EAN8` |
| `Barcode.FORMAT_UPC_A` | `BarcodeEncoding.UPCA` |
| `Barcode.FORMAT_UPC_E` | `BarcodeEncoding.UPCE` |
| `Barcode.FORMAT_PDF417` | `BarcodeEncoding.PDF417` |
| `Barcode.FORMAT_DATA_MATRIX` | `BarcodeEncoding.DataMatrix` |
| `Barcode.FORMAT_AZTEC` | `BarcodeEncoding.Aztec` |
| `Barcode.FORMAT_ITF` | `BarcodeEncoding.ITF` |
| `Barcode.FORMAT_CODABAR` | `BarcodeEncoding.Codabar` |
| `Barcode.FORMAT_ALL_FORMATS` | Omit `ExpectedBarcodeTypes` |

Using format flags in IronBarcode:

```csharp
using IronBarCode;

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    ExpectedBarcodeTypes = BarcodeEncoding.QRCode | BarcodeEncoding.Code128 | BarcodeEncoding.EAN13
};

var results = BarcodeReader.Read("product-image.jpg", options);
```

The bitwise OR combination works the same way as ML Kit's vararg format list.

## Result Access: rawValue and format

ML Kit's result object exposes `rawValue` (a `String?`) and `format` (an `Int` constant). IronBarcode's result exposes `Value` (a `string`) and `BarcodeType` (a `BarcodeEncoding` enum value).

```kotlin
// ML Kit Kotlin — result fields
val rawValue: String? = barcode.rawValue
val format: Int = barcode.format
val boundingBox: Rect? = barcode.boundingBox
val displayValue: String? = barcode.displayValue
```

```csharp
// IronBarcode C# — result fields
string value = barcode.Value;
BarcodeEncoding type = barcode.BarcodeType;
System.Drawing.Bitmap barcodeImage = barcode.BarcodeImage;
```

`barcode.Value` is always a non-null string in IronBarcode — if the read succeeded, the value is present. `barcode.BarcodeType` is the `BarcodeEncoding` enum member, which you can compare directly: `if (barcode.BarcodeType == BarcodeEncoding.QRCode)`.

## What's Different in .NET

**Synchronous API instead of callbacks.** This is the most significant structural change. ML Kit's `scanner.process()` returns a `Task<List<Barcode>>` in Android's sense — you chain listeners. IronBarcode's `BarcodeReader.Read()` returns the result inline. If you need to run it off the UI thread in a MAUI app, wrap it in `Task.Run()`:

```csharp
using IronBarCode;

// In a MAUI ViewModel or page code-behind
var results = await Task.Run(() => BarcodeReader.Read(imagePath));
foreach (var barcode in results)
{
    // update UI on main thread
    MainThread.BeginInvokeOnMainThread(() =>
    {
        ResultLabel.Text = barcode.Value;
    });
}
```

**No context parameter.** Every ML Kit call that constructs an `InputImage` requires an Android `Context`. IronBarcode needs only a file path or stream. Removing context threading from barcode logic simplifies the code considerably.

**No Google Play Services constraint.** The unbundled ML Kit model runs through Play Services — `BarcodeScanning.getClient()` checks Play Services availability at runtime and throws if unavailable. IronBarcode has no runtime service check. It either reads the image or throws a standard exception.

**Standard exception handling.** ML Kit's `addOnFailureListener` receives a Java `Exception` subclass. In .NET, failures surface as standard `System.Exception` throws, catchable with try/catch in the normal way.

## Reading from PDF Documents

ML Kit has no PDF support. `InputImage.fromFilePath()` with a `.pdf` URI either fails or reads only the first page as a rasterized image, depending on the Android version. If your porting scenario involves documents — invoice processing, logistics manifests, form scanning — IronBarcode handles PDFs natively:

```csharp
using IronBarCode;

// Read all barcodes from all pages of a PDF
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};

var results = BarcodeReader.Read("invoice-batch.pdf", options);
foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.BarcodeType} — {barcode.Value}");
}
```

No image extraction step, no third-party PDF library, no page-iteration loop with separate rendering. Pass the PDF path, get all barcode values back with their page numbers.

## New Capabilities: Generation

ML Kit does not generate barcodes — it only reads them. If your ported application needs to produce labels, tickets, or QR codes, IronBarcode covers that with the same package.

**Code 128 for shipping labels:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("SHIP-2024-98341", BarcodeEncoding.Code128)
    .ResizeTo(400, 120)
    .SaveAsPng("shipping-label.png");
```

**QR code generation:**

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com/track/98341", 500)
    .SaveAsPng("tracking-qr.png");
```

**QR code with logo and color:**

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com/product/4821", 500)
    .AddBrandLogo("company-logo.png")
    .ChangeBarCodeColor(System.Drawing.Color.DarkBlue)
    .SaveAsPng("product-qr.png");
```

**Return barcode as byte array for an HTTP response:**

```csharp
using IronBarCode;

// In an ASP.NET Core controller action
byte[] barcodeBytes = BarcodeWriter.CreateBarcode("ORDER-7734", BarcodeEncoding.QRCode)
    .ToPngBinaryData();

return File(barcodeBytes, "image/png");
```

None of these patterns have ML Kit equivalents. They are new capabilities available because you are working in a full .NET barcode library rather than a mobile-only scanner.

## Server-Side Batch Processing

ML Kit processes one image per call, requires Android/iOS runtime, and has no concept of server-side execution. IronBarcode processes files in a loop, runs in ASP.NET Core, and scales normally:

```csharp
using IronBarCode;

// Process a folder of scanned document images
var imageFiles = Directory.GetFiles("/data/scans", "*.jpg");
var allResults = new List<(string File, string Value, BarcodeEncoding Type)>();

foreach (var file in imageFiles)
{
    var options = new BarcodeReaderOptions
    {
        Speed = ReadingSpeed.Faster,
        ExpectMultipleBarcodes = false
    };

    var results = BarcodeReader.Read(file, options);
    foreach (var barcode in results)
    {
        allResults.Add((file, barcode.Value, barcode.BarcodeType));
    }
}

// Write results to CSV, database, etc.
foreach (var (file, value, type) in allResults)
{
    Console.WriteLine($"{file}: [{type}] {value}");
}
```

This pattern — reading a folder of images, extracting barcodes, aggregating results — is not possible with ML Kit. It is a standard IronBarcode workflow.

## Feature Comparison

| Feature | Google ML Kit | IronBarcode |
|---|---|---|
| First-party .NET NuGet package | None | `BarCode` |
| C# / .NET API from vendor | None | Yes |
| Barcode reading | Yes (Android/iOS) | Yes (all platforms) |
| Barcode generation | No | Yes |
| QR code generation | No | Yes |
| QR logo embedding | No | Yes |
| PDF input | No | Yes |
| Multi-page document support | No | Yes |
| Camera/frame input | Yes | Via image file |
| Server-side deployment | No | Yes |
| ASP.NET Core | No | Yes |
| Azure Functions | No | Yes |
| Docker / Linux | No | Yes |
| Google Play Services required (unbundled) | Yes | No |
| Firebase dependency | No (since June 2020) | No |
| Synchronous .NET API | No | Yes |
| `ExpectMultipleBarcodes` option | Via result list | `BarcodeReaderOptions` |
| Format specification | `setBarcodeFormats()` | `ExpectedBarcodeTypes` |
| Speed/accuracy tradeoff | Fixed (model-based) | `ReadingSpeed` enum |
| Pricing | Free (on-device API) | From $799 perpetual (Lite) |
| Platforms | Android, iOS | Windows, Linux, macOS, Docker, Azure, AWS, MAUI |

## Migration Checklist

If you are porting an Android codebase or replacing an unofficial Xamarin ML Kit binding, search your project for these patterns and apply the translations above:

- `com.google.mlkit:barcode-scanning` in Gradle files → remove, add `BarCode` NuGet
- `com.google.android.gms:play-services-mlkit-barcode-scanning` (unbundled) → same removal
- `pod 'GoogleMLKit/BarcodeScanning'` in Podfile → remove
- `BarcodeScannerOptions.Builder()` → `new BarcodeReaderOptions { }`
- `BarcodeScanning.getClient(options)` → remove (no scanner instance in IronBarcode)
- `InputImage.fromFilePath(context, uri)` → file path string argument
- `InputImage.fromBitmap(bitmap, rotation)` → `BarcodeReader.Read(stream)` or byte array overload
- `scanner.process(inputImage)` → `BarcodeReader.Read(path, options)`
- `.addOnSuccessListener { barcodes -> }` → iterate return value of `Read()`
- `.addOnFailureListener { e -> }` → try/catch around `Read()`
- `barcode.rawValue` → `barcode.Value`
- `barcode.format` → `barcode.BarcodeType`
- `Barcode.FORMAT_QR_CODE` → `BarcodeEncoding.QRCode`
- `Barcode.FORMAT_CODE_128` → `BarcodeEncoding.Code128`
- `Barcode.FORMAT_ALL_FORMATS` → omit `ExpectedBarcodeTypes`
- Community Xamarin binding `using` directives → `using IronBarCode;`
- `IronBarCode.License.LicenseKey` should be set in `MauiProgram.cs`, `Program.cs`, or `Startup.cs`

The structural change from callback-based to synchronous code is the main work. The format constants and result field names are direct mappings. PDF and generation support are purely additive — they require no migration, only new code.
