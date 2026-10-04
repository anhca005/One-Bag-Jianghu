import json
import sys
import copy
from pathlib import Path

sys.path.insert(0, str(Path("tools/balance").resolve()))
import balance

items_base = balance.read_json(Path("tools/gemini/out/round-3/items-base.json"))
sects_base = balance.read_json(Path("server/battle-engine/src/main/resources/data/sects.json"))
patch = balance.read_json(Path("tools/gemini/out/round-3/patch.json"))

items_final = copy.deepcopy(items_base)
sects_final = copy.deepcopy(sects_base)

for addr, val in patch.items():
    balance.parameter(items_final, sects_final, {}, addr, val, set_value=True)

out_path = Path("tools/gemini/out/round-3/items-v3-final.json")
balance.write_json(out_path, items_final, indent=2)
print(f"Generated {out_path} with {len(items_final)} items.")
