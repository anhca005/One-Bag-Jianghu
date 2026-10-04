import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))

# Let's place items so man_xa_doc_tui and van_doc_dinh are adjacent or active
# Grid is 5 cols x 4 rows
# Let's test a few variations:

# Candidate A:
# x=0: kich_doc_chu_nga (1x2 at y=0)
# x=1: man_xa_doc_tui (1x1 at y=0), van_doc_dinh (1x1 at y=1)
# x=2: kich_doc_chu_nga (1x2 at y=0)
# x=3: da_ho_y (1x2 at y=0)
# x=4: u_minh_bi_luc (1x1 at y=0)

cand_a = [
    {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "legendary"},
    {"id": "man_xa_doc_tui",   "x": 1, "y": 0, "tier": 3, "rarity": "legendary"},
    {"id": "van_doc_dinh",     "x": 1, "y": 1, "tier": 3, "rarity": "legendary"},
    {"id": "kich_doc_chu_nga", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
    {"id": "da_ho_y",          "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
    {"id": "u_minh_bi_luc",    "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
]

# Candidate B: Tier 4 weapons
cand_b = [
    {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
    {"id": "man_xa_doc_tui",   "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
    {"id": "van_doc_dinh",     "x": 1, "y": 1, "tier": 3, "rarity": "epic"},
    {"id": "kich_doc_chu_nga", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
    {"id": "da_ho_y",          "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
    {"id": "u_minh_bi_luc",    "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
]

# Candidate C: Tier 3 epic
cand_c = [
    {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
    {"id": "man_xa_doc_tui",   "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
    {"id": "van_doc_dinh",     "x": 1, "y": 1, "tier": 3, "rarity": "epic"},
    {"id": "kich_doc_chu_nga", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
    {"id": "da_ho_y",          "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
    {"id": "u_minh_bi_luc",    "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
]

for name, items in [("Cand A (T3 leg)", cand_a), ("Cand B (T4 epic)", cand_b), ("Cand C (T3 epic)", cand_c)]:
    for tp in [1, 2, 3]:
        e = {"name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": tp, "level": 17, "items": items}
        wr, dur = simulate_stage(17, e, runs=100)
        print(f"{name} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(17, 'normal', p17, e, wr, dur)}")
