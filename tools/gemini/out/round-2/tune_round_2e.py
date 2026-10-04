import json, subprocess
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
LOADOUTS = ROOT / "tools" / "gemini" / "out" / "round-2" / "loadouts"
CATALOG = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-base.json"

patch = {
    # Khai Sơn Phủ: 78
    "khai_son_phu.attack": 78,
    "khai_son_phu.cooldown": 4.8,
    "liet_diem_dao.attack": 58,
    "liet_diem_dao.cooldown": 3.3,
    "chan_nhac_chuy.attack": 68,

    # Swords & Fans: balanced so all Thai Thanh builds sit between 33% and 68%
    "thanh_phong_kiem.attack": 35,
    "thanh_phong_kiem.cooldown": 3.8,
    "thanh_phong_kiem.basic.gain_qi": 12,
    "thien_tam_chuoi.triggers.stun": 0.15,
    "ngoc_cot_phien.cooldown": 2.2,
    "ngoc_cot_phien.attack": 20,

    # Buff early low weapons
    "that_tinh_tieu.attack": 9,
    "that_tinh_tieu.cooldown": 2.5,
    "that_tinh_tieu.basic.poison": 2,
    "tu_kim_trao.attack": 13,
    "tu_kim_trao.cooldown": 1.7,
    "xich_sa_chuong.attack": 29,
    "xich_sa_chuong.cooldown": 2.1,

    # Relics / Triggers
    "van_doc_dinh.triggers.drain_qi": 3,
    "u_minh_bi_luc.triggers.heal": 30,
    "thanh_dang_tien.cooldown": 4.0,
    "thanh_dang_tien.attack": 28,

    # Monster items tuning
    "u_minh_quy_trao.attack": 34,
    "u_minh_quy_trao.basic.gain_qi": 8,
    "u_minh_quy_trao.basic.poison": 2,
    "u_minh_quy_trao.ultimate.cost": 25,
    "kich_doc_chu_nga.attack": 26,
    "kich_doc_chu_nga.cooldown": 1.6,
    "kich_doc_chu_nga.basic.gain_qi": 4,
    "kich_doc_chu_nga.basic.poison": 3,
    "bach_ho_cuong_mong.attack": 19,
    "soi_nanh_canh.attack": 24,
    "hoa_diem_xa_cot.triggers.burn": 2,
    "thien_nien_quy_giap.triggers.shield": 280,
    "bang_phach_thuong.attack": 125,
    "bang_phach_thuong.cooldown": 4.6
}

patch_path = ROOT / "tools" / "gemini" / "out" / "round-2" / "test_patch.json"
with open(patch_path, "w", encoding="utf-8") as f:
    json.dump(patch, f, indent=2)

print("Running test Matrix A (50 runs)...")
cmd = [
    "py", "tools/balance/balance.py", "whatif",
    "--loadouts", str(LOADOUTS),
    "--patch", str(patch_path),
    "--items", str(CATALOG),
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "50",
    "--low", "35", "--high", "65"
]
res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
out_path = ROOT / "tools" / "gemini" / "out" / "round-2" / "test_matrix_out.txt"
with open(out_path, "w", encoding="utf-8") as f:
    f.write(res.stdout)
print("Finished.")
