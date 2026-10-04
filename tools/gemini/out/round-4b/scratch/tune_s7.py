import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p7 = json.load(open("tools/gemini/out/round-4b/player/stage_07.json", encoding="utf-8"))

# Original: liet_diem_dao (T2 epic), thiet_cot_quyen_sao (T2 epic), heo_rung_bap_giap (T2 epic)
tests = [
    # Cand 2 with cuong_hoa_dan (T1 common)
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "cuong_hoa_dan", "x": 4, "y": 0, "tier": 1, "rarity": "common"}],
    # Cand 2 with bach_ngoc_boi (T1 common)
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "bach_ngoc_boi", "x": 4, "y": 0, "tier": 1, "rarity": "common"}],
    # Cand 2 with chu_sa_ho_lo (T1 common)
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "chu_sa_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "common"}],
]




for idx, items in enumerate(tests, 1):
    e = {"name": "Liệt Diễm Bang Tinh Anh", "sect": None, "tamPhapLevel": 1, "level": 7, "items": items}
    wr, dur = simulate_stage(7, e, runs=200)
    print(f"S7 Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(7, 'normal', p7, e, wr, dur)}")
