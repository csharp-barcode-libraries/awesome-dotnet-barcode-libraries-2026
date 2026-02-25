# Migrating from Barcode4NET to IronBarcode

This migration isn't about choosing a better library. It's about removing a dependency that can no longer be updated, licensed, or supported. If you have Barcode4NET in production, you likely already know the situation: the product reached end-of-life, new licenses are unavailable, there's no NuGet package, and the library only runs on .NET Framework. The question isn't whether to migrate — it's how to do it with minimum disruption.

This guide is practical and specific. It covers removing the DLL reference, installing IronBarcode via NuGet, and converting the generation code. It also covers the reading and PDF capabilities you gain in the process, which weren't available with Barcode4NET at all.

## What Changes and What Doesn't

The Barcode4NET API was a property-setter pattern — create an object, set `Symbology`, `Data`, `Width`, `Height`, then call `GenerateBarcode()` to get a `Bitmap`. IronBarcode is a fluent static API — call `BarcodeWriter.CreateBarcode()` with data and encoding, chain resize and color options, then call a save method.

The mapping is direct. The code volume stays roughly the same. The main structural change is that IronBarcode returns a barcode result object that writes to disk or returns bytes, rather than returning a `Bitmap` you save separately. If your calling code pipes the `Bitmap` to a `PictureBox` or passes it to an image-manipulation method, you'll adjust at those call sites — it's a `MemoryStream` load rather than a direct bitmap assignment.

Everything else — namespace import, project file reference, CI/CD pipeline step, and DLL file removal — is cleanup work, not code rewriting.

## Step 1: Remove the Barcode4NET DLL Reference

Barcode4NET was never distributed via NuGet. There's no `dotnet remove package` command that handles this. You remove it manually.

**Delete the DLL from source control.** Find the `ThirdParty/Barcode4NET/` directory (or wherever your team stored it — common locations are `lib/`, `references/`, or `external/`) and delete it. If the DLL was committed directly to the repo, this removes it from the working tree. You'll still need to `git rm` it to stage the deletion.

```bash
# Remove the ThirdParty directory and stage the deletion
git rm -r ThirdParty/Barcode4NET/
```

**Remove the `<Reference>` element from each `.csproj` file.** Search across the solution for the reference and remove it:

```xml
<!-- Remove this from every .csproj file that contains it -->
<Reference Include="Barcode4NET">
  <HintPath>..\ThirdParty\Barcode4NET\Barcode4NET.dll</HintPath>
</Reference>
```

Run a search to find every file that needs updating:

```bash
grep -rl "Barcode4NET" --include="*.csproj" .
```

**Remove manual DLL copy steps from build scripts.** If your CI/CD pipeline has steps like `copy ThirdParty\Barcode4NET\*.dll bin\` or custom MSBuild targets that copy the DLL, remove those too. After installing IronBarcode via NuGet, `dotnet restore` handles everything.

## Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

That single command adds IronBarcode to your project and resolves all dependencies. No DLL management, no artifact store, no manual copy steps.

## Step 3: Update Namespaces and License Initialization

Replace the old `using` directive at the top of each file that used Barcode4NET:

```csharp
// Before
using Barcode4NET;

// After
using IronBarCode;
```

Add license initialization at application startup — in `Program.cs`, `Startup.cs`, or wherever your app bootstraps:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

## Code Migration Examples

### Basic Generation

The most common Barcode4NET pattern: create an object, set properties, call `GenerateBarcode()`.

**Before:**

```csharp
using Barcode4NET;
using System.Drawing;
using System.Drawing.Imaging;

public void SaveLabel(string sku, string outputPath)
{
    var barcode = new Barcode4NET.Barcode();
    barcode.Symbology = Symbology.Code128;
    barcode.Data = sku;
    barcode.Width = 400;
    barcode.Height = 120;
    Bitmap barcodeImage = barcode.GenerateBarcode();
    barcodeImage.Save(outputPath, ImageFormat.Png);
}
```

**After:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public void SaveLabel(string sku, string outputPath)
{
    BarcodeWriter.CreateBarcode(sku, BarcodeEncoding.Code128)
        .ResizeTo(400, 120)
        .SaveAsPng(outputPath);
}
```

The property-setter block collapses into a single fluent chain. The call to `.ResizeTo()` replaces the `Width` and `Height` property assignments. `.SaveAsPng()` replaces both `GenerateBarcode()` and `barcodeImage.Save()`.

### WinForms PictureBox Display

If your Barcode4NET code assigned the `Bitmap` return value directly to a `PictureBox` or similar control, you'll load the byte array through a `MemoryStream` instead:

