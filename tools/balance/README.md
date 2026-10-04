# Bộ dò cân bằng

`balance.py` chạy bảng đối đầu từ các đội hình mẫu mà không sửa dữ liệu thật.
Python 3.14, JDK 17 và Maven Wrapper của dự án là đủ; không cần cài thư viện Python.

## Lệnh

```bash
py tools/balance/balance.py matrix --tiers 1,3,6,9 --tp 1 --runs 200
py tools/balance/balance.py matrix --loadouts path/to/loadouts --items items.json --sects sects.json
py tools/balance/balance.py matrix --rarity supreme --tiers 1,9 --runs 60
py tools/balance/balance.py whatif --patch changes.json --tiers 1,9 --runs 100
py tools/balance/balance.py tune --config tools/balance/params.example.json --out best.json --runs 60 --rounds 6
py tools/balance/balance.py rarity-step --loadouts server/battle-engine/samples --tier 3 --tp 1 --runs 100
py tools/balance/balance.py vs --a path/to/team-a --b path/to/team-b --runs 100
```

`matrix`, `whatif`, `tune`, `rarity-step`, and `vs` accept `--level N`. It overrides
the `level` stored in every generated loadout; without it, each JSON keeps its own
level. `vs` also accepts `--level-a N` and `--level-b N`; a side-specific option
takes precedence over the shared `--level` for that side.

All commands accept `--battle-config path/to/config.json`. The file is a partial JSON
object over `BattleConfig` defaults. Unknown fields are rejected. For example, a rarity
curve proposed for balancing can be saved as `rarity-config.json`:

```json
{
  "rarityMultiplierPermille": [1000, 1150, 1320, 1520, 1750, 2000]
}
```

Then run `py tools/balance/balance.py rarity-step --loadouts server/battle-engine/samples --tier 3 --tp 1 --runs 100 --battle-config rarity-config.json`.
`rarity-step` compares each loadout against itself at adjacent rarities (draws count as
half a win), prints an average for every step, and also compares Common T9 with Uncommon T1.

`vs` chạy mọi đội hình trong thư mục A với mọi đội hình trong thư mục B và in bảng
tỉ lệ thắng của phía A; hòa tính nửa trận thắng. Khác với `matrix`, lệnh này giữ nguyên
`tier` (Trọng) và `rarity` (phẩm) của từng vật phẩm trong JSON. Có thể chỉ định dữ liệu
qua `--items` và `--sects`; `--runs` là bắt buộc.

`matrix` và `whatif` in một hàng cho mỗi đội hình, một cột cho mỗi cặp Trọng/Tâm Pháp.
Số trong ô là tỉ lệ thắng trung bình (`TB`) của hàng trước mọi đội khác.
Dấu `!` chỉ giá trị ngoài `--low`/`--high` (mặc định 35–65%).
`--tiers` và `--tp` nhận danh sách cách nhau bằng dấu phẩy.

`tune` dò tọa độ từng thông số, chạy các mức Trọng song song và ghi patch tốt nhất vào `--out`.
Hàm mục tiêu là tổng bình phương phần vượt `--high`, cộng phần thiếu `--low` nhân `--w-low`.
File cấu hình có mảng `parameters`; mỗi phần tử gồm `address`, `step`, `min`, `max`, và `start` tùy chọn.
Không có lệnh tự áp dụng kết quả: người thiết kế phải xem và sửa dữ liệu chính bằng tay.

## Định dạng patch và địa chỉ

Patch là object JSON phẳng, ví dụ:

```json
{
  "thiet_cot_quyen_sao.attack": 12,
  "phuc_ho_con.ultimate.cost": 30,
  "sect:kim_cang.levels.1.attackPct": 0.12
}
```

Địa chỉ vật phẩm hỗ trợ `.attack`, `.cooldown`, `.crit`, `.ultimate.cost`,
`.basic.<op hoặc status>` và `.triggers.<op>`.
Hai dạng hiệu ứng sửa trường đầu tiên có mặt trong `value`, `stacks`, `duration` của hiệu ứng khớp đầu tiên.
Địa chỉ môn phái là `sect:<sect_id>.levels.<level>.<field>`, ví dụ `attackPct`.
Tune configurations may also use `config:<field>` for a numeric scalar or
`config:<field>.<index>` for an array element, for example
`config:rarityMultiplierPermille.1`. These candidates are passed to the Java engine as
temporary battle-config overrides and never modify source files.

## Lưu ý

- Biến thể đội hình được tạo trong thư mục tạm, không bao giờ ghi vào `samples/`.
- Cùng dữ liệu và `--runs` cho kết quả tất định vì engine dùng cùng dãy seed.
- Thời gian tăng theo số đội hình² × số mức × `--runs`; một lượt tune gọi nhiều bảng.
- Kết quả có thể quá khớp với các đội hình mẫu; luôn kiểm tra thêm đội hình thực tế và nguồn sát thương.
