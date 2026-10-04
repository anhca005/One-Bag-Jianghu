import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p21 = json.load(open("tools/gemini/out/round-4b/player/stage_21.json", encoding="utf-8"))
# Player 21: lv 21, top tier 4, top rarity epic
# Enemy 21: Băng Phách Thủ Vệ, sect: han_son, lv 21
# Allowed: tier 3..5, rarity: rare..legendary
print("=== TUNING STAGE 21 ===")
s21_cands = [
    ("T4 rare all", [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "da_ho_y",           "x": 3, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 1, "tier": 4, "rarity": "rare"},
    ]),
    ("T4 epic all", [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "da_ho_y",           "x": 3, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
    ]),
    ("T3 epic all", [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "da_ho_y",           "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 1, "tier": 3, "rarity": "epic"},
    ]),
    ("T3 rare all", [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "da_ho_y",           "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
    ]),
]

for name, items in s21_cands:
    for tp in [1, 2]:
        e = {"name": "Băng Phách Thủ Vệ", "sect": "han_son", "tamPhapLevel": tp, "level": 21, "items": items}
        wr, dur = simulate_stage(21, e, runs=100)
        print(f"S21 {name:15s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(21, 'normal', p21, e, wr, dur)}")
