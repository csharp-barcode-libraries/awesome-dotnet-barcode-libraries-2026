// Install: dotnet add package IronBarcode

using System;
using IronBarCode;
using IronSoftware.Drawing;

namespace BarcodeStyleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IronBarcode Styling Examples");
            Console.WriteLine("========================================\n");

            // Example 1: Custom colors
            Console.WriteLine("Example 1: Custom Colors");
            Console.WriteLine("----------------------------------------");
            var coloredBarcode = BarcodeWriter.CreateBarcode("COLOR-001", BarcodeEncoding.Code128);
            coloredBarcode.ChangeBarCodeColor(Color.DarkBlue);
            coloredBarcode.ChangeBackgroundColor(Color.LightGray);
            coloredBarcode.SaveAsPng("colored-barcode.png");
            Console.WriteLine("✓ Saved: colored-barcode.png");
            Console.WriteLine("  • Bars: Dark Blue");
            Console.WriteLine("  • Background: Light Gray");

            // Example 2: Margins (Quiet Zone)
            Console.WriteLine("\nExample 2: Margins (Quiet Zone)");
            Console.WriteLine("----------------------------------------");
            var marginBarcode = BarcodeWriter.CreateBarcode("MARGIN-001", BarcodeEncoding.Code128);
            marginBarcode.SetMargins(20);
            marginBarcode.SaveAsPng("margin-barcode.png");
            Console.WriteLine("✓ Saved: margin-barcode.png");
            Console.WriteLine("  • Margins: 20px on all sides");

            // Example 3: Asymmetric margins
            Console.WriteLine("\nExample 3: Asymmetric Margins");
            Console.WriteLine("----------------------------------------");
            var asymmetricBarcode = BarcodeWriter.CreateBarcode("ASYMM-001", BarcodeEncoding.Code128);
            asymmetricBarcode.SetMargins(10, 30, 10, 30); // top, right, bottom, left
            asymmetricBarcode.SaveAsPng("asymmetric-barcode.png");
            Console.WriteLine("✓ Saved: asymmetric-barcode.png");
            Console.WriteLine("  • Top/Bottom: 10px");
            Console.WriteLine("  • Left/Right: 30px");

            // Example 4: Text annotations
            Console.WriteLine("\nExample 4: Text Annotations");
            Console.WriteLine("----------------------------------------");
            var annotatedBarcode = BarcodeWriter.CreateBarcode("SKU-2024-001", BarcodeEncoding.Code128);
            annotatedBarcode.AddAnnotationTextAboveBarcode("Premium Widget");
            annotatedBarcode.AddAnnotationTextBelowBarcode("SKU-2024-001");
            annotatedBarcode.SaveAsPng("annotated-barcode.png");
            Console.WriteLine("✓ Saved: annotated-barcode.png");
            Console.WriteLine("  • Above: Premium Widget");
            Console.WriteLine("  • Below: SKU-2024-001");

            // Example 5: Custom font annotations
            Console.WriteLine("\nExample 5: Custom Font Annotations");
            Console.WriteLine("----------------------------------------");
            var customFontBarcode = BarcodeWriter.CreateBarcode("FONT-001", BarcodeEncoding.Code128);
            customFontBarcode.AddAnnotationTextAboveBarcode("Heavy Equipment", FontTypes.Arial, 16);
            customFontBarcode.AddAnnotationTextBelowBarcode("Serial: FONT-001", FontTypes.Arial, 12);
            customFontBarcode.SaveAsPng("custom-font-barcode.png");
            Console.WriteLine("✓ Saved: custom-font-barcode.png");
            Console.WriteLine("  • Above: Arial 16pt");
            Console.WriteLine("  • Below: Arial 12pt");

            // Example 6: Exact size resize
            Console.WriteLine("\nExample 6: Exact Size Resize");
            Console.WriteLine("----------------------------------------");
            var resizedBarcode = BarcodeWriter.CreateBarcode("RESIZE-001", BarcodeEncoding.Code128);
            resizedBarcode.ResizeTo(500, 200);
            resizedBarcode.SaveAsPng("resized-barcode.png");
            Console.WriteLine("✓ Saved: resized-barcode.png");
            Console.WriteLine("  • Dimensions: 500x200 pixels");

            // Example 7: Proportional scaling
            Console.WriteLine("\nExample 7: Proportional Scaling");
            Console.WriteLine("----------------------------------------");

            var scaledLarge = BarcodeWriter.CreateBarcode("SCALE-200", BarcodeEncoding.Code128);
            scaledLarge.ChangeSize(2.0); // 200%
            scaledLarge.SaveAsPng("scaled-large.png");
            Console.WriteLine("✓ Saved: scaled-large.png (200% scale)");

            var scaledSmall = BarcodeWriter.CreateBarcode("SCALE-050", BarcodeEncoding.Code128);
            scaledSmall.ChangeSize(0.5); // 50%
            scaledSmall.SaveAsPng("scaled-small.png");
            Console.WriteLine("✓ Saved: scaled-small.png (50% scale)");

            // Example 8: Fluent API - Complete styling chain
            Console.WriteLine("\nExample 8: Fluent API - Complete Styling");
            Console.WriteLine("----------------------------------------");
            var styledBarcode = BarcodeWriter.CreateBarcode("STYLE-001", BarcodeEncoding.Code128)
                .ChangeBarCodeColor(Color.Navy)
                .ChangeBackgroundColor(Color.WhiteSmoke)
                .SetMargins(15)
                .AddAnnotationTextAboveBarcode("Deluxe Product", FontTypes.Arial, 14)
                .AddAnnotationTextBelowBarcode("STYLE-001", FontTypes.Arial, 12)
                .ResizeTo(500, 200);
            styledBarcode.SaveAsPng("styled-barcode.png");
            Console.WriteLine("✓ Saved: styled-barcode.png");
            Console.WriteLine("  • Colors: Navy on WhiteSmoke");
            Console.WriteLine("  • Margins: 15px");
            Console.WriteLine("  • Annotations: Above + Below");
            Console.WriteLine("  • Size: 500x200 pixels");

            // Example 9: Styled QR code
            Console.WriteLine("\nExample 9: Styled QR Code");
            Console.WriteLine("----------------------------------------");
            var styledQR = BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode)
                .ChangeBarCodeColor(Color.DarkBlue)
                .ChangeBackgroundColor(Color.White)
                .SetMargins(10)
                .ResizeTo(300, 300);
            styledQR.SaveAsPng("styled-qr.png");
            Console.WriteLine("✓ Saved: styled-qr.png");
            Console.WriteLine("  • Colors: Dark Blue on White");
            Console.WriteLine("  • Size: 300x300 pixels");
            Console.WriteLine("  • Margins: 10px quiet zone");

            // Example 10: High-contrast for reliability
            Console.WriteLine("\nExample 10: High-Contrast for Scanning");
            Console.WriteLine("----------------------------------------");
            var highContrast = BarcodeWriter.CreateBarcode("SCAN-001", BarcodeEncoding.Code128)
                .ChangeBarCodeColor(Color.Black)
                .ChangeBackgroundColor(Color.White)
                .SetMargins(20)
                .ResizeTo(400, 150);
            highContrast.SaveAsPng("high-contrast-barcode.png");
            Console.WriteLine("✓ Saved: high-contrast-barcode.png");
            Console.WriteLine("  • Maximum contrast for reliable scanning");

            // Example 11: Product label template function
            Console.WriteLine("\nExample 11: Product Label Template");
            Console.WriteLine("----------------------------------------");

            GeneratedBarcode CreateProductLabel(string sku, string productName)
            {
                return BarcodeWriter.CreateBarcode(sku, BarcodeEncoding.Code128)
                    .ChangeBarCodeColor(Color.Black)
                    .ChangeBackgroundColor(Color.White)
                    .SetMargins(20)
                    .AddAnnotationTextAboveBarcode(productName, FontTypes.Arial, 14)
                    .AddAnnotationTextBelowBarcode(sku, FontTypes.Arial, 12)
                    .ResizeTo(400, 150);
            }

            CreateProductLabel("PROD-A001", "Widget Alpha").SaveAsPng("product-a001.png");
            CreateProductLabel("PROD-B002", "Widget Beta").SaveAsPng("product-b002.png");
            CreateProductLabel("PROD-C003", "Widget Gamma").SaveAsPng("product-c003.png");

            Console.WriteLine("✓ Saved: product-a001.png (Widget Alpha)");
            Console.WriteLine("✓ Saved: product-b002.png (Widget Beta)");
            Console.WriteLine("✓ Saved: product-c003.png (Widget Gamma)");
            Console.WriteLine("  • Consistent template applied to all products");

            // Example 12: Print-ready barcode (300 DPI)
            Console.WriteLine("\nExample 12: Print-Ready Barcode (300 DPI)");
            Console.WriteLine("----------------------------------------");
            // For 1 inch barcode at 300 DPI = 300 pixels width
            var printBarcode = BarcodeWriter.CreateBarcode("PRINT-001", BarcodeEncoding.Code128)
                .ChangeBarCodeColor(Color.Black)
                .ChangeBackgroundColor(Color.White)
                .SetMargins(30) // Larger margins for print
                .ResizeTo(900, 300); // 3 inches x 1 inch at 300 DPI
            printBarcode.SaveAsPng("print-ready-barcode.png");
            Console.WriteLine("✓ Saved: print-ready-barcode.png");
            Console.WriteLine("  • Size: 900x300 pixels (3\" x 1\" at 300 DPI)");
            Console.WriteLine("  • Ready for professional printing");

            // Example 13: Branded barcode with company colors
            Console.WriteLine("\nExample 13: Branded Barcode");
            Console.WriteLine("----------------------------------------");
            var brandedBarcode = BarcodeWriter.CreateBarcode("BRAND-001", BarcodeEncoding.Code128)
                .ChangeBarCodeColor(Color.FromArgb(0, 51, 102)) // Corporate blue
                .ChangeBackgroundColor(Color.FromArgb(240, 240, 240)) // Light gray
                .SetMargins(15)
                .AddAnnotationTextAboveBarcode("Iron Software", FontTypes.Arial, 16)
                .ResizeTo(450, 180);
            brandedBarcode.SaveAsPng("branded-barcode.png");
            Console.WriteLine("✓ Saved: branded-barcode.png");
            Console.WriteLine("  • Custom corporate colors");
            Console.WriteLine("  • Company name annotation");

            Console.WriteLine("\n========================================");
            Console.WriteLine("All styled barcodes generated successfully!");
            Console.WriteLine("========================================");
            Console.WriteLine("\nGenerated files:");
            Console.WriteLine("  • colored-barcode.png");
            Console.WriteLine("  • margin-barcode.png");
            Console.WriteLine("  • asymmetric-barcode.png");
            Console.WriteLine("  • annotated-barcode.png");
            Console.WriteLine("  • custom-font-barcode.png");
            Console.WriteLine("  • resized-barcode.png");
            Console.WriteLine("  • scaled-large.png");
            Console.WriteLine("  • scaled-small.png");
            Console.WriteLine("  • styled-barcode.png");
            Console.WriteLine("  • styled-qr.png");
            Console.WriteLine("  • high-contrast-barcode.png");
            Console.WriteLine("  • product-a001.png, product-b002.png, product-c003.png");
            Console.WriteLine("  • print-ready-barcode.png");
            Console.WriteLine("  • branded-barcode.png");
        }
    }
}
