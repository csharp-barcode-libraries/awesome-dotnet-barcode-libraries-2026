/**
 * Neodynamic Barcode vs IronBarcode: Split SDK Configuration
 *
 * This file demonstrates the split product model in Neodynamic where barcode
 * generation and reading are separate purchases with separate NuGet packages
 * and separate license keys, compared to IronBarcode's unified approach.
 *
 * Key takeaway: Neodynamic requires two separate SDKs (and purchases) for
 * generation and reading. IronBarcode includes both in a single package.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

// ============================================================================
// PART 1: NEODYNAMIC SPLIT SDK SETUP
// ============================================================================

namespace NeodynamicBarcodeExample
{
    /*
     * PROJECT FILE (.csproj) REQUIREMENTS:
     *
     * For generation only:
     * <PackageReference Include="Neodynamic.SDK.Barcode" Version="x.x.x" />
     *
     * For reading only (1D barcodes only):
     * <PackageReference Include="Neodynamic.SDK.BarcodeReader" Version="x.x.x" />
     *
     * For both capabilities (most common need):
     * <PackageReference Include="Neodynamic.SDK.Barcode" Version="x.x.x" />
     * <PackageReference Include="Neodynamic.SDK.BarcodeReader" Version="x.x.x" />
     *
     * Note: Two packages, two purchases, two license keys
     */

    // Install: dotnet add package Neodynamic.SDK.Barcode
    using Neodynamic.SDK.Barcode;

    // Install: dotnet add package Neodynamic.SDK.BarcodeReader
    // (separate purchase required)
    using Neodynamic.SDK.BarcodeReader;

    /// <summary>
    /// Neodynamic requires separate SDK setup for generation and reading.
    /// Each SDK has its own license configuration.
    /// </summary>
    public class NeodynamicSplitSdkSetup
    {
        /// <summary>
        /// Initialize both SDKs - requires two separate license keys
        /// </summary>
        public NeodynamicSplitSdkSetup(
            string generationLicenseOwner,
            string generationLicenseKey,
            string readerLicenseOwner,
            string readerLicenseKey)
        {
            // SDK 1: Barcode Professional (Generation)
            // Requires separate purchase (~$245+)
            BarcodeInfo.LicenseOwner = generationLicenseOwner;
            BarcodeInfo.LicenseKey = generationLicenseKey;

            // SDK 2: Barcode Reader (Recognition)
            // Requires separate purchase (additional cost)
            // Note: Only supports 1D barcodes
            Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseOwner = readerLicenseOwner;
            Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseKey = readerLicenseKey;

            Console.WriteLine("Neodynamic SDKs initialized:");
            Console.WriteLine("  - Barcode Professional SDK (generation)");
            Console.WriteLine("  - Barcode Reader SDK (1D reading only)");
        }

        /// <summary>
        /// Validate that both SDKs are properly licensed
        /// </summary>
        public bool ValidateLicenses()
        {
            bool generationValid = !string.IsNullOrEmpty(BarcodeInfo.LicenseKey);
            bool readerValid = !string.IsNullOrEmpty(
                Neodynamic.SDK.BarcodeReader.BarcodeReader.LicenseKey);

            if (!generationValid)
            {
                Console.WriteLine("WARNING: Generation SDK license not configured");
                Console.WriteLine("  Purchase Barcode Professional SDK to enable generation");
            }

            if (!readerValid)
            {
                Console.WriteLine("WARNING: Reader SDK license not configured");
                Console.WriteLine("  Purchase Barcode Reader SDK to enable 1D reading");
                Console.WriteLine("  Note: Reader SDK does not support QR codes or DataMatrix");
            }

            return generationValid && readerValid;
        }
    }

    /// <summary>
    /// Barcode generation using Neodynamic Barcode Professional SDK
    /// </summary>
    public class NeodynamicBarcodeGeneration
    {
        /// <summary>
        /// Generate a Code128 barcode
        /// </summary>
        public void GenerateCode128(string data, string outputPath)
        {
            var barcode = new BarcodeInfo();
            barcode.Value = data;
            barcode.Symbology = Symbology.Code128;
            barcode.TextAlign = BarcodeTextAlignment.BelowCenter;
            barcode.Dpi = 300;

            Image image = barcode.GetImage();
            image.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine($"Generated Code128: {outputPath}");
        }

        /// <summary>
        /// Generate a QR code - Neodynamic CAN generate QR codes
        /// </summary>
        public void GenerateQrCode(string data, string outputPath)
        {
            var barcode = new BarcodeInfo();
            barcode.Value = data;
            barcode.Symbology = Symbology.QRCode;
            barcode.QRCodeECL = QRCodeErrorCorrectionLevel.H;

            Image image = barcode.GetImage();
            image.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine($"Generated QR Code: {outputPath}");
            Console.WriteLine("NOTE: Neodynamic can GENERATE QR codes");
            Console.WriteLine("      but CANNOT READ them back!");
        }

        /// <summary>
        /// Generate a DataMatrix - Neodynamic CAN generate DataMatrix
        /// </summary>
        public void GenerateDataMatrix(string data, string outputPath)
        {
            var barcode = new BarcodeInfo();
            barcode.Value = data;
            barcode.Symbology = Symbology.DataMatrix;

            Image image = barcode.GetImage();
            image.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine($"Generated DataMatrix: {outputPath}");
            Console.WriteLine("NOTE: Neodynamic can GENERATE DataMatrix");
            Console.WriteLine("      but CANNOT READ it back!");
        }
    }

    /// <summary>
    /// Barcode reading using Neodynamic Barcode Reader SDK
    /// IMPORTANT: Only supports 1D barcodes
    /// </summary>
    public class NeodynamicBarcodeReading
    {
        /// <summary>
        /// Read 1D barcodes from an image
        /// </summary>
        public string[] Read1DBarcodes(string imagePath)
        {
            using var bitmap = new Bitmap(imagePath);

            // Only reads 1D barcodes (Codabar, Code128, EAN, UPC, etc.)
            var results = Neodynamic.SDK.BarcodeReader.BarcodeReader.Read(bitmap);

            if (results == null || !results.Any())
            {
                Console.WriteLine("No 1D barcodes found");
                return Array.Empty<string>();
            }

            return results.Select(r => r.Value).ToArray();
        }

        /// <summary>
        /// Attempt to read QR code - THIS WILL FAIL
        /// Neodynamic Reader SDK does not support 2D barcodes
        /// </summary>
        public string AttemptQrCodeRead(string imagePath)
        {
            Console.WriteLine("Attempting to read QR code with Neodynamic Reader...");

            using var bitmap = new Bitmap(imagePath);
            var results = Neodynamic.SDK.BarcodeReader.BarcodeReader.Read(bitmap);

            if (results == null || !results.Any())
            {
                Console.WriteLine("FAILED: No barcode found");
                Console.WriteLine("REASON: Neodynamic Reader SDK only supports 1D barcodes");
                Console.WriteLine("        QR codes, DataMatrix, PDF417, etc. are NOT supported");
                return null;
            }

            // If this somehow returns a result, it's detecting the QR as noise
            // or misidentifying it as a 1D barcode (incorrect result)
            return results.First().Value;
        }

        /// <summary>
        /// Document the supported vs unsupported formats
        /// </summary>
        public void ListSupportedFormats()
        {
            Console.WriteLine("=== NEODYNAMIC READER SDK FORMAT SUPPORT ===");
            Console.WriteLine();
            Console.WriteLine("SUPPORTED (1D Linear Barcodes):");
            Console.WriteLine("  - Codabar");
            Console.WriteLine("  - Code 128 (A, B, C)");
            Console.WriteLine("  - Code 39, Code 39 Extended");
            Console.WriteLine("  - Code 93, Code 93 Extended");
            Console.WriteLine("  - EAN-8, EAN-13");
            Console.WriteLine("  - Industrial 2 of 5");
            Console.WriteLine("  - Interleaved 2 of 5");
            Console.WriteLine("  - UPC-A, UPC-E");
            Console.WriteLine("  - MSI/Plessey");
            Console.WriteLine("  - Pharmacode");
            Console.WriteLine();
            Console.WriteLine("NOT SUPPORTED (2D Barcodes):");
            Console.WriteLine("  - QR Code");
            Console.WriteLine("  - DataMatrix");
            Console.WriteLine("  - PDF417");
            Console.WriteLine("  - Aztec Code");
            Console.WriteLine("  - MaxiCode");
            Console.WriteLine("  - Micro QR");
            Console.WriteLine();
            Console.WriteLine("If you need to read 2D barcodes, you need a different library.");
        }
    }

    /// <summary>
    /// Demonstrates the irony: Neodynamic can GENERATE 2D codes but not READ them
    /// </summary>
    public class NeodynamicGenerateVsReadMismatch
    {
        public void DemonstrateTheMismatch()
        {
            Console.WriteLine("=== NEODYNAMIC GENERATE vs READ MISMATCH ===");
            Console.WriteLine();
            Console.WriteLine("Barcode Professional SDK (Generation) supports:");
            Console.WriteLine("  1D: Code128, EAN, UPC, Code39, etc.");
            Console.WriteLine("  2D: QR Code, DataMatrix, PDF417, Aztec, etc.");
            Console.WriteLine();
            Console.WriteLine("Barcode Reader SDK (Reading) supports:");
            Console.WriteLine("  1D: Code128, EAN, UPC, Code39, etc.");
            Console.WriteLine("  2D: NONE");
            Console.WriteLine();
            Console.WriteLine("This means:");
            Console.WriteLine("  - You CAN generate a QR code with Neodynamic");
            Console.WriteLine("  - You CANNOT read it back with Neodynamic");
            Console.WriteLine("  - You need a THIRD library for 2D reading");
            Console.WriteLine();
            Console.WriteLine("Practical impact:");
            Console.WriteLine("  - Generate QR code for product label");
            Console.WriteLine("  - Send to customer");
            Console.WriteLine("  - Customer scans and sends back");
            Console.WriteLine("  - You cannot read it with same SDK!");
        }

        public void WorkaroundOptions()
        {
            Console.WriteLine("=== OPTIONS FOR 2D READING ===");
            Console.WriteLine();
            Console.WriteLine("If you need to read QR codes/DataMatrix with Neodynamic:");
            Console.WriteLine();
            Console.WriteLine("Option 1: Add another library for 2D reading");
            Console.WriteLine("  - IronBarcode (full read/write, 50+ formats)");
            Console.WriteLine("  - ZXing.Net (open source, manual format spec)");
            Console.WriteLine("  - Another commercial library");
            Console.WriteLine();
            Console.WriteLine("Option 2: Replace Neodynamic entirely");
            Console.WriteLine("  - Use IronBarcode for both generation AND reading");
            Console.WriteLine("  - Single SDK, single license, all formats");
            Console.WriteLine();
            Console.WriteLine("Option 3: Avoid 2D barcodes (not recommended)");
            Console.WriteLine("  - Stick to 1D formats only");
            Console.WriteLine("  - Miss out on QR code adoption");
            Console.WriteLine("  - Limit future capabilities");
        }
    }

    /// <summary>
    /// Cost analysis for the split SDK model
    /// </summary>
    public class NeodynamicCostAnalysis
    {
        public void AnalyzeCosts()
        {
            Console.WriteLine("=== NEODYNAMIC COST BREAKDOWN ===");
            Console.WriteLine();
            Console.WriteLine("Scenario: Need both generation and 2D reading");
            Console.WriteLine();
            Console.WriteLine("Neodynamic approach:");
            Console.WriteLine("  Barcode Professional SDK: ~$245");
            Console.WriteLine("  Barcode Reader SDK: ~$300+ (estimated)");
            Console.WriteLine("  Note: Still cannot read 2D barcodes!");
            Console.WriteLine("  ─────────────────────────────────────");
            Console.WriteLine("  Subtotal: ~$545+ (1D reading only)");
            Console.WriteLine();
            Console.WriteLine("  To add 2D reading (another library): ~$400+");
            Console.WriteLine("  ─────────────────────────────────────");
            Console.WriteLine("  Total for full capability: ~$945+");
            Console.WriteLine();
            Console.WriteLine("IronBarcode approach:");
            Console.WriteLine("  IronBarcode Lite: $749");
            Console.WriteLine("  Includes: Generation + Reading (1D + 2D)");
            Console.WriteLine("  ─────────────────────────────────────");
            Console.WriteLine("  Total for full capability: $749");
            Console.WriteLine();
            Console.WriteLine("Savings with IronBarcode: ~$196+");
            Console.WriteLine("Plus: Simpler (1 package vs 3)");
        }
    }
}


