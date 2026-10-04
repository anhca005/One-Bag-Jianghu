import json
import subprocess
import shutil
import os
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R3 = ROOT / "tools" / "gemini" / "out" / "round-3"
R2 = ROOT / "tools" / "gemini" / "out" / "round-2"

BAL_DIR = R3 / "balance"
P_DIR = BAL_DIR / "player"
E_DIR = BAL_DIR / "enemy"
temp_a = BAL_DIR / "temp_a"
temp_b = BAL_DIR / "temp_b"

P_DIR.mkdir(parents=True, exist_ok=True)
E_DIR.mkdir(parents=True, exist_ok=True)
temp_a.mkdir(parents=True, exist_ok=True)
temp_b.mkdir(parents=True, exist_ok=True)

ITEMS = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"

# Load round 2 chapters
with open(R2 / "chapters.json", "r", encoding="utf-8") as f:
    chapters_data = json.load(f)

stages_info = {}
for ch in chapters_data["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        stages_info[idx] = {
            "name": st["name"],
            "kind": st["kind"],
            "enemy_name": st["enemy"]["name"],
            "enemy_def": st["enemy"],
            "chapter_idx": ch["index"] - 1,
            "stage_in_ch": st["index"] - 1
        }

def get_target_band(kind):
    if kind == "boss":
        return 50.0, 60.0
    elif kind == "elite":
        return 60.0, 70.0
    return 80.0, 90.0

# 1. Initialize player loadouts
print("Initializing player loadouts (stages 1-30)...")
for idx in range(1, 31):
    fname = f"stage_{idx:02d}.json"
    with open(R2 / "balance" / "player" / fname, "r", encoding="utf-8") as f:
        p_data = json.load(f)
    p_data.pop("hp", None)
    p_data["level"] = idx
    if idx < 10:
        p_data["sect"] = None
    else:
        p_data["sect"] = "thai_thanh"
        p_data["tamPhapLevel"] = 1
    with open(P_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(p_data, f, indent=2, ensure_ascii=False)

# 2. Initialize enemy loadouts
print("Initializing enemy loadouts (stages 1-30)...")
for idx in range(1, 31):
    fname = f"stage_{idx:02d}.json"
    with open(R2 / "balance" / "enemy" / fname, "r", encoding="utf-8") as f:
        e_data = json.load(f)
    e_data.pop("hp", None)
    e_data["level"] = idx
    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(e_data, f, indent=2, ensure_ascii=False)

def eval_single_stage(idx, runs=60):
    fname = f"stage_{idx:02d}.json"
    shutil.copy(P_DIR / fname, temp_a / fname)
    shutil.copy(E_DIR / fname, temp_b / fname)
    
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
            parts = line.strip().split()
            for p in parts:
                if p.endswith("%"):
                    try:
                        return float(p[:-1]), line.strip()
                    except ValueError:
                        pass
    return None, res.stdout

print("\nTuning PvE stages to hit target bands...")
tuned_levels = {}

for idx in range(1, 31):
    info = stages_info[idx]
    kind = info["kind"]
    low, high = get_target_band(kind)
    fname = f"stage_{idx:02d}.json"
    with open(E_DIR / fname, "r", encoding="utf-8") as f:
        e_data = json.load(f)
        
    wr, _ = eval_single_stage(idx, runs=60)
    print(f"Stage {idx:02d} ({kind:6}, band {low:.0f}-{high:.0f}%): init level={e_data['level']} -> WR={wr}%")
    
    if wr is not None and low <= wr <= high:
        tuned_levels[idx] = e_data["level"]
        continue
        
    # Search for level in [1, 40]
    best_lvl = e_data["level"]
    best_diff = 999
    
    # If WR is too high (player too strong), enemy level needs to increase
    # If WR is too low (enemy too strong), enemy level needs to decrease
    lvl_min = 1
    lvl_max = 35
    
    for lvl in range(1, 35):
        e_data["level"] = lvl
        with open(E_DIR / fname, "w", encoding="utf-8") as f:
            json.dump(e_data, f, indent=2, ensure_ascii=False)
        wr, _ = eval_single_stage(idx, runs=50)
        if wr is None:
            continue
        target = (low + high) / 2.0
        diff = abs(wr - target)
        if diff < best_diff:
            best_diff = diff
            best_lvl = lvl
        if low <= wr <= high:
            print(f"  -> Found in-band level: {lvl} (WR={wr:.1f}%)")
            best_lvl = lvl
            break
            
    e_data["level"] = best_lvl
    with open(E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(e_data, f, indent=2, ensure_ascii=False)
    tuned_levels[idx] = best_lvl
    print(f"  Stage {idx:02d} Final Level: {best_lvl}")

print("\nDone tuning all stages!")
