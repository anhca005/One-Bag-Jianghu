import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p6 = json.load(open("tools/gemini/out/round-4b/player/stage_06.json", encoding="utf-8"))

for lvl in [6, 7]:
    for t_armor in [2, 3]:
        for r_armor in ["rare", "epic"]:
            items = [
                {"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
                {"id": "that_tinh_tieu", "x": 0, "y": 2, "tier": 2, "rarity": "rare"},
                {"id": "da_ho_y", "x": 1, "y": 0, "tier": t_armor, "rarity": r_armor},
                {"id": "phi_yen_quyet", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
                {"id": "thiet_giap_y", "x": 3, "y": 0, "tier": 2, "rarity": "rare"}
            ]
            e = {"name": "Ám Hạng Thích Khách", "sect": "thai_thanh", "tamPhapLevel": 1, "level": lvl, "items": items}
            wr, dur = simulate_stage(6, e, runs=200)
            print(f"lvl={lvl}, t_armor={t_armor}, r_armor={r_armor}: WR={wr:5.1f}%, dur={dur:4.1f}s, notes={check_rules(6, 'normal', p6, e, wr, dur)}")
