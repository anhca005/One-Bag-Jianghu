import json, sys
from pathlib import Path

# Add tools/balance to sys.path
ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
sys.path.insert(0, str(ROOT / "tools" / "balance"))

from balance import item_parameter, load_patch, sect_parameter

def export_final_catalog(base_path, patch_path, out_path):
    with open(base_path, "r", encoding="utf-8") as f:
        items = json.load(f)
    patch = load_patch(patch_path)
    
    for address, value in patch.items():
        if address.startswith("sect:"):
            continue
        item_parameter(items, address, value, set_value=True)
        
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(items, f, indent=2, ensure_ascii=False)
    print(f"Exported final items catalog ({len(items)} items) to {out_path}")

if __name__ == "__main__":
    export_final_catalog(
        ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json",
        ROOT / "tools" / "gemini" / "out" / "round-2" / "test_patch.json",
        ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"
    )
