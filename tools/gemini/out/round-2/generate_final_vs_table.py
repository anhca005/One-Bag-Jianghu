import json, subprocess, shutil, sys
from pathlib import Path
sys.stdout.reconfigure(encoding="utf-8")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
DST_BALANCE = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance"
P_DIR = DST_BALANCE / "player"
E_DIR = DST_BALANCE / "enemy"
ITEMS = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

temp_a = DST_BALANCE / "temp_a"
temp_b = DST_BALANCE / "temp_b"

with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "chapters.json", "r", encoding="utf-8") as f:
    chapters_data = json.load(f)

stages_info = {}
for ch in chapters_data["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        stages_info[idx] = {
            "name": st["name"],
            "kind": st["kind"],
            "enemy_name": st["enemy"]["name"]
        }

def get_target_band(kind):
    if kind == "boss":
        return 50.0, 60.0
    elif kind == "elite":
        return 60.0, 70.0
    return 80.0, 90.0

def eval_single_stage(idx, runs=100):
    fname = f"stage_{idx:02d}.json"
    shutil.copy(P_DIR / fname, temp_a / fname)
    shutil.copy(E_DIR / fname, temp_b / fname)
    cmd = ["py", "tools/balance/balance.py", "vs", "--a", str(temp_a), "--b", str(temp_b), "--runs", str(runs), "--items", str(ITEMS), "--sects", str(SECTS)]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
    (temp_a / fname).unlink()
    (temp_b / fname).unlink()
    for line in res.stdout.splitlines():
        if "%" in line and not line.strip().startswith("("):
            for p in line.strip().split():
                if p.endswith("%"):
                    try:
                        return float(p[:-1]), line.strip()
                    except ValueError:
                        pass
    return None, res.stdout

results = []
for idx in range(1, 31):
    info = stages_info[idx]
    kind = info["kind"]
    low, high = get_target_band(kind)
    fname = f"stage_{idx:02d}.json"
    with open(E_DIR / fname, "r", encoding="utf-8") as f:
        e_data = json.load(f)
    wr, line = eval_single_stage(idx, runs=100)
    results.append({
        "stage": idx,
        "name": info["name"],
        "kind": kind,
        "enemy": e_data.get("name", info["enemy_name"]),
        "enemy_hp": e_data["hp"],
        "win_rate": wr,
        "band": f"{low:.0f}-{high:.0f}%",
        "ok": (low <= wr <= high)
    })
    print(f"Stage {idx:02d}: {wr:.1f}% [{low:.0f}-{high:.0f}%] {'OK' if (low <= wr <= high) else 'FAIL'}")

out_table = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance" / "vs_table.txt"
with open(out_table, "w", encoding="utf-8") as f:
    f.write(f"{'Ải':<4} {'Tên Ải':<24} {'Loại':<8} {'Kẻ Địch':<26} {'Tỷ lệ thắng':<12} {'Mục tiêu':<10} {'Đạt?'}\n")
    f.write("-" * 95 + "\n")
    for r in results:
        status = "OK" if r["ok"] else "FAIL"
        f.write(f"{r['stage']:02d}   {r['name'][:22]:<24} {r['kind']:<8} {r['enemy'][:24]:<26} {r['win_rate']:5.1f}%      {r['band']:<10} {status}\n")

print("Generated vs_table.txt successfully!")
