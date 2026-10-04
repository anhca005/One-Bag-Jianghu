import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
NORM_ITEMS_PATH = ROOT / "tools" / "gemini" / "out" / "items-v2" / "items-v2.normalized.json"
PATCH_1_PATH = ROOT / "tools" / "gemini" / "out" / "items-v2-balance-1" / "patch.json"
MONSTER_ITEMS_PATH = ROOT / "tools" / "gemini" / "out" / "pve" / "monster-items.json"
OUT_BASE_PATH = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"

sys.path.insert(0, str(ROOT / "tools" / "balance"))
from balance import parameter

with open(NORM_ITEMS_PATH, "r", encoding="utf-8") as f:
    items = json.load(f)

with open(PATCH_1_PATH, "r", encoding="utf-8") as f:
    patch = json.load(f)

with open(ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json", "r", encoding="utf-8") as f:
    sects = json.load(f)

# Apply round-1 patch without config: keys
applied_count = 0
for addr, val in patch.items():
    if addr.startswith("config:"):
        print(f"Skipping {addr}")
        continue
    parameter(items, sects, {}, addr, val, set_value=True)
    applied_count += 1
print(f"Applied {applied_count} patch entries to normalized items.")

# Load monster items and fix
with open(MONSTER_ITEMS_PATH, "r", encoding="utf-8") as f:
    monster_items = json.load(f)

for m in monster_items:
    if m["id"] == "kich_doc_chu_nga":
        print(f"Fixing kich_doc_chu_nga weaponClass from {m.get('weaponClass')} to claw")
        m["weaponClass"] = "claw"
    elif m["id"] == "hoa_ngiem_xa_cot":
        print("Renaming hoa_ngiem_xa_cot -> hoa_diem_xa_cot")
        m["id"] = "hoa_diem_xa_cot"
        m["name"] = "Hỏa Diệm Xà Cốt"

# Check uniqueness of IDs
item_ids = {it["id"] for it in items}
for m in monster_items:
    if m["id"] in item_ids:
        raise ValueError(f"Duplicate item ID: {m['id']}")
    item_ids.add(m["id"])

combined = items + monster_items
print(f"Total items in items-base.json: {len(combined)} (61 standard + {len(monster_items)} monster)")

with open(OUT_BASE_PATH, "w", encoding="utf-8") as f:
    json.dump(combined, f, ensure_ascii=False, indent=2)
    f.write("\n")

print(f"Wrote {OUT_BASE_PATH}")
