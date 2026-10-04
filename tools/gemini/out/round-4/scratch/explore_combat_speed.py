import json
import subprocess
import os
import sys
import copy
import tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
LOADOUTS = R4 / "loadouts"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()

items_base = json.load(open(balance.DEFAULT_ITEMS, encoding="utf-8"))

# Test multipliers on weapon attack: 1.25, 1.35, 1.5
for factor in [1.25, 1.35, 1.5]:
    test_items = copy.deepcopy(items_base)
    for it in test_items:
        if it.get("type") == "weapon" and "attack" in it:
            it["attack"] = round(it["attack"] * factor)

    with tempfile.TemporaryDirectory(prefix="spd_") as raw_temp:
        temp = Path(raw_temp)
        f_items = temp / "items.json"
        balance.write_json(f_items, test_items, indent=None)

        source = balance.load_source_loadouts(LOADOUTS)
        durations = []
        over_45 = 0
        total = 0

        # Measure across T1 (Lv1, common) and T6 (Lv18, rare)
        for tc in [{"t": 1, "lvl": 1, "r": "common"}, {"t": 6, "lvl": 18, "r": "rare"}]:
            for s_idx, (fname, orig) in enumerate(source):
                ld = copy.deepcopy(orig)
                ld["level"] = tc["lvl"]
                ld["tamPhapLevel"] = 1
                for it in ld["items"]:
                    it["tier"] = tc["t"]
                    it["rarity"] = tc["r"]
                p = temp / f"m_{s_idx}.json"
                balance.write_json(p, ld, indent=None)

                cmd = ["java", "-cp", classpath, "com.onebagjianghu.battle.cli.BattleCli", str(p), str(p), "--runs", "10", "--items", str(f_items)]
                res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
                for line in res.stdout.splitlines():
                    if "thời lượng TB" in line:
                        d = float(line.split("thời lượng TB")[1].strip().replace("s", ""))
                        durations.append(d)
                        total += 1
                        if d > 45.0:
                            over_45 += 1

        avg_d = sum(durations) / len(durations) if durations else 0
        pct_45 = (over_45 / total * 100.0) if total else 0
        print(f"Factor {factor:.2f}: avg_duration = {avg_d:.1f}s, >45s = {pct_45:.1f}% ({over_45}/{total})")
