import json
import subprocess
import os
import sys
import copy
from pathlib import Path

sys.path.insert(0, str(Path("tools/balance").resolve()))
import balance

# Load items-base and sects
with open("tools/gemini/out/round-3/items-base.json", encoding="utf-8") as f:
    items_base = json.load(f)
with open("server/battle-engine/src/main/resources/data/sects.json", encoding="utf-8") as f:
    sects_base = json.load(f)

# Candidate patch
candidate_patch = {
    # Heavy weapon speedups and adjustments
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
    "truy_hon_dao.attack": 8,
    "tieu_dao_phien.attack": 9,
}

# Update config.json
candidate_config = {
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

# Apply patch to items
items_patched = copy.deepcopy(items_base)
sects_patched = copy.deepcopy(sects_base)
balance.apply_patch(items_patched, sects_patched, candidate_patch)

# Save temp files
temp_items = Path("tools/gemini/out/round-3/scratch/temp_items_patched.json")
temp_config = Path("tools/gemini/out/round-3/scratch/temp_config.json")
with open(temp_items, "w", encoding="utf-8") as f:
    json.dump(items_patched, f, indent=2)
with open(temp_config, "w", encoding="utf-8") as f:
    json.dump(candidate_config, f, indent=2)

print("Saved patched items and config. Running Matrix A...")

# Run Matrix A
cmd = [
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", "tools/gemini/out/round-3/loadouts",
    "--items", str(temp_items),
    "--battle-config", str(temp_config),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "30",
    "--level", "10"
]
env = os.environ.copy()
env["PYTHONUTF8"] = "1"
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
print(res.stdout)
