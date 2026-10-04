# Hành Tẩu Giang Hồ (PvE): kế hoạch

Quyết định #33. Bản ra mắt đầu: **3 Hồi × 10 ải**. Cốt truyện tổng viết cho cả game, nhưng chỉ làm chi tiết 3 Hồi đầu.

## Nguyên tắc

- **Mỗi Hồi có bản sắc riêng:** bối cảnh, cốt truyện, bảng màu giao diện, bản đồ chọn ải, nền trận và kiểu quái.
- **Quái trộn:** phần lớn ải là võ nhân (đạo tặc, môn đồ, sát thủ) dùng vật phẩm có sẵn. Một số ải và boss là quái thú hoặc yêu vật, có bộ vật phẩm riêng theo chủ đề.
  - **Vật phẩm của quái dùng chung với người chơi:** chúng nằm trong `items.json` như mọi vật phẩm khác, rơi ra từ ải và người chơi trang bị được.
  - Phải cân bằng cùng bộ vật phẩm chung. Không có cơ chế riêng ngoài các cơ chế đã duyệt (#32).
- **Kể chuyện bằng hội thoại trước và sau ải:** hộp thoại có chân dung và tên nhân vật. Có thể có lời thoại riêng khi thắng và khi thua.
- **Độ khó tăng dần** qua Trọng, phẩm và Tâm Pháp của địch, cùng độ khó của đội hình. Mỗi Hồi có 1 ải tinh anh (ải 5) và 1 boss (ải 10).
- **Tiến trình:**
  - Thắng ải N mới mở ải N+1. Hồi sau mở khi đã hạ boss Hồi trước.
  - Chấm sao theo Khí Huyết còn lại: ≥ 70% được 3 sao, ≥ 40% được 2 sao, thắng được 1 sao.
  - Mỗi ải tốn **Thể Lực**, hồi theo thời gian, có giới hạn mỗi ngày (theo GDD).
- **Phần thưởng:**
  - Lần đầu thắng: nhiều Bạc, nguyên liệu và một vật phẩm cố định.
  - Đánh lại: thắng nhận **15%** thưởng lần đầu của chính ải đó (Bạc, EXP, nguyên liệu; phần lẻ nguyên liệu thành tỉ lệ rơi), không trả lại vật phẩm, đồ rơi giữ nguyên. Tỉ lệ ở `repeatRewardPercent` trong `chapters.json` (quyết định #47).
  - Boss có tỉ lệ rơi vật phẩm của quái.

## Định dạng dữ liệu (chung cho Gemini và Codex)

`Resources/Data/Pve/chapters.json`:

```json
{
  "characters": [
    { "id": "hero", "name": "Hiệp Khách", "portrait": "hero_portrait" },
    { "id": "lao_tieu_phu", "name": "...", "title": "...", "portrait": "npc_lao_tieu_phu" }
  ],
  "chapters": [
    {
      "id": "hoi_1", "index": 1, "title": "Hồi 1", "name": "...", "summary": "...",
      "theme": {
        "mapBackground": "map_hoi_1", "battleBackground": "bg_battle_hoi_1",
        "accent": "#8B2E1F", "paper": "#EFE4CC", "ink": "#2B2420",
        "nodeIcon": "node_hoi_1", "bossNodeIcon": "node_boss_hoi_1"
      },
      "unlock": { "afterChapter": null },
      "stages": [
        {
          "id": "hoi_1_01", "index": 1, "name": "...", "kind": "normal",
          "mapPos": [0.12, 0.70], "description": "...",
          "staminaCost": 6,
          "enemy": {
            "name": "...", "characterId": "...", "portrait": "enemy_...",
            "sect": "liet_diem", "tamPhapLevel": 1, "hp": 1000,
            "items": [ { "id": "...", "x": 0, "y": 0, "tier": 1, "rarity": "common" } ]
          },
          "rewards": {
            "firstClear": { "silver": 200, "heroExp": 60, "materials": { "han_thiet": 3 }, "items": [ { "id": "...", "tier": 1, "rarity": "common" } ] },
            "repeat": "default",
            "drops": [ { "id": "...", "chancePermille": 50, "rarity": "uncommon" } ]
          },
          "dialogue": {
            "before": [ { "speaker": "lao_tieu_phu", "text": "..." } ],
            "afterWin": [ { "speaker": "hero", "text": "..." } ],
            "afterLoss": [ { "speaker": "lao_tieu_phu", "text": "..." } ]
          }
        }
      ]
    }
  ]
}
```

- `kind` nhận một trong ba giá trị `normal`, `elite`, `boss`. `mapPos` là toạ độ 0–1 trên tranh bản đồ, gốc ở trên bên trái.
- Mọi hình được tham chiếu bằng **tên file** (không đuôi). Chưa có hình thì game dùng hình dự phòng: `bg_battle`, `hero_idle`, khung trơn.
- Vật phẩm của quái nằm trong `items.json` như vật phẩm thường, không thêm trường mới (engine từ chối trường lạ). Danh sách id vật phẩm quái ghi ở `chapters.json` → `"monsterItems": [...]`, để Thương Hội không bán chúng: chỉ kiếm được bằng cách đánh ải.

## Phân công

| Ai | Việc |
|---|---|
| Gemini | Cốt truyện tổng; 3 Hồi gồm chủ đề, bảng màu, nhân vật, 30 ải (địch, hội thoại, phần thưởng, đường cong độ khó); 10–15 vật phẩm quái dùng chung. Lệnh: `tools/gemini/task-pve-content.md` |
| Codex | Hệ thống PvE: nạp `chapters.json`, màn bản đồ chọn ải theo theme từng Hồi, popup hội thoại, tiến trình, sao, Thể Lực, phần thưởng ải, nền trận theo Hồi, Home → bản đồ. Lệnh: `tools/codex/task-pve-system.md` |
| ChatGPT | Sau khi có nội dung Gemini: 3 tranh bản đồ, 3 nền trận, chân dung NPC/boss/quái, icon vật phẩm quái, icon nút ải. Prompt: `docs/art/prompts-pve.md` (Claude viết) |
| Claude | Review, kiểm chứng cân bằng vật phẩm quái và độ khó ải bằng mô phỏng, tích hợp dữ liệu, viết prompt hình |
