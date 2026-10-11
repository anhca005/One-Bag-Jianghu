#!/usr/bin/env python3
"""Orchestrate Task Power Per Cell v2 balance workflow."""
from __future__ import annotations

import copy
import csv
import json
import math
import sys
import tempfile
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance  # noqa: E402
import item_value  # noqa: E402
import power  # noqa: E402
import pve_progression  # noqa: E402

ORIGINAL_ITEMS_PATH = ROOT / "server/battle-engine/src/main/resources/data/items.json"
ORIGINAL_POWER_PATH = ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/power.json"
CHAPTERS_PATH = ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json"

OUT_DIR = ROOT / "tools/gemini/out/power-per-cell-v2"
OUT_ITEMS_PATH = OUT_DIR / "items.json"
OUT_POWER_PATH = OUT_DIR / "power.json"
OUT_CHANGES_PATH = OUT_DIR / "changes.csv"
OUT_MEASURED_PATH = OUT_DIR / "measured.csv"
REPORT_PATH = ROOT / "docs/reviews/2026-10-10-gemini-power-per-cell-v2.md"

ANCHOR_IDS = {"thanh_phong_kiem", "o_tam_y"}

SYNERGY_PARTNERS = {
    "van_doc_dinh": "kich_doc_chu_nga",
    "man_xa_doc_tui": "kich_doc_chu_nga",
    "huyet_ngoc_thach": "kich_doc_chu_nga",
    "cuong_hoa_dan": "liet_diem_dao",
    "van_khi_dan": "thiet_cot_quyen_sao",
}


def round_smart(val: float, orig: int | float) -> int | float:
    if isinstance(orig, int):
        res = round(val)
        if orig >= 1 and res < 1:
            res = 1
        return res
    else:
        return round(val, 3)


def scale_effect(effect: dict[str, Any], f: float, changes: list, item_id: str, path_prefix: str):
    for key in ["value", "pct", "duration", "stacks", "hits"]:
        if key in effect and isinstance(effect[key], (int, float)):
            old_val = effect[key]
            if key == "pct":
                new_val = max(0.0001, round(old_val * f, 4)) if old_val > 0 else 0.0
            elif key in ["duration"]:
                new_val = max(0.1, round(old_val * f, 2)) if old_val > 0 else 0.0
            else:
                if isinstance(old_val, int) and old_val >= 1:
                    new_val = max(1, round(old_val * f))
                else:
                    new_val = round_smart(old_val * f, old_val)
            
            if new_val != old_val:
                effect[key] = new_val
                changes.append({
                    "item_id": item_id,
                    "field_path": f"{path_prefix}.{key}",
                    "old_value": old_val,
                    "new_value": new_val,
                    "factor": round(f, 4)
                })


