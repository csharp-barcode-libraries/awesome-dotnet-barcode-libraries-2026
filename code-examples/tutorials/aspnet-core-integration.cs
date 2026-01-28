// Install: dotnet add package IronBarcode
// IronBarcode Tutorial: ASP.NET Core Integration
// Demonstrates barcode generation endpoint, reading endpoint with file upload,
// and QR code generation endpoint with proper error handling

using IronBarCode;
using Microsoft.AspNetCore.Mvc;

namespace BarcodeApi.Controllers
{
    /// <summary>
    /// API Controller for barcode generation and reading operations
    /// Supports barcode generation, barcode reading from uploads, and QR code generation
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : ControllerBase
    {
        /// <summary>
        /// Generate barcode from data and return as JPEG image
        /// POST api/barcode/generate
        /// </summary>
        /// <param name="request">Barcode data and format specification</param>
        /// <returns>JPEG image file</returns>
        [HttpPost("generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GenerateBarcode([FromBody] GenerateRequest request)
        {
            // Validate input data
            if (string.IsNullOrWhiteSpace(request.Data))
            {
                return BadRequest(new { error = "Data cannot be empty" });
            }

            try
            {
                // Create barcode using IronBarcode
                var barcode = BarcodeWriter.CreateBarcode(
                    request.Data,
                    request.Format
                );

                // Convert to JPEG binary data
                byte[] imageBytes = barcode.ToJpegBinaryData();

                // Return image file with appropriate content type
                return File(imageBytes, "image/jpeg", "barcode.jpg");
            }
            catch (ArgumentException ex)
            {
                // Handle invalid barcode data (wrong format for data type)
                return BadRequest(new { error = $"Invalid barcode data: {ex.Message}" });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new { error = "Error generating barcode" });
            }
        }

        /// <summary>
        /// Read barcode from uploaded image file
        /// POST api/barcode/read
        /// </summary>
        /// <param name="file">Image file containing barcode</param>
        /// <returns>JSON with barcode value and format</returns>
        [HttpPost("read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ReadBarcode(IFormFile file)
        {
            // Validate file upload
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            // Optional: Validate file size (prevent memory exhaustion)
            const long maxFileSize = 10 * 1024 * 1024; // 10 MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(new { error = "File size exceeds 10 MB limit" });
            }

            // Optional: Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/tiff", "image/bmp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(new { error = "Invalid file type. Accepted: JPG, PNG, TIFF, BMP" });
            }

            try
            {
                // Read barcode from uploaded file stream
                using (var stream = file.OpenReadStream())
                {
                    // IronBarcode reads directly from stream (no temp file needed)
                    var results = BarcodeReader.Read(stream);

                    if (results == null || !results.Any())
                    {
                        return NotFound(new { error = "No barcode found in image" });
                    }

                    // Return first detected barcode (for multi-barcode support, return all)
                    var barcode = results.FirstOrDefault();
                    return Ok(new
                    {
                        value = barcode.Text,
                        format = barcode.BarcodeType.ToString(),
                        confidence = "High" // IronBarcode confidence is typically high
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error reading barcode: {ex.Message}" });
            }
        }

        /// <summary>
        /// Generate QR code with optional sizing and customization
        /// POST api/barcode/qr
        /// </summary>
        /// <param name="request">QR code data and customization options</param>
        /// <returns>PNG image file</returns>
        [HttpPost("qr")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GenerateQRCode([FromBody] QRRequest request)
        {
            // Validate input data
            if (string.IsNullOrWhiteSpace(request.Data))
            {
                return BadRequest(new { error = "Data cannot be empty" });
            }

            try
            {
                // Create QR code
                var qrCode = BarcodeWriter.CreateBarcode(
                    request.Data,
                    BarcodeEncoding.QRCode
                );

                // Apply sizing if specified
                if (request.Size.HasValue && request.Size.Value > 0)
                {
                    qrCode.ResizeTo(request.Size.Value, request.Size.Value);
                }

                // Convert to PNG (better for QR codes than JPEG)
                byte[] imageBytes = qrCode.ToPngBinaryData();

                return File(imageBytes, "image/png", "qrcode.png");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error generating QR code: {ex.Message}" });
            }
        }

        /// <summary>
        /// Read multiple barcodes from single uploaded image
        /// POST api/barcode/read-multiple
        /// </summary>
        /// <param name="file">Image file containing multiple barcodes</param>
        /// <returns>JSON array of all detected barcodes</returns>
        [HttpPost("read-multiple")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ReadMultipleBarcodes(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    // Configure for multiple barcode detection
                    var options = new BarcodeReaderOptions
                    {
                        ExpectMultipleBarcodes = true
                    };

                    var results = BarcodeReader.Read(stream, options);

                    if (results == null || !results.Any())
                    {
                        return Ok(new { barcodes = new object[0], count = 0 });
                    }

                    // Return all detected barcodes
                    var barcodes = results.Select(b => new
                    {
                        value = b.Text,
                        format = b.BarcodeType.ToString()
                    }).ToArray();

                    return Ok(new { barcodes, count = barcodes.Length });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error reading barcodes: {ex.Message}" });
            }
        }

        /// <summary>
        /// Generate barcode with advanced validation
        /// POST api/barcode/generate-validated
        /// </summary>
        [HttpPost("generate-validated")]
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

            // Format-specific validation
            switch (request.Format)
            {
                case BarcodeEncoding.EAN13:
                    if (request.Data.Length != 12 || !request.Data.All(char.IsDigit))
                    {
                        return BadRequest(new
                        {
                            error = "EAN-13 requires exactly 12 digits",
                            field = "data",
                            received = request.Data.Length
                        });
                    }
                    break;

                case BarcodeEncoding.UPCA:
                    if (request.Data.Length != 11 || !request.Data.All(char.IsDigit))
                    {
                        return BadRequest(new
                        {
                            error = "UPC-A requires exactly 11 digits",
                            field = "data",
                            received = request.Data.Length
                        });
                    }
                    break;

                case BarcodeEncoding.QRCode:
                    // QR codes can hold up to ~4000 characters
                    if (request.Data.Length > 4000)
                    {
                        return BadRequest(new
                        {
                            error = "QR code data exceeds maximum length of 4000 characters",
                            field = "data",
                            received = request.Data.Length
                        });
                    }
                    break;
            }

            try
            {
                var barcode = BarcodeWriter.CreateBarcode(request.Data, request.Format);
                byte[] imageBytes = barcode.ToJpegBinaryData();
                return File(imageBytes, "image/jpeg", "barcode.jpg");
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
    }

    /// <summary>
    /// Request model for barcode generation
    /// </summary>
    public class GenerateRequest
    {
        /// <summary>
        /// Data to encode in barcode
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Barcode format (Code128, QRCode, EAN13, etc.)
        /// Defaults to Code128 if not specified
        /// </summary>
        public BarcodeEncoding Format { get; set; } = BarcodeEncoding.Code128;
    }

    /// <summary>
    /// Request model for QR code generation with customization options
    /// </summary>
    public class QRRequest
    {
        /// <summary>
        /// Data to encode in QR code
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Optional: QR code size in pixels (width and height)
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Optional: Include margins around QR code
        /// </summary>
        public bool IncludeMargins { get; set; } = true;
    }
}


// ===== PROGRAM.CS CONFIGURATION (for reference) =====
// Add this to your Program.cs or Startup.cs for production readiness

/*
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Configure file upload size limits
builder.Services.Configure<FormOptions>(options =>
{
    // Set maximum file upload size to 10 MB
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

// Add controllers
builder.Services.AddControllers();

// Add Swagger for API testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
*/
