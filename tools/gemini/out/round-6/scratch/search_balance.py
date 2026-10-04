import json
import copy
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

def check_layout(items_list):
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

def evaluate_matrix(sects_path=None, runs=40):
    cmd = [
        "py", "tools/balance/balance.py", "matrix",
        "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", str(runs), "--level", "10",
        "--loadouts", str(LOADOUTS_DIR)
    ]
    if sects_path:
        cmd += ["--sects", str(sects_path)]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
    outs = -1
    rows = []
    for line in res.stdout.splitlines():
        if "Outside [" in line:
            outs = int(line.split(":")[1].split("/")[0].strip())
        elif line and not line.startswith("Loadout") and not line.startswith("-") and not line.startswith("Outside") and "%" in line:
            parts = line.split()
            name = " ".join([p for p in parts if not p.endswith("%") and not p.endswith("%!")])
            vals = [p for p in parts if p.endswith("%") or p.endswith("%!")]
            rows.append((name, vals))
    return outs, rows, res.stdout

def save_loadout(fname, data):
    ok, msg = check_layout(data["items"])
    if not ok:
        raise ValueError(f"{fname}: {msg}")
    with open(LOADOUTS_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

if __name__ == "__main__":
    outs, rows, full = evaluate_matrix(runs=40)
    print(f"Current Matrix A outs: {outs}/192")
    for name, vals in rows:
        pcts = [float(v.rstrip("%!")) for v in vals]
        avg = sum(pcts) / len(pcts)
        outs_in_row = sum(1 for p in pcts if p < 35 or p > 65)
        print(f"{name:38s} Avg: {avg:5.1f}% | Outs: {outs_in_row}/8 | Min: {min(pcts):5.1f}% Max: {max(pcts):5.1f}%")
