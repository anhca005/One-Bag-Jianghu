import json
from pathlib import Path

patch = {
    "config:maxDmgReducePermille": 150,
    "bich_thuy_cam.triggers.charge": 0.3,
    "bich_thuy_cam.attack": 58,
    "bich_thuy_cam.cooldown": 4.8,
    "that_tinh_tieu.basic.poison": 4,
    "that_tinh_tieu.attack": 5,
    "tu_kim_trao.attack": 8,
    "han_ngoc_tieu.cooldown": 4.2,
    "han_ngoc_tieu.attack": 42,
    "truy_hon_cung.cooldown": 4.2,
    "bang_phach_thuong.cooldown": 4.6,
    "phuc_ho_con.attack": 40,
    "thiet_cot_quyen_sao.attack": 8,
    "tu_kim_bat.cooldown": 7.0
}

pfile = Path("tools/gemini/out/items-v2-balance-1/trial_patch.json")
pfile.write_text(json.dumps(patch, ensure_ascii=False, indent=2), encoding="utf-8")
print(f"Wrote {len(patch)} changes to trial_patch.json")
