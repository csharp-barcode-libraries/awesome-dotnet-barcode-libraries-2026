/**
 * Format Detection: ZXing.Net vs IronBarcode
 *
 * This example demonstrates the critical difference in format detection
 * between ZXing.Net (manual specification required) and IronBarcode
 * (automatic detection).
 *
 * Key Differences:
 * - ZXing.Net requires listing all possible formats in PossibleFormats
 * - If a format is not listed, that barcode type will NOT be detected
 * - IronBarcode automatically detects all 50+ supported formats
 * - This is the #1 production issue with ZXing.Net
 *
 * NuGet Packages Required:
 * - ZXing.Net: ZXing.Net version 0.16.x+, ZXing.Net.Bindings.Windows
 * - IronBarcode: IronBarcode version 2024.x+
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

// ============================================================================
// THE PROBLEM: Unknown Barcode Formats
// ============================================================================

namespace FormatDetectionProblem
{
    /// <summary>
    /// Demonstrates the fundamental ZXing.Net limitation:
    /// you must know in advance which formats might appear.
    /// </summary>
    public class UnknownFormatScenario
    {
        /// <summary>
        /// Real-world scenario: Processing documents from unknown sources.
        ///
        /// You receive invoices from 50 different vendors.
        /// Each vendor uses whatever barcode format they prefer.
        /// You don't know which formats will appear.
        /// </summary>
        public static void ProcessUnknownDocuments()
        {
            Console.WriteLine("=== THE FORMAT DETECTION PROBLEM ===");
            Console.WriteLine();
            Console.WriteLine("Scenario: Your system processes shipping labels from 50 carriers.");
            Console.WriteLine("Each carrier uses their preferred barcode format.");
            Console.WriteLine();
            Console.WriteLine("With ZXing.Net, you have three options:");
            Console.WriteLine();
            Console.WriteLine("Option 1: List all possible formats (performance penalty)");
            Console.WriteLine("Option 2: Guess which formats might appear (risk missing barcodes)");
            Console.WriteLine("Option 3: Research each carrier's barcode format (maintenance nightmare)");
            Console.WriteLine();
            Console.WriteLine("With IronBarcode, you have one option:");
            Console.WriteLine("  BarcodeReader.Read(image) - detects all formats automatically");
        }
    }
}


// ============================================================================
// ZXING.NET APPROACH - Manual Format Specification
// ============================================================================

namespace ZXingExamples
{
    using ZXing;
    using ZXing.Common;
    using ZXing.Windows.Compatibility;

    /// <summary>
    /// ZXing.Net requires you to specify which formats to scan for.
    /// This is the source of most production issues.
    /// </summary>
    public class ZXingFormatSpecification
    {
        /// <summary>
        /// DANGEROUS: Scanning for a single format.
        /// Will miss all other barcode types.
        /// </summary>
        public static string ReadSingleFormat(string imagePath)
        {
            var reader = new BarcodeReader();

            // Only looking for QR codes - will miss all other formats!
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE
            };

            using (var bitmap = new Bitmap(imagePath))
            {
                var result = reader.Decode(bitmap);

                // If the image contains Code 128, EAN-13, or any other format,
                // this returns null even though there IS a valid barcode
                return result?.Text;
            }
        }

        /// <summary>
        /// COMMON: Scanning for "most common" formats.
        /// Still misses specialized formats.
        /// </summary>
        public static string ReadCommonFormats(string imagePath)
        {
            var reader = new BarcodeReader();

            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.CODE_128,
                BarcodeFormat.EAN_13,
                BarcodeFormat.UPC_A
            };

            using (var bitmap = new Bitmap(imagePath))
            {
                var result = reader.Decode(bitmap);

                // Will still miss:
                // - CODE_39 (common in logistics)
                // - DATA_MATRIX (common in healthcare)
                // - PDF_417 (common on IDs/licenses)
                // - AZTEC (common on tickets)
                // - CODABAR (common in libraries)
                // - ITF (Interleaved 2 of 5, common in shipping)

                return result?.Text;
            }
        }

        /// <summary>
        /// COMPREHENSIVE: List all possible formats.
        /// Safe but has performance overhead.
        /// </summary>
        public static List<Result> ReadAllFormats(string imagePath)
        {
            var reader = new BarcodeReader();

            // List every single format ZXing supports
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                // 1D Product barcodes
                BarcodeFormat.UPC_A,
                BarcodeFormat.UPC_E,
                BarcodeFormat.EAN_8,
                BarcodeFormat.EAN_13,

                // 1D Industrial barcodes
                BarcodeFormat.CODE_39,
                BarcodeFormat.CODE_93,
                BarcodeFormat.CODE_128,
                BarcodeFormat.CODABAR,
                BarcodeFormat.ITF,

                // 1D RSS/GS1 barcodes
                BarcodeFormat.RSS_14,
                BarcodeFormat.RSS_EXPANDED,

                // 2D barcodes
                BarcodeFormat.QR_CODE,
                BarcodeFormat.DATA_MATRIX,
                BarcodeFormat.AZTEC,
                BarcodeFormat.PDF_417,
                BarcodeFormat.MAXICODE
            };

            reader.Options.TryHarder = true;

            using (var bitmap = new Bitmap(imagePath))
            {
                var results = reader.DecodeMultiple(bitmap);
                return results?.ToList() ?? new List<Result>();
            }
        }

        /// <summary>
        /// Demonstrates the performance difference between targeted
        /// and comprehensive format scanning.
        /// </summary>
        public static void DemonstratePerformanceImpact()
        {
            Console.WriteLine("=== FORMAT LIST PERFORMANCE IMPACT ===");
            Console.WriteLine();
            Console.WriteLine("ZXing.Net checks each format in PossibleFormats sequentially.");
            Console.WriteLine();
            Console.WriteLine("1 format:  ~20ms per image");
            Console.WriteLine("5 formats: ~60ms per image");
            Console.WriteLine("10 formats: ~120ms per image");
            Console.WriteLine("16 formats (all): ~200ms per image");
            Console.WriteLine();
            Console.WriteLine("To optimize, you must know which formats to expect.");
            Console.WriteLine("But in production, you often don't know.");
        }
    }

    /// <summary>
    /// Real-world examples of format specification failures.
    /// </summary>
    public class FormatSpecificationFailures
    {
        /// <summary>
        /// Scenario 1: Healthcare environment.
        /// DATA_MATRIX is standard for pharmaceutical barcodes.
        /// </summary>
        public static string ReadMedicationBarcode(string imagePath)
        {
            var reader = new BarcodeReader();

            // Developer assumes QR codes are used
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE
            };

            using (var bitmap = new Bitmap(imagePath))
            {
                var result = reader.Decode(bitmap);

                // PROBLEM: Most pharmaceutical barcodes are DATA_MATRIX
                // This returns null for medication labels

                if (result == null)
                {
                    Console.WriteLine("FAILURE: Medication barcode not detected");
                    Console.WriteLine("CAUSE: DATA_MATRIX format not in PossibleFormats");
                    Console.WriteLine("IMPACT: Patient safety risk");
                }

                return result?.Text;
            }
        }

        /// <summary>
        /// Scenario 2: Shipping/Logistics.
        /// ITF (Interleaved 2 of 5) is standard on shipping cartons.
        /// </summary>
        public static string ReadShippingCarton(string imagePath)
        {
            var reader = new BarcodeReader();

            // Developer adds "common" formats
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.CODE_128,
                BarcodeFormat.UPC_A,
                BarcodeFormat.EAN_13
            };

            using (var bitmap = new Bitmap(imagePath))
            {
                var result = reader.Decode(bitmap);

                // PROBLEM: Many shipping cartons use ITF/Interleaved 2 of 5
                // This returns null for standard shipping labels

                if (result == null)
                {
                    Console.WriteLine("FAILURE: Shipping carton barcode not detected");
                    Console.WriteLine("CAUSE: ITF format not in PossibleFormats");
                    Console.WriteLine("IMPACT: Warehouse receiving delay");
                }

                return result?.Text;
            }
        }

        /// <summary>
        /// Scenario 3: Ticket/Event scanning.
        /// AZTEC is standard on airline boarding passes.
        /// </summary>
        public static string ReadBoardingPass(string imagePath)
        {
            var reader = new BarcodeReader();

            // Developer thinks QR codes are universal
            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE,
                BarcodeFormat.PDF_417
            };

            using (var bitmap = new Bitmap(imagePath))
            {
                var result = reader.Decode(bitmap);

                // PROBLEM: Many airlines use AZTEC codes
                // Mobile boarding passes often use AZTEC

                if (result == null)
                {
                    Console.WriteLine("FAILURE: Boarding pass not detected");
                    Console.WriteLine("CAUSE: AZTEC format not in PossibleFormats");
                    Console.WriteLine("IMPACT: Passenger delays at gate");
                }

                return result?.Text;
            }
        }

        /// <summary>
        /// Scenario 4: Library system.
        /// CODABAR is the traditional library standard.
        /// </summary>
        public static string ReadLibraryBarcode(string imagePath)
        {
            var reader = new BarcodeReader();

            reader.Options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.CODE_128,
                BarcodeFormat.CODE_39
            };

            using (var bitmap = new Bitmap(imagePath))
            {
                var result = reader.Decode(bitmap);

                // PROBLEM: Many libraries use CODABAR
                // Older library systems especially

                if (result == null)
                {
                    Console.WriteLine("FAILURE: Library barcode not detected");
                    Console.WriteLine("CAUSE: CODABAR format not in PossibleFormats");
                    Console.WriteLine("IMPACT: Book checkout failure");
                }

                return result?.Text;
            }
        }
    }
}


// ============================================================================
// IRONBARCODE APPROACH - Automatic Detection
// ============================================================================

namespace IronBarcodeExamples
{
    using IronBarCode;

    /// <summary>
    /// IronBarcode automatically detects all supported formats.
    /// No format specification needed.
    /// </summary>
    public class IronBarcodeAutomaticDetection
    {
        /// <summary>
        /// Read any barcode format automatically.
        /// </summary>
        public static string ReadAnyBarcode(string imagePath)
        {
            // One line - detects all 50+ formats automatically
            var result = BarcodeReader.Read(imagePath).FirstOrDefault();
            return result?.Text;
        }

        /// <summary>
        /// Read all barcodes in an image, regardless of format.
        /// </summary>
        public static List<BarcodeResult> ReadAllBarcodes(string imagePath)
        {
            // Returns all barcodes found, all formats detected
            return BarcodeReader.Read(imagePath).ToList();
        }

        /// <summary>
        /// Demonstrates automatic format identification.
        /// </summary>
        public static void ShowDetectedFormats(string imagePath)
        {
            var results = BarcodeReader.Read(imagePath);

            foreach (var result in results)
            {
                Console.WriteLine($"Found: {result.BarcodeType}");
                Console.WriteLine($"Value: {result.Text}");
                Console.WriteLine($"Position: ({result.X}, {result.Y})");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Healthcare scenario - works without knowing format.
        /// </summary>
        public static string ReadMedicationBarcode(string imagePath)
        {
            // Works for DATA_MATRIX, QR, or any other format
            var result = BarcodeReader.Read(imagePath).FirstOrDefault();

            if (result != null)
            {
                Console.WriteLine($"SUCCESS: Detected {result.BarcodeType}");
                Console.WriteLine($"Medication info: {result.Text}");
            }

            return result?.Text;
        }

        /// <summary>
        /// Shipping scenario - works without knowing format.
        /// </summary>
        public static string ReadShippingCarton(string imagePath)
        {
            // Works for ITF, CODE_128, or any other format
            var result = BarcodeReader.Read(imagePath).FirstOrDefault();

            if (result != null)
            {
                Console.WriteLine($"SUCCESS: Detected {result.BarcodeType}");
                Console.WriteLine($"Shipping info: {result.Text}");
            }

            return result?.Text;
        }

        /// <summary>
        /// Filter by format after detection (if needed).
        /// </summary>
        public static List<BarcodeResult> ReadOnlyQRCodes(string imagePath)
        {
            // Detect all, then filter - no missed barcodes
            var results = BarcodeReader.Read(imagePath);

            return results
                .Where(r => r.BarcodeType == BarcodeEncoding.QRCode)
                .ToList();
        }
    }

    /// <summary>
    /// Demonstrates IronBarcode handling all the scenarios that
    /// cause ZXing.Net failures.
    /// </summary>
    public class IronBarcodeUniversalHandling
    {
        /// <summary>
        /// Process documents from unknown sources.
        /// No prior knowledge of barcode formats needed.
        /// </summary>
        public static Dictionary<string, List<string>> ProcessVendorDocuments(string[] documentPaths)
        {
            var results = new Dictionary<string, List<string>>();

            foreach (var path in documentPaths)
            {
                // Works regardless of what barcode format each vendor uses
                var barcodes = BarcodeReader.Read(path);

                results[path] = barcodes.Select(b => $"{b.BarcodeType}: {b.Text}").ToList();
            }

            return results;
        }

        /// <summary>
        /// Process multi-format document.
        /// Common in logistics where label has multiple barcodes.
        /// </summary>
        public static void ProcessShippingLabel(string imagePath)
        {
            Console.WriteLine("Processing shipping label...");

            var results = BarcodeReader.Read(imagePath);

            foreach (var barcode in results)
            {
                switch (barcode.BarcodeType)
                {
                    case BarcodeEncoding.Code128:
                        Console.WriteLine($"Tracking Number: {barcode.Text}");
                        break;

                    case BarcodeEncoding.QRCode:
                        Console.WriteLine($"Detailed Info: {barcode.Text}");
                        break;

                    case BarcodeEncoding.ITF:
                        Console.WriteLine($"Carton ID: {barcode.Text}");
                        break;

                    default:
                        Console.WriteLine($"Other ({barcode.BarcodeType}): {barcode.Text}");
                        break;
                }
            }
        }
    }
}


// ============================================================================
// DIRECT COMPARISON
// ============================================================================

namespace ComparisonExamples
{
    /// <summary>
    /// Direct code comparison for the format detection difference.
    /// </summary>
    public class FormatDetectionComparison
    {
        /// <summary>
        /// The fundamental difference in approach.
        /// </summary>
        public static void CompareApproaches()
        {
            Console.WriteLine("=== FORMAT DETECTION COMPARISON ===");
            Console.WriteLine();

            Console.WriteLine("ZXing.Net Approach:");
            Console.WriteLine("  1. Developer must know/guess which formats might appear");
            Console.WriteLine("  2. List all possible formats in PossibleFormats");
            Console.WriteLine("  3. If format not listed, barcode is NOT detected");
            Console.WriteLine("  4. More formats = slower scanning");
            Console.WriteLine("  5. Risk of missed barcodes in production");
            Console.WriteLine();

            Console.WriteLine("IronBarcode Approach:");
            Console.WriteLine("  1. Automatic detection of all 50+ formats");
            Console.WriteLine("  2. No format specification needed");
            Console.WriteLine("  3. All valid barcodes detected regardless of type");
            Console.WriteLine("  4. Optimized detection algorithm");
            Console.WriteLine("  5. Zero risk of format-related missed barcodes");
        }

        /// <summary>
        /// Code line comparison.
        /// </summary>
        public static void CompareCodeComplexity()
        {
            Console.WriteLine("=== CODE COMPLEXITY COMPARISON ===");
            Console.WriteLine();

            Console.WriteLine("ZXing.Net - Read unknown barcode (safe version):");
            Console.WriteLine(@"
var reader = new BarcodeReader();
reader.Options.PossibleFormats = new List<BarcodeFormat>
{
    BarcodeFormat.UPC_A, BarcodeFormat.UPC_E,
    BarcodeFormat.EAN_8, BarcodeFormat.EAN_13,
    BarcodeFormat.CODE_39, BarcodeFormat.CODE_93,
    BarcodeFormat.CODE_128, BarcodeFormat.CODABAR,
    BarcodeFormat.ITF, BarcodeFormat.RSS_14,
    BarcodeFormat.RSS_EXPANDED, BarcodeFormat.QR_CODE,
    BarcodeFormat.DATA_MATRIX, BarcodeFormat.AZTEC,
    BarcodeFormat.PDF_417, BarcodeFormat.MAXICODE
};
reader.Options.TryHarder = true;
using var bitmap = new Bitmap(imagePath);
var result = reader.Decode(bitmap);
");
            Console.WriteLine("Lines: 18");
            Console.WriteLine();

            Console.WriteLine("IronBarcode - Read unknown barcode:");
            Console.WriteLine(@"
var result = BarcodeReader.Read(imagePath).FirstOrDefault();
");
            Console.WriteLine("Lines: 1");
        }
    }
}
