import json
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
P_DIR = ROOT / "tools" / "gemini" / "out" / "pve" / "balance" / "player_loadouts"
E_DIR = ROOT / "tools" / "gemini" / "out" / "pve" / "balance" / "enemy_loadouts"

DST_P_DIR = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance" / "player"
DST_E_DIR = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance" / "enemy"
DST_P_DIR.mkdir(parents=True, exist_ok=True)
DST_E_DIR.mkdir(parents=True, exist_ok=True)

# Process stages 1 to 30
for i in range(1, 31):
    fname = f"stage_{i:02d}.json"
    p_file = P_DIR / fname
    e_file = E_DIR / fname
    
    with open(p_file, "r", encoding="utf-8") as f:
        p_loadout = json.load(f)
    with open(e_file, "r", encoding="utf-8") as f:
        e_loadout = json.load(f)
        
    # Decision #21: No sect below level 10 (stages 1-10)
    if i <= 10:
        p_loadout["sect"] = None
        
    # Text replacements in enemy names if needed
    if "Độc Cô Hạnh" in e_loadout.get("name", ""):
        e_loadout["name"] = e_loadout["name"].replace("Độc Cô Hạnh", "Phong Vũ")
    if e_loadout.get("characterId") == "doc_co_hanh":
        e_loadout["characterId"] = "phong_loi"
        e_loadout["portrait"] = "npc_phong_loi"
        
    with open(DST_P_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(p_loadout, f, indent=2, ensure_ascii=False)
    with open(DST_E_DIR / fname, "w", encoding="utf-8") as f:
        json.dump(e_loadout, f, indent=2, ensure_ascii=False)

print("Exported and adjusted all 30 stage loadouts into round-2/balance/player and /enemy.")
