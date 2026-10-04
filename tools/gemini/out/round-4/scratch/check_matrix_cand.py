import json
import subprocess
import os
import sys
import copy
import tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
LOADOUTS = R4 / "loadouts"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

items_base = json.load(open(balance.DEFAULT_ITEMS, encoding="utf-8"))

cfg = {
    "exhaustionStep": 5,
    "maxRegen": 3,
    "burn": {
        "threshold": 50,
        "damage": 85,
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
        "damage": 20,
        "maxHpPermille": 35,
        "ignoreShield": True,
        "decayPerSecond": 0
    }
}

patch_cand = {
    "thiet_cot_quyen_sao.attack": 5,
    "thiet_cot_quyen_sao.cooldown": 1.7,
    
    "thanh_phong_kiem.attack": 14,
    "thanh_phong_kiem.cooldown": 2.6,
    
    "khai_son_phu.attack": 32,
    "khai_son_phu.cooldown": 2.8,
    "khai_son_phu.ultimate.cost": 45,
    
    "chan_nhac_chuy.attack": 30,
    "chan_nhac_chuy.cooldown": 2.8,
    
    "liet_diem_dao.attack": 25,
    "liet_diem_dao.cooldown": 2.5,
    "liet_diem_dao.ultimate.cost": 38,
    
    "tu_mau_song_dao.attack": 15,
    "tu_mau_song_dao.cooldown": 2.0,
    
    "xich_sa_chuong.attack": 15,
    "xich_sa_chuong.cooldown": 1.9,
    
    "bang_phach_thuong.attack": 42,
    "bang_phach_thuong.cooldown": 3.0,
    
    "truy_hon_cung.attack": 28,
    "truy_hon_cung.cooldown": 2.9,
    
    "soi_nanh_canh.attack": 11,
    "soi_nanh_canh.cooldown": 1.9,
    
    "kich_doc_chu_nga.attack": 15,
    "kich_doc_chu_nga.cooldown": 1.6,
    
    "tu_kim_trao.attack": 9,
    "tu_kim_trao.cooldown": 1.7,
    
    "u_minh_quy_trao.attack": 20,
    "u_minh_quy_trao.cooldown": 1.8,
    
    "bach_ho_cuong_mong.attack": 6,
    "bach_ho_cuong_mong.cooldown": 1.8,
    
    "phuc_ho_con.attack": 14,
    "phuc_ho_con.cooldown": 2.6,
    
    "bich_thuy_cam.attack": 20,
    "bich_thuy_cam.cooldown": 3.0,
    
    "thanh_dang_tien.attack": 16,
    "thanh_dang_tien.cooldown": 2.6,
    
    "that_tinh_tieu.attack": 6,
    "that_tinh_tieu.cooldown": 2.1,
    
    "ngoc_cot_phien.attack": 7,
    "ngoc_cot_phien.cooldown": 2.6,
    
    "phan_quan_but.attack": 15,
    "phan_quan_but.cooldown": 2.7,
    
    "han_ngoc_tieu.attack": 15,
    "han_ngoc_tieu.cooldown": 2.8,

    # Outlier triggers
    "kiem_tong_tam_yeu.triggers.charge": 0.15,
    "ngoc_cot_phien.triggers.charge": 0.25,
    "bach_luyen_chan_kinh.triggers.add_attack": 1,
    "bach_luyen_chan_kinh.triggers.charge": 0.6,
    "thai_thanh_tam_kinh.triggers.parry_discount": 10,
    "thien_tam_chuoi.triggers.parry_discount": 5,
}

test_items = copy.deepcopy(items_base)
test_sects = balance.read_json(balance.DEFAULT_SECTS)

for addr, val in patch_cand.items():
    balance.parameter(test_items, test_sects, cfg, addr, val, set_value=True)

with tempfile.TemporaryDirectory(prefix="test_mat_") as raw_temp:
    temp = Path(raw_temp)
    f_items = temp / "items.json"
    f_cfg = temp / "config.json"
    balance.write_json(f_items, test_items, indent=None)
    balance.write_json(f_cfg, cfg, indent=None)

    cmd_mat = [
        "py", "tools/balance/balance.py", "matrix",
        "--loadouts", str(LOADOUTS),
        "--items", str(f_items),
        "--battle-config", str(f_cfg),
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", "60",
        "--level", "10"
    ]
    res_mat = subprocess.run(cmd_mat, capture_output=True, text=True, encoding="utf-8", errors="replace")
    print(res_mat.stdout)
    if res_mat.stderr:
        print("Stderr:", res_mat.stderr[:300])
