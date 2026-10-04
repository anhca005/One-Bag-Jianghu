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

from test_targeted_unlocks import run_matrix, apply_changes

patch = {
    # 1. Whip: tone down T3 charge spam (0.8 -> 0.25), attack 18, cooldown 2.3
    "thanh_dang_tien.attack": 18,
    "thanh_dang_tien.cooldown": 2.3,
    "thanh_dang_tien.unlock.t3.charge": 0.25,
    
    # 2. Cam: attack 16, cooldown 2.2, basic heal 12, gain_qi 12
    "bich_thuy_cam.attack": 16,
    "bich_thuy_cam.cooldown": 2.2,
    "bich_thuy_cam.basic.heal": 12,
    "bich_thuy_cam.basic.gain_qi": 12,
    
    # 3. Thai Thanh: parry charge 0.3s, parry discount 5, gain_qi 6
    "thai_thanh_tam_kinh.unlock.t6.charge": 0.3,
    "thai_thanh_tam_kinh.trigger.parry_discount": 5,
    "thai_thanh_tam_kinh.trigger.gain_qi": 6,
    "kiem_tong_tam_yeu.trigger.charge": 0.1,
    
    # 4. Axe & Hammer: cooldowns 2.4s/2.5s, ultimate costs 38/40
    "khai_son_phu.cooldown": 2.4,
    "khai_son_phu.ultimate.cost": 38,
    "chan_nhac_chuy.cooldown": 2.4,
    "chan_nhac_chuy.ultimate.cost": 40,
    "o_thiet_giap.unlock.t6.shield": 15,
    "o_thiet_giap.unlock.t9.shield": 90,
    
    # 5. Liệt Diễm: reduce early burst
    "liet_diem_dao.attack": 23,
    "xich_sa_chuong.attack": 15,
    "liet_hoa_tam_kinh.trigger.charge": 0.5,
    
    # 6. Poison: tone down burst charge loop at T6/T9
    "that_tinh_tieu.unlock.t9.charge": 0.3,
    "van_doc_dinh.unlock.t6.charge": 0.3,
}

items = apply_changes(base_items, patch)
outs, el, eh, text = run_matrix(items, runs=60)
print(f"Matrix Result: outs={outs}/192, extreme_low={el}, extreme_high={eh}")

cols = ["T1/TP1", "T1/TP9", "T3/TP1", "T3/TP9", "T6/TP1", "T6/TP9", "T9/TP1", "T9/TP9"]
for line in text.splitlines():
    if line.startswith("Loadout") or line.startswith("-") or line.startswith("Outside") or not line.strip():
        continue
    pcts = [p for p in line.split() if p.endswith("%") or p.endswith("%!")]
    if len(pcts) == 8:
        first_pct_idx = line.find(pcts[0])
        name = line[:first_pct_idx].strip()
        for col, p in zip(cols, pcts):
            val = float(p.replace("%!", "").replace("%", ""))
            if val < 15.0:
                print(f"LOW (<15%):  {name:<36} {col}: {val:5.1f}%")
            elif val > 85.0:
                print(f"HIGH (>85%): {name:<36} {col}: {val:5.1f}%")
