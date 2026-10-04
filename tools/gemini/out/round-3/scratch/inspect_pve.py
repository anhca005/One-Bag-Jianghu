import json
from pathlib import Path

R3 = Path("d:/CONG VIEC/One-Bag-Jianghu/tools/gemini/out/round-3/balance")
for i in range(1, 31):
    p = json.load(open(R3 / "player" / f"stage_{i:02d}.json", "r", encoding="utf-8"))
    e = json.load(open(R3 / "enemy" / f"stage_{i:02d}.json", "r", encoding="utf-8"))
    p_tier = p["items"][0].get("tier", 1)
    p_rarity = p["items"][0].get("rarity", "common")
    e_tier = e["items"][0].get("tier", 1)
    e_rarity = e["items"][0].get("rarity", "common")
    print(f"Stage {i:02d}: P(lvl={p['level']}, T{p_tier} {p_rarity}, {len(p['items'])} items) vs E(lvl={e['level']}, T{e_tier} {e_rarity}, {len(e['items'])} items)")
