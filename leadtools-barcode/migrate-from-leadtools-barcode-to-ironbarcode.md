# Migrating from LEADTOOLS Barcode to IronBarcode

This guide covers the complete migration from LEADTOOLS Barcode to IronBarcode for .NET developers. It addresses the removal of five NuGet packages and one native runtime dependency, the replacement of the file-based LEADTOOLS license initialization sequence with a string key, and the translation of LEADTOOLS reading and writing patterns into IronBarcode's static fluent API. Code examples are provided for every major migration scenario, along with an API mapping table, a structured checklist, and guidance on issues that arise during the transition.

## Why Migrate from LEADTOOLS Barcode

**File-Based License Deployment:** LEADTOOLS licensing requires a `.LIC` file to be physically present on disk at a known path on every machine where the application runs. Developer workstations, CI build agents, staging servers, and production containers each require the file to be provisioned before the application will initialize. This is a fundamentally different operational model from string-based secrets, which fit cleanly into environment variables, secrets managers, and CI/CD pipeline configuration. Every new environment added to a deployment topology — a new cloud region, a new Kubernetes node pool, a new developer onboarding — requires a file provisioning step that string-based licensing does not.

**SDK Footprint:** A minimum LEADTOOLS barcode installation requires five NuGet packages: `Leadtools.Barcode`, `Leadtools`, `Leadtools.Codecs`, `Leadtools.Codecs.Png`, and `Leadtools.Codecs.Jpeg`. PDF barcode extraction adds a sixth. On Windows, the MSVC++ 2017 Runtime is a separate native dependency. The published output of a LEADTOOLS barcode application is approximately 148 MB. Applications that need only barcode reading or generation carry the full weight of a comprehensive imaging platform — OCR infrastructure, format codec architecture, and document viewing capabilities — whether or not those features are used.

**Legacy API Design:** LEADTOOLS barcode operations require constructing multiple objects in a specific sequence before any work can be done. Reading a barcode requires a `RasterCodecs` instance to load the image and a `BarcodeEngine` instance to scan it, with an explicit `BarcodeSymbology` array declaring which formats to look for. Generating a barcode requires constructing a `BarcodeData` object, creating a blank `RasterImage` with explicit pixel parameters, filling it with a background color using `FillCommand`, writing the barcode onto it, and saving with `RasterCodecs`. That API reflects the design patterns of its era. IronBarcode performs the same operations through static method calls that accept file paths directly.

**Pricing Transparency:** LEADTOOLS development licenses are priced at $1,295–$1,469 per developer per year. Deployment licenses for production servers are separately quoted through LEADTOOLS sales and are not published. Teams that need a complete cost picture before beginning development must engage with sales before they can confirm that LEADTOOLS fits their budget. IronBarcode's Professional license is $1,499 one-time — perpetual, covering 10 developers and unlimited servers, with no deployment tracking and no separate production license required.

### The Fundamental Problem

The LEADTOOLS initialization sequence requires 20+ lines and four verification steps before a `BarcodeEngine` can be created. IronBarcode replaces the entire sequence with a single assignment:

```csharp
// LEADTOOLS: file path, expiry check, three feature lock checks, engine creation
using Leadtools;
using Leadtools.Barcode;

RasterSupport.SetLicense(
    @"C:\LEADTOOLS23\Support\Common\License\LEADTOOLS.LIC",
    "your-developer-key-here");

if (RasterSupport.KernelExpired)
    throw new InvalidOperationException("LEADTOOLS license has expired");

if (RasterSupport.IsLocked(RasterSupportType.Barcode1DRead))
    throw new InvalidOperationException("1D barcode reading is locked");

if (RasterSupport.IsLocked(RasterSupportType.Barcode2DRead))
    throw new InvalidOperationException("2D barcode reading is locked");

if (RasterSupport.IsLocked(RasterSupportType.BarcodeWrite))
    throw new InvalidOperationException("Barcode writing is locked");

var engine = new BarcodeEngine();
```

