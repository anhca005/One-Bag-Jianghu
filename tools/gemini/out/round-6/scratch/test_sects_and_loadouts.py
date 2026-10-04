import json
import subprocess
import os
import sys
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R6 = ROOT / "tools" / "gemini" / "out" / "round-6"
LOADOUTS_DIR = R6 / "loadouts"

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()
env = os.environ.copy()
env["PYTHONUTF8"] = "1"

# Load original sects
sects = json.load(open(ROOT / "server/battle-engine/src/main/resources/data/sects.json", encoding="utf-8"))

for s in sects:
    if s["id"] == "huyen_doc":
        # Nerf TP6 drain_qi 6 -> 3, TP9 heal 60 -> 25
        for lv in s["levels"]:
            if lv["level"] == 6:
                lv["triggers"] = [{"on": "burst", "from": "self", "status": "poison", "effects": [{"op": "drain_qi", "value": 3}]}]
            if lv["level"] == 9:
                lv["triggers"] = [{"on": "burst", "from": "self", "status": "bleed", "effects": [{"op": "heal", "value": 25}]}]
    elif s["id"] == "thiet_phu":
        # Buff TP6 and TP9: add attackPct 0.08 and 0.08
        for lv in s["levels"]:
            if lv["level"] == 6:
                lv["attackPct"] = 0.08
            if lv["level"] == 9:
                lv["attackPct"] = 0.08
    elif s["id"] == "bich_thuy":
        # TP9: heal 150 -> 60
        for lv in s["levels"]:
            if lv["level"] == 9:
                lv["triggers"] = [{"on": "hp_below", "hpBelow": 0.4, "maxUses": 1, "effects": [{"op": "heal", "value": 60}]}]
    elif s["id"] == "thai_thanh":
        # TP9: ultimate damage 0.3 -> 0.6
        for lv in s["levels"]:
            if lv["level"] == 9:
                lv["ultimate"] = [{"op": "damage", "pct": 0.6}]

sects_test_path = R6 / "scratch" / "test_sects.json"
with open(sects_test_path, "w", encoding="utf-8") as f:
    json.dump(sects, f, ensure_ascii=False, indent=2)

cmd = [
    "py", "tools/balance/balance.py", "matrix",
    "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", "50", "--level", "10",
    "--loadouts", str(LOADOUTS_DIR),
    "--sects", str(sects_test_path)
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
for line in res.stdout.splitlines():
    if "Outside [" in line:
        print("RESULT with tuned sects:", line)

for line in res.stdout.splitlines():
    if "%" in line and not line.startswith("Loadout") and not line.startswith("-") and not line.startswith("Outside"):
        print(line[:85])
