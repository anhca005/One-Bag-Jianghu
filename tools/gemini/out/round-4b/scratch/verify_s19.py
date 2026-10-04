import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p19 = json.load(open("tools/gemini/out/round-4b/player/stage_19.json", encoding="utf-8"))

cand_a = [
    {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
    {"id": "hoa_diem_xa_cot",  "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
    {"id": "thanh_dang_tien",  "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
    {"id": "man_xa_doc_tui",   "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
    {"id": "boi_nguyen_dan",   "x": 3, "y": 1, "tier": 2, "rarity": "rare"},
    {"id": "da_ho_y",          "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
]

# cand_b: what if man_xa is T3?
cand_b = [
    {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
    {"id": "hoa_diem_xa_cot",  "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
    {"id": "thanh_dang_tien",  "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
    {"id": "man_xa_doc_tui",   "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
    {"id": "boi_nguyen_dan",   "x": 3, "y": 1, "tier": 2, "rarity": "rare"},
    {"id": "da_ho_y",          "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
]

for name, items in [("Cand A", cand_a), ("Cand B", cand_b)]:
    e = {"name": "Xích Luyện Xà Tinh", "sect": None, "tamPhapLevel": 2, "level": 19, "items": items, "portrait": "monster_xich_xa"}
    wr, dur = simulate_stage(19, e, runs=200)
    print(f"{name} 200 runs: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(19, 'normal', p19, e, wr, dur)}")
