import json, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, "tools/balance")
import balance

cp = balance.ensure_classpath()
p13 = json.load(open("tools/gemini/out/round-4b/player/stage_13.json", encoding="utf-8"))
KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")

tests = [
    # o_thiet_giap T2 rare
    ("o_thiet_giap T2 rare", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "epic"},
        {"id": "thanh_dang_tien", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "o_thiet_giap",     "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "man_xa_doc_tui",   "x": 4, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "u_minh_bi_luc",    "x": 4, "y": 1, "tier": 2, "rarity": "epic"}
    ]),
    # o_thiet_giap T2 rare, kich_doc T3 epic
    ("o_thiet T2 rare, kich_doc T3", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 3, "rarity": "epic"},
        {"id": "thanh_dang_tien", "x": 1, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "o_thiet_giap",     "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "man_xa_doc_tui",   "x": 4, "y": 0, "tier": 2, "rarity": "epic"},
        {"id": "u_minh_bi_luc",    "x": 4, "y": 1, "tier": 2, "rarity": "epic"}
    ]),
    # o_thiet_giap T2 rare, all rare
    ("all T2 rare, kich_doc T4", [
        {"id": "kich_doc_chu_nga", "x": 0, "y": 0, "tier": 4, "rarity": "rare"},
        {"id": "thanh_dang_tien", "x": 1, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "o_thiet_giap",     "x": 2, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "man_xa_doc_tui",   "x": 4, "y": 0, "tier": 2, "rarity": "rare"},
        {"id": "u_minh_bi_luc",    "x": 4, "y": 1, "tier": 2, "rarity": "rare"}
    ]),
]

with tempfile.TemporaryDirectory() as t:
    t = Path(t)
    for name, items in tests:
        e = {"name": "Man Uyên Độc Thủ", "sect": "huyen_doc", "tamPhapLevel": 2, "level": 13, "items": items}
        e.setdefault("cols", 5); e.setdefault("rows", 4)
        p13.setdefault("cols", 5); p13.setdefault("rows", 4)
        balance.write_json(t / "p.json", {k: p13[k] for k in KEYS if k in p13})
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
        print(f"S13 {name:30s}: WR = {wr:5.1f}%, dur = {dur:4.1f}s | {'OK' if ok else 'FAIL'}")
