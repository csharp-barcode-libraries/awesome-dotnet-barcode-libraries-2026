/**
 * QRCoder QR-Only Example
 *
 * This example demonstrates QRCoder's capabilities within its niche
 * (QR code generation) and compares to IronBarcode's broader support.
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * Article: QRCoder vs IronBarcode Comparison 2026
 */

using System;
using System.IO;

// QRCoder package
// Install: dotnet add package QRCoder
using QRCoder;

// IronBarcode package
// Install: dotnet add package IronBarcode
using IronBarCode;

namespace QRCoderComparison
{
    /// <summary>
    /// Demonstrates QRCoder's QR code generation capabilities and
    /// its limitation of supporting only QR codes.
    /// </summary>
    public class QROnlyExample
    {
        /// <summary>
        /// Shows QRCoder's QR code generation - where it excels.
        /// </summary>
        public void QRCoderBasicGeneration()
        {
            Console.WriteLine("=== QRCoder: Basic QR Code Generation ===\n");

            var qrGenerator = new QRCodeGenerator();

            // Basic URL QR code
            var urlQRData = qrGenerator.CreateQrCode(
                "https://ironsoftware.com",
                QRCodeGenerator.ECCLevel.M
            );
            var urlQR = new PngByteQRCode(urlQRData);
            File.WriteAllBytes("qrcoder-url.png", urlQR.GetGraphic(10));
            Console.WriteLine("Generated: qrcoder-url.png (URL)");

            // Text QR code
            var textQRData = qrGenerator.CreateQrCode(
                "Hello World from QRCoder!",
                QRCodeGenerator.ECCLevel.H
            );
            var textQR = new PngByteQRCode(textQRData);
            File.WriteAllBytes("qrcoder-text.png", textQR.GetGraphic(10));
            Console.WriteLine("Generated: qrcoder-text.png (Text)");

            Console.WriteLine("\nQRCoder handles basic QR generation well.\n");
        }

        /// <summary>
        /// Shows QRCoder's PayloadGenerator feature.
        /// </summary>
        public void QRCoderPayloadGenerators()
        {
            Console.WriteLine("=== QRCoder: PayloadGenerator Feature ===\n");

            var qrGenerator = new QRCodeGenerator();

            // WiFi credentials
            Console.WriteLine("WiFi QR Code:");
            var wifiPayload = new PayloadGenerator.WiFi(
                ssid: "MyWiFiNetwork",
                password: "SecurePassword123",
                authenticationMode: PayloadGenerator.WiFi.Authentication.WPA
            );
            var wifiQR = qrGenerator.CreateQrCode(wifiPayload.ToString(), QRCodeGenerator.ECCLevel.M);
            var wifiPng = new PngByteQRCode(wifiQR);
            File.WriteAllBytes("qrcoder-wifi.png", wifiPng.GetGraphic(10));
            Console.WriteLine($"  Payload: {wifiPayload}");
            Console.WriteLine("  Generated: qrcoder-wifi.png\n");

            // URL shortcut
            Console.WriteLine("URL QR Code:");
            var urlPayload = new PayloadGenerator.Url("https://ironsoftware.com/csharp/barcode/");
            var urlQR = qrGenerator.CreateQrCode(urlPayload.ToString(), QRCodeGenerator.ECCLevel.M);
            var urlPng = new PngByteQRCode(urlQR);
            File.WriteAllBytes("qrcoder-payload-url.png", urlPng.GetGraphic(10));
            Console.WriteLine($"  Payload: {urlPayload}");
            Console.WriteLine("  Generated: qrcoder-payload-url.png\n");

            // SMS
            Console.WriteLine("SMS QR Code:");
            var smsPayload = new PayloadGenerator.SMS("1234567890", "Hello from QRCoder!");
            var smsQR = qrGenerator.CreateQrCode(smsPayload.ToString(), QRCodeGenerator.ECCLevel.M);
            var smsPng = new PngByteQRCode(smsQR);
            File.WriteAllBytes("qrcoder-sms.png", smsPng.GetGraphic(10));
            Console.WriteLine($"  Payload: {smsPayload}");
            Console.WriteLine("  Generated: qrcoder-sms.png\n");

            // Phone number
            Console.WriteLine("Phone QR Code:");
            var phonePayload = new PayloadGenerator.PhoneNumber("1234567890");
            var phoneQR = qrGenerator.CreateQrCode(phonePayload.ToString(), QRCodeGenerator.ECCLevel.M);
            var phonePng = new PngByteQRCode(phoneQR);
            File.WriteAllBytes("qrcoder-phone.png", phonePng.GetGraphic(10));
            Console.WriteLine($"  Payload: {phonePayload}");
            Console.WriteLine("  Generated: qrcoder-phone.png\n");

            Console.WriteLine("PayloadGenerator is a genuinely useful QRCoder feature.\n");
        }

