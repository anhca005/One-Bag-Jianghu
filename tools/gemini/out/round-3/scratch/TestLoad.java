
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