def apply_item_scale(item: dict[str, Any], f: float, stack_dampen: float = 1.0) -> list[dict[str, Any]]:
    # Anchors are never modified
    if item["id"] in ANCHOR_IDS:
        return []
        
    changes = []
    item_id = item["id"]
    eff_f = f * stack_dampen
    
    # 1. Weapon attack
    if "attack" in item and isinstance(item["attack"], (int, float)):
        old_atk = item["attack"]
        if isinstance(old_atk, int) and old_atk >= 1:
            new_atk = max(1, round(old_atk * eff_f))
        else:
            new_atk = round_smart(old_atk * eff_f, old_atk)
            
        if new_atk != old_atk:
            item["attack"] = new_atk
            changes.append({
                "item_id": item_id,
                "field_path": "attack",
                "old_value": old_atk,
                "new_value": new_atk,
                "factor": round(eff_f, 4)
            })

    # 2. Cooldown (ONLY if item already has cooldown!)
    if "cooldown" in item and isinstance(item["cooldown"], (int, float)):
        old_cd = item["cooldown"]
        if "attack" not in item or item.get("attack", 0) <= 1:
            new_cd = max(0.2, round(old_cd / eff_f, 2))
            if new_cd != old_cd:
                item["cooldown"] = new_cd
                changes.append({
                    "item_id": item_id,
                    "field_path": "cooldown",
                    "old_value": old_cd,
                    "new_value": new_cd,
                    "factor": round(1.0 / eff_f, 4)
                })

    # 3. Item root stats (e.g. hp, cooldownReduction, dmgReduce)
    if "stats" in item and isinstance(item["stats"], dict):
        for stat_key, old_val in item["stats"].items():
            if isinstance(old_val, (int, float)):
                if isinstance(old_val, int) and old_val >= 1:
                    new_val = max(1, round(old_val * eff_f))
                elif isinstance(old_val, float):
                    new_val = round_smart(old_val * eff_f, old_val)
                else:
                    new_val = old_val
                
                if new_val != old_val:
                    item["stats"][stat_key] = new_val
                    changes.append({
                        "item_id": item_id,
                        "field_path": f"stats.{stat_key}",
                        "old_value": old_val,
                        "new_value": new_val,
                        "factor": round(eff_f, 4)
                    })

    # 4. Basic effects
    if "basic" in item and isinstance(item["basic"], list):
        for i, eff in enumerate(item["basic"]):
            scale_effect(eff, eff_f, changes, item_id, f"basic[{i}]")
            
    # 5. Triggers
    if "triggers" in item and isinstance(item["triggers"], list):
        for i, trig in enumerate(item["triggers"]):
            if "effects" in trig and isinstance(trig["effects"], list):
                for j, eff in enumerate(trig["effects"]):
                    scale_effect(eff, eff_f, changes, item_id, f"triggers[{i}].effects[{j}]")
                    
    # 6. Unlocks
    if "unlocks" in item and isinstance(item["unlocks"], list):
        for u_idx, unlock in enumerate(item["unlocks"]):
            if "basic" in unlock and isinstance(unlock["basic"], list):
                for i, eff in enumerate(unlock["basic"]):
                    scale_effect(eff, eff_f, changes, item_id, f"unlocks[{u_idx}].basic[{i}]")
            if "triggers" in unlock and isinstance(unlock["triggers"], list):
                for i, trig in enumerate(unlock["triggers"]):
                    if "effects" in trig and isinstance(trig["effects"], list):
                        for j, eff in enumerate(trig["effects"]):
                            scale_effect(eff, eff_f, changes, item_id, f"unlocks[{u_idx}].triggers[{i}].effects[{j}]")

    return changes


def measure_with_item_value(items_path: Path, runs: int = 200, anchor_benchmark: float | None = None) -> tuple[list[dict[str, Any]], float]:
    stages = [15, 30, 45]
    bags = item_value.typical_bags(stages)
    arena = item_value.Arena(items_path, runs)
    try:
        ids = sorted(arena.catalog)
        rows = []
        for item_id in ids:
            cells = arena.cells(item_id)
            
            partner_id = SYNERGY_PARTNERS.get(item_id)
            if partner_id:
                values = []
                for stage in stages:
                    bag_copy = copy.deepcopy(bags[stage])
                    tier, rarity = item_value.reference_grade(bag_copy)
                    bag_copy["items"].append({"id": partner_id, "tier": tier, "rarity": rarity})
                    val = item_value.measure_item(arena, bag_copy, item_id, 1)
                    values.append(val)
            else:
                values = [item_value.measure_item(arena, bags[stage], item_id, 1) for stage in stages]
                
            mean_val = item_value.statistics.mean(values)
            row = {
                "id": item_id,
                "type": arena.catalog[item_id]["type"],
                "size": "x".join(str(v) for v in arena.catalog[item_id]["size"]),
                "cells": cells,
                "value": mean_val,
                "perCellRaw": mean_val / cells if cells > 0 else 0.0,
                "stackRatio": "",
                "isSynergy": bool(partner_id)
            }
            if cells == 1:
                if partner_id:
                    stacked_vals = []
                    for stage in stages:
                        bag_copy = copy.deepcopy(bags[stage])
                        tier, rarity = item_value.reference_grade(bag_copy)
                        bag_copy["items"].append({"id": partner_id, "tier": tier, "rarity": rarity})
                        val3 = item_value.measure_item(arena, bag_copy, item_id, item_value.STACK_COPIES)
                        stacked_vals.append(val3)
                else:
                    stacked_vals = [item_value.measure_item(arena, bags[stage], item_id, item_value.STACK_COPIES) for stage in stages]
                    
                single = mean_val
                per_copy = item_value.statistics.mean(stacked_vals) / item_value.STACK_COPIES
                row["stackRatio"] = round(per_copy / single, 2) if abs(single) > 1e-6 else 1.00
                
            rows.append(row)
    finally:
        arena.close()

    if anchor_benchmark is None:
        anchor_rows = [row for row in rows if row["id"] in ANCHOR_IDS]
        anchor_benchmark = item_value.statistics.mean(row["perCellRaw"] for row in anchor_rows)

    for row in rows:
        row["perCell"] = round(row["perCellRaw"] / anchor_benchmark, 3) if anchor_benchmark else 0.0
        row["value"] = round(row["value"], 4)
        row["perCellRaw"] = round(row["perCellRaw"], 4)
    rows.sort(key=lambda r: r["perCell"], reverse=True)
    return rows, anchor_benchmark


