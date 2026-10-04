import subprocess
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"
CATALOG = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"

print("Running Matrix B (Common)...")
cmd_common = [
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(CATALOG),
    "--tiers", "1,9",
    "--tp", "1",
    "--rarity", "common",
    "--runs", "100"
]
res_c = subprocess.run(cmd_common, capture_output=True, text=True, encoding="utf-8", errors="replace")
with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "matrix_b_common.txt", "w", encoding="utf-8") as f:
    f.write(res_c.stdout)

print("Running Matrix B (Legendary)...")
cmd_leg = [
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(CATALOG),
    "--tiers", "1,9",
    "--tp", "1",
    "--rarity", "legendary",
    "--runs", "100"
]
res_l = subprocess.run(cmd_leg, capture_output=True, text=True, encoding="utf-8", errors="replace")
with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "matrix_b_legendary.txt", "w", encoding="utf-8") as f:
    f.write(res_l.stdout)

print("Matrix B runs completed.")
