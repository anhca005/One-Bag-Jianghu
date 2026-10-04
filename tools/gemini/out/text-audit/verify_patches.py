import json
import re

def get_by_path(data, path):
    tokens = re.findall(r'[^.\[\]]+', path)
    curr = data
    for i, t in enumerate(tokens):
        if i == 0 and isinstance(curr, list) and t in ('items', 'materials', 'sects'):
            continue
        if isinstance(curr, list):
            curr = curr[int(t)]
        elif isinstance(curr, dict):
            curr = curr[t]
        else:
            raise KeyError(f"Cannot traverse {t} in {curr}")
    return curr

patches = [
    ('One-Bag-Jianghu/Assets/_Game/Resources/Data/items.json', 'tools/gemini/out/text-audit/items.patch.json'),
    ('One-Bag-Jianghu/Assets/_Game/Resources/Data/materials.json', 'tools/gemini/out/text-audit/materials.patch.json'),
    ('One-Bag-Jianghu/Assets/_Game/Resources/Data/sects.json', 'tools/gemini/out/text-audit/sects.patch.json'),
    ('One-Bag-Jianghu/Assets/_Game/Resources/Data/shop.json', 'tools/gemini/out/text-audit/shop.patch.json'),
    ('One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json', 'tools/gemini/out/text-audit/chapters.patch.json'),
]

all_ok = True
for data_file, patch_file in patches:
    with open(data_file, 'r', encoding='utf-8') as f:
        data = json.load(f)
    with open(patch_file, 'r', encoding='utf-8') as f:
        patch = json.load(f)
    for entry in patch:
        path = entry['path']
        old = entry['old']
        new = entry['new']
        actual = get_by_path(data, path)
        if actual != old:
            print(f"MISMATCH in {data_file} at {path}:")
            print(f"  Expected old: '{old}'")
            print(f"  Actual:       '{actual}'")
            all_ok = False

if all_ok:
    print("ALL PATCH ENTRIES VERIFIED AND MATCH CURRENT DATA PERFECTLY!")
