import json
import os
import sys
import copy
import tempfile
from pathlib import Path

sys.path.insert(0, str(Path("tools/balance").resolve()))
import balance

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

classpath = balance.ensure_classpath()
loadouts_dir = Path("tools/gemini/out/round-3/loadouts").resolve()
items_path = Path("tools/gemini/out/round-3/items-v3-final.json").resolve()
sects_path = Path("server/battle-engine/src/main/resources/data/sects.json").resolve()
config_path = Path("tools/gemini/out/round-3/config.json").resolve()
tables_dir = Path("tools/gemini/out/round-3/tables").resolve()

source = balance.load_source_loadouts(loadouts_dir)

# 1-Trọng step pairs across the tier progression: T2>T1, T3>T2, T4>T3, T6>T5, T7>T6, T9>T8
# Run with the SAME hero level (level=10) on both sides to isolate Trọng multiplier
pairs = [(2, 1), (3, 2), (4, 3), (6, 5), (7, 6), (9, 8)]
headers = [f"T{h}>T{l}" for h, l in pairs]

rows = []
labels = []

with tempfile.TemporaryDirectory(prefix="tier_step_") as raw_temp:
    temp = Path(raw_temp)
    for source_index, (filename, original) in enumerate(source):
        labels.append(original.get("name") or Path(filename).stem)
        values = []
        for pair_idx, (t_high, t_low) in enumerate(pairs):
            high = copy.deepcopy(original)
            low = copy.deepcopy(original)
            high["level"] = low["level"] = 10
            high["tamPhapLevel"] = low["tamPhapLevel"] = 1
            for it in high["items"]:
                it["tier"] = t_high
                it["rarity"] = "common"
            for it in low["items"]:
                it["tier"] = t_low
                it["rarity"] = "common"
            p_high = temp / f"{source_index}_{pair_idx}_h.json"
            p_low = temp / f"{source_index}_{pair_idx}_l.json"
            balance.write_json(p_high, high, indent=None)
            balance.write_json(p_low, low, indent=None)
            val = balance.cli_match(classpath, p_high, p_low, 100, items_path, sects_path, config_path)
            values.append(val)
        rows.append(values)

width = max(18, min(38, max(len(l) for l in labels)))
out_lines = []
out_lines.append(f"{'Loadout':<{width}} " + " ".join(f"{h:>10}" for h in headers) + f" {'Average':>10}")
out_lines.append("-" * (width + 1 + 11 * (len(headers) + 1)))

for label, vals in zip(labels, rows):
    avg = sum(vals) / len(vals)
    out_lines.append(f"{label[:width]:<{width}} " + " ".join(f"{v:9.1f}%" for v in vals) + f" {avg:9.1f}%")

col_avgs = [sum(row[col] for row in rows) / len(rows) for col in range(len(headers))]
overall_avg = sum(col_avgs) / len(col_avgs)
out_lines.append("-" * (width + 1 + 11 * (len(headers) + 1)))
out_lines.append(f"{'Average':<{width}} " + " ".join(f"{v:9.1f}%" for v in col_avgs) + f" {overall_avg:9.1f}%")
out_lines.append("(same hero level=10 on both sides, common rarity, runs=100)")

result_text = "\n".join(out_lines) + "\n"
print(result_text)

with open(tables_dir / "baseline_tier_step.txt", "w", encoding="utf-8") as f:
    f.write(result_text)

print("Saved baseline_tier_step.txt successfully!")
