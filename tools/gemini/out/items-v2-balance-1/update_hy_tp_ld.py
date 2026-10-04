import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
loadouts_dir = ROOT / "tools/gemini/out/items-v2-balance-1/loadouts"

# Update hybrid_thiet_phu_liet_diem with proper adjacency
tp_hy = {
    "name": "Thiết Phủ Bang - Thiết Phủ Liệt Diễm",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1,
    "items": [
        {"id": "khai_son_phu", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "chan_nhac_chuy", "x": 0, "y": 2, "rarity": "epic"},
        {"id": "tu_mau_song_dao", "x": 2, "y": 0, "rarity": "epic"},
        {"id": "o_thiet_giap", "x": 2, "y": 2, "rarity": "epic"},
        {"id": "chu_sa_ho_lo", "x": 4, "y": 0, "rarity": "epic"},
        {"id": "cuong_hoa_dan", "x": 4, "y": 1, "rarity": "uncommon"},
        {"id": "da_ho_y", "x": 4, "y": 2, "rarity": "rare"}
    ]
}

with open(loadouts_dir / "hybrid_thiet_phu_liet_diem.json", "w", encoding="utf-8") as f:
    json.dump(tp_hy, f, ensure_ascii=False, indent=2)

print("Updated hybrid_thiet_phu_liet_diem with chu_sa_ho_lo adjacent to tu_mau_song_dao!")
