import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p17 = json.load(open("tools/gemini/out/round-4b/player/stage_17.json", encoding="utf-8"))

# Enemy items originally:
# that_tinh_tieu, that_tinh_tieu, da_ho_y, van_doc_dinh, u_minh_bi_luc
# Sect: huyen_doc

# Allowed: tier in [2, 3, 4], rarity in ['rare', 'epic', 'legendary']

base_configs = [
    # Tier 3 or 4, epic or legendary
    ("T3 epic all", [{"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
                     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
                     {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
                     {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
                     {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "epic"}]),
    ("T4 epic all", [{"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
                     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
                     {"id": "da_ho_y", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
                     {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 4, "rarity": "epic"},
                     {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 4, "rarity": "epic"}]),
    ("T3 leg weapons/relic, epic armor/kungfu", 
                    [{"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
                     {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "epic"}]),
    ("T3 leg all",  [{"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"}]),
    ("T4 leg weapons, T3 leg rest",
                    [{"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
                     {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
                     {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "legendary"},
                     {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"}]),
]

for name, items in base_configs:
    e = {"name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 17, "items": items}
    wr, dur = simulate_stage(17, e, runs=100)
    print(f"{name:30s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(17, 'normal', p17, e, wr, dur)}")
