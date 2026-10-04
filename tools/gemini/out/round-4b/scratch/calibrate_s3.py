import json, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, "tools/balance")
import balance

cp = balance.ensure_classpath()
p3 = json.load(open("tools/gemini/out/round-4b/player/stage_03.json", encoding="utf-8"))
KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")

tests = [
    # Sword + staff + thiet_giap
    ("sword + staff + thiet_giap T1 unc", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "phuc_ho_con",      "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thiet_giap_y",     "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
    ]),
    # Sword + fist + thiet_giap
    ("sword + fist + thiet_giap T1 unc", [
        {"id": "thanh_phong_kiem",    "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thiet_giap_y",        "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
    ]),
    # Sword + fist + da_ho_y
    ("sword + fist + da_ho T1 unc", [
        {"id": "thanh_phong_kiem",    "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thiet_cot_quyen_sao", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "da_ho_y",             "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
    ]),
    # Sword + staff + da_ho_y
    ("sword + staff + da_ho T1 unc", [
        {"id": "thanh_phong_kiem", "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "phuc_ho_con",      "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "da_ho_y",          "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
    ]),
    # 4 items with common rarities
    ("sword + staff + fist + da_ho (mixed com/unc)", [
        {"id": "thiet_cot_quyen_sao", "x": 0, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "phuc_ho_con",         "x": 1, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "da_ho_y",             "x": 2, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "thanh_phong_kiem",    "x": 3, "y": 0, "tier": 1, "rarity": "common"}
    ]),
]

with tempfile.TemporaryDirectory() as t:
    t = Path(t)
    for name, items in tests:
        e = {"name": "Sơn Trại Đầu Mục", "sect": None, "tamPhapLevel": 1, "level": 3, "items": items}
        e.setdefault("cols", 5); e.setdefault("rows", 4)
        p3.setdefault("cols", 5); p3.setdefault("rows", 4)
        balance.write_json(t / "p.json", {k: p3[k] for k in KEYS if k in p3})
        balance.write_json(t / "e.json", {k: e[k] for k in KEYS if k in e})
        cmd = ["java", "-cp", cp, balance.MAIN_CLASS, str(t / "p.json"), str(t / "e.json"),
               "--runs", "200",
               "--items", "server/battle-engine/src/main/resources/data/items.json",
               "--sects", "server/battle-engine/src/main/resources/data/sects.json"]
        out = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8").stdout
        pc = re.findall(r"([0-9.]+)%", out)
        wr = float(pc[0]) + float(pc[2]) / 2
        dur = float(re.search(r"TB ([0-9.]+)s", out).group(1))
        ok = 82 <= wr <= 88 and dur <= 40
        print(f"S3 {name:40s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s | {'OK' if ok else 'FAIL'}")
