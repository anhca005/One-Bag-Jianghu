"""Readability check for C# and Java sources (see docs/tech/code-style.md).

Usage (from the repo root):
  py tools/lint/check_style.py PATH [PATH ...] [--max 120] [--summary]

PATH may be a file or a folder (searched recursively for *.cs and *.java).
Exit code 1 when any problem is found.
"""
import argparse
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent))
from fix_continuation_indent import fix as fix_indent  # noqa: E402

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

# A "for (a; b; c)" header legitimately holds two semicolons.
FOR_HEADER = re.compile(r"\bfor\s*\(([^()]|\([^()]*\))*\)")
STRING = re.compile(r'@?"(?:[^"\\]|\\.)*"|\'(?:[^\'\\]|\\.)*\'')
INLINE_BODY = re.compile(r"^\s*(?:else\s+)?(?:if|for|foreach|while)\s*\(.*\)\s*\{.*;.*\}\s*$")
SINGLE_JUMP = re.compile(r"^\s*if\s*\(.*\)\s*(?:return|continue|break|throw)\b[^;]*;\s*$")


def statements(line: str) -> int:
    code = line.split("//", 1)[0]
    code = STRING.sub('""', code)
    code = FOR_HEADER.sub("for()", code)
    return code.count(";")


def check(path: Path, max_len: int):
    problems = []
    for number, line in enumerate(path.read_text(encoding="utf-8", errors="replace").splitlines(), 1):
        stripped = line.strip()
        if stripped.startswith(("//", "*", "/*", "///")):
            continue
        if len(line) > max_len:
            problems.append((number, f"line is {len(line)} chars (> {max_len})"))
        if statements(line) >= 2:
            problems.append((number, f"{statements(line)} statements on one line"))
        elif INLINE_BODY.match(line) and not SINGLE_JUMP.match(line):
            problems.append((number, "if/for/while body on the same line"))
    if path.suffix == ".cs":
        original = path.read_bytes().decode("utf-8-sig").split("\n")
        indented = fix_indent("\n".join(original))[0].split("\n")
        for number, (before, after) in enumerate(zip(original, indented), 1):
            if before != after:
                problems.append((number, "wrapped line at column 0 (run tools/lint/fix_continuation_indent.py)"))
    return problems


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("paths", nargs="+")
    parser.add_argument("--max", type=int, default=120)
    parser.add_argument("--summary", action="store_true", help="one line per file instead of every problem")
    args = parser.parse_args()
    files = []
    for raw in args.paths:
        path = Path(raw)
        if path.is_dir():
            files += sorted(p for p in path.rglob("*") if p.suffix in (".cs", ".java"))
        elif path.suffix in (".cs", ".java"):
            files.append(path)
    total = 0
    for file in files:
        problems = check(file, args.max)
        if not problems:
            continue
        total += len(problems)
        if args.summary:
            print(f"{len(problems):5}  {file}")
        else:
            for number, message in problems:
                print(f"{file}:{number}: {message}")
    print(f"\n{total} problem(s) in {len(files)} file(s) checked.")
    return 1 if total else 0


if __name__ == "__main__":
    sys.exit(main())
