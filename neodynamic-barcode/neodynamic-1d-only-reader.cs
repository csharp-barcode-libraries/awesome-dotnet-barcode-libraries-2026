/**
 * Neodynamic Barcode vs IronBarcode: 1D-Only Reader Limitation
 *
 * This file demonstrates the critical limitation of Neodynamic's Barcode Reader SDK:
 * it ONLY supports 1D (linear) barcodes. QR codes, DataMatrix, PDF417, and other
 * 2D barcodes cannot be read.
 *
 * Key takeaway: If your application needs to read QR codes or DataMatrix barcodes,
 * Neodynamic Barcode Reader SDK is not an option. IronBarcode reads all 50+ formats.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.Drawing;
using System.IO;
using System.Linq;

// ============================================================================
// PART 1: NEODYNAMIC READER LIMITATIONS
// ============================================================================

namespace NeodynamicReaderLimitation
{
    // Install: dotnet add package Neodynamic.SDK.BarcodeReader
    using Neodynamic.SDK.BarcodeReader;

    /// <summary>
    /// Demonstrates the 1D-only limitation of Neodynamic Barcode Reader SDK
    /// </summary>
    public class NeodynamicReaderCapabilities
    {
        /// <summary>
        /// List all supported barcode formats (1D only)
        /// </summary>
        public void ListSupportedFormats()
        {
            Console.WriteLine("=== NEODYNAMIC READER SDK: SUPPORTED FORMATS ===");
            Console.WriteLine();
            Console.WriteLine("1D LINEAR BARCODES (Supported):");
            Console.WriteLine("  Codabar");
            Console.WriteLine("  Code 11");
            Console.WriteLine("  Code 128 (A, B, C)");
            Console.WriteLine("  Code 39");
            Console.WriteLine("  Code 39 Extended");
            Console.WriteLine("  Code 93");
            Console.WriteLine("  Code 93 Extended");
            Console.WriteLine("  EAN-8");
            Console.WriteLine("  EAN-13");
            Console.WriteLine("  Industrial 2 of 5");
            Console.WriteLine("  Interleaved 2 of 5");
            Console.WriteLine("  MSI/Plessey");
            Console.WriteLine("  Pharmacode");
            Console.WriteLine("  UPC-A");
            Console.WriteLine("  UPC-E");
        }

        /// <summary>
        /// List unsupported barcode formats (all 2D)
        /// </summary>
        public void ListUnsupportedFormats()
        {
            Console.WriteLine("=== NEODYNAMIC READER SDK: UNSUPPORTED FORMATS ===");
            Console.WriteLine();
            Console.WriteLine("2D BARCODES (NOT Supported - CANNOT Read):");
            Console.WriteLine("  QR Code           - Most common 2D format");
            Console.WriteLine("  DataMatrix        - Common in manufacturing");
            Console.WriteLine("  PDF417            - Used on IDs, boarding passes");
            Console.WriteLine("  Aztec Code        - Used on airline tickets");
            Console.WriteLine("  MaxiCode          - Used by UPS");
            Console.WriteLine("  Micro QR          - Compact QR variant");
            Console.WriteLine("  Micro PDF417      - Compact PDF417 variant");
            Console.WriteLine("  Han Xin Code      - Chinese standard");
            Console.WriteLine("  DotCode           - High-speed printing");
            Console.WriteLine();
            Console.WriteLine("If you need to read ANY of these formats,");
            Console.WriteLine("Neodynamic Reader SDK will not work.");
        }

        /// <summary>
        /// Document the real-world impact of 1D-only limitation
        /// </summary>
        public void ExplainRealWorldImpact()
        {
            Console.WriteLine("=== REAL-WORLD IMPACT OF 1D-ONLY LIMITATION ===");
            Console.WriteLine();

            var useCases = new[]
            {
                ("Mobile payment QR codes", "QR Code", false),
                ("Restaurant menu QR codes", "QR Code", false),
                ("Product authenticity verification", "QR/DataMatrix", false),
                ("Healthcare specimen tracking", "DataMatrix", false),
                ("Electronic components marking", "DataMatrix", false),
                ("Shipping labels (FedEx, UPS)", "PDF417/QR/MaxiCode", false),
                ("Event ticket scanning", "QR/PDF417", false),
                ("ID document verification", "PDF417", false),
                ("Boarding pass scanning", "PDF417/Aztec", false),
                ("Inventory management", "QR Code", false),
                ("Asset tracking", "QR/DataMatrix", false),
                ("Retail POS scanning", "EAN/UPC", true),
                ("Warehouse shelf labels", "Code 128", true),
                ("Library book tracking", "Code 39", true),
            };

            Console.WriteLine("Use Case                           | Format        | Neodynamic");
            Console.WriteLine("-----------------------------------|---------------|------------");

            foreach (var (useCase, format, supported) in useCases)
            {
                string status = supported ? "Can read" : "CANNOT read";
                Console.WriteLine($"{useCase,-35} | {format,-13} | {status}");
            }

            Console.WriteLine();
            Console.WriteLine("As you can see, many modern applications require 2D barcode reading.");
            Console.WriteLine("Neodynamic Reader SDK cannot serve these use cases.");
        }
    }

    /// <summary>
    /// Demonstrates what happens when trying to read 2D barcodes
    /// </summary>
    public class NeodynamicReadAttempts
    {
        /// <summary>
        /// Successfully read a Code 128 barcode (1D - supported)
        /// </summary>
        public string ReadCode128(string imagePath)
        {
            Console.WriteLine("Attempting to read Code 128 barcode...");

            using var bitmap = new Bitmap(imagePath);
            var results = BarcodeReader.Read(bitmap);

            if (results != null && results.Any())
            {
                string value = results.First().Value;
                Console.WriteLine($"SUCCESS: Read Code 128 value: {value}");
                return value;
            }

            Console.WriteLine("No Code 128 barcode found in image");
            return null;
        }

        /// <summary>
        /// Attempt to read a QR code (2D - NOT supported)
        /// </summary>
        public string AttemptQrCodeRead(string imagePath)
        {
            Console.WriteLine("Attempting to read QR code with Neodynamic...");
            Console.WriteLine();

            using var bitmap = new Bitmap(imagePath);
            var results = BarcodeReader.Read(bitmap);

            if (results == null || !results.Any())
            {
                Console.WriteLine("RESULT: No barcode detected");
                Console.WriteLine();
                Console.WriteLine("EXPLANATION:");
                Console.WriteLine("  Neodynamic Barcode Reader SDK does not support QR codes.");
                Console.WriteLine("  The SDK simply cannot recognize 2D barcode patterns.");
                Console.WriteLine("  This is not an error - it's a limitation of the product.");
                Console.WriteLine();
                Console.WriteLine("SOLUTION:");
                Console.WriteLine("  Use IronBarcode, which reads QR codes automatically.");
                return null;
            }

            // If results are found, it's likely misidentification
            Console.WriteLine("WARNING: Unexpected result - may be misidentification");
            return results.First().Value;
        }

        /// <summary>
        /// Attempt to read a DataMatrix (2D - NOT supported)
        /// </summary>
        public string AttemptDataMatrixRead(string imagePath)
        {
            Console.WriteLine("Attempting to read DataMatrix with Neodynamic...");
            Console.WriteLine();

            using var bitmap = new Bitmap(imagePath);
            var results = BarcodeReader.Read(bitmap);

            if (results == null || !results.Any())
            {
                Console.WriteLine("RESULT: No barcode detected");
                Console.WriteLine();
                Console.WriteLine("EXPLANATION:");
                Console.WriteLine("  Neodynamic Barcode Reader SDK does not support DataMatrix.");
                Console.WriteLine("  DataMatrix is widely used in healthcare and manufacturing.");
                Console.WriteLine("  You cannot use Neodynamic for these applications.");
                return null;
            }

            return results.First().Value;
        }

        /// <summary>
        /// Attempt to read a PDF417 (2D - NOT supported)
        /// </summary>
        public string AttemptPdf417Read(string imagePath)
        {
            Console.WriteLine("Attempting to read PDF417 with Neodynamic...");
            Console.WriteLine();

            using var bitmap = new Bitmap(imagePath);
            var results = BarcodeReader.Read(bitmap);

            if (results == null || !results.Any())
            {
                Console.WriteLine("RESULT: No barcode detected");
                Console.WriteLine();
                Console.WriteLine("EXPLANATION:");
                Console.WriteLine("  Neodynamic Barcode Reader SDK does not support PDF417.");
                Console.WriteLine("  PDF417 is used on driver's licenses, boarding passes, etc.");
                Console.WriteLine("  ID verification applications cannot use Neodynamic Reader.");
                return null;
            }

            return results.First().Value;
        }
    }

    /// <summary>
    /// Shows the irony: Neodynamic can GENERATE 2D codes but not READ them
    /// </summary>
    public class GenerateButCannotRead
    {
        // Generation requires: dotnet add package Neodynamic.SDK.Barcode
        // using Neodynamic.SDK.Barcode;

        /// <summary>
        /// Demonstrates the asymmetry between generation and reading
        /// </summary>
        public void DemonstrateAsymmetry()
        {
            Console.WriteLine("=== NEODYNAMIC GENERATION vs READING ASYMMETRY ===");
            Console.WriteLine();
            Console.WriteLine("Barcode Professional SDK (Generation):");
            Console.WriteLine("  Can generate QR codes:      YES");
            Console.WriteLine("  Can generate DataMatrix:    YES");
            Console.WriteLine("  Can generate PDF417:        YES");
            Console.WriteLine("  Can generate Aztec:         YES");
            Console.WriteLine();
            Console.WriteLine("Barcode Reader SDK (Reading):");
            Console.WriteLine("  Can read QR codes:          NO");
            Console.WriteLine("  Can read DataMatrix:        NO");
            Console.WriteLine("  Can read PDF417:            NO");
            Console.WriteLine("  Can read Aztec:             NO");
            Console.WriteLine();
            Console.WriteLine("This creates a problematic scenario:");
        }

        /// <summary>
        /// Real-world scenario showing the problem
        /// </summary>
        public void ProductLabelScenario()
        {
            Console.WriteLine("=== PRODUCT LABEL SCENARIO ===");
            Console.WriteLine();
            Console.WriteLine("Your business process:");
            Console.WriteLine("  1. Generate QR code for product label (Neodynamic can do this)");
            Console.WriteLine("  2. Print label and attach to product");
            Console.WriteLine("  3. Ship product to customer");
            Console.WriteLine("  4. Customer returns product for warranty");
            Console.WriteLine("  5. Scan QR code to look up order (Neodynamic CANNOT do this)");
            Console.WriteLine();
            Console.WriteLine("You need a DIFFERENT library just to complete the workflow!");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  A) Add ZXing.Net for QR reading (free, but manual config)");
            Console.WriteLine("  B) Add IronBarcode for QR reading (unified, auto-detect)");
            Console.WriteLine("  C) Replace Neodynamic entirely with IronBarcode");
        }

        /// <summary>
        /// Event ticket scenario
        /// </summary>
        public void EventTicketScenario()
        {
            Console.WriteLine("=== EVENT TICKET SCENARIO ===");
            Console.WriteLine();
            Console.WriteLine("Your ticketing system:");
            Console.WriteLine("  1. Generate QR code ticket (Neodynamic: OK)");
            Console.WriteLine("  2. Email ticket to customer");
            Console.WriteLine("  3. Customer presents ticket at venue");
            Console.WriteLine("  4. Scan QR code to validate (Neodynamic: FAILS)");
            Console.WriteLine();
            Console.WriteLine("The core functionality - ticket validation - doesn't work!");
            Console.WriteLine("You cannot build a complete ticketing system with Neodynamic.");
        }
    }

    /// <summary>
    /// Workaround options when Neodynamic cannot read your barcodes
    /// </summary>
    public class NeodynamicWorkarounds
    {
        public void ListWorkaroundOptions()
        {
            Console.WriteLine("=== WORKAROUND OPTIONS FOR 2D READING ===");
            Console.WriteLine();

            Console.WriteLine("Option 1: Add IronBarcode for reading only");
            Console.WriteLine("  Keep: Neodynamic for generation");
            Console.WriteLine("  Add:  IronBarcode Lite ($749) for reading");
            Console.WriteLine("  Total: ~$245 + $749 = ~$994");
            Console.WriteLine("  Result: Works, but two libraries to maintain");
            Console.WriteLine();

            Console.WriteLine("Option 2: Add ZXing.Net for reading");
            Console.WriteLine("  Keep: Neodynamic for generation");
            Console.WriteLine("  Add:  ZXing.Net (free, open source)");
            Console.WriteLine("  Total: ~$245 + $0 = ~$245");
            Console.WriteLine("  Caveats:");
            Console.WriteLine("    - No automatic format detection");
            Console.WriteLine("    - No ML error correction");
            Console.WriteLine("    - No PDF support");
            Console.WriteLine("    - More configuration required");
            Console.WriteLine();

            Console.WriteLine("Option 3: Replace Neodynamic entirely");
            Console.WriteLine("  Remove: Neodynamic SDKs");
            Console.WriteLine("  Add:    IronBarcode ($749)");
            Console.WriteLine("  Total: $749");
            Console.WriteLine("  Result:");
            Console.WriteLine("    - Single unified library");
            Console.WriteLine("    - Generation AND reading");
            Console.WriteLine("    - All 50+ formats including 2D");
            Console.WriteLine("    - Automatic format detection");
            Console.WriteLine("    - Native PDF support");
            Console.WriteLine();

            Console.WriteLine("RECOMMENDATION:");
            Console.WriteLine("  If you need 2D reading, Option 3 is usually best.");
            Console.WriteLine("  Why pay for Neodynamic generation + another reader");
            Console.WriteLine("  when IronBarcode does both for less total cost?");
        }
    }
}


// ============================================================================
// PART 2: IRONBARCODE - READS ALL FORMATS
// ============================================================================

namespace IronBarcodeAllFormats
{
    // Install: dotnet add package IronBarcode
    using IronBarCode;

    /// <summary>
    /// IronBarcode reads ALL barcode formats - 1D and 2D
    /// </summary>
    public class IronBarcodeUniversalReading
    {
        /// <summary>
        /// Read any barcode format - auto-detection handles everything
        /// </summary>
        public string ReadAnyBarcode(string imagePath)
        {
            // Automatically detects format - works for:
            // - All 1D barcodes (Code 128, EAN, UPC, etc.)
            // - All 2D barcodes (QR, DataMatrix, PDF417, etc.)
            var results = BarcodeReader.Read(imagePath);
            return results.FirstOrDefault()?.Text;
        }

        /// <summary>
        /// Read QR codes - works without any special configuration
        /// </summary>
        public string ReadQrCode(string imagePath)
        {
            Console.WriteLine("Reading QR code with IronBarcode...");

            var results = BarcodeReader.Read(imagePath);
            var qrResult = results.FirstOrDefault();

            if (qrResult != null)
            {
                Console.WriteLine($"SUCCESS: {qrResult.Text}");
                Console.WriteLine($"Format detected: {qrResult.BarcodeType}");
                return qrResult.Text;
            }

            Console.WriteLine("No barcode found");
            return null;
        }

        /// <summary>
        /// Read DataMatrix - works without any special configuration
        /// </summary>
        public string ReadDataMatrix(string imagePath)
        {
            Console.WriteLine("Reading DataMatrix with IronBarcode...");

            var results = BarcodeReader.Read(imagePath);
            var dmResult = results.FirstOrDefault();

            if (dmResult != null)
            {
                Console.WriteLine($"SUCCESS: {dmResult.Text}");
                Console.WriteLine($"Format detected: {dmResult.BarcodeType}");
                return dmResult.Text;
            }

            Console.WriteLine("No barcode found");
            return null;
        }

        /// <summary>
        /// Read PDF417 - works without any special configuration
        /// </summary>
        public string ReadPdf417(string imagePath)
        {
            Console.WriteLine("Reading PDF417 with IronBarcode...");

            var results = BarcodeReader.Read(imagePath);
            var pdfResult = results.FirstOrDefault();

            if (pdfResult != null)
            {
                Console.WriteLine($"SUCCESS: {pdfResult.Text}");
                Console.WriteLine($"Format detected: {pdfResult.BarcodeType}");
                return pdfResult.Text;
            }

            Console.WriteLine("No barcode found");
            return null;
        }

        /// <summary>
        /// List all supported formats
        /// </summary>
        public void ListAllSupportedFormats()
        {
            Console.WriteLine("=== IRONBARCODE: ALL 50+ SUPPORTED FORMATS ===");
            Console.WriteLine();
            Console.WriteLine("1D LINEAR BARCODES:");
            Console.WriteLine("  Codabar, Code 11, Code 128, Code 39, Code 39 Extended");
            Console.WriteLine("  Code 93, Code 93 Extended, EAN-8, EAN-13, EAN-14");
            Console.WriteLine("  Industrial 2 of 5, Interleaved 2 of 5, ISBN, ISSN");
            Console.WriteLine("  ITF-14, MSI/Plessey, Pharmacode, UPC-A, UPC-E");
            Console.WriteLine("  Plus many more...");
            Console.WriteLine();
            Console.WriteLine("2D BARCODES (All supported - unlike Neodynamic):");
            Console.WriteLine("  QR Code, Micro QR, QR Code Model 1");
            Console.WriteLine("  DataMatrix, DataMatrix GS1");
            Console.WriteLine("  PDF417, Micro PDF417, PDF417 Compact");
            Console.WriteLine("  Aztec Code");
            Console.WriteLine("  MaxiCode");
            Console.WriteLine("  Plus more...");
            Console.WriteLine();
            Console.WriteLine("Every format works with the same simple API:");
            Console.WriteLine("  var results = BarcodeReader.Read(imagePath);");
        }
    }

    /// <summary>
    /// Direct comparison: Neodynamic vs IronBarcode reading capabilities
    /// </summary>
    public class ReadingCapabilityComparison
    {
        public void CompareCapabilities()
        {
            Console.WriteLine("=== READING CAPABILITY COMPARISON ===");
            Console.WriteLine();
            Console.WriteLine("Format         | Neodynamic | IronBarcode");
            Console.WriteLine("---------------|------------|------------");
            Console.WriteLine("Code 128       | Yes        | Yes");
            Console.WriteLine("EAN-13         | Yes        | Yes");
            Console.WriteLine("UPC-A          | Yes        | Yes");
            Console.WriteLine("Code 39        | Yes        | Yes");
            Console.WriteLine("QR Code        | NO         | Yes");
            Console.WriteLine("DataMatrix     | NO         | Yes");
            Console.WriteLine("PDF417         | NO         | Yes");
            Console.WriteLine("Aztec          | NO         | Yes");
            Console.WriteLine("MaxiCode       | NO         | Yes");
            Console.WriteLine();
            Console.WriteLine("Format detection | Manual  | Automatic");
            Console.WriteLine("PDF support      | No      | Yes");
            Console.WriteLine("ML correction    | No      | Yes");
        }

        public void RecommendationSummary()
        {
            Console.WriteLine("=== RECOMMENDATION SUMMARY ===");
            Console.WriteLine();
            Console.WriteLine("Choose Neodynamic Reader IF:");
            Console.WriteLine("  - You ONLY need to read 1D barcodes");
            Console.WriteLine("  - You will NEVER need QR codes");
            Console.WriteLine("  - You will NEVER need DataMatrix");
            Console.WriteLine("  - You will NEVER need PDF417");
            Console.WriteLine("  - You're certain requirements won't change");
            Console.WriteLine();
            Console.WriteLine("Choose IronBarcode IF:");
            Console.WriteLine("  - You need to read ANY 2D barcodes");
            Console.WriteLine("  - You might need 2D support in the future");
            Console.WriteLine("  - You want a single library for everything");
            Console.WriteLine("  - You want automatic format detection");
            Console.WriteLine("  - You need PDF document scanning");
            Console.WriteLine();
            Console.WriteLine("In 2026, most applications need 2D support.");
            Console.WriteLine("QR codes alone are ubiquitous.");
            Console.WriteLine("Neodynamic Reader is increasingly limited.");
        }
    }
}
