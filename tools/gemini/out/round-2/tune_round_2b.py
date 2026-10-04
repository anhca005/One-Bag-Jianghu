import json, subprocess
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
    {"id": "bang_phach_thuong", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "truy_hon_cung", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "han_bang_chau", "x": 1, "y": 3, "rarity": "rare"},
    {"id": "thien_nien_quy_giap", "x": 2, "y": 0, "rarity": "legendary"},
    {"id": "tuyet_lang_bach_bi", "x": 2, "y": 2, "rarity": "rare"},
    {"id": "bang_tam_linh_chau", "x": 2, "y": 3, "rarity": "rare"},
    {"id": "thanh_tam_dan", "x": 3, "y": 3, "rarity": "rare"},
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

# Update thai_thanh_kiem_khi to the authentic 3-sword loadout
thai_thanh_kiem_khi = {
  "name": "Thái Thanh Quán - Kiếm Khí Xuyên Giáp",
  "hp": 1200, "cols": 5, "rows": 4, "sect": "thai_thanh", "tamPhapLevel": 1,
  "items": [
    {"id": "thanh_phong_kiem", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "thai_thanh_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
    {"id": "thanh_phong_kiem", "x": 1, "y": 0, "rarity": "epic"},
    {"id": "kiem_tong_tam_yeu", "x": 1, "y": 3, "rarity": "epic"},
    {"id": "thanh_phong_kiem", "x": 2, "y": 0, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 2, "y": 3, "rarity": "uncommon"},
    {"id": "o_tam_y", "x": 3, "y": 0, "rarity": "rare"},
    {"id": "thien_tam_chuoi", "x": 3, "y": 2, "rarity": "epic"},
    {"id": "bach_ngoc_boi", "x": 4, "y": 2, "rarity": "epic"},
    {"id": "tuy_tien_ho_lo", "x": 4, "y": 3, "rarity": "epic"}
  ]
}

ok, msg = validate_grid(thai_thanh_kiem_khi)
assert ok, f"thai_thanh_kiem_khi invalid: {msg}"
with open(LOADOUTS / "thai_thanh_kiem_khi.json", "w", encoding="utf-8") as f:
    json.dump(thai_thanh_kiem_khi, f, indent=2, ensure_ascii=False)
print("Updated thai_thanh_kiem_khi loadout to 3 swords.")

patch = {
    # Huyền Độc tuning
    "that_tinh_tieu.cooldown": 2.4,
    "that_tinh_tieu.attack": 7,
    "that_tinh_tieu.basic.poison": 2,
    "tu_kim_trao.cooldown": 1.7,
    "tu_kim_trao.attack": 11,
    "van_doc_dinh.triggers.drain_qi": 4,
    "u_minh_bi_luc.triggers.heal": 35,

    # Thái Thanh sword tuning
    "thanh_phong_kiem.cooldown": 3.9,
    "thanh_phong_kiem.attack": 34,
    "thanh_phong_kiem.basic.gain_qi": 11,
    "thien_tam_chuoi.triggers.stun": 0.2,

    # Thái Thanh fan tuning
    "ngoc_cot_phien.cooldown": 2.2,
    "ngoc_cot_phien.attack": 19,

    # Bích Thủy whip tuning
    "thanh_dang_tien.cooldown": 3.9,
    "thanh_dang_tien.attack": 29,

    # Monster items tuning
    "u_minh_quy_trao.attack": 30,
    "u_minh_quy_trao.basic.gain_qi": 8,
    "u_minh_quy_trao.basic.poison": 3,
    "u_minh_quy_trao.ultimate.cost": 25,
    "kich_doc_chu_nga.attack": 25,
    "kich_doc_chu_nga.cooldown": 1.6,
    "kich_doc_chu_nga.basic.gain_qi": 4,
    "kich_doc_chu_nga.basic.poison": 3,
    "bach_ho_cuong_mong.attack": 18,
    "soi_nanh_canh.attack": 23,
    "hoa_diem_xa_cot.triggers.burn": 2,
    "thien_nien_quy_giap.triggers.shield": 280,
    "bang_phach_thuong.attack": 125,
    "bang_phach_thuong.cooldown": 4.6
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

for line in res.stdout.splitlines():
    if "Outside" in line:
        print(line)
