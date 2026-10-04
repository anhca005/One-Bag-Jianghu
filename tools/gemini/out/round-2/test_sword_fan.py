import json, subprocess
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"
CATALOG = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"

with open(CATALOG, "r", encoding="utf-8") as f:
    cat = {it["id"]: it for it in json.load(f)}

def validate_grid(lb):
    cols = lb.get("cols", 5)
    rows = lb.get("rows", 4)
    grid = {}
    for it in lb["items"]:
        iid = it["id"]
        w, h = cat[iid]["size"]
        x, y = it["x"], it["y"]
        for dx in range(w):
            for dy in range(h):
                pos = (x + dx, y + dy)
                if pos[0] >= cols or pos[1] >= rows or pos[0] < 0 or pos[1] < 0:
                    return False, f"OOB {iid} at {pos}"
                if pos in grid:
                    return False, f"Overlap at {pos}: {grid[pos]} & {iid}"
                grid[pos] = iid
    return True, f"OK ({len(grid)}/20)"

# Let's test thai_thanh_kiem_khi with sword + fan
thai_thanh_kiem_khi = {
  "name": "Thái Thanh Quán - Kiếm Khí Xuyên Giáp",
  "hp": 1200, "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1,
  "items": [
    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thai_thanh_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
    {"id": "thanh_phong_kiem", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "kiem_tong_tam_yeu", "x": 1, "y": 3, "rarity": "epic"},
    {"id": "ngoc_cot_phien", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "thien_tam_chuoi", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "bach_ngoc_boi", "x": 3, "y": 2, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "epic"},
    {"id": "hoi_thien_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

ok, msg = validate_grid(thai_thanh_kiem_khi)
assert ok, f"Invalid: {msg}"
with open(LOADOUTS / "thai_thanh_kiem_khi.json", "w", encoding="utf-8") as f:
    json.dump(thai_thanh_kiem_khi, f, indent=2, ensure_ascii=False)
print("Updated thai_thanh_kiem_khi to sword + fan build.")

cmd = [
    "py", "tools/balance/balance.py", "whatif",
    "--loadouts", str(LOADOUTS),
    "--patch", str(ROOT / "tools" / "gemini" / "out" / "round-2" / "test_patch.json"),
    "--items", str(CATALOG),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "50",
    "--low", "35", "--high", "65"
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
out_path = ROOT / "tools" / "gemini" / "out" / "round-2" / "test_matrix_out.txt"
with open(out_path, "w", encoding="utf-8") as f:
    f.write(res.stdout)
print("Matrix run finished.")
