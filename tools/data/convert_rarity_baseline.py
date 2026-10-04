"""Convert the 21 legacy fixed-rarity items to common baselines and append 40 new items."""
from __future__ import annotations

import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SERVER = ROOT / "server/battle-engine/src/main/resources/data/items.json"
UNITY = ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/items.json"
ALL = ROOT / "tools/gemini/out/rarity/items_all.json"
LEGACY = ROOT / "tools/gemini/out/items.proposed.json"
SAMPLES = [ROOT / "server/battle-engine/samples", ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/Samples"]
MULT = {"common": 1000, "uncommon": 1250, "rare": 1550, "epic": 1900, "legendary": 2300, "supreme": 2750}


def rounded_baseline(value: int, multiplier: int) -> int:
    if value == 0:
        return 0
    limit = ((value + 1) * 1000 + multiplier - 1) // multiplier + 1
    return min(range(1, limit + 1), key=lambda b: (abs(b * multiplier // 1000 - value), -b))


def convert_effect(effect: dict, multiplier: int) -> None:
    if effect.get("op") not in ("gain_qi", "drain_qi") and "value" in effect:
        effect["value"] = rounded_baseline(effect["value"], multiplier)
    if "stacks" in effect:
        effect["stacks"] = rounded_baseline(effect["stacks"], multiplier)


def effect_lists(item: dict):
    yield item.get("basic", [])
    if item.get("ultimate"):
        yield item["ultimate"].get("effects", [])
    for trigger in item.get("triggers", []):
        yield trigger.get("effects", [])
    for unlock in item.get("unlocks", []):
        yield unlock.get("basic", [])
        yield unlock.get("ultimate", [])
        for trigger in unlock.get("triggers", []):
            yield trigger.get("effects", [])


def main() -> None:
    all_items = json.loads(ALL.read_text(encoding="utf-8-sig"))
    # Preserve the original 21 fixed-rarity definitions as the source for deterministic re-runs.
    legacy = json.loads(LEGACY.read_text(encoding="utf-8-sig"))
    old_rarities = {x["id"]: x.get("rarity", "common") for x in legacy}
    rows = []
    for item in legacy:
        rarity = item.get("rarity", "common")
        multiplier = MULT[rarity]
        old_attack = item.get("attack", 0)
        item["attack"] = rounded_baseline(old_attack, multiplier)
        for effects in effect_lists(item):
            for effect in effects:
                convert_effect(effect, multiplier)
        item["rarity"] = "common"
        back = item.get("attack", 0) * multiplier // 1000
        flag = "FLAG" if old_attack and abs(back - old_attack) / old_attack > .05 else "OK"
        rows.append((item["id"], rarity, old_attack, item.get("attack", 0), back, flag))
    new_ids = {x["id"] for x in legacy}
    output = legacy + [x for x in all_items if x["id"] not in new_ids]
    text = json.dumps(output, ensure_ascii=False, indent=2) + "\n"
    SERVER.write_text(text, encoding="utf-8")
    UNITY.write_text(text, encoding="utf-8")
    for directory in SAMPLES:
        if not directory.exists():
            continue
        for path in sorted(directory.rglob("*.json")):
            data = json.loads(path.read_text(encoding="utf-8-sig"))
            changed = False
            for placed in data.get("items", []):
                if "rarity" not in placed and placed.get("id") in old_rarities:
                    placed["rarity"] = old_rarities[placed["id"]]
                    changed = True
            if changed:
                path.write_text(json.dumps(data, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print("id | old rarity | old attack | baseline | back | check")
    for row in rows:
        print(f"{row[0]} | {row[1]} | {row[2]} | {row[3]} | {row[4]} | {row[5]}")


if __name__ == "__main__":
    main()
