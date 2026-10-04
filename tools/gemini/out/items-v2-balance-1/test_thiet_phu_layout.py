import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
loadouts_dir = ROOT / "tools/gemini/out/items-v2-balance-1/loadouts"

# Check old thiet_phu_khai_thien.json in server/battle-engine/samples
old_tp = json.load(open(ROOT / "server/battle-engine/samples/thiet_phu_khai_thien.json", encoding="utf-8"))
print("Old thiet_phu_khai_thien items:")
for it in old_tp["items"]:
    print(f"  {it['id']} at ({it['x']}, {it['y']})")
