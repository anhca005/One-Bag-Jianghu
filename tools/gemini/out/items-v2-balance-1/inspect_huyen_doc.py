import json
import sys

sys.stdout.reconfigure(encoding="utf-8")
catalog = json.load(open("tools/gemini/out/items-v2/items-v2.normalized.json", encoding="utf-8"))
items = {it["id"]: it for it in catalog}

check_ids = ["u_minh_bi_luc", "van_doc_dinh", "huyet_ngoc_thach", "that_tinh_tieu", "tu_kim_trao"]
for cid in check_ids:
    it = items[cid]
    print(f"=== {it['id']} ({it['name']}) ===")
    print("triggers:", json.dumps(it.get("triggers", []), ensure_ascii=False))
    print("unlocks:", json.dumps(it.get("unlocks", []), ensure_ascii=False))
