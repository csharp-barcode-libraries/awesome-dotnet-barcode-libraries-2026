// =============================================================================
// DevExpress vs IronBarcode: Generation-Only Limitation Comparison
// =============================================================================
// This file demonstrates the key architectural difference between DevExpress
// Barcode (generation-only UI control) and IronBarcode (read + write SDK).
//
// Key differences highlighted:
// 1. DevExpress Barcode CANNOT read barcodes (confirmed by DevExpress support)
// 2. DevExpress is designed as UI control, not file processing library
// 3. IronBarcode provides unified read/write in single package
// =============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using IronBarCode;

namespace DevExpressGenerationOnlyComparison
{
    // =========================================================================
    // DEVEXPRESS BARCODE: GENERATION ONLY
    // =========================================================================
    // DevExpress BarCodeControl is a UI control for displaying generated
    // barcodes. It does NOT include barcode recognition/reading capability.
    //
    // From DevExpress Support Center:
    // "DevExpress does not provide barcode recognition/reading functionality."
    // =========================================================================

    /// <summary>
    /// Demonstrates what DevExpress Barcode CAN do: generation as UI controls.
    /// </summary>
    public class DevExpressGeneration
    {
        /// <summary>
        /// WinForms barcode generation using BarCodeControl.
        /// This creates a UI control, not a file.
        /// </summary>
        public void GenerateWinFormsBarcode()
        {
            // DevExpress WinForms example (conceptual - requires DevExpress packages)
            /*
            using DevExpress.XtraBars.BarCode;
            using DevExpress.XtraBars.BarCode.Symbologies;

            // Create the control
            var barCodeControl = new BarCodeControl();

            // Symbology must be explicitly configured
            var symbology = new Code128Generator();
            symbology.CharacterSet = Code128CharacterSet.CharsetAuto;
            barCodeControl.Symbology = symbology;

            // Set the data
            barCodeControl.Text = "12345678";

            // Configure display options
            barCodeControl.AutoModule = true;
            barCodeControl.ShowText = true;

            // The control is designed for forms
            this.Controls.Add(barCodeControl);

            // To save as file, you need complex export:
            // 1. Use PrintingSystem export
            // 2. Or use DrawToBitmap (may have quality issues)
            */
        }

        /// <summary>
        /// Blazor barcode generation using DxBarCode component.
        /// </summary>
        public void GenerateBlazorBarcode()
        {
            // Blazor component (in .razor file):
            /*
            @using DevExpress.Blazor

            <DxBarCode
                Type="BarCodeType.Code128"
                Value="12345678"
                Width="200px"
                Height="100px"
                ShowText="true">
            </DxBarCode>
            */

            // Note: This renders as SVG in browser
            // Saving to file requires JavaScript interop
        }

        /// <summary>
        /// WPF barcode generation using BarCode control.
        /// </summary>
        public void GenerateWpfBarcode()
        {
            // WPF XAML:
            /*
            <dxp:BarCode
                Symbology="Code128"
                Text="12345678"
                Module="2"
                ShowText="True"/>
            */

            // Or in C#:
            /*
            using DevExpress.Xpf.Printing;

            var barCode = new BarCode();
            barCode.Symbology = new Code128Generator();
            barCode.Text = "12345678";
            barCode.Module = 2;
            barCode.ShowText = true;
            */
        }
    }

    /// <summary>
    /// Demonstrates what DevExpress Barcode CANNOT do: reading barcodes.
    /// This is explicitly documented by DevExpress support.
    /// </summary>
    public class DevExpressReadingLimitation
    {
        /// <summary>
        /// DevExpress Barcode does NOT support reading.
        /// This is confirmed by DevExpress support team.
        /// </summary>
        public void CannotReadBarcodes()
        {
            // =====================================================================
            // DEVEXPRESS HAS NO BARCODE READING API
            // =====================================================================
            //
            // From DevExpress Support Center ticket responses:
            // "DevExpress BarCode control is designed for barcode generation only."
            // "We do not provide barcode recognition/scanning functionality."
            // "We do not have plans to implement barcode recognition."
            //
            // There is NO:
            // - ReadBarcode()
            // - ScanImage()
            // - DecodeBarcode()
            // - RecognizeBarcode()
            //
            // DevExpress support recommends third-party libraries:
            // - ZXing.Net (open source)
            // - IronBarcode (commercial)
            // - Dynamsoft (commercial)
            //
            // =====================================================================

            throw new NotSupportedException(
                "DevExpress BarCodeControl does not support barcode reading. " +
                "DevExpress support recommends using third-party libraries."
            );
        }

        /// <summary>
        /// DevExpress cannot extract barcodes from PDF documents.
        /// </summary>
        public void CannotReadFromPDF()
        {
            // PDF barcode extraction is not available.
            // Even with DevExpress Reporting/PDF components,
            // there is no barcode reading functionality.

            throw new NotSupportedException(
                "DevExpress cannot read barcodes from PDF documents. " +
                "A third-party barcode reading library is required."
            );
        }

