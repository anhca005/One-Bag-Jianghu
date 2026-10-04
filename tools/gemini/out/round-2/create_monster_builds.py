import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
CATALOG_PATH = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"
LOADOUTS_DIR = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"

with open(CATALOG_PATH, "r", encoding="utf-8") as f:
    catalog = {it["id"]: it for it in json.load(f)}

def validate_bag(name: str, items: list[dict], cols=5, rows=4):
    grid = [[None for _ in range(cols)] for _ in range(rows)]
    for it in items:
        iid = it["id"]
        if iid not in catalog:
            raise ValueError(f"[{name}] Unknown item id: {iid}")
        w, h = catalog[iid]["size"]
        x, y = it["x"], it["y"]
        if x < 0 or y < 0 or x + w > cols or y + h > rows:
            raise ValueError(f"[{name}] Out of bounds: {iid} at ({x},{y}) size ({w},{h}) in {cols}x{rows}")
        for r in range(y, y + h):
            for c in range(x, x + w):
                if grid[r][c] is not None:
                    raise ValueError(f"[{name}] Overlap at ({c},{r}) between {grid[r][c]} and {iid}")
                grid[r][c] = iid

mb1 = {
    "name": "Huyền Độc Môn - Huyết Thú Cuồng Trảo",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "huyen_doc", "tamPhapLevel": 1,
    "items": [
        {"id": "u_minh_quy_trao", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "heo_rung_bap_giap", "x": 0, "y": 2, "rarity": "rare"},
        {"id": "kich_doc_chu_nga", "x": 2, "y": 0, "rarity": "rare"},
        {"id": "tu_kim_trao", "x": 2, "y": 2, "rarity": "rare"},
        {"id": "that_tinh_tieu", "x": 3, "y": 0, "rarity": "epic"},
        {"id": "da_ho_y", "x": 3, "y": 2, "rarity": "rare"},
        {"id": "man_xa_doc_tui", "x": 4, "y": 0, "rarity": "rare"},
        {"id": "hac_sat_hung_dam", "x": 4, "y": 1, "rarity": "epic"},
        {"id": "giai_doc_dan", "x": 4, "y": 2, "rarity": "uncommon"},
        {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
    ]
}

mb2 = {
    "name": "Hàn Sơn Phái - Băng Quy Tuyệt Hộ",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 1,
    "items": [
        {"id": "bang_phach_thuong", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "thien_nien_quy_giap", "x": 1, "y": 0, "rarity": "legendary"},
        {"id": "bang_phach_co_cot", "x": 1, "y": 2, "rarity": "legendary"},
        {"id": "tuyet_lang_bach_bi", "x": 3, "y": 0, "rarity": "rare"},
        {"id": "han_ngoc_tieu", "x": 3, "y": 1, "rarity": "epic"},
        {"id": "bang_tam_giap", "x": 3, "y": 3, "rarity": "rare"},
        {"id": "bang_tam_linh_chau", "x": 4, "y": 1, "rarity": "rare"},
        {"id": "han_bang_chau", "x": 4, "y": 2, "rarity": "rare"}
    ]
}

mb3 = {
    "name": "Liệt Diễm Đường - Xích Xà Hỏa Liệp",
    "hp": 1200, "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1,
    "items": [
        {"id": "liet_diem_dao", "x": 0, "y": 0, "rarity": "epic"},
        {"id": "liet_hoa_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
        {"id": "soi_nanh_canh", "x": 1, "y": 0, "rarity": "rare"},
        {"id": "hoa_diem_xa_cot", "x": 1, "y": 2, "rarity": "epic"},
        {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "rarity": "rare"},
        {"id": "tu_mau_song_dao", "x": 2, "y": 2, "rarity": "rare"},
        {"id": "chu_sa_ho_lo", "x": 4, "y": 0, "rarity": "epic"},
        {"id": "cuong_hoa_dan", "x": 4, "y": 1, "rarity": "uncommon"},
        {"id": "da_ho_y", "x": 4, "y": 2, "rarity": "rare"}
    ]
}

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

for name, mb in [("mb1", mb1), ("mb2", mb2), ("mb3", mb3), ("mb4", mb4)]:
    validate_bag(name, mb["items"])
    path = LOADOUTS_DIR / f"{name}.json"
    with open(path, "w", encoding="utf-8") as f:
        json.dump(mb, f, ensure_ascii=False, indent=2)
        f.write("\n")
    print(f"Validated and wrote {path}")
