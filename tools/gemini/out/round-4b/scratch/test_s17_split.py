import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))

# tu_kim_trao T3 rare, that_tinh_tieu T3 rare, da_ho_y T3 rare + 2nd defensive/relic item
tests = [
    # da_ho_y + thiet_giap_y
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "thiet_giap_y", "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "van_doc_dinh", "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
     {"id": "u_minh_bi_luc", "x": 3, "y": 1, "tier": 3, "rarity": "rare"}],
    # da_ho_y + bach_ngoc_boi (relic)
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "bach_ngoc_boi", "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "u_minh_bi_luc", "x": 3, "y": 1, "tier": 3, "rarity": "rare"}],
    # da_ho_y + boi_nguyen_dan (pill)
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "boi_nguyen_dan", "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
     {"id": "u_minh_bi_luc", "x": 3, "y": 1, "tier": 3, "rarity": "rare"}],
]

for idx, items in enumerate(tests, 1):
    e = {"name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 17, "items": items}
    wr, dur = simulate_stage(17, e, runs=200)
    print(f"Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(17, 'normal', p17, e, wr, dur)}")
