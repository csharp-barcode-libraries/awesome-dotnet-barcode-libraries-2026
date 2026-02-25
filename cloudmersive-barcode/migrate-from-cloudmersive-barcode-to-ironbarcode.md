# Migrating from Cloudmersive Barcode API to IronBarcode

This is the simplest migration in .NET barcode history. Remove the HTTP client. Add a NuGet package. Delete the API key management. The rest is a find-and-replace.

Cloudmersive's .NET SDK is a generated REST API client — a thin wrapper around `HttpClient` calls. Every piece of Cloudmersive-specific code in your application is either configuration for that client, calls through that client, or error handling for what happens when network requests fail. None of that infrastructure is needed with IronBarcode, which processes barcodes locally with no network dependency.

---

## Quick Start: Three Steps

### Step 1: Swap the Packages

```bash
dotnet remove package Cloudmersive.APIClient.NETCore.Barcode
dotnet add package IronBarcode
```

### Step 2: Replace Namespaces

```csharp
// Remove these
using Cloudmersive.APIClient.NETCore.Barcode.Api;
using Cloudmersive.APIClient.NETCore.Barcode.Client;

// Add this
using IronBarCode;
```

### Step 3: Replace API Key Setup with License Key

```csharp
// Remove this
Configuration.Default.ApiKey.Add("Apikey", "YOUR-CLOUDMERSIVE-API-KEY");

// Replace with this — set once at application startup
IronBarCode.License.LicenseKey = "YOUR-KEY";
```

In an ASP.NET Core application, the license key goes in `Program.cs` before `builder.Build()`:

```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var builder = WebApplication.CreateBuilder(args);
// ... rest of startup
```

---

## Cost: What You Were Spending vs What You'll Spend

Before migrating, it's worth calculating what the Cloudmersive integration actually costs. The math is straightforward:

| Your Daily Volume | Monthly Requests | Estimated Annual Cost | IronBarcode (one-time) |
|---|---|---|---|
| 100/day | ~3,000 | ~$240/year | $749 |
| 1,000/day | ~30,000 | ~$1,200/year | $749 |
| 5,000/day | ~150,000 | ~$1,800/year | $749 |
| 10,000/day | ~300,000 | ~$3,650/year | $749 |
| 25,000/day | ~750,000 | ~$9,000/year | $1,499 (Plus, 3 devs) |

If you're paying more than $63/month for Cloudmersive, IronBarcode pays for itself within the first year. If you're paying $300/month, IronBarcode pays for itself in 2.5 months. After that, the savings are indefinite — IronBarcode is a perpetual license with no renewal and no per-request charges.

The annual savings at 10,000 barcodes per day: approximately $2,900 in the first year, and $3,650 every year after.

---

## Code Migration Examples

### QR Code Generation

The most common Cloudmersive generation call maps directly to IronBarcode:

**Before (Cloudmersive):**
```csharp
using Cloudmersive.APIClient.NETCore.Barcode.Api;
using Cloudmersive.APIClient.NETCore.Barcode.Client;

Configuration.Default.ApiKey.Add("Apikey", "YOUR-CLOUDMERSIVE-API-KEY");

var apiInstance = new GenerateBarcodeApi();

// Each call = HTTPS request, 100-500ms latency
byte[] result = apiInstance.GenerateBarcodeQRCode("https://example.com");
File.WriteAllBytes("qr.png", result);
```

**After (IronBarcode):**
```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// Local — no network call, no latency
byte[] result = QRCodeWriter.CreateQrCode("https://example.com", 500)
    .ToPngBinaryData();
File.WriteAllBytes("qr.png", result);

// Or save directly
QRCodeWriter.CreateQrCode("https://example.com", 500)
    .SaveAsPng("qr.png");
```

### Code128 Generation

**Before (Cloudmersive):**
```csharp
using Cloudmersive.APIClient.NETCore.Barcode.Api;
using Cloudmersive.APIClient.NETCore.Barcode.Client;

Configuration.Default.ApiKey.Add("Apikey", "YOUR-CLOUDMERSIVE-API-KEY");
var apiInstance = new GenerateBarcodeApi();

byte[] result = apiInstance.GenerateBarcodeCode128By("SHIP-2024031500428");
File.WriteAllBytes("barcode.png", result);
```

