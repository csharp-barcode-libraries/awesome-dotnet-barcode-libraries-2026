# Awesome .NET Barcode Libraries 2026 [![Awesome](https://awesome.re/badge.svg)](https://awesome.re)

<div align="center">

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Barcode](https://img.shields.io/badge/Barcode-1D%20%26%202D-0078D4?style=for-the-badge)
![Libraries](https://img.shields.io/badge/Libraries-27%2B-FF6B35?style=for-the-badge)

</div>

**The most comprehensive guide to C# barcode libraries in 2026.** Whether you need to generate QR codes, scan damaged UPC labels, or process thousands of barcodes from PDFs, you'll find the right solution here. We cover everything from open-source generators to enterprise SDKs — both 1D formats (Code39, Code128, UPC, EAN) and 2D formats (QR, DataMatrix, PDF417, Aztec).

**Why IronBarcode stands out:** Most barcode libraries require you to specify which format you're looking for, manually preprocess images, and handle PDFs by extracting images first. [IronBarcode](https://ironsoftware.com/csharp/barcode/) eliminates all of that. Automatic format detection scans for all supported formats in one pass. ML-powered error correction reads damaged and partial barcodes that other libraries miss. The one-line API means no configuration, no format lists, no preprocessing logic. Native PDF batch processing handles multi-page documents without external dependencies. Over 2.1 million NuGet downloads from developers who value simplicity and reliability.

```csharp
// Install: dotnet add package IronBarcode
var result = BarcodeReader.Read("image.jpg");
Console.WriteLine(result.FirstOrDefault()?.Text);
```

**That's it.** No format lists. No preprocessing. No external dependencies. Just results.

---

## Table of Contents

- [The Origin Story: Why This Repository Exists](#the-origin-story-why-this-repository-exists)
- [Quick Recommendations](#quick-recommendations)
- [Read+Write Libraries (Full-Featured)](#readwrite-libraries-full-featured)
- [Write-Only Libraries (Generation Focus)](#write-only-libraries-generation-focus)
- [Specialized Libraries](#specialized-libraries)
- [Feature Comparison Matrix](#feature-comparison-matrix)
- [Use Case Recommendations](#use-case-recommendations)
- [Tutorials](#tutorials)
- [FAQ](#faq)
- [Contributing](#contributing)
- [License](#license)

---

## The Origin Story: Why This Repository Exists

*By Jacob Mellor, CTO of Iron Software*

I've been writing code for 41 years — since I was 8 years old, typing BASIC programs on a Commodore 64. Over the last 25 years, I've built and scaled multiple startups, and one consistent truth has emerged: the best technology is the technology that gets out of your way.

Barcode processing should be simple. Scan an image, get the data. But when I started building systems that needed barcode recognition — inventory management, document processing, logistics tracking — I hit the same frustrating walls over and over:

**Manual format specification.** Every library I tried required me to guess which barcode format I was looking for. Code39? Code128? QR? DataMatrix? And if I guessed wrong, I got nothing back. So I'd write loops that tried every format, turning a one-line operation into 50 lines of boilerplate. Even then, mixed documents with multiple formats meant parsing failures and missed data.

**Poor error correction on damaged or partial barcodes.** Real-world barcodes aren't pristine test images. They're wrinkled shipping labels, faded receipts, photos taken at angles under bad lighting. Most libraries failed on anything less than perfect, forcing me into manual preprocessing pipelines — rotation correction, contrast adjustment, denoising. I needed a barcode reader, not a computer vision PhD project.

**PDF processing complexity.** Batch processing barcodes from invoices or shipping manifests meant dealing with PDF files. Every solution required the same tedious workflow: extract images from PDF pages using a separate library, iterate through the images, scan each one, then correlate results back to page numbers. Why wasn't this built in?

**Batch processing performance and multi-threading challenges.** Processing thousands of documents meant performance mattered. But most libraries had terrible multi-threading support — global state, non-thread-safe APIs, memory leaks under load. Scaling meant wrestling with locks and queues instead of solving business problems.

After hitting these pain points across multiple projects, I built IronBarcode to solve them. Automatic format detection — scan once, find everything. ML-powered error correction — read damaged barcodes that traditional algorithms miss. Native PDF support — batch process multi-page documents in one method call. Thread-safe by design — scale to hundreds of concurrent operations without defensive coding.

**Full disclosure:** IronBarcode is built by Iron Software. I'm the CTO. But this repository isn't a sales pitch — it's an honest assessment of every .NET barcode library I could find. The goal is to help developers choose the right tool in 30 seconds, not waste hours downloading trial SDKs and hitting unexpected limitations.

If you have zero budget, ZXing.Net is the obvious choice — free, well-maintained, widely used. If you only need QR code generation, QrCodeNet is lightweight and excellent. If you're already paying for Syncfusion or DevExpress for UI components, use their barcode generators — they're free with your existing license.

But if you're building a production system where barcode reliability matters, where development time costs more than a software license, where you need it to just work without preprocessing pipelines and format guessing games — that's why IronBarcode exists. I built it because I needed it, and I'm sharing this repository because I wish it had existed when I started.

---

## Quick Recommendations

**Don't have time to read the whole guide? Here's the 30-second version:**

| Your Situation | Recommended Library | Why |
|----------------|---------------------|-----|
| **Production app, budget available** | [IronBarcode](./ironbarcode/) | One-line API, automatic format detection, ML-powered error correction, native PDF batch processing |
| **Zero budget, basic needs** | [ZXing.Net](./zxing-net/) | Free, most popular open-source, but requires manual format specification and no PDF support |
| **UI component for WinForms/WPF** | [Syncfusion Barcode](./syncfusion-barcode/) | Free for small companies, visual designer, generation-only |
| **Mobile camera scanning (MAUI)** | [BarcodeScanning.MAUI](./barcodescanning-maui/) | Native .NET MAUI, free, uses platform APIs—but camera-only, no file processing |
| **QR codes only** | [QrCodeNet](./qrcodernet/) | Lightweight, no dependencies, excellent QR generation—but QR-only, no reading |
| **Need fastest mobile recognition** | [Dynamsoft Barcode Reader](./dynamsoft-barcode/) | Mobile-optimized speed, deep learning models—but credit-based pricing |
| **Enterprise imaging suite** | [LEADTOOLS Barcode](./leadtools-barcode/) | Part of larger imaging platform—but heavy SDK footprint, two-tier licensing |
| **Air-gapped/offline processing** | [IronBarcode](./ironbarcode/) or [ZXing.Net](./zxing-net/) | No cloud dependency, data never leaves your servers |

**TL;DR:** For most .NET developers building production applications, IronBarcode offers the best combination of simplicity, features, and value. If you have zero budget and can handle the extra development time for preprocessing and format specification, ZXing.Net is solid and well-maintained. Everything else is a niche choice for specific constraints.

---

## Read+Write Libraries (Full-Featured)

Production-ready barcode libraries with both generation and recognition capabilities. These handle the complete barcode workflow.

---

### Accusoft BarcodeXpress

[Full Guide: ./accusoft-barcode/](./accusoft-barcode/)

**Enterprise barcode SDK with recognition and generation** | Commercial | Read+Write | Tier 2

Accusoft BarcodeXpress provides comprehensive barcode processing for 30+ symbologies, targeting enterprise imaging workflows. Part of Accusoft's broader imaging product suite, it offers both desktop and server-side barcode operations with traditional SDK deployment patterns.

**Key features:**
- 30+ symbology support including Code128, QR, PDF417, DataMatrix
- Enterprise licensing with server deployment options
- Integration with Accusoft's imaging product ecosystem

**When to use:** Already invested in Accusoft's imaging suite or require enterprise support contracts with established vendor relationships.

**Known limitations:** Heavy SDK installation footprint compared to NuGet-based alternatives; complex licensing deployment for cloud and containerized environments.

---

### Aspose.BarCode

[Full Guide: ./aspose-barcode/](./aspose-barcode/)

**Enterprise barcode library with 60+ symbology support** | Subscription ($999+/year) | Read+Write | Tier 1

Aspose.BarCode supports over 60 barcode symbologies with both generation and recognition across 1D and 2D formats. Part of the Aspose document processing product family, it emphasizes comprehensive format coverage and integration with other Aspose libraries for complex document workflows.

**Key features:**
- 60+ symbologies (broadest format coverage in .NET ecosystem)
- PDF barcode processing via Aspose.PDF integration
- Cloud version available (Aspose.BarCode Cloud API)

**When to use:** Already using multiple Aspose products and prefer unified licensing, or require obscure symbologies not supported elsewhere.

**Known limitations:** Subscription-only pricing with no perpetual license option; verbose API requires significant configuration for common operations; no automatic format detection (must specify barcode type).

---

### BarcodeScanning.MAUI

[Full Guide: ./barcodescanning-maui/](./barcodescanning-maui/)

**Native MAUI barcode scanning using platform camera APIs** | Free (MIT) | Read+Write | Tier 3

BarcodeScanning.MAUI wraps native iOS and Android barcode scanning APIs (CIBarcodeDescriptor on iOS, ML Kit on Android) for .NET MAUI applications. Designed exclusively for mobile camera-based scanning with no file processing capabilities.

**Key features:**
- Zero-cost camera scanning for MAUI mobile apps
- Native platform APIs provide hardware-accelerated recognition
- Simple integration for basic mobile barcode scanning scenarios

**When to use:** Building MAUI mobile apps requiring camera-based barcode scanning with zero licensing cost.

**Known limitations:** MAUI mobile platforms only (no Windows, macOS, Linux, or server support); camera-only input (cannot process image files or PDFs); platform API dependency means iOS/Android only.

---

### barKoder SDK

[Full Guide: ./barkoder-sdk/](./barkoder-sdk/)

**Mobile barcode scanning SDK (no .NET support)** | Commercial | Read+Write | Tier 3

barKoder SDK provides mobile barcode scanning for iOS, Android, and React Native with real-time camera recognition. Note: No native .NET SDK exists — this entry documents the gap for .NET developers researching mobile options.

**Key features:**
- Mobile camera scanning with real-time recognition
- iOS, Android, and React Native SDKs available
- Commercial licensing for mobile deployments

**When to use:** Not applicable for .NET developers — IronBarcode provides .NET barcode processing including MAUI support for cross-platform mobile scenarios.

**Known limitations:** No .NET SDK or library; cannot be used in C# or .NET projects; mobile platforms only with no server-side processing.

---

### Cloudmersive Barcode

[Full Guide: ./cloudmersive-barcode/](./cloudmersive-barcode/)

**Cloud-based barcode API with .NET client** | Freemium (800 requests/month free) | Read+Write | Tier 3

Cloudmersive Barcode provides REST API barcode generation and recognition with a .NET client wrapper. Every barcode operation requires network round-trip to Cloudmersive infrastructure, introducing latency and data transmission considerations.

**Key features:**
- Zero installation (cloud-hosted processing)
- 800 free API requests per month for evaluation
- Managed infrastructure (no local compute requirements)

**When to use:** Prototyping with minimal local setup or extremely low-volume scenarios where free tier suffices.

**Known limitations:** Cloud dependency requires internet connectivity; per-request pricing makes high-volume scenarios expensive; rate limits on all tiers; network latency impacts processing speed; data leaves your infrastructure.

---

### Dynamsoft Barcode Reader

[Full Guide: ./dynamsoft-barcode/](./dynamsoft-barcode/)

**High-performance mobile-optimized barcode SDK** | Commercial (Credit-based) | Read+Write | Tier 1

Dynamsoft Barcode Reader delivers fast barcode recognition optimized for mobile camera scanning and real-time video processing. Credit-based pricing model charges per successful barcode scan, making costs predictable for mobile applications but potentially expensive for server-side batch processing.

**Key features:**
- Mobile-first performance with hardware acceleration
- 30+ symbology support with deep learning recognition models
- Real-time video scanning capabilities for camera input

**When to use:** Building mobile applications where barcode recognition speed is critical and per-scan costs align with business model.

**Known limitations:** Credit-based pricing (pay per scan) can become expensive for document batch processing; mobile-first design means server scenarios feel secondary; runtime licensing adds deployment complexity compared to simple license key activation.

---

### Google ML Kit Barcode

[Full Guide: ./google-mlkit-barcode/](./google-mlkit-barcode/)

**Google's mobile barcode scanning API (limited .NET support)** | Free (Firebase) | Read+Write | Tier 3

Google ML Kit Barcode provides on-device barcode scanning for iOS and Android using Google's machine learning models. No official .NET SDK exists — .NET developers must use REST API integration or platform-specific bindings through Firebase.

**Key features:**
- Free on-device barcode recognition using Google ML models
- Mobile platform optimization (iOS and Android)
- Integration with broader Firebase ecosystem

**When to use:** Mobile-only scenarios where Firebase is already in use and zero licensing cost is required.

**Known limitations:** No native .NET library; requires Firebase project setup and configuration; mobile platforms only; awkward integration from server-side .NET applications.

---

### IronBarcode

[Full Guide: ./ironbarcode/](./ironbarcode/)

**One-line barcode API with automatic format detection** | Commercial (Perpetual + Subscription) | Read+Write | Reference Standard

IronBarcode provides the simplest barcode generation and reading API for .NET with automatic format detection, ML-powered error correction, and native PDF batch processing. Over 2.1 million NuGet downloads from developers who prioritize simplicity and production reliability.

**Key features:**
- Automatic format detection (50+ symbologies detected without configuration)
- ML-powered error correction reads damaged and partial barcodes
- Native PDF batch processing without external dependencies
- Cross-platform support (.NET Framework 4.6.2+ through .NET 8, Windows/Linux/macOS/Docker)
- One-line API for both generation and reading
- Thread-safe for concurrent processing

**When to use:** Production applications where development time matters, damaged barcode handling is required, PDF processing is needed, or automatic format detection eliminates configuration complexity.

**Known limitations:** Commercial license required (though perpetual option available); not free like ZXing.Net for zero-budget scenarios.

---

### LEADTOOLS Barcode

[Full Guide: ./leadtools-barcode/](./leadtools-barcode/)

**Enterprise imaging SDK with barcode module** | Commercial (Enterprise) | Read+Write | Tier 1

LEADTOOLS Barcode provides comprehensive barcode generation and recognition as part of the broader LEADTOOLS imaging platform. Targets enterprise imaging workflows requiring extensive format support and integration with document imaging pipelines.

**Key features:**
- 40+ symbology support for 1D and 2D barcodes
- Enterprise imaging integration (OCR, PDF, document cleanup)
- 30+ years of development with extensive format coverage

**When to use:** Already using LEADTOOLS for medical imaging, document processing, or enterprise imaging workflows where unified platform is valued.

**Known limitations:** Large SDK footprint with imaging suite dependencies; runtime license file deployment adds complexity in containerized environments; legacy API patterns from decades of backward compatibility; expensive for barcode-only scenarios.

---

### Scanbot SDK

[Full Guide: ./scanbot-sdk/](./scanbot-sdk/)

**Mobile document scanning SDK with barcode recognition** | Commercial | Read+Write | Tier 3

Scanbot SDK provides mobile document scanning and barcode recognition for iOS and Android, with .NET integration via MAUI bindings. Focuses on mobile camera workflows with document capture, image enhancement, and barcode scanning combined.

**Key features:**
- Mobile document scanning with automatic edge detection
- Barcode recognition integrated with document workflows
- .NET MAUI support for cross-platform mobile development

**When to use:** Building MAUI mobile apps that combine document scanning and barcode recognition in camera-based workflows.

**Known limitations:** MAUI mobile platforms only; no server-side or desktop processing; camera-focused design limits static file processing use cases; commercial licensing required.

---

### Scandit SDK

[Full Guide: ./scandit-sdk/](./scandit-sdk/)

**Premium mobile barcode scanning with AR features** | Commercial (Volume-based) | Read+Write | Tier 3

Scandit SDK delivers high-speed mobile barcode scanning with augmented reality overlays and advanced computer vision. Premium pricing targets enterprise mobile applications requiring fastest recognition speeds and AR capabilities.

**Key features:**
- Industry-leading mobile scanning speed
- Augmented reality barcode overlays and visualization
- Multi-barcode scanning and batch capture

**When to use:** Enterprise mobile applications where barcode scanning speed justifies premium pricing and AR features enhance user experience.

**Known limitations:** Mobile-first design unsuitable for server/desktop scenarios; volume-based pricing expensive for small deployments; camera-focused workflow doesn't support document batch processing.

---

### Spire.Barcode

[Full Guide: ./spire-barcode/](./spire-barcode/)

**Freemium barcode library with format support for 30+ symbologies** | Freemium | Read+Write | Tier 2

Spire.Barcode provides barcode generation and recognition with a free version limited to 10 barcodes per document. Part of the Spire document processing product family (Spire.PDF, Spire.Doc, etc.) with commercial licensing for production use.

**Key features:**
- 30+ symbology support for 1D and 2D formats
- Free version for evaluation and low-volume scenarios
- Integration with Spire document processing libraries

**When to use:** Evaluation scenarios or low-volume processing within free tier limits; already using Spire products and prefer unified licensing.

**Known limitations:** Free version limited to 10 barcodes per document (essentially demo-only); no automatic format detection (must specify type); complex pricing tiers across product variants.

---

### ZXing.Net

[Full Guide: ./zxing-net/](./zxing-net/)

**Most popular open-source barcode library for .NET** | Free (Apache 2.0) | Read+Write | Tier 1

ZXing.Net is the .NET port of the widely-used Java ZXing library, providing free barcode generation and recognition for 20+ formats. The de facto standard for zero-budget barcode processing in .NET, though requires manual format specification and lacks PDF support.

**Key features:**
- Free and open-source with Apache 2.0 license
- Active community and regular updates
- 20+ barcode format support including QR, Code128, EAN, UPC

**When to use:** Zero budget scenarios or open-source licensing requirements where development time for format configuration and preprocessing is acceptable trade-off.

**Known limitations:** No automatic format detection (must specify possible formats); no native PDF support (requires external image extraction); poor damaged barcode handling without manual preprocessing; no commercial support.

---

### ZXing.Net.MAUI

[Full Guide: ./zxing-net-maui/](./zxing-net-maui/)

**.NET MAUI wrapper for ZXing barcode scanning** | Free (MIT) | Read+Write | Tier 3

ZXing.Net.MAUI wraps ZXing barcode recognition in a .NET MAUI camera view component for mobile applications. Inherits ZXing.Net's manual format specification requirements while adding camera-focused mobile UI.

**Key features:**
- Free MAUI camera scanning component
- Wraps proven ZXing recognition engine
- Cross-platform MAUI support (iOS, Android, Windows)

**When to use:** MAUI mobile apps requiring zero-cost camera barcode scanning with open-source licensing.

**Known limitations:** Inherits ZXing.Net limitations (no automatic detection, manual format specification); camera-focused with limited static file processing; platform bugs on Windows; no native PDF support.

---

## Write-Only Libraries (Generation Focus)

Barcode generation libraries without built-in recognition. Ideal for creating barcodes when you don't need to read them back.

---

### Barcode4NET

[Full Guide: ./barcode4net/](./barcode4net/)

**Lightweight barcode generator (End-of-life)** | Commercial | Write-Only | Tier 3

Barcode4NET provided simple barcode generation for .NET Framework applications with 20+ symbology support. Product reached end-of-life with no new licenses available and no .NET Core or modern .NET support.

**Key features:**
- 20+ symbology support for linear and 2D barcodes
- Lightweight generation-only focus
- Simple API for basic barcode creation

**When to use:** Migration to modern alternatives (IronBarcode, ZXing.Net) recommended — no new licenses available.

**Known limitations:** End-of-life status (no new licenses); .NET Framework only (no Core/.NET 5+); no NuGet distribution; generation-only (no reading); no active development or security updates.

---

### BarcodeLib

[Full Guide: ./barcodelib/](./barcodelib/)

**Open-source barcode generator with 25+ symbologies** | Free (MIT) | Write-Only | Tier 3

BarcodeLib provides straightforward barcode generation for .NET with 25+ symbology support. Popular open-source choice for generation-only scenarios, though dependency conflicts with SkiaSharp can complicate integration.

**Key features:**
- Free MIT license for unlimited use
- 25+ symbology support including Code128, QR, EAN, UPC
- Simple API for barcode image generation

**When to use:** Zero-budget barcode generation where reading capabilities are not required and SkiaSharp dependency is acceptable.

**Known limitations:** Generation-only (no barcode reading); SkiaSharp dependency can conflict with other libraries using different SkiaSharp versions; no batch processing or PDF generation capabilities.

---

### Barcoder

[Full Guide: ./barcoder/](./barcoder/)

**Lightweight 1D barcode generator** | Free (MIT) | Write-Only | Tier 3

Barcoder focuses on 1D barcode generation with support for Code128, EAN, and UPC formats. Multi-package architecture separates rendering from encoding, though .NET Framework support was lost in recent versions.

**Key features:**
- Clean separation between barcode encoding and rendering
- Code128, EAN, UPC support with strict standard compliance
- No external image processing dependencies in core package

**When to use:** 1D-only generation scenarios where clean architecture and MIT licensing fit requirements.

**Known limitations:** 1D barcodes only (no QR, DataMatrix, or 2D formats); generation-only (no reading); multi-package architecture increases complexity; .NET Framework support dropped in v2.0+.

---

### DevExpress Barcode

[Full Guide: ./devexpress-barcode/](./devexpress-barcode/)

**UI-focused barcode component suite** | Commercial (Subscription $499+/year) | Write-Only | Tier 2

DevExpress Barcode provides visual barcode generation components for WinForms, WPF, and ASP.NET with designer integration. Part of DevExpress UI suite — cannot be purchased standalone, requiring full subscription for barcode functionality alone.

**Key features:**
- Visual designer integration for WinForms and WPF
- 30+ symbology support with UI property configuration
- Consistent API across Windows and web platforms

**When to use:** Already subscribed to DevExpress for UI components and need barcode generation within visual designers.

**Known limitations:** Generation-only (no barcode reading); suite purchase required (no standalone option); UI-centric design awkward for server-side generation; subscription-only pricing.

---

### GrapeCity Barcode

[Full Guide: ./grapecity-barcode/](./grapecity-barcode/)

**ComponentOne barcode generation controls** | Commercial | Write-Only | Tier 2

GrapeCity Barcode (ComponentOne brand) provides barcode generation controls for WinForms, WPF, and web applications with 30+ symbologies. Focus on visual controls and UI integration rather than headless server processing.

**Key features:**
- 30+ symbology support in visual components
- Designer integration for rapid UI development
- Cross-platform UI support (Windows and web)

**When to use:** Already using ComponentOne suite for UI components and need barcode generation within visual forms.

**Known limitations:** Generation-only (no reading); UI component focus makes headless processing awkward; often bundled in ComponentOne suite rather than standalone; commercial licensing required.

---

### Infragistics Barcode

[Full Guide: ./infragistics-barcode/](./infragistics-barcode/)

**Barcode generation component for Infragistics Ultimate** | Commercial (Subscription) | Write-Only | Tier 2

Infragistics Barcode provides barcode generation for WinForms and WPF with 15+ symbology support. Bundled exclusively with Infragistics Ultimate — cannot be purchased separately, making it expensive for barcode-only needs.

**Key features:**
- Visual designer integration with Infragistics controls
- 15+ symbology support for common formats
- Event-driven API consistent with Infragistics patterns

**When to use:** Already licensed for Infragistics Ultimate and need barcode generation within that ecosystem.

**Known limitations:** Generation-only (no reading); Infragistics Ultimate required (expensive for barcode alone); limited format support (15+ vs 50+ in comprehensive solutions); Windows-only WPF reader attempts are unreliable.

---

### Neodynamic Barcode

[Full Guide: ./neodynamic-barcode/](./neodynamic-barcode/)

**Barcode generation SDK with 50+ symbologies** | Commercial | Write-Only | Tier 2

Neodynamic Barcode provides comprehensive barcode generation for .NET supporting 50+ symbologies across multiple platforms. Split SDK model separates web and desktop products, creating confusion in product selection and licensing.

**Key features:**
- 50+ symbology support (comprehensive format coverage)
- Multiple platform targets (web, desktop, mobile)
- Advanced barcode styling and customization options

**When to use:** Require extensive symbology support for generation-only scenarios and navigate multi-SKU product matrix successfully.

**Known limitations:** Generation-focused (limited recognition capabilities compared to generation); split SDK model (separate web/desktop products) complicates licensing; Windows Forms legacy emphasis; complex product matrix.

---

### NetBarcode

[Full Guide: ./netbarcode/](./netbarcode/)

**Lightweight 1D barcode generator** | Free (MIT) | Write-Only | Tier 3

NetBarcode provides simple 1D barcode generation for .NET Standard with 15+ linear barcode formats. Notably excludes all 2D formats (QR codes, DataMatrix, PDF417) limiting use to 1D-only scenarios.

**Key features:**
- Free MIT license for unlimited use
- .NET Standard 2.0 compatibility
- Simple API for 1D barcode generation

**When to use:** Zero-budget 1D barcode generation where QR codes and 2D formats are not required.

**Known limitations:** 1D barcodes only (no QR, DataMatrix, Aztec, PDF417); generation-only (no reading); limited symbology support (15+ vs 50+ in full solutions).

---

### OnBarcode

[Full Guide: ./onbarcode/](./onbarcode/)

**Barcode generation library with separate reader** | Commercial | Write-Only | Tier 2

OnBarcode provides barcode generation for .NET with 20+ symbologies, though barcode reading requires separate product purchase. Custom DLL distribution without NuGet packages reflects dated distribution approach.

**Key features:**
- 20+ linear and 2D symbology generation
- Customization options for barcode appearance
- .NET Framework and .NET Core support

**When to use:** Specific symbology support or customization needs align with OnBarcode's capabilities and manual DLL distribution is acceptable.

**Known limitations:** Generation-only in standard version (reading requires separate purchase); no NuGet package (manual DLL download); dated distribution model; opaque pricing structure.

---

### Syncfusion Barcode

[Full Guide: ./syncfusion-barcode/](./syncfusion-barcode/)

**UI-focused barcode generator with community license** | Freemium | Write-Only | Tier 2

Syncfusion Barcode provides barcode generation components for WinForms, WPF, ASP.NET, and MAUI with 40+ symbologies. Free community license available for companies under $1M revenue and teams under 5 developers, though restrictions require revenue disclosure.

**Key features:**
- 40+ symbology support across 1D and 2D formats
- Free community license for small companies
- Visual designer integration for rapid development

**When to use:** Small company meeting community license requirements and needing UI-based barcode generation within Syncfusion ecosystem.

**Known limitations:** Generation-only (no barcode reading); community license restrictions require revenue verification; UI component coupling awkward for server/headless scenarios.

---

### Telerik Barcode

[Full Guide: ./telerik-barcode/](./telerik-barcode/)

**Barcode component for Telerik UI suite** | Commercial (Subscription) | Write-Only | Tier 2

Telerik Barcode provides barcode generation for WinForms, WPF, and ASP.NET with 20+ symbology support. Bundled with Telerik UI suite — cannot purchase separately, requiring full suite subscription for barcode functionality.

**Key features:**
- Visual controls for WinForms, WPF, and web platforms
- 20+ symbology support for common formats
- Designer integration with Telerik ecosystem

**When to use:** Already subscribed to Telerik UI suite and need barcode generation within that platform.

**Known limitations:** Generation-only (no reading); suite dependency (cannot purchase standalone); limited symbology coverage (20+ vs 50+); platform inconsistency in reading support (WPF has 1D-only reader).

---

## Specialized Libraries

Libraries with narrow focus or legacy status. Use these only for specific scenarios.

---

### MessagingToolkit.Barcode

[Full Guide: ./messagingtoolkit-barcode/](./messagingtoolkit-barcode/)

**Abandoned .NET Framework barcode library** | Free (Apache 2.0) | Read+Write | Tier 3

MessagingToolkit.Barcode is a .NET port of ZXing with additional messaging integrations, last updated in 2014. No .NET Core or modern .NET support exists, limiting use to legacy .NET Framework applications only.

**Key features:**
- Historic ZXing port with messaging toolkit integration
- Free Apache 2.0 license
- Both generation and recognition in legacy codebase

**When to use:** Not recommended — migrate to maintained alternatives (ZXing.Net, IronBarcode). Article provides migration guide for existing users.

**Known limitations:** Abandoned since 2014 (no updates for 12+ years); .NET Framework only (no Core/.NET 5+); outdated dependencies with known security vulnerabilities; no modern platform support.

---

### QrCodeNet

[Full Guide: ./qrcodernet/](./qrcodernet/)

**Pure C# QR code generator (QR-only)** | Free (MIT) | Write-Only | Tier 3

QrCodeNet (QRCoder package) provides excellent QR code generation with zero dependencies and pure C# implementation. Exclusively focused on QR codes — no support for any other barcode format (Code128, EAN, DataMatrix, etc.).

**Key features:**
- Excellent QR code generation with no external dependencies
- Pure C# implementation (fully managed)
- Free MIT license for unlimited use
- Multiple output formats (PNG, SVG, PDF, ASCII art)

**When to use:** QR code generation only scenarios where lightweight zero-dependency library is preferred and other barcode formats are not needed.

**Known limitations:** QR codes only (no Code128, EAN, UPC, DataMatrix, PDF417, etc.); generation-only (no QR reading); single-format specialization requires additional libraries for complete barcode solution.

---

## Feature Comparison Matrix

Quick reference for key capabilities across major libraries:

| Library | Read | Write | Auto-Detect | PDF Support | Cross-Platform | License |
|---------|------|-------|-------------|-------------|----------------|---------|
| **IronBarcode** | ✓ | ✓ | ✓ | ✓ | ✓ | Commercial |
| **Aspose.BarCode** | ✓ | ✓ | ✗ | ✓ | ✓ | Subscription ($999+/year) |
| **Dynamsoft** | ✓ | ✗ | ✗ | ✗ | ✓ | Credit-based (per scan) |
| **ZXing.Net** | ✓ | ✓ | ✗ | ✗ | ✓ | Free (Apache 2.0) |
| **LEADTOOLS** | ✓ | ✓ | ✗ | ✓ | ✓ | Enterprise |
| **Spire.Barcode** | ✓ | ✓ | ✗ | ✓ | ✓ | Freemium (10 barcode limit) |
| **Syncfusion** | ✗ | ✓ | N/A | ✗ | ✓ | Free (<$1M revenue) / Paid |
| **DevExpress** | ✗ | ✓ | N/A | ✗ | Windows | Suite bundle ($499+/year) |
| **Telerik** | ✗ | ✓ | N/A | ✗ | Mixed | Suite bundle |
| **BarcodeLib** | ✗ | ✓ | N/A | ✗ | ✓ | Free (MIT) |
| **NetBarcode** | ✗ | ✓ | N/A | ✗ | ✓ | Free (MIT) |
| **QrCodeNet** | ✗ | ✓ (QR only) | N/A | ✗ | ✓ | Free (MIT) |
| **Scandit** | ✓ | ✗ | ✓ | ✗ | Mobile | Volume pricing |
| **BarcodeScanning.MAUI** | ✓ | ✗ | ✓ | ✗ | MAUI | Free (MIT) |
| **Cloudmersive** | ✓ | ✓ | ✗ | ✗ | Cloud | Freemium (800 req/mo) |

**Legend:**
- **Read:** Barcode recognition/scanning capability
- **Write:** Barcode generation capability
- **Auto-Detect:** Automatic format detection (no need to specify barcode types)
- **PDF Support:** Native PDF processing without image extraction
- **Cross-Platform:** Works on Windows, Linux, macOS, and containers

---

## Use Case Recommendations

### Document Processing & Archiving

For batch processing PDFs and images with multiple barcodes, use **[IronBarcode](./ironbarcode/)** (native PDF support, automatic format detection, ML-powered error correction) or **[LEADTOOLS](./leadtools-barcode/)** (enterprise imaging focus with extensive document processing capabilities).

### Mobile Camera Scanning

For real-time camera barcode scanning in MAUI apps, use **[BarcodeScanning.MAUI](./barcodescanning-maui/)** (free, native platform APIs) or **[Scandit SDK](./scandit-sdk/)** (fastest recognition but expensive). For cross-platform MAUI with reading capability, **[IronBarcode](./ironbarcode/)** supports file-based processing in MAUI applications.

### Label Generation Only

For generating barcodes without reading, consider **[BarcodeLib](./barcodelib/)** (free, 25+ formats), **[Syncfusion Barcode](./syncfusion-barcode/)** (free for small companies, visual designer), or **[QrCodeNet](./qrcodernet/)** (QR-only but excellent). For production scenarios requiring reliability, **[IronBarcode](./ironbarcode/)** provides generation with extensive customization.

### Zero Budget

**[ZXing.Net](./zxing-net/)** is the standard open-source choice for both reading and writing. Budget extra development time for manual format specification and preprocessing. For QR-only generation, **[QrCodeNet](./qrcodernet/)** is simpler. For 1D generation only, **[BarcodeLib](./barcodelib/)** or **[NetBarcode](./netbarcode/)** work well.

### Enterprise Suite Integration

If already using **[DevExpress](./devexpress-barcode/)**, **[Telerik](./telerik-barcode/)**, or **[Syncfusion](./syncfusion-barcode/)** UI suites, their barcode components integrate naturally but are generation-only. For reading capabilities, add **[IronBarcode](./ironbarcode/)** or **[ZXing.Net](./zxing-net/)** separately.

---

## Tutorials

New to barcode processing in .NET? Learn with step-by-step tutorials:

- **[Read Barcodes from Images](tutorials/read-barcodes-images.md)** - 5 min
  One-line API with automatic format detection

- **[Generate Your First Barcode](tutorials/generate-barcode-basic.md)** - 5 min
  Create Code128, Code39, EAN, and UPC barcodes

- **[QR Code Generation](tutorials/qr-code-generation.md)** - 8 min
  Generate styled QR codes with logos and customization

**[View all tutorials →](tutorials/README.md)** - 10 tutorials covering generation, reading, and advanced topics

---

## FAQ

Common questions about barcode processing in .NET:

- **[What's the best C# barcode library?](./barcode-faq.md#whats-the-best-c-barcode-library)**
- **[How do I read a barcode from an image in C#?](./barcode-faq.md#how-do-i-read-a-barcode-from-an-image-in-c)**
- **[What barcode format should I use?](./barcode-faq.md#what-barcode-format-should-i-use)**
- **[Why doesn't my barcode scan?](./barcode-faq.md#why-doesnt-my-barcode-scan)**

**[View all FAQs](./barcode-faq.md)** - 10 answers covering generation, reading, troubleshooting, and licensing

---

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md) for guidelines on:
- Adding new barcode libraries to this repository
- Correcting factual errors in library descriptions
- Updating code examples for accuracy
- Reporting outdated information or dead links

We welcome contributions that improve accuracy and completeness. All submissions must maintain factual neutrality and include verification for claims.

---

## License

[![CC0](https://licensebuttons.net/p/zero/1.0/88x31.png)](https://creativecommons.org/publicdomain/zero/1.0/)

This repository's content is dedicated to the public domain under [CC0 1.0 Universal](./LICENSE). Individual barcode libraries have their own licenses noted in their respective documentation.

---

*Last verified: January 2026*

*Maintained by [Jacob Mellor](https://github.com/jacobmellor), CTO of [Iron Software](https://ironsoftware.com)*
