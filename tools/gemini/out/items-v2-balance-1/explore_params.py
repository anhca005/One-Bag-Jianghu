import json
import copy
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
sys.path.insert(0, str(ROOT))
from tools.balance.balance import ensure_classpath, load_patch, patched_files, evaluate_matrix, load_battle_config, read_json

sys.stdout.reconfigure(encoding="utf-8")
cp = ensure_classpath()
loadouts_dir = ROOT / "tools/gemini/out/items-v2-balance-1/loadouts"
items_path = ROOT / "tools/gemini/out/items-v2/items-v2.normalized.json"
sects_path = ROOT / "server/battle-engine/src/main/resources/data/sects.json"

def run_eval(patch_dict, runs=50, tiers=[1, 3, 6, 9], tp=[1, 9]):
    pfile = ROOT / "tools/gemini/out/items-v2-balance-1/temp_patch.json"
    pfile.write_text(json.dumps(patch_dict, ensure_ascii=False, indent=2), encoding="utf-8")
    patch = load_patch(pfile)
    base_config = load_battle_config(cp, None)
    
    import tempfile
    with tempfile.TemporaryDirectory(prefix="onebag-test-") as raw_temp:
        t_items, t_sects, t_config = patched_files(Path(raw_temp), items_path, sects_path, base_config, patch)
        labels, variants, rows = evaluate_matrix(
            cp, loadouts_dir, tiers, tp, runs, t_items, t_sects, parallel=True, battle_config=t_config
        )
    outside = sum(1 for row in rows for v in row if v < 35 or v > 65)
    total = len(labels) * len(variants)
    avg_per_build = [sum(row)/len(row) for row in rows]
    extremes = [(labels[i], avg_per_build[i]) for i in range(len(labels)) if avg_per_build[i] < 25 or avg_per_build[i] > 75]
    return outside, total, extremes, labels, variants, rows

if __name__ == "__main__":
    # Test varying maxDmgReducePermille
    for dr in [450, 500, 550, 600]:
        p = {
            "config:maxDmgReducePermille": dr,
            # Bich Thuy
            "bich_thuy_cam.triggers.charge": 0.25,
            "bich_thuy_cam.attack": 55,
            "thanh_dang_tien.attack": 38,
            # Han Son
            "han_ngoc_tieu.cooldown": 4.5,
            "han_ngoc_tieu.attack": 44,
            "truy_hon_cung.cooldown": 4.4,
            "truy_hon_cung.attack": 90,
            "bang_phach_thuong.cooldown": 4.8,
            "bang_phach_thuong.attack": 120,
            # Thiet Phu
            "khai_son_phu.cooldown": 5.5,
            "khai_son_phu.attack": 85,
            "chan_nhac_chuy.cooldown": 5.2,
            "chan_nhac_chuy.attack": 75,
            # Thai Thanh
            "thanh_phong_kiem.attack": 34,
            "thanh_phong_kiem.cooldown": 4.0,
            "ngoc_cot_phien.attack": 24,
            "kiem_tong_tam_yeu.triggers.charge": 0.5,
            "bach_ngoc_boi.triggers.shield": 25,
            # Huyen Doc
            "that_tinh_tieu.basic.poison": 4,
            "that_tinh_tieu.attack": 5,
            "tu_kim_trao.attack": 8,
            # Kim Cang
            "phuc_ho_con.attack": 42,
            "thiet_cot_quyen_sao.attack": 8.5,
            "bach_luyen_chan_kinh.triggers.add_attack": 5,
            # Liet Diem
            "liet_diem_dao.attack": 68,
        }
        outside, total, extremes, labels, variants, rows = run_eval(p, runs=40)
        print(f"dr={dr}: Outside={outside}/{total} ({outside/total*100:.1f}%), Extremes={len(extremes)}")
        for name, avg in extremes:
            print(f"   {name:38} {avg:.1f}%")
