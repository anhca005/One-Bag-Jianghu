import json, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, "tools/balance")
import balance

RARITY = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")

def band(i):
    return (50, 60) if i % 10 == 0 else (60, 70) if i % 5 == 0 else (80, 90)

cp = balance.ensure_classpath()
doc = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
stages = [s for c in doc["chapters"] for s in c["stages"]]

with tempfile.TemporaryDirectory() as t:
    t = Path(t)
    for i, s in enumerate(stages, 1):
        p = json.load(open(f"tools/gemini/out/round-4b/player/stage_{i:02d}.json", encoding="utf-8"))
        e = dict(s["enemy"])
        e.setdefault("cols", 5); e.setdefault("rows", 4)
        p.setdefault("cols", 5); p.setdefault("rows", 4)
        balance.write_json(t / "p.json", {k: p[k] for k in KEYS if k in p})
        balance.write_json(t / "e.json", {k: e[k] for k in KEYS if k in e})
        cmd = ["java", "-cp", cp, balance.MAIN_CLASS, str(t / "p.json"), str(t / "e.json"),
               "--runs", "200",
               "--items", "server/battle-engine/src/main/resources/data/items.json",
               "--sects", "server/battle-engine/src/main/resources/data/sects.json"]
        res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8")
        out = res.stdout
        pc = re.findall(r"([0-9.]+)%", out)
        if not pc:
            print(f"{i:02d} ERROR: {res.stderr[:200]}")
            continue
        wr = float(pc[0]) + float(pc[2]) / 2
        dur = float(re.search(r"TB ([0-9.]+)s", out).group(1))
        lo, hi = band(i)
        notes = []
        if not (lo + 2 <= wr <= hi - 2): notes.append(f"band {lo+2}-{hi-2}")
        if dur > 40: notes.append("dur>40s")
        rec = s.get("recommendedLevel", p["level"])
        if s["kind"] == "boss" and e["level"] != rec + 1: notes.append("boss!=rec+1")
        if s["kind"] != "boss" and not (rec - 2 <= e["level"] <= rec + 2): notes.append("lvl!=rec±2")
        if e["level"] > 31: notes.append("lvl>31")
        pT = max(x["tier"] for x in p["items"]); eT = max(x["tier"] for x in e["items"])
        if not (pT - 1 <= eT <= pT + 1): notes.append("tier!=player±1")
        pR = max(RARITY.index(x["rarity"]) for x in p["items"])
        eR = max(RARITY.index(x["rarity"]) for x in e["items"])
        up = 1 if s["kind"] == "normal" else 2
        if not (pR - 1 <= eR <= pR + up): notes.append(f"rarity!=player-1..+{up}")
        status = "OK" if not notes else ", ".join(notes)
        print(f"{i:02d} {s['kind']:6s} WR={wr:5.1f}% dur={dur:4.1f}s | {status}")
