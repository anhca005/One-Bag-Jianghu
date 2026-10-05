#!/usr/bin/env python3
"""Re-theme PvE enemy bags while preserving their measured strength."""

from __future__ import annotations

import copy
import json
import sys
from collections import Counter
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import power  # noqa: E402

DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
SOURCE = DATA / "Pve" / "chapters.json"
ITEMS = DATA / "items.json"
POWER = DATA / "power.json"
OUT = ROOT / "tools" / "codex" / "out" / "pve-variety"
RARITIES = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]

WEAPONS = [
    "thanh_phong_kiem",
    "phuc_ho_con",
    "kich_doc_chu_nga",
    "liet_diem_dao",
    "tu_mau_song_dao",
    "that_tinh_tieu",
    "thiet_cot_quyen_sao",
    "liet_diem_dao",
    "thanh_phong_kiem",
    "liet_diem_dao",
    "kich_doc_chu_nga",
    "that_tinh_tieu",
    "kich_doc_chu_nga",
    "liet_diem_dao",
    "that_tinh_tieu",
    "u_minh_quy_trao",
    "that_tinh_tieu",
    "ngoc_cot_phien",
    "thanh_dang_tien",
    "xich_sa_chuong",
    "bang_phach_thuong",
    "thanh_phong_kiem",
    "u_minh_quy_trao",
    "phuc_ho_con",
    "tu_mau_song_dao",
    "u_minh_quy_trao",
    "that_tinh_tieu",
    "chan_nhac_chuy",
    "tu_mau_song_dao",
    "u_minh_quy_trao",
    "liet_diem_dao",
    "bang_phach_thuong",
    "bach_ho_cuong_mong",
    "ngoc_cot_phien",
    "that_tinh_tieu",
    "khai_son_phu",
    "thiet_cot_quyen_sao",
    "truy_hon_cung",
    "thanh_dang_tien",
    "u_minh_quy_trao",
    "bang_phach_thuong",
    "that_tinh_tieu",
    "chan_nhac_chuy",
    "phan_quan_but",
    "ngoc_cot_phien",
    "u_minh_quy_trao",
    "tu_kim_trao",
    "bich_thuy_cam",
    "truy_hon_cung",
    "chan_nhac_chuy",
]

THEME_SUPPORT = {
    "bandit": ["da_ho_y", "thiet_giap_y", "cuong_hoa_dan", "tuy_tien_ho_lo"],
    "beast": ["heo_rung_bap_giap", "tuyet_lang_bach_bi", "ho_mach_dan", "huyet_ngoc_thach"],
    "cold": ["bang_tam_giap", "han_bang_chau", "bang_tam_linh_chau", "thanh_tam_dan"],
    "court": ["kim_tuyen_y", "nho_sam_y", "bach_ngoc_boi", "ngung_than_dan"],
    "machine": ["o_thiet_giap", "thanh_dong_ho_tam_kinh", "tu_kim_bat", "cuong_hoa_dan"],
    "monk": ["phuc_ma_giap", "thien_tam_chuoi", "tu_kim_bat", "vo_niem_kinh"],
    "poison": ["da_ho_y", "giai_doc_dan", "chu_sa_ho_lo", "van_doc_dinh", "man_xa_doc_tui"],
    "shadow": ["da_ho_y", "u_minh_bi_luc", "huyet_ngoc_thach", "van_khi_dan"],
}

THEMES = [
    "bandit", "bandit", "beast", "bandit", "bandit", "shadow", "beast", "bandit", "bandit", "bandit",
    "poison", "poison", "poison", "poison", "poison", "beast", "shadow", "poison", "beast", "poison",
    "cold", "cold", "beast", "monk", "cold", "beast", "shadow", "beast", "shadow", "shadow",
    "bandit", "court", "beast", "bandit", "shadow", "machine", "beast", "court", "shadow", "shadow",
    "court", "shadow", "machine", "court", "court", "shadow", "beast", "court", "machine", "machine",
]


def read_json(path: Path) -> Any:
    """Read a UTF-8 JSON file."""
    return json.loads(path.read_text(encoding="utf-8"))


def write_json(path: Path, value: Any) -> None:
    """Write stable UTF-8 JSON."""
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def tune_lc(enemy: dict[str, Any], target: int, power_data: dict[str, Any]) -> None:
    """Greedily choose tier and rarity steps closest to the old LC."""
    while True:
        current = power.lc(enemy, power_data)
        candidates = []
        for index, item in enumerate(enemy["items"]):
            rarity_index = RARITIES.index(item["rarity"])
            for field, value, low, high in (
                ("tier", int(item["tier"]), 1, 9),
                ("rarity", rarity_index, 0, len(RARITIES) - 1),
            ):
                for delta in (-1, 1):
                    changed_value = value + delta
                    if changed_value < low or changed_value > high:
                        continue
                    candidate = copy.deepcopy(enemy)
                    changed = candidate["items"][index]
                    changed[field] = RARITIES[changed_value] if field == "rarity" else changed_value
                    candidate_lc = power.lc(candidate, power_data)
                    candidates.append((abs(candidate_lc - target), index, field, changed_value, candidate_lc))
        if not candidates:
            return
        best = min(candidates)
        if best[0] >= abs(current - target):
            return
        item = enemy["items"][best[1]]
        item[best[2]] = RARITIES[best[3]] if best[2] == "rarity" else best[3]


