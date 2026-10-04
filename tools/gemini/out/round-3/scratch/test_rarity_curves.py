import json
import subprocess
import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path("tools/balance").resolve()))
import balance

# Let's test different rarity multiplier curves on rarity-step
curves = [
    ("Default #35 (1.0, 1.3, 1.7, 2.2, 2.8, 3.5)", [1000, 1300, 1700, 2200, 2800, 3500]),
    ("Step ~15% (1.0, 1.15, 1.32, 1.52, 1.75, 2.0)", [1000, 1150, 1320, 1520, 1750, 2000]),
    ("Step ~10% (1.0, 1.10, 1.21, 1.33, 1.46, 1.61)", [1000, 1100, 1210, 1330, 1460, 1610]),
    ("Step ~8% (1.0, 1.08, 1.17, 1.26, 1.36, 1.47)", [1000, 1080, 1166, 1260, 1360, 1469]),
    ("Step ~6% #30 (1.0, 1.06, 1.12, 1.19, 1.26, 1.34)", [1000, 1060, 1120, 1190, 1260, 1340]),
]

with open("tools/gemini/out/round-3/config.json", encoding="utf-8") as f:
    base_cfg = json.load(f)

for label, mults in curves:
    cfg = dict(base_cfg)
    cfg["rarityMultiplierPermille"] = mults
    temp_cfg = Path("tools/gemini/out/round-3/scratch/temp_rarity_cfg.json")
    with open(temp_cfg, "w", encoding="utf-8") as f:
        json.dump(cfg, f, indent=2)
    
    cmd = [
        "py", "tools/balance/balance.py", "rarity-step",
        "--loadouts", "tools/gemini/out/round-3/loadouts",
        "--items", "tools/gemini/out/round-3/items-base.json",
        "--battle-config", str(temp_cfg),
        "--tier", "3",
        "--tp", "1",
        "--level", "8",
        "--runs", "50"
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    for line in res.stdout.splitlines():
        if "Average" in line:
            print(f"{label[:40]:<40} -> {line}")
            break
