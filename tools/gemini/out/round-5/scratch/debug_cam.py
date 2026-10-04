import sys
from pathlib import Path
import subprocess

sys.stdout.reconfigure(encoding="utf-8")
ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance

classpath = balance.ensure_classpath()

cmd = [
    "java", "-Dfile.encoding=UTF-8", "-cp", classpath,
    "com.onebagjianghu.battle.cli.BattleCli",
    "server/battle-engine/samples/bich_thuy_duong_sinh.json",
    "server/battle-engine/samples/kim_cang.json",
    "--runs", "1",
    "--seed", "1",
    "-v"
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8")
print(res.stdout[:2000])
