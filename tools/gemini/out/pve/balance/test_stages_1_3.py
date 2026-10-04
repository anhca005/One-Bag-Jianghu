import json
import os
import subprocess
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")

ROOT = Path(__file__).resolve().parents[5]
CATALOG = ROOT / "tools" / "gemini" / "out" / "pve" / "combined_items.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
P_DIR = ROOT / "tools" / "gemini" / "out" / "pve" / "balance" / "player_loadouts"
E_DIR = ROOT / "tools" / "gemini" / "out" / "pve" / "balance" / "enemy_loadouts"

# Starter loadout
p_starter = {
    "name": "Hiệp Khách (Khởi Đầu)",
    "hp": 1000,
    "cols": 5, "rows": 4,
    "sect": None,
    "tamPhapLevel": 1,
    "items": [
        {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "o_tam_y", "x": 3, "y": 0, "tier": 1, "rarity": "common"}
    ]
}

e_01 = {
    "name": "Ải 1: Thảo Khấu Tiên Phong",
    "hp": 750,
    "cols": 5, "rows": 4,
    "sect": None,
    "tamPhapLevel": 1,
    "items": [
        {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "thiet_giap_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
    ]
}

e_02 = {
    "name": "Ải 2: Thủy Phỉ Thám Tử",
    "hp": 850,
    "cols": 5, "rows": 4,
    "sect": None,
    "tamPhapLevel": 1,
    "items": [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "o_tam_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
    ]
}

e_03 = {
    "name": "Ải 3: Hoang Dã Sói Xám",
    "hp": 900,
    "cols": 5, "rows": 4,
    "sect": None,
    "tamPhapLevel": 1,
    "items": [
        {"id": "soi_nanh_canh", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "da_ho_y", "x": 2, "y": 0, "tier": 1, "rarity": "common"}
    ]
}

test_p = P_DIR / "test_p.json"
with open(test_p, "w", encoding="utf-8") as f:
    json.dump(p_starter, f, indent=2)

for i, enemy in enumerate([e_01, e_02, e_03], 1):
    test_e = E_DIR / f"test_e_{i}.json"
    with open(test_e, "w", encoding="utf-8") as f:
        json.dump(enemy, f, indent=2)

cmd = [
    "py", str(ROOT / "tools" / "balance" / "balance.py"), "vs",
    "--a", str(P_DIR),
    "--b", str(E_DIR),
    "--runs", "100",
    "--items", str(CATALOG),
    "--sects", str(SECTS)
]
print("Running vs...")
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
print("STDOUT:\n", res.stdout)
if res.stderr:
    print("STDERR:\n", res.stderr)
