/**
 * API Configuration Verbosity: Aspose.BarCode vs IronBarcode
 *
 * This example demonstrates the API complexity differences between
 * Aspose.BarCode's verbose configuration model and IronBarcode's
 * streamlined one-liner approach.
 *
 * Key Differences:
 * - Aspose.BarCode requires explicit configuration of multiple parameters
 * - IronBarcode uses sensible defaults with optional customization
 * - Line count comparison: 15-25 lines vs 2-5 lines for common operations
 *
 * NuGet Packages Required:
 * - Aspose.BarCode: Aspose.BarCode version 24.x+
 * - IronBarcode: IronBarcode version 2024.x+
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

// ============================================================================
// EXAMPLE 1: Basic Code128 Barcode Generation
// ============================================================================

namespace BasicGeneration
{
    /// <summary>
    /// Aspose.BarCode requires explicit configuration for basic generation.
    /// Multiple parameters must be set even for simple barcodes.
    /// </summary>
    public class AsposeExample
    {
        public void GenerateCode128(string data, string outputPath)
        {
            // Create generator with explicit symbology
            var generator = new Aspose.BarCode.Generation.BarcodeGenerator(
                Aspose.BarCode.Generation.EncodeTypes.Code128,
                data);

            // Configure basic appearance parameters
            generator.Parameters.Barcode.XDimension.Pixels = 2;
            generator.Parameters.Barcode.BarHeight.Pixels = 100;

            // Configure code text display
            generator.Parameters.Barcode.CodeTextParameters.Location =
                Aspose.BarCode.Generation.CodeLocation.Below;
            generator.Parameters.Barcode.CodeTextParameters.Font.FamilyName = "Arial";
            generator.Parameters.Barcode.CodeTextParameters.Font.Size.Pixels = 12;

            // Configure margins/padding
            generator.Parameters.Barcode.Padding.Left.Pixels = 10;
            generator.Parameters.Barcode.Padding.Right.Pixels = 10;
            generator.Parameters.Barcode.Padding.Top.Pixels = 10;
            generator.Parameters.Barcode.Padding.Bottom.Pixels = 10;

            // Configure colors
            generator.Parameters.BackColor = Color.White;
            generator.Parameters.Barcode.BarColor = Color.Black;

            // Generate and save
            generator.Save(outputPath, Aspose.BarCode.Generation.BarCodeImageFormat.Png);
        }
    }

    /// <summary>
    /// IronBarcode achieves the same result with minimal configuration.
    /// Sensible defaults eliminate boilerplate code.
    /// </summary>
    public class IronBarcodeExample
    {
        public void GenerateCode128(string data, string outputPath)
        {
            // One-liner with automatic configuration
            IronBarCode.BarcodeWriter.CreateBarcode(data, IronBarCode.BarcodeEncoding.Code128)
                .SaveAsPng(outputPath);
        }

        // With optional customization if needed
        public void GenerateCode128WithCustomization(string data, string outputPath)
        {
            IronBarCode.BarcodeWriter.CreateBarcode(data, IronBarCode.BarcodeEncoding.Code128)
                .ResizeTo(400, 100)
                .SetMargins(10)
                .SaveAsPng(outputPath);
        }
    }
}


// ============================================================================
// EXAMPLE 2: QR Code with Custom Styling
// ============================================================================

namespace QrCodeStyling
{
    /// <summary>
    /// Aspose.BarCode QR configuration requires understanding multiple
    /// nested parameter objects and QR-specific settings.
    /// </summary>
    public class AsposeExample
    {
        public void GenerateStyledQrCode(string data, string outputPath)
        {
            var generator = new Aspose.BarCode.Generation.BarcodeGenerator(
                Aspose.BarCode.Generation.EncodeTypes.QR,
                data);

            // QR-specific encoding settings
            generator.Parameters.Barcode.QR.QrEncodeMode =
                Aspose.BarCode.Generation.QREncodeMode.Auto;
            generator.Parameters.Barcode.QR.QrErrorLevel =
                Aspose.BarCode.Generation.QRErrorLevel.LevelH;
            generator.Parameters.Barcode.QR.QrVersion =
                Aspose.BarCode.Generation.QRVersion.Auto;

            // Dimension configuration
            generator.Parameters.Barcode.XDimension.Pixels = 10;

            // Color configuration
            generator.Parameters.Barcode.BarColor = Color.DarkBlue;
            generator.Parameters.BackColor = Color.White;

            // Border configuration
            generator.Parameters.Border.Visible = false;

            // Resolution for high-quality output
            generator.Parameters.Resolution = 300;

            // Save
            generator.Save(outputPath, Aspose.BarCode.Generation.BarCodeImageFormat.Png);
        }

        public void GenerateQrCodeWithLogo(string data, string logoPath, string outputPath)
        {
            var generator = new Aspose.BarCode.Generation.BarcodeGenerator(
                Aspose.BarCode.Generation.EncodeTypes.QR,
                data);

            // Must use high error correction for logo overlay
            generator.Parameters.Barcode.QR.QrErrorLevel =
                Aspose.BarCode.Generation.QRErrorLevel.LevelH;
            generator.Parameters.Barcode.XDimension.Pixels = 10;

            // Generate base QR code image
            using var qrImage = generator.GenerateBarCodeImage();

            // Manual logo overlay - Aspose doesn't have built-in logo support
            using var logo = Image.FromFile(logoPath);
            using var graphics = Graphics.FromImage(qrImage);

            // Calculate logo size (typically 20% of QR code)
            int logoSize = qrImage.Width / 5;
            int logoX = (qrImage.Width - logoSize) / 2;
            int logoY = (qrImage.Height - logoSize) / 2;

            // Draw white background behind logo for visibility
            using var whiteBrush = new SolidBrush(Color.White);
            graphics.FillRectangle(whiteBrush, logoX - 5, logoY - 5, logoSize + 10, logoSize + 10);

            // Draw logo
            graphics.DrawImage(logo, logoX, logoY, logoSize, logoSize);

            // Save final image
            qrImage.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    /// <summary>
    /// IronBarcode provides a dedicated QR code writer with built-in
    /// logo support and simpler color configuration.
    /// </summary>
    public class IronBarcodeExample
    {
        public void GenerateStyledQrCode(string data, string outputPath)
        {
            // Fluent API with built-in styling
            var qr = IronBarCode.QRCodeWriter.CreateQrCode(data, 500,
                IronBarCode.QRCodeWriter.QrErrorCorrectionLevel.Highest);
            qr.ChangeBarCodeColor(Color.DarkBlue);
            qr.SaveAsPng(outputPath);
        }

        public void GenerateQrCodeWithLogo(string data, string logoPath, string outputPath)
        {
            // Built-in logo support - no manual image manipulation required
            var qr = IronBarCode.QRCodeWriter.CreateQrCodeWithLogo(data, logoPath, 500);
            qr.SaveAsPng(outputPath);
        }

        public void GenerateQrCodeWithLogoAndColors(string data, string logoPath, string outputPath)
        {
            // Combined styling with logo
            var qr = IronBarCode.QRCodeWriter.CreateQrCodeWithLogo(data, logoPath, 500);
            qr.ChangeBarCodeColor(Color.DarkBlue);
            qr.SaveAsPng(outputPath);
        }
    }
}


// ============================================================================
// EXAMPLE 3: Reading Barcodes - Format Specification vs Auto-Detection
// ============================================================================

namespace BarcodeReading
{
    /// <summary>
    /// Aspose.BarCode requires explicit format specification.
    /// Developers must list all possible barcode types to scan for.
    /// </summary>
    public class AsposeExample
    {
        public string ReadSingleBarcode(string imagePath)
        {
            // Must specify which formats to look for
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.Code128);

            foreach (var result in reader.ReadBarCodes())
            {
                return result.CodeText;
            }

            return null;
        }

        public List<string> ReadMultipleFormats(string imagePath)
        {
            // For unknown formats, must list all possible types
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.Code128,
                Aspose.BarCode.BarCodeRecognition.DecodeType.Code39Standard,
                Aspose.BarCode.BarCodeRecognition.DecodeType.QR,
                Aspose.BarCode.BarCodeRecognition.DecodeType.EAN13,
                Aspose.BarCode.BarCodeRecognition.DecodeType.EAN8,
                Aspose.BarCode.BarCodeRecognition.DecodeType.UPCA,
                Aspose.BarCode.BarCodeRecognition.DecodeType.UPCE,
                Aspose.BarCode.BarCodeRecognition.DecodeType.DataMatrix,
                Aspose.BarCode.BarCodeRecognition.DecodeType.PDF417,
                Aspose.BarCode.BarCodeRecognition.DecodeType.Aztec);

            var results = new List<string>();
            foreach (var result in reader.ReadBarCodes())
            {
                results.Add($"{result.CodeTypeName}: {result.CodeText}");
            }

            return results;
        }

        public List<string> ReadAllSupportedFormats(string imagePath)
        {
            // AllSupportedTypes includes all 60+ formats - performance impact
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.AllSupportedTypes);

            var results = new List<string>();
            foreach (var result in reader.ReadBarCodes())
            {
                results.Add($"{result.CodeTypeName}: {result.CodeText}");
            }

            return results;
        }
    }

    /// <summary>
    /// IronBarcode automatically detects barcode format.
    /// No need to specify formats in advance.
    /// </summary>
    public class IronBarcodeExample
    {
        public string ReadSingleBarcode(string imagePath)
        {
            // Automatic format detection
            var result = IronBarCode.BarcodeReader.Read(imagePath);
            return result.FirstOrDefault()?.Text;
        }

        public List<string> ReadAllBarcodes(string imagePath)
        {
            // Automatically detects all barcode types present
            var results = IronBarCode.BarcodeReader.Read(imagePath);
            return results.Select(r => $"{r.BarcodeType}: {r.Text}").ToList();
        }

        public List<string> ReadWithSpecificFormatHint(string imagePath)
        {
            // Optional: provide format hint for faster processing if known
            var options = new IronBarCode.BarcodeReaderOptions
            {
                ExpectBarcodeTypes = IronBarCode.BarcodeEncoding.Code128 |
                                     IronBarCode.BarcodeEncoding.QRCode
            };

            var results = IronBarCode.BarcodeReader.Read(imagePath, options);
            return results.Select(r => $"{r.BarcodeType}: {r.Text}").ToList();
        }
    }
}


// ============================================================================
// EXAMPLE 4: Quality Settings - Manual vs Automatic
// ============================================================================

namespace QualityConfiguration
{
    /// <summary>
    /// Aspose.BarCode requires manual quality threshold configuration
    /// for handling damaged or low-quality barcodes.
    /// </summary>
    public class AsposeExample
    {
        public List<string> ReadDamagedBarcodes(string imagePath)
        {
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.AllSupportedTypes);

            // Configure quality settings for difficult images
            reader.QualitySettings = Aspose.BarCode.BarCodeRecognition.QualitySettings.MaxQuality;

            // Additional fine-tuning often required
            reader.QualitySettings.AllowMedianSmoothing = true;
            reader.QualitySettings.MedianSmoothingWindowSize = 5;
            reader.QualitySettings.AllowSaltAndPaperFiltering = true;
            reader.QualitySettings.AllowWhiteSpotsRemoving = true;
            reader.QualitySettings.AllowDecreasedImage = true;
            reader.QualitySettings.AllowDetectScanGap = true;
            reader.QualitySettings.AllowOneDWipedBarsRestoration = true;
            reader.QualitySettings.AllowQRMicroQrRestoration = true;
            reader.QualitySettings.AllowRegularImage = true;

            // If still not working, try different approaches
            reader.QualitySettings.AllowInvertImage = true;
            reader.QualitySettings.AllowComplexBackground = true;
            reader.QualitySettings.AllowDatamatrixIndustrialBarcodes = true;

            var results = new List<string>();
            foreach (var result in reader.ReadBarCodes())
            {
                results.Add($"{result.CodeTypeName}: {result.CodeText}");
            }

            return results;
        }

        public List<string> ReadWithCustomThreshold(string imagePath, int threshold)
        {
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.AllSupportedTypes);

            // Manual threshold tuning - trial and error often required
            reader.QualitySettings = Aspose.BarCode.BarCodeRecognition.QualitySettings.NormalQuality;
            reader.QualitySettings.AllowMedianSmoothing = true;
            reader.QualitySettings.MedianSmoothingWindowSize = threshold;

            var results = new List<string>();
            foreach (var result in reader.ReadBarCodes())
            {
                results.Add(result.CodeText);
            }

            return results;
        }
    }

    /// <summary>
    /// IronBarcode uses ML-powered automatic quality handling.
    /// No manual threshold configuration required.
    /// </summary>
    public class IronBarcodeExample
    {
        public List<string> ReadDamagedBarcodes(string imagePath)
        {
            // ML-powered automatic handling of damaged barcodes
            var results = IronBarCode.BarcodeReader.Read(imagePath);
            return results.Select(r => $"{r.BarcodeType}: {r.Text}").ToList();
        }

        public List<string> ReadWithSpeedHint(string imagePath)
        {
            // Optional: adjust speed/accuracy tradeoff
            var options = new IronBarCode.BarcodeReaderOptions
            {
                // Faster processing, may miss some damaged barcodes
                Speed = IronBarCode.ReadingSpeed.Faster
            };

            var results = IronBarCode.BarcodeReader.Read(imagePath, options);
            return results.Select(r => r.Text).ToList();
        }

        public List<string> ReadWithMaximumEffort(string imagePath)
        {
            // Maximum accuracy for very difficult barcodes
            var options = new IronBarCode.BarcodeReaderOptions
            {
                Speed = IronBarCode.ReadingSpeed.Detailed
            };

            var results = IronBarCode.BarcodeReader.Read(imagePath, options);
            return results.Select(r => r.Text).ToList();
        }
    }
}


// ============================================================================
// EXAMPLE 5: Batch Processing Configuration
// ============================================================================

namespace BatchProcessing
{
    /// <summary>
    /// Aspose.BarCode batch processing requires manual loop and
    /// thread management for parallel processing.
    /// </summary>
    public class AsposeExample
    {
        public Dictionary<string, List<string>> ProcessBatch(string[] imagePaths)
        {
            var results = new Dictionary<string, List<string>>();

            foreach (var path in imagePaths)
            {
                using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                    path,
                    Aspose.BarCode.BarCodeRecognition.DecodeType.Code128,
                    Aspose.BarCode.BarCodeRecognition.DecodeType.QR,
                    Aspose.BarCode.BarCodeRecognition.DecodeType.EAN13);

                var barcodes = new List<string>();
                foreach (var result in reader.ReadBarCodes())
                {
                    barcodes.Add(result.CodeText);
                }

                results[path] = barcodes;
            }

            return results;
        }

        public Dictionary<string, List<string>> ProcessBatchParallel(string[] imagePaths)
        {
            var results = new System.Collections.Concurrent.ConcurrentDictionary<string, List<string>>();

            System.Threading.Tasks.Parallel.ForEach(
                imagePaths,
                new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = 4 },
                path =>
                {
                    using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                        path,
                        Aspose.BarCode.BarCodeRecognition.DecodeType.AllSupportedTypes);

                    var barcodes = new List<string>();
                    foreach (var result in reader.ReadBarCodes())
                    {
                        barcodes.Add(result.CodeText);
                    }

                    results[path] = barcodes;
                });

            return new Dictionary<string, List<string>>(results);
        }
    }

    /// <summary>
    /// IronBarcode accepts arrays directly and handles
    /// parallelization internally for optimal performance.
    /// </summary>
    public class IronBarcodeExample
    {
        public Dictionary<string, List<string>> ProcessBatch(string[] imagePaths)
        {
            // Pass array directly - IronBarcode handles parallelization
            var allResults = IronBarCode.BarcodeReader.Read(imagePaths);

            return allResults
                .Where(r => r.InputPath != null)
                .GroupBy(r => r.InputPath)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.Text).ToList());
        }

        public void ProcessBatchWithProgress(string[] imagePaths, Action<int, int> progressCallback)
        {
            int processed = 0;
            int total = imagePaths.Length;

            foreach (var path in imagePaths)
            {
                var results = IronBarCode.BarcodeReader.Read(path);

                foreach (var barcode in results)
                {
                    Console.WriteLine($"{Path.GetFileName(path)}: {barcode.Text}");
                }

                processed++;
                progressCallback?.Invoke(processed, total);
            }
        }
    }
}


// ============================================================================
// EXAMPLE 6: Generation with Full Customization
// ============================================================================

namespace FullCustomization
{
    /// <summary>
    /// Demonstrates Aspose.BarCode's extensive but verbose configuration API.
    /// All parameters must be set individually through nested objects.
    /// </summary>
    public class AsposeExample
    {
        public void GenerateFullyCustomizedBarcode(string data, string outputPath)
        {
            var generator = new Aspose.BarCode.Generation.BarcodeGenerator(
                Aspose.BarCode.Generation.EncodeTypes.Code128,
                data);

            // === Barcode Bar Settings ===
            generator.Parameters.Barcode.XDimension.Pixels = 3;
            generator.Parameters.Barcode.BarHeight.Pixels = 80;
            generator.Parameters.Barcode.BarWidthReduction.Pixels = 0;
            generator.Parameters.Barcode.FilledBars = true;
            generator.Parameters.Barcode.BarColor = Color.Black;

            // === Code Text Settings ===
            generator.Parameters.Barcode.CodeTextParameters.Location =
                Aspose.BarCode.Generation.CodeLocation.Below;
            generator.Parameters.Barcode.CodeTextParameters.Alignment =
                Aspose.BarCode.Generation.TextAlignment.Center;
            generator.Parameters.Barcode.CodeTextParameters.Font.FamilyName = "Consolas";
            generator.Parameters.Barcode.CodeTextParameters.Font.Size.Pixels = 14;
            generator.Parameters.Barcode.CodeTextParameters.Font.Style = FontStyle.Bold;
            generator.Parameters.Barcode.CodeTextParameters.Color = Color.Black;
            generator.Parameters.Barcode.CodeTextParameters.Space.Pixels = 5;

            // === Padding/Margins ===
            generator.Parameters.Barcode.Padding.Left.Pixels = 15;
            generator.Parameters.Barcode.Padding.Right.Pixels = 15;
            generator.Parameters.Barcode.Padding.Top.Pixels = 10;
            generator.Parameters.Barcode.Padding.Bottom.Pixels = 10;

            // === Background Settings ===
            generator.Parameters.BackColor = Color.White;

            // === Border Settings ===
            generator.Parameters.Border.Visible = true;
            generator.Parameters.Border.Color = Color.Gray;
            generator.Parameters.Border.Width.Pixels = 1;
            generator.Parameters.Border.DashStyle =
                Aspose.BarCode.Generation.BorderDashStyle.Solid;

            // === Resolution Settings ===
            generator.Parameters.Resolution = 300;

            // === Image Format Settings ===
            generator.Parameters.Image.Height.Pixels = 150;
            generator.Parameters.Image.Width.Pixels = 400;

            // Save with format
            generator.Save(outputPath, Aspose.BarCode.Generation.BarCodeImageFormat.Png);
        }
    }

    /// <summary>
    /// IronBarcode provides customization through fluent methods,
    /// with sensible defaults that work for most cases.
    /// </summary>
    public class IronBarcodeExample
    {
        public void GenerateFullyCustomizedBarcode(string data, string outputPath)
        {
            // Start with basic generation
            var barcode = IronBarCode.BarcodeWriter.CreateBarcode(
                data,
                IronBarCode.BarcodeEncoding.Code128);

            // Apply customizations via fluent methods
            barcode.ResizeTo(400, 150)
                   .SetMargins(15, 10)
                   .AddBarcodeValueTextBelowBarcode()
                   .SaveAsPng(outputPath);
        }

        public void GenerateWithColors(string data, string outputPath)
        {
            var barcode = IronBarCode.BarcodeWriter.CreateBarcode(
                data,
                IronBarCode.BarcodeEncoding.Code128);

            barcode.ChangeBarCodeColor(Color.DarkBlue);
            barcode.ChangeBackgroundColor(Color.LightGray);
            barcode.SaveAsPng(outputPath);
        }
    }
}


// ============================================================================
// LINE COUNT COMPARISON SUMMARY
// ============================================================================

/*
 * Operation                         | Aspose Lines | IronBarcode Lines
 * ----------------------------------|--------------|-------------------
 * Basic Code128 generation          | 15-20        | 2-3
 * Styled QR code                    | 12-15        | 3-4
 * QR code with logo                 | 25-30        | 2-3
 * Read single barcode               | 8-10         | 2-3
 * Read multiple formats             | 15-20        | 2-3
 * Quality configuration             | 15-20        | 3-4
 * Batch processing                  | 15-20        | 4-6
 * Full customization                | 35-45        | 5-8
 *
 * Key Insight: IronBarcode's API design prioritizes developer productivity.
 * Common operations that require 15+ lines in Aspose.BarCode typically
 * require 2-5 lines in IronBarcode, without sacrificing capability.
 *
 * For complex customization scenarios, IronBarcode's fluent API remains
 * more readable and maintainable than Aspose.BarCode's nested property model.
 */
