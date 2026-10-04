import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p24 = json.load(open("tools/gemini/out/round-4b/player/stage_24.json", encoding="utf-8"))

# Enemy 24: Hộ Pháp Kim Cang (sect: kim_cang)
# Recommended level: 24 (allowed 22..26)
# Player 24: top tier 4, top rarity epic -> allowed enemy tier 3..5, rarity rare..legendary
# Items originally: phuc_ho_con, vo_niem_kinh, phuc_ma_giap, thien_tam_chuoi (were T6 epic)

# Let's check sizes:
# phuc_ho_con: [1, 3] or [1, 4]?
# phuc_ma_giap: [2, 2]
# vo_niem_kinh: [1, 2]
# thien_tam_chuoi: [1, 1]

tests = [
    # T5 epic
    ("T5 epic", [
        {"id": "phuc_ho_con",    "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "phuc_ma_giap",   "x": 1, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "vo_niem_kinh",   "x": 3, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "thien_tam_chuoi","x": 4, "y": 0, "tier": 5, "rarity": "epic"},
    ]),
    # T5 leg
    ("T5 leg", [
        {"id": "phuc_ho_con",    "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "phuc_ma_giap",   "x": 1, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "vo_niem_kinh",   "x": 3, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "thien_tam_chuoi","x": 4, "y": 0, "tier": 5, "rarity": "legendary"},
    ]),
    # T4 leg
    ("T4 leg", [
        {"id": "phuc_ho_con",    "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "phuc_ma_giap",   "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "vo_niem_kinh",   "x": 3, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "thien_tam_chuoi","x": 4, "y": 0, "tier": 4, "rarity": "legendary"},
    ]),
]

for name, items in tests:
    for lvl in [24, 25]:
        for tp in [1, 2]:
            e = {"name": "Hộ Pháp Kim Cang", "sect": "kim_cang", "tamPhapLevel": tp, "level": lvl, "items": items}
            wr, dur = simulate_stage(24, e, runs=100)
            print(f"{name:15s} lv={lvl} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(24, 'normal', p24, e, wr, dur)}")
