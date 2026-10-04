import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p26 = json.load(open("tools/gemini/out/round-4b/player/stage_26.json", encoding="utf-8"))

# Test offensive variants with heo_rung_bap_giap
# Grid:
# col 0-1: bach_ho_cuong_mong (2x1 at y=0)
# col 0: thanh_dong_ho_tam_kinh (1x1 at y=1)
# col 1: bach_ngoc_boi (1x1 at y=1)
# col 2-3: heo_rung_bap_giap (2x2 at y=0)
# col 4: thiet_cot_quyen_sao (1x2 at y=0)

tests = [
    # Supreme claws, leg fist, epic armor & relics
    ("Supreme claws, leg fist, epic rest", [
        {'id': 'bach_ho_cuong_mong',      'x': 0, 'y': 0, 'tier': 5, 'rarity': 'supreme'},
        {'id': 'thanh_dong_ho_tam_kinh',  'x': 0, 'y': 1, 'tier': 4, 'rarity': 'epic'},
        {'id': 'bach_ngoc_boi',           'x': 1, 'y': 1, 'tier': 4, 'rarity': 'epic'},
        {'id': 'heo_rung_bap_giap',       'x': 2, 'y': 0, 'tier': 4, 'rarity': 'epic'},
        {'id': 'thiet_cot_quyen_sao',     'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    ]),
    # Supreme claws, supreme fist, epic rest
    ("Supreme claws, supreme fist, epic rest", [
        {'id': 'bach_ho_cuong_mong',      'x': 0, 'y': 0, 'tier': 5, 'rarity': 'supreme'},
        {'id': 'thanh_dong_ho_tam_kinh',  'x': 0, 'y': 1, 'tier': 4, 'rarity': 'epic'},
        {'id': 'bach_ngoc_boi',           'x': 1, 'y': 1, 'tier': 4, 'rarity': 'epic'},
        {'id': 'heo_rung_bap_giap',       'x': 2, 'y': 0, 'tier': 4, 'rarity': 'epic'},
        {'id': 'thiet_cot_quyen_sao',     'x': 4, 'y': 0, 'tier': 5, 'rarity': 'supreme'},
    ]),
    # Leg claws, leg fist, leg rest
    ("Leg claws, leg fist, leg rest", [
        {'id': 'bach_ho_cuong_mong',      'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'thanh_dong_ho_tam_kinh',  'x': 0, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'bach_ngoc_boi',           'x': 1, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'heo_rung_bap_giap',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'thiet_cot_quyen_sao',     'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    ]),
    # Leg claws, leg fist, leg armor, no relics
    ("Leg claws, leg fist, leg armor only", [
        {'id': 'bach_ho_cuong_mong',      'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'heo_rung_bap_giap',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'thiet_cot_quyen_sao',     'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    ]),
]

for name, items in tests:
    for tp in [1, 2]:
        e = {"name": "Bạch Hổ Tông Sư", "sect": None, "tamPhapLevel": tp, "level": 26, "items": items, "portrait": "enemy_portrait_2"}
        wr, dur = simulate_stage(26, e, runs=100)
        print(f"{name:40s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(26, 'normal', p26, e, wr, dur)}")
