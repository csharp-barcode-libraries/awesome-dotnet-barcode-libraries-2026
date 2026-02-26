MessagingToolkit.Barcode was last updated in 2014. Version 1.7.0.2 is the final release. If you have it in your project, you have a dependency that cannot be installed in any .NET project targeting .NET Core or later — which means it is actively preventing you from modernizing your application's runtime. This guide gets you out.

The migration is not complicated. The two libraries share ZXing heritage, the API surface is small, and the conceptual model is the same: decode an image to get a value, encode a string to get an image. The main differences are naming and the fact that IronBarcode's API is static rather than instance-based. Most teams complete this migration in an afternoon.

## Why Migrate

There are four reasons to migrate, in order of urgency.

First, the framework blocker. MessagingToolkit.Barcode targets .NET Framework 3.5, 4.0, and 4.5. It has no .NET Standard, no .NET Core, and no modern .NET target. As long as it is installed and your code references it, your project's `<TargetFramework>` is pinned to the .NET Framework era. Removing it is a prerequisite for upgrading to .NET 6, 7, 8, or 9. It is not a barcode problem — it is a .NET upgrade problem that manifests as a barcode dependency.

Second, security. Twelve years without security patches means any vulnerability discovered in the library's image parsing code, its ZXing-derived decode logic, or its transitive dependencies remains permanently unpatched. There is no maintainer. There is no patch. Security scanners flag it as abandoned. Compliance frameworks — PCI DSS, HIPAA, SOC 2 — require active patch management, which an abandoned library categorically cannot provide.

Third, the platform targets in the NuGet package description. Silverlight 3, 4, and 5 are listed — they were discontinued in 2021. Windows Phone 7.x and 8.0 are listed — discontinued in 2014 and 2017. These are not edge cases; they are the primary listed targets of a library that has never been updated to support anything else.

Fourth, capability gaps. MessagingToolkit.Barcode has no PDF reading support. It returns a single result per decode call. It requires `System.Drawing.Bitmap` as input, which is Windows-only in .NET 6 and later. The library was adequate for 2012 requirements — it is not adequate for 2026 ones.

## Quick Start: Three Steps

### Step 1 — Remove the old package

```bash
dotnet remove package MessagingToolkit.Barcode
```

If you have a manual DLL reference to `MessagingToolkit.Barcode.dll` in your `.csproj`, remove that reference as well. Check for `<HintPath>` entries pointing to the DLL in any project file.

### Step 2 — Install IronBarcode

```bash
dotnet add package IronBarcode
```

IronBarcode supports .NET Framework 4.6.2 through .NET 9. It installs a single NuGet package with all dependencies bundled — no separate graphics library, no ZXing reference to maintain alongside it.

### Step 3 — Update namespaces and add license initialization

In every file that previously imported MessagingToolkit:

```csharp
// Remove this
using MessagingToolkit.Barcode;

// Add this
using IronBarCode;
```

