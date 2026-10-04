import json
import subprocess
import shutil
import os
import sys
import copy
import tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu").resolve()
R3 = ROOT / "tools/gemini/out/round-3"
BAL_DIR = R3 / "balance"
P_DIR = BAL_DIR / "player"
E_DIR = BAL_DIR / "enemy"

ITEMS_PATH = (R3 / "items-v3-final.json").resolve()
SECTS = (ROOT / "server/battle-engine/src/main/resources/data/sects.json").resolve()
CONFIG = (R3 / "config.json").resolve()

sys.path.insert(0, str((ROOT / "tools/balance").resolve()))
import balance
classpath = balance.ensure_classpath()

items_data = json.load(open(ITEMS_PATH, "r", encoding="utf-8"))
item_sizes = {it["id"]: it["size"] for it in items_data}

def eval_pair(p_data, e_data, runs=80):
    with tempfile.TemporaryDirectory(prefix="pve_") as raw_temp:
        temp = Path(raw_temp)
        fa = temp / "p.json"
        fb = temp / "e.json"
        clean_p = {k: v for k, v in p_data.items() if k in ('items', 'cols', 'level', 'rows', 'name', 'tamPhapLevel', 'sect')}
        clean_e = {k: v for k, v in e_data.items() if k in ('items', 'cols', 'level', 'rows', 'name', 'tamPhapLevel', 'sect')}
        balance.write_json(fa, clean_p, indent=None)
        balance.write_json(fb, clean_e, indent=None)
        return balance.cli_match(classpath, fa, fb, runs, ITEMS_PATH, SECTS, CONFIG)

# Inspect battle events of 1 run
cmd = [
    "java", "-cp", classpath, "com.onebagjianghu.battle.cli.BattleCli",
    str(P_DIR / "stage_01.json"), str(E_DIR / "stage_01.json"),
    "--runs", "1", "--verbose",
    "--items", str(ITEMS_PATH), "--sects", str(SECTS), "--battle-config", str(CONFIG)
]
out = balance.run_checked(cmd, balance.ENGINE).stdout
with open(R3 / "scratch/stage1_log.txt", "w", encoding="utf-8") as f:
    f.write(out)
print("Saved stage1_log.txt")

