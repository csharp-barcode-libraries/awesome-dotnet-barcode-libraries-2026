/**
 * Basic Barcode Generation and Reading: ZXing.Net vs IronBarcode
 *
 * This example demonstrates fundamental barcode operations using
 * ZXing.Net compared to IronBarcode's streamlined approach.
 *
 * Key Differences:
 * - ZXing.Net requires format specification and image loading
 * - IronBarcode auto-detects formats and accepts file paths directly
 * - ZXing.Net needs System.Drawing or ImageSharp bindings
 * - IronBarcode works cross-platform without additional packages
 *
 * NuGet Packages Required:
 * - ZXing.Net: ZXing.Net version 0.16.x+, ZXing.Net.Bindings.Windows
 * - IronBarcode: IronBarcode version 2024.x+
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// ============================================================================
// ZXING.NET APPROACH
// ============================================================================

namespace ZXingExamples
{
    using ZXing;
    using ZXing.Common;
    using ZXing.Windows.Compatibility;

    /// <summary>
    /// Basic barcode reading using ZXing.Net.
    /// Requires ZXing.Net and ZXing.Net.Bindings.Windows packages.
    /// </summary>
    public class BasicZXingReading
    {
        /// <summary>
        /// Read a single barcode from an image file.
        /// Must specify which formats to scan for.
        /// </summary>
        public static string ReadSingleBarcode(string imagePath)
        {
            var reader = new BarcodeReader();

            // Must specify possible formats - ZXing doesn't auto-detect
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128,
                BarcodeFormat.EAN_13,
                BarcodeFormat.CODE_39
            };

            // Must load image as System.Drawing.Bitmap
            using (var bitmap = new Bitmap(imagePath))
            {
                var result = reader.Decode(bitmap);

                if (result == null)
                {
                    return null; // No barcode found (or wrong format specified)
                }

                return result.Text;
            }
        }

        /// <summary>
        /// Read barcode with additional result information.
        /// </summary>
        public static void ReadWithDetails(string imagePath)
        {
            var reader = new BarcodeReader();

            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128
            };

            using (var bitmap = new Bitmap(imagePath))
            {
                var result = reader.Decode(bitmap);

                if (result != null)
                {
                    Console.WriteLine($"Text: {result.Text}");
                    Console.WriteLine($"Format: {result.BarcodeFormat}");
                    Console.WriteLine($"Timestamp: {result.Timestamp}");

                    // Result points are the corners of the barcode
                    if (result.ResultPoints != null)
                    {
                        foreach (var point in result.ResultPoints)
                        {
                            Console.WriteLine($"Point: ({point.X}, {point.Y})");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read multiple barcodes from an image.
        /// </summary>
        public static List<string> ReadMultipleBarcodes(string imagePath)
        {
            var reader = new BarcodeReader();

            // Must list all possible formats
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128,
                BarcodeFormat.EAN_13,
                BarcodeFormat.UPC_A,
                BarcodeFormat.DATA_MATRIX
            };

            reader.Options.TryHarder = true;

            var results = new List<string>();

            using (var bitmap = new Bitmap(imagePath))
            {
                var decoded = reader.DecodeMultiple(bitmap);

                if (decoded != null)
                {
                    foreach (var result in decoded)
                    {
                        results.Add(result.Text);
                    }
                }
            }

            return results;
        }
    }

    /// <summary>
    /// Basic barcode generation using ZXing.Net.
    /// </summary>
    public class BasicZXingGeneration
    {
        /// <summary>
        /// Generate a Code 128 barcode.
        /// </summary>
        public static void GenerateCode128(string data, string outputPath)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 10
                }
            };

            using (var bitmap = writer.Write(data))
            {
                bitmap.Save(outputPath, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Generate a QR code.
        /// </summary>
        public static void GenerateQRCode(string data, string outputPath)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 10
                }
            };

            // Optional: QR-specific options
            writer.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);

            using (var bitmap = writer.Write(data))
            {
                bitmap.Save(outputPath, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Generate an EAN-13 barcode (requires exactly 12 or 13 digits).
        /// </summary>
        public static void GenerateEAN13(string data, string outputPath)
        {
            // EAN-13 requires numeric data
            if (data.Length != 12 && data.Length != 13)
            {
                throw new ArgumentException("EAN-13 requires 12 or 13 digits");
            }

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.EAN_13,
                Options = new EncodingOptions
                {
                    Width = 200,
                    Height = 100,
                    Margin = 10
                }
            };

            using (var bitmap = writer.Write(data))
            {
                bitmap.Save(outputPath, ImageFormat.Png);
            }
        }
    }

    /// <summary>
    /// Demonstrates stream/byte handling with ZXing.Net.
    /// </summary>
    public class ZXingStreamHandling
    {
        /// <summary>
        /// Read barcode from byte array.
        /// Must convert to Bitmap first.
        /// </summary>
        public static string ReadFromBytes(byte[] imageData)
        {
            var reader = new BarcodeReader();
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128
            };

            using (var ms = new MemoryStream(imageData))
            using (var bitmap = new Bitmap(ms))
            {
                var result = reader.Decode(bitmap);
                return result?.Text;
            }
        }

        /// <summary>
        /// Generate barcode to byte array.
        /// </summary>
        public static byte[] GenerateToBytes(string data, BarcodeFormat format)
        {
            var writer = new BarcodeWriter
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 100
                }
            };

            using (var bitmap = writer.Write(data))
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}


// ============================================================================
// IRONBARCODE APPROACH
// ============================================================================

namespace IronBarcodeExamples
{
    using IronBarCode;

    /// <summary>
    /// Basic barcode reading using IronBarcode.
    /// Single package, automatic format detection.
    /// </summary>
    public class BasicIronBarcodeReading
    {
        /// <summary>
        /// Read a single barcode from an image file.
        /// No format specification needed - automatic detection.
        /// </summary>
        public static string ReadSingleBarcode(string imagePath)
        {
            // One line - automatic format detection
            var result = BarcodeReader.Read(imagePath).FirstOrDefault();
            return result?.Text;
        }

        /// <summary>
        /// Read barcode with additional result information.
        /// </summary>
        public static void ReadWithDetails(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            foreach (var result in results)
            {
                Console.WriteLine($"Text: {result.Text}");
                Console.WriteLine($"Format: {result.BarcodeType}");
                Console.WriteLine($"Position: ({result.X}, {result.Y})");
                Console.WriteLine($"Size: {result.Width}x{result.Height}");
                Console.WriteLine($"Confidence: {result.Confidence}");
            }
        }

        /// <summary>
        /// Read multiple barcodes from an image.
        /// Automatic detection of all formats.
        /// </summary>
        public static List<string> ReadMultipleBarcodes(string imagePath)
        {
            // Automatically detects all formats, finds all barcodes
            var results = BarcodeReader.Read(imagePath);
            return results.Select(r => r.Text).ToList();
        }
    }

    /// <summary>
    /// Basic barcode generation using IronBarcode.
    /// </summary>
    public class BasicIronBarcodeGeneration
    {
        /// <summary>
        /// Generate a Code 128 barcode.
        /// </summary>
        public static void GenerateCode128(string data, string outputPath)
        {
            var barcode = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128);
            barcode.ResizeTo(300, 100);
            barcode.SaveAsPng(outputPath);
        }

        /// <summary>
        /// Generate a QR code.
        /// </summary>
        public static void GenerateQRCode(string data, string outputPath)
        {
            var qr = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode);
            qr.ResizeTo(300, 300);
            qr.SaveAsPng(outputPath);
        }

        /// <summary>
        /// Generate an EAN-13 barcode.
        /// IronBarcode handles checksum automatically.
        /// </summary>
        public static void GenerateEAN13(string data, string outputPath)
        {
            // IronBarcode calculates checksum if 12 digits provided
            var barcode = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.EAN13);
            barcode.SaveAsPng(outputPath);
        }
    }

    /// <summary>
    /// Stream/byte handling with IronBarcode - much simpler.
    /// </summary>
    public class IronBarcodeStreamHandling
    {
        /// <summary>
        /// Read barcode from byte array.
        /// Direct support, no conversion needed.
        /// </summary>
        public static string ReadFromBytes(byte[] imageData)
        {
            var result = BarcodeReader.Read(imageData).FirstOrDefault();
            return result?.Text;
        }

        /// <summary>
        /// Generate barcode to byte array.
        /// </summary>
        public static byte[] GenerateToBytes(string data, BarcodeEncoding format)
        {
            var barcode = BarcodeWriter.CreateBarcode(data, format);
            return barcode.ToPngBinaryData();
        }
    }
}


// ============================================================================
// SIDE-BY-SIDE COMPARISON
// ============================================================================

namespace ComparisonExamples
{
    /// <summary>
    /// Direct comparison demonstrating the API complexity difference.
    /// </summary>
    public class SideBySideComparison
    {
        /// <summary>
        /// Compare the simplest possible barcode reading operation.
        /// </summary>
        public static void CompareBasicReading()
        {
            string imagePath = "barcode.png";

            // ZXing.Net approach (8 lines of meaningful code)
            {
                var reader = new ZXing.BarcodeReader();
                reader.Options.PossibleFormats = new List<ZXing.BarcodeFormat>
                {
                    ZXing.BarcodeFormat.QR_CODE,
                    ZXing.BarcodeFormat.CODE_128
                };
                using var bitmap = new Bitmap(imagePath);
                var result = reader.Decode(bitmap);
                string text = result?.Text ?? "";
            }

            // IronBarcode approach (1 line of meaningful code)
            {
                string text = IronBarCode.BarcodeReader.Read(imagePath).FirstOrDefault()?.Text ?? "";
            }
        }

        /// <summary>
        /// Compare barcode generation with custom sizing.
        /// </summary>
        public static void CompareGeneration()
        {
            string data = "PRODUCT-12345";
            string outputPath = "output.png";

            // ZXing.Net approach
            {
                var writer = new ZXing.BarcodeWriter
                {
                    Format = ZXing.BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 400,
                        Height = 150,
                        Margin = 10
                    }
                };
                using var bitmap = writer.Write(data);
                bitmap.Save(outputPath, ImageFormat.Png);
            }

            // IronBarcode approach
            {
                var barcode = IronBarCode.BarcodeWriter.CreateBarcode(data, IronBarCode.BarcodeEncoding.Code128);
                barcode.ResizeTo(400, 150);
                barcode.SaveAsPng(outputPath);
            }
        }

        /// <summary>
        /// Compare error handling approaches.
        /// </summary>
        public static void CompareErrorHandling()
        {
            Console.WriteLine("=== ERROR HANDLING COMPARISON ===");
            Console.WriteLine();
            Console.WriteLine("ZXing.Net common issues:");
            Console.WriteLine("  - Returns null when no barcode found (no error message)");
            Console.WriteLine("  - Returns null when format not in PossibleFormats list");
            Console.WriteLine("  - System.Drawing exceptions on Linux");
            Console.WriteLine("  - Silently fails on unsupported image formats");
            Console.WriteLine();
            Console.WriteLine("IronBarcode approach:");
            Console.WriteLine("  - Returns empty collection when no barcode found");
            Console.WriteLine("  - Clear exceptions with troubleshooting guidance");
            Console.WriteLine("  - Cross-platform without System.Drawing issues");
            Console.WriteLine("  - Supports all major image formats automatically");
        }
    }
}
