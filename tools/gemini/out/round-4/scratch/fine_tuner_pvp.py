import json
import subprocess
import os
import sys
import copy
import tempfile
import time
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
LOADOUTS = R4 / "loadouts"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()

items_base = json.load(open(balance.DEFAULT_ITEMS, encoding="utf-8"))
sects_base = balance.read_json(balance.DEFAULT_SECTS)

def evaluate(patch, cfg, runs=40):
    test_items = copy.deepcopy(items_base)
    test_sects = copy.deepcopy(sects_base)
    test_cfg = copy.deepcopy(cfg)
    
    for addr, val in patch.items():
        balance.parameter(test_items, test_sects, test_cfg, addr, val, set_value=True)

    with tempfile.TemporaryDirectory(prefix="tune_") as raw_temp:
        temp = Path(raw_temp)
        f_items = temp / "items.json"
        f_cfg = temp / "config.json"
        balance.write_json(f_items, test_items, indent=None)
        balance.write_json(f_cfg, test_cfg, indent=None)

        cmd_mat = [
            "py", "tools/balance/balance.py", "matrix",
            "--loadouts", str(LOADOUTS),
            "--items", str(f_items),
            "--battle-config", str(f_cfg),
            "--tiers", "1,3,6,9",
            "--tp", "1,9",
            "--runs", str(runs),
            "--level", "10"
        ]
        res = subprocess.run(cmd_mat, capture_output=True, text=True, encoding="utf-8", errors="replace")
        
        outside = 0
        build_avgs = []
        for line in res.stdout.splitlines():
            if "Outside [35, 65]:" in line:
                parts = line.strip().split()
                outside = int(parts[3].split("/")[0])
            elif len(line) > 10 and "%" in line and not line.startswith("Loadout") and not line.startswith("-"):
                parts = line.strip().split()
                vals = [float(p.rstrip("%!")) for p in parts if p.endswith("%") or p.endswith("%!")]
                if vals:
                    build_avgs.append((parts[0], sum(vals) / len(vals)))

        min_avg = min(b[1] for b in build_avgs) if build_avgs else 0
        max_avg = max(b[1] for b in build_avgs) if build_avgs else 100
        outlier_count = sum(1 for b in build_avgs if b[1] < 30.0 or b[1] > 70.0)
        return outside, min_avg, max_avg, outlier_count, build_avgs

# Baseline candidate patch
base_cfg = {
    "exhaustionStep": 5,
    "maxRegen": 3,
    "burn": {
        "threshold": 50,
        "damage": 75,
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
        "damage": 18,
        "maxHpPermille": 30,
        "ignoreShield": True,
        "decayPerSecond": 0
    }
}

base_patch = {
    "thiet_cot_quyen_sao.attack": 5,
    "thiet_cot_quyen_sao.cooldown": 1.7,
    
    "thanh_phong_kiem.attack": 16,
    "thanh_phong_kiem.cooldown": 2.5,
    
    "khai_son_phu.attack": 30,
    "khai_son_phu.cooldown": 2.8,
    "khai_son_phu.ultimate.cost": 45,
    
    "chan_nhac_chuy.attack": 28,
    "chan_nhac_chuy.cooldown": 2.8,
    
    "liet_diem_dao.attack": 21,
    "liet_diem_dao.cooldown": 2.7,
    "liet_diem_dao.ultimate.cost": 40,
    
    "tu_mau_song_dao.attack": 15,
    "tu_mau_song_dao.cooldown": 2.0,
    
    "xich_sa_chuong.attack": 14,
    "xich_sa_chuong.cooldown": 2.0,
    
    "bang_phach_thuong.attack": 40,
    "bang_phach_thuong.cooldown": 3.0,
    
    "truy_hon_cung.attack": 26,
    "truy_hon_cung.cooldown": 2.9,
    
    "soi_nanh_canh.attack": 11,
    "soi_nanh_canh.cooldown": 1.9,
    
    "kich_doc_chu_nga.attack": 14,
    "kich_doc_chu_nga.cooldown": 1.6,
    
    "tu_kim_trao.attack": 9,
    "tu_kim_trao.cooldown": 1.7,
    
    "u_minh_quy_trao.attack": 19,
    "u_minh_quy_trao.cooldown": 1.8,
    
    "bach_ho_cuong_mong.attack": 6,
    "bach_ho_cuong_mong.cooldown": 1.8,
    
    "phuc_ho_con.attack": 14,
    "phuc_ho_con.cooldown": 2.6,
    
    "bich_thuy_cam.attack": 22,
    "bich_thuy_cam.cooldown": 2.7,
    
    "thanh_dang_tien.attack": 17,
    "thanh_dang_tien.cooldown": 2.4,
    
    "that_tinh_tieu.attack": 6,
    "that_tinh_tieu.cooldown": 2.1,
    
    "ngoc_cot_phien.attack": 8,
    "ngoc_cot_phien.cooldown": 2.4,
    
    "phan_quan_but.attack": 15,
    "phan_quan_but.cooldown": 2.7,
    
    "han_ngoc_tieu.attack": 15,
    "han_ngoc_tieu.cooldown": 2.8,

    # Triggers
    "kiem_tong_tam_yeu.triggers.charge": 0.25,
    "ngoc_cot_phien.triggers.charge": 0.35,
    "bach_luyen_chan_kinh.triggers.add_attack": 1,
    "bach_luyen_chan_kinh.triggers.charge": 0.7,
    "thai_thanh_tam_kinh.triggers.parry_discount": 10,
    "thien_tam_chuoi.triggers.parry_discount": 5,
}

if __name__ == "__main__":
    print("Testing base_patch...")
    outside, min_avg, max_avg, outlier_count, build_avgs = evaluate(base_patch, base_cfg, runs=40)
    print(f"Result: Outside={outside}/192, min_avg={min_avg:.1f}%, max_avg={max_avg:.1f}%, outliers={outlier_count}")
    for name, avg in build_avgs:
        print(f"  {name}: {avg:.1f}%")
