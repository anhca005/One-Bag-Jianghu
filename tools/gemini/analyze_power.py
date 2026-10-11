#!/usr/bin/env python3
"""Analyze item power per cell baseline and outliers for Decision #61."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import power  # noqa: E402

ITEMS_PATH = ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/items.json"


def load_items() -> list[dict[str, Any]]:
    return json.loads(ITEMS_PATH.read_text(encoding="utf-8"))


def analyze_baseline():
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")
    runner = power.Runner()
    raw_items, measurements = power.measure_items(runner, 15)
    catalog = runner.catalog
    
    # Anchor budget per cell:
    # Thanh Phong Kiem (1x3 = 3 cells): weapon -> target off_per_cell
    # O Tam Y (2x2 = 4 cells): armor -> target def_per_cell
    tpk_off = raw_items["thanh_phong_kiem"]["offense"]
    oty_def = raw_items["o_tam_y"]["defense"]
    
    budget_off_cell = tpk_off / 3.0
    budget_def_cell = oty_def / 4.0
    
    print(f"=== ANCHOR BUDGET ===")
    print(f"Thanh Phong Kiếm (3 cells): off={tpk_off:.2f} -> {budget_off_cell:.2f} off/cell")
    print(f"Ô Tàm Y (4 cells): def={oty_def:.2f} -> {budget_def_cell:.2f} def/cell")
    
    results = []
    for item in catalog:
        iid = item["id"]
        itype = item["type"]
        w, h = item["size"]
        cells = w * h
        m = raw_items.get(iid, {"offense": 0.0, "defense": 0.0})
        off = m["offense"]
        defe = m["defense"]
        
        equiv_cells = (off / budget_off_cell) + (defe / budget_def_cell)
        power_per_cell = equiv_cells / cells if cells > 0 else 0.0
        
        results.append({
            "id": iid,
            "name": item["name"],
            "type": itype,
            "size": f"{w}x{h}",
            "cells": cells,
            "offense": off,
            "defense": defe,
            "equiv_cells": round(equiv_cells, 3),
            "power_per_cell": round(power_per_cell, 3),
            "target_equiv_cells": cells,
            "ratio": round(power_per_cell, 3) # target is 1.0 per cell
        })
    
    # Sort by type, cells, ratio
    results.sort(key=lambda x: (x["type"], x["cells"], -x["power_per_cell"]))
    
    print("\n=== BASELINE POWER PER CELL TABLE ===")
    print(f"{'ID':28s} | {'Type':7s} | {'Size':4s} | {'Cells':5s} | {'Offense':7s} | {'Defense':8s} | {'Power/Cell':10s} | {'Ratio':6s}")
    print("-" * 90)
    for r in results:
        print(f"{r['id']:28s} | {r['type']:7s} | {r['size']:4s} | {r['cells']:5d} | {r['offense']:7.2f} | {r['defense']:8.2f} | {r['power_per_cell']:10.3f} | {r['ratio']:6.2f}")
        
    # Group by type and size
    print("\n=== SUMMARY BY TYPE AND SIZE ===")
    groups = {}
    for r in results:
        key = (r["type"], r["size"], r["cells"])
        if key not in groups:
            groups[key] = []
        groups[key].append(r["power_per_cell"])
        
    for (itype, size, cells), vals in sorted(groups.items()):
        avg_p = sum(vals) / len(vals)
        min_p = min(vals)
        max_p = max(vals)
        print(f"Type: {itype:7s} | Size: {size:4s} ({cells}c) | Count: {len(vals):2d} | Min: {min_p:.2f} | Max: {max_p:.2f} | Avg: {avg_p:.2f}")

if __name__ == "__main__":
    analyze_baseline()
