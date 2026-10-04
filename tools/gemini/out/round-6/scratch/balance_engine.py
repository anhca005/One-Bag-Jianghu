import json
import subprocess
import os
import sys
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R6 = ROOT / "tools" / "gemini" / "out" / "round-6"
LOADOUTS_DIR = R6 / "loadouts"

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()
env = os.environ.copy()
env["PYTHONUTF8"] = "1"

ITEMS = json.load(open(ROOT / "server/battle-engine/src/main/resources/data/items.json", encoding="utf-8"))
ITEM_MAP = {it["id"]: it for it in ITEMS}

def check_layout(items_list):
    grid = [[None for _ in range(4)] for _ in range(5)]
    for it in items_list:
        iid = it["id"]
        x, y = it["x"], it["y"]
        meta = ITEM_MAP.get(iid)
        if not meta:
            return False, f"Unknown item {iid}"
        w, h = meta["size"]
        if x < 0 or y < 0 or x + w > 5 or y + h > 4:
            return False, f"Out of bounds: {iid} at ({x},{y}) size ({w},{h})"
        for dx in range(w):
            for dy in range(h):
                if grid[x + dx][y + dy] is not None:
                    return False, f"Overlap at ({x+dx},{y+dy}) between {grid[x+dx][y+dy]} and {iid}"
                grid[x + dx][y + dy] = iid
    return True, "OK"

# 1. han_son_bang_tam: 1 bow (1x3), 1 spear (1x4), 1 brush (1x2), 1 flute (1x2)
han_son_bt = {
  "name": "Hàn Sơn Phái - Băng Tâm Bút Cung",
  "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "truy_hon_cung", "x": 0, "y": 0, "rarity": "epic"},          # (0,0..2) 1x3
    {"id": "thanh_tam_dan", "x": 0, "y": 3, "rarity": "uncommon"},      # (0,3) 1x1
    {"id": "bang_phach_thuong", "x": 1, "y": 0, "rarity": "rare"},      # (1,0..3) 1x4
    {"id": "han_ngoc_tieu", "x": 2, "y": 0, "rarity": "rare"},          # (2,0..1) 1x2
    {"id": "han_bang_chau", "x": 2, "y": 2, "rarity": "epic"},          # (2,2) 1x1
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},      # (2,3) 1x1
    {"id": "phan_quan_but", "x": 3, "y": 0, "rarity": "rare"},          # (3,0..1) 1x2
    {"id": "bang_tam_giap", "x": 3, "y": 2, "rarity": "rare"},          # (3,2, 4,2) 2x1
    {"id": "o_tam_y", "x": 4, "y": 0, "rarity": "rare"},                # (4,0..1) wait, o_tam_y is 2x2!
  ]
}
# Wait! o_tam_y is 2x2, so it cannot fit at (4,0)!
# Let's fix col 3,4 in han_son_bang_tam:
# Col 3,4 Rows 0,1: o_tam_y (2x2) at (3,0)
# Col 3 Rows 2,3: phan_quan_but (1x2) at (3,2)
# Col 4 Rows 2: bang_tam_giap (wait, bang_tam_giap is 2x1! Needs width 2!)
# If bang_tam_giap is 2x1, it takes (x, x+1; y).
# Let's design col 3,4 properly:
# (3,0, 4,0): bang_tam_giap (2x1)
# (3,1, 4,1): nho_sam_y (2x1)
# (3,2..3): phan_quan_but (1x2)
# (4,2): tuy_tien_ho_lo (1x1)
# (4,3): van_khi_dan (1x1)
han_son_bt_clean = {
  "name": "Hàn Sơn Phái - Băng Tâm Bút Cung",
  "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "truy_hon_cung", "x": 0, "y": 0, "rarity": "epic"},          # (0,0..2) 1x3
    {"id": "thanh_tam_dan", "x": 0, "y": 3, "rarity": "uncommon"},      # (0,3) 1x1
    {"id": "bang_phach_thuong", "x": 1, "y": 0, "rarity": "epic"},      # (1,0..3) 1x4
    {"id": "han_ngoc_tieu", "x": 2, "y": 0, "rarity": "rare"},          # (2,0..1) 1x2
    {"id": "han_bang_chau", "x": 2, "y": 2, "rarity": "epic"},          # (2,2) 1x1
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},      # (2,3) 1x1
    {"id": "bang_tam_giap", "x": 3, "y": 0, "rarity": "rare"},          # (3,0, 4,0) 2x1
    {"id": "nho_sam_y", "x": 3, "y": 1, "rarity": "rare"},              # (3,1, 4,1) 2x1
    {"id": "phan_quan_but", "x": 3, "y": 2, "rarity": "rare"},          # (3,2..3) 1x2
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},         # (4,2) 1x1
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}         # (4,3) 1x1
  ]
}

