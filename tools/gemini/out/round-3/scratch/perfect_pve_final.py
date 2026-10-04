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

# Let's check stage 2:
# Player: quyen sao, thanh phong kiem, o tam y. (level 2)
# Enemy: thanh phong kiem (1, legendary), o tam y (1, legendary) -> wr was 90.8%
p2 = json.load(open(P_DIR / "stage_02.json", encoding="utf-8"))
e2 = json.load(open(E_DIR / "stage_02.json", encoding="utf-8"))
print("Tuning Stage 2...")
for r1 in ["rare", "epic", "legendary"]:
    for r2 in ["rare", "epic", "legendary"]:
        cand = copy.deepcopy(e2)
        cand["items"][0]["rarity"] = r1
        cand["items"][1]["rarity"] = r2
        wr = eval_pair(p2, cand, runs=80)
        if wr is not None and 80 <= wr <= 90:
            print(f"Stage 2 locked: r1={r1}, r2={r2} -> wr={wr}%")
            with open(E_DIR / "stage_02.json", "w", encoding="utf-8") as f:
                json.dump(cand, f, indent=2, ensure_ascii=False)
            break
    else: continue
    break

# Stage 3:
# Player level 3. Enemy: Hoang Dã Sói Xám.
# Items: soi_nanh_canh (1x2 at 0,0), da_ho_y (1x2 at 1,0), heo_rung_bap_giap (2x2 at 2,0), that_tinh_tieu (1x2 at 4,0)
p3 = json.load(open(P_DIR / "stage_03.json", encoding="utf-8"))
print("Tuning Stage 3...")
for r in ["common", "uncommon", "rare"]:
    for t in [1, 2]:
        cand = {
            "name": "Ải 3: Hoang Dã Sói Xám",
            "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 3,
            "items": [
                {"id": "soi_nanh_canh", "x": 0, "y": 0, "tier": t, "rarity": r},
                {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": t, "rarity": r},
                {"id": "o_tam_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
            ]
        }
        wr = eval_pair(p3, cand, runs=80)
        if wr is not None and 80 <= wr <= 90:
            print(f"Stage 3 locked: t={t}, r={r} -> wr={wr}%")
            with open(E_DIR / "stage_03.json", "w", encoding="utf-8") as f:
                json.dump(cand, f, indent=2, ensure_ascii=False)
            break
    else: continue
    break

# Stage 5 (Elite: 60-70%):
# wr was 57.6%. Slightly too hard! We want wr in 60-70%.
p5 = json.load(open(P_DIR / "stage_05.json", encoding="utf-8"))
e5 = json.load(open(E_DIR / "stage_05.json", encoding="utf-8"))
print("Tuning Stage 5...")
for r_wp in ["rare", "epic", "legendary"]:
    for r_ar in ["uncommon", "rare", "epic"]:
        cand = copy.deepcopy(e5)
        cand["items"][0]["rarity"] = r_wp # thanh_phong_kiem
        cand["items"][2]["rarity"] = r_ar # kim_tuyen_y
        wr = eval_pair(p5, cand, runs=80)
        if wr is not None and 60 <= wr <= 70:
            print(f"Stage 5 locked: wp={r_wp}, ar={r_ar} -> wr={wr}%")
            with open(E_DIR / "stage_05.json", "w", encoding="utf-8") as f:
                json.dump(cand, f, indent=2, ensure_ascii=False)
            break
    else: continue
    break

# Stage 7:
# wr was 97.5%.
p7 = json.load(open(P_DIR / "stage_07.json", encoding="utf-8"))
e7 = json.load(open(E_DIR / "stage_07.json", encoding="utf-8"))
print("Tuning Stage 7...")
for t in [2, 3, 4]:
    for r in ["rare", "epic", "legendary"]:
        cand = {
            "name": "Ải 7: Liệt Diệm Bang Tinh Anh",
            "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 7,
            "items": [
                {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": t, "rarity": r},
                {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": t, "rarity": r},
                {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": t, "rarity": r},
                {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": t, "rarity": r}
            ]
        }
        wr = eval_pair(p7, cand, runs=60)
        if wr is not None and 80 <= wr <= 90:
            print(f"Stage 7 locked: t={t}, r={r} -> wr={wr}%")
            with open(E_DIR / "stage_07.json", "w", encoding="utf-8") as f:
                json.dump(cand, f, indent=2, ensure_ascii=False)
            break
    else: continue
    break

# Stage 9:
# wr was 90.8%. Slightly too easy.
p9 = json.load(open(P_DIR / "stage_09.json", encoding="utf-8"))
e9 = json.load(open(E_DIR / "stage_09.json", encoding="utf-8"))
print("Tuning Stage 9...")
for t in [2, 3]:
    for r in ["rare", "epic", "legendary"]:
        cand = copy.deepcopy(e9)
        for it in cand["items"]:
            it["tier"] = t
            it["rarity"] = r
        wr = eval_pair(p9, cand, runs=60)
        if wr is not None and 80 <= wr <= 90:
            print(f"Stage 9 locked: t={t}, r={r} -> wr={wr}%")
            with open(E_DIR / "stage_09.json", "w", encoding="utf-8") as f:
                json.dump(cand, f, indent=2, ensure_ascii=False)
            break
    else: continue
    break

# Stage 10 (Boss: 50-60%):
# wr was 61.7%. Slightly too easy.
p10 = json.load(open(P_DIR / "stage_10.json", encoding="utf-8"))
e10 = json.load(open(E_DIR / "stage_10.json", encoding="utf-8"))
print("Tuning Stage 10...")
for r in ["epic", "legendary"]:
    cand = copy.deepcopy(e10)
    cand["items"][0]["rarity"] = r # liet_diem_dao
    wr = eval_pair(p10, cand, runs=80)
    if wr is not None and 50 <= wr <= 60:
        print(f"Stage 10 locked: r={r} -> wr={wr}%")
        with open(E_DIR / "stage_10.json", "w", encoding="utf-8") as f:
            json.dump(cand, f, indent=2, ensure_ascii=False)
        break

print("Finished batch 1!")
