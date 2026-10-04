import re
from pathlib import Path

txt_path = Path("tools/gemini/out/round-2/test_matrix_out.txt")
if not txt_path.exists():
    print("test_matrix_out.txt does not exist.")
    exit()

with open(txt_path, "r", encoding="utf-8", errors="replace") as f:
    lines = f.readlines()

rows = []
outside_total = None
header = None

for line in lines:
    if "Loadout" in line:
        header = line.strip()
    elif "Outside [" in line:
        outside_total = line.strip()
    elif "%" in line and not line.startswith("-"):
        parts = line.split()
        # Find all percentages
        pcts = [p.replace("%", "").replace("!", "") for p in parts if "%" in p]
        if len(pcts) == 8:
            vals = [float(p) for p in pcts]
            name = line[:40].strip()
            avg = sum(vals) / len(vals)
            out_cnt = sum(1 for v in vals if v < 35.0 or v > 65.0)
            rows.append((name, avg, out_cnt, vals))

print(f"Parsed {len(rows)} loadouts.")
if outside_total:
    print(f"Total: {outside_total}")

print("\n=== BUILD AVERAGES ===")
bad_builds = []
for name, avg, out_cnt, vals in rows:
    flag = ""
    if avg < 30.0 or avg > 70.0:
        flag = " [FAIL: avg out of [30, 70]]"
        bad_builds.append((name, avg))
    safe_name = name.encode("ascii", "replace").decode("ascii")
    print(f"{safe_name:40}: avg={avg:5.1f}% (outside {out_cnt}/8){flag}")

if bad_builds:
    print(f"\nFAILING BUILDS ({len(bad_builds)}):")
    for b, a in bad_builds:
        safe_b = b.encode("ascii", "replace").decode("ascii")
        print(f"  - {safe_b}: {a:.1f}%")
else:
    print("\nALL 24 BUILDS PASS THE [30, 70] AVERAGE THRESHOLD!")