def repack(items: list[dict[str, Any]], catalog: dict[str, dict[str, Any]]) -> None:
    """Pack a bag deterministically into its 5x4 grid."""
    while sum(
        catalog[item["id"]]["size"][0] * catalog[item["id"]]["size"][1]
        for item in items
    ) > 20:
        removable = [
            (catalog[item["id"]]["size"][0] * catalog[item["id"]]["size"][1], index)
            for index, item in enumerate(items)
            if catalog[item["id"]].get("type") != "weapon"
        ]
        if not removable:
            raise ValueError("Oversized bag has no removable support item")
        items.pop(min(removable)[1])
    items.sort(
        key=lambda item: (
            -catalog[item["id"]]["size"][0] * catalog[item["id"]]["size"][1],
            item["id"],
        ),
    )

    def place(index: int, occupied: set[tuple[int, int]]) -> bool:
        if index == len(items):
            return True
        item = items[index]
        width, height = catalog[item["id"]]["size"]
        for y in range(4 - height + 1):
            for x in range(5 - width + 1):
                cells = {(x + dx, y + dy) for dx in range(width) for dy in range(height)}
                if not cells.isdisjoint(occupied):
                    continue
                item["x"] = x
                item["y"] = y
                if place(index + 1, occupied | cells):
                    return True
        return False

    if not place(0, set()):
        raise ValueError("Cannot pack stage bag")


def same_size_replacement(
    item_id: str,
    theme: str,
    stage_number: int,
    catalog: dict[str, dict[str, Any]],
) -> str:
    """Select a themed support item with the exact same footprint."""
    old_item = catalog[item_id]
    if old_item.get("type") == "weapon":
        return WEAPONS[stage_number - 1]
    choices = []
    for candidate_id in THEME_SUPPORT[theme]:
        candidate = catalog[candidate_id]
        if candidate["size"] == old_item["size"] and candidate.get("type") != "weapon":
            choices.append(candidate_id)
    if not choices:
        return item_id
    return choices[(stage_number + len(item_id)) % len(choices)]


def build() -> tuple[dict[str, Any], list[dict[str, Any]]]:
    """Build themed bags and return per-stage LC rows."""
    document = read_json(SOURCE)
    power_data = read_json(POWER)
    catalog = {item["id"]: item for item in read_json(ITEMS)}
    rows = []
    stage_number = 0
    for chapter in document["chapters"]:
        for stage in chapter["stages"]:
            stage_number += 1
            old_enemy = copy.deepcopy(stage["enemy"])
            old_lc = power.lc(old_enemy, power_data)
            if stage_number == 1:
                rows.append({"stage": 1, "oldLc": old_lc, "newLc": old_lc})
                continue
            theme = THEMES[stage_number - 1]
            for item in stage["enemy"]["items"]:
                old_catalog_item = catalog[item["id"]]
                if old_catalog_item.get("type") == "weapon":
                    item["id"] = WEAPONS[stage_number - 1]
            repack(stage["enemy"]["items"], catalog)
            tune_lc(stage["enemy"], old_lc, power_data)
            while power.lc(stage["enemy"], power_data) > old_lc * 1.05:
                candidates = []
                for index, item in enumerate(stage["enemy"]["items"]):
                    if catalog[item["id"]].get("type") == "weapon":
                        continue
                    candidate = copy.deepcopy(stage["enemy"])
                    candidate["items"].pop(index)
                    candidate_lc = power.lc(candidate, power_data)
                    candidates.append((abs(candidate_lc - old_lc), index))
                if not candidates:
                    break
                stage["enemy"]["items"].pop(min(candidates)[1])
                repack(stage["enemy"]["items"], catalog)
                tune_lc(stage["enemy"], old_lc, power_data)
            new_lc = power.lc(stage["enemy"], power_data)
            rows.append({"stage": stage_number, "oldLc": old_lc, "newLc": new_lc})
    return document, rows


def validate(document: dict[str, Any], rows: list[dict[str, Any]]) -> None:
    """Validate LC, weapon variety, stage one, and unchanged non-item data."""
    source = read_json(SOURCE)
    catalog = {item["id"]: item for item in read_json(ITEMS)}
    for row in rows:
        if abs(row["newLc"] / row["oldLc"] - 1.0) > 0.05:
            raise ValueError(f"Stage {row['stage']} LC is outside 5%: {row}")
    source_without_items = copy.deepcopy(source)
    result_without_items = copy.deepcopy(document)
    for left_chapter, right_chapter in zip(
        source_without_items["chapters"],
        result_without_items["chapters"],
    ):
        for left_stage, right_stage in zip(left_chapter["stages"], right_chapter["stages"]):
            left_stage["enemy"].pop("items")
            right_stage["enemy"].pop("items")
    if source_without_items != result_without_items:
        raise ValueError("A field other than enemy.items changed")
    if source["chapters"][0]["stages"][0] != document["chapters"][0]["stages"][0]:
        raise ValueError("Stage 1 changed")
    previous = None
    for chapter_index, chapter in enumerate(document["chapters"], 1):
        counts: Counter[str] = Counter()
        for stage in chapter["stages"]:
            weapon_ids = {
                item["id"]
                for item in stage["enemy"]["items"]
                if catalog[item["id"]].get("type") == "weapon"
            }
            counts.update(weapon_ids)
            current = next(iter(weapon_ids))
            if current == previous:
                raise ValueError(f"Consecutive stages use {current}")
            previous = current
        if max(counts.values()) > 3:
            raise ValueError(f"Chapter {chapter_index} repeats a weapon too often: {counts}")
        allowed_sword = 2 if chapter_index == 1 else 1
        if counts["thanh_phong_kiem"] > allowed_sword:
            raise ValueError(f"Chapter {chapter_index} has too many Thanh Phong Kiếm")


def main() -> int:
    """Generate and validate the candidate chapters file."""
    document, rows = build()
    validate(document, rows)
    write_json(OUT / "chapters.json", document)
    write_json(OUT / "tables" / "lc.json", rows)
    print(f"Wrote {OUT / 'chapters.json'}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
