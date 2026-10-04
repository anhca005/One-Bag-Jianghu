import subprocess, json

cmd = [
    "py", "tools/balance/balance.py", "matrix",
    "--catalog", "tools/gemini/out/round-2/trial3_items.json",
    "--loadouts", "tools/gemini/out/round-2/loadouts",
    "--tiers", "1,3,6,9",
    "--tp", "1,9",
    "--runs", "100"
]
# Run matrix and get per-tier/tp breakdown
res = subprocess.run(cmd, capture_output=True, text=True, check=True)
print(res.stdout[:4000])
with open("tools/gemini/out/round-2/trial3_matrix_out.txt", "w", encoding="utf-8") as f:
    f.write(res.stdout)
