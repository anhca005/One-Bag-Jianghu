import json
import subprocess
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

def test_patch(patch_dict, tiers=[1, 3, 6, 9], tp=[1, 9], runs=60):
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
    
    print(f"Outside [35, 65]: {outside}/{total} ({outside/total*100:.1f}%) | Extremes (<25 or >75): {len(extremes)}")
    for i, label in enumerate(labels):
        flag = " ***" if avg_per_build[i] < 25 or avg_per_build[i] > 75 else ""
        print(f"   {label:40} avg={avg_per_build[i]:5.1f}% min={min(rows[i]):5.1f}% max={max(rows[i]):5.1f}%{flag}")
    return outside, extremes, labels, rows

if __name__ == "__main__":
    p = {
        "config:maxDmgReducePermille": 400,
        
        # Bich Thuy nerf
        "bich_thuy_cam.triggers.charge": 0.2,
        "bich_thuy_cam.attack": 52,
        "bich_thuy_cam.cooldown": 4.6,
        "thanh_dang_tien.attack": 36,
        "thanh_dang_tien.cooldown": 3.6,
        "duong_sinh_quyet.triggers.shield": 8,
        "tue_nguyet_bao_y.triggers.shield": 10,
        "tram_huong_lo.cooldown": 6.5,
        
        # Thiet Phu smoothed
        "khai_son_phu.attack": 88,
        "khai_son_phu.cooldown": 5.0,
        "chan_nhac_chuy.attack": 82,
        "chan_nhac_chuy.cooldown": 4.8,
        "o_thiet_giap.triggers.shield": 300,

        
        # Thai Thanh smoothed
        "thanh_phong_kiem.attack": 36,
        "thanh_phong_kiem.cooldown": 3.8,
        "ngoc_cot_phien.attack": 25,
        "kiem_tong_tam_yeu.triggers.charge": 0.5,
        "bach_ngoc_boi.triggers.shield": 25,
        "bach_ngoc_boi.cooldown": 5.5,
        
        # Han Son smoothed
        "han_ngoc_tieu.attack": 42,
        "han_ngoc_tieu.cooldown": 4.4,
        "phan_quan_but.attack": 45,
        "truy_hon_cung.attack": 86,
        "truy_hon_cung.cooldown": 4.4,
        "bang_phach_thuong.attack": 115,
        "bang_phach_thuong.cooldown": 5.0,
        
        # Huyen Doc smoothed
        "that_tinh_tieu.basic.poison": 3,
        "that_tinh_tieu.attack": 4,
        "tu_kim_trao.attack": 7,
        
        # Kim Cang smoothed
        "phuc_ho_con.attack": 44,
        "thiet_cot_quyen_sao.attack": 8,
        "tu_kim_bat.cooldown": 6.0,
        "bach_luyen_chan_kinh.triggers.add_attack": 6,
        
        # Liet Diem smoothed
        "liet_diem_dao.attack": 68,
        "xich_sa_chuong.attack": 24,
        "xich_sa_chuong.cooldown": 2.3,
        "tu_mau_song_dao.attack": 28,
        "tu_mau_song_dao.cooldown": 2.1,
        "chu_sa_ho_lo.cooldown": 4.2
    }
    test_patch(p, runs=60)
