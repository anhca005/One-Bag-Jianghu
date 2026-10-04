import json
from collections import Counter
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
with open(ROOT / "tools/gemini/out/round-2/items-v2-final.json", encoding="utf-8") as f:
    items = json.load(f)

print(f"Total items: {len(items)}")
types = Counter(it.get("type") for it in items)
print("Item types:", types)

non_weapons = [it for it in items if it.get("type") != "weapon"]
print(f"\nTotal non-weapons: {len(non_weapons)}")
for it in non_weapons:
    cells = it["size"][0] * it["size"][1]
    print(f"{it['id']:<26} type={it.get('type'):<8} size={str(it['size']):<8} cells={cells} rarity={it.get('rarity')}")
