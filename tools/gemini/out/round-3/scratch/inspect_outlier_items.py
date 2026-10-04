import json, sys
if hasattr(sys.stdout, "reconfigure"): sys.stdout.reconfigure(encoding="utf-8", errors="replace")

items = json.load(open("tools/gemini/out/round-3/items-base.json", encoding="utf-8"))
items_by_id = {it["id"]: it for it in items}

check_ids = [
    "that_tinh_tieu", "tu_kim_trao", "u_minh_quy_trao", "kich_doc_chu_nga",
    "bang_phach_thuong", "khai_son_phu", "chan_nhac_chuy", "bach_ho_cuong_mong"
]

for cid in check_ids:
    it = items_by_id.get(cid)
    if it:
        print(f"\n=== {it['id']} ({it['name']}, {it['type']}) ===")
        print(f"Attack: {it.get('attack')}, Cooldown: {it.get('cooldown')}, Size: {it.get('size')}")
        print(f"Stats: {it.get('stats')}")
        print(f"Basic: {it.get('basic')}")
        print(f"Triggers: {it.get('triggers')}")
        print(f"Unlocks: {it.get('unlocks')}")
