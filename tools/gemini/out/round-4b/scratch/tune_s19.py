import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p19 = json.load(open("tools/gemini/out/round-4b/player/stage_19.json", encoding="utf-8"))

# Stage 19: Normal
# Enemy: Xích Luyện Xà Tinh (monster_xich_xa, sect=None, tamPhapLevel=2)
# Level: 19
# Player top tier = 3, top rarity = epic
# Allowed tier: 2..4, allowed rarity: rare..legendary

# Items originally:
# hoa_diem_xa_cot, da_ho_y, man_xa_doc_tui, thanh_dang_tien, boi_nguyen_dan

cands = [
    # C1: T3 epic all
    ("T3 epic all", [
        {"id": "hoa_diem_xa_cot", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "da_ho_y",          "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "man_xa_doc_tui",   "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "thanh_dang_tien",  "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "boi_nguyen_dan",   "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
    ]),
    # C2: T3 legendary all
    ("T3 leg all", [
        {"id": "hoa_diem_xa_cot", "x": 0, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "da_ho_y",          "x": 1, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "man_xa_doc_tui",   "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "thanh_dang_tien",  "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "boi_nguyen_dan",   "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
    ]),
    # C3: T4 epic
    ("T4 epic all", [
        {"id": "hoa_diem_xa_cot", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "da_ho_y",          "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "man_xa_doc_tui",   "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "thanh_dang_tien",  "x": 3, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "boi_nguyen_dan",   "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
    ]),
]

for name, items in cands:
    e = {"name": "Xích Luyện Xà Tinh", "sect": None, "tamPhapLevel": 2, "level": 19, "items": items, "portrait": "monster_xich_xa"}
    wr, dur = simulate_stage(19, e, runs=100)
    print(f"{name:20s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(19, 'normal', p19, e, wr, dur)}")
