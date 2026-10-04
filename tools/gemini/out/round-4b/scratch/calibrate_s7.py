import json, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, "tools/balance")
import balance

cp = balance.ensure_classpath()
p7 = json.load(open("tools/gemini/out/round-4b/player/stage_07.json", encoding="utf-8"))
KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")

# Enemy 7: Giang Hồ Khoái Đao, sect: liet_diem, level 7
# Player 7: lv 7, top tier 2, top rarity rare
tests = [
    # C1: both rare
    ("all T2 rare", [
        {"id": "liet_diem_dao",   "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "phan_quan_but",   "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "heo_rung_bap_giap","x": 2, "y": 0, "tier": 2, "rarity": "rare"}
    ]),
    # C2: but rare, armor epic
    ("but rare, armor epic", [
        {"id": "liet_diem_dao",   "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "phan_quan_but",   "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "heo_rung_bap_giap","x": 2, "y": 0, "tier": 2, "rarity": "epic"}
    ]),
    # C3: but epic, armor rare
    ("but epic, armor rare", [
        {"id": "liet_diem_dao",   "x": 0, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "phan_quan_but",   "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "heo_rung_bap_giap","x": 2, "y": 0, "tier": 2, "rarity": "rare"}
    ]),
]

with tempfile.TemporaryDirectory() as t:
    t = Path(t)
    for name, items in tests:
        e = {"name": "Giang Hồ Khoái Đao", "sect": "liet_diem", "tamPhapLevel": 1, "level": 7, "items": items}
        e.setdefault("cols", 5); e.setdefault("rows", 4)
        p7.setdefault("cols", 5); p7.setdefault("rows", 4)
        balance.write_json(t / "p.json", {k: p7[k] for k in KEYS if k in p7})
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
        print(f"S7 {name:25s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s | {'OK' if ok else 'FAIL'}")
