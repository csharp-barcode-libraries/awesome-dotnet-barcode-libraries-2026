# Migrating from Neodynamic Barcode to IronBarcode

This guide provides a complete migration path from `Neodynamic.SDK.Barcode` and `Neodynamic.SDK.BarcodeReader` to IronBarcode. It covers the removal of both Neodynamic packages, the replacement of dual license configuration with a single key, the translation of generation and reading API calls, and the process of gaining 2D barcode reading capability that was not possible with the Neodynamic product set. The guide is written for developers who have existing Neodynamic code and need a systematic, file-by-file approach to completing the migration.

## Why Migrate from Neodynamic Barcode

**2D Reading Gap:** Neodynamic Barcode Professional SDK generates QR Code, DataMatrix, PDF417, and Aztec barcodes. None of those formats can be read by the companion Barcode Reader SDK. A project that generates QR codes for product labels, shipping containers, or document tracking cannot read those same codes back through the reader it purchased alongside the generator. IronBarcode reads and generates QR Code, DataMatrix, PDF417, Aztec, and all other supported 2D formats through the same library and the same license.

**Dual Product Overhead:** Every project that uses both Neodynamic SDKs carries two `PackageReference` entries, two `using` directives, two license blocks, and two separate update cycles. The overhead repeats across every developer machine, every CI pipeline, and every production environment. Consolidating two packages with separate maintenance requirements into a single dependency with a single license is the mechanical outcome of this migration.

**The Third-Library Problem:** Teams that encountered the 2D reading gap and chose to work around it rather than migrate typically added a third library — ZXing.Net being the most common choice — specifically to cover QR code reading. That decision trades one limitation for a different complexity: three barcode-related dependencies, each with its own version constraints, its own release notes, and its own potential for conflict when the .NET runtime or dependent framework is updated. IronBarcode replaces all three.

**Pricing Complexity:** Neodynamic Barcode Professional SDK is priced at approximately $245 for a single developer license. Barcode Reader SDK carries a separate cost. A project that needs both generation and 1D reading must purchase both, and the combined cost approaches or exceeds $500. A project that needs 2D reading still cannot meet that requirement at any price within the Neodynamic product family. IronBarcode Lite at $749 covers generation and reading — 1D and 2D — under a single license.

### The Fundamental Problem

The core issue in any codebase that used both Neodynamic SDKs for a complete barcode workflow is that QR code reading was never possible:

```csharp
// Before: Neodynamic Barcode Reader does not support QR codes
using Neodynamic.SDK.BarcodeReader;
using System.Drawing;

public string ReadQrCode(string imagePath)
{
    using var bitmap = new Bitmap(imagePath);
    var results = BarcodeReader.Read(bitmap);

    // Results are always null or empty for 2D barcodes
    if (results == null || !results.Any())
    {
        throw new NotSupportedException(
            "Neodynamic Barcode Reader does not support QR codes");
    }

    return results.First().Value;
}
```

After migrating to IronBarcode, the same operation becomes a standard read call with no format-specific handling:

```csharp
// After: IronBarcode reads QR codes with the same call used for all formats
using IronBarCode;

public string ReadQrCode(string imagePath)
{
    var result = BarcodeReader.Read(imagePath).FirstOrDefault();
    return result?.Value;
}
```

Every `throw new NotSupportedException(...)` placeholder in a codebase that references 2D barcode reading is removed during this migration and replaced with a working read call.

## IronBarcode vs Neodynamic Barcode: Feature Comparison

| Feature | Neodynamic Barcode Professional | Neodynamic Barcode Reader | IronBarcode |
|---|---|---|---|
| Code 128, EAN-13, UPC-A generation | Yes | N/A | Yes |
| QR Code, DataMatrix, PDF417 generation | Yes | N/A | Yes |
| 1D barcode reading | N/A | Yes | Yes |
| QR Code reading | N/A | **No** | Yes |
| DataMatrix reading | N/A | **No** | Yes |
| PDF417 reading | N/A | **No** | Yes |
| Native PDF barcode reading | No | No | Yes |
| Automatic format detection | N/A | No | Yes |
| NuGet packages required | 1 | 1 | 1 (covers both) |
| License keys required | 1 | 1 (separate) | 1 (covers all) |
| System.Drawing dependency | Yes | Yes | No |
| Linux / Docker support | Limited | Limited | Yes |
| .NET 8 / .NET 9 support | Yes | Limited | Yes |
| Async batch reading | No | No | Yes |

## Quick Start: Neodynamic Barcode to IronBarcode Migration

