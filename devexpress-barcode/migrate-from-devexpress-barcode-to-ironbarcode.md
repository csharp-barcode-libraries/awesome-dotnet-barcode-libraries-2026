# Migrating from DevExpress Barcode Controls to IronBarcode

If you are using DevExpress for its grid, chart, scheduler, or pivot controls, keep those. This migration is specifically about replacing `BarCodeControl` with a library that can also read barcodes, run headlessly, and deploy outside a UI context. The DevExpress UI controls you rely on for your WinForms or Blazor application are not touched by this migration. Only the barcode-specific code changes.

The typical scenario that prompts this migration is one of three things: a reading requirement arrives and DevExpress cannot satisfy it; a new service needs barcode generation in ASP.NET Core or a cloud function where WinForms assemblies are not present; or suite renewal comes around and the cost-per-feature math on using a full UI toolkit for barcode output alone stops making sense.

## Step 1: Install IronBarcode

```bash
dotnet add package IronBarcode
```

If you are keeping other DevExpress controls in the same project, leave the DevExpress NuGet packages in place. Only the barcode-related code in your source files changes. If barcode generation was the only reason DevExpress appeared in a particular project and you use no other DX controls there, you can remove the DevExpress packages from that project after the migration:

```bash
# Only remove DevExpress packages if no other DX controls are used in this project
dotnet remove package DevExpress.Win.Core
```

## Step 2: Add License Initialization

Add IronBarcode license activation once at application startup — in `Program.cs`, `App.xaml.cs`, or your host builder:

```csharp
// In Program.cs (ASP.NET Core) or application entry point
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

No network call. No error code to check. Local validation.

## Step 3: Replace Barcode-Specific Code

Search your codebase for DevExpress barcode types. Everything else stays:

```bash
# Find barcode-related DevExpress usage — ignore grid, chart, and other DX components
grep -r "BarCodeControl\|Code128Generator\|QRCodeGenerator\|DataMatrixGenerator\|PDF417Generator\|DevExpress.XtraBars.BarCode" --include="*.cs" .
grep -r "barCode\.Module\|DrawToBitmap\|BarCode\.Symbology" --include="*.cs" .
```

The results of this search are exactly what you will replace. Nothing else.

## Code Migration Examples

### Code 128 Generation

This is the most common migration. A `BarCodeControl` with a `Code128Generator` symbology becomes a single `BarcodeWriter.CreateBarcode` call.

**Before — DevExpress WinForms control:**

```csharp
using DevExpress.XtraBars.BarCode;
using DevExpress.XtraBars.BarCode.Symbologies;
using System.Drawing;
using System.Drawing.Imaging;

public void GenerateCode128(string data, string outputPath)
{
    var barCode = new BarCodeControl();
    var symbology = new Code128Generator();
    symbology.CharacterSet = Code128CharacterSet.CharsetAuto;
    barCode.Symbology = symbology;
    barCode.Text = data;
    barCode.Module = 0.02f;
    barCode.ShowText = true;

    barCode.Width = 400;
    barCode.Height = 100;
    var bitmap = new Bitmap(barCode.Width, barCode.Height);
    barCode.DrawToBitmap(bitmap, new Rectangle(0, 0, barCode.Width, barCode.Height));
    bitmap.Save(outputPath, ImageFormat.Png);
    bitmap.Dispose();
}
```

**After — IronBarcode:**

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

public void GenerateCode128(string data, string outputPath)
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .ResizeTo(400, 100)
        .SaveAsPng(outputPath);
}
```

The `barCode.Module = 0.02f` document-unit sizing is gone. `.ResizeTo(400, 100)` takes pixels directly. The manual `Bitmap` allocation and `DrawToBitmap` call are replaced by `.SaveAsPng()`, which handles sizing automatically.

### QR Code Generation

**Before — DevExpress QR with error correction:**

```csharp
using DevExpress.XtraBars.BarCode;
using DevExpress.XtraBars.BarCode.Symbologies;
using System.Drawing;
using System.Drawing.Imaging;

public void GenerateQrCode(string url, string outputPath)
{
    var barCode = new BarCodeControl();
    var symbology = new QRCodeGenerator();
    symbology.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;
    symbology.CompactionMode = QRCodeCompactionMode.AlphaNumeric;
    barCode.Symbology = symbology;
    barCode.Text = url;

    barCode.Width = 500;
    barCode.Height = 500;
    var bitmap = new Bitmap(barCode.Width, barCode.Height);
    barCode.DrawToBitmap(bitmap, new Rectangle(0, 0, barCode.Width, barCode.Height));
    bitmap.Save(outputPath, ImageFormat.Png);
    bitmap.Dispose();
}
```

