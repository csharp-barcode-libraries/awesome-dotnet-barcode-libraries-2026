// =============================================================================
// DevExpress vs IronBarcode: Symbology Configuration Comparison
// =============================================================================
// This file demonstrates the difference in barcode format configuration between
// DevExpress (manual symbology class instantiation) and IronBarcode (automatic
// or enum-based selection with sensible defaults).
//
// Key differences highlighted:
// 1. DevExpress requires explicit symbology generator classes
// 2. DevExpress Module property requires DPI calculations
// 3. IronBarcode uses simple enum selection with auto-sizing
// =============================================================================

using System;
using System.Drawing;
using IronBarCode;

namespace SymbologyConfigurationComparison
{
    // =========================================================================
    // DEVEXPRESS SYMBOLOGY CONFIGURATION
    // =========================================================================
    // DevExpress requires creating symbology generator instances and manually
    // configuring their properties. Each symbology type has different options.
    // =========================================================================

    /// <summary>
    /// Demonstrates DevExpress manual symbology configuration patterns.
    /// </summary>
    public class DevExpressSymbologyConfig
    {
        /// <summary>
        /// Code 128 configuration in DevExpress.
        /// Multiple properties must be understood and configured.
        /// </summary>
        public void ConfigureCode128()
        {
            /*
            using DevExpress.XtraBars.BarCode;
            using DevExpress.XtraBars.BarCode.Symbologies;

            var barCode = new BarCodeControl();

            // Create the symbology generator instance
            var symbology = new Code128Generator();

            // Configure character set
            // Options: CharsetAuto, CharsetA, CharsetB, CharsetC
            symbology.CharacterSet = Code128CharacterSet.CharsetAuto;

            // Add checksum (usually auto for Code 128, but configurable)
            symbology.AddLeadingZero = false;

            // Assign to control
            barCode.Symbology = symbology;

            // Set the actual data
            barCode.Text = "ITEM-12345";

            // Configure the Module property (affects physical size)
            // This is where complexity increases significantly
            barCode.Module = 0.02f; // Document units

            // ShowText controls whether human-readable text appears
            barCode.ShowText = true;
            */
        }

        /// <summary>
        /// QR Code configuration in DevExpress.
        /// Several QR-specific options must be understood.
        /// </summary>
        public void ConfigureQRCode()
        {
            /*
            using DevExpress.XtraBars.BarCode;
            using DevExpress.XtraBars.BarCode.Symbologies;

            var barCode = new BarCodeControl();

            // Create QR symbology generator
            var symbology = new QRCodeGenerator();

            // Error correction level - must understand QR concepts
            // Options: L (7%), M (15%), Q (25%), H (30%)
            symbology.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;

            // Compaction mode - data type optimization
            // Options: Numeric, AlphaNumeric, Byte, Kanji
            symbology.CompactionMode = QRCodeCompactionMode.AlphaNumeric;

            // Version (size) - 1 to 40, or Auto
            symbology.Version = QRCodeVersion.Auto;

            // Assign to control
            barCode.Symbology = symbology;

            // Set data
            barCode.Text = "https://example.com";

            // Module for QR codes affects cell size
            barCode.Module = 3f; // Larger for better scanning
            */
        }

        /// <summary>
        /// DataMatrix configuration in DevExpress.
        /// </summary>
        public void ConfigureDataMatrix()
        {
            /*
            using DevExpress.XtraBars.BarCode;
            using DevExpress.XtraBars.BarCode.Symbologies;

            var barCode = new BarCodeControl();

            // Create DataMatrix symbology
            var symbology = new DataMatrixGenerator();

            // Compaction mode
            symbology.CompactionMode = DataMatrixCompactionMode.ASCII;

            // Matrix size - specific dimensions required
            // Must choose from predefined sizes
            symbology.MatrixSize = DataMatrixSize.Matrix26x26;

            // Assign to control
            barCode.Symbology = symbology;

            // Set data
            barCode.Text = "DM-12345-DATA";
            */
        }

