# Task: per-VFX manual tuning (rotation / scale / offset / flip) + editing UI in VFX Lab ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, procedural UGUI, Input System). 39 hand-drawn VFX sprites live in `Assets/_Game/Textures2D/Vfx/` and are looked up by key through `ItemIconLibrary`. They were drawn by an AI, so some point slightly up/down, are too small/large or off-center. The artist needs to fix each one by hand **without editing the PNGs**, and see the result live.

Read first: `Scripts/Battle/Replay/View/ReplayVfxLayer.cs` (Present/Tick/Animate, mirroring for side 1), `Scripts/Battle/Replay/View/VfxLab/` (VfxLabBootstrap, VfxCatalog, README), `Editor/DemoSceneBuilder.cs`, `Editor/ItemIconImporter.cs` (ItemIconLibraryBuilder).

## Hard rules
- Create/modify files only under `One-Bag-Jianghu/Assets/_Game/`. No Packages/, ProjectSettings/, no docs/, no git.
- English code/comments, C# 9, match existing style. Editor-only APIs (`UnityEditor`) only inside `#if UNITY_EDITOR` or in the Editor assembly. Must compile in Unity 6; existing 98 EditMode tests must keep passing.
- Battle rendering must stay identical for any VFX without tuning (identity defaults).

## 1. Data — `Scripts/Battle/Replay/View/VfxTuning.cs`
- `VfxTuning : ScriptableObject`, stored at `Assets/_Game/Resources/VfxTuning.asset`, `static VfxTuning Load()` (Resources, cached; returns null-safe empty instance if missing).
- Serializable entry per key: `key`, `rotation` (degrees, -180..180, default 0), `scale` (float, default 1, clamp 0.2..4), `offset` (Vector2 in canvas px, default 0), `flipX`, `flipY` (bool). Missing key → identity.
- `TryGet(key, out entry)`, `GetOrCreate(key)`, `Reset(key)`, `IsIdentity(entry)`.
- A pure static helper (no Unity objects needed, easy to test), e.g. `VfxTransform Compose(entry, bool mirrored)` → final `localScale` (x, y), `localEulerAngles.z`, and position offset. Rules: the tuning describes the effect as drawn for the attacker on the LEFT (travelling left→right). When mirrored (attacker on the right), the whole tuned result is mirrored horizontally: scale.x sign flips, rotation angle negates, offset.x negates. flipX/flipY apply before mirroring.

## 2. Apply in battle — `ReplayVfxLayer`
- Load tuning once in `Initialize`. In `Present`, store the composed transform on the Effect; in `Animate` apply it together with the existing pop-in scale (multiply) and travel position (add offset); rotation via `localEulerAngles`. Fallback shapes (no art) ignore tuning.
- Add a public way for the lab to re-read tuning after edits (e.g. `RefreshTuning()` or the tuning object is read live each Present — keep it cheap).

## 3. Editing UI — VFX Lab
- Add an **Tuning panel** for the selected entry (put it where it fits without overlapping: e.g. right side of the inspector strip): rotation slider (-180..180) + numeric label, scale slider (0.2..3), offset X / Y sliders (-200..200), Flip X / Flip Y toggles, **Reset** (back to identity), **Save**.
- Every change applies immediately and the selected effect auto-loops while the panel is being edited (so the artist sees the result live). Also apply tuning to the static preview in the inspector box (rotation/scale/flip), so direction can be judged without animation.
- Keyboard shortcuts while in the lab: Q / E rotate −5° / +5° (hold Shift for 1°), Z / X scale −0.05 / +0.05, arrow keys already used for prev/next — use I/J/K/L? `L` is loop already, so use W/A/S/D for offset ±5 px. Show the shortcuts in the hint line.
- Row status in the list: add a small mark (e.g. "✎") on entries that have non-identity tuning.
- **Save**: in the Editor, `EditorUtility.SetDirty(tuning)` + `AssetDatabase.SaveAssets()` (inside `#if UNITY_EDITOR`); show "Đã lưu" feedback. Note: ScriptableObject edits in Play mode already persist in the Editor, Save just forces the write to disk. In a build, Save is hidden.
- Unsaved changes indicator ("● chưa lưu") next to Save.

## 4. Editor
- `Create VFX Lab Scene` menu and a new menu **One Bag Jianghu → Create VFX Tuning Asset** create `Resources/VfxTuning.asset` if missing (never overwrite an existing one).

## 5. Tests — `Tests/EditMode/VfxLab/VfxTuningTests.cs`
- Missing key → identity; identity compose (not mirrored) = scale (1,1), rotation 0, offset 0.
- Mirrored compose: rotation negated, scale.x negative, offset.x negated; flipX + mirror = positive scale.x.
- Scale clamp; Reset removes the tuning.

## 6. Docs
- Update `Scripts/Battle/Replay/View/VfxLab/README.md` (Vietnamese, keep ≤ 40 lines): how to tune, shortcuts, Save, where data lives, "chỉnh cho phía bên TRÁI, bên phải tự lật".

Finish with: files created/changed, anything unsure (API calls you could not verify without Unity).
