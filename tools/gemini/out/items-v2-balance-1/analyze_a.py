import re
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")


text = Path("tools/gemini/out/items-v2-balance-1/baseline_a.txt").read_text(encoding="utf-8")
lines = [l for l in text.splitlines() if "%" in l and not l.startswith("Loadout")]

print(f"{'Build':42} | {'Avg':6} | {'Min':6} | {'Max':6} | Out cells")
print("-" * 75)
for line in lines:
    parts = line.split()
    name = " ".join(parts[:-8])
    # strip trailing %! or %
    vals = [float(v.rstrip("%!")) for v in parts[-8:]]
    avg = sum(vals) / len(vals)
    min_v = min(vals)
    max_v = max(vals)
    out_cnt = sum(1 for v in vals if v < 35 or v > 65)
    flag = " ***" if avg > 75 or avg < 25 else ""
    print(f"{name:42} | {avg:5.1f}% | {min_v:5.1f}% | {max_v:5.1f}% | {out_cnt}/8 {flag}")
