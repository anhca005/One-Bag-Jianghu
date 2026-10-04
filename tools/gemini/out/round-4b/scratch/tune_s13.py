import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p13 = json.load(open("tools/gemini/out/round-4b/player/stage_13.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e13 = doc["chapters"][1]["stages"][2]["enemy"]

# Test setting 1 or 2 items to T4 epic
tests = [
    # claw T4
    ["kich_doc_chu_nga"],
    # claw T4 + spider fang T4
    ["kich_doc_chu_nga", "soi_nanh_canh"],
    # claw T4 + venom relic T4
    ["kich_doc_chu_nga", "man_xa_doc_tui"],
    # armor T4
    ["heo_rung_bap_giap"],
    # claw T4 + armor T4
    ["kich_doc_chu_nga", "heo_rung_bap_giap"],
]

for t4_items in tests:
    items = copy.deepcopy(base_e13["items"])
    for it in items:
        if it["id"] in t4_items:
            it["tier"] = 4
    e = copy.deepcopy(base_e13)
    e["level"] = 13
    e["items"] = items
    wr, dur = simulate_stage(13, e, runs=200)
    print(f"T4={t4_items}: WR={wr:5.1f}%, dur={dur:4.1f}s, notes={check_rules(13, 'normal', p13, e, wr, dur)}")