// ============================================================================
// PART 2: IRONBARCODE UNIFIED APPROACH
// ============================================================================

namespace IronBarcodeUnifiedExample
{
    // Install: dotnet add package IronBarcode
    // That's it. One package. Everything included.
    using IronBarCode;

    /// <summary>
    /// IronBarcode provides unified barcode generation AND reading
    /// in a single SDK with a single license.
    /// </summary>
    public class IronBarcodeUnifiedSetup
    {
        /// <summary>
        /// Initialize IronBarcode - one line, one license, all capabilities
        /// </summary>
        public IronBarcodeUnifiedSetup(string licenseKey)
        {
            // Single license covers:
            // - All barcode generation (1D and 2D)
            // - All barcode reading (1D and 2D)
            // - PDF processing
            // - Batch operations
            IronBarCode.License.LicenseKey = licenseKey;

            Console.WriteLine("IronBarcode initialized:");
            Console.WriteLine("  - Generation: All 50+ formats");
            Console.WriteLine("  - Reading: All 50+ formats (including 2D)");
            Console.WriteLine("  - PDF support: Native");
            Console.WriteLine("  - Single license: Yes");
        }
    }

    /// <summary>
    /// Unified barcode service - generate AND read with one SDK
    /// </summary>
    public class IronBarcodeUnifiedService
    {
        // GENERATION: Works for all formats

