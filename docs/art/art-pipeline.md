# Quy trình làm hình (ChatGPT)

## Nguyên tắc
1. **ChatGPT chỉ vẽ vật thể trên nền trong suốt.** Khung, màu độ hiếm, dấu Trọng do Unity ghép lên để đồng bộ.
2. **Dùng một prompt gốc cố định**, mỗi món chỉ thay phần mô tả.
3. Giữ một **bảng mẫu phong cách**: 5 hình ưng ý nhất, đính kèm làm tham chiếu mỗi lần tạo hình mới.

## Prompt gốc

```
Game item icon, Chinese wuxia style, [MÔ TẢ VẬT THỂ], painterly semi-realistic,
dramatic rim light, subtle [fire/ice/poison] aura, centered, transparent background,
no text, no frame
```

## Quy ước kích thước
- 1 ô lưới = **256 px**. Món 1x3 xuất 256x768, món 2x2 xuất 512x512.
- Vẽ ở độ phân giải gấp 4 rồi thu nhỏ.
- Binh khí dài vẽ nằm chéo.
- PNG nền trong suốt.

## Danh sách hình cho MVP

| Nhóm | Số lượng |
|---|---|
| Icon vật phẩm | ~50 |
| Biểu tượng môn phái | 7 |
| Hiệp Khách | 3 |
| Boss | 6 |
| Icon trạng thái | 12 |
| Nền màn hình | 5 |
| Bộ khung UI (khung độ hiếm, nút, panel) | 1 bộ |

## Lưu ý
Kiểm tra điều khoản sử dụng thương mại của OpenAI trước khi phát hành.
