#!/usr/bin/env python3
"""Build, tune, validate, report, and conditionally apply plausible PvE kits."""

from __future__ import annotations

import argparse
import copy
import json
import re
import shutil
import subprocess
import sys
import tempfile
from collections import Counter
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance  # noqa: E402
import pve_progression  # noqa: E402

DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
SOURCE = DATA / "Pve" / "chapters.json"
ITEMS = DATA / "items.json"
ENGINE_DATA = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data"
OUT_DIR = ROOT / "tools" / "codex" / "out" / "pve-kits-r4"
CANDIDATE = OUT_DIR / "chapters.json"
BACKUP = OUT_DIR / "chapters-before.json"
RESULTS = OUT_DIR / "verification.json"
REPORT = ROOT / "docs" / "reviews" / "2026-10-10-codex-pve-kits-round4.md"
RARITIES = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
LOADOUT_KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")
RESULT_RE = re.compile(r"A thắng ([0-9.]+)%.*?hòa ([0-9.]+)%", re.DOTALL)
COUNT_RANGES = [(3, 6), (5, 9), (7, 12), (9, 15), (10, 18)]
TIER_RANGES = [(1, 2), (1, 3), (2, 4), (3, 6), (4, 7)]
RARITY_CAPS = [1, 2, 2, 3, 3]

WEAPONS = [
    "thanh_phong_kiem", "phuc_ho_con", "kich_doc_chu_nga", "liet_diem_dao",
    "tu_mau_song_dao", "that_tinh_tieu", "thiet_cot_quyen_sao", "liet_diem_dao",
    "thanh_phong_kiem", "liet_diem_dao", "kich_doc_chu_nga", "that_tinh_tieu",
    "kich_doc_chu_nga", "liet_diem_dao", "that_tinh_tieu", "u_minh_quy_trao",
    "that_tinh_tieu", "ngoc_cot_phien", "thanh_dang_tien", "xich_sa_chuong",
    "bang_phach_thuong", "thanh_phong_kiem", "u_minh_quy_trao", "phuc_ho_con",
    "tu_mau_song_dao", "bach_ho_cuong_mong", "that_tinh_tieu", "chan_nhac_chuy",
    "tu_mau_song_dao", "u_minh_quy_trao", "liet_diem_dao", "bang_phach_thuong",
    "bach_ho_cuong_mong", "xich_sa_chuong", "that_tinh_tieu", "khai_son_phu",
    "thiet_cot_quyen_sao", "truy_hon_cung", "thanh_dang_tien", "u_minh_quy_trao",
    "bang_phach_thuong", "that_tinh_tieu", "chan_nhac_chuy", "thanh_phong_kiem",
    "ngoc_cot_phien", "u_minh_quy_trao", "tu_kim_trao", "bich_thuy_cam",
    "truy_hon_cung", "chan_nhac_chuy",
]

THEMES = [
    "bandit", "bandit", "beast", "fire", "bandit", "shadow", "boar", "fire", "bandit", "bandit",
    "poison", "poison", "spider", "poison", "poison", "bear", "shadow", "poison", "snake", "poison",
    "cold", "cold", "wolf", "monk", "cold", "tiger", "shadow", "turtle", "shadow", "shadow",
    "bandit", "court", "cat", "court", "shadow", "machine", "beast", "court", "shadow", "shadow",
    "court", "fire", "machine", "scholar", "court", "shadow", "bird", "court", "machine", "machine",
]

ARMORS = {
    "bandit": ["da_ho_y", "thiet_giap_y", "o_tam_y"],
    "beast": ["tuyet_lang_bach_bi", "heo_rung_bap_giap", "da_ho_y"],
    "bird": ["tuyet_lang_bach_bi", "da_ho_y"],
    "boar": ["heo_rung_bap_giap", "da_ho_y"],
    "bear": ["heo_rung_bap_giap", "da_ho_y"],
    "cat": ["da_ho_y", "tuyet_lang_bach_bi"],
    "cold": ["bang_tam_giap", "tuyet_lang_bach_bi"],
    "court": ["kim_tuyen_y", "nho_sam_y", "thiet_giap_y"],
    "fire": ["thiet_giap_y", "da_ho_y"],
    "machine": ["o_thiet_giap", "thanh_dong_ho_tam_kinh"],
    "monk": ["phuc_ma_giap", "thien_nien_quy_giap"],
    "poison": ["da_ho_y", "thiet_giap_y"],
    "scholar": ["nho_sam_y", "tue_nguyet_bao_y"],
    "shadow": ["da_ho_y", "o_tam_y"],
    "snake": ["da_ho_y", "tuyet_lang_bach_bi"],
    "spider": ["da_ho_y", "tuyet_lang_bach_bi"],
    "tiger": ["tuyet_lang_bach_bi", "da_ho_y"],
    "turtle": ["thien_nien_quy_giap", "bang_tam_giap"],
    "wolf": ["tuyet_lang_bach_bi", "da_ho_y"],
}

