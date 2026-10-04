"""Win rate per PvE stage for fixed player bags.

A = starter kit only; B = starter + Tu Mau Song Dao + Boi Nguyen Dan (all common, tier 1), which is what the
user actually played on 2026-10-04 to clear chapter 1; C = the expected bag in --players.
Player level per stage comes from the expected bag files.

Usage (from the repo root):
  py tools/balance/pve_bags.py [--runs 200] [--stages 20] [--chapters PATH] [--players DIR] [--items PATH]
"""
import argparse
import json
import re
import subprocess
import sys
import tempfile
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / "tools" / "balance"))
import balance  # noqa: E402

KEYS = ("name", "sect", "tamPhapLevel", "level", "items", "cols", "rows")


def item(item_id, x, y):
    return {"id": item_id, "x": x, "y": y, "tier": 1, "rarity": "common"}


STARTER = [item("thanh_phong_kiem", 0, 0), item("o_tam_y", 1, 0)]
PLUS_TWO = STARTER + [item("tu_mau_song_dao", 3, 0), item("boi_nguyen_dan", 0, 3)]


def win_rate(player, enemy, args, classpath, tmp):
    balance.write_json(tmp / "p.json", {k: player[k] for k in KEYS if k in player})
    balance.write_json(tmp / "e.json", {k: enemy[k] for k in KEYS if k in enemy})
    command = ["java", "-cp", classpath, balance.MAIN_CLASS, str(tmp / "p.json"), str(tmp / "e.json"),
               "--runs", str(args.runs), "--items", args.items, "--sects", args.sects]
    output = subprocess.run(command, capture_output=True, text=True, encoding="utf-8").stdout
    percents = re.findall(r"([0-9.]+)%", output)
    return float(percents[0]) + float(percents[2]) / 2


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--runs", type=int, default=200)
    parser.add_argument("--stages", type=int, default=20)
    parser.add_argument("--chapters", default=str(ROOT / "One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json"))
    parser.add_argument("--players", default=str(ROOT / "tools/gemini/out/round-4b/player"))
    parser.add_argument("--items", default=str(ROOT / "server/battle-engine/src/main/resources/data/items.json"))
    parser.add_argument("--sects", default=str(ROOT / "server/battle-engine/src/main/resources/data/sects.json"))
    args = parser.parse_args()

    classpath = balance.ensure_classpath()
    chapters = json.load(open(args.chapters, encoding="utf-8"))["chapters"]
    stages = [stage for chapter in chapters for stage in chapter["stages"]]
    print("ải  loại    lvQuái  cấpNC | A: khởi đầu | B: +Song Đao +Bồi Nguyên | C: túi dự kiến (số món)")
    with tempfile.TemporaryDirectory() as folder:
        tmp = Path(folder)
        for index, stage in enumerate(stages[:args.stages], 1):
            expected = json.load(open(Path(args.players) / f"stage_{index:02d}.json", encoding="utf-8"))
            expected.setdefault("cols", 5)
            expected.setdefault("rows", 4)
            enemy = dict(stage["enemy"])
            enemy.setdefault("cols", 5)
            enemy.setdefault("rows", 4)
            base = {"name": "P", "cols": 5, "rows": 4, "level": expected["level"], "sect": None, "tamPhapLevel": 1}
            starter = win_rate(dict(base, items=STARTER), enemy, args, classpath, tmp)
            bought = win_rate(dict(base, items=PLUS_TWO), enemy, args, classpath, tmp)
            planned = win_rate(expected, enemy, args, classpath, tmp)
            print(f"{index:02d}  {stage['kind']:6}  {enemy['level']:3}    {expected['level']:3}  |   {starter:5.1f}%    |"
                  f"        {bought:5.1f}%           |   {planned:5.1f}% ({len(expected['items'])})")


if __name__ == "__main__":
    main()
