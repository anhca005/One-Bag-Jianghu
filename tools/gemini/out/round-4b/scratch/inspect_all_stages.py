import json, sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path(__file__).resolve().parents[5]
with open(ROOT / "tools/gemini/out/round-4b/chapters.json", encoding="utf-8") as f:
    doc = json.load(f)

stages = [s for c in doc["chapters"] for s in c["stages"]]
for i, s in enumerate(stages, 1):
    e = s["enemy"]
    items_summary = [f"{it['id']}(T{it.get('tier')},{it.get('rarity')})" for it in e["items"]]
    print(f"Stage {i:02d} [{s['kind']:6}]: {s['name']} | E: {e['name']} (lvl {e['level']}, sect {e.get('sect')}, TP {e.get('tamPhapLevel')})")
    print(f"   Items({len(e['items'])}): {', '.join(items_summary)}")
