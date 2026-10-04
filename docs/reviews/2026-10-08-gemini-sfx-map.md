# Đánh giá & Thiết kế Hệ thống Âm thanh (SFX Map & Catalog)

> Ngày: 2026-10-08 | Người thực hiện: Sound Designer (Gemini)  
> Tài liệu liên quan: [sfx-plan.md](../../audio/sfx-plan.md), [combat.md](../design/combat.md), [glossary.md](../design/glossary.md)  
> File sinh ra: `tools/gemini/out/audio/sfx-map.json`, `tools/gemini/out/audio/sfx-catalog.json`

---

## 1. Bảng thay đổi danh mục SFX (So với kế hoạch gốc 62 khóa)

Hệ thống đã chuẩn hóa và mở rộng danh mục đạt **79 khóa** (nằm trong giới hạn quy định $\le 80$ khóa).

| Khóa âm thanh | Phân loại | Nhóm | Lý do điều chỉnh (1 dòng) |
|---|---|---|---|
| `use_pill` | Thêm mới | battle_nonweapon | Đan Dược kích hoạt cần tiếng bẻ nang thuốc thảo mộc và hít thở dược lực. |
| `use_manual` | Thêm mới | battle_nonweapon | Công Pháp kích hoạt cần tiếng lật trang cổ tịch và khí kình vận chuyển. |
| `use_relic` | Thêm mới | battle_nonweapon | Kỳ Trân kích hoạt hiệu ứng cần tiếng ngọc bội/pháp bảo khẽ ngân vang. |
| `use_armor` | Thêm mới | battle_nonweapon | Hộ Cụ kích hoạt bị động/phản đòn cần tiếng giáp vảy đanh gọn siết chặt. |
| `atk_monster` | Thêm mới | battle_pve | Quái thú PvE (sói, heo rừng, gấu, quái) cần tiếng vuốt cào nanh cắn dã tính. |
| `boss_intro` | Thêm mới | battle_pve | Boss giáng lâm đầu trận cần tiếng đại chiến cổ/la đồng uy áp rền vang. |
| `node_unlock` | Thêm mới | ui | Mở nút ải mới trên bản đồ cần tiếng vệt mực khai lộ kèm chuông đồng nhẹ. |
| `star_rating` | Thêm mới | ui | Kết toán ải nhận 1-3 sao cần tiếng đóng dấu lệnh bài kim loại đanh chắc. |
| 10 `ult_*` đặc trưng | Thêm mới | battle_ultimate_sig | 10 vũ khí biểu tượng nhất game cần tiếng Tuyệt Kỹ chữ ký riêng biệt. |
| `stamina_low` | Loại bỏ / Gộp | ui | Gộp vào `ui_error`; tiếng thở dài người dễ sinh tạp âm dị thường khi AI tạo. |
| `atk_flute` | Sửa prompt | battle_weapon | Đổi sang staccato phi giai điệu tránh AI tự sinh đoạn nhạc sáo sáo rỗng. |
| `atk_zither` | Sửa prompt | battle_weapon | Nhấn mạnh tiếng bựt dây cổ cầm gắt gọn, cấm tiến trình hợp âm giai điệu. |
| `ultimate` | Sửa prompt | battle_skill | Rút ngắn từ 1.0s xuống 0.8s, bỏ đuôi ngân dài tránh đè lẫn nhịp trận đánh. |
| `exhaustion` | Sửa prompt | battle_status | Rút ngắn từ 1.2s xuống 0.8s, lấy tiếng chuông lệnh đục khô thay vì chuông ngân. |
| `merge` | Sửa prompt | ui | Bỏ mô tả `magical shimmer` sci-fi, chuyển thành tiếng ngọc bích chạm nhau tự nhiên. |
| `level_up` | Sửa prompt | ui | Rút ngắn còn 1.0s, dùng chuông đồng nghi lễ đơn lẻ thay chuông leo thang. |

---

## 2. Danh sách 10 Tuyệt Kỹ Đặc Trưng (Signature Ultimates)

Chỉ 10 vũ khí mang tính biểu tượng võ hiệp cao nhất được cấp âm thanh Tuyệt Kỹ riêng biệt (B priority, 1 biến thể, $\le 0.8$ s):

