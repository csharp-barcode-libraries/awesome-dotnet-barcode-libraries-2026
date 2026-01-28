// ZXing.Net.MAUI: Camera-Specific Issues and Workarounds
// Demonstrates MAUI-specific camera issues with ZXing.Net.MAUI
// including iPhone 15 auto-focus problems and memory leaks.
//
// Install: dotnet add package ZXing.Net.Maui.Controls
// Known Issues: iPhone 15 PRO focus, camera resource leaks, build errors

using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace ZXingMauiCameraIssuesExample
{
    // ============================================================
    // ISSUE 1: iPhone 15 PRO Auto-Focus Problems
    // Camera fails to focus on barcodes, causing detection failures
    // ============================================================

    public partial class IPhoneFocusIssuePage : ContentPage
    {
        private CameraBarcodeReaderView _cameraView;
        private bool _isDetecting = true;
        private int _failedAttempts = 0;
        private const int MaxAttemptsBeforeTip = 3;

        /*
        GitHub Issue: iPhone 15 PRO models have difficulty focusing on barcodes.
        The camera view shows the barcode but fails to achieve sharp focus
        required for reliable detection.

        Workaround attempts documented below - none are fully reliable.
        */

        public IPhoneFocusIssuePage()
        {
            InitializeComponent();
            _cameraView = this.FindByName<CameraBarcodeReaderView>("CameraView");

            // Check if device may have focus issues
            CheckDeviceForKnownIssues();
        }

        private void CheckDeviceForKnownIssues()
        {
            // iPhone 15 PRO models are documented to have focus issues
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                string model = DeviceInfo.Model;

                // iPhone 15 PRO models: iPhone16,1 (Pro), iPhone16,2 (Pro Max)
                if (model.Contains("iPhone16"))
                {
                    Console.WriteLine("WARNING: iPhone 15 PRO detected.");
                    Console.WriteLine("Known auto-focus issues may affect barcode scanning.");
                    Console.WriteLine("User may need to manually adjust phone distance.");
                }
            }
        }

        // Workaround attempt 1: Torch toggle to trigger focus
        private async void TryTorchToggleFocus()
        {
            // Theory: Toggling torch may trigger focus system
            // Reality: Works sometimes, not reliable

            try
            {
                if (_cameraView != null)
                {
                    _cameraView.IsTorchOn = true;
                    await Task.Delay(200);
                    _cameraView.IsTorchOn = false;

                    Console.WriteLine("Torch toggle attempted to trigger focus.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Torch toggle failed: {ex.Message}");
            }
        }

        // Workaround attempt 2: User instruction overlay
        private void ShowFocusHelpOverlay()
        {
            // When scans are failing, show user instructions
            // Not ideal UX but may help

            var overlay = new Label
            {
                Text = "Having trouble scanning?\n" +
                       "Try moving your phone closer or farther\n" +
                       "until the barcode looks sharp.",
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("#80000000"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = 20
            };

            // Add to page layout...
        }

        // Track failed attempts and show help
        private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (e.Results.Count > 0)
            {
                // Success - reset counter
                _failedAttempts = 0;
                ProcessBarcode(e.Results.First());
            }
        }

        // Called periodically when no detection occurs
        private void OnDetectionTimeout()
        {
            _failedAttempts++;

            if (_failedAttempts == MaxAttemptsBeforeTip)
            {
                // After several failed frames, try torch toggle
                TryTorchToggleFocus();
            }
            else if (_failedAttempts > MaxAttemptsBeforeTip * 2)
            {
                // Still failing - show user help
                ShowFocusHelpOverlay();
            }
        }

        private void ProcessBarcode(BarcodeResult result)
        {
            Console.WriteLine($"Scanned: {result.Value}");
        }
    }

    // ============================================================
    // ISSUE 2: Camera Not Released Properly (Memory Leaks)
    // Camera resources persist after page navigation
    // ============================================================

    public partial class CameraLifecyclePage : ContentPage
    {
        private CameraBarcodeReaderView _cameraView;
        private bool _cameraInitialized = false;

        /*
        GitHub Issue: When navigating away from scanning pages,
        camera resources may not be released properly, causing:
        - Memory leaks over time
        - Camera remaining locked
        - Battery drain from background camera activity
        - Crashes when returning to scan page
        */

        public CameraLifecyclePage()
        {
            InitializeComponent();
            _cameraView = this.FindByName<CameraBarcodeReaderView>("CameraView");
        }

        // CRITICAL: Must manually manage camera lifecycle
        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (_cameraView != null)
                {
                    // Enable camera when page appears
                    _cameraView.IsDetecting = true;
                    _cameraInitialized = true;

                    Console.WriteLine("Camera enabled on page appear.");
                }
            }
            catch (Exception ex)
            {
                // Camera may fail to initialize
                // Especially on return to page
                Console.WriteLine($"Camera init error: {ex.Message}");
                ShowCameraErrorAlert();
            }
        }

        // CRITICAL: Must disable camera when leaving page
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            try
            {
                if (_cameraView != null && _cameraInitialized)
                {
                    // Disable detection to release camera
                    _cameraView.IsDetecting = false;

                    // Note: ZXing.Net.MAUI doesn't have formal Dispose()
                    // This may not fully release resources

                    Console.WriteLine("Camera disabled on page disappear.");
                }
            }
            catch (Exception ex)
            {
                // May fail if camera already released
                Console.WriteLine($"Camera cleanup error: {ex.Message}");
            }
        }

        // Additional cleanup on page destruction
        ~CameraLifecyclePage()
        {
            try
            {
                if (_cameraView != null)
                {
                    _cameraView.IsDetecting = false;
                }
            }
            catch
            {
                // Ignore finalization errors
            }
        }

        private async void ShowCameraErrorAlert()
        {
            await DisplayAlert(
                "Camera Error",
                "Unable to access camera. Please restart the app.",
                "OK");
        }
    }

    // ============================================================
    // ISSUE 3: Memory Monitoring for Camera Leaks
    // Track memory to detect camera resource issues
    // ============================================================

    public class CameraMemoryMonitor
    {
        private long _baselineMemory;
        private int _navigationCount = 0;

        public CameraMemoryMonitor()
        {
            _baselineMemory = GC.GetTotalMemory(true);
        }

        public void OnNavigatedToScanPage()
        {
            _navigationCount++;

            // Force garbage collection to get accurate reading
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long currentMemory = GC.GetTotalMemory(true);
            long memoryGrowth = currentMemory - _baselineMemory;

            Console.WriteLine($"Navigation #{_navigationCount}");
            Console.WriteLine($"Memory growth: {memoryGrowth / 1024 / 1024} MB");

            // WARNING: Memory growing with each navigation indicates leak
            if (memoryGrowth > 50 * 1024 * 1024) // 50MB threshold
            {
                Console.WriteLine("WARNING: Possible camera memory leak detected.");
                Console.WriteLine("Memory has grown significantly since app start.");
            }
        }

        public void Reset()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            _baselineMemory = GC.GetTotalMemory(true);
            _navigationCount = 0;
        }
    }

    // ============================================================
    // ISSUE 4: Android Camera Library Build Errors
    // Compatibility issues with Camera 1.5.0
    // ============================================================

    /*
    GitHub Issue: Android Camera 1.5.0 library causes build errors
    Error manifests as missing method exceptions or type load failures

    WORKAROUND: Pin camera library to earlier version

    In your .csproj file:

    <ItemGroup Condition="'$(TargetFramework)' == 'net8.0-android'">
        <!-- Pin camera library to avoid 1.5.0 issues -->
        <PackageReference Include="Xamarin.AndroidX.Camera.Camera2"
                          Version="1.3.0" />
        <PackageReference Include="Xamarin.AndroidX.Camera.Core"
                          Version="1.3.0" />
        <PackageReference Include="Xamarin.AndroidX.Camera.Lifecycle"
                          Version="1.3.0" />
        <PackageReference Include="Xamarin.AndroidX.Camera.View"
                          Version="1.3.0" />
    </ItemGroup>

    Note: This is a moving target. Check GitHub issues for current status.
    */

    public class AndroidBuildIssueWorkaround
    {
        public void DocumentWorkaround()
        {
            Console.WriteLine("Android Camera 1.5.0 Build Error Workaround:");
            Console.WriteLine("");
            Console.WriteLine("Add to your .csproj file:");
            Console.WriteLine("");
            Console.WriteLine("<ItemGroup Condition=\"'$(TargetFramework)' == 'net8.0-android'\">");
            Console.WriteLine("  <PackageReference Include=\"Xamarin.AndroidX.Camera.Camera2\" Version=\"1.3.0\" />");
            Console.WriteLine("  <PackageReference Include=\"Xamarin.AndroidX.Camera.Core\" Version=\"1.3.0\" />");
            Console.WriteLine("  <PackageReference Include=\"Xamarin.AndroidX.Camera.Lifecycle\" Version=\"1.3.0\" />");
            Console.WriteLine("  <PackageReference Include=\"Xamarin.AndroidX.Camera.View\" Version=\"1.3.0\" />");
            Console.WriteLine("</ItemGroup>");
        }
    }

    // ============================================================
    // SOLUTION: IronBarcode Eliminates Camera Issues
    // Image capture approach avoids all camera management problems
    // ============================================================

    public partial class IronBarcodeNoCameraIssues : ContentPage
    {
        /*
        IronBarcode uses image processing, not live camera feeds.
        This eliminates:
        - iPhone 15 auto-focus issues (you control when image is captured)
        - Camera memory leaks (no camera view to manage)
        - Build errors (no camera library dependencies)
        - Platform-specific camera behavior

        Trade-off: User must tap to capture instead of automatic detection.
        Benefit: Reliable results on all platforms and devices.
        */

        public IronBarcodeNoCameraIssues()
        {
            InitializeComponent();
        }

        // Works on ALL devices - no focus issues
        private async void OnCaptureClicked(object sender, EventArgs e)
        {
            try
            {
                // Step 1: Capture image (user controls when to capture)
                // User can ensure barcode is in focus before capturing
                var photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    // Step 2: Process with IronBarcode
                    using var stream = await photo.OpenReadAsync();
                    using var ms = new System.IO.MemoryStream();
                    await stream.CopyToAsync(ms);

                    var results = IronBarCode.BarcodeReader.Read(ms.ToArray());

                    // Step 3: Handle results
                    if (results.Count() > 0)
                    {
                        await DisplayAlert("Success",
                            $"Type: {results.First().BarcodeType}\n" +
                            $"Value: {results.First().Text}",
                            "OK");
                    }
                    else
                    {
                        await DisplayAlert("Not Found",
                            "No barcode detected. Please try again.",
                            "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // No OnDisappearing cleanup needed - no camera to manage
        // No memory leak monitoring needed - no persistent camera
        // No device-specific workarounds needed - works everywhere
    }

    // ============================================================
    // SUMMARY: ZXing.Net.MAUI Camera Issues
    // ============================================================

    /*
    ZXing.Net.MAUI MAUI-Specific Camera Issues:

    1. iPhone 15 PRO Auto-Focus
       - Camera shows barcode but won't focus
       - No reliable programmatic fix
       - User must manually adjust distance
       - Major UX problem for newer devices

    2. Camera Memory Leaks
       - Resources not released on page navigation
       - Memory grows with repeated scanning
       - Battery drain from background camera
       - Requires manual lifecycle management

    3. Android Camera 1.5.0 Build Errors
       - Compatibility issues cause build failures
       - Requires version pinning workaround
       - Moving target as libraries update

    4. Pre-Release Stability
       - Version 0.5.0 indicates incomplete
       - No guarantees on fixes or timeline
       - Community-maintained with variable support

    IronBarcode Solution:
    - No camera view = no camera issues
    - User captures image when ready (focus confirmed)
    - No memory management required
    - No platform-specific build issues
    - Production-ready with commercial support
    */
}
