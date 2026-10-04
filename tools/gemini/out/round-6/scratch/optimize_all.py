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

def evaluate_loadout_set(sects_path=None, runs=30):
    cmd = [
        "py", "tools/balance/balance.py", "matrix",
        "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", str(runs), "--level", "10",
        "--loadouts", str(LOADOUTS_DIR)
    ]
    if sects_path:
        cmd += ["--sects", str(sects_path)]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
    outs = 999
    rows = []
    for line in res.stdout.splitlines():
        if "Outside [" in line:
            outs = int(line.split(":")[1].split("/")[0].strip())
        elif line and not line.startswith("Loadout") and not line.startswith("-") and not line.startswith("Outside") and "%" in line:
            parts = line.split()
            name = " ".join([p for p in parts if not p.endswith("%") and not p.endswith("%!")])
            vals = [p for p in parts if p.endswith("%") or p.endswith("%!")]
            rows.append((name, vals))
    return outs, rows

def save_loadout(fname, data):
    ok, msg = check_layout(data["items"])
    if not ok:
        raise ValueError(f"{fname}: {msg}")
    with open(LOADOUTS_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

# Loadout variations to try:

# 1. thai_thanh.json: 2 swords without bach_ngoc_boi (balanced double sword)
thai_thanh_v1 = {
  "name": "Thái Thanh Quán - Kiếm Đạo Hóa Kình",
  "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thanh_phong_kiem", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "kiem_tong_tam_yeu", "x": 0, "y": 3, "rarity": "rare"},
    {"id": "thai_thanh_tam_kinh", "x": 1, "y": 3, "rarity": "epic"},
    {"id": "thien_tam_chuoi", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "tuy_tien_ho_lo", "x": 2, "y": 2, "rarity": "rare"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "nho_sam_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 3, "rarity": "common"},
    {"id": "van_khi_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

# 2. thai_thanh_kiem_khi.json: 2 swords + 1 fan + strong armor
thai_thanh_kk_v1 = {
  "name": "Thái Thanh Quán - Kiếm Khí Xuyên Giáp",
  "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thanh_phong_kiem", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "thai_thanh_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
    {"id": "kiem_tong_tam_yeu", "x": 1, "y": 3, "rarity": "rare"},
    {"id": "ngoc_cot_phien", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "tuy_tien_ho_lo", "x": 2, "y": 2, "rarity": "rare"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "thiet_giap_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "van_khi_dan", "x": 3, "y": 3, "rarity": "uncommon"},
    {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 3, "rarity": "common"}
  ]
}

# 3. hybrid_thiet_phu_liet_diem.json: 2 axes + 1 blade with attack manual
thiet_phu_ld_v2 = {
  "name": "Thiết Phủ Bang - Thiết Phủ Liệt Diễm",
  "cols": 5, "rows": 4, "sect": "thiet_phu", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "khai_son_phu", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "khai_son_phu", "x": 0, "y": 2, "rarity": "epic"},
    {"id": "liet_diem_dao", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "chu_sa_ho_lo", "x": 2, "y": 3, "rarity": "rare"},
    {"id": "o_thiet_giap", "x": 3, "y": 0, "rarity": "epic"},
    {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "rare"},
    {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

# 4. bich_thuy_duong_sinh.json: 2 zithers + 1 whip with early armor
duong_sinh_v2 = {
  "name": "Bích Thủy Am - Dưỡng Sinh Cầm Âm",
  "cols": 5, "rows": 4, "sect": "bich_thuy", "tamPhapLevel": 1, "level": 1,
  "items": [
    {"id": "bich_thuy_cam", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "bich_thuy_cam", "x": 0, "y": 2, "rarity": "epic"},
    {"id": "thanh_dang_tien", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "duong_sinh_quyet", "x": 2, "y": 3, "rarity": "epic"}, # wait 1x2 doesn't fit at (2,3)!
  ]
}
