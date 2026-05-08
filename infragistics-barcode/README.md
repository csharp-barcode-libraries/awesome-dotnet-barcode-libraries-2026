# Infragistics Barcode vs IronBarcode: C# Barcode Comparison 2026

*By [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*

Infragistics provides barcode functionality through its Ultimate UI suite, with UltraWinBarcode for WinForms generation and a separate BarcodeReader assembly for WPF recognition. The key limitation: barcode reading is only available on WPF, not WinForms or any web platform. This guide examines Infragistics' fragmented platform support, event-driven API complexity, and how [IronBarcode](https://ironsoftware.com/csharp/barcode/) provides consistent synchronous barcode processing across all platforms.

## Table of Contents

1. [What is Infragistics Barcode?](#what-is-infragistics-barcode)
2. [Suite Bundling Context](#suite-bundling-context)
3. [Platform Fragmentation](#platform-fragmentation)
4. [Installation and Setup](#installation-and-setup)
5. [Code Comparison: WPF-Only Reading](#code-comparison-wpf-only-reading)
6. [Code Comparison: Event-Driven vs Synchronous API](#code-comparison-event-driven-vs-synchronous-api)
7. [Pricing Analysis](#pricing-analysis)
8. [When to Use Each Option](#when-to-use-each-option)
9. [Migration Guide: Infragistics to IronBarcode](#migration-guide-infragistics-to-ironbarcode)

---

## What is Infragistics Barcode?

Infragistics is an enterprise software company that has provided .NET UI controls since 2000. Their barcode functionality is split across two separate components within the Infragistics Ultimate suite:

### UltraWinBarcode (WinForms)

The WinForms barcode component provides generation capabilities:

- Creates barcode images for 1D and 2D formats
- Integrates with WinForms forms and reports
- Supports Code 128, Code 39, EAN, UPC, QR Code, DataMatrix
- **Cannot read barcodes** - generation only

### BarcodeReader (WPF Only)

A separate assembly provides barcode recognition:

- Class: `Infragistics.Controls.Barcodes.BarcodeReader` (Ultimate UI for WPF)
- Available only in the WPF product
- Supports six symbology families: QR, EAN/UPC, Code 39 (including Code 39 Extended), Code 128, MaxiCode, and Interleaved 2 of 5 — DataMatrix is NOT supported by the reader
- Uses event-driven API pattern (`DecodeComplete` callback)
- Can decode from `BitmapSource` images or webcam frames

### The Critical Limitation

Infragistics barcode reading is only available on WPF. WinForms applications, despite having UltraWinBarcode for generation, have no reading capability. Web applications (ASP.NET, Blazor) have neither generation nor reading in the same manner as desktop.

This creates a fragmented experience:
- **WinForms**: Generate only
- **WPF**: Generate and read
- **Web**: Requires different approach entirely

---

## Suite Bundling Context

### Infragistics Pricing Structure

Infragistics sells products primarily through the Ultimate subscription:

| Product | Price (2026) | Includes |
|---------|-------------|----------|
| Infragistics Ultimate | ~$2,300+/year per developer (v25.2 listed at ~$2,307–$2,351 on ComponentSource) | All platforms |
| Infragistics Professional | ~$1,599/year per developer | Web/desktop UI components (no design tools) |
| Ignite UI | Separate web-only bundle | Angular/React/Web Components/Blazor |

*Pricing as of early 2026 from ComponentSource. The Infragistics public pricing page does not display per-developer figures — confirm with sales for your tier.*

Individual platform packages (like WPF-only) are available but often not much cheaper than Ultimate, pushing developers toward the full bundle.

### What You're Buying for Barcode Functionality

When purchasing Infragistics Ultimate at ~$2,300+/year for barcode features:

```
Infragistics Ultimate Contents:
├── Ignite UI for Angular (~100 components)
├── Ignite UI for React (~100 components)
├── Ignite UI for Web Components
├── Ignite UI for Blazor (~60 components)
├── Ultimate UI for WPF (~130 controls)
│   └── BarcodeReader (reading capability HERE)
├── Ultimate UI for WinForms (~150 controls)
│   └── UltraWinBarcode (generation only)
├── Ultimate UI for Xamarin
├── Various charting, grid, and scheduling controls
└── Design tools (App Builder, Indigo.Design)

Barcode-related components:
├── UltraWinBarcode (WinForms generation)
├── XamBarcode (WPF generation)
├── BarcodeReader (WPF reading ONLY)
└── Total: ~2-3% of suite value
```

### The Lock-In Problem

The suite bundling creates several concerns:

**Platform Coverage Gaps:**
Even after purchasing Ultimate, you still can't read barcodes on WinForms or web platforms. The WPF-only reader doesn't help if your application targets other frameworks.

**Renewal Dependency:**
The annual subscription must continue for updates and support. Stopping payment means no new features and potentially no bug fixes.

**Value Proposition:**
If you only need barcode functionality, you're paying for 100+ controls you may never use, and you still don't get consistent reading across platforms.

---

## Platform Fragmentation

This is Infragistics' most significant barcode limitation:

| Platform | Generation | Reading | Notes |
|----------|------------|---------|-------|
| WPF | Yes (XamBarcode) | Yes (BarcodeReader) | Full capability |
| WinForms | Yes (UltraWinBarcode) | **No** | Generation only |
| Blazor | Via Ignite UI | **No** | Client-side generation only |
| ASP.NET | N/A | **No** | No server-side support |
| .NET Core Console | N/A | **No** | Not supported |

Compared to IronBarcode:

| Platform | Generation | Reading | Notes |
|----------|------------|---------|-------|
| WPF | Yes | Yes | Full capability |
| WinForms | Yes | Yes | Full capability |
| .NET 6/7/8 | Yes | Yes | Full capability |
| Blazor Server | Yes | Yes | Full capability |
| ASP.NET Core | Yes | Yes | Full capability |
| Console | Yes | Yes | Full capability |
| Docker/Linux | Yes | Yes | Full capability |
| Azure Functions | Yes | Yes | Full capability |

The disparity is significant: Infragistics provides reading on exactly one platform (WPF), while IronBarcode provides consistent capabilities everywhere.

### Format Support Comparison

| Feature | Infragistics | IronBarcode |
|---------|-------------|-------------|
| Symbology families (Read) | 6 (QR, EAN/UPC, Code 39, Code 128, MaxiCode, Interleaved 2 of 5) | 30+ |
| Symbologies (Write) | ~20 (XamBarcode + UltraWinBarcode combined) | 30+ |
| QR Code Reading | Yes (WPF only) | Yes (everywhere) |
| DataMatrix Reading | **No** (not supported by the WPF reader) | Yes (everywhere) |
| PDF Barcode Reading | No | Yes |
| Automatic Detection | Yes — `Symbology.All` searches every supported family | Yes |
| ML Error Correction | No | Yes |

---

## Installation and Setup

### Infragistics Barcode Installation

**Step 1: Install via the Infragistics installer or private NuGet feed**

Infragistics Ultimate is not on public nuget.org. Distribution is via the platform installer (which drops assemblies under `Program Files`) or via Infragistics' private NuGet feed:

```
https://packages.infragistics.com/nuget/labs/
```

After registering the feed with your credentials, you reference the umbrella WPF or WinForms packages — there is no standalone `Infragistics.WPF.BarcodeReader` package on the public feed.

For WPF with reading capability, the assemblies you need are:

```xml
<Reference Include="InfragisticsWPF.Controls.Barcodes.BarcodeReader" />
<Reference Include="InfragisticsWPF.Controls.Barcodes.XamBarcode" />
```

For WinForms (generation only), the relevant assembly is `Infragistics.Win.UltraWinBarcode` shipped inside the WinForms suite — referenced from the GAC or the install directory.

**Step 2: License Configuration**

Infragistics uses runtime license verification configured through `licenses.licx`:

```csharp
// Build action: licenses.licx file processes embedded license tokens at compile time.
// Runtime checks are performed against your registered serial.
```

### IronBarcode Installation

**Step 1: Single NuGet Package**
```bash
dotnet add package BarCode
```

**Step 2: License Configuration (One Line)**
```csharp
IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";
```

**Step 3: Ready to Use**
```csharp
using IronBarCode;

// Works immediately on any platform
var results = BarcodeReader.Read("barcode.png");
```

### Setup Complexity Comparison

| Setup Step | Infragistics | IronBarcode |
|------------|-------------|-------------|
| Distribution | Installer + private NuGet feed (credentialed) | Public NuGet (`BarCode`) |
| License deployment | `licenses.licx` + serial | Single key string |
| Assembly references | Manual per platform | Automatic |
| Platform configuration | Per-platform setup | None needed |
| Reading capability | Check platform first | Always available |

---

## Code Comparison: WPF-Only Reading

The fundamental issue: Infragistics BarcodeReader only works in WPF applications.

### Scenario: Building a Document Processing Service

**Infragistics Approach (Only Works on WPF):**
```csharp
// This code ONLY works in a WPF application
// Cannot be used in: Console, ASP.NET, WinForms, Blazor, Azure Functions

using Infragistics.Controls.Barcodes;
using System.Windows.Media.Imaging;

namespace InfragisticsWpfReader
{
    public class BarcodeService
    {
        private BarcodeReader _reader;

        public BarcodeService()
        {
            // Initialize reader - WPF assembly required
            _reader = new BarcodeReader();

            // Wire up event handler (async callback pattern)
            _reader.DecodeComplete += OnDecodeComplete;
        }

        public void ReadBarcode(string imagePath)
        {
            // Load image as WPF BitmapSource
            var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

            // Symbology is passed as a Decode() parameter, not a property.
            // The Symbology enum is [Flags]; combine families with bitwise OR.
            // Note: EAN-8/EAN-13/UPC-A/UPC-E share a single EanUpc flag.
            var symbologies = Symbology.Code128 | Symbology.QRCode | Symbology.EanUpc;

            // Start async decode (DecodeAsync) or synchronous (Decode)
            _reader.DecodeAsync(bitmap, symbologies);
            // Result comes via event callback...
        }

        private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
        {
            if (e.SymbolFound)
            {
                Console.WriteLine($"Found: {e.Symbology} = {e.Value}");
            }
        }
    }
}
```

**IronBarcode Approach (Works Everywhere):**
```csharp
// This code works in:
// - Console applications
// - WPF applications
// - WinForms applications
// - ASP.NET Core
// - Blazor Server
// - Azure Functions
// - Docker containers
// - Linux servers

using IronBarCode;

namespace UniversalBarcodeService
{
    public class BarcodeService
    {
        public string ReadBarcode(string imagePath)
        {
            // Synchronous, one-line API
            // Automatic format detection - no specification needed
            var results = BarcodeReader.Read(imagePath);

            return results.FirstOrDefault()?.Value ?? "No barcode found";
        }

        public string[] ReadFromPdf(string pdfPath)
        {
            // PDF support not available in Infragistics
            var results = BarcodeReader.Read(pdfPath);
            return results.Select(r => r.Value).ToArray();
        }
    }
}
```

For detailed WPF-only limitation examples, see: [WPF-Only Reader Code Example](infragistics-wpf-only-reader.cs)

---

## Code Comparison: Event-Driven vs Synchronous API

Even on WPF where Infragistics reading works, the API uses an event-driven pattern that adds complexity.

### The Event-Driven Pattern Problem

**Infragistics BarcodeReader (Event Callbacks):**
```csharp
using Infragistics.Controls.Barcodes;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

public class InfragisticsBarcodeReader
{
    private BarcodeReader _reader;
    private TaskCompletionSource<string> _resultSource;

    public InfragisticsBarcodeReader()
    {
        _reader = new BarcodeReader();
        _reader.DecodeComplete += OnDecodeComplete;
    }

    // Must convert to async/await pattern for usability
    public async Task<string> ReadBarcodeAsync(string imagePath)
    {
        _resultSource = new TaskCompletionSource<string>();

        var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

        // Symbology is a Decode parameter, not a reader property.
        // Note: DataMatrix is NOT in the Symbology enum — the WPF reader
        // does not support DataMatrix. Code 39 is exposed as Code39Ext.
        var symbologies = Symbology.Code128 | Symbology.Code39Ext |
                          Symbology.QRCode | Symbology.EanUpc;

        // Trigger async decode
        _reader.DecodeAsync(bitmap, symbologies);

        // Wait for callback
        return await _resultSource.Task;
    }

    private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
    {
        if (e.SymbolFound)
        {
            _resultSource?.TrySetResult(e.Value);
        }
        else
        {
            _resultSource?.TrySetResult("No barcode found");
        }
    }

    // For batch processing, complexity multiplies
    public async Task<Dictionary<string, string>> ReadMultipleAsync(string[] imagePaths)
    {
        var results = new Dictionary<string, string>();

        foreach (var path in imagePaths)
        {
            // Must await each one sequentially due to event pattern
            var value = await ReadBarcodeAsync(path);
            results[path] = value;
        }

        return results;
    }
}
```

**IronBarcode (Synchronous and Simple):**
```csharp
using IronBarCode;
using System.Collections.Generic;
using System.Linq;

public class IronBarcodeReader
{
    // Simple synchronous read
    public string ReadBarcode(string imagePath)
    {
        var results = BarcodeReader.Read(imagePath);
        return results.FirstOrDefault()?.Value ?? "No barcode found";
    }

    // Batch processing is trivial
    public Dictionary<string, string> ReadMultiple(string[] imagePaths)
    {
        // Pass array directly - IronBarcode handles parallelization
        var results = BarcodeReader.Read(imagePaths);

        return results
            .GroupBy(r => r.InputPath ?? "")
            .ToDictionary(
                g => g.Key,
                g => g.First().Value
            );
    }

    // Async version if needed (for UI responsiveness)
    public async Task<string> ReadBarcodeAsync(string imagePath)
    {
        return await Task.Run(() =>
        {
            var results = BarcodeReader.Read(imagePath);
            return results.FirstOrDefault()?.Value ?? "No barcode found";
        });
    }
}
```

### Line Count Comparison

| Operation | Infragistics Lines | IronBarcode Lines |
|-----------|-------------------|-------------------|
| Setup | 6 | 0 |
| Single read | 15+ | 2 |
| Batch read | 25+ | 5 |
| Async wrapper | Required (15+) | Optional (3) |

For detailed event-driven API comparison, see: [Event-Driven API Code Example](infragistics-event-driven-api.cs)

---

## Pricing Analysis

### Direct Cost Comparison

| Scenario | Infragistics (Annual) | IronBarcode |
|----------|----------------------|-------------|
| Single developer | ~$2,300+/year (Ultimate) | $799 one-time (Lite) |
| 10 developers | ~$23,000+/year | $2,399 one-time (Pro covers up to 10 devs) |
| 5-year TCO (10 developers) | ~$115,000+ | $2,399 |

*Note: Infragistics Ultimate pricing per ComponentSource for v25.2 (~$2,307–$2,351 per developer per year). IronBarcode tiers: $799 Lite / $1,199 Plus / $2,399 Pro / $4,799 Unlimited (perpetual).*

### 5-Year Total Cost Analysis

For a team needing barcode functionality:

```
Infragistics Ultimate (10 developers, ~$2,300/dev/year):
  Year 1: ~$23,000
  Year 2: ~$23,000
  Year 3: ~$23,000
  Year 4: ~$23,000
  Year 5: ~$23,000
  ─────────────────
  Total: ~$115,000

  Note: Reading still only works on WPF!

IronBarcode Pro (10 developers):
  Year 1: $2,399 (one-time)
  Year 2: $0
  Year 3: $0
  Year 4: $0
  Year 5: $0
  ─────────────────
  Total: $2,399

  Includes: Full reading on ALL platforms

Savings with IronBarcode over 5 years: ~$112,000
```

### Cost Per Feature Analysis

| Feature Needed | Infragistics | IronBarcode |
|----------------|-------------|-------------|
| WPF generation | ~$2,300/year | $799 one-time |
| WPF reading | ~$2,300/year | $799 one-time |
| WinForms generation | ~$2,300/year | $799 one-time |
| WinForms reading | **Not available** | $799 one-time |
| ASP.NET reading | **Not available** | $799 one-time |
| PDF reading | **Not available** | $799 one-time |

With IronBarcode, $799 (Lite) covers a single developer across all platforms and features. With Infragistics, ~$2,300+/year still leaves WinForms and web without reading capability.

---

## When to Use Each Option

### Choose Infragistics When:

1. **You're already invested in Infragistics Ultimate** - If your organization uses many Infragistics controls and has existing subscriptions, adding barcode functionality has zero marginal cost.

2. **Your application is WPF-only** - If you exclusively develop WPF applications and never need barcode reading on other platforms, Infragistics BarcodeReader works adequately.

3. **You prefer event-driven patterns** - If your team prefers async callback patterns and your application architecture already uses this style, the Infragistics API may feel familiar.

4. **Enterprise standardization requirements** - If your organization has standardized on Infragistics and adding additional libraries requires extensive approval.

### Choose IronBarcode When:

1. **You need reading on WinForms** - Infragistics has no WinForms barcode reader. IronBarcode reads barcodes on all platforms including WinForms.

2. **You need server-side barcode processing** - ASP.NET Core, Azure Functions, Docker containers, console applications - Infragistics doesn't support these for reading. IronBarcode does.

3. **You prefer synchronous APIs** - IronBarcode's `Read()` method returns results directly, no event wiring or callbacks required.

4. **You need PDF barcode reading** - IronBarcode reads barcodes from PDF documents natively. Infragistics has no PDF support.

5. **You want automatic format detection** - IronBarcode detects barcode types automatically. Infragistics requires you to specify which symbologies to look for.

6. **You prefer perpetual licensing** - One-time purchase vs ongoing subscription.

7. **You only need barcode functionality** - No need to purchase a massive UI suite for one feature.

---

## Migration Guide: Infragistics to IronBarcode

### Package Changes

**Remove Infragistics references:**
```xml
<!-- Remove from .csproj — assemblies referenced from the Infragistics install,
     or PackageReferences pulled from the private feed
     (https://packages.infragistics.com/nuget/labs/) -->
<Reference Include="InfragisticsWPF.Controls.Barcodes.BarcodeReader" />
<Reference Include="InfragisticsWPF.Controls.Barcodes.XamBarcode" />
<Reference Include="Infragistics.Win.UltraWinBarcode" />
```

**Add IronBarcode (public NuGet, package id `BarCode`):**
```xml
<PackageReference Include="BarCode" Version="*" />
```

Or via CLI:
```bash
dotnet add package BarCode
```

### License Configuration Migration

**Remove Infragistics license code:**
```csharp
// Remove license.licx and license initialization
```

**Add IronBarcode license:**
```csharp
// Add at application startup
IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";
```

### API Mapping Reference

#### Barcode Generation

| Infragistics | IronBarcode | Notes |
|-------------|-------------|-------|
| `XamBarcode.Value = "123"` | `BarcodeWriter.CreateBarcode("123", ...)` | Static factory |
| `UltraWinBarcode.Data = "123"` | `BarcodeWriter.CreateBarcode("123", ...)` | Same API |
| `XamBarcode.Symbology` property | `BarcodeEncoding` enum | Similar pattern |

#### Barcode Reading

| Infragistics | IronBarcode | Notes |
|-------------|-------------|-------|
| `BarcodeReader` class | `BarcodeReader.Read()` | Static method |
| `reader.DecodeComplete` event | Not needed | Synchronous return |
| `reader.Decode(bitmap, Symbology.All)` | Automatic | No format specification |
| `reader.Decode(bitmap, ...)` | `BarcodeReader.Read(path)` | Direct file input |
| `e.Value` (in `ReaderDecodeArgs`) | `result.Value` | Same name |
| `e.Symbology` (in `ReaderDecodeArgs`) | `result.BarcodeType` | Renamed |
| Event callback pattern | Direct result | Simpler code flow |

### Code Migration Examples

#### Example 1: Generation Migration

**Before (Infragistics WPF):**
```xml
<ig:XamBarcode x:Name="barcode"
               Symbology="Code128"
               Data="12345678" />
```

**After (IronBarcode):**
```csharp
var barcode = BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128);
barcode.SaveAsPng("barcode.png");

// Or for WPF display:
var bitmapSource = barcode.ToBitmapSource();
```

#### Example 2: Event-Driven to Synchronous

**Before (Infragistics):**
```csharp
private BarcodeReader _reader;
private string _result;

public void Initialize()
{
    _reader = new BarcodeReader();
    _reader.DecodeComplete += OnDecodeComplete;
}

public void ReadBarcode(BitmapSource image)
{
    _reader.DecodeAsync(image, Symbology.QRCode | Symbology.Code128);
    // Result comes later via event...
}

private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
{
    if (e.SymbolFound)
    {
        _result = e.Value;
        // Now you can use _result...
    }
}
```

**After (IronBarcode):**
```csharp
public string ReadBarcode(string imagePath)
{
    // Synchronous, returns immediately
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value ?? "No barcode found";
}
```

#### Example 3: Enabling Non-WPF Reading (Not Possible in Infragistics)

**Before (Infragistics - WinForms):**
```csharp
// Infragistics WinForms has UltraWinBarcode for GENERATION
// There is NO barcode reading capability for WinForms

var barcode = new UltraWinBarcode();
barcode.Symbology = Symbology.Code128;
barcode.Data = "12345678";
// Can generate, but cannot read

// To read in WinForms, you MUST use a different library
```

**After (IronBarcode - WinForms):**
```csharp
// Full read/write on WinForms
var results = BarcodeReader.Read("barcode.png");
var value = results.First().Value;

// Generation also works
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .SaveAsPng("output.png");
```

### Migration Checklist

**Pre-Migration:**
- [ ] Inventory all Infragistics barcode usage (WPF reader, WinForms generation)
- [ ] Identify platforms currently blocked (WinForms reading, server-side)
- [ ] Document symbologies currently used
- [ ] Obtain IronBarcode license

**Migration:**
- [ ] Add IronBarcode NuGet package
- [ ] Configure IronBarcode license
- [ ] Replace generation code (XamBarcode/UltraWinBarcode to BarcodeWriter)
- [ ] Replace event-driven reading with synchronous Read()
- [ ] Remove symbology specification (use auto-detection)
- [ ] Enable previously blocked scenarios (WinForms reading, server-side)
- [ ] Remove Infragistics barcode packages (if not using other Infragistics controls)

**Post-Migration:**
- [ ] Test all barcode generation scenarios
- [ ] Test reading on newly enabled platforms (WinForms, server-side)
- [ ] Test automatic format detection
- [ ] Verify PDF barcode reading (newly enabled)
- [ ] Remove event handler code
- [ ] Update documentation

---

## Additional Resources

### Documentation Links

- [IronBarcode Documentation](https://ironsoftware.com/csharp/barcode/docs/) - Official IronBarcode guides
- [IronBarcode on NuGet (`BarCode`)](https://www.nuget.org/packages/BarCode) - Package download
- [Infragistics BarcodeReader Documentation](https://www.infragistics.com/products/wpf/barcodes/barcode-reader) - Official Infragistics guides

### Code Example Files

Working code demonstrating the concepts in this article:

- [WPF-Only Reader Limitation](infragistics-wpf-only-reader.cs) - Shows platform restriction
- [Event-Driven API Comparison](infragistics-event-driven-api.cs) - Sync vs async patterns

### Related Comparisons

- [Aspose.BarCode Comparison](../aspose-barcode/) - Enterprise alternative comparison
- [ZXing.Net Comparison](../zxing-net/) - Open-source alternative comparison
- [Dynamsoft Comparison](../dynamsoft-barcode/) - Another commercial SDK comparison
- [Telerik Comparison](../telerik-barcode/) - Similar UI suite vendor

---

*Last verified: January 2026*
*Author: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of Iron Software*
