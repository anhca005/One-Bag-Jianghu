import json
import re
import os
import sys

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

DATA_DIR = "One-Bag-Jianghu/Assets/_Game/Resources/Data"

# 1. Materials
materials = json.load(open(f"{DATA_DIR}/materials.json", encoding="utf-8"))
print(f"=== MATERIALS ({len(materials)}) ===")
for idx, m in enumerate(materials):
    name = m.get("name", "")
    desc = m.get("description", "")
    if len(name) > 18:
        print(f"[Length > 18] materials[{idx}].name: '{name}' ({len(name)})")
    if len(desc) > 90:
        print(f"[Length > 90] materials[{idx}].description: '{desc}' ({len(desc)})")

# 2. Items
items = json.load(open(f"{DATA_DIR}/items.json", encoding="utf-8"))
print(f"\n=== ITEMS ({len(items)}) ===")
for idx, it in enumerate(items):
    name = it.get("name", "")
    if len(name) > 18:
        print(f"[Length > 18] items[{idx}].name: '{name}' ({len(name)})")

# 3. Sects
sects = json.load(open(f"{DATA_DIR}/sects.json", encoding="utf-8"))
print(f"\n=== SECTS ({len(sects)}) ===")
for idx, s in enumerate(sects):
    name = s.get("name", "")
    tp = s.get("tamPhap", "")
    if len(name) > 18:
        print(f"[Length > 18] sects[{idx}].name: '{name}' ({len(name)})")
    if len(tp) > 18:
        print(f"[Length > 18] sects[{idx}].tamPhap: '{tp}' ({len(tp)})")

# 4. Levels
levels = json.load(open(f"{DATA_DIR}/levels.json", encoding="utf-8"))
print(f"\n=== LEVELS ===")
for idx, u in enumerate(levels.get("unlocks", [])):
    desc = u.get("description", "")
    name = u.get("featureName", "")
    if len(desc) > 90:
        print(f"[Length > 90] levels.unlocks[{idx}].description: '{desc}' ({len(desc)})")
    if len(name) > 18:
        print(f"[Length > 18] levels.unlocks[{idx}].featureName: '{name}' ({len(name)})")

# 5. Chapters
chapters = json.load(open(f"{DATA_DIR}/Pve/chapters.json", encoding="utf-8"))
print(f"\n=== CHAPTERS ===")
chars = chapters.get("characters", {})
for cid, cdata in chars.items():
    cname = cdata.get("name", "")
    ctitle = cdata.get("title", "")
    if len(cname) > 18:
        print(f"[Length > 18] chapters.characters[{cid}].name: '{cname}' ({len(cname)})")
    if len(ctitle) > 18:
        print(f"[Length > 18] chapters.characters[{cid}].title: '{ctitle}' ({len(ctitle)})")

for ch_idx, ch in enumerate(chapters.get("chapters", [])):
    ch_name = ch.get("name", "")
    ch_desc = ch.get("description", "")
    if len(ch_name) > 18:
        print(f"[Length > 18] chapters[{ch_idx}].name: '{ch_name}' ({len(ch_name)})")
    for st_idx, st in enumerate(ch.get("stages", [])):
        st_name = st.get("name", "")
        st_desc = st.get("description", "")
        en_name = st.get("enemy", {}).get("name", "")
        if len(st_name) > 18:
            print(f"[Length > 18] ch[{ch_idx}].stages[{st_idx}].name: '{st_name}' ({len(st_name)})")
        if len(en_name) > 18:
            print(f"[Length > 18] ch[{ch_idx}].stages[{st_idx}].enemy.name: '{en_name}' ({len(en_name)})")
        if len(st_desc) > 90:
            print(f"[Length > 90] ch[{ch_idx}].stages[{st_idx}].description: '{st_desc}' ({len(st_desc)})")
        for d_idx, d in enumerate(st.get("introDialogue", [])):
            text = d.get("text", "")
            sp = d.get("speaker", "")
            if sp not in chars and sp != "player":
                print(f"[Unknown speaker] ch[{ch_idx}].stages[{st_idx}].introDialogue[{d_idx}]: speaker '{sp}' not in characters!")
        for d_idx, d in enumerate(st.get("outroDialogue", [])):
            text = d.get("text", "")
            sp = d.get("speaker", "")
            if sp not in chars and sp != "player":
                print(f"[Unknown speaker] ch[{ch_idx}].stages[{st_idx}].outroDialogue[{d_idx}]: speaker '{sp}' not in characters!")

# 6. MonsterItems
monster_items = chapters.get("monsterItems", [])
print(f"\n=== MONSTER ITEMS ({len(monster_items)}) ===")
for idx, mi in enumerate(monster_items):
    name = mi.get("name", "")
    if len(name) > 18:
        print(f"[Length > 18] chapters.monsterItems[{idx}].name: '{name}' ({len(name)})")

# 7. Shop
shop = json.load(open(f"{DATA_DIR}/shop.json", encoding="utf-8"))
print(f"\n=== SHOP ===")
# check shop fields

# 8. Rewards
rewards = json.load(open(f"{DATA_DIR}/rewards.json", encoding="utf-8"))
print(f"\n=== REWARDS ===")

print("\n--- DATA SCAN COMPLETE ---")
