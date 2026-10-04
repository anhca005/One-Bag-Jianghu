import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p15 = json.load(open("tools/gemini/out/round-4b/player/stage_15.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e15 = doc["chapters"][1]["stages"][4]["enemy"]

# Original: bich_thuy_cam, nho_sam_y, hoi_thien_dan, thanh_tam_dan, duong_sinh_quyet (all T1 supreme)
# Test T2, T3 with different rarities
tests = [
    # all T2 supreme
    (2, "supreme"),
    # all T2 legendary
    (2, "legendary"),
    # all T2 epic
    (2, "epic"),
    # all T3 legendary
    (3, "legendary"),
    # all T3 epic
    (3, "epic"),
]

for t, r in tests:
    items = copy.deepcopy(base_e15["items"])
    for it in items:
        it["tier"] = t
        it["rarity"] = r
    e = copy.deepcopy(base_e15)
    e["level"] = 15
    e["items"] = items
    wr, dur = simulate_stage(15, e, runs=200)
    print(f"T{t} {r:9}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(15, 'elite', p15, e, wr, dur)}")
