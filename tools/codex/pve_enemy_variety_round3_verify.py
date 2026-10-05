#!/usr/bin/env python3
"""Verify and apply the approved PvE enemy-variety Round-3 candidate."""

from __future__ import annotations

import argparse
import copy
import shutil
import sys
from collections import Counter
from pathlib import Path
from typing import Any

import pve_enemy_variety_round2 as round2

sys.path.insert(0, str(round2.ROOT / "tools" / "balance"))
import power  # noqa: E402


ROUND3_STAGES = {9, 16, 18, 20, 23, 27, 31, 34, 36, 41, 42, 44, 50}
ROUND2 = round2.OUT.parent / "chapters-round2.json"
RESULTS = round2.OUT.parent / "tables" / "round3-verification.json"
BACKUP = round2.OUT.parent / "chapters-before.json"
POWER = round2.ROOT / "tools" / "codex" / "out" / "power-hybrid" / "power.json"


def stages(document: dict[str, Any]) -> list[dict[str, Any]]:
    """Flatten chapter stages."""
    return [stage for chapter in document["chapters"] for stage in chapter["stages"]]


def without_items(document: dict[str, Any]) -> dict[str, Any]:
    """Return a copy without enemy item arrays."""
    result = copy.deepcopy(document)
    for stage in stages(result):
        stage["enemy"].pop("items")
    return result


def validate_bag(tuner: round2.Tuner, enemy: dict[str, Any], number: int) -> None:
    """Validate item identity, rarity, bounds, overlap, and weapon copy count."""
    occupied: set[tuple[int, int]] = set()
    weapons: Counter[str] = Counter()
    for item in enemy["items"]:
        if item["id"] not in tuner.catalog:
            raise ValueError(f"Ải {number}: vật phẩm không tồn tại {item['id']}")
        if item["rarity"] not in round2.RARITIES or not 1 <= item["tier"] <= 9:
            raise ValueError(f"Ải {number}: phẩm/Trọng không hợp lệ")
        catalog_item = tuner.catalog[item["id"]]
        width, height = catalog_item["size"]
        cells = {
            (item["x"] + offset_x, item["y"] + offset_y)
            for offset_x in range(width)
            for offset_y in range(height)
        }
        if any(x < 0 or x >= 5 or y < 0 or y >= 4 for x, y in cells):
            raise ValueError(f"Ải {number}: vật phẩm vượt túi")
        if not cells.isdisjoint(occupied):
            raise ValueError(f"Ải {number}: vật phẩm chồng nhau")
        occupied.update(cells)
        if catalog_item.get("type") == "weapon":
            weapons[item["id"]] += 1
    if any(count > 2 for count in weapons.values()):
        raise ValueError(f"Ải {number}: quá 2 bản sao vũ khí")


def validate_hard_rules(tuner: round2.Tuner) -> list[dict[str, int]]:
    """Validate unchanged scope and all weapon-variety rules."""
    candidate = tuner.document
    source = tuner.original
    round2_document = round2.read_json(ROUND2)
    if without_items(candidate) != without_items(source):
        raise ValueError("Có trường ngoài enemy.items bị thay đổi")
    candidate_stages = stages(candidate)
    round2_stages = stages(round2_document)
    if candidate_stages[0] != round2_stages[0]:
        raise ValueError("Ải 1 bị thay đổi")
    for number, (current, previous) in enumerate(zip(candidate_stages, round2_stages), 1):
        if number not in ROUND3_STAGES and current["enemy"]["items"] != previous["enemy"]["items"]:
            raise ValueError(f"Ải {number} ngoài phạm vi Round 3 bị thay đổi")
        validate_bag(tuner, current["enemy"], number)
    chapter_counts = []
    previous_weapon: str | None = None
    for chapter_index, chapter in enumerate(candidate["chapters"], 1):
        counts: Counter[str] = Counter()
        for stage in chapter["stages"]:
            weapon_ids = {
                item["id"]
                for item in stage["enemy"]["items"]
                if tuner.catalog[item["id"]].get("type") == "weapon"
            }
            if not weapon_ids:
                raise ValueError("Địch không có vũ khí")
            counts.update(weapon_ids)
            current_weapon = sorted(weapon_ids)[0]
            if current_weapon == previous_weapon:
                raise ValueError(f"Hai ải liên tiếp dùng {current_weapon}")
            previous_weapon = current_weapon
        if max(counts.values()) > 3:
            raise ValueError(f"Hồi {chapter_index} vượt trần đa dạng: {counts}")
        sword_limit = 2 if chapter_index == 1 else 1
        if counts["thanh_phong_kiem"] > sword_limit:
            raise ValueError(f"Hồi {chapter_index} có quá nhiều Thanh Phong Kiếm")
        chapter_counts.append(dict(counts))
    return chapter_counts


def main() -> int:
    """Confirm the 13 stages, validate, and apply when the gate passes."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    args = argparse.Namespace(probe_runs=20, confirm_runs=50, start_stage=2)
    tuner = round2.Tuner(args)
    try:
        chapter_counts = validate_hard_rules(tuner)
        power_data = round2.read_json(POWER)
        rows = []
        for number in sorted(ROUND3_STAGES):
            stage = stages(tuner.document)[number - 1]
            old_stage = stages(round2.read_json(ROUND2))[number - 1]
            player, _starter, _lucky, _level, _silver = tuner.progression.bags_for_stage(number)
            rate = tuner.measure(player, stage["enemy"], 50)
            limits = round2.band(stage["kind"])
            rows.append({
                "stage": number,
                "enemy": stage["enemy"]["name"],
                "round2Lc": power.lc(old_stage["enemy"], power_data),
                "round3Lc": power.lc(stage["enemy"], power_data),
                "round3WinRate": rate,
                "target": limits[2],
                "band": [limits[0], limits[1]],
                "inBand": limits[0] <= rate <= limits[1],
            })
            print(f"Ải {number:02d}: {rate:.1f}%", flush=True)
        final_count = 36 + sum(row["inBand"] for row in rows)
        stage50_passes = next(row["inBand"] for row in rows if row["stage"] == 50)
        applied = final_count >= 45 and stage50_passes
        result = {
            "runs": 50,
            "rows": rows,
            "chapterCounts": chapter_counts,
            "finalInBand": final_count,
            "applied": applied,
        }
        round2.write_json(RESULTS, result)
        if applied:
            shutil.copy2(round2.SOURCE, BACKUP)
            shutil.copy2(round2.OUT, round2.SOURCE)
        print(f"Trong dải: {final_count}/49; áp dụng: {applied}")
    finally:
        tuner.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
