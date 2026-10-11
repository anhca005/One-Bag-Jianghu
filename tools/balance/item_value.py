#!/usr/bin/env python3
"""Measure each item's combat value inside a realistic bag, per cell (decision #61).

Method:
- Base bag: the "typical" expected player bag at a few stages (pve_progression), placed on a large grid so any
  item can be added without removing others.
- Value of item X: add X to the base bag and fight the unchanged base bag. Both sides swap seats and the results
  are averaged, which removes the first-seat bias.
- Strength is read from the engine's per-side totals, not from the win rate (which saturates at 0%/100% as soon as a
  large item is added). Each side's pool = max HP + healing + Cương Khí; the strength ratio is
  (share of the enemy pool we destroy) / (share of our pool the enemy destroys). Its log is the value: continuous,
  additive, and 0 for an even fight.
- Per cell: the value divided by the item's cell count, normalised so the anchor items average 1.0.
- Stacking: for 1-cell items, also add 3 copies; a per-copy value above the single-copy value means stacking is
  stronger than linear (the exploit found in round 5).

Usage:
  py tools/balance/item_value.py                      # all items, default stages and runs
  py tools/balance/item_value.py --items <items.json> # measure a candidate items file
  py tools/balance/item_value.py --only thai_thanh_tam_kinh,o_tam_y
"""
from __future__ import annotations

import argparse
import csv
import json
import math
import re
import statistics
import subprocess
import sys
import tempfile
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance  # noqa: E402
import pve_progression  # noqa: E402

ENGINE_DATA = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data"
DEFAULT_ITEMS = ENGINE_DATA / "items.json"
SECTS = ENGINE_DATA / "sects.json"
OUT = ROOT / "tools" / "balance" / "out" / "item-value"
BAG_CACHE = OUT / "typical-bags.json"
GRID_COLS = 8
GRID_ROWS = 8
ANCHORS = ["thanh_phong_kiem", "o_tam_y"]
STACK_COPIES = 3
SUMMARY_PREFIX = "A thắng "
SIDE_STATS = re.compile(
    r"\[(?P<side>[AB])\] measure — maxHp (?P<hp>\d+).*?sát thương gây ra TB (?P<damage>[0-9.]+)/trận"
    r".*?hồi máu TB (?P<heal>[0-9.]+) · Cương Khí TB (?P<shield>[0-9.]+)",
    re.S,
)


def read_json(path: Path) -> Any:
    with path.open(encoding="utf-8") as handle:
        return json.load(handle)


def write_json(path: Path, value: Any) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8") as handle:
        json.dump(value, handle, ensure_ascii=False, indent=2)


class Arena:
    """Runs battles with the Java engine on a large grid."""

    def __init__(self, items_path: Path, runs: int) -> None:
        self.items_path = items_path
        self.runs = runs
        self.catalog = {item["id"]: item for item in read_json(items_path)}
        self.classpath = balance.ensure_classpath()
        self.temp_dir = tempfile.TemporaryDirectory(prefix="item-value-")
        self.temp = Path(self.temp_dir.name)

    def close(self) -> None:
        self.temp_dir.cleanup()

    def cells(self, item_id: str) -> int:
        width, height = self.catalog[item_id]["size"]
        return width * height

    def place(self, items: list[dict[str, Any]]) -> list[dict[str, Any]]:
        """Place items top-left first on the large grid; fail loudly if one does not fit."""
        occupied: set[tuple[int, int]] = set()
        placed = []
        for item in items:
            width, height = self.catalog[item["id"]]["size"]
            spot = self._free_spot(occupied, width, height)
            if spot is None:
                raise ValueError(f"No room for {item['id']} on the {GRID_COLS}x{GRID_ROWS} grid")
            x, y = spot
            for cell_x in range(x, x + width):
                for cell_y in range(y, y + height):
                    occupied.add((cell_x, cell_y))
            entry = {"id": item["id"], "tier": item["tier"], "rarity": item["rarity"], "x": x, "y": y}
            placed.append(entry)
        return placed

    @staticmethod
    def _free_spot(occupied: set[tuple[int, int]], width: int, height: int) -> tuple[int, int] | None:
        for y in range(GRID_ROWS - height + 1):
            for x in range(GRID_COLS - width + 1):
                cells = {(x + dx, y + dy) for dx in range(width) for dy in range(height)}
                if not cells & occupied:
                    return x, y
        return None

    def fighter(self, items: list[dict[str, Any]], level: int) -> dict[str, Any]:
        return {
            "name": "measure",
            "sect": None,
            "tamPhapLevel": 1,
            "level": level,
            "cols": GRID_COLS,
            "rows": GRID_ROWS,
            "items": self.place(items),
        }

    def battle(self, side_a: dict[str, Any], side_b: dict[str, Any]) -> str:
        """Engine summary text for side A against side B."""
        paths = []
        for name, fighter in (("a", side_a), ("b", side_b)):
            path = self.temp / f"{name}.json"
            write_json(path, fighter)
            paths.append(path)
        command = [
            "java", "-cp", self.classpath, balance.MAIN_CLASS, str(paths[0]), str(paths[1]),
            "--runs", str(self.runs), "--items", str(self.items_path), "--sects", str(SECTS),
        ]
        output = subprocess.run(
            command, check=True, capture_output=True, text=True, encoding="utf-8", errors="replace",
        ).stdout
        return output

    def strength(self, side_a: dict[str, Any], side_b: dict[str, Any]) -> tuple[float, float]:
        """A's log strength ratio over B and A's win rate, both averaged over the two seats."""
        first = self.battle(side_a, side_b)
        second = self.battle(side_b, side_a)
        log_ratio = (log_strength_ratio(first) - log_strength_ratio(second)) / 2.0
        rate = (parse_rate(first) + 1.0 - parse_rate(second)) / 2.0
        return log_ratio, rate


