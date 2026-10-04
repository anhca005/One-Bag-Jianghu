import json
import subprocess
import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path("tools/balance").resolve()))
import balance

patch_path = Path("tools/gemini/out/round-3/scratch/test_patch.json")
config_path = Path("tools/gemini/out/round-3/scratch/test_config.json")
loadouts_dir = Path("tools/gemini/out/round-3/loadouts").resolve()

# Test matrix when each tier uses its matching level vs level 10
for tier, lvl in [(1, 1), (3, 8), (6, 18), (9, 30)]:
    cmd = [
        "py", "tools/balance/balance.py", "whatif",
        "--patch", str(patch_path),
        "--loadouts", str(loadouts_dir),
        "--items", "tools/gemini/out/round-3/items-base.json",
        "--battle-config", str(config_path),
        "--tiers", str(tier),
        "--tp", "1,9",
        "--runs", "25",
        "--level", str(lvl)
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    for line in res.stdout.splitlines():
        if "Outside" in line:
            print(f"Tier {tier} (Level {lvl}) -> {line}")
            break
