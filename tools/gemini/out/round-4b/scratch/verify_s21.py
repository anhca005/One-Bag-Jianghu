import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p21 = json.load(open("tools/gemini/out/round-4b/player/stage_21.json", encoding="utf-8"))

tests = [
    # weapon T4, armor T3, rest T3
    ("Weapon T4 rare, rest T3 rare", [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "da_ho_y",           "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
    ]),
    # weapon T3, armor T4, rest T3
    ("Armor T4 rare, rest T3 rare", [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "da_ho_y",           "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
    ]),
    # weapon T4, armor T4, relics T3
    ("Wep+Arm T4 rare, relics T3 rare", [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "da_ho_y",           "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
    ]),
]

for name, items in tests:
    e = {"name": "Băng Phách Thủ Vệ", "sect": "han_son", "tamPhapLevel": 1, "level": 21, "items": items}
    wr, dur = simulate_stage(21, e, runs=200)
    print(f"{name:35s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(21, 'normal', p21, e, wr, dur)}")
