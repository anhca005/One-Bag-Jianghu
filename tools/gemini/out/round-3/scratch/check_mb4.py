import subprocess, tempfile, copy, sys
sys.stdout.reconfigure(encoding="utf-8")
from pathlib import Path
ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
sys.path.insert(0, str(ROOT / "tools/balance"))
import balance
cp = balance.ensure_classpath()

source = balance.load_source_loadouts(ROOT / "tools/gemini/out/round-3/loadouts")
mb4_orig = next(d for fn, d in source if "mb4" in fn)

with tempfile.TemporaryDirectory() as temp:
    t = Path(temp)
    mb4_p = t / "mb4.json"
    ld = copy.deepcopy(mb4_orig)
    ld["tamPhapLevel"] = 1
    for it in ld["items"]:
        it["tier"] = 1
        it["rarity"] = "common"
    balance.write_json(mb4_p, ld)

    for fn, orig in source:
        other_p = t / fn
        ld2 = copy.deepcopy(orig)
        ld2["tamPhapLevel"] = 1
        for it in ld2["items"]:
            it["tier"] = 1
            it["rarity"] = "common"
        balance.write_json(other_p, ld2)
        wr = balance.cli_match(cp, mb4_p, other_p, 20, ROOT / "tools/gemini/out/round-3/items-base.json", ROOT / "server/battle-engine/src/main/resources/data/sects.json", ROOT / "tools/gemini/out/round-3/config.json")
        name = ld2.get("name", fn)
        print(f"mb4 vs {name:<35}: {wr:5.1f}%")
