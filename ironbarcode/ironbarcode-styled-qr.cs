/**
 * IronBarcode Styled QR Code Example
 *
 * This example demonstrates QR code customization for branding and
 * visual design. Create professional-looking QR codes with custom
 * colors, embedded logos, and various output formats.
 *
 * Key Features Shown:
 * - Custom foreground/background colors
 * - Logo/brand image embedding
 * - Size and margin control
 * - Error correction levels
 * - Multiple output formats (PNG, SVG, PDF)
 *
 * NuGet Package: IronBarcode version 2024.x+
 * Documentation: https://ironsoftware.com/csharp/barcode/
 */

// Install: dotnet add package IronBarcode
using IronBarCode;
using System;
using System.Drawing;

public class StyledQrExample
{
    public static void Main()
    {
        // Basic colored QR code
        CreateColoredQr();

        // QR with embedded logo
        CreateBrandedQr();

        // Control size and margins
        CreateSizedQr();

        // Error correction levels for logos
        ErrorCorrectionExample();

        // Multiple output formats
        MultiFormatOutput();

        // Create complete marketing QR set
        MarketingQrSet();
    }

    static void CreateColoredQr()
    {
        // Create QR with custom colors
        var qr = QRCodeWriter.CreateQrCode("https://ironsoftware.com");

        // Navy blue on light background
        qr.ChangeBarCodeColor(Color.Navy);
        qr.ChangeBackgroundColor(Color.WhiteSmoke);
        qr.SaveAsPng("qr-navy.png");

        // Red brand color
        var redQr = QRCodeWriter.CreateQrCode("https://example.com");
        redQr.ChangeBarCodeColor(Color.FromArgb(220, 38, 38)); // Tailwind red-600
        redQr.SaveAsPng("qr-red.png");

        // Dark mode
        var darkQr = QRCodeWriter.CreateQrCode("https://example.com");
        darkQr.ChangeBarCodeColor(Color.White);
        darkQr.ChangeBackgroundColor(Color.FromArgb(30, 30, 30));
        darkQr.SaveAsPng("qr-dark.png");

        Console.WriteLine("Created colored QR codes");
    }

    static void CreateBrandedQr()
    {
        // Create QR code at higher error correction for logo tolerance
        var qr = QRCodeWriter.CreateQrCode(
            "https://ironsoftware.com",
            500,
            QRCodeWriter.QrErrorCorrectionLevel.Highest
        );

        // Add centered logo (logo can obscure up to 30% with highest EC)
        qr.AddBrandLogo("company-logo.png");
        qr.SaveAsPng("qr-branded.png");

        // Alternative: Add logo with custom positioning
        var qr2 = QRCodeWriter.CreateQrCode("https://example.com", 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);
        qr2.AddBrandLogo("logo.png", logoPercentage: 20); // Logo takes 20% of QR area
        qr2.SaveAsPng("qr-branded-small-logo.png");

        Console.WriteLine("Created branded QR codes with logos");
    }

    static void CreateSizedQr()
    {
        // Small QR for business cards
        var smallQr = QRCodeWriter.CreateQrCode("https://ironsoftware.com", 200);
        smallQr.SetMargins(5);
        smallQr.SaveAsPng("qr-small.png");

        // Medium for print materials
        var mediumQr = QRCodeWriter.CreateQrCode("https://ironsoftware.com", 500);
        mediumQr.SetMargins(20);
        mediumQr.SaveAsPng("qr-medium.png");

        // Large for banners/posters
        var largeQr = QRCodeWriter.CreateQrCode("https://ironsoftware.com", 1000);
        largeQr.SetMargins(40);
        largeQr.SaveAsPng("qr-large.png");

        // Specific dimensions
        var customQr = QRCodeWriter.CreateQrCode("https://ironsoftware.com");
        customQr.ResizeTo(300, 300);
        customQr.SaveAsPng("qr-300x300.png");

        Console.WriteLine("Created QR codes in various sizes");
    }

