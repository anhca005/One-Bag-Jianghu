import json, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, "tools/balance")
import balance

cp = balance.ensure_classpath()
p11 = json.load(open("tools/gemini/out/round-4b/player/stage_11.json", encoding="utf-8"))
KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")

tests = [
    # whip T1 rare, cam T2 rare
    ("cam T2 rare, whip T1 rare", [
        {"id": "bich_thuy_cam",  "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "thanh_dang_tien","x": 2, "y": 0, "tier": 1, "rarity": "rare"},
        {"id": "da_ho_y",        "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "duong_sinh_quyet","x": 4, "y": 0, "tier": 2, "rarity": "rare"}
    ]),
    # whip T1 unc, cam T2 rare
    ("cam T2 rare, whip T1 unc", [
        {"id": "bich_thuy_cam",  "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "thanh_dang_tien","x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "da_ho_y",        "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "duong_sinh_quyet","x": 4, "y": 0, "tier": 2, "rarity": "rare"}
    ]),
    # whip T1 unc, cam T1 unc
    ("cam T1 unc, whip T1 unc", [
        {"id": "bich_thuy_cam",  "x": 0, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "thanh_dang_tien","x": 2, "y": 0, "tier": 1, "rarity": "uncommon"},
        {"id": "da_ho_y",        "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "duong_sinh_quyet","x": 4, "y": 0, "tier": 2, "rarity": "rare"}
    ]),
    # whip T2 unc, cam T2 unc
    ("cam T2 unc, whip T2 unc", [
        {"id": "bich_thuy_cam",  "x": 0, "y": 0, "tier": 2, "rarity": "uncommon"},
        {"id": "thanh_dang_tien","x": 2, "y": 0, "tier": 2, "rarity": "uncommon"},
        {"id": "da_ho_y",        "x": 3, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "duong_sinh_quyet","x": 4, "y": 0, "tier": 2, "rarity": "rare"}
    ]),
]

with tempfile.TemporaryDirectory() as t:
    t = Path(t)
    for name, items in tests:
        e = {"name": "Bích Thủy Cung Nữ", "sect": "bich_thuy", "tamPhapLevel": 1, "level": 11, "items": items}
        e.setdefault("cols", 5); e.setdefault("rows", 4)
        p11.setdefault("cols", 5); p11.setdefault("rows", 4)
        balance.write_json(t / "p.json", {k: p11[k] for k in KEYS if k in p11})
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
        print(f"S11 {name:30s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s | {'OK' if ok else 'FAIL'}")
