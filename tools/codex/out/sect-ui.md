Implemented the Môn Phái popup redesign.

Changed:

- [SectSelectPopup.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Popups/SectSelectPopup.cs)
- [SectCatalog.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/Bag/SectCatalog.cs)
- [HomeController.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Scripts/UI/Home/HomeController.cs)
- [EconomyPopupPrefabBuilder.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/Building/EconomyPopupPrefabBuilder.cs)
- [ItemIconImporter.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Editor/ItemIconImporter.cs)
- [SectPopupTests.cs](D:/CONG%20VIEC/One-Bag-Jianghu/One-Bag-Jianghu/Assets/_Game/Tests/EditMode/SectPopupTests.cs)

Key results:

- Seven localized sect cards with selection highlighting and “Bổn môn”.
- Full sect detail, lore, weapon specialties, Tâm Pháp art/name/level, and four localized milestone descriptions.
- Permanent-choice confirmation before joining.
- Joined players can always reopen the popup; their own sect is preselected.
- “Sắp mở” Tâm Pháp upgrade button is disabled, with a reserved implementation hook.
- Tutorial events and `sect_choices` target are wired.
- Sect textures import as Sprites.
- `SectArtLibrary.asset` is populated from `Textures2D/Sects` using `sect_<id>` and `tam_phap_<id>` keys.
- Missing images use an accent-colored ink fallback. Currently only the seven `tam_phap_*.png` files are present; missing `sect_*.png` files will therefore use fallbacks.

Rebuild in Unity with:

`One Bag Jianghu → Build → Sect Popup (Overwrite)`

This also rebuilds the sect art library.

Verification:

- Style lint: 0 problems across all six changed/new C# sources.
- `OneBagJianghu.Editor.csproj` build: succeeded with no errors.
- Unity/EditMode tests were not run because Unity was left running as requested.
- No PNG or `.meta` files were written.