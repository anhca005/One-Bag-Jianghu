import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p27 = json.load(open("tools/gemini/out/round-4b/player/stage_27.json", encoding="utf-8"))

def make_s27(t, r, w_t=None, w_r=None):
    wt = w_t or t
    wr = w_r or r
    return [
        {'id': 'u_minh_quy_trao', 'x': 0, 'y': 0, 'tier': wt, 'rarity': wr},
        {'id': 'da_ho_y',          'x': 2, 'y': 0, 'tier': t,  'rarity': r},
        {'id': 'man_xa_doc_tui',   'x': 3, 'y': 0, 'tier': t,  'rarity': r},
        {'id': 'u_minh_bi_luc',    'x': 3, 'y': 1, 'tier': t,  'rarity': r},
        {'id': 'van_doc_dinh',     'x': 4, 'y': 0, 'tier': t,  'rarity': r}
    ]

cands = [
    ("T6 supreme all", make_s27(6, "supreme")),
    ("T6 leg all", make_s27(6, "legendary")),
    ("T5 leg all", make_s27(5, "legendary")),
    ("T5 epic all", make_s27(5, "epic")),
    ("T6 sup wep, T5 leg rest", make_s27(5, "legendary", w_t=6, w_r="supreme")),
    ("T6 leg wep, T5 leg rest", make_s27(5, "legendary", w_t=6, w_r="legendary")),
    ("T6 leg wep, T5 epic rest", make_s27(5, "epic", w_t=6, w_r="legendary")),
]

for name, items in cands:
    for tp in [1, 2]:
        e = {"name": "U Minh Quỷ Trảo", "sect": "huyen_doc", "tamPhapLevel": tp, "level": 27, "items": items, "portrait": "enemy_portrait_3"}
        wr, dur = simulate_stage(27, e, runs=100)
        print(f"{name:30s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(27, 'normal', p27, e, wr, dur)}")
