import json, sys
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
sys.path.insert(0, str(ROOT / "tools" / "balance"))
from balance import item_parameter, load_patch

BASE_ITEMS = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"
PATCH_FILE = ROOT / "tools" / "gemini" / "out" / "round-2" / "patch.json"
FINAL_ITEMS = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"

# Load trial 2e patch as final patch
with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "test_patch.json", "r", encoding="utf-8") as f:
    patch = json.load(f)

# Save as round-2/patch.json
with open(PATCH_FILE, "w", encoding="utf-8") as f:
    json.dump(patch, f, indent=2, ensure_ascii=False)
print(f"Saved patch.json ({len(patch)} entries)")

# Apply patch to items-base.json to create items-v2-final.json
with open(BASE_ITEMS, "r", encoding="utf-8") as f:
    items = json.load(f)

for address, value in patch.items():
    if address.startswith("sect:"):
        continue
    item_parameter(items, address, value, set_value=True)

with open(FINAL_ITEMS, "w", encoding="utf-8") as f:
    json.dump(items, f, indent=2, ensure_ascii=False)
print(f"Saved items-v2-final.json ({len(items)} items)")
