/**
 * Barcoder Multi-Package Architecture Example
 *
 * This example demonstrates Barcoder's multi-package architecture,
 * showing how multiple NuGet packages are required for barcode generation.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Article: Barcoder vs IronBarcode Comparison 2026
 */

using System;
using System.IO;

// Barcoder requires multiple package imports
// Install: dotnet add package Barcoder
// Install: dotnet add package Barcoder.Renderer.Image
using Barcoder;
using Barcoder.Code128;
using Barcoder.Qr;
using Barcoder.DataMatrix;
using Barcoder.Renderers;

// IronBarcode is a single package
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace BarcoderComparison
{
    /// <summary>
    /// Demonstrates the multi-package complexity of Barcoder
    /// compared to IronBarcode's single-package approach.
    /// </summary>
    public class MultiPackageExample
    {
        /// <summary>
        /// Shows the three-package setup required for Barcoder.
        /// </summary>
        public void BarcoderPackageRequirements()
        {
            Console.WriteLine("=== Barcoder Package Requirements ===\n");

            Console.WriteLine("To generate a PNG barcode with Barcoder, you need:");
            Console.WriteLine("\n1. Core package (encoding only):");
            Console.WriteLine("   dotnet add package Barcoder");
            Console.WriteLine("   - Contains: Encoding algorithms");
            Console.WriteLine("   - Produces: IBarcode data structure");
            Console.WriteLine("   - Can NOT produce images alone\n");

            Console.WriteLine("2. Image renderer package:");
            Console.WriteLine("   dotnet add package Barcoder.Renderer.Image");
            Console.WriteLine("   - Contains: PNG/JPEG rendering");
            Console.WriteLine("   - Depends on: SixLabors.ImageSharp");
            Console.WriteLine("   - NOTE: .NET 6+ only (no .NET Framework)\n");

            Console.WriteLine("3. (Optional) SVG renderer package:");
            Console.WriteLine("   dotnet add package Barcoder.Renderer.Svg");
            Console.WriteLine("   - Contains: SVG vector rendering");
            Console.WriteLine("   - Depends on: SVG libraries\n");

            Console.WriteLine("Your .csproj will have:");
            Console.WriteLine(@"
<PackageReference Include=""Barcoder"" Version=""3.0.0"" />
<PackageReference Include=""Barcoder.Renderer.Image"" Version=""3.0.0"" />
<!-- Plus transitive dependencies: -->
<!-- SixLabors.ImageSharp -->
<!-- SixLabors.Fonts -->
");
        }

        /// <summary>
        /// Demonstrates the multi-step Barcoder workflow.
        /// </summary>
        public void BarcoderWorkflow()
        {
            Console.WriteLine("=== Barcoder Multi-Step Workflow ===\n");

            Console.WriteLine("Step 1: Import namespaces (3 different ones)");
            Console.WriteLine("  using Barcoder;");
            Console.WriteLine("  using Barcoder.Code128;  // Encoder for this format");
            Console.WriteLine("  using Barcoder.Renderers;");

            Console.WriteLine("\nStep 2: Encode data to barcode structure");
            IBarcode barcode = Code128Encoder.Encode("PRODUCT-12345");
            Console.WriteLine($"  IBarcode barcode = Code128Encoder.Encode(\"PRODUCT-12345\");");
            Console.WriteLine($"  // Returns: IBarcode with Bounds={barcode.Bounds}");

            Console.WriteLine("\nStep 3: Create renderer with options");
            var renderer = new ImageRenderer(
                new ImageRendererOptions
                {
                    ImageFormat = ImageFormat.Png,
                    PixelSize = 2,
                    BarHeightFor1DBarcode = 50,
                    IncludeEanContentAsText = true
                }
            );
            Console.WriteLine(@"  var renderer = new ImageRenderer(
      new ImageRendererOptions
      {
          ImageFormat = ImageFormat.Png,
          PixelSize = 2,
          BarHeightFor1DBarcode = 50
      }
  );");

            Console.WriteLine("\nStep 4: Open output stream");
            Console.WriteLine("  using var stream = File.OpenWrite(\"barcode.png\");");

            Console.WriteLine("\nStep 5: Render to stream");
            using (var stream = File.OpenWrite("barcoder-workflow.png"))
            {
                renderer.Render(barcode, stream);
            }
            Console.WriteLine("  renderer.Render(barcode, stream);");

            Console.WriteLine("\nTotal: 5 steps, ~15 lines of code\n");
        }

        /// <summary>
        /// Shows the same functionality with IronBarcode.
        /// </summary>
        public void IronBarcodeWorkflow()
        {
            Console.WriteLine("=== IronBarcode Single-Step Workflow ===\n");

            Console.WriteLine("Step 1: Import single namespace");
            Console.WriteLine("  using IronBarCode;");

            Console.WriteLine("\nStep 2: Create and save");
            Console.WriteLine("  BarcodeWriter.CreateBarcode(\"PRODUCT-12345\", BarcodeEncoding.Code128)");
            Console.WriteLine("      .SaveAsPng(\"barcode.png\");");

            // IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";

            BarcodeWriter.CreateBarcode("PRODUCT-12345", BarcodeEncoding.Code128)
                .SaveAsPng("ironbarcode-workflow.png");

            Console.WriteLine("\nTotal: 2 steps, 2 lines of code");
            Console.WriteLine("Reduction: 85% less code\n");
        }

        /// <summary>
        /// Demonstrates different encoder namespaces for different formats.
        /// </summary>
        public void BarcoderFormatSpecificImports()
        {
            Console.WriteLine("=== Barcoder Format-Specific Imports ===\n");

            Console.WriteLine("Each barcode format requires its own namespace import:\n");

            // Code128
            Console.WriteLine("For Code128:");
            Console.WriteLine("  using Barcoder.Code128;");
            Console.WriteLine("  IBarcode barcode = Code128Encoder.Encode(data);");

            IBarcode code128 = Code128Encoder.Encode("CODE128");

            // QR Code
            Console.WriteLine("\nFor QR Code:");
            Console.WriteLine("  using Barcoder.Qr;");
            Console.WriteLine("  IBarcode barcode = QrEncoder.Encode(data, ErrorCorrectionLevel.M);");

            IBarcode qr = QrEncoder.Encode("QRCODE", ErrorCorrectionLevel.M);

            // DataMatrix
            Console.WriteLine("\nFor DataMatrix:");
            Console.WriteLine("  using Barcoder.DataMatrix;");
            Console.WriteLine("  IBarcode barcode = DataMatrixEncoder.Encode(data);");

            IBarcode datamatrix = DataMatrixEncoder.Encode("DATAMATRIX");

            Console.WriteLine("\nThis means for a mixed-format project:");
            Console.WriteLine("  using Barcoder.Code128;   // For Code128");
            Console.WriteLine("  using Barcoder.Code39;    // For Code39");
            Console.WriteLine("  using Barcoder.Ean;       // For EAN");
            Console.WriteLine("  using Barcoder.Qr;        // For QR");
            Console.WriteLine("  using Barcoder.DataMatrix;// For DataMatrix");
            Console.WriteLine("  using Barcoder.Pdf417;    // For PDF417");
            Console.WriteLine("  using Barcoder.Aztec;     // For Aztec");

            Console.WriteLine("\nWith IronBarcode, one namespace covers all:");
            Console.WriteLine("  using IronBarCode;");
            Console.WriteLine("  // Then just change the encoding enum");
            Console.WriteLine("  BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128);");
            Console.WriteLine("  BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode);");
            Console.WriteLine("  BarcodeWriter.CreateBarcode(data, BarcodeEncoding.DataMatrix);");
        }

        /// <summary>
        /// Shows version coordination challenges.
        /// </summary>
        public void VersionCoordinationIssues()
        {
            Console.WriteLine("\n=== Version Coordination Challenges ===\n");

            Console.WriteLine("Barcoder has multiple packages that must stay in sync:");
            Console.WriteLine(@"
  <PackageReference Include=""Barcoder"" Version=""3.0.0"" />
  <PackageReference Include=""Barcoder.Renderer.Image"" Version=""3.0.0"" />
  <PackageReference Include=""Barcoder.Renderer.Svg"" Version=""3.0.0"" />
");

            Console.WriteLine("If versions get out of sync (e.g., during partial updates):");
            Console.WriteLine("  - Interface mismatches can occur");
            Console.WriteLine("  - Runtime errors on Render() calls");
            Console.WriteLine("  - Subtle behavioral differences\n");

            Console.WriteLine("Example scenario:");
            Console.WriteLine("  1. 'dotnet add package Barcoder' updates to 3.1.0");
            Console.WriteLine("  2. Barcoder.Renderer.Image still at 3.0.0");
            Console.WriteLine("  3. IBarcode interface changed in 3.1.0");
            Console.WriteLine("  4. Render() fails at runtime\n");

            Console.WriteLine("IronBarcode: Single package, no coordination needed");
            Console.WriteLine("  dotnet add package IronBarcode");
            Console.WriteLine("  // That's it\n");
        }

        /// <summary>
        /// Demonstrates generating multiple formats with both libraries.
        /// </summary>
        public void GenerateMultipleFormats()
        {
            Console.WriteLine("=== Generate Multiple Formats Comparison ===\n");

            Console.WriteLine("Barcoder approach (multiple encoders, shared renderer):\n");

            var renderer = new ImageRenderer(
                new ImageRendererOptions
                {
                    ImageFormat = ImageFormat.Png,
                    PixelSize = 3
                }
            );

            // Code128
            IBarcode code128 = Code128Encoder.Encode("MULTI-FORMAT-TEST");
            using (var stream = File.OpenWrite("barcoder-multi-code128.png"))
            {
                renderer.Render(code128, stream);
            }
            Console.WriteLine("  Generated: barcoder-multi-code128.png");

            // QR Code
            IBarcode qr = QrEncoder.Encode("MULTI-FORMAT-TEST", ErrorCorrectionLevel.M);
            using (var stream = File.OpenWrite("barcoder-multi-qr.png"))
            {
                renderer.Render(qr, stream);
            }
            Console.WriteLine("  Generated: barcoder-multi-qr.png");

            // DataMatrix
            IBarcode dm = DataMatrixEncoder.Encode("MULTI-FORMAT");
            using (var stream = File.OpenWrite("barcoder-multi-dm.png"))
            {
                renderer.Render(dm, stream);
            }
            Console.WriteLine("  Generated: barcoder-multi-dm.png");

            Console.WriteLine("\n  Lines of code: ~20");

            Console.WriteLine("\nIronBarcode approach (single API):\n");

            BarcodeWriter.CreateBarcode("MULTI-FORMAT-TEST", BarcodeEncoding.Code128)
                .SaveAsPng("iron-multi-code128.png");
            Console.WriteLine("  Generated: iron-multi-code128.png");

            BarcodeWriter.CreateBarcode("MULTI-FORMAT-TEST", BarcodeEncoding.QRCode)
                .SaveAsPng("iron-multi-qr.png");
            Console.WriteLine("  Generated: iron-multi-qr.png");

            BarcodeWriter.CreateBarcode("MULTI-FORMAT", BarcodeEncoding.DataMatrix)
                .SaveAsPng("iron-multi-dm.png");
            Console.WriteLine("  Generated: iron-multi-dm.png");

            Console.WriteLine("\n  Lines of code: 6");
            Console.WriteLine("\n  Code reduction: 70%\n");
        }

        /// <summary>
        /// Summary of multi-package vs single-package tradeoffs.
        /// </summary>
        public void PackageSummary()
        {
            Console.WriteLine("=== Package Architecture Summary ===\n");

            Console.WriteLine("Barcoder Multi-Package:");
            Console.WriteLine("  + Modular (use only what you need)");
            Console.WriteLine("  + Clean separation of concerns");
            Console.WriteLine("  + Can swap renderers");
            Console.WriteLine("  - More packages to manage");
            Console.WriteLine("  - Version coordination required");
            Console.WriteLine("  - More imports per file");
            Console.WriteLine("  - More verbose code");
            Console.WriteLine("  - .NET Framework not supported (Image renderer)");

            Console.WriteLine("\nIronBarcode Single-Package:");
            Console.WriteLine("  + One package does everything");
            Console.WriteLine("  + No version coordination");
            Console.WriteLine("  + Simpler code");
            Console.WriteLine("  + Includes reading capability");
            Console.WriteLine("  + .NET Framework supported");
            Console.WriteLine("  + PDF support built-in");
            Console.WriteLine("  - Larger single package");
            Console.WriteLine("  - Commercial license required\n");
        }

        public static void Main(string[] args)
        {
            var example = new MultiPackageExample();

            example.BarcoderPackageRequirements();
            example.BarcoderWorkflow();
            example.IronBarcodeWorkflow();
            example.BarcoderFormatSpecificImports();
            example.VersionCoordinationIssues();
            example.GenerateMultipleFormats();
            example.PackageSummary();

            Console.WriteLine("=== Conclusion ===");
            Console.WriteLine("Barcoder: Multi-package architecture adds complexity");
            Console.WriteLine("IronBarcode: Single-package simplicity with more features");
        }
    }
}
