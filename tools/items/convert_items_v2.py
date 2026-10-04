#!/usr/bin/env python3
"""Convert Gemini's Items v2 draft into the strict battle-engine JSON schema."""

from __future__ import annotations

import copy
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "tools/gemini/out/items-v2/items-v2.json"
OUTPUT = ROOT / "tools/gemini/out/items-v2/items-v2.normalized.json"

TRIGGERS = {
    "damaged": "damaged", "healed": "healed", "controlled": "controlled",
    "crit_taken": "crit_taken", "enemy_ultimate": "enemy_ultimate",
    "qi_empty": "qi_empty", "shield_break_success": "shield_broken",
    "burst_taken": "burst_taken",
}

ITEM_KEYS = {"id", "name", "type", "weaponClass", "rarity", "size", "attack", "cooldown", "crit", "basic", "active", "ultimate", "triggers", "unlocks"}
UNLOCK_KEYS = {"tier", "attack", "cooldown", "crit", "ultimateCost", "basic", "active", "ultimate", "triggers"}
TRIGGER_KEYS = {"on", "from", "hpBelow", "status", "maxUses", "effects"}
EFFECT_KEYS = {"op", "target", "pct", "hits", "value", "status", "stacks", "duration", "ignoreShield", "stat", "maxStacks", "minQiAfter"}
ULTIMATE_KEYS = {"cost", "effects"}


def fail(item_id: str, where: str, fields: set[str]) -> None:
    if fields:
        raise ValueError(f"{item_id}: unsupported field(s) at {where}: {', '.join(sorted(fields))}")


def normalize_effect(effect: dict, item_id: str, where: str, changes: list[str]) -> dict:
    effect = copy.deepcopy(effect)
    condition = effect.pop("condition", None)
    if condition is not None:
        if item_id == "thuan_duong_cong" and effect.get("op") == "add_attack":
            # The draft attached Hậu Kình to a passive trigger and also requested an
            # unsupported timed percentage attack buff. Decision #32 permits the gate
            # only in an ultimate effect, so use the engine's explicit flat permanent buff.
            effect["value"] = 25
            effect.pop("pct", None)
            effect.pop("duration", None)
            changes.append(f"{where}: unsupported timed +25% trigger buff -> add_attack value 25; dropped invalid {condition}")
            condition = None
    if condition is not None:
        match = re.fullmatch(r"qi_remaining_ge_(\d+)", condition)
        if not match:
            raise ValueError(f"{item_id}: unsupported condition at {where}: {condition!r}")
        effect["minQiAfter"] = int(match.group(1))
        changes.append(f"{where}: condition {condition} -> minQiAfter {match.group(1)}")
    if effect.get("op") == "retaliate":
        if item_id != "phuc_ma_giap":
            raise ValueError(f"{item_id}: rejected op retaliate at {where}")
        effect = {"op": "damage", "value": 12}
        changes.append(f"{where}: retaliate -> damage value 12")
    if effect.get("op") == "buff_stack" and effect.get("stat") in ("crit", "dmg_reduce") and "value" in effect:
        # The engine reads fractional crit/dmg_reduce stacks from pct; an int value field would
        # truncate 0.02 to 0 and leave pct at its 1.0 default (100% per stack).
        effect["pct"] = effect.pop("value")
        changes.append(f"{where}: buff_stack {effect['stat']} value -> pct {effect['pct']}")
    fail(item_id, where, set(effect) - EFFECT_KEYS)
    return effect


def normalize_trigger(trigger: dict, item_id: str, where: str, changes: list[str]) -> dict:
    trigger = copy.deepcopy(trigger)
    old = trigger.get("on")
    if old == "status_threshold_reached":
        if item_id != "giai_doc_dan":
            raise ValueError(f"{item_id}: rejected trigger status_threshold_reached at {where}")
        trigger["on"] = "burst_taken"
        changes.append(f"{where}: status_threshold_reached -> burst_taken")
    elif old in TRIGGERS:
        trigger["on"] = TRIGGERS[old]
        if old != trigger["on"]:
            changes.append(f"{where}: {old} -> {trigger['on']}")
    trigger["effects"] = [normalize_effect(e, item_id, f"{where}.effects[{i}]", changes) for i, e in enumerate(trigger.get("effects", []))]
    fail(item_id, where, set(trigger) - TRIGGER_KEYS)
    return trigger


def normalize_item(raw: dict) -> tuple[dict, list[str]]:
    item = copy.deepcopy(raw)
    item_id = item.get("id", "<missing id>")
    changes: list[str] = []
    if "_needs" in item:
        item.pop("_needs")
        changes.append("dropped _needs")
    for field in ("basic", "active"):
        item[field] = [normalize_effect(e, item_id, f"{field}[{i}]", changes) for i, e in enumerate(item.get(field, []))]
        if not item[field] and field not in raw:
            item.pop(field)
    if item.get("ultimate") is not None:
        fail(item_id, "ultimate", set(item["ultimate"]) - ULTIMATE_KEYS)
        item["ultimate"]["effects"] = [normalize_effect(e, item_id, f"ultimate.effects[{i}]", changes) for i, e in enumerate(item["ultimate"].get("effects", []))]
    item["triggers"] = [normalize_trigger(t, item_id, f"triggers[{i}]", changes) for i, t in enumerate(item.get("triggers", []))]
    for ui, unlock in enumerate(item.get("unlocks", [])):
        fail(item_id, f"unlocks[{ui}]", set(unlock) - UNLOCK_KEYS)
        for field in ("basic", "active", "ultimate"):
            unlock[field] = [normalize_effect(e, item_id, f"unlocks[{ui}].{field}[{i}]", changes) for i, e in enumerate(unlock.get(field, []))]
            if not unlock[field] and field not in raw.get("unlocks", [])[ui]:
                unlock.pop(field)
        unlock["triggers"] = [normalize_trigger(t, item_id, f"unlocks[{ui}].triggers[{i}]", changes) for i, t in enumerate(unlock.get("triggers", []))]
    fail(item_id, "item", set(item) - ITEM_KEYS)
    return item, changes


def main() -> None:
    catalog = json.loads(SOURCE.read_text(encoding="utf-8"))
    normalized = []
    for raw in catalog:
        item, changes = normalize_item(raw)
        normalized.append(item)
        print(f"{item['id']}:")
        for change in changes:
            print(f"  - {change}")
        if not changes:
            print("  - no changes")
    OUTPUT.write_text(json.dumps(normalized, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {len(normalized)} items to {OUTPUT}")


if __name__ == "__main__":
    main()
