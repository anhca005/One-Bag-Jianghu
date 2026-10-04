import json, subprocess
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"
ITEMS = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

mb3 = {
  "name": "Liệt Diễm Đường - Xích Xà Hỏa Liệp",
  "hp": 1200, "cols": 5, "rows": 4, "sect": "liet_diem", "tamPhapLevel": 1,
  "items": [
    {"id": "liet_diem_dao", "x": 0, "y": 0, "rarity": "epic"},
    {"id": "liet_hoa_tam_kinh", "x": 0, "y": 3, "rarity": "epic"},
    {"id": "hoa_diem_xa_cot", "x": 1, "y": 0, "rarity": "rare"},
    {"id": "soi_nanh_canh", "x": 1, "y": 2, "rarity": "rare"},
    {"id": "heo_rung_bap_giap", "x": 2, "y": 0, "rarity": "rare"},
    {"id": "da_ho_y", "x": 2, "y": 2, "rarity": "rare"},
    {"id": "chu_sa_ho_lo", "x": 3, "y": 2, "rarity": "epic"},
    {"id": "cuong_hoa_dan", "x": 3, "y": 3, "rarity": "uncommon"},
    {"id": "liet_diem_dao", "x": 4, "y": 0, "rarity": "epic"},
    {"id": "boi_nguyen_dan", "x": 4, "y": 3, "rarity": "uncommon"}
  ]
}

with open(LOADOUTS / "mb3.json", "w", encoding="utf-8") as f:
    json.dump(mb3, f, indent=2, ensure_ascii=False)
print("Updated mb3 with heo_rung_bap_giap.")

cmd = [
    "py", "tools/balance/balance.py", "rarity-step",
    "--loadouts", str(LOADOUTS),
    "--tier", "3",
    "--tp", "1",
    "--runs", "50",
    "--items", str(ITEMS),
    "--sects", str(SECTS)
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
for line in res.stdout.splitlines():
    if "Xích Xà" in line or "100.0%" in line or "Average" in line:
        print(line.encode("ascii", "replace").decode("ascii"))