    static void ErrorCorrectionExample()
    {
        string data = "https://ironsoftware.com/csharp/barcode/";

        // Low - fastest scan, no logo tolerance
        var lowEc = QRCodeWriter.CreateQrCode(data, 300, QRCodeWriter.QrErrorCorrectionLevel.Low);
        lowEc.SaveAsPng("qr-ec-low.png");

        // Medium - balanced (default)
        var medEc = QRCodeWriter.CreateQrCode(data, 300, QRCodeWriter.QrErrorCorrectionLevel.Medium);
        medEc.SaveAsPng("qr-ec-medium.png");

        // Quartile - good damage tolerance
        var quartileEc = QRCodeWriter.CreateQrCode(data, 300, QRCodeWriter.QrErrorCorrectionLevel.Quartile);
        quartileEc.SaveAsPng("qr-ec-quartile.png");

        // Highest - best for logos and damage (up to 30% can be obscured)
        var highEc = QRCodeWriter.CreateQrCode(data, 300, QRCodeWriter.QrErrorCorrectionLevel.Highest);
        highEc.AddBrandLogo("logo.png");
        highEc.SaveAsPng("qr-ec-highest.png");

        Console.WriteLine("Created QR codes with different error correction levels");
        Console.WriteLine("  Low: Fastest scan, smallest size, no damage tolerance");
        Console.WriteLine("  Medium: Default balance");
        Console.WriteLine("  Quartile: ~25% damage tolerance");
        Console.WriteLine("  Highest: ~30% damage tolerance (best for logos)");
    }

    static void MultiFormatOutput()
    {
        var qr = QRCodeWriter.CreateQrCode("https://ironsoftware.com");
        qr.ChangeBarCodeColor(Color.DarkSlateBlue);

        // Raster formats
        qr.SaveAsPng("qr-output.png");
        qr.SaveAsJpeg("qr-output.jpg");
        qr.SaveAsBmp("qr-output.bmp");
        qr.SaveAsGif("qr-output.gif");
        qr.SaveAsTiff("qr-output.tiff");

        // Vector format (infinite scaling)
        qr.SaveAsSvg("qr-output.svg");

        // PDF for print
        qr.SaveAsPdf("qr-output.pdf");

        // HTML for web embedding
        string htmlTag = qr.ToHtmlTag();
        Console.WriteLine($"HTML tag: {htmlTag.Substring(0, 60)}...");

        // Data URL for inline CSS/HTML
        string dataUrl = qr.ToDataUrl();
        Console.WriteLine($"Data URL: {dataUrl.Substring(0, 50)}...");

        Console.WriteLine("Exported QR to 9 different formats");
    }

    static void MarketingQrSet()
    {
        string websiteUrl = "https://ironsoftware.com/csharp/barcode/?utm_source=qr&utm_campaign=2026";

        // Brand colors
        var brandPrimary = Color.FromArgb(0, 51, 102);    // Navy
        var brandAccent = Color.FromArgb(255, 153, 0);    // Orange

        // Standard white background
        var standardQr = QRCodeWriter.CreateQrCode(websiteUrl, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);
        standardQr.ChangeBarCodeColor(brandPrimary);
        standardQr.AddBrandLogo("iron-logo.png");
        standardQr.SaveAsPng("marketing-qr-standard.png");
        standardQr.SaveAsSvg("marketing-qr-standard.svg");

        // Inverted for dark backgrounds
        var invertedQr = QRCodeWriter.CreateQrCode(websiteUrl, 500, QRCodeWriter.QrErrorCorrectionLevel.Highest);
        invertedQr.ChangeBarCodeColor(Color.White);
        invertedQr.ChangeBackgroundColor(Color.Transparent);
        invertedQr.SaveAsPng("marketing-qr-inverted.png");

        // Accent color variant
        var accentQr = QRCodeWriter.CreateQrCode(websiteUrl, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium);
        accentQr.ChangeBarCodeColor(brandAccent);
        accentQr.SaveAsPng("marketing-qr-accent.png");

        Console.WriteLine("Created complete marketing QR code set:");
        Console.WriteLine("  - Standard (PNG + SVG)");
        Console.WriteLine("  - Inverted for dark backgrounds");
        Console.WriteLine("  - Accent color variant");
    }
}
