# Migrating from Syncfusion Barcode to IronBarcode

Most teams migrating away from Syncfusion barcode are not dissatisfied with it as a UI control. It generates barcodes and displays them on forms. The problem arrives when a new requirement appears — scan barcodes from uploaded images, extract barcodes from PDF documents, generate barcode files in a serverless function — and Syncfusion's `SfBarcode` and `SfBarcodeGenerator` have no reading API, no PDF output, and no headless generation path. At that point, you need a different library. Once a library that reads barcodes is in the project anyway, it almost always makes sense to consolidate generation there too rather than maintaining two separate tools.

The other common trigger is the Community License boundary. Syncfusion's Community License requires all four of these conditions simultaneously: annual gross revenue below $1,000,000, five or fewer developers, ten or fewer total employees, and total outside capital raised below $3,000,000. Government organizations are ineligible regardless of size. A startup that crosses any one threshold on a Monday morning owes a commercial license by that afternoon. The transition goes from $0 to $995+ per developer per year — and that is the Standard tier for a single platform. Enterprise is $2,995 per developer per year. IronBarcode's [pricing model](https://ironsoftware.com/csharp/barcode/licensing/) has no thresholds that flip on company size or funding rounds.

This guide covers replacing Syncfusion barcode controls with IronBarcode across WinForms, Blazor, and MAUI projects. If your project uses other Syncfusion controls — grids, charts, schedulers — leave those packages alone. Only the barcode-specific code changes here.

## Why Migrate

Beyond the reading gap, two other friction points come up regularly.

The first is version-specific license keys. Syncfusion ties license keys to specific Essential Studio versions. Upgrading from version 24.x to 25.x means getting a new key from the account portal, updating it in every environment's secrets, and redeploying. A CI/CD pipeline that runs fine after a NuGet version pin can silently fail — with trial watermarks in production — if the key is not also updated. IronBarcode's [license key registration](https://ironsoftware.com/csharp/barcode/get-started/license-keys/) is not version-specific within a major release. Update the NuGet package; the key stays the same.

The second is platform boilerplate. A Syncfusion Blazor application requires `SyncfusionLicenseProvider.RegisterLicense("KEY")` before any control is instantiated, `builder.Services.AddSyncfusionBlazor()` in `Program.cs`, and `@using Syncfusion.Blazor.BarcodeGenerator` in `_Imports.razor`. For MAUI, `builder.ConfigureSyncfusionCore()` is also required. IronBarcode's equivalent is one line at startup, identical across all platforms.

## Step 1: Remove Syncfusion Barcode Packages

Run the removal command for the platform(s) in use:

```bash
# Blazor
dotnet remove package Syncfusion.Blazor.BarcodeGenerator

# WinForms
dotnet remove package Syncfusion.Barcode.WinForms

# WPF
dotnet remove package Syncfusion.SfBarcode.WPF

# MAUI
dotnet remove package Syncfusion.Maui.Barcode
```

If the project uses other Syncfusion controls (grid, chart, etc.), do not remove the core Syncfusion packages. Only remove the barcode-specific package listed above. The `Syncfusion.Licensing` package can be removed only if no other Syncfusion packages remain.

## Step 2: Install IronBarcode

```bash
dotnet add package IronBarcode
```

One package. Works in WinForms, WPF, Blazor, MAUI, ASP.NET Core, console applications, Azure Functions, and Docker containers.

## Step 3: Replace License Registration and Namespace Imports

Remove the Syncfusion license and service registration code:

```csharp
// Remove this line wherever it appears (Program.cs, App.xaml.cs, etc.)
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR-KEY");

// Remove from Program.cs in Blazor projects (only if barcode was the only Syncfusion component)
builder.Services.AddSyncfusionBlazor();

// Remove from _Imports.razor
// @using Syncfusion.Blazor
// @using Syncfusion.Blazor.BarcodeGenerator

// Remove from MAUI MauiProgram.cs (only if no other Syncfusion components remain)
builder.ConfigureSyncfusionCore();
```

Add IronBarcode license activation at the application entry point:

```csharp
// In Program.cs, App.xaml.cs, or host builder — one line, all platforms
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

Add the IronBarcode namespace import to any file that uses barcode functionality:

```csharp
using IronBarCode;
```

For CI/CD and Docker deployments, storing the key in an environment variable is straightforward:

```csharp
IronBarCode.License.LicenseKey = Environment.GetEnvironmentVariable("IRONBARCODE_LICENSE");
```

## Code Migration Examples

### WinForms: SfBarcode to BarcodeWriter

The WinForms migration replaces the `SfBarcode` control — and the `DrawToBitmap` pattern required to get a file out of it — with a direct `SaveAsPng` call.

**Before — Syncfusion WinForms:**

```csharp
using Syncfusion.Windows.Forms.Barcode;
using System.Drawing;
using System.Drawing.Imaging;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR-KEY");

var barcode = new SfBarcode();
barcode.Text = "12345678";
barcode.Symbology = BarcodeSymbolType.Code128A;
barcode.BarHeight = 100;
barcode.NarrowBarWidth = 1;
barcode.ShowText = true;

barcode.Width = 400;
barcode.Height = 150;
using var bitmap = new Bitmap(barcode.Width, barcode.Height);
barcode.DrawToBitmap(bitmap, barcode.ClientRectangle);
bitmap.Save("barcode.png", ImageFormat.Png);
```

**After — IronBarcode:**

```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .ResizeTo(400, 150)
    .SaveAsPng("barcode.png");
```

The `BarcodeSymbolType.Code128A` enum maps to `BarcodeEncoding.Code128`. The `DrawToBitmap` + `Bitmap` allocation + `bitmap.Save` pattern collapses into `.SaveAsPng()`. The full range of [1D barcode formats](https://ironsoftware.com/csharp/barcode/how-to/create-1d-barcodes/) available in IronBarcode goes well beyond what Syncfusion supports. If you need to display the generated barcode on a WinForms form, generate to bytes and set a `PictureBox`:

```csharp
using IronBarCode;
using System.Drawing;
using System.IO;

private void UpdateBarcodeDisplay(string value)
{
    var bytes = BarcodeWriter.CreateBarcode(value, BarcodeEncoding.Code128)
        .ResizeTo(400, 150)
        .ToPngBinaryData();

    using var ms = new MemoryStream(bytes);
    pictureBox1.Image = Image.FromStream(ms);
}
```

### Blazor: SfBarcodeGenerator Component to Server-Side API

The Blazor migration is a conceptual shift. `SfBarcodeGenerator` is a Razor component that renders a barcode in the browser. IronBarcode has no Razor component — it is a server-side generation library. The replacement pattern is to generate the barcode server-side and return it as an image source.

**Before — Syncfusion Blazor component:**

```razor
@page "/labels"
@using Syncfusion.Blazor.BarcodeGenerator

<SfBarcodeGenerator
    Width="300px"
    Height="150px"
    Type="BarcodeType.Code128"
    Value="@trackingNumber">
    <BarcodeGeneratorDisplayText Visibility="true"></BarcodeGeneratorDisplayText>
</SfBarcodeGenerator>

@code {
    private string trackingNumber = "12345678";
}
```

**After — IronBarcode as image endpoint:**

In `Program.cs` or a controller, add a barcode image endpoint:

```csharp
using IronBarCode;

app.MapGet("/barcode/{value}", (string value) =>
{
    var bytes = BarcodeWriter.CreateBarcode(value, BarcodeEncoding.Code128)
        .ResizeTo(300, 150)
        .ToPngBinaryData();

    return Results.File(bytes, "image/png");
});
```

In your Blazor component, reference the endpoint as an image source:

```razor
@page "/labels"

<img src="/barcode/@trackingNumber" alt="Barcode: @trackingNumber" />

@code {
    private string trackingNumber = "12345678";
}
```

This approach works in Blazor Server and Blazor WebAssembly (with a hosted API backend). It also means your barcode generation code is fully testable as an HTTP endpoint and reusable from any client — not locked to a Razor component.

### Blazor: SfQRCodeGenerator to QRCodeWriter

**Before — Syncfusion QR Blazor component:**

```razor
@using Syncfusion.Blazor.BarcodeGenerator

<SfQRCodeGenerator
    Width="300px"
    Height="300px"
    Value="https://example.com">
    <QRCodeGeneratorDisplayText Visibility="false">
    </QRCodeGeneratorDisplayText>
</SfQRCodeGenerator>
```

**After — IronBarcode QR endpoint:**

```csharp
// In Program.cs
app.MapGet("/qr/{data}", (string data) =>
{
    var bytes = QRCodeWriter.CreateQrCode(data, 300, QRCodeWriter.QrErrorCorrectionLevel.Highest)
        .ToPngBinaryData();

    return Results.File(bytes, "image/png");
});
```

```razor
@* In your Blazor component *@
<img src="/qr/@Uri.EscapeDataString(targetUrl)" alt="QR Code" />
```

QR codes with a brand logo — not possible with Syncfusion:

```csharp
app.MapGet("/qr-branded/{data}", (string data) =>
{
    var bytes = QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest)
        .AddBrandLogo("wwwroot/logo.png")
        .ToPngBinaryData();

    return Results.File(bytes, "image/png");
});
```

### Adding Reading Capability (Replaces OPX)

If you were using Syncfusion Barcode Reader OPX — or planning to purchase it — IronBarcode replaces it with the same NuGet package already installed. No second product. No second license. No ZXing wrapper to manage.

```csharp
using IronBarCode;

// Read from an image file — automatic format detection
var results = BarcodeReader.Read("uploaded-scan.png");
foreach (var result in results)
{
    Console.WriteLine($"Format: {result.Format}");
    Console.WriteLine($"Value: {result.Value}");
}
```

The [barcode reading documentation](https://ironsoftware.com/csharp/barcode/how-to/read-barcodes-from-images/) covers reading from byte arrays (for web upload handlers), reading with speed/accuracy tradeoffs, and multi-barcode detection options.

Reading from PDF documents is equally direct — no need for a separate PDF library:

```csharp
using IronBarCode;

// Read all barcodes from all pages of a PDF
var pdfResults = BarcodeReader.Read("shipping-manifest.pdf");
foreach (var result in pdfResults)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
}
```

### Generating Barcodes Directly to PDF

Syncfusion barcode controls have no PDF output path. IronBarcode has `.SaveAsPdf()` as a first-class output format — see the [barcode PDF generation guide](https://ironsoftware.com/csharp/barcode/how-to/create-barcode-as-pdf/) for multi-page and embedded-barcode options:

```csharp
using IronBarCode;

