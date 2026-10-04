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

ITEMS_PATH = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"

items_data = json.load(open(ITEMS_PATH, "r", encoding="utf-8"))
item_sizes = {it["id"]: it["size"] for it in items_data}

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

def find_free_pos(placed_items, new_item_id, cols=5, rows=4):
    w, h = item_sizes[new_item_id]
    grid = [[False for _ in range(cols)] for _ in range(rows)]
    for it in placed_items:
        iw, ih = item_sizes[it["id"]]
        for r in range(it["y"], it["y"] + ih):
            for c in range(it["x"], it["x"] + iw):
                if 0 <= r < rows and 0 <= c < cols:
                    grid[r][c] = True
    for y in range(rows - h + 1):
        for x in range(cols - w + 1):
            clash = False
            for r in range(y, y + h):
                for c in range(x, x + w):
                    if grid[r][c]:
                        clash = True
                        break
                if clash: break
            if not clash:
                return x, y
    return None

def eval_cand(cand_p, cand_e, runs=80):
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
        "--items", str(ITEMS_PATH),
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

# Calibrate each stage individually
for idx in range(1, 31):
    fname = f"stage_{idx:02d}.json"
    low, high = get_target_band(idx)
    target = (low + high) / 2.0
    
    with open(P_DIR / fname, "r", encoding="utf-8") as f: p_data = json.load(f)
    with open(E_DIR / fname, "r", encoding="utf-8") as f: e_data = json.load(f)
    
    # Ensure enemy level matches stage index
    e_data["level"] = idx
    if idx <= 10:
        p_data["sect"] = None
        with open(P_DIR / fname, "w", encoding="utf-8") as f: json.dump(p_data, f, indent=2, ensure_ascii=False)
        
    wr = eval_cand(p_data, e_data, runs=80)
    print(f"\nStage {idx:02d} ({low:.0f}-{high:.0f}%): current WR={wr}%")
    if wr is not None and low <= wr <= high:
        print(f"  Stage {idx:02d} OK!")
        continue
        
    best_e = copy.deepcopy(e_data)
    best_diff = abs(wr - target) if wr is not None else 999
    best_wr = wr
    found = False

    # Step 1: Adjust rarities of existing items
    for item_idx in range(len(e_data["items"])):
        for r in rarities:
            cand = copy.deepcopy(best_e)
            cand["items"][item_idx]["rarity"] = r
            c_wr = eval_cand(p_data, cand, runs=40)
            if c_wr is None: continue
            diff = abs(c_wr - target)
            if diff < best_diff:
                best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
            if low <= c_wr <= high:
                print(f"  Stage {idx:02d} REACHED BAND via item[{item_idx}]={r}: {c_wr}%")
                found = True
                break
        if found: break

    # Step 2: Try adding a small item (1x1 or 2x1) if enemy still losing 100%
    if not found and best_wr is not None and best_wr > high:
        p_tier = p_data["items"][0].get("tier", 1)
        for cand_id in ["thiet_giap_y", "da_ho_y", "boi_nguyen_dan", "bach_ngoc_boi", "thanh_dong_ho_tam_kinh"]:
            pos = find_free_pos(best_e["items"], cand_id)
            if pos is None: continue
            cand = copy.deepcopy(best_e)
            cand["items"].append({
                "id": cand_id,
                "x": pos[0],
                "y": pos[1],
                "tier": p_tier,
                "rarity": "common"
            })
            for r in ["common", "uncommon", "rare", "epic"]:
                cand["items"][-1]["rarity"] = r
                c_wr = eval_cand(p_data, cand, runs=40)
                if c_wr is None: continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                if low <= c_wr <= high:
                    print(f"  Stage {idx:02d} REACHED BAND with +{cand_id} ({r}): {c_wr}%")
                    found = True
                    break
            if found: break

    # Step 3: If enemy is too strong (wr < low), reduce tier or rarity
    if not found and best_wr is not None and best_wr < low:
        for item_idx in range(len(best_e["items"])):
            cand = copy.deepcopy(best_e)
            cur_tier = cand["items"][item_idx].get("tier", 1)
            if cur_tier > 1:
                cand["items"][item_idx]["tier"] = cur_tier - 1
            cand["items"][item_idx]["rarity"] = "common"
            c_wr = eval_cand(p_data, cand, runs=40)
            if c_wr is None: continue
            diff = abs(c_wr - target)
            if diff < best_diff:
                best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
            if low <= c_wr <= high:
                print(f"  Stage {idx:02d} REACHED BAND by lowering item[{item_idx}]: {c_wr}%")
                found = True
                break

    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(best_e, f, indent=2, ensure_ascii=False)
    print(f"  Saved Stage {idx:02d}: Final WR={best_wr}%")

print("\nFinalizing complete!")
