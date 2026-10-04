import json

with open("tools/gemini/out/round-3/items-base.json", encoding="utf-8") as f:
    items = json.load(f)

for it in items:
    wc = str(it.get("weaponClass", ""))
    atk = str(it.get("attack", ""))
    print(f"{it['id']:28} {it['type']:8} {wc:8} {atk:5} {it['name']}")
