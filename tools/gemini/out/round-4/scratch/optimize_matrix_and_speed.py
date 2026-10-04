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
classpath = balance.ensure_classpath()

items_base = json.load(open(balance.DEFAULT_ITEMS, encoding="utf-8"))

# Base config
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

# Candidate patch
patch_cand = {
    # Scale weapon attacks (~1.4x - 1.5x) and tune cooldowns
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

# Test applying patch and evaluate
test_items = copy.deepcopy(items_base)
test_sects = balance.read_json(balance.DEFAULT_SECTS)

for addr, val in patch_cand.items():
    balance.parameter(test_items, test_sects, cfg, addr, val, set_value=True)

with tempfile.TemporaryDirectory(prefix="test_opt_") as raw_temp:
    temp = Path(raw_temp)
    f_items = temp / "items.json"
    f_cfg = temp / "config.json"
    balance.write_json(f_items, test_items, indent=None)
    balance.write_json(f_cfg, cfg, indent=None)

    # 1. Fight length check (T1, T3, T6, T9)
    source = balance.load_source_loadouts(LOADOUTS)
    durations = []
    over_45 = 0
    total = 0
    for tc in [
        {"name": "T1", "t": 1, "lvl": 1, "r": "common"},
        {"name": "T3", "t": 3, "lvl": 8, "r": "uncommon"},
        {"name": "T6", "t": 6, "lvl": 18, "r": "rare"},
        {"name": "T9", "t": 9, "lvl": 30, "r": "epic"}
    ]:
        t_durations = []
        t_over45 = 0
        t_tot = 0
        for s_idx, (fname, orig) in enumerate(source):
            ld = copy.deepcopy(orig)
            ld["level"] = tc["lvl"]
            ld["tamPhapLevel"] = 1
            for it in ld["items"]:
                it["tier"] = tc["t"]
                it["rarity"] = tc["r"]
            p = temp / f"m_{s_idx}_{tc['t']}.json"
            balance.write_json(p, ld, indent=None)

        for i in range(len(source)):
            p_a = temp / f"m_{i}_{tc['t']}.json"
            for j in [i, (i + 1) % len(source)]:
                p_b = temp / f"m_{j}_{tc['t']}.json"
                cmd = ["java", "-cp", classpath, "com.onebagjianghu.battle.cli.BattleCli", str(p_a), str(p_b), "--runs", "15", "--items", str(f_items), "--battle-config", str(f_cfg)]
                res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
                for line in res.stdout.splitlines():
                    if "thời lượng TB" in line:
                        d = float(line.split("thời lượng TB")[1].strip().replace("s", ""))
                        t_durations.append(d)
                        t_tot += 1
                        if d > 45.0:
                            t_over45 += 1

        avg_t = sum(t_durations) / len(t_durations) if t_durations else 0
        pct_t = (t_over45 / t_tot * 100.0) if t_tot else 0
        print(f"Tier {tc['t']}: avg = {avg_t:.1f}s, >45s = {pct_t:.1f}% ({t_over45}/{t_tot})")
        durations.extend(t_durations)
        over_45 += t_over45
        total += t_tot

    avg_all = sum(durations) / len(durations) if durations else 0
    pct_all = (over_45 / total * 100.0) if total else 0
    print(f"\nOVERALL FIGHT LENGTH: avg = {avg_all:.1f}s, >45s = {pct_all:.1f}% ({over_45}/{total})")

    # 2. Matrix A check (tiers 1,3,6,9; tp 1,9; runs 50)
    print("\nRunning Matrix A check...")
    cmd_mat = [
        "py", "tools/balance/balance.py", "matrix",
        "--loadouts", str(LOADOUTS),
        "--items", str(f_items),
        "--battle-config", str(f_cfg),
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", "50",
        "--level", "10"
    ]
    res_mat = subprocess.run(cmd_mat, capture_output=True, text=True, encoding="utf-8", errors="replace")
    for line in res_mat.stdout.splitlines():
        if "Outside [35, 65]:" in line:
            print("Matrix A Result:", line.strip())
        elif line.startswith("#") or (len(line) > 10 and "%" in line and not line.startswith("Loadout")):
            parts = line.strip().split()
            # print build averages or outliers
            row_vals = [float(p[:-1].replace("!", "")) for p in parts if p.endswith("%") or p.endswith("%!")]
            if row_vals:
                avg = sum(row_vals) / len(row_vals)
                if avg < 30.0 or avg > 70.0:
                    print(f"  OUTLIER BUILD: {parts[0]} avg = {avg:.1f}%")
