import json
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
with open(ROOT / "tools/gemini/out/round-2/items-v2-final.json", encoding="utf-8") as f:
    items = json.load(f)

# Find all ops used in basic, active, ultimate, triggers, unlocks
effect_ops = set()
for it in items:
    for e in it.get("basic", []):
        effect_ops.add((e.get("op"), "basic", tuple(k for k in e.keys() if k != "op")))
    for e in it.get("active", []):
        effect_ops.add((e.get("op"), "active", tuple(k for k in e.keys() if k != "op")))
    if it.get("ultimate") and "effects" in it["ultimate"]:
        for e in it["ultimate"]["effects"]:
            effect_ops.add((e.get("op"), "ultimate", tuple(k for k in e.keys() if k != "op")))
    for tr in it.get("triggers", []):
        for e in tr.get("effects", []):
            effect_ops.add((e.get("op"), "trigger", tuple(k for k in e.keys() if k != "op")))
    for un in it.get("unlocks", []):
        for e in un.get("basic", []):
            effect_ops.add((e.get("op"), "unlock_basic", tuple(k for k in e.keys() if k != "op")))
        for e in un.get("active", []):
            effect_ops.add((e.get("op"), "unlock_active", tuple(k for k in e.keys() if k != "op")))
        for e in un.get("ultimate", []):
            effect_ops.add((e.get("op"), "unlock_ult", tuple(k for k in e.keys() if k != "op")))
        for tr in un.get("triggers", []):
            for e in tr.get("effects", []):
                effect_ops.add((e.get("op"), "unlock_trigger", tuple(k for k in e.keys() if k != "op")))

print("All effect ops and their fields:")
for op, loc, keys in sorted(effect_ops):
    print(f"  {op:<16} in {loc:<15} keys={keys}")
