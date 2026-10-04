import re
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
ASSETS_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game"

button_labels = []

# Find SetLabel, Button(..., "..."), UiBuild.Button(..., "..."), etc.
patterns = [
    r'SetLabel\s*\(\s*"([^"]+)"',
    r'Button\s*\([^,]+,\s*"[^"]*",\s*"([^"]+)"',
    r'UiBuild\.Button\s*\([^,]+,\s*"[^"]*",\s*"([^"]+)"',
    r'new\s+ConfirmPopupData\s*\([^,]+,\s*[^,]+,\s*"([^"]+)",\s*"([^"]+)"',
]

for p in ASSETS_DIR.rglob("*.cs"):
    try:
        content = p.read_text(encoding="utf-8")
    except Exception:
        continue
    rel = p.relative_to(ROOT)
    for lno, line in enumerate(content.splitlines(), 1):
        if line.strip().startswith("//") or "Debug.Log" in line:
            continue
        
        # Check SetLabel
        m = re.search(r'SetLabel\s*\(\s*([^)]+)\)', line)
        if m:
            arg = m.group(1).strip()
            button_labels.append((str(rel), lno, arg, "SetLabel"))
            
        # Check Button(panel, "Name", "Caption"...)
        m2 = re.search(r'Button\s*\(\s*[^,]+,\s*"[^"]+",\s*"([^"]+)"', line)
        if m2:
            caption = m2.group(1).strip()
            button_labels.append((str(rel), lno, caption, "BuilderButton"))
            
        # Check ConfirmPopupData
        m3 = re.search(r'new\s+ConfirmPopupData\s*\(\s*[^,]+,\s*[^,]+,\s*"([^"]+)",\s*"([^"]+)"', line)
        if m3:
            button_labels.append((str(rel), lno, m3.group(1).strip(), "ConfirmYes"))
            button_labels.append((str(rel), lno, m3.group(2).strip(), "ConfirmNo"))

print(f"Total button labels detected: {len(button_labels)}")
for file, lno, text, kind in button_labels:
    clean_text = text.replace('"', '').replace('$', '')
    # Check if length > 14
    if len(clean_text) > 14:
        print(f"[BUTTON > 14] {file}:{lno} ({len(clean_text)} chars) -> '{clean_text}' [{kind}]")
    else:
        print(f"  [BUTTON OK] {file}:{lno} ({len(clean_text)} chars) -> '{clean_text}' [{kind}]")
