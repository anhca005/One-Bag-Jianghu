import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p9 = json.load(open("tools/gemini/out/round-4b/player/stage_09.json", encoding="utf-8"))

# Original: thanh_phong_kiem(T2,epic), o_tam_y(T2,epic), phi_yen_quyet(T1,epic), tuy_tien_ho_lo(T1,epic)
tests_s9 = [
    # T2 epic armor, sword T1 epic, relics T1 epic
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "phi_yen_quyet", "x": 3, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "epic"}],
    # T2 epic armor, sword T2 rare, relics T1 rare
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "phi_yen_quyet", "x": 3, "y": 0, "tier": 1, "rarity": "rare"},
     {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "rare"}],
    # T2 epic armor, sword T2 epic, relics T1 rare
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "phi_yen_quyet", "x": 3, "y": 0, "tier": 1, "rarity": "rare"},
     {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "rare"}],
    # T2 epic armor, sword T2 epic, phi_yen_quyet T1 epic, tuy_tien_ho_lo T1 rare
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "phi_yen_quyet", "x": 3, "y": 0, "tier": 1, "rarity": "epic"},
     {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "rare"}],
]


for idx, items in enumerate(tests_s9, 1):
    e = {"name": "Ba Hổ Tiên Phong Vệ", "sect": "thai_thanh", "tamPhapLevel": 1, "level": 9, "items": items}
    wr, dur = simulate_stage(9, e, runs=200)
    print(f"S9 Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(9, 'normal', p9, e, wr, dur)}")