**After — IronBarcode:**

```csharp
using IronBarCode;

public void GenerateQrCode(string url, string outputPath)
{
    QRCodeWriter.CreateQrCode(url, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest)
        .SaveAsPng(outputPath);
}
```

`QRCodeErrorCorrectionLevel.H` maps to `QRCodeWriter.QrErrorCorrectionLevel.Highest`. The `CompactionMode.AlphaNumeric` setting is handled automatically by IronBarcode based on the content.

**QR code with a brand logo (net-new capability — not possible with DevExpress):**

```csharp
using IronBarCode;

public void GenerateBrandedQrCode(string url, string logoPath, string outputPath)
{
    QRCodeWriter.CreateQrCode(url, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest)
        .AddBrandLogo(logoPath)
        .SaveAsPng(outputPath);
}
```

### Data Matrix Generation

**Before — DevExpress:**

```csharp
using DevExpress.XtraBars.BarCode;
using DevExpress.XtraBars.BarCode.Symbologies;

var barCode = new BarCodeControl();
var symbology = new DataMatrixGenerator();
symbology.MatrixSize = DataMatrixSize.Matrix26x26;
barCode.Symbology = symbology;
barCode.Text = "PART-7734-X";
// ... DrawToBitmap pattern
```

**After — IronBarcode:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("PART-7734-X", BarcodeEncoding.DataMatrix)
    .ResizeTo(260, 260)
    .SaveAsPng("datamatrix.png");
```

### PDF417 Generation

**Before — DevExpress:**

```csharp
using DevExpress.XtraBars.BarCode;
using DevExpress.XtraBars.BarCode.Symbologies;

var barCode = new BarCodeControl();
barCode.Symbology = new PDF417Generator();
barCode.Text = "SHIPMENT-DATA-2026";
// ... DrawToBitmap pattern
```

**After — IronBarcode:**

```csharp
using IronBarCode;

BarcodeWriter.CreateBarcode("SHIPMENT-DATA-2026", BarcodeEncoding.PDF417)
    .ResizeTo(400, 150)
    .SaveAsPng("pdf417.png");
```

### Adding Reading (Net-New Capability)

DevExpress provides no reading API. If a reading requirement has arrived, this is where IronBarcode pays for itself immediately:

```csharp
using IronBarCode;

// Read from an image file
var results = BarcodeReader.Read("uploaded-label.png");
foreach (var result in results)
{
    Console.WriteLine($"Found {result.Format}: {result.Value}");
}

// Read with options for better accuracy on difficult images
var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
    MaxParallelThreads = 4
};
var detailedResults = BarcodeReader.Read("multi-barcode-sheet.png", options);
```

### ASP.NET Core Barcode Endpoint

This was not achievable with `BarCodeControl` without WinForms workarounds. IronBarcode supports this natively:

```csharp
using IronBarCode;

// In Program.cs or a controller
app.MapGet("/label/{sku}", (string sku) =>
{
    var pngBytes = BarcodeWriter.CreateBarcode(sku, BarcodeEncoding.Code128)
        .ResizeTo(400, 100)
        .ToPngBinaryData();

    return Results.File(pngBytes, "image/png", $"{sku}.png");
});

app.MapGet("/qr/{data}", (string data) =>
{
    var pngBytes = QRCodeWriter.CreateQrCode(data, 300, QRCodeWriter.QrErrorCorrectionLevel.Highest)
        .ToPngBinaryData();

    return Results.File(pngBytes, "image/png");
});
```

### Reading Barcodes from PDFs

IronBarcode reads from PDF files natively. No PdfiumViewer, no rendering loop, no extra packages:

```csharp
using IronBarCode;

// Read all barcodes from all pages of a PDF
var results = BarcodeReader.Read("shipping-manifest.pdf");
foreach (var result in results)
{
    Console.WriteLine($"Barcode: {result.Value} | Format: {result.Format}");
}
```

## Common Migration Issues

### barCode.Module Uses Document Units, Not Pixels

`barCode.Module` controls the width of the narrowest bar in document units (dependent on the rendering DPI context). This is not a pixel count. When migrating, do not try to convert the module value to pixels — instead, decide what pixel dimensions you want and use `.ResizeTo(width, height)` directly.

```csharp
// Before: barCode.Module = 0.02f  — document units, indirect sizing
// After:
.ResizeTo(400, 100)  // explicit pixel dimensions
```

### DrawToBitmap Requires Pre-Allocated Bitmap

The old pattern allocated a `Bitmap` of a specific size, then called `DrawToBitmap` to render into it. IronBarcode's `.SaveAsPng()` handles all of this internally. You do not pre-allocate anything.

```csharp
// Before: must know size upfront, allocate, draw, save, dispose
barCode.Width = 400;
barCode.Height = 100;
var bitmap = new Bitmap(barCode.Width, barCode.Height);
barCode.DrawToBitmap(bitmap, new Rectangle(0, 0, barCode.Width, barCode.Height));
bitmap.Save(path, ImageFormat.Png);
bitmap.Dispose();

