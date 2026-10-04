import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p18 = json.load(open("tools/gemini/out/round-4b/player/stage_18.json", encoding="utf-8"))

# Test o_tam_y variants
for t in [2, 3]:
    for r in ["rare", "epic"]:
        items = [
            {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 3, "rarity": "rare"},
            {"id": "o_tam_y",     "x": 1, "y": 0, "tier": t, "rarity": r},
            {"id": "van_doc_dinh","x": 3, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "u_minh_bi_luc","x": 3, "y": 1, "tier": 3, "rarity": "rare"},
        ]
        e = {"name": "Huyền Độc Trưởng Lão", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 18, "items": items}
        wr, dur = simulate_stage(18, e, runs=100)
        print(f"o_tam_y T{t} {r:5s} (minimal items): WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(18, 'normal', p18, e, wr, dur)}")
