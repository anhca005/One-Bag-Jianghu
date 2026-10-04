import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p22 = json.load(open("tools/gemini/out/round-4b/player/stage_22.json", encoding="utf-8"))

def make_s22(w1_r, w2_r, g_r, hbc_r, pyq_r, hl_r, t=5):
    return [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": t, "rarity": w1_r},
        {"id": "bang_tam_giap",     "x": 1, "y": 0, "tier": t, "rarity": g_r},
        {"id": "han_ngoc_tieu",     "x": 3, "y": 0, "tier": t, "rarity": w2_r},
        {"id": "han_bang_chau",     "x": 4, "y": 0, "tier": t, "rarity": hbc_r},
        {"id": "phi_yen_quyet",     "x": 4, "y": 1, "tier": t, "rarity": pyq_r},
        {"id": "tuy_tien_ho_lo",    "x": 1, "y": 1, "tier": t, "rarity": hl_r},
    ]

tests = [
    # w1 leg, w2 leg, rest epic
    ("w1 leg, w2 leg, rest epic", make_s22("legendary", "legendary", "epic", "epic", "epic", "epic")),
    # w1 leg, w2 leg, hbc leg, rest epic
    ("w1 leg, w2 leg, hbc leg, rest epic", make_s22("legendary", "legendary", "epic", "legendary", "epic", "epic")),
    # w1 leg, w2 leg, pyq leg, rest epic
    ("w1 leg, w2 leg, pyq leg, rest epic", make_s22("legendary", "legendary", "epic", "epic", "legendary", "epic")),
]

for name, items in tests:
    e = {"name": "Hàn Đàm Cổ Kiếm", "sect": "han_son", "tamPhapLevel": 1, "level": 22, "items": items}
    wr, dur = simulate_stage(22, e, runs=200)
    print(f"{name:35s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(22, 'normal', p22, e, wr, dur)}")
