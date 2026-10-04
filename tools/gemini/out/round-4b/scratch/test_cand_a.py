import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p1 = json.load(open("tools/gemini/out/round-4b/player/stage_01.json", encoding="utf-8"))

cand_a = [
    {"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
    {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"}
]

for seed in range(5):
    e1 = {"name": "Thảo Khấu Tiên Phong", "sect": None, "tamPhapLevel": 1, "level": 1, "items": cand_a}
    wr, dur = simulate_stage(1, e1, runs=200)
    print(f"Run {seed+1}: WR = {wr:.1f}%, dur = {dur:.1f}s, notes = {check_rules(1, 'normal', p1, e1, wr, dur)}")
