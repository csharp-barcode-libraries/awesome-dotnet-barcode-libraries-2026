const fs = require('fs');
const path = require('path');

const dirs = [
  // Plan 1
  'accusoft-barcode', 'aspose-barcode', 'barcode4net', 'barcodelib',
  'barcoder', 'barcodescanning-maui', 'barkoder-sdk',
  // Plan 2
  'leadtools-barcode', 'messagingtoolkit-barcode', 'neodynamic-barcode',
  'netbarcode', 'onbarcode', 'qrcodernet', 'scanbot-sdk', 'scandit-sdk',
  'spire-barcode', 'syncfusion-barcode', 'telerik-barcode', 'zxing-net',
  'zxing-net-maui'
];

const contractionPattern = /\b(don't|doesn't|isn't|aren't|wasn't|weren't|can't|couldn't|wouldn't|shouldn't|won't|haven't|hasn't|hadn't|there's|that's|it's|I'm|I've|I'll|I'd|you're|you've|you'll|you'd|we're|we've|we'll|we'd|they're|they've|they'll|they'd|he's|she's|let's)\b/gi;

let issues = [];

for (const dir of dirs) {
  const files = fs.readdirSync(dir).filter(f => f.endsWith('.md') && (f.startsWith('compare-') || f.startsWith('migrate-')));
  for (const file of files) {
    const fullPath = path.join(dir, file);
    const content = fs.readFileSync(fullPath, 'utf8');
    const lines = content.split('\n');

    // Check for --- separators (standalone)
    lines.forEach((line, i) => {
      if (/^---\s*$/.test(line)) {
        issues.push({ file: fullPath, line: i+1, type: 'separator', text: line });
      }
    });

    // Check for contractions OUTSIDE code blocks
    let inCode = false;
    lines.forEach((line, i) => {
      if (line.startsWith('```')) inCode = !inCode;
      if (!inCode) {
        const matches = line.match(contractionPattern);
        if (matches) {
          issues.push({ file: fullPath, line: i+1, type: 'contraction', text: line.trim().substring(0, 100) });
        }
      }
    });

    // Check for bylines
    lines.forEach((line, i) => {
      if (/^\*By /.test(line) || /^\*\*By /.test(line)) {
        issues.push({ file: fullPath, line: i+1, type: 'byline', text: line.trim() });
      }
    });

    // Check for Related Comparisons
    lines.forEach((line, i) => {
      if (/Related Comparisons/i.test(line)) {
        issues.push({ file: fullPath, line: i+1, type: 'related-comparisons', text: line.trim() });
      }
    });

    // Check for Last verified
    lines.forEach((line, i) => {
      if (/Last verified/i.test(line)) {
        issues.push({ file: fullPath, line: i+1, type: 'last-verified', text: line.trim() });
      }
    });
  }
}

if (issues.length === 0) {
  console.log('ALL CLEAN — zero issues found across all 40 articles.');
} else {
  console.log('ISSUES FOUND: ' + issues.length);
  for (const issue of issues) {
    console.log('[' + issue.type + '] ' + issue.file + ':' + issue.line + ' => ' + issue.text);
  }
}
