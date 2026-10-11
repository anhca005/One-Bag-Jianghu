#!/usr/bin/env python3
"""Build every PvE enemy kit from a Lực Chiến curve (decision #61, LC v4 additive).

Curve:
- normal stages follow the typical player bag's LC at that stage (Hồi 1 at 85%, later chapters at 100%), and never
  drop below the previous normal stage;
- an elite is a spike above the normal stage before it (+15% in Hồi 1, +25% later), a boss a bigger spike (+30% in
  Hồi 1, +55% later). Spikes do not carry over: the stage after them continues from the normal stage before them.

Kits:
- keep each stage's themed weapon(s) from the current data; add armor and support items that fit the enemy
  (beast, poison, fire, ice, monk, sword, iron, U Minh, generic);
- each chapter has a typical tier/rarity strength (like the player's bag at that point); the number of cells follows
  from the target, so late enemies fill the bag like the player does; tier and rarity then land the LC on target.

No simulation: the user tests by playing. Stage 1 is left exactly as it is.

Usage:
  py tools/balance/pve_lc_curve.py            # write the candidate and the report table
  py tools/balance/pve_lc_curve.py --apply    # also write enemy items + recommendedPower into the game file
"""
from __future__ import annotations

import argparse
import copy
import json
import sys
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import power  # noqa: E402

GAME_CHAPTERS = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data" / "Pve" / "chapters.json"
ITEMS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "items.json"
OUT = ROOT / "tools" / "balance" / "out" / "lc-v4"
POWER = OUT / "power.json"
PLAYER_CURVE = OUT / "player-curve.json"
COLS = 5
ROWS = 4
RARITIES = ["common", "uncommon", "rare", "epic", "legendary"]

NORMAL_SHARE = {1: 0.85}
ELITE_SPIKE = {1: 1.15}
BOSS_SPIKE = {1: 1.30}
DEFAULT_NORMAL_SHARE = 1.0
DEFAULT_ELITE_SPIKE = 1.25
DEFAULT_BOSS_SPIKE = 1.55

# Typical tier x rarity multiplier at the first and last stage of each chapter; elites and bosses carry more.
GRADE_RAMP = {1: (1.0, 1.15), 2: (1.15, 1.3), 3: (1.25, 1.45), 4: (1.35, 1.6), 5: (1.5, 1.8)}
ELITE_GRADE_BONUS = 0.15
BOSS_GRADE_BONUS = 0.25
MIN_CELLS = 5
# Highest rarity allowed per chapter for normal / elite+boss enemies.
RARITY_CAP = {1: ("uncommon", "rare"), 2: ("rare", "epic"), 3: ("rare", "epic"),
              4: ("epic", "legendary"), 5: ("epic", "legendary")}
# Highest Trọng per chapter for normal / elite+boss enemies, so strength comes from rarity before tier 9.
TIER_CAP = {1: (4, 5), 2: (5, 6), 3: (6, 7), 4: (7, 8), 5: (9, 9)}

BEAST_WORDS = ["Sói", "Trư", "Hùng", "Xà", "Tri Chu", "Lang", "Hổ", "Quy", "Miêu", "Ngưu", "Điểu"]
THEME_SUPPORT = {
    "beast": ["hac_sat_hung_dam", "man_xa_doc_tui", "bang_tam_linh_chau", "hoa_diem_xa_cot", "hoi_thien_dan"],
    "poison": ["van_doc_dinh", "man_xa_doc_tui", "giai_doc_dan", "ti_khang_dan", "huyet_ngoc_thach"],
    "fire": ["liet_hoa_tam_kinh", "cuong_hoa_dan", "chu_sa_ho_lo", "tram_huong_lo"],
    "ice": ["han_bang_chau", "bang_tam_linh_chau", "ngung_than_dan", "thanh_tam_dan"],
    "monk": ["thien_tam_chuoi", "thuan_duong_cong", "vo_niem_kinh", "ho_mach_dan"],
    "sword": ["thai_thanh_tam_kinh", "kiem_tong_tam_yeu", "am_duong_ngoc", "phi_yen_quyet"],
    "iron": ["bach_luyen_chan_kinh", "quy_nguyen_quyet", "van_khi_dan", "tu_kim_bat"],
    "uminh": ["u_minh_bi_luc", "huyet_ngoc_thach", "da_minh_chau", "cuu_chuyen_hoan_hon_dan"],
    "generic": ["kim_sang_duoc", "boi_nguyen_dan", "bach_ngoc_boi", "tuy_tien_ho_lo", "duong_sinh_quyet"],
}
THEME_ARMOR = {
    "beast": ["tuyet_lang_bach_bi", "heo_rung_bap_giap", "thien_nien_quy_giap"],
    "poison": ["da_ho_y", "tue_nguyet_bao_y", "nho_sam_y"],
    "fire": ["thiet_giap_y", "kim_tuyen_y", "o_tam_y"],
    "ice": ["bang_tam_giap", "long_lan_giap", "nho_sam_y"],
    "monk": ["phuc_ma_giap", "kim_tuyen_y", "thanh_dong_ho_tam_kinh"],
    "sword": ["o_tam_y", "tue_nguyet_bao_y", "thanh_dong_ho_tam_kinh"],
    "iron": ["o_thiet_giap", "thiet_giap_y", "long_lan_giap"],
    "uminh": ["da_ho_y", "long_lan_giap", "phuc_ma_giap"],
    "generic": ["o_tam_y", "thiet_giap_y", "nho_sam_y"],
}
SECT_THEME = {"huyen_doc": "poison", "bich_thuy": "poison", "liet_diem": "fire", "han_son": "ice",
              "kim_cang": "monk", "thai_thanh": "sword", "thiet_phu": "iron"}


