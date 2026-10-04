# Tổng quan thiết kế (GDD)

## Ý tưởng

Auto-battler xếp túi đồ phong cách võ hiệp. Người chơi sở hữu một **Hiệp Khách** và một **Bao Phục** dạng lưới ô. Họ sắp xếp binh khí, hộ cụ, công pháp, đan dược, kỳ trân vào túi; khi vào trận, các món tự kích hoạt theo thời gian và hai bên tự đánh nhau. Chiến thuật nằm ở việc chọn đồ, cường hóa và **sắp xếp vị trí** (hiệu ứng liền kề, bộ môn phái).

Tham khảo thể loại: Backpack Battles. Không sao chép tên hay hình ảnh của game tham chiếu.

## Vòng lặp chính

```
Hành Tẩu Giang Hồ (PvE)
  → bạc, nguyên liệu, mảnh binh khí, kinh nghiệm
  → cường hóa vật phẩm (Trọng), mở thêm ô Bao Phục, nâng Hiệp Khách
  → leo Luận Kiếm Đài (PvP)
  → đồ Chí Tôn, trang phục, danh hiệu
```

## Chế độ chơi

### Hành Tẩu Giang Hồ (PvE)
- Chia theo **Hồi** (chương), mỗi Hồi khoảng 10 ải, ải cuối là boss.
- Đội hình địch thiết kế sẵn.
- Phần thưởng: bạc, kinh nghiệm cho Hiệp Khách, nguyên liệu cường hóa, mảnh để mở khóa vật phẩm trong Vạn Bảo Phổ.
- Có giới hạn **Thể Lực** mỗi ngày. Chấm 1–3 sao theo lượng khí huyết còn lại.

### Luận Kiếm Đài (PvP)
- Đấu **bất đồng bộ** với ghost (bản lưu đội hình) của người chơi khác.
- Điểm xếp hạng kiểu ELO/MMR, chia bậc. Mỗi mùa (*Kỳ Luận Kiếm*) khoảng 4 tuần.
- Cuối mùa trao đồ Chí Tôn, trang phục, khung avatar theo thứ hạng. Hạng nhất nhận danh hiệu *Thiên Hạ Đệ Nhất*.

## Tiến trình (giữ lâu dài)

| Thứ giữ lại | Cách tăng |
|---|---|
| Vật phẩm | Nhận từ PvE, Thương Hội, PvP; cường hóa bằng tài nguyên PvE từ Nhất Trọng tới Cửu Trọng, mốc Trọng 3 / 6 / 9 là **Đột Phá** mở thêm hiệu ứng ([items.md](items.md#đột-phá)) |
| Bao Phục | Mở thêm ô theo cảnh giới của Hiệp Khách, có mức tối đa chung |
| Hiệp Khách | Chọn môn phái; lên cấp làm **Tâm Pháp** của phái mạnh dần (mốc 1 / 3 / 6 / 9). Dùng được mọi binh khí, nhưng Tâm Pháp chỉ buff các hệ binh khí của phái ([items.md](items.md#môn-phái-và-tâm-pháp)) |
| Vạn Bảo Phổ | Mở khóa vật phẩm |
| Trang phục, danh hiệu | Phần thưởng PvP |

## PvP công bằng

Vì đồ giữ lâu dài (quyết định #4), cần các biện pháp sau:
1. **Kích thước Bao Phục là giới hạn cứng**, có mức tối đa như nhau cho mọi người.
2. **Ghép trận theo điểm hạng và khoảng Chiến Uy** (chênh không quá ±15%).
3. **Cường hóa có lợi ích giảm dần** theo mỗi Trọng.
4. **Chia bảng xếp hạng theo cảnh giới** để người mới không phải đua với người chơi lâu năm.
5. Đồ Chí Tôn mạnh nhờ **cơ chế độc đáo**, không nhờ chỉ số vượt trội.
6. **Khắc chế giữa các build là có chủ đích** (quyết định #13). Đối thủ PvP là **ngẫu nhiên** trong khoảng ghép trận, người chơi không biết trước và không đổi túi theo đối thủ. Build tốt là build thắng được nhiều loại đối thủ, không phải build khắc một đối thủ cụ thể.

## Tiền tệ (dự kiến)
- **Bạc:** tiền thường, từ PvE.
- **Nguyên Bảo:** tiền nạp.
- **Danh Vọng:** tiền PvP, đổi đồ ở Luận Kiếm Đài.
