// BarcodeScanning.Native.Maui: Camera-Only Barcode Scanning
// Demonstrates the camera-only nature of BarcodeScanning.Native.Maui
// and contrasts with IronBarcode's universal approach.
//
// Install: dotnet add package BarcodeScanning.Native.Maui
// Current version: 3.0.3 (Feb 2026)
// Platforms: iOS, Android, Windows (since 3.0.1, ZXingCpp), macOS Catalyst
// Recognition engines differ by platform: Apple Vision (iOS/macOS),
// Google ML Kit (Android), ZXingCpp (Windows).

using BarcodeScanning;
using Microsoft.Maui.Controls;

namespace CameraOnlyExample
{
    // ============================================================
    // PART 1: BarcodeScanning.Native.Maui Setup
    // This library ONLY works with MAUI camera input
    // ============================================================

    /*
    XAML Setup Required (in your .xaml page):

    <ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                 xmlns:scanner="clr-namespace:BarcodeScanning;assembly=BarcodeScanning.Native.Maui"
                 x:Class="YourApp.ScannerPage">

        <Grid>
            <scanner:CameraView x:Name="CameraView"
                                OnDetectionFinished="OnBarcodeDetected"
                                CameraEnabled="True"
                                VibrationOnDetected="True"
                                TorchOn="False" />

            <Label x:Name="ResultLabel"
                   VerticalOptions="End"
                   BackgroundColor="#80000000"
                   TextColor="White"
                   Padding="10" />
        </Grid>
    </ContentPage>
    */

    public partial class BarcodeScanningSamplePage : ContentPage
    {
        public BarcodeScanningSamplePage()
        {
            InitializeComponent();
        }

