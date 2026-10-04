import json
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
SRC_LOADOUTS = ROOT / "tools/gemini/out/round-2/loadouts"
DST_LOADOUTS = ROOT / "tools/gemini/out/round-3/loadouts"
DST_LOADOUTS.mkdir(parents=True, exist_ok=True)

for p in SRC_LOADOUTS.glob("*.json"):
    data = json.loads(p.read_text(encoding="utf-8"))
    if "hp" in data:
        del data["hp"]
    data["level"] = 1
    (DST_LOADOUTS / p.name).write_text(json.dumps(data, indent=2, ensure_ascii=False), encoding="utf-8")

print(f"Converted {len(list(DST_LOADOUTS.glob('*.json')))} loadouts to round-3/loadouts/ with level: 1")