### Step 1: Remove Both Neodynamic Packages

Both packages must be removed. They have no shared dependencies that affect removal order:

```bash
dotnet remove package Neodynamic.SDK.Barcode
dotnet remove package Neodynamic.SDK.BarcodeReader
```

Verify both entries are gone from the `.csproj` file before proceeding.

### Step 2: Add IronBarcode

```bash
dotnet add package IronBarcode
```

### Step 3: Replace Dual License Configuration

Remove both Neodynamic license blocks and replace them with a single IronBarcode key assignment. This assignment is made once at application startup — in `Program.cs`, `Startup.cs`, or the application's composition root:

```csharp
// Before: two license blocks, two products
using Neodynamic.SDK.Barcode;
using Neodynamic.SDK.BarcodeReader;

BarcodeInfo.LicenseOwner = "Your Company";
BarcodeInfo.LicenseKey = "GEN-LICENSE-KEY";

Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseOwner = "Your Company";
Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseKey = "READ-LICENSE-KEY";
```

```csharp
// After: one key covers generation and reading across all formats
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

Any configuration files, environment variables, or secrets stores that hold Neodynamic license keys can be decommissioned after the migration is verified.

## Code Migration Examples

### Generating a Code 128 Barcode

**Neodynamic Approach:**

```csharp
using Neodynamic.SDK.Barcode;
using System.Drawing.Imaging;

public void GenerateCode128(string data, string outputPath)
{
    var barcode = new BarcodeInfo();
    barcode.Value = data;
    barcode.Symbology = Symbology.Code128;
    barcode.TextAlign = BarcodeTextAlignment.BelowCenter;
    barcode.Dpi = 300;

    System.Drawing.Image image = barcode.GetImage();
    image.Save(outputPath, ImageFormat.Png);
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

public void GenerateCode128(string data, string outputPath)
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128, 300)
        .SaveAsPng(outputPath);
}
```

The `System.Drawing.Imaging` import is no longer required. The `BarcodeInfo` instance and its property assignments collapse into a single method call. For sizing, annotation, and output format options, see the [creating 1D barcodes guide](https://ironsoftware.com/csharp/barcode/how-to/create-1d-barcodes/).

### Generating a QR Code

**Neodynamic Approach:**

```csharp
using Neodynamic.SDK.Barcode;

public void GenerateQrCode(string data, string outputPath)
{
    var barcode = new BarcodeInfo();
    barcode.Value = data;
    barcode.Symbology = Symbology.QRCode;
    barcode.QRCodeECL = QRCodeErrorCorrectionLevel.H;

    barcode.GetImage().Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

public void GenerateQrCode(string data, string outputPath)
{
    QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest)
        .SaveAsPng(outputPath);
}
```

The generation translation changes only the API surface. The critical difference introduced by this migration is that the same application can now also read the QR codes it generates, through the same IronBarcode library and the same license. For a full treatment of 2D barcode creation, see the [creating 2D barcodes guide](https://ironsoftware.com/csharp/barcode/how-to/create-2d-barcodes/).

### Reading 1D Barcodes

**Neodynamic Approach:**

```csharp
using Neodynamic.SDK.BarcodeReader;
using System.Drawing;

