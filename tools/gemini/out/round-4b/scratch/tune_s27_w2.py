import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p27 = json.load(open("tools/gemini/out/round-4b/player/stage_27.json", encoding="utf-8"))

def make_s27_w2(w2_id, w2_t, w2_r, armor_id="heo_rung_bap_giap", armor_t=5, armor_r="legendary"):
    return [
        {'id': 'u_minh_quy_trao', 'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': w2_id,              'x': 0, 'y': 2, 'tier': w2_t, 'rarity': w2_r},
        {'id': armor_id,           'x': 2, 'y': 0, 'tier': armor_t, 'rarity': armor_r},
        {'id': 'man_xa_doc_tui',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'u_minh_bi_luc',    'x': 4, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'van_doc_dinh',     'x': 4, 'y': 2, 'tier': 5, 'rarity': 'legendary'},
    ]

tests = [
    # tu_kim T4 epic
    ("tu_kim T4 epic", make_s27_w2("tu_kim_trao", 4, "epic")),
    # tu_kim T4 rare
    ("tu_kim T4 rare", make_s27_w2("tu_kim_trao", 4, "rare")),
    # tu_kim T3 epic
    ("tu_kim T3 epic", make_s27_w2("tu_kim_trao", 3, "epic")),
    # that_tinh_tieu T5 leg
    ("that_tinh T5 leg", make_s27_w2("that_tinh_tieu", 5, "legendary")),
    # that_tinh_tieu T5 epic
    ("that_tinh T5 epic", make_s27_w2("that_tinh_tieu", 5, "epic")),
    # that_tinh_tieu T4 epic
    ("that_tinh T4 epic", make_s27_w2("that_tinh_tieu", 4, "epic")),
]

for name, items in tests:
    e = {"name": "U Minh Quỷ Trảo", "sect": "huyen_doc", "tamPhapLevel": 1, "level": 27, "items": items, "portrait": "enemy_portrait_3"}
    wr, dur = simulate_stage(27, e, runs=100)
    print(f"{name:25s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(27, 'normal', p27, e, wr, dur)}")
