import argparse
import subprocess
import os
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R6 = ROOT / "tools" / "gemini" / "out" / "round-6"
TABLES = R6 / "tables"
TABLES.mkdir(parents=True, exist_ok=True)

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()

env = os.environ.copy()
env["PYTHONUTF8"] = "1"

def run_logged(cmd, outfile):
    print(f"Executing: {' '.join(cmd)}")
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", env=env)
    with open(outfile, "w", encoding="utf-8") as f:
        f.write(res.stdout)
    if res.stderr:
        print(f"Stderr: {res.stderr[:300]}")
    print(f"Saved: {outfile}")
    return res.stdout

def count_100_cells(rarity_output):
    count = 0
    lines = rarity_output.splitlines()
    for line in lines:
        if not line or line.startswith("Loadout") or line.startswith("-") or line.startswith("Average") or line.startswith("("):
            continue
        parts = line.split()
        tokens = [p for p in parts if p.endswith("%")]
        if len(tokens) >= 5:
            for t in tokens[:5]:
                if t == "100.0%":
                    count += 1
    return count

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--mode", required=True, choices=["baseline", "step1_hp", "step2_loadouts", "final"])
    parser.add_argument("--loadouts", default=str(ROOT / "server" / "battle-engine" / "samples"))
    parser.add_argument("--items", default=None)
    parser.add_argument("--sects", default=None)
    parser.add_argument("--battle-config", default=None)
    parser.add_argument("--chapters", default=str(ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data" / "Pve" / "chapters.json"))
    parser.add_argument("--players", default=str(R6 / "player"))
    args = parser.parse_args()

    prefix = f"{args.mode}_"
    config_opt = ["--battle-config", args.battle_config] if args.battle_config else []
    items_opt = ["--items", args.items] if args.items else []
    sects_opt = ["--sects", args.sects] if args.sects else []
    loadouts_opt = ["--loadouts", args.loadouts]

    # 1. Matrix A
    print(f"\n--- 1. Matrix A ({args.mode}) ---")
    cmd_matrix = [
        "py", "tools/balance/balance.py", "matrix",
        "--tiers", "1,3,6,9",
        "--tp", "1,9",
        "--runs", "100",
        "--level", "10"
    ] + loadouts_opt + items_opt + sects_opt + config_opt
    out_matrix = run_logged(cmd_matrix, TABLES / f"{prefix}matrix_a.txt")

    # 2. Rarity Step
    print(f"\n--- 2. Rarity Step ({args.mode}) ---")
    cmd_rarity = [
        "py", "tools/balance/balance.py", "rarity-step",
        "--tier", "3",
        "--tp", "1",
        "--level", "10",
        "--runs", "100"
    ] + loadouts_opt + items_opt + sects_opt + config_opt
    out_rarity = run_logged(cmd_rarity, TABLES / f"{prefix}rarity_step.txt")
    c100 = count_100_cells(out_rarity)
    with open(TABLES / f"{prefix}rarity_step.txt", "a", encoding="utf-8") as f:
        f.write(f"\nNumber of 100% cells: {c100}\n")
    print(f"Number of 100% cells in {args.mode}: {c100}")

    # 3. Tier Step
    print(f"\n--- 3. Tier Step ({args.mode}) ---")
    cmd_tier = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-6/scratch/TierStepRunner.java"
    ] + loadouts_opt + items_opt + sects_opt + config_opt
    run_logged(cmd_tier, TABLES / f"{prefix}tier_step.txt")

    # 4. Fight Length
    print(f"\n--- 4. Fight Length ({args.mode}) ---")
    cmd_fl = [
        "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
        "tools/gemini/out/round-6/scratch/FightLengthRunner.java"
    ] + loadouts_opt + items_opt + sects_opt + config_opt
    run_logged(cmd_fl, TABLES / f"{prefix}fight_length.txt")

    # 5. PvE Check
    print(f"\n--- 5. PvE Check ({args.mode}) ---")
    cmd_pve = [
        "py", "tools/balance/pve_check.py",
        "--chapters", args.chapters,
        "--players", args.players,
        "--runs", "200"
    ] + items_opt + sects_opt + config_opt
    run_logged(cmd_pve, TABLES / f"{prefix}pve_check.txt")

    print(f"\nAll 5 benchmarks completed for mode: {args.mode}!")

if __name__ == "__main__":
    main()
