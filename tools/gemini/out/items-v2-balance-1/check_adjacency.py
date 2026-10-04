import json
import sys

sys.stdout.reconfigure(encoding="utf-8")
items = json.load(open("tools/gemini/out/items-v2/items-v2.normalized.json", encoding="utf-8"))

print("=== ITEMS WITH ADJACENCY OR SPECIAL REQUIREMENTS ===")
for it in items:
    raw_str = json.dumps(it, ensure_ascii=False)
    if "adjacent" in raw_str:
        print(f"[{it['id']:25}] size={it['size']} type={it['type']:7} name={it['name']}")

