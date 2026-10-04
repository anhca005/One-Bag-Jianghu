import json
import os
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")

ROOT = Path(__file__).resolve().parents[5]
CATALOG_PATH = ROOT / "tools" / "gemini" / "out" / "pve" / "combined_items.json"
OUT_PVE = ROOT / "tools" / "gemini" / "out" / "pve"
BALANCE_DIR = OUT_PVE / "balance"
P_DIR = BALANCE_DIR / "player_loadouts"
E_DIR = BALANCE_DIR / "enemy_loadouts"

# Import chapter modules
sys.path.insert(0, str(BALANCE_DIR))
from chapter_1_data import CHAPTER_1, P_LOADOUTS_1
from chapter_2_data import CHAPTER_2, P_LOADOUTS_2
from chapter_3_data import CHAPTER_3, P_LOADOUTS_3

with open(CATALOG_PATH, encoding="utf-8") as f:
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

def validate_dialogue(stage_id: str, dialogue: dict):
    for part in ["before", "afterWin", "afterLoss"]:
        lines = dialogue.get(part, [])
        if len(lines) < 2:
            raise ValueError(f"[{stage_id}] {part} has fewer than 2 lines")
        for line in lines:
            text = line.get("text", "")
            if len(text) > 120:
                raise ValueError(f"[{stage_id}] {part} line exceeds 120 chars: {text} ({len(text)} chars)")

CHARACTERS = [
    {
        "id": "hero",
        "name": "Hiệp Khách",
        "title": "Thiếu Niên Hành Giả",
        "portrait": "hero_portrait"
    },
    {
        "id": "lao_tieu_phu",
        "name": "Lục Tiều Phu",
        "title": "Hộ Nang Ẩn Sĩ",
        "portrait": "npc_lao_tieu_phu"
    },
    {
        "id": "tieu_linh_nhi",
        "name": "Bạch Linh Nhi",
        "title": "Vạn Hoa Dược Nữ",
        "portrait": "npc_tieu_linh_nhi"
    },
    {
        "id": "doc_co_hanh",
        "name": "Độc Cô Hạnh",
        "title": "Phong Lôi Khoái Đao",
        "portrait": "npc_doc_co_hanh"
    },
    {
        "id": "bui_chen",
        "name": "Bùi Chấn",
        "title": "Nộ Ba Hổ",
        "portrait": "boss_bui_chen"
    },
    {
        "id": "hac_doc_nuong_tu",
        "name": "Hắc Độc Nương Tử",
        "title": "Vạn Xà Độc Ma",
        "portrait": "boss_hac_doc_nuong_tu"
    },
    {
        "id": "da_sat_thien",
        "name": "Dạ Sát Thiên",
        "title": "U Minh Tả Sứ",
        "portrait": "boss_da_sat_thien"
    }
]

MONSTER_ITEMS = [
    "soi_nanh_canh",
    "heo_rung_bap_giap",
    "man_xa_doc_tui",
    "u_minh_quy_trao",
    "kich_doc_chu_nga",
    "bang_tam_linh_chau",
    "tuyet_lang_bach_bi",
    "hac_sat_hung_dam",
    "bach_ho_cuong_mong",
    "hoa_ngiem_xa_cot",
    "bang_phach_co_cot",
    "thien_nien_quy_giap"
]

all_chapters = [CHAPTER_1, CHAPTER_2, CHAPTER_3]
all_player_loadouts = P_LOADOUTS_1 + P_LOADOUTS_2 + P_LOADOUTS_3

# Validation
print("Validating content...")
stage_count = 0
for ch_idx, ch in enumerate(all_chapters, 1):
    for st_idx, st in enumerate(ch["stages"], 1):
        stage_count += 1
        st_id = st["id"]
        # Validate dialogue
        validate_dialogue(st_id, st["dialogue"])
        # Validate enemy bag
        e_info = st["enemy"]
        validate_bag(f"Enemy {st_id}", e_info["items"])
        # Validate player bag
        p_info = all_player_loadouts[stage_count - 1]
        validate_bag(f"Player {st_id}", p_info["items"])

print(f"Validation successful for {stage_count} stages!")

# Write chapters.json
chapters_json_data = {
    "characters": CHARACTERS,
    "monsterItems": MONSTER_ITEMS,
    "chapters": all_chapters
}

chapters_json_path = OUT_PVE / "chapters.json"
with open(chapters_json_path, "w", encoding="utf-8") as f:
    json.dump(chapters_json_data, f, ensure_ascii=False, indent=2)
print(f"Wrote {chapters_json_path}")

# Write loadouts
P_DIR.mkdir(parents=True, exist_ok=True)
E_DIR.mkdir(parents=True, exist_ok=True)

# Also chapter subfolders
for c in [1, 2, 3]:
    (BALANCE_DIR / f"c{c}_player").mkdir(parents=True, exist_ok=True)
    (BALANCE_DIR / f"c{c}_enemy").mkdir(parents=True, exist_ok=True)

stage_global_idx = 0
for ch_idx, ch in enumerate(all_chapters, 1):
    for st_idx, st in enumerate(ch["stages"], 1):
        stage_global_idx += 1
        st_id = st["id"]
        num_str = f"{stage_global_idx:02d}"
        
        # Player loadout
        p_loadout = all_player_loadouts[stage_global_idx - 1]
        p_loadout_copy = dict(p_loadout)
        p_loadout_copy["cols"] = 5
        p_loadout_copy["rows"] = 4
        
        p_file = P_DIR / f"stage_{num_str}.json"
        with open(p_file, "w", encoding="utf-8") as f:
            json.dump(p_loadout_copy, f, ensure_ascii=False, indent=2)
            
        c_p_file = BALANCE_DIR / f"c{ch_idx}_player" / f"stage_{num_str}.json"
        with open(c_p_file, "w", encoding="utf-8") as f:
            json.dump(p_loadout_copy, f, ensure_ascii=False, indent=2)
            
        # Enemy loadout
        e_info = st["enemy"]
        e_loadout = {
            "name": f"Ải {stage_global_idx}: {e_info['name']}",
            "hp": e_info["hp"],
            "cols": 5,
            "rows": 4,
            "sect": e_info["sect"],
            "tamPhapLevel": e_info["tamPhapLevel"],
            "items": e_info["items"]
        }
        
        e_file = E_DIR / f"stage_{num_str}.json"
        with open(e_file, "w", encoding="utf-8") as f:
            json.dump(e_loadout, f, ensure_ascii=False, indent=2)
            
        c_e_file = BALANCE_DIR / f"c{ch_idx}_enemy" / f"stage_{num_str}.json"
        with open(c_e_file, "w", encoding="utf-8") as f:
            json.dump(e_loadout, f, ensure_ascii=False, indent=2)

print(f"Generated all {stage_global_idx} player and enemy loadouts successfully!")