# 2. bich_thuy_duong_sinh: 1 zither + 2 whips
bich_thuy_ds_clean = {
  "name": "Bích Thủy Am - Dưỡng Sinh Cầm Âm",
  "cols": 5, "rows": 4, "sect": "bich_thuy", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_dang_tien", "x": 0, "y": 0, "rarity": "epic"},         # (0,0..2) 1x3
    {"id": "kim_sang_duoc", "x": 0, "y": 3, "rarity": "uncommon"},      # (0,3) 1x1
    {"id": "thanh_dang_tien", "x": 1, "y": 0, "rarity": "rare"},         # (1,0..2) 1x3
    {"id": "boi_nguyen_dan", "x": 1, "y": 3, "rarity": "uncommon"},      # (1,3) 1x1
    {"id": "duong_sinh_quyet", "x": 2, "y": 0, "rarity": "epic"},        # (2,0..1) 1x2
    {"id": "tram_huong_lo", "x": 2, "y": 2, "rarity": "epic"},           # (2,2) 1x1
    {"id": "tuy_tien_ho_lo", "x": 2, "y": 3, "rarity": "rare"},          # (2,3) 1x1
    {"id": "bich_thuy_cam", "x": 3, "y": 0, "rarity": "rare"},           # (3,0..1, 4,0..1) 2x2
    {"id": "tue_nguyet_bao_y", "x": 3, "y": 2, "rarity": "rare"}         # (3,2..3, 4,2..3) 2x2
  ]
}

# 3. hybrid_han_son_bich_thuy: flute + brush + whip + moderate armor
han_bich_clean = {
  "name": "Hàn Sơn Phái - Hàn Bích Hợp Bích",
  "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "han_ngoc_tieu", "x": 0, "y": 0, "rarity": "epic"},           # (0,0..1) 1x2
    {"id": "phan_quan_but", "x": 0, "y": 2, "rarity": "epic"},           # (0,2..3) 1x2
    {"id": "thanh_dang_tien", "x": 1, "y": 0, "rarity": "epic"},         # (1,0..2) 1x3
    {"id": "tram_huong_lo", "x": 1, "y": 3, "rarity": "epic"},           # (1,3) 1x1
    {"id": "duong_sinh_quyet", "x": 2, "y": 0, "rarity": "epic"},        # (2,0..1) 1x2
    {"id": "thanh_tam_dan", "x": 2, "y": 2, "rarity": "uncommon"},       # (2,2) 1x1
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},      # (2,3) 1x1
    {"id": "bang_tam_giap", "x": 3, "y": 0, "rarity": "rare"},           # (3,0, 4,0) 2x1
    {"id": "nho_sam_y", "x": 3, "y": 1, "rarity": "rare"},               # (3,1, 4,1) 2x1
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},                 # (3,2..3) 1x2
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},          # (4,2) 1x1
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}          # (4,3) 1x1
  ]
}

# Validate layouts
for name, data in [("han_son_bang_tam.json", han_son_bt_clean),
                   ("bich_thuy_duong_sinh.json", bich_thuy_ds_clean),
                   ("hybrid_han_son_bich_thuy.json", han_bich_clean)]:
    ok, msg = check_layout(data["items"])
    if not ok:
        print(f"ERROR: {name}: {msg}")
        sys.exit(1)
    with open(LOADOUTS_DIR / name, "w", encoding="utf-8") as f:
        json.dump(data, f, indent=2)
    print(f"Saved {name}")

print("\nAll 3 loadouts saved. Measuring Matrix A and Fight Length...")
cmd_mat = [
    "py", "tools/balance/balance.py", "matrix",
    "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", "40", "--level", "10",
    "--loadouts", str(LOADOUTS_DIR)
]
res_mat = subprocess.run(cmd_mat, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
for line in res_mat.stdout.splitlines():
    if "Outside [" in line:
        print("Matrix A:", line)

cmd_fl = [
    "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
    "tools/gemini/out/round-6/scratch/FightLengthRunner.java",
    "--loadouts", str(LOADOUTS_DIR)
]
res_fl = subprocess.run(cmd_fl, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
for l in res_fl.stdout.splitlines():
    if "Trọng" in l or "Toàn cục" in l:
        print(" ", l.strip())
