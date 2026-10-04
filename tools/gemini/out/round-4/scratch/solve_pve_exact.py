import json
import subprocess
import os
import sys
import copy
import tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
ITEMS_PATH = R4 / "items-final.json"
CONFIG_PATH = R4 / "config.json"
SECTS_PATH = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()

items_data = json.load(open(ITEMS_PATH, "r", encoding="utf-8"))
item_sizes = {it["id"]: it["size"] for it in items_data}

def clean_loadout(ld):
    return {
        "name": ld.get("name", "Unit"),
        "cols": ld.get("cols", 5),
        "rows": ld.get("rows", 4),
        "level": ld.get("level", 1),
        "sect": ld.get("sect"),
        "tamPhapLevel": ld.get("tamPhapLevel", 1),
        "items": copy.deepcopy(ld["items"])
    }

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

def eval_pair(p_data, e_data, runs=80):
    if not check_no_overlap(p_data) or not check_no_overlap(e_data):
        return None
    with tempfile.TemporaryDirectory(prefix="match_") as raw_temp:
        temp = Path(raw_temp)
        fa = temp / "p.json"
        fb = temp / "e.json"
        balance.write_json(fa, clean_loadout(p_data), indent=None)
        balance.write_json(fb, clean_loadout(e_data), indent=None)
        return balance.cli_match(classpath, fa, fb, runs, str(ITEMS_PATH), str(SECTS_PATH), str(CONFIG_PATH))

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

# Setup player loadouts 1-5 correctly
p_dir = R4 / "balance" / "player"
players_1_5 = {
    1: {
        "name": "Người Chơi (Ải 1)",
        "cols": 5, "rows": 4, "level": 1, "sect": None, "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "o_tam_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}
        ]
    },
    2: {
        "name": "Người Chơi (Ải 2)",
        "cols": 5, "rows": 4, "level": 2, "sect": None, "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "o_tam_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 0, "tier": 1, "rarity": "common"}
        ]
    },
    3: {
        "name": "Người Chơi (Ải 3)",
        "cols": 5, "rows": 4, "level": 3, "sect": None, "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "o_tam_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "kim_sang_duoc", "x": 3, "y": 1, "tier": 1, "rarity": "common"}
        ]
    },
    4: {
        "name": "Người Chơi (Ải 4)",
        "cols": 5, "rows": 4, "level": 4, "sect": None, "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "o_tam_y", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "kim_sang_duoc", "x": 3, "y": 1, "tier": 1, "rarity": "common"},
            {"id": "soi_nanh_canh", "x": 4, "y": 0, "tier": 1, "rarity": "uncommon"}
        ]
    },
    5: {
        "name": "Người Chơi (Ải 5)",
        "cols": 5, "rows": 4, "level": 5, "sect": None, "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "rare"},
            {"id": "o_tam_y", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "kim_sang_duoc", "x": 3, "y": 1, "tier": 1, "rarity": "uncommon"},
            {"id": "soi_nanh_canh", "x": 4, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "thai_thanh_tam_kinh", "x": 3, "y": 2, "tier": 1, "rarity": "uncommon"}
        ]
    }
}

for k, p in players_1_5.items():
    balance.write_json(p_dir / f"stage_{k:02d}.json", p)

print("Validated players 1-5 without overlap.")

# Calibration loop for 30 stages
ch_path = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data" / "Pve" / "chapters.json"
chapters_doc = json.load(open(ch_path, "r", encoding="utf-8"))

e_dir = R4 / "balance" / "enemy"
e_dir.mkdir(parents=True, exist_ok=True)

stages_flat = []
for ch_idx, ch in enumerate(chapters_doc["chapters"]):
    for st_idx, st in enumerate(ch["stages"]):
        stages_flat.append((ch_idx, st_idx, st))

vs_rows = []
rarities = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]

