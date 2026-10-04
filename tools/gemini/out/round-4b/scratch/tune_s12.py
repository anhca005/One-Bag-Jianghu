import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p12 = json.load(open("tools/gemini/out/round-4b/player/stage_12.json", encoding="utf-8"))
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
base_e12 = doc["chapters"][1]["stages"][1]["enemy"]

# Original: 2 claws, o_tam_y, u_minh_bi_luc
# Allowed: T in [2..4], R in [uncommon..epic]
tests = [
    # 1 claw T3 epic, 1 claw T2 epic, o_tam_y T2 epic, u_minh T2 epic
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
     {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 2, "rarity": "epic"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "u_minh_bi_luc", "x": 3, "y": 0, "tier": 2, "rarity": "epic"}],
    # 1 claw T3 rare, 1 claw T2 rare, o_tam_y T2 epic, u_minh T2 epic
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 2, "rarity": "rare"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "u_minh_bi_luc", "x": 3, "y": 0, "tier": 2, "rarity": "epic"}],
    # 1 claw T3 rare, 1 claw T2 epic, o_tam_y T2 epic, u_minh T2 epic
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 2, "rarity": "epic"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "u_minh_bi_luc", "x": 3, "y": 0, "tier": 2, "rarity": "epic"}],
    # T2 epic with a pill or poison: e.g. van_doc_dinh (T2 epic)
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 2, "rarity": "epic"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "u_minh_bi_luc", "x": 3, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 2, "rarity": "epic"}],
]


for idx, items in enumerate(tests, 1):
    e = copy.deepcopy(base_e12)
    e["level"] = 12
    e["items"] = items
    wr, dur = simulate_stage(12, e, runs=200)
    print(f"Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(12, 'normal', p12, e, wr, dur)}")
