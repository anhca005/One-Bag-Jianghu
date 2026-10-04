import json
import shutil
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

# Reset loadouts to the exact step2 state
# In step2:
# thai_thanh had thanh_tam_dan instead of kiem_tong_tam_yeu
# new_thiet_phu_hammer had chuy_phong (1 hammer + 1 axe + 1 fist)
# hybrid_thiet_phu_liet_diem had thiet_phu_ld
# kim_cang_bach_luyen had 2 fists + 1 staff + tu_kim_bat + o_tam_y + thiet_giap_y
# hybrid_han_son_bich_thuy was original from samples

# Let's inspect step2 loadouts:
print("Resetting to step2 base...")
# Copy original samples first
for f in (ROOT / "server" / "battle-engine" / "samples").glob("*.json"):
    shutil.copy(f, LOADOUTS_DIR / f.name)

# Apply thai_thanh kiem_tong replacement
tt = json.load(open(LOADOUTS_DIR / "thai_thanh.json", encoding="utf-8"))
for it in tt["items"]:
    if it["id"] == "kiem_tong_tam_yeu":
        it["id"] = "thanh_tam_dan"
        it["rarity"] = "uncommon"
with open(LOADOUTS_DIR / "thai_thanh.json", "w", encoding="utf-8") as f:
    json.dump(tt, f, indent=2)

# Apply new_thiet_phu_hammer
chuy_phong = {
  "name": "Thiết Phủ Bang - Chấn Nhạc Chùy Phong",
  "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "chan_nhac_chuy", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "khai_son_phu", "x": 0, "y": 2, "rarity": "epic"},
    {"id": "thiet_cot_quyen_sao", "x": 2, "y": 0, "rarity": "rare"},
    {"id": "bach_luyen_chan_kinh", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "o_thiet_giap", "x": 3, "y": 0, "rarity": "epic"},
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
    {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}
with open(LOADOUTS_DIR / "new_thiet_phu_hammer.json", "w", encoding="utf-8") as f:
    json.dump(chuy_phong, f, indent=2)

# Apply kim_cang_bach_luyen with 3 fists + 1 staff or 2 fists + 1 staff + moderate armor
# Let's make sure it has balanced defense: 2 fists + 1 staff + o_tam_y + tu_kim_bat (NO thiet_giap_y)
kc_bl = {
  "name": "Kim Cang Tự - Bách Luyện Thuần Quyền",
  "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "phuc_ho_con", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "thiet_cot_quyen_sao", "x": 1, "y": 2, "rarity": "rare"},
    {"id": "bach_luyen_chan_kinh", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "tu_kim_bat", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "cuu_chuyen_hoan_hon_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 3, "y": 2, "rarity": "uncommon"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"},
    {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 3, "rarity": "common"}
  ]
}
with open(LOADOUTS_DIR / "kim_cang_bach_luyen.json", "w", encoding="utf-8") as f:
    json.dump(kc_bl, f, indent=2)

# Apply huyen_doc: replace 1 claw with da_ho_y
for hf in ["huyen_doc.json", "huyen_doc_van_doc.json"]:
    hd = json.load(open(LOADOUTS_DIR / hf, encoding="utf-8"))
    for it in hd["items"]:
        if it["id"] == "tu_kim_trao" and it["x"] == 1 and it["y"] == 2:
            it["id"] = "da_ho_y"
            it["rarity"] = "rare"
    with open(LOADOUTS_DIR / hf, "w", encoding="utf-8") as f:
        json.dump(hd, f, indent=2)

print("Base setup complete. Testing fast matrix...")
cmd = [
    "py", "tools/balance/balance.py", "matrix",
    "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", "40", "--level", "10",
    "--loadouts", str(LOADOUTS_DIR)
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
for line in res.stdout.splitlines():
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
        print("  FL:", l.strip())
