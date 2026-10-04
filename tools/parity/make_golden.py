"""Generate golden battle logs from the Java battle-engine for the C# port's parity tests.

Each case file = {"seed", "a": <loadout>, "b": <loadout>, "expected": <BattleResult as BattleCli --json writes it>}.
Re-run whenever the Java engine, items.json or sects.json change, then re-run the Unity parity tests.

    py tools/parity/make_golden.py [--out One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Data/Parity]
"""
import argparse
import json
import subprocess
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
ENGINE = ROOT / "server" / "battle-engine"
SAMPLES = ENGINE / "samples"

# (tier, tamPhapLevel, pair offset, seed base): a spread of tiers/levels so Đột Phá and Tâm Pháp are covered.
VARIANTS = [(1, 1, 1, 11), (1, 1, 7, 23), (3, 6, 3, 37), (9, 9, 5, 41)]
PER_VARIANT = 8
RARITIES = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]
V2_PAIRS = [
    (["o_tam_y"], ["thiet_cot_quyen_sao"]),
    (["kim_tuyen_y"], ["thanh_phong_kiem"]),
    (["thanh_tam_dan"], ["bang_phach_thuong"]),
    (["nho_sam_y"], ["thanh_phong_kiem"]),
    (["van_khi_dan", "thanh_phong_kiem"], ["phan_quan_but"]),
    (["bach_luyen_chan_kinh", "thiet_cot_quyen_sao"], ["boi_nguyen_dan"]),
    (["giai_doc_dan"], ["that_tinh_tieu"]),
    (["bich_thuy_cam", "tuy_tien_ho_lo"], ["thiet_cot_quyen_sao"]),
    (["tu_kim_trao", "o_tam_y"], ["thiet_cot_quyen_sao"]),
    (["cuu_chuyen_hoan_hon_dan"], ["khai_son_phu"]),
    (["thanh_dong_ho_tam_kinh"], ["bang_phach_thuong"]),
    (["thanh_phong_kiem", "boi_nguyen_dan"], ["thiet_cot_quyen_sao"]),
]


def classpath():
    cp_file = ENGINE / "target" / "cp.txt"
    if not (ENGINE / "target" / "classes").exists() or not cp_file.exists():
        raise SystemExit("Build first: cd server && ./mvnw -q compile (and target/cp.txt must exist, see tools/balance/README.md)")
    return str(ENGINE / "target" / "classes") + ";" + cp_file.read_text().strip()


def with_levels(loadout, tier, tp, case_index, definitions, new_by_size):
    d = json.loads(json.dumps(loadout))
    for j, it in enumerate(d["items"]):
        it["tier"] = tier
        it["rarity"] = RARITIES[(case_index + j) % len(RARITIES)]
        if case_index % 2 == 1:
            size = tuple(definitions[it["id"]].get("size", [1, 1]))
            choices = new_by_size.get(size, [])
            if choices:
                it["id"] = choices[(case_index + j) % len(choices)]["id"]
    d["tamPhapLevel"] = tp
    return d


def v2_loadout(name, ids, tier, definitions):
    x = 0
    items = []
    for item_id in ids:
        items.append({"id": item_id, "x": x, "y": 0, "tier": tier, "rarity": "common"})
        x += definitions[item_id].get("size", [1, 1])[0]
    return {"name": name, "level": 1, "cols": 10, "rows": 4, "tamPhapLevel": 1, "items": items}


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--out", default=str(ROOT / "One-Bag-Jianghu/Assets/_Game/Tests/EditMode/Data/Parity"))
    ap.add_argument("--items", help="alternate item catalog (default remains the bundled items.json)")
    args = ap.parse_args()
    out = Path(args.out)
    out.mkdir(parents=True, exist_ok=True)
    for old in out.glob("case_*.json"):
        old.unlink()
    loadouts = [json.loads(p.read_text(encoding="utf-8")) for p in sorted(SAMPLES.glob("*.json"))]
    items_path = Path(args.items).resolve() if args.items else ENGINE / "src/main/resources/data/items.json"
    item_list = json.loads(items_path.read_text(encoding="utf-8"))
    definitions = {x["id"]: x for x in item_list}
    new_by_size = {}
    for item in item_list[21:]:
        new_by_size.setdefault(tuple(item.get("size", [1, 1])), []).append(item)
    cp = classpath()
    n = 0
    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        work = [(v[0], v[1], v[2], v[3], i) for v in VARIANTS for i in range(PER_VARIANT)] if not args.items else [(9, 1, 0, 83, i) for i in range(len(V2_PAIRS))]
        for tier, tp, offset, seed_base, i in work:
                if args.items:
                    a = v2_loadout("v2-a-" + str(i + 1), V2_PAIRS[i][0], tier, definitions)
                    b = v2_loadout("v2-b-" + str(i + 1), V2_PAIRS[i][1], tier, definitions)
                else:
                    a = with_levels(loadouts[(i * 3) % len(loadouts)], tier, tp, n * 2, definitions, new_by_size)
                    b = with_levels(loadouts[(i * 3 + offset) % len(loadouts)], tier, tp, n * 2 + 1, definitions, new_by_size)
                seed = seed_base * 1000 + i
                pa, pb, res = tmp / "a.json", tmp / "b.json", tmp / "r.json"
                pa.write_text(json.dumps(a, ensure_ascii=False), encoding="utf-8")
                pb.write_text(json.dumps(b, ensure_ascii=False), encoding="utf-8")
                subprocess.run(["java", "-cp", cp, "com.onebagjianghu.battle.cli.BattleCli", str(pa), str(pb),
                                "--seed", str(seed), "--json", str(res), "--items", str(items_path)], cwd=ENGINE, check=True, capture_output=True)
                expected = json.loads(res.read_text(encoding="utf-8"))
                case = {"seed": seed, "a": a, "b": b, "expected": expected}
                n += 1
                (out / f"case_{n:02d}.json").write_text(json.dumps(case, ensure_ascii=False, separators=(",", ":")), encoding="utf-8")
    print(f"wrote {n} cases to {out}")


if __name__ == "__main__":
    main()
