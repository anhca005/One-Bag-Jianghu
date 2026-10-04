"""Re-run every PvE stage: expected player bag vs. the stage enemy read from chapters.json.

Usage (from the repo root):
  py tools/balance/pve_check.py [--chapters PATH] [--players DIR] [--items PATH]
                                [--battle-config PATH] [--runs 200]

Defaults: the game's chapters.json and items.json, engine default config,
players from tools/gemini/out/round-4b/player/stage_NN.json.
Prints win rate (draws count half), average fight length, both sides' level/tier/rarity,
and checks the #34 bands (with a 2-point margin) plus the round-4b progression rules:
average fight <= 40 s; boss level = recommendedLevel + 1; other enemies within recommendedLevel +-2
and <= 31; enemy top tier within the player's top tier +-1; enemy top rarity from one step below
the player's top rarity up to +1 (normal) or +2 (elite/boss).
"""
import argparse, json, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance  # noqa: E402

RARITY = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")


def band(i):
    return (50, 60) if i % 10 == 0 else (60, 70) if i % 5 == 0 else (80, 90)


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--chapters", default=str(ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json"))
    ap.add_argument("--players", default=str(ROOT / "tools/gemini/out/round-4b/player"))
    ap.add_argument("--items", default=str(ROOT / "server/battle-engine/src/main/resources/data/items.json"))
    ap.add_argument("--sects", default=str(ROOT / "server/battle-engine/src/main/resources/data/sects.json"))
    ap.add_argument("--battle-config", default=None)
    ap.add_argument("--runs", type=int, default=200)
    a = ap.parse_args()
    cp = balance.ensure_classpath()
    doc = json.load(open(a.chapters, encoding="utf-8"))
    stages = [s for c in doc["chapters"] for s in c["stages"]]
    fails = 0
    with tempfile.TemporaryDirectory() as t:
        t = Path(t)
        for i, s in enumerate(stages, 1):
            p = json.load(open(Path(a.players) / f"stage_{i:02d}.json", encoding="utf-8"))
            e = dict(s["enemy"]); e.setdefault("cols", 5); e.setdefault("rows", 4)
            p.setdefault("cols", 5); p.setdefault("rows", 4)
            balance.write_json(t / "p.json", {k: p[k] for k in KEYS if k in p})
            balance.write_json(t / "e.json", {k: e[k] for k in KEYS if k in e})
            cmd = ["java", "-cp", cp, balance.MAIN_CLASS, str(t / "p.json"), str(t / "e.json"),
                   "--runs", str(a.runs), "--items", a.items, "--sects", a.sects]
            if a.battle_config:
                cmd += ["--battle-config", a.battle_config]
            out = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8").stdout
            pc = re.findall(r"([0-9.]+)%", out)
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
            fails += bool(notes)
            er = sorted({x["rarity"] for x in e["items"]}, key=RARITY.index)
            et = sorted({x["tier"] for x in e["items"]})
            pr = sorted({x["rarity"] for x in p["items"]}, key=RARITY.index)
            pt = sorted({x["tier"] for x in p["items"]})
            print(f"{i:02d} {s['kind']:6} {wr:5.1f}% {dur:5.1f}s | P lv{p['level']:2} n{len(p['items'])} T{pt} {pr}"
                  f" | E lv{e['level']:2} n{len(e['items'])} T{et} {er} | rec {rec} | {'OK' if not notes else ', '.join(notes)}")
    print(f"\nStages breaking a rule: {fails}/{len(stages)}")
    return 1 if fails else 0


if __name__ == "__main__":
    sys.exit(main())
