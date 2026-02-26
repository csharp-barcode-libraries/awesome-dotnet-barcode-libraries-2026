# Migrating from Barcode4NET to IronBarcode

This migration guide covers removing a Barcode4NET dependency and replacing it with IronBarcode, with step-by-step instructions, code comparisons, and practical examples for .NET developers undertaking this transition. Barcode4NET reached end-of-life without a formal announcement — new licenses stopped being sold, no NuGet package was ever published, and the library runs exclusively on .NET Framework. This guide is practical and specific: it covers removing the DLL reference, installing IronBarcode via NuGet, and converting the generation code. It also covers the reading and PDF capabilities that become available after migration, which were not available through Barcode4NET at all.

## Why Migrate from Barcode4NET

Teams migrating from Barcode4NET report these consistent triggers:

**No New Licenses Available:** Barcode4NET licenses are no longer sold through ComponentSource or any other channel. If a developer joins a team using Barcode4NET, there is no mechanism to purchase a seat for them. A contractor joining the project or a second developer needing to work on barcode features cannot be legally licensed under the original product terms.

**No NuGet Package:** This is not a minor inconvenience — it is a fundamental incompatibility with modern .NET workflows. Every developer who clones the repository must know where the DLL lives. CI/CD pipelines must be manually configured to locate it. The `dotnet restore` command does nothing to resolve it. The DLL is either checked into source control or maintained in a private artifact store, both of which require ongoing manual maintenance.

**No .NET 5, 6, 7, 8, or 9 Support:** Barcode4NET targets .NET Framework exclusively. When an organization upgrades its target framework to `net8.0`, the build breaks because the library was compiled against .NET Framework assemblies that are no longer referenced. There is no patch coming. If the team needs to move to modern .NET for performance, for Linux or Docker deployment, or for long-term Microsoft support, Barcode4NET is a hard blocker.

**No Security Patches:** The library is frozen at whatever state it was in when development stopped. Any security issues discovered since then remain permanently unaddressed. There is no issue tracker, no maintainer to contact, and no vendor response process.

**No Bug Fixes:** If a rendering issue appears with a particular symbology variant or an edge case in barcode dimensions, there is no resolution path. The library will not be updated.

### The Fundamental Problem

The most common discovery moment is a developer upgrading the target framework during a .NET modernization project. The build fails because Barcode4NET's DLL was compiled against .NET Framework assemblies that are no longer available under the new target. The problem is not the generation API itself — it is the underlying dependency that cannot move forward.

```csharp
// Barcode4NET — locked to .NET Framework, no NuGet package
// Requires manual DLL reference in every .csproj:
// <Reference Include="Barcode4NET">
//   <HintPath>..\ThirdParty\Barcode4NET\Barcode4NET.dll</HintPath>
// </Reference>
using Barcode4NET;
using System.Drawing;
using System.Drawing.Imaging;

var barcode = new Barcode4NET.Barcode();
barcode.Symbology = Symbology.Code128;
barcode.Data = "ITEM-12345";
barcode.Width = 300;
barcode.Height = 100;
Bitmap barcodeImage = barcode.GenerateBarcode();
barcodeImage.Save(outputPath, ImageFormat.Png);
```

```csharp
// IronBarcode — NuGet package, .NET Framework 4.6.2 through .NET 9
// dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .SaveAsPng(outputPath);
```

## IronBarcode vs Barcode4NET: Feature Comparison

| Feature | Barcode4NET | IronBarcode |
|---|---|---|
| NuGet package | No — manual DLL only | Yes (`IronBarcode`) |
| .NET Framework support | Yes (.NET Framework only) | Yes (.NET Framework 4.6.2+) |
| .NET Core / .NET 5+ support | No | Yes (.NET 5, 6, 7, 8, 9) |
| Linux / macOS support | No | Yes |
| Docker support | No | Yes |
| Azure / AWS Lambda | No | Yes |
| Barcode generation | Yes | Yes |
| Barcode reading / scanning | No | Yes (`BarcodeReader.Read()`) |
| PDF barcode reading | No | Yes (native, no extra library) |
| QR code with logo | No | Yes (`AddBrandLogo()`) |
| Fluent chainable API | No | Yes |
| Active maintenance | No (end-of-life) | Yes |
| New licenses available | No | Yes — Lite $749, Plus $1,499, Professional $2,999, Unlimited $5,999 |
| Security patches | No | Yes |
| CI/CD pipeline integration | Manual DLL copy steps | Standard `dotnet restore` |

