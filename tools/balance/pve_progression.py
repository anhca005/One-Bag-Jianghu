#!/usr/bin/env python3
"""Model item, material, exchange, and Trong spending for the PvE curve."""
from __future__ import annotations

import argparse
import copy
import json
import random
import sys
from collections import Counter
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import power  # noqa: E402

DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
CHAPTERS = DATA / "Pve" / "chapters.json"
ITEMS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "items.json"
POWER = ROOT / "tools" / "codex" / "out" / "power-hybrid" / "power.json"
COLS = 5
ROWS = 4
SEEDS = tuple(52000 + index for index in range(9))
LUCKY_REPLAYS = 3


def read_json(path: Path) -> Any:
    """Read one UTF-8 JSON document."""
    with path.open(encoding="utf-8") as stream:
        return json.load(stream)


def write_json(path: Path, value: Any) -> None:
    """Write one stable UTF-8 JSON document."""
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="\n") as stream:
        json.dump(value, stream, ensure_ascii=False, indent=2)
        stream.write("\n")


def level_from_exp(total_exp: int, levels: dict[str, Any]) -> int:
    """Convert cumulative EXP to a hero level."""
    level = 1
    remaining = total_exp
    for threshold in levels["expToNext"]:
        if remaining < threshold:
            break
        remaining -= threshold
        level += 1
    return min(level, levels["maxLevel"])


@dataclass
class Offer:
    """One concrete shop slot."""
    kind: str
    content_id: str
    quantity: int
    price: int
    rarity: str = ""


@dataclass
class EconomyState:
    """Mutable economy state for one seed."""
    owned: list[dict[str, Any]]
    silver: int
    materials: Counter[str] = field(default_factory=Counter)
    spent: Counter[str] = field(default_factory=Counter)
    next_uid: int = 0


@dataclass
class CurvePoint:
    """One seed's state immediately before a fight."""
    bag: dict[str, Any]
    level: int
    silver: int
    bag_lc: int
    seed: int
    average_tier: float
    spent: dict[str, int]
    materials: dict[str, int]


