import json
import subprocess
import os
from pathlib import Path

# We can run rarity-step or whatif with a candidate patch
def test_rarity_step(patch_data=None, runs=50, level=8):
    patch_path = Path("tools/gemini/out/round-3/scratch/temp_patch.json")
    if patch_data:
        with open(patch_path, "w", encoding="utf-8") as f:
            json.dump(patch_data, f, indent=2)
    
    cmd = [
        "py", "tools/balance/balance.py", "rarity-step",
        "--loadouts", "tools/gemini/out/round-3/loadouts",
        "--items", "tools/gemini/out/round-3/items-base.json",
        "--battle-config", "tools/gemini/out/round-3/config.json",
        "--tier", "3",
        "--tp", "1",
        "--level", str(level),
        "--runs", str(runs)
    ]
    # Note: rarity-step in balance.py does not take --patch directly,
    # but we can apply patch to a temporary items.json and pass --items temp_items.json!
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, env=env)
    print(res.stdout[-1500:])

if __name__ == "__main__":
    test_rarity_step()
