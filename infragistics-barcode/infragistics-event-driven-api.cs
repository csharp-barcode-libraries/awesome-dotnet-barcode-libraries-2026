/**
 * Infragistics BarcodeReader Event-Driven API Complexity
 *
 * This example demonstrates the API complexity of Infragistics BarcodeReader:
 * - Uses event-driven pattern (DecodeComplete callback)
 * - Requires symbology specification (no auto-detection)
 * - Complex async handling for sequential reads
 * - Additional boilerplate for batch processing
 *
 * IronBarcode provides synchronous, one-line API with automatic detection.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Comparison: Infragistics Barcode vs IronBarcode
 */

// ============================================================================
// INFRAGISTICS APPROACH: Event-Driven with Callbacks
// ============================================================================

namespace InfragisticsEventDriven
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Infragistics BarcodeReader uses an event-driven pattern
    /// Results are delivered via DecodeComplete event callback
    /// </summary>
    public class InfragisticsEventDrivenReader
    {
        /*
        using Infragistics.Controls.Barcodes;
        using System.Windows.Media.Imaging;

        private BarcodeReader _reader;

        // Must maintain state for the callback
        private TaskCompletionSource<string> _currentResult;
        private string _currentImagePath;

        public InfragisticsEventDrivenReader()
        {
            // Step 1: Initialize reader
            _reader = new BarcodeReader();

            // Step 2: Wire up event handler
            _reader.DecodeComplete += OnDecodeComplete;
        }

        /// <summary>
        /// Basic single barcode read - already complex
        /// </summary>
        public async Task<string> ReadBarcodeAsync(string imagePath)
        {
            // Step 3: Create task completion source for async/await conversion
            _currentResult = new TaskCompletionSource<string>();
            _currentImagePath = imagePath;

            // Step 4: Load image as WPF BitmapSource
            var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

            // Step 5: Configure symbology types (NO auto-detection)
            // Must specify EVERY format you want to support
            _reader.SymbologyTypes = SymbologyType.Code128 |
                                     SymbologyType.Code39 |
                                     SymbologyType.Code39Extended |
                                     SymbologyType.QR |
                                     SymbologyType.EAN8 |
                                     SymbologyType.EAN13 |
                                     SymbologyType.UPCA |
                                     SymbologyType.UPCE |
                                     SymbologyType.DataMatrix |
                                     SymbologyType.Interleaved2of5;

            // Step 6: Trigger decode (async operation)
            _reader.Decode(bitmap);

            // Step 7: Wait for callback to complete
            // Result comes via OnDecodeComplete event
            return await _currentResult.Task;
        }

        /// <summary>
        /// Event handler - called when decode completes
        /// </summary>
        private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
        {
            // Step 8: Process result in callback
            if (e.SymbologyValue != null)
            {
                _currentResult?.TrySetResult($"{e.Symbology}: {e.SymbologyValue}");
            }
            else
            {
                _currentResult?.TrySetResult("No barcode found");
            }
        }

        /// <summary>
        /// Batch processing with events is much more complex
        /// </summary>
        public async Task<Dictionary<string, string>> ReadMultipleBarcodes(string[] imagePaths)
        {
            var results = new Dictionary<string, string>();

            // Must process sequentially due to shared event handler
            // Cannot easily parallelize with event-driven pattern
            foreach (var path in imagePaths)
            {
                try
                {
                    var result = await ReadBarcodeAsync(path);
                    results[path] = result;
                }
                catch (Exception ex)
                {
                    results[path] = $"Error: {ex.Message}";
                }
            }

            return results;
        }

        public void Dispose()
        {
            // Step 9: Clean up event handler
            if (_reader != null)
            {
                _reader.DecodeComplete -= OnDecodeComplete;
                _reader = null;
            }
        }
        */

        public void ShowEventDrivenComplexity()
        {
            Console.WriteLine("Infragistics BarcodeReader Event-Driven Pattern:");
            Console.WriteLine();
            Console.WriteLine("  Required Steps for Single Read:");
            Console.WriteLine("  1. Initialize BarcodeReader instance");
            Console.WriteLine("  2. Wire up DecodeComplete event handler");
            Console.WriteLine("  3. Create TaskCompletionSource for async conversion");
            Console.WriteLine("  4. Load image as BitmapSource");
            Console.WriteLine("  5. Configure SymbologyTypes (must specify each format)");
            Console.WriteLine("  6. Call Decode() to start async operation");
            Console.WriteLine("  7. Wait for event callback");
            Console.WriteLine("  8. Process result in callback");
            Console.WriteLine("  9. Clean up event handler on dispose");
            Console.WriteLine();
            Console.WriteLine("  Total lines of code: 40-60 for basic read");
            Console.WriteLine("  Batch processing: Additional 20+ lines");
        }
    }

    /// <summary>
    /// Full implementation showing event-driven complexity
    /// </summary>
    public class FullInfragisticsImplementation
    {
        public void ShowFullCode()
        {
            var code = @"
// Infragistics Event-Driven Barcode Reading (Full Implementation)
// Lines: ~70 for production-ready code

public class InfragisticsBarcodeService : IDisposable
{
    private BarcodeReader _reader;
    private TaskCompletionSource<DecodeResult> _pendingResult;
    private readonly object _lock = new object();
    private bool _isProcessing;

    public InfragisticsBarcodeService()
    {
        _reader = new BarcodeReader();
        _reader.DecodeComplete += OnDecodeComplete;
    }

    public async Task<string> ReadBarcodeAsync(string imagePath, int timeoutMs = 5000)
    {
        // Thread safety for concurrent calls
        lock (_lock)
        {
            if (_isProcessing)
                throw new InvalidOperationException(""Reader is busy"");
            _isProcessing = true;
        }

        try
        {
            _pendingResult = new TaskCompletionSource<DecodeResult>();

            // Load image
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();

            // Configure symbologies - no auto-detection
            _reader.SymbologyTypes = SymbologyType.Code128 |
                                     SymbologyType.Code39 |
                                     SymbologyType.QR |
                                     SymbologyType.EAN13 |
                                     SymbologyType.DataMatrix;

            // Start decode
            _reader.Decode(bitmap);

            // Wait with timeout
            var completedTask = await Task.WhenAny(
                _pendingResult.Task,
                Task.Delay(timeoutMs)
            );

            if (completedTask != _pendingResult.Task)
            {
                throw new TimeoutException(""Barcode decode timed out"");
            }

            var result = await _pendingResult.Task;
            return result.Value ?? ""No barcode found"";
        }
        finally
        {
            lock (_lock)
            {
                _isProcessing = false;
            }
        }
    }

    private void OnDecodeComplete(object sender, ReaderDecodeArgs e)
    {
        _pendingResult?.TrySetResult(new DecodeResult
        {
            Symbology = e.Symbology?.ToString(),
            Value = e.SymbologyValue
        });
    }

    public void Dispose()
    {
        if (_reader != null)
        {
            _reader.DecodeComplete -= OnDecodeComplete;
            _reader = null;
        }
    }

    private class DecodeResult
    {
        public string Symbology { get; set; }
        public string Value { get; set; }
    }
}

// Usage:
using var service = new InfragisticsBarcodeService();
var result = await service.ReadBarcodeAsync(""barcode.png"");
";

            Console.WriteLine(code);
        }
    }
}

