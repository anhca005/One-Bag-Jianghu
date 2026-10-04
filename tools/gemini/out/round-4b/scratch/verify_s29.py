import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p29 = json.load(open("tools/gemini/out/round-4b/player/stage_29.json", encoding="utf-8"))

def make_s29_tweak(w_t=6, w_r="legendary", a_t=5, a_r="legendary", f_t=5, f_r="legendary"):
    return [
        {'id': 'thanh_phong_kiem', 'x': 0, 'y': 0, 'tier': w_t, 'rarity': w_r},
        {'id': 'phi_yen_quyet',    'x': 1, 'y': 0, 'tier': f_t, 'rarity': f_r},
        {'id': 'tuy_tien_ho_lo',   'x': 1, 'y': 1, 'tier': 5,   'rarity': 'legendary'},
        {'id': 'kim_tuyen_y',      'x': 2, 'y': 0, 'tier': a_t, 'rarity': a_r},
        {'id': 'kiem_tong_tam_yeu','x': 4, 'y': 0, 'tier': 5,   'rarity': 'legendary'}
    ]

tests = [
    # Sword supreme, rest T5 leg
    ("Sword T6 sup, rest T5 leg", make_s29_tweak(w_t=6, w_r="supreme")),
    # Armor T6 leg, sword T6 leg
    ("Armor T6 leg, sword T6 leg", make_s29_tweak(w_t=6, w_r="legendary", a_t=6, a_r="legendary")),
    # phi_yen_quyet T6 leg
    ("pyq T6 leg, sword T6 leg", make_s29_tweak(w_t=6, w_r="legendary", f_t=6, f_r="legendary")),
    # Sword T6 leg, armor supreme
    ("Armor T5 sup, sword T6 leg", make_s29_tweak(w_t=6, w_r="legendary", a_t=5, a_r="supreme")),
]

for name, items in tests:
    e = {"name": "Tuyết Kiếm Tông Sư", "sect": "thai_thanh", "tamPhapLevel": 1, "level": 29, "items": items, "portrait": "enemy_portrait_1"}
    wr, dur = simulate_stage(29, e, runs=200)
    print(f"{name:30s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(29, 'normal', p29, e, wr, dur)}")