**After (IronBarcode):**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

BarcodeWriter.CreateBarcode("SHIP-2024031500428", BarcodeEncoding.Code128)
    .SaveAsPng("barcode.png");

// Or get bytes directly
byte[] result = BarcodeWriter.CreateBarcode("SHIP-2024031500428", BarcodeEncoding.Code128)
    .ToPngBinaryData();
```

### Barcode Reading

**Before (Cloudmersive):**
```csharp
using Cloudmersive.APIClient.NETCore.Barcode.Api;
using Cloudmersive.APIClient.NETCore.Barcode.Client;

Configuration.Default.ApiKey.Add("Apikey", "YOUR-CLOUDMERSIVE-API-KEY");
var scanApi = new BarcodeScanApi();

using (var stream = File.OpenRead("barcode.png"))
{
    var result = scanApi.BarcodeScanImage(stream);
    if (result.Successful == true)
    {
        Console.WriteLine($"Value: {result.RawText}");
        Console.WriteLine($"Type: {result.BarcodeType}");
    }
}
```

**After (IronBarcode):**
```csharp
// NuGet: dotnet add package IronBarcode
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var results = BarcodeReader.Read("barcode.png");
var result = results.First();
Console.WriteLine($"Value: {result.Value}");
Console.WriteLine($"Type: {result.Format}");
```

### Async Barcode Reading

If your Cloudmersive integration uses the async API:

**Before (Cloudmersive):**
```csharp
using Cloudmersive.APIClient.NETCore.Barcode.Api;
using Cloudmersive.APIClient.NETCore.Barcode.Client;

Configuration.Default.ApiKey.Add("Apikey", "YOUR-CLOUDMERSIVE-API-KEY");
var scanApi = new BarcodeScanApi();

