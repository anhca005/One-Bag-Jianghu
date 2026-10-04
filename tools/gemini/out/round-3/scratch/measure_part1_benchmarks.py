import json
import subprocess
import os
import sys
import copy
import re
import tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R3 = ROOT / "tools" / "gemini" / "out" / "round-3"
LOADOUTS_DIR = R3 / "loadouts"
ITEMS = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"
TABLES = R3 / "tables"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

classpath = balance.ensure_classpath()
source = balance.load_source_loadouts(LOADOUTS_DIR)

# Configurations for Part 1 benchmarks:
# 1. Level 1, T1 common (Phàm)
# 2. Level 10, T3 uncommon (Lương)
# 3. Level 30, T9 supreme (Chí Tôn)

benchmarks = [
    {"name": "(Cấp 1, T1 Phàm)", "level": 1, "tier": 1, "rarity": "common", "tp": 1},
    {"name": "(Cấp 10, T3 Lương)", "level": 10, "tier": 3, "rarity": "uncommon", "tp": 1},
    {"name": "(Cấp 30, T9 Chí Tôn)", "level": 30, "tier": 9, "rarity": "supreme", "tp": 9},
]

results = []

with tempfile.TemporaryDirectory(prefix="bm_") as raw_temp:
    temp = Path(raw_temp)
    for b in benchmarks:
        print(f"Measuring {b['name']}...")
        hp_list = []
        duration_list = []
        
        # Prepare loadout files
        paths = []
        for idx, (fname, orig) in enumerate(source):
            ld = copy.deepcopy(orig)
            ld["level"] = b["level"]
            ld["tamPhapLevel"] = b["tp"]
            for it in ld["items"]:
                it["tier"] = b["tier"]
                it["rarity"] = b["rarity"]
            p = temp / f"{idx}.json"
            balance.write_json(p, ld, indent=None)
            paths.append(p)
            
        # Get Max HP for each loadout using BattleCli 1 run json
        for idx, p in enumerate(paths):
            res_json = temp / f"res_{idx}.json"
            cmd = [
                "java", "-cp", classpath, "com.onebagjianghu.battle.cli.BattleCli",
                str(p), str(p), "--seed", "1", "--json", str(res_json),
                "--items", str(ITEMS), "--sects", str(SECTS),
                "--battle-config", str(CONFIG)
            ]
            balance.run_checked(cmd, balance.ENGINE)
            with open(res_json, "r", encoding="utf-8") as f:
                r_data = json.load(f)
                # First HP before events or calculate max HP
                # Events have damage or we can see initial KH
            # Alternatively, run 2 runs without seed to get duration and HP stats
            cmd_stats = [
                "java", "-cp", classpath, "com.onebagjianghu.battle.cli.BattleCli",
                str(p), str(p), "--runs", "2",
                "--items", str(ITEMS), "--sects", str(SECTS),
                "--battle-config", str(CONFIG)
            ]
            out = balance.run_checked(cmd_stats, balance.ENGINE).stdout
            # Extract "thời lượng TB X.Xs"
            m = re.search(r"thời lượng TB ([0-9.]+)s", out)
            if m:
                duration_list.append(float(m.group(1)))
            # Extract KH max from "[A] Max HP: XXX" or breakdown
            m_hp = re.search(r"maxHp\s+(\d+)", out)
            if m_hp:
                hp_list.append(int(m_hp.group(1)))

            else:
                # Calculate directly from items stats
                # Base HP = 100 + 5*(lvl-1)
                pass

        results.append({
            "benchmark": b["name"],
            "avg_hp": sum(hp_list) / len(hp_list) if hp_list else 0,
            "min_hp": min(hp_list) if hp_list else 0,
            "max_hp": max(hp_list) if hp_list else 0,
            "avg_duration": sum(duration_list) / len(duration_list) if duration_list else 0,
            "min_duration": min(duration_list) if duration_list else 0,
            "max_duration": max(duration_list) if duration_list else 0,
        })

print("\n--- KẾT QUẢ PHẦN 1: KHÍ HUYẾT VÀ THỜI LƯỢNG TRẬN ĐẤU ---")
print(f"{'Mốc Thử Nghiệm':<26} {'Khí Huyết (Min - Max / TB)':<30} {'Thời Lượng TB (s)':<18}")
print("-" * 75)
for r in results:
    hp_str = f"{r['min_hp']} - {r['max_hp']} (TB: {r['avg_hp']:.1f})"
    dur_str = f"{r['avg_duration']:.1f}s (dải {r['min_duration']:.1f}s - {r['max_duration']:.1f}s)"
    print(f"{r['benchmark']:<26} {hp_str:<30} {dur_str:<18}")

with open(TABLES / "part1_benchmarks.txt", "w", encoding="utf-8") as f:
    f.write(f"{'Mốc Thử Nghiệm':<26} {'Khí Huyết (Min - Max / TB)':<30} {'Thời Lượng TB (s)':<18}\n")
    f.write("-" * 75 + "\n")
    for r in results:
        hp_str = f"{r['min_hp']} - {r['max_hp']} (TB: {r['avg_hp']:.1f})"
        dur_str = f"{r['avg_duration']:.1f}s (dải {r['min_duration']:.1f}s - {r['max_duration']:.1f}s)"
        f.write(f"{r['benchmark']:<26} {hp_str:<30} {dur_str:<18}\n")
