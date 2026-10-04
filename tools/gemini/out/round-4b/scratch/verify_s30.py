import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p30 = json.load(open("tools/gemini/out/round-4b/player/stage_30.json", encoding="utf-8"))

def make_s30(st, sr, ft, fr, at, ar, ct, cr, yt, yr, lt, lr):
    return [
        {'id': 'bang_phach_thuong',  'x': 0, 'y': 0, 'tier': st, 'rarity': sr},
        {'id': 'han_ngoc_tieu',      'x': 1, 'y': 0, 'tier': ft, 'rarity': fr},
        {'id': 'bang_tam_giap',      'x': 2, 'y': 0, 'tier': at, 'rarity': ar},
        {'id': 'bang_tam_linh_chau', 'x': 4, 'y': 0, 'tier': lt, 'rarity': lr},
        {'id': 'han_bang_chau',      'x': 4, 'y': 1, 'tier': ct, 'rarity': cr},
        {'id': 'kiem_tong_tam_yeu',  'x': 4, 'y': 2, 'tier': yt, 'rarity': yr},
    ]

tests = [
    # spear T6 leg, armor T5 epic, rest T6 epic
    ("Spear T6 leg, armor T5 epic", make_s30(6, "legendary", 6, "epic", 5, "epic", 6, "epic", 6, "epic", 6, "epic")),
    # spear T6 leg, armor T5 leg, rest T5 epic
    ("Spear T6 leg, armor T5 leg, rest T5 epic", make_s30(6, "legendary", 5, "epic", 5, "legendary", 5, "epic", 5, "epic", 5, "epic")),
    # spear T6 epic, armor T5 epic, flute T6 leg
    ("Flute T6 leg, armor T5 epic", make_s30(6, "epic", 6, "legendary", 5, "epic", 6, "epic", 6, "epic", 6, "epic")),
    # All T6 epic, armor T4 epic (lower stall)
    ("All T6 epic, armor T4 epic", make_s30(6, "epic", 6, "epic", 4, "epic", 6, "epic", 6, "epic", 6, "epic")),
    # spear T6 leg, all else T5 epic
    ("Spear T6 leg, rest T5 epic", make_s30(6, "legendary", 5, "epic", 5, "epic", 5, "epic", 5, "epic", 5, "epic")),
]

for name, items in tests:
    e = {"name": "Dạ Sát Thiên", "sect": "han_son", "tamPhapLevel": 1, "level": 31, "items": items, "characterId": "da_sat_thien", "portrait": "boss_da_sat_thien"}
    wr, dur = simulate_stage(30, e, runs=200)
    print(f"{name:40s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(30, 'boss', p30, e, wr, dur)}")
