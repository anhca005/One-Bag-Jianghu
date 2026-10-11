#!/usr/bin/env python3
import json
from pathlib import Path

p = Path("tools/gemini/out/power-per-cell-v2/items.json")
items = json.loads(p.read_text(encoding="utf-8"))

invalid = []
for item in items:
    cd = item.get("cooldown")
    atk = item.get("attack")
    if cd is not None and cd <= 0:
        invalid.append((item["id"], "cooldown <= 0", cd))
    if atk is not None and atk < 0:
        invalid.append((item["id"], "attack < 0", atk))

print(f"Total items checked: {len(items)}")
print(f"Invalid items found: {len(invalid)}")
for item_id, reason, val in invalid:
    print(f"  {item_id}: {reason} = {val}")
