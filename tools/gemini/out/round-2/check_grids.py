import json
from pathlib import Path

LOADOUTS = Path("tools/gemini/out/round-2/loadouts")
CATALOG = Path("tools/gemini/out/round-2/items-base.json")

with open(CATALOG, "r", encoding="utf-8") as f:
    cat = {it["id"]: it for it in json.load(f)}

for lp in sorted(LOADOUTS.glob("*.json")):
    with open(lp, "r", encoding="utf-8") as f:
        lb = json.load(f)
    cols = lb.get("cols", 5)
    rows = lb.get("rows", 4)
    grid = {}
    errors = []
    for it in lb["items"]:
        iid = it["id"]
        if iid not in cat:
            errors.append(f"Unknown item {iid}")
            continue
        w, h = cat[iid]["size"]
        x, y = it["x"], it["y"]
        for dx in range(w):
            for dy in range(h):
                pos = (x + dx, y + dy)
                if pos[0] >= cols or pos[1] >= rows or pos[0] < 0 or pos[1] < 0:
                    errors.append(f"OOB {iid} at {pos}")
                elif pos in grid:
                    errors.append(f"Overlap at {pos} between {grid[pos]} and {iid}")
                else:
                    grid[pos] = iid
    if errors:
        print(f"FAILED {lp.name}: {errors}")
    else:
        print(f"OK {lp.name} (occupied {len(grid)}/20)")
