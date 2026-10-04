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

def validate_and_find_free(placed_items, new_item_id, cols=5, rows=4):
    w, h = item_sizes[new_item_id]
    occupied = set()
    for it in placed_items:
        iw, ih = item_sizes[it["id"]]
        for r in range(it["y"], it["y"] + ih):
            for c in range(it["x"], it["x"] + iw):
                occupied.add((c, r))
    for y in range(rows - h + 1):
        for x in range(cols - w + 1):
            clash = any((x + dx, y + dy) in occupied for dx in range(w) for dy in range(h))
            if not clash:
                return x, y
    return None

def check_no_overlap(loadout):
    cols, rows = loadout.get("cols", 5), loadout.get("rows", 4)
    occupied = set()
    for it in loadout["items"]:
        w, h = item_sizes[it["id"]]
        for r in range(it["y"], it["y"] + h):
            for c in range(it["x"], it["x"] + w):
                if c < 0 or c >= cols or r < 0 or r >= rows:
                    return False
                if (c, r) in occupied:
                    return False
                occupied.add((c, r))
    return True

def eval_cand(cand_p, cand_e, runs=80):
    if not check_no_overlap(cand_e):
        return None
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

# Start with clean chapter enemies from round-2/chapters.json as base
ch2 = json.load(open("tools/gemini/out/round-2/chapters.json", "r", encoding="utf-8"))
orig_enemies = {}
for ch in ch2["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        e = copy.deepcopy(st["enemy"])
        e.pop("hp", None)
        e["level"] = idx
        e["cols"] = 5
        e["rows"] = 4
        orig_enemies[idx] = e

print("Calibrating all 30 stages cleanly...")

results = []

for idx in range(1, 31):
    low, high = get_target_band(idx)
    target = (low + high) / 2.0
    fname = f"stage_{idx:02d}.json"
    
    with open(P_DIR / fname, "r", encoding="utf-8") as f: p_data = json.load(f)
    if idx <= 10:
        p_data["sect"] = None
        with open(P_DIR / fname, "w", encoding="utf-8") as f: json.dump(p_data, f, indent=2, ensure_ascii=False)
        
    base_e = orig_enemies[idx]
    
    # Try the original enemy first
    wr = eval_cand(p_data, base_e, runs=60)
    best_e = copy.deepcopy(base_e)
    best_wr = wr
    best_diff = abs(wr - target) if wr is not None else 999
    
    if wr is not None and low <= wr <= high:
        print(f"Stage {idx:02d}: baseline OK ({wr}%)")
        results.append((idx, low, high, wr, True, best_e))
        with open(E_DIR / fname, "w", encoding="utf-8") as f: json.dump(best_e, f, indent=2, ensure_ascii=False)
        continue

    # Try uniform rarity steps
    found = False
    for t in [base_e["items"][0].get("tier", 1), base_e["items"][0].get("tier", 1) + 1, max(1, base_e["items"][0].get("tier", 1) - 1)]:
        for r in rarities:
            cand = copy.deepcopy(base_e)
            for it in cand["items"]:
                it["tier"] = t
                it["rarity"] = r
            c_wr = eval_cand(p_data, cand, runs=40)
            if c_wr is None: continue
            diff = abs(c_wr - target)
            if diff < best_diff:
                best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
            if low <= c_wr <= high:
                print(f"Stage {idx:02d}: FOUND uniform T{t} {r} -> WR={c_wr}%")
                found = True
                break
        if found: break

    # Try adding a small item (1x1 boi_nguyen_dan or bach_ngoc_boi, or 2x1 thiet_giap_y)
    if not found and best_wr is not None and best_wr > high:
        p_tier = p_data["items"][0].get("tier", 1)
        for add_id in ["boi_nguyen_dan", "bach_ngoc_boi", "thiet_giap_y", "da_ho_y"]:
            pos = validate_and_find_free(best_e["items"], add_id)
            if pos is None: continue
            for r in ["common", "uncommon", "rare", "epic", "legendary"]:
                cand = copy.deepcopy(best_e)
                cand["items"].append({"id": add_id, "x": pos[0], "y": pos[1], "tier": p_tier, "rarity": r})
                c_wr = eval_cand(p_data, cand, runs=40)
                if c_wr is None: continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                if low <= c_wr <= high:
                    print(f"Stage {idx:02d}: FOUND with +{add_id} {r} -> WR={c_wr}%")
                    found = True
                    break
            if found: break

    # If still not found, try per-item rarities
    if not found:
        cand = copy.deepcopy(best_e)
        for it_idx in range(len(cand["items"])):
            for r in rarities:
                cand["items"][it_idx]["rarity"] = r
                c_wr = eval_cand(p_data, cand, runs=40)
                if c_wr is None: continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                if low <= c_wr <= high:
                    print(f"Stage {idx:02d}: FOUND per-item item[{it_idx}]={r} -> WR={c_wr}%")
                    found = True
                    break
            if found: break

    # Save to enemy directory
    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(best_e, f, indent=2, ensure_ascii=False)
    ok = (best_wr is not None and low <= best_wr <= high)
    print(f"Stage {idx:02d}: Final WR={best_wr}% ({'OK' if ok else 'FAIL'})")
    results.append((idx, low, high, best_wr, ok, best_e))

print("\nCalibration finished! Running final verification on all 30 stages with runs=100...")

# Generate final vs_table.txt
ch_data = json.load(open("tools/gemini/out/round-2/chapters.json", "r", encoding="utf-8"))
stages_meta = {}
for ch in ch_data["chapters"]:
    for st in ch["stages"]:
        i = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        stages_meta[i] = {
            "name": st["name"],
            "kind": st["kind"],
            "enemy_name": st["enemy"]["name"],
            "ch_idx": ch["index"] - 1,
            "st_idx": st["index"] - 1
        }

final_rows = []
for idx in range(1, 31):
    fname = f"stage_{idx:02d}.json"
    with open(P_DIR / fname, "r", encoding="utf-8") as f: p_data = json.load(f)
    with open(E_DIR / fname, "r", encoding="utf-8") as f: e_data = json.load(f)
    wr = eval_cand(p_data, e_data, runs=100)
    low, high = get_target_band(idx)
    kind = stages_meta[idx]["kind"]
    ok = (wr is not None and low <= wr <= high)
    final_rows.append({
        "stage": idx,
        "name": stages_meta[idx]["name"],
        "kind": kind,
        "enemy": e_data["name"],
        "enemy_lvl": e_data["level"],
        "p_lvl": p_data["level"],
        "p_tier": p_data["items"][0].get("tier", 1),
        "p_rarity": p_data["items"][0].get("rarity", "common"),
        "win_rate": wr,
        "band": f"{low:.0f}-{high:.0f}%",
        "ok": ok
    })
    # Update chapters.json stage enemy
    c_i = stages_meta[idx]["ch_idx"]
    s_i = stages_meta[idx]["st_idx"]
    ch_data["chapters"][c_i]["stages"][s_i]["enemy"] = copy.deepcopy(e_data)

out_table = BAL_DIR / "vs_table.txt"
with open(out_table, "w", encoding="utf-8") as f:
    f.write(f"{'Ải':<4} {'Tên Ải':<24} {'Loại':<8} {'Kẻ Địch':<26} {'Tỷ lệ thắng':<12} {'Mục tiêu':<10} {'Đạt?'}\n")
    f.write("-" * 95 + "\n")
    for r in final_rows:
        status = "OK" if r["ok"] else "FAIL"
        f.write(f"{r['stage']:02d}   {r['name'][:22]:<24} {r['kind']:<8} {r['enemy'][:24]:<26} {r['win_rate']:5.1f}%      {r['band']:<10} {status}\n")

# Save round-3/chapters.json
with open(R3 / "chapters.json", "w", encoding="utf-8") as f:
    json.dump(ch_data, f, indent=2, ensure_ascii=False)

print("Generated round-3/chapters.json and round-3/balance/vs_table.txt successfully!")
