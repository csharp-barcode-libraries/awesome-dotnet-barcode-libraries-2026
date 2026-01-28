// =============================================================================
// Syncfusion vs IronBarcode: Generation-Only Limitation Comparison
// =============================================================================
// This file demonstrates the key architectural difference between Syncfusion
// Barcode (generation-only UI control) and IronBarcode (read + write SDK).
//
// Key differences highlighted:
// 1. Syncfusion Barcode component CANNOT read barcodes
// 2. Syncfusion reading requires separate OPX product (uses ZXing internally)
// 3. IronBarcode provides unified read/write in single package
// =============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using IronBarCode;

namespace GenerationOnlyComparison
{
    // =========================================================================
    // SYNCFUSION BARCODE: GENERATION ONLY
    // =========================================================================
    // Syncfusion's barcode component is designed as a UI control for displaying
    // generated barcodes. It does NOT include barcode recognition/reading.
    // =========================================================================

    /// <summary>
    /// Demonstrates what Syncfusion Barcode CAN do: generation as UI controls.
    /// These examples show the UI control-based approach.
    /// </summary>
    public class SyncfusionGeneration
    {
        /// <summary>
        /// WinForms barcode generation using SfBarcode control.
        /// Note: This is a UI control, not a file generation library.
        /// </summary>
        public void GenerateWinFormsBarcode()
        {
            // Syncfusion WinForms example (conceptual - requires Syncfusion packages)
            /*
            using Syncfusion.Windows.Forms.Barcode;

            var barcode = new SfBarcode();
            barcode.Text = "12345678";
            barcode.Symbology = BarcodeSymbolType.Code128A;
            barcode.BarHeight = 100;
            barcode.NarrowBarWidth = 1;
            barcode.ShowText = true;
            barcode.TextFont = new Font("Arial", 10);

            // The control is designed to be added to a form
            this.Controls.Add(barcode);

            // To save as file, you need manual bitmap work:
            using var bitmap = new Bitmap(barcode.Width, barcode.Height);
            barcode.DrawToBitmap(bitmap, barcode.ClientRectangle);
            bitmap.Save("syncfusion-barcode.png", ImageFormat.Png);
            */
        }

        /// <summary>
        /// Blazor barcode generation using SfBarcodeGenerator component.
        /// This is a Razor component, not C# code.
        /// </summary>
        public void GenerateBlazorBarcode()
        {
            // Blazor component (in .razor file):
            /*
            @using Syncfusion.Blazor.BarcodeGenerator

            <SfBarcodeGenerator
                Width="200px"
                Height="150px"
                Type="BarcodeType.Code128"
                Value="12345678">
                <BarcodeGeneratorDisplayText Visibility="true">
                </BarcodeGeneratorDisplayText>
            </SfBarcodeGenerator>
            */

            // Note: Exporting the rendered barcode requires JavaScript interop
            // and additional configuration for file download
        }

        /// <summary>
        /// QR Code generation in Syncfusion Blazor.
        /// </summary>
        public void GenerateQRCodeBlazor()
        {
            // Blazor component:
            /*
            <SfQRCodeGenerator
                Width="200px"
                Height="200px"
                Value="https://example.com">
                <QRCodeGeneratorDisplayText Visibility="false">
                </QRCodeGeneratorDisplayText>
            </SfQRCodeGenerator>
            */
        }
    }

    /// <summary>
    /// Demonstrates what Syncfusion Barcode CANNOT do: reading barcodes.
    /// </summary>
    public class SyncfusionReadingLimitation
    {
        /// <summary>
        /// Syncfusion Barcode component does NOT support reading.
        /// This method shows what you CANNOT do with Syncfusion Barcode.
        /// </summary>
        public void CannotReadBarcodes()
        {
            // =====================================================================
            // SYNCFUSION BARCODE HAS NO READING API
            // =====================================================================
            //
            // There is no equivalent to:
            // - BarcodeReader.Read()
            // - DecodeBarcode()
            // - ScanImage()
            //
            // The SfBarcode and SfBarcodeGenerator classes are WRITE-ONLY.
            //
            // If you need to read barcodes with Syncfusion, you need:
            // 1. Syncfusion Barcode Reader OPX (separate product)
            // 2. Which internally uses ZXing.Net
            // 3. Purchased and licensed separately
            //
            // =====================================================================

            throw new NotSupportedException(
                "Syncfusion Barcode component does not support reading barcodes. " +
                "You need the separate 'Barcode Reader OPX' product."
            );
        }

