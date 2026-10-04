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

# 1. thai_thanh.json: 1 sword (1x3), 1 fan (1x2), relics/manuals
thai_thanh = {
  "name": "Thái Thanh Quán - Kiếm Đạo Hóa Kình",
  "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},       # (0,0..2)
    {"id": "ngoc_cot_phien", "x": 1, "y": 0, "rarity": "rare"},          # (1,0..1)
    {"id": "thai_thanh_tam_kinh", "x": 1, "y": 2, "rarity": "epic"},     # (1,2) 1x1
    {"id": "kiem_tong_tam_yeu", "x": 0, "y": 3, "rarity": "rare"},       # (0,3) 1x1
    {"id": "tuy_tien_ho_lo", "x": 1, "y": 3, "rarity": "rare"},          # (1,3) 1x1
    {"id": "thien_tam_chuoi", "x": 2, "y": 0, "rarity": "epic"},         # (2,0..1) 1x2
    {"id": "bach_ngoc_boi", "x": 2, "y": 2, "rarity": "epic"},           # (2,2) 1x1
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},       # (2,3) 1x1
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},                 # (3,0..1, 4,0..1) 2x2
    {"id": "nho_sam_y", "x": 3, "y": 2, "rarity": "rare"},               # (3,2, 4,2) 2x1
    {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 3, "rarity": "common"},# (3,3) 1x1
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}          # (4,3) 1x1
  ]
}

# 2. hybrid_thiet_phu_liet_diem.json: 1 axe (2x2) + 1 blade (1x3) + 1 palm (1x2)
thiet_phu_liet_diem = {
  "name": "Thiết Phủ Bang - Thiết Phủ Liệt Diễm",
  "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "khai_son_phu", "x": 0, "y": 0, "rarity": "epic"},           # (0,0..1, 1,0..1) 2x2
    {"id": "xich_sa_chuong", "x": 0, "y": 2, "rarity": "rare"},          # (0,2..3) 1x2
    {"id": "chu_sa_ho_lo", "x": 1, "y": 2, "rarity": "rare"},            # (1,2) 1x1
    {"id": "liet_hoa_tam_kinh", "x": 1, "y": 3, "rarity": "epic"},       # (1,3) 1x1
    {"id": "liet_diem_dao", "x": 2, "y": 0, "rarity": "epic"},           # (2,0..2) 1x3
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},       # (2,3) 1x1
    {"id": "o_thiet_giap", "x": 3, "y": 0, "rarity": "epic"},            # (3,0..1, 4,0..1) 2x2
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},                 # (3,2..3) 1x2
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},          # (4,2) 1x1
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}          # (4,3) 1x1
  ]
}

# 3. new_thiet_phu_hammer.json: 1 hammer (2x2) + 1 axe (2x2) + 1 fist (1x2)
chuy_phong = {
  "name": "Thiết Phủ Bang - Chấn Nhạc Chùy Phong",
  "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "chan_nhac_chuy", "x": 0, "y": 0, "rarity": "epic"},          # (0,0..1, 1,0..1) 2x2
    {"id": "khai_son_phu", "x": 0, "y": 2, "rarity": "epic"},            # (0,2..3, 1,2..3) 2x2
    {"id": "thiet_cot_quyen_sao", "x": 2, "y": 0, "rarity": "rare"},      # (2,0..1) 1x2
    {"id": "bach_luyen_chan_kinh", "x": 2, "y": 2, "rarity": "epic"},    # (2,2..3) 1x2
    {"id": "o_thiet_giap", "x": 3, "y": 0, "rarity": "epic"},            # (3,0..1, 4,0..1) 2x2
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},                 # (3,2..3) 1x2
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},          # (4,2) 1x1
    {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}       # (4,3) 1x1
  ]
}

# 4. kim_cang_bach_luyen.json: 2 fists (1x2 each) + 1 staff (1x4)
bach_luyen = {
  "name": "Kim Cang Tự - Bách Luyện Thuần Quyền",
  "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "phuc_ho_con", "x": 0, "y": 0, "rarity": "epic"},             # (0,0..3) 1x4
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "rarity": "rare"},     # (1,0..1) 1x2
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 2, "rarity": "rare"},     # (1,2..3) 1x2
    {"id": "bach_luyen_chan_kinh", "x": 2, "y": 0, "rarity": "epic"},    # (2,0..1) 1x2
    {"id": "tu_kim_bat", "x": 2, "y": 2, "rarity": "epic"},              # (2,2, 3,2) 2x1
    {"id": "cuu_chuyen_hoan_hon_dan", "x": 2, "y": 3, "rarity": "uncommon"}, # (2,3) 1x1
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "epic"},                 # (3,0..1, 4,0..1) 2x2
    {"id": "thiet_giap_y", "x": 3, "y": 3, "rarity": "rare"},            # (3,3, 4,3) 2x1
    {"id": "boi_nguyen_dan", "x": 4, "y": 2, "rarity": "uncommon"}       # (4,2) 1x1
  ]
}

# 5. hybrid_han_son_bich_thuy.json: flute (1x2) + brush (1x2) + whip (1x3)
han_bich = {
  "name": "Hàn Sơn Phái - Hàn Bích Hợp Bích",
  "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "han_ngoc_tieu", "x": 0, "y": 0, "rarity": "epic"},           # (0,0..1) 1x2
    {"id": "phan_quan_but", "x": 0, "y": 2, "rarity": "epic"},           # (0,2..3) 1x2
    {"id": "thanh_dang_tien", "x": 1, "y": 0, "rarity": "epic"},         # (1,0..2) 1x3
    {"id": "duong_sinh_quyet", "x": 2, "y": 0, "rarity": "epic"},        # (2,0..1) 1x2
    {"id": "tram_huong_lo", "x": 1, "y": 3, "rarity": "epic"},           # (1,3) 1x1
    {"id": "thanh_tam_dan", "x": 2, "y": 2, "rarity": "uncommon"},       # (2,2) 1x1
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},       # (2,3) 1x1
    {"id": "bang_tam_giap", "x": 3, "y": 0, "rarity": "rare"},           # (3,0, 4,0) 2x1
    {"id": "nho_sam_y", "x": 3, "y": 1, "rarity": "rare"},               # (3,1, 4,1) 2x1
    {"id": "o_tam_y", "x": 3, "y": 2, "rarity": "rare"}                  # (3,2..3, 4,2..3) 2x2
  ]
}

# 6. bich_thuy_duong_sinh.json: 1 zither (2x2) + 2 whips (1x3 each)
duong_sinh = {
  "name": "Bích Thủy Am - Dưỡng Sinh Cầm Âm",
  "cols": 5, "rows": 4, "sect": "bich_thuy", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "bich_thuy_cam", "x": 0, "y": 0, "rarity": "epic"},           # (0,0..1, 1,0..1) 2x2
    {"id": "thanh_dang_tien", "x": 2, "y": 0, "rarity": "rare"},         # (2,0..2) 1x3
    {"id": "thanh_dang_tien", "x": 0, "y": 2, "rarity": "rare"},         # (0,2..3)? wait, whip is 1x3!
  ]
}
