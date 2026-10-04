import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p20 = json.load(open("tools/gemini/out/round-4b/player/stage_20.json", encoding="utf-8"))

# Test mixing T4 and T5 items
base_items = [
    {"id": "tu_kim_trao",   "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
    {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
    {"id": "u_minh_bi_luc",  "x": 1, "y": 1, "tier": 4, "rarity": "epic"},
    {"id": "o_thiet_giap",  "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
    {"id": "van_doc_dinh",   "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
    {"id": "giai_doc_dan",   "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
]

def make_variant(tiers, rarities=None):
    items = json.loads(json.dumps(base_items))
    for i, t in enumerate(tiers):
        items[i]["tier"] = t
        if rarities and i < len(rarities) and rarities[i]:
            items[i]["rarity"] = rarities[i]
    return items

tests = [
    # weapon T5, rest T4
    ("Weapon T5, rest T4", make_variant([5, 4, 4, 4, 4, 4])),
    # armor T5, rest T4
    ("Armor T5, rest T4", make_variant([4, 4, 4, 5, 4, 4])),
    # man_xa T5, rest T4
    ("man_xa T5, rest T4", make_variant([4, 5, 4, 4, 4, 4])),
    # vdd T5, rest T4
    ("vdd T5, rest T4", make_variant([4, 4, 4, 4, 5, 4])),
    # weapon T5, armor T5
    ("weapon T5, armor T5", make_variant([5, 4, 4, 5, 4, 4])),
    # weapon leg, rest epic
    ("weapon T4 leg, rest T4 epic", make_variant([4, 4, 4, 4, 4, 4], ["legendary", "epic", "epic", "epic", "epic", "epic"])),
    # armor leg, rest epic
    ("armor T4 leg, rest T4 epic", make_variant([4, 4, 4, 4, 4, 4], ["epic", "epic", "epic", "legendary", "epic", "epic"])),
]

for name, items in tests:
    for tp in [2, 3]:
        e = {"name": "Hắc Độc Nương Tử", "sect": "huyen_doc", "tamPhapLevel": tp, "level": 21, "items": items, "characterId": "hac_doc_nuong_tu", "portrait": "boss_hac_doc_nuong_tu"}
        wr, dur = simulate_stage(20, e, runs=100)
        print(f"{name:30s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(20, 'boss', p20, e, wr, dur)}")
