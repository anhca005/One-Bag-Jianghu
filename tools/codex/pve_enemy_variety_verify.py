#!/usr/bin/env python3
"""Verify PvE enemy variety against the original stages."""

from __future__ import annotations

import json
import re
import subprocess
import sys
import tempfile
from collections import Counter
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance  # noqa: E402
import power  # noqa: E402
import pve_progression  # noqa: E402

DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
SOURCE = DATA / "Pve" / "chapters.json"
CANDIDATE = ROOT / "tools" / "codex" / "out" / "pve-variety" / "chapters.json"
RESULTS = ROOT / "tools" / "codex" / "out" / "pve-variety" / "tables" / "verification.json"
REPORT = ROOT / "docs" / "reviews" / "2026-10-05-codex-pve-enemy-variety.md"
POWER = DATA / "power.json"
ENGINE_DATA = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data"
RESULT_RE = re.compile(r"A thắng ([0-9.]+)%.*?hòa ([0-9.]+)%", re.DOTALL)
RARITIES = {"common", "uncommon", "rare", "epic", "legendary", "supreme"}
LOADOUT_KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")


def read_json(path: Path) -> Any:
    """Read a UTF-8 JSON file."""
    return json.loads(path.read_text(encoding="utf-8"))


def write_json(path: Path, value: Any) -> None:
    """Write a stable UTF-8 JSON file."""
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def stages(document: dict[str, Any]) -> list[dict[str, Any]]:
    """Flatten the chapter stage lists."""
    return [stage for chapter in document["chapters"] for stage in chapter["stages"]]


def weapon_ids(enemy: dict[str, Any], catalog: dict[str, dict[str, Any]]) -> list[str]:
    """Return all weapon IDs in an enemy bag."""
    return [
        item["id"]
        for item in enemy["items"]
        if catalog[item["id"]].get("type") == "weapon"
    ]


def validate_bag(enemy: dict[str, Any], catalog: dict[str, dict[str, Any]]) -> None:
    """Validate IDs, rarities, bounds, and non-overlapping placements."""
    occupied: set[tuple[int, int]] = set()
    for item in enemy["items"]:
        if item["id"] not in catalog:
            raise ValueError(f"Unknown item: {item['id']}")
        if item["rarity"] not in RARITIES:
            raise ValueError(f"Unknown rarity: {item['rarity']}")
        width, height = catalog[item["id"]]["size"]
        cells = {
            (item["x"] + dx, item["y"] + dy)
            for dx in range(width)
            for dy in range(height)
        }
        if any(x < 0 or x >= 5 or y < 0 or y >= 4 for x, y in cells):
            raise ValueError(f"Out-of-bounds item: {item}")
        if not cells.isdisjoint(occupied):
            raise ValueError(f"Overlapping item: {item}")
        occupied.update(cells)


