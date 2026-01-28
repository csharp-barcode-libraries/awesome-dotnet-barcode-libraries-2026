// Install-Package IronBarcode
using System;
using System.Linq;
using IronBarCode;

namespace DamagedBarcodeReading
{
    /// <summary>
    /// Demonstrates IronBarcode's ML-powered error correction for damaged barcodes
    /// Scenarios: rotation, perspective distortion, poor lighting, physical damage
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== IronBarcode ML Error Correction Demo ===");
            Console.WriteLine("Demonstrates reading damaged, rotated, and low-quality barcodes\n");

            // Scenario 1: Confidence threshold adjustment
            DemonstrateConfidenceThreshold();

            // Scenario 2: Rotated barcodes
            DemonstrateRotationHandling();

            // Scenario 3: Perspective distortion
            DemonstratePerspectiveCorrection();

            // Scenario 4: Poor lighting
            DemonstrateLightingCorrection();

            // Scenario 5: Physical damage
            DemonstrateDamageRecovery();

            // Scenario 6: Combined challenges
            DemonstrateCombinedChallenges();
        }

        /// <summary>
        /// Adjust confidence threshold for damaged barcodes
        /// Lower threshold accepts more ambiguous reads
        /// </summary>
        static void DemonstrateConfidenceThreshold()
        {
            Console.WriteLine("--- Confidence Threshold Configuration ---");

            // Lenient settings for damaged barcodes
            var lenientOptions = new BarcodeReaderOptions
            {
                ConfidenceThreshold = 0.5,  // 50% confidence (lower than default 0.7)
                ReadingSpeed = ReadingSpeed.Detailed
            };

            // Strict settings for critical applications
            var strictOptions = new BarcodeReaderOptions
            {
                ConfidenceThreshold = 0.9,  // 90% confidence
                ReadingSpeed = ReadingSpeed.Detailed
            };

            var result = BarcodeReader.Read("damaged-barcode.png", lenientOptions);

            if (result.Any())
            {
                var barcode = result.First();
                Console.WriteLine($"Decoded: {barcode.Text}");
                Console.WriteLine($"Confidence: {barcode.ReadingConfidence:P0}");
                Console.WriteLine($"Format: {barcode.BarcodeType}\n");
            }
            else
            {
                Console.WriteLine("Could not decode even with low confidence threshold\n");
            }
        }

        /// <summary>
        /// Automatic rotation detection and correction
        /// Works for any angle - no manual preprocessing needed
        /// </summary>
        static void DemonstrateRotationHandling()
        {
            Console.WriteLine("--- Automatic Rotation Handling ---");

            var options = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Detailed
            };

            // IronBarcode automatically detects and corrects rotation
            var result = BarcodeReader.Read("rotated-barcode.png", options);

            if (result.Any())
            {
                var barcode = result.First();
                Console.WriteLine($"Decoded rotated barcode: {barcode.Text}");
                Console.WriteLine($"Detected orientation: {barcode.Orientation}°");
                Console.WriteLine("Note: Works for any rotation angle (15°, 30°, 45°, 90°, etc.)\n");
            }
            else
            {
                Console.WriteLine("Rotation too severe or barcode unreadable\n");
            }
        }

        /// <summary>
        /// Perspective correction for camera angles and skew
        /// Common in mobile scanning scenarios
        /// </summary>
        static void DemonstratePerspectiveCorrection()
        {
            Console.WriteLine("--- Perspective Distortion Correction ---");

            var options = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Detailed,
                ExpectBarcodeTypes = BarcodeEncoding.All
            };

            // Read barcode from mobile photo taken at angle
            var result = BarcodeReader.Read("skewed-mobile-photo.jpg", options);

            if (result.Any())
            {
                Console.WriteLine($"Decoded skewed barcode: {result.First().Text}");
                Console.WriteLine("ML model corrected perspective distortion automatically\n");
            }
            else
            {
                Console.WriteLine("Perspective distortion too severe for correction\n");
            }
        }

        /// <summary>
        /// Handle various lighting conditions
        /// Underexposed, overexposed, and uneven lighting
        /// </summary>
        static void DemonstrateLightingCorrection()
        {
            Console.WriteLine("--- Lighting Condition Handling ---");

            var options = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Detailed,
                ConfidenceThreshold = 0.6
            };

            // Example 1: Dark/underexposed image
            var darkResult = BarcodeReader.Read("dark-warehouse.jpg", options);
            Console.WriteLine($"Dark image result: {darkResult.FirstOrDefault()?.Text ?? "Not detected"}");

            // Example 2: Bright/overexposed image
            var brightResult = BarcodeReader.Read("overexposed-flash.jpg", options);
            Console.WriteLine($"Bright image result: {brightResult.FirstOrDefault()?.Text ?? "Not detected"}");

            // Example 3: Uneven lighting with shadows
            var shadowResult = BarcodeReader.Read("partial-shadow.jpg", options);
            Console.WriteLine($"Shadow image result: {shadowResult.FirstOrDefault()?.Text ?? "Not detected"}");

            Console.WriteLine("ReadingSpeed.Detailed applies adaptive contrast enhancement\n");
        }

        /// <summary>
        /// Physical damage scenarios: torn labels, faded printing, obstructions
        /// QR codes can recover from up to 30% damage with Level H error correction
        /// </summary>
        static void DemonstrateDamageRecovery()
        {
            Console.WriteLine("--- Physical Damage Recovery ---");

            var options = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Detailed,
                ConfidenceThreshold = 0.5,  // Lower threshold for damaged content
                ExpectBarcodeTypes = BarcodeEncoding.QRCode
            };

            // Attempt to read damaged QR code
            var result = BarcodeReader.Read("torn-qr-code.png", options);

            if (result.Any())
            {
                var barcode = result.First();
                Console.WriteLine($"Recovered damaged barcode: {barcode.Text}");
                Console.WriteLine($"Confidence: {barcode.ReadingConfidence:P0}");
                Console.WriteLine($"Format: {barcode.BarcodeType}");
                Console.WriteLine("\nDamage tolerance by format:");
                Console.WriteLine("  QR Code (Level H): Up to 30% damage");
                Console.WriteLine("  Data Matrix: Up to 25% damage");
                Console.WriteLine("  Code128: Minimal (<5% damage)\n");
            }
            else
            {
                Console.WriteLine("Damage too extensive for recovery\n");
            }
        }

        /// <summary>
        /// Combined challenges: rotation + lighting + damage
        /// Demonstrates full ML pipeline capability
        /// </summary>
        static void DemonstrateCombinedChallenges()
        {
            Console.WriteLine("--- Combined Challenges ---");

            // Maximum ML error correction settings
            var options = new BarcodeReaderOptions
            {
                ReadingSpeed = ReadingSpeed.Detailed,
                ConfidenceThreshold = 0.5,
                ExpectBarcodeTypes = BarcodeEncoding.All,
                ExpectMultipleBarcodes = false
            };

            // Real-world scenario: rotated, poorly lit, partially damaged barcode
            var result = BarcodeReader.Read("worst-case-barcode.jpg", options);

            if (result.Any())
            {
                var barcode = result.First();
                Console.WriteLine("Successfully decoded challenging barcode!");
                Console.WriteLine($"Value: {barcode.Text}");
                Console.WriteLine($"Confidence: {barcode.ReadingConfidence:P0}");
                Console.WriteLine($"Rotation: {barcode.Orientation}°");
                Console.WriteLine("\nML model handled:");
                Console.WriteLine("  ✓ Rotation correction");
                Console.WriteLine("  ✓ Lighting normalization");
                Console.WriteLine("  ✓ Damage reconstruction\n");
            }
            else
            {
                Console.WriteLine("Barcode quality too poor for reliable decoding\n");
            }
        }
    }
}
