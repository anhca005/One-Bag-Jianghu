# Task G: HomeScene "around the background" layout with hotspots ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1). Read: `docs/decisions.md` #20, #21 (incl. "Hai cửa vào trận ở Home"), `docs/reviews/2026-09-29-gemini-home-layout.md` (the layout spec — authoritative for rects, states, hierarchy), `Assets/_Game/Editor/Building/README.md`, current `Scripts/UI/Home/HomeController.cs` and `Editor/Building/LoginHomeSceneBuilder.cs`.

## Hard rules
- Modify only: `Scripts/UI/Home/`, `Editor/Building/LoginHomeSceneBuilder.cs` (Home part), new `Prefabs/UI/Home/Hotspot.prefab` + its builder, `Scripts/Core/GameSession.cs` only if a read-only property is missing, `Tests/`. Do not touch Gameplay, Bag, Battle, Engine, popups.
- Everything pre-built in the scene (decision #20). Change the builder; Claude re-runs it with Overwrite. Tell Claude the exact methods.
- English, C# 9, existing style. No git. You cannot run Unity.
- **Known pitfalls:** `bool ok = x != null && x.TryGet(k, out Sprite s);` then using `s` → CS0165 (declare first). Never block the main thread on async work.

## Build
1. **`Hotspot` prefab + `HomeHotspot` component**: transparent hit area (Image alpha 0 but raycast target), `Plaque` (Image + Text label), `LockIcon` + `LockText`, `Badge`. States from the spec: normal, hover (plaque scale 1.06, gold outline, one-line tooltip under the plaque), pressed (−2 px), locked (grey 40%, lock + "Cấp 10", tooltip "Đạt cấp 10 mở khóa gia nhập Môn Phái", click = small shake), badge on/off. Serialized: label, tooltip text, locked flag, required level, badge flag, `UnityEvent onClick`.
2. **HomeScene tree** exactly as the spec section 4, names: `Canvas/Background`, `Canvas/HeroZone`, `Canvas/Header/{HeroName, ExpBar, LevelText, Btn_Settings}` (stamina/silver objects present but inactive — systems not implemented), `Canvas/Hotspots/Hotspot_{Pve,Pvp,Bag,Storage,Codex,Shop,Sect}` at the spec rects/anchors, `Canvas/BattleArea/{WarningText, Btn_Pve, Btn_Pvp}` (**keep these names**).
3. **Behaviour**: Btn_Pve / Btn_Pvp → battle directly (current behaviour, with the bag warning + disabled state). Hotspot_Pve / Hotspot_Pvp → for now the same as the buttons (later: stage select / arena lobby), and they respect the same bag validation (show the warning). Hotspot_Bag → BagPopup. Storage/Codex/Shop → "Sắp ra mắt" MessagePopup. Hotspot_Sect → locked while `GameSession.HeroLevel < 10`. Btn_Settings and Esc → SettingsPopup. Keys 1 = PvE, 2 = PvP, 3 = Bao Phục (Input System).
4. **Art hookup**: images come from `ItemIconLibrary` by key when present, else keep the flat Thủy Mặc placeholder colors: `bg_home` (Background), `hero_idle` (HeroZone), `plaque_label` (plaque, 9-slice if the sprite has borders), `icon_lock`, `badge_new`. Resolve at runtime in `HomeController`/`HomeHotspot` via the library (the art is not drawn yet; the builder must not require it). Make sure the icon library indexes `Textures2D/UI/**` too (check `Editor/ItemIconImporter.cs` — if it only scans some folders, extend it; that file is allowed for this change).
5. **Tests**: update `Tests/EditMode/UI/LoginHomeSceneTests.cs` (expected objects/names, 7 hotspots, battle area) and the PlayMode full flow if names changed; add an EditMode test for hotspot lock state by level.

Finish with: files created/changed, exact builder methods (Overwrite) Claude must run, anything unsure.
