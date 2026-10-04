import json
import re
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
DATA_DIR = ROOT / "One-Bag-Jianghu" / "Assets" / "_Game" / "Resources" / "Data"

# Load materials
materials = json.load(open(DATA_DIR / "materials.json", encoding="utf-8"))
items = json.load(open(DATA_DIR / "items.json", encoding="utf-8"))
sects = json.load(open(DATA_DIR / "sects.json", encoding="utf-8"))
chapters = json.load(open(DATA_DIR / "Pve" / "chapters.json", encoding="utf-8"))

print("=== CHECKING VIETNAMESE QUALITY IN DATA ===")

def check_text(source, text):
    if not text:
        return
    # Check space before colon or semicolon
    if re.search(r'\s+[:;]', text):
        print(f"[PUNCTUATION] Space before colon/semicolon in {source}: '{text}'")
    # Check double spaces
    if "  " in text and "   " not in text: # ignore large tab spaces if intentional
        print(f"[TYPO] Double space in {source}: '{text}'")
    # Check three dots vs ellipsis
    if "..." in text:
        print(f"[TYPO] Three dots '...' instead of '…' in {source}: '{text}'")
    # Check incorrect diacritics or words
    # Common mistakes: "thuỷ" vs "thủy", "hoà" vs "hòa", "giầy" vs "giày"
    # Check lowercase/uppercase inconsistency
    # e.g., "khí huyết" vs "Khí Huyết", "nội lực" vs "Nội Lực"
    # Check English words
    for w in ["hp", "qi", "dps", "buff", "debuff", "crit", "stun", "seal", "slow", "burn", "bleed", "poison", "regen"]:
        # only match whole word
        if re.search(rf'\b{w}\b', text, re.IGNORECASE):
            print(f"[ENGLISH LEAK] Word '{w}' found in {source}: '{text}'")

# Check materials
for idx, m in enumerate(materials):
    check_text(f"materials[{idx}].name", m.get("name"))
    check_text(f"materials[{idx}].description", m.get("description"))

# Check items
for idx, it in enumerate(items):
    check_text(f"items[{idx}].name", it.get("name"))

# Check sects
for idx, s in enumerate(sects):
    check_text(f"sects[{idx}].name", s.get("name"))
    check_text(f"sects[{idx}].tamPhap", s.get("tamPhap"))

# Check chapters
for ch in chapters.get("chapters", []):
    chid = ch.get("chapterId")
    check_text(f"ch[{chid}].name", ch.get("name"))
    check_text(f"ch[{chid}].description", ch.get("description"))
    for st in ch.get("stages", []):
        sid = st.get("stageId")
        check_text(f"ch[{chid}].st[{sid}].name", st.get("name"))
        check_text(f"ch[{chid}].st[{sid}].description", st.get("description"))
        check_text(f"ch[{chid}].st[{sid}].enemy.name", st.get("enemy", {}).get("name"))
        for d in st.get("introDialogue", []):
            check_text(f"ch[{chid}].st[{sid}].intro[{d.get('speaker')}]", d.get("text"))
        for d in st.get("outroDialogue", []):
            check_text(f"ch[{chid}].st[{sid}].outro[{d.get('speaker')}]", d.get("text"))
