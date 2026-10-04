"""Build economy-aware PvE player bags for the stage-curve review."""

import json
from pathlib import Path


OUTPUT = Path(__file__).parent
ROOT = OUTPUT.parents[3]
DATA = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"
POSITIONS = [(x, y) for y in range(4) for x in range(5)]
ITEM_SIZES = {}


def read_json(path):
    return json.loads(path.read_text(encoding="utf-8"))


def write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def level_from_exp(total_exp, levels):
    level = 1
    remaining = total_exp
    for threshold in levels["expToNext"]:
        if remaining < threshold:
            break
        remaining -= threshold
        level += 1
    return min(level, levels["maxLevel"])


def first_free(items):
    occupied = set()
    for item in items:
        width, height = ITEM_SIZES[item["id"]]
        for x in range(item["x"], item["x"] + width):
            for y in range(item["y"], item["y"] + height):
                occupied.add((x, y))
    for x, y in POSITIONS:
        width, height = ITEM_SIZES[first_free.item_id]
        cells = {(cell_x, cell_y) for cell_x in range(x, x + width) for cell_y in range(y, y + height)}
        if x + width <= 5 and y + height <= 4 and not cells.intersection(occupied):
            return x, y
    return None


def add_item(items, reward):
    first_free.item_id = reward["id"]
    position = first_free(items)
    if position is None:
        return
    items.append(
        {
            "id": reward["id"],
            "x": position[0],
            "y": position[1],
            "tier": reward.get("tier", 1),
            "rarity": reward.get("rarity", "common"),
        }
    )


def repack(items):
    packed = []
    for item in items:
        add_item(packed, item)
    return packed


def make_player(label, stage_number, level, items):
    return {
        "name": f"{label} s{stage_number:02d}",
        "cols": 5,
        "rows": 4,
        "level": level,
        "sect": None,
        "tamPhapLevel": 1,
        "items": items,
    }


def affordable_rarity(budget, shop):
    prices = shop["itemBasePrices"]
    refresh = shop["paidRefreshCosts"][0]
    if budget >= prices["rare"] + refresh * 2:
        return "rare", prices["rare"] + refresh * 2
    if budget >= prices["uncommon"] + refresh * 2:
        return "uncommon", prices["uncommon"] + refresh * 2
    if budget >= prices["common"]:
        return "common", prices["common"]
    return None, 0


def repeat_income(stage, count, percent):
    reward = stage["rewards"]["firstClear"]
    silver = max(1, round(reward.get("silver", 0) * percent / 100))
    exp = max(1, round(reward.get("heroExp", 0) * percent / 100))
    return silver * count, exp * count


def main():
    starter = read_json(OUTPUT / "starter.json")
    chapters = read_json(OUTPUT / "chapters.json")
    shop = read_json(OUTPUT / "shop.json")
    levels = read_json(DATA / "levels.json")
    item_catalog = read_json(ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "items.json")
    ITEM_SIZES.update({item["id"]: item["size"] for item in item_catalog})
    stages = [stage for chapter in chapters["chapters"] for stage in chapter["stages"]]
    for stage in stages:
        stage["enemy"]["items"] = repack(stage["enemy"]["items"])
    write_json(OUTPUT / "chapters.json", chapters)
    monster_ids = set(chapters.get("monsterItems", []))
    weapons = [item["id"] for item in item_catalog if item.get("type") == "weapon" and item["id"] not in monster_ids]
    loadout_by_key = {entry["itemKey"]: entry for entry in starter["loadout"]}
    owned = []
    for entry in starter["items"]:
        loadout = loadout_by_key[entry["key"]]
        add_item(
            owned,
            {
                "id": entry["itemId"],
                "tier": entry["tier"],
                "rarity": entry["rarity"],
            },
        )
        owned[-1]["x"] = loadout["x"]
        owned[-1]["y"] = loadout["y"]

    silver = starter["currencies"]["bac"]
    total_exp = 0
    repeat_percent = chapters["repeatRewardPercent"]
    print("stage level silver_k0 silver_k3 items no_shop shop_smart purchase")
    for stage_number, stage in enumerate(stages, 1):
        level = level_from_exp(total_exp, levels)
        repeat_silver = 0
        if stage_number > 1:
            repeat_silver, _ = repeat_income(stages[stage_number - 2], 3, repeat_percent)
        no_shop_items = [dict(item) for item in owned]
        shop_items = [dict(item) for item in owned]
        rarity, spend = affordable_rarity(silver, shop)
        purchase = "-"
        if rarity is not None:
            owned_ids = {item["id"] for item in shop_items}
            weapon_id = next((item_id for item_id in weapons if item_id not in owned_ids), weapons[0])
            previous_count = len(shop_items)
            add_item(shop_items, {"id": weapon_id, "tier": 1, "rarity": rarity})
            if len(shop_items) > previous_count:
                purchase = f"{weapon_id}@{rarity};expected_cost={spend}"
        no_shop = make_player("no_shop", stage_number, level, no_shop_items)
        shop_smart = make_player("shop_smart", stage_number, level, shop_items)
        write_json(OUTPUT / "players" / "no_shop" / f"stage_{stage_number:02d}.json", no_shop)
        write_json(OUTPUT / "players" / "shop_smart" / f"stage_{stage_number:02d}.json", shop_smart)
        print(
            f"{stage_number:02d} {level:02d} {silver} {silver + repeat_silver} "
            f"{len(owned)} {len(no_shop_items)} {len(shop_items)} {purchase}"
        )
        first_clear = stage["rewards"]["firstClear"]
        silver += first_clear.get("silver", 0)
        total_exp += first_clear.get("heroExp", 0)
        for reward_item in first_clear.get("items", []):
            add_item(owned, reward_item)


if __name__ == "__main__":
    main()
