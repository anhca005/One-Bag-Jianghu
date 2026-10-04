import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

p14 = json.load(open("tools/gemini/out/round-4b/player/stage_14.json", encoding="utf-8"))
# Allowed: T in [2..4], R in [uncommon..epic]

tests_s14 = [
    # Cand 2 with kich_doc_chu_nga T4 epic
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
     {"id": "man_xa_doc_tui", "x": 1, "y": 2, "tier": 3, "rarity": "epic"},
     {"id": "nho_sam_y", "x": 2, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "duong_sinh_quyet", "x": 4, "y": 0, "tier": 2, "rarity": "epic"}],
    # Cand 2 with man_xa_doc_tui T4 epic
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
     {"id": "man_xa_doc_tui", "x": 1, "y": 2, "tier": 4, "rarity": "epic"},
     {"id": "nho_sam_y", "x": 2, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "duong_sinh_quyet", "x": 4, "y": 0, "tier": 2, "rarity": "epic"}],
    # Cand 2 with a pill (hoi_thien_dan)
    [{"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
     {"id": "man_xa_doc_tui", "x": 1, "y": 2, "tier": 3, "rarity": "epic"},
     {"id": "nho_sam_y", "x": 2, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "duong_sinh_quyet", "x": 4, "y": 0, "tier": 2, "rarity": "epic"},
     {"id": "hoi_thien_dan", "x": 4, "y": 2, "tier": 2, "rarity": "epic"}],
]




for idx, items in enumerate(tests_s14, 1):
    tp = 1 if idx == 4 else 2
    e = {"name": "Man Cương Vu Đao", "sect": "bich_thuy", "tamPhapLevel": tp, "level": 14, "items": items}
    wr, dur = simulate_stage(14, e, runs=200)
    print(f"S14 Cand {idx} (TP={tp}): WR={wr:5.1f}%, dur={dur:4.1f}s, notes={check_rules(14, 'normal', p14, e, wr, dur)}")

