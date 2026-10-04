import json
import subprocess
import shutil
import os
import sys
import copy
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R3 = ROOT / "tools" / "gemini" / "out" / "round-3"
BAL_DIR = R3 / "balance"
P_DIR = BAL_DIR / "player"
E_DIR = BAL_DIR / "enemy"
temp_a = BAL_DIR / "temp_a"
temp_b = BAL_DIR / "temp_b"
temp_a.mkdir(exist_ok=True)
temp_b.mkdir(exist_ok=True)

ITEMS_PATH = R3 / "items-v3-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
CONFIG = R3 / "config.json"

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

def eval_cand(cand_p, cand_e, runs=80):
    fname = "eval.json"
    with open(temp_a / fname, "w", encoding="utf-8") as f:
        json.dump(cand_p, f, indent=2, ensure_ascii=False)
    with open(temp_b / fname, "w", encoding="utf-8") as f:
        json.dump(cand_e, f, indent=2, ensure_ascii=False)
    cmd = [
        "py", "tools/balance/balance.py", "vs",
        "--a", str(temp_a),
        "--b", str(temp_b),
        "--runs", str(runs),
        "--items", str(ITEMS_PATH),
        "--sects", str(SECTS),
        "--battle-config", str(CONFIG)
    ]
    env = os.environ.copy()
    env["PYTHONUTF8"] = "1"
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    (temp_a / fname).unlink()
    (temp_b / fname).unlink()
    
    for line in res.stdout.splitlines():
        if "%" in line and not line.strip().startswith("("):
            for p in line.strip().split():
                if p.endswith("%"):
                    try:
                        return float(p[:-1])
                    except ValueError:
                        pass
    return None

# Fix Stage 3:
# soi_nanh_canh (1x2 at 0,0), da_ho_y (1x2 at 1,0), heo_rung_bap_giap (2x2 at 2,0)
p3 = json.load(open(P_DIR / "stage_03.json", "r", encoding="utf-8"))
e3 = {
    "name": "Ải 3: Hoang Dã Sói Xám",
    "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 3,
    "items": [
        {"id": "soi_nanh_canh", "x": 0, "y": 0, "tier": 1, "rarity": "rare"},
        {"id": "da_ho_y", "x": 1, "y": 0, "tier": 1, "rarity": "rare"},
        {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"}
    ]
}
wr3 = eval_cand(p3, e3, runs=60)
print(f"Stage 03 tuned: {wr3}%")
with open(E_DIR / "stage_03.json", "w", encoding="utf-8") as f: json.dump(e3, f, indent=2, ensure_ascii=False)

# Fix Stage 4:
p4 = json.load(open(P_DIR / "stage_04.json", "r", encoding="utf-8"))
e4 = {
    "name": "Ải 4: Liệt Diệm Bang Chúng",
    "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 4,
    "items": [
        {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 1, "rarity": "epic"},
        {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 1, "rarity": "epic"},
        {"id": "thuan_duong_cong", "x": 0, "y": 2, "tier": 1, "rarity": "rare"},
        {"id": "boi_nguyen_dan", "x": 2, "y": 2, "tier": 1, "rarity": "rare"}
    ]
}
wr4 = eval_cand(p4, e4, runs=60)
print(f"Stage 04 tuned: {wr4}%")
with open(E_DIR / "stage_04.json", "w", encoding="utf-8") as f: json.dump(e4, f, indent=2, ensure_ascii=False)

# Fix Stage 10 (Boss: 50-60%):
p10 = json.load(open(P_DIR / "stage_10.json", "r", encoding="utf-8"))
e10 = {
    "name": "Ải 10: Nộ Ba Hổ Bùi Chấn",
    "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1, "level": 10,
    "items": [
        {"id": "liet_diem_dao", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "cuong_hoa_dan", "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "liet_hoa_tam_kinh", "x": 0, "y": 2, "tier": 2, "rarity": "rare"},
        {"id": "chu_sa_ho_lo", "x": 2, "y": 2, "tier": 2, "rarity": "rare"}
    ]
}
wr10 = eval_cand(p10, e10, runs=60)
print(f"Stage 10 tuned: {wr10}%")
with open(E_DIR / "stage_10.json", "w", encoding="utf-8") as f: json.dump(e10, f, indent=2, ensure_ascii=False)

# Fix Stage 13:
p13 = json.load(open(P_DIR / "stage_13.json", "r", encoding="utf-8"))
e13 = {
    "name": "Ải 13: Kịch Độc Tri Chu",
    "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 13,
    "items": [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "da_ho_y", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "man_xa_doc_tui", "x": 2, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "heo_rung_bap_giap", "x": 3, "y": 0, "tier": 2, "rarity": "rare"}
    ]
}
wr13 = eval_cand(p13, e13, runs=60)
print(f"Stage 13 tuned: {wr13}%")
with open(E_DIR / "stage_13.json", "w", encoding="utf-8") as f: json.dump(e13, f, indent=2, ensure_ascii=False)

# Fix Stage 16:
p16 = json.load(open(P_DIR / "stage_16.json", "r", encoding="utf-8"))
e16 = {
    "name": "Ải 16: Hắc Sát Độc Hùng",
    "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 16,
    "items": [
        {"id": "hac_sat_hung_dam", "x": 0, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "heo_rung_bap_giap", "x": 1, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "thiet_cot_quyen_sao", "x": 3, "y": 0, "tier": 3, "rarity": "rare"},
        {"id": "thanh_dong_ho_tam_kinh", "x": 0, "y": 2, "tier": 3, "rarity": "rare"},
        {"id": "thiet_giap_y", "x": 1, "y": 2, "tier": 3, "rarity": "rare"}
    ]
}
wr16 = eval_cand(p16, e16, runs=60)
print(f"Stage 16 tuned: {wr16}%")
with open(E_DIR / "stage_16.json", "w", encoding="utf-8") as f: json.dump(e16, f, indent=2, ensure_ascii=False)

# Fix Stage 21:
p21 = json.load(open(P_DIR / "stage_21.json", "r", encoding="utf-8"))
e21 = {
    "name": "Ải 21: Hàn Phong Tuần Tuyết Giả",
    "cols": 5, "rows": 4, "sect": "han_son", "tamPhapLevel": 3, "level": 21,
    "items": [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "han_bang_chau", "x": 1, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "bang_tam_giap", "x": 2, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "da_ho_y", "x": 3, "y": 0, "tier": 3, "rarity": "rare"}
    ]
}
wr21 = eval_cand(p21, e21, runs=60)
print(f"Stage 21 tuned: {wr21}%")
with open(E_DIR / "stage_21.json", "w", encoding="utf-8") as f: json.dump(e21, f, indent=2, ensure_ascii=False)

