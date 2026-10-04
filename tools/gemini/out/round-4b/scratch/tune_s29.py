import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p29 = json.load(open("tools/gemini/out/round-4b/player/stage_29.json", encoding="utf-8"))

# Enemy 29:
# Level: 29
# Sect: thai_thanh
# Player 29 top tier: 5, top rarity: legendary
# Allowed tier: 4..6, rarity: epic..supreme
# Target WR: [82, 88]%, dur <= 40s

def make_s29(tier, rarity, w_t=None, w_r=None):
    wt = w_t or tier
    wr = w_r or rarity
    return [
        {'id': 'thanh_phong_kiem', 'x': 0, 'y': 0, 'tier': wt,   'rarity': wr},
        {'id': 'phi_yen_quyet',    'x': 1, 'y': 0, 'tier': tier, 'rarity': rarity},
        {'id': 'tuy_tien_ho_lo',   'x': 1, 'y': 1, 'tier': tier, 'rarity': rarity},
        {'id': 'kim_tuyen_y',      'x': 2, 'y': 0, 'tier': tier, 'rarity': rarity},
        {'id': 'kiem_tong_tam_yeu','x': 4, 'y': 0, 'tier': tier, 'rarity': rarity}
    ]

cands = [
    ("T5 epic all", make_s29(5, "epic")),
    ("T5 leg all",  make_s29(5, "legendary")),
    ("T6 leg all",  make_s29(6, "legendary")),
    ("T6 epic all", make_s29(6, "epic")),
    ("T5 leg, sword T6 sup", make_s29(5, "legendary", 6, "supreme")),
    ("T5 leg, sword T6 leg", make_s29(5, "legendary", 6, "legendary")),
    ("T5 epic, sword T6 leg", make_s29(5, "epic", 6, "legendary")),
]

for name, items in cands:
    for tp in [1, 2, 3]:
        e = {"name": "Tuyết Kiếm Tông Sư", "sect": "thai_thanh", "tamPhapLevel": tp, "level": 29, "items": items, "portrait": "enemy_portrait_1"}
        wr, dur = simulate_stage(29, e, runs=100)
        print(f"{name:25s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(29, 'normal', p29, e, wr, dur)}")
