#!/usr/bin/env python3
"""Shift PvE support items toward the measured baseline win rates."""

from __future__ import annotations

import json
import math
from pathlib import Path
from typing import Any

import pve_enemy_variety as variety

ROOT = Path(__file__).resolve().parents[2]
DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
CHAPTERS = ROOT / "tools" / "codex" / "out" / "pve-variety" / "chapters.json"
VERIFY = ROOT / "tools" / "codex" / "out" / "pve-variety" / "tables" / "verification.json"


def read_json(path: Path) -> Any:
    """Read a UTF-8 JSON file."""
    return json.loads(path.read_text(encoding="utf-8"))


def item_score(item_id: str, power_data: dict[str, Any], defensive: bool) -> float:
    """Score an item toward offense or defense."""
    values = power_data["items"][item_id]
    if defensive:
        return values["defense"] / max(0.01, values["offense"])
    return values["offense"] / max(0.01, values["defense"])


def main() -> int:
    """Apply one feedback adjustment pass to failed stages."""
    document = read_json(CHAPTERS)
    verification = read_json(VERIFY)
    power_data = read_json(DATA / "power.json")
    catalog_list = read_json(DATA / "items.json")
    catalog = {item["id"]: item for item in catalog_list}
    non_weapons = [item for item in catalog_list if item.get("type") != "weapon"]
    stage_list = [stage for chapter in document["chapters"] for stage in chapter["stages"]]
    for row, stage in zip(verification["rows"], stage_list):
        difference = row["newWinRate"] - row["oldWinRate"]
        if abs(difference) <= 10.0:
            continue
        defensive = difference < 0.0
        support = [
            item
            for item in stage["enemy"]["items"]
            if catalog[item["id"]].get("type") != "weapon"
        ]
        support.sort(
            key=lambda item: item_score(item["id"], power_data, defensive),
        )
        change_count = max(1, math.ceil(len(support) * min(1.0, abs(difference) / 100.0)))
        for item in support[:change_count]:
            size = catalog[item["id"]]["size"]
            choices = [candidate for candidate in non_weapons if candidate["size"] == size]
            selected = max(
                choices,
                key=lambda candidate: item_score(candidate["id"], power_data, defensive),
            )
            item["id"] = selected["id"]
        variety.repack(stage["enemy"]["items"], catalog)
        variety.tune_lc(stage["enemy"], row["oldLc"], power_data)
    variety.write_json(CHAPTERS, document)
    print(f"Adjusted {CHAPTERS}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
