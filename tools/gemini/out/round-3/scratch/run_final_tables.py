import subprocess
import os
import sys
import copy
import tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R3 = ROOT / "tools" / "gemini" / "out" / "round-3"
TABLES = R3 / "tables"
TABLES.mkdir(parents=True, exist_ok=True)

ITEMS = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"
LOADOUTS = R3 / "loadouts"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

env = os.environ.copy()
env["PYTHONUTF8"] = "1"

def run_cmd(cmd, outfile):
    print(f"Running: {' '.join(cmd)}")
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    with open(outfile, "w", encoding="utf-8") as f:
        f.write(res.stdout)
    print(f"Saved to {outfile}")
    if res.stderr:
        print(f"Stderr: {res.stderr[:300]}")

# 1. Matrix A
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS),
    "--sects", str(SECTS),
    "--battle-config", str(CONFIG),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "100",
    "--level", "10"
], TABLES / "final_matrix_a.txt")

# 2. Matrix B Common
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS),
    "--sects", str(SECTS),
    "--battle-config", str(CONFIG),
    "--tiers", "1,9",
    "--tp", "1",
    "--rarity", "common",
    "--runs", "100",
    "--level", "10"
], TABLES / "final_matrix_b_common.txt")

# 3. Matrix B Legendary
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS),
    "--sects", str(SECTS),
    "--battle-config", str(CONFIG),
    "--tiers", "1,9",
    "--tp", "1",
    "--rarity", "legendary",
    "--runs", "100",
    "--level", "10"
], TABLES / "final_matrix_b_legendary.txt")

# 4. Rarity Step
run_cmd([
    "py", "tools/balance/balance.py", "rarity-step",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS),
    "--sects", str(SECTS),
    "--battle-config", str(CONFIG),
    "--tier", "3",
    "--tp", "1",
    "--level", "8",
    "--runs", "100"
], TABLES / "final_rarity_step.txt")

# 5. Tier Step (same hero level=10 on both sides)
classpath = balance.ensure_classpath()
source = balance.load_source_loadouts(LOADOUTS)
pairs = [(2, 1), (3, 2), (4, 3), (6, 5), (7, 6), (9, 8)]
headers = [f"T{h}>T{l}" for h, l in pairs]

rows = []
labels = []

with tempfile.TemporaryDirectory(prefix="tier_step_final_") as raw_temp:
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
            val = balance.cli_match(classpath, p_high, p_low, 100, ITEMS, SECTS, CONFIG)
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

with open(TABLES / "final_tier_step.txt", "w", encoding="utf-8") as f:
    f.write("\n".join(out_lines) + "\n")
print(f"Saved {TABLES / 'final_tier_step.txt'}")

print("All final balance tables generated successfully!")
