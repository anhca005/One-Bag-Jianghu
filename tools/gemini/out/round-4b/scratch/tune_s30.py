import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p30 = json.load(open("tools/gemini/out/round-4b/player/stage_30.json", encoding="utf-8"))

# Enemy 30: Dạ Sát Thiên (characterId: "da_sat_thien", sect: "han_son")
# Boss level MUST be: recommendedLevel + 1 = 30 + 1 = 31!
# Player 30 top tier: 5, top rarity: legendary
# Allowed boss tier: 4..6, rarity: epic..supreme
# Target WR: [52, 58]%, dur <= 40s

# Let's test offensive & balanced boss builds:
# bang_phach_thuong: 1x4 at x=0, y=0
# han_ngoc_tieu: 1x2 or second weapon like thanh_phong_kiem (1x3)
# armor: bang_tam_giap (2x1) or thien_nien_quy_giap (2x2)
# relics/manuals: bang_tam_linh_chau, han_bang_chau, kiem_tong_tam_yeu, cuu_chuyen_hoan_hon_dan

cands = [
    # C1: Without turtle shell, using bang_tam_giap + dual weapons (spear + flute)
    ("Spear + Flute + bang_tam T6 leg", [
        {'id': 'bang_phach_thuong',  'x': 0, 'y': 0, 'tier': 6, 'rarity': 'legendary'},
        {'id': 'han_ngoc_tieu',      'x': 1, 'y': 0, 'tier': 6, 'rarity': 'legendary'},
        {'id': 'bang_tam_giap',      'x': 2, 'y': 0, 'tier': 6, 'rarity': 'legendary'},
        {'id': 'bang_tam_linh_chau', 'x': 4, 'y': 0, 'tier': 6, 'rarity': 'legendary'},
        {'id': 'han_bang_chau',      'x': 4, 'y': 1, 'tier': 6, 'rarity': 'legendary'},
        {'id': 'kiem_tong_tam_yeu',  'x': 4, 'y': 2, 'tier': 6, 'rarity': 'legendary'},
        {'id': 'bach_ngoc_boi',      'x': 2, 'y': 1, 'tier': 6, 'rarity': 'legendary'},
    ]),
    # C2: Same with T5 leg
    ("Spear + Flute + bang_tam T5 leg", [
        {'id': 'bang_phach_thuong',  'x': 0, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'han_ngoc_tieu',      'x': 1, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'bang_tam_giap',      'x': 2, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'bang_tam_linh_chau', 'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'han_bang_chau',      'x': 4, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'kiem_tong_tam_yeu',  'x': 4, 'y': 2, 'tier': 5, 'rarity': 'legendary'},
        {'id': 'bach_ngoc_boi',      'x': 2, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
    ]),
    # C3: With thien_nien_quy_giap (2x2) and spear only
    ("Turtle shell + spear T6 epic", [
        {'id': 'bang_phach_thuong',   'x': 0, 'y': 0, 'tier': 6, 'rarity': 'epic'},
        {'id': 'thien_nien_quy_giap', 'x': 1, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'bang_tam_linh_chau',  'x': 3, 'y': 0, 'tier': 5, 'rarity': 'epic'},
        {'id': 'han_bang_chau',       'x': 3, 'y': 1, 'tier': 5, 'rarity': 'epic'},
        {'id': 'kiem_tong_tam_yeu',   'x': 4, 'y': 0, 'tier': 5, 'rarity': 'epic'},
    ]),
]

for name, items in cands:
    for tp in [2, 3]:
        e = {"name": "Dạ Sát Thiên", "sect": "han_son", "tamPhapLevel": tp, "level": 31, "items": items, "characterId": "da_sat_thien", "portrait": "boss_da_sat_thien"}
        wr, dur = simulate_stage(30, e, runs=100)
        print(f"{name:32s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(30, 'boss', p30, e, wr, dur)}")
