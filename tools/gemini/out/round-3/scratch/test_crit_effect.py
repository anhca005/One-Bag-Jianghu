import json
import subprocess
import os
from pathlib import Path

# Load items-base.json
with open("tools/gemini/out/round-3/items-base.json", encoding="utf-8") as f:
    items = json.load(f)

for crit_val in [0.05, 0.15, 0.25, 0.35]:
    # Modify all weapons' crit
    test_items = []
    for it in items:
        it_copy = dict(it)
        if it_copy.get("type") == "weapon":
            it_copy["crit"] = crit_val
        test_items.append(it_copy)
    
    temp_path = Path(f"tools/gemini/out/round-3/scratch/items_crit_{int(crit_val*100)}.json")
    with open(temp_path, "w", encoding="utf-8") as f:
        json.dump(test_items, f, indent=2)
    
    cmd = [
        "py", "tools/balance/balance.py", "rarity-step",
        "--loadouts", "tools/gemini/out/round-3/loadouts",
        "--items", str(temp_path),
        "--battle-config", "tools/gemini/out/round-3/config.json",
        "--tier", "3",
        "--tp", "1",
        "--level", "8",
        "--runs", "20"
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    for line in res.stdout.splitlines():
        if "Average" in line:
            print(f"Crit {crit_val:.2f} -> {line}")
            break
