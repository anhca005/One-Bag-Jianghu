#!/usr/bin/env python3
"""Tune growing PvE enemy bags for decision #49."""

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
DEFAULT_OUT = ROOT / "tools" / "codex" / "out" / "pve-tuner-v2" / "chapters.json"
RARITIES = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
LOADOUT_KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")
RESULT_RE = re.compile(
    r"A thắng ([0-9.]+)%\s+\|\s+B thắng ([0-9.]+)%\s+\|\s+hòa ([0-9.]+)%.*?TB ([0-9.]+)s",
    re.DOTALL,
)


@dataclass(frozen=True)
class LadderStep:
    """One whole-bag tier and rarity pair."""

    tier: int
    rarity: str
    strength: float


@dataclass
class Measurement:
    """One engine measurement; draws count as half a player win."""

    win_rate: float
    seconds: float


@dataclass
class StageResult:
    """Report data retained for a tuned stage."""

    number: int
    kind: str
    player_level: int
    player_items: int
    player_weapons: int
    enemy_items: int
    enemy_weapons: int
    ladder_index: int
    tier: int
    rarity: str
    added: list[str]
    no_shop: Measurement
    reason: str
    shop_smart: Measurement | None = None


def read_json(path: Path) -> Any:
    """Read a UTF-8 JSON document."""
    with path.open(encoding="utf-8") as stream:
        return json.load(stream)


def write_json(path: Path, value: Any) -> None:
    """Write stable human-readable JSON."""
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="\n") as stream:
        json.dump(value, stream, ensure_ascii=False, indent=2)
        stream.write("\n")


def stage_band(kind: str) -> tuple[float, float]:
    """Return the decision-#49 player-win band."""
    if kind == "boss":
        return 45.0, 55.0
    if kind == "elite":
        return 55.0, 65.0
    return 65.0, 80.0


