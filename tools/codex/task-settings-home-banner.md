# Codex task: Settings popup + Home top banner

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu` (Unity 6000.3.9f1, UGUI, Input System).
All game code lives under `Assets/_Game/`. You cannot run Unity; Claude will build, run tests and take snapshots.

**Read first:**
- `docs/tech/code-style.md`. It is mandatory: one statement per line, max 120 columns, no `if`/`for` bodies on the same line.
- `docs/decisions.md` #45. Numbers use `UiFormat.Number` (`1.200`); long labels shrink to fit.
- `Scripts/UI/Popups/SettingsPopup.cs`, `Editor/Building/PopupPrefabBuilder.cs` (`BuildSettings`).
- `Scripts/UI/Home/HomeController.cs`, `Editor/Building/LoginHomeSceneBuilder.cs` (`BuildHomeScene`).
- `Editor/Building/UiBuild.cs`, `Editor/Building/CommonUiPrefabBuilder.cs` (Slider, Toggle, Dropdown prefabs), `Editor/ItemIconImporter.cs` (sprite import and 9-slice borders).
- `Scripts/UI/Common/UiKit.cs` (`CurrencyArt`, `SingleLineFit`, `SlicedAutoFit`).
- `Scripts/UI/Popups/ConfirmPopup.cs`, `Scripts/Core/SceneFlow.cs`, `Scripts/Core/GameBootstrap.cs`.
- `Tests/EditMode/UI/LoginHomeSceneTests.cs` and the existing popup tests.

## Hard rules
- **No git commands at all.**
- **Do not touch** `LoginScene` or its builder code path. The user edited that scene by hand.
- **Do not edit** `.unity`, `.prefab` or `.asset` files by hand. Change the builders only; Claude rebuilds.
- **Only create or edit:**
  - `Scripts/Core/GameSettings.cs` (new);
  - `Scripts/Core/GameBootstrap.cs` (only to apply settings at startup);
  - `Scripts/UI/Popups/SettingsPopup.cs`;
  - `Scripts/UI/Home/HomeController.cs`;
  - `Editor/Building/PopupPrefabBuilder.cs` (only `BuildSettings` and small helpers it needs);
  - `Editor/Building/LoginHomeSceneBuilder.cs` (only `BuildHomeScene` and its helpers);
  - `Editor/ItemIconImporter.cs` (only to add 9-slice borders for the new sprites);
  - `Editor/Building/SceneSnapshot.cs` (only to add a `SettingsPopup` capture);
  - new tests under `Tests/EditMode/`, plus updates to existing tests that check the Home header or the Settings popup.
- **UI text is Vietnamese; code is English.**
- **Every file you touch must pass:** `py tools/lint/check_style.py <files>`, run from `D:/CONG VIEC/One-Bag-Jianghu`.

## Part 1: GameSettings (`Scripts/Core/GameSettings.cs`)

A plain C# settings model with an injectable store, so tests never touch real PlayerPrefs.

- **`ISettingsStore`**: get and set float, int and bool by key, plus `Save()`.
  - `PlayerPrefsSettingsStore` is the real store.
  - `MemorySettingsStore` is the test store. Keep it public so tests can use it.
- **Keys:** prefix `"OneBagJianghu.Settings."`.
- **Fields and defaults:**

  | Field | Default | Notes |
  |---|---|---|
  | `MasterVolume` | 1 | float, clamped 0..1 |
  | `MusicVolume` | 0.8 | float, clamped 0..1 |
  | `SfxVolume` | 1 | float, clamped 0..1 |
  | `Muted` | false | |
  | `Fullscreen` | true | |
  | `ResolutionWidth`, `ResolutionHeight` | 0 | 0 = keep the current resolution |
  | `VSync` | true | |
  | `FrameRateLimit` | 60 | allowed values 30, 60, 120, or -1 for no limit; other values snap to 60 |

- **Methods:**
  - `Load(store)`;
  - `Save()`;
  - `ResetToDefaults()`;
  - `event Action Changed`;
  - `float EffectiveMusicVolume => Muted ? 0 : MasterVolume * MusicVolume`, and the same for SFX.
  - These two are what the future audio system will read; there is no audio yet.
- **`Apply()`** pushes the settings to Unity:
  - `AudioListener.volume = Muted ? 0 : MasterVolume`;
  - `QualitySettings.vSyncCount = VSync ? 1 : 0`;
  - `Application.targetFrameRate = VSync ? -1 : FrameRateLimit`;
  - fullscreen and resolution via `Screen.SetResolution(w, h, mode)`, using `FullScreenMode.FullScreenWindow` or `Windowed`. When the width is 0, keep the current size.
- **`static GameSettings Current`:** created lazily from `PlayerPrefsSettingsStore`. `GameBootstrap.Initialize` calls `GameSettings.Current.Apply()` once.

## Part 2: Settings popup

**Rebuild** `BuildSettings` as a paper panel of about 900×780, with the title "Cài đặt" and three sections. Each section has a small header in the Title font and a thin ink divider.

**1. Âm thanh**
- Three rows: "Âm lượng chung", "Nhạc nền", "Hiệu ứng".
- Each row has the label on the left, a Slider from the common `Slider` prefab in the middle, and a value label such as "80%" on the right.
- A Toggle labelled "Tắt tiếng".
- When muted, the sliders stay usable but are drawn at 50% alpha.

**2. Hiển thị**
- Toggle "Toàn màn hình".
- Dropdown "Độ phân giải":
  - options come from `Screen.resolutions`, distinct width×height, at least 1280×720, sorted from largest;
  - format each as `1920 × 1080`;
  - select the current resolution, or the closest one.
- Toggle "Đồng bộ dọc (VSync)".
- Dropdown "Giới hạn FPS" with options 30, 60, 120, "Không giới hạn". It is non-interactable while VSync is on.

**3. Tài khoản**
- Line "Hiệp khách: <HeroName>".
- If the profile has a stable id field, add a second line "Mã: <id>". If it has none, skip that line. **Do not invent an id.**
- Button "Về màn đăng nhập":
  1. `ConfirmPopup` ("Về màn đăng nhập", "Tiến trình đã được lưu. Quay về màn đăng nhập?", "Đồng ý", "Hủy");
  2. on confirm: save the profile, close the settings popup, `SceneFlow.Go(GameScene.Login)`.
- Button "Thoát game":
  1. `ConfirmPopup` ("Thoát game", "Bạn muốn rời giang hồ?", "Thoát", "Hủy");
  2. on confirm: save, then `Application.Quit()`. Under `#if UNITY_EDITOR`, set `EditorApplication.isPlaying = false` instead.

