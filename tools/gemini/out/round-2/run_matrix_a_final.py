import subprocess
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"
CATALOG = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"

print("Running final Matrix A (100 runs)...")
cmd = [
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(CATALOG),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "100",
    "--low", "35", "--high", "65"
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
out_path = ROOT / "tools" / "gemini" / "out" / "round-2" / "matrix_a_final.txt"
with open(out_path, "w", encoding="utf-8") as f:
    f.write(res.stdout)
print("Finished final Matrix A.")
