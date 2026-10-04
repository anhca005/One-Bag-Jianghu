import json
import glob

for p in sorted(glob.glob("tools/gemini/out/round-2/balance/enemy/*.json")):
    with open(p, encoding="utf-8") as f:
        d = json.load(f)
    print(f"{p.split('\\')[-1]:15} hp={d.get('hp')} sect={d.get('sect')} items={len(d.get('items', []))}")
