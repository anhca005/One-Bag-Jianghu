package round5;

import com.onebagjianghu.battle.sim.*;
import com.onebagjianghu.battle.data.*;
import java.nio.file.*;
import java.util.*;

public class CheckStats {
    public static void main(String[] args) throws Exception {
        ItemCatalog cat = ItemCatalog.loadDefault();
        SectCatalog sects = SectCatalog.loadDefault();
        BattleConfig cfg = BattleConfig.defaults();
        BattleSimulator sim = new BattleSimulator(cat, sects, cfg);

        for (Path p : Files.list(Path.of("server/battle-engine/samples")).sorted().toList()) {
            Loadout orig = Loadout.load(p);
            for (int t : new int[]{1, 3, 6, 9}) {
                Loadout ld = new Loadout();
                ld.name = orig.name;
                ld.sect = orig.sect;
                ld.level = 10;
                ld.tamPhapLevel = 1;
                for (Loadout.PlacedItem it : orig.items) {
                    Loadout.PlacedItem ni = new Loadout.PlacedItem();
                    ni.id = it.id; ni.x = it.x; ni.y = it.y; ni.tier = t;
                    ni.rarity = Rarity.COMMON;
                    ld.items.add(ni);
                }
                var stats = sim.fighterStats(ld);
                if (stats.dmgReducePermille() > 0) {
                    System.out.printf("%-35s T%d: dmgReduce=%d‰%n", orig.name, t, stats.dmgReducePermille());
                }
            }
        }
    }
}