def read_json(path: Path) -> Any:
    with path.open(encoding="utf-8") as handle:
        return json.load(handle)


def theme_of(enemy: dict[str, Any]) -> str:
    name = enemy["name"]
    if enemy.get("portrait", "").startswith("monster_") or any(word in name for word in BEAST_WORDS):
        return "beast"
    if "U Minh" in name:
        return "uminh"
    return SECT_THEME.get(enemy.get("sect") or "", "generic")


def level_points(catalog: dict[str, Any], level: int) -> float:
    return catalog["levelBase"] + catalog["levelPoints"] * (max(1, level) - 1)


def targets(stages: list[dict[str, Any]], player: list[dict[str, Any]]) -> list[int]:
    """Target enemy LC per stage, following the curve rules in the module docstring."""
    result = []
    last_normal = 0.0
    for index, stage in enumerate(stages):
        chapter = index // 10 + 1
        if stage["kind"] == "normal":
            share = NORMAL_SHARE.get(chapter, DEFAULT_NORMAL_SHARE)
            value = max(player[index]["lc"] * share, last_normal)
            last_normal = value
        elif stage["kind"] == "elite":
            value = last_normal * ELITE_SPIKE.get(chapter, DEFAULT_ELITE_SPIKE)
        else:
            value = last_normal * BOSS_SPIKE.get(chapter, DEFAULT_BOSS_SPIKE)
        result.append(round(value))
    return result


def wanted_cells(index: int, kind: str, budget: float, points_per_cell: float) -> int:
    """Cells needed to reach the item budget at this point's typical tier/rarity strength."""
    chapter = index // 10 + 1
    first, last = GRADE_RAMP[chapter]
    grade = first + (last - first) * (index % 10) / 9.0
    if kind == "elite":
        grade += ELITE_GRADE_BONUS
    if kind == "boss":
        grade += BOSS_GRADE_BONUS
    cells = round(budget / (points_per_cell * grade))
    return max(MIN_CELLS, min(cells, COLS * ROWS))


