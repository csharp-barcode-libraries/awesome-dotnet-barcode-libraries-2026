/**
 * MessagingToolkit.Barcode to IronBarcode Migration Guide
 *
 * This file provides complete before/after examples for migrating
 * from the abandoned MessagingToolkit.Barcode to IronBarcode.
 *
 * Migration benefits:
 * - Modern .NET support (6, 7, 8, 9)
 * - Active development and security updates
 * - PDF support and automatic format detection
 * - Cross-platform deployment (Windows, Linux, macOS, Docker)
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// IronBarcode - modern, actively maintained library
// Install: dotnet add package IronBarcode
using IronBarcode;

namespace MessagingToolkitMigrationExample
{
    #region API Mapping Reference

    /// <summary>
    /// API mapping between MessagingToolkit.Barcode and IronBarcode
    /// </summary>
    public static class ApiMappingReference
    {
        public static void ShowApiMapping()
        {
            Console.WriteLine("=== API Mapping Reference ===\n");

            var mappings = new[]
            {
                ("MessagingToolkit", "IronBarcode", "Notes"),
                ("---", "---", "---"),
                ("BarcodeDecoder", "BarcodeReader", "Static class in IronBarcode"),
                ("decoder.Decode(bitmap)", "BarcodeReader.Read(path)", "Accepts path, stream, or byte[]"),
                ("BarcodeEncoder", "BarcodeWriter", "Static class in IronBarcode"),
                ("encoder.Encode(data)", "BarcodeWriter.CreateBarcode(data, encoding)", "Explicit encoding"),
                ("Result.Text", "BarcodeResult.Value", "Property rename"),
                ("Result.BarcodeFormat", "BarcodeResult.BarcodeType", "Property rename"),
                ("BarcodeFormat.QrCode", "BarcodeEncoding.QRCode", "Enum rename"),
                ("BarcodeFormat.Code128", "BarcodeEncoding.Code128", "Same name"),
                ("N/A", "BarcodeReader.Read(\"file.pdf\")", "PDF support (NEW)"),
                ("N/A", "barcode.PageNumber", "Multi-page support (NEW)"),
            };

            foreach (var (mt, ib, notes) in mappings)
            {
                Console.WriteLine($"{mt,-35} {ib,-40} {notes}");
            }
        }
    }

    #endregion

    #region Basic Migration Examples

    /// <summary>
    /// Basic barcode reading migration examples
    /// </summary>
    public class ReadingMigration
    {
        /// <summary>
        /// BEFORE: MessagingToolkit.Barcode barcode reading
        /// This code only works on .NET Framework
        /// </summary>
        public void ShowOldCode()
        {
            Console.WriteLine("=== OLD CODE (MessagingToolkit.Barcode) ===\n");

            var code = @"
// MessagingToolkit.Barcode - .NET Framework only
using MessagingToolkit.Barcode;
using System.Drawing;

public string ReadBarcode(string imagePath)
{
    var decoder = new BarcodeDecoder();

    using (var bitmap = new Bitmap(imagePath))
    {
        var result = decoder.Decode(bitmap);
        return result?.Text;
    }
}";
            Console.WriteLine(code);
        }

        /// <summary>
        /// AFTER: IronBarcode barcode reading
        /// Works on all .NET platforms
        /// </summary>
        public string ReadBarcode(string imagePath)
        {
            // IronBarcode - works on all modern .NET
            // No Bitmap required - accepts file path directly
            var results = BarcodeReader.Read(imagePath);

            // Returns collection (handles multi-barcode images)
            return results.FirstOrDefault()?.Value;
        }

        /// <summary>
        /// Shows the new code pattern
        /// </summary>
        public void ShowNewCode()
        {
            Console.WriteLine("\n=== NEW CODE (IronBarcode) ===\n");

            var code = @"
// IronBarcode - all .NET platforms
using IronBarcode;

public string ReadBarcode(string imagePath)
{
    var results = BarcodeReader.Read(imagePath);
    return results.FirstOrDefault()?.Value;
}";
            Console.WriteLine(code);
        }
    }

    /// <summary>
    /// Basic barcode generation migration examples
    /// </summary>
    public class GenerationMigration
    {
        /// <summary>
        /// BEFORE: MessagingToolkit.Barcode generation
        /// </summary>
        public void ShowOldCode()
        {
            Console.WriteLine("\n=== OLD CODE (Generation) ===\n");

            var code = @"
// MessagingToolkit.Barcode - .NET Framework only
using MessagingToolkit.Barcode;

public void CreateBarcode(string data, string outputPath)
{
    var encoder = new BarcodeEncoder();
    encoder.Format = BarcodeFormat.QrCode;

    var bitmap = encoder.Encode(data);
    bitmap.Save(outputPath);
}";
            Console.WriteLine(code);
        }

        /// <summary>
        /// AFTER: IronBarcode generation
        /// </summary>
        public void CreateBarcode(string data, string outputPath)
        {
            // IronBarcode - explicit encoding, fluent API
            var barcode = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode);
            barcode.SaveAsPng(outputPath);
        }

        /// <summary>
        /// Shows the new code pattern
        /// </summary>
        public void ShowNewCode()
        {
            Console.WriteLine("\n=== NEW CODE (Generation) ===\n");

            var code = @"
// IronBarcode - all platforms
using IronBarcode;

public void CreateBarcode(string data, string outputPath)
{
    var barcode = BarcodeWriter.CreateBarcode(data, BarcodeEncoding.QRCode);
    barcode.SaveAsPng(outputPath);
}";
            Console.WriteLine(code);
        }
    }

    #endregion

    #region Advanced Migration Examples

    /// <summary>
    /// Multiple barcode handling migration
    /// </summary>
    public class MultipleBarcodesMigration
    {
        /// <summary>
        /// OLD: MessagingToolkit returned single result
        /// </summary>
        public void ShowOldPattern()
        {
            Console.WriteLine("\n=== OLD PATTERN (Single Result) ===\n");

            var code = @"
// MessagingToolkit returned single result
var result = decoder.Decode(bitmap);
if (result != null)
{
    ProcessBarcode(result.Text);
}";
            Console.WriteLine(code);
        }

        /// <summary>
        /// NEW: IronBarcode returns collection for all barcodes in image
        /// </summary>
        public List<string> ReadAllBarcodes(string imagePath)
        {
            // IronBarcode finds ALL barcodes in image
            var results = BarcodeReader.Read(imagePath);

            // Process each found barcode
            return results.Select(r => r.Value).ToList();
        }

        public void ShowNewPattern()
        {
            Console.WriteLine("\n=== NEW PATTERN (Collection) ===\n");

            var code = @"
// IronBarcode returns all barcodes found
var results = BarcodeReader.Read(imagePath);

foreach (var barcode in results)
{
    Console.WriteLine($""Type: {barcode.BarcodeType}"");
    Console.WriteLine($""Value: {barcode.Value}"");
}

// Or use LINQ
var values = results.Select(r => r.Value).ToList();";
            Console.WriteLine(code);
        }
    }

    /// <summary>
    /// Stream-based processing migration
    /// </summary>
    public class StreamProcessingMigration
    {
        /// <summary>
        /// OLD: Required Bitmap from stream
        /// </summary>
        public void ShowOldPattern()
        {
            Console.WriteLine("\n=== OLD PATTERN (Stream) ===\n");

            var code = @"
// MessagingToolkit required Bitmap
using (var stream = File.OpenRead(imagePath))
using (var bitmap = new Bitmap(stream))
{
    var result = decoder.Decode(bitmap);
    return result?.Text;
}";
            Console.WriteLine(code);
        }

        /// <summary>
        /// NEW: IronBarcode accepts stream directly
        /// </summary>
        public string ReadFromStream(Stream imageStream)
        {
            // IronBarcode reads directly from stream
            var results = BarcodeReader.Read(imageStream);
            return results.FirstOrDefault()?.Value;
        }

        public void ShowNewPattern()
        {
            Console.WriteLine("\n=== NEW PATTERN (Stream) ===\n");

            var code = @"
// IronBarcode accepts stream directly
using (var stream = File.OpenRead(imagePath))
{
    var results = BarcodeReader.Read(stream);
    return results.FirstOrDefault()?.Value;
}

// Or even simpler - just pass the path
var results = BarcodeReader.Read(imagePath);";
            Console.WriteLine(code);
        }
    }

    #endregion

    #region New Capabilities After Migration

    /// <summary>
    /// Demonstrates features available after migration
    /// that were not possible with MessagingToolkit
    /// </summary>
    public class NewCapabilities
    {
        /// <summary>
        /// PDF support - NOT available in MessagingToolkit
        /// </summary>
        public void DemonstratePdfSupport()
        {
            Console.WriteLine("\n=== NEW: PDF Support ===\n");

            var code = @"
// Read barcodes from PDF documents
// This was IMPOSSIBLE with MessagingToolkit
var results = BarcodeReader.Read(""invoice.pdf"");

foreach (var barcode in results)
{
    Console.WriteLine($""Page {barcode.PageNumber}: {barcode.Value}"");
}";
            Console.WriteLine(code);
        }

        /// <summary>
        /// Automatic format detection - NOT in MessagingToolkit
        /// </summary>
        public void DemonstrateAutoDetection()
        {
            Console.WriteLine("\n=== NEW: Automatic Format Detection ===\n");

            var code = @"
// No need to specify barcode format
// IronBarcode auto-detects from 50+ formats
var results = BarcodeReader.Read(""unknown-barcode.png"");

Console.WriteLine($""Detected: {results.First().BarcodeType}"");
// Could be: QRCode, Code128, EAN13, DataMatrix, PDF417, etc.";
            Console.WriteLine(code);
        }

        /// <summary>
        /// Cross-platform deployment - NOT possible with MessagingToolkit
        /// </summary>
        public void DemonstrateCrossPlatform()
        {
            Console.WriteLine("\n=== NEW: Cross-Platform ===\n");

            Console.WriteLine("With IronBarcode, deploy to:");
            Console.WriteLine("  - Windows (x64, x86, ARM)");
            Console.WriteLine("  - Linux (x64, ARM)");
            Console.WriteLine("  - macOS (Intel, Apple Silicon)");
            Console.WriteLine("  - Docker containers");
            Console.WriteLine("  - Azure App Service");
            Console.WriteLine("  - AWS Lambda");
            Console.WriteLine();
            Console.WriteLine("MessagingToolkit: Windows .NET Framework ONLY");
        }

        /// <summary>
        /// Modern .NET features - NOT possible with MessagingToolkit
        /// </summary>
        public void DemonstrateModernNet()
        {
            Console.WriteLine("\n=== NEW: Modern .NET Features ===\n");

            var code = @"
// Works with async patterns
public async Task<string> ProcessBarcodeAsync(IFormFile upload)
{
    await using var stream = upload.OpenReadStream();
    var results = BarcodeReader.Read(stream);
    return results.FirstOrDefault()?.Value ?? ""No barcode"";
}

// Works with records (C# 9+)
public record BarcodeData(string Value, string Type, int Page);

// Works with pattern matching (C# 8+)
var description = barcode.BarcodeType switch
{
    BarcodeEncoding.QRCode => ""QR Code"",
    BarcodeEncoding.Code128 => ""Code 128"",
    _ => ""Other format""
};";
            Console.WriteLine(code);
        }

        /// <summary>
        /// Actual working example with IronBarcode
        /// </summary>
        public void RunLiveExample()
        {
            Console.WriteLine("\n=== LIVE EXAMPLE ===\n");

            // Generate a barcode
            var qrCode = BarcodeWriter.CreateBarcode(
                "Migrated from MessagingToolkit to IronBarcode",
                BarcodeEncoding.QRCode);

            var bytes = qrCode.ToPngBinaryData();
            Console.WriteLine($"Generated QR code: {bytes.Length} bytes");

            // Read it back using stream
            using var stream = new MemoryStream(bytes);
            var results = BarcodeReader.Read(stream);

            Console.WriteLine($"Read back: {results.First().Value}");
            Console.WriteLine($"Format: {results.First().BarcodeType}");
        }
    }

    #endregion

    #region Migration Steps

    /// <summary>
    /// Step-by-step migration guide
    /// </summary>
    public class MigrationSteps
    {
        public void ShowSteps()
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("MIGRATION STEPS");
            Console.WriteLine("========================================\n");

            Console.WriteLine("Step 1: Install IronBarcode");
            Console.WriteLine("  dotnet add package IronBarcode");
            Console.WriteLine();

            Console.WriteLine("Step 2: Update using statements");
            Console.WriteLine("  Remove: using MessagingToolkit.Barcode;");
            Console.WriteLine("  Add:    using IronBarcode;");
            Console.WriteLine();

            Console.WriteLine("Step 3: Replace decoder code");
            Console.WriteLine("  Old: new BarcodeDecoder().Decode(bitmap)");
            Console.WriteLine("  New: BarcodeReader.Read(path)");
            Console.WriteLine();

            Console.WriteLine("Step 4: Replace encoder code");
            Console.WriteLine("  Old: new BarcodeEncoder().Encode(data)");
            Console.WriteLine("  New: BarcodeWriter.CreateBarcode(data, encoding)");
            Console.WriteLine();

            Console.WriteLine("Step 5: Update result handling");
            Console.WriteLine("  Old: result.Text");
            Console.WriteLine("  New: results.First().Value");
            Console.WriteLine();

            Console.WriteLine("Step 6: Remove MessagingToolkit package");
            Console.WriteLine("  dotnet remove package MessagingToolkit.Barcode");
            Console.WriteLine();

            Console.WriteLine("Step 7: (Optional) Update target framework");
            Console.WriteLine("  Change from net472 to net8.0");
        }
    }

    #endregion

    /// <summary>
    /// Entry point demonstrating migration
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("MessagingToolkit to IronBarcode Migration");
            Console.WriteLine("========================================\n");

            // Show API mapping
            ApiMappingReference.ShowApiMapping();

            // Show reading migration
            var reading = new ReadingMigration();
            reading.ShowOldCode();
            reading.ShowNewCode();

            // Show generation migration
            var generation = new GenerationMigration();
            generation.ShowOldCode();
            generation.ShowNewCode();

            // Show multiple barcodes handling
            var multiple = new MultipleBarcodesMigration();
            multiple.ShowOldPattern();
            multiple.ShowNewPattern();

            // Show stream processing
            var streams = new StreamProcessingMigration();
            streams.ShowOldPattern();
            streams.ShowNewPattern();

            // Show new capabilities
            var newFeatures = new NewCapabilities();
            newFeatures.DemonstratePdfSupport();
            newFeatures.DemonstrateAutoDetection();
            newFeatures.DemonstrateCrossPlatform();
            newFeatures.DemonstrateModernNet();
            newFeatures.RunLiveExample();

            // Show migration steps
            var steps = new MigrationSteps();
            steps.ShowSteps();

            Console.WriteLine("\n========================================");
            Console.WriteLine("Migration complete - enjoy modern .NET!");
            Console.WriteLine("========================================");
        }
    }
}