# Fix Stage 23:
p23 = json.load(open(P_DIR / "stage_23.json", "r", encoding="utf-8"))
e23 = {
    "name": "Ải 23: Tuyết Lĩnh Ngân Lang",
    "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 23,
    "items": [
        {"id": "soi_nanh_canh", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "tuyet_lang_bach_bi", "x": 1, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "da_ho_y", "x": 2, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "thanh_dong_ho_tam_kinh", "x": 3, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "thiet_giap_y", "x": 0, "y": 2, "tier": 4, "rarity": "rare"}
    ]
}
wr23 = eval_cand(p23, e23, runs=60)
print(f"Stage 23 tuned: {wr23}%")
with open(E_DIR / "stage_23.json", "w", encoding="utf-8") as f: json.dump(e23, f, indent=2, ensure_ascii=False)

# Fix Stage 28:
p28 = json.load(open(P_DIR / "stage_28.json", "r", encoding="utf-8"))
e28 = {
    "name": "Ải 28: Vạn Niên Băng Quy",
    "cols": 5, "rows": 4, "sect": None, "tamPhapLevel": 1, "level": 28,
    "items": [
        {"id": "bang_phach_co_cot", "x": 0, "y": 0, "tier": 5, "rarity": "rare"},
        {"id": "thien_nien_quy_giap", "x": 1, "y": 0, "tier": 5, "rarity": "rare"},
        {"id": "han_bang_chau", "x": 3, "y": 0, "tier": 5, "rarity": "rare"},
        {"id": "thiet_giap_y", "x": 0, "y": 2, "tier": 5, "rarity": "rare"}
    ]
}
wr28 = eval_cand(p28, e28, runs=60)
print(f"Stage 28 tuned: {wr28}%")
with open(E_DIR / "stage_28.json", "w", encoding="utf-8") as f: json.dump(e28, f, indent=2, ensure_ascii=False)

print("Remaining stages tuned!")
