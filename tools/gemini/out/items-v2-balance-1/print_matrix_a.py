import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[4]
sys.path.insert(0, str(ROOT))
from tools.balance.balance import ensure_classpath, load_patch, patched_files, evaluate_matrix, load_battle_config, print_table

sys.stdout.reconfigure(encoding="utf-8")
cp = ensure_classpath()
loadouts_dir = ROOT / "tools/gemini/out/items-v2-balance-1/loadouts"
items_path = ROOT / "tools/gemini/out/items-v2/items-v2.normalized.json"
sects_path = ROOT / "server/battle-engine/src/main/resources/data/sects.json"

pfile = ROOT / "tools/gemini/out/items-v2-balance-1/temp_patch.json"
patch = load_patch(pfile)
base_config = load_battle_config(cp, None)

import tempfile
with tempfile.TemporaryDirectory() as raw_temp:
    t_items, t_sects, t_config = patched_files(Path(raw_temp), items_path, sects_path, base_config, patch)
    labels, variants, rows = evaluate_matrix(
        cp, loadouts_dir, [1, 3, 6, 9], [1, 9], 100, t_items, t_sects, parallel=True, battle_config=t_config
    )

print_table(labels, variants, rows, 35, 65)
