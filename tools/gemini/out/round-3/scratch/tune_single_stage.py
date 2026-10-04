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

ITEMS = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"

def get_target_band(kind):
    if kind == "boss":
        return 50.0, 60.0
    elif kind == "elite":
        return 60.0, 70.0
    return 80.0, 90.0

def eval_candidate(cand_enemy, stage_idx, runs=60):
    fname = f"stage_{stage_idx:02d}.json"
    shutil.copy(P_DIR / fname, temp_a / fname)
    with open(temp_b / fname, "w", encoding="utf-8") as f:
        json.dump(cand_enemy, f, indent=2, ensure_ascii=False)
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
            for p in line.strip().split():
                if p.endswith("%"):
                    try:
                        return float(p[:-1])
                    except ValueError:
                        pass
    return None

# Let's test stage 1:
with open(E_DIR / "stage_01.json", "r", encoding="utf-8") as f:
    orig = json.load(f)

print("Original stage 1 enemy:", orig)
for e_lvl in [1, 2, 3]:
    for weapon_rarity in ["common", "uncommon"]:
        for armor_rarity in ["common", "uncommon", "rare"]:
            # test adding a 2nd weapon or small armor
            for extra in [
                None,
                {"id": "thiet_giap_y", "x": 0, "y": 2, "tier": 1, "rarity": "common"},
                {"id": "boi_nguyen_dan", "x": 4, "y": 3, "tier": 1, "rarity": "common"},
                {"id": "thiet_cot_quyen_sao", "x": 0, "y": 2, "tier": 1, "rarity": "common"},
                {"id": "thanh_phong_kiem", "x": 4, "y": 0, "tier": 1, "rarity": "common"},
            ]:
                cand = copy.deepcopy(orig)
                cand["level"] = e_lvl
                cand["items"][0]["rarity"] = weapon_rarity
                cand["items"][1]["rarity"] = armor_rarity
                if extra:
                    cand["items"].append(extra)
                wr = eval_candidate(cand, 1, runs=20)
                extra_id = extra['id'] if extra else None
                print(f"lvl={e_lvl}, w={weapon_rarity}, a={armor_rarity}, extra={extra_id} -> WR={wr}%")
                if wr is not None and 80.0 <= wr <= 90.0:
                    print("PERFECT MATCH FOR STAGE 1!")
                    sys.exit(0)

