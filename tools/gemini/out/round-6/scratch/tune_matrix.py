import json
import shutil
import subprocess
import os
import sys
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R6 = ROOT / "tools" / "gemini" / "out" / "round-6"
LOADOUTS_DIR = R6 / "loadouts"

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()
env = os.environ.copy()
env["PYTHONUTF8"] = "1"

ITEMS = json.load(open(ROOT / "server/battle-engine/src/main/resources/data/items.json", encoding="utf-8"))
ITEM_MAP = {it["id"]: it for it in ITEMS}

def check_overlap(items_list):
    grid = [[None for _ in range(4)] for _ in range(5)]
    for it in items_list:
        iid = it["id"]
        x, y = it["x"], it["y"]
        meta = ITEM_MAP.get(iid)
        if not meta:
            return False, f"Unknown item {iid}"
        w, h = meta["size"]
        if x < 0 or y < 0 or x + w > 5 or y + h > 4:
            return False, f"Out of bounds: {iid} at ({x},{y}) size ({w},{h})"
        for dx in range(w):
            for dy in range(h):
                if grid[x + dx][y + dy] is not None:
                    return False, f"Overlap at ({x+dx},{y+dy}) between {grid[x+dx][y+dy]} and {iid}"
                grid[x + dx][y + dy] = iid
    return True, "OK"

def evaluate_fast(runs=30):
    cmd = [
        "py", "tools/balance/balance.py", "matrix",
        "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", str(runs), "--level", "10",
        "--loadouts", str(LOADOUTS_DIR)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
    outs = -1
    for line in res.stdout.splitlines():
        if "Outside [" in line:
            outs = int(line.split(":")[1].split("/")[0].strip())
    return outs

def save_loadout(filename, data):
    ok, msg = check_overlap(data["items"])
    if not ok:
        raise ValueError(f"Invalid layout in {filename}: {msg}")
    path = LOADOUTS_DIR / filename
    with open(path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

if __name__ == "__main__":
    print("Baseline before changes:")
    outs = evaluate_fast(30)
    print(f"Matrix A outs: {outs}/192")
