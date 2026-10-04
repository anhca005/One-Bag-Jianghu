import re
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
ASSETS_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game"

print("=== CHECKING NUMBER FORMATTING IN CODE & DATA ===")

dot_numbers = []
comma_numbers = []

for p in ASSETS_DIR.rglob("*.*"):
    if p.suffix in [".cs", ".json", ".prefab"]:
        try:
            content = p.read_text(encoding="utf-8")
        except Exception:
            continue
        rel = p.relative_to(ROOT)
        for lno, line in enumerate(content.splitlines(), 1):
            if "Debug.Log" in line:
                continue
            # Look for 1.000 or 10.000 in strings
            m_dot = re.findall(r'\b\d{1,3}\.\d{3}\b', line)
            for d in m_dot:
                dot_numbers.append((str(rel), lno, d, line.strip()))
            m_comma = re.findall(r'\b\d{1,3},\d{3}\b', line)
            for c in m_comma:
                comma_numbers.append((str(rel), lno, c, line.strip()))

print(f"Dot thousands count (e.g. 10.000): {len(dot_numbers)}")
for f, l, num, ctx in dot_numbers[:10]:
    print(f"  [DOT] {f}:{l} -> {num} in '{ctx[:80]}'")

print(f"Comma thousands count (e.g. 10,000): {len(comma_numbers)}")
for f, l, num, ctx in comma_numbers[:10]:
    print(f"  [COMMA] {f}:{l} -> {num} in '{ctx[:80]}'")
