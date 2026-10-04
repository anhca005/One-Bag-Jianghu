import json
import sys

# 1. items.patch.json
items_patch = [
    {
        "path": "items[0].name",
        "old": "Thiết Cốt Quyền Sáo",
        "new": "Thiết Cốt Quyền"
    },
    {
        "path": "items[10].name",
        "old": "Thái Thanh Tâm Kinh",
        "new": "Thái Thanh Kinh"
    },
    {
        "path": "items[29].name",
        "old": "Thanh Đồng Hộ Tâm Kính",
        "new": "Đồng Hộ Tâm Kính"
    },
    {
        "path": "items[33].name",
        "old": "Bách Luyện Chân Kinh",
        "new": "Bách Luyện Kinh"
    },
    {
        "path": "items[38].name",
        "old": "Thiên Địa Vô Cực Công",
        "new": "Thiên Địa Vô Cực"
    },
    {
        "path": "items[49].name",
        "old": "Cửu Chuyển Hoàn Hồn Đan",
        "new": "Cửu Chuyển Đan"
    },
    {
        "path": "items[72].name",
        "old": "Thiên Niên Quy Giáp",
        "new": "Cổ Quy Giáp"
    }
]

# 2. materials.patch.json
materials_patch = [
    {
        "path": "materials[2].name",
        "old": "Thiên Ngoại Vẫn Thiết",
        "new": "Thiên Ngoại Thiết"
    },
    {
        "path": "materials[3].name",
        "old": "Binh Hồn Tinh Thạch",
        "new": "Binh Hồn Thạch"
    },
    {
        "path": "materials[7].name",
        "old": "Huyền Giáp Hồn Thạch",
        "new": "Huyền Giáp Thạch"
    },
    {
        "path": "materials[14].name",
        "old": "Côn Lôn Tuyết Liên",
        "new": "Băng Sơn Tuyết Liên"
    }
]

# 3. sects.patch.json
sects_patch = [
    {
        "path": "sects[0].tamPhap",
        "old": "Kim Cang Thiết Cốt Quyết",
        "new": "Kim Cang Thiết Cốt"
    },
    {
        "path": "sects[2].tamPhap",
        "old": "Liệt Diễm Tâm Hỏa Quyết",
        "new": "Liệt Diễm Tâm Hỏa"
    },
    {
        "path": "sects[4].tamPhap",
        "old": "Hàn Sơn Băng Tâm Quyết",
        "new": "Hàn Sơn Băng Tâm"
    }
]

# 4. shop.patch.json
shop_patch = [
    {
        "path": "reputationOffers[1].name",
        "old": "Rương Chất Dẫn Ngẫu Nhiên",
        "new": "Rương Chất Dẫn"
    },
    {
        "path": "reputationOffers[2].name",
        "old": "Rương Nguyên Liệu Bậc 3",
        "new": "Rương Liệu Bậc 3"
    }
]

