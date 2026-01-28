// ZXing.Net.MAUI: Inherited ZXing.Net Limitations in MAUI Context
// Demonstrates how ZXing.Net's core limitations affect MAUI applications
// and shows IronBarcode's automatic detection alternative.
//
// Install: dotnet add package ZXing.Net.Maui.Controls
// Platforms: iOS, Android (NO Windows, NO macOS)
//
// For detailed ZXing.Net limitations, see: ../zxing-net/README.md

using Microsoft.Maui.Controls;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace ZXingMauiLimitationsExample
{
    // ============================================================
    // LIMITATION 1: Format Specification Required (Inherited from ZXing.Net)
    // ZXing.Net.MAUI inherits the requirement to specify barcode formats
    // ============================================================

    public partial class FormatSpecificationPage : ContentPage
    {
        /*
        XAML Setup:

        <ContentPage xmlns:zxing="clr-namespace:ZXing.Net.Maui.Controls;assembly=ZXing.Net.MAUI.Controls"
                     x:Class="YourApp.FormatSpecificationPage">

            <zxing:CameraBarcodeReaderView x:Name="CameraView"
                Options="{Binding ReaderOptions}"
                BarcodesDetected="OnBarcodesDetected" />

        </ContentPage>
        */

        // BarcodeReaderOptions MUST be configured with formats
        public BarcodeReaderOptions ReaderOptions { get; }

        public FormatSpecificationPage()
        {
            InitializeComponent();

            // REQUIRED: Must specify which formats to scan for
            // This is inherited from ZXing.Net core - not a MAUI-specific issue
            ReaderOptions = new BarcodeReaderOptions
            {
                // You must list every format you want to detect
                Formats = BarcodeFormats.QRCode |
                         BarcodeFormats.Code128 |
                         BarcodeFormats.Code39 |
                         BarcodeFormats.Ean13 |
                         BarcodeFormats.Ean8 |
                         BarcodeFormats.UpcA |
                         BarcodeFormats.UpcE |
                         BarcodeFormats.DataMatrix |
                         BarcodeFormats.Pdf417,

                // Additional options that may help
                TryHarder = true,
                AutoRotate = true,
                Multiple = true
            };

            BindingContext = this;
        }

        private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var result in e.Results)
                {
                    Console.WriteLine($"Detected: {result.Format} - {result.Value}");
                }
            });
        }

        // PROBLEM: What if you miss a format?
        public void DemonstrateFormatMissProblem()
        {
            // Scenario: User scans a GS1 DataBar (RSS-14) barcode
            // But you forgot to include it in your formats list

            var limitedOptions = new BarcodeReaderOptions
            {
                Formats = BarcodeFormats.QRCode | BarcodeFormats.Code128
                // GS1DataBar not included!
            };

            // Result: GS1 DataBar barcodes will NEVER be detected
            // even if they're perfectly visible in the camera
            // User frustration: "Why won't this barcode scan?"
        }
    }

    // ============================================================
    // LIMITATION 2: No Automatic Format Detection
    // Unlike IronBarcode, you can't just scan "any barcode"
    // ============================================================

    public class AutoDetectionComparison
    {
        // ZXing.Net.MAUI approach - MUST specify formats
        public BarcodeReaderOptions CreateZXingOptions()
        {
            // For "scan any barcode" scenario, you must include ALL formats
            // This has performance implications

            return new BarcodeReaderOptions
            {
                // Include every possible format...
                Formats = BarcodeFormats.QRCode |
                         BarcodeFormats.DataMatrix |
                         BarcodeFormats.Aztec |
                         BarcodeFormats.Pdf417 |
                         BarcodeFormats.Code128 |
                         BarcodeFormats.Code39 |
                         BarcodeFormats.Code93 |
                         BarcodeFormats.Ean13 |
                         BarcodeFormats.Ean8 |
                         BarcodeFormats.UpcA |
                         BarcodeFormats.UpcE |
                         BarcodeFormats.Codabar |
                         BarcodeFormats.Itf |
                         BarcodeFormats.Msi |
                         BarcodeFormats.Plessey |
                         BarcodeFormats.MaxiCode,
                         // And any others...

                // More formats = slower scanning
                // Tradeoff between coverage and performance
                TryHarder = true
            };
        }

        // IronBarcode approach - Automatic detection
        public void IronBarcodeAutomatic(byte[] imageBytes)
        {
            // No format specification needed
            // All 50+ formats detected automatically
            // Optimized internally for performance

            var results = IronBarCode.BarcodeReader.Read(imageBytes);

            foreach (var barcode in results)
            {
                // Format is reported in the result
                Console.WriteLine($"Auto-detected: {barcode.BarcodeType} - {barcode.Text}");
            }
        }
    }

    // ============================================================
    // LIMITATION 3: No PDF Support (Inherited from ZXing.Net)
    // Camera-based library can't process documents anyway
    // ============================================================

    public class PdfLimitation
    {
        public void CannotProcessPdf()
        {
            // ZXing.Net.MAUI is camera-only
            // But even ZXing.Net core has no PDF support

            // IMPOSSIBLE with ZXing.Net.MAUI:
            // string pdfPath = "shipping-label.pdf";
            // var results = scanner.ReadFromPdf(pdfPath);  // NO SUCH API

            // For PDF processing, ZXing requires:
            // 1. External PDF renderer (PdfiumViewer, etc.)
            // 2. Render each page to image
            // 3. Process images with ZXing
            // 4. 30+ lines of code for basic functionality

            Console.WriteLine("ZXing.Net.MAUI cannot process PDFs.");
            Console.WriteLine("See ../zxing-net/README.md for PDF workarounds.");
        }

        // IronBarcode handles PDFs natively
        public void IronBarcodePdfSupport()
        {
            // Single line - all pages, all formats
            var results = IronBarCode.BarcodeReader.Read("shipping-label.pdf");

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page {barcode.PageNumber}: {barcode.Text}");
            }
        }
    }

    // ============================================================
    // LIMITATION 4: Limited Error Correction (Inherited from ZXing.Net)
    // Damaged/faded barcodes often fail to scan
    // ============================================================

    public class ErrorCorrectionComparison
    {
        // ZXing.Net.MAUI - TryHarder is the only option
        public BarcodeReaderOptions ZXingErrorHandling()
        {
            return new BarcodeReaderOptions
            {
                Formats = BarcodeFormats.Code128,

                // TryHarder: More aggressive scanning, but still limited
                TryHarder = true,

                // That's it - no ML-powered error correction
                // No adaptive preprocessing
                // Limited tolerance for damaged barcodes
            };
        }

        // IronBarcode - ML-powered error correction
        public void IronBarcodeErrorCorrection(byte[] damagedBarcodeImage)
        {
            // IronBarcode uses ML to handle:
            // - Scratched barcodes
            // - Faded print
            // - Partial occlusion
            // - Low contrast
            // - Noise

            var results = IronBarCode.BarcodeReader.Read(damagedBarcodeImage);
            // Higher success rate on real-world damaged barcodes
        }
    }

    // ============================================================
    // LIMITATION 5: No Windows MAUI Support
    // Windows is completely unsupported
    // ============================================================

    public class WindowsSupportComparison
    {
        public void ZXingWindowsStatus()
        {
            // From GitHub: Windows MAUI is NOT supported
            // No camera implementation for Windows
            // No roadmap to add it

            // If you need Windows MAUI:
            // - ZXing.Net.MAUI cannot help
            // - BarcodeScanning.Native.Maui cannot help
            // - IronBarcode works on all MAUI platforms

            Console.WriteLine("ZXing.Net.MAUI does not support Windows MAUI.");
            Console.WriteLine("For Windows MAUI barcode scanning, use IronBarcode.");
        }

        public void IronBarcodeWindowsSupport()
        {
            // IronBarcode works on ALL MAUI platforms:
            // - iOS
            // - Android
            // - Windows
            // - macOS

            // Same code, same results, everywhere
        }
    }

    // ============================================================
    // IRONBARCODE ALTERNATIVE: Capture and Process Workflow
    // Universal approach that works on all platforms
    // ============================================================

    public partial class IronBarcodeMauiPage : ContentPage
    {
        public IronBarcodeMauiPage()
        {
            InitializeComponent();
        }

        // Works on iOS, Android, AND Windows MAUI
        private async void OnScanClicked(object sender, EventArgs e)
        {
            try
            {
                // Step 1: Capture image
                var photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    // Step 2: Get bytes
                    using var stream = await photo.OpenReadAsync();
                    using var ms = new System.IO.MemoryStream();
                    await stream.CopyToAsync(ms);

                    // Step 3: Process with automatic detection
                    var results = IronBarCode.BarcodeReader.Read(ms.ToArray());

                    // Step 4: Handle results
                    if (results.Count() > 0)
                    {
                        var barcode = results.First();
                        await DisplayAlert("Success",
                            $"Type: {barcode.BarcodeType}\nValue: {barcode.Text}",
                            "OK");
                    }
                    else
                    {
                        await DisplayAlert("No Barcode", "No barcode found", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    // ============================================================
    // SUMMARY: ZXing.Net.MAUI Inherited Limitations
    // ============================================================

    /*
    ZXing.Net.MAUI inherits ALL ZXing.Net limitations:

    1. Format Specification Required
       - Must specify which formats to scan
       - Missing format = barcode not detected
       - Performance tradeoff with many formats

    2. No Automatic Detection
       - Can't scan "any barcode" efficiently
       - User frustration when unexpected format appears

    3. No PDF Support
       - Cannot process PDF documents
       - Requires external libraries for PDF workarounds

    4. Limited Error Correction
       - TryHarder is the only option
       - No ML-powered correction
       - Poor performance on damaged barcodes

    5. No Windows Support (MAUI-specific)
       - Windows MAUI completely unsupported
       - No implementation, no roadmap

    For detailed ZXing.Net core analysis, see:
    ../zxing-net/README.md

    IronBarcode provides:
    - Automatic format detection
    - Native PDF support
    - ML-powered error correction
    - All MAUI platforms including Windows
    */
}
