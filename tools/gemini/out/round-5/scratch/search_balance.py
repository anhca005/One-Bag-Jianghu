import copy
import json
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path("d:/CONG VIEC\One-Bag-Jianghu")
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
                # Outside [35, 65]: X/192 (extreme <15%: Y, >85%: Z)
                parts = line.split()
                # find X/192
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
                # e.g. "unlock_t3_charge": 0.4
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
                # e.g. "trigger_charge": 0.4
                prop = k.replace("trigger_", "")
                for trig in it.get("triggers", []):
                    for eff in trig.get("effects", []):
                        if eff.get("op") == prop:
                            eff["duration" if prop == "charge" else "value"] = v
    return items

if __name__ == "__main__":
    outs, el, eh, text = run_matrix(base_items, runs=60)
    print(f"Baseline at 60 runs: outside={outs}, extreme_low={el}, extreme_high={eh}")
