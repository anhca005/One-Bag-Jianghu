# Máy dò PvE v2 — Codex, 2026-10-04

## Kết quả

Chạy engine đã biên dịch sẵn, 20 trận/probe và 50 trận/xác nhận; hòa tính nửa trận thắng.
Có **1/30** ải trong dải. `shop-smart` chỉ là số đo tham khảo.

Thang sức mạnh gồm đủ 54 cặp `(Trọng, phẩm)`. Vì hai hệ số cùng nhân các chỉ số vật phẩm,
sức mạnh dùng để sắp là `tierMultiplier × rarityMultiplier`; hòa được sắp ổn định theo Trọng rồi phẩm.
Các mốc Đột Phá làm kết quả thực tế không đơn điệu: nhiều cặp kề nhau nhảy từ 100% xuống 0%.

| Ải | Loại | Cấp | Người món/VK | Địch món/VK | Địch | Không shop | Shop-smart | Giây |
|---:|---|---:|---:|---:|---|---:|---:|---:|
| 1 | thường | 1 | 2/1 | 2/2 | T2 uncommon | 84% | 100% | 19,0 |
| 2 | thường | 2 | 3/1 | 3/2 | T2 uncommon | 91% | 100% | 22,6 |
| 3 | thường | 3 | 4/1 | 4/2 | T2 uncommon | 1% | 100% | 29,8 |
| 4 | thường | 5 | 5/2 | 5/2 | T2 uncommon | 0% | 100% | 22,3 |
| 5 | tinh anh | 6 | 6/2 | 7/2 | T2 uncommon | 0% | 44% | 15,4 |
| 6 | thường | 7 | 7/2 | 7/2 | T2 uncommon | 0% | 100% | 20,8 |
| 7 | thường | 8 | 8/3 | 8/3 | T2 uncommon | 0% | 2% | 11,2 |
| 8 | thường | 9 | 8/2 | 8/2 | T2 uncommon | 0% | 100% | 29,7 |
| 9 | thường | 10 | 9/2 | 9/2 | T2 uncommon | 2% | 94% | 32,1 |
| 10 | Thủ Lĩnh | 11 | 10/2 | 11/2 | T2 uncommon | 0% | 28% | 23,7 |
| 11 | thường | 12 | 10/3 | 8/3 | T1 common | 70% | 100% | 19,0 |
| 12 | thường | 13 | 11/3 | 7/3 | T1 common | 84% | 100% | 30,0 |
| 13 | thường | 13 | 12/3 | 10/3 | T2 uncommon | 90% | 90% | 18,8 |
| 14 | thường | 14 | 12/3 | 12/3 | T1 rare | 100% | 100% | 16,4 |
| 15 | tinh anh | 15 | 12/3 | 9/3 | T1 rare | 0% | 35% | 31,2 |
| 16 | thường | 15 | 12/3 | 8/3 | T4 rare | 91% | 100% | 35,4 |
| 17 | thường | 16 | 12/3 | 12/3 | T2 epic | 100% | 100% | 9,7 |
| 18 | thường | 17 | 12/3 | 8/3 | T2 epic | 43% | 43% | 52,0 |
| 19 | thường | 17 | 13/3 | 12/3 | T2 epic | 100% | 100% | 11,2 |
| 20 | Thủ Lĩnh | 18 | 10/4 | 11/4 | T2 supreme | 100% | 100% | 10,6 |
| 21 | thường | 18 | 12/3 | 10/3 | T5 supreme | 52% | 52% | 32,8 |
| 22 | thường | 19 | 12/3 | 11/3 | T5 supreme | 40% | 40% | 29,8 |
| 23 | thường | 19 | 13/3 | 8/3 | T5 supreme | 0% | 0% | 38,7 |
| 24 | thường | 20 | 15/3 | 10/3 | T5 supreme | 14% | 14% | 33,9 |
| 25 | tinh anh | 20 | 13/2 | 12/2 | T5 supreme | 0% | 0% | 34,4 |
| 26 | thường | 21 | 13/2 | 13/2 | T5 supreme | 13% | 13% | 54,9 |
| 27 | thường | 21 | 12/3 | 10/3 | T5 supreme | 0% | 0% | 8,0 |
| 28 | thường | 22 | 11/3 | 11/3 | T5 supreme | 0% | 0% | 32,4 |
| 29 | thường | 22 | 8/3 | 8/3 | T5 supreme | 0% | 0% | 11,0 |
| 30 | Thủ Lĩnh | 23 | 8/3 | 9/3 | T5 supreme | 0% | 0% | 24,8 |

