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

# Test 1: Nerve high-tier outliers via unlocks only
p_nerf = {
    # Whip: tone down T3 charge spam
    "thanh_dang_tien.unlock.t3.charge": 0.25,
    # Thai Thanh: tone down T6 parry charge spam
    "thai_thanh_tam_kinh.unlock.t6.charge": 0.3,
    # Poison: tone down T9 / T6 burst charge
    "that_tinh_tieu.unlock.t9.charge": 0.4,
    "van_doc_dinh.unlock.t6.charge": 0.4,
    # Kim Cang Axe/Hammer hybrid: tone down T3 attack buff
    "bach_luyen_chan_kinh.basic.attack": 1,
}

items1 = apply_changes(base_items, p_nerf)
outs, el, eh, text1 = run_matrix(items1, runs=60)
print(f"Step 1 (Nerf high-tier unlocks): outs={outs}/192, extreme_low={el}, extreme_high={eh}")

# Test 2: Add buffs to low-tier weak items via unlocks & modest base
p_buff = dict(p_nerf)
p_buff.update({
    # Help Axe & Hammer survive at T6/T9
    "o_thiet_giap.unlock.t6.shield": 15,
    "o_thiet_giap.unlock.t9.shield": 90,
    "chan_nhac_chuy.cooldown": 2.5,
    "chan_nhac_chuy.ultimate.cost": 42,
    "khai_son_phu.cooldown": 2.5,
    "khai_son_phu.ultimate.cost": 40,
    
    # Help Cam & Whip at T1
    "thanh_dang_tien.attack": 18,
    "thanh_dang_tien.cooldown": 2.3,
    "bich_thuy_cam.attack": 14,
    "bich_thuy_cam.cooldown": 2.3,
})

items2 = apply_changes(base_items, p_buff)
outs, el, eh, text2 = run_matrix(items2, runs=60)
print(f"Step 2 (Nerf high-tier + Buff weak): outs={outs}/192, extreme_low={el}, extreme_high={eh}")

print("\n--- Rows from Step 2 ---")
for line in text2.splitlines():
    if any(k in line for k in ["Thanh Đằng", "Cầm Âm", "Hóa Kình", "Hắc Huyết", "Vạn Độc", "Khai Sơn", "Chấn Nhạc", "Cuồng Đao"]):
        print(line)
