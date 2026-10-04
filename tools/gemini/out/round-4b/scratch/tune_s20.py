import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p20 = json.load(open("tools/gemini/out/round-4b/player/stage_20.json", encoding="utf-8"))

# Enemy 20: Hắc Độc Nương Tử (boss_hac_doc_nuong_tu, sect="huyen_doc", characterId="hac_doc_nuong_tu")
# Boss level MUST be 21
# Allowed tier: 3..5, rarity: rare..supreme
# Target WR: [52, 58]%, dur <= 40s

# Original layout:
# tu_kim_trao (1x2 at x=0, y=0)
# man_xa_doc_tui (1x1 at x=1, y=0)
# u_minh_bi_luc (1x1 at x=1, y=1)
# o_thiet_giap (2x2 at x=2, y=0)
# van_doc_dinh (1x1 at x=4, y=0)
# giai_doc_dan (1x1 at x=4, y=1)

tests = [
    # T4 epic all
    ("T4 epic all", [
        {"id": "tu_kim_trao",   "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "u_minh_bi_luc",  "x": 1, "y": 1, "tier": 4, "rarity": "epic"},
        {"id": "o_thiet_giap",  "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "van_doc_dinh",   "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "giai_doc_dan",   "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
    ]),
    # T5 epic all
    ("T5 epic all", [
        {"id": "tu_kim_trao",   "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "u_minh_bi_luc",  "x": 1, "y": 1, "tier": 5, "rarity": "epic"},
        {"id": "o_thiet_giap",  "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "van_doc_dinh",   "x": 4, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "giai_doc_dan",   "x": 4, "y": 1, "tier": 5, "rarity": "epic"},
    ]),
    # T4 leg all
    ("T4 leg all", [
        {"id": "tu_kim_trao",   "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "u_minh_bi_luc",  "x": 1, "y": 1, "tier": 4, "rarity": "legendary"},
        {"id": "o_thiet_giap",  "x": 2, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "van_doc_dinh",   "x": 4, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "giai_doc_dan",   "x": 4, "y": 1, "tier": 4, "rarity": "legendary"},
    ]),
    # Dual tu_kim_trao: add second claw at x=0, y=2
    ("Dual tu_kim T4 epic", [
        {"id": "tu_kim_trao",   "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "tu_kim_trao",   "x": 0, "y": 2, "tier": 4, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "u_minh_bi_luc",  "x": 1, "y": 1, "tier": 4, "rarity": "epic"},
        {"id": "o_thiet_giap",  "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "van_doc_dinh",   "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "giai_doc_dan",   "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
    ]),
]

for name, items in tests:
    for tp in [2, 3]:
        e = {"name": "Hắc Độc Nương Tử", "sect": "huyen_doc", "tamPhapLevel": tp, "level": 21, "items": items, "characterId": "hac_doc_nuong_tu", "portrait": "boss_hac_doc_nuong_tu"}
        wr, dur = simulate_stage(20, e, runs=100)
        print(f"{name:25s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(20, 'boss', p20, e, wr, dur)}")
