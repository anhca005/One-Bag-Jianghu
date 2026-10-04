import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
loadouts_dir = ROOT / "tools/gemini/out/items-v2-balance-1/loadouts"

# Update hybrid_thai_thanh_liet_diem.json
hy_tt_ld = {
    "name": "Thái Thanh Quán - Thái Thanh Liệt Diễm",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1,
    "items": [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "thai_thanh_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
        {"id": "liet_diem_dao", "x": 1, "y": 0, "rarity": "epic"},
        {"id": "liet_hoa_tam_kinh", "x": 1, "y": 3, "rarity": "epic"},
        {"id": "kiem_tong_tam_yeu", "x": 2, "y": 0, "rarity": "epic"},
        {"id": "bach_ngoc_boi", "x": 2, "y": 1, "rarity": "epic"},
        {"id": "xich_sa_chuong", "x": 2, "y": 2, "rarity": "rare"},
        {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
        {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
        {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
    ]
}

with open(loadouts_dir / "hybrid_thai_thanh_liet_diem.json", "w", encoding="utf-8") as f:
    json.dump(hy_tt_ld, f, ensure_ascii=False, indent=2)

print("Updated hybrid_thai_thanh_liet_diem.json successfully!")