## Ngoài dải

- Ngoài dải: **1–10, 12–30**. Chỉ ải 11 trong dải.
- Bước thang gần nhất nhưng hai bậc kề nằm hai phía vách lớn: 1, 12–14, 16–17, 20–21.
- Ràng buộc thứ tự trong Hồi giữ bậc không giảm: 2–10, 15, 18–19, 22–30.
- Fallback đáy thang ở ải 12 bỏ một `o_tam_y`; xác nhận vẫn 84%, ngoài dải.
- Túi không đạt số món mục tiêu vì hình thật và yêu cầu giữ đồ gốc: 11–13, 15–16, 18–19, 21–25, 27.
- Dữ liệu gốc ải 1–4 có 2 binh khí khi người chỉ có 1; giữ toàn bộ đồ gốc nên không thể đồng thời giảm về 1.
- Quá 40 giây: ải 18 (52,0 s), 26 (54,9 s); chỉ gắn cờ, không tune lại.
- `pve_progression.py` không phát hiện lỗi và không cần sửa.

## Món thêm theo ải

1 —; 2 —; 3 `thiet_giap_y`; 4 `o_thiet_giap` ×2; 5 `thanh_phong_kiem`, `phi_yen_quyet` ×2.
6 `da_ho_y` ×2; 7 `liet_diem_dao`, `heo_rung_bap_giap`, `thiet_giap_y`, `phi_yen_quyet` ×2.
8 `liet_diem_dao`, `o_thiet_giap`, `liet_hoa_tam_kinh` ×2; 9 `thanh_phong_kiem`, `phi_yen_quyet` ×4.
10 `liet_diem_dao`, `liet_hoa_tam_kinh` ×4; 11 `bich_thuy_cam`, `da_ho_y` ×2, `u_minh_bi_luc`.
12 `tu_kim_trao`, `o_tam_y` ×2, `u_minh_bi_luc`; fallback bỏ một `o_tam_y`.
13 `kich_doc_chu_nga`, `o_thiet_giap`, `man_xa_doc_tui` ×3.
14 `thanh_dang_tien`, `man_xa_doc_tui` ×6; 15 `bich_thuy_cam`, `thanh_dang_tien`, `nho_sam_y`, `hoi_thien_dan`.
16 `kich_doc_chu_nga`, `heo_rung_bap_giap` ×2; 17 `kich_doc_chu_nga`, `man_xa_doc_tui` ×5.
18 `tu_kim_trao`, `o_tam_y` ×2; 19 `kich_doc_chu_nga`, `hoa_diem_xa_cot` ×2, `man_xa_doc_tui` ×3.
20 `tu_kim_trao` ×3, `man_xa_doc_tui` ×2; 21 `han_ngoc_tieu`, `bang_tam_giap` ×3, `han_bang_chau`.
22 `thanh_phong_kiem`, `bang_tam_giap` ×3, `han_bang_chau`; 23 `soi_nanh_canh`, `heo_rung_bap_giap`, `tuyet_lang_bach_bi`.
24 `thiet_cot_quyen_sao`, `thiet_giap_y` ×2, `han_bang_chau` ×2.
25 `han_ngoc_tieu`, `bang_tam_giap` ×3, `bang_tam_linh_chau` ×2.
26 `thanh_dong_ho_tam_kinh` ×8; 27 `u_minh_quy_trao` ×2, `bach_ngoc_boi` ×2.
28 `han_ngoc_tieu`, `bang_tam_giap` ×3, `han_bang_chau` ×2.
29 `thanh_phong_kiem`, `han_ngoc_tieu`, `phi_yen_quyet`.
30 `han_ngoc_tieu`, `bang_tam_giap` ×2.

## Tái tạo trong PowerShell

```powershell
New-Item -ItemType Directory -Force tools/codex/out/pve-tuner-v2/tables | Out-Null
py tools/balance/pve_progression.py --stages 1-30 | Tee-Object -FilePath tools/codex/out/pve-tuner-v2/tables/progression.txt
py tools/balance/pve_tune.py --stages 1-30 --runs 20 --confirm-runs 50 --out tools/codex/out/pve-tuner-v2/chapters.json | Tee-Object -FilePath tools/codex/out/pve-tuner-v2/tables/tuner.txt
py tools/lint/check_style.py tools/balance/pve_progression.py tools/balance/pve_tune.py | Tee-Object -FilePath tools/codex/out/pve-tuner-v2/tables/style.txt
```
