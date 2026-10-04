package round5;

import com.onebagjianghu.battle.sim.*;
import com.onebagjianghu.battle.data.*;
import java.nio.file.*;
import java.util.*;

public class MaxDmgReduceTracker {
    public static void main(String[] args) throws Exception {
        ItemCatalog cat = ItemCatalog.loadDefault();
        SectCatalog sects = SectCatalog.loadDefault();
        BattleConfig cfg = BattleConfig.defaults();
        BattleSimulator sim = new BattleSimulator(cat, sects, cfg);

        List<Loadout> samples = Files.list(Path.of("server/battle-engine/samples"))
            .filter(p -> p.toString().endsWith(".json"))
            .map(Loadout::load).toList();

        int maxDrSeen = 0;
        String maxLoadout = "";

        for (int t : new int[]{1, 3, 6, 9}) {
            for (Loadout a : samples) {
                Loadout la = adapt(a, t);
                for (Loadout b : samples) {
                    Loadout lb = adapt(b, t);
                    for (int seed = 1; seed <= 5; seed++) {
                        BattleResult r = sim.run(la, lb, seed);
                        for (BattleEvent ev : r.events()) {
                            if (ev.type == EventType.DMG_REDUCE && ev.value != null) {
                                if (ev.value > maxDrSeen) {
                                    maxDrSeen = ev.value;
                                    maxLoadout = la.name + " T" + t;
                                }
                            }
                        }
                    }
                }
            }
        }
        System.out.printf(Locale.ROOT, "Max dmgReduce seen across all matches: %d permille (%.1f%%) in %s%n",
            maxDrSeen, maxDrSeen / 10.0, maxLoadout);
    }

    private static Loadout adapt(Loadout orig, int tier) {
        Loadout ld = new Loadout();
        ld.name = orig.name; ld.sect = orig.sect; ld.level = 10; ld.tamPhapLevel = 1;
        for (var it : orig.items) {
            var ni = new Loadout.PlacedItem();
            ni.id = it.id; ni.x = it.x; ni.y = it.y; ni.tier = tier; ni.rarity = Rarity.COMMON;
            ld.items.add(ni);
        }
        return ld;
    }
}
