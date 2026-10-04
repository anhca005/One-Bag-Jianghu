import json
import subprocess
import shutil
import os
import sys
import copy
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R3 = ROOT / "tools" / "gemini" / "out" / "round-3"
BAL_DIR = R3 / "balance"
P_DIR = BAL_DIR / "player"
E_DIR = BAL_DIR / "enemy"
temp_a = BAL_DIR / "temp_a"
temp_b = BAL_DIR / "temp_b"

ITEMS = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"

with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "chapters.json", "r", encoding="utf-8") as f:
    chapters_data = json.load(f)

stages_info = {}
for ch in chapters_data["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        stages_info[idx] = {
            "name": st["name"],
            "kind": st["kind"],
            "enemy_name": st["enemy"]["name"],
            "chapter_idx": ch["index"] - 1,
            "stage_in_ch": st["index"] - 1
        }

def get_target_band(kind):
    if kind == "boss":
        return 50.0, 60.0
    elif kind == "elite":
        return 60.0, 70.0
    return 80.0, 90.0

def eval_stage(idx, runs=60):
    fname = f"stage_{idx:02d}.json"
    shutil.copy(P_DIR / fname, temp_a / fname)
    shutil.copy(E_DIR / fname, temp_b / fname)
    
    cmd = [
        "py", "tools/balance/balance.py", "vs",
        "--a", str(temp_a),
        "--b", str(temp_b),
        "--runs", str(runs),
        "--items", str(ITEMS),
        "--sects", str(SECTS),
        "--battle-config", str(CONFIG)
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    (temp_a / fname).unlink()
    (temp_b / fname).unlink()
    
    for line in res.stdout.splitlines():
        if "%" in line and not line.strip().startswith("("):
            parts = line.strip().split()
            for p in parts:
                if p.endswith("%"):
                    try:
                        return float(p[:-1])
                    except ValueError:
                        pass
    return None

rarities = ["common", "uncommon", "rare", "epic", "legendary"]

def calibrate_stage(idx):
    info = stages_info[idx]
    kind = info["kind"]
    low, high = get_target_band(kind)
    fname = f"stage_{idx:02d}.json"
    
    with open(E_DIR / fname, "r", encoding="utf-8") as f:
        e_data = json.load(f)
        
    print(f"\n--- Calibrating Stage {idx:02d} ({info['name']}, {kind}, band {low:.0f}-{high:.0f}%) ---")
    
    # We can tune:
    # 1. enemy level
    # 2. item tiers / rarities
    wr = eval_stage(idx, runs=60)
    print(f"  Initial WR: {wr}% (enemy level={e_data.get('level')})")
    
    if wr is not None and low <= wr <= high:
        print(f"  Stage {idx:02d} is already in band!")
        return wr
        
    best_wr = wr
    best_diff = 999
    best_state = copy.deepcopy(e_data)
    
    # If WR is 100%, enemy needs more items or higher tier/rarity/level
    # If WR is 0%, enemy is too strong, reduce enemy tier/rarity/level
    target = (low + high) / 2.0
    
    # Grid search / coordinate adjustment
    for tier_offset in [0, 1, -1, 2, -2]:
        for r_offset in [0, 1, -1]:
            for lvl_cand in [idx, max(1, idx - 2), idx + 2, max(1, idx - 4), idx + 4]:
                cand = copy.deepcopy(best_state)
                cand["level"] = lvl_cand
                for it in cand["items"]:
                    it["tier"] = max(1, min(9, it.get("tier", 1) + tier_offset))
                    cur_r_idx = rarities.index(it.get("rarity", "common")) if it.get("rarity") in rarities else 0
                    new_r_idx = max(0, min(len(rarities) - 1, cur_r_idx + r_offset))
                    it["rarity"] = rarities[new_r_idx]
                
                with open(E_DIR / fname, "w", encoding="utf-8") as f:
                    json.dump(cand, f, indent=2, ensure_ascii=False)
                
                wr_c = eval_stage(idx, runs=40)
                if wr_c is None:
                    continue
                diff = abs(wr_c - target)
                if diff < best_diff:
                    best_diff = diff
                    best_wr = wr_c
                    best_state = copy.deepcopy(cand)
                    print(f"  New best: WR={wr_c:.1f}% (lvl={lvl_cand}, t_off={tier_offset}, r_off={r_offset})")
                    
                if low <= wr_c <= high:
                    print(f"  REACHED BAND: WR={wr_c:.1f}%!")
                    with open(E_DIR / fname, "w", encoding="utf-8") as f:
                        json.dump(best_state, f, indent=2, ensure_ascii=False)
                    return wr_c
                    
    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(best_state, f, indent=2, ensure_ascii=False)
    return best_wr

for idx in range(1, 31):
    calibrate_stage(idx)

print("\nCalibration pass completed!")
