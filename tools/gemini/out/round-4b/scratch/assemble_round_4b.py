import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

# Read original chapters.json
orig_chapters = json.load(open("One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json", encoding="utf-8"))

# Load all 30 stage enemy configurations
enemies = {
    # Hồi 1
    1: {
        "name": "Sơn Tặc Lưu Manh", "sect": None, "tamPhapLevel": 1, "level": 1,
        "items": [
            {"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"}
        ]
    },
    2: {
        "name": "Thảo Khấu Tiên Phong", "sect": None, "tamPhapLevel": 1, "level": 2,
        "items": [
            {"id": "phuc_ho_con", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"}
        ]
    },
    3: {
        "name": "Sơn Trại Đầu Mục", "sect": None, "tamPhapLevel": 1, "level": 3,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "phuc_ho_con", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"}
        ]
    },
    4: {
        "name": "Lưu Manh Đao Khách", "sect": "liet_diem", "tamPhapLevel": 1, "level": 4,
        "items": [
            {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 1, "rarity": "rare"}
        ]
    },
    5: {
        "name": "Hắc Phong Trại Chủ", "sect": "thai_thanh", "tamPhapLevel": 2, "level": 5,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "legendary"},
            {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 1, "rarity": "epic"},
            {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 1, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "legendary"}
        ]
    },
    6: {
        "name": "Ám Tiễn Thích Khách", "sect": "thai_thanh", "tamPhapLevel": 2, "level": 6,
        "items": [
            {"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "da_ho_y", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "phi_yen_quyet", "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "thiet_giap_y", "x": 0, "y": 2, "tier": 2, "rarity": "rare"}
        ]
    },
    7: {
        "name": "Giang Hồ Khoái Đao", "sect": "liet_diem", "tamPhapLevel": 1, "level": 7,
        "items": [
            {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "phan_quan_but", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}
        ]
    },
    8: {
        "name": "Xích Diễm Cuồng Đao", "sect": "liet_diem", "tamPhapLevel": 2, "level": 8,
        "items": [
            {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "o_thiet_giap", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
            {"id": "liet_hoa_tam_kinh", "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "cuong_hoa_dan", "x": 3, "y": 1, "tier": 3, "rarity": "epic"}
        ]
    },
    9: {
        "name": "Thanh Vân Kiếm Sĩ", "sect": "thai_thanh", "tamPhapLevel": 2, "level": 9,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 1, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 1, "y": 1, "tier": 1, "rarity": "epic"},
            {"id": "o_tam_y", "x": 2, "y": 0, "tier": 2, "rarity": "epic"}
        ]
    },
    10: {
        "name": "Liệt Diễm Đao Vương", "sect": "liet_diem", "tamPhapLevel": 2, "level": 11,
        "items": [
            {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "liet_hoa_tam_kinh", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "cuong_hoa_dan", "x": 1, "y": 1, "tier": 3, "rarity": "rare"},
            {"id": "chu_sa_ho_lo", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "bach_ngoc_boi", "x": 2, "y": 1, "tier": 3, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 3, "y": 0, "tier": 3, "rarity": "uncommon"}
        ]
    },

    # Hồi 2
    11: {
        "name": "Bích Thủy Cung Nữ", "sect": "bich_thuy", "tamPhapLevel": 1, "level": 11,
        "items": [
            {"id": "bich_thuy_cam", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "thanh_dang_tien", "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "da_ho_y", "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "duong_sinh_quyet", "x": 4, "y": 0, "tier": 2, "rarity": "rare"}
        ]
    },
    12: {
        "name": "Huyền Độc Đao Khách", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 12,
        "items": [
            {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "o_tam_y", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 2, "rarity": "epic"}
        ]
    },
    13: {
        "name": "Man Uyên Độc Thủ", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 13,
        "items": [
            {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "thanh_dang_tien", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
            {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "man_xa_doc_tui", "x": 4, "y": 0, "tier": 2, "rarity": "epic"},
            {"id": "u_minh_bi_luc", "x": 4, "y": 1, "tier": 2, "rarity": "epic"}
        ]
    },
    14: {
        "name": "Độc Hạt Đạo Nhân", "sect": "bich_thuy", "tamPhapLevel": 2, "level": 14,
        "items": [
            {"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "man_xa_doc_tui", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "duong_sinh_quyet", "x": 2, "y": 1, "tier": 2, "rarity": "epic"},
            {"id": "nho_sam_y", "x": 3, "y": 0, "tier": 2, "rarity": "epic"}
        ]
    },
    15: {
        "name": "Bích Sa Ma Nữ", "sect": "bich_thuy", "tamPhapLevel": 2, "level": 15,
        "items": [
            {"id": "bich_thuy_cam", "x": 0, "y": 0, "tier": 2, "rarity": "supreme"},
            {"id": "nho_sam_y", "x": 2, "y": 0, "tier": 2, "rarity": "supreme"},
            {"id": "hoi_thien_dan", "x": 4, "y": 0, "tier": 2, "rarity": "supreme"},
            {"id": "thanh_tam_dan", "x": 4, "y": 1, "tier": 2, "rarity": "legendary"},
            {"id": "duong_sinh_quyet", "x": 4, "y": 2, "tier": 2, "rarity": "legendary"}
        ]
    },
    16: {
        "name": "Man Điển Độc Hùng", "sect": None, "tamPhapLevel": 2, "level": 16,
        "items": [
            {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "legendary"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "legendary"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 1, "tier": 3, "rarity": "legendary"}
        ]
    },
    17: {
        "name": "U Minh Ám Sát Thí Tử", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 17,
        "items": [
            {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "van_doc_dinh", "x": 1, "y": 1, "tier": 3, "rarity": "epic"},
            {"id": "kich_doc_chu_nga", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "da_ho_y", "x": 3, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "epic"}
        ]
    },
    18: {
        "name": "Huyền Độc Trưởng Lão", "sect": "huyen_doc", "tamPhapLevel": 1, "level": 18,
        "items": [
            {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "tu_kim_trao", "x": 0, "y": 2, "tier": 3, "rarity": "rare"},
            {"id": "o_tam_y", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "u_minh_bi_luc", "x": 3, "y": 1, "tier": 2, "rarity": "rare"}
        ]
    },
    19: {
        "name": "Xích Luyện Xà Tinh", "sect": None, "tamPhapLevel": 2, "level": 19,
        "items": [
            {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "hoa_diem_xa_cot", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "thanh_dang_tien", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "man_xa_doc_tui", "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 3, "y": 1, "tier": 2, "rarity": "rare"},
            {"id": "da_ho_y", "x": 4, "y": 0, "tier": 2, "rarity": "rare"}
        ]
    },
    20: {
        "name": "Hắc Độc Nương Tử", "sect": "huyen_doc", "tamPhapLevel": 3, "level": 21,
        "items": [
            {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "u_minh_bi_luc", "x": 1, "y": 1, "tier": 4, "rarity": "epic"},
            {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 4, "rarity": "epic"}
        ]
    },

    # Hồi 3
    21: {
        "name": "Băng Phách Thủ Vệ", "sect": "han_son", "tamPhapLevel": 1, "level": 21,
        "items": [
            {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
            {"id": "bang_tam_giap", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "da_ho_y", "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "han_bang_chau", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "han_ngoc_tieu", "x": 4, "y": 1, "tier": 3, "rarity": "rare"}
        ]
    },
    22: {
        "name": "Hàn Đàm Cổ Kiếm", "sect": "han_son", "tamPhapLevel": 1, "level": 22,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "bang_tam_giap", "x": 1, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "han_ngoc_tieu", "x": 3, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "han_bang_chau", "x": 4, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "phi_yen_quyet", "x": 4, "y": 1, "tier": 5, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 1, "y": 1, "tier": 5, "rarity": "epic"}
        ]
    },
    23: {
        "name": "Tuyết Lang Vương", "sect": None, "tamPhapLevel": 2, "level": 25,
        "items": [
            {"id": "soi_nanh_canh", "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "tuyet_lang_bach_bi", "x": 0, "y": 2, "tier": 5, "rarity": "legendary"},
            {"id": "da_ho_y", "x": 4, "y": 0, "tier": 5, "rarity": "legendary"}
        ]
    },
    24: {
        "name": "Hộ Pháp Kim Cang", "sect": "kim_cang", "tamPhapLevel": 2, "level": 24,
        "items": [
            {"id": "phuc_ho_con", "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "vo_niem_kinh", "x": 4, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "thien_tam_chuoi", "x": 4, "y": 2, "tier": 5, "rarity": "epic"}
        ]
    },
    25: {
        "name": "Băng Lam Kiếm Khách", "sect": "han_son", "tamPhapLevel": 2, "level": 25,
        "items": [
            {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 5, "rarity": "supreme"},
            {"id": "bang_tam_giap", "x": 1, "y": 0, "tier": 5, "rarity": "supreme"},
            {"id": "bang_tam_linh_chau", "x": 3, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "han_bang_chau", "x": 4, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "kiem_tong_tam_yeu", "x": 3, "y": 1, "tier": 5, "rarity": "legendary"},
            {"id": "bach_ngoc_boi", "x": 4, "y": 1, "tier": 5, "rarity": "legendary"}
        ]
    },
    26: {
        "name": "Bạch Hổ Tông Sư", "sect": None, "tamPhapLevel": 1, "level": 26,
        "items": [
            {"id": "bach_ho_cuong_mong", "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "thanh_dong_ho_tam_kinh", "x": 0, "y": 1, "tier": 3, "rarity": "legendary"},
            {"id": "bach_ngoc_boi", "x": 1, "y": 1, "tier": 5, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "thiet_cot_quyen_sao", "x": 4, "y": 0, "tier": 5, "rarity": "legendary"}
        ]
    },
    27: {
        "name": "U Minh Quỷ Trảo", "sect": "huyen_doc", "tamPhapLevel": 1, "level": 27,
        "items": [
            {"id": "u_minh_quy_trao", "x": 0, "y": 0, "tier": 5, "rarity": "supreme"},
            {"id": "bach_ngoc_boi", "x": 0, "y": 2, "tier": 5, "rarity": "legendary"},
            {"id": "da_ho_y", "x": 2, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "man_xa_doc_tui", "x": 3, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "u_minh_bi_luc", "x": 3, "y": 1, "tier": 5, "rarity": "legendary"},
            {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 5, "rarity": "legendary"}
        ]
    },
    28: {
        "name": "Hàn Băng Thần Quy", "sect": None, "tamPhapLevel": 1, "level": 28,
        "items": [
            {"id": "bang_tam_giap", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "bang_phach_thuong", "x": 4, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "han_ngoc_tieu", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "han_bang_chau", "x": 3, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "kiem_tong_tam_yeu", "x": 3, "y": 1, "tier": 4, "rarity": "epic"}
        ]
    },
    29: {
        "name": "Tuyết Kiếm Tông Sư", "sect": "thai_thanh", "tamPhapLevel": 1, "level": 29,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 6, "rarity": "legendary"},
            {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "tuy_tien_ho_lo", "x": 1, "y": 1, "tier": 5, "rarity": "legendary"},
            {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 5, "rarity": "supreme"},
            {"id": "kiem_tong_tam_yeu", "x": 4, "y": 0, "tier": 5, "rarity": "legendary"}
        ]
    },
    30: {
        "name": "Dạ Sát Thiên", "sect": "han_son", "tamPhapLevel": 1, "level": 31,
        "items": [
            {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 6, "rarity": "epic"},
            {"id": "han_ngoc_tieu", "x": 1, "y": 0, "tier": 6, "rarity": "legendary"},
            {"id": "bang_tam_giap", "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "bang_tam_linh_chau", "x": 4, "y": 0, "tier": 6, "rarity": "epic"},
            {"id": "han_bang_chau", "x": 4, "y": 1, "tier": 6, "rarity": "epic"},
            {"id": "kiem_tong_tam_yeu", "x": 4, "y": 2, "tier": 6, "rarity": "epic"}
        ]
    }
}

# Update only enemy fields in chapters.json
out_chapters = json.loads(json.dumps(orig_chapters))
for c_idx, chap in enumerate(out_chapters["chapters"]):
    for s_idx, stage in enumerate(chap["stages"]):
        st_num = c_idx * 10 + s_idx + 1
        cfg = enemies[st_num]
        e = stage["enemy"]
        # Update only enemy fields
        e["level"] = cfg["level"]
        e["sect"] = cfg["sect"]
        e["tamPhapLevel"] = cfg["tamPhapLevel"]
        e["items"] = cfg["items"]

# Write to tools/gemini/out/round-4b/chapters.json
with open("tools/gemini/out/round-4b/chapters.json", "w", encoding="utf-8") as f:
    json.dump(out_chapters, f, indent=2, ensure_ascii=False)

print("Saved tools/gemini/out/round-4b/chapters.json successfully!")
