# Quy tắc viết code (C# Unity và Java)

Người dùng phải đọc và tự sửa được code, nên code phải **dễ đọc trước, ngắn sau**. Mọi task giao Codex đều phải theo file này. Claude kiểm bằng `tools/lint/check_style.py` trước khi nhận.

## Bắt buộc
1. **Mỗi dòng một câu lệnh.** Không viết `a = 1; b = 2; c();` trên cùng một dòng.
2. **Dòng tối đa 120 ký tự.** Lệnh gọi dài thì xuống dòng theo tham số.
3. **Ngoặc nhọn theo kiểu Allman** (mặc định của Unity/C#): `{` nằm trên dòng riêng. Không viết `if`, `for`, `foreach`, `while` có thân trên cùng một dòng.
   - **Ngoại lệ:** `return`, `continue`, `break` hoặc `throw` một lệnh đứng sau `if` ngắn, gói gọn trong 120 ký tự.
4. **Không lồng lambda dài trong đối số.** Tách ra hàm có tên.
5. **Builder giao diện:**
   - mỗi phần tử UI tạo bằng một khối vài dòng, kèm comment ngắn nói nó là gì (ví dụ `// Khung lời thoại: tranh cuộn 3:1`);
   - các số bố cục (anchor, kích thước) đặt tên rõ, hoặc gom thành hằng số ở đầu file nếu dùng lại.
6. **Tên rõ nghĩa:**
   - không đặt biến 1 chữ cái, trừ chỉ số vòng lặp `i/j/x/y`;
   - không dùng `c`, `p`, `s` cho catalog, profile, stage.
7. **Hàm public có `/// <summary>` ngắn** nếu tên chưa đủ nói nó làm gì.
8. **Test:**
   - mỗi test chia ba khối `// Arrange`, `// Act`, `// Assert`, hoặc tách bằng dòng trống;
   - không nhồi cả test vào một dòng.

## Nên
- Mỗi file một class chính, dưới khoảng 300 dòng. File quá dài thì tách theo trách nhiệm, ví dụ builder theo từng popup.
- Comment giải thích **vì sao**, không nhắc lại code làm **gì**.
- Code mới viết giống code xung quanh đã được dọn sạch, không bắt chước đoạn chưa dọn.

## Kiểm tra
```
py tools/lint/check_style.py <đường dẫn hoặc thư mục...>
```
Lệnh này báo các dòng dài hơn 120 ký tự, các dòng có từ 2 câu lệnh trở lên, và `if`/`for` viết cả thân trên một dòng. Task Codex chỉ được nhận khi các file nó tạo hoặc sửa không còn lỗi nào.
