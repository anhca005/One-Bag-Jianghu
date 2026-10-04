import subprocess
from pathlib import Path
import sys
ROOT = Path("d:/CONG VIEC/One-Bag-Jianghu")
sys.path.insert(0, str(ROOT / "tools/balance"))
import balance
cp = balance.ensure_classpath()

code = """
import com.onebagjianghu.battle.sim.BattleConfig;
import java.nio.file.Path;

public class TestLoad {
    public static void main(String[] args) {
        BattleConfig cfg = BattleConfig.loadOverride(Path.of(args[0]));
        System.out.println("burn.damage: " + cfg.burn.damage);
        System.out.println("burn.threshold: " + cfg.burn.threshold);
        System.out.println("exhaustionStep: " + cfg.exhaustionStep);
    }
}
"""
Path("tools/gemini/out/round-3/scratch/TestLoad.java").write_text(code)
subprocess.run(["javac", "-cp", cp, "tools/gemini/out/round-3/scratch/TestLoad.java"], check=True)
res = subprocess.run(["java", "-cp", cp + ";tools/gemini/out/round-3/scratch", "TestLoad", "tools/gemini/out/round-3/scratch/test_override.json"], capture_output=True, text=True)
print(res.stdout)
