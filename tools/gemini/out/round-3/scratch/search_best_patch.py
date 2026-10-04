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
        "damage": 22,
        "maxHpPermille": 0,
        "ignoreShield": False,
        "decayPerSecond": 0
    },
    "poison": {
        "threshold": 50,
        "damage": 16,
        "maxHpPermille": 0,
        "ignoreShield": True,
        "decayPerSecond": 0
    },
    "bleed": {
        "threshold": 50,
        "damage": 10,
        "maxHpPermille": 20,
        "ignoreShield": True,
        "decayPerSecond": 0
    }
}
balance.write_json(config_path, config_base)

def eval_candidate(patch, runs=25):
    balance.write_json(patch_path, patch)
    cmd = [
        "py", "tools/balance/balance.py", "whatif",
        "--patch", str(patch_path),
        "--loadouts", str(loadouts_dir),
        "--items", "tools/gemini/out/round-3/items-base.json",
        "--battle-config", str(config_path),
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", str(runs),
        "--level", "10"
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

base_patch = {
    # 1. Buff under-performing heavy weapons (Axe, Hammer, Spear)
    "khai_son_phu.cooldown": 2.5,
    "khai_son_phu.attack": 32,
    "khai_son_phu.ultimate.cost": 36,
    
    "chan_nhac_chuy.cooldown": 2.4,
    "chan_nhac_chuy.attack": 28,
    "chan_nhac_chuy.ultimate.cost": 36,
    
    "bang_phach_thuong.cooldown": 2.5,
    "bang_phach_thuong.attack": 30,
    "bang_phach_thuong.ultimate.cost": 36,
    
    "truy_hon_cung.cooldown": 2.8,
    "truy_hon_cung.attack": 20,
    "truy_hon_cung.ultimate.cost": 42,
    
    # 2. Under-performing monster claw items (mb1)
    "u_minh_quy_trao.attack": 12,
    "u_minh_quy_trao.cooldown": 2.0,
    
    "kich_doc_chu_nga.attack": 9,
    "kich_doc_chu_nga.cooldown": 1.8,
    
    # 3. Staff & Fist
    "phuc_ho_con.cooldown": 2.2,
    "phuc_ho_con.attack": 13,
    "thiet_cot_quyen_sao.attack": 4,
    
    # 4. Zither, Whip, Flute, Brush
    "bich_thuy_cam.cooldown": 2.8,
    "bich_thuy_cam.attack": 18,
    
    "thanh_dang_tien.cooldown": 2.4,
    "thanh_dang_tien.attack": 14,
    
    "phan_quan_but.cooldown": 2.4,
    "phan_quan_but.attack": 12,
    
    "han_ngoc_tieu.cooldown": 2.6,
    "han_ngoc_tieu.attack": 14,
    
    # 5. Nerf over-performing weapons (Blade, Fan, Sword, Claws)
    "liet_diem_dao.cooldown": 3.8,
    "liet_diem_dao.attack": 10,
    
    "xich_sa_chuong.cooldown": 2.8,
    "xich_sa_chuong.attack": 4,
    
    "tu_mau_song_dao.cooldown": 2.8,
    "tu_mau_song_dao.attack": 5,
    
    "ngoc_cot_phien.cooldown": 2.4,
    "ngoc_cot_phien.attack": 5,
    
    "thanh_phong_kiem.cooldown": 2.6,
    "thanh_phong_kiem.attack": 8,
    
    "that_tinh_tieu.attack": 2,
    "that_tinh_tieu.cooldown": 3.0,
    
    "tu_kim_trao.attack": 3,
    
    # Monster items
    "bach_ho_cuong_mong.attack": 3,
    "bach_ho_cuong_mong.cooldown": 2.4,
    
    "soi_nanh_canh.attack": 4,
    "soi_nanh_canh.cooldown": 2.4,
}

out_cnt, stdout = eval_candidate(base_patch, runs=25)
print(f"Base candidate Outside count: {out_cnt}/192")
print(stdout)
