# Test adjustment for stage 14 and 15
import json, subprocess, sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[5]
CATALOG = ROOT / "tools" / "gemini" / "out" / "pve" / "combined_items.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

p14 = {
    "name": "Người Chơi (Ải 14)",
    "hp": 1350,
    "cols": 5, "rows": 4,
    "sect": "thai_thanh",
    "tamPhapLevel": 2,
    "items": [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "rare"}
    ]
}

e14 = {
    "name": "Ải 14: Man Cương Vu Đao",
    "hp": 1250,
    "cols": 5, "rows": 4,
    "sect": "huyen_doc",
    "tamPhapLevel": 2,
    "items": [
        {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "da_ho_y", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "man_xa_doc_tui", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}
    ]
}

p15 = {
    "name": "Người Chơi (Ải 15)",
    "hp": 1400,
    "cols": 5, "rows": 4,
    "sect": "thai_thanh",
    "tamPhapLevel": 2,
    "items": [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "rare"}
    ]
}

e15 = {
    "name": "Ải 15: Vạn Hoa Nghịch Đồ",
    "hp": 1380,
    "cols": 5, "rows": 4,
    "sect": "bich_thuy",
    "tamPhapLevel": 2,
    "items": [
        {"id": "bich_thuy_cam", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "nho_sam_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"}
    ]
}

with open("tools/gemini/out/pve/balance/c2_player/stage_14.json", "w", encoding="utf-8") as f:
    json.dump(p14, f, indent=2)
with open("tools/gemini/out/pve/balance/c2_enemy/stage_14.json", "w", encoding="utf-8") as f:
    json.dump(e14, f, indent=2)
with open("tools/gemini/out/pve/balance/c2_player/stage_15.json", "w", encoding="utf-8") as f:
    json.dump(p15, f, indent=2)
with open("tools/gemini/out/pve/balance/c2_enemy/stage_15.json", "w", encoding="utf-8") as f:
    json.dump(e15, f, indent=2)

print("Updated stage 14 and 15 test files.")
