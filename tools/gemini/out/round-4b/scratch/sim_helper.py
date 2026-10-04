import json, os, re, subprocess, sys, tempfile
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

def simulate_enemy(stage_num, enemy_data, runs=80):
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

if __name__ == "__main__":
    print("Testing Stage 1 baseline...")
    with open(CHAPTERS_PATH, encoding="utf-8") as f:
        doc = json.load(f)
    s1_enemy = doc["chapters"][0]["stages"][0]["enemy"]
    wr, dur = simulate_enemy(1, s1_enemy, runs=80)
    print(f"Stage 1: WR = {wr:.1f}%, Duration = {dur:.1f}s")
