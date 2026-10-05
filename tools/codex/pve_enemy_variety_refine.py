#!/usr/bin/env python3
"""Refine Round-2 outliers by changing one item at a time."""

from __future__ import annotations

import argparse
import copy
import sys
from typing import Any

import pve_enemy_variety_round2 as round2


ROUND3_STAGES = {9, 16, 18, 20, 23, 27, 31, 34, 36, 41, 42, 44, 50}


def item_step(item: dict[str, Any], direction: int) -> dict[str, Any] | None:
    """Move one item by a small tier or rarity step."""
    changed = copy.deepcopy(item)
    rarity_index = round2.RARITIES.index(item["rarity"])
    if direction > 0:
        if changed["tier"] < 9:
            changed["tier"] += 1
            return changed
        if rarity_index < len(round2.RARITIES) - 1:
            changed["rarity"] = round2.RARITIES[rarity_index + 1]
            return changed
    else:
        if changed["tier"] > 1:
            changed["tier"] -= 1
            return changed
        if rarity_index > 0:
            changed["rarity"] = round2.RARITIES[rarity_index - 1]
            return changed
    return None


def candidates(tuner: round2.Tuner, enemy: dict[str, Any], direction: int) -> list[dict[str, Any]]:
    """Build legal one-coordinate candidates in the desired direction."""
    result = []
    for offset in range(1, min(10, len(enemy["items"]))):
        ordered = enemy["items"][offset:] + enemy["items"][:offset]
        packed: list[dict[str, Any]] = []
        valid = True
        for source in ordered:
            item = {key: source[key] for key in ("id", "tier", "rarity")}
            if not tuner.progression._place(packed, item):
                valid = False
                break
        if valid:
            changed = copy.deepcopy(enemy)
            changed["items"] = packed
            result.append(changed)
    for index, item in enumerate(enemy["items"]):
        changed_item = item_step(item, direction)
        if changed_item is None:
            continue
        changed = copy.deepcopy(enemy)
        changed["items"][index] = changed_item
        result.append(changed)
    for count in range(2, len(enemy["items"]) + 1):
        changed = copy.deepcopy(enemy)
        changed_count = 0
        for index in range(count):
            changed_item = item_step(changed["items"][index], direction)
            if changed_item is not None:
                changed["items"][index] = changed_item
                changed_count += 1
        if changed_count:
            result.append(changed)
    weapon_items = [
        item
        for item in enemy["items"]
        if tuner.catalog[item["id"]].get("type") == "weapon"
    ]
    if direction > 0 and len(weapon_items) == 1:
        changed = copy.deepcopy(enemy)
        changed["items"].append(copy.deepcopy(weapon_items[0]))
        while True:
            try:
                changed["items"] = tuner.pack(changed["items"])
                result.insert(0, changed)
                break
            except ValueError:
                support = [
                    index
                    for index, item in enumerate(changed["items"])
                    if tuner.catalog[item["id"]].get("type") != "weapon"
                ]
                if not support:
                    break
                changed["items"].pop(support[-1])
    if direction > 0:
        offensive = ["chu_sa_ho_lo", "thien_dia_vo_cuc_cong", "am_duong_ngoc", "tu_kim_bat"]
        support = [
            index
            for index, item in enumerate(enemy["items"])
            if tuner.catalog[item["id"]].get("type") != "weapon"
        ]
        for count in range(1, len(support) + 1):
            changed = copy.deepcopy(enemy)
            for offset, index in enumerate(support[:count]):
                replacement = offensive[offset % len(offensive)]
                if tuner.catalog[replacement]["size"] != tuner.catalog[changed["items"][index]["id"]]["size"]:
                    continue
                changed["items"][index]["id"] = replacement
            result.append(changed)
    if direction < 0:
        for index, item in enumerate(enemy["items"]):
            if tuner.catalog[item["id"]].get("type") == "weapon":
                continue
            changed = copy.deepcopy(enemy)
            changed["items"].pop(index)
            changed["items"] = tuner.pack(changed["items"])
            result.append(changed)
    return result


def refine_stage(tuner: round2.Tuner, number: int, stage: dict[str, Any]) -> dict[str, Any]:
    """Greedily improve one stage with 20-run coordinate probes."""
    player, _starter, _lucky, _level, _silver = tuner.progression.bags_for_stage(number)
    limits = round2.band(stage["kind"])
    enemy = copy.deepcopy(stage["enemy"])
    rate = tuner.measure(player, enemy, tuner.args.probe_runs)
    for _attempt in range(16):
        if abs(rate - limits[2]) <= 5.0:
            break
        direction = 1 if rate > limits[1] else -1
        if limits[0] <= rate <= limits[1]:
            direction = 1 if rate > limits[2] else -1
        best_enemy = enemy
        best_rate = rate
        best_distance = abs(rate - limits[2])
        for candidate in candidates(tuner, enemy, direction):
            candidate_rate = tuner.measure(player, candidate, tuner.args.probe_runs)
            candidate_distance = abs(candidate_rate - limits[2])
            if candidate_distance < best_distance:
                best_enemy = candidate
                best_rate = candidate_rate
                best_distance = candidate_distance
            if candidate_distance <= 5.0:
                break
        if best_enemy is enemy:
            break
        enemy = best_enemy
        rate = best_rate
    confirmed = tuner.measure(player, enemy, tuner.args.confirm_runs)
    print(f"Ải {number:02d}: tinh chỉnh {rate:.1f}%, xác nhận {confirmed:.1f}%", flush=True)
    return enemy


def main() -> int:
    """Refine every currently out-of-band stage and save the candidate."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    args = argparse.Namespace(probe_runs=20, confirm_runs=50, start_stage=2)
    tuner = round2.Tuner(args)
    try:
        number = 0
        for chapter in tuner.document["chapters"]:
            for stage in chapter["stages"]:
                number += 1
                if number == 1:
                    continue
                player, _starter, _lucky, _level, _silver = tuner.progression.bags_for_stage(number)
                if number not in ROUND3_STAGES:
                    continue
                current = tuner.measure(player, stage["enemy"], 50)
                limits = round2.band(stage["kind"])
                if limits[0] <= current <= limits[1]:
                    continue
                stage["enemy"]["items"] = refine_stage(tuner, number, stage)["items"]
            round2.write_json(round2.OUT, tuner.document)
    finally:
        tuner.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
