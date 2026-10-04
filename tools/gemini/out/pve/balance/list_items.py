import json
import sys
sys.stdout.reconfigure(encoding="utf-8")

with open("tools/gemini/out/pve/combined_items.json", encoding="utf-8") as f:
    items = json.load(f)

for it in items:
    print(f"{it['id']}: type={it.get('type')}, size={it.get('size')}, name={it.get('name')}")
