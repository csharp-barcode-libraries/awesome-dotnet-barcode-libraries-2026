/**
 * Barcoder Rendering Complexity Example
 *
 * This example demonstrates the rendering pipeline complexity in Barcoder,
 * showing the IBarcode interface, renderer configuration, and stream handling.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Article: Barcoder vs IronBarcode Comparison 2026
 */

using System;
using System.IO;

// Barcoder packages
// Install: dotnet add package Barcoder
// Install: dotnet add package Barcoder.Renderer.Image
using Barcoder;
using Barcoder.Code128;
using Barcoder.Qr;
using Barcoder.Ean;
using Barcoder.Renderers;

// IronBarcode package
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace BarcoderComparison
{
    /// <summary>
    /// Demonstrates the rendering pipeline complexity in Barcoder
    /// compared to IronBarcode's direct output methods.
    /// </summary>
    public class RenderingComplexityExample
    {
        /// <summary>
        /// Shows the IBarcode interface that Barcoder uses.
        /// </summary>
        public void IBarCodeInterfaceExplanation()
        {
            Console.WriteLine("=== Barcoder IBarcode Interface ===\n");

            Console.WriteLine("When you encode with Barcoder, you get an IBarcode:");

            IBarcode barcode = Code128Encoder.Encode("INTERFACE-EXAMPLE");

            Console.WriteLine($"\nIBarcode barcode = Code128Encoder.Encode(\"INTERFACE-EXAMPLE\");");
            Console.WriteLine($"\nIBarcode properties:");
            Console.WriteLine($"  Bounds: {barcode.Bounds}");
            Console.WriteLine($"  Metadata.IsLinear: {barcode.Metadata.CodeKind == BarcodeKind.Linear}");

            Console.WriteLine("\nThe IBarcode interface contains:");
            Console.WriteLine("  - Bounds (dimensions)");
            Console.WriteLine("  - Content (encoded data)");
            Console.WriteLine("  - Metadata (barcode type info)");
            Console.WriteLine("  - At(x, y) method for pixel access");

            Console.WriteLine("\nIt does NOT contain:");
            Console.WriteLine("  - Any output methods (SaveAsPng, etc.)");
            Console.WriteLine("  - Any image data");
            Console.WriteLine("  - Any rendering capability");

            Console.WriteLine("\nTo get an image, you MUST use a renderer package.\n");
        }

        /// <summary>
        /// Demonstrates renderer configuration options.
        /// </summary>
        public void RendererConfiguration()
        {
            Console.WriteLine("=== Barcoder Renderer Configuration ===\n");

            Console.WriteLine("ImageRenderer requires ImageRendererOptions:");

            var options = new ImageRendererOptions
            {
                ImageFormat = ImageFormat.Png,
                PixelSize = 3,
                BarHeightFor1DBarcode = 60,
                TopMargin = 10,
                BottomMargin = 10,
                LeftMargin = 10,
                RightMargin = 10,
                IncludeEanContentAsText = false
            };

            Console.WriteLine(@"
var options = new ImageRendererOptions
{
    ImageFormat = ImageFormat.Png,  // or Jpeg, Gif, Bmp
    PixelSize = 3,                  // Module size in pixels
    BarHeightFor1DBarcode = 60,     // Height for 1D codes
    TopMargin = 10,
    BottomMargin = 10,
    LeftMargin = 10,
    RightMargin = 10,
    IncludeEanContentAsText = false // Show human-readable text
};");

            var renderer = new ImageRenderer(options);

            Console.WriteLine("\nvar renderer = new ImageRenderer(options);");
            Console.WriteLine("\nNote: Renderer instance can be reused for multiple barcodes.");
            Console.WriteLine("But you must manage the renderer lifecycle yourself.\n");

            // Demo usage
            IBarcode barcode = Code128Encoder.Encode("CONFIG-DEMO");
            using (var stream = File.OpenWrite("barcoder-config.png"))
            {
                renderer.Render(barcode, stream);
            }
            Console.WriteLine("Generated: barcoder-config.png\n");
        }

        /// <summary>
        /// Shows the stream-based output pattern.
        /// </summary>
        public void StreamBasedOutput()
        {
            Console.WriteLine("=== Barcoder Stream-Based Output ===\n");

            Console.WriteLine("Barcoder ALWAYS outputs to streams:");

            var renderer = new ImageRenderer(
                new ImageRendererOptions { ImageFormat = ImageFormat.Png }
            );

            IBarcode barcode = Code128Encoder.Encode("STREAM-EXAMPLE");

            // File output (must manage stream)
            Console.WriteLine("\n1. File output (manual stream management):");
            Console.WriteLine(@"
using (var stream = File.OpenWrite(""barcode.png""))
{
    renderer.Render(barcode, stream);
}
// Stream is automatically disposed");

            using (var stream = File.OpenWrite("barcoder-stream.png"))
            {
                renderer.Render(barcode, stream);
            }

            // Memory output (for web APIs, etc.)
            Console.WriteLine("\n2. Memory output (for byte arrays):");
            Console.WriteLine(@"
using (var ms = new MemoryStream())
{
    renderer.Render(barcode, ms);
    byte[] bytes = ms.ToArray();
}
// Must remember ToArray() before disposing");

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                renderer.Render(barcode, ms);
                imageBytes = ms.ToArray();
            }
            Console.WriteLine($"   Generated {imageBytes.Length} bytes");

            // HTTP response (common web scenario)
            Console.WriteLine("\n3. HTTP response (hypothetical):");
            Console.WriteLine(@"
public IActionResult GetBarcode()
{
    var renderer = new ImageRenderer(/*options*/);
    IBarcode barcode = Code128Encoder.Encode(""data"");

    using (var ms = new MemoryStream())
    {
        renderer.Render(barcode, ms);
        return File(ms.ToArray(), ""image/png"");
    }
}");

            Console.WriteLine("\nEvery output requires stream setup and disposal.\n");
        }

        /// <summary>
        /// Compares to IronBarcode's direct output methods.
        /// </summary>
        public void IronBarcodeDirectOutput()
        {
            Console.WriteLine("=== IronBarcode Direct Output Methods ===\n");

            // IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";

            Console.WriteLine("IronBarcode provides direct output methods:\n");

            var barcode = BarcodeWriter.CreateBarcode("DIRECT-OUTPUT", BarcodeEncoding.Code128);

            // File outputs (one-liners)
            Console.WriteLine("1. File outputs (one-liners):");
            Console.WriteLine("   barcode.SaveAsPng(\"barcode.png\");");
            Console.WriteLine("   barcode.SaveAsJpeg(\"barcode.jpg\");");
            Console.WriteLine("   barcode.SaveAsPdf(\"barcode.pdf\");");
            Console.WriteLine("   barcode.SaveAsSvg(\"barcode.svg\");");
            Console.WriteLine("   barcode.SaveAsTiff(\"barcode.tiff\");");

            barcode.SaveAsPng("ironbarcode-direct.png");
            Console.WriteLine("\n   Generated: ironbarcode-direct.png");

            // Binary data outputs
            Console.WriteLine("\n2. Binary data outputs:");
            Console.WriteLine("   byte[] png = barcode.ToPngBinaryData();");
            Console.WriteLine("   byte[] jpg = barcode.ToJpegBinaryData();");

            byte[] pngData = barcode.ToPngBinaryData();
            Console.WriteLine($"   Generated {pngData.Length} bytes (PNG)");

            // Stream outputs when needed
            Console.WriteLine("\n3. Stream output (when you actually need a stream):");
            Console.WriteLine("   barcode.ToStream(ImageFormat.Png)");

            Console.WriteLine("\nNo renderer setup. No stream management. No disposal.\n");
        }

        /// <summary>
        /// Side-by-side comparison of generating multiple barcodes.
        /// </summary>
        public void MultiBarcodeBatchComparison()
        {
            Console.WriteLine("=== Batch Generation Comparison ===\n");

            string[] productCodes = { "SKU-001", "SKU-002", "SKU-003", "SKU-004", "SKU-005" };

            Console.WriteLine("Scenario: Generate 5 product barcodes\n");

            // Barcoder approach
            Console.WriteLine("Barcoder approach:");
            Console.WriteLine(@"
var renderer = new ImageRenderer(new ImageRendererOptions
{
    ImageFormat = ImageFormat.Png,
    PixelSize = 2
});

foreach (string code in productCodes)
{
    IBarcode barcode = Code128Encoder.Encode(code);
    using (var stream = File.OpenWrite($""barcoder-{code}.png""))
    {
        renderer.Render(barcode, stream);
    }
}");

            var renderer = new ImageRenderer(new ImageRendererOptions
            {
                ImageFormat = ImageFormat.Png,
                PixelSize = 2
            });

            foreach (string code in productCodes)
            {
                IBarcode barcode = Code128Encoder.Encode(code);
                using (var stream = File.OpenWrite($"barcoder-batch-{code}.png"))
                {
                    renderer.Render(barcode, stream);
                }
            }

            Console.WriteLine("Lines of code: 12");

            // IronBarcode approach
            Console.WriteLine("\nIronBarcode approach:");
            Console.WriteLine(@"
foreach (string code in productCodes)
{
    BarcodeWriter.CreateBarcode(code, BarcodeEncoding.Code128)
        .SaveAsPng($""iron-{code}.png"");
}");

            foreach (string code in productCodes)
            {
                BarcodeWriter.CreateBarcode(code, BarcodeEncoding.Code128)
                    .SaveAsPng($"iron-batch-{code}.png");
            }

            Console.WriteLine("Lines of code: 5");
            Console.WriteLine("\nCode reduction: 58%\n");
        }

        /// <summary>
        /// Shows error handling differences.
        /// </summary>
        public void ErrorHandlingComparison()
        {
            Console.WriteLine("=== Error Handling Comparison ===\n");

            Console.WriteLine("Barcoder (multiple failure points):");
            Console.WriteLine(@"
try
{
    // Encoding can fail (invalid data)
    IBarcode barcode = Code128Encoder.Encode(data);

    // Renderer creation can fail (invalid options)
    var renderer = new ImageRenderer(options);

    // Stream can fail (file permissions, disk full)
    using (var stream = File.OpenWrite(path))
    {
        // Rendering can fail (memory, format issues)
        renderer.Render(barcode, stream);
    }
}
catch (EncodingException ex)
{
    // Handle encoding errors
}
catch (IOException ex)
{
    // Handle file errors
}
catch (Exception ex)
{
    // Handle rendering errors
}");

            Console.WriteLine("\nIronBarcode (single operation):");
            Console.WriteLine(@"
try
{
    BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
        .SaveAsPng(path);
}
catch (BarcodeException ex)
{
    // Handle any barcode error
}
catch (IOException ex)
{
    // Handle file errors
}");

            Console.WriteLine("\nFewer failure points = simpler error handling\n");
        }

        /// <summary>
        /// Shows format-specific rendering complexity.
        /// </summary>
        public void FormatSpecificRendering()
        {
            Console.WriteLine("=== Format-Specific Rendering ===\n");

            var renderer = new ImageRenderer(
                new ImageRendererOptions { ImageFormat = ImageFormat.Png, PixelSize = 3 }
            );

            Console.WriteLine("Barcoder: Same renderer, different encoders\n");

            // 1D barcode
            Console.WriteLine("1D (Code128):");
            IBarcode code128 = Code128Encoder.Encode("1D-EXAMPLE");
            Console.WriteLine($"  Dimensions: {code128.Bounds.X} x {code128.Bounds.Y}");
            using (var s = File.OpenWrite("barcoder-render-1d.png"))
            {
                renderer.Render(code128, s);
            }

            // 2D barcode
            Console.WriteLine("\n2D (QR Code):");
            IBarcode qr = QrEncoder.Encode("2D-EXAMPLE", ErrorCorrectionLevel.M);
            Console.WriteLine($"  Dimensions: {qr.Bounds.X} x {qr.Bounds.Y}");
            using (var s = File.OpenWrite("barcoder-render-2d.png"))
            {
                renderer.Render(qr, s);
            }

            Console.WriteLine("\nNote: Renderer options affect both 1D and 2D differently.");
            Console.WriteLine("BarHeightFor1DBarcode only applies to 1D.");
            Console.WriteLine("PixelSize affects module size for 2D.\n");

            Console.WriteLine("IronBarcode: Unified fluent API\n");

            // 1D with method chaining
            var iron1d = BarcodeWriter.CreateBarcode("1D-EXAMPLE", BarcodeEncoding.Code128)
                .ResizeTo(300, 100);
            iron1d.SaveAsPng("iron-render-1d.png");
            Console.WriteLine("1D (Code128): .ResizeTo(300, 100)");

            // 2D with method chaining
            var iron2d = BarcodeWriter.CreateBarcode("2D-EXAMPLE", BarcodeEncoding.QRCode)
                .ResizeTo(200, 200);
            iron2d.SaveAsPng("iron-render-2d.png");
            Console.WriteLine("2D (QR Code): .ResizeTo(200, 200)");

            Console.WriteLine("\nSame API pattern for both 1D and 2D.\n");
        }

        /// <summary>
        /// Summary of rendering complexity differences.
        /// </summary>
        public void RenderingSummary()
        {
            Console.WriteLine("=== Rendering Complexity Summary ===\n");

            Console.WriteLine("Barcoder Rendering Pipeline:");
            Console.WriteLine("  1. Call format-specific Encoder");
            Console.WriteLine("  2. Receive IBarcode data structure");
            Console.WriteLine("  3. Create ImageRendererOptions");
            Console.WriteLine("  4. Create ImageRenderer");
            Console.WriteLine("  5. Open output stream");
            Console.WriteLine("  6. Call renderer.Render()");
            Console.WriteLine("  7. Dispose stream");
            Console.WriteLine("  Steps: 7");

            Console.WriteLine("\nIronBarcode Direct Output:");
            Console.WriteLine("  1. Call BarcodeWriter.CreateBarcode()");
            Console.WriteLine("  2. Call .SaveAsPng() (or other format)");
            Console.WriteLine("  Steps: 2");

            Console.WriteLine("\nComplexity Reduction: 71% fewer steps");
            Console.WriteLine("Code Reduction: 60-80% fewer lines");
            Console.WriteLine("Error Points: Fewer places for failures\n");
        }

        public static void Main(string[] args)
        {
            var example = new RenderingComplexityExample();

            example.IBarCodeInterfaceExplanation();
            example.RendererConfiguration();
            example.StreamBasedOutput();
            example.IronBarcodeDirectOutput();
            example.MultiBarcodeBatchComparison();
            example.ErrorHandlingComparison();
            example.FormatSpecificRendering();
            example.RenderingSummary();

            Console.WriteLine("=== Conclusion ===");
            Console.WriteLine("Barcoder: Flexible but verbose rendering pipeline");
            Console.WriteLine("IronBarcode: Direct output methods reduce complexity");
        }
    }
}