def log_strength_ratio(output: str) -> float:
    """log of (share of B's pool that A destroys) / (share of A's pool that B destroys)."""
    sides = {}
    for match in SIDE_STATS.finditer(output):
        pool = float(match["hp"]) + float(match["heal"]) + float(match["shield"])
        sides[match["side"]] = (max(float(match["damage"]), 1.0), max(pool, 1.0))
    if set(sides) != {"A", "B"}:
        raise RuntimeError("Cannot read both sides' stats from the engine output")
    damage_a, pool_a = sides["A"]
    damage_b, pool_b = sides["B"]
    return math.log(damage_a / pool_b) - math.log(damage_b / pool_a)


def parse_rate(output: str) -> float:
    start = output.find(SUMMARY_PREFIX)
    if start < 0:
        raise RuntimeError("Cannot find the battle summary in the engine output")
    won_text = output[start + len(SUMMARY_PREFIX):].split("%", 1)[0]
    draw_start = output.find("hòa ", start)
    draw_text = output[draw_start + len("hòa "):].split("%", 1)[0] if draw_start >= 0 else "0"
    return (float(won_text) + float(draw_text) / 2.0) / 100.0


def typical_bags(stages: list[int]) -> dict[int, dict[str, Any]]:
    """Typical expected bags per stage, cached because the economy simulation is slow."""
    cache: dict[str, Any] = read_json(BAG_CACHE) if BAG_CACHE.exists() else {}
    missing = [stage for stage in stages if str(stage) not in cache]
    if missing:
        progression = pve_progression.Progression()
        for stage in missing:
            bag = progression.bags_for_stage(stage)[0]
            cache[str(stage)] = {"level": bag["level"], "items": bag["items"]}
            print(f"cached typical bag for stage {stage}", flush=True)
        write_json(BAG_CACHE, cache)
    return {stage: cache[str(stage)] for stage in stages}


def reference_grade(bag: dict[str, Any]) -> tuple[int, str]:
    """The tier and rarity a measured item gets: the bag's median tier, common rarity."""
    tiers = [item["tier"] for item in bag["items"]]
    return int(round(statistics.median(tiers))), "common"


def measure_item(arena: Arena, bag: dict[str, Any], item_id: str, copies: int) -> float:
    """Log strength ratio gained by adding `copies` of the item to the bag."""
    tier, rarity = reference_grade(bag)
    base_items = [dict(item) for item in bag["items"]]
    added = [{"id": item_id, "tier": tier, "rarity": rarity} for _ in range(copies)]
    with_item = arena.fighter(base_items + added, bag["level"])
    without = arena.fighter(base_items, bag["level"])
    log_ratio, _rate = arena.strength(with_item, without)
    return log_ratio


def measure_rows(
    arena: Arena, bags: dict[int, dict[str, Any]], stages: list[int], ids: list[str],
) -> tuple[list[dict[str, Any]], float]:
    """Measure the given items (anchors are always added) and normalise per cell to the anchors of this run."""
    ids = list(ids)
    for anchor in ANCHORS:
        if anchor not in ids:
            ids.append(anchor)
    rows = []
    for item_id in ids:
        cells = arena.cells(item_id)
        values = [measure_item(arena, bags[stage], item_id, 1) for stage in stages]
        single = statistics.mean(values)
        row = {
            "id": item_id,
            "type": arena.catalog[item_id]["type"],
            "size": "x".join(str(value) for value in arena.catalog[item_id]["size"]),
            "cells": cells,
            "value": single,
            "perCellRaw": single / cells,
            "stackRatio": "",
        }
        if cells == 1:
            stacked = [measure_item(arena, bags[stage], item_id, STACK_COPIES) for stage in stages]
            per_copy = statistics.mean(stacked) / STACK_COPIES
            row["stackRatio"] = round(per_copy / single, 2) if abs(single) > 1e-6 else ""
        rows.append(row)
        print(f"{item_id:28s} {row['type']:7s} {row['size']:4s} value {row['value']:+.3f}", flush=True)
    anchor_rows = [row for row in rows if row["id"] in ANCHORS]
    anchor_per_cell = statistics.mean(row["perCellRaw"] for row in anchor_rows)
    for row in rows:
        row["perCell"] = round(row["perCellRaw"] / anchor_per_cell, 3) if anchor_per_cell else 0.0
        row["value"] = round(row["value"], 4)
        row["perCellRaw"] = round(row["perCellRaw"], 4)
    rows.sort(key=lambda row: row["perCell"], reverse=True)
    return rows, anchor_per_cell


def write_rows(path: Path, rows: list[dict[str, Any]]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(rows[0]))
        writer.writeheader()
        writer.writerows(rows)


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--items", type=Path, default=DEFAULT_ITEMS, help="items.json to measure")
    parser.add_argument("--stages", default="15,30,45", help="stages whose typical bag is the base")
    parser.add_argument("--runs", type=int, default=1000, help="battles per seat per measurement")
    parser.add_argument("--only", default="", help="comma-separated item ids (default: all)")
    parser.add_argument("--out", type=Path, default=OUT / "item-value.csv")
    args = parser.parse_args()

    stages = [int(value) for value in args.stages.split(",")]
    bags = typical_bags(stages)
    arena = Arena(args.items, args.runs)
    try:
        ids = [value for value in args.only.split(",") if value] or sorted(arena.catalog)
        rows, anchor_per_cell = measure_rows(arena, bags, stages, ids)
    finally:
        arena.close()
    write_rows(args.out, rows)
    print(f"wrote {args.out} ({len(rows)} items); anchor per-cell raw = {anchor_per_cell:.4f}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
