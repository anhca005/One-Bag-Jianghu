import json, sys
if hasattr(sys.stdout, "reconfigure"): sys.stdout.reconfigure(encoding="utf-8", errors="replace")

base = json.load(open("tools/gemini/out/round-3/items-base.json", encoding="utf-8"))
final = json.load(open("tools/gemini/out/round-3/items-v3-final.json", encoding="utf-8"))

b_map = {it["id"]: it for it in base}
f_map = {it["id"]: it for it in final}

for it_id in [
    "that_tinh_tieu", "tu_kim_trao", "u_minh_quy_trao", "kich_doc_chu_nga",
    "khai_son_phu", "chan_nhac_chuy", "bang_phach_thuong", "thiet_cot_quyen_sao",
    "liet_diem_dao", "thanh_phong_kiem", "ngoc_cot_phien", "phuc_ho_con",
    "truy_hon_cung", "phan_quan_but", "han_ngoc_tieu", "bich_thuy_cam",
    "thanh_dang_tien", "xich_sa_chuong", "tu_mau_song_dao", "bach_ho_cuong_mong"
]:
    b = b_map.get(it_id, {})
    f = f_map.get(it_id, {})
    print(f"{it_id:<22} base: atk={b.get('attack')}, cd={b.get('cooldown')} | final: atk={f.get('attack')}, cd={f.get('cooldown')}")
