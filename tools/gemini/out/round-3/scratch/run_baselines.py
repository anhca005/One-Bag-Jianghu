import subprocess
import os
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R3 = ROOT / "tools" / "gemini" / "out" / "round-3"
TABLES = R3 / "tables"
TABLES.mkdir(parents=True, exist_ok=True)

ITEMS = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"
LOADOUTS = R3 / "loadouts"

env = os.environ.copy()
env["PYTHONUTF8"] = "1"

def run_cmd(cmd, outfile):
    print(f"Running: {' '.join(cmd)}")
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    with open(outfile, "w", encoding="utf-8") as f:
        f.write(res.stdout)
    print(f"Saved to {outfile}")
    if res.stderr:
        print(f"Stderr: {res.stderr[:300]}")

# 1. Matrix A
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS),
    "--sects", str(SECTS),
    "--battle-config", str(CONFIG),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "100",
    "--level", "10"
], TABLES / "baseline_matrix_a.txt")

# 2. Matrix B Common
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS),
    "--sects", str(SECTS),
    "--battle-config", str(CONFIG),
    "--tiers", "1,9",
    "--tp", "1",
    "--rarity", "common",
    "--runs", "100",
    "--level", "10"
], TABLES / "baseline_matrix_b_common.txt")

# 3. Matrix B Legendary
run_cmd([
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS),
    "--sects", str(SECTS),
    "--battle-config", str(CONFIG),
    "--tiers", "1,9",
    "--tp", "1",
    "--rarity", "legendary",
    "--runs", "100",
    "--level", "10"
], TABLES / "baseline_matrix_b_legendary.txt")

# 4. Rarity Step
run_cmd([
    "py", "tools/balance/balance.py", "rarity-step",
    "--loadouts", str(LOADOUTS),
    "--items", str(ITEMS),
    "--sects", str(SECTS),
    "--battle-config", str(CONFIG),
    "--tier", "3",
    "--tp", "1",
    "--level", "8",
    "--runs", "100"
], TABLES / "baseline_rarity_step.txt")

print("Baseline runs complete!")
