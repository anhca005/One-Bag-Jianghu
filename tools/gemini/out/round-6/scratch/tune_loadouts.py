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

def evaluate_matrix(runs=50):
    cmd = [
        "py", "tools/balance/balance.py", "matrix",
        "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", str(runs), "--level", "10",
        "--loadouts", str(LOADOUTS_DIR)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
    outs = -1
    rows = []
    for line in res.stdout.splitlines():
        if "Outside [" in line:
            outs = int(line.split(":")[1].split("/")[0].strip())
        elif line and not line.startswith("Loadout") and not line.startswith("-") and not line.startswith("Outside") and "%" in line:
            parts = line.split()
            name = " ".join([p for p in parts if not p.endswith("%") and not p.endswith("%!")])
            vals = [p for p in parts if p.endswith("%") or p.endswith("%!")]
            rows.append((name, vals))
    return outs, rows, res.stdout

def evaluate_fight_length():
    cmd = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-6/scratch/FightLengthRunner.java",
        "--loadouts", str(LOADOUTS_DIR)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
    return res.stdout

def evaluate_tier_step():
    cmd = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-6/scratch/TierStepRunner.java",
        "--loadouts", str(LOADOUTS_DIR)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
    return res.stdout

if __name__ == "__main__":
    runs = int(sys.argv[1]) if len(sys.argv) > 1 else 50
    print(f"Evaluating loadouts in {LOADOUTS_DIR} with {runs} runs...")
    outs, rows, full_out = evaluate_matrix(runs)
    print(f"\nMatrix A outside [35, 65]: {outs}/192")
    for name, vals in rows:
        # Check if row has extreme outliers
        pcts = [float(v.rstrip("%!")) for v in vals]
        avg = sum(pcts) / len(pcts)
        flag = " *** OUTLIER ***" if avg < 35 or avg > 65 or any(p < 15 or p > 85 for p in pcts) else ""
        print(f"{name:40s} Avg: {avg:5.1f}% | Min: {min(pcts):5.1f}% Max: {max(pcts):5.1f}%{flag}")
