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

# Let's test Stage 1:
# Player: quyen sao, thanh phong kiem, o tam y. (240 HP)
# We want player winrate to be 80-90%.
p1 = json.load(open(P_DIR / "stage_01.json", "r", encoding="utf-8"))
for t in [1, 2]:
    for r in ["common", "uncommon", "rare"]:
        for arm in ["thiet_giap_y", "da_ho_y", "o_tam_y"]:
            e1 = {
                "name": "Thảo Khấu Tiên Phong",
                "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 1,
                "items": [
                    {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": t, "rarity": r},
                    {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": t, "rarity": r},
                    {"id": arm, "x": 3, "y": 0, "tier": t, "rarity": r}
                ]
            }
            wr = eval_pair(p1, e1, runs=40)
            if wr is not None and 75 <= wr <= 92:
                print(f"Stage 1 cand: t={t}, r={r}, arm={arm} -> wr={wr}%")
                if 80 <= wr <= 90:
                    with open(E_DIR / "stage_01.json", "w", encoding="utf-8") as f:
                        json.dump(e1, f, indent=2, ensure_ascii=False)
                    print(f"  --> LOCKED STAGE 1: wr={wr}%")
                    break
        else:
            continue
        break
    else:
        continue
    break
