import json
import re
import os
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
DATA_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"

# Known Kim Dung / Reference Game keywords to search for
KIM_DUNG_TERMS = [
    "độc cô", "độc cô cầu bại", "quách tĩnh", "hoàng dung", "dương quá", "tiểu long nữ",
    "trương vô kỵ", "triệu mẫn", "chu chỉ nhược", "lệnh hồ xung", "nhậm doanh doanh",
    "đông phương bất bại", "nhạc bất quần", "tiêu phong", "kiều phong", "đoàn dự", "hư trúc",
    "hàng long", "hàng long thập bát", "đả cẩu bổng", "cửu âm chân kinh", "cửu dương chân kinh",
    "cửu âm", "cửu dương", "độc cô cửu kiếm", "lục mạch thần kiếm", "càn khôn đại na di",
    "quỳ hoa bảo điển", "tịch tà kiếm phổ", "thái cực quyền", "thái cực kiếm", "dịch cân kinh",
    "thiếu lâm", "võ đang", "nga mi", "hoa sơn", "không đồng", "côn lôn", "tiêu dao", "cái bang",
    "minh giáo", "nhật nguyệt thần giáo", "toàn chân", "cổ mộ", "đào hoa đảo", "tinh túc"
]

print("=== 1. AUDITING CHAPTERS.JSON ===")
chapters_path = DATA_DIR / "Pve" / "chapters.json"
chapters_data = json.load(open(chapters_path, encoding="utf-8"))

characters_list = chapters_data.get("characters", [])
char_map = {}
if isinstance(characters_list, list):
    for cinfo in characters_list:
        cid = cinfo.get("id")
        cname = cinfo.get("name", "")
        ctitle = cinfo.get("title", "")
        char_map[cid] = cinfo
        print(f"Char: {cid} -> {cname} ({ctitle})")
        for kd in KIM_DUNG_TERMS:
            if kd in cname.lower() or kd in ctitle.lower():
                print(f"  [KIM DUNG RISK] Character '{cid}' matched '{kd}' in name='{cname}' or title='{ctitle}'")
elif isinstance(characters_list, dict):
    for cid, cinfo in characters_list.items():
        cname = cinfo.get("name", "")
        ctitle = cinfo.get("title", "")
        char_map[cid] = cinfo
        print(f"Char: {cid} -> {cname} ({ctitle})")
        for kd in KIM_DUNG_TERMS:
            if kd in cname.lower() or kd in ctitle.lower():
                print(f"  [KIM DUNG RISK] Character '{cid}' matched '{kd}' in name='{cname}' or title='{ctitle}'")

for ch in chapters_data.get("chapters", []):
    chid = ch.get("chapterId")
    chname = ch.get("name", "")
    chdesc = ch.get("description", "")
    print(f"\nChapter {chid}: {chname}")
    print(f"  Desc: {chdesc}")
    for st in ch.get("stages", []):
        sid = st.get("stageId")
        stype = st.get("type")
        sname = st.get("name", "")
        sdesc = st.get("description", "")
        ename = st.get("enemy", {}).get("name", "")
        
        # Check Kim Dung
        for kd in KIM_DUNG_TERMS:
            if kd in sname.lower() or kd in sdesc.lower() or kd in ename.lower():
                print(f"  [KIM DUNG RISK] Stage {sid} '{sname}' / Enemy '{ename}' matched '{kd}'")
        
        # Check Length
        if len(sname) > 18:
            print(f"  [Length > 18] Stage {sid} name: '{sname}' ({len(sname)})")
        if len(ename) > 18:
            print(f"  [Length > 18] Stage {sid} enemy.name: '{ename}' ({len(ename)})")
        if len(sdesc) > 90:
            print(f"  [Length > 90] Stage {sid} desc: '{sdesc}' ({len(sdesc)})")

        # Check Dialogue speakers
        for d in st.get("introDialogue", []):
            sp = d.get("speaker")
            text = d.get("text", "")
            if sp not in char_map and sp != "player":
                print(f"  [UNKNOWN SPEAKER] Stage {sid} intro speaker: '{sp}'")
        for d in st.get("outroDialogue", []):
            sp = d.get("speaker")
            text = d.get("text", "")
            if sp not in char_map and sp != "player":
                print(f"  [UNKNOWN SPEAKER] Stage {sid} outro speaker: '{sp}'")

print("\n=== 2. AUDITING ITEMS.JSON ===")
items_data = json.load(open(DATA_DIR / "items.json", encoding="utf-8"))
for it in items_data:
    iid = it.get("id")
    iname = it.get("name", "")
    for kd in KIM_DUNG_TERMS:
        if kd in iname.lower():
            print(f"  [KIM DUNG RISK] Item '{iid}' name='{iname}' matched '{kd}'")

print("\n=== 3. AUDITING SECTS.JSON ===")
sects_data = json.load(open(DATA_DIR / "sects.json", encoding="utf-8"))
for s in sects_data:
    sid = s.get("id")
    sname = s.get("name", "")
    tpname = s.get("tamPhap", "")
    for kd in KIM_DUNG_TERMS:
        if kd in sname.lower() or kd in tpname.lower():
            print(f"  [KIM DUNG RISK] Sect '{sid}' name='{sname}' or tamPhap='{tpname}' matched '{kd}'")

print("\n=== 4. AUDITING MATERIALS.JSON ===")
materials_data = json.load(open(DATA_DIR / "materials.json", encoding="utf-8"))
for m in materials_data:
    mid = m.get("id")
    mname = m.get("name", "")
    mdesc = m.get("description", "")
    for kd in KIM_DUNG_TERMS:
        if kd in mname.lower() or kd in mdesc.lower():
            print(f"  [KIM DUNG RISK] Material '{mid}' name='{mname}' matched '{kd}'")
