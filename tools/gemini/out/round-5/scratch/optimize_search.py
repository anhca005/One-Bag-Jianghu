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

def run_matrix(items, config=None, runs=60):
    with tempfile.TemporaryDirectory(prefix="m_") as raw_temp:
        temp = Path(raw_temp)
        f_items = temp / "items.json"
        balance.write_json(f_items, items, indent=None)
        
        cmd = [
            "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
            "tools/gemini/out/round-5/scratch/MatrixRunner.java",
            "--items", str(f_items),
            "--runs", str(runs),
            "--level", "10"
        ]
        if config:
            f_cfg = temp / "config.json"
            balance.write_json(f_cfg, config, indent=None)
            cmd += ["--battle-config", str(f_cfg)]
            
        res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
        out = res.stdout
        
        outside = 0
        extreme_low = 0
        extreme_high = 0
        for line in out.splitlines():
            if "Outside" in line:
                parts = line.split()
                for p in parts:
                    if "/192" in p:
                        outside = int(p.split("/")[0])
                if "<15%:" in parts:
                    idx = parts.index("<15%:")
                    extreme_low = int(parts[idx+1].replace(",", ""))
                if ">85%:" in parts:
                    idx = parts.index(">85%:")
                    extreme_high = int(parts[idx+1].replace(")", ""))
        return outside, extreme_low, extreme_high, out

def modify_item(it, key, val):
    parts = key.split(".")
    if len(parts) == 1:
        it[parts[0]] = val
    elif len(parts) == 2:
        if parts[0] == "stats":
            it.setdefault("stats", {})[parts[1]] = val
        elif parts[0] == "ultimate":
            if it.get("ultimate"): it["ultimate"][parts[1]] = val
        elif parts[0] == "basic":
            for eff in it.get("basic", []):
                if eff.get("op") == parts[1] or eff.get("status") == parts[1]:
                    if "value" in eff: eff["value"] = val
                    elif "duration" in eff: eff["duration"] = val
                    elif "stacks" in eff: eff["stacks"] = val
                    elif "pct" in eff: eff["pct"] = val
    elif len(parts) == 3 and parts[0] == "unlock":
        t = int(parts[1].replace("t", ""))
        for unl in it.get("unlocks", []):
            if unl.get("tier") == t:
                if parts[2] in ("attack", "crit"):
                    unl[parts[2]] = val
                else:
                    for trig in unl.get("triggers", []):
                        for eff in trig.get("effects", []):
                            if eff.get("op") == parts[2]:
                                if "value" in eff: eff["value"] = val
                                elif "duration" in eff: eff["duration"] = val
                                elif "stacks" in eff: eff["stacks"] = val

def apply_changes(items_list, patch_dict):
    items = copy.deepcopy(items_list)
    by_id = {it["id"]: it for it in items}
    for full_key, val in patch_dict.items():
        parts = full_key.split(".", 1)
        item_id = parts[0]
        sub_key = parts[1]
        if item_id in by_id:
            modify_item(by_id[item_id], sub_key, val)
    return items

# Base test
outs, el, eh, _ = run_matrix(base_items, runs=60)
print(f"Base at 60 runs: outside={outs}, extreme_low={el}, extreme_high={eh}")

# Let's test a progressive build-up of fixes:
fixes = [
    # 1. Thai Thanh Sword Parry Charge nerf (tame 94% down)
    ("1. Thai Thanh Parry charge 0.3s", {
        "thai_thanh_tam_kinh.unlock.t6.charge": 0.3
    }),
    # 2. Poison T9 loop nerf (tame 93% down)
    ("2. Poison burst charge 0.3s", {
        "thai_thanh_tam_kinh.unlock.t6.charge": 0.3,
        "that_tinh_tieu.unlock.t9.charge": 0.3,
        "van_doc_dinh.unlock.t6.charge": 0.3,
    }),
    # 3. Whip T3 spam nerf (tame 81% down)
    ("3. Whip T3 charge 0.25s", {
        "thai_thanh_tam_kinh.unlock.t6.charge": 0.3,
        "that_tinh_tieu.unlock.t9.charge": 0.3,
        "van_doc_dinh.unlock.t6.charge": 0.3,
        "thanh_dang_tien.unlock.t3.charge": 0.25,
    }),
    # 4. Axe & Hammer survival buff (rescue 7% up to ~45%)
    ("4. Axe/Hammer/Armor buffs", {
        "thai_thanh_tam_kinh.unlock.t6.charge": 0.3,
        "that_tinh_tieu.unlock.t9.charge": 0.3,
        "van_doc_dinh.unlock.t6.charge": 0.3,
        "thanh_dang_tien.unlock.t3.charge": 0.25,
        "khai_son_phu.cooldown": 2.4,
        "khai_son_phu.ultimate.cost": 40,
        "chan_nhac_chuy.cooldown": 2.5,
        "chan_nhac_chuy.ultimate.cost": 42,
        "o_thiet_giap.unlock.t6.shield": 15,
        "o_thiet_giap.unlock.t9.shield": 90,
    }),
    # 5. Liệt Diễm slight early tone down
    ("5. Liệt Diễm early attack tweak", {
        "thai_thanh_tam_kinh.unlock.t6.charge": 0.3,
        "that_tinh_tieu.unlock.t9.charge": 0.3,
        "van_doc_dinh.unlock.t6.charge": 0.3,
        "thanh_dang_tien.unlock.t3.charge": 0.25,
        "khai_son_phu.cooldown": 2.4,
        "khai_son_phu.ultimate.cost": 40,
        "chan_nhac_chuy.cooldown": 2.5,
        "chan_nhac_chuy.ultimate.cost": 42,
        "o_thiet_giap.unlock.t6.shield": 15,
        "o_thiet_giap.unlock.t9.shield": 90,
        "liet_diem_dao.attack": 24,
        "xich_sa_chuong.attack": 16,
    }),
    # 6. Cam modest base buff
    ("6. Cam base attack 13", {
        "thai_thanh_tam_kinh.unlock.t6.charge": 0.3,
        "that_tinh_tieu.unlock.t9.charge": 0.3,
        "van_doc_dinh.unlock.t6.charge": 0.3,
        "thanh_dang_tien.unlock.t3.charge": 0.25,
        "khai_son_phu.cooldown": 2.4,
        "khai_son_phu.ultimate.cost": 40,
        "chan_nhac_chuy.cooldown": 2.5,
        "chan_nhac_chuy.ultimate.cost": 42,
        "o_thiet_giap.unlock.t6.shield": 15,
        "o_thiet_giap.unlock.t9.shield": 90,
        "liet_diem_dao.attack": 24,
        "xich_sa_chuong.attack": 16,
        "bich_thuy_cam.attack": 13,
        "bich_thuy_cam.cooldown": 2.4,
    })
]

for label, patch in fixes:
    it = apply_changes(base_items, patch)
    outs, el, eh, text = run_matrix(it, runs=60)
    print(f"{label:<38}: outs={outs:3d}/192, extreme_low={el:2d}, extreme_high={eh:2d}")
