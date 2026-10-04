import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p28 = json.load(open("tools/gemini/out/round-4b/player/stage_28.json", encoding="utf-8"))

def make_s28_5(st, sr, ft, fr, at, ar, ct, cr, yt, yr):
    return [
        {'id': 'bang_tam_giap',       'x': 0, 'y': 0, 'tier': at, 'rarity': ar},
        {'id': 'bang_phach_thuong',   'x': 4, 'y': 0, 'tier': st, 'rarity': sr},
        {'id': 'han_ngoc_tieu',       'x': 2, 'y': 0, 'tier': ft, 'rarity': fr},
        {'id': 'han_bang_chau',       'x': 3, 'y': 0, 'tier': ct, 'rarity': cr},
        {'id': 'kiem_tong_tam_yeu',   'x': 3, 'y': 1, 'tier': yt, 'rarity': yr},
    ]

tests = [
    # All T4 epic
    ("All T4 epic", make_s28_5(4, "epic", 4, "epic", 4, "epic", 4, "epic", 4, "epic")),
    # Spear T5, rest T4 epic
    ("Spear T5, rest T4 epic", make_s28_5(5, "epic", 4, "epic", 4, "epic", 4, "epic", 4, "epic")),
    # Flute T5, rest T4 epic
    ("Flute T5, rest T4 epic", make_s28_5(4, "epic", 5, "epic", 4, "epic", 4, "epic", 4, "epic")),
    # tamyeu T4, rest T5 epic
    ("tamyeu T4, rest T5 epic", make_s28_5(5, "epic", 5, "epic", 5, "epic", 5, "epic", 4, "epic")),
    # Spear T4, Flute T4, rest T5 epic
    ("weps T4, rest T5 epic", make_s28_5(4, "epic", 4, "epic", 5, "epic", 5, "epic", 5, "epic")),
]

for name, items in tests:
    e = {"name": "Hàn Băng Thần Quy", "sect": None, "tamPhapLevel": 1, "level": 28, "items": items, "portrait": "monster_than_quy"}
    wr, dur = simulate_stage(28, e, runs=200)
    print(f"{name:30s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(28, 'normal', p28, e, wr, dur)}")
