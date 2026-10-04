import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
sys.path.insert(0, str(ROOT))
from tools.balance.balance import ensure_classpath, load_patch, patched_files, evaluate_matrix, load_battle_config, read_json

sys.stdout.reconfigure(encoding="utf-8")
cp = ensure_classpath()
loadouts_dir = ROOT / "tools/gemini/out/items-v2-balance-1/loadouts"
items_path = ROOT / "tools/gemini/out/items-v2/items-v2.normalized.json"
sects_path = ROOT / "server/battle-engine/src/main/resources/data/sects.json"

# Update thiet_phu.json to 3 axes
tp1 = {
    "name": "Thiết Phủ Bang - Bá Khí Khai Sơn",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1,
    "items": [
        {"id": "khai_son_phu", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "khai_son_phu", "x": 0, "y": 2, "rarity": "epic"},
        {"id": "khai_son_phu", "x": 2, "y": 0, "rarity": "rare"},
        {"id": "o_thiet_giap", "x": 2, "y": 2, "rarity": "epic"},
        {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
        {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
    ]
}
with open(loadouts_dir / "thiet_phu.json", "w", encoding="utf-8") as f:
    json.dump(tp1, f, ensure_ascii=False, indent=2)

# Update new_thiet_phu_hammer.json to 1 hammer + 2 axes
tp2 = {
    "name": "Thiết Phủ Bang - Chấn Nhạc Chùy Phong",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1,
    "items": [
        {"id": "chan_nhac_chuy", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "khai_son_phu", "x": 0, "y": 2, "rarity": "epic"},
        {"id": "khai_son_phu", "x": 2, "y": 0, "rarity": "rare"},
        {"id": "o_thiet_giap", "x": 2, "y": 2, "rarity": "epic"},
        {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
        {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
    ]
}
with open(loadouts_dir / "new_thiet_phu_hammer.json", "w", encoding="utf-8") as f:
    json.dump(tp2, f, ensure_ascii=False, indent=2)

# Update hybrid_thiet_phu_liet_diem.json to add xich_sa_chuong
tp_hy = {
    "name": "Thiết Phủ Bang - Thiết Phủ Liệt Diễm",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1,
    "items": [
        {"id": "khai_son_phu", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "tu_mau_song_dao", "x": 0, "y": 2, "rarity": "epic"},
        {"id": "o_thiet_giap", "x": 2, "y": 0, "rarity": "epic"},
        {"id": "xich_sa_chuong", "x": 2, "y": 2, "rarity": "rare"},
        {"id": "chu_sa_ho_lo", "x": 3, "y": 2, "rarity": "epic"},
        {"id": "cuong_hoa_dan", "x": 3, "y": 3, "rarity": "uncommon"},
        {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
        {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
    ]
}
with open(loadouts_dir / "hybrid_thiet_phu_liet_diem.json", "w", encoding="utf-8") as f:
    json.dump(tp_hy, f, ensure_ascii=False, indent=2)

# Update huyen_doc_van_doc.json: separate u_minh_bi_luc and huyet_ngoc_thach
# to break the multi-hit infinite charge loop
hd_vd = {
    "name": "Huyền Độc Môn - Vạn Độc Phệ Huyết",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "huyen_doc", "tamPhapLevel": 1,
    "items": [
        {"id": "that_tinh_tieu", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "that_tinh_tieu", "x": 0, "y": 2, "rarity": "epic"},
        {"id": "tu_kim_trao", "x": 1, "y": 0, "rarity": "epic"},
        {"id": "tu_kim_trao", "x": 1, "y": 2, "rarity": "epic"},
        {"id": "u_minh_bi_luc", "x": 2, "y": 0, "rarity": "epic"},
        {"id": "van_doc_dinh", "x": 2, "y": 1, "rarity": "epic"},
        {"id": "giai_doc_dan", "x": 2, "y": 2, "rarity": "uncommon"},
        {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
        {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
        {"id": "huyet_ngoc_thach", "x": 3, "y": 2, "rarity": "epic"},
        {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
        {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 3, "rarity": "common"},
        {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
    ]
}
with open(loadouts_dir / "huyen_doc_van_doc.json", "w", encoding="utf-8") as f:
    json.dump(hd_vd, f, ensure_ascii=False, indent=2)

print("Updated 4 loadouts successfully!")
