import json

with open("tools/gemini/out/round-2/items-base.json", "r", encoding="utf-8") as f:
    items = {it["id"]: it for it in json.load(f)}

monster_item_ids = [
    "soi_nanh_canh", "heo_rung_bap_giap", "man_xa_doc_tui", "u_minh_quy_trao",
    "kich_doc_chu_nga", "bang_tam_linh_chau", "tuyet_lang_bach_bi", "hac_sat_hung_dam",
    "bach_ho_cuong_mong", "hoa_diem_xa_cot", "bang_phach_co_cot", "thien_nien_quy_giap"
]

print("=== Monster Items stats in trial3 ===")
for mid in monster_item_ids:
    it = items.get(mid)
    if not it:
        print(f"Missing: {mid}")
        continue
    t = it.get("type")
    atk = it.get("attack", 0)
    cd = it.get("cooldown", 0)
    dps = round(atk / cd, 1) if cd > 0 else 0
    print(f"{mid:22} | {t:7} | atk={atk} cd={cd} (dps={dps}) | size={it.get('size')} | rarity={it.get('rarity')}")
