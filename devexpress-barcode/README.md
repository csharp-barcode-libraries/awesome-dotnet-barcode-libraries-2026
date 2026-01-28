# DevExpress Barcode vs IronBarcode: C# Barcode Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

DevExpress BarCodeControl is a UI component within the DevExpress WinForms, WPF, and ASP.NET/Blazor subscriptions, providing barcode generation capabilities as part of a larger control library. As a bundled component with no standalone purchase option, DevExpress Barcode targets developers already invested in the DevExpress ecosystem. This guide examines DevExpress's barcode capabilities, its generation-only limitation, suite subscription requirements, and how it compares to standalone alternatives like [IronBarcode](https://ironsoftware.com/csharp/barcode/) for barcode-specific development needs.

## Table of Contents

1. [What is DevExpress Barcode?](#what-is-devexpress-barcode)
2. [Suite Bundling Context](#suite-bundling-context)
3. [Capabilities Comparison](#capabilities-comparison)
4. [Installation and Setup](#installation-and-setup)
5. [Code Comparison](#code-comparison)
6. [Pricing Analysis](#pricing-analysis)
7. [When to Use Each](#when-to-use-each)
8. [Migration Guide](#migration-guide)

---

## What is DevExpress Barcode?

DevExpress is a major provider of UI controls and frameworks for .NET developers. Their barcode component, BarCodeControl, is included within their WinForms, WPF, and web platform subscriptions as a generation control.

### Product Architecture

The DevExpress barcode component is distributed as part of platform-specific assemblies:

- **DevExpress.XtraEditors.vXX.X.dll** (WinForms) - Contains BarCodeControl
- **DevExpress.Xpf.Printing.vXX.X.dll** (WPF) - Contains BarCode
- **DevExpress.Blazor.dll** (Blazor) - Contains DxBarCode

Unlike standalone barcode libraries, these are not available as individual NuGet packages. You install the entire DevExpress platform suite to access the barcode control.

### Core Capabilities

**What DevExpress Barcode Does:**
- Generates 1D barcodes (Code 128, Code 39, EAN, UPC, etc.)
- Generates 2D barcodes (QR Code, DataMatrix, PDF417)
- Renders barcodes within forms and reports
- Integrates with DevExpress Reporting for printing
- Provides Symbology property for format selection

**What DevExpress Barcode Does NOT Do:**
- Read or scan barcodes from images
- Recognize barcode content from files
- Extract barcodes from PDF documents
- Automatic barcode format detection (input side)
- Process barcodes in batch from file systems

This is confirmed directly in DevExpress support tickets: "DevExpress does not provide barcode recognition/reading functionality."

### Symbology Configuration

DevExpress requires explicit symbology configuration through the Symbology property:

```csharp
barCodeControl.Symbology = new Code128Generator();
// or
barCodeControl.Symbology = new QRCodeGenerator();
```

Each symbology type has specific properties that must be configured. There is no automatic "best fit" selection based on data content.

---

## Suite Bundling Context

DevExpress does not sell barcode functionality separately. Understanding your options requires understanding their subscription model.

### Available Subscription Options

| Subscription | New License | Annual Renewal | Barcode Included |
|-------------|-------------|----------------|------------------|
| DXperience (WinForms + WPF) | $1,199 | $550/year | Yes |
| Universal (All platforms) | $1,799 | $850/year | Yes |
| Blazor Subscription | $399 | $199/year | Yes (Blazor only) |
| DevExtreme (JS/Angular) | N/A | N/A | No .NET barcode |

**There is no "barcode only" purchase option.** If you need DevExpress Barcode, you must purchase at minimum a $399 Blazor subscription or $1,199 DXperience subscription.

### No Perpetual License Available

DevExpress operates on a subscription-only model:

- **No perpetual license option** - You must renew annually to continue using new versions
- **Grace period** - After subscription lapses, you can use the last version you licensed
- **Updates require active subscription** - Bug fixes in new versions require renewal

This contrasts with perpetual licensing where you pay once and own the license indefinitely.

### Suite Lock-In Economics

When evaluating the total cost, consider the barcode-specific value:

```
DevExpress DXperience Subscription:
├── Grid controls
├── Data editors
├── Charting
├── Scheduling
├── Navigation controls
├── Ribbon/Toolbar controls
├── Reporting
├── ... 200+ other controls
└── BarCodeControl (~0.5% of total)

If you only need barcode functionality:
- Year 1: $1,199 (new license)
- Year 2: $550 (renewal)
- Year 3: $550 (renewal)
- Year 4: $550 (renewal)
- Year 5: $550 (renewal)
- 5-year total: $3,399

IronBarcode Professional:
- Year 1: $2,999 (one-time, 10 devs)
- Years 2-5: $0
- 5-year total: $2,999

Single developer comparison:
- DevExpress 5-year: $3,399
- IronBarcode Lite 5-year: $749
- Savings: $2,650
```

---

## Capabilities Comparison

### Feature Matrix

| Feature | DevExpress Barcode | IronBarcode |
|---------|-------------------|-------------|
| **Generation** | Yes | Yes |
| **Reading/Recognition** | No | Yes |
| **1D Barcode Formats** | 25+ | 30+ |
| **2D Barcode Formats** | QR, DataMatrix, PDF417, Aztec | QR, DataMatrix, PDF417, Aztec, MaxiCode |
| **PDF Barcode Extraction** | No | Yes (native) |
| **PDF Barcode Embedding** | Via Reporting only | Direct API |
| **Automatic Format Detection** | No (manual Symbology) | Yes |
| **ML Error Correction** | No | Yes |
| **Batch File Processing** | No | Built-in |
| **Logo Embedding (QR)** | No | Yes |
| **Standalone License** | No | Yes |

### Symbology Support Detail

**DevExpress Supported Formats:**

1D: Codabar, Code 11, Code 39, Code 39 Extended, Code 93, Code 93 Extended, Code 128, EAN-8, EAN-13, UPC-A, UPC-E, UPC Supplemental 2, UPC Supplemental 5, Industrial 2 of 5, Interleaved 2 of 5, Matrix 2 of 5, MSI/Plessey, PostNet, USPS Intelligent Mail

2D: QR Code, DataMatrix, PDF417, Aztec

**IronBarcode Additional Formats:**

All DevExpress formats plus: MaxiCode, Micro QR, Micro PDF417, GS1-128, GS1 DataBar, RSS-14, RSS Limited, RSS Expanded, Royal Mail, Australia Post, Pharmacode, Telepen, Code 16K, and more.

### Reading Capability (The Key Difference)

DevExpress support has explicitly confirmed multiple times that barcode reading is not available:

> "DevExpress BarCode control is designed for barcode generation only. We do not provide barcode recognition/scanning functionality."
> - DevExpress Support Center

For reading barcodes with DevExpress controls, their support team suggests using third-party libraries like ZXing.Net or... IronBarcode.

---

## Installation and Setup

### DevExpress Installation

**Step 1: Subscription Activation**

1. Purchase DevExpress subscription ($399-$1,799)
2. Log into DevExpress Customer Portal
3. Download DevExpress installer or obtain NuGet feed credentials
4. Install via DevExpress Project Wizard or NuGet

**Step 2: NuGet Configuration (if using NuGet)**

DevExpress packages are not on public NuGet.org. You need to configure their private feed:

```xml
<!-- nuget.config -->
<configuration>
  <packageSources>
    <add key="DevExpress" value="https://nuget.devexpress.com/{YOUR-FEED-KEY}/api" />
  </packageSources>
</configuration>
```

**Step 3: Package Installation**

```bash
# For WinForms
dotnet add package DevExpress.Win.BarcodeControl

# For WPF
dotnet add package DevExpress.Wpf.Printing

# For Blazor
dotnet add package DevExpress.Blazor
```

**Step 4: License Verification**

DevExpress validates your subscription at build time. Without an active subscription:
- Trial watermarks appear on controls
- Build warnings are generated
- Updates are not available

### IronBarcode Installation

**Step 1: NuGet Installation**

```bash
# Single package from public NuGet.org
dotnet add package IronBarcode
```

**Step 2: License Configuration**

```csharp
// Single line at startup
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

No private NuGet feeds, no subscription verification at build time, no additional configuration.

### Setup Complexity Comparison

| Step | DevExpress | IronBarcode |
|------|------------|-------------|
| Purchase/Activate | Required (portal login) | Key delivered via email |
| NuGet source | Private feed configuration | Public NuGet.org |
| Packages to install | Platform-specific suite packages | Single package |
| Build verification | Subscription check | None |
| Trial limitations | Watermarks on controls | Watermarks on output |
| Offline development | Requires activation | Works immediately |

---

## Code Comparison

### Scenario 1: Basic Barcode Generation

**DevExpress (WinForms):**

```csharp
using DevExpress.XtraBars.BarCode;
using DevExpress.XtraBars.BarCode.Symbologies;

public void GenerateBarcode()
{
    // Create the control
    var barCodeControl = new BarCodeControl();

    // Configure symbology (required - no auto-detection)
    barCodeControl.Symbology = new Code128Generator();

    // Set the data
    barCodeControl.Text = "12345678";

    // Configure appearance
    barCodeControl.AutoModule = true;
    barCodeControl.ShowText = true;

    // For specific DPI/printing, set Module
    // barCodeControl.Module = 0.02; // Affects printed size

    // Add to form
    this.Controls.Add(barCodeControl);

    // To save as image file (not built-in):
    // Export through printing subsystem required
}
```

**DevExpress (Blazor):**

```razor
@using DevExpress.Blazor

<DxBarCode
    Type="BarCodeType.Code128"
    Value="12345678"
    Width="200px"
    Height="100px"
    ShowText="true">
</DxBarCode>
```

**IronBarcode:**

```csharp
using IronBarCode;

// One line to generate and save
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");
```

**Key Difference:** DevExpress focuses on UI control embedding. IronBarcode focuses on file/stream generation with direct save methods.

For detailed generation examples, see [DevExpress Generation-Only Example](devexpress-generation-only.cs).

### Scenario 2: Barcode Reading

**DevExpress:**

```csharp
// DevExpress Barcode does NOT support reading.
// From DevExpress Support:
// "We do not have plans to implement barcode recognition."

// There is no API for:
// - Reading from images
// - Reading from PDFs
// - Scanning from camera
// - Batch processing files

// Recommended alternatives from DevExpress support:
// - ZXing.Net (open source)
// - IronBarcode (commercial)
// - Dynamsoft (commercial)
```

**IronBarcode:**

```csharp
using IronBarCode;

// Single line reading with automatic format detection
var results = BarcodeReader.Read("barcode.png");

foreach (var barcode in results)
{
    Console.WriteLine($"Type: {barcode.BarcodeType}");
    Console.WriteLine($"Value: {barcode.Text}");
}
```

**Key Difference:** IronBarcode includes reading; DevExpress does not offer it at all.

### Scenario 3: Symbology Configuration

**DevExpress (Manual Configuration Required):**

```csharp
using DevExpress.XtraBars.BarCode.Symbologies;

public void ConfigureSymbology()
{
    var barCode = new BarCodeControl();

    // Code 128 with specific options
    var code128 = new Code128Generator();
    code128.CharacterSet = Code128CharacterSet.CharsetAuto;
    barCode.Symbology = code128;
    barCode.Text = "ITEM-12345";

    // QR Code with specific options
    var qrCode = new QRCodeGenerator();
    qrCode.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;
    qrCode.CompactionMode = QRCodeCompactionMode.AlphaNumeric;
    qrCode.Version = 5; // Explicit version
    barCode.Symbology = qrCode;
    barCode.Text = "https://example.com";

    // DataMatrix with specific options
    var dataMatrix = new DataMatrixGenerator();
    dataMatrix.CompactionMode = DataMatrixCompactionMode.ASCII;
    dataMatrix.MatrixSize = DataMatrixSize.Matrix26x26;
    barCode.Symbology = dataMatrix;
    barCode.Text = "DM-12345";

    // Module property affects physical size
    // Must be calculated based on printer DPI
    barCode.Module = CalculateModuleForDPI(300);
}

private double CalculateModuleForDPI(int dpi)
{
    // DevExpress Module is in document units
    // Formula depends on target output
    return 1.0 / dpi * 96; // Example calculation
}
```

**IronBarcode (Simpler Configuration):**

```csharp
using IronBarCode;

public void GenerateWithOptions()
{
    // Code 128 - encoding selected automatically
    BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
        .ResizeTo(400, 100)
        .SaveAsPng("code128.png");

    // QR Code - error correction as parameter
    var qr = QRCodeWriter.CreateQrCode(
        "https://example.com",
        500,
        QRCodeWriter.QrErrorCorrectionLevel.Highest
    );
    qr.SaveAsPng("qrcode.png");

    // DataMatrix - size handled automatically
    BarcodeWriter.CreateBarcode("DM-12345", BarcodeEncoding.DataMatrix)
        .SaveAsPng("datamatrix.png");
}
```

For detailed symbology configuration comparisons, see [DevExpress Symbology Configuration Example](devexpress-symbology-config.cs).

---

## Pricing Analysis

### License Comparison

| Aspect | DevExpress | IronBarcode |
|--------|------------|-------------|
| **License Model** | Subscription only | Perpetual with optional renewal |
| **Standalone Barcode** | Not available | Yes |
| **Minimum Entry (1 dev)** | $399/year (Blazor) or $1,199 (WinForms/WPF) | $749 one-time |
| **Renewal Required** | Yes (for updates) | No |
| **Barcode Reading** | Not included (use third-party) | Included |

### 5-Year Total Cost of Ownership

**Scenario: Single Developer, WinForms Application**

```
DevExpress DXperience:
  Year 1: $1,199 (new license)
  Year 2: $550 (renewal)
  Year 3: $550 (renewal)
  Year 4: $550 (renewal)
  Year 5: $550 (renewal)
  ─────────────────────
  Total: $3,399

IronBarcode Lite:
  Year 1: $749 (one-time)
  Year 2: $0
  Year 3: $0
  Year 4: $0
  Year 5: $0
  ─────────────────────
  Total: $749

Savings with IronBarcode: $2,650
```

**Scenario: 10-Developer Team, Full Platform Coverage**

```
DevExpress Universal:
  Year 1: $17,990 (10 × $1,799)
  Year 2: $8,500 (10 × $850)
  Year 3: $8,500
  Year 4: $8,500
  Year 5: $8,500
  ─────────────────────
  Total: $51,990

IronBarcode Professional:
  Year 1: $2,999 (one-time, 10 devs)
  Year 2: $0
  Year 3: $0
  Year 4: $0
  Year 5: $0
  ─────────────────────
  Total: $2,999

Savings with IronBarcode: $48,991
```

### Hidden Cost: Reading Requires Third-Party

If you need barcode reading with DevExpress, you must purchase or integrate a third-party library:

```
DevExpress DXperience + ZXing.Net:
  DevExpress: $3,399 (5-year)
  ZXing.Net: $0 (open source)
  Integration effort: Varies
  Limitation: Manual format specification, no ML correction

DevExpress DXperience + IronBarcode (hybrid):
  DevExpress: $3,399 (5-year)
  IronBarcode: $749
  Total: $4,148
  At which point: Why not just use IronBarcode for generation too?
```

---

## When to Use Each

### Choose DevExpress Barcode When:

1. **You already own DevExpress subscriptions** - If your organization already licenses DevExpress for DataGrids, Reporting, Schedulers, and other controls, the barcode component is included at no additional cost.

2. **You need barcode in DevExpress Reports** - If you're generating printed reports with DevExpress Reporting and need embedded barcodes, the integration is straightforward within that ecosystem.

3. **You only need generation** - If your requirement is solely to display generated barcodes in UI forms and never read/scan them, DevExpress covers that use case.

4. **Your team knows DevExpress APIs** - If your development team is already experienced with DevExpress patterns and conventions, there's less learning curve.

### Choose IronBarcode When:

1. **You need reading AND writing** - If your workflow involves both generating barcodes and reading them from images or documents, IronBarcode provides both in a single library. DevExpress cannot do reading.

2. **You only need barcode functionality** - If you don't need DataGrids, Charts, Schedulers, and 200+ other controls, paying $749 once for IronBarcode is more economical than $1,199+/year for DevExpress.

3. **You prefer perpetual licensing** - If you want to pay once and own your license rather than annual subscription renewals, IronBarcode's perpetual model provides budget certainty.

4. **You process PDFs** - If you extract barcodes from PDF documents or generate barcodes directly to PDF, IronBarcode handles this natively. DevExpress requires their Reporting product for PDF barcode generation.

5. **You're building backend services** - If your barcode processing happens in console apps, Web APIs, Azure Functions, or serverless environments rather than UI forms, IronBarcode's library approach is more appropriate than UI controls.

6. **You need automatic format detection** - If you process barcodes where you don't know the format in advance, IronBarcode's ML-powered detection eliminates manual symbology specification.

---

## Migration Guide

### Why Developers Migrate

Common reasons for moving from DevExpress Barcode to IronBarcode:

| Symptom | Root Cause | IronBarcode Solution |
|---------|------------|---------------------|
| "We need to read barcodes" | DevExpress is generation-only | Read + Write in single library |
| "Subscription costs add up" | No perpetual option | One-time perpetual license |
| "We only use barcode from the suite" | Suite bundling | Standalone focused product |
| "Module/DPI calculation is complex" | Manual symbology config | Automatic sizing |
| "Need barcode in non-UI contexts" | UI control architecture | Library-first design |

### Package Migration

**Remove DevExpress (platform specific):**

```xml
<!-- Remove from .csproj -->
<PackageReference Include="DevExpress.Win.BarcodeControl" Version="xx.x.x" />
<!-- Or remove nuget.config private feed -->
```

**Add IronBarcode:**

```xml
<PackageReference Include="IronBarcode" Version="2024.x.x" />
```

Or via CLI:

```bash
dotnet remove package DevExpress.Win.BarcodeControl
dotnet add package IronBarcode
```

### API Mapping Reference

#### Generation Classes

| DevExpress | IronBarcode | Notes |
|------------|-------------|-------|
| `BarCodeControl` | `BarcodeWriter.CreateBarcode()` | Control vs factory |
| `new Code128Generator()` | `BarcodeEncoding.Code128` | Class vs enum |
| `new QRCodeGenerator()` | `QRCodeWriter.CreateQrCode()` | Class vs method |
| `.Text = "value"` | Parameter in method | Property vs parameter |
| `.Module` (DPI) | `.ResizeTo()` | Manual vs automatic |

#### Code Migration Example

**Before (DevExpress WinForms):**

```csharp
using DevExpress.XtraBars.BarCode;
using DevExpress.XtraBars.BarCode.Symbologies;

// Create control
var barCode = new BarCodeControl();

// Configure symbology
var symbology = new Code128Generator();
symbology.CharacterSet = Code128CharacterSet.CharsetAuto;
barCode.Symbology = symbology;

// Set value
barCode.Text = "12345678";

// Configure appearance
barCode.AutoModule = true;
barCode.ShowText = true;

// Cannot easily save to file - need PrintingSystem export
// or DrawToBitmap approach
```

**After (IronBarcode):**

```csharp
using IronBarCode;

// One line to generate with defaults
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");

// Or with customization
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .AddAnnotationTextBelowBarcode("12345678")
    .SaveAsPng("barcode.png");
```

**Changes:**
- Fewer lines (2-4 vs 8+)
- Direct file save instead of UI control approach
- No symbology class instantiation
- Automatic sizing instead of Module calculation

### Migration Checklist

**Pre-Migration:**
- [ ] Inventory all DevExpress barcode usage points
- [ ] Check if reading capability is needed (DevExpress can't do it)
- [ ] Verify you're not using other DevExpress controls (or migrate separately)
- [ ] Obtain IronBarcode license

**Migration:**
- [ ] Remove DevExpress barcode packages
- [ ] Add IronBarcode NuGet package
- [ ] Replace BarCodeControl with BarcodeWriter calls
- [ ] Replace Symbology classes with BarcodeEncoding enums
- [ ] Add reading code where now possible
- [ ] Remove Module/DPI calculations (IronBarcode auto-sizes)

**Post-Migration:**
- [ ] Test all barcode generation scenarios
- [ ] Test barcode reading scenarios (new capability)
- [ ] Remove DevExpress NuGet feed if no longer needed
- [ ] Update license documentation

---

## Additional Resources

### Documentation Links

- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/) - Official guides and API reference
- [IronBarcode on NuGet](https://www.nuget.org/packages/BarCode) - Package download
- [DevExpress Barcode Documentation](https://docs.devexpress.com/WindowsForms/DevExpress.XtraBars.BarCode.BarCodeControl) - Official DevExpress guides

### Code Example Files

Working code demonstrating the concepts in this article:

- [Generation-Only Limitation](devexpress-generation-only.cs) - Reading capability comparison
- [Symbology Configuration](devexpress-symbology-config.cs) - Manual vs automatic configuration

### Related Comparisons

- [Aspose.BarCode Comparison](../aspose-barcode/) - Enterprise subscription alternative
- [Syncfusion Barcode Comparison](../syncfusion-barcode/) - Another UI suite component
- [ZXing.Net Comparison](../zxing-net/) - Open-source alternative

---

*Last verified: January 2026*
*Author: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*