// After: size specified once, everything else handled
BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
    .ResizeTo(400, 100)
    .SaveAsPng(path);
```

### WinForms Data Binding

If `BarCodeControl` was bound to a data source in a WinForms designer, the IronBarcode equivalent is to read the value from your model explicitly and pass it to `BarcodeWriter.CreateBarcode`. IronBarcode does not participate in WinForms data binding — it is a programmatic API, not a UI control. If you need to display the generated barcode on a form, generate the image bytes and set them on a `PictureBox`:

```csharp
using IronBarCode;

// Generate and display in a WinForms PictureBox
private void UpdateBarcodeDisplay(string value)
{
    var bytes = BarcodeWriter.CreateBarcode(value, BarcodeEncoding.Code128)
        .ResizeTo(400, 100)
        .ToPngBinaryData();

    using var ms = new System.IO.MemoryStream(bytes);
    pictureBox1.Image = System.Drawing.Image.FromStream(ms);
}
```

### Namespace Replacement

Old imports to remove:

```csharp
// Remove these
using DevExpress.XtraBars.BarCode;
using DevExpress.XtraBars.BarCode.Symbologies;
```

New import to add:

```csharp
// Add this
using IronBarCode;
```

## API Mapping Reference

| DevExpress Barcode | IronBarcode Equivalent |
|---|---|
| `new BarCodeControl()` | Static — no instance |
| `new Code128Generator()` + `barCode.Symbology = symbology` | `BarcodeEncoding.Code128` parameter |
| `new QRCodeGenerator()` + `QRCodeErrorCorrectionLevel.H` | `QRCodeWriter.CreateQrCode(data, size, QRCodeWriter.QrErrorCorrectionLevel.Highest)` |
| `new DataMatrixGenerator()` + `DataMatrixSize.Matrix26x26` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.DataMatrix)` |
| `new PDF417Generator()` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.PDF417)` |
| `new AztecGenerator()` | `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Aztec)` |
| `barCode.Text = value` | First argument to `CreateBarcode` or `CreateQrCode` |
| `barCode.Module = 0.02f` | `.ResizeTo(width, height)` in pixels |
| `barCode.ShowText = true` | `.AddBarcodeText()` |
| `DrawToBitmap(bitmap, rect)` | `.SaveAsPng(path)` |
| `new Bitmap(w, h)` + manual disposal | Not needed |
| Bitmap → `MemoryStream` → HTTP | `.ToPngBinaryData()` |
| No reading API | `BarcodeReader.Read(path)` |
| `using DevExpress.XtraBars.BarCode` | `using IronBarCode` |

## Migration Checklist

Use this grep pattern to find every file that needs updating:

```bash
grep -r "BarCodeControl\|Code128Generator\|QRCodeGenerator\|DataMatrixGenerator\|PDF417Generator\|AztecGenerator" --include="*.cs" .
grep -r "barCode\.Module\|barCode\.Symbology\|DrawToBitmap\|DevExpress\.XtraBars\.BarCode" --include="*.cs" .
```

Work through each match:

- Replace `using DevExpress.XtraBars.BarCode;` and `using DevExpress.XtraBars.BarCode.Symbologies;` with `using IronBarCode;`
- Replace `new BarCodeControl()` + symbology setup with `BarcodeWriter.CreateBarcode(data, BarcodeEncoding.X)`
- Replace `new QRCodeGenerator()` + symbology setup with `QRCodeWriter.CreateQrCode(data, size, errorLevel)`
- Replace `barCode.Module = X` with `.ResizeTo(width, height)`
- Replace `DrawToBitmap` + `bitmap.Save` pattern with `.SaveAsPng(path)`
- Replace Bitmap-to-MemoryStream patterns with `.ToPngBinaryData()`
- Add `IronBarCode.License.LicenseKey = "YOUR-KEY"` at application startup
- Add `BarcodeReader.Read()` calls wherever reading capability is needed

If a project uses DevExpress only for barcode controls and no other DX components, remove the DevExpress NuGet packages after all barcode references are replaced. If the project uses DevExpress grid, chart, or other UI controls, leave those packages in place — this migration only touches barcode code.
