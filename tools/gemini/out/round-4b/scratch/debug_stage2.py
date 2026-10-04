import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from sim_helper import simulate_enemy, ITEMS_DB

base_enemy = {
    "name": "Thủy Phỉ Thám Tử",
    "sect": None,
    "tamPhapLevel": 1,
    "level": 2,
    "items": []
}

candidates = [
    # thanh_phong_kiem + o_tam_y
    [{"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # thiet_cot_quyen_sao + o_tam_y
    [{"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
    # tu_kim_trao + o_tam_y
    [{"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
     {"id": "o_tam_y", "x": 1, "y": 0, "tier": 1, "rarity": "common"}],
]







for idx, c in enumerate(candidates, 1):
    e = dict(base_enemy)
    e["items"] = c
    wr, dur = simulate_enemy(2, e, runs=80)
    print(f"Cand {idx:2d}: WR={wr:5.1f}%, dur={dur:4.1f}s | {[(it['id'], it['tier'], it['rarity']) for it in c]}")

