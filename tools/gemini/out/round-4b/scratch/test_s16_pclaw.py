import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p16 = json.load(open("tools/gemini/out/round-4b/player/stage_16.json", encoding="utf-8"))

tests = [
    # kich_doc_chu_nga T3 rare + thiet_cot T3 rare + armor T3 leg
    [{"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
    # kich_doc_chu_nga T3 epic + thiet_cot T3 epic + armor T3 leg
    [{"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
    # kich_doc_chu_nga T3 rare + thiet_cot T3 leg + armor T3 leg
    [{"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
]

for idx, items in enumerate(tests, 1):
    e = {"name": "Hắc Sát Độc Hùng", "sect": None, "tamPhapLevel": 2, "level": 16, "items": items}
    wr, dur = simulate_stage(16, e, runs=200)
    print(f"Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(16, 'normal', p16, e, wr, dur)}")
