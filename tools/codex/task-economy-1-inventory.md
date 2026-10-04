# Task I (economy 1/3): player profile, inventory & currencies — offline foundation ("One Bag Jianghu")

Repo root `D:\CONG VIEC\One-Bag-Jianghu`, Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1). Read `docs/decisions.md` #4, #12, #20, #21, #22, `docs/design/gdd.md`, `docs/design/items.md`, `Scripts/Core/GameSession.cs`, `Scripts/UI/Common/BagLoadoutPanel.cs`, `Scripts/Bag/`.

## Why
Today the player's bag can use any catalog item at any Trọng, and the profile only stores name/sect in PlayerPrefs. Decision #22 needs real ownership: items the player owns (each with its own Trọng), materials, currencies. The numbers (materials, costs, shop, rewards) come later from Gemini; this task builds the model and storage so they can be plugged in as data.

## Hard rules
- Modify only under `One-Bag-Jianghu/Assets/_Game/`: new `Scripts/Economy/` (pure C#, **no UnityEngine**, own asmdef referencing `OneBagJianghu.Engine` + `Unity.Newtonsoft.Json` only, so it can be mirrored on the Java server later), `Scripts/Core/GameSession.cs`, `Scripts/UI/Common/BagLoadoutPanel.cs`, `Scripts/UI/Popups/BagPopup.cs`, `Scripts/Bag/` (only if the tray needs a source abstraction), `Resources/Data/` (new placeholder data), tests. Do not touch `Scripts/Engine`, battle replay, VFX, scenes' layout (builders only if a new serialized field needs wiring; Claude re-runs builders).
- English, C# 9, existing style. No git. You cannot run Unity.
- Known pitfalls: CS0165 with `out` inside `&&` (declare first); never block the main thread on async.

## Build
1. **Model (`Scripts/Economy/`)**: `PlayerProfile` { version, heroName, heroLevel, heroExp, sectId, tamPhapLevel, currencies (bac, nguyen_bao, danh_vong as a dictionary id→long), materials (id→int), items: list of `OwnedItem` { uid (string, unique), itemId, tier 1..9, acquiredAt }, loadout (bag placement referencing owned uids) }. Pure functions with validation and clear error messages: `AddItem`, `RemoveItem`, `AddMaterial`, `SpendMaterials`, `AddCurrency`, `TrySpend` (atomic: either everything is paid or nothing changes).
2. **Data**: `MaterialCatalog` loading `Resources/Data/materials.json` (placeholder with 2–3 materials, schema `{id, name, itemType, band, description}`; unknown fields rejected like `ItemCatalog`). The real list will come from Gemini.
3. **Storage**: `ProfileStore` saves/loads JSON at `Application.persistentDataPath/profile.json` (the Unity-facing wrapper lives in the game assembly), atomic write (temp file + replace), keeps one `.bak`, schema `version` with a migration from the current PlayerPrefs profile (name, sect) — do not lose existing players' names. A new profile gets a **starter kit** defined in data (`Resources/Data/starter.json`: e.g. 2 weapons + 1 armor at Trọng 1, some Bạc) — keep the current default loadout playable.
4. **GameSession** exposes the profile; `CurrentLoadoutJson` is built from the loadout + owned items' tiers (the engine input format stays exactly the same — parity tests must not change).
5. **Bag UI**: the player's `BagPopup` tray lists **owned items only** (each copy separately, showing its Trọng); placing/removing moves owned uids in/out of the loadout; saving writes the profile. `BagDemo` keeps the full-catalog developer mode (add a serialized source option; do not edit the BagDemo scene by hand — tell Claude if a builder must be re-run).
6. **Tests** (EditMode): profile atomic spending, add/remove, JSON round-trip, migration from PlayerPrefs values, starter kit, loadout → engine JSON uses owned tiers, unknown-field rejection. Keep all existing EditMode (132) and PlayMode (5) tests green; the PlayMode full flow must still reach a battle (the starter kit provides a weapon).

Finish with: files created/changed, builder methods Claude must re-run (if any), anything unsure.
