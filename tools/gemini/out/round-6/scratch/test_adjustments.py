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

# 1. Adjust thai_thanh.json: replace kiem_tong_tam_yeu with thanh_tam_dan
data = json.load(open(LOADOUTS_DIR / "thai_thanh.json", encoding="utf-8"))
for it in data["items"]:
    if it["id"] == "kiem_tong_tam_yeu":
        it["id"] = "thanh_tam_dan"
        it["rarity"] = "uncommon"
with open(LOADOUTS_DIR / "thai_thanh.json", "w", encoding="utf-8") as f:
    json.dump(data, f, ensure_ascii=False, indent=2)

# 2. Adjust huyen_doc.json: replace 1 claw with armor/pill
# Original has: that_tinh_tieu at (0,0), that_tinh_tieu at (0,2), tu_kim_trao at (1,0), tu_kim_trao at (1,2)
data_hd = json.load(open(LOADOUTS_DIR / "huyen_doc.json", encoding="utf-8"))
# Replace tu_kim_trao at (1,2) with da_ho_y (1x2 armor)
for it in data_hd["items"]:
    if it["id"] == "tu_kim_trao" and it["x"] == 1 and it["y"] == 2:
        it["id"] = "da_ho_y"
        it["rarity"] = "rare"
with open(LOADOUTS_DIR / "huyen_doc.json", "w", encoding="utf-8") as f:
    json.dump(data_hd, f, ensure_ascii=False, indent=2)

# 3. Adjust huyen_doc_van_doc.json: replace 1 claw with da_ho_y (1x2 armor)
data_vd = json.load(open(LOADOUTS_DIR / "huyen_doc_van_doc.json", encoding="utf-8"))
for it in data_vd["items"]:
    if it["id"] == "tu_kim_trao" and it["x"] == 1 and it["y"] == 2:
        it["id"] = "da_ho_y"
        it["rarity"] = "rare"
with open(LOADOUTS_DIR / "huyen_doc_van_doc.json", "w", encoding="utf-8") as f:
    json.dump(data_vd, f, ensure_ascii=False, indent=2)

# 4. Restore original hybrid_han_son_bich_thuy.json from server
shutil_path = ROOT / "server" / "battle-engine" / "samples" / "hybrid_han_son_bich_thuy.json"
import shutil
shutil.copy(shutil_path, LOADOUTS_DIR / "hybrid_han_son_bich_thuy.json")

print("Adjustments applied. Running matrix evaluation...")
cmd = [
    "py", "tools/balance/balance.py", "matrix",
    "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", "40", "--level", "10",
    "--loadouts", str(LOADOUTS_DIR)
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
for line in res.stdout.splitlines():
    if "Outside [" in line:
        print("RESULT:", line)
for line in res.stdout.splitlines():
    if "%" in line and not line.startswith("Loadout") and not line.startswith("-") and not line.startswith("Outside"):
        if any(k in line for k in ["Thái Thanh Quán - Kiếm Đạo", "Huyền Độc Môn", "Hàn Sơn Phái - Hàn Bích"]):
            print(line[:85])
