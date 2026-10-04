import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e17 = doc["chapters"][1]["stages"][6]["enemy"]

# Original items: that_tinh_tieu, that_tinh_tieu, da_ho_y, van_doc_dinh, u_minh_bi_luc
# Allowed: T in [2..4], R in [rare..legendary]
tests = [
    # claw T4 + dart T4 + o_tam_y T4 leg + van_doc_dinh T3 leg + u_minh T3 leg
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "o_tam_y", "x": 2, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
    # 2 claws T4 + o_tam_y T4 leg + van_doc_dinh T3 leg + u_minh T3 leg
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "o_tam_y", "x": 2, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
    # poison claw T4 + dart T4 + o_tam_y T4 leg
    [{"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "o_tam_y", "x": 2, "y": 0, "tier": 4, "rarity": "legendary"},
     {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
     {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}],
]


for idx, items in enumerate(tests, 1):
    e = copy.deepcopy(base_e17)
    e["level"] = 17
    e["items"] = items
    wr, dur = simulate_stage(17, e, runs=200)
    print(f"Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(17, 'normal', p17, e, wr, dur)}")

