import json
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open('One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json', 'r', encoding='utf-8') as f:
    chapters = json.load(f)

for ch in chapters.get("chapters", []):
    print(f"\n================ CHAPTER {ch.get('id')}: {ch.get('name')} ================")
    for st in ch.get("stages", []):
        sid = st.get("id")
        idx = st.get("index")
        sname = st.get("name")
        ename = st.get("enemy", {}).get("name")
        dialogue = st.get("dialogue", {})
        if isinstance(dialogue, dict):
            for timing, lines in dialogue.items():
                if isinstance(lines, list):
                    for line in lines:
                        if isinstance(line, dict):
                            print(f"  Stage {idx} [{timing}] {line.get('speaker')}: \"{line.get('text')}\"")
                        else:
                            print(f"  Stage {idx} [{timing}]: {line}")
                elif isinstance(lines, str):
                    print(f"  Stage {idx} [{timing}]: {lines}")
