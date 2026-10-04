import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p10 = json.load(open("tools/gemini/out/round-4b/player/stage_10.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e10 = doc["chapters"][0]["stages"][9]["enemy"]

for r in ["rare", "epic", "legendary"]:
    items = copy.deepcopy(base_e10["items"])
    for it in items:
        it["rarity"] = r
    e = {
        "name": base_e10["name"], "sect": "liet_diem", "tamPhapLevel": 1, "level": 11,
        "items": items, "portrait": base_e10.get("portrait")
    }
    wr, dur = simulate_stage(10, e, runs=80)
    print(f"All {r:9}: WR = {wr:5.1f}%, dur = {dur:4.1f}s")
