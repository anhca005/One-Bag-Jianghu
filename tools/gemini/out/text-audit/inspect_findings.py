import json
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open('tools/gemini/out/text-audit/code_findings.json', 'r', encoding='utf-8') as f:
    findings = json.load(f)

for idx, it in enumerate(findings[:25]):
    print(f"[{idx+1}] {it.get('file')}:{it.get('line')} - {it.get('category')}: {it.get('issue')}")
