#!/usr/bin/env python3
"""Construct PvE enemies as mirrors of expected player bags (decision #50)."""

from __future__ import annotations

import argparse
import copy
import json
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
import pve_progression  # noqa: E402

DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
SOURCE_CHAPTERS = DATA / "Pve" / "chapters.json"
ENGINE = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data"
ENGINE_ITEMS = ENGINE / "items.json"
ENGINE_SECTS = ENGINE / "sects.json"
DEFAULT_OUT = ROOT / "tools" / "codex" / "out" / "pve-mirror" / "chapters.json"
RARITIES = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
LOADOUT_KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")
RESULT_RE = re.compile(
    r"A thắng ([0-9.]+)%\s+\|\s+B thắng ([0-9.]+)%\s+\|\s+hòa ([0-9.]+)%.*?TB ([0-9.]+)s",
    re.DOTALL,
)


@dataclass
class Measurement:
    """One engine measurement; draws count as half a player win."""

    win_rate: float
    seconds: float


@dataclass
class StageResult:
    """All report fields for one completed stage."""

    number: int
    kind: str
    player_level: int
    player_items: int
    player_weapons: int
    enemy_items: int
    enemy_weapons: int
    adjustment: str
    no_shop: Measurement
    shop_smart: Measurement | None = None


class TeeOutput:
    """Copy console output verbatim to the output tables directory."""

    def __init__(self, console: Any, path: Path) -> None:
        path.parent.mkdir(parents=True, exist_ok=True)
        self.console = console
        self.file = path.open("w", encoding="utf-8", newline="\n")

    def write(self, value: str) -> int:
        """Write the same text to both destinations."""
        self.console.write(value)
        self.file.write(value)
        return len(value)

    def flush(self) -> None:
        """Flush both destinations."""
        self.console.flush()
        self.file.flush()

    def close(self) -> None:
        """Close only the owned file stream."""
        self.file.close()


def read_json(path: Path) -> Any:
    """Read one UTF-8 JSON document."""
    with path.open(encoding="utf-8") as stream:
        return json.load(stream)


def write_json(path: Path, value: Any) -> None:
    """Write stable human-readable JSON, as used by pve_tune."""
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="\n") as stream:
        json.dump(value, stream, ensure_ascii=False, indent=2)
        stream.write("\n")


def stage_band(kind: str) -> tuple[float, float]:
    """Return the decision-#50 player-win band."""
    if kind == "boss":
        return 40.0, 55.0
    if kind == "elite":
        return 55.0, 70.0
    return 65.0, 85.0


