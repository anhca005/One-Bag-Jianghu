package round5;

import com.onebagjianghu.battle.data.ItemCatalog;
import com.onebagjianghu.battle.data.Rarity;
import com.onebagjianghu.battle.data.SectCatalog;
import com.onebagjianghu.battle.sim.BattleConfig;
import com.onebagjianghu.battle.sim.BattleResult;
import com.onebagjianghu.battle.sim.BattleSimulator;
import com.onebagjianghu.battle.sim.Loadout;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.*;
import java.util.stream.Stream;

public class FightLengthRunner {
    record TierConfig(String name, int tier, int level, Rarity rarity, int tp) {}

    public static void main(String[] args) throws IOException {
        Path loadoutsDir = Path.of("server/battle-engine/samples");
        Path itemsPath = null;
        Path sectsPath = Path.of("server/battle-engine/src/main/resources/data/sects.json");
        Path configPath = null;

        for (int i = 0; i < args.length; i++) {
            switch (args[i]) {
                case "--loadouts" -> loadoutsDir = Path.of(args[++i]);
                case "--items" -> itemsPath = Path.of(args[++i]);
                case "--sects" -> sectsPath = Path.of(args[++i]);
                case "--battle-config" -> configPath = Path.of(args[++i]);
            }
        }

        ItemCatalog catalog = itemsPath == null ? ItemCatalog.loadDefault() : ItemCatalog.load(itemsPath);
        SectCatalog sects = sectsPath == null ? SectCatalog.loadDefault() : SectCatalog.load(sectsPath);
        BattleConfig config = configPath == null ? BattleConfig.defaults() : BattleConfig.loadOverride(configPath);
        BattleSimulator sim = new BattleSimulator(catalog, sects, config);

        List<Path> files;
        try (Stream<Path> s = Files.list(loadoutsDir)) {
            files = s.filter(p -> p.toString().endsWith(".json")).sorted().toList();
        }

        List<Loadout> originals = files.stream().map(Loadout::load).toList();

        List<TierConfig> tiers = List.of(
            new TierConfig("Trọng 1 (Cấp 1, Phàm)", 1, 1, Rarity.COMMON, 1),
            new TierConfig("Trọng 3 (Cấp 8, Lương)", 3, 8, Rarity.UNCOMMON, 1),
            new TierConfig("Trọng 6 (Cấp 18, Tinh)", 6, 18, Rarity.RARE, 1),
            new TierConfig("Trọng 9 (Cấp 30, Tuyệt)", 9, 30, Rarity.EPIC, 1)
        );

        System.out.printf("%-28s %-16s %-20s %s%n", "Mốc Trọng & Cấp", "Thời Lượng TB", "Tỷ Lệ Trận > 45s", "Mục Tiêu");
        System.out.println("-".repeat(80));

        double allDurationsTotal = 0;
        int allFightsTotal = 0;
        int allOver45Total = 0;

        for (TierConfig tc : tiers) {
            List<Loadout> adapted = new ArrayList<>();
            for (Loadout orig : originals) {
                Loadout ld = new Loadout();
                ld.name = orig.name;
                ld.sect = orig.sect;
                ld.tamPhapLevel = tc.tp();
                ld.level = tc.level();
                ld.cols = orig.cols;
                ld.rows = orig.rows;
                for (Loadout.PlacedItem it : orig.items) {
                    Loadout.PlacedItem ni = new Loadout.PlacedItem();
                    ni.id = it.id;
                    ni.x = it.x;
                    ni.y = it.y;
                    ni.tier = tc.tier();
                    ni.rarity = tc.rarity();
                    ld.items.add(ni);
                }
                adapted.add(ld);
            }

            int n = adapted.size();
            long totalMs = 0;
            int totalFights = 0;
            int over45Count = 0;

            for (int i = 0; i < n; i++) {
                int[] opponents = { i, (i + 1) % n, (i + 7) % n };
                for (int j : opponents) {
                    Loadout a = adapted.get(i);
                    Loadout b = adapted.get(j);
                    for (int seed = 1; seed <= 15; seed++) {
                        BattleResult r = sim.run(a, b, seed);
                        totalFights++;
                        totalMs += r.durationMs();
                        if (r.durationMs() > 45000) {
                            over45Count++;
                        }
                    }
                }
            }

            double avgSec = (totalMs / 1000.0) / totalFights;
            double pctOver45 = (100.0 * over45Count) / totalFights;
            allDurationsTotal += totalMs / 1000.0;
            allFightsTotal += totalFights;
            allOver45Total += over45Count;

            System.out.printf(Locale.ROOT, "%-28s %5.1fs           %5.1f%%              15-35s / < 20%%%n",
                tc.name(), avgSec, pctOver45);
        }
        System.out.println("-".repeat(80));
        double overallAvg = allDurationsTotal / allFightsTotal;
        double overallPctOver45 = (100.0 * allOver45Total) / allFightsTotal;
        System.out.printf(Locale.ROOT, "%-28s %5.1fs           %5.1f%%%n", "Toàn cục", overallAvg, overallPctOver45);
        System.out.printf("(Tổng số trận đo: %d trận cá thể, mỗi mốc %d trận)%n", allFightsTotal, allFightsTotal / tiers.size());
    }
}
