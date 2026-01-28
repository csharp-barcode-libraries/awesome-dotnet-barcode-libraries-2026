/**
 * IronBarcode PDF Batch Processing Example
 *
 * This example demonstrates IronBarcode's native PDF support for
 * document workflow automation. Read barcodes from multi-page PDFs,
 * process folders of documents, and output structured results.
 *
 * Key Features Shown:
 * - Native PDF barcode reading (no external dependencies)
 * - Specific page selection
 * - Batch folder processing
 * - CSV/JSON output generation
 * - Progress reporting for large batches
 *
 * NuGet Package: IronBarcode version 2024.x+
 * Documentation: https://ironsoftware.com/csharp/barcode/
 */

// Install: dotnet add package IronBarcode
using IronBarCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class PdfBatchExample
{
    public static void Main()
    {
        // Read all pages from a PDF
        ReadEntirePdf();

        // Read specific pages (memory efficient)
        ReadSpecificPages();

        // Process folder of PDFs
        ProcessPdfFolder();

        // Output to CSV
        ExportToCsv();

        // Output to JSON
        ExportToJson();

        // Progress reporting for large batches
        BatchWithProgress();
    }

    static void ReadEntirePdf()
    {
        // Native PDF support - all pages processed automatically
        var results = BarcodeReader.Read("invoices.pdf");

        Console.WriteLine($"Found {results.Count()} barcodes in PDF:");
        foreach (var barcode in results)
        {
            Console.WriteLine($"  Page {barcode.PageNumber}: {barcode.Value} ({barcode.Format})");
        }
    }

    static void ReadSpecificPages()
    {
        // For large documents, specify only the pages you need
        var results = BarcodeReader.Read("large-document.pdf", pages: new[] { 1, 5, 10 });

        Console.WriteLine("Barcodes from pages 1, 5, and 10:");
        foreach (var barcode in results)
        {
            Console.WriteLine($"  Page {barcode.PageNumber}: {barcode.Value}");
        }

        // Page range for consecutive pages
        var rangeResults = BarcodeReader.Read("document.pdf", pageRange: 1..10);
        Console.WriteLine($"Pages 1-10 contained {rangeResults.Count()} barcodes");
    }

    static void ProcessPdfFolder()
    {
        string inputFolder = "invoices";
        var allResults = new Dictionary<string, List<string>>();

        foreach (var pdfFile in Directory.GetFiles(inputFolder, "*.pdf"))
        {
            var results = BarcodeReader.Read(pdfFile);
            var barcodeValues = results.Select(b => b.Value).ToList();

            allResults[Path.GetFileName(pdfFile)] = barcodeValues;

            Console.WriteLine($"{Path.GetFileName(pdfFile)}: {barcodeValues.Count} barcodes");
        }

        Console.WriteLine($"\nProcessed {allResults.Count} PDF files");
        Console.WriteLine($"Total barcodes found: {allResults.Values.Sum(v => v.Count)}");
    }

    static void ExportToCsv()
    {
        string inputFolder = "documents";
        var csvLines = new List<string> { "Filename,Page,BarcodeValue,Format,Confidence" };

        foreach (var pdfFile in Directory.GetFiles(inputFolder, "*.pdf"))
        {
            var results = BarcodeReader.Read(pdfFile);
            foreach (var barcode in results)
            {
                // Escape values for CSV
                var value = barcode.Value.Contains(",")
                    ? $"\"{barcode.Value}\""
                    : barcode.Value;

                csvLines.Add($"{Path.GetFileName(pdfFile)},{barcode.PageNumber},{value},{barcode.Format},{barcode.Confidence}");
            }
        }

        File.WriteAllLines("barcode-inventory.csv", csvLines);
        Console.WriteLine($"Exported {csvLines.Count - 1} barcodes to barcode-inventory.csv");
    }

    static void ExportToJson()
    {
        string inputFolder = "documents";
        var exportData = new List<object>();

        foreach (var pdfFile in Directory.GetFiles(inputFolder, "*.pdf"))
        {
            var results = BarcodeReader.Read(pdfFile);
            foreach (var barcode in results)
            {
                exportData.Add(new
                {
                    file = Path.GetFileName(pdfFile),
                    page = barcode.PageNumber,
                    value = barcode.Value,
                    format = barcode.Format.ToString(),
                    confidence = barcode.Confidence,
                    position = new { x = barcode.X, y = barcode.Y }
                });
            }
        }

        var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("barcode-inventory.json", json);
        Console.WriteLine($"Exported {exportData.Count} barcodes to barcode-inventory.json");
    }

    static void BatchWithProgress()
    {
        var pdfFiles = Directory.GetFiles("large-batch", "*.pdf").ToArray();
        int totalFiles = pdfFiles.Length;
        int processed = 0;
        int totalBarcodes = 0;

        Console.WriteLine($"Processing {totalFiles} PDF files...\n");

        foreach (var pdfFile in pdfFiles)
        {
            var results = BarcodeReader.Read(pdfFile);
            int count = results.Count();
            totalBarcodes += count;
            processed++;

            // Progress update
            double percent = (double)processed / totalFiles * 100;
            Console.Write($"\rProgress: {processed}/{totalFiles} ({percent:F1}%) - {totalBarcodes} barcodes found");
        }

        Console.WriteLine($"\n\nBatch complete: {totalBarcodes} barcodes from {totalFiles} files");
    }
}
