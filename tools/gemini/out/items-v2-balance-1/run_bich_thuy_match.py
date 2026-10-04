import json
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
sys.path.insert(0, str(ROOT))
from tools.balance.balance import ensure_classpath, make_variant

sys.stdout.reconfigure(encoding="utf-8")
cp = ensure_classpath()
loadouts_dir = ROOT / "tools/gemini/out/items-v2-balance-1/loadouts"
items_path = ROOT / "tools/gemini/out/items-v2/items-v2.normalized.json"
sects_path = ROOT / "server/battle-engine/src/main/resources/data/sects.json"

temp_dir = ROOT / "tools/gemini/out/items-v2-balance-1/temp_match"
if temp_dir.exists():
    import shutil
    shutil.rmtree(temp_dir)

source = [(p.name, json.load(open(p, encoding="utf-8"))) for p in [loadouts_dir / "bich_thuy_duong_sinh.json", loadouts_dir / "liet_diem.json"]]
make_variant(source, temp_dir, tier=1, tp=1)

cmd = [
    "java", "-cp", cp, "com.onebagjianghu.battle.cli.BattleCli",
    str(temp_dir / "bich_thuy_duong_sinh.json"), str(temp_dir / "liet_diem.json"),
    "--items", str(items_path), "--sects", str(sects_path), "--seed", "1"
]
p = subprocess.run(cmd, cwd=ROOT / "server/battle-engine", capture_output=True, text=True, encoding="utf-8", errors="replace")
print("Exit code:", p.returncode)
print(p.stdout[:3000])