        /// <summary>
        /// PDF417 configuration in DevExpress.
        /// </summary>
        public void ConfigurePDF417()
        {
            /*
            using DevExpress.XtraBars.BarCode;
            using DevExpress.XtraBars.BarCode.Symbologies;

            var barCode = new BarCodeControl();

            // Create PDF417 symbology
            var symbology = new PDF417Generator();

            // Error correction level (0-8)
            symbology.ErrorCorrectionLevel = 5;

            // Columns (1-30)
            symbology.Columns = 10;

            // Rows (3-90, or 0 for auto)
            symbology.Rows = 0; // Auto

            // Compaction mode
            symbology.CompactionMode = PDF417CompactionMode.Text;

            // Truncated mode (Compact PDF417)
            symbology.Truncated = false;

            // Assign to control
            barCode.Symbology = symbology;

            barCode.Text = "PDF417 data here";
            */
        }

        /// <summary>
        /// The Module property complexity in DevExpress.
        /// Understanding this is crucial for print quality.
        /// </summary>
        public void UnderstandModuleProperty()
        {
            /*
            // The Module property in DevExpress determines the width of the
            // narrowest element (bar or space) in document units.

            // Document units depend on the control's container and scale.
            // For printing, you need to calculate based on target DPI.

            // Formula: Module = DesiredWidthInInches * 100
            // Or for specific DPI:
            // Module = DesiredPixelWidth / DPI * 100

            var barCode = new BarCodeControl();

            // For 300 DPI printer, 2-pixel narrow bar:
            // Module = (2 / 300) * 100 = 0.667
            barCode.Module = 0.667f;

            // AutoModule = true lets DevExpress calculate,
            // but may not give optimal print results
            barCode.AutoModule = true;

            // Module affects:
            // - Physical printed size
            // - Scan reliability
            // - Label fit

            // Common issues (from DevExpress Support Center):
            // - "Barcode too small to scan"
            // - "Barcode doesn't fit on label"
            // - "Module calculation for thermal printers"
            */
        }
    }

    // =========================================================================
    // IRONBARCODE SYMBOLOGY CONFIGURATION
    // =========================================================================
    // IronBarcode uses enum-based format selection with sensible defaults.
    // No complex class instantiation or DPI calculations required.
    // =========================================================================

    /// <summary>
    /// IronBarcode simplified symbology configuration.
    /// </summary>
    public class IronBarcodeSymbologyConfig
    {
        /// <summary>
        /// Code 128 in IronBarcode - one line with enum.
        /// </summary>
        public void ConfigureCode128()
        {
            // Simple enum selection - no generator class needed
            var barcode = BarcodeWriter.CreateBarcode(
                "ITEM-12345",
                BarcodeEncoding.Code128
            );

            // Size is specified in pixels, not document units
            barcode.ResizeTo(400, 100);

            // Save directly
            barcode.SaveAsPng("code128.png");

            // Or all in one line:
            BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
                .ResizeTo(400, 100)
                .SaveAsPng("code128_oneliner.png");
        }

        /// <summary>
        /// QR Code in IronBarcode - error correction as parameter.
        /// </summary>
        public void ConfigureQRCode()
        {
            // Error correction level as simple parameter
            var qr = QRCodeWriter.CreateQrCode(
                "https://example.com",
                500, // Size in pixels
                QRCodeWriter.QrErrorCorrectionLevel.Highest // Simple enum
            );

            // Color customization
            qr.ChangeBarCodeColor(Color.DarkBlue);

            qr.SaveAsPng("qrcode.png");

            // Or with logo (DevExpress doesn't support this):
            var qrWithLogo = QRCodeWriter.CreateQrCodeWithLogo(
                "https://example.com",
                "logo.png",
                500
            );
            qrWithLogo.SaveAsPng("qrcode_logo.png");
        }

