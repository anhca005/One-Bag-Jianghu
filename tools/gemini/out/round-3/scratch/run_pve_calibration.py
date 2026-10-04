import json
import subprocess
import shutil
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
BAL_DIR = R3 / "balance"
P_DIR = BAL_DIR / "player"
E_DIR = BAL_DIR / "enemy"

ITEMS_PATH = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()

items_data = json.load(open(ITEMS_PATH, "r", encoding="utf-8"))
item_sizes = {it["id"]: it["size"] for it in items_data}

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

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

def find_free_pos(placed_items, new_item_id, cols=5, rows=4):
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

def clean_loadout(ld):
    return {
        "name": ld["name"],
        "cols": ld.get("cols", 5),
        "rows": ld.get("rows", 4),
        "level": ld["level"],
        "sect": ld.get("sect"),
        "tamPhapLevel": ld.get("tamPhapLevel", 1),
        "items": copy.deepcopy(ld["items"])
    }

def eval_pair(p_data, e_data, runs=80):
    if not check_no_overlap(e_data) or not check_no_overlap(p_data):
        return None
    with tempfile.TemporaryDirectory(prefix="pve_") as raw_temp:
        temp = Path(raw_temp)
        fa = temp / "p.json"
        fb = temp / "e.json"
        balance.write_json(fa, clean_loadout(p_data), indent=None)
        balance.write_json(fb, clean_loadout(e_data), indent=None)
        return balance.cli_match(classpath, fa, fb, runs, ITEMS_PATH, SECTS, CONFIG)

# Load round 2 chapters to get pristine base enemy setups
ch2 = json.load(open("tools/gemini/out/round-2/chapters.json", "r", encoding="utf-8"))
stages_meta = {}
for ch in ch2["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        e = copy.deepcopy(st["enemy"])
        e.pop("hp", None)
        e["level"] = idx
        e["cols"] = 5
        e["rows"] = 4
        stages_meta[idx] = {
            "name": st["name"],
            "kind": st["kind"],
            "enemy": e,
            "ch_idx": ch["index"] - 1,
            "st_idx": st["index"] - 1
        }

rarities = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]

calibrated_enemies = {}

