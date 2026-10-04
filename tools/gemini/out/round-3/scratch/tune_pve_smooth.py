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

with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "chapters.json", "r", encoding="utf-8") as f:
    chapters_data = json.load(f)

stages_info = {}
for ch in chapters_data["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        stages_info[idx] = {
            "name": st["name"],
            "kind": st["kind"],
            "enemy_name": st["enemy"]["name"],
            "enemy": st["enemy"],
            "chapter_idx": ch["index"] - 1,
            "stage_in_ch": st["index"] - 1
        }

def get_target_band(kind):
    if kind == "boss":
        return 50.0, 60.0
    elif kind == "elite":
        return 60.0, 70.0
    return 80.0, 90.0

def eval_single_stage(idx, runs=80):
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
                        return float(p[:-1])
                    except ValueError:
                        pass
    return None

# We can adjust enemy loadouts to hit the bands cleanly
# Let's inspect stage by stage and calibrate
print("Starting PvE calibration...")
results = []
for idx in range(1, 31):
    kind = stages_info[idx]["kind"]
    low, high = get_target_band(kind)
    fname = f"stage_{idx:02d}.json"
    
    with open(P_DIR / fname, "r", encoding="utf-8") as f:
        p_data = json.load(f)
    with open(E_DIR / fname, "r", encoding="utf-8") as f:
        e_data = json.load(f)
        
    wr = eval_single_stage(idx, runs=60)
    print(f"Stage {idx:02d} ({kind:6}, band {low:.0f}-{high:.0f}%): WR={wr}%")
    results.append((idx, stages_info[idx]["name"], kind, stages_info[idx]["enemy_name"], wr))
