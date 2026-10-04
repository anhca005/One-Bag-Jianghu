import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p6 = json.load(open("tools/gemini/out/round-4b/player/stage_06.json", encoding="utf-8"))
# Original items: that_tinh_tieu, that_tinh_tieu, da_ho_y, phi_yen_quyet, thiet_giap_y
# Allowed E: level 6, T in [1..3], R in [uncommon..epic]

tests_s6 = [
    # that_tinh_tieu T3 rare, rest T2 rare
    ([{"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
      {"id": "that_tinh_tieu", "x": 0, "y": 2, "tier": 2, "rarity": "rare"},
      {"id": "da_ho_y", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
      {"id": "phi_yen_quyet", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
      {"id": "thiet_giap_y", "x": 3, "y": 0, "tier": 2, "rarity": "rare"}], 1),
    # both darts T3 rare
    ([{"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
      {"id": "that_tinh_tieu", "x": 0, "y": 2, "tier": 3, "rarity": "rare"},
      {"id": "da_ho_y", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
      {"id": "phi_yen_quyet", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
      {"id": "thiet_giap_y", "x": 3, "y": 0, "tier": 2, "rarity": "rare"}], 1),
    # all T2 rare, tamPhapLevel 2
    ([{"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
      {"id": "that_tinh_tieu", "x": 0, "y": 2, "tier": 2, "rarity": "rare"},
      {"id": "da_ho_y", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
      {"id": "phi_yen_quyet", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
      {"id": "thiet_giap_y", "x": 3, "y": 0, "tier": 2, "rarity": "rare"}], 2),
]

for idx, (items, tp) in enumerate(tests_s6, 1):
    e = {
        "name": "Ám Hạng Thích Khách", "sect": "thai_thanh", "tamPhapLevel": tp, "level": 6,
        "items": items
    }
    wr, dur = simulate_stage(6, e, runs=200)
    print(f"S6 Opt {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(6, 'normal', p6, e, wr, dur)}")


for idx, items in enumerate(tests_s6, 1):
    e = {
        "name": "Ám Hạng Thích Khách", "sect": "thai_thanh", "tamPhapLevel": 1, "level": 6,
        "items": items
    }
    wr, dur = simulate_stage(6, e, runs=200)
    print(f"S6 Cand {idx}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(6, 'normal', p6, e, wr, dur)}")
