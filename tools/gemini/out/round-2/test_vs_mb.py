import subprocess, json

loadouts = ["mb1", "mb2", "mb3", "mb4"]
targets = ["thiet_phu", "liet_diem", "thai_thanh", "kim_cang"]

for m in loadouts:
    for t in targets:
        cmd = [
            "py", "tools/balance/balance.py", "vs",
            f"tools/gemini/out/round-2/loadouts/{m}.json",
            f"tools/gemini/out/round-2/loadouts/{t}.json",
            "--patch", "tools/gemini/out/round-2/temp_patch.json",
            "--runs", "50",
            "--tier", "6",
            "--tp", "1"
        ]
        res = subprocess.run(cmd, capture_output=True, text=True)
        # Parse win rate
        for line in res.stdout.splitlines():
            if "Win rate:" in line or "Wins:" in line or "%" in line:
                print(f"{m} vs {t} (T6): {line.strip()}")