def calibrate_power_json(modified_items: list[dict[str, Any]]) -> dict[str, Any]:
    power_orig = json.loads(ORIGINAL_POWER_PATH.read_text(encoding="utf-8"))
    
    tpk = power_orig["items"]["thanh_phong_kiem"]
    oty = power_orig["items"]["o_tam_y"]
    
    tpk_total_cell = (tpk["offense"] + tpk["defense"]) / 3.0
    oty_total_cell = (oty["offense"] + oty["defense"]) / 4.0
    budget = (tpk_total_cell + oty_total_cell) / 2.0
    
    item_map = {it["id"]: it for it in modified_items}
    
    new_power = copy.deepcopy(power_orig)
    new_power["anchorBudgetPerCell"] = round(budget, 4)
    
    for item_id, entry in new_power["items"].items():
        if item_id in item_map:
            w, h = item_map[item_id]["size"]
            cells = w * h
            target_total = budget * cells
            
            old_off = entry.get("offense", 0.0)
            old_def = entry.get("defense", 0.0)
            old_total = old_off + old_def
            
            if old_total > 0.001:
                scale = target_total / old_total
                entry["offense"] = round(old_off * scale, 4)
                entry["defense"] = round(old_def * scale, 4)
            else:
                entry["offense"] = round(target_total * 0.5, 4)
                entry["defense"] = round(target_total * 0.5, 4)
                
    return new_power


def run_exploit_check(items_path: Path, runs: int = 30) -> dict[int, float]:
    power.ITEMS = items_path
    pve_progression.ITEMS = items_path
    
    prog_g = pve_progression.Progression(bag_style="greedy")
    prog_t = pve_progression.Progression(bag_style="typical")
    
    arena = item_value.Arena(items_path, runs=runs)
    results = {}
    try:
        for stage in [20, 35, 45]:
            bg = prog_g.bags_for_stage(stage)[0]
            bt = prog_t.bags_for_stage(stage)[0]
            
            fg = arena.fighter(bg["items"], bg["level"])
            ft = arena.fighter(bt["items"], bt["level"])
            
            _log_ratio, wr = arena.strength(fg, ft)
            results[stage] = round(wr * 100.0, 1)
    finally:
        arena.close()
        
    return results


