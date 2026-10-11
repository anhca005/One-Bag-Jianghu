#!/usr/bin/env python3
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import power

power.ITEMS = ROOT / "tools/gemini/out/power-per-cell/items.json"
runner = power.Runner()
raw_items, measurements = power.measure_items(runner, 15)

tpk_off = raw_items["thanh_phong_kiem"]["offense"]
oty_def = raw_items["o_tam_y"]["defense"]
budget_off_cell = tpk_off / 3.0
budget_def_cell = oty_def / 4.0

print(f"Anchor Budget: off_cell={budget_off_cell:.4f}, def_cell={budget_def_cell:.4f}")

out_of_band = []
in_band = []
for item in runner.catalog:
    iid = item["id"]
    itype = item["type"]
    w, h = item["size"]
    cells = w * h
    m = raw_items.get(iid, {"offense": 0.0, "defense": 0.0})
    off = m["offense"]
    defe = m["defense"]
    equiv = (off / budget_off_cell) + (defe / budget_def_cell)
    ppc = equiv / cells if cells > 0 else 0.0
    rec = {"id": iid, "type": itype, "size": f"{w}x{h}", "cells": cells, "offense": off, "defense": defe, "ppc": ppc}
    if 0.90 <= ppc <= 1.10:
        in_band.append(rec)
    else:
        out_of_band.append(rec)

print(f"Total items measured: {len(runner.catalog)}")
print(f"In-band items: {len(in_band)} / {len(runner.catalog)}")
print(f"Out-of-band items ({len(out_of_band)}):")
print(f"{'ID':28s} | {'Type':7s} | {'Size':4s} | {'Cells':5s} | {'Offense':7s} | {'Defense':8s} | {'Power/Cell':10s}")
print("-" * 85)
for r in sorted(out_of_band, key=lambda x: -x["ppc"]):
    print(f"{r['id']:28s} | {r['type']:7s} | {r['size']:4s} | {r['cells']:5d} | {r['offense']:7.2f} | {r['defense']:8.2f} | {r['ppc']:10.3f}")
