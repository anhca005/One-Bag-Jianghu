import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open("server/battle-engine/src/main/resources/data/items.json", encoding="utf-8") as f:
    items = json.load(f)

for it in items:
    if it.get("type") == "weapon":
        print(f"{it['id']:25}: attack={it.get('attack'):2}, cd={it.get('cooldown'):3.1f}, class={it.get('weaponClass')}, rarity={it.get('rarity')}, size={it.get('size')}")
