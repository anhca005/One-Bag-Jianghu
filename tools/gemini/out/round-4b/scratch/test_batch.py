import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p1 = json.load(open("tools/gemini/out/round-4b/player/stage_01.json", encoding="utf-8"))
p2 = json.load(open("tools/gemini/out/round-4b/player/stage_02.json", encoding="utf-8"))
candidates_s1 = [
    # thanh_dang_tien + thanh_dong_ho_tam_kinh
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "thanh_dong_ho_tam_kinh", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # thanh_dang_tien + tuy_tien_ho_lo
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "tuy_tien_ho_lo", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # thanh_dang_tien + nho_sam_y
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "nho_sam_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # thanh_dang_tien + da_ho_y
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "da_ho_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # thanh_dang_tien + bach_ngoc_boi
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "bach_ngoc_boi", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # thanh_dang_tien (uncommon) + thanh_dong_ho_tam_kinh
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
     {"id": "thanh_dong_ho_tam_kinh", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
]


for idx, c in enumerate(candidates_s1, 1):
    e1 = {"name": "Thảo Khấu Tiên Phong", "sect": None, "tamPhapLevel": 1, "level": 1, "items": c}
    wr1, dur1 = simulate_stage(1, e1, runs=200)
    print(f"S1 Cand {idx}: WR = {wr1:5.1f}%, dur = {dur1:4.1f}s, notes = {check_rules(1, 'normal', p1, e1, wr1, dur1)}")


e2 = {
    "name": "Thủy Phỉ Thám Tử", "sect": None, "tamPhapLevel": 1, "level": 2,
    "items": [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "phuc_ho_con", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
    ]
}
wr2, dur2 = simulate_stage(2, e2, runs=200)
print("Stage 2 (200 runs): WR =", wr2, "dur =", dur2, "notes =", check_rules(2, "normal", p2, e2, wr2, dur2))

