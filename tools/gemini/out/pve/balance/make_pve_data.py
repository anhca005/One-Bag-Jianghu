import json
import os
import subprocess
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")

ROOT = Path(__file__).resolve().parents[5]
CATALOG_PATH = ROOT / "tools" / "gemini" / "out" / "pve" / "combined_items.json"
SECTS_PATH = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
OUT_PVE = ROOT / "tools" / "gemini" / "out" / "pve"
BALANCE_DIR = OUT_PVE / "balance"
P_DIR = BALANCE_DIR / "player_loadouts"
E_DIR = BALANCE_DIR / "enemy_loadouts"

P_DIR.mkdir(parents=True, exist_ok=True)
E_DIR.mkdir(parents=True, exist_ok=True)

with open(CATALOG_PATH, encoding="utf-8") as f:
    catalog = {it["id"]: it for it in json.load(f)}

def validate_bag(name: str, items: list[dict], cols=5, rows=4):
    grid = [[None for _ in range(cols)] for _ in range(rows)]
    for it in items:
        iid = it["id"]
        if iid not in catalog:
            raise ValueError(f"[{name}] Unknown item id: {iid}")
        w, h = catalog[iid]["size"]
        x, y = it["x"], it["y"]
        if x < 0 or y < 0 or x + w > cols or y + h > rows:
            raise ValueError(f"[{name}] Out of bounds: {iid} at ({x},{y}) size ({w},{h}) in {cols}x{rows}")
        for r in range(y, y + h):
            for c in range(x, x + w):
                if grid[r][c] is not None:
                    raise ValueError(f"[{name}] Overlap at ({c},{r}) between {grid[r][c]} and {iid}")
                grid[r][c] = iid

# Helper to validate dialogue line lengths
def validate_dialogue(stage_id: str, dialogue: dict):
    for part in ["before", "afterWin", "afterLoss"]:
        lines = dialogue.get(part, [])
        if len(lines) < 2:
            raise ValueError(f"[{stage_id}] {part} has fewer than 2 lines")
        for line in lines:
            text = line.get("text", "")
            if len(text) > 120:
                raise ValueError(f"[{stage_id}] {part} line exceeds 120 chars: {text} ({len(text)} chars)")

print("Base setup done.")
