import json, subprocess, re, shutil
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
DST_BALANCE = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance"
P_DIR = DST_BALANCE / "player"
E_DIR = DST_BALANCE / "enemy"
ITEMS = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
PATCH = ROOT / "tools" / "gemini" / "out" / "round-2" / "test_patch.json"

temp_a = DST_BALANCE / "temp_a"
temp_b = DST_BALANCE / "temp_b"
temp_a.mkdir(exist_ok=True)
temp_b.mkdir(exist_ok=True)

def run_stage(stage_idx, runs=100):
    fname = f"stage_{stage_idx:02d}.json"
    shutil.copy(P_DIR / fname, temp_a / fname)
    shutil.copy(E_DIR / fname, temp_b / fname)
    
    cmd = [
        "py", "tools/balance/balance.py", "vs",
        "--a", str(temp_a),
        "--b", str(temp_b),
        "--runs", str(runs),
        "--items", str(ITEMS),
        "--sects", str(SECTS)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
    
    # Clean up temp
    (temp_a / fname).unlink()
    (temp_b / fname).unlink()
    
    # Parse win rate
    # Output format has: "P_name vs E_name: X.X% (wins: W, losses: L, draws: D)"
    for line in res.stdout.splitlines():
        if "%" in line:
            return line.strip(), res.stdout
    return f"Failed to parse: {res.stdout}", res.stdout

print("Pve runner script ready.")
