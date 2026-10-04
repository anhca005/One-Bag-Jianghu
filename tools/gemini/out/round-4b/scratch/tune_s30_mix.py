import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p30 = json.load(open("tools/gemini/out/round-4b/player/stage_30.json", encoding="utf-8"))

def make_s30_test(spear_t, spear_r, flute_t, flute_r, rest_t, rest_r, bnb=True):
    items = [
        {'id': 'bang_phach_thuong',  'x': 0, 'y': 0, 'tier': spear_t, 'rarity': spear_r},
        {'id': 'han_ngoc_tieu',      'x': 1, 'y': 0, 'tier': flute_t, 'rarity': flute_r},
        {'id': 'bang_tam_giap',      'x': 2, 'y': 0, 'tier': rest_t,  'rarity': rest_r},
        {'id': 'bang_tam_linh_chau', 'x': 4, 'y': 0, 'tier': rest_t,  'rarity': rest_r},
        {'id': 'han_bang_chau',      'x': 4, 'y': 1, 'tier': rest_t,  'rarity': rest_r},
        {'id': 'kiem_tong_tam_yeu',  'x': 4, 'y': 2, 'tier': rest_t,  'rarity': rest_r},
    ]
    if bnb:
        items.append({'id': 'bach_ngoc_boi', 'x': 2, 'y': 1, 'tier': rest_t, 'rarity': rest_r})
    return items

tests = [
    # All T6 epic with bnb
    ("All T6 epic with bnb", make_s30_test(6, "epic", 6, "epic", 6, "epic", bnb=True)),
    # All T6 epic NO bnb
    ("All T6 epic no bnb", make_s30_test(6, "epic", 6, "epic", 6, "epic", bnb=False)),
    # All T5 epic with bnb
    ("All T5 epic with bnb", make_s30_test(5, "epic", 5, "epic", 5, "epic", bnb=True)),
    # All T5 epic NO bnb
    ("All T5 epic no bnb", make_s30_test(5, "epic", 5, "epic", 5, "epic", bnb=False)),
    # T6 leg weps, rest T6 epic
    ("T6 leg weps, rest T6 epic", make_s30_test(6, "legendary", 6, "legendary", 6, "epic", bnb=True)),
    # T5 leg weps, rest T5 epic
    ("T5 leg weps, rest T5 epic", make_s30_test(5, "legendary", 5, "legendary", 5, "epic", bnb=True)),
]

for name, items in tests:
    for tp in [1, 2]:
        e = {"name": "Dạ Sát Thiên", "sect": "han_son", "tamPhapLevel": tp, "level": 31, "items": items, "characterId": "da_sat_thien", "portrait": "boss_da_sat_thien"}
        wr, dur = simulate_stage(30, e, runs=100)
        print(f"{name:30s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(30, 'boss', p30, e, wr, dur)}")
