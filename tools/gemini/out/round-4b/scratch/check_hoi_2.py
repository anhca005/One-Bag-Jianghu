import copy, json, sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

from tune_stages import simulate_stage, check_rules

CHAPTERS = json.load(open("tools/gemini/out/round-4b/chapters.json", encoding="utf-8"))
stages = [s for c in CHAPTERS["chapters"] for s in c["stages"]]

print("Checking Hồi 2 stages (11-20)...")
for s_num in range(11, 21):
    s = stages[s_num - 1]
    p = json.load(open(f"tools/gemini/out/round-4b/player/stage_{s_num:02d}.json", encoding="utf-8"))
    e = copy.deepcopy(s["enemy"])
    target_lvl = s_num + 1 if s["kind"] == "boss" else s_num
    e["level"] = target_lvl
    wr, dur = simulate_stage(s_num, e, runs=80)
    notes = check_rules(s_num, s["kind"], p, e, wr, dur)
    print(f"Stage {s_num:02d} [{s['kind']:6}] (lvl {target_lvl}): WR={wr:5.1f}%, dur={dur:4.1f}s | notes: {notes}")
