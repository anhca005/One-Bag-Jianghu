# Codex task: story quest system (Hành Tẩu Lục)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.** Paths below are under `Assets/_Game/`.

**The spec is `docs/design/quests.md` — read it fully; it is the source of truth** (rules, JSON format, objective
types, goto targets, Hồi 1 content). Story context: `docs/design/story-bible.md`. Terms: `docs/design/glossary.md`.

## Build
1. **Data** `Resources/Data/quests.json`: the 9 Hồi 1 quests from the spec table. Write each `text` (1–2 sentences,
   in the giver's voice, Vietnamese, wuxia tone, consistent with story-bible Hồi 1 beats; giver mostly
   `lao_tieu_phu`, q1_05 may be `phong_loi`). Gemini will add Hồi 2–5 later in the same file, so the loader must take
   any number of chapters.
2. **Catalog + service** (logic, no UI; follow how `PveCatalog`/`TutorialCatalog` load data and how
   `PveRewardService` pays rewards and adds materials):
   - load/validate quests (unique ids, known objective types, known stage ids, known material ids);
   - profile state in `PlayerProfile` (new JSON fields, old saves must still load): index/id of the active quest,
     claimed ids, and the action-counter baseline taken when a quest becomes active;
   - action counters (`shop_buy`, `item_upgrade`, `item_merge`, `pve_win`) persisted in the profile and incremented
     where those actions succeed (Thương Hội purchase, StoragePopup upgrade, MergeService/StoragePopup merge, PvE win
     in `GameplayController`). State objectives read the profile (`PveRules.IsCleared`, hero level, sect id, Tâm Pháp
     level, loadout count, owned item tiers);
   - `Progress(quest)` → current/target + done; `Claim()` pays silver/heroExp (use the same level-up path PvE
     rewards use) and materials (fail with "Tàng Bảo Các không đủ chỗ." and pay nothing if they do not fit), then
     advances to the next quest; an event/callback when progress or the active quest changes;
   - the objective label text per the spec table (stage names from the PvE catalog).
3. **Home tracker** — a prefab `Resources/UI/Home/QuestTracker.prefab` built by a new builder method with its own menu
   item "One Bag Jianghu/Build/Quest UI (Overwrite)" (do **not** change or rebuild HomeScene; the user hand-edits it).
   `HomeController` instantiates it at runtime under the Home canvas (left side, below the top bar, about 420x120,
   not covering the hotspots — look at the current Home layout in `Logs/Snapshots/HomeScene.png`). Shows: "Hồi N" +
   quest title, objective label + progress (e.g. "Mua 1 món ở Thương Hội  0/1"), a main/side marker, and a
   "Nhận thưởng" button when done (pulsing gently). Clicking the body runs `goto` (open stage map at the stage's
   chapter / Thương Hội / Tàng Bảo Các / Bao Phục / Môn Phái — reuse HomeController's existing open methods). Use the
   UI kit art in `Textures2D/UI/Kit/` (`panel_inset`, `hud_panel`, `button_*`, `icon_*`) with fallbacks. Refreshes when
   the profile changes (after battles, purchases, popups closing).
4. **Popup `QuestLogPopup`** (title "Hành Tẩu Lục"), built in the popup builder style, opened from a small button on
   the tracker: chapters as tabs or sections; each quest row shows status (done/active/locked "???"), title, giver
   name, `text`, rewards with icons; the active one can be claimed here too.
5. **Claim feedback**: play the existing "reward" sfx, show the rewards (reuse `ResultPopup` reward-card style or a
   short MessagePopup) and save the profile.
6. **Tests** (EditMode): catalog validation; each objective type's progress; counters only count after activation;
   state objectives already met complete immediately; claim pays and advances; claim with full Tàng Bảo Các fails
   and pays nothing; old profile JSON without the new fields loads.

## Check
- `py tools/lint/check_style.py <every C# file you changed or created>` until clean; `dotnet build` the generated
  `.csproj` files (new files missing from a `.csproj`: build with a temporary copy that includes them, delete the copy
  and its `obj/` caches afterwards).
- **Do not launch Unity.** No git. Do not touch LoginScene, HomeScene, the loading frame, `Map_hoi_1`, the replay
  builder, or any existing `.prefab`/`.unity` file (new prefabs are created by the user running your menu).
- Do not edit `docs/` (the leader does).

## Report (`-o` output)
Files created/changed, profile fields added, where counters are incremented, the menu(s) the user must run, anything
from the spec you could not do.
