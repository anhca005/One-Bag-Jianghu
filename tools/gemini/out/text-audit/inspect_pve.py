import json
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open('One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for c_idx, ch in enumerate(data['chapters']):
    for s_idx, st in enumerate(ch['stages']):
        print(f"=== Ch {c_idx+1} Stage {st['index']} ({st['id']}) ===")
        print(f"  Name: {st['name']}")
        print(f"  Enemy: {st['enemy']['name']}")
        print(f"  Desc: {st['description']}")
        for timing, lines in st.get('dialogue', {}).items():
            if isinstance(lines, list):
                for line in lines:
                    print(f"    [{timing}] {line.get('speaker')}: {line.get('text')}")
