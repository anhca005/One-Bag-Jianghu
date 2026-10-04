import json
import sys

sys.stdout.reconfigure(encoding="utf-8")
items = json.load(open("tools/gemini/out/items-v2/items-v2.normalized.json", encoding="utf-8"))
for it in items:
    wc = it.get("weaponClass", "-")
    cd = it.get("cooldown", 0)
    print(f"{it['id']:26} | {it['type']:7} | {wc:7} | {it['size']} | {cd}s | {it['name']}")
