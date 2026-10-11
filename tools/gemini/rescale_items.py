#!/usr/bin/env python3
"""Rescale item stats for Decision #61 (power proportional to cells)."""
from __future__ import annotations

import copy
import csv
import json
import math
import sys
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import power  # noqa: E402

ORIGINAL_ITEMS_PATH = ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/items.json"
OUT_DIR = ROOT / "tools/gemini/out/power-per-cell"
OUT_ITEMS_PATH = OUT_DIR / "items.json"
OUT_CHANGES_PATH = OUT_DIR / "changes.csv"
OUT_POWER_PATH = OUT_DIR / "power.json"


def round_smart(val: float, orig: int | float) -> int | float:
    if isinstance(orig, int):
        res = round(val)
        if orig >= 1 and res < 1:
            res = 1
        return res
    else:
        return round(val, 3)


def scale_effect(effect: dict[str, Any], k: float, changes: list, item_id: str, path_prefix: str):
    for key in ["value", "pct"]:
        if key in effect and isinstance(effect[key], (int, float)):
            old_val = effect[key]
            if key == "pct":
                new_val = round(old_val * k, 4)
            else:
                new_val = round_smart(old_val * k, old_val)
            
            if new_val != old_val:
                effect[key] = new_val
                changes.append({
                    "item_id": item_id,
                    "field_path": f"{path_prefix}.{key}",
                    "old_value": old_val,
                    "new_value": new_val,
                    "factor": round(k, 4)
                })


def apply_item_scale(item: dict[str, Any], k: float) -> list[dict[str, Any]]:
    changes = []
    item_id = item["id"]
    
    # 1. Weapon attack and cooldown
    if "attack" in item and isinstance(item["attack"], (int, float)):
        old_atk = item["attack"]
        old_cd = item.get("cooldown", 1.0)
        
        # If attack is small and k < 1, increase cooldown instead of reducing attack to 0
        if old_atk <= 2 and k < 0.8:
            new_cd = round(old_cd / k, 2)
            if new_cd != old_cd:
                item["cooldown"] = new_cd
                changes.append({
                    "item_id": item_id,
                    "field_path": "cooldown",
                    "old_value": old_cd,
                    "new_value": new_cd,
                    "factor": round(1.0 / k, 4)
                })
        else:
            new_atk = round_smart(old_atk * k, old_atk)
            if new_atk != old_atk:
                item["attack"] = new_atk
                changes.append({
                    "item_id": item_id,
                    "field_path": "attack",
                    "old_value": old_atk,
                    "new_value": new_atk,
                    "factor": round(k, 4)
                })

    # 2. Basic effects
    if "basic" in item and isinstance(item["basic"], list):
        for i, eff in enumerate(item["basic"]):
            scale_effect(eff, k, changes, item_id, f"basic[{i}]")
            
    # 3. Triggers
    if "triggers" in item and isinstance(item["triggers"], list):
        for i, trig in enumerate(item["triggers"]):
            if "effects" in trig and isinstance(trig["effects"], list):
                for j, eff in enumerate(trig["effects"]):
                    scale_effect(eff, k, changes, item_id, f"triggers[{i}].effects[{j}]")
                    
    # 4. Unlocks
    if "unlocks" in item and isinstance(item["unlocks"], list):
        for u_idx, unlock in enumerate(item["unlocks"]):
            if "basic" in unlock and isinstance(unlock["basic"], list):
                for i, eff in enumerate(unlock["basic"]):
                    scale_effect(eff, k, changes, item_id, f"unlocks[{u_idx}].basic[{i}]")
            if "triggers" in unlock and isinstance(unlock["triggers"], list):
                for i, trig in enumerate(unlock["triggers"]):
                    if "effects" in trig and isinstance(trig["effects"], list):
                        for j, eff in enumerate(trig["effects"]):
                            scale_effect(eff, k, changes, item_id, f"unlocks[{u_idx}].triggers[{i}].effects[{j}]")

    return changes


def main():
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")
        
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    orig_items = json.loads(ORIGINAL_ITEMS_PATH.read_text(encoding="utf-8"))
    
    modified_items = copy.deepcopy(orig_items)
    all_changes_dict = {}
    
    for pass_num in range(1, 3):
        print(f"--- Iteration Pass {pass_num} ---", flush=True)
        OUT_ITEMS_PATH.write_text(json.dumps(modified_items, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
        power.ITEMS = OUT_ITEMS_PATH
        
        runner = power.Runner()
        raw_items, measurements = power.measure_items(runner, 15)
        
        tpk_off = raw_items["thanh_phong_kiem"]["offense"]
        oty_def = raw_items["o_tam_y"]["defense"]
        budget_off_cell = tpk_off / 3.0
        budget_def_cell = oty_def / 4.0
        
        print(f"Pass {pass_num} Anchor budget: off_cell={budget_off_cell:.4f}, def_cell={budget_def_cell:.4f}")
        
        for item in modified_items:
            iid = item["id"]
            w, h = item["size"]
            cells = w * h
            m = raw_items.get(iid, {"offense": 0.0, "defense": 0.0})
            off = m["offense"]
            defe = m["defense"]
            
            equiv_cells = (off / budget_off_cell) + (defe / budget_def_cell)
            
            if equiv_cells < 0.05:
                continue
                
            k = cells / equiv_cells
            k_soft = max(0.05, min(10.0, k))
            
            if abs(k_soft - 1.0) > 0.08:
                chg = apply_item_scale(item, k_soft)
                for c in chg:
                    key = (c["item_id"], c["field_path"])
                    if key not in all_changes_dict:
                        all_changes_dict[key] = c
                    else:
                        all_changes_dict[key]["new_value"] = c["new_value"]
                        orig_val = all_changes_dict[key]["old_value"]
                        new_val = c["new_value"]
                        all_changes_dict[key]["factor"] = round(new_val / orig_val, 4) if orig_val else 1.0

    # Save candidate items.json
    OUT_ITEMS_PATH.write_text(json.dumps(modified_items, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Saved final rescaled items to {OUT_ITEMS_PATH}")
    
    final_changes = list(all_changes_dict.values())
    with OUT_CHANGES_PATH.open("w", encoding="utf-8", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=["item_id", "field_path", "old_value", "new_value", "factor"])
        writer.writeheader()
        writer.writerows(final_changes)
        
    print(f"Recorded {len(final_changes)} stat field changes to {OUT_CHANGES_PATH}")


if __name__ == "__main__":
    main()
