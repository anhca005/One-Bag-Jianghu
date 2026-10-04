#!/usr/bin/env python3
"""Measure, calibrate, validate, and calculate display-only Luc Chien v1-v3."""
from __future__ import annotations

import argparse
import json
import math
import random
import re
import subprocess
import sys
import tempfile
from collections import defaultdict
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance  # noqa: E402
import pve_progression  # noqa: E402

DATA = ROOT / "server/battle-engine/src/main/resources/data"
ITEMS = DATA / "items.json"
SECTS = DATA / "sects.json"
OUT = ROOT / "tools/codex/out/power-v3"
POWER = OUT / "power.json"
TABLES = OUT / "tables"
V1 = ROOT / "tools/codex/out/power/power.json"
V2 = ROOT / "tools/codex/out/power-v2/power.json"
VALIDATION = ROOT / "tools/codex/out/power-v2/tables/validation.json"
RARITIES = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
RARITY = dict(zip(RARITIES, [1.0, 1.1, 1.21, 1.33, 1.46, 1.6]))
TIER = {str(value): 1.0 + 0.05 * (value - 1) for value in range(1, 10)}
SUMMARY = re.compile(r"A thắng ([0-9.]+)%.*?hòa ([0-9.]+)%.*?TB ([0-9.]+)s", re.S)
STATS = re.compile(r"\[A\].*?maxHp (\d+) · Hộ Thân (\d+)‰.*?sát thương gây ra TB ([0-9.]+)/trận", re.S)
SUPPORT = re.compile(r"hồi máu TB ([0-9.]+) · Cương Khí TB ([0-9.]+)")
GROUND = {
    "bang_phach_thuong": 63.0,
    "tu_mau_song_dao": 60.0,
    "chan_nhac_chuy": 60.0,
    "khai_son_phu": 47.0,
    "liet_diem_dao": 23.0,
    "none": 0.0,
}


def read(path: Path) -> Any:
    with path.open(encoding="utf-8") as stream:
        return json.load(stream)


def write(path: Path, value: Any) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def lc(fighter: dict[str, Any], power: dict[str, Any]) -> int:
    """Return LC for a v1, v2, or v3 catalog."""
    if power.get("version", 1) == 1:
        total = power["levelBase"] + power["levelPoints"] * (max(1, fighter.get("level", 1)) - 1)
        for item in fighter.get("items", []):
            value = power["items"][item["id"]]
            value *= power["tierFactor"][str(item.get("tier", 1))]
            value *= power["rarityFactor"][item.get("rarity", "common")]
            total += value
        return round(total)
    offense = 0.0
    defense = power["levelBaseDefense"]
    defense += power["levelDefensePerLevel"] * (max(1, fighter.get("level", 1)) - 1)
    for item in fighter.get("items", []):
        multiplier = power["tierMultiplier"][str(item.get("tier", 1))]
        multiplier *= power["rarityMultiplier"][item.get("rarity", "common")]
        offense += power["items"][item["id"]]["offense"] * multiplier
        defense += power["items"][item["id"]]["defense"] * multiplier
    return round(power["scale"] * math.sqrt(max(0.0, offense * defense)))


def sigmoid(value: float) -> float:
    return 1.0 / (1.0 + math.exp(-max(-30.0, min(30.0, value))))


