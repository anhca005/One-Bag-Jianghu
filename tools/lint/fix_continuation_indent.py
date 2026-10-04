"""Indent wrapped lines that a formatter left at column 0.

Usage (from the repo root):
  py tools/lint/fix_continuation_indent.py PATH [PATH ...] [--check]

A C# file in this project wraps everything in a block namespace, so any code line at column 0 that is not
a top-level declaration is a continuation of the statement above it. Such lines get the indentation of the
last indented line plus four spaces. Lines inside multi-line verbatim (@"...") or raw (\"\"\"...\"\"\")
strings are left untouched, because spaces there would change the string.
"""
import argparse
import re
import sys
from pathlib import Path

TOP_LEVEL = re.compile(
    r"^(using |namespace |public |internal |sealed |static |abstract |partial |class |enum |struct |"
    r"interface |delegate |record |file |\{|\}|#|\[|//|/\*|\*)")


def string_state_after(line: str, in_verbatim: bool, in_raw: bool):
    """Track whether a multi-line verbatim or raw string is still open after this line."""
    i = 0
    while i < len(line):
        if in_raw:
            end = line.find('"""', i)
            if end < 0:
                return False, True
            in_raw = False
            i = end + 3
            continue
        if in_verbatim:
            if line[i] == '"':
                if i + 1 < len(line) and line[i + 1] == '"':
                    i += 2
                    continue
                in_verbatim = False
            i += 1
            continue
        if line.startswith("//", i):
            break
        if line.startswith('"""', i):
            in_raw = True
            i += 3
            continue
        if line.startswith('@"', i) or line.startswith('$@"', i) or line.startswith('@$"', i):
            in_verbatim = True
            i += line.index('"', i) - i + 1
            continue
        if line[i] == '"':
            i += 1
            while i < len(line) and line[i] != '"':
                i += 2 if line[i] == "\\" else 1
            i += 1
            continue
        if line[i] == "'":
            i += 1
            while i < len(line) and line[i] != "'":
                i += 2 if line[i] == "\\" else 1
            i += 1
            continue
        i += 1
    return in_verbatim, in_raw


CODE_STRINGS = re.compile(r'@"(?:[^"]|"")*"|"(?:[^"\\]|\\.)*"|\'(?:[^\'\\]|\\.)*\'')


def brace_balance(line: str) -> int:
    """Net '{' minus '}' on a line, ignoring braces inside string and char literals and comments."""
    code = CODE_STRINGS.sub('""', line.split("//", 1)[0])
    return code.count("{") - code.count("}")


def indent_of(line: str) -> int:
    return len(line) - len(line.lstrip(" "))


def fix(text: str):
    lines = text.split("\n")
    out = []
    base = 0
    in_verbatim = in_raw = False
    changed = 0
    previous_code = ""
    # Initializer blocks a formatter left at column 0: each entry is [shift, open braces still unmatched].
    blocks = []
    for line in lines:
        inside_string = in_verbatim or in_raw
        stripped = line.rstrip("\r")
        if not inside_string and stripped.strip():
            opens_block = (stripped.startswith("{") and base > 0
                           and not previous_code.lstrip().startswith("namespace "))
            if opens_block:
                # Align the opening brace with the line that introduced it, keep the block's inner offsets.
                shift = indent_of(out[-1]) if out else base
                blocks.append([shift, 0])
            if blocks:
                line = " " * blocks[-1][0] + line
                changed += 1
                blocks[-1][1] += brace_balance(stripped)
                if blocks[-1][1] <= 0:
                    blocks.pop()
            elif not stripped[0].isspace() and not TOP_LEVEL.match(stripped) and base > 0:
                line = " " * (base + 4) + line
                changed += 1
            else:
                base = indent_of(stripped)
            previous_code = stripped
        out.append(line)
        in_verbatim, in_raw = string_state_after(stripped, in_verbatim, in_raw)
    return "\n".join(out), changed


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("paths", nargs="+")
    parser.add_argument("--check", action="store_true", help="report only, do not write")
    args = parser.parse_args()
    files = []
    for raw in args.paths:
        path = Path(raw)
        files += sorted(path.rglob("*.cs")) if path.is_dir() else [path]
    total = 0
    for file in files:
        text = file.read_bytes().decode("utf-8-sig")
        fixed, changed = fix(text)
        if not changed:
            continue
        total += changed
        print(f"{changed:5}  {file}")
        if not args.check:
            had_bom = file.read_bytes().startswith(b"\xef\xbb\xbf")
            file.write_text(fixed, encoding="utf-8-sig" if had_bom else "utf-8", newline="")
    print(f"\n{total} line(s) {'to fix' if args.check else 'fixed'}.")
    return 1 if (args.check and total) else 0


if __name__ == "__main__":
    sys.exit(main())
