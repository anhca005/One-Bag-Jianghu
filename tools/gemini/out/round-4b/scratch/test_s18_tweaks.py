import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p18 = json.load(open("tools/gemini/out/round-4b/player/stage_18.json", encoding="utf-8"))

# Test minor tweaks:
tests = [
    # vdd T2
    ("vdd T2", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 3, "rarity": "rare"},
        {"id": "o_tam_y",     "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "van_doc_dinh","x": 3, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "u_minh_bi_luc","x": 3, "y": 1, "tier": 3, "rarity": "rare"},
    ]),
    # umbl T2
    ("umbl T2", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 3, "rarity": "rare"},
        {"id": "o_tam_y",     "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "van_doc_dinh","x": 3, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "u_minh_bi_luc","x": 3, "y": 1, "tier": 2, "rarity": "rare"},
    ]),
    # both T2
    ("both T2", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 3, "rarity": "rare"},
        {"id": "o_tam_y",     "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "van_doc_dinh","x": 3, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "u_minh_bi_luc","x": 3, "y": 1, "tier": 2, "rarity": "rare"},
    ]),
]

for name, items in tests:
    e = {"name": "Huyền Độc Trưởng Lão", "sect": "huyen_doc", "tamPhapLevel": 1, "level": 18, "items": items}
    wr, dur = simulate_stage(18, e, runs=200)
    print(f"{name}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(18, 'normal', p18, e, wr, dur)}")
