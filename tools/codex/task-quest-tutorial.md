# Codex task: tutorial steps for the quest tracker (Hành Tẩu Lục)

Repo root: `D:/CONG VIEC/One-Bag-Jianghu`; Unity project `One-Bag-Jianghu/` (Unity 6000.3.9f1, UGUI). Code in English,
UI text Vietnamese. **Read `docs/tech/code-style.md` and follow it.** Paths below are under `Assets/_Game/`.

Tutorial system: `Resources/Data/tutorial.json` (sequences of steps: `line`, `target`, `completeEvent`),
`Scripts/Onboarding/*` (`OnboardingRuntime.Publish(eventId)`, `TutorialService`, `TutorialOverlay`, `TutorialTarget`
components with an id the overlay points at). Quest UI: `Scripts/UI/Home/QuestTracker.cs`,
`Scripts/UI/Popups/QuestLogPopup.cs`, `Editor/Building/QuestUiBuilder.cs`; spec `docs/design/quests.md`.

## Flow wanted
The quest steps go **inside the `shop` sequence, between its first step (close the map) and "Ghé Thương Hội"**, so
the first quest (q1_01, done by the first battle) is claimed and q1_02 "Mua 1 món ở Thương Hội" is active before the
tutorial purchase (otherwise that purchase is not counted). New steps (Vietnamese lines, Lục Tiều Phu's voice, short):
1. target `home_quest` (the tracker): "Khung này là Hành Tẩu Lục, ghi việc cần làm tiếp theo. Việc đầu đã xong nên
   khung đang sáng. Bấm vào xem." → completeEvent `quest_log_opened`.
2. target `quest_claim` (the "Nhận thưởng" button of the done row in the log): "Bấm Nhận thưởng để lấy Bạc, EXP và
   nguyên liệu. Nhận xong, việc kế tiếp sẽ mở ra." → completeEvent `quest_claimed`.
3. target `quest_log_close` (the log's close button): "Việc kế là sắm đồ. Khi chưa xong việc, cứ bấm vào khung, ta sẽ
   dẫn ngươi tới đúng chỗ." → completeEvent `quest_log_closed`.
Then the existing "Có Bạc rồi. Ghé Thương Hội xem hàng." and the purchase step continue.

## Do
- Edit `tutorial.json` as above (keep everything else unchanged).
- Publish `quest_log_opened` when `QuestLogPopup` shows, `quest_claimed` after a successful claim (from the log or
  anywhere else claims happen), `quest_log_closed` when the log closes.
- `TutorialTarget` ids: `home_quest` on the tracker root (builder), `quest_log_close` on the log's close button
  (builder), `quest_claim` on the claim button of the active done row (rows are created at runtime: add/configure the
  component there; make sure the overlay can find it after the row list is built, e.g. build rows before publishing
  `quest_log_opened`, or refresh the overlay).
- While this tutorial step points at the tracker, clicking the tracker must open the log even if the code would
  otherwise run `goto` (the quest is done at that point, so the normal rule already opens the log — verify).
- Edge: if the player already claimed q1_01 before this step (e.g. old save), steps 1–3 must not block: when the
  active quest is not done at step 2, auto-complete steps 2 (publish `quest_claimed` immediately when the log opens
  and nothing is claimable), so the sequence still reaches the shop steps.
- Tests (EditMode): the shop sequence contains the three new steps in order; publishing the three events advances
  it; the edge case above does not block.

## Check
- `py tools/lint/check_style.py <every C# file you changed>` until clean; `dotnet build` (temporary `.csproj` copy for
  new files if needed, delete it and its `obj/` caches afterwards).
- **Do not launch Unity.** No git. Do not touch scenes or existing `.prefab` files (the user re-runs
  "Quest UI (Overwrite)"). Do not edit `docs/`.

## Report (`-o` output)
Files changed, where each event is published, where each target id lives, test results of the build.
