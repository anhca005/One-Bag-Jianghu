Implemented the Hành Tẩu Lục tutorial flow.

Files changed:

- `Assets/_Game/Resources/Data/tutorial.json`
- `Assets/_Game/Scripts/Onboarding/TutorialService.cs`
- `Assets/_Game/Scripts/Onboarding/OnboardingRuntime.cs`
- `Assets/_Game/Scripts/Quests/QuestService.cs`
- `Assets/_Game/Scripts/UI/Popups/QuestLogPopup.cs`
- `Assets/_Game/Editor/Building/QuestUiBuilder.cs`
- `Assets/_Game/Tests/EditMode/Onboarding/OnboardingTests.cs`

Events:

- `quest_log_opened`: published after `QuestLogPopup` builds its rows.
- `quest_claimed`: published by `QuestService.Claim` after any successful claim.
- `quest_log_closed`: published from `QuestLogPopup.OnHidden`.
- When no quest is claimable, opening the log immediately advances both `quest_log_opened` and `quest_claimed`.

Targets:

- `home_quest`: tracker prefab root, configured by `QuestUiBuilder`.
- `quest_log_close`: log close button, configured by `QuestUiBuilder`.
- `quest_claim`: added at runtime to the active completed row’s claim button.

Verification:

- Style lint: **0 problems in 6 changed C# files**.
- Temporary game/editor/test project build: **succeeded, 0 errors, 0 warnings**.
- Added EditMode coverage for step ordering/event advancement and the already-claimed edge case.
- `dotnet test` cannot execute Unity EditMode tests outside Unity’s test runner; Unity was not launched as requested.
- Temporary project files and their generated `obj` cache entries were removed.
- No scenes, prefabs, or documentation were modified.