using (var stream = File.OpenRead("barcode.png"))
{
    // Async because it's a network call
    var result = await scanApi.BarcodeScanImageAsync(stream);
    return result.Successful == true ? result.RawText : null;
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// IronBarcode is synchronous — no network call means no async needed
// Wrap in Task.Run if you need to free the calling thread for CPU-bound work
var results = await Task.Run(() => BarcodeReader.Read("barcode.png"));
return results.FirstOrDefault()?.Value;
```

### Reading Barcodes from PDF

Cloudmersive has no native PDF support. If your existing code extracts images from PDFs before scanning, replace that entire pipeline with a single IronBarcode call:

**Before (Cloudmersive — requires separate PDF extraction):**
```csharp
// Requires separate PDF library to extract pages as images
// Then one Cloudmersive API call per extracted page image
// Each call = separate HTTPS request = separate latency + billing

var scanApi = new BarcodeScanApi();
foreach (var pageImagePath in ExtractPdfPages("document.pdf"))
{
    using var stream = File.OpenRead(pageImagePath);
    var result = await scanApi.BarcodeScanImageAsync(stream);
    if (result.Successful == true)
    {
        Console.WriteLine($"Found: {result.RawText}");
    }
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

// One call — all pages, all barcodes, no image extraction step
var results = BarcodeReader.Read("document.pdf");
foreach (var result in results)
{
    Console.WriteLine($"Page {result.PageNumber}: {result.Value}");
}
```

### Multi-Barcode Detection

If you have code that scans documents with multiple barcodes:

**Before (Cloudmersive — multiple calls required):**
```csharp
// Cloudmersive returns one barcode per scan — multiple calls for multiple barcodes
// Each call has latency and billing implications
var scanApi = new BarcodeScanApi();
var barcodeValues = new List<string>();

foreach (var croppedRegion in CropBarcodeRegions("invoice.png"))
{
    using var stream = new MemoryStream(croppedRegion);
    var result = await scanApi.BarcodeScanImageAsync(stream);
    if (result.Successful == true)
        barcodeValues.Add(result.RawText);
}
```

**After (IronBarcode):**
```csharp
using IronBarCode;

IronBarCode.License.LicenseKey = "YOUR-KEY";

var options = new BarcodeReaderOptions
{
    Speed = ReadingSpeed.Balanced,
    ExpectMultipleBarcodes = true,
};

// One call — returns all barcodes in the image
var results = BarcodeReader.Read("invoice.png", options);
var barcodeValues = results.Select(r => r.Value).ToList();
```

---

## Removing Infrastructure That's No Longer Needed

This is the part of the migration where you delete more code than you write.

### Remove HTTP Exception Handling

Cloudmersive operations can fail with network errors, HTTP 4xx/5xx responses, and timeout exceptions. This typically produces try/catch infrastructure that IronBarcode doesn't need:

```csharp
// Remove all of this
try
{
    var result = await scanApi.BarcodeScanImageAsync(stream);
    // ...
}
catch (ApiException ex) when (ex.ErrorCode == 429)
{
    // Rate limited — wait and retry
    await Task.Delay(TimeSpan.FromSeconds(2));
    // retry logic...
}
catch (ApiException ex) when (ex.ErrorCode == 503)
{
    // Service unavailable — Cloudmersive is down
    _logger.LogError("Cloudmersive unavailable: {Message}", ex.Message);
    throw;
}
catch (HttpRequestException ex)
{
    // Network error
    _logger.LogError("Network failure: {Message}", ex.Message);
    throw;
}
catch (TaskCanceledException)
{
    // Timeout
    _logger.LogError("Cloudmersive request timed out");
    throw;
}
```

IronBarcode doesn't throw network exceptions. It throws `BarcodeException` if a barcode cannot be decoded from the input, which is a content issue rather than an infrastructure issue. The simplified error handling:

```csharp
using IronBarCode;

var results = BarcodeReader.Read("barcode.png");
if (!results.Any())
{
    _logger.LogWarning("No barcode detected in image");
}
```

### Remove Retry Logic

If your Cloudmersive integration includes retry policies — Polly, custom retry loops, or exponential backoff — remove them entirely:

```csharp
// Remove retry infrastructure
// var retryPolicy = Policy
//     .Handle<ApiException>()
//     .WaitAndRetryAsync(3, retryAttempt =>
//         TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

// IronBarcode — no retry needed
var results = BarcodeReader.Read("barcode.png");
```

### Remove Rate Limit Tracking

If your code tracks API call counts to stay within Cloudmersive's monthly quota:

```csharp
// Remove quota tracking
// private static int _monthlyCallCount = 0;
// private const int MonthlyLimit = 30000;
//
// if (Interlocked.Increment(ref _monthlyCallCount) > MonthlyLimit)
//     throw new InvalidOperationException("Monthly Cloudmersive quota exceeded");

// IronBarcode — no quota
var results = BarcodeReader.Read("barcode.png");
```

### Remove Network Timeout Configuration

Cloudmersive clients often include timeout configuration. Remove it:

```csharp
// Remove timeout configuration
// Configuration.Default.Timeout = 30000; // 30 second timeout
// Configuration.Default.ConnectionTimeout = 5000;

// IronBarcode has no network timeout — no network
```

### Remove API Key Rotation

If your application rotates API keys on a schedule or stores them in secret management:

```csharp
// Simplify this pattern
// private string GetCloudmersiveApiKey() =>
//     _secretManager.GetSecret("cloudmersive-api-key-current");
//
// Configuration.Default.ApiKey["Apikey"] = GetCloudmersiveApiKey();

// IronBarcode — set once at startup
IronBarCode.License.LicenseKey = _configuration["IronBarcode:LicenseKey"];
```

---

## Common Migration Issues

### Cloudmersive Returned `byte[]` for Generation

Cloudmersive's generation methods return `byte[]` directly. IronBarcode returns a `GeneratedBarcode` object that gives you more options:

```csharp
// Cloudmersive returned byte[] directly
byte[] cloudmersiveResult = apiInstance.GenerateBarcodeQRCode("data");

// IronBarcode — call ToPngBinaryData() for the equivalent byte[]
byte[] ironBarcodeResult = BarcodeWriter.CreateBarcode("data", BarcodeEncoding.QRCode)
    .ToPngBinaryData();

// Or save directly (often simpler)
BarcodeWriter.CreateBarcode("data", BarcodeEncoding.QRCode)
    .SaveAsPng("output.png");

// Or get other formats
byte[] jpegBytes = BarcodeWriter.CreateBarcode("data", BarcodeEncoding.QRCode)
    .ToJpegBinaryData();
```

### Cloudmersive Reading Returned a JSON Response Model

Cloudmersive's scan result is a response model with properties like `Successful`, `RawText`, and `BarcodeType`. IronBarcode returns typed result objects:

```csharp
// Cloudmersive result model
if (cloudmersiveResult.Successful == true)
{
    string value = cloudmersiveResult.RawText;
    string type = cloudmersiveResult.BarcodeType;
}

// IronBarcode result — use .Value and .Format directly
var results = BarcodeReader.Read("barcode.png");
if (results.Any())
{
    string value = results.First().Value;
    string format = results.First().Format.ToString();
}
```

### Multiple Cloudmersive API Instance Types

Cloudmersive separates generation and scanning into `GenerateBarcodeApi` and `BarcodeScanApi` instances. IronBarcode uses static classes — no instances needed:

```csharp
// Cloudmersive — two separate API instances
var generateApi = new GenerateBarcodeApi();
var scanApi = new BarcodeScanApi();

// IronBarcode — static, no instances
// BarcodeWriter for generation
// BarcodeReader for reading
// QRCodeWriter for QR codes specifically
```

---

## Migration Checklist

Run these searches in your codebase before and after migration to verify you've caught every Cloudmersive reference:

```bash
# Find all Cloudmersive references
grep -r "Cloudmersive" --include="*.cs" .
grep -r "GenerateBarcodeApi" --include="*.cs" .
grep -r "BarcodeScanApi" --include="*.cs" .
grep -r "Configuration.Default.ApiKey" --include="*.cs" .
grep -r '"Apikey"' --include="*.cs" .
grep -r "GenerateBarcodeQRCode" --include="*.cs" .
grep -r "GenerateBarcodeCode128" --include="*.cs" .
grep -r "BarcodeScanImage" --include="*.cs" .
grep -r "cloudmersive" --include="*.csproj" .
grep -r "cloudmersive" --include="*.json" .
```

After migration, all searches should return zero results. Then verify IronBarcode is correctly referenced:

```bash
grep -r "IronBarCode" --include="*.cs" .
grep -r "BarcodeReader" --include="*.cs" .
grep -r "BarcodeWriter" --include="*.cs" .
```

### Configuration Checklist

- Remove `Cloudmersive.APIClient.NETCore.Barcode` from all `.csproj` files
- Remove Cloudmersive API key from `appsettings.json`, environment variables, and secret stores
- Add IronBarcode NuGet package
- Add `IronBarCode.License.LicenseKey` initialization at application startup
- Remove retry policies and circuit breakers for Cloudmersive calls
- Remove rate limit tracking code
- Remove network timeout configuration

### Testing Checklist

After completing the migration:

- Verify QR code generation produces valid, scannable output
- Verify Code128 and other linear barcode generation
- Verify barcode reading from common image formats (PNG, JPEG, BMP)
- Test PDF reading if you process PDF documents
- Test multi-barcode detection if applicable
- Confirm no Cloudmersive API calls appear in network traffic
- Confirm the application runs correctly with no internet connection

---

## What You Gain After Migration

Beyond cost savings, the migration removes an entire class of operational concerns:

**No more network-dependent barcode processing.** Your document processing pipeline runs regardless of Cloudmersive's availability, your internet connection quality, or network latency spikes.

**No more quota management.** Month-end spikes in document volume don't create surprise overages or processing failures.

**No more data leaving your network.** Barcode images containing sensitive customer, financial, or health information stay on your infrastructure.

**Simpler code.** The retry infrastructure, timeout handling, and HTTP exception management that production Cloudmersive integrations require is gone. What replaces it is a direct method call.

The migration is mechanical. The benefits are structural — eliminating the dependency on an external network service from a workflow that doesn't need one.
