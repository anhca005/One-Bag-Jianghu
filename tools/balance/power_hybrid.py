#!/usr/bin/env python3
"""Build the Luc Chien catalog the game uses (decision #55).

v2 fitted every item's offense and defense on win rates, but its weapon offense saturated: 16 weapons came out
identical, and Song Dao carried a hand-set 140. v3 measured weapon DPS straight from the engine, which ranks
weapons correctly but predicts whole fights worse than v2.

This script keeps v2 and replaces only the weapon offense values:
    offense = median v2 weapon offense x sqrt(v3 weapon DPS / median v3 weapon DPS)
The square root keeps v2's overall weapon weight, which v2's fit chose, and spreads weapons by measured DPS.
The v2 Song Dao override is excluded from the v2 median. No per-item number is set by hand.
`scale` is refit so the starter kit at level 1 = 100.

Usage:
    py tools/balance/power_hybrid.py [--out tools/codex/out/power-hybrid/power.json]
"""
from __future__ import annotations

import argparse
import copy
import math
import statistics
from pathlib import Path
from typing import Any

import power

V2_OVERRIDE = "tu_mau_song_dao"
STARTER = {
    "level": 1,
    "items": [
        {"id": "thanh_phong_kiem", "tier": 1, "rarity": "common"},
        {"id": "o_tam_y", "tier": 1, "rarity": "common"},
    ],
}


def weapon_ids() -> list[str]:
    catalog = power.read(power.ITEMS)
    items = catalog["items"] if isinstance(catalog, dict) else catalog
    return [item["id"] for item in items if item.get("type") == "weapon"]


def build(v2: dict[str, Any], v3: dict[str, Any]) -> dict[str, Any]:
    weapons = [item_id for item_id in weapon_ids() if item_id in v2["items"] and item_id in v3["items"]]
    v2_offense = [v2["items"][item_id]["offense"] for item_id in weapons if item_id != V2_OVERRIDE]
    v3_offense = [v3["items"][item_id]["offense"] for item_id in weapons]
    v2_median = statistics.median(v2_offense)
    v3_median = statistics.median(v3_offense)

    hybrid = copy.deepcopy(v2)
    for item_id in weapons:
        ratio = v3["items"][item_id]["offense"] / v3_median
        hybrid["items"][item_id]["offense"] = round(v2_median * math.sqrt(ratio), 4)

    # Rescale so the starter kit lands on exactly 100. A huge probe scale keeps lc()'s rounding negligible.
    probe_scale = 1_000_000.0
    hybrid["scale"] = probe_scale
    hybrid["scale"] = round(100.0 * probe_scale / power.lc(STARTER, hybrid), 8)
    return hybrid


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--out", type=Path, default=power.ROOT / "tools/codex/out/power-hybrid/power.json")
    args = parser.parse_args()
    hybrid = build(power.read(power.V2), power.read(power.POWER))
    power.write(args.out, hybrid)
    print("starter LC:", power.lc(STARTER, hybrid), "scale:", hybrid["scale"], "->", args.out)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