        // Camera detection event - fired when barcode appears in camera frame
        // Note: e.BarcodeResults is IReadOnlySet<BarcodeResult> — no integer indexer
        private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArg e)
        {
            // WARNING: This ONLY fires when camera sees a barcode
            // You CANNOT:
            //   - Process an existing image file
            //   - Process a PDF document
            //   - Process a screenshot
            //   - Process a base64 string

            if (e.BarcodeResults.Count > 0)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var barcode = e.BarcodeResults.First();
                    ResultLabel.Text = $"Scanned: {barcode.DisplayValue}";

                    // Note: Format names come from three different engines
                    // iOS/macOS uses Apple Vision framework
                    // Android uses Google ML Kit
                    // Windows uses ZXingCpp
                    // Symbology coverage and naming may differ across them
                    Console.WriteLine($"Format: {barcode.BarcodeFormat}");
                    Console.WriteLine($"Raw Value: {barcode.RawValue}");
                    Console.WriteLine($"Display Value: {barcode.DisplayValue}");
                });
            }
        }

        // Control camera state
        public void StartScanning()
        {
            CameraView.CameraEnabled = true;
        }

        public void StopScanning()
        {
            CameraView.CameraEnabled = false;
        }

        public void ToggleTorch()
        {
            CameraView.TorchOn = !CameraView.TorchOn;
        }
    }

    // ============================================================
    // PART 2: What BarcodeScanning.Native.Maui CANNOT Do
    // ============================================================

    public class CameraOnlyLimitations
    {
        // LIMITATION: Cannot process image files
        public void CannotProcessImageFile()
        {
            // This is IMPOSSIBLE with BarcodeScanning.Native.Maui:
            // string imagePath = "barcode.png";
            // var result = scanner.ReadFromFile(imagePath);  // NO SUCH API

            // The library ONLY provides CameraView control
            // There is no method to process existing images

            Console.WriteLine("BarcodeScanning.Native.Maui has no API for image files.");
            Console.WriteLine("It is a camera wrapper, not an image processing library.");
        }

        // LIMITATION: Cannot process PDF documents
        public void CannotProcessPDF()
        {
            // This is IMPOSSIBLE with BarcodeScanning.Native.Maui:
            // string pdfPath = "shipping-label.pdf";
            // var results = scanner.ReadFromPDF(pdfPath);  // NO SUCH API

            Console.WriteLine("BarcodeScanning.Native.Maui cannot process PDFs.");
            Console.WriteLine("Use IronBarcode for PDF barcode extraction.");
        }

        // LIMITATION: Cannot process byte arrays
        public void CannotProcessByteArray()
        {
            // This is IMPOSSIBLE with BarcodeScanning.Native.Maui:
            // byte[] imageData = File.ReadAllBytes("barcode.png");
            // var result = scanner.ReadFromBytes(imageData);  // NO SUCH API

            Console.WriteLine("BarcodeScanning.Native.Maui cannot process byte arrays.");
            Console.WriteLine("It requires live camera feed only.");
        }

        // NOTE: Windows MAUI support was added in version 3.0.1
        // Windows uses a different recognition engine than iOS/Android
        public void WindowsUsesDifferentEngine()
        {
            // Windows target depends on the third-party ZXingCpp package
            // (not Apple Vision and not Google ML Kit)
            // Symbology coverage and decoding behavior on Windows
            // therefore differ from the iOS and Android code paths

            Console.WriteLine("Windows MAUI: supported via ZXingCpp (since 3.0.1).");
            Console.WriteLine("iOS/macOS: Apple Vision. Android: Google ML Kit.");
            Console.WriteLine("Behavior is engine-dependent and not identical across platforms.");
        }
    }

    // ============================================================
    // PART 3: IronBarcode Alternative - Universal Approach
    // Works everywhere: MAUI, ASP.NET, Console, WPF, etc.
    // ============================================================

    /*
    Install: dotnet add package BarCode
    (NuGet package id is "BarCode"; namespace is "IronBarCode" with capital C)

    IronBarcode provides image-based processing that works universally.
    For MAUI mobile apps, capture an image first, then process it.
    */

    public class IronBarcodeUniversalApproach
    {
        // Process from file path - works on all platforms
        public void ReadFromFile()
        {
            // Works on: iOS, Android, Windows, macOS, Linux
            var results = IronBarCode.BarcodeReader.Read("barcode.png");

            foreach (var barcode in results)
            {
                Console.WriteLine($"Format: {barcode.BarcodeType}");
                Console.WriteLine($"Value: {barcode.Value}");
            }
        }

        // Process from byte array - ideal for MAUI camera capture
        public void ReadFromBytes(byte[] capturedImage)
        {
            // Capture image with MAUI Essentials MediaPicker
            // Then process with IronBarcode

            var results = IronBarCode.BarcodeReader.Read(capturedImage);

            foreach (var barcode in results)
            {
                // Results are consistent across all platforms
                // No iOS vs Android differences to handle
                Console.WriteLine($"Value: {barcode.Value}");
            }
        }

        // Process PDF documents - not possible with camera wrappers
        public void ReadFromPDF()
        {
            // Native PDF support - no external libraries needed
            var results = IronBarCode.BarcodeReader.Read("shipping-manifest.pdf");

            foreach (var barcode in results)
            {
                Console.WriteLine($"Page: {barcode.PageNumber}");
                Console.WriteLine($"Value: {barcode.Value}");
            }
        }

        // Process stream - useful for network or cloud sources
        public void ReadFromStream(System.IO.Stream imageStream)
        {
            var results = IronBarCode.BarcodeReader.Read(imageStream);

            foreach (var barcode in results)
            {
                Console.WriteLine($"Value: {barcode.Value}");
            }
        }
    }

    // ============================================================
    // PART 4: MAUI Workflow with IronBarcode
    // Capture image, then process - works on ALL MAUI platforms
    // ============================================================

    public partial class IronBarcodeMauiPage : ContentPage
    {
        public IronBarcodeMauiPage()
        {
            InitializeComponent();
        }

        // Works on iOS, Android, AND Windows MAUI
        private async void OnScanButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Step 1: Capture image using MAUI Essentials
                var photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    // Step 2: Get image bytes
                    using var stream = await photo.OpenReadAsync();
                    using var memoryStream = new System.IO.MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    // Step 3: Process with IronBarcode
                    var results = IronBarCode.BarcodeReader.Read(imageBytes);

                    // Step 4: Handle results
                    if (results.Count() > 0)
                    {
                        var barcode = results.First();
                        await DisplayAlert("Barcode Found",
                            $"Type: {barcode.BarcodeType}\nValue: {barcode.Value}",
                            "OK");
                    }
                    else
                    {
                        await DisplayAlert("No Barcode", "No barcode detected in image.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Can also process existing photos from gallery
        private async void OnPickPhotoClicked(object sender, EventArgs e)
        {
            var photo = await MediaPicker.PickPhotoAsync();

            if (photo != null)
            {
                using var stream = await photo.OpenReadAsync();
                using var memoryStream = new System.IO.MemoryStream();
                await stream.CopyToAsync(memoryStream);

                var results = IronBarCode.BarcodeReader.Read(memoryStream.ToArray());

                // Process results...
            }
        }
    }

    // ============================================================
    // SUMMARY: Camera Wrapper vs Universal Library
    // ============================================================

    /*
    BarcodeScanning.Native.Maui (v3.0.3):
    - Camera-only: CameraView control fires events when barcode in frame
    - MAUI: iOS, Android, Windows (since 3.0.1, ZXingCpp), macOS Catalyst
    - Three engines: Apple Vision / Google ML Kit / ZXingCpp depending on platform
    - No image processing: Cannot read files, streams, byte arrays, PDFs
    - Free: MIT license

    IronBarcode:
    - Image processing: Read from any source (file, stream, bytes, PDF)
    - Universal .NET: MAUI (all platforms), ASP.NET, Console, WPF, etc.
    - Single managed engine across all targets
    - PDF native: Built-in document processing
    - Commercial: Licensed, supported (perpetual: $799 / $1,199 / $2,399 / $4,799)

    For production MAUI apps that need a single engine across platforms,
    document processing, or server-side reuse, IronBarcode is the
    appropriate choice.

    For quick MAUI camera prototypes with zero budget,
    BarcodeScanning.Native.Maui may be sufficient.
    */
}
