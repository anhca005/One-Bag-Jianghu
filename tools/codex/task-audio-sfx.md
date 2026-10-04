# Codex task: audio system (SFX) for battle replay and UI

Unity project: `D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu`. All code is under `Assets/_Game/`. You cannot run Unity;
Claude builds and tests.

**Read first:**
- `docs/tech/code-style.md` (mandatory);
- `../docs/audio/sfx-plan.md` section 4, the system design;
- `../docs/reviews/2026-10-08-gemini-sfx-map.md`;
- the data files `../tools/gemini/out/audio/sfx-catalog.json` (79 keys) and `../tools/gemini/out/audio/sfx-map.json`
  (per-item attack / hit / ultimate / status keys);
- `Scripts/Core/GameSettings.cs`: Master/Music/Sfx volumes and Muted already exist, use them;
- the replay player and its event types, under `Scripts/Battle` / `Scripts/UI/Replay`;
- `Editor/ItemIconImporter.cs` and `ItemIconLibrary`, the pattern for a library filled by an editor menu;
- `Tests/PlayMode` and its `PlayerDataGuard`.

## Scope (follow sfx-plan section 4, with these changes)
1. **Data.**
   - Copy `sfx-catalog.json` and `sfx-map.json` into `Resources/Data/Audio/`.
   - Runtime reads the per-item keys from `sfx-map.json`. Do not hardcode item→sound tables in C#.
2. **`SfxLibrary`** (ScriptableObject at `Resources/Audio/SfxLibrary.asset`).
   - Each entry has: key, `AudioClip[]` variants, base volume, pitch jitter (default ±5%), and a minimum interval.
   - Base volume and duration come from the catalog where given.
   - **Editor menu** "One Bag Jianghu/Audio/Rebuild Sfx Library":
     - scan `Assets/_Game/Audio/Sfx/**` for `.ogg`/`.wav`/`.mp3`;
     - group by file name `key_N` → `key`;
     - save the asset.
     - Batch-callable: `OneBagJianghu.EditorTools.Audio.SfxLibraryBuilder.Rebuild`.
   - Clips will arrive later. **Everything must work with zero clips.**
3. **`AudioManager`.**
   - Bootstrapped singleton with `DontDestroyOnLoad`; hook it into the existing bootstrap.
   - Pool of about 12 `AudioSource`s; at most 3 simultaneous plays of the same key.
   - **Volumes:** use `GameSettings.Current.EffectiveSfxVolume` for SFX and UI, and react when settings change. **Do not
     add new PlayerPrefs keys.** Keep a Music channel stub using `EffectiveMusicVolume` for later.
   - `Play(string key)` and `Play(string key, float volumeScale)`. An unknown key or a key with no clips does nothing
     and logs once.
4. **`BattleSfxResolver`** (pure C#, testable). Maps replay events to keys, using `sfx-map.json` per item and the
   catalog `event` fields:
   - basic `Trigger` → item `attackKey`;
   - ultimate → item `ultimateKey`, else `ultimate` + `attackKey`;
   - `Damage` → item `hitKey`, `hit_crit` on a crit, `shield_hit` when fully absorbed;
   - status apply/tick → status keys;
   - `Start` → `battle_start`, plus `boss_intro` on PvE boss stages if easy;
   - `End` → `win` / `lose` from the player's view.
   - Non-weapon items (pill, manual, relic, armor) use their mapped keys.
   - **Replay speed:**
     - play events only in the time window just advanced;
     - never burst-play after a seek or skip;
     - above 2× speed, play only `Trigger`, crit `Damage` and `End`.
5. **UI.**
   - **`UiSfx` component:** click on `Button`, open/close on popups.
     - Add it through the **builders**, to the common button prefabs and the popup base, not by hand-editing prefabs.
     - Any builder you touch must stay buildable. Never touch `LoginScene` or `BuildLoginScene`.
   - **Direct calls** at the right places:
     - item pick/drop/bad drop (bag and storage drag);
     - shop buy/sell → `coin`;
     - not enough currency → `ui_error`;
     - upgrade, merge, level-up, reward, stage select, dialogue next, node unlock, star rating.
     - Use the keys in the catalog. Keys without clips are silent, which is fine.
6. **Settings popup.** It already has volume sliders. Make sure changing them updates `AudioManager` live.

## Tests
- **EditMode:**
  - every key the resolver or UI can emit exists in the catalog, and every `sfx-map.json` key exists in the catalog;
  - resolver cases: basic, ultimate with and without a signature key, crit, shield, status, end win/lose;
  - the speed and seek rule: no burst after a seek, and filtering above 2×;
  - `SfxLibraryBuilder` grouping by file name, tested on name lists;
  - `AudioManager` with no clips does not throw.
- **PlayMode:** one replay runs to the end with an empty library and no errors.
  - Keep `PlayerDataGuard` protecting settings: tests must not change the user's PlayerPrefs.

## Allowed files
- New code under `Scripts/Audio/`, `Editor/Audio/` and `Tests/**/Audio/`;
- `Resources/Data/Audio/`, `Resources/Audio/`, and the folder `Audio/Sfx/` (create it empty, with a `.gitkeep` or
  README);
- minimal hooks in:
  - the existing replay player;
  - bootstrap;
  - the bag/storage drag code;
  - shop, upgrade and merge popups;
  - the PvE map and dialogue;
  - result popup;
  - the Settings popup;
  - the common UI and popup builders.

**Do not touch:**
- `tools/**`, `docs/**`;
- game balance data;
- `LoginScene`.

## Rules
- **No git.**
- Run `py tools/lint/check_style.py <changed .cs files>` from `D:/CONG VIEC/One-Bag-Jianghu`; it must be clean for
  new files and for the lines you changed.

## Report (`-o` output)
- The files changed.
- Where each hook is.
- The keys used that have no catalog entry (should be none).
- The lint result.
- How Claude adds clips: the folder, file naming and the menu.
