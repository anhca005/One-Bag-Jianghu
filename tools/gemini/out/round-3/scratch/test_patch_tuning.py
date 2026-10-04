import json
import subprocess
import os
import sys
import copy
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R3 = ROOT / "tools" / "gemini" / "out" / "round-3"
SCRATCH = R3 / "scratch"
BASE_ITEMS = R3 / "items-base.json"
PATCH_FILE = R3 / "patch.json"
FINAL_ITEMS = R3 / "items-v3-final.json"
CONFIG = R3 / "config.json"
LOADOUTS = R3 / "loadouts"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

def apply_patch_to_base(patch_dict):
    items = json.load(open(BASE_ITEMS, "r", encoding="utf-8"))
    for address, val in patch_dict.items():
        balance.item_parameter(items, address, val, set_value=True)
    temp_items = SCRATCH / "test_items.json"
    with open(temp_items, "w", encoding="utf-8") as f:
        json.dump(items, f, indent=2, ensure_ascii=False)
    return temp_items

def eval_matrix_a(items_file, runs=60):
    cmd = [
        "py", "tools/balance/balance.py", "matrix",
        "--loadouts", str(LOADOUTS),
        "--items", str(items_file),
        "--sects", str(SECTS),
        "--battle-config", str(CONFIG),
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", str(runs),
        "--level", "10"
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    
    # Parse outside count and build averages
    outside = 0
    build_avgs = []
    lines = res.stdout.splitlines()
    for l in lines:
        if "Outside [35, 65]:" in l:
            outside = int(l.split(":")[1].strip().split("/")[0])
        elif "%" in l and not l.startswith("Loadout") and not l.startswith("-"):
            # row
            parts = l.strip().split()
            vals = []
            for p in parts[1:]:
                clean_p = p.replace("%", "").replace("!", "")
                try:
                    vals.append(float(clean_p))
                except ValueError:
                    pass
            if vals:
                build_avgs.append(sum(vals) / len(vals))
    return outside, build_avgs, res.stdout

# Current patch
current_patch = json.load(open(PATCH_FILE, "r", encoding="utf-8"))

# Test candidate modifications
candidate = copy.deepcopy(current_patch)

# 1. Buff Huyền Độc weapons
candidate["that_tinh_tieu.attack"] = 6
candidate["that_tinh_tieu.cooldown"] = 2.2
candidate["tu_kim_trao.attack"] = 8
candidate["tu_kim_trao.cooldown"] = 1.7
candidate["u_minh_quy_trao.attack"] = 16
candidate["u_minh_quy_trao.cooldown"] = 1.8
candidate["kich_doc_chu_nga.attack"] = 12
candidate["kich_doc_chu_nga.cooldown"] = 1.5

# 2. Nerf overperforming heavy weapons
candidate["khai_son_phu.attack"] = 19
candidate["khai_son_phu.cooldown"] = 4.0
candidate["chan_nhac_chuy.attack"] = 18
candidate["chan_nhac_chuy.cooldown"] = 3.8
candidate["phuc_ho_con.attack"] = 8
candidate["phuc_ho_con.cooldown"] = 3.0

# 3. Nerf Hàn Bích Hợp Bích weapons slightly
candidate["phan_quan_but.attack"] = 10
candidate["phan_quan_but.cooldown"] = 2.8
candidate["han_ngoc_tieu.attack"] = 10
candidate["han_ngoc_tieu.cooldown"] = 3.2

# 4. Buff Băng Phách Thương
candidate["bang_phach_thuong.attack"] = 32
candidate["bang_phach_thuong.cooldown"] = 3.6

print("Testing candidate patch...")
test_items = apply_patch_to_base(candidate)
outside, b_avgs, out_text = eval_matrix_a(test_items, runs=60)
print(f"Candidate result: Outside count = {outside}/192 (previous was 131/192)")
if b_avgs:
    min_b = min(b_avgs)
    max_b = max(b_avgs)
    print(f"Build averages range: {min_b:.1f}% to {max_b:.1f}%")

with open(SCRATCH / "candidate_matrix_a.txt", "w", encoding="utf-8") as f:
    f.write(out_text)
