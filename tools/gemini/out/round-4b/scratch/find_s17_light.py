import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))

# Test da_ho_y or nho_sam_y at T3/T4
for arm in ["da_ho_y", "nho_sam_y", "bang_tam_giap"]:
    for t_arm in [3, 4]:
        for r_arm in ["rare", "epic", "legendary"]:
            for t_claw in [3, 4]:
                for r_claw in ["rare", "epic", "legendary"]:
                    items = [
                        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": t_claw, "rarity": r_claw},
                        {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
                        {"id": arm, "x": 2, "y": 0, "tier": t_arm, "rarity": r_arm},
                        {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
                        {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "rare"}
                    ]
                    e = {"name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 17, "items": items}
                    wr, dur = simulate_stage(17, e, runs=60)
                    if 82.0 <= wr <= 88.0 and dur <= 30.0:
                        print(f"FOUND S17: WR={wr:.1f}%, dur={dur:.1f}s | arm={arm}(T{t_arm},{r_arm}), claw=(T{t_claw},{r_claw})")
                        sys.exit(0)
