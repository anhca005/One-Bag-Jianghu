import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p10 = json.load(open("tools/gemini/out/round-4b/player/stage_10.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e10 = doc["chapters"][0]["stages"][9]["enemy"]

# Items: liet_diem_dao, heo_rung_bap_giap, cuong_hoa_dan, liet_hoa_tam_kinh, chu_sa_ho_lo, bach_ngoc_boi
# Test combinations of uncommon and rare
item_names = [it["id"] for it in base_e10["items"]]

tests = [
    # blade + manual + both relics + pill rare (only armor uncommon)
    ("all_rare_except_armor", ["liet_diem_dao", "liet_hoa_tam_kinh", "chu_sa_ho_lo", "bach_ngoc_boi", "cuong_hoa_dan"]),
    # what if blade is epic, armor uncommon, rest uncommon?
    ("blade_epic", ["liet_diem_dao_epic"]),
]

for label, rare_items in tests:
    items = copy.deepcopy(base_e10["items"])
    for it in items:
        if label == "blade_epic" and it["id"] == "liet_diem_dao":
            it["rarity"] = "epic"
        else:
            it["rarity"] = "rare" if it["id"] in rare_items else "uncommon"
    e = {
        "name": base_e10["name"], "sect": "liet_diem", "tamPhapLevel": 1, "level": 11,
        "items": items, "portrait": base_e10.get("portrait")
    }
    wr, dur = simulate_stage(10, e, runs=200)
    print(f"rare={label}: WR={wr:5.1f}%, dur={dur:4.1f}s, notes={check_rules(10, 'boss', p10, e, wr, dur)}")



for label, rare_items in tests:
    items = copy.deepcopy(base_e10["items"])
    for it in items:
        it["rarity"] = "rare" if it["id"] in rare_items else "uncommon"
    e = {
        "name": base_e10["name"], "sect": "liet_diem", "tamPhapLevel": 1, "level": 11,
        "items": items, "portrait": base_e10.get("portrait")
    }
    wr, dur = simulate_stage(10, e, runs=200)
    print(f"rare={label}: WR={wr:5.1f}%, dur={dur:4.1f}s, notes={check_rules(10, 'boss', p10, e, wr, dur)}")