// ============================================================================
// IRONBARCODE APPROACH: Synchronous One-Liner
// ============================================================================

namespace IronBarcodeSynchronous
{
    using IronBarCode;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// IronBarcode provides simple synchronous API
    /// No events, no callbacks, no complexity
    /// </summary>
    public class IronBarcodeSynchronousReader
    {
        // Install: dotnet add package IronBarcode
        // License: IronBarCode.License.LicenseKey = "YOUR-KEY";

        /// <summary>
        /// Single barcode read - one line
        /// </summary>
        public string ReadBarcode(string imagePath)
        {
            // That's it. One line.
            // - Automatic format detection
            // - Synchronous result
            // - No event handling
            var results = BarcodeReader.Read(imagePath);

            return results.FirstOrDefault()?.Text ?? "No barcode found";
        }

        /// <summary>
        /// Batch processing - still simple
        /// </summary>
        public Dictionary<string, string> ReadMultipleBarcodes(string[] imagePaths)
        {
            // Pass array directly - IronBarcode handles everything
            // - Internal parallelization
            // - Automatic format detection per image
            // - Results grouped by source
            var results = BarcodeReader.Read(imagePaths);

            return results
                .GroupBy(r => r.InputPath ?? "")
                .ToDictionary(
                    g => g.Key,
                    g => g.First().Text
                );
        }

