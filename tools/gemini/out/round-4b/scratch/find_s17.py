import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))

# Test Cand 1 with lower tier/rarity o_tam_y
for t_armor in [2, 3]:
    for r_armor in ["rare", "epic"]:
        for t_claw in [3, 4]:
            for r_claw in ["rare", "epic"]:
                items = [
                    {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": t_claw, "rarity": r_claw},
                    {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "o_tam_y", "x": 2, "y": 0, "tier": t_armor, "rarity": r_armor},
                    {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "rare"}
                ]
                e = {"name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 17, "items": items}
                wr, dur = simulate_stage(17, e, runs=80)
                print(f"t_arm={t_armor}, r_arm={r_armor}, t_claw={t_claw}, r_claw={r_claw}: WR={wr:5.1f}%, dur={dur:4.1f}s")

