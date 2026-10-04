import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p10 = json.load(open("tools/gemini/out/round-4b/player/stage_10.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e10 = doc["chapters"][0]["stages"][9]["enemy"]

# Original: liet_diem_dao, heo_rung_bap_giap, cuong_hoa_dan, liet_hoa_tam_kinh, chu_sa_ho_lo, bach_ngoc_boi (all T3 uncommon)
# Let's test with rare and epic rarities, tamPhapLevel 1 or 2
hits = []
for tp in [1, 2]:
    for r_blade in ["rare", "epic"]:
        for r_armor in ["rare", "epic"]:
            for r_manual in ["rare", "epic"]:
                for r_pill in ["rare", "epic"]:
                    items = copy.deepcopy(base_e10["items"])
                    for it in items:
                        if it["id"] == "liet_diem_dao": it["rarity"] = r_blade
                        elif it["id"] == "heo_rung_bap_giap": it["rarity"] = r_armor
                        elif it["id"] == "liet_hoa_tam_kinh": it["rarity"] = r_manual
                        elif it["id"] == "cuong_hoa_dan": it["rarity"] = r_pill
                        else: it["rarity"] = "rare"
                    e = {
                        "name": base_e10["name"], "sect": "liet_diem", "tamPhapLevel": tp, "level": 11,
                        "items": items, "portrait": base_e10.get("portrait")
                    }
                    wr, dur = simulate_stage(10, e, runs=60)
                    if 52.0 <= wr <= 58.0 and dur <= 35.0:
                        hits.append((wr, dur, tp, r_blade, r_armor, r_manual, r_pill))
                        print(f"HIT S10: WR={wr:.1f}%, dur={dur:.1f}s | TP={tp}, blade={r_blade}, armor={r_armor}, man={r_manual}, pill={r_pill}")
                        if len(hits) >= 4: break
                if len(hits) >= 4: break
            if len(hits) >= 4: break
        if len(hits) >= 4: break
    if len(hits) >= 4: break
