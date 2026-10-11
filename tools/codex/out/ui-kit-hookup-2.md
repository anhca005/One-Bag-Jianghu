Implemented UI kit groups E–G and redrew the play/pause icons.

Highlights:

- Battle bars, fighter panels, timeline, VS emblem, and replay-control icons hooked up.
- Result victory/defeat art switches at runtime; reward entry uses `reward_slot`.
- Tutorial uses `tutorial_ring` plus an animated, automatically positioned pointer.
- Encyclopedia category icons are matched semantically.
- Every sprite integration retains a missing-file fallback.
- `star_burst` skipped because no earned-star widget currently exists.
- Unity was not launched.

Verification:

- Style checker: 0 problems across all six changed C# files.
- Full solution build: 0 errors, 0 warnings.
- Replay and onboarding EditMode test assemblies compile.

Run these Unity menus in order:

1. `One Bag Jianghu/Reimport Art And Rebuild Icon Library`
2. `One Bag Jianghu/Build/Replay Prefabs (Overwrite)`
3. `One Bag Jianghu/Build Popup Prefabs (Overwrite)`

Full handoff: [ui-kit-hookup-2.md](D:/CONG%20VIEC/One-Bag-Jianghu/tools/codex/out/ui-kit-hookup-2.md)