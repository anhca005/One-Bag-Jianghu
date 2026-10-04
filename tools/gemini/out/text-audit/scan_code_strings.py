import os
import re
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
SCRIPTS_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Scripts"
EDITOR_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Editor" / "Building"

# Match string literals in C# code
string_pattern = re.compile(r'"([^"\\]*(?:\\.[^"\\]*)*)"')

def check_file(filepath):
    rel_path = filepath.relative_to(ROOT)
    try:
        content = filepath.read_text(encoding="utf-8")
    except Exception as e:
        return
    
    lines = content.splitlines()
    for lno, line in enumerate(lines, 1):
        stripped = line.strip()
        # Skip comments
        if stripped.startswith("//") or stripped.startswith("/*") or stripped.startswith("*"):
            continue
        # Skip Debug.Log / LogWarning / LogError
        if "Debug.Log" in line or "Debug.LogWarning" in line or "Debug.LogError" in line:
            continue
        
        # Check string literals
        matches = string_pattern.findall(line)
        for m in matches:
            # unescape
            s = m.encode().decode('unicode_escape', errors='ignore') if '\\' in m else m
            
            # 1. Raw snake_case id (e.g. han_thiet, o_thiet, etc)
            if re.match(r'^[a-z]+_[a-z0-9_]+$', s) and len(s) > 4:
                # check if it's assigned to text, or in Fail/Message/Format
                if any(k in line for k in ["text", "Text", "Fail", "Format", "Add", "options", "Show", "Title", "Message"]):
                    print(f"[RAW ID] {rel_path}:{lno} -> \"{s}\" in: {stripped[:100]}")
            
            # 2. English enum or uppercase string (e.g. "QI", "BASIC", "WEAPON")
            if re.match(r'^[A-Z][A-Z0-9_]{1,15}$', s) and s not in ["OK", "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "N0", "UTF8", "RGB", "RGBA"]:
                if any(k in line for k in ["text", "Text", "Format", "Append", "return", "case", "default"]):
                    print(f"[UPPERCASE/ENUM] {rel_path}:{lno} -> \"{s}\" in: {stripped[:100]}")

            # 3. Terminology: "sinh lực" instead of "khí huyết"
            if re.search(r'\bsinh lực\b', s, re.IGNORECASE):
                print(f"[TERM: sinh lực -> khí huyết] {rel_path}:{lno} -> \"{s}\"")

            # 4. Terminology: "hồi phục" for regen
            if "hồi phục" in s.lower() and "điều tức" not in s.lower():
                # check context
                if any(k in line for k in ["Format", "Text", "Describe", "Status"]):
                    print(f"[TERM: hồi phục -> Điều Tức?] {rel_path}:{lno} -> \"{s}\"")

            # 5. Length risk:
            # Button caption > 14 chars
            if ("Button" in line or "btn" in line.lower()) and len(s) > 14 and not s.startswith("Assets") and not s.startswith("http"):
                # Could be a button caption
                if any(k in line for k in ["UiButton", "Button(", "\"Confirm\"", "\"Cancel\"", "\"Upgrade\"", "\"Merge\"", "\"Sort\""]):
                    print(f"[BUTTON LENGTH > 14] {rel_path}:{lno} -> \"{s}\" ({len(s)})")

            # 6. Check single line description > 90
            if len(s) > 90 and not s.startswith("Assets") and not s.startswith("http") and not line.strip().startswith("using"):
                print(f"[STRING LENGTH > 90] {rel_path}:{lno} -> length {len(s)}: \"{s[:50]}...\"")

print("=== SCANNING SCRIPTS ===")
for p in SCRIPTS_DIR.rglob("*.cs"):
    check_file(p)

print("\n=== SCANNING EDITOR BUILDERS ===")
for p in EDITOR_DIR.rglob("*.cs"):
    check_file(p)