        /// <summary>
        /// Async version (if needed for UI)
        /// </summary>
        public async Task<string> ReadBarcodeAsync(string imagePath)
        {
            // Simple async wrapper around synchronous API
            return await Task.Run(() =>
            {
                var results = BarcodeReader.Read(imagePath);
                return results.FirstOrDefault()?.Text ?? "No barcode found";
            });
        }

        public void ShowSimplicity()
        {
            Console.WriteLine("IronBarcode Synchronous Pattern:");
            Console.WriteLine();
            Console.WriteLine("  Required Steps for Single Read:");
            Console.WriteLine("  1. Call BarcodeReader.Read(imagePath)");
            Console.WriteLine("  2. Get result");
            Console.WriteLine();
            Console.WriteLine("  That's it. Two steps.");
            Console.WriteLine();
            Console.WriteLine("  Total lines of code: 2 for basic read");
            Console.WriteLine("  Batch processing: 5 lines");
        }
    }

    /// <summary>
    /// Full IronBarcode implementation
    /// </summary>
    public class FullIronBarcodeImplementation
    {
        public void ShowFullCode()
        {
            var code = @"
// IronBarcode Synchronous Barcode Reading (Full Implementation)
// Lines: ~15 for production-ready code

public class IronBarcodeService
{
    public string ReadBarcode(string imagePath)
    {
        var results = BarcodeReader.Read(imagePath);
        return results.FirstOrDefault()?.Text ?? ""No barcode found"";
    }

    public Dictionary<string, string> ReadBatch(string[] imagePaths)
    {
        var results = BarcodeReader.Read(imagePaths);

        return results
            .GroupBy(r => r.InputPath)
            .ToDictionary(g => g.Key, g => g.First().Text);
    }

    public string ReadFromPdf(string pdfPath)
    {
        var results = BarcodeReader.Read(pdfPath);
        return string.Join("", "", results.Select(r => r.Text));
    }
}

// Usage:
var service = new IronBarcodeService();
var result = service.ReadBarcode(""barcode.png"");
";

            Console.WriteLine(code);
        }
    }
}

// ============================================================================
// SIDE-BY-SIDE CODE COMPARISON
// ============================================================================

namespace CodeComparison
{
    using System;

