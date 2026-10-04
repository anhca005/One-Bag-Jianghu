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
            "chapter_idx": ch["index"] - 1,
            "stage_in_ch": st["index"] - 1
        }

def get_target_band(kind):
    if kind == "boss":
        return 50.0, 60.0
    elif kind == "elite":
        return 60.0, 70.0
    return 80.0, 90.0

def eval_single_stage(idx, runs=60):
    fname = f"stage_{idx:02d}.json"
    shutil.copy(P_DIR / fname, temp_a / fname)
    shutil.copy(E_DIR / fname, temp_b / fname)
    
    cmd = [
        "py", "tools/balance/balance.py", "vs",
        "--a", str(temp_a),
        "--b", str(temp_b),
        "--runs", str(runs),
        "--items", str(ITEMS),
        "--sects", str(SECTS)
    ]
    res = subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace")
    (temp_a / fname).unlink()
    (temp_b / fname).unlink()
    
    for line in res.stdout.splitlines():
        if "%" in line and not line.strip().startswith("("):
            parts = line.strip().split()
            for p in parts:
                if p.endswith("%"):
                    try:
                        return float(p[:-1]), line.strip()
                    except ValueError:
                        pass
    return None, res.stdout

def tune_all_stages():
    results = []
    for idx in range(1, 31):
        info = stages_info[idx]
        kind = info["kind"]
        low, high = get_target_band(kind)
        fname = f"stage_{idx:02d}.json"
        
        # Load enemy loadout
        with open(E_DIR / fname, "r", encoding="utf-8") as f:
            e_data = json.load(f)
            
        print(f"=== Stage {idx:02d} ({info['name']}, {kind}, band {low:.0f}-{high:.0f}%) ===")
        
        # Test current HP
        wr, _ = eval_single_stage(idx, runs=60)
        print(f"  Init: HP={e_data['hp']} -> WinRate={wr:.1f}%")
        
        if wr is not None and low <= wr <= high:
            print(f"  Stage {idx:02d} ALREADY IN BAND! ({wr:.1f}%)")
        else:
            # Binary search
            if wr is not None and wr > high:
                hp_min = e_data["hp"]
                hp_max = e_data["hp"] * 4
            elif wr is not None and wr < low:
                hp_min = max(100, int(e_data["hp"] * 0.2))
                hp_max = e_data["hp"]
            else:
                hp_min, hp_max = 200, 6000
                
            best_hp = e_data["hp"]
            best_diff = 999
            
            for step in range(8):
                mid_hp = int(round((hp_min + hp_max) / 20.0) * 10)
                e_data["hp"] = mid_hp
                with open(E_DIR / fname, "w", encoding="utf-8") as f:
                    json.dump(e_data, f, indent=2, ensure_ascii=False)
                    
                wr, _ = eval_single_stage(idx, runs=60)
                if wr is None:
                    break
                print(f"  Step {step}: HP={mid_hp} -> WinRate={wr:.1f}% (range [{hp_min}, {hp_max}])")
                
                target = (low + high) / 2.0
                if abs(wr - target) < best_diff:
                    best_diff = abs(wr - target)
                    best_hp = mid_hp
                    
                if low <= wr <= high:
                    print(f"  Stage {idx:02d} REACHED BAND! ({wr:.1f}%)")
                    break
                    
                if wr > high:
                    # Player winning too much -> need enemy to be stronger (higher HP)
                    hp_min = mid_hp + 10
                    if hp_max < hp_min:
                        hp_max = hp_min * 2
                else:
                    # Player winning too little -> need enemy to be weaker (lower HP)
                    hp_max = mid_hp - 10
                    if hp_max < hp_min:
                        hp_min = max(100, hp_max // 2)
                        
            # Set to best found HP
            e_data["hp"] = best_hp
            with open(E_DIR / fname, "w", encoding="utf-8") as f:
                json.dump(e_data, f, indent=2, ensure_ascii=False)
                
        # Final evaluation with 100 runs
        final_wr, final_line = eval_single_stage(idx, runs=100)
        results.append({
            "stage": idx,
            "name": info["name"],
            "kind": kind,
            "enemy": e_data.get("name", info["enemy_name"]),
            "enemy_hp": e_data["hp"],
            "win_rate": final_wr,
            "band": f"{low:.0f}-{high:.0f}%",
            "ok": (low <= final_wr <= high),
            "line": final_line
        })
        print(f"  FINAL: HP={e_data['hp']} -> WinRate={final_wr:.1f}% [{'OK' if (low <= final_wr <= high) else 'FAIL'}]")
        
        # Update chapters.json
        ch_i = info["chapter_idx"]
        st_i = info["stage_in_ch"]
        chapters_data["chapters"][ch_i]["stages"][st_i]["enemy"]["hp"] = e_data["hp"]
        
    with open(ROOT / "tools" / "gemini" / "out" / "round-2" / "chapters.json", "w", encoding="utf-8") as f:
        json.dump(chapters_data, f, indent=2, ensure_ascii=False)
        
    return results

if __name__ == "__main__":
    res = tune_all_stages()
    out_table = ROOT / "tools" / "gemini" / "out" / "round-2" / "balance" / "vs_table.txt"
    with open(out_table, "w", encoding="utf-8") as f:
        f.write(f"{'Ải':<4} {'Tên Ải':<24} {'Loại':<8} {'Kẻ Địch':<26} {'Tỷ lệ thắng':<12} {'Mục tiêu':<10} {'Đạt?'}\n")
        f.write("-" * 95 + "\n")
        for r in res:
            status = "OK" if r["ok"] else "FAIL"
            f.write(f"{r['stage']:02d}   {r['name'][:22]:<24} {r['kind']:<8} {r['enemy'][:24]:<26} {r['win_rate']:5.1f}%      {r['band']:<10} {status}\n")
    print("Wrote vs_table.txt successfully!")
