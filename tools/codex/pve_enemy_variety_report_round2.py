#!/usr/bin/env python3
"""Write the final Vietnamese Round-2 enemy-variety report."""

from __future__ import annotations

import json
from collections import Counter
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
CANDIDATE = ROOT / "tools" / "codex" / "out" / "pve-variety" / "chapters.json"
VERIFY = CANDIDATE.parent / "tables" / "verification.json"
REPORT = ROOT / "docs" / "reviews" / "2026-10-05-codex-pve-enemy-variety.md"


def read_json(path: Path) -> Any:
    """Read UTF-8 JSON."""
    return json.loads(path.read_text(encoding="utf-8"))


def limits(kind: str) -> tuple[float, float, float]:
    """Return the Round-2 low, high, and target rates."""
    target = 50.0 if kind == "boss" else 62.0 if kind == "elite" else 72.0
    return target - 12.0, target + 12.0, target


def main() -> int:
    """Build the report from the independent 50-run verification."""
    candidate = read_json(CANDIDATE)
    verification = read_json(VERIFY)
    catalog = {item["id"]: item for item in read_json(DATA / "items.json")}
    stages = [stage for chapter in candidate["chapters"] for stage in chapter["stages"]]
    rows = verification["rows"]
    passed = []
    for row, stage in zip(rows, stages):
        low, high, target = limits(stage["kind"])
        row["target"] = target
        row["round2Pass"] = row["stage"] == 1 or low <= row["newWinRate"] <= high
        if row["stage"] > 1 and row["round2Pass"]:
            passed.append(row)
    failures = [row for row in rows if row["stage"] > 1 and not row["round2Pass"]]
    biggest_lc = max(rows[1:], key=lambda row: abs(row["newLc"] - row["oldLc"]))
    biggest_rate = max(rows[1:], key=lambda row: abs(row["winRateChange"]))
    lines = [
        "# Rà soát độ đa dạng trang bị địch PvE",
        "",
        "## Round 1 — giữ LC",
        "",
        "Round 1 giữ được LC của 50/50 ải trong ±5%, nhưng chỉ 10/50 ải giữ được tỉ lệ thắng cũ trong ±10 điểm. "
        "Kết quả cho thấy LC không dự báo đúng khi đổi lớp vũ khí; điển hình ải 4 đổi sang đao, LC 203 → 210 "
        "nhưng tỉ lệ thắng 92% → 0%. Vì vậy bản Round 1 không được áp dụng.",
        "",
        "## Round 2 — dò trực tiếp theo tỉ lệ thắng",
        "",
        f"Kết quả xác nhận 50 trận: **{len(passed)}/49 ải chỉnh sửa nằm trong dải**. "
        "Điều kiện áp dụng cần ít nhất 45/49, nên **không áp dụng** ứng viên vào dữ liệu game và không tạo "
        "`chapters-before.json`. Tất cả luật cứng về vũ khí, vị trí túi, ải 1 và trường dữ liệu không phải "
        "`enemy.items` đều đạt.",
        "",
        "Dải mục tiêu: thường 72% (60–84%), tinh anh 62% (50–74%), Thủ Lĩnh 50% (38–62%). "
        "`recommendedPower` được giữ nguyên.",
        "",
        "### Theo từng ải",
        "",
        "| Ải | Địch | Vũ khí cũ → mới | LC cũ → mới | Mục tiêu | Thắng cũ → mới | Kết quả |",
        "|---:|---|---|---:|---:|---:|---|",
    ]
    for row in rows:
        old_weapons = ", ".join(catalog[item_id]["name"] for item_id in row["oldWeapons"])
        new_weapons = ", ".join(catalog[item_id]["name"] for item_id in row["newWeapons"])
        result = "miễn" if row["stage"] == 1 else "đạt" if row["round2Pass"] else "không đạt"
        lines.append(
            f"| {row['stage']} | {row['enemy']} | {old_weapons} → {new_weapons} | "
            f"{row['oldLc']} → {row['newLc']} | {row['target']:.0f}% | "
            f"{row['oldWinRate']:.1f}% → {row['newWinRate']:.1f}% | {result} |"
        )
    lines.extend(["", "### Số lần dùng vũ khí theo hồi", ""])
    for chapter in candidate["chapters"]:
        counts: Counter[str] = Counter()
        for stage in chapter["stages"]:
            weapon_ids = {
                item["id"]
                for item in stage["enemy"]["items"]
                if catalog[item["id"]].get("type") == "weapon"
            }
            counts.update(weapon_ids)
        summary = "; ".join(f"{catalog[item_id]['name']}: {count}" for item_id, count in counts.items())
        lines.append(f"- {chapter['id']}: {summary}.")
    lines.extend(["", "### Các ải ngoài dải", ""])
    for row in failures:
        low, high, _target = limits(stages[row["stage"] - 1]["kind"])
        direction = "quá dễ" if row["newWinRate"] > high else "quá khó"
        lines.append(
            f"- Ải {row['stage']}: {row['newWinRate']:.1f}% (dải {low:.0f}–{high:.0f}%), {direction}; "
            "các nấc phẩm/Trọng và thay đổi từng món vẫn nhảy qua dải hoặc không đổi kết quả."
        )
    lines.extend([
        "",
        "### Biến động lớn nhất",
        "",
        f"- LC: ải {biggest_lc['stage']}, {biggest_lc['oldLc']} → {biggest_lc['newLc']} "
        f"({biggest_lc['newLc'] - biggest_lc['oldLc']:+d}). LC chỉ được báo cáo, không dùng làm ràng buộc.",
        f"- Tỉ lệ thắng: ải {biggest_rate['stage']}, {biggest_rate['oldWinRate']:.1f}% → "
        f"{biggest_rate['newWinRate']:.1f}% ({biggest_rate['winRateChange']:+.1f} điểm).",
        "",
        "### Tệp",
        "",
        "- Ứng viên chưa áp dụng: `tools/codex/out/pve-variety/chapters.json`.",
        "- Số đo xác nhận: `tools/codex/out/pve-variety/tables/verification.json`.",
        "- Script dò/tinh chỉnh Round 2: `tools/codex/pve_enemy_variety_round2.py`, "
        "`tools/codex/pve_enemy_variety_refine.py`.",
        "- Dữ liệu chính `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`: không đổi.",
        "- `tools/codex/out/pve-variety/chapters-before.json`: không tạo vì chưa đạt 45/49.",
        "",
    ])
    REPORT.write_text("\n".join(lines), encoding="utf-8")
    print(f"Wrote {REPORT}; pass={len(passed)}/49; failures={len(failures)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
