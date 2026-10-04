import subprocess

cmd = [
    "java", "-jar", "server/battle-engine/target/battle-engine-1.0.0.jar",
    "--p1", "tools/gemini/out/round-2/loadouts/mb3.json",
    "--p2", "tools/gemini/out/round-2/loadouts/liet_diem.json",
    "--items", "tools/gemini/out/round-2/items-base.json",
    "--patch", "tools/gemini/out/round-2/temp_patch.json",
    "--p1-tier", "6", "--p2-tier", "6",
    "--runs", "1",
    "--log"
]
res = subprocess.run(cmd, capture_output=True, text=True)
print("=== P1 items resolved ===")
for line in res.stdout.splitlines()[:50]:
    print(line)