**Before:**

```csharp
using Barcode4NET;
using System.Drawing;

private void RefreshBarcodeDisplay(string data)
{
    var barcode = new Barcode4NET.Barcode();
    barcode.Symbology = Symbology.Code128;
    barcode.Data = data;
    barcode.Width = 300;
    barcode.Height = 100;
    pictureBox1.Image = barcode.GenerateBarcode();
}
```

**After:**

```csharp
using IronBarCode;
using System.Drawing;
using System.IO;

private void RefreshBarcodeDisplay(string data)
{
    byte[] pngBytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ResizeTo(300, 100)
        .ToPngBinaryData();

    using var stream = new MemoryStream(pngBytes);
    pictureBox1.Image = Image.FromStream(stream);
}
```

The `MemoryStream` step is the only structural addition. Everything else is a direct translation.

### QR Code Generation

**Before (if your Barcode4NET version supported QR):**

```csharp
using Barcode4NET;

var barcode = new Barcode4NET.Barcode();
barcode.Symbology = Symbology.QRCode;
barcode.Data = "https://example.com/product/99";
barcode.Width = 300;
barcode.Height = 300;
Bitmap qrImage = barcode.GenerateBarcode();
qrImage.Save("qr.png", System.Drawing.Imaging.ImageFormat.Png);
```

**After:**

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com/product/99", 300)
    .SaveAsPng("qr.png");
```

IronBarcode uses a separate `QRCodeWriter` class for QR codes, which gives you access to QR-specific options like error correction level and brand logo embedding. The `QRCodeWriter.CreateQrCode()` second parameter is the pixel size of the square image.

### Adding Barcode Reading (Net-New Capability)

Barcode4NET had no reading API at all. If you're processing scanned documents or need to verify generated barcodes, you can add this now without pulling in a second library:

```csharp
using IronBarCode;

// Read a barcode from an image file
var results = BarcodeReader.Read("scanned-label.png");
foreach (var result in results)
{
    Console.WriteLine($"Value: {result.Value}");
    Console.WriteLine($"Symbology: {result.Format}");
}

// Read all barcodes from a PDF — pages are handled automatically
var pdfResults = BarcodeReader.Read("invoice-batch.pdf");
foreach (var result in pdfResults)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
}

// Configure reading speed and multi-barcode detection
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true
};
var multiResults = BarcodeReader.Read("warehouse-scan.png", options);
```

Reading from PDFs requires no additional library. `BarcodeReader.Read()` handles both image and PDF inputs natively.

### Upgrading the Target Framework

Once Barcode4NET is removed, the .NET Framework constraint goes with it. Update the `TargetFramework` element in your `.csproj`:

```xml
<!-- Before — locked to .NET Framework to support Barcode4NET -->
<PropertyGroup>
  <TargetFramework>net472</TargetFramework>
</PropertyGroup>

<!-- After — upgrade freely -->
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

IronBarcode supports .NET Framework 4.6.2 through .NET 9, so you can upgrade the TFM to whatever your organization targets without any barcode library constraint.

### ASP.NET Core Controller — Returning Barcode as Image Response

If you're modernizing a Web Forms application at the same time, here's the ASP.NET Core pattern for returning a barcode image from a controller action:

```csharp
using IronBarCode;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/barcodes")]
public class BarcodesController : ControllerBase
{
    [HttpGet("{data}")]
    public IActionResult Get(string data)
    {
        byte[] pngBytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
            .ResizeTo(400, 120)
            .ToPngBinaryData();

        return File(pngBytes, "image/png");
    }
}
```

`.ToPngBinaryData()` returns a `byte[]` directly, which slots into `File()` without any intermediate conversion.

## Common Migration Issues

**No `dotnet remove package` for DLL references.** Because Barcode4NET was never a NuGet package, there's no package manager command to remove it. The `<Reference Include="Barcode4NET">` element must be removed from each `.csproj` file manually, and the DLL directory must be deleted from source control explicitly. Don't skip this step — leaving the DLL reference in place alongside the new NuGet reference will cause build errors from ambiguous type resolution.

**`GenerateBarcode()` returns `Bitmap`, `.ToPngBinaryData()` returns `byte[]`.** If your code assigns the return value to a variable typed as `Bitmap`, or passes it directly to any method that expects a `Bitmap`, you need to update those call sites. Searching for `.GenerateBarcode()` will find them all. In most cases, the change is wrapping the byte array in a `MemoryStream` and calling `Image.FromStream()`.

