import json
import subprocess
import os
import sys
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")
R6 = ROOT / "tools" / "gemini" / "out" / "round-6"
LOADOUTS_DIR = R6 / "loadouts"

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

# 1. Fix han_son_bang_tam.json: replace 1 brush with offensive spear or damage relic
# Size: phan_quan_but is 1x2. Replace with bang_phach_thuong (1x4 at col 1? Col 1 has 4 rows!)
# Col 0: truy_hon_cung (1x3) + thanh_tam_dan (1x1)
# Col 1: bang_phach_thuong (1x4) at (1,0)
# Col 2: han_ngoc_tieu (1x2) at (2,0), han_bang_chau (1x1) at (2,2), boi_nguyen_dan (1x1) at (2,3)
# Col 3,4: bang_tam_giap (2x1) at (3,0), nho_sam_y (2x1) at (3,1), o_tam_y (2x2) at (3,2)
han_son_bt = {
  "name": "Hàn Sơn Phái - Băng Tâm Bút Cung",
  "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "truy_hon_cung", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thanh_tam_dan", "x": 0, "y": 3, "rarity": "uncommon"},
    {"id": "bang_phach_thuong", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "han_ngoc_tieu", "x": 2, "y": 0, "rarity": "rare"},
    {"id": "han_bang_chau", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "bang_tam_giap", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "nho_sam_y", "x": 3, "y": 1, "rarity": "rare"},
    {"id": "o_tam_y", "x": 3, "y": 2, "rarity": "epic"}
  ]
}

# 2. Fix kim_cang.json (Hộ Thể Côn Quyền: was 95% > 45s)
# Replace 1 fist with tiger claws or offensive gear
# Col 0: phuc_ho_con (1x4)
# Col 1: thiet_cot_quyen_sao (1x2) at (1,0), tu_kim_bat (2x1) at (1,2) -> wait tu_kim_bat is 2x1
# Let's inspect original kim_cang.json:
# phuc_ho_con at (0,0), thiet_cot_quyen_sao at (1,0), thiet_cot_quyen_sao at (1,2)
# bach_luyen_chan_kinh (1x2) at (2,0), tu_kim_bat (2x1) at (2,2), cuu_chuyen_hoan_hon_dan (1x1) at (2,3)
# thiet_giap_y (2x1) at (3,0), thanh_dong_ho_tam_kinh (1x1) at (3,1), boi_nguyen_dan (1x1) at (3,3)
# o_tam_y (2x2) at (3,2)? wait, col 3 has 2x2 o_tam_y at (3,2..3, 4,2..3)
# Replace thiet_cot_quyen_sao at (1,2) with bach_ho_cuong_mong (2x1) or offensive relic:
kim_cang_ht = {
  "name": "Kim Cang Tự - Hộ Thể Côn Quyền",
  "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "phuc_ho_con", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "bach_ho_cuong_mong", "x": 1, "y": 2, "rarity": "rare"},     # (1,2, 2,2) 2x1
    {"id": "cuu_chuyen_hoan_hon_dan", "x": 1, "y": 3, "rarity": "uncommon"},# (1,3) 1x1
    {"id": "bach_luyen_chan_kinh", "x": 2, "y": 0, "rarity": "epic"},    # (2,0..1) 1x2
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},       # (2,3) 1x1
    {"id": "thiet_giap_y", "x": 3, "y": 0, "rarity": "rare"},            # (3,0, 4,0) 2x1
    {"id": "tu_kim_bat", "x": 3, "y": 1, "rarity": "epic"},              # (3,1, 4,1) 2x1
    {"id": "o_tam_y", "x": 3, "y": 2, "rarity": "epic"}                  # (3,2..3, 4,2..3) 2x2
  ]
}

# 3. Fix hybrid_han_son_bich_thuy.json:
# Replace zither (2x2) with spear or whip to eliminate the 50s stall
# Col 0: han_ngoc_tieu (1x2) at (0,0), phan_quan_but (1x2) at (0,2)
# Col 1: thanh_dang_tien (1x3) at (1,0), tram_huong_lo (1x1) at (1,3)
# Col 2: duong_sinh_quyet (1x2) at (2,0), thanh_tam_dan (1x1) at (2,2), boi_nguyen_dan (1x1) at (2,3)
# Col 3,4: bang_tam_giap (2x1) at (3,0), nho_sam_y (2x1) at (3,1), o_tam_y (2x2) at (3,2)
han_bich_fast = {
  "name": "Hàn Sơn Phái - Hàn Bích Hợp Bích",
  "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "han_ngoc_tieu", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "phan_quan_but", "x": 0, "y": 2, "rarity": "epic"},
    {"id": "thanh_dang_tien", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "tram_huong_lo", "x": 1, "y": 3, "rarity": "epic"},
    {"id": "duong_sinh_quyet", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "thanh_tam_dan", "x": 2, "y": 2, "rarity": "uncommon"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "bang_tam_giap", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "nho_sam_y", "x": 3, "y": 1, "rarity": "rare"},
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

to_save = [
    ("han_son_bang_tam.json", han_son_bt),
    ("kim_cang.json", kim_cang_ht),
    ("hybrid_han_son_bich_thuy.json", han_bich_fast),
]

for fname, data in to_save:
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
