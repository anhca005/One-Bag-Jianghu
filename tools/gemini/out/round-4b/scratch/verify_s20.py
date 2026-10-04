import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p20 = json.load(open("tools/gemini/out/round-4b/player/stage_20.json", encoding="utf-8"))

# Test variations around Armor T5, rest T4 at 200 runs
tests = [
    # 1. Armor T5, rest T4, tp=3
    ("Armor T5 epic, rest T4 epic, tp=3", 3, [
        {"id": "tu_kim_trao",   "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "u_minh_bi_luc",  "x": 1, "y": 1, "tier": 4, "rarity": "epic"},
        {"id": "o_thiet_giap",  "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "van_doc_dinh",   "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "giai_doc_dan",   "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
    ]),
    # 2. What if u_minh_bi_luc is T5?
    ("Armor T5 + umbl T5, rest T4, tp=3", 3, [
        {"id": "tu_kim_trao",   "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "u_minh_bi_luc",  "x": 1, "y": 1, "tier": 5, "rarity": "epic"},
        {"id": "o_thiet_giap",  "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "van_doc_dinh",   "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "giai_doc_dan",   "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
    ]),
    # 3. What if giai_doc_dan is T5?
    ("Armor T5 + giai_doc T5, rest T4, tp=3", 3, [
        {"id": "tu_kim_trao",   "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "u_minh_bi_luc",  "x": 1, "y": 1, "tier": 4, "rarity": "epic"},
        {"id": "o_thiet_giap",  "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "van_doc_dinh",   "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "giai_doc_dan",   "x": 4, "y": 1, "tier": 5, "rarity": "epic"},
    ]),
]

for name, tp, items in tests:
    e = {"name": "Hắc Độc Nương Tử", "sect": "huyen_doc", "tamPhapLevel": tp, "level": 21, "items": items, "characterId": "hac_doc_nuong_tu", "portrait": "boss_hac_doc_nuong_tu"}
    wr, dur = simulate_stage(20, e, runs=200)
    print(f"{name:40s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(20, 'boss', p20, e, wr, dur)}")