**Symbology enum values change namespace, not names.** `Symbology.Code128` becomes `BarcodeEncoding.Code128`. `Symbology.QRCode` becomes `BarcodeEncoding.QRCode`. The names are consistent — only the enum type changes. A find-and-replace of `Symbology.` to `BarcodeEncoding.` covers the common cases, though you'll want to verify each one.

**Width and Height are now a method, not properties.** Barcode4NET used `barcode.Width = 300; barcode.Height = 100;` as separate property assignments. IronBarcode uses `.ResizeTo(300, 100)` as a chained method call. This is syntactically different enough that a simple find-and-replace won't catch it — search for `.Width =` and `.Height =` in barcode-related code and update each one to the chained form.

## Migration Checklist

Run these searches across your solution before starting. They'll give you an accurate picture of the scope:

```bash
# Find all Barcode4NET usages
grep -rn "Barcode4NET" --include="*.cs" .

# Find the using directive
grep -rn "using Barcode4NET" --include="*.cs" .

# Find object creation
grep -rn "new Barcode4NET.Barcode()" --include="*.cs" .

# Find Symbology enum usage
grep -rn "Symbology\." --include="*.cs" .

# Find GenerateBarcode calls
grep -rn "\.GenerateBarcode()" --include="*.cs" .

# Find the DLL reference in project files
grep -rn "Barcode4NET" --include="*.csproj" .

# Find DLL references in build scripts
grep -rn "Barcode4NET" --include="*.yml" .
grep -rn "Barcode4NET" --include="*.yaml" .
grep -rn "ThirdParty/Barcode4NET" .
```

Work through the checklist in this order:

- [ ] Run searches above and note the file count for each pattern
- [ ] Delete `ThirdParty/Barcode4NET/` from source control (`git rm -r`)
- [ ] Remove `<Reference Include="Barcode4NET">` from all `.csproj` files
- [ ] Remove manual DLL copy steps from build scripts and CI/CD pipeline files
- [ ] Run `dotnet add package IronBarcode` for each project in the solution
- [ ] Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` to application startup
- [ ] Replace `using Barcode4NET;` with `using IronBarCode;` in all `.cs` files
- [ ] Replace `new Barcode4NET.Barcode()` blocks with `BarcodeWriter.CreateBarcode()` calls
- [ ] Replace `Symbology.X` with `BarcodeEncoding.X` throughout
- [ ] Replace `barcode.Width = N; barcode.Height = M;` with `.ResizeTo(N, M)`
- [ ] Replace `.GenerateBarcode()` assignments with `.SaveAsPng()` or `.ToPngBinaryData()`
- [ ] Update `PictureBox` or other image control assignments to use `MemoryStream` pattern
- [ ] Update `TargetFramework` in `.csproj` files if upgrading from `net4xx`
- [ ] Run the full build and fix any remaining compile errors
- [ ] Test barcode output visually against known-good samples
- [ ] Verify CI/CD pipeline completes without manual DLL steps

## Pricing Reference

IronBarcode is available with perpetual licenses. Current pricing (as of 2026):

| Tier | Developers | Price |
|---|---|---|
| Lite | 1 developer | $749 |
| Plus | 3 developers | $1,499 |
| Professional | 10 developers | $2,999 |
| Unlimited | Unlimited | $5,999 |

All tiers include royalty-free deployment and .NET Framework through .NET 9 support.

## What You Gain Beyond the Direct Replacement

The migration removes the EOL dependency and restores your ability to upgrade .NET versions. But it also adds capabilities that weren't available through Barcode4NET:

**Barcode reading.** `BarcodeReader.Read()` works on images, PDFs, and streams. If your application receives scanned documents, processes user-uploaded images, or needs to verify generated barcodes, this is available immediately after migration — no additional library required.

**PDF support.** `BarcodeReader.Read("file.pdf")` reads barcodes from PDFs natively. If you're generating barcodes that end up embedded in PDF invoices or shipping documents, you can read them back without extracting images first.

**Linux and Docker.** Removing the .NET Framework constraint means you can containerize services that generate barcodes. IronBarcode runs on the standard .NET runtime images without additional system dependencies.

**Standard NuGet dependency management.** `dotnet restore` handles everything. New developers clone the repo, restore packages, and build — no DLL files to track down, no artifact store to configure, no manual copy steps to document in a README.

The migration cost is primarily the removal work — deleting the DLL, updating project files, updating build scripts. The code changes are minimal and mechanical. The gain is a dependency that's actually maintained, actually installable by new team members, and compatible with wherever your .NET stack is going.
