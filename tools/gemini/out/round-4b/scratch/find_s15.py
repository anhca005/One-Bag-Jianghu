import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p15 = json.load(open("tools/gemini/out/round-4b/player/stage_15.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e15 = doc["chapters"][1]["stages"][4]["enemy"]

# Items: bich_thuy_cam, nho_sam_y, hoi_thien_dan, thanh_tam_dan, duong_sinh_quyet
# Test setting 1 or 2 items to supreme, rest legendary/epic
tests = [
    # zither + manual supreme, rest legendary
    ["bich_thuy_cam", "duong_sinh_quyet"],
    # zither + armor supreme, rest legendary
    ["bich_thuy_cam", "nho_sam_y"],
    # zither + pill (hoi_thien_dan) supreme, rest legendary
    ["bich_thuy_cam", "hoi_thien_dan"],
    # zither + thanh_tam_dan supreme, rest legendary
    ["bich_thuy_cam", "thanh_tam_dan"],
    # zither + manual + pill supreme, rest legendary
    ["bich_thuy_cam", "duong_sinh_quyet", "hoi_thien_dan"],
    # zither + armor + pill supreme, rest legendary
    ["bich_thuy_cam", "nho_sam_y", "hoi_thien_dan"],
]

for sup_items in tests:
    items = copy.deepcopy(base_e15["items"])
    for it in items:
        it["tier"] = 2
        it["rarity"] = "supreme" if it["id"] in sup_items else "legendary"
    e = copy.deepcopy(base_e15)
    e["level"] = 15
    e["items"] = items
    wr, dur = simulate_stage(15, e, runs=200)
    print(f"supreme={sup_items}: WR={wr:5.1f}%, dur={dur:4.1f}s, notes={check_rules(15, 'elite', p15, e, wr, dur)}")


