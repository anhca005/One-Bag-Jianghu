# UI image distortion audit

Generated at 1920×1080. Aspect error is `abs(ln(rect aspect / sprite aspect)) × 100`. 
Only Sliced/Tiled uses and distorted Simple uses above 3% are included.

## Summary

| Class | Uses |
|---|---:|
| A | 18 |
| B | 0 |
| C | 0 |
| D | 2 |

Proposed new art files: **0**.

### Five worst distortions

- Infinity% — `Assets/_Game/Textures2D/UI/Kit/list_row_compact.png` at `Assets/_Game/Resources/UI/Popups/EncyclopediaPopup.prefab :: EncyclopediaPopup/Panel/Categories Viewport/Categories Content/Category Row Template` (0×48).
- Infinity% — `Assets/_Game/Textures2D/UI/Kit/list_row_compact.png` at `Assets/_Game/Resources/UI/Popups/EncyclopediaPopup.prefab :: EncyclopediaPopup/Panel/Entries Viewport/Entries Content/Entry Row Template` (0×48).
- 4.68% — `Assets/_Game/Textures2D/UI/Kit/popup_frame_wide.png` at `Assets/_Game/Resources/UI/Popups/MessagePopup.prefab :: MessagePopup/Panel` (720×420).
- 4.24% — `Assets/_Game/Textures2D/UI/Kit/button_secondary_compact.png` at `Assets/_Game/Resources/UI/Popups/ConfirmPopup.prefab :: ConfirmPopup/Panel/Cancel Button` (280×64).
- 4.24% — `Assets/_Game/Textures2D/UI/Kit/button_primary_compact.png` at `Assets/_Game/Resources/UI/Popups/ConfirmPopup.prefab :: ConfirmPopup/Panel/Confirm Button` (280×64).

## Measured uses by sprite

### `Assets/_Game/Textures2D/UI/Kit/button_primary_compact.png`

| Class | Source and object | Type | Preserve | PPU | Rect | Sprite px | Aspects | Error | Driven | Inactive |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A | `Assets/_Game/Resources/UI/Popups/ConfirmPopup.prefab :: ConfirmPopup/Panel/Confirm Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/MessagePopup.prefab :: MessagePopup/Panel/Close Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/ResultPopup.prefab :: ResultPopup/Panel/Play Again Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |

### `Assets/_Game/Textures2D/UI/Kit/button_secondary_compact.png`

| Class | Source and object | Type | Preserve | PPU | Rect | Sprite px | Aspects | Error | Driven | Inactive |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A | `Assets/_Game/Resources/UI/Popups/ConfirmPopup.prefab :: ConfirmPopup/Panel/Cancel Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/EncyclopediaPopup.prefab :: EncyclopediaPopup/Panel/Replay Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/IntroPopup.prefab :: IntroPopup/Panel/Skip Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/ResultPopup.prefab :: ResultPopup/Panel/Home Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/StoryPopup.prefab :: StoryPopup/Panel/Skip Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/TutorialOverlay.prefab :: TutorialOverlay/Panel/Guide Box/Skip Tutorial Button` | Simple | False | 1 | 280×64 | 520×124 | 4.375 / 4.194 | 4.24% | False | True |

### `Assets/_Game/Textures2D/UI/Kit/divider_brush.png`

| Class | Source and object | Type | Preserve | PPU | Rect | Sprite px | Aspects | Error | Driven | Inactive |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A | `Assets/_Game/Resources/UI/Popups/SettingsPopup.prefab :: SettingsPopup/Panel/Account Header Divider` | Simple | False | 1 | 480×12 | 1427×37 | 40 / 38.568 | 3.65% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/SettingsPopup.prefab :: SettingsPopup/Panel/Audio Header Divider` | Simple | False | 1 | 480×12 | 1427×37 | 40 / 38.568 | 3.65% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/SettingsPopup.prefab :: SettingsPopup/Panel/Display Header Divider` | Simple | False | 1 | 480×12 | 1427×37 | 40 / 38.568 | 3.65% | False | True |

### `Assets/_Game/Textures2D/UI/Kit/dropdown_field.png`

| Class | Source and object | Type | Preserve | PPU | Rect | Sprite px | Aspects | Error | Driven | Inactive |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A | `Assets/_Game/Resources/UI/Popups/ShopPopup.prefab :: ShopPopup/Panel/Exchange Panel/Input` | Simple | False | 1 | 541.44×81.33 | 1512×218 | 6.657 / 6.936 | 4.1% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/ShopPopup.prefab :: ShopPopup/Panel/Exchange Panel/Target` | Simple | False | 1 | 541.44×81.33 | 1512×218 | 6.657 / 6.936 | 4.1% | False | True |

### `Assets/_Game/Textures2D/UI/Kit/list_row_compact.png`

| Class | Source and object | Type | Preserve | PPU | Rect | Sprite px | Aspects | Error | Driven | Inactive |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| D | `Assets/_Game/Resources/UI/Popups/EncyclopediaPopup.prefab :: EncyclopediaPopup/Panel/Categories Viewport/Categories Content/Category Row Template` | Simple | False | 1 | 0×48 | 604×96 | 0 / 6.292 | Infinity% | True | True |
| D | `Assets/_Game/Resources/UI/Popups/EncyclopediaPopup.prefab :: EncyclopediaPopup/Panel/Entries Viewport/Entries Content/Entry Row Template` | Simple | False | 1 | 0×48 | 604×96 | 0 / 6.292 | Infinity% | True | True |

### `Assets/_Game/Textures2D/UI/Kit/popup_frame_wide.png`