// Generate a barcode and save it as a PDF
BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
    .ResizeTo(400, 150)
    .SaveAsPdf("barcode-label.pdf");
```

## API Mapping Reference

| Syncfusion | IronBarcode |
|---|---|
| `SyncfusionLicenseProvider.RegisterLicense("KEY")` | `IronBarCode.License.LicenseKey = "key"` |
| `builder.Services.AddSyncfusionBlazor()` | Not needed |
| `builder.ConfigureSyncfusionCore()` | Not needed |
| `new SfBarcode()` | Static — `BarcodeWriter.CreateBarcode()` |
| `barcode.Text = "12345678"` | First parameter of `CreateBarcode` |
| `barcode.Symbology = BarcodeSymbolType.Code128A` | `BarcodeEncoding.Code128` (parameter) |
| `barcode.Symbology = BarcodeSymbolType.QRBarcode` | Use `QRCodeWriter.CreateQrCode()` instead |
| `barcode.BarHeight = 100` | `.ResizeTo(width, 100)` |
| `barcode.ShowText = true` | `.AddBarcodeText()` |
| `barcode.DrawToBitmap(bitmap, rect)` | `.SaveAsPng(path)` |
| Manual `Bitmap` → `MemoryStream` | `.ToPngBinaryData()` |
| `<SfBarcodeGenerator Type="BarcodeType.Code128" Value="...">` | `BarcodeWriter.CreateBarcode(value, BarcodeEncoding.Code128)` server-side |
| `<SfQRCodeGenerator Value="...">` | `QRCodeWriter.CreateQrCode(value, size)` server-side |
| `BarcodeType.Code128` (Blazor enum) | `BarcodeEncoding.Code128` |
| No reading API — requires separate Barcode Reader OPX | `BarcodeReader.Read(path)` |
| Barcode Reader OPX (wraps ZXing.Net) | `BarcodeReader.Read(path)` — native, no wrapper |

## Migration Checklist

Use these grep patterns to locate every file that needs updating:

```bash
grep -r "SyncfusionLicenseProvider.RegisterLicense\|AddSyncfusionBlazor\|ConfigureSyncfusionCore" --include="*.cs" .
grep -r "new SfBarcode\(\)\|SfBarcodeGenerator\|SfQRCodeGenerator" --include="*.cs" --include="*.razor" .
grep -r "BarcodeSymbolType\.\|BarcodeType\." --include="*.cs" --include="*.razor" .
grep -r "DrawToBitmap\|barcode\.Text\s*=" --include="*.cs" .
```

Work through each match:

- Replace `Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("KEY")` with `IronBarCode.License.LicenseKey = "KEY"` at application startup
- Remove `builder.Services.AddSyncfusionBlazor()` if no other Syncfusion components remain in the project
- Remove `builder.ConfigureSyncfusionCore()` if no other Syncfusion components remain in the project
- Remove `@using Syncfusion.Blazor` and `@using Syncfusion.Blazor.BarcodeGenerator` from `_Imports.razor`
- Replace `using Syncfusion.Windows.Forms.Barcode` and `using Syncfusion.Licensing` with `using IronBarCode`
- Replace `new SfBarcode()` + property assignments + `DrawToBitmap` with `BarcodeWriter.CreateBarcode(value, BarcodeEncoding.X).ResizeTo(w, h).SaveAsPng(path)`
- Replace `<SfBarcodeGenerator>` Razor components with a server-side image endpoint and an `<img src="...">` tag
- Replace `<SfQRCodeGenerator>` Razor components with `QRCodeWriter.CreateQrCode()` endpoint
- Replace `BarcodeSymbolType.Code128A` (and similar) with `BarcodeEncoding.Code128`
- Replace `BarcodeType.Code128` (Blazor enum) with `BarcodeEncoding.Code128`
- Add `BarcodeReader.Read()` wherever reading capability was previously deferred to Barcode Reader OPX or a separate ZXing integration

After all barcode references are replaced, remove the Syncfusion barcode NuGet package(s) from the project. If no other Syncfusion packages remain, remove the `Syncfusion.Licensing` package as well.

---

The migration resolves a structural problem that Syncfusion's architecture cannot fix: `SfBarcode` and `SfBarcodeGenerator` are UI rendering controls. They cannot read barcodes. To read barcodes, you buy Barcode Reader OPX, which wraps a library that is free to use without Syncfusion. IronBarcode handles both generation and reading in one package, through one API, under one license — without that layer of paid indirection.