# 5. chapters.patch.json
chapters_patch = [
    {
        "path": "chapters[0].summary",
        "old": "Thiếu niên hiệp khách mang theo tàn nang gia truyền đặt chân đến bến đò Thanh Khê mùa thu lá rụng, tao ngộ Lão Tiều Phu và đối đầu thủy phỉ Ba Hổ Trại.",
        "new": "Thiếu niên hiệp khách mang theo tàn nang gia truyền đặt chân đến bến đò Thanh Khê mùa thu lá rụng, tao ngộ Lục Tiều Phu và đối đầu thủy phỉ Ba Hổ Trại."
    },
    {
        "path": "chapters[0].stages[0].enemy.name",
        "old": "Thảo Khấu Tiên Phong",
        "new": "Thảo Khấu Phong"
    },
    {
        "path": "chapters[0].stages[3].enemy.name",
        "old": "Liệt Diễm Bang Chúng",
        "new": "Liệt Diễm Môn Đồ"
    },
    {
        "path": "chapters[0].stages[4].enemy.name",
        "old": "Phong Lôi Khoái Đao Phong Vũ",
        "new": "Khoái Đao Phong Vũ"
    },
    {
        "path": "chapters[0].stages[4].dialogue.afterLoss[1].text",
        "old": "Đao quá chậm! Cương Khí hộ thân còn non nớt lắm, luyện lại đi!",
        "new": "Đao quá chậm! Cương Khí còn non nớt lắm, luyện lại đi!"
    },
    {
        "path": "chapters[0].stages[5].enemy.name",
        "old": "Ám Hạng Thích Khách",
        "new": "Ám Hạng Sát Thủ"
    },
    {
        "path": "chapters[0].stages[6].enemy.name",
        "old": "Liệt Diễm Bang Tinh Anh",
        "new": "Thiết Bì Trư Thú"
    },
    {
        "path": "chapters[0].stages[6].dialogue.afterLoss[1].text",
        "old": "Sức trâu húc của nó quá hung hãn. Đừng đối đầu trực diện khi nó tích nộ.",
        "new": "Sức húc của nó quá hung hãn. Đừng đối đầu trực diện khi nó tụ khí."
    },
    {
        "path": "chapters[0].stages[7].enemy.name",
        "old": "Liệt Diễm Bang Hương Chủ",
        "new": "Liệt Diễm Đầu Mục"
    },
    {
        "path": "chapters[0].stages[8].name",
        "old": "Ba Hổ Trại Tiền Doanh",
        "new": "Tiền Doanh Ba Hổ"
    },
    {
        "path": "chapters[0].stages[8].enemy.name",
        "old": "Ba Hổ Tiên Phong Vệ",
        "new": "Ba Hổ Tiên Phong"
    },
    {
        "path": "chapters[0].stages[9].name",
        "old": "Thủy Hỏa Phân Tranh Bến Bạc",
        "new": "Bến Bạc Phân Tranh"
    },
    {
        "path": "chapters[1].stages[3].dialogue.before[1].text",
        "old": "Hắn dùng Dưỡng Sinh Quyết kết hợp roi gai độc, hồi phục sinh lực liên tục.",
        "new": "Hắn dùng Dưỡng Sinh Quyết kết hợp roi gai độc, hồi Khí Huyết liên tục."
    },
    {
        "path": "chapters[1].stages[4].enemy.name",
        "old": "Bách Thảo Nghịch Đồ",
        "new": "Bách Thảo Phản Đồ"
    },
    {
        "path": "chapters[1].stages[5].dialogue.afterWin[0].text",
        "old": "Hùng đan đã vào tay. Sinh lực dồi dào, giải trừ được nhiều chướng độc.",
        "new": "Hùng đan đã vào tay. Khí huyết dồi dào, giải trừ được nhiều chướng độc."
    },
    {
        "path": "chapters[1].stages[6].enemy.name",
        "old": "U Minh Ám Sát Thí Tử",
        "new": "U Minh Thí Tử"
    },
    {
        "path": "chapters[1].stages[7].enemy.name",
        "old": "Huyền Độc Trưởng Lão",
        "new": "Huyền Độc Lão"
    },
    {
        "path": "chapters[2].stages[0].name",
        "old": "Tuyết Dốc Băng Khẩu",
        "new": "Tuyết Dốc Băng Môn"
    },
    {
        "path": "chapters[2].stages[0].enemy.name",
        "old": "Hàn Phong Tuần Tuyết Giả",
        "new": "Hàn Phong Tuần Sĩ"
    },
    {
        "path": "chapters[2].stages[1].name",
        "old": "Phong Nhận Trầm Khê",
        "new": "Phong Nhận Khê"
    },
    {
        "path": "chapters[2].stages[2].name",
        "old": "Tuyết Lĩnh Hàn Phong",
        "new": "Tuyết Lĩnh Phong"
    },
    {
        "path": "chapters[2].stages[2].enemy.name",
        "old": "Tuyết Lĩnh Ngân Lang",
        "new": "Tuyết Ngân Lang"
    },
    {
        "path": "chapters[2].stages[3].enemy.name",
        "old": "Kim Cang Hộ Giới Tăng",
        "new": "Kim Cang Hộ Tăng"
    },
    {
        "path": "chapters[2].stages[5].enemy.name",
        "old": "Băng Nguyên Bạch Hổ",
        "new": "Băng Bạch Hổ"
    },
    {
        "path": "chapters[2].stages[6].name",
        "old": "Hàn Tinh Thạch Thất",
        "new": "Hàn Thạch Thất"
    },
    {
        "path": "chapters[2].stages[6].enemy.name",
        "old": "U Minh Chấp Pháp Sứ",
        "new": "U Minh Chấp Pháp"
    },
    {
        "path": "chapters[2].stages[6].dialogue.afterLoss[1].text",
        "old": "Hắn hút cạn sinh lực để hồi phục! Phải dồn sát thương kết liễu nhanh.",
        "new": "Hắn hút cạn Khí Huyết để hồi phục! Phải dồn sát thương kết liễu nhanh."
    },
    {
        "path": "chapters[2].stages[8].name",
        "old": "Vọng Nguyệt Băng Đỉnh",
        "new": "Vọng Nguyệt Đỉnh"
    },
    {
        "path": "chapters[2].stages[8].enemy.name",
        "old": "U Minh Tuyệt Mệnh Kiếm Vệ",
        "new": "Tuyệt Mệnh Kiếm Vệ"
    },
    {
        "path": "chapters[2].stages[9].name",
        "old": "Phong Thiền Băng Đài",
        "new": "Phong Thiền Đài"
    },
    {
        "path": "chapters[2].stages[9].enemy.name",
        "old": "U Minh Tả Sứ Dạ Sát Thiên",
        "new": "Dạ Sát Thiên"
    },
    {
        "path": "chapters[2].stages[9].description",
        "old": "Đỉnh tuyết sơn ngút ngàn mây gió, U Minh Tả Sứ Dạ Sát Thiên bày trận pháp luyện hóa Thần Nang.",
        "new": "Đỉnh tuyết ngút ngàn mây gió, Dạ Sát Thiên bày trận pháp luyện hóa Thần Nang."
    }
]

files = {
    'tools/gemini/out/text-audit/items.patch.json': items_patch,
    'tools/gemini/out/text-audit/materials.patch.json': materials_patch,
    'tools/gemini/out/text-audit/sects.patch.json': sects_patch,
    'tools/gemini/out/text-audit/shop.patch.json': shop_patch,
    'tools/gemini/out/text-audit/chapters.patch.json': chapters_patch
}

for path, content in files.items():
    with open(path, 'w', encoding='utf-8') as f:
        json.dump(content, f, ensure_ascii=False, indent=2)
    print(f"Wrote {path} with {len(content)} entries.")
