/*
 * OnBarcode Manual DLL vs NuGet Distribution, and IronBarcode's NuGet-First Model
 *
 * OnBarcode offers two distribution paths in parallel: a traditional download
 * (purchase, receive ZIP of DLLs, add manual references) and NuGet packages
 * (OnBarcode.Barcode.Generator, OnBarcode.Barcode.Reader, plus framework and
 * SkiaSharp variants). This example contrasts the manual DLL workflow with
 * IronBarcode's NuGet-only distribution under the package id "BarCode".
 *
 * Key differences demonstrated:
 * 1. Manual DLL management (still offered by OnBarcode)
 * 2. OnBarcode NuGet packages
 * 3. IronBarcode NuGet-only distribution
 * 4. Developer experience implications
 *
 * Author: Jacob Mellor, CTO of Iron Software
 * https://ironsoftware.com/about-us/authors/jacobmellor/
 */

using System;
using System.IO;

namespace BarcodeComparison
{
    /// <summary>
    /// Demonstrates OnBarcode's distribution evolution vs IronBarcode's modern approach
    /// </summary>
    public class ManualSetupDemo
    {
        /// <summary>
        /// Demonstrates OnBarcode's historical manual setup process
        /// </summary>
        public void DemonstrateHistoricalSetup()
        {
            Console.WriteLine("=== OnBarcode Manual DLL Distribution (Still Offered) ===\n");

            Console.WriteLine("OnBarcode continues to offer a manual DLL distribution alongside its NuGet packages.\n");
            Console.WriteLine("Customers who select this path follow these steps:\n");

            Console.WriteLine("Step 1: Purchase and Download");
            Console.WriteLine(@"
    1. Visit OnBarcode.com
    2. Choose product tier (Linear or Linear+2D Generator;
       Reader sold separately) and license size
    3. Purchase license (perpetual, 30-day money-back)
    4. Receive download link via email
    5. Download ZIP archive containing:
       - OnBarcode.Barcode.dll
       - OnBarcode.Barcode.xml (documentation)
       - License.txt
       - Sample projects
");

            Console.WriteLine("Step 2: Extract and Store DLLs");
            Console.WriteLine(@"
    Project Structure:
    MyProject/
    ├── lib/                      <- Must create and manage
    │   ├── OnBarcode.Barcode.dll <- Manual placement
    │   └── OnBarcode.Barcode.xml
    ├── src/
    │   └── MyCode.cs
    └── MyProject.csproj
");

            Console.WriteLine("Step 3: Add Reference to Project");
            Console.WriteLine(@"
    <!-- In .csproj file - manual reference -->
    <ItemGroup>
      <Reference Include=""OnBarcode.Barcode"">
        <HintPath>lib\OnBarcode.Barcode.dll</HintPath>
        <Private>true</Private>
      </Reference>
    </ItemGroup>

    <!-- Or in old-style .NET Framework project -->
    <!-- Right-click References -> Add Reference -> Browse -> Select DLL -->
");

            Console.WriteLine("Step 4: Configure Build to Copy DLLs");
            Console.WriteLine(@"
    <!-- Ensure DLL is copied to output -->
    <ItemGroup>
      <None Include=""lib\OnBarcode.Barcode.dll"">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      </None>
    </ItemGroup>
");

            Console.WriteLine("Step 5: Commit DLLs to Source Control");
            Console.WriteLine(@"
    # DLLs must be tracked in repository
    git add lib/OnBarcode.Barcode.dll
    git commit -m ""Add OnBarcode library""

    # Problems:
    # - Binary files in git
    # - Large repository size over time
    # - Version conflicts possible
");

            Console.WriteLine("\nProblems with manual setup:");
            Console.WriteLine("- No automatic dependency resolution");
            Console.WriteLine("- Manual version tracking");
            Console.WriteLine("- DLLs in source control");
            Console.WriteLine("- Complex upgrade process");
            Console.WriteLine("- No restore from fresh clone");
            Console.WriteLine("- Different setup per developer\n");
        }

        /// <summary>
        /// Demonstrates upgrade complexity with manual DLLs
        /// </summary>
        public void DemonstrateManualUpgrade()
        {
            Console.WriteLine("=== Upgrading OnBarcode (Manual DLL Approach) ===\n");

            Console.WriteLine("When OnBarcode releases a new version:\n");

            Console.WriteLine("Step 1: Check for Updates");
            Console.WriteLine(@"
    - Visit OnBarcode.com website
    - Check version numbers manually
    - No automatic notification
    - Review changelog (if available)
");

            Console.WriteLine("Step 2: Download New Version");
            Console.WriteLine(@"
    - Login to account
    - Download new ZIP
    - Verify download integrity
    - Extract to temporary location
");

            Console.WriteLine("Step 3: Replace DLLs");
            Console.WriteLine(@"
    - Close Visual Studio
    - Delete old DLLs from lib/ folder
    - Copy new DLLs to lib/ folder
    - Reopen Visual Studio
    - Rebuild project
");

            Console.WriteLine("Step 4: Update Source Control");
            Console.WriteLine(@"
    git add lib/OnBarcode.Barcode.dll
    git commit -m ""Upgrade OnBarcode to version X.Y""
    git push

    # Binary diff - large commit
    # No easy way to see what changed
");

            Console.WriteLine("Step 5: Notify Team");
            Console.WriteLine(@"
    - Team members must pull
    - Rebuild all projects
    - May require VS restart
    - Potential merge conflicts with binary
");

            Console.WriteLine("\nRisks of manual upgrade:");
            Console.WriteLine("- Version mismatch across team");
            Console.WriteLine("- Missing changelog review");
            Console.WriteLine("- No rollback safety net");
            Console.WriteLine("- Breaking changes not detected until runtime\n");
        }

        /// <summary>
        /// Demonstrates OnBarcode's recent NuGet adoption
        /// </summary>
        public void DemonstrateNuGetTransition()
        {
            Console.WriteLine("=== OnBarcode NuGet Packages ===\n");

            Console.WriteLine("OnBarcode publishes its libraries on nuget.org under the OnBarcode profile.\n");

            Console.WriteLine("Available Packages (current versions: Generator 10.5.x, Reader 10.3.x):");
            Console.WriteLine(@"
    OnBarcode.Barcode.Generator                    <- Generation (cross-platform)
    OnBarcode.Barcode.Generator.SkiaSharp          <- SkiaSharp variant
    OnBarcode.Barcode.Generator.AspNet.Framework   <- ASP.NET Framework variant
    OnBarcode.Barcode.Generator.WinForms.Framework <- WinForms variant
    OnBarcode.Barcode.Reader                       <- Reading (separate purchase!)
    OnBarcode.Barcode.Reader.Framework             <- .NET Framework reader
    OnBarcode.Barcode.Reader.Document.PDF          <- PDF reading add-on
");

            Console.WriteLine("Installation (Modern Approach):");
            Console.WriteLine(@"
    dotnet add package OnBarcode.Barcode.Generator
");

            Console.WriteLine("Improvements over manual:");
            Console.WriteLine("- Standard NuGet installation");
            Console.WriteLine("- Automatic dependency resolution");
            Console.WriteLine("- Easy version updates");
            Console.WriteLine("- No DLLs in source control\n");

            Console.WriteLine("Remaining Challenges:");
            Console.WriteLine("- Multiple package variants to choose from (Framework, SkiaSharp, AspNet)");
            Console.WriteLine("- Reader still requires separate purchase and separate package");
            Console.WriteLine("- PDF reading requires the Reader.Document.PDF add-on package\n");
        }

        /// <summary>
        /// Demonstrates IronBarcode's always-modern approach
        /// </summary>
        public void DemonstrateIronBarcodeModern()
        {
            Console.WriteLine("=== IronBarcode Distribution (NuGet-Only) ===\n");

            Console.WriteLine("IronBarcode is distributed exclusively through NuGet under the package id \"BarCode\":\n");

            Console.WriteLine("Installation:");
            Console.WriteLine(@"
    # Single command. Note: NuGet package id is ""BarCode"";
    # the C# namespace is ""IronBarCode"" (capital C).
    dotnet add package BarCode

    # That's it. No manual steps. No separate Reader package.
");

            Console.WriteLine("What happens automatically:");
            Console.WriteLine(@"
    1. Package downloaded from NuGet.org
    2. Dependencies resolved automatically
    3. Assembly references added to project
    4. Restore works from any fresh clone
    5. IntelliSense immediately available
");

            Console.WriteLine("Version Management:");
            Console.WriteLine(@"
    # Upgrade to latest
    dotnet add package BarCode

    # Or specify version
    dotnet add package BarCode --version 2025.1.0

    # Check for updates
    dotnet list package --outdated
");

            Console.WriteLine("Team Synchronization:");
            Console.WriteLine(@"
    # In .csproj - version tracked as text
    <PackageReference Include=""BarCode"" Version=""2025.1.0"" />

    # Any team member runs:
    dotnet restore

    # Everyone has same version
");

            Console.WriteLine("\nBenefits of NuGet-only distribution:");
            Console.WriteLine("- Single package covers generation and reading");
            Console.WriteLine("- Documentation assumes NuGet workflow");
            Console.WriteLine("- No manual DLL handoff to maintain\n");
        }

        /// <summary>
        /// Demonstrates CI/CD implications
        /// </summary>
        public void DemonstrateCICDImplications()
        {
            Console.WriteLine("=== CI/CD Pipeline Comparison ===\n");

            Console.WriteLine("OnBarcode (Manual DLL Approach):\n");
            Console.WriteLine(@"
    # azure-pipelines.yml or similar

    # Problem: DLLs must be in repository
    # OR pipeline must handle download

    steps:
    - checkout: self

    # Option A: DLLs committed to repo (bad)
    # Works but bloats repository

    # Option B: Download in pipeline (complex)
    - script: |
        curl -o onbarcode.zip https://download-link.com/onbarcode.zip
        unzip onbarcode.zip -d lib/
      displayName: 'Download OnBarcode'

    - task: DotNetCoreCLI@2
      inputs:
        command: build

    # Challenges:
    # - Download links may expire
    # - Authentication needed
    # - Versioning manual
    # - No caching benefits
");

            Console.WriteLine("\nOnBarcode (NuGet Approach):\n");
            Console.WriteLine(@"
    # azure-pipelines.yml

    steps:
    - checkout: self

    - task: DotNetCoreCLI@2
      inputs:
        command: restore  # NuGet packages restored

    - task: DotNetCoreCLI@2
      inputs:
        command: build

    # Two restores still produce two packages and two licenses to manage
");

            Console.WriteLine("\nIronBarcode (NuGet, Single Package):\n");
            Console.WriteLine(@"
    # azure-pipelines.yml

    steps:
    - checkout: self

    - task: DotNetCoreCLI@2
      inputs:
        command: restore  # Standard NuGet restore

    - task: DotNetCoreCLI@2
      inputs:
        command: build

    # Benefits:
    # - Standard workflow
    # - NuGet cache works
    # - Version lock files
    # - Reproducible builds
    # - Single package covers generation and reading
");
        }

        /// <summary>
        /// Demonstrates Docker deployment differences
        /// </summary>
        public void DemonstrateDockerDifferences()
        {
            Console.WriteLine("=== Docker Deployment Comparison ===\n");

            Console.WriteLine("OnBarcode (Manual DLL - must include in image):\n");
            Console.WriteLine(@"
    # Dockerfile

    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src

    # Must copy DLLs into container
    COPY lib/ /src/lib/

    COPY . .
    RUN dotnet restore
    RUN dotnet publish -c Release -o /app

    FROM mcr.microsoft.com/dotnet/aspnet:8.0
    WORKDIR /app

    # DLLs must be in publish output
    COPY --from=build /app .

    ENTRYPOINT [""dotnet"", ""MyApp.dll""]

    # Challenges:
    # - DLLs must exist before build
    # - Image includes binaries
    # - Updates require rebuild
");

            Console.WriteLine("\nOnBarcode (NuGet - standard approach):\n");
            Console.WriteLine(@"
    # Dockerfile

    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src

    COPY *.csproj .
    RUN dotnet restore  # NuGet packages downloaded

    COPY . .
    RUN dotnet publish -c Release -o /app

    FROM mcr.microsoft.com/dotnet/aspnet:8.0
    WORKDIR /app
    COPY --from=build /app .

    ENTRYPOINT [""dotnet"", ""MyApp.dll""]

    # Standard pattern
");

            Console.WriteLine("\nIronBarcode (Same NuGet pattern, more proven):\n");
            Console.WriteLine(@"
    # Dockerfile

    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src

    # Standard restore/build/publish
    COPY *.csproj .
    RUN dotnet restore

    COPY . .
    RUN dotnet publish -c Release -o /app

    FROM mcr.microsoft.com/dotnet/aspnet:8.0
    WORKDIR /app
    COPY --from=build /app .

    # License via environment variable
    ENV IRONBARCODE_LICENSE=your-key-here

    ENTRYPOINT [""dotnet"", ""MyApp.dll""]

    # Benefits:
    # - Standard Docker pattern
    # - License key as env var (12-factor)
    # - No files to manage
    # - Works in all container orchestrators
");
        }

        /// <summary>
        /// Demonstrates download count comparison
        /// </summary>
        public void DemonstrateDownloadComparison()
        {
            Console.WriteLine("=== NuGet Download Counts (Maturity Indicator) ===\n");

            Console.WriteLine("Package download counts indicate adoption and trust:\n");

            Console.WriteLine("IronBarcode:");
            Console.WriteLine("  - Package: BarCode (NuGet)");
            Console.WriteLine("  - Distribution: NuGet only");
            Console.WriteLine("  - Conclusion: Single package covers generation and reading\n");

            Console.WriteLine("OnBarcode:");
            Console.WriteLine("  - Package: OnBarcode.Barcode.Generator (10.5.x)");
            Console.WriteLine("  - Plus: OnBarcode.Barcode.Reader, framework variants, PDF add-on");
            Console.WriteLine("  - Distribution: NuGet and direct DLL download both supported");
            Console.WriteLine("  - Conclusion: Multiple packages and tiers to choose from\n");

            Console.WriteLine("What download counts indicate:");
            Console.WriteLine("- More downloads = more real-world testing");
            Console.WriteLine("- Edge cases discovered and fixed");
            Console.WriteLine("- Community familiarity\n");
        }

        /// <summary>
        /// Summary comparison
        /// </summary>
        public void PrintSummary()
        {
            Console.WriteLine("=== Summary: Distribution Model Comparison ===\n");

            Console.WriteLine("| Aspect                  | OnBarcode (Manual DLL) | OnBarcode (NuGet)        | IronBarcode (BarCode) |");
            Console.WriteLine("|-------------------------|------------------------|--------------------------|-----------------------|");
            Console.WriteLine("| Distribution            | DLL ZIP from website   | nuget.org                | nuget.org             |");
            Console.WriteLine("| Version management      | Manual                 | Automatic                | Automatic             |");
            Console.WriteLine("| CI/CD support           | Complex                | Standard                 | Standard              |");
            Console.WriteLine("| Docker friendly         | Manual                 | Standard                 | Standard              |");
            Console.WriteLine("| Packages required       | 1+ DLLs                | 2+ (Generator + Reader)  | 1                     |");
            Console.WriteLine("| Documentation focus     | Manual setup           | NuGet + manual paths     | NuGet only            |");
            Console.WriteLine();

            Console.WriteLine("Recommendation:");
            Console.WriteLine("- OnBarcode's NuGet packages remove most manual-setup pain");
            Console.WriteLine("- Reader is still a separate package and separate purchase");
            Console.WriteLine("- IronBarcode consolidates generation + reading into one package");
            Console.WriteLine("- Choose IronBarcode when both capabilities are needed\n");
        }
    }

    /// <summary>
    /// Demonstrates complete setup comparison
    /// </summary>
    public class CompleteSetupComparison
    {
        /// <summary>
        /// Shows side-by-side new project setup
        /// </summary>
        public void DemonstrateNewProjectSetup()
        {
            Console.WriteLine("=== New Project Setup: Complete Comparison ===\n");

            Console.WriteLine("Setting up a new project with barcode capabilities:\n");

            Console.WriteLine("OnBarcode (Generation + Reading):\n");
            Console.WriteLine(@"
    # Step 1: Create project
    dotnet new console -n MyBarcodeApp
    cd MyBarcodeApp

    # Step 2: Install generator
    dotnet add package OnBarcode.Barcode.Generator

    # Step 3: Install reader (separate package and separate purchase!)
    dotnet add package OnBarcode.Barcode.Reader

    # Step 4: Configure TWO licenses
    using OnBarcode.Barcode;
    License.RegisterLicense(""GENERATOR-KEY"");
    License.RegisterLicense(""READER-KEY"");

    # Step 5: Write code (two different APIs)
    # ... see separate-reader example

    # Total packages: 2
    # Total licenses: 2
    # APIs to learn: 2
");

            Console.WriteLine("\nIronBarcode (Generation + Reading):\n");
            Console.WriteLine(@"
    # Step 1: Create project
    dotnet new console -n MyBarcodeApp
    cd MyBarcodeApp

    # Step 2: Install IronBarcode (NuGet package id is ""BarCode"")
    dotnet add package BarCode

    # Step 3: Configure license
    IronBarCode.License.LicenseKey = ""YOUR-KEY"";

    # Step 4: Write code (unified API)
    var barcode = BarcodeWriter.CreateBarcode(""data"", BarcodeEncoding.Code128);
    barcode.SaveAsPng(""barcode.png"");

    var results = BarcodeReader.Read(""barcode.png"");

    # Total packages: 1
    # Total licenses: 1
    # APIs to learn: 1
");

            Console.WriteLine("\nTime comparison:");
            Console.WriteLine("- OnBarcode: ~15 minutes (two packages, two licenses, two APIs)");
            Console.WriteLine("- IronBarcode: ~5 minutes (one of everything)\n");
        }
    }

    /// <summary>
    /// Program entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var setupDemo = new ManualSetupDemo();

            setupDemo.DemonstrateHistoricalSetup();
            setupDemo.DemonstrateManualUpgrade();
            setupDemo.DemonstrateNuGetTransition();
            setupDemo.DemonstrateIronBarcodeModern();
            setupDemo.DemonstrateCICDImplications();
            setupDemo.DemonstrateDockerDifferences();
            setupDemo.DemonstrateDownloadComparison();
            setupDemo.PrintSummary();

            var completeDemo = new CompleteSetupComparison();
            completeDemo.DemonstrateNewProjectSetup();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
