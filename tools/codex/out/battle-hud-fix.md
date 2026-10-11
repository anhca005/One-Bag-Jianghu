Implemented the battle-screen review fixes without launching Unity.

Files changed:

- [ReplayPrefabBuilder.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/ReplayPrefabBuilder.cs>)
- [UiBuild.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/UiBuild.cs>)
- [ItemIconImporter.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/ItemIconImporter.cs>)
- [ReplayScreen.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/View/ReplayScreen.cs>)
- [GameplayPrefabTests.cs](</D:/CONG VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/UI/GameplayPrefabTests.cs>)

Key layout changes:

- Player avatar: x=0–250; bag now starts at x=270.
- Enemy bag: x=0–560; avatar starts at x=580.
- Both sides consequently have a clear 20 px gap.
- Portrait backing and art inset increased from 12 px to 22 px.
- Restart control removed; runtime fallback also no longer creates it.
- Speed controls use 64×64 `button_square` art at x=184/254/324.
- Collapsed log moved to x=550, width 1300, height 96.
- Expanded log is 1320×440 at x=300, with 76×42 px content clearance.
- Exit uses a 64×64 round button at x=1832, y=66 with its label underneath.
- Clock plate is 280×112; clock and stage title are children with shadow and outline.
- HP frame is 620×39; Cương Khí and Nội Lực frames are 620×21.
- Status sockets are fixed 32×32 cells with inset icons and 25×18 count badges.
- HUD uses `hud_panel`; avatar title uses `name_plate`.

Added 9-slice/import entries:

- `hud_panel`: `(145,42,145,42)`, PPU multiplier 6
- `gauge_frame_large`: `(150,24,150,24)`, multiplier 10
- `gauge_frame_small`: `(150,13,150,13)`, multiplier 10
- `status_slot`: `(230,230,230,230)`, multiplier 16
- `count_badge`: `(235,105,235,105)`, multiplier 18
- `log_panel`: `(150,45,150,45)`, multiplier 6
- `button_exit`: `(230,230,230,230)`, multiplier 16
- `name_plate`: `(190,55,190,55)`, multiplier 8
- `clock_plate`: `(215,90,215,90)`, multiplier 10
- `button_square`: `(230,230,230,230)`, multiplier 16

Tests now assert the removed restart object, symmetric avatar/bag gaps, new log/clock/exit/HUD/gauge/name artwork, portrait inset, outlines, and status socket hierarchy.

Verification:

- Style checker: 0 problems across all five changed C# files.
- All generated `.csproj` files build successfully.
- Two existing obsolete-Unity-API warnings occurred in unrelated files during the first game build.
- Unity prefab rebuilding, EditMode execution, and screenshots were intentionally skipped as requested.