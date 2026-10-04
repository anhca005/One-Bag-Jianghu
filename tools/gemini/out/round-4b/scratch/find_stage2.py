import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from sim_helper import simulate_enemy, ITEMS_DB

base_enemy = {
    "name": "Thủy Phỉ Thám Tử",
    "sect": None,
    "tamPhapLevel": 1,
    "level": 2,
    "items": []
}

# Weapons/items suitable for Thủy Phỉ Thám Tử:
# phuc_ho_con (staff), thanh_dang_tien (whip), thiet_cot_quyen_sao (fist), that_tinh_tieu (dart), tu_kim_trao (claw)
# thiet_giap_y (armor), boi_nguyen_dan, etc.
# Allowed enemy: T in [1..2], R in [common..uncommon]

tests = []
for w1 in ["phuc_ho_con", "thanh_dang_tien", "thiet_cot_quyen_sao", "that_tinh_tieu", "tu_kim_trao"]:
    for r1 in ["common", "uncommon"]:
        for t1 in [1, 2]:
            # with 2nd item
            for w2 in ["thiet_cot_quyen_sao", "that_tinh_tieu", "tu_kim_trao", "thiet_giap_y", "boi_nguyen_dan"]:
                for r2 in ["common", "uncommon"]:
                    # w1 at (0, 0), w2 at (2, 0)
                    tests.append([
                        {"id": w1, "x": 0, "y": 0, "tier": t1, "rarity": r1},
                        {"id": w2, "x": 2, "y": 0, "tier": 1, "rarity": r2}
                    ])

print(f"Total tests for stage 2: {len(tests)}")
results = []
for idx, items in enumerate(tests):
    e = dict(base_enemy)
    e["items"] = items
    try:
        wr, dur = simulate_enemy(2, e, runs=60)
        if 83.0 <= wr <= 87.0 and dur <= 28.0:
            results.append((wr, dur, items))
            print(f"HIT #{idx}: WR={wr:.1f}%, dur={dur:.1f}s | {[(it['id'], it['tier'], it['rarity']) for it in items]}")
    except Exception as exc:
        pass

print(f"\nTotal hits found: {len(results)}")
