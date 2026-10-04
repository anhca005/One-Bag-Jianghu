import json
import subprocess
import os
import sys
import copy
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, str(Path("tools/balance").resolve()))
import balance

items_base = balance.read_json(Path("tools/gemini/out/round-3/items-base.json"))
sects_base = balance.read_json(Path("server/battle-engine/src/main/resources/data/sects.json"))
loadouts_dir = Path("tools/gemini/out/round-3/loadouts").resolve()

def test_setup(patch, config, runs=25, level=10):
    items_final = copy.deepcopy(items_base)
    sects_final = copy.deepcopy(sects_base)
    for addr, val in patch.items():
        balance.parameter(items_final, sects_final, {}, addr, val, set_value=True)
        
    temp_items = Path("tools/gemini/out/round-3/scratch/direct_items.json")
    temp_config = Path("tools/gemini/out/round-3/scratch/direct_config.json")
    balance.write_json(temp_items, items_final)
    balance.write_json(temp_config, config)
    
    cmd = [
        "py", "tools/balance/balance.py", "matrix",
        "--loadouts", str(loadouts_dir),
        "--items", str(temp_items),
        "--battle-config", str(temp_config),
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", str(runs),
        "--level", str(level)
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    
    outside = 999
    for line in res.stdout.splitlines():
        if "Outside" in line:
            parts = line.split(":")[-1].strip().split("/")[0]
            outside = int(parts)
            break
    return outside, res.stdout

cfg = {
    "exhaustionStep": 5,
    "maxRegen": 8,
    "burn": {
        "threshold": 50,
        "damage": 65,
        "maxHpPermille": 0,
        "ignoreShield": False,
        "decayPerSecond": 0
    },
    "poison": {
        "threshold": 50,
        "damage": 30,
        "maxHpPermille": 0,
        "ignoreShield": True,
        "decayPerSecond": 0
    },
    "bleed": {
        "threshold": 50,
        "damage": 15,
        "maxHpPermille": 35,
        "ignoreShield": True,
        "decayPerSecond": 0
    }
}

p = {
    # Monster claw fix
    "bach_ho_cuong_mong.attack": 2,
    
    # Heavy weapons
    "khai_son_phu.cooldown": 3.4,
    "khai_son_phu.attack": 25,
    "chan_nhac_chuy.cooldown": 3.2,
    "chan_nhac_chuy.attack": 24,
    "bang_phach_thuong.cooldown": 3.4,
    "bang_phach_thuong.attack": 26,
    
    # Bow, brush, flute, zither, whip
    "truy_hon_cung.cooldown": 3.4,
    "truy_hon_cung.attack": 22,
    "phan_quan_but.cooldown": 2.6,
    "phan_quan_but.attack": 12,
    "han_ngoc_tieu.cooldown": 3.0,
    "han_ngoc_tieu.attack": 12,
    "bich_thuy_cam.cooldown": 3.2,
    "bich_thuy_cam.attack": 17,
    "thanh_dang_tien.cooldown": 2.6,
    "thanh_dang_tien.attack": 12,
    
    # Sword & Fan (nerfed to tame Thai Thanh)
    "thanh_phong_kiem.cooldown": 2.8,
    "thanh_phong_kiem.attack": 8,
    "ngoc_cot_phien.cooldown": 2.4,
    "ngoc_cot_phien.attack": 5,
    
    # Staff & Fist
    "phuc_ho_con.cooldown": 2.6,
    "phuc_ho_con.attack": 9,
    "thiet_cot_quyen_sao.attack": 4,
    
    # Blade, Palm, Dual (buffed to bring Liet Diem into band)
    "liet_diem_dao.cooldown": 3.0,
    "liet_diem_dao.attack": 18,
    "xich_sa_chuong.cooldown": 2.2,
    "xich_sa_chuong.attack": 10,
    "tu_mau_song_dao.cooldown": 2.4,
    "tu_mau_song_dao.attack": 9,
    
    "tu_kim_trao.attack": 4,
    "that_tinh_tieu.attack": 3,
    
    # Monster items
    "u_minh_quy_trao.attack": 12,
    "u_minh_quy_trao.cooldown": 2.0,
    "kich_doc_chu_nga.attack": 9,
    "kich_doc_chu_nga.cooldown": 1.8,
    "soi_nanh_canh.attack": 7,
    "soi_nanh_canh.cooldown": 2.0,
}

outside, out = test_setup(p, cfg, runs=25, level=10)
print(f"Direct Matrix Outside: {outside}/192")
print(out)