class Runner:
    def __init__(self) -> None:
        self.catalog = read(ITEMS)
        self.by_id = {item["id"]: item for item in self.catalog}
        self.ids = list(self.by_id)
        self.placer = pve_progression.Progression()
        self.classpath = balance.ensure_classpath()

    def fighter(self, ids: list[str], level: int, rng: random.Random | None = None) -> dict[str, Any]:
        placed = []
        for item_id in ids:
            rarity = rng.choices(RARITIES, [48, 25, 14, 7, 4, 2])[0] if rng else "common"
            tier = rng.randint(1, min(9, 1 + level // 3)) if rng else 1
            self.placer._place(placed, {"id": item_id, "tier": tier, "rarity": rarity})
        return {
            "name": "power", "sect": "thai_thanh", "tamPhapLevel": 1, "level": level,
            "cols": 6, "rows": 8, "items": placed,
        }

    def run(self, side_a: dict[str, Any], side_b: dict[str, Any], runs: int, temp: Path,
            config: dict[str, Any] | None = None) -> str:
        paths = []
        for name, fighter in (("a", side_a), ("b", side_b)):
            path = temp / f"{name}.json"
            write(path, fighter)
            paths.append(path)
        command = [
            "java", "-cp", self.classpath, balance.MAIN_CLASS, str(paths[0]), str(paths[1]),
            "--runs", str(runs), "--items", str(ITEMS), "--sects", str(SECTS),
        ]
        if config:
            config_path = temp / "config.json"
            write(config_path, config)
            command.extend(["--battle-config", str(config_path)])
        return subprocess.run(
            command, check=True, capture_output=True, text=True, encoding="utf-8", errors="replace",
        ).stdout

    def fight(self, side_a: dict[str, Any], side_b: dict[str, Any], runs: int, temp: Path) -> float:
        match = SUMMARY.search(self.run(side_a, side_b, runs, temp))
        if not match:
            raise RuntimeError("Cannot parse battle summary")
        return (float(match.group(1)) + float(match.group(2)) / 2.0) / 100.0

    def breakdown(self, side_a: dict[str, Any], side_b: dict[str, Any], runs: int, temp: Path,
                  config: dict[str, Any]) -> dict[str, float]:
        output = self.run(side_a, side_b, runs, temp, config)
        summary = SUMMARY.search(output)
        stats = STATS.search(output)
        support = SUPPORT.search(output)
        if not summary or not stats or not support:
            raise RuntimeError(output)
        return {
            "duration": float(summary.group(3)), "maxHp": float(stats.group(1)),
            "reduction": float(stats.group(2)) / 1000.0, "damage": float(stats.group(3)),
            "heal": float(support.group(1)), "shield": float(support.group(2)),
        }

    def random_pair(self, rng: random.Random) -> tuple[dict[str, Any], dict[str, Any]]:
        fighters = []
        for unused in range(2):
            count = rng.randint(2, 10)
            fighters.append(self.fighter(rng.sample(self.ids, count), rng.randint(1, 23), rng))
        return fighters[0], fighters[1]


def effective_defense(stats: dict[str, float], incoming: float) -> float:
    reduction = min(0.799, stats["reduction"])
    avoided = incoming * reduction / max(0.001, 1.0 - reduction)
    time_factor = 30.0 / max(0.1, stats["duration"])
    return stats["maxHp"] + (stats["heal"] + stats["shield"] + avoided) * time_factor


def measure_items(runner: Runner, runs: int) -> tuple[dict[str, Any], list[dict[str, Any]]]:
    dummy_cfg = {"baseHp": 100000, "hpPerLevel": 0, "maxDurationMs": 60000,
                 "exhaustionStartMs": 999000}
    defense_cfg = {"baseHp": 360, "hpPerLevel": 0, "maxDurationMs": 30000,
                   "exhaustionStartMs": 999000}
    empty = runner.fighter([], 1)
    reference = runner.fighter(["thanh_phong_kiem"], 1)
    attacker = runner.fighter(["bang_phach_thuong", "chan_nhac_chuy", "tu_mau_song_dao"], 1)
    rows = []
    values = {}
    with tempfile.TemporaryDirectory(prefix="power-v3-items-") as raw:
        temp = Path(raw)
        reference_stats = runner.breakdown(reference, empty, runs, temp, dummy_cfg)
        base_stats = runner.breakdown(empty, attacker, runs, temp, defense_cfg)
        base_defense = effective_defense(base_stats, base_stats["damage"])
        reference_dps = reference_stats["damage"] / reference_stats["duration"]
        for index, item_id in enumerate(runner.ids, 1):
            item = runner.by_id[item_id]
            offense_ids = [item_id] if item["type"] == "weapon" else ["thanh_phong_kiem", item_id]
            offense_stats = runner.breakdown(runner.fighter(offense_ids, 1), empty, runs, temp, dummy_cfg)
            offense = offense_stats["damage"] / offense_stats["duration"]
            if item["type"] != "weapon":
                offense = max(0.0, offense - reference_dps)
            defense_stats = runner.breakdown(runner.fighter([item_id], 1), attacker, runs, temp, defense_cfg)
            defense = max(0.0, effective_defense(defense_stats, base_stats["damage"]) - base_defense)
            values[item_id] = {"offense": round(offense, 4), "defense": round(defense, 4)}
            rows.append({"item": item_id, "offenseStats": offense_stats,
                         "defenseStats": defense_stats, "value": values[item_id]})
            if index % 10 == 0:
                print(f"measure: {index}/{len(runner.ids)}", flush=True)
    return values, rows


def metric(rows: list[dict[str, Any]], power: dict[str, Any], slope: float) -> dict[str, float]:
    loss = 0.0
    correct = 0
    for row in rows:
        ratio = max(1, lc(row["a"], power)) / max(1, lc(row["b"], power))
        predicted = max(1e-7, min(1 - 1e-7, sigmoid(slope * math.log(ratio))))
        actual = row["actual"]
        loss -= actual * math.log(predicted) + (1 - actual) * math.log(1 - predicted)
        correct += int((ratio > 1) == (actual > 0.5))
    return {"accuracyPct": round(100 * correct / len(rows), 1), "logLoss": round(loss / len(rows), 4)}


def fit_slope(rows: list[dict[str, Any]], power: dict[str, Any]) -> float:
    choices = [value / 20.0 for value in range(1, 301)]
    return min(choices, key=lambda value: metric(rows, power, value)["logLoss"])


def ranks(values: list[float]) -> list[float]:
    return [sum(other < value for other in values) + (sum(other == value for other in values) + 1) / 2
            for value in values]


def spearman(actual: list[float], predicted: list[float]) -> float:
    left = ranks(actual)
    right = ranks(predicted)
    left_mean = sum(left) / len(left)
    right_mean = sum(right) / len(right)
    numerator = sum((a - left_mean) * (b - right_mean) for a, b in zip(left, right))
    left_sum = sum((value - left_mean) ** 2 for value in left)
    right_sum = sum((value - right_mean) ** 2 for value in right)
    return numerator / math.sqrt(left_sum * right_sum)


def ground_table(power: dict[str, Any], slope: float, runner: Runner) -> dict[str, Any]:
    base = ["thanh_phong_kiem", "o_tam_y", "boi_nguyen_dan", "kim_sang_duoc"]
    chapters = read(ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json")
    def find_stage(value: Any) -> dict[str, Any] | None:
        if isinstance(value, dict):
            if value.get("id") == "hoi_1_07":
                return value
            return next((found for child in value.values() if (found := find_stage(child))), None)
        if isinstance(value, list):
            return next((found for child in value if (found := find_stage(child))), None)
        return None
    stage = find_stage(chapters)
    if stage is None:
        raise ValueError("hoi_1_07 not found")
    enemy = stage.get("enemy", stage.get("opponent", stage))
    enemy_lc = lc(enemy, power)
    rows = []
    for item_id, actual in GROUND.items():
        player = {
            "name": "user ground truth", "sect": "thai_thanh", "tamPhapLevel": 1,
            "level": 9, "cols": 5, "rows": 4,
            "items": [
                {"id": base[0], "tier": 1, "rarity": "common", "x": 0, "y": 0},
                {"id": base[1], "tier": 1, "rarity": "common", "x": 1, "y": 0},
                {"id": base[2], "tier": 1, "rarity": "common", "x": 1, "y": 2},
                {"id": base[3], "tier": 1, "rarity": "common", "x": 2, "y": 2},
            ],
        }
        if item_id != "none":
            player["items"].append(
                {"id": item_id, "tier": 1, "rarity": "common", "x": 3, "y": 0},
            )
        predicted = 100 * sigmoid(slope * math.log(lc(player, power) / max(1, enemy_lc)))
        rows.append({"item": item_id, "actualPct": actual, "predictedPct": round(predicted, 1)})
    return {"rows": rows, "spearman": round(spearman(list(GROUND.values()),
                                                       [row["predictedPct"] for row in rows]), 4)}


def item_tables(power: dict[str, Any], runner: Runner) -> dict[str, Any]:
    rows = [{"id": item["id"], "name": item["name"], "type": item["type"], "size": item["size"],
             "lc": lc(runner.fighter([item["id"]], 1), power)} for item in runner.catalog]
    medians = {}
    for item_type in sorted({row["type"] for row in rows}):
        values = sorted(row["lc"] for row in rows if row["type"] == item_type)
        medians[item_type] = values[(len(values) - 1) // 2]
    groups = defaultdict(list)
    for row in rows:
        groups[(row["type"], tuple(row["size"]))].append(row["lc"])
    outliers = []
    for row in rows:
        values = sorted(groups[(row["type"], tuple(row["size"]))])
        median = (values[(len(values) - 1) // 2] + values[len(values) // 2]) / 2
        if median and row["lc"] > 1.4 * median:
            outliers.append({**row, "groupMedian": median, "abovePct": round(100 * (row["lc"] / median - 1), 1)})
    return {"top10": sorted(rows, key=lambda row: -row["lc"])[:10],
            "bottom10": sorted(rows, key=lambda row: row["lc"])[:10],
            "typeMedians": medians, "outliers": outliers}


def build_power(items: dict[str, Any]) -> dict[str, Any]:
    offense = items["thanh_phong_kiem"]["offense"] + items["o_tam_y"]["offense"]
    defense = 100 + items["thanh_phong_kiem"]["defense"] + items["o_tam_y"]["defense"]
    return {"version": 3, "recommendedFactor": 1.1, "tierMultiplier": TIER,
            "rarityMultiplier": RARITY, "levelBaseDefense": 100, "levelDefensePerLevel": 5,
            "scale": round(100 / math.sqrt(max(0.0001, offense * defense)), 8), "items": items}


def calibrate(args: argparse.Namespace) -> None:
    runner = Runner()
    if args.measure or not (TABLES / "item-values.json").exists():
        items, measurements = measure_items(runner, args.item_runs)
        write(TABLES / "item-measurements.json", measurements)
        write(TABLES / "item-values.json", items)
    else:
        items = read(TABLES / "item-values.json")
    power = build_power(items)
    fit_rows = []
    rng = random.Random(53003)
    with tempfile.TemporaryDirectory(prefix="power-v3-fit-") as raw:
        for index in range(args.fit_matchups):
            side_a, side_b = runner.random_pair(rng)
            fit_rows.append({"a": side_a, "b": side_b,
                             "actual": runner.fight(side_a, side_b, args.fit_runs, Path(raw))})
    slope = fit_slope(fit_rows, power)
    validation = read(VALIDATION)
    metrics = {"v1": metric(validation, read(V1), 5.0), "v2": metric(validation, read(V2), 5.0),
               "v3": metric(validation, power, slope), "slopeK": slope}
    write(POWER, power)
    write(TABLES / "fit.json", fit_rows)
    write(TABLES / "validation.json", validation)
    write(TABLES / "metrics.json", metrics)
    write(TABLES / "ground-truth.json", ground_table(power, slope, runner))
    write(TABLES / "items.json", item_tables(power, runner))
    print(json.dumps(metrics, ensure_ascii=False))


def measure_only(args: argparse.Namespace) -> None:
    runner = Runner()
    items, measurements = measure_items(runner, args.item_runs)
    write(TABLES / "item-measurements.json", measurements)
    write(TABLES / "item-values.json", items)
    print(f"wrote {len(items)} item measurements")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("loadout", nargs="?", type=Path)
    parser.add_argument("--power", type=Path, default=POWER)
    parser.add_argument("--calibrate", action="store_true")
    parser.add_argument("--validate", action="store_true")
    parser.add_argument("--measure", action="store_true")
    parser.add_argument("--item-runs", type=int, default=10)
    parser.add_argument("--fit-matchups", type=int, default=50)
    parser.add_argument("--fit-runs", type=int, default=20)
    args = parser.parse_args()
    if args.measure:
        measure_only(args)
    elif args.calibrate:
        calibrate(args)
    elif args.validate:
        metrics = read(TABLES / "metrics.json")
        validation = read(VALIDATION)
        slope = metrics["slopeK"]
        power = read(POWER)
        print(json.dumps({"v1": metric(validation, read(V1), 5.0),
                          "v2": metric(validation, read(V2), 5.0),
                          "v3": metric(validation, power, slope)}, ensure_ascii=False))
    elif args.loadout:
        print(lc(read(args.loadout), read(args.power)))
    else:
        parser.error("loadout, --measure, --calibrate, or --validate required")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
