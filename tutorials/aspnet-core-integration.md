---
title: "How to Integrate Barcodes in ASP.NET Core"
reading_time: "10 min"
difficulty: "intermediate"
last_updated: "2026-01-23"
category: "read-barcodes"
---

# How to Integrate Barcodes in ASP.NET Core

Build barcode functionality into REST APIs with endpoints for generating barcodes from data and reading barcodes from uploaded images. This tutorial demonstrates creating ASP.NET Core controllers that handle both barcode creation and scanning operations.

---

## Prerequisites

Before starting, ensure you have:

- .NET 6 or later
- ASP.NET Core Web API project
- IronBarcode NuGet package installed
- Visual Studio 2022 or VS Code

Install IronBarcode via NuGet Package Manager or CLI:

```bash
dotnet add package IronBarcode
```

**Note:** For barcode reading basics, see [Read Barcodes from Images](./read-barcodes-images.md). For generation basics, see [Generate Your First Barcode](./generate-barcode-basic.md).

---

## Step 1: Set Up the Project

Create a new ASP.NET Core Web API project and add the IronBarcode NuGet package.

```bash
# Create new Web API project
dotnet new webapi -n BarcodeApi

# Navigate to project directory
cd BarcodeApi

# Add IronBarcode package
dotnet add package IronBarcode
```

Create a new controller file named `BarcodeController.cs` in the Controllers directory. This controller will host all barcode-related endpoints.

---

## Step 2: Create Barcode Generation Endpoint

Build an endpoint that accepts data and format parameters, generates a barcode, and returns the image bytes with appropriate content type.

```csharp
using IronBarCode;
using Microsoft.AspNetCore.Mvc;

namespace BarcodeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateBarcode([FromBody] GenerateRequest request)
        {
            try
            {
                // Create barcode from provided data and format
                var barcode = BarcodeWriter.CreateBarcode(
                    request.Data,
                    request.Format
                );

                // Convert to JPEG binary data
                byte[] imageBytes = barcode.ToJpegBinaryData();

                // Return image with appropriate content type
                return File(imageBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class GenerateRequest
    {
        public string Data { get; set; }
        public BarcodeEncoding Format { get; set; } = BarcodeEncoding.Code128;
    }
}
```

The endpoint accepts JSON with `data` (the barcode content) and `format` (barcode type) parameters. The `BarcodeWriter.CreateBarcode()` method generates the barcode, and `ToJpegBinaryData()` converts it to image bytes for HTTP response.

**Test with curl:**

```bash
curl -X POST http://localhost:5000/api/barcode/generate \
  -H "Content-Type: application/json" \
  -d '{"data": "PRODUCT-12345", "format": "Code128"}' \
  --output barcode.jpg
```

---

## Step 3: Create Barcode Reading Endpoint

Build an endpoint that accepts file uploads, reads barcode data, and returns the decoded value and format type as JSON.

```csharp
[HttpPost("read")]
public IActionResult ReadBarcode(IFormFile file)
{
    if (file == null || file.Length == 0)
    {
        return BadRequest(new { error = "No file uploaded" });
    }

    try
    {
        // Read barcode from uploaded file stream
        using (var stream = file.OpenReadStream())
        {
            var results = BarcodeReader.Read(stream);

            if (results == null || !results.Any())
            {
                return NotFound(new { error = "No barcode found in image" });
            }

            // Return first detected barcode
            var barcode = results.FirstOrDefault();
            return Ok(new
            {
                value = barcode.Text,
                format = barcode.BarcodeType.ToString(),
                confidence = "High"
            });
        }
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}
```

The endpoint uses `IFormFile` to handle file uploads. The `BarcodeReader.Read()` method accepts a stream, enabling processing without saving to disk. This approach handles uploaded images efficiently in memory.

**Test with curl:**

```bash
curl -X POST http://localhost:5000/api/barcode/read \
  -F "file=@barcode.jpg"
```

---

## Step 4: Create QR Code Generation Endpoint

Build a specialized endpoint for QR code generation with customization options including size and margins.

```csharp
[HttpPost("qr")]
public IActionResult GenerateQRCode([FromBody] QRRequest request)
{
    try
    {
        // Create QR code from data
        var qrCode = BarcodeWriter.CreateBarcode(
            request.Data,
            BarcodeEncoding.QRCode
        );

        // Apply size if specified
        if (request.Size.HasValue)
        {
            qrCode.ResizeTo(request.Size.Value, request.Size.Value);
        }

        // Configure margins
        if (request.IncludeMargins == false)
        {
            qrCode.AddBarcodeValueTextBelowBarcode();
        }

        // Convert to PNG for better QR code clarity
        byte[] imageBytes = qrCode.ToPngBinaryData();

        return File(imageBytes, "image/png");
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}

public class QRRequest
{
    public string Data { get; set; }
    public int? Size { get; set; }
    public bool IncludeMargins { get; set; } = true;
}
```

QR codes work best as PNG images due to their sharp edges and geometric patterns. The endpoint supports optional size specification and margin control for different use cases.

**Test with curl:**

```bash
curl -X POST http://localhost:5000/api/barcode/qr \
  -H "Content-Type: application/json" \
  -d '{"data": "https://example.com", "size": 400}' \
  --output qrcode.png
```

---

## Step 5: Add Error Handling

Implement comprehensive error handling for invalid barcode data, unsupported formats, and file processing failures.

