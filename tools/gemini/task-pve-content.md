# Lệnh cho Gemini: nội dung Hành Tẩu Giang Hồ (3 Hồi × 10 ải)

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

Nên dán **sau** khi xong lệnh `task-items-v2-balance-1.md`, vì phần vật phẩm quái cần dùng bộ vật phẩm đã cân bằng.

---

You are the narrative and level designer of "One Bag Jianghu", a wuxia backpack auto-battler with a Thủy Mặc ink-wash art style.

Read first:
- `CLAUDE.md`
- `docs/design/pve.md`: the plan and the **exact data format** you must follow
- `docs/decisions.md` #32 and #33
- `docs/design/gdd.md`, `docs/design/glossary.md`, `docs/design/combat.md`, `docs/design/balance.md`
- `server/battle-engine/src/main/resources/data/sects.json` (7 sects)
- The item catalog: use `tools/gemini/out/items-v2-balance-1/items-v2-balanced.json` if it exists, otherwise `tools/gemini/out/items-v2/items-v2.normalized.json`
- The art style references in `docs/art/` (prompts files, `style-options.md`)

## Hard rules
- **Original content.** No names, places, sects or plots from Kim Dung novels or other existing works. Use no famous real martial-arts names. Invent the world, the people and the places. Vietnamese Hán-Việt style names fit the game.
- Use only the glossary terms (Khí Huyết, Nội Lực, Tuyệt Kỹ, Hóa Kình, Cương Khí, Hồi, ải…).
- Do NOT modify real data, code or docs. No git. Create ONLY:
  - `docs/reviews/2026-10-01-gemini-pve-content.md` (Vietnamese report, ≤ 300 lines)
  - `tools/gemini/out/pve/`:
    - `chapters.json` in the exact format of `docs/design/pve.md`
    - `monster-items.json` in the catalog format
    - `balance/`: loadouts and tables
    - `art-brief.md`

## 1. World and overall plot
- A one-page story for the whole game (the 7 sects, the main conflict, the hero's journey). Write it so it can grow past Hồi 3.
- Name 4–6 recurring characters: allies, a rival and the main villain, each with a role and a personality.

## 2. Three chapters, each with its own identity
For each Hồi, give:
- The setting: region, season and mood. The 3 must contrast clearly, for example misty river town / frozen mountain pass / burning desert fort, but invent your own.
- The story arc in 10 beats, one beat per stage.
- **A UI theme:**
  - `accent`, `paper` and `ink` colours that stay readable and fit Thủy Mặc;
  - a map description: what the stage-select painting shows, and where the path winds;
  - a battle background description;
  - node icon ideas for normal, elite and boss stages.
- **The enemy mix:**
  - mostly human fighters of 2–3 sects that fit the region;
  - beasts or yokai at about 2–3 stages per Hồi, plus the boss where it fits.
- Each Hồi teaches a mechanic through its enemies:
  - Hồi 1: basics (Hóa Kình, Cương Khí);
  - Hồi 2: statuses and burst;
  - Hồi 3: control and relics.

  Adapt this if your story suggests better.

## 3. Thirty stages
For every stage give all the fields of the format:
- id, name, kind, `mapPos`, description, `staminaCost`
- `enemy`: name, character, portrait key, sect, `tamPhapLevel`, `hp`, and a full 5×4 bag of valid item ids with x, y, tier and rarity
- rewards: `firstClear`, `drops`
- dialogue `before`, `afterWin`, `afterLoss`: 2–5 short lines each. Bosses and story stages may have more. Keep lines short enough for a dialogue box (≤ 120 characters).

**Difficulty curve:** a new player starts with the starter bag (`One-Bag-Jianghu/Assets/_Game/Resources/Data/starter.json`) and grows through rewards and the shop.
- Hồi 1 stages 1–3 must be beatable with the starter bag.
- The boss of Hồi 3 should need roughly Trọng 4–5 Tinh Phẩm items.

Give the intended player power per stage in a table, and **verify** it: simulate each stage enemy against a "expected player" loadout at that point. Build those loadouts yourself, and state the expected win rate. Use:

```
py tools/balance/balance.py vs --a <player-loadouts-folder> --b <enemy-loadouts-folder> --items <catalog> --runs 100
```

This command keeps each item's own `tier` and `rarity` from the loadout JSON, unlike `matrix`, which overrides them. Read `tools/balance/README.md`. If `vs` does not exist yet, stop and report it; Codex is adding it.

## 4. Monster items, shared with players (10–15 items)
- Thematic items carried by the beasts and bosses: claws, fangs, hides, venom sacs, spirit pearls…
- They use **only approved mechanics** (decision #32) and the normal item format. Each needs an `id`, a `name`, a `type`, a `size`, Đột Phá unlocks, and must fit the balance rules.
- They must be fun for players too: each one should open a build idea that does not exist yet.
- Validate them in the same matrices as the normal items, with at least 2 sample player builds that use them, and report the tables verbatim.
- List them in `chapters.json` → `monsterItems`.

## 5. Art brief (`art-brief.md`)
A list of every image the content needs, with the file key used in `chapters.json` and a one-paragraph visual description that respects the Thủy Mặc style:
- 3 maps
- 3 battle backgrounds
- node icons
- NPC, boss and monster portraits (half-body, 1024×1536, facing left for enemies)
- monster item icons

Claude turns it into ChatGPT prompts. Keep it as small as the content allows: reuse `enemy_portrait_1..3` for generic human enemies.

## Report
- Story summary and characters.
- Per-chapter identity table: setting, colours, enemies, mechanic taught.
- Stage list table: stage, name, kind, enemy, expected player power, expected win rate.
- The monster items table and their balance tables (verbatim).
- Art list counts.
- At most 5 open questions.
