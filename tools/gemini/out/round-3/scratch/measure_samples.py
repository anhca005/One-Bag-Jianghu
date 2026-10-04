import json, subprocess, tempfile, copy, sys
sys.stdout.reconfigure(encoding="utf-8")
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
sys.path.insert(0, str(ROOT / "tools/balance"))
import balance
cp = balance.ensure_classpath()

LOADOUTS_DIR = ROOT / "tools/gemini/out/round-3/loadouts"
ITEMS_BASE = ROOT / "tools/gemini/out/round-3/items-base.json"
SECTS = ROOT / "server/battle-engine/src/main/resources/data/sects.json"
CONFIG = ROOT / "tools/gemini/out/round-3/config.json"

source = balance.load_source_loadouts(LOADOUTS_DIR)

benchmarks = [
    ("Level 1, T1 Phàm", 1, 1, "common", 1),
    ("Level 10, T3 Lương", 10, 3, "uncommon", 3),
    ("Level 30, T9 Chí Tôn", 30, 9, "supreme", 9)
]

for title, level, tier, rarity, tp in benchmarks:
    print(f"\n==================== {title} ====================")
    with tempfile.TemporaryDirectory(prefix="onebag-bench-") as raw_temp:
        temp = Path(raw_temp)
        # Create loadout variants
        variants = []
        for filename, orig in source:
            ld = copy.deepcopy(orig)
            ld["level"] = level
            ld["tamPhapLevel"] = tp
            for it in ld["items"]:
                it["tier"] = tier
                it["rarity"] = rarity
            p = temp / filename
            balance.write_json(p, ld)
            variants.append((p, str(orig.get("name") or Path(filename).stem)))

        # 1. Measure max HP using BattleCli single match breakdown
        hp_list = []
        duration_list = []
        # Run each loadout against itself (mirror fight) to check fight length and max HP
        for p, name in variants:
            cmd = [
                "java", "-cp", cp, "com.onebagjianghu.battle.cli.BattleCli",
                str(p), str(p),
                "--runs", "10",
                "--items", str(ITEMS_BASE),
                "--sects", str(SECTS),
                "--battle-config", str(CONFIG)
            ]
            res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
            # Parse maxHp and duration
            # "thời lượng TB 26.7s"
            # "[A] ... — maxHp 315"
            dur = None
            max_hp = None
            for line in res.stdout.splitlines():
                if "thời lượng TB" in line:
                    parts = line.split("thời lượng TB")
                    dur = float(parts[1].replace("s", "").strip())
                if "— maxHp" in line and max_hp is None:
                    parts = line.split("— maxHp")
                    max_hp = int(parts[1].split()[0].strip())
            
            if max_hp is not None and dur is not None:
                hp_list.append(max_hp)
                duration_list.append(dur)
                print(f"{name:<38}: maxHp={max_hp:4d}  avg_fight_length={dur:4.1f}s")
            else:
                print(f"{name:<38}: ERROR parsing output: {res.stdout[:200]}")

        avg_hp = sum(hp_list) / len(hp_list) if hp_list else 0
        min_hp = min(hp_list) if hp_list else 0
        max_hp_val = max(hp_list) if hp_list else 0
        avg_dur = sum(duration_list) / len(duration_list) if duration_list else 0
        min_dur = min(duration_list) if duration_list else 0
        max_dur = max(duration_list) if duration_list else 0
        print("-" * 65)
        print(f"Summary for {title}:")
        print(f"  Max HP range: {min_hp} - {max_hp_val} (Average: {avg_hp:.1f})")
        print(f"  Fight length range: {min_dur:.1f}s - {max_dur:.1f}s (Average: {avg_dur:.1f}s)")
