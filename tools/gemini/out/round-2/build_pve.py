import json, re
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
SRC_CHAPTERS = ROOT / "tools" / "gemini" / "out" / "pve" / "chapters.json"
DST_CHAPTERS = ROOT / "tools" / "gemini" / "out" / "round-2" / "chapters.json"
DST_BALANCE = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance"
DST_BALANCE.mkdir(parents=True, exist_ok=True)
PLAYER_DIR = DST_BALANCE / "player"
ENEMY_DIR = DST_BALANCE / "enemy"
PLAYER_DIR.mkdir(parents=True, exist_ok=True)
ENEMY_DIR.mkdir(parents=True, exist_ok=True)

with open(SRC_CHAPTERS, "r", encoding="utf-8") as f:
    data = json.load(f)

# 1. Update characters list
for char in data.get("characters", []):
    if char["id"] == "doc_co_hanh":
        char["id"] = "phong_loi"
        char["name"] = "Phong Vũ"
        char["title"] = "Phong Lôi Khoái Đao"
        char["portrait"] = "npc_phong_loi"
    elif char["id"] == "tieu_linh_nhi":
        char["title"] = "Bách Thảo Dược Nữ"

# 2. Update monsterItems list
data["monsterItems"] = [
    "soi_nanh_canh", "heo_rung_bap_giap", "man_xa_doc_tui", "u_minh_quy_trao",
    "kich_doc_chu_nga", "bang_tam_linh_chau", "tuyet_lang_bach_bi", "hac_sat_hung_dam",
    "bach_ho_cuong_mong", "hoa_diem_xa_cot", "bang_phach_co_cot", "thien_nien_quy_giap"
]

# Text replacement helper
def replace_text(s):
    if not isinstance(s, str):
        return s
    s = s.replace("Độc Cô Hạnh", "Phong Vũ")
    s = s.replace("doc_co_hanh", "phong_loi")
    s = s.replace("Vạn Hoa Cốc", "Bách Thảo Cốc")
    s = s.replace("Vạn Hoa Dược Nữ", "Bách Thảo Dược Nữ")
    s = s.replace("hoa_ngiem_xa_cot", "hoa_diem_xa_cot")
    s = s.replace("Vi Phẩm", "Tinh Phẩm")
    s = s.replace("Cực Phẩm", "Tuyệt Phẩm")
    return s

def recursive_replace(obj):
    if isinstance(obj, dict):
        return {replace_text(k): recursive_replace(v) for k, v in obj.items()}
    elif isinstance(obj, list):
        return [recursive_replace(item) for item in obj]
    elif isinstance(obj, str):
        return replace_text(obj)
    return obj

data = recursive_replace(data)

# Add level 10 sect invite dialogue in stage 10 afterWin
stage_10 = data["chapters"][0]["stages"][9]
dialogue_win = stage_10["dialogue"]["afterWin"]
# Add invitation lines if not already present
dialogue_win.append({
    "speaker": "lao_tieu_phu",
    "text": "Trận chiến này giúp ngươi đả thông kinh mạch, tấn thăng Đệ Thập Trọng (Cấp 10)! Giang hồ rộng lớn, hãy chọn cho mình một môn phái để tu luyện Tâm Pháp nhập thế."
})
dialogue_win.append({
    "speaker": "hero",
    "text": "Đa tạ tiền bối chỉ bảo. Ta sẽ tìm đến sơn môn thích hợp để bái sư học nghệ!"
})

with open(DST_CHAPTERS, "w", encoding="utf-8") as f:
    json.dump(data, f, indent=2, ensure_ascii=False)

print(f"Saved initial cleaned chapters to {DST_CHAPTERS}")
