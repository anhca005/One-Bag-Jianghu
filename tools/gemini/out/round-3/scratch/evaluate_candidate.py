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

loadouts_dir = Path("tools/gemini/out/round-3/loadouts").resolve()
patch_path = Path("tools/gemini/out/round-3/scratch/test_patch.json")
config_path = Path("tools/gemini/out/round-3/scratch/test_config.json")

# Config with burst damage sized for 250-400 HP pool
config_base = {
    "exhaustionStep": 5,
    "maxRegen": 8,
    "burn": {
        "threshold": 50,
        "damage": 25,
        "maxHpPermille": 0,
        "ignoreShield": False,
        "decayPerSecond": 0
    },
    "poison": {
        "threshold": 50,
        "damage": 20,
        "maxHpPermille": 0,
        "ignoreShield": True,
        "decayPerSecond": 0
    },
    "bleed": {
        "threshold": 50,
        "damage": 12,
        "maxHpPermille": 25,
        "ignoreShield": True,
        "decayPerSecond": 0
    }
}
balance.write_json(config_path, config_base)

def evaluate_patch(patch):
    balance.write_json(patch_path, patch)
    cmd = [
        "py", "tools/balance/balance.py", "whatif",
        "--patch", str(patch_path),
        "--loadouts", str(loadouts_dir),
        "--items", "tools/gemini/out/round-3/items-base.json",
        "--battle-config", str(config_path),
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", "30",
        "--level", "10"
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    if res.returncode != 0:
        print(f"Error: {res.stderr}")
        return None, 999
    
    outside = 999
    lines = res.stdout.splitlines()
    for line in lines:
        if "Outside" in line:
            parts = line.split(":")[-1].strip().split("/")[0]
            outside = int(parts)
            break
    return res.stdout, outside

candidate = {
    # 1. Heavy weapons (Axe, Hammer, Spear) - High Attack & Low Ult Cost
    "khai_son_phu.cooldown": 2.8,
    "khai_son_phu.attack": 36,
    "khai_son_phu.ultimate.cost": 38,
    
    "chan_nhac_chuy.cooldown": 2.6,
    "chan_nhac_chuy.attack": 32,
    "chan_nhac_chuy.ultimate.cost": 38,
    
    "bang_phach_thuong.cooldown": 2.8,
    "bang_phach_thuong.attack": 34,
    "bang_phach_thuong.ultimate.cost": 38,
    
    "truy_hon_cung.cooldown": 2.8,
    "truy_hon_cung.attack": 30,
    "truy_hon_cung.ultimate.cost": 40,
    
    "phan_quan_but.cooldown": 2.2,
    "phan_quan_but.attack": 16,
    
    "han_ngoc_tieu.cooldown": 2.4,
    "han_ngoc_tieu.attack": 18,
    
    "bich_thuy_cam.cooldown": 2.6,
    "bich_thuy_cam.attack": 22,
    
    "thanh_dang_tien.cooldown": 2.4,
    "thanh_dang_tien.attack": 16,
    
    # 2. Medium weapons - Nerf high performers (Sword, Fan)
    "thanh_phong_kiem.cooldown": 2.4,
    "thanh_phong_kiem.attack": 9,
    
    "ngoc_cot_phien.cooldown": 2.2,
    "ngoc_cot_phien.attack": 6,
    
    "phuc_ho_con.cooldown": 2.4,
    "phuc_ho_con.attack": 10,
    
    # 3. Blade & Palm adjustments (Liệt Diễm)
    "liet_diem_dao.cooldown": 3.6,
    "liet_diem_dao.attack": 12,
    
    "xich_sa_chuong.cooldown": 2.6,
    "xich_sa_chuong.attack": 5,
    
    "tu_mau_song_dao.cooldown": 2.6,
    "tu_mau_song_dao.attack": 6,
    
    # 4. Fists & Claws
    "thiet_cot_quyen_sao.attack": 4,
    "tu_kim_trao.attack": 3,
    "that_tinh_tieu.attack": 2,
    
    # 5. Monster items
    "bach_ho_cuong_mong.attack": 3,
    "bach_ho_cuong_mong.cooldown": 2.4,
    
    "u_minh_quy_trao.attack": 8,
    "u_minh_quy_trao.cooldown": 2.4,
    
    "kich_doc_chu_nga.attack": 6,
    "kich_doc_chu_nga.cooldown": 2.2,
    
    "soi_nanh_canh.attack": 5,
    "soi_nanh_canh.cooldown": 2.4,
}

stdout, out_cnt = evaluate_patch(candidate)
print(f"Candidate Outside count: {out_cnt}/192")
print(stdout)
