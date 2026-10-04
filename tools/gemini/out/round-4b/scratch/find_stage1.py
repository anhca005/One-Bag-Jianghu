import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from sim_helper import simulate_enemy, ITEMS_DB

base_enemy = {
    "name": "Thảo Khấu Tiên Phong",
    "sect": None,
    "tamPhapLevel": 1,
    "level": 1,
    "items": []
}

# Weapons suitable for Thảo Khấu:
# thanh_dang_tien (whip), thiet_cot_quyen_sao (fist), that_tinh_tieu (dart), thanh_phong_kiem (sword), tu_kim_trao (claw)
# Armors/relics/pills:
# thiet_giap_y, o_tam_y, boi_nguyen_dan, kim_sang_duoc, bach_ngoc_boi

# Let's test combinations with different tiers (1..2) and rarities (common..uncommon)
tests = []

# Try dual weapons or weapon + relic/pill
for w1 in ["thanh_dang_tien", "thiet_cot_quyen_sao", "that_tinh_tieu", "tu_kim_trao", "thanh_phong_kiem"]:
    for r1 in ["common", "uncommon"]:
        for t1 in [1, 2]:
            # with armor
            tests.append([
                {"id": w1, "x": 0, "y": 0, "tier": t1, "rarity": r1},
                {"id": "thiet_giap_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}
            ])
            # with 2nd offensive item
            tests.append([
                {"id": w1, "x": 0, "y": 0, "tier": t1, "rarity": r1},
                {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 1, "rarity": "common"}
            ])
            # with that_tinh_tieu
            tests.append([
                {"id": w1, "x": 0, "y": 0, "tier": t1, "rarity": r1},
                {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 1, "rarity": "common"}
            ])
            # with tu_kim_trao
            tests.append([
                {"id": w1, "x": 0, "y": 0, "tier": t1, "rarity": r1},
                {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 1, "rarity": "common"}
            ])

print(f"Total tests: {len(tests)}")
results = []
for idx, items in enumerate(tests):
    e = dict(base_enemy)
    e["items"] = items
    try:
        wr, dur = simulate_enemy(1, e, runs=60)
        if 80.0 <= wr <= 90.0 and dur <= 30.0:
            results.append((wr, dur, items))
            print(f"HIT #{idx}: WR={wr:.1f}%, dur={dur:.1f}s | {[(it['id'], it['tier'], it['rarity']) for it in items]}")
    except Exception as exc:
        pass

print(f"\nTotal hits found: {len(results)}")
