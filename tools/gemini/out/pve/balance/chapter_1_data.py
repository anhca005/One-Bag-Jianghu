# Chapter 1 Data: Thanh Khê Trấn (Stages 1-10)

CHAPTER_1 = {
    "id": "hoi_1",
    "index": 1,
    "title": "Hồi 1",
    "name": "Thanh Khê Trấn",
    "summary": "Thiếu niên hiệp khách mang theo tàn nang gia truyền đặt chân đến bến đò Thanh Khê mùa thu lá rụng, tao ngộ Lão Tiều Phu và đối đầu thủy phỉ Ba Hổ Trại.",
    "theme": {
        "mapBackground": "map_hoi_1",
        "battleBackground": "bg_battle_hoi_1",
        "accent": "#8B2E1F",
        "paper": "#EFE4CC",
        "ink": "#2B2420",
        "nodeIcon": "node_hoi_1",
        "bossNodeIcon": "node_boss_hoi_1"
    },
    "unlock": {"afterChapter": None},
    "stages": [
        {
            "id": "hoi_1_01",
            "index": 1,
            "name": "Trấn Khẩu Sơn Đạo",
            "kind": "normal",
            "mapPos": [0.12, 0.75],
            "description": "Lối vào trấn Thanh Khê sương sớm lãng đãng, bọn thảo khấu chực chờ cướp bóc.",
            "staminaCost": 6,
            "enemy": {
                "name": "Thảo Khấu Tiên Phong",
                "characterId": "thao_khau",
                "portrait": "enemy_portrait_1",
                "sect": None,
                "tamPhapLevel": 1,
                "hp": 750,
                "items": [
                    {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
                    {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 200,
                    "heroExp": 50,
                    "materials": {"han_thiet": 3},
                    "items": [{"id": "boi_nguyen_dan", "tier": 1, "rarity": "common"}]
                },
                "repeat": "default",
                "drops": [{"id": "thiet_cot_quyen_sao", "chancePermille": 150, "rarity": "common"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Phía trước là lối vào Thanh Khê Trấn. Sao lại có kẻ chặn đường?"},
                    {"speaker": "lao_tieu_phu", "text": "Tiểu hữu, cẩn thận! Bọn thảo khấu ven đường chuyên cướp của người lạ."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Lũ thảo khấu này đã tháo chạy. Đa tạ lão bá nhắc nhở."},
                    {"speaker": "lao_tieu_phu", "text": "Túi càn khôn của ngươi xếp đặt rất khéo, đúng là có căn cơ võ học."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "lao_tieu_phu", "text": "Bình tĩnh lại. Sắp xếp lại binh khí trong túi, giữ Hóa Kình phòng thủ."}
                ]
            }
        },
        {
            "id": "hoi_1_02",
            "index": 2,
            "name": "Phong Diệp Bến Đò",
            "kind": "normal",
            "mapPos": [0.20, 0.68],
            "description": "Bến đò rực sắc phong đỏ, thủy phỉ tuần tra gắt gao trên các thuyền nan.",
            "staminaCost": 6,
            "enemy": {
                "name": "Thủy Phỉ Thám Tử",
                "characterId": "thuy_phi",
                "portrait": "enemy_portrait_1",
                "sect": None,
                "tamPhapLevel": 1,
                "hp": 850,
                "items": [
                    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
                    {"id": "o_tam_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 250,
                    "heroExp": 60,
                    "materials": {"han_thiet": 4},
                    "items": [{"id": "kim_sang_duoc", "tier": 1, "rarity": "common"}]
                },
                "repeat": "default",
                "drops": [{"id": "thanh_phong_kiem", "chancePermille": 120, "rarity": "common"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Thuyền đò đều bị giữ lại. Các ngươi là người phương nào?"},
                    {"speaker": "lao_tieu_phu", "text": "Là tai mắt của Ba Hổ Trại. Chúng phong tỏa bến sông rồi."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Bến đò đã thông. Chúng ta có thể qua sông."},
                    {"speaker": "lao_tieu_phu", "text": "Kiếm chiêu dứt khoát. Nhưng phía trước mới là sào huyệt thực sự."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "lao_tieu_phu", "text": "Thủy phỉ đao pháp linh hoạt. Hãy mang theo đan dược hồi phục."}
                ]
            }
        },
        {
            "id": "hoi_1_03",
            "index": 3,
            "name": "Hòe Thụ Thôn Ngoại",
            "kind": "normal",
            "mapPos": [0.28, 0.60],
            "description": "Gốc hòe già đầu thôn hoang vắng, bầy sói đói từ rừng sâu mò ra rình mồi.",
            "staminaCost": 6,
            "enemy": {
                "name": "Hoang Dã Sói Xám",
                "characterId": "hoang_lang",
                "portrait": "monster_soi_xam",
                "sect": None,
                "tamPhapLevel": 1,
                "hp": 900,
                "items": [
                    {"id": "soi_nanh_canh", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
                    {"id": "da_ho_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 300,
                    "heroExp": 70,
                    "materials": {"da_thu": 5},
                    "items": [{"id": "soi_nanh_canh", "tier": 1, "rarity": "uncommon"}]
                },
                "repeat": "default",
                "drops": [{"id": "soi_nanh_canh", "chancePermille": 100, "rarity": "common"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Tiếng gầm gừ trong bụi rậm... Dã thú biến dị sao?"},
                    {"speaker": "lao_tieu_phu", "text": "Dị thú hấp thu linh khí hoang dã, nanh vuốt sắc bén dị thường."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Nanh sói này cứng chắc khác thường, có thể gắn vào binh khí."},
                    {"speaker": "lao_tieu_phu", "text": "Đó là bảo vật của quái thú. Đem vào túi càn khôn tăng bộc phát."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "lao_tieu_phu", "text": "Sói hoang tấn công dồn dập. Dùng áo giáp che chắn cẩn thận."}
                ]
            }
        },
        {
            "id": "hoi_1_04",
            "index": 4,
            "name": "Thanh Khê Trà Thất",
            "kind": "normal",
            "mapPos": [0.35, 0.52],
            "description": "Quán trà ven đường vương khói lửa, đệ tử Liệt Diệm Bang ngang ngược chiếm cứ.",
            "staminaCost": 6,
            "enemy": {
                "name": "Liệt Diệm Bang Chúng",
                "characterId": "liet_diem_bang_chung",
                "portrait": "enemy_portrait_2",
                "sect": "liet_diem",
                "tamPhapLevel": 1,
                "hp": 980,
                "items": [
                    {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
                    {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"},
                    {"id": "thuan_duong_cong", "x": 2, "y": 1, "tier": 1, "rarity": "common"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 350,
                    "heroExp": 80,
                    "materials": {"han_thiet": 5},
                    "items": [{"id": "thai_thanh_tam_kinh", "tier": 1, "rarity": "uncommon"}]
                },
                "repeat": "default",
                "drops": [{"id": "thuan_duong_cong", "chancePermille": 80, "rarity": "common"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Quán trà thanh tịnh lại nồng nặc mùi hỏa dược."},
                    {"speaker": "lao_tieu_phu", "text": "Liệt Diệm Bang tính tình nóng nảy, đao pháp mang nhiệt khí thiêu đốt."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Hóa Kình vững chãi đã triệt tiêu được sức nóng từ hỏa đao."},
                    {"speaker": "lao_tieu_phu", "text": "Rất tốt. Tâm pháp bổ trợ sẽ giúp nội lực vận hành nhuần nhuyễn."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "lao_tieu_phu", "text": "Đừng để bị thiêu đốt liên tục. Giữ khoảng cách và phản kích."}
                ]
            }
        },
        {
            "id": "hoi_1_05",
            "index": 5,
            "name": "Phong Ba Đình",
            "kind": "elite",
            "mapPos": [0.42, 0.45],
            "description": "Ngôi đình cổ giữa hồ sen tàn, đao khách Phong Lôi Đạo đứng đợi trong mưa gió.",
            "staminaCost": 8,
            "enemy": {
                "name": "Phong Lôi Khoái Đao Độc Cô Hạnh",
                "characterId": "doc_co_hanh",
                "portrait": "npc_doc_co_hanh",
                "sect": "thai_thanh",
                "tamPhapLevel": 1,
                "hp": 1100,
                "items": [
                    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "rare"},
                    {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
                    {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
                    {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 1, "rarity": "uncommon"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 500,
                    "heroExp": 120,
                    "materials": {"han_thiet": 8, "huyen_tinh": 2},
                    "items": [{"id": "tuy_tien_ho_lo", "tier": 1, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "phi_yen_quyet", "chancePermille": 100, "rarity": "uncommon"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "doc_co_hanh", "text": "Kẻ mang tàn nang kia, rút đao ra! Đao pháp của ta không đợi kẻ yếu."},
                    {"speaker": "hero", "text": "Các hạ là Độc Cô Hạnh? Ta không có ý kết oán, chỉ muốn mượn đường."}
                ],
                "afterWin": [
                    {"speaker": "doc_co_hanh", "text": "Hảo chiêu số! Ngươi quả nhiên có tư cách mang càn khôn nang."},
                    {"speaker": "hero", "text": "Đa tạ Độc Cô huynh hạ thủ lưu tình."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Đao quá chậm! Cương Khí hộ thân còn non nớt lắm, luyện lại đi!"}
                ]
            }
        },
        {
            "id": "hoi_1_06",
            "index": 6,
            "name": "Ngô Đồng Cổ Hạng",
            "kind": "normal",
            "mapPos": [0.50, 0.48],
            "description": "Ngõ hẹp lá ngô đồng rơi đầy, bóng người thoắt ẩn thoắt hiện trong bóng tối.",
            "staminaCost": 6,
            "enemy": {
                "name": "Ám Hạng Thích Khách",
                "characterId": "am_sat",
                "portrait": "enemy_portrait_1",
                "sect": "thai_thanh",
                "tamPhapLevel": 1,
                "hp": 1050,
                "items": [
                    {"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 2, "rarity": "common"},
                    {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 2, "rarity": "common"},
                    {"id": "da_ho_y", "x": 2, "y": 0, "tier": 2, "rarity": "common"},
                    {"id": "phi_yen_quyet", "x": 3, "y": 0, "tier": 2, "rarity": "common"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 400,
                    "heroExp": 90,
                    "materials": {"han_thiet": 6},
                    "items": [{"id": "that_tinh_tieu", "tier": 2, "rarity": "uncommon"}]
                },
                "repeat": "default",
                "drops": [{"id": "that_tinh_tieu", "chancePermille": 100, "rarity": "common"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Gió lạnh thổi qua ngõ hẹp... Có sát khí!"},
                    {"speaker": "lao_tieu_phu", "text": "Là ám khí môn đồ phục kích. Coi chừng phi tiêu độc."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Ám khí nhanh nhưng uy lực không đủ phá Cương Khí."},
                    {"speaker": "lao_tieu_phu", "text": "Biết dùng hộ giáp hóa giải ám khí, kinh nghiệm của ngươi tăng rồi."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "lao_tieu_phu", "text": "Ám khí liên hoàn rất rát. Cần giáp kiên cố để che chắn ngực."}
                ]
            }
        },
        {
            "id": "hoi_1_07",
            "index": 7,
            "name": "Thủy Tạ Nguy Kiều",
            "kind": "normal",
            "mapPos": [0.58, 0.40],
            "description": "Cây cầu gỗ chênh vênh bên đầm nước, con quái trư da sắt án ngữ đầu cầu.",
            "staminaCost": 6,
            "enemy": {
                "name": "Thiết Bì Trư Thú",
                "characterId": "tru_thu",
                "portrait": "monster_heo_rung",
                "sect": None,
                "tamPhapLevel": 1,
                "hp": 1150,
                "items": [
                    {"id": "heo_rung_bap_giap", "x": 0, "y": 0, "tier": 2, "rarity": "uncommon"},
                    {"id": "thiet_cot_quyen_sao", "x": 2, "y": 0, "tier": 2, "rarity": "uncommon"},
                    {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 0, "tier": 2, "rarity": "uncommon"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 450,
                    "heroExp": 100,
                    "materials": {"da_thu": 8},
                    "items": [{"id": "heo_rung_bap_giap", "tier": 2, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "heo_rung_bap_giap", "chancePermille": 120, "rarity": "uncommon"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Con thú khổng lồ kia da dày như thiết giáp, húc đổ cả lan can cầu!"},
                    {"speaker": "lao_tieu_phu", "text": "Thiết Bì Trư Thú! Đòn húc của nó rất nặng, phải kiên nhẫn phá giáp."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Lớp da bắp giáp của nó thật phi thường, phòng ngự cực kỳ kiên cố."},
                    {"speaker": "lao_tieu_phu", "text": "Lấy tấm giáp bì đó rèn thành áo giáp, sau này đối đầu kình địch sẽ cần."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "lao_tieu_phu", "text": "Sức trâu húc của nó quá hung hãn. Đừng đối đầu trực diện khi nó tích nộ."}
                ]
            }
        },
        {
            "id": "hoi_1_08",
            "index": 8,
            "name": "Phong Hỏa Cổ Đài",
            "kind": "normal",
            "mapPos": [0.66, 0.32],
            "description": "Đài phong hỏa cháy rực trong hoàng hôn, hương chủ Liệt Diệm Bang trấn giữ lối lên trại.",
            "staminaCost": 6,
            "enemy": {
                "name": "Liệt Diệm Bang Hương Chủ",
                "characterId": "liet_diem_huong_chu",
                "portrait": "enemy_portrait_2",
                "sect": "liet_diem",
                "tamPhapLevel": 1,
                "hp": 1180,
                "items": [
                    {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "uncommon"},
                    {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 2, "rarity": "uncommon"},
                    {"id": "liet_hoa_tam_kinh", "x": 1, "y": 0, "tier": 2, "rarity": "uncommon"},
                    {"id": "cuong_hoa_dan", "x": 1, "y": 1, "tier": 2, "rarity": "uncommon"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 500,
                    "heroExp": 110,
                    "materials": {"han_thiet": 8, "xich_dong": 4},
                    "items": [{"id": "liet_hoa_tam_kinh", "tier": 2, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "cuong_hoa_dan", "chancePermille": 100, "rarity": "uncommon"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Khói lửa mịt mù. Lại thêm cao thủ Liệt Diệm Bang cản đường."},
                    {"speaker": "lao_tieu_phu", "text": "Hắn là Hương Chủ giữ cửa trại. Đao hỏa của hắn phối hợp đan dược cuồng hỏa."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Hỏa diễm đã tàn. Đài phong hỏa đã bị khống chế."},
                    {"speaker": "lao_tieu_phu", "text": "Sắp tới sào huyệt rồi, hãy điều tức nội thương trước khi vào trại."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "lao_tieu_phu", "text": "Cuồng Hỏa Đan khiến đao lực tăng vọt. Cần dùng Hóa Kình giảm sát thương."}
                ]
            }
        },
        {
            "id": "hoi_1_09",
            "index": 9,
            "name": "Ba Hổ Trại Tiền Doanh",
            "kind": "normal",
            "mapPos": [0.75, 0.25],
            "description": "Tiền doanh kiên cố trước cổng trại, tinh binh hộ vệ của trại chủ bày trận nghiêm ngặt.",
            "staminaCost": 6,
            "enemy": {
                "name": "Ba Hổ Tiên Phong Vệ",
                "characterId": "ba_ho_ve",
                "portrait": "enemy_portrait_1",
                "sect": "thai_thanh",
                "tamPhapLevel": 1,
                "hp": 1220,
                "items": [
                    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "o_tam_y", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "tuy_tien_ho_lo", "x": 1, "y": 1, "tier": 2, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 550,
                    "heroExp": 120,
                    "materials": {"han_thiet": 10},
                    "items": [{"id": "kim_sang_duoc", "tier": 2, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "o_tam_y", "chancePermille": 100, "rarity": "uncommon"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Phía sau cánh cổng này chính là sảnh đường của Nộ Ba Hổ Bùi Chấn."},
                    {"speaker": "lao_tieu_phu", "text": "Tiên phong vệ này kiếm pháp thuần thục, đừng khinh địch."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Tiền doanh đã phá! Cửa lớn Ba Hổ Trại đã mở toang."},
                    {"speaker": "lao_tieu_phu", "text": "Bùi Chấn đang đợi bên trong. Hắn hung hãn như mãnh hổ hạ sơn."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "lao_tieu_phu", "text": "Địch thủ phối hợp giáp và hồ lô rất cân bằng. Xem lại bố cục trang bị."}
                ]
            }
        },
        {
            "id": "hoi_1_10",
            "index": 10,
            "name": "Thủy Hỏa Phân Tranh Bến Bạc",
            "kind": "boss",
            "mapPos": [0.85, 0.18],
            "description": "Đại bản doanh Ba Hổ Trại bên vách đá dựng đứng, Nộ Ba Hổ vung đại đao cuồng nộ.",
            "staminaCost": 10,
            "enemy": {
                "name": "Nộ Ba Hổ Bùi Chấn",
                "characterId": "bui_chen",
                "portrait": "boss_bui_chen",
                "sect": "liet_diem",
                "tamPhapLevel": 1,
                "hp": 1350,
                "items": [
                    {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "cuong_hoa_dan", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "liet_hoa_tam_kinh", "x": 1, "y": 1, "tier": 2, "rarity": "rare"},
                    {"id": "chu_sa_ho_lo", "x": 4, "y": 0, "tier": 2, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1000,
                    "heroExp": 250,
                    "materials": {"han_thiet": 15, "huyen_tinh": 5},
                    "items": [{"id": "liet_diem_dao", "tier": 2, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "chu_sa_ho_lo", "chancePermille": 150, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "bui_chen", "text": "Tiểu tử không biết trời cao đất dày! Dám xông vào Ba Hổ Trại đoạt tàn đồ?"},
                    {"speaker": "hero", "text": "Bùi trại chủ, ngươi cấu kết tà giáo hại dân lành, hôm nay phải đền tội!"},
                    {"speaker": "bui_chen", "text": "Đại đao của ta sẽ chém ngươi thành muôn mảnh!"}
                ],
                "afterWin": [
                    {"speaker": "bui_chen", "text": "Khụ... Ta thua rồi... Nhưng U Minh Giáo... sẽ không tha cho ngươi..."},
                    {"speaker": "hero", "text": "Tàn đồ Thần Nang đã tìm lại được một mảnh. Chân tướng bắt đầu hé lộ."},
                    {"speaker": "lao_tieu_phu", "text": "Làm tốt lắm tiểu hữu! Con đường phía trước dẫn vào Thập Vạn Man Uyên hiểm độc."}
                ],
                "afterLoss": [
                    {"speaker": "bui_chen", "text": "Ha ha ha! Nhãi ranh miệng còn hôi sữa đòi khiêu chiến Nộ Ba Hổ?"},
                    {"speaker": "lao_tieu_phu", "text": "Đao lực của Bùi Chấn bộc phát rất mạnh. Cần phối hợp phòng ngự và hồi phục."}
                ]
            }
        }
    ]
}

# Player loadouts for stages 1-10
P_LOADOUTS_1 = [
    # Stage 1: Starter bag
    {
        "name": "Người Chơi (Ải 1)",
        "hp": 1000,
        "cols": 5, "rows": 4,
        "sect": None,
        "tamPhapLevel": 1,
        "items": [
            {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "o_tam_y", "x": 3, "y": 0, "tier": 1, "rarity": "common"}
        ]
    },
    # Stage 2: Starter bag
    {
        "name": "Người Chơi (Ải 2)",
        "hp": 1000,
        "cols": 5, "rows": 4,
        "sect": None,
        "tamPhapLevel": 1,
        "items": [
            {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "o_tam_y", "x": 3, "y": 0, "tier": 1, "rarity": "common"}
        ]
    },
    # Stage 3: Starter bag (verifying starter bag beats 1-3)
    {
        "name": "Người Chơi (Ải 3)",
        "hp": 1000,
        "cols": 5, "rows": 4,
        "sect": None,
        "tamPhapLevel": 1,
        "items": [
            {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
            {"id": "o_tam_y", "x": 3, "y": 0, "tier": 1, "rarity": "common"}
        ]
    },
    # Stage 4: Starter + Bồi Nguyên Đan + Sói Nanh
    {
        "name": "Người Chơi (Ải 4)",
        "hp": 1020,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 1,
        "items": [
            {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "soi_nanh_canh", "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "o_tam_y", "x": 3, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 0, "y": 2, "tier": 1, "rarity": "uncommon"}
        ]
    },
    # Stage 5: T1/T2 Mix vs Elite 1
    {
        "name": "Người Chơi (Ải 5)",
        "hp": 1050,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "thai_thanh_tam_kinh", "x": 1, "y": 2, "tier": 1, "rarity": "uncommon"},
            {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 4, "y": 0, "tier": 1, "rarity": "rare"}
        ]
    },
    # Stage 6: T2 Uncommon
    {
        "name": "Người Chơi (Ải 6)",
        "hp": 1080,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "uncommon"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "uncommon"},
            {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 2, "rarity": "uncommon"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 2, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 2, "rarity": "uncommon"}
        ]
    },
    # Stage 7: T2 Uncommon/Rare
    {
        "name": "Người Chơi (Ải 7)",
        "hp": 1100,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "soi_nanh_canh", "x": 1, "y": 0, "tier": 2, "rarity": "uncommon"},
            {"id": "o_tam_y", "x": 2, "y": 0, "tier": 2, "rarity": "uncommon"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 2, "rarity": "uncommon"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 2, "rarity": "rare"}
        ]
    },
    # Stage 8: T2 Rare
    {
        "name": "Người Chơi (Ải 8)",
        "hp": 1120,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 2, "rarity": "rare"}
        ]
    },
    # Stage 9: T2 Rare
    {
        "name": "Người Chơi (Ải 9)",
        "hp": 1150,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "phi_yen_quyet", "x": 4, "y": 1, "tier": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 2, "rarity": "rare"}
        ]
    },
    # Stage 10: T2 Rare Full Build vs Boss 1
    {
        "name": "Người Chơi (Ải 10)",
        "hp": 1200,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 1,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "phi_yen_quyet", "x": 4, "y": 1, "tier": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 2, "rarity": "rare"},
            {"id": "tram_huong_lo", "x": 0, "y": 3, "tier": 2, "rarity": "rare"}
        ]
    }
]
