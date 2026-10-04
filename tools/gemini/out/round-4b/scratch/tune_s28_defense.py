import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p28 = json.load(open("tools/gemini/out/round-4b/player/stage_28.json", encoding="utf-8"))

tests = [
    # Option 1: bang_tam_giap (2x1) instead of thien_nien_quy_giap
    ("bang_tam_giap + bang_phach_co_cot T5 epic", [
        {'id': 'bang_phach_co_cot',   'x': 0, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'bang_tam_giap',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'bang_phach_thuong',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'han_ngoc_tieu',       'x': 0, 'y': 2, 'tier': 5, 'rarity': 'epic'},
        {'id': 'han_bang_chau',       'x': 1, 'y': 2, 'tier': 5, 'rarity': 'epic'}
    ]),
    # Option 2: thien_nien_quy_giap but NO bang_phach_co_cot
    ("thien_nien_quy_giap only T5 epic", [
        {'id': 'thien_nien_quy_giap', 'x': 0, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'bang_phach_thuong',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'han_ngoc_tieu',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'han_bang_chau',       'x': 3, 'y': 0, 'tier': 5, 'rarity': 'epic'}
    ]),
    # Option 3: bang_tam_giap + NO co_cot
    ("bang_tam_giap + no co_cot T5 epic", [
        {'id': 'bang_tam_giap',       'x': 0, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'bang_phach_thuong',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'han_ngoc_tieu',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'han_bang_chau',       'x': 3, 'y': 0, 'tier': 5, 'rarity': 'epic'}
    ]),
    # Option 4: bang_tam_giap + no co_cot T5 leg
    ("bang_tam_giap + no co_cot T5 leg", [
        {'id': 'bang_tam_giap',       'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'bang_phach_thuong',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'han_ngoc_tieu',       'x': 2, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'han_bang_chau',       'x': 3, 'y': 0, 'tier': 5, 'rarity': 'legendary'}
    ]),
]

for name, items in tests:
    e = {"name": "Hàn Băng Thần Quy", "sect": None, "tamPhapLevel": 1, "level": 28, "items": items, "portrait": "monster_than_quy"}
    wr, dur = simulate_stage(28, e, runs=100)
    print(f"{name:40s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(28, 'normal', p28, e, wr, dur)}")
