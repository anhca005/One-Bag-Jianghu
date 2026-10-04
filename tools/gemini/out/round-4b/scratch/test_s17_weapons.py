import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))

# Enemy 17 allowed:
# Level: 17
# Sect: huyen_doc, tamPhapLevel: 1..3
# Tier: player top tier is 3, so enemy top tier in [2, 3, 4]
# Rarity: player top rarity is epic, so enemy top rarity in [rare, epic, legendary]

# Let's test combinations:
loadouts = [
    # 1. kich_doc_chu_nga T4 leg + that_tinh_tieu T4 leg + da_ho_y T3 leg + van_doc_dinh T3 leg + u_minh_bi_luc T3 leg
    ("Kich doc T4 leg + TTT T4 leg + da_ho T3 leg + vdd T3 leg + umbl T3 leg", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"}
    ]),
    # 2. dual kich_doc_chu_nga
    ("Dual Kich doc T3 leg + da_ho T3 leg + vdd T3 leg + umbl T3 leg", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"}
    ]),
    # 3. dual kich_doc_chu_nga T4 epic
    ("Dual Kich doc T4 epic + da_ho T3 epic + vdd T3 epic + umbl T3 epic", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "epic"}
    ]),
    # 4. tu_kim_trao T4 leg + that_tinh_tieu T4 leg + da_ho T3 leg + vdd T3 leg + umbl T3 leg
    ("Tu kim T4 leg + TTT T4 leg + da_ho T3 leg + vdd T3 leg + umbl T3 leg", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"}
    ]),
    # 5. tu_kim_trao T3 leg + tu_kim_trao T3 leg + da_ho T3 leg + vdd T3 leg + umbl T3 leg
    ("Dual tu kim T3 leg + da_ho T3 leg + vdd T3 leg + umbl T3 leg", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
        {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"}
    ]),
]

for name, items in loadouts:
    e = {"name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 17, "items": items}
    wr, dur = simulate_stage(17, e, runs=100)
    print(f"{name:65s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(17, 'normal', p17, e, wr, dur)}")