        /// <summary>
        /// Syncfusion cannot extract barcodes from PDFs.
        /// </summary>
        public void CannotReadFromPDF()
        {
            // PDF barcode extraction is not supported.
            // Even with Syncfusion.Pdf component, there's no barcode reading API.
            // You would need to:
            // 1. Use Syncfusion.Pdf to extract images from PDF
            // 2. Use Barcode Reader OPX (separate product) to decode images
            // 3. Manage two separate products and licenses

            throw new NotSupportedException(
                "Syncfusion Barcode cannot read barcodes from PDF documents."
            );
        }
    }

    // =========================================================================
    // IRONBARCODE: UNIFIED READ AND WRITE
    // =========================================================================
    // IronBarcode provides both generation and reading in a single library.
    // No separate products, no separate purchases, unified API.
    // =========================================================================

    /// <summary>
    /// IronBarcode generation examples - direct file output, no UI control needed.
    /// </summary>
    public class IronBarcodeGeneration
    {
        /// <summary>
        /// Generate Code128 barcode - single line.
        /// </summary>
        public void GenerateCode128()
        {
            // Install: dotnet add package IronBarcode

            // One line to generate and save
            BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                .SaveAsPng("barcode.png");
        }

        /// <summary>
        /// Generate QR code with customization.
        /// </summary>
        public void GenerateQRCode()
        {
            var qr = QRCodeWriter.CreateQrCode("https://example.com", 500);
            qr.ChangeBarCodeColor(Color.DarkBlue);
            qr.SaveAsPng("qrcode.png");
        }

        /// <summary>
        /// Generate barcode directly to PDF.
        /// </summary>
        public void GenerateToPDF()
        {
            // Direct PDF output - no additional library needed
            BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                .SaveAsPdf("barcode.pdf");
        }

        /// <summary>
        /// Generate to byte array for web response.
        /// </summary>
        public byte[] GenerateToBytes()
        {
            return BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                .ToPngBinaryData();
        }
    }

    /// <summary>
    /// IronBarcode reading examples - the capability Syncfusion lacks.
    /// </summary>
    public class IronBarcodeReading
    {
        /// <summary>
        /// Read barcode from image - automatic format detection.
        /// </summary>
        public void ReadFromImage()
        {
            // Automatic format detection - no need to specify barcode type
            var results = BarcodeReader.Read("barcode.png");

            foreach (var barcode in results)
            {
                Console.WriteLine($"Type: {barcode.BarcodeType}");
                Console.WriteLine($"Value: {barcode.Text}");
                Console.WriteLine($"Confidence: {barcode.Confidence}");
            }
        }

        /// <summary>
        /// Read all barcodes from a PDF document.
        /// </summary>
        public void ReadFromPDF()
        {
            // PDF support is native - no separate library needed
            var results = BarcodeReader.Read("document.pdf");

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
            }
        }

