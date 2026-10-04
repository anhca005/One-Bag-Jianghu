import json, copy, tempfile, subprocess
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"
ITEMS = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

with open(LOADOUTS / "mb3.json", "r", encoding="utf-8") as f:
    mb3 = json.load(f)

# Let's inspect mb3 items
print("=== MB3 items ===")
for it in mb3["items"]:
    print(f"  {it['id']} at ({it['x']},{it['y']})")

# Let's test different small adjustments to mb3 to avoid the 100% deterministic burst lock
