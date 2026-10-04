import json
import copy
import tempfile
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
R4 = ROOT / "tools" / "gemini" / "out" / "round-4"
ITEMS_PATH = R4 / "items-final.json"
CONFIG_PATH = R4 / "config.json"
SECTS_PATH = ROOT / "server" / "battle-engine" / "src" / "main" / "resources" / "data" / "sects.json"

sys.path.insert(0, str((ROOT / "tools" / "balance").resolve()))
import balance
classpath = balance.ensure_classpath()

def clean_loadout(ld):
    return {
        "name": ld.get("name", "Unit"),
        "cols": ld.get("cols", 5),
        "rows": ld.get("rows", 4),
        "level": ld.get("level", 1),
        "sect": ld.get("sect"),
        "tamPhapLevel": ld.get("tamPhapLevel", 1),
        "items": copy.deepcopy(ld["items"])
    }

def get_target_band(idx):
    if idx in (10, 20, 30):
        return 50.0, 60.0
    elif idx in (5, 15, 25):
        return 60.0, 70.0
    return 80.0, 90.0

ch_doc = json.load(open(R4 / "chapters.json", encoding="utf-8"))
p_dir = R4 / "balance" / "player"
e_dir = R4 / "balance" / "enemy"

stages_flat = []
for ch in ch_doc["chapters"]:
    for st in ch["stages"]:
        stages_flat.append(st)

vs_rows = []
all_ok = True

print("=== VERIFYING ALL 30 PVE STAGES (100 runs) ===")
for idx in range(1, 31):
    low, high = get_target_band(idx)
    st = stages_flat[idx - 1]
    p_data = json.load(open(p_dir / f"stage_{idx:02d}.json", encoding="utf-8"))
    e_data = json.load(open(e_dir / f"stage_{idx:02d}.json", encoding="utf-8"))
    
    with tempfile.TemporaryDirectory(prefix="pve_ver_") as raw_temp:
        temp = Path(raw_temp)
        fa = temp / "p.json"
        fb = temp / "e.json"
        balance.write_json(fa, clean_loadout(p_data), indent=None)
        balance.write_json(fb, clean_loadout(e_data), indent=None)
        wr = balance.cli_match(classpath, fa, fb, 100, str(ITEMS_PATH), str(SECTS_PATH), str(CONFIG_PATH))
    
    ok = (low <= wr <= high)
    if not ok: all_ok = False
    ok_str = "ĐẠT" if ok else "CHƯA ĐẠT"
    print(f"Ải {idx:02d} ({low:.0f}-{high:.0f}%): WR={wr:5.1f}% [{ok_str}]")
    
    vs_rows.append({
        "idx": idx,
        "name": st["name"],
        "kind": st["kind"],
        "enemy_name": st["enemy"]["name"],
        "enemy_lvl": e_data["level"],
        "winrate": wr,
        "target": f"{low:.0f}-{high:.0f}%",
        "ok": ok_str
    })

status_msg = "100% ĐẠT!" if all_ok else "SOME NOT MET"
print(f"\nALL 30 STAGES STATUS: {status_msg}")

table_lines = []
table_lines.append(f"{'Ải':<4} {'Tên Ải':<24} {'Loại':<8} {'Kẻ Địch':<26} {'Cấp Địch':<10} {'Tỷ Lệ Thắng':<14} {'Mục Tiêu':<10} {'Đạt?'}")
table_lines.append("-" * 105)
for r in vs_rows:
    table_lines.append(
        f"{r['idx']:02d}   {r['name']:<24} {r['kind']:<8} {r['enemy_name']:<26} {r['enemy_lvl']:<10} "
        f"{r['winrate']:5.1f}%         {r['target']:<10} {r['ok']}"
    )
table_lines.append("-" * 105)

with open(R4 / "balance" / "vs_table.txt", "w", encoding="utf-8") as f:
    f.write("\n".join(table_lines) + "\n")
print("Written round-4/balance/vs_table.txt!")
