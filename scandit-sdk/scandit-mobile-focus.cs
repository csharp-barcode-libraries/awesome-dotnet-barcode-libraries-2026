/*
 * Scandit SDK vs IronBarcode: Mobile Focus Comparison
 *
 * This file demonstrates the fundamental architectural difference
 * between Scandit's camera-first design and IronBarcode's
 * programmatic file-processing approach.
 *
 * Key Insight: Scandit is built for real-time camera scanning.
 * IronBarcode is built for file/document processing.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/csharp/barcode/
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ScanditVsIronBarcode
{
    // ============================================================
    // SCANDIT APPROACH: Camera-Centric Mobile Scanning
    // ============================================================

    /// <summary>
    /// Scandit SDK is designed for real-time camera scanning.
    /// This pseudocode illustrates the camera-first architecture.
    /// </summary>
    public class ScanditCameraApproach
    {
        /*
         * Scandit requires:
         * - Mobile MAUI/Xamarin project
         * - Camera permissions in app manifest
         * - Camera hardware on device
         * - UI view for camera preview
         * - Real-time video frame processing
         *
         * This is the correct tool for:
         * - Retail self-checkout apps
         * - Warehouse pick/pack mobile apps
         * - Field service barcode verification
         * - Any scenario where a person scans physical items
         */

        // Conceptual Scandit implementation
        // (Actual API requires Scandit.DataCapture.* packages)

        public void InitializeScandit()
        {
            // Step 1: Initialize with license key
            // DataCaptureContext.ForLicenseKey("YOUR-SCANDIT-LICENSE");

            // Step 2: Configure which barcode types to scan
            // var settings = BarcodeCaptureSettings.Create();
            // settings.EnableSymbologies(Symbology.Ean13Upca, Symbology.QrCode);

            // Step 3: Create capture mode and attach to camera
            // barcodeCapture = BarcodeCapture.Create(context, settings);

            // Step 4: Setup camera frame source
            // camera = Camera.GetDefaultCamera();
            // context.SetFrameSourceAsync(camera);

            // Step 5: Add UI view for camera preview
            // This requires XAML or UI framework integration
        }

        public void StartScanning()
        {
            // Turn on camera and start frame processing
            // camera.SwitchToDesiredStateAsync(FrameSourceState.On);
            // barcodeCapture.IsEnabled = true;

            // Scandit processes each video frame looking for barcodes
            // Results arrive via event callbacks with sub-100ms latency
        }

        public void OnBarcodeScanned(string barcodeData, string symbology)
        {
            // This callback fires when barcode is detected in camera view
            Console.WriteLine($"Scanned: {barcodeData} ({symbology})");

            // Typical actions:
            // - Play audio feedback
            // - Update AR overlay with product info
            // - Add item to cart/inventory
            // - Navigate to detail screen
        }

        /*
         * Scandit Strengths:
         * - Optimized for mobile camera hardware
         * - Sub-100ms scanning latency
         * - AR overlay capabilities
         * - MatrixScan for multiple simultaneous barcodes
         * - Extensive device compatibility testing
         *
         * Scandit Limitations:
         * - Requires camera hardware
         * - Mobile-first design
         * - Volume-based pricing
         * - Limited server-side use cases
         * - Complex pricing requires quotes
         */
    }

    // ============================================================
    // IRONBARCODE APPROACH: File-First Document Processing
    // ============================================================

    /// <summary>
    /// IronBarcode is designed for programmatic file processing.
    /// Works in any .NET environment without camera requirements.
    /// </summary>
    public class IronBarcodeFileApproach
    {
        /*
         * IronBarcode requires:
         * - Any .NET project (console, web, desktop, mobile)
         * - Image files, PDFs, or byte arrays as input
         * - No camera or UI requirements
         *
         * Install: dotnet add package IronBarcode
         */

        public void ProcessDocumentBarcodes()
        {
            // IronBarcode processes files, not camera feeds
            // Works in console apps, APIs, Azure Functions, Docker, etc.

            // Read barcodes from PDF
            var pdfResults = IronBarCode.BarcodeReader.Read("invoice.pdf");
            foreach (var barcode in pdfResults)
            {
                Console.WriteLine($"PDF Page {barcode.PageNumber}: {barcode.Text}");
            }

            // Read barcodes from image
            var imageResults = IronBarCode.BarcodeReader.Read("barcode.png");
            foreach (var barcode in imageResults)
            {
                Console.WriteLine($"Found: {barcode.BarcodeType} = {barcode.Text}");
            }

            // Batch process directory of images
            var files = Directory.GetFiles("./scanned-documents/", "*.png");
            foreach (var file in files)
            {
                var results = IronBarCode.BarcodeReader.Read(file);
                ProcessBarcodeResults(file, results);
            }
        }

        public void GenerateBarcodes()
        {
            // Generate barcode and save to file
            IronBarCode.BarcodeWriter.CreateBarcode("123456789",
                    IronBarCode.BarcodeEncoding.Code128)
                .ResizeTo(400, 100)
                .SaveAsPng("shipping-label.png");

            // Generate QR code with URL
            IronBarCode.BarcodeWriter.CreateBarcode("https://example.com/product/12345",
                    IronBarCode.BarcodeEncoding.QRCode)
                .ResizeTo(200, 200)
                .SaveAsPng("product-qr.png");

            // Generate barcode directly to PDF
            IronBarCode.BarcodeWriter.CreateBarcode("TRACK-123-456",
                    IronBarCode.BarcodeEncoding.Code128)
                .SaveAsPdf("tracking-barcode.pdf");
        }

        public void ServerSideProcessing()
        {
            // IronBarcode is ideal for server-side scenarios
            // These would be impractical with camera-based SDKs

            // ASP.NET Core endpoint processing uploaded files
            // Azure Function processing blob storage documents
            // Docker container batch processing
            // Linux server document workflows
        }

        private void ProcessBarcodeResults(string filename,
            IronBarCode.BarcodeResults results)
        {
            Console.WriteLine($"File: {filename}");
            foreach (var barcode in results)
            {
                Console.WriteLine($"  {barcode.BarcodeType}: {barcode.Text}");
            }
        }
    }

    // ============================================================
    // IRONBARCODE ON MOBILE: Programmatic Use Cases
    // ============================================================

    /// <summary>
    /// IronBarcode works on mobile (MAUI, Avalonia) for
    /// programmatic scenarios - not camera preview scanning.
    /// </summary>
    public class IronBarcodeMobileScenarios
    {
        /*
         * IronBarcode on MAUI/Avalonia:
         * - Process images from camera roll/gallery
         * - Extract barcodes from PDF attachments
         * - Generate barcodes for display
         * - Process captured image files
         *
         * IronBarcode does NOT provide:
         * - Camera preview UI
         * - Real-time video frame processing
         * - Camera permission handling
         * - AR overlays
         *
         * For camera scanning, use Scandit or BarcodeScanning.MAUI
         * For file processing on mobile, use IronBarcode
         */

        public void ProcessPhotoFromGallery(string photoPath)
        {
            // User selects photo from gallery
            // IronBarcode processes the image file

            var results = IronBarCode.BarcodeReader.Read(photoPath);
            if (results.Any())
            {
                var barcode = results.First();
                Console.WriteLine($"Found barcode: {barcode.Text}");
            }
        }

        public void ProcessEmailAttachment(byte[] pdfBytes)
        {
            // User receives email with PDF attachment
            // IronBarcode extracts barcodes from PDF

            using var stream = new MemoryStream(pdfBytes);
            var results = IronBarCode.BarcodeReader.Read(stream);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Extracted: {barcode.Text}");
            }
        }

        public byte[] GenerateTicketQrCode(string ticketId)
        {
            // Generate QR code for mobile ticket display
            var qrCode = IronBarCode.BarcodeWriter.CreateBarcode(
                $"TICKET:{ticketId}",
                IronBarCode.BarcodeEncoding.QRCode);

            return qrCode.ToPngBinaryData();
        }
    }

    // ============================================================
    // HYBRID APPROACH: Both Tools Together
    // ============================================================

    /// <summary>
    /// For organizations with both use cases, use each tool
    /// for what it does best.
    /// </summary>
    public class HybridApproach
    {
        /*
         * Mobile Field App (Scandit):
         * - Warehouse worker scans items with camera
         * - Real-time AR overlay shows item details
         * - Scanned data sent to backend API
         *
         * Backend Server (IronBarcode):
         * - API receives uploaded documents
         * - Extracts barcodes from shipping PDFs
         * - Generates barcode labels for printing
         * - Batch processes document archives
         *
         * Each tool handles its optimal use case
         */

        public void FieldWorkerScanning()
        {
            // Scandit handles camera-based scanning
            // Fast, AR-enabled, mobile-optimized
        }

        public void BackendDocumentProcessing()
        {
            // IronBarcode handles document workflows
            var results = IronBarCode.BarcodeReader.Read("uploaded-invoice.pdf");
            foreach (var barcode in results)
            {
                RouteDocument(barcode.Text);
            }
        }

        private void RouteDocument(string barcodeValue)
        {
            // Route document based on barcode content
            Console.WriteLine($"Routing document: {barcodeValue}");
        }
    }

    // ============================================================
    // DECISION FRAMEWORK
    // ============================================================

    public static class DecisionFramework
    {
        public static void PrintDecisionGuide()
        {
            Console.WriteLine("=== Scandit vs IronBarcode Decision Guide ===");
            Console.WriteLine();
            Console.WriteLine("Choose SCANDIT when:");
            Console.WriteLine("  - Building mobile camera scanning apps");
            Console.WriteLine("  - Needing AR overlay experiences");
            Console.WriteLine("  - Real-time scanning is critical");
            Console.WriteLine("  - Deploying large mobile workforces");
            Console.WriteLine();
            Console.WriteLine("Choose IRONBARCODE when:");
            Console.WriteLine("  - Processing barcodes from files/documents");
            Console.WriteLine("  - Building server-side barcode APIs");
            Console.WriteLine("  - Needing predictable licensing costs");
            Console.WriteLine("  - Requiring PDF barcode extraction");
            Console.WriteLine("  - Processing damaged/low-quality barcodes");
            Console.WriteLine();
            Console.WriteLine("Consider BOTH when:");
            Console.WriteLine("  - Mobile workers scan physical items (Scandit)");
            Console.WriteLine("  - AND server processes documents (IronBarcode)");
        }
    }
}
