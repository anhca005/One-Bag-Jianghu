import json, subprocess, shutil
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
DST_BALANCE = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance"
P_DIR = DST_BALANCE / "player"
E_DIR = DST_BALANCE / "enemy"
ITEMS = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

temp_a = DST_BALANCE / "temp_a"
temp_b = DST_BALANCE / "temp_b"
temp_a.mkdir(exist_ok=True)
temp_b.mkdir(exist_ok=True)

def simulate_stage(stage_idx, runs=100):
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
    
    (temp_a / fname).unlink()
    (temp_b / fname).unlink()
    
    # Parse line
    # "P vs E: 85.0% (wins: 85, losses: 15, draws: 0)"
    for line in res.stdout.splitlines():
        if "%" in line and "vs" in line:
            parts = line.split(":")
            # After last ':' is percentage
            pct_part = parts[-1].split("%")[0].strip()
            return float(pct_part), line.strip()
    return 0.0, f"Error: {res.stdout}"

print("Pve simulator helper loaded.")
