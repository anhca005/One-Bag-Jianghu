import json
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "levels.json", "r", encoding="utf-8") as f:
    levels_data = json.load(f)

exp_to_next = levels_data["expToNext"]
cumul_needed = [0] # index = level (1-indexed)
total = 0
for exp in exp_to_next:
    total += exp
    cumul_needed.append(total)

print("EXP needed to reach each level:")
for lvl in range(1, 12):
    print(f"Level {lvl:2d}: {cumul_needed[lvl-1]:4d} total EXP (exp to next: {exp_to_next[lvl-1] if lvl < len(cumul_needed) else 0})")

# Stage breakdown:
# Stage 1: First clear 60, replays 0
# Stage 2: First clear 60, replays 0
# Stage 3: First clear 60, replays 0
# Stage 4: First clear 60, replays 1 (20)
# Stage 5 (Elite): First clear 60, replays 1 (20)
# Stage 6: First clear 60, replays 1 (20)
# Stage 7: First clear 60, replays 1 (20)
# Stage 8: First clear 60, replays 0
# Stage 9: First clear 60, replays 1 (20)
# Stage 10 (Boss): First clear 60, replays 0
print("\nCumulative EXP progression by stage:")
stages = [
    ("Ải 01: Trấn Khẩu Sơn Đạo", 60, 0),
    ("Ải 02: Bến Đò Phong Diệp", 60, 0),
    ("Ải 03: Hòe Thụ Thôn Ngoại", 60, 0),
    ("Ải 04: Thanh Khê Trà Thất", 60, 1),
    ("Ải 05: Phong Ba Đình (Tinh Anh)", 60, 1),
    ("Ải 06: Ngô Đồng Cổ Hạng", 60, 1),
    ("Ải 07: Thủy Tạ Nguy Kiều", 60, 1),
    ("Ải 08: Phong Hỏa Cổ Đài", 60, 0),
    ("Ải 09: Ba Hổ Tiền Doanh", 60, 1),
    ("Ải 10: Bến Bạc Phân Tranh (Boss)", 60, 0),
]

current_exp = 0
current_lvl = 1

print(f"{'Stage':<32} {'FirstClear':<12} {'Replays':<10} {'Total EXP':<12} {'Level Reached':<14}")
print("-" * 75)
for name, fc, rep in stages:
    gain = fc + rep * 20
    current_exp += gain
    while current_lvl < 10 and current_exp >= cumul_needed[current_lvl]:
        current_lvl += 1
    safe_name = name.encode("ascii", "replace").decode("ascii")
    print(f"{safe_name:<32} {fc:<12} {rep:<10} {current_exp:<12} Level {current_lvl:<10}")
