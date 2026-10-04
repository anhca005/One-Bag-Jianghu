import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ch = json.load(open("One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json", encoding="utf-8"))
c3 = ch["chapters"][2]

for s in c3["stages"]:
    st_idx = 20 + s["index"]
    p = json.load(open(f"tools/gemini/out/round-4b/player/stage_{st_idx}.json", encoding="utf-8"))
    p_top_tier = max(it["tier"] for it in p["items"])
    p_top_rarity = p["items"][0]["rarity"] # let's check properly
    rarity_order = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
    p_max_r_idx = max(rarity_order.index(it["rarity"]) for it in p["items"])
    p_max_r = rarity_order[p_max_r_idx]

    e = s["enemy"]
    e_tiers = [it["tier"] for it in e["items"]]
    e_rarities = [it["rarity"] for it in e["items"]]
    e_max_r_idx = max(rarity_order.index(r) for r in e_rarities)
    e_max_r = rarity_order[e_max_r_idx]

    # Check allowed ranges:
    # Tier: p_top_tier +- 1
    # Rarity: from p_max_r_idx - 1 to p_max_r_idx + (2 if elite/boss else 1)
    t_min = p_top_tier - 1
    t_max = p_top_tier + 1
    r_min_idx = max(0, p_max_r_idx - 1)
    r_max_idx = min(len(rarity_order) - 1, p_max_r_idx + (2 if s["kind"] in ("elite", "boss") else 1))

    print(f"Stage {st_idx:2d} ({s['kind']:5s}): recLv={s['recommendedLevel']}, enemyLv={e.get('level')}")
    print(f"   Player: lv={p['level']}, topTier={p_top_tier}, topRarity={p_max_r}")
    print(f"   Enemy:  tiers={set(e_tiers)} (allowed {t_min}..{t_max}), rarities={set(e_rarities)} (allowed {rarity_order[r_min_idx]}..{rarity_order[r_max_idx]})")
    print(f"   Items:  {[(it['id'], it['tier'], it['rarity']) for it in e['items']]}")
    print()
