# Chapter 2 Data: Thập Vạn Man Uyên (Stages 11-20)

CHAPTER_2 = {
    "id": "hoi_2",
    "index": 2,
    "title": "Hồi 2",
    "name": "Thập Vạn Man Uyên",
    "summary": "Tiến sâu vào vùng đầm lầy chướng khí ngút trời, hiệp khách cùng Bạch Linh Nhi giải trừ độc chướng, đối đầu thế lực Huyền Độc Môn và ác thú ngàn năm.",
    "theme": {
        "mapBackground": "map_hoi_2",
        "battleBackground": "bg_battle_hoi_2",
        "accent": "#1B5E3B",
        "paper": "#E5EAD8",
        "ink": "#1A241C",
        "nodeIcon": "node_hoi_2",
        "bossNodeIcon": "node_boss_hoi_2"
    },
    "unlock": {"afterChapter": "hoi_1"},
    "stages": [
        {
            "id": "hoi_2_01",
            "index": 1,
            "name": "Man Uyên Nhập Khẩu",
            "kind": "normal",
            "mapPos": [0.10, 0.80],
            "description": "Lối vào chướng cốc mù sương tím biếc, trinh sát man tộc cảnh giác rình rập.",
            "staminaCost": 6,
            "enemy": {
                "name": "Man Tộc Thám Lộ Giả",
                "characterId": "man_toc",
                "portrait": "enemy_portrait_3",
                "sect": "huyen_doc",
                "tamPhapLevel": 2,
                "hp": 1250,
                "items": [
                    {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "da_ho_y", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "van_doc_dinh", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 600,
                    "heroExp": 130,
                    "materials": {"u_minh_thao": 5},
                    "items": [{"id": "giai_doc_dan", "tier": 2, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "tu_kim_trao", "chancePermille": 100, "rarity": "uncommon"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Khí độc nồng nặc quá, hít vào thấy lồng ngực căng rát."},
                    {"speaker": "tieu_linh_nhi", "text": "Uống viên Giải Độc Đan này đi! Thám tử man tộc phía trước đã phát hiện chúng ta."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Độc trảo của bọn chúng quả nhiên hiểm hóc."},
                    {"speaker": "tieu_linh_nhi", "text": "Man Uyên này còn nhiều cạm bẫy lắm, chúng ta phải cẩn thận từng bước."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Ngươi trúng độc tích tụ rồi! Phải chuẩn bị đan dược giải độc kịp thời."}
                ]
            }
        },
        {
            "id": "hoi_2_02",
            "index": 2,
            "name": "Ô Đầm Nhai",
            "kind": "normal",
            "mapPos": [0.18, 0.72],
            "description": "Vách đá rêu phong bên đầm bùn đen kịch, môn đồ Huyền Độc Môn tụ tập luyện độc.",
            "staminaCost": 6,
            "enemy": {
                "name": "Huyền Độc Môn Đồ",
                "characterId": "huyen_doc_do",
                "portrait": "enemy_portrait_3",
                "sect": "huyen_doc",
                "tamPhapLevel": 2,
                "hp": 1300,
                "items": [
                    {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "o_tam_y", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
                    {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 2, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 650,
                    "heroExp": 140,
                    "materials": {"u_minh_thao": 6},
                    "items": [{"id": "u_minh_bi_luc", "tier": 2, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "van_doc_dinh", "chancePermille": 100, "rarity": "uncommon"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Những vò gốm kia đang bốc khói đen... Chúng đang luyện độc công."},
                    {"speaker": "tieu_linh_nhi", "text": "Huyền Độc Môn dùng ám khí và độc trảo tích tầng độc tố, chớ kéo dài trận chiến!"}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Đoạt được mật tịch U Minh Bí Lục, ghi chép cách vận chuyển độc khí."},
                    {"speaker": "tieu_linh_nhi", "text": "Hiểu rõ cơ chế độc của chúng, sau này giao chiến sẽ dễ dàng hơn."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Độc tầng quá cao khiến sát thương bộc phát! Phải tốc chiến tốc thắng."}
                ]
            }
        },
        {
            "id": "hoi_2_03",
            "index": 3,
            "name": "Hủ Cốt Thung",
            "kind": "normal",
            "mapPos": [0.26, 0.65],
            "description": "Bãi xương trắng rợn người, nhện độc khổng lồ giăng tơ khắp các thân cổ thụ.",
            "staminaCost": 6,
            "enemy": {
                "name": "Kịch Độc Tri Chu",
                "characterId": "doc_chu",
                "portrait": "monster_tri_chu",
                "sect": None,
                "tamPhapLevel": 2,
                "hp": 1350,
                "items": [
                    {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "da_ho_y", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "man_xa_doc_tui", "x": 2, "y": 0, "tier": 2, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 700,
                    "heroExp": 150,
                    "materials": {"doc_tuyen": 8},
                    "items": [{"id": "kich_doc_chu_nga", "tier": 3, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "kich_doc_chu_nga", "chancePermille": 120, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Mạng nhện dính chặt lấy binh khí... Con nhện độc này to như bò mộng!"},
                    {"speaker": "tieu_linh_nhi", "text": "Kịch Độc Chu Ngà của nó chứa nọc cực mạnh, hãy cẩn thận nanh độc!"}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Thu được cặp chu ngà sắc bén. Thứ này phối hợp độc tính rất tốt."},
                    {"speaker": "tieu_linh_nhi", "text": "Đúng vậy! Binh khí này làm tăng sát thương mỗi khi kẻ địch trúng độc."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Nanh độc nhện cắn rách hộ thể rồi. Cần tăng cường Cương Khí bảo vệ."}
                ]
            }
        },
        {
            "id": "hoi_2_04",
            "index": 4,
            "name": "Xà Khẩu Hiệp",
            "kind": "normal",
            "mapPos": [0.35, 0.58],
            "description": "Hẻm núi uốn lượn như miệng rắn, vu đao man tộc múa may tà thuật cổ xưa.",
            "staminaCost": 6,
            "enemy": {
                "name": "Man Cương Vu Đao",
                "characterId": "vu_dao",
                "portrait": "enemy_portrait_3",
                "sect": "bich_thuy",
                "tamPhapLevel": 2,
                "hp": 1380,
                "items": [
                    {"id": "thanh_dang_tien", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "duong_sinh_quyet", "x": 1, "y": 0, "tier": 3, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 750,
                    "heroExp": 160,
                    "materials": {"u_minh_thao": 8},
                    "items": [{"id": "thanh_dang_tien", "tier": 3, "rarity": "rare"}]
                },
                "repeat": "default",
                "drops": [{"id": "duong_sinh_quyet", "chancePermille": 100, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Roi mây xanh biếc cuốn gió phần phật... Đao pháp này có nguồn gốc Vạn Hoa Cốc?"},
                    {"speaker": "tieu_linh_nhi", "text": "Hắn dùng Dưỡng Sinh Quyết kết hợp roi gai độc, hồi phục sinh lực liên tục."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Phá được thế roi liên hoàn. Hắn đã bỏ chạy vào sâu trong cốc."},
                    {"speaker": "tieu_linh_nhi", "text": "Vạn Hoa Cốc vốn dĩ thanh tao, sao lại có kẻ sa vào tà thuật thế này..."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Hắn hồi máu quá nhanh. Phải dồn sát thương bộc phát để hạ gục hắn!"}
                ]
            }
        },
        {
            "id": "hoi_2_05",
            "index": 5,
            "name": "U Cốc Dược Đàn",
            "kind": "elite",
            "mapPos": [0.44, 0.50],
            "description": "Đàn tế dược cổ xưa giữa thung lũng, phản đồ Vạn Hoa Cốc đang luyện chế độc đan.",
            "staminaCost": 8,
            "enemy": {
                "name": "Vạn Hoa Nghịch Đồ",
                "characterId": "van_hoa_nghich_do",
                "portrait": "enemy_portrait_2",
                "sect": "bich_thuy",
                "tamPhapLevel": 2,
                "hp": 1500,
                "items": [
                    {"id": "bich_thuy_cam", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "nho_sam_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "hoi_thien_dan", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "thanh_tam_dan", "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
                    {"id": "duong_sinh_quyet", "x": 2, "y": 1, "tier": 3, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 900,
                    "heroExp": 200,
                    "materials": {"u_minh_thao": 10, "linh_chi": 3},
                    "items": [{"id": "hoi_thien_dan", "tier": 3, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "bich_thuy_cam", "chancePermille": 100, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "tieu_linh_nhi", "text": "Sư huynh! Vì sao huynh lại phản bội cốc chủ, câu kết với U Minh Giáo?"},
                    {"speaker": "hero", "text": "Tiếng đàn ma quái... Hắn dùng âm luật khống chế dược tính!"}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Đàn đứt, khúc tàn. Dược đàn đã bị phong tỏa."},
                    {"speaker": "tieu_linh_nhi", "text": "Muội đã thu hồi bí dược của bổn môn. Đa tạ hiệp khách trượng nghĩa tương trợ!"}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Tiếng đàn hồi máu và gây choáng liên tục. Cần ngắt nhịp thi triển của hắn."}
                ]
            }
        },
        {
            "id": "hoi_2_06",
            "index": 6,
            "name": "Mê Vụ Đàm",
            "kind": "normal",
            "mapPos": [0.52, 0.45],
            "description": "Đầm nước sương mù mờ ảo, cự hùng lông đen hắc ám từ dưới nước trồi lên.",
            "staminaCost": 6,
            "enemy": {
                "name": "Hắc Sát Độc Hùng",
                "characterId": "doc_hung",
                "portrait": "monster_gau_den",
                "sect": None,
                "tamPhapLevel": 2,
                "hp": 1500,
                "items": [
                    {"id": "hac_sat_hung_dam", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "heo_rung_bap_giap", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "thiet_cot_quyen_sao", "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "thanh_dong_ho_tam_kinh", "x": 0, "y": 1, "tier": 3, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 800,
                    "heroExp": 170,
                    "materials": {"da_thu": 12},
                    "items": [{"id": "hac_sat_hung_dam", "tier": 3, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "hac_sat_hung_dam", "chancePermille": 150, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Một con hắc hùng khổng lồ! Móng vuốt của nó đen kịt độc chướng."},
                    {"speaker": "tieu_linh_nhi", "text": "Hắc Sát Hùng Đảm của nó là dược liệu vô giá, nhưng độc khí của nó cực mạnh."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Hùng đan đã vào tay. Sinh lực dồi dào, giải trừ được nhiều chướng độc."},
                    {"speaker": "tieu_linh_nhi", "text": "Hùng đảm này dùng làm đan dược phụ trợ sẽ tăng cường hộ thể đáng kinh ngạc."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Cú tát của gấu đen quá nặng! Phải giữ vững Hóa Kình giảm chấn thương."}
                ]
            }
        },
        {
            "id": "hoi_2_07",
            "index": 7,
            "name": "Lam Hoa Trạch",
            "kind": "normal",
            "mapPos": [0.60, 0.38],
            "description": "Đầm hoa lam nở rộ huyền ảo, sát thủ bóng tối ẩn mình dưới cánh hoa độc.",
            "staminaCost": 6,
            "enemy": {
                "name": "U Minh Ám Sát Thí Tử",
                "characterId": "am_sat_thi_tu",
                "portrait": "enemy_portrait_1",
                "sect": "huyen_doc",
                "tamPhapLevel": 2,
                "hp": 1450,
                "items": [
                    {"id": "that_tinh_tieu", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "that_tinh_tieu", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "da_ho_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "van_doc_dinh", "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "u_minh_bi_luc", "x": 4, "y": 0, "tier": 3, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 850,
                    "heroExp": 180,
                    "materials": {"u_minh_thao": 10},
                    "items": [{"id": "van_doc_dinh", "tier": 3, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "that_tinh_tieu", "chancePermille": 120, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Hoa lam tuy đẹp nhưng hương thơm lại khiến đầu óc choáng váng."},
                    {"speaker": "tieu_linh_nhi", "text": "Là tử sĩ của U Minh Giáo! Chúng dùng độc hoa che giấu ám khí bạo kích."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Ám sát thất bại, bọn chúng đã tự vẫn. Kỷ luật của giáo phái này thật đáng sợ."},
                    {"speaker": "tieu_linh_nhi", "text": "Chúng quyết giữ bí mật đàn tế phía trước. Chúng ta càng phải đến đó."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Ám khí bạo kích tầm gần quá nhanh! Cần nâng cao hộ giáp kiên cố."}
                ]
            }
        },
        {
            "id": "hoi_2_08",
            "index": 8,
            "name": "Độc Long Khê",
            "kind": "normal",
            "mapPos": [0.68, 0.30],
            "description": "Dòng suối nước xanh biếc sôi sùng sục, trưởng lão Huyền Độc Môn đích thân trấn giữ.",
            "staminaCost": 6,
            "enemy": {
                "name": "Huyền Độc Trưởng Lão",
                "characterId": "huyen_doc_truong_lao",
                "portrait": "enemy_portrait_3",
                "sect": "huyen_doc",
                "tamPhapLevel": 2,
                "hp": 1550,
                "items": [
                    {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "tu_kim_trao", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "o_tam_y", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
                    {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "rare"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 900,
                    "heroExp": 190,
                    "materials": {"u_minh_thao": 12, "huyen_tinh": 4},
                    "items": [{"id": "tu_kim_trao", "tier": 3, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "o_tam_y", "chancePermille": 100, "rarity": "rare"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Khí tức âm lãnh này... Ngươi là trưởng lão Huyền Độc Môn?"},
                    {"speaker": "tieu_linh_nhi", "text": "Lão tặc này luyện vạn độc vào cơ thể, móng vuốt đã biến thành hắc sắc."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Song trảo đã gãy. Trưởng lão tà môn đã quy phục."},
                    {"speaker": "tieu_linh_nhi", "text": "Tử Kim Trảo phẩm chất cực cao, uy lực bộc phát độc công phi phàm."}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Song trảo cào rách liên tục khiến độc ngấm sâu. Cần dùng Hóa Kình chống đỡ."}
                ]
            }
        },
        {
            "id": "hoi_2_09",
            "index": 9,
            "name": "Vạn Xà Quật",
            "kind": "normal",
            "mapPos": [0.76, 0.22],
            "description": "Hang động ngàn xà lúc nhúc, con xà tinh hỏa diệm phun nọc độc đỏ rực.",
            "staminaCost": 6,
            "enemy": {
                "name": "Xích Luyện Xà Tinh",
                "characterId": "xa_tinh",
                "portrait": "monster_xich_xa",
                "sect": None,
                "tamPhapLevel": 2,
                "hp": 1600,
                "items": [
                    {"id": "hoa_ngiem_xa_cot", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "da_ho_y", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "man_xa_doc_tui", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "thanh_dang_tien", "x": 3, "y": 0, "tier": 3, "rarity": "epic"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 950,
                    "heroExp": 200,
                    "materials": {"xa_bi": 15},
                    "items": [{"id": "hoa_ngiem_xa_cot", "tier": 3, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "hoa_ngiem_xa_cot", "chancePermille": 150, "rarity": "epic"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hero", "text": "Tiếng rít rợn người khắp trần hang... Cự xà toàn thân đỏ rực như than hồng!"},
                    {"speaker": "tieu_linh_nhi", "text": "Xích Luyện Xà Tinh ngàn năm! Xương cốt của nó là Hỏa Diệm Xà Cốt huyền thoại."}
                ],
                "afterWin": [
                    {"speaker": "hero", "text": "Đoạt được Hỏa Diệm Xà Cốt, nhiệt khí và độc tính hòa quyện tuyệt vời."},
                    {"speaker": "tieu_linh_nhi", "text": "Sau hang động này chính là đàn tế của Hắc Độc Nương Tử!"}
                ],
                "afterLoss": [
                {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
                    {"speaker": "tieu_linh_nhi", "text": "Đòn quất đuôi xà cực mạnh kết hợp thiêu đốt! Cần giữ vững phòng tuyến."}
                ]
            }
        },
        {
            "id": "hoi_2_10",
            "index": 10,
            "name": "Thần Bí Cổ Đàn",
            "kind": "boss",
            "mapPos": [0.86, 0.15],
            "description": "Đàn tế cổ ngập tràn chướng khí, Hắc Độc Nương Tử triệu hoán vạn độc quyết chiến.",
            "staminaCost": 10,
            "enemy": {
                "name": "Hắc Độc Nương Tử",
                "characterId": "hac_doc_nuong_tu",
                "portrait": "boss_hac_doc_nuong_tu",
                "sect": "huyen_doc",
                "tamPhapLevel": 2,
                "hp": 1750,
                "items": [
                    {"id": "tu_kim_trao", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "man_xa_doc_tui", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "u_minh_bi_luc", "x": 1, "y": 1, "tier": 3, "rarity": "epic"},
                    {"id": "o_thiet_giap", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "van_doc_dinh", "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
                    {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "epic"}
                ]
            },
            "rewards": {
                "firstClear": {
                    "silver": 1500,
                    "heroExp": 350,
                    "materials": {"u_minh_thao": 20, "huyen_tinh": 8},
                    "items": [{"id": "man_xa_doc_tui", "tier": 3, "rarity": "epic"}]
                },
                "repeat": "default",
                "drops": [{"id": "man_xa_doc_tui", "chancePermille": 200, "rarity": "epic"}]
            },
            "dialogue": {
                "before": [
                    {"speaker": "hac_doc_nuong_tu", "text": "Kẻ nào dám kinh động đại trận của bổn nương tử? Lại là tiểu nha đầu Vạn Hoa Cốc sao?"},
                    {"speaker": "hero", "text": "Hắc Độc Nương Tử, giao mảnh tàn đồ Thần Nang ra đây!"},
                    {"speaker": "hac_doc_nuong_tu", "text": "Muốn tàn đồ? Hãy nếm thử Mãng Xà Độc Túi vạn kiếp bất phục!"}
                ],
                "afterWin": [
                    {"speaker": "hac_doc_nuong_tu", "text": "Không thể nào... Vạn độc của ta sao lại bị phá giải... Tả Sứ sẽ trả thù cho ta..."},
                    {"speaker": "hero", "text": "Mảnh tàn chương thứ hai đã thu hồi! Man Uyên chướng khí bắt đầu tan dần."},
                    {"speaker": "tieu_linh_nhi", "text": "Tả Sứ mà mụ ta nhắc tới chính là Dạ Sát Thiên đang chiếm giữ Hàn Nguyệt Cô Phong!"}
                ],
                "afterLoss": [
                    {"speaker": "hac_doc_nuong_tu", "text": "Ha ha ha! Trúng vạn độc của bổn nương tử thì thần tiên cũng khó cứu!"},
                    {"speaker": "tieu_linh_nhi", "text": "Độc tố bộc phát toàn diện rồi. Cần trang bị Giải Độc Đan và tăng tốc diệt địch."}
                ]
            }
        }
    ]
}

# Player loadouts for stages 11-20
P_LOADOUTS_2 = [
    # Stage 11: T2/T3 transition
    {
        "name": "Người Chơi (Ải 11)",
        "hp": 1220,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 2, "rarity": "rare"}
        ]
    },
    # Stage 12: T3 Uncommon/Rare
    {
        "name": "Người Chơi (Ải 12)",
        "hp": 1250,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "uncommon"},
            {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 3, "rarity": "uncommon"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "uncommon"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
            {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 2, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "uncommon"}
        ]
    },
    # Stage 13: T3 Rare
    {
        "name": "Người Chơi (Ải 13)",
        "hp": 1280,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "soi_nanh_canh", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "rare"}
        ]
    },
    # Stage 14: T3 Rare
    {
        "name": "Người Chơi (Ải 14)",
        "hp": 1300,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "rare"}
        ]
    },
    # Stage 15: T3 Rare/Epic vs Elite 2
    {
        "name": "Người Chơi (Ải 15)",
        "hp": 1350,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "phi_yen_quyet", "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "rare"}
        ]
    },
    # Stage 16: T3 Rare/Epic
    {
        "name": "Người Chơi (Ải 16)",
        "hp": 1380,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "kim_tuyen_y", "x": 2, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "rare"}
        ]
    },
    # Stage 17: T3 Epic
    {
        "name": "Người Chơi (Ải 17)",
        "hp": 1400,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "soi_nanh_canh", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "rare"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 1, "tier": 3, "rarity": "rare"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "rare"}
        ]
    },
    # Stage 18: T3 Epic
    {
        "name": "Người Chơi (Ải 18)",
        "hp": 1420,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "tuy_tien_ho_lo", "x": 4, "y": 1, "tier": 3, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "epic"}
        ]
    },
    # Stage 19: T3 Epic
    {
        "name": "Người Chơi (Ải 19)",
        "hp": 1450,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "phi_yen_quyet", "x": 4, "y": 1, "tier": 3, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "epic"}
        ]
    },
    # Stage 20: T3/T4 Epic Build vs Boss 2
    {
        "name": "Người Chơi (Ải 20)",
        "hp": 1500,
        "cols": 5, "rows": 4,
        "sect": "thai_thanh",
        "tamPhapLevel": 2,
        "items": [
            {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
            {"id": "kich_doc_chu_nga", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "tier": 3, "rarity": "epic"},
            {"id": "giai_doc_dan", "x": 4, "y": 1, "tier": 3, "rarity": "epic"},
            {"id": "phi_yen_quyet", "x": 4, "y": 2, "tier": 3, "rarity": "epic"},
            {"id": "boi_nguyen_dan", "x": 1, "y": 2, "tier": 3, "rarity": "epic"}
        ]
    }
]
