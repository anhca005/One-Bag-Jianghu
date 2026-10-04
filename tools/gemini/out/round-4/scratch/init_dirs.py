import os
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
for sub in ["tables", "loadouts", "balance", "scratch", "balance/player", "balance/enemy"]:
    (R4 / sub).mkdir(parents=True, exist_ok=True)
print("Round-4 directories initialized!")
