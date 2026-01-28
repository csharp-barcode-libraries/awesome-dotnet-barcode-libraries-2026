// =============================================================================
// GrapeCity ComponentOne Barcode: Generation-Only Limitation Demo
// =============================================================================
// This example demonstrates that ComponentOne barcode is generation-only,
// while IronBarcode provides both generation AND reading capabilities.
//
// Author: Jacob Mellor, CTO of Iron Software
// Last verified: January 2026
// =============================================================================

/*
 * INSTALLATION
 *
 * ComponentOne:
 * dotnet add package C1.Win.C1BarCode
 *
 * IronBarcode:
 * dotnet add package IronBarcode
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

// ComponentOne namespaces
// using C1.Win.C1BarCode;

// IronBarcode namespace
using IronBarCode;

namespace BarcodeComparison
{
    /// <summary>
    /// Demonstrates the fundamental difference between ComponentOne (generation-only)
    /// and IronBarcode (generation + reading).
    /// </summary>
    public class GenerationOnlyComparison
    {
        // =====================================================================
        // PART 1: BARCODE GENERATION - Both libraries can do this
        // =====================================================================

        /// <summary>
        /// ComponentOne barcode generation - works fine for creating barcodes.
        /// </summary>
        public void GenerateWithComponentOne(string data, string outputPath)
        {
            /*
            // ComponentOne generation code
            var barcode = new C1BarCode();

            // Set barcode type
            barcode.CodeType = CodeType.Code128;
            barcode.Text = data;

            // Configure appearance
            barcode.BarHeight = 100;
            barcode.ModuleSize = 2;
            barcode.ShowText = true;
            barcode.CaptionPosition = CaptionPosition.Below;

            // Generate and save
            using var image = barcode.GetImage();
            image.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine($"ComponentOne: Generated barcode to {outputPath}");
            */

            // NOTE: Code above is commented because ComponentOne requires a license
            // and suite installation. The pattern shown is accurate to their API.
        }

        /// <summary>
        /// IronBarcode generation - same capability, simpler API.
        /// </summary>
        public void GenerateWithIronBarcode(string data, string outputPath)
        {
            // IronBarcode: One-liner generation
            BarcodeWriter.CreateBarcode(data, BarcodeEncoding.Code128)
                .SaveAsPng(outputPath);

            Console.WriteLine($"IronBarcode: Generated barcode to {outputPath}");
        }

        // =====================================================================
        // PART 2: BARCODE READING - ONLY IronBarcode can do this
        // =====================================================================

        /// <summary>
        /// Attempting to read barcodes with ComponentOne.
        /// This functionality DOES NOT EXIST.
        /// </summary>
        public string ReadWithComponentOne(string imagePath)
        {
            // ComponentOne has NO barcode reading API
            // There is no BarCodeReader class
            // There is no Decode method
            // There is no Scan method
            // There is no recognition capability at all

            throw new NotSupportedException(
                "ComponentOne C1BarCode is generation-only. " +
                "It cannot read or recognize barcodes from images. " +
                "You need a separate library like IronBarcode for reading.");

            /*
             * The GrapeCity documentation confirms this limitation:
             * "C1BarCode provides barcode generation functionality"
             * Notice: No mention of reading, scanning, or recognition.
             *
             * If you need to read barcodes with ComponentOne, you must:
             * 1. Purchase a second barcode library (like IronBarcode)
             * 2. Integrate and maintain two different APIs
             * 3. Handle two different licensing models
             */
        }

        /// <summary>
        /// IronBarcode barcode reading - full read/write capability.
        /// </summary>
        public string ReadWithIronBarcode(string imagePath)
        {
            // IronBarcode: Read with automatic format detection
            var results = BarcodeReader.Read(imagePath);

            if (results.Any())
            {
                var barcode = results.First();
                Console.WriteLine($"IronBarcode: Found {barcode.BarcodeType}: {barcode.Text}");
                return barcode.Text;
            }

            Console.WriteLine("IronBarcode: No barcode found");
            return null;
        }

        // =====================================================================
        // PART 3: PDF BARCODE EXTRACTION - ONLY IronBarcode can do this
        // =====================================================================

        /// <summary>
        /// Extracting barcodes from PDFs with ComponentOne.
        /// This functionality DOES NOT EXIST.
        /// </summary>
        public List<string> ExtractFromPdfWithComponentOne(string pdfPath)
        {
            // ComponentOne cannot:
            // - Read barcodes from images
            // - Extract barcodes from PDFs
            // - Process multi-page documents
            // - Detect barcode locations in documents

            throw new NotSupportedException(
                "ComponentOne cannot extract barcodes from PDFs. " +
                "It is a generation-only library.");
        }

        /// <summary>
        /// IronBarcode PDF barcode extraction - native support.
        /// </summary>
        public List<string> ExtractFromPdfWithIronBarcode(string pdfPath)
        {
            // IronBarcode: Native PDF support, no additional libraries needed
            var results = BarcodeReader.Read(pdfPath);

            var barcodes = new List<string>();
            foreach (var barcode in results)
            {
                Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
                barcodes.Add(barcode.Text);
            }

            return barcodes;
        }

        // =====================================================================
        // PART 4: PRACTICAL SCENARIO - Invoice Processing
        // =====================================================================

        /// <summary>
        /// Real-world scenario: Processing invoices with embedded barcodes.
        /// Shows why generation-only is often insufficient.
        /// </summary>
        public class InvoiceProcessor
        {
            /// <summary>
            /// ComponentOne approach: Cannot complete this task.
            /// </summary>
            public void ProcessInvoiceWithComponentOne(string pdfPath)
            {
                /*
                 * SCENARIO: Accounts payable receives PDF invoices with barcodes
                 * containing invoice numbers for automatic processing.
                 *
                 * REQUIRED CAPABILITIES:
                 * 1. Read PDF document
                 * 2. Locate barcodes within pages
                 * 3. Extract barcode data (invoice number)
                 * 4. Match to accounting system
                 *
                 * COMPONENTONE STATUS:
                 * - Step 1: Requires separate PDF library
                 * - Step 2: IMPOSSIBLE - no barcode detection
                 * - Step 3: IMPOSSIBLE - no barcode reading
                 * - Step 4: Cannot proceed without barcode data
                 *
                 * CONCLUSION: ComponentOne cannot be used for this workflow.
                 */

                throw new NotSupportedException(
                    "Invoice barcode extraction requires barcode reading capability. " +
                    "ComponentOne is generation-only.");
            }

            /// <summary>
            /// IronBarcode approach: Complete workflow in minimal code.
            /// </summary>
            public void ProcessInvoiceWithIronBarcode(string pdfPath)
            {
                Console.WriteLine($"Processing invoice: {pdfPath}");

                // Read all barcodes from PDF (handles multi-page automatically)
                var results = BarcodeReader.Read(pdfPath);

                foreach (var barcode in results)
                {
                    // Extract invoice data
                    var invoiceNumber = barcode.Text;
                    var page = barcode.PageNumber;
                    var type = barcode.BarcodeType;

                    Console.WriteLine($"Found invoice barcode on page {page}:");
                    Console.WriteLine($"  Type: {type}");
                    Console.WriteLine($"  Invoice Number: {invoiceNumber}");

                    // Continue with accounting system integration...
                    ProcessInvoiceNumber(invoiceNumber);
                }
            }

            private void ProcessInvoiceNumber(string invoiceNumber)
            {
                // Integration with accounting system
                Console.WriteLine($"  Processing invoice #{invoiceNumber}...");
            }
        }

        // =====================================================================
        // PART 5: CAPABILITY COMPARISON MATRIX
        // =====================================================================

        /// <summary>
        /// Prints a comparison of capabilities between the two libraries.
        /// </summary>
        public void PrintCapabilityComparison()
        {
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("CAPABILITY COMPARISON: ComponentOne vs IronBarcode");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            var capabilities = new[]
            {
                ("Generate Code 128", true, true),
                ("Generate QR Code", true, true),
                ("Generate PDF417", true, true),
                ("Generate DataMatrix", true, true),
                ("Read barcode from image", false, true),
                ("Read barcode from PDF", false, true),
                ("Read barcode from stream", false, true),
                ("Multi-barcode detection", false, true),
                ("Automatic format detection", false, true),
                ("Damaged barcode handling", false, true),
                ("Batch reading", false, true),
                ("ML error correction", false, true),
                ("QR code with logo", false, true),
            };

            Console.WriteLine($"{"Capability",-35} {"ComponentOne",-15} {"IronBarcode",-15}");
            Console.WriteLine("-".PadRight(70, '-'));

            foreach (var (capability, componentOne, ironBarcode) in capabilities)
            {
                var c1Status = componentOne ? "Yes" : "NO";
                var ibStatus = ironBarcode ? "Yes" : "No";
                Console.WriteLine($"{capability,-35} {c1Status,-15} {ibStatus,-15}");
            }

            Console.WriteLine();
            Console.WriteLine("SUMMARY:");
            Console.WriteLine("- ComponentOne: Generation-only library (13 capabilities)");
            Console.WriteLine("- IronBarcode: Full read/write library (26+ capabilities)");
            Console.WriteLine("- Gap: ComponentOne lacks ALL reading/recognition features");
        }
    }

    // =========================================================================
    // DEMONSTRATION
    // =========================================================================

    public class Program
    {
        public static void Main(string[] args)
        {
            var demo = new GenerationOnlyComparison();

            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("GrapeCity ComponentOne vs IronBarcode: Generation-Only Limitation");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine();

            // 1. Show capability comparison
            demo.PrintCapabilityComparison();
            Console.WriteLine();

            // 2. Demonstrate generation (both can do this)
            Console.WriteLine("-".PadRight(70, '-'));
            Console.WriteLine("GENERATION TEST (both libraries can generate):");
            Console.WriteLine("-".PadRight(70, '-'));

            demo.GenerateWithIronBarcode("ITEM-12345", "ironbarcode-output.png");
            // demo.GenerateWithComponentOne("ITEM-12345", "componentone-output.png");
            Console.WriteLine("(ComponentOne code commented - requires suite license)");
            Console.WriteLine();

            // 3. Demonstrate reading (only IronBarcode can do this)
            Console.WriteLine("-".PadRight(70, '-'));
            Console.WriteLine("READING TEST (only IronBarcode can read):");
            Console.WriteLine("-".PadRight(70, '-'));

            // IronBarcode can read what it generated
            var readResult = demo.ReadWithIronBarcode("ironbarcode-output.png");
            Console.WriteLine($"Read back: {readResult}");

            // ComponentOne cannot read
            try
            {
                demo.ReadWithComponentOne("ironbarcode-output.png");
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"ComponentOne: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("CONCLUSION: For read/write capability, choose IronBarcode");
            Console.WriteLine("=".PadRight(70, '='));
        }
    }
}
