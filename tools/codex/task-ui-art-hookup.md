# Task H: hook the finished UI art into scenes & prefabs (visible in the Editor) ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1). Read `docs/decisions.md` #20/#21, `docs/art/prompts-ui.md` (what each image is for), `Assets/_Game/Editor/Building/README.md`, `Editor/ItemIconImporter.cs` (all `Textures2D/**` become Sprites and are indexed by file name in `ItemIconLibrary`).

New art (already imported, keys = file names):
- `Textures2D/UI/`: `bg_login`, `bg_loading`, `bg_home`, `bg_battle` (1536×1024, opaque), `frame_panel`, `button_primary`, `button_secondary`, `progress_bar`, `seal_title`, `hero_portrait_frame`, `plaque_label`, `icon_lock`, `badge_new`, `hero_idle`, `frame_tooltip` (transparent; sizes vary, e.g. buttons 2172×724).
- `Textures2D/Status/status_{burn,poison,bleed,internal_injury,slow,seal,stun,haste,charge,regen,heal}` (transparent, shown at ~32 px).

## Problem
Home currently assigns `bg_home`/`hero_idle`/… at runtime in `HomeController`, so the scene in the Editor shows flat placeholders. Decision #20: static art must be **serialized in the scene/prefab** (visible when opening it). Runtime lookup stays only for dynamic content (item icons, VFX, status icons chosen per event).

## Hard rules
- Modify under `One-Bag-Jianghu/Assets/_Game/` only: builders in `Editor/Building/`, `Editor/ItemIconImporter.cs` (import settings only), `Scripts/UI/**`, `Scripts/Battle/Replay/View/FighterPanelView.cs` (+ other replay views only for status icons), tests. Not `Scripts/Engine`, not VFX Lab logic, not `VfxTuning`.
- **Do not move hotspot rects** in `LoginHomeSceneBuilder` (Claude just aligned them to `bg_home`; keep x/y/w/h exactly).
- Builders assign sprites via `AssetDatabase.LoadAssetAtPath<Sprite>` when the file exists, else keep the current placeholder color (builders must still work without art). Claude re-runs all builders with Overwrite.
- English, C# 9, existing style. No git. You cannot run Unity.
- Known pitfalls: CS0165 with `out` inside `&&` (declare first); never block the main thread on async.

## Work
1. **Import settings** (`ItemIconImporter`): for `Textures2D/UI/`, max size 2048, no mipmaps, and **9-slice borders** for `button_primary`, `button_secondary`, `frame_panel`, `frame_tooltip`, `plaque_label`, `progress_bar` (compute sensible borders from the art: measure the ornament width at each end/corner via the alpha channel in the importer or hardcode per key in a small table; document the numbers). Status icons: max size 256.
2. **Common prefabs**: `Button_Primary`/`Button_Secondary` use the button sprites (Sliced), label color readable (paper on vermilion, paper on ink); `Panel_Paper` uses `frame_panel` (Sliced) over the paper color; a new `Tooltip` prefab uses `frame_tooltip`. Keep sizes/layouts working.
3. **Scenes**: Login background `bg_login`; Services/LoadingPanel background `bg_loading` + progress bar frame `progress_bar` (fill stays an Image inside); Home: background, hero (`hero_idle` in HeroZone, preserve aspect, bottom aligned), hotspot plaques (`plaque_label` Sliced), lock `icon_lock`, badge `badge_new`; Gameplay/Replay background `bg_battle` (EnvelopeParent like Home); ResultPopup title seal `seal_title`; any portrait frame slot → `hero_portrait_frame` if a fitting slot exists (else skip and report).
   Remove the runtime art lookup from `HomeController`/`HomeHotspot` once it is serialized (keep a null-safe fallback only if cheap).
4. **Status icons in replay**: where `FighterPanelView` shows statuses as text/colored boxes, show the status icon (`status_<code>` via `ItemIconLibrary`, dynamic → runtime lookup is fine) with the stack/points number overlaid; fall back to the current text if an icon is missing. Codes are the lowercase status ids from the battle log (see glossary `docs/design/glossary.md` "Trạng thái").
5. **Tests**: EditMode — each background Image in Login/Home/Gameplay scenes and the Services prefab has a non-null sprite when the file exists; button prefabs use Sliced sprites; status icon lookup returns a sprite for all 11 codes. Keep all 132 EditMode + 5 PlayMode tests green (update expectations only where the art legitimately changed them).

Finish with: files created/changed, the 9-slice numbers chosen, the exact builder methods (Overwrite, in order) Claude must run, anything unsure.
