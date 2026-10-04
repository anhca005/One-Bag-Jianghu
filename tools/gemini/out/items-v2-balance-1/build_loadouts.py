import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
CATALOG_PATH = ROOT / "tools/gemini/out/items-v2/items-v2.normalized.json"
OUT_DIR = ROOT / "tools/gemini/out/items-v2-balance-1/loadouts"
OUT_DIR.mkdir(parents=True, exist_ok=True)

catalog = json.load(open(CATALOG_PATH, encoding="utf-8"))
items_by_id = {it["id"]: it for it in catalog}

def validate_bag(name, items, cols=5, rows=4):
    grid = [[None for _ in range(cols)] for _ in range(rows)]
    for idx, placed in enumerate(items):
        item_id = placed["id"]
        if item_id not in items_by_id:
            raise ValueError(f"{name}: unknown item {item_id}")
        it = items_by_id[item_id]
        w, h = it["size"]
        x, y = placed["x"], placed["y"]
        if x < 0 or y < 0 or x + w > cols or y + h > rows:
            raise ValueError(f"{name}: item {item_id} at ({x},{y}) size ({w},{h}) out of bounds ({cols}x{rows})")
        for dy in range(h):
            for dx in range(w):
                if grid[y + dy][x + dx] is not None:
                    raise ValueError(f"{name}: item {item_id} overlaps with {grid[y+dy][x+dx]} at ({x+dx}, {y+dy})")
                grid[y + dy][x + dx] = item_id
    return True