| Class | Source and object | Type | Preserve | PPU | Rect | Sprite px | Aspects | Error | Driven | Inactive |
|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| A | `Assets/_Game/Resources/UI/Popups/ConfirmPopup.prefab :: ConfirmPopup/Panel` | Simple | False | 1 | 760×440 | 2048×1140 | 1.727 / 1.796 | 3.93% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/MessagePopup.prefab :: MessagePopup/Panel` | Simple | False | 1 | 720×420 | 2048×1140 | 1.714 / 1.796 | 4.68% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/SectSelectPopup.prefab :: SectSelectPopup/Panel` | Simple | False | 1 | 1600×920 | 2048×1140 | 1.739 / 1.796 | 3.25% | False | True |
| A | `Assets/_Game/Resources/UI/Popups/ShopPopup.prefab :: ShopPopup/Panel` | Simple | False | 1 | 1600×920 | 2048×1140 | 1.739 / 1.796 | 3.25% | False | True |

## Runtime

Code-created or code-mutated Images found by the requested source scan. The final field is the nearest visible sprite key or size expression when present.

- `Assets/_Game/Scripts/Battle/Replay/View/FighterPanelView.cs:121` — `frame.type = owner.type;`
- `Assets/_Game/Scripts/Battle/Replay/View/FighterPanelView.cs:128` — `owner.type = Image.Type.Simple;`
- `Assets/_Game/Scripts/Battle/Replay/View/ReplayItemView.cs:195` — `cooldownOverlay.type = Image.Type.Filled;`
- `Assets/_Game/Scripts/Battle/Replay/View/VfxLab/VfxLabBootstrap.cs:543` — `rootBackground.type = Image.Type.Simple;`
- `Assets/_Game/Scripts/Battle/Replay/View/VfxLab/VfxLabBootstrap.cs:559` — `rootBackground.type = Image.Type.Tiled;`
- `Assets/_Game/Scripts/Engine/Data.cs:588` — `if (d.type == ItemType.WEAPON && !d.IsActive())`
- `Assets/_Game/Scripts/Engine/Data.cs:590` — `if (d.type == ItemType.WEAPON && d.attack <= 0)`
- `Assets/_Game/Scripts/Engine/Data.cs:592` — `if (d.type == ItemType.WEAPON && !WeaponClasses.Contains(d.weaponClass))`
- `Assets/_Game/Scripts/Engine/Data.cs:596` — `if (d.type == ItemType.WEAPON && d.size != null && d.size.Length == 2 && d.size[0] * d.size[1] < 2)`
- `Assets/_Game/Scripts/Engine/Data.cs:604` — `if (d.IsActive() && (d.type == ItemType.RELIC \|\| d.type == ItemType.MANUAL) && d.active.Count == 0)`
- `Assets/_Game/Scripts/Engine/Simulation.cs:142` — `this.type = type;`
- `Assets/_Game/Scripts/Engine/Simulation.cs:378` — `return def.type == ItemType.WEAPON;`
- `Assets/_Game/Scripts/UI/Common/ItemDescriber.cs:45` — `if (raw.type == ItemType.WEAPON)`
- `Assets/_Game/Scripts/UI/Common/ItemDescriber.cs:104` — `if (raw.type == ItemType.WEAPON)`
- `Assets/_Game/Scripts/UI/Common/ItemDescriber.cs:107` — `if (raw.type == ItemType.ARMOR \|\| raw.type == ItemType.RELIC)`
- `Assets/_Game/Scripts/UI/Common/ItemDescriber.cs:109` — `if (raw.type == ItemType.MANUAL)`
- `Assets/_Game/Scripts/UI/Common/SlicedAutoFit.cs:9` — `public sealed class SlicedAutoFit : UIBehaviour`
- `Assets/_Game/Scripts/UI/Common/SlicedAutoFit.cs:14` — `// This drives Image.pixelsPerUnitMultiplier (it is raised only when the borders would not fit),`
- `Assets/_Game/Scripts/UI/Common/SlicedAutoFit.cs:43` — `if (sprite == null \|\| image.type != Image.Type.Sliced)`
- `Assets/_Game/Scripts/UI/Common/SlicedAutoFit.cs:61` — `image.pixelsPerUnitMultiplier = Mathf.Max(preferredMultiplier, horizontal, vertical);`
- `Assets/_Game/Scripts/UI/Loading/LoadingPanel.cs:67` — `if (fill != null && fill.type == Image.Type.Filled)`
- `Assets/_Game/Scripts/UI/Popups/ResultPopup.cs:279` — `frame.type = Image.Type.Simple;`
- `Assets/_Game/Scripts/UI/Pve/DialoguePopup.cs:64` — `dialogueFrame.type = Image.Type.Simple;`
- `Assets/_Game/Scripts/UI/Pve/StageMapPopup.cs:373` — `image.type = Image.Type.Simple;`

## Fallbacks

No adjacent `ApplyUiSprite` fallback chains were found.

## Could not be measured

None.

## Sprite proposal summary

| Sprite | Class | New art files needed |
|---|---|---|
| `Assets/_Game/Textures2D/UI/Kit/button_primary_compact.png` | A | — |
| `Assets/_Game/Textures2D/UI/Kit/button_secondary_compact.png` | A | — |
| `Assets/_Game/Textures2D/UI/Kit/divider_brush.png` | A | — |
| `Assets/_Game/Textures2D/UI/Kit/dropdown_field.png` | A | — |
| `Assets/_Game/Textures2D/UI/Kit/list_row_compact.png` | D | — |
| `Assets/_Game/Textures2D/UI/Kit/popup_frame_wide.png` | A | — |
