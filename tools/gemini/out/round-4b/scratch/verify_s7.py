import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p7 = json.load(open("tools/gemini/out/round-4b/player/stage_07.json", encoding="utf-8"))
cands = [
    # HIT 2: phan_quan_but epic, o_thiet_giap epic, blade epic
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "phan_quan_but", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 2, "rarity": "epic"}],
    # phan_quan_but epic, heo_rung_bap_giap epic, blade rare
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "phan_quan_but", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "epic"}],
    # phan_quan_but rare, heo_rung_bap_giap epic, blade epic
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "phan_quan_but", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "epic"}],
    # thiet_cot_quyen_sao epic, heo_rung_bap_giap rare, blade epic at level 8
    [{"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}],
]

for idx, items in enumerate(cands, 1):
    lvl = 8 if idx == 4 else 7
    e = {"name": "Liệt Diễm Bang Tinh Anh", "sect": None, "tamPhapLevel": 1, "level": lvl, "items": items}
    wr, dur = simulate_stage(7, e, runs=200)
    print(f"Cand {idx} (lvl {lvl}): WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(7, 'normal', p7, e, wr, dur)}")

