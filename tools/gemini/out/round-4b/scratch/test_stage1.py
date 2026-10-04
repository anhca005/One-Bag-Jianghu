import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from sim_helper import simulate_enemy

base_enemy = {
    "name": "Thảo Khấu Tiên Phong",
    "sect": None,
    "tamPhapLevel": 1,
    "level": 1,
    "items": []
}

from sim_helper import simulate_enemy, ITEMS_DB

base_enemy = {
    "name": "Thảo Khấu Tiên Phong",
    "sect": None,
    "tamPhapLevel": 1,
    "level": 1,
    "items": []
}

# Let's inspect size of items
for iid in ["thiet_cot_quyen_sao", "that_tinh_tieu", "thanh_phong_kiem", "thanh_dang_tien", "tu_kim_trao"]:
    print(iid, "size:", ITEMS_DB[iid].get("size"))

candidates = [
    # 1. thanh_dang_tien (common T1) + boi_nguyen_dan (common T1)
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "boi_nguyen_dan", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # 2. thanh_phong_kiem (uncommon T1) + thiet_giap_y (common T1)
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
     {"id": "thiet_giap_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # 3. thiet_cot_quyen_sao (uncommon T1) + thiet_giap_y (uncommon T1)
    [{"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
     {"id": "thiet_giap_y", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"}],
    # 4. thanh_dang_tien (T2 common) without armor
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 2, "rarity": "common"}],
    # 5. thanh_phong_kiem (T1 common) + thiet_cot_quyen_sao (T1 common)
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # 6. thanh_phong_kiem (T1 common) + thiet_giap_y (T1 common)
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "thiet_giap_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
]

for idx, c in enumerate(candidates, 1):
    e = dict(base_enemy)
    e["items"] = c
    wr, dur = simulate_enemy(1, e, runs=200)
    print(f"Candidate {idx}: WR = {wr:.1f}%, Duration = {dur:.1f}s")