class Progression:
    """Roll the real shop and greedily maximize marginal LC per Bac."""

    def __init__(self, bag_style: str = "typical") -> None:
        self.bag_style = bag_style
        self.starter = read_json(DATA / "starter.json")
        self.shop = read_json(DATA / "shop.json")
        self.levels = read_json(DATA / "levels.json")
        self.chapters = read_json(CHAPTERS)
        self.catalog = read_json(ITEMS)
        self.power = read_json(POWER)
        self.upgrades = read_json(DATA / "upgrade_costs.json")["steps"]
        self.material_catalog = read_json(DATA / "materials.json")
        self.catalog_by_id = {item["id"]: item for item in self.catalog}
        self.material_by_id = {item["id"]: item for item in self.material_catalog}
        self.material_id = {
            (item["itemType"], item["tier"], item["isCatalyst"]): item["id"]
            for item in self.material_catalog
            if not item.get("pvpOnly")
        }
        self.stages = [stage for chapter in self.chapters["chapters"] for stage in chapter["stages"]]
        monster_ids = set(self.chapters.get("monsterItems", []))
        self.shop_ids = [item["id"] for item in self.catalog if item["id"] not in monster_ids]
        self.curves: dict[tuple[bool, str], list[list[CurvePoint]]] = {}
        self.marginal_cache: dict[tuple[str, int, str, int], int] = {}

    def _group(self, item_id: str) -> str:
        item_type = self.catalog_by_id[item_id].get("type", "")
        return "pill_relic" if item_type in ("pill", "relic") else item_type

    def _area(self, item: dict[str, Any]) -> int:
        width, height = self.catalog_by_id[item["id"]]["size"]
        return width * height

    def _place(self, packed: list[dict[str, Any]], candidate: dict[str, Any]) -> bool:
        occupied = set()
        for item in packed:
            width, height = self.catalog_by_id[item["id"]]["size"]
            occupied.update(
                (x, y)
                for x in range(item["x"], item["x"] + width)
                for y in range(item["y"], item["y"] + height)
            )
        width, height = self.catalog_by_id[candidate["id"]]["size"]
        for y in range(ROWS):
            for x in range(COLS):
                cells = {
                    (cell_x, cell_y)
                    for cell_x in range(x, x + width)
                    for cell_y in range(y, y + height)
                }
                if x + width <= COLS and y + height <= ROWS and not cells.intersection(occupied):
                    placed = copy.deepcopy(candidate)
                    placed["x"] = x
                    placed["y"] = y
                    packed.append(placed)
                    return True
        return False

    def _item_marginal(self, item: dict[str, Any], level: int) -> int:
        key = (item["id"], item["tier"], item["rarity"], level)
        if key in self.marginal_cache:
            return self.marginal_cache[key]
        empty = self._loadout("empty", 0, level, [])
        single = self._loadout("single", 0, level, [{**item, "x": 0, "y": 0}])
        value = power.lc(single, self.power) - power.lc(empty, self.power)
        self.marginal_cache[key] = value
        return value

    def _sort_key(self, item: dict[str, Any], level: int) -> tuple[float, int, int, str, str]:
        marginal = self._item_marginal(item, level)
        return (-marginal / self._area(item), -marginal, self._area(item), item["id"], item.get("uid", ""))

    def select_bag(
        self, owned: list[dict[str, Any]], level: int = 1, bag_style: str | None = None,
    ) -> list[dict[str, Any]]:
        """Pack items into the bag according to greedy or typical style."""
        style = self.bag_style if bag_style is None else bag_style
        ordered = sorted(owned, key=lambda item: self._sort_key(item, level))
        weapons = [item for item in ordered if self.catalog_by_id[item["id"]].get("type") == "weapon"]
        if not weapons:
            raise ValueError("Owned item set has no weapon")
        if style == "greedy":
            reserved = weapons[0]
            packed: list[dict[str, Any]] = []
            self._place(packed, reserved)
            skipped_reserved = False
            for candidate in ordered:
                if candidate is reserved and not skipped_reserved:
                    skipped_reserved = True
                    continue
                self._place(packed, candidate)
            return packed

        res_weapons = weapons[:2] if len(weapons) >= 2 else weapons[:1]
        armors = [item for item in ordered if self.catalog_by_id[item["id"]].get("type") == "armor"]
        res_armors = armors[:1] if armors else []
        reserved_list = res_weapons + res_armors

        packed: list[dict[str, Any]] = []
        placed_ids: set[int] = set()
        for res_item in reserved_list:
            if self._place(packed, res_item):
                placed_ids.add(id(res_item))

        manual_count = sum(1 for item in packed if self.catalog_by_id[item["id"]].get("type") == "manual")
        pill_count = sum(1 for item in packed if self.catalog_by_id[item["id"]].get("type") == "pill")

        for candidate in ordered:
            if id(candidate) in placed_ids:
                continue
            item_type = self.catalog_by_id[candidate["id"]].get("type", "")
            if item_type == "manual" and manual_count >= 3:
                continue
            if item_type == "pill" and pill_count >= 3:
                continue
            if self._place(packed, candidate):
                placed_ids.add(id(candidate))
                if item_type == "manual":
                    manual_count += 1
                elif item_type == "pill":
                    pill_count += 1
        return packed

    def _starter_state(self) -> EconomyState:
        owned = []
        for index, entry in enumerate(self.starter["items"]):
            owned.append({
                "id": entry["itemId"], "tier": entry["tier"], "rarity": entry["rarity"],
                "uid": f"starter-{index}",
            })
        return EconomyState(owned, self.starter["currencies"]["bac"], next_uid=len(owned))

    def _bag_lc(self, owned: list[dict[str, Any]], level: int) -> int:
        return power.lc(self._loadout("candidate", 0, level, self.select_bag(owned, level)), self.power)

    @staticmethod
    def _weighted(slot: dict[str, Any], rng: random.Random) -> str:
        return rng.choices(slot["rarities"], weights=slot["weights"], k=1)[0]

    def _pick_material(self, rng: random.Random, tier: int, catalyst: bool, group: str = "") -> str:
        pool = [
            item["id"] for item in self.material_catalog
            if item["tier"] == tier
            and item["isCatalyst"] == catalyst
            and not item.get("pvpOnly")
            and (not group or item["itemType"] == group)
        ]
        return rng.choice(pool)

    def _price(self, item_id: str, rarity: str, discount: int) -> int:
        base = self.shop["itemBasePrices"][rarity]
        item_type = self.catalog_by_id[item_id].get("type", "")
        multiplier = self.shop.get("itemTypePriceMultipliers", {}).get(item_type, 1.0)
        return round(base * multiplier) * (100 - discount) // 100

    def _roll_offers(self, rng: random.Random) -> list[Offer]:
        offers = []
        for slot in self.shop["slots"]:
            slot_type = slot["type"]
            if slot_type == "material":
                material_id = self._pick_material(rng, slot["tier"], False)
                offers.append(Offer("material", material_id, slot["quantity"], slot["price"]))
                continue
            if slot_type == "catalyst_or_tier3":
                if rng.randrange(100) < 40:
                    offers.append(Offer("material", self._pick_material(rng, 0, True), 1, 1200))
                else:
                    offers.append(Offer("material", self._pick_material(rng, 3, False), 2, 800))
                continue
            if slot_type == "discount" and rng.randrange(100) < 30:
                material_id = self.material_id[("pill_relic", 1, False)]
                price = 250 * (100 - slot["discountPercent"]) // 100
                offers.append(Offer("material", material_id, 5, price))
                continue
            rarity = self._weighted(slot, rng)
            item_id = rng.choice(self.shop_ids)
            price = self._price(item_id, rarity, slot.get("discountPercent", 0))
            offers.append(Offer("item", item_id, 1, price, rarity))
        return offers

    def _upgrade_cost(self, item: dict[str, Any]) -> dict[str, int]:
        step = self.upgrades[item["rarity"]][item["tier"] - 1]
        group = self._group(item["id"])
        costs = {self.material_id[(group, step["materialTier"], False)]: step["materialCount"]}
        if step["catalystCount"]:
            costs[self.material_id[(group, 0, True)]] = step["catalystCount"]
        return costs

    def _conversion_plan(
        self, inventory: Counter[str], target_id: str, count: int,
    ) -> tuple[int, Counter[str]] | None:
        target = self.material_by_id[target_id]
        if target["isCatalyst"]:
            return None
        candidates = []
        for source in self.material_catalog:
            if source["isCatalyst"] or source.get("pvpOnly"):
                continue
            spend = Counter()
            fee = 0
            if source["itemType"] != target["itemType"] and source["tier"] == target["tier"]:
                spend[source["id"]] = 2 * count
                fee = self.shop["exchange"]["crossFees"][source["tier"] - 1] * count
            elif source["itemType"] == target["itemType"] and source["tier"] + 1 == target["tier"]:
                spend[source["id"]] = 3 * count
                fee = self.shop["exchange"]["sameFees"][source["tier"] - 1] * count
            else:
                continue
            if all(inventory[key] >= value for key, value in spend.items()):
                candidates.append((fee, spend))
        return min(candidates, key=lambda entry: entry[0]) if candidates else None

    def _material_plan(
        self, inventory: Counter[str], costs: dict[str, int],
    ) -> tuple[int, Counter[str]] | None:
        remaining = inventory.copy()
        spend = Counter()
        fee = 0
        for material_id, required in costs.items():
            direct = min(required, remaining[material_id])
            remaining[material_id] -= direct
            spend[material_id] += direct
            missing = required - direct
            if missing == 0:
                continue
            conversion = self._conversion_plan(remaining, material_id, missing)
            if conversion is None:
                return None
            conversion_fee, conversion_spend = conversion
            fee += conversion_fee
            for source_id, quantity in conversion_spend.items():
                remaining[source_id] -= quantity
                spend[source_id] += quantity
        return fee, spend

    def _upgrade_gain(self, state: EconomyState, item: dict[str, Any], level: int) -> int:
        if item["tier"] >= 9:
            return 0
        old_lc = self._bag_lc(state.owned, level)
        item["tier"] += 1
        new_lc = self._bag_lc(state.owned, level)
        item["tier"] -= 1
        return new_lc - old_lc

    def _best_upgrade(
        self, state: EconomyState, level: int,
    ) -> tuple[float, dict[str, Any], int, Counter[str]] | None:
        candidates = []
        bag_uids = {item["uid"] for item in self.select_bag(state.owned, level)}
        for item in state.owned:
            if item["uid"] not in bag_uids or item["tier"] >= 9:
                continue
            step = self.upgrades[item["rarity"]][item["tier"] - 1]
            plan = self._material_plan(state.materials, self._upgrade_cost(item))
            if plan is None:
                continue
            exchange_fee, material_spend = plan
            total_cost = step["silver"] + exchange_fee
            gain = self._upgrade_gain(state, item, level)
            if gain > 0 and total_cost <= state.silver:
                candidates.append((gain / total_cost, item, total_cost, material_spend))
        return max(candidates, key=lambda entry: entry[0]) if candidates else None

    def _material_value(self, state: EconomyState, offer: Offer, level: int) -> float:
        trial = state.materials.copy()
        trial[offer.content_id] += offer.quantity
        best = 0.0
        bag_uids = {item["uid"] for item in self.select_bag(state.owned, level)}
        for item in state.owned:
            if item["uid"] not in bag_uids or item["tier"] >= 9:
                continue
            gain = self._upgrade_gain(state, item, level)
            costs = self._upgrade_cost(item)
            before = sum(min(state.materials[key], value) / value for key, value in costs.items())
            after = sum(min(trial[key], value) / value for key, value in costs.items())
            progress = max(0.0, after - before) / len(costs)
            best = max(best, gain * progress)
        return best

    def _best_offer(
        self, state: EconomyState, level: int, offers: list[Offer],
    ) -> tuple[float, int, float] | None:
        current_lc = self._bag_lc(state.owned, level)
        candidates = []
        for index, offer in enumerate(offers):
            if offer.price > state.silver:
                continue
            if offer.kind == "item":
                candidate = {"id": offer.content_id, "tier": 1, "rarity": offer.rarity, "uid": "trial"}
                gain = self._bag_lc(state.owned + [candidate], level) - current_lc
            else:
                gain = self._material_value(state, offer, level)
            if gain > 0:
                candidates.append((gain / offer.price, index, gain))
        return max(candidates, key=lambda entry: (entry[0], entry[2])) if candidates else None

    def _buy(self, state: EconomyState, offer: Offer) -> None:
        state.silver -= offer.price
        if offer.kind == "item":
            state.owned.append({
                "id": offer.content_id, "tier": 1, "rarity": offer.rarity,
                "uid": f"item-{state.next_uid}",
            })
            state.next_uid += 1
            state.spent["items"] += offer.price
        else:
            state.materials[offer.content_id] += offer.quantity
            state.spent["materials"] += offer.price

    def _apply_upgrade(
        self, state: EconomyState, item: dict[str, Any], total_cost: int, material_spend: Counter[str],
    ) -> None:
        step = self.upgrades[item["rarity"]][item["tier"] - 1]
        state.silver -= total_cost
        state.spent["upgrades"] += step["silver"]
        state.spent["materials"] += total_cost - step["silver"]
        state.materials.subtract(material_spend)
        item["tier"] += 1

    def _refresh_cost(self, refresh_index: int) -> int:
        """Paid shop refresh price: a fixed price, or the legacy escalating list."""
        if "paidRefreshCost" in self.shop:
            return int(self.shop["paidRefreshCost"])
        costs = self.shop["paidRefreshCosts"]
        return int(costs[min(refresh_index, len(costs) - 1)])

    def _shop_session(self, state: EconomyState, level: int, rng: random.Random) -> None:
        offers = self._roll_offers(rng)
        refresh_index = 0
        while True:
            offer_choice = self._best_offer(state, level, offers)
            upgrade_choice = self._best_upgrade(state, level)
            offer_ratio = offer_choice[0] if offer_choice else 0.0
            upgrade_ratio = upgrade_choice[0] if upgrade_choice else 0.0
            if upgrade_choice and upgrade_ratio >= offer_ratio:
                _, item, cost, material_spend = upgrade_choice
                self._apply_upgrade(state, item, cost, material_spend)
                continue
            if offer_choice:
                _, index, _gain = offer_choice
                self._buy(state, offers.pop(index))
                continue
            refresh_cost = self._refresh_cost(refresh_index)
            if refresh_cost > state.silver:
                break
            refreshed = self._roll_offers(rng)
            if self._best_offer(state, level, refreshed) is None:
                break
            state.silver -= refresh_cost
            state.spent["refresh"] += refresh_cost
            refresh_index += 1
            offers = refreshed

    def _point(self, state: EconomyState, level: int, number: int, seed: int, lucky: bool) -> CurvePoint:
        bag_items = self.select_bag(state.owned, level)
        bag = self._loadout("lucky" if lucky else "expected", number, level, bag_items)
        average_tier = sum(item["tier"] for item in bag_items) / len(bag_items)
        materials = {key: value for key, value in sorted(state.materials.items()) if value}
        return CurvePoint(
            bag, level, state.silver, power.lc(bag, self.power), seed, average_tier,
            dict(state.spent), materials,
        )

    def _simulate(self, lucky: bool) -> list[list[CurvePoint]]:
        key = (lucky, self.bag_style)
        if key in self.curves:
            return self.curves[key]
        by_stage = [[] for _stage in self.stages]
        for seed in SEEDS:
            rng = random.Random(seed)
            state = self._starter_state()
            total_exp = self.starter.get("heroExp", 0)
            for index, stage in enumerate(self.stages):
                if lucky and index > 0:
                    prior = self.stages[index - 1]["rewards"]["firstClear"]
                    state.silver += LUCKY_REPLAYS * max(1, round(prior.get("silver", 0) * 0.15))
                level = level_from_exp(total_exp, self.levels)
                self._shop_session(state, level, rng)
                by_stage[index].append(self._point(state, level, index + 1, seed, lucky))
                reward = stage["rewards"]["firstClear"]
                state.silver += reward.get("silver", 0)
                state.materials.update(reward.get("materials", {}))
                total_exp += reward.get("heroExp", 0)
        self.curves[key] = by_stage
        return by_stage

    def point_for_stage(self, stage_number: int) -> CurvePoint:
        """Return the median-LC expected point for one stage."""
        points = sorted(self._simulate(False)[stage_number - 1], key=lambda point: (point.bag_lc, point.seed))
        return copy.deepcopy(points[len(points) // 2])

    def bags_for_stage(
        self, stage_number: int,
    ) -> tuple[dict[str, Any], dict[str, Any], dict[str, Any], int, int]:
        """Return median, starter-only, upper-quartile bags, level, and unspent Bac."""
        if stage_number < 1 or stage_number > len(self.stages):
            raise ValueError(f"Stage must be in 1..{len(self.stages)}")
        expected_points = sorted(self._simulate(False)[stage_number - 1], key=lambda point: point.bag_lc)
        lucky_points = sorted(self._simulate(True)[stage_number - 1], key=lambda point: point.bag_lc)
        expected = expected_points[len(expected_points) // 2]
        lucky = lucky_points[round(0.75 * (len(lucky_points) - 1))]
        starter_items = self.select_bag(self._starter_state().owned, expected.level)
        starter = self._loadout("starter-only", stage_number, expected.level, starter_items)
        return copy.deepcopy(expected.bag), starter, copy.deepcopy(lucky.bag), expected.level, expected.silver

    @staticmethod
    def _loadout(label: str, number: int, level: int, items: list[dict[str, Any]]) -> dict[str, Any]:
        clean = [{key: value for key, value in item.items() if key != "uid"} for item in items]
        return {
            "name": f"{label} s{number:02d}", "cols": COLS, "rows": ROWS, "level": level,
            "sect": None, "tamPhapLevel": 1, "items": clean,
        }


_DEFAULT_PROGRESSION: Progression | None = None


def bags_for_stage(
    stage_number: int, bag_style: str = "typical",
) -> tuple[dict[str, Any], dict[str, Any], dict[str, Any], int, int]:
    """Return upgrade-aware bags and pre-stage economy data."""
    global _DEFAULT_PROGRESSION
    if _DEFAULT_PROGRESSION is None or _DEFAULT_PROGRESSION.bag_style != bag_style:
        _DEFAULT_PROGRESSION = Progression(bag_style=bag_style)
    return _DEFAULT_PROGRESSION.bags_for_stage(stage_number)


def parse_stage_range(raw: str, maximum: int) -> list[int]:
    """Parse one inclusive N-M range or one stage number."""
    first, last = (int(value) for value in raw.split("-", 1)) if "-" in raw else (int(raw), int(raw))
    if first < 1 or last > maximum or first > last:
        raise ValueError(f"--stages must be within 1-{maximum}")
    return list(range(first, last + 1))


def main() -> int:
    """Print and optionally save the selected Monte Carlo curve."""
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--stages", default="1-50", help="inclusive stage range")
    parser.add_argument("--json", type=Path, help="optional machine-readable curve")
    args = parser.parse_args()
    progression = Progression()
    rows = []
    print("stage level items lc avg_tier item_bac mat_bac up_bac ref_bac unspent materials")
    for stage_number in parse_stage_range(args.stages, len(progression.stages)):
        point = progression.point_for_stage(stage_number)
        row = {
            "stage": stage_number, "level": point.level, "items": len(point.bag["items"]),
            "lc": point.bag_lc, "averageTier": round(point.average_tier, 2),
            "spentItems": point.spent.get("items", 0),
            "spentMaterials": point.spent.get("materials", 0),
            "spentUpgrades": point.spent.get("upgrades", 0),
            "spentRefresh": point.spent.get("refresh", 0), "unspent": point.silver,
            "materials": point.materials,
        }
        rows.append(row)
        print(
            f"{stage_number:02d} {point.level:02d} {row['items']:2d} {point.bag_lc:4d} "
            f"{point.average_tier:4.2f} {row['spentItems']:6d} {row['spentMaterials']:6d} "
            f"{row['spentUpgrades']:6d} {row['spentRefresh']:6d} {point.silver:7d} "
            f"{json.dumps(point.materials, ensure_ascii=False, separators=(',', ':'))}"
        )
    if args.json:
        write_json(args.json, rows)
    return 0


if __name__ == "__main__":
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    raise SystemExit(main())
