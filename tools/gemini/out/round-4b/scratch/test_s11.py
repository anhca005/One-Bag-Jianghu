import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p11 = json.load(open("tools/gemini/out/round-4b/player/stage_11.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e11 = doc["chapters"][1]["stages"][0]["enemy"]

items11 = copy.deepcopy(base_e11["items"])
for it in items11:
    if it["rarity"] == "supreme":
        it["rarity"] = "epic"

e11 = copy.deepcopy(base_e11)
e11["level"] = 11
e11["items"] = items11

wr, dur = simulate_stage(11, e11, runs=200)
print(f"Stage 11: WR = {wr:.1f}%, dur = {dur:.1f}s, notes = {check_rules(11, 'normal', p11, e11, wr, dur)}")
