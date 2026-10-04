import json
import subprocess
import sys
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
CATALOG = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"

sys.stdout.reconfigure(encoding="utf-8")

# Let's adjust mb2, mb3, and thai_thanh_kiem_khi
with open(CATALOG, "r", encoding="utf-8") as f:
    cat = {it["id"]: it for it in json.load(f)}

def validate_bag(name: str, items: list[dict], cols=5, rows=4):
    grid = [[None for _ in range(cols)] for _ in range(rows)]
    for it in items:
        iid = it["id"]
        w, h = cat[iid]["size"]
        x, y = it["x"], it["y"]
        if x < 0 or y < 0 or x + w > cols or y + h > rows:
            raise ValueError(f"[{name}] Out of bounds: {iid} at ({x},{y}) size ({w},{h}) in {cols}x{rows}")
        for r in range(y, y + h):
            for c in range(x, x + w):
                if grid[r][c] is not None:
                    raise ValueError(f"[{name}] Overlap at ({c},{r}) between {grid[r][c]} and {iid}")
                grid[r][c] = iid

# Improved mb2 (Hàn Sơn Phái - Băng Quy Tuyệt Hộ)
# Give it faster attack rate and healing/defense
mb2_tuned = {
    "name": "Hàn Sơn Phái - Băng Quy Tuyệt Hộ",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1,
    "items": [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "thien_nien_quy_giap", "x": 1, "y": 0, "rarity": "legendary"},
        {"id": "bang_phach_co_cot", "x": 1, "y": 2, "rarity": "legendary"},
        {"id": "tuyet_lang_bach_bi", "x": 3, "y": 0, "rarity": "rare"},
        {"id": "han_ngoc_tieu", "x": 3, "y": 1, "rarity": "epic"},
        {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
        {"id": "bang_tam_linh_chau", "x": 4, "y": 3, "rarity": "rare"},
        {"id": "han_bang_chau", "x": 4, "y": 1, "rarity": "rare"},
        {"id": "kim_sang_duoc", "x": 4, "y": 2, "rarity": "uncommon"}
    ]
}

