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
LOADOUTS = R3 / "loadouts"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

# Config override
config_data = {
    "exhaustionStep": 5,
    "maxRegen": 6,
    "burn": {
        "threshold": 50,
        "damage": 65,
        "maxHpPermille": 0,
        "ignoreShield": False,
        "decayPerSecond": 0
    },
    "poison": {
        "threshold": 50,
        "damage": 35,
        "maxHpPermille": 0,
        "ignoreShield": True,
        "decayPerSecond": 0
    },
    "bleed": {
        "threshold": 50,
        "damage": 15,
        "maxHpPermille": 30,
        "ignoreShield": True,
        "decayPerSecond": 0
    }
}
test_config = SCRATCH / "opt_config.json"
with open(test_config, "w", encoding="utf-8") as f:
    json.dump(config_data, f, indent=2, ensure_ascii=False)

patch = {
    # Fist / Gloves
    "thiet_cot_quyen_sao.attack": 3,
    "thiet_cot_quyen_sao.cooldown": 1.7,
    "bach_ho_cuong_mong.attack": 4,
    "bach_ho_cuong_mong.cooldown": 1.9,
    
    # Heavy weapons
    "khai_son_phu.attack": 22,
    "khai_son_phu.cooldown": 3.6,
    "chan_nhac_chuy.attack": 21,
    "chan_nhac_chuy.cooldown": 3.4,
    
    # Spear
    "bang_phach_thuong.attack": 30,
    "bang_phach_thuong.cooldown": 3.5,
    
    # Staff
    "phuc_ho_con.attack": 9,
    "phuc_ho_con.cooldown": 2.8,
    
    # Blades / Palms
    "liet_diem_dao.attack": 17,
    "liet_diem_dao.cooldown": 3.1,
    "tu_mau_song_dao.attack": 9,
    "tu_mau_song_dao.cooldown": 2.3,
    "xich_sa_chuong.attack": 10,
    "xich_sa_chuong.cooldown": 2.2,
    
    # Swords / Fans / Flutes / Brushes
    "thanh_phong_kiem.attack": 10,
    "thanh_phong_kiem.cooldown": 2.7,
    "ngoc_cot_phien.attack": 5,
    "ngoc_cot_phien.cooldown": 2.5,
    "phan_quan_but.attack": 11,
    "phan_quan_but.cooldown": 2.8,
    "han_ngoc_tieu.attack": 11,
    "han_ngoc_tieu.cooldown": 3.1,
    "bich_thuy_cam.attack": 15,
    "bich_thuy_cam.cooldown": 3.3,
    "thanh_dang_tien.attack": 12,
    "thanh_dang_tien.cooldown": 2.7,
    "truy_hon_cung.attack": 21,
    "truy_hon_cung.cooldown": 3.5,
    
    # Claws / Hidden weapons
    "that_tinh_tieu.attack": 4,
    "that_tinh_tieu.cooldown": 2.3,
    "tu_kim_trao.attack": 6,
    "tu_kim_trao.cooldown": 1.7,
    "u_minh_quy_trao.attack": 14,
    "u_minh_quy_trao.cooldown": 1.8,
    "kich_doc_chu_nga.attack": 11,
    "kich_doc_chu_nga.cooldown": 1.6,
    "soi_nanh_canh.attack": 7,
    "soi_nanh_canh.cooldown": 2.0
}

items = json.load(open(BASE_ITEMS, "r", encoding="utf-8"))
for address, val in patch.items():
    balance.item_parameter(items, address, val, set_value=True)

test_items = SCRATCH / "opt_items_v3.json"
with open(test_items, "w", encoding="utf-8") as f:
    json.dump(items, f, indent=2, ensure_ascii=False)

cmd = [
    "py", "tools/balance/balance.py", "matrix",
    "--loadouts", str(LOADOUTS),
    "--items", str(test_items),
    "--sects", str(SECTS),
    "--battle-config", str(test_config),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "60",
    "--level", "10"
]
env = os.environ.copy()
env["PYTHONUTF8"] = "1"
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)

print(res.stdout)
with open(SCRATCH / "opt_matrix_a_v3.txt", "w", encoding="utf-8") as f:
    f.write(res.stdout)
