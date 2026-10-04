import json, copy
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
SRC_ITEMS = ROOT / "tools/gemini/out/round-2/items-v2-final.json"
DST_ITEMS = ROOT / "tools/gemini/out/round-3/items-base.json"

with open(SRC_ITEMS, "r", encoding="utf-8") as f:
    items = json.load(f)

def scale_val(val, factor=0.25, min_val=1):
    return max(min_val, int(round(val * factor)))

def rescale_effects(effects):
    for e in effects:
        op = e.get("op")
        if op in ("shield", "heal", "raise_max_hp", "shield_break") and "value" in e:
            e["value"] = scale_val(e["value"], 0.25, 2)
        elif op == "damage" and "value" in e:
            e["value"] = scale_val(e["value"], 0.25, 1)
        elif op == "add_attack" and "value" in e:
            e["value"] = scale_val(e["value"], 0.25, 1)
        elif op == "buff_stack" and e.get("stat") == "attack" and "value" in e:
            e["value"] = scale_val(e["value"], 0.25, 1)

rescaled_items = []
for orig in items:
    it = copy.deepcopy(orig)
    itype = it.get("type")
    w, h = it.get("size", [1, 1])
    cells = w * h

    # 1. Add stats for non-weapons
    stats = {}
    if itype == "armor":
        stats["hp"] = 35 * cells
    elif itype == "relic":
        stats["hp"] = 20 * cells
    elif itype == "manual":
        stats["cooldownReduction"] = round(0.05 * cells, 2)
    elif itype == "pill":
        stats["dmgReduce"] = round(0.03 * cells, 2)
    
    if stats:
        it["stats"] = stats

    # 2. Rescale weapon attack (factor 0.30 to ensure fights end in 15-35s)
    if "attack" in it and it["attack"] is not None:
        it["attack"] = scale_val(it["attack"], 0.30, 2)

    # 3. Rescale effects in basic, active, ultimate, triggers
    rescale_effects(it.get("basic", []))
    rescale_effects(it.get("active", []))
    if it.get("ultimate") and "effects" in it["ultimate"]:
        rescale_effects(it["ultimate"]["effects"])
    for tr in it.get("triggers", []):
        rescale_effects(tr.get("effects", []))

    # 4. Rescale unlocks
    for un in it.get("unlocks", []):
        if "attack" in un and un["attack"] is not None:
            un["attack"] = scale_val(un["attack"], 0.25, 1)
        rescale_effects(un.get("basic", []))
        rescale_effects(un.get("active", []))
        rescale_effects(un.get("ultimate", []))
        for tr in un.get("triggers", []):
            rescale_effects(tr.get("effects", []))

    rescaled_items.append(it)

with open(DST_ITEMS, "w", encoding="utf-8") as f:
    json.dump(rescaled_items, f, indent=2, ensure_ascii=False)

print(f"Saved {len(rescaled_items)} rescaled items to {DST_ITEMS}")
