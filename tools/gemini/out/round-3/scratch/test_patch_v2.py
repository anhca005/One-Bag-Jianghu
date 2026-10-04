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
CONFIG_FILE = R3 / "config.json"
LOADOUTS = R3 / "loadouts"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

# Load config and adjust maxRegen
config_data = json.load(open(CONFIG_FILE, "r", encoding="utf-8"))
config_data["maxRegen"] = 4
temp_config = SCRATCH / "test_config.json"
with open(temp_config, "w", encoding="utf-8") as f:
    json.dump(config_data, f, indent=2, ensure_ascii=False)

def apply_patch(patch_dict, out_file):
    items = json.load(open(BASE_ITEMS, "r", encoding="utf-8"))
    for address, val in patch_dict.items():
        balance.item_parameter(items, address, val, set_value=True)
    with open(out_file, "w", encoding="utf-8") as f:
        json.dump(items, f, indent=2, ensure_ascii=False)

def eval_matrix(items_file, cfg_file, runs=50):
    cmd = [
        "py", "tools/balance/balance.py", "matrix",
        "--loadouts", str(LOADOUTS),
        "--items", str(items_file),
        "--sects", str(SECTS),
        "--battle-config", str(cfg_file),
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", str(runs),
        "--level", "10"
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    
    outside = 192
    build_avgs = []
    lines = res.stdout.splitlines()
    for l in lines:
        if "Outside [35, 65]:" in l:
            outside = int(l.split(":")[1].strip().split("/")[0])
        elif "%" in l and not l.startswith("Loadout") and not l.startswith("-"):
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

patch = {
    # Fist / Gloves
    "thiet_cot_quyen_sao.attack": 3,
    "thiet_cot_quyen_sao.cooldown": 1.7,
    "bach_ho_cuong_mong.attack": 4,
    "bach_ho_cuong_mong.cooldown": 1.9,
    
    # Heavy weapons (Axes / Hammers)
    "khai_son_phu.attack": 22,
    "khai_son_phu.cooldown": 3.7,
    "chan_nhac_chuy.attack": 21,
    "chan_nhac_chuy.cooldown": 3.5,
    
    # Spears
    "bang_phach_thuong.attack": 34,
    "bang_phach_thuong.cooldown": 3.6,
    
    # Staff
    "phuc_ho_con.attack": 9,
    "phuc_ho_con.cooldown": 2.8,
    
    # Blades / Palms
    "liet_diem_dao.attack": 17,
    "liet_diem_dao.cooldown": 3.1,
    "tu_mau_song_dao.attack": 9,
    "tu_mau_song_dao.cooldown": 2.2,
    "xich_sa_chuong.attack": 10,
    "xich_sa_chuong.cooldown": 2.2,
    
    # Swords / Fans / Flutes / Brushes
    "thanh_phong_kiem.attack": 13,
    "thanh_phong_kiem.cooldown": 2.5,
    "ngoc_cot_phien.attack": 4,
    "ngoc_cot_phien.cooldown": 2.5,
    "ngoc_cot_phien.crit": 0.05,
    "phan_quan_but.attack": 10,
    "phan_quan_but.cooldown": 2.9,
    "han_ngoc_tieu.attack": 10,
    "han_ngoc_tieu.cooldown": 3.2,
    "bich_thuy_cam.attack": 14,
    "bich_thuy_cam.cooldown": 3.4,
    "thanh_dang_tien.attack": 11,
    "thanh_dang_tien.cooldown": 2.8,
    "truy_hon_cung.attack": 20,
    "truy_hon_cung.cooldown": 3.6,
    
    # Claws / Hidden weapons (Huyền Độc)
    "that_tinh_tieu.attack": 4,
    "that_tinh_tieu.cooldown": 2.2,
    "tu_kim_trao.attack": 6,
    "tu_kim_trao.cooldown": 1.7,
    "u_minh_quy_trao.attack": 14,
    "u_minh_quy_trao.cooldown": 1.8,
    "kich_doc_chu_nga.attack": 11,
    "kich_doc_chu_nga.cooldown": 1.6,
    "soi_nanh_canh.attack": 7,
    "soi_nanh_canh.cooldown": 2.0
}

temp_items = SCRATCH / "opt_items_v2.json"
apply_patch(patch, temp_items)
outside, b_avgs, out_text = eval_matrix(temp_items, temp_config, runs=50)

print(f"Trial with maxRegen=4 and tuned weapons: Outside = {outside}/192")
if b_avgs:
    print(f"Build averages: min={min(b_avgs):.1f}%, max={max(b_avgs):.1f}%")
    bad_low = [i for i, v in enumerate(b_avgs) if v < 30.0]
    bad_high = [i for i, v in enumerate(b_avgs) if v > 70.0]
    print(f"Builds <30%: {len(bad_low)}, Builds >70%: {len(bad_high)}")

with open(SCRATCH / "trial_matrix_a_v2.txt", "w", encoding="utf-8") as f:
    f.write(out_text)
