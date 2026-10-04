import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p23 = json.load(open("tools/gemini/out/round-4b/player/stage_23.json", encoding="utf-8"))

# Enemy 23: Tuyết Lang Vương (monster_tuyet_lang, sect=None)
# Level: 23
# Allowed tier: 3..5, rarity: rare..legendary

base_items = [
    {'id': 'soi_nanh_canh',     'x': 0, 'y': 0, 'tier': 4, 'rarity': 'legendary'},
    {'id': 'kich_doc_chu_nga',  'x': 1, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'heo_rung_bap_giap', 'x': 2, 'y': 0, 'tier': 4, 'rarity': 'legendary'},
    {'id': 'tuyet_lang_bach_bi','x': 0, 'y': 2, 'tier': 4, 'rarity': 'legendary'},
    {'id': 'da_ho_y',           'x': 4, 'y': 0, 'tier': 4, 'rarity': 'legendary'}
]

cands = [
    # Original items at level 23
    ("Orig items lv23", base_items),
    # T4 leg all
    ("T4 leg all", [
        {'id': 'soi_nanh_canh',     'x': 0, 'y': 0, 'tier': 4, 'rarity': 'legendary'},
        {'id': 'kich_doc_chu_nga',  'x': 1, 'y': 0, 'tier': 4, 'rarity': 'legendary'},
        {'id': 'heo_rung_bap_giap', 'x': 2, 'y': 0, 'tier': 4, 'rarity': 'legendary'},
        {'id': 'tuyet_lang_bach_bi','x': 0, 'y': 2, 'tier': 4, 'rarity': 'legendary'},
        {'id': 'da_ho_y',           'x': 4, 'y': 0, 'tier': 4, 'rarity': 'legendary'}
    ]),
    # T4 epic all
    ("T4 epic all", [
        {'id': 'soi_nanh_canh',     'x': 0, 'y': 0, 'tier': 4, 'rarity': 'epic'},
        {'id': 'kich_doc_chu_nga',  'x': 1, 'y': 0, 'tier': 4, 'rarity': 'epic'},
        {'id': 'heo_rung_bap_giap', 'x': 2, 'y': 0, 'tier': 4, 'rarity': 'epic'},
        {'id': 'tuyet_lang_bach_bi','x': 0, 'y': 2, 'tier': 4, 'rarity': 'epic'},
        {'id': 'da_ho_y',           'x': 4, 'y': 0, 'tier': 4, 'rarity': 'epic'}
    ]),
    # T5 epic all
    ("T5 epic all", [
        {'id': 'soi_nanh_canh',     'x': 0, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'kich_doc_chu_nga',  'x': 1, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'heo_rung_bap_giap', 'x': 2, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'tuyet_lang_bach_bi','x': 0, 'y': 2, 'tier': 5, 'rarity': 'epic'},
        {'id': 'da_ho_y',           'x': 4, 'y': 0, 'tier': 5, 'rarity': 'epic'}
    ]),
]

for name, items in cands:
    e = {"name": "Tuyết Lang Vương", "sect": None, "tamPhapLevel": 2, "level": 23, "items": items, "portrait": "monster_tuyet_lang"}
    wr, dur = simulate_stage(23, e, runs=100)
    print(f"{name:20s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(23, 'normal', p23, e, wr, dur)}")
