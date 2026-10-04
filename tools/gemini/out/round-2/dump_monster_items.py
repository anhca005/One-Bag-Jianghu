import json

with open("tools/gemini/out/round-2/items-base.json", "r", encoding="utf-8") as f:
    items = {it["id"]: it for it in json.load(f)}

monster_item_ids = [
    "soi_nanh_canh", "heo_rung_bap_giap", "man_xa_doc_tui", "u_minh_quy_trao",
    "kich_doc_chu_nga", "bang_tam_linh_chau", "tuyet_lang_bach_bi", "hac_sat_hung_dam",
    "bach_ho_cuong_mong", "hoa_diem_xa_cot", "bang_phach_co_cot", "thien_nien_quy_giap"
]

with open("tools/gemini/out/round-2/monster_items_dump.json", "w", encoding="utf-8") as out:
    json.dump([items[mid] for mid in monster_item_ids], out, indent=2, ensure_ascii=False)
print("Dumped 12 monster items.")
