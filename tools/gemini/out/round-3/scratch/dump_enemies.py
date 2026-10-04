import json, sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")


c = json.load(open("tools/gemini/out/round-2/chapters.json", "r", encoding="utf-8"))
for ch in c["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        e = st["enemy"]
        item_names = [f"{it['id']}(T{it.get('tier',1)}_{it.get('rarity','com')})" for it in e["items"]]
        print(f"Stage {idx:02d} ({st['kind']}): {e['name']} - {', '.join(item_names)}")
