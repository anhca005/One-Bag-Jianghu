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

def run_quick_matrix(runs=40):
    cmd = [
        "py", "tools/balance/balance.py", "matrix",
        "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", str(runs), "--level", "10",
        "--loadouts", str(LOADOUTS_DIR)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
    outs = -1
    for line in res.stdout.splitlines():
        if "Outside [" in line:
            outs = int(line.split(":")[1].split("/")[0].strip())
    return outs, res.stdout

def run_fight_length():
    cmd = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-6/scratch/FightLengthRunner.java",
        "--loadouts", str(LOADOUTS_DIR)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env, cwd=ROOT)
    lines = [l.strip() for l in res.stdout.splitlines() if "Trọng" in l or "Toàn cục" in l]
    return lines

if __name__ == "__main__":
    outs, text = run_quick_matrix()
    print(f"Current Matrix A: {outs}/192 outs")
    fl = run_fight_length()
    print("Fight lengths:")
    for l in fl:
        print("  ", l)
