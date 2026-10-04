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

# 1. kim_cang_bach_luyen.json: Reduce defensive armor, add offensive Tiger Claws (bach_ho_cuong_mong 2x1)
# (0,0..3): phuc_ho_con (1x4)
# (1,0..1): thiet_cot_quyen_sao (1x2)
# (1,2..3): thiet_cot_quyen_sao (1x2)
# (2,0..1): bach_luyen_chan_kinh (1x2)
# (2,2, 3,2): bach_ho_cuong_mong (2x1) -> white tiger claws, adds high attack!
# (2,3): cuu_chuyen_hoan_hon_dan (1x1)
# (3,0..1, 4,0..1): o_tam_y (2x2)
# (3,3): boi_nguyen_dan (1x1)
# (4,2): tuy_tien_ho_lo (1x1)
# (4,3): van_khi_dan (1x1)
bach_luyen_fast = {
  "name": "Kim Cang Tự - Bách Luyện Thuần Quyền",
  "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "phuc_ho_con", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 2, "rarity": "rare"},
    {"id": "bach_luyen_chan_kinh", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "bach_ho_cuong_mong", "x": 2, "y": 2, "rarity": "rare"},
    {"id": "cuu_chuyen_hoan_hon_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

# 2. bich_thuy_duong_sinh.json: 2 whips + 1 zither (replaces 2 zithers with 2 whips)
# (0,0..2): thanh_dang_tien (1x3)
# (0,3): kim_sang_duoc (1x1)
# (1,0..2): thanh_dang_tien (1x3)
# (1,3): boi_nguyen_dan (1x1)
# (2,0..1): duong_sinh_quyet (1x2)
# (2,2): tram_huong_lo (1x1)
# (2,3): tuy_tien_ho_lo (1x1)
# (3,0..1, 4,0..1): bich_thuy_cam (2x2)
# (3,2..3, 4,2..3): tue_nguyet_bao_y (2x2)
duong_sinh_fast = {
  "name": "Bích Thủy Am - Dưỡng Sinh Cầm Âm",
  "cols": 5, "rows": 4, "sect": "bich_thuy", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_dang_tien", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "kim_sang_duoc", "x": 0, "y": 3, "rarity": "uncommon"},
    {"id": "thanh_dang_tien", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 1, "y": 3, "rarity": "uncommon"},
    {"id": "duong_sinh_quyet", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "tram_huong_lo", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "tuy_tien_ho_lo", "x": 2, "y": 3, "rarity": "rare"},
    {"id": "bich_thuy_cam", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "tue_nguyet_bao_y", "x": 3, "y": 2, "rarity": "rare"}
  ]
}

# 3. Restore claws to huyen_doc.json so it deals active damage and doesn't drag out
import shutil
shutil.copy(ROOT / "server/battle-engine/samples/huyen_doc.json", LOADOUTS_DIR / "huyen_doc.json")
shutil.copy(ROOT / "server/battle-engine/samples/huyen_doc_van_doc.json", LOADOUTS_DIR / "huyen_doc_van_doc.json")

# Save bach_luyen and duong_sinh
for fname, data in [("kim_cang_bach_luyen.json", bach_luyen_fast), ("bich_thuy_duong_sinh.json", duong_sinh_fast)]:
    ok, msg = check_layout(data["items"])
    if not ok:
        print(f"ERROR in {fname}: {msg}")
        sys.exit(1)
    with open(LOADOUTS_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
    print(f"Saved {fname}")

print("\nRunning FightLengthRunner to check tier > 45s...")
cmd = [
    "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
    "tools/gemini/out/round-6/scratch/FightLengthRunner.java",
    "--loadouts", str(LOADOUTS_DIR)
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
print(res.stdout)