**Bottom row:** "Mặc định" (secondary) resets to defaults, applies and refreshes the controls; "Đóng" (primary).

**Behaviour**
- Every change applies immediately and saves.
- `OnSetup` reads `GameSettings.Current` into the controls without firing listeners.
- Remove listeners in `OnDestroy`.

**Layout**
- Use anchors so nothing overlaps at 1920×1080.
- Labels get `SingleLineFit` so long text shrinks instead of overflowing.
- Dropdowns use the common `Dropdown` prefab unchanged. It now works; do not modify it.

## Part 3: Home top banner (`BuildHomeScene` + `HomeController`)

Replace the flat paper header with a richer banner, about 112 px tall, stretched across the top.

**Background**
- Image named "Header" with sprite `home_banner` (Sliced). Add borders in `ItemIconImporter` so the two ornamented ends are not stretched.
- If the sprite is missing, keep the current translucent paper colour.

**Left group, starting at x=24**
- **Avatar**, 96×96:
  - a `RectMask2D` square containing the `hero_portrait` sprite, anchored to the top so the face shows (portrait is 1024×1536, half-body; size it about 96×144, top-aligned);
  - on top, the `avatar_frame` sprite, or `hero_portrait_frame` if `avatar_frame` is missing.
- **Name**, to the right of the avatar: Title font, 28, `SingleLineFit`.
- **Level badge**: the `seal_title` sprite, about 52×52, with the text "Cấp N" or just the number. Choose what stays readable, and keep `levelText` wired.
- **EXP bar**:
  - about 300×30, under the name;
  - frame `progress_bar` (Simple, `preserveAspect`);
  - fill is a solid Simple Image driven by the Slider fill rect, with anchorMax (0,1) and inset inside the frame. Copy the approach of `GameControllerPrefabBuilder.BuildLoadingProgress`. **Do not use `Image.Type.Filled` without a sprite;** it ignores `fillAmount`;
  - `expText` overlays it.

**Right group**
- **Four currency slots**: Thể Lực, Bạc, Nguyên Bảo, Danh Vọng.
  - Each slot is about 190×56 with the `currency_slot` sprite (Sliced), falling back to `plaque_label`.
  - Each holds a 40×40 icon on the left (`icon_stamina`, `icon_bac`, `icon_nguyen_bao`, `icon_danh_vong`, through `CurrencyArt`) and the **number only** on the right, in light paper colour on the dark slot.
  - Stamina shows `120/120`.
  - The caption ("Bạc"…) moves to a hover tooltip using the existing tooltip mechanism, if one fits simply. Otherwise drop the caption.
  - Keep `staminaText`, `silverText`, `premiumText` and `reputationText` wired to the number labels.
- **Settings button** at the far right: a square icon button of about 72×72 with the `icon_settings` sprite. Fall back to the current "Cài đặt" secondary button if the sprite is missing. Hover tooltip "Cài đặt (Esc)". Keep `settingsButton` wired.

**HomeController**
- `SetCurrency` shows only the number. Remove the caption from the label text.
- Stamina shows `current/max`, if the max is available from the profile or config. Check what exists; do not invent it.

**Fallbacks**
- Every new sprite may be missing until the art arrives. The scene must still build and look acceptable with the current art.
- Use `UiBuild.ApplyUiSprite`'s return value.

## Part 4: Tests (EditMode)
- **`GameSettingsTests`** with `MemorySettingsStore`:
  - defaults;
  - clamping of volumes;
  - frame-rate snapping;
  - save then load round-trip;
  - `ResetToDefaults`;
  - effective volumes when muted.
  - **Do not** call `Apply()` in tests.
- **`LoginHomeSceneTests` / popup tests:** update the header assertions to the new hierarchy. Assert that the Settings popup prefab has every serialized reference assigned (use the existing `AssertAssignedReferences` pattern).

## Part 5: Snapshot
In `SceneSnapshot.CaptureAll`, add a capture of `SettingsPopup` the way other popups are captured, so Claude can check the layout.

## Report
Write to the `-o` output:
- the files changed;
- what you could not do and why;
- any assumption, such as the id field and the stamina max;
- the lint result.