        public void GenerateCode128(string data, string outputPath)
        {
            BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
                .SaveAsPng(outputPath);
            Console.WriteLine($"Generated Code128: {outputPath}");
        }

        public void GenerateQrCode(string data, string outputPath)
        {
            QRCodeWriter.CreateQrCode(data, 500).SaveAsPng(outputPath);
            Console.WriteLine($"Generated QR Code: {outputPath}");
        }

        public void GenerateDataMatrix(string data, string outputPath)
        {
            BarcodeWriter.CreateBarcode(data, BarcodeEncoding.DataMatrix)
                .SaveAsPng(outputPath);
            Console.WriteLine($"Generated DataMatrix: {outputPath}");
        }

        // READING: Works for all formats (unlike Neodynamic)

        public string[] ReadBarcodes(string imagePath)
        {
            // Auto-detects format, reads ALL barcode types
            var results = BarcodeReader.Read(imagePath);
            return results.Select(r => r.Text).ToArray();
        }

        public string ReadQrCode(string imagePath)
        {
            // QR codes work - no separate SDK needed
            var result = BarcodeReader.Read(imagePath).FirstOrDefault();
            return result?.Text;
        }

        public string ReadDataMatrix(string imagePath)
        {
            // DataMatrix works - no separate SDK needed
            var result = BarcodeReader.Read(imagePath).FirstOrDefault();
            return result?.Text;
        }

