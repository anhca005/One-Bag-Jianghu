import json
import subprocess
import sys
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
CATALOG = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"

sys.stdout.reconfigure(encoding="utf-8")

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

# 1. mb1: Huyền Độc Môn - Huyết Thú Cuồng Trảo
mb1 = {
    "name": "Huyền Độc Môn - Huyết Thú Cuồng Trảo",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "huyen_doc", "tamPhapLevel": 1,
    "items": [
        {"id": "u_minh_quy_trao", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "heo_rung_bap_giap", "x": 0, "y": 2, "rarity": "rare"},
        {"id": "kich_doc_chu_nga", "x": 2, "y": 0, "rarity": "rare"},
        {"id": "u_minh_bi_luc", "x": 2, "y": 2, "rarity": "epic"},
        {"id": "that_tinh_tieu", "x": 3, "y": 0, "rarity": "epic"},
        {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
        {"id": "man_xa_doc_tui", "x": 4, "y": 0, "rarity": "rare"},
        {"id": "hac_sat_hung_dam", "x": 4, "y": 1, "rarity": "epic"},
        {"id": "giai_doc_dan", "x": 4, "y": 2, "rarity": "uncommon"},
        {"id": "van_doc_dinh", "x": 2, "y": 3, "rarity": "epic"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
    ]
}

# 2. mb2: Hàn Sơn Phái - Băng Quy Tuyệt Hộ
mb2 = {
    "name": "Hàn Sơn Phái - Băng Quy Tuyệt Hộ",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1,
    "items": [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "thien_nien_quy_giap", "x": 1, "y": 0, "rarity": "legendary"},
        {"id": "bang_phach_co_cot", "x": 1, "y": 2, "rarity": "legendary"},
        {"id": "tuyet_lang_bach_bi", "x": 3, "y": 0, "rarity": "rare"},
        {"id": "han_ngoc_tieu", "x": 3, "y": 1, "rarity": "epic"},
        {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
        {"id": "han_bang_chau", "x": 4, "y": 1, "rarity": "rare"},
        {"id": "kim_sang_duoc", "x": 4, "y": 2, "rarity": "uncommon"},
        {"id": "bang_tam_linh_chau", "x": 4, "y": 3, "rarity": "rare"}
    ]
}

# 3. mb3: Liệt Diễm Đường - Xích Xà Hỏa Liệp
mb3 = {
    "name": "Liệt Diễm Đường - Xích Xà Hỏa Liệp",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1,
    "items": [
        {"id": "liet_diem_dao", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "liet_hoa_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
        {"id": "hoa_diem_xa_cot", "x": 1, "y": 0, "rarity": "epic"},
        {"id": "soi_nanh_canh", "x": 1, "y": 2, "rarity": "rare"},
        {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "rarity": "rare"},
        {"id": "tu_mau_song_dao", "x": 2, "y": 2, "rarity": "rare"},
        {"id": "chu_sa_ho_lo", "x": 4, "y": 0, "rarity": "epic"},
        {"id": "cuong_hoa_dan", "x": 4, "y": 1, "rarity": "uncommon"},
        {"id": "da_ho_y", "x": 4, "y": 2, "rarity": "rare"}
    ]
}

# 4. mb4: Kim Cang Tự - Bạch Hổ Bá Quyền
mb4 = {
    "name": "Kim Cang Tự - Bạch Hổ Bá Quyền",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1,
    "items": [
        {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "thiet_cot_quyen_sao", "x": 0, "y": 2, "rarity": "epic"},
        {"id": "bach_ho_cuong_mong", "x": 1, "y": 0, "rarity": "epic"},
        {"id": "tuyet_lang_bach_bi", "x": 1, "y": 1, "rarity": "rare"},
        {"id": "o_thiet_giap", "x": 1, "y": 2, "rarity": "rare"},
        {"id": "phuc_ho_con", "x": 3, "y": 0, "rarity": "epic"},
        {"id": "hac_sat_hung_dam", "x": 4, "y": 0, "rarity": "epic"},
        {"id": "bach_luyen_chan_kinh", "x": 4, "y": 1, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
    ]
}

# 5. thai_thanh_kiem_khi
ttkk = {
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
        {"id": "boi_nguyen_dan", "x": 4, "y": 2, "rarity": "uncommon"},
        {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 3, "rarity": "common"}
    ]
}

for name, lb in [("mb1", mb1), ("mb2", mb2), ("mb3", mb3), ("mb4", mb4), ("thai_thanh_kiem_khi", ttkk)]:
    validate_bag(name, lb["items"])
    with open(LOADOUTS / f"{name}.json", "w", encoding="utf-8") as f:
        json.dump(lb, f, indent=2)

print("Validated and wrote updated loadouts.")

patch_trial_3 = {
    # Huyền Độc tuning (stabilize around 55-65%)
    "that_tinh_tieu.cooldown": 2.4,
    "that_tinh_tieu.basic.poison": 2,
    "that_tinh_tieu.attack": 6,
    "tu_kim_trao.cooldown": 1.7,
    "tu_kim_trao.attack": 10,
    "van_doc_dinh.triggers.drain_qi": 4,
    "u_minh_bi_luc.triggers.heal": 35,
    # Bích Thủy whip tuning
    "thanh_dang_tien.cooldown": 3.9,
    "thanh_dang_tien.attack": 34,
    # Thái Thanh fan tuning
    "ngoc_cot_phien.cooldown": 2.1,
    "ngoc_cot_phien.attack": 23,
    # Thái Thanh sword tuning
    "thanh_phong_kiem.cooldown": 3.8,
    "thanh_phong_kiem.attack": 38,
    "kiem_tong_tam_yeu.triggers.charge": 0.35,
    # Liệt Diễm palm
    "xich_sa_chuong.attack": 32,
    "xich_sa_chuong.cooldown": 2.0,
    # Hàn Sơn spear
    "bang_phach_thuong.attack": 120,
    "bang_phach_thuong.cooldown": 4.6,
    "han_ngoc_tieu.attack": 44,
    "han_ngoc_tieu.cooldown": 4.0,
    # Monster items tuning
    "soi_nanh_canh.attack": 24,
    "soi_nanh_canh.cooldown": 1.8,
    "kich_doc_chu_nga.attack": 26,
    "kich_doc_chu_nga.cooldown": 1.7,
    "bach_ho_cuong_mong.attack": 30,
    "bach_ho_cuong_mong.cooldown": 1.8,
    "thien_nien_quy_giap.triggers.shield": 220,
    "heo_rung_bap_giap.triggers.shield": 180
}

patch_path = ROOT / "tools" / "gemini" / "out" / "round-2" / "temp_patch.json"
with open(patch_path, "w", encoding="utf-8") as f:
    json.dump(patch_trial_3, f, indent=2)

cmd = [
    "py", str(ROOT / "tools" / "balance" / "balance.py"), "whatif",
    "--patch", str(patch_path),
    "--loadouts", str(LOADOUTS),
    "--items", str(CATALOG),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "60"
]
print("Running whatif Matrix A...")
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
        
print(f"=== Trial 3 Matrix A ===")
print(f"Result: {outside_str}")
bad_rows = [r for r in rows if r[1] < 30.0 or r[1] > 70.0]
if bad_rows:
    print(f"Bad rows (<30% or >70%): {len(bad_rows)}")
    for r in bad_rows:
        print(f"  {r[0]:<35}: avg={r[1]:.1f}%")
else:
    print("ALL 24 BUILDS SIT BETWEEN 30% AND 70%!")

for r in rows:
    outside_cnt = sum(1 for x in r[2] if x < 35.0 or x > 65.0)
    flag = "(!)" if r[1] < 30 or r[1] > 70 else ""
    print(f"{r[0]:<38}: avg={r[1]:5.1f}% (outside {outside_cnt}/8) {flag}")
