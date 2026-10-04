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

public class TierStepRunner {
    record Pair(int high, int low) {}

    public static void main(String[] args) throws IOException {
        Path loadoutsDir = Path.of("server/battle-engine/samples");
        Path itemsPath = null;
        Path sectsPath = Path.of("server/battle-engine/src/main/resources/data/sects.json");
        Path configPath = null;
        int runs = 100;

        for (int i = 0; i < args.length; i++) {
            switch (args[i]) {
                case "--loadouts" -> loadoutsDir = Path.of(args[++i]);
                case "--items" -> itemsPath = Path.of(args[++i]);
                case "--sects" -> sectsPath = Path.of(args[++i]);
                case "--battle-config" -> configPath = Path.of(args[++i]);
                case "--runs" -> runs = Integer.parseInt(args[++i]);
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

        List<Pair> pairs = List.of(
            new Pair(2, 1),
            new Pair(3, 2),
            new Pair(4, 3),
            new Pair(6, 5),
            new Pair(7, 6),
            new Pair(9, 8)
        );

        int maxNameLen = originals.stream().mapToInt(l -> l.name.length()).max().orElse(20);
        int colWidth = Math.max(30, maxNameLen + 2);

        StringBuilder sb = new StringBuilder();
        sb.append(String.format("%-" + colWidth + "s", "Loadout"));
        for (Pair p : pairs) {
            sb.append(String.format("%11s", "T" + p.high + ">T" + p.low));
        }
        sb.append(String.format("%11s%n", "Average"));
        sb.append("-".repeat(colWidth + 11 * (pairs.size() + 1))).append("\n");

        double[] colSums = new double[pairs.size()];

        for (Loadout orig : originals) {
            sb.append(String.format("%-" + colWidth + "s", orig.name));
            double rowSum = 0;

            for (int pIdx = 0; pIdx < pairs.size(); pIdx++) {
                Pair p = pairs.get(pIdx);
                Loadout high = adapt(orig, p.high);
                Loadout low = adapt(orig, p.low);

                int winHigh = 0;
                int draw = 0;
                for (int seed = 1; seed <= runs; seed++) {
                    BattleResult r = sim.run(high, low, seed);
                    if (r.winner() == 0) winHigh++;
                    else if (r.isDraw()) draw++;
                }

                double wr = (100.0 * winHigh + 50.0 * draw) / runs;
                rowSum += wr;
                colSums[pIdx] += wr;
                sb.append(String.format(Locale.ROOT, "%10.1f%%", wr));
            }

            double rowAvg = rowSum / pairs.size();
            sb.append(String.format(Locale.ROOT, "%10.1f%%%n", rowAvg));
        }

        sb.append("-".repeat(colWidth + 11 * (pairs.size() + 1))).append("\n");
        sb.append(String.format("%-" + colWidth + "s", "Average"));
        double totAvg = 0;
        for (int pIdx = 0; pIdx < pairs.size(); pIdx++) {
            double cAvg = colSums[pIdx] / originals.size();
            totAvg += cAvg;
            sb.append(String.format(Locale.ROOT, "%10.1f%%", cAvg));
        }
        totAvg /= pairs.size();
        sb.append(String.format(Locale.ROOT, "%10.1f%%%n", totAvg));
        sb.append(String.format("(same hero level=10 on both sides, common rarity, runs=%d)%n", runs));

        System.out.print(sb);
    }

    private static Loadout adapt(Loadout orig, int tier) {
        Loadout ld = new Loadout();
        ld.name = orig.name;
        ld.sect = orig.sect;
        ld.tamPhapLevel = 1;
        ld.level = 10;
        ld.cols = orig.cols;
        ld.rows = orig.rows;
        for (Loadout.PlacedItem it : orig.items) {
            Loadout.PlacedItem ni = new Loadout.PlacedItem();
            ni.id = it.id;
            ni.x = it.x;
            ni.y = it.y;
            ni.tier = tier;
            ni.rarity = Rarity.COMMON;
            ld.items.add(ni);
        }
        return ld;
    }
}
