import os
import re
import sys
import json
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
SCRIPTS_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Scripts"
EDITOR_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Editor" / "Building"

TERMS_TO_CHECK = [
    (r'\bsinh lực\b', "Sinh Lực", "Khí Huyết"),
    (r'\bhồi phục\b', "Hồi Phục", "Điều Tức / Liệu Thương"),
    (r'\bbạo kích\b', "Bạo Kích", "Hội Tâm"),
    (r'\bchí mạng\b', "Chí Mạng", "Hội Tâm"),
    (r'\bkhiên\b', "Khiên", "Cương Khí"),
    (r'\btúi đồ\b', "Túi Đồ", "Bao Phục"),
    (r'\bkho đồ\b', "Kho Đồ", "Tàng Bảo Các"),
    (r'\bcửa hàng\b', "Cửa Hàng", "Thương Hội"),
    (r'\blực chiến\b', "Lực Chiến", "Chiến Uy"),
    (r'\bđấu trường\b', "Đấu Trường", "Luận Kiếm Đài"),
    (r'\bnhân vật\b', "Nhân Vật", "Hiệp Khách"),
    (r'\bvõ hiệp\b', "Võ Hiệp", "Hiệp Khách"),
    (r'\bgiảm sát thương\b', "Giảm Sát Thương", "Hộ Thân"),
]

code_findings = []

def scan_cs_file(filepath):
    rel_path = filepath.relative_to(ROOT)
    try:
        content = filepath.read_text(encoding="utf-8")
    except Exception:
        return
    
    lines = content.splitlines()
    for lno, line in enumerate(lines, 1):
        stripped = line.strip()
        if stripped.startswith("//") or stripped.startswith("/*") or stripped.startswith("*"):
            continue
        if "Debug.Log" in line or "Debug.LogWarning" in line or "Debug.LogError" in line:
            continue
        
        # Check string literals
        matches = re.findall(r'"([^"\\]*(?:\\.[^"\\]*)*)"', line)
        for s in matches:
            s_clean = s.encode().decode('unicode_escape', errors='ignore') if '\\' in s else s
            if not s_clean:
                continue
            
            # Check terminology
            for pattern, bad_term, good_term in TERMS_TO_CHECK:
                if re.search(pattern, s_clean, re.IGNORECASE):
                    code_findings.append({
                        "file": str(rel_path),
                        "line": lno,
                        "category": "terminology",
                        "issue": f"Dùng '{bad_term}' thay vì '{good_term}'",
                        "old": s_clean,
                        "context": stripped
                    })
            
            # Check raw ID leakage
            if re.match(r'^[a-z]+_[a-z0-9_]+$', s_clean) and len(s_clean) > 4:
                if any(k in line for k in ["text", "Text", "Fail", "Format", "Add", "options", "Show", "Title", "Message"]):
                    code_findings.append({
                        "file": str(rel_path),
                        "line": lno,
                        "category": "raw_id",
                        "issue": f"Raw snake_case id '{s_clean}' có thể hiển thị ra màn hình",
                        "old": s_clean,
                        "context": stripped
                    })
            
            # Check UPPERCASE enum leakage
            if re.match(r'^[A-Z][A-Z0-9_]{1,15}$', s_clean) and s_clean not in ["OK", "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "N0", "UTF8", "RGB", "RGBA"]:
                if any(k in line for k in ["text", "Text", "Format", "Append", "return", "case", "default"]):
                    code_findings.append({
                        "file": str(rel_path),
                        "line": lno,
                        "category": "raw_id",
                        "issue": f"Enum/Raw English '{s_clean}' có thể hiển thị ra màn hình",
                        "old": s_clean,
                        "context": stripped
                    })

            # Check Button length > 14
            if ("Button" in line or "btn" in line.lower() or "Button(" in line) and len(s_clean) > 14:
                # filter out prefab paths or method names
                if not s_clean.startswith("Assets") and not s_clean.startswith("http") and not s_clean.endswith(".prefab") and not s_clean.startswith("Button_"):
                    if any(k in line for k in ["UiButton", "Button(", "Text(", "\"Confirm\"", "\"Cancel\"", "button"]):
                        code_findings.append({
                            "file": str(rel_path),
                            "line": lno,
                            "category": "length",
                            "issue": f"Nút bấm quá dài ({len(s_clean)} ký tự > 14)",
                            "old": s_clean,
                            "context": stripped
                        })

print("Scanning C# scripts...")
for p in SCRIPTS_DIR.rglob("*.cs"):
    scan_cs_file(p)

for p in EDITOR_DIR.rglob("*.cs"):
    scan_cs_file(p)

print(f"Total code findings: {len(code_findings)}")
with open("tools/gemini/out/text-audit/code_findings.json", "w", encoding="utf-8") as f:
    json.dump(code_findings, f, indent=2, ensure_ascii=False)
