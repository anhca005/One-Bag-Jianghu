import json
import glob
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")


samples = sorted(glob.glob("server/battle-engine/samples/*.json"))
print(f"Total samples: {len(samples)}")
for s in samples:
    with open(s, "r", encoding="utf-8") as f:
        data = json.load(f)
    items = [it["id"] for it in data["items"]]
    p = Path(s).name
    print(f"{p:32} | {data.get('sect'):12} | {data.get('name')}")
    print(f"   Items: {', '.join(items)}")
