// Install: dotnet add package IronBarcode

using System;
using IronBarCode;

namespace QRCodeExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Basic QR code generation
            Console.WriteLine("Generating basic QR code...");
            var basicQR = BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode);
            basicQR.SaveAsPng("basic-qr.png");
            Console.WriteLine("✓ Basic QR code saved: basic-qr.png");

            // QR code with embedded logo
            // NOTE: Replace "logo.png" with your actual logo file path
            Console.WriteLine("\nGenerating QR code with logo...");
            try
            {
                var logoQR = QRCodeWriter.CreateQrCodeWithLogo(
                    "https://ironsoftware.com/csharp/barcode",
                    new QRCodeLogo("logo.png")
                );
                logoQR.SaveAsPng("logo-qr.png");
                Console.WriteLine("✓ Logo QR code saved: logo-qr.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Logo QR skipped (no logo.png found): {ex.Message}");
            }

            // Custom-sized QR code with margins
            Console.WriteLine("\nGenerating custom-sized QR code...");
            var customQR = BarcodeWriter.CreateBarcode("https://example.com", BarcodeEncoding.QRCode);
            customQR.ResizeTo(400, 400);
            customQR.SetMargins(15);
            customQR.SaveAsPng("custom-qr.png");
            Console.WriteLine("✓ Custom-sized QR code saved: custom-qr.png (400x400px with 15px margins)");

            // High error correction QR code
            Console.WriteLine("\nGenerating high error correction QR code...");
            var options = new QRCodeWriterOptions
            {
                ErrorCorrectionLevel = ErrorCorrectionLevel.High
            };
            var highEccQR = QRCodeWriter.CreateQrCode("Important data that needs protection", options);
            highEccQR.SaveAsPng("high-ecc-qr.png");
            Console.WriteLine("✓ High ECC QR code saved: high-ecc-qr.png (30% damage recovery)");

            // vCard contact QR code
            Console.WriteLine("\nGenerating vCard contact QR code...");
            string vCard = @"BEGIN:VCARD
VERSION:3.0
FN:Jacob Mellor
ORG:Iron Software
TEL:+1-555-0123
EMAIL:jacob@ironsoftware.com
URL:https://ironsoftware.com
END:VCARD";
            var contactQR = BarcodeWriter.CreateBarcode(vCard, BarcodeEncoding.QRCode);
            contactQR.SaveAsPng("contact-qr.png");
            Console.WriteLine("✓ Contact QR code saved: contact-qr.png");

            // WiFi network QR code
            Console.WriteLine("\nGenerating WiFi network QR code...");
            string wifiCredentials = "WIFI:S:GuestNetwork;T:WPA;P:SecurePassword123;;";
            var wifiQR = BarcodeWriter.CreateBarcode(wifiCredentials, BarcodeEncoding.QRCode);
            wifiQR.SaveAsPng("wifi-qr.png");
            Console.WriteLine("✓ WiFi QR code saved: wifi-qr.png");

            // URL QR code
            Console.WriteLine("\nGenerating URL QR code...");
            var urlQR = BarcodeWriter.CreateBarcode("https://github.com/iron-software", BarcodeEncoding.QRCode);
            urlQR.SaveAsPng("url-qr.png");
            Console.WriteLine("✓ URL QR code saved: url-qr.png");

            // Plain text QR code
            Console.WriteLine("\nGenerating plain text QR code...");
            var textQR = BarcodeWriter.CreateBarcode("Visit our booth at Hall A, Stand 42", BarcodeEncoding.QRCode);
            textQR.SaveAsPng("text-qr.png");
            Console.WriteLine("✓ Text QR code saved: text-qr.png");

            Console.WriteLine("\n========================================");
            Console.WriteLine("All QR codes generated successfully!");
            Console.WriteLine("========================================");
            Console.WriteLine("\nQR Codes created:");
            Console.WriteLine("  • basic-qr.png - Simple URL QR code");
            Console.WriteLine("  • logo-qr.png - QR code with embedded logo");
            Console.WriteLine("  • custom-qr.png - Custom sized with margins");
            Console.WriteLine("  • high-ecc-qr.png - High error correction");
            Console.WriteLine("  • contact-qr.png - vCard contact info");
            Console.WriteLine("  • wifi-qr.png - WiFi credentials");
            Console.WriteLine("  • url-qr.png - GitHub URL");
            Console.WriteLine("  • text-qr.png - Plain text message");
        }
    }
}
