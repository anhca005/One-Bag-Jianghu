# Hiệu chỉnh Lực Chiến với battle engine

## Phương pháp

Sinh túi từ `bags_for_stage(1..30)`, biến thể đổi món cùng loại, ±1 phẩm/Trọng, thêm/bớt món và ±2 cấp; ghép các ải lân cận bằng seed cố định. Fit dùng 400 cặp × 20 trận; validation mới dùng 80 × 20.
Fit mô hình `sigmoid(k × ln(LC_A/LC_B))` bằng L-BFGS-B và log-loss nhị thức. Các bước hệ số được tham số hóa không âm; base có prior chỉ số và regularization về trung bình loại để giữ món hiếm hợp lý.
LC chỉ để hiển thị, không đi vào engine. `recommendedFactor=1.1` nghĩa là giao diện đề nghị 110% LC quái.
Sect và Tâm Pháp bị bỏ qua đúng phạm vi v1.

## Hệ số đã fit

- `levelBase=4.72`, `levelPoints=0.01`, `k=30.6797`.
- Trọng: T1=1, T2=1.07, T3=1.31, T4=1.5, T5=1.52, T6=2.07, T7=2.12, T8=2.17, T9=2.22.
- Phẩm: common=1, uncommon=1.01, rare=1.02, epic=1.2, legendary=1.53, supreme=1.63.

## Base cao nhất / thấp nhất

| Hạng | Cao nhất | Base | Thấp nhất | Base |
|---:|---|---:|---|---:|
| 1 | Long Lân Giáp (`long_lan_giap`) | 60.35 | Cuồng Hỏa Đan (`cuong_hoa_dan`) | 4.80 |
| 2 | Ô Tàm Y (`o_tam_y`) | 57.45 | Huyết Ngọc Thạch (`huyet_ngoc_thach`) | 4.31 |
| 3 | Âm Dương Ngọc (`am_duong_ngoc`) | 57.25 | Kim Sáng Dược (`kim_sang_duoc`) | 4.10 |
| 4 | U Minh Quỷ Trảo (`u_minh_quy_trao`) | 55.15 | Quy Nguyên Quyết (`quy_nguyen_quyet`) | 4.07 |
| 5 | Liệt Diễm Đao (`liet_diem_dao`) | 53.93 | U Minh Bí Lục (`u_minh_bi_luc`) | 4.00 |
| 6 | Băng Phách Thương (`bang_phach_thuong`) | 53.86 | Phi Yến Quyết (`phi_yen_quyet`) | 2.80 |
| 7 | Chấn Nhạc Chùy (`chan_nhac_chuy`) | 53.72 | Kiếm Tông Tâm Yếu (`kiem_tong_tam_yeu`) | 2.35 |
| 8 | Bích Thủy Cầm (`bich_thuy_cam`) | 50.48 | Vạn Độc Đỉnh (`van_doc_dinh`) | 2.32 |
| 9 | Phục Hổ Côn (`phuc_ho_con`) | 49.77 | Mãng Xà Độc Túi (`man_xa_doc_tui`) | 1.23 |
| 10 | Cương Trư Bì (`heo_rung_bap_giap`) | 46.54 | Băng Tàm Linh Châu (`bang_tam_linh_chau`) | 0.87 |

## Validation giữ lại

Accuracy hướng LC cao hơn: **89.6%** (77 cặp không hòa LC); log-loss: **0.2829**.

| Tỷ lệ LC A/B | n | Dự đoán | Thực tế |
|---|---:|---:|---:|
| ≤0.8 | 9 | 0.0% | 0.0% |
| 0.8–0.9 | 13 | 1.1% | 2.1% |
| 0.9–1.0 | 19 | 29.0% | 29.1% |
| 1.0–1.1 | 19 | 74.1% | 81.8% |
| 1.1–1.25 | 8 | 98.6% | 88.1% |
| ≥1.25 | 12 | 100.0% | 100.0% |

## Đường cong Lực Chiến

| Ải | Cấp | Không mua | Mua khôn | Ải | Cấp | Không mua | Mua khôn |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 1 | 100 | 167 | 16 | 15 | 325 | 374 |
| 2 | 2 | 109 | 229 | 17 | 16 | 347 | 347 |
| 3 | 3 | 113 | 257 | 18 | 17 | 342 | 342 |
| 4 | 5 | 135 | 236 | 19 | 17 | 338 | 338 |
| 5 | 6 | 144 | 239 | 20 | 18 | 397 | 397 |
| 6 | 7 | 168 | 227 | 21 | 18 | 357 | 357 |
| 7 | 8 | 180 | 243 | 22 | 19 | 361 | 361 |
| 8 | 9 | 193 | 240 | 23 | 19 | 366 | 366 |
| 9 | 10 | 205 | 236 | 24 | 20 | 377 | 377 |
| 10 | 11 | 210 | 218 | 25 | 20 | 360 | 360 |
| 11 | 12 | 211 | 249 | 26 | 21 | 349 | 349 |
| 12 | 13 | 223 | 254 | 27 | 21 | 348 | 348 |
| 13 | 13 | 227 | 227 | 28 | 22 | 346 | 346 |
| 14 | 14 | 251 | 323 | 29 | 22 | 359 | 359 |
| 15 | 15 | 291 | 305 | 30 | 23 | 333 | 333 |

## Tái lập (PowerShell, từ repo root)

```powershell
py tools/balance/power.py --calibrate --matchups 400 --runs 20
py tools/lint/check_style.py tools/balance/power.py
py tools/balance/power.py path/to/loadout.json
```
