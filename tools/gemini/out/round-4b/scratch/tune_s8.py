import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p8 = json.load(open("tools/gemini/out/round-4b/player/stage_08.json", encoding="utf-8"))

# Original items: liet_diem_dao(T3,epic), o_thiet_giap(T3,epic), liet_hoa_tam_kinh(T3,epic), cuong_hoa_dan(T3,epic)
# Allowed E: level 8, T in [1..3], R in [uncommon..epic]

tests_s8 = [
    # T2 epic armor, rest T3 epic
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
     {"id": "o_thiet_giap", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "liet_hoa_tam_kinh", "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
     {"id": "cuong_hoa_dan", "x": 4, "y": 0, "tier": 3, "rarity": "epic"}],
    # all T2 epic
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "o_thiet_giap", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "liet_hoa_tam_kinh", "x": 3, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "cuong_hoa_dan", "x": 4, "y": 0, "tier": 2, "rarity": "epic"}],
    # T3 rare blade, T2 epic armor, T2 rare manual, T2 rare pill
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "o_thiet_giap", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "liet_hoa_tam_kinh", "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "cuong_hoa_dan", "x": 4, "y": 0, "tier": 2, "rarity": "rare"}],
    # T3 epic blade, T2 rare armor, T2 rare manual, T2 rare pill
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
     {"id": "o_thiet_giap", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "liet_hoa_tam_kinh", "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "cuong_hoa_dan", "x": 4, "y": 0, "tier": 2, "rarity": "rare"}],
]

for idx, items in enumerate(tests_s8, 1):
    e = {"name": "Liệt Diễm Bang Hương Chủ", "sect": "liet_diem", "tamPhapLevel": 1, "level": 8, "items": items}
    wr, dur = simulate_stage(8, e, runs=200)
    print(f"S8 Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(8, 'normal', p8, e, wr, dur)}")
