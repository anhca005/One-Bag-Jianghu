# Codex task: background music (BGM) and stings

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `Scripts/Audio/AudioManager.cs` (SFX pool; already exposes `MusicVolume` from `GameSettings.EffectiveMusicVolume`);
- `Scripts/Audio/SfxLibrary.cs` and `Editor/Audio/SfxLibraryBuilder.cs` (the pattern for a Resources library asset);
- `Scripts/Core/SceneFlow.cs`, `Scripts/UI/Home/HomeController.cs`, `Scripts/UI/Pve/StageMapPopup.cs`,
  the replay/battle screen controller, the PvP (Luận Kiếm Đài) entry, and the intro screen if it exists
  (another task may have just added `Scripts/UI/Onboarding/`; only hook into it, do not restructure it).

## Clips (already in place)
`Assets/_Game/Audio/Music/*.ogg`:
- loops: `bgm_home`, `bgm_map_hoi_1` … `bgm_map_hoi_5`, `bgm_battle`, `bgm_boss`, `bgm_pvp`, `bgm_intro`;
- one-shots: `sting_victory` (5.3 s), `sting_defeat` (6 s).

Make the importer set music clips to **Streaming** load type, Vorbis, not preloaded (an `AssetPostprocessor` rule
for `Audio/Music/`, or extend an existing one).

## Do
1. **`MusicLibrary`** ScriptableObject in `Resources/Audio/MusicLibrary.asset`: key → clip, built by an editor menu
   (`One Bag Jianghu/Audio/Rebuild Music Library`) that scans `Audio/Music/`, like `SfxLibraryBuilder`. Also callable
   in batch: `OneBagJianghu.EditorTools.Audio.MusicLibraryBuilder.Rebuild`.
2. **`MusicPlayer`** (inside `AudioManager` or a sibling component on the same object):
   - two `AudioSource`s for a crossfade (default 1.0 s, unscaled time); loops loop;
   - `PlayMusic(key)`: no-op if the same key is already playing; unknown key → one warning, keep current music;
   - `PlaySting(key)`: ducks the music to 20 % while the sting plays, then restores it;
   - `StopMusic(fade)`;
   - volume follows `GameSettings` music volume live (the Settings slider already exists; check it is wired).
3. **Where music changes:**
   - Login and Home → `bgm_home`;
   - stage map popup open → `bgm_map_hoi_<chapter>` for the chapter shown, switching when the player changes chapter;
     closing it returns to `bgm_home`;
   - PvE battle replay → `bgm_boss` for boss stages (use the existing boss flag on the stage), else `bgm_battle`;
   - PvP battle → `bgm_pvp`;
   - battle result: `sting_victory` / `sting_defeat`, then the screen's music resumes;
   - intro scroll panels (if present) → `bgm_intro`.
   Keep each hook to a line or two; put the key choice in one small pure helper (`MusicKeys.ForStage(...)` etc.).
4. **Credits:** in the Settings popup add a small static line at the bottom:
   `Nhạc: Richard Guo · Tiếng động: Kenney (CC0)`. Use the existing text style.

## Tests (EditMode)
- `MusicKeys`: chapter 1–5 → map keys; boss vs normal stage; out-of-range chapter → falls back to `bgm_map_hoi_1`.
- Library lookup: unknown key returns false without throwing.

## Rules
- **No git.** Never touch `LoginScene`, `BuildLoginScene`, `Map_hoi_1.prefab` or the GameController loading panel.
- Lint the changed `.cs` files with `py tools/lint/check_style.py`; they must be clean.

## Report (`-o` output)
- The files changed, where each hook sits, the lint result.