## Quick Start: Barcode4NET to IronBarcode Migration

The migration can begin immediately with these foundational steps.

### Step 1: Replace NuGet Package

Barcode4NET was never distributed as a NuGet package, so there is no `dotnet remove package` command. Remove it manually.

Delete the DLL from source control and stage the deletion:

```bash
git rm -r ThirdParty/Barcode4NET/
```

Remove the `<Reference>` element from each `.csproj` file. Search across the solution to find every file that needs updating:

```bash
grep -rl "Barcode4NET" --include="*.csproj" .
```

Remove this element from every `.csproj` file that contains it:

```xml
<Reference Include="Barcode4NET">
  <HintPath>..\ThirdParty\Barcode4NET\Barcode4NET.dll</HintPath>
</Reference>
```

Then install IronBarcode:

```bash
dotnet add package IronBarcode
```

That single command adds IronBarcode and resolves all dependencies. No DLL management, no artifact store, no manual copy steps.

### Step 2: Update Namespaces

Replace the old `using` directive at the top of each file that used Barcode4NET:

```csharp
// Before
using Barcode4NET;

// After
using IronBarCode;
```

### Step 3: Initialize License

Add license initialization at application startup — in `Program.cs`, `Startup.cs`, or wherever your application bootstraps:

```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

## Code Migration Examples

### Basic Label Generation

The most common Barcode4NET pattern: create an object, set properties, call `GenerateBarcode()` to receive a `Bitmap`, then save it.

**Barcode4NET Approach:**

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

**IronBarcode Approach:**

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

The property-setter block collapses into a single fluent chain. The `.ResizeTo()` call replaces the `Width` and `Height` property assignments. `.SaveAsPng()` replaces both `GenerateBarcode()` and the separate `barcodeImage.Save()` call. For additional generation options, see the [IronBarcode barcode generation documentation](https://ironsoftware.com/csharp/barcode/tutorials/generate-barcode/).

### WinForms PictureBox Display

If your Barcode4NET code assigned the `Bitmap` return value directly to a `PictureBox` or similar control, you will load a byte array through a `MemoryStream` instead.

**Barcode4NET Approach:**

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

**IronBarcode Approach:**

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

The `MemoryStream` step is the only structural addition. Everything else is a direct translation from the property-setter pattern to the fluent chain.

### QR Code Generation

**Barcode4NET Approach:**

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

**IronBarcode Approach:**

```csharp
using IronBarCode;

QRCodeWriter.CreateQrCode("https://example.com/product/99", 300)
    .SaveAsPng("qr.png");
```

IronBarcode uses a dedicated `QRCodeWriter` class for QR codes, which gives access to QR-specific options such as error correction level and brand logo embedding. The second parameter to `QRCodeWriter.CreateQrCode()` is the pixel size of the square image.

### Adding Barcode Reading

Barcode4NET had no reading API. If your application processes scanned documents, verifies generated barcodes, or extracts data from PDF invoices, this capability is available immediately after migration without adding a second library.

**Barcode4NET Approach:**

```csharp
// No reading API available in Barcode4NET.
// Required a separate library (e.g., ZXing.Net) to decode barcodes.
```

**IronBarcode Approach:**

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
```

