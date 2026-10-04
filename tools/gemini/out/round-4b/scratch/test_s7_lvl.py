import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p7 = json.load(open("tools/gemini/out/round-4b/player/stage_07.json", encoding="utf-8"))

cand_2 = [
    {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
    {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}
]

tests = [
    # lvl 7, fist T3 epic
    (7, [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
         {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
         {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}]),
    # lvl 7, fist T3 rare
    (7, [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
         {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
         {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}]),
    # lvl 8, fist T3 rare
    (8, [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
         {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
         {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}]),
]

for lvl, items in tests:
    e = {"name": "Liệt Diễm Bang Tinh Anh", "sect": None, "tamPhapLevel": 1, "level": lvl, "items": items}
    wr, dur = simulate_stage(7, e, runs=200)
    print(f"lvl={lvl}, fist T3: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(7, 'normal', p7, e, wr, dur)}")

