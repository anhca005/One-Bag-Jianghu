import copy, json, os, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path(__file__).resolve().parents[5]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance

RARITY = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")
ITEMS_PATH = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "items.json"
SECTS_PATH = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"
PLAYERS_DIR = ROOT / "tools" / "gemini" / "out" / "round-4b" / "player"
CHAPTERS_PATH = ROOT / "tools" / "gemini" / "out" / "round-4b" / "chapters.json"

CP = balance.ensure_classpath()
with open(ITEMS_PATH, encoding="utf-8") as f:
    ITEMS_DB = {it["id"]: it for it in json.load(f)}

def band(i):
    return (50, 60) if i % 10 == 0 else (60, 70) if i % 5 == 0 else (80, 90)

def simulate_stage(stage_num, enemy_data, runs=80):
    p_path = PLAYERS_DIR / f"stage_{stage_num:02d}.json"
    p = json.load(open(p_path, encoding="utf-8"))
    e = dict(enemy_data)
    e.setdefault("cols", 5); e.setdefault("rows", 4)
    p.setdefault("cols", 5); p.setdefault("rows", 4)
    with tempfile.TemporaryDirectory() as t:
        t = Path(t)
        balance.write_json(t / "p.json", {k: p[k] for k in KEYS if k in p})
        balance.write_json(t / "e.json", {k: e[k] for k in KEYS if k in e})
        cmd = [
            "java", "-cp", CP, balance.MAIN_CLASS,
            str(t / "p.json"), str(t / "e.json"),
            "--runs", str(runs),
            "--items", str(ITEMS_PATH),
            "--sects", str(SECTS_PATH)
        ]
        res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8")
        out = res.stdout
        pc = re.findall(r"([0-9.]+)%", out)
        if not pc:
            raise RuntimeError(f"Engine output parsing failed: {out}\nSTDERR: {res.stderr}")
        wr = float(pc[0]) + float(pc[2]) / 2
        dur_match = re.search(r"TB ([0-9.]+)s", out)
        dur = float(dur_match.group(1)) if dur_match else 0.0
        return wr, dur

def check_rules(stage_num, stage_kind, p, e, wr, dur):
    lo, hi = band(stage_num)
    notes = []
    # Band rule with 2-point margin
    if not (lo + 2 <= wr <= hi - 2):
        notes.append(f"band {lo+2}-{hi-2} (got {wr:.1f}%)")
    max_dur = 30.0 if stage_num in (1, 2) else 40.0
    if dur > max_dur:
        notes.append(f"dur>{max_dur}s (got {dur:.1f}s)")
    rec = stage_num
    if stage_kind == "boss" and e["level"] != rec + 1:
        notes.append(f"boss!=rec+1 (got lv{e['level']})")
    if stage_kind != "boss" and not (rec - 2 <= e["level"] <= rec + 2):
        notes.append(f"lvl!=rec±2 (got lv{e['level']})")
    if e["level"] > 31:
        notes.append("lvl>31")
    pT = max(x["tier"] for x in p["items"])
    eT = max(x["tier"] for x in e["items"])
    if not (pT - 1 <= eT <= pT + 1):
        notes.append(f"tier!=p±1 (pT={pT}, eT={eT})")
    pR = max(RARITY.index(x["rarity"]) for x in p["items"])
    eR = max(RARITY.index(x["rarity"]) for x in e["items"])
    up = 1 if stage_kind == "normal" else 2
    if not (pR - 1 <= eR <= pR + up):
        notes.append(f"rarity!=p-1..+{up} (pR={RARITY[pR]}, eR={RARITY[eR]})")
    return notes

if __name__ == "__main__":
    print("Sim helper and checker ready.")
