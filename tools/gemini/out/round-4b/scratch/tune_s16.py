import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p16 = json.load(open("tools/gemini/out/round-4b/player/stage_16.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e16 = doc["chapters"][1]["stages"][5]["enemy"]

# Allowed: T in [2..4], R in [rare..legendary]
tests = []
for t in [3, 4]:
    for r in ["epic", "legendary"]:
        items = copy.deepcopy(base_e16["items"])
        for it in items:
            it["tier"] = t
            it["rarity"] = r
        tests.append((t, r, items))

for t, r, items in tests:
    e = copy.deepcopy(base_e16)
    e["level"] = 16
    e["items"] = items
    wr, dur = simulate_stage(16, e, runs=200)
    print(f"T{t} {r:9}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(16, 'normal', p16, e, wr, dur)}")
