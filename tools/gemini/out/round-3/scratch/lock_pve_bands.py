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

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

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

def check_all(runs=80):
    res = []
    for idx in range(1, 31):
        fname = f"stage_{idx:02d}.json"
        with open(P_DIR / fname, "r", encoding="utf-8") as f: p_data = json.load(f)
        with open(E_DIR / fname, "r", encoding="utf-8") as f: e_data = json.load(f)
        wr = eval_cand(p_data, e_data, runs=runs)
        low, high = get_target_band(idx)
        kind = "boss" if idx in (10, 20, 30) else ("elite" if idx in (5, 15, 25) else "normal")
        ok = (wr is not None and low <= wr <= high)
        res.append((idx, kind, wr, low, high, ok))
        status = "OK" if ok else f"FAIL (wr={wr}%)"
        print(f"Stage {idx:02d} ({kind:<6} [{low:.0f}-{high:.0f}%]): {status}")
    return res

# Fine-tune the remaining outliers
# Stage 1: give enemy thanh_phong_kiem
with open(E_DIR / "stage_01.json", "r", encoding="utf-8") as f: e1 = json.load(f)
e1["items"].append({"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"})
with open(E_DIR / "stage_01.json", "w", encoding="utf-8") as f: json.dump(e1, f, indent=2, ensure_ascii=False)

# Stage 3: give enemy thanh_phong_kiem
with open(E_DIR / "stage_03.json", "r", encoding="utf-8") as f: e3 = json.load(f)
e3["items"].append({"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"})
with open(E_DIR / "stage_03.json", "w", encoding="utf-8") as f: json.dump(e3, f, indent=2, ensure_ascii=False)

# Stage 7: give enemy extra weapon or armor
with open(E_DIR / "stage_07.json", "r", encoding="utf-8") as f: e7 = json.load(f)
e7["items"].append({"id": "thiet_giap_y", "x": 2, "y": 2, "tier": 2, "rarity": "rare"})
with open(E_DIR / "stage_07.json", "w", encoding="utf-8") as f: json.dump(e7, f, indent=2, ensure_ascii=False)

# Stage 10 (Boss): tune to 50-60%
with open(E_DIR / "stage_10.json", "r", encoding="utf-8") as f: e10 = json.load(f)
for it in e10["items"]:
    it["tier"] = 2
    it["rarity"] = "rare"
with open(E_DIR / "stage_10.json", "w", encoding="utf-8") as f: json.dump(e10, f, indent=2, ensure_ascii=False)

# Stage 13: give enemy extra armor
with open(E_DIR / "stage_13.json", "r", encoding="utf-8") as f: e13 = json.load(f)
e13["items"].append({"id": "thiet_giap_y", "x": 0, "y": 2, "tier": 3, "rarity": "rare"})
with open(E_DIR / "stage_13.json", "w", encoding="utf-8") as f: json.dump(e13, f, indent=2, ensure_ascii=False)

# Stage 16: give enemy extra defense
with open(E_DIR / "stage_16.json", "r", encoding="utf-8") as f: e16 = json.load(f)
e16["items"].append({"id": "da_ho_y", "x": 4, "y": 0, "tier": 3, "rarity": "rare"})
with open(E_DIR / "stage_16.json", "w", encoding="utf-8") as f: json.dump(e16, f, indent=2, ensure_ascii=False)

# Stage 17: give enemy extra defense
with open(E_DIR / "stage_17.json", "r", encoding="utf-8") as f: e17 = json.load(f)
e17["items"].append({"id": "thiet_giap_y", "x": 1, "y": 3, "tier": 3, "rarity": "epic"})
with open(E_DIR / "stage_17.json", "w", encoding="utf-8") as f: json.dump(e17, f, indent=2, ensure_ascii=False)

# Stage 20 (Boss): tune to 50-60%
with open(E_DIR / "stage_20.json", "r", encoding="utf-8") as f: e20 = json.load(f)
for it in e20["items"]:
    it["tier"] = 3
    it["rarity"] = "rare"
with open(E_DIR / "stage_20.json", "w", encoding="utf-8") as f: json.dump(e20, f, indent=2, ensure_ascii=False)

# Stage 21: give enemy extra item
with open(E_DIR / "stage_21.json", "r", encoding="utf-8") as f: e21 = json.load(f)
e21["items"].append({"id": "da_ho_y", "x": 0, "y": 2, "tier": 4, "rarity": "rare"})
with open(E_DIR / "stage_21.json", "w", encoding="utf-8") as f: json.dump(e21, f, indent=2, ensure_ascii=False)

# Stage 23: give enemy extra item
with open(E_DIR / "stage_23.json", "r", encoding="utf-8") as f: e23 = json.load(f)
e23["items"].append({"id": "thiet_giap_y", "x": 3, "y": 2, "tier": 4, "rarity": "rare"})
with open(E_DIR / "stage_23.json", "w", encoding="utf-8") as f: json.dump(e23, f, indent=2, ensure_ascii=False)

# Stage 28: give enemy extra item
with open(E_DIR / "stage_28.json", "r", encoding="utf-8") as f: e28 = json.load(f)
e28["items"].append({"id": "thiet_giap_y", "x": 3, "y": 2, "tier": 5, "rarity": "epic"})
with open(E_DIR / "stage_28.json", "w", encoding="utf-8") as f: json.dump(e28, f, indent=2, ensure_ascii=False)

# Stage 30 (Boss): tune to 50-60%
with open(E_DIR / "stage_30.json", "r", encoding="utf-8") as f: e30 = json.load(f)
for it in e30["items"]:
    it["tier"] = 5
    it["rarity"] = "epic"
with open(E_DIR / "stage_30.json", "w", encoding="utf-8") as f: json.dump(e30, f, indent=2, ensure_ascii=False)

print("\nVerifying all 30 stages...")
check_all(runs=80)
