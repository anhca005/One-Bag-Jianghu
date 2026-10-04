#!/usr/bin/env python3
"""Build and verify every PvE enemy from calibrated Luc Chien."""

from __future__ import annotations

import argparse
import copy
import json
import math
import re
import subprocess
import sys
import tempfile
from dataclasses import dataclass
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance  # noqa: E402
import power  # noqa: E402
import pve_progression  # noqa: E402

DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
SOURCE_CHAPTERS = DATA / "Pve" / "chapters.json"
POWER_PATH = ROOT / "tools" / "codex" / "out" / "power-hybrid" / "power.json"
ENGINE_DATA = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data"
ENGINE_ITEMS = ENGINE_DATA / "items.json"
ENGINE_SECTS = ENGINE_DATA / "sects.json"
DEFAULT_OUT = ROOT / "tools" / "codex" / "out" / "pve-upgrade" / "chapters.json"
RARITIES = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
LOADOUT_KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")
RESULT_RE = re.compile(
    r"A thắng ([0-9.]+)%\s+\|\s+B thắng ([0-9.]+)%\s+\|\s+hòa ([0-9.]+)%.*?TB ([0-9.]+)s",
    re.DOTALL,
)
TARGET_WINS = {"normal": 0.72, "elite": 0.62, "boss": 0.50}
BANDS = {"normal": (62.0, 82.0), "elite": (52.0, 72.0), "boss": (40.0, 60.0)}
STAGE_ONE_TARGET = 0.80
STAGE_ONE_BAND = (70.0, 90.0)
SLOPE = 30.68


def read_json(path: Path) -> Any:
    """Read one UTF-8 JSON document."""
    with path.open(encoding="utf-8") as stream:
        return json.load(stream)


def write_json(path: Path, value: Any) -> None:
    """Write stable, human-readable JSON."""
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="\n") as stream:
        json.dump(value, stream, ensure_ascii=False, indent=2)
        stream.write("\n")


@dataclass
class Measurement:
    """One battle-engine measurement."""

    win_rate: float
    seconds: float


@dataclass
class StageResult:
    """Fields needed by the final report."""

    number: int
    kind: str
    level: int
    silver: int
    expected_items: int
    expected_lc: int
    average_tier: float
    spent_items: int
    spent_materials: int
    spent_upgrades: int
    spent_refresh: int
    materials: dict[str, int]
    enemy_lc: int
    enemy_items: int
    expected: Measurement
    starter_only: Measurement
    lucky: Measurement
    target_lc: float
    adjustment: str
    correction_steps: int
    tune_rate: float
    fresh_rate: float


class TeeOutput:
    """Copy stdout verbatim into a table artifact."""

    def __init__(self, console: Any, path: Path) -> None:
        path.parent.mkdir(parents=True, exist_ok=True)
        self.console = console
        self.file = path.open("w", encoding="utf-8", newline="\n")

    def write(self, value: str) -> int:
        """Write to both streams."""
        self.console.write(value)
        self.file.write(value)
        return len(value)

    def flush(self) -> None:
        """Flush both streams."""
        self.console.flush()
        self.file.flush()

    def close(self) -> None:
        """Close the owned stream."""
        self.file.close()


