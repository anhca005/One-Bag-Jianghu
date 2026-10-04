import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p12 = json.load(open("tools/gemini/out/round-4b/player/stage_12.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e12 = doc["chapters"][1]["stages"][1]["enemy"]

# Let's search around Cand 2:
# 1 claw T3 rare, 1 claw T2 rare/uncommon, armor T2 rare/epic
for r_c2 in ["uncommon", "rare", "epic"]:
    for r_armor in ["rare", "epic"]:
        for r_man in ["rare", "epic"]:
            items = [
                {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
                {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 2, "rarity": r_c2},
                {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": r_armor},
                {"id": "u_minh_bi_luc", "x": 3, "y": 0, "tier": 2, "rarity": r_man}
            ]
            e = copy.deepcopy(base_e12)
            e["level"] = 12
            e["items"] = items
            wr, dur = simulate_stage(12, e, runs=200)
            if 82.0 <= wr <= 88.0 and dur <= 35.0:
                print(f"FOUND S12: WR={wr:.1f}%, dur={dur:.1f}s | r_c2={r_c2}, r_armor={r_armor}, r_man={r_man}")
