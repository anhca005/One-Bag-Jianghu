import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]

b_a = (ROOT / 'tools/gemini/out/items-v2-balance-1/baseline_a.txt').read_text(encoding='utf-8').strip()
f_a = (ROOT / 'tools/gemini/out/items-v2-balance-1/final_a.txt').read_text(encoding='utf-8').strip()
b_bc = (ROOT / 'tools/gemini/out/items-v2-balance-1/baseline_b_common.txt').read_text(encoding='utf-8').strip()
f_bc = (ROOT / 'tools/gemini/out/items-v2-balance-1/final_b_common.txt').read_text(encoding='utf-8').strip()
b_bl = (ROOT / 'tools/gemini/out/items-v2-balance-1/baseline_b_legendary.txt').read_text(encoding='utf-8').strip()
f_bl = (ROOT / 'tools/gemini/out/items-v2-balance-1/final_b_legendary.txt').read_text(encoding='utf-8').strip()
b_rs = (ROOT / 'tools/gemini/out/items-v2-balance-1/baseline_rarity_step.txt').read_text(encoding='utf-8').strip()
f_rs = (ROOT / 'tools/gemini/out/items-v2-balance-1/final_rarity_step.txt').read_text(encoding='utf-8').strip()

parts = [
    '# Báo Cáo Cân Bằng Đợt 1 Items v2 — Nhất Nang Giang Hồ',
    '**Căn cứ:** Quyết định #13, #25–#32; `docs/design/balance.md`. Patch: `tools/gemini/out/items-v2-balance-1/patch.json`.',
    '## 1. 20 Đội Hình Mẫu v2 (Loadouts)',
    '1. `bich_thuy_duong_sinh.json`: Bích Thủy Am - Dưỡng Sinh Cầm Âm (bich_thuy). Dưỡng Sinh Quyết + Tuế Nguyệt Bảo Y.',
    '2. `han_son_bang_tam.json`: Hàn Sơn Phái - Băng Tâm Bút Cung (han_son). Băng Tâm Giáp + Hàn Băng Châu + Bút + Cung.',
    '3. `huyen_doc.json`: Huyền Độc Môn - Hắc Huyết Ám Khí (huyen_doc). Thất Tinh Tiêu + Tử Kim Trảo + U Minh Bí Lục + Vạn Độc Đỉnh.',
    '4. `huyen_doc_van_doc.json`: Huyền Độc Môn - Vạn Độc Phệ Huyết (huyen_doc). Thất Tinh Tiêu + Tử Kim Trảo + U Minh Bí Lục.',
    '5. `hybrid_han_son_bich_thuy.json`: Hàn Sơn Phái - Hàn Bích Hợp Bích (han_son). Hàn Ngọc Tiêu + Bích Thủy Cầm (Hybrid).',
    '6. `hybrid_kim_cang_thiet_phu.json`: Kim Cang Tự - Kim Cang Thiết Phủ (kim_cang). Côn + Rìu + Bách Luyện Chân Kinh (Hybrid).',
    '7. `hybrid_thai_thanh_liet_diem.json`: Thái Thanh Quán - Thái Thanh Liệt Diễm (thai_thanh). Kiếm + Đao + Thái Thanh Kinh (Hybrid).',
    '8. `hybrid_thiet_phu_liet_diem.json`: Thiết Phủ Bang - Thiết Phủ Liệt Diễm (thiet_phu). Khai Sơn Phủ x2 + Liệt Diễm Đao (Hybrid).',
    '9. `kim_cang.json`: Kim Cang Tự - Hộ Thể Côn Quyền (kim_cang). Côn + Quyền + Thiết Giáp Y + Tử Kim Bát phản kích.',
    '10. `kim_cang_bach_luyen.json`: Kim Cang Tự - Bách Luyện Thuần Quyền (kim_cang). Quyền sáo x4 + Bách Luyện Chân Kinh.',
    '11. `liet_diem.json`: Liệt Diễm Đường - Cuồng Đao Liệt Hỏa (liet_diem). Liệt Diễm Đao + Chu Sa Hồ Lô + Liệt Hỏa Tâm Kinh.',
    '12. `liet_diem_hoa_hai.json`: Liệt Diễm Đường - Hỏa Hải Song Đao (liet_diem). Tử Mẫu Song Đao + Liệt Diễm Đao + Chu Sa Hồ Lô.',
    '13. `new_bich_thuy_whip.json`: Bích Thủy Am - Thanh Đằng Trường Tiên (bich_thuy). Thanh Đằng Tiên x2 + Trầm Hương Lô.',
    '14. `new_han_son_spear.json`: Hàn Sơn Phái - Hàn Băng Thương Pháp (han_son). Băng Phách Thương + Hàn Băng Châu + Băng Tâm Giáp.',
    '15. `new_liet_diem_palm.json`: Liệt Diễm Đường - Xích Sa Chưởng Kình (liet_diem). Xích Sa Chưởng + Chu Sa Hồ Lô + Dạ Hổ Y.',
    '16. `new_thai_thanh_fan.json`: Thái Thanh Quán - Tiêu Dao Phiến Pháp (thai_thanh). Ngọc Cốt Phiến + Bạch Ngọc Bội + Vô Niệm Kinh.',
    '17. `new_thiet_phu_hammer.json`: Thiết Phủ Bang - Chấn Nhạc Chùy Phong (thiet_phu). Chấn Nhạc Chùy + Khai Sơn Phủ + Ô Thiết Giáp.',
    '18. `thai_thanh.json`: Thái Thanh Quán - Kiếm Đạo Hóa Kình (thai_thanh). Thanh Phong Kiếm + Bạch Ngọc Bội + Vô Niệm Kinh.',
    '19. `thai_thanh_kiem_khi.json`: Thái Thanh Quán - Kiếm Khí Xuyên Giáp (thai_thanh). Thanh Phong Kiếm x3 + Kiếm Tông Tâm Yếu.',
    '20. `thiet_phu.json`: Thiết Phủ Bang - Bá Khí Khai Sơn (thiet_phu). Khai Sơn Phủ x3 + Ô Thiết Giáp + Dạ Hổ Y.',
    '## 2. Bảng Kết Quả Đối Đầu (Verbatim)',
    '### 2.1. Ma trận A: Baseline vs Final (Tiers 1, 3, 6, 9 x TP 1, 9; Runs 100)',
    '```text\n' + b_a + '\n```',
    '```text\n' + f_a + '\n```',
    '### 2.2. Ma trận B Phẩm Phàm: Baseline vs Final (Common; Tiers 1, 9 x TP 1; Runs 100)',
    '```text\n' + b_bc + '\n```',
    '```text\n' + f_bc + '\n```',
    '### 2.3. Ma trận B Thần Binh: Baseline vs Final (Legendary; Tiers 1, 9 x TP 1; Runs 100)',
    '```text\n' + b_bl + '\n```',
    '```text\n' + f_bl + '\n```',
    '### 2.4. Bậc phẩm chất: Baseline vs Final (Rarity Step; Tier 3, TP 1; Runs 100)',
    '```text\n' + b_rs + '\n```',
    '```text\n' + f_rs + '\n```',
    '## 3. Chẩn Đoán Lệch Điểm & Cơ Chế Lạm Dụng',
    '- **Lặp vô hạn (Infinite Loop):** `van_doc_dinh` kích hoạt `status_applied` gây độc -> tự kích hoạt lặp đệ quy tới kịch trần `depth=8`, kéo theo `u_minh_bi_luc` nạp 0.5s liên tục làm vũ khí xuất chiêu vô tận. Capping: Tách liền kề, hạ nạp `u_minh_bi_luc`.',
    '- **Lỗi schema Hộ Thân (Immortal DR):** `thiet_giap_y` và `duong_sinh_quyet` để `value: 0.02` thay vì `pct`, engine gán 100% giảm thương, chạm trần `maxDmgReducePermille=800` (bất tử). Khắc phục: siết trần Hộ Thân toàn cục `config:maxDmgReducePermille: 400` (40%).',
    '- **Cỗ máy hồi phục Bích Thủy:** `bich_thuy_cam` nhận nạp 1.0s mỗi nhịp Điều Tức. Khắc phục: hạ nạp `triggers.charge` từ 1.0s -> 0.2s, hồi chiêu 4.6s, gọt khiên `duong_sinh_quyet` (8) và `tue_nguyet_bao_y` (10).',
    '- **Rìu chùy Thiết Phủ:** Rìu chùy hồi chiêu dài lép vế T6/T9. Khắc phục: tăng công Khai Sơn Phủ (88), Chấn Nhạc Chùy (82), hồi chiêu 4.8s-5.0s; hạ khiên mở đầu `o_thiet_giap` (160) tránh áp đảo T1.',
    '- **Ám khí Huyền Độc:** `that_tinh_tieu` (công 4) và `tu_kim_trao` (công 7) không phá nổi giáp T1. Khắc phục: nâng công lên 7 và 11, độc cơ bản lên 3.',
    '## 4. Lệnh Tái Hiện & Câu Hỏi Mở',
    '- `whatif --patch tools/gemini/out/items-v2-balance-1/patch.json --items tools/gemini/out/items-v2/items-v2.normalized.json --loadouts tools/gemini/out/items-v2-balance-1/loadouts --tiers 1,3,6,9 --tp 1,9 --runs 100`',
    '- `whatif ... --tiers 1,9 --tp 1 --runs 100 --rarity common` / `--rarity legendary`',
    '- `rarity-step --items tools/gemini/out/items-v2-balance-1/items-v2-balanced.json --loadouts tools/gemini/out/items-v2-balance-1/loadouts --tier 3 --tp 1 --runs 100`',
    '1. Có nên bổ sung `internal_cooldown` (~0.5s) cho trigger `status_applied` để chặn triệt để lặp đệ quy độc/cháy?',
    '2. `Thanh Đồng Hộ Tâm Kính` (Hộ Cụ 1x1) và `Xích Sa Chưởng` (vải quấn tay) có nên quy chuẩn lại thành Kỳ Trân và Bí Kíp ở v3?',
    '3. Kịch Độc ở T9 áp đảo các lối chơi thuần Cương Khí; có nên tăng cường dược phẩm giải độc (`giai_doc_dan`) thành trang bị thường trực cho các phái đối đầu?'
]

doc = '\n'.join(parts)
lines = doc.splitlines()
print('Total lines:', len(lines))
assert len(lines) <= 250, f'Exceeded limit: {len(lines)}'
(ROOT / 'docs/reviews/2026-10-01-gemini-items-v2-balance-1.md').write_text(doc + '\n', encoding='utf-8')
print('Report written successfully!')
