import re

# Helper to enrich afterLoss if only 1 line
def fix_file(path, guide_name):
    with open(path, "r", encoding="utf-8") as f:
        content = f.read()

    # Pattern where afterLoss has only 1 speaker
    # e.g.:
    # "afterLoss": [
    #     {"speaker": "...", "text": "..."}
    # ]
    # We want to ensure hero also has a line:
    # {"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."},
    # followed by the guide's advice.
    lines = content.splitlines()
    new_lines = []
    i = 0
    while i < len(lines):
        line = lines[i]
        new_lines.append(line)
        if '"afterLoss": [' in line:
            # check next lines
            j = i + 1
            entry_count = 0
            while j < len(lines) and ']' not in lines[j]:
                if '{"speaker":' in lines[j]:
                    entry_count += 1
                j += 1
            if entry_count == 1:
                # Add hero line before the single line
                indent = "                "
                hero_line = f'{indent}{{"speaker": "hero", "text": "Chiêu thức đối phương quá hung hiểm, ta sơ suất rồi..."}},'
                new_lines.append(hero_line)
        i += 1

    with open(path, "w", encoding="utf-8") as f:
        f.write("\n".join(new_lines) + "\n")
    print(f"Fixed {path}")

fix_file("tools/gemini/out/pve/balance/chapter_1_data.py", "lao_tieu_phu")
fix_file("tools/gemini/out/pve/balance/chapter_2_data.py", "tieu_linh_nhi")
fix_file("tools/gemini/out/pve/balance/chapter_3_data.py", "doc_co_hanh")
