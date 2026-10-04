import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))

tests = [
    # tu_kim_trao T2 rare, that_tinh_tieu T3 rare, o_tam_y T3 rare, TP=2
    (2, 2, [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "o_tam_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "rare"}]),
    # tu_kim_trao T3 rare, that_tinh_tieu T2 rare, o_tam_y T3 rare, TP=2
    (2, 2, [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "o_tam_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "rare"}]),
    # all T3 rare, TP=1
    (2, 1, [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "o_tam_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "rare"}]),
    # tu_kim_trao T2 rare, that_tinh_tieu T2 rare, o_tam_y T3 rare, TP=2
    (2, 2, [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "o_tam_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 3, "rarity": "rare"}]),
]

for idx, (arm_t, tp, items) in enumerate(tests, 1):
    e = {"name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": tp, "level": 17, "items": items}
    wr, dur = simulate_stage(17, e, runs=200)
    print(f"Cand {idx} (TP={tp}): WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(17, 'normal', p17, e, wr, dur)}")
