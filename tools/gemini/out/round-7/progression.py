"""
progression.py - Round 7 player bag model
Uses round-7 starter.json (600 Bac) and round-7 chapters.json.

Method for shop_smart:
  Slot 0 of Thuong Hoi: rarities [common, uncommon, rare], weights [60, 35, 5].
  Updated prices: common=300, uncommon=600, rare=1400.
  Refresh costs: [150, 250, ...].
  
  Budget thresholds for "smart buy" (item + up to 2 refreshes):
    - rare:     1400 + 2*150 = 1700 Bac
    - uncommon:  600 + 2*150 =  900 Bac
    - common:    300          =  300 Bac  (no refresh needed, common is ~60% weight)

  shop_smart adds ONE extra item to the bag (the best affordable weapon not already owned).
  The shop item uses tier=1 and the best rarity affordable.
"""
import json
from pathlib import Path

ROUND7 = Path(__file__).parent
ROOT = ROUND7.parents[3]

# Use round-7 config files
STARTER_PATH  = ROUND7 / "starter.json"
CHAPTERS_PATH = ROUND7 / "chapters.json"
SHOP_PATH     = ROUND7 / "shop.json"
ITEMS_PATH    = ROOT / "server/battle-engine/src/main/resources/data/items.json"
LEVELS_PATH   = ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/levels.json"

starter   = json.loads(STARTER_PATH.read_text("utf-8"))
chapters  = json.loads(CHAPTERS_PATH.read_text("utf-8"))
levels_d  = json.loads(LEVELS_PATH.read_text("utf-8"))
shop_d    = json.loads(SHOP_PATH.read_text("utf-8"))
items_all = json.loads(ITEMS_PATH.read_text("utf-8"))

stages = [s for ch in chapters["chapters"] for s in ch["stages"]]
EXP_TO_NEXT = levels_d["expToNext"]
MAX_LEVEL   = levels_d["maxLevel"]

def level_from_exp(exp):
    lv = 1
    for threshold in EXP_TO_NEXT:
        if exp < threshold:
            return lv
        exp -= threshold
        lv += 1
        if lv >= MAX_LEVEL:
            return MAX_LEVEL
    return MAX_LEVEL

PRICES = shop_d["itemBasePrices"]
REFRESH_COST_1 = shop_d["paidRefreshCosts"][0]  # 150

MONSTER_IDS = set(chapters.get("monsterItems", []))
SHOP_WEAPONS = [it for it in items_all
                if it.get("type") == "weapon" and it["id"] not in MONSTER_IDS]

def best_shop_buy(budget, owned_ids):
    # Thresholds with 2 refreshes budgeted
    if budget >= PRICES["rare"] + 2 * REFRESH_COST_1:
        target_rarity = "rare"
    elif budget >= PRICES["uncommon"] + 2 * REFRESH_COST_1:
        target_rarity = "uncommon"
    elif budget >= PRICES["common"]:
        target_rarity = "common"
    else:
        return None
    # Find first unowned weapon
    for it in SHOP_WEAPONS:
        if it["id"] not in owned_ids:
            return {"id": it["id"], "tier": 1, "rarity": target_rarity}
    return None

STARTER_BAC   = starter["currencies"]["bac"]
STARTER_ITEMS = [
    {"id": si["itemId"], "x": lo["x"], "y": lo["y"],
     "tier": si["tier"], "rarity": si["rarity"]}
    for si, lo in zip(starter["items"], starter["loadout"])
]

accumulated_bac   = STARTER_BAC
accumulated_exp   = 0
accumulated_items = list(STARTER_ITEMS)

NO_SHOP_DIR = ROUND7 / "players" / "no_shop"
SHOP_DIR    = ROUND7 / "players" / "shop_smart"
NO_SHOP_DIR.mkdir(parents=True, exist_ok=True)
SHOP_DIR.mkdir(parents=True, exist_ok=True)

POSITIONS = [(x, y) for y in range(4) for x in range(5)]

def pack(items):
    packed = []; used = set()
    for it in items:
        pos = (it["x"], it["y"])
        if pos not in used:
            used.add(pos); packed.append(dict(it))
        else:
            for x, y in POSITIONS:
                if (x, y) not in used:
                    used.add((x, y)); packed.append(dict(it, x=x, y=y)); break
    return packed

def next_free(items):
    used = {(it["x"], it["y"]) for it in items}
    for x, y in POSITIONS:
        if (x, y) not in used:
            return x, y
    return (0, 3)

print("Round-7 Progression Model")
print(f"Starter: {STARTER_BAC} Bac, {len(STARTER_ITEMS)} items")
print(f"Shop prices: common={PRICES['common']}, uncommon={PRICES['uncommon']}, rare={PRICES['rare']}")
print()
print(f"{'N':>2}  {'lv':>4}  {'Bac':>6}  {'no_shop':>9}  {'shop_smart':>10}  {'shop_buy':>20}")
print("-" * 70)

for idx, stage in enumerate(stages, 1):
    current_level = level_from_exp(accumulated_exp)
    current_bac   = accumulated_bac

    packed = pack(accumulated_items)

    no_shop_bag = {
        "name": f"no_shop s{idx:02d}",
        "cols": 5, "rows": 4,
        "level": current_level,
        "sect": None, "tamPhapLevel": 1,
        "items": packed
    }

    owned = {it["id"] for it in packed}
    extra = best_shop_buy(current_bac, owned)
    shop_items = list(packed)
    shop_buy_str = "-"
    if extra:
        px, py = next_free(shop_items)
        shop_items.append(dict(extra, x=px, y=py))
        shop_buy_str = f"{extra['id']}@{extra['rarity']}"

    shop_bag = {
        "name": f"shop_smart s{idx:02d}",
        "cols": 5, "rows": 4,
        "level": current_level,
        "sect": None, "tamPhapLevel": 1,
        "items": shop_items
    }

    (NO_SHOP_DIR / f"stage_{idx:02d}.json").write_text(
        json.dumps(no_shop_bag, ensure_ascii=False, indent=2), encoding="utf-8")
    (SHOP_DIR / f"stage_{idx:02d}.json").write_text(
        json.dumps(shop_bag, ensure_ascii=False, indent=2), encoding="utf-8")

    print(f"{idx:02d}  lv{current_level:2d}  {current_bac:6d}  {len(packed):9d}  {len(shop_items):10d}  {shop_buy_str}")

    fc = stage["rewards"].get("firstClear", {})
    accumulated_bac += fc.get("silver", 0)
    accumulated_exp += fc.get("heroExp", 0)
    for it in fc.get("items", []):
        nx, ny = next_free(accumulated_items)
        accumulated_items.append({
            "id": it["id"], "x": nx, "y": ny,
            "tier": it.get("tier", 1), "rarity": it.get("rarity", "common")
        })

print(f"\nFinal: Bac={accumulated_bac} EXP={accumulated_exp} lv={level_from_exp(accumulated_exp)} items={len(accumulated_items)}")
print("Done.")
