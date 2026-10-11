# Codex task: draw and hook up the quest UI (tracker + Hành Tẩu Lục), with a running light when done

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.** Paths below are under `Assets/_Game/` unless
they start with `docs/` or `tools/`. Spec of the quest system: `docs/design/quests.md`.

Current code: `Editor/Building/QuestUiBuilder.cs` (menu "Quest UI (Overwrite)"), `Scripts/UI/Home/QuestTracker.cs`,
`Scripts/UI/Popups/QuestLogPopup.cs`. Today the tracker reuses the stretched `hud_panel` and has a "Nhận thưởng"
button; the log is plain text rows.

## Part A: draw the art (use your built-in image generation, `image_gen`)
Style: the same family as `Textures2D/UI/Kit/*` (Thủy Mặc ink, rice paper, dark lacquer, ochre gold, vermilion); look
at `docs/art/reference/01-style-board-thuy-mac.png`, `Kit/hud_panel.png`, `Kit/shop_card_wide.png`,
`Kit/price_tag.png`, `Kit/popup_frame.png` (or the current popup frame). Prompt style: `docs/art/prompts-ui-kit.md`
("Tin mở đầu" + table). **Append the prompts you used as group I (#110+) to `docs/art/prompts-ui-kit.md`.**

Draw each piece **at the proportion it is used** (so the game stretches it whole, no 9-slice), transparent background
(`py tools/art/remove_checker.py` if needed), trimmed with ~2% padding, no text/letters/numbers/seals with characters:

| File (in `Kit/`) | Use, size in game | Notes |
|---|---|---|
| `quest_tracker.png` | Home tracker, 440x128 (about 7:2) | A hanging scroll/plaque: rice-paper face, thin lacquer border, gold corners, a small round slot on the left for an icon, plain face for two text lines |
| `quest_badge_main.png` / `quest_badge_side.png` | Small marker on the tracker and log rows, ~72x30 | Same shape: vermilion lacquer tag (main) / dark ink tag (side), plain center for a 2–3 letter label written by the game |
| `quest_icon_scroll.png` | Icon in the tracker's left slot, ~56x56 | A tied mission scroll |
| `quest_log_row.png` | One quest row in the log, ~820x150 (about 11:2) | Paper strip with thin gold rule, a square slot on the left for the status icon |
| `quest_status_done.png` / `quest_status_active.png` / `quest_status_locked.png` | Status icon in the row slot, ~56x56 | Done: vermilion check seal (no characters); active: gold brush circle; locked: dark padlock |
| `quest_glow_spark.png` | The running light, ~48x48 | A soft golden light spark with a short tail, additive-looking, transparent edges |

## Part B: change the UI
1. **Tracker** (`QuestTracker` + builder):
   - use `quest_tracker.png` (Simple, whole), the scroll icon in its slot, the main/side badge with the label
     "CHÍNH"/"PHỤ";
   - **remove the "Nhận thưởng" button** (and its field/code);
   - clicking the tracker: if the quest is done → open `QuestLogPopup` (claim happens there); otherwise run `goto`
     as today. The small log button stays (or merge it into the body click if it no longer fits the art; keep a way
     to open the log at any time);
   - **when the active quest is done, a light runs around the tracker's border**: a `quest_glow_spark` (plus 3–4
     fading trail copies) travelling clockwise along the rect's edge at a steady speed (one lap about 2.5 s), and
     the frame tinted slightly warmer/brighter with a gentle pulse. It stops and hides when not done. Put it in its
     own component (e.g. `UiBorderRunner`) so it can be reused; unscaled time; no allocations per frame.
2. **Log popup** (`QuestLogPopup` + builder): rows use `quest_log_row.png` with the status icon, badge, title, giver
   name, `text`, objective + progress, and rewards with icons (silver `Textures2D/Pve/icon_bac.png`, EXP
   `Kit/icon_exp`, materials from the material/item icon library the shop and storage use) instead of plain text.
   The active done quest's row shows the **"Nhận thưởng"** button (primary button prefab) and the same running
   light around that row. Locked rows show "???" with the lock icon.
3. Every sprite keeps a fallback when missing. Keep the tracker position (top-left, below the sect chip: anchored
   top-left at (24, -236)).
4. Update/add EditMode tests for: tracker has no claim button; runner active only when done; log row count/state.

## Check
- `py tools/lint/check_style.py <every C# file you changed or created>` until clean; `dotnet build` (new files missing
  from a `.csproj`: build with a temporary copy that includes them, then delete the copy and its `obj/` caches).
- **Do not launch Unity.** No git. Do not touch scenes or any existing `.prefab` (the user runs "Quest UI (Overwrite)").
- Write only: the PNGs above, `docs/art/prompts-ui-kit.md` (append group I), the quest UI C# files, the builder, and
  tests.

## Report (`-o` output)
A table of images (file, pixel size, alpha yes/no, redrawn), files changed, how the running light works, the menu to
run.
