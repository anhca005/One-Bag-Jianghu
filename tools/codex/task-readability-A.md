# Codex task RD-A: make the code readable (formatting only, no behaviour change)

Unity project: `D:\CONG VIEC\One-Bag-Jianghu\One-Bag-Jianghu`.

**Read `docs/tech/code-style.md` first.** It is the rule set. The user reads and edits this code and finds it too dense: many statements per line, 200-character lines, `if`/`for` bodies crammed onto one line.

## Your files (this batch only)
- `Assets/_Game/Scripts/Bag/**`, `Scripts/Core/**`, `Scripts/Economy/**`, `Scripts/Net/**`, `Scripts/Pve/**`, `Scripts/UI/**`
- `Assets/_Game/Editor/DataSyncMenu.cs`, `Assets/_Game/Editor/ItemIconImporter.cs`

Do not edit any other file. Other batches run in parallel on other folders. **Never touch `Assets/_Game/Editor/Building/**`**: the user asked to leave the builders as they are.

## Do
Reformat every file in scope until `py tools/lint/check_style.py One-Bag-Jianghu/Assets/_Game/Scripts/Bag One-Bag-Jianghu/Assets/_Game/Scripts/Core One-Bag-Jianghu/Assets/_Game/Scripts/Economy One-Bag-Jianghu/Assets/_Game/Scripts/Net One-Bag-Jianghu/Assets/_Game/Scripts/Pve One-Bag-Jianghu/Assets/_Game/Scripts/UI One-Bag-Jianghu/Assets/_Game/Editor/DataSyncMenu.cs One-Bag-Jianghu/Assets/_Game/Editor/ItemIconImporter.cs` reports **0 problems**:
- one statement per line;
- lines ≤ 120 characters (wrap long calls by argument);
- Allman braces;
- no `if`/`for`/`foreach`/`while` body on the same line, except a single `return`/`continue`/`break`/`throw` after a short `if`;
- split long inline lambdas into named private methods;
- in tests, separate Arrange / Act / Assert with blank lines (or comments).
- You may rename **local variables and private method parameters** that are 1-letter or cryptic (`c`, `p`, `s` → `catalog`, `profile`, `stage`). Loop indices `i/j/x/y` may stay.

## Must NOT change (formatting-only refactor)
- **Behaviour:** no logic, order of operations, condition or constant changes.
- **Names Unity or other code depends on:**
  - `[SerializeField]` fields, public members, class names, namespaces, enum values (renaming a serialized field breaks prefabs);
  - JSON property names and `[JsonProperty]` strings.
- **Public API signatures.**
- **Files:** do not move or rename them. `.meta` files are untouched.
- **Do not add features or "improve" logic while reformatting.** If you see a bug, list it in the result instead.
- **Comments:** keep existing ones. Add short *why* comments only where code was unclear.

## Check before you finish
- `py tools/lint/check_style.py One-Bag-Jianghu/Assets/_Game/Scripts/Bag One-Bag-Jianghu/Assets/_Game/Scripts/Core One-Bag-Jianghu/Assets/_Game/Scripts/Economy One-Bag-Jianghu/Assets/_Game/Scripts/Net One-Bag-Jianghu/Assets/_Game/Scripts/Pve One-Bag-Jianghu/Assets/_Game/Scripts/UI One-Bag-Jianghu/Assets/_Game/Editor/DataSyncMenu.cs One-Bag-Jianghu/Assets/_Game/Editor/ItemIconImporter.cs` → 0 problems; paste the last line in the result.
- The C# still compiles. Read your diff once looking for accidental logic changes, e.g. a statement moved out of an `if` when you split a line. That is the main risk.
- **No git. Do not run Unity**; Claude builds and runs all tests.

## Result
- The files changed and the problem count before → after.
- Any suspected bug you noticed (not fixed).