    public class ApiComplexityComparison
    {
        public void ShowSingleReadComparison()
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          SINGLE BARCODE READ: INFRAGISTICS vs IRONBARCODE        ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║  INFRAGISTICS (Event-Driven Pattern)                              ║");
            Console.WriteLine("║  ───────────────────────────────────────                          ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Setup (constructor)                                         ║");
            Console.WriteLine("║    _reader = new BarcodeReader();                                 ║");
            Console.WriteLine("║    _reader.DecodeComplete += OnDecodeComplete;                    ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Read method                                                 ║");
            Console.WriteLine("║    public async Task<string> ReadBarcodeAsync(string path)        ║");
            Console.WriteLine("║    {                                                              ║");
            Console.WriteLine("║        _result = new TaskCompletionSource<string>();              ║");
            Console.WriteLine("║        var bitmap = new BitmapImage(new Uri(path));               ║");
            Console.WriteLine("║        _reader.SymbologyTypes = SymbologyType.Code128 |           ║");
            Console.WriteLine("║                                 SymbologyType.QR |                ║");
            Console.WriteLine("║                                 SymbologyType.EAN13;              ║");
            Console.WriteLine("║        _reader.Decode(bitmap);                                    ║");
            Console.WriteLine("║        return await _result.Task;                                 ║");
            Console.WriteLine("║    }                                                              ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Callback handler                                            ║");
            Console.WriteLine("║    private void OnDecodeComplete(object s, ReaderDecodeArgs e)    ║");
            Console.WriteLine("║    {                                                              ║");
            Console.WriteLine("║        _result?.TrySetResult(e.SymbologyValue);                   ║");
            Console.WriteLine("║    }                                                              ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Total: ~25 lines                                            ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║  IRONBARCODE (Synchronous)                                        ║");
            Console.WriteLine("║  ─────────────────────────                                        ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    public string ReadBarcode(string path)                         ║");
            Console.WriteLine("║    {                                                              ║");
            Console.WriteLine("║        var results = BarcodeReader.Read(path);                    ║");
            Console.WriteLine("║        return results.FirstOrDefault()?.Text;                     ║");
            Console.WriteLine("║    }                                                              ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Total: 4 lines                                              ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝");
        }

        public void ShowBatchProcessingComparison()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           BATCH PROCESSING: INFRAGISTICS vs IRONBARCODE          ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║  INFRAGISTICS                                                     ║");
            Console.WriteLine("║  ───────────                                                      ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    public async Task<Dictionary<string, string>>                  ║");
            Console.WriteLine("║        ReadMultipleBarcodes(string[] paths)                       ║");
            Console.WriteLine("║    {                                                              ║");
            Console.WriteLine("║        var results = new Dictionary<string, string>();            ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║        // Must process sequentially (shared event handler)        ║");
            Console.WriteLine("║        foreach (var path in paths)                                ║");
            Console.WriteLine("║        {                                                          ║");
            Console.WriteLine("║            try                                                    ║");
            Console.WriteLine("║            {                                                      ║");
            Console.WriteLine("║                var result = await ReadBarcodeAsync(path);         ║");
            Console.WriteLine("║                results[path] = result;                            ║");
            Console.WriteLine("║            }                                                      ║");
            Console.WriteLine("║            catch (Exception ex)                                   ║");
            Console.WriteLine("║            {                                                      ║");
            Console.WriteLine("║                results[path] = $\"Error: {ex.Message}\";          ║");
            Console.WriteLine("║            }                                                      ║");
            Console.WriteLine("║        }                                                          ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║        return results;                                            ║");
            Console.WriteLine("║    }                                                              ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Total: ~20 lines (plus the ReadBarcodeAsync method)         ║");
            Console.WriteLine("║    // Cannot easily parallelize with event pattern                ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║  IRONBARCODE                                                      ║");
            Console.WriteLine("║  ──────────                                                       ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    public Dictionary<string, string>                              ║");
            Console.WriteLine("║        ReadMultipleBarcodes(string[] paths)                       ║");
            Console.WriteLine("║    {                                                              ║");
            Console.WriteLine("║        // Pass array directly - IronBarcode parallelizes          ║");
            Console.WriteLine("║        var results = BarcodeReader.Read(paths);                   ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║        return results                                             ║");
            Console.WriteLine("║            .GroupBy(r => r.InputPath)                             ║");
            Console.WriteLine("║            .ToDictionary(g => g.Key, g => g.First().Text);        ║");
            Console.WriteLine("║    }                                                              ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Total: 7 lines                                              ║");
            Console.WriteLine("║    // Automatic parallelization built-in                          ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝");
        }

        public void ShowFormatDetectionComparison()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         FORMAT DETECTION: INFRAGISTICS vs IRONBARCODE            ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║  INFRAGISTICS - Manual Specification Required                     ║");
            Console.WriteLine("║  ───────────────────────────────────────────                      ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Must specify EVERY format you want to support               ║");
            Console.WriteLine("║    _reader.SymbologyTypes = SymbologyType.Code128 |               ║");
            Console.WriteLine("║                            SymbologyType.Code39 |                 ║");
            Console.WriteLine("║                            SymbologyType.Code39Extended |         ║");
            Console.WriteLine("║                            SymbologyType.QR |                     ║");
            Console.WriteLine("║                            SymbologyType.EAN8 |                   ║");
            Console.WriteLine("║                            SymbologyType.EAN13 |                  ║");
            Console.WriteLine("║                            SymbologyType.UPCA |                   ║");
            Console.WriteLine("║                            SymbologyType.UPCE |                   ║");
            Console.WriteLine("║                            SymbologyType.DataMatrix |             ║");
            Console.WriteLine("║                            SymbologyType.Interleaved2of5;         ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // If you forget a format, that barcode won't be decoded       ║");
            Console.WriteLine("║    // More formats = slower scanning                              ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║  IRONBARCODE - Automatic Detection                                ║");
            Console.WriteLine("║  ────────────────────────────────                                 ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // No format specification needed                              ║");
            Console.WriteLine("║    var results = BarcodeReader.Read(imagePath);                   ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║    // Automatically detects any of 50+ formats                    ║");
            Console.WriteLine("║    // Optimized detection algorithm (doesn't slow down)           ║");
            Console.WriteLine("║    // Never misses a format because you forgot to specify it      ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝");
        }
    }

    /// <summary>
    /// Line count comparison table
    /// </summary>
    public class LineCountSummary
    {
        public void ShowTable()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    LINE COUNT COMPARISON                          ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║  Operation              │ Infragistics │ IronBarcode │ Reduction ║");
            Console.WriteLine("║  ──────────────────────────────────────────────────────────────── ║");
            Console.WriteLine("║  Setup/Constructor      │     6        │      0      │   100%    ║");
            Console.WriteLine("║  Single Read            │    20+       │      2      │    90%    ║");
            Console.WriteLine("║  Event Handler          │    10        │      0      │   100%    ║");
            Console.WriteLine("║  Async Conversion       │    15        │      3*     │    80%    ║");
            Console.WriteLine("║  Batch Processing       │    25        │      5      │    80%    ║");
            Console.WriteLine("║  Format Specification   │    10        │      0      │   100%    ║");
            Console.WriteLine("║  Cleanup/Dispose        │     5        │      0      │   100%    ║");
            Console.WriteLine("║  ──────────────────────────────────────────────────────────────── ║");
            Console.WriteLine("║  Production Service     │    70+       │     15      │    78%    ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("║  * IronBarcode is synchronous by default; async wrapper optional  ║");
            Console.WriteLine("║                                                                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝");
        }
    }
}

