import json
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

findings = json.load(open('tools/gemini/out/text-audit/code_findings.json', encoding='utf-8'))
print(f"Total findings: {len(findings)}")
for idx, f in enumerate(findings):
    print(f"{idx+1}. [{f['category'].upper()}] {f['file']}:{f['line']} -> {f['issue']}: '{f['old']}'")
