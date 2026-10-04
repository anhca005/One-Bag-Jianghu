from pathlib import Path

txt_path = Path("tools/gemini/out/round-2/test_matrix_out.txt")
with open(txt_path, "r", encoding="utf-8", errors="replace") as f:
    lines = f.readlines()

for line in lines:
    safe = line.encode("ascii", "replace").decode("ascii")
    print(safe, end="")
