import json, subprocess, shutil
from pathlib import Path

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
DST_BALANCE = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance"
P_DIR = DST_BALANCE / "player"
E_DIR = DST_BALANCE / "enemy"
ITEMS = ROOT / "tools" / "gemini" / "out" / "round-2" / "items-v2-final.json"
SECTS = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

temp_a = DST_BALANCE / "temp_a"
temp_b = DST_BALANCE / "temp_b"
temp_a.mkdir(exist_ok=True)
temp_b.mkdir(exist_ok=True)

with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "chapters.json", "r", encoding="utf-8") as f:
    chapters_data = json.load(f)

stages_info = {}
for ch in chapters_data["chapters"]:
    for st in ch["stages"]:
        idx = st["index"] if ch["index"] == 1 else (st["index"] + (ch["index"] - 1) * 10)
        stages_info[idx] = {
            "name": st["name"],
            "kind": st["kind"],
            "enemy_name": st["enemy"]["name"],
            "enemy_hp": st["enemy"]["hp"],
        }

def get_target_band(kind):
    if kind == "boss":
        return 50.0, 60.0
    elif kind == "elite":
        return 60.0, 70.0
    return 80.0, 90.0

results = []
print("Evaluating all 30 PvE stages vs...")

for stage_idx in range(1, 31):
    fname = f"stage_{stage_idx:02d}.json"
    shutil.copy(P_DIR / fname, temp_a / fname)
    shutil.copy(E_DIR / fname, temp_b / fname)
    
    cmd = [
        "py", "tools/balance/balance.py", "vs",
        "--a", str(temp_a),
        "--b", str(temp_b),
        "--runs", "100",
        "--items", str(ITEMS),
        "--sects", str(SECTS)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
    
    (temp_a / fname).unlink()
    (temp_b / fname).unlink()
    
    # Parse
    win_rate = None
    for line in res.stdout.splitlines():
        if "%" in line and "vs" in line:
            parts = line.split(":")
            win_rate = float(parts[-1].split("%")[0].strip())
            break
            
    info = stages_info.get(stage_idx, {})
    kind = info.get("kind", "normal")
    low, high = get_target_band(kind)
    ok = (win_rate is not None) and (low <= win_rate <= high)
    results.append({
        "stage": stage_idx,
        "name": info.get("name"),
        "kind": kind,
        "enemy": info.get("enemy_name"),
        "win_rate": win_rate,
        "band": f"{low:.0f}-{high:.0f}%",
        "ok": ok
    })

print(f"{'Ải':<4} {'Tên Ải':<24} {'Loại':<8} {'Kẻ Địch':<26} {'Tỷ lệ thắng':<12} {'Mục tiêu':<10} {'Đạt?'}")
print("-" * 95)
for r in results:
    status = "OK" if r["ok"] else "FAIL"
    print(f"{r['stage']:02d}   {r['name'][:22]:<24} {r['kind']:<8} {r['enemy'][:24]:<26} {r['win_rate']:5.1f}%      {r['band']:<10} {status}")