        /// <summary>
        /// DevExpress cannot process barcode images in batch.
        /// </summary>
        public void CannotBatchProcess()
        {
            // No batch reading capability exists.
            // BarCodeControl is a UI control, not a processing library.

            throw new NotSupportedException(
                "DevExpress BarCodeControl cannot process barcode images in batch."
            );
        }
    }

    // =========================================================================
    // IRONBARCODE: UNIFIED READ AND WRITE
    // =========================================================================
    // IronBarcode provides both generation and reading in a single library.
    // No separate products, no third-party integration needed.
    // =========================================================================

    /// <summary>
    /// IronBarcode generation examples - file-focused, not UI control.
    /// </summary>
    public class IronBarcodeGeneration
    {
        /// <summary>
        /// Generate Code128 barcode - single line with file output.
        /// </summary>
        public void GenerateCode128()
        {
            // Install: dotnet add package IronBarcode

            // One line to generate and save
            BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                .SaveAsPng("barcode.png");

            // No UI control setup required
            // No symbology class instantiation
            // Direct file output
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
        /// Generate barcode directly to PDF - no reporting system needed.
        /// </summary>
        public void GenerateToPDF()
        {
            // Direct PDF output without DevExpress Reporting
            BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                .SaveAsPdf("barcode.pdf");
        }

        /// <summary>
        /// Generate to byte array for web API response.
        /// </summary>
        public byte[] GenerateToBytes()
        {
            return BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                .ToPngBinaryData();
        }

        /// <summary>
        /// Batch generation - generate multiple barcodes efficiently.
        /// </summary>
        public void GenerateBatch(string[] values)
        {
            foreach (var value in values)
            {
                BarcodeWriter.CreateBarcode(value, BarcodeEncoding.Code128)
                    .SaveAsPng($"barcode_{value}.png");
            }
        }
    }

    /// <summary>
    /// IronBarcode reading examples - the capability DevExpress lacks.
    /// </summary>
    public class IronBarcodeReading
    {
        /// <summary>
        /// Read barcode from image - automatic format detection.
        /// No need to specify symbology type.
        /// </summary>
        public void ReadFromImage()
        {
            // Automatic format detection - ML-powered
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
        /// Native support - no additional library needed.
        /// </summary>
        public void ReadFromPDF()
        {
            var results = BarcodeReader.Read("document.pdf");

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
            }
        }

        /// <summary>
        /// Read from byte array (e.g., web upload, camera capture).
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
        /// Built-in parallelization for performance.
        /// </summary>
        public Dictionary<string, List<string>> ReadMultipleFiles(string[] filePaths)
        {
            // Process multiple files in one call
            var results = BarcodeReader.Read(filePaths);

            var grouped = new Dictionary<string, List<string>>();
            foreach (var barcode in results)
            {
                var path = barcode.InputPath ?? "unknown";
                if (!grouped.ContainsKey(path))
                    grouped[path] = new List<string>();
                grouped[path].Add(barcode.Text);
            }

            return grouped;
        }
    }

    // =========================================================================
    // REAL-WORLD SCENARIO: INVENTORY MANAGEMENT
    // =========================================================================
    // Demonstrates a workflow requiring both reading and writing.
    // =========================================================================

    /// <summary>
    /// Inventory management workflow comparison.
    /// </summary>
    public class InventoryManagementWorkflow
    {
        /// <summary>
        /// With DevExpress: Incomplete workflow - cannot read barcodes.
        /// </summary>
        public void DevExpressApproach()
        {
            // STEP 1: Generate product barcodes - POSSIBLE
            // Using BarCodeControl in forms or reports

            // STEP 2: Print labels with DevExpress Reporting - POSSIBLE
            // Barcodes render correctly in printed reports

            // STEP 3: Scan products with warehouse scanner - NOT POSSIBLE
            // DevExpress cannot decode barcode images from scanner

            // STEP 4: Match scanned barcode to inventory - NOT POSSIBLE
            // Cannot complete workflow without reading capability

            // To implement this with DevExpress:
            // - Generate labels with DevExpress
            // - Use ZXing.Net or IronBarcode for reading
            // - Manage two different barcode libraries
            // - Different APIs, different patterns
        }

