import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p22 = json.load(open("tools/gemini/out/round-4b/player/stage_22.json", encoding="utf-8"))

# Test mixing legendary and epic on T5 6 items
# Items: thanh_phong_kiem, bang_tam_giap, han_ngoc_tieu, han_bang_chau, phi_yen_quyet, tuy_tien_ho_lo
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
    # Sword leg, flute leg, rest epic
    ("w1 leg, w2 leg, rest epic", make_s22("legendary", "legendary", "epic", "epic", "epic", "epic")),
    # Sword leg, flute epic, rest epic
    ("w1 leg, rest epic", make_s22("legendary", "epic", "epic", "epic", "epic", "epic")),
    # Sword epic, flute leg, rest epic
    ("w2 leg, rest epic", make_s22("epic", "legendary", "epic", "epic", "epic", "epic")),
    # Sword leg, armor leg, rest epic
    ("w1 leg, armor leg, rest epic", make_s22("legendary", "epic", "legendary", "epic", "epic", "epic")),
    # w1 leg, w2 leg, armor leg, rest epic
    ("w1 leg, w2 leg, armor leg, rest epic", make_s22("legendary", "legendary", "legendary", "epic", "epic", "epic")),
]

for name, items in tests:
    for tp in [1, 2]:
        e = {"name": "Hàn Đàm Cổ Kiếm", "sect": "han_son", "tamPhapLevel": tp, "level": 22, "items": items}
        wr, dur = simulate_stage(22, e, runs=100)
        print(f"{name:40s} tp={tp}: WR = {wr:5.1f}%, dur = {dur:4.1f}s, notes = {check_rules(22, 'normal', p22, e, wr, dur)}")
