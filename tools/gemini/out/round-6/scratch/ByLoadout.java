
import com.onebagjianghu.battle.data.*;
import com.onebagjianghu.battle.sim.*;
import java.nio.file.*;
import java.util.*;
import java.util.stream.*;

public class ByLoadout {
    public static void main(String[] args) throws Exception {
        Path dir = Path.of("tools/gemini/out/round-6/loadouts");
        ItemCatalog catalog = ItemCatalog.loadDefault();
        SectCatalog sects = SectCatalog.loadDefault();
        BattleConfig config = BattleConfig.defaults();
        BattleSimulator sim = new BattleSimulator(catalog, sects, config);

        List<Path> files;
        try (Stream<Path> s = Files.list(dir)) {
            files = s.filter(p -> p.toString().endsWith(".json")).sorted().toList();
        }
        List<Loadout> originals = files.stream().map(Loadout::load).toList();
        int n = originals.size();

        int[] tiers = {3, 6, 9};
        for (int tier : tiers) {
            System.out.println("=== TIER " + tier + " ===");
            Map<String, Integer> over45 = new HashMap<>();
            Map<String, Long> totalMs = new HashMap<>();
            Map<String, Integer> totalFights = new HashMap<>();

            for (int i = 0; i < n; i++) {
                Loadout a = adapt(originals.get(i), tier);
                int[] opponents = { i, (i + 1) % n, (i + 7) % n };
                for (int j : opponents) {
                    Loadout b = adapt(originals.get(j), tier);
                    for (int seed = 1; seed <= 15; seed++) {
                        BattleResult r = sim.run(a, b, seed);
                        over45.merge(a.name, r.durationMs() > 45000 ? 1 : 0, Integer::sum);
                        totalMs.merge(a.name, (long) r.durationMs(), Long::sum);
                        totalFights.merge(a.name, 1, Integer::sum);
                    }
                }
            }
            List<Map.Entry<String, Integer>> sorted = new ArrayList<>(over45.entrySet());
            sorted.sort((x, y) -> y.getValue().compareTo(x.getValue()));
            for (var e : sorted.subList(0, 10)) {
                int count = totalFights.get(e.getKey());
                double avg = (totalMs.get(e.getKey()) / 1000.0) / count;
                double pct = (100.0 * e.getValue()) / count;
                System.out.printf("%-35s: %4.1f%% > 45s (avg %4.1fs)%n", e.getKey(), pct, avg);
            }
        }
    }

    private static Loadout adapt(Loadout orig, int tier) {
        Loadout ld = new Loadout();
        ld.name = orig.name;
        ld.sect = orig.sect;
        ld.tamPhapLevel = 1;
        ld.level = tier == 3 ? 8 : (tier == 6 ? 18 : 30);
        ld.cols = orig.cols;
        ld.rows = orig.rows;
        Rarity r = tier == 3 ? Rarity.UNCOMMON : (tier == 6 ? Rarity.RARE : Rarity.EPIC);
        for (Loadout.PlacedItem it : orig.items) {
            Loadout.PlacedItem ni = new Loadout.PlacedItem();
            ni.id = it.id;
            ni.x = it.x;
            ni.y = it.y;
            ni.tier = tier;
            ni.rarity = r;
            ld.items.add(ni);
        }
        return ld;
    }
}