```csharp
// IronBarcode: one line, no file required
using IronBarCode;

IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

## IronBarcode vs LEADTOOLS Barcode: Feature Comparison

| Feature | LEADTOOLS Barcode | IronBarcode |
|---|---|---|
| License model | File + developer key | String key only |
| NuGet packages required | 5+ | 1 |
| Native runtime dependency | MSVC++ 2017 (Windows) | None |
| Published output size | ~148 MB | ~39 MB |
| Total symbologies supported | 40+ | 50+ |
| Auto-detect barcode format | Limited | Yes |
| PDF barcode extraction | Separate package required | Built-in |
| ML error correction | No | Yes |
| QR code logo branding | No | Yes |
| Docker deployment | File mount required | Environment variable |
| Initialization code | 20+ lines | 1 line |
| API style | Legacy object graph | Static fluent |
| Cross-platform | Partial (native deps) | Full (.NET Standard) |
| Deployment license pricing | Contact sales (not published) | Included in perpetual purchase |

## Quick Start: LEADTOOLS Barcode to IronBarcode Migration

### Step 1: Remove LEADTOOLS Packages

Remove all LEADTOOLS packages from the project:

```bash
dotnet remove package Leadtools.Barcode
dotnet remove package Leadtools
dotnet remove package Leadtools.Codecs
dotnet remove package Leadtools.Codecs.Png
dotnet remove package Leadtools.Codecs.Jpeg
```

If the PDF codec was added for barcode extraction from PDF files:

```bash
dotnet remove package Leadtools.Codecs.Pdf
```

On Windows Server deployments, the MSVC++ 2017 Runtime can be removed from provisioning scripts after the migration is complete. IronBarcode has no native runtime dependencies on any platform.

### Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

This single package includes all image format support, native PDF barcode extraction, ML error correction, and all 50+ supported symbologies. No additional codec packages are required.

### Step 3: Initialize the License

Replace the entire LEADTOOLS initialization block with a single line. Locate every call to `RasterSupport.SetLicense` in the codebase — there will typically be exactly one, at application startup — and replace the entire surrounding initialization sequence:

```csharp
// Before: entire LEADTOOLS initialization block (20+ lines)
// After:
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-LICENSE-KEY";
```

The license key can be sourced from any string-capable configuration mechanism: an environment variable, `appsettings.json`, Azure Key Vault, AWS Secrets Manager, or a Kubernetes secret. For environment-based license configuration, Docker deployment patterns, and all supported configuration approaches, see the [license key setup documentation](https://ironsoftware.com/csharp/barcode/licensing/).

## Code Migration Examples

### Reading Barcodes from Images

LEADTOOLS separates image loading from barcode scanning into two distinct object layers. A `RasterCodecs` instance loads the image file into a `RasterImage`, and a `BarcodeEngine` instance scans it. The caller must also provide an explicit array of `BarcodeSymbology` values; formats not listed in the array will not be detected.

**LEADTOOLS Approach:**
```csharp
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

using var codecs = new RasterCodecs();
using var image = codecs.Load("scan.png");

var engine = new BarcodeEngine();

var symbologies = new[]
{
    BarcodeSymbology.Code128,
    BarcodeSymbology.Code39,
    BarcodeSymbology.QR,
    BarcodeSymbology.DataMatrix,
    BarcodeSymbology.EAN13,
    BarcodeSymbology.UPCA
};

var barcodes = engine.Reader.ReadBarcodes(
    image,
    LogicalRectangle.Empty,
    0,
    symbologies);

foreach (var barcode in barcodes)
    Console.WriteLine($"{barcode.Symbology}: {barcode.Value}");
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

var results = BarcodeReader.Read("scan.png");

foreach (var result in results)
    Console.WriteLine($"{result.Format}: {result.Value}");
