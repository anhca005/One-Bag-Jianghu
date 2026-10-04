import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p19 = json.load(open("tools/gemini/out/round-4b/player/stage_19.json", encoding="utf-8"))

# Test rare items or single weapon adjacent
tests = [
    # Rare all
    ("T3 rare all", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "hoa_diem_xa_cot",  "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "thanh_dang_tien",  "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "man_xa_doc_tui",   "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "boi_nguyen_dan",   "x": 3, "y": 1, "tier": 3, "rarity": "rare"},
        {"id": "da_ho_y",          "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
    ]),
    # T2 rare all
    ("T2 rare all", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "hoa_diem_xa_cot",  "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "thanh_dang_tien",  "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "man_xa_doc_tui",   "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "boi_nguyen_dan",   "x": 3, "y": 1, "tier": 2, "rarity": "rare"},
        {"id": "da_ho_y",          "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
    ]),
    # Only 1 weapon (kich_doc_chu_nga) adjacent to hoa_diem_xa_cot
    ("1 weapon (kich_doc) T3 rare", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "hoa_diem_xa_cot",  "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "man_xa_doc_tui",   "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "boi_nguyen_dan",   "x": 2, "y": 1, "tier": 3, "rarity": "rare"},
        {"id": "da_ho_y",          "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
    ]),
    # Only 1 weapon (kich_doc) T3 epic
    ("1 weapon (kich_doc) T3 epic", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "hoa_diem_xa_cot",  "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "man_xa_doc_tui",   "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "boi_nguyen_dan",   "x": 2, "y": 1, "tier": 3, "rarity": "epic"},
        {"id": "da_ho_y",          "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
    ]),
    # 1 weapon (thanh_dang_tien) T3 epic
    ("1 weapon (thanh_dang) T3 epic", [
        {"id": "thanh_dang_tien",  "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "hoa_diem_xa_cot",  "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "man_xa_doc_tui",   "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "boi_nguyen_dan",   "x": 2, "y": 1, "tier": 3, "rarity": "epic"},
        {"id": "da_ho_y",          "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
    ]),
    # 1 weapon (thanh_dang_tien) T3 rare
    ("1 weapon (thanh_dang) T3 rare", [
        {"id": "thanh_dang_tien",  "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "hoa_diem_xa_cot",  "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "man_xa_doc_tui",   "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "boi_nguyen_dan",   "x": 2, "y": 1, "tier": 3, "rarity": "rare"},
        {"id": "da_ho_y",          "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
    ]),
]

for name, items in tests:
    e = {"name": "Xích Luyện Xà Tinh", "sect": None, "tamPhapLevel": 2, "level": 19, "items": items, "portrait": "monster_xich_xa"}
    wr, dur = simulate_stage(19, e, runs=100)
    print(f"{name:35s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(19, 'normal', p19, e, wr, dur)}")
