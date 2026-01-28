/**
 * IronBarcode One-Line API Example
 *
 * This example demonstrates IronBarcode's "one line to read, one line to write"
 * philosophy. Generate any barcode format with a single method chain.
 * Read any barcode without specifying its type.
 *
 * Key Features Shown:
 * - Single-line barcode generation
 * - Single-line barcode reading
 * - Fluent API chaining
 * - Multiple format support
 *
 * NuGet Package: IronBarcode version 2024.x+
 * Documentation: https://ironsoftware.com/csharp/barcode/
 */

// Install: dotnet add package IronBarcode
using IronBarCode;
using System;
using System.Linq;

public class OneLineGenerationExample
{
    public static void Main()
    {
        // ONE LINE - Generate a Code128 barcode and save to file
        BarcodeWriter.CreateBarcode("PROD-2026-001", BarcodeEncoding.Code128).SaveAsPng("code128.png");

        // ONE LINE - Read any barcode (automatic format detection)
        string value = BarcodeReader.Read("code128.png").First().Value;
        Console.WriteLine($"Read value: {value}");

        // Generate multiple formats with the same simple pattern
        GenerateMultipleFormats();

        // Demonstrate fluent API chaining
        FluentApiExample();

        // Show read-back verification pattern
        RoundTripExample();
    }

    static void GenerateMultipleFormats()
    {
        // Code128 - common for shipping labels
        BarcodeWriter.CreateBarcode("SHIP-12345", BarcodeEncoding.Code128).SaveAsPng("shipping.png");

        // QR Code - URLs, contact info
        BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode).SaveAsPng("website-qr.png");

        // EAN-13 - retail products
        BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13).SaveAsPng("product-ean.png");

        // PDF417 - ID cards, shipping documents
        BarcodeWriter.CreateBarcode("DL:SMITH,JOHN,1990-05-15", BarcodeEncoding.PDF417).SaveAsPng("id-pdf417.png");

        // DataMatrix - small parts, electronics
        BarcodeWriter.CreateBarcode("SN:ABC123XYZ", BarcodeEncoding.DataMatrix).SaveAsPng("serial-dm.png");

        // UPC-A - US retail
        BarcodeWriter.CreateBarcode("012345678905", BarcodeEncoding.UPCA).SaveAsPng("retail-upc.png");

        Console.WriteLine("Generated 6 barcode formats");
    }

    static void FluentApiExample()
    {
        // Chain multiple operations in one statement
        BarcodeWriter.CreateBarcode("FLUENT-API-DEMO", BarcodeEncoding.Code128)
            .ResizeTo(400, 120)
            .SetMargins(10)
            .SaveAsPng("fluent-barcode.png");

        // QR with styling chain
        QRCodeWriter.CreateQrCode("https://ironsoftware.com/csharp/barcode/")
            .ChangeBarCodeColor(System.Drawing.Color.DarkBlue)
            .ChangeBackgroundColor(System.Drawing.Color.WhiteSmoke)
            .SaveAsPng("styled-qr.png");

        Console.WriteLine("Created barcodes with fluent API chaining");
    }

    static void RoundTripExample()
    {
        string originalData = "VERIFY-THIS-DATA-2026";

        // Generate
        BarcodeWriter.CreateBarcode(originalData, BarcodeEncoding.Code128).SaveAsPng("verify.png");

        // Read back (automatic format detection)
        var result = BarcodeReader.Read("verify.png").First();

        // Verify
        bool matches = result.Value == originalData;
        Console.WriteLine($"Round-trip verification: {(matches ? "PASSED" : "FAILED")}");
        Console.WriteLine($"Original: {originalData}");
        Console.WriteLine($"Read back: {result.Value}");
        Console.WriteLine($"Detected format: {result.Format}");
    }
}
