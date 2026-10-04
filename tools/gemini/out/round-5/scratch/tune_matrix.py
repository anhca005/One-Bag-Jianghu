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

def make_config(cfg_flat):
    if not cfg_flat:
        return None
    res = {}
    for k, v in cfg_flat.items():
        if "." in k:
            grp, prop = k.split(".", 1)
            res.setdefault(grp, {})[prop] = v
        else:
            res[k] = v
    return res

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
            balance.write_json(f_cfg, make_config(config), indent=None)
            cmd += ["--battle-config", str(f_cfg)]
            
        res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
        out = res.stdout
        if res.stderr:
            print("STDERR:", res.stderr[:200])
        
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
        elif parts[0] == "trigger":
            for trig in it.get("triggers", []):
                for eff in trig.get("effects", []):
                    if eff.get("op") == parts[1]:
                        if "value" in eff: eff["value"] = val
                        elif "duration" in eff: eff["duration"] = val
                        elif "stacks" in eff: eff["stacks"] = val
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
        if full_key.startswith("config:"):
            continue
        parts = full_key.split(".", 1)
        item_id = parts[0]
        sub_key = parts[1]
        if item_id in by_id:
            modify_item(by_id[item_id], sub_key, val)
    return items

# Baseline test
outs, el, eh, _ = run_matrix(base_items, runs=60)
print(f"Base: outs={outs}/192, extreme_low={el}, extreme_high={eh}")

# Candidate 1: Whip & Cam
p1 = {
    "thanh_dang_tien.attack": 18,
    "thanh_dang_tien.cooldown": 2.2,
    "thanh_dang_tien.unlock.t3.charge": 0.35,
    "bich_thuy_cam.attack": 15,
    "bich_thuy_cam.cooldown": 2.2,
}
outs, el, eh, _ = run_matrix(apply_changes(base_items, p1), runs=60)
print(f"P1 (Whip & Cam): outs={outs}/192, extreme_low={el}, extreme_high={eh}")

# Candidate 2: + Axe & Hammer
p2 = dict(p1)
p2.update({
    "khai_son_phu.cooldown": 2.3,
    "khai_son_phu.ultimate.cost": 38,
    "chan_nhac_chuy.cooldown": 2.4,
    "chan_nhac_chuy.ultimate.cost": 40,
    "o_thiet_giap.stats.hp": 180,
    "o_thiet_giap.unlock.t6.shield": 12,
})
outs, el, eh, _ = run_matrix(apply_changes(base_items, p2), runs=60)
print(f"P2 (+ Axe & Hammer): outs={outs}/192, extreme_low={el}, extreme_high={eh}")

# Candidate 3: + Thai Thanh Sword nerf
p3 = dict(p2)
p3.update({
    "thai_thanh_tam_kinh.unlock.t6.charge": 0.4,
    "thai_thanh_tam_kinh.trigger.gain_qi": 6,
})
outs, el, eh, _ = run_matrix(apply_changes(base_items, p3), runs=60)
print(f"P3 (+ Sword nerf): outs={outs}/192, extreme_low={el}, extreme_high={eh}")

# Candidate 4: + Poison & Burn tuning
p4 = dict(p3)
p4.update({
    "that_tinh_tieu.attack": 7,
    "that_tinh_tieu.cooldown": 2.0,
    "that_tinh_tieu.unlock.t9.charge": 0.6,
    "van_doc_dinh.unlock.t6.charge": 0.6,
    "liet_diem_dao.attack": 23,
    "xich_sa_chuong.attack": 15,
})
cfg4 = {
    "poison.damage": 28,
    "burn.damage": 65
}
outs, el, eh, _ = run_matrix(apply_changes(base_items, p4), cfg4, runs=60)
print(f"P4 (+ Poison & Burn): outs={outs}/192, extreme_low={el}, extreme_high={eh}")