SUPPORTS = {
    "bandit": ["kim_sang_duoc", "tuy_tien_ho_lo", "cuong_hoa_dan", "thuan_duong_cong"],
    "beast": ["ho_mach_dan", "huyet_ngoc_thach", "hac_sat_hung_dam", "duong_sinh_quyet"],
    "bird": ["phi_yen_quyet", "van_khi_dan", "huyet_ngoc_thach"],
    "boar": ["ho_mach_dan", "heo_rung_bap_giap", "cuong_hoa_dan"],
    "bear": ["hac_sat_hung_dam", "ho_mach_dan", "huyet_ngoc_thach"],
    "cat": ["phi_yen_quyet", "van_khi_dan", "huyet_ngoc_thach"],
    "cold": ["han_bang_chau", "bang_tam_linh_chau", "thanh_tam_dan", "quy_nguyen_quyet"],
    "court": ["bach_ngoc_boi", "ngung_than_dan", "thai_thanh_tam_kinh", "am_duong_ngoc"],
    "fire": ["cuong_hoa_dan", "liet_hoa_tam_kinh", "tuy_tien_ho_lo"],
    "machine": ["thanh_dong_ho_tam_kinh", "tu_kim_bat", "cuong_hoa_dan", "bach_luyen_chan_kinh"],
    "monk": ["thien_tam_chuoi", "vo_niem_kinh", "tu_kim_bat", "duong_sinh_quyet"],
    "poison": ["giai_doc_dan", "chu_sa_ho_lo", "van_doc_dinh", "man_xa_doc_tui"],
    "scholar": ["thai_thanh_tam_kinh", "bach_ngoc_boi", "ngung_than_dan", "quy_nguyen_quyet"],
    "shadow": ["u_minh_bi_luc", "huyet_ngoc_thach", "van_khi_dan", "da_minh_chau"],
    "snake": ["man_xa_doc_tui", "giai_doc_dan", "hoa_diem_xa_cot"],
    "spider": ["van_doc_dinh", "giai_doc_dan", "man_xa_doc_tui"],
    "tiger": ["huyet_ngoc_thach", "ho_mach_dan", "cuong_hoa_dan"],
    "turtle": ["bang_phach_co_cot", "ho_mach_dan", "quy_nguyen_quyet"],
    "wolf": ["tuyet_lang_bach_bi", "ho_mach_dan", "huyet_ngoc_thach"],
}

THEME_WEAPONS = {
    "bandit": ["phuc_ho_con", "liet_diem_dao", "tu_mau_song_dao", "khai_son_phu", "chan_nhac_chuy"],
    "beast": ["thiet_cot_quyen_sao", "kich_doc_chu_nga", "tu_kim_trao", "bach_ho_cuong_mong"],
    "bird": ["tu_kim_trao", "bach_ho_cuong_mong", "thiet_cot_quyen_sao"],
    "boar": ["thiet_cot_quyen_sao", "bach_ho_cuong_mong", "kich_doc_chu_nga"],
    "bear": ["u_minh_quy_trao", "bach_ho_cuong_mong", "thiet_cot_quyen_sao"],
    "cat": ["bach_ho_cuong_mong", "tu_kim_trao", "kich_doc_chu_nga"],
    "cold": ["bang_phach_thuong", "thanh_phong_kiem", "han_ngoc_tieu"],
    "court": ["bang_phach_thuong", "truy_hon_cung", "phan_quan_but", "ngoc_cot_phien"],
    "fire": ["liet_diem_dao", "tu_mau_song_dao", "xich_sa_chuong"],
    "machine": ["chan_nhac_chuy", "khai_son_phu", "truy_hon_cung"],
    "monk": ["phuc_ho_con", "xich_sa_chuong", "thiet_cot_quyen_sao"],
    "poison": ["that_tinh_tieu", "kich_doc_chu_nga", "xich_sa_chuong", "thanh_dang_tien"],
    "scholar": ["phan_quan_but", "ngoc_cot_phien", "thanh_phong_kiem"],
    "shadow": ["that_tinh_tieu", "u_minh_quy_trao", "tu_kim_trao", "thanh_dang_tien"],
    "snake": ["thanh_dang_tien", "kich_doc_chu_nga", "that_tinh_tieu"],
    "spider": ["kich_doc_chu_nga", "that_tinh_tieu", "u_minh_quy_trao"],
    "tiger": ["bach_ho_cuong_mong", "tu_kim_trao", "u_minh_quy_trao"],
    "turtle": ["chan_nhac_chuy", "xich_sa_chuong", "thiet_cot_quyen_sao"],
    "wolf": ["u_minh_quy_trao", "kich_doc_chu_nga", "bach_ho_cuong_mong"],
}