public string[] Read1DBarcodes(string imagePath)
{
    using var bitmap = new Bitmap(imagePath);
    var results = BarcodeReader.Read(bitmap);
    return results?.Select(r => r.Value).ToArray() ?? Array.Empty<string>();
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

public string[] Read1DBarcodes(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.Select(r => r.Value).ToArray();
}
```

The `Bitmap` construction and `System.Drawing` import are removed. IronBarcode accepts the file path directly. The `result.Value` property name is the same in both APIs, so downstream code that processes the returned values does not change. For multi-barcode image reading and preprocessing options, see the [reading barcodes from images guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/).

### Reading QR Codes (Previously Impossible)

**Neodynamic Approach:**

```csharp
// This method could not be implemented with Neodynamic Barcode Reader
public string ReadQrCode(string imagePath)
{
    throw new NotSupportedException(
        "Neodynamic Barcode Reader does not support QR codes");
}
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

public string ReadQrCode(string imagePath)
{
    var result = BarcodeReader.Read(imagePath).FirstOrDefault();
    return result?.Value;
}
```

Every `throw new NotSupportedException(...)` placeholder that references 2D barcode reading becomes a standard read call. The method body changes from a thrown exception to a working implementation. No special handling is required for QR codes; IronBarcode identifies the format automatically.

### Reading Barcodes from PDF Documents (New Capability)

**Neodynamic Approach:**

```csharp
// Neodynamic Reader cannot read barcodes from PDF files directly
// An image extraction step using a separate library was required before reading
// No direct equivalent exists
```

**IronBarcode Approach:**

```csharp
using IronBarCode;

public void ReadBarcodesFromPdf(string pdfPath)
{
    var results = BarcodeReader.Read(pdfPath);

    foreach (var result in results)
    {
        Console.WriteLine($"Value: {result.Value} | Format: {result.Format} | Page: {result.PageNumber}");
    }
}
```

IronBarcode reads barcodes directly from PDF documents across all pages. The page number on which each barcode was found is returned in the result. This capability requires no additional library and no intermediate image extraction step.

## Neodynamic Barcode API to IronBarcode Mapping Reference

| Neodynamic API | IronBarcode Equivalent | Notes |
|---|---|---|
| `BarcodeInfo.LicenseOwner = "..."` | `IronBarCode.License.LicenseKey = "key"` | Merged into single key |
| `BarcodeInfo.LicenseKey = "..."` | (part of single key above) | No separate owner field |
| `Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseOwner` | (removed) | Not required |
| `Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseKey` | (removed) | Not required |
| `new BarcodeInfo()` | `BarcodeWriter.CreateBarcode(data, encoding)` | Static, no instance |
| `barcode.Value = data` | First parameter of `CreateBarcode` | Passed at construction |
| `barcode.Symbology = Symbology.Code128` | `BarcodeEncoding.Code128` | Second parameter |
| `barcode.Symbology = Symbology.QRCode` | `BarcodeEncoding.QRCode` | Full round-trip now supported |
| `barcode.QRCodeECL = QRCodeErrorCorrectionLevel.H` | `QRCodeWriter.QrErrorCorrectionLevel.Highest` | Different class entry point |
| `barcode.GetImage().Save(path, ImageFormat.Png)` | `.SaveAsPng(path)` | Fluent, no ImageFormat enum |
| `BarcodeReader.Read(bitmap)` | `BarcodeReader.Read(imagePath)` | File path replaces Bitmap |
| `result.Value` | `result.Value` | Same property name |
| `Symbology.DataMatrix` | `BarcodeEncoding.DataMatrix` | Generation only → generation and reading |
| `throw new NotSupportedException(...)` for 2D reading | `BarcodeReader.Read(imagePath)` | Replace with working implementation |

## Common Migration Issues and Solutions

### Issue 1: Two-Package Removal Order

**Problem:** Developers removing only one Neodynamic package and leaving the other causes compilation errors because the two SDKs share no types but some source files may import both namespaces in the same file.

**Solution:** Remove both packages before making any source changes. After both are removed, search for all `using Neodynamic.SDK.Barcode` and `using Neodynamic.SDK.BarcodeReader` directives and remove them together:

```bash
dotnet remove package Neodynamic.SDK.Barcode
dotnet remove package Neodynamic.SDK.BarcodeReader
```

Then audit with:

```bash
grep -r "Neodynamic.SDK" --include="*.cs" .
```

### Issue 2: Dual License Configuration Cleanup

**Problem:** The two Neodynamic license blocks often appear in different locations — one in the application startup class and one in a service initialiser. If only one block is removed, the remaining code references a namespace that no longer exists and fails to compile.

**Solution:** Search for all four Neodynamic license property assignments and remove them together before adding the IronBarcode key:

```bash
grep -r "LicenseOwner\|LicenseKey" --include="*.cs" .
```

Replace all four lines with a single assignment:

```csharp
IronBarCode.License.LicenseKey = "YOUR-IRONBARCODE-KEY";
```

### Issue 3: NotSupportedException Removal from Catch Blocks

**Problem:** Some codebases that used Neodynamic Reader added `catch (NotSupportedException)` blocks specifically to handle the silent failure of 2D barcode reads. Those catch blocks must be removed along with the throwing code; otherwise the catch clause references a code path that can no longer occur.

**Solution:** Search for `NotSupportedException` in barcode-related files and review each occurrence. If the exception was raised in response to a 2D format read attempt, remove both the throw and the catch:

```bash
grep -r "NotSupportedException" --include="*.cs" .
```

Replace the throwing method body with `BarcodeReader.Read(imagePath)` and remove the corresponding catch clause from any caller.

## Neodynamic Barcode Migration Checklist

### Pre-Migration

- Search for all Neodynamic package references: `grep -r "Neodynamic.SDK" --include="*.csproj" .`
- Search for all Neodynamic using directives: `grep -r "using Neodynamic" --include="*.cs" .`
- Search for license configuration locations: `grep -r "LicenseOwner\|LicenseKey" --include="*.cs" .`
- Search for all `BarcodeInfo` instantiation sites: `grep -r "new BarcodeInfo\(\)" --include="*.cs" .`
- Search for all Neodynamic read calls: `grep -r "BarcodeReader.Read" --include="*.cs" .`
- Search for all `NotSupportedException` usages in barcode paths: `grep -r "NotSupportedException" --include="*.cs" .`
- Identify any third-party library (ZXing.Net or similar) that was added specifically to cover the 2D reading gap — this can be removed after migration
- Record all Neodynamic license keys so they can be decommissioned from secrets stores after verification

### Code Update Tasks

1. Run `dotnet remove package Neodynamic.SDK.Barcode`
2. Run `dotnet remove package Neodynamic.SDK.BarcodeReader`
3. Run `dotnet add package IronBarcode`
4. Remove all `using Neodynamic.SDK.Barcode` and `using Neodynamic.SDK.BarcodeReader` directives
5. Add `using IronBarCode` in all files that perform barcode operations
6. Remove both Neodynamic license configuration blocks
7. Add `IronBarCode.License.LicenseKey = "YOUR-KEY"` once at application startup
8. Replace `new BarcodeInfo()` + property assignments + `barcode.GetImage().Save(...)` with `BarcodeWriter.CreateBarcode(data, encoding).SaveAsPng(path)`
9. Replace `QRCodeWriter` patterns accordingly using `QRCodeWriter.CreateQrCode()`
10. Replace `BarcodeReader.Read(bitmap)` calls by removing the `new Bitmap(imagePath)` construction and passing the file path directly to `BarcodeReader.Read(imagePath)`
11. Remove all `throw new NotSupportedException(...)` placeholders for 2D reading and replace with `BarcodeReader.Read(imagePath)`
12. Remove corresponding `catch (NotSupportedException)` blocks from callers
13. Remove any third-party library that was added solely to cover the Neodynamic 2D reading gap

### Post-Migration Testing

- Verify Code 128 barcode generation produces output matching previous dimensions and DPI settings
- Verify QR Code generation produces scannable output
- Verify 1D barcode reading returns the same values as the previous Neodynamic Reader implementation
- Verify QR Code reading succeeds on images that previously threw `NotSupportedException`
- Verify DataMatrix and PDF417 reading succeeds if those formats appear in the application's input
- Test PDF barcode reading if the application processes PDF documents
- Confirm all Neodynamic license key references have been removed from configuration files and environment variables
- Confirm the third-party 2D library (if one was present) has been fully removed and no remaining imports reference it

## Key Benefits of Migrating to IronBarcode

**Unified Read-Write Capability:** After migration, every symbology that IronBarcode can generate can also be read back through the same library. The asymmetry between Neodynamic's generator and reader — where QR codes, DataMatrix, PDF417, and Aztec could be produced but not consumed — is eliminated. Applications that previously required a workaround or a third library for 2D reading gain that capability without any additional integration work.

**Single License Administration:** One NuGet package, one license key, and one configuration block replace two separate product subscriptions. Renewal, key rotation, and environment variable management become half as complex. The reduction in administrative overhead is ongoing rather than a one-time gain.

**Native PDF Barcode Reading:** IronBarcode reads barcodes from PDF documents directly, returning page number metadata alongside barcode values. Applications that process shipping manifests, invoice documents, or medical records containing embedded barcodes no longer require an intermediate image extraction step or an additional PDF library to support barcode reading. See the full list of [supported barcode formats](https://ironsoftware.com/csharp/barcode/get-started/supported-barcode-formats/) for the complete set of symbologies and input types covered.

**Elimination of System.Drawing Dependency:** Neodynamic's SDKs depend on `System.Drawing`, which requires additional native library configuration on Linux and in Docker containers. IronBarcode carries no `System.Drawing` dependency, which means the same codebase runs on Windows, Linux, and in containerised cloud environments without platform-specific configuration changes.

**Simplified Licensing Cost for Full Capability:** A project that needed both Neodynamic SDKs plus a third library for 2D reading was paying for three dependencies to accomplish what IronBarcode provides under a single license. For projects that require generation plus 2D reading, the migration consolidates both the technical footprint and the licensing cost into a single commercial product. Current pricing information is available on the [IronBarcode licensing page](https://ironsoftware.com/csharp/barcode/licensing/).
