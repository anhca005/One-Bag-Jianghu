#!/usr/bin/env python3
"""Tune the themed PvE candidate directly against decision #55 win bands."""

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
SOURCE = DATA / "Pve" / "chapters.json"
OUT = ROOT / "tools" / "codex" / "out" / "pve-variety" / "chapters.json"
ENGINE_DATA = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data"
TABLE = OUT.parent / "tables" / "round2-tuning.json"
RARITIES = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
LOADOUT_KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")
RESULT_RE = re.compile(r"A thắng ([0-9.]+)%.*?hòa ([0-9.]+)%", re.DOTALL)


@dataclass(frozen=True)
class Step:
    """A whole-bag tier and rarity setting."""

    tier: int
    rarity: str
    strength: float


def read_json(path: Path) -> Any:
    """Read a UTF-8 JSON document."""
    return json.loads(path.read_text(encoding="utf-8"))


def write_json(path: Path, value: Any) -> None:
    """Write stable UTF-8 JSON."""
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def band(kind: str) -> tuple[float, float, float]:
    """Return low, high, and target player win rates from decision #55."""
    target = 50.0 if kind == "boss" else 62.0 if kind == "elite" else 72.0
    return target - 12.0, target + 12.0, target


class Tuner:
    """Tune enemy item strength while retaining the Round-1 themes."""

    def __init__(self, args: argparse.Namespace) -> None:
        self.args = args
        self.document = read_json(OUT)
        self.original = read_json(SOURCE)
        self.progression = pve_progression.Progression()
        self.classpath = balance.ensure_classpath()
        catalog_list = read_json(DATA / "items.json")
        self.catalog = {item["id"]: item for item in catalog_list}
        config = balance.load_battle_config(self.classpath, None)
        self.steps = sorted(
            (
                Step(tier_index + 1, RARITIES[rarity_index], tier * rarity / 1_000_000)
                for tier_index, tier in enumerate(config["tierMultiplierPermille"])
                for rarity_index, rarity in enumerate(config["rarityMultiplierPermille"])
            ),
            key=lambda step: (step.strength, step.tier, RARITIES.index(step.rarity)),
        )
        self.temp_dir = tempfile.TemporaryDirectory(prefix="pve-variety-round2-")
        self.temp = Path(self.temp_dir.name)
        self.rows: list[dict[str, Any]] = []

    def close(self) -> None:
        """Remove temporary battle inputs."""
        self.temp_dir.cleanup()

    def measure(self, player: dict[str, Any], enemy: dict[str, Any], runs: int) -> float:
        """Measure player wins, counting a draw as half a win."""
        paths = []
        for filename, loadout in (("player.json", player), ("enemy.json", enemy)):
            path = self.temp / filename
            prepared = {key: loadout[key] for key in LOADOUT_KEYS if key in loadout}
            prepared.setdefault("cols", 5)
            prepared.setdefault("rows", 4)
            write_json(path, prepared)
            paths.append(path)
        command = [
            "java",
            "-cp",
            self.classpath,
            balance.MAIN_CLASS,
            str(paths[0]),
            str(paths[1]),
            "--runs",
            str(runs),
            "--items",
            str(ENGINE_DATA / "items.json"),
            "--sects",
            str(ENGINE_DATA / "sects.json"),
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
        return float(match.group(1)) + float(match.group(2)) / 2.0

    def apply_step(self, enemy: dict[str, Any], index: int) -> dict[str, Any]:
        """Apply one uniform strength step without changing item identities."""
        result = copy.deepcopy(enemy)
        step = self.steps[index]
        for item in result["items"]:
            item["tier"] = step.tier
            item["rarity"] = step.rarity
        return result

    def pack(self, items: list[dict[str, Any]]) -> list[dict[str, Any]]:
        """Repack placed items into the standard 5 by 4 bag."""
        packed: list[dict[str, Any]] = []
        ordered = sorted(
            items,
            key=lambda item: -self.catalog[item["id"]]["size"][0] * self.catalog[item["id"]]["size"][1],
        )
        for source in ordered:
            item = {key: source[key] for key in ("id", "tier", "rarity")}
            if not self.progression._place(packed, item):
                raise ValueError(f"Cannot pack {item['id']}")
        return packed

    def cap_weapons(self, enemy: dict[str, Any]) -> None:
        """Keep no more than two copies of one themed weapon per enemy."""
        counts: dict[str, int] = {}
        kept = []
        for item in enemy["items"]:
            is_weapon = self.catalog[item["id"]].get("type") == "weapon"
            counts[item["id"]] = counts.get(item["id"], 0) + int(is_weapon)
            if is_weapon and counts[item["id"]] > 2:
                continue
            kept.append(item)
        enemy["items"] = self.pack(kept)

    def mirror_with_themed_weapon(
        self,
        player: dict[str, Any],
        themed_enemy: dict[str, Any],
        kind: str,
    ) -> dict[str, Any]:
        """Mirror the expected bag but retain the Round-1 themed weapon identity."""
        weapon_ids = [
            item["id"]
            for item in themed_enemy["items"]
            if self.catalog[item["id"]].get("type") == "weapon"
        ]
        if not weapon_ids:
            raise ValueError(f"No themed weapon for {themed_enemy['name']}")
        themed_id = weapon_ids[0]
        items = []
        themed_count = 0
        for source in player["items"]:
            item = copy.deepcopy(source)
            if self.catalog[item["id"]].get("type") == "weapon":
                if themed_count >= 2:
                    continue
                item["id"] = themed_id
                themed_count += 1
            items.append(item)
        if themed_count == 0:
            items.append({"id": themed_id, "tier": 1, "rarity": "common"})
        enemy = copy.deepcopy(themed_enemy)
        while True:
            try:
                enemy["items"] = self.pack(items)
                break
            except ValueError:
                support = [
                    index
                    for index, item in enumerate(items)
                    if self.catalog[item["id"]].get("type") != "weapon"
                ]
                if not support:
                    raise
                items.pop(support[-1])
        return enemy

    @staticmethod
    def distance(rate: float, limits: tuple[float, float, float]) -> tuple[float, float]:
        """Rank a rate first by band distance and then target distance."""
        low, high, target = limits
        outside = low - rate if rate < low else rate - high if rate > high else 0.0
        return outside, abs(rate - target)

    def search(self, player: dict[str, Any], enemy: dict[str, Any], limits: tuple[float, float, float]) -> tuple[int, float]:
        """Binary-search whole-bag strength with approximately 20-run probes."""
        low = 0
        high = len(self.steps) - 1
        seen = {
            0: self.measure(player, self.apply_step(enemy, 0), self.args.probe_runs),
            len(self.steps) - 1: self.measure(
                player,
                self.apply_step(enemy, len(self.steps) - 1),
                self.args.probe_runs,
            ),
        }
        while low <= high and len(seen) < 8:
            index = (low + high) // 2
            rate = self.measure(player, self.apply_step(enemy, index), self.args.probe_runs)
            seen[index] = rate
            if rate > limits[1]:
                low = index + 1
            elif rate < limits[0]:
                high = index - 1
            else:
                break
        index = min(
            seen,
            key=lambda candidate: (self.distance(seen[candidate], limits), candidate),
        )
        return index, seen[index]

    def remove_support(self, enemy: dict[str, Any]) -> bool:
        """Remove one support item, preferring the smallest footprint."""
        choices = [
            (self.catalog[item["id"]]["size"][0] * self.catalog[item["id"]]["size"][1], index)
            for index, item in enumerate(enemy["items"])
            if self.catalog[item["id"]].get("type") != "weapon"
        ]
        if not choices:
            return False
        enemy["items"].pop(min(choices)[1])
        enemy["items"] = self.pack(enemy["items"])
        return True

    def confirm_nearby(
        self,
        player: dict[str, Any],
        enemy: dict[str, Any],
        index: int,
        limits: tuple[float, float, float],
    ) -> tuple[int, float]:
        """Confirm the selected step and up to two neighboring steps at 50 runs."""
        candidates = {}
        for candidate in range(max(0, index - 2), min(len(self.steps), index + 3)):
            rate = self.measure(player, self.apply_step(enemy, candidate), self.args.confirm_runs)
            candidates[candidate] = rate
            if self.distance(rate, limits)[0] == 0.0:
                break
        selected = min(candidates, key=lambda candidate: self.distance(candidates[candidate], limits))
        return selected, candidates[selected]

    def tune_stage(self, number: int, stage: dict[str, Any]) -> None:
        """Tune one stage and store its confirmed result."""
        player, _starter, _lucky, _level, _silver = self.progression.bags_for_stage(number)
        enemy = self.mirror_with_themed_weapon(player, stage["enemy"], stage["kind"])
        self.cap_weapons(enemy)
        limits = band(stage["kind"])
        removed = 0
        index, probe = self.search(player, enemy, limits)
        while index == 0 and probe < limits[0] and self.remove_support(enemy):
            removed += 1
            index, probe = self.search(player, enemy, limits)
        index, confirmed = self.confirm_nearby(player, enemy, index, limits)
        stage["enemy"]["items"] = self.apply_step(enemy, index)["items"]
        self.rows.append({
            "stage": number,
            "kind": stage["kind"],
            "target": limits[2],
            "band": [limits[0], limits[1]],
            "probe": probe,
            "confirmed": confirmed,
            "tier": self.steps[index].tier,
            "rarity": self.steps[index].rarity,
            "removedSupport": removed,
            "status": "trong_dai" if limits[0] <= confirmed <= limits[1] else "ngoai_dai",
        })
        print(
            f"Ải {number:02d}: dò {probe:.1f}%, xác nhận {confirmed:.1f}%, "
            f"T{self.steps[index].tier} {self.steps[index].rarity}, bỏ {removed}",
            flush=True,
        )

    def run(self) -> None:
        """Tune stages 2 through 50 and save the candidate and measurements."""
        number = 0
        for chapter in self.document["chapters"]:
            for stage in chapter["stages"]:
                number += 1
                if number < self.args.start_stage:
                    continue
                self.tune_stage(number, stage)
            write_json(OUT, self.document)
        write_json(TABLE, self.rows)


def main() -> int:
    """Run Round 2 tuning."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--probe-runs", type=int, default=20)
    parser.add_argument("--confirm-runs", type=int, default=50)
    parser.add_argument("--start-stage", type=int, default=2)
    args = parser.parse_args()
    if not 1 <= args.probe_runs <= 50 or not 1 <= args.confirm_runs <= 50:
        raise ValueError("Run counts must be between 1 and 50")
    tuner = Tuner(args)
    try:
        tuner.run()
    finally:
        tuner.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
