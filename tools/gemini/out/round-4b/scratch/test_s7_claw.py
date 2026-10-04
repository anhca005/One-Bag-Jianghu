import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p7 = json.load(open("tools/gemini/out/round-4b/player/stage_07.json", encoding="utf-8"))

for r_blade in ["rare", "epic"]:
    for r_claw in ["uncommon", "rare", "epic"]:
        for r_armor in ["rare", "epic"]:
            items = [
                {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": r_blade},
                {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 2, "rarity": r_claw},
                {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": r_armor}
            ]
            e = {"name": "Liệt Diễm Bang Tinh Anh", "sect": None, "tamPhapLevel": 1, "level": 7, "items": items}
            wr, dur = simulate_stage(7, e, runs=200)
            if 82.0 <= wr <= 88.0 and dur <= 30.0:
                print(f"FOUND S7: WR={wr:.1f}%, dur={dur:.1f}s | blade={r_blade}, claw={r_claw}, armor={r_armor}")
