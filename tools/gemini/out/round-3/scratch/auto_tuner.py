import json
import subprocess
import os
import sys
import copy
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, str(Path("tools/balance").resolve()))
import balance

items_base = balance.read_json(Path("tools/gemini/out/round-3/items-base.json"))
sects_base = balance.read_json(Path("server/battle-engine/src/main/resources/data/sects.json"))
loadouts_dir = Path("tools/gemini/out/round-3/loadouts").resolve()

# Base config with tuned values for new HP scale
config_base = {
    "exhaustionStep": 5,
    "maxRegen": 8,
    "burn": {
        "threshold": 50,
        "damage": 55,
        "maxHpPermille": 0,
        "ignoreShield": False,
        "decayPerSecond": 0
    },
    "poison": {
        "threshold": 50,
        "damage": 42,
        "maxHpPermille": 0,
        "ignoreShield": True,
        "decayPerSecond": 0
    },
    "bleed": {
        "threshold": 50,
        "damage": 26,
        "maxHpPermille": 60,
        "ignoreShield": True,
        "decayPerSecond": 0
    }
}

# Base candidate patch
patch = {
    # Heavy weapon buffs
    "khai_son_phu.cooldown": 3.4,
    "khai_son_phu.attack": 26,
    "chan_nhac_chuy.cooldown": 3.2,
    "chan_nhac_chuy.attack": 25,
    "bang_phach_thuong.cooldown": 3.4,
    "bang_phach_thuong.attack": 24,
    
    # mb4 nerf
    "bach_ho_cuong_mong.attack": 3,
    
    # mb1 buff
    "u_minh_quy_trao.attack": 12,
    "kich_doc_chu_nga.attack": 10,
    
    # Fast weapon adjustments
    "thiet_cot_quyen_sao.attack": 5,
    "tu_mau_song_dao.attack": 7,
    "phuc_ho_con.attack": 10,
    "ngoc_cot_phien.attack": 7,
    "truy_hon_cung.attack": 22,
}

patch_path = Path("tools/gemini/out/round-3/scratch/test_patch.json")
config_path = Path("tools/gemini/out/round-3/scratch/test_config.json")

balance.write_json(patch_path, patch)
balance.write_json(config_path, config_base)

def run_matrix(tiers, tp, runs=30, level=10, rarity=None):
    cmd = [
        "py", "tools/balance/balance.py", "whatif",
        "--patch", str(patch_path),
        "--loadouts", str(loadouts_dir),
        "--items", "tools/gemini/out/round-3/items-base.json",
        "--battle-config", str(config_path),
        "--tiers", tiers,
        "--tp", tp,
        "--runs", str(runs),
        "--level", str(level)
    ]
    if rarity:
        cmd.extend(["--rarity", rarity])
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    if res.returncode != 0:
        print(f"Error {res.returncode}:\n{res.stderr}")
    return res.stdout

print("=== Running Matrix A (runs=30, level=10) ===")
out_a = run_matrix("1,3,6,9", "1,9", runs=30, level=10)
print(out_a)

for line in out_a.splitlines():
    if "Outside" in line:
        print(f"RESULT: {line}")
