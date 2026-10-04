import json
import os
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")

ROOT = Path(__file__).resolve().parents[4]
CATALOG_PATH = ROOT / "tools" / "gemini" / "out" / "pve" / "combined_items.json"
PLAYER_DIR = ROOT / "tools" / "gemini" / "out" / "pve" / "balance" / "player_loadouts"
ENEMY_DIR = ROOT / "tools" / "gemini" / "out" / "pve" / "balance" / "enemy_loadouts"

PLAYER_DIR.mkdir(parents=True, exist_ok=True)
ENEMY_DIR.mkdir(parents=True, exist_ok=True)

with open(CATALOG_PATH, encoding="utf-8") as f:
    catalog = {it["id"]: it for it in json.load(f)}

def validate_loadout(name: str, loadout: dict):
    cols = loadout.get("cols", 5)
    rows = loadout.get("rows", 4)
    grid = [[False for _ in range(cols)] for _ in range(rows)]
    for it in loadout["items"]:
        item_id = it["id"]
        if item_id not in catalog:
            raise ValueError(f"[{name}] Unknown item id: {item_id}")
        size = catalog[item_id]["size"]
        x, y = it["x"], it["y"]
        w, h = size[0], size[1]
        if x < 0 or y < 0 or x + w > cols or y + h > rows:
            raise ValueError(f"[{name}] Item {item_id} out of bounds: at ({x},{y}) size ({w},{h}) grid ({cols}x{rows})")
        for r in range(y, y + h):
            for c in range(x, x + w):
                if grid[r][c]:
                    raise ValueError(f"[{name}] Cell overlap at ({c}, {r}) with item {item_id}")
                grid[r][c] = True

print("Loaded catalog and validator ready.")
