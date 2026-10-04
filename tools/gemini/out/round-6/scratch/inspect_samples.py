import json
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")


ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
SAMPLES = ROOT / "server" / "battle-engine" / "samples"
ITEMS = json.load(open(ROOT / "server/battle-engine/src/main/resources/data/items.json", encoding="utf-8"))
ITEM_MAP = {it["id"]: it for it in ITEMS}

files = sorted(SAMPLES.glob("*.json"))
print(f"Total samples: {len(files)}")
print(f"{'Filename':30s} {'Name':40s} {'Sect':12s} {'Weapons':25s} {'Slots':5s}")
print("-" * 115)

for f in files:
    data = json.load(open(f, encoding="utf-8"))
    name = data.get("name", f.stem)
    sect = data.get("sect", "none")
    weapons = []
    total_cells = 0
    type_counts = {}
    for it in data.get("items", []):
        iid = it["id"]
        meta = ITEM_MAP.get(iid, {})
        itype = meta.get("type", "unknown")
        type_counts[itype] = type_counts.get(itype, 0) + 1
        size = meta.get("size", [1, 1])
        total_cells += size[0] * size[1]
        if itype == "weapon":
            wclass = meta.get("weaponClass", "weapon")
            weapons.append(f"{iid}({wclass})")
    w_str = ", ".join(weapons)
    print(f"{f.name:30s} {name:40s} {str(sect):12s} {w_str:30s} {total_cells:2d}/20")