1. **Thanh Phong Kiếm** (`ult_sword_wind`): Kiếm pháp trấn phái khởi đầu, 3 kiếm liên hoàn xé gió kết hợp Vận Khí phiêu dật.
2. **Liệt Diễm Đao** (`ult_blade_fire`): Đao pháp hỏa diễm nòng cốt Liệt Diễm Bang, trảm kích cuồng nộ bốc cháy rực lửa than hồng.
3. **Phục Hổ Côn** (`ult_staff_tiger`): Côn pháp trấn sơn Phật môn, tiếng bổ đập côn gỗ nặng trĩu hòa âm hổ khiếu làm Choáng đối thủ.
4. **Khai Sơn Phủ** (`ult_axe_quake`): Vũ khí Phá Cương mạnh nhất game (45 điểm), tiếng rìu chém nứt toác đá tảng chấn động sơn hà.
5. **Bích Thủy Cầm** (`ult_zither_wave`): Nhạc khí hỗ trợ tối thượng, sóng âm cổ cầm cuộn trào như thủy triều chữa lành và tạo khiên.
6. **Hàn Ngọc Tiêu** (`ult_flute_frost`): Nhạc khí khống chế, tiếng sáo ngọc the thé buốt giá như bão tuyết phong tỏa làm Trì Trệ toàn trường.
7. **Xích Sa Chưởng** (`ult_palm_sand`): Tuyệt kỹ chưởng pháp độc môn xuyên Cương Khí, bão cát sa mạc nóng cháy thiêu đốt ngũ tạng.
8. **Chấn Nhạc Chùy** (`ult_hammer_thunder`): Trọng binh khí nặng nhất, cú nện sấm sét kinh thiên động địa phá khiên và gây Choáng.
9. **Truy Hồn Cung** (`ult_bow_soul`): Cung pháp tầm xa sát thủ, tiễn khí siêu thanh xé toạc hộ thể và Điểm Huyệt đối phương.
10. **U Minh Quỷ Trảo** (`ult_claw_ghost`): Vũ khí tà phái quỷ dị của Boss Dạ Sát Thiên, móng vuốt xé rách hư không ma mị gây Trì Trệ.

*(5 vũ khí có Tuyệt Kỹ còn lại: Phán Quan Bút, Ngọc Cốt Phiến, Băng Phách Thương, Thanh Đằng Tiên, Tử Mẫu Song Đao sử dụng âm Tuyệt Kỹ chung `ultimate` kết hợp `atk_*` để tối ưu chi phí).*

---

## 3. Ngân sách & Phân bổ Độ ưu tiên (Runway Credits)

- **Quy tắc tính:** $1\text{ credit/giây}$. SFX thường gồm $1-2$ biến thể ($\le 0.8$ s); 4 âm thanh nền loop 20 s ($4 \times 20 = 80$ credit).
- **Tài khoản hiện có:** 499 credit miễn phí (Free tier).

| Bộ ưu tiên | Mục tiêu | Số khóa | Biến thể | Credit chính xác (s) | Credit trần (làm tròn s) |
|---|---|---|---|---|---|
| **Bộ A** | Tối thiểu để chơi thử (Core Playtest) | 28 | 49 | 24.7 | 52 |
| **Bộ B** | Trải nghiệm trọn vẹn (Full Combat, Sig Ults, PvE) | 39 | 50 | 33.9 | 60 |
| **Bộ C** | Trau chuốt & 4 Loop môi trường (Polish & Ambience) | 12 | 12 | 85.0 (80s loop) | 93 |
| **TỔNG CỘNG** | **Toàn bộ hệ thống hoàn chỉnh** | **79** | **111** | **143.6 credit** | **205 credit** |

> Dù tính theo giây thực tế (143.6 credit) hay làm tròn trần mỗi lần sinh (205 credit), ngân sách vẫn nằm trọn trong 499 credit hiện có, giữ lại dư địa $294 - 355$ credit để người dùng duyệt và yêu cầu sinh lại các tiếng chưa ưng ý.

---

## 4. Hướng dẫn Hòa âm & Cơ chế Phát lại (Mixing Notes)

