import json

with open("tools/gemini/out/round-3/items-base.json", encoding="utf-8") as f:
    items = {it["id"]: it for it in json.load(f)}

monster_ids = [
    'u_minh_quy_trao', 'heo_rung_bap_giap', 'kich_doc_chu_nga', 'man_xa_doc_tui', 'hac_sat_hung_dam',
    'thien_nien_quy_giap', 'tuyet_lang_bach_bi', 'bang_tam_linh_chau',
    'hoa_diem_xa_cot', 'soi_nanh_canh',
    'bach_ho_cuong_mong', 'bang_phach_co_cot'
]

for mid in monster_ids:
    it = items.get(mid)
    print(f"=== {mid} ({it.get('name')}) ===")
    print(f"  type: {it.get('type')}, size: {it.get('size')}, weaponClass: {it.get('weaponClass')}")
    print(f"  attack: {it.get('attack')}, cd: {it.get('cooldown')}, crit: {it.get('crit')}")
    print(f"  stats: {it.get('stats')}")
    print(f"  basic: {it.get('basic')}")
    print(f"  triggers: {it.get('triggers')}")
    print(f"  ultimate: {it.get('ultimate')}")
