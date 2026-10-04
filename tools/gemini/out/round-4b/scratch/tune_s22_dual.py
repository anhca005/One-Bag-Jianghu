import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p22 = json.load(open("tools/gemini/out/round-4b/player/stage_22.json", encoding="utf-8"))

def layout(tier, rarity, spear_t=None, sword_t=None):
    st = spear_t or tier
    swt = sword_t or tier
    return [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": st, "rarity": rarity},
        {"id": "thanh_phong_kiem",  "x": 1, "y": 0, "tier": swt, "rarity": rarity},
        {"id": "bang_tam_giap",     "x": 2, "y": 0, "tier": tier, "rarity": rarity},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": tier, "rarity": rarity},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 1, "tier": tier, "rarity": rarity},
        {"id": "kiem_tong_tam_yeu", "x": 4, "y": 3, "tier": tier, "rarity": rarity},
    ]

cands = [
    ("Dual wep T4 epic", layout(4, "epic")),
    ("Dual wep T4 rare", layout(4, "rare")),
    ("Dual wep T3 epic", layout(3, "epic")),
    ("Dual wep T3 rare", layout(3, "rare")),
    ("Spear T5, rest T4 epic", layout(4, "epic", spear_t=5)),
    ("Sword T5, rest T4 epic", layout(4, "epic", sword_t=5)),
]

for name, items in cands:
    for tp in [1, 2]:
        e = {"name": "Hàn Đàm Cổ Kiếm", "sect": "han_son", "tamPhapLevel": tp, "level": 22, "items": items}
        wr, dur = simulate_stage(22, e, runs=100)
        print(f"{name:30s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(22, 'normal', p22, e, wr, dur)}")