        /// <summary>
        /// Shows QRCoder's multiple output formats.
        /// </summary>
        public void QRCoderOutputFormats()
        {
            Console.WriteLine("=== QRCoder: Output Format Options ===\n");

            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode("OUTPUT-FORMAT-TEST", QRCodeGenerator.ECCLevel.M);

            // PNG output
            var pngQR = new PngByteQRCode(qrData);
            File.WriteAllBytes("qrcoder-output.png", pngQR.GetGraphic(10));
            Console.WriteLine("PNG: qrcoder-output.png");

            // BMP output
            var bmpQR = new BitmapByteQRCode(qrData);
            File.WriteAllBytes("qrcoder-output.bmp", bmpQR.GetGraphic(10));
            Console.WriteLine("BMP: qrcoder-output.bmp");

            // SVG output
            var svgQR = new SvgQRCode(qrData);
            File.WriteAllText("qrcoder-output.svg", svgQR.GetGraphic(10));
            Console.WriteLine("SVG: qrcoder-output.svg");

            // ASCII art output
            var asciiQR = new AsciiQRCode(qrData);
            string asciiArt = asciiQR.GetGraphic(1);
            Console.WriteLine("\nASCII Art preview:");
            // Print first few lines
            var lines = asciiArt.Split('\n');
            for (int i = 0; i < Math.Min(5, lines.Length); i++)
            {
                Console.WriteLine("  " + lines[i]);
            }
            Console.WriteLine("  ...(truncated)");

            // Base64 output
            var base64QR = new Base64QRCode(qrData);
            string base64 = base64QR.GetGraphic(10);
            Console.WriteLine($"\nBase64: {base64.Substring(0, 50)}...");

            Console.WriteLine("\nMultiple output formats available in QRCoder.\n");
        }

        /// <summary>
        /// Shows IronBarcode generating QR codes plus other formats.
        /// </summary>
        public void IronBarcodeQRPlusMore()
        {
            Console.WriteLine("=== IronBarcode: QR Codes Plus All Other Formats ===\n");

            // IronBarCode.License.LicenseKey = "YOUR-KEY-HERE";

            // QR Code (same as QRCoder capability)
            Console.WriteLine("QR Code (same as QRCoder):");
            BarcodeWriter.CreateBarcode("https://ironsoftware.com", BarcodeEncoding.QRCode)
                .ResizeTo(200, 200)
                .SaveAsPng("iron-qr.png");
            Console.WriteLine("  Generated: iron-qr.png\n");

            // Code128 (NOT available in QRCoder)
            Console.WriteLine("Code128 (NOT available in QRCoder):");
            BarcodeWriter.CreateBarcode("SHIPPING-12345", BarcodeEncoding.Code128)
                .SaveAsPng("iron-code128.png");
            Console.WriteLine("  Generated: iron-code128.png\n");

            // EAN-13 (NOT available in QRCoder)
            Console.WriteLine("EAN-13 (NOT available in QRCoder):");
            BarcodeWriter.CreateBarcode("5901234123457", BarcodeEncoding.EAN13)
                .SaveAsPng("iron-ean13.png");
            Console.WriteLine("  Generated: iron-ean13.png\n");

            // DataMatrix (NOT available in QRCoder)
            Console.WriteLine("DataMatrix (NOT available in QRCoder):");
            BarcodeWriter.CreateBarcode("PHARMA-LOT-12345", BarcodeEncoding.DataMatrix)
                .SaveAsPng("iron-datamatrix.png");
            Console.WriteLine("  Generated: iron-datamatrix.png\n");

            // PDF417 (NOT available in QRCoder)
            Console.WriteLine("PDF417 (NOT available in QRCoder):");
            BarcodeWriter.CreateBarcode("ID-DOCUMENT-DATA", BarcodeEncoding.PDF417)
                .SaveAsPng("iron-pdf417.png");
            Console.WriteLine("  Generated: iron-pdf417.png\n");

            // Aztec (NOT available in QRCoder)
            Console.WriteLine("Aztec (NOT available in QRCoder):");
            BarcodeWriter.CreateBarcode("BOARDING-PASS-DATA", BarcodeEncoding.Aztec)
                .SaveAsPng("iron-aztec.png");
            Console.WriteLine("  Generated: iron-aztec.png\n");

            Console.WriteLine("IronBarcode: Same API for ALL barcode formats.\n");
        }

