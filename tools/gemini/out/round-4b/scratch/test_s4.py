import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

# Stage 04
p4 = json.load(open("tools/gemini/out/round-4b/player/stage_04.json", encoding="utf-8"))
# Original items: liet_diem_dao(T3,supreme), thiet_giap_y(T3,supreme), thuan_duong_cong(T3,supreme)
# Allowed E: T in [1..2], R in [common..rare]

tests_s4 = [
    # liet_diem_dao (T2 rare) + that_tinh_tieu (T1 common) + o_thiet_giap (T2 rare)
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}],
    # liet_diem_dao (T2 rare) + thiet_cot_quyen_sao (T1 common) + o_thiet_giap (T2 rare)
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}],
    # liet_diem_dao (T2 rare) + thiet_cot_quyen_sao (T1 uncommon) + o_thiet_giap (T1 rare)
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
     {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 1, "rarity": "rare"}],
    # liet_diem_dao (T2 rare) + that_tinh_tieu (T1 uncommon) + o_thiet_giap (T2 rare)
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
     {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}],
]




for idx, items in enumerate(tests_s4, 1):
    e = {
        "name": "Liệt Diễm Bang Chúng", "sect": "liet_diem", "tamPhapLevel": 1, "level": 4,
        "items": items
    }
    wr, dur = simulate_stage(4, e, runs=200)
    print(f"S4 Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(4, 'normal', p4, e, wr, dur)}")
