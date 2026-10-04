import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p25 = json.load(open("tools/gemini/out/round-4b/player/stage_25.json", encoding="utf-8"))

# Enemy 25: Elite
# Level: 25
# Allowed tier: 3..5, rarity: epic..supreme
# Target WR: [62, 68]%, dur <= 40s

# bang_phach_thuong: 1x4 at x=0
# bang_tam_giap: 2x1 at x=1, y=0
# bang_tam_linh_chau: 1x1 at x=3, y=0
# han_bang_chau: 1x1 at x=4, y=0
# kiem_tong_tam_yeu: 1x1 at x=3, y=1
# bach_ngoc_boi: 1x1 at x=4, y=1

def make_s25(w_t, w_r, o_t, o_r):
    return [
        {"id": "bang_phach_thuong",   "x": 0, "y": 0, "tier": w_t, "rarity": w_r},
        {"id": "bang_tam_giap",       "x": 1, "y": 0, "tier": o_t, "rarity": o_r},
        {"id": "bang_tam_linh_chau",  "x": 3, "y": 0, "tier": o_t, "rarity": o_r},
        {"id": "han_bang_chau",       "x": 4, "y": 0, "tier": o_t, "rarity": o_r},
        {"id": "kiem_tong_tam_yeu",   "x": 3, "y": 1, "tier": o_t, "rarity": o_r},
        {"id": "bach_ngoc_boi",       "x": 4, "y": 1, "tier": o_t, "rarity": o_r},
    ]

tests = [
    ("T5 leg spear, T4 leg rest", make_s25(5, "legendary", 4, "legendary")),
    ("T5 sup spear, T4 leg rest", make_s25(5, "supreme", 4, "legendary")),
    ("T5 leg spear, T5 leg rest", make_s25(5, "legendary", 5, "legendary")),
    ("T5 sup spear, T5 leg rest", make_s25(5, "supreme", 5, "legendary")),
    ("T4 leg spear, T4 leg rest", make_s25(4, "legendary", 4, "legendary")),
    ("T5 epic spear, T4 epic rest", make_s25(5, "epic", 4, "epic")),
]

for name, items in tests:
    for tp in [2, 3]:
        e = {"name": "Băng Lam Kiếm Khách", "sect": "han_son", "tamPhapLevel": tp, "level": 25, "items": items}
        wr, dur = simulate_stage(25, e, runs=100)
        print(f"{name:30s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(25, 'elite', p25, e, wr, dur)}")