loadouts = {
    # 1. Kim Cang 1 (Côn + Quyền)
    "kim_cang.json": {
        "name": "Kim Cang Tự - Hộ Thể Côn Quyền",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1,
        "items": [
            {"id": "phuc_ho_con", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 2, "rarity": "rare"},
            {"id": "bach_luyen_chan_kinh", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "tu_kim_bat", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "cuu_chuyen_hoan_hon_dan", "x": 2, "y": 3, "rarity": "uncommon"},
            {"id": "thiet_giap_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 1, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "van_khi_dan", "x": 4, "y": 1, "rarity": "uncommon"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
            {"id": "hoi_thien_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 2. Kim Cang 2 (Bách Luyện Thuần Quyền)
    "kim_cang_bach_luyen.json": {
        "name": "Kim Cang Tự - Bách Luyện Thuần Quyền",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1,
        "items": [
            {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 0, "y": 2, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 2, "rarity": "rare"},
            {"id": "bach_luyen_chan_kinh", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "cuu_chuyen_hoan_hon_dan", "x": 2, "y": 2, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
            {"id": "tu_kim_bat", "x": 3, "y": 0, "rarity": "epic"},
            {"id": "o_tam_y", "x": 3, "y": 1, "rarity": "epic"},
            {"id": "thiet_giap_y", "x": 3, "y": 3, "rarity": "rare"}
        ]
    },

    # 3. Thái Thanh 1 (Kiếm Đạo Hóa Kình)
    "thai_thanh.json": {
        "name": "Thái Thanh Quán - Kiếm Đạo Hóa Kình",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "kiem_tong_tam_yeu", "x": 0, "y": 3, "rarity": "rare"},
            {"id": "thai_thanh_tam_kinh", "x": 1, "y": 3, "rarity": "epic"},
            {"id": "thien_tam_chuoi", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "bach_ngoc_boi", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
            {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "common"},
            {"id": "van_khi_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "hoi_thien_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 4. Thái Thanh 2 (Kiếm Khí Xuyên Giáp)
    "thai_thanh_kiem_khi.json": {
        "name": "Thái Thanh Quán - Kiếm Khí Xuyên Giáp",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "rarity": "rare"},
            {"id": "thanh_phong_kiem", "x": 2, "y": 0, "rarity": "rare"},
            {"id": "kiem_tong_tam_yeu", "x": 0, "y": 3, "rarity": "epic"},
            {"id": "thai_thanh_tam_kinh", "x": 1, "y": 3, "rarity": "epic"},
            {"id": "bach_ngoc_boi", "x": 2, "y": 3, "rarity": "epic"},
            {"id": "kim_tuyen_y", "x": 3, "y": 0, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 4, "y": 2, "rarity": "uncommon"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 3, "rarity": "common"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 5. Thái Thanh 3 (Tiêu Dao Phiến Pháp)
    "new_thai_thanh_fan.json": {
        "name": "Thái Thanh Quán - Tiêu Dao Phiến Pháp",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1,
        "items": [
            {"id": "ngoc_cot_phien", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "ngoc_cot_phien", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "rarity": "rare"},
            {"id": "kiem_tong_tam_yeu", "x": 1, "y": 3, "rarity": "epic"},
            {"id": "thai_thanh_tam_kinh", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "bach_ngoc_boi", "x": 2, "y": 1, "rarity": "epic"},
            {"id": "thien_tam_chuoi", "x": 2, "y": 2, "rarity": "rare"},
            {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 6. Liệt Diễm 1 (Cuồng Đao Liệt Hỏa)
    "liet_diem.json": {
        "name": "Liệt Diễm Đường - Cuồng Đao Liệt Hỏa",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1,
        "items": [
            {"id": "liet_diem_dao", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "liet_diem_dao", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "liet_hoa_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
            {"id": "chu_sa_ho_lo", "x": 1, "y": 3, "rarity": "epic"},
            {"id": "da_ho_y", "x": 2, "y": 0, "rarity": "rare"},
            {"id": "cuong_hoa_dan", "x": 2, "y": 2, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
            {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "common"},
            {"id": "hoi_thien_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 7. Liệt Diễm 2 (Hỏa Hải Song Đao)
    "liet_diem_hoa_hai.json": {
        "name": "Liệt Diễm Đường - Hỏa Hải Song Đao",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1,
        "items": [
            {"id": "tu_mau_song_dao", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "liet_hoa_tam_kinh", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "cuong_hoa_dan", "x": 0, "y": 3, "rarity": "uncommon"},
            {"id": "chu_sa_ho_lo", "x": 1, "y": 2, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 3, "rarity": "uncommon"},
            {"id": "liet_diem_dao", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 2, "y": 3, "rarity": "common"},
            {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 8. Liệt Diễm 3 (Xích Sa Chưởng Kình)
    "new_liet_diem_palm.json": {
        "name": "Liệt Diễm Đường - Xích Sa Chưởng Kình",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1,
        "items": [
            {"id": "xich_sa_chuong", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "xich_sa_chuong", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "liet_diem_dao", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "liet_hoa_tam_kinh", "x": 1, "y": 3, "rarity": "epic"},
            {"id": "chu_sa_ho_lo", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "cuong_hoa_dan", "x": 2, "y": 1, "rarity": "uncommon"},
            {"id": "da_ho_y", "x": 2, "y": 2, "rarity": "rare"},
            {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 9. Huyền Độc 1 (Hắc Huyết Ám Khí)
    "huyen_doc.json": {
        "name": "Huyền Độc Môn - Hắc Huyết Ám Khí",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "huyen_doc", "tamPhapLevel": 1,
        "items": [
            {"id": "that_tinh_tieu", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "that_tinh_tieu", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "tu_kim_trao", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "tu_kim_trao", "x": 1, "y": 2, "rarity": "epic"},
            {"id": "u_minh_bi_luc", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "van_doc_dinh", "x": 2, "y": 1, "rarity": "epic"},
            {"id": "huyet_ngoc_thach", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "giai_doc_dan", "x": 2, "y": 3, "rarity": "uncommon"},
            {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 10. Huyền Độc 2 (Vạn Độc Phệ Huyết)
    "huyen_doc_van_doc.json": {
        "name": "Huyền Độc Môn - Vạn Độc Phệ Huyết",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "huyen_doc", "tamPhapLevel": 1,
        "items": [
            {"id": "that_tinh_tieu", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "that_tinh_tieu", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "tu_kim_trao", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "tu_kim_trao", "x": 1, "y": 2, "rarity": "epic"},
            {"id": "that_tinh_tieu", "x": 2, "y": 0, "rarity": "rare"},
            {"id": "u_minh_bi_luc", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "van_doc_dinh", "x": 2, "y": 3, "rarity": "epic"},
            {"id": "huyet_ngoc_thach", "x": 3, "y": 0, "rarity": "epic"},
            {"id": "giai_doc_dan", "x": 3, "y": 1, "rarity": "uncommon"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "common"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 11. Hàn Sơn 1 (Hàn Băng Thương Pháp)
    "new_han_son_spear.json": {
        "name": "Hàn Sơn Phái - Hàn Băng Thương Pháp",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1,
        "items": [
            {"id": "bang_phach_thuong", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "han_ngoc_tieu", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "phan_quan_but", "x": 1, "y": 2, "rarity": "epic"},
            {"id": "bang_tam_giap", "x": 2, "y": 0, "rarity": "rare"},
            {"id": "han_bang_chau", "x": 2, "y": 1, "rarity": "epic"},
            {"id": "thanh_tam_dan", "x": 3, "y": 1, "rarity": "uncommon"},
            {"id": "nho_sam_y", "x": 2, "y": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
            {"id": "van_khi_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "truy_hon_cung", "x": 4, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 3, "rarity": "rare"}
        ]
    },

    # 12. Hàn Sơn 2 (Băng Tâm Bút Cung)
    "han_son_bang_tam.json": {
        "name": "Hàn Sơn Phái - Băng Tâm Bút Cung",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1,
        "items": [
            {"id": "truy_hon_cung", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "thanh_tam_dan", "x": 0, "y": 3, "rarity": "uncommon"},
            {"id": "phan_quan_but", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "phan_quan_but", "x": 1, "y": 2, "rarity": "epic"},
            {"id": "han_ngoc_tieu", "x": 2, "y": 0, "rarity": "rare"},
            {"id": "han_bang_chau", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
            {"id": "bang_tam_giap", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "nho_sam_y", "x": 3, "y": 1, "rarity": "rare"},
            {"id": "o_tam_y", "x": 3, "y": 2, "rarity": "epic"}
        ]
    },

    # 13. Thiết Phủ 1 (Bá Khí Khai Sơn)
    "thiet_phu.json": {
        "name": "Thiết Phủ Bang - Bá Khí Khai Sơn",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1,
        "items": [
            {"id": "khai_son_phu", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "khai_son_phu", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "o_thiet_giap", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "phuc_ma_giap", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 14. Thiết Phủ 2 (Chấn Nhạc Chùy Phong)
    "new_thiet_phu_hammer.json": {
        "name": "Thiết Phủ Bang - Chấn Nhạc Chùy Phong",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1,
        "items": [
            {"id": "chan_nhac_chuy", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "khai_son_phu", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "o_thiet_giap", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "phuc_ma_giap", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 15. Bích Thủy 1 (Thanh Đằng Trường Tiên)
    "new_bich_thuy_whip.json": {
        "name": "Bích Thủy Am - Thanh Đằng Trường Tiên",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "bich_thuy", "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_dang_tien", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "thanh_dang_tien", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "kim_sang_duoc", "x": 0, "y": 3, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 3, "rarity": "uncommon"},
            {"id": "duong_sinh_quyet", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "tram_huong_lo", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 2, "y": 3, "rarity": "rare"},
            {"id": "bich_thuy_cam", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "tue_nguyet_bao_y", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "common"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 16. Bích Thủy 2 (Dưỡng Sinh Cầm Âm)
    "bich_thuy_duong_sinh.json": {
        "name": "Bích Thủy Am - Dưỡng Sinh Cầm Âm",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "bich_thuy", "tamPhapLevel": 1,
        "items": [
            {"id": "bich_thuy_cam", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "bich_thuy_cam", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "duong_sinh_quyet", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "tue_nguyet_bao_y", "x": 2, "y": 2, "rarity": "rare"},
            {"id": "tram_huong_lo", "x": 3, "y": 0, "rarity": "epic"},
            {"id": "kim_sang_duoc", "x": 3, "y": 1, "rarity": "uncommon"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "thanh_dang_tien", "x": 4, "y": 0, "rarity": "rare"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 17. Hybrid 1: Hàn Sơn - Bích Thủy
    "hybrid_han_son_bich_thuy.json": {
        "name": "Hàn Sơn Phái - Hàn Bích Hợp Bích",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1,
        "items": [
            {"id": "han_ngoc_tieu", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "phan_quan_but", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "bich_thuy_cam", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "duong_sinh_quyet", "x": 1, "y": 2, "rarity": "epic"},
            {"id": "tram_huong_lo", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "thanh_tam_dan", "x": 2, "y": 3, "rarity": "uncommon"},
            {"id": "bang_tam_giap", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "nho_sam_y", "x": 3, "y": 1, "rarity": "rare"},
            {"id": "o_tam_y", "x": 3, "y": 2, "rarity": "epic"}
        ]
    },

    # 18. Hybrid 2: Kim Cang - Thiết Phủ
    "hybrid_kim_cang_thiet_phu.json": {
        "name": "Kim Cang Tự - Kim Cang Thiết Phủ",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1,
        "items": [
            {"id": "phuc_ho_con", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "khai_son_phu", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 2, "rarity": "rare"},
            {"id": "bach_luyen_chan_kinh", "x": 2, "y": 2, "rarity": "epic"},
            {"id": "thiet_giap_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "tu_kim_bat", "x": 3, "y": 1, "rarity": "epic"},
            {"id": "o_tam_y", "x": 3, "y": 2, "rarity": "epic"}
        ]
    },

    # 19. Hybrid 3: Thái Thanh - Liệt Diễm
    "hybrid_thai_thanh_liet_diem.json": {
        "name": "Thái Thanh Quán - Thái Thanh Liệt Diễm",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "liet_diem_dao", "x": 1, "y": 0, "rarity": "epic"},
            {"id": "thai_thanh_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
            {"id": "liet_hoa_tam_kinh", "x": 1, "y": 3, "rarity": "epic"},
            {"id": "kiem_tong_tam_yeu", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "bach_ngoc_boi", "x": 2, "y": 1, "rarity": "epic"},
            {"id": "da_ho_y", "x": 2, "y": 2, "rarity": "rare"},
            {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 3, "y": 2, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    },

    # 20. Hybrid 4: Thiết Phủ - Liệt Diễm
    "hybrid_thiet_phu_liet_diem.json": {
        "name": "Thiết Phủ Bang - Thiết Phủ Liệt Diễm",
        "hp": 1200, "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1,
        "items": [
            {"id": "khai_son_phu", "x": 0, "y": 0, "rarity": "epic"},
            {"id": "tu_mau_song_dao", "x": 0, "y": 2, "rarity": "epic"},
            {"id": "o_thiet_giap", "x": 2, "y": 0, "rarity": "epic"},
            {"id": "da_ho_y", "x": 2, "y": 2, "rarity": "rare"},
            {"id": "chu_sa_ho_lo", "x": 3, "y": 2, "rarity": "epic"},
            {"id": "cuong_hoa_dan", "x": 3, "y": 3, "rarity": "uncommon"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "rarity": "rare"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 1, "rarity": "common"},
            {"id": "boi_nguyen_dan", "x": 4, "y": 2, "rarity": "uncommon"},
            {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
        ]
    }
}

for filename, data in loadouts.items():
    validate_bag(filename, data["items"])
    filepath = OUT_DIR / filename
    with open(filepath, "w", encoding="utf-8", newline="\n") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
        f.write("\n")

print(f"Successfully generated and validated {len(loadouts)} loadouts in {OUT_DIR}")
