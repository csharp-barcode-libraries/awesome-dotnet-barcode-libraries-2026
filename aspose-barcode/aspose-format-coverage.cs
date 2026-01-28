/**
 * Format Coverage Comparison: Aspose.BarCode vs IronBarcode
 *
 * This example demonstrates how each library handles barcode format
 * detection and multi-format scenarios.
 *
 * Key Differences:
 * - Aspose.BarCode supports 60+ symbologies (strength)
 * - Aspose.BarCode requires manual format specification
 * - IronBarcode supports 50+ symbologies with automatic detection
 * - IronBarcode's auto-detection eliminates format guessing
 *
 * NuGet Packages Required:
 * - Aspose.BarCode: Aspose.BarCode version 24.x+
 * - IronBarcode: IronBarcode version 2024.x+
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ============================================================================
// EXAMPLE 1: Unknown Barcode Format Reading
// ============================================================================

namespace UnknownFormatReading
{
    /// <summary>
    /// Common real-world scenario: you receive an image and don't know
    /// what type of barcode it contains. Aspose.BarCode requires you to
    /// either guess or scan for all possible formats.
    /// </summary>
    public class AsposeExample
    {
        public string ReadUnknownBarcode(string imagePath)
        {
            // Option A: Guess the most common formats
            // Risk: Miss the barcode if it's not in your list
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.Code128,
                Aspose.BarCode.BarCodeRecognition.DecodeType.QR,
                Aspose.BarCode.BarCodeRecognition.DecodeType.EAN13,
                Aspose.BarCode.BarCodeRecognition.DecodeType.DataMatrix);

            foreach (var result in reader.ReadBarCodes())
            {
                return $"{result.CodeTypeName}: {result.CodeText}";
            }

            return null;
        }

        public string ReadUnknownBarcodeAllTypes(string imagePath)
        {
            // Option B: Scan for all supported types
            // Risk: Slower processing, potential false positives
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.AllSupportedTypes);

            foreach (var result in reader.ReadBarCodes())
            {
                return $"{result.CodeTypeName}: {result.CodeText}";
            }

            return null;
        }
    }

    /// <summary>
    /// IronBarcode automatically detects the barcode format.
    /// No guessing required - the ML model identifies the format.
    /// </summary>
    public class IronBarcodeExample
    {
        public string ReadUnknownBarcode(string imagePath)
        {
            // Automatic format detection - no format specification needed
            var results = IronBarCode.BarcodeReader.Read(imagePath);
            var first = results.FirstOrDefault();

            return first != null ? $"{first.BarcodeType}: {first.Text}" : null;
        }
    }
}


// ============================================================================
// EXAMPLE 2: Multi-Format Document Processing
// ============================================================================

namespace MultiFormatProcessing
{
    /// <summary>
    /// Processing documents that may contain multiple different
    /// barcode types (common in logistics, healthcare, retail).
    /// </summary>
    public class AsposeExample
    {
        // Common retail formats
        private readonly Aspose.BarCode.BarCodeRecognition.BaseDecodeType[] _retailFormats = new[]
        {
            Aspose.BarCode.BarCodeRecognition.DecodeType.EAN13,
            Aspose.BarCode.BarCodeRecognition.DecodeType.EAN8,
            Aspose.BarCode.BarCodeRecognition.DecodeType.UPCA,
            Aspose.BarCode.BarCodeRecognition.DecodeType.UPCE,
            Aspose.BarCode.BarCodeRecognition.DecodeType.Code128,
            Aspose.BarCode.BarCodeRecognition.DecodeType.QR
        };

        // Common logistics formats
        private readonly Aspose.BarCode.BarCodeRecognition.BaseDecodeType[] _logisticsFormats = new[]
        {
            Aspose.BarCode.BarCodeRecognition.DecodeType.Code128,
            Aspose.BarCode.BarCodeRecognition.DecodeType.Code39Standard,
            Aspose.BarCode.BarCodeRecognition.DecodeType.ITF14,
            Aspose.BarCode.BarCodeRecognition.DecodeType.GS1Code128,
            Aspose.BarCode.BarCodeRecognition.DecodeType.DataMatrix,
            Aspose.BarCode.BarCodeRecognition.DecodeType.PDF417
        };

        // Healthcare formats
        private readonly Aspose.BarCode.BarCodeRecognition.BaseDecodeType[] _healthcareFormats = new[]
        {
            Aspose.BarCode.BarCodeRecognition.DecodeType.DataMatrix,
            Aspose.BarCode.BarCodeRecognition.DecodeType.QR,
            Aspose.BarCode.BarCodeRecognition.DecodeType.Aztec,
            Aspose.BarCode.BarCodeRecognition.DecodeType.Code128,
            Aspose.BarCode.BarCodeRecognition.DecodeType.GS1DataMatrix
        };

        public List<BarcodeResult> ProcessRetailDocument(string imagePath)
        {
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath);
            reader.SetBarCodeReadType(_retailFormats);

            return reader.ReadBarCodes()
                .Select(r => new BarcodeResult
                {
                    Type = r.CodeTypeName,
                    Value = r.CodeText
                })
                .ToList();
        }

        public List<BarcodeResult> ProcessLogisticsDocument(string imagePath)
        {
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath);
            reader.SetBarCodeReadType(_logisticsFormats);

            return reader.ReadBarCodes()
                .Select(r => new BarcodeResult
                {
                    Type = r.CodeTypeName,
                    Value = r.CodeText
                })
                .ToList();
        }

        public List<BarcodeResult> ProcessHealthcareDocument(string imagePath)
        {
            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath);
            reader.SetBarCodeReadType(_healthcareFormats);

            return reader.ReadBarCodes()
                .Select(r => new BarcodeResult
                {
                    Type = r.CodeTypeName,
                    Value = r.CodeText
                })
                .ToList();
        }
    }

    /// <summary>
    /// IronBarcode handles all industry scenarios with automatic detection.
    /// No need to maintain industry-specific format lists.
    /// </summary>
    public class IronBarcodeExample
    {
        public List<BarcodeResult> ProcessAnyDocument(string imagePath)
        {
            // Single method handles all industries
            var results = IronBarCode.BarcodeReader.Read(imagePath);

            return results.Select(r => new BarcodeResult
            {
                Type = r.BarcodeType.ToString(),
                Value = r.Text
            }).ToList();
        }

        // If you want to optimize for specific formats, it's optional
        public List<BarcodeResult> ProcessWithFormatHint(
            string imagePath,
            IronBarCode.BarcodeEncoding expectedFormats)
        {
            var options = new IronBarCode.BarcodeReaderOptions
            {
                ExpectBarcodeTypes = expectedFormats
            };

            var results = IronBarCode.BarcodeReader.Read(imagePath, options);

            return results.Select(r => new BarcodeResult
            {
                Type = r.BarcodeType.ToString(),
                Value = r.Text
            }).ToList();
        }
    }

    public class BarcodeResult
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}


// ============================================================================
// EXAMPLE 3: Symbology Generation Across Both Libraries
// ============================================================================

namespace SymbologyGeneration
{
    /// <summary>
    /// Aspose.BarCode supports 60+ symbologies for generation.
    /// Each requires the appropriate EncodeType specification.
    /// </summary>
    public class AsposeExample
    {
        public void GenerateCommonFormats(string data, string outputDir)
        {
            // 1D Linear Barcodes
            GenerateBarcode(data, "code128.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.Code128);
            GenerateBarcode(data, "code39.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.Code39Standard);
            GenerateBarcode(data, "ean13.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.EAN13);
            GenerateBarcode(data, "upca.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.UPCA);

            // 2D Barcodes
            GenerateBarcode(data, "qrcode.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.QR);
            GenerateBarcode(data, "datamatrix.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.DataMatrix);
            GenerateBarcode(data, "pdf417.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.Pdf417);
            GenerateBarcode(data, "aztec.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.Aztec);
        }

        private void GenerateBarcode(string data, string filename, string outputDir,
            Aspose.BarCode.Generation.BaseEncodeType encodeType)
        {
            var generator = new Aspose.BarCode.Generation.BarcodeGenerator(encodeType, data);
            generator.Parameters.Barcode.XDimension.Pixels = 2;
            generator.Save(Path.Combine(outputDir, filename),
                Aspose.BarCode.Generation.BarCodeImageFormat.Png);
        }

        // Aspose.BarCode-exclusive formats (examples)
        public void GenerateSpecializedFormats(string data, string outputDir)
        {
            // MaxiCode - primarily used by UPS
            GenerateBarcode(data, "maxicode.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.MaxiCode);

            // DotCode - for high-speed industrial printing
            GenerateBarcode(data, "dotcode.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.DotCode);

            // Han Xin Code - Chinese standard
            GenerateBarcode(data, "hanxin.png", outputDir,
                Aspose.BarCode.Generation.EncodeTypes.HanXin);
        }
    }

    /// <summary>
    /// IronBarcode supports 50+ symbologies covering all common use cases.
    /// Simpler API with BarcodeEncoding enum.
    /// </summary>
    public class IronBarcodeExample
    {
        public void GenerateCommonFormats(string data, string outputDir)
        {
            // 1D Linear Barcodes
            GenerateBarcode(data, "code128.png", outputDir,
                IronBarCode.BarcodeEncoding.Code128);
            GenerateBarcode(data, "code39.png", outputDir,
                IronBarCode.BarcodeEncoding.Code39);
            GenerateBarcode(data, "ean13.png", outputDir,
                IronBarCode.BarcodeEncoding.EAN13);
            GenerateBarcode(data, "upca.png", outputDir,
                IronBarCode.BarcodeEncoding.UPCA);

            // 2D Barcodes
            GenerateBarcode(data, "qrcode.png", outputDir,
                IronBarCode.BarcodeEncoding.QRCode);
            GenerateBarcode(data, "datamatrix.png", outputDir,
                IronBarCode.BarcodeEncoding.DataMatrix);
            GenerateBarcode(data, "pdf417.png", outputDir,
                IronBarCode.BarcodeEncoding.PDF417);
            GenerateBarcode(data, "aztec.png", outputDir,
                IronBarCode.BarcodeEncoding.Aztec);
        }

        private void GenerateBarcode(string data, string filename, string outputDir,
            IronBarCode.BarcodeEncoding encoding)
        {
            IronBarCode.BarcodeWriter.CreateBarcode(data, encoding)
                .SaveAsPng(Path.Combine(outputDir, filename));
        }
    }
}


// ============================================================================
// EXAMPLE 4: Format Detection Accuracy Comparison
// ============================================================================

namespace FormatDetection
{
    /// <summary>
    /// Demonstrates the difference between manual format specification
    /// and automatic format detection in accuracy and convenience.
    /// </summary>
    public class AsposeExample
    {
        public DetectionResult DetectWithGuessing(string imagePath)
        {
            var result = new DetectionResult();

            // Try common formats first
            var commonFormats = new[]
            {
                Aspose.BarCode.BarCodeRecognition.DecodeType.QR,
                Aspose.BarCode.BarCodeRecognition.DecodeType.Code128,
                Aspose.BarCode.BarCodeRecognition.DecodeType.EAN13
            };

            using (var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath))
            {
                reader.SetBarCodeReadType(commonFormats);
                result.AttemptsWithCommon++;

                foreach (var barcode in reader.ReadBarCodes())
                {
                    result.Found = true;
                    result.Type = barcode.CodeTypeName;
                    result.Value = barcode.CodeText;
                    return result;
                }
            }

            // If not found, try extended formats
            var extendedFormats = new[]
            {
                Aspose.BarCode.BarCodeRecognition.DecodeType.DataMatrix,
                Aspose.BarCode.BarCodeRecognition.DecodeType.PDF417,
                Aspose.BarCode.BarCodeRecognition.DecodeType.Aztec,
                Aspose.BarCode.BarCodeRecognition.DecodeType.Code39Standard
            };

            using (var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath))
            {
                reader.SetBarCodeReadType(extendedFormats);
                result.AttemptsWithExtended++;

                foreach (var barcode in reader.ReadBarCodes())
                {
                    result.Found = true;
                    result.Type = barcode.CodeTypeName;
                    result.Value = barcode.CodeText;
                    return result;
                }
            }

            // Last resort: all types
            using (var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.AllSupportedTypes))
            {
                result.AttemptsWithAll++;

                foreach (var barcode in reader.ReadBarCodes())
                {
                    result.Found = true;
                    result.Type = barcode.CodeTypeName;
                    result.Value = barcode.CodeText;
                    return result;
                }
            }

            result.Found = false;
            return result;
        }
    }

    /// <summary>
    /// IronBarcode's automatic detection finds the format in a single call.
    /// </summary>
    public class IronBarcodeExample
    {
        public DetectionResult DetectAutomatically(string imagePath)
        {
            var result = new DetectionResult
            {
                AttemptsWithCommon = 1  // Always just one attempt
            };

            var barcodes = IronBarCode.BarcodeReader.Read(imagePath);
            var first = barcodes.FirstOrDefault();

            if (first != null)
            {
                result.Found = true;
                result.Type = first.BarcodeType.ToString();
                result.Value = first.Text;
            }
            else
            {
                result.Found = false;
            }

            return result;
        }
    }

    public class DetectionResult
    {
        public bool Found { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int AttemptsWithCommon { get; set; }
        public int AttemptsWithExtended { get; set; }
        public int AttemptsWithAll { get; set; }

        public int TotalAttempts => AttemptsWithCommon + AttemptsWithExtended + AttemptsWithAll;
    }
}


// ============================================================================
// EXAMPLE 5: GS1/GTIN Standards Support
// ============================================================================

namespace GS1Standards
{
    /// <summary>
    /// Both libraries support GS1 standards, but API complexity differs.
    /// GS1 barcodes encode product/logistics data in standardized formats.
    /// </summary>
    public class AsposeExample
    {
        public void GenerateGS1Barcodes(string outputDir)
        {
            // GS1-128 (Code 128 with GS1 application identifiers)
            var gs1Code128 = new Aspose.BarCode.Generation.BarcodeGenerator(
                Aspose.BarCode.Generation.EncodeTypes.GS1Code128,
                "(01)12345678901234(10)BATCH123");
            gs1Code128.Parameters.Barcode.XDimension.Pixels = 2;
            gs1Code128.Save(Path.Combine(outputDir, "gs1-128.png"),
                Aspose.BarCode.Generation.BarCodeImageFormat.Png);

            // GS1 DataMatrix
            var gs1DataMatrix = new Aspose.BarCode.Generation.BarcodeGenerator(
                Aspose.BarCode.Generation.EncodeTypes.GS1DataMatrix,
                "(01)12345678901234(17)251231(10)BATCH");
            gs1DataMatrix.Parameters.Barcode.XDimension.Pixels = 4;
            gs1DataMatrix.Save(Path.Combine(outputDir, "gs1-datamatrix.png"),
                Aspose.BarCode.Generation.BarCodeImageFormat.Png);

            // GS1 QR Code
            var gs1QR = new Aspose.BarCode.Generation.BarcodeGenerator(
                Aspose.BarCode.Generation.EncodeTypes.GS1QR,
                "(01)12345678901234");
            gs1QR.Parameters.Barcode.XDimension.Pixels = 10;
            gs1QR.Save(Path.Combine(outputDir, "gs1-qr.png"),
                Aspose.BarCode.Generation.BarCodeImageFormat.Png);
        }

        public List<GS1Result> ReadGS1Barcodes(string imagePath)
        {
            var results = new List<GS1Result>();

            using var reader = new Aspose.BarCode.BarCodeRecognition.BarCodeReader(
                imagePath,
                Aspose.BarCode.BarCodeRecognition.DecodeType.GS1Code128,
                Aspose.BarCode.BarCodeRecognition.DecodeType.GS1DataMatrix,
                Aspose.BarCode.BarCodeRecognition.DecodeType.GS1QR);

            foreach (var barcode in reader.ReadBarCodes())
            {
                results.Add(new GS1Result
                {
                    Type = barcode.CodeTypeName,
                    RawValue = barcode.CodeText,
                    // Manual AI parsing would be needed for structured data
                });
            }

            return results;
        }
    }

    /// <summary>
    /// IronBarcode handles GS1 with automatic format detection.
    /// </summary>
    public class IronBarcodeExample
    {
        public void GenerateGS1Barcodes(string outputDir)
        {
            // GS1-128
            IronBarCode.BarcodeWriter.CreateBarcode(
                "(01)12345678901234(10)BATCH123",
                IronBarCode.BarcodeEncoding.GS1128)
                .SaveAsPng(Path.Combine(outputDir, "gs1-128.png"));

            // Standard formats with GS1 data also work
            IronBarCode.BarcodeWriter.CreateBarcode(
                "(01)12345678901234",
                IronBarCode.BarcodeEncoding.DataMatrix)
                .SaveAsPng(Path.Combine(outputDir, "gs1-datamatrix.png"));
        }

        public List<GS1Result> ReadGS1Barcodes(string imagePath)
        {
            // Automatic detection handles GS1 variants
            var barcodes = IronBarCode.BarcodeReader.Read(imagePath);

            return barcodes.Select(b => new GS1Result
            {
                Type = b.BarcodeType.ToString(),
                RawValue = b.Text
            }).ToList();
        }
    }

    public class GS1Result
    {
        public string Type { get; set; }
        public string RawValue { get; set; }
    }
}


// ============================================================================
// FORMAT COVERAGE SUMMARY
// ============================================================================

/*
 * Symbology Coverage Comparison
 * =============================
 *
 * Common 1D Barcodes (Both Libraries):
 * - Code 128 (all subsets)
 * - Code 39 (standard and extended)
 * - Code 93
 * - EAN-8, EAN-13
 * - UPC-A, UPC-E
 * - ITF (Interleaved 2 of 5)
 * - Codabar
 * - MSI
 *
 * Common 2D Barcodes (Both Libraries):
 * - QR Code (all versions)
 * - Micro QR Code
 * - DataMatrix (all sizes)
 * - PDF417 (standard and compact)
 * - Micro PDF417
 * - Aztec Code
 *
 * GS1 Standards (Both Libraries):
 * - GS1-128 (EAN-128)
 * - GS1 DataMatrix
 * - GS1 DataBar (RSS)
 *
 * Aspose.BarCode Additional Formats:
 * - MaxiCode (UPS shipping)
 * - DotCode (industrial printing)
 * - Han Xin Code (Chinese standard)
 * - Australia Post
 * - Royal Mail 4-State
 * - USPS OneCode
 * - Swiss Post Parcel
 * - Various regional postal codes
 *
 * Key Insight:
 * =============
 * While Aspose.BarCode supports ~10 more symbologies than IronBarcode,
 * most of these are specialized postal codes or regional formats.
 *
 * For typical business applications (retail, logistics, healthcare,
 * manufacturing), both libraries provide complete format coverage.
 *
 * The practical difference is in API design:
 * - Aspose: Must specify formats manually
 * - IronBarcode: Automatic detection eliminates format specification
 *
 * For most developers, automatic detection provides more value than
 * marginally more symbology support with manual specification.
 */
