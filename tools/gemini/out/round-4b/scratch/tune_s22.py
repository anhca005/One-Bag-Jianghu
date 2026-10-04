import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p22 = json.load(open("tools/gemini/out/round-4b/player/stage_22.json", encoding="utf-8"))

# Enemy 22:
# Level: 22
# Sect: han_son
# Allowed: tier 3..5, rarity: rare..legendary

# Grid 5x4:
# col 0: thanh_phong_kiem (1x3 at y=0)
# col 1-2: bang_tam_giap (2x1 at y=0)
# col 3: han_ngoc_tieu (1x1 at y=0)
# col 4: han_bang_chau (1x1 at y=0)
# Maybe add a 5th item like da_ho_y or second weapon or relic if needed

cands = [
    # C1: 4 items T4 epic
    ("4 items T4 epic", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "han_ngoc_tieu",     "x": 3, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
    ]),
    # C2: 4 items T4 leg
    ("4 items T4 leg", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "han_ngoc_tieu",     "x": 3, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 4, "rarity": "legendary"},
    ]),
    # C3: 4 items T5 epic
    ("4 items T5 epic", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "han_ngoc_tieu",     "x": 3, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 5, "rarity": "epic"},
    ]),
    # C4: 5 items with da_ho_y
    ("5 items (with da_ho) T4 epic", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "da_ho_y",           "x": 1, "y": 1, "tier": 4, "rarity": "epic"},
        {"id": "han_ngoc_tieu",     "x": 3, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
    ]),
    # C5: 5 items with da_ho T4 rare
    ("5 items (with da_ho) T4 rare", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "da_ho_y",           "x": 1, "y": 1, "tier": 4, "rarity": "rare"},
        {"id": "han_ngoc_tieu",     "x": 3, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 4, "rarity": "rare"},
    ]),
]

for name, items in cands:
    for tp in [1, 2]:
        e = {"name": "Hàn Đàm Cổ Kiếm", "sect": "han_son", "tamPhapLevel": tp, "level": 22, "items": items}
        wr, dur = simulate_stage(22, e, runs=100)
        print(f"{name:30s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(22, 'normal', p22, e, wr, dur)}")
