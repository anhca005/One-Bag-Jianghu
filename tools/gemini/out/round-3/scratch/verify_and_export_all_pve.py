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

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

def eval_pair(p_data, e_data, runs=100):
    with tempfile.TemporaryDirectory(prefix="pve_") as raw_temp:
        temp = Path(raw_temp)
        fa = temp / "p.json"
        fb = temp / "e.json"
        clean_p = {k: v for k, v in p_data.items() if k in ('items', 'cols', 'level', 'rows', 'name', 'tamPhapLevel', 'sect')}
        clean_e = {k: v for k, v in e_data.items() if k in ('items', 'cols', 'level', 'rows', 'name', 'tamPhapLevel', 'sect')}
        balance.write_json(fa, clean_p, indent=None)
        balance.write_json(fb, clean_e, indent=None)
        return balance.cli_match(classpath, fa, fb, runs, ITEMS_PATH, SECTS, CONFIG)

# Load round-2 chapters as template
ch2 = json.load(open(ROOT / "tools/gemini/out/round-2/chapters.json", "r", encoding="utf-8"))

stages_meta = {}
for ch in ch2["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        stages_meta[idx] = {
            "name": st["name"],
            "kind": st["kind"],
            "ch_idx": ch["index"] - 1,
            "st_idx": st["index"] - 1,
            "characterId": st["enemy"].get("characterId", f"enemy_{idx}"),
            "portrait": st["enemy"].get("portrait", f"portrait_{idx}")
        }

results = []
all_ok = True

print("Running 100 runs per stage across all 30 stages...")
for idx in range(1, 31):
    fname = f"stage_{idx:02d}.json"
    p_data = json.load(open(P_DIR / fname, "r", encoding="utf-8"))
    e_data = json.load(open(E_DIR / fname, "r", encoding="utf-8"))
    
    wr = eval_pair(p_data, e_data, runs=100)
    low, high = get_target_band(idx)
    kind = stages_meta[idx]["kind"]
    ok = (wr is not None and low <= wr <= high)
    if not ok:
        all_ok = False
    
    p_items_summary = f"Lv{p_data['level']} T{p_data['items'][0].get('tier', 1)} {p_data['items'][0].get('rarity', 'common')}"
    
    results.append({
        "stage": idx,
        "name": stages_meta[idx]["name"],
        "kind": kind,
        "enemy_name": e_data["name"],
        "enemy_level": e_data["level"],
        "player_level": p_data["level"],
        "player_summary": p_items_summary,
        "win_rate": wr,
        "band": f"{low:.0f}-{high:.0f}%",
        "ok": ok
    })
    print(f"Stage {idx:02d} ({kind:6s}): WR = {wr:5.1f}% [Target: {low:.0f}-{high:.0f}%] -> {'OK' if ok else 'FAIL'}")

    # Update chapters.json
    c_i = stages_meta[idx]["ch_idx"]
    s_i = stages_meta[idx]["st_idx"]
    st_enemy = copy.deepcopy(e_data)
    st_enemy["characterId"] = stages_meta[idx]["characterId"]
    st_enemy["portrait"] = stages_meta[idx]["portrait"]
    # Ensure no 'hp' key
    if "hp" in st_enemy:
        del st_enemy["hp"]
    ch2["chapters"][c_i]["stages"][s_i]["enemy"] = st_enemy

# Write balance/vs_table.txt verbatim
table_path = BAL_DIR / "vs_table.txt"
with open(table_path, "w", encoding="utf-8") as f:
    f.write(f"{'Ải':<4} {'Tên Ải':<24} {'Loại':<8} {'Kẻ Địch':<26} {'Cấp Địch':<10} {'Tỷ Lệ Thắng':<14} {'Mục Tiêu':<10} {'Đạt?'}\n")
    f.write("-" * 105 + "\n")
    for r in results:
        status = "ĐẠT" if r["ok"] else "CHƯA ĐẠT"
        f.write(f"{r['stage']:02d}   {r['name'][:22]:<24} {r['kind']:<8} {r['enemy_name'][:24]:<26} {r['enemy_level']:<10} {r['win_rate']:5.1f}%         {r['band']:<10} {status}\n")

# Save round-3 chapters.json
with open(R3 / "chapters.json", "w", encoding="utf-8") as f:
    json.dump(ch2, f, indent=2, ensure_ascii=False)

print(f"\nALL 30 STAGES STATUS: {'SUCCESS - 100% IN BAND!' if all_ok else 'SOME FAILED'}")