```

IronBarcode accepts the file path directly, loads the image internally, and auto-detects formats across all 50+ supported symbologies without requiring the caller to enumerate expected types. For advanced configuration — reading speed tuning, multi-barcode detection, and low-quality image handling — see the [reading barcodes from images guide](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/).

### Generating a Code 128 Barcode

LEADTOOLS generation requires constructing a `BarcodeData` object, allocating a blank `RasterImage` with explicit memory flags, pixel depth, and byte order, filling the image with a background color, writing the barcode onto it, and then saving with `RasterCodecs`. IronBarcode performs all of those steps through a single fluent method chain.

**LEADTOOLS Approach:**
```csharp
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

var engine = new BarcodeEngine();

var barcodeData = new BarcodeData(BarcodeSymbology.Code128)
{
    Value = "ITEM-98765",
    Bounds = new LeadRect(0, 0, 500, 120)
};

using var image = new RasterImage(
    RasterMemoryFlags.Conventional,
    500, 120, 24,
    RasterByteOrder.Bgr,
    RasterViewPerspective.TopLeft,
    null, IntPtr.Zero, 0);

new FillCommand(RasterColor.White).Run(image);
engine.Writer.WriteBarcode(image, barcodeData, null);

using var codecs = new RasterCodecs();
codecs.Save(image, "item-barcode.png", RasterImageFormat.Png, 0);
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("ITEM-98765", BarcodeEncoding.Code128)
    .ResizeTo(500, 120)
    .SaveAsPng("item-barcode.png");
```

For a full walkthrough of sizing, styling, margin control, and supported output formats, see [creating barcode images](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-images/).

### Generating a QR Code with Logo Branding

LEADTOOLS QR code generation follows the same multi-step `RasterImage` construction pattern as other barcode types. Logo embedding in the center of a QR code is not supported. IronBarcode provides `QRCodeWriter` with a dedicated `AddBrandLogo` method.

**LEADTOOLS Approach:**
```csharp
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

var engine = new BarcodeEngine();

var qrData = new BarcodeData(BarcodeSymbology.QR)
{
    Value = "https://example.com/product/12345",
    Bounds = new LeadRect(0, 0, 400, 400)
};

using var image = new RasterImage(
    RasterMemoryFlags.Conventional,
    400, 400, 24,
    RasterByteOrder.Bgr,
    RasterViewPerspective.TopLeft,
    null, IntPtr.Zero, 0);

new FillCommand(RasterColor.White).Run(image);
engine.Writer.WriteBarcode(image, qrData, null);

using var codecs = new RasterCodecs();
codecs.Save(image, "product-qr.png", RasterImageFormat.Png, 0);
// Note: LEADTOOLS does not support logo embedding in QR codes
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

var qr = QRCodeWriter.CreateQrCode(
    "https://example.com/product/12345",
    400,
    QRCodeWriter.QrErrorCorrectionLevel.Highest);

qr.AddBrandLogo("brand-logo.png");
qr.SaveAsPng("product-qr.png");
```

### Extracting Barcodes from PDF Documents

LEADTOOLS PDF barcode extraction requires the separate `Leadtools.Codecs.Pdf` package, explicit PDF loading configuration, a manual page count query, and a loop over each page. Each page must be loaded individually as a `RasterImage` before scanning. IronBarcode includes PDF support in the base package and handles page iteration internally.

**LEADTOOLS Approach:**
```csharp
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;

// Requires: dotnet add package Leadtools.Codecs.Pdf
using var codecs = new RasterCodecs();
codecs.Options.Pdf.Load.Resolution = 300;
codecs.Options.Pdf.Load.DisplayDepth = 24;

var engine = new BarcodeEngine();
var results = new List<string>();
int pageCount = codecs.GetTotalPages("shipment.pdf");