        /// <summary>
        /// Read from byte array (e.g., web upload).
        /// </summary>
        public void ReadFromBytes(byte[] imageData)
        {
            var results = BarcodeReader.Read(imageData);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Found: {barcode.Text}");
            }
        }

        /// <summary>
        /// Batch read from multiple files.
        /// </summary>
        public void ReadMultipleFiles(string[] filePaths)
        {
            // Process multiple files in one call
            var results = BarcodeReader.Read(filePaths);

            foreach (var barcode in results)
            {
                Console.WriteLine($"File: {barcode.InputPath}");
                Console.WriteLine($"Value: {barcode.Text}");
            }
        }
    }

    // =========================================================================
    // REAL-WORLD SCENARIO: SHIPPING LABEL PROCESSING
    // =========================================================================
    // Demonstrates a common workflow that requires both reading and writing.
    // =========================================================================

    /// <summary>
    /// Shipping label workflow comparison.
    /// </summary>
    public class ShippingLabelWorkflow
    {
        /// <summary>
        /// With Syncfusion: Partial workflow, reading not possible.
        /// </summary>
        public void SyncfusionApproach()
        {
            // STEP 1: Generate shipping label barcode - POSSIBLE
            // (Using SfBarcode control or SfBarcodeGenerator)

            // STEP 2: Scan incoming package barcode - NOT POSSIBLE
            // Syncfusion Barcode cannot read barcodes from scanner images

            // STEP 3: Match and verify - NOT POSSIBLE
            // Cannot complete verification workflow without reading

            // To complete this workflow with Syncfusion:
            // - Need Barcode Reader OPX (separate purchase)
            // - Or integrate ZXing.Net separately
            // - Manage two different libraries/products
        }

        /// <summary>
        /// With IronBarcode: Complete workflow in single library.
        /// </summary>
        public Dictionary<string, string> IronBarcodeApproach()
        {
            var verificationResults = new Dictionary<string, string>();

            // STEP 1: Generate shipping label barcodes
            var trackingCodes = new[] { "PKG001", "PKG002", "PKG003" };
            foreach (var code in trackingCodes)
            {
                BarcodeWriter.CreateBarcode(code, BarcodeEncoding.Code128)
                    .SaveAsPng($"label_{code}.png");
            }

            // STEP 2: Later, scan incoming packages (images from scanner)
            var scannedImages = Directory.GetFiles("incoming_scans", "*.png");
            var scannedResults = BarcodeReader.Read(scannedImages);

            // STEP 3: Match and verify
            foreach (var scanned in scannedResults)
            {
                var packageId = scanned.Text;
                var sourceFile = scanned.InputPath;
                var matchStatus = trackingCodes.Contains(packageId)
                    ? "VERIFIED"
                    : "UNKNOWN";

                verificationResults[packageId] = matchStatus;
                Console.WriteLine($"{packageId} from {sourceFile}: {matchStatus}");
            }

            return verificationResults;
        }
    }

    // =========================================================================
    // COMPARISON SUMMARY
    // =========================================================================

    /// <summary>
    /// Feature comparison matrix.
    /// </summary>
    public static class FeatureComparisonMatrix
    {
        /*
        | Capability                | Syncfusion Barcode | IronBarcode |
        |---------------------------|-------------------|-------------|
        | Generate 1D barcodes      | Yes               | Yes         |
        | Generate QR codes         | Yes               | Yes         |
        | Generate DataMatrix       | Yes               | Yes         |
        | Save to PNG               | Manual (bitmap)   | Built-in    |
        | Save to PDF               | No                | Built-in    |
        | Read from image           | NO                | Yes         |
        | Read from PDF             | NO                | Yes         |
        | Auto format detection     | N/A               | Yes         |
        | Batch processing          | Manual            | Built-in    |
        | ML error correction       | No                | Yes         |
        | UI control mode           | Primary           | Optional    |
        | Library/SDK mode          | Limited           | Primary     |
        | Console app support       | Awkward           | Native      |
        | Web API support           | Limited           | Native      |
        | Products needed           | 2+ for read/write | 1           |
        */
    }

    // =========================================================================
    // PRACTICAL IMPACT: CODE COMPLEXITY
    // =========================================================================

    /// <summary>
    /// Shows the practical code difference for a complete read/write workflow.
    /// </summary>
    public class CodeComplexityComparison
    {
        /// <summary>
        /// Lines of code needed for complete barcode workflow.
        /// </summary>
        public void CountLinesOfCode()
        {
            // =====================================================
            // SYNCFUSION (with separate reader product)
            // =====================================================
            // 1. Generate barcode (WinForms): ~10 lines
            // 2. Save to file (bitmap work): ~5 lines
            // 3. Read barcode (OPX/ZXing): ~15 lines (different API)
            // 4. License registration: ~2 lines
            // Total: ~32 lines + two products to manage

            // =====================================================
            // IRONBARCODE
            // =====================================================
            // 1. Generate barcode: 2 lines
            // 2. Save to file: (included above)
            // 3. Read barcode: 2 lines
            // 4. License registration: 1 line
            // Total: ~5 lines, one product

            // Reduction: ~85% less code, one product instead of two
        }
    }
}
