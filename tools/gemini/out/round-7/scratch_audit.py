import json, subprocess, sys, re, tempfile
from pathlib import Path

ROOT = Path(r"d:\CONG VIEC\One-Bag-Jianghu")
sys.path.insert(0, str(ROOT / "tools/balance"))
import balance

ITEMS = json.loads((ROOT/"server/battle-engine/src/main/resources/data/items.json").read_text("utf-8"))
CHAP  = json.loads((ROOT/"One-Bag-Jianghu/Assets/_Game/Resources/Data/Pve/chapters.json").read_text("utf-8"))
STAGES_ALL = [s for ch in CHAP["chapters"] for s in ch["stages"]]
MONSTER_IDS = set(CHAP.get("monsterItems",[]))
WEAPONS = [it for it in ITEMS if it.get("type") == "weapon" and it["id"] not in MONSTER_IDS]
TARGET_STAGES = [2, 5, 9, 10, 12]
PLAYER_LEVELS = {2:2, 5:6, 9:10, 10:10, 12:12}
ITEMS_PATH = str(ROOT/"server/battle-engine/src/main/resources/data/items.json")
SECTS_PATH = str(ROOT/"server/battle-engine/src/main/resources/data/sects.json")
cp = balance.ensure_classpath()
KEYS = ("name","sect","tamPhapLevel","level","items","cols","rows")

def run_vs(player_bag, enemy_dict, runs=200):
    with tempfile.TemporaryDirectory() as t:
        tp = Path(t)
        balance.write_json(tp/"p.json", {k:player_bag[k] for k in KEYS if k in player_bag})
        e2 = dict(enemy_dict); e2.setdefault("cols",5); e2.setdefault("rows",4)
        balance.write_json(tp/"e.json", {k:e2[k] for k in KEYS if k in e2})
        out = subprocess.run(
            ["java","-cp",cp,balance.MAIN_CLASS,str(tp/"p.json"),str(tp/"e.json"),
             "--runs",str(runs),"--items",ITEMS_PATH,"--sects",SECTS_PATH],
            capture_output=True, text=True, encoding="utf-8").stdout
        m = re.search(r"A th[aắ]ng\s+([0-9.]+)%.*?h[oòa]+\s+([0-9.]+)%", out)
        if m:
            return float(m.group(1)) + float(m.group(2))/2
        pcts = re.findall(r"([0-9]+\.[0-9]+)%", out)
        return float(pcts[0]) + float(pcts[2])/2 if len(pcts)>=3 else 0.0

results = {}
for wep in WEAPONS:
    wid = wep["id"]
    results[wid] = {}
    for sidx in TARGET_STAGES:
        enemy = dict(STAGES_ALL[sidx-1]["enemy"])
        enemy.setdefault("cols",5); enemy.setdefault("rows",4)
        plv = PLAYER_LEVELS[sidx]
        items_in_bag = [
            {"id":"thanh_phong_kiem","x":0,"y":0,"tier":1,"rarity":"common"},
            {"id":"o_tam_y","x":1,"y":0,"tier":1,"rarity":"common"},
            {"id":"boi_nguyen_dan","x":2,"y":0,"tier":1,"rarity":"common"},
        ]
        if wid not in ("thanh_phong_kiem","o_tam_y","boi_nguyen_dan"):
            items_in_bag.append({"id":wid,"x":3,"y":0,"tier":1,"rarity":"common"})
        bag = {"name":f"audit","cols":5,"rows":4,"level":plv,"sect":None,"tamPhapLevel":1,"items":items_in_bag}
        wr = run_vs(bag, enemy, runs=200)
        results[wid][sidx] = wr
        print(f"  {wid:28s} s{sidx}: {wr:5.1f}%", flush=True)

print("\n=== WEAPON AUDIT (starter + Boi Nguyen Dan + each weapon, T1 common) ===")
print(f"{'Weapon':30s}{'s2':>7}{'s5':>7}{'s9':>7}{'s10':>7}{'s12':>7}{'AVG':>7}")
print("-"*75)
all_avgs = {}
for wid in sorted(results):
    wm = results[wid]
    vals = [wm[s] for s in TARGET_STAGES if s in wm]
    avg = sum(vals)/len(vals) if vals else 0
    all_avgs[wid] = avg
    row = f"{wid:30s}"
    for s in TARGET_STAGES: row += f"{wm.get(s,0):7.1f}"
    row += f"{avg:7.1f}"
    print(row)

avgs = sorted(all_avgs.values())
median = avgs[len(avgs)//2]
print(f"\nMedian avg: {median:.1f}%  Outlier threshold: {median+15:.1f}%")
for wid,avg in sorted(all_avgs.items(), key=lambda x:-x[1]):
    if avg > median+15:
        print(f"  OUTLIER: {wid}: {avg:.1f}% (+{avg-median:.1f} above median)")

out_path = Path(r"d:\CONG VIEC\One-Bag-Jianghu\tools\gemini\out\round-7\tables\weapon_audit_raw.json")
out_path.write_text(json.dumps({"results":results,"all_avgs":all_avgs,"median":median},indent=2))
print(f"\nRaw results saved to {out_path}")