for (int page = 1; page <= pageCount; page++)
{
    using var image = codecs.Load(
        "shipment.pdf", 0,
        CodecsLoadByteOrder.BgrOrGray,
        page, page);

    var barcodes = engine.Reader.ReadBarcodes(
        image, LogicalRectangle.Empty, 0, null);

    foreach (var barcode in barcodes)
        results.Add($"Page {page}: {barcode.Value}");
}
```

**IronBarcode Approach:**
```csharp
using IronBarCode;

var results = BarcodeReader.Read("shipment.pdf");

foreach (var result in results)
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
```

### Docker Deployment

The file management requirement that defines every LEADTOOLS Docker deployment is eliminated entirely when migrating to IronBarcode.

**LEADTOOLS Approach:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY publish/ .

# License file must be physically present in the container image
COPY LEADTOOLS.LIC /app/license/LEADTOOLS.LIC

ENV LEADTOOLS_LICENSE_PATH=/app/license/LEADTOOLS.LIC
ENV LEADTOOLS_DEVELOPER_KEY=your-developer-key

ENTRYPOINT ["dotnet", "YourApp.dll"]
```

**IronBarcode Approach:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY publish/ .

ENV IRONBARCODE_LICENSE=your-license-key

ENTRYPOINT ["dotnet", "YourApp.dll"]
```

The license key is injected at container runtime through Docker secrets, Kubernetes secrets, or any environment variable mechanism. No file provisioning step is required. The [Docker and Linux deployment guide](https://ironsoftware.com/csharp/barcode/get-started/docker-linux/) covers Alpine and Debian base images, multi-stage builds, and Kubernetes deployment patterns.

## LEADTOOLS Barcode API to IronBarcode Mapping Reference

| LEADTOOLS | IronBarcode | Notes |
|---|---|---|
| `RasterSupport.SetLicense(path, key)` | `IronBarCode.License.LicenseKey = "key"` | Key only — no file |
| `RasterSupport.KernelExpired` | (removed) | No expiry check needed |
| `RasterSupport.IsLocked(RasterSupportType.Barcode1DRead)` | (removed) | All features included |
| `RasterSupport.IsLocked(RasterSupportType.Barcode2DRead)` | (removed) | All features included |
| `RasterSupport.IsLocked(RasterSupportType.BarcodeWrite)` | (removed) | All features included |
| `new BarcodeEngine()` | Static — no instance | `BarcodeReader`, `BarcodeWriter` are static |
| `new RasterCodecs()` | (removed) | Pass file path directly |
| `codecs.Load(imagePath)` | (removed) | Pass file path directly |
| `engine.Reader.ReadBarcodes(image, LogicalRectangle.Empty, 0, symbologies)` | `BarcodeReader.Read(imagePath)` | Auto-detects symbologies |
| `BarcodeData.Value` | `result.Value` | Same property name |
| `BarcodeData.Symbology` | `result.Format` | Property renamed |
| `new BarcodeData(BarcodeSymbology.Code128)` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)` | Fluent creation |
| `BarcodeSymbology.Code128` | `BarcodeEncoding.Code128` | Namespace change |
| `BarcodeSymbology.QR` | `BarcodeEncoding.QRCode` | Name change |
| `BarcodeSymbology.DataMatrix` | `BarcodeEncoding.DataMatrix` | Same name |
| `BarcodeSymbology.PDF417` | `BarcodeEncoding.PDF417` | Same name |
| `BarcodeSymbology.EAN13` | `BarcodeEncoding.EAN13` | Same name |
| `BarcodeSymbology.UPCA` | `BarcodeEncoding.UPCA` | Same name |
| `BarcodeSymbology.Interleaved2of5` | `BarcodeEncoding.Interleaved2of5` | Same name |
| `BarcodeSymbology.Codabar` | `BarcodeEncoding.Codabar` | Same name |
| `engine.Writer.WriteBarcode(image, data, null)` + `codecs.Save(...)` | `.SaveAsPng(path)` | One method chain |
| `new RasterImage(...)` + `new FillCommand(RasterColor.White).Run(image)` | (removed) | Internal to IronBarcode |

