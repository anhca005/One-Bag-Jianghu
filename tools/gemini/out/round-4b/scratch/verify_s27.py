import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p27 = json.load(open("tools/gemini/out/round-4b/player/stage_27.json", encoding="utf-8"))

def make_s27_single(w_r="legendary", bnb_r="legendary", daho_r="legendary"):
    return [
        {'id': 'u_minh_quy_trao', 'x': 0, 'y': 0, 'tier': 5, 'rarity': w_r},
        {'id': 'bach_ngoc_boi',    'x': 0, 'y': 2, 'tier': 5, 'rarity': bnb_r},
        {'id': 'da_ho_y',          'x': 2, 'y': 0, 'tier': 5, 'rarity': daho_r},
        {'id': 'man_xa_doc_tui',   'x': 3, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'u_minh_bi_luc',    'x': 3, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'van_doc_dinh',     'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    ]

tests = [
    # w supreme, bnb leg, daho leg
    ("w supreme, bnb leg", make_s27_single(w_r="supreme")),
    # w supreme, bnb supreme, daho leg
    ("w supreme, bnb supreme", make_s27_single(w_r="supreme", bnb_r="supreme")),
    # w leg, bnb supreme, daho leg
    ("w leg, bnb supreme", make_s27_single(bnb_r="supreme")),
    # w supreme, bnb leg, daho supreme
    ("w supreme, bnb leg, daho supreme", make_s27_single(w_r="supreme", daho_r="supreme")),
    # w leg, bnb leg, daho supreme
    ("w leg, bnb leg, daho supreme", make_s27_single(daho_r="supreme")),
]

for name, items in tests:
    e = {"name": "U Minh Quỷ Trảo", "sect": "huyen_doc", "tamPhapLevel": 1, "level": 27, "items": items, "portrait": "enemy_portrait_3"}
    wr, dur = simulate_stage(27, e, runs=200)
    print(f"{name:35s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(27, 'normal', p27, e, wr, dur)}")
