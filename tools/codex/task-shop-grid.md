# Task Q: Thương Hội as a 2×3 card grid ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/`. Task P built `ShopPopup` with the 6 offers as long full-width row buttons. The owner wants the shop redesigned: **a 2 rows × 3 columns grid, one item per cell**. Read `Scripts/UI/Popups/ShopPopup.cs`, `Scripts/Economy/{ShopService,ShopCatalog}.cs`, `Editor/Building/{EconomyPopupPrefabBuilder,CommonUiPrefabBuilder,UiBuild}.cs`, `Scripts/UI/Popups/StoragePopup.cs` (rarity colours, icon loading), `docs/design/glossary.md` (rarity colours), tests under `Tests/EditMode/UI`, `Tests/EditMode/Economy`, `Tests/PlayMode`.

## Layout (Thương Hội tab, popup ≈ 1600×900 at 1920×1080)
- Header row (unchanged content): Bạc, Danh Vọng, next free refresh countdown, **Làm mới** button with price.
- Body: `GridLayoutGroup` 3 columns × 2 rows, fixed cell ≈ 420×300, spacing ≈ 28, centred. Always 6 cells (slot order 1..6: row 1 = slots 1–3, row 2 = slots 4–6).
- **Card prefab** `ShopCard.prefab` (built once by a builder, `ShopCard` component with serialized refs):
  - Paper panel (`frame_panel` art via existing 9-slice helpers) with a **border tinted by rarity** (materials: tier colour or neutral).
  - Large icon area (≈ 60% of the card height), icon keeps aspect (`preserveAspect`), placeholder = ink silhouette + name when art is missing.
  - Name (≥ 22 px), second line: rarity name + "1 Trọng" for item copies, or "×N" stack for materials, item type label (Binh Khí/Hộ Cụ/…).
  - Price line with a Bạc label (text for now; icon slot ready for a future `icon_bac`), a red/discount tag when the slot has a discount (show old price struck through if the data has it).
  - **Mua** button at the bottom (common primary button). Disabled with reason text when not enough Bạc. Bought → card dimmed with a vermilion "Đã mua" seal overlay (use `seal_title` art if present), button hidden.
  - Hover/click on the card shows the item tooltip text (stats summary: rarity, Trọng, type, attack/cooldown for weapons, description of main effect if easy) in the existing tooltip prefab if available; otherwise skip tooltip.
- Buying still asks for confirmation (ConfirmPopup) showing item + price.
- **Tiệm Danh Vọng** tab: same card prefab in a single row of 3 cards (price in Danh Vọng, "còn N/tuần"). **Đổi nguyên liệu** tab: unchanged unless it breaks.

## Hard rules
(Note: the 10×20 spatial storage and its "không đủ chỗ" rule come in the next task R — do not implement storage capacity here.)
- Modify only: `Scripts/UI/Popups/ShopPopup.cs`, new `Scripts/UI/Popups/ShopCard.cs`, `Editor/Building/EconomyPopupPrefabBuilder.cs` (and `CommonUiPrefabBuilder.cs` only if a shared helper is needed), `Tests/EditMode/UI/**` (not Gameplay*), `Tests/EditMode/Economy/**`, `Tests/PlayMode/**`. Do not change `ShopService` behaviour.
- UI is authored by builders into prefabs (decision #20); popups initialise lazily (PopupManager calls Setup before Awake — see `StoragePopup.EnsureInitialized`). English code, Vietnamese UI. C# 9. No git. You cannot run Unity; re-read for compile errors (CS0165; fully qualify `UnityEngine.UI.Image.Type` in builders; test asmdefs must reference used assemblies).

## Tests
Prefab test: ShopPopup has a GridLayoutGroup with 3 columns, 6 ShopCard children after setup (or card template + container), all ShopCard refs assigned. Existing PlayMode shop/storage tests stay green (adapt the shop test to click the card's Mua button). Finish with: files changed, builder menu items Claude must run (order), anything unsure.
