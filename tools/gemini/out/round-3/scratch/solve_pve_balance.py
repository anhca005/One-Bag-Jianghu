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

def eval_candidate(cand_p, cand_e, runs=80):
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

# Ensure player stage 10 has sect: None
p10_path = P_DIR / "stage_10.json"
with open(p10_path, "r", encoding="utf-8") as f:
    p10 = json.load(f)
if p10.get("sect") is not None:
    p10["sect"] = None
    with open(p10_path, "w", encoding="utf-8") as f:
        json.dump(p10, f, indent=2, ensure_ascii=False)

def solve_stage(idx):
    low, high = get_target_band(idx)
    target = (low + high) / 2.0
    fname = f"stage_{idx:02d}.json"
    with open(P_DIR / fname, "r", encoding="utf-8") as f:
        p_data = json.load(f)
    with open(E_DIR / fname, "r", encoding="utf-8") as f:
        e_data = json.load(f)

    # Initial check
    wr = eval_candidate(p_data, e_data, runs=80)
    print(f"\n--- Stage {idx:02d} (Target: {low:.0f}-{high:.0f}%) Initial WR: {wr}% ---")
    if wr is not None and low <= wr <= high:
        print(f"Stage {idx:02d} already in band!")
        return wr

    best_e = copy.deepcopy(e_data)
    best_diff = abs(wr - target) if wr is not None else 999
    best_wr = wr

    # We want smooth enemy levels: idx +- 1
    levels_to_try = [idx, max(1, idx - 1), idx + 1]

    # Try adjusting item rarities across the board or per item
    # Also if enemy has significantly fewer items than player, try tier bumps
    base_tier = e_data["items"][0].get("tier", 1)
    tiers_to_try = [base_tier, base_tier + 1, max(1, base_tier - 1), base_tier + 2]

    # Quick search: uniform tier & rarity
    for lvl in levels_to_try:
        for t in tiers_to_try:
            for r in rarities:
                cand = copy.deepcopy(e_data)
                cand["level"] = lvl
                for it in cand["items"]:
                    it["tier"] = t
                    it["rarity"] = r
                c_wr = eval_candidate(p_data, cand, runs=50)
                if c_wr is None:
                    continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff = diff
                    best_wr = c_wr
                    best_e = copy.deepcopy(cand)
                    print(f"  Uniform try: lvl={lvl}, T{t} {r} -> WR={c_wr}%")
                if low <= c_wr <= high:
                    print(f"  FOUND uniform: lvl={lvl}, T{t} {r} -> WR={c_wr}%")
                    with open(E_DIR / fname, "w", encoding="utf-8") as f:
                        json.dump(best_e, f, indent=2, ensure_ascii=False)
                    return c_wr

    # If still not in band, try mixing rarities per item
    for lvl in levels_to_try:
        cand = copy.deepcopy(best_e)
        cand["level"] = lvl
        for item_idx in range(len(cand["items"])):
            for r in rarities:
                cand["items"][item_idx]["rarity"] = r
                c_wr = eval_candidate(p_data, cand, runs=50)
                if c_wr is None:
                    continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff = diff
                    best_wr = c_wr
                    best_e = copy.deepcopy(cand)
                    print(f"  Per-item try: lvl={lvl}, item[{item_idx}]={r} -> WR={c_wr}%")
                if low <= c_wr <= high:
                    print(f"  FOUND per-item: lvl={lvl} -> WR={c_wr}%")
                    with open(E_DIR / fname, "w", encoding="utf-8") as f:
                        json.dump(best_e, f, indent=2, ensure_ascii=False)
                    return c_wr

    # If still not in band and WR is 100%, enemy might need an extra defensive/thematic item
    if best_wr is not None and best_wr > high:
        extra_candidates = [
            {"id": "thiet_giap_y", "x": 3, "y": 0, "tier": base_tier, "rarity": "common"},
            {"id": "da_ho_y", "x": 3, "y": 0, "tier": base_tier, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 4, "y": 3, "tier": base_tier, "rarity": "common"},
            {"id": "thiet_ho_uyen", "x": 4, "y": 2, "tier": base_tier, "rarity": "common"},
            {"id": "bach_ngoc_boi", "x": 4, "y": 2, "tier": base_tier, "rarity": "common"}
        ]
        for extra in extra_candidates:
            for lvl in levels_to_try:
                for r in ["common", "uncommon", "rare", "epic"]:
                    cand = copy.deepcopy(e_data)
                    cand["level"] = lvl
                    extra_copy = copy.deepcopy(extra)
                    extra_copy["rarity"] = r
                    cand["items"].append(extra_copy)
                    c_wr = eval_candidate(p_data, cand, runs=50)
                    if c_wr is None:
                        continue
                    diff = abs(c_wr - target)
                    if diff < best_diff:
                        best_diff = diff
                        best_wr = c_wr
                        best_e = copy.deepcopy(cand)
                        print(f"  Extra-item try: +{extra['id']} lvl={lvl} {r} -> WR={c_wr}%")
                    if low <= c_wr <= high:
                        print(f"  FOUND with extra item: +{extra['id']} lvl={lvl} -> WR={c_wr}%")
                        with open(E_DIR / fname, "w", encoding="utf-8") as f:
                            json.dump(best_e, f, indent=2, ensure_ascii=False)
                        return c_wr

    # Save best found
    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(best_e, f, indent=2, ensure_ascii=False)
    print(f"  Saved best for stage {idx:02d}: WR={best_wr}%")
    return best_wr

for idx in range(1, 31):
    solve_stage(idx)

print("\nAll 30 stages tuning complete!")
