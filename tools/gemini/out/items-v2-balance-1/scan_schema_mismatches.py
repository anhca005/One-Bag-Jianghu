import json
import sys

sys.stdout.reconfigure(encoding="utf-8")
cat = json.load(open("tools/gemini/out/items-v2/items-v2.normalized.json", encoding="utf-8"))

def check_effect(it_id, place, eff):
    op = eff.get("op")
    val = eff.get("value")
    pct = eff.get("pct")
    stat = eff.get("stat")
    if isinstance(val, float) or (val is not None and 0 < val < 1):
        print(f"FLOAT VALUE: {it_id} at {place}: op={op} stat={stat} value={val} pct={pct}")
    if op in ["dmg_reduce", "add_crit", "lifesteal"] and (pct is None or pct == 1.0) and val is not None:
        print(f"MISSING PCT: {it_id} at {place}: op={op} val={val} pct={pct}")
    if op == "buff_stack" and stat in ["crit", "dmg_reduce"] and (pct is None or pct == 1.0) and val is not None:
        print(f"BUFF_STACK FLOAT VAL: {it_id} at {place}: op={op} stat={stat} val={val} pct={pct}")

for it in cat:
    for f in ["basic", "active"]:
        for i, e in enumerate(it.get(f, [])):
            check_effect(it["id"], f"{f}[{i}]", e)
    if it.get("ultimate"):
        for i, e in enumerate(it["ultimate"].get("effects", [])):
            check_effect(it["id"], f"ultimate[{i}]", e)
    for i, t in enumerate(it.get("triggers", [])):
        for j, e in enumerate(t.get("effects", [])):
            check_effect(it["id"], f"triggers[{i}].effects[{j}]", e)
    for u in it.get("unlocks", []):
        tier = u.get("tier")
        for f in ["basic", "active", "ultimate"]:
            for i, e in enumerate(u.get(f, [])):
                check_effect(it["id"], f"unlock_T{tier}.{f}[{i}]", e)
        for i, t in enumerate(u.get("triggers", [])):
            for j, e in enumerate(t.get("effects", [])):
                check_effect(it["id"], f"unlock_T{tier}.triggers[{i}].effects[{j}]", e)
