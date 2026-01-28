/*
 * Scanbot SDK vs IronBarcode: Camera Focus Analysis
 *
 * This file demonstrates the fundamental difference between
 * Scanbot's camera-centric design and IronBarcode's file-centric
 * processing approach.
 *
 * Key Insight: Scanbot is built for real-time camera scanning.
 * IronBarcode is built for file and document processing.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.IO;
using System.Threading.Tasks;

namespace ScanbotCameraFocusAnalysis
{
    // ============================================================
    // SCANBOT: CAMERA-CENTRIC DESIGN
    // ============================================================

    /// <summary>
    /// Scanbot SDK is designed around real-time camera scanning.
    /// The entire API assumes camera input and UI presentation.
    /// </summary>
    public class ScanbotCameraDesign
    {
        /*
         * Scanbot Camera Workflow:
         *
         * 1. User launches scanner in app
         * 2. Camera preview appears full-screen
         * 3. Viewfinder overlay shows scan region
         * 4. User points camera at barcode
         * 5. SDK processes video frames in real-time
         * 6. Barcode detected → visual feedback
         * 7. Result returned to app
         * 8. Camera closes
         *
         * This is excellent for point-of-scan scenarios:
         * - Retail self-checkout
         * - Warehouse scanning
         * - Asset tracking
         * - Ticket validation
         */

        public void DescribeCameraWorkflow()
        {
            Console.WriteLine("=== Scanbot Camera Workflow ===");
            Console.WriteLine();
            Console.WriteLine("Step 1: Configure scanner");
            Console.WriteLine("  var config = new BarcodeScannerConfiguration();");
            Console.WriteLine("  config.AcceptedFormats = new[] { BarcodeFormat.QrCode };");
            Console.WriteLine("  config.TopBarBackgroundColor = Colors.Blue;");
            Console.WriteLine();
            Console.WriteLine("Step 2: Open camera scanner");
            Console.WriteLine("  var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(config);");
            Console.WriteLine();
            Console.WriteLine("Step 3: User scans with camera (UI takes over)");
            Console.WriteLine("  - Full-screen camera preview");
            Console.WriteLine("  - Viewfinder overlay");
            Console.WriteLine("  - Real-time barcode detection");
            Console.WriteLine("  - Visual/audio feedback on detection");
            Console.WriteLine();
            Console.WriteLine("Step 4: Get results after scanning");
            Console.WriteLine("  if (result.Status == OperationResult.Ok)");
            Console.WriteLine("  {");
            Console.WriteLine("      var barcodes = result.Barcodes;");
            Console.WriteLine("  }");
        }

        // Conceptual Scanbot camera scanning code
        // (Actual implementation requires ScanbotBarcodeSDK.MAUI package in MAUI project)

        public async Task ConceptualScanbotScanning()
        {
            /*
             * // Requires MAUI project with ScanbotBarcodeSDK.MAUI package
             *
             * using ScanbotBarcodeSDK.MAUI;
             *
             * // Configure scanner appearance and behavior
             * var configuration = new BarcodeScannerConfiguration
             * {
             *     AcceptedFormats = new[]
             *     {
             *         BarcodeFormat.Code128,
             *         BarcodeFormat.QrCode,
             *         BarcodeFormat.Ean13,
             *         BarcodeFormat.DataMatrix
             *     },
             *     FinderAspectRatio = new AspectRatio(1, 1),
             *     TopBarBackgroundColor = Colors.DarkBlue,
             *     FlashEnabled = false,
             *     OrientationLockMode = OrientationLockMode.Portrait
             * };
             *
             * // This opens full-screen camera scanner
             * // User interface takes over the entire screen
             * // User points camera at barcode
             * // SDK returns when barcode detected or user cancels
             * var result = await ScanbotBarcodeSDK.BarcodeScanner.Open(configuration);
             *
             * if (result.Status == OperationResult.Ok)
             * {
             *     foreach (var barcode in result.Barcodes)
             *     {
             *         Console.WriteLine($"{barcode.Format}: {barcode.Text}");
             *     }
             * }
             */
            await Task.CompletedTask;
        }
    }

    // ============================================================
    // IRONBARCODE: FILE-CENTRIC DESIGN
    // ============================================================

    /// <summary>
    /// IronBarcode is designed around file and document processing.
    /// The API assumes file input and programmatic output.
    /// </summary>
    public class IronBarcodeFileDesign
    {
        /*
         * IronBarcode File Workflow:
         *
         * 1. Barcode exists in file (image, PDF, stream)
         * 2. Code calls BarcodeReader.Read()
         * 3. Library processes file content
         * 4. Results returned as data objects
         * 5. Code uses barcode values
         *
         * This is excellent for document processing:
         * - Server-side PDF processing
         * - Batch document scanning
         * - API endpoints for file uploads
         * - Automated document workflows
         */

        public void DescribeFileWorkflow()
        {
            Console.WriteLine("=== IronBarcode File Workflow ===");
            Console.WriteLine();
            Console.WriteLine("// Install: dotnet add package IronBarcode");
            Console.WriteLine("using IronBarCode;");
            Console.WriteLine();
            Console.WriteLine("// Single-line barcode reading from file");
            Console.WriteLine("var results = BarcodeReader.Read(\"document.pdf\");");
            Console.WriteLine();
            Console.WriteLine("// Process results programmatically");
            Console.WriteLine("foreach (var barcode in results)");
            Console.WriteLine("{");
            Console.WriteLine("    Console.WriteLine($\"{barcode.BarcodeType}: {barcode.Text}\");");
            Console.WriteLine("}");
            Console.WriteLine();
            Console.WriteLine("No camera, no UI, no user interaction required.");
        }

        // Actual IronBarcode file processing examples

        public void ProcessImageFile(string imagePath)
        {
            // Read barcodes from image file
            var results = IronBarCode.BarcodeReader.Read(imagePath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Found {barcode.BarcodeType}: {barcode.Text}");
            }
        }

        public void ProcessPdfDocument(string pdfPath)
        {
            // Read barcodes from PDF
            var results = IronBarCode.BarcodeReader.Read(pdfPath);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
            }
        }

        public void BatchProcessDirectory(string directoryPath)
        {
            // Process all PDFs in directory
            foreach (var file in Directory.GetFiles(directoryPath, "*.pdf"))
            {
                var results = IronBarCode.BarcodeReader.Read(file);
                Console.WriteLine($"{Path.GetFileName(file)}: {results.Count()} barcodes");
            }
        }

        public string[] ProcessStream(Stream inputStream)
        {
            // Process from stream (useful for uploads)
            var results = IronBarCode.BarcodeReader.Read(inputStream);
            return results.Select(b => b.Text).ToArray();
        }
    }

    // ============================================================
    // THE FUNDAMENTAL DIFFERENCE
    // ============================================================

    /// <summary>
    /// Comparison showing why these tools solve different problems.
    /// </summary>
    public class FundamentalDifference
    {
        public void ExplainDifference()
        {
            Console.WriteLine("=== The Fundamental Difference ===");
            Console.WriteLine();
            Console.WriteLine("SCANBOT processes: Camera video frames in real-time");
            Console.WriteLine("IRONBARCODE processes: Files (images, PDFs, streams)");
            Console.WriteLine();
            Console.WriteLine("┌────────────────────────────────────────────────┐");
            Console.WriteLine("│                    SCANBOT                     │");
            Console.WriteLine("│                                                │");
            Console.WriteLine("│   Camera → Video Frames → Detection → Result  │");
            Console.WriteLine("│     ↓          ↓            ↓          ↓     │");
            Console.WriteLine("│   User      Real-time    Viewfinder  Callback │");
            Console.WriteLine("│   Points    Processing   Feedback             │");
            Console.WriteLine("└────────────────────────────────────────────────┘");
            Console.WriteLine();
            Console.WriteLine("┌────────────────────────────────────────────────┐");
            Console.WriteLine("│                  IRONBARCODE                   │");
            Console.WriteLine("│                                                │");
            Console.WriteLine("│   File → Load Content → Detection → Result    │");
            Console.WriteLine("│    ↓          ↓            ↓          ↓      │");
            Console.WriteLine("│   Path/    Image/PDF    ML-Powered   Return   │");
            Console.WriteLine("│   Stream   Processing   Analysis     Value    │");
            Console.WriteLine("└────────────────────────────────────────────────┘");
        }

        public void ShowInputComparison()
        {
            Console.WriteLine();
            Console.WriteLine("=== Input Source Comparison ===");
            Console.WriteLine();
            Console.WriteLine("SCANBOT accepts:");
            Console.WriteLine("  - Live camera video frames");
            Console.WriteLine("  - (That's it - camera-focused)");
            Console.WriteLine();
            Console.WriteLine("IRONBARCODE accepts:");
            Console.WriteLine("  - Image file paths (PNG, JPG, BMP, GIF, TIFF)");
            Console.WriteLine("  - PDF file paths");
            Console.WriteLine("  - File streams");
            Console.WriteLine("  - Memory streams");
            Console.WriteLine("  - Byte arrays");
            Console.WriteLine("  - System.Drawing.Image objects");
            Console.WriteLine("  - Multi-page PDFs");
            Console.WriteLine("  - Compressed archives");
        }
    }

    // ============================================================
    // HYBRID APPROACH: CAMERA CAPTURE + FILE PROCESSING
    // ============================================================

    /// <summary>
    /// Shows how to get camera-like behavior without Scanbot
    /// by using platform APIs for capture + IronBarcode for processing.
    /// </summary>
    public class HybridApproach
    {
        /*
         * Alternative Approach in MAUI:
         *
         * 1. Use MAUI MediaPicker to capture photo
         * 2. Save photo to file
         * 3. Process with IronBarcode
         *
         * This doesn't provide real-time scanning UI,
         * but works for "capture then process" workflows.
         */

        public async Task CaptureAndProcessWithIronBarcode()
        {
            Console.WriteLine("=== Hybrid Approach: Capture + Process ===");
            Console.WriteLine();
            Console.WriteLine("// In MAUI, use MediaPicker for camera capture");
            Console.WriteLine("var photo = await MediaPicker.CapturePhotoAsync();");
            Console.WriteLine();
            Console.WriteLine("if (photo != null)");
            Console.WriteLine("{");
            Console.WriteLine("    // Save to file");
            Console.WriteLine("    var path = Path.Combine(FileSystem.CacheDirectory, photo.FileName);");
            Console.WriteLine("    using var source = await photo.OpenReadAsync();");
            Console.WriteLine("    using var dest = File.OpenWrite(path);");
            Console.WriteLine("    await source.CopyToAsync(dest);");
            Console.WriteLine();
            Console.WriteLine("    // Process with IronBarcode");
            Console.WriteLine("    var results = BarcodeReader.Read(path);");
            Console.WriteLine("    foreach (var barcode in results)");
            Console.WriteLine("    {");
            Console.WriteLine("        Console.WriteLine($\"{barcode.BarcodeType}: {barcode.Text}\");");
            Console.WriteLine("    }");
            Console.WriteLine("}");
            Console.WriteLine();
            Console.WriteLine("This gives you camera input without Scanbot's camera UI,");
            Console.WriteLine("but loses real-time scanning feedback.");

            await Task.CompletedTask;
        }

        public void CompareApproaches()
        {
            Console.WriteLine();
            Console.WriteLine("=== Approach Comparison ===");
            Console.WriteLine();
            Console.WriteLine("| Feature                    | Scanbot Camera | Hybrid (MediaPicker+Iron) |");
            Console.WriteLine("|----------------------------|----------------|---------------------------|");
            Console.WriteLine("| Real-time barcode feedback | Yes            | No                        |");
            Console.WriteLine("| Viewfinder overlay         | Yes            | No                        |");
            Console.WriteLine("| Auto-focus on barcode      | Yes            | No                        |");
            Console.WriteLine("| Continuous scanning        | Yes            | No                        |");
            Console.WriteLine("| Works on saved images      | No             | Yes                       |");
            Console.WriteLine("| Works on PDFs              | No             | Yes                       |");
            Console.WriteLine("| ML error correction        | No             | Yes                       |");
            Console.WriteLine("| Damaged barcode recovery   | Limited        | Yes                       |");
        }
    }

    // ============================================================
    // IRONBARCODE ON MOBILE: PROGRAMMATIC SCENARIOS
    // ============================================================

    /// <summary>
    /// IronBarcode works on mobile for programmatic use cases
    /// that don't require real-time camera scanning.
    /// </summary>
    public class IronBarcodeMobileScenarios
    {
        public void DescribeMobileUseCases()
        {
            Console.WriteLine("=== IronBarcode Mobile Use Cases ===");
            Console.WriteLine();
            Console.WriteLine("IronBarcode on MAUI/Avalonia can:");
            Console.WriteLine();
            Console.WriteLine("1. Process image from photo gallery:");
            Console.WriteLine("   var photo = await MediaPicker.PickPhotoAsync();");
            Console.WriteLine("   var results = BarcodeReader.Read(photoPath);");
            Console.WriteLine();
            Console.WriteLine("2. Process PDF attachment from email:");
            Console.WriteLine("   var results = BarcodeReader.Read(pdfStream);");
            Console.WriteLine();
            Console.WriteLine("3. Generate QR code for display:");
            Console.WriteLine("   var qr = BarcodeWriter.CreateBarcode(data, QRCode);");
            Console.WriteLine("   var bytes = qr.ToPngBinaryData();");
            Console.WriteLine();
            Console.WriteLine("4. Process captured photo (after capture):");
            Console.WriteLine("   var photo = await MediaPicker.CapturePhotoAsync();");
            Console.WriteLine("   // ... save and process with BarcodeReader");
        }

        // Process image selected from gallery
        public async Task ProcessGalleryImage()
        {
            /*
             * // MAUI example
             * var photo = await MediaPicker.PickPhotoAsync();
             *
             * if (photo != null)
             * {
             *     using var stream = await photo.OpenReadAsync();
             *     var results = BarcodeReader.Read(stream);
             *
             *     foreach (var barcode in results)
             *     {
             *         await DisplayAlert("Found", barcode.Text, "OK");
             *     }
             * }
             */
            await Task.CompletedTask;
        }

        // Process PDF received as email attachment
        public void ProcessPdfAttachment(Stream pdfStream)
        {
            var results = IronBarCode.BarcodeReader.Read(pdfStream);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
            }
        }

        // Generate QR code for mobile ticket
        public byte[] GenerateMobileTicketQr(string ticketId)
        {
            var qr = IronBarCode.BarcodeWriter.CreateBarcode(
                $"TICKET:{ticketId}",
                IronBarCode.BarcodeEncoding.QRCode);

            return qr.ToPngBinaryData();
        }
    }

    // ============================================================
    // DECISION FRAMEWORK
    // ============================================================

    public static class CameraFocusDecision
    {
        public static void PrintDecisionGuide()
        {
            Console.WriteLine("=== Camera Focus Decision Guide ===");
            Console.WriteLine();
            Console.WriteLine("Do users actively scan physical barcodes with camera?");
            Console.WriteLine("├── YES: Scanbot (or BarcodeScanning.MAUI)");
            Console.WriteLine("│   Good for: Retail, warehouse, asset tracking");
            Console.WriteLine("│");
            Console.WriteLine("└── NO: IronBarcode");
            Console.WriteLine("    ├── Processing uploaded files? → IronBarcode");
            Console.WriteLine("    ├── Server-side processing? → IronBarcode");
            Console.WriteLine("    ├── PDF barcode extraction? → IronBarcode");
            Console.WriteLine("    ├── Batch processing? → IronBarcode");
            Console.WriteLine("    └── Generating barcodes? → IronBarcode");
        }

        public static void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine("=== Summary ===");
            Console.WriteLine();
            Console.WriteLine("Scanbot:");
            Console.WriteLine("  - Built for real-time camera scanning");
            Console.WriteLine("  - Provides camera UI with viewfinder");
            Console.WriteLine("  - Excellent for mobile point-of-scan");
            Console.WriteLine("  - MAUI mobile only");
            Console.WriteLine();
            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  - Built for file and document processing");
            Console.WriteLine("  - Programmatic API, no camera UI");
            Console.WriteLine("  - Excellent for server and desktop");
            Console.WriteLine("  - Works everywhere .NET runs");
            Console.WriteLine();
            Console.WriteLine("Neither is universally better.");
            Console.WriteLine("Choose based on your input source:");
            Console.WriteLine("  Camera feed → Scanbot");
            Console.WriteLine("  Files/PDFs → IronBarcode");
        }
    }

    // ============================================================
    // MAIN ENTRY POINT
    // ============================================================

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Scanbot vs IronBarcode: Camera Focus Analysis");
            Console.WriteLine("===============================================");
            Console.WriteLine();

            var scanbot = new ScanbotCameraDesign();
            scanbot.DescribeCameraWorkflow();

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine();

            var ironBarcode = new IronBarcodeFileDesign();
            ironBarcode.DescribeFileWorkflow();

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine();

            var difference = new FundamentalDifference();
            difference.ExplainDifference();
            difference.ShowInputComparison();

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine();

            var hybrid = new HybridApproach();
            await hybrid.CaptureAndProcessWithIronBarcode();
            hybrid.CompareApproaches();

            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine();

            CameraFocusDecision.PrintDecisionGuide();
            CameraFocusDecision.PrintSummary();
        }
    }
}