class KitBuilder:
    def __init__(self, catalog: dict[str, Any], items: dict[str, dict[str, Any]]) -> None:
        self.catalog = catalog
        self.items = items
        self.previous_armor = ""

    def cells(self, item_id: str) -> int:
        width, height = self.items[item_id]["size"]
        return width * height

    def choose(self, enemy: dict[str, Any], kind: str, cells: int) -> list[str]:
        """Pick item ids: themed weapon(s), armor, then support items until the cell budget is used."""
        theme = theme_of(enemy)
        weapons = list(dict.fromkeys(
            item["id"] for item in enemy["items"] if self.items[item["id"]]["type"] == "weapon"
        ))
        chosen = [weapons[0]]
        if kind == "boss" or cells >= 14:
            chosen.append(weapons[1] if len(weapons) > 1 else weapons[0])
        armors = [armor for armor in THEME_ARMOR[theme] if armor != self.previous_armor]
        chosen.append(armors[0])
        self.previous_armor = armors[0]
        if cells >= 17:
            chosen.append(armors[1] if len(armors) > 1 else armors[0])
        supports = THEME_SUPPORT[theme] + THEME_SUPPORT["generic"]
        used = sum(self.cells(item_id) for item_id in chosen)
        chosen.append(supports[0])
        used += self.cells(supports[0])
        for support in supports[1:]:
            if used >= cells:
                break
            if chosen.count(support) >= 2 or used + self.cells(support) > cells:
                continue
            chosen.append(support)
            used += self.cells(support)
        return chosen

    def place(self, ids: list[str]) -> list[dict[str, Any]]:
        """Pack the largest items first on the 5x4 grid; items that do not fit are dropped."""
        occupied: set[tuple[int, int]] = set()
        placed = []
        for item_id in sorted(ids, key=self.cells, reverse=True):
            width, height = self.items[item_id]["size"]
            spot = self._free_spot(occupied, width, height)
            if spot is None:
                continue
            x, y = spot
            for dx in range(width):
                for dy in range(height):
                    occupied.add((x + dx, y + dy))
            placed.append({"id": item_id, "tier": 1, "rarity": "common", "x": x, "y": y})
        return placed

    @staticmethod
    def _free_spot(occupied: set[tuple[int, int]], width: int, height: int) -> tuple[int, int] | None:
        for y in range(ROWS - height + 1):
            for x in range(COLS - width + 1):
                cells = {(x + dx, y + dy) for dx in range(width) for dy in range(height)}
                if not cells & occupied:
                    return x, y
        return None

    def grade(self, placed: list[dict[str, Any]], budget: float, rarity_cap: str, tier_cap: int) -> None:
        """Set tier and rarity on each item so the kit's item points come close to the budget."""
        base = sum(self.catalog["items"][item["id"]] for item in placed)
        wanted = budget / base if base else 1.0
        allowed = RARITIES[:RARITIES.index(rarity_cap) + 1]
        carry = 0.0
        for item in placed:
            points = self.catalog["items"][item["id"]]
            goal = wanted + carry / points
            rarity, tier = self._closest(goal, allowed, tier_cap)
            item["rarity"] = rarity
            item["tier"] = tier
            got = self.catalog["tierFactor"][str(tier)] * self.catalog["rarityFactor"][rarity]
            carry += (wanted - got) * points

    def _closest(self, goal: float, allowed: list[str], tier_cap: int) -> tuple[str, int]:
        best = ("common", 1)
        best_error = float("inf")
        for rarity in allowed:
            for tier in range(1, tier_cap + 1):
                value = self.catalog["tierFactor"][str(tier)] * self.catalog["rarityFactor"][rarity]
                error = abs(value - goal)
                if error < best_error - 1e-9:
                    best = (rarity, tier)
                    best_error = error
        return best


def build(apply: bool) -> int:
    catalog = read_json(POWER)
    items = {item["id"]: item for item in read_json(ITEMS)}
    player = read_json(PLAYER_CURVE)
    raw = GAME_CHAPTERS.read_text(encoding="utf-8")
    newline = "\r\n" if "\r\n" in raw else "\n"
    document = json.loads(raw)
    stages = [stage for chapter in document["chapters"] for stage in chapter["stages"]]
    goals = targets(stages, player)
    builder = KitBuilder(catalog, items)
    rows = []
    for index, stage in enumerate(stages):
        enemy = stage["enemy"]
        chapter = index // 10 + 1
        if index == 0:
            rows.append((stage, goals[index], power.lc(enemy, catalog), "giữ nguyên"))
            continue
        budget = goals[index] - level_points(catalog, enemy["level"])
        cells = wanted_cells(index, stage["kind"], budget, catalog["pointsPerCell"])
        placed = builder.place(builder.choose(enemy, stage["kind"], cells))
        rank = 0 if stage["kind"] == "normal" else 1
        builder.grade(placed, budget, RARITY_CAP[chapter][rank], TIER_CAP[chapter][rank])
        enemy["items"] = placed
        stage["recommendedPower"] = goals[index]
        rows.append((stage, goals[index], power.lc(enemy, catalog), theme_of(enemy)))
    write_report(rows)
    candidate = OUT / "chapters.json"
    text = json.dumps(document, ensure_ascii=False, indent=2).replace("\n", newline) + newline
    candidate.write_text(text, encoding="utf-8", newline="")
    if apply:
        GAME_CHAPTERS.write_text(text, encoding="utf-8", newline="")
        print(f"applied to {GAME_CHAPTERS}")
    return 0


def write_report(rows: list[tuple[dict[str, Any], int, int, str]]) -> None:
    lines = ["| Ải | Loại | Địch | Chủ đề | LC mục tiêu | LC đạt | Món | Trọng | Phẩm |", "|---|---|---|---|---|---|---|---|---|"]
    for stage, goal, got, theme in rows:
        enemy = stage["enemy"]
        tiers = [item["tier"] for item in enemy["items"]]
        rarities = sorted({item["rarity"] for item in enemy["items"]}, key=RARITIES.index)
        lines.append(
            f"| {stage['id']} | {stage['kind']} | {enemy['name']} | {theme} | {goal} | {got} | "
            f"{len(enemy['items'])} | {min(tiers)}–{max(tiers)} | {', '.join(rarities)} |"
        )
    (OUT / "curve.md").write_text("\n".join(lines) + "\n", encoding="utf-8")
    for line in lines[2:]:
        print(line)


def main() -> int:
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--apply", action="store_true", help="write the result into the game chapters.json")
    args = parser.parse_args()
    return build(args.apply)


if __name__ == "__main__":
    raise SystemExit(main())
