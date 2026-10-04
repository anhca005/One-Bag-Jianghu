import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p6 = json.load(open("tools/gemini/out/round-4b/player/stage_06.json", encoding="utf-8"))

items = [
    {"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
    {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 2, "rarity": "rare"},
    {"id": "da_ho_y", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
    {"id": "phi_yen_quyet", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
    {"id": "thiet_giap_y", "x": 3, "y": 0, "tier": 2, "rarity": "rare"}
]

e = {"name": "Ám Hạng Thích Khách", "sect": "thai_thanh", "tamPhapLevel": 1, "level": 6, "items": items}
wr, dur = simulate_stage(6, e, runs=200)
print(f"S6 with tu_kim_trao: WR = {wr:.1f}%, dur = {dur:.1f}s, notes = {check_rules(6, 'normal', p6, e, wr, dur)}")
