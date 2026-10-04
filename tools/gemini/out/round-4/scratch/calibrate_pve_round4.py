import json
import subprocess
import os
import sys
import copy
import tempfile
import shutil
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
TABLES = R4 / "tables"
TABLES.mkdir(parents=True, exist_ok=True)
LOADOUTS = R4 / "loadouts"

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

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

def eval_match(p_data, e_data, runs=80):
    with tempfile.TemporaryDirectory(prefix="match_") as raw_temp:
        temp = Path(raw_temp)
        fa = temp / "p.json"
        fb = temp / "e.json"
        balance.write_json(fa, clean_loadout(p_data), indent=None)
        balance.write_json(fb, clean_loadout(e_data), indent=None)
        return balance.cli_match(classpath, fa, fb, runs, str(ITEMS_PATH), str(SECTS_PATH), str(CONFIG_PATH))

# -------------------------------------------------------------
# Part 1: PvE Calibration
# -------------------------------------------------------------
def calibrate_pve():
    print("=== CALIBRATING PVE STAGES ===")
    ch_path = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data" / "Pve" / "chapters.json"
    chapters_doc = json.load(open(ch_path, "r", encoding="utf-8"))

    p_dir = R4 / "balance" / "player"
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
        ch_idx, st_idx, st = stages_flat[idx - 1]
        p_data = json.load(open(p_dir / f"stage_{idx:02d}.json", "r", encoding="utf-8"))
        e_data = copy.deepcopy(st["enemy"])
        
        # Test current enemy
        wr = eval_match(p_data, e_data, runs=80)
        print(f"Stage {idx:02d} ({low:.0f}-{high:.0f}%): Initial WR = {wr:.1f}%")

        if low <= wr <= high:
            calibrated_enemy = e_data
            final_wr = wr
        else:
            # Calibrate enemy
            best_e = copy.deepcopy(e_data)
            best_wr = wr
            best_diff = abs(wr - (low + high) / 2.0)
            
            # Grid search on enemy level and items tier/rarity
            curr_lvl = e_data.get("level", idx)
            for lvl_offset in [0, 1, -1, 2, -2, 3, -3, 4, 5]:
                target_lvl = max(1, curr_lvl + lvl_offset)
                for t_offset in [0, 1, -1]:
                    for r in rarities:
                        cand = copy.deepcopy(e_data)
                        cand["level"] = target_lvl
                        for it in cand["items"]:
                            curr_t = it.get("tier", 1)
                            cand_t = max(1, curr_t + t_offset)
                            it["tier"] = cand_t
                            it["rarity"] = r
                        c_wr = eval_match(p_data, cand, runs=40)
                        diff = abs(c_wr - (low + high) / 2.0)
                        if diff < best_diff:
                            best_diff, best_wr, best_e = diff, c_wr, copy.deepcopy(cand)
                        if low <= c_wr <= high:
                            break
                    if low <= best_wr <= high:
                        break
                if low <= best_wr <= high:
                    break
            
            # Re-verify best with 100 runs
            final_wr = eval_match(p_data, best_e, runs=100)
            if not (low <= final_wr <= high):
                # Minor fine-tune level
                for adj_lvl in [best_e["level"] - 1, best_e["level"] + 1, best_e["level"] - 2, best_e["level"] + 2]:
                    if adj_lvl < 1: continue
                    test_adj = copy.deepcopy(best_e)
                    test_adj["level"] = adj_lvl
                    adj_wr = eval_match(p_data, test_adj, runs=100)
                    if abs(adj_wr - (low + high) / 2.0) < abs(final_wr - (low + high) / 2.0):
                        final_wr = adj_wr
                        best_e = test_adj
                    if low <= final_wr <= high:
                        break
            
            calibrated_enemy = best_e
            print(f"  -> Calibrated Stage {idx:02d}: Level={calibrated_enemy['level']}, WR={final_wr:.1f}%")

        # Save calibrated enemy to chapters_doc
        st["enemy"]["level"] = calibrated_enemy["level"]
        st["enemy"]["items"] = calibrated_enemy["items"]
        if "sect" in calibrated_enemy:
            st["enemy"]["sect"] = calibrated_enemy["sect"]
        if "tamPhapLevel" in calibrated_enemy:
            st["enemy"]["tamPhapLevel"] = calibrated_enemy["tamPhapLevel"]

        # Save enemy json
        balance.write_json(e_dir / f"stage_{idx:02d}.json", calibrated_enemy)

        ok_str = "ĐẠT" if low <= final_wr <= high else "CHƯA ĐẠT"
        vs_rows.append({
            "idx": idx,
            "name": st["name"],
            "kind": st["kind"],
            "enemy_name": st["enemy"]["name"],
            "enemy_lvl": calibrated_enemy["level"],
            "winrate": final_wr,
            "target": f"{low:.0f}-{high:.0f}%",
            "ok": ok_str
        })

    # Save calibrated chapters.json
    balance.write_json(R4 / "chapters.json", chapters_doc)
    print("Saved round-4/chapters.json successfully!")

    # Write vs_table.txt
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

if __name__ == "__main__":
    calibrate_pve()
