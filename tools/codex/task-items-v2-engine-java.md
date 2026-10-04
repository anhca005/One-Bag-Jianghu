# Codex task X1: Items v2, Java engine mechanics + catalog conversion

Repo root: `D:\CONG VIEC\One-Bag-Jianghu`. Read first:
- `CLAUDE.md`
- `docs/decisions.md` **#32** (the approved list; it is the authority)
- `docs/design/combat.md`, `docs/design/glossary.md`
- `tools/gemini/out/items-v2/mechanics.md` (Gemini's semantics; #32 overrides it where they differ)
- `docs/reviews/2026-09-30-gemini-items-v2.md`
- the engine in `server/battle-engine/src/main/java/com/onebagjianghu/battle/**`

## Rules
- Edit `server/battle-engine/**` (main and test) and `server/README.md`. Create `tools/items/convert_items_v2.py` and the output `tools/gemini/out/items-v2/items-v2.normalized.json`.
- **Do NOT** replace the real `items.json` files (Claude swaps them after review). Do not edit the Unity project, docs (except `server/README.md`) or the Gemini outputs. **No git.**
- The simulation must stay deterministic: integer math, no new RNG calls unless needed, and RNG call order documented. The existing golden parity cases (`tools/parity/make_golden.py`) must still produce **identical** results with the **current** `items.json`. In other words, new mechanics must not change behaviour when unused. Prove this by regenerating goldens into a temp folder and diffing them with the committed ones in `One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Data/Parity`.
- Document every new rule in the Javadoc of the enum constant or field. A C# port follows in the next task, so keep the logic simple and explicit, with no Java-only library tricks.

## Mechanics to implement (decision #32)
Data model, JSON in lowercase snake case like the existing fields:

1. **Active cooldown for relics and manuals:** `ItemDef.cooldown` (seconds) and `ItemDef.active` (a list of `EffectDef`) for types `relic` and `manual`.
   - The item charges like a weapon: Khinh Công, Trì Trệ, Điểm Huyệt, Định Thân and `charge` all affect it the same way.
   - When full, it fires its `active` effects (no damage, no Nội Lực gain unless listed) and resets.
   - `active` can be extended by unlocks (add `active` to `UnlockDef`, append like `basic`).
   - Items with `cooldown = 0` stay passive, as today.
2. **New `TriggerEvent`s.** Say in Javadoc when each fires within the tick and what `from` (TriggerSource) means for it.
   - `DAMAGED`: own hero took > 0 damage from an enemy weapon hit or an enemy status burst (after Cương Khí). Not for Kiệt Lực.
   - `HEALED`: own hero gained > 0 HP (heal, regen tick, lifesteal).
   - `CONTROLLED`: an own item received Điểm Huyệt or Trì Trệ, or own hero received Định Thân.
   - `CRIT_TAKEN`: own hero took a Hội Tâm hit.
   - `ENEMY_ULTIMATE`: the enemy started a Tuyệt Kỹ. Fire it after the parry check. Say whether a parried one counts: it should count.
   - `QI_EMPTY`: own Nội Lực dropped to 0 from > 0.
   - `SHIELD_BROKEN`: own side reduced the enemy's Cương Khí from > 0 to 0.
   - `BURST_TAKEN`: a status burst hit own hero (optional `status` filter).

   Add a TriggerSource value **`ENEMY`** if Gemini's data needs `from: "enemy"`. It means the event was caused by the enemy. Map it where it makes sense, or reject it in validation where it does not.
3. **New `EffectOp`s:**
   - `BUFF_STACK`: params `stat` (`attack` | `crit` | `dmg_reduce`), `value` (int for attack; fraction for crit / dmg_reduce, stored as permille like the existing fields), `maxStacks`.
     - The stack counter lives on the effect's target (the item for item targets, the hero for `dmg_reduce`) and is keyed by the source item + trigger, so that two different items stack separately.
     - Each application adds one stack up to `maxStacks`. Once at the cap, the stat stops growing.
     - The `value` scales with tier × rarity through the existing `Scale` rule for attack. Crit and dmg_reduce are not scaled, matching the current `ADD_CRIT` and `DMG_REDUCE`. dmg_reduce still respects `maxDmgReducePermille`.
     - Emit BUFF events with the stack count.
   - `CLEANSE` (default target SELF):
     - Without `status`: remove Trì Trệ and Điểm Huyệt from all own items, and Định Thân from the hero.
     - With `status` in burn / poison / bleed: subtract `value` points from that accumulator (≥ 0). With `value` 0 it clears the accumulator.
     - Emit an event.
   - `LIFESTEAL` (target SELF): used inside `basic` or `ultimate` effect lists. It heals `pct` of the damage actually dealt (shield + HP) by the **preceding DAMAGE effect in the same list**, rounded down. Heal goes through the normal heal path, so it fires `HEALED`.
   - `RAISE_MAX_HP` (target SELF): max HP += scaled `value`, and HP += the same amount.
4. **Hậu Kình condition:** an optional `minQiAfter` (int) on `EffectDef`. Only valid inside `ultimate` effects. The effect runs only if the owner's Nội Lực after paying the cost is ≥ `minQiAfter`. It never runs when the Tuyệt Kỹ is parried, because parried ultimates do not resolve their effects.
5. **Liên Kích:** reuse the existing `hits` on DAMAGE. Check that each hit rolls Hội Tâm and fires HIT/CRIT separately, and document it. No new op.
6. **Rejected (do not implement):** `retaliate`, `status_threshold_reached`.

Also update `ItemCatalog` validation for every new field (reject unknown fields, as it already does), `LogFormatter` (Vietnamese wording from the glossary), and the event types if needed.

## Catalog conversion
Write `tools/items/convert_items_v2.py`. It reads `tools/gemini/out/items-v2/items-v2.json` and writes `items-v2.normalized.json` in the exact engine schema:
- Drop `_needs`.
- Map Gemini's trigger names to the enum names above: `damaged`, `healed`, `controlled`, `crit_taken`, `enemy_ultimate`, `qi_empty`, `shield_break_success` → `shield_broken`, `burst_taken`.
- `condition: "qi_remaining_ge_N"` → `minQiAfter: N`.
- Other ad-hoc fields: map them or fail loudly with the item id.
- `phuc_ma_giap`: replace `retaliate pct 0.15` with `{"op":"damage","value":12}` on its `damaged` trigger (the exact number will be balanced later).
- `giai_doc_dan`: replace `status_threshold_reached` with `burst_taken`.
- Print a list of every change the script made per item.

The normalized catalog must load in the engine with no validation errors, and the balance tool must run with it:

```
py tools/balance/balance.py matrix --items tools/gemini/out/items-v2/items-v2.normalized.json --tiers 1,9 --tp 1 --runs 50
```

Use the existing sample loadouts. If some samples reference items whose design changed, that is fine, as long as they run.

## Tests (JUnit)
For every new trigger and op, write at least one focused test using small inline catalogs, the way the existing tests do:
- relic active cooldown (including Trì Trệ/Khinh Công interaction)
- `BUFF_STACK` capping and separate stacks per source
- `CLEANSE` for both modes
- `LIFESTEAL` amount and the `HEALED` chain
- `RAISE_MAX_HP`
- `minQiAfter` true/false/parried
- each new trigger firing exactly once per cause
- `hits` rolling crit per hit

Plus a test that loads the normalized catalog.

## Self-check
- `cd server && ./mvnw -q test` passes.
- The golden diff with the current `items.json` is identical (describe the command you used).
- The balance matrix above runs. Paste its summary line in the result.
- In your result, list the new JSON fields and enum values exactly: the C# port and the Unity describer depend on it.