for idx in range(1, 31):
    low, high = get_target_band(idx)
    target = (low + high) / 2.0
    ch_idx, st_idx, st = stages_flat[idx - 1]
    p_data = json.load(open(p_dir / f"stage_{idx:02d}.json", "r", encoding="utf-8"))
    e_data = copy.deepcopy(st["enemy"])
    e_data.pop("portrait", None)
    e_data["cols"] = 5
    e_data["rows"] = 4
    
    wr = eval_pair(p_data, e_data, runs=80)
    print(f"\nStage {idx:02d} ({low:.0f}-{high:.0f}%): Baseline WR={wr:.1f}%")
    
    if wr is not None and low <= wr <= high:
        calibrated_e = e_data
        final_wr = wr
    else:
        best_e = copy.deepcopy(e_data)
        best_wr = wr if wr is not None else 0.0
        best_diff = abs(best_wr - target)
        
        # 1. Level adjustments
        curr_lvl = e_data.get("level", idx)
        for delta_lvl in range(-5, 8):
            lvl = max(1, curr_lvl + delta_lvl)
            cand = copy.deepcopy(e_data)
            cand["level"] = lvl
            c_wr = eval_pair(p_data, cand, runs=40)
            if c_wr is None: continue
            diff = abs(c_wr - target)
            if diff < best_diff:
                best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
            if low <= c_wr <= high:
                break
        
        # 2. Tier / Rarity adjustment if still outside
        if not (low <= best_wr <= high):
            for t_shift in [0, 1, -1, 2, -2]:
                for r in rarities:
                    for delta_lvl in range(-4, 6):
                        lvl = max(1, curr_lvl + delta_lvl)
                        cand = copy.deepcopy(e_data)
                        cand["level"] = lvl
                        for it in cand["items"]:
                            it["tier"] = max(1, it.get("tier", 1) + t_shift)
                            it["rarity"] = r
                        c_wr = eval_pair(p_data, cand, runs=40)
                        if c_wr is None: continue
                        diff = abs(c_wr - target)
                        if diff < best_diff:
                            best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                        if low <= c_wr <= high:
                            break
                    if low <= best_wr <= high:
                        break
                if low <= best_wr <= high:
                    break
        
        # 3. Add small defensive item / pill if enemy still too weak (wr > high)
        if not (low <= best_wr <= high) and best_wr > high:
            for add_id in ["boi_nguyen_dan", "kim_sang_duoc", "da_ho_y", "thiet_giap_y"]:
                pos = find_free_pos(best_e["items"], add_id)
                if pos is None: continue
                cand = copy.deepcopy(best_e)
                cand["items"].append({
                    "id": add_id,
                    "x": pos[0],
                    "y": pos[1],
                    "tier": cand["items"][0].get("tier", 1),
                    "rarity": cand["items"][0].get("rarity", "common")
                })
                for delta_lvl in range(-3, 5):
                    cand["level"] = max(1, best_e["level"] + delta_lvl)
                    c_wr = eval_pair(p_data, cand, runs=40)
                    if c_wr is None: continue
                    diff = abs(c_wr - target)
                    if diff < best_diff:
                        best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                    if low <= c_wr <= high:
                        break
                if low <= best_wr <= high:
                    break

        # Verify with 100 runs
        final_wr = eval_pair(p_data, best_e, runs=100)
        calibrated_e = best_e
        print(f"  -> Calibrated Stage {idx:02d}: Level={calibrated_e['level']}, WR={final_wr:.1f}%")

    # Update chapters_doc
    st["enemy"]["level"] = calibrated_e["level"]
    st["enemy"]["items"] = calibrated_e["items"]
    if "sect" in calibrated_e:
        st["enemy"]["sect"] = calibrated_e["sect"]
    if "tamPhapLevel" in calibrated_e:
        st["enemy"]["tamPhapLevel"] = calibrated_e["tamPhapLevel"]

    balance.write_json(e_dir / f"stage_{idx:02d}.json", calibrated_e)

    ok_str = "ĐẠT" if low <= final_wr <= high else "CHƯA ĐẠT"
    vs_rows.append({
        "idx": idx,
        "name": st["name"],
        "kind": st["kind"],
        "enemy_name": st["enemy"]["name"],
        "enemy_lvl": calibrated_e["level"],
        "winrate": final_wr,
        "target": f"{low:.0f}-{high:.0f}%",
        "ok": ok_str
    })

balance.write_json(R4 / "chapters.json", chapters_doc)
print("\nSaved round-4/chapters.json successfully!")

table_lines = []
table_lines.append(f"{'Ải':<4} {'Tên Ải':<24} {'Loại':<8} {'Kẻ Địch':<26} {'Cấp Địch':<10} {'Tỷ Lệ Thắng':<14} {'Mục Tiêu':<10} {'Đạt?'}")
table_lines.append("-" * 105)
for r in vs_rows:
    table_lines.append(
        f"{r['idx']:02d}   {r['name']:<24} {r['kind']:<8} {r['enemy_name']:<26} {r['enemy_lvl']:<10} "
        f"{r['winrate']:5.1f}%         {r['target']:<10} {r['ok']}"
    )
table_lines.append("-" * 105)

with open(R4 / "balance" / "vs_table.txt", "w", encoding="utf-8") as f:
    f.write("\n".join(table_lines) + "\n")

print("Saved round-4/balance/vs_table.txt successfully!")

