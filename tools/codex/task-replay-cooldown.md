# Codex task CD: basic-attack cooldown overlay on items in the battle replay

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`.

Read first:
- `Scripts/Battle/Replay/README.md`, `BattleTimeline.cs`, `BattleEvent.cs`, `BattleLog.cs`;
- `Scripts/Battle/Replay/View/ReplayItemView.cs`, `ReplayScreen.cs`, `ReplayStyle.cs`;
- `Editor/Building/ReplayPrefabBuilder.cs`;
- the replay tests under `Tests/`.

**Builders only, never edit prefabs by hand. No git. Do not run Unity** (the license fails in your sandbox); Claude runs it.

## What the user wants
While a battle plays, every item that attacks on a cooldown shows a **dark, semi-transparent overlay** over its cell:
- right after the item fires, the overlay covers the whole cell;
- it then **shrinks downward** (the top edge drops), until it is gone at the moment the item fires again.

It works like a cooldown wipe in action games, in the ink style. Black at about 45–55% alpha (`ReplayStyle`), with no hard border.

## Source of truth: the battle log, not a re-simulation
The client has the whole log, so derive each cycle from the item's real activations:
- An activation is a `TRIGGER` event whose `item` is that uid and which is the item's own attack: basic or ultimate. Check how `ReplayScreen` already decides that a `TRIGGER` is a weapon attack (it launches VFX for it) and reuse that rule. Do not count passive triggers, such as on-hit procs.
- For time `t` between activations `a_k` and `a_{k+1}` of the same item:

  `fill = (a_{k+1} − t) / (a_{k+1} − a_k)`

  1 = full overlay, 0 = gone.

  Using the next real activation automatically includes Trì Trệ (slow), Điểm Huyệt (seal), haste, stun and parry delays. That is intended.
- **Before the first activation:** use `[0, a_0]`, so the first cooldown also drains from battle start.
- **After the last activation** (the battle ended first): keep draining at the item's nominal cooldown from `items.json`. Use the length of its last measured cycle if there is no catalog entry. Clamp the result to [0, 1].
- **Items that never activate** (armor, pills…): no overlay.
- Also apply this to relics/manuals with an `active` + `cooldown`, if they produce such `TRIGGER` events. The rule is the same.
- Precompute per-item activation lists once when the log is loaded, in `BattleTimeline` or a small new class such as `ItemCooldownTrack`. `Refresh(timeMs)` must be allocation-free.
- It must be correct when **seeking/scrubbing** backwards and forwards, at x1/x2/x4, and when paused (frozen).

## View
- In `ReplayItemView`, add an `Image` "CooldownOverlay" above the icon/background and below the name/status texts and the flash.
- Use `Image.Type.Filled`, `fillMethod = Vertical`, `fillOrigin = Bottom`, so lowering `fillAmount` makes the top edge drop.
- `raycastTarget = false`.
- `ReplayScreen` updates each view's fill from the track on every refresh, including after a seek.
- The overlay must not hide the status tags ("Điểm Huyệt", "Trì Trệ"…) or the rarity outline.
- Build it in `ReplayPrefabBuilder` for `ReplayItemView.prefab`.

## Tests (EditMode, plus one PlayMode check if cheap)
- **Track math** on a hand-made log: activations at 1000, 3000, 6000:
  - t=0 → 1.0;
  - t=500 → 0.5;
  - t=1000 → 1.0 (it just fired);
  - t=2000 → 0.5;
  - t=4500 → 0.5;
  - after 6000 it drains over the nominal cooldown.
- Seeking backwards gives the same values as playing forward.
- Passive or other-item `TRIGGER`s do not reset an item's cycle.
- Items with no activations: fill 0, and the overlay is inactive.
- The prefab has the overlay with `Filled`/`Vertical`/`Bottom` and `raycastTarget` false.

## Rules
- Edit only:
  - `Assets/_Game/Scripts/**`, except `Scripts/Engine/**`;
  - `Assets/_Game/Editor/**`;
  - `Assets/_Game/Tests/**`.
- No git. Do not touch the data JSON or `tools/gemini/**`.

## Result
- The changed files.
- How an activation is identified.
- What Claude should look at in a replay.