```csharp
[HttpPost("generate-safe")]
public IActionResult GenerateBarcodeWithValidation([FromBody] GenerateRequest request)
{
    // Validate input
    if (string.IsNullOrWhiteSpace(request.Data))
    {
        return BadRequest(new
        {
            error = "Barcode data cannot be empty",
            field = "data"
        });
    }

    // Validate data length for specific formats
    if (request.Format == BarcodeEncoding.EAN13)
    {
        if (request.Data.Length != 12)
        {
            return BadRequest(new
            {
                error = "EAN-13 requires exactly 12 digits",
                field = "data",
                received = request.Data.Length
            });
        }
    }

    try
    {
        var barcode = BarcodeWriter.CreateBarcode(
            request.Data,
            request.Format
        );

        byte[] imageBytes = barcode.ToJpegBinaryData();
        return File(imageBytes, "image/jpeg");
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { error = $"Invalid barcode data: {ex.Message}" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "Internal server error generating barcode" });
    }
}
```

Proper error handling returns appropriate HTTP status codes: 400 for client errors (invalid data), 404 for no barcode found, and 500 for server errors. This enables client applications to handle failures gracefully.

---

## Step 6: Configure for Production

Configure ASP.NET Core settings for production barcode processing, including file size limits and memory considerations.

**Update `Program.cs` or `Startup.cs`:**

```csharp
// Configure file upload size limits
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB limit
});
```

**Memory considerations:**

- Barcode reading processes entire image in memory
- For large files (>5 MB), consider streaming or size validation
- File size limits prevent memory exhaustion attacks
- See [Batch Processing](./batch-processing.md) for high-volume scenarios

**Production checklist:**

- Set appropriate file size limits (5-10 MB recommended)
- Add request timeout configuration
- Implement rate limiting for public APIs
- Log barcode processing failures for debugging
- Consider adding authentication middleware
- Cache frequently generated barcodes (optional)

---

## Complete Working Example

Here's the complete `BarcodeController` with all three endpoints and proper error handling:

```csharp
// Install: dotnet add package IronBarcode
using IronBarCode;
using Microsoft.AspNetCore.Mvc;

namespace BarcodeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : ControllerBase
    {
        // POST api/barcode/generate
        [HttpPost("generate")]
        public IActionResult GenerateBarcode([FromBody] GenerateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Data))
            {
                return BadRequest(new { error = "Data cannot be empty" });
            }

            try
            {
                var barcode = BarcodeWriter.CreateBarcode(
                    request.Data,
                    request.Format
                );

                byte[] imageBytes = barcode.ToJpegBinaryData();
                return File(imageBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST api/barcode/read
        [HttpPost("read")]
        public IActionResult ReadBarcode(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var results = BarcodeReader.Read(stream);

                    if (results == null || !results.Any())
                    {
                        return NotFound(new { error = "No barcode found" });
                    }

                    var barcode = results.FirstOrDefault();
                    return Ok(new
                    {
                        value = barcode.Text,
                        format = barcode.BarcodeType.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST api/barcode/qr
        [HttpPost("qr")]
        public IActionResult GenerateQRCode([FromBody] QRRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Data))
            {
                return BadRequest(new { error = "Data cannot be empty" });
            }

            try
            {
                var qrCode = BarcodeWriter.CreateBarcode(
                    request.Data,
                    BarcodeEncoding.QRCode
                );

                if (request.Size.HasValue)
                {
                    qrCode.ResizeTo(request.Size.Value, request.Size.Value);
                }

                byte[] imageBytes = qrCode.ToPngBinaryData();
                return File(imageBytes, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // Request models
    public class GenerateRequest
    {
        public string Data { get; set; }
        public BarcodeEncoding Format { get; set; } = BarcodeEncoding.Code128;
    }

    public class QRRequest
    {
        public string Data { get; set; }
        public int? Size { get; set; }
        public bool IncludeMargins { get; set; } = true;
    }
}
```

**Download:** [Complete code file](../code-examples/tutorials/aspnet-core-integration.cs)

---

## Testing the API

Test the endpoints using curl, Postman, or the built-in Swagger UI.

**Enable Swagger (if not already configured):**

The ASP.NET Core Web API template includes Swagger by default. Navigate to `https://localhost:5001/swagger` to access the interactive API documentation.

**Example curl commands:**

```bash
# Generate Code128 barcode
curl -X POST http://localhost:5000/api/barcode/generate \
  -H "Content-Type: application/json" \
  -d '{"data": "ORDER-12345", "format": "Code128"}' \
  --output barcode.jpg

# Read barcode from image
curl -X POST http://localhost:5000/api/barcode/read \
  -F "file=@barcode.jpg"

# Generate QR code
curl -X POST http://localhost:5000/api/barcode/qr \
  -H "Content-Type: application/json" \
  -d '{"data": "https://example.com", "size": 300}' \
  --output qr.png
```

**Expected responses:**

- Generate endpoints: Binary image data (JPEG or PNG)
- Read endpoint: JSON with `{"value": "...", "format": "..."}`

---

## Next Steps

Now that you understand ASP.NET Core barcode integration, explore these related tutorials:

- **[Batch Processing with Multi-Threading](./batch-processing.md)** - Process multiple barcode requests efficiently
- **[Read Barcodes from PDFs](./read-barcodes-pdfs.md)** - Handle PDF file uploads in web APIs
- **[Barcode Styling and Customization](./barcode-styling.md)** - Customize generated barcode appearance

Return to [All Tutorials](./README.md)

---

Last verified: January 2026

*Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [Iron Software](https://ironsoftware.com)*
