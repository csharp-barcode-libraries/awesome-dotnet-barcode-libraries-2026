// Install: dotnet add package IronBarcode
// IronBarcode Tutorial: Generating Basic Barcodes
// Demonstrates Code128, Code39, EAN-13, UPC-A generation
// and saving in multiple image formats

using System;
using IronBarCode;

namespace BarcodeGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== IronBarcode - Basic Barcode Generation Tutorial ===\n");

            // Example 1: Code128 barcode generation
            Console.WriteLine("Example 1: Code128 (general purpose)");
            GenerateCode128();

            // Example 2: Code39 barcode generation
            Console.WriteLine("\nExample 2: Code39 (alphanumeric)");
            GenerateCode39();

            // Example 3: EAN-13 retail barcodes
            Console.WriteLine("\nExample 3: EAN-13 (international retail)");
            GenerateEAN13();

            // Example 4: UPC-A retail barcodes
            Console.WriteLine("\nExample 4: UPC-A (North American retail)");
            GenerateUPCA();

            // Example 5: Save in multiple formats
            Console.WriteLine("\nExample 5: Multiple image formats");
            SaveMultipleFormats();

            Console.WriteLine("\n=== All examples completed ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void GenerateCode128()
        {
            // Code128 supports full ASCII character set
            var barcode = BarcodeWriter.CreateBarcode("ITEM-12345", BarcodeEncoding.Code128);
            barcode.SaveAsPng("code128-example.png");
            Console.WriteLine("  Generated: code128-example.png");
            Console.WriteLine("  Data: ITEM-12345");
            Console.WriteLine("  Format: Code128 (supports alphanumeric)");
        }

        static void GenerateCode39()
        {
            // Code39 supports uppercase letters, numbers, and limited special characters
            var barcode1 = BarcodeWriter.CreateBarcode("ORDER-ABC-789", BarcodeEncoding.Code39);
            barcode1.SaveAsPng("code39-example.png");
            Console.WriteLine("  Generated: code39-example.png");
            Console.WriteLine("  Data: ORDER-ABC-789");

            // Code39 automatically converts lowercase to uppercase
            var barcode2 = BarcodeWriter.CreateBarcode("item-xyz", BarcodeEncoding.Code39);
            barcode2.SaveAsPng("code39-uppercase.png");
            Console.WriteLine("  Generated: code39-uppercase.png");
            Console.WriteLine("  Data: item-xyz (converted to ITEM-XYZ)");
        }

        static void GenerateEAN13()
        {
            // EAN-13 requires 12 digits (13th checksum digit calculated automatically)
            var barcode = BarcodeWriter.CreateBarcode("501234567890", BarcodeEncoding.EAN13);
            barcode.SaveAsPng("ean13-example.png");
            Console.WriteLine("  Generated: ean13-example.png");
            Console.WriteLine("  Input: 501234567890 (12 digits)");
            Console.WriteLine("  Output: 13 digits with automatic checksum");
            Console.WriteLine("  Use: International retail products");
        }

        static void GenerateUPCA()
        {
            // UPC-A requires 11 digits (12th checksum digit calculated automatically)
            var barcode = BarcodeWriter.CreateBarcode("01234567890", BarcodeEncoding.UPCA);
            barcode.SaveAsPng("upca-example.png");
            Console.WriteLine("  Generated: upca-example.png");
            Console.WriteLine("  Input: 01234567890 (11 digits)");
            Console.WriteLine("  Output: 12 digits with automatic checksum");
            Console.WriteLine("  Use: North American retail products");
        }

        static void SaveMultipleFormats()
        {
            var barcode = BarcodeWriter.CreateBarcode("MULTI-FORMAT-DEMO", BarcodeEncoding.Code128);

            // Save in different image formats
            barcode.SaveAsPng("multi-format.png");
            Console.WriteLine("  Saved: multi-format.png (recommended for web)");

            barcode.SaveAsJpeg("multi-format.jpg");
            Console.WriteLine("  Saved: multi-format.jpg (smaller size, lossy)");

            barcode.SaveAsGif("multi-format.gif");
            Console.WriteLine("  Saved: multi-format.gif (good for simple graphics)");

            barcode.SaveAsTiff("multi-format.tiff");
            Console.WriteLine("  Saved: multi-format.tiff (best for print/archival)");
        }

        // Bonus: Generate multiple barcodes in batch
        static void BatchGenerate()
        {
            var productCodes = new[] { "PROD-001", "PROD-002", "PROD-003", "PROD-004", "PROD-005" };

            foreach (var code in productCodes)
            {
                var barcode = BarcodeWriter.CreateBarcode(code, BarcodeEncoding.Code128);
                barcode.SaveAsPng($"{code}.png");
                Console.WriteLine($"  Generated: {code}.png");
            }
        }
    }
}
