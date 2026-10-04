import json
from pathlib import Path
from collections import Counter

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
with open(ROOT / "tools/gemini/out/round-2/items-v2-final.json", encoding="utf-8") as f:
    items = {it["id"]: it for it in json.load(f)}

loadouts_dir = ROOT / "tools/gemini/out/round-2/loadouts"
for p in sorted(loadouts_dir.glob("*.json")):
    d = json.loads(p.read_text(encoding="utf-8"))
    counts = Counter()
    cells = Counter()
    for placed in d["items"]:
        it = items[placed["id"]]
        t = it["type"]
        c = it["size"][0] * it["size"][1]
        counts[t] += 1
        cells[t] += c
    print(f"{p.stem:<30} armor:{cells['armor']}c({counts['armor']}) relic:{cells['relic']}c({counts['relic']}) manual:{cells['manual']}c({counts['manual']}) pill:{cells['pill']}c({counts['pill']}) weapon:{cells['weapon']}c({counts['weapon']})")
