import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p28 = json.load(open("tools/gemini/out/round-4b/player/stage_28.json", encoding="utf-8"))

def make_s28_test(st, sr, ft, fr, rt, rr, chau=True, tamyeu=True):
    items = [
        {'id': 'bang_tam_giap',       'x': 0, 'y': 0, 'tier': rt, 'rarity': rr},
        {'id': 'bang_phach_thuong',   'x': 4, 'y': 0, 'tier': st, 'rarity': sr},
        {'id': 'han_ngoc_tieu',       'x': 2, 'y': 0, 'tier': ft, 'rarity': fr},
        {'id': 'han_bang_chau',       'x': 3, 'y': 0, 'tier': rt, 'rarity': rr},
    ]
    if tamyeu:
        items.append({'id': 'kiem_tong_tam_yeu', 'x': 3, 'y': 1, 'tier': rt, 'rarity': rr})
    if chau:
        items.append({'id': 'bang_tam_linh_chau', 'x': 0, 'y': 1, 'tier': rt, 'rarity': rr})
    return items

tests = [
    # All epic
    ("All T5 epic", make_s28_test(5, "epic", 5, "epic", 5, "epic")),
    ("All T4 epic", make_s28_test(4, "epic", 4, "epic", 4, "epic")),
    # T5 epic without bang_tam_linh_chau
    ("T5 epic, no linh_chau", make_s28_test(5, "epic", 5, "epic", 5, "epic", chau=False)),
    # T5 epic without tam_yeu
    ("T5 epic, no tam_yeu", make_s28_test(5, "epic", 5, "epic", 5, "epic", tamyeu=False)),
    # T5 epic without either (only 4 items)
    ("T5 epic, 4 items", make_s28_test(5, "epic", 5, "epic", 5, "epic", chau=False, tamyeu=False)),
    # T5 leg, 4 items
    ("T5 leg, 4 items", make_s28_test(5, "legendary", 5, "legendary", 5, "legendary", chau=False, tamyeu=False)),
]

for name, items in tests:
    e = {"name": "Hàn Băng Thần Quy", "sect": None, "tamPhapLevel": 1, "level": 28, "items": items, "portrait": "monster_than_quy"}
    wr, dur = simulate_stage(28, e, runs=100)
    print(f"{name:25s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(28, 'normal', p28, e, wr, dur)}")
