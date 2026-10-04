# Chapter 3 Data: Hàn Nguyệt Cô Phong (Stages 21-30)

CHAPTER_3 = {
    "id": "hoi_3",
    "index": 3,
    "title": "Hồi 3",
    "name": "Hàn Nguyệt Cô Phong",
    "summary": "Leo lên đỉnh núi tuyết ngàn năm lạnh giá, hiệp khách tương ngộ Độc Cô Hạnh, giải trừ tâm ma Hàn Sơn Phái và quyết chiến U Minh Tả Sứ Dạ Sát Thiên.",
    "theme": {
        "mapBackground": "map_hoi_3",
        "battleBackground": "bg_battle_hoi_3",
        "accent": "#2C5282",
        "paper": "#E6EDF5",
        "ink": "#16202E",
        "nodeIcon": "node_hoi_3",
        "bossNodeIcon": "node_boss_hoi_3"
    },
    "unlock": {"afterChapter": "hoi_2"},
    "stages": [
        {
            "id": "hoi_3_01",
            "index": 1,
            "name": "Tuyết Dốc Băng Khẩu",
            "kind": "normal",
            "mapPos": [0.10, 0.85],
            "description": "Lối lên đỉnh núi tuyết gió gào thét từng cơn, hộ sơn kiếm sĩ tuần tra trong bão tuyết.",
            "staminaCost": 6,
            "enemy": {
                "name": "Hàn Phong Tuần Tuyết Giả",
                "characterId": "tuan_tuyet",
                "portrait": "enemy_portrait_1",
                "sect": "han_son",
                "tamPhapLevel": 3,
                "hp": 1600,
                "items": [
                    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "han_bang_chau", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "bang_tam_giap", "x": 2, "y": 0, "tier": 3, "rarity": "epic"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1000,
                    "heroExp": 220,
                    "materials": {"han_bang_thach": 6},
                    "items": [{"id": "han_bang_chau", "tier": 3, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "bang_tam_giap", "chancePermille": 100, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Hàn phong thấu xương. Lên đến đây kiếm khí đã có thể đóng băng huyết mạch."},
                    {"speaker": "doc_co_hanh", "text": "Hàn Sơn Phái kiếm pháp lạnh lùng, dính đòn sẽ bị làm chậm tốc độ ra chiêu!"}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Kiếm khí sắc lạnh, nhưng tâm chí kiên định đã phá tan hàn băng."},
                    {"speaker": "doc_co_hanh", "text": "Tốt! Kiếm chiêu của ngươi đã đạt đến cảnh giới ngưng tụ chân khí."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Bị kiếm khí hàn băng khóa chặt rồi! Hãy dùng bảo vật chống lạnh."}
                ]
            }
        },
        {
            "id": "hoi_3_02",
            "index": 2,
            "name": "Phong Nhận Trầm Khê",
            "kind": "normal",
            "mapPos": [0.18, 0.76],
            "description": "Thung lũng băng đóng băng dòng suối ngàn năm, kiếm sĩ tinh nhuệ bày trận kiếm trận.",
            "staminaCost": 6,
            "enemy": {
                "name": "Hàn Sơn Kiếm Sĩ",
                "characterId": "han_son_kiem_si",
                "portrait": "enemy_portrait_2",
                "sect": "han_son",
                "tamPhapLevel": 3,
                "hp": 1650,
                "items": [
                    {"id": "han_ngoc_tieu", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "bang_tam_giap", "x": 2, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "han_bang_chau", "x": 4, "y": 0, "tier": 4, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1100,
                    "heroExp": 240,
                    "materials": {"han_bang_thach": 8},
                    "items": [{"id": "han_ngoc_tieu", "tier": 4, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "han_bang_chau", "chancePermille": 100, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Tiếng tiêu ngọc vang lên trong gió tuyết... Âm luật như hàn đao cắt da thịt."},
                    {"speaker": "doc_co_hanh", "text": "Hàn Ngọc Tiêu phối hợp kiếm vũ, vừa khống chế vừa gây sát thương nội tức."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Bẻ gãy tiêu ngọc, phá tan kiếm trận. Hàn Sơn Phái quả nhiên danh bất hư truyền."},
                    {"speaker": "doc_co_hanh", "text": "Họ bị tà thuật của U Minh Giáo làm mê muội tâm trí, chớ nên hạ sát thương tâm."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Tiếng tiêu ngọc làm phân tán nội lực. Cần định tâm giữ vững Cương Khí."}
                ]
            }
        },
        {
            "id": "hoi_3_03",
            "index": 3,
            "name": "Tuyết Lĩnh Hàn Phong",
            "kind": "normal",
            "mapPos": [0.26, 0.68],
            "description": "Sườn núi cheo leo tuyết phủ ngập gối, sói tuyết bạc gầm vang trong bão tuyết.",
            "staminaCost": 6,
            "enemy": {
                "name": "Tuyết Lĩnh Ngân Lang",
                "characterId": "ngan_lang",
                "portrait": "monster_ngan_lang",
                "sect": None,
                "tamPhapLevel": 3,
                "hp": 1700,
                "items": [
                    {"id": "soi_nanh_canh", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "tuyet_lang_bach_bi", "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "da_ho_y", "x": 3, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 0, "tier": 4, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1150,
                    "heroExp": 250,
                    "materials": {"bach_lang_bi": 10},
                    "items": [{"id": "tuyet_lang_bach_bi", "tier": 4, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "tuyet_lang_bach_bi", "chancePermille": 150, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Bóng trắng lao nhanh trong tuyết lốc... Ngân Lang đầu đàn!"},
                    {"speaker": "doc_co_hanh", "text": "Lông da của nó miễn nhiễm hàn khí, vuốt sói sắc bén xé toạc hộ giáp."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Thu được Tuyết Lang Bạch Bì. Tấm da này nhẹ như lông hồng mà cản được bão tuyết."},
                    {"speaker": "doc_co_hanh", "text": "Trang bị tấm da này vào túi sẽ tăng mạnh tốc độ né tránh và hộ thể."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Nó lao tới từ điểm mù trong tuyết! Đừng để nó tấn công liên hoàn."}
                ]
            }
        },
        {
            "id": "hoi_3_04",
            "index": 4,
            "name": "Băng Hà Cổ Nhai",
            "kind": "normal",
            "mapPos": [0.35, 0.60],
            "description": "Vách đá cổ bên sông băng dựng đứng, cao tăng Kim Cang Tự thân mang hộ thể kim cang.",
            "staminaCost": 6,
            "enemy": {
                "name": "Kim Cang Hộ Giới Tăng",
                "characterId": "ho_gioi_tang",
                "portrait": "enemy_portrait_2",
                "sect": "kim_cang",
                "tamPhapLevel": 3,
                "hp": 1850,
                "items": [
                    {"id": "phuc_ho_con", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "vo_niem_kinh", "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "phuc_ma_giap", "x": 2, "y": 0, "tier": 4, "rarity": "rare"},
                    {"id": "thien_tam_chuoi", "x": 4, "y": 0, "tier": 4, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1200,
                    "heroExp": 260,
                    "materials": {"han_bang_thach": 10},
                    "items": [{"id": "phuc_ma_giap", "tier": 4, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "thien_tam_chuoi", "chancePermille": 100, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Đại sư! Vì sao người xuất gia lại trấn thủ nơi hiểm địa này?"},
                    {"speaker": "doc_co_hanh", "text": "Hắn đã trúng Hám Tâm Độc của tà giáo, tâm trí chỉ còn ý niệm trấn áp!"}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Phá vỡ Phục Ma Giáp, đại sư đã tỉnh lại và ngồi thiền tĩnh tâm."},
                    {"speaker": "doc_co_hanh", "text": "Côn pháp Kim Cang hộ thể thật khủng khiếp, may mà chúng ta có bảo khí."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Cương Khí của hắn quá dày! Cần dùng đòn đánh bộc phát xuyên phá giáp."}
                ]
            }
        },
        {
            "id": "hoi_3_05",
            "index": 5,
            "name": "Phong Vân Băng Đài",
            "kind": "elite",
            "mapPos": [0.45, 0.50],
            "description": "Đài đá phong vân giữa lưng chừng trời, đệ tử chân truyền Hàn Sơn Phái xuất kiếm băng hàn.",
            "staminaCost": 8,
            "enemy": {
                "name": "Hàn Sơn Đại Đệ Tử",
                "characterId": "han_son_dai_de_tu",
                "portrait": "enemy_portrait_1",
                "sect": "han_son",
                "tamPhapLevel": 3,
                "hp": 1950,
                "items": [
                    {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
                    {"id": "bang_tam_linh_chau", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
                    {"id": "han_bang_chau", "x": 1, "y": 1, "tier": 4, "rarity": "epic"},
                    {"id": "bang_tam_giap", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
                    {"id": "kiem_tong_tam_yeu", "x": 4, "y": 0, "tier": 4, "rarity": "epic"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1500,
                    "heroExp": 320,
                    "materials": {"han_bang_thach": 15, "huyen_tinh": 6},
                    "items": [{"id": "bang_tam_linh_chau", "tier": 4, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "bang_phach_thuong", "chancePermille": 100, "rarity": "epic"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Thương pháp băng phách thật lẫm liệt, một thương đâm ra ngưng kết không khí!"},
                    {"speaker": "doc_co_hanh", "text": "Hắn sở hữu Băng Tàm Linh Châu, hồi nội lực cực nhanh và làm chậm đối thủ."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Linh châu đã rơi ra, băng thương đã hạ. Đỉnh núi chỉ còn cách một bước."},
                    {"speaker": "doc_co_hanh", "text": "Băng Tàm Linh Châu là tuyệt phẩm bảo vật! Đột phá nó sẽ mở ra kỳ tích."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Hàn khí làm chậm chiêu thức khiến ngươi lỡ nhịp. Phải hóa giải khống chế!"}
                ]
            }
        },
        {
            "id": "hoi_3_06",
            "index": 6,
            "name": "Bạch Hổ Tuyệt Hạp",
            "kind": "normal",
            "mapPos": [0.55, 0.42],
            "description": "Hẻm vực sâu thăm thẳm ngập tuyết trắng, mãnh thú chúa tể núi tuyết rình mồi.",
            "staminaCost": 6,
            "enemy": {
                "name": "Băng Nguyên Bạch Hổ",
                "characterId": "bach_ho",
                "portrait": "monster_bach_ho",
                "sect": None,
                "tamPhapLevel": 3,
                "hp": 1950,
                "items": [
                    {"id": "bach_ho_cuong_mong", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
                    {"id": "thanh_dong_ho_tam_kinh", "x": 0, "y": 1, "tier": 4, "rarity": "epic"},
                    {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
                    {"id": "thiet_cot_quyen_sao", "x": 4, "y": 0, "tier": 4, "rarity": "epic"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1300,
                    "heroExp": 280,
                    "materials": {"bach_ho_cot": 12},
                    "items": [{"id": "bach_ho_cuong_mong", "tier": 4, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "bach_ho_cuong_mong", "chancePermille": 150, "rarity": "epic"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Tiếng hổ gầm chấn động cả hẻm núi! Tuyết lở từng mảng lớn."},
                    {"speaker": "doc_co_hanh", "text": "Bạch Hổ Cuồng Móng của nó có thể xé nát kim thạch và gây choáng váng!"}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Đoạt được Bạch Hổ Cuồng Móng, sát khí bộc phát vô song."},
                    {"speaker": "doc_co_hanh", "text": "Vũ khí này khi kẻ địch choáng váng sẽ gây sát thương chí mạng cực lớn."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Bị choáng váng liên tục khiến ngươi không thể xuất chiêu! Cần tăng kháng."}
                ]
            }
        },
        {
            "id": "hoi_3_07",
            "index": 7,
            "name": "Hàn Tinh Thạch Thất",
            "kind": "normal",
            "mapPos": [0.65, 0.35],
            "description": "Thạch thất cổ khắc đầy tà văn U Minh Giáo, chấp pháp sứ áo tím âm hiểm phục kích.",
            "staminaCost": 6,
            "enemy": {
                "name": "U Minh Chấp Pháp Sứ",
                "characterId": "chap_phap_su",
                "portrait": "enemy_portrait_3",
                "sect": "huyen_doc",
                "tamPhapLevel": 3,
                "hp": 2000,
                "items": [
                    {"id": "u_minh_quy_trao", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
                    {"id": "da_ho_y", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
                    {"id": "man_xa_doc_tui", "x": 3, "y": 0, "tier": 4, "rarity": "epic"},
                    {"id": "u_minh_bi_luc", "x": 3, "y": 1, "tier": 4, "rarity": "epic"},
                    {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 4, "rarity": "epic"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1400,
                    "heroExp": 300,
                    "materials": {"u_minh_thao": 15, "han_bang_thach": 10},
                    "items": [{"id": "u_minh_quy_trao", "tier": 4, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "u_minh_quy_trao", "chancePermille": 150, "rarity": "epic"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "U Minh Giáo quả nhiên đã cắm rễ tận đỉnh tuyết sơn này."},
                    {"speaker": "doc_co_hanh", "text": "Chấp Pháp Sứ dùng U Minh Quỷ Trảo, mỗi đòn hút máu nuôi tà công!"}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Quỷ trảo đã bị chém đứt. Tà văn trong thạch thất tan biến."},
                    {"speaker": "doc_co_hanh", "text": "U Minh Quỷ Trảo này kích thước lớn nhưng năng lực sinh tồn cực cao."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Hắn hút cạn sinh lực để hồi phục! Phải dồn sát thương kết liễu nhanh."}
                ]
            }
        },
        {
            "id": "hoi_3_08",
            "index": 8,
            "name": "Phong Ma Cổ Đạo",
            "kind": "normal",
            "mapPos": [0.73, 0.28],
            "description": "Con đường đá cổ ngập tràn băng tảng khổng lồ, huyền quy ngàn năm phong tỏa lối đi.",
            "staminaCost": 6,
            "enemy": {
                "name": "Vạn Niên Băng Quy",
                "characterId": "bang_quy",
                "portrait": "monster_bang_quy",
                "sect": None,
                "tamPhapLevel": 4,
                "hp": 2150,
                "items": [
                    {"id": "bang_phach_co_cot", "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
                    {"id": "thien_nien_quy_giap", "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
                    {"id": "han_bang_chau", "x": 4, "y": 0, "tier": 5, "rarity": "epic"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1500,
                    "heroExp": 320,
                    "materials": {"bang_quy_giap": 15},
                    "items": [{"id": "bang_phach_co_cot", "tier": 5, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "bang_phach_co_cot", "chancePermille": 150, "rarity": "epic"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Khối đá băng khổng lồ kia... cử động rồi! Là một con huyền quy ngàn năm!"},
                    {"speaker": "doc_co_hanh", "text": "Thiên Niên Quy Giáp và Băng Phách Cổ Cốt! Phòng thủ tuyệt đối, bất khả xâm phạm."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Lớp mai rùa kiên cố cuối cùng cũng nứt ra. Chúng ta đã có thần giáp trong tay."},
                    {"speaker": "doc_co_hanh", "text": "Bảo vật quy giáp này là chìa khóa để đối đầu Tuyệt Kỹ của Dạ Sát Thiên."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Đánh vào mai rùa bị phản chấn quá đau! Cần điều hòa lại nhịp công kích."}
                ]
            }
        },
        {
            "id": "hoi_3_09",
            "index": 9,
            "name": "Vọng Nguyệt Băng Đỉnh",
            "kind": "normal",
            "mapPos": [0.82, 0.20],
            "description": "Vọng nguyệt đài ngập tràn hàn khí tử thần, thân vệ kiếm khách bảo vệ tế đàn quyết tử.",
            "staminaCost": 6,
            "enemy": {
                "name": "U Minh Tuyệt Mệnh Kiếm Vệ",
                "characterId": "tuyet_menh_ve",
                "portrait": "enemy_portrait_1",
                "sect": "thai_thanh",
                "tamPhapLevel": 4,
                "hp": 2100,
                "items": [
                    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
                    {"id": "phi_yen_quyet", "x": 1, "y": 0, "tier": 5, "rarity": "epic"},
                    {"id": "tuy_tien_ho_lo", "x": 1, "y": 1, "tier": 5, "rarity": "epic"},
                    {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 5, "rarity": "epic"},
                    {"id": "kiem_tong_tam_yeu", "x": 4, "y": 0, "tier": 5, "rarity": "epic"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1600,
                    "heroExp": 340,
                    "materials": {"han_bang_thach": 15, "huyen_tinh": 8},
                    "items": [{"id": "kiem_tong_tam_yeu", "tier": 5, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "kim_tuyen_y", "chancePermille": 100, "rarity": "epic"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Phía trên chính là Phong Thiền Băng Đài. Không ai có thể ngăn ta bước tiếp!"},
                    {"speaker": "doc_co_hanh", "text": "Tử vệ cuối cùng này đã dâng hiến linh hồn cho tà kiếm, kiếm chiêu cực nhanh!"}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Đài tế đã mở! Dạ Sát Thiên, món nợ máu mười năm trước hôm nay phải tính!"},
                    {"speaker": "doc_co_hanh", "text": "Hiệp khách huynh đệ, ta sẽ hộ vệ phía sau, hãy dốc toàn lực diệt trừ ma đầu!"}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "doc_co_hanh", "text": "Kiếm chiêu quá gấp gáp. Cần tĩnh tâm, vận chuyển Tâm Pháp tối cao."}
                ]
            }
        },
        {
            "id": "hoi_3_10",
            "index": 10,
            "name": "Phong Thiền Băng Đài",
            "kind": "boss",
            "mapPos": [0.90, 0.12],
            "description": "Đỉnh tuyết sơn ngút ngàn mây gió, U Minh Tả Sứ Dạ Sát Thiên bày trận pháp luyện hóa Thần Nang.",
            "staminaCost": 10,
            "enemy": {
                "name": "U Minh Tả Sứ Dạ Sát Thiên",
                "characterId": "da_sat_thien",
                "portrait": "boss_da_sat_thien",
                "sect": "han_son",
                "tamPhapLevel": 4,
                "hp": 2350,
                "items": [
                    {"id": "bang_phach_thuong", "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
                    {"id": "thien_nien_quy_giap", "x": 1, "y": 0, "tier": 5, "rarity": "legendary"},
                    {"id": "bang_tam_giap", "x": 1, "y": 2, "tier": 5, "rarity": "legendary"},
                    {"id": "bang_tam_linh_chau", "x": 3, "y": 0, "tier": 5, "rarity": "legendary"},
                    {"id": "han_bang_chau", "x": 3, "y": 1, "tier": 5, "rarity": "legendary"},
                    {"id": "kiem_tong_tam_yeu", "x": 4, "y": 0, "tier": 5, "rarity": "legendary"},
                    {"id": "cuu_chuyen_hoan_hon_dan", "x": 4, "y": 1, "tier": 5, "rarity": "legendary"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 3000,
                    "heroExp": 800,
                    "materials": {"han_bang_thach": 30, "huyen_tinh": 15},
                    "items": [{"id": "thien_nien_quy_giap", "tier": 5, "rarity": "legendary"}]
                },
                "repeat": "default",
                "drops": [{"id": "thien_nien_quy_giap", "chancePermille": 250, "rarity": "legendary"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "da_sat_thien", "text": "Mười năm trước sơn trang của ngươi bị hủy diệt, hôm nay lại tự tìm tới cửa nộp mạng sao?"},
                    {"speaker": "hero", "text": "Dạ Sát Thiên! Nợ máu sơn trang và tà tâm đồ sát Cửu Châu của ngươi sẽ chấm dứt tại đây!"},
                    {"speaker": "da_sat_thien", "text": "Càn khôn tàn nang trên tay ngươi sẽ hoàn thiện Thần Nang của U Minh Giáo. Tiếp chiêu đi!"}
                ],
                "afterWin": [
                    {"speaker": "da_sat_thien", "text": "Phốc... Thần Nang sao có thể nhận ngươi làm chủ... Giáo chủ... sẽ không tha..."},
                    {"speaker": "hero", "text": "Tà bất thắng chính. Ba mảnh tàn nang đã hợp nhất thành một khối hoàn chỉnh!"},
                    {"speaker": "doc_co_hanh", "text": "Đại thắng rồi! Nhưng Dạ Sát Thiên chỉ là Tả Sứ, Thao Thiết Giáo Chủ vẫn đang ẩn thân nơi Trung Nguyên..."}
                ],
                "afterLoss": [
                    {"speaker": "da_sat_thien", "text": "Kiến cỏ đòi lay đại thụ! Tàn nang này thuộc về U Minh Giáo ta!"},
                    {"speaker": "doc_co_hanh", "text": "Thương pháp băng phách và giáp quy của hắn quá hoàn hảo. Cần trang bị Trọng 5 Tinh Phẩm mới địch nổi!"}
                ]
            }
        }
    ]
}

# Player loadouts for stages 21-30
P_LOADOUTS_3 = [
    # Stage 21: T4 Rare
    {
        "name": "Người Chơi (Ải 21)",
        "hp": 1500,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 3,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 4, "rarity": "rare"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 4, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 1, "tier": 4, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 4, "rarity": "rare"}
        ]
    },
    # Stage 22: T4 Rare/Epic
    {
        "name": "Người Chơi (Ải 22)",
        "hp": 1550,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 3,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "soi_nanh_canh", "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 1, "tier": 4, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 4, "rarity": "epic"}
        ]
    },
    # Stage 23: T4 Epic
    {
        "name": "Người Chơi (Ải 23)",
        "hp": 1580,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 3,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "phi_yen_quyet", "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 4, "rarity": "epic"}
        ]
    },
    # Stage 24: T4 Epic
    {
        "name": "Người Chơi (Ải 24)",
        "hp": 1620,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 3,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "soi_nanh_canh", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "tuyet_lang_bach_bi", "x": 0, "y": 3, "tier": 4, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 4, "rarity": "epic"}
        ]
    },
    # Stage 25: T4 Epic/Legendary vs Elite 3
    {
        "name": "Người Chơi (Ải 25)",
        "hp": 1680,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 3,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "tuyet_lang_bach_bi", "x": 0, "y": 3, "tier": 4, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 4, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 4, "rarity": "epic"}
        ]
    },
    # Stage 26: T4 Legendary
    {
        "name": "Người Chơi (Ải 26)",
        "hp": 1720,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 4,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "legendary"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 4, "rarity": "legendary"},
            {"id": "tuyet_lang_bach_bi", "x": 0, "y": 3, "tier": 4, "rarity": "legendary"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 4, "rarity": "legendary"},
            {"id": "bang_tam_linh_chau", "x": 4, "y": 1, "tier": 4, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 4, "rarity": "legendary"}
        ]
    },
    # Stage 27: T4/T5 Legendary
    {
        "name": "Người Chơi (Ải 27)",
        "hp": 1750,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 4,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "rare"},
            {"id": "bach_ho_cuong_mong", "x": 1, "y": 0, "tier": 4, "rarity": "legendary"},
            {"id": "heo_rung_bap_giap", "x": 3, "y": 0, "tier": 4, "rarity": "legendary"},
            {"id": "tuyet_lang_bach_bi", "x": 1, "y": 1, "tier": 4, "rarity": "legendary"},
            {"id": "bang_tam_linh_chau", "x": 3, "y": 2, "tier": 4, "rarity": "legendary"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 2, "tier": 4, "rarity": "legendary"},
            {"id": "boi_nguyen_dan", "x": 0, "y": 3, "tier": 4, "rarity": "legendary"}
        ]
    },
    # Stage 28: T5 Legendary
    {
        "name": "Người Chơi (Ải 28)",
        "hp": 1780,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 4,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "bach_ho_cuong_mong", "x": 1, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 3, "y": 0, "tier": 5, "rarity": "epic"},
            {"id": "tuyet_lang_bach_bi", "x": 1, "y": 1, "tier": 5, "rarity": "epic"},
            {"id": "bang_tam_linh_chau", "x": 3, "y": 2, "tier": 5, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 2, "tier": 5, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 0, "y": 3, "tier": 5, "rarity": "epic"}
        ]
    },
    # Stage 29: T5 Legendary
    {
        "name": "Người Chơi (Ải 29)",
        "hp": 1800,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 4,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "bach_ho_cuong_mong", "x": 1, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "heo_rung_bap_giap", "x": 3, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "tuyet_lang_bach_bi", "x": 1, "y": 1, "tier": 5, "rarity": "legendary"},
            {"id": "bang_tam_linh_chau", "x": 3, "y": 2, "tier": 5, "rarity": "legendary"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 2, "tier": 5, "rarity": "legendary"},
            {"id": "boi_nguyen_dan", "x": 0, "y": 3, "tier": 5, "rarity": "legendary"}
        ]
    },
    # Stage 30: Trọng 5 Tinh Phẩm Full Build vs Boss 3 Dạ Sát Thiên
    {
        "name": "Người Chơi (Ải 30)",
        "hp": 1850,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 4,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "bach_ho_cuong_mong", "x": 1, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "heo_rung_bap_giap", "x": 3, "y": 0, "tier": 5, "rarity": "legendary"},
            {"id": "bang_tam_linh_chau", "x": 1, "y": 1, "tier": 5, "rarity": "legendary"},
            {"id": "hac_sat_hung_dam", "x": 2, "y": 1, "tier": 5, "rarity": "legendary"},
            {"id": "tuyet_lang_bach_bi", "x": 1, "y": 2, "tier": 5, "rarity": "legendary"},
            {"id": "boi_nguyen_dan", "x": 0, "y": 3, "tier": 5, "rarity": "legendary"},
            {"id": "cuu_chuyen_hoan_hon_dan", "x": 3, "y": 2, "tier": 5, "rarity": "legendary"}
        ]
    }
]