## Common Migration Issues and Solutions

### Issue 1: .LIC File References in CI/CD

**LEADTOOLS:** CI/CD pipelines that build or deploy LEADTOOLS applications must provision the `.LIC` file — either checking it into source control, decoding it from a base64 secret, or mounting it as a volume artifact. Each pipeline stage that runs the application needs the file accessible.

**Solution:** Remove all `.LIC` file handling from the pipeline. Replace with a string secret:

```yaml
# Example: GitHub Actions environment variable
env:
  IRONBARCODE_LICENSE: ${{ secrets.IRONBARCODE_LICENSE }}
```

The string value is stored in the CI/CD secrets store alongside other string-valued credentials. No file decoding, no artifact provisioning, no volume configuration is required.

### Issue 2: MSVC++ Runtime Dependency on Windows

**LEADTOOLS:** Windows Server deployments require the MSVC++ 2017 Runtime to be installed on the host. Provisioning scripts, container base images, and Windows AMIs used for LEADTOOLS deployments typically include a step to install this runtime.

**Solution:** Remove the MSVC++ 2017 Runtime installation step from all Windows provisioning scripts and container images. IronBarcode has no native runtime dependencies. The `dotnet/aspnet` base image is sufficient without additional runtime packages.

### Issue 3: RasterSupportType Feature Lock Checks

**LEADTOOLS:** The initialization sequence includes `RasterSupport.IsLocked()` checks for each barcode feature — `Barcode1DRead`, `Barcode2DRead`, and `BarcodeWrite`. These checks are defensive code required by the LEADTOOLS architecture to verify that the license file covers each required capability.

**Solution:** Remove all `RasterSupport.IsLocked()` calls. IronBarcode includes all reading and writing capabilities in a single license. There are no per-feature lock states and no feature verification step is needed:

```csharp
// Remove these lines entirely:
// if (RasterSupport.IsLocked(RasterSupportType.Barcode1DRead)) ...
// if (RasterSupport.IsLocked(RasterSupportType.Barcode2DRead)) ...
// if (RasterSupport.IsLocked(RasterSupportType.BarcodeWrite)) ...
```

### Issue 4: BarcodeSymbology Array Removal

**LEADTOOLS:** Every call to `engine.Reader.ReadBarcodes()` requires an explicit `BarcodeSymbology[]` parameter. Barcodes whose symbology is not listed in the array will not be detected, which can cause silent missed reads when a new barcode type is introduced in an input document.

**Solution:** Remove the symbology array. `BarcodeReader.Read()` auto-detects all supported formats. If performance tuning is needed for a known format set, `BarcodeReaderOptions` provides optional hints without requiring exhaustive enumeration:

```csharp
// Optional: hint the reader for performance in format-known scenarios
var options = new BarcodeReaderOptions
{
    ExpectBarcodeTypes = BarcodeEncoding.Code128 | BarcodeEncoding.QRCode
};
var results = BarcodeReader.Read("scan.png", options);
```

## LEADTOOLS Barcode Migration Checklist

### Pre-Migration Tasks

Audit all LEADTOOLS usage in the codebase before making any changes:

```bash
grep -r "using Leadtools" --include="*.cs" .
grep -r "RasterSupport\|BarcodeEngine\|RasterCodecs" --include="*.cs" .
grep -r "BarcodeSymbology\|BarcodeData\|RasterImage" --include="*.cs" .
grep -r "LEADTOOLS.LIC\|SetLicense\|KernelExpired" --include="*.cs" .
grep -r "COPY LEADTOOLS.LIC\|LEADTOOLS_LICENSE_PATH" --include="*.dockerfile" --include="Dockerfile" .
```

Document the count and location of initialization blocks, read call sites, write call sites, and Dockerfile references before beginning code changes.

### Code Update Tasks

