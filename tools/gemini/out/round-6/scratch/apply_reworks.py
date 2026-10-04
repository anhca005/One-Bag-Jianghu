import json
import sys
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R6 = ROOT / "tools" / "gemini" / "out" / "round-6"
LOADOUTS_DIR = R6 / "loadouts"

ITEMS = json.load(open(ROOT / "server/battle-engine/src/main/resources/data/items.json", encoding="utf-8"))
ITEM_MAP = {it["id"]: it for it in ITEMS}

def check_layout(items_list):
    grid = [[None for _ in range(4)] for _ in range(5)]
    for it in items_list:
        iid = it["id"]
        x, y = it["x"], it["y"]
        meta = ITEM_MAP.get(iid)
        if not meta:
            return False, f"Unknown item {iid}"
        w, h = meta["size"]
        if x < 0 or y < 0 or x + w > 5 or y + h > 4:
            return False, f"Out of bounds: {iid} at ({x},{y}) size ({w},{h})"
        for dx in range(w):
            for dy in range(h):
                if grid[x + dx][y + dy] is not None:
                    return False, f"Overlap at ({x+dx},{y+dy}) between {grid[x+dx][y+dy]} and {iid}"
                grid[x + dx][y + dy] = iid
    return True, "OK"

# 1. thai_thanh.json: Kiếm Đạo Hóa Kình
thai_thanh = {
  "name": "Thái Thanh Quán - Kiếm Đạo Hóa Kình",
  "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "ngoc_cot_phien", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "kiem_tong_tam_yeu", "x": 0, "y": 3, "rarity": "rare"},
    {"id": "thai_thanh_tam_kinh", "x": 1, "y": 2, "rarity": "epic"},
    {"id": "tuy_tien_ho_lo", "x": 1, "y": 3, "rarity": "rare"},
    {"id": "thien_tam_chuoi", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "bach_ngoc_boi", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "nho_sam_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 3, "rarity": "common"},
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

# 2. hybrid_thiet_phu_liet_diem.json: Thiết Phủ Liệt Diễm
thiet_phu_liet_diem = {
  "name": "Thiết Phủ Bang - Thiết Phủ Liệt Diễm",
  "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "khai_son_phu", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "xich_sa_chuong", "x": 0, "y": 2, "rarity": "rare"},
    {"id": "chu_sa_ho_lo", "x": 1, "y": 2, "rarity": "rare"},
    {"id": "liet_hoa_tam_kinh", "x": 1, "y": 3, "rarity": "epic"},
    {"id": "liet_diem_dao", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_thiet_giap", "x": 3, "y": 0, "rarity": "epic"},
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

# 3. new_thiet_phu_hammer.json: Chấn Nhạc Chùy Phong
chuy_phong = {
  "name": "Thiết Phủ Bang - Chấn Nhạc Chùy Phong",
  "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "chan_nhac_chuy", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "khai_son_phu", "x": 0, "y": 2, "rarity": "epic"},
    {"id": "thiet_cot_quyen_sao", "x": 2, "y": 0, "rarity": "rare"},
    {"id": "bach_luyen_chan_kinh", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "o_thiet_giap", "x": 3, "y": 0, "rarity": "epic"},
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
    {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

# 4. kim_cang_bach_luyen.json: Bách Luyện Thuần Quyền
bach_luyen = {
  "name": "Kim Cang Tự - Bách Luyện Thuần Quyền",
  "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "phuc_ho_con", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 2, "rarity": "rare"},
    {"id": "bach_luyen_chan_kinh", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "tu_kim_bat", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "cuu_chuyen_hoan_hon_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 4, "y": 2, "rarity": "uncommon"},
    {"id": "thiet_giap_y", "x": 3, "y": 3, "rarity": "rare"}
  ]
}

# 5. hybrid_han_son_bich_thuy.json: Hàn Bích Hợp Bích
han_bich = {
  "name": "Hàn Sơn Phái - Hàn Bích Hợp Bích",
  "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "han_ngoc_tieu", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "phan_quan_but", "x": 0, "y": 2, "rarity": "epic"},
    {"id": "thanh_dang_tien", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "tram_huong_lo", "x": 1, "y": 3, "rarity": "epic"},
    {"id": "duong_sinh_quyet", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "thanh_tam_dan", "x": 2, "y": 2, "rarity": "uncommon"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "bang_tam_giap", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "nho_sam_y", "x": 3, "y": 1, "rarity": "rare"},
    {"id": "o_tam_y", "x": 3, "y": 2, "rarity": "rare"}
  ]
}

# 6. bich_thuy_duong_sinh.json: Dưỡng Sinh Cầm Âm
duong_sinh = {
  "name": "Bích Thủy Am - Dưỡng Sinh Cầm Âm",
  "cols": 5, "rows": 4, "sect": "bich_thuy", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_dang_tien", "x": 0, "y": 0, "rarity": "rare"},
    {"id": "kim_sang_duoc", "x": 0, "y": 3, "rarity": "uncommon"},
    {"id": "bich_thuy_cam", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "duong_sinh_quyet", "x": 1, "y": 2, "rarity": "epic"},
    {"id": "tram_huong_lo", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "tue_nguyet_bao_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "phan_quan_but", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

# 7. hybrid_thai_thanh_liet_diem.json: Thái Thanh Liệt Diễm
thai_thanh_liet_diem = {
  "name": "Thái Thanh Quán - Thái Thanh Liệt Diễm",
  "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thai_thanh_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
    {"id": "ngoc_cot_phien", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "liet_hoa_tam_kinh", "x": 1, "y": 2, "rarity": "epic"},
    {"id": "chu_sa_ho_lo", "x": 1, "y": 3, "rarity": "rare"},
    {"id": "liet_diem_dao", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "kiem_tong_tam_yeu", "x": 2, "y": 3, "rarity": "epic"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "bach_ngoc_boi", "x": 4, "y": 2, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

# 8. thai_thanh_kiem_khi.json: Kiếm Khí Xuyên Giáp
thai_thanh_kiem_khi = {
  "name": "Thái Thanh Quán - Kiếm Khí Xuyên Giáp",
  "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thai_thanh_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
    {"id": "ngoc_cot_phien", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "kiem_tong_tam_yeu", "x": 1, "y": 2, "rarity": "epic"},
    {"id": "bach_ngoc_boi", "x": 1, "y": 3, "rarity": "epic"},
    {"id": "thien_tam_chuoi", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "tuy_tien_ho_lo", "x": 2, "y": 2, "rarity": "rare"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "thiet_giap_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "van_khi_dan", "x": 3, "y": 3, "rarity": "uncommon"},
    {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 3, "rarity": "common"}
  ]
}

reworks = [
    ("thai_thanh.json", thai_thanh),
    ("hybrid_thiet_phu_liet_diem.json", thiet_phu_liet_diem),
    ("new_thiet_phu_hammer.json", chuy_phong),
    ("kim_cang_bach_luyen.json", bach_luyen),
    ("hybrid_han_son_bich_thuy.json", han_bich),
    ("bich_thuy_duong_sinh.json", duong_sinh),
    ("hybrid_thai_thanh_liet_diem.json", thai_thanh_liet_diem),
    ("thai_thanh_kiem_khi.json", thai_thanh_kiem_khi),
]

for fname, data in reworks:
    ok, msg = check_layout(data["items"])
    if not ok:
        print(f"ERROR in {fname}: {msg}")
        sys.exit(1)
    with open(LOADOUTS_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
    print(f"Updated {fname}")

print("\nAll 8 loadouts updated and verified valid layout!")
