import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p27 = json.load(open("tools/gemini/out/round-4b/player/stage_27.json", encoding="utf-8"))

# Enemy 27:
# Grid 5x4:
# col 0-1: u_minh_quy_trao (2x2 at y=0)
# col 2-3: armor (o_thiet_giap 2x2 at y=0) or (heo_rung 2x2)
# col 4: man_xa_doc_tui (1x1 at y=0), u_minh_bi_luc (1x1 at y=1), van_doc_dinh (1x1 at y=2)
# col 0: tu_kim_trao (1x2 at y=2) or similar

tests = [
    # Armor o_thiet_giap T5 leg
    ("o_thiet_giap T5 leg", [
        {'id': 'u_minh_quy_trao', 'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'o_thiet_giap',     'x': 2, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'man_xa_doc_tui',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'u_minh_bi_luc',    'x': 4, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'van_doc_dinh',     'x': 4, 'y': 2, 'tier': 5, 'rarity': 'legendary'},
    ]),
    # Armor heo_rung T5 leg
    ("heo_rung T5 leg", [
        {'id': 'u_minh_quy_trao', 'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'heo_rung_bap_giap','x': 2, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'man_xa_doc_tui',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'u_minh_bi_luc',    'x': 4, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'van_doc_dinh',     'x': 4, 'y': 2, 'tier': 5, 'rarity': 'legendary'},
    ]),
    # Armor heo_rung T5 leg + tu_kim_trao T5 leg at x=0, y=2
    ("heo_rung T5 leg + tu_kim T5 leg", [
        {'id': 'u_minh_quy_trao', 'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'tu_kim_trao',      'x': 0, 'y': 2, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'heo_rung_bap_giap','x': 2, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'man_xa_doc_tui',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'u_minh_bi_luc',    'x': 4, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'van_doc_dinh',     'x': 4, 'y': 2, 'tier': 5, 'rarity': 'legendary'},
    ]),
]

for name, items in tests:
    for tp in [1, 2]:
        e = {"name": "U Minh Quỷ Trảo", "sect": "huyen_doc", "tamPhapLevel": tp, "level": 27, "items": items, "portrait": "enemy_portrait_3"}
        wr, dur = simulate_stage(27, e, runs=100)
        print(f"{name:35s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(27, 'normal', p27, e, wr, dur)}")
