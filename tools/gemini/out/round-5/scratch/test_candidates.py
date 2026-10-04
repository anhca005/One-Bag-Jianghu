import copy
import json
import subprocess
import sys
import tempfile
from pathlib import Path

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

def modify_items(items, changes):
    items = copy.deepcopy(items)
    by_id = {it["id"]: it for it in items}
    for item_id, mods in changes.items():
        if item_id not in by_id:
            continue
        it = by_id[item_id]
        for k, v in mods.items():
            if k == "attack": it["attack"] = v
            elif k == "cooldown": it["cooldown"] = v
            elif k == "crit": it["crit"] = v
            elif k == "hp": it.setdefault("stats", {})["hp"] = v
            elif k == "ultimate_cost":
                if it.get("ultimate"): it["ultimate"]["cost"] = v
            elif k.startswith("unlock_t"):
                tier = int(k.split("_")[1].replace("t", ""))
                prop = "_".join(k.split("_")[2:])
                for unl in it.get("unlocks", []):
                    if unl.get("tier") == tier:
                        if prop == "attack": unl["attack"] = v
                        elif prop == "crit": unl["crit"] = v
                        elif prop == "charge":
                            for trig in unl.get("triggers", []):
                                for eff in trig.get("effects", []):
                                    if eff.get("op") == "charge":
                                        eff["duration"] = v
                        elif prop == "shield":
                            for trig in unl.get("triggers", []):
                                for eff in trig.get("effects", []):
                                    if eff.get("op") == "shield":
                                        eff["value"] = v
            elif k.startswith("trigger_"):
                prop = k.replace("trigger_", "")
                for trig in it.get("triggers", []):
                    for eff in trig.get("effects", []):
                        if eff.get("op") == prop:
                            eff["duration" if prop == "charge" else "value"] = v
    return items

candidates = [
    ("Base", {}, None),
    
    # 1. Whip fix
    ("Whip charge 0.3s + attack 18", {
        "thanh_dang_tien": {"attack": 18, "cooldown": 2.2, "unlock_t3_charge": 0.3}
    }, None),
    
    # 2. Cam fix
    ("Cam attack 15 + cd 2.2", {
        "bich_thuy_cam": {"attack": 15, "cooldown": 2.2}
    }, None),
    
    # 3. Sword / Parry fix
    ("Thai Thanh Parry charge 0.4s", {
        "thai_thanh_tam_kinh": {"unlock_t6_charge": 0.4}
    }, None),
    
    # 4. Axe / Hammer fix
    ("Axe/Hammer speed & cost", {
        "khai_son_phu": {"cooldown": 2.4, "ultimate_cost": 38},
        "chan_nhac_chuy": {"cooldown": 2.4, "ultimate_cost": 40},
        "o_thiet_giap": {"hp": 180, "unlock_t6_shield": 12}
    }, None),
    
    # 5. Poison taming
    ("Poison taming", {
        "that_tinh_tieu": {"attack": 7, "cooldown": 2.0, "unlock_t9_charge": 0.5},
        "van_doc_dinh": {"unlock_t6_charge": 0.6}
    }, {"poison.damage": 28}),
    
    # 6. Burn taming
    ("Burn taming", {
        "liet_diem_dao": {"attack": 23},
        "xich_sa_chuong": {"attack": 15}
    }, {"burn.damage": 65}),
]

print("Testing individual candidates at 60 runs:")
for name, mods, cfg in candidates:
    it = modify_items(base_items, mods)
    outs, el, eh, _ = run_matrix(it, cfg, runs=60)
    print(f"{name:<35}: outs={outs:3d}/192, extreme_low={el:2d}, extreme_high={eh:2d}")

# Now test cumulative combination of all 6
all_mods = {}
all_cfg = {}
for name, mods, cfg in candidates[1:]:
    all_mods.update(mods)
    if cfg:
        all_cfg.update(cfg)

print("\nTesting FULL COMBINATION of all 6 changes:")
it_all = modify_items(base_items, all_mods)
outs, el, eh, out_text = run_matrix(it_all, all_cfg, runs=60)
print(f"FULL COMBINATION: outs={outs:3d}/192, extreme_low={el:2d}, extreme_high={eh:2d}")
