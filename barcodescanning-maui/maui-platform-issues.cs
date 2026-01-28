// BarcodeScanning.Native.Maui: Platform-Specific Issues
// Demonstrates documented issues with BarcodeScanning.Native.Maui
// and provides IronBarcode alternatives for reliable scanning.
//
// Install: dotnet add package BarcodeScanning.Native.Maui
// Known Issues: iOS UPC-A extra digit, permission crashes, PDF417 failures

using BarcodeScanning;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace PlatformIssuesExample
{
    // ============================================================
    // ISSUE 1: iOS UPC-A Extra Leading Zero
    // iOS Vision framework returns UPC-A with 13 digits instead of 12
    // ============================================================

    public class UpcADigitIssue
    {
        // BarcodeScanning.Native.Maui on iOS adds extra leading zero to UPC-A
        public string NormalizeUpcA(OnDetectionFinishedEventArgs e)
        {
            foreach (var barcode in e.BarcodeResults)
            {
                string value = barcode.DisplayValue;

                // Problem: iOS returns "0012345678905" (13 digits)
                // Expected: "012345678905" (12 digits)

                // Detection: Check platform, format, and digit count
                if (DeviceInfo.Platform == DevicePlatform.iOS &&
                    barcode.BarcodeFormat == BarcodeFormats.UpcA)
                {
                    // iOS UPC-A often has extra leading zero
                    if (value.Length == 13 && value.StartsWith("0"))
                    {
                        // Remove the extra leading zero
                        value = value.Substring(1);

                        Console.WriteLine($"iOS UPC-A normalized: {barcode.DisplayValue} -> {value}");
                    }
                }

                // Now you have consistent 12-digit UPC-A
                return value;
            }

            return string.Empty;
        }

        // Cross-platform normalization wrapper
        public string GetNormalizedBarcodeValue(BarcodeResult barcode)
        {
            string value = barcode.DisplayValue;

            // Handle iOS UPC-A quirk
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                switch (barcode.BarcodeFormat)
                {
                    case BarcodeFormats.UpcA:
                        if (value.Length == 13 && value.StartsWith("0"))
                        {
                            value = value.Substring(1);
                        }
                        break;

                    case BarcodeFormats.UpcE:
                        // UPC-E may also have format differences
                        // Add normalization as needed
                        break;
                }
            }

            // Handle Android-specific format differences if any
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // ML Kit returns consistent formatting
                // But verify against your expected formats
            }

            return value;
        }
    }

    // ============================================================
    // ISSUE 2: iOS Permission Denial Causes App Crash
    // Camera view throws exception when permission denied
    // ============================================================

    public partial class SafePermissionPage : ContentPage
    {
        private CameraView _cameraView;

        public SafePermissionPage()
        {
            InitializeComponent();
        }

        // WRONG: Initializing camera without permission check can crash
        public void UnsafeInitialization()
        {
            // DON'T DO THIS - iOS will crash if permission not granted
            // _cameraView.CameraEnabled = true;
        }

        // CORRECT: Always check permissions before enabling camera
        public async Task<bool> SafeInitialization()
        {
            try
            {
                // Step 1: Check current permission status
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                Console.WriteLine($"Camera permission status: {status}");

                // Step 2: Request permission if not granted
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();

                    if (status != PermissionStatus.Granted)
                    {
                        // User denied permission
                        // DO NOT enable camera - will crash on iOS

                        await DisplayAlert(
                            "Camera Permission Required",
                            "Barcode scanning requires camera access. Please enable in Settings.",
                            "OK");

                        return false;
                    }
                }

                // Step 3: Permission granted - safe to enable
                _cameraView.CameraEnabled = true;
                return true;
            }
            catch (PermissionException ex)
            {
                // Platform-specific permission error
                Console.WriteLine($"Permission exception: {ex.Message}");

                await DisplayAlert(
                    "Permission Error",
                    "Unable to request camera permission. Check device settings.",
                    "OK");

                return false;
            }
            catch (Exception ex)
            {
                // Unexpected error during permission handling
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }

        // Handle case where permission was previously denied
        public async Task HandlePreviouslyDeniedPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status == PermissionStatus.Denied)
            {
                // On iOS, once denied, must go to Settings
                // Show instructions to user

                bool openSettings = await DisplayAlert(
                    "Camera Access Disabled",
                    "Camera permission was denied. Open Settings to enable?",
                    "Open Settings",
                    "Cancel");

                if (openSettings)
                {
                    // Open device settings
                    AppInfo.ShowSettingsUI();
                }
            }
        }

        // Lifecycle handling for permission changes
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Re-check permissions when page appears
            // User may have changed settings while app was backgrounded
            await SafeInitialization();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Always disable camera when leaving page
            // Prevents resource leaks and permission issues
            if (_cameraView != null)
            {
                _cameraView.CameraEnabled = false;
            }
        }
    }

    // ============================================================
    // ISSUE 3: PDF417 Scanning "Very Problematic"
    // Native platform APIs have poor PDF417 recognition
    // ============================================================

    public class Pdf417ReliabilityIssue
    {
        private int _scanAttempts = 0;
        private const int MaxRetries = 3;

        // PDF417 detection is unreliable with native APIs
        public void HandlePdf417Detection(OnDetectionFinishedEventArgs e)
        {
            // Look for PDF417 results
            var pdf417Results = e.BarcodeResults
                .Where(b => b.BarcodeFormat == BarcodeFormats.Pdf417)
                .ToList();

            if (pdf417Results.Count > 0)
            {
                // Success - rare but possible
                _scanAttempts = 0;
                ProcessPdf417(pdf417Results[0]);
            }
            else
            {
                // Common scenario: PDF417 in frame but not detected
                // GitHub issue: "PDF417 scanning very problematic, most scans never occur"

                _scanAttempts++;

                if (_scanAttempts < MaxRetries)
                {
                    ShowScanningTips();
                }
                else
                {
                    // After multiple failures, suggest alternative
                    SuggestAlternative();
                }
            }
        }

        private void ShowScanningTips()
        {
            // Tips that sometimes help with PDF417 detection
            Console.WriteLine("PDF417 Scanning Tips:");
            Console.WriteLine("1. Hold device steady");
            Console.WriteLine("2. Ensure good lighting");
            Console.WriteLine("3. Position barcode to fill frame");
            Console.WriteLine("4. Avoid glare on surface");
            Console.WriteLine("5. Try different angles");
        }

        private void SuggestAlternative()
        {
            Console.WriteLine("PDF417 detection failed multiple times.");
            Console.WriteLine("Consider:");
            Console.WriteLine("1. Taking a photo and processing with IronBarcode");
            Console.WriteLine("2. Using IronBarcode for reliable PDF417 scanning");
            Console.WriteLine("3. Manual data entry as fallback");
        }

        private void ProcessPdf417(BarcodeResult result)
        {
            // PDF417 often contains structured data (e.g., driver's license)
            Console.WriteLine($"PDF417 detected: {result.DisplayValue}");
        }
    }

    // ============================================================
    // ISSUE 4: Cross-Platform Format Inconsistencies
    // Same barcode returns different metadata on iOS vs Android
    // ============================================================

    public class CrossPlatformInconsistencies
    {
        // Format enumeration may differ between platforms
        public string GetConsistentFormatName(BarcodeResult barcode)
        {
            // BarcodeFormats enum comes from native APIs
            // Naming may vary between iOS and Android

            var format = barcode.BarcodeFormat;

            // Normalize to consistent naming
            return format switch
            {
                BarcodeFormats.QRCode => "QR_CODE",
                BarcodeFormats.Code128 => "CODE_128",
                BarcodeFormats.Code39 => "CODE_39",
                BarcodeFormats.Code93 => "CODE_93",
                BarcodeFormats.Ean13 => "EAN_13",
                BarcodeFormats.Ean8 => "EAN_8",
                BarcodeFormats.UpcA => "UPC_A",
                BarcodeFormats.UpcE => "UPC_E",
                BarcodeFormats.Itf => "ITF",
                BarcodeFormats.Codabar => "CODABAR",
                BarcodeFormats.DataMatrix => "DATA_MATRIX",
                BarcodeFormats.Pdf417 => "PDF417",
                BarcodeFormats.Aztec => "AZTEC",
                _ => format.ToString()
            };
        }

        // Metadata availability differs between platforms
        public void HandlePlatformSpecificMetadata(BarcodeResult barcode)
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // iOS may provide different metadata fields
                // Vision framework has specific data structure
                Console.WriteLine($"iOS Raw: {barcode.RawValue}");
            }
            else if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Android ML Kit provides its own metadata
                // Bounding box, corner points may differ
                Console.WriteLine($"Android Raw: {barcode.RawValue}");
            }

            // Always use DisplayValue for consistent text data
            Console.WriteLine($"Consistent Display Value: {barcode.DisplayValue}");
        }
    }

    // ============================================================
    // SOLUTION: IronBarcode for Consistent, Reliable Scanning
    // Same results on every platform, every time
    // ============================================================

    public class IronBarcodeConsistentResults
    {
        // No platform normalization needed - managed code everywhere
        public void ProcessWithConsistency(byte[] imageData)
        {
            var results = IronBarCode.BarcodeReader.Read(imageData);

            foreach (var barcode in results)
            {
                // Same enum, same values, same behavior
                // iOS, Android, Windows, macOS, Linux - all identical
                Console.WriteLine($"Type: {barcode.BarcodeType}");
                Console.WriteLine($"Value: {barcode.Text}");

                // UPC-A is always 12 digits - no iOS normalization needed
                if (barcode.BarcodeType == IronBarCode.BarcodeEncoding.UPCA)
                {
                    // Guaranteed 12 digits on all platforms
                    Console.WriteLine($"UPC-A (12 digits): {barcode.Text}");
                }
            }
        }

        // PDF417 works reliably with ML-powered detection
        public void ReliablePdf417(byte[] imageData)
        {
            var results = IronBarCode.BarcodeReader.Read(imageData);

            var pdf417 = results.FirstOrDefault(
                r => r.BarcodeType == IronBarCode.BarcodeEncoding.PDF417);

            if (pdf417 != null)
            {
                // ML-powered error correction handles damaged/partial barcodes
                Console.WriteLine($"PDF417 detected: {pdf417.Text}");

                // Driver's license data, shipping labels, etc.
                // All decode reliably
            }
        }

        // No permission crash risk - no camera dependency
        public void NoPermissionIssues()
        {
            // IronBarcode processes images, not camera feeds
            // No camera permission needed
            // No risk of permission-related crashes

            // Capture image with MAUI Essentials (handles permissions)
            // Then process with IronBarcode (no additional permissions)
        }
    }

    // ============================================================
    // SUMMARY: Platform Issues and Solutions
    // ============================================================

    /*
    BarcodeScanning.Native.Maui Platform Issues:

    1. iOS UPC-A Extra Digit
       - Problem: 13 digits returned instead of 12
       - Workaround: Manual normalization code
       - IronBarcode: Always returns correct 12 digits

    2. iOS Permission Crash
       - Problem: App crashes if camera denied
       - Workaround: Defensive permission checking
       - IronBarcode: No camera needed, no permission crash

    3. PDF417 Unreliable
       - Problem: "Most scans never occur" per GitHub
       - Workaround: Multiple attempts, user instructions
       - IronBarcode: ML-powered, reliable detection

    4. Cross-Platform Inconsistencies
       - Problem: Different results iOS vs Android
       - Workaround: Normalization layer
       - IronBarcode: Same managed code everywhere

    For production applications requiring reliability,
    IronBarcode eliminates these platform-specific issues
    through consistent managed code implementation.
    */
}
