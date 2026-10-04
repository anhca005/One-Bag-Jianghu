# Lệnh cho Gemini: vòng 4b, làm lại tiến trình quái PvE

Mở thư mục `D:\CONG VIEC\One-Bag-Jianghu` trong Antigravity, dán toàn bộ phần dưới dấu `---`.

---

You are the balance designer of "One Bag Jianghu". Read:
- `docs/decisions.md` #33, #34, #39 and #40;
- your round-4 report `docs/reviews/2026-10-04-gemini-round-4-pvp.md`;
- `docs/design/pve.md`;
- `tools/balance/pve_check.py`.

## Status after round 4 (Claude verified)
- **Accepted and now the game data:**
  - your round-4 items: `server/battle-engine/src/main/resources/data/items.json`;
  - your config values, which are now the engine defaults. Do not pass `--battle-config`.
- **Re-measured by Claude:**
  - fights now average 23–29 s, with 5–19% of fights over 45 s (measured per fight, not per matchup);
  - matrix A: 103/192 cells out;
  - tier-step: 61.1%.
- Your round-4 `chapters.json` is in the game **temporarily**. Rebuild first: `cd server && ./mvnw -q test`.
- **Why PvE is not accepted:** `py tools/balance/pve_check.py` (200 runs per stage, read straight from `chapters.json`) reports **27/30 stages breaking a rule**:
  - **Bands at 100 runs sat on the edge.** At 200 runs, 10 stages fall outside, for example stage 2 at 91.2%, stage 10 at 49.8%, stage 15 at 72.0% and stage 25 at 57.0%.
  - **Enemy levels make no sense as a journey:**
    - stage 6 is level 13, then stage 7 is level 2;
    - the Hồi 2 boss is level 11 while stage 19 is level 25;
    - stages 23 and 28 are level 36–37, above the hero cap of 30;
    - the bosses are not "player + 1 level" as decision #39 requires.
  - **Enemy rarity/tier make no sense either:**
    - stage 4 has a Chí Tôn T3 item at level 3;
    - the Hồi 3 boss uses Phàm items while normal stages use Chí Tôn.
  - **Some fights are long:** stage 1 takes 42 s (it is the tutorial), and stages 2, 14, 16, 24, 28 and 30 take 45–53 s.

## Goal
Make the 30 stages feel like a **progression**: enemies grow steadily stronger, and the difficulty lands in the bands **robustly**.

## Hard rules (checked by `pve_check.py`; Claude re-runs it)
- **Win rate at `--runs 200`, inside the band with a 2-point margin:**
  - normal 82–88%;
  - elite 62–68%;
  - boss 52–58%.
- **Average fight length per stage ≤ 40 s.** Stages 1–2 should be ≤ 30 s, because they are the tutorial.
- **Enemy level:**
  - boss = `recommendedLevel + 1`;
  - other stages within `recommendedLevel ± 2`;
  - never above 31.
  - Enemy level should not go down from one stage to the next by more than 2.
- **Tier:** the enemy's top Trọng must be within the player's top Trọng ± 1.
- **Rarity:** the enemy's top phẩm must be from one step below the player's top phẩm up to:
  - +1 step for normal stages;
  - +2 steps for elite and boss stages.
- **Tuning knobs:**
  - **use:** enemy item choice and count, positions, Trọng, phẩm (within the rules), `tamPhapLevel` and `sect`;
  - **do not touch:** `items.json`, the engine config, stage ids, names, stories, dialogue and rewards;
  - **keep:** each enemy's identity (beast vs. martial artist, its sect theme).
- **Expected player bags** (`tools/gemini/out/round-4/balance/player/stage_NN.json`):
  - keep stages 1–5, which already follow the starter kit and rewards;
  - you may revise stages 6–30, but they must stay a plausible path:
    - level = stage number;
    - Trọng and phẩm never decrease;
    - a `sect` only from level 10 on;
    - derived from rewards, drops and Thương Hội purchases;
  - explain in one line per Hồi.
- Answer to your round-4 question 2: **raise enemy gear, not enemy level.** Your questions 1 and 3 go to the user; leave them out of this round.

## Do
1. Copy `tools/gemini/out/round-4/balance/player/` to `tools/gemini/out/round-4b/player/`, then edit there.
2. Write `tools/gemini/out/round-4b/chapters.json`: the game file with only `enemy` blocks changed.
3. Iterate until this prints `Stages breaking a rule: 0/30`:
   `py tools/balance/pve_check.py --chapters tools/gemini/out/round-4b/chapters.json --players tools/gemini/out/round-4b/player --runs 200`
4. Save that final output verbatim to `tools/gemini/out/round-4b/pve_check.txt`.

## Hard limits
- Create ONLY:
  - `tools/gemini/out/round-4b/`;
  - `docs/reviews/2026-10-05-gemini-round-4b-pve.md` (Vietnamese, ≤ 150 lines).
- No git. Do not edit game data, code or docs.

## Report
- A met / not-met line for each hard rule. If any rule could not be met, say so and name the stage.
- The final `pve_check` table, verbatim.
- One line per Hồi describing how enemies grow: level, Trọng and phẩm from the first stage to the boss.
- At most 3 open questions.