        /// <summary>
        /// DataMatrix in IronBarcode - automatic sizing.
        /// </summary>
        public void ConfigureDataMatrix()
        {
            // No manual matrix size selection needed
            // IronBarcode automatically selects optimal size
            var dm = BarcodeWriter.CreateBarcode(
                "DM-12345-DATA",
                BarcodeEncoding.DataMatrix
            );

            dm.SaveAsPng("datamatrix.png");

            // Size can be adjusted after creation
            dm.ResizeTo(200, 200);
            dm.SaveAsPng("datamatrix_resized.png");
        }

        /// <summary>
        /// PDF417 in IronBarcode - error correction simplified.
        /// </summary>
        public void ConfigurePDF417()
        {
            // Automatic row/column calculation
            var pdf417 = BarcodeWriter.CreateBarcode(
                "PDF417 data here",
                BarcodeEncoding.PDF417
            );

            pdf417.SaveAsPng("pdf417.png");
        }

        /// <summary>
        /// Size handling in IronBarcode - pixels, not document units.
        /// </summary>
        public void UnderstandSizing()
        {
            // IronBarcode uses straightforward pixel dimensions

            // Create barcode
            var barcode = BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128);

            // Resize to specific pixel dimensions
            barcode.ResizeTo(400, 100);

            // Or resize maintaining aspect ratio
            barcode.ResizeToWidth(400);
            barcode.ResizeToHeight(100);

            // For print at specific DPI:
            // 300 DPI, 2 inch width = 600 pixels
            barcode.ResizeTo(600, 150); // For 2" x 0.5" at 300 DPI

