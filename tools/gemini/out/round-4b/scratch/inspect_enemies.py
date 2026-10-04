import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8") as f:
    doc = json.load(f)

for c in doc["chapters"]:
    print(f"=== Chapter {c['id']}: {c['name']} ===")
    for s in c["stages"]:
        e = s["enemy"]
        item_names = [f"{it['id']}(T{it.get('tier')},{it.get('rarity')})" for it in e["items"]]
        print(f"Stage {s['id']}: {s['name']} ({s['kind']}) | Enemy: {e.get('name')} (lvl {e.get('level')}, sect {e.get('sect')}, TP {e.get('tamPhapLevel')}) | items({len(e['items'])}): {', '.join(item_names)}")
