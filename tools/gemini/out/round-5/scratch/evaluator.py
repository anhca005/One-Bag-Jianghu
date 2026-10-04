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

def apply_patch(items_list, patch):
    items = copy.deepcopy(items_list)
    by_id = {it["id"]: it for it in items}
    
    for key, val in patch.items():
        if key.startswith("config:"):
            continue
        parts = key.split(".")
        item_id = parts[0]
        if item_id not in by_id:
            print(f"Warning: {item_id} not found in items")
            continue
        it = by_id[item_id]
        
        if len(parts) == 2:
            prop = parts[1]
            if prop in ("attack", "cooldown", "crit"):
                it[prop] = val
            elif prop in ("hp", "cooldownReduction", "dmgReduce"):
                it.setdefault("stats", {})[prop] = val
        elif len(parts) == 3 and parts[1] == "ultimate" and parts[2] == "cost":
            if it.get("ultimate"):
                it["ultimate"]["cost"] = val
        elif len(parts) == 3 and parts[1] == "basic":
            op_or_status = parts[2]
            for eff in it.get("basic", []):
                if eff.get("op") == op_or_status or eff.get("status") == op_or_status:
                    if "value" in eff: eff["value"] = val
                    elif "stacks" in eff: eff["stacks"] = val
                    elif "duration" in eff: eff["duration"] = val
                    elif "pct" in eff: eff["pct"] = val
                    break
        elif len(parts) == 3 and parts[1] == "triggers":
            op = parts[2]
            for trig in it.get("triggers", []):
                for eff in trig.get("effects", []):
                    if eff.get("op") == op:
                        if "value" in eff: eff["value"] = val
                        elif "stacks" in eff: eff["stacks"] = val
                        elif "duration" in eff: eff["duration"] = val
                        elif "pct" in eff: eff["pct"] = val
                        break
        elif len(parts) == 4 and parts[1] == "unlocks":
            # e.g. item_id.unlocks.tier_3.attack = val
            tier_num = int(parts[2].replace("tier_", "").replace("t", ""))
            prop = parts[3]
            for unl in it.get("unlocks", []):
                if unl.get("tier") == tier_num:
                    unl[prop] = val
                    break
    return items

def evaluate_matrix(items, config=None, runs=60):
    with tempfile.TemporaryDirectory(prefix="ev_") as raw_temp:
        temp = Path(raw_temp)
        f_items = temp / "items.json"
        balance.write_json(f_items, items, indent=None)
        
        cfg_args = []
        if config:
            f_cfg = temp / "config.json"
            balance.write_json(f_cfg, config, indent=None)
            cfg_args = ["--battle-config", str(f_cfg)]
            
        cmd = [
            "py", "tools/balance/balance.py", "matrix",
            "--items", str(f_items),
            "--tiers", "1,3,6,9",
            "--tp", "1,9",
            "--runs", str(runs),
            "--level", "10"
        ] + cfg_args
        
        res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
        out = res.stdout
        
        outs = 0
        extreme_low = 0
        extreme_high = 0
        lines = out.splitlines()
        for line in lines:
            if "Outside" in line:
                # e.g. Outside [35, 65]: 103/192
                parts = line.split(":")
                outs = int(parts[1].strip().split("/")[0])
            if line.startswith("#") or any(line.startswith(prefix) for prefix in ["Bích", "Hàn", "Huyền", "Kim", "Thái", "Thiết", "Liệt"]):
                pcts = [p for p in line.split() if p.endswith("%") or p.endswith("%!")]
                for p in pcts:
                    val = float(p.replace("%!", "").replace("%", ""))
                    if val < 15.0: extreme_low += 1
                    if val > 85.0: extreme_high += 1
                    
        return outs, extreme_low, extreme_high, out

if __name__ == "__main__":
    outs, el, eh, text = evaluate_matrix(base_items, runs=60)
    print(f"Base at 60 runs: outs={outs}/192, extreme_low(<15%)={el}, extreme_high(>85%)={eh}")