            // No Module calculations required
            // No document unit conversions
            // WYSIWYG pixel sizing
        }
    }

    // =========================================================================
    // SIDE-BY-SIDE CODE COMPARISON
    // =========================================================================

    /// <summary>
    /// Direct comparison of the same task in both libraries.
    /// </summary>
    public class SideBySideComparison
    {
        /// <summary>
        /// Generate Code 128 barcode - both approaches.
        /// </summary>
        public void CompareCode128Generation()
        {
            // =====================================================
            // DEVEXPRESS (8+ lines)
            // =====================================================
            /*
            var barCode = new BarCodeControl();
            var symbology = new Code128Generator();
            symbology.CharacterSet = Code128CharacterSet.CharsetAuto;
            barCode.Symbology = symbology;
            barCode.Text = "ITEM-12345";
            barCode.Module = 0.02f;
            barCode.ShowText = true;
            // Plus export code for file save...
            */

            // =====================================================
            // IRONBARCODE (1-2 lines)
            // =====================================================
            BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128)
                .SaveAsPng("item_barcode.png");
        }

        /// <summary>
        /// Generate QR code with high error correction - both approaches.
        /// </summary>
        public void CompareQRCodeGeneration()
        {
            // =====================================================
            // DEVEXPRESS (10+ lines)
            // =====================================================
            /*
            var barCode = new BarCodeControl();
            var symbology = new QRCodeGenerator();
            symbology.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;
            symbology.CompactionMode = QRCodeCompactionMode.AlphaNumeric;
            symbology.Version = QRCodeVersion.Auto;
            barCode.Symbology = symbology;
            barCode.Text = "https://example.com";
            barCode.Module = 3f;
            barCode.ShowText = false;
            // Plus export code for file save...
            */

            // =====================================================
            // IRONBARCODE (2-3 lines)
            // =====================================================
            var qr = QRCodeWriter.CreateQrCode(
                "https://example.com",
                500,
                QRCodeWriter.QrErrorCorrectionLevel.Highest
            );
            qr.SaveAsPng("qrcode.png");
        }

        /// <summary>
        /// Multiple format generation - demonstrating API consistency.
        /// </summary>
        public void CompareMultipleFormats()
        {
            // =====================================================
            // DEVEXPRESS - Different generator class for each format
            // =====================================================
            /*
            // Code 128
            var code128 = new Code128Generator();
            code128.CharacterSet = Code128CharacterSet.CharsetAuto;

            // QR Code
            var qrCode = new QRCodeGenerator();
            qrCode.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;

            // DataMatrix
            var dataMatrix = new DataMatrixGenerator();
            dataMatrix.MatrixSize = DataMatrixSize.Matrix26x26;

            // PDF417
            var pdf417 = new PDF417Generator();
            pdf417.ErrorCorrectionLevel = 5;

            // Each has different properties to configure
            // Each requires creating a new instance
            // Each has different configuration patterns
            */

            // =====================================================
            // IRONBARCODE - Same pattern for all formats
            // =====================================================

            // Code 128
            BarcodeWriter.CreateBarcode("DATA", BarcodeEncoding.Code128)
                .SaveAsPng("code128.png");

            // QR Code (specialized writer for more options)
            QRCodeWriter.CreateQrCode("DATA", 500)
                .SaveAsPng("qrcode.png");

            // DataMatrix
            BarcodeWriter.CreateBarcode("DATA", BarcodeEncoding.DataMatrix)
                .SaveAsPng("datamatrix.png");

            // PDF417
            BarcodeWriter.CreateBarcode("DATA", BarcodeEncoding.PDF417)
                .SaveAsPng("pdf417.png");

            // Consistent pattern: CreateBarcode(data, format).Save*()
        }
    }

    // =========================================================================
    // CONFIGURATION COMPLEXITY METRICS
    // =========================================================================

    /// <summary>
    /// Quantifying the complexity difference.
    /// </summary>
    public static class ComplexityMetrics
    {
        /*
        | Metric                        | DevExpress | IronBarcode |
        |-------------------------------|------------|-------------|
        | Classes to understand         | 15+        | 2           |
        | Generator classes (symbology) | 20+        | 0 (enums)   |
        | Properties per format         | 5-10       | 1-3         |
        | Lines for basic generation    | 8-10       | 1-2         |
        | DPI/Module calculation        | Required   | Not needed  |
        | Size specified in             | Doc units  | Pixels      |
        | Default configuration         | Manual     | Automatic   |
        | Error correction (QR)         | Enum + set | Parameter   |
        | Matrix size (DataMatrix)      | Manual     | Automatic   |
        | API pattern consistency       | Varies     | Consistent  |
        */

        public const int DevExpressClassesToLearn = 15;
        public const int IronBarcodeClassesToLearn = 2;

        public const int DevExpressLinesForBasicTask = 8;
        public const int IronBarcodeLinesForBasicTask = 2;

        public const string DevExpressSizeUnit = "Document units (requires DPI calculation)";
        public const string IronBarcodeSizeUnit = "Pixels (WYSIWYG)";
    }

    // =========================================================================
    // COMMON DEVEXPRESS SUPPORT ISSUES (from Support Center)
    // =========================================================================

    /// <summary>
    /// Documents common issues developers encounter with DevExpress symbology.
    /// </summary>
    public static class CommonDevExpressIssues
    {
        // Issue 1: Module sizing for print
        public const string Issue1 = "How do I calculate the correct Module value for my label printer?";
        // DevExpress response: Complex formula involving DPI and target physical size

        // Issue 2: Code 128 character set
        public const string Issue2 = "My Code 128 barcode doesn't encode correctly with special characters";
        // DevExpress response: Must understand and configure CharacterSet property

        // Issue 3: QR version selection
        public const string Issue3 = "How do I know which QR version to use?";
        // DevExpress response: Depends on data length and error correction level

        // Issue 4: DataMatrix size
        public const string Issue4 = "My DataMatrix doesn't fit the data";
        // DevExpress response: Must select appropriate MatrixSize from predefined options

        // Issue 5: Export quality
        public const string Issue5 = "Barcode image quality is poor when exported";
        // DevExpress response: Module and export resolution must be configured correctly
    }
}
