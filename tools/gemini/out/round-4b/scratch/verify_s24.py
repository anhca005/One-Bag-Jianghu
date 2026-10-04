import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p24 = json.load(open("tools/gemini/out/round-4b/player/stage_24.json", encoding="utf-8"))

tests = [
    # Staff T5 epic, Fist T3 rare, armor T5 epic
    ("Staff T5 epic, Fist T3 rare, armor T5 epic", [
        {"id": "phuc_ho_con",         "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "thiet_giap_y",        "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "vo_niem_kinh",        "x": 4, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "thien_tam_chuoi",     "x": 4, "y": 2, "tier": 5, "rarity": "epic"},
    ]),
    # Staff T4 epic, Fist T4 rare, armor T4 epic
    ("Staff T4 epic, Fist T4 rare, armor T4 epic", [
        {"id": "phuc_ho_con",         "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "thiet_giap_y",        "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "vo_niem_kinh",        "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "thien_tam_chuoi",     "x": 4, "y": 2, "tier": 4, "rarity": "epic"},
    ]),
    # Staff T5 epic, Fist T4 rare, armor T5 rare
    ("Staff T5 epic, Fist T4 rare, armor T5 rare", [
        {"id": "phuc_ho_con",         "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "thiet_giap_y",        "x": 2, "y": 0, "tier": 5, "rarity": "rare"},
        {"id": "vo_niem_kinh",        "x": 4, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "thien_tam_chuoi",     "x": 4, "y": 2, "tier": 5, "rarity": "epic"},
    ]),
]

for name, items in tests:
    e = {"name": "Hộ Pháp Kim Cang", "sect": "kim_cang", "tamPhapLevel": 2, "level": 24, "items": items}
    wr, dur = simulate_stage(24, e, runs=200)
    print(f"{name:45s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(24, 'normal', p24, e, wr, dur)}")