### 4.1 Âm lượng tương đối theo nhóm (Relative Volume)
| Kênh / Nhóm | Âm lượng (dB) | Tiêu chuẩn LUFS | Các âm thanh áp dụng |
|---|---|---|---|
| **Impact / Climax** | $0\text{ dB}$ | $-16\text{ LUFS}$ | `hit_crit`, `parry`, 10 `ult_*`, `ultimate`, `battle_start`, `win`, `lose` |
| **Combat Primary** | $-2\text{ dB}$ | $-18\text{ LUFS}$ | `hit_metal`, `hit_blunt`, `atk_*`, `atk_monster`, `boss_intro`, `ko` |
| **Combat Secondary** | $-4\text{ dB}$ | $-20\text{ LUFS}$ | `shield_gain`, `shield_hit`, `heal`, `stun`, `burn_apply`, `poison_apply`, `bleed_apply` |
| **UI Primary** | $-6\text{ dB}$ | $-20\text{ LUFS}$ | `ui_click`, `ui_open`, `ui_close`, `ui_error`, `item_pick`, `item_drop`, `coin`, `reward`, `star_rating`, `node_unlock` |
| **Status / Passive** | $-8\text{ dB}$ | $-24\text{ LUFS}$ | `burn_tick`, `poison_tick`, `bleed_tick`, `use_manual`, `use_relic`, `use_armor`, `buff`, `charge`, `qi_gain` |
| **Micro UI** | $-12\text{ dB}$ | $-28\text{ LUFS}$ | `ui_hover`, `dmg_reduce`, `item_drop_bad`, `dialogue_next` |
| **Ambience** | $-18\text{ dB}$ | $-30\text{ LUFS}$ | `amb_home`, `amb_map_hoi_*` (chạy nền êm dịu, không lấn át nhạc & trận) |

### 4.2 Giới hạn đồng thời (Max Concurrent) & Giãn cách (Debounce)
- **Tối đa 1 nguồn (Single Instance):** `battle_start`, `win`, `lose`, `boss_intro`, `exhaustion`, các `ult_*`, `upgrade`, `merge`, `level_up`.
- **Tối đa 2 nguồn:** `atk_*`, `parry`, `hit_crit`, `heal`, `shield_gain`, `stun`, `use_pill`.
- **Tối đa 3 nguồn:** `hit_metal`, `hit_blunt`, `shield_hit`, `ui_click`, `item_pick`, `item_drop`.
- **Giãn cách cùng khóa (Cooldown):** Tối thiểu $80\text{ ms}$ (nhỏ hơn 1 tick $100\text{ ms}$) để triệt tiêu hiện tượng dồn âm (phasing/clipping) khi nhiều vũ khí cùng xuất chiêu trong 1 tick.

### 4.3 Quy tắc phát ở tốc độ cao (Tua nhanh $\times 4$)
Khi tua nhanh trận đấu ở tốc độ $\times 4$, **tuyệt đối không phát dồn** nhằm chống vỡ tiếng và quá tải AudioSource:
- **CẤM PHÁT (Mute):** Mọi tiếng tick định kỳ (`burn_tick`, `poison_tick`, `bleed_tick`, `exhaustion`); mọi kích hoạt bị động nhỏ (`use_manual`, `use_relic`, `use_armor`, `buff`, `qi_gain`, `dmg_reduce`); các đòn trúng thường không bạo kích (`hit_metal`, `hit_blunt`, `shield_hit`).
- **CHỈ ĐƯỢC PHÁT (Pass-through):** Đòn Tuyệt Kỹ (`ult_*` / `ultimate`), Đỡ đòn thành công (`parry`), Sát thương Hội Tâm (`hit_crit`) và Kết thúc trận (`win` / `lose` / `ko`).

---

## 5. Câu hỏi mở dành cho người dùng

1. **Âm thanh Boss Entrance:** Hiện tại `boss_intro` đang dùng tiếng đại chiến cổ phối hợp thanh la đồng trang nghiêm. Bạn có muốn bổ sung thêm tiếng cười gằn / thoại riêng cho từng Boss cụ thể trong tương lai không?
2. **Biến thể âm thanh Bộ A:** Bộ A dự kiến sinh 2 biến thể cho mỗi tiếng đánh thường để chống nhàm tai. Khi Claude chạy sinh file thực tế trên Runway, bạn muốn sinh cả 2 biến thể ngay từ đầu hay chỉ sinh 1 bản nghe duyệt trước?
3. **Cơ chế âm thanh Hóa Kình (Parry):** Khi Hóa Kình thành công, ta phát tiếng va chạm kim loại nảy lửa (`parry`). Đối thủ bị Định Thân 1 giây ngay sau đó có cần phát kèm tiếng khựng lại (`stun`) không, hay chỉ cần riêng tiếng `parry` là đủ dứt khoát?