        /// <summary>
        /// With IronBarcode: Complete workflow in single library.
        /// </summary>
        public void IronBarcodeApproach()
        {
            // STEP 1: Generate product barcodes
            var products = new[]
            {
                ("SKU-001", "Widget A"),
                ("SKU-002", "Widget B"),
                ("SKU-003", "Gadget X")
            };

            foreach (var (sku, name) in products)
            {
                var barcode = BarcodeWriter.CreateBarcode(sku, BarcodeEncoding.Code128);
                barcode.AddAnnotationTextBelowBarcode(name);
                barcode.SaveAsPng($"label_{sku}.png");
            }

            // STEP 2: Labels are printed (same files work for printing)

            // STEP 3: Scan received shipment
            var scannedImages = Directory.GetFiles("scanner_output", "*.png");
            var scannedResults = BarcodeReader.Read(scannedImages);

            // STEP 4: Match to inventory
            foreach (var scanned in scannedResults)
            {
                var matchingProduct = Array.Find(products, p => p.Item1 == scanned.Text);
                if (matchingProduct != default)
                {
                    Console.WriteLine($"Received: {matchingProduct.Item2} (SKU: {scanned.Text})");
                }
                else
                {
                    Console.WriteLine($"Unknown product: {scanned.Text}");
                }
            }
        }
    }

    // =========================================================================
    // DEVEXPRESS SUPPORT QUOTES
    // =========================================================================

    /// <summary>
    /// Documented statements from DevExpress support about reading capability.
    /// </summary>
    public static class DevExpressSupportQuotes
    {
        // These are paraphrased from public DevExpress Support Center tickets:

        public const string Quote1 =
            "DevExpress BarCode control is designed for barcode generation only. " +
            "We do not provide barcode recognition/scanning functionality.";

        public const string Quote2 =
            "We do not have plans to implement barcode recognition at this time.";

        public const string Quote3 =
            "For barcode reading, we recommend using third-party libraries " +
            "such as ZXing.Net, IronBarcode, or Dynamsoft.";

        public const string Quote4 =
            "Our BarCodeControl generates barcodes for display and printing. " +
            "Reading barcodes from images is not supported.";
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
        | Capability                | DevExpress Barcode | IronBarcode |
        |---------------------------|-------------------|-------------|
        | Generate 1D barcodes      | Yes               | Yes         |
        | Generate QR codes         | Yes               | Yes         |
        | Generate DataMatrix       | Yes               | Yes         |
        | Generate PDF417           | Yes               | Yes         |
        | Save to PNG               | Complex (export)  | Built-in    |
        | Save to PDF               | Via Reporting     | Built-in    |
        | Read from image           | NO                | Yes         |
        | Read from PDF             | NO                | Yes         |
        | Auto format detection     | N/A (generation)  | Yes         |
        | Batch reading             | NO                | Built-in    |
        | ML error correction       | No                | Yes         |
        | UI control mode           | Primary design    | Optional    |
        | Library/SDK mode          | Not primary       | Primary     |
        | Console app support       | Requires assembly | Native      |
        | Web API support           | Requires assembly | Native      |
        | Standalone license        | No (suite only)   | Yes         |
        */
    }

    // =========================================================================
    // CODE COMPLEXITY COMPARISON
    // =========================================================================

    /// <summary>
    /// Demonstrates code complexity difference for common tasks.
    /// </summary>
    public class CodeComplexityComparison
    {
        /// <summary>
        /// DevExpress: Save barcode to file (not straightforward).
        /// </summary>
        public void DevExpressSaveToFile()
        {
            // DevExpress is UI-control focused, saving to file is awkward:
            /*
            // Option 1: DrawToBitmap (may have quality issues)
            var barCode = new BarCodeControl();
            barCode.Symbology = new Code128Generator();
            barCode.Text = "12345678";

            // Must set size first
            barCode.Size = new Size(300, 100);

            using var bitmap = new Bitmap(barCode.Width, barCode.Height);
            barCode.DrawToBitmap(bitmap, barCode.ClientRectangle);
            bitmap.Save("barcode.png", ImageFormat.Png);

            // Option 2: Use PrintingSystem export
            // Much more complex, involves report creation
            */

            // Total: ~10-20 lines for simple file save
        }

        /// <summary>
        /// IronBarcode: Save barcode to file (straightforward).
        /// </summary>
        public void IronBarcodeSaveToFile()
        {
            // Direct and simple:
            BarcodeWriter.CreateBarcode("12345678", BarcodeEncoding.Code128)
                .SaveAsPng("barcode.png");

            // Total: 2 lines (or 1 line if you prefer)
        }

        /// <summary>
        /// Complete read/write workflow line comparison.
        /// </summary>
        public void CompareLineCount()
        {
            // =====================================================
            // DEVEXPRESS + ZXING.NET (for reading)
            // =====================================================
            // 1. Generate barcode (BarCodeControl): ~8 lines
            // 2. Save to file (DrawToBitmap): ~6 lines
            // 3. Read barcode (ZXing.Net): ~10 lines
            //    - Create reader
            //    - Configure hints
            //    - Process image
            //    - Handle result
            // 4. Suite subscription: N/A (code, but cost)
            // Total: ~24 lines + two libraries to manage

            // =====================================================
            // IRONBARCODE
            // =====================================================
            // 1. Generate barcode: 2 lines
            // 2. Save to file: (included above)
            // 3. Read barcode: 2 lines
            // 4. License setup: 1 line
            // Total: ~5 lines, one library

            // Reduction: ~80% less code, unified API
        }
    }
}