def read_json(path: Path) -> Any:
    """Read UTF-8 JSON."""
    return json.loads(path.read_text(encoding="utf-8"))


def write_json(path: Path, value: Any) -> None:
    """Write two-space JSON with CRLF."""
    path.parent.mkdir(parents=True, exist_ok=True)
    text = json.dumps(value, ensure_ascii=False, indent=2) + "\n"
    path.write_bytes(text.replace("\n", "\r\n").encode("utf-8"))


def target_band(chapter: int, kind: str) -> tuple[float, float, float]:
    """Return target, low, and high player win percentages."""
    if chapter == 1:
        target = 65.0 if kind == "boss" else 75.0 if kind == "elite" else 85.0
        low = 55.0 if kind == "boss" else 65.0 if kind == "elite" else 75.0
        high = 78.0 if kind == "boss" else 88.0 if kind == "elite" else 95.0
        return target, low, high
    target = 50.0 if kind == "boss" else 62.0 if kind == "elite" else 72.0
    return target, target - 12.0, target + 12.0


class Round4:
    """Construct and measure plausible enemy kits."""

    def __init__(self, probe_runs: int, confirm_runs: int) -> None:
        self.probe_runs = probe_runs
        self.confirm_runs = confirm_runs
        self.source = read_json(SOURCE)
        self.document = copy.deepcopy(self.source)
        self.catalog = {item["id"]: item for item in read_json(ITEMS)}
        self.progression = pve_progression.Progression()
        if "paidRefreshCosts" not in self.progression.shop:
            refresh_cost = self.progression.shop["paidRefreshCost"]
            self.progression.shop["paidRefreshCosts"] = [refresh_cost]
        self.classpath = balance.ensure_classpath()
        self.temp_dir = tempfile.TemporaryDirectory(prefix="pve-kits-r4-")
        self.temp = Path(self.temp_dir.name)
        self.rows: list[dict[str, Any]] = []
        self.weapon_counts: dict[int, Counter[str]] = {}
        self.previous_weapons: dict[int, set[str]] = {}

    def close(self) -> None:
        """Remove temporary battle inputs."""
        self.temp_dir.cleanup()

    def pack(self, items: list[dict[str, Any]]) -> list[dict[str, Any]]:
        """Pack items into a 5 by 4 enemy grid."""
        ordered = sorted(
            items,
            key=lambda item: -self.catalog[item["id"]]["size"][0] * self.catalog[item["id"]]["size"][1],
        )
        packed: list[dict[str, Any]] = []
        for source in ordered:
            item = {key: source[key] for key in ("id", "tier", "rarity")}
            if not self.progression._place(packed, item):
                raise ValueError(f"Không xếp được {item['id']}")
        return packed

    def item_count(self, chapter: int, local_index: int, kind: str) -> int:
        """Choose a steadily growing count inside the chapter range."""
        low, high = COUNT_RANGES[chapter - 1]
        count = low + round((high - low) * local_index / 9)
        if kind == "elite":
            count = max(count, high - 1)
        if kind == "boss":
            count = high
        return min(count, high)

    def identities(
        self,
        number: int,
        chapter: int,
        local_index: int,
        kind: str,
        count_override: int | None = None,
    ) -> list[str]:
        """Build themed weapon, armor, and support identities."""
        theme = THEMES[number - 1]
        count = count_override or self.item_count(chapter, local_index, kind)
        identities = [WEAPONS[number - 1]]
        identities.append(ARMORS[theme][local_index % len(ARMORS[theme])])
        identities.append(SUPPORTS[theme][0])
        weapon_target = min(4, count - 2)
        weapon_pool = [WEAPONS[number - 1]] + THEME_WEAPONS[theme]
        chapter_counts = self.weapon_counts.setdefault(chapter, Counter())
        previous = self.previous_weapons.get(chapter, set())
        next_primary = WEAPONS[number] if local_index < 9 else ""
        cursor = 0
        attempts = 0
        while sum(self.catalog[item_id]["type"] == "weapon" for item_id in identities) < weapon_target:
            attempts += 1
            if attempts > len(weapon_pool) * 4:
                break
            candidate = weapon_pool[cursor % len(weapon_pool)]
            cursor += 1
            if identities.count(candidate) >= 2:
                continue
            if candidate != WEAPONS[number - 1] and candidate in previous:
                continue
            if candidate != WEAPONS[number - 1] and candidate == next_primary:
                continue
            if candidate not in identities and chapter_counts[candidate] >= 3:
                continue
            identities.append(candidate)
        stage_weapons = {
            item_id
            for item_id in identities
            if self.catalog[item_id]["type"] == "weapon"
        }
        chapter_counts.update(stage_weapons)
        self.previous_weapons[chapter] = stage_weapons
        support = SUPPORTS[theme]
        cursor = 1
        while len(identities) < count:
            identities.append(support[cursor % len(support)])
            cursor += 1
        return identities

    def make_enemy(
        self,
        stage: dict[str, Any],
        identities: list[str],
        tier: int,
        rarity: str,
    ) -> dict[str, Any]:
        """Make an enemy variant while retaining non-item combat fields."""
        enemy = copy.deepcopy(stage["enemy"])
        raw = [{"id": item_id, "tier": tier, "rarity": rarity} for item_id in identities]
        enemy["items"] = self.pack(raw)
        return enemy

    def measure(self, player: dict[str, Any], enemy: dict[str, Any], runs: int) -> float:
        """Measure player win rate; draws count as half a win."""
        paths = []
        for filename, loadout in (("player.json", player), ("enemy.json", enemy)):
            path = self.temp / filename
            prepared = {key: loadout[key] for key in LOADOUT_KEYS if key in loadout}
            prepared.setdefault("cols", 5)
            prepared.setdefault("rows", 4)
            path.write_text(json.dumps(prepared, ensure_ascii=False), encoding="utf-8")
            paths.append(path)
        command = [
            "java", "-cp", self.classpath, balance.MAIN_CLASS, str(paths[0]), str(paths[1]),
            "--runs", str(runs), "--items", str(ENGINE_DATA / "items.json"),
            "--sects", str(ENGINE_DATA / "sects.json"),
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
            raise RuntimeError(f"Không đọc được kết quả:\n{completed.stdout}")
        return float(match.group(1)) + float(match.group(2)) / 2.0

    @staticmethod
    def distance(rate: float, target: float, low: float, high: float) -> tuple[float, float]:
        """Rank a result by band distance, then target distance."""
        outside = low - rate if rate < low else rate - high if rate > high else 0.0
        return outside, abs(rate - target)

    def allowed_steps(self, chapter: int, kind: str) -> list[tuple[int, str]]:
        """Return strength-ordered chapter-legal uniform steps."""
        low, high = TIER_RANGES[chapter - 1]
        if kind == "boss":
            high += 1
        rarity_cap = RARITY_CAPS[chapter - 1]
        if chapter == 3 and kind == "boss":
            rarity_cap = 3
        steps = [
            (tier, RARITIES[rarity_index])
            for tier in range(low, high + 1)
            for rarity_index in range(rarity_cap + 1)
        ]
        multipliers = balance.load_battle_config(self.classpath, None)
        tier_values = multipliers["tierMultiplierPermille"]
        rarity_values = multipliers["rarityMultiplierPermille"]
        return sorted(
            steps,
            key=lambda step: (
                tier_values[step[0] - 1] * rarity_values[RARITIES.index(step[1])],
                step[0],
                RARITIES.index(step[1]),
            ),
        )

    def probe_steps(
        self,
        player: dict[str, Any],
        stage: dict[str, Any],
        identities: list[str],
        chapter: int,
    ) -> tuple[dict[str, Any], float]:
        """Binary-probe legal uniform steps and return the closest candidate."""
        target, low_band, high_band = target_band(chapter, stage["kind"])
        steps = self.allowed_steps(chapter, stage["kind"])
        low = 0
        high = len(steps) - 1
        seen: dict[int, tuple[dict[str, Any], float]] = {}
        for index in (0, len(steps) - 1):
            tier, rarity = steps[index]
            enemy = self.make_enemy(stage, identities, tier, rarity)
            seen[index] = (enemy, self.measure(player, enemy, self.probe_runs))
        while low <= high and len(seen) < 6:
            index = (low + high) // 2
            tier, rarity = steps[index]
            enemy = self.make_enemy(stage, identities, tier, rarity)
            rate = self.measure(player, enemy, self.probe_runs)
            seen[index] = (enemy, rate)
            if rate > high_band:
                low = index + 1
            elif rate < low_band:
                high = index - 1
            else:
                break
        best = min(seen, key=lambda index: self.distance(seen[index][1], target, low_band, high_band))
        return seen[best]

    def mixed_variants(
        self,
        enemy: dict[str, Any],
        chapter: int,
        kind: str,
        strengthen: bool,
    ) -> list[dict[str, Any]]:
        """Create legal one-step mixed-tier and mixed-rarity refinements."""
        variants = [enemy]
        tier_low, tier_high = TIER_RANGES[chapter - 1]
        if kind == "boss":
            tier_high += 1
        rarity_cap = RARITY_CAPS[chapter - 1]
        if chapter == 3 and kind == "boss":
            rarity_cap = 3
        deltas = (1, -1) if strengthen else (-1, 1)
        for delta in deltas:
            for index in range(len(enemy["items"])):
                variant = copy.deepcopy(enemy)
                item = variant["items"][index]
                changed_tier = item["tier"] + delta
                if tier_low <= changed_tier <= tier_high:
                    item["tier"] = changed_tier
                    variants.append(variant)
                variant = copy.deepcopy(enemy)
                item = variant["items"][index]
                rarity_index = RARITIES.index(item["rarity"]) + delta
                if 0 <= rarity_index <= rarity_cap:
                    item["rarity"] = RARITIES[rarity_index]
                    variants.append(variant)
        return variants

    def tune_stage(self, number: int, chapter: int, local_index: int, stage: dict[str, Any]) -> None:
        """Tune one stage using only plausible, chapter-legal variants."""
        player, _starter, _lucky, _level, _silver = self.progression.bags_for_stage(number)
        target, low_band, high_band = target_band(chapter, stage["kind"])
        count_low, count_high = COUNT_RANGES[chapter - 1]
        base_ids = self.identities(number, chapter, local_index, stage["kind"], count_high)
        counts = [len(base_ids)]
        counts.extend(count for count in range(count_low, count_high + 1) if count not in counts)
        probe_candidates = []
        for count in counts:
            identities = base_ids[:count]
            while len(identities) < count:
                identities.append(SUPPORTS[THEMES[number - 1]][len(identities) % len(SUPPORTS[THEMES[number - 1]])])
            if not any(self.catalog[item_id]["type"] == "armor" for item_id in identities):
                continue
            try:
                enemy, rate = self.probe_steps(player, stage, identities, chapter)
            except ValueError:
                continue
            probe_candidates.append((enemy, rate))
        if not probe_candidates:
            theme = THEMES[number - 1]
            for count in counts:
                identities = [WEAPONS[number - 1], ARMORS[theme][0], SUPPORTS[theme][0]]
                while len(identities) < count:
                    identities.append(SUPPORTS[theme][len(identities) % len(SUPPORTS[theme])])
                try:
                    enemy, rate = self.probe_steps(player, stage, identities, chapter)
                except ValueError:
                    continue
                probe_candidates.append((enemy, rate))
        if not probe_candidates:
            raise ValueError(f"Ải {number}: không có kit nào xếp vừa lưới")
        probe_candidates.sort(
            key=lambda pair: self.distance(pair[1], target, low_band, high_band),
        )
        confirmed = [
            (enemy, self.measure(player, enemy, self.confirm_runs))
            for enemy, _probe_rate in probe_candidates
        ]
        selected_base, base_rate = min(
            confirmed,
            key=lambda pair: self.distance(pair[1], target, low_band, high_band),
        )
        strengthen = base_rate > high_band
        variants = self.mixed_variants(selected_base, chapter, stage["kind"], strengthen)
        for variant in variants[1:7]:
            rate = self.measure(player, variant, self.confirm_runs)
            confirmed.append((variant, rate))
            if low_band <= rate <= high_band:
                break
        selected, rate = min(
            confirmed,
            key=lambda pair: self.distance(pair[1], target, low_band, high_band),
        )
        stage["enemy"]["items"] = selected["items"]
        probe_rate = probe_candidates[0][1]
        self.rows.append({
            "stage": number,
            "chapter": chapter,
            "kind": stage["kind"],
            "enemy": stage["enemy"]["name"],
            "probe": probe_rate,
            "winRate": rate,
            "target": target,
            "band": [low_band, high_band],
            "inBand": low_band <= rate <= high_band,
        })
        print(f"Ải {number:02d}: {rate:5.1f}% · {len(selected['items'])} món", flush=True)

    def run(self) -> None:
        """Tune stages 2 through 50; stage 1 remains byte-for-data identical."""
        number = 0
        for chapter, chapter_data in enumerate(self.document["chapters"], 1):
            for local_index, stage in enumerate(chapter_data["stages"]):
                number += 1
                if number == 1:
                    continue
                self.tune_stage(number, chapter, local_index, stage)
            write_json(CANDIDATE, self.document)

    def validate(self) -> list[str]:
        """Validate every hard rule and return human-readable checks."""
        source_copy = copy.deepcopy(self.source)
        candidate_copy = copy.deepcopy(self.document)
        for source_chapter, candidate_chapter in zip(source_copy["chapters"], candidate_copy["chapters"]):
            for source_stage, candidate_stage in zip(source_chapter["stages"], candidate_chapter["stages"]):
                source_stage["enemy"].pop("items")
                candidate_stage["enemy"].pop("items")
        if source_copy != candidate_copy:
            raise ValueError("Có trường ngoài enemy.items bị thay đổi")
        if self.document["chapters"][0]["stages"][0] != self.source["chapters"][0]["stages"][0]:
            raise ValueError("Ải 1 bị thay đổi")
        previous_average: float | None = None
        number = 0
        for chapter, chapter_data in enumerate(self.document["chapters"], 1):
            counts: Counter[str] = Counter()
            previous_weapon: set[str] | None = None
            previous_average = None
            for stage in chapter_data["stages"]:
                number += 1
                if number == 1:
                    continue
                items = stage["enemy"]["items"]
                low_count, high_count = COUNT_RANGES[chapter - 1]
                if not low_count <= len(items) <= high_count:
                    raise ValueError(f"Ải {number}: sai số món")
                types = {self.catalog[item["id"]]["type"] for item in items}
                if not {"weapon", "armor"}.issubset(types) or not types.intersection({"pill", "manual", "relic"}):
                    raise ValueError(f"Ải {number}: kit không đủ nhóm")
                occupied: set[tuple[int, int]] = set()
                weapon_copies: Counter[str] = Counter()
                weapon_ids = set()
                for item in items:
                    catalog_item = self.catalog[item["id"]]
                    width, height = catalog_item["size"]
                    cells = {
                        (item["x"] + offset_x, item["y"] + offset_y)
                        for offset_x in range(width)
                        for offset_y in range(height)
                    }
                    if any(x < 0 or x >= 5 or y < 0 or y >= 4 for x, y in cells):
                        raise ValueError(f"Ải {number}: vượt lưới")
                    if not cells.isdisjoint(occupied):
                        raise ValueError(f"Ải {number}: chồng ô")
                    occupied.update(cells)
                    tier_low, tier_high = TIER_RANGES[chapter - 1]
                    tier_high += int(stage["kind"] == "boss")
                    if not tier_low <= item["tier"] <= tier_high:
                        raise ValueError(f"Ải {number}: tier ngoài dải")
                    rarity_cap = RARITY_CAPS[chapter - 1]
                    if chapter == 3 and stage["kind"] == "boss":
                        rarity_cap = 3
                    if RARITIES.index(item["rarity"]) > rarity_cap:
                        raise ValueError(f"Ải {number}: phẩm ngoài dải")
                    if catalog_item["type"] == "weapon":
                        weapon_ids.add(item["id"])
                        weapon_copies[item["id"]] += 1
                if any(value > 2 for value in weapon_copies.values()):
                    raise ValueError(f"Ải {number}: quá hai bản sao vũ khí")
                counts.update(weapon_ids)
                if previous_weapon is not None and weapon_ids.intersection(previous_weapon):
                    raise ValueError(f"Ải {number}: lặp vũ khí liên tiếp")
                previous_weapon = weapon_ids
                average = sum(item["tier"] for item in items) / len(items)
                if previous_average is not None and average < previous_average - 1.0 - 1e-9:
                    raise ValueError(f"Ải {number}: tier trung bình giảm quá 1")
                previous_average = average
            if counts and max(counts.values()) > 3:
                raise ValueError(f"Hồi {chapter}: vũ khí xuất hiện quá 3 ải")
            sword_limit = 2 if chapter == 1 else 1
            if counts["thanh_phong_kiem"] > sword_limit:
                raise ValueError(f"Hồi {chapter}: quá nhiều Thanh Phong Kiếm")
        return [
            "Ải 1 giữ nguyên.",
            "49/49 ải được kiểm tra lưới; ải 2–50 đủ vũ khí, giáp và hỗ trợ.",
            "Số món, tier, phẩm và độ giảm tier trung bình đúng dải từng Hồi.",
            "Đa dạng vũ khí, giới hạn bản sao và Thanh Phong Kiếm đều đạt.",
            "Chỉ enemy.items khác dữ liệu nguồn.",
        ]

    def smooth_average_tiers(self) -> list[int]:
        """Raise the fewest low tiers needed to satisfy the chapter slope rule."""
        changed = []
        number = 0
        for chapter, chapter_data in enumerate(self.document["chapters"], 1):
            previous_average: float | None = None
            for stage in chapter_data["stages"]:
                number += 1
                items = stage["enemy"]["items"]
                if number == 1:
                    previous_average = sum(item["tier"] for item in items) / len(items)
                    continue
                tier_high = TIER_RANGES[chapter - 1][1] + int(stage["kind"] == "boss")
                average = sum(item["tier"] for item in items) / len(items)
                floor = previous_average - 1.0 if previous_average is not None else average
                touched = False
                while average + 1e-9 < floor:
                    candidates = [item for item in items if item["tier"] < tier_high]
                    if not candidates:
                        raise ValueError(f"Ải {number}: không thể làm trơn tier trung bình")
                    min(candidates, key=lambda item: (item["tier"], item["id"]))["tier"] += 1
                    average = sum(item["tier"] for item in items) / len(items)
                    touched = True
                if touched:
                    changed.append(number)
                previous_average = average
        return changed

    def normalize_weapon_variety(self) -> list[int]:
        """Remove auxiliary weapons while retaining each Round-1 themed weapon."""
        changed = []
        number = 0
        for chapter_data in self.document["chapters"]:
            for stage in chapter_data["stages"]:
                number += 1
                if number == 1:
                    continue
                primary = WEAPONS[number - 1]
                theme = THEMES[number - 1]
                small_support = [
                    item_id
                    for item_id in SUPPORTS[theme]
                    if self.catalog[item_id]["size"] == [1, 1]
                ]
                if not small_support:
                    small_support = ["kim_sang_duoc"]
                replacement_index = 0
                touched = False
                for item in stage["enemy"]["items"]:
                    if self.catalog[item["id"]]["type"] != "weapon" or item["id"] == primary:
                        continue
                    item["id"] = small_support[replacement_index % len(small_support)]
                    replacement_index += 1
                    touched = True
                if touched:
                    stage["enemy"]["items"] = self.pack(stage["enemy"]["items"])
                    changed.append(number)
        return changed

    def confirm_document(self) -> None:
        """Confirm every tuned stage in the current document at 40 runs."""
        self.rows = []
        number = 0
        for chapter, chapter_data in enumerate(self.document["chapters"], 1):
            for stage in chapter_data["stages"]:
                number += 1
                if number == 1:
                    continue
                player, _starter, _lucky, _level, _silver = self.progression.bags_for_stage(number)
                target, low_band, high_band = target_band(chapter, stage["kind"])
                rate = self.measure(player, stage["enemy"], self.confirm_runs)
                self.rows.append({
                    "stage": number,
                    "chapter": chapter,
                    "kind": stage["kind"],
                    "enemy": stage["enemy"]["name"],
                    "probe": None,
                    "winRate": rate,
                    "target": target,
                    "band": [low_band, high_band],
                    "inBand": low_band <= rate <= high_band,
                })
                print(f"Xác nhận ải {number:02d}: {rate:5.1f}%", flush=True)

    def write_report(self, checks: list[str], applied: bool) -> None:
        """Write the Vietnamese Round-4 review."""
        rows_by_stage = {row["stage"]: row for row in self.rows}
        lines = [
            "# PvE enemy kits — Round 4",
            "",
            f"Kết quả: **{'đã áp dụng' if applied else 'chưa áp dụng'}**. "
            f"Trong dải: **{sum(row['inBand'] for row in self.rows)}/49**.",
            "",
            "| Ải | Địch | Món | Tier | Phẩm | Vũ khí | Thắng | Mục tiêu | Trong dải |",
            "|---:|---|---:|---|---|---|---:|---|:---:|",
        ]
        number = 0
        for chapter in self.document["chapters"]:
            for stage in chapter["stages"]:
                number += 1
                items = stage["enemy"]["items"]
                weapons = [
                    self.catalog[item["id"]]["name"]
                    for item in items
                    if self.catalog[item["id"]]["type"] == "weapon"
                ]
                if number == 1:
                    lines.append(
                        f"| 1 | {stage['enemy']['name']} | 1 | T1 | common | "
                        "Thanh Phong Kiếm | miễn đo | miễn | — |"
                    )
                    continue
                row = rows_by_stage[number]
                tiers = [item["tier"] for item in items]
                rarity_indexes = [RARITIES.index(item["rarity"]) for item in items]
                rarity_range = RARITIES[min(rarity_indexes)]
                if min(rarity_indexes) != max(rarity_indexes):
                    rarity_range += f"–{RARITIES[max(rarity_indexes)]}"
                lines.append(
                    f"| {number} | {stage['enemy']['name']} | {len(items)} | "
                    f"T{min(tiers)}–{max(tiers)} | {rarity_range} | {', '.join(weapons)} | "
                    f"{row['winRate']:.1f}% | {row['target']:.0f}% "
                    f"({row['band'][0]:.0f}–{row['band'][1]:.0f}) | "
                    f"{'Có' if row['inBand'] else 'Không'} |"
                )
        lines.extend(["", "## Rule check", ""])
        lines.extend(f"- {check}" for check in checks)
        failed = [row for row in self.rows if not row["inBand"]]
        lines.extend(["", "## Ngoài dải", ""])
        if failed:
            lines.append(
                "Tỷ lệ cao hơn dải nghĩa là kit hợp lệ gần nhất vẫn quá yếu; tỷ lệ thấp hơn dải nghĩa là "
                "kit hợp lệ gần nhất vẫn quá mạnh. Không phá hard rules để ép số."
            )
            lines.append("")
            for row in failed:
                lines.append(
                    f"- Ải {row['stage']}: {row['winRate']:.1f}% ngoài "
                    f"{row['band'][0]:.0f}–{row['band'][1]:.0f}%; giữ hard rules."
                )
        else:
            lines.append("- Không có.")
        REPORT.write_text("\n".join(lines) + "\n", encoding="utf-8")


def main() -> int:
    """Run Round 4 and apply only after the requested gate passes."""
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--probe-runs", type=int, default=20)
    parser.add_argument("--confirm-runs", type=int, default=40)
    parser.add_argument("--finalize-existing", action="store_true")
    args = parser.parse_args()
    if args.probe_runs != 20 or args.confirm_runs != 40:
        raise ValueError("Round 4 phải dùng probe 20 và confirm 40")
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    tuner = Round4(args.probe_runs, args.confirm_runs)
    try:
        if args.finalize_existing:
            tuner.document = read_json(CANDIDATE)
            weapon_changes = tuner.normalize_weapon_variety()
            print(f"Chuẩn hóa vũ khí phụ: {weapon_changes}")
            changed = tuner.smooth_average_tiers()
            print(f"Làm trơn tier: {changed}")
            tuner.confirm_document()
            write_json(CANDIDATE, tuner.document)
        else:
            tuner.run()
        checks = tuner.validate()
        in_band = sum(row["inBand"] for row in tuner.rows)
        bosses_pass = all(row["inBand"] for row in tuner.rows if row["kind"] == "boss")
        applied = in_band >= 44 and bosses_pass
        write_json(RESULTS, {"rows": tuner.rows, "inBand": in_band, "bossesPass": bosses_pass, "applied": applied})
        if applied:
            shutil.copy2(SOURCE, BACKUP)
            shutil.copy2(CANDIDATE, SOURCE)
        tuner.write_report(checks, applied)
        print(f"Trong dải: {in_band}/49; boss đạt: {bosses_pass}; áp dụng: {applied}")
    finally:
        tuner.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