        /// <summary>
        /// Round-trip demonstration: generate, then read back
        /// </summary>
        public void RoundTripDemo(string data, string tempPath)
        {
            Console.WriteLine("=== ROUND TRIP DEMONSTRATION ===");
            Console.WriteLine();

            // Generate a QR code
            GenerateQrCode(data, tempPath);
            Console.WriteLine($"Generated QR code with data: {data}");

            // Read it back - same SDK!
            string readBack = ReadQrCode(tempPath);
            Console.WriteLine($"Read back from QR code: {readBack}");

            bool success = data == readBack;
            Console.WriteLine($"Round-trip success: {success}");
            Console.WriteLine();
            Console.WriteLine("With Neodynamic, this would require:");
            Console.WriteLine("  - One SDK for generation");
            Console.WriteLine("  - A DIFFERENT library for QR reading");
            Console.WriteLine("  - Because Neodynamic Reader cannot read QR codes");
        }
    }

    /// <summary>
    /// Comparison summary between split and unified approaches
    /// </summary>
    public class SdkComparisonSummary
    {
        public void PrintComparison()
        {
            Console.WriteLine("=== SPLIT SDK vs UNIFIED SDK COMPARISON ===");
            Console.WriteLine();
            Console.WriteLine("NEODYNAMIC (Split SDK Model):");
            Console.WriteLine("  Packages needed: 2 (generation + reader)");
            Console.WriteLine("  License keys: 2 (separate purchases)");
            Console.WriteLine("  Generation formats: 40+ (1D and 2D)");
            Console.WriteLine("  Reading formats: 1D ONLY (no QR, DataMatrix)");
            Console.WriteLine("  For full 2D reading: Need THIRD library");
            Console.WriteLine("  Total purchases for full capability: 3");
            Console.WriteLine();
            Console.WriteLine("IRONBARCODE (Unified SDK):");
            Console.WriteLine("  Packages needed: 1");
            Console.WriteLine("  License keys: 1");
            Console.WriteLine("  Generation formats: 50+");
            Console.WriteLine("  Reading formats: 50+ (including all 2D)");
            Console.WriteLine("  For full capability: Already included");
            Console.WriteLine("  Total purchases for full capability: 1");
            Console.WriteLine();
            Console.WriteLine("RECOMMENDATION:");
            Console.WriteLine("  If you only need generation: Neodynamic is cheaper");
            Console.WriteLine("  If you need generation + 2D reading: IronBarcode");
            Console.WriteLine("  If you need round-trip (generate then read): IronBarcode");
        }
    }
}
