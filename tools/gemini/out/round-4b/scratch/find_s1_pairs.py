import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p1 = json.load(open("tools/gemini/out/round-4b/player/stage_01.json", encoding="utf-8"))
weapons = [
    "thanh_dang_tien", "tu_kim_trao", "thiet_cot_quyen_sao", "that_tinh_tieu",
    "phuc_ho_con", "phan_quan_but", "ngoc_cot_phien", "thanh_phong_kiem"
]

tests = []
for i in range(len(weapons)):
    for j in range(i, len(weapons)):
        w1, w2 = weapons[i], weapons[j]
        for r1 in ["common", "uncommon"]:
            for r2 in ["common"]:
                tests.append([
                    {"id": w1, "x": 0, "y": 0, "tier": 1, "rarity": r1},
                    {"id": w2, "x": 1, "y": 0, "tier": 1, "rarity": r2}
                ])

print(f"Total pair tests: {len(tests)}")
hits = []
for idx, items in enumerate(tests):
    e1 = {"name": "Thảo Khấu Tiên Phong", "sect": None, "tamPhapLevel": 1, "level": 1, "items": items}
    wr, dur = simulate_stage(1, e1, runs=60)
    if 82.0 <= wr <= 88.0 and dur <= 25.0:
        hits.append((wr, dur, items))
        print(f"HIT #{idx}: WR={wr:.1f}%, dur={dur:.1f}s | {[(it['id'], it['rarity']) for it in items]}")

print(f"\nTotal hits found: {len(hits)}")
