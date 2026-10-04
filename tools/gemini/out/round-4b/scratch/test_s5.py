import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p5 = json.load(open("tools/gemini/out/round-4b/player/stage_05.json", encoding="utf-8"))
# Original enemy items in stage 5:
# thanh_phong_kiem(T1,legendary), phi_yen_quyet(T1,supreme), kim_tuyen_y(T1,epic), tuy_tien_ho_lo(T1,legendary)
# Max rarity allowed: legendary (rare + 2)

tests_s5 = [
    # phi_yen_quyet set to legendary
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "legendary"},
     {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 1, "rarity": "legendary"},
     {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "legendary"}],
    # phi_yen_quyet set to epic
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "legendary"},
     {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "legendary"}],
    # all epic
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "epic"}],
]

for idx, items in enumerate(tests_s5, 1):
    # also test enemy level: 5 vs 6 (rec is 5, 5+-2 allows both, but 5 keeps level progression smooth)
    for lvl in [5, 6]:
        e = {
            "name": "Phong Lôi Khoái Đao Phong Vũ", "sect": "thai_thanh", "tamPhapLevel": 1, "level": lvl,
            "items": items
        }
        wr, dur = simulate_stage(5, e, runs=200)
        print(f"S5 Cand {idx} (lvl {lvl}): WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(5, 'elite', p5, e, wr, dur)}")