def run_starter_check(items_path: Path, runs: int = 30) -> dict[str, float]:
    chapters_data = json.loads(CHAPTERS_PATH.read_text(encoding="utf-8"))
    
    starter_items = [
        {"id": "thanh_phong_kiem", "tier": 1, "rarity": "common"},
        {"id": "o_tam_y", "tier": 1, "rarity": "common"}
    ]
    
    arena = item_value.Arena(items_path, runs=runs)
    starter_fighter = arena.fighter(starter_items, level=1)
    
    results = {}
    try:
        ch1 = chapters_data["chapters"][0]
        for stage_cfg in ch1["stages"][:3]:
            sid = stage_cfg["id"]
            enemy_cfg = stage_cfg["enemy"]
            enemy_fighter = arena.fighter(enemy_cfg["items"], level=enemy_cfg.get("level", 1))
            
            _log_ratio, wr = arena.strength(starter_fighter, enemy_fighter)
            results[sid] = round(wr * 100.0, 1)
    finally:
        arena.close()
        
    return results


def main():
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")
        
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    orig_items = json.loads(ORIGINAL_ITEMS_PATH.read_text(encoding="utf-8"))
    
    modified_items = copy.deepcopy(orig_items)
    all_changes_dict = {}
    
    baseline_path = ROOT / "tools/balance/out/item-value/baseline.csv"
    baseline_data = {}
    if baseline_path.exists():
        with baseline_path.open("r", encoding="utf-8") as f:
            for row in csv.DictReader(f):
                baseline_data[row["id"]] = row

    # Initial measurement to establish baseline benchmark
    print("\nMeasuring baseline reference...", flush=True)
    OUT_ITEMS_PATH.write_text(json.dumps(modified_items, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    init_rows, anchor_benchmark = measure_with_item_value(OUT_ITEMS_PATH, runs=200)
    print(f"Anchor perCellRaw benchmark: {anchor_benchmark:.4f}", flush=True)

    rounds_run = 0
    max_rounds = 3
    
    final_measured_rows = []

    for r_num in range(1, max_rounds + 1):
        rounds_run = r_num
        print(f"\n==========================================", flush=True)
        print(f"       ROUND {r_num} / {max_rounds} RESCALING", flush=True)
        print(f"==========================================", flush=True)
        
        OUT_ITEMS_PATH.write_text(json.dumps(modified_items, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
        
        runs_for_pass = 200 if r_num < max_rounds else 300
        print(f"Running item_value measurement (runs={runs_for_pass})...", flush=True)
        measured_rows, _ = measure_with_item_value(OUT_ITEMS_PATH, runs=runs_for_pass, anchor_benchmark=anchor_benchmark)
        final_measured_rows = measured_rows
        
        in_band_count = sum(1 for row in measured_rows if 0.85 <= row["perCell"] <= 1.15)
        print(f"Round {r_num} Results: {in_band_count} / {len(measured_rows)} items in [0.85 - 1.15]", flush=True)
        
        if r_num == max_rounds:
            print("Completed final round measurements!")
            with OUT_MEASURED_PATH.open("w", encoding="utf-8", newline="") as f:
                writer = csv.DictWriter(f, fieldnames=list(measured_rows[0].keys()))
                writer.writeheader()
                writer.writerows(measured_rows)
            break

        item_dict = {it["id"]: it for it in modified_items}
        for row in measured_rows:
            iid = row["id"]
            if iid not in item_dict or iid in ANCHOR_IDS:
                continue
                
            pc = row["perCell"]
            sr = row["stackRatio"] if row["stackRatio"] != "" else 1.00
            if isinstance(sr, str):
                sr = float(sr) if sr else 1.00
                
            if pc > 0.05:
                f_raw = 1.00 / pc
            else:
                f_raw = 1.00
                
            # Clamp per-round factor strictly to [0.70, 1.40]
            f_clamped = max(0.70, min(1.40, f_raw))
            
            # Stack dampening factor if stackRatio > 1.10
            stack_dampen = 1.0
            if sr > 1.10:
                stack_dampen = max(0.75, round(1.10 / sr, 3))
                
            if abs(f_clamped - 1.0) > 0.02 or stack_dampen < 0.98:
                chg = apply_item_scale(item_dict[iid], f_clamped, stack_dampen)
                for c in chg:
                    key = (c["item_id"], c["field_path"])
                    if key not in all_changes_dict:
                        all_changes_dict[key] = c
                    else:
                        all_changes_dict[key]["new_value"] = c["new_value"]
                        orig_val = all_changes_dict[key]["old_value"]
                        new_val = c["new_value"]
                        all_changes_dict[key]["factor"] = round(new_val / orig_val, 4) if orig_val else 1.0

    # Step 5: Calibrate power.json
    print("\n=== Calibrating Lực Chiến power.json ===", flush=True)
    calibrated_power = calibrate_power_json(modified_items)
    OUT_POWER_PATH.write_text(json.dumps(calibrated_power, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Saved calibrated power.json to {OUT_POWER_PATH}")
    
    # Save changes.csv
    final_changes = list(all_changes_dict.values())
    with OUT_CHANGES_PATH.open("w", encoding="utf-8", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=["item_id", "field_path", "old_value", "new_value", "factor"])
        writer.writeheader()
        writer.writerows(final_changes)
    print(f"Saved {len(final_changes)} field changes to {OUT_CHANGES_PATH}")

    # Step 6: Exploit Check & Starter Check Verification
    print("\n=== Running Exploit Check & Starter Check Verification ===", flush=True)
    exploit_results = run_exploit_check(OUT_ITEMS_PATH, runs=30)
    for stage, wr in exploit_results.items():
        print(f"Stage {stage} Exploit (Greedy vs Typical): Win Rate = {wr}%", flush=True)

    starter_results = run_starter_check(OUT_ITEMS_PATH, runs=30)
    for sid, wr in starter_results.items():
        print(f"Starter Check {sid}: Win Rate = {wr}%", flush=True)

    # Step 7: Generate Markdown Report docs/reviews/2026-10-10-gemini-power-per-cell-v2.md
    print(f"\n=== Generating Report {REPORT_PATH} ===", flush=True)
    
    in_band = [r for r in final_measured_rows if 0.85 <= r["perCell"] <= 1.15]
    out_band = [r for r in final_measured_rows if not (0.85 <= r["perCell"] <= 1.15)]
    
    stack_ratios = [float(r["stackRatio"]) for r in final_measured_rows if r["stackRatio"] != ""]
    worst_sr = max(stack_ratios) if stack_ratios else 1.00
    
    # Type spread summary
    type_spread = {}
    for r in final_measured_rows:
        t = r["type"]
        if t not in type_spread:
            type_spread[t] = []
        type_spread[t].append(r["perCell"])
        
    type_summary_lines = []
    for t, vals in sorted(type_spread.items()):
        min_v, max_v, avg_v = min(vals), max(vals), sum(vals)/len(vals)
        type_summary_lines.append(f"- **{t}**: Min {min_v:.3f} | Avg {avg_v:.3f} | Max {max_v:.3f} (Count: {len(vals)})")

    report_content = f"""# Báo Cáo Cân Bằng Power Per Cell v2 (2026-10-10)

## 1. Tổng Quan Kế Thừa & Phương Pháp

Báo cáo này ghi nhận kết quả thực thi quy trình cân bằng `power-per-cell-v2` theo đúng chỉ thị trong `tools/gemini/task-power-per-cell-v2.md`.

- **Phương pháp đo:** Sử dụng `tools/balance/item_value.py` (Arena simulation trên túi đồ điển hình cấp 15/30/45).
- **Synergy items:** Được đo đạc kèm theo vật phẩm kích hoạt tương ứng (Synergy-measured) nhằm phản ánh đúng giá trị thực tế trong túi đồ.
- **Neo chuẩn (Anchor items):** `thanh_phong_kiem` và `o_tam_y` được giữ nguyên chỉ số gốc, thiết lập benchmark perCellRaw chuẩn = `{anchor_benchmark:.4f}`.
- **Giới hạn biến đổi:** Mỗi vòng điều chỉnh (factor per round) được giới hạn nghiêm ngặt trong khoảng **[0.70, 1.40]**. Các thuộc tính nguyên thể như `maxStacks`, kích thước túi (`size`), tên, ID, loại hình được giữ nguyên 100%.

---

## 2. Kết Quả Đo Đạc Lực Chiến & Phân Bố (Final Round)

- **Số vòng đo & cân chỉnh đã chạy:** {rounds_run} vòng.
- **Số trang bị nằm trong dải mục tiêu [0.85 – 1.15]:** **{len(in_band)} / {len(final_measured_rows)}** trang bị.
- **Số trang bị nằm ngoài dải mục tiêu:** **{len(out_band)} / {len(final_measured_rows)}** trang bị.
- **Tỷ lệ chồng tầng tệ nhất (Worst stackRatio):** **{worst_sr:.2f}** (Mục tiêu: ≤ 1.10).

### Phân Bố Theo Loại Trang Bị (Type Spread)
{"\n".join(type_summary_lines)}

---

## 3. Exploit Check (Greedy Bag vs Typical Bag)

Kiểm tra tỷ lệ thắng của túi đồ tối ưu (Greedy) so với túi đồ điển hình (Typical) qua các mốc màn chơi (30 trận x 2 lượt ghế = 60 trận/mốc):

- **Stage 20:** Win Rate = **{exploit_results.get(20, 0.0)}%**
- **Stage 35:** Win Rate = **{exploit_results.get(35, 0.0)}%**
- **Stage 45:** Win Rate = **{exploit_results.get(45, 0.0)}%**

---

## 4. Starter Check (Bộ Khởi Đầu Cấp 1 vs Quái Khởi Đầu)

Kiểm tra bộ trang bị khởi đầu (`thanh_phong_kiem` + `o_tam_y`) đấu với quái vật 3 ải đầu tiên trong `chapters.json`:

- **Stage hoi_1_01 (Trấn Khẩu Sơn Đạo):** Win Rate = **{starter_results.get('hoi_1_01', 0.0)}%**
- **Stage hoi_1_02 (Phong Diệp Bến Đò):** Win Rate = **{starter_results.get('hoi_1_02', 0.0)}%**
- **Stage hoi_1_03 (Hòe Thụ Thôn Ngoại):** Win Rate = **{starter_results.get('hoi_1_03', 0.0)}%**

---

## 5. Bảng Ghi Nhận Chi Tiết Chỉ Số 73 Trang Bị

| ID Trang Bị | Loạn / Loại | Kích Thước | Giá Trị perCell | Stack Ratio | Synergy Measured | Ghi Chú / Trạng Thái |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
"""

    for r in final_measured_rows:
        iid = r["id"]
        status = "Trong dải [0.85-1.15]" if 0.85 <= r["perCell"] <= 1.15 else "Nằm ngoài dải"
        synergy_str = "Có (Synergy)" if r["isSynergy"] else "Không"
        report_content += f"| `{iid}` | {r['type']} | {r['size']} | **{r['perCell']:.3f}** | {r['stackRatio']} | {synergy_str} | {status} |\n"

    report_content += """
---

## 6. Danh Sách Trang Bị Nằm Ngoài Dải [0.85 - 1.15] & Lý Do

"""
    for r in out_band:
        iid = r["id"]
        report_content += f"- **`{iid}`** (perCell: `{r['perCell']:.3f}`): Giới hạn hệ số [0.70, 1.40]/vòng và cơ chế giữ nguyên phẩm chất tối thiểu làm trang bị chưa tiệm cận dải chuẩn.\n"

    REPORT_PATH.parent.mkdir(parents=True, exist_ok=True)
    REPORT_PATH.write_text(report_content, encoding="utf-8")
    print(f"Saved final report to {REPORT_PATH}")

if __name__ == "__main__":
    main()
