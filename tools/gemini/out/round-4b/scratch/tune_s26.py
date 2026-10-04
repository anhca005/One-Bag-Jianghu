import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p26 = json.load(open("tools/gemini/out/round-4b/player/stage_26.json", encoding="utf-8"))

# Enemy 26: Bạch Hổ Tông Sư
# Level: 26
# Allowed tier: 3..5, rarity: epic..supreme
# Target WR: [82, 88]%, dur <= 40s

# Items:
# bach_ho_cuong_mong: x=0, y=0 (2x1)
# thanh_dong_ho_tam_kinh: x=0, y=1 (1x1)
# bach_ngoc_boi: x=1, y=1 (1x1)
# heo_rung_bap_giap: x=2, y=0 (2x2)
# thiet_cot_quyen_sao: x=4, y=0 (1x2)
# thiet_giap_y: x=0, y=2 (2x1)

def make_s26(tier, rarity, wep_r=None):
    wr = wep_r or rarity
    return [
        {'id': 'bach_ho_cuong_mong',      'x': 0, 'y': 0, 'tier': tier, 'rarity': wr},
        {'id': 'thanh_dong_ho_tam_kinh',  'x': 0, 'y': 1, 'tier': tier, 'rarity': rarity},
        {'id': 'bach_ngoc_boi',           'x': 1, 'y': 1, 'tier': tier, 'rarity': rarity},
        {'id': 'heo_rung_bap_giap',       'x': 2, 'y': 0, 'tier': tier, 'rarity': rarity},
        {'id': 'thiet_cot_quyen_sao',     'x': 4, 'y': 0, 'tier': tier, 'rarity': wr},
        {'id': 'thiet_giap_y',            'x': 0, 'y': 2, 'tier': tier, 'rarity': rarity}
    ]

tests = [
    ("T4 epic all", make_s26(4, "epic")),
    ("T5 epic all", make_s26(5, "epic")),
    ("T4 leg all", make_s26(4, "legendary")),
    ("T5 leg all", make_s26(5, "legendary")),
    ("T4 epic, weps leg", make_s26(4, "epic", wep_r="legendary")),
    ("T5 epic, weps leg", make_s26(5, "epic", wep_r="legendary")),
]

for name, items in tests:
    for tp in [1, 2]:
        e = {"name": "Bạch Hổ Tông Sư", "sect": None, "tamPhapLevel": tp, "level": 26, "items": items, "portrait": "enemy_portrait_2"}
        wr, dur = simulate_stage(26, e, runs=100)
        print(f"{name:25s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(26, 'normal', p26, e, wr, dur)}")
