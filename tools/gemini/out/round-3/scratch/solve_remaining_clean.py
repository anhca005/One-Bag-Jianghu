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

def eval_pair(p_data, e_data, runs=80):
    with tempfile.TemporaryDirectory(prefix="pve_") as raw_temp:
        temp = Path(raw_temp)
        fa = temp / "p.json"
        fb = temp / "e.json"
        clean_p = {k: v for k, v in p_data.items() if k in ('items', 'cols', 'level', 'rows', 'name', 'tamPhapLevel', 'sect')}
        clean_e = {k: v for k, v in e_data.items() if k in ('items', 'cols', 'level', 'rows', 'name', 'tamPhapLevel', 'sect')}
        balance.write_json(fa, clean_p, indent=None)
        balance.write_json(fb, clean_e, indent=None)
        return balance.cli_match(classpath, fa, fb, runs, ITEMS_PATH, SECTS, CONFIG)

rarities = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]

# 1. Stage 3 (Target: 80-90%)
print("Solving Stage 3...")
p3 = json.load(open(P_DIR / "stage_03.json", encoding="utf-8"))
for t in [1, 2]:
    for r in ["common", "uncommon", "rare"]:
        for r_ar in ["common", "uncommon"]:
            cand = {
                "name": "Ải 3: Hoang Dã Sói Xám",
                "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 3,
                "items": [
                    {"id": "soi_nanh_canh", "x": 0, "y": 0, "tier": t, "rarity": r},
                    {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": t, "rarity": r},
                    {"id": "o_tam_y", "x": 2, "y": 0, "tier": 1, "rarity": r_ar}
                ]
            }
            wr = eval_pair(p3, cand, runs=60)
            if wr is not None and 80 <= wr <= 90:
                print(f"  Stage 3 SUCCESS: t={t}, r={r}, r_ar={r_ar} -> {wr}%")
                json.dump(cand, open(E_DIR / "stage_03.json", "w", encoding="utf-8"), indent=2, ensure_ascii=False)
                break
        else: continue
        break
    else: continue
    break

# 2. Stage 7 (Target: 80-90%)
print("Solving Stage 7...")
p7 = json.load(open(P_DIR / "stage_07.json", encoding="utf-8"))
for t in [2, 3]:
    for r in ["rare", "epic"]:
        cand = {
            "name": "Ải 7: Liệt Diệm Bang Tinh Anh",
            "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 7,
            "items": [
                {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": t, "rarity": r},
                {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": t, "rarity": r},
                {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": t, "rarity": r}
            ]
        }
        wr = eval_pair(p7, cand, runs=60)
        if wr is not None and 80 <= wr <= 90:
            print(f"  Stage 7 SUCCESS: t={t}, r={r} -> {wr}%")
            json.dump(cand, open(E_DIR / "stage_07.json", "w", encoding="utf-8"), indent=2, ensure_ascii=False)
            break
    else: continue
    break

# 3. Stage 9 (Target: 80-90%)
print("Solving Stage 9...")
p9 = json.load(open(P_DIR / "stage_09.json", encoding="utf-8"))
e9 = json.load(open(E_DIR / "stage_09.json", encoding="utf-8"))
for t in [2, 3]:
    for r in ["epic", "legendary"]:
        cand = copy.deepcopy(e9)
        cand["items"][0]["tier"] = t
        cand["items"][0]["rarity"] = r
        cand["items"][1]["tier"] = t
        wr = eval_pair(p9, cand, runs=60)
        if wr is not None and 80 <= wr <= 90:
            print(f"  Stage 9 SUCCESS: t={t}, r={r} -> {wr}%")
            json.dump(cand, open(E_DIR / "stage_09.json", "w", encoding="utf-8"), indent=2, ensure_ascii=False)
            break
    else: continue
    break

# 4. Stage 10 (Boss: 50-60%)
print("Solving Stage 10...")
p10 = json.load(open(P_DIR / "stage_10.json", encoding="utf-8"))
e10 = json.load(open(E_DIR / "stage_10.json", encoding="utf-8"))
for r in ["epic", "legendary", "supreme"]:
    cand = copy.deepcopy(e10)
    cand["items"][0]["rarity"] = r # liet_diem_dao
    cand["items"][1]["rarity"] = r # heo_rung_bap_giap
    wr = eval_pair(p10, cand, runs=60)
    if wr is not None and 50 <= wr <= 60:
        print(f"  Stage 10 SUCCESS: r={r} -> {wr}%")
        json.dump(cand, open(E_DIR / "stage_10.json", "w", encoding="utf-8"), indent=2, ensure_ascii=False)
        break

# 5. Stage 11 (Target: 80-90%)
print("Solving Stage 11...")
p11 = json.load(open(P_DIR / "stage_11.json", encoding="utf-8"))
for t in [2, 3]:
    for r in ["rare", "epic"]:
        cand = {
            "name": "Ải 11: Vạn Độc Môn Đồ",
            "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 11,
            "items": [
                {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": t, "rarity": r},
                {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": t, "rarity": r},
                {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": t, "rarity": r},
                {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": t, "rarity": r}
            ]
        }
        wr = eval_pair(p11, cand, runs=60)
        if wr is not None and 80 <= wr <= 90:
            print(f"  Stage 11 SUCCESS: t={t}, r={r} -> {wr}%")
            json.dump(cand, open(E_DIR / "stage_11.json", "w", encoding="utf-8"), indent=2, ensure_ascii=False)
            break
    else: continue
    break

# 6. Stage 13 (Target: 80-90%)
print("Solving Stage 13...")
p13 = json.load(open(P_DIR / "stage_13.json", encoding="utf-8"))
for t in [2, 3]:
    for r in ["rare", "epic"]:
        cand = {
            "name": "Ải 13: Kịch Độc Tri Chu",
            "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 13,
            "items": [
                {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": t, "rarity": r},
                {"id": "soi_nanh_canh", "x": 1, "y": 0, "tier": t, "rarity": r},
                {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": t, "rarity": r},
                {"id": "man_xa_doc_tui", "x": 4, "y": 0, "tier": t, "rarity": r}
            ]
        }
        wr = eval_pair(p13, cand, runs=60)
        if wr is not None and 80 <= wr <= 90:
            print(f"  Stage 13 SUCCESS: t={t}, r={r} -> {wr}%")
            json.dump(cand, open(E_DIR / "stage_13.json", "w", encoding="utf-8"), indent=2, ensure_ascii=False)
            break
    else: continue
    break

print("Finished batch 1-13!")
