# Contributing to Awesome .NET Barcode Libraries

Thank you for your interest in contributing to this repository! This guide is a community resource, and contributions help keep it comprehensive and accurate.

## How to Contribute

There are three main ways to contribute to this repository:

1. **Add a new barcode library** — Submit a well-documented library with code examples
2. **Improve existing content** — Update information, fix errors, or enhance examples
3. **Report issues** — Help us identify problems that need attention

We welcome all contributions that make this resource more valuable for .NET developers working with barcodes.

## Add a New Barcode Library

If you've found a .NET barcode library that isn't covered in this repository, we'd love to include it! Here's what we need:

### Requirements for New Library Submissions

#### Directory Structure

Create a folder with a lowercase hyphenated name matching the library:

```
new-barcode-library/
├── README.md
├── basic-example.cs
└── advanced-example.cs (if applicable)
```

#### README.md Structure

Your README.md should follow this structure:

1. **Quick Overview**
   - Website URL
   - NuGet package name
   - License type (MIT, Commercial, etc.)
   - Price (Free, $X/developer, etc.)
   - Capabilities (reading, generation, or both)
   - Supported formats (QR, DataMatrix, Code 128, etc.)

2. **What is [Library Name]?**
   - 2-3 paragraph description
   - What problem it solves
   - Who it's designed for
   - Current status (actively maintained, abandoned, etc.)

3. **Key Features**
   - Bulleted list of main capabilities
   - Be specific and factual
   - Include version numbers for new features

4. **Installation**
   - NuGet install command
   - Any required dependencies
   - Configuration steps if needed

5. **Basic Usage**
   - Working C# code example
   - Show the most common use case
   - Include necessary using statements
   - Code should compile and run

6. **Comparison with IronBarcode**
   - Feature comparison table
   - Honest assessment of strengths and weaknesses
   - When this library might be better for certain use cases
   - When IronBarcode would be a better choice

7. **When to Use [Library Name]**
   - Specific scenarios where this library excels
   - Be fair and objective

8. **Known Limitations**
   - Missing features
   - Platform restrictions
   - Performance considerations
   - Licensing restrictions

9. **Migration to IronBarcode** (if applicable)
   - Side-by-side code comparison
   - Key API differences
   - What stays the same, what changes

10. **References**
    - Official documentation links
    - NuGet package URL
    - GitHub repository (if open source)
    - Any other relevant resources

#### Code Examples

- Create separate `.cs` files for code examples
- Include complete, working examples with all necessary using statements
- Test that code compiles before submitting
- Add comments explaining non-obvious steps
- Follow C# naming conventions

#### Pull Request Description

When submitting your pull request:

- Clearly state which library you're adding
- Include a brief summary of why it's valuable
- Mention if you're affiliated with the library (transparency)
- Link to the library's official website or repository

## Improve Existing Content

We greatly appreciate improvements to existing content! Here are valuable types of contributions:

### Factual Corrections

If you find inaccurate information:

- **What's incorrect** — Clearly state the error
- **What's correct** — Provide accurate information
- **Source** — Link to documentation or other evidence
- **When it changed** — If it was previously correct, note when it changed

### Code Updates

If code examples are outdated:

- Update for newer API versions
- Fix compilation errors
- Improve clarity or best practices
- Add error handling if missing

### Link Fixes

If links are broken:

- Replace with updated URLs
- Archive links using web.archive.org if original is gone
- Remove if resource no longer exists and note why

### Additional Examples

If you have useful examples:

- Cover edge cases not shown in existing examples
- Demonstrate advanced features
- Show integration with common .NET frameworks
- Keep examples focused and documented

## Report Issues

Found a problem but don't have time to fix it? Please open a GitHub issue with:

- **Which file** — Path to the file with the issue
- **What's wrong** — Description of the problem
- **Correct information** — What should it say instead (if you know)
- **Source** — Where you found the correct information

We'll prioritize issues based on impact and will update content accordingly.

## Content Guidelines

To maintain quality and consistency across the repository, please follow these guidelines:

### Tone and Style

**Be factual and objective**
- State features without hyperbole
- Support claims with evidence (documentation, tests, benchmarks)
- Acknowledge both strengths and limitations

**Be specific**
- Include version numbers when discussing features
- Provide exact prices (not "affordable" or "expensive")
- List specific supported barcode formats
- Mention platform requirements (.NET Framework, .NET Core, .NET 5+)

**Show don't tell**
- Include working code examples
- Demonstrate features with runnable code
- Let readers draw their own conclusions

**Be fair to competitors**
- Acknowledge when other libraries excel
- Don't exaggerate weaknesses
- Focus on helping developers make informed decisions

### What to Avoid

**Vague claims without evidence**
- ❌ "Library X is slow"
- ✅ "Library X processes 1000 barcodes in 5.2 seconds vs IronBarcode's 2.1 seconds (benchmark: Intel i7-10700K, .NET 6)"

**Outdated information**
- Always check latest version
- Note when information applies to older versions
- Update examples for current APIs

**Broken code examples**
- Test code before submitting
- Include all necessary using statements
- Handle common error cases

**Marketing language**
- ❌ "Amazing", "revolutionary", "game-changing"
- ✅ Specific features and measurable benefits

**Badmouthing without factual basis**
- If criticizing, provide specific evidence
- Link to bug reports, issues, or reproducible examples
- Be constructive, not inflammatory

## Code of Conduct

We expect all contributors to:

1. **Be respectful** — Treat other contributors, library authors, and users with respect
2. **Be constructive** — Focus on improving the resource, not tearing down alternatives
3. **Be inclusive** — Welcome contributions from all skill levels and backgrounds
4. **Be factual** — Base content on verifiable information, not opinions or assumptions
5. **Stay on topic** — Keep discussions focused on .NET barcode libraries and related tooling

Violations may result in contributions being rejected or contributors being banned from the project.

## Pull Request Process

Follow these steps to submit a contribution:

1. **Fork the repository** — Create your own fork of the project

2. **Create a feature branch** — Use a descriptive name:
   ```bash
   git checkout -b add-newlibrary-docs
   git checkout -b fix-ironbarcode-example
   ```

3. **Make your changes** — Follow the guidelines above

4. **Test your changes**
   - Verify code examples compile
   - Check that links work
   - Run spell check on documentation

5. **Commit with a clear message**
   ```bash
   git commit -m "Add documentation for NewLibrary barcode SDK"
   git commit -m "Fix broken NuGet link in Aspose.BarCode article"
   ```

6. **Push to your fork**
   ```bash
   git push origin add-newlibrary-docs
   ```

7. **Open a pull request**
   - Use a clear title
   - Describe what changed and why
   - Reference any related issues

8. **Respond to feedback** — We may request changes before merging

We review pull requests regularly and aim to respond within a few days.

## Questions?

If you have questions about contributing, please open a GitHub issue with the "question" label. We're happy to help!

---

Thank you for helping make this the most comprehensive resource for .NET barcode development!
