import copy
import json
import subprocess
import sys
import tempfile
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

classpath = balance.ensure_classpath()
base_items = json.load(open("server/battle-engine/src/main/resources/data/items.json", encoding="utf-8"))

from test_targeted_unlocks import apply_changes

candidate_patch = {
    # 1. Thai Thanh Parry loop
    "thai_thanh_tam_kinh.unlock.t6.charge": 0.3,
    "thai_thanh_tam_kinh.trigger.parry_discount": 5,
    "kiem_tong_tam_yeu.trigger.charge": 0.1,
    
    # 2. Whip: tone down T3 charge spam, modest early buff
    "thanh_dang_tien.attack": 18,
    "thanh_dang_tien.cooldown": 2.3,
    "thanh_dang_tien.unlock.t3.charge": 0.25,
    
    # 3. Cam: base attack & cd
    "bich_thuy_cam.attack": 15,
    "bich_thuy_cam.cooldown": 2.3,
    "bich_thuy_cam.basic.gain_qi": 10,
    
    # 4. Hammer speed & ultimate
    "chan_nhac_chuy.cooldown": 2.5,
    "chan_nhac_chuy.ultimate.cost": 42,
    
    # 5. Heavy armor scaling
    "o_thiet_giap.unlock.t6.shield": 12,
    "o_thiet_giap.unlock.t9.shield": 80,
    
    # 6. Poison loop tame
    "that_tinh_tieu.unlock.t9.charge": 0.4,
    "van_doc_dinh.unlock.t6.charge": 0.4,
}

items = apply_changes(base_items, candidate_patch)

with tempfile.TemporaryDirectory(prefix="check_") as raw_temp:
    temp = Path(raw_temp)
    f_items = temp / "items.json"
    balance.write_json(f_items, items, indent=None)
    
    # 1. Matrix A
    print("=== 1. Matrix A ===")
    cmd_m = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-5/scratch/MatrixRunner.java",
        "--items", str(f_items),
        "--runs", "100",
        "--level", "10"
    ]
    res_m = subprocess.run(cmd_m, capture_output=True, text=True, encoding="utf-8")
    for line in res_m.stdout.splitlines():
        if "Outside" in line:
            print(line)
            
    # 2. Tier Step
    print("\n=== 2. Tier Step ===")
    cmd_t = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-5/scratch/TierStepRunner.java",
        "--items", str(f_items),
        "--runs", "100"
    ]
    res_t = subprocess.run(cmd_t, capture_output=True, text=True, encoding="utf-8")
    for line in res_t.stdout.splitlines():
        if "Average" in line:
            print(line)
            
    # 3. Fight Length
    print("\n=== 3. Fight Length ===")
    cmd_f = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-5/scratch/FightLengthRunner.java",
        "--items", str(f_items)
    ]
    res_f = subprocess.run(cmd_f, capture_output=True, text=True, encoding="utf-8")
    print(res_f.stdout)
    
    # 4. Rarity Step
    print("\n=== 4. Rarity Step ===")
    cmd_r = [
        "py", "tools/balance/balance.py", "rarity-step",
        "--loadouts", "server/battle-engine/samples",
        "--items", str(f_items),
        "--tier", "3", "--tp", "1", "--level", "10", "--runs", "100"
    ]
    res_r = subprocess.run(cmd_r, capture_output=True, text=True, encoding="utf-8")
    for line in res_r.stdout.splitlines():
        if "Average" in line:
            print(line)
    # count 100% cells
    from benchmarks import count_100_cells
    c100 = count_100_cells(res_r.stdout)
    print(f"Number of 100% cells: {c100} (target <= 31)")