Add license initialization once, at application startup — in `Program.cs`, `Startup.cs`, or equivalent. A [license key](https://ironsoftware.com/csharp/barcode/get-started/license-keys/) is required for production use; the library runs in trial mode without one.

```csharp
// Add once at application startup
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

## Code Migration Examples

### Reading: BarcodeDecoder to BarcodeReader.Read()

The old pattern required instantiating `BarcodeDecoder`, creating a `Bitmap` from the file path, passing the bitmap to `.Decode()`, and checking `result.Text`. Each step required a separate object and a null check — `.Decode()` returned null when no barcode was found.

**Before:**

```csharp
using MessagingToolkit.Barcode;
using System.Drawing;

public string ReadBarcode(string imagePath)
{
    var decoder = new BarcodeDecoder();

    using (var bitmap = new Bitmap(imagePath))
    {
        var result = decoder.Decode(bitmap);

        if (result != null)
        {
            return result.Text;
        }
    }

    return null;
}
```

**After:**

```csharp
using IronBarCode;

public string ReadBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value;
}
```

`BarcodeReader.Read()` accepts a file path directly — no `Bitmap` required, no `System.Drawing` import needed. It returns a collection rather than a single nullable result, which handles images that contain multiple barcodes without requiring a separate decode-all call. The equivalent of `result.Text` is `result.Value`.

### Reading: Accessing Format Information

MessagingToolkit exposed the detected format through `result.BarcodeFormat`. IronBarcode exposes it through `result.Format`. Both are enum values on the result object.

**Before:**

```csharp
var result = decoder.Decode(bitmap);
if (result != null)
{
    Console.WriteLine($"Value: {result.Text}");
    Console.WriteLine($"Format: {result.BarcodeFormat}");
}
```

**After:**

```csharp
var results = BarcodeReader.Read("barcode.png");
var first = results.FirstOrDefault();
if (first != null)
{
    Console.WriteLine($"Value: {first.Value}");
    Console.WriteLine($"Format: {first.Format}");
}
```

### Writing: BarcodeEncoder to BarcodeWriter.CreateBarcode()

The old generation pattern required an instance of `BarcodeEncoder`, setting the `Format` property before calling `Encode()`, and then saving the resulting `Bitmap` using `System.Drawing`. IronBarcode's writer is static — no instance, format is a parameter, and the result object has its own save methods.

**Before:**

```csharp
using MessagingToolkit.Barcode;

public void CreateBarcode(string data, string outputPath)
{
    var encoder = new BarcodeEncoder();
    encoder.Format = BarcodeFormat.QrCode;

    var bitmap = encoder.Encode(data);
    bitmap.Save(outputPath);
}
```

**After:**

```csharp
using IronBarCode;

public void CreateBarcode(string data, string outputPath)
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode)
        .SaveAsPng(outputPath);
}
```

For Code128 and other 1D formats, the same pattern applies. IronBarcode's [1D barcode generation](https://ironsoftware.com/csharp/barcode/how-to/create-1d-barcodes/) supports Code128, Code39, EAN-13, EAN-8, UPC-A, UPC-E, and all the formats MessagingToolkit.Barcode covered:

```csharp
// Code128
BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
    .SaveAsPng("code128.png");

// EAN-13
BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
    .SaveAsPng("ean13.png");
```

### WinForms: Replacing the Bitmap Input

If your existing code passes a `System.Drawing.Image` or `Bitmap` from a `PictureBox` or another control to `BarcodeDecoder`, the migration involves converting it to a stream. IronBarcode accepts streams directly — it does not need a `Bitmap` object.

**Before:**

```csharp
private void btnScan_Click(object sender, EventArgs e)
{
    var decoder = new BarcodeDecoder();
    using (var bitmap = new Bitmap(pictureBox1.Image))
    {
        var result = decoder.Decode(bitmap);
        if (result != null)
        {
            textBoxResult.Text = result.Text;
        }
    }
}
```

**After:**

```csharp
private void btnScan_Click(object sender, EventArgs e)
{
    using var stream = new MemoryStream();
    pictureBox1.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
    stream.Position = 0;

    var results = BarcodeReader.Read(stream);
    if (results.Any())
    {
        textBoxResult.Text = results.First().Value;
    }
}
```

This pattern works in .NET Framework projects during the migration period. Once you complete the migration and upgrade the target framework, you can remove the `System.Drawing` dependency entirely — IronBarcode's stream input will accept any `Stream` from any image source without needing `System.Drawing.Imaging`.

### New Capability: PDF Reading

MessagingToolkit.Barcode had no PDF support. IronBarcode reads barcodes from PDF documents using the same `BarcodeReader.Read()` call that reads images. This is not a configuration option — it detects the file type automatically.

```csharp
// Read all barcodes from every page of a PDF
var results = BarcodeReader.Read("invoice.pdf");
foreach (var barcode in results)
{
    Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Value} ({barcode.Format})");
}
```

For applications that process scanned documents, shipping manifests, or multi-page invoice batches, this eliminates the need for a separate PDF extraction step before calling the barcode decoder. IronBarcode handles all of it internally.

### Upgrading the Target Framework

Once MessagingToolkit.Barcode is removed and all references are replaced, you can update your project file's target framework. This step is optional but is the primary reason many teams undertake this migration:

```xml
<!-- Before: blocked on .NET Framework -->
<PropertyGroup>
  <TargetFramework>net472</TargetFramework>
