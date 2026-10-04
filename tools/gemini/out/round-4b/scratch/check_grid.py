import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

with open("server/battle-engine/src/main/resources/data/items.json", encoding="utf-8") as f:
    items = {it["id"]: it for it in json.load(f)}

with open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8") as f:
    doc = json.load(f)

overlaps = 0
out_of_bounds = 0
for c in doc["chapters"]:
    for s in c["stages"]:
        e = s["enemy"]
        grid = {}
        for it in e["items"]:
            iid = it["id"]
            meta = items.get(iid)
            if not meta:
                print(f"Stage {s['id']}: item {iid} NOT IN DB!")
                continue
            w = meta.get("width", 1)
            h = meta.get("height", 1)
            x, y = it.get("x", 0), it.get("y", 0)
            if x + w > 5 or y + h > 4:
                print(f"Stage {s['id']}: item {iid} out of bounds: x={x}, y={y}, w={w}, h={h}")
                out_of_bounds += 1
            for dx in range(w):
                for dy in range(h):
                    pos = (x + dx, y + dy)
                    if pos in grid:
                        print(f"Stage {s['id']}: OVERLAP at {pos} between {grid[pos]} and {iid}")
                        overlaps += 1
                    grid[pos] = iid

print(f"Grid check completed: overlaps={overlaps}, out_of_bounds={out_of_bounds}")
