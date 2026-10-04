# Lệnh cho Gemini (Antigravity): thiết kế Tâm Pháp mốc 3 / 6 / 9

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are a game designer helping on "One Bag Jianghu", a 2D wuxia backpack auto-battler (items of different sizes in a 5x4 bag fight automatically, both sides simultaneously). Design the **Tâm Pháp** (sect passive) milestones for all 7 sects and validate them with the battle simulator.

## Background (decision #15 in docs/decisions.md)
- Items are sect-neutral; a hero of any sect can use any weapon.
- A sect's only combat effect is its **Tâm Pháp**: a passive that buffs the weapon classes listed for that sect. One Tâm Pháp per sect, milestones at levels **1 / 3 / 6 / 9**, cumulative.
- Level 1 already exists for every sect: `+10% Lực Đạo` on its weapon classes. You design levels 3, 6 and 9.

## Hard rules
- Create ONLY these files:
  - `tools/gemini/out/sects.proposed.json` (full proposal: same format as the real file),
  - loadouts for testing under `tools/gemini/out/loadouts/` (copies of the samples with a different `tamPhapLevel`),
  - the report `docs/reviews/2026-09-27-gemini-tam-phap.md` (Vietnamese).
- Do NOT modify any existing file (`sects.json`, `items.json`, Java code, samples, docs). Do NOT run git.
- Use game terms from `docs/design/glossary.md`. No names from Kim Dung novels or other games.

## Read first
- `docs/design/items.md` (section "Môn phái và Tâm Pháp", weapon classes, effect `op` list, targets, Đột Phá), `docs/design/combat.md`, `docs/design/balance.md` (budget rules and all lessons learned, especially pass 3), `docs/decisions.md` #12–#15.
- `server/battle-engine/src/main/resources/data/sects.json` (current file and format), `items.json`, `server/battle-engine/samples/*.json` (15 loadouts, each has a `sect`).
- Java reference: `server/battle-engine/src/main/java/com/onebagjianghu/battle/data/TamPhapLevelDef.java` and `SectDef.java` (exactly how a milestone is applied).

## Milestone format
Each entry in `levels`: `{ "level": 3, "attackPct": 0.05, "cooldownPct": 0.05, "crit": 0.05, "ultimateCost": -5, "basic": [...], "ultimate": [...], "triggers": [...] }`. All fields optional except `level`. `attackPct` and `cooldownPct` are fractions and cumulative across milestones (total cooldownPct must stay < 0.5). `basic` / `ultimate` / `triggers` are appended to every buffed weapon, using the same effect/trigger JSON as items.json.

## Design goals
1. Each Tâm Pháp reinforces its sect's playstyle (items.md table). Level 3 = a stat step; level 6 = a distinct mechanic; level 9 = a signature effect that makes the sect feel different.
2. Budget: level 3 ≈ +5% value of the buffed weapons, level 6 ≈ +10%, level 9 ≈ +15% (cumulative ≈ +40% with level 1). Show brief arithmetic.
3. Avoid the known traps (balance.md): effects appended to every copy of a small weapon multiply (prefer `from: self` on triggers, prefer % stats for small weapons); extra build-up stacks create breakpoints at 50 (prefer speed/crit); no free Hóa Kình engines (no-ultimate weapons give ≤ 1 qi/s); control on all enemy items ≤ 1 s; Điều Tức is capped at 30/s.
4. Sects whose classes have no items yet (Chưởng, Phiến, Thương, Chùy, Tiên) still get a full design.

## Validate
From the `server` folder (Git Bash, or PowerShell with `.\mvnw.cmd` after `chcp 65001`):
```
./mvnw -q -pl battle-engine exec:java -Dexec.args="--matrix ../tools/gemini/out/loadouts/L9 --runs 200 --sects ../tools/gemini/out/sects.proposed.json"
```
(paths in `exec.args` are relative to `server/`). Build 4 folders `L1`, `L3`, `L6`, `L9` under `tools/gemini/out/loadouts/`, each holding all 15 samples with `tamPhapLevel` set to that value, and run the matrix for each with your proposed file. An invalid proposal prints `Lỗi: ...`; fix it.
Target: in every folder, no loadout's average (TB column) above 70%. Iterate on numbers until true. Loadouts below 35% are acceptable only if they are clearly weak layouts (explain).

## Report (`docs/reviews/2026-09-27-gemini-tam-phap.md`, Vietnamese, ≤ 300 lines)
1. Per sect: Tâm Pháp name (keep the current name or propose a better one), description of levels 1/3/6/9 in Vietnamese, budget arithmetic.
2. The 4 matrices' TB columns in one table (loadout × level).
3. What you changed while iterating and why.
4. Risks and open questions (max 5).