// ============================================================================
// MAIN: Run the comparisons
// ============================================================================

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Infragistics vs IronBarcode: API Complexity Comparison ===");
        Console.WriteLine();

        // Show Infragistics complexity
        var infragistics = new InfragisticsEventDriven.InfragisticsEventDrivenReader();
        infragistics.ShowEventDrivenComplexity();
        Console.WriteLine();

        Console.WriteLine("Infragistics Full Implementation Example:");
        var infragisticsFull = new InfragisticsEventDriven.FullInfragisticsImplementation();
        infragisticsFull.ShowFullCode();
        Console.WriteLine();

        // Show IronBarcode simplicity
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine();

        var ironBarcode = new IronBarcodeSynchronous.IronBarcodeSynchronousReader();
        ironBarcode.ShowSimplicity();
        Console.WriteLine();

        Console.WriteLine("IronBarcode Full Implementation Example:");
        var ironBarcodeFull = new IronBarcodeSynchronous.FullIronBarcodeImplementation();
        ironBarcodeFull.ShowFullCode();
        Console.WriteLine();

        // Side-by-side comparisons
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine();

        var comparison = new CodeComparison.ApiComplexityComparison();
        comparison.ShowSingleReadComparison();
        comparison.ShowBatchProcessingComparison();
        comparison.ShowFormatDetectionComparison();

        // Summary
        var summary = new CodeComparison.LineCountSummary();
        summary.ShowTable();
    }
}
