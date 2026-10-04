import json, subprocess, copy
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"
CATALOG = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"

with open(CATALOG, "r", encoding="utf-8") as f:
    cat = {it["id"]: it for it in json.load(f)}

def validate_grid(lb):
    cols = lb.get("cols", 5)
    rows = lb.get("rows", 4)
    grid = {}
    for it in lb["items"]:
        iid = it["id"]
        if iid not in cat:
            return False, f"Unknown {iid}"
        w, h = cat[iid]["size"]
        x, y = it["x"], it["y"]
        for dx in range(w):
            for dy in range(h):
                pos = (x + dx, y + dy)
                if pos[0] >= cols or pos[1] >= rows or pos[0] < 0 or pos[1] < 0:
                    return False, f"OOB {iid} at {pos}"
                if pos in grid:
                    return False, f"Overlap at {pos}: {grid[pos]} & {iid}"
                grid[pos] = iid
    return True, f"OK ({len(grid)}/20)"

# Let's define the 4 monster loadouts
mb1 = {
  "name": "Huyền Độc Môn - Huyết Thú Cuồng Trảo",
  "hp": 1200, "cols": 5, "rows": 4, "sect": "huyen_doc", "tamPhapLevel": 1,
  "items": [
    {"id": "u_minh_quy_trao", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "heo_rung_bap_giap", "x": 0, "y": 2, "rarity": "rare"},
    {"id": "kich_doc_chu_nga", "x": 2, "y": 0, "rarity": "rare"},
    {"id": "that_tinh_tieu", "x": 2, "y": 2, "rarity": "epic"},
    {"id": "van_doc_dinh", "x": 3, "y": 0, "rarity": "epic"},
    {"id": "u_minh_bi_luc", "x": 3, "y": 1, "rarity": "epic"},
    {"id": "man_xa_doc_tui", "x": 3, "y": 2, "rarity": "rare"},
    {"id": "hac_sat_hung_dam", "x": 3, "y": 3, "rarity": "epic"},
    {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
    {"id": "giai_doc_dan", "x": 4, "y": 2, "rarity": "uncommon"},
    {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

mb2 = {
  "name": "Hàn Sơn Phái - Băng Quy Tuyệt Hộ",
  "hp": 1200, "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1,
  "items": [
    {"id": "truy_hon_cung", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thanh_tam_dan", "x": 0, "y": 3, "rarity": "uncommon"},
    {"id": "phan_quan_but", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "han_ngoc_tieu", "x": 1, "y": 2, "rarity": "epic"},
    {"id": "thien_nien_quy_giap", "x": 2, "y": 0, "rarity": "legendary"},
    {"id": "tuyet_lang_bach_bi", "x": 2, "y": 2, "rarity": "rare"},
    {"id": "bang_tam_linh_chau", "x": 2, "y": 3, "rarity": "rare"},
    {"id": "han_bang_chau", "x": 3, "y": 3, "rarity": "rare"},
    {"id": "da_ho_y", "x": 4, "y": 0, "rarity": "rare"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 2, "rarity": "epic"}
  ]
}

mb3 = {
  "name": "Liệt Diễm Đường - Xích Xà Hỏa Liệp",
  "hp": 1200, "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1,
  "items": [
    {"id": "liet_diem_dao", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "liet_hoa_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
    {"id": "hoa_diem_xa_cot", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "soi_nanh_canh", "x": 1, "y": 2, "rarity": "rare"},
    {"id": "liet_diem_dao", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "chu_sa_ho_lo", "x": 2, "y": 3, "rarity": "epic"},
    {"id": "da_ho_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "cuong_hoa_dan", "x": 3, "y": 2, "rarity": "uncommon"},
    {"id": "boi_nguyen_dan", "x": 3, "y": 3, "rarity": "uncommon"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 0, "rarity": "epic"},
    {"id": "hac_sat_hung_dam", "x": 4, "y": 2, "rarity": "epic"},
    {"id": "giai_doc_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

mb4 = {
  "name": "Kim Cang Tự - Bạch Hổ Bá Quyền",
  "hp": 1200, "cols": 5, "rows": 4, "sect": "kim_cang", "tamPhapLevel": 1,
  "items": [
    {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thiet_cot_quyen_sao", "x": 0, "y": 2, "rarity": "epic"},
    {"id": "phuc_ho_con", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "bach_ho_cuong_mong", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "bang_phach_co_cot", "x": 2, "y": 1, "rarity": "legendary"},
    {"id": "tuyet_lang_bach_bi", "x": 2, "y": 3, "rarity": "rare"},
    {"id": "bach_luyen_chan_kinh", "x": 4, "y": 0, "rarity": "epic"},
    {"id": "thanh_dong_ho_tam_kinh", "x": 4, "y": 2, "rarity": "epic"},
    {"id": "hac_sat_hung_dam", "x": 4, "y": 3, "rarity": "epic"}
  ]
}

for name, lb in [("mb1", mb1), ("mb2", mb2), ("mb3", mb3), ("mb4", mb4)]:
    ok, msg = validate_grid(lb)
    assert ok, f"{name} invalid: {msg}"
    with open(LOADOUTS / f"{name}.json", "w", encoding="utf-8") as f:
        json.dump(lb, f, indent=2, ensure_ascii=False)
print("Updated all 4 monster loadouts.")

# Let's also adjust the patch for weapons/skills to bring matrix into tight balance
# Let's inspect thai_thanh sword build to calm it down slightly:
# thanh_phong_kiem cooldown: 3.8 -> 4.0 or attack 36 -> 32
# and monster items stats buff so they compete well:
# u_minh_quy_trao: attack 28 -> 32, basic gain_qi 2 -> 6
# bach_ho_cuong_mong: attack 26 -> 30
# soi_nanh_canh: attack 20 -> 24
# kich_doc_chu_nga: attack 22 -> 25

patch = {
    # Huyền Độc tuning
    "that_tinh_tieu.cooldown": 2.4,
    "that_tinh_tieu.attack": 6,
    "that_tinh_tieu.basic.poison": 2,
    "tu_kim_trao.cooldown": 1.7,
    "tu_kim_trao.attack": 10,
    "van_doc_dinh.triggers.drain_qi": 4,
    "u_minh_bi_luc.triggers.heal": 35,

    # Thái Thanh sword tuning (it was 77%)
    "thanh_phong_kiem.cooldown": 4.1,
    "thanh_phong_kiem.attack": 32,
    "thanh_phong_kiem.basic.gain_qi": 10,

    # Thái Thanh fan tuning (it was 70.7%)
    "ngoc_cot_phien.cooldown": 2.2,
    "ngoc_cot_phien.attack": 19,

    # Bích Thủy whip tuning (it was 67.9%)
    "thanh_dang_tien.cooldown": 4.0,
    "thanh_dang_tien.attack": 29,

    # Monster items tuning
    "u_minh_quy_trao.attack": 32,
    "u_minh_quy_trao.basic.gain_qi": 8,
    "u_minh_quy_trao.ultimate.cost": 30,
    "kich_doc_chu_nga.attack": 26,
    "kich_doc_chu_nga.cooldown": 1.7,
    "bach_ho_cuong_mong.attack": 30,
    "soi_nanh_canh.attack": 25,
    "hoa_diem_xa_cot.triggers.burn": 2,
    "thien_nien_quy_giap.triggers.shield": 260
}

patch_path = ROOT / "tools" / "gemini" / "out" / "round-2" / "test_patch.json"
with open(patch_path, "w", encoding="utf-8") as f:
    json.dump(patch, f, indent=2)

print("Running test Matrix A (50 runs)...")
cmd = [
    "py", "tools/balance/balance.py", "whatif",
    "--loadouts", str(LOADOUTS),
    "--patch", str(patch_path),
    "--items", str(CATALOG),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "50",
    "--low", "35", "--high", "65"
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
out_path = ROOT / "tools" / "gemini" / "out" / "round-2" / "test_matrix_out.txt"
with open(out_path, "w", encoding="utf-8") as f:
    f.write(res.stdout)

# Print parsed summary safely
for line in res.stdout.splitlines():
    if "Outside" in line or "Loadout" in line or "Bách" in line or "Bạch" in line or "Huyết" in line or "Băng" in line or "Xích" in line:
        safe_line = line.encode("ascii", "replace").decode("ascii")
        print(safe_line)
    elif "!" in line:
        safe_line = line.encode("ascii", "replace").decode("ascii")
        print(safe_line)

