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

def eval_stage(idx, runs=80):
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

rarities = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]

# Fine-tuning per stage
def fine_tune_single(idx):
    info = stages_info[idx]
    kind = info["kind"]
    low, high = get_target_band(kind)
    target = (low + high) / 2.0
    fname = f"stage_{idx:02d}.json"
    
    with open(E_DIR / fname, "r", encoding="utf-8") as f:
        e_data = json.load(f)
    with open(P_DIR / fname, "r", encoding="utf-8") as f:
        p_data = json.load(f)
        
    wr = eval_stage(idx, runs=80)
    print(f"Stage {idx:02d} ({info['name']}, {kind}, target {low:.0f}-{high:.0f}%): initial WR={wr}%")
    if wr is not None and low <= wr <= high:
        return wr
        
    best_wr = wr
    best_diff = abs(wr - target) if wr is not None else 999
    best_state = copy.deepcopy(e_data)
    
    # Try adjusting enemy level smoothly around idx
    # If WR is 100% (player too strong):
    # Try adding an armor item or consumable if enemy has fewer items than player
    if wr is not None and wr > high:
        if len(e_data["items"]) < len(p_data["items"]):
            # Add an armor or pill from player's tier
            p_tier = p_data["items"][0].get("tier", 1)
            extra_items = [
                {"id": "boi_nguyen_dan", "x": 4, "y": 3, "tier": p_tier, "rarity": "common"},
                {"id": "da_ho_y", "x": 3, "y": 0, "tier": p_tier, "rarity": "common"},
                {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": p_tier, "rarity": "common"},
            ]
            for extra in extra_items:
                cand = copy.deepcopy(e_data)
                # find free placement or append
                cand["items"].append(extra)
                with open(E_DIR / fname, "w", encoding="utf-8") as f:
                    json.dump(cand, f, indent=2, ensure_ascii=False)
                c_wr = eval_stage(idx, runs=60)
                if c_wr is not None:
                    diff = abs(c_wr - target)
                    if diff < best_diff:
                        best_diff = diff
                        best_wr = c_wr
                        best_state = copy.deepcopy(cand)
                    if low <= c_wr <= high:
                        print(f"  -> Reached band by adding {extra['id']}: WR={c_wr}%")
                        return c_wr
                        
    # Try item rarity stepping on individual items
    for item_idx in range(len(e_data["items"])):
        for r in rarities:
            for lvl in [idx, max(1, idx - 1), idx + 1, max(1, idx - 2), idx + 2]:
                cand = copy.deepcopy(best_state)
                cand["level"] = lvl
                cand["items"][item_idx]["rarity"] = r
                with open(E_DIR / fname, "w", encoding="utf-8") as f:
                    json.dump(cand, f, indent=2, ensure_ascii=False)
                c_wr = eval_stage(idx, runs=60)
                if c_wr is None:
                    continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff = diff
                    best_wr = c_wr
                    best_state = copy.deepcopy(cand)
                    print(f"  -> New best: WR={c_wr}% (lvl={lvl}, item[{item_idx}].rarity={r})")
                if low <= c_wr <= high:
                    print(f"  -> REACHED BAND: WR={c_wr}%!")
                    with open(E_DIR / fname, "w", encoding="utf-8") as f:
                        json.dump(best_state, f, indent=2, ensure_ascii=False)
                    return c_wr

    # Save best
    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(best_state, f, indent=2, ensure_ascii=False)
    return best_wr

for i in range(1, 31):
    fine_tune_single(i)

print("\nDone fine tuning all 30 stages!")