        /// <summary>
        /// Demonstrates that QRCoder cannot read QR codes.
        /// </summary>
        public void ReadingComparison()
        {
            Console.WriteLine("=== Reading QR Codes Comparison ===\n");

            Console.WriteLine("QRCoder:");
            Console.WriteLine("  QRCoder is generation-only.");
            Console.WriteLine("  There is NO reading/decoding API.");
            Console.WriteLine("  The following methods DO NOT EXIST:");
            Console.WriteLine("    - qrGenerator.Decode(imagePath)");
            Console.WriteLine("    - QRCodeReader.Read(imagePath)");
            Console.WriteLine("    - qrData.DecodeFromImage()");
            Console.WriteLine("\n  To read QR codes with QRCoder, you need a separate library.\n");

            Console.WriteLine("IronBarcode:");
            // Generate a QR to read
            BarcodeWriter.CreateBarcode("READ-THIS-QR", BarcodeEncoding.QRCode)
                .SaveAsPng("iron-to-read.png");

            // Read it back
            var results = BarcodeReader.Read("iron-to-read.png");
            Console.WriteLine("  var results = BarcodeReader.Read(\"iron-to-read.png\");");
            foreach (var result in results)
            {
                Console.WriteLine($"  Read value: {result.Text}");
                Console.WriteLine($"  Barcode type: {result.BarcodeType}");
            }

            Console.WriteLine("\n  Reading is built into IronBarcode - no extra library needed.\n");
        }

        /// <summary>
        /// Shows WiFi QR code generation in both libraries.
        /// </summary>
        public void WiFiQRComparison()
        {
            Console.WriteLine("=== WiFi QR Code Comparison ===\n");

            string ssid = "TestNetwork";
            string password = "TestPassword123";

            // QRCoder with PayloadGenerator
            Console.WriteLine("QRCoder (with PayloadGenerator):");
            var qrGenerator = new QRCodeGenerator();
            var wifiPayload = new PayloadGenerator.WiFi(ssid, password, PayloadGenerator.WiFi.Authentication.WPA);
            var qrData = qrGenerator.CreateQrCode(wifiPayload.ToString(), QRCodeGenerator.ECCLevel.M);
            var pngQR = new PngByteQRCode(qrData);
            File.WriteAllBytes("compare-wifi-qrcoder.png", pngQR.GetGraphic(10));
            Console.WriteLine($"  Payload: {wifiPayload}");
            Console.WriteLine("  Generated: compare-wifi-qrcoder.png\n");

            // IronBarcode (using standard WiFi format)
            Console.WriteLine("IronBarcode (standard WiFi format):");
            string wifiData = $"WIFI:T:WPA;S:{ssid};P:{password};;";
            BarcodeWriter.CreateBarcode(wifiData, BarcodeEncoding.QRCode)
                .SaveAsPng("compare-wifi-iron.png");
            Console.WriteLine($"  Payload: {wifiData}");
            Console.WriteLine("  Generated: compare-wifi-iron.png\n");

            Console.WriteLine("Both produce scannable WiFi QR codes.");
            Console.WriteLine("QRCoder's PayloadGenerator is convenient but not required.\n");
        }

        /// <summary>
        /// Summary of QR-only limitation.
        /// </summary>
        public void LimitationSummary()
        {
            Console.WriteLine("=== QRCoder Limitation Summary ===\n");

            Console.WriteLine("What QRCoder CAN do:");
            Console.WriteLine("  + Generate QR codes (excellent)");
            Console.WriteLine("  + Multiple output formats (PNG, SVG, ASCII, etc.)");
            Console.WriteLine("  + PayloadGenerator helpers (WiFi, URL, SMS, etc.)");
            Console.WriteLine("  + Zero dependencies (pure C#)");
            Console.WriteLine("  + Error correction control (L, M, Q, H)");
            Console.WriteLine("  + Color customization");
            Console.WriteLine("  + Logo embedding\n");

            Console.WriteLine("What QRCoder CANNOT do:");
            Console.WriteLine("  - Generate Code128, Code39, or any 1D barcode");
            Console.WriteLine("  - Generate DataMatrix, PDF417, Aztec, or other 2D formats");
            Console.WriteLine("  - Read/decode ANY barcode (including QR)");
            Console.WriteLine("  - Extract barcodes from PDFs");
            Console.WriteLine("  - Process mixed-format documents\n");

            Console.WriteLine("Bottom line:");
            Console.WriteLine("  QRCoder is excellent if you ONLY need QR code generation.");
            Console.WriteLine("  IronBarcode is better if you need flexibility or reading.\n");
        }

        public static void Main(string[] args)
        {
            var example = new QROnlyExample();

            example.QRCoderBasicGeneration();
            example.QRCoderPayloadGenerators();
            example.QRCoderOutputFormats();
            example.IronBarcodeQRPlusMore();
            example.ReadingComparison();
            example.WiFiQRComparison();
            example.LimitationSummary();

            Console.WriteLine("=== Summary ===");
            Console.WriteLine("QRCoder: Excellent QR-only generation library");
            Console.WriteLine("IronBarcode: Complete barcode solution (all formats + reading)");
        }
    }
}
