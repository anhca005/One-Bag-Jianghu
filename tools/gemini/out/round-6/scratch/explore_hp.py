import json
import subprocess
import os
import sys
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

classpath = balance.ensure_classpath()
env = os.environ.copy()
env["PYTHONUTF8"] = "1"

candidates = [
    (100, 5),   # default
    (150, 5),
    (200, 5),
    (250, 5),
    (300, 5),
    (150, 10),
    (200, 10),
    (250, 10),
    (100, 15),
    (150, 15),
    (200, 15),
    (300, 15),
    (200, 20),
]

def eval_hp(base_hp, hp_per_level):
    cfg = {"baseHp": base_hp, "hpPerLevel": hp_per_level}
    cfg_path = ROOT / "tools" / "gemini" / "out" / "round-6" / "scratch" / "temp_hp_config.json"
    with open(cfg_path, "w", encoding="utf-8") as f:
        json.dump(cfg, f)

    # 1. Run Matrix A
    cmd_mat = [
        "py", "tools/balance/balance.py", "matrix",
        "--tiers", "1,3,6,9", "--tp", "1,9", "--runs", "50", "--level", "10",
        "--battle-config", str(cfg_path)
    ]
    res_mat = subprocess.run(cmd_mat, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    outs = -1
    for line in res_mat.stdout.splitlines():
        if "Outside [" in line:
            outs = int(line.split(":")[1].split("/")[0].strip())

    # 2. Run Fight Length
    cmd_fl = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-6/scratch/FightLengthRunner.java",
        "--battle-config", str(cfg_path)
    ]
    res_fl = subprocess.run(cmd_fl, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    lines_fl = [l.strip() for l in res_fl.stdout.splitlines() if "Trọng" in l or "Toàn cục" in l]

    print(f"baseHp={base_hp:3d}, hpPerLevel={hp_per_level:2d} -> Matrix A outs={outs}/192")
    for l in lines_fl:
        print("   ", l)
    return outs

if __name__ == "__main__":
    print("Testing HP variations...")
    results = []
    for bhp, hpl in candidates:
        outs = eval_hp(bhp, hpl)
        results.append((outs, bhp, hpl))
    results.sort()
    print("\nSorted results:")
    for outs, bhp, hpl in results:
        print(f"Outs: {outs:3d}/192 | baseHp={bhp}, hpPerLevel={hpl}")