# Improved mb3 (Liệt Diễm Đường - Xích Xà Hỏa Liệp)
# Pure burn + bleed synergy
mb3_tuned = {
    "name": "Liệt Diễm Đường - Xích Xà Hỏa Liệp",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1,
    "items": [
        {"id": "liet_diem_dao", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "liet_hoa_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
        {"id": "soi_nanh_canh", "x": 1, "y": 0, "rarity": "rare"},
        {"id": "hoa_diem_xa_cot", "x": 1, "y": 2, "rarity": "epic"},
        {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "rarity": "rare"},
        {"id": "xich_sa_chuong", "x": 2, "y": 2, "rarity": "epic"},
        {"id": "chu_sa_ho_lo", "x": 3, "y": 2, "rarity": "epic"},
        {"id": "cuong_hoa_dan", "x": 3, "y": 3, "rarity": "uncommon"},
        {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 2, "rarity": "uncommon"}
    ]
}

# Improved thai_thanh_kiem_khi
# Replace 1 sword with protective relic to stabilize early game
ttkk_tuned = {
    "name": "Thái Thanh Quán - Kiếm Khí Xuyên Giáp",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1,
    "items": [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "thai_thanh_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
        {"id": "thanh_phong_kiem", "x": 1, "y": 0, "rarity": "epic"},
        {"id": "kiem_tong_tam_yeu", "x": 1, "y": 3, "rarity": "epic"},
        {"id": "o_tam_y", "x": 2, "y": 0, "rarity": "rare"},
        {"id": "kim_tuyen_y", "x": 2, "y": 2, "rarity": "rare"},
        {"id": "bach_ngoc_boi", "x": 4, "y": 0, "rarity": "epic"},
        {"id": "tuy_tien_ho_lo", "x": 4, "y": 1, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 2, "rarity": "uncommon"}
    ]
}

validate_bag("mb2", mb2_tuned["items"])
validate_bag("mb3", mb3_tuned["items"])
validate_bag("ttkk", ttkk_tuned["items"])

with open(LOADOUTS / "mb2.json", "w", encoding="utf-8") as f:
    json.dump(mb2_tuned, f, indent=2)
with open(LOADOUTS / "mb3.json", "w", encoding="utf-8") as f:
    json.dump(mb3_tuned, f, indent=2)
with open(LOADOUTS / "thai_thanh_kiem_khi.json", "w", encoding="utf-8") as f:
    json.dump(ttkk_tuned, f, indent=2)

print("Updated mb2, mb3, thai_thanh_kiem_khi loadouts.")

patch_trial_2 = {
    # Huyền Độc tuning (calm down T6/T9 without breaking T1)
    "that_tinh_tieu.cooldown": 2.5,
    "that_tinh_tieu.basic.poison": 2,
    "that_tinh_tieu.attack": 6,
    "tu_kim_trao.cooldown": 1.8,
    "tu_kim_trao.attack": 9,
    "van_doc_dinh.triggers.drain_qi": 4,
    "u_minh_bi_luc.triggers.heal": 35,
    # Bích Thủy whip tuning
    "thanh_dang_tien.cooldown": 4.1,
    "thanh_dang_tien.attack": 32,
    # Thái Thanh fan tuning
    "ngoc_cot_phien.cooldown": 2.2,
    "ngoc_cot_phien.attack": 22,
    # Thái Thanh sword tuning
    "thanh_phong_kiem.cooldown": 4.1,
    "kiem_tong_tam_yeu.triggers.charge": 0.25,
    # Liệt Diễm palm
    "xich_sa_chuong.attack": 34,
    "xich_sa_chuong.cooldown": 1.9,
    # Monster items tuning
    "soi_nanh_canh.attack": 26,
    "soi_nanh_canh.cooldown": 1.8,
    "kich_doc_chu_nga.attack": 28,
    "kich_doc_chu_nga.cooldown": 1.6,
    "bach_ho_cuong_mong.attack": 34,
    "bach_ho_cuong_mong.cooldown": 1.7,
    "bang_phach_thuong.attack": 140,
    "bang_phach_thuong.cooldown": 4.2,
    "han_ngoc_tieu.attack": 46,
    "han_ngoc_tieu.cooldown": 3.8,
    "thien_nien_quy_giap.triggers.shield": 250,
    "heo_rung_bap_giap.triggers.shield": 200,
    "hoa_diem_xa_cot.triggers.burn": 2
}

def run_matrix(patch_dict: dict, label: str):
    patch_path = ROOT / "tools" / "gemini" / "out" / "round-2" / "temp_patch.json"
    with open(patch_path, "w", encoding="utf-8") as f:
        json.dump(patch_dict, f, indent=2)

    cmd = [
        "py", str(ROOT / "tools" / "balance" / "balance.py"), "whatif",
        "--patch", str(patch_path),
        "--loadouts", str(LOADOUTS),
        "--items", str(CATALOG),
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", "60"
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
    output = res.stdout
    
    outside_match = re.search(r"Outside \[35, 65\]:\s*(\d+)/(\d+)", output)
    outside_str = outside_match.group(0) if outside_match else "N/A"
    
    rows = []
    for line in output.splitlines():
        nums = [float(x) for x in re.findall(r"([0-9]+\.[0-9]+)%", line)]
        if len(nums) == 8:
            name = line[:38].strip()
            avg = sum(nums) / len(nums)
            rows.append((name, avg, nums))
            
    print(f"=== {label} ===")
    print(f"Result: {outside_str}")
    bad_rows = [r for r in rows if r[1] < 30.0 or r[1] > 70.0]
    if bad_rows:
        print(f"Bad rows (<30% or >70%): {len(bad_rows)}")
        for r in bad_rows:
            print(f"  {r[0]:<35}: avg={r[1]:.1f}%")
    else:
        print("ALL 24 BUILDS SIT BETWEEN 30% AND 70%!")
    return outside_str, rows

run_matrix(patch_trial_2, "Trial 2")
