import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p28 = json.load(open("tools/gemini/out/round-4b/player/stage_28.json", encoding="utf-8"))

# Enemy 28:
# Level: 28
# Player 28 top tier: 5, top rarity: legendary
# Allowed tier: 4..6, rarity: epic..supreme
# Target WR: [82, 88]%, dur <= 40s

# Positions:
# bang_phach_co_cot: 2x2 at x=0, y=0
# thien_nien_quy_giap: 2x2 at x=2, y=0
# bang_phach_thuong: 1x4 at x=4, y=0
# han_ngoc_tieu: 1x2 at x=0, y=2
# han_bang_chau: 1x1 at x=1, y=2

def make_s28(tier, rarity, spear_t=None, spear_r=None):
    st = spear_t or tier
    sr = spear_r or rarity
    return [
        {'id': 'bang_phach_co_cot',   'x': 0, 'y': 0, 'tier': tier, 'rarity': rarity},
        {'id': 'thien_nien_quy_giap', 'x': 2, 'y': 0, 'tier': tier, 'rarity': rarity},
        {'id': 'bang_phach_thuong',   'x': 4, 'y': 0, 'tier': st,   'rarity': sr},
        {'id': 'han_ngoc_tieu',       'x': 0, 'y': 2, 'tier': tier, 'rarity': rarity},
        {'id': 'han_bang_chau',       'x': 1, 'y': 2, 'tier': tier, 'rarity': rarity}
    ]

cands = [
    ("T5 epic all", make_s28(5, "epic")),
    ("T4 epic all", make_s28(4, "epic")),
    ("T5 leg all",  make_s28(5, "legendary")),
    ("T4 leg all",  make_s28(4, "legendary")),
    ("T4 epic, spear T5 leg", make_s28(4, "epic", 5, "legendary")),
    ("T4 epic, spear T6 leg", make_s28(4, "epic", 6, "legendary")),
]

for name, items in cands:
    e = {"name": "Hàn Băng Thần Quy", "sect": None, "tamPhapLevel": 1, "level": 28, "items": items, "portrait": "monster_than_quy"}
    wr, dur = simulate_stage(28, e, runs=100)
    print(f"{name:25s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(28, 'normal', p28, e, wr, dur)}")
