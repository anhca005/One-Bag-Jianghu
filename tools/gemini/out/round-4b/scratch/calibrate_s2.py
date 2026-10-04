import json, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, "tools/balance")
import balance

cp = balance.ensure_classpath()
p2 = json.load(open("tools/gemini/out/round-4b/player/stage_02.json", encoding="utf-8"))
KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")

tests = [
    # C1: armor T1 unc, sword T1 unc
    ("armor T1 unc, sword T1 unc", [
        {"id": "phuc_ho_con",     "x": 0, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thiet_giap_y",    "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"}
    ]),
    # C2: armor T1 unc, staff T1 unc
    ("armor T1 unc, staff T1 unc", [
        {"id": "phuc_ho_con",     "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "thiet_giap_y",    "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"}
    ]),
    # C3: all 3 T1 unc
    ("all 3 T1 unc", [
        {"id": "phuc_ho_con",     "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thiet_giap_y",    "x": 2, "y": 0, "tier": 1, "rarity": "uncommon"}
    ]),
    # C4: armor T2 com, sword T1 com, staff T1 com
    ("armor T2 com", [
        {"id": "phuc_ho_con",     "x": 0, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "thanh_phong_kiem", "x": 1, "y": 0, "tier": 1, "rarity": "common"},
        {"id": "thiet_giap_y",    "x": 2, "y": 0, "tier": 2, "rarity": "common"}
    ]),
]

with tempfile.TemporaryDirectory() as t:
    t = Path(t)
    for name, items in tests:
        e = {"name": "Thảo Khấu Tiên Phong", "sect": None, "tamPhapLevel": 1, "level": 2, "items": items}
        e.setdefault("cols", 5); e.setdefault("rows", 4)
        p2.setdefault("cols", 5); p2.setdefault("rows", 4)
        balance.write_json(t / "p.json", {k: p2[k] for k in KEYS if k in p2})
        balance.write_json(t / "e.json", {k: e[k] for k in KEYS if k in e})
        cmd = ["java", "-cp", cp, balance.MAIN_CLASS, str(t / "p.json"), str(t / "e.json"),
               "--runs", "200",
               "--items", "server/battle-engine/src/main/resources/data/items.json",
               "--sects", "server/battle-engine/src/main/resources/data/sects.json"]
        out = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8").stdout
        pc = re.findall(r"([0-9.]+)%", out)
        wr = float(pc[0]) + float(pc[2]) / 2
        dur = float(re.search(r"TB ([0-9.]+)s", out).group(1))
        ok = 82 <= wr <= 88 and dur <= 30
        print(f"S2 {name:30s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s | {'OK' if ok else 'FAIL'}")
