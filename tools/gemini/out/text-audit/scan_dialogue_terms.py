import json
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open('One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json', 'r', encoding='utf-8') as f:
    chapters = json.load(f)

terms_to_check = ['sinh lực', 'cương khí', 'hồi phục', 'nộ ', 'nộ,', 'nộ.', 'võ hiệp', 'côn lôn', 'thiếu lâm', 'võ đang', 'hoa sơn', 'nga mi', 'cái bang', 'đoàn dự', 'tiêu dao']

for c_idx, ch in enumerate(chapters.get('chapters', [])):
    cid = ch.get('id')
    for s_idx, st in enumerate(ch.get('stages', [])):
        s_id = st.get('id')
        s_name = st.get('name')
        s_desc = st.get('description', '')
        # check desc
        for t in terms_to_check:
            if t in s_desc.lower():
                print(f"[DESC] Ch {cid} St {st.get('index')} ({s_name}): found '{t}' in desc: \"{s_desc}\"")
                print(f"  Path: chapters[{c_idx}].stages[{s_idx}].description")

        dialogue = st.get('dialogue', {})
        for timing, lines in dialogue.items():
            if isinstance(lines, list):
                for l_idx, line in enumerate(lines):
                    txt = line.get('text', '')
                    spk = line.get('speaker', '')
                    for t in terms_to_check:
                        if t in txt.lower():
                            print(f"[DIALOGUE] Ch {cid} St {st.get('index')} [{timing}][{l_idx}] ({spk}): found '{t}' in \"{txt}\"")
                            print(f"  Path: chapters[{c_idx}].stages[{s_idx}].dialogue.{timing}[{l_idx}].text")
