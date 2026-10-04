import subprocess
import os
import sys
import copy
import json
import tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
TABLES = R4 / "tables"
TABLES.mkdir(parents=True, exist_ok=True)
LOADOUTS = R4 / "loadouts"

ITEMS_PATH = R4 / "items-final.json"
CONFIG_PATH = R4 / "config.json"
SECTS_PATH = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()

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

# 1. Final Matrix A
print("=== 1. Final Matrix A ===")
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS_PATH),
    "--battle-config", str(CONFIG_PATH),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "100",
    "--level", "10"
], TABLES / "final_matrix_a.txt")

# 2. Final Matrix B Common
print("=== 2. Final Matrix B Common ===")
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS_PATH),
    "--battle-config", str(CONFIG_PATH),
    "--tiers", "1,9",
    "--tp", "1",
    "--rarity", "common",
    "--runs", "100",
    "--level", "10"
], TABLES / "final_matrix_b_common.txt")

# 3. Final Matrix B Legendary
print("=== 3. Final Matrix B Legendary ===")
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS_PATH),
    "--battle-config", str(CONFIG_PATH),
    "--tiers", "1,9",
    "--tp", "1",
    "--rarity", "legendary",
    "--runs", "100",
    "--level", "10"
], TABLES / "final_matrix_b_legendary.txt")

# 4. Final Rarity Step
print("=== 4. Final Rarity Step ===")
run_cmd([
    "py", "tools/balance/balance.py", "rarity-step",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS_PATH),
    "--battle-config", str(CONFIG_PATH),
    "--tier", "3",
    "--tp", "1",
    "--level", "10",
    "--runs", "100"
], TABLES / "final_rarity_step.txt")

# 5. Final Tier Step
print("=== 5. Final Tier Step ===")
source = balance.load_source_loadouts(LOADOUTS)
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
            val = balance.cli_match(classpath, p_high, p_low, 100, str(ITEMS_PATH), str(SECTS_PATH), str(CONFIG_PATH))
            values.append(val)
        rows.append(values)

width = max(18, min(38, max(len(l) for l in labels)))
out_lines = []
out_lines.append(f"{'Loadout':<{width}} " + " ".join(f"{h:>10}" for h in headers) + f" {'Average':>10}")
out_lines.append("-" * (width + 1 + 11 * (len(headers) + 1)))

for label, row_vals in zip(labels, rows):
    avg = sum(row_vals) / len(row_vals) if row_vals else 0
    vals_str = " ".join(f"{v:9.1f}%" for v in row_vals)
    out_lines.append(f"{label:<{width}} {vals_str} {avg:9.1f}%")

out_lines.append("-" * (width + 1 + 11 * (len(headers) + 1)))
col_avgs = [sum(rows[r][c] for r in range(len(rows))) / len(rows) for c in range(len(headers))]
tot_avg = sum(col_avgs) / len(col_avgs)
out_lines.append(f"{'Average':<{width}} " + " ".join(f"{ca:9.1f}%" for ca in col_avgs) + f" {tot_avg:9.1f}%")
out_lines.append("(same hero level=10 on both sides, common rarity, runs=100)\n")

with open(TABLES / "final_tier_step.txt", "w", encoding="utf-8") as f:
    f.write("\n".join(out_lines))
print(f"Saved final_tier_step.txt")

# 6. Final Fight Length Table
print("=== 6. Final Fight Length Table ===")
tier_configs = [
    {"name": "Trọng 1 (Cấp 1, Phàm)", "tier": 1, "level": 1, "rarity": "common", "tp": 1},
    {"name": "Trọng 3 (Cấp 8, Lương)", "tier": 3, "level": 8, "rarity": "uncommon", "tp": 1},
    {"name": "Trọng 6 (Cấp 18, Tinh)", "tier": 6, "level": 18, "rarity": "rare", "tp": 1},
    {"name": "Trọng 9 (Cấp 30, Tuyệt)", "tier": 9, "level": 30, "rarity": "epic", "tp": 1},
]

fl_rows = []
for tc in tier_configs:
    durations = []
    over_45 = 0
    total_battles = 0
    with tempfile.TemporaryDirectory(prefix="fl_") as raw_temp:
        temp = Path(raw_temp)
        p_paths = []
        for s_idx, (fname, orig) in enumerate(source):
            ld = copy.deepcopy(orig)
            ld["level"] = tc["level"]
            ld["tamPhapLevel"] = tc["tp"]
            for it in ld["items"]:
                it["tier"] = tc["tier"]
                it["rarity"] = tc["rarity"]
            p = temp / f"{s_idx}.json"
            balance.write_json(p, ld, indent=None)
            p_paths.append(p)
        
        for i in range(len(p_paths)):
            for j in [i, (i + 1) % len(p_paths), (i + 7) % len(p_paths)]:
                cmd = ["java", "-cp", classpath, "com.onebagjianghu.battle.cli.BattleCli", str(p_paths[i]), str(p_paths[j]), "--runs", "15", "--items", str(ITEMS_PATH), "--sects", str(SECTS_PATH), "--battle-config", str(CONFIG_PATH)]
                res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
                for line in res.stdout.splitlines():
                    if "thời lượng TB" in line:
                        parts = line.split("thời lượng TB")
                        dur_str = parts[1].strip().replace("s", "")
                        try:
                            d = float(dur_str)
                            durations.append(d)
                            total_battles += 1
                            if d > 45.0:
                                over_45 += 1
                        except ValueError:
                            pass

    avg_d = sum(durations) / len(durations) if durations else 0
    pct_over_45 = (over_45 / total_battles * 100.0) if total_battles else 0
    fl_rows.append({
        "tier_name": tc["name"],
        "avg_duration": avg_d,
        "share_over_45": pct_over_45,
        "sample_count": total_battles
    })
    print(f"  {tc['name']}: TB = {avg_d:.1f}s, Tỷ lệ > 45s = {pct_over_45:.1f}%")

fl_lines = []
fl_lines.append(f"{'Mốc Trọng & Cấp':<28} {'Thời Lượng TB':<16} {'Tỷ Lệ Trận > 45s':<20} {'Mục Tiêu'}")
fl_lines.append("-" * 80)
for r in fl_rows:
    fl_lines.append(f"{r['tier_name']:<28} {r['avg_duration']:5.1f}s           {r['share_over_45']:5.1f}%              15-35s / < 20%")
fl_lines.append("-" * 80)

with open(TABLES / "final_fight_lengths.txt", "w", encoding="utf-8") as f:
    f.write("\n".join(fl_lines) + "\n")
print(f"Saved final_fight_lengths.txt successfully!")

print("\nALL FINAL TABLES COMPLETED SUCCESSFULLY!")