class Tuner:
    """Build themed enemy bags and tune their power ladder."""

    def __init__(self, args: argparse.Namespace) -> None:
        self.args = args
        self.classpath = balance.ensure_classpath()
        self.document = read_json(SOURCE_CHAPTERS)
        self.original = copy.deepcopy(self.document)
        self.progression = pve_progression.Progression()
        catalog = read_json(ENGINE_ITEMS)
        self.catalog = {item["id"]: item for item in catalog}
        self.catalog_order = [item["id"] for item in catalog]
        self.ladder = self._load_ladder()
        self.results: list[StageResult] = []
        self.temp_dir = tempfile.TemporaryDirectory(prefix="pve-tuner-v2-")
        self.temp_path = Path(self.temp_dir.name)

    def close(self) -> None:
        """Remove temporary battle loadouts."""
        self.temp_dir.cleanup()

    def _load_ladder(self) -> list[LadderStep]:
        config = balance.load_battle_config(self.classpath, None)
        tiers = config["tierMultiplierPermille"]
        rarities = config["rarityMultiplierPermille"]
        steps = [
            LadderStep(tier_index + 1, RARITIES[rarity_index], tier * rarity / 1_000_000)
            for tier_index, tier in enumerate(tiers)
            for rarity_index, rarity in enumerate(rarities)
        ]
        return sorted(steps, key=lambda step: (step.strength, step.tier, RARITIES.index(step.rarity)))

    def _weapon_count(self, items: list[dict[str, Any]]) -> int:
        return sum(self.catalog[item["id"]].get("type") == "weapon" for item in items)

    def measure(self, player: dict[str, Any], enemy: dict[str, Any], runs: int) -> Measurement:
        """Measure one matchup with the pre-built engine."""
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
            command, check=True, capture_output=True, text=True, encoding="utf-8", errors="replace",
        )
        match = RESULT_RE.search(completed.stdout)
        if match is None:
            raise RuntimeError(f"Cannot parse battle output:\n{completed.stdout}")
        wins = float(match.group(1)) + float(match.group(3)) / 2.0
        return Measurement(wins, float(match.group(4)))

    def _chapter_ids(self, chapter_index: int) -> list[str]:
        chapter = self.original["chapters"][chapter_index]
        return [item["id"] for stage in chapter["stages"] for item in stage["enemy"]["items"]]

    def _candidate_ids(self, enemy: dict[str, Any], chapter_index: int, weapon: bool) -> list[str]:
        own = [item["id"] for item in enemy["items"]]
        preferred_types = {self.catalog[item_id].get("type") for item_id in own}
        general = sorted(
            self.catalog_order,
            key=lambda item_id: (self.catalog[item_id].get("type") not in preferred_types, item_id),
        )
        ordered = own + self._chapter_ids(chapter_index) + general
        result = []
        for item_id in ordered:
            is_weapon = self.catalog[item_id].get("type") == "weapon"
            if is_weapon == weapon and item_id not in result:
                result.append(item_id)
        return result

    def _pack(self, items: list[dict[str, Any]]) -> list[dict[str, Any]] | None:
        packed: list[dict[str, Any]] = []
        for source in items:
            candidate = {key: source[key] for key in ("id", "tier", "rarity")}
            if not self.progression._place(packed, candidate):
                return None
        return packed

    def _add_item(
        self, items: list[dict[str, Any]], candidates: list[str], step: LadderStep,
    ) -> tuple[list[dict[str, Any]], str] | None:
        for item_id in candidates:
            addition = {"id": item_id, "tier": step.tier, "rarity": step.rarity}
            packed = self._pack(items + [addition])
            if packed is not None:
                return packed, item_id
        return None

    def _build_enemy(
        self, stage: dict[str, Any], chapter_index: int, player: dict[str, Any], level: int,
    ) -> tuple[dict[str, Any], list[str]]:
        enemy = copy.deepcopy(stage["enemy"])
        enemy["level"] = level + int(stage["kind"] == "boss")
        enemy.setdefault("cols", 5)
        enemy.setdefault("rows", 4)
        target_items = len(player["items"]) + int(stage["kind"] in ("elite", "boss"))
        target_weapons = max(1, self._weapon_count(player["items"]))
        original = [{"id": item["id"], "tier": 1, "rarity": "common"} for item in enemy["items"]]
        packed = self._pack(original)
        if packed is None:
            raise ValueError(f"Original enemy bag does not fit: {enemy['name']}")
        added = []
        while self._weapon_count(packed) < target_weapons:
            choice = self._add_item(packed, self._candidate_ids(enemy, chapter_index, True), self.ladder[0])
            if choice is None:
                break
            packed, item_id = choice
            added.append(item_id)
        while len(packed) < target_items:
            choice = self._add_item(packed, self._candidate_ids(enemy, chapter_index, False), self.ladder[0])
            if choice is None:
                break
            packed, item_id = choice
            added.append(item_id)
        enemy["items"] = packed
        return enemy, added

    def _apply_step(self, enemy: dict[str, Any], ladder_index: int) -> dict[str, Any]:
        variant = copy.deepcopy(enemy)
        step = self.ladder[ladder_index]
        for item in variant["items"]:
            item["tier"] = step.tier
            item["rarity"] = step.rarity
        return variant

    def _remove_lowest_non_weapon(self, enemy: dict[str, Any]) -> str | None:
        candidates = [
            item for item in enemy["items"] if self.catalog[item["id"]].get("type") != "weapon"
        ]
        if not candidates:
            return None
        removed = min(
            candidates,
            key=lambda item: (
                item["tier"] * (RARITIES.index(item["rarity"]) + 1),
                item["id"],
            ),
        )
        enemy["items"].remove(removed)
        enemy["items"] = self._pack(enemy["items"])
        return removed["id"]

    @staticmethod
    def _distance(measurement: Measurement, band: tuple[float, float]) -> float:
        if measurement.win_rate < band[0]:
            return band[0] - measurement.win_rate
        if measurement.win_rate > band[1]:
            return measurement.win_rate - band[1]
        return 0.0

    def _best_index(
        self, candidates: dict[int, Measurement], band: tuple[float, float],
    ) -> int:
        midpoint = sum(band) / 2
        return min(
            candidates,
            key=lambda index: (
                self._distance(candidates[index], band),
                abs(candidates[index].win_rate - midpoint),
                index,
            ),
        )

    def _search(
        self, player: dict[str, Any], enemy: dict[str, Any], floor: int, band: tuple[float, float],
    ) -> tuple[int, Measurement]:
        low = floor
        high = len(self.ladder) - 1
        candidates: dict[int, Measurement] = {}
        for probe in range(1, 8):
            if low > high:
                break
            ladder_index = (low + high) // 2
            measurement = self.measure(player, self._apply_step(enemy, ladder_index), self.args.runs)
            candidates[ladder_index] = measurement
            step = self.ladder[ladder_index]
            print(
                f"  probe {probe}/7: bậc {ladder_index:02d} T{step.tier} {step.rarity}"
                f" -> {measurement.win_rate:5.1f}%",
                flush=True,
            )
            if measurement.win_rate > band[1]:
                low = ladder_index + 1
            elif measurement.win_rate < band[0]:
                high = ladder_index - 1
            else:
                break
        selected = self._best_index(candidates, band)
        return selected, candidates[selected]

    def _confirm(
        self, player: dict[str, Any], enemy: dict[str, Any], ladder_index: int,
        band: tuple[float, float], floor: int,
    ) -> tuple[int, Measurement]:
        confirmed = self.measure(player, self._apply_step(enemy, ladder_index), self.args.confirm_runs)
        print(f"  xác nhận: {confirmed.win_rate:5.1f}% · {confirmed.seconds:4.1f}s", flush=True)
        if self._distance(confirmed, band) <= 5.0:
            return ladder_index, confirmed
        direction = 1 if confirmed.win_rate > band[1] else -1
        adjacent = ladder_index + direction
        if adjacent < floor or adjacent >= len(self.ladder):
            return ladder_index, confirmed
        second = self.measure(player, self._apply_step(enemy, adjacent), self.args.confirm_runs)
        print(f"  xác nhận bậc kề {adjacent:02d}: {second.win_rate:5.1f}% · {second.seconds:4.1f}s", flush=True)
        if self._distance(second, band) <= self._distance(confirmed, band):
            return adjacent, second
        return ladder_index, confirmed

    def _constraint_floor(self, chapter_index: int, local_index: int) -> int:
        chapter_start = sum(len(chapter["stages"]) for chapter in self.document["chapters"][:chapter_index])
        earlier = self.results[chapter_start:chapter_start + local_index]
        normals = [result.ladder_index for result in earlier if result.kind == "normal"]
        return normals[-1] if normals else 0

    def tune_stage(self, number: int, chapter_index: int, local_index: int) -> None:
        stage = self.document["chapters"][chapter_index]["stages"][local_index]
        no_shop, shop_smart, level, _silver = self.progression.bags_for_stage(number)
        stage["recommendedLevel"] = level
        enemy, added = self._build_enemy(stage, chapter_index, no_shop, level)
        floor = self._constraint_floor(chapter_index, local_index)
        band = stage_band(stage["kind"])
        print(
            f"Ải {number:02d} {stage['kind']} · cấp {level} · người {len(no_shop['items'])} món/"
            f"{self._weapon_count(no_shop['items'])} binh khí · sàn bậc {floor}",
            flush=True,
        )
        ladder_index, _probe = self._search(no_shop, enemy, floor, band)
        if ladder_index == 0 and _probe.win_rate < band[0]:
            removed = self._remove_lowest_non_weapon(enemy)
            if removed is not None:
                added.append(f"bỏ {removed}")
                ladder_index, _probe = self._search(no_shop, enemy, floor, band)
        elif ladder_index == len(self.ladder) - 1 and _probe.win_rate > band[1]:
            choice = self._add_item(
                enemy["items"], self._candidate_ids(enemy, chapter_index, False), self.ladder[-1],
            )
            if choice is not None:
                enemy["items"], item_id = choice
                added.append(item_id)
                ladder_index, _probe = self._search(no_shop, enemy, floor, band)
        ladder_index, confirmed = self._confirm(no_shop, enemy, ladder_index, band, floor)
        selected = self._apply_step(enemy, ladder_index)
        stage["enemy"] = selected
        reason = ""
        if self._distance(confirmed, band) > 0:
            reason = "ràng buộc thứ tự" if ladder_index == floor and floor > 0 else "bước thang gần nhất"
        step = self.ladder[ladder_index]
        self.results.append(StageResult(
            number, stage["kind"], level, len(no_shop["items"]), self._weapon_count(no_shop["items"]),
            len(selected["items"]), self._weapon_count(selected["items"]), ladder_index, step.tier,
            step.rarity, added, confirmed, reason,
        ))
        self.pending_shop[number] = shop_smart

    def run(self, selected_numbers: list[int]) -> None:
        """Tune stages, save every chapter, then measure shop-smart."""
        self.pending_shop: dict[int, dict[str, Any]] = {}
        number = 0
        for chapter_index, chapter in enumerate(self.document["chapters"]):
            touched = False
            for local_index, _stage in enumerate(chapter["stages"]):
                number += 1
                if number not in selected_numbers:
                    continue
                touched = True
                self.tune_stage(number, chapter_index, local_index)
            if touched:
                write_json(self.args.out, self.document)
                print(f"Đã lưu hết Hồi {chapter_index + 1}: {self.args.out}", flush=True)
        print("Đo shop-smart 50 trận trên cấu hình đã chốt...", flush=True)
        stages = [stage for chapter in self.document["chapters"] for stage in chapter["stages"]]
        for result in self.results:
            result.shop_smart = self.measure(
                self.pending_shop[result.number], stages[result.number - 1]["enemy"], self.args.confirm_runs,
            )
        self.print_table()

    def print_table(self) -> None:
        """Print stable JSON rows for the report."""
        print("\nFINAL_TABLE")
        for result in self.results:
            low, high = stage_band(result.kind)
            record = {
                "stage": result.number, "kind": result.kind, "playerLevel": result.player_level,
                "playerItems": result.player_items, "playerWeapons": result.player_weapons,
                "enemyItems": result.enemy_items, "enemyWeapons": result.enemy_weapons,
                "ladder": result.ladder_index, "tier": result.tier, "rarity": result.rarity,
                "noShop": result.no_shop.win_rate,
                "shopSmart": result.shop_smart.win_rate if result.shop_smart else 0.0,
                "seconds": result.no_shop.seconds,
                "status": "trong_dai" if low <= result.no_shop.win_rate <= high else "ngoai_dai",
                "reason": result.reason, "added": result.added,
            }
            print(json.dumps(record, ensure_ascii=False))


def build_parser() -> argparse.ArgumentParser:
    """Build the v1-compatible CLI."""
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--stages", default="1-30", help="inclusive stage range")
    parser.add_argument("--runs", type=int, default=20, help="runs per search probe")
    parser.add_argument("--confirm-runs", type=int, default=50, help="runs per confirmation and shop measure")
    parser.add_argument("--out", type=Path, default=DEFAULT_OUT, help="output chapters JSON")
    return parser


def main() -> int:
    """Run the command-line tuner."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    args = build_parser().parse_args()
    if args.runs < 1 or args.confirm_runs < 1:
        raise ValueError("Run counts must be positive")
    args.out = args.out.resolve()
    tuner = Tuner(args)
    try:
        tuner.run(pve_progression.parse_stage_range(args.stages, 30))
    finally:
        tuner.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
