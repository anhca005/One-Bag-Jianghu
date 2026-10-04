import json, sys
from pathlib import Path
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

RARITY = ["common", "uncommon", "rare", "epic", "legendary", "supreme"]

for i in range(1, 31):
    p = json.load(open(f"tools/gemini/out/round-4b/player/stage_{i:02d}.json", encoding="utf-8"))
    pt = max(x["tier"] for x in p["items"])
    pr_idx = max(RARITY.index(x["rarity"]) for x in p["items"])
    pr = RARITY[pr_idx]
    kind = "boss" if i % 10 == 0 else "elite" if i % 5 == 0 else "normal"
    up = 1 if kind == "normal" else 2
    
    allowed_t = (max(1, pt - 1), pt + 1)
    allowed_r = (max(0, pr_idx - 1), min(len(RARITY) - 1, pr_idx + up))
    r_str = f"[{RARITY[allowed_r[0]]}..{RARITY[allowed_r[1]]}]"
    
    print(f"Stage {i:02d} ({kind:6}): P lv{p['level']:2} T{pt} {pr:9} | Allowed E: T[{allowed_t[0]}..{allowed_t[1]}] R{r_str}")
