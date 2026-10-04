import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p16 = json.load(open("tools/gemini/out/round-4b/player/stage_16.json", encoding="utf-8"))
# Give the bear claw weapons:
# hac_sat_hung_dam, heo_rung_bap_giap, tu_kim_trao, thiet_cot_quyen_sao (or kich_doc_chu_nga)
# Check grid layout: 5x4
# tu_kim_trao: [1, 2], thiet_cot_quyen_sao: [1, 2], heo_rung_bap_giap: [2, 2], hac_sat_hung_dam: [1, 1], thanh_dong: [1, 1]

tests = [
    # Cand 1 with heo_rung T4 legendary
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
    # Cand 1 with hac_sat_hung_dam T4
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
    # Cand 1 with venom bag (man_xa_doc_tui)
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "man_xa_doc_tui", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
]


for idx, items in enumerate(tests, 1):
    e = {"name": "Hắc Sát Độc Hùng", "sect": None, "tamPhapLevel": 2, "level": 16, "items": items}
    wr, dur = simulate_stage(16, e, runs=200)
    print(f"Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(16, 'normal', p16, e, wr, dur)}")
