import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p23 = json.load(open("tools/gemini/out/round-4b/player/stage_23.json", encoding="utf-8"))

items_t5_leg = [
    {'id': 'soi_nanh_canh',     'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'kich_doc_chu_nga',  'x': 1, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'heo_rung_bap_giap', 'x': 2, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'tuyet_lang_bach_bi','x': 0, 'y': 2, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'da_ho_y',           'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'}
]

for lvl in [23, 24, 25]:
    for tp in [1, 2, 3]:
        e = {"name": "Tuyết Lang Vương", "sect": None, "tamPhapLevel": tp, "level": lvl, "items": items_t5_leg, "portrait": "monster_tuyet_lang"}
        wr, dur = simulate_stage(23, e, runs=200)
        print(f"lvl={lvl} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(23, 'normal', p23, e, wr, dur)}")
