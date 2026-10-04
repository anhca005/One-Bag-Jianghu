import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p25 = json.load(open("tools/gemini/out/round-4b/player/stage_25.json", encoding="utf-8"))

items_base = [
    {'id': 'bang_phach_thuong',  'x': 0, 'y': 0, 'tier': 5, 'rarity': 'supreme'},
    {'id': 'bang_tam_giap',      'x': 1, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'bang_tam_linh_chau', 'x': 3, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'han_bang_chau',      'x': 4, 'y': 0, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'kiem_tong_tam_yeu',  'x': 3, 'y': 1, 'tier': 5, 'rarity': 'legendary'},
    {'id': 'bach_ngoc_boi',      'x': 4, 'y': 1, 'tier': 5, 'rarity': 'legendary'}
]

# Try making 1 more item supreme
tests = [
    ("bang_tam_linh_chau supreme", 2, "bang_tam_linh_chau"),
    ("han_bang_chau supreme", 3, "han_bang_chau"),
    ("kiem_tong_tam_yeu supreme", 4, "kiem_tong_tam_yeu"),
    ("bang_tam_giap supreme", 1, "bang_tam_giap"),
]

for name, idx, item_id in tests:
    it = json.loads(json.dumps(items_base))
    it[idx]["rarity"] = "supreme"
    e = {'name': 'Băng Lam Kiếm Khách', 'sect': 'han_son', 'tamPhapLevel': 2, 'level': 25, 'items': it}
    wr, dur = simulate_stage(25, e, runs=200)
    print(f"{name:30s}: WR = {wr:.1f}%, dur = {dur:.1f}s, notes = {check_rules(25, 'elite', p25, e, wr, dur)}")
