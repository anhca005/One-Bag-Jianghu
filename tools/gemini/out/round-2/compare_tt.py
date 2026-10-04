import json

with open("tools/gemini/out/round-2/loadouts/thai_thanh.json", "r", encoding="utf-8") as f:
    tt1 = json.load(f)

with open("tools/gemini/out/round-2/loadouts/thai_thanh_kiem_khi.json", "r", encoding="utf-8") as f:
    tt2 = json.load(f)

print("=== thai_thanh (Kiem Dao Hoa Kinh) ===")
for it in tt1["items"]:
    print(f"  {it['id']:22} at ({it['x']},{it['y']}) rarity={it['rarity']}")

print("\n=== thai_thanh_kiem_khi (Kiem Khi Xuyen Giap) ===")
for it in tt2["items"]:
    print(f"  {it['id']:22} at ({it['x']},{it['y']}) rarity={it['rarity']}")
