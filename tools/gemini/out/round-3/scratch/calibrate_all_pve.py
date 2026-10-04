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
temp_a.mkdir(exist_ok=True)
temp_b.mkdir(exist_ok=True)

ITEMS = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

def eval_cand(cand_p, cand_e, runs=60):
    fname = "eval.json"
    with open(temp_a / fname, "w", encoding="utf-8") as f:
        json.dump(cand_p, f, indent=2, ensure_ascii=False)
    with open(temp_b / fname, "w", encoding="utf-8") as f:
        json.dump(cand_e, f, indent=2, ensure_ascii=False)
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
            for p in line.strip().split():
                if p.endswith("%"):
                    try:
                        return float(p[:-1])
                    except ValueError:
                        pass
    return None

rarities = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]

# Pve calibration loop
for idx in range(1, 31):
    low, high = get_target_band(idx)
    target = (low + high) / 2.0
    fname = f"stage_{idx:02d}.json"
    with open(P_DIR / fname, "r", encoding="utf-8") as f:
        p_data = json.load(f)
    with open(E_DIR / fname, "r", encoding="utf-8") as f:
        e_data = json.load(f)
        
    # Ensure enemy level matches stage index smoothly
    e_data["level"] = idx
    # Ensure player stage 1..10 has sect: None
    if idx <= 10:
        p_data["sect"] = None
        with open(P_DIR / fname, "w", encoding="utf-8") as f:
            json.dump(p_data, f, indent=2, ensure_ascii=False)

    wr = eval_cand(p_data, e_data, runs=60)
    print(f"\n[Stage {idx:02d}] Target: {low:.0f}-{high:.0f}%, Initial: {wr}%")
    if wr is not None and low <= wr <= high:
        print(f"  Stage {idx:02d} OK!")
        continue

    best_diff = abs(wr - target) if wr is not None else 999
    best_e = copy.deepcopy(e_data)
    best_wr = wr

    # 1. Try uniform tier & rarity adjustments
    p_tier = p_data["items"][0].get("tier", 1)
    cand_tiers = [p_tier, p_tier + 1, max(1, p_tier - 1), p_tier + 2]
    found = False
    
    for t in cand_tiers:
        for r in rarities:
            cand = copy.deepcopy(e_data)
            for it in cand["items"]:
                it["tier"] = t
                it["rarity"] = r
            c_wr = eval_cand(p_data, cand, runs=40)
            if c_wr is None: continue
            diff = abs(c_wr - target)
            if diff < best_diff:
                best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
            if low <= c_wr <= high:
                print(f"  FOUND uniform: T{t} {r} -> WR={c_wr}%")
                found = True
                break
        if found: break

    # 2. If not found, try per-item rarities
    if not found:
        cand = copy.deepcopy(best_e)
        for i_idx in range(len(cand["items"])):
            for r in rarities:
                cand["items"][i_idx]["rarity"] = r
                c_wr = eval_cand(p_data, cand, runs=40)
                if c_wr is None: continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                if low <= c_wr <= high:
                    print(f"  FOUND per-item: item[{i_idx}]={r} -> WR={c_wr}%")
                    found = True
                    break
            if found: break

    # 3. If still not found and WR is 100%, enemy might need an extra armor/item
    if not found and best_wr is not None and best_wr > high:
        extra_cands = [
            {"id": "thiet_giap_y", "x": 3, "y": 0, "tier": p_tier, "rarity": "common"},
            {"id": "da_ho_y", "x": 3, "y": 0, "tier": p_tier, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 4, "y": 3, "tier": p_tier, "rarity": "common"},
            {"id": "bach_ngoc_boi", "x": 4, "y": 2, "tier": p_tier, "rarity": "common"}
        ]
        for extra in extra_cands:
            for r in ["common", "uncommon", "rare", "epic", "legendary"]:
                cand = copy.deepcopy(e_data)
                ex = copy.deepcopy(extra)
                ex["rarity"] = r
                cand["items"].append(ex)
                c_wr = eval_cand(p_data, cand, runs=40)
                if c_wr is None: continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                if low <= c_wr <= high:
                    print(f"  FOUND with extra {extra['id']} ({r}) -> WR={c_wr}%")
                    found = True
                    break
            if found: break

    # Save best
    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(best_e, f, indent=2, ensure_ascii=False)
    print(f"  Saved Stage {idx:02d}: WR={best_wr}%")

print("\nPvE Calibration complete!")
