import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")

cols = ["T1/TP1", "T1/TP9", "T3/TP1", "T3/TP9", "T6/TP1", "T6/TP9", "T9/TP1", "T9/TP9"]

with open("tools/gemini/out/round-5/tables/baseline_matrix_a.txt", encoding="utf-8") as f:
    for line in f:
        if line.startswith("Loadout") or line.startswith("-") or line.startswith("Outside") or not line.strip():
            continue
        parts = line.split()
        pcts = [p for p in parts if p.endswith("%") or p.endswith("%!")]
        if len(pcts) == 8:
            first_pct_idx = line.find(pcts[0])
            name = line[:first_pct_idx].strip()
            for col, p in zip(cols, pcts):
                val = float(p.replace("%!", "").replace("%", ""))
                if val < 15.0:
                    print(f"EXTREME LOW (<15%):  {name:<36} {col}: {val:5.1f}%")
                elif val > 85.0:
                    print(f"EXTREME HIGH (>85%): {name:<36} {col}: {val:5.1f}%")
