import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p26 = json.load(open("tools/gemini/out/round-4b/player/stage_26.json", encoding="utf-8"))

tests = [
    # fist T4 epic, mirror T5 leg
    ("fist T4 epic, mirror T5 leg", [
        {'id': 'bach_ho_cuong_mong',      'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'thanh_dong_ho_tam_kinh',  'x': 0, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'bach_ngoc_boi',           'x': 1, 'y': 1, 'tier': 5, 'rarity': 'epic'},
        {'id': 'heo_rung_bap_giap',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'thiet_cot_quyen_sao',     'x': 4, 'y': 0, 'tier': 4, 'rarity': 'epic'},
    ]),
    # fist T4 leg, mirror T4 leg
    ("fist T4 leg, mirror T4 leg", [
        {'id': 'bach_ho_cuong_mong',      'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'thanh_dong_ho_tam_kinh',  'x': 0, 'y': 1, 'tier': 4, 'rarity': 'legendary'},
        {'id': 'bach_ngoc_boi',           'x': 1, 'y': 1, 'tier': 5, 'rarity': 'epic'},
        {'id': 'heo_rung_bap_giap',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'thiet_cot_quyen_sao',     'x': 4, 'y': 0, 'tier': 4, 'rarity': 'legendary'},
    ]),
    # mirror T3 leg, boi epic
    ("mirror T3 leg, boi epic", [
        {'id': 'bach_ho_cuong_mong',      'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'thanh_dong_ho_tam_kinh',  'x': 0, 'y': 1, 'tier': 3, 'rarity': 'legendary'},
        {'id': 'bach_ngoc_boi',           'x': 1, 'y': 1, 'tier': 5, 'rarity': 'epic'},
        {'id': 'heo_rung_bap_giap',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'thiet_cot_quyen_sao',     'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    ]),
]

for name, items in tests:
    e = {"name": "Bạch Hổ Tông Sư", "sect": None, "tamPhapLevel": 1, "level": 26, "items": items, "portrait": "enemy_portrait_2"}
    wr, dur = simulate_stage(26, e, runs=200)
    print(f"{name:30s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(26, 'normal', p26, e, wr, dur)}")
