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
        "damage": 15,
        "maxHpPermille": 0,
        "ignoreShield": True,
        "decayPerSecond": 0
    },
    "bleed": {
        "threshold": 50,
        "damage": 8,
        "maxHpPermille": 15,
        "ignoreShield": True,
        "decayPerSecond": 0
    }
}
balance.write_json(config_path, config_base)

def eval_matrix_a(patch, runs=50, level=10):
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
        "--level", str(level)
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    return res.stdout

def eval_matrix_b(patch, runs=50, level=10):
    balance.write_json(patch_path, patch)
    results = {}
    for r in ["common", "legendary"]:
        cmd = [
            "py", "tools/balance/balance.py", "whatif",
            "--patch", str(patch_path),
            "--loadouts", str(loadouts_dir),
            "--items", "tools/gemini/out/round-3/items-base.json",
            "--battle-config", str(config_path),
            "--tiers", "1,9",
            "--tp", "1",
            "--runs", str(runs),
            "--level", str(level),
            "--rarity", r
        ]
        env = os.environ.copy()
        env["PYTHONUTF8"] = "1"
        res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
        results[r] = res.stdout
    return results

fine_patch = {
    # Heavy weapons
    "khai_son_phu.cooldown": 2.7,
    "khai_son_phu.attack": 27,
    "khai_son_phu.ultimate.cost": 38,
    
    "chan_nhac_chuy.cooldown": 2.5,
    "chan_nhac_chuy.attack": 25,
    "chan_nhac_chuy.ultimate.cost": 38,
    
    "bang_phach_thuong.cooldown": 2.7,
    "bang_phach_thuong.attack": 27,
    "bang_phach_thuong.ultimate.cost": 38,
    
    "truy_hon_cung.cooldown": 2.9,
    "truy_hon_cung.attack": 21,
    "truy_hon_cung.ultimate.cost": 42,
    
    "phan_quan_but.cooldown": 2.4,
    "phan_quan_but.attack": 12,
    
    "han_ngoc_tieu.cooldown": 2.5,
    "han_ngoc_tieu.attack": 13,
    
    "bich_thuy_cam.cooldown": 2.7,
    "bich_thuy_cam.attack": 18,
    
    "thanh_dang_tien.cooldown": 2.4,
    "thanh_dang_tien.attack": 14,
    
    # Medium weapons
    "thanh_phong_kiem.cooldown": 2.5,
    "thanh_phong_kiem.attack": 9,
    
    "ngoc_cot_phien.cooldown": 2.3,
    "ngoc_cot_phien.attack": 6,
    
    "phuc_ho_con.cooldown": 2.3,
    "phuc_ho_con.attack": 12,
    
    # Fast weapons
    "liet_diem_dao.cooldown": 3.7,
    "liet_diem_dao.attack": 11,
    
    "xich_sa_chuong.cooldown": 2.7,
    "xich_sa_chuong.attack": 5,
    
    "tu_mau_song_dao.cooldown": 2.7,
    "tu_mau_song_dao.attack": 5,
    
    "thiet_cot_quyen_sao.attack": 4,
    "tu_kim_trao.attack": 3,
    "that_tinh_tieu.attack": 2,
    "that_tinh_tieu.cooldown": 3.0,
    
    # Monster items
    "bach_ho_cuong_mong.attack": 3,
    "bach_ho_cuong_mong.cooldown": 2.4,
    
    "u_minh_quy_trao.attack": 10,
    "u_minh_quy_trao.cooldown": 2.2,
    
    "kich_doc_chu_nga.attack": 7,
    "kich_doc_chu_nga.cooldown": 2.0,
    
    "soi_nanh_canh.attack": 4,
    "soi_nanh_canh.cooldown": 2.4,
}

stdout_a = eval_matrix_a(fine_patch, runs=50, level=10)
print("=== Matrix A (runs=50, level=10) ===")
print(stdout_a)

stdout_b = eval_matrix_b(fine_patch, runs=50, level=10)
print("=== Matrix B (Common) ===")
print(stdout_b["common"])
print("=== Matrix B (Legendary) ===")
print(stdout_b["legendary"])
