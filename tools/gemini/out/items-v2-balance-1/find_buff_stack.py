import json
import sys

sys.stdout.reconfigure(encoding="utf-8")
cat = json.load(open("tools/gemini/out/items-v2/items-v2.normalized.json", encoding="utf-8"))

for it in cat:
    s = json.dumps(it, ensure_ascii=False)
    if "buff_stack" in s:
        print(f"=== {it['id']} ({it['name']}) ===")
        for t in it.get("triggers", []):
            for e in t.get("effects", []):
                if e.get("op") == "buff_stack":
                    print("  trigger:", e)
        for u in it.get("unlocks", []):
            for t in u.get("triggers", []):
                for e in t.get("effects", []):
                    if e.get("op") == "buff_stack":
                        print(f"  unlock T{u.get('tier')}:", e)