1. Remove all five LEADTOOLS NuGet packages (and the PDF codec if present)
2. Install `IronBarcode` via `dotnet add package IronBarcode`
3. Replace all `using Leadtools.*` namespace imports with `using IronBarCode`
4. Replace the entire `RasterSupport.SetLicense` initialization block with `IronBarCode.License.LicenseKey = "key"`
5. Remove all `RasterSupport.KernelExpired` checks
6. Remove all `RasterSupport.IsLocked(RasterSupportType.*)` checks
7. Remove all `new BarcodeEngine()` instantiations
8. Remove all `new RasterCodecs()` instantiations from barcode code paths
9. Replace `codecs.Load(path)` + `engine.Reader.ReadBarcodes(image, ...)` with `BarcodeReader.Read(path)`
10. Replace `new BarcodeData(...)` + `new RasterImage(...)` + `FillCommand` + `WriteBarcode` + `codecs.Save()` with `BarcodeWriter.CreateBarcode(...).SaveAsPng(path)`
11. Replace `BarcodeSymbology.*` enum references with `BarcodeEncoding.*` (note: `QR` becomes `QRCode`)
12. Remove `COPY LEADTOOLS.LIC` lines from all Dockerfiles
13. Remove `LEADTOOLS_LICENSE_PATH` and `LEADTOOLS_DEVELOPER_KEY` environment variable declarations from Dockerfiles and deployment configuration
14. Add `IRONBARCODE_LICENSE` environment variable to Dockerfiles and secrets configuration
15. Remove MSVC++ 2017 Runtime installation from Windows provisioning scripts

### Post-Migration Testing

- Verify all barcode read operations return expected values across all symbology types previously covered by the `BarcodeSymbology[]` array
- Verify Code 128, QR code, and any other generated barcode types produce valid, scannable output
- Confirm PDF barcode extraction works correctly without `Leadtools.Codecs.Pdf`
- Build and start the container image without mounting any `.LIC` file and confirm the application initializes correctly
- Run the CI/CD pipeline without the `.LIC` file provisioning step and confirm all stages complete successfully
- Verify the published output size and container image pull time have improved

## Key Benefits of Migrating to IronBarcode

**Simplified Deployment Architecture:** After migration, every environment — development, CI, staging, production — receives the license through the same string-valued secrets mechanism used for API keys, database passwords, and other credentials. No file provisioning, no artifact handling, and no volume mounts are required. Adding a new deployment environment is the same operation as adding any other secret.

**Reduced Dependency Surface:** One NuGet package replaces five, and the MSVC++ 2017 Runtime requirement is eliminated. The published output of a barcode service drops from approximately 148 MB to approximately 39 MB. Smaller images mean faster container pulls, lower storage costs in container registries, and reduced cold start times in serverless and auto-scaling environments.

**Streamlined Application Startup:** The 20-line LEADTOOLS initialization sequence — file path resolution, expiry verification, and three per-feature lock checks — is replaced with a single line. Application startup is faster and the initialization code path has no file system dependency that could fail in a new environment.

**Comprehensive Format Detection:** IronBarcode reads across all 50+ supported symbologies without requiring the caller to specify expected formats. The risk of silent missed reads — caused by omitting a symbology from the LEADTOOLS `BarcodeSymbology[]` array — is eliminated. When a new barcode type appears in an input document, IronBarcode detects it without requiring a code change.

**Predictable Total Cost:** A single perpetual license purchase covers development, testing, and unlimited production deployments with no deployment tracking, no annual renewal requirement, and no separate production license to obtain. Teams can confirm the complete cost of the library before beginning development, without a sales conversation.

**Modern API Design:** `BarcodeReader.Read()` and `BarcodeWriter.CreateBarcode()` are static entry points that accept file paths directly. The multi-layer object construction that LEADTOOLS reading and writing require — codec instances, engine instances, image allocation, background fill — is handled internally. Code that reads and generates barcodes is shorter, easier to review, and easier to test.
