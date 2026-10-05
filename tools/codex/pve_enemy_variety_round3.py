#!/usr/bin/env python3
"""Finish Round 3 by tuning only the approved out-of-band stages."""

from __future__ import annotations

import argparse
import copy
import sys
from typing import Any

import pve_enemy_variety_round2 as round2


VARIANTS = {
    34: [["xich_sa_chuong"]],
    36: [["chan_nhac_chuy"], ["liet_diem_dao"]],
    44: [["thanh_phong_kiem"], ["thanh_phong_kiem", "thanh_phong_kiem"]],
    50: [["chan_nhac_chuy", "chan_nhac_chuy"], ["tu_mau_song_dao", "tu_mau_song_dao"]],
}


def replace_weapons(
    tuner: round2.Tuner,
    enemy: dict[str, Any],
    weapon_ids: list[str],
) -> dict[str, Any]:
    """Replace all weapons, retaining support items that still fit."""
    result = copy.deepcopy(enemy)
    support = [
        item
        for item in result["items"]
        if tuner.catalog[item["id"]].get("type") != "weapon"
    ]
    template = result["items"][0]
    weapons = [
        {"id": item_id, "tier": template["tier"], "rarity": template["rarity"]}
        for item_id in weapon_ids
    ]
    items = weapons + support
    while True:
        try:
            result["items"] = tuner.pack(items)
            return result
        except ValueError:
            items.pop()


def tune_variant(
    tuner: round2.Tuner,
    number: int,
    stage: dict[str, Any],
    weapon_ids: list[str],
) -> tuple[dict[str, Any], float]:
    """Tune one weapon variant uniformly, then confirm it at 50 runs."""
    player, _starter, _lucky, _level, _silver = tuner.progression.bags_for_stage(number)
    limits = round2.band(stage["kind"])
    enemy = replace_weapons(tuner, stage["enemy"], weapon_ids)
    index, _probe = tuner.search(player, enemy, limits)
    index, confirmed = tuner.confirm_nearby(player, enemy, index, limits)
    return tuner.apply_step(enemy, index), confirmed


def main() -> int:
    """Try thematic alternatives for Round-3 discontinuities."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    args = argparse.Namespace(probe_runs=20, confirm_runs=50, start_stage=2)
    tuner = round2.Tuner(args)
    try:
        number = 0
        for chapter in tuner.document["chapters"]:
            for stage in chapter["stages"]:
                number += 1
                if number not in VARIANTS:
                    continue
                limits = round2.band(stage["kind"])
                best_enemy = copy.deepcopy(stage["enemy"])
                best_rate = 100.0
                for weapon_ids in VARIANTS[number]:
                    enemy, rate = tune_variant(tuner, number, stage, weapon_ids)
                    print(f"Ải {number:02d} {weapon_ids}: {rate:.1f}%", flush=True)
                    if tuner.distance(rate, limits) < tuner.distance(best_rate, limits):
                        best_enemy = enemy
                        best_rate = rate
                    if limits[0] <= rate <= limits[1]:
                        break
                stage["enemy"]["items"] = best_enemy["items"]
            round2.write_json(round2.OUT, tuner.document)
    finally:
        tuner.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
