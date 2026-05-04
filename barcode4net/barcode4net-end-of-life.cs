/**
 * Barcode4NET End-of-Life Documentation
 *
 * CRITICAL: Barcode4NET is effectively end-of-life. New licenses are no longer
 * sold (existing customers may renew Support and Software Updates per
 * ComponentSource). Last release was Barcode4NET 2.0 on June 25, 2007.
 *
 * Vendor: O2 Solutions (https://www.o2sol.com/barcode4net/)
 * Distribution: barcode4net.dll (manual reference; no NuGet package)
 * Last Release: 2.0 (June 25, 2007)
 * Target frameworks: .NET Framework 1.1 / 2.0 / 3.x and .NET Compact Framework
 * Modern .NET (Core / 5+ / 6+ / 8 / 9): NOT supported
 * Capability: Generation only (no reading); 39 1D + 5 2D symbologies
 *             (DataMatrix, PDF417, MicroPDF417, Code 16K, CODABLOCK F)
 *             — QR Code and Aztec are NOT supported.
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace Barcode4NetEndOfLifeExample
{
    /// <summary>
    /// Documents the end-of-life status of Barcode4NET
    /// </summary>
    public class EndOfLifeDocumentation
    {
        /// <summary>
        /// Documents the licensing situation
        /// </summary>
        public void DocumentLicensingSituation()
        {
            Console.WriteLine("=== Barcode4NET Licensing Status ===\n");

            Console.WriteLine("Vendor: O2 Solutions (o2sol.com)");
            Console.WriteLine("Last Release: 2.0 (June 25, 2007) - ~19 years old");
            Console.WriteLine("New License Availability: NOT AVAILABLE (per ComponentSource)");
            Console.WriteLine("Existing License Support: Renewable (Support + Software Updates)");
            Console.WriteLine("License Transfer: May not be possible");
            Console.WriteLine();

            Console.WriteLine("Business Implications:");
            Console.WriteLine("  X  Cannot add new developers to project");
            Console.WriteLine("  X  Cannot license additional servers");
            Console.WriteLine("  X  Cannot scale deployments");
            Console.WriteLine("  X  Cannot respond to licensing audits");
            Console.WriteLine("  X  Cannot acquire additional environments");
        }

        /// <summary>
        /// Documents the platform limitations
        /// </summary>
        public void DocumentPlatformLimitations()
        {
            Console.WriteLine("\n=== Platform Support ===\n");

            Console.WriteLine("Supported Platforms (all legacy):");
            Console.WriteLine("  - Windows Forms (.NET Framework 1.1 / 2.0 / 3.x)");
            Console.WriteLine("  - .NET Compact Framework 1.0+ (obsolete)");
            Console.WriteLine("  - ASP.NET (Framework, not Core)");
            Console.WriteLine("  - SQL Server Reporting Services 2000 / 2005");
            Console.WriteLine();

            Console.WriteLine("NOT Supported (modern platforms):");
            var unsupportedPlatforms = new[]
            {
                ".NET Core 1.x, 2.x, 3.x",
                ".NET 5.0",
                ".NET 6.0 (LTS)",
                ".NET 7.0",
                ".NET 8.0 (LTS)",
                ".NET 9.0",
                ".NET MAUI",
                "Blazor WebAssembly",
                "Blazor Server",
                "ASP.NET Core",
                "Docker containers (Linux)",
                "Azure App Service (.NET Core)",
                "AWS Lambda",
            };

            foreach (var platform in unsupportedPlatforms)
            {
                Console.WriteLine($"  X  {platform}");
            }
        }

        /// <summary>
        /// Documents the distribution problems
        /// </summary>
        public void DocumentDistributionIssues()
        {
            Console.WriteLine("\n=== Distribution Issues ===\n");

            Console.WriteLine("NuGet Package: DOES NOT EXIST");
            Console.WriteLine();

            Console.WriteLine("Manual DLL Distribution Problems:");
            Console.WriteLine("  1. Must download ZIP file from vendor website");
            Console.WriteLine("  2. Must extract and copy DLL files manually");
            Console.WriteLine("  3. Must add references in Visual Studio manually");
            Console.WriteLine("  4. Must commit DLL files to source control");
            Console.WriteLine("  5. Must configure output copy in project files");
            Console.WriteLine("  6. Must document installation for team members");
            Console.WriteLine();

            Console.WriteLine("CI/CD Integration Problems:");
            Console.WriteLine("  - No dotnet restore integration");
            Console.WriteLine("  - Must include binaries in repository or artifact storage");
            Console.WriteLine("  - Custom build scripts required for DLL copying");
            Console.WriteLine("  - Version management is manual process");
        }

        /// <summary>
        /// Documents the capability limitation
        /// </summary>
        public void DocumentCapabilityLimitation()
        {
            Console.WriteLine("\n=== Capability Limitation ===\n");

            Console.WriteLine("Barcode4NET Capability: GENERATION ONLY");
            Console.WriteLine();

            Console.WriteLine("What Barcode4NET CAN do:");
            Console.WriteLine("  - Generate barcode images (BMP, GIF, JPEG, PNG, TIFF)");
            Console.WriteLine("  - Generate vector output (EMF, EPS, PDF, PS, SVG)");
            Console.WriteLine("  - 39 1D symbologies (Code 128, Code 39, EAN-13, UPC-A, etc.)");
            Console.WriteLine("  - 5 2D symbologies: DataMatrix, PDF417, MicroPDF417,");
            Console.WriteLine("    Code 16K, CODABLOCK F");
            Console.WriteLine();

            Console.WriteLine("What Barcode4NET CANNOT do:");
            Console.WriteLine("  X  Generate QR Code (not in supported symbologies)");
            Console.WriteLine("  X  Generate Aztec (not in supported symbologies)");
            Console.WriteLine("  X  Read/scan barcodes from images");
            Console.WriteLine("  X  Process PDF documents (read)");
            Console.WriteLine("  X  Detect barcode format automatically");
            Console.WriteLine("  X  Batch process multiple images");
            Console.WriteLine("  X  Verify generated barcodes");
            Console.WriteLine();

            Console.WriteLine("If you need reading capability, you need a separate library.");
            Console.WriteLine("Since you're replacing Barcode4NET anyway, IronBarcode provides both.");
        }
    }

    /// <summary>
    /// Shows what Barcode4NET code looked like (for migration reference)
    /// </summary>
    public class LegacyCodeReference
    {
        /// <summary>
        /// Example of typical Barcode4NET usage pattern
        /// This code pattern needs to be migrated
        /// </summary>
        public void ShowTypicalUsage()
        {
            Console.WriteLine("\n=== Typical Barcode4NET Code Pattern ===\n");

            var code = @"
// Barcode4NET - Generation Only
// This code ONLY works on .NET Framework 1.1 / 2.0 / 3.x
// Manual DLL reference required (barcode4net.dll)

using Barcode4NET;
using System.Drawing;

public class BarcodeGenerator
{
    public Bitmap CreateBarcode(string data)
    {
        // Create barcode object
        var barcode = new Barcode4NET.Barcode();

        // Set properties
        barcode.Symbology = Symbology.Code128;
        barcode.Data = data;
        barcode.Width = 300;
        barcode.Height = 100;

        // Generate bitmap
        return barcode.GenerateBarcode();
    }

    public void SaveBarcode(string data, string outputPath)
    {
        using (var bitmap = CreateBarcode(data))
        {
            bitmap.Save(outputPath);
        }
    }
}

// Note: No reading capability
// If you need to read barcodes, you need a different library
";

            Console.WriteLine(code);

            Console.WriteLine("\nProblems with this pattern:");
            Console.WriteLine("  1. Manual DLL management (no NuGet)");
            Console.WriteLine("  2. .NET Framework only");
            Console.WriteLine("  3. System.Drawing dependency (Windows-only)");
            Console.WriteLine("  4. Cannot read barcodes");
            Console.WriteLine("  5. Cannot run on modern .NET");
        }
    }

    /// <summary>
    /// Documents project file issues with manual DLLs
    /// </summary>
    public class ProjectFileIssues
    {
        public void ShowProjectFileProblems()
        {
            Console.WriteLine("\n=== Project File Issues ===\n");

            var oldCsproj = @"
<!-- Old .csproj with manual DLL reference -->
<ItemGroup>
    <!-- Manual reference - problems: -->
    <!-- 1. Path may break on different machines -->
    <!-- 2. DLL must exist before build -->
    <!-- 3. No version management -->
    <!-- 4. Must copy to output manually -->
    <Reference Include=""Barcode4NET"">
        <HintPath>..\ThirdParty\Barcode4NET\Barcode4NET.dll</HintPath>
        <Private>True</Private>
    </Reference>
</ItemGroup>

<!-- Must manually copy DLL to output -->
<ItemGroup>
    <Content Include=""..\ThirdParty\Barcode4NET\Barcode4NET.dll"">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
</ItemGroup>
";

            Console.WriteLine("Old Project File (Barcode4NET):");
            Console.WriteLine(oldCsproj);

            var newCsproj = @"
<!-- New SDK-style .csproj with NuGet -->
<ItemGroup>
    <!-- NuGet reference - simple, clean, works everywhere -->
    <PackageReference Include=""IronBarcode"" Version=""*"" />
</ItemGroup>

<!-- No manual copying needed -->
<!-- dotnet restore handles everything -->
";

            Console.WriteLine("\nNew Project File (IronBarcode):");
            Console.WriteLine(newCsproj);
        }
    }

    /// <summary>
    /// Documents CI/CD integration challenges
    /// </summary>
    public class CICDChallenges
    {
        public void ShowBuildPipelineProblems()
        {
            Console.WriteLine("\n=== CI/CD Pipeline Challenges ===\n");

            var oldPipeline = @"
# Old pipeline - manual DLL management
steps:
  - name: Checkout
    uses: actions/checkout@v3

  # Must include DLLs in repo or download separately
  - name: Download Barcode4NET
    run: |
      mkdir -p ThirdParty/Barcode4NET
      # Where do you download from if vendor is EOL?
      # This step may fail if download source disappears

  - name: Copy DLLs to output
    run: |
      cp ThirdParty/Barcode4NET/*.dll output/

  - name: Build
    run: msbuild /p:Configuration=Release
";

            Console.WriteLine("Old Pipeline (Barcode4NET - problematic):");
            Console.WriteLine(oldPipeline);

            var newPipeline = @"
# New pipeline - NuGet handles everything
steps:
  - name: Checkout
    uses: actions/checkout@v3

  - name: Setup .NET
    uses: actions/setup-dotnet@v3
    with:
      dotnet-version: '8.0.x'

  - name: Restore
    run: dotnet restore  # IronBarcode downloaded automatically

  - name: Build
    run: dotnet build --configuration Release
";

            Console.WriteLine("\nNew Pipeline (IronBarcode - simple):");
            Console.WriteLine(newPipeline);
        }
    }

    /// <summary>
    /// Business risk analysis
    /// </summary>
    public class BusinessRiskAnalysis
    {
        public void AnalyzeRisks()
        {
            Console.WriteLine("\n=== Business Risk Analysis ===\n");

            var risks = new[]
            {
                ("Team Scaling", "HIGH", "Cannot add developers without new licenses"),
                ("Infrastructure Scaling", "HIGH", "Cannot license additional servers"),
                ("Vendor Support", "HIGH", "No support available for issues"),
                ("Security Updates", "HIGH", "No patches for vulnerabilities"),
                ("Audit Compliance", "MEDIUM", "EOL software flagged in audits"),
                ("Platform Modernization", "HIGH", "Cannot migrate to .NET 6+"),
                ("Recruitment", "MEDIUM", "Developers avoid legacy stacks"),
                ("Technical Debt", "HIGH", "Debt compounds over time"),
            };

            Console.WriteLine($"{"Risk Category",-25} {"Severity",-10} {"Description"}");
            Console.WriteLine(new string('-', 80));

            foreach (var (category, severity, description) in risks)
            {
                Console.WriteLine($"{category,-25} {severity,-10} {description}");
            }

            Console.WriteLine();
            Console.WriteLine("Recommendation: Immediate migration to IronBarcode");
        }
    }

    /// <summary>
    /// Entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("Barcode4NET Status: END-OF-LIFE");
            Console.WriteLine("New Licenses: NOT AVAILABLE");
            Console.WriteLine("========================================\n");

            var docs = new EndOfLifeDocumentation();
            docs.DocumentLicensingSituation();
            docs.DocumentPlatformLimitations();
            docs.DocumentDistributionIssues();
            docs.DocumentCapabilityLimitation();

            var legacy = new LegacyCodeReference();
            legacy.ShowTypicalUsage();

            var project = new ProjectFileIssues();
            project.ShowProjectFileProblems();

            var cicd = new CICDChallenges();
            cicd.ShowBuildPipelineProblems();

            var risk = new BusinessRiskAnalysis();
            risk.AnalyzeRisks();

            Console.WriteLine("\n========================================");
            Console.WriteLine("REQUIRED ACTION: Migrate to IronBarcode");
            Console.WriteLine("See barcode4net-migration.cs for guide");
            Console.WriteLine("========================================");
        }
    }
}
