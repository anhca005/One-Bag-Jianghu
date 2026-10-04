# Lệnh cho Gemini: thiết kế bảng âm thanh cho từng vật phẩm và sự kiện

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity (chọn model nào cũng được), dán toàn bộ phần dưới dấu `---`.

---

You are the sound designer of "One Bag Jianghu" (Nhất Nang Giang Hồ), a Vietnamese wuxia bag-packing auto-battler with an ink-wash (Thủy Mặc) look. The plan is in `docs/audio/sfx-plan.md`. **Read it fully first.** It contains:
- a catalogue of about 60 sound keys with prompts;
- the planned `BattleSfxResolver`, which maps battle events to keys by weapon class.

Sounds will be generated later with a text-to-sound-effect tool, at 1 credit per second. Nothing is generated now.

## Read also
- `docs/design/combat.md`: event types and statuses.
- `One-Bag-Jianghu/Assets/_Game/Scripts/Battle/Replay/BattleEventType.cs`: battle event types.
- `server/battle-engine/src/main/resources/data/items.json`: all 73 items, with `type`, `weaponClass`, effects and statuses.
- `One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json`: enemies (for boss and creature sounds).
- `docs/design/glossary.md`: terms.

## Tasks
1. **Per-item mapping.** For every item, decide:
   - `attackKey` for its basic trigger: usually `atk_<weaponClass>`. Non-weapons that trigger (pills, manuals, relics) need a fitting key, e.g. `use_pill`, `use_manual`, `use_relic`;
   - `hitKey`: `hit_metal` / `hit_blunt` / other;
   - optional `ultimateKey`, when the item's ultimate deserves its own signature sound. Keep these few, at most 10 across the game, for the most iconic items;
   - optional `statusKey` override, e.g. a fire weapon whose burn should sound different.
   Every key you use must exist in the plan or in your additions (task 2).
2. **Catalogue review.**
   - **Add** missing keys the game needs, with a prompt in the same style as the plan: under 0.8 s for battle sounds, "dry, close, no music", natural materials, no electronic sounds. Candidates:
     - non-weapon triggers;
     - sect-themed ultimates;
     - creature or monster attacks for PvE enemies;
     - boss entrance;
     - map node unlock;
     - star rating.
   - **Remove or merge** keys that are redundant.
   - **Fix** prompts likely to give bad results: too vague, too long, or likely to produce music.
   - **Keep the total at or below 80 keys.**
3. **Priority and budget.** Assign A/B/C, where A is the minimum set for a playtest, about 30 keys. Estimate credits: duration × variants, plus 4 ambience loops of 20 s.
4. **Mixing notes.** Give relative volume per group in dB, e.g. UI −6 vs hits 0, and a max-concurrent rule per key. Also say which sounds must not play at ×4 replay speed.

## Rules
- **Create only:**
  - `tools/gemini/out/audio/sfx-map.json` (format below);
  - `tools/gemini/out/audio/sfx-catalog.json`: the full final catalogue as `[{ "key", "group", "priority", "durationSec", "variants", "loop", "prompt", "event" }]`;
  - `docs/reviews/2026-10-08-gemini-sfx-map.md`: Vietnamese, ≤ 200 lines.
- **Do not edit** game data, code, `docs/audio/sfx-plan.md` (Claude merges your result into it), or git.
- **Names:** no names from Kim Dung novels or reference games in keys or prompts.

`sfx-map.json` format:
```json
{
  "items": { "<itemId>": { "attackKey": "atk_sword", "hitKey": "hit_metal", "ultimateKey": null, "statusKey": null } },
  "enemies": { "<enemyId or name>": { "voiceKey": null } },
  "events": { "Shield": "shield_gain", "Heal": "heal" }
}
```

## Report
- A table of changes to the catalogue: added, removed or merged, with one line of reason each.
- The 10 signature ultimates and why each was chosen.
- The budget: credits for sets A, B, C and the total.
- At most 3 open questions for the user.
