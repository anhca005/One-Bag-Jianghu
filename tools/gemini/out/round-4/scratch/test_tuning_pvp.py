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
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
LOADOUTS = R4 / "loadouts"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()

items_base = json.load(open(balance.DEFAULT_ITEMS, encoding="utf-8"))
items_dict = {it["id"]: it for it in items_base}

def evaluate_patch(patch, config_dict=None, runs=60):
    # Apply patch to items
    test_items = copy.deepcopy(items_base)
    by_id = {it["id"]: it for it in test_items}
    for k, v in patch.items():
        parts = k.split(".")
        iid = parts[0]
        prop = parts[1]
        if iid in by_id:
            if len(parts) == 2:
                by_id[iid][prop] = v
            elif len(parts) == 4 and prop == "triggers":
                # e.g. item.triggers.0.effects.0.value
                t_idx = int(parts[2])
                # handled if needed
                pass

    with tempfile.TemporaryDirectory(prefix="eval_") as raw_temp:
        temp = Path(raw_temp)
        f_items = temp / "items.json"
        f_cfg = temp / "config.json"
        balance.write_json(f_items, test_items, indent=None)
        if config_dict:
            balance.write_json(f_cfg, config_dict, indent=None)
        else:
            f_cfg = None

        # 1. Measure fight lengths across 24 mirror matches
        source = balance.load_source_loadouts(LOADOUTS)
        mirror_durations = []
        over_45 = 0
        total_m = 0
        
        # Test mirror matches at T3 (Lv8, Lương) and T6 (Lv18, Tinh)
        for tc in [{"t": 3, "lvl": 8, "r": "uncommon"}, {"t": 6, "lvl": 18, "r": "rare"}]:
            for s_idx, (fname, orig) in enumerate(source):
                ld = copy.deepcopy(orig)
                ld["level"] = tc["lvl"]
                ld["tamPhapLevel"] = 1
                for it in ld["items"]:
                    it["tier"] = tc["t"]
                    it["rarity"] = tc["r"]
                p = temp / f"m_{s_idx}.json"
                balance.write_json(p, ld, indent=None)
                
                cmd = ["java", "-cp", classpath, "com.onebagjianghu.battle.cli.BattleCli", str(p), str(p), "--runs", "10", "--items", str(f_items)]
                if f_cfg:
                    cmd.extend(["--battle-config", str(f_cfg)])
                res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
                for line in res.stdout.splitlines():
                    if "thời lượng TB" in line:
                        dur = float(line.split("thời lượng TB")[1].strip().replace("s", ""))
                        mirror_durations.append(dur)
                        total_m += 1
                        if dur > 45.0:
                            over_45 += 1

        avg_dur = sum(mirror_durations) / len(mirror_durations) if mirror_durations else 0
        pct_over_45 = (over_45 / total_m * 100.0) if total_m else 0
        print(f"Fight duration: avg={avg_dur:.1f}s, >45s={pct_over_45:.1f}% ({over_45}/{total_m})")

        # 2. Run quick Matrix A (tiers 3,6, runs=30)
        cmd_mat = [
            "py", "tools/balance/balance.py", "matrix",
            "--loadouts", str(LOADOUTS),
            "--items", str(f_items),
            "--tiers", "3,6",
            "--tp", "1",
            "--runs", str(runs),
            "--level", "10"
        ]
        if f_cfg:
            cmd_mat.extend(["--battle-config", str(f_cfg)])
        res_mat = subprocess.run(cmd_mat, capture_output=True, text=True, encoding="utf-8", errors="replace")
        
        outside = 0
        total_cells = 0
        for line in res_mat.stdout.splitlines():
            if "Outside [35, 65]:" in line:
                print("  Matrix preview:", line.strip())
            elif line.startswith("#") or (len(line) > 10 and "%" in line):
                pass
        return avg_dur, pct_over_45

# Baseline run
print("Evaluating current baseline...")
evaluate_patch({}, runs=30)
