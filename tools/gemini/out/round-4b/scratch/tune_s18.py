import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p18 = json.load(open("tools/gemini/out/round-4b/player/stage_18.json", encoding="utf-8"))

# Enemy 18:
# Name: Huyền Độc Trưởng Lão
# Sect: huyen_doc
# Level: 18
# Allowed tier: 2..4, rarity: rare..legendary

# Let's test combinations of dual tu_kim_trao, armor, van_doc_dinh, etc.
cands = [
    # C1: T3 rare tu_kim_trao x2, da_ho_y T3 rare, van_doc_dinh T3 rare, u_minh_bi_luc T3 rare
    ("Dual tu_kim T3 rare, da_ho T3 rare, vdd T3 rare, umbl T3 rare", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "da_ho_y",     "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "van_doc_dinh","x": 3, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "u_minh_bi_luc","x": 4, "y": 0, "tier": 3, "rarity": "rare"},
    ]),
    # C2: T3 epic
    ("Dual tu_kim T3 epic, da_ho T3 epic, vdd T3 epic, umbl T3 epic", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "da_ho_y",     "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "van_doc_dinh","x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "u_minh_bi_luc","x": 4, "y": 0, "tier": 3, "rarity": "epic"},
    ]),
    # C3: T4 epic tu_kim_trao x2, da_ho T3 epic, vdd T3 epic, umbl T3 epic
    ("Dual tu_kim T4 epic, da_ho T3 epic, vdd T3 epic, umbl T3 epic", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "da_ho_y",     "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "van_doc_dinh","x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "u_minh_bi_luc","x": 4, "y": 0, "tier": 3, "rarity": "epic"},
    ]),
    # C4: T4 epic tu_kim_trao x2 + man_xa_doc_tui T3 epic
    ("Dual tu_kim T4 epic + man_xa + vdd + da_ho + umbl", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "man_xa_doc_tui","x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "tu_kim_trao", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "da_ho_y",     "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "van_doc_dinh","x": 4, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "u_minh_bi_luc","x": 4, "y": 1, "tier": 3, "rarity": "epic"},
    ]),
    # C5: T3 leg tu_kim_trao x2 + man_xa + vdd + da_ho + umbl
    ("Dual tu_kim T3 leg + man_xa + vdd + da_ho + umbl", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "man_xa_doc_tui","x": 1, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "tu_kim_trao", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "da_ho_y",     "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "van_doc_dinh","x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "u_minh_bi_luc","x": 4, "y": 1, "tier": 3, "rarity": "legendary"},
    ]),
]

for name, items in cands:
    for tp in [1, 2]:
        e = {"name": "Huyền Độc Trưởng Lão", "sect": "huyen_doc", "tamPhapLevel": tp, "level": 18, "items": items}
        wr, dur = simulate_stage(18, e, runs=100)
        print(f"{name:55s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(18, 'normal', p18, e, wr, dur)}")
