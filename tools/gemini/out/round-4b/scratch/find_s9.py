import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p9 = json.load(open("tools/gemini/out/round-4b/player/stage_09.json", encoding="utf-8"))

def make_e9(lvl=9, t_sword=2, r_sword="epic", t_armor=2, r_armor="epic", t_man=1, r_man="epic", t_rel=1, r_rel="epic"):
    return {
        "name": "Ba Hổ Tiên Phong Vệ", "sect": "thai_thanh", "tamPhapLevel": 1, "level": lvl,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": t_sword, "rarity": r_sword},
            {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": t_man, "rarity": r_man},
            {"id": "tuy_tien_ho_lo", "x": 1, "y": 1, "tier": t_rel, "rarity": r_rel},
            {"id": "o_tam_y", "x": 2, "y": 0, "tier": t_armor, "rarity": r_armor},
        ]
    }

# Baseline at level 9 gave 75.0%
for r_man in ["rare", "epic"]:
    for r_sword in ["rare", "epic"]:
        for t_sword in [1, 2]:
            e = make_e9(lvl=9, t_sword=t_sword, r_sword=r_sword, r_man=r_man)
            wr, dur = simulate_stage(9, e, runs=200)
            if 82.0 <= wr <= 88.0 and dur <= 35.0:
                print(f"FOUND S9: WR={wr:.1f}%, dur={dur:.1f}s | t_sword={t_sword}, r_sword={r_sword}, r_man={r_man}")