def measure(
    player: dict[str, Any],
    enemy: dict[str, Any],
    classpath: str,
    temp_path: Path,
) -> float:
    """Measure the player win rate over exactly 50 engine runs."""
    paths = []
    for filename, loadout in (("player.json", player), ("enemy.json", enemy)):
        path = temp_path / filename
        prepared = {key: loadout[key] for key in LOADOUT_KEYS if key in loadout}
        prepared.setdefault("cols", 5)
        prepared.setdefault("rows", 4)
        write_json(path, prepared)
        paths.append(path)
    command = [
        "java",
        "-cp",
        classpath,
        balance.MAIN_CLASS,
        str(paths[0]),
        str(paths[1]),
        "--runs",
        "50",
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
        raise RuntimeError(f"Cannot parse battle result:\n{completed.stdout}")
    return float(match.group(1)) + float(match.group(2)) / 2.0


def write_report(
    rows: list[dict[str, Any]],
    chapter_counts: list[dict[str, Any]],
    catalog: dict[str, dict[str, Any]],
) -> None:
    """Write the Vietnamese verification report."""
    failed_lc = [row for row in rows if not row["lcPass"]]
    failed_rate = [row for row in rows if not row["winRatePass"]]
    biggest_lc = max(rows, key=lambda row: abs(row["lcChangePct"]))
    biggest_rate = max(rows, key=lambda row: abs(row["winRateChange"]))
    lines = [
        "# Rà soát độ đa dạng trang bị địch PvE",
        "",
        "## Kết luận",
        "",
        "Bản ứng viên **chưa đạt** điều kiện tỉ lệ thắng ±10 điểm, nên chưa chép vào dữ liệu game và chưa tạo bản sao lưu.",
        f"LC: {len(rows) - len(failed_lc)}/50 ải đạt ±5%. Tỉ lệ thắng: "
        f"{len(rows) - len(failed_rate)}/50 ải đạt ±10 điểm (50 trận mỗi phép đo).",
        "",
        "Nguyên nhân chính: battle engine rất nhạy với hành vi/lớp vũ khí nhưng LC không phản ánh đầy đủ. "
        "Ví dụ ải 4 đổi sang vũ khí lớp blade làm tỉ lệ thắng 92% → 0% trong khi LC chỉ đổi 203 → 210.",
        "",
        "## Theo từng ải",
        "",
        "| Ải | Địch | Vũ khí cũ → mới | LC cũ → mới | Thắng cũ → mới | Kết quả |",
        "|---:|---|---|---:|---:|---|",
    ]
    for row in rows:
        old_names = ", ".join(catalog[item_id]["name"] for item_id in row["oldWeapons"])
        new_names = ", ".join(catalog[item_id]["name"] for item_id in row["newWeapons"])
        status = "đạt" if row["lcPass"] and row["winRatePass"] else "không đạt"
        lines.append(
            f"| {row['stage']} | {row['enemy']} | {old_names} → {new_names} | "
            f"{row['oldLc']} → {row['newLc']} | {row['oldWinRate']:.1f}% → "
            f"{row['newWinRate']:.1f}% | {status} |"
        )
    lines.extend(["", "## Số lần dùng vũ khí theo hồi", ""])
    for entry in chapter_counts:
        names = [
            f"{catalog[item_id]['name']}: {count}"
            for item_id, count in entry["weapons"].items()
        ]
        lines.append(f"- {entry['chapter']}: " + "; ".join(names) + ".")
    lines.extend([
        "",
        "## Biến động lớn nhất",
        "",
        f"- LC: ải {biggest_lc['stage']}, {biggest_lc['oldLc']} → {biggest_lc['newLc']} "
        f"({biggest_lc['lcChangePct']:+.2f}%).",
        f"- Tỉ lệ thắng: ải {biggest_rate['stage']}, {biggest_rate['oldWinRate']:.1f}% → "
        f"{biggest_rate['newWinRate']:.1f}% ({biggest_rate['winRateChange']:+.1f} điểm).",
        "",
        "## Tệp",
        "",
        "- Ứng viên: `tools/codex/out/pve-variety/chapters.json`.",
        "- Số đo: `tools/codex/out/pve-variety/tables/verification.json`.",
        "- Dữ liệu chính `Resources/Data/Pve/chapters.json`: không đổi.",
        "- `chapters-before.json`: chưa tạo vì điều kiện kiểm tra chưa đạt.",
        "",
    ])
    REPORT.write_text("\n".join(lines), encoding="utf-8")


def main() -> int:
    """Run structural, LC, variety, and battle checks."""
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    source = read_json(SOURCE)
    candidate = read_json(CANDIDATE)
    power_data = read_json(POWER)
    catalog_list = read_json(DATA / "items.json")
    catalog = {item["id"]: item for item in catalog_list}
    old_stages = stages(source)
    new_stages = stages(candidate)
    progression = pve_progression.Progression()
    classpath = balance.ensure_classpath()
    rows = []
    with tempfile.TemporaryDirectory(prefix="pve-variety-") as raw_temp:
        temp_path = Path(raw_temp)
        for number, (old_stage, new_stage) in enumerate(zip(old_stages, new_stages), 1):
            validate_bag(new_stage["enemy"], catalog)
            player, _starter, _lucky, _level, _silver = progression.bags_for_stage(number)
            old_rate = measure(player, old_stage["enemy"], classpath, temp_path)
            new_rate = measure(player, new_stage["enemy"], classpath, temp_path)
            old_lc = power.lc(old_stage["enemy"], power_data)
            new_lc = power.lc(new_stage["enemy"], power_data)
            row = {
                "stage": number,
                "enemy": old_stage["enemy"]["name"],
                "oldWeapons": weapon_ids(old_stage["enemy"], catalog),
                "newWeapons": weapon_ids(new_stage["enemy"], catalog),
                "oldLc": old_lc,
                "newLc": new_lc,
                "lcChangePct": round(100.0 * (new_lc / old_lc - 1.0), 2),
                "oldWinRate": old_rate,
                "newWinRate": new_rate,
                "winRateChange": round(new_rate - old_rate, 1),
            }
            row["lcPass"] = abs(row["lcChangePct"]) <= 5.0
            row["winRatePass"] = abs(row["winRateChange"]) <= 10.0
            rows.append(row)
            print(
                f"Ải {number:02d}: LC {old_lc}->{new_lc}; "
                f"thắng {old_rate:.1f}%->{new_rate:.1f}%",
                flush=True,
            )
    chapter_counts = []
    for chapter in candidate["chapters"]:
        counts: Counter[str] = Counter()
        for stage in chapter["stages"]:
            counts.update(set(weapon_ids(stage["enemy"], catalog)))
        chapter_counts.append({"chapter": chapter["id"], "weapons": dict(counts)})
    write_json(RESULTS, {"runs": 50, "rows": rows, "chapterCounts": chapter_counts})
    write_report(rows, chapter_counts, catalog)
    print(f"Wrote {RESULTS}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
