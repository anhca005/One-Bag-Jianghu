import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p22 = json.load(open("tools/gemini/out/round-4b/player/stage_22.json", encoding="utf-8"))

# Enemy 22: Hàn Đàm Cổ Kiếm
# Level 22, sect han_son
# Allowed: tier 3..5, rarity: rare..legendary

# Let's test combinations with 5-6 items, tier 4-5, legendary
tests = [
    # C1: thanh_phong_kiem + han_ngoc_tieu + bang_tam_giap + han_bang_chau + phi_yen_quyet + tuy_tien_ho_lo
    ("T5 leg 6 items", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "han_ngoc_tieu",     "x": 3, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "phi_yen_quyet",     "x": 4, "y": 1, "tier": 5, "rarity": "legendary"},
        {"id": "tuy_tien_ho_lo",    "x": 1, "y": 1, "tier": 5, "rarity": "legendary"},
    ]),
    # C2: T5 epic 6 items
    ("T5 epic 6 items", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "han_ngoc_tieu",     "x": 3, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": 5, "rarity": "epic"},
        {"id": "phi_yen_quyet",     "x": 4, "y": 1, "tier": 5, "rarity": "epic"},
        {"id": "tuy_tien_ho_lo",    "x": 1, "y": 1, "tier": 5, "rarity": "epic"},
    ]),
    # C3: dual thanh_phong_kiem + han_ngoc_tieu
    ("Dual thanh_phong + han_ngoc T4 leg", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "bang_tam_giap",     "x": 2, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 0, "tier": 4, "rarity": "legendary"},
        {"id": "han_bang_chau",     "x": 4, "y": 2, "tier": 4, "rarity": "legendary"},
        {"id": "phi_yen_quyet",     "x": 4, "y": 3, "tier": 4, "rarity": "legendary"},
    ]),
    # C4: dual thanh_phong + han_ngoc T5 leg
    ("Dual thanh_phong + han_ngoc T5 leg", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "bang_tam_giap",     "x": 2, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "han_ngoc_tieu",     "x": 4, "y": 0, "tier": 5, "rarity": "legendary"},
        {"id": "han_bang_chau",     "x": 4, "y": 2, "tier": 5, "rarity": "legendary"},
        {"id": "phi_yen_quyet",     "x": 4, "y": 3, "tier": 5, "rarity": "legendary"},
    ]),
]

for name, items in tests:
    for tp in [1, 2, 3]:
        e = {"name": "Hàn Đàm Cổ Kiếm", "sect": "han_son", "tamPhapLevel": tp, "level": 22, "items": items}
        wr, dur = simulate_stage(22, e, runs=100)
        print(f"{name:35s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(22, 'normal', p22, e, wr, dur)}")
