import json
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open('One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

chars = data.get('characters', [])
print("Characters defined in chapters.json (type:", type(chars), "):")
for c in chars:
    print(" ", c)

char_ids = {c.get('id'): c.get('name') for c in chars if isinstance(c, dict)}

all_speakers = set()
for ch in data.get('chapters', []):
    for st in ch.get('stages', []):
        dialogue = st.get('dialogue', {})
        for timing, lines in dialogue.items():
            if isinstance(lines, list):
                for line in lines:
                    all_speakers.add(line.get('speaker'))

print("\nAll speakers used in dialogues:")
for spk in sorted(all_speakers):
    name = char_ids.get(spk)
    print(f"  {spk}: exists={spk in char_ids} (Name: {name})")
