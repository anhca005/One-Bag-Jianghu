import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p18 = json.load(open("tools/gemini/out/round-4b/player/stage_18.json", encoding="utf-8"))

# Test different defensive setups for enemy 18
# Grid 5x4:
# col 0: tu_kim_trao (1x2 at y=0), tu_kim_trao (1x2 at y=2)
# col 1-2: armor (2x2) like heo_rung_bap_giap or o_thiet_giap
# col 3: man_xa_doc_tui (1x1), van_doc_dinh (1x1), u_minh_bi_luc (1x1)
# col 4: pill or relic like hac_sat_hung_dam or giai_doc_dan

cands = [
    ("o_thiet_giap T3 epic", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 4, "rarity": "epic"},
        {"id": "o_thiet_giap", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "van_doc_dinh", "x": 3, "y": 1, "tier": 3, "rarity": "epic"},
        {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "epic"},
    ]),
    ("heo_rung T3 epic", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 4, "rarity": "epic"},
        {"id": "heo_rung_bap_giap", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "van_doc_dinh", "x": 3, "y": 1, "tier": 3, "rarity": "epic"},
        {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "epic"},
    ]),
    ("o_tam_y T3 epic", [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 4, "rarity": "epic"},
        {"id": "o_tam_y", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "van_doc_dinh", "x": 3, "y": 1, "tier": 3, "rarity": "epic"},
        {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "epic"},
    ]),
]

for name, items in cands:
    for tier in [3, 4]:
        for rarity in ["epic", "legendary"]:
            # Set weapons to tier/rarity
            it = json.loads(json.dumps(items))
            it[0]["tier"] = tier; it[0]["rarity"] = rarity
            it[1]["tier"] = tier; it[1]["rarity"] = rarity
            e = {"name": "Huyền Độc Trưởng Lão", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 18, "items": it}
            wr, dur = simulate_stage(18, e, runs=100)
            print(f"{name:20s} T{tier} {rarity:9s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(18, 'normal', p18, e, wr, dur)}")
