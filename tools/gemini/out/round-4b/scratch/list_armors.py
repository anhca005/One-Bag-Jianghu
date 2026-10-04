import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open("server/battle-engine/src/main/resources/data/items.json", encoding="utf-8") as f:
    items = json.load(f)

for it in items:
    if it.get("type") in ("armor", "relic", "pill", "manual"):
        print(f"{it['id']:25}: type={it.get('type'):6}, hp={it.get('stats', {}).get('hp', 0):3}, rarity={it.get('rarity')}, size={it.get('size')}, triggers={[t.get('on') for t in it.get('triggers', [])]}")
