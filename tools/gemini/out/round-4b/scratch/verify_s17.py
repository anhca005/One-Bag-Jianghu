import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))

cand_b = [
    {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
    {"id": "man_xa_doc_tui",   "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
    {"id": "van_doc_dinh",     "x": 1, "y": 1, "tier": 3, "rarity": "epic"},
    {"id": "kich_doc_chu_nga", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
    {"id": "da_ho_y",          "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
    {"id": "u_minh_bi_luc",    "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
]

e = {"name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 17, "items": cand_b}
wr, dur = simulate_stage(17, e, runs=200)
print(f"Cand B 200 runs: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(17, 'normal', p17, e, wr, dur)}")