Reading from PDFs requires no additional library. `BarcodeReader.Read()` handles both image and PDF inputs natively. See the [barcode reading documentation](https://ironsoftware.com/csharp/barcode/tutorials/read-barcode/) for configuration options including reading speed and multi-barcode detection.

### Upgrading the Target Framework

Once Barcode4NET is removed, the .NET Framework constraint goes with it. Update the `TargetFramework` element in your `.csproj`:

**Barcode4NET Approach:**

```xml
<!-- Locked to .NET Framework to support Barcode4NET -->
<PropertyGroup>
  <TargetFramework>net472</TargetFramework>
</PropertyGroup>
```

**IronBarcode Approach:**

```xml
<!-- Upgrade freely — IronBarcode supports net8.0 and later -->
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

IronBarcode supports .NET Framework 4.6.2 through .NET 9, so the target framework can be upgraded to whatever your organization targets without any barcode library constraint.

### ASP.NET Core Controller — Returning Barcode as Image Response

If you are modernizing a Web Forms application at the same time, here is the ASP.NET Core pattern for returning a barcode image from a controller action:

**Barcode4NET Approach:**

```csharp
// Barcode4NET was not compatible with ASP.NET Core.
// Web Forms HttpHandler was required for image delivery.
```

**IronBarcode Approach:**

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

`.ToPngBinaryData()` returns a `byte[]` directly, which slots into the `File()` result without any intermediate conversion.

## Barcode4NET API to IronBarcode Mapping Reference

| Barcode4NET | IronBarcode |
|---|---|
| `new Barcode4NET.Barcode()` | `BarcodeWriter.CreateBarcode(data, encoding)` |
| `barcode.Symbology = Symbology.Code128` | `BarcodeEncoding.Code128` (parameter to `CreateBarcode`) |
| `barcode.Data = "ITEM-12345"` | First parameter of `CreateBarcode()` |
| `barcode.Width = 300; barcode.Height = 100` | `.ResizeTo(300, 100)` |
| `barcode.GenerateBarcode()` returns `Bitmap` | `.SaveAsPng(path)` / `.ToPngBinaryData()` |
| `Symbology.QRCode` | `BarcodeEncoding.QRCode` |
| `Symbology.Code39` | `BarcodeEncoding.Code39` |
| `Symbology.EAN13` | `BarcodeEncoding.EAN13` |
| `Symbology.UPCA` | `BarcodeEncoding.UPCA` |
| `<Reference Include="Barcode4NET">` | `<PackageReference Include="IronBarcode" />` |
| `.NET Framework only` | `.NET Framework 4.6.2 through .NET 9` |
| Generation only | Generation + reading (`BarcodeReader.Read()`) |
| No PDF support | `BarcodeReader.Read("doc.pdf")` native |

## Common Migration Issues and Solutions

### Issue 1: No `dotnet remove package` for DLL References

**Barcode4NET:** Because Barcode4NET was never a NuGet package, there is no package manager command to remove it. The `<Reference Include="Barcode4NET">` element must be removed from each `.csproj` file manually, and the DLL directory must be deleted from source control explicitly.

**Solution:** Run a search to locate all project files that reference the DLL, then remove the elements by hand:

```bash
grep -rn "Barcode4NET" --include="*.csproj" .
```

Do not leave the DLL reference in place alongside the new NuGet reference — doing so causes build errors from ambiguous type resolution.

### Issue 2: Return Type Change from Bitmap to Byte Array

**Barcode4NET:** `GenerateBarcode()` returns `System.Drawing.Bitmap`. Code that assigns the return value to a `Bitmap`-typed variable or passes it to any method expecting a `Bitmap` — such as a `PictureBox` — must be updated.

**Solution:** Replace `.GenerateBarcode()` assignments with `.ToPngBinaryData()` and load the byte array through a `MemoryStream` at the call site:

```csharp
byte[] pngBytes = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .ToPngBinaryData();

using var stream = new MemoryStream(pngBytes);
pictureBox1.Image = Image.FromStream(stream);
```

Searching for `.GenerateBarcode()` across the solution will locate all call sites that need updating.

### Issue 3: Symbology Enum Namespace Change

**Barcode4NET:** Uses `Symbology.Code128`, `Symbology.QRCode`, `Symbology.Code39`, and similar values from the `Barcode4NET` namespace.

**Solution:** Replace with `BarcodeEncoding` enum values. All common names are preserved — only the type name changes:

```csharp
// Before
barcode.Symbology = Symbology.Code128;

// After
BarcodeEncoding.Code128  // passed as parameter to CreateBarcode()
```

A solution-wide find-and-replace of `Symbology.` with `BarcodeEncoding.` covers most cases. Review each replacement to confirm context.

### Issue 4: Width and Height Are Now a Method, Not Properties

**Barcode4NET:** Size was configured as two separate property assignments: `barcode.Width = 300; barcode.Height = 100;`

**Solution:** Use the `.ResizeTo(width, height)` chained method. This is syntactically different enough that a simple find-and-replace will not catch it — search for `.Width =` and `.Height =` in barcode-related code and update each occurrence:

```csharp
// Before
barcode.Width = 300;
barcode.Height = 100;

// After — chained on the CreateBarcode call
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .ResizeTo(300, 100)
    .SaveAsPng(outputPath);
```

## Barcode4NET Migration Checklist

### Pre-Migration Tasks

Run these searches across your solution before starting. They provide an accurate picture of the migration scope:

```bash
# Find all Barcode4NET usages in source files
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

Note the file count for each pattern. Document coordinate-based drawing code, font usage patterns, and any build script steps that copy the DLL.

### Code Update Tasks

1. Delete `ThirdParty/Barcode4NET/` from source control (`git rm -r`)
2. Remove `<Reference Include="Barcode4NET">` from all `.csproj` files
3. Remove manual DLL copy steps from build scripts and CI/CD pipeline files
4. Run `dotnet add package IronBarcode` for each project in the solution
5. Add `IronBarCode.License.LicenseKey = "YOUR-KEY";` to application startup
6. Replace `using Barcode4NET;` with `using IronBarCode;` in all `.cs` files
7. Replace `new Barcode4NET.Barcode()` blocks with `BarcodeWriter.CreateBarcode()` calls
8. Replace `Symbology.X` with `BarcodeEncoding.X` throughout
9. Replace `barcode.Width = N; barcode.Height = M;` with `.ResizeTo(N, M)`
10. Replace `.GenerateBarcode()` assignments with `.SaveAsPng()` or `.ToPngBinaryData()`
11. Update `PictureBox` or other image control assignments to use the `MemoryStream` pattern
12. Update `TargetFramework` in `.csproj` files if upgrading from `net4xx`
13. Run the full build and fix any remaining compile errors

### Post-Migration Testing

- Compare visual barcode output against known-good samples to confirm appearance
- Verify barcode scanner hardware can read generated barcodes from all affected symbologies
- Test `BarcodeEncoding` values match expected output for Code128, Code39, EAN-13, UPC-A, and QR Code
- Verify the `MemoryStream` pattern displays correctly in WinForms controls where applicable
- Confirm CI/CD pipeline completes successfully using only `dotnet restore` without manual DLL steps
- Test barcode reading functionality if `BarcodeReader.Read()` was added as part of the migration

## Key Benefits of Migrating to IronBarcode

**Restored Ability to License New Team Members:** IronBarcode is an active commercial product with perpetual licenses available at defined price points. New developers, contractors, and team additions can be licensed immediately. The situation where a developer joins and cannot legally work on barcode-related code ceases to exist.

**Standard NuGet Dependency Management:** After the migration, `dotnet restore` handles everything. New developers clone the repository, restore packages, and build without any knowledge of DLL locations, artifact stores, or manual copy procedures. The friction that Barcode4NET's DLL distribution created in every CI/CD pipeline and onboarding workflow is eliminated.

**Full .NET Version Freedom:** Removing the Barcode4NET constraint unblocks .NET modernization projects. The target framework can be upgraded to .NET 8, .NET 9, or future versions without any barcode library limitation. Linux deployment, Docker containerization, and cloud serverless runtimes all become available.

**Barcode Reading Without a Second Library:** `BarcodeReader.Read()` decodes barcodes from images and PDFs natively. Teams that were previously combining Barcode4NET for generation with ZXing.Net or another library for reading can consolidate to a single maintained dependency. PDF barcode reading requires no additional PDF library.

**Security and Compliance Posture:** IronBarcode receives regular security patches and is tracked against current CVE databases. Software Bill of Materials audits will no longer surface Barcode4NET as an end-of-life commercial dependency with no vendor response process. The remediation finding that Barcode4NET creates in security audits resolves as soon as the migration completes.

**Active Development Cadence:** IronBarcode ships regular updates aligned with new .NET releases. With .NET 10 expected in late 2026, compatibility updates will be available without any action required beyond updating the package version. The library will not be frozen at the state it was in when a vendor went quiet.
