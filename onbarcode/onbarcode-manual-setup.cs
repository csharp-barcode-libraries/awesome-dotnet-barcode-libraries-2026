/*
 * OnBarcode Historical DLL Distribution vs IronBarcode Modern Distribution
 *
 * This example demonstrates the evolution of OnBarcode's distribution model
 * from manual DLL downloads to NuGet packages, and compares it to
 * IronBarcode's always-modern NuGet-first approach.
 *
 * Key differences demonstrated:
 * 1. Historical manual DLL management
 * 2. Recent NuGet adoption
 * 3. Always-modern vs transitional distribution
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
            Console.WriteLine("=== OnBarcode Historical Distribution (Pre-2025) ===\n");

            Console.WriteLine("Before NuGet packages were available, OnBarcode required manual setup:\n");

            Console.WriteLine("Step 1: Purchase and Download");
            Console.WriteLine(@"
    1. Visit OnBarcode.com
    2. Contact sales for pricing
    3. Purchase license
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
            Console.WriteLine("=== OnBarcode NuGet Packages (Recent Addition 2025-2026) ===\n");

            Console.WriteLine("OnBarcode has recently added NuGet distribution:\n");

            Console.WriteLine("Available Packages:");
            Console.WriteLine(@"
    OnBarcode.Barcode.Generator   <- Barcode generation
    OnBarcode.QRCode              <- QR code specific
    OnBarcode.Barcode.Reader      <- Separate reading (separate purchase!)
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
            Console.WriteLine("- Newer packages may have fewer downloads");
            Console.WriteLine("- Documentation may reference old manual approach");
            Console.WriteLine("- Reader still requires separate purchase");
            Console.WriteLine("- Transition period confusion\n");
        }

        /// <summary>
        /// Demonstrates IronBarcode's always-modern approach
        /// </summary>
        public void DemonstrateIronBarcodeModern()
        {
            Console.WriteLine("=== IronBarcode Distribution (NuGet-First Since Launch) ===\n");

            Console.WriteLine("IronBarcode has been NuGet-first from the beginning:\n");

            Console.WriteLine("Installation:");
            Console.WriteLine(@"
    # Single command
    dotnet add package IronBarcode

    # That's it. No manual steps.
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
    dotnet add package IronBarcode

    # Or specify version
    dotnet add package IronBarcode --version 2024.1.0

    # Check for updates
    dotnet list package --outdated
");

            Console.WriteLine("Team Synchronization:");
            Console.WriteLine(@"
    # In .csproj - version tracked as text
    <PackageReference Include=""IronBarcode"" Version=""2024.1.0"" />

    # Any team member runs:
    dotnet restore

    # Everyone has same version
");

            Console.WriteLine("\nBenefits of NuGet-first:");
            Console.WriteLine("- 2.1M+ downloads validates reliability");
            Console.WriteLine("- Mature package, extensive testing");
            Console.WriteLine("- Documentation assumes NuGet workflow");
            Console.WriteLine("- No legacy manual process baggage\n");
        }

        /// <summary>
        /// Demonstrates CI/CD implications
        /// </summary>
        public void DemonstrateCICDImplications()
        {
            Console.WriteLine("=== CI/CD Pipeline Comparison ===\n");

            Console.WriteLine("OnBarcode (Historical Manual Approach):\n");
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

            Console.WriteLine("\nOnBarcode (New NuGet Approach):\n");
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

    # Better, but:
    # - Package is newer, less battle-tested in CI
    # - May have compatibility issues in transition
");

            Console.WriteLine("\nIronBarcode (NuGet-First):\n");
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
    # - Millions of successful restores
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
            Console.WriteLine("  - Package: IronBarcode");
            Console.WriteLine("  - Downloads: 2.1M+ total");
            Console.WriteLine("  - Years on NuGet: Many");
            Console.WriteLine("  - Conclusion: Battle-tested, widely adopted\n");

            Console.WriteLine("OnBarcode:");
            Console.WriteLine("  - Package: OnBarcode.Barcode.Generator");
            Console.WriteLine("  - Downloads: Lower (newer packages)");
            Console.WriteLine("  - NuGet presence: Recent (2025-2026)");
            Console.WriteLine("  - Conclusion: Transitioning, less proven via NuGet\n");

            Console.WriteLine("What download counts indicate:");
            Console.WriteLine("- More downloads = more real-world testing");
            Console.WriteLine("- Edge cases discovered and fixed");
            Console.WriteLine("- Community familiarity");
            Console.WriteLine("- Stack Overflow answers available");
            Console.WriteLine("- Tutorial availability\n");
        }

        /// <summary>
        /// Summary comparison
        /// </summary>
        public void PrintSummary()
        {
            Console.WriteLine("=== Summary: Distribution Model Comparison ===\n");

            Console.WriteLine("| Aspect                  | OnBarcode (Historical) | OnBarcode (New)    | IronBarcode        |");
            Console.WriteLine("|-------------------------|------------------------|--------------------|--------------------|");
            Console.WriteLine("| Distribution            | Manual DLL             | NuGet              | NuGet              |");
            Console.WriteLine("| Available since         | Legacy                 | 2025-2026          | Years              |");
            Console.WriteLine("| Version management      | Manual                 | Automatic          | Automatic          |");
            Console.WriteLine("| CI/CD support           | Complex                | Standard           | Standard           |");
            Console.WriteLine("| Docker friendly         | Manual                 | Standard           | Standard           |");
            Console.WriteLine("| NuGet downloads         | N/A                    | Lower (newer)      | 2.1M+              |");
            Console.WriteLine("| Documentation focus     | Manual setup           | Transitioning      | NuGet workflow     |");
            Console.WriteLine();

            Console.WriteLine("Recommendation:");
            Console.WriteLine("- OnBarcode's NuGet packages are welcome but newer");
            Console.WriteLine("- Historical manual approach still in documentation");
            Console.WriteLine("- IronBarcode's NuGet-first approach is more proven");
            Console.WriteLine("- Choose IronBarcode for modern deployment workflows\n");
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

    # Step 3: Install reader (separate package!)
    dotnet add package OnBarcode.Barcode.Reader

    # Step 4: Configure TWO licenses
    OnBarcode.Barcode.License.SetLicense(""GENERATOR-KEY"");
    OnBarcode.Barcode.Reader.License.SetLicense(""READER-KEY"");

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

    # Step 2: Install IronBarcode
    dotnet add package IronBarcode

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