class MirrorBuilder:
    """Construct, measure, correct, and save mirror enemies."""

    def __init__(self, args: argparse.Namespace) -> None:
        self.args = args
        self.classpath = balance.ensure_classpath()
        self.document = read_json(SOURCE_CHAPTERS)
        self.progression = pve_progression.Progression()
        catalog = read_json(ENGINE_ITEMS)
        self.catalog = {item["id"]: item for item in catalog}
        self.results: list[StageResult] = []
        self.pending_shop: dict[int, dict[str, Any]] = {}
        self.temp_dir = tempfile.TemporaryDirectory(prefix="pve-mirror-")
        self.temp_path = Path(self.temp_dir.name)

    def close(self) -> None:
        """Remove temporary battle loadouts."""
        self.temp_dir.cleanup()

    def _is_weapon(self, item: dict[str, Any]) -> bool:
        return self.catalog[item["id"]].get("type") == "weapon"

    def _weapon_count(self, items: list[dict[str, Any]]) -> int:
        return sum(self._is_weapon(item) for item in items)

    def _score(self, item: dict[str, Any]) -> tuple[int, int, int]:
        return self.progression._score(item)

    @staticmethod
    def _distance(measurement: Measurement, band: tuple[float, float]) -> float:
        if measurement.win_rate < band[0]:
            return band[0] - measurement.win_rate
        if measurement.win_rate > band[1]:
            return measurement.win_rate - band[1]
        return 0.0

    def measure(self, player: dict[str, Any], enemy: dict[str, Any]) -> Measurement:
        """Measure one matchup with the pre-built engine, as used by pve_tune."""
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
            "--runs", str(self.args.runs), "--items", str(ENGINE_ITEMS), "--sects", str(ENGINE_SECTS),
        ]
        completed = subprocess.run(
            command, check=True, capture_output=True, text=True, encoding="utf-8", errors="replace",
        )
        match = RESULT_RE.search(completed.stdout)
        if match is None:
            raise RuntimeError(f"Cannot parse battle output:\n{completed.stdout}")
        wins = float(match.group(1)) + float(match.group(3)) / 2.0
        return Measurement(wins, float(match.group(4)))

    def _ranked_non_weapons(
        self, enemy: dict[str, Any], reverse: bool,
    ) -> list[dict[str, Any]]:
        candidates = [item for item in enemy["items"] if not self._is_weapon(item)]
        return sorted(candidates, key=lambda item: (self._score(item), item["id"]), reverse=reverse)

    def _remove_lowest(self, enemy: dict[str, Any], count: int) -> list[str]:
        removed = self._ranked_non_weapons(enemy, reverse=False)[:count]
        removed_ids = [item["id"] for item in removed]
        enemy["items"] = [item for item in enemy["items"] if item not in removed]
        return removed_ids

    @staticmethod
    def _change_rarity(item: dict[str, Any], delta: int) -> bool:
        old_index = RARITIES.index(item["rarity"])
        new_index = min(max(old_index + delta, 0), len(RARITIES) - 1)
        item["rarity"] = RARITIES[new_index]
        return old_index != new_index

    def _lower_highest(self, enemy: dict[str, Any], count: int) -> list[str]:
        candidates = self._ranked_non_weapons(enemy, reverse=True)
        if not candidates:
            candidates = [item for item in enemy["items"] if self._is_weapon(item)]
            candidates.sort(key=lambda item: (self._score(item), item["id"]), reverse=True)
            count = min(count, 1)
        changed = []
        for item in candidates[:count]:
            if self._change_rarity(item, -1):
                changed.append(item["id"])
        return changed

    def _raise_highest_non_weapon(self, enemy: dict[str, Any]) -> list[str]:
        candidates = self._ranked_non_weapons(enemy, reverse=True)
        if not candidates:
            return []
        return [candidates[0]["id"]] if self._change_rarity(candidates[0], 1) else []

    def _copy_enemy(
        self, stage: dict[str, Any], player: dict[str, Any], player_level: int,
    ) -> dict[str, Any]:
        enemy = copy.deepcopy(stage["enemy"])
        enemy["level"] = player_level + int(stage["kind"] == "boss")
        enemy["items"] = copy.deepcopy(player["items"])
        return enemy

    def _initial_variant(
        self, stage: dict[str, Any], player: dict[str, Any], level: int,
    ) -> tuple[dict[str, Any], str]:
        enemy = self._copy_enemy(stage, player, level)
        kind = stage["kind"]
        if kind == "normal":
            removed = self._remove_lowest(enemy, 1)
            return enemy, f"bỏ 1: {', '.join(removed)}"
        if kind == "elite":
            lowered = self._lower_highest(enemy, 2)
            return enemy, f"hạ phẩm 2: {', '.join(lowered)}"
        return enemy, "bản sao chính xác"

    def _corrected_variant(
        self, stage: dict[str, Any], player: dict[str, Any], level: int, too_easy: bool,
    ) -> tuple[dict[str, Any], str]:
        enemy = self._copy_enemy(stage, player, level)
        kind = stage["kind"]
        if kind == "normal":
            count = 0 if too_easy else 2
            removed = self._remove_lowest(enemy, count)
            return enemy, f"hiệu chỉnh bỏ {count}: {', '.join(removed) or 'không'}"
        if kind == "elite":
            count = 1 if too_easy else 3
            lowered = self._lower_highest(enemy, count)
            if not too_easy and len(lowered) < count:
                removed = self._remove_lowest(enemy, 1)
                return enemy, (
                    f"hiệu chỉnh hạ phẩm {count}: {', '.join(lowered)}; "
                    f"bỏ 1: {', '.join(removed)}"
                )
            return enemy, f"hiệu chỉnh hạ phẩm {count}: {', '.join(lowered)}"
        if too_easy:
            raised = self._raise_highest_non_weapon(enemy)
            return enemy, f"hiệu chỉnh tăng phẩm 1: {', '.join(raised)}"
        lowered = self._lower_highest(enemy, 1)
        return enemy, f"hiệu chỉnh hạ phẩm 1: {', '.join(lowered)}"

    def _choose(
        self, first_enemy: dict[str, Any], first_measure: Measurement,
        second_enemy: dict[str, Any], second_measure: Measurement, band: tuple[float, float],
    ) -> tuple[dict[str, Any], Measurement, bool]:
        first_key = (self._distance(first_measure, band), abs(first_measure.win_rate - sum(band) / 2))
        second_key = (self._distance(second_measure, band), abs(second_measure.win_rate - sum(band) / 2))
        if second_key < first_key:
            return second_enemy, second_measure, True
        return first_enemy, first_measure, False

    def build_stage(self, number: int, stage: dict[str, Any]) -> None:
        """Build and measure one stage, applying at most one correction pass."""
        no_shop, shop_smart, level, _silver = self.progression.bags_for_stage(number)
        stage["recommendedLevel"] = level
        first_enemy, first_label = self._initial_variant(stage, no_shop, level)
        band = stage_band(stage["kind"])
        first_measure = self.measure(no_shop, first_enemy)
        selected_enemy = first_enemy
        selected_measure = first_measure
        adjustment = first_label
        correction_rate = None
        if self._distance(first_measure, band) > 0:
            too_easy = first_measure.win_rate > band[1]
            second_enemy, second_label = self._corrected_variant(stage, no_shop, level, too_easy)
            second_measure = self.measure(no_shop, second_enemy)
            correction_rate = second_measure.win_rate
            selected_enemy, selected_measure, used_second = self._choose(
                first_enemy, first_measure, second_enemy, second_measure, band,
            )
            if used_second:
                adjustment = second_label
        stage["enemy"] = selected_enemy
        self.pending_shop[number] = shop_smart
        result = StageResult(
            number, stage["kind"], level, len(no_shop["items"]), self._weapon_count(no_shop["items"]),
            len(selected_enemy["items"]), self._weapon_count(selected_enemy["items"]), adjustment,
            selected_measure,
        )
        self.results.append(result)
        correction_text = "" if correction_rate is None else f" -> sửa {correction_rate:.1f}%"
        print(
            f"Ải {number:02d} {stage['kind']} cấp {level}: {first_measure.win_rate:.1f}%"
            f"{correction_text}; chọn {selected_measure.win_rate:.1f}% · {selected_measure.seconds:.1f}s",
            flush=True,
        )

    def run(self, selected_numbers: list[int]) -> None:
        """Build selected stages, save after every chapter, then measure shop-smart."""
        number = 0
        for chapter_index, chapter in enumerate(self.document["chapters"]):
            touched = False
            for stage in chapter["stages"]:
                number += 1
                if number not in selected_numbers:
                    continue
                touched = True
                self.build_stage(number, stage)
            if touched:
                write_json(self.args.out, self.document)
                print(f"Đã lưu hết Hồi {chapter_index + 1}: {self.args.out}", flush=True)
        stages = [stage for chapter in self.document["chapters"] for stage in chapter["stages"]]
        for result in self.results:
            result.shop_smart = self.measure(
                self.pending_shop[result.number], stages[result.number - 1]["enemy"],
            )
        self.print_table()

    def print_table(self) -> None:
        """Print stable JSON rows for the report and verbatim capture."""
        print("\nFINAL_TABLE")
        for result in self.results:
            low, high = stage_band(result.kind)
            record = {
                "stage": result.number, "kind": result.kind, "playerLevel": result.player_level,
                "playerItems": result.player_items, "playerWeapons": result.player_weapons,
                "enemyItems": result.enemy_items, "enemyWeapons": result.enemy_weapons,
                "adjustment": result.adjustment, "noShop": result.no_shop.win_rate,
                "shopSmart": result.shop_smart.win_rate if result.shop_smart else 0.0,
                "seconds": result.no_shop.seconds,
                "status": "trong_dai" if low <= result.no_shop.win_rate <= high else "ngoai_dai",
            }
            print(json.dumps(record, ensure_ascii=False))


def build_parser() -> argparse.ArgumentParser:
    """Build the decision-#50 command-line interface."""
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--stages", default="1-30", help="inclusive stage range")
    parser.add_argument("--runs", type=int, default=50, help="runs per measurement")
    parser.add_argument("--out", type=Path, default=DEFAULT_OUT, help="output chapters JSON")
    return parser


def main() -> int:
    """Run the mirror builder."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    args = build_parser().parse_args()
    if args.runs < 1:
        raise ValueError("--runs must be positive")
    args.out = args.out.resolve()
    original_stdout = sys.stdout
    tee = TeeOutput(original_stdout, args.out.parent / "tables" / "mirror.txt")
    sys.stdout = tee
    try:
        builder = MirrorBuilder(args)
        try:
            builder.run(pve_progression.parse_stage_range(args.stages, 30))
        finally:
            builder.close()
    finally:
        sys.stdout = original_stdout
        tee.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