</PropertyGroup>

<!-- After: modern .NET -->
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

IronBarcode supports .NET Framework 4.6.2 through .NET 9, so it is compatible on both sides of this change. You can install it before removing MessagingToolkit.Barcode, run both packages side by side during the migration, verify the new code, and then remove the old package and update the target framework as a final step.

## API Mapping Table

| MessagingToolkit.Barcode | IronBarcode | Notes |
|---|---|---|
| `new BarcodeDecoder()` | Static — `BarcodeReader.Read()` | No instance required |
| `barcodeReader.Decode(bitmap)` | `BarcodeReader.Read(path)` | Accepts path, stream, or byte[] |
| `result.Text` | `result.Value` | Property renamed |
| `result.BarcodeFormat` | `result.Format` | Property renamed |
| `new BarcodeEncoder()` | Static — `BarcodeWriter.CreateBarcode()` | No instance required |
| `barcodeWriter.Format = BarcodeFormat.QrCode` | `BarcodeEncoding.QRCode` (parameter) | Format passed as parameter, not property |
| `barcodeWriter.Encode("data")` returns `Bitmap` | `BarcodeWriter.CreateBarcode("data", BarcodeEncoding.QRCode)` | Returns fluent result, not Bitmap |
| `bitmap.Save("path.png")` | `.SaveAsPng("path.png")` | Fluent method on result object |
| `BarcodeFormat.QrCode` | `BarcodeEncoding.QRCode` | Enum namespace and value renamed |
| `BarcodeFormat.Code128` | `BarcodeEncoding.Code128` | Same symbolic name |
| Returns null if not found | Returns empty collection | Check `.Any()` or `.FirstOrDefault()` |
| .NET Framework 3.5–4.5 only | .NET 4.6.2 through .NET 9 | Full modern .NET support |
| PDF reading | Not available | `BarcodeReader.Read("file.pdf")` |

## Migration Checklist

Run these searches across your codebase to find every location that needs updating. All five patterns should return zero results when migration is complete.

```bash
# Find all using statements (most reliable starting point)
grep -r "using MessagingToolkit.Barcode" --include="*.cs"

# Find decoder instantiations
grep -r "BarcodeDecoder" --include="*.cs"

# Find encoder instantiations
grep -r "BarcodeEncoder" --include="*.cs"

# Find decode calls (may appear as decoder.Decode or barcodeReader.Decode)
grep -r "\.Decode(" --include="*.cs"

# Find encode calls
grep -r "\.Encode(" --include="*.cs"
```

After the grep pass:

- [ ] All `using MessagingToolkit.Barcode;` statements replaced with `using IronBarCode;`
- [ ] License key initialization added at application startup
- [ ] All `BarcodeDecoder` instances replaced with `BarcodeReader.Read()` calls
- [ ] All `BarcodeEncoder` instances replaced with `BarcodeWriter.CreateBarcode()` calls
- [ ] All `result.Text` references updated to `result.Value`
- [ ] All `result.BarcodeFormat` references updated to `result.Format`
- [ ] All `bitmap.Decode(bitmap)` patterns replaced with path or stream inputs
- [ ] All `encoder.Encode(data)` patterns replaced with `CreateBarcode(data, encoding)` chains
- [ ] `MessagingToolkit.Barcode` removed from all `.csproj` and `packages.config` files
- [ ] `dotnet remove package MessagingToolkit.Barcode` executed and verified
- [ ] Build succeeds with zero MessagingToolkit references remaining
- [ ] Barcode read operations tested with real barcode images from your application
- [ ] Barcode generation tested and output verified against expected format
- [ ] (Optional) Target framework updated to `net8.0` or `net9.0`

The point this migration closes is a specific one: the inability to move forward. As long as MessagingToolkit.Barcode is in your dependency tree, your application cannot run on the platforms where modern .NET applications run. That is not a limitation you work around — it is one you remove.

