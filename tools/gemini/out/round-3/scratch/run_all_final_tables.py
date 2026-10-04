import json
import subprocess
import os
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

loadouts_dir = Path("tools/gemini/out/round-3/loadouts").resolve()
items_final = Path("tools/gemini/out/round-3/items-v3-final.json").resolve()
config_file = Path("tools/gemini/out/round-3/config.json").resolve()
out_dir = Path("tools/gemini/out/round-3/scratch").resolve()

env = os.environ.copy()
env["PYTHONUTF8"] = "1"

print("1. Running Matrix A (--tiers 1,3,6,9 --tp 1,9 --runs 100 --level 10)...")
cmd_a = [
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(loadouts_dir),
    "--items", str(items_final),
    "--battle-config", str(config_file),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "100",
    "--level", "10"
]
res_a = subprocess.run(cmd_a, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
with open(out_dir / "matrix_a_verbatim.txt", "w", encoding="utf-8") as f:
    f.write(res_a.stdout)
print("Saved matrix_a_verbatim.txt")

print("2. Running Matrix B Common (--tiers 1,9 --tp 1 --runs 100 --rarity common)...")
cmd_b_c = [
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(loadouts_dir),
    "--items", str(items_final),
    "--battle-config", str(config_file),
    "--tiers", "1,9",
    "--tp", "1",
    "--runs", "100",
    "--level", "10",
    "--rarity", "common"
]
res_b_c = subprocess.run(cmd_b_c, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
with open(out_dir / "matrix_b_common_verbatim.txt", "w", encoding="utf-8") as f:
    f.write(res_b_c.stdout)
print("Saved matrix_b_common_verbatim.txt")

print("3. Running Matrix B Legendary (--tiers 1,9 --tp 1 --runs 100 --rarity legendary)...")
cmd_b_l = [
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(loadouts_dir),
    "--items", str(items_final),
    "--battle-config", str(config_file),
    "--tiers", "1,9",
    "--tp", "1",
    "--runs", "100",
    "--level", "10",
    "--rarity", "legendary"
]
res_b_l = subprocess.run(cmd_b_l, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
with open(out_dir / "matrix_b_legendary_verbatim.txt", "w", encoding="utf-8") as f:
    f.write(res_b_l.stdout)
print("Saved matrix_b_legendary_verbatim.txt")

print("4. Running Rarity-Step (--tier 3 --tp 1 --level 8 --runs 100)...")
cmd_r = [
    "py", "tools/balance/balance.py", "rarity-step",
    "--loadouts", str(loadouts_dir),
    "--items", str(items_final),
    "--battle-config", str(config_file),
    "--tier", "3",
    "--tp", "1",
    "--level", "8",
    "--runs", "100"
]
res_r = subprocess.run(cmd_r, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
with open(out_dir / "rarity_step_verbatim.txt", "w", encoding="utf-8") as f:
    f.write(res_r.stdout)
print("Saved rarity_step_verbatim.txt")

print("All runs completed successfully!")
