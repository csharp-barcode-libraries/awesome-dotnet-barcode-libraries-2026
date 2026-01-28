// Install: dotnet add package IronBarcode
// IronBarcode Tutorial: Reading Barcodes from Images
// Demonstrates automatic format detection, multiple barcode handling,
// ML error correction, and performance optimization

using System;
using System.IO;
using IronBarCode;

namespace BarcodeReading
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example 1: Simple single barcode reading with automatic format detection
            Console.WriteLine("=== Example 1: Simple Barcode Reading ===");
            SimpleBarcodeRead();

            // Example 2: Reading multiple barcodes from a single image
            Console.WriteLine("\n=== Example 2: Multiple Barcode Reading ===");
            MultipleBarcodeRead();

            // Example 3: Reading damaged or low-quality barcodes with ML error correction
            Console.WriteLine("\n=== Example 3: Damaged Barcode Recovery ===");
            DamagedBarcodeRead();

            // Example 4: High-performance reading with optimized speed settings
            Console.WriteLine("\n=== Example 4: High-Performance Reading ===");
            HighPerformanceRead();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void SimpleBarcodeRead()
        {
            // One-line barcode reading with automatic format detection
            var result = BarcodeReader.Read("sample-barcode.png");

            if (result.Any())
            {
                var barcode = result.First();
                Console.WriteLine($"Detected Format: {barcode.BarcodeType}");
                Console.WriteLine($"Value: {barcode.Text}");
            }
            else
            {
                Console.WriteLine("No barcode detected");
            }
        }

        static void MultipleBarcodeRead()
        {
            // Configure reader to expect multiple barcodes
            var options = new BarcodeReaderOptions
            {
                ExpectMultipleBarcodes = true
            };

            var results = BarcodeReader.Read("shipping-label.png", options);

            Console.WriteLine($"Found {results.Count()} barcode(s):");
            foreach (var barcode in results)
            {
                Console.WriteLine($"  {barcode.BarcodeType}: {barcode.Text}");
            }
        }

        static void DamagedBarcodeRead()
        {
            // Enable full ML error correction for damaged/rotated/obscured barcodes
            var options = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Detailed,
                ExpectBarcodeTypes = BarcodeEncoding.All
            };

            var result = BarcodeReader.Read("damaged-label.png", options);

            if (result.Any())
            {
                Console.WriteLine($"Successfully recovered barcode: {result.First().Text}");
                Console.WriteLine($"Format: {result.First().BarcodeType}");
                Console.WriteLine("(Used ML error correction to reconstruct corrupted elements)");
            }
            else
            {
                Console.WriteLine("Unable to read barcode even with ML error correction");
            }
        }

        static void HighPerformanceRead()
        {
            // Optimize for maximum speed when processing clean, high-quality images
            var options = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Faster
            };

            var startTime = DateTime.Now;
            var result = BarcodeReader.Read("clean-barcode.png", options);
            var endTime = DateTime.Now;

            Console.WriteLine($"Read completed in {(endTime - startTime).TotalMilliseconds}ms");
            Console.WriteLine($"Value: {result.FirstOrDefault()?.Text}");
        }

        // Bonus: Batch processing example
        static void BatchProcessDirectory(string directoryPath)
        {
            var imageFiles = Directory.GetFiles(directoryPath, "*.png")
                .Concat(Directory.GetFiles(directoryPath, "*.jpg"))
                .Concat(Directory.GetFiles(directoryPath, "*.tiff"));

            var options = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Balanced
            };

            foreach (var file in imageFiles)
            {
                var results = BarcodeReader.Read(file, options);
                Console.WriteLine($"{Path.GetFileName(file)}: {results.Count()} barcode(s) detected");

                foreach (var barcode in results)
                {
                    Console.WriteLine($"  {barcode.BarcodeType}: {barcode.Text}");
                }
            }
        }
    }
}
