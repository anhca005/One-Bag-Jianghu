# Task L: redesign the battle (Gameplay) screen layout ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1). Read `docs/decisions.md` #20, #21 and **#26**, `docs/tech/ui-flow.md`, `Editor/Building/*` (+ README: UI is authored by Editor builders into prefabs/scenes, never built at runtime), `Editor/Building/ReplayPrefabBuilder.cs`, `GameplaySceneBuilder.cs`, `Scripts/Battle/Replay/View/*` (ReplayScreen, FighterPanelView, ReplayItemView, ReplayVfxLayer, ReplayStyle), `Scripts/UI/Gameplay/GameplayController.cs`, the Replay/Gameplay tests.

## Target layout (1920×1080 reference, CanvasScaler as today)
The owner's sketch, decided in #26:

```
┌──────────────────────────── TOP BAR ─────────────────────────────┐
│ (left) stage/opponent name    (centre) battle clock   (right) Thoát │
├──────────────────────────────────────────────────────────────────┤
│  [AVATAR]   [ MY BAG 5×4 ]      ← gap for      [ ENEMY BAG 5×4 ]   [AVATAR] │
│  name        items drawn in       travelling      mirrored side       name  │
│  HP bar      their real cells      strokes →                         HP bar │
│  Cương Khí                                                          Cương Khí│
│  Nội Lực                                                              Nội Lực│
│  status chips (Bỏng/Độc/Chảy Máu/Hồi Phục…)                         chips   │
├──────────────────────────── BOTTOM BAR ──────────────────────────┤
│  Tạm dừng · x1 · x2 · x4          collapsed battle log (last 2–3 lines, click to expand) │
└──────────────────────────────────────────────────────────────────┘
```

- One horizontal band in the middle, left→right: player avatar, player bag, empty gap, enemy bag, enemy avatar. The battle background (`bg_battle`) stays visible behind/around the band; top and bottom bars are thin (≈ 90 px and ≈ 120 px), semi-transparent paper/ink panels.
- **Avatar block** per side: portrait inside `hero_portrait_frame` (player: `hero_idle`; enemy: `hero_idle` mirrored and ink-tinted until enemy art exists — use `ItemIconLibrary` keys, missing art → placeholder), name above, then **Sinh Lực, Cương Khí, Nội Lực** bars (reuse `progress_bar` art) with numbers, then status chips with the status icons (`Textures2D/Status`) and stack counts. Everything fighter-level shows here.
- **Bags**: the real 5×4 grid of each side, items at their cells (existing ReplayItemView: icon, cooldown fill, Tuyệt Kỹ flash, item status labels SEAL/SLOW/HASTE stay on items). Enemy bag mirrored horizontally so both bags face the centre. Size the cells so both bags + avatars fit with a clear centre gap (≈ 300 px or more).
- **VFX anchors**: travelling strokes go from the source item to the **opponent avatar**; hits, bursts, parries and floating numbers play on the **avatar** of the affected side (ReplayVfxLayer already uses the fighter RectTransform as target — point `fighters[side]` / floating text to the avatar portrait, not the whole panel).
- **Top bar**: left = stage/opponent name (GameplayController already knows it or pass it through), centre = battle clock mm:ss from the replay clock, right = "Thoát" button (confirm popup → Home).
- **Bottom bar**: pause/resume + speed x1/x2/x4 (existing controls, restyled with the common button prefabs), and the battle log collapsed to the last 3 lines; clicking it expands a scrollable panel over the band (click again to collapse). Remove the old 30%-height bottom log panel.

## Hard rules
- Modify only: `Editor/Building/ReplayPrefabBuilder.cs`, `Editor/Building/GameplaySceneBuilder.cs`, `Scripts/Battle/Replay/View/**` (not `VfxLab/`, not `VfxTuning.cs`), `Scripts/UI/Gameplay/**`, `Tests/EditMode/Replay/**`, `Tests/EditMode/UI/**` (Replay/Gameplay prefab tests only), `Tests/PlayMode/**` (only if a flow test references renamed objects).
- Keep the public behaviour of ReplayScreen (load a replay, play, events, result callback) so GameplayController, ReplayDemo and the PlayMode flow keep working. Rename/regroup GameObjects freely but update tests.
- English code, Vietnamese UI strings with glossary terms. C# 9, existing style. No git. You cannot run Unity — re-read for compile errors (CS0165: never `out` inside `&&` when the variable is used later; fully qualify `UnityEngine.UI.Image.Type` inside builders).

## Tests
EditMode prefab tests: the new objects exist (TopBar/Clock/ExitButton, BottomBar/SpeedButtons/LogToggle, per side Avatar/Portrait/HpBar/ShieldBar/QiBar/StatusChips/Bag), required serialized references assigned, VFX target of each side is the avatar portrait. Keep all existing tests green.

Finish with: files changed, builder menu items Claude must run (order), anything unsure.
