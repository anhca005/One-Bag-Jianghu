import copy, json, os, re, subprocess, sys, tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules, band, RARITY, KEYS, ROOT, ITEMS_DB

CHAPTERS_PATH = ROOT / "tools" / "gemini" / "out" / "round-4b" / "chapters.json"
PLAYERS_DIR = ROOT / "tools" / "gemini" / "out" / "round-4b" / "player"

def get_stage_info(stage_num):
    doc = json.load(open(CHAPTERS_PATH, encoding="utf-8"))
    stages = [s for c in doc["chapters"] for s in c["stages"]]
    s = stages[stage_num - 1]
    p = json.load(open(PLAYERS_DIR / f"stage_{stage_num:02d}.json", encoding="utf-8"))
    return doc, s, p

def check_current_stage(stage_num, runs=200):
    doc, s, p = get_stage_info(stage_num)
    e = s["enemy"]
    wr, dur = simulate_stage(stage_num, e, runs=runs)
    notes = check_rules(stage_num, s["kind"], p, e, wr, dur)
    print(f"Stage {stage_num:02d} [{s['kind']:6}] (runs={runs}): WR={wr:5.1f}%, dur={dur:4.1f}s | notes: {notes}")
    return wr, dur, notes

if __name__ == "__main__":
    print("Checking Hồi 1 stages (1-10)...")
    for i in range(1, 11):
        check_current_stage(i, runs=80)
