import json
import glob
from pathlib import Path

# Load monster item IDs from round-2/chapters.json
with open("tools/gemini/out/round-2/chapters.json", encoding="utf-8") as f:
    ch = json.load(f)
monster_items = set(ch.get("monsterItems", []))
print(f"Monster items count: {len(monster_items)}")
print(f"Monster items: {monster_items}")

# Check which loadouts contain monster items
loadouts = sorted(glob.glob("tools/gemini/out/round-3/loadouts/*.json"))
monster_loadouts = []
regular_loadouts = []

for p in loadouts:
    with open(p, encoding="utf-8") as f:
        data = json.load(f)
    items_in_loadout = [it["id"] for it in data.get("items", [])]
    m_found = [it for it in items_in_loadout if it in monster_items]
    name = data.get("name", Path(p).stem)
    if m_found:
        monster_loadouts.append((Path(p).name, name, m_found))
    else:
        regular_loadouts.append((Path(p).name, name))

print("\nMonster loadouts:")
for fn, name, m in monster_loadouts:
    print(f"  {fn} ({name}): {m}")

print(f"\nTotal loadouts: {len(loadouts)} ({len(regular_loadouts)} regular, {len(monster_loadouts)} monster)")