for idx in range(1, 31):
    low, high = get_target_band(idx)
    target = (low + high) / 2.0
    fname = f"stage_{idx:02d}.json"
    
    with open(P_DIR / fname, "r", encoding="utf-8") as f: p_data = json.load(f)
    if idx <= 10:
        p_data["sect"] = None
        with open(P_DIR / fname, "w", encoding="utf-8") as f: json.dump(p_data, f, indent=2, ensure_ascii=False)
        
    e_base = stages_meta[idx]["enemy"]
    
    # Evaluate baseline
    wr = eval_pair(p_data, e_base, runs=50)
    print(f"\nStage {idx:02d} ({low:.0f}-{high:.0f}%): baseline WR={wr}%")
    if wr is not None and low <= wr <= high:
        calibrated_enemies[idx] = e_base
        with open(E_DIR / fname, "w", encoding="utf-8") as f: json.dump(e_base, f, indent=2, ensure_ascii=False)
        continue

    best_e = copy.deepcopy(e_base)
    best_wr = wr
    best_diff = abs(wr - target) if wr is not None else 999
    found = False

    # 1. Try uniform tier & rarity shifts
    p_tier = p_data["items"][0].get("tier", 1)
    for t in [p_tier, p_tier + 1, max(1, p_tier - 1)]:
        for r in rarities:
            cand = copy.deepcopy(e_base)
            for it in cand["items"]:
                it["tier"] = t
                it["rarity"] = r
            c_wr = eval_pair(p_data, cand, runs=40)
            if c_wr is None: continue
            diff = abs(c_wr - target)
            if diff < best_diff:
                best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
            if low <= c_wr <= high:
                print(f"  Stage {idx:02d}: FOUND uniform T{t} {r} -> WR={c_wr}%")
                found = True
                break
        if found: break

    # 2. Try adding a small item (1x1 or 2x1) if enemy still losing > high
    if not found and best_wr is not None and best_wr > high:
        for add_id in ["boi_nguyen_dan", "bach_ngoc_boi", "thiet_giap_y", "da_ho_y"]:
            pos = find_free_pos(best_e["items"], add_id)
            if pos is None: continue
            for r in ["common", "uncommon", "rare", "epic", "legendary"]:
                cand = copy.deepcopy(best_e)
                cand["items"].append({"id": add_id, "x": pos[0], "y": pos[1], "tier": p_tier, "rarity": r})
                c_wr = eval_pair(p_data, cand, runs=40)
                if c_wr is None: continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                if low <= c_wr <= high:
                    print(f"  Stage {idx:02d}: FOUND +{add_id} {r} -> WR={c_wr}%")
                    found = True
                    break
            if found: break

    # 3. Try per-item adjustments
    if not found:
        cand = copy.deepcopy(best_e)
        for i_idx in range(len(cand["items"])):
            for r in rarities:
                cand["items"][i_idx]["rarity"] = r
                c_wr = eval_pair(p_data, cand, runs=40)
                if c_wr is None: continue
                diff = abs(c_wr - target)
                if diff < best_diff:
                    best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                if low <= c_wr <= high:
                    print(f"  Stage {idx:02d}: FOUND per-item item[{i_idx}]={r} -> WR={c_wr}%")
                    found = True
                    break
            if found: break

    calibrated_enemies[idx] = best_e
    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(best_e, f, indent=2, ensure_ascii=False)
    ok = (best_wr is not None and low <= best_wr <= high)
    print(f"Stage {idx:02d}: Final WR={best_wr}% ({'OK' if ok else 'FAIL'})")

# Verification pass with runs=100
print("\nRunning final 100-run verification across all 30 stages...")
final_rows = []
for idx in range(1, 31):
    fname = f"stage_{idx:02d}.json"
    with open(P_DIR / fname, "r", encoding="utf-8") as f: p_data = json.load(f)
    with open(E_DIR / fname, "r", encoding="utf-8") as f: e_data = json.load(f)
    wr = eval_pair(p_data, e_data, runs=100)
    low, high = get_target_band(idx)
    kind = stages_meta[idx]["kind"]
    ok = (wr is not None and low <= wr <= high)
    final_rows.append({
        "stage": idx,
        "name": stages_meta[idx]["name"],
        "kind": kind,
        "enemy": e_data["name"],
        "win_rate": wr,
        "band": f"{low:.0f}-{high:.0f}%",
        "ok": ok
    })
    # Update chapter stage enemy
    c_i = stages_meta[idx]["ch_idx"]
    s_i = stages_meta[idx]["st_idx"]
    ch2["chapters"][c_i]["stages"][s_i]["enemy"] = copy.deepcopy(e_data)

out_table = BAL_DIR / "vs_table.txt"
with open(out_table, "w", encoding="utf-8") as f:
    f.write(f"{'Ải':<4} {'Tên Ải':<24} {'Loại':<8} {'Kẻ Địch':<26} {'Tỷ lệ thắng':<12} {'Mục tiêu':<10} {'Đạt?'}\n")
    f.write("-" * 95 + "\n")
    for r in final_rows:
        status = "OK" if r["ok"] else "FAIL"
        f.write(f"{r['stage']:02d}   {r['name'][:22]:<24} {r['kind']:<8} {r['enemy'][:24]:<26} {r['win_rate']:5.1f}%      {r['band']:<10} {status}\n")

# Write chapters.json
with open(R3 / "chapters.json", "w", encoding="utf-8") as f:
    json.dump(ch2, f, indent=2, ensure_ascii=False)

print("Generated round-3/chapters.json and round-3/balance/vs_table.txt successfully!")