class LcBuilder:
    """Construct LC-targeted enemies and verify them in the battle engine."""

    def __init__(self, args: argparse.Namespace) -> None:
        self.args = args
        self.document = read_json(SOURCE_CHAPTERS)
        self.power = read_json(POWER_PATH)
        self.catalog = read_json(ENGINE_ITEMS)
        self.catalog_by_id = {item["id"]: item for item in self.catalog}
        self.progression = pve_progression.Progression()
        self.classpath = balance.ensure_classpath()
        self.results: list[StageResult] = []
        self.existing_rows: list[dict[str, Any]] = []
        self.temp_dir = tempfile.TemporaryDirectory(prefix="pve-lc-")
        self.temp_path = Path(self.temp_dir.name)

    def close(self) -> None:
        """Remove temporary loadouts."""
        self.temp_dir.cleanup()

    def item_base(self, item: dict[str, Any]) -> float:
        """Return the calibrated base value used for deterministic ties."""
        loadout = {
            "level": 1,
            "items": [{**item, "x": 0, "y": 0}],
        }
        return power.lc(loadout, self.power)

    def is_weapon(self, item: dict[str, Any]) -> bool:
        """Return whether an item instance is a weapon."""
        return self.catalog_by_id[item["id"]].get("type") == "weapon"

    def measure(self, player: dict[str, Any], enemy: dict[str, Any], runs: int) -> Measurement:
        """Measure player wins, counting a draw as half a win."""
        paths = []
        for filename, loadout in (("player.json", player), ("enemy.json", enemy)):
            path = self.temp_path / filename
            prepared = {key: loadout[key] for key in LOADOUT_KEYS if key in loadout}
            prepared.setdefault("cols", 5)
            prepared.setdefault("rows", 4)
            write_json(path, prepared)
            paths.append(path)
        command = [
            "java", "-cp", self.classpath, balance.MAIN_CLASS, str(paths[0]), str(paths[1]),
            "--runs", str(runs), "--items", str(ENGINE_ITEMS), "--sects", str(ENGINE_SECTS),
        ]
        completed = subprocess.run(
            command,
            check=True,
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
        )
        match = RESULT_RE.search(completed.stdout)
        if match is None:
            raise RuntimeError(f"Cannot parse battle output:\n{completed.stdout}")
        return Measurement(float(match.group(1)) + float(match.group(3)) / 2.0, float(match.group(4)))

    def mirror(self, stage: dict[str, Any], player: dict[str, Any], level: int) -> dict[str, Any]:
        """Copy only combat fields into the source enemy identity."""
        enemy = copy.deepcopy(stage["enemy"])
        enemy["level"] = min(30, level + int(stage["kind"] == "boss"))
        enemy["items"] = copy.deepcopy(player["items"])
        return enemy

    def move_key(self, enemy: dict[str, Any], target: float, item: dict[str, Any]) -> tuple[Any, ...]:
        """Order moves by target distance and the specified deterministic ties."""
        return (
            abs(power.lc(enemy, self.power) - target),
            int(self.is_weapon(item)),
            self.item_base(item),
            item["id"],
        )

    def best_step(self, enemy: dict[str, Any], target: float, field: str) -> dict[str, Any] | None:
        """Find the closest one-step rarity or tier move."""
        candidates = []
        for index, item in enumerate(enemy["items"]):
            if field == "rarity":
                old_value = RARITIES.index(item["rarity"])
                limits = (0, len(RARITIES) - 1)
            else:
                old_value = int(item["tier"])
                limits = (1, 9)
            for delta in (-1, 1):
                new_value = old_value + delta
                if not limits[0] <= new_value <= limits[1]:
                    continue
                candidate = copy.deepcopy(enemy)
                changed = candidate["items"][index]
                changed[field] = RARITIES[new_value] if field == "rarity" else new_value
                candidates.append((self.move_key(candidate, target, changed), candidate))
        if not candidates:
            return None
        return min(candidates, key=lambda entry: entry[0])[1]

    def place_added(self, items: list[dict[str, Any]], item_id: str) -> list[dict[str, Any]] | None:
        """Repack an enemy after adding one common T1 non-weapon."""
        unplaced = [{key: item[key] for key in ("id", "tier", "rarity")} for item in items]
        unplaced.append({"id": item_id, "tier": 1, "rarity": "common"})
        packed: list[dict[str, Any]] = []
        for item in unplaced:
            if not self.progression._place(packed, item):
                return None
        return packed

    def best_item_count_step(
        self, enemy: dict[str, Any], target: float, minimum_weapons: int,
    ) -> dict[str, Any] | None:
        """Find the closest allowed one-item removal or addition."""
        candidates = []
        for index, item in enumerate(enemy["items"]):
            if self.is_weapon(item):
                continue
            candidate = copy.deepcopy(enemy)
            changed = candidate["items"].pop(index)
            candidates.append((self.move_key(candidate, target, changed), candidate))
        non_weapons = [item for item in self.catalog if item.get("type") != "weapon"]
        for catalog_item in non_weapons:
            packed = self.place_added(enemy["items"], catalog_item["id"])
            if packed is None:
                continue
            candidate = copy.deepcopy(enemy)
            candidate["items"] = packed
            changed = candidate["items"][-1]
            candidates.append((self.move_key(candidate, target, changed), candidate))
        if not candidates:
            return None
        selected = min(candidates, key=lambda entry: entry[0])[1]
        weapon_count = sum(self.is_weapon(item) for item in selected["items"])
        return selected if weapon_count >= minimum_weapons else None

    def tune(self, mirror: dict[str, Any], target: float, minimum_weapons: int) -> tuple[dict[str, Any], str]:
        """Apply the smallest permitted moves until none improves LC distance."""
        enemy = copy.deepcopy(mirror)
        moves = []
        for field in ("rarity", "tier"):
            while abs(power.lc(enemy, self.power) / target - 1.0) > 0.015:
                candidate = self.best_step(enemy, target, field)
                if candidate is None:
                    break
                old_distance = abs(power.lc(enemy, self.power) - target)
                new_distance = abs(power.lc(candidate, self.power) - target)
                if new_distance >= old_distance:
                    break
                enemy = candidate
                moves.append(field)
        while abs(power.lc(enemy, self.power) / target - 1.0) > 0.015:
            candidate = self.best_item_count_step(enemy, target, minimum_weapons)
            if candidate is None:
                break
            old_distance = abs(power.lc(enemy, self.power) - target)
            new_distance = abs(power.lc(candidate, self.power) - target)
            if new_distance >= old_distance:
                break
            enemy = candidate
            moves.append("item")
        label = "mirror" if not moves else ",".join(moves)
        return enemy, label

    @staticmethod
    def distance_to_band(rate: float, band: tuple[float, float]) -> float:
        """Return zero in-band and distance to the nearest edge otherwise."""
        if rate < band[0]:
            return band[0] - rate
        if rate > band[1]:
            return rate - band[1]
        return 0.0

    def build_stage(self, number: int, stage: dict[str, Any]) -> None:
        """Build, verify, optionally correct, and record one stage."""
        expected, starter_only, lucky, level, silver = self.progression.bags_for_stage(number)
        curve_point = self.progression.point_for_stage(number)
        stage["recommendedLevel"] = level
        target_bag = starter_only if number == 1 else expected
        player_lc = power.lc(target_bag, self.power)
        stage["recommendedPower"] = player_lc
        win_target = STAGE_ONE_TARGET if number == 1 else TARGET_WINS[stage["kind"]]
        ratio = math.exp(math.log(win_target / (1.0 - win_target)) / SLOPE)
        target = player_lc / ratio
        base_enemy = self.mirror(stage, target_bag, level)
        minimum_weapons = sum(self.is_weapon(item) for item in target_bag["items"])
        selected_enemy, selected_label = self.tune(base_enemy, target, minimum_weapons)
        selected_measure = self.measure(target_bag, selected_enemy, 20)
        selected_target = target
        band = STAGE_ONE_BAND if number == 1 else BANDS[stage["kind"]]
        correction_steps = 0
        measurements = [selected_measure.win_rate]
        while self.distance_to_band(selected_measure.win_rate, band) > 0 and correction_steps < 5:
            observed = min(0.95, max(0.05, selected_measure.win_rate / 100.0))
            observed_logit = math.log(observed / (1.0 - observed))
            target_logit = math.log(win_target / (1.0 - win_target))
            factor = math.exp((observed_logit - target_logit) / SLOPE)
            factor = min(1.15, max(0.85, factor))
            selected_target *= factor
            candidate, label = self.tune(base_enemy, selected_target, minimum_weapons)
            candidate_measure = self.measure(target_bag, candidate, 20)
            correction_steps += 1
            measurements.append(candidate_measure.win_rate)
            old_key = (
                self.distance_to_band(selected_measure.win_rate, band),
                abs(selected_measure.win_rate - win_target * 100),
            )
            new_key = (
                self.distance_to_band(candidate_measure.win_rate, band),
                abs(candidate_measure.win_rate - win_target * 100),
            )
            if new_key <= old_key:
                selected_enemy = candidate
                selected_measure = candidate_measure
                selected_label = label
        stage["enemy"] = selected_enemy
        tune_rate = selected_measure.win_rate
        first_fifty = self.measure(target_bag, selected_enemy, 50)
        first_hundred = self.measure(target_bag, selected_enemy, 100)
        fresh_rate = 2.0 * first_hundred.win_rate - first_fifty.win_rate
        selected_measure = Measurement(fresh_rate, first_hundred.seconds)
        starter_measure = Measurement(0.0, 0.0)
        lucky_measure = Measurement(0.0, 0.0)
        enemy_lc = power.lc(selected_enemy, self.power)
        self.results.append(StageResult(
            number,
            stage["kind"],
            level,
            silver,
            len(target_bag["items"]),
            player_lc,
            curve_point.average_tier,
            curve_point.spent.get("items", 0),
            curve_point.spent.get("materials", 0),
            curve_point.spent.get("upgrades", 0),
            curve_point.spent.get("refresh", 0),
            curve_point.materials,
            enemy_lc,
            len(selected_enemy["items"]),
            selected_measure,
            starter_measure,
            lucky_measure,
            selected_target,
            selected_label,
            correction_steps,
            tune_rate,
            fresh_rate,
        ))
        print(
            f"Ải {number:02d} {stage['kind']} cấp {level}: LC {player_lc}/{enemy_lc}, "
            f"dò {' -> '.join(f'{value:.1f}%' for value in measurements)}, "
            f"xác nhận fresh {selected_measure.win_rate:.1f}%, "
            f"{selected_measure.seconds:.1f}s",
            flush=True,
        )

    def measure_existing_stage(self, number: int, stage: dict[str, Any]) -> None:
        """Measure an untouched early stage on fresh seeds."""
        player, _starter, _lucky, level, silver = self.progression.bags_for_stage(number)
        point = self.progression.point_for_stage(number)
        first_fifty = self.measure(player, stage["enemy"], 50)
        first_hundred = self.measure(player, stage["enemy"], 100)
        fresh_rate = 2.0 * first_hundred.win_rate - first_fifty.win_rate
        self.existing_rows.append({
            "stage": number,
            "kind": stage["kind"],
            "level": level,
            "unspentSilver": silver,
            "expectedItems": len(player["items"]),
            "expectedLc": power.lc(player, self.power),
            "averageTier": round(point.average_tier, 2),
            "spentItems": point.spent.get("items", 0),
            "spentMaterials": point.spent.get("materials", 0),
            "spentUpgrades": point.spent.get("upgrades", 0),
            "spentRefresh": point.spent.get("refresh", 0),
            "materials": point.materials,
            "enemyLc": power.lc(stage["enemy"], self.power),
            "freshSeeds": fresh_rate,
            "seconds": first_hundred.seconds,
        })
        print(f"Ải {number:02d} giữ nguyên: fresh {fresh_rate:.1f}%, {first_hundred.seconds:.1f}s", flush=True)

    def run(self, selected_numbers: list[int]) -> None:
        """Build stages and persist after each completed chapter."""
        number = 0
        for chapter_index, chapter in enumerate(self.document["chapters"]):
            touched = False
            for stage in chapter["stages"]:
                number += 1
                if number not in selected_numbers:
                    continue
                touched = True
                if self.args.measure_existing:
                    self.measure_existing_stage(number, stage)
                else:
                    self.build_stage(number, stage)
            if touched and not self.args.measure_existing:
                write_json(self.args.out, self.document)
                print(f"Đã lưu hết Hồi {chapter_index + 1}: {self.args.out}", flush=True)
        if self.args.measure_existing:
            path = self.args.out.parent / "tables" / "existing-results.json"
            write_json(path, self.existing_rows)
        else:
            self.write_results()

    def write_results(self) -> None:
        """Write machine-readable report rows and echo them verbatim."""
        rows = []
        print("\nFINAL_TABLE")
        for result in self.results:
            low, high = STAGE_ONE_BAND if result.number == 1 else BANDS[result.kind]
            row = {
                "stage": result.number,
                "kind": result.kind,
                "level": result.level,
                "silver": result.silver,
                "unspentSilver": result.silver,
                "expectedItems": result.expected_items,
                "expectedLc": result.expected_lc,
                "averageTier": round(result.average_tier, 2),
                "spentItems": result.spent_items,
                "spentMaterials": result.spent_materials,
                "spentUpgrades": result.spent_upgrades,
                "spentRefresh": result.spent_refresh,
                "materials": result.materials,
                "recommendedPower": result.expected_lc,
                "enemyLc": result.enemy_lc,
                "enemyItems": result.enemy_items,
                "targetLc": round(result.target_lc, 2),
                "expected": result.expected.win_rate,
                "tuneSeeds": result.tune_rate,
                "freshSeeds": result.fresh_rate,
                "starterOnly": result.starter_only.win_rate,
                "lucky": result.lucky.win_rate,
                "seconds": result.expected.seconds,
                "adjustment": result.adjustment,
                "correctionSteps": result.correction_steps,
                "status": "trong_dai" if low <= result.expected.win_rate <= high else "ngoai_dai",
            }
            rows.append(row)
            print(json.dumps(row, ensure_ascii=False))
        write_json(self.args.out.parent / "tables" / "results.json", rows)


def build_parser() -> argparse.ArgumentParser:
    """Build the command-line interface."""
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--stages", default="15-50", help="inclusive stage range")
    parser.add_argument("--runs", type=int, default=50, help="runs per final verification")
    parser.add_argument("--out", type=Path, default=DEFAULT_OUT, help="output chapters JSON")
    parser.add_argument("--measure-existing", action="store_true", help="measure without rebuilding")
    return parser


def main() -> int:
    """Run the LC builder."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    args = build_parser().parse_args()
    if args.runs < 1:
        raise ValueError("--runs must be positive")
    args.out = args.out.resolve()
    original_stdout = sys.stdout
    tee = TeeOutput(original_stdout, args.out.parent / "tables" / "pve-lc.txt")
    sys.stdout = tee
    try:
        builder = LcBuilder(args)
        try:
            maximum = sum(len(chapter["stages"]) for chapter in builder.document["chapters"])
            builder.run(pve_progression.parse_stage_range(args.stages, maximum))
        finally:
            builder.close()
    finally:
        sys.stdout = original_stdout
        tee.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
