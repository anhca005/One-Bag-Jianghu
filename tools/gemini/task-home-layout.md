# Lệnh cho Gemini: thiết kế lại bố cục HomeScene "quanh tranh nền"

Dán toàn bộ phần dưới vào Antigravity.

---

You are the UX/UI designer of "One Bag Jianghu" (2D wuxia backpack auto-battler, PC, 1920×1080, Thủy Mặc ink style).

## Hard rules
- Create ONLY `docs/reviews/2026-09-29-gemini-home-layout.md` (Vietnamese, ≤ 220 lines). Do not modify any other file. Do not run git.
- Game terms exactly as in `docs/design/glossary.md`. No names from Kim Dung novels or other games (your previous spec used "Tiêu Phong", a Kim Dung character — never do that; use "Vân Du" as the sample hero name).

## Read first
`docs/decisions.md` #20 and #21 (new: no sect at account creation, sect unlocks at hero level 10; the bag is arranged outside the battle; GameplayScene has NO preparation phase; empty bag / no weapon → warning text right above the battle button; **Home entries are placed around the background painting, not a single centered column**), `docs/tech/ui-flow.md`, your previous spec `docs/reviews/2026-09-27-gemini-ui-screens.md` (section 4), `docs/art/art-pipeline.md`, `docs/art/prompts-ui.md`, images in `docs/art/reference/`.

## Deliver
1. **Concept**: the Home screen is one full-screen ink landscape painting (e.g. a mountain town / sect courtyard); each feature is a **hotspot on a place in the painting** (building, path, banner, table…) with a small calligraphy label plaque. Explain the metaphor for each: Hành Tẩu Giang Hồ (PvE), Luận Kiếm Đài (PvP), Bao Phục (bag), Tàng Bảo Các, Vạn Bảo Phổ, Thương Hội, Môn Phái (locked until level 10, shows lock + "Cấp 10"), Cài đặt.
2. **Layout on 1920×1080**: ASCII wireframe + table of every element with rect (x, y, w, h), anchor, and behaviour at 16:10 and when the painting is cropped. Keep a clear central/lower zone for the hero figure. Header bar (name, level/EXP bar, currency placeholders) and the **main battle area**: two primary buttons (PvE, PvP) placement + the **warning text slot right above them** ("Bao Phục chưa có binh khí" / "Bao Phục đang trống").
3. **Hotspot states**: normal, hover (ink glow/outline, label enlarges, tooltip with short description), pressed, locked (greyed + lock icon + unlock condition), "new" badge. Keyboard: Esc opens Settings; number keys optional.
4. **Hierarchy** for the Unity scene (names like `Canvas/Background`, `Canvas/Hotspots/Hotspot_Pve`, `Canvas/Header/...`, `Canvas/BattleArea/WarningText`), consistent with decision #20 (everything pre-built in the scene; hotspots are instances of one `Hotspot` prefab: image area + label plaque + lock icon + badge).
5. **Art brief** for ChatGPT: the background painting (size 1536×1024 generated → used at 1920×1080; which regions must stay empty/quiet for UI; where each hotspot place must be drawn so it matches your rects), label plaque, lock icon, badge — each as a ready-to-paste English prompt following the style lock in `docs/art/art-pipeline.md`.
6. Max 3 open questions.
