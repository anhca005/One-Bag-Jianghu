import json
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open('One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for c_idx, ch in enumerate(data['chapters']):
    for s_idx, st in enumerate(ch['stages']):
        ename = st['enemy']['name']
        desc = st['description']
        all_text = desc + " " + " ".join([l.get('text', '') for timing, lines in st.get('dialogue', {}).items() if isinstance(lines, list) for l in lines])
        # check if enemy name or significant words appear
        words = [w for w in ename.split() if len(w) > 2]
        found = any(w.lower() in all_text.lower() for w in words)
        print(f"Ch {c_idx+1} St {st['index']} ({st['id']}): Enemy '{ename}' -> Found in text: {found}")
        if not found:
            print(f"  --> Potential mismatch! Desc: '{desc}'")
