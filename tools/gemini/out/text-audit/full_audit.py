import json
import re
import os
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
ASSETS_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game"
DATA_DIR = ASSETS_DIR / "Resources" / "Data"
SCRIPTS_DIR = ASSETS_DIR / "Scripts"
EDITOR_DIR = ASSETS_DIR / "Editor" / "Building"

findings = {
    "raw_id": [],
    "terminology": [],
    "typo": [],
    "length": [],
    "naming": [],
    "story": []
}

# -------------------------------------------------------------
# 1. AUDIT DATA FILES
# -------------------------------------------------------------

# A. materials.json
materials_path = DATA_DIR / "materials.json"
materials_data = json.load(open(materials_path, encoding="utf-8"))
for idx, m in enumerate(materials_data):
    mid = m.get("id")
    name = m.get("name", "")
    desc = m.get("description", "")
    
    if len(name) > 18:
        findings["length"].append({
            "source": f"materials.json:materials[{idx}].name",
            "text": name,
            "len": len(name),
            "limit": 18,
            "type": "name"
        })
    if len(desc) > 90:
        findings["length"].append({
            "source": f"materials.json:materials[{idx}].description",
            "text": desc,
            "len": len(desc),
            "limit": 90,
            "type": "desc"
        })
    # check typo / punctuation
    if "..." in desc:
        findings["typo"].append({
            "source": f"materials.json:materials[{idx}].description",
            "issue": "Dùng '...' thay vì dấu ba chấm chuẩn '…'",
            "text": desc
        })

# B. items.json
items_path = DATA_DIR / "items.json"
items_data = json.load(open(items_path, encoding="utf-8"))
for idx, it in enumerate(items_data):
    iid = it.get("id")
    name = it.get("name", "")
    if len(name) > 18:
        findings["length"].append({
            "source": f"items.json:items[{idx}].name ({iid})",
            "text": name,
            "len": len(name),
            "limit": 18,
            "type": "name"
        })

# C. sects.json
sects_path = DATA_DIR / "sects.json"
sects_data = json.load(open(sects_path, encoding="utf-8"))
for idx, s in enumerate(sects_data):
    sid = s.get("id")
    name = s.get("name", "")
    tp = s.get("tamPhap", "")
    if len(name) > 18:
        findings["length"].append({
            "source": f"sects.json:sects[{idx}].name ({sid})",
            "text": name,
            "len": len(name),
            "limit": 18,
            "type": "name"
        })
    if len(tp) > 18:
        findings["length"].append({
            "source": f"sects.json:sects[{idx}].tamPhap ({sid})",
            "text": tp,
            "len": len(tp),
            "limit": 18,
            "type": "name"
        })

# D. chapters.json
chapters_path = DATA_DIR / "Pve" / "chapters.json"
chapters_data = json.load(open(chapters_path, encoding="utf-8"))

char_map = {c["id"]: c for c in chapters_data.get("characters", [])}

for ch_idx, ch in enumerate(chapters_data.get("chapters", [])):
    chid = ch.get("chapterId")
    chname = ch.get("name", "")
    chdesc = ch.get("description", "")
    
    # Story consistency check:
    # H1: Thanh Khê Trấn (autumn town, bandits, Liệt Diễm Bang, Ba Hổ Trại, boss Bùi Chấn)
    # H2: Thập Vạn Man Uyên (poison swamp, beasts, Bách Thảo Cốc, Huyền Độc Môn, boss Hắc Độc Nương Tử)
    # H3: Hàn Nguyệt Cô Phong (snow mountain, cold wind, U Minh giáo, boss Dạ Sát Thiên)
    for st_idx, st in enumerate(ch.get("stages", [])):
        sid = st.get("stageId")
        stype = st.get("type")
        sname = st.get("name", "")
        sdesc = st.get("description", "")
        enemy = st.get("enemy", {})
        ename = enemy.get("name", "")
        
        # Length check
        if len(sname) > 18:
            findings["length"].append({
                "source": f"chapters.json:ch[{chid}].stages[{sid}].name",
                "text": sname,
                "len": len(sname),
                "limit": 18,
                "type": "name"
            })
        if len(ename) > 18:
            findings["length"].append({
                "source": f"chapters.json:ch[{chid}].stages[{sid}].enemy.name",
                "text": ename,
                "len": len(ename),
                "limit": 18,
                "type": "name"
            })
        if len(sdesc) > 90:
            findings["length"].append({
                "source": f"chapters.json:ch[{chid}].stages[{sid}].description",
                "text": sdesc,
                "len": len(sdesc),
                "limit": 90,
                "type": "desc"
            })
            
        # Punctuation check
        if "..." in sdesc or "..." in sname:
            findings["typo"].append({
                "source": f"chapters.json:ch[{chid}].stages[{sid}]",
                "issue": "Dùng '...' thay vì '…'",
                "text": sdesc if "..." in sdesc else sname
            })
            
        # Dialogues
        for d_idx, d in enumerate(st.get("introDialogue", [])):
            sp = d.get("speaker")
            text = d.get("text", "")
            if sp not in char_map and sp != "player":
                findings["story"].append({
                    "source": f"chapters.json:ch[{chid}].stages[{sid}].introDialogue[{d_idx}]",
                    "issue": f"Người nói '{sp}' không tồn tại trong danh sách characters!",
                    "text": text
                })
            if "..." in text:
                findings["typo"].append({
                    "source": f"chapters.json:ch[{chid}].stages[{sid}].introDialogue[{d_idx}]",
                    "issue": "Dùng '...' thay vì '…'",
                    "text": text
                })
        for d_idx, d in enumerate(st.get("outroDialogue", [])):
            sp = d.get("speaker")
            text = d.get("text", "")
            if sp not in char_map and sp != "player":
                findings["story"].append({
                    "source": f"chapters.json:ch[{chid}].stages[{sid}].outroDialogue[{d_idx}]",
                    "issue": f"Người nói '{sp}' không tồn tại trong danh sách characters!",
                    "text": text
                })
            if "..." in text:
                findings["typo"].append({
                    "source": f"chapters.json:ch[{chid}].stages[{sid}].outroDialogue[{d_idx}]",
                    "issue": "Dùng '...' thay vì '…'",
                    "text": text
                })

# Print summary of Data Audit
print(f"Data audit: {len(findings['length'])} length risks, {len(findings['typo'])} typos, {len(findings['story'])} story issues.")

with open("tools/gemini/out/text-audit/data_findings.json", "w", encoding="utf-8") as f:
    json.dump(findings, f, indent=2, ensure_ascii=False)
