import json
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open('One-Bag-Jianghu/Assets/_Game/Resources/Data/items.json', 'r', encoding='utf-8') as f:
    items = json.load(f)

long_items = []
for idx, it in enumerate(items):
    name = it.get('name', '')
    if len(name) > 18:
        long_items.append((idx, it.get('id'), name, len(name)))

for idx, i_id, name, l in long_items:
    print(f"Index {idx} ({i_id}): '{name}' ({l} chars)")
