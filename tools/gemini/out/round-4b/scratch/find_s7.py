import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p7 = json.load(open("tools/gemini/out/round-4b/player/stage_07.json", encoding="utf-8"))

weapons = ["thiet_cot_quyen_sao", "tu_kim_trao", "that_tinh_tieu", "phan_quan_but", "ngoc_cot_phien"]
armors = ["heo_rung_bap_giap", "thiet_giap_y", "o_thiet_giap", "da_ho_y"]

hits = []
for w2 in weapons:
    for a in armors:
        for rw2 in ["rare", "epic", "uncommon"]:
            for ra in ["rare", "epic", "uncommon"]:
                for r_blade in ["rare", "epic"]:
                    items = [
                        {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": r_blade},
                        {"id": w2, "x": 1, "y": 0, "tier": 2, "rarity": rw2},
                        {"id": a, "x": 2, "y": 0, "tier": 2, "rarity": ra}
                    ]
                    e = {"name": "Liệt Diễm Bang Tinh Anh", "sect": None, "tamPhapLevel": 1, "level": 7, "items": items}
                    wr, dur = simulate_stage(7, e, runs=60)
                    if 83.5 <= wr <= 86.5 and dur <= 30.0:
                        hits.append((wr, dur, items))
                        print(f"HIT: WR={wr:.1f}%, dur={dur:.1f}s | w2={w2}({rw2}), a={a}({ra}), blade=({r_blade})")
                        if len(hits) >= 3:
                            break
            if len(hits) >= 3: break
        if len(hits) >= 3: break
    if len(hits) >= 3: break
