import json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

items_db = {it['id']: it for it in json.load(open('server/battle-engine/src/main/resources/data/items.json', encoding='utf-8'))}

def check_bag(name, items, cols=5, rows=4):
    grid = [[None for _ in range(cols)] for _ in range(rows)]
    for idx, it in enumerate(items):
        iid = it['id']
        size = items_db[iid]['size']
        w, h = size[0], size[1]
        x, y = it['x'], it['y']
        if x < 0 or y < 0 or x + w > cols or y + h > rows:
            print(f"ERROR: {name}: item #{idx} ({iid}) at [{x},{y},{w},{h}] out of bag bounds ({cols}x{rows})")
            return False
        for r in range(y, y + h):
            for c in range(x, x + w):
                if grid[r][c] is not None:
                    print(f"ERROR: {name}: item #{idx} ({iid}) overlaps item #{grid[r][c]} ({items[grid[r][c]]['id']}) at ({c}, {r})")
                    return False
                grid[r][c] = idx
    return True

ch = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
for c_idx, chap in enumerate(ch["chapters"]):
    for s_idx, s in enumerate(chap["stages"]):
        st_num = c_idx * 10 + s_idx + 1
        e = s["enemy"]
        check_bag(f"Stage {st_num} enemy ({e['name']})", e["items"], e.get("cols", 5), e.get("rows", 4))
        p = json.load(open(f"tools/gemini/out/round-4b/player/stage_{st_num:02d}.json", encoding="utf-8"))
        check_bag(f"Stage {st_num} player", p["items"], p.get("cols", 5), p.get("rows", 4))

print("Overlap check finished.")
