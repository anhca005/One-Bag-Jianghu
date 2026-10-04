import json, subprocess, shutil, sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R3 = ROOT / "tools" / "gemini" / "out" / "round-3"
temp_a = R3 / "balance" / "temp_a"
temp_b = R3 / "balance" / "temp_b"
fname = "stage_01.json"
shutil.copy(R3 / "balance" / "player" / fname, temp_a / fname)
shutil.copy(R3 / "balance" / "enemy" / fname, temp_b / fname)

cmd = ["py", "tools/balance/balance.py", "vs", "--a", str(temp_a), "--b", str(temp_b), "--runs", "1", "--items", str(R3 / "items-v3-final.json"), "--sects", str(ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"), "--battle-config", str(R3 / "config.json")]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
print(res.stdout)
(temp_a / fname).unlink()
(temp_b / fname).unlink()
