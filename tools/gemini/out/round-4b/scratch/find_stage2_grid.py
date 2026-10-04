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

weapons = ["phuc_ho_con", "thanh_dang_tien", "thiet_cot_quyen_sao", "that_tinh_tieu", "tu_kim_trao", "thanh_phong_kiem"]

tests = []
# 2 weapons + thiet_giap_y
for w1 in weapons:
    for w2 in weapons:
        if w1 == w2:
            continue
        for t1 in [1, 2]:
            for r1 in ["common", "uncommon"]:
                for t2 in [1]:
                    for r2 in ["common"]:
                        tests.append([
                            {"id": w1, "x": 0, "y": 0, "tier": t1, "rarity": r1},
                            {"id": w2, "x": 1, "y": 0, "tier": t2, "rarity": r2},
                            {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
                        ])

print(f"Total tests: {len(tests)}")
hits = []
for idx, items in enumerate(tests):
    e = dict(base_enemy)
    e["items"] = items
    try:
        wr, dur = simulate_enemy(2, e, runs=60)
        if 80.0 <= wr <= 90.0 and dur <= 30.0:
            hits.append((wr, dur, items))
            print(f"HIT #{idx}: WR={wr:.1f}%, dur={dur:.1f}s | {[(it['id'], it['tier'], it['rarity']) for it in items]}")
    except Exception as exc:
        pass

print(f"\nTotal hits found: {len(hits)}")
